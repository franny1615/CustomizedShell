using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public partial class MaterialPaginationModel : ObservableObject
{
    [ObservableProperty]
    public int currentPage = 1;

    [ObservableProperty]
    public int totalPages = 1;
}

public class PageChangedEventArgs : EventArgs
{
    public int Page { get; set; } = -1;
}

public class MaterialPagination : Border
{
    #region Event
    public event EventHandler<PageChangedEventArgs> PageChanged;
    #endregion

    #region Public Properties
    public static readonly BindableProperty CurrentPageProperty = BindableProperty.Create(nameof(CurrentPageProperty), typeof(int), typeof(MaterialPagination), 1);
    public int CurrentPage
    {
        get => (int)GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    public static readonly BindableProperty TotalPagesProperty = BindableProperty.Create(nameof(TotalPagesProperty), typeof(int), typeof(MaterialPagination), 1);
    public int TotalPages 
    { 
        get => (int)GetValue(TotalPagesProperty); 
        set => SetValue(TotalPagesProperty, value); 
    }

    public static readonly BindableProperty ContentColorProperty = BindableProperty.Create(nameof(ContentColorProperty), typeof(Color), typeof(MaterialPagination), null);
    public Color ContentColor
    {
        get => (Color)GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }

    #endregion

    #region Private Properties
    private MaterialPaginationModel _Model => (MaterialPaginationModel)BindingContext;
    private readonly MaterialImage _Previous = new()
    {
        Icon = MaterialIcon.Chevron_left,
        IconSize = 35,
    };
    private readonly MaterialImage _Next = new()
    {
        Icon = MaterialIcon.Chevron_right,
        IconSize = 35,
    };
    private readonly Label _CurrentPageLabel = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
    };
    private readonly Grid _ContentLayout = new()
    {
        ColumnDefinitions = Columns.Define(40, Star, 40),
        ColumnSpacing = 4
    };
    #endregion

    #region Constructor
    public MaterialPagination(MaterialPaginationModel model)
    {
        BindingContext = model;

        StrokeShape = new RoundRectangle { CornerRadius = 5 };
        HeightRequest = 40;

        this.SetBinding(CurrentPageProperty, "CurrentPage");
        this.SetBinding(TotalPagesProperty, "TotalPages");

        _ContentLayout.Children.Add(_Previous.Column(0).Center());
        _ContentLayout.Children.Add(_CurrentPageLabel.Column(1).Center());
        _ContentLayout.Children.Add(_Next.Column(2).Center());

        _Previous.TapGesture(Previous);
        _Next.TapGesture(Next);

        UI();

        Content = _ContentLayout;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == CurrentPageProperty.PropertyName ||
            propertyName == TotalPagesProperty.PropertyName)
        {
            UI();
        }
        else if (propertyName == ContentColorProperty.PropertyName)
        {
            _Previous.IconColor = ContentColor;
            _Next.IconColor = ContentColor;
            _CurrentPageLabel.TextColor = ContentColor;
        }
    }
    #endregion

    #region Helpers
    private void UI()
    {
        if (_Model.CurrentPage == 1)
        {
            _Previous.IsVisible = false;
        }
        else if (_Model.CurrentPage > 1)
        {
            _Previous.IsVisible = true;
        }

        if (_Model.CurrentPage == _Model.TotalPages)
        {
            _Next.IsVisible = false;
        }
        else if (_Model.CurrentPage < _Model.TotalPages) 
        {
            _Next.IsVisible = true; 
        }

        _CurrentPageLabel.Text = $"{_Model.CurrentPage} / {_Model.TotalPages}";
    }

    private void Previous()
    {
        if (_Model.CurrentPage - 1 <= 0)
        {
            _Model.CurrentPage = 1;
        }
        else
        {
            _Model.CurrentPage--;
        }

        PageChanged?.Invoke(this, new() { Page = _Model.CurrentPage });
    }

    private void Next()
    {
        if (_Model.CurrentPage + 1 > _Model.TotalPages)
        {
            _Model.CurrentPage = _Model.TotalPages;
        }
        else
        {
            _Model.CurrentPage++;
        }

        PageChanged?.Invoke(this, new() { Page = _Model.CurrentPage });
    }
    #endregion
}
