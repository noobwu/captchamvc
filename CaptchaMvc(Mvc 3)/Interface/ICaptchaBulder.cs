using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the base model to create a new captcha.
    /// </summary>
    public interface ICaptchaBulder
    {
        /// <summary>
        /// Create a new captcha to the specified <see cref="IBuildInfoModel"/>.
        /// </summary>
        /// <param name="buildInfoModel">The specified <see cref="IBuildInfoModel"/>.</param>
        /// <returns>The html string with the captcha.</returns>
        MvcHtmlString Build(IBuildInfoModel buildInfoModel);
    }
}