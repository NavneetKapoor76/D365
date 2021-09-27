/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Contact {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-299
            Form.hideContactFromParentCustomerLookup(formContext);
            //SHER-275
            Form.setContactTitleFilter(formContext);
        }

        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-299
            Form.resetPhoneNumber(formContext, "mobilephone");
            //SHER-299
            Form.resetPhoneNumber(formContext, "telephone1");
            //SHER-299
            Form.hideContactFromParentCustomerLookup(formContext);
        }

        public static onChange_dpam_os_gender(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-275
            Form.setContactTitleFilter(formContext);
        }

        public static onChange_dpam_os_language(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-275
            Form.setContactTitleFilter(formContext);
        }

        //function to reset the phone number
        static resetPhoneNumber(formContext: Xrm.FormContext, fieldName: string) {
            let phoneAttribute: Xrm.Page.Attribute = formContext.getAttribute(fieldName);

            if (phoneAttribute.getValue()) {

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
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_contacttitle").addPreSearch(Form.filterContactTitleLookup);
        }

        //function to add a custom filter on the "dpam_lk_contacttitle" field based on Contact language & gender
        static filterContactTitleLookup(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let languageAttribute: Xrm.Page.OptionSetAttribute = formContext.getAttribute("dpam_os_language");
            let genderAttribute: Xrm.Page.OptionSetAttribute = formContext.getAttribute("dpam_os_gender");

            if (languageAttribute.getValue() != null && genderAttribute.getValue() != null) {

                let filter: string = `<filter type="and">
                             <condition attribute="dpam_os_gender" operator="eq" value="${genderAttribute.getValue()}" />
                             <condition attribute="dpam_os_language" operator="eq" value="${languageAttribute.getValue()}" />
                          </filter>`;

                formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_contacttitle").addCustomFilter(filter, "dpam_contacttitle");
            }
        }
    }
}