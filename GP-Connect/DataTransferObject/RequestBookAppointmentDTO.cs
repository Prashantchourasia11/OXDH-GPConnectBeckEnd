namespace GP_Connect.DataTransferObject
{
    public class RequestBookAppointmentDTOActor
    {
        public string reference { get; set; }
    }

    public class RequestBookAppointmentDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class RequestBookAppointmentDTOContained
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public RequestBookAppointmentDTOMeta meta { get; set; }
        public List<RequestBookAppointmentDTOIdentifier> identifier { get; set; }
        public List<RequestBookAppointmentDTOType> type { get; set; }
        public string name { get; set; }
        public List<RequestBookAppointmentDTOTelecom> telecom { get; set; }
    }

    public class RequestBookAppointmentDTOExtension
    {
        public string url { get; set; }
        public RequestBookAppointmentDTOValueReference valueReference { get; set; }
        public string valueString { get; set; }
    }

    public class RequestBookAppointmentDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class RequestBookAppointmentDTOMeta
    {
        public List<string> profile { get; set; }
    }

    public class RequestBookAppointmentDTOParticipant
    {
        public RequestBookAppointmentDTOActor actor { get; set; }
        public string status { get; set; }
    }

    public class RequestBookAppointmentDTO
    {
        public string resourceType { get; set; }
        public RequestBookAppointmentDTOMeta meta { get; set; }
        public List<RequestBookAppointmentDTOContained> contained { get; set; }
        public List<RequestBookAppointmentDTOExtension> extension { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public List<RequestBookAppointmentDTOSlot> slot { get; set; }
        public DateTime created { get; set; }
        public string comment { get; set; }
        public List<RequestBookAppointmentDTOParticipant> participant { get; set; }
        public string id { get; set; }
    }

    public class RequestBookAppointmentDTOSlot
    {
        public string reference { get; set; }
    }

    public class RequestBookAppointmentDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class RequestBookAppointmentDTOType
    {
        public List<RequestBookAppointmentDTOCoding> coding { get; set; }
    }

    public class RequestBookAppointmentDTOValueReference
    {
        public string reference { get; set; }
    }


}
