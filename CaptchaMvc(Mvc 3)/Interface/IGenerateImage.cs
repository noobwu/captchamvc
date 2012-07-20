using System.Drawing;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Interface for implementing captcha image.
    /// </summary>
    public interface IGenerateImage
    {
        /// <summary>
        ///  Create a captcha image.
        /// </summary>
        /// <param name="captchaText">The specified text for image.</param>
        /// <returns>The captcha image.</returns>
        Bitmap Generate(string captchaText);
    }
}