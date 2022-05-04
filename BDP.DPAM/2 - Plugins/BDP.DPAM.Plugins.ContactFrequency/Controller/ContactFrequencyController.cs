using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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

        /// <summary>
        /// Throw an exception if another contact frequency already exists with the same counterparty, the same start date, the same end date and the same number of target activities
        /// </summary>
        internal void PotentialDuplicationManagement()
        {
            if (!_target.Contains("dpam_lk_counterparty") && !_target.Contains("dpam_dt_startdate") && !_target.Contains("dpam_dt_enddate") && !_target.Contains("dpam_int_numberoftargetactivities")) return;

            _tracing.Trace("PotentialDuplicationManagement - Start");

            var mergedContactFrequency = _target.MergeEntity(_preImage);
            var counterparty = mergedContactFrequency.GetAttributeValue<EntityReference>("dpam_lk_counterparty");
            var startDate = mergedContactFrequency.GetAttributeValue<DateTime?>("dpam_dt_startdate");
            var endDate = mergedContactFrequency.GetAttributeValue<DateTime?>("dpam_dt_enddate");
            var numberOfTargetActivities = mergedContactFrequency.GetAttributeValue<int>("dpam_int_numberoftargetactivities");

            var contactFrequencyAlreadyExists = CheckContactFrequencyAlreadyExists(counterparty, startDate, endDate, numberOfTargetActivities);

            if(contactFrequencyAlreadyExists)
            {
                throw new Exception("A Contact Frequency for this Counterparty already exists with this Start Date, this End Date and this N° of Target Activities.");
            }

            _tracing.Trace("PotentialDuplicationManagement - End");
        }

        /// <summary>
        /// Check if another contact frequency already exists with the same counterparty, the same start date, the same end date and the same number of target activities
        /// </summary>
        /// <param name="counterparty">the counterparty linked to the contact frequency</param>
        /// <param name="startDate">the start date linked to the contact frequency</param>
        /// <param name="endDate">the end date linked to the contact frequency</param>
        /// <param name="numberOfTargetActivities">the number of target activities linked to the contact frequency</param>
        /// <returns></returns>
        private bool CheckContactFrequencyAlreadyExists(EntityReference counterparty, DateTime? startDate, DateTime? endDate, int numberOfTargetActivities)
        {
            if (counterparty == null || startDate == null || endDate == null) return false;

            _tracing.Trace("CheckContactFrequencyAlreadyExists - Start");

            var contactFrequencyAlreadyExists = false;

            var conditionCounterparty = new ConditionExpression("dpam_lk_counterparty", ConditionOperator.Equal, counterparty.Id);
            var conditionStartDate = new ConditionExpression("dpam_dt_startdate", ConditionOperator.Equal, startDate);
            var conditionEndDate = new ConditionExpression("dpam_dt_enddate", ConditionOperator.Equal, endDate);
            var conditionTarget = new ConditionExpression("dpam_int_numberoftargetactivities", ConditionOperator.Equal, numberOfTargetActivities);

            var query = new QueryExpression("dpam_contactfrequency");
            query.Criteria.AddCondition(conditionCounterparty);
            query.Criteria.AddCondition(conditionStartDate);
            query.Criteria.AddCondition(conditionEndDate);
            query.Criteria.AddCondition(conditionTarget);

            var contactFrequencyCollection = _service.RetrieveMultiple(query);

            if (contactFrequencyCollection != null && contactFrequencyCollection.Entities.Count > 0) contactFrequencyAlreadyExists = true;

            _tracing.Trace("CheckContactFrequencyAlreadyExists - End");
            return contactFrequencyAlreadyExists;
        }
    }
}
