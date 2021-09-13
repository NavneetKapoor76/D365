using System;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;


namespace BDP.DPAM.Plugins.Account.Controller
{
    class AccountController : PluginManagerBase
    {

        internal AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        internal void CompleteSegmentation()
        {
            if (!this._target.Contains("dpam_lk_localbusinesssegmentation") || _context.MessageName != MessageName.Update)
                return;
            this._tracing.Trace("CompleteSegmentation on Account - Start");
            if (this._target["dpam_lk_localbusinesssegmentation"] != null)
            {
                Entity localSegmentationEntity = _service.Retrieve(_target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation"), "dpam_lk_businesssegmentation");

                this._target["dpam_lk_businesssegmentation"] = localSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation");
            }
            else
            {
                this._target["dpam_lk_businesssegmentation"] = null;

            }
            this._tracing.Trace("CompleteSegmentation on Account - End");
        }
    }
}
