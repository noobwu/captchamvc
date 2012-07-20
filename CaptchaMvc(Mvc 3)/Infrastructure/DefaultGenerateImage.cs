using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// The base implementation of the generation of images.
    /// </summary>
    public class DefaultGenerateImage : IGenerateImage
    {
        #region Fields

        private const int Width = 200;
        private const int Height = 70;

        private const double WarpFactor = 1.6;
        private const double XAmp = WarpFactor*Width/100;
        private const double YAmp = WarpFactor*Height/85;
        private const double XFreq = 2*Math.PI/Width;
        private const double YFreq = 2*Math.PI/Height;


        private readonly FontFamily[] _fonts =
            {
                new FontFamily("Times New Roman"),
                new FontFamily("Georgia"),
                new FontFamily("Arial"),
                new FontFamily("Comic Sans MS")
            };

        #endregion

        #region IGenerateImage Members

        /// <summary>
        /// Creating an image for a Captcha.
        /// </summary>
        /// <param name="captchaText">Text Captcha.</param>
        /// <returns></returns>
        public virtual Bitmap Generate(string captchaText)
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                var rect = new Rectangle(0, 0, Width, Height);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (var solidBrush = new SolidBrush(Color.White))
                {
                    graphics.FillRectangle(solidBrush, rect);
                }

                //Randomly choose the font name.
                FontFamily family = _fonts[RandomNumber.Next(_fonts.Length - 1)];
                int size = (Width*2/captchaText.Length);
                var font = new Font(family, size);

                //Select the font size.
                var meas = new SizeF(0, 0);
                while (size > 2 && (meas = graphics.MeasureString(captchaText, font)).Width > Width ||
                       meas.Height > Height)
                {
                    font.Dispose();
                    size -= 2;
                    font = new Font(family, size);
                }

                using (var fontFormat = new StringFormat())
                {
                    //Format the font in the center.
                    fontFormat.Alignment = StringAlignment.Center;
                    fontFormat.LineAlignment = StringAlignment.Center;

                    var path = new GraphicsPath();
                    path.AddString(captchaText, font.FontFamily, (int) font.Style, font.Size, rect, fontFormat);
                    using (var solidBrush = new SolidBrush(Color.Blue))
                    {
                        graphics.FillPath(solidBrush, DeformPath(path));
                    }
                }
                font.Dispose();
            }
            return bmp;
        }

        #endregion

        #region Method

        /// <summary>
        /// Deform the specified <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="graphicsPath">The specified <see cref="GraphicsPath"/></param>
        /// <returns>The deformed <see cref="GraphicsPath"/>.</returns>
        protected static GraphicsPath DeformPath(GraphicsPath graphicsPath)
        {
            var deformed = new PointF[graphicsPath.PathPoints.Length];
            var rng = new Random();
            double xSeed = rng.NextDouble()*2*Math.PI;
            double ySeed = rng.NextDouble()*2*Math.PI;
            for (int i = 0; i < graphicsPath.PathPoints.Length; i++)
            {
                PointF original = graphicsPath.PathPoints[i];
                double val = XFreq*original.X*YFreq*original.Y;
                var xOffset = (int) (XAmp*Math.Sin(val + xSeed));
                var yOffset = (int) (YAmp*Math.Sin(val + ySeed));
                deformed[i] = new PointF(original.X + xOffset, original.Y + yOffset);
            }
            return new GraphicsPath(deformed, graphicsPath.PathTypes);
        }

        #endregion
    }
}