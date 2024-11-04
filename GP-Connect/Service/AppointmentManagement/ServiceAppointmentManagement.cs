using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.FHIR_JSON;
using GP_Connect.FHIR_JSON.AccessDocument;
using GP_Connect.FHIR_JSON.AppointmentManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.JSInterop;
using Microsoft.SharePoint.Packaging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swagger.Net;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;

namespace GP_Connect.Service.AppointmentManagement
{
    public class ServiceAppointmentManagement
    {

        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();

        #endregion

        #region Constructor

        public ServiceAppointmentManagement()
        {

            _crmServiceClient = crmCon.crmconnectionOXVC();
        }

        #endregion

        #region Method

        public dynamic GetFreeSlot(string fromDate, string toDate, string status, string _include, string fullUrl, string ods, string orgType, string _includerecurse)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                if (status == null)
                {
                    finaljson[0] = abai.BadRequestJSON("Status query is missing.");
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }
                else if (status == "free")
                {
                    status = "101";
                }
                else
                {
                    finaljson[0] = abai.InvalidParameterJSON("Slot status must be free, can't search by :" + status);
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (fromDate == null)
                {
                    finaljson[0] = abai.BadRequestJSON("From date is missing.");
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }

                if (toDate == null)
                {
                    finaljson[0] = abai.BadRequestJSON("To date is missing.");
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }

                if (fromDate != "")
                {
                    if (fromDate.Contains("ge"))
                    {
                        fromDate = fromDate.Replace("ge", "");
                        if (fromDate.Length < 10)
                        {
                            finaljson[0] = abai.InvalidParameterJSON("From date is invalid.");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                        var checkerFromDate = ConvertToProperDateFormat(fromDate, "start");
                        if (checkerFromDate == "Invalid_date_format")
                        {
                            finaljson[0] = abai.InvalidParameterJSON("From date is invalid.");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }
                    else
                    {
                        finaljson[0] = abai.InvalidParameterJSON("From date is invalid.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }
                else
                {
                    finaljson[0] = abai.InvalidParameterJSON("From date is invalid.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (toDate != "")
                {
                    if (toDate.Contains("le"))
                    {
                        toDate = toDate.Replace("le", "");
                        var checkertoDate = ConvertToProperDateFormat(toDate, "start");
                        if (checkertoDate == "Invalid_date_format")
                        {
                            finaljson[0] = abai.InvalidParameterJSON("To date is invalid.");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }
                    else
                    {
                        finaljson[0] = abai.InvalidParameterJSON("To date is invalid.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }
                else
                {
                    finaljson[0] = abai.InvalidParameterJSON("To date is invalid.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }


                if (DateTime.Parse(fromDate) > DateTime.Parse(toDate))
                {
                    finaljson[0] = abai.InvalidParameterJSON("From date is always less than to date.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                else
                {
                    var totalDays = (DateTime.Parse(toDate) - DateTime.Parse(fromDate)).TotalDays;
                    if (totalDays <= 14)
                    {

                    }
                    else
                    {
                        finaljson[0] = abai.InvalidParameterJSON("The gap between the start date and the end date must not exceed 14 days.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }


                var freeSlotxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' top='5000'>
                                     <entity name='msemr_slot'>
                                       <attribute name='msemr_slotid' />
                                       <attribute name='msemr_name' />
                                       <attribute name='createdon' />
                                       <attribute name='bcrm_versionnumber' />
                                       <attribute name='statuscode' />
                                       <attribute name='statecode' />
                                       <attribute name='msemr_status' />
                                       <attribute name='msemr_start' />
                                       <attribute name='msemr_specialty' />
                                       <attribute name='bcrm_slotstatus' />
                                       <attribute name='bcrm_slotduration' />
                                       <attribute name='bcrm_shift' />
                                       <attribute name='msemr_servicetypenew' />
                                       <attribute name='msemr_servicecategory' />
                                       <attribute name='bcrm_sequencenumber' />
                                       <attribute name='msemr_schedule' />
                                       <attribute name='overriddencreatedon' />
                                       <attribute name='bcrm_preferredcontactmethod' />
                                       <attribute name='owningbusinessunit' />
                                       <attribute name='ownerid' />
                                       <attribute name='msemr_overbooked' />
                                       <attribute name='modifiedon' />
                                       <attribute name='modifiedonbehalfby' />
                                       <attribute name='modifiedby' />
                                       <attribute name='msemr_end' />
                                       <attribute name='createdonbehalfby' />
                                       <attribute name='createdby' />
                                       <attribute name='msemr_comment' />
                                       <attribute name='msemr_appointmenttypenew' />
                                       <attribute name='msemr_appointmentemrid' />
                                       <order attribute='msemr_name' descending='false' />
                                       <filter type='and'>
                                         <condition attribute='msemr_end' operator='on-or-before' value='" + toDate + @"' />
                                         <condition attribute='msemr_start' operator='on-or-after' value='" + fromDate + @"' />
                                         <condition attribute='bcrm_slotstatus' operator='eq' value='" + status + @"' />
                                       </filter>
                                       <link-entity name='bcrm_shifts' from='bcrm_shiftsid' to='bcrm_shift' visible='false' link-type='outer' alias='schedule'>
                                         <attribute name='bcrm_sequencenumber' />
                                         <attribute name='bcrm_service' />
                                         <attribute name='bcrm_versionnumber' />
                                         <attribute name='bcrm_scheduleaccessibility' />
                                          <attribute name='bcrm_todate' />
                                          <attribute name='bcrm_staff' />
                                          <attribute name='bcrm_gplocation' />
                                          <attribute name='bcrm_fromdate' />
                                          <attribute name='bcrm_clinic' />
                                        <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_clinic' visible='false' link-type='outer' alias='organisation'>
                                        <attribute name='bcrm_gpc_telecom_value' />
                                        <attribute name='bcrm_gpc_sequence_number' />
                                        <attribute name='bcrm_clinictype' />     
                                        <attribute name='bcrm_odscode' />
                                        <attribute name='bcrm_name' />
                                        </link-entity>
                                      
                                       </link-entity>
                                     </entity>
                                    </fetch>";


                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(freeSlotxml));

                if (AnswerCollection.Entities.Count > 0)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    foreach (var record in AnswerCollection.Entities)
                    {
                        if (record.Attributes.Contains("schedule.bcrm_scheduleaccessibility"))
                        {
                            dynamic scheduleAccessiblity = record.FormattedValues["schedule.bcrm_scheduleaccessibility"].ToString().ToUpper(); 
                            if (scheduleAccessiblity == "GP CONNECT")
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }
                    }
                    AnswerCollection = answerCollectionCorrect;
                }

                //first scenario
                if (ods == "" && orgType == "" && AnswerCollection.Entities.Count > 0)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    foreach (var record in AnswerCollection.Entities)
                    {
                        if (record.Attributes.Contains("organisation.bcrm_clinictype"))
                        {

                            dynamic orType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString().ToUpper() : "";
                            if (orType.ToString().ToUpper().Contains("OTHER"))
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }
                    }
                    AnswerCollection = answerCollectionCorrect;
                }

                // second scenario
                if (ods == "" && orgType != "" && AnswerCollection.Entities.Count > 0)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    foreach (var record in AnswerCollection.Entities)
                    {
                        if (record.Attributes.Contains("organisation.bcrm_clinictype"))
                        {

                            dynamic orType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString().ToUpper() : "";
                            if (orgType.ToString().ToUpper() == orType || orType.ToString().ToUpper().Contains("OTHER"))
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }
                    }
                    AnswerCollection = answerCollectionCorrect;
                }

                // third scenario
                if (ods != "" && orgType == "" && AnswerCollection.Entities.Count > 0)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    foreach (var record in AnswerCollection.Entities)
                    {
                        if (record.Attributes.Contains("organisation.bcrm_clinictype"))
                        {
                            dynamic orType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString().ToUpper() : "";
                            if (orType.ToString().ToUpper().Contains("OTHER"))
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }

                        if (record.Attributes.Contains("organisation.bcrm_odscode"))
                        {
                            dynamic ContainODS = record["organisation.bcrm_odscode"];
                            var odsfinal = ContainODS.Value.ToString().ToUpper();
                            if (ods.ToString().ToUpper() == odsfinal)
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }
                    }
                    AnswerCollection = answerCollectionCorrect;
                }

                // fourth scenario
                if (ods != "" && orgType != "" && AnswerCollection.Entities.Count > 0)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    foreach (var record in AnswerCollection.Entities)
                    {
                        if (record.Attributes.Contains("organisation.bcrm_clinictype"))
                        {

                            dynamic orType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString().ToUpper() : "";
                            if (orgType.ToString().ToUpper() == orType || orType.ToString().ToUpper().Contains("OTHER"))
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }

                        if (record.Attributes.Contains("organisation.bcrm_odscode"))
                        {
                            dynamic ContainODS = record["organisation.bcrm_odscode"];
                            var odsfinal = ContainODS.Value.ToString().ToUpper();
                            if (ods.ToString().ToUpper() == odsfinal)
                            {
                                answerCollectionCorrect.Entities.Add(record);
                            }
                        }
                    }
                    AnswerCollection = answerCollectionCorrect;
                }



                if (AnswerCollection.Entities.Count > 50)
                {
                    EntityCollection answerCollectionCorrect = new EntityCollection();
                    for (var i = 0; i < 25; i++)
                    {
                        answerCollectionCorrect.Entities.Add(AnswerCollection[i]);
                    }
                    for (var i = AnswerCollection.Entities.Count - 1; i < AnswerCollection.Entities.Count - 25; i--)
                    {
                        answerCollectionCorrect.Entities.Add(AnswerCollection[i]);
                    }
                    AnswerCollection = answerCollectionCorrect;
                }



                if (AnswerCollection.Entities.Count > 0)
                {
                    try
                    {
                        DateTime startDate = DateTimeOffset.Parse(fromDate).DateTime;
                        DateTime endDate = DateTimeOffset.Parse(toDate).DateTime;
                        AnswerCollection = GetFilterRecord(AnswerCollection, startDate, endDate);
                    }
                    catch (Exception)
                    {

                    }

                }

                List<SlotDTO> slotList = new List<SlotDTO>();

                List<int> ScheduleSequenceNumber = new List<int>();
                List<string> PractitionerList = new List<string>();
                List<string> GPLocationList = new List<string>();
                List<string> OrganizationList = new List<string>();


                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        SlotDTO slotDetail = GetSlotDTO();

                        slotDetail.resource.resourceType = "Slot";

                        slotDetail.resource.meta.versionId = record.Attributes.Contains("bcrm_versionnumber") ? record["bcrm_versionnumber"].ToString() : "1";
                        slotDetail.resource.meta.profile[0] = "https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1";

                        slotDetail.resource.extension[0].url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2";
                        slotDetail.resource.extension[0].valueCode = record.Attributes.Contains("bcrm_preferredcontactmethod") ? record.FormattedValues["bcrm_preferredcontactmethod"].ToString() : "";

                        if (slotDetail.resource.extension[0].valueCode.Contains("Telephone"))
                        {
                            slotDetail.resource.extension[0].valueCode = "Telephone";
                        }

                        if (slotDetail.resource.extension[0].valueCode.Contains("Video"))
                        {
                            slotDetail.resource.extension[0].valueCode = "Video";
                        }

                        dynamic service = record.Attributes.Contains("schedule.bcrm_service") ? record["schedule.bcrm_service"] : string.Empty;
                        slotDetail.resource.serviceType[0].text = service.Value.Name;

                        dynamic scheduleNo = record.Attributes.Contains("schedule.bcrm_sequencenumber") ? record["schedule.bcrm_sequencenumber"] : string.Empty;
                        slotDetail.resource.schedule.reference = "Schedule/" + scheduleNo.Value;
                        ScheduleSequenceNumber.Add(int.Parse(scheduleNo.Value));

                        if (record.Attributes.Contains("msemr_start")) { slotDetail.resource.start = (DateTime)record.Attributes["msemr_start"]; }
                        if (record.Attributes.Contains("msemr_end")) { slotDetail.resource.end = (DateTime)record.Attributes["msemr_end"]; }
                        if (record.Attributes.Contains("bcrm_sequencenumber")) { slotDetail.resource.id = record.Attributes.Contains("bcrm_sequencenumber") ? record["bcrm_sequencenumber"].ToString() : string.Empty; }

                        slotDetail.resource.status = record.Attributes.Contains("bcrm_slotstatus") ? record.FormattedValues["bcrm_slotstatus"].ToString().ToLower() : string.Empty;

                        slotList.Add(slotDetail);


                        dynamic staffId = record.Attributes.Contains("schedule.bcrm_staff") ? record["schedule.bcrm_staff"] : string.Empty;
                        PractitionerList.Add(staffId.Value.Id.ToString());

                        dynamic gpLocationId = record.Attributes.Contains("schedule.bcrm_gplocation") ? record["schedule.bcrm_gplocation"] : string.Empty;
                        GPLocationList.Add(gpLocationId.Value.Id.ToString());

                        dynamic organizationId = record.Attributes.Contains("schedule.bcrm_clinic") ? record["schedule.bcrm_clinic"] : string.Empty;
                        OrganizationList.Add(organizationId.Value.Id.ToString());

                    }
                }
                List<int> ScheduleSequenceUniqueValues = ScheduleSequenceNumber.Distinct().ToList();
                List<string> PractitionerIdUniqueValue = PractitionerList.Distinct().ToList();
                List<string> GPLocationIdUniqueValue = GPLocationList.Distinct().ToList();
                List<string> OrganizationIdUniqueValue = OrganizationList.Distinct().ToList();

                var slotListJSON = new JavaScriptSerializer().Serialize(slotList);
                var scheduleJSON = GetSheduleJSON(ScheduleSequenceUniqueValues);
                var practitionerJSON = GetPractitionerJSON(PractitionerIdUniqueValue);
                var locationJSON = GetGPLocationJSON(GPLocationIdUniqueValue);
                var organizationJSON = GetOrganizationJSON(OrganizationIdUniqueValue);

                if (_includerecurse != null)
                {
                    if (_includerecurse == "Schedule:actor:Practitioner,Schedule:actor:Location,Location:managingOrganization")
                    {

                    }
                    else if (_includerecurse.Contains("Practitioner") && _includerecurse.Contains("Location"))
                    {

                    }
                    else if (_includerecurse.Contains("Practitioner"))
                    {
                        locationJSON = "";
                    }
                    else if (_includerecurse.Contains("Location"))
                    {
                        practitionerJSON = "";
                    }




                }


                var result = CreateFinalJSONForSlot(slotListJSON, scheduleJSON, practitionerJSON, locationJSON, organizationJSON);


                finaljson[0] = result;
                finaljson[1] = "";
                finaljson[2] = "200";
                return finaljson;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic BookAnAppointment(RequestBookAppointmentDTO bookAppointment, string BookAppointmentDTOString, string Prefer)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                if (BookAppointmentDTOString.Contains("https://fhir.nhs.uk/Id/gpconnect-appointment-identifier"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Identifier Element is not recognized");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (BookAppointmentDTOString.Contains("\"reason\":["))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Contain Invalid field : reason");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (!BookAppointmentDTOString.Contains("\"resourceType\": \"Organization\""))
                {
                    if (!BookAppointmentDTOString.Contains("\"resourceType\":\"Organization\""))
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Booking organization object is mandatory object.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }

                if (BookAppointmentDTOString.Contains("http://hl7.org/fhir/stu3/valueset-c80-practice-codes"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("practice code should not be acceptable.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (!BookAppointmentDTOString.Contains("https://fhir.nhs.uk/STU3/CodeSystem/GPConnect-OrganisationType-1"))
                {
                    var slotNo1 = bookAppointment.slot[0].reference.Replace("Slot/", "");
                    var slotOtherStatus = CheckSlotOrganizationTypeIsOther(slotNo1);
                    if(slotOtherStatus == false)
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Organization type is required as this is not an unrestricted slot.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }
                if (BookAppointmentDTOString.Contains("\"invalidField\":"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Contain Invalid field : invalidField");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (bookAppointment.participant != null)
                {
                    if (bookAppointment.participant[0].status != "accepted")
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Participant Status Field Is Missing");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                } else
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Participant Field Is Missing");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }


                if (bookAppointment.resourceType != "Appointment")
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Resource Type Is Invalid.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (bookAppointment.extension[0].valueReference.reference.Contains("https:"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Invalid Extension Value Reference");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (bookAppointment.start.ToString().Contains("0001") || bookAppointment.end.ToString().Contains("0001") || bookAppointment.status == null)
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Please check json may be start,end or status field is not present.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (bookAppointment.contained[0].name == null || bookAppointment.contained[0].name == "")
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Organization name should not be empty.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (bookAppointment.contained[0].telecom == null)
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Organization Telecom Should Not Be Null");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (bookAppointment.comment != null)
                {
                    if (bookAppointment.comment.Length > 499)
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Comments too long");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }

                if (bookAppointment.description != null)
                {
                    if (bookAppointment.description.Length > 100)
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Description too long.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }



                var orgNo = bookAppointment.contained[0].id;
                if (orgNo == "1")
                {
                    orgNo = "1000";
                }

                var slotNo = "";
                var locNo = "";
                var patientNo = "";

                var SlotCount = "";
                List<string> SlotsNumberIds = new List<string>();

                try
                {
                    SlotCount = bookAppointment.slot.Count.ToString();
                    foreach (var slot in bookAppointment.slot)
                    {
                        string slotId = slot.reference.Replace("Slot/", "");
                        SlotsNumberIds.Add(slotId);
                    }
                    slotNo = bookAppointment.slot[0].reference.Replace("Slot/", "");
                    locNo = bookAppointment.participant[1].actor.reference.Replace("Location/", "");
                    patientNo = bookAppointment.participant[0].actor.reference.Replace("Patient/", "");
                }
                catch (Exception)
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("You need to mandatory send patient ,slot and location id.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;

                }


                var startDate = bookAppointment.start;
                var endDate = bookAppointment.end;
                var description = bookAppointment.description;
                var status = bookAppointment.status;
                var comments = bookAppointment.comment;
                

                var checkSlotStatus = CheckSlotIsAvailableOrNot(slotNo);
                if (checkSlotStatus == "NotFound")
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Invalid slot Number");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }


                if (checkSlotStatus != "free")
                {
                    finaljson[0] = abai.SlotNotAvailbleJSON();
                    finaljson[1] = "";
                    finaljson[2] = "409";
                    return finaljson;
                }


                var orgLookupId = GetOrganizationLookUpId(orgNo);
                if (orgLookupId == null) { return null; }
                var slotLookupId = GetSlotLookUpId(slotNo);
                if (slotLookupId == null) { return null; }
                var locationLookupId = GetLocationLookUpId(locNo);
                if (locationLookupId == null) { return null; }
                var patientLookupId = GetPatientLookUpId(patientNo);
                if (patientLookupId == null) { return null; }

                // need to write code here , slot is available or not.

                var checkerMultipleSlot = CheckMultipleSlotIsOkorNot(SlotsNumberIds, startDate, endDate);

                if(checkerMultipleSlot == false)
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Something went wrong, possibly because the slots being passed are not adjacent or are associated with different schedules or services or delivery channel.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                var serviceNameAndPractictionerLookId = GetserviceNameAndPractictionerLookId(slotNo);

                Entity GPConnectAppointment = new Entity("appointment", new Guid());
                GPConnectAppointment["bcrm_clinic"] = new EntityReference("bcrm_clinic", new Guid(orgLookupId));
                GPConnectAppointment["bcrm_slot"] = new EntityReference("msemr_slot", new Guid(slotLookupId[0]));
                GPConnectAppointment["bcrm_gplocation"] = new EntityReference("bcrm_gpclocation", new Guid(locationLookupId));
                GPConnectAppointment["bcrm_clinician"] = new EntityReference("bcrm_staff", new Guid(serviceNameAndPractictionerLookId[0]));
                GPConnectAppointment["regardingobjectid"] = new EntityReference("contact", new Guid(patientLookupId));
                GPConnectAppointment["bcrm_required"] = new EntityReference("contact", new Guid(patientLookupId));

                if (status == "booked")
                {
                    GPConnectAppointment["bcrm_status"] = new OptionSetValue(6);
                }
                GPConnectAppointment["bcrm_servicename"] = serviceNameAndPractictionerLookId[1];
                GPConnectAppointment["bcrm_ra_visit_number"] = "1";
                GPConnectAppointment["scheduledstart"] = startDate;
                GPConnectAppointment["scheduledend"] = endDate;
                GPConnectAppointment["description"] = description;


                GPConnectAppointment["bcrm_slotsequencelist"] = new JavaScriptSerializer().Serialize(SlotsNumberIds).ToString();
                GPConnectAppointment["bcrm_gpcappointmentjson"] = new JavaScriptSerializer().Serialize(bookAppointment).ToString();


                GPConnectAppointment["subject"] = "GP Connect Appointment (Slot : "+ new JavaScriptSerializer().Serialize(SlotsNumberIds).ToString() + ")";

                GPConnectAppointment["bcrm_notes"] = comments;
                GPConnectAppointment["bcrm_allappointmentjson"] = new JavaScriptSerializer().Serialize(bookAppointment).ToString();
                GPConnectAppointment["bcrm_locationname"] = "GP-CONNECT";
                GPConnectAppointment["bcrm_doctorname"] = "GP-Connect";
                GPConnectAppointment["bcrm_doctorstatus"] = new OptionSetValue(2);
                GPConnectAppointment["bcrm_preferredcontactmethod"] = new OptionSetValue(int.Parse(slotLookupId[1]));

                var res1 = _crmServiceClient.Create(GPConnectAppointment);
                Console.WriteLine(res1);

                var appointmentLookup = res1.ToString();


                var slotAddBooked = SetAllSlotAsBooked(SlotsNumberIds, appointmentLookup);

                var appointmentNo = GetAppointmentNumber(appointmentLookup);
                var bookedAppointmentJSON = ReadAnAppointment(appointmentNo, "Internal");

                if (Prefer == "return=minimal")
                {
                    finaljson[0] = "";
                    finaljson[1] = "";
                    finaljson[2] = "200";
                    return finaljson;
                }

                finaljson[0] = bookedAppointmentJSON;
                finaljson[1] = "";
                finaljson[2] = "200";
                return finaljson;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic ReadAnAppointment(string appoinbtmentId, string type)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];

                AppointmentByIdDTO appointmentDetails = new AppointmentByIdDTO();
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                var AppontmentXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='appointment'>
                                 <attribute name='subject' />
                                 <attribute name='statecode' />
                                 <attribute name='scheduledstart' />
                                 <attribute name='scheduledend' />
                                 <attribute name='createdby' />
                                 <attribute name='bcrm_status' />
                                 <attribute name='bcrm_cancellationreason' />
                                 <attribute name='regardingobjectid' />
                                 <attribute name='activityid' />
                                 <attribute name='instancetypecode' />
                                 <attribute name='bcrm_servicename' />
                                 <attribute name='bcrm_ra_visit_number' />
                                 <attribute name='bcrm_notes' />
                                 <attribute name='bcrm_slotsequencelist' />
                                 <attribute name='description' />
                                 <attribute name='createdon' />
                                <attribute name='bcrm_preferredcontactmethod' />
                                 <attribute name='bcrm_appointmentno' />
                                 <order attribute='subject' descending='false' />
                                <filter type='and'>
                                   <condition attribute='bcrm_appointmentno' operator='eq' value='" + appoinbtmentId + @"' />
                                 </filter>
                                 <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_clinic' visible='false' link-type='outer' alias='organisation'>
                                   <attribute name='bcrm_gpc_telecom_value' />
                                   <attribute name='bcrm_gpc_sequence_number' />
                                   <attribute name='bcrm_clinictype' />     
                                   <attribute name='bcrm_odscode' />
                                   <attribute name='bcrm_name' />
                                 </link-entity>
                                 <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_clinician' visible='false' link-type='outer' alias='practitioner'>
                                   <attribute name='bcrm_staffid' />
                                   <attribute name='bcrm_gpc_sequence_number' />
                                   <attribute name='bcrm_email' />
                                   <attribute name='bcrm_gpc_sdsid' />
                                 </link-entity>
                                 <link-entity name='msemr_slot' from='msemr_slotid' to='bcrm_slot' visible='false' link-type='outer' alias='slot'>
                                   <attribute name='msemr_appointmenttypenew' />
                                   <attribute name='msemr_appointmentemrid' />
                                   <attribute name='bcrm_sequencenumber' />
                                 </link-entity>
                                 <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gplocation' visible='false' link-type='outer' alias='location'>
                                   <attribute name='bcrm_gpcsequencenumber' />
                                 </link-entity>
                                 <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='patient'>
                                   <attribute name='bcrm_membershipstatus' />
                                   <attribute name='bcrm_gpc_sequence_number' />
                                 </link-entity>
                               </entity>
                             </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(AppontmentXml));



                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    AppointmentByIdDTO appDetails = new AppointmentByIdDTO();
                    appDetails.appointmnetId = record.Attributes.Contains("bcrm_appointmentno") ? record["bcrm_appointmentno"].ToString() : "1";
                    appDetails.versionNumber = record.Attributes.Contains("bcrm_ra_visit_number") ? record["bcrm_ra_visit_number"].ToString() : "1";
                    appDetails.Status = record.Attributes.Contains("bcrm_status") ? record.FormattedValues["bcrm_status"].ToString() : "";
                    appDetails.ServiceCategory = record.Attributes.Contains("bcrm_servicename") ? record["bcrm_servicename"].ToString() : "";
                    appDetails.Description = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                    appDetails.slotList = record.Attributes.Contains("bcrm_slotsequencelist") ? record["bcrm_slotsequencelist"].ToString() : "";

                    appDetails.comments = record.Attributes.Contains("bcrm_notes") ? record["bcrm_notes"].ToString() : null;
                    appDetails.CancellationReson = record.Attributes.Contains("bcrm_cancellationreason") ? record["bcrm_cancellationreason"].ToString() : "1";

                    if (record.Attributes.Contains("createdon")) { appDetails.createdOn = (DateTime)record.Attributes["createdon"]; }
                    if (record.Attributes.Contains("scheduledstart")) { appDetails.startDate = (DateTime)record.Attributes["scheduledstart"]; }
                    if (record.Attributes.Contains("scheduledend")) { appDetails.endDate = (DateTime)record.Attributes["scheduledend"]; }

                    appDetails.organizationType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString() : "";
                    appDetails.DeleiveryChannel = record.Attributes.Contains("bcrm_preferredcontactmethod") ? record.FormattedValues["bcrm_preferredcontactmethod"].ToString() : "";

                    if (appDetails.DeleiveryChannel.Contains("Telephone"))
                    {
                        appDetails.DeleiveryChannel = "Telephone";
                    }

                    if (appDetails.DeleiveryChannel.Contains("Video"))
                    {
                        appDetails.DeleiveryChannel = "Video";
                    }


                    dynamic organizationSequence = record.Attributes.Contains("organisation.bcrm_gpc_sequence_number") ? record["organisation.bcrm_gpc_sequence_number"] : "";
                    appDetails.organizationId = organizationSequence.Value.ToString();

                    dynamic odsCode = record.Attributes.Contains("organisation.bcrm_odscode") ? record["organisation.bcrm_odscode"] : "";
                    appDetails.odsCode = odsCode.Value.ToString();

                    dynamic organizationName = record.Attributes.Contains("organisation.bcrm_name") ? record["organisation.bcrm_name"] : "";
                    appDetails.organizationName = organizationName.Value.ToString();

                    dynamic organizationTelecome = record.Attributes.Contains("organisation.bcrm_gpc_telecom_value") ? record["organisation.bcrm_gpc_telecom_value"] : "";
                    appDetails.PhoneNumber = organizationTelecome.Value.ToString();

                    dynamic patientRef = record.Attributes.Contains("patient.bcrm_gpc_sequence_number") ? record["patient.bcrm_gpc_sequence_number"] : "";
                    appDetails.patientReference = patientRef.Value.ToString();

                    dynamic slotRef = record.Attributes.Contains("slot.bcrm_sequencenumber") ? record["slot.bcrm_sequencenumber"] : "";
                    appDetails.SlotReference = slotRef.Value.ToString();

                    dynamic practRef = record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number") ? record["practitioner.bcrm_gpc_sequence_number"] : "";
                    appDetails.PractionerReference = practRef.Value.ToString();

                    dynamic locRef = record.Attributes.Contains("location.bcrm_gpcsequencenumber") ? record["location.bcrm_gpcsequencenumber"] : "";
                    appDetails.locationReference = locRef.Value.ToString();

                    var roles = GetOnlyStaffJobRole(appDetails.PractionerReference);
                    appDetails.PractionerDisplayRole = roles.resource.extension[0].valueCodeableConcept.coding[0].display;
                    appDetails.PractitionerRoleCode = roles.resource.extension[0].valueCodeableConcept.coding[0].code;

                    if (type == "External" && appDetails.startDate < DateTime.UtcNow.AddDays(-1))
                    {
                        finaljson[0] = abai.InvalidResourceFoundWhenPastAppointmentAccessJSON("You can't access past appointment.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }

                    if (appDetails.comments == "")
                    {
                        appDetails.comments = null;
                    }
                    if (appDetails.Description == "")
                    {
                        appDetails.Description = null;
                    }

                    AppointmentByAppoIDDetails app = new AppointmentByAppoIDDetails();

                    var result = "";
                    if (appDetails.Status.ToString().ToLower() == "cancelled")
                    {
                        result = app.GetJSONBYCancelAppointmentId(appDetails);
                    }
                    else
                    {
                        result = app.GetJSONBYAppointmentId(appDetails);
                    }

                    if (result.Contains("\"comment\": \"\""))
                    {
                        result = result.Replace("\"comment\": \"\",", "");
                    }

                    if (result.Contains("\"description\": \"\""))
                    {
                        result = result.Replace("\"description\": \"\",", "");
                    }

                    dynamic res;
                    if (appDetails.organizationType.ToString().ToLower().Contains("other"))
                    {
                        res = JsonConvert.DeserializeObject<AppointmentGetByReverseDTOWithoutType>(result);
                    }
                    else
                    {
                        res = JsonConvert.DeserializeObject<AppointmentGetByReverseDTO>(result);
                    }

                    if (res != null)
                    {
                        
                        if (appDetails.slotList != null)
                        {
                            var allslotList = JsonConvert.DeserializeObject<List<string>>(appDetails.slotList);
                            if(allslotList.Count > 1)
                            {
                                for (var i = 1; i < allslotList.Count; i++)
                                {
                                    AppointmentGetByReverseDTOSlot ddd = new AppointmentGetByReverseDTOSlot();
                                    ddd.reference = "Slot/"+ allslotList[i];
                                    res.slot.Add(ddd);
                                }
                            }
                        }
                    }

                    List<object> allobject = new List<object>();

                    JObject firstobjVal = JObject.Parse(res.extension[0].ToString());
                    firstObjectOfExtensionApp ab = new firstObjectOfExtensionApp();
                    ab.url = (string)firstobjVal["url"];
                    ab.valueReference = new firstObjectOfExtensionAppValueReference();
                    ab.valueReference.reference = (string)firstobjVal["valueReference"]["reference"];
                    allobject.Add(ab);

                    JObject SecondobjVal = JObject.Parse(res.extension[1].ToString());

                    SecondObjectOfExtensionApp so = new SecondObjectOfExtensionApp();
                    so.url = (string)SecondobjVal["url"];
                    so.valueCodeableConcept = new SecondObjectOfExtensionAppValueCodeableConcept();

                    so.valueCodeableConcept.coding = new List<SecondObjectOfExtensionAppCoding>();
                    var coding = new SecondObjectOfExtensionAppCoding();
                    coding.system = (string)SecondobjVal["valueCodeableConcept"]["coding"][0]["system"];
                    coding.code = (string)SecondobjVal["valueCodeableConcept"]["coding"][0]["code"];
                    coding.display = (string)SecondobjVal["valueCodeableConcept"]["coding"][0]["display"];

                    so.valueCodeableConcept.coding.Add(coding);

                    allobject.Add(so);

                    JObject ThirdobjVal = JObject.Parse(res.extension[2].ToString());
                    ThirdObjectOfExtensionApp to = new ThirdObjectOfExtensionApp();
                    to.url = (string)ThirdobjVal["url"];
                    to.valueCode = (string)ThirdobjVal["valueCode"];

                    allobject.Add(to);

                    if (appDetails.Status.ToString().ToLower() == "cancelled")
                    {
                        JObject fourthObj = JObject.Parse(res.extension[3].ToString());
                        FourthObjectOfExtensionApp po = new FourthObjectOfExtensionApp();
                        po.url = (string)fourthObj["url"];
                        po.valueString = (string)fourthObj["valueString"];

                        allobject.Add(po);
                    }
                    res.extension = allobject;

                    if (type == "Internal")
                    {
                        return res;
                    }


                    finaljson[0] = res;
                    finaljson[1] = appDetails.versionNumber;
                    finaljson[2] = "200";
                    return finaljson;
                }



                finaljson[0] = abai.BadRequestAppointmentJSON(appoinbtmentId);
                finaljson[1] = "";
                finaljson[2] = "404";
                return finaljson;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic GetAppointmentGetByPatientId(string patientId, string fromDate, string toDate, string SspInterctionId, string token)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                if (!SspInterctionId.Contains("appointment"))
                {
                    finaljson[0] = abai.BadRequestSSPInteractionIdNotMatchedJSON();
                    finaljson[1] = "";
                    finaljson[2] = "400";
                    return finaljson;
                }
                if (token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token.Replace("Bearer ", ""));

                    // Extract requested_scope value
                    var requestedScope = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "requested_scope")?.Value;

                    if (!requestedScope.Contains("patient/*.read"))
                    {
                        finaljson[0] = abai.BadRequestJWTTokenNotMatchedJSON();
                        finaljson[1] = "";
                        finaljson[2] = "400";
                        return finaljson;
                    }
                }



                fromDate = fromDate.Replace("ge", "");
                toDate = toDate.Replace("le", "");

                var stratDateCheckingStatus = IsValidDate(fromDate);
                var endDateCheckingStatus = IsValidDate(toDate);

                if (stratDateCheckingStatus == false || endDateCheckingStatus == false)
                {
                    finaljson[0] = abai.InvalidParameterDateFormatJSON();
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                var startDat = DateTime.Parse(fromDate);
                if (startDat.AddDays(1) < DateTime.UtcNow)
                {
                    finaljson[0] = abai.InvalidParameterStartDateShouldNotBePassedDateJSON();
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }



                if (DateTime.Parse(fromDate) > DateTime.Parse(toDate))
                {

                    finaljson[0] = abai.InvalidParameterStartDateIsGreaterThanEndDateJSON();
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }


                var appoinXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='appointment'>
                                    <attribute name='subject' />
                                    <attribute name='statecode' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='createdby' />
                                    <attribute name='regardingobjectid' />
                                    <attribute name='activityid' />
                                    <attribute name='instancetypecode' />
                                    <attribute name='bcrm_appointmentno' />
                                    <order attribute='subject' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='scheduledstart' operator='on-or-after' value='" + fromDate + @"' />
                                      <condition attribute='scheduledstart' operator='on-or-before' value='" + toDate + @"' />
                                      <condition attribute='bcrm_locationname' operator='eq' value='GP-CONNECT' />
                                    </filter>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='inner' alias='ae'>
                                      <filter type='and'>
                                        <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + patientId + @"' />
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(appoinXML));
                AppointmentGetByPatientIdDTO appDetails = new AppointmentGetByPatientIdDTO();
                appDetails.resourceType = "Bundle";
                appDetails.type = "searchset";

                AppointmentGetByReverseDTOMeta1 am = new AppointmentGetByReverseDTOMeta1();
                am.lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

                appDetails.meta = am;

                List<dynamic> appDTO = new List<dynamic>();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var suppTotalAppo = AnswerCollection.Entities.Count;

                    for (var i = 0; i < AnswerCollection.Entities.Count; i++)
                    {
                        var appointmentNo = AnswerCollection.Entities[i].Attributes.Contains("bcrm_appointmentno") ? AnswerCollection.Entities[i]["bcrm_appointmentno"].ToString() : "";
                        if (appointmentNo != "")
                        {
                            var appDetails1 = ReadAnAppointment(appointmentNo, "Internal");
                            var resource = new Dictionary<string, object>
                            {
                               { "resource",  appDetails1}
                            };

                            appDTO.Add(resource);
                        }
                    }
                    appDetails.entry = appDTO;
                    finaljson[0] = appDetails;
                    finaljson[1] = "";
                    finaljson[2] = "200";
                    return finaljson;
                }
                appDetails.entry = new List<dynamic>();
                finaljson[0] = appDetails;
                finaljson[1] = "";
                finaljson[2] = "200";
                return finaljson;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic UpdateAppointment(string IfMathch, RequestBookAppointmentDTO bookAppointment, string sspInterectionId, string BookAppointmentDTOString)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                if (BookAppointmentDTOString.Contains("appointmentType"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Invalid Field : AppointmentType");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }

                if (sspInterectionId == "urn:nhs:names:services:gpconnect:fhir:rest:cancel:appointment-1")
                {
                    var res = CancelAppointment(bookAppointment, IfMathch);
                    return res;
                }

                if (bookAppointment.resourceType != "Appointment")
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Resource Type is Invalid");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (bookAppointment.status.ToString().ToLower() != "booked")
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Passing Status Is Invalid :" + bookAppointment.status);
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (BookAppointmentDTOString.Contains("https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-AppointmentCancellationReason-1"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Cancellation Reason Not Allowed To Update.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (BookAppointmentDTOString.Contains("minutesDuration"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("You can't update minute Duration.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (BookAppointmentDTOString.Contains("priority"))
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("You Can't Update Priority.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }
                if (bookAppointment.extension != null)
                {
                    for (var i = 0; i < bookAppointment.extension.Count; i++)
                    {
                        if (bookAppointment.extension[i].url == "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-BookingOrganisation-1")
                        {
                            if (bookAppointment.extension[i].valueReference.reference.ToString().Contains("https"))
                            {
                                finaljson[0] = abai.InvalidResourceFoundJSON("Organization Id is not proper format.");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                        }
                    }
                }

                var appointmendId = bookAppointment.id;
                var AppontmentXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='appointment'>
                                 <attribute name='subject' />
                                 <attribute name='statecode' />
                                 <attribute name='bcrm_status' />
                                 <attribute name='regardingobjectid' />
                                 <attribute name='activityid' />
                                 <attribute name='instancetypecode' />
                                 <attribute name='bcrm_servicename' />
                                 <attribute name='bcrm_ra_visit_number' />
                                 <attribute name='bcrm_notes' />
                                 <attribute name='scheduledstart' />
                                 <attribute name='description' />
                                 <attribute name='createdon' />
                                 <attribute name='bcrm_preferredcontactmethod' />
                                 <attribute name='bcrm_appointmentno' />
                                 <order attribute='subject' descending='false' />
                                <filter type='and'>
                                   <condition attribute='bcrm_appointmentno' operator='eq' value='" + appointmendId + @"' />
                                 </filter>
                               </entity>
                             </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(AppontmentXml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var appointmentStatus = record.Attributes.Contains("bcrm_status") ? record.FormattedValues["bcrm_status"].ToString() : "";
                    var versionNumber = record.Attributes.Contains("bcrm_ra_visit_number") ? record["bcrm_ra_visit_number"].ToString() : "";
                    var appLookup = record.Attributes.Contains("activityid") ? record["activityid"].ToString() : "";

                    if (record.Attributes.Contains("scheduledstart"))
                    {
                        var startDate = (DateTime)record.Attributes["scheduledstart"];
                        if (startDate < DateTime.UtcNow)
                        {
                            finaljson[0] = abai.InvalidResourceFoundJSON("cannot update a past appointment.");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }

                    if (IfMathch != "")
                    {
                        if (IfMathch != versionNumber)
                        {
                            finaljson[0] = abai.IfMatchIsNotOkJSON();
                            finaljson[1] = "";
                            finaljson[2] = "409";
                            return finaljson;
                        }
                    }


                    if (appointmentStatus.ToString().ToLower() != "booked")
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("you are able to update only booked appointment.");
                        finaljson[1] = versionNumber;
                        finaljson[2] = "422";
                        return finaljson;
                    }
                    else
                    {
                        if (appLookup != null)
                        {
                            var comments = bookAppointment.comment;
                            var description = bookAppointment.description;
                            var updatedversion = int.Parse(versionNumber) + 1;
                            if (comments != null)
                            {
                                if (comments.Length > 499)
                                {
                                    finaljson[0] = abai.InvalidResourceFoundJSON("Comments too long");
                                    finaljson[1] = versionNumber;
                                    finaljson[2] = "422";
                                    return finaljson;
                                }
                            }

                            if (description != null)
                            {
                                if (description.Length > 100)
                                {
                                    finaljson[0] = abai.InvalidResourceFoundJSON("Description too long.");
                                    finaljson[1] = versionNumber;
                                    finaljson[2] = "422";
                                    return finaljson;
                                }
                            }


                            Entity GPConnectAppointment = new Entity("appointment", new Guid(appLookup));
                            GPConnectAppointment["bcrm_notes"] = comments;
                            GPConnectAppointment["description"] = description;
                            GPConnectAppointment["bcrm_ra_visit_number"] = updatedversion.ToString();
                            _crmServiceClient.Update(GPConnectAppointment);

                            var result = ReadAnAppointment(appointmendId, "Internal");
                            finaljson[0] = result;
                            finaljson[1] = updatedversion;
                            finaljson[2] = "200";
                            return finaljson;
                        }
                    }
                }

                finaljson[0] = abai.InvalidResourceFoundJSON("appointment id not exist.");
                finaljson[1] = "";
                finaljson[2] = "404";
                return finaljson;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic CancelAppointment(RequestBookAppointmentDTO bookAppointment, string IfMathch)
        {
            try
            {

                dynamic[] finaljson = new dynamic[3];
                AppointmentByAppoIDDetails abai = new AppointmentByAppoIDDetails();

                if (bookAppointment.status != null)
                {
                    if (bookAppointment.status.ToString().ToLower() != "cancelled")
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Status must be cancelled.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }
                }
                else
                {
                    finaljson[0] = abai.InvalidResourceFoundJSON("Status element is missing.");
                    finaljson[1] = "";
                    finaljson[2] = "422";
                    return finaljson;
                }


                if (bookAppointment.participant != null)
                {
                    for (var i = 0; i < bookAppointment.participant.Count; i++)
                    {
                        if (bookAppointment.participant[i].status == "declined")
                        {
                            finaljson[0] = abai.InvalidResourceFoundJSON("participant status should not be declined");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }
                }
                if (bookAppointment.extension != null)
                {
                    for (var i = 0; i < bookAppointment.extension.Count; i++)
                    {
                        if (bookAppointment.extension[i].url == "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-BookingOrganisation-1")
                        {
                            if (bookAppointment.extension[i].valueReference.reference.ToString().Contains("https"))
                            {
                                finaljson[0] = abai.InvalidResourceFoundJSON("Organization Id is not proper format.");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                        }
                    }
                }

                var appointmendId = bookAppointment.id;


                var AppontmentXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='appointment'>
                                 <attribute name='subject' />
                                 <attribute name='statecode' />
                                 <attribute name='bcrm_status' />
                                 <attribute name='regardingobjectid' />
                                 <attribute name='activityid' />
                                 <attribute name='instancetypecode' />
                                 <attribute name='bcrm_servicename' />
                                 <attribute name='bcrm_ra_visit_number' />
                                 <attribute name='bcrm_notes' />
                                 <attribute name='scheduledstart' />
                                 <attribute name='description' />
                                 <attribute name='createdon' />
                                <attribute name='bcrm_slot' />
                                <attribute name='bcrm_preferredcontactmethod' />
                                 <attribute name='bcrm_appointmentno' />
                                 <order attribute='subject' descending='false' />
                                <filter type='and'>
                                   <condition attribute='bcrm_appointmentno' operator='eq' value='" + appointmendId + @"' />
                                 </filter>
                               </entity>
                             </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(AppontmentXml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var appointmentStatus = record.Attributes.Contains("bcrm_status") ? record.FormattedValues["bcrm_status"].ToString() : "";
                    var versionNumber = record.Attributes.Contains("bcrm_ra_visit_number") ? record["bcrm_ra_visit_number"].ToString() : "";
                    var description = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                    var comments = record.Attributes.Contains("bcrm_notes") ? record["bcrm_notes"].ToString() : "";
                    var appLookup = record.Attributes.Contains("activityid") ? record["activityid"].ToString() : "";

                    if (IfMathch != "")
                    {
                        if (IfMathch != versionNumber)
                        {
                            finaljson[0] = abai.IfMatchFHIRIsNotOkJSON();
                            finaljson[1] = "";
                            finaljson[2] = "409";
                            return finaljson;
                        }
                    }

                    if (bookAppointment.comment != "")
                    {
                        if (bookAppointment.comment != null)
                        {
                            if (comments.ToString().ToLower() != bookAppointment.comment.ToString().ToLower())
                            {
                                finaljson[0] = abai.InvalidResourceFoundJSON("You cannot update the comment element. only the appointment status and cancellation reason elements can be modified.");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                        }

                    }
                    if (bookAppointment.description != "")
                    {
                        if (bookAppointment.description != null)
                        {
                            if (description.ToString().ToLower() != bookAppointment.description.ToString().ToLower())
                            {
                                finaljson[0] = abai.InvalidResourceFoundJSON("You cannot update the description element. only the appointment status and cancellation reason elements can be modified.");
                                finaljson[1] = "";
                                finaljson[2] = "422";
                                return finaljson;
                            }
                        }
                    }
                    if (record.Attributes.Contains("scheduledstart"))
                    {
                        var startDate = (DateTime)record.Attributes["scheduledstart"];
                        if (startDate < DateTime.UtcNow)
                        {
                            finaljson[0] = abai.InvalidResourceFoundJSON("can't be cancel past appointment");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                    }
                    else if (appointmentStatus.ToString().ToLower() != "booked")
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("The appointment must be in 'booked' status for cancel appointment, but its current status is '" + appointmentStatus + ".'");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }

                    var cancelReasonSta = false;
                    foreach (var item in bookAppointment.extension)
                    {
                        if (item.url == "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-AppointmentCancellationReason-1")
                        {
                            cancelReasonSta = true;
                        }
                    }
                    if (cancelReasonSta == false)
                    {
                        finaljson[0] = abai.InvalidResourceFoundJSON("Cancellation Reason must not be empty.");
                        finaljson[1] = "";
                        finaljson[2] = "422";
                        return finaljson;
                    }

                    if (appLookup != null)
                    {



                        var updatedversion = int.Parse(versionNumber) + 1;


                        Entity GPConnectAppointment = new Entity("appointment", new Guid(appLookup));
                        GPConnectAppointment["bcrm_status"] = new OptionSetValue(4);
                        GPConnectAppointment["bcrm_ra_visit_number"] = updatedversion.ToString();

                        var CancellationReasonChecker = false;
                        var CancellationText = "";
                        foreach (var item in bookAppointment.extension)
                        {
                            if (item.url == "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-AppointmentCancellationReason-1")
                            {
                                CancellationReasonChecker = true;
                                CancellationText = item.valueString;
                                GPConnectAppointment["bcrm_cancellationreason"] = item.valueString;
                            }
                        }

                        if (CancellationReasonChecker == false || CancellationText == "")
                        {
                            finaljson[0] = abai.InvalidParameterJSON("Cancellation Reason must not be empty.");
                            finaljson[1] = "";
                            finaljson[2] = "422";
                            return finaljson;
                        }
                        if (record.Attributes.Contains("bcrm_slot"))
                        {
                            dynamic slotLookup = record["bcrm_slot"];
                            var id = slotLookup.Id.ToString();
                            Entity slotUpdate = new Entity("msemr_slot", new Guid(id));
                            slotUpdate["bcrm_slotstatus"] = new OptionSetValue(101);
                            _crmServiceClient.Update(slotUpdate);
                        }

                        _crmServiceClient.Update(GPConnectAppointment);
                        var result = ReadAnAppointment(appointmendId, "Internal");

                        finaljson[0] = result;
                        finaljson[1] = updatedversion;
                        finaljson[2] = "200";
                        return finaljson;
                    }



                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        #endregion

        #region Internal-Method

        internal SlotDTO GetSlotDTO()
        {
            var slotDetails = @"{
                                    ""resource"": {
                                      ""resourceType"": ""Slot"",
                                      ""id"": ""1584"",
                                      ""meta"": {
                                        ""versionId"": ""1471219260000"",
                                        ""profile"": [
                                          ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Slot-1""
                                        ]
                                      },
                                      ""extension"": [
                                        {
                                          ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                          ""valueCode"": ""In-person""
                                        }
                                      ],
                                      ""serviceType"": [
                                        {
                                          ""text"": ""General GP Appointment""
                                        }
                                      ],
                                      ""schedule"": {
                                        ""reference"": ""Schedule/14""
                                      },
                                      ""status"": ""free"",
                                      ""start"": ""2016-08-15T11:30:00+01:00"",
                                      ""end"": ""2016-08-15T11:40:00+01:00""
                                    }
                                  }";

            var slotData = JsonConvert.DeserializeObject<SlotDTO>(slotDetails);
            return slotData;
        }

        internal ScheduleDTO GetScheduleDTO()
        {
            var scheduleDetails = @" {
                                  ""resource"": {
                                    ""resourceType"": ""Schedule"",
                                    ""id"": ""14"",
                                    ""meta"": {
                                      ""versionId"": ""1469444400000"",
                                      ""profile"": [
                                        ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Schedule-1""
                                      ]
                                    },
                                    ""extension"": [
                                      {
                                        ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-PractitionerRole-1"",
                                        ""valueCodeableConcept"": {
                                          ""coding"": [
                                            {
                                              ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1"",
                                              ""code"": ""R0260"",
                                              ""display"": ""General Medical Practitioner""
                                            }
                                          ]
                                        }
                                      }
                                    ],
                                    ""serviceCategory"": {
                                      ""text"": ""General GP Appointments""
                                    },
                                    ""actor"": [
                                      {
                                        ""reference"": ""Location/17""
                                      },
                                      {
                                        ""reference"": ""Practitioner/2""
                                      }
                                    ],
                                    ""planningHorizon"": {
                                      ""start"": ""2016-08-15T09:00:00+01:00"",
                                      ""end"": ""2016-08-15T12:00:00+01:00""
                                    }
                                  }
                                }";
            var scheduletData = JsonConvert.DeserializeObject<ScheduleDTO>(scheduleDetails);
            return scheduletData;
        }

        internal AppointmentPractitionerDTO GetPractitionerDTO()
        {
            var practitionerDetails = @"  {
                                             ""resource"": {
                                               ""resourceType"": ""Practitioner"",
                                               ""id"": ""2"",
                                               ""meta"": {
                                                 ""versionId"": ""636064088099800115"",
                                                 ""profile"": [
                                                   ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Practitioner-1""
                                                 ]
                                               },
                                               ""identifier"": [
                                                 {
                                                   ""system"": ""https://fhir.nhs.uk/Id/sds-user-id"",
                                                   ""value"": ""111122223333""
                                                 }
                                               ],
                                               ""name"": [
                                                 {
                                                   ""family"": ""Black"",
                                                   ""given"": [
                                                     ""Sarah""
                                                   ],
                                                   ""prefix"": [
                                                     ""Mrs""
                                                   ]
                                                 }
                                               ],
                                               ""gender"": ""female""
                                             }
                                           }";
            var practitionerData = JsonConvert.DeserializeObject<AppointmentPractitionerDTO>(practitionerDetails);
            return practitionerData;

        }

        internal AppointmentLocationDTO GetLocationDTO()
        {
            string locationDetails = @"  {
                                          ""resource"": {
                                            ""resourceType"": ""Location"",
                                            ""id"": ""17"",
                                            ""meta"": {
                                              ""versionId"": ""636064088100870233"",
                                              ""profile"": [
                                                ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1""
                                              ]
                                            },
                                            ""name"": ""The Trevelyan Practice"",
                                            ""address"": {
                                              ""line"": [
                                                ""Trevelyan Square"",
                                                ""Boar Ln"",
                                                ""Leeds""
                                              ],
                                              ""postalCode"": ""LS1 6AE""
                                            },
                                            ""telecom"": {
                                              ""system"": ""phone"",
                                              ""value"": ""03003035678"",
                                              ""use"": ""work""
                                            },
                                            ""managingOrganization"": {
                                              ""reference"": ""Organization/23""
                                            }
                                          }
                                        }";
            var locationsDetails = JsonConvert.DeserializeObject<AppointmentLocationDTO>(locationDetails);
            return locationsDetails;


        }

        internal AppointmentOrganizationDTO GetOrganizationDTO()
        {
            string organizationDetails = @"{
                                          ""resource"": {
                                            ""resourceType"": ""Organization"",
                                            ""id"": ""23"",
                                            ""meta"": {
                                              ""versionId"": ""636064088098730113"",
                                              ""profile"": [
                                                ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                              ]
                                            },
                                            ""identifier"": [
                                              {
                                                ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                                ""value"": ""A00001""
                                              }
                                            ],
                                            ""name"": ""The Trevelyan Practice"",
                                            ""address"": {
                                              ""line"": [
                                                ""Trevelyan Square"",
                                                ""Boar Ln""
                                              ],
                                              ""city"": ""Leeds"",
                                              ""district"": ""West Yorkshire"",
                                              ""postalCode"": ""LS1 6AE""
                                            },
                                            ""telecom"": {
                                              ""system"": ""phone"",
                                              ""value"": ""03003035678"",
                                              ""use"": ""work""
                                            }
                                          }
                                        }";
            var organizationsDetails = JsonConvert.DeserializeObject<AppointmentOrganizationDTO>(organizationDetails);
            return organizationsDetails;
        }

        internal AppointmentGetByReverseDTO GetReverseAppointment()
        {
            var xml = @"{
                                    ""resourceType"": ""Appointment"",
                                    ""id"": ""148"",
                                    ""meta"": {
                                      ""versionId"": ""1503310820000"",
                                      ""profile"": [
                                        ""https://fhir.nhs.uk/STU3/StructureDefinition/GPConnect-Appointment-1""
                                      ]
                                    },
                                    ""contained"": [
                                      {
                                        ""resourceType"": ""Organization"",
                                        ""id"": ""1"",
                                        ""meta"": {
                                          ""profile"": [
                                            ""https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Organization-1""
                                          ]
                                        },
                                        ""identifier"": [
                                          {
                                            ""system"": ""https://fhir.nhs.uk/Id/ods-organization-code"",
                                            ""value"": ""A00001""
                                          }
                                        ],
                                        ""type"": [
                                          {
                                            ""coding"": [
                                              {
                                                ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/GPConnect-OrganisationType-1"",
                                                ""code"": ""gp-practice""
                                              }
                                            ]
                                          }
                                        ],
                                        ""name"": ""Test Organization Name"",
                                        ""telecom"": [
                                          {
                                            ""system"": ""phone"",
                                            ""value"": ""0300 303 5678""
                                          }
                                        ]
                                      }
                                    ],
                                    ""extension"": [
                                      {
                                        ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-BookingOrganisation-1"",
                                        ""valueReference"": {
                                          ""reference"": ""#1""
                                        }
                                      },
                                      {
                                        ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-PractitionerRole-1"",
                                        ""valueCodeableConcept"": {
                                          ""coding"": [
                                            {
                                              ""system"": ""https://fhir.nhs.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1"",
                                              ""code"": ""R0260"",
                                              ""display"": ""General Medical Practitioner""
                                            }
                                          ]
                                        }
                                      },
                                      {
                                        ""url"": ""https://fhir.nhs.uk/STU3/StructureDefinition/Extension-GPConnect-DeliveryChannel-2"",
                                        ""valueCode"": ""In-person""
                                      }
                                    ],
                                    ""status"": ""booked"",
                                    ""serviceCategory"": {
                                      ""text"": ""General GP Appointments""
                                    },
                                    ""serviceType"": [
                                      {
                                        ""text"": ""General GP Appointment""
                                      }
                                    ],
                                    ""description"": ""GP Connect Appointment description 148"",
                                    ""start"": ""2017-08-21T10:20:00+01:00"",
                                    ""end"": ""2017-08-21T10:50:00+01:00"",
                                    ""slot"": [
                                      {
                                        ""reference"": ""Slot/544""
                                      }
                                    ],
                                    ""created"": ""2017-07-09T13:48:41+01:00"",
                                    ""comment"": ""Test Appointment Comment 148"",
                                    ""participant"": [
                                      {
                                        ""actor"": {
                                          ""reference"": ""Patient/2""
                                        },
                                        ""status"": ""accepted""
                                      },
                                      {
                                        ""actor"": {
                                          ""reference"": ""Location/1""
                                        },
                                        ""status"": ""accepted""
                                      },
                                      {
                                        ""actor"": {
                                          ""reference"": ""Practitioner/2""
                                        },
                                        ""status"": ""accepted""
                                      }
                                    ]
                                  }";
            JObject appointmentJson = JObject.Parse(xml);
            dynamic js = JsonConvert.DeserializeObject<dynamic>(xml);
            var appoint = JsonConvert.DeserializeObject<AppointmentGetByReverseDTO>(xml);

            var js1 = js.ToObject<AppointmentGetByReverseDTO>();
            return appoint;

        }

        internal ResponseBookAppointmentDTO GetResponseBookAppointment(string appointmentId)
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string GetSheduleJSON(List<int> scheduleSequenceList)
        {
            try
            {

                List<ScheduleDTO> scheduleDTOList = new List<ScheduleDTO>();
                foreach (var sequenceNo in scheduleSequenceList)
                {
                    var scheduleXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bcrm_shifts'>
                                    <attribute name='bcrm_shiftsid' />
                                    <attribute name='bcrm_name' />
                                    <attribute name='createdon' />
                                    <attribute name='bcrm_versionnumber' />
                                    <attribute name='bcrm_sequencenumber' />
                                    <attribute name='bcrm_service' />
                                    <attribute name='bcrm_staff' />
                                    <attribute name='bcrm_gplocation' />
                                    <attribute name='bcrm_todate' />
                                    <attribute name='bcrm_fromdate' />
                                    <order attribute='bcrm_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bcrm_sequencenumber' operator='eq' value='" + sequenceNo + @"' />
                                    </filter>
                                   <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_staff' visible='false' link-type='outer' alias='practitioner'>
                                       <attribute name='bcrm_gpc_sequence_number' />
                                     </link-entity>
                                     <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gplocation' visible='false' link-type='outer' alias='loaction'>
                                       <attribute name='bcrm_gpcsequencenumber' />
                                     </link-entity>
                                  </entity>
                                </fetch>";

                    EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(scheduleXML));
                    if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                    {
                        var record = AnswerCollection.Entities[0];
                        ScheduleDTO scheduleDTO = GetScheduleDTO();
                        scheduleDTO.resource.id = record.Attributes.Contains("bcrm_sequencenumber") ? record["bcrm_sequencenumber"].ToString() : "1";
                        scheduleDTO.resource.meta.versionId = record.Attributes.Contains("bcrm_versionnumber") ? record["bcrm_versionnumber"].ToString() : "1";



                        dynamic service = record.Attributes.Contains("bcrm_service") ? record["bcrm_service"] : string.Empty;
                        scheduleDTO.resource.serviceCategory.text = service.Name;

                        dynamic locationSequence = record.Attributes.Contains("loaction.bcrm_gpcsequencenumber") ? record["loaction.bcrm_gpcsequencenumber"] : string.Empty;
                        scheduleDTO.resource.actor[0].reference = "Location/" + locationSequence.Value;

                        dynamic practionerSequence = record.Attributes.Contains("practitioner.bcrm_gpc_sequence_number") ? record["practitioner.bcrm_gpc_sequence_number"] : string.Empty;
                        scheduleDTO.resource.actor[1].reference = "Practitioner/" + practionerSequence.Value;

                        if (record.Attributes.Contains("bcrm_fromdate")) { scheduleDTO.resource.planningHorizon.start = (DateTime)record.Attributes["bcrm_fromdate"]; }
                        if (record.Attributes.Contains("bcrm_todate")) { scheduleDTO.resource.planningHorizon.end = (DateTime)record.Attributes["bcrm_todate"]; }

                        if (practionerSequence.Value != null)
                        {
                            var staffRoles = GetOnlyStaffJobRole(practionerSequence.Value);
                            if (staffRoles != null)
                            {
                                scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].code = staffRoles.resource.extension[0].valueCodeableConcept.coding[0].code;
                                scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].display = staffRoles.resource.extension[0].valueCodeableConcept.coding[0].display;
                            }
                            else
                            {
                                scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].code = "";
                                scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].display = "";
                            }
                        }

                        scheduleDTOList.Add(scheduleDTO);


                    }
                }
                return new JavaScriptSerializer().Serialize(scheduleDTOList);

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }



        }

        internal ScheduleDTO GetOnlyStaffJobRole(string practitionerSequenceNumber)
        {
            try
            {
                string staffxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bcrm_staff'>
                                      <attribute name='bcrm_staffid' />
                                      <attribute name='bcrm_name' />
                                      <attribute name='createdon' />
                                      <attribute name='bcrm_staffrole' />
                                      <attribute name='bcrm_roles' />
                                      <order attribute='bcrm_name' descending='false' />
                                      <filter type='and'>
                                        <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + practitionerSequenceNumber + @"' />
                                      </filter>
                                      <link-entity name='bcrm_oxwrsecurityroles' from='bcrm_oxwrsecurityrolesid' to='bcrm_roles' visible='false' link-type='outer' alias='roles'>
                                        <attribute name='bcrm_name' />
                                        <attribute name='bcrm_jobrolecode' />
                                      </link-entity>
                                    </entity>
                                  </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(staffxml));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    ScheduleDTO scheduleDTO = GetScheduleDTO();

                    dynamic roleCode = record.Attributes.Contains("roles.bcrm_jobrolecode") ? record["roles.bcrm_jobrolecode"] : string.Empty;
                    scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].code = roleCode.Value;

                    dynamic roleDisplay = record.Attributes.Contains("roles.bcrm_name") ? record["roles.bcrm_name"] : string.Empty;
                    scheduleDTO.resource.extension[0].valueCodeableConcept.coding[0].display = roleDisplay.Value;

                    return scheduleDTO;


                }


                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string GetPractitionerJSON(List<string> PractitionerIdUniqueValue)
        {
            try
            {
                List<AppointmentPractitionerDTO> practionerDetailsList = new List<AppointmentPractitionerDTO>();

                foreach (var practitionerId in PractitionerIdUniqueValue)
                {
                    var practXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='bcrm_staff'>
                                     <attribute name='bcrm_staffid' />
                                     <attribute name='bcrm_name' />
                                     <attribute name='createdon' />
                                     <attribute name='bcrm_gpc_versionid' />
                                     <attribute name='bcrm_gpc_sequence_number' />
                                     <attribute name='bcrm_gpc_sdsid' />
                                     <attribute name='bcrm_gpc_prefix' />
                                     <attribute name='bcrm_gpc_name_given' />
                                     <attribute name='bcrm_gpc_name_family' />
                                     <attribute name='bcrm_gender' />
                                     <order attribute='bcrm_name' descending='false' />
                                     <filter type='and'>
                                       <condition attribute='bcrm_staffid' operator='eq' uitype='bcrm_staff' value='{" + practitionerId + @"}' />
                                     </filter>
                                   </entity>
                                 </fetch>";
                    EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(practXML));
                    if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                    {
                        foreach (var record in AnswerCollection.Entities)
                        {
                            AppointmentPractitionerDTO practitionerDetail = GetPractitionerDTO();
                            practitionerDetail.resource.id = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                            practitionerDetail.resource.meta.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : "1";
                            practitionerDetail.resource.identifier[0].value = record.Attributes.Contains("bcrm_gpc_sdsid") ? record["bcrm_gpc_sdsid"].ToString() : string.Empty;
                            practitionerDetail.resource.name[0].family = record.Attributes.Contains("bcrm_gpc_name_family") ? record["bcrm_gpc_name_family"].ToString() : string.Empty;
                            practitionerDetail.resource.name[0].given[0] = record.Attributes.Contains("bcrm_gpc_name_given") ? record["bcrm_gpc_name_given"].ToString() : string.Empty;
                            practitionerDetail.resource.name[0].prefix[0] = record.Attributes.Contains("bcrm_gpc_prefix") ? record.FormattedValues["bcrm_gpc_prefix"].ToString() : "";
                            practitionerDetail.resource.gender = record.Attributes.Contains("bcrm_gender") ? record.FormattedValues["bcrm_gender"].ToString().ToLower() : "";

                            practionerDetailsList.Add(practitionerDetail);
                        }
                    }


                }
                return new JavaScriptSerializer().Serialize(practionerDetailsList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string GetGPLocationJSON(List<string> GPlocation)
        {
            try
            {
                List<AppointmentLocationDTO> appointmentDetailsDTO = new List<AppointmentLocationDTO>();
                foreach (var locationId in GPlocation)
                {

                    var locationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='bcrm_gpclocation'>
                                  <attribute name='bcrm_gpclocationid' />
                                  <attribute name='bcrm_name' />
                                  <attribute name='createdon' />
                                  <attribute name='bcrm_gpc_managingorganization' />
                                  <attribute name='bcrm_gpc_name' />
                                  <attribute name='bcrm_gpc_versionid' />
                                  <attribute name='bcrm_gpcsequencenumber' />
                                  <attribute name='bcrm_gpc_address_postalcode' />
                                  <attribute name='bcrm_gpc_address_line' />
                                  <order attribute='bcrm_name' descending='false' />
                                  <filter type='and'>
                                    <condition attribute='bcrm_gpclocationid' operator='eq' uitype='bcrm_gpclocation' value='{" + locationId + @"}' />
                                  </filter>
                                  <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_managingorganization' visible='false' link-type='outer' alias='organization'>
                                    <attribute name='bcrm_gpc_sequence_number' />
                                  </link-entity>
                                </entity>
                              </fetch>";
                    EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(locationXML));
                    if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                    {
                        foreach (var record in AnswerCollection.Entities)
                        {
                            AppointmentLocationDTO locationDetails = GetLocationDTO();
                            locationDetails.resource.meta.profile[0] = "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-Location-1";
                            locationDetails.resource.id = record.Attributes.Contains("bcrm_gpcsequencenumber") ? record["bcrm_gpcsequencenumber"].ToString() : string.Empty;
                            locationDetails.resource.meta.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;
                            locationDetails.resource.name = record.Attributes.Contains("bcrm_gpc_name") ? record["bcrm_gpc_name"].ToString() : string.Empty;
                            locationDetails.resource.address.line[0] = record.Attributes.Contains("bcrm_gpc_address_line") ? record["bcrm_gpc_address_line"].ToString() : string.Empty;
                            locationDetails.resource.address.postalCode = record.Attributes.Contains("bcrm_gpc_address_postalcode") ? record["bcrm_gpc_address_postalcode"].ToString() : string.Empty;
                            dynamic organizationId = record.Attributes.Contains("organization.bcrm_gpc_sequence_number") ? record["organization.bcrm_gpc_sequence_number"] : string.Empty;
                            locationDetails.resource.managingOrganization.reference = "Organization/" + organizationId.Value;

                            appointmentDetailsDTO.Add(locationDetails);
                        }
                    }
                }
                return new JavaScriptSerializer().Serialize(appointmentDetailsDTO);
            }
            catch (Exception ex)
            {
                return null;
            }



        }

        internal string GetOrganizationJSON(List<string> organizations)
        {
            List<AppointmentOrganizationDTO> appointmentDetailsList = new List<AppointmentOrganizationDTO>();
            foreach (var organizationId in organizations)
            {
                var organizationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                           <entity name='bcrm_clinic'>
                                             <attribute name='bcrm_clinicid' />
                                             <attribute name='bcrm_name' />
                                             <attribute name='bcrm_house_number_street_name'/>
                                             <attribute name='bcrm_city_or_town'/>
                                             <attribute name='createdon' />
                                             <attribute name='bcrm_postal_code' />
                                             <attribute name='bcrm_odscode' />
                                             <attribute name='bcrm_gpc_versionid' />
                                             <attribute name='bcrm_gpc_telecom_value' />
                                             <attribute name='bcrm_gpc_telecom_use' />
                                             <attribute name='bcrm_gpc_telecom_system' />
                                             <attribute name='bcrm_gpc_sequence_number' />
                                             <attribute name='bcrm_gpc_address_use' />
                                             <attribute name='bcrm_gpc_address_district' />
                                             <attribute name='bcrm_accountname' />
                                             <order attribute='bcrm_name' descending='false' />
                                              <filter type='and'>
                                                <condition attribute='bcrm_clinicid' operator='eq' value='{" + organizationId + @"}' />
                                              </filter>
                                           </entity>
                                         </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(organizationXML));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        AppointmentOrganizationDTO organizationDetails = GetOrganizationDTO();
                        organizationDetails.resource.id = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        organizationDetails.resource.meta.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;
                        organizationDetails.resource.identifier[0].value = record.Attributes.Contains("bcrm_odscode") ? record["bcrm_odscode"].ToString() : string.Empty;
                        organizationDetails.resource.name = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                        organizationDetails.resource.address.line[0] = record.Attributes.Contains("bcrm_house_number_street_name") ? record["bcrm_house_number_street_name"].ToString() : string.Empty;

                        organizationDetails.resource.address.city = record.Attributes.Contains("bcrm_city_or_town") ? record["bcrm_city_or_town"].ToString() : string.Empty;
                        organizationDetails.resource.address.district = record.Attributes.Contains("bcrm_gpc_address_district") ? record["bcrm_gpc_address_district"].ToString() : string.Empty;
                        organizationDetails.resource.address.postalCode = record.Attributes.Contains("bcrm_postal_code") ? record["bcrm_postal_code"].ToString() : string.Empty;
                        organizationDetails.resource.telecom.value = record.Attributes.Contains("bcrm_gpc_telecom_value") ? record["bcrm_gpc_telecom_value"].ToString() : string.Empty;


                        organizationDetails.resource.telecom.system = record.Attributes.Contains("bcrm_gpc_telecom_system") ? record["bcrm_gpc_telecom_system"].ToString() : string.Empty;
                        organizationDetails.resource.telecom.use = record.Attributes.Contains("bcrm_gpc_telecom_use") ? record.FormattedValues["bcrm_gpc_telecom_use"].ToString() : string.Empty;
                        appointmentDetailsList.Add(organizationDetails);
                    }
                }
            }
            return new JavaScriptSerializer().Serialize(appointmentDetailsList);
        }

        internal FinalJSONofSearchSlotDTO1 CreateFinalJSONForSlot(string slots, string schedules, string practioners, string locations, string organizations)
        {
            try
            {


                FinalJSONofSearchSlotDTO1 FinalJSONofSearchSlotDTO1 = new FinalJSONofSearchSlotDTO1();
                FinalJSONofSearchSlotDTO1.resourceType = "Bundle";
                FinalJSONofSearchSlotDTO1.type = "searchset";

                var slotJSON = JsonConvert.DeserializeObject<List<SlotDTO>>(slots);
                var scheduleJSON = JsonConvert.DeserializeObject<List<ScheduleDTO>>(schedules);


                var organizationJSON = JsonConvert.DeserializeObject<List<AppointmentOrganizationDTO>>(organizations);

                // var totalObj = slotJSON.Count + scheduleJSON.Count + practitionerJSON.Count + locationJSON.Count + organizationJSON.Count;

                List<object> mainList = new List<object>();

                for (int i = 0; i < slotJSON.Count; i++)
                {
                    mainList.Add(slotJSON[i]);
                }
                for (int i = 0; i < scheduleJSON.Count; i++)
                {
                    mainList.Add(scheduleJSON[i]);
                }

                if (practioners != "")
                {
                    var practitionerJSON = JsonConvert.DeserializeObject<List<AppointmentPractitionerDTO>>(practioners);
                    for (int i = 0; i < practitionerJSON.Count; i++)
                    {
                        mainList.Add(practitionerJSON[i]);
                    }
                }

                if (locations != "")
                {
                    var locationJSON = JsonConvert.DeserializeObject<List<AppointmentLocationDTO>>(locations);
                    for (int i = 0; i < locationJSON.Count; i++)
                    {
                        mainList.Add(locationJSON[i]);
                    }
                }

                for (int i = 0; i < organizationJSON.Count; i++)
                {
                    mainList.Add(organizationJSON[i]);
                }

                FinalJSONofSearchSlotDTO1.entry = mainList;

                return FinalJSONofSearchSlotDTO1;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        internal string GetOrganizationLookUpId(string seqNo)
        {
            try
            {
                var orgXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                             <entity name='bcrm_clinic'>
                               <attribute name='bcrm_clinicid' />
                               <attribute name='bcrm_name' />
                               <attribute name='createdon' />
                               <order attribute='bcrm_name' descending='false' />
                               <filter type='and'>
                                 <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + seqNo + @"' />
                               </filter>
                             </entity>
                           </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(orgXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var orgId = record.Attributes.Contains("bcrm_clinicid") ? record["bcrm_clinicid"].ToString() : string.Empty;
                    return orgId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        internal List<string> GetSlotLookUpId(string seqNo)
        {
            try
            {
                var slotXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msemr_slot'>
                                <attribute name='msemr_slotid' />
                                <attribute name='msemr_name' />
                                <attribute name='bcrm_preferredcontactmethod' />
                                <attribute name='createdon' />
                                <order attribute='msemr_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bcrm_sequencenumber' operator='eq' value='" + seqNo + @"' />
                                  <condition attribute='bcrm_slotstatus' operator='eq' value='101' />
                                </filter>
                              </entity>
                            </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(slotXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    List<string> SlotDetails = new List<string>();
                    var record = AnswerCollection.Entities[0];
                    var slotId = record.Attributes.Contains("msemr_slotid") ? record["msemr_slotid"].ToString() : string.Empty;

                    SlotDetails.Add(slotId);

                    dynamic preferred_method_no = record.Attributes.Contains("bcrm_preferredcontactmethod") ? record["bcrm_preferredcontactmethod"] : "";
                    SlotDetails.Add(preferred_method_no.Value.ToString());


                    //SlotDetails.Add(preferred_method_no);
                    return SlotDetails;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string GetLocationLookUpId(string seqNo)
        {
            try
            {
                var locationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                 <entity name='bcrm_gpclocation'>
                                   <attribute name='bcrm_gpclocationid' />
                                   <attribute name='bcrm_name' />
                                   <attribute name='createdon' />
                                   <order attribute='bcrm_name' descending='false' />
                                   <filter type='and'>
                                     <condition attribute='bcrm_gpcsequencenumber' operator='eq' value='" + seqNo + @"' />
                                   </filter>
                                 </entity>
                               </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(locationXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var locationId = record.Attributes.Contains("bcrm_gpclocationid") ? record["bcrm_gpclocationid"].ToString() : string.Empty;
                    return locationId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        internal string GetPatientLookUpId(string seqNo)
        {
            try
            {
                var patientXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='contact'>
                                     <attribute name='fullname' />
                                     <attribute name='telephone1' />
                                     <attribute name='contactid' />
                                     <order attribute='fullname' descending='false' />
                                     <filter type='and'>
                                       <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + seqNo + @"' />
                                     </filter>
                                   </entity>
                                 </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(patientXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var patientId = record.Attributes.Contains("contactid") ? record["contactid"].ToString() : string.Empty;
                    return patientId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        internal List<string> GetserviceNameAndPractictionerLookId(string slotNo)
        {
            var slotxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msemr_slot'>
                                <attribute name='msemr_slotid' />
                                <attribute name='msemr_name' />
                                <attribute name='createdon' />
                                <order attribute='msemr_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bcrm_sequencenumber' operator='eq' value='" + slotNo + @"' />
                                </filter>
                                <link-entity name='bcrm_shifts' from='bcrm_shiftsid' to='bcrm_shift' visible='false' link-type='outer' alias='shift'>
                                  <attribute name='bcrm_staff' />
                                  <attribute name='bcrm_service' />
                                </link-entity>
                              </entity>
                           </fetch>";
            List<string> shiftDetails = new List<string>();

            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(slotxml));

            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                var record = AnswerCollection.Entities[0];

                dynamic practionerLookUp = record.Attributes.Contains("shift.bcrm_staff") ? record["shift.bcrm_staff"] : string.Empty;
                shiftDetails.Add(practionerLookUp.Value.Id.ToString());

                dynamic serviceName = record.Attributes.Contains("shift.bcrm_service") ? record["shift.bcrm_service"] : string.Empty;
                shiftDetails.Add(serviceName.Value.Name.ToString());

            }
            return shiftDetails;
        }

        internal string GetAppointmentNumber(string appointmentLookup)
        {
            try
            {
                var xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                             <entity name='appointment'>
                               <attribute name='subject' />
                               <attribute name='statecode' />
                               <attribute name='scheduledstart' />
                               <attribute name='scheduledend' />
                               <attribute name='createdby' />
                               <attribute name='bcrm_appointmentno' />
                               <attribute name='activityid' />
                               <attribute name='instancetypecode' />
                               <order attribute='subject' descending='false' />
                               <filter type='and'>
                                 <condition attribute='activityid' operator='eq' uiname='' uitype='appointment' value='{" + appointmentLookup + @"}' />
                               </filter>
                             </entity>
                           </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(xml));



                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {


                    var record = AnswerCollection.Entities[0];
                    var appointmnetId = record.Attributes.Contains("bcrm_appointmentno") ? record["bcrm_appointmentno"].ToString() : "";
                    return appointmnetId;

                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        internal string CheckSlotIsAvailableOrNot(string slotNo)
        {
            var slotxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msemr_slot'>
                                <attribute name='msemr_slotid' />
                                <attribute name='msemr_name' />
                                <attribute name='createdon' />
                                <attribute name='bcrm_slotstatus' />
                                <order attribute='msemr_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bcrm_sequencenumber' operator='eq' value='" + slotNo + @"' />
                                </filter>
                              </entity>
                           </fetch>";
            EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(slotxml));
            if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
            {
                var record = AnswerCollection.Entities[0];

                var slotStatus = record.Attributes.Contains("bcrm_slotstatus") ? record.FormattedValues["bcrm_slotstatus"].ToString().ToLower() : string.Empty;
                if (slotStatus.ToString().ToLower() != "free")
                {
                    return "booked";
                }
                else
                {
                    return "free";
                }
            }

            return "NotFound";
        }

        public static bool IsValidDate(string dateString)
        {
            // Check if the length of the input is exactly 10 characters (yyyy-MM-dd)
            if (dateString.Length != 10)
            {
                return false;
            }

            // Check if the 5th and 8th characters are hyphens ('-')
            if (dateString[4] != '-' || dateString[7] != '-')
            {
                return false;
            }

            // Extract year, month, and day parts
            string yearPart = dateString.Substring(0, 4);
            string monthPart = dateString.Substring(5, 2);
            string dayPart = dateString.Substring(8, 2);

            // Check if the year part is numeric and has 4 digits
            if (!IsAllDigits(yearPart) || yearPart.Length != 4)
            {
                return false;
            }

            // Check if the month part is numeric and between 1 and 12
            if (!IsAllDigits(monthPart) || !IsValidMonth(monthPart))
            {
                return false;
            }

            // Check if the day part is numeric and between 1 and 31
            if (!IsAllDigits(dayPart) || !IsValidDay(dayPart))
            {
                return false;
            }

            // If all checks pass, the date is valid
            return true;
        }

        // Helper function to check if a string contains only digits
        private static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        // Helper function to check if the month is between 01 and 12
        private static bool IsValidMonth(string month)
        {
            int monthInt = int.Parse(month);
            return monthInt >= 1 && monthInt <= 12;
        }

        // Helper function to check if the day is between 01 and 31
        private static bool IsValidDay(string day)
        {
            int dayInt = int.Parse(day);
            return dayInt >= 1 && dayInt <= 31;
        }

        internal EntityCollection GetFilterRecord(EntityCollection answerCollection, DateTime startDate, DateTime endDate)
        {
            if (answerCollection.Entities.Count > 0)
            {
                // Create an EntityCollection to hold the filtered records
                EntityCollection filteredRecords = new EntityCollection
                {
                    EntityName = answerCollection.EntityName // Keep the same entity name

                };

                // Iterate through each record in the EntityCollection
                foreach (Entity entity in answerCollection.Entities)
                {
                    // Ensure the required fields are present
                    if (entity.Contains("msemr_start") && entity.Contains("msemr_end"))
                    {
                        DateTime msemrStart = (DateTime)entity["msemr_start"];
                        DateTime msemrEnd = (DateTime)entity["msemr_end"];

                        // Check if the record falls within the date range
                        if (msemrStart >= startDate && msemrEnd <= endDate)
                        {
                            // Add the record to the filtered EntityCollection
                            filteredRecords.Entities.Add(entity);
                        }
                    }
                }

                return filteredRecords;

            }
            return new EntityCollection();
        }

        internal bool CheckMultipleSlotIsOkorNot(List<string> SlotIds, DateTime startDate, DateTime endDate)
        {
            try
            {
                var scheduleSequenceNumber = "";
                DateTime lastStartDate = DateTime.Now;
                DateTime lastEndDate = DateTime.Now;
                var preferredMethod = "";

                for (var i=0;i< SlotIds.Count; i++)
                {
                    var slotxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msemr_slot'>
                                <attribute name='msemr_slotid' />
                                <attribute name='msemr_name' />
                                <attribute name='bcrm_slotstatus' />
                                <attribute name='msemr_start' />
                                <attribute name='bcrm_preferredcontactmethod' />
                                <attribute name='msemr_end' />
                                <attribute name='createdon' />
                                <order attribute='msemr_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bcrm_sequencenumber' operator='eq' value='" + SlotIds[i] + @"' />
                                </filter>
                                <link-entity name='bcrm_shifts' from='bcrm_shiftsid' to='bcrm_shift' visible='false' link-type='outer' alias='shift'>
                                  <attribute name='bcrm_staff' />
                                  <attribute name='bcrm_service' />
                                  <attribute name='bcrm_sequencenumber' />
                                </link-entity>
                              </entity>
                           </fetch>";
                    List<string> shiftDetails = new List<string>();

                    EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(slotxml));

                    if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                    {
                        var record = AnswerCollection.Entities[0];

                        dynamic practionerLookUp = record.Attributes.Contains("shift.bcrm_staff") ? record["shift.bcrm_staff"] : string.Empty;
                        shiftDetails.Add(practionerLookUp.Value.Id.ToString());

                        dynamic serviceName = record.Attributes.Contains("shift.bcrm_service") ? record["shift.bcrm_service"] : string.Empty;
                        shiftDetails.Add(serviceName.Value.Name.ToString());

                        dynamic sequenceNumber = record.Attributes.Contains("shift.bcrm_sequencenumber") ? record["shift.bcrm_sequencenumber"] : string.Empty;

                        if (record.Attributes.Contains("msemr_start"))
                        {
                            var startDateCRM = (DateTime)record.Attributes["msemr_start"];
                            if(i != 0)
                            {
                                if (startDateCRM != lastEndDate)
                                {
                                    return false;
                                }
                            }
                            lastStartDate = startDateCRM;
                        }

                        if (record.Attributes.Contains("msemr_end"))
                        {
                            var endDateCRM = (DateTime)record.Attributes["msemr_end"];
                            lastEndDate = endDateCRM;
                        }

                        if (i==0)
                        {
                            scheduleSequenceNumber = sequenceNumber.Value.ToString();

                        }
                        else
                        {
                            if(scheduleSequenceNumber != sequenceNumber.Value.ToString())
                            {
                                return false;
                            }
                        }

                        var slotStatus = record.Attributes.Contains("bcrm_slotstatus") ? record.FormattedValues["bcrm_slotstatus"].ToString().ToLower() : string.Empty;
                        if (slotStatus.ToString().ToLower() != "free")
                        {
                            return false;
                        }

                        if (i==0)
                        {
                            if (record.Attributes.Contains("msemr_start")) 
                            {
                                var startDateCRM = (DateTime)record.Attributes["msemr_start"];
                                if (startDateCRM != startDate)
                                {
                                    return false;
                                }
                            }
                        }

                        if(i == SlotIds.Count-1)
                        {
                            if (record.Attributes.Contains("msemr_end"))
                            {
                                var endDateCRM = (DateTime)record.Attributes["msemr_end"];
                                if (endDateCRM != endDate)
                                {
                                    return false;
                                }
                            }
                        }
                        if (i == 0)
                        {
                            if (record.Attributes.Contains("bcrm_preferredcontactmethod"))
                            {
                                preferredMethod = record.FormattedValues["bcrm_preferredcontactmethod"].ToString().ToUpper();
                            }
                        }
                        else
                        {
                            if (record.Attributes.Contains("bcrm_preferredcontactmethod"))
                            {
                                var intPrefMeth = record.FormattedValues["bcrm_preferredcontactmethod"].ToString().ToUpper();
                                if(intPrefMeth != preferredMethod)
                                {
                                    return false;
                                }
                            }
                        }

                    }
                }




                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        internal bool SetAllSlotAsBooked(List<string> SlotIds,string appointmentLookupId)
        {
            try
            {
                List<string> slotLookUpIds = new List<string>();

                for (var i = 0; i < SlotIds.Count; i++)
                {
                    var slotxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msemr_slot'>
                                <attribute name='msemr_slotid' />
                                <attribute name='msemr_name' />
                                <attribute name='bcrm_slotstatus' />
                                <attribute name='msemr_start' />
                                <attribute name='bcrm_preferredcontactmethod' />
                                <attribute name='msemr_end' />
                                <attribute name='createdon' />
                                <order attribute='msemr_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bcrm_sequencenumber' operator='eq' value='" + SlotIds[i] + @"' />
                                </filter>
                              </entity>
                           </fetch>";
                    EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(slotxml));
                    if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                    {
                        var record = AnswerCollection.Entities[0];
                        slotLookUpIds.Add(record.Id.ToString());

                    }
                }

                for(var i=0;i<slotLookUpIds.Count;i++)
                {
                    Entity slotUpdate = new Entity("msemr_slot", new Guid(slotLookUpIds[i]));
                    slotUpdate["bcrm_slotstatus"] = new OptionSetValue(103);
                    slotUpdate["bcrm_appointment"] = new EntityReference("appointment", new Guid(appointmentLookupId));
                    _crmServiceClient.Update(slotUpdate);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        internal bool CheckSlotOrganizationTypeIsOther(string slotId)
        {
            try
            {
                var SlotOrgTypexml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' top='5000'>
                                     <entity name='msemr_slot'>
                                       <attribute name='msemr_slotid' />
                                       <attribute name='msemr_name' />
                                       <attribute name='createdon' />
                                       <attribute name='bcrm_versionnumber' />
                                       <attribute name='statuscode' />
                                       <attribute name='statecode' />
                                       <attribute name='msemr_status' />
                                       <attribute name='msemr_start' />
                                       <attribute name='msemr_specialty' />
                                       <attribute name='bcrm_slotstatus' />
                                       <attribute name='bcrm_slotduration' />
                                       <attribute name='bcrm_shift' />
                                       <attribute name='msemr_servicetypenew' />
                                       <attribute name='msemr_servicecategory' />
                                       <attribute name='bcrm_sequencenumber' />
                                       <attribute name='msemr_schedule' />
                                       <attribute name='overriddencreatedon' />
                                       <attribute name='bcrm_preferredcontactmethod' />
                                       <attribute name='owningbusinessunit' />
                                       <attribute name='ownerid' />
                                       <attribute name='msemr_overbooked' />
                                       <attribute name='modifiedon' />
                                       <attribute name='modifiedonbehalfby' />
                                       <attribute name='modifiedby' />
                                       <attribute name='msemr_end' />
                                       <attribute name='createdonbehalfby' />
                                       <attribute name='createdby' />
                                       <attribute name='msemr_comment' />
                                       <attribute name='msemr_appointmenttypenew' />
                                       <attribute name='msemr_appointmentemrid' />
                                       <order attribute='msemr_name' descending='false' />
                                       <filter type='and'>
                                         <condition attribute='bcrm_sequencenumber' operator='eq' value='" + slotId + @"' />
                                       </filter>
                                       <link-entity name='bcrm_shifts' from='bcrm_shiftsid' to='bcrm_shift' visible='false' link-type='outer' alias='schedule'>
                                         <attribute name='bcrm_sequencenumber' />
                                         <attribute name='bcrm_service' />
                                         <attribute name='bcrm_versionnumber' />
                                         <attribute name='bcrm_scheduleaccessibility' />
                                          <attribute name='bcrm_todate' />
                                          <attribute name='bcrm_staff' />
                                          <attribute name='bcrm_gplocation' />
                                          <attribute name='bcrm_fromdate' />
                                          <attribute name='bcrm_clinic' />
                                        <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_clinic' visible='false' link-type='outer' alias='organisation'>
                                        <attribute name='bcrm_gpc_telecom_value' />
                                        <attribute name='bcrm_gpc_sequence_number' />
                                        <attribute name='bcrm_clinictype' />     
                                        <attribute name='bcrm_odscode' />
                                        <attribute name='bcrm_name' />
                                        </link-entity>
                                       </link-entity>
                                     </entity>
                                    </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(SlotOrgTypexml));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    var orgType = record.Attributes.Contains("organisation.bcrm_clinictype") ? record.FormattedValues["organisation.bcrm_clinictype"].ToString() : "";
                    if(orgType.ToString().ToUpper().Contains("OTHER"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #region ConvertDateFormat

        public static string ConvertToProperDateFormat(string inputDate, string type)
        {
            DateTime parsedDate;

            // Define the different input formats
            string[] dateFormats = {
                                    
                                      "yyyy-MM-ddTHH:mm:sszzz",   // Full date, time, and time zone offset (e.g., 2015-10-23T16:38:32+05:30)
                                      "yyyy-MM-dd"
                                  };

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
                    else if (inputDate.Length == 7) // Year and Month
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, 1).ToString("yyyy-MM-dd");
                    }
                    else // Full date
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
                    else if (inputDate.Length == 7) // Year and Month
                    {
                        return new DateTime(parsedDate.Year, parsedDate.Month, DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month)).ToString("yyyy-MM-dd");
                    }
                    else // Full date
                    {
                        return parsedDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            // If parsing fails, return "Invalid_date_format"
            return "Invalid_date_format";
        }



        #endregion

        #endregion
    }
}
