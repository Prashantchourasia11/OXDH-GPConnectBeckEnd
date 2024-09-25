using GP_Connect.DataTransferObject;

namespace GP_Connect.Service.Foundation
{
    public interface IServiceFoundation
    {
        string FindAPatient(string NHSNumber);

        string ReadAPatient(string id);

        string FindAPractioner(string sdsId);

        string ReadAPractioner(string id);

        string FindAOrganization(string odsCode);

        string ReadAOrganization(string id);

        string ReadALocation(string id);

        string CreatePatientRecord(RegisterPatientDTO patientDetails);
    }
}
