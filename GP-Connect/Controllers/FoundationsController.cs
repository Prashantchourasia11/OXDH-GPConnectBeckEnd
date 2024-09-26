using AngleSharp.Dom;
using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.Service.AccessDocument;
using GP_Connect.Service.Foundation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using Swagger.Net.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace GP_Connect.Controllers
{
    [Route("W7M0I/STU3/1/gpconnect")]
    [ApiController]
    public class FoundationsController : ControllerBase
    {
        #region Properties

        ServiceFoundation serviceFoundation = new ServiceFoundation();
        ServiceAccessDocument serviceDocument = new ServiceAccessDocument();

        #endregion

        #region Constructor

        #endregion

        #region Method

        /// <summary>
        ///  Get the FHIR® conformance profile
        /// </summary>

        [HttpGet]
        [HttpDelete]
        [HttpPut]
        [HttpPost]
        [HttpPatch]
        [HttpOptions]
        [Route("metadata")]
        public ActionResult Metadata(
     [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
     [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
     [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
     [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
     [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM")
        {
            try
            {

                // hello
                if (HttpContext.Request.Method == "POST" || HttpContext.Request.Method == "PUT" || HttpContext.Request.Method == "DELETE" || HttpContext.Request.Method == "PATCH" || HttpContext.Request.Method == "OPTIONS")
                {
                    return new JsonResult(BadRequestJSON(HttpContext.Request.Method))
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }
                if(!HttpContext.Request.Path.ToString().Contains("metadata"))
                {
                    return new JsonResult(RESOURCENITFOUNDJSON(HttpContext.Request.Path.ToString()))
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
                    };
                }

                var result = serviceFoundation.FoundationMetaData();
                return new JsonResult(result)
                {
                    ContentType = "application/fhir+json"
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{*url}")]  // Catch-all route
        [HttpPost("{*url}")]
        [HttpPut("{*url}")]
        [HttpDelete("{*url}")]
        public ActionResult HandleNonMetaAction(string url)
        {
            // Only return this response if the requested URL doesn't start with 'meta'
            if (url.Contains("meta"))
            {
                return new JsonResult(BadRequestJSON(HttpContext.Request.Method))
                {
                    ContentType = "application/fhir+json",
                    StatusCode = 400
                };
            }

            return new JsonResult(BadRequestJSON(HttpContext.Request.Path.ToString()))
            {
                ContentType = "application/fhir+json",
                StatusCode = 400
            };
        }

        /// <summary>
        /// Find a patient
        /// </summary>

        [HttpGet]
        [Route("Patient")]
        [PatientValidateIdentifierFilter]
        public ActionResult Patient(
      [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
      [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
      [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
      [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
      [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
         [FromQuery(Name = "identifier")] string identifier = null)
        {
            try
            {
                // checking
                var nhsNumber = "";
                if (identifier.Contains('|'))
                {
                    nhsNumber = identifier.Split('|')[1];
                }

                var fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                var queryParams = QueryHelpers.ParseQuery(Request.QueryString.Value);
                int identifierCount = queryParams.ContainsKey("identifier") ? queryParams["identifier"].Count : 0;

                var result = serviceFoundation.FindAPatient(nhsNumber, "", identifier, identifierCount, fullUrl);

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
        /// Read a patient
        /// </summary>

        [HttpGet]
        [Route("Patient/{id}")]
        public ActionResult PatientId(
               string id,
              [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM"
            )
        {
            try
            {
                var result = serviceFoundation.ReadAPatient(id.ToString(), SspInterctionId, "External");

                Response.Headers.Add("Cache-Control", "no-store");

                if (result[2] == "InvalidInteractionId")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }

                if (result[2] == "NotFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
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
        /// Find a Practitioner
        /// </summary>

        [HttpGet]
        [Route("Practitioner")]
        [PractitionerValidateIdentifierFilter]
        public ActionResult Practitioner(

             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:practitioner-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "identifier")] string identifier = null
             )
        {
            try
            {
                var req = Request;
                var fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                var queryParams = QueryHelpers.ParseQuery(Request.QueryString.Value);
                int identifierCount = queryParams.ContainsKey("identifier") ? queryParams["identifier"].Count : 0;


                var sdsId = "";
                if (identifier.Contains('|'))
                {
                    sdsId = identifier.Split('|')[1];
                }
                var result = serviceFoundation.FindAPractioner(sdsId, identifier, SspInterctionId, identifierCount, fullUrl);
                Response.Headers.Add("Cache-Control", "no-store");
                if (result[2] == "NotFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
                    };
                }
                if (result[2] == "InvalidParameter")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 422
                    };
                }
                if (result[2] == "InvalidInteractionId" || result[2] == "MultipleIdentifier" || result[2] == "IdentifierSpellingMistake")
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

                #region commented Code

                //    var jsonContent = new
                //    {
                //        resourceType = "Practitioner",
                //        id = "1",
                //        meta = new
                //        {
                //            versionId = "1469444400000",
                //            lastUpdated = "2016-07-25T12:00:00.000+01:00",
                //            profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1" }
                //        },
                //        extension = new[]
                //{
                //    new
                //    {
                //        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                //        extension = new[]
                //        {
                //            new
                //            {
                //                url = "language",
                //                valueCodeableConcept = new
                //                {
                //                    coding = new[]
                //                    {
                //                        new
                //                        {
                //                            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
                //                            code = "de",
                //                            display = "German"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    new
                //    {
                //        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                //        extension = new[]
                //        {
                //            new
                //            {
                //                url = "language",
                //                valueCodeableConcept = new
                //                {
                //                    coding = new[]
                //                    {
                //                        new
                //                        {
                //                            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
                //                            code = "en",
                //                            display = "English"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},
                //        identifier = new[]
                //{
                //    new
                //    {
                //        system = "https://fhir.nhs.uk/Id/sds-user-id",
                //        value = "G13579135"
                //    }
                //},
                //        name = new[]
                //{
                //    new
                //    {
                //        use = "usual",
                //        family = "Gilbert",
                //        given = new[] { "Nichole" },
                //        prefix = new[] { "Miss" }
                //    }
                //},
                //        gender = "female"
                //    };
                //    return new JsonResult(jsonContent)
                //    {
                //        ContentType = "application/fhir+json"
                //    };

                #endregion

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Read a Practitioner
        /// </summary>

        [HttpGet]
        [Route("Practitioner/{id}")]
        public ActionResult PractitionerId(
            string id,
              [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM"
            )
        {
            try
            {
                var result = serviceFoundation.ReadAPractioner(id.ToString(), SspInterctionId, "External");
                Response.Headers.Add("Cache-Control", "no-store");
                if (result[2] == "NotFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
                    };
                }
                if (result[2] == "InvalidInteractionId")
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

                #region commented Code

                //    var jsonContent = new
                //    {
                //        resourceType = "Practitioner",
                //        id = "1",
                //        meta = new
                //        {
                //            versionId = "1469444400000",
                //            lastUpdated = "2016-07-25T12:00:00.000+01:00",
                //            profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1" }
                //        },
                //        extension = new[]
                //{
                //    new
                //    {
                //        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                //        extension = new[]
                //        {
                //            new
                //            {
                //                url = "language",
                //                valueCodeableConcept = new
                //                {
                //                    coding = new[]
                //                    {
                //                        new
                //                        {
                //                            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
                //                            code = "de",
                //                            display = "German"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    },
                //    new
                //    {
                //        url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CareConnect-GPC-NHSCommunication-1",
                //        extension = new[]
                //        {
                //            new
                //            {
                //                url = "language",
                //                valueCodeableConcept = new
                //                {
                //                    coding = new[]
                //                    {
                //                        new
                //                        {
                //                            system = "https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-HumanLanguage-1",
                //                            code = "en",
                //                            display = "English"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},
                //        identifier = new[]
                //{
                //    new
                //    {
                //        system = "https://fhir.nhs.uk/Id/sds-user-id",
                //        value = "G13579135"
                //    }
                //},
                //        name = new[]
                //{
                //    new
                //    {
                //        use = "usual",
                //        family = "Gilbert",
                //        given = new[] { "Nichole" },
                //        prefix = new[] { "Miss" }
                //    }
                //},
                //        gender = "female"
                //    };
                //    return new JsonResult(jsonContent)
                //    {
                //        ContentType = "application/fhir+json"
                //    };

                #endregion
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Find a Organization
        /// </summary>

        [HttpGet]
        [Route("Organization")]
        [OrganizationValidateIdentifierFilter]

        public ActionResult Organization(
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:organization-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "identifier")] string identifier = null
             )
        {
            try
            {

                var odsCode = "";
                if (identifier.Contains('|'))
                {
                    odsCode = identifier.Split('|')[1];
                }

                var result = serviceFoundation.FindAOrganization(odsCode, SspInterctionId);
                Response.Headers.Add("Cache-Control", "no-store");
                if (result[2] == "NotFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
                    };
                }
                if (result[2] == "InvalidInteractionId")
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
        /// Read a Organization
        /// </summary>

        [HttpGet]
        [Route("Organization/{id}")]
        public ActionResult OrganizationId(
             string id,
              [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorizations")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM"

            )
        {
            try
            {
                var result = serviceFoundation.ReadAOrganization(id, SspInterctionId, "External");
                Response.Headers.Add("Cache-Control", "no-store");


                if (result[2] == "NotFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 404
                    };
                }
                if (result[2] == "InvalidInteractionId")
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
        /// Read a Location
        /// </summary>

        [HttpGet]
        [Route("Location/{id}")]
        public ActionResult LocationId(
            string id,
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:patient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM"

            )
        {
            try
            {
                var result = serviceFoundation.ReadALocation(id, SspInterctionId, "External");
                Response.Headers.Add("Cache-Control", "no-store");

                if (result[2] == "NotFound")
                {
                    return NotFound();

                }
                if (result[2] == "InvalidInteractionId")
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
        /// Register a patient
        /// </summary>

        [HttpPost]
        [Route("Patient/$gpc.registerpatient")]
        public ActionResult registerAPatient(
              [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:fhir:operation:gpc.registerpatient-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL0NvbnN1bWVyU3lzdGVtVVJMIiwic3ViIjoiMSIsImF1ZCI6Imh0dHBzOi8vYXV0aG9yaXplLmZoaXIubmhzLm5ldC90b2tlbiIsImV4cCI6MTcyMjg2NTIyNSwiaWF0IjoxNzIyODY0OTI1LCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwicmVxdWVzdGluZ19kZXZpY2UiOnsicmVzb3VyY2VUeXBlIjoiRGV2aWNlIiwiaWRlbnRpZmllciI6W3sic3lzdGVtIjoiR1BDb25uZWN0VGVzdFN5c3RlbSIsInZhbHVlIjoiQ2xpZW50In1dLCJtb2RlbCI6InYxIiwidmVyc2lvbiI6IjEuMSJ9LCJyZXF1ZXN0aW5nX29yZ2FuaXphdGlvbiI6eyJyZXNvdXJjZVR5cGUiOiJPcmdhbml6YXRpb24iLCJpZGVudGlmaWVyIjpbeyJzeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZSIsInZhbHVlIjoiR1BDQTAwMDEifV0sIm5hbWUiOiJHUCBDb25uZWN0IEFzc3VyYW5jZSJ9LCJyZXF1ZXN0aW5nX3ByYWN0aXRpb25lciI6eyJyZXNvdXJjZVR5cGUiOiJQcmFjdGl0aW9uZXIiLCJpZCI6IjEiLCJpZGVudGlmaWVyIjpbeyJzeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy11c2VyLWlkIiwidmFsdWUiOiJHQ0FTRFMwMDAxIn0seyJzeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWQiLCJ2YWx1ZSI6IjExMjIzMzQ0NTU2NiJ9LHsic3lzdGVtIjoiaHR0cHM6Ly9jb25zdW1lcnN1cHBsaWVyLmNvbS9JZC91c2VyLWd1aWQiLCJ2YWx1ZSI6Ijk4ZWQ0Zjc4LTgxNGQtNDI2Ni04ZDViLWNkZTc0MmYzMDkzYyJ9XSwibmFtZSI6W3siZmFtaWx5IjoiQXNzdXJhbmNlUHJhY3RpdGlvbmVyIiwiZ2l2ZW4iOlsiQXNzdXJhbmNlVGVzdCJdLCJwcmVmaXgiOlsiTXIiXX1dfSwicmVxdWVzdGVkX3Njb3BlIjoicGF0aWVudC8qLndyaXRlIn0.",
             [FromBody] dynamic RegisterPatientDTO = null)
        {
            try
            {
                var xx = RegisterPatientDTO.ToString();

                var patientDetails = JsonConvert.DeserializeObject<RegisterPatientDTO>(RegisterPatientDTO.ToString());

                var result = serviceFoundation.CreatePatientRecord(patientDetails, RegisterPatientDTO.ToString(), Authorization);
                Response.Headers.Add("Cache-Control", "no-store");

                if (result[2] == "InvalidNHSNumber" || result[2] == "DuplicateField" || result[2] == "InvalidDemographic" || result[2] == "NoFamilyName" || result[2] == "NoDobSupplied" || result[2] == "NoOfficialSupplied" || result[2] == "MoreThanOneResourcesFound" || result[2] == "NotPassingValueOrSystem" || result[2] == "JWTClaimIssue")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }

                if (result[2] == "DuplicateAddressUse" || result[2] == "OldAddress" || result[2] == "WorkAddress" || result[2] == "PropertiesInvalid" || result[2] == "MultipleSameExtensionFound")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 422
                    };
                }

                if (result[2] == "DuplicateRejected")
                {
                    return new JsonResult(result[0])
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 409
                    };
                }

                return new JsonResult(result[0])
                {
                    ContentType = "application/fhir+json",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Patient/{*operation}")]
        public ActionResult HandleInvalidOperation()
        {
            var response = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[] { "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1" }
                },
                issue = new[]
                {
            new
            {
                severity = "error",
                code = "invalid",
                details = new
                {
                    coding = new[]
                    {
                        new
                        {
                            system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                            code = "REFERENCE_NOT_FOUND",
                            display = "REFERENCE_NOT_FOUND"
                        }
                    }
                },
                diagnostics = "Unknown operation type"
            }
        }
            };

            return new JsonResult(response)
            {
                ContentType = "application/fhir+json",
                StatusCode = 400
            };
        }

        /// <summary>
        /// Find a patient
        /// </summary>

        [HttpGet]
        [Route("Patient/{id}/DocumentReference")]
        public ActionResult DocumentReference(
             [FromRoute] string id,
             [FromHeader(Name = "Ssp-TraceID")][Required] string SspTraceId = "09a01679-2564-0fb4-5129-aecc81ea2706",
             [FromHeader(Name = "Ssp-From")][Required] string SspFrom = "200000000359",
             [FromHeader(Name = "Ssp-To")][Required] string SspTo = "918999198993",
             [FromHeader(Name = "Ssp-InteractionID")][Required] string SspInterctionId = "urn:nhs:names:services:gpconnect:documents:fhir:rest:search:documentreference-1",
             [FromHeader(Name = "Authorization")][Required] string Authorization = "Bearer g1112R_ccQ1Ebbb4gtHBP1aaaNM",
             [FromQuery(Name = "_include")][Required] string _includes = "DocumentReference:subject:Patient",
             [FromQuery(Name = "_revinclude:recurse")][Required] string _revincluderecurse = "PractitionerRole:practitioner",
             [FromQuery(Name = "Created-start")] string Createdstart = "ge2024-01-05",
              [FromQuery(Name = "Created-end")] string CreatedEnd = "ge2024-10-05",
               [FromQuery(Name = "author")] string author = "docs",
                [FromQuery(Name = "description")] string description = "docs"
             )
        {
            try
            {
                var result = serviceDocument.GetDocumentReference(id, Createdstart, CreatedEnd, author, description, SspTraceId);

                return new JsonResult(result)
                {
                    ContentType = "application/fhir+json",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #endregion


        #region Internal-Method
        internal dynamic BadRequestJSON(string type)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
            {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
{
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "The interaction ID does not match the HTTP request verb (interaction ID - urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1 HTTP verb - type)"
                }
            }
            };
            return operationOutcome;
        }
        internal dynamic RESOURCENITFOUNDJSON(string ROUTE)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
            {
                        "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                    }
                },
                issue = new[]
{
                    new
                    {
                        severity = "error",
                        code = "invalid",
                        details = new
                        {
                            coding = new[]
                        {
                                new
                                {
                                    system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                    code = "REFERENCE_NOT_FOUND",
                                    display = "REFERENCE_NOT_FOUND"
                                }
                            }
                        },
                        diagnostics = "Request containts invalid resource (" + ROUTE + ")"
                    }
                }
            };
            return operationOutcome;
        }
        #endregion
    }

    #region EmptyValidationClasses

    public class OrganizationValidateIdentifierFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
               {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
           {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "No identifier parameter found!"
                }
            }
            };
            var operationOutcome1 = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
                {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
            {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "Resource in request does not match resource in interaction (request - Organization interaction )"
                }
            }
            };

            var errorOccur = false;

            if (context.ActionArguments.ContainsKey("SspInterctionId"))
            {
                var sspInteractionId = context.ActionArguments["SspInterctionId"] as string;
                if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:search:organization-1")
                {



                    context.Result = new JsonResult(operationOutcome1)
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400,

                    };
                    context.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
                    errorOccur = true;
                }

            }

            if (errorOccur == false)
            {
                if (!context.ActionArguments.ContainsKey("identifier") || string.IsNullOrEmpty(context.ActionArguments["identifier"] as string))
                {

                    context.Result = new JsonResult(operationOutcome)
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class PractitionerValidateIdentifierFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
               {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
           {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "No identifier parameter found!"
                }
            }
            };
            var operationOutcome1 = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
                {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
            {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "Resource in request does not match resource in interaction (request - Practitioner interaction )"
                }
            }
            };

            var errorOccur = false;

            if (context.ActionArguments.ContainsKey("SspInterctionId"))
            {
                var sspInteractionId = context.ActionArguments["SspInterctionId"] as string;
                if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:search:practitioner-1")
                {



                    context.Result = new JsonResult(operationOutcome1)
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400,

                    };
                    context.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
                    errorOccur = true;
                }

            }

            if (errorOccur == false)
            {
                if (!context.ActionArguments.ContainsKey("identifier") || string.IsNullOrEmpty(context.ActionArguments["identifier"] as string))
                {

                    context.Result = new JsonResult(operationOutcome)
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class PatientValidateIdentifierFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var operationOutcome = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
               {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
           {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "No identifier parameter found!"
                }
            }
            };
            var operationOutcome1 = new
            {
                resourceType = "OperationOutcome",
                meta = new
                {
                    profile = new[]
                {
                    "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-OperationOutcome-1"
                }
                },
                issue = new[]
            {
                new
                {
                    severity = "error",
                    code = "invalid",
                    details = new
                    {
                        coding = new[]
                        {
                            new
                            {
                                system = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1",
                                code = "BAD_REQUEST",
                                display = "BAD_REQUEST"
                            }
                        }
                    },
                    diagnostics = "Resource in request does not match resource in interaction (request - Practitioner interaction )"
                }
            }
            };

            var errorOccur = false;

            if (errorOccur == false)
            {
                if (!context.ActionArguments.ContainsKey("identifier") || string.IsNullOrEmpty(context.ActionArguments["identifier"] as string))
                {

                    context.Result = new JsonResult(operationOutcome)
                    {
                        ContentType = "application/fhir+json",
                        StatusCode = 400
                    };
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class HttpMethodCheckAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedMethods;

        public HttpMethodCheckAttribute(params string[] allowedMethods)
        {
            _allowedMethods = allowedMethods;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_allowedMethods.Contains(context.HttpContext.Request.Method.ToUpper()))
            {
                context.Result = new BadRequestObjectResult($"HTTP {context.HttpContext.Request.Method} is not allowed on this endpoint.");
            }
            base.OnActionExecuting(context);
        }
    }

    #endregion
}
