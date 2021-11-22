var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var ProductInterest;
            (function (ProductInterest) {
                class Form {
                    static onLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-503
                        Form.setInitialProductVisibility(executionContext);
                        //SHER-503
                        Form.setAssetClassFilter(formContext);
                    }
                    static QuickCreateonLoad(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //SHER-503
                        Form.setInitialProductVisibility(executionContext);
                        //SHER-503
                        Form.setAssetClassFilter(formContext);
                    }
                    static onChange_dpam_lk_product_category(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let productCategoryAttribute = formContext.getAttribute("dpam_lk_product_category");
                        if (productCategoryAttribute.getValue() != null) {
                            formContext.getControl("dpam_lk_product").setVisible(true);
                            Form.setFundNameFilter(formContext);
                        }
                        else {
                            formContext.getControl("dpam_lk_product").setVisible(false);
                        }
                        formContext.getAttribute("dpam_lk_product").setValue(null);
                        formContext.getAttribute("dpam_lk_shareclass").setValue(null);
                    }
                    static onChange_dpam_lk_product_assetclass(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let productAssetClassAttribute = formContext.getAttribute("dpam_lk_product_assetclass");
                        if (productAssetClassAttribute.getValue() != null) {
                            formContext.getControl("dpam_lk_product_category").setVisible(true);
                            formContext.getControl("dpam_lk_product").setVisible(false);
                            Form.setCategoryFilter(formContext);
                        }
                        else {
                            formContext.getControl("dpam_lk_product_category").setVisible(false);
                            formContext.getControl("dpam_lk_product").setVisible(false);
                        }
                        formContext.getAttribute("dpam_lk_product_category").setValue(null);
                        formContext.getAttribute("dpam_lk_product").setValue(null);
                        formContext.getAttribute("dpam_lk_shareclass").setValue(null);
                    }
                    static onChange_dpam_lk_product(executionContext) {
                        const formContext = executionContext.getFormContext();
                        formContext.getAttribute("dpam_lk_shareclass").setValue(null);
                        let productAttribute = formContext.getAttribute("dpam_lk_product");
                        if (productAttribute.getValue() != null && productAttribute.getValue()[0] && productAttribute.getValue()[0].id) {
                            let fetchXml = `?fetchXml=<fetch top="1"><entity name="dpam_shareclass"><attribute name="dpam_shareclassid"/><attribute name="dpam_s_shareclass"/><filter><condition attribute="dpam_s_shareclass" operator="eq" value="F" /></filter><link-entity name="dpam_shareclass_product" from="dpam_shareclassid" to="dpam_shareclassid" intersect="true"><filter><condition attribute="productid" operator="eq" value= "${productAttribute.getValue()[0].id}" /></filter></link-entity></entity></fetch>`;
                            Xrm.WebApi.retrieveMultipleRecords("dpam_shareclass", fetchXml).then(function success(result) {
                                if (result.entities.length > 0) {
                                    let lookupValues = new Array();
                                    lookupValues[0] = new LookupValue();
                                    lookupValues[0].id = result.entities[0].dpam_shareclassid;
                                    lookupValues[0].name = result.entities[0].dpam_s_shareclass;
                                    lookupValues[0].entityType = "dpam_shareclass";
                                    console.log(lookupValues[0]);
                                    let shareclassAttribute = formContext.getAttribute("dpam_lk_shareclass");
                                    shareclassAttribute.setValue(lookupValues);
                                }
                            }, function (error) {
                                console.log(error.message);
                                // handle error conditions
                            });
                        }
                    }
                    static setAssetClassFilter(formContext) {
                        formContext.getControl("dpam_lk_product_assetclass").addPreSearch(Form.filterAssetClassFilter);
                    }
                    static setCategoryFilter(formContext) {
                        formContext.getControl("dpam_lk_product_category").addPreSearch(Form.filterCategoryFilter);
                    }
                    static setFundNameFilter(formContext) {
                        formContext.getControl("dpam_lk_product").addPreSearch(Form.filterFundNameFilter);
                    }
                    static filterAssetClassFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        //Filter lookup on product with no parent -> parentproductid is empty
                        let filter = `<filter type="and">
                              <condition attribute="parentproductid" operator="null" >     
                              </condition>
                            </filter>`;
                        formContext.getControl("dpam_lk_product_assetclass").addCustomFilter(filter, "product");
                    }
                    static filterCategoryFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let assetClassAttribute = formContext.getAttribute("dpam_lk_product_assetclass");
                        if (assetClassAttribute.getValue() != null && assetClassAttribute.getValue()[0] && assetClassAttribute.getValue()[0].id) {
                            let productid = assetClassAttribute.getValue()[0].id;
                            let filter = `<filter type="and">
                              <condition attribute="parentproductid" operator="eq" value=" ${productid}" >     
                              </condition>
                            </filter>`;
                            formContext.getControl("dpam_lk_product_category").addCustomFilter(filter, "product");
                        }
                    }
                    static filterFundNameFilter(executionContext) {
                        const formContext = executionContext.getFormContext();
                        let categoryAttribute = formContext.getAttribute("dpam_lk_product_category");
                        if (categoryAttribute.getValue() != null && categoryAttribute.getValue()[0] && categoryAttribute.getValue()[0].id) {
                            let productid = categoryAttribute.getValue()[0].id;
                            let filter = `<filter type="and">
                              <condition attribute="parentproductid" operator="eq" value=" ${productid}" >     
                              </condition>
                            </filter>`;
                            formContext.getControl("dpam_lk_product").addCustomFilter(filter, "product");
                        }
                    }
                    static setInitialProductVisibility(executionContext) {
                        return __awaiter(this, void 0, void 0, function* () {
                            const formContext = executionContext.getFormContext();
                            //Check if it is create mode
                            if (formContext.ui.getFormType() == 1) {
                                formContext.getControl("dpam_lk_product_assetclass").setVisible(false);
                                formContext.getControl("dpam_lk_product_category").setVisible(false);
                                formContext.getControl("dpam_lk_product").setVisible(false);
                                let productAssetClassAttribute = formContext.getAttribute("dpam_lk_product_assetclass");
                                if (productAssetClassAttribute.getValue() != null && productAssetClassAttribute.getValue()[0] && productAssetClassAttribute.getValue()[0].id) {
                                    let endLoop = false;
                                    let currentProductid = productAssetClassAttribute.getValue()[0].id;
                                    let lookupValues1;
                                    let tempLookupValues;
                                    let lookupValues2;
                                    let lookupValues3;
                                    let cpt = 1;
                                    while (!endLoop) {
                                        yield Xrm.WebApi.retrieveRecord("product", currentProductid).then(function success(result) {
                                            if (!result._parentproductid_value || result._parentproductid_value == null) {
                                                lookupValues1 = Form.fillLookup(result);
                                                endLoop = true;
                                            }
                                            else {
                                                if (tempLookupValues != null) {
                                                    lookupValues3 = tempLookupValues;
                                                    lookupValues2 = Form.fillLookup(result);
                                                    currentProductid = result._parentproductid_value;
                                                }
                                                else {
                                                    tempLookupValues = Form.fillLookup(result);
                                                    currentProductid = result._parentproductid_value;
                                                }
                                            }
                                        }, function (error) {
                                            console.log(error.message);
                                            endLoop = true;
                                            // handle error conditions
                                        });
                                        // security guard in case of no end. max level = 3
                                        cpt = cpt + 1;
                                        if (cpt > 3) {
                                            endLoop = true;
                                        }
                                    }
                                    if (lookupValues3 == null) {
                                        if (tempLookupValues != null) {
                                            lookupValues2 = tempLookupValues;
                                        }
                                    }
                                    formContext.getAttribute("dpam_lk_product_assetclass").setValue(null);
                                    formContext.getAttribute("dpam_lk_product_category").setValue(null);
                                    formContext.getAttribute("dpam_lk_product").setValue(null);
                                    formContext.getControl("dpam_lk_product_assetclass").setVisible(true);
                                    if (lookupValues1 != null) {
                                        formContext.getAttribute("dpam_lk_product_assetclass").setValue(lookupValues1);
                                        formContext.getControl("dpam_lk_product_category").setVisible(true);
                                        Form.setCategoryFilter(formContext);
                                    }
                                    if (lookupValues2 != null) {
                                        formContext.getAttribute("dpam_lk_product_category").setValue(lookupValues2);
                                        formContext.getControl("dpam_lk_product").setVisible(true);
                                        Form.setFundNameFilter(formContext);
                                    }
                                    if (lookupValues3 != null) {
                                        formContext.getAttribute("dpam_lk_product").setValue(lookupValues3);
                                        Form.onChange_dpam_lk_product(executionContext);
                                    }
                                }
                                else {
                                    // case with all product field empty.
                                    formContext.getControl("dpam_lk_product_assetclass").setVisible(true);
                                    formContext.getControl("dpam_lk_product_category").setVisible(false);
                                    formContext.getControl("dpam_lk_product").setVisible(false);
                                }
                            }
                            else {
                                // case updatemanage visibility and filter
                                let productAssetClassAttribute = formContext.getAttribute("dpam_lk_product_assetclass");
                                if (productAssetClassAttribute.getValue() != null) {
                                    formContext.getControl("dpam_lk_product_category").setVisible(true);
                                    Form.setCategoryFilter(formContext);
                                    let productCategoryAttribute = formContext.getAttribute("dpam_lk_product_category");
                                    if (productCategoryAttribute.getValue() != null) {
                                        formContext.getControl("dpam_lk_product").setVisible(true);
                                        Form.setFundNameFilter(formContext);
                                    }
                                    else {
                                        formContext.getControl("dpam_lk_product").setVisible(false);
                                    }
                                }
                                else {
                                    formContext.getControl("dpam_lk_product_category").setVisible(false);
                                    formContext.getControl("dpam_lk_product").setVisible(false);
                                }
                            }
                        });
                    }
                    static fillLookup(result) {
                        let lookupValues = new Array();
                        lookupValues[0] = new LookupValue();
                        lookupValues[0].id = result.productid;
                        lookupValues[0].name = result.name;
                        lookupValues[0].entityType = "product";
                        return lookupValues;
                    }
                }
                ProductInterest.Form = Form;
                class LookupValue {
                }
            })(ProductInterest = WR.ProductInterest || (WR.ProductInterest = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=ProductInterest.js.map