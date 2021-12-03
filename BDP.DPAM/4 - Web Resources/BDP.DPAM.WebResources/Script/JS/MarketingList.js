/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
var BDP;
(function (BDP) {
    var DPAM;
    (function (DPAM) {
        var WR;
        (function (WR) {
            var MarketingList;
            (function (MarketingList) {
                //Variables used by isContactFrequencyCreatedOnEmpty function
                let isContactFrequencyCreatedOnEmpty;
                let previousSelectedItemId;
                class Ribbon {
                    //function to open the set contact frequencies page on the form
                    static openSetContactFrequenciesPageOnForm() {
                        let selectedId = Xrm.Page.data.entity.getId();
                        Ribbon.openSetContactFrequenciesPage(selectedId);
                    }
                    //function to open the set contact frequencies page
                    static openSetContactFrequenciesPage(selectedId) {
                        let pageInput = {
                            pageType: "custom",
                            name: "dpam_setcontactfrequenciespage_ba3d3",
                            entityName: "list",
                            recordId: selectedId
                        };
                        let navigationOptions = {
                            target: 2,
                            width: 550,
                            height: 460,
                            title: "Set Contact Frequencies"
                        };
                        Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                            .then(function success() {
                            Xrm.Page.data.refresh(true);
                        }, function error() {
                            console.log(error);
                        });
                    }
                    //function returns true if the user is a sales manager or a system administrator
                    static isSalesManagerOrAdmin() {
                        let userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
                        let isSalesManagerOrAdmin = false;
                        userRoles.forEach(function hasRoleName(item, index) {
                            if (item.name == "DPAM - Sales Manager" || item.name == "System Administrator")
                                isSalesManagerOrAdmin = true;
                        });
                        return isSalesManagerOrAdmin;
                    }
                    //function returns true if the contact frequency created on field is empty
                    static isContactFrequencyCreatedOnEmpty(selectedId, primaryControl) {
                        if (selectedId == null)
                            return;
                        if (previousSelectedItemId === selectedId)
                            return isContactFrequencyCreatedOnEmpty;
                        previousSelectedItemId = selectedId;
                        let recordId = selectedId.replace("{", "").replace("}", "");
                        Xrm.WebApi.retrieveRecord("list", recordId, "?$select=dpam_dt_contactfrequencycreatedon").then(function success(result) {
                            isContactFrequencyCreatedOnEmpty = result.dpam_dt_contactfrequencycreatedon == null;
                            primaryControl.refreshRibbon();
                        }, function (error) {
                            console.log(error.message);
                        });
                        return isContactFrequencyCreatedOnEmpty;
                    }
                }
                MarketingList.Ribbon = Ribbon;
            })(MarketingList = WR.MarketingList || (WR.MarketingList = {}));
        })(WR = DPAM.WR || (DPAM.WR = {}));
    })(DPAM = BDP.DPAM || (BDP.DPAM = {}));
})(BDP || (BDP = {}));
//# sourceMappingURL=MarketingList.js.map