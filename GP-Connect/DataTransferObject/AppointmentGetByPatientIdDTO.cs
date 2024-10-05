namespace GP_Connect.DataTransferObject
{
    public class AppointmentGetByPatientIdDTO
    {
        public string resourceType { get; set; }

        public string type { get; set; }

        public AppointmentGetByReverseDTOMeta1 meta { get; set; }

        public dynamic entry { get; set; }

    }
    public class AppointmentGetByReverseDTOMeta1
    {
        public string lastUpdated { get; set; }
        
    }
}
