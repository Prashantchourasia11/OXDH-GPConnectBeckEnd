namespace GP_Connect.DataTransferObject
{
    public class SlotDTOExtension
    {
        public string url { get; set; }
        public string valueCode { get; set; }
    }

    public class SlotDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class SlotDTOResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public SlotDTOMeta meta { get; set; }
        public List<SlotDTOExtension> extension { get; set; }
        public List<SlotDTOServiceType> serviceType { get; set; }
        public SlotDTOSchedule schedule { get; set; }
        public string status { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }

    public class SlotDTO
    {
        public SlotDTOResource resource { get; set; }
    }

    public class SlotDTOSchedule
    {
        public string reference { get; set; }
    }

    public class SlotDTOServiceType
    {
        public string text { get; set; }
    }
}
