using System.Web;
using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Provides basic methods for creating captcha.
    /// </summary>
    public interface ICaptchaBuilderProvider
    {
        /// <summary>
        /// Create a new captcha to the specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the captcha.</returns>
        MvcHtmlString GenerateCaptcha(IBuildInfoModel buildInfoModel);

        /// <summary>
        /// Create a captcha image for specified <see cref="IDrawingModel"/> and write it in response.
        /// </summary>
        /// <param name="response">The specified <see cref="HttpResponseBase"/>.</param>
        /// <param name="drawingModel">The specified <see cref="IDrawingModel"/>.</param>
        void WriteCaptchaImage(HttpResponseBase response, IDrawingModel drawingModel);

        /// <summary>
        /// Create a captcha error image and write it in response.
        /// </summary>
        /// <param name="response">The specified <see cref="HttpResponse"/>.</param>
        void WriteErrorImage(HttpResponseBase response);

        /// <summary>
        /// Generate a java-script to update the captcha.
        /// </summary>
        /// <param name="updateInfo">The specified <see cref="IUpdateInfoModel"/>.</param>
        /// <returns>The specified <see cref="ActionResult"/> to update the captcha.</returns>
        ActionResult RefreshCaptcha(IUpdateInfoModel updateInfo);
    }
}