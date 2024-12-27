using GP_Connect.DataTransferObject;
using GP_Connect.Service.AccessStructureRecord;
using GP_Connect.Service.Foundation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GP_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessStructureRecordController : ControllerBase
    {
        #region Properties


        #endregion

        #region Constructor

        ServiceAccessStructureRecord service = new ServiceAccessStructureRecord();

        #endregion

        #region Method

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
     [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
     [FromQuery(Name = "identifier")][Required] string identifier = "https://fhir.nhs.uk/Id/nhs-number|9658218873"
     )
        {
            try
            {
           

                var nhs_number = identifier.Substring(identifier.Length - Math.Min(10, identifier.Length));
                //ServiceFoundation serviceFoundation = new ServiceFoundation();
                //var result = serviceFoundation.FindAPatient(nhs_number,"");
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        /// Retrieve patient's structured record
        /// </summary>

        [HttpPost]
        [Route("/Patient/$gpc.getstructuredrecord")]
        public ActionResult getstructuredrecord(
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
                var bodyResponse = JsonConvert.DeserializeObject<RequestAccessStructureRecordDTO>(body.ToString());
                var response = service.GetAccessStructureRecord(bodyResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/Patient/$gpc.migratestructuredrecord")]
        public ActionResult migratestructuredrecord(
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
                var bodyResponse = JsonConvert.DeserializeObject<RequestAccessStructureRecordDTO>(body.ToString());
                var response = service.GetAccessStructureRecord(bodyResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("checking")]
        public ActionResult Checking([FromBody] object body)
        {
            try
            {
                // Convert body to string
                string bodyString = body.ToString();

                // Parse FHIR resource
                var jsonParser = new FhirJsonParser();
                var v1 = jsonParser.Parse<Resource>(bodyString);

                return Ok(v1); // Optionally return the parsed resource
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #endregion
    }
    public class getdataDTO
    {
        public dynamic body { get; set; }
    }
}
