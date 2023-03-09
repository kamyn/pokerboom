using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazored.LocalStorage;
using PokerBoom.Shared.Models;
using System.Net.Http.Json;
using PokerBoom.Client.States;
using Microsoft.AspNetCore.Components.Authorization;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;

namespace PokerBoom.Client.Pages
{
    public class TableListBase : ComponentBase
    {
        [Inject] protected ILocalStorageService? _localStorage { get; set; }
        [Inject] protected NavigationManager? _navigationManager { get; set; }
        [Inject] protected HttpClient? _httpClient { get; set; }
        [Inject] protected BalanceState _balanceState { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public AuthenticationState AuthState { get; set; }
        protected int selectedRowNumber = -1;
        protected MudTable<PokerTable>? mudTable { get; set; }
        protected int? StackAmount { get; set; }
        protected PokerTable? Table { get;set; }
        protected string? searchTableString { get; set; }
        protected PokerTable? selectedTable { get; set; }
        protected IEnumerable<PokerTable>? Tables { get; set; }
        protected string? ChangeBalanceUsername { get; set; }
        protected int? ChangeBalanceValue { get; set; }
        protected string? CreateNewTableName { get; set; }
        protected int? CreateNewTableSmallBlind { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var result = await _httpClient.GetFromJsonAsync<GetTablesResult>("/api/table");
            if (result.Successful)
            {
                Tables = result.PokerTables;
            }

            var response = await _httpClient.GetAsync($"/api/balance?username={AuthState.User.Identity.Name}");
            int balance = (await response.Content.ReadFromJsonAsync<GetBalanceViewModel>()).Balance;

            _balanceState.OnBalanceChanged.Invoke(balance);

            StateHasChanged();
        }

        protected string SelectedRowClassFunc(PokerTable element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        protected bool FilterFunc1(PokerTable element) => FilterFunc(element, searchTableString);

        protected bool FilterFunc(PokerTable element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        protected async Task ConnectToTable()
        {
            if (StackAmount != null && StackAmount > 0)
            {
                await _localStorage.SetItemAsync("currentTable", selectedTable.Id);
                await _localStorage.SetItemAsync("stackAmount", StackAmount);
                _navigationManager.NavigateTo("/game");
            }
        }

        protected async Task ChangeBalance()
        {
            if (_httpClient != null && ChangeBalanceUsername != null && ChangeBalanceValue != null)
            {
                var result = await _httpClient.PostAsJsonAsync("/api/changebalance", new ChangeBalanceViewModel
                {
                    UserName = ChangeBalanceUsername,
                    BalanceValue = (int)ChangeBalanceValue
                });
            }
        }

        protected async Task CreateNewTable()
        {
            if (_httpClient != null && CreateNewTableName != null && CreateNewTableSmallBlind != null)
            {
                var result = await _httpClient.PostAsJsonAsync("/api/createnewtable", new CreateNewTableViewModel
                {
                    Name = CreateNewTableName,
                    SmallBlind = (int)CreateNewTableSmallBlind
                });
                await Task.Delay(2000);
                var resultTables = await _httpClient.GetFromJsonAsync<GetTablesResult>("/api/table");
                if (resultTables != null && resultTables.Successful)
                {
                    Tables = resultTables.PokerTables;
                }
            }
        }

        protected async Task RemoveTable()
        {
            if (_httpClient != null && selectedTable != null)
            {
                var result = await _httpClient.PostAsJsonAsync("/api/removetable", new RemoveTableViewModel
                {
                    TableId = selectedTable.Id
                });
                await Task.Delay(2000);
                var resultTables = await _httpClient.GetFromJsonAsync<GetTablesResult>("/api/table");
                if (resultTables != null && resultTables.Successful)
                {
                    Tables = resultTables.PokerTables;
                }
            }
        }
    }
}
