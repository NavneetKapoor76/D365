/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Contact;
            (function (Contact) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-299
                        Form.hideContactFromParentCustomerLookup(formContext);
                        //SHER-275
                        Form.setContactTitleFilter(formContext);
                    }
                    static QuickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-299
                        Form.resetPhoneNumber(formContext, "mobilephone");
                        //SHER-299
                        Form.resetPhoneNumber(formContext, "telephone1");
                        //SHER-299
                        Form.hideContactFromParentCustomerLookup(formContext);
                    }
                    static onChange_dpam_os_gender(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-275
                        Form.setContactTitleFilter(formContext);
                    }
                    static onChange_dpam_os_language(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-275
                        Form.setContactTitleFilter(formContext);
                    }
                    //function to reset the phone number
                    static resetPhoneNumber(formContext, fieldName) {
                        let phoneAttribute = formContext.getAttribute(fieldName);
                        if (phoneAttribute.getValue()) {
                            let value = phoneAttribute.getValue();
                            phoneAttribute.setValue(null);
                            phoneAttribute.setValue(value);
                        }
                    }
                    //function to keep only counterparty in the "parentcustomerid" lookup
                    static hideContactFromParentCustomerLookup(formContext) {
                        formContext.getControl("parentcustomerid").setEntityTypes(["account"]);
                    }
                    //function to set the filter on the "dpam_lk_contacttitle" field
                    static setContactTitleFilter(formContext) {
                        let languageAttribute = formContext.getAttribute("dpam_os_language");
                        let genderAttribute = formContext.getAttribute("dpam_os_gender");
                        if (languageAttribute.getValue() != null && genderAttribute.getValue() != null) {
                            formContext.getControl("dpam_lk_contacttitle").addPreSearch(Form.filterContactTitleLookup);
                        }
                        else {
                            formContext.getControl("dpam_lk_contacttitle").removePreSearch(Form.filterContactTitleLookup);
                        }
                    }
                    //function to add a custom filter on the "dpam_lk_contacttitle" field based on Contact language & gender
                    static filterContactTitleLookup(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let filter = `<filter type="and">
                             <condition attribute="dpam_os_gender" operator="eq" value="${formContext.getAttribute("dpam_os_gender").getValue()}" />
                             <condition attribute="dpam_os_language" operator="eq" value="${formContext.getAttribute("dpam_os_language").getValue()}" />
                          </filter>`;
                        formContext.getControl("dpam_lk_contacttitle").addCustomFilter(filter, "dpam_contacttitle");
                    }
                }
                Contact.Form = Form;
            })(Contact = WR.Contact || (WR.Contact = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Contact.js.map