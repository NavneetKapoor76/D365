using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDP.DPAM.Plugins.EventRegistration
{
    internal class EventRegistrationController : PluginManagerBase
    {
        internal EventRegistrationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Deactivate the registration responses when the event registration is deactivated
        /// </summary>
        internal void DeactivateRegistrationResponses()
        {
            if (!_target.Contains("statecode") || _target.GetAttributeValue<OptionSetValue>("statecode")?.Value != (int)EventRegistration_StateCode.Inactive) return;

            _tracing.Trace("DeactivateRegistrationResponses - Start");

            var eventRegistrationCondition = new ConditionExpression("msevtmgt_eventregistration", ConditionOperator.Equal, _target.Id);

            var query = new QueryExpression("msevtmgt_registrationresponse")
            {
                ColumnSet = new ColumnSet("msevtmgt_registrationresponseid")
            };
            query.Criteria.AddCondition(eventRegistrationCondition);

            var registrationResponseCollection = _service.RetrieveMultiple(query);

            if (registrationResponseCollection == null || registrationResponseCollection.Entities.Count < 1)
            {
                _tracing.Trace("DeactivateRegistrationResponses - End");
                return;
            }

            foreach (var registrationResponse in registrationResponseCollection.Entities)
            {
                registrationResponse["statecode"] = new OptionSetValue((int)RegistrationResponse_StateCode.Inactive);
                registrationResponse["statuscode"] = new OptionSetValue((int)RegistrationResponse_StatusCode.Inactive);
                _service.Update(registrationResponse);
            }

            _tracing.Trace("DeactivateRegistrationResponses - End");
        }

        /// <summary>
        /// Update the related Sessions Registrations as Canceled on Event Registration Deactivation 
        /// </summary>
        internal void CancelRelatedSessionRegistrations()
        {
            if (!this._target.Contains("statecode") || 
                this._target.GetAttributeValue<OptionSetValue>("statecode").Value != Convert.ToInt32(EventRegistration_StateCode.Inactive))
                return;

            this._tracing.Trace("CancelRelatedSessionRegistrationOnRegistrationCancelation - Start");

            EntityCollection relatedActiveSessionRegistrations = this.GetRelatedActiveSessionRegistrations(this._target.Id);

            foreach (Entity sessionRegistration in relatedActiveSessionRegistrations.Entities)
            {
                Entity sessionRegistrationToUpdate = new Entity(sessionRegistration.LogicalName);
                sessionRegistrationToUpdate.Id = sessionRegistration.Id;
                sessionRegistrationToUpdate["statecode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StateCode.Inactive));
                sessionRegistrationToUpdate["statuscode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StatusCode.Canceled));

                this._service.Update(sessionRegistrationToUpdate);
            }

            this._tracing.Trace("CancelRelatedSessionRegistrationOnRegistrationCancelation - End");
        }

        /// <summary>
        /// Retrieve the related Active Session Registration
        /// </summary>
        /// <param name="eventRegistrationId"></param>
        /// <returns>EntityCollection of Session Registration</returns>
        private EntityCollection GetRelatedActiveSessionRegistrations(Guid eventRegistrationId)
        {
            this._tracing.Trace("GetRelatedActiveSessionRegistrations - Start");

            EntityCollection retVal = null;

            ConditionExpression conditionEventRegistrationId = new ConditionExpression("msevtmgt_registrationid", ConditionOperator.Equal, eventRegistrationId);
            ConditionExpression conditionStateCode = new ConditionExpression("statecode", ConditionOperator.Equal, Convert.ToInt32(SessionRegistration_StateCode.Active));

            QueryExpression query = new QueryExpression("msevtmgt_sessionregistration");
            query.Criteria.AddCondition(conditionEventRegistrationId);
            query.Criteria.AddCondition(conditionStateCode);

            retVal = this._service.RetrieveMultiple(query);

            this._tracing.Trace("GetRelatedActiveSessionRegistrations - End");

            return retVal;
        }
    }
}
