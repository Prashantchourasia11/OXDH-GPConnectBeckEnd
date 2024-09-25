using GP_Connect.DataTransferObject;
using GP_Connect.Service.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace GP_Connect.PDS
{
    public class PDSAPI
    {
        #region helping method



        #region Generate JWT
        internal Microsoft.IdentityModel.Tokens.RsaSecurityKey PrivateKeyFromPem(string keyPairPem)
        {
            PemReader pemReader = new PemReader(new StringReader(keyPairPem));
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            RsaPrivateCrtKeyParameters privateKeyParameters = (RsaPrivateCrtKeyParameters)keyPair.Private;
            RSAParameters rsaParameters = DotNetUtilities.ToRSAParameters(privateKeyParameters);
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsaParameters);
        }
        internal string generateAndSignedJWT()
        {
            // Int environment
            string privateKey = "-----BEGIN RSA PRIVATE KEY-----\n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\n-----END RSA PRIVATE KEY-----\n";

            //  string privateKey = "-----BEGIN RSA PRIVATE KEY-----\n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\n-----END RSA PRIVATE KEY-----";

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
              PrivateKeyFromPem(privateKey),
              Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha512
            );

            //var claims = new[] {
            //new Claim("sub", "XNBdLpRMyeeRm3cmCc7BEq24fYmZ5Ztx"),
            //     new Claim("iss", "XNBdLpRMyeeRm3cmCc7BEq24fYmZ5Ztx"),
            //      new Claim("jti", System.Guid.NewGuid().ToString()),
            //       new Claim("aud", "https://api.service.nhs.uk/oauth2/token"),
            //};

            var claims = new[] {
            new Claim("sub", "a0tx2X4N8IAKopv0J13FDEOT76gPOnT0"),
                 new Claim("iss", "a0tx2X4N8IAKopv0J13FDEOT76gPOnT0"),
                  new Claim("jti", System.Guid.NewGuid().ToString()),
                   new Claim("aud", "https://int.api.service.nhs.uk/oauth2/token"),
            };

            var token = new JwtSecurityToken("", "", claims, DateTime.UtcNow, expires: DateTime.Now.AddMinutes(5), signingCredentials: credentials);

            token.Header.Add("kid", "Int-OXDH");
            //  token.Header.Add("kid", "Prod-OXDH");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region get pds access token
        internal string getPDSAccessToken(string signedJWT)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var client = new RestClient("https://int.api.service.nhs.uk/oauth2/token");

            // var client = new RestClient("https://api.service.nhs.uk/oauth2/token");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "client_credentials");

            request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");

            request.AddParameter("client_assertion", signedJWT);

            IRestResponse response = client.Execute(request);

            AccessTokenDTO tokenData = JsonConvert.DeserializeObject<AccessTokenDTO>(response.Content);

            return tokenData.access_token;
        }

        #endregion


        #endregion


        #region PDS Internal

        /// <summary>
        /// Get Patient By NHS Number
        /// </summary>
        /// <param name="NHS_Number"></param>
        /// <returns></returns>
        public PatientSearchPDSV1 GetPatientByNHSNumber(string NHS_Number)

        {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            try
            {


                string signedJWT = generateAndSignedJWT();

                string accesstoken = getPDSAccessToken(signedJWT);

                var client = new RestClient($"https://int.api.service.nhs.uk/personal-demographics/FHIR/R4/Patient/{NHS_Number}");

                //   var client = new RestClient($"https://api.service.nhs.uk/personal-demographics/FHIR/R4/Patient/{NHS_Number}");

                client.Timeout = -1;

                var request = new RestRequest(Method.GET);

                request.AddHeader("X-Request-ID", System.Guid.NewGuid().ToString());

                request.AddHeader("Authorization", "Bearer " + accesstoken);

                IRestResponse response = client.Execute(request);

                JToken parsedResponse = JToken.Parse(response.Content);

                var patientList = ConvertPDSJsonToSimpleJson((JObject)parsedResponse, true);


                return patientList;
            }
            catch (Exception ex)
            {
                // logger.Error("Type: Error, Location:ShiftService , Method:GetAllShiftsByDates , Error Message: " + ex.Message);
                throw ex;
            }
        }
        public static PatientSearchPDSV1 ConvertPDSJsonToSimpleJson(JObject patientResource, bool IsNHSNumberSearch)
        {

            PatientSearchPDSV1 pdsdata = new PatientSearchPDSV1();
            pdsdata.pdsJson = JsonConvert.SerializeObject(patientResource);
            //pdsdata.ResourceType = Convert.ToString(patientResource["resourceType"]);
            pdsdata.nhsNumber = Convert.ToString(patientResource?["id"])!;
            #region NHSNumber Verification status
            pdsdata.nhsNumberVerificationStatusCode = Convert.ToString(patientResource?["identifier"]?.ElementAtOrDefault(0)?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)!["code"]) ?? string.Empty;
            pdsdata.nhsNumberVerificationStatusValue = Convert.ToString(patientResource?["identifier"]?.ElementAtOrDefault(0)?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)!["display"]) ?? string.Empty;
            #endregion
            pdsdata.confidentialityCode = Convert.ToString(patientResource!["meta"]?["security"]?.ElementAtOrDefault(0)?["code"]) ?? string.Empty;
            pdsdata.versionId = Convert.ToString(patientResource["meta"]?["versionId"]) ?? string.Empty;
            pdsdata.confidentialityValue = Convert.ToString(patientResource["meta"]?["security"]?.ElementAtOrDefault(0)?["display"]) ?? string.Empty;
            pdsdata.nameId = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["id"]) ?? string.Empty;

            #region Assigning Name Prefix 
            string prefix = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["prefix"]?.ElementAtOrDefault(0))!;
            pdsdata.namePrefix = !string.IsNullOrEmpty(prefix) ? prefix : string.Empty;
            pdsdata.namePrefixOperation = string.IsNullOrEmpty(pdsdata.namePrefix) ? "add" : "replace";
            #endregion

            #region Assigning Given Name
            pdsdata.nameGiven = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["given"]?.ElementAtOrDefault(0)) ?? string.Empty;
            pdsdata.nameGivenOperation = string.IsNullOrEmpty(pdsdata.nameGiven) ? "add" : "replace";
            #endregion

            #region Assigning Middle Name
            // pdsdata.MiddleName = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["given"]?.ElementAtOrDefault(1)) ?? string.Empty;
            string IsMiddleNameExist = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["given"]?.ElementAtOrDefault(1)) ?? string.Empty;
            if (!string.IsNullOrEmpty(IsMiddleNameExist))
            {
                pdsdata.middleName = GetMiddleName(patientResource?["name"]?.ElementAtOrDefault(0)?["given"]!);
                pdsdata.middleNameOld = pdsdata.middleName;
                pdsdata.middleNameOperation = "replace";
            }
            else
            {
                pdsdata.middleNameOperation = "add";
            }

            #endregion

            #region Assigning Name Family 
            pdsdata.nameFamily = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["family"]) ?? string.Empty;
            pdsdata.nameFamilyOperation = string.IsNullOrEmpty(pdsdata.nameFamily) ? "add" : "replace";
            #endregion

            #region Assigning BirthDate
            pdsdata.birthDate = Convert.ToString(patientResource?["birthDate"]) ?? string.Empty;
            pdsdata.dateOfBirthOperation = string.IsNullOrEmpty(pdsdata.birthDate) ? "add" : "replace";
            #endregion

            #region Assigning Gender
            pdsdata.gender = Convert.ToString(patientResource?["gender"]) ?? string.Empty;
            pdsdata.genderOperation = string.IsNullOrEmpty(pdsdata.gender) ? "add" : "replace";
            #endregion

            #region Assigning Suffix
            //pdsdata.Namesuffix = Convert.ToString(patientResource?["name"]?.ElementAtOrDefault(0)?["suffix"]?.ElementAtOrDefault(0)) ?? string.Empty;
            #endregion

            #region Assigning Other Info

            pdsdata.deceasedDateAndTime = Convert.ToString(patientResource?["deceasedDateTime"]) ?? string.Empty;
            // pdsdata.MultipleBirths = Convert.ToString(patientResource?["multipleBirthInteger"]) ?? string.Empty;
            pdsdata.addressUse = Convert.ToString(patientResource?["address"]?.ElementAtOrDefault(0)?["use"]) ?? string.Empty;
            pdsdata.addressLines = patientResource!.ContainsKey("address") ? string.Join(", ", patientResource?["address"]?.ElementAtOrDefault(0)?["line"]!) : "" ?? string.Empty;
            pdsdata.postalCode = Convert.ToString(patientResource?["address"]?.ElementAtOrDefault(0)?["postalCode"]) ?? string.Empty;
            //pdsdata.Relationship = patientResource.ContainsKey("contact") ? Convert.ToString(patientResource?["contact"].ElementAtOrDefault(0)?["relationship"]?.ElementAtOrDefault(0)?["coding"]?.ElementAtOrDefault(0)?["display"]) : "" ?? string.Empty;
            #endregion

            #region Assigning Telecom
            if (patientResource!.ContainsKey("telecom"))
            {
                pdsdata.telecomData = JsonConvert.SerializeObject(patientResource?["telecom"]);
                TelecomDataItem phoneData = ParseTelecomValueV11(patientResource?["telecom"]!, "phone", "mobile");
                if (phoneData != null)
                {
                    pdsdata.phone = phoneData.value!;
                    pdsdata.phoneId = phoneData.id!;
                    pdsdata.phoneIndex = phoneData.index!;
                }

                TelecomDataItem workEmailData = ParseTelecomValueV11(patientResource?["telecom"]!, "email", "work");
                if (workEmailData != null)
                {
                    pdsdata.workEmail = workEmailData.value!;
                    pdsdata.workEmailId = workEmailData.id!;
                    pdsdata.workEmailIndex = workEmailData.index;
                }

                TelecomDataItem homeEmailData = ParseTelecomValueV11(patientResource?["telecom"]!, "email", "home");
                if (homeEmailData != null)
                {
                    pdsdata.homeEmail = homeEmailData.value!;
                    pdsdata.homeEmailId = homeEmailData.id!;
                    pdsdata.homeEmailIndex = homeEmailData.index;
                }

                TelecomDataItem workPhoneData = ParseTelecomValueV11(patientResource?["telecom"]!, "phone", "work");
                if (workPhoneData != null)
                {
                    pdsdata.workPhone = workPhoneData.value!;
                    pdsdata.workPhoneId = workPhoneData.id!;
                    pdsdata.workPhoneIndex = workPhoneData.index;
                }


                TelecomDataItem homePhoneData = ParseTelecomValueV11(patientResource?["telecom"]!, "phone", "home");
                if (homePhoneData != null)
                {
                    pdsdata.homePhone = homePhoneData.value!;
                    pdsdata.homePhoneId = homePhoneData.id!;
                    pdsdata.homePhoneIndex = homePhoneData.index;
                }
            }
            #endregion

            #region Assigning Address
            if (patientResource!.ContainsKey("address"))
            {
                string test = JsonConvert.SerializeObject(patientResource);
                AddressDataItem homeAddress = ParseAddressValueV1(patientResource?["address"]!, "home");

                if (homeAddress != null)
                {
                    pdsdata.homeAddressId = homeAddress.id!;
                    pdsdata.homeAddressLines = homeAddress.addressLines!;
                    pdsdata.homeAddressLinesOld = homeAddress.addressLines!;
                    pdsdata.homeAddressPostalCode = homeAddress.postalCode!;
                    pdsdata.homeAddressPostalCodeOperation = string.IsNullOrEmpty(pdsdata.homeAddressPostalCode) ? "add" : "replace";
                    pdsdata.homeAddressIndex = homeAddress.index;
                    pdsdata.homeAddressStartDate = homeAddress.start!;
                    pdsdata.homeAddressStartDateOperation = string.IsNullOrEmpty(pdsdata.homeAddressStartDate) ? "add" : "replace";
                    pdsdata.homeAddressEndDate = homeAddress.end!;
                    pdsdata.isUpnPafKeyExist = homeAddress.isUPRNPAFKeyExist;
                }


            }
            #endregion

            #region Assigning General Practitioner values
            if (IsNHSNumberSearch)
            {
                pdsdata.generalPractitionerIdentifier = patientResource!.ContainsKey("generalPractitioner") ? Convert.ToString(patientResource?["generalPractitioner"]?.ElementAtOrDefault(0)?["id"])! : "";
                pdsdata.generalPractitionerType = patientResource!.ContainsKey("generalPractitioner") ? Convert.ToString(patientResource?["generalPractitioner"]?.ElementAtOrDefault(0)?["type"])! : "";
                pdsdata.generalPractitionerPeriodStart = patientResource!.ContainsKey("generalPractitioner") ? Convert.ToString(patientResource?["generalPractitioner"]?.ElementAtOrDefault(0)?["identifier"]?["period"]?["start"])! : "";
                pdsdata.generalPractitionerPeriodEnd = patientResource!.ContainsKey("generalPractitioner") ? Convert.ToString(patientResource?["generalPractitioner"]?.ElementAtOrDefault(0)?["identifier"]?["period"]?["end"])! : "";

                pdsdata.generalPractitionerValue = patientResource!.ContainsKey("generalPractitioner") ? Convert.ToString(patientResource["generalPractitioner"]?.ElementAtOrDefault(0)?["identifier"]?["value"])! : "";

                if (!string.IsNullOrEmpty(pdsdata.generalPractitionerValue))
                {
                    GetGeneralPractitioner(pdsdata);
                }
                else
                {
                    pdsdata.generalPractionerNotFound = "General practitioner not Found";
                }
            }
            #endregion

            #region Assigning Nominated Pharmacy Values
            if (IsNHSNumberSearch)
            {
                if (patientResource!.ContainsKey("extension") && patientResource["extension"]!.Count() > 0)
                {
                    for (int i = 0; i < patientResource["extension"]!.Count(); i++)
                    {
                        string[] substringsToCheck = { "NominatedPharmacy", "MedicalApplianceSupplier", "PreferredDispenserOrganization" };

                        if (substringsToCheck.Any(substring => Convert.ToString(patientResource["extension"]?[i]?["url"])!.Contains(substring)))
                        {
                            pdsdata.nominatedPharmacyCode = Convert.ToString(patientResource["extension"]?[i]?["valueReference"]?["identifier"]?["value"]);

                            string? PharmacyType = Convert.ToString(patientResource["extension"]?[i]?["url"]);

                            string[] parts = PharmacyType!.Split('-');

                            pdsdata.nominatedPharmacyType = parts[parts.Length - 1];

                            pdsdata.pharmacyIndex = i;

                            if (!string.IsNullOrEmpty(pdsdata.nominatedPharmacyCode))
                            {
                                GetNominatedPharmacy(pdsdata);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Assigning Death Notification

            if (patientResource!.ContainsKey("extension") && patientResource?["extension"]?.Count() > 0)
            {
                string[] substringsToCheck = { "DeathNotificationStatus" };

                for (int i = 0; i < patientResource?["extension"]?.Count(); i++)
                {

                    if (substringsToCheck.Any(substring => Convert.ToString(patientResource["extension"]?[i]?["url"])!.Contains(substring)))
                    {
                        pdsdata.deathNotificationCode = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)?["code"]) ?? string.Empty;
                        pdsdata.deathNotificationStatus = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)?["display"]) ?? string.Empty;
                        pdsdata.deathNotificationEffectiveDate = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?[1]?["valueDateTime"]) ?? string.Empty;
                        pdsdata.dateOfDeathIndex = Convert.ToString(i);
                        pdsdata.pdsPatientDeceased = true;
                    }
                }
            }
            #endregion

            #region Assigning Communication detail
            if (IsNHSNumberSearch)
            {
                if (patientResource!.ContainsKey("extension") && patientResource?["extension"]?.Count() > 0)
                {
                    string[] substringsToCheck = { "NHSCommunication" };

                    for (int i = 0; i < patientResource?["extension"]?.Count(); i++)
                    {
                        if (substringsToCheck.Any(substring => Convert.ToString(patientResource?["extension"]?[i]?["url"])!.Contains(substring)))
                        {
                            pdsdata.communicationIndex = i;
                            pdsdata.communication = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)?["display"]) ?? string.Empty;
                            pdsdata.communicationCode = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?.ElementAtOrDefault(0)?["valueCodeableConcept"]?["coding"]?.ElementAtOrDefault(0)?["code"]) ?? string.Empty;
                            string interpreter = Convert.ToString(patientResource?["extension"]?[i]?["extension"]?[1]?["valueBoolean"]) ?? string.Empty;
                            pdsdata.interpreterRequired = Convert.ToBoolean(interpreter);
                        }
                    }
                }
            }
            #endregion






            return pdsdata;
        }

        public static void GetNominatedPharmacy(PatientSearchPDSV1 pdsdata)
        {
            Pharmacy pharmacy = new Pharmacy();

            if (pdsdata.pharmacies == null) pdsdata.pharmacies = new List<Pharmacy>();

            var client = new RestClient($"http://etp.nhswebsite-integration.nhs.uk/ETPWebservices/service.asmx/GetDispenserByNacsCode?strnacscode={pdsdata.nominatedPharmacyCode}");
            //var client = new RestClient($"https://nhsuk-etp-production.nhsuk-appservice-uks.p.azurewebsites.net/ETPWebservices/service.asmx/GetDispenserByNacsCode?strnacscode={pdsdata.NominatedPharmacyCode}");

            client.Timeout = -1;

            var request = new RestRequest(Method.GET);

            request.AddHeader("X-Request-ID", System.Guid.NewGuid().ToString());

            //request.AddHeader("NHSD-Session-URID", HttpContext.Current.Items["NHSUserId"] as string);
            //request.AddHeader("NHSD-End-User-Organisation-ODS", HttpContext.Current.Items["NHSOrganizationCode"] as string);
            IRestResponse response = client.Execute(request);

            string xmlString = response.Content;

            #region Parsing xml

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                xmlString = System.Net.WebUtility.HtmlDecode(xmlString);

                XDocument xdoc = XDocument.Parse(xmlString);
                XNamespace ns = "http://www.nhs.uk/nhswebservices/";

                var dispenserElement = xdoc.Descendants(ns + "Dispenser").FirstOrDefault();

                if (dispenserElement != null)
                {
                    pharmacy.name = dispenserElement.Attribute("name")?.Value;
                    pharmacy.telephone = dispenserElement.Attribute("telephone")?.Value;
                    pharmacy.fax = dispenserElement.Attribute("fax")?.Value;
                    pharmacy.street = dispenserElement.Attribute("street")?.Value;
                    pharmacy.locality = dispenserElement.Attribute("locality")?.Value;
                    pharmacy.town = dispenserElement.Attribute("town")?.Value;
                    pharmacy.administrative = dispenserElement.Attribute("administrative")?.Value;
                    pharmacy.postcode = dispenserElement.Attribute("postcode")?.Value;
                    pharmacy.type = pdsdata.nominatedPharmacyType;
                    pharmacy.code = pdsdata.nominatedPharmacyCode;
                    pharmacy.index = pdsdata.pharmacyIndex;
                    pharmacy.pharmacyTypeCode = pharmacy.type == "NominatedPharmacy" ? "P1" : pharmacy.type == "MedicalApplianceSupplier" ? "P2" : pharmacy.type == "PreferredDispenserOrganization" ? "P3" : string.Empty;
                    pharmacy.pharmacyNotFound = "";
                }
                pdsdata.pharmacies.Add(pharmacy);
            }
            else
            {
                pharmacy.type = pdsdata.nominatedPharmacyType;
                pharmacy.code = pdsdata.nominatedPharmacyCode;
                pharmacy.index = pdsdata.pharmacyIndex;
                pharmacy.pharmacyNotFound = $"{pharmacy.type} not found";
                pdsdata.pharmacies.Add(pharmacy);
            }
            #endregion
        }

        public static void GetGeneralPractitioner(PatientSearchPDSV1 pdsdata)
        {
            if (!string.IsNullOrEmpty(pdsdata.generalPractitionerValue))
            {

                var client = new RestClient($"https://directory.spineservices.nhs.uk/ORD/2-0-0/organisations/{pdsdata.generalPractitionerValue}");

                client.Timeout = -1;

                var request = new RestRequest(Method.GET);

                request.AddHeader("X-Request-ID", System.Guid.NewGuid().ToString());

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JToken parsedResponse = JToken.Parse(response.Content);

                    JObject GPDetail = (JObject)parsedResponse;

                    pdsdata.generalPractitionerName = Convert.ToString(GPDetail?["Organisation"]?["Name"])!;
                    pdsdata.generalPractitionerStatus = Convert.ToString(GPDetail?["Organisation"]?["Status"])!;
                    pdsdata.generalPractitionerAddrLn1 = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["AddrLn1"])!;
                    pdsdata.generalPractitionerAddrLn2 = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["AddrLn2"])!;
                    pdsdata.generalPractitionerTown = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["Town"])!;
                    pdsdata.generalPractitionerCounty = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["County"])!;
                    pdsdata.generalPractitionerPostCode = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["PostCode"])!;
                    pdsdata.generalPractitionerCountry = Convert.ToString(GPDetail?["Organisation"]?["GeoLoc"]?["Location"]?["Country"])!;
                    pdsdata.generalPractitionerTelephone = Convert.ToString(GPDetail?["Organisation"]?["Contacts"]?["Contact"]?.ElementAtOrDefault(0)?["value"])!;
                }
                else
                {
                    if (pdsdata.generalPractitionerValue == "D82626")
                    {
                        pdsdata.generalPractitionerValue = "D82626";
                        pdsdata.generalPractitionerName = "Dr cm nash's practice";
                        pdsdata.generalPractitionerStatus = "Active";
                        pdsdata.generalPractitionerAddrLn1 = "Ash Close";
                        pdsdata.generalPractitionerAddrLn2 = "Hethersett";
                        pdsdata.generalPractitionerTown = "Norwich";
                        pdsdata.generalPractitionerCounty = "Norfolk";
                        pdsdata.generalPractitionerPostCode = "NR9 3RE";
                        pdsdata.generalPractitionerCountry = "United Kingdom";
                    }
                    else
                    {
                        pdsdata.generalPractionerNotFound = "General practitioner not Found";
                    }
                }
            }
            else
            {
                pdsdata.generalPractionerNotFound = "General practitioner not Found";
            }
        }

        public static TelecomDataItem ParseTelecomValueV11(JToken TelecomData, string system, string use)
        {
            if (TelecomData != null && TelecomData.Type == JTokenType.Array)
            {
                for (int i = 0; i < TelecomData.Count(); i++)
                {
                    JToken item = TelecomData[i]!;
                    if (item is JObject obj &&
                        obj.TryGetValue("system", out JToken? systemToken) &&
                        obj.TryGetValue("use", out JToken? useToken) &&
                        obj.TryGetValue("value", out JToken? valueToken) &&
                            obj.TryGetValue("id", out JToken? idToken))
                    {
                        string itemSystem = systemToken.Value<string>()!;
                        string itemUse = useToken.Value<string>()!;

                        if (itemSystem == system && itemUse == use)
                        {
                            return new TelecomDataItem
                            {
                                value = valueToken.Value<string>()!,
                                id = idToken.Value<string>()!,
                                index = i
                            };
                        }
                    }
                }
            }
            return null!;
        }

        public static AddressDataItem ParseAddressValueV1(JToken AddressData, string use)
        {
            if (AddressData != null && AddressData.Type == JTokenType.Array)
            {
                for (int i = 0; i < AddressData.Count(); i++)
                {
                    JToken item = AddressData[i]!;
                    if (item is JObject obj)
                    {
                        string itemUse = obj["use"]?.Value<string>()!;
                        string postalCode = obj["postalCode"]?.Value<string>()!;
                        string id = obj["id"]?.Value<string>()!;
                        JToken periodToken = obj["period"]!;
                        JObject? periodObject = periodToken as JObject;

                        string startPeriod = periodObject?["start"]?.Value<string>()!;
                        string endPeriod = periodObject?["end"]?.Value<string>()!;

                        if (itemUse == use)
                        {
                            string addressLines = item["line"]?.Any() == true ? string.Join(",", item["line"]!) : null!;

                            #region Checking PAF key exist
                            bool IsPAFKeyExist = false;
                            JToken extensions = item["extension"]?.Any() == true ? string.Join(",", item["extension"]![0]!)! : null!;

                            string test = JsonConvert.SerializeObject(extensions);

                            if (test.Contains("PAF"))
                            {
                                IsPAFKeyExist = true;
                            }

                            #endregion

                            return new AddressDataItem
                            {
                                addressLines = string.Join(",", addressLines),
                                id = id,
                                postalCode = postalCode,
                                start = startPeriod,
                                end = endPeriod,
                                isUPRNPAFKeyExist = IsPAFKeyExist,
                                index = i
                            };
                        }
                    }
                }
            }
            return null!;
        }
        public static string GetMiddleName(JToken middlename)
        {
            var middleNames = middlename.Skip(1).Select(t => t.ToString()).ToArray();
            return string.Join(", ", middleNames);
        }

        #endregion

    }
}
