using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Components.Controls;

public enum FloatingActionButtonStyle
{
	Small,
	Regular,
	Large,
	Extended,
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
		Colors.Transparent
	);

	public Color FABBackgroundColor
	{
		get => (Color)GetValue(FABBackgroundColorProperty);
		set => SetValue(FABBackgroundColorProperty, value);
	}

	public static readonly BindableProperty TextProperty = BindableProperty.Create(
		nameof(TextProperty),
		typeof(string),
		typeof(FloatingActionButton),
		string.Empty
	);

	public string Text 
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
		nameof(TextColorProperty),
		typeof(Color),
		typeof(FloatingActionButton),
		null
	);

	public Color TextColor 
	{
		get => (Color)GetValue(TextColorProperty);
		set => SetValue(TextColorProperty, value);
	}

	private readonly TapGestureRecognizer _FABClick = new()
	{
		NumberOfTapsRequired = 1
	};

	private readonly HorizontalStackLayout _FABLayout = new()
	{
		HorizontalOptions = LayoutOptions.Center,
		Spacing = 4,
	};
	private readonly Border _FABContainer = new() { Stroke = Colors.Transparent };
	private readonly Image _FABImage = new()
	{
		VerticalOptions = LayoutOptions.Center,
		HorizontalOptions = LayoutOptions.Center
	};
	private readonly Label _FABLabel = new()
	{
		FontSize = 16,
		FontAttributes = FontAttributes.Bold,
		VerticalOptions = LayoutOptions.Center
	};

	public FloatingActionButton()
	{
		GestureRecognizers.Add(_FABClick);
		
		_FABContainer.Content = _FABLayout;
		
		Content = _FABContainer;

		Loaded += FABLoaded;
		Unloaded += FABUnloaded;
	}

	private void FABLoaded(object sender, EventArgs e)
	{
		Application.Current.RequestedThemeChanged += ThemeChanged;
		_FABClick.Tapped += FABHasBeenClicked;
	}

	private void FABUnloaded(object sender, EventArgs e)
	{
		Application.Current.RequestedThemeChanged -= ThemeChanged;
		_FABClick.Tapped -= FABHasBeenClicked;
	}

	private async void FABHasBeenClicked(object sender, TappedEventArgs e)
	{
		await this.ScaleTo(0.95, 70);
		await this.ScaleTo(1.0, 70);

		Clicked?.Invoke(this, null);
	}

	private void ThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		ApplyShadow();
	}

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
		if (propertyName == ImageSourceProperty.PropertyName)
		{
			_FABImage.Source = ImageSource;
		}
		else if (propertyName == TextProperty.PropertyName)
		{
			_FABLabel.Text = Text;
		}
		else if (propertyName == TextColorProperty.PropertyName)
		{
			_FABLabel.TextColor = TextColor;
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
		_FABContainer.BackgroundColor = FABBackgroundColor;
		_FABLayout.Clear();
		switch (FABStyle)
		{
			case FloatingActionButtonStyle.Small:
				ApplyShadow();
				_FABLayout.Add(_FABImage);
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 12 };
				_FABContainer.HeightRequest = 40;
				_FABContainer.WidthRequest = 40;
				_FABImage.HeightRequest = 24;
				_FABImage.WidthRequest = 24;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Regular:
				ApplyShadow();
				_FABLayout.Add(_FABImage);
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 16 };
				_FABContainer.HeightRequest = 56;
				_FABContainer.WidthRequest = 56;
				_FABImage.HeightRequest = 24;
				_FABImage.WidthRequest = 24;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Large:
				ApplyShadow();
				_FABLayout.Add(_FABImage);
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 28 };
				_FABContainer.HeightRequest = 96;
				_FABContainer.WidthRequest = 96;
				_FABImage.HeightRequest = 36;
				_FABImage.WidthRequest = 36;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Extended:
				ApplyShadow();
				if (ImageSource != null)
				{
					_FABLayout.Add(_FABImage);
				}
				_FABLayout.Add(_FABLabel);
				_FABContainer.StrokeShape = new RoundRectangle { CornerRadius = 16 };
				_FABContainer.HeightRequest = 56;
				_FABImage.HeightRequest = 24;
				_FABImage.WidthRequest = 24;
				_FABImage.Aspect = Aspect.AspectFit;
				break;
			case FloatingActionButtonStyle.Unknown:
			default:
				break;
		}
	}

	private void ApplyShadow()
	{
		if (FABBackgroundColor.Equals(Colors.Transparent))
		{
			return;
		}
		if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
		{
			_FABContainer.Shadow = new Shadow 
			{
				Brush = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.DarkGray : Color.FromRgb(86, 86, 84), 
				Radius = 2, 
				Offset = new Point(0, 1) 
			};
		}
		else
		{
			_FABContainer.Shadow = new Shadow 
			{ 
				Brush = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.DarkGray : Color.FromRgb(86, 86, 84),
				Radius = 10, 
				Offset = new Point(0, 5) 
			};
		}
	}
}