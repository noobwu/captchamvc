using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Represents the policy which makes a captcha intelligence using a fake input element.
    /// </summary>
    public class FakeInputIntelligencePolicy : IIntelligencePolicy
    {
        #region Fields

        private const string StyleCss = "<style>#{0} {{ display: none; }}</style>";

        private const string InputHtml =
            @"<input type=""hidden"" value=""{0}"" name=""{1}"" id=""{1}""/><input type=""text"" value="""" name=""{2}"" id=""{2}""/>";

        private readonly DefaultCaptchaManager _captchaManager;
        private string _fakeInputName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeInputIntelligencePolicy" /> class.
        /// </summary>
        public FakeInputIntelligencePolicy(DefaultCaptchaManager captchaManager, string fakeInputName = null)
        {
            Validate.ArgumentNotNull(captchaManager, "captchaManager");
            if (string.IsNullOrEmpty(fakeInputName))
                fakeInputName = "email_required_value";
            _captchaManager = captchaManager;
            _fakeInputName = fakeInputName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of input field, if you use the intelligent captcha.
        /// </summary>
        public string FakeInputName
        {
            get { return _fakeInputName; }
            set
            {
                Validate.PropertyNotNullOrEmpty(value, "FakeInputName");
                _fakeInputName = value;
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
            ValueProviderResult fakeValue = controller.ValueProvider.GetValue(FakeInputName);
            return fakeValue != null && _captchaManager.StorageProvider.Remove(tokenValue.AttemptedValue) &&
                   string.IsNullOrEmpty(fakeValue.AttemptedValue);
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

        /// <summary>
        ///     Renders only captcha markup, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        protected virtual string RenderMarkup(ICaptcha captcha)
        {
            return string.Format(InputHtml, captcha.BuildInfo.TokenValue, captcha.BuildInfo.TokenElementId,
                                 FakeInputName);
        }

        /// <summary>
        ///     Renders only captcha scripts, if any.
        /// </summary>
        /// <returns>
        ///     An instance of string.
        /// </returns>
        protected virtual string RenderScript(ICaptcha captcha)
        {
            return string.Format(StyleCss, FakeInputName);
        }

        #endregion
    }
}