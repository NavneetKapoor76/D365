/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var ContactFrequency;
            (function (ContactFrequency) {
                //Variable global to know if the user has the "DPAM - Sales Person" security role
                let isSalesPerson;
                //Variable global to know if the user has the "DPAM - Sales Manager" security role
                let isSalesManager;
                //Variable global to know the value of "dpam_int_numberoftargetactivities" field
                let numberOfTargetActivitiesValue;
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-421
                        Form.manageAccessToNumberOfTargetActivitiesField(formContext);
                    }
                    static onChange_dpam_int_numberoftargetactivities(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-421
                        Form.checkNumberOfTargetActivitiesValue(formContext);
                    }
                    //function to set the disabled property to False when the user has the "DPAM -Sales Manager" or "DPAM - Sales Person" security role
                    static manageAccessToNumberOfTargetActivitiesField(formContext) {
                        let currentUserRoles = Xrm.Page.context.getUserRoles();
                        let roleCondition = "";
                        currentUserRoles.forEach(function (role) {
                            roleCondition += `<condition attribute="roleid" operator="eq" value="{${role}}" />`;
                        });
                        let fetchXml = `?fetchXml=<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
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
                        Xrm.WebApi.retrieveMultipleRecords("role", fetchXml).then(function success(result) {
                            isSalesManager = false;
                            isSalesPerson = false;
                            for (let i = 0; i < result.entities.length; i++) {
                                let roleName = result.entities[i].name;
                                if (roleName == "DPAM - Sales Manager" || roleName == "System Administrator")
                                    isSalesManager = true;
                                if (roleName == "DPAM - Sales Person")
                                    isSalesPerson = true;
                            }
                            if (isSalesPerson || isSalesManager)
                                formContext.getControl("dpam_int_numberoftargetactivities").setDisabled(false);
                            if (isSalesPerson) {
                                numberOfTargetActivitiesValue = formContext.getAttribute("dpam_int_numberoftargetactivities").getValue();
                            }
                        }, function (error) {
                            console.log(error.message);
                        });
                    }
                    //function to display a notification when the value of the "dpam_int_numberoftargetactivities" field is greater than the new value set by a user with the "DPAM - Sales Person" security role
                    static checkNumberOfTargetActivitiesValue(formContext) {
                        if (!isSalesPerson || isSalesManager)
                            return;
                        formContext.getControl("dpam_int_numberoftargetactivities").clearNotification();
                        let numberOfTargetActivitiesNewValue = formContext.getAttribute("dpam_int_numberoftargetactivities").getValue();
                        if (numberOfTargetActivitiesNewValue < numberOfTargetActivitiesValue) {
                            let message = `The value must be greater than ${numberOfTargetActivitiesValue.toString()}`;
                            formContext.getControl("dpam_int_numberoftargetactivities").setNotification(message, "invalidTarget");
                        }
                    }
                }
                ContactFrequency.Form = Form;
            })(ContactFrequency = WR.ContactFrequency || (WR.ContactFrequency = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=ContactFrequency.js.map