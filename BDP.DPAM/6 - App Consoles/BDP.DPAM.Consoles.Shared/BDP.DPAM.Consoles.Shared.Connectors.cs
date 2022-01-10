using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.ServiceModel.Description;
using System.Threading;

namespace BDP.DPAM.Consoles.Shared
{
    public static class CRMConnector
    {

        public static IOrganizationService GetOrganizationServiceClientSecret(string clientId, string clientSecret, string organizationUri)
        {
            try
            {
                var conn = new CrmServiceClient($@"AuthType=ClientSecret;url={organizationUri};ClientId={clientId};ClientSecret={clientSecret}");

                return conn.OrganizationWebProxyClient != null ? conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
                return null;
            }
        }


        public static IOrganizationService GetLegacyOrganizationService(string ProcessName, int i = 0)
        {
            try
            {
                Uri url = new Uri(System.Configuration.ConfigurationManager.AppSettings["OnPremiseUrl"]);
                IServiceConfiguration<IOrganizationService> config = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(serviceUri: url);
                OrganizationServiceProxy org;
                if (System.Configuration.ConfigurationManager.AppSettings["OnPremiseUserName"] != null)
                {

                    ClientCredentials cred = new ClientCredentials();
                    cred.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["OnPremiseUserName"];
                    cred.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["OnPremisePassword"];
                    org = new OrganizationServiceProxy(config, cred);

                }
                else
                {
                    AuthenticationCredentials authCredentials = new AuthenticationCredentials();
                    ICredentials credentials = CredentialCache.DefaultCredentials;
                    NetworkCredential credential = credentials.GetCredential(url, "Basic");
                    authCredentials.ClientCredentials.Windows.ClientCredential = credential;
                    org = new OrganizationServiceProxy(config, authCredentials.ClientCredentials);

                }

                org.Timeout = new TimeSpan(0, 30, 0);
                return org;
            }
            catch (Exception ex)
            {

                if (ex.InnerException.ToString().Contains("timeout"))
                {
                    if (i > 10)
                        throw ex;

                    if (i > 5)
                        Console.WriteLine("A timeout occurs during the service instanciation.Retry Count ={0}", i.ToString());
                    Thread.Sleep(1000 * 15);
                    return GetLegacyOrganizationService(ProcessName, i++);
                }
                else
                {
                    throw ex;
                }
            }

        }
    }
}
