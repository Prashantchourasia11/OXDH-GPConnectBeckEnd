using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.Service.Foundation;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System.Text;
namespace GP_Connect.Logger
{
    public class EventLogger : IEventLogger
    {
        #region Properties
        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();
        Guid LogId;
        #endregion

        #region Constructor
        public EventLogger()
        {
            _crmServiceClient = crmCon.crmconnectionOXVC();
        }
        #endregion


        #region  Converting  Token into JSON 
        public string ConvertBearerTokenToJson(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                throw new ArgumentException("Authorization header cannot be null or empty.", nameof(authorizationHeader));
            }

            // Remove the Bearer prefix
            string base64UrlString = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader.Substring("Bearer ".Length)
                : authorizationHeader;

            try
            {
                // Split the JWT into header, payload, and signature
                string[] parts = base64UrlString.Split('.');
                if (parts.Length != 3)
                {
                    throw new ArgumentException("JWT token is not in the correct format.");
                }

                // Decode the payload part
                string payloadBase64Url = parts[1];
                string payloadBase64 = Base64UrlToBase64(payloadBase64Url);

                // Decode Base64 string to byte array
                byte[] decodedBytes = Convert.FromBase64String(payloadBase64);
                string decodedString = Encoding.UTF8.GetString(decodedBytes);
                try
                {
                    // Deserialize the JSON string
                    var jsonObject = JsonConvert.DeserializeObject(decodedString);
                        
                    // Serialize the object back to JSON with indentation
                    return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                }
                catch (JsonException jsonEx)
                {
                    // Log JSON deserialization errors
                    Console.WriteLine("Error deserializing JSON payload:");
                    Console.WriteLine(jsonEx.Message);
                    throw new ArgumentException("Error deserializing JSON payload.", jsonEx);
                }
            }
            catch (FormatException ex)
            {
                // Log Base64 decoding errors
                Console.WriteLine("The provided Base64Url string is invalid:");
                Console.WriteLine(ex.Message);
                throw new ArgumentException("The provided Base64Url string is invalid.", ex);
            }
        }

        // Convert Base64Url to Base64
        private string Base64UrlToBase64(string base64Url)
        {
            // Replace URL-safe characters with Base64 characters
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            // Add padding if necessary
            int padding = 4 - (base64.Length % 4);
            if (padding < 4)
            {
                base64 = base64.PadRight(base64.Length + padding, '=');
            }

            return base64;
        }
        #endregion

      

        #region Audit Event Type Ex. For Access HTML
        public Guid AuditEventType(RequestAccessHTMLDTO request, dynamic response, string sspTraceId, string authoizations ,String sspFrom , String sspTo , String sspInteractionId,string responseCode)
        {
         try {
                Guid logId = Guid.Empty;

                if (!string.IsNullOrEmpty(request.parameter[0].valueIdentifier.value))
                {
                    logId = CreateAudit(request.parameter[1].valueCodeableConcept.coding[0].code.ToString(), new JavaScriptSerializer().Serialize(request), JsonConvert.SerializeObject(response).ToString(), sspTraceId, authoizations, request.parameter[0].valueIdentifier.value.ToString(), sspFrom, sspTo, sspInteractionId, responseCode);
                }
                else
                {
                    logId = CreateAudit(request.parameter[1].valueCodeableConcept.coding[0].code.ToString(), new JavaScriptSerializer().Serialize(request), JsonConvert.SerializeObject(response).ToString(), sspTraceId, authoizations, "", sspFrom, sspTo, sspInteractionId , responseCode);

                }

                return logId;
            }
            catch (Exception ex)
            {
                Guid logId = Guid.Empty;
                logId = CreateAudit("", new JavaScriptSerializer().Serialize(request), JsonConvert.SerializeObject(response).ToString(), sspTraceId, authoizations, "", sspFrom, sspTo, sspInteractionId, responseCode);

                throw ex;
            }
        }
        #endregion

