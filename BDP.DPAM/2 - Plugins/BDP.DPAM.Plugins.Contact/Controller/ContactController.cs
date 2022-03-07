using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Plugins.Contact
{
    internal class ContactController : PluginManagerBase
    {
        internal ContactController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Add address field in the Target entity
        /// </summary>
        internal void AddAddressFieldInTargetBasedOnMainLocation()
        {
            if (!_target.Contains("dpam_lk_mainlocation")) return;

            _tracing.Trace("AddAddressFieldInTargetBasedOnMainLocation - Start");

            var mainLocationEntityReference = _target.GetAttributeValue<EntityReference>("dpam_lk_mainlocation");

            if (mainLocationEntityReference == null && _context.MessageName == MessageName.Create)
            {
                _tracing.Trace("AddAddressFieldInTargetBasedOnMainLocation - End");
                return;
            }

            var mainLocationEntity = new Entity("dpam_location");
            if (mainLocationEntityReference != null)
            {
                var columnSet = new ColumnSet("dpam_lk_country", "dpam_s_street1", "dpam_s_street2", "dpam_s_street3", "dpam_s_postalcode", "dpam_s_city", "dpam_postofficebox");               
                mainLocationEntity = _service.Retrieve(mainLocationEntityReference.LogicalName, mainLocationEntityReference.Id, columnSet);
            }

            var attributeCollection = new Dictionary<string, string>
            {
                //location's attribute, contact's attribute
                { "dpam_lk_country", "dpam_lk_country" },
                { "dpam_s_street1", "address1_line1" },
                { "dpam_s_street2", "address1_line2" },
                { "dpam_s_street3", "address1_line3" },
                { "dpam_s_postalcode", "address1_postalcode" },
                { "dpam_s_city", "address1_city" },
                { "dpam_postofficebox", "address1_postofficebox" }
            };

            foreach (var key in attributeCollection.Keys)
            {
                _target[attributeCollection[key]] = mainLocationEntity.Contains(key) ? mainLocationEntity[key] : null;
                if (key == "dpam_lk_country")
                {
                    var countryName = string.Empty;
                    if (mainLocationEntity.Contains(key))
                    {
                        var country = _service.Retrieve("dpam_country", mainLocationEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id, new ColumnSet("dpam_s_name"));
                        countryName = country.GetAttributeValue<string>("dpam_s_name");
                    }
                    _target["address1_country"] = countryName;
                }
            }

            _tracing.Trace("AddAddressFieldInTargetBasedOnMainLocation - End");
        }

        /// <summary>
        /// Set the field dpam_lk_greeting based on the Language and the Gender of the Contact
        /// </summary>
        internal void SetContactGreetingBasedOnLanguageAndGender()
        {
            if (!_target.Contains("dpam_os_language") && !_target.Contains("dpam_os_gender"))
                return;

            _tracing.Trace("SetContactGreetingBasedOnLanguageAndGender - Start");

            Entity mergedContact = _target.MergeEntity(_preImage);

            OptionSetValue contactLanguage = mergedContact.GetAttributeValue<OptionSetValue>("dpam_os_language");
            OptionSetValue contactGender = mergedContact.GetAttributeValue<OptionSetValue>("dpam_os_gender");

            EntityReference contactGreetingRef = null;

            if (contactLanguage != null && contactGender != null)
                contactGreetingRef = GetGreetingRefBasedOnLanguageAndGender(contactLanguage.Value, contactGender.Value);

            _target["dpam_lk_greeting"] = contactGreetingRef;

            _tracing.Trace("SetContactGreetingBasedOnLanguageAndGender - End");
        }

        /// <summary>
        /// Retrieve a Reference to a Greeting based on a Language and a Gender
        /// </summary>
        /// <param name="languageValue">Language to use</param>
        /// <param name="genderValue">Gender to use</param>
        /// <returns>Reference to a Greeting</returns>
        private EntityReference GetGreetingRefBasedOnLanguageAndGender(int languageValue, int genderValue)
        {
            _tracing.Trace("GetGreetingRefBasedOnLanguageAndGender - Start");

            EntityReference retVal = null;

            ConditionExpression conditionGender = new ConditionExpression("dpam_os_gender", ConditionOperator.Equal, genderValue);
            ConditionExpression conditionLanguage = new ConditionExpression("dpam_os_language", ConditionOperator.Equal, languageValue);

            QueryExpression query = new QueryExpression("dpam_greeting");
            query.Criteria.AddCondition(conditionGender);
            query.Criteria.AddCondition(conditionLanguage);

            EntityCollection result = _service.RetrieveMultiple(query);

            if(result.Entities.Count > 1)
                throw new Exception($"Multiple Greetings found for Language {languageValue} and Gender {genderValue}");

            if (result.Entities.Count > 0)
                retVal = result.Entities[0].ToEntityReference();

            _tracing.Trace("GetGreetingRefBasedOnLanguageAndGender - End");

            return retVal;
        }

        /// <summary>
        /// Set the field Direct Line with the value of the field Main Phone of the related CounterParty
        /// </summary>
        internal void SetContactDirectLineBasedOnCounterpartyMainPhone()
        {
            if (!_target.Contains("parentcustomerid"))
                return;

            _tracing.Trace("SetContactDirectLineBasedOnCounterpartyMainPhone - Start");

            EntityReference parentCounterPartyRef = _target.GetAttributeValue<EntityReference>("parentcustomerid");

            // Should never be a Contact 
            //  -> JS has been added on the form to ensure selection of a Counterparty. 
            //  -> System will throw an Exception in case of error during the retrieve
            Entity counterParty = _service.Retrieve(parentCounterPartyRef, new string[] { "telephone1" });
            string counterPartyMainPhone = counterParty.GetAttributeValue<string>("telephone1");

            // Do not sync in case of creation if the Counterparty Main phone is empty
            if (_context.MessageName.ToLower() != "create" || !string.IsNullOrWhiteSpace(counterPartyMainPhone))
                _target["business2"] = counterPartyMainPhone;

            _tracing.Trace("SetContactDirectLineBasedOnCounterpartyMainPhone - End");
        }

        /// <summary>
        /// The "donotbulkemail" field has the opposite value of the "dpam_b_bulkemailoptinmarketingtechnical" field
        /// </summary>
        internal void ManageEmailOptInMarketingBulkEmail()
        {
            if (!_target.Contains("dpam_b_bulkemailoptinmarketingtechnical") || !_target.GetAttributeValue<bool>("dpam_b_bulkemailoptinmarketingtechnical")) return;

            _tracing.Trace("ManageEmailOptInMarketingBulkEmail - Start");

            _target["donotbulkemail"] = !_target.GetAttributeValue<bool>("dpam_b_bulkemailoptinmarketingtechnical");

            _tracing.Trace("ManageEmailOptInMarketingBulkEmail - End");
        }

        /// <summary>
        /// Set the "dpam_os_bulkemailoptinrequest" column to "Handled" when target contains "donotbulkemail" and the "dpam_os_bulkemailoptinrequest" column is equal to "Processing" in the preImage
        /// </summary>
        internal void ManageStatusRequestEmailOptinMarketing()
        {
            if (!_target.Contains("donotbulkemail") || !_preImage.Contains("dpam_os_bulkemailoptinrequest") || _preImage.GetAttributeValue<OptionSetValue>("dpam_os_bulkemailoptinrequest").Value != (int)MarketingOptinRequest.Processing) return;

            _tracing.Trace("ManageStatusRequestEmailOptinMarketing - Start");

            _target["dpam_os_bulkemailoptinrequest"] = new OptionSetValue((int)MarketingOptinRequest.Handled);

            _tracing.Trace("ManageStatusRequestEmailOptinMarketing - End");
        }
    }
}