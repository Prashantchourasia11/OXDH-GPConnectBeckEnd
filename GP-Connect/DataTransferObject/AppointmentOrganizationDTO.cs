namespace GP_Connect.DataTransferObject
{
   
    public class AppointmentOrganizationAddress
    {
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string postalCode { get; set; }
    }

    public class AppointmentOrganizationIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class AppointmentOrganizationMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class AppointmentOrganizationResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentOrganizationMeta meta { get; set; }
        public List<AppointmentOrganizationIdentifier> identifier { get; set; }
        public string name { get; set; }
        public AppointmentOrganizationAddress address { get; set; }
        public AppointmentOrganizationTelecom telecom { get; set; }
    }

    public class AppointmentOrganizationDTO
    {
        public AppointmentOrganizationResource resource { get; set; }
    }

    public class AppointmentOrganizationTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }
}
