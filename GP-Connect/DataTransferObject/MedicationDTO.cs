﻿namespace GP_Connect.DataTransferObject
{
    public class MedicationDTO
    {
        public string type { get; set; }

        public DateTime startDate { get; set; } 

        public string MedicationItem { get; set; }

        public string DosageInstruction { get; set; }   

        public string Quantity { get; set; }    

        public DateTime endDate { get; set; }

        public string DaysDuration { get; set; }

        public string AdditionalInformation { get; set; }
        public DateTime LastIssuedDate { get; set; }
        public string NumberOfPrescriptionIsuued { get; set; }

        public string MaxIssues { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime DiscountinuedDate { get; set; }
        public string DiscountinuedReason { get; set; }

        public string ControlledDrug { get; set; }
    }
}
