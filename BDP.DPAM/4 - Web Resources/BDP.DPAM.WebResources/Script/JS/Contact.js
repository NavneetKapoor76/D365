/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var Contact;
            (function (Contact) {
                class _Static {
                    constructor() {
                        this.field = {
                            contact: {
                                mobilephone: "mobilephone",
                                telephone1: "telephone1"
                            }
                        };
                    }
                }
                Contact.Static = new _Static();
                class Form {
                    static QuickCreateonLoad(executionContext) {
                        this.resetPhoneNumber(executionContext, Contact.Static.field.contact.mobilephone);
                        this.resetPhoneNumber(executionContext, Contact.Static.field.contact.telephone1);
                    }
                    static resetPhoneNumber(executionContext, fieldName) {
                        const formContext = executionContext.getFormContext();
                        let phoneAttribute = formContext.getAttribute(fieldName);
                        if (phoneAttribute != null && phoneAttribute.getValue()) {
                            let value = phoneAttribute.getValue();
                            phoneAttribute.setValue(null);
                            phoneAttribute.setValue(value);
                        }
                    }
                }
                Contact.Form = Form;
            })(Contact = WR.Contact || (WR.Contact = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=Contact.js.map