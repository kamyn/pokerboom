﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazored.LocalStorage;
using PokerBoom.Shared.Models;
using System.Net.Http.Json;

namespace PokerBoom.Client.Pages
{
    public class TableListBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected HttpClient _httpClient { get; set; }
        protected int selectedRowNumber = -1;
        protected MudTable<PokerTable> mudTable { get; set; }
        protected string StackAmount { get; set; }
        protected PokerTable Table { get;set; }
        protected string searchTableString { get; set; }
        protected PokerTable selectedTable { get; set; }
        protected IEnumerable<PokerTable> Tables { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<GetTablesResult>("/api/table");
            if (result.Successful)
            {
                Tables = result.PokerTables;
            }
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
            await _localStorage.SetItemAsync("currentTable", selectedTable.Id);
            _navigationManager.NavigateTo("/game");
        }
    }
}
