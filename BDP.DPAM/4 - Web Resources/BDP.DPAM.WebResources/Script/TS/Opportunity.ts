/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Opportunity {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            //SHER-335
            Form.setChannelsFilter(formContext);
            //SHER-335
            Form.manageCompetitiveBiddingVisibility(formContext);

        }

        public static quickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            //SHER-335
            Form.setChannelsFilter(formContext);
        }

        public static onChange_dpam_os_opportunitydepartment(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            //SHER-335
            Form.setChannelsFilter(formContext);
            //SHER-335
            Form.manageCompetitiveBiddingVisibility(formContext);
        }

        //set the filter on dpam_os_channels field
        static setChannelsFilter(formContext: Xrm.FormContext) {
            let opportunityDepartmentValue: number = formContext.getAttribute("dpam_os_opportunitydepartment").getValue();

            let channelControl: Xrm.Page.OptionSetControl = formContext.getControl("dpam_os_channels");
            let channelAttribute: Xrm.Page.OptionSetAttribute = formContext.getAttribute("dpam_os_channels");
            let channelOptions: Xrm.OptionSetValue[] = channelAttribute.getOptions();
            channelControl.clearOptions();

            channelOptions.forEach(function (option: Xrm.OptionSetValue) {
                //Opportunity Deparment = Distributor && channel = Mandate or Distribution
                if (opportunityDepartmentValue == 100000000 && (option.value == 100000000 || option.value == 100000001)) {
                    channelControl.addOption(option);
                }
                //Opportunity Department = Institutional && channel = Investment Consultant or Client or Tender
                else if (opportunityDepartmentValue == 100000001 && (option.value == 100000002 || option.value == 100000003 || option.value == 100000004)) {
                    channelControl.addOption(option);
                }
            })
        }

        //manage the visibility of dpam_b_competitivebidding field
        static manageCompetitiveBiddingVisibility(formContext: Xrm.FormContext) {
            let opportunityDepartmentValue: number = formContext.getAttribute("dpam_os_opportunitydepartment").getValue();

            let competitiveBiddingIsVisible: boolean = opportunityDepartmentValue == 100000001; //Institutional
            formContext.getControl<Xrm.Controls.StandardControl>("dpam_b_competitivebidding").setVisible(competitiveBiddingIsVisible);
        }
    }
}