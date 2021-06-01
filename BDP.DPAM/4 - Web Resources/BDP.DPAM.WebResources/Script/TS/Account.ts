/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Account {

    export class Form {
        public static onLoad(executionContext: Xrm.Events.EventContext): void {

            const formContext: Xrm.FormContext = executionContext.getFormContext();

            const name = formContext.getAttribute("name").getValue();
            alert(`test ${name}`);
        }
    }
}
