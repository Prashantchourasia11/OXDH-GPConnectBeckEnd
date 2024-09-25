using GP_Connect.DataTransferObject;

namespace GP_Connect.Service.AccessDocument
{
    public interface IServiceAccessDocument
    {
        BundleResponseDTO GetDocumentReference(string patientId , string Createdstart , string CreatedEnd , string author , string description); 

        ResponseDocumentBase64 GetBase64UsingCRMGuid(string crmGuid);
    }
}
