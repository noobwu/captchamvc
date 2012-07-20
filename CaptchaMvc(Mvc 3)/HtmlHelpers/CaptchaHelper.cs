﻿using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Models;

namespace CaptchaMvc.HtmlHelpers
{
    /// <summary>
    /// Provides extension methods to work with the captcha.
    /// </summary>
    public static class CaptchaHelper
    {
        #region Public methods

        /// <summary>
        /// Create a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="length">The specified length of characters.</param>
        /// <returns>The html string with a captcha.</returns>
        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper, int length)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
        }

        /// <summary>
        /// Create a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="textRefreshButton">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="length">The specified length of characters.</param>
        /// <returns>The html string with a captcha.</returns>
        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper, string textRefreshButton, string inputText,
                                            int length)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText),
                                                new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, textRefreshButton),
                                                new ParameterModel(DefaultCaptchaManager.LengthAttribute, length));
        }

        /// <summary>
        /// Create a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="textRefreshButton">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="length">The specified length of characters.</param>
        /// <param name="requiredMessageText">The specified required message text.</param>
        /// <returns>The html string with a captcha.</returns>
        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper, string textRefreshButton, string inputText,
                                            int length, string requiredMessageText)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText),
                                                new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, textRefreshButton),
                                                new ParameterModel(DefaultCaptchaManager.LengthAttribute, length),
                                                new ParameterModel(DefaultCaptchaManager.IsRequiredAttribute, true),
                                                new ParameterModel(DefaultCaptchaManager.RequiredMessageAttribute, requiredMessageText));
        }

        /// <summary>
        /// Create a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <returns>The html string with a math captcha.</returns>
        public static MvcHtmlString MathCaptcha(this HtmlHelper htmlHelper)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
        }

        /// <summary>
        /// Create a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="textRefreshButton">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <returns>The html string with a math captcha.</returns>
        public static MvcHtmlString MathCaptcha(this HtmlHelper htmlHelper, string textRefreshButton, string inputText)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText),
                                                new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, textRefreshButton),
                                                new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true));
        }

        /// <summary>
        /// Create a new math captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="textRefreshButton">The specified refresh button text.</param>
        /// <param name="inputText">The specified input text.</param>
        /// <param name="requiredMessageText">The specified required message text.</param>
        /// <returns>The html string with a math captcha.</returns>
        public static MvcHtmlString MathCaptcha(this HtmlHelper htmlHelper, string textRefreshButton, string inputText,
                                                string requiredMessageText)
        {
            return CaptchaUtils.GenerateCaptcha(htmlHelper,
                                                new ParameterModel(DefaultCaptchaManager.InputTextAttribute, inputText),
                                                new ParameterModel(DefaultCaptchaManager.RefreshTextAttribute, textRefreshButton),
                                                new ParameterModel(DefaultCaptchaManager.MathCaptchaAttribute, true),
                                                new ParameterModel(DefaultCaptchaManager.IsRequiredAttribute, true),
                                                new ParameterModel(DefaultCaptchaManager.RequiredMessageAttribute, requiredMessageText));
        }

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controllerBase">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="textError">The specified error message.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        public static bool IsCaptchaVerify(this ControllerBase controllerBase, string textError)
        {
            return CaptchaUtils.ValidateCaptcha(controllerBase,
                                                new ParameterModel(DefaultCaptchaManager.ErrorAttribute, textError));
        }

        #endregion
    }
}