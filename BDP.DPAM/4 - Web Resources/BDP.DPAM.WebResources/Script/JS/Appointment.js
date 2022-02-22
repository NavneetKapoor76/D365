/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Appointment;
            (function (Appointment) {
                class Ribbon {
                    /* SHER-990
                     * function to reopen the closed appointment
                     */
                    static reOpenAppointment(formContext) {
                        let data = {
                            "statecode": 3 /* Scheduled */,
                            "statuscode": 5 /* Busy */
                        };
                        const confirmStrings = { confirmButtonLabel: "Reopen", text: `Do you want to reopen this Appointment ? You can't undo this action.`, title: "Confirm Reactivation" };
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
                Appointment.Ribbon = Ribbon;
            })(Appointment = WR.Appointment || (WR.Appointment = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Appointment.js.map