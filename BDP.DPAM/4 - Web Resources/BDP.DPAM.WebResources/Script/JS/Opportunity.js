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
                        //SHER-335
                        Form.setChannelsFilter(formContext);
                        //SHER-335
                        Form.manageCompetitiveBiddingVisibility(formContext);
                    }
                    static quickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-335
                        Form.setChannelsFilter(formContext);
                    }
                    static onChange_dpam_os_opportunitydepartment(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-335
                        Form.setChannelsFilter(formContext);
                        //SHER-335
                        Form.manageCompetitiveBiddingVisibility(formContext);
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
                }
                Opportunity.Form = Form;
            })(Opportunity = WR.Opportunity || (WR.Opportunity = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Opportunity.js.map