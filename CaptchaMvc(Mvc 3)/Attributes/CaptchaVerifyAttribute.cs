using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Models;

namespace CaptchaMvc.Attributes
{
    /// <summary>
    /// Attribute to validate the captcha.
    /// </summary>
    public class CaptchaVerifyAttribute : ActionFilterAttribute
    {
        #region Fields

        private readonly string _textError;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaVerifyAttribute"/> class.
        /// </summary>
        public CaptchaVerifyAttribute(string textError)
        {
            _textError = textError;
        }

        #endregion

        #region Override

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CaptchaUtils.ValidateCaptcha(filterContext.Controller,
                                         new ParameterModel(DefaultCaptchaManager.ErrorAttribute, GetErrorMessage()));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an error message.
        /// </summary>
        /// <returns>The error message.</returns>
        protected virtual string GetErrorMessage()
        {
            return _textError;
        }

        #endregion
    }
}