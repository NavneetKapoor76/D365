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
                        if (formContext.ui.getFormType() == XrmEnum.FormType.Create
                            ||
                            formContext.ui.getFormType() == XrmEnum.FormType.QuickCreate) {
                            //SHER-660
                            Form.setDueDateToTodayDate(formContext);
                            //SHER-660
                            Form.setDefaultDuration(formContext);
                        }
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
            })(PhoneCall = WR.PhoneCall || (WR.PhoneCall = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=PhoneCall.js.map