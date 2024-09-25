namespace GP_Connect.DataTransferObject
{
    public class PractionerDTO
    {
        public string resourceId { get; set; }

        public DateTime lastUpdated { get; set; }

        public string sequenceNumber { get; set; }

        public string versionId { get; set; }

        public DateTime modifiedDate { get; set; }

        public string languageCode { get; set; }

        public string languageDisplay { get; set; }
        public string sdsId { get; set; }

        public string family { get; set; }

        public string given { get; set; }

        public string prefix { get; set; }

        public string gender { get; set; }

        public List<practitionerLanguageDTO> practitionerLanguages { get; set; }

        public string modeOfCommunication { get; set; }

        public string interpreterRequired { get; set; }

        public string communucationProficiency { get; set; }

        public bool currentStatus { get; set; }

        public List<string> JobRoles { get; set; }

    }
    public class practitionerLanguageDTO
    {
        public string languageName { get; set; }
        public string languageCode { get; set; }

    }
}
