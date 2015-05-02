using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CEP.Common.Events;
using com.espertech.esper.client;
using log4net;
using CEP.Common.Utils;
using System.Security.Permissions;
using System.IO;
using System.Threading;
using System.Xml;

namespace CEP.Server
{

    
    class EventProcessor
    {
        private string directoryPath = "C:\\Integration\\Input";
        public static string inlineRule;
        public static bool startListen = true;
        private CEP.Server.Services.StatementManagerService statementManager = new CEP.Server.Services.StatementManagerService();
        private static Dictionary<string, string> processesData = new Dictionary<string, string>();

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static EPServiceProvider epService = EPServiceProviderManager.GetDefaultProvider();

        public EventProcessor()
        {
            this.advertiseEventTypes();
            this.listenForInputDirectory();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void listenForInputDirectory()
        {
            // Create a new FileSystemWatcher and set its properties.
            System.IO.FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = this.directoryPath;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.xml";

            // Add event handlers.            
            watcher.Created += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // start read file
            Thread.Sleep(1000);
            string xmlText = System.IO.File.ReadAllText(e.FullPath);
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlText);
            XmlNode parentRule = xDoc.SelectSingleNode("/monitoringRules/rule");
            if (startListen) 
            {
                inlineRule = createEPLStatementsRec(parentRule);
                Console.WriteLine(inlineRule);
            }            
        }

        private string createEPLStatementsRec(XmlNode rule)
        {
            XmlNode LHSNode = null, RHSNode = null;
            string lhsRuleName = null, rhsRuleName = null, ruleType = null, operation = "";


            if (rule.FirstChild.Name == "LHS")
            {
                LHSNode = rule.FirstChild;
            }

            if (rule.FirstChild.Name == "RHS")
            {
                RHSNode = rule.FirstChild;
            }

            if (rule.LastChild.Name == "LHS")
            {
                LHSNode = rule.LastChild;
            }

            if (rule.LastChild.Name == "RHS")
            {
                RHSNode = rule.LastChild;
            }
            // get rule attributes
            Dictionary<string, string> prams = parseRuleParams(rule);
            prams["type"] = prams["type"].Replace(" ", "");                

            // base case has LHS/operand and/or RHS/operand
            if (LHSNode != null && LHSNode.FirstChild.Name == "operand" && RHSNode != null && RHSNode.FirstChild.Name == "operand")
            {
                XmlNode lhsOperand = LHSNode.FirstChild;
                XmlNode rhsOperand = RHSNode.FirstChild;
                operation = lhsOperand.Attributes["name"].Value + "@" + prams["type"] + "@" + rhsOperand.Attributes["name"].Value;
                statementManager.CreateXMLStatement(operation, LHSNode, RHSNode, prams);
                return "(" + operation + ")";
            }
            if (LHSNode != null && RHSNode == null && LHSNode.FirstChild.Name == "operand")
            {
                XmlNode lhsOperand = LHSNode.FirstChild;
                operation = prams["type"] + "@" + lhsOperand.Attributes["name"].Value;
                statementManager.CreateXMLStatement(operation, LHSNode, RHSNode, prams);
                return "(" + operation + ")";
            }
                
            // recursive call 

            // LHS
            if (LHSNode != null)
            {

                if (LHSNode.FirstChild.Name == "rule")
                {
                    lhsRuleName = createEPLStatementsRec(LHSNode.FirstChild);
                    
                    if (RHSNode != null && RHSNode.FirstChild.Name == "rule")
                    {
                        // LHS rule & RHS rule 
                        rhsRuleName = createEPLStatementsRec(RHSNode.FirstChild);
                        return "(" + lhsRuleName + "@" + prams["type"] + "@" + rhsRuleName + ")";

                    }
                    //else if (RHSNode != null && RHSNode.FirstChild.Name == "operand")
                    //{
                    //    // TODO assume that the only valid rules in this case is "AND" and "OR"
                    //    // LHS rule & RHS operand
                    //    // treat RHS as exists rule                         
                    //    XmlNode rhsOperand = RHSNode.FirstChild;
                    //    statementManager.CreateXMLStatement(rhsOperand.Attributes["name"].Value, (XmlNode)null, RHSNode, prams);
                    //    return "(" + lhsRuleName + " " + ruleType.Replace(" ", "") + " " + rhsOperand.Attributes["name"].Value + ")";
                    //}
                }
                //else if (LHSNode.FirstChild.Name == "operand")
                //{
                //    if (RHSNode != null && RHSNode.FirstChild.Name == "rule")
                //    {
                //        // TODO assume that the only valid rules in this case is "AND" and "OR"
                //        // LHS operand & RHS rule  
                //        XmlNode lhsOperand = LHSNode.FirstChild;
                //        statementManager.CreateXMLStatement(lhsOperand.Attributes["name"].Value, LHSNode, (XmlNode)null, prams);
                //        rhsRuleName = createEPLStatementsRec(RHSNode.FirstChild);
                //        return "(" + lhsOperand.Attributes["name"].Value + " " + ruleType.Replace(" ", "") + " " + rhsRuleName + ")";
                //    }
                //}
            }
            return null;
        }

