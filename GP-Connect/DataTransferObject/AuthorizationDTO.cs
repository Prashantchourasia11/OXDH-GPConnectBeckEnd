namespace GP_Connect.DataTransferObject
{
    public class Identifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class Name
    {
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }

    public class RequestingDevice
    {
        public string resourceType { get; set; }
        public List<Identifier> identifier { get; set; }
        public string model { get; set; }
        public string version { get; set; }
    }

    public class RequestingOrganization
    {
        public string resourceType { get; set; }
        public List<Identifier> identifier { get; set; }
        public string name { get; set; }
    }

    public class RequestingPractitioner
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public List<Identifier> identifier { get; set; }
        public List<Name> name { get; set; }
    }

    public class AuthorizationDTO
    {
        public string iss { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public string exp { get; set; }
        public string iat { get; set; }
        public string reason_for_request { get; set; }
        public string requested_scope { get; set; }
        public RequestingDevice requesting_device { get; set; }
        public RequestingOrganization requesting_organization { get; set; }
        public RequestingPractitioner requesting_practitioner { get; set; }
    }
}
