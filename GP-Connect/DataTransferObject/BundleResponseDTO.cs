namespace GP_Connect.DataTransferObject
{
   
     

        public class BundleResponseDTOMeta
    {
            public List<string> profile { get; set; }
        }

        public class BundleResponseDTO
    {
            public string id { get; set; }
            public string resourceType { get; set; }
            public BundleResponseDTOMeta meta { get; set; }
            public string type { get; set; }
            public List<object> entry { get; set; }
        }




    
}
