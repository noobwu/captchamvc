using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CaptchaMVC.Models;
using System.Web.Mvc.Ajax;

namespace CaptchaMVC.HtmlHelpers
{
    public static class CaptchaHelper
    {

        #region Field

        private const string CaptchaFormat = @"
<br/>
<img id=""CaptchaImage"" src=""{0}""/>
{1}
<br/>
";
        private const string DefaultName = "default";

        private static string _nameGenerateImage;
        private static string _nameEncryption;

        private static IEncryption _encryption;
        private static IGenerateImage _generateImage;

        #endregion

        #region Create and update captcha

        /// <summary>
        /// Helper method to create captcha.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="textRefreshButton"></param>
        /// <param name="inputText"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper, string textRefreshButton, string inputText, int length)
        {
            return GenerateFullCaptcha(htmlHelper, textRefreshButton, inputText, length);
        }

        /// <summary>
        /// Helper method to create captcha.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static MvcHtmlString Captcha(this HtmlHelper htmlHelper, int length)
        {
            return GenerateFullCaptcha(htmlHelper, length);
        }

        /// <summary>
        /// Create full captcha
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static MvcHtmlString GenerateFullCaptcha(HtmlHelper htmlHelper, int length)
        {
            return GenerateFullCaptcha(htmlHelper, "Refresh", "Input symbols:", length);
        }


        /// <summary>
        /// Create full captcha
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="text"></param>
        /// <param name="inputText"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static MvcHtmlString GenerateFullCaptcha(HtmlHelper htmlHelper, string text, string inputText, int length)
        {
            var encryptorModel = GetEncryptorModel();
            var captchaText = RandomText.Generate(length);
            var encryptText = GetEncryption().Encrypt(captchaText, encryptorModel.Password, encryptorModel.Salt);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var url = urlHelper.Action("Create", "CaptchaImage", new { encryptText });
            var ajax = new AjaxHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);
            var refresh = ajax.ActionLink(text, "NewCaptcha", "CaptchaImage", new { l = length },
                                          new AjaxOptions { UpdateTargetId = "CaptchaDeText", OnSuccess = "Success" });

            return MvcHtmlString.Create(string.Format(CaptchaFormat, url, htmlHelper.Hidden("CaptchaDeText", encryptText)) +
                                         refresh.ToHtmlString() + " <br/>" + inputText + "<br/>" +
                                        htmlHelper.TextBox("CaptchaInputText") + "<br/>" + htmlHelper.ValidationMessage("CaptchaInputText"));
        }

        /// <summary>
        /// Create partial captcha
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static RefreshModel GeneratePartialCaptcha(RequestContext requestContext, int length)
        {
            var encryptorModel = GetEncryptorModel();
            var captchaText = RandomText.Generate(length);
            var encryptText = GetEncryption().Encrypt(captchaText, encryptorModel.Password, encryptorModel.Salt);
            var urlHelper = new UrlHelper(requestContext);
            var url = urlHelper.Action("Create", "CaptchaImage", new { encryptText });

            return new RefreshModel() { Code = encryptText, Image = url };

        }

        #endregion

        #region Verify captcha

        /// <summary>
        /// Check for proper input captcha
        /// </summary>
        /// <param name="controllerBase"></param>
        /// <param name="textError">text for error</param>
        /// <returns></returns>
        public static bool IsCaptchaVerify(this ControllerBase controllerBase, string textError)
        {
            try
            {
                var captchaModel = new CaptchaModel
                {
                    CaptchaDeText =
                        controllerBase.ValueProvider.GetValue("CaptchaDeText").AttemptedValue,
                    CaptchaInputText =
                        controllerBase.ValueProvider.GetValue("CaptchaInputText").AttemptedValue
                };

                controllerBase.ViewData.ModelState.Remove("CaptchaDeText");
                controllerBase.ViewData.ModelState.Remove("CaptchaInputText");
                var isVerify = IsVerify(captchaModel);
                if (!isVerify)
                    controllerBase.ViewData.ModelState.AddModelError("CaptchaInputText", textError);

                return isVerify;
            }
            catch (Exception)
            {

                throw new NullReferenceException("Form not contain CaptchaModel");
            }
            
        }

