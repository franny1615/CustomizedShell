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
        RowDefinitions = Rows.Define(50, Star, Auto),
        ColumnDefinitions = Columns.Define(30, Star),
        ColumnSpacing = 8,
        RowSpacing = 8,
        Padding = new Thickness(16, 8, 16, 8)
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
        _ContentLayout.Children.Add(_Search.Row(1).Column(0).ColumnSpan(2));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;
    }

    private void Clicked(object sender, EventArgs e)
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
                    for (int i = _ViewModel.Items.Count - 1; i > 0; i--)
                    {
                        if (_ViewModel.Items[i].HeadLine == item.HeadLine && _ViewModel.Items[i].SupportingText == item.SupportingText)
                        {
                            _ViewModel.Items[i].IsSelected = item.IsSelected;
                        }
                        else
                        {
                            _ViewModel.Items[i].IsSelected = false;
                        }
                    }

                    break;
                case SelectType.MultiSelect:
                    // remove it from selected list when its unselected
                    for (int i = _ViewModel.SelectedItems.Count - 1; i > 0; i--)
                    {
                        if (_ViewModel.SelectedItems[i].HeadLine == item.HeadLine && _ViewModel.SelectedItems[i].SupportingText == item.SupportingText)
                        {
                            if (!item.IsSelected)
                            {
                                _ViewModel.SelectedItems.RemoveAt(i);
                            }
                        }
                    }

                    if (item.IsSelected)
                    {
                        _ViewModel.SelectedItems.Add(item);
                    }

                    // update ui list to reflect change
                    for (int i = _ViewModel.Items.Count - 1; i > 0; i--)
                    {
                        if (_ViewModel.Items[i].HeadLine == item.HeadLine && _ViewModel.Items[i].SupportingText == item.SupportingText)
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