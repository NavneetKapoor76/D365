using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.ProductInterest.Test
{
    public class PreCreateProductInterestTest
    {
        [Fact]
        public void SetDefaultName_Create_With_Name()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var contactName = "Contact Name";
            var productName = "Product A";
            var shareClassName = "A";

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"fullname", contactName}
                }
            };
            entityList.Add(contact);

            var product = new Entity("product")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", productName}
                }
            };
            entityList.Add(product);

            var shareClass = new Entity("dpam_shareclass")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_shareclass", shareClassName}
                }
            };
            entityList.Add(shareClass);

            fakeContext.Initialize(entityList);

            var productInterestTarget = new Entity("dpam_productinterest")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_contact", contact.ToEntityReference() },
                    {"dpam_lk_product", product.ToEntityReference() },
                    {"dpam_lk_shareclass", shareClass.ToEntityReference() }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", productInterestTarget }
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

            fakeContext.ExecutePluginWith<PreCreateProductInterest>(executionFakeContext);

            Assert.True(productInterestTarget.Contains("dpam_name"));
            Assert.Equal($"{contactName} - {productName} - {shareClassName}", productInterestTarget.GetAttributeValue<string>("dpam_name"));
        }

        [Fact]
        public void SetDefaultName_Create_Whitout_Name()
        {
            var fakeContext = new XrmFakedContext();

            var productInterestTarget = new Entity("dpam_productinterest")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_b_factsheet", true}
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", productInterestTarget }
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

            fakeContext.ExecutePluginWith<PreCreateProductInterest>(executionFakeContext);

            Assert.False(productInterestTarget.Contains("dpam_name"));
        }
    }
}
