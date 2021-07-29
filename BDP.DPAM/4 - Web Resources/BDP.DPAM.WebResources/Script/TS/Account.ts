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
                dpam_os_counterpartytype: "dpam_os_counterpartytype",
                dpam_lk_businesssegmentation: "dpam_lk_businesssegmentation"
            }
        };

    }
    export let Static = new _Static();


    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            this.setBusinessSegmentationFilter(executionContext);
        }

        public static onChange_dpam_lk_vatnumber(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            this.checkValidVATNumber(formContext);
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

        //function to add a custom filter on the dpam_lk_businesssegmentation field
        static filterBusinessSegmentation(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();            

            let filter = `<filter type="and" >
                              <condition attribute="dpam_mos_counterpartytype" operator="null" >
                              </condition>
                            </filter>`;

            let _dpam_os_counterpartytype: Xrm.Page.Attribute = formContext.getAttribute(Static.field.account.dpam_os_counterpartytype);
            if (_dpam_os_counterpartytype != null && _dpam_os_counterpartytype.getValue() != null) {
                filter = `<filter type="and">
                              <condition attribute="dpam_mos_counterpartytype" operator="contain-values">
                                <value>${_dpam_os_counterpartytype.getValue()}</value>
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

    }
}