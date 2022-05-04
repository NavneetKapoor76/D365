/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var OpportunityProduct;
            (function (OpportunityProduct) {
                class Subgrid {
                    static hideAddNewProduct(opportunityId) {
                        if (opportunityId) {
                            let fetchXml = `?fetchXml=<fetch top="1">
                                        <entity name="opportunityproduct" >
                                            <attribute name="opportunityid" />
                                        <filter>
                                            <condition attribute="opportunityid" operator="eq" value="${opportunityId}" />
                                        </filter></entity></fetch>`;
                            return new Promise(function (resolve, reject) {
                                Xrm.WebApi.retrieveMultipleRecords("opportunityproduct", fetchXml).then(function success(result) {
                                    var hasProduct = false;
                                    if (result.entities.length == 0)
                                        hasProduct = true;
                                    // return true or false
                                    resolve(hasProduct);
                                }, function (error) {
                                    reject(error.message);
                                    console.log(error.message);
                                });
                            });
                        }
                        else {
                            return false;
                        }
                    }
                }
                OpportunityProduct.Subgrid = Subgrid;
            })(OpportunityProduct = WR.OpportunityProduct || (WR.OpportunityProduct = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=OpportunityProduct.js.map