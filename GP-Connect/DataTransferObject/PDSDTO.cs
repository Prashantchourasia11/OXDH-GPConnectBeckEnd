namespace GP_Connect.DataTransferObject
{
    
    public class PDSDTOAddress
    {
        public string id { get; set; }
        public List<string> line { get; set; }
        public PDSDTOPeriod period { get; set; }
        public string postalCode { get; set; }
        public string use { get; set; }
    }

    public class PDSDTOCoding
    {
        public string code { get; set; }
        public string display { get; set; }
        public string system { get; set; }
        public string version { get; set; }
    }

    public class PDSDTOExtension
    {
        public string url { get; set; }
        public PDSDTOValueReference valueReference { get; set; }
        public List<PDSDTOExtension> extension { get; set; }
        public PDSDTOValueCodeableConcept valueCodeableConcept { get; set; }
        public bool? valueBoolean { get; set; }
    }

    public class PDSDTOGeneralPractitioner
    {
        public string id { get; set; }
        public PDSDTOIdentifier identifier { get; set; }
        public string type { get; set; }
    }

    public class PDSDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
        public PDSDTOPeriod period { get; set; }
        public List<PDSDTOExtension> extension { get; set; }
    }

    public class PDSDTOMeta
    {
        public List<PDSDTOSecurity> security { get; set; }
        public string versionId { get; set; }
    }

    public class PDSDTOName
    {
        public string family { get; set; }
        public List<string> given { get; set; }
        public string id { get; set; }
        public PDSDTOPeriod period { get; set; }
        public List<string> prefix { get; set; }
        public string use { get; set; }
    }

    public class PDSDTOPeriod
    {
        public string start { get; set; }
    }

    public class PDSDTO
    {
        public List<PDSDTOAddress> address { get; set; }
        public string birthDate { get; set; }
        public List<PDSDTOExtension> extension { get; set; }

        public DateTime deceasedDateTime { get; set; }
        public string gender { get; set; }
        public List<PDSDTOGeneralPractitioner> generalPractitioner { get; set; }
        public string id { get; set; }
        public List<PDSDTOIdentifier> identifier { get; set; }
        public PDSDTOMeta meta { get; set; }
        public List<PDSDTOName> name { get; set; }
        public string resourceType { get; set; }
        public List<PDSDTOTelecom> telecom { get; set; }
    }

    public class PDSDTOSecurity
    {
        public string code { get; set; }
        public string display { get; set; }
        public string system { get; set; }
    }

    public class PDSDTOTelecom
    {
        public string id { get; set; }
        public PDSDTOPeriod period { get; set; }
        public string system { get; set; }
        public string use { get; set; }
        public string value { get; set; }
    }

    public class PDSDTOValueCodeableConcept
    {
        public List<PDSDTOCoding> coding { get; set; }
    }

    public class PDSDTOValueReference
    {
        public PDSDTOIdentifier identifier { get; set; }
    }

}
