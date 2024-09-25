namespace GP_Connect.DataTransferObject
{
    public class ProblemAndIssueDTO
    {
        public string entry {  get; set; }

        public string significance { get; set; }

        public string details { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }   

        public string status { get; set; }
    }
}
