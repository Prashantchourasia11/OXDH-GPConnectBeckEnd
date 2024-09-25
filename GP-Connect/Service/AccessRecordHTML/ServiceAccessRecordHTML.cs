using AngleSharp.Text;
using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.Service.CommonMethods;
using Microsoft.SharePoint.News.DataModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace GP_Connect.Service.AccessRecordHTML
{
    public class ServiceAccessRecordHTML : IServiceAccessRecordHTML
    {
        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();

        #endregion

        #region Constructor

        public ServiceAccessRecordHTML()
        {
            _crmServiceClient = crmCon.crmconnectionOXVC();
        }

        #endregion

        #region Method

        public ResponseAccessHTML GetAccessHTMLRecord(RequestAccessHTMLDTO htmlDetails)
        {
            try
            {
                ResponseAccessHTML finalResponse = new ResponseAccessHTML();
                finalResponse.resourceType = "Bundle";
                finalResponse.type = "searchset";
                finalResponse.id =  Guid.NewGuid().ToString();

                List<object> htmlRecordList = new List<object>(); 

                var nhsNumber = htmlDetails.parameter[0].valueIdentifier.value;
                var code = htmlDetails.parameter[1].valueCodeableConcept.coding[0].code;
                ServiceCommonMethod scm = new ServiceCommonMethod();
                var patientDetails = scm.GetAllDetailsOfPatientByPatientIdUsedForHTMLACCESS(nhsNumber);

                finalResponse.entry = patientDetails;

                var device = MakeDeviceJSON();
                finalResponse.entry.Add(ConvertToResource(device));


                if (code == "SUM")
                {
                    var res = makeSummaryObject(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if(code == "ENC")
                {
                    var res = GetEncounterDetails(nhsNumber);  
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if (code == "CLI")
                {
                    var res = makeClinicalItem(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if (code == "PRB")
                {
                    var res = GetObjectForProblemAndIssue(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if (code == "ALL")
                {
                    var res = GetCurrentAndResolvedAllergy(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if (code == "MED")
                {
                    var res = makeMedicationObject(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }

                if (code == "REF")
                {
                    var res = GetReferralObject(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }

                if (code == "OBS")
                {
                    var res = GetObservationObject(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }

                if (code == "IMM")
                {
                    var res = MakeImmunizationObject(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }
                if (code == "ADM")
                {
                    var res = MakeobjectForAdministractiveItem(nhsNumber);
                    finalResponse.entry.Add(ConvertToResource(res));
                }


                return finalResponse;
            }
            catch(Exception ex)
            {
                return null;
            }
        }





        #endregion

        #region Internal Method

        #region ConvertToResource
        internal object ConvertToResource(object finalObj)
        {
            var FinalObject = new Dictionary<string, object>
                          {
                              { "resource",  finalObj}
                          };
            return FinalObject;
        }
        #endregion

        #region Device
        internal object MakeDeviceJSON()
        {
            try
            {
                var device = new Dictionary<string, object>
        {
            { "resourceType", "Device" },
            { "id", "f4d020c6-eaff-4528-83ba-e81a1cfd30dc" },
            { "meta", new Dictionary<string, object>
                {
                    { "versionId", "9008072080809363811" },
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-device-1"
                        }
                    }
                }
            },
            { "identifier", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "EMIS" },
                        { "value", "EMIS" }
                    }
                }
            }
        };
                return device;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Allergy Section
        internal object GetCurrentAndResolvedAllergy(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var allergyXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='msemr_allergyintolerance'>
                                     <attribute name='msemr_allergyintoleranceid' />
                                     <attribute name='msemr_name' />
                                     <attribute name='createdon' />
                                     <attribute name='statecode' />
                                     <attribute name='bcrm_enddate' />
                                     <attribute name='bcrm_asserteddate' />
                                     <order attribute='bcrm_asserteddate' descending='true' />
                                     <link-entity name='contact' from='contactid' to='msemr_patient' link-type='inner' alias='patient'>
                           
                                       <attribute name='bcrm_gpc_sequence_number' />
                                       <filter type='and'>
                                         <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                       </filter>
                                     <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                            <attribute name='bcrm_name' />
                                         </link-entity>
                                    
                                     </link-entity>
                                   </entity>
                                 </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(allergyXML));
                List<AllerguStartEndDetailsDTO> allergyList = new List<AllerguStartEndDetailsDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        AllerguStartEndDetailsDTO allergyRecord = new AllerguStartEndDetailsDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        allergyRecord.allergyName = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_asserteddate")) { allergyRecord.startDate = (DateTime)record.Attributes["bcrm_asserteddate"]; }
                        if (record.Attributes.Contains("bcrm_enddate")) { allergyRecord.endDate = (DateTime)record.Attributes["bcrm_enddate"]; }
                        allergyRecord.allergyStatus = record.Attributes.Contains("statecode") ? record.FormattedValues["statecode"].ToString() : "";
                        allergyList.Add(allergyRecord);
                    }
                }
                if (allergyList.Count > 0)
                {
                    var activeAllergyDiv = makeActiveAllergyList(allergyList);
                    var inActiveAllergyDiv = makeInActiveAllergyList(allergyList);

                    var finalAllergyString = "<div> <h1>Allergies and Adverse Reactions</h1> <div> <p> GP transfer banner </p> </div> <div> <p> Content banner </p> </div> <div> <h2>Current Allergies and Adverse Reactions</h2> <div> <p> Content banner </p> </div> <div> <p> Exclusion banner </p> </div> <table> <thead> <tr> <th>Start Date</th> <th>Details</th> </tr> </thead> <tbody> </tbody> " + activeAllergyDiv + " </table> </div> <div> <h2>Historical Allergies and Adverse Reactions</h2> <div> <p> Content banner </p> </div> <div> <p> Exclusion banner </p> </div> <table> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Details</th> </tr> </thead> <tbody> " + inActiveAllergyDiv + " </tbody> </table> </div> </div>";

                    var allergyJSON = MakeAllergyCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, finalAllergyString);
                    return allergyJSON;

                }


                return null;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        internal string makeActiveAllergyList(List<AllerguStartEndDetailsDTO> allergyList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in allergyList)
                {
                    if (item.allergyStatus.ToLower().ToString() == "active")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.allergyName + "</td>";
                        htmlContent += "</tr>";
                    }
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        internal string makeInActiveAllergyList(List<AllerguStartEndDetailsDTO> allergyList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in allergyList)
                {
                    if (item.allergyStatus.ToLower().ToString() == "inactive")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.allergyName + "</td>";
                        htmlContent += "</tr>";
                    }
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        internal object MakeAllergyCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Allergies and Sensitivities" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "ALL" }
                                        }
                                    }
                                },
                                { "text", "Allergies and Sensitivities" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        #region Encounter
        internal object GetEncounterDetails(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var encounterXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='msemr_encounter'>
                                  <attribute name='msemr_encounterid' />
                                  <attribute name='msemr_name' />
                                  <attribute name='msemr_encounterstartdate' />
                                   <attribute name='bcrm_details' />
                                  <attribute name='createdon' />
                                  <order attribute='msemr_encounterstartdate' descending='true' />
                                  <link-entity name='contact' from='contactid' to='msemr_encounterpatientidentifier' link-type='inner' alias='patient'>
                                       <attribute name='bcrm_gpc_sequence_number' />
                                     <filter type='and'>
                                      <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                    </filter>
                                     <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                            <attribute name='bcrm_name' />
                                         </link-entity>
                                   
                                  </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(encounterXML));
                List<EncounterDetailsDTO> encounterDetailsList = new List<EncounterDetailsDTO>();


                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        EncounterDetailsDTO encounterDetails = new EncounterDetailsDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        encounterDetails.title = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                        encounterDetails.details = record.Attributes.Contains("bcrm_details") ? record["bcrm_details"].ToString() : string.Empty;
                        if (record.Attributes.Contains("msemr_encounterstartdate")) { encounterDetails.Date = (DateTime)record.Attributes["msemr_encounterstartdate"]; }
                        encounterDetailsList.Add(encounterDetails);

                    }
                }
                if (encounterDetailsList.Count > 0)
                {
                    var divString = CreateEncounterHTMLDivByJSON(encounterDetailsList);
                    var finalobject = CreateEncounterJSONByUsingRecord(patientSequenceNumber, organizationSequenceNumber, organizationName, divString);
                    return finalobject;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        internal string CreateEncounterHTMLDivByJSON(List<EncounterDetailsDTO> encounterList)
        {
            var tableTD = "";
            foreach (var item in encounterList)
            {
                tableTD += "<tr>";
                tableTD += "<td>" + item.Date.ToString("dd-MMM-yyyy") + "</td>";
                tableTD += "<td>" + item.title + "</td>";
                tableTD += "<td>" + item.details + "</td>";
                tableTD += "</tr>";
            }

            string div = "<div> <h1>Encounters</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> For the period "+DateTime.Now.AddYears(-1).ToString("dd-MMM-yyyy") +" to "+DateTime.Now.ToString("dd-MMM-yyyy")+ " </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"enc-tab\"> <thead> <tr> <th>Date</th> <th>Title</th> <th>Details</th> </tr> </thead> <tbody> " + tableTD + "  </tbody> </table> </div> </div>";
            return div;
        }
        internal object CreateEncounterJSONByUsingRecord(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Encounters" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "ENC" }
                                        }
                                    }
                                },
                                { "text", "Encounters" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Problems And Issue

        public object GetObjectForProblemAndIssue(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var conditionsXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='msemr_condition'>
                                 <attribute name='msemr_conditionid' />
                                 <attribute name='msemr_name' />
                                 <attribute name='createdon' />
                                 <attribute name='msemr_asserteddate' />
                                 <attribute name='bcrm_enddate' />
                                 <attribute name='msemr_name' />
                                 <attribute name='statecode' />
                                 <attribute name='bcrm_significance' />
                                 <attribute name='bcrm_description' />
                                 <order attribute='msemr_asserteddate' descending='true' />
                                 <link-entity name='contact' from='contactid' to='msemr_subjecttypepatient' link-type='inner' alias='patient'>
                                   <attribute name='bcrm_gpc_sequence_number' />
                                   <filter type='and'>
                                     <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                   </filter>
                                  <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                         <attribute name='bcrm_name' />
                                      </link-entity>
                                 </link-entity>
                               </entity>
                             </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(conditionsXML));
                List<ProblemAndIssueDTO> problemIssueList = new List<ProblemAndIssueDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        ProblemAndIssueDTO PAIDTO = new ProblemAndIssueDTO();

                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        PAIDTO.entry = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("msemr_asserteddate")) { PAIDTO.startDate = (DateTime)record.Attributes["msemr_asserteddate"]; }
                        if (record.Attributes.Contains("bcrm_enddate")) { PAIDTO.endDate = (DateTime)record.Attributes["bcrm_enddate"]; }
                        PAIDTO.details = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;
                        PAIDTO.significance = record.Attributes.Contains("bcrm_significance") ? record.FormattedValues["bcrm_significance"].ToString() : string.Empty;
                        PAIDTO.status = record.Attributes.Contains("statecode") ? record.FormattedValues["statecode"].ToString() : "";

                        problemIssueList.Add(PAIDTO);

                    }
                }

                if(problemIssueList.Count > 0)
                {
                    var activeProbString = MakeActiveProblemAndIssue(problemIssueList);
                    var majorInactiveString = MakeMajorInactiveProblemAndIssue(problemIssueList);
                    var otherInavtiveString = MakeOtherInactiveProblemAndIssue(problemIssueList);

                    var finalDiv = "<div> <h1>Problems and Issues</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div> <h2>Active Problems and Issues</h2> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Date banner </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"prb-tab-act\"> <thead> <tr> <th>Start Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> "+ activeProbString + " </tbody> </table> </div> <div> <h2>Major Inactive Problems and Issues</h2> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Date banner </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner - </p> </div> <table id=\"prb-tab-majinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> "+ majorInactiveString + " </tbody> </table> </div> <div> <h2>Other Inactive Problems and Issues</h2> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Date banner </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"prb-tab-othinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> "+ otherInavtiveString + "  </tbody> </table> </div> </div>";
                    var res = MakeProblemAndIssueCompositionObject(patientSequenceNumber,organizationSequenceNumber,organizationName,finalDiv);
                    return res;

                }



                        return new object();
            }
            catch (Exception ex) 
            {
                return new object();
            }
        }

        public string MakeActiveProblemAndIssue(List<ProblemAndIssueDTO> problemIssueList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in problemIssueList)
                {
                    if (item.status.ToLower().ToString() == "active")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.entry + "</td>";
                        htmlContent += "<td>" + item.significance + "</td>";
                        htmlContent += "<td>" + item.details + "</td>";
                        htmlContent += "</tr>";
                    }
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        public string MakeMajorInactiveProblemAndIssue(List<ProblemAndIssueDTO> problemIssueList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in problemIssueList)
                {
                    if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() == "major")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.entry + "</td>";
                        htmlContent += "<td>" + item.significance + "</td>";
                        htmlContent += "<td>" + item.details + "</td>";
                        htmlContent += "</tr>";
                    }
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string MakeOtherInactiveProblemAndIssue(List<ProblemAndIssueDTO> problemIssueList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in problemIssueList)
                {
                    if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() != "major")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.entry + "</td>";
                        htmlContent += "<td>" + item.significance + "</td>";
                        htmlContent += "<td>" + item.details + "</td>";
                        htmlContent += "</tr>";
                    }
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        internal object MakeProblemAndIssueCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Problem and issues" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "PRB" }
                                        }
                                    }
                                },
                                { "text", "Problem and issues" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        #endregion

        #region Referral

        internal object GetReferralObject(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var referraalXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='msemr_referralrequest'>
                                      <attribute name='msemr_referralrequestid' />
                                      <attribute name='msemr_name' />
                                      <attribute name='bcrm_toreferraldoctor' />
                                      <attribute name='bcrm_priority' />
                                      <attribute name='msemr_description' />
                                      <attribute name='createdon' />
                                      <order attribute='createdon' descending='true' />
                                      <link-entity name='contact' from='contactid' to='msemr_requesteragentpatient' link-type='inner' alias='patient'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                        <filter type='and'>
                                          <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                        </filter>
                                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                           <attribute name='bcrm_gpc_sequence_number' />
                                           <attribute name='bcrm_name' />
                                        </link-entity>
                                      </link-entity>
                                    </entity>
                                  </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(referraalXML));
                List<ReferralDTO> referralList = new List<ReferralDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        ReferralDTO referralDetails = new ReferralDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        referralDetails.fromdoctor = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                        referralDetails.description = record.Attributes.Contains("msemr_description") ? record["msemr_description"].ToString() : string.Empty;
                        referralDetails.toDoctor = record.Attributes.Contains("bcrm_toreferraldoctor") ? record["bcrm_toreferraldoctor"].ToString() : string.Empty;

                        if (record.Attributes.Contains("createdon")) { referralDetails.createdon = (DateTime)record.Attributes["createdon"]; }
                      
                        referralDetails.priority = record.Attributes.Contains("bcrm_priority") ? record.FormattedValues["bcrm_priority"].ToString() : "";
                        referralList.Add(referralDetails);
                    }
                }

                if(referralList.Count > 0)
                {
                    var res = makeReferralDiv(referralList);
                    var htmlcontent = "<div> <h1>Referrals</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Data items until "+DateTime.Now.ToString("dd-MMM-yyyy")+ "</p>  </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"ref-tab\"> <thead> <tr> <th>Date</th> <th>From</th> <th>To</th> <th>Priority</th> <th>Details</th> </tr> </thead> <tbody> " + res+" </tbody> </table> </div>";
                    var finalObj = MakeReferralCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }


                return new object();
            }
            catch (Exception) 
            {
              return new object();
            }
        }
        internal string makeReferralDiv(List<ReferralDTO> referralList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in referralList)
                {
                    htmlContent += "<tr>";
                    htmlContent += "<td>" + item.createdon.ToString("dd-MMM-yyyy") + "</td>";
                    htmlContent += "<td>" + item.fromdoctor + "</td>";
                    htmlContent += "<td>" + item.toDoctor + "</td>";
                    htmlContent += "<td>" + item.priority + "</td>";
                    htmlContent += "<td>" + item.description + "</td>";
                    htmlContent += "</tr>";
                }
                return htmlContent;
            }
            catch (Exception e) 
            {
              return string.Empty;
            }

        }
        internal object MakeReferralCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Referrals" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "REF" }
                                        }
                                    }
                                },
                                { "text", "Referrals" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        #region Observation

        public object GetObservationObject(string nhsNumber)
        {
            var patientSequenceNumber = "";
            var organizationSequenceNumber = "";
            var organizationName = "";

            var obsXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                           <entity name='msemr_observation'>
                             <attribute name='msemr_observationid' />
                             <attribute name='msemr_description' />
                             <attribute name='createdon' />
                             <attribute name='bcrm_title' />
                             <attribute name='msemr_valuestring' />
                             <attribute name='msemr_valuerangehighlimit' />
                             <attribute name='msemr_description' />
                             <order attribute='createdon' descending='true' />
                             <link-entity name='contact' from='contactid' to='msemr_subjecttypepatient' link-type='inner' alias='patient'>
                               <attribute name='bcrm_gpc_sequence_number' />
                               <filter type='and'>
                                 <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber+ @"' />
                               </filter>
                              <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                     <attribute name='bcrm_gpc_sequence_number' />
                                     <attribute name='bcrm_name' />
                                  </link-entity>
                             </link-entity>
                           </entity>
                         </fetch>";

            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(obsXML));
            List<ObservationDTO> observationList = new List<ObservationDTO>();

            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                foreach (var record in AnswerCollection.Entities)
                {
                    ObservationDTO observationDetails = new ObservationDTO();
                    if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                    {
                        dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                        patientSequenceNumber = patientRecord.Value;
                    }

                    if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                    {
                        dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                        organizationSequenceNumber = organizationRecord.Value;
                    }

                    if (record.Attributes.Contains("organization.bcrm_name"))
                    {
                        dynamic organizationRecord = record["organization.bcrm_name"];
                        organizationName = organizationRecord.Value;
                    }

                    observationDetails.title = record.Attributes.Contains("bcrm_title") ? record["bcrm_title"].ToString() : string.Empty;
                    observationDetails.valueString = record.Attributes.Contains("msemr_valuestring") ? record["msemr_valuestring"].ToString() : string.Empty;
                    observationDetails.range = record.Attributes.Contains("msemr_valuerangehighlimit") ? record["msemr_valuerangehighlimit"].ToString() : string.Empty;
                    observationDetails.details = record.Attributes.Contains("msemr_description") ? record["msemr_description"].ToString() : string.Empty;
                    if (record.Attributes.Contains("createdon")) { observationDetails.createdOn = (DateTime)record.Attributes["createdon"]; }


                    observationList.Add(observationDetails);
                }
            }

            if (observationList.Count > 0)
            {
               var res = makeObservationhtmlContents(observationList);
               var htmlContent = "<div> <h1>Observations</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Data items until "+DateTime.Now.ToString("dd-MMM-yyyy")+ "</p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"obs-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Value</th> <th>Details</th> </tr> </thead> "+res+" <tbody> </tbody> </table> </div>";

               var finalObj = MakeObservationObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlContent);
               return finalObj;

            }

            return new object();
        }

        public string makeObservationhtmlContents(List<ObservationDTO> observationList)
        {
            var htmlContent = "";
            foreach (var item in observationList)
            {
                
                htmlContent += "<tr>";
                htmlContent += "<td>" + item.createdOn.ToString("dd-MMM-yyyy") + "</td>";
                htmlContent += "<td>" + item.title + "</td>";
                htmlContent += "<td>" + item.valueString + "</td>";
                htmlContent += "<td>" + item.range + "</td>";
                htmlContent += "<td>" + item.details + "</td>";
                htmlContent += "</tr>";
                
            }
            return htmlContent;
        }
        internal object MakeObservationObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Observations" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "OBS" }
                                        }
                                    }
                                },
                                { "text", "Observations" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        #endregion

        #region Immunization

        internal object MakeImmunizationObject(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                string immunizationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bcrm_immunization'>
                                      <attribute name='bcrm_immunizationid' />
                                      <attribute name='bcrm_name' />
                                      <attribute name='createdon' />
                                      <attribute name='bcrm_daterecorded' />
                                      <attribute name='bcrm_partdosenumber' />
                                      <attribute name='bcrm_contents' />
                                      <attribute name='bcrm_description' />
                                      <order attribute='createdon' descending='true' />
                                      <link-entity name='contact' from='contactid' to='bcrm_patient' link-type='inner' alias='patient'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                        <filter type='and'>
                                          <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber+ @"' />
                                        </filter>
                                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                            <attribute name='bcrm_name' />
                                         </link-entity>
                                      </link-entity>
                                    </entity>
                                  </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(immunizationXML));
                List<ImmunizationDTO> immunizationList = new List<ImmunizationDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        ImmunizationDTO immunizationRecord = new ImmunizationDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        immunizationRecord.vaccinationName = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        immunizationRecord.Part = record.Attributes.Contains("bcrm_partdosenumber") ? record["bcrm_partdosenumber"].ToString() : string.Empty;
                        immunizationRecord.Content = record.Attributes.Contains("bcrm_contents") ? record["bcrm_contents"].ToString() : string.Empty;
                        immunizationRecord.Details = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_daterecorded")) { immunizationRecord.RecDate = (DateTime)record.Attributes["bcrm_daterecorded"]; }

                        immunizationList.Add(immunizationRecord);
                    }
                }
                if(immunizationList.Count > 0)
                {
                    var res = makeImmunizationHtmlContent(immunizationList);
                    var htmlContent = "<div> <h1>Immunisations</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"imm-tab\"> <thead> <tr> <th>Date</th> <th>Vaccination</th> <th>Part</th> <th>Contents</th> <th>Details</th> </tr> </thead> <tbody> "+ res + " </tbody> </table> </div>";    
                    var finalObj = MakeImmunizationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlContent);
                    return finalObj;
                }
                else
                {
                    var htmlContent = "<div> <h1>Immunisations</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"exclusion-banner\"> <p> <p>Items excluded due to confidentiality and/or patient preferences</p> </p> </div>  </div>";
                    var finalObj = MakeImmunizationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlContent);
                    return finalObj;
                }

            }
            catch(Exception)
            {
                return new object();
            }
           
        }
        internal string makeImmunizationHtmlContent(List<ImmunizationDTO> immunizationList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in immunizationList)
                {
                    htmlContent += "<tr>";
                    htmlContent += "<td>" + item.RecDate.ToString("dd-MMM-yyyy") + "</td>";
                    htmlContent += "<td>" + item.vaccinationName + "</td>";
                    htmlContent += "<td>" + item.Part + "</td>";
                    htmlContent += "<td>" + item.Content + "</td>";
                    htmlContent += "<td>" + item.Details + "</td>";
                    htmlContent += "</tr>";
                }
                return htmlContent;
            }
            catch (Exception e)
            {
                return string.Empty;
            }


        }
        internal object MakeImmunizationCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Immunisations" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "IMM" }
                                        }
                                    }
                                },
                                { "text", "Immunisations" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        #region Administractive Item

        internal object MakeobjectForAdministractiveItem(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";


                var administractorXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='bcrm_administractiveitem'>
                                          <attribute name='bcrm_administractiveitemid' />
                                          <attribute name='bcrm_name' />
                                          <attribute name='createdon' />
                                           <attribute name='bcrm_recordeddate' />
                                           <attribute name='bcrm_description' />
                                          <order attribute='bcrm_recordeddate' descending='true' />
                                          <link-entity name='contact' from='contactid' to='bcrm_patient' link-type='inner' alias='patient'>
                                           <attribute name='bcrm_gpc_sequence_number' />
                                            <filter type='and'>
                                              <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                            </filter>
                                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                                 <attribute name='bcrm_gpc_sequence_number' />
                                                 <attribute name='bcrm_name' />
                                              </link-entity>
                                          </link-entity>
                                        </entity>
                                      </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(administractorXML));
                List<AdministractorItemDTO> administractorList = new List<AdministractorItemDTO>();
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        AdministractorItemDTO administractiveRecord = new AdministractorItemDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        administractiveRecord.Name = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_recordeddate")) { administractiveRecord.recDate = (DateTime)record.Attributes["bcrm_recordeddate"]; }
                        administractiveRecord.details = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;

                        administractorList.Add(administractiveRecord);
                    }
                }
                if (administractorList.Count > 0)
                {
                    var res = MakeAdministractorDiv(administractorList);
                    var htmlcontent = "<div> <h1>Administrative Items</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> Data items until "+DateTime.Now.ToString("dd-MMM-yyyy")+ " </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"adm-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> </tr> </thead> <tbody> "+res+" </tbody> </table> </div>";
                    var finalObj = MakeAdministractorCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;

                }

                return new object();
            }
            catch (Exception) 
            {
              return new object();
            }

        }
        internal string MakeAdministractorDiv(List<AdministractorItemDTO> administractorList)
        {
            var htmlContent = "";
            foreach (var item in administractorList)
            {
                
                    htmlContent += "<tr>";
                    htmlContent += "<td>" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
                    htmlContent += "<td>" + item.Name + "</td>";
                    htmlContent += "<td>" + item.details + "</td>";
                    htmlContent += "</tr>";
            }
            return htmlContent;
        }
        internal object MakeAdministractorCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Administrative items" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "ADM" }
                                        }
                                    }
                                },
                                { "text", "Administrative items" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        #endregion

        #region Clinical Item

        internal object makeClinicalItem(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var clinicalItemXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='bcrm_observationrequest'>
                                          <attribute name='bcrm_observationrequestid' />
                                          <attribute name='bcrm_name' />
                                          <attribute name='createdon' />
                                         <attribute name='bcrm_description' />
                                         <attribute name='bcrm_recordeddate' />
                                          <order attribute='bcrm_recordeddate' descending='true' />
                                          <link-entity name='contact' from='contactid' to='bcrm_patient' link-type='inner' alias='patient'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                            <filter type='and'>
                                              <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber+ @"' />
                                            </filter>
                                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                                 <attribute name='bcrm_gpc_sequence_number' />
                                                 <attribute name='bcrm_name' />
                                              </link-entity>
                                          </link-entity>
                                        </entity>
                                      </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(clinicalItemXML));
                List<ClinicalItemDTO> clinicalItemList = new List<ClinicalItemDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        ClinicalItemDTO clinicalRecord = new ClinicalItemDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        clinicalRecord.name = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_recordeddate")) { clinicalRecord.recDate = (DateTime)record.Attributes["bcrm_recordeddate"]; }
                        clinicalRecord.details = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;

                        clinicalItemList.Add(clinicalRecord);
                    }
                }
                if (clinicalItemList.Count > 0)
                {
                    var res = makehtmlcontentofclinicalitems(clinicalItemList);
                    var htmlcontent = "<div> <h1>Clinical Items</h1> <div class=\"gptransfer-banner\"> <p> GP transfer banner </p> </div> <div class=\"content-banner\"> <p> Content banner </p> </div> <div class=\"date-banner\"> <p> For the period "+DateTime.Now.AddYears(-1).ToString("dd-MMM-yyyy") +" to "+DateTime.Now.ToString("dd-MMM-yyyy")+ " </p> </div> <div class=\"exclusion-banner\"> <p> Exclusion banner </p> </div> <table id=\"cli-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> </tr> </thead> <tbody> "+res+" </tbody> </table> </div>";
                    var finalObj = MakeClinicalItemCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }


                return new object();
            }
            catch (Exception ) 
            {
              return new object();
            }
        }

        internal string makehtmlcontentofclinicalitems(List<ClinicalItemDTO> clinicalItemList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in clinicalItemList)
                {
                    
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.name + "</td>";
                        htmlContent += "<td>" + item.details + "</td>";
                        htmlContent += "</tr>";
                    
                }
                return htmlContent;
            }
            catch(Exception)
            {
                return "";
            }

        }
        internal object MakeClinicalItemCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Clinical items" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "CLI" }
                                        }
                                    }
                                },
                                { "text", "Clinical items" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        #endregion

        #region Medication

        internal object makeMedicationObject(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var medicationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='bcrm_prescription'>
                                        <attribute name='bcrm_prescriptionid' />
                                        <attribute name='bcrm_name' />
                                        <attribute name='createdon' />
                                        <attribute name='bcrm_type' />
                                        <attribute name='bcrm_drugstartdate' />
                                        <attribute name='bcrm_directions' />
                                        <attribute name='bcrm_quantityvalue' />
                                        <attribute name='bcrm_drugenddate' />
                                        <attribute name='bcrm_expectedsupplyvalue' />
                                        <attribute name='bcrm_additionalinformation' />
                                        <attribute name='bcrm_lastissueddate' />
                                        <attribute name='bcrm_numberofprescriptionsissued' />
                                        <attribute name='bcrm_maxissues' />
                                        <attribute name='bcrm_reviewdate' />
                                        <attribute name='bcrm_controlleddrug' />
                                        <attribute name='bcrm_discontinueddate' />
                                        <attribute name='bcrm_discontinuationreason' />
                                        <order attribute='createdon' descending='true' />
                                        <link-entity name='contact' from='contactid' to='bcrm_contact' link-type='inner' alias='patient'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                          <filter type='and'>
                                            <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber+ @"' />
                                          </filter>
                                         <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                                <attribute name='bcrm_gpc_sequence_number' />
                                                <attribute name='bcrm_name' />
                                             </link-entity>
                                        </link-entity>
                                      </entity>
                                    </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(medicationXML));
                List<MedicationDTO> medicationList = new List<MedicationDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        MedicationDTO medicationRecord = new MedicationDTO();
                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        medicationRecord.MedicationItem = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_drugstartdate")) { medicationRecord.startDate = (DateTime)record.Attributes["bcrm_drugstartdate"]; }
                        medicationRecord.type = record.Attributes.Contains("bcrm_type") ? record.FormattedValues["bcrm_type"].ToString() : string.Empty;
                        medicationRecord.DosageInstruction = record.Attributes.Contains("bcrm_directions") ? record["bcrm_directions"].ToString() : string.Empty;
                        medicationRecord.Quantity = record.Attributes.Contains("bcrm_quantityvalue") ? record["bcrm_quantityvalue"].ToString() : string.Empty;

                        if (record.Attributes.Contains("bcrm_drugenddate")) { medicationRecord.endDate = (DateTime)record.Attributes["bcrm_drugenddate"]; }
                        if (record.Attributes.Contains("bcrm_lastissueddate")) { medicationRecord.LastIssuedDate = (DateTime)record.Attributes["bcrm_lastissueddate"]; }
                        if (record.Attributes.Contains("bcrm_reviewdate")) { medicationRecord.ReviewDate = (DateTime)record.Attributes["bcrm_reviewdate"]; }
                        if (record.Attributes.Contains("bcrm_discontinueddate")) { medicationRecord.DiscountinuedDate = (DateTime)record.Attributes["bcrm_discontinueddate"]; }

                        medicationRecord.DaysDuration = record.Attributes.Contains("bcrm_expectedsupplyvalue") ? record["bcrm_expectedsupplyvalue"].ToString() : string.Empty;
                        medicationRecord.AdditionalInformation = record.Attributes.Contains("bcrm_additionalinformation") ? record["bcrm_additionalinformation"].ToString() : string.Empty;
                        medicationRecord.NumberOfPrescriptionIsuued = record.Attributes.Contains("bcrm_numberofprescriptionsissued") ? record["bcrm_numberofprescriptionsissued"].ToString() : string.Empty;
                        medicationRecord.MaxIssues = record.Attributes.Contains("bcrm_maxissues") ? record["bcrm_maxissues"].ToString() : string.Empty;

                        medicationRecord.DiscountinuedReason = record.Attributes.Contains("bcrm_discontinuationreason") ? record["bcrm_discontinuationreason"].ToString() : string.Empty;

                        medicationRecord.ControlledDrug = record.Attributes.Contains("bcrm_controlleddrug") ? record["bcrm_controlleddrug"].ToString() : string.Empty;


                        medicationList.Add(medicationRecord);
                    }
                }
                if (medicationList.Count > 0)
                {
                    var htmlcontenmt = createHtmlcontentofMedication(medicationList);
                    var finalObj = MakeMedicationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontenmt);
                    return finalObj;
                }
                else
                {
                    var htmlcontenmt = "<div> <h2>Acute Medication (Last 12 Months)</h2> <div class=\"content-banner\"> <p> This is a content banner written by oxdh. </p> </div> <div class=\"exclusion-banner\"> <p> Item excluded due to confidentiality anr/or patient preferences. </p> </div></div>";
                    var finalObj = MakeMedicationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontenmt);
                    return finalObj;
                }

            }
            catch (Exception) 
            {
             return new object();
            }
        }

        internal string createHtmlcontentofMedication(List<MedicationDTO> medicationList)
        {
            try
            {
                var acuteMedicationDiv = "";
                var repeatMedicationDiv = "";
                var discountinuedReapeatMedication = "";
                var allMedication = "";
                var allmedicationIssue = "";

                foreach (var item in medicationList) 
                {
                  if(item.startDate >= DateTime.Now.AddYears(-1) && item.startDate <= DateTime.Now && item.type.ToLower().ToString() == "acute")
                    {
                        acuteMedicationDiv += "<tr>";
                        acuteMedicationDiv += "<td>" + item.type + "</td>";
                        acuteMedicationDiv += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        acuteMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                        acuteMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                        acuteMedicationDiv += "<td>" + item.Quantity + "</td>";
                        acuteMedicationDiv += "<td>" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                        acuteMedicationDiv += "<td>" + item.DaysDuration + "</td>";
                        acuteMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        acuteMedicationDiv += "</tr>";
                    }
                    if (item.type.ToLower().ToString() == "repeat" && item.DiscountinuedReason == string.Empty)
                    {
                        repeatMedicationDiv += "<tr>";
                        repeatMedicationDiv += "<td>" + item.type + "</td>";
                        repeatMedicationDiv += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                        repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                        repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                        repeatMedicationDiv += "<td>" + item.LastIssuedDate.ToString("dd-MMM-yyyy") + "</td>";
                        repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                        repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                        repeatMedicationDiv += "<td>" + item.ReviewDate.ToString("dd-MMM-yyyy") + "</td>";
                        repeatMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        repeatMedicationDiv += "</tr>";
                    }
                    if (item.type.ToLower().ToString() == "repeat" && item.DiscountinuedReason != string.Empty)
                    {
                        discountinuedReapeatMedication += "<tr>";
                        discountinuedReapeatMedication += "<td>" + item.type + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.LastIssuedDate.ToString("dd-MMM-yyyy") + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.MedicationItem + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.DosageInstruction + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.Quantity + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.DiscountinuedDate.ToString("dd-MMM-yyyy") + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.DiscountinuedReason + "</td>";
                        discountinuedReapeatMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        discountinuedReapeatMedication += "</tr>";
                    }
                    if (item.MedicationItem != string.Empty)
                    {
                        allMedication += "<tr>";
                        allMedication += "<td colspan='7' class='med-item-column'> <strong>" + item.MedicationItem + "</strong></td>";
                        allMedication += "</tr>";
                        allMedication += "<tr>";
                        allMedication += "<td>" + item.type + "</td>";
                        allMedication += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        allMedication += "<td>" + item.MedicationItem + "</td>";
                        allMedication += "<td>" + item.DosageInstruction + "</td>";
                        allMedication += "<td>" + item.Quantity + "</td>";
                        allMedication += "<td>" + item.LastIssuedDate.ToString("dd-MMM-yyyy") + "</td>";
                        allMedication += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                        allMedication += "<td>" + item.DiscountinuedReason + "</td>";
                        allMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        allMedication += "</tr>";
                    }
                    if (item.LastIssuedDate.ToString() != "01-01-0001 00:00:00")
                    {
                        allmedicationIssue += "<tr>";
                        allmedicationIssue += "<td colspan='7' class='med-item-column'><strong>" + item.MedicationItem + "</strong></td>";
                        allmedicationIssue += "</tr>";

                        allmedicationIssue += "<tr>";
                        allmedicationIssue += "<td>" + item.type + "</td>";
                        allmedicationIssue += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        allmedicationIssue += "<td>" + item.MedicationItem + "</td>";
                        allmedicationIssue += "<td>" + item.DosageInstruction + "</td>";
                        allmedicationIssue += "<td>" + item.Quantity + "</td>";
                        allmedicationIssue += "<td>" + item.LastIssuedDate.ToString("dd-MMM-yyyy") + "</td>";
                        allmedicationIssue += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                        allmedicationIssue += "<td>" + item.DiscountinuedReason + "</td>";
                        allmedicationIssue += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        allmedicationIssue += "</tr>";
                    }
                }


                var htmlcontent = "<div> <h1>Medications</h1> <div class=\"gptransfer-banner\"> <p> Patient record transfer from previous GP Practice not yet complete; any information recorded before " + DateTime.Now.AddYears(-1).ToString("dd-MMM-yyyy") + " has been excluded </p> </div> <div class=\"content-banner\"> <p> May also contain immunisations issued as medications </p> </div>" +
                    "<div> <h2>Acute Medication (Last 12 Months)</h2> <div> <p>Data items until "+DateTime.Now.ToString("dd-MMM-yyyy")+ "</p> </div>" + "<table id=\"med-tab-acu-med\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Scheduled End Date</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> " + acuteMedicationDiv+" </tbody> </table> </div>" +
                    "<div> <h2>Current Repeat Medication</h2> <div class=\"content-banner\"> <p> The Review Date is that set for each Repeat Course. Reviews may be conducted according to a diary event which differs from the dates shown </p> </div> <table id=\"med-tab-curr-rep\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Max Issues</th> <th>Review Date</th> <th>Additional Information</th> </tr> </thead> <tbody> " + repeatMedicationDiv+" </tbody> </table> </div> " +
                    "<div> <h2>Discontinued Repeat Medication</h2> <div class=\"content-banner\"> <p> All repeat medication ended by a clinician action </p>  </div> <table id=\"med-tab-dis-rep\"> <thead> <tr> <th>Type</th> <th>Last Issued Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Discontinued Date</th> <th>Discontinuation Reason</th> <th>Additional Information</th> </tr> </thead> <tbody> " + discountinuedReapeatMedication+" </tbody> </table> </div> " +
                    "<div> <h2>All Medication</h2> <div class=\"content-banner\"> <p> For the period "+DateTime.Now.AddYears(-1).ToString("dd-MMM-yyyy") +" to "+DateTime.Now.ToString("dd-MMM-yyyy")+ " </p> </div> <div class=\"date-banner\"> <p style='color:red;'> All relevant items subject to patient preferences and / or RCGP exclusions </p> </div>  <table id=\"med-tab-all-sum\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Discontinuation Details</th> <th>Additional Information</th> </tr> </thead> <tbody> " + allMedication+" </tbody> </table> </div>" +
                    "<div> <h2>All Medication Issues</h2> <div class=\"content-banner\"> <p> For the period "+DateTime.Now.AddYears(-1).ToString("dd-MMM-yyyy") +" to "+DateTime.Now.ToString("dd-MMM-yyyy")+ " </p> </div> <div class=\"date-banner\"> <p style='color:red;'> All relevant items subject to patient preferences and / or RCGP exclusions </p> </div>  <table id=\"med-tab-all-iss\"> <thead> <tr> <th>Type</th> <th>Issue Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> "+allmedicationIssue+" </tbody> </table> </div> </div> </div>";
 
                return htmlcontent;

            }
            catch(Exception ex) 
            {
                return "";
            }
        }

        internal object MakeMedicationCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Medications" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "MED" }
                                        }
                                    }
                                },
                                { "text", "Medications" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion

        #region Summary

        internal object makeSummaryObject(string nhsNumber)
        {
            try
            {
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var emergencyCodeXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                       <entity name='bcrm_emergencycode'>
                                         <attribute name='bcrm_emergencycodeid' />
                                         <attribute name='bcrm_name' />
                                         <attribute name='createdon' />
                                         <attribute name='bcrm_recordeddate' />
                                         <attribute name='bcrm_description' />
                                            <attribute name='bcrm_locationoffurtherinformation' />
                                            <order attribute='bcrm_name' descending='false' />
                                         <link-entity name='contact' from='contactid' to='bcrm_patient' link-type='inner' alias='patient'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                           <filter type='and'>
                                             <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                           </filter>
                                           <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                                  <attribute name='bcrm_gpc_sequence_number' />
                                                  <attribute name='bcrm_name' />
                                               </link-entity>
                                         </link-entity>
                                       </entity>
                                     </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(emergencyCodeXML));
                List<emergencyCodeDTO> emergencyCodeList = new List<emergencyCodeDTO>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        emergencyCodeDTO EMRCod = new emergencyCodeDTO();

                        if (record.Attributes.Contains("patient.bcrm_gpc_sequence_number"))
                        {
                            dynamic patientRecord = record["patient.bcrm_gpc_sequence_number"];
                            patientSequenceNumber = patientRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_gpc_sequence_number"];
                            organizationSequenceNumber = organizationRecord.Value;
                        }

                        if (record.Attributes.Contains("organization.bcrm_name"))
                        {
                            dynamic organizationRecord = record["organization.bcrm_name"];
                            organizationName = organizationRecord.Value;
                        }

                        EMRCod.name = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        EMRCod.description = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;
                        EMRCod.locationoffurtherInformation = record.Attributes.Contains("bcrm_locationoffurtherinformation") ? record["bcrm_locationoffurtherinformation"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_recordeddate")) { EMRCod.recDate = (DateTime)record.Attributes["bcrm_recordeddate"]; }

                        emergencyCodeList.Add(EMRCod);

                    }
                }
                if(emergencyCodeList.Count > 0)
                {
                    var emerCodeHTMLContetnt = makehtmlcontentofEmergencyCodes(emergencyCodeList);
                    var thrreEccounterhtmlcontent = lastthreeencountersHTMLContents(nhsNumber);

                    // store problem and issue json
                    var problemAndIssueDataJSON = makeActiveAndMajorInactiveProblemAndIssue(nhsNumber);
                    //

                    var activeProblemhtmlcontent = MakeActiveProblemAndIssue(problemAndIssueDataJSON);
                    var majorInactivehtmlcontent = MakeMajorInactiveProblemAndIssue(problemAndIssueDataJSON);

                    var activeAlleryhtmlcontent = makecurrentallergydivhtmlcontent(nhsNumber);

                    // store json medication json
                    var medicationJson = makeacuteandrepeatmedication(nhsNumber);

                    var acutemedicationhtmlcontent = makeOnlyacutemedication(medicationJson);
                    var currentRepeatMedication = makeOnlyCurrentReapeatMedication(medicationJson);

                    var finalhtmlcontentofsummary = "<div xmlns=\\\"http://www.w3.org/1999/xhtml\\\"> <h1>Summary</h1> <div> <h2>Emergency Codes</h2> <div class=\\\"content-banner\\\"> <p>This section is enabled in response to a national health event to highlight specific codes from across the patient record, further details may be available in other parts of the record.</p> </div> <table id=\"cli-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> <th>Location of further information</th> </tr> </thead> <tbody>"+ emerCodeHTMLContetnt + "  </tbody> </table> </div> <div> <h2>Last 3 Encounters</h2> <div class=\"gptransfer-banner\"> <p> This is transfer banner GP2GP. written by OXDH.  </p> </div> <div class=\"content-banner\"> <p> This is content banner written by OXDH. </p> </div> <div class=\"date-banner\"> <p> This is date banner written by OXDH. </p> </div> <div class=\"exclusion-banner\"> <p> This is exclusive banner written by OXDH. </p> </div> <table id=\\\"enc-tab\\\"> <thead> <tr> <th>Date</th> <th>Title</th> <th>Details</th> </tr> </thead> <tbody> " + thrreEccounterhtmlcontent + " </tbody> </table> </div> <div> <h2>Active Problems and Issues</h2> <div class=\"content-banner\"> <p>This is content banner written by OXDH.</p> </div> <div class=\"date-banner\"> <p>This is date banner written by OXDH.</p> </div> <div class=\"exclusion-banner\"> <p>This is exclusive banner written by OXDH.</p> </div> <table id=\\\"prb-tab-act\\\"> <thead> <tr> <th>Start Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> " + activeProblemhtmlcontent + " </tbody> </table> </div> <div> <h2>Major Inactive Problems and Issues</h2> <table id=\"prb-tab-majinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> "+ majorInactivehtmlcontent + " </tbody> </table> </div> <div> <h2>Current Allergies and Adverse Reactions</h2> <div class=\"content-banner\"> <p> This is content banner written by OXDH. </p> </div> <div class=\"exclusion-banner\"> <p> This is exclusive banner written by OXDH. </p> </div> <table id=\\\"all-tab-curr\\\"> <thead> <tr> <th>Start Date</th> <th>Details</th> </tr> </thead> <tbody> " + activeAlleryhtmlcontent + " </tbody> </table> </div> <div> <h2>Acute Medication (Last 12 Months)</h2> <div> <p>Scheduled End Date is not always captured in the source; where it was not recorded, the displayed date is calculated from start date and days duration</p> </div> <table id=\\\"med-tab-acu-med\\\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Scheduled End Date</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> " + acutemedicationhtmlcontent + " </tbody> </table> </div> <div> <h2>Current Repeat Medication</h2> <div class=\"content-banner\"> <p> This is content banner written by OXDH. </p> </div> <div class=\"exclusion-banner\"> <p> This is Exclusive banner written by OXDH. </p> </div> <table id=\\\"med-tab-curr-rep\\\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Max Issues</th> <th>Review Date</th> <th>Additional Information</th> </tr> </thead> <tbody> " + currentRepeatMedication + " </tbody> </table> </div> </div>";

                    var finalObj = MakeSummaryCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, finalhtmlcontentofsummary);
                    return finalObj;

                }

                return new object();

            }
            catch (Exception)
            {
             return new object();
            }
        }
        internal string makehtmlcontentofEmergencyCodes(List<emergencyCodeDTO> emergencyCodeList)
        {
            try
            {
                var htmlContent = "";
                foreach (var item in emergencyCodeList)
                {
                        htmlContent += "<tr>";
                        htmlContent += "<td>" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td>" + item.name + "</td>";
                        htmlContent += "<td>" + item.description + "</td>";
                        htmlContent += "<td>" + item.locationoffurtherInformation + "</td>";
                        htmlContent += "</tr>";
                    
                }
                return htmlContent;
            }
            catch(Exception)
            {
                return "";
            }

        }
        internal string lastthreeencountersHTMLContents(string nhsNumber)
        {
            var encounterXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' top='3'>
                              <entity name='msemr_encounter'>
                                <attribute name='msemr_encounterid' />
                                <attribute name='msemr_name' />
                                <attribute name='msemr_encounterstartdate' />
                                <attribute name='createdon' />
                                 <attribute name='bcrm_details' />
                                <order attribute='createdon' descending='true' />
                                <link-entity name='contact' from='contactid' to='msemr_encounterpatientidentifier' link-type='inner' alias='patient'>
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                  </filter>
                                  <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                    <attribute name='bcrm_gpc_sequence_number' />
                                    <attribute name='bcrm_name' />
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(encounterXML));
            List<EncounterDetailsDTO> encounterDetailsList = new List<EncounterDetailsDTO>();


            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                foreach (var record in AnswerCollection.Entities)
                {
                    EncounterDetailsDTO encounterDetails = new EncounterDetailsDTO();

                    encounterDetails.title = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                    encounterDetails.details = record.Attributes.Contains("bcrm_details") ? record["bcrm_details"].ToString() : string.Empty;
                    if (record.Attributes.Contains("msemr_encounterstartdate")) { encounterDetails.Date = (DateTime)record.Attributes["msemr_encounterstartdate"]; }
                    encounterDetailsList.Add(encounterDetails);

                }
            }
            if(encounterDetailsList.Count > 0)
            {
                var encHtmlContent = "";
                foreach (var item in encounterDetailsList)
                {
                    encHtmlContent += "<tr>";
                    encHtmlContent += "<td>" + item.Date.ToString("dd-MMM-yyyy") + "</td>";
                    encHtmlContent += "<td>" + item.title + "</td>";
                    encHtmlContent += "<td>" + item.details + "</td>";
                    encHtmlContent += "</tr>";
                }
                return encHtmlContent;
            }



            return "";
        }
        internal List<ProblemAndIssueDTO> makeActiveAndMajorInactiveProblemAndIssue(string nhsNumber)
        {
            var conditionsXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='msemr_condition'>
                                 <attribute name='msemr_conditionid' />
                                 <attribute name='msemr_name' />
                                 <attribute name='createdon' />
                                 <attribute name='msemr_asserteddate' />
                                 <attribute name='bcrm_enddate' />
                                 <attribute name='msemr_name' />
                                 <attribute name='statecode' />
                                 <attribute name='bcrm_significance' />
                                 <attribute name='bcrm_description' />
                                 <order attribute='msemr_name' descending='false' />
                                 <link-entity name='contact' from='contactid' to='msemr_subjecttypepatient' link-type='inner' alias='patient'>
                                   <attribute name='bcrm_gpc_sequence_number' />
                                   <filter type='and'>
                                     <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                   </filter>
                                  <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                         <attribute name='bcrm_name' />
                                      </link-entity>
                                 </link-entity>
                               </entity>
                             </fetch>";
            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(conditionsXML));
            List<ProblemAndIssueDTO> problemIssueList = new List<ProblemAndIssueDTO>();

            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                foreach (var record in AnswerCollection.Entities)
                {
                    ProblemAndIssueDTO PAIDTO = new ProblemAndIssueDTO();

                    PAIDTO.entry = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                    if (record.Attributes.Contains("msemr_asserteddate")) { PAIDTO.startDate = (DateTime)record.Attributes["msemr_asserteddate"]; }
                    if (record.Attributes.Contains("bcrm_enddate")) { PAIDTO.endDate = (DateTime)record.Attributes["bcrm_enddate"]; }
                    PAIDTO.details = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;
                    PAIDTO.significance = record.Attributes.Contains("bcrm_significance") ? record.FormattedValues["bcrm_significance"].ToString() : string.Empty;
                    PAIDTO.status = record.Attributes.Contains("statecode") ? record.FormattedValues["statecode"].ToString() : "";

                    problemIssueList.Add(PAIDTO);
                }
            }
            return problemIssueList;
        }
        internal string makecurrentallergydivhtmlcontent(string nhsNumber)
        {
            var allergyXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='msemr_allergyintolerance'>
                                     <attribute name='msemr_allergyintoleranceid' />
                                     <attribute name='msemr_name' />
                                     <attribute name='createdon' />
                                     <attribute name='statecode' />
                                     <attribute name='bcrm_enddate' />
                                     <attribute name='bcrm_asserteddate' />
                                     <order attribute='msemr_name' descending='false' />
                                     <link-entity name='contact' from='contactid' to='msemr_patient' link-type='inner' alias='patient'>
                           
                                       <attribute name='bcrm_gpc_sequence_number' />
                                       <filter type='and'>
                                         <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                       </filter>
                                     <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                            <attribute name='bcrm_name' />
                                         </link-entity>
                                     </link-entity>
                                   </entity>
                                 </fetch>";

            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(allergyXML));
            List<AllerguStartEndDetailsDTO> allergyList = new List<AllerguStartEndDetailsDTO>();

            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                foreach (var record in AnswerCollection.Entities)
                {
                    AllerguStartEndDetailsDTO allergyRecord = new AllerguStartEndDetailsDTO();
                    

                    allergyRecord.allergyName = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                    if (record.Attributes.Contains("bcrm_asserteddate")) { allergyRecord.startDate = (DateTime)record.Attributes["bcrm_asserteddate"]; }
                    if (record.Attributes.Contains("bcrm_enddate")) { allergyRecord.endDate = (DateTime)record.Attributes["bcrm_enddate"]; }
                    allergyRecord.allergyStatus = record.Attributes.Contains("statecode") ? record.FormattedValues["statecode"].ToString() : "";
                    allergyList.Add(allergyRecord);
                }
            }
            if(allergyList.Count > 0)
            {
                var htmlcon = makeActiveAllergyList(allergyList);
                return htmlcon;
            }


            return "";
        }
        internal List<MedicationDTO> makeacuteandrepeatmedication(string nhsNumber)
        {
            var medicationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='bcrm_prescription'>
                                        <attribute name='bcrm_prescriptionid' />
                                        <attribute name='bcrm_name' />
                                        <attribute name='createdon' />
                                        <attribute name='bcrm_type' />
                                        <attribute name='bcrm_drugstartdate' />
                                        <attribute name='bcrm_directions' />
                                        <attribute name='bcrm_quantityvalue' />
                                        <attribute name='bcrm_drugenddate' />
                                        <attribute name='bcrm_expectedsupplyvalue' />
                                        <attribute name='bcrm_additionalinformation' />
                                        <attribute name='bcrm_lastissueddate' />
                                        <attribute name='bcrm_numberofprescriptionsissued' />
                                        <attribute name='bcrm_maxissues' />
                                        <attribute name='bcrm_reviewdate' />
                                        <attribute name='bcrm_discontinueddate' />
                                        <attribute name='bcrm_discontinuationreason' />
                                        <order attribute='bcrm_name' descending='false' />
                                        <link-entity name='contact' from='contactid' to='bcrm_contact' link-type='inner' alias='patient'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                          <filter type='and'>
                                            <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                          </filter>
                                         <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' link-type='inner' alias='organization'>
                                                <attribute name='bcrm_gpc_sequence_number' />
                                                <attribute name='bcrm_name' />
                                             </link-entity>
                                        </link-entity>
                                      </entity>
                                    </fetch>";
            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(medicationXML));
            List<MedicationDTO> medicationList = new List<MedicationDTO>();

            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                foreach (var record in AnswerCollection.Entities)
                {
                    MedicationDTO medicationRecord = new MedicationDTO();
                  

                    medicationRecord.MedicationItem = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                    if (record.Attributes.Contains("bcrm_drugstartdate")) { medicationRecord.startDate = (DateTime)record.Attributes["bcrm_drugstartdate"]; }
                    medicationRecord.type = record.Attributes.Contains("bcrm_type") ? record.FormattedValues["bcrm_type"].ToString() : string.Empty;
                    medicationRecord.DosageInstruction = record.Attributes.Contains("bcrm_directions") ? record["bcrm_directions"].ToString() : string.Empty;
                    medicationRecord.Quantity = record.Attributes.Contains("bcrm_quantityvalue") ? record["bcrm_quantityvalue"].ToString() : string.Empty;

                    if (record.Attributes.Contains("bcrm_drugenddate")) { medicationRecord.endDate = (DateTime)record.Attributes["bcrm_drugenddate"]; }
                    if (record.Attributes.Contains("bcrm_lastissueddate")) { medicationRecord.LastIssuedDate = (DateTime)record.Attributes["bcrm_lastissueddate"]; }
                    if (record.Attributes.Contains("bcrm_reviewdate")) { medicationRecord.ReviewDate = (DateTime)record.Attributes["bcrm_reviewdate"]; }
                    if (record.Attributes.Contains("bcrm_discontinueddate")) { medicationRecord.DiscountinuedDate = (DateTime)record.Attributes["bcrm_discontinueddate"]; }

                    medicationRecord.DaysDuration = record.Attributes.Contains("bcrm_expectedsupplyvalue") ? record["bcrm_expectedsupplyvalue"].ToString() : string.Empty;
                    medicationRecord.AdditionalInformation = record.Attributes.Contains("bcrm_additionalinformation") ? record["bcrm_additionalinformation"].ToString() : string.Empty;
                    medicationRecord.NumberOfPrescriptionIsuued = record.Attributes.Contains("bcrm_numberofprescriptionsissued") ? record["bcrm_numberofprescriptionsissued"].ToString() : string.Empty;
                    medicationRecord.MaxIssues = record.Attributes.Contains("bcrm_maxissues") ? record["bcrm_maxissues"].ToString() : string.Empty;

                    medicationRecord.DiscountinuedReason = record.Attributes.Contains("bcrm_discontinuationreason") ? record["bcrm_discontinuationreason"].ToString() : string.Empty;

                    medicationList.Add(medicationRecord);
                }
            }
            return medicationList;
           

        }
        internal string makeOnlyacutemedication(List<MedicationDTO> medicationList)
        {
            var acuteMedicationDiv = "";
            foreach (var item in medicationList)
            {
                if (item.startDate >= DateTime.Now.AddYears(-1) && item.startDate <= DateTime.Now && item.type.ToLower().ToString() == "acute")
                {
                    acuteMedicationDiv += "<tr>";
                    acuteMedicationDiv += "<td>" + item.type + "</td>";
                    acuteMedicationDiv += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                    acuteMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                    acuteMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                    acuteMedicationDiv += "<td>" + item.Quantity + "</td>";
                    acuteMedicationDiv += "<td>" + item.DaysDuration + "</td>";
                    acuteMedicationDiv += "<td>" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                    acuteMedicationDiv += "<td>" + item.AdditionalInformation + "</td>";
                }
            }
            return acuteMedicationDiv;
        }
        internal string makeOnlyCurrentReapeatMedication(List<MedicationDTO> medicationList)
        {
            var repeatMedicationDiv = "";
            foreach (var item in medicationList)
            {

                if (item.type.ToLower().ToString() == "repeat" && item.DiscountinuedReason == string.Empty)
                {
                    repeatMedicationDiv += "<tr>";
                    repeatMedicationDiv += "<td>" + item.type + "</td>";
                    repeatMedicationDiv += "<td>" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                    repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                    repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                    repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                    repeatMedicationDiv += "<td>" + item.LastIssuedDate.ToString("dd-MMM-yyyy") + "</td>";
                    repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                    repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                    repeatMedicationDiv += "<td>" + item.ReviewDate.ToString("dd-MMM-yyyy") + "</td>";
                    repeatMedicationDiv += "<td>" + item.AdditionalInformation + "</td>";
                }
            }
            return repeatMedicationDiv;
        }
        internal object MakeSummaryCompositionObject(string patinetSequenceNumber, string organizationSequenceNumber, string organizationName, string htmlDiv)
        {
            try
            {
                var composition = new Dictionary<string, object>
        {
            { "resourceType", "Composition" },
            { "id", Guid.NewGuid().ToString() },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "http://fhir.nhs.net/StructureDefinition/gpconnect-carerecord-composition-1"
                        }
                    }
                }
            },
            { "date", DateTime.UtcNow },
            { "title", "Patient Care Record" },
            { "status", "final" },
            { "subject", new Dictionary<string, string>
                {
                    { "reference", "Patient/" + patinetSequenceNumber }
                }
            },
            { "author", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "reference", "Device/f4d020c6-eaff-4528-83ba-e81a1cfd30dc" }
                    }
                }
            },
            { "custodian", new Dictionary<string, string>
                {
                    { "reference", "Organization/"+ organizationSequenceNumber },
                    { "display", organizationName }
                }
            },
            { "section", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "title", "Summary" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "SUM" }
                                        }
                                    }
                                },
                                { "text", "Summary" }
                            }
                        },
                        { "text", new Dictionary<string, string>
                            {
                                { "status", "generated" },
                                { "div", htmlDiv }
                            }
                        }
                    }
                }
            }
        };


                return composition;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion


        #endregion

    }
}
