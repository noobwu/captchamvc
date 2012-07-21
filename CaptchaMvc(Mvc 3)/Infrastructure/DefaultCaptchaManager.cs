using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
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

        private const string SessionLockerKey = "____________SessionLocker_____________";
        private const string SessionKeys = "____________SessionKeys_____________";
        private const string SessionDrawingKeys = "____________SessionDrawingKeys_____________";

        private readonly object _locker = new object();
        private string _imageElementName;
        private string _inputElementName;
        private string _tokenElementName;
        private string _tokenParameterName;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCaptchaManager"/> class.
        /// </summary>
        public DefaultCaptchaManager()
            : this("t", "CaptchaInputText", "CaptchaImage", "CaptchaDeText")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCaptchaManager"/> class.
        /// </summary>
        public DefaultCaptchaManager(string tokenParameterName, string inputElementName, string imageElementName,
                                     string tokenElementName)
        {
            if (string.IsNullOrEmpty(tokenParameterName)) throw new ArgumentNullException("tokenParameterName");
            if (string.IsNullOrEmpty(inputElementName)) throw new ArgumentNullException("inputElementName");
            if (string.IsNullOrEmpty(imageElementName)) throw new ArgumentNullException("imageElementName");
            if (string.IsNullOrEmpty(tokenElementName)) throw new ArgumentNullException("tokenElementName");
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

        #endregion

        #region Property

        /// <summary>
        /// The object to synchronize access to the collections, DrawingKeys and ValidateKeys.
        /// </summary>
        protected object SessionLocker
        {
            get
            {
                lock (_locker)
                {
                    object locker = HttpContext.Current.Session[SessionLockerKey];
                    if (locker == null)
                    {
                        locker = new object();
                        HttpContext.Current.Session[SessionLockerKey] = locker;
                    }
                    return locker;
                }
            }
        }

        /// <summary>
        /// Contains tokens that have not yet been validated.
        /// </summary>
        protected IDictionary<string, ICaptchaValue> ValidateKeys
        {
            get
            {
                lock (_locker)
                {
                    var list = HttpContext.Current.Session[SessionKeys] as IDictionary<string, ICaptchaValue>;
                    if (list == null)
                    {
                        list = new Dictionary<string, ICaptchaValue>();
                        HttpContext.Current.Session[SessionKeys] = list;
                    }
                    return list;
                }
            }
        }

        /// <summary>
        /// Contains tokens that have not yet been displayed.
        /// </summary>
        protected IDictionary<string, ICaptchaValue> DrawingKeys
        {
            get
            {
                lock (_locker)
                {
                    var list = HttpContext.Current.Session[SessionDrawingKeys] as IDictionary<string, ICaptchaValue>;
                    if (list == null)
                    {
                        list = new Dictionary<string, ICaptchaValue>();
                        HttpContext.Current.Session[SessionDrawingKeys] = list;
                    }
                    return list;
                }
            }
        }

        /// <summary>
        /// The token parameter name.
        /// </summary>
        protected internal string TokenParameterName
        {
            get { return _tokenParameterName; }
            set
            {
                CaptchaUtils.IsNotNull(value, "The TokenParameterName can not be null.");
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
                CaptchaUtils.IsNotNull(value, "The InputElementName can not be null.");
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
                CaptchaUtils.IsNotNull(value, "The ImageElementName can not be null.");
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
                CaptchaUtils.IsNotNull(value, "The TokenElementName can not be null.");
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

            if (parameterContainer.IsContain(MathCaptchaAttribute))
                return new MathBuildInfoModel(TokenParameterName, MathCaptchaAttribute, isRequired, requiredText,
                                              refreshText, findInputText ? inputText : "The answer is", htmlHelper,
                                              InputElementName, TokenElementName,
                                              ImageElementName, imgUrl, refreshUrl, captchaPair.Key);

            return new DefaultBuildInfoModel(TokenParameterName, requiredText, isRequired,
                                             refreshText, findInputText ? inputText : "Input symbols", htmlHelper,
                                             InputElementName, ImageElementName, TokenElementName, refreshUrl, imgUrl,
                                             captchaPair.Key);
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
        /// Create a <see cref="IBuildInfoModel"/> for create a new captcha.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns>An instance of <see cref="IBuildInfoModel"/>.</returns>
        public virtual IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer)
        {
            KeyValuePair<string, ICaptchaValue> captchaPair = CreateCaptchaPair(parameterContainer, null);
            lock (SessionLocker)
            {
                DrawingKeys.Add(captchaPair);
                ValidateKeys.Add(captchaPair);
            }
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string imgUrl = urlHelper.Action("Generate", "DefaultCaptcha", new {t = captchaPair.Key});
            string refreshUrl = urlHelper.Action("Refresh", "DefaultCaptcha");
            return CreateBuildInfo(htmlHelper, parameterContainer, captchaPair, imgUrl, refreshUrl);
        }

        /// <summary>
        /// Create a new <see cref="IDrawingModel"/> for drawing a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IDrawingModel"/>.</returns>
        public virtual IDrawingModel GetDrawingModel(HttpRequestBase request)
        {
            string token = request.Params[TokenParameterName];
            if (string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue value;
            lock (SessionLocker)
            {
                if (!DrawingKeys.TryGetValue(token, out value))
                    throw new ArgumentException("The key is to generate incorrect.");
                DrawingKeys.Remove(token);
            }
            return CreateDrawingModel(new RequestParameterContainer(request), value);
        }


        /// <summary>
        /// Create a new <see cref="IBuildInfoModel"/> for update a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IUpdateInfoModel"/>.</returns>
        public virtual IUpdateInfoModel Update(HttpRequestBase request)
        {
            IParameterContainer parameterContainer = new RequestParameterContainer(request);
            string token;
            parameterContainer.TryGet(TokenParameterName, out token, null);
            if (string.IsNullOrEmpty(token))
                throw new KeyNotFoundException("The key is to generate not found.");
            ICaptchaValue value;
            lock (SessionLocker)
            {
                if (!ValidateKeys.TryGetValue(token, out value))
                    throw new ArgumentException("The key is to update incorrect.");
                DrawingKeys.Remove(token);
                ValidateKeys.Remove(token);
            }
            KeyValuePair<string, ICaptchaValue> encryptValue = CreateCaptchaPair(parameterContainer, value);
            string newUrl = new UrlHelper(request.RequestContext).Action("Generate", "DefaultCaptcha",
                                                                         new {t = encryptValue.Key});
            lock (SessionLocker)
            {
                DrawingKeys.Add(encryptValue);
                ValidateKeys.Add(encryptValue);
            }
            return new DefaultUpdateInfoModel(TokenElementName, encryptValue.Key, newUrl, ImageElementName);
        }

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        public virtual bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer)
        {
            string tokenValue = controller.ValueProvider.GetValue(TokenElementName).AttemptedValue;
            string inputText = controller.ValueProvider.GetValue(InputElementName).AttemptedValue;
            ICaptchaValue value;
            lock (SessionLocker)
            {
                DrawingKeys.Remove(tokenValue);
                if (!ValidateKeys.TryGetValue(tokenValue, out value))
                {
                    WriteError(controller, parameterContainer);
                    return false;
                }
                ValidateKeys.Remove(tokenValue);
            }
            if (string.IsNullOrEmpty(inputText))
            {
                WriteError(controller, parameterContainer);
                return false;
            }
            bool isVerify = value.IsEqual(inputText);
            if (isVerify)
                return true;
            WriteError(controller, parameterContainer);
            return false;
        }

        #endregion
    }
}