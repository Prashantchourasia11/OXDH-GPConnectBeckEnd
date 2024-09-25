namespace GP_Connect.DataTransferObject
{
    public class OrganizationDTO
    {
        public string resourceId { get; set; }

        public string organizationName { get; set; }

        public DateTime lastUpdated { get; set; }

        public string sequenceNumber { get; set; }

        public string versionId { get; set; }

        public string odsCode { get; set; }

        public string phoneNumber { get; set; }

        public string addressLine { get; set; }

        public string city { get; set; }

        public string district { get; set; }

        public string postalCode { get; set; }

        public bool currentStatus { get; set; } 
    }
}
