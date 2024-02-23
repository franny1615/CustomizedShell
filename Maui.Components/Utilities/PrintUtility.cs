#if ANDROID
using Android.Graphics;
using Android.Util;
using AndroidX.Print;
#endif

#if IOS
using Foundation;
using UIKit;
#endif

namespace Maui.Components.Utilities;

public class PrintUtility
{
    public static void PrintBase64Image(string imageBase64)
    {
#if ANDROID
        var helper = new PrintHelper(Platform.CurrentActivity);
        helper.ScaleMode = PrintHelper.ScaleModeFit;

        byte[] decodedString = Base64.Decode(imageBase64, Base64Flags.Default);
        Bitmap map = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length); 

        helper.PrintBitmap("Print", map);
#endif

#if IOS
        UIPrintInfo printInfo = UIPrintInfo.FromDictionary(new());
        printInfo.OutputType = UIPrintInfoOutputType.Photo;
        printInfo.JobName = "Print";

        UIPrintInteractionController printController = UIPrintInteractionController.SharedPrintController;
        printController.PrintInfo = printInfo;
        printController.PrintingItem = UIImage.LoadFromData(new NSData(imageBase64, NSDataBase64DecodingOptions.IgnoreUnknownCharacters));

        printController.Present(true, (_, _, _) => { });
#endif
    }
}
