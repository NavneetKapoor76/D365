﻿using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.PhoneCall.Test
{
    public class PostUpdatePhoneCallTest
    {
        [Theory]
        [InlineData((int)PhoneCall_StatusCode.Made)]
        [InlineData((int)PhoneCall_StatusCode.Received)]
        public void UpdateNumberOfCompletedActivitiesOnContactFrequency_When_PhoneCall_Is_Completed(int statusCode)
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),

            };
            entityList.Add(counterparty);

            var contactFrequency = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberofcompletedactivities", 0 },
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", new DateTime(2021,10,18) },
                    {"dpam_dt_enddate", new DateTime(2021,11,1) }
                }
            };
            entityList.Add(contactFrequency);

            var contactFrequencyNotUpdate = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberofcompletedactivities", 0 },
                    {"dpam_lk_counterparty", new EntityReference("account", Guid.NewGuid()) },
                    {"dpam_dt_startdate", new DateTime(2021,10,18) },
                    {"dpam_dt_enddate", new DateTime(2021,11,1) }
                }
            };
            entityList.Add(contactFrequencyNotUpdate);

            fakeContext.Initialize(entityList);

            var phoneCallId = Guid.NewGuid();
            var phoneCallTarget = new Entity("phonecall")
            {
                Id = phoneCallId,
                Attributes =
                {
                    {"statuscode", new OptionSetValue(statusCode) }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", phoneCallTarget }
            };

            var phoneCallPostImage = new Entity("phonecall")
            {
                Id = phoneCallId,
                Attributes =
                {
                    { "statuscode", new OptionSetValue(statusCode) },
                    {"regardingobjectid", counterparty.ToEntityReference() },
                    {"scheduledend", new DateTime(2021,10,22) }
                }
            };

            var postEntityImages = new EntityImageCollection
            {
                {"PostImage", phoneCallPostImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = postEntityImages,
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdatePhoneCall>(executionFakeContext);

            var resultContactFrequency = fakeContext.CreateQuery("dpam_contactfrequency");
            foreach(var entity in resultContactFrequency)
            {
                if (entity.Id == contactFrequency.Id)
                {
                    Assert.Equal(1, entity.GetAttributeValue<int>("dpam_int_numberofcompletedactivities"));
                }
                else if(entity.Id == contactFrequencyNotUpdate.Id)
                {
                    Assert.Equal(0, contactFrequencyNotUpdate.GetAttributeValue<int>("dpam_int_numberofcompletedactivities"));
                }
            }
        }

        [Fact]
        public void UpdateNumberOfCompletedActivitiesOnContactFrequency_When_PhoneCall_Is_Not_Completed()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),

            };
            entityList.Add(counterparty);

            var contactFrequency = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberofcompletedactivities", 0 },
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", new DateTime(2021,10,18) },
                    {"dpam_dt_enddate", new DateTime(2021,11,1) }
                }
            };
            entityList.Add(contactFrequency);

            var contactFrequencyNotUpdate = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberofcompletedactivities", 0 },
                    {"dpam_lk_counterparty", new EntityReference("account", Guid.NewGuid()) },
                    {"dpam_dt_startdate", new DateTime(2021,10,18) },
                    {"dpam_dt_enddate", new DateTime(2021,11,1) }
                }
            };
            entityList.Add(contactFrequencyNotUpdate);

            fakeContext.Initialize(entityList);

            var phoneCallId = Guid.NewGuid();
            var phoneCallTarget = new Entity("phonecall")
            {
                Id = phoneCallId,
                Attributes =
                {
                    { "statuscode", new OptionSetValue((int)PhoneCall_StatusCode.Open) }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", phoneCallTarget }
            };

            var phoneCallPostImage = new Entity("phonecall")
            {
                Id = phoneCallId,
                Attributes =
                {
                    { "statuscode", new OptionSetValue((int)PhoneCall_StatusCode.Open) },
                    {"regardingobjectid", counterparty.ToEntityReference() },
                    {"scheduledend", new DateTime(2021,10,22) }
                }
            };

            var postEntityImages = new EntityImageCollection
            {
                {"PostImage", phoneCallPostImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = postEntityImages,
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdatePhoneCall>(executionFakeContext);

            var resultContactFrequency = fakeContext.CreateQuery("dpam_contactfrequency");
            foreach (var entity in resultContactFrequency)
            {
                if (entity.Id == contactFrequency.Id)
                {
                    Assert.Equal(0, entity.GetAttributeValue<int>("dpam_int_numberofcompletedactivities"));
                }
                else if (entity.Id == contactFrequencyNotUpdate.Id)
                {
                    Assert.Equal(0, contactFrequencyNotUpdate.GetAttributeValue<int>("dpam_int_numberofcompletedactivities"));
                }
            }
        }
    }
}
