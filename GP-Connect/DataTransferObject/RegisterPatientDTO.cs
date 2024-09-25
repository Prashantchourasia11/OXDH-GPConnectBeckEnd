namespace GP_Connect.DataTransferObject
{
    public class RegisterPatientDTOAddress
    {
        public string use { get; set; }
        public string type { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string postalCode { get; set; }
    }

    public class RegisterPatientDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class RegisterPatientDTOExtension
    {
        public string url { get; set; }
        public List<RegisterPatientDTOExtension> extension { get; set; }
        public RegisterPatientDTOValueCodeableConcept valueCodeableConcept { get; set; }
        public bool? valueBoolean { get; set; }
    }

    public class RegisterPatientDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class RegisterPatientDTOMeta
    {
        public List<string> profile { get; set; }
    }

    public class RegisterPatientDTOName
    {
        public string use { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }

    public class RegisterPatientDTOParameter
    {
        public string name { get; set; }
        public RegisterPatientDTOResource resource { get; set; }
    }

    public class RegisterPatientDTOResource
    {
        public string resourceType { get; set; }
        public RegisterPatientDTOMeta meta { get; set; }
        public List<RegisterPatientDTOIdentifier> identifier { get; set; }
        public List<RegisterPatientDTOName> name { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public List<RegisterPatientDTOTelecom> telecom { get; set; }
        public List<RegisterPatientDTOAddress> address { get; set; }
        public List<RegisterPatientDTOExtension> extension { get; set; }
    }

    public class RegisterPatientDTO
    {
        public string resourceType { get; set; }
        public List<RegisterPatientDTOParameter> parameter { get; set; }
    }

    public class RegisterPatientDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }

    public class RegisterPatientDTOValueCodeableConcept
    {
        public List<RegisterPatientDTOCoding> coding { get; set; }
    }

    public class RegistractionTelecomCheckerDTO
    {
        public string use { get; set; }
        public string system { get; set; }
    }
}