        /// <summary>
        /// Check for proper input captcha
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Obsolete("Not correct work. Use attribute CaptchaVerifyAttribute")]
        public static bool Verify(CaptchaModel model)
        {
            return IsVerify(model);
        }

        /// <summary>
        /// Help method for verify
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        internal static bool IsVerify(CaptchaModel captcha)
        {
            try
            {
                var encryptorModel = GetEncryptorModel();
                if (encryptorModel == null)
                    return false;

                var textDecrypt = GetEncryption().Decrypt(captcha.CaptchaDeText, encryptorModel.Password, encryptorModel.Salt);
                return textDecrypt == captcha.CaptchaInputText;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Helper method for captcha

        /// <summary>
        /// Returns the model for decoding from the web.config
        /// </summary>
        /// <returns></returns>
        internal static EncryptorModel GetEncryptorModel()
        {
            var pass = ConfigurationManager.AppSettings["CaptchaPass"];
            var salt = ConfigurationManager.AppSettings["CaptchaSalt"];
            if ((string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(salt)))
                throw new ConfigurationErrorsException("In the web.config file, there are no options for Captcha.");
            try
            {
                var encryptorModel = new EncryptorModel() { Password = pass, Salt = Convert.FromBase64String(salt) };
                return encryptorModel;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Returns the implementation IGenerateImage custom or default.
        /// </summary>
        /// <returns></returns>
        internal static IGenerateImage GetGenerateImage()
        {
            var nameType = ConfigurationManager.AppSettings["CaptchaIGenerate"];
            
            if (!string.IsNullOrEmpty(nameType))
            {
                if (nameType == _nameGenerateImage)
                    return _generateImage;

                var type = GetType("IGenerateImage", nameType);
                if (type != null)
                {
                    var result = (IGenerateImage)type.Assembly.CreateInstance(type.FullName, true);
                    _generateImage = result;
                    _nameGenerateImage = nameType;

                    return _generateImage;
                }
            }

            if (DefaultName == _nameGenerateImage)
                return _generateImage;

            _nameGenerateImage = DefaultName;
            _generateImage = new GenerateImage();
            return _generateImage;
        }

        /// <summary>
        /// Returns the implementation IEncryption custom or default.
        /// </summary>
        /// <returns></returns>
        internal static IEncryption GetEncryption()
        {
            var nameType = ConfigurationManager.AppSettings["CaptchaIEncryption"];

            if (!string.IsNullOrEmpty(nameType))
            {
                if (nameType == _nameEncryption)
                    return _encryption;

                var type = GetType("IEncryption", nameType);
                if (type != null)
                {
                    var result = (IEncryption)type.Assembly.CreateInstance(type.FullName, true);
                    _encryption = result;
                    _nameEncryption = nameType;

                    return _encryption;
                }
            }
            if (DefaultName == _nameEncryption)
                return _encryption;

            _nameEncryption = DefaultName;
            _encryption = new Encryption();
            return _encryption;
        }

        /// <summary>
        /// Search type from Assemblies
        /// </summary>
        /// <param name="nameInterface"></param>
        /// <param name="nameType"></param>
        /// <returns></returns>
        private static Type GetType(string nameInterface, string nameType)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type typeImage = null;
            foreach (var assembly in allAssemblies.Where(assembl => !assembl.FullName.Contains("System")))
            {
                typeImage = (from type in assembly.GetTypes()
                             where type.IsClass &&
                                   (type.GetInterface(nameInterface) != null) && type.FullName == nameType
                             select type).FirstOrDefault();

                if (typeImage != null)
                    break;
            }

            return typeImage;
        }

        #endregion
    }
}
