using System.Text.Json.Serialization;

namespace GP_Connect.DataTransferObject
{

    public class AppointmentGetByReverseDTOActor
    {
        public string reference { get; set; }
    }

    public class AppointmentGetByReverseDTOCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class AppointmentGetByReverseDTOCodingType
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class AppointmentGetByReverseDTOContained
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentGetByReverseDTOContainetMeta meta { get; set; }
        public List<AppointmentGetByReverseDTOIdentifier> identifier { get; set; }
        public List<AppointmentGetByReverseDTOType> type { get; set; }
        public string name { get; set; }
        public List<AppointmentGetByReverseDTOTelecom> telecom { get; set; }
    }

    public class AppointmentGetByReverseDTOContainedNotIncludeType
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentGetByReverseDTOContainetMeta meta { get; set; }
        public List<AppointmentGetByReverseDTOIdentifier> identifier { get; set; }
        public string name { get; set; }
        public List<AppointmentGetByReverseDTOTelecom> telecom { get; set; }
    }


    public class AppointmentGetByReverseDTOExtension
    {
      
    }

    public class AppointmentGetByReverseDTOIdentifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class AppointmentGetByReverseDTOMeta
    {
        public string versionId { get; set; }
        public List<string> profile { get; set; }
    }
    public class AppointmentGetByReverseDTOContainetMeta
    {
 
        public List<string> profile { get; set; }
    }

    public class AppointmentGetByReverseDTOParticipant
    {
        public AppointmentGetByReverseDTOActor actor { get; set; }
        public string status { get; set; }
    }

    public class AppointmentGetByReverseDTO
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentGetByReverseDTOMeta meta { get; set; }
        public List<AppointmentGetByReverseDTOContained> contained { get; set; }
        public List<object> extension { get; set; }
        public string status { get; set; }
        public AppointmentGetByReverseDTOServiceCategory serviceCategory { get; set; }
        public List<AppointmentGetByReverseDTOServiceType> serviceType { get; set; }
        
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public List<AppointmentGetByReverseDTOSlot> slot { get; set; }
        public DateTime created { get; set; }

        public List<AppointmentGetByReverseDTOParticipant> participant { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string comment { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string description { get; set; }
    }


    public class AppointmentGetByReverseDTOServiceCategory
    {
        public string text { get; set; }
    }

    public class AppointmentGetByReverseDTOServiceType
    {
        public string text { get; set; }
    }

    public class AppointmentGetByReverseDTOSlot
    {
        public string reference { get; set; }
    }

    public class AppointmentGetByReverseDTOTelecom
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class AppointmentGetByReverseDTOType
    {
        public List<AppointmentGetByReverseDTOCodingType> coding { get; set; }
    }

    public class AppointmentGetByReverseDTOValueCodeableConcept
    {
        public List<AppointmentGetByReverseDTOCoding> coding { get; set; }
    }

    public class AppointmentGetByReverseDTOValueReference
    {
        public string reference { get; set; }
    }


    public class firstObjectOfExtensionApp
    {
        public string url { get; set; }
        public firstObjectOfExtensionAppValueReference valueReference { get; set; }
    }

    public class firstObjectOfExtensionAppValueReference
    {
        public string reference { get; set; }
    }

    public class SecondObjectOfExtensionAppCoding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class SecondObjectOfExtensionApp
    {
        public string url { get; set; }
        public SecondObjectOfExtensionAppValueCodeableConcept valueCodeableConcept { get; set; }
    }

    public class SecondObjectOfExtensionAppValueCodeableConcept
    {
        public List<SecondObjectOfExtensionAppCoding> coding { get; set; }
    }
    public class ThirdObjectOfExtensionApp
    {
        public string url { get; set; }
        public string valueCode { get; set; }
    }
    public class FourthObjectOfExtensionApp
    {
        public string url { get; set; }
        public string valueString { get; set; }
    }
    public class AppointmentGetByReverseDTOWithoutType
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public AppointmentGetByReverseDTOMeta meta { get; set; }
        public List<AppointmentGetByReverseDTOContainedNotIncludeType> contained { get; set; }
        public List<object> extension { get; set; }
        public string status { get; set; }
        public AppointmentGetByReverseDTOServiceCategory serviceCategory { get; set; }
        public List<AppointmentGetByReverseDTOServiceType> serviceType { get; set; }

        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public List<AppointmentGetByReverseDTOSlot> slot { get; set; }
        public DateTime created { get; set; }

        public List<AppointmentGetByReverseDTOParticipant> participant { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string comment { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string description { get; set; }
    }
}
