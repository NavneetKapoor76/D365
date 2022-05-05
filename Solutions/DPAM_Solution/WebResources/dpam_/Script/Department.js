/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Department;
            (function (Department) {
                //Variables used for the KYC-AML button
                let idCardStatusIsStarted = false;
                let isRefreshByKycAmlCustomPage = true;
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
                class Ribbon {
                    // SHER-1175 : function to open the KYC-AML Custom Page
                    static openKYCAMLCustomPage(formContext, buttonName, allMandatoryFieldsAreFilled) {
                        formContext.ui.clearFormNotification("KycAmlCustomPage");
                        let pageInput = {
                            pageType: "custom",
                            name: "dpam_kycamlcustompage_c2710",
                            entityName: "dpam_departments",
                            recordId: formContext.data.entity.getId()
                        };
                        let navigationOptions = {
                            target: 2,
                            width: 1151,
                            height: 850,
                            title: "KYC AML Page"
                        };
                        if (allMandatoryFieldsAreFilled) {
                            Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                                .then(function success() {
                                isRefreshByKycAmlCustomPage = true;
                                formContext.data.refresh(true);
                                formContext.ui.refreshRibbon();
                            }, function error() {
                                console.log(error);
                            });
                        }
                        else {
                            let message = buttonName + " : Thanks to first fill the mandatory fields in the main form of the counterparty/department";
                            formContext.ui.setFormNotification(message, "ERROR", "KycAmlCustomPage");
                        }
                    }
                    //SHER-1175 : Check if all mandatory field are filled for the identification card and open the KYC-AML custom page
                    static allMandatoryFieldsAreFilledForIdentificationCard(formContext, buttonName) {
                        let counterpartyAttribute = formContext.getAttribute("dpam_lk_counterparty");
                        let departmentNameAttribute = formContext.getAttribute("dpam_s_name");
                        let addressAttribute = formContext.getAttribute("dpam_lk_location");
                        let complianceSegmentationAttribute = formContext.getAttribute("dpam_lk_compliancesegmentation");
                        if (counterpartyAttribute.getValue() == null || departmentNameAttribute.getValue() == null
                            || addressAttribute.getValue() == null || complianceSegmentationAttribute.getValue() == null) {
                            Ribbon.openKYCAMLCustomPage(formContext, buttonName, false);
                            return;
                        }
                        let options = `?$select=name, _dpam_lk_legalform_value, _dpam_lk_country_value`;
                        Xrm.WebApi.retrieveRecord("account", counterpartyAttribute.getValue()[0].id, options).then(function success(result) {
                            let counterpartyNameAttribute = result["name"];
                            let legalFormGuid = result["_dpam_lk_legalform_value"];
                            let countryGuid = result["_dpam_lk_country_value"];
                            let allMandatoryFieldsAreFilled = true;
                            if (counterpartyNameAttribute == null || legalFormGuid == null || countryGuid == null)
                                allMandatoryFieldsAreFilled = false;
                            Ribbon.openKYCAMLCustomPage(formContext, buttonName, allMandatoryFieldsAreFilled);
                        }, function (error) {
                            console.log(error.message);
                            // handle error conditions
                        });
                    }
                    //Manage the visibility of the 'Start KYC-AML' and 'Continue KYC-AML' button
                    static startOrContinueKycAmlButtonVisibility(formContext) {
                        let counterpartyAttribute = formContext.getAttribute("dpam_lk_counterparty");
                        if (counterpartyAttribute.getValue() == null)
                            false;
                        if (!isRefreshByKycAmlCustomPage)
                            return idCardStatusIsStarted;
                        let options = `?$select= dpam_os_idcardstatus`;
                        Xrm.WebApi.retrieveRecord("account", counterpartyAttribute.getValue()[0].id, options).then(function success(result) {
                            idCardStatusIsStarted = result.dpam_os_idcardstatus == 100000000;
                            isRefreshByKycAmlCustomPage = false;
                            formContext.ui.refreshRibbon();
                        }, function (error) {
                            console.log(error.message);
                            // handle error conditions
                        });
                        return idCardStatusIsStarted;
                    }
                }
                Department.Ribbon = Ribbon;
            })(Department = WR.Department || (WR.Department = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Department.js.map