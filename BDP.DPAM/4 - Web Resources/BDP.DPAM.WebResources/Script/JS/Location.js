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
                        const formContext = executionContext.getFormContext();
                        //SHER-163
                        Form.initDefaultCountryCode(formContext);
                    }
                    //function to initialize the field dpam_s_alpha2code based on dpam_lk_country
                    static initDefaultCountryCode(formContext) {
                        let defaultPhoneCountryValue = "BE";
                        let countryAttribute = formContext.getAttribute("dpam_lk_country");
                        if (countryAttribute.getValue()
                            && countryAttribute.getValue()[0]
                            && countryAttribute.getValue()[0].id) {
                            let countryLookupValue = countryAttribute.getValue()[0];
                            Xrm.WebApi.retrieveRecord(countryLookupValue.entityType, countryLookupValue.id, `?$select=dpam_s_alpha2code`).then(function (result) {
                                defaultPhoneCountryValue = result["dpam_s_alpha2code"];
                            }, function (error) { }).finally(function () {
                                formContext.getAttribute("dpam_s_country_alpha2code").setValue(defaultPhoneCountryValue);
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