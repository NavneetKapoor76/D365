/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Account;
            (function (Account) {
                class _Static {
                    constructor() {
                        this.field = {
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
                }
                Account.Static = new _Static();
                class Form {
                    static onLoad(executionContext) {
                    }
                    static onChange_dpam_lk_country(executionContext) {
                        this.initDefaultCountryCode(executionContext);
                    }
                    static onChange_dpam_lk_vatnumber(executionContext) {
                        const formContext = executionContext.getFormContext();
                        this.checkValidVATNumber(formContext);
                    }
                    //function to check if the VAT number in the account is valid based on the VAT format of the country.
                    static checkValidVATNumber(formContext) {
                        formContext.getControl(Account.Static.field.account.dpam_s_vatnumber).clearNotification();
                        let _VATNumber_attribute = formContext.getAttribute(Account.Static.field.account.dpam_s_vatnumber);
                        if (_VATNumber_attribute.getValue()) {
                            let _country_attribute = formContext.getAttribute(Account.Static.field.account.dpam_lk_country);
                            if (_country_attribute.getValue() && _country_attribute.getValue()[0] && _country_attribute.getValue()[0].id) {
                                let _country_lookupvalue = _country_attribute.getValue()[0];
                                Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Account.Static.field.dpam_country.dpam_s_vatformat}`).then(function success(result) {
                                    let _VATFormatValue = result[Account.Static.field.dpam_country.dpam_s_vatformat];
                                    if (_VATFormatValue != null && !_VATNumber_attribute.getValue().match(_VATFormatValue)) {
                                        formContext.getControl(Account.Static.field.account.dpam_s_vatnumber).setNotification("The VAT format isn't valid.", "invalidFormat");
                                    }
                                }, function (error) {
                                    console.log(error.message);
                                });
                            }
                            else {
                                formContext.getControl(Account.Static.field.account.dpam_s_vatnumber).setNotification("The country field is empty, no VAT number can be entered.", "countryEmpty");
                            }
                        }
                    }
                    //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
                    static initDefaultCountryCode(executionContext) {
                        let _defaultPhoneCountryValue = "BE";
                        let _country_attribute = executionContext.getFormContext().getAttribute(Account.Static.field.account.dpam_lk_country);
                        if (_country_attribute.getValue()
                            && _country_attribute.getValue()[0]
                            && _country_attribute.getValue()[0].id) {
                            let _country_lookupvalue = _country_attribute.getValue()[0];
                            Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Account.Static.field.dpam_country.dpam_s_alpha2code}`).then(function (result) {
                                _defaultPhoneCountryValue = result[Account.Static.field.dpam_country.dpam_s_alpha2code];
                            }, function (error) { }).finally(function () {
                                executionContext.getFormContext().getAttribute(Account.Static.field.account.dpam_s_country_alpha2code).setValue(_defaultPhoneCountryValue);
                            });
                        }
                    }
                }
                Account.Form = Form;
            })(Account = WR.Account || (WR.Account = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Account.js.map