using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP.Server
{
    public static class EPLTemplates
    {

        // parameters [ScopeStart,ScopeEnd,st , se, TaskName, minOccurs, Antecedent, RuleID]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5- TaskName: This is the name of the task that the rule is monitoring. For example the rule would say that completed(C) must occur at least 2 times. TaskName is C in this case.
        //6- MinOccurs: This is the lower bound that the monitored event of a certain task must occur more than the minOccurs.
        //7- Antecedent: This defines the event type for TaskName that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //9- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}
        
        //[DONE] ToDo [Ahmed Awad]: This method is no longer correct and it must be removed and all its references are changed to refer to either the below
        // min occurrences or above max occurrences methods below.
//        public static string exists(string TaskName)
//        {
//            Guid id = Guid.NewGuid();
//            var anti_pattern_1_below_min_occurrences_rule_antecedent = @"
//                SELECT P.ProcessID as ProcessID    
//                FROM ProcessComplete.win:keepall() as P 
//                WHERE NOT EXISTS (select * from TaskComplete.win:keepall() as TA where P.ProcessID = TA.ProcessID and cast(TA.Task, string) = '{0}') ";

//            return string.Format(anti_pattern_1_below_min_occurrences_rule_antecedent, TaskName);
//        }
        
        
        // parameters [ScopeStart,ScopeEnd,st , se, TaskName, minOccurs, Antecedent, RuleID]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5- TaskName: This is the name of the task that the rule is monitoring. For example the rule would say that completed(C) must occur at least 2 times. TaskName is C in this case.
        //6- MinOccurs: This is the lower bound that the monitored event of a certain task must occur more than the minOccurs.
        //7- Antecedent: This defines the event type for TaskName that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //9- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string below_min_occurrences(string ScopeStart, string ScopeEnd, string st, string se, string TaskName, int minOccurs, string Antecedent)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "below_min_occurrences";
            string RuleID = id.ToString();
            // [DONE] ToDo [Ahmed Awad]: See the RuleViolationEvent in the common project.
            var anti_pattern_1_below_min_occurrences_rule_antecedent = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {6}({4}) occurred less than {5} within {0}({2}) and {1}({3}) in process ', '{7}','{8}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                                e = {1}( cast(e.Task, string) = '{3}' , ProcessID = s.ProcessID ) 
                                ))] as scope 
                                WHERE {5} > ( select count (*) from {6}.win:keepall() as T 
                                WHERE cast(T.Task, string) = '{4}' and (T.TimeStamp between scope.s.TimeStamp and scope.e.TimeStamp ) )";


            return string.Format(anti_pattern_1_below_min_occurrences_rule_antecedent, ScopeStart, ScopeEnd, st, se, TaskName, minOccurs, Antecedent, RuleID, RuleType);
        }
        public static string above_max_occurrences_exists(string ScopeStart, string ScopeEnd, string st, string se, string TaskName, int minOccurs, string Antecedent)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "below_min_occurrences";
            string RuleID = id.ToString();
            // [DONE] ToDo [Ahmed Awad]: See the RuleViolationEvent in the common project.
            var anti_pattern_1_below_min_occurrences_rule_antecedent = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {6}({4}) occurred less than {5} within {0}({2}) and {1}({3}) in process ', '{7}','{8}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                                e = {1}( cast(e.Task, string) = '{3}' , ProcessID = s.ProcessID ) 
                                ))] as scope 
                                WHERE {5} < ( select count (*) from {6}.win:keepall() as T 
                                WHERE cast(T.Task, string) = '{4}' and (T.TimeStamp between scope.s.TimeStamp and scope.e.TimeStamp ) )";


            return string.Format(anti_pattern_1_below_min_occurrences_rule_antecedent, ScopeStart, ScopeEnd, st, se, TaskName, minOccurs, Antecedent, RuleID, RuleType);
        }
        // parameters [ScopeStart,ScopeEnd,st , se, TaskName, maxOccurs, Antecedent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskName: This is the name of the task that the rule is monitoring. For example the rule would say that completed(C) must occur at least 2 times. TaskName is C in this case.
        //6- MaxOccurs: This is the lower bound that the monitored event of a certain task must occur more than the minOccurs.
        //7-Antecedent: This defines the event type for TaskName that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //9-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}
        public static string absence_anti_pattern(string ScopeStart, string ScopeEnd, string st, string se, string TaskName, int maxOccurs, string Antecedent)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "above_max_occurrences";
            string RuleID = id.ToString();
            var anti_pattern_2_above_max_occurrences_rule_antecedent = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {6}({4}) occurred less than {5} within {0}({2}) and {1}({3}) in process ', '{7}','{8}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            /*every*/( 
                                d = {6}( cast(d.Task, string) = '{4}', ProcessID = s.ProcessID )
                                and not e = {1}( cast(e.Task, string) = '{3}' , ProcessID = s.ProcessID ) 
                                )))] as scope 
                                /*WHERE {5} < ( select count (*) from {6}.win:keepall() as T 
                                WHERE cast(T.Task, string) = '{4}' and TimeStamp > scope.s.TimeStamp )*/";

            return string.Format(anti_pattern_2_above_max_occurrences_rule_antecedent, ScopeStart, ScopeEnd, st, se, TaskName, maxOccurs, Antecedent, RuleID, RuleType);
        }

        // parameters [ScopeStart,ScopeEnd,st , se, TaskNameAntecedent,TaskNameConsequent,WA, Antecedent,Consequent,Absent,timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1-  ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2-  ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3-  st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4-  se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-  TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6-  TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-  WA: This is a set of task names. The purpose is to make sure that non of the elements of that set has occurred between Antcedent and Consequent. Note how that shall be implemented in the EPL statement below.
        //8-  Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //9-  Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //10- Absent: This defines the event type for tasks in WA that needs to absent. If a rule is on the form Sequence(Completed(A),Completed(D),withAbsence={Completed(B),Completed(C)}), then Absent is bound to the event type Completed where WA = {B,C}
        //11- TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //12- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //13- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string sequence_and_response(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string WA, string Antecedent, string Consequent, string Absent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "sequence_and_response";
            string RuleID = id.ToString();

            var anti_pattern_3_to_detect_sequence_and_response_violations = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {7}({4}) was not followed by {8}({5}) within {0}({2}) and {1}({3}) in process ', '{11}','{12}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Antecedent = {7}( cast(Antecedent.Task, string) = '{4}', ProcessID = s.ProcessID )
                                ->(
                                    e = {1}( cast(e.Task, string) = '{3}' , ProcessID = s.ProcessID )" +
                                    (!String.IsNullOrEmpty(Absent) && !String.IsNullOrEmpty(WA) ? " or absent = {9}( cast(absent.Task, string) in ({6}), ProcessID = s.ProcessID )" : "") +
                                    (!String.IsNullOrEmpty(timeSpan) ? " or timer:interval ({10})" : "") +
                                  ") and not Consequent = {8} ( cast(Consequent.Task, string) = '{5}' , ProcessID = s.ProcessID )"+
                        ")))]";

            return string.Format(anti_pattern_3_to_detect_sequence_and_response_violations, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, WA, Antecedent, Consequent, Absent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart,ScopeEnd,st , se, TaskNameAntecedent,TaskNameConsequent,Antecedent,Consequent,timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5- TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7- Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //9- TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //10- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //11- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string response_when_isBefore_equals_false(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "response_when_isBefore_equals_false";
            string RuleID = id.ToString();

            var anti_pattern_4_to_detect_response_violation_when_isBefore_equals_false = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {6}({4}) followed by {7}({5}) within {0}({2}) and {1}({3}) was violated in process but before {8} ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Antecedent = {6}( cast(Antecedent.Task, string) = '{4}', ProcessID = s.ProcessID )
                                ->(
                                    e = {7}( cast(e.Task, string) = '{5}' , ProcessID = s.ProcessID)" +
                                   (!String.IsNullOrEmpty(timeSpan) ? "and not timer : interval ( {8} )" : "")+ 
                                  "))))]";

            return string.Format(anti_pattern_4_to_detect_response_violation_when_isBefore_equals_false, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }


        // parameters [ScopeStart,ScopeEnd,st , se, TaskNameAntecedent,TaskNameConsequent,Antecedent,Consequent,timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5- TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7- Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //9- TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //10- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //11- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string one_to_one_response(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "one_to_one_response";
            string RuleID = id.ToString();

            var anti_pattern_5_to_detect_one_to_one_response_violation = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'One to one response violation. Successive occurrences of Event {6}({4}) were detected without detecting Event {7}({5}) in between within {0}({2}) and {1}({3}) was violated in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Antecedent = {6}( cast(Antecedent.Task, string) = '{4}' , ProcessID = s.ProcessID )
                                ->(
                                    Antecedent2 = {6}( cast(Antecedent2.Task, string) = '{4}' , ProcessID = s.ProcessID )
                                    and not Consequent={7}( cast(Consequent.Task, string)='{5}' , ProcessID = s.ProcessID)
                                  )                                          
                            )))]";

            return string.Format(anti_pattern_5_to_detect_one_to_one_response_violation, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }


        // parameters [ScopeStart,ScopeEnd,st , se, TaskNameAntecedent,TaskNameConsequent,Antecedent,Consequent,timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5- TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7- Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8- Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //9- TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //10- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //11- RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string precedes_where_consequent_never_occurred(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "precedes_violation_where_consequent_never_occurred";
            string RuleID = id.ToString();

            var anti_pattern_6_to_detect_precedes_violation_where_consequent_never_occurred = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Precedes Rule violation: Event {7}({5}) never occurred before {6}({4}) within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                             (  not  Consequent = {7}( cast(Consequent.Task, string) in ('{3}','{5}'), ProcessID = s.ProcessID)) until
                                Antecedent = {6}( cast(Antecedent.Task, string) = '{4}' , ProcessID = s.ProcessID )
                                        
                            ))]";


            return string.Format(anti_pattern_6_to_detect_precedes_violation_where_consequent_never_occurred, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart, ScopeEnd, st ,se, TaskNameAntecedent, TaskNameConsequent, WA, Antecedent, Consequent, Absent, timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7- WA: This is a set of task names. The purpose is to make sure that non of the elements of that set has occurred between Antcedent and Consequent. Note how that shall be implemented in the EPL statement below.
        //8-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //9-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //10-Absent: This defines the event type for tasks in WA that needs to absent. If a rule is on the form Sequence(Completed(A),Completed(D),withAbsence={Completed(B),Completed(C)}), then Absent is bound to the event type Completed where WA = {B,C}
        //11-TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //12- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //13-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string precedes_where_forbidden_or_timer(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string WA, string Antecedent, string Consequent, string Absent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "precedes_violation_where_forbidden_or_timer";
            string RuleID = id.ToString();

            var anti_pattern_7_to_detect_precedes_violation_where_forbidden_or_timer = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {7}({5}) is observed and either {10} or one of the tasks in {8} were observed and then  {6}({4}) within {0}({2}) and {1}({3}) in process ', '{11}','{12}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                     Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID)                                     
                                    ->(    
                                        e = {1}( cast(e.Task, string) = '{3}', ProcessID = s.ProcessID )" +
                                        ((!String.IsNullOrEmpty(Absent) && !String.IsNullOrEmpty(WA)) ? "or absent = {9}( cast(absent.Task, string) in ({8}), ProcessID = s.ProcessID )" : "") +                                                                        
                                        (!String.IsNullOrEmpty(timeSpan) ? "or timer : interval ( {10} )" : "") +
                                        ")->( Antecedent = {6}( cast(Antecedent.Task, string) = '{4}' , ProcessID = s.ProcessID))" +
                                   ")))]";


            return string.Format(anti_pattern_7_to_detect_precedes_violation_where_forbidden_or_timer, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, WA, Absent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart, ScopeEnd, st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the event type Completed
        //9-TimeSpan: defines the time period within which Consequent(TaskNameConsequent) has to be observed after Antecedent(TaskNameAntecedent). So, a rule could say B must occur after A in no more than 3 days. We have to check how does ESPER  define time spans for example how can we express 3 days in an EPL statement.
        //10- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //11-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string precedence_when_isBefore_equals_false(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "precedence_violation_when_isBefore_equals_false";
            string RuleID = id.ToString();

            var anti_pattern_8_to_detect_precedence_violation_when_isBefore_equals_false = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {7}({5}) occurred before {6}({4}) within {0}({2}) and {1}({3}) but sooner that the time span {8} in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                     Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID)
                                     and not 
                                     e = {1} ( cast(e.Task, string)='{3}' , ProcessID = s.ProcessID )
                                    ->(    
                                        Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID )" +
                                        (!String.IsNullOrEmpty(timeSpan) ? "and not timer : interval ( {8} )" : "") +
                                    "))))]";  

            return string.Format(anti_pattern_8_to_detect_precedence_violation_when_isBefore_equals_false, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }


        // parameters [ScopeStart, ScopeEnd,st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the ezvent type Completed
        //9- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //10-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string one_to_one_precedes(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "one_to_one_precedes";
            string RuleID = id.ToString();

            var anti_pattern_9_to_detect_one_to_one_precedes_violation = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Two or more occurrences  of Event {6}({4}) were detected without detecting Event {7}({5}) in between within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                               Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID )  
                                    ->((not
                                        Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID)) until    
                                        Antecedent2={6} ( cast(Antecedent2.Task,string)='{4}' , ProcessID = s.ProcessID)
                                         
                                    )                            
                                )
                            )
                        ) 
                    ] 
                ";  

            return string.Format(anti_pattern_9_to_detect_one_to_one_precedes_violation, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }


        // parameters [ScopeStart, ScopeEnd,st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the ezvent type Completed
        //9- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //10-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string separation_of_duty_violation_1(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "separation_of_duty_violation_1";
            string RuleID = id.ToString();

            var anti_pattern_10_separation_of_duty_violation_1 = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Events {6}({4}) and {7}({5}) were performed by Resource s.Antecedent.resource  within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                               Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID ) 
                                    -> every (    
                                       Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID ) 
                                    )                            
                                )
                            )
                        ) 
                    ] WHERE Consequent.Resource = Antecedent.Resource
                ";  

            return string.Format(anti_pattern_10_separation_of_duty_violation_1, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart, ScopeEnd,st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the ezvent type Completed
        //9- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //10-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string separation_of_duty_violation_2(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "separation_of_duty_violation_2";
            string RuleID = id.ToString();

            var anti_pattern_10_separation_of_duty_violation_2 = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Events {6}({4}) and {7}({5}) were performed by Resource s.Antecedent.resource  within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID ) 
                                    -> every (    
                                       Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID ) 
                                    )                            
                                )
                            )
                        ) 
                    ] WHERE Consequent.Resource = Antecedent.Resource 
                ";      

            return string.Format(anti_pattern_10_separation_of_duty_violation_2, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }


        // parameters [ScopeStart, ScopeEnd,st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the ezvent type Completed
        //9- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //10-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string bind_of_duty_violation_1(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "bind_of_duty_violation_1";
            string RuleID = id.ToString();

            var anti_pattern_11_bind_of_duty_violation_1 = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Events {6}({4}) and {7}({5}) were not performed by the same Resource within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID ) 
                                    -> every(    
                                        Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID )
                                    )                            
                                )
                            )
                        ) 
                    ] WHERE Consequent.Resource <> Antecedent.Resource
                ";

            return string.Format(anti_pattern_11_bind_of_duty_violation_1, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart, ScopeEnd,st ,se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6- TaskNameConsequent: This is the name of the task that the rule is monitoring as second part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameConsequent is D in this case.
        //7-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //8-Consequent: This defines the event type for TaskNameConsequent that needs to be monitored. As explained in point 5 Completed(D) the consequent in this case is the ezvent type Completed
        //9- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //10-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string bind_of_duty_violation_2(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string TaskNameConsequent, string Antecedent, string Consequent, string timeSpan)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "bind_of_duty_violation_2";
            string RuleID = id.ToString();
            var anti_pattern_11_bind_of_duty_violation_2 = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Events {6}({4}) and {7}({5}) were not performed by the same Resource within {0}({2}) and {1}({3}) in process ', '{9}','{10}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Consequent= {7} ( cast(Consequent.Task,string) ='{5}' , ProcessID = s.ProcessID ) 
                                    -> every (    
                                        Antecedent= {6} ( cast(Antecedent.Task,string)='{4}' , ProcessID = s.ProcessID )
                                    )                            
                                )
                            )
                        ) 
                    ] WHERE Consequent.Resource <> Antecedent.Resource
                ";


            return string.Format(anti_pattern_11_bind_of_duty_violation_2, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, TaskNameConsequent, Antecedent, Consequent, timeSpan, RuleID, RuleType);
        }

        // parameters [ScopeStart, ScopeEnd, st ,se, TaskNameAntecedent, Antecedent, Resource, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //7-Resource: This defines the resource that must perform the task. 
        //8- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //9-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string performed_by_resource_violation(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string Antecedent, string Resource)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "performed_by_resource_violation";
            string RuleID = id.ToString();
            var anti_pattern_12_performed_by_resource_violation = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {5}({4}) was not performed by {6} within {0}({2}) and {1}({3}) in process ', '{7}','{8}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                 Antecedent= {5} ( cast(Antecedent.Task, string)='{4}' , ProcessID = s.ProcessID )                                                              
                                )
                            )
                        ) 
                    ] WHERE '{6}' <> cast(Antecedent.Resource, string) 
                ";

            return string.Format(anti_pattern_12_performed_by_resource_violation, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, Antecedent, Resource, RuleID, RuleType);
        }



        // parameters [ScopeStart, ScopeEnd, st ,se, TaskNameAntecedent, Antecedent, Role, RuleID, RuleType]
        // Explanation of parameters
        //1- ScopeStart: this defines an event type to start the  monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //2- ScopeEnd: this defines an event type to end the monitoring of a rule violation. Event type is like started, completed, failed, etc.
        //3- st: This represents the task name for which the scope start event is monitored for example Completed(A) would signal the monitoring
        //4- se: This represents the task name for which the scope end event is monitored for example allocated(B) would signal the termination of the rule monitoring scope
        //5-TaskNameAntecedent: This is the name of the task that the rule is monitoring as first part. For example the rule would say that completed(C) is followed by Completed(D), TaskNameAntecedent is C in this case.
        //6-Antecedent: This defines the event type for TaskNameAntecedent that needs to be monitored. As explained in point 5 Completed(C) the antecedent in this case is the event type Completed
        //7-Role: This defines the Role of the resource that must perform the task. 
        //8- RuleID: This is a system genereted number to keep track of the rule for which this anti pattern query is created. Currently, you can use a GUID that you assign in the rule editor.
        //9-RuleType in this case it can be on of the following values {Exists, absence, sequence, next, precedes, one to one precedes, response, one to one response, SoD, BoD, performed by role, performed by resource}

        public static string performed_by_role_violation(string ScopeStart, string ScopeEnd, string st, string se, string TaskNameAntecedent, string Antecedent, string Role)
        {

            Guid id = Guid.NewGuid();
            string RuleType = "performed_by_role_violation";
            string RuleID = id.ToString();
            var anti_pattern_13_performed_by_role_violation = @"insert into RuleViolationEvent (processID, Message, RuleID, RuleType)
            select s.ProcessID, 'Event {5}({4}) was not performed by {6} within {0}({2}) and {1}({3}) in process ', '{7}','{8}'
                FROM PATTERN [ 
                        every(            
                            s = {0}( cast(s.Task, string) = '{2}' )
                            ->(
                            every( 
                                Antecedent= {5} ( cast(Antecedent.Task, string)='{4}' , ProcessID = s.ProcessID )                                                              
                                )
                            )
                        ) 
                    ] WHERE '{6}' <> cast(Antecedent.Role, string)
                ";


            return string.Format(anti_pattern_13_performed_by_role_violation, ScopeStart, ScopeEnd, st, se, TaskNameAntecedent, Antecedent, Role, RuleID, RuleType);
        }

        //[DONE] ToDo [Ahmed Awad]: I cleaned the old commented EPL statements as they are no longer needed.
    }
}
