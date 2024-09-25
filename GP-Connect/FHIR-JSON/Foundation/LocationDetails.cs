using GP_Connect.DataTransferObject;

namespace GP_Connect.FHIR_JSON
{
    public class LocationDetails
    {
        public string ReadALocationJSON(LocationDTO locationDetails)
        {

            try
            {
                var locationJSON = @"{
                                       ""resourceType"": ""Location"",
                                         ""id"": """ + locationDetails.sequenceId + @""",
                                         ""meta"": {
                                           ""versionId"": """ + locationDetails.versionId + @""",
                                           ""lastUpdated"": """ + locationDetails.lastupdated.ToString("yyyy-MM-ddTHH:mm:sszzz") + @""",
                                           ""profile"": [
                                             ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1""
                                           ]
                                         },
                                         ""status"": """ + locationDetails.status + @""",
                                         ""name"": """ + locationDetails.name + @""",
                                         ""address"": {
                                           ""line"": [
                                             """ + locationDetails.addressLine + @"""
                                           ],
                                           ""city"": """ + locationDetails.city + @""",
                                           ""district"": """ + locationDetails.district + @""",
                                           ""postalCode"": """ + locationDetails.postalcode + @""",
                                           ""country"": """ + locationDetails.country + @"""
                                         },
                                          ""telecom"": [
                                                   {
                                                     ""system"": """+locationDetails.telecomSystem+@""",
                                                     ""value"": """+locationDetails.telecomValue+@""",
                                                     ""use"": """+locationDetails.telecomUse+@"""
                                                   }],
                                         ""managingOrganization"": {
                                           ""reference"": ""Organization/"+locationDetails.managingOrganisationsequenceNumber+@"/_history/"+locationDetails.versionId+@"""
                                         }
                                     }";


                return locationJSON;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public dynamic ReadALocationFHIRJSON(LocationDTO locationDetails)
        {


            var locationJson = new
            {
                resourceType = "Location",
                id = locationDetails.sequenceId,
                meta = new
                {
                    versionId = locationDetails.versionId,
                    lastUpdated = locationDetails.lastupdated.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    profile = new[]
        {
                        "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1"
                    }
                },
                status = locationDetails.status,
                name = locationDetails.name,
                address = new
                {
                    line = new[]
        {
                        locationDetails.addressLine
                    },
                    city = locationDetails.city,
                    district = locationDetails.district,
                    postalCode = locationDetails.postalcode,
                    country = locationDetails.country
                },
                telecom = new
                {
                    system = locationDetails.telecomSystem,
                    value = locationDetails.telecomValue,
                    use = locationDetails.telecomUse
                },
                managingOrganization = new
                {
                    //  reference = "Organization/" + locationDetails.managingOrganisationsequenceNumber + "/_history/" + locationDetails.versionId 
                    reference = "Organization/" + locationDetails.managingOrganisationsequenceNumber
                }
            };
            return locationJson;
        }
        


    }
}
