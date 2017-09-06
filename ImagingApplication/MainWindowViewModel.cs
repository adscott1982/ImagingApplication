namespace ImagingApplication
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class MainWindowViewModel : BindableBase
    {
        private string processedText;

        public MainWindowViewModel()
        {
            this.DoImageCommand = new DelegateCommand(this.DoImage);
        }

        public ICommand DoImageCommand { get; }

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
        private void DoImage()
        {
            this.ProcessedText = ImagingTools.Tools.PerformOcr(@"test images\helloworld.png");
        }
    }
}
