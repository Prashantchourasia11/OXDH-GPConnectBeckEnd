using GP_Connect.CRM_Connection;
using GP_Connect.DataTransferObject;
using GP_Connect.FHIR_JSON;
using GP_Connect.FHIR_JSON.Foundation;
using GP_Connect.PDS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Swagger.Net;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using Entity = Microsoft.Xrm.Sdk.Entity;

namespace GP_Connect.Service.Foundation
{

    public class ServiceFoundation
    {

        #region Properties

        IOrganizationService _crmServiceClient = null;
        CRM_connection crmCon = new CRM_connection();

        #endregion

        #region Constructor

        public ServiceFoundation()
        {

            _crmServiceClient = crmCon.crmconnectionOXVC();
        }

        #endregion

        #region Method

        public dynamic FoundationMetaData()
        {
            MetaData md = new MetaData();
            var data = md.FoundationMetaData();
            return data;

        }
        public dynamic FindAPatient(string NHSNumber, string RegType, string identifier, int identifierCount, string fullUrl,string SspTraceId)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];

                PatientDetails pd = new PatientDetails();
                if (!identifier.Contains("|"))
                {
                    finaljson[0] = pd.PatientIdentifierNotPresent();
                    finaljson[1] = "";
                    finaljson[2] = "IdentifierNotPresent";
                    return finaljson;
                }
                else
                {
                    if (!identifier.Contains("https://fhir.nhs.uk/Id/nhs-number"))
                    {
                        finaljson[0] = pd.PatientInvalidParameter(identifier);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidParameter";
                        return finaljson;
                    }
                    else if (NHSNumber == "")
                    {
                        finaljson[0] = pd.PatientInvalidParameter(identifier);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidParameter";
                        return finaljson;
                    }
                }


                if (!identifier.Contains("https://fhir.nhs.uk/Id/nhs-number"))
                {
                    finaljson[0] = pd.PatientIdentifierIsNotValid(identifier);
                    finaljson[1] = "";
                    finaljson[2] = "InvalidIdentifier";
                    return finaljson;
                }

                if (identifierCount > 1)
                {
                    finaljson[0] = pd.MultipleIndentifierExist(identifierCount);
                    finaljson[1] = "";
                    finaljson[2] = "MultipleIdentifier";
                    return finaljson;
                }

                if (!fullUrl.Contains("identifier"))
                {
                    finaljson[0] = pd.IndentifierSpellingMistake();
                    finaljson[1] = "";
                    finaljson[2] = "IdentifierMissing";
                    return finaljson;
                }


                //**************************Get Profile Data On behalf Of NHS Number ****************************************//
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                          <attribute name='lastname' />
                        <attribute name='telephone1' />
                        <attribute name='address1_telephone1' />
                        <attribute name='contactid' />
                        <attribute name='bcrm_pdsversionid' />
                        <attribute name='mobilephone' />
                        <attribute name='lastname' />
                        <attribute name='gendercode' />
                        <attribute name='firstname' />
                        <attribute name='emailaddress1' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_line1' />
                        <attribute name='address1_stateorprovince' />
                        <attribute name='address1_country' />
                        <attribute name='address1_city' />
                        <attribute name='entityimage' />
                        <attribute name='bcrm_policynumber' />
                        <attribute name='bcrm_healthcodenumber' />
                        <attribute name='bcrm_patientnumber' />
                        <attribute name='bcrm_nhsnumber' />
                        <attribute name='bcrm_ifrnhsno' />
                        <attribute name='bcrm_funded' />
                        <attribute name='parentcustomerid' />
                        <attribute name='birthdate' />
                        <attribute name='preferredcontactmethodcode' />
                        <attribute name='donotemail' />
                        <attribute name='followemail' />
                        <attribute name='donotbulkemail' />
                        <attribute name='donotphone' />
                        <attribute name='donotpostalmail' />
                        <attribute name='donotfax' />
                        <attribute name='msemr_contact1relationship' />
                        <attribute name='msemr_contact1' />
                        <attribute name='bcrm_wheredidyouhearaboutus' />
                        <attribute name='bcrm_iunderstandthatdoctornowisaprivategpservi' />
                        <attribute name='bcrm_iunderstandthatacancellationfeewillbeappl' />
                        <attribute name='bcrm_iwishtoreceivetextmessagesfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceiveemailsfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceivepostalcommunicationfromdoct' />
                        <attribute name='bcrm_iwouldliketoreceiveupdatesaboutnewservice' />
                        <attribute name='bcrm_iamhappyfordoctornowtocontactmynextofkin' />
                        <attribute name='bcrm_smokingstatus' />
                        <attribute name='bcrm_howmanyunitsofalcoholdoyoudrinkinanavera' />
                        <attribute name='bcrm_neverdrinkalcohol' />
                        <attribute name='bcrm_diabetes' />
                        <attribute name='bcrm_highbloodpressure' />
                        <attribute name='bcrm_heartdisease' />
                        <attribute name='bcrm_kidneydisease' />
                        <attribute name='bcrm_cancer' />
                        <attribute name='bcrm_thyroiddisease' />
                        <attribute name='bcrm_epilepsy' />
                        <attribute name='bcrm_depression' />
                        <attribute name='bcrm_stroke' />
                        <attribute name='bcrm_asthma' />
                        <attribute name='bcrm_gptype' />
                        <attribute name='bcrm_transientischaemicattack' />
                        <attribute name='bcrm_mentalillness' />
                        <attribute name='bcrm_heartrhythmproblems' />
                        <attribute name='bcrm_pleasegivedetailsofanyothersignificantill' />
                        <attribute name='bcrm_pleasegivedetailsofanymedicalconditionswi' />
                        <attribute name='bcrm_pleasegivedetailsifyouhaveanyallergiestoa' />
                        <attribute name='bcrm_kinsfirstname' />
                        <attribute name='bcrm_kinslastname' />
                        <attribute name='address2_line1' />
                        <attribute name='emailaddress2' />
                        <attribute name='bcrm_gpsurgeryname' />
                        <attribute name='bcrm_doctorsname' />
                        <attribute name='address3_city' />
                        <attribute name='telephone3' />
                        <attribute name='address2_postalcode' />
                         <attribute name='bcrm_occuption' />
                         <attribute name='bcrm_smokingstatus' />
                         <attribute name='bcrm_stoppedsmoking' />
                          <attribute name='bcrm_gpc_sequence_number' />
                           <attribute name='bcrm_pdsjson' />
                         <attribute name='bcrm_stoppeddrinkingalcohol' />
                          <attribute name='bcrm_gpc_registractionperiod' />
                         <attribute name='bcrm_alcoholintake' />
                        <attribute name='bcrm_modeofcommunication' />
                         <attribute name='bcrm_gpc_communicationproficiency' />
                         <attribute name='bcrm_title' />
                         <attribute name='description' />
                         <attribute name='bcrm_donotallowsms' />
                       <attribute name='statuscode' />

                    <attribute name='mobilephone' />
                    <attribute name='address2_telephone1' />
                    <attribute name='emailaddress3' />
                    <attribute name='address1_telephone1' />
                    <attribute name='emailaddress2' />
          

                      <attribute name='bcrm_languages' />
                      <attribute name='bcrm_interpreterrequired' />
                      <attribute name='bcrm_nhsnumberverificationstatus' />
                      <attribute name='bcrm_nhsnumberverificationstatusdisplay' />
                       <attribute name='bcrm_sensitivename' />
                        <attribute name='bcrm_middlename' />
                        <attribute name='bcrm_deceaseddate' />
                         <attribute name='bcrm_age' />
                        <order attribute='createdon' descending='true' />
                        <filter type='or'>
                          <condition attribute='bcrm_nhsnumber' operator='like' value='%" + NHSNumber + @"%' />
                        </filter>
                       <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='staff'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='clinic'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gpc_preferredbarnchsurgery' visible='false' link-type='outer' alias='location'>
                            <attribute name='bcrm_gpcsequencenumber' />
                          </link-entity>
                       
