﻿/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Contact {

    class _Static {
        readonly field = {
            contact: {
                mobilephone: "mobilephone",
                telephone1: "telephone1"
            }
        };

    }
    export let Static = new _Static();

    export class Form {
        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            this.resetPhoneNumber(executionContext, Static.field.contact.mobilephone);
            this.resetPhoneNumber(executionContext, Static.field.contact.telephone1);
        }

        static resetPhoneNumber(executionContext: Xrm.Events.EventContext, fieldName: string) {
            const formContext = executionContext.getFormContext();
            let phoneAttribute: Xrm.Page.Attribute = formContext.getAttribute(fieldName);

            if (phoneAttribute != null && phoneAttribute.getValue()) {

                let value: string = phoneAttribute.getValue();
                phoneAttribute.setValue(null);
                phoneAttribute.setValue(value);
            }       
        }
    }
}