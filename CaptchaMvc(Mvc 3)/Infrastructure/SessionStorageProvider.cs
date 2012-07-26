using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Represents the storage to save a captcha tokens in session.
    /// </summary>
    public class SessionStorageProvider : IStorageProvider
    {
        #region Fields

        private const string SessionValidateKey = "____________SessionValidateKey_____________";
        private const string SessionDrawingKey = "____________SessionDrawingKey_____________";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionStorageProvider"/> class.
        /// </summary>
        public SessionStorageProvider() : this(10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionStorageProvider"/> class.
        /// </summary>
        /// <param name="maxCount">Gets or sets the maximum values.</param>
        public SessionStorageProvider(int maxCount)
        {
            MaxCount = maxCount;
        }

        #endregion

        #region IStorageProvider Members

        /// <summary>
        /// Adds the specified token and <see cref="ICaptchaValue"/> to the storage.
        /// </summary>
        /// <param name="captchaPair">The specified <see cref="KeyValuePair{TKey,TValue}"/></param>
        public virtual void Add(KeyValuePair<string, ICaptchaValue> captchaPair)
        {
            if (captchaPair.Value == null)
                throw new ArgumentNullException("captchaPair");
            ClearIfNeed(DrawingKeys);
            ClearIfNeed(ValidateKeys);
            DrawingKeys.Add(captchaPair);
            ValidateKeys.Add(captchaPair);
        }

        /// <summary>
        /// Gets the <see cref="ICaptchaValue"/> associated with the specified token.
        /// </summary>
        /// <param name="token">The token of the value to get.</param>
        /// <returns>When this method returns, contains the value associated with the specified token, if the token is found; otherwise, return <c>null</c> value.</returns>
        public virtual ICaptchaValue GetDrawingValue(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            ICaptchaValue value;
            if (DrawingKeys.TryGetValue(token, out value))
                DrawingKeys.Remove(token);
            return value;
        }

        /// <summary>
        /// Gets the <see cref="ICaptchaValue"/> associated with the specified token.
        /// </summary>
        /// <param name="token">The token of the value to get.</param>
        /// <returns>When this method returns, contains the value associated with the specified token, if the token is found; otherwise, return <c>null</c> value.</returns>
        public virtual ICaptchaValue GetValidationValue(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            ICaptchaValue value;
            if (ValidateKeys.TryGetValue(token, out value))
                ValidateKeys.Remove(token);
            return value;
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the maximum values.
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// Contains tokens that have not yet been validated.
        /// </summary>
        protected IDictionary<string, ICaptchaValue> ValidateKeys
        {
            get { return GetDictionaryFromSession(SessionValidateKey); }
        }

        /// <summary>
        /// Contains tokens that have not yet been displayed.
        /// </summary>
        protected IDictionary<string, ICaptchaValue> DrawingKeys
        {
            get { return GetDictionaryFromSession(SessionDrawingKey); }
        }

        #endregion

        #region Method

        private void ClearIfNeed(IDictionary<string, ICaptchaValue> dictionary)
        {
            //Remove the two value from a session.
            if (dictionary.Count < MaxCount) return;
            dictionary.Remove(dictionary.Keys.First());
            if (dictionary.Count == 0) return;
            dictionary.Remove(dictionary.Keys.First());
        }

        private static IDictionary<string, ICaptchaValue> GetDictionaryFromSession(string key)
        {
            var dictionary = HttpContext.Current.Session[key] as IDictionary<string, ICaptchaValue>;
            if (dictionary == null)
            {
                dictionary = new ConcurrentDictionary<string, ICaptchaValue>();
                HttpContext.Current.Session[key] = dictionary;
            }
            return dictionary;
        }

        #endregion
    }
}