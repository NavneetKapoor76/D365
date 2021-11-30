/// <reference path="../../node_modules/@types/xrm/index.d.ts" />
namespace BDP.DPAM.WR.Segment {   

    //Variables used by isContactFrequencyCreatedOnEmpty function
    let isContactFrequencyCreatedOnEmpty: boolean;
    let previousSelectedItemId: string;

    export class Ribbon {
        //function to open the set contact frequencies page on the form
        public static openSetContactFrequenciesPageOnForm(): void {
            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_setcontactfrequenciespage_ba3d3",
                entityName: "msdyncrm_segment",
                recordId: Xrm.Page.data.entity.getId()
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 550,
                height: 460,
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

        //function to open the set contact frequencies page
        public static openSetContactFrequenciesPage(selectedId: string): void {

            let pageInput: Xrm.Navigation.CustomPage = {
                pageType: "custom",
                name: "dpam_setcontactfrequenciespage_ba3d3",
                entityName: "msdyncrm_segment",
                recordId: selectedId
            };

            let navigationOptions: Xrm.Navigation.NavigationOptions = {
                target: 2,
                width: 550,
                height: 460,
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

            Xrm.WebApi.retrieveRecord("msdyncrm_segment", recordId, "?$select=dpam_dt_contactfrequencycreatedon").then(
                function success(result) {
                    isContactFrequencyCreatedOnEmpty = result.dpam_dt_contactfrequencycreatedon == null;
                    primaryControl.refreshRibbon();
                },
                function (error) {
                    console.log(error.message);
                    });

            return isContactFrequencyCreatedOnEmpty;
        }

    }
}