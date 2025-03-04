using GP_Connect.DataTransferObject;
using static GP_Connect.Logger.EventLogger;

namespace GP_Connect.Logger
{
    public interface IEventLogger
    {
        Guid  AuditEventType(RequestAccessHTMLDTO request, dynamic response,string sspTraceId, string authoizations, string sspFrom , string sspTo , string s, string responseCode);
        Guid CreateAudit(string keyword, string request, string response, string sspTraceId, string authoizations,string nhsNumber, string sspFrom, string sspTo, string sspInteractionId, string responseCode);
   
    }
}
