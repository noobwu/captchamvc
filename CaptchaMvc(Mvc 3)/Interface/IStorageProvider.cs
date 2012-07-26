using System.Collections.Generic;

namespace CaptchaMvc.Interface
{
    /// <summary>
    /// Represents the storage to save a captcha tokens.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Adds the specified token and <see cref="ICaptchaValue"/> to the storage.
        /// </summary>
        /// <param name="captchaPair">The specified <see cref="KeyValuePair{TKey,TValue}"/></param>
        void Add(KeyValuePair<string, ICaptchaValue> captchaPair);

        /// <summary>
        /// Gets the <see cref="ICaptchaValue"/> associated with the specified token.
        /// </summary>
        /// <param name="token">The token of the value to get.</param>
        /// <returns>When this method returns, contains the value associated with the specified token, if the token is found; otherwise, return <c>null</c> value.</returns>
        ICaptchaValue GetDrawingValue(string token);

        /// <summary>
        /// Gets the <see cref="ICaptchaValue"/> associated with the specified token.
        /// </summary>
        /// <param name="token">The token of the value to get.</param>
        /// <returns>When this method returns, contains the value associated with the specified token, if the token is found; otherwise, return <c>null</c> value.</returns>
        ICaptchaValue GetValidationValue(string token);
    }
}