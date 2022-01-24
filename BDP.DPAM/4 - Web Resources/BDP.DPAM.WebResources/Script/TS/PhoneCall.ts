/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.PhoneCall {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            if (formContext.ui.getFormType() == XrmEnum.FormType.Create
                ||
                formContext.ui.getFormType() == XrmEnum.FormType.QuickCreate
            ) {
                //SHER-660
                Form.setDueDateToTodayDate(formContext);
                //SHER-660
                Form.setDefaultDuration(formContext);
            }
        }

        public static quickCreateOnLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-660
            Form.setDueDateToTodayDate(formContext);
            //SHER-660
            Form.setDefaultDuration(formContext);
        }

        //Set "scheduldend" (Due Date) field to today's date by default at creation.
        static setDueDateToTodayDate(formContext: Xrm.FormContext) {
            var currentDate = new Date();
            formContext.getAttribute("scheduledend").setValue(currentDate);
        }

        //Set "actualdurationminutes" (Duration) field to 15 minutes by default at creation.
        static setDefaultDuration(formContext: Xrm.FormContext) {
            formContext.getAttribute("actualdurationminutes").setValue(15);
        }
    }
}