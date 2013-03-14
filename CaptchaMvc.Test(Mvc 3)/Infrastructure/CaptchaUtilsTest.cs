using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using CaptchaMvc.Models;
using NUnit.Framework;

namespace CaptchaMvc.Test.Infrastructure
{
    public class FakeImageGeneratorGenerator : IImageGenerator
    {
        #region Implementation of IImageGenerator

        /// <summary>
        ///  Create a captcha image.
        /// </summary>
        /// <param name="captchaText">The specified text for image.</param>
        /// <returns>The captcha image.</returns>
        public Bitmap Generate(string captchaText)
        {
            return null;
        }

        #endregion
    }

    public class FakeCaptchaManager:ICaptchaManager
    {
        #region Implementation of ICaptchaManager

        /// <summary>
        /// Create a <see cref="IBuildInfoModel"/> for create a new captcha.
        /// </summary>
        /// <param name="htmlHelper">The specified <see cref="HtmlHelper"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns>An instance of <see cref="IBuildInfoModel"/>.</returns>
        public IBuildInfoModel GenerateNew(HtmlHelper htmlHelper, IParameterContainer parameterContainer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new <see cref="IDrawingModel"/> for drawing a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IDrawingModel"/>.</returns>
        public IDrawingModel GetDrawingModel(HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new <see cref="IBuildInfoModel"/> for update a captcha.
        /// </summary>
        /// <param name="request">The specified <see cref="HttpRequestBase"/>.</param>
        /// <returns>An instance of <see cref="IUpdateInfoModel"/>.</returns>
        public IUpdateInfoModel Update(HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the captcha is valid, and write error message if need.
        /// </summary>
        /// <param name="controller">The specified <see cref="ControllerBase"/>.</param>
        /// <param name="parameterContainer">The specified <see cref="IParameterContainer"/>.</param>
        /// <returns><c>True</c> if the captcha is valid; otherwise, <c>false</c>.</returns>
        public bool ValidateCaptcha(ControllerBase controller, IParameterContainer parameterContainer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class FakeResolver:IDependencyResolver
    {

        #region Implementation of IDependencyResolver

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        /// <param name="serviceType">The type of the requested service or object.</param>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ICaptchaManager))
                return new FakeCaptchaManager();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <returns>
        /// The requested services.
        /// </returns>
        /// <param name="serviceType">The type of the requested services.</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [TestFixture]
    public class CaptchaUtilsTest
    {
        [Test]
        public void TestDefaultValues()
        {
            Assert.IsNotNull(CaptchaUtils.BuilderProvider);
            Assert.IsNotNull(CaptchaUtils.CaptchaManager);
            Assert.IsInstanceOf(typeof (DefaultCaptchaManager), CaptchaUtils.CaptchaManager);
            Assert.IsInstanceOf(typeof (DefaultCaptchaBuilderProvider), CaptchaUtils.BuilderProvider);
        }

        [Test]
        public void TestLoadFromConfig()
        {
            Assert.IsNotNull(CaptchaUtils.ImageGeneratorGenerator);
            Assert.IsInstanceOf(typeof (FakeImageGeneratorGenerator), CaptchaUtils.ImageGeneratorGenerator);
        }

        [Test]
        public void TestResolver()
        {
            DependencyResolver.SetResolver(new FakeResolver());
            Assert.IsNotNull(CaptchaUtils.CaptchaManager);
            Assert.IsInstanceOf(typeof(FakeCaptchaManager), CaptchaUtils.CaptchaManager);
        }
    }
}