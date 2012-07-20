using System;
using System.Web.Mvc;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model with information for create a captcha.
    /// </summary>
    public abstract class BaseBuildInfoModel : IBuildInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBuildInfoModel"/> class.
        /// </summary>
        protected BaseBuildInfoModel(string tokenParameterName, string requiredMessage, bool isRequired,
                                     string refreshButtonText, string inputText, HtmlHelper htmlHelper,
                                     string inputElementId, string imageElementId, string tokenElementId,
                                     string refreshUrl,
                                     string imageUrl, string tokenValue)
        {
            if (tokenParameterName == null) throw new ArgumentNullException("tokenParameterName");
            if (inputElementId == null) throw new ArgumentNullException("inputElementId");
            if (imageElementId == null) throw new ArgumentNullException("imageElementId");
            if (tokenElementId == null) throw new ArgumentNullException("tokenElementId");
            if (tokenValue == null) throw new ArgumentNullException("tokenValue");
            TokenParameterName = tokenParameterName;
            RequiredMessage = requiredMessage;
            IsRequired = isRequired;
            RefreshButtonText = refreshButtonText;
            InputText = inputText;
            HtmlHelper = htmlHelper;
            InputElementId = inputElementId;
            ImageElementId = imageElementId;
            TokenElementId = tokenElementId;
            RefreshUrl = refreshUrl;
            ImageUrl = imageUrl;
            TokenValue = tokenValue;
        }

        #endregion

        #region Implementation of IBuildInfoModel

        /// <summary>
        /// Gets the token parameter name.
        /// </summary>
        public string TokenParameterName { get; private set; }

        /// <summary>
        /// Gets the required field message.
        /// </summary>
        public string RequiredMessage { get; private set; }

        /// <summary>
        /// Gets the is required flag.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Gets the refresh button text.
        /// </summary>
        public string RefreshButtonText { get; private set; }

        /// <summary>
        /// Gets the input text.
        /// </summary>
        public string InputText { get; private set; }

        /// <summary>
        /// Gets the specified <see cref="HtmlHelper"/>.
        /// </summary>
        public HtmlHelper HtmlHelper { get; private set; }

        /// <summary>
        /// Gets the input element id in DOM.
        /// </summary>
        public string InputElementId { get; private set; }

        /// <summary>
        /// Gets the token element id in DOM.
        /// </summary>
        public string TokenElementId { get; private set; }

        /// <summary>
        /// Gets the image element id in DOM.
        /// </summary>
        public string ImageElementId { get; private set; }

        /// <summary>
        /// Gets the image url.
        /// </summary>
        public string ImageUrl { get; private set; }

        /// <summary>
        /// Gets the refresh url.
        /// </summary>
        public string RefreshUrl { get; private set; }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        public string TokenValue { get; private set; }

        #endregion
    }
}