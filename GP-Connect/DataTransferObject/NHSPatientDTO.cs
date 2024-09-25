using Nancy.Json;

namespace GP_Connect.DataTransferObject
{
    public class NHSPatientDTOAddress
    {
        public string use { get; set; }
        public string type { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string postalCode { get; set; }
    }

    public class NHSPatientDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class NHSPatientDTOExtension
    {
        public string url { get; set; }
        public List<NHSPatientDTOExtension> extension { get; set; }
        public NHSPatientDTOValuePeriod valuePeriod { get; set; }
        public NHSPatientDTOValueReference valueReference { get; set; }
        public NHSPatientDTOValueCodeableConcept valueCodeableConcept { get; set; }
        public bool? valueBoolean { get; set; }
    }

    public class NHSPatientDTOGeneralPractitioner
    {
        public string reference { get; set; }
    }

    public class NHSPatientDTOIdentifier
    {
        public List<NHSPatientDTOExtension> extension { get; set; }
        public string system { get; set; }
        public string value { get; set; }
    }

    public class NHSPatientDTOManagingOrganization
    {
        public string reference { get; set; }
    }

    public class NHSPatientDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class NHSPatientDTOName
    {
        public string use { get; set; }
        public string text { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }

    public class NHSPatientDTO
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public NHSPatientDTOMeta meta { get; set; }
        public List<Object> extension { get; set; }
        public List<NHSPatientDTOIdentifier> identifier { get; set; }
        public bool active { get; set; }
        public List<NHSPatientDTOName> name { get; set; }
        public List<NHSPatientDTOTelecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public List<NHSPatientDTOAddress> address { get; set; }
        public List<NHSPatientDTOGeneralPractitioner> generalPractitioner { get; set; }
        public NHSPatientDTOManagingOrganization managingOrganization { get; set; }
    }

    public class NHSPatientDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

    public class NHSPatientDTOValueCodeableConcept
    {
        public List<NHSPatientDTOCoding> coding { get; set; }
    }

    public class NHSPatientDTOValuePeriod
    {
        public DateTime start { get; set; }
    }

    public class NHSPatientDTOValueReference
    {
        public string reference { get; set; }
    }

    public class NHSPatientDTOExtentionfirst
    {
        public string url { get; set; }
        public List<object> extension { get; set; }
    }

}
