using GenCode128;
using System.Drawing;

namespace Maui.Inventory.Api.Utilities;

public static class BarcodeGenerator
{
    private const int CQuietWidth = 10;

    private static readonly int[,] CPatterns = new int[107, 8]
    {
        { 2, 1, 2, 2, 2, 2, 0, 0 },
        { 2, 2, 2, 1, 2, 2, 0, 0 },
        { 2, 2, 2, 2, 2, 1, 0, 0 },
        { 1, 2, 1, 2, 2, 3, 0, 0 },
        { 1, 2, 1, 3, 2, 2, 0, 0 },
        { 1, 3, 1, 2, 2, 2, 0, 0 },
        { 1, 2, 2, 2, 1, 3, 0, 0 },
        { 1, 2, 2, 3, 1, 2, 0, 0 },
        { 1, 3, 2, 2, 1, 2, 0, 0 },
        { 2, 2, 1, 2, 1, 3, 0, 0 },
        { 2, 2, 1, 3, 1, 2, 0, 0 },
        { 2, 3, 1, 2, 1, 2, 0, 0 },
        { 1, 1, 2, 2, 3, 2, 0, 0 },
        { 1, 2, 2, 1, 3, 2, 0, 0 },
        { 1, 2, 2, 2, 3, 1, 0, 0 },
        { 1, 1, 3, 2, 2, 2, 0, 0 },
        { 1, 2, 3, 1, 2, 2, 0, 0 },
        { 1, 2, 3, 2, 2, 1, 0, 0 },
        { 2, 2, 3, 2, 1, 1, 0, 0 },
        { 2, 2, 1, 1, 3, 2, 0, 0 },
        { 2, 2, 1, 2, 3, 1, 0, 0 },
        { 2, 1, 3, 2, 1, 2, 0, 0 },
        { 2, 2, 3, 1, 1, 2, 0, 0 },
        { 3, 1, 2, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 2, 2, 2, 0, 0 },
        { 3, 2, 1, 1, 2, 2, 0, 0 },
        { 3, 2, 1, 2, 2, 1, 0, 0 },
        { 3, 1, 2, 2, 1, 2, 0, 0 },
        { 3, 2, 2, 1, 1, 2, 0, 0 },
        { 3, 2, 2, 2, 1, 1, 0, 0 },
        { 2, 1, 2, 1, 2, 3, 0, 0 },
        { 2, 1, 2, 3, 2, 1, 0, 0 },
        { 2, 3, 2, 1, 2, 1, 0, 0 },
        { 1, 1, 1, 3, 2, 3, 0, 0 },
        { 1, 3, 1, 1, 2, 3, 0, 0 },
        { 1, 3, 1, 3, 2, 1, 0, 0 },
        { 1, 1, 2, 3, 1, 3, 0, 0 },
        { 1, 3, 2, 1, 1, 3, 0, 0 },
        { 1, 3, 2, 3, 1, 1, 0, 0 },
        { 2, 1, 1, 3, 1, 3, 0, 0 },
        { 2, 3, 1, 1, 1, 3, 0, 0 },
        { 2, 3, 1, 3, 1, 1, 0, 0 },
        { 1, 1, 2, 1, 3, 3, 0, 0 },
        { 1, 1, 2, 3, 3, 1, 0, 0 },
        { 1, 3, 2, 1, 3, 1, 0, 0 },
        { 1, 1, 3, 1, 2, 3, 0, 0 },
        { 1, 1, 3, 3, 2, 1, 0, 0 },
        { 1, 3, 3, 1, 2, 1, 0, 0 },
        { 3, 1, 3, 1, 2, 1, 0, 0 },
        { 2, 1, 1, 3, 3, 1, 0, 0 },
        { 2, 3, 1, 1, 3, 1, 0, 0 },
        { 2, 1, 3, 1, 1, 3, 0, 0 },
        { 2, 1, 3, 3, 1, 1, 0, 0 },
        { 2, 1, 3, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 1, 2, 3, 0, 0 },
        { 3, 1, 1, 3, 2, 1, 0, 0 },
        { 3, 3, 1, 1, 2, 1, 0, 0 },
        { 3, 1, 2, 1, 1, 3, 0, 0 },
        { 3, 1, 2, 3, 1, 1, 0, 0 },
        { 3, 3, 2, 1, 1, 1, 0, 0 },
        { 3, 1, 4, 1, 1, 1, 0, 0 },
        { 2, 2, 1, 4, 1, 1, 0, 0 },
        { 4, 3, 1, 1, 1, 1, 0, 0 },
        { 1, 1, 1, 2, 2, 4, 0, 0 },
        { 1, 1, 1, 4, 2, 2, 0, 0 },
        { 1, 2, 1, 1, 2, 4, 0, 0 },
        { 1, 2, 1, 4, 2, 1, 0, 0 },
        { 1, 4, 1, 1, 2, 2, 0, 0 },
        { 1, 4, 1, 2, 2, 1, 0, 0 },
        { 1, 1, 2, 2, 1, 4, 0, 0 },
        { 1, 1, 2, 4, 1, 2, 0, 0 },
        { 1, 2, 2, 1, 1, 4, 0, 0 },
        { 1, 2, 2, 4, 1, 1, 0, 0 },
        { 1, 4, 2, 1, 1, 2, 0, 0 },
        { 1, 4, 2, 2, 1, 1, 0, 0 },
        { 2, 4, 1, 2, 1, 1, 0, 0 },
        { 2, 2, 1, 1, 1, 4, 0, 0 },
        { 4, 1, 3, 1, 1, 1, 0, 0 },
        { 2, 4, 1, 1, 1, 2, 0, 0 },
        { 1, 3, 4, 1, 1, 1, 0, 0 },
        { 1, 1, 1, 2, 4, 2, 0, 0 },
        { 1, 2, 1, 1, 4, 2, 0, 0 },
        { 1, 2, 1, 2, 4, 1, 0, 0 },
        { 1, 1, 4, 2, 1, 2, 0, 0 },
        { 1, 2, 4, 1, 1, 2, 0, 0 },
        { 1, 2, 4, 2, 1, 1, 0, 0 },
        { 4, 1, 1, 2, 1, 2, 0, 0 },
        { 4, 2, 1, 1, 1, 2, 0, 0 },
        { 4, 2, 1, 2, 1, 1, 0, 0 },
        { 2, 1, 2, 1, 4, 1, 0, 0 },
        { 2, 1, 4, 1, 2, 1, 0, 0 },
        { 4, 1, 2, 1, 2, 1, 0, 0 },
        { 1, 1, 1, 1, 4, 3, 0, 0 },
        { 1, 1, 1, 3, 4, 1, 0, 0 },
        { 1, 3, 1, 1, 4, 1, 0, 0 },
        { 1, 1, 4, 1, 1, 3, 0, 0 },
        { 1, 1, 4, 3, 1, 1, 0, 0 },
        { 4, 1, 1, 1, 1, 3, 0, 0 },
        { 4, 1, 1, 3, 1, 1, 0, 0 },
        { 1, 1, 3, 1, 4, 1, 0, 0 },
        { 1, 1, 4, 1, 3, 1, 0, 0 },
        { 3, 1, 1, 1, 4, 1, 0, 0 },
        { 4, 1, 1, 1, 3, 1, 0, 0 },
        { 2, 1, 1, 4, 1, 2, 0, 0 },
        { 2, 1, 1, 2, 1, 4, 0, 0 },
        { 2, 1, 1, 2, 3, 2, 0, 0 },
        { 2, 3, 3, 1, 1, 1, 2, 0 }
    };

