using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
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

        /// <summary>
        /// Set the default name of the contact frequency
        /// Format: counterparty name - start date - end date
        /// </summary>
        internal void SetDefaultName()
        {
            if (!_target.Contains("dpam_lk_counterparty") && !_target.Contains("dpam_dt_startdate") && !_target.Contains("dpam_dt_enddate")) return;

            _tracing.Trace("SetDefaultName - Start");

            var contactFrequencyMerged = _target.MergeEntity(_preImage);

            var counterpartyName = CommonLibrary.GetRecordName(_service, contactFrequencyMerged.GetAttributeValue<EntityReference>("dpam_lk_counterparty"), "name");
            var startDate = contactFrequencyMerged.GetAttributeValue<DateTime?>("dpam_dt_startdate")?.ToString("dd/MM/yyyy");
            var endDate = contactFrequencyMerged.GetAttributeValue<DateTime?>("dpam_dt_enddate")?.ToString("dd/MM/yyyy");

            _target["dpam_s_name"] = $"{counterpartyName} - {startDate} - {endDate}";

            _tracing.Trace("SetDefaultName - End");
        }
    }
}
