using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using PokerBoom.Client.States;
using PokerBoom.Shared.Models;
using System.Net.Http.Json;

namespace PokerBoom.Client.Pages
{
    public class GameListBase : ComponentBase
    {
        [Inject] protected ILocalStorageService _localStorage { get; set; }
        [Inject] protected NavigationManager _navigationManager { get; set; }
        [Inject] protected HttpClient _httpClient { get; set; }
        [Inject] protected BalanceState _balanceState { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public AuthenticationState AuthState { get; set; }

        protected int selectedRowNumber = -1;
        protected MudTable<GameReview>? mudTable { get; set; }
        protected GameReview? Game { get; set; }
        protected string? searchGameString { get; set; }
        protected GameReview? selectedGame { get; set; }
        protected IEnumerable<GameReview>? Games { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var result = await _httpClient.GetFromJsonAsync<GetGamesResultViewModel>("/api/games");
            if (result.Successful)
            {
                Games = result.Games;
            }

            var response = await _httpClient.GetAsync($"/api/balance?username={AuthState.User.Identity.Name}");
            int balance = (await response.Content.ReadFromJsonAsync<GetBalanceViewModel>()).Balance;

            _balanceState.OnBalanceChanged.Invoke(balance);

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        protected async Task ViewGame()
        {
            await _localStorage.SetItemAsync("reviewGameId", selectedGame.Id);
            _navigationManager.NavigateTo("/reviewgame");
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
