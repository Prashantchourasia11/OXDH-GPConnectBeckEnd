using GP_Connect.DataTransferObject;

namespace GP_Connect.FHIR_JSON.AppointmentManagement
{
    public class AppointmentByAppoIDDetails
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Method

        public string GetJSONBYAppointmentId(AppointmentByIdDTO appDetails)
        {
            var appointmentIdJson = @"{
                               ""resourceType"": ""Appointment"",
                               ""id"": """+appDetails.appointmnetId+@""",
                               ""meta"": {
                                 ""versionId"": """+appDetails.versionNumber+@""",
                                 ""profile"": [
                                   ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Appointment-1""
                                 ]
                               },
                               ""contained"": [
                                 {
                                   ""resourceType"": ""Organization"",
                                   ""id"": """+appDetails.organizationId+@""",
                                   ""meta"": {
                                     ""profile"": [
                                       ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                     ]
                                   },
                                   ""identifier"": [
                                     {
                                       ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                       ""value"": """+appDetails.odsCode+@"""
                                     }
                                   ],
                                   ""type"": [
                                     {
                                       ""coding"": [
                                         {
                                           ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/GPConnect-OrganisationType-1"",
                                           ""code"": """+appDetails.organizationType+@"""
                                         }
                                       ]
                                     }
                                   ],
                                   ""name"": """+appDetails.organizationName+@""",
                                   ""telecom"": [
                                     {
                                       ""system"": ""phone"",
                                       ""value"": """+appDetails.PhoneNumber+@"""
                                     }
                                   ]
                                 }
                               ],
                               ""extension"": [
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-BookingOrganisation-1"",
                                   ""valueReference"": {
                                     ""reference"": ""#"+appDetails.organizationId+@"""
                                   }
                                 },
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-PractitionerRole-1"",
                                   ""valueCodeableConcept"": {
                                     ""coding"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1"",
                                         ""code"": """+appDetails.PractitionerRoleCode+@""",
                                         ""display"": """+appDetails.PractionerDisplayRole+@"""
                                       }
                                     ]
                                   }
                                 },
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                   ""valueCode"": """+appDetails.DeleiveryChannel+@"""
                                 }
                               ],
                               ""status"": """+appDetails.Status.ToString().ToLower()+@""",
                               ""serviceCategory"": {
                                 ""text"": """+appDetails.ServiceCategory+@"""
                               },
                               ""serviceType"": [
                                 {
                                   ""text"": """+appDetails.ServiceCategory+@"""
                                 }
                               ],
                               ""description"": """+appDetails.Description+@""",
                               ""start"": """+appDetails.startDate.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""end"": """+appDetails.endDate.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""slot"": [
                                 {
                                   ""reference"": ""Slot/"+appDetails.SlotReference+@"""
                                 }
                               ],
                               ""created"": """+appDetails.createdOn.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""comment"": """+appDetails.comments+@""",
                               ""participant"": [
                                 {
                                   ""actor"": {
                                     ""reference"": ""Patient/"+appDetails.patientReference+@"""
                                   },
                                   ""status"": ""accepted""
                                 },
                                 {
                                   ""actor"": {
                                     ""reference"": ""Location/"+appDetails.locationReference+@"""
                                   },
                                   ""status"": ""accepted""
                                 },
                                 {
                                   ""actor"": {
                                     ""reference"": ""Practitioner/"+appDetails.PractionerReference+@"""
                                   },
                                   ""status"": ""accepted""
                                 }
                               ]
                             }";
            return appointmentIdJson;

        }

        public string GetJSONBYCancelAppointmentId(AppointmentByIdDTO appDetails)
        {

            var appointmentIdJson = @"{
                               ""resourceType"": ""Appointment"",
                               ""id"": """ + appDetails.appointmnetId + @""",
                               ""meta"": {
                                 ""versionId"": """ + appDetails.versionNumber + @""",
                                 ""profile"": [
                                   ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Appointment-1""
                                 ]
                               },
                               ""contained"": [
                                 {
                                   ""resourceType"": ""Organization"",
                                   ""id"": """ + appDetails.organizationId + @""",
                                   ""meta"": {
                                     ""profile"": [
                                       ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                     ]
                                   },
                                   ""identifier"": [
                                     {
                                       ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                       ""value"": """ + appDetails.odsCode + @"""
                                     }
                                   ],
                                   ""type"": [
                                     {
                                       ""coding"": [
                                         {
                                           ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/GPConnect-OrganisationType-1"",
                                           ""code"": """ + appDetails.organizationType + @"""
                                         }
                                       ]
                                     }
                                   ],
                                   ""name"": """ + appDetails.organizationName + @""",
                                   ""telecom"": [
                                     {
                                       ""system"": ""phone"",
                                       ""value"": """ + appDetails.PhoneNumber + @"""
                                     }
                                   ]
                                 }
                               ],
                               ""extension"": [
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-BookingOrganisation-1"",
                                   ""valueReference"": {
                                     ""reference"": ""#" + appDetails.organizationId + @"""
                                   }
                                 },
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-PractitionerRole-1"",
                                   ""valueCodeableConcept"": {
                                     ""coding"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1"",
                                         ""code"": """ + appDetails.PractitionerRoleCode + @""",
                                         ""display"": """ + appDetails.PractionerDisplayRole + @"""
                                       }
                                     ]
                                   }
                                 },
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                   ""valueCode"": """ + appDetails.DeleiveryChannel + @"""
                                 },
                                 {
                                   ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-AppointmentCancellationReason-1"",
                                   ""valueString"": """ + appDetails.CancellationReson + @"""
                                 }
                               ],
                               ""status"": """ + appDetails.Status.ToString().ToLower() + @""",
                               ""serviceCategory"": {
                                 ""text"": """ + appDetails.ServiceCategory + @"""
                               },
                               ""serviceType"": [
                                 {
                                   ""text"": """ + appDetails.ServiceCategory + @"""
                                 }
                               ],
                               ""description"": """ + appDetails.Description + @""",
                               ""start"": """ + appDetails.startDate.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""end"": """ + appDetails.endDate.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""slot"": [
                                 {
                                   ""reference"": ""Slot/" + appDetails.SlotReference + @"""
                                 }
                               ],
                               ""created"": """ + appDetails.createdOn.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                               ""comment"": " + appDetails.comments + @",
                               ""participant"": [
                                 {
                                   ""actor"": {
                                     ""reference"": ""Patient/" + appDetails.patientReference + @"""
                                   },
                                   ""status"": ""accepted""
                                 },
                                 {
                                   ""actor"": {
                                     ""reference"": ""Location/" + appDetails.locationReference + @"""
                                   },
                                   ""status"": ""accepted""
                                 },
                                 {
                                   ""actor"": {
                                     ""reference"": ""Practitioner/" + appDetails.PractionerReference + @"""
                                   },
                                   ""status"": ""accepted""
                                 }
                               ]
                             }";
            return appointmentIdJson;

        }

        public dynamic BadRequestAppointmentJSON(string id)
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
                                code = "REFERENCE_NOT_FOUND",
                                display = "REFERENCE_NOT_FOUND"
                            }
                        }
                    },
                    diagnostics = "Appointment Id " + id + "Does Not Exist."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic MakeEmptyBuddleRetrieveAppointmentJSON()
        {
            var jsonContent = new
            {
                resourceType = "Bundle",
                type = "searchset",
                entry = new object[] { }
            };
            return jsonContent;
        }
        public dynamic InvalidParameterStartDateIsGreaterThanEndDateJSON()
        {
            var json = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            {
                "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/StructureDefinition/gpconnect-operationoutcome-1"
                        }
                    }
                }
            },
            {
                "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "invalid" },
                        {
                            "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/ValueSet/gpconnect-error-or-warning-code-1" },
                                            { "code", "INVALID_PARAMETER" },
                                            { "display", "Invalid Parameter" }
                                        }
                                    }
                                }
                            }
                        },
                        { "diagnostics", "start date should not be greater than end date" }
                    }
                }
            }
        };
            return json;
        }

        public dynamic BadRequestSSPInteractionIdNotMatchedJSON()
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
                    diagnostics = "ssp interaction id is not correct."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic InvalidParameterStartDateShouldNotBePassedDateJSON()
        {
            var json = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            {
                "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/StructureDefinition/gpconnect-operationoutcome-1"
                        }
                    }
                }
            },
            {
                "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "invalid" },
                        {
                            "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/ValueSet/gpconnect-error-or-warning-code-1" },
                                            { "code", "INVALID_PARAMETER" },
                                            { "display", "Invalid Parameter" }
                                        }
                                    }
                                }
                            }
                        },
                        { "diagnostics", "start date should not be past date" }
                    }
                }
            }
        };
            return json;
        }
        public dynamic PatientNotFoundUsingJSONFHIR()
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
                                code = "REFERENCE_NOT_FOUND",
                                display = "REFERENCE_NOT_FOUND"
                            }
                        }
                    },
                    diagnostics = "Id not matched"
                }
            }
            };
            return operationOutcomeJson;
        }

        public dynamic InvalidParameterDateFormatJSON()
        {
            var json = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            {
                "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/StructureDefinition/gpconnect-operationoutcome-1"
                        }
                    }
                }
            },
            {
                "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "invalid" },
                        {
                            "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/ValueSet/gpconnect-error-or-warning-code-1" },
                                            { "code", "INVALID_PARAMETER" },
                                            { "display", "Invalid Parameter" }
                                        }
                                    }
                                }
                            }
                        },
                        { "diagnostics", "date format is invalid or contain invalid prefix" }
                    }
                }
            }
        };
            return json;
        }

        public dynamic BadRequestJWTTokenNotMatchedJSON()
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
                    diagnostics = "jwt token requested scope mismatched."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic InvalidResourceFoundJSON(string text)
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
                                display = "Invalid Resource"
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic SlotNotAvailbleJSON()
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
                                display = "Create would lead to creation of duplicate resource"
                            }
                        }
                    },
                    diagnostics = "Slot Not Available."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic IfMatchIsNotOkJSON()
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
                                display = "Version Id Not Matched."
                            }
                        }
                    },
                    diagnostics = "Version Id is invalid"
                }
            }
            };
            return operationOutcome;
        }

        public dynamic IfMatchFHIRIsNotOkJSON()
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
                                code = "FHIR_CONSTRAINT_VIOLATION",
                                display = "FHIR CONSTRAINT VIOLATION"
                            }
                        }
                    },
                    diagnostics = "Version Id is invalid"
                }
            }
            };
            return operationOutcome;
        }
        public dynamic InvalidParameterJSON(string text)
        {
            var json = new Dictionary<string, object>
        {
            { "resourceType", "OperationOutcome" },
            {
                "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string>
                        {
                            "https://fhir.nhs.uk/StructureDefinition/gpconnect-operationoutcome-1"
                        }
                    }
                }
            },
            {
                "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "invalid" },
                        {
                            "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/ValueSet/gpconnect-error-or-warning-code-1" },
                                            { "code", "INVALID_PARAMETER" },
                                            { "display", "Invalid Parameter" }
                                        }
                                    }
                                }
                            }
                        },
                        { "diagnostics", text }
                    }
                }
            }
        };
            return json;
        }

        public dynamic InvalidResourceFoundWhenPastAppointmentAccessJSON(string text)
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
                                display = "Submitted resource is not valid."
                            }
                        }
                    },
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        public dynamic BadRequestJSON(string text)
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
                    diagnostics = text
                }
            }
            };
            return operationOutcome;
        }

        #endregion
    }
}
