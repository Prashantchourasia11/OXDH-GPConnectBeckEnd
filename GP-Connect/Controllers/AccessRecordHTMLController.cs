using GP_Connect.DataTransferObject;
using GP_Connect.Service.AccessRecordHTML;
using GP_Connect.Service.AccessStructureRecord;
using GP_Connect.Service.Foundation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GP_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRecordHTMLController : ControllerBase
    {

        #region Properties


        #endregion

        #region Constructor

        ServiceAccessRecordHTML service = new ServiceAccessRecordHTML();
   

        #endregion


        /// <summary>
        ///  Get the FHIR® conformance profile
        /// </summary>

        [HttpGet]
        [Route("Metadata")]
        public ActionResult Metadata(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM"
     
             )
        {
            try
            {
           
                return Ok("");

               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieve a care record section
        /// </summary>

        [HttpPost]
        [Route("/Patient/$gpc.getcarerecord")]
        public ActionResult getcarerecord(
     [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
     [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
     [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
     [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
     [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
      [FromBody] dynamic body = null
     )
        {
            try
            {
                var bodyResponse = JsonConvert.DeserializeObject<RequestAccessHTMLDTO>(body.ToString());
                var response = service.GetAccessHTMLRecord(bodyResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
