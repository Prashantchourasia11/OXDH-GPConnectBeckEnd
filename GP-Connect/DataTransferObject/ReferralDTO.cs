namespace GP_Connect.DataTransferObject
{
    public class ReferralDTO
    {
        public DateTime recDate { get; set; }

        public string fromdoctor { get; set; }

        public string toDoctor { get; set; }    

        public string priority { get; set; }

        public string description { get; set; }

        public string confidential { get; set; }

        public int recordedDay { get; set; }

        public int recordedMonth { get; set; }

        public int recordedYear { get; set; }

        public string recDateType { get; set; }



    }
}
