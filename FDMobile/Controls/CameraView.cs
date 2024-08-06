using FDMobile.Controls.Handlers;

namespace FDMobile.Controls;

public class CameraView : View 
{
	public static readonly BindableProperty PositionProperty =
		BindableProperty.Create(nameof(Position), typeof(CameraPosition), typeof(CameraView), CameraPosition.Unset);

	public static readonly BindableProperty CurrentImageSampleProperty = 
		BindableProperty.Create(nameof(CurrentImageSample), typeof(byte[]), typeof(CameraView));

	public CameraPosition Position
	{
		get => (CameraPosition)GetValue(PositionProperty);
		set => SetValue(PositionProperty, value);	
	}

	public byte[] CurrentImageSample
	{
		get => (byte[])GetValue(CurrentImageSampleProperty);
		set => SetValue(CurrentImageSampleProperty, value);	
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