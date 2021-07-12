/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {

    class _Static {
        readonly field = {
            dpam_country: {
                dpam_s_alpha2code: "dpam_s_alpha2code",
                dpam_s_vatformat: "dpam_s_vatformat"
            },
            account: {
                dpam_lk_country: "dpam_lk_country",
                dpam_s_country_alpha2code: "dpam_s_country_alpha2code",
                dpam_s_vatnumber: "dpam_s_vatnumber"
            }
        };

    }
    export let Static = new _Static();


    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext = executionContext.getFormContext();
            this.checkValidVATNumber(formContext);
        }

        public static onChange_dpam_lk_country() {
            this.initDefaultCountryCode();
        }

        public static onChange_dpam_lk_vatnumber(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            this.checkValidVATNumber(formContext);
        }

        //function to check if the VAT number in the account is valid based on the VAT format of the country.
        static checkValidVATNumber(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).clearNotification();

            let _VATNumber_attribute: Xrm.Page.StringAttribute = formContext.getAttribute<Xrm.Page.StringAttribute>(Static.field.account.dpam_s_vatnumber);

            if (_VATNumber_attribute.getValue()) {
                let _country_attribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>(Static.field.account.dpam_lk_country);

                if (_country_attribute.getValue() && _country_attribute.getValue()[0] && _country_attribute.getValue()[0].id) {
                    let _country_lookupvalue = _country_attribute.getValue()[0];
                    Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Static.field.dpam_country.dpam_s_vatformat}`).then(
                        function success(result) {
                            let _VATFormatValue = result[Static.field.dpam_country.dpam_s_vatformat];
                            if (_VATFormatValue != null && !_VATNumber_attribute.getValue().match(_VATFormatValue)) {
                                formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).setNotification("The VAT format isn't valid.", "invalidFormat")
                            }
                        },
                        function (error) {
                            console.log(error.message);
                        }
                    );
                } else {
                    formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).setNotification("The country field is empty, no VAT number can be entered.", "countryEmpty")
                }
            }
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
                    `?$select=${Static.field.dpam_country.dpam_s_alpha2code}`

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