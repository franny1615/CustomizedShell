using FDMobile.Pages;

namespace FDMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (!File.Exists(MoveNetUtility.ModelPath))
            {
                Task.Run(async () => 
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("movenetV4.onnx");
                    using var memStream = new MemoryStream();
                    stream.CopyTo(memStream);
                    File.WriteAllBytes(MoveNetUtility.ModelPath, memStream.ToArray());
                });
            }

            MainPage = new AppShell();
        }
    }
}
