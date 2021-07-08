/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {

    class _Static {
        readonly field = {
            dpam_country: {
                dpam_s_alpha2code: "dpam_s_alpha2code"
            },
            account: {
                dpam_lk_country: "dpam_lk_country",
                dpam_s_country_alpha2code: "dpam_s_country_alpha2code"
            }
        };

    }
    export let Static = new _Static();


    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {

            /* const formContext: Xrm.FormContext = executionContext.getFormContext();
 
             const name = formContext.getAttribute("name").getValue();
             alert(`test ${name}`);*/
        }
        public static onChange_dpam_lk_country() {

            this.initDefaultCountryCode();

        }

        //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
        static initDefaultCountryCode() {

            let _defaultPhoneCountryValue: string = "BE";
            let _country_attribute: Xrm.Page.LookupAttribute = Xrm.Page.getAttribute<Xrm.Page.LookupAttribute>(Static.field.account.dpam_lk_country);


            if (_country_attribute.getValue()
                && _country_attribute.getValue()[0]
                && _country_attribute.getValue()[0].id) {

                let _country_lookupvalue = _country_attribute.getValue()[0];
                Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id,
                    `$select=${Static.field.dpam_country.dpam_s_alpha2code}`

                ).then(function (result) {
                    _defaultPhoneCountryValue = result[Static.field.dpam_country.dpam_s_alpha2code];
                },
                    function (error) { }

                ).finally(function () {
                    Xrm.Page.getAttribute(Static.field.account.dpam_s_country_alpha2code).setValue(_defaultPhoneCountryValue);
                });
            }
        }
    }
}