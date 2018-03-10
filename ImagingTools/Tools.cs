using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.XFeatures2D;
using System.IO;
using Emgu.CV.Util;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Features2D;
using System.Drawing;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ImagingTools
{
    public class Tools
    {
        private CascadeClassifier faceCascade;
        private CascadeClassifier eyeCascade;

        public Tools()
        {
            this.faceCascade = new CascadeClassifier(@"haar cascades\haarcascade_frontalface_default.xml");
            this.eyeCascade = new CascadeClassifier(@"haar cascades\haarcascade_eye.xml");
        }

        public static string PerformOcr(string imagePath)
        {
            using (var image = new Image<Bgr, byte>(Path.GetFullPath(imagePath)))
            {
                using (var ocrProvider = new Tesseract(@"tesseract data\", "eng", OcrEngineMode.Default))
                {
                    ocrProvider.SetImage(image);
                    ocrProvider.Recognize();
                    var text = ocrProvider.GetUTF8Text();

                    text += "added";
                    return text;
                }
            }
        }

        public static bool IsObjectInScene(string modelImagePath, string sceneImagePath)
        {
            using (var modelImage = new Image<Gray, byte>(modelImagePath))
            using (var sceneImage = new Image<Gray, byte>(sceneImagePath))
            {
                var surfDetector = new SURF(400);

                var modelKeyPoints = new VectorOfKeyPoint(surfDetector.Detect(modelImage));
                var sceneKeyPoints = new VectorOfKeyPoint(surfDetector.Detect(sceneImage));

                var modelDescriptors = ComputeDescriptors(surfDetector, modelKeyPoints, modelImage);
                var sceneDescriptors = ComputeDescriptors(surfDetector, sceneKeyPoints, sceneImage);

                var homoMatrix = Match(modelDescriptors, sceneDescriptors, modelKeyPoints, sceneKeyPoints);

                if (homoMatrix == null)
                {
                    return false;
                }

                //draw a rectangle along the projected model
                Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                PointF[] pts = new PointF[]
                {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                };
                pts = CvInvoke.PerspectiveTransform(pts, homoMatrix);

                Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                using (VectorOfPoint vp = new VectorOfPoint(points))
                using (var displayImage = new Image<Bgr, byte>(sceneImagePath))
                {
                    CvInvoke.Polylines(displayImage, vp, true, new MCvScalar(0, 255, 0), 5);
                    ImageViewer.Show(displayImage);
                }
            }

            
            return false;
        }

        private static UMat ComputeDescriptors(SURF surf, VectorOfKeyPoint imageFeatures, Image<Gray, byte> image)
        {
            var keyPoints = new VectorOfKeyPoint();
            var descriptors = new UMat();

            keyPoints.Push(imageFeatures);

            surf.Compute(image, keyPoints, descriptors);

            return descriptors;
        }

        private static Mat Match(UMat modelDescriptors, UMat sceneDescriptors, VectorOfKeyPoint modelKeyPoints, VectorOfKeyPoint sceneKeyPoints)
        {
            var matcher = new BFMatcher(DistanceType.L2);
            matcher.Add(modelDescriptors);

            var matches = new VectorOfVectorOfDMatch();
            var indicesMatrix = new Matrix<int>(sceneDescriptors.Rows, 2);

            Mat mask;

            using (var distanceMatrix = new Matrix<float>(sceneDescriptors.Rows, 2))
            {
                matcher.KnnMatch(sceneDescriptors, matches, 2, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
            }

            var result = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, sceneKeyPoints, matches, mask, 1.5d, 20);
            var homoMatrix = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, sceneKeyPoints, matches, mask, 2);
            return homoMatrix;
        }

        public TimeSpan PerformEyeDetection(string imagePath)
        {
            var sw = Stopwatch.StartNew();
            using (var image = new Image<Gray, byte>(imagePath))
            {
                var faces = this.faceCascade.DetectMultiScale(image, 1.3d, 5);

                if (faces.Length > 0)
                {
                    foreach (var rectangle in faces)
                    {
                        var subRect = image.GetSubRect(rectangle);

                        var eyes = this.eyeCascade.DetectMultiScale(subRect);

                        foreach (var eye in eyes)
                        {
                            CvInvoke.Rectangle(subRect, eye, new MCvScalar(0, 255, 0));
                        }
                    }
                }
                else
                {
                    var eyes = this.eyeCascade.DetectMultiScale(image, 1.1d, 4);

                    foreach (var eye in eyes)
                    {
                        CvInvoke.Rectangle(image, eye, new MCvScalar(0, 255, 0));
                    }
                }

                sw.Stop();
                ImageViewer.Show(image);
            }

            return sw.Elapsed;
        }
    }
}
