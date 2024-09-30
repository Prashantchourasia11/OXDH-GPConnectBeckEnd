using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.FHIR_JSON.AppointmentManagement;
using GP_Connect.Service.CommonMethods;
using Microsoft.SharePoint.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Swagger.Net;
using System.Reflection.Metadata;
using System.Security;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using RestSharp;
using GP_Connect.Service.Foundation;
using GP_Connect.FHIR_JSON.AccessDocument;

namespace GP_Connect.Service.AccessDocument
{
    public class ServiceAccessDocument
    {
        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();
        ServiceCommonMethod scm = new ServiceCommonMethod();
        ServiceFoundation foundation = new ServiceFoundation();
        #endregion

        #region Constructor

        public ServiceAccessDocument()
        {

            _crmServiceClient = crmCon.crmconnectionOXVC();
        }

        #endregion

        #region Method

        public dynamic GetDocumentReference(string patientId, string Createdstart, string CreatedEnd, string author, string description,string SspTraceId,string fullUrl)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];

                //DocumentDetails dd = new DocumentDetails();
                //return dd.DocumentReferenceTurnedOff();

                if(fullUrl.Contains("BadParam"))
                {
                    DocumentDetails dd = new DocumentDetails();
                    finaljson[0] = dd.InvalidParameterJSON();
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if(!fullUrl.Contains("_include"))
                {
                    DocumentDetails dd = new DocumentDetails();
                    finaljson[0] = dd.InvalidParameterJSON();
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
              
                if (fullUrl != null)
                {
                    var queryString = fullUrl.Split('?').LastOrDefault();

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        var queryParams = queryString.Split('&')
                                          .Select(param => param.Split(new[] { '=' }, 2)) // Split only at the first '='
                                          .Where(parts => parts.Length == 2) // Ensure there are both key and value
                                          .ToLookup(parts => parts[0], parts => Uri.UnescapeDataString(parts[1]));



                        if (queryParams.Contains("author"))
                        {
                            var author1 = queryParams["author"].FirstOrDefault(); // Since there's only one 'author'

                            // Check if the 'author' contains any invalid characters like '.' or '|'
                            if (!author1.Contains("https://fhir.nhs.uk/Id/ods-organization-code"))
                            {
                                DocumentDetails dd = new DocumentDetails();
                                finaljson[0] = dd.InvalidAuthoreJSON();
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                            else
                            {
                               
                            }
                        }
                        else
                        {
                            
                        }
                    }
                }




            

                BundleResponseDTO bundleResponse = new BundleResponseDTO();
                bundleResponse.resourceType = "Bundle";
                bundleResponse.id = SspTraceId;

                bundleResponse.meta = new BundleResponseDTOMeta();
                bundleResponse.meta.profile = new List<string>();

                bundleResponse.meta.profile.Add("https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Searchset-Bundle-1");
                bundleResponse.type = "searchset";


                List<object> finalResponse = new List<object>();

                var tempPatient = IsPatientAdminttedInTemporary(patientId);
                if(tempPatient == true)
                {
                    finaljson[0] = patientNotFoundJSON();
                    finaljson[1] = "";
                    finaljson[2] = "404";
                    return finaljson;
                }


                var patientDetails = scm.GetAllDetailsOfPatientByPatientIdUsedForDocument(patientId);
                if (patientDetails == null)
                {
                    finaljson[0] = patientNotFoundJSON();
                    finaljson[1] = "";
                    finaljson[2] = "404";
                    return finaljson;
                }

                foreach (var item in patientDetails[0])
                {
                    finalResponse.Add(item);
                }
                var documentList = GetAllDocument(patientId, Createdstart, CreatedEnd, author, description, patientDetails[1]);
               
                foreach (var item in documentList)
                {
                    finalResponse.Add(item);
                }

                bundleResponse.entry = finalResponse;

                finaljson[0] = bundleResponse;
                finaljson[1] = "";
                finaljson[2] = "200";
                return finaljson;
               
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public dynamic GetBase64UsingCRMGuid(string crmGuid)
        {
            try
            {
                var filePath = "";
                var contentType = "";
                var base64 = "";
                var fileXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bcrm_filesharing'>
                                      <attribute name='bcrm_filesharingid' />
                                      <attribute name='bcrm_name' />
                                      <attribute name='createdon' />
                                      <attribute name='bcrm_fileurl' />
                                       <attribute name='bcrm_confidentiality' />
                                      <attribute name='bcrm_contenttype' />
                                      <order attribute='bcrm_name' descending='false' />
                                      <filter type='and'>
                                        <condition attribute='bcrm_filesharingid' operator='eq' uiname='Inpatient final discharge letter' uitype='bcrm_filesharing' value='{" + crmGuid+@"}' />
                                      </filter>
                                    </entity>
                                  </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(fileXML));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    filePath = record.Attributes.Contains("bcrm_fileurl") ? record["bcrm_fileurl"].ToString() : string.Empty;
                    contentType = record.Attributes.Contains("bcrm_contenttype") ? record["bcrm_contenttype"].ToString() : string.Empty;
                    var confidentially = record.Attributes.Contains("bcrm_confidentiality") ? record["bcrm_confidentiality"] : string.Empty;

                    if(confidentially.ToString().ToLower() == "true")
                    {
                        return DocumentNotFoundJSON();
                    }

                    dynamic resul = GetDataByFilePath(filePath);

                    //if (filePath != "")
                    //{
                    //    var client = new HttpClient();
                    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://prod-20.uksouth.logic.azure.com:443/workflows/94a189ef5321420db19d999e6852c588/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UDq7NSekPW9aQFPealFXgXUPtsW-mWLDoS9DMa138vA");
                    //    request.Headers.Add("Accept", "application/json");
                    //    request.Headers.Add("Cookie", "ARRAffinity=f2f616db1fb852bebf8d33f4c2e809bd2033537707f3f091cc1bbac7ec19b816; ARRAffinitySameSite=f2f616db1fb852bebf8d33f4c2e809bd2033537707f3f091cc1bbac7ec19b816");
                    //    var content = new StringContent(filePath, null, "text/plain");
                    //    request.Content = content;
                    //    var response = await client.SendAsync(request);
                    //    response.EnsureSuccessStatusCode();
                    //    base64 = await response.Content.ReadAsStringAsync();
                    //}

                    ResponseDocumentBase64 res = new ResponseDocumentBase64();
                    res.id = crmGuid;
                    res.resourceType = "Binary";
                    res.contentType = contentType;
                    res.content = resul.Result;
                        
                    return res;

                }

                return DocumentNotFoundJSON();
            }
            catch(Exception ex) 
            {
                return null;
            }
        }

