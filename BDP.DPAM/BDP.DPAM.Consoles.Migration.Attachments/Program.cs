using BDP.DPAM.Consoles.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Threading;

namespace BDP.DPAM.Consoles.Migration.Attachments
{
    class Program
    {
        static void Main(string[] args)
        {
            AllocConsole();
            //To run console application
            Console.WriteLine("Microsoft Dynamics CRM Integration task started...", Environment.CommandLine);
            Console.WriteLine("Please do not stop or close this window!");

            try
            {
                Execute();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private static void Execute()
        {
            IOrganizationService service = CRMConnector.GetLegacyOrganizationService("Get Action Configurator");

            EntityCollection result_bd_fetchquery = new EntityCollection() { PagingCookie = "" };
            int page = 1;
            int created = 0;
            int duplicate = 0;
            string file = File.ReadAllText("Paging\\paging.txt");
            page = int.Parse(file.Split(";".ToCharArray())[0]);
            result_bd_fetchquery.PagingCookie = file.Split(";".ToCharArray())[1];

            if (file.Split(";".ToCharArray()).Length > 2)
            {

                string isdone = file.Split(";".ToCharArray())[2];

                if (isdone == "done")
                {

                    Console.WriteLine("all is treated");
                    Console.ReadLine();
                    return;
                }
            }

            do
            {
                result_bd_fetchquery = service.RetrieveMultiple(new FetchExpression(
                    string.Format(File.ReadAllText("FetchXML\\Request.xml")

                , page,
                    WebUtility.HtmlEncode(result_bd_fetchquery.PagingCookie))));
                page++;
                IOrganizationService localConn = null;

                while (localConn == null)
                {
                    localConn = CRMConnector.GetOrganizationServiceClientSecret(
                                System.Configuration.ConfigurationManager.AppSettings["TargetUserId"],
                                System.Configuration.ConfigurationManager.AppSettings["TargetSecret"],
                                System.Configuration.ConfigurationManager.AppSettings["TargetURL"]);
                    Thread.Sleep(1000);
                }

                ExecuteMultipleRequest requestWithResults
                    = new ExecuteMultipleRequest()
                    {
                        // Assign settings that define execution behavior: continue on error, return responses. 
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = true,
                            ReturnResponses = true
                        },
                        // Create an empty organization request collection.
                        Requests = new OrganizationRequestCollection()
                    };


                foreach (Entity emr2 in result_bd_fetchquery.Entities)
                {
                    Entity _new = new Entity(emr2.LogicalName);

                    _new.Id = emr2.GetAttributeValue<EntityReference>("attachmentid").Id;
                    _new["filename"] = emr2.GetAttributeValue<string>("filename");
                    _new["subject"] = emr2.GetAttributeValue<string>("subject");
                    _new["objectid"] = emr2["objectid"];
                    _new["body"] = emr2["body"];
                    _new["mimetype"] = emr2.GetAttributeValue<string>("mimetype");
                    _new["objecttypecode"] = emr2["objecttypecode"];


                    requestWithResults.Requests.Add(new CreateRequest() { Target = _new });
                }

                try
                {

                    ExecuteMultipleResponse resp = localConn.Execute(requestWithResults) as ExecuteMultipleResponse;
                    if (resp.IsFaulted)
                    {

                        foreach (ExecuteMultipleResponseItem or in resp.Responses)
                        {
                            if (or.Fault != null)
                            {
                                if (or.Fault.Message != "Cannot insert duplicate key." && !or.Fault.Message.StartsWith("Cannot insert duplicate key"))
                                {
                                    Console.WriteLine(); Console.WriteLine(or.Fault.Message); Console.WriteLine();
                                }
                                else
                                {

                                    Console.Write($"\rTreatment in progress        " + $"" + $":{created }/{created + duplicate++}(duplicate:{duplicate}) Page{ page.ToString()}");
                                }



                            }
                            else
                            {

                                Console.Write($"\rTreatment in progress        " + $"" + $":{created++}/{created + duplicate}(duplicate:{duplicate}) Page{ page.ToString()}");
                            }

                        }

                    }

                }
                catch (FaultException ex)
                {

                    if (ex.Message != "Cannot insert duplicate key." && !ex.Message.StartsWith("Cannot insert duplicate key"))
                    {

                        Console.WriteLine(); Console.WriteLine("Error : " + ex.Message); Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error : " + ex.Message);
                    Console.WriteLine();

                }

                if (result_bd_fetchquery.MoreRecords)
                {
                    File.WriteAllText("Paging\\paging.txt", page.ToString() + ";" + result_bd_fetchquery.PagingCookie);
                }
                else
                {
                    File.WriteAllText("Paging\\paging.txt", page.ToString() + ";;done");
                }

            }
            while (result_bd_fetchquery.MoreRecords);
        }
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();



    }
}
