using System.Web;
using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Provides basic methods for work with captcha.
    /// </summary>
    public interface ICaptchaManager
    {
        /// <summary>
        /// Gets or sets the storage to save a captcha tokens.
        /// </summary>
        IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// Create a <see cref="IBuildInfoModel"/> for create a new captcha.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns>An instance of <see cref="IBuildInfoModel"/>.</returns>
        IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer);

        /// <summary>
        /// Create a new <see cref="IDrawingModel"/> for drawing a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IDrawingModel"/>.</returns>
        IDrawingModel GetDrawingModel(HttpRequestBase request);

        /// <summary>
        /// Create a new <see cref="IBuildInfoModel"/> for update a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IUpdateInfoModel"/>.</returns>
        IUpdateInfoModel Update(HttpRequestBase request);

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer);
    }
}