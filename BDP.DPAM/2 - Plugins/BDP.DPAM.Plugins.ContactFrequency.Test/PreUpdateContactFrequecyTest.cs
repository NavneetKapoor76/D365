using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.ContactFrequency.Test
{
    public class PreUpdateContactFrequecyTest
    {
        [Fact]
        public void UpdateNumberOfRemainingActivitiesValue_When_Target_Is_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 6 }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 2 },
                    {"dpam_int_numberofcompletedactivities", 2 }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", contactFrequencyPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            Assert.False(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));

            fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);

            Assert.True(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));
            Assert.Equal(4, contactFrequencyTarget.GetAttributeValue<int>("dpam_int_numberofremaingactivities"));
        }

        [Fact]
        public void UpdateNumberOfRemainingActivitiesValue_When_Completed_Activities_Is_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberofcompletedactivities", 3 }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 4 },
                    {"dpam_int_numberofcompletedactivities", 2 }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", contactFrequencyPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            Assert.False(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));

            fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);

            Assert.True(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));
            Assert.Equal(1, contactFrequencyTarget.GetAttributeValue<int>("dpam_int_numberofremaingactivities"));
        }

        [Fact]
        public void UpdateNumberOfRemainingActivitiesValue_When_Completed_Activities_And_Target_Are_Not_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid()
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 4 },
                    {"dpam_int_numberofcompletedactivities", 2 }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", contactFrequencyPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            Assert.False(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));

            fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);

            Assert.False(contactFrequencyTarget.Contains("dpam_int_numberofremaingactivities"));
        }

        [Fact]
        public void SetDefaultName_Name_Is_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var counterpartyName = "Counterparty";
            var startDate = new DateTime(2021, 10, 1);
            var endDate = new DateTime(2021, 11, 30);

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", counterpartyName}
                }
            };

            fakeContext.Initialize(counterparty);

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", startDate }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = contactFrequencyTarget.Id,
                Attributes =
                {
                    {"dpam_dt_enddate", endDate }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", contactFrequencyPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);

            Assert.True(contactFrequencyTarget.Contains("dpam_s_name"));
            Assert.Equal($"{counterpartyName} - {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}", contactFrequencyTarget.GetAttributeValue<string>("dpam_s_name"));
        }

        [Fact]
        public void SetDefaultName_Name_Is_Not_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var counterpartyName = "Counterparty";
            var startDate = new DateTime(2021, 10, 1);
            var endDate = new DateTime(2021, 11, 30);

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", counterpartyName}
                }
            };

            fakeContext.Initialize(counterparty);

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 10 }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = contactFrequencyTarget.Id,
                Attributes =
                {

                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", startDate },
                    {"dpam_dt_enddate", endDate }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", contactFrequencyPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);

            Assert.False(contactFrequencyTarget.Contains("dpam_s_name"));
        }

        [Theory]
        [InlineData("01/11/2021", "01/11/2021")]
        [InlineData("01/11/2021", "05/11/2021")]
        public void PotentialDuplicationManagement(string existingContactFrequencyDate, string targetDate)
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var existingContactFrequencyStartDate = DateTime.ParseExact(existingContactFrequencyDate, "dd/MM/yyyy", null);
            var targetContactFrequencyStartDate = DateTime.ParseExact(targetDate, "dd/MM/yyyy", null);

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid()
            };
            entityList.Add(counterparty);

            var existingContactFrequency = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", existingContactFrequencyStartDate },
                    {"dpam_dt_enddate", new DateTime(2021, 11, 30) },
                    {"dpam_int_numberoftargetactivities", 20 }
                }
            };
            entityList.Add(existingContactFrequency);

            fakeContext.Initialize(entityList);

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_dt_startdate", targetContactFrequencyStartDate },
                    {"dpam_int_numberoftargetactivities", 20 }
                }
            };

            var contactFrequencyPreImage = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", new DateTime(2021, 11, 1) },
                    {"dpam_dt_enddate", new DateTime(2021, 11, 30) },
                    {"dpam_int_numberoftargetactivities", 10 }
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection { { "Target", contactFrequencyTarget } },
                PreEntityImages = new EntityImageCollection { { "PreImage", contactFrequencyPreImage } },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            Exception ex = null;

            try
            {
                fakeContext.ExecutePluginWith<PreUpdateContactFrequency>(executionFakeContext);
            }
            catch (Exception e)
            {
                ex = e;
            }

            if (existingContactFrequencyDate == targetDate) Assert.NotNull(ex);
            else Assert.Null(ex);

        }
    }
}
