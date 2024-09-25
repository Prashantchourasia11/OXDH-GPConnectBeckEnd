using GP_Connect.DataTransferObject;

namespace GP_Connect.Service.AccessStructureRecord
{
    public interface IServiceAccessStructureRecord
    {
        List<object> GetAccessStructureRecord(RequestAccessStructureRecordDTO patientDetailsRequest);
    }
}
