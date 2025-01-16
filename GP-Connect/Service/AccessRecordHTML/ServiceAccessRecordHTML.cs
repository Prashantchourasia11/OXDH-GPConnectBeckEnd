using AngleSharp.Text;
using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.FHIR_JSON.AccessHTML;
using GP_Connect.Service.CommonMethods;
using Microsoft.SharePoint.News.DataModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Nancy.Json;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace GP_Connect.Service.AccessRecordHTML
{
    public class ServiceAccessRecordHTML : IServiceAccessRecordHTML
    {
        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();
        BannerDTO bannerDTO = new BannerDTO();

        public static string authorSequenceNumber = "";

        #endregion

        #region Constructor

        public ServiceAccessRecordHTML()
        {
            _crmServiceClient = crmCon.crmconnectionOXVC();
        }

        #endregion

        #region Method

        public dynamic GetAccessHTMLRecord(RequestAccessHTMLDTO htmlDetails)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                AccessHTMLDetails ahd = new AccessHTMLDetails();

                if (htmlDetails.resourceType == null )
                {
                    finaljson[0] = ahd.BADREQUESTJSON("Failed to parse request body as JSON resource. Error was: Failed to parse JSON content, error was: Did not find any content to parse");
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }

                if (htmlDetails.resourceType != "Parameters")
                {
                    finaljson[0] = ahd.INVALIDRESOURCEJSON("Resource Type is Invalid.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                var timePeriod = false;
                var startTime = "";
                var endTime = "";
                var nhsNumber = "";
                var code = "";
                var nhsNumberExist = false;
                var recordExist = false;
                var timePeriodExist = false;

                if (htmlDetails.parameter.Count == 1)
                {
                    if (htmlDetails.parameter[0].name != "patientNHSNumber")
                    {
                        finaljson[0] = ahd.INVALIDNHSNUMBERJSON("NHS number in body doesn't match the header");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                    if (htmlDetails.parameter[0].name != "recordSection")
                    {
                        finaljson[0] = ahd.BADREQUESTJSON("Invalid Parameter");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }
                }

                

                for(var i=0;i< htmlDetails.parameter.Count;i++)
                {
                    if (htmlDetails.parameter[i].name != "patientNHSNumber")
                    {
                        if(i == htmlDetails.parameter.Count-1 && nhsNumberExist == false)
                        {
                            finaljson[0] = ahd.INVALIDPARAMETERJSON("Missing Parameter : patientNHSNumber ");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                        
                    }
                    else
                    {
                        if (htmlDetails.parameter[i].name == "patientNHSNumber")
                        {
                            nhsNumber = htmlDetails.parameter[i].valueIdentifier.value;
                            if(nhsNumber == "")
                            {
                                finaljson[0] = ahd.INVALIDNHSNUMBERJSON("NHS number is invalid.");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }

                            if (nhsNumberExist == true || nhsNumber == "1234567891")
                            {
                                finaljson[0] = ahd.INVALIDNHSNUMBERJSON("NHS number is invalid.");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }

                            nhsNumberExist = true;
                            
                          
                            if(htmlDetails.parameter[i].valueIdentifier.system != "http://fhir.nhs.net/Id/nhs-number")
                            {
                                finaljson[0] = ahd.INVALIDNHSNUMBERJSON("NHS number in body doesn't match the header");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }
                        }
                            
                    }
                }

                for (var i = 0; i < htmlDetails.parameter.Count; i++)
                {
                    if (htmlDetails.parameter[i].name != "recordSection")
                    {
                        if (i == htmlDetails.parameter.Count - 1 && recordExist == false)
                        {
                            finaljson[0] = ahd.INVALIDPARAMETERJSON("Missing Parameter : recordSection ");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }
                    else
                    {
                        if(htmlDetails.parameter[i].name == "recordSection")
                        {
                            if(recordExist == true)
                            {
                                finaljson[0] = ahd.BADREQUESTJSON("Multiple Sections Added");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }

                            recordExist = true;
                            if (htmlDetails.parameter[i].valueCodeableConcept.coding[0].system == "")
                            {
                                finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid System");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                            if (htmlDetails.parameter[i].valueCodeableConcept.coding[0].system != "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1")
                            {
                                finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid System");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }
                            code = htmlDetails.parameter[i].valueCodeableConcept.coding[0].code;
                        }
                      
                    }
                }
            

               if(htmlDetails.parameter.Count > 2)
                {
                    for (var i = 0; i < htmlDetails.parameter.Count; i++)
                    {
                        if (htmlDetails.parameter[i].name != "timePeriod")
                        {
                            if (i == htmlDetails.parameter.Count - 1 && timePeriodExist == false)
                            {
                                finaljson[0] = ahd.INVALIDPARAMETERJSON("Missing Parameter : timePeriod ");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                            else
                            {
                                if (htmlDetails.parameter[i].name == "timePeriod")
                                {
                                    timePeriodExist = true;
                                }
                            }
                        }
                        else
                        {
                            if (htmlDetails.parameter[i].name == "timePeriod")
                            {
                                timePeriodExist = true;
                            }
                        }
                    }
                   
                }

                if (htmlDetails.parameter != null)
                {
                    
                if (htmlDetails.parameter.Count > 2)
                {
                    var nhsnumbercheck = false;
                    var recordSection = false;

                    for(var i=0;i< htmlDetails.parameter.Count;i++)
                    {
                        if (htmlDetails.parameter[i].name == "patientNHSNumber")
                        {
                            
                            if (nhsnumbercheck == true)
                            {
                                finaljson[0] = ahd.INVALIDIDENTIFIERSYSTEMJSON("NHS Number Invalid");
                                finaljson[1] = "";
                                finaljson[2] = "400";
                                return finaljson;
                            }
                            else
                            {
                                nhsnumbercheck = true;
                            }
                               
                        }
                           
                            if (htmlDetails.parameter[i].name == "timePeriod")
                        {
                                timePeriod = true;
                                startTime = htmlDetails.parameter[i].valuePeriod.start;
                                endTime = htmlDetails.parameter[i].valuePeriod.end;
                                if(startTime != null)
                                {
                                    startTime = ConvertToProperDateFormat(startTime,"start");
                                    if(startTime == "Invalid_date_format")
                                    {
                                        finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid Date Format Parameter");
                                        finaljson[1] = "";
                                        finaljson[2] = "422";
                                        return finaljson;
                                    }
                                }
                                else
                                {
                                    startTime = "";
                                }

                                if(endTime != null)
                                {
                                    endTime = ConvertToProperDateFormat(endTime,"end");
                                    if (endTime == "Invalid_date_format")
                                    {
                                        finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid Date Format Parameter");
                                        finaljson[1] = "";
                                        finaljson[2] = "422";
                                        return finaljson;
                                    }
                                }
                                else
                                {
                                    endTime = "";
                                }
                                
                                if(startTime != "" && endTime != "")
                                {
                                    DateTime startDate = DateTime.Parse(startTime);
                                    DateTime endDate = DateTime.Parse(endTime);

                                    if (startDate > endDate)
                                    {
                                        finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid Dates.");
                                        finaljson[1] = "";
                                        finaljson[2] = "422";
                                        return finaljson;
                                    }
                                  
                                }
                                

                        }
                    }


                }
                }


              

                if (code == "ALL" || code == "IMM" || code == "SUM")
                {
                    if (timePeriod == true)
                    {
                        finaljson[0] = ahd.BADREQUESTJSON("timePeriod isn't a valid param for the section");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }
                }

                if (code == "SUM" || code == "ENC" || code == "CLI" || code == "PRB" ||
                     code == "ALL" || code == "MED" || code == "REF" || code == "OBS" || code == "IMM" || code == "ADM")
                {

                }
                else
                {
                    finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid Section Code : "+code);
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                ResponseAccessHTML finalResponse = new ResponseAccessHTML();
                finalResponse.resourceType = "Bundle";
                finalResponse.type = "searchset";
                finalResponse.id =  Guid.NewGuid().ToString();

                List<object> htmlRecordList = new List<object>(); 

              

                var consent = CheckPatientConsent(nhsNumber);
                if (consent[0] == null || consent[1] == null || consent[1] == "" || consent[3] == "True" || consent[2] == "deceased_moreThan_30days")
                {
                    finaljson[0] = ahd.PATIENTNOTFOUNDJSON("Patient Record Not Found.");
                    finaljson[1] = "";
                    finaljson[2] = "404";
                    return finaljson;
                }

            
                if(consent[0] == "No" || consent[0] == "Ask")
                {
                    finaljson[0] = ahd.NOPATIENTCONSENTJSON("Patient has not consented to share.");
                    finaljson[1] = "";
                    finaljson[2] = "403";
                    return finaljson;
                }

                ServiceCommonMethod scm = new ServiceCommonMethod();
                var patientDetails = scm.GetAllDetailsOfPatientByPatientIdUsedForHTMLACCESS(nhsNumber);
                bannerDTO = GetAllBannerContent(nhsNumber);

                finalResponse.entry = patientDetails;

                authorSequenceNumber = scm.GetPractitionerSeqNumberByNHSnumber(nhsNumber);

                //var device = MakeDeviceJSON();
                //finalResponse.entry.Add(ConvertToResource(device));




                if (code == "SUM")
                {
                    var res = makeSummaryObject(nhsNumber);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if(code == "ENC")
                {
                    var res = GetEncounterDetails(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "CLI")
                {
                    var res = makeClinicalItem(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "PRB")
                {
                    var res = GetObjectForProblemAndIssue(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "ALL")
                {
                    var res = GetCurrentAndResolvedAllergy(nhsNumber);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "MED")
                {
                    var res = makeMedicationObject(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "REF")
                {
                    var res = GetReferralObject(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "OBS")
                {
                    var res = GetObservationObject(nhsNumber,startTime,endTime);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "IMM")
                {
                    var res = MakeImmunizationObject(nhsNumber);
                    if (res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else if (code == "ADM")
                {
                    var res = MakeobjectForAdministractiveItem(nhsNumber,startTime,endTime);
                    if(res.ToString() != "System.Object")
                    {
                        finalResponse.entry.Add(ConvertToResource(res));
                    }
                }
                else
                {
                    finaljson[0] = ahd.INVALIDPARAMETERJSON("Invalid section"); ;
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }




                if (finalResponse.entry.Count == 4)
                {
                    object TempCom = finalResponse.entry[3];
                    object TempPatient = finalResponse.entry[0];

                    finalResponse.entry[0] = TempCom;
                    finalResponse.entry[3] = TempPatient;
                }
               
                var checkLastData = new JavaScriptSerializer().Serialize(finalResponse);

                finaljson[0] = finalResponse;
                finaljson[1] = "";
                finaljson[2] = "200";
                return finaljson;
              
            }
            catch(Exception ex)
            {
                dynamic[] finaljson = new dynamic[3];
                AccessHTMLDetails ahd = new AccessHTMLDetails();
                finaljson[0] = ahd.BADREQUESTJSON("Failed to parse request body as JSON resource. Error was: Failed to parse JSON content, error was: Did not find any content to parse");
                finaljson[1] = "";
                finaljson[2] = "400";
                return finaljson;
            }
        }





        #endregion

        #region Internal Method

        #region Check-patientConsent

        internal dynamic CheckPatientConsent(string nhsNumber)
        {
            dynamic[] finaljson = new dynamic[4];

            var ConsentXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                         <entity name='contact'>
                         <attribute name='fullname'/>
                         <attribute name='telephone1'/>
                         <attribute name='contactid'/>
                         <attribute name='bcrm_patientconsent'/>
                         <attribute name='bcrm_pdsjson'/>
                          <attribute name='bcrm_deceaseddate'/>
                          <attribute name='bcrm_sensitivepatient'/>
                         <order attribute='fullname' descending='false'/>
                         <filter type='and'>
                         <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"'/>
                         </filter>
                         </entity>
                         </fetch>";
            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(ConsentXML));
            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                var record = AnswerCollection.Entities[0];
                if(record.Attributes.Contains("bcrm_patientconsent"))
                {
                    finaljson[0] = record.Attributes.Contains("bcrm_patientconsent") ? record.FormattedValues["bcrm_patientconsent"].ToString() : "";
                }
                if (record.Attributes.Contains("bcrm_pdsjson"))
                {
                    finaljson[1] = record.Attributes.Contains("bcrm_pdsjson") ? record["bcrm_pdsjson"].ToString() : "";
                }
                if (record.Attributes.Contains("bcrm_deceaseddate")) 
                {
                    var deceasedDate = (DateTime)record.Attributes["bcrm_deceaseddate"]; 
                    if(deceasedDate > DateTime.UtcNow.AddDays(-28))
                    {
                        finaljson[2] = "deceased_withIn_30days";
                    }
                    else
                    {
                        finaljson[2] = "deceased_moreThan_30days";
                    }
                }
                if (record.Attributes.Contains("bcrm_sensitivepatient"))
                {
                    finaljson[3] = record.Attributes.Contains("bcrm_sensitivepatient") ? record["bcrm_sensitivepatient"].ToString() : "";
                }
            }
            return finaljson;
        }

        #endregion

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
                                     <attribute name='bcrm_enddateday' />
                                     <attribute name='bcrm_enddatemonth' />
                                     <attribute name='bcrm_enddateyear' />
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
                        var endDay = record.Attributes.Contains("bcrm_enddateday") ? record["bcrm_enddateday"].ToString() : "";
                        var endMonth = record.Attributes.Contains("bcrm_enddatemonth") ? record["bcrm_enddatemonth"].ToString() : "";
                        var endYear = record.Attributes.Contains("bcrm_enddateyear") ? record["bcrm_enddateyear"].ToString() : "";

                        if (record.Attributes.Contains("bcrm_enddateday") &&
                            record.Attributes.Contains("bcrm_enddatemonth") &&
                            record.Attributes.Contains("bcrm_enddateyear"))
                        {
                            int day = int.Parse(record["bcrm_enddateday"].ToString());
                            int month = int.Parse(record["bcrm_enddatemonth"].ToString());
                            int year = int.Parse(record["bcrm_enddateyear"].ToString());

                            allergyRecord.endDate = new DateTime(year, month, day);
                        }


                        allergyRecord.allergyName = record.Attributes.Contains("msemr_name") ? record["msemr_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_asserteddate")) { allergyRecord.startDate = (DateTime)record.Attributes["bcrm_asserteddate"]; }
                       
                        allergyRecord.allergyStatus = record.Attributes.Contains("statecode") ? record.FormattedValues["statecode"].ToString() : "";
                        allergyList.Add(allergyRecord);
                    }
                }
                if (allergyList.Count > 0)
                {
                    var activeAllergyDiv = makeActiveAllergyList(allergyList);
                    var inActiveAllergyDiv = makeInActiveAllergyList(allergyList);

                    var finalAllergyString = "<div xmlns=\"http://www.w3.org/1999/xhtml\"> <h1>Allergies and Adverse Reactions</h1> " + bannerDTO.GpTransferBanner + bannerDTO.AllergiesandAdverseReactionsContentBanner + " <div> <h2>Current Allergies and Adverse Reactions</h2>  "+bannerDTO.CurrentAllergiesandAdverseReactionsContentBanner+ bannerDTO.CurrentAllergiesandAdverseReactionsExclusiveBanner +" <table id=\"all-tab-curr\"> <thead> <tr> <th>Start Date</th> <th>Details</th> </tr> </thead> <tbody> " + activeAllergyDiv + " </tbody>  </table> </div> <div> <h2>Historical Allergies and Adverse Reactions</h2>  "+bannerDTO.HistoricalAllergiesandAdverseReactionsContentBanner+ bannerDTO.HistoricalAllergiesandAdverseReactionsExclusiveBanner +" <table id=\"all-tab-hist\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Details</th> </tr> </thead> <tbody> " + inActiveAllergyDiv + " </tbody> </table> </div> </div>";

                    var allergyJSON = MakeAllergyCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, finalAllergyString);
                    return allergyJSON;

                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];
                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\">"+bannerDTO.GpTransferBanner+"<h1>Allergies and Adverse Reactions</h1><div><h2>Current Allergies and Adverse Reactions</h2><p>No 'Current Allergies and Adverse Reactions' data is recorded for this patient.</p></div><div><h2>Historical Allergies and Adverse Reactions</h2><p>No 'Historical Allergies and Adverse Reactions' data is recorded for this patient.</p></div></div>";
                    var finalObj = MakeAllergyCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }

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
                        htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
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
                // Sort the list by endDate descending
                allergyList = allergyList.OrderByDescending(item => item.endDate).ToList();

                var htmlContent = "";
                foreach (var item in allergyList)
                {
                    if (item.allergyStatus.ToLower().ToString() == "inactive")
                    {
                        htmlContent += "<tr>";
                        htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")  },
             { "type", new Dictionary<string, object>
             {
             { "coding", new List<Dictionary<string, string>>
                 {
                     new Dictionary<string, string>
                     {
                         { "system", "http://snomed.info/sct" },
                         { "code", "425173008" },
                         { "display", "record extract" }
                     }
                 }
             },
             { "text", "record extract" }
             }
             },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                        { "title", "Allergies and Adverse Reactions" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "ALL" },
                                            { "display", "Allergies and Adverse Reactions" }
                                        }
                                    }
                                },
                                { "text", "Allergies and Adverse Reactions" }
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
        internal object GetEncounterDetails(string nhsNumber, string startDate, string endDate)
        {
            try
            {
             
              
                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }

                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if (startDate != "")
                {
                    filterstring += "<condition attribute='msemr_encounterstartdate' operator='on-or-after' value='" + startDate + @"' />";
                }
                if (endDate != "")
                {
                    filterstring += "<condition attribute='msemr_encounterstartdate' operator='on-or-before' value='" + endDate + @"' />";
                }

                var encounterXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='msemr_encounter'>
                                  <attribute name='msemr_encounterid' />
                                  <attribute name='msemr_name' />
                                  <attribute name='msemr_encounterstartdate' />
                                   <attribute name='bcrm_details' />
                                  <attribute name='createdon' />
                                     <filter type='and'>
                                      "+filterstring+@"
                                    </filter>
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
                    var divString = CreateEncounterHTMLDivByJSON( nhsNumber ,encounterDetailsList,startDate,endDate);
                    var finalobject = CreateEncounterJSONByUsingRecord(patientSequenceNumber, organizationSequenceNumber, organizationName, divString);
                    return finalobject;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];

                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"> "+ bannerDTO.GpTransferBanner + bannerDTO.EncounterContentBanner + datefilterBanner + bannerDTO.EncounterExclusiveBanner + "   <p>No 'Encounters' data is recorded for this patient.</p></div>";
                    var finalObj = CreateEncounterJSONByUsingRecord(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
                
            }
            catch (Exception ex)
            {
                return new object();
            }
        }
        internal string CreateEncounterHTMLDivByJSON(string nhsNumber,List<EncounterDetailsDTO> encounterList,string startDate,string endDate)
        {
         

            var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

            if (startDate != "" && endDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }
            else if (startDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }
            else if (endDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }
            var tableTD = "";
            foreach (var item in encounterList)
            {
                tableTD += "<tr>";
                tableTD += "<td class=\"date-column\">" + item.Date.ToString("dd-MMM-yyyy") + "</td>";
                tableTD += "<td>" + item.title + "</td>";
                tableTD += "<td>" + item.details + "</td>";
                tableTD += "</tr>";
            }

            string div = "<div xmlns=\"http://www.w3.org/1999/xhtml\"> <h1>Encounters</h1> "+ bannerDTO.GpTransferBanner + bannerDTO.EncounterContentBanner + datefilterBanner+ bannerDTO.EncounterExclusiveBanner +"  <table id=\"enc-tab\"> <thead> <tr> <th>Date</th> <th>Title</th> <th>Details</th> </tr> </thead> <tbody> " + tableTD + "  </tbody> </table> </div>";
           
            if(encounterList.Count == 0 && startDate != "" && endDate != "")
            {
                div = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Encounters</h1> "+ bannerDTO.GpTransferBanner + bannerDTO.EncounterContentBanner + datefilterBanner + bannerDTO.EncounterExclusiveBanner + "<p>No 'Encounters' data is recorded for this patient.</p></div>";
            }
            else if(encounterList.Count == 0)
            {
                div = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Encounters</h1> " + bannerDTO.GpTransferBanner + bannerDTO.EncounterContentBanner + datefilterBanner + bannerDTO.EncounterExclusiveBanner + " <div><p>No 'Encounters' data is recorded for this patient.</p></div>";
            }

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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "ENC" },
                                            { "display", "Encounters" }
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

        public object GetObjectForProblemAndIssue(string nhsNumber, string startDate, string endDate)
        {
            try
            {
               
                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if (startDate != "")
                {
                    filterstring += "<condition attribute='msemr_asserteddate' operator='on-or-after' value='" + startDate + @"' />";
                }
                if (endDate != "")
                {
                    filterstring += "<condition attribute='msemr_asserteddate' operator='on-or-before' value='" + endDate + @"' />";
                }


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
                    var majorInactiveString = MakeMajorInactiveProblemAndIssueForNotSummary(problemIssueList,startDate,endDate);
                    var otherInavtiveString = MakeOtherInactiveProblemAndIssue(problemIssueList, startDate, endDate);

                    if(activeProbString == "")
                    {
                        activeProbString = "<p>No 'Active Problems and Issues' data is recorded for this patient.</p>";
                    }
                    if(majorInactiveString == "")
                    {
                        majorInactiveString = "<p>No 'Major Inactive Problems and Issues' data is recorded for this patient.</p>";
                    }
                    if(otherInavtiveString == "")
                    {
                        otherInavtiveString = "<p>No 'Other Inactive Problems and Issues' data is recorded for this patient.</p";
                    }
                    else
                    {
                        otherInavtiveString = "<table id=\"prb-tab-othinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> " + otherInavtiveString + "  </tbody> </table>";
                    }


                    var finalDiv = @"<div> <h1>Problems and Issues</h1> "+ bannerDTO.GpTransferBanner + bannerDTO.ProblemsandIssuesContentBanner + "" +
                        " <div> <h2>Active Problems and Issues</h2> "+bannerDTO.ActiveProblemsandIssuesContentBanner+" <div class=\"date-banner\"><p>Date filter not applied</p></div> "+bannerDTO.ActiveProblemsandIssuesExclusiveBanner+" <table id=\"prb-tab-act\"> <thead> <tr> <th>Start Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> " + activeProbString + " </tbody> </table>" +
                        "</div> <div> <h2>Major Inactive Problems and Issues</h2>"+ bannerDTO.MajorInactiveProblemsandIssuesContentBanner + datefilterBanner+ bannerDTO.MajorInactiveProblemsandIssuesExclusiveBanner +"  <table id=\"prb-tab-majinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> "+ majorInactiveString + " </tbody> </table>" +
                        " </div> <div> <h2>Other Inactive Problems and Issues</h2>  " + bannerDTO.OtherInactiveProblemsandIssuesContentBanner + datefilterBanner + bannerDTO.OtherInactiveProblemsandIssuesExclusiveBanner + otherInavtiveString +"  </div> </div>";
                    var res = MakeProblemAndIssueCompositionObject(patientSequenceNumber,organizationSequenceNumber,organizationName,finalDiv);
                    return res;

                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];

                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\">"+bannerDTO.GpTransferBanner+  "<h1>Problems and Issues</h1> <div><h2>Active Problems and Issues</h2>  <div class=\"date-banner\"><p>Date filter not applied</p></div><p>No 'Active Problems and Issues' data is recorded for this patient.</p></div><div><h2>Major Inactive Problems and Issues</h2>"+datefilterBanner+"<p>No 'Major Inactive Problems and Issues' data is recorded for this patient.</p></div><div><h2>Other Inactive Problems and Issues</h2>"+datefilterBanner+"<p>No 'Other Inactive Problems and Issues' data is recorded for this patient.</p></div></div>";
                    var finalObj = MakeProblemAndIssueCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                    
                }

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
                        htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
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
                        htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                        htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
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


        public string MakeMajorInactiveProblemAndIssueForNotSummary(List<ProblemAndIssueDTO> problemIssueList,string startDate,string endDate)
        {
            try
            {
                // Sort the list by endDate descending
                problemIssueList = problemIssueList.OrderByDescending(item => item.endDate).ToList();

                var htmlContent = "";
                foreach (var item in problemIssueList)
                {
                    if(startDate != "")
                    {
                        if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() == "major" && item.startDate > DateTime.Parse(startDate) && item.startDate < DateTime.Parse(endDate))
                        {
                            htmlContent += "<tr>";
                            htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td>" + item.entry + "</td>";
                            htmlContent += "<td>" + item.significance + "</td>";
                            htmlContent += "<td>" + item.details + "</td>";
                            htmlContent += "</tr>";
                        }
                    }
                    else
                    {
                        if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() == "major")
                        {
                            htmlContent += "<tr>";
                            htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td>" + item.entry + "</td>";
                            htmlContent += "<td>" + item.significance + "</td>";
                            htmlContent += "<td>" + item.details + "</td>";
                            htmlContent += "</tr>";
                        }
                    }
                  
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string MakeOtherInactiveProblemAndIssue(List<ProblemAndIssueDTO> problemIssueList,string startDate,string endDate)
        {
            try
            {
                // Sort the list by endDate descending
                problemIssueList = problemIssueList.OrderByDescending(item => item.endDate).ToList();


                var htmlContent = "";
                foreach (var item in problemIssueList)
                {
                    if(startDate != "")
                    {
                        if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() != "major" && item.startDate > DateTime.Parse(startDate) && item.startDate < DateTime.Parse(endDate))
                        {
                            htmlContent += "<tr>";
                            htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td>" + item.entry + "</td>";
                            htmlContent += "<td>" + item.significance + "</td>";
                            htmlContent += "<td>" + item.details + "</td>";
                            htmlContent += "</tr>";
                        }
                    }
                    else
                    {
                        if (item.status.ToLower().ToString() == "inactive" && item.significance.ToLower().ToString() != "major")
                        {
                            htmlContent += "<tr>";
                            htmlContent += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td class=\"date-column\">" + item.endDate.ToString("dd-MMM-yyyy") + "</td>";
                            htmlContent += "<td>" + item.entry + "</td>";
                            htmlContent += "<td>" + item.significance + "</td>";
                            htmlContent += "<td>" + item.details + "</td>";
                            htmlContent += "</tr>";
                        }
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                        { "title", "Problems and Issues" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "PRB" },
                                            { "display", "Problems and Issues" }
                                        }
                                    }
                                },
                                { "text", "Problems and Issues" }
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

        internal object GetReferralObject(string nhsNumber, string startDate, string endDate)
        {
            try
            {
               

                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }

                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if (startDate != "")
                {
                    filterstring += "<condition attribute='createdon' operator='on-or-after' value='" + startDate + @"' />";
                }
                if (endDate != "")
                {
                    filterstring += "<condition attribute='createdon' operator='on-or-before' value='" + endDate + @"' />";
                }


                var referraalXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='msemr_referralrequest'>
                                      <attribute name='msemr_referralrequestid' />
                                      <attribute name='msemr_name' />
                                      <attribute name='bcrm_toreferraldoctor' />
                                      <attribute name='bcrm_priority' />
                                      <attribute name='msemr_description' />
                                      <attribute name='createdon' />
                                      <order attribute='createdon' descending='true' />
                                      <filter type='and'>
                                         "+filterstring+@"
                                      </filter>
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
                    var htmlcontent = "<div> <h1>Referrals</h1> "+bannerDTO.GpTransferBanner + bannerDTO.ReferralsContentBanner + datefilterBanner + bannerDTO.ReferralsExclusiveBanner + "   <table id=\"ref-tab\"> <thead> <tr> <th>Date</th> <th>From</th> <th>To</th> <th>Priority</th> <th>Details</th> </tr> </thead> <tbody> " + res+" </tbody> </table> </div>";
                    var finalObj = MakeReferralCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];

                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Referrals</h1>  "+ bannerDTO.GpTransferBanner + datefilterBanner + " <p>No 'Referrals' data is recorded for this patient.</p></div>";
                    var finalObj = MakeReferralCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                    
                }
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
                    htmlContent += "<td class=\"date-column\">" + item.createdon.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "REF" },
                                             { "display", "Referrals" }
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

        public object GetObservationObject(string nhsNumber, string startDate, string endDate)
        {
          

            var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

            if (startDate != "" && endDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }
            else if (startDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }
            else if (endDate != "")
            {
                datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
            }

            var patientSequenceNumber = "";
            var organizationSequenceNumber = "";
            var organizationName = "";

            var filterstring = "";
            if (startDate != "")
            {
                filterstring += "<condition attribute='createdon' operator='on-or-after' value='" + startDate + @"' />";
            }
            if (endDate != "")
            {
                filterstring += "<condition attribute='createdon' operator='on-or-before' value='" + endDate + @"' />";
            }


            var obsXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                           <entity name='msemr_observation'>
                             <attribute name='msemr_observationid' />
                             <attribute name='msemr_description' />
                             <attribute name='createdon' />
                             <attribute name='bcrm_title' />
                             <attribute name='msemr_valuestring' />
                             <attribute name='msemr_valuerangehighlimit' />
                             <attribute name='msemr_description' />
                                <filter type='and'>
                                 "+filterstring+@"      
                               </filter>
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
               var htmlContent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"> <h1>Observations</h1> " + bannerDTO.GpTransferBanner + bannerDTO.ObservationsContentBanner +datefilterBanner+ bannerDTO.ObservationsExclusiveBanner+"  <table id=\"obs-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Value</th> <th>Range</th> <th>Details</th> </tr> </thead> <tbody> "+res+" </tbody> </table> </div>";

               var finalObj = MakeObservationObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlContent);
               return finalObj;

            }
            else
            {
                ServiceCommonMethod scm = new ServiceCommonMethod();
                var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                patientSequenceNumber = patientDetails[0];
                organizationName = patientDetails[1];
                organizationSequenceNumber = patientDetails[2];

                var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Observations</h1> " + bannerDTO.GpTransferBanner + bannerDTO.ObservationsContentBanner + datefilterBanner + bannerDTO.ObservationsExclusiveBanner + " <p>No 'Observations' data is recorded for this patient.</p></div>";
                var finalObj = MakeObservationObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                return finalObj;
                
            }
        }

        public string makeObservationhtmlContents(List<ObservationDTO> observationList)
        {
            var htmlContent = "";
            foreach (var item in observationList)
            {
                
                htmlContent += "<tr>";
                htmlContent += "<td class=\"date-column\">" + item.createdOn.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "OBS" },
                                             { "display", "Observations" }
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
                    var htmlContent = "<div> <h1>Immunisations</h1> "+bannerDTO.GpTransferBanner + bannerDTO.ImmunisationsContentBanner + bannerDTO.ImmunisationsExclusiveBanner +"  <table id=\"imm-tab\"> <thead> <tr> <th>Date</th> <th>Vaccination</th> <th>Part</th> <th>Contents</th> <th>Details</th> </tr> </thead> <tbody> "+ res + " </tbody> </table> </div>";    
                    var finalObj = MakeImmunizationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlContent);
                    return finalObj;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];
                    var htmlContent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Immunisations</h1> "+bannerDTO.GpTransferBanner+ bannerDTO.ImmunisationsContentBanner + bannerDTO.ImmunisationsExclusiveBanner+ " <div class=\"exclusion-banner\"> <p> Items excluded due to confidentiality and/or patient preferences. </p> </div> <p>No 'Immunisations' data is recorded for this patient.</p></div>";
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
                    htmlContent += "<td class=\"date-column\">" + item.RecDate.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "IMM" },
                                            { "display", "Immunisations" }
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

        internal object MakeobjectForAdministractiveItem(string nhsNumber,string startDate,string endDate)
        {
            try
            {
             

                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") +"' to " + "'"+ DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") +"'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") +"'</p> </div>";
                }

                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if(startDate != "")
                {
                    filterstring += "<condition attribute='bcrm_recordeddate' operator='on-or-after' value='" + startDate+@"' />";
                }
                if(endDate != "")
                {
                    filterstring += "<condition attribute='bcrm_recordeddate' operator='on-or-before' value='" + endDate + @"' />";
                }

                var administractorXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='bcrm_administractiveitem'>
                                          <attribute name='bcrm_administractiveitemid' />
                                          <attribute name='bcrm_name' />
                                          <attribute name='createdon' />
                                           <attribute name='bcrm_recordeddate' />
                                           <attribute name='bcrm_description' />
                                           <filter type='and'>
                                                "+filterstring+@"
                                            </filter>
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
                    var htmlcontent = "<div> <h1>Administrative Items</h1> "+ bannerDTO.GpTransferBanner + bannerDTO.AdministrativeItemsContentBanner + datefilterBanner  + bannerDTO.AdministrativeItemsExclusiveBanner +"  <table id=\"adm-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> </tr> </thead> <tbody> "+res+" </tbody> </table> </div>";
                    //  var htmlcontent = "<div> <h1>Administrative Items</h1>  "+ datefilterBanner + " <table id=\"adm-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> </tr> </thead> <tbody> " + res + " </tbody> </table> </div>";
                   // var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Administrative Items</h1>"+datefilterBanner+"<table id=\"adm-tab\"><thead><tr><th>Date</th><th>Entry</th><th>Details</th></tr></thead><tbody>"+res+"</tbody></table></div>";
                    var finalObj = MakeAdministractorCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];
                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Administrative Items</h1>"+ bannerDTO.GpTransferBanner + datefilterBanner+"  <p>No 'Administrative Items' data is recorded for this patient.</p></div>";
                    var finalObj = MakeAdministractorCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
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
                    htmlContent += "<td class=\"date-column\">" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                        { "title", "Administrative Items" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "ADM" },
                                            { "display", "Administrative Items" }
                                        }
                                    }
                                },
                                { "text", "Administrative Items" }
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

        internal object makeClinicalItem(string nhsNumber, string startDate, string endDate)
        {
            try
            {
              

                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }

                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if (startDate != "")
                {
                    filterstring += "<condition attribute='bcrm_recordeddate' operator='on-or-after' value='" + startDate + @"' />";
                }
                if (endDate != "")
                {
                    filterstring += "<condition attribute='bcrm_recordeddate' operator='on-or-before' value='" + endDate + @"' />";
                }

                var clinicalItemXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='bcrm_observationrequest'>
                                          <attribute name='bcrm_observationrequestid' />
                                          <attribute name='bcrm_name' />
                                          <attribute name='createdon' />
                                         <attribute name='bcrm_description' />
                                         <attribute name='bcrm_recordeddate' />
                                            <filter type='and'>
                                                "+filterstring+@"
                                            </filter>
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
                    var htmlcontent = "<div> <h1>Clinical Items</h1> " + bannerDTO.GpTransferBanner + bannerDTO.ClinicalItemContentBanner + datefilterBanner + bannerDTO.ClinicalItemExclusiveBanner + "<table id=\"cli-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> </tr> </thead> <tbody> " +res+" </tbody> </table> </div>";
                    var finalObj = MakeClinicalItemCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];
                    var htmlcontent = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Clinical Items</h1> " + bannerDTO.GpTransferBanner + bannerDTO.ClinicalItemContentBanner + datefilterBanner + bannerDTO.ClinicalItemExclusiveBanner + "< p>No 'Clinical Items' data is recorded for this patient.</p></div>";
                    var finalObj = MakeClinicalItemCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontent);
                    return finalObj;
                }
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
                        htmlContent += "<td class=\"date-column\">" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                        { "title", "Clinical Items" },
                        { "code", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, string>>
                                    {
                                        new Dictionary<string, string>
                                        {
                                            { "system", "http://fhir.nhs.net/ValueSet/gpconnect-record-section-1" },
                                            { "code", "CLI" },
                                            { "display", "Clinical Items" }
                                        }
                                    }
                                },
                                { "text", "Clinical Items" }
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

        internal object makeMedicationObject(string nhsNumber, string startDate, string endDate)
        {
            try
            {
               

                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }

                var patientSequenceNumber = "";
                var organizationSequenceNumber = "";
                var organizationName = "";

                var filterstring = "";
                if (startDate != "")
                {
                    filterstring += "<condition attribute='bcrm_drugstartdate' operator='on-or-after' value='" + startDate + @"' />";
                }
                if (endDate != "")
                {
                    filterstring += "<condition attribute='bcrm_drugstartdate' operator='on-or-before' value='" + endDate + @"' />";
                }

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
                                        <attribute name='bcrm_dosageunit' />
                                        <attribute name='bcrm_expectedsupplyvalue' />
                                        <attribute name='bcrm_additionalinformation' />
                                        <attribute name='bcrm_lastissueddate' />
                                        <attribute name='bcrm_numberofprescriptionsissued' />
                                        <attribute name='bcrm_maxissues' />
                                        <attribute name='bcrm_reviewdate' />
                                        <attribute name='bcrm_prescribingagencytype' />
                                        <attribute name='bcrm_lastauthoriseddate' />
                                        <attribute name='bcrm_medicationcancelledreason' />
                                        <attribute name='bcrm_medicationcancelleddate' />
                                        <attribute name='bcrm_linkedproblem' />
                                        <attribute name='bcrm_reasonforthemedication' />
                                        <attribute name='bcrm_othersupportinginformation' />
                                        <attribute name='bcrm_controlleddrug' />
                                        <attribute name='bcrm_discontinueddate' />
                                        <attribute name='bcrm_discontinuationreason' />
                                        <order attribute='bcrm_drugstartdate' descending='true' />
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

                        var doseUnit = record.Attributes.Contains("bcrm_dosageunit") ? record.FormattedValues["bcrm_dosageunit"].ToString() : string.Empty;
                        medicationRecord.MedicationItem = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_drugstartdate")) { medicationRecord.startDate = (DateTime)record.Attributes["bcrm_drugstartdate"]; }

                        if(medicationRecord.startDate.Year == 1)
                        {
                            if (record.Attributes.Contains("createdon")) { medicationRecord.startDate = (DateTime)record.Attributes["createdon"]; }
                        }

                        medicationRecord.type = record.Attributes.Contains("bcrm_type") ? record.FormattedValues["bcrm_type"].ToString() : string.Empty;
                        medicationRecord.DosageInstruction = record.Attributes.Contains("bcrm_directions") ? record["bcrm_directions"].ToString() : string.Empty;
                        medicationRecord.Quantity = record.Attributes.Contains("bcrm_quantityvalue") ? record["bcrm_quantityvalue"].ToString() : string.Empty;

                        medicationRecord.PrescribingAgencyType = record.Attributes.Contains("bcrm_prescribingagencytype") ? record["bcrm_prescribingagencytype"].ToString() : string.Empty;
                        medicationRecord.MedicationCancelledReason = record.Attributes.Contains("bcrm_medicationcancelledreason") ? record["bcrm_medicationcancelledreason"].ToString() : string.Empty;
                        medicationRecord.linkedProblem = record.Attributes.Contains("bcrm_linkedproblem") ? record["bcrm_linkedproblem"].ToString() : string.Empty;
                        medicationRecord.OtherSupportingInformation = record.Attributes.Contains("bcrm_othersupportinginformation") ? record["bcrm_othersupportinginformation"].ToString() : string.Empty;
                        medicationRecord.ReasonForMedication = record.Attributes.Contains("bcrm_reasonforthemedication") ? record["bcrm_reasonforthemedication"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_lastauthoriseddate")) { medicationRecord.LastAutorizedDate = (DateTime)record.Attributes["bcrm_lastauthoriseddate"]; }
                        if (record.Attributes.Contains("bcrm_medicationcancelleddate")) { medicationRecord.Medicationcancelleddate = (DateTime)record.Attributes["bcrm_medicationcancelleddate"]; }

                        if (medicationRecord.Quantity != string.Empty)
                        {
                            medicationRecord.Quantity = medicationRecord.Quantity +" "+ GetMeasuringQuantityDrugUsingName(medicationRecord.MedicationItem);
                        }

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
                    var htmlcontenmt = createHtmlcontentofMedication(nhsNumber ,medicationList,startDate,endDate);
                    var finalObj = MakeMedicationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontenmt);
                    return finalObj;
                }
                else
                {
                    ServiceCommonMethod scm = new ServiceCommonMethod();
                    var patientDetails = scm.GetAllDetailsByNHSNumber(nhsNumber);
                    patientSequenceNumber = patientDetails[0];
                    organizationName = patientDetails[1];
                    organizationSequenceNumber = patientDetails[2];

                    var htmlcontenmt = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Medications</h1> "+bannerDTO.GpTransferBanner +"<div><h2>Acute Medication (Last 12 Months)</h2><div class=\"date-banner\"><p>All relevant items</p></div><p>No 'Acute Medication (Last 12 Months)' data is recorded for this patient.</p></div><div><h2>Current Repeat Medication</h2><div class=\"date-banner\"><p>All relevant items</p></div><p>No 'Current Repeat Medication' data is recorded for this patient.</p></div><div><h2>Discontinued Repeat Medication</h2><div class=\"content-banner\"><p>All repeat medication ended by a clinician action</p></div><div class=\"date-banner\"><p>Date filter not applied</p></div><p>No 'Discontinued Repeat Medication' data is recorded for this patient.</p></div><div><h2>All Medication</h2><div class=\"date-banner\"><p>All relevant items</p></div><p>No 'All Medication' data is recorded for this patient.</p></div><div><h2>All Medication Issues</h2><div class=\"date-banner\"><p>All relevant items</p></div><p>No 'All Medication Issues' data is recorded for this patient.</p></div></div>";
                    var finalObj = MakeMedicationCompositionObject(patientSequenceNumber, organizationSequenceNumber, organizationName, htmlcontenmt);
                    return finalObj;
                }

            }
            catch (Exception) 
            {
             return new object();
            }
        }

        internal string createHtmlcontentofMedication(string nhsNumber, List<MedicationDTO> medicationList,string startDate,string endDate)
        {
            try
            {
               foreach(var item in medicationList)
               {
                    if(item.ControlledDrug != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> CONTROLLED DRUG : </b>" + item.ControlledDrug + " <br>";
                    }
                    if (item.type.ToString().ToLower() == "acute" && item.MedicationCancelledReason != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> CANCELLED DATE : </b>" + item.Medicationcancelleddate.ToString("dd-MMM-yyyy") + " CANCELLED REASON : "+item.MedicationCancelledReason+" <br>";
                    }
                    if (item.ReasonForMedication != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> Reason for Medication : </b>" + item.ReasonForMedication + " <br>";
                    }
                    if (item.linkedProblem != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> Linked Problem : </b>" + item.linkedProblem + " <br>";
                    }
                    if (item.OtherSupportingInformation != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> Other supporting information : </b>" + item.OtherSupportingInformation + " <br>";
                    }
                    if (item.type.ToString().ToLower().Contains("repeat") && item.LastAutorizedDate.Year != 1)
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> Last Authorised : </b>" + item.LastAutorizedDate.ToString("dd-MMM-yyyy") + " <br>";
                    }
                    if (item.type.ToString().ToLower().Contains("repeat") && item.NumberOfPrescriptionIsuued != "")
                    {
                        item.AdditionalInformation = item.AdditionalInformation + "<b> Number issues authorised : </b>" + item.NumberOfPrescriptionIsuued + " <br>";
                    }
                    if (item.type == "")
                    {
                        if(item.PrescribingAgencyType.ToString().ToLower().Trim() == "acute")
                        {
                            item.type = "Acute – Unknown Prescriber";
                        }
                        else if (item.PrescribingAgencyType.ToString().ToLower().Trim() == "repeat")
                        {
                            item.type = "Repeat – Unknown Prescriber";
                        }
                        else if (item.PrescribingAgencyType.ToString().ToLower().Contains("acute"))
                        {
                            item.type = item.PrescribingAgencyType;
                        }
                        else if (item.PrescribingAgencyType.ToString().ToLower().Contains("repeat"))
                        {
                            item.type = item.PrescribingAgencyType;
                        }
                        else
                        {
                            item.type = "Acute – Unknown Prescriber";
                        }
                    }
                }



                var bannerNotAppliedDefined = "<div class=\"date-banner\"><p>Date filter not applied</p></div>";
               
                var datefilterBanner = "<div class=\"date-banner\"><p>All relevant items</p></div>";

                if (startDate != "" && endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>For the period '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "' to " + "'" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (startDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items from '" + DateTime.Parse(startDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }
                else if (endDate != "")
                {
                    datefilterBanner = "<div class=\"date-banner\"> <p>Data items until '" + DateTime.Parse(endDate).ToString("dd-MMM-yyyy") + "'</p> </div>";
                }

                var acuteMedicationDiv = "";
                var repeatMedicationDiv = "";
                var discountinuedReapeatMedication = "";
                var allMedication = "";
                var allmedicationIssue = "";

                var acuteMedicationDivTable = "";
                var repeatMedicationDivTable = "";
                var discountinuedReapeatMedicationTable = "";
                var allMedicationTable = "";
                var allmedicationIssueTable = "";

               

                foreach (var item in medicationList) 
                {
                  if(item.startDate.AddDays(-1) >= DateTime.Now.AddYears(-1) && item.startDate <= DateTime.Now && item.type.ToLower().ToString().Contains("acute"))
                    {
                        acuteMedicationDiv += "<tr>";
                        acuteMedicationDiv += "<td>" + item.type + "</td>";
                        acuteMedicationDiv += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                        acuteMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                        acuteMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                        acuteMedicationDiv += "<td>" + item.Quantity + "</td>";
                        acuteMedicationDiv += "<td class=\"date-column\">" + (item.endDate.Year == 1 ? "" : item.endDate.ToString("dd-MMM-yyyy")) + "</td>";
                        acuteMedicationDiv += "<td>" + item.DaysDuration + "</td>";
                        acuteMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        acuteMedicationDiv += "</tr>";
                    }
                    if (item.type.ToLower().ToString().Contains("repeat") && item.DiscountinuedReason == string.Empty)
                    {
                        if(item.startDate.Year != 1)
                        {
                            if(item.endDate.Year == 1)
                            {
                                repeatMedicationDiv += "<tr>";
                                repeatMedicationDiv += "<td>" + item.type + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                                repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                                repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                                repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.ReviewDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.ReviewDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                repeatMedicationDiv += "</tr>";
                            }
                            else if (item.startDate <= DateTime.UtcNow && DateTime.UtcNow <= item.endDate)
                            {
                                repeatMedicationDiv += "<tr>";
                                repeatMedicationDiv += "<td>" + item.type + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                                repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                                repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                                repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                                repeatMedicationDiv += "<td class=\"date-column\">" + (item.ReviewDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.ReviewDate.ToString("dd-MMM-yyyy")) + "</td>";
                                repeatMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                repeatMedicationDiv += "</tr>";
                            }
                        }
                    }
                 
                    if(startDate != "" && endDate != "")
                    {
                        if (item.MedicationItem != string.Empty && item.startDate > DateTime.Parse(startDate) && item.startDate < DateTime.Parse(endDate))
                        {
                            

                            allMedication += "<tr>";
                            allMedication += "<td colspan='9' class='med-item-column'> <strong>" + item.MedicationItem + "</strong></td>";
                            allMedication += "</tr>";
                            allMedication += "<tr>";
                            allMedication += "<td>" + item.type + "</td>";
                            allMedication += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            allMedication += "<td>" + item.MedicationItem + "</td>";
                            allMedication += "<td>" + item.DosageInstruction + "</td>";
                            allMedication += "<td>" + item.Quantity + "</td>";
                            if (item.type.ToString().ToLower() == "acute" || item.type.ToString().ToLower() == "repeat")
                            {
                                allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            }
                            else if (item.LastIssuedDate.Year == 1)
                            {
                                allMedication += "<td></td>";
                            }
                            else
                            {
                                allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            }
                            allMedication += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                            allMedication += "<td>" + item.DiscountinuedReason + "</td>";
                            allMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            allMedication += "</tr>";
                        }
                    }
                    else
                    {
                        if(startDate != "" && endDate == "")
                        {
                            if(item.MedicationItem != string.Empty && item.startDate > DateTime.Parse(startDate) && item.startDate.Year != 1)
                            {
                                allMedication += "<tr>";
                                allMedication += "<td colspan='9' class='med-item-column'> <strong>" + item.MedicationItem + "</strong></td>";
                                allMedication += "</tr>";
                                allMedication += "<tr>";
                                allMedication += "<td>" + item.type + "</td>";
                                allMedication += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                allMedication += "<td>" + item.MedicationItem + "</td>";
                                allMedication += "<td>" + item.DosageInstruction + "</td>";
                                allMedication += "<td>" + item.Quantity + "</td>";
                                if (item.type.ToString().ToLower() == "acute" || item.type.ToString().ToLower() == "repeat")
                                {
                                    allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                }
                                else if (item.LastIssuedDate.Year == 1)
                                {
                                    allMedication += "<td></td>";
                                }
                                else
                                {
                                    allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                }
                                allMedication += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                                allMedication += "<td>" + item.DiscountinuedReason + "</td>";
                                allMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                allMedication += "</tr>";
                            }
                        }
                        else if(startDate == "" && endDate != "")
                        {
                            if (item.MedicationItem != string.Empty && item.startDate < DateTime.Parse(endDate) && item.endDate.Year != 1)
                            {
                                allMedication += "<tr>";
                                allMedication += "<td colspan='9' class='med-item-column'> <strong>" + item.MedicationItem + "</strong></td>";
                                allMedication += "</tr>";
                                allMedication += "<tr>";
                                allMedication += "<td>" + item.type + "</td>";
                                allMedication += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                allMedication += "<td>" + item.MedicationItem + "</td>";
                                allMedication += "<td>" + item.DosageInstruction + "</td>";
                                allMedication += "<td>" + item.Quantity + "</td>";
                                if (item.type.ToString().ToLower() == "acute" || item.type.ToString().ToLower() == "repeat")
                                {
                                    allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                }
                                else if (item.LastIssuedDate.Year == 1)
                                {
                                    allMedication += "<td></td>";
                                }
                                else
                                {
                                    allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                                }
                                allMedication += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                                allMedication += "<td>" + item.DiscountinuedReason + "</td>";
                                allMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                allMedication += "</tr>";
                            }
                        }
                        else if (item.MedicationItem != string.Empty)
                        {
                            allMedication += "<tr>";
                            allMedication += "<td colspan='9' class='med-item-column'> <strong>" + item.MedicationItem + "</strong></td>";
                            allMedication += "</tr>";
                            allMedication += "<tr>";
                            allMedication += "<td>" + item.type + "</td>";
                            allMedication += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            allMedication += "<td>" + item.MedicationItem + "</td>";
                            allMedication += "<td>" + item.DosageInstruction + "</td>";
                            allMedication += "<td>" + item.Quantity + "</td>";
                            if (item.type.ToString().ToLower() == "acute" || item.type.ToString().ToLower() == "repeat")
                            {
                                allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            }
                            else if(item.LastIssuedDate.Year == 1)
                            {
                                allMedication += "<td></td>";
                            }
                            else
                            {
                                allMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            }
                            allMedication += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                            allMedication += "<td>" + item.DiscountinuedReason + "</td>";
                            allMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            allMedication += "</tr>";
                        }
                    }

                }

                // Sort the list by LastIssuedDate descending
                var medicationListDis = medicationList.OrderByDescending(item => item.LastIssuedDate).ToList();

                foreach (var item in medicationListDis)
                {
                    if (item.type.ToLower().ToString().Contains("repeat") && item.DiscountinuedReason != string.Empty)
                    {
                        discountinuedReapeatMedication += "<tr>";
                        discountinuedReapeatMedication += "<td>" + item.type + "</td>";
                        discountinuedReapeatMedication += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.MedicationItem + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.DosageInstruction + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.Quantity + "</td>";
                        discountinuedReapeatMedication += "<td class=\"date-column\">" + (item.DiscountinuedDate.Year == 1 ? "" : item.DiscountinuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                        discountinuedReapeatMedication += "<td>" + item.DiscountinuedReason + "</td>";
                        discountinuedReapeatMedication += "<td><b>" + item.AdditionalInformation + "</b></td>";
                        discountinuedReapeatMedication += "</tr>";
                    }
                }

                // Sort the list by Issue descending
                var medicationListAllMedIssue = medicationList.OrderByDescending(item => item.startDate).ToList();

                foreach (var item in medicationListAllMedIssue)
                {
                    if (startDate != "" && endDate != "")
                    {
                        if (!item.LastIssuedDate.ToString().Contains("0001") && (item.DiscountinuedReason != string.Empty || item.MedicationCancelledReason != string.Empty) && item.startDate > DateTime.Parse(startDate) && item.startDate < DateTime.Parse(endDate))
                        {
                            allmedicationIssue += "<tr>";
                            allmedicationIssue += "<td colspan='7' class='med-item-column'><strong>" + item.MedicationItem + "</strong></td>";
                            allmedicationIssue += "</tr>";

                            allmedicationIssue += "<tr>";
                            allmedicationIssue += "<td>" + item.type + "</td>";
                            allmedicationIssue += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            allmedicationIssue += "<td>" + item.MedicationItem + "</td>";
                            allmedicationIssue += "<td>" + item.DosageInstruction + "</td>";
                            allmedicationIssue += "<td>" + item.Quantity + "</td>";
                            allmedicationIssue += "<td>" + item.DaysDuration + "</td>";
                            allmedicationIssue += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            allmedicationIssue += "</tr>";
                        }
                    }
                    else
                    {
                        if(startDate != "" && endDate == "")
                        {
                            if (!item.LastIssuedDate.ToString().Contains("0001") && (item.DiscountinuedReason != string.Empty || item.MedicationCancelledReason != string.Empty) && item.startDate > DateTime.Parse(startDate) && item.startDate.Year != 1)
                            {
                                allmedicationIssue += "<tr>";
                                allmedicationIssue += "<td colspan='7' class='med-item-column'><strong>" + item.MedicationItem + "</strong></td>";
                                allmedicationIssue += "</tr>";

                                allmedicationIssue += "<tr>";
                                allmedicationIssue += "<td>" + item.type + "</td>";
                                allmedicationIssue += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                allmedicationIssue += "<td>" + item.MedicationItem + "</td>";
                                allmedicationIssue += "<td>" + item.DosageInstruction + "</td>";
                                allmedicationIssue += "<td>" + item.Quantity + "</td>";
                                allmedicationIssue += "<td>" + item.DaysDuration + "</td>";
                                allmedicationIssue += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                allmedicationIssue += "</tr>";
                            }
                        }
                        else if(startDate == "" && endDate != "")
                        {
                            if(!item.LastIssuedDate.ToString().Contains("0001") && item.startDate < DateTime.Parse(endDate) && (item.DiscountinuedReason != string.Empty || item.MedicationCancelledReason != string.Empty))
                            {
                            allmedicationIssue += "<tr>";
                            allmedicationIssue += "<td colspan='7' class='med-item-column'><strong>" + item.MedicationItem + "</strong></td>";
                            allmedicationIssue += "</tr>";

                            allmedicationIssue += "<tr>";
                            allmedicationIssue += "<td>" + item.type + "</td>";
                            allmedicationIssue += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            allmedicationIssue += "<td>" + item.MedicationItem + "</td>";
                            allmedicationIssue += "<td>" + item.DosageInstruction + "</td>";
                            allmedicationIssue += "<td>" + item.Quantity + "</td>";
                            allmedicationIssue += "<td>" + item.DaysDuration + "</td>";
                            allmedicationIssue += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            allmedicationIssue += "</tr>";
                            }
                        }
                        else
                        {
                            if (!item.LastIssuedDate.ToString().Contains("0001") && (item.DiscountinuedReason != string.Empty || item.MedicationCancelledReason != string.Empty))
                            {
                                allmedicationIssue += "<tr>";
                                allmedicationIssue += "<td colspan='7' class='med-item-column'><strong>" + item.MedicationItem + "</strong></td>";
                                allmedicationIssue += "</tr>";

                                allmedicationIssue += "<tr>";
                                allmedicationIssue += "<td>" + item.type + "</td>";
                                allmedicationIssue += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                                allmedicationIssue += "<td>" + item.MedicationItem + "</td>";
                                allmedicationIssue += "<td>" + item.DosageInstruction + "</td>";
                                allmedicationIssue += "<td>" + item.Quantity + "</td>";
                                allmedicationIssue += "<td>" + item.DaysDuration + "</td>";
                                allmedicationIssue += "<td><b>" + item.AdditionalInformation + "</b></td>";
                                allmedicationIssue += "</tr>";
                            }
                        }
                    }
                }



                if (acuteMedicationDiv == "")
                {
                    acuteMedicationDivTable = "<p>No 'Acute Medication (Last 12 Months)' data is recorded for this patient.</p>";
                }
                else
                {
                    acuteMedicationDivTable = "<table id=\"med-tab-acu-med\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Scheduled End Date</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> " + acuteMedicationDiv+" </tbody> </table>";
                }


                if (repeatMedicationDiv == "")
                {
                    repeatMedicationDivTable = "<p>No 'Current Repeat Medication' data is recorded for this patient.</p>";
                }
                else
                {
                    repeatMedicationDivTable = "  <table id=\"med-tab-curr-rep\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Max Issues</th> <th>Review Date</th> <th>Additional Information</th> </tr> </thead> <tbody> " + repeatMedicationDiv+" </tbody> </table> ";
                }


                if (discountinuedReapeatMedication == "")
                {
                    discountinuedReapeatMedicationTable = "<p>Date filter not applied</p></div><p>No 'Discontinued Repeat Medication' data is recorded for this patient.</p>";    
                }
                else
                {
                    discountinuedReapeatMedicationTable = " <table id=\"med-tab-dis-rep\"> <thead> <tr> <th>Type</th> <th>Last Issued Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Discontinued Date</th> <th>Discontinuation Reason</th> <th>Additional Information</th> </tr> </thead> <tbody> " + discountinuedReapeatMedication+" </tbody> </table> ";
                }



                if (allMedication == "")
                {
                    allMedicationTable = "<p>No 'All Medication' data is recorded for this patient.</p>";
                }
                else
                {
                    allMedicationTable = " <table id=\"med-tab-all-sum\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Discontinuation Details</th> <th>Additional Information</th> </tr> </thead> <tbody> " + allMedication+" </tbody> </table>  ";
                }

                if (allmedicationIssue == "")
                {
                    allmedicationIssueTable = "<p>No 'All Medication Issues' data is recorded for this patient.</p>";
                }
                else
                {
                    allmedicationIssueTable = "  <table id=\"med-tab-all-iss\"> <thead> <tr> <th>Type</th> <th>Issue Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> "+allmedicationIssue+" </tbody> </table> ";
                }

                if(startDate == "" || endDate == "")
                {
                    bannerNotAppliedDefined = "<div class=\"date-banner\"><p>All relevant items</p></div>";
                }


                var htmlcontent = "<div> <h1>Medications</h1> "+ bannerDTO.GpTransferBanner + bannerDTO.MedicationsContentBanner + "<div> <h2>Acute Medication (Last 12 Months)</h2>  " + bannerDTO.AcuteMedicationContentBanner + bannerDTO.AcuteMedicationExclusiveBanner + acuteMedicationDivTable  + "</div>" +
                    "<div> <h2>Current Repeat Medication</h2> " +bannerDTO.CurrentRepeatMedicationContentBanner + bannerDTO.CurrentRepeatMedicationExclusiveBanner +  repeatMedicationDivTable + "</div>" +
                    "<div> <h2>Discontinued Repeat Medication</h2>" +bannerDTO.DiscontinuedRepeatMedicationContentBanner + bannerDTO.DiscontinuedRepeatMedicationExclusiveBanner + discountinuedReapeatMedicationTable + "</div>" +
                    "<div> <h2>All Medication</h2> " +bannerDTO.AllMedicationContentBanner +datefilterBanner+ bannerDTO.AllMedicationExclusiveBanner + allMedicationTable+ "</div>" +
                    "<div> <h2>All Medication Issues</h2> " + bannerDTO.AllMedicationIssueContentBanner + datefilterBanner+ bannerDTO.AllMedicationIssueExclusiveBanner + allmedicationIssueTable + " </div> </div>";
 
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "MED" },
                                            { "display", "Medications" }
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
                if(emergencyCodeList.Count >= 0)
                {
                    var emergenercyTableHtml = "<p>No 'Emergency Code' data is recorded for this patient.</p>";
                    var encounterlasthtml = "<p>No 'Last 3 Encounters' data is recorded for this patient.</p>";
                    var activeProblemhtml = "<p>No 'Active Problems and Issues' data is recorded for this patient.</p>";
                    var manjorInavtiveProblemhtml = "<p>No 'Major Inactive Problems and Issues' data is recorded for this patient.</p>";
                    var currentallergyandadversehtml = "<p>No 'Current Allergies and Adverse Reactions' data is recorded for this patient.</p>";
                    var acutemedicationhtml = "<p>No 'Acute Medication (Last 12 Months)' data is recorded for this patient.</p>";
                    var currentrepeatmedicatipon = "<p>No 'Current Repeat Medication' data is recorded for this patient.</p>";



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

                    if(emerCodeHTMLContetnt != "")
                    {
                        emergenercyTableHtml = "<table id=\"cli-tab\"> <thead> <tr> <th>Date</th> <th>Entry</th> <th>Details</th> <th>Location of further information</th> </tr> </thead> <tbody>" + emerCodeHTMLContetnt + "  </tbody> </table>";
                    }
                    if(thrreEccounterhtmlcontent != "")
                    {
                        encounterlasthtml = "<table id=\"enc-tab\"> <thead> <tr> <th>Date</th> <th>Title</th> <th>Details</th> </tr> </thead> <tbody> " + thrreEccounterhtmlcontent + " </tbody> </table>";
                    }
                    if(activeProblemhtmlcontent != "")
                    {
                        activeProblemhtml = " <table id=\"prb-tab-act\"> <thead> <tr> <th>Start Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> " + activeProblemhtmlcontent + " </tbody> </table>";

                    }
                    if (majorInactivehtmlcontent != "")
                    {
                        manjorInavtiveProblemhtml = "<table id=\"prb-tab-majinact\"> <thead> <tr> <th>Start Date</th> <th>End Date</th> <th>Entry</th> <th>Significance</th> <th>Details</th> </tr> </thead> <tbody> " + majorInactivehtmlcontent + " </tbody> </table>";

                    }
                    if (activeAlleryhtmlcontent != "")
                    {
                        currentallergyandadversehtml = " <table id=\"all-tab-curr\"> <thead> <tr> <th>Start Date</th> <th>Details</th> </tr> </thead> <tbody> " + activeAlleryhtmlcontent + " </tbody> </table>";

                    }
                    if (acutemedicationhtmlcontent != "")
                    {
                        acutemedicationhtml = "<div> <p>Scheduled End Date is not always captured in the source; where it was not recorded, the displayed date is calculated from start date and days duration</p> </div> <table id=\"med-tab-acu-med\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Scheduled End Date</th> <th>Days Duration</th> <th>Additional Information</th> </tr> </thead> <tbody> " + acutemedicationhtmlcontent + " </tbody> </table>";

                    }
                    if (currentRepeatMedication != "")
                    {
                        currentrepeatmedicatipon = "<table id=\"med-tab-curr-rep\"> <thead> <tr> <th>Type</th> <th>Start Date</th> <th>Medication Item</th> <th>Dosage Instruction</th> <th>Quantity</th> <th>Last Issued Date</th> <th>Number of Prescriptions Issued</th> <th>Max Issues</th> <th>Review Date</th> <th>Additional Information</th> </tr> </thead> <tbody> " + currentRepeatMedication + " </tbody> </table>";

                    }

                    var finalhtmlcontentofsummary = "<div> <h1>Summary</h1> "+bannerDTO.GpTransferBanner+" <div> <h2>Emergency Codes</h2> "+ emergenercyTableHtml + "  </div> <div> <h2>Last 3 Encounters</h2>  " + bannerDTO.EncounterContentBanner + bannerDTO.EncounterExclusiveBanner+  encounterlasthtml + "  </div> <div> <h2>Active Problems and Issues</h2> " +bannerDTO.ActiveProblemsandIssuesContentBanner+ bannerDTO.ActiveProblemsandIssuesExclusiveBanner +  activeProblemhtml + " </div> <div> <h2>Major Inactive Problems and Issues</h2> "+ bannerDTO.MajorInactiveProblemsandIssuesContentBanner + bannerDTO.MajorInactiveProblemsandIssuesExclusiveBanner  +manjorInavtiveProblemhtml + "  </div> <div> <h2>Current Allergies and Adverse Reactions</h2> " + bannerDTO.CurrentAllergiesandAdverseReactionsContentBanner + bannerDTO.CurrentAllergiesandAdverseReactionsExclusiveBanner  +currentallergyandadversehtml+ " </div> <div> <h2>Acute Medication (Last 12 Months)</h2> " + bannerDTO.AcuteMedicationContentBanner + bannerDTO.AcuteMedicationExclusiveBanner + acutemedicationhtml + " </div> <div> <h2>Current Repeat Medication</h2> "+ bannerDTO.CurrentRepeatMedicationContentBanner + bannerDTO.CurrentRepeatMedicationExclusiveBanner + currentrepeatmedicatipon + " </div> </div>";

                
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
                        htmlContent += "<td class=\"date-column\">" + item.recDate.ToString("dd-MMM-yyyy") + "</td>";
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
                    encHtmlContent += "<td class=\"date-column\">" + item.Date.ToString("dd-MMM-yyyy") + "</td>";
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
                                        <attribute name='bcrm_prescribingagencytype' />
                                        <attribute name='bcrm_lastauthoriseddate' />
                                        <attribute name='bcrm_medicationcancelledreason' />
                                        <attribute name='bcrm_medicationcancelleddate' />
                                        <attribute name='bcrm_linkedproblem' />
                                        <attribute name='bcrm_reasonforthemedication' />
                                        <attribute name='bcrm_othersupportinginformation' />
                                        <attribute name='bcrm_controlleddrug' />

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

                    if (medicationRecord.Quantity != string.Empty)
                    {
                        medicationRecord.Quantity = medicationRecord.Quantity + " " + GetMeasuringQuantityDrugUsingName(medicationRecord.MedicationItem);
                    }

                    if (record.Attributes.Contains("bcrm_drugenddate")) { medicationRecord.endDate = (DateTime)record.Attributes["bcrm_drugenddate"]; }
                    if (record.Attributes.Contains("bcrm_lastissueddate")) { medicationRecord.LastIssuedDate = (DateTime)record.Attributes["bcrm_lastissueddate"]; }
                    if (record.Attributes.Contains("bcrm_reviewdate")) { medicationRecord.ReviewDate = (DateTime)record.Attributes["bcrm_reviewdate"]; }
                    if (record.Attributes.Contains("bcrm_discontinueddate")) { medicationRecord.DiscountinuedDate = (DateTime)record.Attributes["bcrm_discontinueddate"]; }

                    medicationRecord.DaysDuration = record.Attributes.Contains("bcrm_expectedsupplyvalue") ? record["bcrm_expectedsupplyvalue"].ToString() : string.Empty;
                    medicationRecord.AdditionalInformation = record.Attributes.Contains("bcrm_additionalinformation") ? record["bcrm_additionalinformation"].ToString() : string.Empty;
                    medicationRecord.NumberOfPrescriptionIsuued = record.Attributes.Contains("bcrm_numberofprescriptionsissued") ? record["bcrm_numberofprescriptionsissued"].ToString() : string.Empty;
                    medicationRecord.MaxIssues = record.Attributes.Contains("bcrm_maxissues") ? record["bcrm_maxissues"].ToString() : string.Empty;

                    medicationRecord.DiscountinuedReason = record.Attributes.Contains("bcrm_discontinuationreason") ? record["bcrm_discontinuationreason"].ToString() : string.Empty;

                    medicationRecord.PrescribingAgencyType = record.Attributes.Contains("bcrm_prescribingagencytype") ? record["bcrm_prescribingagencytype"].ToString() : string.Empty;
                    medicationRecord.MedicationCancelledReason = record.Attributes.Contains("bcrm_medicationcancelledreason") ? record["bcrm_medicationcancelledreason"].ToString() : string.Empty;
                    medicationRecord.linkedProblem = record.Attributes.Contains("bcrm_linkedproblem") ? record["bcrm_linkedproblem"].ToString() : string.Empty;
                    medicationRecord.OtherSupportingInformation = record.Attributes.Contains("bcrm_othersupportinginformation") ? record["bcrm_othersupportinginformation"].ToString() : string.Empty;
                    medicationRecord.ReasonForMedication = record.Attributes.Contains("bcrm_reasonforthemedication") ? record["bcrm_reasonforthemedication"].ToString() : string.Empty;
                    if (record.Attributes.Contains("bcrm_lastauthoriseddate")) { medicationRecord.LastAutorizedDate = (DateTime)record.Attributes["bcrm_lastauthoriseddate"]; }
                    if (record.Attributes.Contains("bcrm_medicationcancelleddate")) { medicationRecord.Medicationcancelleddate = (DateTime)record.Attributes["bcrm_medicationcancelleddate"]; }

                    if (medicationRecord.startDate.Year == 1)
                    {
                        if (record.Attributes.Contains("createdon")) { medicationRecord.startDate = (DateTime)record.Attributes["createdon"]; }
                    }


                    medicationRecord.ControlledDrug = record.Attributes.Contains("bcrm_controlleddrug") ? record["bcrm_controlleddrug"].ToString() : string.Empty;

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
                if (item.ControlledDrug != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> CONTROLLED DRUG : </b>" + item.ControlledDrug + " <br>";
                }
                if (item.type.ToString().ToLower() == "acute" && item.MedicationCancelledReason != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> CANCELLED DATE : </b>" + item.Medicationcancelleddate.ToString("dd-MMM-yyyy") + " CANCELLED REASON : " + item.MedicationCancelledReason + " <br>";
                }
                if (item.ReasonForMedication != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Reason for Medication : </b>" + item.ReasonForMedication + " <br>";
                }
                if (item.linkedProblem != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Linked Problem : </b>" + item.linkedProblem + " <br>";
                }
                if (item.OtherSupportingInformation != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Other supporting information : </b>" + item.OtherSupportingInformation + " <br>";
                }
                if (item.type.ToString().ToLower().Contains("repeat") && item.LastAutorizedDate.Year != 1)
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Last Authorised : </b>" + item.LastAutorizedDate.ToString("dd-MMM-yyyy") + " <br>";
                }
                if (item.type.ToString().ToLower().Contains("repeat") && item.NumberOfPrescriptionIsuued != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Number issues authorised : </b>" + item.NumberOfPrescriptionIsuued + " <br>";
                }
                if (item.type == "")
                {
                    if (item.PrescribingAgencyType.ToString().ToLower().Trim() == "acute")
                    {
                        item.type = "Acute – Unknown Prescriber";
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Trim() == "repeat")
                    {
                        item.type = "Repeat – Unknown Prescriber";
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Contains("acute"))
                    {
                        item.type = item.PrescribingAgencyType;
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Contains("repeat"))
                    {
                        item.type = item.PrescribingAgencyType;
                    }
                    else
                    {
                        item.type = "Acute – Unknown Prescriber";
                    }
                }
            }
            foreach (var item in medicationList)
            {
                if (item.startDate >= DateTime.Now.AddYears(-1) && item.startDate <= DateTime.Now && item.type.ToLower().ToString().Contains("acute"))
                {
                    acuteMedicationDiv += "<tr>";
                    acuteMedicationDiv += "<td class=\"date-column\">" + item.type + "</td>";
                    acuteMedicationDiv += "<td class=\"date-column\">" + item.startDate.ToString("dd-MMM-yyyy") + "</td>";
                    acuteMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                    acuteMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                    acuteMedicationDiv += "<td>" + item.Quantity + "</td>";
                    acuteMedicationDiv += "<td class=\"date-column\">" + (item.endDate.Year == 1 ? "" : item.endDate.ToString("dd-MMM-yyyy")) + "</td>";
                    acuteMedicationDiv += "<td>" + item.DaysDuration + "</td>";
                    acuteMedicationDiv += "<td>" + item.AdditionalInformation + "</td>";
                    acuteMedicationDiv += "</tr>";
                }
            }
            return acuteMedicationDiv;
        }
        internal string makeOnlyCurrentReapeatMedication(List<MedicationDTO> medicationList)
        {
            var repeatMedicationDiv = "";
            foreach (var item in medicationList)
            {
                if (item.ControlledDrug != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> CONTROLLED DRUG : </b>" + item.ControlledDrug + " <br>";
                }
                if (item.type.ToString().ToLower() == "acute" && item.MedicationCancelledReason != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> CANCELLED DATE : </b>" + item.Medicationcancelleddate.ToString("dd-MMM-yyyy") + " CANCELLED REASON : " + item.MedicationCancelledReason + " <br>";
                }
                if (item.ReasonForMedication != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Reason for Medication : </b>" + item.ReasonForMedication + " <br>";
                }
                if (item.linkedProblem != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Linked Problem : </b>" + item.linkedProblem + " <br>";
                }
                if (item.OtherSupportingInformation != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Other supporting information : </b>" + item.OtherSupportingInformation + " <br>";
                }
                if (item.type.ToString().ToLower().Contains("repeat") && item.LastAutorizedDate.Year != 1)
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Last Authorised : </b>" + item.LastAutorizedDate.ToString("dd-MMM-yyyy") + " <br>";
                }
                if (item.type.ToString().ToLower().Contains("repeat") && item.NumberOfPrescriptionIsuued != "")
                {
                    item.AdditionalInformation = item.AdditionalInformation + "<b> Number issues authorised : </b>" + item.NumberOfPrescriptionIsuued + " <br>";
                }
                if (item.type == "")
                {
                    if (item.PrescribingAgencyType.ToString().ToLower().Trim() == "acute")
                    {
                        item.type = "Acute – Unknown Prescriber";
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Trim() == "repeat")
                    {
                        item.type = "Repeat – Unknown Prescriber";
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Contains("acute"))
                    {
                        item.type = item.PrescribingAgencyType;
                    }
                    else if (item.PrescribingAgencyType.ToString().ToLower().Contains("repeat"))
                    {
                        item.type = item.PrescribingAgencyType;
                    }
                    else
                    {
                        item.type = "Acute – Unknown Prescriber";
                    }
                }
            }
            foreach (var item in medicationList)
            {
                if (item.type.ToLower().ToString().Contains("repeat") && item.DiscountinuedReason == string.Empty)
                {
                    if (item.startDate.Year != 1)
                    {
                        if (item.endDate.Year == 1)
                        {
                            repeatMedicationDiv += "<tr>";
                            repeatMedicationDiv += "<td>" + item.type + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                            repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                            repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                            repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.ReviewDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.ReviewDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            repeatMedicationDiv += "</tr>";
                        }
                        else if (item.startDate <= DateTime.UtcNow && DateTime.UtcNow <= item.endDate)
                        {
                            repeatMedicationDiv += "<tr>";
                            repeatMedicationDiv += "<td>" + item.type + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.startDate.Year == 1 ? "" : item.startDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td>" + item.MedicationItem + "</td>";
                            repeatMedicationDiv += "<td>" + item.DosageInstruction + "</td>";
                            repeatMedicationDiv += "<td>" + item.Quantity + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.LastIssuedDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.LastIssuedDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td>" + item.NumberOfPrescriptionIsuued + "</td>";
                            repeatMedicationDiv += "<td>" + item.MaxIssues + "</td>";
                            repeatMedicationDiv += "<td class=\"date-column\">" + (item.ReviewDate.Year == 1 ? item.startDate.ToString("dd-MMM-yyyy") : item.ReviewDate.ToString("dd-MMM-yyyy")) + "</td>";
                            repeatMedicationDiv += "<td><b>" + item.AdditionalInformation + "</b></td>";
                            repeatMedicationDiv += "</tr>";
                        }
                    }
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
            { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz") },
            { "type", new Dictionary<string, object>
            {
            { "coding", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "system", "http://snomed.info/sct" },
                        { "code", "425173008" },
                        { "display", "record extract" }
                    }
                }
            },
            { "text", "record extract" }
            }
            },
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
                { "reference", "Practitioner/" + authorSequenceNumber }
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
                                            { "code", "SUM" },
                                            { "display", "Summary" }
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
        internal string GetMeasuringQuantityDrugUsingName(string name)
        {
            try
            {
                switch (name.ToLower()) // Convert to lowercase for case-insensitive matching
                {
                    case string s when s.Contains("tablet"):
                        return "tablets";
                    case string s when s.Contains("capsule"):
                        return "capsules";
                    case string s when s.Contains("gram"):
                        return "grams";
                    case string s when s.Contains("ml"):
                        return "mls";
                    case string s when s.Contains("sachet"):
                        return "sachets";
                    case string s when s.Contains("strip"):
                        return "strips";
                    case string s when s.Contains("needle"):
                        return "needles";
                    case string s when s.Contains("ampoule"):
                        return "ampoules";
                    case string s when s.Contains("drop"):
                        return "drops";
                    case string s when s.Contains("bottle"):
                        return "bottles";
                    case string s when s.Contains("suppository"):
                        return "suppositories";
                    case string s when s.Contains("spray"):
                        return "sprays";
                    case string s when s.Contains("tube"):
                        return "tubes";
                    case string s when s.Contains("patch"):
                        return "patches";
                    case string s when s.Contains("cartridge"):
                        return "cartridges";
                    case string s when s.Contains("lozenge"):
                        return "lozenges";
                    case string s when s.Contains("vial"):
                        return "vials";
                    case string s when s.Contains("inhaler"):
                        return "inhalers";
                    case string s when s.Contains("cream"):
                        return "creams";
                    case string s when s.Contains("ointment"):
                        return "ointments";
                    case string s when s.Contains("syringe"):
                        return "syringes";
                    case string s when s.Contains("powder"):
                        return "powders";
                    case string s when s.Contains("solution"):
                        return "solutions";
                    case string s when s.Contains("pellet"):
                        return "pellets";
                    case string s when s.Contains("kit"):
                        return "kits";
                    case string s when s.Contains("suppositories"):
                        return "suppositories";
                    case string s when s.Contains("solid"):
                        return "gram";
                    default:
                        return "items"; // Return the original name if no match is found
                }
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        #endregion

        #region ConvertDateFormat

        public static string ConvertToProperDateFormat(string inputDate, string type)
        {
            DateTime parsedDate;

            // Define the different input formats
            string[] dateFormats = {
                                      "yyyy",                      // Year only
                                      "yyyy-MM",                   // Year and Month
                                      "yyyy-MM-dd",                // Year, Month, and Day
                                      "yyyy-MM-ddTHH",             // Year, Month, Day, and Hour
                                      "yyyy-MM-ddTHH:mm",          // Year, Month, Day, Hour, and Minute
                                      "yyyy-MM-ddTHH:mm:sszzz",    // Full date, time, and time zone offset (e.g., 2015-10-23T16:38:32+05:30)
                                      "yyyy-MM-ddTHH:mm:ssZ"       // Full date and time in UTC (Z at the end)
                                  };

            // Attempt to parse the input date with the formats provided
            if (DateTime.TryParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                if (type.ToLower() == "start")
                {
                    // Return the start of the provided date
                    if (inputDate.Length == 4) // Year only
                    {
                        return new DateTime(parsedDate.Year, 1, 1).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Length == 7) // Year and Month
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, 1).ToString("yyyy-MM-dd");
                    }
                    else // Full date
                    {
                        return parsedDate.ToString("yyyy-MM-dd");
                    }
                }
                else if (type.ToLower() == "end")
                {
                    // Return the end of the provided date
                    if (inputDate.Length == 4) // Year only
                    {
                        return new DateTime(parsedDate.Year, 12, 31).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Length == 7) // Year and Month
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month)).ToString("yyyy-MM-dd");
                    }
                    else // Full date
                    {
                        return parsedDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            // If parsing fails, return "Invalid_date_format"
            return "Invalid_date_format";
        }



        #endregion

        #region Get All Banner

        internal BannerDTO GetAllBannerContent(string nhsNumber)
        {
            try
            {
                BannerDTO bannerDTO = new BannerDTO();

                var bannerXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                       <entity name='bcrm_patientaccesshtmlbanner'>
                                         <attribute name='bcrm_patientaccesshtmlbannerid' />
                                         <attribute name='bcrm_name' />
                                         <attribute name='createdon' />
                                         <attribute name='statuscode' />
                                         <attribute name='statecode' />
                                         <attribute name='bcrm_referralsexclusionbanner' />
                                         <attribute name='bcrm_referralscontentbanner' />
                                         <attribute name='overriddencreatedon' />
                                         <attribute name='bcrm_problemsandissuescontentbanner' />
                                         <attribute name='bcrm_patient' />
                                         <attribute name='owningbusinessunit' />
                                         <attribute name='ownerid' />
                                         <attribute name='bcrm_otherinactiveproblemsandissuesexclusionbanne' />
                                         <attribute name='bcrm_otherinactiveproblemsandissuescontentbanner' />
                                         <attribute name='bcrm_observationsexclusionbanner' />
                                         <attribute name='bcrm_observationscontentbanner' />
                                         <attribute name='modifiedon' />
                                         <attribute name='modifiedonbehalfby' />
                                         <attribute name='modifiedby' />
                                         <attribute name='bcrm_medicationscontentbanner' />
                                         <attribute name='bcrm_majorinactiveproblemsandissuesexclusionbanne' />
                                         <attribute name='bcrm_majorinactiveproblemsandissuescontentbanner' />
                                         <attribute name='bcrm_immunisationsexclusionbanner' />
                                         <attribute name='bcrm_immunisationscontentbanner' />
                                         <attribute name='bcrm_historicalallergiesandadversereactionsexclusionb' />
                                         <attribute name='bcrm_historicalallergiesandadversereactionscontentban' />
                                         <attribute name='bcrm_encountersexclusionbanner' />
                                         <attribute name='bcrm_encounterscontentbanner' />
                                         <attribute name='bcrm_discontinuedrepeatmedicationexclusionbanner' />
                                         <attribute name='bcrm_discontinuedrepeatmedicationcontentbanner' />
                                         <attribute name='bcrm_currentallergiesandadversereactionsexclusionbanne' />
                                         <attribute name='bcrm_currentallergiesandadversereactioncontentbanner' />
                                         <attribute name='bcrm_currentrepeatmedicationexclusionbanner' />
                                         <attribute name='bcrm_currentrepeatmedicationcontentbanner' />
                                         <attribute name='createdonbehalfby' />
                                         <attribute name='createdby' />
                                         <attribute name='bcrm_clinicalitemsexclusionbanner' />
                                         <attribute name='bcrm_clinicalitemscontentbanner' />
                                         <attribute name='bcrm_allergiesandadversereactioncontentbanner' />
                                         <attribute name='bcrm_allmedicationissuesexclusionbanner' />
                                         <attribute name='bcrm_allmedicationissuescontentbanner' />
                                         <attribute name='bcrm_allmedicationexclusionbanner' />
                                         <attribute name='bcrm_allmedicationcontentbanner' />
                                         <attribute name='bcrm_administrativeitemsexclusionbanner' />
                                         <attribute name='bcrm_administrativeitemscontentbanner' />
                                         <attribute name='bcrm_acutemedicationexclusionbanner' />
                                         <attribute name='bcrm_acutemedicationcontentbanner' />
                                         <attribute name='bcrm_activeproblemsandissuesexclusionbanner' />
                                         <attribute name='bcrm_activeproblemsandissuescontentbanner' />
                                         <order attribute='bcrm_name' descending='false' />
                                         <link-entity name='contact' from='contactid' to='bcrm_patient' link-type='inner' alias='patient'>
                                         <attribute name='bcrm_gptransferbanner' />
                                           <filter type='and'>
                                             <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                           </filter>
                                         </link-entity>
                                       </entity>
                                     </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(bannerXML));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    dynamic gptranData = record.Attributes.Contains("patient.bcrm_gptransferbanner") ? record["patient.bcrm_gptransferbanner"] : string.Empty;
                    try
                    {
                        bannerDTO.GpTransferBanner = gptranData.Value;
                    }
                    catch (Exception) { }
   
                    bannerDTO.EncounterContentBanner = record.Attributes.Contains("bcrm_encounterscontentbanner") ? record["bcrm_encounterscontentbanner"].ToString() : string.Empty;
                    bannerDTO.EncounterExclusiveBanner = record.Attributes.Contains("bcrm_encountersexclusionbanner") ? record["bcrm_encountersexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.ClinicalItemContentBanner = record.Attributes.Contains("bcrm_clinicalitemscontentbanner") ? record["bcrm_clinicalitemscontentbanner"].ToString() : string.Empty;
                    bannerDTO.ClinicalItemExclusiveBanner = record.Attributes.Contains("bcrm_clinicalitemsexclusionbanner") ? record["bcrm_clinicalitemsexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.ProblemsandIssuesContentBanner = record.Attributes.Contains("bcrm_activeproblemsandissuescontentbanner") ? record["bcrm_activeproblemsandissuescontentbanner"].ToString() : string.Empty;
                    bannerDTO.ActiveProblemsandIssuesContentBanner = record.Attributes.Contains("bcrm_activeproblemsandissuescontentbanner") ? record["bcrm_activeproblemsandissuescontentbanner"].ToString() : string.Empty;
                    bannerDTO.ActiveProblemsandIssuesExclusiveBanner = record.Attributes.Contains("bcrm_activeproblemsandissuesexclusionbanner") ? record["bcrm_activeproblemsandissuesexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.MajorInactiveProblemsandIssuesContentBanner = record.Attributes.Contains("bcrm_majorinactiveproblemsandissuescontentbanner") ? record["bcrm_majorinactiveproblemsandissuescontentbanner"].ToString() : string.Empty;
                    bannerDTO.MajorInactiveProblemsandIssuesExclusiveBanner = record.Attributes.Contains("bcrm_majorinactiveproblemsandissuesexclusionbanne") ? record["bcrm_majorinactiveproblemsandissuesexclusionbanne"].ToString() : string.Empty;
                    bannerDTO.OtherInactiveProblemsandIssuesContentBanner = record.Attributes.Contains("bcrm_otherinactiveproblemsandissuescontentbanner") ? record["bcrm_otherinactiveproblemsandissuescontentbanner"].ToString() : string.Empty;
                    bannerDTO.OtherInactiveProblemsandIssuesExclusiveBanner = record.Attributes.Contains("bcrm_otherinactiveproblemsandissuesexclusionbanne") ? record["bcrm_otherinactiveproblemsandissuesexclusionbanne"].ToString() : string.Empty;

                    bannerDTO.AllergiesandAdverseReactionsContentBanner = record.Attributes.Contains("bcrm_allergiesandadversereactioncontentbanner") ? record["bcrm_allergiesandadversereactioncontentbanner"].ToString() : string.Empty;
                    bannerDTO.CurrentAllergiesandAdverseReactionsContentBanner = record.Attributes.Contains("bcrm_currentallergiesandadversereactioncontentbanner") ? record["bcrm_currentallergiesandadversereactioncontentbanner"].ToString() : string.Empty;
                    bannerDTO.CurrentAllergiesandAdverseReactionsExclusiveBanner = record.Attributes.Contains("bcrm_currentallergiesandadversereactionsexclusionbanne") ? record["bcrm_currentallergiesandadversereactionsexclusionbanne"].ToString() : string.Empty;
                    bannerDTO.HistoricalAllergiesandAdverseReactionsContentBanner = record.Attributes.Contains("bcrm_historicalallergiesandadversereactionscontentban") ? record["bcrm_historicalallergiesandadversereactionscontentban"].ToString() : string.Empty;
                    bannerDTO.HistoricalAllergiesandAdverseReactionsExclusiveBanner = record.Attributes.Contains("bcrm_historicalallergiesandadversereactionsexclusionb") ? record["bcrm_historicalallergiesandadversereactionsexclusionb"].ToString() : string.Empty;



                    bannerDTO.MedicationsContentBanner = record.Attributes.Contains("bcrm_medicationscontentbanner") ? record["bcrm_medicationscontentbanner"].ToString() : string.Empty;
                    bannerDTO.AcuteMedicationContentBanner = record.Attributes.Contains("bcrm_acutemedicationcontentbanner") ? record["bcrm_acutemedicationcontentbanner"].ToString() : string.Empty;
                    bannerDTO.AcuteMedicationExclusiveBanner = record.Attributes.Contains("bcrm_acutemedicationexclusionbanner") ? record["bcrm_acutemedicationexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.CurrentRepeatMedicationContentBanner = record.Attributes.Contains("bcrm_currentrepeatmedicationcontentbanner") ? record["bcrm_currentrepeatmedicationcontentbanner"].ToString() : string.Empty;
                    bannerDTO.CurrentRepeatMedicationExclusiveBanner = record.Attributes.Contains("bcrm_currentrepeatmedicationexclusionbanner") ? record["bcrm_currentrepeatmedicationexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.DiscontinuedRepeatMedicationContentBanner = record.Attributes.Contains("bcrm_discontinuedrepeatmedicationcontentbanner") ? record["bcrm_discontinuedrepeatmedicationcontentbanner"].ToString() : string.Empty;
                    bannerDTO.DiscontinuedRepeatMedicationExclusiveBanner = record.Attributes.Contains("bcrm_discontinuedrepeatmedicationexclusionbanner") ? record["bcrm_discontinuedrepeatmedicationexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.AllMedicationContentBanner = record.Attributes.Contains("bcrm_allmedicationcontentbanner") ? record["bcrm_allmedicationcontentbanner"].ToString() : string.Empty;
                    bannerDTO.AllMedicationExclusiveBanner = record.Attributes.Contains("bcrm_allmedicationexclusionbanner") ? record["bcrm_allmedicationexclusionbanner"].ToString() : string.Empty;
                    bannerDTO.AllMedicationIssueContentBanner = record.Attributes.Contains("bcrm_allmedicationissuescontentbanner") ? record["bcrm_allmedicationissuescontentbanner"].ToString() : string.Empty;
                    bannerDTO.AllMedicationIssueExclusiveBanner = record.Attributes.Contains("bcrm_allmedicationissuesexclusionbanner") ? record["bcrm_allmedicationissuesexclusionbanner"].ToString() : string.Empty;

                    bannerDTO.ReferralsContentBanner = record.Attributes.Contains("bcrm_referralscontentbanner") ? record["bcrm_referralscontentbanner"].ToString() : string.Empty;
                    bannerDTO.ReferralsExclusiveBanner = record.Attributes.Contains("bcrm_referralsexclusionbanner") ? record["bcrm_referralsexclusionbanner"].ToString() : string.Empty;

                    bannerDTO.ObservationsContentBanner = record.Attributes.Contains("bcrm_observationscontentbanner") ? record["bcrm_observationscontentbanner"].ToString() : string.Empty;
                    bannerDTO.ObservationsExclusiveBanner = record.Attributes.Contains("bcrm_observationsexclusionbanner") ? record["bcrm_observationsexclusionbanner"].ToString() : string.Empty;

                    bannerDTO.ImmunisationsContentBanner = record.Attributes.Contains("bcrm_immunisationscontentbanner") ? record["bcrm_immunisationscontentbanner"].ToString() : string.Empty;
                    bannerDTO.ImmunisationsExclusiveBanner = record.Attributes.Contains("bcrm_immunisationsexclusionbanner") ? record["bcrm_immunisationsexclusionbanner"].ToString() : string.Empty;

                    bannerDTO.AdministrativeItemsContentBanner = record.Attributes.Contains("bcrm_administrativeitemscontentbanner") ? record["bcrm_administrativeitemscontentbanner"].ToString() : string.Empty;
                    bannerDTO.AdministrativeItemsExclusiveBanner = record.Attributes.Contains("bcrm_administrativeitemsexclusionbanner") ? record["bcrm_administrativeitemsexclusionbanner"].ToString() : string.Empty;




                    bannerDTO.EncounterContentBanner = WrapWithHtml(bannerDTO.EncounterContentBanner, "content-banner");
                    bannerDTO.EncounterExclusiveBanner = WrapWithHtml(bannerDTO.EncounterExclusiveBanner, "exclusion-banner");
                    bannerDTO.ClinicalItemContentBanner = WrapWithHtml(bannerDTO.ClinicalItemContentBanner, "content-banner");
                    bannerDTO.ClinicalItemExclusiveBanner = WrapWithHtml(bannerDTO.ClinicalItemExclusiveBanner, "exclusion-banner");
                    bannerDTO.ProblemsandIssuesContentBanner = WrapWithHtml(bannerDTO.ProblemsandIssuesContentBanner, "content-banner");
                    bannerDTO.ActiveProblemsandIssuesContentBanner = WrapWithHtml(bannerDTO.ActiveProblemsandIssuesContentBanner, "content-banner");
                    bannerDTO.ActiveProblemsandIssuesExclusiveBanner = WrapWithHtml(bannerDTO.ActiveProblemsandIssuesExclusiveBanner, "exclusion-banner");
                    bannerDTO.MajorInactiveProblemsandIssuesContentBanner = WrapWithHtml(bannerDTO.MajorInactiveProblemsandIssuesContentBanner, "content-banner");
                    bannerDTO.MajorInactiveProblemsandIssuesExclusiveBanner = WrapWithHtml(bannerDTO.MajorInactiveProblemsandIssuesExclusiveBanner, "exclusion-banner");
                    bannerDTO.OtherInactiveProblemsandIssuesContentBanner = WrapWithHtml(bannerDTO.OtherInactiveProblemsandIssuesContentBanner, "content-banner");
                    bannerDTO.OtherInactiveProblemsandIssuesExclusiveBanner = WrapWithHtml(bannerDTO.OtherInactiveProblemsandIssuesExclusiveBanner, "exclusion-banner");

                    bannerDTO.AllergiesandAdverseReactionsContentBanner = WrapWithHtml(bannerDTO.AllergiesandAdverseReactionsContentBanner, "content-banner");
                    bannerDTO.CurrentAllergiesandAdverseReactionsContentBanner = WrapWithHtml(bannerDTO.CurrentAllergiesandAdverseReactionsContentBanner, "content-banner");
                    bannerDTO.CurrentAllergiesandAdverseReactionsExclusiveBanner = WrapWithHtml(bannerDTO.CurrentAllergiesandAdverseReactionsExclusiveBanner, "exclusion-banner");
                    bannerDTO.HistoricalAllergiesandAdverseReactionsContentBanner = WrapWithHtml(bannerDTO.HistoricalAllergiesandAdverseReactionsContentBanner, "content-banner");
                    bannerDTO.HistoricalAllergiesandAdverseReactionsExclusiveBanner = WrapWithHtml(bannerDTO.HistoricalAllergiesandAdverseReactionsExclusiveBanner, "exclusion-banner");

                    bannerDTO.MedicationsContentBanner = WrapWithHtml(bannerDTO.MedicationsContentBanner, "content-banner");
                    bannerDTO.AcuteMedicationContentBanner = WrapWithHtml(bannerDTO.AcuteMedicationContentBanner, "content-banner");
                    bannerDTO.AcuteMedicationExclusiveBanner = WrapWithHtml(bannerDTO.AcuteMedicationExclusiveBanner, "exclusion-banner");
                    bannerDTO.CurrentRepeatMedicationContentBanner = WrapWithHtml(bannerDTO.CurrentRepeatMedicationContentBanner, "content-banner");
                    bannerDTO.CurrentRepeatMedicationExclusiveBanner = WrapWithHtml(bannerDTO.CurrentRepeatMedicationExclusiveBanner, "exclusion-banner");
                    bannerDTO.DiscontinuedRepeatMedicationContentBanner = WrapWithHtml(bannerDTO.DiscontinuedRepeatMedicationContentBanner, "content-banner");
                    bannerDTO.DiscontinuedRepeatMedicationExclusiveBanner = WrapWithHtml(bannerDTO.DiscontinuedRepeatMedicationExclusiveBanner, "exclusion-banner");
                    bannerDTO.AllMedicationContentBanner = WrapWithHtml(bannerDTO.AllMedicationContentBanner, "content-banner");
                    bannerDTO.AllMedicationExclusiveBanner = WrapWithHtml(bannerDTO.AllMedicationExclusiveBanner, "exclusion-banner");
                    bannerDTO.AllMedicationIssueContentBanner = WrapWithHtml(bannerDTO.AllMedicationIssueContentBanner, "content-banner");
                    bannerDTO.AllMedicationIssueExclusiveBanner = WrapWithHtml(bannerDTO.AllMedicationIssueExclusiveBanner, "exclusion-banner");

                    bannerDTO.ReferralsContentBanner = WrapWithHtml(bannerDTO.ReferralsContentBanner, "content-banner");
                    bannerDTO.ReferralsExclusiveBanner = WrapWithHtml(bannerDTO.ReferralsExclusiveBanner, "exclusion-banner");

                    bannerDTO.ObservationsContentBanner = WrapWithHtml(bannerDTO.ObservationsContentBanner, "content-banner");
                    bannerDTO.ObservationsExclusiveBanner = WrapWithHtml(bannerDTO.ObservationsExclusiveBanner, "exclusion-banner");

                    bannerDTO.ImmunisationsContentBanner = WrapWithHtml(bannerDTO.ImmunisationsContentBanner, "content-banner");
                    bannerDTO.ImmunisationsExclusiveBanner = WrapWithHtml(bannerDTO.ImmunisationsExclusiveBanner, "exclusion-banner");

                    bannerDTO.AdministrativeItemsContentBanner = WrapWithHtml(bannerDTO.AdministrativeItemsContentBanner, "content-banner");
                    bannerDTO.AdministrativeItemsExclusiveBanner = WrapWithHtml(bannerDTO.AdministrativeItemsExclusiveBanner, "exclusion-banner");

                    bannerDTO.GpTransferBanner = WrapWithHtml(bannerDTO.GpTransferBanner, "gptransfer-banner");

                }
                return bannerDTO;
            }
            catch (Exception ex)
            {
                return new BannerDTO();
            }
        }
        string WrapWithHtml(string value, string cssClass)
        {
            return !string.IsNullOrEmpty(value) ? $"<div class=\"{cssClass}\"><p>{value}</p></div>" : string.Empty;
        }
        #endregion

        #endregion

    }
}