                      </entity>
                    </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));

                List<PatientDTO> AllPatientDetails = new List<PatientDTO>();
                PatientDTO crmUserProfile = new PatientDTO();


                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    {
                        crmUserProfile.Id = record.Id;

                        dynamic staffEntity = record.Attributes.Contains("staff.bcrm_gpc_sequence_number") ? record["staff.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.stuffRefrenceNumber = staffEntity.ToString() != "" ? staffEntity.Value : string.Empty;

                        dynamic clinicEntity = record.Attributes.Contains("clinic.bcrm_gpc_sequence_number") ? record["clinic.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.clinicRefrenceNumber = clinicEntity.ToString() != "" ? clinicEntity.Value : string.Empty;

                        dynamic locationEntity = record.Attributes.Contains("location.bcrm_gpcsequencenumber") ? record["location.bcrm_gpcsequencenumber"] : string.Empty;
                        crmUserProfile.locationRefrenceNumber = locationEntity.ToString() != "" ? locationEntity.Value : string.Empty;

                        crmUserProfile.bcrm_title_label = record.Attributes.Contains("bcrm_title") ? record.FormattedValues["bcrm_title"].ToString() : string.Empty;

                        crmUserProfile.modeOfCommunication = record.Attributes.Contains("bcrm_modeofcommunication") ? record.FormattedValues["bcrm_modeofcommunication"].ToString() : string.Empty;
                        crmUserProfile.communicationProficiency = record.Attributes.Contains("bcrm_gpc_communicationproficiency") ? record.FormattedValues["bcrm_gpc_communicationproficiency"].ToString() : string.Empty;

                        dynamic check = record.Attributes.Contains("Patient_relationships") ? record["Patient_relationships"].ToString() : string.Empty;

                        crmUserProfile.IsSensitive = record.Attributes.Contains("bcrm_sensitivename") ? record["bcrm_sensitivename"].ToString() : string.Empty;

                        crmUserProfile.bcrm_middlename = record.Attributes.Contains("bcrm_middlename") ? record["bcrm_middlename"].ToString() : string.Empty;
                        crmUserProfile.fullname = record.Attributes.Contains("fullname") ? record["fullname"].ToString() : string.Empty;
                        crmUserProfile.firstname = record.Attributes.Contains("firstname") ? record["firstname"].ToString() : string.Empty;
                        crmUserProfile.lastname = record.Attributes.Contains("lastname") ? record["lastname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_age = record.Attributes.Contains("bcrm_age") ? record["bcrm_age"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;
                        crmUserProfile.emailaddress1 = record.Attributes.Contains("emailaddress1") ? record["emailaddress1"].ToString() : string.Empty;
                        crmUserProfile.address1_city = record.Attributes.Contains("address1_city") ? record["address1_city"].ToString() : "";
                        crmUserProfile.address1_country = record.Attributes.Contains("address1_country") ? record["address1_country"].ToString() : "";
                        crmUserProfile.address1_line1 = record.Attributes.Contains("address1_line1") ? record["address1_line1"].ToString() : "";
                        crmUserProfile.telephone1 = record.Attributes.Contains("telephone1") ? record["telephone1"].ToString() : "";
                        crmUserProfile.address1_stateorprovince = record.Attributes.Contains("address1_stateorprovince") ? record["address1_stateorprovince"].ToString() : "";
                        crmUserProfile.address1_postalcode = record.Attributes.Contains("address1_postalcode") ? record["address1_postalcode"].ToString() : "";
                        if (record.Attributes.Contains("entityimage")) crmUserProfile.entityimage = record["entityimage"] as byte[];
                        if (record.Attributes.Contains("birthdate")) { crmUserProfile.birthdate = (DateTime)record.Attributes["birthdate"]; }
                        if (record.Attributes.Contains("bcrm_deceaseddate")) { crmUserProfile.deceasedDate = (DateTime)record.Attributes["bcrm_deceaseddate"]; }
                        crmUserProfile.gender = record.Attributes.Contains("gendercode") ? record.FormattedValues["gendercode"].ToString() : string.Empty;

                        crmUserProfile.gendercode = record.Contains("gendercode") && record["gendercode"] != null ? Convert.ToInt32(((OptionSetValue)record["gendercode"]).Value) : 0;
                        crmUserProfile.bcrm_policynumber = record.Attributes.Contains("bcrm_policynumber") ? record["bcrm_policynumber"].ToString() : "";
                        crmUserProfile.bcrm_healthcodenumber = record.Attributes.Contains("bcrm_healthcodenumber") ? record["bcrm_healthcodenumber"].ToString() : "";
                        crmUserProfile.bcrm_patientnumber = record.Attributes.Contains("bcrm_patientnumber") ? record["bcrm_patientnumber"].ToString() : "";
                        crmUserProfile.bcrm_nhsnumber = record.Attributes.Contains("bcrm_nhsnumber") ? record["bcrm_nhsnumber"].ToString() : "";
                        crmUserProfile.bcrm_ifrnhsno = record.Attributes.Contains("bcrm_ifrnhsno") ? record["bcrm_ifrnhsno"].ToString() : "";
                        crmUserProfile.bcrm_funded_name = record.Attributes.Contains("bcrm_funded") ? record.FormattedValues["bcrm_funded"].ToString() : string.Empty;
                        crmUserProfile.bcrm_funded = record.Contains("bcrm_funded") && record["bcrm_funded"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_funded"]).Value) : 0;
                        crmUserProfile.preferredcontactmethodcode = record.Contains("preferredcontactmethodcode") && record["preferredcontactmethodcode"] != null ? Convert.ToInt32(((OptionSetValue)record["preferredcontactmethodcode"]).Value) : 0;
                        crmUserProfile.parentcustomerid = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Id : Guid.Empty;
                        crmUserProfile.parentcustomername = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Name : string.Empty;
                        crmUserProfile.donotemail = record.Attributes.Contains("donotemail") ? ((bool)record["donotemail"]) : false;
                        crmUserProfile.followemail = record.Attributes.Contains("followemail") ? ((bool)record["followemail"]) : false;
                        crmUserProfile.donotbulkemail = record.Attributes.Contains("donotbulkemail") ? ((bool)record["donotbulkemail"]) : false;
                        crmUserProfile.donotphone = record.Attributes.Contains("donotphone") ? ((bool)record["donotphone"]) : false;
                        crmUserProfile.donotpostalmail = record.Attributes.Contains("donotpostalmail") ? ((bool)record["donotpostalmail"]) : false;
                        crmUserProfile.donotfax = record.Attributes.Contains("donotfax") ? ((bool)record["donotfax"]) : false;
                        //Fields from Doctor's Now
                        crmUserProfile.msemr_contact1relationship = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Id : Guid.Empty;
                        crmUserProfile.msemr_contact1relationship_name = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Name : string.Empty;
                        crmUserProfile.msemr_contact1 = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Id : Guid.Empty;
                        crmUserProfile.emergencycontact = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Name : String.Empty;
                        crmUserProfile.bcrm_wheredidyouhearaboutus = record.Contains("bcrm_wheredidyouhearaboutus") && record["bcrm_wheredidyouhearaboutus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_wheredidyouhearaboutus"]).Value) : 0;
                        crmUserProfile.bcrm_gptype = record.Contains("bcrm_gptype") && record["bcrm_gptype"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_gptype"]).Value) : 0;
                        crmUserProfile.bcrm_iunderstandthatdoctornowisaprivategpservi = record.Attributes.Contains("bcrm_iunderstandthatdoctornowisaprivategpservi") ? ((bool)record["bcrm_iunderstandthatdoctornowisaprivategpservi"]) : false;
                        crmUserProfile.bcrm_iunderstandthatacancellationfeewillbeappl = record.Attributes.Contains("bcrm_iunderstandthatacancellationfeewillbeappl") ? ((bool)record["bcrm_iunderstandthatacancellationfeewillbeappl"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivetextmessagesfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceivetextmessagesfromdoctornow") ? ((bool)record["bcrm_iwishtoreceivetextmessagesfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceiveemailsfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceiveemailsfromdoctornow") ? ((bool)record["bcrm_iwishtoreceiveemailsfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivepostalcommunicationfromdoct = record.Attributes.Contains("bcrm_iwishtoreceivepostalcommunicationfromdoct") ? ((bool)record["bcrm_iwishtoreceivepostalcommunicationfromdoct"]) : false;
                        crmUserProfile.bcrm_iwouldliketoreceiveupdatesaboutnewservice = record.Attributes.Contains("bcrm_iwouldliketoreceiveupdatesaboutnewservice") ? ((bool)record["bcrm_iwouldliketoreceiveupdatesaboutnewservice"]) : false;
                        crmUserProfile.bcrm_iamhappyfordoctornowtocontactmynextofkin = record.Attributes.Contains("bcrm_iamhappyfordoctornowtocontactmynextofkin") ? ((bool)record["bcrm_iamhappyfordoctornowtocontactmynextofkin"]) : false;
                        crmUserProfile.bcrm_smokingstatus = record.Contains("bcrm_smokingstatus") && record["bcrm_smokingstatus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_smokingstatus"]).Value) : 0;
                        crmUserProfile.bcrm_occuption = record.Attributes.Contains("bcrm_occuption") ? record["bcrm_occuption"].ToString() : "";
                        crmUserProfile.bcrm_stoppedsmoking = record.Attributes.Contains("bcrm_stoppedsmoking") ? record["bcrm_stoppedsmoking"].ToString() : "";
                        crmUserProfile.bcrm_stoppeddrinkingalcohol = record.Attributes.Contains("bcrm_stoppeddrinkingalcohol") ? record["bcrm_stoppeddrinkingalcohol"].ToString() : "";
                        crmUserProfile.bcrm_alcoholintake = record.Contains("bcrm_alcoholintake") && record["bcrm_alcoholintake"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_alcoholintake"]).Value) : 0;
                        crmUserProfile.bcrm_alcoholintake_name = record.Attributes.Contains("bcrm_alcoholintake") ? record.FormattedValues["bcrm_alcoholintake"].ToString() : string.Empty;
                        crmUserProfile.otherDetails = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                        crmUserProfile.bcrm_howmanyunitsofalcoholdoyoudrinkinanavera = record.Attributes.Contains("bcrm_howmanyunitsofalcoholdoyoudrinkinanavera") ? record["bcrm_howmanyunitsofalcoholdoyoudrinkinanavera"].ToString() : string.Empty;
                        crmUserProfile.bcrm_neverdrinkalcohol = record.Attributes.Contains("bcrm_neverdrinkalcohol") ? ((bool)record["bcrm_neverdrinkalcohol"]) : false;
                        crmUserProfile.bcrm_diabetes = record.Attributes.Contains("bcrm_diabetes") ? ((bool)record["bcrm_diabetes"]) : false;
                        crmUserProfile.bcrm_highbloodpressure = record.Attributes.Contains("bcrm_highbloodpressure") ? ((bool)record["bcrm_highbloodpressure"]) : false;
                        crmUserProfile.bcrm_heartdisease = record.Attributes.Contains("bcrm_heartdisease") ? ((bool)record["bcrm_heartdisease"]) : false;
                        crmUserProfile.bcrm_kidneydisease = record.Attributes.Contains("bcrm_kidneydisease") ? ((bool)record["bcrm_kidneydisease"]) : false;
                        crmUserProfile.bcrm_cancer = record.Attributes.Contains("bcrm_cancer") ? ((bool)record["bcrm_cancer"]) : false;
                        crmUserProfile.bcrm_thyroiddisease = record.Attributes.Contains("bcrm_thyroiddisease") ? ((bool)record["bcrm_thyroiddisease"]) : false;
                        crmUserProfile.bcrm_epilepsy = record.Attributes.Contains("bcrm_epilepsy") ? ((bool)record["bcrm_epilepsy"]) : false;
                        crmUserProfile.bcrm_depression = record.Attributes.Contains("bcrm_depression") ? ((bool)record["bcrm_depression"]) : false;
                        crmUserProfile.bcrm_stroke = record.Attributes.Contains("bcrm_stroke") ? ((bool)record["bcrm_stroke"]) : false;
                        crmUserProfile.bcrm_asthma = record.Attributes.Contains("bcrm_asthma") ? ((bool)record["bcrm_asthma"]) : false;
                        crmUserProfile.bcrm_transientischaemicattack = record.Attributes.Contains("bcrm_transientischaemicattack") ? ((bool)record["bcrm_transientischaemicattack"]) : false;
                        crmUserProfile.bcrm_mentalillness = record.Attributes.Contains("bcrm_mentalillness") ? ((bool)record["bcrm_mentalillness"]) : false;
                        crmUserProfile.bcrm_heartrhythmproblems = record.Attributes.Contains("bcrm_heartrhythmproblems") ? ((bool)record["bcrm_heartrhythmproblems"]) : false;
                        crmUserProfile.bcrm_pleasegivedetailsofanyothersignificantill = record.Attributes.Contains("bcrm_pleasegivedetailsofanyothersignificantill") ? record["bcrm_pleasegivedetailsofanyothersignificantill"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsofanymedicalconditionswi = record.Attributes.Contains("bcrm_pleasegivedetailsofanymedicalconditionswi") ? record["bcrm_pleasegivedetailsofanymedicalconditionswi"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsifyouhaveanyallergiestoa = record.Attributes.Contains("bcrm_pleasegivedetailsifyouhaveanyallergiestoa") ? record["bcrm_pleasegivedetailsifyouhaveanyallergiestoa"].ToString() : "";
                        crmUserProfile.bcrm_kinsfirstname = record.Attributes.Contains("bcrm_kinsfirstname") ? record["bcrm_kinsfirstname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_kinslastname = record.Attributes.Contains("bcrm_kinslastname") ? record["bcrm_kinslastname"].ToString() : string.Empty;
                        crmUserProfile.address2_line1 = record.Attributes.Contains("address2_line1") ? record["address2_line1"].ToString() : string.Empty;
                        crmUserProfile.emailaddress2 = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.address2_postalcode = record.Attributes.Contains("address2_postalcode") ? record["address2_postalcode"].ToString() : string.Empty;
                        crmUserProfile.bcrm_gpsurgeryname = record.Attributes.Contains("bcrm_gpsurgeryname") ? record["bcrm_gpsurgeryname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_doctorsname = record.Attributes.Contains("bcrm_doctorsname") ? record["bcrm_doctorsname"].ToString() : string.Empty;
                        crmUserProfile.address3_city = record.Attributes.Contains("address3_city") ? record["address3_city"].ToString() : string.Empty;
                        crmUserProfile.telephone3 = record.Attributes.Contains("telephone3") ? record["telephone3"].ToString() : string.Empty;
                        crmUserProfile.bcrm_donotallowsms = record.Contains("bcrm_donotallowsms") && record["bcrm_donotallowsms"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_donotallowsms"]).Value) : 0;
                        crmUserProfile.GPCSequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_gpc_registractionperiod")) { crmUserProfile.GPCRegistractionDate = (DateTime)record.Attributes["bcrm_gpc_registractionperiod"]; }
                        crmUserProfile.HomePhone1 = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.PdsVersionId = record.Attributes.Contains("bcrm_pdsversionid") ? record["bcrm_pdsversionid"].ToString() : string.Empty;
                        crmUserProfile.pdsJson = record.Attributes.Contains("bcrm_pdsjson") ? record["bcrm_pdsjson"].ToString() : string.Empty;

                        crmUserProfile.InterpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Code = record.Attributes.Contains("bcrm_nhsnumberverificationstatus") ? record["bcrm_nhsnumberverificationstatus"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Display = record.Attributes.Contains("bcrm_nhsnumberverificationstatusdisplay") ? record["bcrm_nhsnumberverificationstatusdisplay"].ToString() : string.Empty;


                        crmUserProfile.homeEmail = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.homePhone = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.workEmail = record.Attributes.Contains("emailaddress3") ? record["emailaddress3"].ToString() : string.Empty;
                        crmUserProfile.workPhone = record.Attributes.Contains("address2_telephone1") ? record["address2_telephone1"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;



                        var patientLanguages = record.Attributes.Contains("bcrm_languages") ? record.FormattedValues["bcrm_languages"].ToString() : string.Empty;
                        if (patientLanguages != "")
                        {
                            string[] allLanguages = patientLanguages.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            crmUserProfile.patientLanguages = languageList;
                        }


                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            crmUserProfile.statusReason = true;
                        }
                        else
                        {
                            crmUserProfile.statusReason = false;
                            var json = pd.EmptyBuddlePatientJSON(SspTraceId);

                            finaljson[0] = json;
                            finaljson[1] = crmUserProfile.PdsVersionId;
                            finaljson[2] = "Success";
                            return finaljson;
                        }

                        if (!crmUserProfile.deceasedDate.ToString().Contains("0001"))
                        {
                            crmUserProfile.statusReason = false;
                            var json = pd.EmptyBuddlePatientJSON(SspTraceId);

                            finaljson[0] = json;
                            finaljson[1] = crmUserProfile.PdsVersionId;
                            finaljson[2] = "Success";
                            return finaljson;

                        }

                        if (crmUserProfile.IsSensitive == "True")
                        {
                            var json = pd.EmptyBuddlePatientJSON(SspTraceId);

                            finaljson[0] = json;
                            finaljson[1] = crmUserProfile.PdsVersionId;
                            finaljson[2] = "Success";
                            return finaljson;
                        }


                        var relatedPerson = GetRelatedPerson(crmUserProfile.Id.ToString(), crmUserProfile);

                        AllPatientDetails.Add(relatedPerson);
                    }

                    if (AllPatientDetails.Count > 0)
                    {

                        var json = pd.FindAPatientUsingJSONFHIR(AllPatientDetails[0], RegType,SspTraceId);

                        finaljson[0] = json;
                        finaljson[1] = crmUserProfile.PdsVersionId;
                        finaljson[2] = "Success";
                        return finaljson;
                    }
                }


                PractionerDetails praDet = new PractionerDetails();

                finaljson[0] = pd.EmptyBuddlePatientJSON(SspTraceId);
                finaljson[1] = "";
                finaljson[2] = "NotFound";
                return finaljson;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic ReadAPatient(string id, string sspInteractionId, string source)
        {
            try
            {
                PatientDetails pd = new PatientDetails();
                dynamic[] finaljson = new dynamic[3];

                if (source == "External")
                {
                    if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:read:patient-1")
                    {
                        var res = pd.WrongInteractionId(sspInteractionId, "Patient");
                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "InvalidInteractionId";
                        return finaljson;

                    }
                }


                //**************************Get Profile Data On behalf Of NHS Number ****************************************//
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                          <attribute name='lastname' />
                        <attribute name='telephone1' />
                        <attribute name='address1_telephone1' />
                        <attribute name='contactid' />
                        <attribute name='bcrm_pdsversionid' />
                        <attribute name='mobilephone' />
                        <attribute name='lastname' />
                        <attribute name='gendercode' />
                        <attribute name='firstname' />
                        <attribute name='emailaddress1' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_line1' />
                        <attribute name='address1_stateorprovince' />
                        <attribute name='address1_country' />
                        <attribute name='address1_city' />
                        <attribute name='entityimage' />
                        <attribute name='bcrm_policynumber' />
                        <attribute name='bcrm_healthcodenumber' />
                        <attribute name='bcrm_patientnumber' />
                        <attribute name='bcrm_nhsnumber' />
                        <attribute name='bcrm_ifrnhsno' />
                        <attribute name='bcrm_funded' />
                        <attribute name='parentcustomerid' />
                        <attribute name='birthdate' />
                        <attribute name='preferredcontactmethodcode' />
                        <attribute name='donotemail' />
                        <attribute name='followemail' />
                        <attribute name='donotbulkemail' />
                        <attribute name='donotphone' />
                        <attribute name='donotpostalmail' />
                        <attribute name='donotfax' />
                        <attribute name='msemr_contact1relationship' />
                        <attribute name='msemr_contact1' />
                        <attribute name='bcrm_wheredidyouhearaboutus' />
                        <attribute name='bcrm_iunderstandthatdoctornowisaprivategpservi' />
                        <attribute name='bcrm_iunderstandthatacancellationfeewillbeappl' />
                        <attribute name='bcrm_iwishtoreceivetextmessagesfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceiveemailsfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceivepostalcommunicationfromdoct' />
                        <attribute name='bcrm_iwouldliketoreceiveupdatesaboutnewservice' />
                        <attribute name='bcrm_iamhappyfordoctornowtocontactmynextofkin' />
                        <attribute name='bcrm_smokingstatus' />
                        <attribute name='bcrm_howmanyunitsofalcoholdoyoudrinkinanavera' />
                        <attribute name='bcrm_neverdrinkalcohol' />
                        <attribute name='bcrm_diabetes' />
                        <attribute name='bcrm_highbloodpressure' />
                        <attribute name='bcrm_heartdisease' />
                        <attribute name='bcrm_kidneydisease' />
                        <attribute name='bcrm_cancer' />
                        <attribute name='bcrm_thyroiddisease' />
                        <attribute name='bcrm_epilepsy' />
                        <attribute name='bcrm_depression' />
                        <attribute name='bcrm_stroke' />
                        <attribute name='bcrm_asthma' />
                        <attribute name='bcrm_gptype' />
                        <attribute name='bcrm_transientischaemicattack' />
                        <attribute name='bcrm_mentalillness' />
                        <attribute name='bcrm_heartrhythmproblems' />
                        <attribute name='bcrm_pleasegivedetailsofanyothersignificantill' />
                        <attribute name='bcrm_pleasegivedetailsofanymedicalconditionswi' />
                        <attribute name='bcrm_pleasegivedetailsifyouhaveanyallergiestoa' />
                        <attribute name='bcrm_kinsfirstname' />
                        <attribute name='bcrm_kinslastname' />
                        <attribute name='address2_line1' />
                        <attribute name='emailaddress2' />
                        <attribute name='bcrm_gpsurgeryname' />
                        <attribute name='bcrm_doctorsname' />
                        <attribute name='address3_city' />
                        <attribute name='telephone3' />
                        <attribute name='address2_postalcode' />
                         <attribute name='bcrm_occuption' />
                         <attribute name='bcrm_smokingstatus' />
                         <attribute name='bcrm_stoppedsmoking' />
                          <attribute name='bcrm_gpc_sequence_number' />
                           <attribute name='bcrm_pdsjson' />
                         <attribute name='bcrm_stoppeddrinkingalcohol' />
                          <attribute name='bcrm_gpc_registractionperiod' />
                         <attribute name='bcrm_alcoholintake' />
                        <attribute name='bcrm_modeofcommunication' />
                         <attribute name='bcrm_gpc_communicationproficiency' />
                         <attribute name='bcrm_title' />
                         <attribute name='description' />
                         <attribute name='bcrm_donotallowsms' />
                       <attribute name='statuscode' />

                    <attribute name='mobilephone' />
                    <attribute name='address2_telephone1' />
                    <attribute name='emailaddress3' />
                    <attribute name='address1_telephone1' />
                    <attribute name='emailaddress2' />

                      <attribute name='bcrm_languages' />
                      <attribute name='bcrm_interpreterrequired' />
                      <attribute name='bcrm_nhsnumberverificationstatus' />
                      <attribute name='bcrm_nhsnumberverificationstatusdisplay' />

                        <attribute name='bcrm_middlename' />
                        <attribute name='bcrm_deceaseddate' />
                         <attribute name='bcrm_age' />
                        <order attribute='fullname' descending='false' />
                        <filter type='or'>
                          <condition attribute='bcrm_gpc_sequence_number' operator='like' value='%" + id + @"%' />
                        </filter>
                       <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='staff'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='clinic'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gpc_preferredbarnchsurgery' visible='false' link-type='outer' alias='location'>
                            <attribute name='bcrm_gpcsequencenumber' />
                          </link-entity>
                       
                      </entity>
                    </fetch>";
                _crmServiceClient = crmCon.crmconnectionOXVC();
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));



                List<PatientDTO> AllPatientDetails = new List<PatientDTO>();
                PatientDTO crmUserProfile = new PatientDTO();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {

                        crmUserProfile.Id = record.Id;

                        dynamic staffEntity = record.Attributes.Contains("staff.bcrm_gpc_sequence_number") ? record["staff.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.stuffRefrenceNumber = staffEntity.ToString() != "" ? staffEntity.Value : string.Empty;

                        dynamic clinicEntity = record.Attributes.Contains("clinic.bcrm_gpc_sequence_number") ? record["clinic.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.clinicRefrenceNumber = clinicEntity.ToString() != "" ? clinicEntity.Value : string.Empty;

                        dynamic locationEntity = record.Attributes.Contains("location.bcrm_gpcsequencenumber") ? record["location.bcrm_gpcsequencenumber"] : string.Empty;
                        crmUserProfile.locationRefrenceNumber = locationEntity.ToString() != "" ? locationEntity.Value : string.Empty;

                        crmUserProfile.bcrm_title_label = record.Attributes.Contains("bcrm_title") ? record.FormattedValues["bcrm_title"].ToString() : string.Empty;

                        crmUserProfile.modeOfCommunication = record.Attributes.Contains("bcrm_modeofcommunication") ? record.FormattedValues["bcrm_modeofcommunication"].ToString() : string.Empty;
                        crmUserProfile.communicationProficiency = record.Attributes.Contains("bcrm_gpc_communicationproficiency") ? record.FormattedValues["bcrm_gpc_communicationproficiency"].ToString() : string.Empty;

                        dynamic check = record.Attributes.Contains("Patient_relationships") ? record["Patient_relationships"].ToString() : string.Empty;

                        crmUserProfile.bcrm_middlename = record.Attributes.Contains("bcrm_middlename") ? record["bcrm_middlename"].ToString() : string.Empty;
                        crmUserProfile.fullname = record.Attributes.Contains("fullname") ? record["fullname"].ToString() : string.Empty;
                        crmUserProfile.firstname = record.Attributes.Contains("firstname") ? record["firstname"].ToString() : string.Empty;
                        crmUserProfile.lastname = record.Attributes.Contains("lastname") ? record["lastname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_age = record.Attributes.Contains("bcrm_age") ? record["bcrm_age"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;
                        crmUserProfile.emailaddress1 = record.Attributes.Contains("emailaddress1") ? record["emailaddress1"].ToString() : string.Empty;
                        crmUserProfile.address1_city = record.Attributes.Contains("address1_city") ? record["address1_city"].ToString() : "";
                        crmUserProfile.address1_country = record.Attributes.Contains("address1_country") ? record["address1_country"].ToString() : "";
                        crmUserProfile.address1_line1 = record.Attributes.Contains("address1_line1") ? record["address1_line1"].ToString() : "";
                        crmUserProfile.telephone1 = record.Attributes.Contains("telephone1") ? record["telephone1"].ToString() : "";
                        crmUserProfile.address1_stateorprovince = record.Attributes.Contains("address1_stateorprovince") ? record["address1_stateorprovince"].ToString() : "";
                        crmUserProfile.address1_postalcode = record.Attributes.Contains("address1_postalcode") ? record["address1_postalcode"].ToString() : "";
                        if (record.Attributes.Contains("entityimage")) crmUserProfile.entityimage = record["entityimage"] as byte[];
                        if (record.Attributes.Contains("birthdate")) { crmUserProfile.birthdate = (DateTime)record.Attributes["birthdate"]; }
                        crmUserProfile.gender = record.Attributes.Contains("gendercode") ? record.FormattedValues["gendercode"].ToString() : string.Empty;

                        crmUserProfile.gendercode = record.Contains("gendercode") && record["gendercode"] != null ? Convert.ToInt32(((OptionSetValue)record["gendercode"]).Value) : 0;
                        crmUserProfile.bcrm_policynumber = record.Attributes.Contains("bcrm_policynumber") ? record["bcrm_policynumber"].ToString() : "";
                        crmUserProfile.bcrm_healthcodenumber = record.Attributes.Contains("bcrm_healthcodenumber") ? record["bcrm_healthcodenumber"].ToString() : "";
                        crmUserProfile.bcrm_patientnumber = record.Attributes.Contains("bcrm_patientnumber") ? record["bcrm_patientnumber"].ToString() : "";
                        crmUserProfile.bcrm_nhsnumber = record.Attributes.Contains("bcrm_nhsnumber") ? record["bcrm_nhsnumber"].ToString() : "";
                        crmUserProfile.bcrm_ifrnhsno = record.Attributes.Contains("bcrm_ifrnhsno") ? record["bcrm_ifrnhsno"].ToString() : "";
                        crmUserProfile.bcrm_funded_name = record.Attributes.Contains("bcrm_funded") ? record.FormattedValues["bcrm_funded"].ToString() : string.Empty;
                        crmUserProfile.bcrm_funded = record.Contains("bcrm_funded") && record["bcrm_funded"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_funded"]).Value) : 0;
                        crmUserProfile.preferredcontactmethodcode = record.Contains("preferredcontactmethodcode") && record["preferredcontactmethodcode"] != null ? Convert.ToInt32(((OptionSetValue)record["preferredcontactmethodcode"]).Value) : 0;
                        crmUserProfile.parentcustomerid = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Id : Guid.Empty;
                        crmUserProfile.parentcustomername = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Name : string.Empty;
                        crmUserProfile.donotemail = record.Attributes.Contains("donotemail") ? ((bool)record["donotemail"]) : false;
                        crmUserProfile.followemail = record.Attributes.Contains("followemail") ? ((bool)record["followemail"]) : false;
                        crmUserProfile.donotbulkemail = record.Attributes.Contains("donotbulkemail") ? ((bool)record["donotbulkemail"]) : false;
                        crmUserProfile.donotphone = record.Attributes.Contains("donotphone") ? ((bool)record["donotphone"]) : false;
                        crmUserProfile.donotpostalmail = record.Attributes.Contains("donotpostalmail") ? ((bool)record["donotpostalmail"]) : false;
                        crmUserProfile.donotfax = record.Attributes.Contains("donotfax") ? ((bool)record["donotfax"]) : false;
                        //Fields from Doctor's Now
                        crmUserProfile.msemr_contact1relationship = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Id : Guid.Empty;
                        crmUserProfile.msemr_contact1relationship_name = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Name : string.Empty;
                        crmUserProfile.msemr_contact1 = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Id : Guid.Empty;
                        crmUserProfile.emergencycontact = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Name : String.Empty;
                        crmUserProfile.bcrm_wheredidyouhearaboutus = record.Contains("bcrm_wheredidyouhearaboutus") && record["bcrm_wheredidyouhearaboutus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_wheredidyouhearaboutus"]).Value) : 0;
                        crmUserProfile.bcrm_gptype = record.Contains("bcrm_gptype") && record["bcrm_gptype"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_gptype"]).Value) : 0;
                        crmUserProfile.bcrm_iunderstandthatdoctornowisaprivategpservi = record.Attributes.Contains("bcrm_iunderstandthatdoctornowisaprivategpservi") ? ((bool)record["bcrm_iunderstandthatdoctornowisaprivategpservi"]) : false;
                        crmUserProfile.bcrm_iunderstandthatacancellationfeewillbeappl = record.Attributes.Contains("bcrm_iunderstandthatacancellationfeewillbeappl") ? ((bool)record["bcrm_iunderstandthatacancellationfeewillbeappl"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivetextmessagesfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceivetextmessagesfromdoctornow") ? ((bool)record["bcrm_iwishtoreceivetextmessagesfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceiveemailsfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceiveemailsfromdoctornow") ? ((bool)record["bcrm_iwishtoreceiveemailsfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivepostalcommunicationfromdoct = record.Attributes.Contains("bcrm_iwishtoreceivepostalcommunicationfromdoct") ? ((bool)record["bcrm_iwishtoreceivepostalcommunicationfromdoct"]) : false;
                        crmUserProfile.bcrm_iwouldliketoreceiveupdatesaboutnewservice = record.Attributes.Contains("bcrm_iwouldliketoreceiveupdatesaboutnewservice") ? ((bool)record["bcrm_iwouldliketoreceiveupdatesaboutnewservice"]) : false;
                        crmUserProfile.bcrm_iamhappyfordoctornowtocontactmynextofkin = record.Attributes.Contains("bcrm_iamhappyfordoctornowtocontactmynextofkin") ? ((bool)record["bcrm_iamhappyfordoctornowtocontactmynextofkin"]) : false;
                        crmUserProfile.bcrm_smokingstatus = record.Contains("bcrm_smokingstatus") && record["bcrm_smokingstatus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_smokingstatus"]).Value) : 0;
                        crmUserProfile.bcrm_occuption = record.Attributes.Contains("bcrm_occuption") ? record["bcrm_occuption"].ToString() : "";
                        crmUserProfile.bcrm_stoppedsmoking = record.Attributes.Contains("bcrm_stoppedsmoking") ? record["bcrm_stoppedsmoking"].ToString() : "";
                        crmUserProfile.bcrm_stoppeddrinkingalcohol = record.Attributes.Contains("bcrm_stoppeddrinkingalcohol") ? record["bcrm_stoppeddrinkingalcohol"].ToString() : "";
                        crmUserProfile.bcrm_alcoholintake = record.Contains("bcrm_alcoholintake") && record["bcrm_alcoholintake"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_alcoholintake"]).Value) : 0;
                        crmUserProfile.bcrm_alcoholintake_name = record.Attributes.Contains("bcrm_alcoholintake") ? record.FormattedValues["bcrm_alcoholintake"].ToString() : string.Empty;
                        crmUserProfile.otherDetails = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                        crmUserProfile.bcrm_howmanyunitsofalcoholdoyoudrinkinanavera = record.Attributes.Contains("bcrm_howmanyunitsofalcoholdoyoudrinkinanavera") ? record["bcrm_howmanyunitsofalcoholdoyoudrinkinanavera"].ToString() : string.Empty;
                        crmUserProfile.bcrm_neverdrinkalcohol = record.Attributes.Contains("bcrm_neverdrinkalcohol") ? ((bool)record["bcrm_neverdrinkalcohol"]) : false;
                        crmUserProfile.bcrm_diabetes = record.Attributes.Contains("bcrm_diabetes") ? ((bool)record["bcrm_diabetes"]) : false;
                        crmUserProfile.bcrm_highbloodpressure = record.Attributes.Contains("bcrm_highbloodpressure") ? ((bool)record["bcrm_highbloodpressure"]) : false;
                        crmUserProfile.bcrm_heartdisease = record.Attributes.Contains("bcrm_heartdisease") ? ((bool)record["bcrm_heartdisease"]) : false;
                        crmUserProfile.bcrm_kidneydisease = record.Attributes.Contains("bcrm_kidneydisease") ? ((bool)record["bcrm_kidneydisease"]) : false;
                        crmUserProfile.bcrm_cancer = record.Attributes.Contains("bcrm_cancer") ? ((bool)record["bcrm_cancer"]) : false;
                        crmUserProfile.bcrm_thyroiddisease = record.Attributes.Contains("bcrm_thyroiddisease") ? ((bool)record["bcrm_thyroiddisease"]) : false;
                        crmUserProfile.bcrm_epilepsy = record.Attributes.Contains("bcrm_epilepsy") ? ((bool)record["bcrm_epilepsy"]) : false;
                        crmUserProfile.bcrm_depression = record.Attributes.Contains("bcrm_depression") ? ((bool)record["bcrm_depression"]) : false;
                        crmUserProfile.bcrm_stroke = record.Attributes.Contains("bcrm_stroke") ? ((bool)record["bcrm_stroke"]) : false;
                        crmUserProfile.bcrm_asthma = record.Attributes.Contains("bcrm_asthma") ? ((bool)record["bcrm_asthma"]) : false;
                        crmUserProfile.bcrm_transientischaemicattack = record.Attributes.Contains("bcrm_transientischaemicattack") ? ((bool)record["bcrm_transientischaemicattack"]) : false;
                        crmUserProfile.bcrm_mentalillness = record.Attributes.Contains("bcrm_mentalillness") ? ((bool)record["bcrm_mentalillness"]) : false;
                        crmUserProfile.bcrm_heartrhythmproblems = record.Attributes.Contains("bcrm_heartrhythmproblems") ? ((bool)record["bcrm_heartrhythmproblems"]) : false;
                        crmUserProfile.bcrm_pleasegivedetailsofanyothersignificantill = record.Attributes.Contains("bcrm_pleasegivedetailsofanyothersignificantill") ? record["bcrm_pleasegivedetailsofanyothersignificantill"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsofanymedicalconditionswi = record.Attributes.Contains("bcrm_pleasegivedetailsofanymedicalconditionswi") ? record["bcrm_pleasegivedetailsofanymedicalconditionswi"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsifyouhaveanyallergiestoa = record.Attributes.Contains("bcrm_pleasegivedetailsifyouhaveanyallergiestoa") ? record["bcrm_pleasegivedetailsifyouhaveanyallergiestoa"].ToString() : "";
                        crmUserProfile.bcrm_kinsfirstname = record.Attributes.Contains("bcrm_kinsfirstname") ? record["bcrm_kinsfirstname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_kinslastname = record.Attributes.Contains("bcrm_kinslastname") ? record["bcrm_kinslastname"].ToString() : string.Empty;
                        crmUserProfile.address2_line1 = record.Attributes.Contains("address2_line1") ? record["address2_line1"].ToString() : string.Empty;
                        crmUserProfile.emailaddress2 = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.address2_postalcode = record.Attributes.Contains("address2_postalcode") ? record["address2_postalcode"].ToString() : string.Empty;
                        crmUserProfile.bcrm_gpsurgeryname = record.Attributes.Contains("bcrm_gpsurgeryname") ? record["bcrm_gpsurgeryname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_doctorsname = record.Attributes.Contains("bcrm_doctorsname") ? record["bcrm_doctorsname"].ToString() : string.Empty;
                        crmUserProfile.address3_city = record.Attributes.Contains("address3_city") ? record["address3_city"].ToString() : string.Empty;
                        crmUserProfile.telephone3 = record.Attributes.Contains("telephone3") ? record["telephone3"].ToString() : string.Empty;
                        crmUserProfile.bcrm_donotallowsms = record.Contains("bcrm_donotallowsms") && record["bcrm_donotallowsms"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_donotallowsms"]).Value) : 0;
                        crmUserProfile.GPCSequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_gpc_registractionperiod")) { crmUserProfile.GPCRegistractionDate = (DateTime)record.Attributes["bcrm_gpc_registractionperiod"]; }
                        crmUserProfile.HomePhone1 = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.PdsVersionId = record.Attributes.Contains("bcrm_pdsversionid") ? record["bcrm_pdsversionid"].ToString() : string.Empty;
                        crmUserProfile.pdsJson = record.Attributes.Contains("bcrm_pdsjson") ? record["bcrm_pdsjson"].ToString() : string.Empty;

                        crmUserProfile.InterpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Code = record.Attributes.Contains("bcrm_nhsnumberverificationstatus") ? record["bcrm_nhsnumberverificationstatus"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Display = record.Attributes.Contains("bcrm_nhsnumberverificationstatusdisplay") ? record["bcrm_nhsnumberverificationstatusdisplay"].ToString() : string.Empty;

                        crmUserProfile.homeEmail = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.homePhone = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.workEmail = record.Attributes.Contains("emailaddress3") ? record["emailaddress3"].ToString() : string.Empty;
                        crmUserProfile.workPhone = record.Attributes.Contains("address2_telephone1") ? record["address2_telephone1"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;


                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            crmUserProfile.statusReason = true;
                        }
                        else
                        {
                            crmUserProfile.statusReason = false;
                        }

                        if (crmUserProfile.pdsJson != "")
                        {
                            try
                            {
                                //var PatientData = JsonConvert.DeserializeObject<PDSDTO>(crmUserProfile.pdsJson);
                                //crmUserProfile.NHS_number_Verification_Status_Code = PatientData.identifier[0].extension[0].valueCodeableConcept.coding[0].code;
                                //crmUserProfile.NHS_number_Verification_Status_Display = PatientData.identifier[0].extension[0].valueCodeableConcept.coding[0].display;
                                //crmUserProfile.LanguageCode = PatientData.extension[2].extension[0].valueCodeableConcept.coding[0].code;
                                //crmUserProfile.LanguageDisplay = PatientData.extension[2].extension[0].valueCodeableConcept.coding[0].display;
                                //crmUserProfile.InterpreterRequired = PatientData.extension[2].extension[1].valueBoolean.ToString();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message.ToString());
                            }
                        }
                        var patientLanguages = record.Attributes.Contains("bcrm_languages") ? record.FormattedValues["bcrm_languages"].ToString() : string.Empty;
                        if (patientLanguages != "")
                        {
                            string[] allLanguages = patientLanguages.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            crmUserProfile.patientLanguages = languageList;

                        }

                        var relatedPerson = GetRelatedPerson(crmUserProfile.Id.ToString(), crmUserProfile);

                        AllPatientDetails.Add(relatedPerson);
                    }

                    if (AllPatientDetails.Count > 0)
                    {
                        if (source == "Internal")
                        {
                            return pd.ReadAPatientUsingJSONFHIR(AllPatientDetails[0]);
                        }


                        var res = pd.ReadAPatientUsingJSONFHIR(AllPatientDetails[0]);
                        finaljson[0] = res;
                        finaljson[1] = crmUserProfile.PdsVersionId;
                        finaljson[2] = "Success";
                        return finaljson;

                    }
                }
                PatientDetails pd1 = new PatientDetails();

                finaljson[0] = pd1.PatientNotFoundUsingJSONFHIR(id);
                finaljson[1] = "";
                finaljson[2] = "NotFound";
                return finaljson;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic FindAPractioner(string sdsId, string identifier, string sspInteractionId, int identifierCount, string fullUrl)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];

                PractionerDetails praDet = new PractionerDetails();

                if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:search:practitioner-1")
                {
                    finaljson[0] = praDet.InvalidSSPInteractionId();
                    finaljson[1] = "";
                    finaljson[2] = "InvalidInteractionId";
                    return finaljson;
                }

                if (identifierCount > 1)
                {
                    finaljson[0] = praDet.MultipleIndentifierExist(identifierCount);
                    finaljson[1] = "";
                    finaljson[2] = "MultipleIdentifier";
                    return finaljson;

                }

                if (fullUrl.Contains("Identifier"))
                {
                    finaljson[0] = praDet.IndentifierSpellingMistake();
                    finaljson[1] = "";
                    finaljson[2] = "IdentifierSpellingMistake";
                    return finaljson;
                }


                if (!identifier.Contains("|"))
                {
                    finaljson[0] = praDet.MissingIdentifier(identifier);
                    finaljson[1] = "";
                    finaljson[2] = "InvalidParameter";
                    return finaljson;
                }
                else
                {
                    if (!identifier.Contains("https://fhir.nhs.uk/Id/sds-user-id"))
                    {
                        finaljson[0] = praDet.MissingIdentifier(identifier);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidParameter";
                        return finaljson;
                    }
                    else if (sdsId == "")
                    {
                        finaljson[0] = praDet.MissingIdentifier(identifier);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidParameter";
                        return finaljson;
                    }
                }


                if (!identifier.Contains("https://fhir.nhs.uk/Id/sds-user-id"))
                {
                    finaljson[0] = praDet.MissingIdentifier(identifier);
                    finaljson[1] = "";
                    finaljson[2] = "InvalidIdentifier";
                    return finaljson;
                }

                string stuffXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='bcrm_staff'>
                            <attribute name='bcrm_staffid' />
                            <attribute name='bcrm_name' />
                            <attribute name='createdon' />
                            <attribute name='bcrm_uuid' />
                            <attribute name='bcrm_userinfo' />
                            <attribute name='bcrm_user' />
                            <attribute name='bcrm_type' />
                            <attribute name='bcrm_tenantid' />
                            <attribute name='statuscode' />
                            <attribute name='bcrm_status' />
                            <attribute name='statecode' />
                            <attribute name='bcrm_starttime' />
                            <attribute name='bcrm_staffrole' />
                            <attribute name='bcrm_staff_uniqueid' />
                            <attribute name='bcrm_staffgroup' />
                            <attribute name='bcrm_stafffilename' />
                            <attribute name='bcrm_signature' />
                            <attribute name='bcrm_sessionid' />
                            <attribute name='bcrm_roles' />
                            <attribute name='bcrm_resourcegrpvideocode' />
                            <attribute name='bcrm_resourcegrptelecode' />
                            <attribute name='bcrm_resourcegrpf2fcode' />
                            <attribute name='overriddencreatedon' />
                            <attribute name='bcrm_recordcreatedby' />
                            <attribute name='bcrm_recordchangedon' />
                            <attribute name='bcrm_modifiedby' />
                            <attribute name='bcrm_professionalcode' />
                            <attribute name='bcrm_professionalbody' />
                            <attribute name='bcrm_postcode' />
                            <attribute name='owningbusinessunit' />
                            <attribute name='ownerid' />
                            <attribute name='modifiedon' />
                            <attribute name='modifiedonbehalfby' />
                            <attribute name='modifiedby' />
                            <attribute name='bcrm_mobilephone' />
                            <attribute name='bcrm_mncnumber' />
                            <attribute name='bcrm_lastname' />
                            <attribute name='bcrm_issessionactive' />
                            <attribute name='bcrm_isavailableforbooking' />
                            <attribute name='bcrm_issatffactive' />
                            <attribute name='bcrm_haslicense' />
                            <attribute name='bcrm_gpc_versionid' />
                            <attribute name='bcrm_gpc_sequence_number' />
                            <attribute name='bcrm_gpc_sdsid' />
                            <attribute name='bcrm_gpc_resourceid' />
                            <attribute name='bcrm_gpc_prefix' />
                            <attribute name='bcrm_gpc_odscode' />
                            <attribute name='bcrm_gpc_name_use' />
                            <attribute name='bcrm_gpc_name_given' />
                            <attribute name='bcrm_gpc_name_family' />
                            <attribute name='bcrm_gpc_humanlanguagecodeanddisplay' />
                            <attribute name='bcrm_gmcnumber' />
                            <attribute name='bcrm_gender' />
                            <attribute name='bcrm_fulltimeequivalent' />
                            <attribute name='bcrm_firstname' />
                            <attribute name='bcrm_expirydate' />
                            <attribute name='bcrm_endtime' />
                            <attribute name='bcrm_email' />
                            <attribute name='bcrm_dateofbirth' />
                            <attribute name='createdonbehalfby' />
                            <attribute name='createdby' />
                            <attribute name='bcrm_countryqualificationgroup' />
                            <attribute name='bcrm_countryofqualificationarea' />
                            <attribute name='bcrm_clinic' />
                            <attribute name='bcrm_calledother' />
                            <attribute name='bcrm_calledmessage' />
                            <attribute name='bcrm_calledapikey' />
                            <attribute name='bcrm_calendarfilters' />
                            <attribute name='bcrm_booking_staffid' />
                            <attribute name='bcrm_bookingemail' />
                            <attribute name='bcrm_associatedorganisations' />
                            <attribute name='bcrm_aliases' />
                            <attribute name='bcrm_ageband' />
                            <attribute name='bcrm_gpccommunicationproficiency' />
                            <attribute name='bcrm_interpreterrequired' />
                            <attribute name='bcrm_gpcmodeofcommunication' />
                            <attribute name='bcrm_age' />
                            <attribute name='bcrm_aduserid' />
                            <attribute name='bcrm_activatedon' />
                            <order attribute='bcrm_name' descending='false' />
                            <filter type='and'>
                              <condition attribute='bcrm_gpc_sdsid' operator='eq' value='" + sdsId + @"' />
                            </filter>
                          </entity>
                        </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(stuffXML));

                //Entity MailEntity = new Entity("bcrm_staff", new Guid("c1486f15-fbbf-ee11-9079-0022481ab71c"));
                //MailEntity["bcrm_gpc_versionid"] = "5";
                //_crmServiceClient.Update(MailEntity);


                PractionerDTO practionerDetails = new PractionerDTO();


                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        practionerDetails.resourceId = record.Id.ToString();
                        if (record.Attributes.Contains("modifiedon")) { practionerDetails.modifiedDate = (DateTime)record.Attributes["modifiedon"]; }
                        practionerDetails.sequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        practionerDetails.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;

                        practionerDetails.modeOfCommunication = record.Attributes.Contains("bcrm_gpcmodeofcommunication") ? record.FormattedValues["bcrm_gpcmodeofcommunication"].ToString() : string.Empty;
                        practionerDetails.communucationProficiency = record.Attributes.Contains("bcrm_gpccommunicationproficiency") ? record.FormattedValues["bcrm_gpccommunicationproficiency"].ToString() : string.Empty;
                        practionerDetails.interpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString().ToLower() : string.Empty;

                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            practionerDetails.currentStatus = true;
                        }
                        else
                        {
                            practionerDetails.currentStatus = false;
                        }

                        practionerDetails.sdsId = record.Attributes.Contains("bcrm_gpc_sdsid") ? record["bcrm_gpc_sdsid"].ToString() : string.Empty;
                        practionerDetails.family = record.Attributes.Contains("bcrm_gpc_name_family") ? record["bcrm_gpc_name_family"].ToString() : string.Empty;
                        practionerDetails.given = record.Attributes.Contains("bcrm_gpc_name_given") ? record["bcrm_gpc_name_given"].ToString() : string.Empty;

                        //  practionerDetails.gender = record.Attributes.Contains("bcrm_gender") ? record["bcrm_gender"].ToString() : string.Empty;

                        practionerDetails.gender = record.Attributes.Contains("bcrm_gender") ? record.FormattedValues["bcrm_gender"].ToString().ToLower() : string.Empty;

                        practionerDetails.prefix = record.Attributes.Contains("bcrm_gpc_prefix") ? record.FormattedValues["bcrm_gpc_prefix"].ToString().ToLower() : string.Empty;

                        var stufflanguage = record.Attributes.Contains("bcrm_gpc_humanlanguagecodeanddisplay") ? record.FormattedValues["bcrm_gpc_humanlanguagecodeanddisplay"].ToString() : string.Empty;
                        if (stufflanguage != "")
                        {
                            string[] allLanguages = stufflanguage.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            practionerDetails.practitionerLanguages = languageList;

                        }

                        if (practionerDetails.sdsId != null)
                        {
                            practionerDetails.JobRoles = getPractitionerRoleCodes(practionerDetails.sdsId);
                        }

                    }

                    var json = praDet.FindAPractitionerUsingJSONFHIR(practionerDetails);

                    finaljson[0] = json;
                    finaljson[1] = practionerDetails.versionId;
                    finaljson[2] = "Success";
                    return finaljson;

                }
                if (AnswerCollection.Entities.Count == 0)
                {

                    var json = praDet.MakeEmptyBuddlePractitionerJSON();
                    finaljson[0] = json;
                    finaljson[1] = "";
                    finaljson[2] = "Success";
                    return finaljson;

                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic ReadAPractioner(string id, string sspInteractionId, string source)
        {
            try
            {

                dynamic[] finaljson = new dynamic[3];
                if (source == "External")
                {
                    if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:read:practitioner-1")
                    {
                        PatientDetails pd = new PatientDetails();
                        var res = pd.WrongInteractionId(sspInteractionId, "Practitioner");
                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "InvalidInteractionId";
                        return finaljson;

                    }
                }

                string stuffXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='bcrm_staff'>
                            <attribute name='bcrm_staffid' />
                            <attribute name='bcrm_name' />
                            <attribute name='createdon' />
                            <attribute name='bcrm_uuid' />
                            <attribute name='bcrm_userinfo' />
                            <attribute name='bcrm_user' />
                            <attribute name='bcrm_type' />
                            <attribute name='bcrm_tenantid' />
                            <attribute name='statuscode' />
                            <attribute name='bcrm_status' />
                            <attribute name='statecode' />
                            <attribute name='bcrm_starttime' />
                            <attribute name='bcrm_staffrole' />
                            <attribute name='bcrm_staff_uniqueid' />
                            <attribute name='bcrm_staffgroup' />
                            <attribute name='bcrm_stafffilename' />
                            <attribute name='bcrm_signature' />
                            <attribute name='bcrm_sessionid' />
                            <attribute name='bcrm_roles' />
                            <attribute name='bcrm_resourcegrpvideocode' />
                            <attribute name='bcrm_resourcegrptelecode' />
                            <attribute name='bcrm_resourcegrpf2fcode' />
                            <attribute name='overriddencreatedon' />
                            <attribute name='bcrm_recordcreatedby' />
                            <attribute name='bcrm_recordchangedon' />
                            <attribute name='bcrm_modifiedby' />
                            <attribute name='bcrm_professionalcode' />
                            <attribute name='bcrm_professionalbody' />
                            <attribute name='bcrm_postcode' />
                            <attribute name='owningbusinessunit' />
                            <attribute name='ownerid' />
                            <attribute name='modifiedon' />
                            <attribute name='modifiedonbehalfby' />
                            <attribute name='modifiedby' />
                            <attribute name='bcrm_mobilephone' />
                            <attribute name='bcrm_mncnumber' />
                            <attribute name='bcrm_lastname' />
                            <attribute name='bcrm_issessionactive' />
                            <attribute name='bcrm_isavailableforbooking' />
                            <attribute name='bcrm_issatffactive' />
                            <attribute name='bcrm_haslicense' />
                            <attribute name='bcrm_gpc_versionid' />
                            <attribute name='bcrm_gpc_sequence_number' />
                            <attribute name='bcrm_gpc_sdsid' />
                            <attribute name='bcrm_gpc_resourceid' />
                            <attribute name='bcrm_gpc_prefix' />
                            <attribute name='bcrm_gpc_odscode' />
                            <attribute name='bcrm_gpc_name_use' />
                            <attribute name='bcrm_gpc_name_given' />
                            <attribute name='bcrm_gpc_name_family' />
                            <attribute name='bcrm_gpc_humanlanguagecodeanddisplay' />
                            <attribute name='bcrm_gmcnumber' />
                            <attribute name='bcrm_gender' />
                            <attribute name='bcrm_fulltimeequivalent' />
                            <attribute name='bcrm_firstname' />
                            <attribute name='bcrm_expirydate' />
                            <attribute name='bcrm_endtime' />
                            <attribute name='bcrm_email' />
                            <attribute name='bcrm_dateofbirth' />
                            <attribute name='createdonbehalfby' />
                            <attribute name='createdby' />
                            <attribute name='bcrm_countryqualificationgroup' />
                            <attribute name='bcrm_countryofqualificationarea' />
                            <attribute name='bcrm_clinic' />
                            <attribute name='bcrm_calledother' />
                            <attribute name='bcrm_calledmessage' />
                            <attribute name='bcrm_calledapikey' />
                            <attribute name='bcrm_calendarfilters' />
                            <attribute name='bcrm_booking_staffid' />
                            <attribute name='bcrm_bookingemail' />
                            <attribute name='bcrm_associatedorganisations' />
                            <attribute name='bcrm_aliases' />
                            <attribute name='bcrm_ageband' />
                            <attribute name='bcrm_gpccommunicationproficiency' />
                            <attribute name='bcrm_interpreterrequired' />
                            <attribute name='bcrm_gpcmodeofcommunication' />
                            <attribute name='bcrm_age' />
                            <attribute name='bcrm_aduserid' />
                            <attribute name='bcrm_activatedon' />
                            <order attribute='bcrm_name' descending='false' />
                            <filter type='and'>
                              <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + id + @"' />
                            </filter>
                          </entity>
                        </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(stuffXML));

                PractionerDTO practionerDetails = new PractionerDTO();

                PractionerDetails praDet = new PractionerDetails();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        practionerDetails.resourceId = record.Id.ToString();
                        if (record.Attributes.Contains("modifiedon")) { practionerDetails.modifiedDate = (DateTime)record.Attributes["modifiedon"]; }
                        practionerDetails.sequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        practionerDetails.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;

                        practionerDetails.modeOfCommunication = record.Attributes.Contains("bcrm_gpcmodeofcommunication") ? record.FormattedValues["bcrm_gpcmodeofcommunication"].ToString() : string.Empty;
                        practionerDetails.communucationProficiency = record.Attributes.Contains("bcrm_gpccommunicationproficiency") ? record.FormattedValues["bcrm_gpccommunicationproficiency"].ToString() : string.Empty;
                        practionerDetails.interpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString().ToLower() : string.Empty;

                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            practionerDetails.currentStatus = true;
                        }
                        else
                        {
                            practionerDetails.currentStatus = false;
                        }

                        practionerDetails.sdsId = record.Attributes.Contains("bcrm_gpc_sdsid") ? record["bcrm_gpc_sdsid"].ToString() : string.Empty;
                        practionerDetails.family = record.Attributes.Contains("bcrm_gpc_name_family") ? record["bcrm_gpc_name_family"].ToString() : string.Empty;
                        practionerDetails.given = record.Attributes.Contains("bcrm_gpc_name_given") ? record["bcrm_gpc_name_given"].ToString() : string.Empty;

                        //  practionerDetails.gender = record.Attributes.Contains("bcrm_gender") ? record["bcrm_gender"].ToString() : string.Empty;

                        practionerDetails.gender = record.Attributes.Contains("bcrm_gender") ? record.FormattedValues["bcrm_gender"].ToString().ToLower() : string.Empty;

                        practionerDetails.prefix = record.Attributes.Contains("bcrm_gpc_prefix") ? record.FormattedValues["bcrm_gpc_prefix"].ToString().ToLower() : string.Empty;

                        var stufflanguage = record.Attributes.Contains("bcrm_gpc_humanlanguagecodeanddisplay") ? record.FormattedValues["bcrm_gpc_humanlanguagecodeanddisplay"].ToString() : string.Empty;
                        if (stufflanguage != "")
                        {
                            string[] allLanguages = stufflanguage.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            practionerDetails.practitionerLanguages = languageList;

                        }
                        if (practionerDetails.sdsId != null)
                        {
                            practionerDetails.JobRoles = getPractitionerRoleCodes(practionerDetails.sdsId);
                        }
                    }

                    if (source == "Internal")
                    {
                        return praDet.ReadAPractitionerUsingJSONFHIR(practionerDetails);
                    }


                    var json = praDet.ReadAPractitionerUsingJSONFHIR(practionerDetails);
                    finaljson[0] = json;
                    finaljson[1] = practionerDetails.versionId;
                    finaljson[2] = "Success";
                    return finaljson;
                }

                finaljson[0] = praDet.NoPractitionerFound(id);
                finaljson[1] = "";
                finaljson[2] = "NotFound";
                return finaljson;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic FindAOrganization(string odsCode, string SspInterctionId)
        {
            try
            {
                OrganisationDetails od = new OrganisationDetails();
                dynamic[] finaljson = new dynamic[3];

                if (!SspInterctionId.Contains("urn:nhs:names:services:gpconnect:fhir:rest:search:organization-1"))
                {
                    finaljson[0] = od.WrongSSPinteractionId();
                    finaljson[1] = "";
                    finaljson[2] = "InvalidInteractionId";
                }


                string organizationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                       <entity name='bcrm_clinic'>
                         <attribute name='bcrm_clinicid' />
                         <attribute name='bcrm_name' />
                         <attribute name='createdon' />
                         <attribute name='bcrm_treatment_stage_name' />
                         <attribute name='bcrm_treatment_stage_guid' />
                         <attribute name='bcrm_treatment_cycle_name' />
                         <attribute name='bcrm_sub' />
                         <attribute name='bcrm_locality' />
                         <attribute name='bcrm_street_name' />
                         <attribute name='statuscode' />
                         <attribute name='statecode' />
                         <attribute name='bcrm_sortcode' />
                         <attribute name='bcrm_region' />
                         <attribute name='overriddencreatedon' />
                         <attribute name='bcrm_publicurl' />
                         <attribute name='bcrm_postal_code' />
                         <attribute name='bcrm_phone' />
                         <attribute name='bcrm_pcnname' />
                         <attribute name='bcrm_pcncode' />
                         <attribute name='bcrm_parentclinic' />
                         <attribute name='bcrm_outofhoursphone' />
                         <attribute name='bcrm_otherphone' />
                         <attribute name='bcrm_otheremail' />
                         <attribute name='bcrm_othercontact' />
                         <attribute name='bcrm_odscode' />
                         <attribute name='bcrm_fullclinicname' />
                         <attribute name='modifiedon' />
                         <attribute name='modifiedonbehalfby' />
                         <attribute name='modifiedby' />
                         <attribute name='bcrm_logourl' />
                         <attribute name='bcrm_last_registration_number' />
                         <attribute name='bcrm_icb' />
                         <attribute name='bcrm_iban' />
                         <attribute name='bcrm_house_number_street_name' />
                         <attribute name='bcrm_gpc_versionid' />
                         <attribute name='bcrm_gpc_telecom_value' />
                         <attribute name='bcrm_gpc_telecom_use' />
                         <attribute name='bcrm_gpc_telecom_system' />
                         <attribute name='bcrm_gpc_sequence_number' />
                         <attribute name='bcrm_gpc_address_use' />
                         <attribute name='bcrm_gpc_address_district' />
                         <attribute name='bcrm_footerfield' />
                         <attribute name='bcrm_financephone' />
                         <attribute name='bcrm_financeemail' />
                         <attribute name='bcrm_financecontact' />
                         <attribute name='bcrm_fax' />
                         <attribute name='bcrm_email' />
                         <attribute name='bcrm_datainfo' />
                         <attribute name='bcrm_currency' />
                         <attribute name='createdonbehalfby' />
                         <attribute name='createdby' />
                         <attribute name='bcrm_country' />
                         <attribute name='bcrm_clinic_uk_centre_code' />
                         <attribute name='bcrm_clinictype' />
                         <attribute name='bcrm_clinicinfo' />
                         <attribute name='bcrm_city_or_town' />
                         <attribute name='bcrm_cimar_vanity_name' />
                         <attribute name='bcrm_cimar_user_account_uuid' />
                         <attribute name='bcrm_cimar_passphrase' />
                         <attribute name='bcrm_cimar_iv_hex_key' />
                         <attribute name='bcrm_businessuser' />
                         <attribute name='bcrm_businessunituserid' />
                         <attribute name='bcrm_businessunitusername' />
                         <attribute name='bcrm_bic' />
                         <attribute name='bcrm_bank' />
                         <attribute name='bcrm_address' />
                         <attribute name='bcrm_accountname' />
                         <attribute name='bcrm_accountcode' />
                         <order attribute='bcrm_name' descending='false' />
                         <filter type='and'>
                             <condition attribute='bcrm_odscode' operator='eq' value='" + odsCode + @"' />
                         </filter>
                       </entity>
                     </fetch>";


                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(organizationXML));




                OrganizationDTO organizationDetails = new OrganizationDTO();

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {

                    var record = AnswerCollection.Entities[0];

                    organizationDetails.resourceId = record.Id.ToString();
                    if (record.Attributes.Contains("modifiedon")) { organizationDetails.lastUpdated = (DateTime)record.Attributes["modifiedon"]; }
                    organizationDetails.sequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                    organizationDetails.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;

                    organizationDetails.odsCode = record.Attributes.Contains("bcrm_odscode") ? record["bcrm_odscode"].ToString() : string.Empty;
                    organizationDetails.phoneNumber = record.Attributes.Contains("bcrm_gpc_telecom_value") ? record["bcrm_gpc_telecom_value"].ToString() : string.Empty;
                    organizationDetails.addressLine = record.Attributes.Contains("bcrm_house_number_street_name") ? record["bcrm_house_number_street_name"].ToString() : string.Empty;
                    organizationDetails.city = record.Attributes.Contains("bcrm_city_or_town") ? record["bcrm_city_or_town"].ToString() : string.Empty;
                    organizationDetails.district = record.Attributes.Contains("bcrm_gpc_address_district") ? record["bcrm_gpc_address_district"].ToString() : string.Empty;
                    organizationDetails.postalCode = record.Attributes.Contains("bcrm_postal_code") ? record["bcrm_postal_code"].ToString() : string.Empty;
                    organizationDetails.organizationName = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;

                    dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                    var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                    if (statusValue == 1)
                    {
                        organizationDetails.currentStatus = true;
                    }
                    else
                    {
                        organizationDetails.currentStatus = false;
                    }






                    var json = od.FindOrganizationFHIRJSON(organizationDetails);


                    finaljson[0] = json;
                    finaljson[1] = organizationDetails.versionId;
                    finaljson[2] = "Success";
                    return finaljson;
                }

                finaljson[0] = od.MakeEmptyBuddleOrganizationJSON();
                finaljson[1] = "";
                finaljson[2] = "Success";
                return finaljson;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic ReadAOrganization(string id, string sspInteractionId, string source)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                if (source == "External")
                {
                    if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:read:organization-1")
                    {
                        PatientDetails pd = new PatientDetails();
                        var res = pd.WrongInteractionId(sspInteractionId, "Organization");
                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "InvalidInteractionId";
                        return finaljson;
                    }
                }

                string organizationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                 <entity name='bcrm_clinic'>
                   <attribute name='bcrm_clinicid' />
                   <attribute name='bcrm_name' />
                   <attribute name='createdon' />
                   <attribute name='bcrm_treatment_stage_name' />
                   <attribute name='bcrm_treatment_stage_guid' />
                   <attribute name='bcrm_treatment_cycle_name' />
                   <attribute name='bcrm_sub' />
                   <attribute name='bcrm_locality' />
                   <attribute name='bcrm_street_name' />
                   <attribute name='statuscode' />
                   <attribute name='statecode' />
                   <attribute name='bcrm_sortcode' />
                   <attribute name='bcrm_region' />
                   <attribute name='overriddencreatedon' />
                   <attribute name='bcrm_publicurl' />
                   <attribute name='bcrm_postal_code' />
                   <attribute name='bcrm_phone' />
                   <attribute name='bcrm_pcnname' />
                   <attribute name='bcrm_pcncode' />
                   <attribute name='bcrm_parentclinic' />
                   <attribute name='bcrm_outofhoursphone' />
                   <attribute name='bcrm_otherphone' />
                   <attribute name='bcrm_otheremail' />
                   <attribute name='bcrm_othercontact' />
                   <attribute name='bcrm_odscode' />
                   <attribute name='bcrm_fullclinicname' />
                   <attribute name='modifiedon' />
                   <attribute name='modifiedonbehalfby' />
                   <attribute name='modifiedby' />
                   <attribute name='bcrm_logourl' />
                   <attribute name='bcrm_last_registration_number' />
                   <attribute name='bcrm_icb' />
                   <attribute name='bcrm_iban' />
                   <attribute name='bcrm_house_number_street_name' />
                   <attribute name='bcrm_gpc_versionid' />
                   <attribute name='bcrm_gpc_telecom_value' />
                   <attribute name='bcrm_gpc_telecom_use' />
                   <attribute name='bcrm_gpc_telecom_system' />
                   <attribute name='bcrm_gpc_sequence_number' />
                   <attribute name='bcrm_gpc_address_use' />
                   <attribute name='bcrm_gpc_address_district' />
                   <attribute name='bcrm_footerfield' />
                   <attribute name='bcrm_financephone' />
                   <attribute name='bcrm_financeemail' />
                   <attribute name='bcrm_financecontact' />
                   <attribute name='bcrm_fax' />
                   <attribute name='bcrm_email' />
                   <attribute name='bcrm_datainfo' />
                   <attribute name='bcrm_currency' />
                   <attribute name='createdonbehalfby' />
                   <attribute name='createdby' />
                   <attribute name='bcrm_country' />
                   <attribute name='bcrm_clinic_uk_centre_code' />
                   <attribute name='bcrm_clinictype' />
                   <attribute name='bcrm_clinicinfo' />
                   <attribute name='bcrm_city_or_town' />
                   <attribute name='bcrm_cimar_vanity_name' />
                   <attribute name='bcrm_cimar_user_account_uuid' />
                   <attribute name='bcrm_cimar_passphrase' />
                   <attribute name='bcrm_cimar_iv_hex_key' />
                   <attribute name='bcrm_businessuser' />
                   <attribute name='bcrm_businessunituserid' />
                   <attribute name='bcrm_businessunitusername' />
                   <attribute name='bcrm_bic' />
                   <attribute name='bcrm_bank' />
                   <attribute name='bcrm_address' />
                   <attribute name='bcrm_accountname' />
                   <attribute name='bcrm_accountcode' />
                   <order attribute='bcrm_name' descending='false' />
                   <filter type='and'>
                       <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + id + @"' />
                   </filter>
                 </entity>
               </fetch>";


                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(organizationXML));

                OrganizationDTO organizationDetails = new OrganizationDTO();
                OrganisationDetails od = new OrganisationDetails();



                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        organizationDetails.resourceId = record.Id.ToString();
                        if (record.Attributes.Contains("modifiedon")) { organizationDetails.lastUpdated = (DateTime)record.Attributes["modifiedon"]; }
                        organizationDetails.sequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        organizationDetails.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;

                        organizationDetails.odsCode = record.Attributes.Contains("bcrm_odscode") ? record["bcrm_odscode"].ToString() : string.Empty;
                        organizationDetails.phoneNumber = record.Attributes.Contains("bcrm_gpc_telecom_value") ? record["bcrm_gpc_telecom_value"].ToString() : string.Empty;
                        organizationDetails.addressLine = record.Attributes.Contains("bcrm_house_number_street_name") ? record["bcrm_house_number_street_name"].ToString() : string.Empty;
                        organizationDetails.city = record.Attributes.Contains("bcrm_city_or_town") ? record["bcrm_city_or_town"].ToString() : string.Empty;
                        organizationDetails.district = record.Attributes.Contains("bcrm_gpc_address_district") ? record["bcrm_gpc_address_district"].ToString() : string.Empty;
                        organizationDetails.postalCode = record.Attributes.Contains("bcrm_postal_code") ? record["bcrm_postal_code"].ToString() : string.Empty;
                        organizationDetails.organizationName = record.Attributes.Contains("bcrm_name") ? record["bcrm_name"].ToString() : string.Empty;
                    }

                    if (source == "Internal")
                    {
                        return od.ReadOrganizationFHIRJSON(organizationDetails);
                    }


                    var json = od.ReadOrganizationFHIRJSON(organizationDetails);

                    finaljson[0] = json;
                    finaljson[1] = organizationDetails.versionId;
                    finaljson[2] = "Success";
                    return finaljson;
                }
                finaljson[0] = od.OrganizationNotFoundJSON(id);
                finaljson[1] = "";
                finaljson[2] = "NotFound";
                return finaljson;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public dynamic ReadALocation(string id, string sspInteractionId, string source)
        {
            try
            {
                dynamic[] finaljson = new dynamic[3];
                if (source == "External")
                {
                    if (sspInteractionId != "urn:nhs:names:services:gpconnect:fhir:rest:read:location-1")
                    {
                        PatientDetails pd = new PatientDetails();
                        var res = pd.WrongInteractionId(sspInteractionId, "Location");
                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "InvalidInteractionId";
                        return finaljson;

                    }
                }


                string locationXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                     <entity name='bcrm_gpclocation'>
                                       <attribute name='bcrm_gpclocationid' />
                                       <attribute name='bcrm_name' />
                                       <attribute name='createdon' />
                                       <attribute name='statuscode' />
                                       <attribute name='statecode' />
                                       <attribute name='overriddencreatedon' />
                                       <attribute name='owningbusinessunit' />
                                       <attribute name='ownerid' />
                                       <attribute name='modifiedon' />
                                       <attribute name='modifiedonbehalfby' />
                                       <attribute name='modifiedby' />
                                       <attribute name='bcrm_gpc_managingorganization' />
                                       <attribute name='bcrm_gpc_name' />
                                       <attribute name='bcrm_gpc_versionid' />
                                       <attribute name='bcrm_gpc_status' />
                                       <attribute name='bcrm_telecomuse' />
                                       <attribute name='bcrm_telecomsystem' />
                                       <attribute name='bcrm_telecomvalue' />
                                       <attribute name='bcrm_gpc_address_postalcode' />
                                       <attribute name='bcrm_gpc_address_country' />
                                       <attribute name='bcrm_gpcsequencenumber' />
                                       <attribute name='bcrm_gpc_address_district' />
                                       <attribute name='createdonbehalfby' />
                                       <attribute name='createdby' />
                                       <attribute name='bcrm_gpc_address_city' />
                                       <attribute name='bcrm_gpc_address_line' />
                                       <order attribute='bcrm_name' descending='false' />
                                  
                                       <filter type='and'>
                                         <condition attribute='bcrm_gpcsequencenumber' operator='eq' value='" + id + @"' />
                                       </filter>
                                       <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_managingorganization' visible='false' link-type='outer' alias='managingorganization'>
                                         <attribute name='bcrm_gpc_sequence_number' />
                                       </link-entity>
                                     </entity>
                                   </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(locationXML));

                LocationDTO locationDetails = new LocationDTO();



                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {

                        if (record.Attributes.Contains("modifiedon")) { locationDetails.lastupdated = (DateTime)record.Attributes["modifiedon"]; }
                        locationDetails.sequenceId = record.Attributes.Contains("bcrm_gpcsequencenumber") ? record["bcrm_gpcsequencenumber"].ToString() : string.Empty;
                        locationDetails.versionId = record.Attributes.Contains("bcrm_gpc_versionid") ? record["bcrm_gpc_versionid"].ToString() : string.Empty;
                        locationDetails.status = record.Attributes.Contains("bcrm_gpc_status") ? record.FormattedValues["bcrm_gpc_status"].ToString() : string.Empty;

                        locationDetails.telecomUse = record.Attributes.Contains("bcrm_telecomuse") ? record.FormattedValues["bcrm_telecomuse"].ToString() : string.Empty;
                        locationDetails.telecomSystem = record.Attributes.Contains("bcrm_telecomsystem") ? record.FormattedValues["bcrm_telecomsystem"].ToString() : string.Empty;
                        locationDetails.telecomValue = record.Attributes.Contains("bcrm_telecomvalue") ? record["bcrm_telecomvalue"].ToString() : string.Empty;

                        locationDetails.name = record.Attributes.Contains("bcrm_gpc_name") ? record["bcrm_gpc_name"].ToString() : string.Empty;
                        locationDetails.addressLine = record.Attributes.Contains("bcrm_gpc_address_line") ? record["bcrm_gpc_address_line"].ToString() : string.Empty;
                        locationDetails.city = record.Attributes.Contains("bcrm_gpc_address_city") ? record["bcrm_gpc_address_city"].ToString() : string.Empty;
                        locationDetails.district = record.Attributes.Contains("bcrm_gpc_address_district") ? record["bcrm_gpc_address_district"].ToString() : string.Empty;
                        locationDetails.postalcode = record.Attributes.Contains("bcrm_gpc_address_postalcode") ? record["bcrm_gpc_address_postalcode"].ToString() : string.Empty;
                        locationDetails.country = record.Attributes.Contains("bcrm_gpc_address_country") ? record["bcrm_gpc_address_country"].ToString() : string.Empty;
                        dynamic manOrg = record.Attributes.Contains("managingorganization.bcrm_gpc_sequence_number") ? record["managingorganization.bcrm_gpc_sequence_number"] : string.Empty;
                        locationDetails.managingOrganisationsequenceNumber = manOrg.ToString() != "" ? manOrg.Value : string.Empty;
                    }




                    LocationDetails ld = new LocationDetails();
                    var json = ld.ReadALocationFHIRJSON(locationDetails);
                    if (source == "Internal")
                    {
                        return ld.ReadALocationFHIRJSON(locationDetails);
                    }



                    finaljson[0] = json;
                    finaljson[1] = locationDetails.versionId;
                    finaljson[2] = "Success";

                    return finaljson;
                }
                finaljson[0] = "";
                finaljson[1] = "";
                finaljson[2] = "NotFound";

                return finaljson;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }



        }
        public dynamic ReadPractionerRoleJSON(string PractitionerId)
        {
            try
            {
                var res = intNHSPractitionerRoleDTO(PractitionerId);
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public dynamic CreatePatientRecord(RegisterPatientDTO patientDetails, string patientDetailsString, string token)
        {
            try
            {
                RegisterPatientDetails rps = new RegisterPatientDetails();

                dynamic[] finaljson = new dynamic[3];

                if (patientDetailsString != null)
                {
                    var CheckActive = patientDetailsString.Contains("\"active\":");
                    var CheckBirths = patientDetailsString.Contains("\"multipleBirthBoolean\":");
                    var CheckCareGeneralPractitioner = patientDetailsString.Contains("\"generalPractitioner\":");
                    var CheckContact = patientDetailsString.Contains("\"contact\":");
                    var CheckManagingOrg = patientDetailsString.Contains("\"managingOrganization\":");
                    var CheckMarrital = patientDetailsString.Contains("\"maritalStatus\":");
                    var CheckAnimal = patientDetailsString.Contains("\"animal\":");
                    var CheckCommunication = patientDetailsString.Contains("\"communication\":");
                    var CheckDeceasedDateTime = patientDetailsString.Contains("\"deceasedDateTime\":");
                    var CheckPhoto = patientDetailsString.Contains("\"photo\":");
                    var CheckinvalidField = patientDetailsString.Contains("\"invalidField\":");

                    if (CheckActive == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("active");
                    }
                    else if (CheckBirths == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("multipleBirthBoolean");
                    }
                    else if (CheckCareGeneralPractitioner == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("generalPractitioner");
                    }
                    else if (CheckContact == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("contact");
                    }
                    else if (CheckManagingOrg == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("managingOrganization");
                    }
                    else if (CheckMarrital == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("maritalStatus");
                    }
                    else if (CheckAnimal == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("animal");
                    }
                    else if (CheckCommunication == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("communication");
                    }
                    else if (CheckDeceasedDateTime == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("deceasedDateTime");
                    }
                    else if (CheckPhoto == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("photo");
                    }
                    else if (CheckinvalidField == true)
                    {
                        finaljson[0] = rps.UnneccassaryFields("invalidField");
                    }
                    else if (patientDetails.parameter[0].resource.resourceType.ToString().ToLower() != "patient")
                    {
                        finaljson[0] = rps.UnneccassaryFields("Invalid Patient Resource Type");
                    }
                    else if (patientDetails.resourceType.ToString().ToLower() != "parameters")
                    {
                        finaljson[0] = rps.UnneccassaryFields("Invalid Bundle Resource Type");
                    }

                    if (finaljson[0] != null)
                    {
                        finaljson[1] = "";
                        finaljson[2] = "PropertiesInvalid";
                        return finaljson;
                    }
                }
                if (token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token.Replace("Bearer ", ""));

                    // Extract requested_scope value
                    var requestedScope = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "requested_scope")?.Value;

                    if (requestedScope != "patient/*.write")
                    {
                        finaljson[0] = rps.JWTClaimIssue();
                        finaljson[1] = "";
                        finaljson[2] = "JWTClaimIssue";
                        return finaljson;
                    }
                }

                if (patientDetails.parameter[0].resource.identifier != null)
                {
                    foreach (var item in patientDetails.parameter[0].resource.identifier)
                    {
                        if (item.system == null || item.value == null)
                        {
                            finaljson[0] = rps.NotPassingValueOrSystem();
                            finaljson[1] = "";
                            finaljson[2] = "NotPassingValueOrSystem";
                            return finaljson;
                        }
                    }

                }

                if (patientDetails.parameter[0].resource.extension != null)
                {
                    if (patientDetails.parameter[0].resource.extension.Count > 1)
                    {
                        List<RegisterPatientDTOExtension> extensionList = new List<RegisterPatientDTOExtension>();
                        foreach (var item in patientDetails.parameter[0].resource.extension)
                        {
                            for (var i = 0; i < extensionList.Count; i++)
                            {
                                if (extensionList[i].url == item.url)
                                {
                                    finaljson[0] = rps.MultipleSameExtensionFound();
                                    finaljson[1] = "";
                                    finaljson[2] = "MultipleSameExtensionFound";
                                    return finaljson;
                                }
                            }

                            RegisterPatientDTOExtension ext = new RegisterPatientDTOExtension();
                            ext.url = item.url;
                            extensionList.Add(ext);
                        }

                    }
                }




                RegisterPatientResponse registerPatientResponse = new RegisterPatientResponse();
                var NHSNumber = "";
                var GivenName = "";
                var FamilyName = "";
                var Dob = "";
                var MiddleName = "";
                var languageName = "";
                var interpreteredRequired = false;
                var patientAddressUseIsTemp = "no";

                if (patientDetails.parameter[0].resource.extension != null)
                {
                    if (patientDetails.parameter[0].resource.extension[0].valueCodeableConcept != null)
                    {
                        languageName = patientDetails.parameter[0].resource.extension[0].valueCodeableConcept.coding[0].display;
                    }
                }

                if (patientDetails.parameter[0].resource.extension != null)
                {
                    for (int i = 0; i < patientDetails.parameter[0].resource.extension.Count; i++)
                    {
                        for (int j = 0; j < patientDetails.parameter[0].resource.extension[i].extension.Count; j++)
                        {
                            if (patientDetails.parameter[0].resource.extension[i].extension[j].url == "interpreterRequired")
                            {
                                if (patientDetails.parameter[0].resource.extension[i].extension[j].valueBoolean == true)
                                {

                                    interpreteredRequired = true;
                                }

                            }
                        }

                    }

                }

                if (patientDetails.parameter[0].resource.birthDate == null)
                {
                    finaljson[0] = rps.NoDobSupplied();
                    finaljson[1] = "";
                    finaljson[2] = "NoDobSupplied";
                    return finaljson;
                }

                if (patientDetails.parameter.Count > 1)
                {
                    finaljson[0] = rps.MoreThanOneResources();
                    finaljson[1] = "";
                    finaljson[2] = "MoreThanOneResourcesFound";
                    return finaljson;
                }


                if (patientDetails.parameter[0].resource.identifier != null)
                {
                    NHSNumber = patientDetails.parameter[0].resource.identifier[0].value;
                }
                if (patientDetails.parameter[0].resource.birthDate != null)
                {
                    Dob = patientDetails.parameter[0].resource.birthDate.ToString();
                }


                if (patientDetails.parameter[0].resource.name != null)
                {
                    GivenName = patientDetails.parameter[0].resource.name[0].given[0];
                    FamilyName = patientDetails.parameter[0].resource.name[0].family;

                    if (patientDetails.parameter[0].resource.name[0].use != "official")
                    {
                        finaljson[0] = rps.NoOfficialSupplied();
                        finaljson[1] = "";
                        finaljson[2] = "NoOfficialSupplied";
                        return finaljson;
                    }
                    if (patientDetails.parameter[0].resource.name[0].given.Count > 1)
                    {
                        for (var i = 1; i < patientDetails.parameter[0].resource.name[0].given.Count; i++)
                        {
                            if (MiddleName == "")
                            {
                                MiddleName += patientDetails.parameter[0].resource.name[0].given[i];
                            }
                            else
                            {
                                MiddleName += "," + patientDetails.parameter[0].resource.name[0].given[i];
                            }

                        }
                    }
                }


                if (NHSNumber != null)
                {
                    var st = Regex.IsMatch(NHSNumber, @"^\d{10}$");
                    if (st == false)
                    {
                        finaljson[0] = rps.RegisterPatientInvalidNHSNumber(NHSNumber);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidNHSNumber";
                        return finaljson;
                    }
                }



                var patientDeceasedStatus = checkPatientIsDeceasedOrNotInOurRecord(NHSNumber);
                if (patientDeceasedStatus == true)
                {
                    finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                    finaljson[1] = "";
                    finaljson[2] = "InvalidNHSNumber";
                    return finaljson;

                }

                var PatientGpRegistractionStatusstatus = checkPatientLocalGPorNot(NHSNumber);
                if (PatientGpRegistractionStatusstatus == "active")
                {
                    finaljson[0] = rps.PatientIsAlreadyExistInOurGP(NHSNumber);
                    finaljson[1] = "";
                    finaljson[2] = "DuplicateRejected";
                    return finaljson;

                }
                PDSAPI pa = new PDSAPI();

                var patientSearchPDS = pa.GetPatientByNHSNumber(NHSNumber);

                if (patientSearchPDS.nameGiven == "" || patientSearchPDS.nameFamily == "")
                {

                    finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                    finaljson[1] = "";
                    finaljson[2] = "InvalidNHSNumber";
                    return finaljson;
                }

                if (patientSearchPDS != null)
                {
                    string? pdsDOB = string.Empty;
                    string? pdsGivenName = string.Empty;
                    string? pdsFamilyName = string.Empty;
                    string? pdsNHSNumber = string.Empty;

                    if (patientSearchPDS.birthDate != null) pdsDOB = patientSearchPDS.birthDate;
                    if (patientSearchPDS?.nameGiven != null) pdsGivenName = patientSearchPDS?.nameGiven;
                    //pdsGivenName = patientSearchPDS?.name?[0] != null ? patientSearchPDS?.name?[0].given?[0] != null ? patientSearchPDS?.name?[0].given?[0] : string.Empty : string.Empty;
                    if (patientSearchPDS?.nameFamily != null) pdsFamilyName = patientSearchPDS?.nameFamily;
                    if (patientSearchPDS?.nhsNumber != null) pdsNHSNumber = patientSearchPDS?.nhsNumber;

                    if (pdsNHSNumber != NHSNumber)
                    {
                        registerPatientResponse.message = "NHS Number is Superseded";
                        registerPatientResponse.errorOccurStatus = true;

                        finaljson[0] = rps.SupersededNHSNumber(NHSNumber);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidDemographic";
                        return finaljson;

                    }
                    if (pdsDOB != Dob)
                    {

                        DateTime parsePDSDOB = DateTime.ParseExact(pdsDOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        DateTime parseDOB = DateTime.ParseExact(Dob, "yyyy-MM-dd", CultureInfo.InvariantCulture);


                        bool checkDate = parsePDSDOB.Day == parseDOB.Day ? true : false;
                        bool checkMonth = parsePDSDOB.Month == parseDOB.Month ? true : false;
                        bool checkYear = parsePDSDOB.Year == parseDOB.Year ? true : false;

                        if ((checkDate && checkMonth) || (checkMonth && checkYear) || (checkYear && checkDate))
                        {
                            registerPatientResponse.message = "At least two conditions are true";
                            registerPatientResponse.errorOccurStatus = true;

                        }
                        else
                        {
                            registerPatientResponse.message = "Dob Mismatched";
                            registerPatientResponse.errorOccurStatus = true;
                            finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                            finaljson[1] = "";
                            finaljson[2] = "InvalidDemographic";
                            return finaljson;
                        }
                    }

                    if (FamilyName != "")
                    {
                        if (pdsFamilyName.ToString().ToLower()?.Substring(0, 3) != FamilyName.ToString().ToLower()?.Substring(0, 3))
                        {
                            registerPatientResponse.message = "Family Name Mismatched";
                            registerPatientResponse.errorOccurStatus = true;
                            finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                            finaljson[1] = "";
                            finaljson[2] = "InvalidDemographic";
                            return finaljson;
                        }
                    }
                    else
                    {
                        finaljson[0] = rps.NoFamilyNameInJSON();
                        finaljson[1] = "";
                        finaljson[2] = "NoFamilyName";
                        return finaljson;
                    }



                    if (GivenName.ToString().ToLower()?.Substring(0, 1) != pdsGivenName.ToString().ToLower()?.Substring(0, 1))
                    {
                        registerPatientResponse.message = "Given Name Mismatched";
                        registerPatientResponse.errorOccurStatus = true;
                        finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidDemographic";
                        return finaljson;
                    }

                    if (patientSearchPDS?.deceasedDateAndTime.ToString() != "")
                    {
                        registerPatientResponse.message = "Patient is deceased";
                        registerPatientResponse.errorOccurStatus = true;
                        finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidDemographic";
                        return finaljson;
                    }

                    if (patientSearchPDS?.confidentialityCode.ToString() != "U")
                    {
                        registerPatientResponse.message = "PDS Flag is Invalid";
                        registerPatientResponse.errorOccurStatus = true;
                        finaljson[0] = rps.InvalidDemographicNHS(NHSNumber);
                        finaljson[1] = "";
                        finaljson[2] = "InvalidDemographic";
                        return finaljson;
                    }

                    if (patientDetails.parameter[0].resource.telecom != null)
                    {
                        if (patientDetails.parameter[0].resource.telecom.Count > 0)
                        {


                            List<RegistractionTelecomCheckerDTO> RTCDList = new List<RegistractionTelecomCheckerDTO>();
                            foreach (var item in patientDetails.parameter[0].resource.telecom)
                            {
                                for (var i = 0; i < RTCDList.Count; i++)
                                {
                                    if (RTCDList[i].system == item.system && RTCDList[i].use == item.use)
                                    {

                                        finaljson[0] = rps.DuplicateTelecomField(RTCDList[i].system);
                                        finaljson[1] = "";
                                        finaljson[2] = "DuplicateField";
                                        return finaljson;
                                    }
                                }
                                RegistractionTelecomCheckerDTO rtcd = new RegistractionTelecomCheckerDTO();
                                rtcd.system = item.system;
                                rtcd.use = item.use;
                                RTCDList.Add(rtcd);
                            }


                            for (var i = 0; i < patientDetails.parameter[0].resource.telecom.Count; i++)
                            {

                                if (patientDetails.parameter[0].resource.telecom[i].system == "phone" && patientDetails.parameter[0].resource.telecom[i].use == "mobile")
                                {
                                    patientSearchPDS.phone = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "email" && patientDetails.parameter[0].resource.telecom[i].use == "work")
                                {
                                    patientSearchPDS.workEmail = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "email" && patientDetails.parameter[0].resource.telecom[i].use == "home")
                                {
                                    patientSearchPDS.homeEmail = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "phone" && patientDetails.parameter[0].resource.telecom[i].use == "work")
                                {
                                    patientSearchPDS.workPhone = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "phone" && patientDetails.parameter[0].resource.telecom[i].use == "home")
                                {
                                    patientSearchPDS.homePhone = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "phone" && patientDetails.parameter[0].resource.telecom[i].use == "temp")
                                {
                                    patientSearchPDS.tempPhone = patientDetails.parameter[0].resource.telecom[i].value;
                                }
                                if (patientDetails.parameter[0].resource.telecom[i].system == "email" && patientDetails.parameter[0].resource.telecom[i].use == "temp")
                                {
                                    patientSearchPDS.tempEmail = patientDetails.parameter[0].resource.telecom[i].value;
                                }

                            }
                        }
                    }

                    var PDSaddressDictionary = new Dictionary<string, string>
                    {
                        { "address1_postalcode", patientSearchPDS.postalCode },
                        { "address1_stateorprovince", patientSearchPDS.address1_stateorprovince  },
                        { "address1_city", patientSearchPDS.address1_city  },
                        { "address1_line1", patientSearchPDS.addressLines },
                        { "use", patientSearchPDS.addressUse}
                    };


                    if (MiddleName != "")
                    {
                        patientSearchPDS.middleName = MiddleName;
                    }
                    patientSearchPDS.GPCRegistractionJSON = new JavaScriptSerializer().Serialize(patientDetails);

                    if (patientDetails.parameter[0].resource.address != null)
                    {
                        if (patientDetails.parameter[0].resource.address.Count > 0)
                        {
                            List<string> addressUseList = new List<string>();

                            foreach (var item in patientDetails.parameter[0].resource.address)
                            {
                                patientSearchPDS.AddressType = patientDetails.parameter[0].resource.address[0].use;
                                if (item.use.ToString().ToLower() == "temp")
                                {
                                    patientAddressUseIsTemp = "yes";
                                }
                                if (item.use.ToString().ToLower() == "old")
                                {
                                    finaljson[0] = rps.OldAddressUseValue();
                                    finaljson[1] = "";
                                    finaljson[2] = "OldAddress";
                                    return finaljson;
                                }
                                if (item.use.ToString().ToLower() == "work")
                                {
                                    finaljson[0] = rps.WorkAddressUseValue();
                                    finaljson[1] = "";
                                    finaljson[2] = "WorkAddress";
                                    return finaljson;
                                }

                                for (var j = 0; j < addressUseList.Count; j++)
                                {
                                    if (addressUseList[j] == item.use)
                                    {
                                        finaljson[0] = rps.DuplicateAddressUseValue();
                                        finaljson[1] = "";
                                        finaljson[2] = "DuplicateAddressUse";
                                        return finaljson;
                                    }
                                }
                                addressUseList.Add(item.use);
                            }

                            patientSearchPDS.address1_line1 = patientDetails.parameter[0].resource.address[0].line[0];
                            patientSearchPDS.address1_city = patientDetails.parameter[0].resource.address[0].city;
                            patientSearchPDS.address1_stateorprovince = patientDetails.parameter[0].resource.address[0].district;
                            patientSearchPDS.address1_postalcode = patientDetails.parameter[0].resource.address[0].postalCode;
                        }
                    }
                    if (interpreteredRequired == true)
                    {
                        patientSearchPDS.interpreterRequired = true;
                    }

                    if (PatientGpRegistractionStatusstatus == "inactive")
                    {
                        patientSearchPDS.PatientExistStatus = "yes";
                    }


                    if (!registerPatientResponse.errorOccurStatus)
                    {
                        var createPatientRespose = CreatePatientinCRM(patientSearchPDS!, languageName);
                        registerPatientResponse.id = createPatientRespose.id;

                        var identifier = "https://fhir.nhs.uk/Id/nhs-number|" + NHSNumber;
                        var res = RegisterAPatientCreateResponse(NHSNumber, "T", patientSearchPDS.PatientExistStatus, patientAddressUseIsTemp, "", PDSaddressDictionary);

                        finaljson[0] = res;
                        finaljson[1] = "";
                        finaljson[2] = "Success";
                        return finaljson;

                    }
                    //var patientPDSJSON = patientSearchPDS?.ToString();
                }



                return "";
            }
            catch (Exception ex)
            {
                RegisterPatientDetails rps = new RegisterPatientDetails();

                dynamic[] finaljson = new dynamic[3];
                finaljson[0] = rps.InvalidDemographicNHS("");
                finaljson[1] = "";
                finaljson[2] = "InvalidDemographic";
                return finaljson;

            }
        }
        public CreatePatientResponse CreatePatientinCRM(PatientSearchPDSV1 patientDetails, string languageName)
        {

            CreatePatientResponse patientResponse = new CreatePatientResponse();
            try
            {

                Entity createRecords = new Entity("contact");

                if (!string.IsNullOrEmpty(patientDetails.nominatedPharmacyCode))
                {
                    createRecords["bcrm_pdsnominatedpharmacycode"] = patientDetails.nominatedPharmacyCode;
                }
                if (!string.IsNullOrEmpty(patientDetails.phone))
                {
                    createRecords["mobilephone"] = patientDetails.phone;
                }

                if (!string.IsNullOrEmpty(patientDetails.generalPractitionerIdentifier))
                {
                    createRecords["bcrm_gpidentifier"] = patientDetails.generalPractitionerIdentifier;
                }
                if (!string.IsNullOrEmpty(patientDetails.generalPractitionerPeriodStart))
                {
                    createRecords["bcrm_gpperiodstart"] = Convert.ToDateTime(patientDetails.generalPractitionerPeriodStart);
                }
                if (!string.IsNullOrEmpty(patientDetails.generalPractitionerPeriodEnd))
                {
                    createRecords["bcrm_gpperiodend"] = Convert.ToDateTime(patientDetails.generalPractitionerPeriodEnd);
                }
                if (!string.IsNullOrEmpty(patientDetails.generalPractitionerType))
                {
                    createRecords["bcrm_pdsgptype"] = patientDetails.generalPractitionerType;
                }
                if (!string.IsNullOrEmpty(patientDetails.generalPractitionerValue))
                {
                    createRecords["bcrm_gpvalue"] = patientDetails.generalPractitionerValue;
                }
                if (!string.IsNullOrEmpty(patientDetails.deathNotificationEffectiveDate))
                {
                    createRecords["bcrm_deceaseddate"] = Convert.ToDateTime(patientDetails.deathNotificationEffectiveDate);
                }
                if (!string.IsNullOrEmpty(patientDetails.deathNotificationStatus))
                {
                    createRecords["bcrm_deathnotificationstatus"] = patientDetails.deathNotificationStatus;
                }

                if (!string.IsNullOrEmpty(patientDetails.workEmail))
                {
                    createRecords["emailaddress3"] = patientDetails.workEmail;
                }
                if (!string.IsNullOrEmpty(patientDetails.workPhone))
                {
                    createRecords["address2_telephone1"] = patientDetails.workPhone;
                }
                if (!string.IsNullOrEmpty(patientDetails.homePhone))
                {
                    createRecords["address1_telephone1"] = patientDetails.homePhone;
                }
                if (!string.IsNullOrEmpty(patientDetails.homeEmail))
                {
                    createRecords["emailaddress2"] = patientDetails.homeEmail;
                }

                if (!string.IsNullOrEmpty(patientDetails.versionId))
                {
                    createRecords["bcrm_pdsversionid"] = patientDetails.versionId;
                }
                if (!string.IsNullOrEmpty(patientDetails.nameGiven))
                {
                    createRecords["firstname"] = patientDetails.nameGiven;
                }
                if (!string.IsNullOrEmpty(patientDetails.pdsJson))
                {
                    createRecords["bcrm_pdsjson"] = patientDetails.pdsJson;
                }

                if (!string.IsNullOrEmpty(patientDetails.nameFamily))
                {
                    createRecords["lastname"] = patientDetails.nameFamily;
                }

                if (!string.IsNullOrEmpty(patientDetails.gender))
                {
                    createRecords["gendercode"] = new OptionSetValue(GetGenderCode(patientDetails.gender));
                    // Set Contact Type Patient 
                    createRecords["msemr_contacttype"] = new OptionSetValue(935000000);
                }
                if (!string.IsNullOrEmpty(patientDetails.birthDate))
                {
                    createRecords["birthdate"] = Convert.ToDateTime(patientDetails.birthDate);
                }
                if (patientDetails.nhsNumber != null && patientDetails.nhsNumber != "")
                {
                    createRecords["bcrm_nhsnumber"] = patientDetails.nhsNumber;
                }
                if (patientDetails.homeAddressPostalCode != null && patientDetails.homeAddressPostalCode != "")
                {
                    createRecords["address3_postalcode"] = patientDetails.homeAddressPostalCode;
                }
                if (patientDetails.homeAddressLines != null && patientDetails.homeAddressLines != "")
                {
                    createRecords["address3_line1"] = patientDetails.homeAddressLines;
                }
                if (!string.IsNullOrEmpty(patientDetails.middleName))
                {
                    createRecords["bcrm_middlename"] = patientDetails.middleName;
                }

                if (!string.IsNullOrEmpty(patientDetails.address1_line1))
                {
                    createRecords["address1_line1"] = patientDetails.address1_line1;
                }
                if (!string.IsNullOrEmpty(patientDetails.address1_city))
                {
                    createRecords["address1_city"] = patientDetails.address1_city;
                }
                if (!string.IsNullOrEmpty(patientDetails.address1_stateorprovince))
                {
                    createRecords["address1_stateorprovince"] = patientDetails.address1_stateorprovince;
                }
                if (!string.IsNullOrEmpty(patientDetails.address1_postalcode))
                {
                    createRecords["address1_postalcode"] = patientDetails.address1_postalcode;
                }
                // check
                if (!string.IsNullOrEmpty(patientDetails.tempPhone))
                {
                    createRecords["bcrm_tempphone"] = patientDetails.tempPhone;
                }

                if (!string.IsNullOrEmpty(patientDetails.tempEmail))
                {
                    createRecords["bcrm_tempemail"] = patientDetails.tempEmail;
                }

                if (!string.IsNullOrEmpty(patientDetails.GPCRegistractionJSON))
                {
                    createRecords["bcrm_gpc_registraction_json"] = patientDetails.GPCRegistractionJSON;
                }

                if (!string.IsNullOrEmpty(patientDetails.AddressType))
                {
                    if (patientDetails.AddressType.ToString().ToLower() == "temp")
                    {
                        createRecords["bcrm_addresstype"] = new OptionSetValue(1);
                    }
                    if (patientDetails.AddressType.ToString().ToLower() == "home")
                    {
                        createRecords["bcrm_addresstype"] = new OptionSetValue(2);
                    }
                    if (patientDetails.AddressType.ToString().ToLower() == "work")
                    {
                        createRecords["bcrm_addresstype"] = new OptionSetValue(3);
                    }
                }

                if (patientDetails.interpreterRequired == true)
                {
                    createRecords["bcrm_interpreterrequired"] = true;
                }


                // check 1
                createRecords["bcrm_nhsnumberverificationstatus"] = "01";
                createRecords["bcrm_nhsnumberverificationstatusdisplay"] = "Number present and verified";
                createRecords["bcrm_gpc_preferredbarnchsurgery"] = new EntityReference("bcrm_gpclocation", new Guid("fe550c85-cbed-ee11-a1fd-002248c6247b"));
                createRecords["bcrm_gpc_generalpractioner"] = new EntityReference("bcrm_staff", new Guid("c1486f15-fbbf-ee11-9079-0022481ab71c"));
                createRecords["bcrm_gpc_manangingorganisation"] = new EntityReference("bcrm_clinic", new Guid("91b2ee6a-cbed-ee11-a1fd-002248c6247b"));

                createRecords["bcrm_gms1type"] = new OptionSetValue(271400000);
                createRecords["bcrm_modeofcommunication"] = new OptionSetValue(271400002);
                createRecords["bcrm_gpc_communicationproficiency"] = new OptionSetValue(271400002);

                createRecords["bcrm_gpc_registractionperiod"] = DateTime.Now;
                createRecords["bcrm_temporaryrecordexpiry"] = DateTime.Now.AddMonths(3);

                if (languageName != "")
                {
                    if (languageName == "English")
                    {
                        OptionSetValueCollection languages = new OptionSetValueCollection
                        {
                            new OptionSetValue(271400000)
                        };
                        createRecords["bcrm_languages"] = languages;
                    }
                    else if (languageName == "Persian")
                    {
                        OptionSetValueCollection languages = new OptionSetValueCollection
                        {
                            new OptionSetValue(271400001)
                        };
                        createRecords["bcrm_languages"] = languages;
                    }
                    else if (languageName == "Abkhazian")
                    {
                        OptionSetValueCollection languages = new OptionSetValueCollection
                        {
                            new OptionSetValue(271400002)
                        };
                        createRecords["bcrm_languages"] = languages;
                    }
                    else if (languageName == "Braille")
                    {
                        OptionSetValueCollection languages = new OptionSetValueCollection
                        {
                            new OptionSetValue(271400003)
                        };
                        createRecords["bcrm_languages"] = languages;
                    }
                    else
                    {
                        OptionSetValueCollection languages = new OptionSetValueCollection
                        {
                            new OptionSetValue(271400000)
                        };
                        createRecords["bcrm_languages"] = languages;
                    }
                }
                else
                {
                    OptionSetValueCollection languages = new OptionSetValueCollection
                {
                    new OptionSetValue(271400000)
                };
                    createRecords["bcrm_languages"] = languages;
                }



                #region setting title for pds
                if (!string.IsNullOrEmpty(patientDetails.namePrefix))
                {
                    int title = GetTitleCode(patientDetails.namePrefix);
                    if (title != 0)
                    {
                        createRecords["bcrm_title"] = new OptionSetValue(title);
                    }
                }
                #endregion

                Guid contactId = _crmServiceClient.Create(createRecords);
                patientResponse.id = contactId;
                patientResponse.status = true;
                patientResponse.responseMessage = "Patient Created Successfully";

                return patientResponse;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        int GetGenderCode(string name)
        {
            string lowerCaseName = name.ToLower(); // Convert to lowercase for case-insensitivity

            switch (lowerCaseName)
            {
                case "male":
                    return 1;
                case "female":
                    return 2;
                case "other":
                    return 3;
                case "non-binary":
                    return 4;
                case "intersex":
                    return 7;
                case "transgender":
                    return 5;
                case "Unknown":
                    return 6;
                case "not stated":
                    return 8;
                default:
                    // Default code if the name doesn't match any of the specified titles
                    return 0;
            }
        }
        int GetTitleCode(string name)
        {
            string lowerCaseName = name.ToLower(); // Convert to lowercase for case-insensitivity

            switch (lowerCaseName)
            {
                case "sir":
                    return 1;
                case "dr":
                    return 271400001;
                case "master":
                    return 7;
                case "lady":
                    return 8;
                case "lord":
                    return 9;
                case "councillor":
                    return 10;
                case "professor":
                    return 11;
                case "reverend":
                    return 12;
                case "captain":
                    return 13;
                case "brother":
                    return 14;
                case "tt col":
                    return 15;
                case "major":
                    return 16;
                case "mx":
                    return 17;
                case "prof":
                    return 18;
                case "sister":
                    return 19;
                case "miss":
                    return 2;
                case "mr":
                    return 3;
                case "ms":
                    return 4;
                case "mrs":
                    return 5;
                default:
                    // Default code if the name doesn't match any of the specified titles
                    return 0;
            }
        }

        #endregion

        #region Internal-Method
        internal PatientDTO GetRelatedPerson(string id, PatientDTO patientDTO)
        {
            try
            {
                string relatedPersonXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                     <entity name='msemr_relatedperson'>
                                       <attribute name='msemr_relatedpersonid' />
                                       <attribute name='msemr_name' />
                                       <attribute name='createdon' />
                                       <attribute name='statuscode' />
                                       <attribute name='statecode' />
                                       <attribute name='msemr_relationship' />
                                       <attribute name='msemr_relatedpersonperiodstartdate' />
                                       <attribute name='msemr_relatedpersonperiodenddate' />
                                       <attribute name='bcrm_relatedpersonadditionaldetails' />
                                       <attribute name='msemr_relatedperson' />
                                       <attribute name='overriddencreatedon' />
                                       <attribute name='msemr_patient' />
                                       <attribute name='owningbusinessunit' />
                                       <attribute name='ownerid' />
                                       <attribute name='modifiedon' />
                                       <attribute name='modifiedonbehalfby' />
                                       <attribute name='modifiedby' />
                                       <attribute name='createdonbehalfby' />
                                       <attribute name='createdby' />
                                       <attribute name='bcrm_assignproxystatus' />
                                       <attribute name='msemr_active' />
                                       <order attribute='msemr_name' descending='false' />
                                       <filter type='and'>
                                         <condition attribute='msemr_patient' operator='eq' uitype='contact' value='{" + id + @"}' />
                                       </filter>
                                       <link-entity name='contact' from='contactid' to='msemr_relatedperson' visible='false' link-type='outer' alias='relatedPerson'>
                                         <attribute name='company' />
                                         <attribute name='bcbi_companyid' />
                                         <attribute name='address3_telephone3' />
                                         <attribute name='address3_telephone2' />
                                         <attribute name='address3_telephone1' />
                                         <attribute name='address3_fax' />
                                         <attribute name='address3_county' />
                                         <attribute name='address3_city' />
                                         <attribute name='bcrm_accepteddatasharing' />
                                         <attribute name='lastname' />
                                         <attribute name='firstname' />
                                         <attribute name='gendercode' />
                                         <attribute name='fullname' />
                                         <attribute name='bcrm_title' />
                                         <attribute name='mobilephone' />
                                         <attribute name='address1_line1' />
                                         <attribute name='address1_postalcode' />
                                         <attribute name='bcrm_joiningfee_base' />
                                       </link-entity>
                                     </entity>
                                    </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(relatedPersonXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {

                        dynamic fullname = record.Attributes.Contains("relatedPerson.fullname") ? record["relatedPerson.fullname"] : string.Empty;
                        patientDTO.RelatedPersonFullName = fullname.ToString() != "" ? fullname.Value : string.Empty;

                        dynamic givenname = record.Attributes.Contains("relatedPerson.firstname") ? record["relatedPerson.firstname"] : string.Empty;
                        patientDTO.RelatedPersonGivenName = givenname.ToString() != "" ? givenname.Value : string.Empty;

                        dynamic familyname = record.Attributes.Contains("relatedPerson.lastname") ? record["relatedPerson.lastname"] : string.Empty;
                        patientDTO.RelatedPersonFamilyName = familyname.ToString() != "" ? familyname.Value : string.Empty;


                        patientDTO.RelatedPersonPrefix = record.Attributes.Contains("relatedPerson.bcrm_title") ? record.FormattedValues["relatedPerson.bcrm_title"].ToString() : string.Empty;


                        dynamic mobilePhone = record.Attributes.Contains("relatedPerson.mobilephone") ? record["relatedPerson.mobilephone"] : string.Empty;
                        patientDTO.RelatedPersonMobilePhone = mobilePhone.ToString() != "" ? mobilePhone.Value : string.Empty;

                        dynamic addressLine = record.Attributes.Contains("relatedPerson.address1_line1") ? record["relatedPerson.address1_line1"] : string.Empty;
                        patientDTO.RelatedPersonAddressLine = addressLine.ToString() != "" ? addressLine.Value : string.Empty;

                        dynamic postalCode = record.Attributes.Contains("relatedPerson.address1_postalcode") ? record["relatedPerson.address1_postalcode"] : string.Empty;
                        patientDTO.RelatedPersonPostalCode = postalCode.ToString() != "" ? postalCode.Value : string.Empty;


                        patientDTO.RelatedPersonGender = record.Attributes.Contains("relatedPerson.gendercode") ? record.FormattedValues["relatedPerson.gendercode"].ToString().ToLower() : string.Empty;

                        dynamic relPerTxt = record.Attributes.Contains("msemr_relationship") ? record["msemr_relationship"] : string.Empty;
                        patientDTO.RelatedPersonRelationship = relPerTxt.ToString() != "" ? relPerTxt.Name : string.Empty;
                    }
                }

                return patientDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        internal string checkPatientLocalGPorNot(string NHS_number)
        {
            try
            {
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='contact'>
                                 <attribute name='fullname' />
                                 <attribute name='telephone1' />
                                 <attribute name='contactid' />
                                 <attribute name='statuscode' />
                                 <order attribute='fullname' descending='false' />
                                 <filter type='and'>
                                   
                                   <condition attribute='bcrm_nhsnumber' operator='like' value='" + NHS_number + @"' />
                                 </filter>
                               </entity>
                             </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                    var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                    if (statusValue == 2)
                    {
                        return "inactive";
                    }
                    else
                    {
                        return "active";
                    }

                }
                else
                {
                    return "notExist";
                }

            }
            catch (Exception ex)
            {
                return "notExist";
            }
        }
        internal bool checkPatientIsDeceasedOrNotInOurRecord(string NHS_number)
        {
            try
            {
                var finalStatus = false;
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                               <entity name='contact'>
                                 <attribute name='fullname' />
                                 <attribute name='telephone1' />
                                 <attribute name='contactid' />
                                 <attribute name='statuscode' />
                                <attribute name='bcrm_sensitivename' />
                                 <attribute name='bcrm_deceaseddate' />
                                 <order attribute='createdon' descending='true' />
                                 <filter type='and'>
                                   
                                   <condition attribute='bcrm_nhsnumber' operator='like' value='" + NHS_number + @"' />
                                 </filter>
                               </entity>
                             </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));
                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {

                    var record = AnswerCollection.Entities[0];
                    if (record.Attributes.Contains("bcrm_deceaseddate"))
                    {
                        var deceasedDate = (DateTime)record.Attributes["bcrm_deceaseddate"];
                        if (deceasedDate.ToString() == "01-01-0001 00:00:00")
                        {
                            finalStatus = false;
                        }
                        else
                        {
                            finalStatus = true;
                        }
                    }
                    if (record.Attributes.Contains("bcrm_sensitivename") && finalStatus == false)
                    {
                        var IsSensitive = record.Attributes["bcrm_sensitivename"].ToString();
                        if (IsSensitive == "True")
                        {
                            finalStatus = true;
                        }
                        else
                        {
                            finalStatus = false;
                        }
                    }
                  
                }

                return finalStatus;
            }
            catch (Exception ex)
            {
                return false;
            }
        }





        internal dynamic intNHSPractitionerRoleDTO(string practionerSequenceId)
        {
            try
            {
                var organizationId = "";
                var roleCode = "";
                var roleDisplay = "";
                var id = "";

                var practitionerRoleXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='bcrm_staff'>
                                          <attribute name='bcrm_staffid' />
                                          <attribute name='bcrm_name' />
                                          <attribute name='createdon' />
                                          <order attribute='bcrm_name' descending='false' />
                                          <filter type='and'>
                                            <condition attribute='bcrm_gpc_sequence_number' operator='eq' value='" + practionerSequenceId + @"' />
                                          </filter>
                                          <link-entity name='bcrm_oxwrsecurityroles' from='bcrm_oxwrsecurityrolesid' to='bcrm_roles' visible='false' link-type='outer' alias='Roles'>
                                            <attribute name='bcrm_oxwrsecurityrolesid' />
                                            <attribute name='bcrm_name' />
                                            <attribute name='bcrm_jobrolecode' />
                                            <attribute name='createdon' />
                                          </link-entity>
                                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_clinic' visible='false' link-type='outer' alias='Organization'>
                                            <attribute name='bcrm_gpc_sequence_number' />
                                          </link-entity>
                                        </entity>
                                      </fetch>";
                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(practitionerRoleXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];

                    dynamic orgLookUp = record.Attributes.Contains("Organization.bcrm_gpc_sequence_number") ? record["Organization.bcrm_gpc_sequence_number"] : null;
                    organizationId = orgLookUp.Value;

                    dynamic roleCodeLookup = record.Attributes.Contains("Roles.bcrm_jobrolecode") ? record["Roles.bcrm_jobrolecode"] : null;
                    roleCode = roleCodeLookup.Value;

                    dynamic roleDisplayLookup = record.Attributes.Contains("Roles.bcrm_name") ? record["Roles.bcrm_name"] : null;
                    roleDisplay = roleDisplayLookup.Value;

                    id = record.Id.ToString();

                }


                var practitionerRole = new Dictionary<string, object>
                                       {
                                           { "meta", new Dictionary<string, object>
                                               {
                                                   { "profile", new List<string> { "https://fhir.nhs.uk/STU3/StructureDefinition/CareConnect-GPC-PractitionerRole-1" } }
                                               }
                                           },
                                           { "resourceType", "PractitionerRole" },
                                           { "id", id },
                                           { "practitioner", new Dictionary<string, object>
                                               {
                                                   { "reference", "Practitioner/" + practionerSequenceId }
                                               }
                                           },
                                           { "organization", new Dictionary<string, object>
                                               {
                                                   { "reference", "Organization/"+organizationId }
                                               }
                                           },
                                           { "code", new List<Dictionary<string, object>>
                                               {
                                                   new Dictionary<string, object>
                                                   {
                                                       { "coding", new List<Dictionary<string, object>>
                                                           {
                                                               new Dictionary<string, object>
                                                               {
                                                                   { "system", "https://fhir.hl7.org.uk/STU3/CodeSystem/CareConnect-SDSJobRoleName-1" },
                                                                   { "code", roleCode },
                                                                   { "display", roleDisplay }
                                                               }
                                                           }
                                                       }
                                                   }
                                               }
                                           }
                                       };


                return practitionerRole;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal List<string> getPractitionerRoleCodes(string sdsUserId)
        {
            try
            {
                var Rolelist = new List<string>();


                string PractitionerJobRoleXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                          <entity name='bcrm_oxwrsecurityroles'>
                                            <attribute name='bcrm_name' />
                                            <attribute name='createdon' />
                                            <attribute name='bcrm_jobrolecode' />
                                            <attribute name='bcrm_activitycode' />
                                            <attribute name='bcrm_oxwrsecurityrolesid' />
                                            <order attribute='bcrm_name' descending='false' />
                                            <link-entity name='bcrm_staff_nhs_job_role' from='bcrm_oxwrsecurityrolesid' to='bcrm_oxwrsecurityrolesid' visible='false' intersect='true'>
                                              <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_staffid' alias='ac'>
                                                <filter type='and'>
                                                  <condition attribute='bcrm_gpc_sdsid' operator='eq' value='" + sdsUserId + @"' />
                                                </filter>
                                              </link-entity>
                                            </link-entity>
                                          </entity>
                                        </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(PractitionerJobRoleXML));

                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    foreach (var record in AnswerCollection.Entities)
                    {
                        var roleId = record.Attributes.Contains("bcrm_jobrolecode") ? record["bcrm_jobrolecode"].ToString() : "";
                        if (roleId != "")
                        {
                            Rolelist.Add(roleId);
                        }
                    }
                }
                return Rolelist;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }

        }
        internal dynamic RegisterAPatientCreateResponse(string NHSNumber, string RegType, string existStatus, string addressTypeIsTemp, string telecomTypeIsTemp, Dictionary<string, string> pdsAddress)
        {
            try
            {
                //**************************Get Profile Data On behalf Of NHS Number ****************************************//
                string contactXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                          <attribute name='lastname' />
                        <attribute name='telephone1' />
                        <attribute name='address1_telephone1' />
                        <attribute name='contactid' />
                        <attribute name='bcrm_pdsversionid' />
                        <attribute name='mobilephone' />
                        <attribute name='lastname' />
                        <attribute name='gendercode' />
                        <attribute name='firstname' />
                        <attribute name='emailaddress1' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_line1' />
                        <attribute name='address1_stateorprovince' />
                        <attribute name='address1_country' />
                        <attribute name='address1_city' />
                        <attribute name='entityimage' />
                        <attribute name='bcrm_policynumber' />
                        <attribute name='bcrm_healthcodenumber' />
                        <attribute name='bcrm_patientnumber' />
                        <attribute name='bcrm_nhsnumber' />
                        <attribute name='bcrm_ifrnhsno' />
                        <attribute name='bcrm_funded' />
                        <attribute name='parentcustomerid' />
                        <attribute name='birthdate' />
                        <attribute name='bcrm_middlename' />
                        <attribute name='preferredcontactmethodcode' />
                        <attribute name='donotemail' />
                        <attribute name='followemail' />
                        <attribute name='donotbulkemail' />
                        <attribute name='donotphone' />
                        <attribute name='donotpostalmail' />
                        <attribute name='donotfax' />
                        <attribute name='msemr_contact1relationship' />
                        <attribute name='msemr_contact1' />
                        <attribute name='bcrm_wheredidyouhearaboutus' />
                        <attribute name='bcrm_iunderstandthatdoctornowisaprivategpservi' />
                        <attribute name='bcrm_iunderstandthatacancellationfeewillbeappl' />
                        <attribute name='bcrm_iwishtoreceivetextmessagesfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceiveemailsfromdoctornow' />
                        <attribute name='bcrm_iwishtoreceivepostalcommunicationfromdoct' />
                        <attribute name='bcrm_iwouldliketoreceiveupdatesaboutnewservice' />
                        <attribute name='bcrm_iamhappyfordoctornowtocontactmynextofkin' />
                        <attribute name='bcrm_smokingstatus' />
                        <attribute name='bcrm_howmanyunitsofalcoholdoyoudrinkinanavera' />
                        <attribute name='bcrm_neverdrinkalcohol' />
                        <attribute name='bcrm_diabetes' />
                        <attribute name='bcrm_highbloodpressure' />
                        <attribute name='bcrm_heartdisease' />
                        <attribute name='bcrm_kidneydisease' />
                        <attribute name='bcrm_cancer' />
                        <attribute name='bcrm_thyroiddisease' />
                        <attribute name='bcrm_epilepsy' />
                        <attribute name='bcrm_depression' />
                        <attribute name='bcrm_stroke' />
                        <attribute name='bcrm_asthma' />
                        <attribute name='bcrm_gptype' />
                        <attribute name='bcrm_transientischaemicattack' />
                        <attribute name='bcrm_mentalillness' />
                        <attribute name='bcrm_heartrhythmproblems' />
                        <attribute name='bcrm_pleasegivedetailsofanyothersignificantill' />
                        <attribute name='bcrm_pleasegivedetailsofanymedicalconditionswi' />
                        <attribute name='bcrm_pleasegivedetailsifyouhaveanyallergiestoa' />
                        <attribute name='bcrm_kinsfirstname' />
                        <attribute name='bcrm_kinslastname' />
                        <attribute name='address2_line1' />
                        <attribute name='emailaddress2' />
                        <attribute name='bcrm_gpsurgeryname' />
                        <attribute name='bcrm_doctorsname' />
                        <attribute name='address3_city' />
                        <attribute name='telephone3' />
                        <attribute name='address2_postalcode' />
                         <attribute name='bcrm_occuption' />
                         <attribute name='bcrm_smokingstatus' />
                         <attribute name='bcrm_stoppedsmoking' />
                          <attribute name='bcrm_gpc_sequence_number' />
                           <attribute name='bcrm_pdsjson' />
                         <attribute name='bcrm_stoppeddrinkingalcohol' />
                          <attribute name='bcrm_gpc_registractionperiod' />
                         <attribute name='bcrm_alcoholintake' />
                        <attribute name='bcrm_modeofcommunication' />
                         <attribute name='bcrm_gpc_communicationproficiency' />
                         <attribute name='bcrm_title' />
                         <attribute name='description' />
                         <attribute name='bcrm_donotallowsms' />
                       <attribute name='statuscode' />

                    <attribute name='mobilephone' />
                    <attribute name='address2_telephone1' />
                    <attribute name='emailaddress3' />
                    <attribute name='address1_telephone1' />
                    <attribute name='emailaddress2' />

                    <attribute name='bcrm_tempemail' />
                    <attribute name='bcrm_tempphone' />

                      <attribute name='bcrm_languages' />
                      <attribute name='bcrm_interpreterrequired' />
                      <attribute name='bcrm_nhsnumberverificationstatus' />
                      <attribute name='bcrm_nhsnumberverificationstatusdisplay' />

                        <attribute name='bcrm_deceaseddate' />
                         <attribute name='bcrm_age' />
                        <order attribute='createdon' descending='true' />
                        <filter type='or'>
                          <condition attribute='bcrm_nhsnumber' operator='like' value='%" + NHSNumber + @"%' />
                        </filter>
                       <link-entity name='bcrm_staff' from='bcrm_staffid' to='bcrm_gpc_generalpractioner' visible='false' link-type='outer' alias='staff'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_clinic' from='bcrm_clinicid' to='bcrm_gpc_manangingorganisation' visible='false' link-type='outer' alias='clinic'>
                            <attribute name='bcrm_gpc_sequence_number' />
                          </link-entity>
                          <link-entity name='bcrm_gpclocation' from='bcrm_gpclocationid' to='bcrm_gpc_preferredbarnchsurgery' visible='false' link-type='outer' alias='location'>
                            <attribute name='bcrm_gpcsequencenumber' />
                          </link-entity>
                       
                      </entity>
                    </fetch>";

                EntityCollection AnswerCollection = _crmServiceClient.RetrieveMultiple(new FetchExpression(contactXML));

                List<PatientDTO> AllPatientDetails = new List<PatientDTO>();
                PatientDTO crmUserProfile = new PatientDTO();


                if (AnswerCollection != null && AnswerCollection.Entities.Count > 0)
                {
                    var record = AnswerCollection.Entities[0];
                    {
                        crmUserProfile.Id = record.Id;

                        dynamic staffEntity = record.Attributes.Contains("staff.bcrm_gpc_sequence_number") ? record["staff.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.stuffRefrenceNumber = staffEntity.ToString() != "" ? staffEntity.Value : string.Empty;

                        dynamic clinicEntity = record.Attributes.Contains("clinic.bcrm_gpc_sequence_number") ? record["clinic.bcrm_gpc_sequence_number"] : string.Empty;
                        crmUserProfile.clinicRefrenceNumber = clinicEntity.ToString() != "" ? clinicEntity.Value : string.Empty;

                        dynamic locationEntity = record.Attributes.Contains("location.bcrm_gpcsequencenumber") ? record["location.bcrm_gpcsequencenumber"] : string.Empty;
                        crmUserProfile.locationRefrenceNumber = locationEntity.ToString() != "" ? locationEntity.Value : string.Empty;

                        crmUserProfile.bcrm_title_label = record.Attributes.Contains("bcrm_title") ? record.FormattedValues["bcrm_title"].ToString() : string.Empty;

                        crmUserProfile.modeOfCommunication = record.Attributes.Contains("bcrm_modeofcommunication") ? record.FormattedValues["bcrm_modeofcommunication"].ToString() : string.Empty;
                        crmUserProfile.communicationProficiency = record.Attributes.Contains("bcrm_gpc_communicationproficiency") ? record.FormattedValues["bcrm_gpc_communicationproficiency"].ToString() : string.Empty;

                        dynamic check = record.Attributes.Contains("Patient_relationships") ? record["Patient_relationships"].ToString() : string.Empty;

                        crmUserProfile.bcrm_middlename = record.Attributes.Contains("bcrm_middlename") ? record["bcrm_middlename"].ToString() : string.Empty;
                        crmUserProfile.fullname = record.Attributes.Contains("fullname") ? record["fullname"].ToString() : string.Empty;
                        crmUserProfile.firstname = record.Attributes.Contains("firstname") ? record["firstname"].ToString() : string.Empty;
                        crmUserProfile.lastname = record.Attributes.Contains("lastname") ? record["lastname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_age = record.Attributes.Contains("bcrm_age") ? record["bcrm_age"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;
                        crmUserProfile.emailaddress1 = record.Attributes.Contains("emailaddress1") ? record["emailaddress1"].ToString() : string.Empty;
                        crmUserProfile.address1_city = record.Attributes.Contains("address1_city") ? record["address1_city"].ToString() : "";
                        crmUserProfile.address1_country = record.Attributes.Contains("address1_country") ? record["address1_country"].ToString() : "";
                        crmUserProfile.address1_line1 = record.Attributes.Contains("address1_line1") ? record["address1_line1"].ToString() : "";
                        crmUserProfile.telephone1 = record.Attributes.Contains("telephone1") ? record["telephone1"].ToString() : "";
                        crmUserProfile.address1_stateorprovince = record.Attributes.Contains("address1_stateorprovince") ? record["address1_stateorprovince"].ToString() : "";
                        crmUserProfile.address1_postalcode = record.Attributes.Contains("address1_postalcode") ? record["address1_postalcode"].ToString() : "";
                        if (record.Attributes.Contains("entityimage")) crmUserProfile.entityimage = record["entityimage"] as byte[];
                        if (record.Attributes.Contains("birthdate")) { crmUserProfile.birthdate = (DateTime)record.Attributes["birthdate"]; }
                        if (record.Attributes.Contains("bcrm_deceaseddate")) { crmUserProfile.deceasedDate = (DateTime)record.Attributes["bcrm_deceaseddate"]; }
                        crmUserProfile.gender = record.Attributes.Contains("gendercode") ? record.FormattedValues["gendercode"].ToString() : string.Empty;

                        crmUserProfile.gendercode = record.Contains("gendercode") && record["gendercode"] != null ? Convert.ToInt32(((OptionSetValue)record["gendercode"]).Value) : 0;
                        crmUserProfile.bcrm_policynumber = record.Attributes.Contains("bcrm_policynumber") ? record["bcrm_policynumber"].ToString() : "";
                        crmUserProfile.bcrm_healthcodenumber = record.Attributes.Contains("bcrm_healthcodenumber") ? record["bcrm_healthcodenumber"].ToString() : "";
                        crmUserProfile.bcrm_patientnumber = record.Attributes.Contains("bcrm_patientnumber") ? record["bcrm_patientnumber"].ToString() : "";
                        crmUserProfile.bcrm_nhsnumber = record.Attributes.Contains("bcrm_nhsnumber") ? record["bcrm_nhsnumber"].ToString() : "";
                        crmUserProfile.bcrm_ifrnhsno = record.Attributes.Contains("bcrm_ifrnhsno") ? record["bcrm_ifrnhsno"].ToString() : "";
                        crmUserProfile.bcrm_funded_name = record.Attributes.Contains("bcrm_funded") ? record.FormattedValues["bcrm_funded"].ToString() : string.Empty;
                        crmUserProfile.bcrm_funded = record.Contains("bcrm_funded") && record["bcrm_funded"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_funded"]).Value) : 0;
                        crmUserProfile.preferredcontactmethodcode = record.Contains("preferredcontactmethodcode") && record["preferredcontactmethodcode"] != null ? Convert.ToInt32(((OptionSetValue)record["preferredcontactmethodcode"]).Value) : 0;
                        crmUserProfile.parentcustomerid = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Id : Guid.Empty;
                        crmUserProfile.parentcustomername = record.Attributes.Contains("parentcustomerid") ? ((EntityReference)record["parentcustomerid"]).Name : string.Empty;
                        crmUserProfile.donotemail = record.Attributes.Contains("donotemail") ? ((bool)record["donotemail"]) : false;
                        crmUserProfile.followemail = record.Attributes.Contains("followemail") ? ((bool)record["followemail"]) : false;
                        crmUserProfile.donotbulkemail = record.Attributes.Contains("donotbulkemail") ? ((bool)record["donotbulkemail"]) : false;
                        crmUserProfile.donotphone = record.Attributes.Contains("donotphone") ? ((bool)record["donotphone"]) : false;
                        crmUserProfile.donotpostalmail = record.Attributes.Contains("donotpostalmail") ? ((bool)record["donotpostalmail"]) : false;
                        crmUserProfile.donotfax = record.Attributes.Contains("donotfax") ? ((bool)record["donotfax"]) : false;
                        //Fields from Doctor's Now
                        crmUserProfile.msemr_contact1relationship = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Id : Guid.Empty;
                        crmUserProfile.msemr_contact1relationship_name = record.Attributes.Contains("msemr_contact1relationship") ? ((EntityReference)record["msemr_contact1relationship"]).Name : string.Empty;
                        crmUserProfile.msemr_contact1 = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Id : Guid.Empty;
                        crmUserProfile.emergencycontact = record.Attributes.Contains("msemr_contact1") ? ((EntityReference)record["msemr_contact1"]).Name : String.Empty;
                        crmUserProfile.bcrm_wheredidyouhearaboutus = record.Contains("bcrm_wheredidyouhearaboutus") && record["bcrm_wheredidyouhearaboutus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_wheredidyouhearaboutus"]).Value) : 0;
                        crmUserProfile.bcrm_gptype = record.Contains("bcrm_gptype") && record["bcrm_gptype"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_gptype"]).Value) : 0;
                        crmUserProfile.bcrm_iunderstandthatdoctornowisaprivategpservi = record.Attributes.Contains("bcrm_iunderstandthatdoctornowisaprivategpservi") ? ((bool)record["bcrm_iunderstandthatdoctornowisaprivategpservi"]) : false;
                        crmUserProfile.bcrm_iunderstandthatacancellationfeewillbeappl = record.Attributes.Contains("bcrm_iunderstandthatacancellationfeewillbeappl") ? ((bool)record["bcrm_iunderstandthatacancellationfeewillbeappl"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivetextmessagesfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceivetextmessagesfromdoctornow") ? ((bool)record["bcrm_iwishtoreceivetextmessagesfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceiveemailsfromdoctornow = record.Attributes.Contains("bcrm_iwishtoreceiveemailsfromdoctornow") ? ((bool)record["bcrm_iwishtoreceiveemailsfromdoctornow"]) : false;
                        crmUserProfile.bcrm_iwishtoreceivepostalcommunicationfromdoct = record.Attributes.Contains("bcrm_iwishtoreceivepostalcommunicationfromdoct") ? ((bool)record["bcrm_iwishtoreceivepostalcommunicationfromdoct"]) : false;
                        crmUserProfile.bcrm_iwouldliketoreceiveupdatesaboutnewservice = record.Attributes.Contains("bcrm_iwouldliketoreceiveupdatesaboutnewservice") ? ((bool)record["bcrm_iwouldliketoreceiveupdatesaboutnewservice"]) : false;
                        crmUserProfile.bcrm_iamhappyfordoctornowtocontactmynextofkin = record.Attributes.Contains("bcrm_iamhappyfordoctornowtocontactmynextofkin") ? ((bool)record["bcrm_iamhappyfordoctornowtocontactmynextofkin"]) : false;
                        crmUserProfile.bcrm_smokingstatus = record.Contains("bcrm_smokingstatus") && record["bcrm_smokingstatus"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_smokingstatus"]).Value) : 0;
                        crmUserProfile.bcrm_occuption = record.Attributes.Contains("bcrm_occuption") ? record["bcrm_occuption"].ToString() : "";
                        crmUserProfile.bcrm_stoppedsmoking = record.Attributes.Contains("bcrm_stoppedsmoking") ? record["bcrm_stoppedsmoking"].ToString() : "";
                        crmUserProfile.bcrm_stoppeddrinkingalcohol = record.Attributes.Contains("bcrm_stoppeddrinkingalcohol") ? record["bcrm_stoppeddrinkingalcohol"].ToString() : "";
                        crmUserProfile.bcrm_alcoholintake = record.Contains("bcrm_alcoholintake") && record["bcrm_alcoholintake"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_alcoholintake"]).Value) : 0;
                        crmUserProfile.bcrm_alcoholintake_name = record.Attributes.Contains("bcrm_alcoholintake") ? record.FormattedValues["bcrm_alcoholintake"].ToString() : string.Empty;
                        crmUserProfile.otherDetails = record.Attributes.Contains("description") ? record["description"].ToString() : "";
                        crmUserProfile.bcrm_howmanyunitsofalcoholdoyoudrinkinanavera = record.Attributes.Contains("bcrm_howmanyunitsofalcoholdoyoudrinkinanavera") ? record["bcrm_howmanyunitsofalcoholdoyoudrinkinanavera"].ToString() : string.Empty;
                        crmUserProfile.bcrm_neverdrinkalcohol = record.Attributes.Contains("bcrm_neverdrinkalcohol") ? ((bool)record["bcrm_neverdrinkalcohol"]) : false;
                        crmUserProfile.bcrm_diabetes = record.Attributes.Contains("bcrm_diabetes") ? ((bool)record["bcrm_diabetes"]) : false;
                        crmUserProfile.bcrm_highbloodpressure = record.Attributes.Contains("bcrm_highbloodpressure") ? ((bool)record["bcrm_highbloodpressure"]) : false;
                        crmUserProfile.bcrm_heartdisease = record.Attributes.Contains("bcrm_heartdisease") ? ((bool)record["bcrm_heartdisease"]) : false;
                        crmUserProfile.bcrm_kidneydisease = record.Attributes.Contains("bcrm_kidneydisease") ? ((bool)record["bcrm_kidneydisease"]) : false;
                        crmUserProfile.bcrm_cancer = record.Attributes.Contains("bcrm_cancer") ? ((bool)record["bcrm_cancer"]) : false;
                        crmUserProfile.bcrm_thyroiddisease = record.Attributes.Contains("bcrm_thyroiddisease") ? ((bool)record["bcrm_thyroiddisease"]) : false;
                        crmUserProfile.bcrm_epilepsy = record.Attributes.Contains("bcrm_epilepsy") ? ((bool)record["bcrm_epilepsy"]) : false;
                        crmUserProfile.bcrm_depression = record.Attributes.Contains("bcrm_depression") ? ((bool)record["bcrm_depression"]) : false;
                        crmUserProfile.bcrm_stroke = record.Attributes.Contains("bcrm_stroke") ? ((bool)record["bcrm_stroke"]) : false;
                        crmUserProfile.bcrm_asthma = record.Attributes.Contains("bcrm_asthma") ? ((bool)record["bcrm_asthma"]) : false;
                        crmUserProfile.bcrm_transientischaemicattack = record.Attributes.Contains("bcrm_transientischaemicattack") ? ((bool)record["bcrm_transientischaemicattack"]) : false;
                        crmUserProfile.bcrm_mentalillness = record.Attributes.Contains("bcrm_mentalillness") ? ((bool)record["bcrm_mentalillness"]) : false;
                        crmUserProfile.bcrm_heartrhythmproblems = record.Attributes.Contains("bcrm_heartrhythmproblems") ? ((bool)record["bcrm_heartrhythmproblems"]) : false;
                        crmUserProfile.bcrm_pleasegivedetailsofanyothersignificantill = record.Attributes.Contains("bcrm_pleasegivedetailsofanyothersignificantill") ? record["bcrm_pleasegivedetailsofanyothersignificantill"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsofanymedicalconditionswi = record.Attributes.Contains("bcrm_pleasegivedetailsofanymedicalconditionswi") ? record["bcrm_pleasegivedetailsofanymedicalconditionswi"].ToString() : "";
                        crmUserProfile.bcrm_pleasegivedetailsifyouhaveanyallergiestoa = record.Attributes.Contains("bcrm_pleasegivedetailsifyouhaveanyallergiestoa") ? record["bcrm_pleasegivedetailsifyouhaveanyallergiestoa"].ToString() : "";
                        crmUserProfile.bcrm_kinsfirstname = record.Attributes.Contains("bcrm_kinsfirstname") ? record["bcrm_kinsfirstname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_kinslastname = record.Attributes.Contains("bcrm_kinslastname") ? record["bcrm_kinslastname"].ToString() : string.Empty;
                        crmUserProfile.address2_line1 = record.Attributes.Contains("address2_line1") ? record["address2_line1"].ToString() : string.Empty;
                        crmUserProfile.emailaddress2 = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.address2_postalcode = record.Attributes.Contains("address2_postalcode") ? record["address2_postalcode"].ToString() : string.Empty;
                        crmUserProfile.bcrm_gpsurgeryname = record.Attributes.Contains("bcrm_gpsurgeryname") ? record["bcrm_gpsurgeryname"].ToString() : string.Empty;
                        crmUserProfile.bcrm_doctorsname = record.Attributes.Contains("bcrm_doctorsname") ? record["bcrm_doctorsname"].ToString() : string.Empty;
                        crmUserProfile.address3_city = record.Attributes.Contains("address3_city") ? record["address3_city"].ToString() : string.Empty;
                        crmUserProfile.telephone3 = record.Attributes.Contains("telephone3") ? record["telephone3"].ToString() : string.Empty;
                        crmUserProfile.bcrm_donotallowsms = record.Contains("bcrm_donotallowsms") && record["bcrm_donotallowsms"] != null ? Convert.ToInt32(((OptionSetValue)record["bcrm_donotallowsms"]).Value) : 0;
                        crmUserProfile.GPCSequenceNumber = record.Attributes.Contains("bcrm_gpc_sequence_number") ? record["bcrm_gpc_sequence_number"].ToString() : string.Empty;
                        if (record.Attributes.Contains("bcrm_gpc_registractionperiod")) { crmUserProfile.GPCRegistractionDate = (DateTime)record.Attributes["bcrm_gpc_registractionperiod"]; }
                        crmUserProfile.HomePhone1 = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.PdsVersionId = record.Attributes.Contains("bcrm_pdsversionid") ? record["bcrm_pdsversionid"].ToString() : string.Empty;
                        crmUserProfile.pdsJson = record.Attributes.Contains("bcrm_pdsjson") ? record["bcrm_pdsjson"].ToString() : string.Empty;

                        crmUserProfile.InterpreterRequired = record.Attributes.Contains("bcrm_interpreterrequired") ? record.FormattedValues["bcrm_interpreterrequired"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Code = record.Attributes.Contains("bcrm_nhsnumberverificationstatus") ? record["bcrm_nhsnumberverificationstatus"].ToString() : string.Empty;
                        crmUserProfile.NHS_number_Verification_Status_Display = record.Attributes.Contains("bcrm_nhsnumberverificationstatusdisplay") ? record["bcrm_nhsnumberverificationstatusdisplay"].ToString() : string.Empty;


                        crmUserProfile.homeEmail = record.Attributes.Contains("emailaddress2") ? record["emailaddress2"].ToString() : string.Empty;
                        crmUserProfile.homePhone = record.Attributes.Contains("address1_telephone1") ? record["address1_telephone1"].ToString() : string.Empty;
                        crmUserProfile.workEmail = record.Attributes.Contains("emailaddress3") ? record["emailaddress3"].ToString() : string.Empty;
                        crmUserProfile.workPhone = record.Attributes.Contains("address2_telephone1") ? record["address2_telephone1"].ToString() : string.Empty;
                        crmUserProfile.mobilephone = record.Attributes.Contains("mobilephone") ? record["mobilephone"].ToString() : string.Empty;

                        crmUserProfile.tempEmail = record.Attributes.Contains("bcrm_tempemail") ? record["bcrm_tempemail"].ToString() : string.Empty;
                        crmUserProfile.tempPhone = record.Attributes.Contains("bcrm_tempphone") ? record["bcrm_tempphone"].ToString() : string.Empty;

                        var patientLanguages = record.Attributes.Contains("bcrm_languages") ? record.FormattedValues["bcrm_languages"].ToString() : string.Empty;
                        if (patientLanguages != "")
                        {
                            string[] allLanguages = patientLanguages.Split(new string[] { "; " }, StringSplitOptions.None);
                            List<practitionerLanguageDTO> languageList = new List<practitionerLanguageDTO>();

                            foreach (var item in allLanguages)
                            {
                                practitionerLanguageDTO pralan = new practitionerLanguageDTO();
                                string pattern = @"\((.*?)\)";
                                Match match = Regex.Match(item, pattern);
                                if (match.Success)
                                {
                                    pralan.languageCode = match.Groups[1].Value;
                                }
                                pralan.languageName = Regex.Replace(item, pattern, "");
                                pralan.languageName = Regex.Replace(pralan.languageName, "-", "").Trim();
                                languageList.Add(pralan);
                            }
                            crmUserProfile.patientLanguages = languageList;

                        }


                        dynamic stausCode = record.Attributes.Contains("statuscode") ? record["statuscode"] : string.Empty;
                        var statusValue = stausCode.ToString() != "" ? stausCode.Value : 0;
                        if (statusValue == 1)
                        {
                            crmUserProfile.statusReason = true;
                        }
                        else
                        {
                            crmUserProfile.statusReason = false;
                        }

                        AllPatientDetails.Add(crmUserProfile);

                    }
                    if (AllPatientDetails.Count > 0)
                    {
                        RegisterPatientDetails pd = new RegisterPatientDetails();

                        var json = pd.RegisterANewPatientAUsingJSONFHIR(AllPatientDetails[0], existStatus, addressTypeIsTemp, telecomTypeIsTemp, pdsAddress);

                        return json;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        #endregion

    }
}

