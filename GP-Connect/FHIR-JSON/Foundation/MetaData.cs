namespace GP_Connect.FHIR_JSON.Foundation
{
    public class MetaData
    {
        public dynamic FoundationMetaData()
        {
            var capabilityStatementJson = new
            {
                resourceType = "CapabilityStatement",
                version = "3.0.1",
                name = "OX Digital Health",
                status = "active",
                date = "2024-07-22",
                publisher = "OXDH Beckend Team",
                contact = new[]
           {
                new { name = "Prashant Chourasiya" }
            },
                description = "This server implements the OXDH API version 3.0.1",
                copyright = "Copyright OXDH 2024",
                kind = "capability",
                software = new
                {
                    name = "IIS FHIR Server",
                    version = "3.0.1",
                    releaseDate = "2024-07-22T00:00:00+01:00"
                },
                fhirVersion = "3.0.1",
                acceptUnknown = "both",
                format = new[]
           {
                "application/fhir+json"
            },
                profile = new[]
           {
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Patient-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-PractitionerRole-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Appointment-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Schedule-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-AllergyIntolerance-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Medication-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationStatement-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-MedicationRequest-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-List-1" },
                new { reference = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-StructuredRecord-Bundle-1" }
            },
                rest = new[]
           {
                new
                {
                    mode = "server",
                    security = new
                    {
                        cors = true
                    },
                    resource = new object[]
                    {
                        new
                        {
                            type = "Patient",
                            interaction = new[]
                            {
                                new { code = "read" },
                                new { code = "search-type" }
                            },
                            searchParam = new[]
                            {
                                new
                                {
                                    name = "identifier",
                                    type = "token",
                                    documentation = "NHS Number (i.e. https://fhir.nhs.uk/Id/nhs-number|123456789)"
                                }
                            }
                        },
                        new
                        {
                            type = "Organization",
                            interaction = new[]
                            {
                                new { code = "read" },
                                new { code = "search-type" }
                            },
                            searchParam = new[]
                            {
                                new
                                {
                                    name = "identifier",
                                    type = "token",
                                    documentation = "ODS Code (i.e. https://fhir.nhs.uk/Id/ods-organization-code|Y12345)"
                                }
                            }
                        },
                        new
                        {
                            type = "Practitioner",
                            interaction = new[]
                            {
                                new { code = "read" },
                                new { code = "search-type" }
                            },
                            searchParam = new[]
                            {
                                new
                                {
                                    name = "identifier",
                                    type = "token",
                                    documentation = "SDS User Id (i.e. https://fhir.nhs.uk/Id/sds-user-id|999999)"
                                }
                            }
                        },
                        new
                        {
                            type = "Location",
                            interaction = new[]
                            {
                                new { code = "read" }
                            }
                        },
                        new
                        {
                            type = "Appointment",
                            interaction = new[]
                            {
                                new { code = "read" },
                                new { code = "create" },
                                new { code = "update" },
                                new { code = "search-type" }
                            },
                            updateCreate = false,
                            searchParam = new[]
                            {
                                new
                                {
                                    name = "identifier",
                                    type = "token",
                                    documentation = "NHS Number (i.e. https://fhir.nhs.uk/Id/nhs-number|123456789)"
                                }
                            }
                        },
                        new
                        {
                            type = "Slot",
                            interaction = new[]
                            {
                                new { code = "search-type" }
                            },
                            searchInclude = new[]
                            {
                                "Schedule:actor:Location",
                                "Schedule:actor:Practitioner",
                                "Slot:schedule",
                                "Location:managingOrganization"
                            },
                            searchParam = new[]
                            {
                                new { name = "start", type = "date" },
                                new { name = "end", type = "date" },
                                new { name = "status", type = "token" },
                                new { name = "searchFilter", type = "token" }
                            }
                        }
                    },
                    operation = new[]
                    {
                        new
                        {
                            name = "gpc.registerpatient",
                            definition = new
                            {
                                reference = "https://fhir.nhs.uk/STU3/OperationDefinition/GPConnect-RegisterPatient-Operation-1"
                            }
                        },
                        new
                        {
                            name = "gpc.getstructuredrecord",
                            definition = new
                            {
                                reference = "https://fhir.nhs.uk/STU3/OperationDefinition/GPConnect-GetStructuredRecord-Operation-1/_history/1.12"
                            }
                        }
                    }
                }
            }
            };
            return capabilityStatementJson;
        }
    }
}
