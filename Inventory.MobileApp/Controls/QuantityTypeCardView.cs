using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using System.Runtime.CompilerServices;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.Controls;

public class QuantityTypeCardView : Border 
{
    public event EventHandler? Delete;
    public event EventHandler? Edit;

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(QuantityTypeCardView), null);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    private readonly Label _Description = new();
    private readonly Image _TrashIcon = new();
    private readonly Image _EditIcon = new(); 
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };
    private readonly TouchBehavior _EditTouch = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };

    public QuantityTypeCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");
        _Description.SetDynamicResource(Label.TextColorProperty, "TextColor");

        _TouchBehavior.Command = new Command(() => Delete?.Invoke(this, EventArgs.Empty));
        _EditTouch.Command = new Command(() => Edit?.Invoke(this, EventArgs.Empty));

        _Description
            .Font(size: 16)
            .Bold()
            .CenterVertical();
        _EditIcon
            .Center()
            .Behaviors([_EditTouch])
            .ApplyMaterialIcon(MaterialIcon.Edit, 24, Color.FromArgb("#646464"));
        _TrashIcon
            .Center()
            .Behaviors([_TouchBehavior])
            .ApplyMaterialIcon(MaterialIcon.Delete, 24, Colors.Red);

        Content = new Grid
        {
            ColumnDefinitions = Columns.Define(Star, 32, 32),
            ColumnSpacing = 0,
            Children =
            {
                _Description.Column(0),
                _EditIcon.Column(1),
                _TrashIcon.Column(2),
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
