using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.Appointment
{
    internal class AppointmentController : PluginManagerBase
    {
        internal AppointmentController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Update the "dpam_int_numberofcompletedactivities" field on Contact Frequency when the statuscode is equal to completed
        /// </summary>
        internal void UpdateNumberOfCompletedActivitiesOnContactFrequency()
        {
            if (!_target.Contains("statuscode")
                || _target.GetAttributeValue<OptionSetValue>("statuscode").Value != (int)Appointment_StatusCode.Completed) return;

            _tracing.Trace("UpdateNumberOfCompletedActivitiesOnContactFrequency - Start");
            
            var regarding = _postImage.Contains("regardingobjectid") ? _postImage.GetAttributeValue<EntityReference>("regardingobjectid") : null;

            if (regarding == null)
            {
                _tracing.Trace("UpdateNumberOfCompletedActivitiesOnContactFrequency - End");
                return;
            }

            EntityReference counterparty = null;

            switch(regarding.LogicalName)
            {
                case "account":
                    counterparty = regarding;
                    break;
                case "contact":
                    var contact = _service.Retrieve(regarding.LogicalName, regarding.Id, new ColumnSet("parentcustomerid"));
                    counterparty = contact.GetAttributeValue<EntityReference>("parentcustomerid");
                    break;
                case "opportunity":
                    var opportunity = _service.Retrieve(regarding.LogicalName, regarding.Id, new ColumnSet("parentaccountid"));
                    counterparty = opportunity.GetAttributeValue<EntityReference>("parentaccountid");
                    break;
            }

            if (counterparty == null)
            {
                _tracing.Trace("UpdateNumberOfCompletedActivitiesOnContactFrequency - End");
                return;
            }

            var counterpartyCondition = new ConditionExpression("dpam_lk_counterparty", ConditionOperator.Equal, counterparty.Id);
            var startDateCondition = new ConditionExpression("dpam_dt_startdate", ConditionOperator.LessEqual, _postImage.GetAttributeValue<DateTime>("scheduledstart"));
            var endDateCondition = new ConditionExpression("dpam_dt_enddate", ConditionOperator.GreaterEqual, _postImage.GetAttributeValue<DateTime>("scheduledstart"));

            var query = new QueryExpression("dpam_contactfrequency")
            {
                ColumnSet = new ColumnSet("dpam_int_numberofcompletedactivities")
            };
            query.Criteria.AddCondition(counterpartyCondition);
            query.Criteria.AddCondition(startDateCondition);
            query.Criteria.AddCondition(endDateCondition);

            var contactFrequencyCollection = _service.RetrieveMultiple(query);

            if (contactFrequencyCollection.Entities.Count < 1)
            {
                _tracing.Trace("UpdateNumberOfCompletedActivitiesOnContactFrequency - End");
                return;
            }

            foreach (var contactFrequency in contactFrequencyCollection.Entities)
            {
                var numberOfCompletedActivities = contactFrequency.GetAttributeValue<int>("dpam_int_numberofcompletedactivities") + 1;

                contactFrequency["dpam_int_numberofcompletedactivities"] = numberOfCompletedActivities;
                _service.Update(contactFrequency);
            }

            _tracing.Trace("UpdateNumberOfCompletedActivitiesOnContactFrequency - End");
        }

        /// <summary>
        /// Manage the Sort Date Column
        /// </summary>
        internal void ManageSortDateColumn()
        {
            _tracing.Trace("ManageSortDateColumn - Start");

            CommonLibrary.ManageSortDateColumnOnActivity(_target, "scheduledstart");

            _tracing.Trace("ManageSortDateColumn - End");
        }
    }
}
