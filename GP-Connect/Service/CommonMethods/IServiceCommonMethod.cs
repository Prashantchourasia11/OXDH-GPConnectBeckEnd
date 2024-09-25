namespace GP_Connect.Service.CommonMethods
{
    public interface IServiceCommonMethod
    {
        List<object> GetAllDetailsOfPatientByNHSnumber(string nhsNumber);

        dynamic GetAllDetailsOfPatientByPatientIdUsedForDocument(string nhsNumber);

        List<object> GetAllDetailsOfPatientByPatientIdUsedForHTMLACCESS(string nhsNumber);
    }
}
