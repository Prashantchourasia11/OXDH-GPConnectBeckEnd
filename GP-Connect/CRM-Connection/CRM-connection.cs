using Microsoft.Crm.Sdk.Messages;
using Microsoft.Rest;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Net;
using System.ServiceModel;

using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Tooling.Connector;

namespace GP_Connect.CRM_Connection
{
    public class CRM_connection
    {
        #region CRM Connection            

        static string organizationUri = "https://oxdh1.crm11.dynamics.com";
        static string clientId = "e76f7080-6a4e-4174-bb36-76d082e80839";
        static string clientSecret = "LnZuNQ2fkdCCFkL9Q1-6O-eEP_A7wF~.51";

        static string organizationUrioxvc = "https://oxvh1.crm11.dynamics.com";
        static string clientIdoxvc = "a638a67a-45b9-46cf-954d-c19076d2c1db";
        static string clientSecretoxvc = "a5y4Cx6g6-koZmjoc7oeH7R-APgW._xq85";

        #endregion

        #region Calss Level Objects 

        private static OrganizationServiceProxy proxy = null;
        private static IOrganizationService Service = null;
        #endregion

        #region Method
        public IOrganizationService crmconnection()
        {
            try
            {
                ServiceClient service = new ServiceClient($@"AuthType=ClientSecret;url={organizationUri};ClientId={clientId};ClientSecret={clientSecret}");
                //WhoAmIResponse whoAmIResponse = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
                //Console.WriteLine($"Connected with UserId: {whoAmIResponse.UserId}");
                return service;
            }
            catch (Exception ex)
            {

                return null;

            }

        }

        public IOrganizationService crmconnectionOXVC()
        {
            try
            {
              
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

             ServiceClient service = new ServiceClient($@"AuthType=ClientSecret;url={organizationUrioxvc};ClientId={clientIdoxvc};ClientSecret={clientSecretoxvc};SkipDiscovery=true;RequireNewInstance=true");
            //WhoAmIResponse whoAmIResponse = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
            //Console.WriteLine($"Connected with UserId: {whoAmIResponse.UserId}");
            return service;

            }
            catch (Exception ex) 
            {
                return null;
            }
        }
        public IOrganizationService crmconnectionOXVC1()
        {
            ServiceClient service = new ServiceClient($@"AuthType=ClientSecret;url={organizationUrioxvc};ClientId={clientIdoxvc};ClientSecret={clientSecretoxvc}");
            //WhoAmIResponse whoAmIResponse = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
            //Console.WriteLine($"Connected with UserId: {whoAmIResponse.UserId}");
            return service;
        }


        #endregion
    }
}