        public async Task<string> GetDataByFilePath(string filepath)
        {
            if (filepath != "")
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://prod-20.uksouth.logic.azure.com:443/workflows/94a189ef5321420db19d999e6852c588/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UDq7NSekPW9aQFPealFXgXUPtsW-mWLDoS9DMa138vA");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Cookie", "ARRAffinity=f2f616db1fb852bebf8d33f4c2e809bd2033537707f3f091cc1bbac7ec19b816; ARRAffinitySameSite=f2f616db1fb852bebf8d33f4c2e809bd2033537707f3f091cc1bbac7ec19b816");
                var content = new StringContent(filepath, null, "text/plain");
                request.Content = content;
                var response = client.Send(request);
                var resy = await response.Content.ReadAsStringAsync();
                return resy;
            }
            return null;

        }


        #endregion


        #region Internal Method

        internal List<object> GetAllDocument(string patientId, string Createdstart, string CreatedEnd, string author, string description,string practitionerId)
        {
            try
            {
                List<string> PractitionerDocumentSeq = new List<string>();

                var filterstringforquestionar = "";
                //if (Createdstart != null)
                //{
                //    Createdstart = Createdstart.Replace("ge", "");
                //    CreatedEnd = CreatedEnd.Replace("le", "");
                //    filterstringforquestionar += "<condition attribute='createdon' operator='on-or-after' value='" + Createdstart + @"' />";
                //    filterstringforquestionar += "<condition attribute='createdon' operator='on-or-before' value='" + CreatedEnd + @"' />";
                //}
                //if(author != null)
                //{
                //    filterstringforquestionar += "<condition attribute='bcrm_authorpractitioner' operator='eq' uiname='Prashant Chourasiya' uitype='bcrm_staff' value='{"+author+@"}' />";
                //}
                //if(description != null) 
                //{
                //    filterstringforquestionar += "<condition attribute='bcrm_description' operator='like' value='"+ description + @"' />";
                //}

                List<object> documentList = new List<object>();

                var documentXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bcrm_filesharing'>
                                    <attribute name='bcrm_filesharingid' />
                                    <attribute name='bcrm_name' />
                                    <attribute name='createdon' />
                                    <attribute name='statuscode' />
                                    <attribute name='statecode' />
                                    <attribute name='bcrm_sl_servicedisplay' />
                                    <attribute name='bcrm_sl_servicecode' />
                                    <attribute name='bcrm_sl_documentdisplay' />
                                    <attribute name='bcrm_sl_documentcode' />
                                    <attribute name='bcrm_sequencenumber' />
                                    <attribute name='overriddencreatedon' />
                                    <attribute name='bcrm_pin' />
                                    <attribute name='owningbusinessunit' />
                                    <attribute name='ownerid' />
                                    <attribute name='modifiedon' />
                                    <attribute name='modifiedonbehalfby' />
                                    <attribute name='modifiedby' />
                                    <attribute name='bcrm_indexeddate' />
                                    <attribute name='bcrm_gpstatus' />
                                    <attribute name='bcrm_folderidentifierpath' />
                                    <attribute name='bcrm_fileurl' />
                                    <attribute name='bcrm_fileshorturl' />
                                    <attribute name='bcrm_filesasurl' />
                                    <attribute name='bcrm_filename' />
                                    <attribute name='bcrm_confidentiality' />
                                    <attribute name='bcrm_fileaccesstime' />
                                    <attribute name='bcrm_encounter' />
                                    <attribute name='bcrm_documentsizemb' />
                                    <attribute name='bcrm_description' />
                                    <attribute name='bcrm_dateofbirth' />
                                    <attribute name='createdonbehalfby' />
                                    <attribute name='createdby' />
                                    <attribute name='bcrm_contenturl' />
                                    <attribute name='bcrm_contenttype' />
                                    <attribute name='bcrm_contact' />
                                    <attribute name='bcrm_indexeddate' />
                                    <attribute name='bcrm_authorpractitioner' />
                                    <link-entity name='msemr_encounter' from='msemr_encounterid' to='bcrm_encounter' visible='false' link-type='outer' alias='encounter'>
                                          <attribute name='msemr_encounteridentifier' />
                                    </link-entity>
                                    <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_authorpractitioner' visible='false' link-type='outer' alias='practitioner'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='bcrm_contact' link-type='inner' alias='patient'>
                                       <attribute name='bcrm_gms1type' />
                                      <filter type='and'>
                                        <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + patientId+@"' />
                                        "+ filterstringforquestionar + @"
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(documentXml));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach ( var record in AnswerCollection.Entities)
                    {
                        DocumentReferenceDTO documentDetails = new DocumentReferenceDTO();
                        documentDetails.masterIdentifierCRMGuid = record.Id.ToString();
                        documentDetails.patientSequenceNumber = patientId;
                        documentDetails.SequenceId =  record.Attributes.Contains("bcrm_sequencenumber") ? record["bcrm_sequencenumber"].ToString() : string.Empty;
                        documentDetails.documentCode = record.Attributes.Contains("bcrm_sl_documentcode") ? record["bcrm_sl_documentcode"].ToString() : string.Empty;
                        documentDetails.documentDisplay = record.Attributes.Contains("bcrm_sl_documentdisplay") ? record["bcrm_sl_documentdisplay"].ToString() : string.Empty;
                        
                        documentDetails.description = record.Attributes.Contains("bcrm_description") ? record["bcrm_description"].ToString() : string.Empty;
                        documentDetails.contentType = record.Attributes.Contains("bcrm_contenttype") ? record["bcrm_contenttype"].ToString() : string.Empty;
                        documentDetails.fileUrl = record.Attributes.Contains("bcrm_fileurl") ? record["bcrm_fileurl"].ToString() : string.Empty;
                        documentDetails.size = record.Attributes.Contains("bcrm_documentsizemb") ? record["bcrm_documentsizemb"].ToString() : string.Empty;
                        documentDetails.serviceCode = record.Attributes.Contains("bcrm_sl_servicecode") ? record["bcrm_sl_servicecode"].ToString() : string.Empty;
                        documentDetails.serviceDisplay = record.Attributes.Contains("bcrm_sl_servicedisplay") ? record["bcrm_sl_servicedisplay"].ToString() : string.Empty;
                       documentDetails.encounterNumber = record.Attributes.Contains("encounter.msemr_encounteridentifier") ? record["encounter.msemr_encounteridentifier"].ToString() : string.Empty;
                        documentDetails.status = record.Attributes.Contains("bcrm_gpstatus") ? record["bcrm_gpstatus"].ToString() : string.Empty;

                        var patientRegType = record.Attributes.Contains("patient.bcrm_gms1type") ? record.FormattedValues["patient.bcrm_gms1type"].ToString() : string.Empty;
                        var confidentially = record.Attributes.Contains("bcrm_confidentiality") ? record["bcrm_confidentiality"] : string.Empty;

                        if (confidentially.ToString().ToLower() == "true")
                        {
                            documentDetails.size = "0";
                        }
                        if (patientRegType == "Temporary")
                        {
                            return new List<object>();
                        }


                        if (record.Attributes.Contains("encounter.msemr_encounteridentifier"))
                        {
                            dynamic encounter = record["encounter.msemr_encounteridentifier"];
                            documentDetails.encounterNumber = encounter.Value;
                        }
                        if (record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number"))
                        {
                            dynamic practitioner = record["practitioner.bcrm_gpc_sequence_number"];
                            documentDetails.practitionerSequenceNumber = practitioner.Value;
                        }

                        if (record.Attributes.Contains("createdon")) { documentDetails.createdOn = (DateTime)record.Attributes["createdon"]; }
                        if (record.Attributes.Contains("bcrm_indexeddate")) { documentDetails.inexed = (DateTime)record.Attributes["bcrm_indexeddate"]; }

                        if(documentDetails.practitionerSequenceNumber != practitionerId)
                        {
                            var resu1 = false;
                            for(var i =0; i< PractitionerDocumentSeq.Count;i++)
                            {
                                if (PractitionerDocumentSeq[i] == documentDetails.practitionerSequenceNumber)
                                {
                                    resu1 = false;
                                }
                                else
                                {
                                    resu1 = true;
                                }
                            }
                            if(PractitionerDocumentSeq.Count == 0 || resu1 == true)
                            {
                                PractitionerDocumentSeq.Add(documentDetails.practitionerSequenceNumber);

                                var practitionerJSON = foundation.ReadAPractioner(documentDetails.practitionerSequenceNumber, "", "Internal");
                                var practitioner = new Dictionary<string, object>
                                 {
                              { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/GP0001/STU3/1/gpconnect-documents/Practitioner/"+documentDetails.practitionerSequenceNumber},
                              { "resource",  practitionerJSON}
                                  };
                                documentList.Add(practitioner);

                                var practitionerRoleJSON = foundation.ReadPractionerRoleJSON(documentDetails.practitionerSequenceNumber);

                                var practitionerRole = new Dictionary<string, object>
                             {
                            { "fullUrl", "https://oxdhgpconnect.azurewebsites.net/STU3/1/gpconnect-documents/PractitionerRole/" + 25695 },
                            { "resource",  practitionerRoleJSON}
                             };
                                documentList.Add(practitionerRole);
                            }

                           

                        }


                        documentList.Add(GetDocumentListToModel(documentDetails));

                    }
                }


                return documentList;
            }
            catch(Exception)
            {
                return new List<object>();
            }

        }

        internal object GetDocumentListToModel(DocumentReferenceDTO DRDTO)
        {
            try
            {

                var documentReference = new Dictionary<string, object>
        {
            { "resourceType", "DocumentReference" },
            { "id", DRDTO.SequenceId },
            {
                "meta", new Dictionary<string, object>
                {
                    {
                        "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-DocumentReference-1"
                        }
                    }
                }
            },
            {
                "masterIdentifier", new Dictionary<string, object>
                {
                    { "system", "LocalSystem/1" },
                    { "value", DRDTO.masterIdentifierCRMGuid }
                }
            },
            {
                "identifier", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "system", "https://fhir.nhs.uk/Id/cross-care-setting-identifier" },
                        { "value", DRDTO.masterIdentifierCRMGuid }
                    }
                }
            },
            { "status", "current" },
            {
                "type", new Dictionary<string, object>
                {
                    {
                        "coding", new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                { "system", "http://snomed.info/sct" },
                                { "code", DRDTO.documentCode },
                                { "display", DRDTO.documentDisplay }
                            }
                        }
                    }
                }
            },
            { "created", DRDTO.createdOn.ToString("yyyy-MM-ddTHH:mm:sszzz")},
            { "indexed", DRDTO.inexed.ToString("yyyy-MM-ddTHH:mm:sszzz")},
             {
                "subject", new Dictionary<string, object>
                {
                    { "reference", "Patient/" + DRDTO.patientSequenceNumber }
                }
            },
            {
                "author", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "reference", "Practitioner/" +DRDTO.practitionerSequenceNumber }
                    }
                }
            },
            { "description", DRDTO.description },
            {
              "content" , contentCheckFile(DRDTO)
            },
            {
                "context", new Dictionary<string, object>
                {
                    {
                        "practiceSetting", new Dictionary<string, object>
                        {
                            {
                                "coding", new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        { "system", "http://snomed.info/sct" },
                                        { "code", DRDTO.serviceCode },
                                        { "display", DRDTO.serviceDisplay }
                                    }
                                }
                            }
                        }
                    },
                    {
                        "encounter", new Dictionary<string, object>
                        {
                            { "reference", "Encounter/" + DRDTO.encounterNumber }
                        }
                    }
                }
            },
            {
                        "custodian", new Dictionary<string, object>
                        {
                            { "reference", "Organization/1000"}
                        }
                    }
        };

                //, I have commented this code because of error
               // {
                 //   "custodian", new Dictionary<string, object>
                   //     {
                     //       { "reference", "Organization/" + 1000 }
                       // }
                   // }
                //

                var documentReferenceFinal = new Dictionary<string, object>
                          {
                              { "fullUrl", "https://test-gpc-w7m0i.oxdh.thirdparty.nhs.uk/W7M0I/STU3/1/gpconnect/Binary/" + DRDTO.masterIdentifierCRMGuid },
                              { "resource",  documentReference}
                          };

                return documentReferenceFinal;
            }
            catch(Exception ex) 
            {
             return new object();   
            }
        }

        internal object DocumentNotFoundJSON()
        {
            var operationOutcome = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                        }
                    }
                }
            },
            { "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "not-found" },
                        { "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/STU3/ValueSet/Spine-ErrorOrWarningCode-1" },
                                            { "code", "NO_RECORD_FOUND" },
                                            { "display", "No record found" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
            return operationOutcome;
        }

        internal object patientNotFoundJSON()
        {
            var operationOutcome = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                        }
                    }
                }
            },
            { "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "not-found" },
                        { "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/STU3/ValueSet/Spine-ErrorOrWarningCode-1" },
                                            { "code", "PATIENT_NOT_FOUND" },
                                            { "display", "Patient not found" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
            return operationOutcome;
        }

        internal bool IsPatientAdminttedInTemporary(string patientId)
        {
            var xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                        <entity name='contact'>
                          <attribute name='fullname' />
                          <attribute name='telephone1' />
                          <attribute name='contactid' />
                          <order attribute='fullname' descending='false' />
                          <filter type='and'>
                            <condition attribute='bcrm_gms1type' operator='eq' value='271400000' />
                            <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + patientId + @"' />
                          </filter>
                        </entity>
                      </fetch>";
            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(xml));
            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal object contentCheckFile(DocumentReferenceDTO DRDTO)
        {
            try
            {
                var sizeString = Regex.Match(DRDTO.size, @"^\d+").Value;
                var sizeNumber = int.Parse(sizeString);
                if (sizeNumber >= 6000000)
                {

                    return new Dictionary<string, object>
                    {
                        { "attachment", new Dictionary<string, object>
                            {
                                { "contentType", DRDTO.contentType },
                                { "title", "file size 6000000 bytes greater than maximum allowable (5MB)" },
                                { "size", Regex.Match(DRDTO.size.ToString(), @"^\d+").Value }
                            }
                        }
                    };
                
                }
                else
                {
                    return new Dictionary<string, object>
                    {
                        { "attachment", new Dictionary<string, object>
                            {
                                { "contentType", DRDTO.contentType },
                                { "url", "https://localhost:7090/W7M0I/STU3/1/gpconnect/Binary/" + DRDTO.masterIdentifierCRMGuid },
                                { "size", Regex.Match(DRDTO.size.ToString(), @"^\d+").Value }
                            }
                        }
                    };
                }
            
            }
             catch 
            {
               return new object();
            }
            
        }


        #endregion

    }
}
