using System.Linq;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha intelligence using the javascript.
    /// </summary>
    public class JavaScriptIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/>";
        private const string ScriptValue =
            @"<script>$(function () {{var tok = document.getElementById(""{0}"");var inv = tok.value.split('').reverse().join('');$('<input>').attr({{ type: 'hidden', name: '{1}', value: inv }}).appendTo(tok.form);}});</script>";
        private readonly DefaultCaptchaManager _captchaManager;
        private string _validationInputName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptIntelligencePolicy" /> class.
        /// </summary>
        public JavaScriptIntelligencePolicy(DefaultCaptchaManager captchaManager, string validationInputName = null)
        {
            Validate.ArgumentNotNull(captchaManager, "captchaManager");
            if (string.IsNullOrEmpty(validationInputName))
                validationInputName = "validation_token";
            _captchaManager = captchaManager;
            _validationInputName = validationInputName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of input field, if you use the intelligent captcha.
        /// </summary>
        public string ValidationInputName
        {
            get { return _validationInputName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "ValidationInputName");
                _validationInputName = value;
            }
        }

        #endregion

        #region Implementation of IIntelligencePolicy

        /// <summary>
        ///     Determines whether the intelligence captcha is valid.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the intelligence captcha is valid; <c>false</c> not valid; <c>null</c> is not intelligence captcha.
        /// </returns>
        public bool? IsValid(ControllerBase controller, IParameterContainer parameterContainer)
        {
            ValueProviderResult tokenValue = controller.ValueProvider.GetValue(_captchaManager.TokenElementName);
            if (tokenValue == null || string.IsNullOrEmpty(tokenValue.AttemptedValue))
                return null;
            if (!controller.TempData.Remove(tokenValue.AttemptedValue))
                return null;
            ValueProviderResult validationVal = controller.ValueProvider.GetValue(ValidationInputName);
            return validationVal != null && _captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue) &&
                   Reverse(validationVal.AttemptedValue) == tokenValue.AttemptedValue;
        }

        /// <summary>
        ///     Makes the specified captcha "intelligent".
        /// </summary>
        /// <param name="captcha">
        ///     The specified <see cref="ICaptcha" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        public ICaptcha MakeIntelligent(ICaptcha captcha, IParameterContainer parameterContainer)
        {
            Validate.ArgumentNotNull(captcha, "captcha");
            captcha.BuildInfo.HtmlHelper.ViewContext.TempData[captcha.BuildInfo.TokenValue] = true;
            return new IntelligentCaptchaDecorator(captcha, RenderMarkup, RenderScript);
        }

        #endregion

        #region Methods

        private static string Reverse(string st)
        {
            return new string(st.Reverse().ToArray());
        }

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        protected virtual string RenderMarkup(ICaptcha captcha)
        {
            return string.Format(InputHtml, captcha.BuildInfo.TokenValue, captcha.BuildInfo.TokenElementId);
        }

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        protected virtual string RenderScript(ICaptcha captcha)
        {
            return string.Format(ScriptValue, captcha.BuildInfo.TokenElementId, ValidationInputName);
        }

        #endregion
    }
}