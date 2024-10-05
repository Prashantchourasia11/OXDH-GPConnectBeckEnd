namespace GP_Connect.DataTransferObject
{
    public class AppointmentByIdDTO
    {
        public string appointmnetId { get; set; }

        public string versionNumber { get; set; }
        public string organizationId { get; set; }
        public string odsCode { get; set; }
        public string organizationType { get; set; }
        public string organizationName { get; set; }
        public string PhoneNumber { get; set; }
        public string PractitionerRoleCode { get; set; }
        public string PractionerDisplayRole { get; set; }

        public string DeleiveryChannel { get; set; }
        public string Status { get; set; }
        public string ServiceCategory { get; set; }
        public string Description { get; set; }
        public string SlotReference { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string comments { get; set; }
        public string patientReference { get; set; }

        public string locationReference { get; set; }
        public string PractionerReference { get; set; }

        public string CancellationReson {  get; set; }  

    }
}
