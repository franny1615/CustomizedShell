using Inventory.API.Models;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using System.Reflection;

namespace Inventory.API.Utilities;

public class MoveNetUtility
{
    public static string ModelPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "movenetV4.onnx");
    public static MLContext MLContext = new MLContext();
    public const string InputTensorName = "input";
    public const string OutputTensorName = "output_0";
    public const int ImageWidth = 192;
    public const int ImageHeight = 192;

    public static RepoResult<float[]> Predict(Stream imageStream)
    {
        try
        {
            // open model
            using var modelStream = File.OpenRead(ModelPath);
            using var modelMemoryStream = new MemoryStream();
            modelStream.CopyTo(modelMemoryStream);

            var model = modelMemoryStream.ToArray();
            var session = new InferenceSession(model);

            // Predict
            var input = new DenseTensor<int>(GetInput(imageStream), new[]
            {
                1,
                ImageWidth,
                ImageHeight,
                3
            });
            using var results = session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(InputTensorName, input)
            });

            // Merge the prediction array with the labels. Produce tuples of landmark name and its probability.
            var predictions = results.FirstOrDefault(i => i.Name == OutputTensorName)?.AsTensor<float>().ToArray();

            return new RepoResult<float[]> { Data = predictions ?? [], ErrorMessage = null };
        }
        catch (Exception e)
        {
            return new RepoResult<float[]> { Data = null, ErrorMessage = e.ToString() };
        }
    }

    private static int[] GetInput(Stream imageStream)
    {
        using var sourceBitmap = SKBitmap.Decode(imageStream);
        var pixels = sourceBitmap.Bytes;

        // if (sourceBitmap.Width != ImageWidth || sourceBitmap.Height != ImageHeight)
        // {
        //     float ratio = (float)Math.Min(ImageWidth, ImageHeight) / Math.Min(sourceBitmap.Width, sourceBitmap.Height);

        //     using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(
        //         (int)(ratio * sourceBitmap.Width),
        //         (int)(ratio * sourceBitmap.Height)),
        //         SKFilterQuality.Medium);

        //     var horizontalCrop = scaledBitmap.Width - ImageWidth;
        //     var verticalCrop = scaledBitmap.Height - ImageHeight;
        //     var leftOffset = horizontalCrop == 0 ? 0 : horizontalCrop / 2;
        //     var topOffset = verticalCrop == 0 ? 0 : verticalCrop / 2;

        //     var cropRect = SKRectI.Create(
        //         new SKPointI(leftOffset, topOffset),
        //     new SKSizeI(ImageWidth, ImageHeight));

        //     using SKImage currentImage = SKImage.FromBitmap(scaledBitmap);
        //     using SKImage croppedImage = currentImage.Subset(cropRect);
        //     using SKBitmap croppedBitmap = SKBitmap.FromImage(croppedImage);

        //     pixels = croppedBitmap.Bytes;
        // }

        var bytesPerPixel = sourceBitmap.BytesPerPixel;
        var rowLength = ImageWidth * bytesPerPixel;
        var channelLength = ImageWidth * ImageHeight;
        var channelData = new float[channelLength * 3];
        var channelDataIndex = 0;

        for (int y = 0; y < ImageHeight; y++)
        {
            var rowOffset = y * rowLength;

            for (int x = 0, columnOffset = 0; x < ImageWidth; x++, columnOffset += bytesPerPixel)
            {
                var pixelOffset = rowOffset + columnOffset;

                var pixelR = pixels[pixelOffset];
                var pixelG = pixels[pixelOffset + 1];
                var pixelB = pixels[pixelOffset + 2];

                var rChannelIndex = channelDataIndex;
                var gChannelIndex = channelDataIndex + 1;
                var bChannelIndex = channelDataIndex + 2;

                channelData[rChannelIndex] = pixelR; // (pixelR / 255f - 0.485f) / 0.229f;
                channelData[gChannelIndex] = pixelG; // (pixelG / 255f - 0.456f) / 0.224f;
                channelData[bChannelIndex] = pixelB; // (pixelB / 255f - 0.406f) / 0.225f;

                channelDataIndex++;
            }
        }

        var mapped = new int[channelLength * 3];
        for (int i = 0; i < channelData.Length; i++)
        {
            mapped[i] = (int)channelData[i];
        }

        return mapped;
    }
}

public class MoveNetPacket
{
    public MoveNetPacketType Type { get; set; } = MoveNetPacketType.KeepAlive;
    public string Message { get; set; } = string.Empty;
    public float[] Detections { get; set; } = [];
}

public enum MoveNetPacketType
{
    KeepAlive,
    Picture,
    DetectionResult
}