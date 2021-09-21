/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {

    class _Static {
        readonly field = {
            dpam_country: {
                dpam_s_alpha2code: "dpam_s_alpha2code",
                dpam_s_vatformat: "dpam_s_vatformat",
                dpam_s_vatformatexample: "dpam_s_vatformatexample"
            },
            account: {
                dpam_lk_country: "dpam_lk_country",
                dpam_s_country_alpha2code: "dpam_s_country_alpha2code",
                dpam_s_vatnumber: "dpam_s_vatnumber",
                dpam_mos_counterpartytype: "dpam_mos_counterpartytype",
                dpam_lk_businesssegmentation: "dpam_lk_businesssegmentation",
                dpam_lk_counterpartymifidcategory: "dpam_lk_counterpartymifidcategory",
                dpam_lk_compliancesegmentation: "dpam_lk_compliancesegmentation"
            },
            dpam_settings: {
                dpam_s_value: "dpam_s_value"
            }
        };

    }
    export let Static = new _Static();


    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            this.setBusinessSegmentationFilter(executionContext);
            this.setComplianceSegmentationFilter(executionContext);
            this.setLocalBusinessSegmentationFilter(executionContext);
             // SHER-292
            this.manageBusinessSegmentationVisibility(executionContext);
        }

        public static onChange_dpam_lk_vatnumber(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            this.checkValidVATNumber(formContext);
        }

        // SHER-292
        public static onChange_dpam_lk_country(executionContext: Xrm.Events.EventContext) {
            this.manageBusinessSegmentationVisibility(executionContext);
        }

        //function to check if the VAT number in the account is valid based on the VAT format of the country.
        static checkValidVATNumber(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).clearNotification();

            let _VATNumber_attribute: Xrm.Page.StringAttribute = formContext.getAttribute<Xrm.Page.StringAttribute>(Static.field.account.dpam_s_vatnumber);

            if (_VATNumber_attribute.getValue()) {
                //Remove spaces & non alphanumeric caracters from the VAT
                let _VATNumberFormatted_attribute = _VATNumber_attribute.getValue().replace(/[^0-9a-zA-Z]/g, '');
                formContext.getAttribute(Static.field.account.dpam_s_vatnumber).setValue(_VATNumberFormatted_attribute);

                let _country_attribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>(Static.field.account.dpam_lk_country);

                if (_country_attribute.getValue() && _country_attribute.getValue()[0] && _country_attribute.getValue()[0].id) {
                    let _country_lookupvalue = _country_attribute.getValue()[0];
                    Xrm.WebApi.retrieveRecord(_country_lookupvalue.entityType, _country_lookupvalue.id, `?$select=${Static.field.dpam_country.dpam_s_vatformat}, ${Static.field.dpam_country.dpam_s_vatformatexample}`).then(
                        function success(result) {
                            let _VATFormatValue = result[Static.field.dpam_country.dpam_s_vatformat];
                            if (_VATFormatValue != null && !_VATNumberFormatted_attribute.match(_VATFormatValue)) {
                                formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).setNotification("The format isn't valid. Please use following format: " + result[Static.field.dpam_country.dpam_s_vatformatexample], "invalidFormat")
                            }
                        },
                        function (error) {
                            console.log(error.message);
                        }
                    );
                } else {
                    formContext.getControl<Xrm.Controls.StandardControl>(Static.field.account.dpam_s_vatnumber).setNotification("The country field is empty, no VAT number can be entered.", "countryEmpty")
                }
            }
        }

        //function to add a custom filter on the dpam_lk_compliancesegmentation field
        static filterComplianceSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();

            let filter = `<filter type="and" >
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="null" >
                              </condition>
                            </filter>`;

            let _dpam_lk_counterpartymifidcategory: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>(Static.field.account.dpam_lk_counterpartymifidcategory);
            if (_dpam_lk_counterpartymifidcategory != null && _dpam_lk_counterpartymifidcategory.getValue() != null) {
                let id: string = _dpam_lk_counterpartymifidcategory.getValue()[0].id;
                filter = `<filter type="and">
                              <condition attribute="dpam_lk_counterpartymifidcategory" operator="eq" value=" ${id}" >     
                              </condition>
                            </filter>`;
            }

            formContext.getControl<Xrm.Page.LookupControl>(Static.field.account.dpam_lk_compliancesegmentation).addCustomFilter(filter, "dpam_counterpartycompliancesegmentation");
        }


        //function to add a custom filter on the dpam_lk_businesssegmentation field
        static filterBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();

            let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;

            let _dpam_mos_counterpartytype: Xrm.Page.Attribute = formContext.getAttribute(Static.field.account.dpam_mos_counterpartytype);
            if (_dpam_mos_counterpartytype != null && _dpam_mos_counterpartytype.getValue() != null) {
                let selectedOptions: Int32Array = _dpam_mos_counterpartytype.getValue();
                let values = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                            </filter>`;
            }

            formContext.getControl<Xrm.Page.LookupControl>(Static.field.account.dpam_lk_businesssegmentation).addCustomFilter(filter, "dpam_counterpartybusinesssegmentation");
        }

        //function to set the filter on the dpam_lk_businesssegmentation field
        static setBusinessSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            let _dpam_lk_businesssegmentation_control: Xrm.Page.LookupControl = formContext.getControl(Static.field.account.dpam_lk_businesssegmentation);

            if (_dpam_lk_businesssegmentation_control != null) {
                _dpam_lk_businesssegmentation_control.addPreSearch(this.filterBusinessSegmentation);
            }
        }
        //function to set the filter on the dpam_lk_compliancesegmentation field
        static setComplianceSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            let _dpam_lk_compliancesegmentation_control: Xrm.Page.LookupControl = formContext.getControl(Static.field.account.dpam_lk_compliancesegmentation);

            if (_dpam_lk_compliancesegmentation_control != null) {
                _dpam_lk_compliancesegmentation_control.addPreSearch(this.filterComplianceSegmentation);
            }
        }
        // Opens the "Lei Code Search" Canvas app in a dialog based on the URL retrieved from the settings entity.
        static dialogCanvasApp() {
            let dialogOptions = { height: 815, width: 1350 };

            Xrm.WebApi.retrieveRecord("dpam_settings", "a53657d3-25f4-eb11-94ef-000d3a237027", `?$select=${Static.field.dpam_settings.dpam_s_value}`).then(
                function success(result) {
                    Xrm.Navigation.openUrl(result[Static.field.dpam_settings.dpam_s_value], dialogOptions);
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }

        //function to add a custom filter on the dpam_lk_localbusinesssegmentation field
        static filterLocalBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();

            let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;

            let _dpam_mos_counterpartytype: Xrm.Page.Attribute = formContext.getAttribute("dpam_mos_counterpartytype");
            let _dpam_lk_country: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");
            if (_dpam_mos_counterpartytype != null && _dpam_mos_counterpartytype.getValue() != null && _dpam_lk_country != null && _dpam_lk_country.getValue() != null) {
                let selectedOptions: Int32Array = _dpam_mos_counterpartytype.getValue();
                let values = "";

                selectedOptions.forEach(function (item) {
                    values += `<value>${item}</value>`;
                });

                filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                ${values}
                              </condition>
                              <condition attribute="dpam_lk_country" operator="eq" uitype="dpam_country" value="${_dpam_lk_country.getValue()[0].id}" />
                            </filter>`;
            }

            formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_localbusinesssegmentation").addCustomFilter(filter, "dpam_cplocalbusinesssegmentation");
        }

        //function to set the filter on the dpam_lk_localbusinesssegmentation field
        static setLocalBusinessSegmentationFilter(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            let _dpam_lk_localbusinesssegmentation_control: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");

            if (_dpam_lk_localbusinesssegmentation_control != null) {
                _dpam_lk_localbusinesssegmentation_control.addPreSearch(this.filterLocalBusinessSegmentation);
            }
        }


        static manageBusinessSegmentationVisibility(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            //retrieve the country of counterparty.
            let _dpam_lk_country: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_country");
            let _dpam_lk_localbusinesssegmentation_control: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_localbusinesssegmentation");
            let _dpam_lk_businesssegmentation_control: Xrm.Page.LookupControl = formContext.getControl("dpam_lk_businesssegmentation");
            _dpam_lk_localbusinesssegmentation_control.setVisible(false);
            _dpam_lk_businesssegmentation_control.setVisible(false);
            if (_dpam_lk_country != null && _dpam_lk_country.getValue() != null && _dpam_lk_country.getValue()[0] && _dpam_lk_country.getValue()[0].id) {
                var fetchXml = `?fetchXml=<fetch top="1"><entity name="dpam_cplocalbusinesssegmentation" ><attribute name="dpam_cplocalbusinesssegmentationid" /><filter><condition attribute="dpam_lk_country" operator="eq" value="${_dpam_lk_country.getValue()[0].id}" /></filter></entity></fetch>`;
                // search at least one occurence of this country in Local segmentation
                Xrm.WebApi.retrieveMultipleRecords("dpam_cplocalbusinesssegmentation", fetchXml).then(
                    function success(result) {
                        
                        if (result.entities.length > 0) {
                            // found
                            // if one found, set visible Local segmentation and hide generic segmentation  (fill generic segmentation)
                            _dpam_lk_localbusinesssegmentation_control.setVisible(true);
                            _dpam_lk_businesssegmentation_control.setVisible(false);
                        } else {
                            // nothing found
                            // if not found set visible generic segmentation and hide local segmentation (fill local to null)
                            _dpam_lk_localbusinesssegmentation_control.setVisible(false);
                            _dpam_lk_businesssegmentation_control.setVisible(true);
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