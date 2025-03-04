namespace GP_Connect.DataTransferObject
{
    public class EncounterDetailsDTO
    {
        public DateTime Date { get; set; }

        public string title { get; set; }   

        public string details { get; set; } 

        public string confidential { get; set; }

        public int encounterDay { get; set; }

        public int encounterMonth { get; set; }

        public int encounterYear { get; set; }

        public string encounterDateType { get; set; }


    }
}
