namespace GP_Connect.DataTransferObject
{
    public class AllerguStartEndDetailsDTO
    {
        public string allergyName { get; set; }

        public string allergyStatus { get; set; }   

        public DateTime recDate { get; set; } 

        public DateTime recEndDate { get; set; } 

        public string confidential {  get; set; }


        public int recordedDay { get; set; }

        public int recordedMonth { get; set; }

        public int recordedYear { get; set; }

        public string recDateType { get; set; }

        public int recordedEndDay { get; set; }

        public int recordedEndMonth { get; set; }

        public int recordedEndYear { get; set; }

        public string recEndDateType { get; set; }



    }
}
