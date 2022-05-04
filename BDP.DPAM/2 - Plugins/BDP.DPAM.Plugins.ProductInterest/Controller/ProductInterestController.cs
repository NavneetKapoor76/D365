using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.ProductInterest
{
    internal class ProductInterestController : PluginManagerBase
    {
        internal ProductInterestController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Set the default name of the product interest
        /// Format: contact fullname - product name - share class name
        /// </summary>
        internal void SetDefaultName()
        {
            if (!_target.Contains("dpam_lk_contact") && !_target.Contains("dpam_lk_product") && !_target.Contains("dpam_lk_shareclass")) return;

            _tracing.Trace("SetDefaultName - Start");

            var contactFrequencyMerged = _target.MergeEntity(_preImage);

            var contactName = CommonLibrary.GetRecordName(_service, contactFrequencyMerged.GetAttributeValue<EntityReference>("dpam_lk_contact"), "fullname");
            var productName = CommonLibrary.GetRecordName(_service, contactFrequencyMerged.GetAttributeValue<EntityReference>("dpam_lk_product"), "name");
            var shareClassName = CommonLibrary.GetRecordName(_service, contactFrequencyMerged.GetAttributeValue<EntityReference>("dpam_lk_shareclass"), "dpam_s_shareclass");

            _target["dpam_name"] = $"{contactName} - {productName} - {shareClassName}";

            _tracing.Trace("SetDefaultName - End");
        }
    }
}
