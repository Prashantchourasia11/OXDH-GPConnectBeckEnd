namespace GP_Connect.DataTransferObject
{
   public class DocumentReferenceDTO
    {
        public string SequenceId { get; set;}

        public string masterIdentifierCRMGuid { get; set;}

        public string status { get; set;}

        public string documentCode { get; set;} 

        public string documentDisplay { get; set;}

        public string patientSequenceNumber { get; set;}    

        public DateTime createdOn { get; set;}

        public DateTime inexed { get; set;}

        public string practitionerSequenceNumber { get; set;}   

        public string description { get; set;}  

        public string contentType { get; set;} 

        public string fileUrl { get; set;}  

        public string size { get; set;}

        public string serviceCode { get; set;}

        public string serviceDisplay { get; set;}

        public string encounterNumber { get; set;}

    }

}
