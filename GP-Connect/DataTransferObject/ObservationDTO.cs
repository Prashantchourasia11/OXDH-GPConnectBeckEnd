namespace GP_Connect.DataTransferObject
{
    public class ObservationDTO
    {
        public DateTime recDate { get; set; } 

        public string title { get; set; }

        public string valueString { get; set; }

        public string range { get; set; }

        public string details { get; set; }

        public int recordedDay { get; set; }

        public int recordedMonth { get; set; }

        public int recordedYear { get; set; }

        public string recDateType { get; set; }

        public string confidential { get; set; }

    }
}
