namespace GP_Connect.DataTransferObject
{
    public class PatientSearchPDSV1
    {
        public bool patientId { get; set; }
        public bool isUpnPafKeyExist { get; set; }
        public bool pdsPatientDeceased { get; set; }
        public bool pdsPatientRestricted { get; set; }
        public bool pdsLocalDataNotSync { get; set; }
        public bool pdsNhsNumberSuperSeded { get; set; }
        public bool pdsPatientNotFoundByNhsNumber { get; set; }
        public bool pdsPatientMobileNumberNotSync { get; set; }
        public bool pdsInvalidResource { get; set; }
        public string? middleNameOperation { get; set; }
        public string? namePrefixOperation { get; set; }
        public string? nameGivenOperation { get; set; }
        public string? nameFamilyOperation { get; set; }
        public string? homeAddressLinesOld { get; set; }
        public string? genderOperation { get; set; }
        public string? dateOfBirthOperation { get; set; }
        public string? communication { get; set; }
        public int communicationIndex { get; set; }
        public string? communicationCode { get; set; }
        public bool interpreterRequired { get; set; }
        public string resourceType { get; set; } = string.Empty;
        public string nhsNumber { get; set; } = string.Empty;
        public string nhsNumberVerificationStatusCode { get; set; } = string.Empty;
        public string nhsNumberVerificationStatusValue { get; set; } = string.Empty;
        public string confidentialityValue { get; set; } = string.Empty;
        public string confidentialityCode { get; set; } = string.Empty;
        public string nameId { get; set; } = string.Empty;
        public string namePrefix { get; set; } = string.Empty;
        public string nameGiven { get; set; } = string.Empty;
        public string middleName { get; set; } = string.Empty;
        public string middleNameOld { get; set; } = string.Empty;
        public string nameFamily { get; set; } = string.Empty;
        public string nameSuffix { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string birthDate { get; set; } = string.Empty;
        public string deceasedDateAndTime { get; set; } = string.Empty;
        public string multipleBirths { get; set; } = string.Empty;
        public string deathNotificationCode { get; set; } = string.Empty;
        public string deathNotificationStatus { get; set; } = string.Empty;
        public string deathNotificationEffectiveDate { get; set; } = string.Empty;
        public string? deathAddOrUpdateType { get; set; }
        public string? dateOfDeathIndex { get; set; }
        public string address { get; set; } = string.Empty;
        public string addressUse { get; set; } = string.Empty;
        public string addressLines { get; set; } = string.Empty;
        public string homeAddressLines { get; set; } = string.Empty;
        public string homeAddressId { get; set; } = string.Empty;
        public int homeAddressIndex { get; set; }
        public string homeAddressPostalCode { get; set; } = string.Empty;
        public string homeAddressPostalCodeOperation { get; set; } = string.Empty;
        public string homeAddressStartDate { get; set; } = string.Empty;
        public string homeAddressStartDateOperation { get; set; } = string.Empty;
        public string homeAddressEndDate { get; set; } = string.Empty;
        public string postalCode { get; set; } = string.Empty;
        public string relationship { get; set; } = string.Empty;
        public string phoneId { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public int phoneIndex { get; set; }
        public string homePhoneId { get; set; } = string.Empty;
        public string homePhone { get; set; } = string.Empty;
        public int homePhoneIndex { get; set; }
        public string workPhoneId { get; set; } = string.Empty;
        public string workPhone { get; set; } = string.Empty;
        public int workPhoneIndex { get; set; }
        public string homeEmailId { get; set; } = string.Empty;
        public string homeEmail { get; set; } = string.Empty;
        public int homeEmailIndex { get; set; }
        public string workEmailId { get; set; } = string.Empty;
        public string workEmail { get; set; } = string.Empty;
        public int workEmailIndex { get; set; }
        public string generalPractitionerIdentifier { get; set; } = string.Empty;
        public string generalPractitionerType { get; set; } = string.Empty;
        public string generalPractitionerValue { get; set; } = string.Empty;

        //public string WorkAddressLines { get; set; } = string.Empty;
        //public string WorkAddressId { get; set; } = string.Empty;
        //public int WorkAddressIndex { get; set; }
        //public string WorkAddressPostalCode { get; set; } = string.Empty;
        //public string WorkAddressStartDate { get; set; } = string.Empty;
        //public string WorkAddressEndDate { get; set; } = string.Empty;


        public string generalPractitionerPeriodStart { get; set; } = string.Empty;
        public string generalPractitionerPeriodEnd { get; set; } = string.Empty;
        public string generalPractitionerName { get; set; } = string.Empty;
        public string generalPractitionerStatus { get; set; } = string.Empty;
        public string generalPractitionerAddrLn1 { get; set; } = string.Empty;
        public string generalPractitionerAddrLn2 { get; set; } = string.Empty;
        public string generalPractitionerTown { get; set; } = string.Empty;
        public string generalPractitionerCounty { get; set; } = string.Empty;
        public string generalPractitionerPostCode { get; set; } = string.Empty;
        public string generalPractitionerCountry { get; set; } = string.Empty;
        public string generalPractitionerTelephone { get; set; } = string.Empty;
        public string generalPractionerNotFound { get; set; } = string.Empty;
        public string errorCode { get; set; } = string.Empty;
        public string errorDetail { get; set; } = string.Empty;
        public string errorDignostics { get; set; } = string.Empty;
        public string versionId { get; set; } = string.Empty;
        public string pdsJson { get; set; } = string.Empty;
        public string? nominatedPharmacyCode { get; set; }
        public string? nominatedPharmacyName { get; set; }
        public string? nominatedPharmacyTelephone { get; set; }
        public string? nominatedPharmacyFax { get; set; }
        public string? nominatedPharmacyStreet { get; set; }
        public string? nominatedPharmacyLocality { get; set; }
        public string? nominatedPharmacyTown { get; set; }
        public string? nominatedPharmacyAdministrative { get; set; }
        public string? nominatedPharmacyPostcode { get; set; }
        public string? nominatedPharmacyNotFound { get; set; }
        public string? nominatedPharmacyType { get; set; }
        public string? telecomData { get; set; }
        public List<Pharmacy>? pharmacies { get; set; }
        public int pharmacyIndex { get; set; }

        public string address1_line1 {  get; set; }

        public string address1_city { get; set; }

        public string address1_stateorprovince { get; set; }

        public string address1_postalcode { get; set; }

        public string GPConnectAddressCheck { get; set; }

        public string GPConnectTelecomCheck { get; set; }

        public string PatientExistStatus {  get; set; }

        public string tempEmail { get; set; }

        public string tempPhone { get; set; }

        public string GPCRegistractionJSON { get; set; }

        public string AddressType { get; set; }
    }
    public class Pharmacy
    {
        public string? name { get; set; }
        public int index { get; set; }
        public string? code { get; set; }
        public string? telephone { get; set; }
        public string? fax { get; set; }
        public string? street { get; set; }
        public string? locality { get; set; }
        public string? town { get; set; }
        public string? administrative { get; set; }
        public string? postcode { get; set; }
        public string? type { get; set; }
        public string? pharmacyNotFound { get; set; }
        public string? pharmacyTypeCode { get; set; }
    }
    public class AddressDataItem
    {
        public string? id { get; set; }
        public string? postalCode { get; set; }
        public string? addressLines { get; set; }
        public string? start { get; set; }
        public string? end { get; set; }
        public int index { get; set; }
        public bool isUPRNPAFKeyExist { get; set; }
    }
    public class TelecomDataItem
    {
        public string? value { get; set; }
        public int index { get; set; }
        public string? id { get; set; }
    }
    public class RegisterPatientResponse
    {
        public Guid id { get; set; }
        public string? message { get; set; }
        public bool errorOccurStatus { get; set; }
    }
    public class CreatePatientResponse
    {
        public Guid id { get; set; }
        public bool status { get; set; }
        public string? responseMessage { get; set; }
    }
}
