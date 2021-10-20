/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-174
            Form.setBusinessSegmentationFilter(formContext);
            //SHER-244
            Form.setComplianceSegmentationFilter(formContext);
            //SHER-268
            Form.setLocalBusinessSegmentationFilter(formContext);
            //SHER-292 + SHER-426
            Form.manageBusinessSegmentationVisibility(formContext);
            //SHER-313
            Form.manageCountryVisibility(formContext);
        }

        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            //SHER-344 All same than form load except SHER-313 Country visibility
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-174
            Form.setBusinessSegmentationFilter(formContext);
            //SHER-244
            Form.setComplianceSegmentationFilter(formContext);
            //SHER-268
            Form.setLocalBusinessSegmentationFilter(formContext);
            //SHER-292 + SHER-426
            Form.manageBusinessSegmentationVisibility(formContext);

            formContext.getAttribute("dpam_lk_country").setRequiredLevel("required");
        }

        public static onChange_dpam_lk_vatnumber(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-92
            Form.checkValidVATNumber(formContext);
        }

        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-292 + SHER-426
            Form.manageBusinessSegmentationVisibility(formContext);
            Form.resetSegmentation(formContext);

        }

        static resetSegmentation(formContext: Xrm.FormContext) {
            let localBusinessAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_lk_localbusinesssegmentation");
            let businessAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_lk_businesssegmentation");
            localBusinessAttribute.setValue(null);
            businessAttribute.setValue(null);

        }

        //function to check if the VAT number in the account is valid based on the VAT format of the country.
        static checkValidVATNumber(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.StandardControl>("dpam_s_vatnumber").clearNotification();

            let VATNumberAttribute: Xrm.Page.StringAttribute = formContext.getAttribute<Xrm.Page.StringAttribute>("dpam_s_vatnumber");

            if (VATNumberAttribute.getValue()) {
                //Remove spaces & non alphanumeric caracters from the VAT
                let VATNumberFormatted: string = VATNumberAttribute.getValue().replace(/[^0-9a-zA-Z]/g, '');
                formContext.getAttribute("dpam_s_vatnumber").setValue(VATNumberFormatted);

                let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_country");

                if (countryAttribute.getValue() && countryAttribute.getValue()[0] && countryAttribute.getValue()[0].id) {
                    let countryLookupvalue: Xrm.LookupValue = countryAttribute.getValue()[0];
                    Xrm.WebApi.retrieveRecord(countryLookupvalue.entityType, countryLookupvalue.id, `?$select=dpam_s_vatformat, dpam_s_vatformatexample`).then(
                        function success(result) {
                            let VATFormatValue = result["dpam_s_vatformat"];
                            if (VATFormatValue != null && !VATNumberFormatted.match(VATFormatValue)) {
                                formContext.getControl<Xrm.Controls.StandardControl>("dpam_s_vatnumber").setNotification("The format isn't valid. Please use following format: " + result["dpam_s_vatformatexample"], "invalidFormat")
                            }
                        },
                        function (error) {
                            console.log(error.message);
                        }
                    );
                } else {
                    formContext.getControl<Xrm.Controls.StandardControl>("dpam_s_vatnumber").setNotification("The country field is empty, no VAT number can be entered.", "countryEmpty")
                }
            }
        }

        //function to add a custom filter on the dpam_lk_compliancesegmentation field
        static filterComplianceSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let cpMifidCategoryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_counterpartymifidcategory");
            if (cpMifidCategoryAttribute.getValue() != null) {
                let cpMifidCategoryId: string = cpMifidCategoryAttribute.getValue()[0].id;
                let filter: string = `<filter type="and">
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="eq" value=" ${cpMifidCategoryId}" >     
                              </condition>
                            </filter>`;

                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_compliancesegmentation").addCustomFilter(filter, "dpam_counterpartycompliancesegmentation");
            }
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
        //function to set the filter on the dpam_lk_compliancesegmentation field
        static setComplianceSegmentationFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_compliancesegmentation").addPreSearch(Form.filterComplianceSegmentation);
        }
        // Opens the "Lei Code Search" Canvas app in a dialog based on the URL retrieved from the settings entity.
        static dialogCanvasApp() {
            let dialogOptions = { height: 815, width: 1350 };

            Xrm.WebApi.retrieveRecord("dpam_settings", "a53657d3-25f4-eb11-94ef-000d3a237027", `?$select=dpam_s_value`).then(
                function success(result) {
                    Xrm.Navigation.openUrl(result["dpam_s_value"], dialogOptions);
                },
                function (error) {
                    console.log(error.message);
                }
            );
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

        //function to set the visibility of the dpam_lk_localbusinesssegmentation field and enable/disable the dpam_lk_businesssegmentation field
        static manageBusinessSegmentationVisibility(formContext: Xrm.FormContext) {
            //retrieve the country of counterparty.
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");
            let localbusinessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
            let businessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_businesssegmentation");
            
            if (countryAttribute.getValue() != null && countryAttribute.getValue()[0] && countryAttribute.getValue()[0].id) {
                let fetchXml: string = `?fetchXml=<fetch top="1"><entity name="dpam_cplocalbusinesssegmentation" ><attribute name="dpam_cplocalbusinesssegmentationid" /><filter><condition attribute="dpam_lk_country" operator="eq" value="${countryAttribute.getValue()[0].id}" /></filter></entity></fetch>`;
                // search at least one occurence of this country in Local segmentation
                Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(
                    function success(result) {
                        let localBusinessSegmentationIsVisible: boolean = false;

                        // if one found, set visible Local segmentation and disable generic segmentation  (fill generic segmentation)
                        if (result.entities.length > 0) localBusinessSegmentationIsVisible = true;
                        
                        localbusinessSegmentationControl.setVisible(localBusinessSegmentationIsVisible);
                        businessSegmentationControl.setDisabled(localBusinessSegmentationIsVisible);
                    },
                    function (error) {
                        console.log(error.message);
                        // handle error conditions
                    }
                );
            }
        }

        // On creation of counterparty, country must be mandatory & visible in order to have the "local business segmentation" pre-filtered
        static manageCountryVisibility(formContext: Xrm.FormContext) {
            //Check if it is create mode
            if (formContext.ui.getFormType() == 1) {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_country").setVisible(true);
                formContext.getAttribute("dpam_lk_country").setRequiredLevel("required");
            } else {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_country").setVisible(false);
            }
        }
    }

    export class Ribbon {

        //SHER-428 : function to open the "LEI Code Search Page" custom page
        public static openLEICodeSearchPage() {
            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_leicodesearchpage_1806a",
                entityName: "account"
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 1365,
                height: 815,
                title: "LEI Code Search Engine"
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

        // SHER-428 : function to open the "LEI Code Search Page" custom page in a record
        public static openLEICodeSearchPageOnForm() {
            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_leicodesearchpage_1806a",
                entityName: "account",
                recordId: Xrm.Page.data.entity.getId()
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 1365,
                height: 815,
                title: "LEI Code Search Engine"
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