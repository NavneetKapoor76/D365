using BDP.DPAM.Shared.Manager_Base;
using System;

namespace BDP.DPAM.Plugins.MarketingEmailTemplate
{
    internal class MarketingEmailTemplateController : PluginManagerBase
    {
        internal MarketingEmailTemplateController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        /// <summary>
        /// Set the Default From Fields from the Marketing Email Template
        /// </summary>
        internal void DefaultFromFields()
        {
            _tracing.Trace("Default From Fields - Start");

            if (string.IsNullOrEmpty(_target.GetAttributeValue<string>("msdyncrm_fromemail")))
                _target["msdyncrm_fromemail"] = "DPAM@dpaminvestments.com";

            if (string.IsNullOrEmpty(_target.GetAttributeValue<string>("msdyncrm_fromname")))
                _target["msdyncrm_fromname"] = "DPAM";

            _tracing.Trace("Default From Fields - End");
        }
    }
}