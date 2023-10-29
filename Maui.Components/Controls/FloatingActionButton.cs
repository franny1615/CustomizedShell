using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Components.Controls;

public enum FloatingActionButtonStyle
{
	Small,
	Regular,
	Large,
	Unknown
}

public class FloatingActionButton : ContentView 
{
	public event EventHandler<ClickedEventArgs> Clicked;

	public static readonly BindableProperty FABStyleProperty = BindableProperty.Create(
		nameof(FABStyleProperty),
		typeof(FloatingActionButtonStyle),
		typeof(FloatingActionButton),
		FloatingActionButtonStyle.Unknown
	);

	public FloatingActionButtonStyle FABStyle 
	{
		get => (FloatingActionButtonStyle)GetValue(FABStyleProperty);
		set => SetValue(FABStyleProperty, value);
	}

	public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
		nameof(ImageSourceProperty),
		typeof(ImageSource),
		typeof(FloatingActionButton),
		null
	);

	public ImageSource ImageSource 
	{
		get => (ImageSource)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public static readonly BindableProperty FABBackgroundColorProperty = BindableProperty.Create(
		nameof(FABBackgroundColorProperty),
		typeof(Color),
		typeof(FloatingActionButton),
		null
	);

	public Color FABBackgroundColor
	{
		get => (Color)GetValue(FABBackgroundColorProperty);
		set => SetValue(FABBackgroundColorProperty, value);
	}

	private readonly TapGestureRecognizer _FABClick = new()
	{
		NumberOfTapsRequired = 1
	};

	private readonly Grid _FABLayout = new();
	private readonly Border _FABContainer = new();
	private readonly Image _FABImage = new()
	{
		VerticalOptions = LayoutOptions.Center,
		HorizontalOptions = LayoutOptions.Center
	};

	public FloatingActionButton()
	{
		GestureRecognizers.Add(_FABClick);
		_FABContainer.Content = _FABImage;
		_FABLayout.Children.Add(_FABContainer);
		Content = _FABLayout;

		Loaded += FABLoaded;
		Unloaded += FABUnloaded;
	}

	private void FABLoaded(object sender, EventArgs e)
	{
		_FABClick.Tapped += FABHasBeenClicked;
	}

	private void FABUnloaded(object sender, EventArgs e)
	{
		_FABClick.Tapped -= FABHasBeenClicked;
	}

	private async void FABHasBeenClicked(object sender, TappedEventArgs e)
	{
		await this.ScaleTo(0.8, 40);
		await this.ScaleTo(1.0, 40);

		Clicked?.Invoke(this, null);
	}

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
		if (propertyName == ImageSourceProperty.PropertyName)
		{
			_FABImage.Source = ImageSource;
		}
		else if (propertyName == FABBackgroundColorProperty.PropertyName)
		{
			_FABContainer.BackgroundColor = FABBackgroundColor;
		}
		else if (propertyName == FABStyleProperty.PropertyName)
		{
			ApplyFABStyle();
		}
    }

	private void ApplyFABStyle()
	{
		switch (FABStyle)
		{
			case FloatingActionButtonStyle.Small:
				_FABLayout.Shadow = new Shadow { Brush = Colors.DarkGray, Radius = 2, Offset = new Point(0, 1) };
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 12 };
				_FABContainer.HeightRequest = 40;
				_FABContainer.WidthRequest = 40;
				_FABImage.HeightRequest = 24;
				_FABImage.WidthRequest = 24;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Regular:
				_FABLayout.Shadow = new Shadow { Brush = Colors.DarkGray, Radius = 2, Offset = new Point(0, 1) };
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 16 };
				_FABContainer.HeightRequest = 56;
				_FABContainer.WidthRequest = 56;
				_FABImage.HeightRequest = 24;
				_FABImage.WidthRequest = 24;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Large:
				_FABLayout.Shadow = new Shadow { Brush = Colors.DarkGray, Radius = 2, Offset = new Point(0, 1) };
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 28 };
				_FABContainer.HeightRequest = 96;
				_FABContainer.WidthRequest = 96;
				_FABImage.HeightRequest = 36;
				_FABImage.WidthRequest = 36;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Unknown:
			default:
				break;
		}
	}
}