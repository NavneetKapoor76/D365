/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.OpportunityProduct {
    export class Subgrid {
        public static  hideAddNewProduct(opportunityId: number): any {
            if (opportunityId) {
            let fetchXml: string = `?fetchXml=<fetch top="1">
                                        <entity name="opportunityproduct" >
                                            <attribute name="opportunityid" />
                                        <filter>
                                            <condition attribute="opportunityid" operator="eq" value="${opportunityId}" />
                                        </filter></entity></fetch>`;

            return new Promise(function (resolve, reject) {
                Xrm.WebApi.retrieveMultipleRecords("opportunityproduct", fetchXml).then(
                    function success(result) {
                        var hasProduct = false;
                        if (result.entities.length == 0) hasProduct= true;

                        // return true or false
                        resolve(hasProduct);
                    },
                    function (error) {
                        reject(error.message);
                        console.log(error.message);
                    }
                );
                });
            }
            else {
                return false;
        }
    }
}