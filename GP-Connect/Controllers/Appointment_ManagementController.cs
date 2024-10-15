using GP_Connect.DataTransferObject;
using GP_Connect.Service.AppointmentManagement;
using GP_Connect.Service.Foundation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GP_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Appointment_ManagementController : ControllerBase
    {
        #region Properties


        #endregion

        #region Constructor
        ServiceAppointmentManagement sam = new ServiceAppointmentManagement();
        #endregion

        #region Method

        /// <summary>
        /// Read Patient Appointment
        /// </summary>

        [HttpGet]
        [Route("Patient/Appointment")]
        public ActionResult Patient(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "identifier")][Required] string id = "1000",
             [FromQuery(Name = "start")][Required] string start = "ge2024-05-01",
             [FromQuery(Name = "starts")][Required] string end = "le2024-05-10"
             )
        {
            try
            {
                var res = sam.GetAppointmentGetByPatientId(id, start, end,SspInterctionId,Authorization);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Search for free slots
       /// </summary>

        [HttpGet]
        [Route("Slot")]
        public ActionResult Slot(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "Start ")][Required] string Start = "ge2024-08-20",
             [FromQuery(Name = "End")][Required] string End = "le2024-08-30",
             [FromQuery(Name = "Status ")][Required] string Status = "free",
             [FromQuery(Name = "_Include ")][Required] string _Include = "Slot:schedule",
             [FromQuery(Name = "_include:recurse")] string _includerecurse = "--",
             [FromQuery(Name = "searchFilter")] string searchFilter = "--"
             )
          {
            try
            {
               
                var res = sam.GetFreeSlot(Start, End, Status, _Include,"","","");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        ///  Book an appointment
        /// </summary>

        [HttpPost]
        [Route("BookAnAppointment")]
        public ActionResult BookAnAppointment(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromBody] dynamic BookAppointmentDTO = null
             )
           {
            try
            {
                var appointmentDetails = JsonConvert.DeserializeObject<RequestBookAppointmentDTO>(BookAppointmentDTO.ToString());
                var result = sam.BookAnAppointment(appointmentDetails, BookAppointmentDTO.ToString(),"");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Read an appointment
        /// </summary>

        [HttpGet]
        [Route("ReadAnAppoinbtment")]
        public ActionResult ReadAnAppoinbtment(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "id")][Required] string id = "1"
             )
        {
            try
            {
                var result = sam.ReadAnAppointment(id,"External");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Amend or cancel appointment
        /// </summary>

        [HttpPut]
        [Route("AmendOrCancelAppointment")]
        public ActionResult AmendOrCancelAppointment(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:update:appointment-1",
             [FromHeader(Name = "If-Match")][Required] string IfMatch = "W/1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "id")][Required] string identifier = "1005",
             [FromBody] dynamic BookAppointmentDTO = null
             )
        {
            try
            {
                var appointmentDetails = JsonConvert.DeserializeObject<RequestBookAppointmentDTO>(BookAppointmentDTO.ToString());
                if(identifier != appointmentDetails.id)
                {
                    return StatusCode(422, GetInvalidResourceJSON());
                }

                if (SspInterctionId == "urn:nhs:names:services:gpconnect:fhir:rest:update:appointment-1")
                {
                    var result = sam.UpdateAppointment(IfMatch, appointmentDetails, SspInterctionId, BookAppointmentDTO.ToString());
                    if(result == null)
                    {
                        return StatusCode(422, GetInvalidResourceJSON());
                    }
                    return Ok(result);
                }
                else if(SspInterctionId == "urn:nhs:names:services:gpconnect:fhir:rest:cancel:appointment-1")
                {
                    var result = sam.CancelAppointment(appointmentDetails,"");
                    if (result == null)
                    {
                        return StatusCode(422, GetInvalidResourceJSON());
                    }
                    return Ok(result);
                }

                 return StatusCode(422, GetInvalidResourceJSON());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Internal Method

        internal Object GetInvalidResourceJSON()
        {
            var dictionary = new Dictionary<string, object>
           {
            { "resourceType", "OperationOutcome" },
            { "meta", new Dictionary<string, object>
                {
                    { "profile", new List<string> { "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1" } }
                }
            },
            { "issue", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "severity", "error" },
                        { "code", "value" },
                        { "details", new Dictionary<string, object>
                            {
                                { "coding", new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            { "system", "https://fhir.nhs.uk/STU3/ValueSet/Spine-ErrorOrWarningCode-1" },
                                            { "code", "invalid" },
                                            { "display", "INVALID_RESOURCE" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
            return dictionary;
        }

        #endregion
    }
}
