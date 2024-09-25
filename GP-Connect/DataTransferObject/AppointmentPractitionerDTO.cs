namespace GP_Connect.DataTransferObject
{

    public class PractitionerDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class PractitionerDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class PractitionerDTOName
    {
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }

    public class PractitionerDTOResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public PractitionerDTOMeta meta { get; set; }
        public List<PractitionerDTOIdentifier> identifier { get; set; }
        public List<PractitionerDTOName> name { get; set; }
        public string gender { get; set; }
    }

    public class AppointmentPractitionerDTO
    {
        public PractitionerDTOResource resource { get; set; }
    }

}
