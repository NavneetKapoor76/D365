using System;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;


namespace BDP.DPAM.Plugins.Account.Controller
{
    internal class AccountController : PluginManagerBase
    {

        internal AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Fill in the field dpam_lk_businesssegmentation
        /// </summary>
        internal void CompleteSegmentation()
        {
            if (!_target.Contains("dpam_lk_localbusinesssegmentation"))
                return;

            _tracing.Trace("CompleteSegmentation - Start");

            if (_target["dpam_lk_localbusinesssegmentation"] != null)
            {
                Entity localSegmentationEntity = _service.Retrieve(_target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation"), "dpam_lk_businesssegmentation");

                _target["dpam_lk_businesssegmentation"] = localSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation");
            }
            else
            {
                _target["dpam_lk_businesssegmentation"] = null;

            }

            _tracing.Trace("CompleteSegmentation - End");
        }

        /// <summary>
        /// If the country of the counterparty has changed to a different one, then we need to empty the fields "dpam_lk_localbusinesssegmentation" & "dpam_lk_businesssegmentation".
        /// </summary>
        internal void CheckLocalAndBusinessSegmentationCountry()
        {
            if (!_target.Contains("dpam_lk_country"))
                return;

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - Start");

            if (_target["dpam_lk_country"] == null || _preImage["dpam_lk_country"] == null || 
                    (_target.GetAttributeValue<EntityReference>("dpam_lk_country") != _preImage.GetAttributeValue<EntityReference>("dpam_lk_country")))
            {
                _target["dpam_lk_localbusinesssegmentation"] = null;
                _target["dpam_lk_businesssegmentation"] = null;
            }

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - End");
        }
    }
}
