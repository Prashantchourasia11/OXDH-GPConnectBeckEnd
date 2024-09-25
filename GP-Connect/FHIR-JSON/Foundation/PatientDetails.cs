using GP_Connect.DataTransferObject;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace GP_Connect.FHIR_JSON
{
    public class PatientDetails
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Method


        public dynamic FindAPatientUsingJSONFHIR(PatientDTO patientDetails, string regType)
        {
            if (regType == "T")
            {
                return RegisterNewPatientResponse(patientDetails);
            }

            if (patientDetails.deceasedDate.ToString() != "01-01-0001 00:00:00")
            {
                return SendingResponseWithDeacesedPatient(patientDetails);
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





            var jsonContent = new
            {
                resourceType = "Bundle",
                id = patientDetails.Id,
                meta = new
                {
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                },
                type = "searchset",
                entry = new object[]
          {
                new
                {
                    resource = new
                    {
                        resourceType = "Patient",
                        id = patientDetails.GPCSequenceNumber,
                        meta = new
                        {
                            versionId = patientDetails.PdsVersionId,
                            profile = new string[]
                            {
                                "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1"
                            }
                        },
                        extension = new object[]
                        {
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
                                extension = new object[]
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
                                            reference = "Location/"+patientDetails.locationRefrenceNumber
                                        }
                                    }
                                }
                            },
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                                extension = MakeDynamicallyExtensionJSON(patientDetails)
                            }
                        },
                        identifier = new object[]
                        {
                            new
                            {
                                extension = new object[]
                                {
                                    new
                                    {
                                        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                                        valueCodeableConcept = new
                                        {
                                            coding = new object[]
                                            {
                                                new
                                                {
                                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                                    code = patientDetails.NHS_number_Verification_Status_Code,
                                                    display = patientDetails.NHS_number_Verification_Status_Display
                                                }
                                            }
                                        }
                                    }
                                },
                                system = "https://fhir.nhs.uk/Id/nhs-number",
                                value = patientDetails.bcrm_nhsnumber
                            }
                        },
                        active = patientDetails.statusReason.ToString().ToLower(),
                         name = new object[]
                    {
                        CreatePatientName(patientDetails)
                    },
                        telecom = makeTelecomJson(patientDetails),
                        gender = patientDetails.gender.ToLower(),
                        birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),

                        address = new object[]
                        {
                            new
                            {
                                use = "home",
                                type = "physical",
                                line = new string[] {patientDetails.address1_line1 },
                                city = patientDetails.address1_city,
                                district = patientDetails.address1_stateorprovince,
                                postalCode = patientDetails.address1_postalcode
                            }
                        },
                        contact = makeContactJson(patientDetails),

                        generalPractitioner = new object[]
                        {
                            new { reference = "Practitioner/" +patientDetails.stuffRefrenceNumber }
                        },
                        managingOrganization = new
                        {
                            reference = "Organization/" +patientDetails.clinicRefrenceNumber
                        }
                    }
                }
              }
            };

            return jsonContent;

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

        internal dynamic makeContactJson(PatientDTO patientDetails)
        {
            if (patientDetails.RelatedPersonFullName == null && patientDetails.RelatedPersonFamilyName == null)
            {
                return new List<object>();
            }
            else
            {

                var contacts = new object[] {
                new
                {
                    relationship = new object[]
                                {
                                    new { text = patientDetails.RelatedPersonRelationship }
                                },
                    name = new
                    {
                        use = "official",
                        text = patientDetails.RelatedPersonFullName,
                        family = patientDetails.RelatedPersonFamilyName,
                        given = new string[] { patientDetails.RelatedPersonGivenName },
                        prefix = new string[] { patientDetails.RelatedPersonPrefix }
                    },
                    telecom = new object[]
                                {
                                    new
                                    {
                                        system = "phone",
                                        value = patientDetails.RelatedPersonMobilePhone,
                                        use = "mobile"
                                    }
                                },
                    address = new
                    {
                        use = "home",
                        type = "physical",
                        line = new string[] { patientDetails.RelatedPersonAddressLine },
                        postalCode = patientDetails.RelatedPersonPostalCode
                    },
                    gender = patientDetails.RelatedPersonGender
                }
                };

                return contacts;
            }

        }


        public dynamic ReadAPatientUsingJSONFHIR(PatientDTO patientDetails)
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
                    profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1" }
                },
                extension = new object[]
    {
        new
        {
            url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
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
            url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
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
                    url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                    valueCodeableConcept = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                code = patientDetails.NHS_number_Verification_Status_Code,
                                display = patientDetails.NHS_number_Verification_Status_Display
                            }
                        }
                    }
                }
            },
            system = "https://fhir.nhs.uk/Id/nhs-number",
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
                generalPractitioner = new[]
    {
        new
        {
            reference = "Practitioner/" + patientDetails.stuffRefrenceNumber
        }
    },
                managingOrganization = new
                {
                    reference = "Organization/" + patientDetails.clinicRefrenceNumber
                }
            };


            return patient;
        }

        public dynamic PatientNotFoundUsingJSONFHIR(string id)
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
                                code = "PATIENT_NOT_FOUND",
                                display = "PATIENT_NOT_FOUND"
                            }
                        }
                    },
                    diagnostics = "No patient details found for patient ID: Patient/" +id
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic PatientIdentifierIsNotValid(string identifier)
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
                                code = "INVALID_IDENTIFIER_SYSTEM",
                                display = "INVALID_IDENTIFIER_SYSTEM"
                            }
                        }
                    },
                    diagnostics = "The given identifier system code ( " + identifier +") is not an expected code - [https://fhir.nhs.uk/Id/nhs-number]"
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic PatientIdentifierNotPresent()
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
                                code = "INVALID_PARAMETER",
                                display = "INVALID_PARAMETER"
                            }
                        }
                    },
                    diagnostics = "One or both of the identifier system and value are missing from given identifier : "
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic PatientInvalidParameter(string identifier)
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
                                code = "INVALID_PARAMETER",
                                display = "INVALID_PARAMETER"
                            }
                        }
                    },
                    diagnostics = "The identifier is invalid. System -  Value - " + identifier
                }
            }
            };
            return operationOutcomeJson;
        }
        public dynamic PatientNoIdentifierPresent()
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
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "No identifier parameter found!"
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic RegisterNewPatientResponse(PatientDTO patientDetails)
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





            var jsonContent = new
            {
                resourceType = "Bundle",
                id = patientDetails.Id,
                meta = new
                {
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                },
                type = "searchset",
                entry = new object[]
          {
                new
                {
                    resource = new
                    {
                        resourceType = "Patient",
                        id = patientDetails.GPCSequenceNumber,
                        meta = new
                        {
                            versionId = patientDetails.PdsVersionId,
                            profile = new string[]
                            {
                                "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1"
                            }
                        },
                        extension = new object[]
                        {
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
                                extension = new object[]
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
                                            reference = "Location/"+patientDetails.locationRefrenceNumber
                                        }
                                    },
                                  new
                                    {
                                       url = "registrationType",
                                         valueCodeableConcept = new
                                       {
                                         coding = new[]
                                             {
                                          new
                                         {
                                    system = "https://fhir.nhs.uk/CareConnect-RegistrationType-1",
                                    code = "T",
                                    display = "Temporary"
                                         }
                                      }
                                    }
                                  }
                                }
                            },
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                                extension = MakeDynamicallyExtensionJSON(patientDetails)
                            }
                        },
                        identifier = new object[]
                        {
                            new
                            {
                                extension = new object[]
                                {
                                    new
                                    {
                                        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                                        valueCodeableConcept = new
                                        {
                                            coding = new object[]
                                            {
                                                new
                                                {
                                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                                    code = patientDetails.NHS_number_Verification_Status_Code,
                                                    display = patientDetails.NHS_number_Verification_Status_Display
                                                }
                                            }
                                        }
                                    }
                                },
                                system = "https://fhir.nhs.uk/Id/nhs-number",
                                value = patientDetails.bcrm_nhsnumber
                            }
                        },
                        active = patientDetails.statusReason.ToString().ToLower(),
                        name = new object[]
                        {
                            new
                            {
                                use = "official",
                                text = patientDetails.fullname,
                                family = patientDetails.lastname,
                                given = new string[] { patientDetails.firstname },
                                prefix = new string[] { patientDetails.bcrm_title_label }
                            }
                        },
                        telecom = makeTelecomJson(patientDetails),
                        gender = patientDetails.gender.ToLower(),
                        birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),

                        address = new object[]
                        {
                            new
                            {
                                use = "home",
                                type = "physical",
                                line = new string[] {patientDetails.address1_line1 },
                                city = patientDetails.address1_city,
                                district = patientDetails.address1_stateorprovince,
                                postalCode = patientDetails.address1_postalcode
                            }
                        },
                        contact = makeContactJson(patientDetails),

                        generalPractitioner = new object[]
                        {
                            new { reference = "Practitioner/" +patientDetails.stuffRefrenceNumber }
                        },
                        managingOrganization = new
                        {
                            reference = "Organization/" +patientDetails.clinicRefrenceNumber
                        }
                    }
                }
              }
            };

            return jsonContent;

        }

        public dynamic SendingResponseWithDeacesedPatient(PatientDTO patientDetails)
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





            var jsonContent = new
            {
                resourceType = "Bundle",
                id = patientDetails.Id,
                meta = new
                {
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
                },
                type = "searchset",
                entry = new object[]
          {
                new
                {
                    resource = new
                    {
                        resourceType = "Patient",
                        id = patientDetails.GPCSequenceNumber,
                        meta = new
                        {
                            versionId = patientDetails.PdsVersionId,
                            profile = new string[]
                            {
                                "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1"
                            }
                        },
                        extension = new object[]
                        {
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-RegistrationDetails-1",
                                extension = new object[]
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
                                            reference = "Location/"+patientDetails.locationRefrenceNumber
                                        }
                                    }
                                }
                            },
                            new
                            {
                                url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                                extension = MakeDynamicallyExtensionJSON(patientDetails)
                            }
                        },
                        identifier = new object[]
                        {
                            new
                            {
                                extension = new object[]
                                {
                                    new
                                    {
                                        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSNumberVerificationStatus-1",
                                        valueCodeableConcept = new
                                        {
                                            coding = new object[]
                                            {
                                                new
                                                {
                                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-NHSNumberVerificationStatus-1",
                                                    code = patientDetails.NHS_number_Verification_Status_Code,
                                                    display = patientDetails.NHS_number_Verification_Status_Display
                                                }
                                            }
                                        }
                                    }
                                },
                                system = "https://fhir.nhs.uk/Id/nhs-number",
                                value = patientDetails.bcrm_nhsnumber
                            }
                        },
                        active = patientDetails.statusReason.ToString().ToLower(),
                        name = new object[]
                        {
                            new
                            {
                                use = "official",
                                text = patientDetails.fullname,
                                family = patientDetails.lastname,
                                given = new string[] { patientDetails.firstname },
                                prefix = new string[] { patientDetails.bcrm_title_label }
                            }
                        },
                        telecom = makeTelecomJson(patientDetails),
                        gender = patientDetails.gender.ToLower(),
                        birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),
                        deceasedDateTime = patientDetails.deceasedDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        address = new object[]
                        {
                            new
                            {
                                use = "home",
                                type = "physical",
                                line = new string[] {patientDetails.address1_line1 },
                                city = patientDetails.address1_city,
                                district = patientDetails.address1_stateorprovince,
                                postalCode = patientDetails.address1_postalcode
                            }
                        },
                        contact = makeContactJson(patientDetails),

                        generalPractitioner = new object[]
                        {
                            new { reference = "Practitioner/" +patientDetails.stuffRefrenceNumber }
                        },
                        managingOrganization = new
                        {
                            reference = "Organization/" +patientDetails.clinicRefrenceNumber
                        }
                    }
                }
              }
            };

            return jsonContent;
        }

        public dynamic WrongInteractionId(string sspInterectionId, string source)
        {
            var errorMessage = "";
            if (sspInterectionId == "urn:nhs:names:services:gpconnect:fhir:rest:read:practitioner-1")
            {
                errorMessage = "Resource in request does not match resource in interaction (request - " + source + " interaction - practitioner)";
            }
            else if (sspInterectionId == "urn:nhs:names:services:gpconnect:fhir:rest:read:location-1")
            {
                errorMessage = "Resource in request does not match resource in interaction (request - " + source + " interaction - location)";
            }
            else if (sspInterectionId == "urn:nhs:names:services:gpconnect:fhir:rest:read:patient-1")
            {
                errorMessage = "Resource in request does not match resource in interaction (request - " + source + " interaction - patient)";
            }
            else if (sspInterectionId == "urn:nhs:names:services:gpconnect:fhir:rest:read:organization-1")
            {
                errorMessage = "Resource in request does not match resource in interaction (request - " + source + " interaction - organization)";
            }
            else
            {
                errorMessage = "Unable to locate interaction corresponding to the given interaction ID (" + sspInterectionId + ")";
            }

            var operationOutcomeJson = new
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
                    diagnostics = errorMessage
                }
            }
            };
            return operationOutcomeJson;
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

        public dynamic EmptyBuddlePatientJSON(string id1)
        {
            var jsonContent = new
            {
                id = id1,
                resourceType = "Bundle",
                type = "searchset",
                entry = new object[] { }
            };
            return jsonContent;
        }
        #endregion
    }
}
