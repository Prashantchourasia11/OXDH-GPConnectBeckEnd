using GP_Connect.DataTransferObject;
using GP_Connect.Service.AccessDocument;
using GP_Connect.Service.AppointmentManagement;
using GP_Connect.Service.Foundation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GP_Connect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessDocumentController : ControllerBase
    {
        #region Properties


        #endregion

        #region Constructor
        ServiceAccessDocument sam = new ServiceAccessDocument();
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
        /// Find a patient
        /// </summary>

        [HttpGet]
        [Route("Patient")]
        public ActionResult Patient(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "identifier")][Required] string identifier = "https://fhir.nhs.uk/Id/nhs-number|9728338449"
             )
           {
            try
            {
                var nhsNumber = "";
                if (identifier.Contains('|'))
                {
                    nhsNumber = identifier.Split('|')[1];
                }

                var fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                var queryParams = QueryHelpers.ParseQuery(Request.QueryString.Value);
                int identifierCount = queryParams.ContainsKey("identifier") ? queryParams["identifier"].Count : 0;


                ServiceFoundation sf = new ServiceFoundation();
                var result = sf.FindAPatient(nhsNumber, "", identifier, identifierCount, fullUrl, SspTraceId);
                Response.Headers.Add("Cache-Control", "no-store");

                if (result[2] == "InvalidIdentifier" || result[2] == "InvalidParameter")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 422
                    };
                }
                if (result[2] == "IdentifierNotPresent")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 422
                    };
                }
                if (result[2] == "MultipleIdentifier" || result[2] == "IdentifierMissing")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }
                try { Response.Headers.Add("ETag", "W/\"" + result[1] + "\""); } catch (Exception) { }

                return new JsonResult(result[0])
                {
                    ContentType = "application/fhir+json"
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Search for a patient's documents
        /// </summary>

        [HttpGet]
        [Route("DocumentReference")]
        public ActionResult DocumentReference(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:documents:fhir:rest:search:documentreference-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "id ")][Required] string id = "999",
             [FromQuery(Name = "_include")][Required] string _includes = "DocumentReference:subject:Patient",
             [FromQuery(Name = "_revinclude:recurse")][Required] string _revincluderecurse = "PractitionerRole:practitioner",
             [FromQuery(Name = "Created-start")][Required] string Createdstart = "ge2024-01-05",
              [FromQuery(Name = "Created-end")][Required] string CreatedEnd = "ge2024-10-05",
               [FromQuery(Name = "author")] string author = "docs",
                [FromQuery(Name = "description")] string description = "docs"
             )
         {
            try
            {
                
                var result = sam.GetDocumentReference(id, Createdstart , CreatedEnd , author , description, SspTraceId,"");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Retrieve a document
        /// </summary>

        [HttpGet]
        [Route("Binary")]
        public ActionResult Binary(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "id ")][Required] string id = "2"
             )
        {
            try
            {

                var result = sam.GetBase64UsingCRMGuid(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("MakeProperDate")]
        public ActionResult MakeProperDate(string inputDate, string type)
        {
            try
            {

                var result = ConvertToProperDateFormat(inputDate, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
  
        public static string ConvertToProperDateFormat(string inputDate, string type)
        {
            DateTime parsedDate;

            // Define the different input formats
                    string[] dateFormats = {
                    "yyyy",                      // Year only
                    "yyyy-MM",                   // Year and Month
                    "yyyy-MM-dd",                // Year, Month, and Day
                    "yyyy-MM-ddTHH",             // Year, Month, Day, and Hour
                    "yyyy-MM-ddTHH:mm",          // Year, Month, Day, Hour, and Minute
                    "yyyy-MM-ddTHH:mm:sszzz",    // Full date, time, and time zone offset (e.g., 2015-10-23T16:38:32+05:30)
                    "yyyy-MM-ddTHH:mm:ssZ",      // Full date and time in UTC (Z at the end)
                    "dd-MMM-yyyy",               // Day-Month-Year (e.g., 05-Jan-2025)
                    "MMM-yyyy"                   // Month-Year (e.g., Jan-2025)
                };

            // Attempt to parse the input date with the formats provided
            // Attempt to parse the input date with the formats provided
            if (DateTime.TryParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                if (type.ToLower() == "start")
                {
                    // Return the start of the provided date
                    if (inputDate.Length == 4) // Year only
                    {
                        return new DateTime(parsedDate.Year, 1, 1).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Contains("-") && inputDate.Split('-').Length == 2) // Month-Year (MMM-yyyy)
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, 1).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Contains("-") && inputDate.Split('-').Length == 3) // Full date with day
                    {
                        return parsedDate.ToString("yyyy-MM-dd");
                    }
                }
                else if (type.ToLower() == "end")
                {
                    // Return the end of the provided date
                    if (inputDate.Length == 4) // Year only
                    {
                        return new DateTime(parsedDate.Year, 12, 31).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Contains("-") && inputDate.Split('-').Length == 2) // Month-Year (MMM-yyyy)
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month)).ToString("yyyy-MM-dd");
                    }
                    else if (inputDate.Contains("-") && inputDate.Split('-').Length == 3) // Full date with day
                    {
                        return parsedDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            // If parsing fails, return "Invalid_date_format"
            return "Invalid_date_format";
        }
    




    #endregion


}
}
