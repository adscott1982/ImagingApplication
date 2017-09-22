using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.XFeatures2D;
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

        public static bool IsObjectInScene(string objectImagePath, string sceneImagePath)
        {
            using (var objectImage = new Image<Gray, byte>(objectImagePath))
            {
                var surfDetector = new SURF(400);
                var imageFeatures = surfDetector.Detect(objectImage);

                using (var imageWithFeatures = new Image<Gray, byte>(objectImage.Bitmap))
                {
                    foreach(var imageFeature in imageFeatures)
                    {
                        var point = new System.Drawing.PointF(imageFeature.Point.X, imageFeature.Point.Y);
                        imageWithFeatures.Draw(new Cross2DF(point, 5, 5), new Gray(), 1);
                    }

                    ImageViewer.Show(imageWithFeatures);
                }
            }

            return false;
        }
    }
}