        #region CreateAudit
        public Guid CreateAudit(string keyword, string request, string response, string sspTraceId, string authorizations, string nhsNumber, string sspFrom, string sspTo, string sspInteractionId, string responseCode)
        {
            try
            {
                Entity Logs = new Entity("bcrm_logs");
                var contactGuid = GetPatientDetailsByNHSNumber(nhsNumber);
                if (contactGuid != Guid.Empty)
                { 
                    Logs.Attributes["bcrm_patient"] = new EntityReference("contact",contactGuid);
                }
                Logs.Attributes["bcrm_keyword"] = keyword;
                Logs.Attributes["bcrm_name"] = "Access HTML Record";
                Logs.Attributes["bcrm_requestheader"] = request;
                Logs.Attributes["bcrm_description"] = response;
                Logs.Attributes["createdon"] = DateTime.Now;
                Logs.Attributes["bcrm_activitytime"] = DateTime.UtcNow;
                Logs.Attributes["bcrm_url"] = "https://test-gpc-w7m0i.oxdh.thirdparty.nhs.uk/W7M0I/STU3/1/gpconnect/Patient/$gpc.getcarerecord";
                Logs.Attributes["bcrm_authorizationjson"] = ConvertBearerTokenToJson(authorizations).ToString();
                // Root rootObject = JsonConvert.DeserializeObject<Root>(ConvertBearerTokenToJson(authorizations))!;

                // Extract the value
                //string odsCode = rootObject.RequestingPractitioner?.Identifier.FirstOrDefault(id => id.System == "https://fhir.nhs.uk/Id/sds-user-id")?.Value!;
                //Logs.Attributes["bcrm_odscode"] = odsCode;
                Console.WriteLine(ConvertBearerTokenToJson(authorizations));
                Logs.Attributes["bcrm_ssptraceid"] = sspTraceId;
                Logs.Attributes["bcrm_apiid"] = Guid.NewGuid().ToString();
                Logs.Attributes["bcrm_nhsnumber"] = nhsNumber;
                Logs.Attributes["bcrm_sspfrom"] = sspFrom;
                Logs.Attributes["bcrm_sspto"] = sspTo;
                Logs.Attributes["bcrm_sspinteractionid"] = sspInteractionId;
                Logs.Attributes["bcrm_statuscode"] = responseCode;
                EntityCollection newLog = new EntityCollection();
                newLog.Entities.Add(Logs);
                LogId = _crmServiceClient.Create(Logs);
                return LogId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        #endregion

        #region Update Status Code
        public void UpdateStatusCode(string StatusCode, string LogId)
        {
            try
            {
                Entity updateLogs = new Entity("bcrm_logs", new Guid(LogId));
                updateLogs.Attributes["bcrm_statuscode"] = StatusCode;
                _crmServiceClient.Update(updateLogs);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Pateint Details By NHS number
        public Guid GetPatientDetailsByNHSNumber(string NHSNumber)
        {
            QueryExpression query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("contactid"), // Fetch only the ID
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                {
                    new ConditionExpression("bcrm_nhsnumber", ConditionOperator.Equal, NHSNumber)
                }
                }
            };
            EntityCollection result = _crmServiceClient.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {   
                Entity patient = result.Entities[0];
                Guid patientId = patient.Id;
                return patientId;
            }
            return Guid.Empty;
        }
        #endregion

        public class AccessStructuredRecordRequestAttributes
        {
            public string NhsNumber { get; set; }
            public string LogMessage { get; set; }
        }
        public class Root
        {
            [JsonProperty("iss")]
            public string Iss { get; set; }

            [JsonProperty("sub")]
            public string Sub { get; set; }

            [JsonProperty("aud")]
            public string Aud { get; set; }

            [JsonProperty("exp")]
            public long Exp { get; set; }

            [JsonProperty("iat")]
            public long Iat { get; set; }

            [JsonProperty("reason_for_request")]
            public string ReasonForRequest { get; set; }

            [JsonProperty("requesting_device")]
            public Device RequestingDevice { get; set; }

            [JsonProperty("requesting_organization")]
            public Organization RequestingOrganization { get; set; }

            [JsonProperty("requesting_practitioner")]
            public Practitioner RequestingPractitioner { get; set; }

            [JsonProperty("requested_scope")]
            public string RequestedScope { get; set; }
        }

        public class Device
        {
            [JsonProperty("resourceType")]
            public string ResourceType { get; set; }

            [JsonProperty("identifier")]
            public List<Identifier> Identifier { get; set; }

            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }
        }

        public class Organization
        {
            [JsonProperty("resourceType")]
            public string ResourceType { get; set; }

            [JsonProperty("identifier")]
            public List<Identifier> Identifier { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Practitioner
        {
            [JsonProperty("resourceType")]
            public string ResourceType { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("identifier")]
            public List<Identifier> Identifier { get; set; }

            [JsonProperty("name")]
            public List<Name> Name { get; set; }
        }

        public class Identifier
        {
            [JsonProperty("system")]
            public string System { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class Name
        {
            [JsonProperty("family")]
            public string Family { get; set; }

            [JsonProperty("given")]
            public List<string> Given { get; set; }

            [JsonProperty("prefix")]
            public List<string> Prefix { get; set; }
        }
    }
}
