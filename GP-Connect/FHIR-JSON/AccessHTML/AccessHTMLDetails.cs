using GP_Connect.DataTransferObject;
using System.Text.RegularExpressions;

namespace GP_Connect.FHIR_JSON.AccessHTML
{
    public class AccessHTMLDetails
    {
        public dynamic INVALIDIDENTIFIERSYSTEMJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "INVALID_IDENTIFIER_SYSTEM",
                                display = "INVALID_IDENTIFIER_SYSTEM"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic INVALIDNHSNUMBERJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "INVALID_NHS_NUMBER",
                                display = "INVALID_NHS_NUMBER"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic BADREQUESTJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic NOPATIENTCONSENTJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "NO_PATIENT_CONSENT",
                                display = "NO_PATIENT_CONSENT"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic PATIENTNOTFOUNDJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "PATIENT_NOT_FOUND",
                                display = "PATIENT_NOT_FOUND"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic INVALIDPARAMETERJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "INVALID_PARAMETER",
                                display = "INVALID_PARAMETER"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic INVALIDRESOURCEJSON(string text)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
           {
                    "http://fhir.nhs.net/StructureDefinition/gpconnect-operationoutcome-1"
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
                                system = "http://fhir.nhs.net/ValueSet/gpconnect-error-or-warning-code-1",
                                code = "INVALID_RESOURCE",
                                display = "INVALID_RESOURCE"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }
        public dynamic ReadAPatientUsingJSONFHIR(PatientDTO patientDetails)
        {
            if(!patientDetails.deceasedDate.ToString().Contains("0001"))
            {
                return DeceasedReadAPatientUsingJSONFHIR(patientDetails);
            }

            var modeOfCommunicationCode = "";
            var modeOfCommunicationDisplay = "";
            var communicationProficiencyCode = "";
            var communicationProficiencyDisplay = "";

            if (patientDetails.modeOfCommunication != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.modeOfCommunication, pattern);
                if (match.Success)
                {
                    modeOfCommunicationCode = match.Groups[1].Value;
                }
                modeOfCommunicationDisplay = Regex.Replace(patientDetails.modeOfCommunication, pattern, "");
                modeOfCommunicationDisplay = Regex.Replace(modeOfCommunicationDisplay, "-", "").Trim();
            }

            if (patientDetails.communicationProficiency != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.communicationProficiency, pattern);
                if (match.Success)
                {
                    communicationProficiencyCode = match.Groups[1].Value;
                }
                communicationProficiencyDisplay = Regex.Replace(patientDetails.communicationProficiency, pattern, "");
                communicationProficiencyDisplay = Regex.Replace(communicationProficiencyDisplay, "-", "").Trim();
            }

            var interpreterRequired = false;
            if (patientDetails.InterpreterRequired == "true")
            {
                interpreterRequired = true;

            }


