using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Aruco;
using Emgu.CV.Util;

namespace EmguCVTestApp
{
    using Point = System.Drawing.Point;
    using PointF = System.Drawing.PointF;
    /// <summary>
    /// Результат обработки кадра.
    /// </summary>
    public struct FrameResult
    {
        /// <summary>
        /// Центр объекта на кадре.
        /// </summary>
        public Point QRCenter;
        public Point[] QRLocation;

        public PointF[] ArucoLocation;

        public double TiltAngle;
        public double HorizontalParallax;
        public double VerticalParallax;
        /// <summary>
        /// Визуальное отображение полученных данных.
        /// </summary>
        public Image<Bgr, byte> Visual;
        public string Commentary;
        internal FrameResult(Image<Bgr, byte> visual = null)
        {
            QRCenter = new Point();
            QRLocation = null;
            ArucoLocation = null;
            TiltAngle = double.NaN;
            HorizontalParallax = double.NaN;
            VerticalParallax = double.NaN;
            Visual = visual.Clone();
            Commentary = "";
        }
    }

    

    

    public static class PointOperations
    {
        public static PointF Middle(params PointF[] points)
        {
            PointF Res = new PointF(0, 0);
            foreach (PointF p in points)
            {
                Res.X += p.X;
                Res.Y += p.Y;
            }
            Res.X /= points.Length;
            Res.Y /= points.Length;
            return Res;
        }
        public static Point Middle(params Point[] points)
        {
            Point Res = new Point(0, 0);
            foreach (Point p in points)
            {
                Res.X += p.X;
                Res.Y += p.Y;
            }
            Res.X /= points.Length;
            Res.Y /= points.Length;
            return Res;
        }
    }
    /// <summary>
    /// Обрабатывает поступающие кадры.
    /// </summary>
    public class FrameProcessor
    {
        private Dictionary _dict;

        private Dictionary ArucoDictionary
        {
            get
            {
                if (_dict == null)
                    _dict = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_100);
                return _dict;
            }

        }

        ZBar.ImageScanner Scanner = new ZBar.ImageScanner();
        VideoWriter vid = null;
        public FrameProcessor()
        {
            // готовим сканер кодов.
            Scanner.Cache = true;
            Scanner.SetConfiguration(0, ZBar.Config.Enable, 0); //запрещаем искать все коды
            Scanner.SetConfiguration(ZBar.SymbolType.QRCODE, ZBar.Config.Enable, 1); // разрешаем QR
            _detectorParameters = new DetectorParameters();
            _detectorParameters.AdaptiveThreshConstant = 7;
            _detectorParameters.AdaptiveThreshWinSizeMax = 23;
            _detectorParameters.AdaptiveThreshWinSizeMin = 3;
            _detectorParameters.AdaptiveThreshWinSizeStep = 10;
            _detectorParameters.CornerRefinementMaxIterations = 30;
            _detectorParameters.CornerRefinementMinAccuracy = 0.1;
            _detectorParameters.CornerRefinementWinSize = 5;
            _detectorParameters.CornerRefinementMethod = DetectorParameters.RefinementMethod.None;
            _detectorParameters.ErrorCorrectionRate = 0.6;
            _detectorParameters.MarkerBorderBits = 1;
            _detectorParameters.MaxErroneousBitsInBorderRate = 0.35;
            _detectorParameters.MaxMarkerPerimeterRate = 4.0;
            _detectorParameters.MinCornerDistanceRate = 0.05;
            _detectorParameters.MinDistanceToBorder = 3;
            _detectorParameters.MinMarkerDistanceRate = 0.05;
            _detectorParameters.MinMarkerPerimeterRate = 0.03;
            _detectorParameters.MinOtsuStdDev = 5.0;
            _detectorParameters.PerspectiveRemoveIgnoredMarginPerCell = 0.13;
            _detectorParameters.PerspectiveRemovePixelPerCell = 8;
            _detectorParameters.PolygonalApproxAccuracyRate = 0.1;

        }

        private DetectorParameters _detectorParameters;
        Image<Gray, byte> grayframe = null;
        Image<Gray, byte> bwframe = null;
        VectorOfInt ids = new VectorOfInt();
        VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF();
        VectorOfVectorOfPointF rejected = new VectorOfVectorOfPointF();

        /// <summary>
        /// Обрабатывает один поступивший кадр.
        /// </summary>
        /// <param name="frame">Кадр для анализа.</param>
        /// <returns>Сведения об объектах в кадре.</returns>
        public FrameResult ProcessFrame(Image<Bgr, byte> frame)
        {
            if (bwframe == null) bwframe = new Image<Gray, byte>(frame.Size);
            if (grayframe == null) grayframe = new Image<Gray, byte>(frame.Size);
            //if (vid == null) vid = new VideoWriter("video.avi", 24, frame.Size, true);
            vid?.Write(frame.Mat);
            grayframe.ConvertFrom(frame);
            int blocksize = 51;
            CvInvoke.AdaptiveThreshold(grayframe, bwframe, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC, Emgu.CV.CvEnum.ThresholdType.Binary, blocksize, 5);
            FrameResult fr = new FrameResult(frame);

            ArucoInvoke.DetectMarkers(bwframe, ArucoDictionary, corners, ids, _detectorParameters, rejected);
            if (ids.Size > 0)
            {
                fr.ArucoLocation = corners[0].ToArray();
            }
            // Ищем коды на изображении
            List<ZBar.Symbol> symbols = Scanner.Scan(bwframe.ToBitmap());
            if (symbols.Count == 0) return fr;
            ZBar.Symbol s = symbols[0];
            fr.QRLocation = s.Location.ToArray();
            fr.QRCenter = PointOperations.Middle(fr.QRLocation);
            Point RightMiddle = PointOperations.Middle(fr.QRLocation[3], fr.QRLocation[2]);
            fr.TiltAngle = Math.Atan2(RightMiddle.Y - fr.QRCenter.Y, RightMiddle.X - fr.QRCenter.X);
            fr.Commentary  = s.Data;

            Visualize(fr);
            return fr;
        }

        public void Visualize(FrameResult fr)
        {
            if (fr.QRLocation != null)
            {
                fr.Visual.DrawPolyline(fr.QRLocation, true, new Bgr(0, 0, 255), 2);
                fr.Visual.Draw(new CircleF(fr.QRCenter, 6), new Bgr(0, 255, 0), 2);
                fr.Visual.Draw(new CircleF(fr.QRLocation[0], 6), new Bgr(255, 255, 255), 2);
                fr.Visual.Draw(new CircleF(fr.QRLocation[2], 6), new Bgr(255, 0, 0), 2);
                Point TopMiddle = PointOperations.Middle(fr.QRLocation[0], fr.QRLocation[3]);
                fr.Visual.Draw(new LineSegment2D(fr.QRCenter, TopMiddle), new Bgr(0, 255, 0), 2);
            }
            if (fr.ArucoLocation != null)
            {
                ArucoInvoke.DrawDetectedMarkers(fr.Visual, corners, ids, new MCvScalar(0, 255, 0));
            }
        }
    }
}
