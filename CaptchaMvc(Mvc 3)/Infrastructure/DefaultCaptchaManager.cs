using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Provides basic methods for work with captcha.
    /// </summary>
    public class DefaultCaptchaManager : ICaptchaManager
    {
        #region Fields

        private string _imageElementName;
        private string _inputElementName;
        private IStorageProvider _storageProvider;
        private string _tokenElementName;
        private string _tokenParameterName;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCaptchaManager"/> class.
        /// </summary>
        public DefaultCaptchaManager()
            : this(new SessionStorageProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCaptchaManager"/> class.
        /// </summary>
        public DefaultCaptchaManager(IStorageProvider storageProvider)
            : this(storageProvider, "t", "CaptchaInputText", "CaptchaImage", "CaptchaDeText")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCaptchaManager"/> class.
        /// </summary>
        public DefaultCaptchaManager(IStorageProvider storageProvider, string tokenParameterName,
                                     string inputElementName, string imageElementName,
                                     string tokenElementName)
        {
            if (storageProvider == null) throw new ArgumentNullException("storageProvider");
            if (string.IsNullOrEmpty(tokenParameterName))
                throw new ArgumentNullException("tokenParameterName");
            if (string.IsNullOrEmpty(inputElementName))
                throw new ArgumentNullException("inputElementName");
            if (string.IsNullOrEmpty(imageElementName))
                throw new ArgumentNullException("imageElementName");
            if (string.IsNullOrEmpty(tokenElementName))
                throw new ArgumentNullException("tokenElementName");
            StorageProvider = storageProvider;
            TokenParameterName = tokenParameterName;
            InputElementName = inputElementName;
            ImageElementName = imageElementName;
            TokenElementName = tokenElementName;
        }

        #endregion

        #region Parameters

        /// <summary>
        /// The required parameter key.
        /// </summary>
        public const string IsRequiredAttribute = "__________IsRequired____________";

        /// <summary>
        /// The required message parameter key.
        /// </summary>
        public const string RequiredMessageAttribute = "__________RequiredMessage____________";

        /// <summary>
        /// The error message parameter key.
        /// </summary>
        public const string ErrorAttribute = "______ErrorAttribute______";

        /// <summary>
        /// The length of characters parameter key.
        /// </summary>
        public const string LengthAttribute = "_______LengthAttribute_______";

        /// <summary>
        /// The refresh button text parameter key.
        /// </summary>
        public const string RefreshTextAttribute = "_______RefreshTextAttribute_______";

        /// <summary>
        /// The input text parameter key.
        /// </summary>
        public const string InputTextAttribute = "_______InputTextAttribute_______";

        /// <summary>
        /// The math parameter key.
        /// </summary>
        public const string MathCaptchaAttribute = "__m__";

        /// <summary>
        /// The partial view key.
        /// </summary>
        public const string PartialViewNameAttribute = "____PartialViewNameAttribute____";

        /// <summary>
        /// The partial view data attribute.
        /// </summary>
        public const string PartialViewDataAttribute = "____PartialViewDataAttribute____";

        #endregion

        #region Property

        /// <summary>
        /// The token parameter name.
        /// </summary>
        protected internal string TokenParameterName
        {
            get { return _tokenParameterName; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The property TokenParameterName can not be null.");
                _tokenParameterName = value;
            }
        }

        /// <summary>
        /// The input element name in DOM.
        /// </summary>
        protected internal string InputElementName
        {
            get { return _inputElementName; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The property InputElementName can not be null.");
                _inputElementName = value;
            }
        }

        /// <summary>
        /// The image element name in DOM.
        /// </summary>
        protected internal string ImageElementName
        {
            get { return _imageElementName; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The property ImageElementName can not be null.");
                _imageElementName = value;
            }
        }

        /// <summary>
        /// The token element name in DOM.
        /// </summary>
        protected internal string TokenElementName
        {
            get { return _tokenElementName; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The property TokenElementName can not be null.");
                _tokenElementName = value;
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// Create an <see cref="IBuildInfoModel"/> for the specified <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <param name="captchaPair">The specified <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <param name="imgUrl">The specified image url.</param>
        /// <param name="refreshUrl">The specified refresh url.</param>
        /// <returns>An instance of <see cref="IBuildInfoModel"/>.</returns>
        protected virtual IBuildInfoModel CreateBuildInfo(HtmlHelper htmlHelper, IParameterContainer parameterContainer,
                                                          KeyValuePair<string, ICaptchaValue> captchaPair, string imgUrl,
                                                          string refreshUrl)
        {
            string requiredText = null;
            string refreshText;
            string inputText;
            parameterContainer.TryGet(RefreshTextAttribute, out refreshText, "Refresh");
            bool findInputText = parameterContainer.TryGet(InputTextAttribute, out inputText);
            bool isRequired = parameterContainer.IsContain(IsRequiredAttribute);
            if (isRequired)
                parameterContainer.TryGet(RequiredMessageAttribute, out requiredText, "This is a required field.");

            IBuildInfoModel buildInfo;
            if (parameterContainer.IsContain(MathCaptchaAttribute))
                buildInfo = new MathBuildInfoModel(TokenParameterName, MathCaptchaAttribute, isRequired, requiredText,
                                                   refreshText, findInputText ? inputText : "The answer is", htmlHelper,
                                                   InputElementName, TokenElementName,
                                                   ImageElementName, imgUrl, refreshUrl, captchaPair.Key);
            else
                buildInfo = new DefaultBuildInfoModel(TokenParameterName, requiredText, isRequired,
                                                      refreshText, findInputText ? inputText : "Input symbols",
                                                      htmlHelper,
                                                      InputElementName, ImageElementName, TokenElementName, refreshUrl,
                                                      imgUrl,
                                                      captchaPair.Key);

            //If is it a partial view.
            if (parameterContainer.IsContain(PartialViewNameAttribute))
            {
                ViewDataDictionary viewData;
                parameterContainer.TryGet(PartialViewDataAttribute, out viewData);
                return new PartialBuildInfoModel(htmlHelper, buildInfo,
                                                 parameterContainer.Get<string>(PartialViewNameAttribute), viewData);
            }
            return buildInfo;
        }

        /// <summary>
        /// Generate a specified <see cref="KeyValuePair{TKey,TValue}"/> for a captcha.
        /// </summary>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <param name="oldValue">The old value if any.</param>
        /// <returns>An instance of <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        protected virtual KeyValuePair<string, ICaptchaValue> CreateCaptchaPair(IParameterContainer parameterContainer,
                                                                                ICaptchaValue oldValue)
        {
            if (parameterContainer.IsContain(MathCaptchaAttribute))
                return GenerateMathCaptcha();

            int length;
            if (oldValue != null)
                length = oldValue.CaptchaText.Length;
            else if (!parameterContainer.TryGet(LengthAttribute, out length))
                throw new ArgumentException("Parameter is not specified for the length of the captcha.");
            if (length <= 0)
                throw new ArgumentException("The length parameter can not be <= 0.");
            return GenerateSimpleCaptcha(length);
        }

        /// <summary>
        /// Create a new <see cref="IDrawingModel"/> for drawing a captcha.
        /// </summary>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <param name="captchaValue">The specified <see cref="ICaptchaValue"/>.</param>
        /// <returns>An instance of <see cref="IDrawingModel"/>.</returns>
        protected virtual IDrawingModel CreateDrawingModel(IParameterContainer parameterContainer,
                                                           ICaptchaValue captchaValue)
        {
            return new DefaultDrawingModel(captchaValue.CaptchaText);
        }

        /// <summary>
        /// Generate a specified <see cref="KeyValuePair{TKey,TValue}"/> for a math captcha.
        /// </summary>
        /// <returns>An instance of <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        protected virtual KeyValuePair<string, ICaptchaValue> GenerateMathCaptcha()
        {
            int first = RandomNumber.Next(100, 1000);
            int second = RandomNumber.Next(1, 100);
            string text;
            int result;
            int next = RandomNumber.Next(0, 1);
            switch (next)
            {
                case 0:
                    text = string.Format("{0} + {1} = ?", first, second);
                    result = first + second;
                    break;
                case 1:
                    text = string.Format("{0} - {1} = ?", first, second);
                    result = first - second;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                                                           new NumberCaptchaValue(text, result));
        }

        /// <summary>
        /// Generate a specified <see cref="KeyValuePair{TKey,TValue}"/> for a text captcha.
        /// </summary>
        /// <param name="length">The specified length of characters.</param>
        /// <returns>An instance of <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        protected virtual KeyValuePair<string, ICaptchaValue> GenerateSimpleCaptcha(int length)
        {
            string randomText = RandomText.Generate(GetCharacters(), length);
            return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                                                           new StringCaptchaValue(randomText, randomText, true));
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="controllerBase">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        protected virtual void WriteError(ControllerBase controllerBase, IParameterContainer parameterContainer)
        {
            string errorText;
            parameterContainer.TryGet(ErrorAttribute, out errorText, "Captcha is not valid");
            controllerBase.ViewData.ModelState.AddModelError(InputElementName, errorText);
        }

        /// <summary>
        /// Generate a URL for a captcha image.
        /// </summary>
        /// <param name="urlHelper">The specified <see cref="UrlHelper"/>.</param>
        /// <param name="captchaPair">The specified <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <returns>The url of captcha image.</returns>
        protected virtual string GenerateImageUrl(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            return urlHelper.Action("Generate", "DefaultCaptcha",
                                    new RouteValueDictionary {{TokenParameterName, captchaPair.Key}});
        }

        /// <summary>
        /// Generate a URL for a refresh captcha.
        /// </summary>
        /// <param name="urlHelper">The specified <see cref="UrlHelper"/>.</param>
        /// <param name="captchaPair">The specified <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <returns>The url of refresh captcha.</returns>
        protected virtual string GenerateRefreshUrl(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            return urlHelper.Action("Refresh", "DefaultCaptcha");
        }

        /// <summary>
        /// Get the characters for creating captcha.
        /// </summary>
        /// <returns>The characters.</returns>
        protected virtual string GetCharacters()
        {
            string chars = ConfigurationManager.AppSettings["CaptchaChars"];
            if (string.IsNullOrEmpty(chars))
                chars = "qwertyuipasdfghjklzcvbnm123456789";
            return chars;
        }

        #endregion

        #region Implementation of ICaptchaManager

        /// <summary>
        /// Gets or sets the storage to save a captcha tokens.
        /// </summary>
        public IStorageProvider StorageProvider
        {
            get { return _storageProvider; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The property StorageProvider can not be null.");
                _storageProvider = value;
            }
        }

        /// <summary>
        /// Create a <see cref="IBuildInfoModel"/> for create a new captcha.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns>An instance of <see cref="IBuildInfoModel"/>.</returns>
        public virtual IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (parameterContainer == null)
                throw new ArgumentNullException("parameterContainer");
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, null);
            StorageProvider.Add(captchaPair);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string imgUrl = GenerateImageUrl(urlHelper, captchaPair);
            string refreshUrl = GenerateRefreshUrl(urlHelper, captchaPair);
            return CreateBuildInfo(htmlHelper, parameterContainer, captchaPair, imgUrl, refreshUrl);
        }

        /// <summary>
        /// Create a new <see cref="IDrawingModel"/> for drawing a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IDrawingModel"/>.</returns>
        public virtual IDrawingModel GetDrawingModel(HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            string token = request.Params[TokenParameterName];
            if (string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue captchaValue = StorageProvider.GetDrawingValue(token);
            if (captchaValue == null)
                throw new ArgumentException("The key is to generate incorrect.");
            return CreateDrawingModel(new RequestParameterContainer(request), captchaValue);
        }

        /// <summary>
        /// Create a new <see cref="IBuildInfoModel"/> for update a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IUpdateInfoModel"/>.</returns>
        public virtual IUpdateInfoModel Update(HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            IParameterContainer parameterContainer = new RequestParameterContainer(request);
            string token;
            parameterContainer.TryGet(TokenParameterName, out token, null);
            if (string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue captchaValue = StorageProvider.GetValidationValue(token);
            if (captchaValue == null)
                throw new ArgumentException("The key is to update incorrect.");
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, captchaValue);
            string newUrl = GenerateImageUrl(new UrlHelper(request.RequestContext), captchaPair);
            StorageProvider.Add(captchaPair);
            return new DefaultUpdateInfoModel(TokenElementName, captchaPair.Key, newUrl, ImageElementName);
        }

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        public virtual bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            if (parameterContainer == null)
                throw new ArgumentNullException("parameterContainer");
            string tokenValue = controller.ValueProvider.GetValue(TokenElementName).AttemptedValue;
            string inputText = controller.ValueProvider.GetValue(InputElementName).AttemptedValue;
            ICaptchaValue captchaValue = StorageProvider.GetValidationValue(tokenValue);
            if (captchaValue == null || string.IsNullOrEmpty(inputText))
            {
                WriteError(controller, parameterContainer);
                return false;
            }
            bool isVerify = captchaValue.IsEqual(inputText);
            if (isVerify)
                return true;
            WriteError(controller, parameterContainer);
            return false;
        }

        #endregion
    }
}