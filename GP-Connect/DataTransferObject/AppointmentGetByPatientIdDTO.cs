namespace GP_Connect.DataTransferObject
{
    public class AppointmentGetByPatientIdDTO
    {
        public string resourceType { get; set; }

        public string type { get; set; }

        public List<AppointmentGetByReverseDTO> entry { get; set; }

    }
}
