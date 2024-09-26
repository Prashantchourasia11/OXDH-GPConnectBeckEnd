using Nancy.Json;
using System.IdentityModel.Tokens.Jwt;

namespace GP_Connect.JWT_Checker
{
    public class JWTChecker
    {
        public dynamic JWTCheckingFunction(string token)
        {
            dynamic[] finaljson = new dynamic[3];
            try
            {
                if (token != "")
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token.Replace("Bearer ", ""));

                    var aud = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aud")?.Value;
                    var exp = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "exp")?.Value;
                    var iat = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iat")?.Value;
                    var iss = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
                    var reason_for_request = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "reason_for_request")?.Value;
                    var requested_scope = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "requested_scope")?.Value;
                    var requesting_device = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "requesting_device")?.Value;
                    var requesting_practitioner = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "requesting_practitioner")?.Value;

                    if (aud == null)
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim aud is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }
                    if (iss == null)
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim iss is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }
                    else if (exp != null && iat != null)
                    {
                        // Convert the exp and iat claims (which are in seconds since Unix epoch) to DateTime
                        var expUnix = long.Parse(exp);
                        var iatUnix = long.Parse(iat);

                        var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                        var iatDateTime = DateTimeOffset.FromUnixTimeSeconds(iatUnix).UtcDateTime;

                        // Check if the token has expired
                        if (expDateTime < DateTime.UtcNow)
                        {
                            finaljson[0] = JWTErrorJSON("JWT has expired.");
                            finaljson[1] = "";
                            finaljson[2] = "400";
                            return finaljson;
                        }

                        // Check if expiration time is at least 5 minutes after the issued time
                        var timeDifference = expDateTime - iatDateTime;
                        if (timeDifference < TimeSpan.FromMinutes(5))
                        {
                            finaljson[0] = JWTErrorJSON("JWT expiry time is less than 5 minutes after issued time.");
                            finaljson[1] = "";
                            finaljson[2] = "400";
                            return finaljson;
                        }
                        var fiveMinStatus = JWT_ExpiryTime_GreaterThan300Seconds(exp, iat);
                        if(fiveMinStatus == true)
                        {
                            finaljson[0] = JWTErrorJSON("JWT expiry time is not 5 minutes after the creation time.");
                            finaljson[1] = "";
                            finaljson[2] = "400";
                            return finaljson;
                        }

                        // Token is valid and meets the 5-minute condition
                    }
                    else
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim exp or iat is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                    if(reason_for_request == null)
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim reason_for_request is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                    if(requested_scope != null)
                    {
                        if(requested_scope.Contains("write") || requested_scope.Contains("read"))
                        {

                        }
                        else
                        {
                            finaljson[0] = JWTErrorJSON("JWT Bad Request Exception Invalid requested scope:" + requested_scope);
                            finaljson[1] = "";
                            finaljson[2] = "400";
                            return finaljson;
                        }

                    }
                    else
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim requested_scope is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                    if(requesting_device != null)
                    {
                        if(requesting_device.Contains("invalidField"))
                        {
                            finaljson[0] = JWTErrorJSON("Invalid Resource claim requesting_device in JWT (Not a valid Fhir Resource - Unknown element found during parse)");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                        if (!requesting_device.Contains("\"resourceType\":\"Device\""))
                        {
                            finaljson[0] = JWTErrorJSON("Invalid Resource claim requesting_device in JWT (Not a valid Fhir Resource - Unknown element found during parse)");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }

                    }
                    else
                    {
                        finaljson[0] = JWTErrorJSON("JWT JSON entry incomplete: claim requesting_device is null.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                    if (requesting_practitioner != null)
                    {
                        if (!requesting_practitioner.Contains("\"resourceType\":\"Practitioner\""))
                        {
                            finaljson[0] = JWTErrorJSON("Invalid Resource claim requesting_practitioner in JWT (Not a valid Fhir Resource - Unknown element found during parse)");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    
                    }
                    else
                    {
                        finaljson[0] = JWTErrorJSON("JWT required claim requesting_practitioner is not present.");
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }

                }
                else
                {
                    finaljson[0] = JWTErrorJSON("Authorization is missing.");
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }
                return finaljson;
            }
            catch (Exception ex)
            {
                finaljson[0] = "";
                finaljson[1] = "";
                finaljson[2] = "400";
                return finaljson;
            }
        }
        internal dynamic JWTErrorJSON(string errorName)
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
                    diagnostics = errorName
                }
                  }
              };
                return operationOutcome;
            

        }
        internal bool JWT_ExpiryTime_GreaterThan300Seconds(string exp, string iat)
        {
            // Convert exp and iat claims (which are in seconds since Unix epoch) to DateTime
            var expUnix = long.Parse(exp);
            var iatUnix = long.Parse(iat);

            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            var iatDateTime = DateTimeOffset.FromUnixTimeSeconds(iatUnix).UtcDateTime;

            // Check if expiration time is greater than 300 seconds after issued time
            return (expDateTime - iatDateTime).TotalSeconds > 300;
        }
    }
}
