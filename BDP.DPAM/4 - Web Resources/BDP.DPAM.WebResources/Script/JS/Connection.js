/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Connection;
            (function (Connection) {
                //SHER-657 - Variable global to save the orginal list of EntityTypes of "record2id"
                let allConnectedToEntityTypes;
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-627
                        Form.setConnectedToAsThisRoleFilter(formContext);
                        //SHER-657
                        allConnectedToEntityTypes = formContext.getControl("record1id").getEntityTypes();
                        Form.filterConnectedToEntities(formContext);
                    }
                    //SHER-657 : function to keep only Counterparty, Contact and User in the "record2id" field if the field "record1id" is a Counterparty.
                    static onChange_record1id(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let connectedFromValue = formContext.getAttribute("record1id").getValue();
                        if (connectedFromValue == null || connectedFromValue[0].entityType != "account") {
                            formContext.getControl("record2id").setEntityTypes(allConnectedToEntityTypes);
                        }
                        else {
                            formContext.getControl("record2id").setEntityTypes(["account", "contact", "systemuser"]);
                        }
                    }
                    //function to set the filter on the record2roleid field
                    static setConnectedToAsThisRoleFilter(formContext) {
                        formContext.getControl("record2roleid").addPreSearch(Form.filterConnectedToAsThisRole);
                    }
                    //function to add a custom filter on the record2roleid field
                    static filterConnectedToAsThisRole(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let connectedFromValue = formContext.getAttribute("record1id").getValue();
                        let connectedToValue = formContext.getAttribute("record2id").getValue();
                        if (connectedFromValue == null || connectedFromValue[0].entityType != "opportunity" || connectedToValue == null)
                            return;
                        let category = "";
                        switch (connectedToValue[0].entityType) {
                            case "contact":
                            case "account":
                                category = "1000"; //Stakeholder
                                break;
                            case "systemuser":
                                category = "1001"; //Sales Team
                                break;
                        }
                        if (category == "")
                            return;
                        //statecode 0 = active
                        let filter = `<filter type="and">
                             <condition attribute="statecode" operator="eq" value="0" />
                             <condition attribute="category" operator="eq" value="${category}" />
                            </filter>`;
                        formContext.getControl("record2roleid").addCustomFilter(filter, "connectionrole");
                    }
                    //function to keep only Counterparty, Contact and User in the "record2id" field if the field "record1id" is a Counterparty.
                    static filterConnectedToEntities(formContext) {
                        let connectedFromValue = formContext.getAttribute("record1id").getValue();
                        if (connectedFromValue == null || connectedFromValue[0].entityType != "account")
                            return;
                        formContext.getControl("record2id").setEntityTypes(["account", "contact", "systemuser"]);
                    }
                }
                Connection.Form = Form;
            })(Connection = WR.Connection || (WR.Connection = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Connection.js.map