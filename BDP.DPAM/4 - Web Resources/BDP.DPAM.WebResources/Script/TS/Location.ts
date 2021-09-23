/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Location {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {

        }

        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-163
            Form.initDefaultCountryCode(formContext);
        }

        //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
        static initDefaultCountryCode(formContext: Xrm.FormContext) {

            let defaultPhoneCountryValue: string = "BE";
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_country");

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
                    formContext.getAttribute("dpam_s_country_alpha2code").setValue(defaultPhoneCountryValue);
                });
            }
        }
    }
}