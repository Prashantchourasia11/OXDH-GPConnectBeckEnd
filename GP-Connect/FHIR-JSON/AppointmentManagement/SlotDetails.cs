namespace GP_Connect.FHIR_JSON.AppointmentManagement
{
    public class SlotDetails
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Method

        public string GetFreeSlotJSON()
        {
                   var FreeSlotJSON = @"{
                               ""resourceType"": ""Bundle"",
                               ""type"": ""searchset"",
                               ""entry"": [
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Slot"",
                                     ""id"": ""100001"",
                                     ""meta"": {
                                       ""versionId"": ""2000"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1""
                                       ]
                                     },
                                     ""extension"": [
                                       {
                                         ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                         ""valueCode"": ""In-person""
                                       }
                                     ],
                                     ""serviceType"": [
                                       {
                                         ""text"": ""General GP Appointment""
                                       }
                                     ],
                                     ""schedule"": {
                                       ""reference"": ""Schedule/1002""
                                     },
                                     ""status"": ""free"",
                                     ""start"": ""2024-04-12T11:00:00+01:00"",
                                     ""end"": ""2024-04-12T11:30:00+01:00""
                                   }
                                 },
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Slot"",
                                     ""id"": ""100002"",
                                     ""meta"": {
                                       ""versionId"": ""400"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1""
                                       ]
                                     },
                                     ""extension"": [
                                       {
                                         ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                         ""valueCode"": ""Video""
                                       }
                                     ],
                                     ""serviceType"": [
                                       {
                                         ""text"": ""NHS Health Check""
                                       }
                                     ],
                                     ""schedule"": {
                                       ""reference"": ""Schedule/1001""
                                     },
                                     ""status"": ""free"",
                                     ""start"": ""2024-04-12T09:00:00+01:00"",
                                     ""end"": ""2024-04-12T09:30:00+01:00""
                                   }
                                 },
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Schedule"",
                                     ""id"": ""1001"",
                                     ""meta"": {
                                       ""versionId"": ""300"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Schedule-1""
                                       ]
                                     },
                                     ""extension"": [
                                       {
                                         ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-PractitionerRole-1"",
                                         ""valueCodeableConcept"": {
                                           ""coding"": [
                                             {
                                               ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1"",
                                               ""code"": ""R0260"",
                                               ""display"": ""General Medical Practitioner""
                                             }
                                           ]
                                         }
                                       }
                                     ],
                                     ""serviceCategory"": {
                                       ""text"": ""General GP Appointments""
                                     },
                                     ""actor"": [
                                       {
                                         ""reference"": ""Location/999""
                                       },
                                       {
                                         ""reference"": ""Practitioner/1000""
                                       }
                                     ],
                                     ""planningHorizon"": {
                                       ""start"": ""2024-04-11T08:00:00+01:00"",
                                       ""end"": ""2024-04-20T07:00:00+01:00""
                                     }
                                   }
                                 },
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Practitioner"",
                                     ""id"": ""1000"",
                                     ""meta"": {
                                       ""versionId"": ""3000"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1""
                                       ]
                                     },
                                     ""identifier"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/Id/sds-user-id"",
                                         ""value"": ""G82657""
                                       }
                                     ],
                                     ""name"": [
                                       {
                                         ""family"": ""OXDH"",
                                         ""given"": [
                                           ""TesterA""
                                         ],
                                         ""prefix"": [
                                           ""Mr""
                                         ]
                                       }
                                     ],
                                     ""gender"": ""female""
                                   }
                                 },
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Location"",
                                     ""id"": ""999"",
                                     ""meta"": {
                                       ""versionId"": ""5"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1""
                                       ]
                                     },
                                     ""name"": ""Building A"",
                                     ""address"": {
                                       ""line"": [
                                         ""23 Main Street , Pudsey"" 
                                       ],
                                       ""postalCode"": ""GPC 111""
                                     },
                                     ""telecom"": {
                                       ""system"": ""phone"",
                                       ""value"": ""03003035678"",
                                       ""use"": ""work""
                                     },
                                     ""managingOrganization"": {
                                       ""reference"": ""Organization/3500""
                                     }
                                   }
                                 },
                                 {
                                   ""resource"": {
                                     ""resourceType"": ""Organization"",
                                     ""id"": ""2000"",
                                     ""meta"": {
                                       ""versionId"": ""3000"",
                                       ""profile"": [
                                         ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                       ]
                                     },
                                     ""identifier"": [
                                       {
                                         ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                         ""value"": ""A00001""
                                       }
                                     ],
                                     ""name"": ""The Trevelyan Practice"",
                                     ""address"": {
                                       ""line"": [
                                         ""Trevelyan Square""
                                       ],
                                       ""city"": ""Hounslow"",
                                       ""district"": ""West Yorkshire"",
                                       ""postalCode"": ""TW6 1EW""
                                     },
                                     ""telecom"": {
                                       ""system"": ""phone"",
                                       ""value"": ""44000000000"",
                                       ""use"": ""work""
                                     }
                                   }
                                 }
                               ]
                             }";

              return FreeSlotJSON;
          }



        #endregion


    }
}
