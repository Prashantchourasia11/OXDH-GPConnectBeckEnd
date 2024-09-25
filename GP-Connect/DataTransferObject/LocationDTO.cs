namespace GP_Connect.DataTransferObject
{
    public class LocationDTO
    {
        public string sequenceId { get; set; }

        public string versionId { get; set; }

        public DateTime lastupdated { get; set; }

        public string status { get; set; }

        public string name { get; set; }

        public string addressLine { get; set; }

        public string city { get; set; }

        public string district { get; set; }

        public string postalcode { get; set; }

        public string country { get; set; }

        public string managingOrganisationsequenceNumber { get; set; }

        public string telecomUse { get; set; }  
        public string telecomSystem { get; set; }   
        public string telecomValue { get; set; }
    }
}
