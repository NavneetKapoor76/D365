/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Contact {

    class _Static {
        readonly field = {
            contact: {
                mobilephone: "mobilephone",
                telephone1: "telephone1"
            }
        };

    }
    export let Static = new _Static();

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            var formContext = executionContext.getFormContext();

            //SHER-299
            this.hideContactFromParentCustomerLookup(formContext);

            //SHER-275
            this.setContactTitleFilter(formContext);
        }

        //SHER-275
        public static onChange_dpam_os_gender(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            this.setContactTitleFilter(formContext);
        }

        //SHER-275
        public static onChange_dpam_os_language(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();
            this.setContactTitleFilter(formContext);
        }

        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            this.resetPhoneNumber(executionContext, Static.field.contact.mobilephone);
            this.resetPhoneNumber(executionContext, Static.field.contact.telephone1);
        }

        static resetPhoneNumber(executionContext: Xrm.Events.EventContext, fieldName: string) {
            const formContext = executionContext.getFormContext();
            let phoneAttribute: Xrm.Page.Attribute = formContext.getAttribute(fieldName);

            if (phoneAttribute != null && phoneAttribute.getValue()) {

                let value: string = phoneAttribute.getValue();
                phoneAttribute.setValue(null);
                phoneAttribute.setValue(value);
            }       
        }

        //function to keep only counterparty in the "parentcustomerid" lookup
        static hideContactFromParentCustomerLookup(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("parentcustomerid").setEntityTypes(["account"]);
        }

        //function to set the filter on the "dpam_lk_contacttitle" field
        static setContactTitleFilter(formContext: Xrm.FormContext) {
            let _dpam_os_language: Xrm.Page.OptionSetAttribute = formContext.getAttribute("dpam_os_language");
            let _dpam_os_gender: Xrm.Page.OptionSetAttribute = formContext.getAttribute("dpam_os_gender");

            if (_dpam_os_language != null && _dpam_os_language.getValue() != null && _dpam_os_gender != null && _dpam_os_gender.getValue() != null) {
                formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_contacttitle").addPreSearch(this.filterContactTitleLookup);
            } else {
                formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_contacttitle").removePreSearch(this.filterContactTitleLookup);
            }
        }

        //function to add a custom filter on the "dpam_lk_contacttitle" field based on Contact language & gender
        static filterContactTitleLookup(executionContext: Xrm.Events.EventContext) {
            const formContext = executionContext.getFormContext();

            var filter = `<filter type="and">
                             <condition attribute="dpam_os_gender" operator="eq" value="${formContext.getAttribute("dpam_os_gender").getValue()}" />
                             <condition attribute="dpam_os_language" operator="eq" value="${formContext.getAttribute("dpam_os_language").getValue()}" />
                          </filter>`;
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_contacttitle").addCustomFilter(filter, "dpam_contacttitle");
        }
    }
}