using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System.IO;

namespace ImagingTools
{
    public static class Tools
    {
        public static string PerformOcr(string imagePath)
        {
            using (var image = new Image<Bgr, byte>(Path.GetFullPath(imagePath)))
            {
                using (var ocrProvider = new Tesseract(@"tesseract data\", "eng", OcrEngineMode.Default))
                {
                    ocrProvider.SetImage(image);
                    ocrProvider.Recognize();
                    var text = ocrProvider.GetUTF8Text();
                    return text;
                }
            }
        }
    }
}
