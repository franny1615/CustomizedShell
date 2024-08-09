using Inventory.API.Models;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
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
            var input = ConvertImageToFloatTensorUnsafe(imageStream);
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

    public static DenseTensor<int> ConvertImageToFloatTensorUnsafe(Stream imageStream)
    {
        DenseTensor<int> data = new DenseTensor<int>([1, ImageWidth, ImageHeight, 3]);

        Bitmap image = new Bitmap(imageStream);    
        
        BitmapData bmd = image.LockBits(
            new Rectangle(0, 0, image.Width, image.Height), 
            ImageLockMode.ReadOnly, 
            image.PixelFormat);

        int PixelSize = 3;

        unsafe
        {
            for (int y = 0; y < bmd.Height; y++)
            {
                // row is a pointer to a full row of data with each of its colors
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                for (int x = 0; x < bmd.Width; x++)
                {           
                    data[0, y, x, 0] = row[x*PixelSize + 0];
                    data[0, y, x, 1] = row[x*PixelSize + 1];
                    data[0, y, x, 2] = row[x*PixelSize + 2];
                }
            }

            image.UnlockBits(bmd);
        }
        
        return data;
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