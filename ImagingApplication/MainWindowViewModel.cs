using System.Diagnostics;
using ImagingTools;
using Microsoft.Win32;

namespace ImagingApplication
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class MainWindowViewModel : BindableBase
    {
        private string processedText;
        private bool isProcessing;
        private Tools imageTools;
        private string selectedImagePath;

        public MainWindowViewModel()
        {
            this.SelectedImagePath = @"test images\Straight.jpg";
            this.imageTools = new Tools();
            this.SelectImageFileCommand = new DelegateCommand(this.SelectImageFile, this.CanSelectImageFile).ObservesProperty(() => this.IsProcessing);
            this.PerformOcrCommand = new DelegateCommand(this.PerformOcr, this.CanPerformOcr).ObservesProperty(() => this.IsProcessing);
            this.FindObjectInSceneCommand = new DelegateCommand(this.FindObjectInScene, this.CanFindObjectInScene).ObservesProperty(() => this.IsProcessing);
            this.PerformEyeDetectionCommand = new DelegateCommand(this.PerformEyeDetection, this.CanPerformEyeDetection).ObservesProperty(() => this.IsProcessing);
        }

        public ICommand SelectImageFileCommand { get; }

        public ICommand PerformOcrCommand { get; }

        public ICommand FindObjectInSceneCommand { get; }

        public ICommand PerformEyeDetectionCommand { get; }

        public string SelectedImagePath
        {
            get => this.selectedImagePath;
            set => this.SetProperty(ref this.selectedImagePath, value);
        }

        public string ProcessedText
        {
            get => this.processedText;
            set => this.SetProperty(ref this.processedText, value);
        }

        public bool IsProcessing
        {
            get => this.isProcessing;
            set => this.SetProperty(ref this.isProcessing, value);
        }

        private bool CanSelectImageFile()
        {
            return !this.IsProcessing;
        }

        private void SelectImageFile()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "test images";
            openFileDialog.Filter = "Images|*.jpg;*.png;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog().HasValue)
            {
                this.SelectedImagePath = openFileDialog.FileName;
            }
        }

        private bool CanPerformOcr()
        {
            return !this.IsProcessing;
        }

        private async void PerformOcr()
        {
            this.IsProcessing = true;
            this.ProcessedText = "Working...";
            this.ProcessedText = await Task.Run(() => ImagingTools.Tools.PerformOcr(@"test images\helloworld.png"));
            this.IsProcessing = false;
        }

        private bool CanFindObjectInScene()
        {
            return !this.IsProcessing;
        }

        private async void FindObjectInScene()
        {
            this.IsProcessing = true;
            this.ProcessedText = "Working...";
            await Task.Run(() => Tools.IsObjectInScene(@"test images\model.jpg", @"test images\scene.jpg"));
            this.ProcessedText = null;
            this.IsProcessing = false;
        }

        private bool CanPerformEyeDetection()
        {
            return !this.IsProcessing;
        }

        private async void PerformEyeDetection()
        {
            this.IsProcessing = true;
            this.ProcessedText = "Working...";
            //var time = await Task.Run(() => this.imageTools.PerformEyeDetection(@"test images\straight.jpg"));
            //var time = await Task.Run(() => this.imageTools.PerformEyeDetection(@"test images\Straightsmallerbmp.bmp"));
            var time = await Task.Run(() => this.imageTools.PerformEyeDetection(this.SelectedImagePath));
            this.ProcessedText = $"Eye detection completed in {time.TotalMilliseconds:F2} ms";
            this.IsProcessing = false;
        }
    }
}
