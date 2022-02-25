/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Task;
            (function (Task) {
                class Ribbon {
                    /* SHER-990
                     * function to reopen the closed task
                     */
                    static reOpenTask(formContext) {
                        let data = {
                            "statecode": 0 /* Active */,
                            "statuscode": 2 /* NotStarted */
                        };
                        const confirmStrings = { confirmButtonLabel: "Reopen", text: `Do you want to reopen this Task ? You can't undo this action.`, title: "Confirm Reactivation" };
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
                Task.Ribbon = Ribbon;
            })(Task = WR.Task || (WR.Task = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Task.js.map