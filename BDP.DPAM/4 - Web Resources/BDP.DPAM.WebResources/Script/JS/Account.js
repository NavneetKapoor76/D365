/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Account;
            (function (Account) {
                class Form {
                    static onLoad(executionContext) {
                        //SHER-174
                        this.setBusinessSegmentationFilter(executionContext);
                        //SHER-244
                        this.setComplianceSegmentationFilter(executionContext);
                        //SHER-268
                        this.setLocalBusinessSegmentationFilter(executionContext);
                        // SHER-292
                        this.manageBusinessSegmentationVisibility(executionContext);
                    }
                    static onChange_dpam_lk_vatnumber(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-92
                        this.checkValidVATNumber(formContext);
                    }
                    // SHER-292
                    static onChange_dpam_lk_country(executionContext) {
                        //SHER-292
                        this.manageBusinessSegmentationVisibility(executionContext);
                    }
                    //function to check if the VAT number in the account is valid based on the VAT format of the country.
                    static checkValidVATNumber(formContext) {
                        formContext.getControl("dpam_s_vatnumber").clearNotification();
                        let _VATNumber_attribute = formContext.getAttribute("dpam_s_vatnumber");
                        if (_VATNumber_attribute.getValue()) {
                            //Remove spaces & non alphanumeric caracters from the VAT
                            let _VATNumberFormatted_attribute = _VATNumber_attribute.getValue().replace(/[^0-9a-zA-Z]/g, '');
                            formContext.getAttribute("dpam_s_vatnumber").setValue(_VATNumberFormatted_attribute);
                            let _country_attribute = formContext.getAttribute("dpam_lk_country");
                            if (_country_attribute.getValue() && _country_attribute.getValue()[0] && _country_attribute.getValue()[0].id) {
                                let _country_lookupvalue = _country_attribute.getValue()[0];
                                Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=dpam_s_vatformat, dpam_s_vatformatexample`).then(function success(result) {
                                    let _VATFormatValue = result["dpam_s_vatformat"];
                                    if (_VATFormatValue != null && !_VATNumberFormatted_attribute.match(_VATFormatValue)) {
                                        formContext.getControl("dpam_s_vatnumber").setNotification("The format isn't valid. Please use following format: " + result["dpam_s_vatformatexample"], "invalidFormat");
                                    }
                                }, function (error) {
                                    console.log(error.message);
                                });
                            }
                            else {
                                formContext.getControl("dpam_s_vatnumber").setNotification("The country field is empty, no VAT number can be entered.", "countryEmpty");
                            }
                        }
                    }
                    //function to add a custom filter on the dpam_lk_compliancesegmentation field
                    static filterComplianceSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let filter = `<filter type="and" >
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="null" >
                              </condition>
                            </filter>`;
                        let _dpam_lk_counterpartymifidcategory = formContext.getAttribute("dpam_lk_counterpartymifidcategory");
                        if (_dpam_lk_counterpartymifidcategory != null && _dpam_lk_counterpartymifidcategory.getValue() != null) {
                            let id = _dpam_lk_counterpartymifidcategory.getValue()[0].id;
                            filter = `<filter type="and">
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="eq" value=" ${id}" >     
                              </condition>
                            </filter>`;
                        }
                        formContext.getControl("dpam_lk_compliancesegmentation").addCustomFilter(filter, "dpam_counterpartycompliancesegmentation");
                    }
                    //function to add a custom filter on the dpam_lk_businesssegmentation field
                    static filterBusinessSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;
                        let _dpam_mos_counterpartytype = formContext.getAttribute("dpam_mos_counterpartytype");
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
                        formContext.getControl("dpam_lk_businesssegmentation").addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
                    }
                    //function to set the filter on the dpam_lk_businesssegmentation field
                    static setBusinessSegmentationFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let _dpam_lk_businesssegmentation_control = formContext.getControl("dpam_lk_businesssegmentation");
                        if (_dpam_lk_businesssegmentation_control != null) {
                            _dpam_lk_businesssegmentation_control.addPreSearch(this.filterBusinessSegmentation);
                        }
                    }
                    //function to set the filter on the dpam_lk_compliancesegmentation field
                    static setComplianceSegmentationFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let _dpam_lk_compliancesegmentation_control = formContext.getControl("dpam_lk_compliancesegmentation");
                        if (_dpam_lk_compliancesegmentation_control != null) {
                            _dpam_lk_compliancesegmentation_control.addPreSearch(this.filterComplianceSegmentation);
                        }
                    }
                    // Opens the "Lei Code Search" Canvas app in a dialog based on the URL retrieved from the settings entity.
                    static dialogCanvasApp() {
                        let dialogOptions = { height: 815, width: 1350 };
                        Xrm.WebApi.retrieveRecord("dpam_settings", "a53657d3-25f4-eb11-94ef-000d3a237027", `?$select=dpam_s_value`).then(function success(result) {
                            Xrm.Navigation.openUrl(result["dpam_s_value"], dialogOptions);
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
                    static manageBusinessSegmentationVisibility(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //retrieve the country of counterparty.
                        let _dpam_lk_country = formContext.getAttribute("dpam_lk_country");
                        let _dpam_lk_localbusinesssegmentation_control = formContext.getControl("dpam_lk_localbusinesssegmentation");
                        let _dpam_lk_businesssegmentation_control = formContext.getControl("dpam_lk_businesssegmentation");
                        _dpam_lk_localbusinesssegmentation_control.setVisible(false);
                        _dpam_lk_businesssegmentation_control.setVisible(false);
                        if (_dpam_lk_country != null && _dpam_lk_country.getValue() != null && _dpam_lk_country.getValue()[0] && _dpam_lk_country.getValue()[0].id) {
                            var fetchXml = `?fetchXml=<fetch top="1"><entity name="dpam_cplocalbusinesssegmentation" ><attribute name="dpam_cplocalbusinesssegmentationid" /><filter><condition attribute="dpam_lk_country" operator="eq" value="${_dpam_lk_country.getValue()[0].id}" /></filter></entity></fetch>`;
                            // search at least one occurence of this country in Local segmentation
                            Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(function success(result) {
                                if (result.entities.length > 0) {
                                    // found
                                    // if one found, set visible Local segmentation and hide generic segmentation  (fill generic segmentation)
                                    _dpam_lk_localbusinesssegmentation_control.setVisible(true);
                                    _dpam_lk_businesssegmentation_control.setVisible(false);
                                }
                                else {
                                    // nothing found
                                    // if not found set visible generic segmentation and hide local segmentation (fill local to null)
                                    _dpam_lk_localbusinesssegmentation_control.setVisible(false);
                                    _dpam_lk_businesssegmentation_control.setVisible(true);
                                }
                            }, function (error) {
                                console.log(error.message);
                                // handle error conditions
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