    public static Image Code128(string description, string inputData, int barWeight, bool addQuietZone)
    {
        int[] codes = new Code128Content(inputData).Codes;
        int num = ((codes.Length - 3) * 11 + 35) * barWeight;
        int height = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(num) * 0.15f));
        if (addQuietZone)
        {
            num += 20 * barWeight;
        }

        float descH = 48F; 

        Image image = new Bitmap(num, 200);
        using Graphics graphics = Graphics.FromImage(image);

        RectangleF rect = new RectangleF(0, 8, num, descH);
        graphics.DrawString(description, new Font("Arial", 12F, FontStyle.Regular), Brushes.Black, rect, new StringFormat
        {
            Alignment = StringAlignment.Center,
        });

        graphics.FillRectangle(Brushes.White, 0, descH, num, height);
        int num2 = (addQuietZone ? (10 * barWeight) : 0);
        foreach (int num3 in codes)
        {
            for (int j = 0; j < 8; j += 2)
            {
                int num4 = CPatterns[num3, j] * barWeight;
                int num5 = CPatterns[num3, j + 1] * barWeight;
                if (num4 > 0)
                {
                    graphics.FillRectangle(Brushes.Black, num2, descH, num4, height);
                }

                num2 += num4 + num5;
            }
        }

        RectangleF rect2 = new RectangleF(0, descH + height, num, 16);
        graphics.DrawString(inputData, new Font("Arial", 12F, FontStyle.Regular), Brushes.Black, rect2, new StringFormat
        {
            Alignment = StringAlignment.Center,
        });

        return image;
    }
}
