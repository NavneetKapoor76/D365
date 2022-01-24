using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace BDP.DPAM.Plugins.DefaultUserSettings
{
    internal class DefaultUserSettingsController : PluginManagerBase
    {
        internal DefaultUserSettingsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Adapt the user Settings
        /// </summary>
        internal void Update()
        {
            if (!_target.Contains("dpam_lk_localbusinesssegmentation")) return;

            _tracing.Trace("FillInBusinessSegmentation - Start");

            Entity e_usersettings = RetrieveUserSettingsId();


            e_usersettings["autocreatecontactonpromote"] = null;

            _service.Update(e_usersettings);

            _tracing.Trace("FillInBusinessSegmentation - End");
        }

        private Entity RetrieveUserSettingsId()
        {
            QueryByAttribute qe_userSettings = new QueryByAttribute("usersettings");
            qe_userSettings.ColumnSet = new ColumnSet("autocreatecontactonpromote", "address1_city", "emailaddress1");
            qe_userSettings.Attributes.AddRange("systemuser");
            qe_userSettings.Values.AddRange(_target.Id);
            return _service.RetrieveMultiple(qe_userSettings).Entities[0];
        }
    }
}
