namespace GP_Connect.DataTransferObject
{
 
    public class ResponseBookAppointmentDTOActor
    {
        public string reference { get; set; }
    }

    public class ResponseBookAppointmentDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class ResponseBookAppointmentDTOContained
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public ResponseBookAppointmentDTOMeta meta { get; set; }
        public List<ResponseBookAppointmentDTOIdentifier> identifier { get; set; }
        public string name { get; set; }
        public List<ResponseBookAppointmentDTOTelecom> telecom { get; set; }
    }

    public class ResponseBookAppointmentDTOExtension
    {
        public string url { get; set; }
        public ResponseBookAppointmentDTOValueReference valueReference { get; set; }
        public ResponseBookAppointmentDTOValueCodeableConcept valueCodeableConcept { get; set; }
        public string valueCode { get; set; }
    }

    public class ResponseBookAppointmentDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class ResponseBookAppointmentDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class ResponseBookAppointmentDTOParticipant
    {
        public ResponseBookAppointmentDTOActor actor { get; set; }
        public string status { get; set; }
    }

    public class ResponseBookAppointmentDTO
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public ResponseBookAppointmentDTOMeta meta { get; set; }
        public List<ResponseBookAppointmentDTOContained> contained { get; set; }
        public List<ResponseBookAppointmentDTOExtension> extension { get; set; }
        public string status { get; set; }
        public ResponseBookAppointmentDTOServiceCategory serviceCategory { get; set; }
        public List<ResponseBookAppointmentDTOServiceType> serviceType { get; set; }
        public string description { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public List<ResponseBookAppointmentDTOSlot> slot { get; set; }
        public DateTime created { get; set; }
        public string comment { get; set; }
        public List<ResponseBookAppointmentDTOParticipant> participant { get; set; }
    }

    public class ResponseBookAppointmentDTOServiceCategory
    {
        public string text { get; set; }
    }

    public class ResponseBookAppointmentDTOServiceType
    {
        public string text { get; set; }
    }

    public class ResponseBookAppointmentDTOSlot
    {
        public string reference { get; set; }
    }

    public class ResponseBookAppointmentDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class ResponseBookAppointmentDTOValueCodeableConcept
    {
        public List<ResponseBookAppointmentDTOCoding> coding { get; set; }
    }

    public class ResponseBookAppointmentDTOValueReference
    {
        public string reference { get; set; }
    }

}
