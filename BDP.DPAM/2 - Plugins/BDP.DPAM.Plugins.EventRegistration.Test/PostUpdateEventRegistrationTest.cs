using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Cancel related Sessions Registrations on Event Registration cancelation

        [Fact]
        public void CancelRelatedSessionRegistrations_UpdateEventRegistrationAsCanceled_RelatedActiveSessionRegistrationShouldBeCanceled()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();

            // Target Event Registration 
            Entity target = new Entity("msevtmgt_eventregistration");
            target.Id = Guid.NewGuid();
            target["statecode"] = new OptionSetValue(Convert.ToInt32(EventRegistration_StateCode.Inactive));
            target["statuscode"] = new OptionSetValue(Convert.ToInt32(EventRegistration_StatusCode.Canceled));

            // Session Registrations
            List<Entity> sessionRegistrations = new List<Entity>();

            // Related Session Registration Active
            Entity relatedSessionRegistrationActive = new Entity("msevtmgt_sessionregistration");
            relatedSessionRegistrationActive.Id = Guid.NewGuid();
            relatedSessionRegistrationActive["msevtmgt_registrationid"] = target.ToEntityReference();
            relatedSessionRegistrationActive["statecode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StateCode.Active));
            relatedSessionRegistrationActive["statuscode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StatusCode.Attented));
            relatedSessionRegistrationActive["modifiedon"] = new DateTime(2021, 04, 04);
            sessionRegistrations.Add(relatedSessionRegistrationActive);

            // Related Session Registration Inactive
            Entity relatedSessionRegistrationInactive = new Entity("msevtmgt_sessionregistration");
            relatedSessionRegistrationInactive.Id = Guid.NewGuid();
            relatedSessionRegistrationInactive["msevtmgt_registrationid"] = target.ToEntityReference();
            relatedSessionRegistrationInactive["statecode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StateCode.Inactive));
            relatedSessionRegistrationInactive["statuscode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StatusCode.Canceled));
            relatedSessionRegistrationInactive["modifiedon"] = new DateTime(2021, 04, 04);
            sessionRegistrations.Add(relatedSessionRegistrationInactive);

            // Not Related Session Registration Active
            Entity notRelatedSessionRegistrationActive = new Entity("msevtmgt_sessionregistration");
            notRelatedSessionRegistrationActive.Id = Guid.NewGuid();
            notRelatedSessionRegistrationActive["msevtmgt_registrationid"] = new EntityReference(target.LogicalName, Guid.NewGuid());
            notRelatedSessionRegistrationActive["statecode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StateCode.Active));
            notRelatedSessionRegistrationActive["statuscode"] = new OptionSetValue(Convert.ToInt32(SessionRegistration_StatusCode.Attented));
            notRelatedSessionRegistrationActive["modifiedon"] = new DateTime(2021, 04, 04);
            sessionRegistrations.Add(notRelatedSessionRegistrationActive);

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 40,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(sessionRegistrations);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PostUpdateEventRegistration>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            ConditionExpression conditionEventRegistrationId = new ConditionExpression("msevtmgt_registrationid", ConditionOperator.Equal, target.Id);
            ConditionExpression conditionStatusCode = new ConditionExpression("statuscode", ConditionOperator.Equal, Convert.ToInt32(SessionRegistration_StatusCode.Canceled));
            ConditionExpression conditionModifiedOn = new ConditionExpression("modifiedon", ConditionOperator.Today);

            QueryExpression query = new QueryExpression("msevtmgt_sessionregistration");
            query.Criteria.AddCondition(conditionEventRegistrationId);
            query.Criteria.AddCondition(conditionStatusCode);
            query.Criteria.AddCondition(conditionModifiedOn);

            EntityCollection result = fakedService.RetrieveMultiple(query);

            Assert.True(result.Entities.Count == sessionRegistrations.Count(sr =>
                sr.GetAttributeValue<EntityReference>("msevtmgt_registrationid").Id == target.Id &&
                sr.GetAttributeValue<OptionSetValue>("statecode").Value == Convert.ToInt32(SessionRegistration_StateCode.Active)));

            #endregion
        }

        #endregion
    }
}
