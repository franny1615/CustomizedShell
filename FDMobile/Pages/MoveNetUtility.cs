using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using System.Reflection;

namespace FDMobile.Pages;

public class MoveNetUtility
{
    public static string ModelPath => Path.Combine(FileSystem.AppDataDirectory, "movenetV4.onnx");
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
            var input = GetInput(imageStream);
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

    private static DenseTensor<int> GetInput(Stream imageStream)
    {
        DenseTensor<int> data = new DenseTensor<int>([1, ImageWidth, ImageHeight, 3]);

        using var sourceBitmap = SKBitmap.Decode(imageStream);
        var pixels = sourceBitmap.Bytes;

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

                data[0, y, x, 0] = pixelR;
                data[0, y, x, 1] = pixelG;
                data[0, y, x, 2] = pixelB;

                channelDataIndex++;
            }
        }

        return data;
    }
}

public class RepoResult<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
