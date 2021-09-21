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

            let defaultPhoneCountryValue: string = "BE";
            let countryAttribute: Xrm.Page.LookupAttribute = executionContext.getFormContext().getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_country");

            if (countryAttribute.getValue()
                && countryAttribute.getValue()[0]
                && countryAttribute.getValue()[0].id) {

                let countryLookupValue: Xrm.LookupValue = countryAttribute.getValue()[0];
                Xrm.WebApi.retrieveRecord(countryLookupValue.entityType, countryLookupValue.id,
                    `?$select=dpam_s_alpha2code`

                ).then(function (result) {
                    defaultPhoneCountryValue = result["dpam_s_alpha2code"];
                },
                    function (error) { }

                ).finally(function () {
                    executionContext.getFormContext().getAttribute("dpam_s_country_alpha2code").setValue(defaultPhoneCountryValue);
                });
            }
        }
    }
}