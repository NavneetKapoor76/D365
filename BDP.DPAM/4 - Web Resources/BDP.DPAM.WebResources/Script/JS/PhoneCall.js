/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var PhoneCall;
            (function (PhoneCall) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-660
                        Form.setDueDateToTodayDate(formContext);
                        //SHER-660
                        Form.setDefaultDuration(formContext);
                    }
                    static quickCreateOnLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-660
                        Form.setDueDateToTodayDate(formContext);
                        //SHER-660
                        Form.setDefaultDuration(formContext);
                    }
                    //Set "scheduldend" (Due Date) field to today's date by default at creation.
                    static setDueDateToTodayDate(formContext) {
                        var currentDate = new Date();
                        formContext.getAttribute("scheduledend").setValue(currentDate);
                    }
                    //Set "actualdurationminutes" (Duration) field to 15 minutes by default at creation.
                    static setDefaultDuration(formContext) {
                        formContext.getAttribute("actualdurationminutes").setValue(15);
                    }
                }
                PhoneCall.Form = Form;
                class Ribbon {
                    /* SHER-990
                    * function to reopen the closed phonecall
                     */
                    static reOpenPhoneCall(formContext) {
                        let data = {
                            "statecode": 0 /* Active */,
                            "statuscode": 1 /* Open */
                        };
                        const confirmStrings = {
                            confirmButtonLabel: "Reopen", text: `Do you want to reopen this Phone Call ? You can't undo this action.`, title: "Confirm Reactivation"
                        };
                        const confirmOptions = { height: 200, width: 450 };
                        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(function (success) {
                            if (success.confirmed) {
                                Xrm.WebApi.updateRecord(formContext.data.entity.getEntityName(), formContext.data.entity.getId(), data).then(function success(result) {
                                    console.log("Activity updated");
                                    formContext.data.refresh(true);
                                    // perform operations on record update
                                }, function (error) {
                                    console.log(error.message);
                                    // handle error conditions
                                });
                            }
                        });
                    }
                }
                PhoneCall.Ribbon = Ribbon;
            })(PhoneCall = WR.PhoneCall || (WR.PhoneCall = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=PhoneCall.js.map