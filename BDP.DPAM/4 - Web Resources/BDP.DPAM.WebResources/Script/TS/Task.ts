/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Task {
    export class Ribbon {
        /* SHER-990
         * function to reopen the closed task
         */
        public static reOpenTask(formContext: Xrm.FormContext) {

            let data = {
                "statecode": Enum.Task_StateCode.Active,
                "statuscode": Enum.Task_StatusCode.NotStarted
            };

            const confirmStrings: Xrm.Navigation.ConfirmStrings = { confirmButtonLabel: "Reopen",    text: `Do you want to reopen this Task ? You can't undo this action.`, title: "Confirm Reactivation" };
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