using GP_Connect.DataTransferObject;
using GP_Connect.Service.CommonMethods;
using GP_Connect.Service.Foundation;

namespace GP_Connect.Service.AccessStructureRecord
{
    public class ServiceAccessStructureRecord : IServiceAccessStructureRecord
    {
        #region Properties

        #endregion

        #region Constructor

        ServiceFoundation foundation = new ServiceFoundation();

        #endregion

        #region 

        public List<object> GetAccessStructureRecord(RequestAccessStructureRecordDTO patientDetailsRequest)
        {
            try
            {
                List<object> finalResponse = new List<object>();

                var nhsNumber = "";
                var allergy = false;
                var resolvedAllergy = false;
                var medication = false;
                var consultation = false;
                var consulationNumber = 0;
                var problems = false;
                var Immunisations = false;
                var UncategorisedData = false;
                var Investigations = false;
                var Referrals = false;

                foreach(var item in patientDetailsRequest.parameter)
                {
                    if(item.name == "patientNHSNumber")
                    {
                        nhsNumber = item.valueIdentifier.value;
                    }
                    if (item.name == "includeAllergies")
                    {
                        allergy = true;
                        if (item.part[0].valueBoolean == true)
                        {
                            resolvedAllergy = true;
                        }
                    }
                    if(item.name == "includeMedication")
                    {
                        medication = true;
                    }
                    if (item.name == "includeConsultations")
                    {
                        consultation = true;
                        if (item.part[0].valueInteger > 0)
                        {
                            consulationNumber = (int)item.part[0].valueInteger;
                        }
                    }
                    if (item.name == "includeProblems")
                    {
                        problems = true;
                    }
                    if (item.name == "includeImmunisations")
                    {
                        Immunisations = true;
                    }
                    if (item.name == "includeUncategorisedData")
                    {
                        UncategorisedData = true;
                    }
                    if (item.name == "includeInvestigations")
                    {
                        Investigations = true;
                    }
                    if (item.name == "includeReferrals")
                    {
                        Referrals = true;
                    }

                }

                if(nhsNumber != "")
                {
                    ServiceCommonMethod SCM = new ServiceCommonMethod();
                    var basicPatientDetails = SCM.GetAllDetailsOfPatientByNHSnumber(nhsNumber);
                    finalResponse.AddRange(basicPatientDetails);
                }

                if(allergy == true)
                {

                }
        
            

                return finalResponse;
            }
            catch(Exception)
            {
                return new List<object>();
            }
        }



        #endregion
    }
}
