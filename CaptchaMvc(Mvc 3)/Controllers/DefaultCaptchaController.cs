using System;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Controllers
{
    /// <summary>
    /// The controller is responsible for creating and updating captcha.
    /// </summary>
    public class DefaultCaptchaController : Controller
    {
        /// <summary>
        /// Generate a new captcha image.
        /// </summary>
        public virtual void Generate()
        {
            try
            {
                if (Request.UrlReferrer.AbsolutePath == Request.Url.AbsolutePath)
                    throw new Exception();
                IDrawingModel drawingModel = CaptchaUtils.CaptchaManager.GetDrawingModel(Request);
                CaptchaUtils.BuilderProvider.WriteCaptchaImage(Response, drawingModel);
            }
            catch (Exception)
            {
                CaptchaUtils.BuilderProvider.WriteErrorImage(Response);
            }
        }

        /// <summary>
        /// Reresh a captcha.
        /// </summary>
        /// <returns>The specified <see cref="ActionResult"/>.</returns>
        public virtual ActionResult Refresh()
        {
            if (Request.IsAjaxRequest())
            {
                IUpdateInfoModel infoModel = CaptchaUtils.CaptchaManager.Update(Request);
                return CaptchaUtils.BuilderProvider.RefreshCaptcha(infoModel);
            }

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
    }
}