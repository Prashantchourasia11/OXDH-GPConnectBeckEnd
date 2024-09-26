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
    }
}
