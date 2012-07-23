using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Implementation of a <see cref="ICaptchaBulder"/> for build partial captcha.
    /// </summary>
    public class PartialCaptchaBuilder : ICaptchaBulder
    {
        #region Implementation of ICaptchaBulder

        /// <summary>
        /// Create a new captcha to the specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the captcha.</returns>
        public MvcHtmlString Build(IBuildInfoModel buildInfoModel)
        {
            var infoModel = buildInfoModel as PartialBuildInfoModel;
            if (infoModel == null)
                throw new ArgumentException("A PartialCaptchaBuilder can only work with the PartialBuildInfoModel.");
            if (infoModel.ViewData != null)
                return infoModel.HtmlHelper.Partial(infoModel.PartialViewName, infoModel.BuildInfoModel,
                                                    infoModel.ViewData);
            return infoModel.HtmlHelper.Partial(infoModel.PartialViewName, infoModel.BuildInfoModel);
        }

        #endregion
    }
}