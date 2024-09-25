using GP_Connect.CRM_Connection;
using GP_Connect.FHIR_JSON;
using GP_Connect.Service.Foundation;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
                        var patientJSON = foundation.ReadAPatient(PatientSequenceNumber,"", "Internal");
                        var patient = new Dictionary<string, object>
                          {
                              { "resource",  patientJSON}
                          };
                        finalResponse.Add(patient);
                    }

                    if (PractitionerSequenceNumber != "")
                    {
                        var practitionerJSON = foundation.ReadAPractioner(PractitionerSequenceNumber, "", "Internal");
                        var practitioner = new Dictionary<string, object>
                          {
                              { "resource",  practitionerJSON}
                          };
                        finalResponse.Add(practitioner);
                      
                    }

                    if (OrganizationSequenceNumber != "")
                    {
                        var organizationJSON = foundation.ReadAOrganization(OrganizationSequenceNumber, "", "Internal");
                        var organization = new Dictionary<string, object>
                          {
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
        #endregion


    }
}