            var patient = new
            {
                resourceType = "Patient",
                id = patientDetails.GPCSequenceNumber,
                meta = new
                {
                    versionId = patientDetails.PdsVersionId,
                    profile = new[] { "http://fhir.nhs.net/StructureDefinition/gpconnect-patient-1" }
                },
                extension = new object[]
    {
        new
        {
            url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
            extension = new Object[]
            {
                new
                {
                    url = "registrationPeriod",
                    valuePeriod = new
                    {
                        start = patientDetails.GPCRegistractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")
                    }
                },
                new
                {
                    url = "preferredBranchSurgery",
                    valueReference = new
                    {
                        reference = "Location/" + patientDetails.locationRefrenceNumber
                    }
                }
            }
        },
        new
        {
            url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
            extension = MakeDynamicallyExtensionJSON(patientDetails)
        }
    },
                identifier = new[]
    {
        new
        {
            extension = new[]
            {
                new
                {
                    url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                    valueCodeableConcept = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "http://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                code = patientDetails.NHS_number_Verification_Status_Code,
                                display = patientDetails.NHS_number_Verification_Status_Display
                            }
                        }
                    }
                }
            },
            system = "http://fhir.nhs.net/Id/nhs-number",
            value = patientDetails.bcrm_nhsnumber
        }
    },
                active = true,
                name = new object[]
                 {
                        CreatePatientName(patientDetails)
                 },
                telecom = makeTelecomJson(patientDetails),
                gender = patientDetails.gender.ToLower(),
                birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),
                address = new[]
    {
        new
        {
            use = "home",
            type = "physical",
            line = new[] { patientDetails.address1_line1 },
            city = patientDetails.address1_city,
            postalCode = patientDetails.address1_postalcode
        }
    },
                careProvider = new[]
    {
        new
        {
            reference = "Practitioner/" + patientDetails.stuffRefrenceNumber
        }
    }
            };


            return patient;
        }
        internal dynamic MakeDynamicallyExtensionJSON(PatientDTO patientDetails)
        {
            var interpretered = false;
            if (patientDetails.InterpreterRequired == "true")
            {
                interpretered = true;
            }

            var modeOfCommunicationCode = "";
            var modeOfCommunicationDisplay = "";
            var communicationProficiencyCode = "";
            var communicationProficiencyDisplay = "";

            if (patientDetails.modeOfCommunication != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.modeOfCommunication, pattern);
                if (match.Success)
                {
                    modeOfCommunicationCode = match.Groups[1].Value;
                }
                modeOfCommunicationDisplay = Regex.Replace(patientDetails.modeOfCommunication, pattern, "");
                modeOfCommunicationDisplay = Regex.Replace(modeOfCommunicationDisplay, "-", "").Trim();
            }
            if (patientDetails.communicationProficiency != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.communicationProficiency, pattern);
                if (match.Success)
                {
                    communicationProficiencyCode = match.Groups[1].Value;
                }
                communicationProficiencyDisplay = Regex.Replace(patientDetails.communicationProficiency, pattern, "");
                communicationProficiencyDisplay = Regex.Replace(communicationProficiencyDisplay, "-", "").Trim();
            }

            var languages = patientDetails.patientLanguages;

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
                            system = "http://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
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
                        system = "http://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityMode-1",
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
                        system = "http://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityProficiency-1",
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
        internal dynamic makeTelecomJson(PatientDTO patientDetails)
        {
            var telecoms = new List<object>();
            if (patientDetails.workEmail != "")
            {
                telecoms.Add(new
                {
                    system = "email",
                    value = patientDetails.workEmail,
                    use = "work"
                });
            }
            if (patientDetails.workPhone != "")
            {
                telecoms.Add(new
                {
                    system = "phone",
                    value = patientDetails.workPhone,
                    use = "work"
                });
            }
            if (patientDetails.homePhone != "")
            {
                telecoms.Add(new
                {
                    system = "phone",
                    value = patientDetails.homePhone,
                    use = "home"
                });
            }
            if (patientDetails.homeEmail != "")
            {
                telecoms.Add(new
                {
                    system = "email",
                    value = patientDetails.homeEmail,
                    use = "home"
                });
            }
            if (patientDetails.mobilephone != "")
            {
                telecoms.Add(new
                {
                    system = "phone",
                    value = patientDetails.mobilephone,
                    use = "mobile"
                });
            }


            return telecoms;
        }
        internal object CreatePatientName(PatientDTO patientDetails)
        {
            if (string.IsNullOrEmpty(patientDetails.bcrm_middlename))
            {
                return new
                {
                    use = "official",
                    text = patientDetails.firstname + " " + patientDetails.lastname,
                    family = patientDetails.lastname,
                    given = new string[] { patientDetails.firstname },
                    prefix = new string[] { patientDetails.bcrm_title_label }
                };
            }
            else
            {
                string[] givenNames;

                if (patientDetails.bcrm_middlename.Contains(","))
                {
                    string[] middleNames = patientDetails.bcrm_middlename.Split(',');
                    givenNames = new string[middleNames.Length + 1];
                    givenNames[0] = patientDetails.firstname;

                    for (int i = 0; i < middleNames.Length; i++)
                    {
                        givenNames[i + 1] = middleNames[i].Trim();
                    }
                }
                else
                {
                    givenNames = new string[] { patientDetails.firstname, patientDetails.bcrm_middlename.Trim() };
                }

                return new
                {
                    use = "official",
                    text = patientDetails.firstname + " " + patientDetails.bcrm_middlename + " " + patientDetails.lastname,
                    family = patientDetails.lastname,
                    given = givenNames,
                    prefix = new string[] { patientDetails.bcrm_title_label }
                };
            }
        }

        public dynamic DeceasedReadAPatientUsingJSONFHIR(PatientDTO patientDetails)
        {
            

            var modeOfCommunicationCode = "";
            var modeOfCommunicationDisplay = "";
            var communicationProficiencyCode = "";
            var communicationProficiencyDisplay = "";

            if (patientDetails.modeOfCommunication != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.modeOfCommunication, pattern);
                if (match.Success)
                {
                    modeOfCommunicationCode = match.Groups[1].Value;
                }
                modeOfCommunicationDisplay = Regex.Replace(patientDetails.modeOfCommunication, pattern, "");
                modeOfCommunicationDisplay = Regex.Replace(modeOfCommunicationDisplay, "-", "").Trim();
            }

            if (patientDetails.communicationProficiency != "")
            {
                string pattern = @"\((.*?)\)";
                Match match = Regex.Match(patientDetails.communicationProficiency, pattern);
                if (match.Success)
                {
                    communicationProficiencyCode = match.Groups[1].Value;
                }
                communicationProficiencyDisplay = Regex.Replace(patientDetails.communicationProficiency, pattern, "");
                communicationProficiencyDisplay = Regex.Replace(communicationProficiencyDisplay, "-", "").Trim();
            }

            var interpreterRequired = false;
            if (patientDetails.InterpreterRequired == "true")
            {
                interpreterRequired = true;

            }


            var patient = new
            {
                resourceType = "Patient",
                id = patientDetails.GPCSequenceNumber,
                meta = new
                {
                    versionId = patientDetails.PdsVersionId,
                    profile = new[] { "http://fhir.nhs.net/StructureDefinition/gpconnect-patient-1" }
                },
                extension = new object[]
    {
        new
        {
            url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
            extension = new Object[]
            {
                new
                {
                    url = "registrationPeriod",
                    valuePeriod = new
                    {
                        start = patientDetails.GPCRegistractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")
                    }
                },
                new
                {
                    url = "preferredBranchSurgery",
                    valueReference = new
                    {
                        reference = "Location/" + patientDetails.locationRefrenceNumber
                    }
                }
            }
        },
        new
        {
            url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
            extension = MakeDynamicallyExtensionJSON(patientDetails)
        }
    },
                identifier = new[]
    {
        new
        {
            extension = new[]
            {
                new
                {
                    url = "http://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                    valueCodeableConcept = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "http://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                code = patientDetails.NHS_number_Verification_Status_Code,
                                display = patientDetails.NHS_number_Verification_Status_Display
                            }
                        }
                    }
                }
            },
            system = "http://fhir.nhs.net/Id/nhs-number",
            value = patientDetails.bcrm_nhsnumber
        }
    },
                active = true,
                name = new object[]
                 {
                        CreatePatientName(patientDetails)
                 },
                telecom = makeTelecomJson(patientDetails),
                gender = patientDetails.gender.ToLower(),
                birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),
                deceasedDateTime = patientDetails.deceasedDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                address = new[]
    {
        new
        {
            use = "home",
            type = "physical",
            line = new[] { patientDetails.address1_line1 },
            city = patientDetails.address1_city,
            postalCode = patientDetails.address1_postalcode
        }
    },
                careProvider = new[]
    {
        new
        {
            reference = "Practitioner/" + patientDetails.stuffRefrenceNumber
        }
    }
            };


            return patient;
        }
    }
}
