namespace GP_Connect.DataTransferObject
{
    public class ClinicalItemDTO
    {
        public string name { get; set; }

        public DateTime recDate { get; set; } 
        
        public string details { get; set; }

        public string confidential { get; set; }

        public int recordedDay { get; set; }

        public int recordedMonth { get; set; }

        public int recordedYear { get; set; }

        public string recDateType { get; set; }

    }
}
