using System;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Plugins.Account
{
    internal class AccountController : PluginManagerBase
    {

        internal AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Fill in the field dpam_lk_businesssegmentation
        /// </summary>
        internal void CompleteSegmentation()
        {
            if (!_target.Contains("dpam_lk_localbusinesssegmentation"))
                return;

            _tracing.Trace("CompleteSegmentation - Start");

            if (_target["dpam_lk_localbusinesssegmentation"] != null)
            {
                Entity localSegmentationEntity = _service.Retrieve(_target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation"), "dpam_lk_businesssegmentation");

                _target["dpam_lk_businesssegmentation"] = localSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation");
            }
            else
            {
                _target["dpam_lk_businesssegmentation"] = null;

            }

            _tracing.Trace("CompleteSegmentation - End");
        }

        /// <summary>
        /// If the country of the counterparty has changed to a different one, then we need to empty the fields "dpam_lk_localbusinesssegmentation" & "dpam_lk_businesssegmentation".
        /// </summary>
        internal void CheckLocalAndBusinessSegmentationCountry()
        {
            if (!_target.Contains("dpam_lk_country"))
                return;

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - Start");

            // The _preImage doesn't contain a field which is null, so testing it with "== null" will give you an error.
            if (_target["dpam_lk_country"] == null || !_preImage.Contains("dpam_lk_country") || 
                    !_target.GetAttributeValue<EntityReference>("dpam_lk_country").Equals(_preImage.GetAttributeValue<EntityReference>("dpam_lk_country")))
            {
                _target["dpam_lk_localbusinesssegmentation"] = null;
                _target["dpam_lk_businesssegmentation"] = null;
            }

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - End");
        }

        /// <summary>
        /// Create a location when an originating lead exists
        /// </summary>
        internal void CreateLocationWhenOriginatingLeadExists()
        {
            if (!_target.Contains("originatingleadid") || _target["originatingleadid"] == null) return;

            var location = new Entity("dpam_location");

            if (_target.Contains("address1_line1"))
                location["dpam_s_street1"] = _target["address1_line1"];

            if (_target.Contains("address1_line2"))
                location["dpam_s_street2"] = _target["address1_line2"];

            if (_target.Contains("address1_postofficebox"))
                location["dpam_postofficebox"] = _target["address1_postofficebox"];

            if (_target.Contains("address1_postalcode"))
                location["dpam_s_postalcode"] = _target["address1_postalcode"];

            if (_target.Contains("address1_city"))
                location["dpam_s_city"] = _target["address1_city"];

            if (_target.Contains("dpam_lk_country"))
                location["dpam_lk_country"] = _target["dpam_lk_country"];

            location["dpam_lk_account"] = _target.ToEntityReference();
            location["dpam_b_main"] = true;
            location["dpam_b_business"] = true;

            _service.Create(location);
        }

        /// <summary>
        /// Add fields in the target when an originating lead exists
        /// </summary>
        internal void AddFieldsInTargetWhenOriginatingLeadExists()
        {
            if (!_target.Contains("originatingleadid") || _target["originatingleadid"] == null) return;

            var street1 = string.Empty;
            var postalCode = string.Empty;
            var city = string.Empty;
            var country = string.Empty;

            if (_target.Contains("dpam_lk_country"))
                country = CommonLibrary.GetRecordName(_service, _target.GetAttributeValue<EntityReference>("dpam_lk_country"), "dpam_s_name");

            _target["address1_country"] = country;

            if (_target.Contains("address1_line1"))
                street1 = _target.GetAttributeValue<string>("address1_line1");

            if (_target.Contains("address1_postalcode"))
                postalCode = _target.GetAttributeValue<string>("address1_postalcode");

            if (_target.Contains("address1_city"))
                city = _target.GetAttributeValue<string>("address1_city");

            _target["dpam_s_address1"] = $"{street1}, {postalCode}, {city}, {country}";
        }

        /// <summary>
        /// Manage the following fields based on the lifestage: "dpam_b_exclient", "dpam_b_withinvestmentinthepast" 
        /// </summary>
        internal void ManageExClientLifestage()
        {
            if (!_target.Contains("statuscode")) return;

            _tracing.Trace("ManageExClientLifestage - Start");

            var statuscodePreImage = _preImage.GetAttributeValue<OptionSetValue>("statuscode");
            var statuscodeTarget = _target.GetAttributeValue<OptionSetValue>("statuscode");

            //Lifestage goes from Active to Prospect
            if(statuscodePreImage.Value == (int)Account_StatusCode.Active && statuscodeTarget.Value == (int)Account_StatusCode.Prospect)
            {
                _target["dpam_b_exclient"] = true;
                _target["dpam_b_withinvestmentinthepast"] = true;
            }
            //Lifestage goes from Prospect to Active
            else if (statuscodePreImage.Value == (int)Account_StatusCode.Prospect && statuscodeTarget.Value == (int)Account_StatusCode.Active)
            {
                _target["dpam_b_exclient"] = false;
            }

            _tracing.Trace("ManageExClientLifestage - End");
        }
    }
}
