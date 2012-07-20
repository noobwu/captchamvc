using System.Web.Mvc;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the default model with information for create a captcha.
    /// </summary>
    public class DefaultBuildInfoModel : BaseBuildInfoModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBuildInfoModel"/> class.
        /// </summary>
        public DefaultBuildInfoModel(string tokenParameterName, string requiredMessage, bool isRequired,
                                     string refreshButtonText, string inputText, HtmlHelper htmlHelper,
                                     string inputElementId, string imageElementId, string tokenElementId,
                                     string refreshUrl, string imageUrl, string tokenValue)
            : base(
                tokenParameterName, requiredMessage, isRequired, refreshButtonText, inputText, htmlHelper,
                inputElementId, imageElementId, tokenElementId, refreshUrl, imageUrl, tokenValue)
        {
        }

        #endregion
    }
}