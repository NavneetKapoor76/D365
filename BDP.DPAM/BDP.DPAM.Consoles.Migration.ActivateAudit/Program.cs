using BDP.DPAM.Consoles.Shared;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Consoles.Migration.ActivateAudit
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] list = new string[] { "dpam_country",
                "dpam_bpfcustomerjourney",
                "dpam_contactfrequency",
                "dpam_contacttitle",
                "dpam_country",
                "dpam_counterpartybusinesssegmentation",
                "dpam_counterpartycompliancesegmentation",
                "dpam_counterpartylegalform",
                "dpam_cplocalbusinesssegmentation",
                "dpam_counterpartymifidcategory",
                "dpam_departments",
                "dpam_greeting",

                "dpam_jobfunction","dpam_leadsource",
                "dpam_location",
                "dpam_productinterest",
                "dpam_settings",
                "dpam_shareclass",


            };
            IOrganizationService localConn = CRMConnector.GetOrganizationServiceClientSecret(
System.Configuration.ConfigurationManager.AppSettings["TargetUserId"],
System.Configuration.ConfigurationManager.AppSettings["TargetSecret"],
System.Configuration.ConfigurationManager.AppSettings["TargetURL"]);

            foreach (string name in list) { 

            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity,
                LogicalName = name
            };
            RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)localConn.Execute(retrieveEntityRequest);
            EntityMetadata eMetaData = retrieveAccountEntityResponse.EntityMetadata;

            eMetaData.IsAuditEnabled.Value = true;


                UpdateEntityRequest updateEntityRequest = new UpdateEntityRequest();
                updateEntityRequest.Entity = eMetaData;
                UpdateEntityResponse result = localConn.Execute(updateEntityRequest) as UpdateEntityResponse;
                Console.WriteLine(name + " is activated");

            }

    


        }
    }
}
