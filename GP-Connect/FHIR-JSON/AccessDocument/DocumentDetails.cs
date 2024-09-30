namespace GP_Connect.FHIR_JSON.AccessDocument
{
    public class DocumentDetails
    {
        public dynamic DocumentReferenceTurnedOff()
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
                                code = "ACCESS_DENIED",
                                display = "ACCESS_DENIED"
                            }
                        }
                    },
                    diagnostics = "The provider has disabled access to the document reference."
                }
            }
            };
            return operationOutcome;
        }

        public dynamic InvalidAuthoreJSON()
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
                        { "diagnostics", "author search parameter is Invalid" }
                    }
                }
            }
        };
            return json;
        }

        public dynamic InvalidParameterJSON()
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
                        { "diagnostics", "Invalid parameter" }
                    }
                }
            }
        };
            return json;
        }


    }
}
