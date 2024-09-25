using GP_Connect.DataTransferObject;

namespace GP_Connect.FHIR_JSON
{
    public class OrganisationDetails
    {
        public string FindAOrganizationJSON(OrganizationDTO organizationDetails)
        {
            var organizationAddresssJson = "";
            if(organizationDetails.addressLine != "")
            {
                organizationAddresssJson = @",
                                             ""address"": [
                                               {
                                                 ""use"": ""work"",
                                                 ""line"": [
                                                   """ + organizationDetails.addressLine + @"""
                                                 ],
                                                 ""city"": """ + organizationDetails.city + @""",
                                                 ""district"": """ + organizationDetails.district + @""",
                                                 ""postalCode"": """ + organizationDetails.postalCode + @"""
                                               }
                                             ]";
            }

            try
            {
                var organizationJSON = @"{
                                       ""resourceType"": ""Bundle"",
                                       ""id"": """+organizationDetails.resourceId+@""",
                                       ""meta"": {
                                         ""lastUpdated"": """+ DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz") + @"""
                                       },
                                       ""type"": ""searchset"",
                                       ""entry"": [
                                         {
                                           ""resource"": {
                                             ""resourceType"": ""Organization"",
                                             ""id"": """+organizationDetails.sequenceNumber+@""",
                                             ""meta"": {
                                               ""versionId"": """+organizationDetails.versionId+@""",
                                               ""lastUpdated"": """+ organizationDetails.lastUpdated.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                                               ""profile"": [
                                                 ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                               ]
                                             },
                                             ""identifier"": [
                                               {
                                                 ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                                 ""value"": """+organizationDetails.odsCode+ @"""
                                               }
                                             ],
                                             ""active"": "+organizationDetails.currentStatus.ToString().ToLower()+@",
                                             ""name"": """ + organizationDetails.organizationName+@""",
                                             ""telecom"": [
                                               {
                                                 ""system"": ""phone"",
                                                 ""value"": """+organizationDetails.phoneNumber+@""",
                                                 ""use"": ""work""
                                               }
                                             ]
                                             "+ organizationAddresssJson + @"
                                           }
                                         }
                                       ]
                                      }";


                return organizationJSON;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ReadAOrganizationJSON(OrganizationDTO organizationDetails)
        {

            try
            {
                var organizationJSON = @"{
                                             ""resourceType"": ""Organization"",
                                             ""id"": """ + organizationDetails.sequenceNumber + @""",
                                             ""meta"": {
                                               ""versionId"": """ + organizationDetails.versionId + @""",
                                               ""lastUpdated"": """ + organizationDetails.lastUpdated.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                                               ""profile"": [
                                                 ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                               ]
                                             },
                                             ""identifier"": [
                                               {
                                                 ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                                 ""value"": """ + organizationDetails.odsCode + @"""
                                               }
                                             ],
                                             ""name"": ""GP Connect Demonstrator"",
                                             ""telecom"": [
                                               {
                                                 ""system"": ""phone"",
                                                 ""value"": """ + organizationDetails.phoneNumber + @""",
                                                 ""use"": ""work""
                                               }
                                             ],
                                             ""address"": [
                                               {
                                                 ""use"": ""work"",
                                                 ""line"": [
                                                   """ + organizationDetails.addressLine + @"""
                                                 ],
                                                 ""city"": """ + organizationDetails.city + @""",
                                                 ""district"": """ + organizationDetails.district + @""",
                                                 ""postalCode"": """ + organizationDetails.postalCode + @"""
                                               }
                                             ]
                                           }";


                return organizationJSON;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public dynamic FindOrganizationFHIRJSON(OrganizationDTO organizationDetails)
        {
            var status = true;
            if(organizationDetails.currentStatus.ToString().ToLower() == "false")
            {
                status = false;
            }

            var jsonContent = new
            {
                resourceType = "Bundle",
                id = organizationDetails.resourceId,
                meta = new
                {
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                },
                type = "searchset",
                entry = new[]
              {
                    new
                    {
                        resource = new
                        {
                            resourceType = "Organization",
                            id = organizationDetails.sequenceNumber,
                            meta = new
                            {
                                versionId = organizationDetails.versionId,
                                lastUpdated = organizationDetails.lastUpdated.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                profile = new[]
                                {
                                    "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1"
                                }
                            },
                            identifier = new[]
                            {
                                new
                                {
                                    system = "https://fhir.nhs.uk/Id/ods-organization-code",
                                    value = organizationDetails.odsCode
                                }
                            },
                            active = status,
                            name = organizationDetails.organizationName,
                            telecom = new[]
                            {
                                new
                                {
                                    system = "phone",
                                    value = organizationDetails.phoneNumber,
                                    use = "work"
                                }
                            },
                            address = new[]
                            {
                                new
                                {
                                    use = "work",
                                    line = new[]
                                    {
                                       organizationDetails.addressLine
                                    },
                                    city = organizationDetails.city,
                                    district = organizationDetails.district,
                                    postalCode = organizationDetails.postalCode
                                }
                            }
                        }
                    }
                }
            };
            return jsonContent;
        }

        public dynamic ReadOrganizationFHIRJSON(OrganizationDTO organizationDetails)
        {
            var organizationJson = new
            {
                resourceType = "Organization",
                id = organizationDetails.sequenceNumber,
                meta = new
                {
                    versionId = organizationDetails.versionId,
                    lastUpdated = organizationDetails.lastUpdated.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[]
                                  {
                                      "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1"
                                  }
                                          },
                                          identifier = new[]
                              {
                                  new
                                  {
                                      system = "https://fhir.nhs.uk/Id/ods-organization-code",
                                      value = organizationDetails.odsCode
                                  }
                              },
                                          name = organizationDetails.organizationName,
                                          telecom = new[]
                              {
                                  new
                                  {
                                      system = "phone",
                                      value = organizationDetails.phoneNumber,
                                      use = "work"
                                  }
                              },
                                          address = new[]
                              {
                                  new
                                  {
                                      use = "work",
                                      line = new[]
                                      {
                                          organizationDetails.addressLine
                                      },
                                      city = organizationDetails.city,
                                      district = organizationDetails.district,
                                      postalCode = organizationDetails.postalCode
                                  }
                              }
                           };

            return organizationJson;

        }

        public dynamic OrganizationNotFoundJSON(string id)
        {
            var operationOutcomeJson = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1" }
                },
                issue = new[]
           {
                    new
                    {
                        severity = "error",
                        code = "invalid",
                        details = new
                        {
                            coding = new[]
                        {
                                new
                                {
                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                    code = "ORGANISATION_NOT_FOUND",
                                    display = "ORGANISATION_NOT_FOUND"
                                }
                            }
                        },
                        diagnostics = "No organization details found for organization ID: " + id
                    }
                }
            };
            return operationOutcomeJson;
        }

        public dynamic MakeEmptyBuddleOrganizationJSON()
        {
            var jsonContent = new
            {
                resourceType = "Bundle",
                type = "searchset",
                entry = new object[] { }
            };
            return jsonContent;
        }

        public dynamic WrongSSPinteractionId()
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
                {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
            {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "Interaction Id must be : urn:nhs:names:services:gpconnect:fhir:rest:search:organization-1"
                }
            }
            };
            return operationOutcome;
        }
    }
}
