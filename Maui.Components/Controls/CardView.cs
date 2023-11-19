namespace Maui.Components.Controls;

public class CardView : ContentView
{
    #region Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSourceProperty),
        typeof(ImageSource),
        typeof(CardView),
        null
    );

    public ImageSource ImageSource 
    {
        get => (ImageSource) GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    
    #endregion
}
