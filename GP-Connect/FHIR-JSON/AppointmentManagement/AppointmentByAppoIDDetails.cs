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
        #endregion
    }
}
