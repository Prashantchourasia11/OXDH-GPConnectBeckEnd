namespace GP_Connect.DataTransferObject
{
    public class ImmunizationDTO
    {
        public DateTime recDate { get; set; }

        public string vaccinationName { get; set; } 

        public string Part { get; set; }

        public string Content { get; set; }

        public string Details { get; set; }

        public int recordedDay { get; set; }

        public int recordedMonth { get; set; }

        public int recordedYear { get; set; }

        public string recDateType { get; set; }


    }
}
