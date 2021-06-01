using System;
using Microsoft.Xrm.Sdk;
using BDP.DPAM.Shared.Manager_Base;


namespace BDP.DPAM.CAPI.Contact
{
 
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ContactController ctrl = new ContactController(serviceProvider);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
