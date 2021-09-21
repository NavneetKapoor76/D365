/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Location;
            (function (Location) {
                class Form {
                    static onLoad(executionContext) {
                    }
                    static onChange_dpam_lk_country(executionContext) {
                        //SHER-163
                        Form.initDefaultCountryCode(executionContext);
                    }
                    //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
                    static initDefaultCountryCode(executionContext) {
                        let _defaultPhoneCountryValue = "BE";
                        let _country_attribute = executionContext.getFormContext().getAttribute("dpam_lk_country");
                        if (_country_attribute.getValue()
                            && _country_attribute.getValue()[0]
                            && _country_attribute.getValue()[0].id) {
                            let _country_lookupvalue = _country_attribute.getValue()[0];
                            Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=dpam_s_alpha2code`).then(function (result) {
                                _defaultPhoneCountryValue = result["dpam_s_alpha2code"];
                            }, function (error) { }).finally(function () {
                                executionContext.getFormContext().getAttribute("dpam_s_country_alpha2code").setValue(_defaultPhoneCountryValue);
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