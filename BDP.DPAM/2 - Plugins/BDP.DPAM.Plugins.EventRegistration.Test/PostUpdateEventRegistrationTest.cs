using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.EventRegistration.Test
{
    public class PostUpdateEventRegistrationTest
    {
        [Theory]
        [InlineData((int)EventRegistration_StateCode.Active, (int)RegistrationResponse_StateCode.Active, (int)RegistrationResponse_StatusCode.Active)]
        [InlineData((int)EventRegistration_StateCode.Inactive, (int)RegistrationResponse_StateCode.Inactive, (int)RegistrationResponse_StatusCode.Inactive)]
        public void Registration_Responses_Are_Deactivated_Or_Not_Based_On_Event_Registration_StateCode(int eventRegistrationStateCode, int expectedRegistrationResponseStateCode, int expectedRegistrationResponseStatusCode)
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var eventRegistrationTarget = new Entity("msevtmgt_eventregistration")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"statecode", new OptionSetValue(eventRegistrationStateCode) }
                }
            };

            var registrationResponseOne = new Entity("msevtmgt_registrationresponse")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"msevtmgt_eventregistration", eventRegistrationTarget.ToEntityReference()},
                    {"statecode", new OptionSetValue((int)RegistrationResponse_StateCode.Active) },
                    {"statuscode", new OptionSetValue((int)RegistrationResponse_StatusCode.Active) }
                }
            };
            entityList.Add(registrationResponseOne);

            var registrationResponseTwo = new Entity("msevtmgt_registrationresponse")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"msevtmgt_eventregistration", eventRegistrationTarget.ToEntityReference()},
                    {"statecode", new OptionSetValue((int)RegistrationResponse_StateCode.Active) },
                    {"statuscode", new OptionSetValue((int)RegistrationResponse_StatusCode.Active) }
                }
            };
            entityList.Add(registrationResponseTwo);

            fakeContext.Initialize(entityList);

            var inputParameters = new ParameterCollection
            {
                {"Target", eventRegistrationTarget }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdateEventRegistration>(executionFakeContext);

            var resultRegistrationResponses = fakeContext.CreateQuery("msevtmgt_registrationresponse");
            foreach (var registrationResponse in resultRegistrationResponses)
            {
                Assert.Equal(expectedRegistrationResponseStateCode, registrationResponse.GetAttributeValue<OptionSetValue>("statecode")?.Value);
                Assert.Equal(expectedRegistrationResponseStatusCode, registrationResponse.GetAttributeValue<OptionSetValue>("statuscode")?.Value);
            }
        }
    }
}
