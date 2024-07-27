using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class EntryLikeButton : ContentView
{
    public event EventHandler? Clicked;

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryLikeButton));
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryLikeButton));
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    private readonly Label _PlaceholderLabel = new Label()
        .FontSize(16)
        .Bold()
        .CenterVertical();
    private readonly Label _TextLabel = new Label()
        .FontSize(16)
        .Bold()
        .CenterVertical();
    private readonly Image _ChevronDown = new Image()
        .ApplyMaterialIcon(MaterialIcon.Arrow_drop_down, 18, UIService.Color("TextColor"));
    private readonly Border _Border = new Border()
        .Height(DeviceInfo.Current.Platform == DevicePlatform.iOS ? 40 : 50)
        .BorderColor(Colors.DarkGray)
        .BorderThickness(2)
        .BorderShape(new RoundRectangle { CornerRadius = 5 })
        .Padding(new Thickness(8, 0, 8, 0));

    public EntryLikeButton()
    {
        _PlaceholderLabel.TextColor = Color.FromArgb("#646464");
        _TextLabel.SetDynamicResource(Label.TextColorProperty, "TextColor");
        _Border.SetDynamicResource(Border.BackgroundColorProperty, "EBGC");

        _Border.Content = new Grid 
        {
            ColumnDefinitions = Columns.Define(Star, 40),
            ColumnSpacing = 12,
            Children = 
            {
                _TextLabel.Column(0),
                _ChevronDown.Column(1)
            }
        };
        Content = new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                _PlaceholderLabel,
                _Border
            }
        };

        UpdateThemeRelatedItems();
        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (msg.Value)
                {
                    case InternalMessage.ThemeChanged:
                        UpdateThemeRelatedItems();
                        break;
                }
            });
        });

        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, EventArgs.Empty);
        });
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextProperty.PropertyName)
        {
            _TextLabel.Text = Text;
        }
        else if (propertyName == PlaceholderProperty.PropertyName)
        {
            _PlaceholderLabel.Text = Placeholder;
        }
    }

    private void UpdateThemeRelatedItems()
    {
        if (_PlaceholderLabel == null) // disposed
            return;

        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
        _PlaceholderLabel.TextColor = color;
    }
}
