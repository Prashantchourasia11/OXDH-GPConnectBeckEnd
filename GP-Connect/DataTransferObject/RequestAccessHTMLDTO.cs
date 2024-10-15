namespace GP_Connect.DataTransferObject
{
 
    public class RequestAccessHTMLDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class RequestAccessHTMLDTOParameter
    {
        public string name { get; set; }
        public RequestAccessHTMLDTOValueIdentifier valueIdentifier { get; set; }
        public RequestAccessHTMLDTOValueCodeableConcept valueCodeableConcept { get; set; }

        public RequestAccessHTMLDTOValuevaluePeriod valuePeriod { get; set; }
    }

    public class RequestAccessHTMLDTO
    {
        public string resourceType { get; set; }
        public List<RequestAccessHTMLDTOParameter> parameter { get; set; }
    }

    public class RequestAccessHTMLDTOValueCodeableConcept
    {
        public List<RequestAccessHTMLDTOCoding> coding { get; set; }
    }

    public class RequestAccessHTMLDTOValueIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }
    public class RequestAccessHTMLDTOValuevaluePeriod
    {
        public string start { get; set; }
        public string end { get; set; }
    }
}
