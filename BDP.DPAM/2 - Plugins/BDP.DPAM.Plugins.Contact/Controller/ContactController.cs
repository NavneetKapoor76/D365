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
        /// <param name="messageName">string: create or update</param>
        internal void AddAddressFieldInTargetBasedOnMainLocation(string messageName)
        {
            if (!_target.Contains("dpam_lk_mainlocation")) return;

            var mainLocationEntityReference = _target.GetAttributeValue<EntityReference>("dpam_lk_mainlocation");

            if (mainLocationEntityReference == null && messageName == "create") return;

            var mainLocationEntity = new Entity("dpam_location");
            if (mainLocationEntityReference != null)
            {
                var columnSet = new ColumnSet("dpam_lk_country", "dpam_s_street1", "dpam_s_street2", "dpam_s_street3", "dpam_s_postalcode", "dpam_s_city", "dpam_postofficebox");
                _tracing.Trace($"AddAddressFieldInTargetBasedOnMainLocation function - Retrieve {mainLocationEntityReference.LogicalName}");
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
                        _tracing.Trace($"AddAddressFieldInTargetBasedOnMainLocation function - Retrieve dpam_country");
                        var country = _service.Retrieve("dpam_country", mainLocationEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id, new ColumnSet("dpam_s_name"));
                        countryName = country.GetAttributeValue<string>("dpam_s_name");
                    }
                    _target["address1_country"] = countryName;
                }
            }
        }

        /// <summary>
        /// Set the field dpam_lk_greeting based on the Language and the Gender of the Contact
        /// </summary>
        internal void SetContactGreetingBasedOnLanguageAndGender()
        {
            if (!this._target.Contains("dpam_os_language") && !this._target.Contains("gendercode"))
                return;

            this._tracing.Trace("SetContactGreetingBasedOnLanguageAndGender - Start");

            Entity mergedContact = this._target.MergeEntity(this._preImage);

            OptionSetValue contactLanguage = mergedContact.GetAttributeValue<OptionSetValue>("dpam_os_language");
            OptionSetValue contactGender = mergedContact.GetAttributeValue<OptionSetValue>("gendercode");

            EntityReference contactGreetingRef = null;

            if (contactLanguage != null && contactGender != null)
            {
                int relatedGenderCode = 
                    contactGender.Value == Convert.ToInt32(Contact_Gender.Female) ? Convert.ToInt32(Greeting_Gender.Female) : 
                    contactGender.Value == Convert.ToInt32(Contact_Gender.Male) ? Convert.ToInt32(Greeting_Gender.Male) : 
                    Convert.ToInt32(Greeting_Gender.NonBinary);

                contactGreetingRef = this.GetGreetingRefBasedOnLanguageAndGender(contactLanguage.Value, relatedGenderCode);
            }

            this._target["dpam_lk_greeting"] = contactGreetingRef;

            this._tracing.Trace("SetContactGreetingBasedOnLanguageAndGender - End");
        }

        /// <summary>
        /// Retrieve a Reference to a Greeting based on a Language and a Gender
        /// </summary>
        /// <param name="languageValue">Language to use</param>
        /// <param name="genderValue">Gender to use</param>
        /// <returns>Reference to a Greeting</returns>
        private EntityReference GetGreetingRefBasedOnLanguageAndGender(int languageValue, int genderValue)
        {
            this._tracing.Trace("GetGreetingRefBasedOnLanguageAndGender - Start");

            EntityReference retVal = null;

            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dpam_greeting'>
                                <attribute name='dpam_greetingid' />
                                <filter type='and'>
                                  <condition attribute='dpam_os_gender' operator='eq' value='{genderValue}' />
                                  <condition attribute='dpam_os_language' operator='eq' value='{languageValue}' />
                                </filter>
                              </entity>
                            </fetch>";

            FetchExpression query = new FetchExpression(fetch);
            EntityCollection result = this._service.RetrieveMultiple(query);

            if(result.Entities.Count > 1)
                throw new Exception($"Multiple Greetings found for Language {languageValue} and Gender {genderValue}");

            if (result.Entities.Count > 0)
                retVal = result.Entities[0].ToEntityReference();

            this._tracing.Trace("GetGreetingRefBasedOnLanguageAndGender - End");

            return retVal;
        }
    }
}