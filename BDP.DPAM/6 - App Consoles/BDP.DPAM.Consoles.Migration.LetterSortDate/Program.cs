using BDP.DPAM.Consoles.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Threading;

namespace BDP.DPAM.Consoles.Migration.LetterSortDate
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
                ExecuteTask();
                ExecuteLetter();
                Console.WriteLine("End");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private static void ExecuteLetter()
        {
            Console.WriteLine("Letters");
            IOrganizationService     service = CRMConnector.GetOrganizationServiceClientSecret(
                         System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineUserId"],
                         System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineSecret"],
                         System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineURL"]);

            EntityCollection result_bd_fetchquery = new EntityCollection() { PagingCookie = "" };
            int page = 1;
            int created = 0;
            int duplicate = 0;

            do
            {
                result_bd_fetchquery = service.RetrieveMultiple(new FetchExpression(
                    string.Format(File.ReadAllText("FetchXML\\Request.xml")

                , page,
                    WebUtility.HtmlEncode(result_bd_fetchquery.PagingCookie))));
                page++;


                while (service == null)
                {
                    service = CRMConnector.GetOrganizationServiceClientSecret(
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineUserId"],
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineSecret"],
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineURL"]);
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

                    _new.Id = emr2.Id;
                    if(emr2.Contains("scheduledstart"))
                    _new["sortdate"] = emr2.GetAttributeValue<DateTime>("scheduledstart");
                    if (emr2.Contains("createdon"))
                        _new["sortdate"] = emr2.GetAttributeValue<DateTime>("createdon");


                    requestWithResults.Requests.Add(new UpdateRequest() { Target = _new });
                }

                try
                {

                    ExecuteMultipleResponse resp = service.Execute(requestWithResults) as ExecuteMultipleResponse;
                    if (resp.IsFaulted)
                    {

                        foreach (ExecuteMultipleResponseItem or in resp.Responses)
                        {
                            if (or.Fault != null)
                            {                               
                                    Console.WriteLine(); Console.WriteLine(or.Fault.Message); Console.WriteLine();

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
                        Console.WriteLine(); Console.WriteLine("Error : " + ex.Message); Console.WriteLine();
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error : " + ex.Message);
                    Console.WriteLine();

                }              

            }
            while (result_bd_fetchquery.MoreRecords);
        }

        private static void ExecuteTask()
        {
            Console.WriteLine("Tasks");
            IOrganizationService service = CRMConnector.GetOrganizationServiceClientSecret(
                              System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineUserId"],
                              System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineSecret"],
                              System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineURL"]);

            EntityCollection result_bd_fetchquery = new EntityCollection() { PagingCookie = "" };
            int page = 1;
            int created = 0;
            int duplicate = 0;

            do
            {
                result_bd_fetchquery = service.RetrieveMultiple(new FetchExpression(
                    string.Format(File.ReadAllText("FetchXML\\TaskRequest.xml")

                , page,
                    WebUtility.HtmlEncode(result_bd_fetchquery.PagingCookie))));
                page++;


                while (service == null)
                {
                    service = CRMConnector.GetOrganizationServiceClientSecret(
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineUserId"],
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineSecret"],
                                System.Configuration.ConfigurationManager.AppSettings["DPAM.OnlineURL"]);
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

                    _new.Id = emr2.Id;
                    if (emr2.Contains("scheduledstart"))
                        _new["sortdate"] = emr2.GetAttributeValue<DateTime>("scheduledstart");
                    if (emr2.Contains("createdon"))
                        _new["sortdate"] = emr2.GetAttributeValue<DateTime>("createdon");


                    requestWithResults.Requests.Add(new UpdateRequest() { Target = _new });
                }

                try
                {

                    ExecuteMultipleResponse resp = service.Execute(requestWithResults) as ExecuteMultipleResponse;
                    if (resp.IsFaulted)
                    {

                        foreach (ExecuteMultipleResponseItem or in resp.Responses)
                        {
                            if (or.Fault != null)
                            {
                                Console.WriteLine(); Console.WriteLine(or.Fault.Message); Console.WriteLine();

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
                    Console.WriteLine(); Console.WriteLine("Error : " + ex.Message); Console.WriteLine();

                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error : " + ex.Message);
                    Console.WriteLine();

                }

            }
            while (result_bd_fetchquery.MoreRecords);
        }
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();



    }
}
