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

        public MainWindowViewModel()
        {
            this.PerformOcrCommand = new DelegateCommand(this.PerformOcr, this.CanPerformOcr).ObservesProperty(() => this.IsProcessing);
        }

        public ICommand PerformOcrCommand { get; }

        public string ProcessedText
        {
            get
            {
                return this.processedText;
            }

            set
            {
                this.SetProperty(ref this.processedText, value);
            }
        }

        public bool IsProcessing
        {
            get
            {
                return this.isProcessing;
            }

            set
            {
                this.SetProperty(ref this.isProcessing, value);
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
    }
}
