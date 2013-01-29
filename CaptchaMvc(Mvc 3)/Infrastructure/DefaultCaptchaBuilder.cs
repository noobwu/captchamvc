using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Default implementation of a <see cref="ICaptchaBulder" />.
    /// </summary>
    public class DefaultCaptchaBuilder : ICaptchaBulder
    {
        #region Nested types

        /// <summary>
        ///     Represents the model for refresh button.
        /// </summary>
        protected sealed class RefreshButton
        {
            #region Fields

            /// <summary>
            ///     Gets the markup.
            /// </summary>
            public readonly string Markup;

            /// <summary>
            ///     Gets the script.
            /// </summary>
            public readonly string Script;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            public RefreshButton(string markup, string script)
            {
                Validate.ArgumentNotNull(markup, "markup");
                Validate.ArgumentNotNull(script, "script");
                Markup = markup;
                Script = script;
            }

            #endregion
        }

        #endregion

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
        ///     Creates a new captcha using the specified <see cref="IBuildInfoModel" />.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>An instance of <see cref="ICaptcha"/>.</returns>
        public virtual ICaptcha Build(IBuildInfoModel buildInfoModel)
        {
            string captchaFormat = GenerateCaptchaImage(buildInfoModel);
            string generateTokenElement = GenerateTokenElement(buildInfoModel);
            string inputElement = GenerateInputElement(buildInfoModel);
            RefreshButton refreshButton = GenerateRefreshButton(buildInfoModel);
            string markup = string.Format("{0}{1} <br/>{2}<br/>{3}<br/>{4}", captchaFormat,
                                          generateTokenElement, refreshButton.Markup,
                                          buildInfoModel.InputText, inputElement);
            return new CaptchaModel(markup, refreshButton.Script);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a html string to represent the image.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>The html string with the image.</returns>
        protected virtual string GenerateCaptchaImage(IBuildInfoModel buildInfoModel)
        {
            return string.Format(CaptchaFormat, buildInfoModel.ImageElementId, buildInfoModel.ImageUrl);
        }

        /// <summary>
        ///     Creates a html string to represent the token element.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>The html string with the token element.</returns>
        protected virtual string GenerateTokenElement(IBuildInfoModel buildInfoModel)
        {
            return buildInfoModel.HtmlHelper.Hidden(buildInfoModel.TokenElementId,
                                                    buildInfoModel.TokenValue).ToHtmlString();
        }

        /// <summary>
        ///     Creates a html string to represent the input element.
        /// </summary>
        /// <param name="buildInfo">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
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

            var validationMessage = string.Empty;
            bool addSpan;
            if (buildInfo.ParameterContainer.TryGet(DefaultCaptchaManager.IsNeedValidationSpanAttribute, out addSpan) && addSpan)
                validationMessage = string.Format(
                     @"<span class=""field-validation-valid"" data-valmsg-for=""{0}"" data-valmsg-replace=""true""></span>",
                     buildInfo.InputElementId);

            return string.Format("{0}<br/>{1}", input, validationMessage);
        }

        /// <summary>
        ///     Creates a html string to represent the refresh button element.
        /// </summary>
        /// <param name="buildInfoModel">
        ///     The specified <see cref="IBuildInfoModel" />.
        /// </param>
        /// <returns>The html string with the refresh button element.</returns>
        protected virtual RefreshButton GenerateRefreshButton(IBuildInfoModel buildInfoModel)
        {
            string id = Guid.NewGuid().ToString("N");
            string functionName = string.Format("______{0}________()", Guid.NewGuid().ToString("N"));
            var tagA = new TagBuilder("a");
            tagA.Attributes.Add("onclick", functionName);
            tagA.Attributes.Add("href", "#" + buildInfoModel.InputElementId);
            tagA.Attributes.Add("style", "display:none;");
            tagA.SetInnerText(buildInfoModel.RefreshButtonText);
            tagA.Attributes.Add("id", id);
            string updateScript = string.Format(UpdateScript, id, buildInfoModel.RefreshUrl,
                                                buildInfoModel.TokenParameterName,
                                                buildInfoModel.TokenElementId, functionName);
            return new RefreshButton(tagA.ToString(), updateScript);
        }

        #endregion
    }
}