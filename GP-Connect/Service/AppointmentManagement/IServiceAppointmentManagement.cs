using GP_Connect.DataTransferObject;
using Microsoft.Crm.Sdk.Messages;

namespace GP_Connect.Service.AppointmentManagement
{
    public interface IServiceAppointmentManagement
    {
        FinalJSONofSearchSlotDTO1 GetFreeSlot(string fromDate, string toDate, string status, string _include);

        AppointmentGetByReverseDTO BookAnAppointment(RequestBookAppointmentDTO bookAppointment);

        AppointmentGetByReverseDTO ReadAnAppoinbtment(string appoinbtmentId);

        AppointmentGetByPatientIdDTO GetAppointmentGetByPatientId(string patientId, string fromDate, string toDate);

        AppointmentGetByReverseDTO UpdateAppointment(string versionId,RequestBookAppointmentDTO bookAppointment);

        AppointmentGetByReverseDTO CancelAppointment(string versionId, RequestBookAppointmentDTO bookAppointment);
    }
}
