using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.FHIR_JSON;
using GP_Connect.FHIR_JSON.AccessHTML;
using GP_Connect.Service.Foundation;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace GP_Connect.Service.CommonMethods
{
    public class ServiceCommonMethod : IServiceCommonMethod
    {
        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();
        ServiceFoundation foundation = new ServiceFoundation();

        #endregion

        #region Constructor

        public ServiceCommonMethod()
        {
            _crmServiceClient = crmCon.crmconnectionOXVC();  
        }

        #endregion



        #region Method

        public dynamic GetAllDetailsOfPatientByPatientIdUsedForDocument(string patientId)
        {
            try
            {

                List<object> finalResponse = new List<object>();
                var PatientSequenceNumber = "";
                var PractitionerSequenceNumber = "";
                var OrganizationSequenceNumber = "";


                var contXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                  <attribute name='fullname' />
                                  <attribute name='telephone1' />
                                  <attribute name='contactid' />
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <order attribute='fullname' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + patientId + @"' />
                                  </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='practitioner'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                      <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='organization'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contXml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    if (record.Attributes.Contains("bcrm_gpc_sequence_number"))
                    {
                        PatientSequenceNumber = record["bcrm_gpc_sequence_number"].ToString();
                    }
                    if (record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                    {
                        dynamic PractitionerDetails = record["practitioner.bcrm_gpc_sequence_number"];
                        PractitionerSequenceNumber = PractitionerDetails.Value;

                    }
                    if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                    {
                        dynamic OrganizatiomnDetails = record["organization.bcrm_gpc_sequence_number"];
                        OrganizationSequenceNumber = OrganizatiomnDetails.Value;

                    }

                    if (PatientSequenceNumber != "")
                    {
                        var patientJSON = foundation.ReadAPatient(PatientSequenceNumber,"", "Internal");
                        var patient = new Dictionary<string, object>
                          {
                              { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/GP0001/STU3/1/gpconnect-documents/Patient/"+PatientSequenceNumber },
                              { "resource",  patientJSON}
                          };

                        finalResponse.Add(patient);
                    }

                    if (PractitionerSequenceNumber != "")
                    {
                        var practitionerJSON = foundation.ReadAPractioner(PractitionerSequenceNumber, "", "Internal");
                        var practitioner = new Dictionary<string, object>
                          {
                              { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/GP0001/STU3/1/gpconnect-documents/Practitioner/"+PractitionerSequenceNumber},
                              { "resource",  practitionerJSON}
                          };
                        finalResponse.Add(practitioner);


                        var practitionerRoleJSON = foundation.ReadPractionerRoleJSON(PractitionerSequenceNumber);

                        var practitionerRole = new Dictionary<string, object>
                        {
                            { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/STU3/1/gpconnect-documents/PractitionerRole/" + 85695 },
                            { "resource",  practitionerRoleJSON}
                        };
                        finalResponse.Add(practitionerRole);
                    }

                    if (OrganizationSequenceNumber != "")
                    {
                        var organizationJSON = foundation.ReadAOrganization(OrganizationSequenceNumber,"","Internal");
                        var organization = new Dictionary<string, object>
                          {
                              { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/GP0001/STU3/1/gpconnect-documents/Organization/"+OrganizationSequenceNumber},
                              { "resource",  organizationJSON}
                          };
                        finalResponse.Add(organization);
                    }
                    dynamic[] finaljson = new dynamic[3];

                    finaljson[0] = finalResponse;
                    finaljson[1] = PractitionerSequenceNumber;
                   

                    return finaljson;

                    

                }

                return null;
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        public List<object> GetAllDetailsOfPatientByNHSnumber(string nhsNumber)
        {
            try
            {
               
                List<object> finalResponse = new List<object>();
                var PatientSequenceNumber = "";
                var PractitionerSequenceNumber = "";
                var OrganizationSequenceNumber = "";


                var contXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                  <attribute name='fullname' />
                                  <attribute name='telephone1' />
                                  <attribute name='contactid' />
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <order attribute='fullname' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_nhsnumber' operator='eq' value='"+ nhsNumber + @"' />
                                  </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='practitioner'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                      <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='organization'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contXml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];  
                    
                    if(record.Attributes.Contains("bcrm_gpc_sequence_number"))
                    {
                        PatientSequenceNumber = record["bcrm_gpc_sequence_number"].ToString();
                    }
                    if(record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                    {
                        dynamic PractitionerDetails = record["practitioner.bcrm_gpc_sequence_number"];
                        PractitionerSequenceNumber = PractitionerDetails.Value;

                    }
                    if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                    {
                        dynamic OrganizatiomnDetails = record["organization.bcrm_gpc_sequence_number"];
                        OrganizationSequenceNumber = OrganizatiomnDetails.Value;

                    }
                   
                    if(PatientSequenceNumber != "")
                    {
                        var patientJSON = foundation.ReadAPatient(PatientSequenceNumber,"", "Internal");
                        finalResponse.Add(patientJSON);
                    }

                    if (PractitionerSequenceNumber != "")
                    {
                        var practitionerJSON = foundation.ReadAPractioner(PractitionerSequenceNumber, "", "Internal");
                        finalResponse.Add(practitionerJSON);
                        var practitionerRoleJSON = foundation.ReadPractionerRoleJSON(PractitionerSequenceNumber);
                        finalResponse.Add(practitionerRoleJSON);
                    }

                    if (OrganizationSequenceNumber != "")
                    {
                        var organizationJSON = foundation.ReadAOrganization(OrganizationSequenceNumber, "", "Internal");
                        finalResponse.Add(organizationJSON);
                    }

                   return finalResponse;

                }

                   return null;
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        public List<object> GetAllDetailsOfPatientByPatientIdUsedForHTMLACCESS(string nhsNumber)
        {
            try
            {

                List<object> finalResponse = new List<object>();
                var PatientSequenceNumber = "";
                var PractitionerSequenceNumber = "";
                var OrganizationSequenceNumber = "";


                var contXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                  <attribute name='fullname' />
                                  <attribute name='telephone1' />
                                  <attribute name='contactid' />
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <order attribute='fullname' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                  </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='practitioner'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                      <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='organization'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contXml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    if (record.Attributes.Contains("bcrm_gpc_sequence_number"))
                    {
                        PatientSequenceNumber = record["bcrm_gpc_sequence_number"].ToString();
                    }
                    if (record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                    {
                        dynamic PractitionerDetails = record["practitioner.bcrm_gpc_sequence_number"];
                        PractitionerSequenceNumber = PractitionerDetails.Value;

                    }
                    if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                    {
                        dynamic OrganizatiomnDetails = record["organization.bcrm_gpc_sequence_number"];
                        OrganizationSequenceNumber = OrganizatiomnDetails.Value;

                    }

                    if (PatientSequenceNumber != "")
                    {
                        var patientJSON = ReadAPatient(PatientSequenceNumber,"", "Internal");
                        var patient = new Dictionary<string, object>
                          {
                            { "fullUrl" , "Patient/"+PatientSequenceNumber },
                              { "resource",  patientJSON}
                          };
                        finalResponse.Add(patient);
                    }

                    if (PractitionerSequenceNumber != "")
                    {
                        var practitionerJSON = foundation.ReadAPractioner(PractitionerSequenceNumber, "", "Internal");
                        var practitioner = new Dictionary<string, object>
                          {
                            { "fullUrl" , "Practitioner/"+PractitionerSequenceNumber },
                              { "resource",  practitionerJSON}
                          };
                        finalResponse.Add(practitioner);
                    }

                    if (OrganizationSequenceNumber != "")
                    {
                        var organizationJSON = foundation.ReadAOrganization(OrganizationSequenceNumber, "", "Internal");
                        var organization = new Dictionary<string, object>
                          {
                            { "fullUrl" , "Organization/"+OrganizationSequenceNumber },
                            { "resource",  organizationJSON}
                          };
                        finalResponse.Add(organization);
                    }

                    return finalResponse;
                }
                return null;
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        public dynamic GetAllDetailsByNHSNumber(string nhsNumber)
        {
           try
            {
                dynamic[] finaljson = new dynamic[3];

                var PatientSequenceNumber = "";
                var PractitionerSequenceNumber = "";
                var OrganizationSequenceNumber = "";
                var OrganizationName= "";

                var contXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                  <attribute name='fullname' />
                                  <attribute name='telephone1' />
                                  <attribute name='contactid' />
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <order attribute='fullname' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                  </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='practitioner'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                      <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='organization'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                         <attribute name='bcrm_name' />
                                      </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contXml));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    if (record.Attributes.Contains("bcrm_gpc_sequence_number"))
                    {
                        PatientSequenceNumber = record["bcrm_gpc_sequence_number"].ToString();
                    }
                    if (record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                    {
                        dynamic PractitionerDetails = record["practitioner.bcrm_gpc_sequence_number"];
                        PractitionerSequenceNumber = PractitionerDetails.Value;

                    }
                    if (record.Attributes.Contains("organization.bcrm_name"))
                    {
                        dynamic OrganizationDetails = record["organization.bcrm_name"];
                        OrganizationName = OrganizationDetails.Value;

                    }
                    if (record.Attributes.Contains("organization.bcrm_gpc_sequence_number"))
                    {
                        dynamic OrganizatiomnDetails = record["organization.bcrm_gpc_sequence_number"];
                        OrganizationSequenceNumber = OrganizatiomnDetails.Value;

                    }

                    
                }
                finaljson[0] = PatientSequenceNumber;
                finaljson[1] = OrganizationName;
                finaljson[2] = OrganizationSequenceNumber;
                return finaljson;
            }
            catch(Exception ex)
            {
                dynamic[] finaljson = new dynamic[3];
                finaljson[0] = "";
                finaljson[1] = "";
                finaljson[2] = "";
                return finaljson;
            }
        }

        public string GetPractitionerSeqNumberByNHSnumber(string nhsNumber)
        {
            try
            {
                var PractitionerSequenceNumber = "";

                var contXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                  <attribute name='fullname' />
                                  <attribute name='telephone1' />
                                  <attribute name='contactid' />
                                  <attribute name='bcrm_gpc_sequence_number' />
                                  <order attribute='fullname' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_nhsnumber' operator='eq' value='" + nhsNumber + @"' />
                                  </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='practitioner'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                      </link-entity>
                                      <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='organization'>
                                        <attribute name='bcrm_gpc_sequence_number' />
                                         <attribute name='bcrm_name' />
                                      </link-entity>
                                </entity>
                              </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contXml));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    if (record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                    {
                        dynamic PractitionerDetails = record["practitioner.bcrm_gpc_sequence_number"];
                        PractitionerSequenceNumber = PractitionerDetails.Value;
                    }
                }
                return PractitionerSequenceNumber;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region Internal-Method

        internal dynamic ReadAPatient(string id, string sspInteractionId, string source)
        {
            try
            {
                PatientDetails pd = new PatientDetails();
                dynamic[] finaljson = new dynamic[3];

                if (source == "External")
                {
                    if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:read:patient-1")
                    {
                        var res = pd.WrongInteractionId(sspInteractionId, "Patient");
                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "InvalidInteractionId";
                        return finaljson;

                    }
                }


                //**************************Get Profile Data On behalf Of NHS Number ****************************************//
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                          <attribute name='lastname' />
                        <attribute name='telephone1' />
                        <attribute name='address1_telephone1' />
                        <attribute name='contactid' />
                        <attribute name='bcrm_pdsversionid' />
                        <attribute name='mobilephone' />
                        <attribute name='lastname' />
                        <attribute name='gendercode' />
                        <attribute name='firstname' />
                        <attribute name='emailaddress1' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_line1' />
                        <attribute name='address1_stateorprovince' />
                        <attribute name='address1_country' />
                        <attribute name='address1_city' />
                        <attribute name='entityimage' />
                        <attribute name='bcrm_policynumber' />
                        <attribute name='bcrm_healthcodenumber' />
                        <attribute name='bcrm_patientnumber' />
                        <attribute name='bcrm_nhsnumber' />
                        <attribute name='bcrm_ifrnhsno' />
                        <attribute name='bcrm_funded' />
                        <attribute name='parentcustomerid' />
                        <attribute name='birthdate' />
                        <attribute name='preferredcontactmethodcode' />
                        <attribute name='donotemail' />
                        <attribute name='followemail' />
                        <attribute name='donotbulkemail' />
                        <attribute name='donotphone' />
                        <attribute name='donotpostalmail' />
                        <attribute name='donotfax' />
                        <attribute name='msemr_contact1relationship' />
                        <attribute name='msemr_contact1' />
                        <attribute name='bcrm_wheredidyouhearaboutus' />
                        <attribute name='bcrm_iunderstandthatdoctornowisaprivategpservi' />
                        <attribute name='bcrm_iunderstandthatacancellationfeewillbeappl' />
                        <attribute name='bcrm_iwishtoreceivetextmessagesfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceiveemailsfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceivepostalcommunicationfromdoct' />
                        <attribute name='bcrm_iwouldliketoreceiveupdatesaboutnewservice' />
                        <attribute name='bcrm_iamhappyfordoctornowtocontactmynextofkin' />
                        <attribute name='bcrm_smokingstatus' />
                        <attribute name='bcrm_howmanyunitsofalcoholdoyoudrinkinanavera' />
                        <attribute name='bcrm_neverdrinkalcohol' />
                        <attribute name='bcrm_diabetes' />
                        <attribute name='bcrm_highbloodpressure' />
                        <attribute name='bcrm_heartdisease' />
                        <attribute name='bcrm_kidneydisease' />
                        <attribute name='bcrm_cancer' />
                        <attribute name='bcrm_thyroiddisease' />
                        <attribute name='bcrm_epilepsy' />
                        <attribute name='bcrm_depression' />
                        <attribute name='bcrm_stroke' />
                        <attribute name='bcrm_asthma' />
                        <attribute name='bcrm_gptype' />
                        <attribute name='bcrm_transientischaemicattack' />
                        <attribute name='bcrm_mentalillness' />
                        <attribute name='bcrm_heartrhythmproblems' />
                        <attribute name='bcrm_pleasegivedetailsofanyothersignificantill' />
                        <attribute name='bcrm_pleasegivedetailsofanymedicalconditionswi' />
                        <attribute name='bcrm_pleasegivedetailsifyouhaveanyallergiestoa' />
                        <attribute name='bcrm_kinsfirstname' />
                        <attribute name='bcrm_kinslastname' />
                        <attribute name='address2_line1' />
                        <attribute name='emailaddress2' />
                        <attribute name='bcrm_gpsurgeryname' />
                        <attribute name='bcrm_doctorsname' />
                        <attribute name='address3_city' />
                        <attribute name='telephone3' />
                        <attribute name='address2_postalcode' />
                         <attribute name='bcrm_occuption' />
                         <attribute name='bcrm_smokingstatus' />
                         <attribute name='bcrm_stoppedsmoking' />
                          <attribute name='bcrm_gpc_sequence_number' />
                           <attribute name='bcrm_pdsjson' />
                         <attribute name='bcrm_stoppeddrinkingalcohol' />
                          <attribute name='bcrm_gpc_registractionperiod' />
                         <attribute name='bcrm_alcoholintake' />
                        <attribute name='bcrm_modeofcommunication' />
                         <attribute name='bcrm_gpc_communicationproficiency' />
                         <attribute name='bcrm_title' />
                         <attribute name='description' />
                         <attribute name='bcrm_donotallowsms' />
                       <attribute name='statuscode' />

                    <attribute name='mobilephone' />
                    <attribute name='address2_telephone1' />
                    <attribute name='emailaddress3' />
                    <attribute name='address1_telephone1' />
                    <attribute name='emailaddress2' />

                      <attribute name='bcrm_languages' />
                      <attribute name='bcrm_interpreterrequired' />
                      <attribute name='bcrm_nhsnumberverificationstatus' />
                      <attribute name='bcrm_nhsnumberverificationstatusdisplay' />

                        <attribute name='bcrm_middlename' />
                        <attribute name='bcrm_deceaseddate' />
                         <attribute name='bcrm_age' />
                        <order attribute='fullname' descending='false' />
                        <filter type='and'>
                          <condition attribute='bcrm_gpc_sequence_number' operator='like' value='%" + id + @"%' />
                        </filter>
                       <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='staff'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='clinic'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gpc_preferredbarnchsurgery' visible='false' link-type='outer' alias='location'>
                            <attribute name='bcrm_gpcsequencenumber' />
                          </link-entity>
                       
                      </entity>
                    </fetch>";
                _crmServiceClient = crmCon.crmconnectionOXVC();
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));



                List<PatientDTO> AllPatientDetails = new List<PatientDTO>();
                PatientDTO crmUserProfile = new PatientDTO();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var ItenNo = 0;

                    for(var i = 0;i< AnswerCollection.Entities.Count;i++)
                    {
                        var rec = AnswerCollection.Entities[i].Attributes.Contains("bcrm_gpc_sequence_number") ? AnswerCollection.Entities[i]["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        if(rec == id)
                        {
                            ItenNo = i;
                        }
                    }

                    var record = AnswerCollection.Entities[ItenNo];
                    {

                        crmUserProfile.Id = record.Id;

                        dynamic staffEntity = record.Attributes.Contains("staff.bcrm_gpc_sequence_number") ? record["staff.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.stuffRefrenceNumber = staffEntity.ToString() != "" ? staffEntity.Value : string.Empty;

                        dynamic clinicEntity = record.Attributes.Contains("clinic.bcrm_gpc_sequence_number") ? record["clinic.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.clinicRefrenceNumber = clinicEntity.ToString() != "" ? clinicEntity.Value : string.Empty;

                        dynamic locationEntity = record.Attributes.Contains("location.bcrm_gpcsequencenumber") ? record["location.bcrm_gpcsequencenumber"] : string.Empty;
                        crmUserProfile.locationRefrenceNumber = locationEntity.ToString() != "" ? locationEntity.Value : string.Empty;

                        crmUserProfile.bcrm_title_label = record.Attributes.Contains("bcrm_title") ? record.FormattedValues["bcrm_title"].ToString() : string.Empty;

                        crmUserProfile.modeOfCommunication = record.Attributes.Contains("bcrm_modeofcommunication") ? record.FormattedValues["bcrm_modeofcommunication"].ToString() : string.Empty;
                        crmUserProfile.communicationProficiency = record.Attributes.Contains("bcrm_gpc_communicationproficiency") ? record.FormattedValues["bcrm_gpc_communicationproficiency"].ToString() : string.Empty;

                        dynamic check = record.Attributes.Contains("Patient_relationships") ? record["Patient_relationships"].ToString() : string.Empty;

                        crmUserProfile.bcrm_middlename = record.Attributes.Contains("bcrm_middlename") ? record["bcrm_middlename"].ToString() : string.Empty;
                        crmUserProfile.fullname = record.Attributes.Contains("fullname") ? record["fullname"].ToString() : string.Empty;
                        crmUserProfile.firstname = record.Attributes.Contains("firstname") ? record["firstname"].ToString() : string.Empty;
                        crmUserProfile.lastname = record.Attributes.Contains("lastname") ? record["lastname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_age = record.Attributes.Contains("bcrm_age") ? record["bcrm_age"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;
                        crmUserProfile.emailaddress1 = record.Attributes.Contains("emailaddress1") ? record["emailaddress1"].ToString() : string.Empty;
                        crmUserProfile.address1_city = record.Attributes.Contains("address1_city") ? record["address1_city"].ToString() : "";
                        crmUserProfile.address1_country = record.Attributes.Contains("address1_country") ? record["address1_country"].ToString() : "";
                        crmUserProfile.address1_line1 = record.Attributes.Contains("address1_line1") ? record["address1_line1"].ToString() : "";
                        crmUserProfile.telephone1 = record.Attributes.Contains("telephone1") ? record["telephone1"].ToString() : "";
                        crmUserProfile.address1_stateorprovince = record.Attributes.Contains("address1_stateorprovince") ? record["address1_stateorprovince"].ToString() : "";
                        crmUserProfile.address1_postalcode = record.Attributes.Contains("address1_postalcode") ? record["address1_postalcode"].ToString() : "";
                        if (record.Attributes.Contains("entityimage")) crmUserProfile.entityimage = record["entityimage"] as byte[];
                        if (record.Attributes.Contains("birthdate")) { crmUserProfile.birthdate = (DateTime)record.Attributes["birthdate"]; }
                        crmUserProfile.gender = record.Attributes.Contains("gendercode") ? record.FormattedValues["gendercode"].ToString() : string.Empty;

                        crmUserProfile.gendercode = record.Contains("gendercode") && record["gendercode"] != null ? Convert.ToInt32(((OptionSetValue)record["gendercode"]).Value) : 0;
                        crmUserProfile.bcrm_policynumber = record.Attributes.Contains("bcrm_policynumber") ? record["bcrm_policynumber"].ToString() : "";
                        crmUserProfile.bcrm_healthcodenumber = record.Attributes.Contains("bcrm_healthcodenumber") ? record["bcrm_healthcodenumber"].ToString() : "";
                        crmUserProfile.bcrm_patientnumber = record.Attributes.Contains("bcrm_patientnumber") ? record["bcrm_patientnumber"].ToString() : "";
                        crmUserProfile.bcrm_nhsnumber = record.Attributes.Contains("bcrm_nhsnumber") ? record["bcrm_nhsnumber"].ToString() : "";
                        crmUserProfile.bcrm_ifrnhsno = record.Attributes.Contains("bcrm_ifrnhsno") ? record["bcrm_ifrnhsno"].ToString() : "";
                        crmUserProfile.bcrm_funded_name = record.Attributes.Contains("bcrm_funded") ? record.FormattedValues["bcrm_funded"].ToString() : string.Empty;
                        crmUserProfile.bcrm_funded = record.Contains("bcrm_funded") && record["bcrm_funded"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_funded"]).Value) : 0;
                        crmUserProfile.preferredcontactmethodcode = record.Contains("preferredcontactmethodcode") && record["preferredcontactmethodcode"] != null ? Convert.ToInt32(((OptionSetValue)record["preferredcontactmethodcode"]).Value) : 0;
                        crmUserProfile.parentcustomerid = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Id : Guid.Empty;
                        crmUserProfile.parentcustomername = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Name : string.Empty;
                        crmUserProfile.donotemail = record.Attributes.Contains("donotemail") ? ((bool)record["donotemail"]) : false;
                        crmUserProfile.followemail = record.Attributes.Contains("followemail") ? ((bool)record["followemail"]) : false;
                        crmUserProfile.donotbulkemail = record.Attributes.Contains("donotbulkemail") ? ((bool)record["donotbulkemail"]) : false;
                        crmUserProfile.donotphone = record.Attributes.Contains("donotphone") ? ((bool)record["donotphone"]) : false;
                        crmUserProfile.donotpostalmail = record.Attributes.Contains("donotpostalmail") ? ((bool)record["donotpostalmail"]) : false;
                        crmUserProfile.donotfax = record.Attributes.Contains("donotfax") ? ((bool)record["donotfax"]) : false;
                        //Fields from Doctor's Now
                        crmUserProfile.msemr_contact1relationship = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Id : Guid.Empty;
                        crmUserProfile.msemr_contact1relationship_name = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Name : string.Empty;
                        crmUserProfile.msemr_contact1 = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Id : Guid.Empty;
                        crmUserProfile.emergencycontact = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Name : String.Empty;
                        crmUserProfile.bcrm_wheredidyouhearaboutus = record.Contains("bcrm_wheredidyouhearaboutus") && record["bcrm_wheredidyouhearaboutus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_wheredidyouhearaboutus"]).Value) : 0;
                        crmUserProfile.bcrm_gptype = record.Contains("bcrm_gptype") && record["bcrm_gptype"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_gptype"]).Value) : 0;
                        crmUserProfile.bcrm_iunderstandthatdoctornowisaprivategpservi = record.Attributes.Contains("bcrm_iunderstandthatdoctornowisaprivategpservi") ? ((bool)record["bcrm_iunderstandthatdoctornowisaprivategpservi"]) : false;
                        crmUserProfile.bcrm_iunderstandthatacancellationfeewillbeappl = record.Attributes.Contains("bcrm_iunderstandthatacancellationfeewillbeappl") ? ((bool)record["bcrm_iunderstandthatacancellationfeewillbeappl"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivetextmessagesfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceivetextmessagesfromdoctornow") ? ((bool)record["bcrm_iwishtoreceivetextmessagesfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceiveemailsfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceiveemailsfromdoctornow") ? ((bool)record["bcrm_iwishtoreceiveemailsfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivepostalcommunicationfromdoct = record.Attributes.Contains("bcrm_iwishtoreceivepostalcommunicationfromdoct") ? ((bool)record["bcrm_iwishtoreceivepostalcommunicationfromdoct"]) : false;
                        crmUserProfile.bcrm_iwouldliketoreceiveupdatesaboutnewservice = record.Attributes.Contains("bcrm_iwouldliketoreceiveupdatesaboutnewservice") ? ((bool)record["bcrm_iwouldliketoreceiveupdatesaboutnewservice"]) : false;
                        crmUserProfile.bcrm_iamhappyfordoctornowtocontactmynextofkin = record.Attributes.Contains("bcrm_iamhappyfordoctornowtocontactmynextofkin") ? ((bool)record["bcrm_iamhappyfordoctornowtocontactmynextofkin"]) : false;
                        crmUserProfile.bcrm_smokingstatus = record.Contains("bcrm_smokingstatus") && record["bcrm_smokingstatus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_smokingstatus"]).Value) : 0;
                        crmUserProfile.bcrm_occuption = record.Attributes.Contains("bcrm_occuption") ? record["bcrm_occuption"].ToString() : "";
                        crmUserProfile.bcrm_stoppedsmoking = record.Attributes.Contains("bcrm_stoppedsmoking") ? record["bcrm_stoppedsmoking"].ToString() : "";
                        crmUserProfile.bcrm_stoppeddrinkingalcohol = record.Attributes.Contains("bcrm_stoppeddrinkingalcohol") ? record["bcrm_stoppeddrinkingalcohol"].ToString() : "";
                        crmUserProfile.bcrm_alcoholintake = record.Contains("bcrm_alcoholintake") && record["bcrm_alcoholintake"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_alcoholintake"]).Value) : 0;
                        crmUserProfile.bcrm_alcoholintake_name = record.Attributes.Contains("bcrm_alcoholintake") ? record.FormattedValues["bcrm_alcoholintake"].ToString() : string.Empty;
                        crmUserProfile.otherDetails = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                        crmUserProfile.bcrm_howmanyunitsofalcoholdoyoudrinkinanavera = record.Attributes.Contains("bcrm_howmanyunitsofalcoholdoyoudrinkinanavera") ? record["bcrm_howmanyunitsofalcoholdoyoudrinkinanavera"].ToString() : string.Empty;
                        crmUserProfile.bcrm_neverdrinkalcohol = record.Attributes.Contains("bcrm_neverdrinkalcohol") ? ((bool)record["bcrm_neverdrinkalcohol"]) : false;
                        crmUserProfile.bcrm_diabetes = record.Attributes.Contains("bcrm_diabetes") ? ((bool)record["bcrm_diabetes"]) : false;
                        crmUserProfile.bcrm_highbloodpressure = record.Attributes.Contains("bcrm_highbloodpressure") ? ((bool)record["bcrm_highbloodpressure"]) : false;
                        crmUserProfile.bcrm_heartdisease = record.Attributes.Contains("bcrm_heartdisease") ? ((bool)record["bcrm_heartdisease"]) : false;
                        crmUserProfile.bcrm_kidneydisease = record.Attributes.Contains("bcrm_kidneydisease") ? ((bool)record["bcrm_kidneydisease"]) : false;
                        crmUserProfile.bcrm_cancer = record.Attributes.Contains("bcrm_cancer") ? ((bool)record["bcrm_cancer"]) : false;
                        crmUserProfile.bcrm_thyroiddisease = record.Attributes.Contains("bcrm_thyroiddisease") ? ((bool)record["bcrm_thyroiddisease"]) : false;
                        crmUserProfile.bcrm_epilepsy = record.Attributes.Contains("bcrm_epilepsy") ? ((bool)record["bcrm_epilepsy"]) : false;
                        crmUserProfile.bcrm_depression = record.Attributes.Contains("bcrm_depression") ? ((bool)record["bcrm_depression"]) : false;
                        crmUserProfile.bcrm_stroke = record.Attributes.Contains("bcrm_stroke") ? ((bool)record["bcrm_stroke"]) : false;
                        crmUserProfile.bcrm_asthma = record.Attributes.Contains("bcrm_asthma") ? ((bool)record["bcrm_asthma"]) : false;
                        crmUserProfile.bcrm_transientischaemicattack = record.Attributes.Contains("bcrm_transientischaemicattack") ? ((bool)record["bcrm_transientischaemicattack"]) : false;
                        crmUserProfile.bcrm_mentalillness = record.Attributes.Contains("bcrm_mentalillness") ? ((bool)record["bcrm_mentalillness"]) : false;
                        crmUserProfile.bcrm_heartrhythmproblems = record.Attributes.Contains("bcrm_heartrhythmproblems") ? ((bool)record["bcrm_heartrhythmproblems"]) : false;
                        crmUserProfile.bcrm_pleasegivedetailsofanyothersignificantill = record.Attributes.Contains("bcrm_pleasegivedetailsofanyothersignificantill") ? record["bcrm_pleasegivedetailsofanyothersignificantill"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsofanymedicalconditionswi = record.Attributes.Contains("bcrm_pleasegivedetailsofanymedicalconditionswi") ? record["bcrm_pleasegivedetailsofanymedicalconditionswi"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsifyouhaveanyallergiestoa = record.Attributes.Contains("bcrm_pleasegivedetailsifyouhaveanyallergiestoa") ? record["bcrm_pleasegivedetailsifyouhaveanyallergiestoa"].ToString() : "";
                        crmUserProfile.bcrm_kinsfirstname = record.Attributes.Contains("bcrm_kinsfirstname") ? record["bcrm_kinsfirstname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_kinslastname = record.Attributes.Contains("bcrm_kinslastname") ? record["bcrm_kinslastname"].ToString() : string.Empty;
                        crmUserProfile.address2_line1 = record.Attributes.Contains("address2_line1") ? record["address2_line1"].ToString() : string.Empty;
                        crmUserProfile.emailaddress2 = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.address2_postalcode = record.Attributes.Contains("address2_postalcode") ? record["address2_postalcode"].ToString() : string.Empty;
                        crmUserProfile.bcrm_gpsurgeryname = record.Attributes.Contains("bcrm_gpsurgeryname") ? record["bcrm_gpsurgeryname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_doctorsname = record.Attributes.Contains("bcrm_doctorsname") ? record["bcrm_doctorsname"].ToString() : string.Empty;
                        crmUserProfile.address3_city = record.Attributes.Contains("address3_city") ? record["address3_city"].ToString() : string.Empty;
                        crmUserProfile.telephone3 = record.Attributes.Contains("telephone3") ? record["telephone3"].ToString() : string.Empty;
                        crmUserProfile.bcrm_donotallowsms = record.Contains("bcrm_donotallowsms") && record["bcrm_donotallowsms"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_donotallowsms"]).Value) : 0;
                        crmUserProfile.GPCSequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_gpc_registractionperiod")) { crmUserProfile.GPCRegistractionDate = (DateTime)record.Attributes["bcrm_gpc_registractionperiod"]; }
                        crmUserProfile.HomePhone1 = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.PdsVersionId = record.Attributes.Contains("bcrm_pdsversionid") ? record["bcrm_pdsversionid"].ToString() : string.Empty;
                        crmUserProfile.pdsJson = record.Attributes.Contains("bcrm_pdsjson") ? record["bcrm_pdsjson"].ToString() : string.Empty;

                        crmUserProfile.InterpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Code = record.Attributes.Contains("bcrm_nhsnumberverificationstatus") ? record["bcrm_nhsnumberverificationstatus"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Display = record.Attributes.Contains("bcrm_nhsnumberverificationstatusdisplay") ? record["bcrm_nhsnumberverificationstatusdisplay"].ToString() : string.Empty;

                        crmUserProfile.homeEmail = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.homePhone = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.workEmail = record.Attributes.Contains("emailaddress3") ? record["emailaddress3"].ToString() : string.Empty;
                        crmUserProfile.workPhone = record.Attributes.Contains("address2_telephone1") ? record["address2_telephone1"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;

                        if (record.Attributes.Contains("bcrm_deceaseddate")) { crmUserProfile.deceasedDate = (DateTime)record.Attributes["bcrm_deceaseddate"]; }

                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            crmUserProfile.statusReason = true;
                        }
                        else
                        {
                            crmUserProfile.statusReason = false;
                        }
                        

                        if (crmUserProfile.pdsJson != "")
                        {
                            try
                            {
                                //var PatientData = JsonConvert.DeserializeObject<PDSDTO>(crmUserProfile.pdsJson);
                                //crmUserProfile.NHS_number_Verification_Status_Code = PatientData.identifier[0].extension[0].valueCodeableConcept.coding[0].code;
                                //crmUserProfile.NHS_number_Verification_Status_Display = PatientData.identifier[0].extension[0].valueCodeableConcept.coding[0].display;
                                //crmUserProfile.LanguageCode = PatientData.extension[2].extension[0].valueCodeableConcept.coding[0].code;
                                //crmUserProfile.LanguageDisplay = PatientData.extension[2].extension[0].valueCodeableConcept.coding[0].display;
                                //crmUserProfile.InterpreterRequired = PatientData.extension[2].extension[1].valueBoolean.ToString();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message.ToString());
                            }
                        }
                        var patientLanguages = record.Attributes.Contains("bcrm_languages") ? record.FormattedValues["bcrm_languages"].ToString() : string.Empty;
                        if (patientLanguages != "")
                        {
                            string[] allLanguages = patientLanguages.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            crmUserProfile.patientLanguages = languageList;

                        }
                        AllPatientDetails.Add(crmUserProfile);
                    }

                    if (AllPatientDetails.Count > 0)
                    {
                        AccessHTMLDetails accessHTMLDetails = new AccessHTMLDetails();
                        if (source == "Internal")
                        {
                            return accessHTMLDetails.ReadAPatientUsingJSONFHIR(AllPatientDetails[0]);
                        }


                        var res = pd.ReadAPatientUsingJSONFHIR(AllPatientDetails[0]);
                        finaljson[0] = res;
                        finaljson[1] = crmUserProfile.PdsVersionId;
                        finaljson[2] = "Success";
                        return finaljson;

                    }
                }
                PatientDetails pd1 = new PatientDetails();

                finaljson[0] = pd1.PatientNotFoundUsingJSONFHIR(id);
                finaljson[1] = "";
                finaljson[2] = "NotFound";
                return finaljson;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }


        #endregion

    }
}
