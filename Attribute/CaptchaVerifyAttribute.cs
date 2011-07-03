using System;
using System.Web.Mvc;
using CaptchaMVC.HtmlHelpers;
using CaptchaMVC.Models;

namespace CaptchaMVC.Attribute
{
    public class CaptchaVerifyAttribute :ActionFilterAttribute
    {
        private readonly string _textError;

        public CaptchaVerifyAttribute(string textError)
        {
            _textError = textError;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ControllerBase controllerBase = filterContext.Controller;
                var captchaModel = new CaptchaModel
                {
                    CaptchaDeText =
                        controllerBase.ValueProvider.GetValue("CaptchaDeText").AttemptedValue,
                    CaptchaInputText =
                        controllerBase.ValueProvider.GetValue("CaptchaInputText").AttemptedValue
                };
                if (!CaptchaHelper.IsVerify(captchaModel))
                    controllerBase.ViewData.ModelState.AddModelError("CaptchaInputText", _textError);
            }
            catch (Exception)
            {

                throw new NullReferenceException("Form not contain CaptchaModel");
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}
