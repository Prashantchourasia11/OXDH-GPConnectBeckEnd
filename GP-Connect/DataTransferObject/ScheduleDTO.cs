namespace GP_Connect.DataTransferObject
{
   
    public class ScheduleDTOActor
    {
        public string reference { get; set; }
    }

    public class ScheduleDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class ScheduleDTOExtension
    {
        public string url { get; set; }
        public ScheduleDTOValueCodeableConcept valueCodeableConcept { get; set; }
    }

    public class ScheduleDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }

    public class ScheduleDTOPlanningHorizon
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }

    public class ScheduleDTOResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public ScheduleDTOMeta meta { get; set; }
        public List<ScheduleDTOExtension> extension { get; set; }
        public ScheduleDTOServiceCategory serviceCategory { get; set; }
        public List<ScheduleDTOActor> actor { get; set; }
        public ScheduleDTOPlanningHorizon planningHorizon { get; set; }
    }

    public class ScheduleDTO
    {
        public ScheduleDTOResource resource { get; set; }
    }

    public class ScheduleDTOServiceCategory
    {
        public string text { get; set; }
    }

    public class ScheduleDTOValueCodeableConcept
    {
        public List<ScheduleDTOCoding> coding { get; set; }
    }

}
