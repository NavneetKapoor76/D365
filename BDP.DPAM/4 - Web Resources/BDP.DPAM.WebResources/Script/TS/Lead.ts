/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Lead {

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-331
            Form.setBusinessSegmentationFilter(formContext);
            //SHER-331
            Form.setLocalBusinessSegmentationFilter(formContext);
            //SHER-331
            Form.manageBusinessSegmentationVisibility(formContext);
        }

        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-331
            Form.manageBusinessSegmentationVisibility(formContext);
        }

        //function to add a custom filter on the dpam_lk_businesssegmentation field
        static filterBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let counterpartyTypeAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_counterpartytype");
            if (counterpartyTypeAttribute.getValue() != null) {
                let selectedOptions: Int32Array = counterpartyTypeAttribute.getValue();
                let values: string = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                let filter: string = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                            </filter>`;

                formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_businesssegmentation").addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
            }
        }

        //function to set the filter on the dpam_lk_businesssegmentation field
        static setBusinessSegmentationFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_businesssegmentation").addPreSearch(Form.filterBusinessSegmentation);
        }

        //function to add a custom filter on the dpam_lk_localbusinesssegmentation field
        static filterLocalBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let counterpartyTypeAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_counterpartytype");
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");

            if (counterpartyTypeAttribute.getValue() != null && countryAttribute.getValue() != null) {
                let selectedOptions: Int32Array = counterpartyTypeAttribute.getValue();
                let values: string = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                let filter: string = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                              <condition attribute="dpam_lk_country" operator="eq" uitype="dpam_country" value="${countryAttribute.getValue()[0].id}" />
                            </filter>`;

                formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_localbusinesssegmentation").addCustomFilter(filter, "dpam_cplocalbusinesssegmentation");
            }
        }

        //function to set the filter on the dpam_lk_localbusinesssegmentation field
        static setLocalBusinessSegmentationFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_localbusinesssegmentation").addPreSearch(Form.filterLocalBusinessSegmentation);
        }

        //function to set the visibility of the following fields: dpam_lk_localbusinesssegmentation, dpam_lk_businesssegmentation
        static manageBusinessSegmentationVisibility(formContext: Xrm.FormContext) {
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");

            if (countryAttribute.getValue() == null) return;

            let localbusinessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
            let businessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_businesssegmentation");

            let fetchXml: string = `?fetchXml=<fetch top="1">
                                        <entity name="dpam_cplocalbusinesssegmentation" >
                                            <attribute name="dpam_cplocalbusinesssegmentationid" />
                                        <filter>
                                            <condition attribute="dpam_lk_country" operator="eq" value="${countryAttribute.getValue()[0].id}" />
                                        </filter></entity></fetch>`;

            Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(
                function success(result) {
                    let localBusinessSegmentationIsVisible: boolean = false;

                    if (result.entities.length > 0) localBusinessSegmentationIsVisible = true;

                    localbusinessSegmentationControl.setVisible(localBusinessSegmentationIsVisible);
                    businessSegmentationControl.setVisible(!localBusinessSegmentationIsVisible);
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }
    }

    export class Ribbon {
        //function to open the disqualify lead custom page
        public static openDisqualifyLeadCustomPage() {
            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_disqualifyleadcustompage_3731d",
                entityName: "lead",
                recordId: Xrm.Page.data.entity.getId()
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 530,
                height: 350,
                title: "Disqualification"
            };

            Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                .then(
                    function success() {
                        Xrm.Page.data.refresh(true);
                    },
                    function error() {
                        console.log(error);
                    });
        }
    }
}