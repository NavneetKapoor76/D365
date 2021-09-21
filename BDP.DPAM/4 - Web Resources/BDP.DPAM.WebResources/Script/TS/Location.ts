/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Location {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {

        }

        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            //SHER-163
            Form.initDefaultCountryCode(executionContext);
        }

        //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
        static initDefaultCountryCode(executionContext: Xrm.Events.EventContext) {

            let _defaultPhoneCountryValue: string = "BE";
            let _country_attribute: Xrm.Page.LookupAttribute = executionContext.getFormContext().getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_country");

            if (_country_attribute.getValue()
                && _country_attribute.getValue()[0]
                && _country_attribute.getValue()[0].id) {

                let _country_lookupvalue = _country_attribute.getValue()[0];
                Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id,
                    `?$select=dpam_s_alpha2code`

                ).then(function (result) {
                    _defaultPhoneCountryValue = result["dpam_s_alpha2code"];
                },
                    function (error) { }

                ).finally(function () {
                    executionContext.getFormContext().getAttribute("dpam_s_country_alpha2code").setValue(_defaultPhoneCountryValue);
                });
            }
        }
    }
}