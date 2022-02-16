using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace BDP.DPAM.Plugins.MarketingEmailTemplate.Test
{
    public class PreCreateMarketingEmailTest
    {
        [Fact]
        public void DefaultFromFields_Default()
        {
            XrmFakedContext fakeContext = new XrmFakedContext();



            Guid msdyncrm_marketingemailtemplate_Guid = Guid.NewGuid();
            Entity msdyncrm_marketingemailtemplate_Target = new Entity("msdyncrm_marketingemailtemplate")
            {
                Id = msdyncrm_marketingemailtemplate_Guid,
                Attributes =
                {
                    {"ownerid", new  EntityReference("systemuser", Guid.NewGuid())}

                }
            };

            ParameterCollection inputParameters = new ParameterCollection
            {
                {"Target", msdyncrm_marketingemailtemplate_Target }
            };

            XrmFakedPluginExecutionContext executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateMarketingEmailTemplate>(executionFakeContext);

            Assert.Equal("DPAM@dpaminvestments.com", msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromemail"));
            Assert.Equal("DPAM", msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromname"));
        }

        [Fact]
        public void DefaultFromFields_NotDefault()
        {
            XrmFakedContext fakeContext = new XrmFakedContext();


            Guid msdyncrm_marketingemailtemplate_Guid = Guid.NewGuid();
            string msdyncrm_fromname = "TEST";
            string msdyncrm_fromemail = "t@email.be";
            Entity msdyncrm_marketingemailtemplate_Target = new Entity("msdyncrm_marketingemailtemplate")
            {
                Id = msdyncrm_marketingemailtemplate_Guid,
                Attributes =
                {
                    {"ownerid", new  EntityReference("systemuser", Guid.NewGuid())},
                           {"msdyncrm_fromname",msdyncrm_fromname},
                           {"msdyncrm_fromemail", msdyncrm_fromemail}

                }
            };

            ParameterCollection inputParameters = new ParameterCollection
            {
                {"Target", msdyncrm_marketingemailtemplate_Target }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateMarketingEmailTemplate>(executionFakeContext);

            Assert.NotEqual("DPAM@dpaminvestments.com", msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromemail"));
            Assert.NotEqual("DPAM", msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromname"));
            Assert.Equal(msdyncrm_fromemail, msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromemail"));
            Assert.Equal(msdyncrm_fromname, msdyncrm_marketingemailtemplate_Target.GetAttributeValue<string>("msdyncrm_fromname"));
        }
    }
}
