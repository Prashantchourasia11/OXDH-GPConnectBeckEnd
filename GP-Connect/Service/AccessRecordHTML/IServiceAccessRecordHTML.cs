using GP_Connect.DataTransferObject;

namespace GP_Connect.Service.AccessRecordHTML
{
    public interface IServiceAccessRecordHTML
    {
        dynamic GetAccessHTMLRecord(RequestAccessHTMLDTO htmlDetails);

    }
}
