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
                        //SHER-362
                        Form.manageContactTitleVisibility(formContext);
                    }
                    static QuickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-299
                        Form.resetPhoneNumber(formContext, "mobilephone");
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
                        //SHER-362
                        Form.manageContactTitleVisibility(formContext);
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
                        formContext.getControl("dpam_lk_contacttitle").addPreSearch(Form.filterContactTitleLookup);
                    }
                    //function to add a custom filter on the "dpam_lk_contacttitle" field based on Contact language & gender
                    static filterContactTitleLookup(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let languageAttribute = formContext.getAttribute("dpam_os_language");
                        let genderAttribute = formContext.getAttribute("dpam_os_gender");
                        if (languageAttribute.getValue() != null && genderAttribute.getValue() != null) {
                            let filter = `<filter type="and">
                             <condition attribute="dpam_os_gender" operator="eq" value="${genderAttribute.getValue()}" />
                             <condition attribute="dpam_os_language" operator="eq" value="${languageAttribute.getValue()}" />
                          </filter>`;
                            formContext.getControl("dpam_lk_contacttitle").addCustomFilter(filter, "dpam_contacttitle");
                        }
                    }
                    //Function to hide the "dpam_lk_contacttitle" field when the language is German
                    static manageContactTitleVisibility(formContext) {
                        let languageValue = formContext.getAttribute("dpam_os_language").getValue();
                        let isContactTitleVisible = languageValue == 100000002; //German
                        formContext.getControl("dpam_lk_contacttitle").setVisible(isContactTitleVisible);
                    }
                }
                Contact.Form = Form;
                class Ribbon {
                    /* SHER-970
                     * function to open the deactivate contact custom page on the form
                     */
                    static openDeactivateContactCustomPage(formContext) {
                        let pageInput = {
                            pageType: "custom",
                            name: "dpam_deactivatecontactcustompage_57457",
                            entityName: "contact",
                            recordId: formContext.data.entity.getId()
                        };
                        let navigationOptions = {
                            target: 2,
                            width: 560,
                            height: 320,
                            title: "Confirm Deactivation"
                        };
                        Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                            .then(function success() {
                            formContext.data.refresh(true);
                        }, function error() {
                            console.log(error);
                        });
                    }
                }
                Contact.Ribbon = Ribbon;
            })(Contact = WR.Contact || (WR.Contact = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Contact.js.map