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
                    var font = FontFace.HersheyPlain;

                    foreach(var imageFeature in imageFeatures)
                    {
                        var x = (int)imageFeature.Point.X;
                        var y = (int)imageFeature.Point.Y;
                        var point = new System.Drawing.Point(x, y);
                        imageWithFeatures.Draw("x", point, font, 1d, new Gray());
                    }

                    ImageViewer.Show(imageWithFeatures);
                }
            }

            return false;
        }
    }
}
