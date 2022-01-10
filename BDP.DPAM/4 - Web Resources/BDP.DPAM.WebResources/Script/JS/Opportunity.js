/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Opportunity;
            (function (Opportunity) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-335 to add filter & SHER-368 to remove the filter so keep it as commented in case of.
                        //Form.setChannelsFilter(formContext);
                        //SHER-335
                        Form.manageCompetitiveBiddingVisibility(formContext);
                        //SHER-521
                        Form.addOpportunityProductSubgridEventListener(formContext);
                    }
                    static quickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-335 to add filter & SHER-368 to remove the filter so keep it as commented in case of.
                        //Form.setChannelsFilter(formContext);
                    }
                    static onChange_dpam_os_opportunitydepartment(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-335 to add filter & SHER-368 to remove the filter so keep it as commented in case of.
                        //Form.setChannelsFilter(formContext);
                        //SHER-335
                        Form.manageCompetitiveBiddingVisibility(formContext);
                    }
                    static onChange_dpam_os_probability(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-368
                        Form.setRating(formContext);
                    }
                    //set the Rating based on the Probability
                    static setRating(formContext) {
                        let probabilityValue = formContext.getAttribute("dpam_os_probability").getValue();
                        if (probabilityValue == 100000000 || probabilityValue == 100000001 || probabilityValue == 100000002) { // 0% - 10% - 25%
                            formContext.getAttribute("opportunityratingcode").setValue(3); // Cold
                        }
                        else if (probabilityValue == 100000003) { // 50%
                            formContext.getAttribute("opportunityratingcode").setValue(2); // Warm
                        }
                        else if (probabilityValue == 100000004 || probabilityValue == 100000005 || probabilityValue == 100000006) { // 75% - 90% - 100%
                            formContext.getAttribute("opportunityratingcode").setValue(1); // Hot
                        }
                    }
                    //set the filter on dpam_os_channels field
                    static setChannelsFilter(formContext) {
                        let opportunityDepartmentValue = formContext.getAttribute("dpam_os_opportunitydepartment").getValue();
                        let channelControl = formContext.getControl("dpam_os_channels");
                        let channelAttribute = formContext.getAttribute("dpam_os_channels");
                        let channelOptions = channelAttribute.getOptions();
                        channelControl.clearOptions();
                        channelOptions.forEach(function (option) {
                            //Opportunity Deparment = Distributor && channel = Mandate or Distribution
                            if (opportunityDepartmentValue == 100000000 && (option.value == 100000000 || option.value == 100000001)) {
                                channelControl.addOption(option);
                            }
                            //Opportunity Department = Institutional && channel = Investment Consultant or Client or Tender
                            else if (opportunityDepartmentValue == 100000001 && (option.value == 100000002 || option.value == 100000003 || option.value == 100000004)) {
                                channelControl.addOption(option);
                            }
                        });
                    }
                    //manage the visibility of dpam_b_competitivebidding field
                    static manageCompetitiveBiddingVisibility(formContext) {
                        let opportunityDepartmentValue = formContext.getAttribute("dpam_os_opportunitydepartment").getValue();
                        let competitiveBiddingIsVisible = opportunityDepartmentValue == 100000001; //Institutional
                        formContext.getControl("dpam_b_competitivebidding").setVisible(competitiveBiddingIsVisible);
                    }
                    //add an event on the onload of the opportunity product subgrid
                    static addOpportunityProductSubgridEventListener(formContext) {
                        let opportunityProductSubgrid = formContext.getControl("Subgrid_OpportunityProduct");
                        if (opportunityProductSubgrid == null) {
                            setTimeout(function () { Form.addOpportunityProductSubgridEventListener(formContext); }, 500);
                            return;
                        }
                        opportunityProductSubgrid.addOnLoad(Form.refreshRibbonOfOpportunityProductSubgrid);
                    }
                    //refresh the ribbon of the opportunity product subgrid
                    static refreshRibbonOfOpportunityProductSubgrid(executionContext) {
                        const formContext = executionContext.getFormContext();
                        formContext.getControl("Subgrid_OpportunityProduct").refreshRibbon();
                    }
                }
                Opportunity.Form = Form;
            })(Opportunity = WR.Opportunity || (WR.Opportunity = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Opportunity.js.map