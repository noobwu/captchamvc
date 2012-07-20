namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the base model for storing ​​captcha values.
    /// </summary>
    public interface ICaptchaValue
    {
        /// <summary>
        /// Gets the specified captcha text.
        /// </summary>
        string CaptchaText { get; }

        /// <summary>
        /// Gets the specified captcha value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Determines whether the current captcha value is equal for the input value.
        /// </summary>
        /// <param name="inputText">The specified input text.</param>
        /// <returns><c>True</c> if the value is equals; otherwise, <c>false</c>.</returns>
        bool IsEqual(string inputText);
    }
}