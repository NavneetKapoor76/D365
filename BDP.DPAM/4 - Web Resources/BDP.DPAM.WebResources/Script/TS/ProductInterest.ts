/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.ProductInterest {

    export class Form {


        public static onLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-503
            Form.setInitialProductVisibility(executionContext);
            //SHER-503
            Form.setAssetClassFilter(formContext);

        }


        public static QuickCreateonLoad(executionContext: Xrm.Events.EventContext): void {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //SHER-503
            Form.setInitialProductVisibility(executionContext);
            //SHER-503
            Form.setAssetClassFilter(formContext);


        }

        public static onChange_dpam_lk_product_category(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            let productCategoryAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_product_category");
            if (productCategoryAttribute.getValue() != null) {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(true);
                Form.setFundNameFilter(formContext)
            } else {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
            }
            formContext.getAttribute("dpam_lk_product").setValue(null);
            formContext.getAttribute("dpam_lk_shareclass").setValue(null);
        }

        public static onChange_dpam_lk_product_assetclass(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            let productAssetClassAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_product_assetclass");
            if (productAssetClassAttribute.getValue() != null) {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(true);
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
                Form.setCategoryFilter(formContext);
            } else {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(false);
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
            }

            formContext.getAttribute("dpam_lk_product_category").setValue(null);
            formContext.getAttribute("dpam_lk_product").setValue(null);
            formContext.getAttribute("dpam_lk_shareclass").setValue(null);
        }

        public static onChange_dpam_lk_product(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            formContext.getAttribute("dpam_lk_shareclass").setValue(null);
            let productAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute("dpam_lk_product");
            if (productAttribute.getValue() != null && productAttribute.getValue()[0] && productAttribute.getValue()[0].id) {
                let fetchXml: string = `?fetchXml=<fetch top="1"><entity name="dpam_shareclass"><attribute name="dpam_shareclassid"/><attribute name="dpam_s_shareclass"/><filter><condition attribute="dpam_s_shareclass" operator="eq" value="F" /></filter><link-entity name="dpam_shareclass_product" from="dpam_shareclassid" to="dpam_shareclassid" intersect="true"><filter><condition attribute="productid" operator="eq" value= "${productAttribute.getValue()[0].id}" /></filter></link-entity></entity></fetch>`;
                Xrm.WebApi.retrieveMultipleRecords("dpam_shareclass", fetchXml).then(
                    function success(result) {
                        if (result.entities.length > 0) {
                            let lookupValues: Array<LookupValue> = new Array();
                            lookupValues[0] = new LookupValue();
                            lookupValues[0].id = result.entities[0].dpam_shareclassid;
                            lookupValues[0].name = result.entities[0].dpam_s_shareclass;
                            lookupValues[0].entityType = "dpam_shareclass";
                            console.log(lookupValues[0]);
                            let shareclassAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_shareclass");
                            shareclassAttribute.setValue(lookupValues);
                        }
                    },
                    function (error) {
                        console.log(error.message);
                        // handle error conditions
                    }
                );
            }
        }

        static setAssetClassFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_product_assetclass").addPreSearch(Form.filterAssetClassFilter);
        }
        static setCategoryFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_product_category").addPreSearch(Form.filterCategoryFilter);
        }
        static setFundNameFilter(formContext: Xrm.FormContext) {
            formContext.getControl<Xrm.Controls.LookupControl>("dpam_lk_product").addPreSearch(Form.filterFundNameFilter);
        }

        static filterAssetClassFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //Filter lookup on product with no parent -> parentproductid is empty
            let filter: string = `<filter type="and">
                              <condition attribute="parentproductid" operator="null" >     
                              </condition>
                            </filter>`;

            formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_assetclass").addCustomFilter(filter, "product");
        }


        static filterCategoryFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            let assetClassAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_product_assetclass");
            if (assetClassAttribute.getValue() != null && assetClassAttribute.getValue()[0] && assetClassAttribute.getValue()[0].id) {
                let productid: string = assetClassAttribute.getValue()[0].id;
                let filter: string = `<filter type="and">
                              <condition attribute="parentproductid" operator="eq" value=" ${productid}" >     
                              </condition>
                            </filter>`;

                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").addCustomFilter(filter, "product");
            }
        }

        static filterFundNameFilter(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            let categoryAttribute: Xrm.Page.LookupAttribute = formContext.getAttribute<Xrm.Page.LookupAttribute>("dpam_lk_product_category");
            if (categoryAttribute.getValue() != null && categoryAttribute.getValue()[0] && categoryAttribute.getValue()[0].id) {
                let productid: string = categoryAttribute.getValue()[0].id;
                let filter: string = `<filter type="and">
                              <condition attribute="parentproductid" operator="eq" value=" ${productid}" >     
                              </condition>
                            </filter>`;

                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").addCustomFilter(filter, "product");
            }
        }


        static async setInitialProductVisibility(executionContext: Xrm.Events.EventContext) {
            const formContext: Xrm.FormContext = executionContext.getFormContext();
            //Check if it is create mode
            if (formContext.ui.getFormType() == 1) {
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_assetclass").setVisible(false);
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(false);
                formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
                let productAssetClassAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_product_assetclass");
                if (productAssetClassAttribute.getValue() != null && productAssetClassAttribute.getValue()[0] && productAssetClassAttribute.getValue()[0].id) {

                    let endLoop: boolean = false;
                    let currentProductid: string = productAssetClassAttribute.getValue()[0].id;
                    let lookupValues1: Array<LookupValue>;
                    let tempLookupValues: Array<LookupValue>;
                    let lookupValues2: Array<LookupValue>;
                    let lookupValues3: Array<LookupValue>;
                    let cpt: number = 1
                    while (!endLoop) {

                        await Xrm.WebApi.retrieveRecord("product", currentProductid).then(
                            function success(result) {
                                if (!result._parentproductid_value || result._parentproductid_value == null) {
                                    lookupValues1 = Form.fillLookup(result);
                                    endLoop = true;

                                } else {
                                    if (tempLookupValues != null) {
                                        lookupValues3 = tempLookupValues;
                                        lookupValues2 = Form.fillLookup(result);
                                        currentProductid = result._parentproductid_value;
                                    } else {
                                        tempLookupValues = Form.fillLookup(result);
                                        currentProductid = result._parentproductid_value;
                                    }
                                }
                            },
                            function (error) {
                                console.log(error.message);
                                endLoop = true;
                                // handle error conditions
                            }
                        );

                        // security guard in case of no end. max level = 3
                        cpt = cpt + 1
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
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_assetclass").setVisible(true);

                    if (lookupValues1 != null) {
                        formContext.getAttribute("dpam_lk_product_assetclass").setValue(lookupValues1);
                        formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(true);
                        Form.setCategoryFilter(formContext);
                    }
                    if (lookupValues2 != null) {
                        formContext.getAttribute("dpam_lk_product_category").setValue(lookupValues2);
                        formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(true);
                        Form.setFundNameFilter(formContext);
                    }
                    if (lookupValues3 != null) {
                        formContext.getAttribute("dpam_lk_product").setValue(lookupValues3);
                        Form.onChange_dpam_lk_product(executionContext);
                    }

                } else {
                    // case with all product field empty.
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_assetclass").setVisible(true);
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(false);
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
                }
            } else {
                // case updatemanage visibility and filter
                let productAssetClassAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_product_assetclass");
                if (productAssetClassAttribute.getValue() != null) {
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(true);
                    Form.setCategoryFilter(formContext);
                    let productCategoryAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("dpam_lk_product_category");
                    if (productCategoryAttribute.getValue() != null) {
                        formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(true);
                        Form.setFundNameFilter(formContext)
                    } else {
                        formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
                    }
                } else {
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product_category").setVisible(false);
                    formContext.getControl<Xrm.Page.LookupControl>("dpam_lk_product").setVisible(false);
                }
            }
        }

        static fillLookup(result: Xrm.Entity): Array<LookupValue> { 
            let lookupValues: Array<LookupValue> = new Array();
            lookupValues[0] = new LookupValue();
            lookupValues[0].id = result.productid;
            lookupValues[0].name = result.name;
            lookupValues[0].entityType = "product";
            return lookupValues;        
        }
        

       
    }
    class LookupValue {
        id: string;
        name: string;
        entityType: string;
    }

}