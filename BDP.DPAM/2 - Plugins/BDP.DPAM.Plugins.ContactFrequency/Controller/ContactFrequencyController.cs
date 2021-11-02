using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Manager_Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.ContactFrequency
{
    internal class ContactFrequencyController : PluginManagerBase
    {
        internal ContactFrequencyController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Add the "dpam_int_numberofremaingactivities" field in the target when the "dpam_int_numberoftargetactivities" field or the "dpam_int_numberofcompletedactivities" field is updated
        /// </summary>
        internal void UpdateNumberOfRemainingActivitiesValue()
        {
            if (!_target.Contains("dpam_int_numberoftargetactivities") && !_target.Contains("dpam_int_numberofcompletedactivities")) return;

            _tracing.Trace("UpdateNumberOfRemainingActivitiesValue - Start");

            var contactFrequencyMerged = _target.MergeEntity(_preImage);
            var numberOfRemainingActivities = contactFrequencyMerged.GetAttributeValue<int>("dpam_int_numberoftargetactivities") - contactFrequencyMerged.GetAttributeValue<int>("dpam_int_numberofcompletedactivities");

            _target["dpam_int_numberofremaingactivities"] = numberOfRemainingActivities;

            _tracing.Trace("UpdateNumberOfRemainingActivitiesValue - End");
        }
    }
}
