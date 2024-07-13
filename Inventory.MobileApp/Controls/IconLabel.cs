using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public enum IconLabelType
{
    Text,
    Toggle
}

public class IconLabel : Grid
{
    public event EventHandler<ToggledEventArgs>? Toggled;

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(IconLabel), null);
    public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(IconLabel), null);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(IconLabel), null);
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }

    public static readonly BindableProperty LabelTypeProperty = BindableProperty.Create(nameof(LabelType), typeof(IconLabelType), typeof(IconLabel), IconLabelType.Text);
    public IconLabelType LabelType { get => (IconLabelType)GetValue(LabelTypeProperty); set => SetValue(LabelTypeProperty, value); }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(IconLabel), false);
    public bool IsToggled { get => (bool)GetValue(IsToggledProperty); set => SetValue(IsToggledProperty, value); }

    private readonly Label _Header = new Label().FontSize(10).TextColor(Color.FromArgb("#c7c7cc")).Bold().Start();
    private readonly Label _Text = new Label { MaxLines = 3, LineBreakMode = LineBreakMode.WordWrap, VerticalOptions = LayoutOptions.Center }.FontSize(16).Bold();
    private readonly Image _Icon = new() { VerticalOptions = LayoutOptions.Center };
    private readonly Switch _Switch = new() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

    public IconLabel()
    {
        Margin = 0;
        Padding = 0;
        RowSpacing = 0;
        ColumnSpacing = 8;
        MinimumHeightRequest = 48;

        ColumnDefinitions = Columns.Define(Star, 16);
        RowDefinitions = Rows.Define(Auto, Star);

        _Switch.Toggled += (s, e) => 
        { 
            IsToggled = e.Value;
            Toggled?.Invoke(this, e); 
        };

        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (msg.Value)
                {
                    case InternalMessage.ThemeChanged:
                        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
                        UpdateIcon(color);
                        UpdateHeader(color);
                        break;
                }
            });
        });
    }

    private void UpdateIcon(Color color)
    {
        if (_Icon != null && !string.IsNullOrEmpty(Icon))
        {
            _Icon.ApplyMaterialIcon(Icon, 14, color);
        }
    }

    private void UpdateHeader(Color color)
    {
        if (_Header != null && !string.IsNullOrEmpty(Header))
        {
            _Header.TextColor = color;
            _Header.Text = Header;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
        if (propertyName == TextProperty.PropertyName)
        {
            _Text.Text = Text;
            LayoutItems();
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            UpdateIcon(color);
            LayoutItems();
        }
        else if (propertyName == HeaderProperty.PropertyName)
        {
            UpdateHeader(color);
            LayoutItems();
        }
        else if (propertyName == LabelTypeProperty.PropertyName)
        {
            LayoutItems();
        }
        else if (propertyName == IsToggledProperty.PropertyName)
        {
            _Switch.IsToggled = IsToggled;
        }
    }

    private void LayoutItems()
    {
        Clear();
        if (!string.IsNullOrEmpty(Header))
        {
            Add(_Header.Row(0).Column(0).ColumnSpan(2));
            LayoutTextItems();
        }
        else
        {
            LayoutTextItems();
        }
    }

    private void LayoutTextItems()
    {
        if (string.IsNullOrEmpty(Icon))
        {
            switch (LabelType)
            {
                case IconLabelType.Text:
                    Add(_Text.Row(1).Column(0).ColumnSpan(2));
                    break;
                case IconLabelType.Toggle:
                    Add(_Switch.Row(1).Column(0).ColumnSpan(2));
                    break;
            }
        }
        else
        {
            switch (LabelType)
            {
                case IconLabelType.Text:
                    Add(_Text.Row(1).Column(0).ColumnSpan(1));
                    Add(_Icon.Row(1).Column(1));
                    break;
                case IconLabelType.Toggle:
                    Add(_Switch.Row(1).Column(0).ColumnSpan(1));
                    Add(_Icon.Row(1).Column(1));
                    break;
            }
        }
    }
}
