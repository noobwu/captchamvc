using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// A base class for work with captcha.
    /// </summary>
    public class DefaultCaptchaBuilderProvider : ICaptchaBuilderProvider
    {
        #region Fields

        private static readonly byte[] ErrorBytes = CreateErrorBitmap();

        #endregion

        #region Implementation of ICaptchaBuilderProvider

        /// <summary>
        /// Create a new captcha to the specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the captcha.</returns>
        public virtual MvcHtmlString GenerateCaptcha(IBuildInfoModel buildInfoModel)
        {
            if (buildInfoModel == null) throw new ArgumentNullException("buildInfoModel");
            return GetCaptchaBuilder(buildInfoModel).Build(buildInfoModel);
        }

        /// <summary>
        /// Create a captcha image for specified <see cref="IDrawingModel"/> and write it in response.
        /// </summary>
        /// <param name="response">The specified <see cref="HttpResponseBase"/>.</param>
        /// <param name="drawingModel">The specified <see cref="IDrawingModel"/>.</param>
        public virtual void WriteCaptchaImage(HttpResponseBase response, IDrawingModel drawingModel)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (drawingModel == null) throw new ArgumentNullException("drawingModel");
            using (Bitmap bitmap = CaptchaUtils.ImageGenerator.Generate(drawingModel.Text))
            {
                response.ContentType = "image/gif";
                bitmap.Save(response.OutputStream, ImageFormat.Gif);
            }
        }

        /// <summary>
        /// Create a captcha error image and write it in response.
        /// </summary>
        /// <param name="response">The specified <see cref="HttpResponse"/>.</param>
        public virtual void WriteErrorImage(HttpResponseBase response)
        {
            if (response == null) throw new ArgumentNullException("response");
            response.ContentType = "image/gif";
            response.OutputStream.Write(ErrorBytes, 0, ErrorBytes.Length);
        }

        /// <summary>
        /// Generate a java-script to update the captcha.
        /// </summary>
        /// <param name="updateInfo">The specified <see cref="IUpdateInfoModel"/>.</param>
        /// <returns>The specified <see cref="ActionResult"/> to update the captcha.</returns>
        public virtual ActionResult RefreshCaptcha(IUpdateInfoModel updateInfo)
        {
            if (updateInfo == null)
                throw new ArgumentNullException("updateInfo");
            string script = string.Format(@"$('#{0}').attr(""value"", ""{1}"");
$('#{2}').attr(""src"", ""{3}"");", updateInfo.TokenElementId,
                                          updateInfo.TokenValue,
                                          updateInfo.ImageElementId, updateInfo.ImageUrl);
            return new JavaScriptResult {Script = script};
        }

        #endregion

        #region Method

        /// <summary>
        /// Get the <see cref="ICaptchaBulder"/> for build captcha with specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>An instance of <see cref="ICaptchaBulder"/>.</returns>
        protected virtual ICaptchaBulder GetCaptchaBuilder(IBuildInfoModel buildInfoModel)
        {
            if (buildInfoModel is DefaultBuildInfoModel)
                return new DefaultCaptchaBuilder();
            if (buildInfoModel is MathBuildInfoModel)
                return new MathCaptchaBuilder();
            throw new NotSupportedException(
                "DefaultCaptchaBuilderProvider does not support the type of a IBuildInfoModel = " +
                buildInfoModel.GetType());
        }

        private static byte[] CreateErrorBitmap()
        {
            using (var errorBmp = new Bitmap(200, 70))
            {
                using (Graphics gr = Graphics.FromImage(errorBmp))
                {
                    gr.DrawLine(Pens.Red, 0, 0, 200, 70);
                    gr.DrawLine(Pens.Red, 0, 70, 200, 0);
                }
                using (var memoryStream = new MemoryStream())
                {
                    errorBmp.Save(memoryStream, ImageFormat.Gif);
                    return memoryStream.ToArray();
                }
            }
        }

        #endregion
    }
}