namespace GP_Connect.DataTransferObject
{
    public class ResponseAccessHTML
    {
        public string resourceType { get; set; }

        public string id { get; set; }

        public string type { get; set; }

        public List<object> entry { get;set; }
    }
}
