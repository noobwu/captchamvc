﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    /// Provides methods to work with the captcha.
    /// </summary>
    public static class CaptchaUtils
    {
        #region Fields

        private static readonly IGenerateImage DefaultGenerateImage = new DefaultGenerateImage();
        private static readonly ICaptchaBuilderProvider DefaultBuilderProvider = new DefaultCaptchaBuilderProvider();
        private static readonly ICaptchaManager DefaultEncryptionProvider = new DefaultCaptchaManager();

        #endregion

        #region Property

        /// <summary>
        /// Gets the current <see cref="ICaptchaBuilderProvider"/>.
        /// </summary>
        public static ICaptchaBuilderProvider BuilderProvider
        {
            get { return GetService("DefaultCaptchaBuilderProvider", () => DefaultBuilderProvider); }
        }

        /// <summary>
        /// Gets the current <see cref="ICaptchaManager"/>.
        /// </summary>
        public static ICaptchaManager CaptchaManager
        {
            get { return GetService("DefaultCaptchaManager", () => DefaultEncryptionProvider); }
        }

        /// <summary>
        /// Gets the current <see cref="IGenerateImage"/>.
        /// </summary>
        public static IGenerateImage ImageGenerator
        {
            get { return GetService("CaptchaIGenerate", () => DefaultGenerateImage); }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create a new captcha with the specified arguments.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameters">The specified attributes.</param>
        /// <returns>The html string with a captcha.</returns>
        public static MvcHtmlString GenerateCaptcha(HtmlHelper htmlHelper, params ParameterModel[] parameters)
        {
            IBuildInfoModel buildInfoModel = CaptchaManager.GenerateNew(htmlHelper, new ParameterModelContainer(parameters));
            return BuilderProvider.GenerateCaptcha(buildInfoModel);
        }

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="attributes">The specified attributes.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        public static bool ValidateCaptcha(ControllerBase controller, params ParameterModel[] attributes)
        {
            return CaptchaManager.ValidateCaptcha(controller, new ParameterModelContainer(attributes));
        }

        #endregion

        #region Internal methods

        internal static T GetService<T>(string configName, Func<T> defaultValue) where T : class
        {
            string nameType = ConfigurationManager.AppSettings[configName];
            if (!string.IsNullOrEmpty(nameType))
            {
                Type type = Type.GetType(nameType, false, true);
                if (type == null)
                    throw new TypeLoadException(
                        string.Format(
                            "When load the {1}. Type the name of the {0} can not be found in assemblies.",
                            nameType, configName));
                return (T) type.Assembly.CreateInstance(type.FullName, true);
            }
            T service;
            if (!TryGetService(out service))
                service = defaultValue();
            return service;
        }

        internal static bool TryGetService<T>(out T result) where T : class
        {
            result = default(T);
            if (DependencyResolver.Current == null) return false;
            try
            {
                result = DependencyResolver.Current.GetService<T>();
            }
            catch
            {
                return false;
            }
            return result != null;
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static bool TryFindParameter<T>(this IEnumerable<ParameterModel> parameters, string name, out T result,
                                                 T defaultValue)
        {
            result = defaultValue;
            if (!IsContain(parameters, name))
                return false;
            result = FindParameter<T>(parameters, name);
            return true;
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static bool TryFindParameter<T>(this IEnumerable<ParameterModel> parameters, string name, out T result)
        {
            return TryFindParameter(parameters, name, out result, default(T));
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static T FindParameter<T>(this IEnumerable<ParameterModel> parameters, string name)
        {
            ParameterModel parameter = parameters.FirstOrDefault(model => model.Name.Equals(name));
            if (parameter == null)
                return default(T);
            return (T) parameter.Value;
        }

        /// <summary>
        /// Determines whether the collection of parameters contains a specific value.
        /// </summary>
        /// <param name="parameters">The specified collection of parameters.</param>
        /// <param name="name">The parameter name for search.</param>
        /// <returns><c>True</c> if the parameter is found in the collection; otherwise, <c>false</c>.</returns>
        internal static bool IsContain(this IEnumerable<ParameterModel> parameters, string name)
        {
            return parameters.Any(model => model.Name.Equals(name));
        }

        #endregion
    }
}