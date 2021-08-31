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
                                dpam_s_vatformat: "dpam_s_vatformat",
                                dpam_s_vatformatexample: "dpam_s_vatformatexample"
                            },
                            account: {
                                dpam_lk_country: "dpam_lk_country",
                                dpam_s_country_alpha2code: "dpam_s_country_alpha2code",
                                dpam_s_vatnumber: "dpam_s_vatnumber",
                                dpam_mos_counterpartytype: "dpam_mos_counterpartytype",
                                dpam_lk_businesssegmentation: "dpam_lk_businesssegmentation"
                            },
                            dpam_settings: {
                                dpam_s_value: "dpam_s_value"
                            }
                        };
                    }
                }
                Account.Static = new _Static();
                class Form {
                    static onLoad(executionContext) {
                        this.setBusinessSegmentationFilter(executionContext);
                        this.setLocalBusinessSegmentationFilter(executionContext);
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
                            //Remove spaces & non alphanumeric caracters from the VAT
                            let _VATNumberFormatted_attribute = _VATNumber_attribute.getValue().replace(/[^0-9a-zA-Z]/g, '');
                            formContext.getAttribute(Account.Static.field.account.dpam_s_vatnumber).setValue(_VATNumberFormatted_attribute);
                            let _country_attribute = formContext.getAttribute(Account.Static.field.account.dpam_lk_country);
                            if (_country_attribute.getValue() && _country_attribute.getValue()[0] && _country_attribute.getValue()[0].id) {
                                let _country_lookupvalue = _country_attribute.getValue()[0];
                                Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Account.Static.field.dpam_country.dpam_s_vatformat}, ${Account.Static.field.dpam_country.dpam_s_vatformatexample}`).then(function success(result) {
                                    let _VATFormatValue = result[Account.Static.field.dpam_country.dpam_s_vatformat];
                                    if (_VATFormatValue != null && !_VATNumberFormatted_attribute.match(_VATFormatValue)) {
                                        formContext.getControl(Account.Static.field.account.dpam_s_vatnumber).setNotification("The format isn't valid. Please use following format: " + result[Account.Static.field.dpam_country.dpam_s_vatformatexample], "invalidFormat");
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
                    //function to add a custom filter on the dpam_lk_businesssegmentation field
                    static filterBusinessSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;
                        let _dpam_mos_counterpartytype = formContext.getAttribute(Account.Static.field.account.dpam_mos_counterpartytype);
                        if (_dpam_mos_counterpartytype != null && _dpam_mos_counterpartytype.getValue() != null) {
                            let selectedOptions = _dpam_mos_counterpartytype.getValue();
                            let values = "";
                            selectedOptions.forEach(function (item) {
                                values += `<value>${item}</value>`;
                            });
                            filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                            </filter>`;
                        }
                        formContext.getControl(Account.Static.field.account.dpam_lk_businesssegmentation).addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
                    }
                    //function to set the filter on the dpam_lk_businesssegmentation field
                    static setBusinessSegmentationFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let _dpam_lk_businesssegmentation_control = formContext.getControl(Account.Static.field.account.dpam_lk_businesssegmentation);
                        if (_dpam_lk_businesssegmentation_control != null) {
                            _dpam_lk_businesssegmentation_control.addPreSearch(this.filterBusinessSegmentation);
                        }
                    }
                    // Opens the "Lei Code Search" Canvas app in a dialog based on the URL retrieved from the settings entity.
                    static dialogCanvasApp() {
                        let dialogOptions = { height: 815, width: 1350 };
                        Xrm.WebApi.retrieveRecord("dpam_settings", "a53657d3-25f4-eb11-94ef-000d3a237027", `?$select=${Account.Static.field.dpam_settings.dpam_s_value}`).then(function success(result) {
                            Xrm.Navigation.openUrl(result[Account.Static.field.dpam_settings.dpam_s_value], dialogOptions);
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                    //function to add a custom filter on the dpam_lk_localbusinesssegmentation field
                    static filterLocalBusinessSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;
                        let _dpam_mos_counterpartytype = formContext.getAttribute("dpam_mos_counterpartytype");
                        let _dpam_lk_country = formContext.getAttribute("dpam_lk_country");
                        if (_dpam_mos_counterpartytype != null && _dpam_mos_counterpartytype.getValue() != null && _dpam_lk_country != null && _dpam_lk_country.getValue() != null) {
                            let selectedOptions = _dpam_mos_counterpartytype.getValue();
                            let values = "";
                            selectedOptions.forEach(function (item) {
                                values += `<value>${item}</value>`;
                            });
                            filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                              <condition attribute="dpam_lk_country" operator="eq" uitype="dpam_country" value="${_dpam_lk_country.getValue()[0].id}" />
                            </filter>`;
                        }
                        formContext.getControl("dpam_lk_localbusinesssegmentation").addCustomFilter(filter, "dpam_cplocalbusinesssegmentation");
                    }
                    //function to set the filter on the dpam_lk_localbusinesssegmentation field
                    static setLocalBusinessSegmentationFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let _dpam_lk_localbusinesssegmentation_control = formContext.getControl("dpam_lk_localbusinesssegmentation");
                        if (_dpam_lk_localbusinesssegmentation_control != null) {
                            _dpam_lk_localbusinesssegmentation_control.addPreSearch(this.filterLocalBusinessSegmentation);
                        }
                    }
                }
                Account.Form = Form;
            })(Account = WR.Account || (WR.Account = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Account.js.map