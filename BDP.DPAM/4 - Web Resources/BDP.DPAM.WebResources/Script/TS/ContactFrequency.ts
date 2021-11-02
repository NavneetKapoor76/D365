/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.ContactFrequency {

    //Variable global to know if the user has the "DPAM - Sales Person" security role
    let isSalesPerson: boolean;
    //Variable global to know if the user has the "DPAM - Sales Manager" security role
    let isSalesManager: boolean;
    //Variable global to know the value of "dpam_int_numberoftargetactivities" field
    let numberOfTargetActivitiesValue: number;

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-421
            Form.manageAccessToNumberOfTargetActivitiesField(formContext);
        }

        public static onChange_dpam_int_numberoftargetactivities(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-421
            Form.checkNumberOfTargetActivitiesValue(formContext);
        }

        //function to set the disabled property to False when the user has the "DPAM -Sales Manager" or "DPAM - Sales Person" security role
        static manageAccessToNumberOfTargetActivitiesField(formContext: Xrm.FormContext): void {
            let currentUserRoles: string[] = Xrm.Page.context.getUserRoles();
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
                    isSalesManager = false;
                    isSalesPerson = false;
                    for (let i = 0; i < result.entities.length; i++) {
                        let roleName: string = result.entities[i].name;

                        if (roleName == "DPAM - Sales Manager" || roleName == "System Administrator") isSalesManager = true;
                        if (roleName == "DPAM - Sales Person") isSalesPerson = true;    
                    }

                    if (isSalesPerson || isSalesManager) formContext.getControl<Xrm.Controls.NumberControl>("dpam_int_numberoftargetactivities").setDisabled(false);
                    if (isSalesPerson) {
                        numberOfTargetActivitiesValue = formContext.getAttribute("dpam_int_numberoftargetactivities").getValue();
                    }
                },
                function (error) {
                    console.log(error.message);
                });
        }

        //function to display a notification when the value of the "dpam_int_numberoftargetactivities" field is greater than the new value set by a user with the "DPAM - Sales Person" security role
        static checkNumberOfTargetActivitiesValue(formContext: Xrm.FormContext): void {
            if (!isSalesPerson || isSalesManager) return;

            formContext.getControl<Xrm.Controls.NumberControl>("dpam_int_numberoftargetactivities").clearNotification();

            let numberOfTargetActivitiesNewValue: number = formContext.getAttribute("dpam_int_numberoftargetactivities").getValue();

            if (numberOfTargetActivitiesNewValue < numberOfTargetActivitiesValue) {
                let message: string = `The value must be greater than ${numberOfTargetActivitiesValue.toString()}`;
                formContext.getControl<Xrm.Controls.NumberControl>("dpam_int_numberoftargetactivities").setNotification(message, "invalidTarget");
            }
        }
    }
}