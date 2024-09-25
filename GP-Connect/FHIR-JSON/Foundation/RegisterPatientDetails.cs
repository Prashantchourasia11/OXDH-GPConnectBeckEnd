using GP_Connect.DataTransferObject;
using Microsoft.SharePoint.News.DataModel;
using System.Text.RegularExpressions;

namespace GP_Connect.FHIR_JSON.Foundation
{
    public class RegisterPatientDetails
    {

        public dynamic RegisterANewPatientAUsingJSONFHIR(PatientDTO patientDetails, string existStatus, string addressTypeIsTemp, string telecomTypeIsTemp, Dictionary<string, string> pdsAddress)
        {
         
            if(existStatus == "yes")
            {
                var res = RegisterAExistPatientAUsingJSONFHIR(patientDetails, existStatus, addressTypeIsTemp, telecomTypeIsTemp, pdsAddress);
                return res;
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
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[]
                    {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Searchset-Bundle-1"
                    }
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
                                       url = "registrationType",
                                         valueCodeableConcept = new
                                       {
                                         coding = new[]
                                             {
                                          new
                                         {
                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-RegistrationType-1",
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
                        CreatePatientName(patientDetails)
                        },
                        telecom = makeTelecomJson(patientDetails),
                        gender = patientDetails.gender.ToLower(),
                        birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),

                        address = makeAddressJSON(patientDetails,addressTypeIsTemp,pdsAddress),
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


        public dynamic RegisterAExistPatientAUsingJSONFHIR(PatientDTO patientDetails, string existStatus, string addressTypeIsTemp, string telecomTypeIsTemp, Dictionary<string, string> pdsAddress)
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
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[]
                    {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Searchset-Bundle-1"
                    }
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
                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-RegistrationType-1",
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
                        CreatePatientName(patientDetails)
                        },
                        telecom = makeTelecomJson(patientDetails),
                        gender = patientDetails.gender.ToLower(),
                        birthDate = patientDetails.birthdate.ToString("yyyy-MM-dd"),

                        address = makeAddressJSON(patientDetails,addressTypeIsTemp,pdsAddress),
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
        public object CreatePatientName(PatientDTO patientDetails)
        {
            if (string.IsNullOrEmpty(patientDetails.bcrm_middlename))
            {
                return new
                {
                    use = "official",
                    text = patientDetails.firstname + " " + patientDetails.lastname,
                    family = patientDetails.lastname,
                    given = new string[] { patientDetails.firstname }
                   
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
                    given = givenNames
                   
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

            //extensions.Add(new
            //{
            //    url = "preferred",
            //    valueBoolean = false
            //});

            //extensions.Add(new
            //{
            //    url = "modeOfCommunication",
            //    valueCodeableConcept = new
            //    {
            //        coding = new object[]
            //        {
            //        new
            //        {
            //            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityMode-1",
            //            code = modeOfCommunicationCode,
            //            display = modeOfCommunicationDisplay
            //        }
            //        }
            //    }
            //});

            //extensions.Add(new
            //{
            //    url = "communicationProficiency",
            //    valueCodeableConcept = new
            //    {
            //        coding = new object[]
            //        {
            //        new
            //        {
            //            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-LanguageAbilityProficiency-1",
            //            code = communicationProficiencyCode,
            //            display = communicationProficiencyDisplay
            //        }
            //        }
            //    }
            //});

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
            if (patientDetails.tempPhone != "")
            {
                telecoms.Add(new
                {
                    system = "phone",
                    value = patientDetails.tempPhone,
                    use = "temp",
                    period = new
                    {
                        start = patientDetails.GPCRegistractionDate.ToString("yyyy-MM-dd"),
                        end = patientDetails.GPCRegistractionDate.AddMonths(3).ToString("yyyy-MM-dd")
                    }
                });
            }
            if (patientDetails.tempEmail != "")
            {
                telecoms.Add(new
                {
                    system = "email",
                    value = patientDetails.tempEmail,
                    use = "temp",
                    period = new
                    {
                        start = patientDetails.GPCRegistractionDate.ToString("yyyy-MM-dd"),
                        end = patientDetails.GPCRegistractionDate.AddMonths(3).ToString("yyyy-MM-dd")
                    }
                });
            }


            return telecoms;
        }
        internal dynamic makeAddressJSON(PatientDTO patientDetails,string IsTempRecord, Dictionary<string, string> pdsAddress)
        {
            var useValue = "temp";
            if(IsTempRecord == "no")
            {
                useValue = "home";
            }

            var address = new Dictionary<string, object>
             {
                 { "use", useValue },
                 { "type", "physical" }
             };

            if (patientDetails.address1_line1 != null)
            {
                address["line"] = new string[] { patientDetails.address1_line1 };
            }

            if (patientDetails.address1_city != null)
            {
                address["city"] = patientDetails.address1_city;
            }

          

            if (patientDetails.address1_postalcode != null)
            {
                address["postalCode"] = patientDetails.address1_postalcode;
            }

            if(IsTempRecord == "yes")
            {
                address["period"] = new
                {
                    start = patientDetails.GPCRegistractionDate.ToString("yyyy-MM-dd"),
                    end = patientDetails.GPCRegistractionDate.AddMonths(3).ToString("yyyy-MM-dd")
                };
            }

           

            bool isAddressEmpty = address.Count == 2; // Only 'use' and 'type' are in the dictionary

            if (isAddressEmpty)
            {
                return new object[]
                {
                  new
                  {
                      use = "home",
                      type = "physical"
                  }
                 };
                }

            if(IsTempRecord != "no")
            {
              pdsAddress.TryGetValue("use", out var use);
              if (use != null)
              {
              var pdsAddressObj = new Dictionary<string, object>
              {
                  { "use", use },
                  { "type", "physical" }
              };

                if (pdsAddress.TryGetValue("address1_line1", out var line1))
                {
                        if(line1 != null)
                        {
                            if (line1.Contains(","))
                            {
                                var lines = line1
                               .Split(',')
                               .Select(line => line.Trim())
                               .ToArray();
                                pdsAddressObj["line"] = lines;
                            }
                            else
                            {
                                pdsAddressObj["line"] = line1;
                            }
                        }

                    
                }

                if (pdsAddress.TryGetValue("address1_city", out var city))
                {
                        if(city != null)
                        {
                            pdsAddressObj["city"] = city;
                        }

                  
                }

                if (pdsAddress.TryGetValue("address1_postalcode", out var postalCode))
                {
                        if (postalCode != null)
                        {
                            pdsAddressObj["postalCode"] = postalCode;
                        }
                        
                }

                if (pdsAddress.TryGetValue("address1_stateorprovince", out var state))
                {
                        if (state != null)
                        {
                            pdsAddressObj["state"] = state;
                        }
                
                }
                return new object[] { address, pdsAddressObj };
                }
            }


            return new object[] { address };
        }

        //internal dynamic makeAddressJSON1(PatientDTO patientDetails)
        //{
        //    if(patientDetails.address1_line1 == null && patientDetails.address1_city == null)
        //    {
        //        var res = new object[]
        //                              {
        //                    new
        //                    {
        //                        use = "temp",
        //                        type = "physical",
        //                        line = new string[] {patientDetails.address1_line1 },
        //                        city = patientDetails.address1_city,
        //                        district = patientDetails.address1_stateorprovince,
        //                        postalCode = patientDetails.address1_postalcode
        //                    }
        //                              };
        //        return res;
        //    }
        //    else
        //    {
        //        var res = new object[]
        //                             {
        //                    new
        //                    {
        //                        use = "home",
        //                        type = "physical"
                             
        //                    }
        //                  };
        //        return res;
        //    }
         
        //}

        public dynamic RegisterPatientInvalidNHSNumber(string nhsNumber)
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
                                code = "INVALID_NHS_NUMBER",
                                display = "INVALID_NHS_NUMBER"
                            }
                        }
                    },
                    diagnostics = "Invalid NHS number submitted: " + nhsNumber
                }
            }
            };
            return operationOutcome;

        }
     
        public dynamic DuplicateTelecomField(string system)
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
                    diagnostics = "Only one Telecom of type "+system+" is allowed in a register patient request."
                }
            }
            };
            return operationOutcome;

        }

        public dynamic DuplicateAddressUseValue()
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
                                code = "INVALID_RESOURCE",
                                display = "INVALID_RESOURCE"
                            }
                        }
                    },
                    diagnostics = "Only a single address of each use type can be sent in a register patient request."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic MultipleSameExtensionFound()
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
                                code = "INVALID_RESOURCE",
                                display = "INVALID_RESOURCE"
                            }
                        }
                    },
                    diagnostics = "Invalid/multiple patient extensions found."
                }
            }
            };
            return operationOutcome;
        }
        public dynamic OldAddressUseValue()
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
                                code = "INVALID_RESOURCE",
                                display = "INVALID_RESOURCE"
                            }
                        }
                    },
                    diagnostics = "Address use type OLD cannot be sent in a register patient request."
                }
            }
            };
            return operationOutcome;

        }

        public dynamic WorkAddressUseValue()
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
                                code = "INVALID_RESOURCE",
                                display = "INVALID_RESOURCE"
                            }
                        }
                    },
                    diagnostics = "Address use type WORK cannot be sent in a register patient request."
                }
            }
            };
            return operationOutcome;

        }

        public dynamic UnneccassaryFields(string fieldName)
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
                                    code = "INVALID_RESOURCE",
                                    display = "INVALID_RESOURCE"
                                }
                            }
                        },
                        diagnostics = "The following properties have been constrained out on the Patient resource - " + fieldName
                    }
                }
            };
            return operationOutcome;
        }

        public dynamic InvalidDemographicNHS(string NHSNumber)
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
                                code = "INVALID_PATIENT_DEMOGRAPHICS",
                                display = "INVALID_PATIENT_DEMOGRAPHICS"
                            }
                        }
                    },
                    diagnostics = "Patient (NHS number - "+NHSNumber+") has invalid demographics"
                }
            }
            };
            return operationOutcome;    
        }

        public dynamic PatientIsAlreadyExistInOurGP(string NHSNumber)
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
                                code = "DUPLICATE_REJECTED",
                                display = "DUPLICATE_REJECTED"
                            }
                        }
                    },
                    diagnostics = "Patient (NHS number - "+NHSNumber+") already exists"
                }
            }
            };
            return operationOutcome;
        }

        public dynamic NoFamilyNameInJSON()
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
                    diagnostics = "The patient must have one and only one family name property. Found 0"
                }
            }
            };

            return operationOutcome;
        }

        public dynamic NoDobSupplied()
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
                    diagnostics = "The Patient date of birth must be supplied"
                }
            }
            };
            return operationOutcome;    
        }

        public dynamic SupersededNHSNumber(string NHSnumber)
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
                                code = "INVALID_NHS_NUMBER",
                                display = "INVALID_NHS_NUMBER"
                            }
                        }
                    },
                    diagnostics = "Patient (NHS number - "+NHSnumber+") is superseded"
                }
            }
            };

            return operationOutcome;
        }

        public dynamic NoOfficialSupplied()
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
                    diagnostics = "The patient must have one Active Name with a Use of OFFICIAL"
                }
            }
            };
            return operationOutcome;
        }
        internal dynamic MoreThanOneResources()
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
                    diagnostics = "The parameter registerPatient cannot be set more than once"
                }
            }
            };
            return operationOutcome;    
        }
        public dynamic NotPassingValueOrSystem()
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
                    diagnostics = "Failed to parse request body as JSON resource. Error was: Failed to parse JSON encoded FHIR content: com.google.gson.stream.MalformedJsonException: Use JsonReader.setLenient(true) to accept malformed JSON at line 1 column 4 path $"
                }
            }
            };
            return operationOutcome;

        }

        public dynamic JWTClaimIssue()
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
                    diagnostics = "The claim requested scope does not match the requested operation (write)"
                }
            }
            };
            return operationOutcome;
        }

    }
}
