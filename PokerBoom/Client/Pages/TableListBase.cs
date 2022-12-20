using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace PokerBoom.Client.Pages
{
    public class TableListBase : ComponentBase
    {
        [Inject] protected NavigationManager _navigationManager { get; set; }
        protected int selectedRowNumber = -1;
        protected MudTable<Element> mudTable;
        protected string TextValue { get; set; }
        protected record Element(string Name, int Players, int SB);
        protected string searchString1 { get; set; }
        protected Element selectedItem1 = null;

        protected string SelectedRowClassFunc(Element element, int rowNumber)
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

        protected IEnumerable<Element> Elements = new List<Element>()
        {
            new Element("стол 1", 3, 10),
            new Element("стол 2", 2, 50),
            new Element("стол 3", 5, 5),
            new Element("стол 4", 0, 200),
        };

        protected bool FilterFunc1(Element element) => FilterFunc(element, searchString1);

        protected bool FilterFunc(Element element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        protected async Task ConnectToTable()
        {
            _navigationManager.NavigateTo("/game");
        }
    }
}
