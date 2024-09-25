namespace GP_Connect.DataTransferObject
{
    public class ResponseDocumentBase64
    {
        public string resourceType { get; set; }

        public string id { get; set; }

        public string contentType { get; set; }

        public string content { get; set; }
    }
}
