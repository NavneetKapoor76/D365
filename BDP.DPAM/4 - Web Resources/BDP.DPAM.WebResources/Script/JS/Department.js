/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Department;
            (function (Department) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-324
                        Form.manageBusinessSegmentationVisibility(formContext);
                        //SHER-736
                        Form.manageRequiredLevelAndVisibilityBasedOnDepartmentType(formContext);
                    }
                    static quickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-324
                        Form.manageBusinessSegmentationVisibility(formContext);
                        //SHER-736
                        Form.manageRequiredLevelAndVisibilityBasedOnDepartmentType(formContext);
                    }
                    static onChange_dpam_lk_counterparty(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-324
                        Form.manageBusinessSegmentationVisibility(formContext);
                    }
                    static onChange_dpam_mos_departmenttype(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-736
                        Form.manageRequiredLevelAndVisibilityBasedOnDepartmentType(formContext);
                    }
                    //function to set the visibility of the following fields: dpam_lk_localbusinesssegmentation, dpam_lk_businesssegmentation
                    static manageBusinessSegmentationVisibility(formContext) {
                        let counterPartyAttribute = formContext.getAttribute("dpam_lk_counterparty");
                        let localbusinessSegmentationControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
                        let businessSegmentationControl = formContext.getControl("dpam_lk_businesssegmentation");
                        if (counterPartyAttribute.getValue() == null)
                            return;
                        let counterPartyValue = counterPartyAttribute.getValue()[0];
                        Xrm.WebApi.retrieveRecord(counterPartyValue.entityType, counterPartyValue.id, "?$expand=dpam_lk_country($select=dpam_countryid)")
                            .then(function success(counterParty) {
                            if (counterParty.dpam_lk_country == null)
                                return null;
                            let fetchXml = `?fetchXml=<fetch top="1">
                                            <entity name="dpam_cplocalbusinesssegmentation" >
                                                <attribute name="dpam_cplocalbusinesssegmentationid" />
                                            <filter>
                                                <condition attribute="dpam_lk_country" operator="eq" value="${counterParty.dpam_lk_country.dpam_countryid}" />
                                            </filter></entity></fetch>`;
                            return Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml);
                        }, function (error) {
                            console.log(error.message);
                        })
                            .then(function success(localBusinessSegmentationCollection) {
                            let localBusinessSegmentationIsVisible = false;
                            if (localBusinessSegmentationCollection != null && localBusinessSegmentationCollection.entities.length > 0)
                                localBusinessSegmentationIsVisible = true;
                            localbusinessSegmentationControl.setVisible(localBusinessSegmentationIsVisible);
                            businessSegmentationControl.setVisible(!localBusinessSegmentationIsVisible);
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                    /* Based on the department type, manage:
                    * - the required level for dpam_lk_mifidcategory and dpam_lk_compliancesegmentation
                    * - the visibility for dpam_lk_compliancesegmentation
                    */
                    static manageRequiredLevelAndVisibilityBasedOnDepartmentType(formContext) {
                        let departmentTypeAttribute = formContext.getAttribute("dpam_mos_departmenttype");
                        let mifidCategoryRequiredLevel = "none";
                        let complianceSegmentationRequiredLevel = "required";
                        let complianceSegmentationVisibility = true;
                        if (departmentTypeAttribute.getValue() != null) {
                            let selectedOptions = departmentTypeAttribute.getValue();
                            if (selectedOptions.length == 1) {
                                switch (selectedOptions[0]) {
                                    case 100000000: /*Client*/
                                        mifidCategoryRequiredLevel = "required";
                                        break;
                                    case 100000005: /*Business Relation*/
                                        complianceSegmentationVisibility = false;
                                        complianceSegmentationRequiredLevel = "none";
                                        break;
                                }
                            }
                        }
                        formContext.getAttribute("dpam_lk_mifidcategory").setRequiredLevel(mifidCategoryRequiredLevel);
                        let complianceSegmentationControl = formContext.getControl("dpam_lk_compliancesegmentation");
                        complianceSegmentationControl.setVisible(complianceSegmentationVisibility);
                        complianceSegmentationControl.getAttribute().setRequiredLevel(complianceSegmentationRequiredLevel);
                    }
                }
                Department.Form = Form;
            })(Department = WR.Department || (WR.Department = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Department.js.map