using GP_Connect.DataTransferObject;
using System.Text.RegularExpressions;

namespace GP_Connect.FHIR_JSON
{
    public class PractionerDetails
    {
        public string FindAPractionerJSON(PractionerDTO practionerDetails)
        {
           
            try
            {


                var modeOfCommunicationCode = "";
                var modeOfCommunicationDisplay = "";
                var communicationProficiencyCode = "";
                var communicationProficiencyDisplay = "";
                var interpreterRequired = false;
                if(practionerDetails.interpreterRequired.ToString().ToLower() == "yes")
                {
                    interpreterRequired = true;
                }

                if (practionerDetails.modeOfCommunication != "")
                {
                    string pattern = @"\((.*?)\)";
                    Match match = Regex.Match(practionerDetails.modeOfCommunication, pattern);
                    if (match.Success)
                    {
                        modeOfCommunicationCode = match.Groups[1].Value;
                    }
                    modeOfCommunicationDisplay = Regex.Replace(practionerDetails.modeOfCommunication, pattern, "");
                    modeOfCommunicationDisplay = Regex.Replace(modeOfCommunicationDisplay, "-", "").Trim();
                }

                if (practionerDetails.communucationProficiency != "")
                {
                    string pattern = @"\((.*?)\)";
                    Match match = Regex.Match(practionerDetails.communucationProficiency, pattern);
                    if (match.Success)
                    {
                        communicationProficiencyCode = match.Groups[1].Value;
                    }
                    communicationProficiencyDisplay = Regex.Replace(practionerDetails.communucationProficiency, pattern, "");
                    communicationProficiencyDisplay = Regex.Replace(communicationProficiencyDisplay, "-", "").Trim();
                }


                string finalLanguageJSON = "";

                for(int i=0;i<practionerDetails.practitionerLanguages.Count; i++)
                {
                    string languageTemplate = @"{
                                        ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1"",
                                        ""extension"": [
                                          {
                                            ""url"": ""language"",
                                            ""valueCodeableConcept"": {
                                              ""coding"": [
                                                {
                                                  ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1"",
                                                  ""code"": """ + practionerDetails.practitionerLanguages[i].languageCode +@""",
                                                  ""display"": """ + practionerDetails.practitionerLanguages[i].languageName +@"""
                                                }
                                              ]
                                            }
                                          }
                                        ]
                                      }";
                    finalLanguageJSON += languageTemplate;
                    finalLanguageJSON += ",";
                }

              finalLanguageJSON += @"
                                    {
                                      ""url"": ""preferred"",
                                      ""valueBoolean"": false
                                    },
                                    {
                                      ""url"": ""modeOfCommunication"",
                                      ""valueCodeableConcept"": {
                                        ""coding"": [
                                          {
                                            ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityMode-1"",
                                            ""code"": """+modeOfCommunicationCode+@""",
                                            ""display"": """+modeOfCommunicationDisplay+@"""
                                          }
                                        ]
                                      }
                                    },
                                    {
                                      ""url"": ""communicationProficiency"",
                                      ""valueCodeableConcept"": {
                                        ""coding"": [
                                          {
                                            ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityProficiency-1"",
                                            ""code"": """+communicationProficiencyCode+@""",
                                            ""display"": """+communicationProficiencyDisplay+@"""
                                          }
                                        ]
                                      }
                                    },
                                    {
                                      ""url"": ""interpreterRequired"",
                                      ""valueBoolean"": "+ interpreterRequired.ToString().ToLower() + @"
                                    }";




                var practionerJSON = @"{
                               ""resourceType"": ""Bundle"",
                               ""id"": """+practionerDetails.resourceId+@""",
                               ""meta"": {
                                 ""lastUpdated"": """+ DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz") + @"""
                               },
                               ""type"": ""searchset"",
                               ""entry"": [
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Practitioner"",
                                     ""id"": """+practionerDetails.sequenceNumber+@""",
                                     ""meta"": {
                                       ""versionId"": """+practionerDetails.versionId+@""",
                                       ""lastUpdated"": """+ practionerDetails.modifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzz") +@""",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1""
                                       ]
                                     },
                                     ""extension"": [
                                      "+finalLanguageJSON+@"
                                     ],
                                     ""identifier"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/Id/sds-user-id"",
                                         ""value"": """+practionerDetails.sdsId+ @"""
                                       }
                                     ],
                                     ""active"": "+practionerDetails.currentStatus.ToString().ToLower()+@",
                                     ""name"": [
                                       {
                                         ""use"": ""usual"",
                                         ""family"": """ + practionerDetails.family+@""",
                                         ""given"": [
                                           """+practionerDetails.given+@"""
                                         ],
                                         ""prefix"": [
                                           ""Mr""
                                         ]
                                       }
                                     ],
                                     ""gender"": """+practionerDetails.gender+@"""
                                   }
                                 }
                               ]
                              }";


                return practionerJSON;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

       
        public dynamic FindAPractitionerUsingJSONFHIR(PractionerDTO practionerDetails)
        {
            var prefix = "Mr";
           if(practionerDetails.prefix != "")
            {
                prefix = practionerDetails.prefix;
            }

            var jsonContent = new
            {
                resourceType = "Bundle",
                id = practionerDetails.resourceId,
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
                resourceType = "Practitioner",
                id = practionerDetails.sequenceNumber,
                meta = new
                {
                    versionId = practionerDetails.versionId,
                    lastUpdated = practionerDetails.modifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[]
                    {
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1"
                    }
                },
                extension = new object[]
                {
                    new
                    {
                        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                        extension = MakeDynamicallyExtensionJSON(practionerDetails)
                    }
                },
                identifier = MakeDynamicallyJSONofSDSidAndProfileId(practionerDetails.sdsId,practionerDetails.JobRoles),
                active = practionerDetails.currentStatus,
                name = new[]
                {
                    new
                    {
                        use = "usual",
                        family = practionerDetails.family,
                        given = new[]
                        {
                            practionerDetails.given
                        },
                        prefix = new[]
                        {
                            prefix
                        }
                    }
                },
                gender = practionerDetails.gender
                }
               }
             }
            };

            return jsonContent;
        }

        public dynamic ReadAPractitionerUsingJSONFHIR(PractionerDTO practionerDetails)
        {
            var prefix = "Mr";
            if (practionerDetails.prefix != "")
            {
                prefix = practionerDetails.prefix;
            }



            var jsonContent = new
            {
                resourceType = "Practitioner",
                id = practionerDetails.sequenceNumber,
                meta = new
                {
                    versionId = practionerDetails.versionId,
                    lastUpdated = practionerDetails.modifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1" }
                },
                extension = new[]
        {
                new
                {
                    url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                    extension = MakeDynamicallyExtensionJSON(practionerDetails)
                }  
             
            },
                identifier = MakeDynamicallyJSONofSDSidAndProfileId(practionerDetails.sdsId, practionerDetails.JobRoles),
                name = new[]
        {
                new
                {
                    use = "usual",
                    family = practionerDetails.family,
                    given = new[] { practionerDetails.gender },
                    prefix = new[] { prefix }
                }
            },
                gender = practionerDetails.gender
            };
       
          

            return jsonContent;
        }

        public string ReadAPractionerJSON(PractionerDTO practionerDetails)
        {

            try
            {
                var practionerJSON = @"{
                                     ""resourceType"": ""Practitioner"",
                                     ""id"": """ + practionerDetails.sequenceNumber + @""",
                                     ""meta"": {
                                       ""versionId"": """ + practionerDetails.versionId + @""",
                                       ""lastUpdated"": """ + practionerDetails.modifiedDate.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1""
                                       ]
                                     },
                                     ""extension"": [
                                       {
                                         ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1"",
                                         ""extension"": [
                                           {
                                             ""url"": ""language"",
                                             ""valueCodeableConcept"": {
                                               ""coding"": [
                                                 {
                                                   ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1"",
                                                   ""code"": """ + practionerDetails.languageCode + @""",
                                                   ""display"": """ + practionerDetails.languageDisplay + @"""
                                                 }
                                               ]
                                             }
                                           }
                                         ]
                                       }
                                       ],
                                     ""identifier"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/Id/sds-user-id"",
                                         ""value"": """ + practionerDetails.sdsId + @"""
                                       }
                                     ],
                                    
                                     ""name"": [
                                       {
                                         ""use"": ""usual"",
                                         ""family"": """ + practionerDetails.family + @""",
                                         ""given"": [
                                           """ + practionerDetails.given + @"""
                                         ],
                                         ""prefix"": [
                                           ""Mr""
                                         ]
                                       }
                                     ],
                                     ""gender"": """ + practionerDetails.gender + @""" 
                              }";


                return practionerJSON;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal dynamic MakeDynamicallyExtensionJSON(PractionerDTO practionerDetails)
        {
            var interpretered = false;
            if (practionerDetails.interpreterRequired == "true")
            {
                interpretered = true;
            }

            var modeOfCommunicationCode = "";
            var modeOfCommunicationDisplay = "";
            var communicationProficiencyCode = "";
            var communicationProficiencyDisplay = "";

            if (practionerDetails.modeOfCommunication != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(practionerDetails.modeOfCommunication, pattern);
                if (match.Success)
                {
                    modeOfCommunicationCode = match.Groups[1].Value;
                }
                modeOfCommunicationDisplay = Regex.Replace(practionerDetails.modeOfCommunication, pattern, "");
                modeOfCommunicationDisplay = Regex.Replace(modeOfCommunicationDisplay, "-", "").Trim();
            }
            if (practionerDetails.communucationProficiency != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(practionerDetails.communucationProficiency, pattern);
                if (match.Success)
                {
                    communicationProficiencyCode = match.Groups[1].Value;
                }
                communicationProficiencyDisplay = Regex.Replace(practionerDetails.communucationProficiency, pattern, "");
                communicationProficiencyDisplay = Regex.Replace(communicationProficiencyDisplay, "-", "").Trim();
            }

            var languages = practionerDetails.practitionerLanguages;

            var extensions = new List<object>();

            foreach (var language in languages)
            {
                extensions.Add(new
                {
                    url = "language",
                    valueCodeableConcept = new
                    {
                        coding = new object[]
                        {
                        new
                        {
                            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
                            code = language.languageCode,
                            display = language.languageName
                        }
                        }
                    }
                });
            }

            extensions.Add(new
            {
                url = "preferred",
                valueBoolean = false
            });

            extensions.Add(new
            {
                url = "modeOfCommunication",
                valueCodeableConcept = new
                {
                    coding = new object[]
                    {
                    new
                    {
                        system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityMode-1",
                        code = modeOfCommunicationCode,
                        display = modeOfCommunicationDisplay
                    }
                    }
                }
            });

            extensions.Add(new
            {
                url = "communicationProficiency",
                valueCodeableConcept = new
                {
                    coding = new object[]
                    {
                    new
                    {
                        system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityProficiency-1",
                        code = communicationProficiencyCode,
                        display = communicationProficiencyDisplay
                    }
                    }
                }
            });

            extensions.Add(new
            {
                url = "interpreterRequired",
                valueBoolean = interpretered
            });
            return extensions;
        }

        public dynamic MakeEmptyBuddlePractitionerJSON()
        {
            var jsonContent = new
            {
                resourceType = "Bundle",
                type = "searchset",
                entry = new object[] { }
            };
            return jsonContent;
        }

        public dynamic NoPractitionerFound(string id)
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
                                code = "PRACTITIONER_NOT_FOUND",
                                display = "PRACTITIONER_NOT_FOUND"
                            }
                        }
                    },
                    diagnostics = "No practitioner details found for practitioner ID: " +id
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic MissingIdentifier(string value)
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
                                code = "INVALID_PARAMETER",
                                display = "INVALID_PARAMETER"
                            }
                        }
                    },
                    diagnostics = "One or both of the identifier system and value are missing from given identifier : " + value
                }
            }
            };
            return operationOutcome;
        }

        public dynamic InvalidSSPInteractionId()
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
                    diagnostics = "Interaction id must be : urn:nhs:names:services:gpconnect:fhir:rest:search:practitioner-1"
                }
            }
            };
            return operationOutcome;
        }

        public dynamic MultipleIndentifierExist(int number)
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
                    diagnostics = "Invalid quantity of identifier parameter found: "+number
                }
            }
            };
            return operationOutcome;
        }

        public dynamic IndentifierSpellingMistake()
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
                    diagnostics = "No identifier parameter found!"
                }
            }
            };
            return operationOutcome;
        }

        public dynamic MakeDynamicallyJSONofSDSidAndProfileId(string sdsId, List<string> jobRoles)
        {
            var identifiers = new List<object>
    {
        new
        {
            system = "https://fhir.nhs.uk/Id/sds-user-id",
            value = sdsId
        }
    };

            foreach (var jobRole in jobRoles)
            {
                identifiers.Add(new
                {
                    system = "https://fhir.nhs.uk/Id/sds-role-profile-id",
                    value = jobRole
                });
            }

            return identifiers.ToArray();
        }

       
    }
}
