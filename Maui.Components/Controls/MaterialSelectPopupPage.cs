using CommunityToolkit.Maui.Markup;
using Maui.Components.Pages;
using System.Collections.ObjectModel;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public enum SelectType
{
    SingleSelect,
    MultiSelect
}

public interface ISelectItem
{
    public int Id { get; set; }
    public string HeadLine { get; set; }
    public string SupportingText { get; set; }
    public bool IsSelected { get; set; }
}

public interface ISelectViewModel : IMaterialListVM<ISelectItem> 
{
    public string NoItemsText { get; set; }
    public string ItemsIcon { get; set; }
    public string Title { get; set; }
    public SelectType SelectType { get; set; }

    public ObservableCollection<ISelectItem> SelectedItems { get; set; }
}

public class MaterialSelectPopupPage : PopupPage
{
    #region Private Properties
    private ISelectViewModel _ViewModel => (ISelectViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(50, Star),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        ColumnSpacing = 8,
        RowSpacing = 0,
        Padding = 8
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
    };
    private readonly MaterialImage _Close = new()
    {
        Icon = MaterialIcon.Close,
        IconSize = 30,
        IconColor = Application.Current.Resources["TextColor"] as Color
    };
    private readonly MaterialList<ISelectItem> _Search;
    #endregion

    #region Constructor
    public MaterialSelectPopupPage(
        ILanguageService languageService,
        ISelectViewModel selectViewModel) : base(languageService)
    {
        BindingContext = selectViewModel;
        _LangService = languageService;

        _Title.Text = _ViewModel.Title;

        _Search = new(selectViewModel.NoItemsText, selectViewModel.ItemsIcon, new DataTemplate(() => 
        {
            var view = new MaterialCardView();
            view.Icon = selectViewModel.ItemsIcon;
            view.TrailingIcon = MaterialIcon.Done;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.SetBinding(MaterialCardView.HeadlineProperty, "HeadLine");
            view.SetBinding(MaterialCardView.SupportingTextProperty, "SupportingText");
            view.SetBinding(MaterialCardView.TrailingIconIsVisibleProperty, "IsSelected");
            view.Clicked += Clicked;

            return view;
        }), selectViewModel, isEditable: false);

        _Close.TapGesture(() => Navigation.PopModalAsync());

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());
        _ContentLayout.Children.Add(_Search.Row(1).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;

        _Search.FetchedNewPage += RefreshSelectedItem;
    }
    ~MaterialSelectPopupPage()
    {
        _Search.FetchedNewPage -= RefreshSelectedItem;
    }

    private void RefreshSelectedItem(object sender, EventArgs e)
    {
        for (int item = 0; item < _ViewModel.Items.Count; item++)
        {
            for (int selected = 0; selected < _ViewModel.SelectedItems.Count; selected++)
            {
                if (_ViewModel.SelectedItems[selected].Id == _ViewModel.Items[item].Id)
                {
                    _ViewModel.Items[item].IsSelected = true;
                }
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.FetchPublic();
    }

    private async void Clicked(object sender, EventArgs e)
    {
        if (sender is MaterialCardView card && card.BindingContext is ISelectItem item)
        {
            if (item.IsSelected)
            {
                item.IsSelected = false;
            }
            else
            {
                item.IsSelected = true;
            }

            switch (_ViewModel.SelectType)
            {
                case SelectType.SingleSelect:
                    _ViewModel.SelectedItems.Clear();

                    if (item.IsSelected)
                    {
                        _ViewModel.SelectedItems.Add(item);   
                    }

                    // update ui list to reflect change
                    for (int i = 0;  i < _ViewModel.Items.Count; i++)
                    {
                        if (_ViewModel.Items[i].Id == item.Id)
                        {
                            _ViewModel.Items[i].IsSelected = item.IsSelected;
                        }
                        else
                        {
                            _ViewModel.Items[i].IsSelected = false;
                        }
                    }

                    await Navigation.PopModalAsync();

                    break;
                case SelectType.MultiSelect:
                    // remove it from selected list when its unselected
                    int foundIndex = -1;
                    for (int i = 0; i < _ViewModel.SelectedItems.Count; i++)
                    {
                        if (_ViewModel.SelectedItems[i].Id == item.Id)
                        {
                            if (!item.IsSelected)
                            {
                                foundIndex = i; 
                                break;
                            }
                        }
                    }
                    if (foundIndex > -1)
                    {
                        _ViewModel.SelectedItems.RemoveAt(foundIndex);
                    }

                    if (item.IsSelected)
                    {
                        _ViewModel.SelectedItems.Add(item);
                    }

                    // update ui list to reflect change
                    for (int i = 0; i < _ViewModel.SelectedItems.Count; i++)
                    {
                        if (_ViewModel.Items[i].Id == item.Id)
                        {
                            _ViewModel.Items[i].IsSelected = item.IsSelected;
                        }
                    }

                    break;
            }
        }
    }
    #endregion
}