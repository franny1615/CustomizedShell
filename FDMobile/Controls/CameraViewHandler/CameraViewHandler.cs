using Microsoft.Maui.Handlers;

namespace FDMobile.Controls.Handlers;

public partial class CameraViewHandler
{
    public static IPropertyMapper<CameraView, CameraViewHandler> PropertyMapper = new PropertyMapper<CameraView, CameraViewHandler>(ViewHandler.ViewMapper)
    {
        [nameof(CameraView.Position)] = MapCameraPosition
    };

    public static CommandMapper<CameraView, CameraViewHandler> CommandMapper = new(ViewCommandMapper)
    {

    };

    public CameraViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }
}
