namespace GP_Connect.DataTransferObject
{
  
    public class AppointmentLocationDTOAddress
    {
        public List<string> line { get; set; }
        public string postalCode { get; set; }
    }

    public class AppointmentLocationDTOManagingOrganization
    {
        public string reference { get; set; }
    }

    public class AppointmentLocationDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class AppointmentLocationDTOResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentLocationDTOMeta meta { get; set; }
        public string name { get; set; }
        public AppointmentLocationDTOAddress address { get; set; }
        public AppointmentLocationDTOTelecom telecom { get; set; }
        public AppointmentLocationDTOManagingOrganization managingOrganization { get; set; }
    }

    public class AppointmentLocationDTO
    {
        public AppointmentLocationDTOResource resource { get; set; }
    }

    public class AppointmentLocationDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

}
