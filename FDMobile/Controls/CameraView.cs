using FDMobile.Controls.Handlers;

namespace FDMobile.Controls;

public class CameraView : View 
{
	public static readonly BindableProperty PositionProperty =
		BindableProperty.Create(nameof(Position), typeof(CameraPosition), typeof(CameraView), CameraPosition.Unset);

	public CameraPosition Position
	{
		get => (CameraPosition)GetValue(PositionProperty);
		set => SetValue(PositionProperty, value);	
	}
}

public static class CameraViewExtensions
{
	public static MauiAppBuilder UseCameraView(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers((handlers) =>
		{
            handlers.AddHandler(typeof(CameraView), typeof(CameraViewHandler));
		});
		return builder;
	}
}

public enum CameraPosition
{
	Unset,
	Front,
	Rear
}