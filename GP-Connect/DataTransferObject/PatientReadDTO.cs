namespace GP_Connect.DataTransferObject
{
    public class PatientReadDTOAddress
    {
        public string use { get; set; }
        public string type { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string postalCode { get; set; }
    }

    public class PatientReadDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class PatientReadDTOContact
    {
        public List<PatientReadDTORelationship> relationship { get; set; }
        public PatientReadDTOName name { get; set; }
        public List<PatientReadDTOTelecom> telecom { get; set; }
        public PatientReadDTOAddress address { get; set; }
        public string gender { get; set; }
    }

    public class PatientReadDTOEntry
    {
        public PatientReadDTOResource resource { get; set; }
    }

    public class PatientReadDTOExtension
    {
        public string url { get; set; }
        public List<PatientReadDTOExtension> extension { get; set; }
        public PatientReadDTOValuePeriod valuePeriod { get; set; }
        public PatientReadDTOValueReference valueReference { get; set; }
        public PatientReadDTOValueCodeableConcept valueCodeableConcept { get; set; }
        public bool? valueBoolean { get; set; }
    }

    public class PatientReadDTOGeneralPractitioner
    {
        public string reference { get; set; }
    }

    public class PatientReadDTOIdentifier
    {
        public List<PatientReadDTOExtension> extension { get; set; }
        public string system { get; set; }
        public string value { get; set; }
    }

    public class PatientReadDTOManagingOrganization
    {
        public string reference { get; set; }
    }

    public class PatientReadDTOMeta
    {
        public DateTime lastUpdated { get; set; }
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class PatientReadDTOName
    {
        public string use { get; set; }
        public string text { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }

    public class PatientReadDTORelationship
    {
        public string text { get; set; }
    }

    public class PatientReadDTOResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public PatientReadDTOMeta meta { get; set; }
        public List<PatientReadDTOExtension> extension { get; set; }
        public List<PatientReadDTOIdentifier> identifier { get; set; }
        public bool active { get; set; }
        public List<PatientReadDTOName> name { get; set; }
        public List<PatientReadDTOTelecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public DateTime deceasedDateTime { get; set; }
        public List<PatientReadDTOAddress> address { get; set; }
        public List<PatientReadDTOContact> contact { get; set; }
        public List<PatientReadDTOGeneralPractitioner> generalPractitioner { get; set; }
        public PatientReadDTOManagingOrganization managingOrganization { get; set; }
    }

    public class PatientReadDTO
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public PatientReadDTOMeta meta { get; set; }
        public string type { get; set; }
        public List<PatientReadDTOEntry> entry { get; set; }
    }

    public class PatientReadDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

    public class PatientReadDTOValueCodeableConcept
    {
        public List<PatientReadDTOCoding> coding { get; set; }
    }

    public class PatientReadDTOValuePeriod
    {
        public DateTime start { get; set; }
    }

    public class PatientReadDTOValueReference
    {
        public string reference { get; set; }
    }

}
