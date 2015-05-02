using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CEP.Server.Adaptor;
using com.espertech.esper.client;
using log4net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CEP.Common.Utils;

namespace CEP.Server.Services
{
    class StatementManagerService : IStatementManagerService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        EPServiceProvider epService = EPServiceProviderManager.GetDefaultProvider();

        public StatementManagerService()
        {
            Log.Info("Created Service Instance of StatementManagerService");
        }
  
        public Boolean CreateStatement(string name, string statement)
        {
            if (epService.EPAdministrator.StatementNames.Contains(name))
            {
                return false;
            }
            try
            {
                var epStatement = epService.EPAdministrator.CreateEPL(statement, name);
                epStatement.Events += defaultUpdateEventHandler;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateXMLStatement(String name, XmlNode LHSRuleNode, XmlNode RHSRuleNode, Dictionary<string, string> prams)
        {
            try
            {
                if (prams["type"].ToLower() == "absence")
                {
                    prams["type"] = "exists";
                }
                TraverseRule(LHSRuleNode, RHSRuleNode, prams, name);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public void DeleteStatements() 
        {
            epService.EPAdministrator.StopAllStatements();
            epService.EPAdministrator.DestroyAllStatements();
        }

        private void TraverseRule(XmlNode LHSRuleNode, XmlNode RHSRuleNode, Dictionary<string, string> prams, String statementName)
        {
            string statementStr = "";
            if (prams["type"] != null)
            {
                string type = prams["type"].ToLower();
                switch (type)
                {
                        
                    case "exists":
                        // select the correct template for each rule type and pass the parametrs 
                        //result = EPLTemplates.exists
                        if (LHSRuleNode != null)
                        {
                            XmlNode operand =  LHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string minOcc = prams["minOccurs"];
                            string maxOcc = prams["maxOccurs"];
                            string taskName = operand.Attributes["name"].Value;
                            string antecedent = "TaskComplete";

                            if (minOcc == "0" && maxOcc == "0")
                            {
                                // absence pattern
                                statementStr = EPLTemplates.absence_anti_pattern(startScope, endScope, st, se, taskName, 0, antecedent);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                epStatement.Events += defaultUpdateEventHandler;

                            }
                            else 
                            {
                                if (minOcc != "*")
                                {
                                    statementStr = EPLTemplates.below_min_occurrences(startScope, endScope, st, se, taskName, Convert.ToInt32(minOcc), antecedent);
                                    EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                                    epStatement.Events += defaultUpdateEventHandler;
                                }

                                if (maxOcc != "*")
                                {
                                    statementStr = EPLTemplates.above_max_occurrences_exists(startScope, endScope, st, se, taskName, Convert.ToInt32(maxOcc), antecedent);
                                    EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                    epStatement.Events += defaultUpdateEventHandler;
                                }
                            }
                        }
                        break;
                    #region atleastimes
                    case "atleasttimes":
                        if (LHSRuleNode != null)
                        {
                            XmlNode operand = LHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0,st.Length -1 );

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0,se.Length -1 );

                            string taskName = operand.Attributes["name"].Value;
                            int minOccur = Convert.ToInt32( prams["numberOfTimes"]);
                            string antecedent = "TaskComplete";

                            statementStr = EPLTemplates.below_min_occurrences(startScope, endScope, st, se, taskName, minOccur, antecedent);
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                        }
                        break;
                    #endregion
                    #region atmosttime
                    case "atmosttimes":
                        if (LHSRuleNode != null)
                        {
                            XmlNode operand = LHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string taskName = operand.Attributes["name"].Value;
                            int maxOccur = Convert.ToInt32(prams["numberOfTimes"]);
                            string antecedent = "TaskComplete";

                            statementStr = EPLTemplates.above_max_occurrences_exists(startScope, endScope, st, se, taskName, maxOccur, antecedent);
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                        }
                        break;
                    #endregion
                    case "sequence":
                        
                        if (LHSRuleNode != null && RHSRuleNode != null)
                        {


                            string isBeforeStr = prams["isBefore"];
                            string isNextStr = prams["isNext"];
                            
                            string timeSpanStr = prams["timeSpan"];
                            string withAbsence = prams["withAbsence"];

                            bool isBefore, isNext;
                            if (!String.IsNullOrEmpty(isBeforeStr))
                            {
                                isBefore = Convert.ToBoolean(isBeforeStr);
                            }
                            else
                            {
                                isBefore = false;
                            }

                            if (!String.IsNullOrEmpty(isNextStr))
                            {
                                isNext = Convert.ToBoolean(isNextStr);
                            }
                            else
                            {
                                isNext = false;
                            }

                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            XmlNode rOperand = RHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;
                            string rTaskName = rOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string consequent = "TaskComplete";
                            
                            if (isNext && !isBefore) 
                            {
                                // ToDo to be implemented later 
                            }
                            else if (isNext)
                            {
                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, "", timeSpanStr);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                epStatement.Events += defaultUpdateEventHandler;
                            }
                            else {
                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, "", timeSpanStr);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                                epStatement.Events += defaultUpdateEventHandler;
                            }
                        }

                        break;
                    case "response":
                        if (LHSRuleNode != null && RHSRuleNode != null)
                        {
                            string isOneToOneStr = prams["isOneToOne"];
                            string isBeforeStr = prams["isBefore"];
                            string timeSpanStr = prams["timeSpan"];
                            string withAbsence = prams["withAbsence"];

                            bool isBefore, isOneToOne;
                            if (!String.IsNullOrEmpty(isBeforeStr))
                            {
                                isBefore = Convert.ToBoolean(isBeforeStr);
                            }
                            else
                            {
                                isBefore = true; // this might look a bit strange but this should be the default value if not specified
                            }

                            if (!String.IsNullOrEmpty(isOneToOneStr))
                            {
                                isOneToOne = Convert.ToBoolean(isOneToOneStr);
                            }
                            else
                            {
                                isOneToOne = false;
                            }

                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            XmlNode rOperand = RHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;
                            string rTaskName = rOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string consequent = "TaskComplete";
                            string absent = "TaskComplete";
                            //TODO: This shall be later refactored to remove repeated code blocks within the different if cases
                            if (!isBefore && isOneToOne) 
                            {
                                //We can address this by instantiating the two respective anti patterns in the same time
                                
                                // anti pattern 4 response with isBefore=false
                                statementStr = EPLTemplates.response_when_isBefore_equals_false(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, timeSpanStr);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                epStatement.Events += defaultUpdateEventHandler;

                                statementStr = EPLTemplates.one_to_one_response(startScope, endScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                                EPStatement epStatement2 = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                                epStatement2.Events += defaultUpdateEventHandler;

                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, "", "");
                                EPStatement epStatement3 = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement3.Events += defaultUpdateEventHandler;

                            }
                            else if (!isBefore)
                            {
                                statementStr = EPLTemplates.response_when_isBefore_equals_false(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, timeSpanStr);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                epStatement.Events += defaultUpdateEventHandler;

                                //To handle the essence of response we have to instantiate the standard response anti pattern as well
                                // but we shall remove the time span here!
                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, "", "");
                                EPStatement epStatement2 = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement2.Events += defaultUpdateEventHandler;
                            }
                            else if (isOneToOne)
                            {
                                statementStr = EPLTemplates.one_to_one_response(startScope, endScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                                epStatement.Events += defaultUpdateEventHandler;

                                //To handle the essence of response we have to instantiate the standard response anti pattern as well

                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, absent, timeSpanStr);
                                EPStatement epStatement2 = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement2.Events += defaultUpdateEventHandler;
                            }
                            else 
                            {
                                statementStr = EPLTemplates.sequence_and_response(startScope, endScope, st, se, lTaskName, rTaskName, withAbsence, antecedent, consequent, absent, timeSpanStr);
                                EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement.Events += defaultUpdateEventHandler;
                            }
                        }
                        break;
                    //case "one to one response":
                    //    if (LHSRuleNode != null && RHSRuleNode != null)
                    //    {
                    //        XmlNode lOperand = LHSRuleNode.FirstChild;
                    //        XmlNode rOperand = RHSRuleNode.FirstChild;
                    //        string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                    //        string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                    //        st = st.Substring(0, st.Length - 1);

                    //        string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                    //        string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                    //        se = se.Substring(0, se.Length - 1);

                    //        string lTaskName = lOperand.Attributes["name"].Value;
                    //        string rTaskName = rOperand.Attributes["name"].Value;

                    //        string antecedent = "TaskComplete";
                    //        string consequent = "TaskComplete";

                    //        result = EPLTemplates.one_to_one_response(startScope, endScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                    //    }
                    //    break;
                    case "precedes":
                        if (LHSRuleNode != null && RHSRuleNode != null)
                        {
                            string isOneToOneStr = prams["isOneToOne"];
                            string isBeforeStr = prams["isBefore"];
                            string timeSpanStr = prams["timeSpan"];
                            string withAbsence = prams["withAbsence"];

                            bool isBefore, isOneToOne;
                            if (!String.IsNullOrEmpty(isBeforeStr))
                            {
                                isBefore = Convert.ToBoolean(isBeforeStr);
                            }
                            else
                            {
                                isBefore = true;
                            }

                            if (!String.IsNullOrEmpty(isOneToOneStr))
                            {
                                isOneToOne = Convert.ToBoolean(isOneToOneStr);
                            }
                            else
                            {
                                isOneToOne = false;
                            }

                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            XmlNode rOperand = RHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;
                            string rTaskName = rOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string consequent = "TaskComplete";
                            string absent = "TaskComplete";
                            EPStatement epStatement;
                            if ((!string.IsNullOrEmpty(timeSpanStr) || !string.IsNullOrEmpty(withAbsence)) && isBefore)
                            {
                                statementStr = EPLTemplates.precedes_where_forbidden_or_timer(startScope, endScope, st, se, rTaskName, lTaskName, withAbsence, consequent, antecedent, absent, timeSpanStr);
                                epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                epStatement.Events += defaultUpdateEventHandler;
                            }
                            // This is the standard violation scenario and has to be monitored anyway!
                            statementStr = EPLTemplates.precedes_where_consequent_never_occurred(startScope, endScope, st, se, rTaskName, lTaskName, consequent, antecedent, timeSpanStr);
                            epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                            epStatement.Events += defaultUpdateEventHandler;

                            //if (!isBefore && isOneToOne)
                            //{
                            //    // The after pattern
                            //}
                            if (!isBefore)
                            {
                                statementStr = EPLTemplates.precedence_when_isBefore_equals_false(startScope, startScope, st, se, rTaskName, lTaskName, consequent, antecedent, timeSpanStr);
                                epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement.Events += defaultUpdateEventHandler;

                                if (!string.IsNullOrEmpty(withAbsence))
                                {
                                    statementStr = EPLTemplates.precedes_where_forbidden_or_timer(startScope, endScope, st, se, rTaskName, lTaskName, withAbsence, consequent, antecedent, absent, "");
                                    epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                                    epStatement.Events += defaultUpdateEventHandler;
                                }
                            }
                            if (isOneToOne) 
                            {
                                statementStr = EPLTemplates.one_to_one_precedes(startScope, startScope, st, se, rTaskName, lTaskName, consequent, antecedent, timeSpanStr);
                                epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_3");
                                epStatement.Events += defaultUpdateEventHandler;
                            }
                        }
                        break;
                    //case "one to one precedes":
                    //    if (LHSRuleNode != null && RHSRuleNode != null)
                    //    {
                    //        XmlNode lOperand = LHSRuleNode.FirstChild;
                    //        XmlNode rOperand = RHSRuleNode.FirstChild;
                    //        string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                    //        string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                    //        st = st.Substring(0, st.Length - 1);

                    //        string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                    //        string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                    //        se = se.Substring(0, se.Length - 1);

                    //        string lTaskName = lOperand.Attributes["name"].Value;
                    //        string rTaskName = rOperand.Attributes["name"].Value;

                    //        string antecedent = "TaskComplete";
                    //        string consequent = "TaskComplete";

                    //        result = EPLTemplates.one_to_one_precedes(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                    //    }
                    //    break;
                    case "separationofduty":
                        if (LHSRuleNode != null && RHSRuleNode != null)
                        {
                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            XmlNode rOperand = RHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;
                            string rTaskName = rOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string consequent = "TaskComplete";

                            statementStr = EPLTemplates.separation_of_duty_violation_1(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                            statementStr = EPLTemplates.separation_of_duty_violation_2(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                            epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                            epStatement.Events += defaultUpdateEventHandler;

                        }
    
                        break;
                    case "bindofduty":
                        if (LHSRuleNode != null && RHSRuleNode != null)
                        {
                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            XmlNode rOperand = RHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;
                            string rTaskName = rOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string consequent = "TaskComplete";

                            statementStr = EPLTemplates.bind_of_duty_violation_1(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                            statementStr = EPLTemplates.bind_of_duty_violation_2(startScope, startScope, st, se, lTaskName, rTaskName, antecedent, consequent, "");
                            epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_2");
                            epStatement.Events += defaultUpdateEventHandler;
  
                        }

                        break;
                    case "performedbyresource":
                        if (LHSRuleNode != null)
                        {
                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string resource = prams["Resource"];

                            statementStr = EPLTemplates.performed_by_resource_violation(startScope, endScope, st, se, lTaskName, antecedent, resource);
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                        }
                        break;
                    case "performedbyrole":
                        if (LHSRuleNode != null)
                        {
                            XmlNode lOperand = LHSRuleNode.FirstChild;
                            string startScope = prams["startScope"].Substring(0, prams["startScope"].IndexOf("("));
                            string st = prams["startScope"].Substring(prams["startScope"].IndexOf("(") + 1);
                            st = st.Substring(0, st.Length - 1);

                            string endScope = prams["endScope"].Substring(0, prams["endScope"].IndexOf("("));
                            string se = prams["endScope"].Substring(prams["endScope"].IndexOf("(") + 1);
                            se = se.Substring(0, se.Length - 1);

                            string lTaskName = lOperand.Attributes["name"].Value;

                            string antecedent = "TaskComplete";
                            string role = prams["Resource"];

                            statementStr = EPLTemplates.performed_by_role_violation(startScope, endScope, st, se, lTaskName, antecedent, role);
                            EPStatement epStatement = epService.EPAdministrator.CreateEPL(statementStr, statementName + "_1");
                            epStatement.Events += defaultUpdateEventHandler;

                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void defaultUpdateEventHandler(object sender, UpdateEventArgs e)
        {
            string overallRule = EventProcessor.inlineRule;
            string originalRule = String.Copy(overallRule);    
            string violatedRule = e.Statement.Name;
            string subRule;
            if (!violatedRule.Contains("--")) 
            {
                subRule = violatedRule.Split('_')[1];
                violatedRule = violatedRule.Split('_')[0];
                
                overallRule = overallRule.Replace(violatedRule, "false");
                EventProcessor.inlineRule = overallRule;
                manuplateRule();

                if (EventProcessor.inlineRule == "false" || EventProcessor.inlineRule == "(false)")
                {
                    EventProcessor.startListen = true;
                    Log.Info("\n");
                    Log.Info("*************************************************************");
                    Log.Info("*************************************************************");
                    Log.Info("Monitoring Rule (" + originalRule + ") is Violated \n as sub-rule: " + violatedRule + "-" + subRule + " is violated");
                    Log.Info("*************************************************************");
                    Log.Info("*************************************************************\n");
                    EventProcessor.inlineRule = originalRule;
                }
            }
        }

        private void manuplateRule() 
        {
            bool needAnotherIteration = false;
            string overallRule = EventProcessor.inlineRule;    
            string [] overallRuleArray = overallRule.Split(' ');
            for (int index = 0; index < overallRuleArray.Length;index++ )
            {
                string ruleItem = overallRuleArray[index];
                if (ruleItem == "AND")
                {
                    if (index - 1 >= 0 && index + 1 < overallRuleArray.Length)
                    {
                        if (overallRuleArray[index - 1].Contains("false") || overallRuleArray[index + 1].Contains("false"))
                        {
                            overallRuleArray[index - 1] = "";
                            overallRuleArray[index] = "false";
                            overallRuleArray[index + 1] = "";
                            needAnotherIteration = true;
                        }
                    }
                }
                else if (ruleItem == "OR")
                {
                    if (overallRuleArray[index - 1].Contains("false") && overallRuleArray[index + 1].Contains("false"))
                    {
                        overallRuleArray[index - 1] = "";
                        overallRuleArray[index] = "false";
                        overallRuleArray[index + 1] = "";
                        needAnotherIteration = true;
                    }
                }
            }
            string updatedRule;
            int noOfItems = getNonEmptyItemsNo(overallRuleArray);
            if (noOfItems == 1)
            {
                updatedRule = string.Join("", overallRuleArray);
            }
            else 
            {
                updatedRule = string.Join(" ", overallRuleArray);
            }
            EventProcessor.inlineRule = updatedRule;
            if (needAnotherIteration && noOfItems > 1)
                manuplateRule();
        }

        private int getNonEmptyItemsNo(string[] rule) 
        {
            int res = 0;
            foreach (string item in rule) 
            {
                if (item != "")
                    res++;
            }
            return res;
        }

        public IDictionary<string, string> GetStatements()
        {
            var dict = new Dictionary<string, string>();

            foreach (var statementName in epService.EPAdministrator.StatementNames)
            {
                var epStatement = epService.EPAdministrator.GetStatement(statementName);
                var name = epStatement.Name;
                var epl = epStatement.Text;
                dict.Add(name, epl);
            }

            return dict;
        }

        public void StopStatement(string name)
        {
            if (epService.EPAdministrator.StatementNames.Contains(name))
            {
                epService.EPAdministrator.GetStatement(name).Stop();
            }
        }

        public void StartStatement(string name)
        {
            if (epService.EPAdministrator.StatementNames.Contains(name))
            {
                epService.EPAdministrator.GetStatement(name).Start();
            }
        }

        public void StopAllStatements()
        {
            epService.EPAdministrator.StopAllStatements();
        }

        public void StartAllStatements()
        {
            epService.EPAdministrator.StartAllStatements();
        }
    }
}
