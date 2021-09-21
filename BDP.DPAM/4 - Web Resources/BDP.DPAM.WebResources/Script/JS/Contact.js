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
                        var formContext = executionContext.getFormContext();
                        //SHER-299
                        this.hideContactFromParentCustomerLookup(formContext);
                        //SHER-275
                        this.setContactTitleFilter(formContext);
                    }
                    static QuickCreateonLoad(executionContext) {
                        var formContext = executionContext.getFormContext();
                        this.resetPhoneNumber(formContext, "mobilephone");
                        this.resetPhoneNumber(formContext, "telephone1");
                        //SHER-299
                        this.hideContactFromParentCustomerLookup(formContext);
                    }
                    //SHER-275
                    static onChange_dpam_os_gender(executionContext) {
                        const formContext = executionContext.getFormContext();
                        this.setContactTitleFilter(formContext);
                    }
                    //SHER-275
                    static onChange_dpam_os_language(executionContext) {
                        const formContext = executionContext.getFormContext();
                        this.setContactTitleFilter(formContext);
                    }
                    static resetPhoneNumber(formContext, fieldName) {
                        let phoneAttribute = formContext.getAttribute(fieldName);
                        if (phoneAttribute != null && phoneAttribute.getValue()) {
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
                        let _dpam_os_language = formContext.getAttribute("dpam_os_language");
                        let _dpam_os_gender = formContext.getAttribute("dpam_os_gender");
                        if (_dpam_os_language != null && _dpam_os_language.getValue() != null && _dpam_os_gender != null && _dpam_os_gender.getValue() != null) {
                            formContext.getControl("dpam_lk_contacttitle").addPreSearch(this.filterContactTitleLookup);
                        }
                        else {
                            formContext.getControl("dpam_lk_contacttitle").removePreSearch(this.filterContactTitleLookup);
                        }
                    }
                    //function to add a custom filter on the "dpam_lk_contacttitle" field based on Contact language & gender
                    static filterContactTitleLookup(executionContext) {
                        const formContext = executionContext.getFormContext();
                        var filter = `<filter type="and">
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