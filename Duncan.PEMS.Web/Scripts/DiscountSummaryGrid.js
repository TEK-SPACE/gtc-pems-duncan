/*BEGIN RETRIEVE FILTER VALUES*/
var currCityId;
function LoadFilterDropdowns(currentCityId) {
    currCityId = currentCityId;
    LoadAccountStates();
}

//Asset Types
function LoadAccountStates() {
    //start with Alarm Status
    $.post('/Discounts/GetAccountStates', OnAccountStatesLoaded);
}
function OnAccountStatesLoaded(data) {
    viewModel.set("accountStates", data);
    LoadDiscountSchemes();
}

//Target Service
function LoadDiscountSchemes() {
    //start with Alarm Status
    $.post('/Discounts/GetDiscountSchemes?cityId=' + currCityId, OnDiscountSchemesLoaded);
}
function OnDiscountSchemesLoaded(data) {
    viewModel.set("discountSchemes", data);
    LoadDiscountStates();
}

//Asset State
function LoadDiscountStates() {
    //start with Alarm Status
    $.post('/Discounts/GetDiscountStates', OnDiscountStatesLoaded);

}
function OnDiscountStatesLoaded(data) {
    viewModel.set("discountStates", data);
    LoadSorts();
}

/*END FILTER VALUES*/
