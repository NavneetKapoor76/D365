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
    }
}
