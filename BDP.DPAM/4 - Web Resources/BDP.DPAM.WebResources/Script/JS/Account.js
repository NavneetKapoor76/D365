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
                                dpam_s_alpha2code: "dpam_s_alpha2code"
                            },
                            account: {
                                dpam_lk_country: "dpam_lk_country",
                                dpam_s_country_alpha2code: "dpam_s_country_alpha2code"
                            }
                        };
                    }
                }
                Account.Static = new _Static();
                class Form {
                    static onLoad(executionContext) {
                        /* const formContext: Xrm.FormContext = executionContext.getFormContext();
             
                         const name = formContext.getAttribute("name").getValue();
                         alert(`test ${name}`);*/
                    }
                    static onChange_dpam_lk_country(executionContext) {
                        this.initDefaultCountryCode(executionContext);
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