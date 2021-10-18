/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Lead;
            (function (Lead) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-331
                        Form.setBusinessSegmentationFilter(formContext);
                        //SHER-331
                        Form.setLocalBusinessSegmentationFilter(formContext);
                        //SHER-331 + SHER-391 + SHER-427
                        Form.manageBusinessSegmentationVisibilityAndRequirementLevel(formContext);
                    }
                    static onChange_dpam_lk_country(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-331 + SHER-391 + SHER-427
                        Form.manageBusinessSegmentationVisibilityAndRequirementLevel(formContext);
                    }
                    static quickCreateOnLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-349
                        Form.setBusinessSegmentationFilter(formContext);
                        //SHER-349
                        Form.setLocalBusinessSegmentationFilter(formContext);
                        //SHER-349 + SHER-391 + SHER-427
                        Form.manageBusinessSegmentationVisibilityAndRequirementLevel(formContext);
                    }
                    static quickCreateOnChange_dpam_lk_country(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-349 + SHER-391 + SHER-427
                        Form.manageBusinessSegmentationVisibilityAndRequirementLevel(formContext);
                    }
                    //function to add a custom filter on the dpam_lk_businesssegmentation field
                    static filterBusinessSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let counterpartyTypeAttribute = formContext.getAttribute("dpam_mos_counterpartytype");
                        if (counterpartyTypeAttribute.getValue() != null) {
                            let selectedOptions = counterpartyTypeAttribute.getValue();
                            let values = "";
                            selectedOptions.forEach(function (item) {
                                values += `<value>${item}</value>`;
                            });
                            let filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                            </filter>`;
                            formContext.getControl("dpam_lk_businesssegmentation").addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
                        }
                    }
                    //function to set the filter on the dpam_lk_businesssegmentation field
                    static setBusinessSegmentationFilter(formContext) {
                        formContext.getControl("dpam_lk_businesssegmentation").addPreSearch(Form.filterBusinessSegmentation);
                    }
                    //function to add a custom filter on the dpam_lk_localbusinesssegmentation field
                    static filterLocalBusinessSegmentation(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let counterpartyTypeAttribute = formContext.getAttribute("dpam_mos_counterpartytype");
                        let countryAttribute = formContext.getAttribute("dpam_lk_country");
                        if (counterpartyTypeAttribute.getValue() != null && countryAttribute.getValue() != null) {
                            let selectedOptions = counterpartyTypeAttribute.getValue();
                            let values = "";
                            selectedOptions.forEach(function (item) {
                                values += `<value>${item}</value>`;
                            });
                            let filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                              <condition attribute="dpam_lk_country" operator="eq" uitype="dpam_country" value="${countryAttribute.getValue()[0].id}" />
                            </filter>`;
                            formContext.getControl("dpam_lk_localbusinesssegmentation").addCustomFilter(filter, "dpam_cplocalbusinesssegmentation");
                        }
                    }
                    //function to set the filter on the dpam_lk_localbusinesssegmentation field
                    static setLocalBusinessSegmentationFilter(formContext) {
                        formContext.getControl("dpam_lk_localbusinesssegmentation").addPreSearch(Form.filterLocalBusinessSegmentation);
                    }
                    /*function to set the visibility and the requirement level of the dpam_lk_localbusinesssegmentation field
                        and enable/disable the dpam_lk_businesssegmentation field*/
                    static manageBusinessSegmentationVisibilityAndRequirementLevel(formContext) {
                        let countryAttribute = formContext.getAttribute("dpam_lk_country");
                        if (countryAttribute.getValue() == null)
                            return;
                        let localbusinessSegmentationControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
                        let businessSegmentationControl = formContext.getControl("dpam_lk_businesssegmentation");
                        let fetchXml = `?fetchXml=<fetch top="1">
                                        <entity name="dpam_cplocalbusinesssegmentation" >
                                            <attribute name="dpam_cplocalbusinesssegmentationid" />
                                        <filter>
                                            <condition attribute="dpam_lk_country" operator="eq" value="${countryAttribute.getValue()[0].id}" />
                                        </filter></entity></fetch>`;
                        Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(function success(result) {
                            let localBusinessSegmentationIsVisible = false;
                            if (result.entities.length > 0)
                                localBusinessSegmentationIsVisible = true;
                            let localBusinessSegmentationRequirementLevel = localBusinessSegmentationIsVisible ? "required" : "none";
                            localbusinessSegmentationControl.setVisible(localBusinessSegmentationIsVisible);
                            localbusinessSegmentationControl.getAttribute().setRequiredLevel(localBusinessSegmentationRequirementLevel);
                            businessSegmentationControl.setDisabled(localBusinessSegmentationIsVisible);
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                }
                Lead.Form = Form;
                class Ribbon {
                    //function to open the disqualify lead custom page
                    static openDisqualifyLeadCustomPage() {
                        let pageInput = {
                            pageType: "custom",
                            name: "dpam_disqualifyleadcustompage_3731d",
                            entityName: "lead",
                            recordId: Xrm.Page.data.entity.getId()
                        };
                        let navigationOptions = {
                            target: 2,
                            width: 530,
                            height: 350,
                            title: "Disqualification"
                        };
                        Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                            .then(function success() {
                            Xrm.Page.data.refresh(true);
                        }, function error() {
                            console.log(error);
                        });
                    }
                }
                Lead.Ribbon = Ribbon;
            })(Lead = WR.Lead || (WR.Lead = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Lead.js.map