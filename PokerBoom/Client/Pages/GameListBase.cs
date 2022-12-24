using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PokerBoom.Shared.Models;
using System.Net.Http.Json;

namespace PokerBoom.Client.Pages
{
    public class GameListBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected HttpClient _httpClient { get; set; }
        protected int selectedRowNumber = -1;
        protected MudTable<GameReview> mudTable { get; set; }
        protected GameReview Table { get; set; }
        protected string searchGameString { get; set; }
        protected GameReview selectedGame { get; set; }
        protected IEnumerable<GameReview> Games { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<GetGamesResultViewModel>("/api/games");
            if (result.Successful)
            {
                Games = result.Games;
            }
            StateHasChanged();
        }

        protected string SelectedRowClassFunc(GameReview element, int rowNumber)
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

        protected bool FilterFunc1(GameReview element) => FilterFunc(element, searchGameString);

        protected bool FilterFunc(GameReview element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.TableName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
