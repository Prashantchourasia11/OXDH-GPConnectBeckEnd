namespace GP_Connect.DataTransferObject
{
  
        public class RequestAccessStructureRecordParameter
        {
            public string name { get; set; }
            public RequestAccessStructureRecordValueIdentifier valueIdentifier { get; set; }
            public List<RequestAccessStructureRecordPart> part { get; set; }
        }

        public class RequestAccessStructureRecordPart
        {
            public string name { get; set; }
            public bool valueBoolean { get; set; }
            public int? valueInteger { get; set; }
        }

        public class RequestAccessStructureRecordDTO
        {
            public string resourceType { get; set; }
            public List<RequestAccessStructureRecordParameter> parameter { get; set; }
        }

        public class RequestAccessStructureRecordValueIdentifier
    {
            public string system { get; set; }
            public string value { get; set; }
        }

    
}
