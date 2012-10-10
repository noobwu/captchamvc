using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Default implementation of a <see cref="ICaptchaBulder"/>.
    /// </summary>
    public class DefaultCaptchaBuilder : ICaptchaBulder
    {
        #region Fields

        private const string CaptchaFormat = @"
<br/>
<img id=""{0}"" src=""{1}""/>";

        private const string UpdateScript = @"
<script type=""text/javascript"">
$(function () {{$('#{0}').show();}});
function {4} {{ $('#{0}').hide(); $.post(""{1}"", {{ {2}: $('#{3}').val() }}, function(){{$('#{0}').show();}}); return false; }}</script>";

        #endregion

        #region Implementation of ICaptchaBulder

        /// <summary>
        /// Create a new captcha to the specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the captcha.</returns>
        public virtual MvcHtmlString Build(IBuildInfoModel buildInfoModel)
        {
            string captchaFormat = GenerateCaptchaImage(buildInfoModel);
            string generateTokenElement = GenerateTokenElement(buildInfoModel);
            string inputElement = GenerateInputElement(buildInfoModel);
            string refreshButton = GenerateRefreshButton(buildInfoModel);
            return
                MvcHtmlString.Create(string.Format("{0}{1} <br/>{2}<br/>{3}<br/>{4}", captchaFormat,
                                                   generateTokenElement, refreshButton,
                                                   buildInfoModel.InputText, inputElement));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a html string to represent the image.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the image.</returns>
        protected virtual string GenerateCaptchaImage(IBuildInfoModel buildInfoModel)
        {
            return string.Format(CaptchaFormat, buildInfoModel.ImageElementId, buildInfoModel.ImageUrl);
        }

        /// <summary>
        /// Create a html string to represent the token element.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the token element.</returns>
        protected virtual string GenerateTokenElement(IBuildInfoModel buildInfoModel)
        {
            return buildInfoModel.HtmlHelper.Hidden(buildInfoModel.TokenElementId,
                                                    buildInfoModel.TokenValue).ToHtmlString();
        }

        /// <summary>
        /// Create a html string to represent the input element.
        /// </summary>
        /// <param name="buildInfo">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the input element.</returns>
        protected virtual string GenerateInputElement(IBuildInfoModel buildInfo)
        {
            IDictionary<string, object> attributes = new Dictionary<string, object>();
            if (buildInfo.IsRequired)
            {
                attributes.Add(@"data-val", "true");
                attributes.Add("data-val-required", buildInfo.RequiredMessage);
            }
            attributes.Add("autocomplete", "off");
            attributes.Add("autocorrect", "off");
            MvcHtmlString input = buildInfo.HtmlHelper.TextBox(buildInfo.InputElementId, null, attributes);
            MvcHtmlString validationMessage = buildInfo.HtmlHelper.ValidationMessage(buildInfo.InputElementId);
            return string.Format("{0}<br/>{1}", input, validationMessage);
        }

        /// <summary>
        /// Create a html string to represent the refresh button element.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the refresh button element.</returns>
        protected virtual string GenerateRefreshButton(IBuildInfoModel buildInfoModel)
        {
            string id = Guid.NewGuid().ToString("N");
            string functionName = string.Format("______{0}________()", Guid.NewGuid().ToString("N"));
            var tagA = new TagBuilder("a");
            tagA.Attributes.Add("onclick", functionName);
            tagA.Attributes.Add("href", "#" + id);
            tagA.Attributes.Add("style", "display:none;");
            tagA.SetInnerText(buildInfoModel.RefreshButtonText);
            tagA.Attributes.Add("id", id);
            string updateScript = string.Format(UpdateScript, id, buildInfoModel.RefreshUrl, buildInfoModel.TokenParameterName,
                                                buildInfoModel.TokenElementId, functionName);
            return string.Format("{0} {1}", updateScript, tagA);
        }

        #endregion
    }
}