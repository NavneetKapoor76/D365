/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {
    
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            //SHER-174
            Form.setBusinessSegmentationFilter(executionContext);
            //SHER-244
            Form.setComplianceSegmentationFilter(executionContext);
            //SHER-268
            Form.setLocalBusinessSegmentationFilter(executionContext);
             // SHER-292
            Form.manageBusinessSegmentationVisibility(executionContext);
        }

        public static onChange_dpam_lk_vatnumber(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-92
            Form.checkValidVATNumber(formContext);
        }

        // SHER-292
        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            //SHER-292
            Form.manageBusinessSegmentationVisibility(executionContext);
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

            let filter: string = `<filter type="and" >
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="null" >
                              </condition>
                            </filter>`;

            let cpMifidCategoryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_counterpartymifidcategory");
            if (cpMifidCategoryAttribute.getValue() != null) {
                let cpMifidCategoryId: string = cpMifidCategoryAttribute.getValue()[0].id;
                filter = `<filter type="and">
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="eq" value=" ${cpMifidCategoryId}" >     
                              </condition>
                            </filter>`;
            }

            formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_compliancesegmentation").addCustomFilter(filter, "dpam_counterpartycompliancesegmentation");
        }


        //function to add a custom filter on the dpam_lk_businesssegmentation field
        static filterBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let filter: string = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;

            let counterpartyTypeAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_counterpartytype");
            if (counterpartyTypeAttribute.getValue() != null) {
                let selectedOptions: Int32Array = counterpartyTypeAttribute.getValue();
                let values: string = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                            </filter>`;
            }

            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_businesssegmentation").addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
        }

        //function to set the filter on the dpam_lk_businesssegmentation field
        static setBusinessSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_businesssegmentation").addPreSearch(Form.filterBusinessSegmentation);
        }
        //function to set the filter on the dpam_lk_compliancesegmentation field
        static setComplianceSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

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

            let filter: string = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;

            let counterpartyTypeAttribute: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_counterpartytype");
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");

            if (counterpartyTypeAttribute.getValue() != null && countryAttribute.getValue() != null) {
                let selectedOptions: Int32Array = counterpartyTypeAttribute.getValue();
                let values: string = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                              <condition attribute="dpam_lk_country" operator="eq" uitype="dpam_country" value="${countryAttribute.getValue()[0].id}" />
                            </filter>`;
            }

            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_localbusinesssegmentation").addCustomFilter(filter, "dpam_cplocalbusinesssegmentation");
        }

        //function to set the filter on the dpam_lk_localbusinesssegmentation field
        static setLocalBusinessSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_localbusinesssegmentation").addPreSearch(Form.filterLocalBusinessSegmentation);
        }

        //function to set the visibility of the following fields: dpam_lk_localbusinesssegmentation, dpam_lk_businesssegmentation
        static manageBusinessSegmentationVisibility(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //retrieve the country of counterparty.
            let countryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");
            let localbusinessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
            let businessSegmentationControl: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_businesssegmentation");

            localbusinessSegmentationControl.setVisible(false);
            businessSegmentationControl.setVisible(false);

            if (countryAttribute.getValue() != null && countryAttribute.getValue()[0] && countryAttribute.getValue()[0].id) {
                var fetchXml = `?fetchXml=<fetch top="1"><entity name="dpam_cplocalbusinesssegmentation" ><attribute name="dpam_cplocalbusinesssegmentationid" /><filter><condition attribute="dpam_lk_country" operator="eq" value="${countryAttribute.getValue()[0].id}" /></filter></entity></fetch>`;
                // search at least one occurence of this country in Local segmentation
                Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(
                    function success(result) {
                        
                        if (result.entities.length > 0) {
                            // found
                            // if one found, set visible Local segmentation and hide generic segmentation  (fill generic segmentation)
                            localbusinessSegmentationControl.setVisible(true);
                            businessSegmentationControl.setVisible(false);
                        } else {
                            // nothing found
                            // if not found set visible generic segmentation and hide local segmentation (fill local to null)
                            localbusinessSegmentationControl.setVisible(false);
                            businessSegmentationControl.setVisible(true);
                        }
                    },
                    function (error) {
                        console.log(error.message);
                        // handle error conditions
                    }
                );
            } 
        }

       

       

    }
}