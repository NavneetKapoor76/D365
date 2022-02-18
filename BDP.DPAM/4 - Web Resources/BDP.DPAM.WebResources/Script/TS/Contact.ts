/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Contact {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-299
            Form.hideContactFromParentCustomerLookup(formContext);
            //SHER-275
            Form.setContactTitleFilter(formContext);
            //SHER-362
            Form.manageContactTitleVisibility(formContext);
            //SHER-767
            Form.manageAccessTodonotbulkemailField(formContext);
        }

        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-299
            Form.resetPhoneNumber(formContext, "mobilephone");
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
            //SHER-362
            Form.manageContactTitleVisibility(formContext);
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

        //Function to hide the "dpam_lk_contacttitle" field when the language is German
        static manageContactTitleVisibility(formContext: Xrm.FormContext) {
            let languageValue: number = formContext.getAttribute("dpam_os_language").getValue();
            let isContactTitleVisible: boolean = languageValue == 100000002; //German

            formContext.getControl<Xrm.Controls.StandardControl>("dpam_lk_contacttitle").setVisible(isContactTitleVisible);
        }

        //function to manage the donotbulkemail field when the user has the "DPAM - Event Administrator" or "DPAM - Marketing Professional - Business" security role
        static manageAccessTodonotbulkemailField(formContext: Xrm.FormContext): void {
            let currentUserRoles: string[] = Xrm.Utility.getGlobalContext().getUserRoles();
            let roleCondition: string = "";

            currentUserRoles.forEach(function (role: string) {
                roleCondition += `<condition attribute="roleid" operator="eq" value="{${role}}" />`
            })

            let fetchXml: string = `?fetchXml=<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
                                  <entity name="role">
                                    <attribute name="name" />
                                    <order attribute="name" descending="false" />
                                    <filter type="and">
                                      <filter type="or">
                                        ${roleCondition}
                                      </filter>
                                    </filter>
                                  </entity>
                                </fetch>`;

            Xrm.WebApi.retrieveMultipleRecords("role", fetchXml).then(
                function success(result) {
                    let isDisabledField = true;
                    for (let i = 0; i < result.entities.length; i++) {
                        let roleName: string = result.entities[i].name;

                        if (roleName == "DPAM - Event Administrator" || roleName == "DPAM - Marketing Professional - Business") isDisabledField = false;
                    }

                    formContext.getControl<Xrm.Controls.StandardControl>("donotbulkemail").setDisabled(isDisabledField);
                    
                },
                function (error) {
                    console.log(error.message);
                });
        }
    }

    export class Ribbon {
        /* SHER-970
         * function to open the deactivate contact custom page on the form
         */
        public static openDeactivateContactCustomPage(formContext: Xrm.FormContext) {
            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_deactivatecontactcustompage_57457",
                entityName: "contact",
                recordId: formContext.data.entity.getId()
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 560,
                height: 320,
                title: "Confirm Deactivation"
            };

            Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                .then(
                    function success() {
                        formContext.data.refresh(true);
                    },
                    function error() {
                        console.log(error);
                    });
        }
    }
}