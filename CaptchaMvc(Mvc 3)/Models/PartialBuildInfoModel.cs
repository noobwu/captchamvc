using System;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the default model with information for create a partial captcha.
    /// </summary>
    public class PartialBuildInfoModel : IBuildInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialBuildInfoModel"/> class.
        /// </summary>
        public PartialBuildInfoModel(HtmlHelper htmlHelper, IBuildInfoModel buildInfoModel, string partialViewName, string scriptPartialViewName,
                                     ViewDataDictionary viewData)
        {
            Validate.ArgumentNotNull(htmlHelper, "htmlHelper");
            Validate.ArgumentNotNull(buildInfoModel, "buildInfoModel");
            Validate.ArgumentNotNullOrEmpty(partialViewName, "partialViewName");
            HtmlHelper = htmlHelper;
            BuildInfoModel = buildInfoModel;
            PartialViewName = partialViewName;
            ScriptPartialViewName = scriptPartialViewName;
            ViewData = viewData;
        }

        #endregion

        #region Implementation of IBuildInfoModel

        /// <summary>
        /// Gets the specified <see cref="IBuildInfoModel"/> for create captcha.
        /// </summary>
        public IBuildInfoModel BuildInfoModel { get; set; }

        /// <summary>
        /// Gets the specified partial view name.
        /// </summary>
        public string PartialViewName { get; set; }

        /// <summary>
        /// Gets the specified script-partial view name, if any.
        /// </summary>
        public string ScriptPartialViewName { get; set; }

        /// <summary>
        /// Gets the specified <see cref="ViewDataDictionary"/>.
        /// </summary>
        public ViewDataDictionary ViewData { get; set; }

        /// <summary>
        ///     Gets the parameter container.
        /// </summary>
        public IParameterContainer ParameterContainer
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the token parameter name.
        /// </summary>
        public string TokenParameterName
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the required field message.
        /// </summary>
        public string RequiredMessage
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the is required flag.
        /// </summary>
        public bool IsRequired
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the refresh button text.
        /// </summary>
        public string RefreshButtonText
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the input text.
        /// </summary>
        public string InputText
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the specified <see cref="IBuildInfoModel.HtmlHelper"/>.
        /// </summary>
        public HtmlHelper HtmlHelper { get; private set; }

        /// <summary>
        /// Gets the input element id in DOM.
        /// </summary>
        public string InputElementId
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the token element id in DOM.
        /// </summary>
        public string TokenElementId
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the image element id in DOM.
        /// </summary>
        public string ImageElementId
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the image url.
        /// </summary>
        public string ImageUrl
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the refresh url.
        /// </summary>
        public string RefreshUrl
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        public string TokenValue
        {
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}