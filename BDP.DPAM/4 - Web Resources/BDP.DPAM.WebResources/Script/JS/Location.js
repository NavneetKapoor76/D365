/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Location;
            (function (Location) {
                class _Static {
                    constructor() {
                        this.field = {
                            dpam_country: {
                                dpam_s_alpha2code: "dpam_s_alpha2code",
                                dpam_s_vatformat: "dpam_s_vatformat"
                            },
                            dpam_location: {
                                dpam_lk_country: "dpam_lk_country",
                                dpam_s_country_alpha2code: "dpam_s_country_alpha2code"
                            }
                        };
                    }
                }
                Location.Static = new _Static();
                class Form {
                    static onLoad(executionContext) {
                    }
                    static onChange_dpam_lk_country(executionContext) {
                        this.initDefaultCountryCode(executionContext);
                    }
                    //function to initialize the field dpam_s_alpha2code based on dpam_lk_country . Copied from Account.JS
                    static initDefaultCountryCode(executionContext) {
                        let _defaultPhoneCountryValue = "BE";
                        let _country_attribute = executionContext.getFormContext().getAttribute(Location.Static.field.dpam_location.dpam_lk_country);
                        if (_country_attribute.getValue()
                            && _country_attribute.getValue()[0]
                            && _country_attribute.getValue()[0].id) {
                            let _country_lookupvalue = _country_attribute.getValue()[0];
                            Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Location.Static.field.dpam_country.dpam_s_alpha2code}`).then(function (result) {
                                _defaultPhoneCountryValue = result[Location.Static.field.dpam_country.dpam_s_alpha2code];
                            }, function (error) { }).finally(function () {
                                executionContext.getFormContext().getAttribute(Location.Static.field.dpam_location.dpam_s_country_alpha2code).setValue(_defaultPhoneCountryValue);
                            });
                        }
                    }
                }
                Location.Form = Form;
            })(Location = WR.Location || (WR.Location = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Location.js.map