        private Dictionary<string, string> parseRuleParams(XmlNode rule)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (XmlAttribute att in rule.Attributes)
            {
                res.Add(att.Name, att.Value);
            }
            return res;
        }



        private void advertiseEventTypes()
        {
            var configuration = epService.EPAdministrator.Configuration;

            //HACK: make EventTypes known manually, because NEsper does not seem to recognize classes in linked libraries
            var type = typeof(IEsperEvent);
            var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != type);

            foreach (var eventType in types)
            {
                Log.DebugFormat("Added EventType {0} as {1}", eventType.FullName, eventType.Name);
                configuration.AddEventType(eventType.Name, eventType);
            }
        }

        private EPStatement createStatement(string name, string expression)
        {
            Log.DebugFormat("Creating statement: {0}", expression);

            var statement = epService.EPAdministrator.CreateEPL(expression, name);
            statement.Events += defaultUpdateEventHandler;

            return statement;
        }

        public void CreateStatements()
        {

            var expr =
                "SELECT \n" +
                "    p.ProcessID as ProcessID \n" +
                ",   p.Name as ProcessName \n" +
                ",   t.TaskID as TaskID \n" +
                ",   t.ProcessID as TaskParentID \n" +
                ",   t.Name as TaskName \n" +
                ",   t.Task as TaskType \n" +
                ",   t.TaskActor as TaskActor \n" +
                "FROM ProcessEvent.win:time(10 sec) as p full outer join \n" +
                "     TaskEvent.win:time(10 sec) as t on p.ProcessID = t.ProcessID";
            // the "_Hide_" to hide this statment from the statment manager
            createStatement("_Hide_MonitoreAllEventChanges_UsedForSimulation", expr);


    // AND
    var and =
    @"
    SELECT 
        P.ProcessID as ProcessID    
    FROM ProcessComplete.win:keepall() as P 
    WHERE  
        NOT EXISTS (select * from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
        OR  
        NOT EXISTS (select * from TaskComplete.win:keepall() as TB where P.ProcessID = TB.ProcessID and cast(TB.Task, string) = 'B') 
    ";
            createStatement("AND: Task(A) And Task(B) Must Complete", and);

    // OR
    var or =
    @"
    SELECT 
        P.ProcessID as ProcessID    
    FROM ProcessComplete.win:keepall() as P 
    WHERE  
        NOT EXISTS (select * from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
        AND  
        NOT EXISTS (select * from TaskComplete.win:keepall() as TB where P.ProcessID = TB.ProcessID and cast(TB.Task, string) = 'B') 
    ";
            createStatement("OR: Task(A) OR Task(B) Must Complete", or);

    // Exists
    var Exists =
    @"
    SELECT 
        P.ProcessID as ProcessID    
    FROM ProcessComplete.win:keepall() as P 
    WHERE  
        NOT EXISTS (select * from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    ";
            createStatement("Exists: Task(A) Must Complete", Exists);


    // Exists_exactly
    var Exists_exactly =
    @"
    SELECT 
        T.ProcessID as ProcessID    
    FROM TaskStart(cast(Task, string) = 'A').win:keepall() as T 
    WHERE  
        2 = (select count(*) from TaskComplete.win:keepall() as TC where T.ProcessID = TC.ProcessID and cast(TC.Task, string) = 'A') 
    ";
    createStatement("Exists(exactly(n)): Task(A) Must Complete exactly two times", Exists_exactly);

    // Exists_least
    var Exists_least =
    @"
    SELECT 
        P.ProcessID as ProcessID    
    FROM ProcessComplete.win:keepall() as P 
    WHERE  
        2 < (select count(*) from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    ";
    createStatement("Exists(least(n)): Task(A) Must Complete at least two times", Exists_least);

    // Exists_most
    var Exists_most =
    @"
    SELECT 
        T.ProcessID as ProcessID    
    FROM TaskStart(cast(Task, string) = 'A').win:keepall() as T 
    WHERE  
        2 > (select count(*) from TaskComplete.win:keepall() as TA where T.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    ";
    createStatement("Exists(most(n)): Task(A) Must Complete at most two times", Exists_most);

    // Exists_between
    var Exists_between =
    @"
    SELECT 
        T.ProcessID as ProcessID    
    FROM TaskStart(cast(Task, string) = 'A').win:keepall() as T 
    WHERE  
        1 >= (select count(*) from TaskComplete.win:keepall() as TA where T.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    AND 
        3 <= (select count(*) from TaskComplete.win:keepall() as TA where T.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    ";
    createStatement("Exists(between(n,m)): Task(A) Must Complete between 1 time and 3 times", Exists_between);


    // Absence
    var Absence =
    @"
    SELECT 
        P.ProcessID as ProcessID    
    FROM ProcessComplete.win:keepall() as P 
    WHERE  
        EXISTS (select count(*) from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = 'A') 
    ";
    createStatement("Absence: Task(A) Must Not Complete", Absence);

    // _AFollowedByB
    var _AFollowedByB =
    @"
    INSERT INTO _AFollowedByB 
        SELECT 
            A.ProcessID as ProcessID
           ,A.TaskID as ATaskID
           ,B.TaskID as BTaskID
        FROM PATTERN [ every            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
            ]
    ";
    createStatement("_AFollowedByB: A followed by B helper", _AFollowedByB);

    // _AFollowedByC
    var _AFollowedByC =
    @"
    INSERT INTO _AFollowedByC
        SELECT 
            A.ProcessID as ProcessID
           ,A.TaskID as ATaskID
           ,C.TaskID as CTaskID
        FROM PATTERN [ every            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                C = TaskComplete(C.ProcessID = A.ProcessID and cast(C.Task, string) = 'C')
            ]
    ";
    createStatement("_AFollowedByC: A followed by C helper", _AFollowedByC);

    // _AFollowedByBAndNotC
    var _AFollowedByBAndNotC =
    @"
    INSERT INTO _AFollowedByBAndNotC
        SELECT 
            A.ProcessID as ProcessID
           ,A.TaskID as ATaskID
           ,B.TaskID as BTaskID
        FROM PATTERN [ every            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                (
                    B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                and not 
                    C = TaskComplete(C.ProcessID = A.ProcessID and cast(C.Task, string) = 'C')
                )
            ]
    ";
    createStatement("_AFollowedByBAndNotC: A followed by B with C absence helper", _AFollowedByBAndNotC);

    // Sequence
    var Sequence =
    @"    
    SELECT P.ProcessID
    FROM ProcessComplete.win:keepall() as P 
         LEFT OUTER JOIN _AFollowedByB.win:keepall() as T 
         on P.ProcessID = T.ProcessID
    WHERE T is null
    ";
    createStatement("Sequence(A,B): Tasks A and B Must Complete in sequence", Sequence);

//    // ResponseV1
//    var ResponseV1 =
//    @"    
//    SELECT P.ProcessID
//    FROM ProcessComplete.win:keepall() as P 
//         LEFT OUTER JOIN _AFollowedByB.win:keepall() as AfB
//         on P.ProcessID = AfB.ProcessID
//         LEFT OUTER JOIN TaskComplete(cast(Task, string) = 'A').win:keepall() as A
//         on P.ProcessID = A.ProcessID
//    WHERE AfB is null and A is not null 
//    ";
//    createStatement("ResponseV1(A,B): Tasks A observed but not Task B ", ResponseV1);

    // ResponseV2
    var ResponseV2 =
    @"    
    SELECT 
        A.ProcessID as ProcessID 
    FROM PATTERN [ 
            every(            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                (
                    P = ProcessComplete(P.ProcessID = A.ProcessID)
                    and not 
                    B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                )
            )
        ]
    ";
    createStatement("ResponseV2(A,B): Tasks A observed but not Task B ", ResponseV2);

    // ResponseWithAbsence
    var ResponseWithAbsence =
    @"    
    SELECT 
        A.ProcessID as ProcessID 
    FROM PATTERN [ 
            every(            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                (
                    (
                         C = TaskComplete(C.ProcessID = A.ProcessID and cast(C.Task, string) = 'C')
                            and not 
                         B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                    )
                    or
                    (
                         P = ProcessComplete(P.ProcessID = A.ProcessID)
                            and not 
                         B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                    )
                )
            )
        ]
    ";
    createStatement("ResponseWithAbsence(A,B, Absence(C)): Tasks A observed but not Task B Or C observer within the scope", ResponseWithAbsence);


    // ResponseWithAbsence
    var ResponseWithAbsenceAndTimeSpan =
    @"    
    SELECT 
        A.ProcessID as ProcessID 
    FROM PATTERN [ 
            every(            
                A = TaskComplete(cast(A.Task, string) = 'A')
                ->
                (
                    (
                         C = TaskComplete(C.ProcessID = A.ProcessID and cast(C.Task, string) = 'C')
                            and not 
                         B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                    )
                    or
                    (
                         P = ProcessComplete(P.ProcessID = A.ProcessID)
                            and not 
                         B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')
                    )
                    or 
                    (
                        timer:interval(2 days) // Time span 
                            and not 
                        B = TaskComplete(B.ProcessID = A.ProcessID and cast(B.Task, string) = 'B')    
                    ) 
                )
            )
        ]
    ";
    createStatement("ResponseWithAbsenceAndTimeSpan(A,B, Absence(C),TimeSpan(60sec)): Tasks A observed but not Task B within (2 days) Or C observer within the scope", ResponseWithAbsenceAndTimeSpan);


  var RulesDescription = @"

    Exists pattern: is a unary pattern that requires a certain event to occur within the process 
    instance. Multiplicity defines how many times the rule is required to hold. For instance, it 
    might be required to execute a task for certain number of times within the same case. The 
    multiplicity constraint can be on the form exactly(n), at least(n), at most(n), or between(n;m).
    A special case of that pattern is the Absence pattern. 

Sequence pattern: and next ones are binary patterns. The sequence pattern requires that both
its input events occur and in sequence, sequence(E1;E2) means that E1 must occur and then E2 
occurs afterwords. A special case of that pattern is the Next pattern where E2 has to be the 
first event to occur, after occurrence of E1, on the event stream with respect to the same 
process instance. Precedes pattern, precedes(E1;E2), requires that if event E2 is observed 
on the stream for some process instance, then event E1 must have been observed before for 
the same process instance. A special case of precedes pattern is the one to one precedence, 
precedes one to one(E1;E2), where no two consecutive occurrences of E2 is allowed without 
having E1 in between. 

Response pattern: response(E1;E2), requires that if event E1 is observed for some process 
instance then E2 must be observed before the instance completes its execution. 

The sequence, precedes and response patterns share the ability to define the 'with absence', 
'time span' and 'alert time span' properties. 

// every (timer:interval(10 sec) and not A)

The with absence attribute might refer to a set of events that are required to be absent 
between E1 and E2. The time span property requires that the consequent event of the pattern must take place within a time window of the
pattern condition event. The time span condition is on the form timestamp(E1=E2)timestamp
where timestamp(E) is a function that returns the time stamp of the input event and  is a
comparison operator. The timestamp is a time value that can be either absolute or computed by
relevance to some other event or process data. For example, in Fig. 4, it might be required that
the employee receives the approved travel details at most two days before the trip date. The alert
time span usually provides a shorter period than the time span in which if the consequent event
didn't take place this signals a possibility of violation. For the example in hand, the compliance
requirement could say 'If the travel request has been approved, then the employee must receive
the reservation data at most two days before the travel date.'. In that case the compliance rule
will be:
Response(operand1 : completed(act2); operand2 : (completed(act5)); condition :
Request:status = approved; scope : (started(case); completed(case));with absence : none;
time span : timestamp(completed(act5)  Request:travelDate 􀀀 2Days); alert time span :
timestamp(completed(act5)  Request:travelDate 􀀀 3Days))

The Until pattern is used in the sense of blocking some events until some other event has
occurred. With reference to Fig. 4, it is not allowed to execute the task \Make a 
ight and
hotel reservations' until \Process Travel request' has been executed. Note that the occurrence
of 'Process Travel Request\ has released the blocking of \Make reservation\. This does not
necessarily mean that 'Make Reservations' has to occur. Rather, it is no longer blocked and
from that point on, it is allowed to execute. A special case of that pattern is the Weak Until in
which the blocking event may never occur2.
The Segregation of Duty pattern captures the famous four eyes principle where it is required
that two tasks are performed by two dierent resources within the same process instance.
[(ToDo) [We need to discuss the variants of that pattern when we come to moni-
toring]]. A special case of that pattern is the bind of duty where it requires the opposite. We
have dened it as a special case because it requires only the negation of the checking condition
to monitor rules instantiated from that pattern. Finally, the Performed By Role pattern and
its special case Performed By Resource describe the role-based allocation and direct allocation
resource patterns [44] which are relevant to compliance monitoring.
Composite patterns are used to logically connect other patterns by the connectors AND, OR,
or NOT. This is used to dene complex rules when a process model is required to comply with
more than one atomic rule that are not necessarily are all required to hold. This is especially
helpful when sub-ideal level of compliance is also needed [28,30] . For instance, we could have a
composite rule on the form R1OR(NOT(R1) ! R2) which means that ideally process instances
should comply with rule R1 but if this is not the case, R2 should be fullled then.


-------------------------------------------
Exists Anti Patterns:

The Exists pattern requires that the completed task event,
completed(t; i) occurs a certain number of times within a certain scope in the process
instance and/or under a specific data condition. Basically, a violation to this rule occurs
when the multiplicity constraint is no longer satisfied within the specified scope. To
track this, two queries are generated. The first query is triggered by the occurrence of the
rules antecedent event, e.g., completed(t; i). When this event is detected on the event
stream , the history of case i (i; scope start; completed(t; i)) is projected. Then,
occurrences of the event completed(t; i) are counted and the multiplicity constraint
is evaluated. The evaluation is not just a comparison between the number of actual
occurrences of the event and the multiplicity range. Rather, it takes into consideration
possible future occurrences of the event. For instance, if the rule requires that task A
to execute exactly three times, then with the first occurrence of completed(A; i) the
multiplicity constraint is not satisfied. However, we cannot conclude at that point that
this is a violation as there might be future occurrences of completed(A; i). On the
other hand, if this is the fourth occurrence of the event completed(A; i) then this is
definitely a violation and in this case, the query generates a rule violation event on
the stream. The second query is triggered by the observation of the event scope end
which signals the completion of the monitoring scope. Similar to the first case, the
process instance history (i; scope start; scope end) is projected and the number of
occurrences of compeleted(t; i) is counted and the multiplicity constraint is evaluated.
The evaluation in this case is comparing the actual occurrences to the range specified
in rule’s multiplicity. If the evaluation fails, a rule violation event is generated on the
stream. In any case, the rule violation event contains the set of all completed(t; i) as its
root cause RC explanation.
To predict violations of the Exists pattern, a query is defined to count the number
of occurrences of the created(t; i) event on the stream within the monitoring scope.
Similar to the queries above, each time such triggering event is detected, the number of
completed(t; i) is counted and multiplicity is checked. If multiplicity is not satisfied, a
warning rule violation event is thrown to the stream. Event created(t; i) is the earliest
point from which a violation can be avoided, see Fig. 1.


--------------------------------
Response Anti Patterns:

The Response pattern can be violated in the following cases:
– The rule’s antecedent occurred but never the consequent within the monitoring
scope and time span, if defined,
– The rule’s antecedent occurred and any of the forbidden events with absence occurs
before the rule’s consequent,
– There is a possible violation if the alert time span elapses before the occurrence of
the consequent event.
To detect time time span-related violations, we depend on timer services. We define
a query, we call it the triggering query, whose responsibility is to initiate timers and
register other monitoring queries whenever the rule’s antecedent is observed within
scope on the stream and data conditions are satisfied. We have basically two timers one
for the alert time span and the other for time span. Whenever these timers fire, they
through respective rule violation events on the stream. The anti pattern queries work
to detect violation related to the first two cases above. There are two queries, the noresponse
query and the show-up query. The no-response query monitors the scope end
occurrence on the stream. The show-up query monitors the occurrence of any of the
events in the with absence property of the rule.
It might be the case that the rule’s consequent occurs before the time span elapses,
before scope end and before any of the forbidden events occurs. In this case execution
is compliant and the timers have to be switched off and the anti pattern queries have
to be un-registered. For this, a query is defined, the compliance query, that detects the
consequent event and switches off those timers and un-registers the queries.
For the example rule we discussed in Section 4, Whenever an event completed(
act2; i) is observed on the stream, the triggering query, evaluates the time span conditions
of the rule. That is, the time span timer is set to fire two days before the travel date
whereas the alert timer is set to fire three days before that date. Also, the anti pattern and
the compliance queries are registered. Now, if alert time span timer fires, a rule violation
event is thrown on the stream and is detected by a global rule violation detection query.
Note that this is a warning alert as actually no violation occurred yet. In that case, the
action of the rule was to alert for the possible violation. If no event completed(act5; i)
is detected on the stream, eventually the time span timer will fire throwing another violation
event that signals the actual violation. In another case, the scope end could be
observed on the stream even before alert timers fire. Here, the anti pattern query will
be triggered and will through a rule violation event on the stream. Note that in the latter
case, the cause of the violation is different from the former case where time span
elapsed. In a third case, if compeleted(act5; i) is observed on the stream, the compliance
query is triggered and as an action it un-registers the anti pattern queries and
switches off the active timers.

";






//var expr =@"
//// AND (E(t1), E(t2), ..., E(tn)) 
//// Task instances (E(t1), E(t2), ..., E(tn)) must all take place 
//SELECT 
//    a.TaskID FROM 
//    pattern 
//        [every a = TaskEvent(Name = 'TaskResourceAllocationkEvent')  
//        ->  
//        (TaskEvent(TaskID = a.TaskID) and not TaskEvent(Name != 'TaskStart'))]
//";

//createStatement("TaskStartIsMissingAfterResourceAllocation", expr);


//            var r = @"select withdraw.accountNumber as accntNum, withdraw.amount as amount         
//                      from WithdrawalEvent.win:time(60 sec) as withdraw
//                      left outer join
//                      LoginEvent.win:time(60 sec) as login
//                          on login.accountNumber = withdraw.accountNumber
//                      where login.accountNumber is null";

            //{
            //    var expr =
            //        "SELECT \n" +
            //        "    TaskID  \n" +
            //        "FROM pattern [every TaskResourceAllocationkEvent -> (timer:interval(60 sec) and not TaskStart)]";

            //    createStatement("TaskStartIsMissingAfterResourceAllocationBy60Sec", expr);
            //}

            //{
            //    var expr =
            //        "// TaskResourceAllocation Must be followed by TaskStart \n" +
            //        "SELECT " +
            //        "   a.TaskID as For_TaskID \n" +
            //        ",  a.Name as Event  \n" +
            //        ",  b.Name as must_Followed_By_Task_SATRT_not\n" +
            //        "    \n" +
            //        "FROM \n" +
            //        "   pattern  \n" +
            //        "      [every a = TaskResourceAllocation  \n" +
            //        "      ->  \n" +
            //        "      (b = TaskEvent(b.TaskID = a.TaskID) and not c = TaskStart(TaskID = a.TaskID))] \n";

            //    createStatement("TransitionMustFollowedBy", expr);
            //}



            //var e =
            //"select ProcessID  " +
            //"from pattern " +
            //    "[every a = {0} -> ({0}(Operation = a.Operation and Symbol = a.Symbol) " +
            //    "and not " +
            //    "{0}(Operation!=a.Operation or Symbol != a.Symbol)) ] ";


        }

        private void defaultUpdateEventHandler(object sender, UpdateEventArgs e)
        {
            var ev = (e.NewEvents.FirstOrDefault().Underlying as Dictionary<string, object>);

            // save processes name and id 
            object processId, processName;
            if (ev.TryGetValue("ProcessID", out processId) && ev.TryGetValue("ProcessName", out processName))
            {
                if (processId != null && processName != null && !processesData.ContainsKey(processId.ToString()))
                    processesData.Add(processId.ToString(), processName.ToString());
            }
            // fill missing data
            if ((!ev.TryGetValue("ProcessID", out processId) || processId == null) && ev.ContainsKey("TaskParentID"))
            {

                ev["ProcessID"] = ev["TaskParentID"].ToString();
                ev["ProcessName"] = processesData[ev["TaskParentID"].ToString()];
            }
            
            Log.Info("An Event (" + e.Statement.Name + ") occured: " + ev.ToDebugString());
        }
    }
}
