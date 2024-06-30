using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class StatusCardView : Border
{
    public event EventHandler? Delete;

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(StatusCardView), null);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    private readonly Label _Description = new();
    private readonly Image _TrashIcon = new();
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };

    public StatusCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");
        _Description.SetDynamicResource(Label.TextColorProperty, "TextColor");

        _TouchBehavior.Command = new Command(() => Delete?.Invoke(this, EventArgs.Empty));

        _Description
            .Font(size: 16)
            .Bold()
            .CenterVertical();
        _TrashIcon
            .Center()
            .Behaviors([_TouchBehavior])
            .ApplyMaterialIcon(MaterialIcon.Delete, 16, Colors.Red);

        Content = new Grid
        {
            ColumnDefinitions = Columns.Define(Star, 24),
            Children =
            {
                _Description.Column(0),
                _TrashIcon.Column(1),
            }
        };
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == DescriptionProperty.PropertyName)
        {
            _Description.Text = Description;
        }
    }
}