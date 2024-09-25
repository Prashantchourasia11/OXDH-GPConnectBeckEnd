namespace GP_Connect.DataTransferObject
{
   
    public class PatientDTO
    {
        public bool PDS_PatientDeceased { get; set; }
        public bool PDS_PatientRestricted { get; set; }
        public bool PDS_LocalDataNotSync { get; set; }
        public bool PDS_NHSNumberSuperSeded { get; set; }
        public bool PDS_PatientNotFoundByNHSNumber { get; set; }
        public bool PDS_PatientMobileNumberNotSync { get; set; }

        public Guid Id { get; set; }
        public string emailaddress1 { get; set; }
        public string PdsVersionId { get; set; }
        public string bcrm_age { get; set; }
        public string bcrm_modifiedby { get; set; }
        public string bcrm_patientalert { get; set; }
        public string fullname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobilephone { get; set; }
        public string telephone1 { get; set; }
        public string address1_line1 { get; set; }
        public string address1_postalcode { get; set; }
        public string address1_city { get; set; }
        public string address1_country { get; set; }
        public string address1_composite { get; set; }
        public string territorycode { get; set; }
        public string address1_stateorprovince { get; set; }
        public string bcrm_policynumber { get; set; }
        public string bcrm_healthcodenumber { get; set; }
        public string bcrm_patientnumber { get; set; }
        public string bcrm_nhsnumber { get; set; }
        public string bcrm_ifrnhsno { get; set; }
        public byte[] entityimage { get; set; }
        public DateTime birthdate { get; set; }
        public DateTime bcrm_joiningdate { get; set; }
        public DateTime bcrm_expirydate { get; set; }
        public DateTime bcrm_renewaldate { get; set; }

        public DateTime deceasedDate { get; set; }

        public string gender { get; set; }
        public int bcrm_title { get; set; }
        public string bcrm_title_label { get; set; }
        public string bcrm_funded_name { get; set; }
        public string bcrm_membershiplevel_name { get; set; }
        public int gendercode { get; set; }
        public int bcrm_funded { get; set; }
        public int bcrm_membershiplevel { get; set; }
        public Guid parentcustomerid { get; set; }
        public string parentcustomername { get; set; }
        public Guid billingAccount { get; set; }
        public string billingAccountName { get; set; }
        public bool donotemail { get; set; }
        public bool followemail { get; set; }
        public bool donotbulkemail { get; set; }
        public bool donotphone { get; set; }
        public bool donotpostalmail { get; set; }
        public bool donotfax { get; set; }
        public int preferredcontactmethodcode { get; set; }

        public string bcrm_kinsfirstname { get; set; }
        public string bcrm_kinslastname { get; set; }
        public string address2_line1 { get; set; }
        public string emailaddress2 { get; set; }
        public string emailaddress3 { get; set; }
        public string address2_postalcode { get; set; }

        public string bcrm_gpsurgeryname { get; set; }
        public string bcrm_doctorsname { get; set; }
        public string address3_city { get; set; }
        public string telephone3 { get; set; }
        public string bcrm_nhssurgery { get; set; }
        public string bcrm_nhssurgerytelephone { get; set; }

        public Guid msemr_contact1relationship { get; set; }
        public string msemr_contact1relationship_name { get; set; }

        public Guid msemr_contact1 { get; set; }
        public string emergencycontact { get; set; }
        public int bcrm_wheredidyouhearaboutus { get; set; }
        public string bcrm_wheredidyouhearaboutus_label { get; set; }
        public int bcrm_gptype { get; set; }
        public bool bcrm_iunderstandthatdoctornowisaprivategpservi { get; set; }
        public bool bcrm_iunderstandthatacancellationfeewillbeappl { get; set; }
        public bool bcrm_iwishtoreceivetextmessagesfromdoctornow { get; set; }
        public bool bcrm_iwishtoreceiveemailsfromdoctornow { get; set; }
        public bool bcrm_iwishtoreceivepostalcommunicationfromdoct { get; set; }
        public bool bcrm_iwouldliketoreceiveupdatesaboutnewservice { get; set; }
        public bool bcrm_iamhappyfordoctornowtocontactmynextofkin { get; set; }
        public int bcrm_smokingstatus { get; set; }
        public string bcrm_smokingstatus_name { get; set; }
        public string bcrm_howmanyunitsofalcoholdoyoudrinkinanavera { get; set; }
        public bool bcrm_neverdrinkalcohol { get; set; }
        public bool bcrm_diabetes { get; set; }
        public bool bcrm_highbloodpressure { get; set; }
        public bool bcrm_heartdisease { get; set; }
        public bool bcrm_kidneydisease { get; set; }
        public bool bcrm_cancer { get; set; }
        public bool bcrm_thyroiddisease { get; set; }
        public bool bcrm_epilepsy { get; set; }
        public bool bcrm_depression { get; set; }
        public bool bcrm_stroke { get; set; }
        public bool bcrm_asthma { get; set; }
        public bool bcrm_transientischaemicattack { get; set; }
        public bool bcrm_mentalillness { get; set; }
        public bool bcrm_heartrhythmproblems { get; set; }
        public string bcrm_pleasegivedetailsofanyothersignificantill { get; set; }
        public string bcrm_pleasegivedetailsofanymedicalconditionswi { get; set; }
        public string bcrm_pleasegivedetailsifyouhaveanyallergiestoa { get; set; }
        public string bcrm_occuption { get; set; }
        public string bcrm_stoppedsmoking { get; set; }
        public string bcrm_stoppeddrinkingalcohol { get; set; }
        public int bcrm_alcoholintake { get; set; }
        public string bcrm_alcoholintake_name { get; set; }
        public string otherDetails { get; set; }
        public string otherDetails2 { get; set; }
        public string otherDetails3 { get; set; }
        public string priceListName { get; set; }
        public Guid priceListID { get; set; }
        public string billingPriceListName { get; set; }
        public Guid billingPriceListID { get; set; }
        public Guid priceListIdForInvoice { get; set; }
        public string bcrm_membershipstatus { get; set; }
        public int bcrm_membershipstatus_value { get; set; }

        public string bcrm_nhsgpname { get; set; }
        public string bcrm_nhsgpsurgeryaddress { get; set; }
        public string msemr_contact1startdate { get; set; }
        public string bcrm_insurancecompany { get; set; }
        public Guid bcrm_insurancecompanyId { get; set; }

        public string bcrm_status { get; set; }
        public string bcrm_referrer { get; set; }
        public string bcrm_religion { get; set; }
        public string bcrm_maritalstatus { get; set; }
        public string bcrm_defaultpayertype { get; set; }
        public string telephone2 { get; set; }
        public string address2_composite { get; set; }
        public string bcrm_preferredgpname { get; set; }
        public Guid bcrm_preferredgpnameId { get; set; }
        public bool bcrm_privacypolicy { get; set; }
        public bool bcrm_ageofconsent { get; set; }
        public bool bcrm_sharingyourpersonaldata { get; set; }
        public bool bcrm_sharinganonymiseddata { get; set; }
        public bool bcrm_communicationconsent { get; set; }
        public bool bcrm_cookiespolicy { get; set; }
        public bool bcrm_termsandconditions { get; set; }

        public Guid accountPrimaryContactId { get; set; }
        public string accountPrimaryContactName { get; set; }

        public string bcrm_parentorganization { get; set; }
        public Guid bcrm_parentorganizationId { get; set; }
        public DateTime bcrm_recordchangedon { get; set; }
        public string Accountrecordcreatedby { get; set; }
        public string bcrm_recordcreatedby { get; set; }
        public int bcrm_donotallowsms { get; set; }
        public string bcrm_middlename { get; set; }
        public string bcrm_referredby_name { get; set; }
        public int bcrm_referredby { get; set; }
        public int ContactType { get; set; }
        public string ContactType_label { get; set; }

        public string WorkEmail { get; set; }
        public string WorkPhone { get; set; }
        public string HomeEmail { get; set; }
        public string HomePhone { get; set; }

        public string HomeAddressLines { get; set; }
        public string HomeAddressPostalCode { get; set; }
        public string WorkAddressLines { get; set; }
        public string WorkAddressPostalCode { get; set; }


        public string ConfidentialityCode { get; set; }



        public bool isGroupParticipantUpdate { get; set; }
        public Guid groupParticipantId { get; set; }
        public string TitleValue { get; set; }

        public string GPCSequenceNumber { get; set; }

        public string HomePhone1 { get; set; }

        public DateTime GPCRegistractionDate { get; set; }

        public string stuffRefrenceNumber { get; set; }

        public string clinicRefrenceNumber { get; set; }

        public string locationRefrenceNumber { get; set; }

        public string modeOfCommunication { get; set; }

        public string communicationProficiency { get; set; }

        public string pdsJson { get; set; }

        public string NHS_number_Verification_Status_Code { get; set; }

        public string NHS_number_Verification_Status_Display { get; set; }

        public string LanguageCode { get; set; }

        public string LanguageDisplay { get; set; }

        public string InterpreterRequired { get; set; }

        public string RelatedPersonGivenName { get; set; }

        public string RelatedPersonFamilyName { get; set; }

        public string RelatedPersonFullName { get; set; }

        public string RelatedPersonPrefix { get; set; }

        public string RelatedPersonMobilePhone { get; set; }

        public string RelatedPersonAddressLine { get; set; }

        public string RelatedPersonPostalCode { get; set; }

        public string RelatedPersonGender { get; set; }

        public string RelatedPersonText { get; set; }

        public string RelatedPersonRelationship { get; set; }

        public bool statusReason { get; set; }

        public List<practitionerLanguageDTO> patientLanguages { get; set; }

        public string workEmail { get;set; }

        public string workPhone { get; set; }

        public string homePhone { get; set; }

        public string homeEmail { get; set; }   

        public string mobilePhone { get; set; }

        public string tempEmail { get; set; }   

        public string tempPhone { get; set; }

        public string IsSensitive { get; set; } 
    }

}
