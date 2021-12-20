/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Connection {

    //SHER-657 - Variable global to save the orginal list of EntityTypes of "record2id"
    let allConnectedToEntityTypes: string[];

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            //SHER-627
            Form.setConnectedToAsThisRoleFilter(formContext);
            //SHER-657
            allConnectedToEntityTypes = formContext.getControl<Xrm.Controls.LookupControl>("record1id").getEntityTypes();
            Form.filterConnectedToEntities(formContext);
        }

        //SHER-657 : function to keep only Counterparty, Contact and User in the "record2id" field if the field "record1id" is a Counterparty.
        public static onChange_record1id(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            let connectedFromValue: Xrm.Page.LookupValue = formContext.getAttribute("record1id").getValue();

            if (connectedFromValue == null || connectedFromValue[0].entityType != "account") {
                formContext.getControl<Xrm.Controls.LookupControl>("record2id").setEntityTypes(allConnectedToEntityTypes);
            } else {
                formContext.getControl<Xrm.Controls.LookupControl>("record2id").setEntityTypes(["account", "contact", "systemuser"]);
            }
        }

        //function to set the filter on the record2roleid field
        static setConnectedToAsThisRoleFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("record2roleid").addPreSearch(Form.filterConnectedToAsThisRole);
        }

        //function to add a custom filter on the record2roleid field
        static filterConnectedToAsThisRole(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();

            let connectedFromValue: Xrm.Page.LookupValue = formContext.getAttribute("record1id").getValue();
            let connectedToValue: Xrm.Page.LookupValue = formContext.getAttribute("record2id").getValue();

            if (connectedFromValue == null || connectedFromValue[0].entityType != "opportunity" || connectedToValue == null ) return;

            let category: string = "";
            switch (connectedToValue[0].entityType) {
                case "contact":
                case "account":
                    category = "1000"; //Stakeholder
                    break;
                case "systemuser":
                    category = "1001"; //Sales Team
                    break;
            }

            if (category == "") return;

            //statecode 0 = active
            let filter: string = `<filter type="and">
                             <condition attribute="statecode" operator="eq" value="0" />
                             <condition attribute="category" operator="eq" value="${category}" />
                            </filter>`;

            formContext.getControl<Xrm.Controls.LookupControl>("record2roleid").addCustomFilter(filter, "connectionrole");
        }

        //function to keep only Counterparty, Contact and User in the "record2id" field if the field "record1id" is a Counterparty.
        static filterConnectedToEntities(formContext: Xrm.FormContext) {
            let connectedFromValue: Xrm.Page.LookupValue = formContext.getAttribute("record1id").getValue();

            if (connectedFromValue == null || connectedFromValue[0].entityType != "account") return;

            formContext.getControl<Xrm.Controls.LookupControl>("record2id").setEntityTypes(["account","contact","systemuser"]);
        }
    }
}