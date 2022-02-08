/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Department {

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-324
            Form.manageBusinessSegmentationVisibility(formContext);
            //SHER-736
            Form.manageRequiredLevelBasedOnCounterpartyType(formContext);
        }

        public static quickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-324
            Form.manageBusinessSegmentationVisibility(formContext);
            //SHER-736
            Form.manageRequiredLevelBasedOnCounterpartyType(formContext);
        }

        public static onChange_dpam_lk_counterparty(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-324
            Form.manageBusinessSegmentationVisibility(formContext);
        }

        public static onChange_dpam_mos_departmenttype(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-736
            Form.manageRequiredLevelBasedOnCounterpartyType(formContext);
        }

        //function to set the visibility of the following fields: dpam_lk_localbusinesssegmentation, dpam_lk_businesssegmentation
        static manageBusinessSegmentationVisibility(formContext: Xrm.FormContext) {
            let counterPartyAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_counterparty");
            let localbusinessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
            let businessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_businesssegmentation");

            if (counterPartyAttribute.getValue() == null) return;

            let counterPartyValue: Xrm.LookupValue = counterPartyAttribute.getValue()[0];

            Xrm.WebApi.retrieveRecord(counterPartyValue.entityType, counterPartyValue.id, "?$expand=dpam_lk_country($select=dpam_countryid)")
                .then(
                    function success(counterParty) {
                        if (counterParty.dpam_lk_country == null) return null;

                        let fetchXml: string = `?fetchXml=<fetch top="1">
                                            <entity name="dpam_cplocalbusinesssegmentation" >
                                                <attribute name="dpam_cplocalbusinesssegmentationid" />
                                            <filter>
                                                <condition attribute="dpam_lk_country" operator="eq" value="${counterParty.dpam_lk_country.dpam_countryid}" />
                                            </filter></entity></fetch>`;
                        return Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml)
                    },
                    function (error) {
                        console.log(error.message);
                    })
                .then(
                    function success(localBusinessSegmentationCollection) {
                        let localBusinessSegmentationIsVisible: boolean = false;

                        if (localBusinessSegmentationCollection != null && localBusinessSegmentationCollection.entities.length > 0) localBusinessSegmentationIsVisible = true;

                        localbusinessSegmentationControl.setVisible(localBusinessSegmentationIsVisible);
                        businessSegmentationControl.setVisible(!localBusinessSegmentationIsVisible);
                    },
                    function (error) {
                        console.log(error.message);
                    });
        }

        //Manage the required level based on the counterparty type for dpam_lk_mifidcategory
        static manageRequiredLevelBasedOnCounterpartyType(formContext: Xrm.FormContext) {
            let departmentTypeAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_departmenttype");
            let mifidCategoryRequiredLevel: Xrm.Attributes.RequirementLevel = "none";

            if (departmentTypeAttribute.getValue() != null) {
                let selectedOptions: Int32Array = departmentTypeAttribute.getValue();

                if (selectedOptions.length == 1 && selectedOptions[0] == 100000000 /*Client*/) {
                    mifidCategoryRequiredLevel = "required";
                }
            }

            formContext.getAttribute("dpam_lk_mifidcategory").setRequiredLevel(mifidCategoryRequiredLevel);

        }

    }
}