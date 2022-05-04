/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.PhoneCall {
    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-660 + SHER-1073
            Form.setDueDateToTodayDate(formContext);
            //SHER-660
            Form.setDefaultDuration(formContext);
        }

        public static quickCreateOnLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-660 + SHER-1073
            Form.setDueDateToTodayDate(formContext);
            //SHER-660
            Form.setDefaultDuration(formContext);
        }

        //Set "scheduldend" (Due Date) field to today's date by default at creation.
        static setDueDateToTodayDate(formContext: Xrm.FormContext) {
            let currentDate: Date = new Date();
            let dueDateAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("scheduledend");

            if (dueDateAttribute.getValue() != null) return;

            dueDateAttribute.setValue(currentDate);
        }

        //Set "actualdurationminutes" (Duration) field to 15 minutes by default at creation.
        static setDefaultDuration(formContext: Xrm.FormContext) {
            formContext.getAttribute("actualdurationminutes").setValue(15);
        }
    }
    export class Ribbon {
        /* SHER-990
        * function to reopen the closed phonecall
         */
        public static reOpenPhoneCall(formContext: Xrm.FormContext) {

            let data = {
                "statecode": Enum.Phonecall_StateCode.Active,
                "statuscode": Enum.Phonecall_StatusCode.Open
            };


            const confirmStrings: Xrm.Navigation.ConfirmStrings = {
                confirmButtonLabel: "Reopen", text: `Do you want to reopen this Phone Call ? You can't undo this action.`, title: "Confirm Reactivation"
            };
            const confirmOptions: Xrm.Navigation.DialogSizeOptions = { height: 200, width: 450 };

            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                function (success) {
                    if (success.confirmed) {


                        Xrm.WebApi.updateRecord(
                            formContext.data.entity.getEntityName(),
                            formContext.data.entity.getId(),
                            data

                        ).then(
                            function success(result) {
                                console.log("Activity updated");
                                formContext.data.refresh(true);
                                // perform operations on record update
                            },
                            function (error) {
                                console.log(error.message);
                                // handle error conditions
                            }
                        );
                    }
                });



        }
    }
}