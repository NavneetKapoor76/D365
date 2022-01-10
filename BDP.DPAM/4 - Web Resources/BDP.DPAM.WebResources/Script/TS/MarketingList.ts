/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.MarketingList {   

    //Variables used by isContactFrequencyCreatedOnEmpty function
    let isContactFrequencyCreatedOnEmpty: boolean;
    let previousSelectedItemId: string;
    //Variables used by manageNewMarketingListButtonVisibility function
    let isNewMarketingListButtonVisible: boolean = false;
    let isPromiseCompleted: boolean = false;

    export class Ribbon {
        //function to open the set contact frequencies page on the form
        public static openSetContactFrequenciesPageOnForm(): void {
            let selectedId: string = Xrm.Page.data.entity.getId();

            Ribbon.openSetContactFrequenciesPage(selectedId);
        }

        //function to open the set contact frequencies page
        public static openSetContactFrequenciesPage(selectedId: string): void {

            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_setcontactfrequenciespage_ba3d3",
                entityName: "list",
                recordId: selectedId
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 570,
                height: 450,
                title: "Set Contact Frequencies"
            };

            Xrm.Navigation.navigateTo(pageInput, navigationOptions)
                .then(
                    function success() {
                        Xrm.Page.data.refresh(true);
                    },
                    function error() {
                        console.log(error);
                    });
        }

        //function returns true if the user is a sales manager or a system administrator
        public static isSalesManagerOrAdmin(): boolean {
            let userRoles: Xrm.Collection.ItemCollection<Xrm.LookupValue> = Xrm.Utility.getGlobalContext().userSettings.roles;
            let isSalesManagerOrAdmin: boolean = false;

            userRoles.forEach(function hasRoleName(item: Xrm.LookupValue, index: number) {
                if (item.name == "DPAM - Sales Manager" || item.name == "System Administrator") isSalesManagerOrAdmin = true;
            });  

            return isSalesManagerOrAdmin;
        }
        
        //function returns true if the contact frequency created on field is empty
        public static isContactFrequencyCreatedOnEmpty(selectedId: string, primaryControl: Xrm.Controls.GridControl) {
            if (selectedId == null) return;

            if (previousSelectedItemId === selectedId) return isContactFrequencyCreatedOnEmpty;

            previousSelectedItemId = selectedId;

            let recordId: string = selectedId.replace("{", "").replace("}", "");

            Xrm.WebApi.retrieveRecord("list", recordId, "?$select=dpam_dt_contactfrequencycreatedon").then(
                function success(result) {
                    isContactFrequencyCreatedOnEmpty = result.dpam_dt_contactfrequencycreatedon == null;
                    primaryControl.refreshRibbon();
                },
                function (error) {
                    console.log(error.message);
                    });

            return isContactFrequencyCreatedOnEmpty;
        }

        //SHER-628
        //The "New Marketing List" button is hidden if the model-driven app is Marketing
        public static manageNewMarketingListButtonVisibility(primaryControl: Xrm.Controls.GridControl | Xrm.FormContext, fromGrid: boolean): boolean {
            
            if (isPromiseCompleted) return isNewMarketingListButtonVisible;

            Xrm.Utility.getGlobalContext().getCurrentAppProperties().then(function (appProperties: Xrm.AppProperties) {
                isPromiseCompleted = true;

                if (appProperties.uniqueName != "msdyncrm_MarketingSMBApp") {
                    isNewMarketingListButtonVisible = true;
                    if (fromGrid) {
                        (primaryControl as Xrm.Controls.GridControl).refreshRibbon();
                    }
                    else {
                        (primaryControl as Xrm.FormContext).ui.refreshRibbon();
                    }
                }

            }, function (error) {
                console.log(error.message);
                return false;
            });

            return isNewMarketingListButtonVisible;

        }

    }
}