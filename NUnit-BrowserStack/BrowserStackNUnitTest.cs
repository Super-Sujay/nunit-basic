using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace BrowserStack
{
    [TestFixture]
    public class BrowserStackNUnitTest
    {
        protected IWebDriver driver;
        protected string profile;
        protected string environment;
        private Local browserStackLocal;

        public BrowserStackNUnitTest(string profile, string environment)
        {
            this.profile = profile;
            this.environment = environment;
        }

        [SetUp]
        public void Init()
        {
            NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
            NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

            DriverOptions options;

            String username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
            if (username == null) username = ConfigurationManager.AppSettings.Get("user");

            String accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
            if (accesskey == null) accesskey = ConfigurationManager.AppSettings.Get("key");

            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            foreach (string key in caps.AllKeys) browserstackOptions.Add(key, caps[key]);
            foreach (string key in settings.AllKeys) browserstackOptions.Add(key, settings[key]);
            browserstackOptions.Add("seleniumVersion", "3.141.0");
            browserstackOptions.Add("userName", username);
            browserstackOptions.Add("accessKey", accesskey);

            foreach (var pair in browserstackOptions) Console.WriteLine($"{pair.Key}: {pair.Value}");

            switch (environment)
            {
                case "chrome":
                    options = new EdgeOptions();
                    options.AddAdditionalCapability("browserName", "Chrome");
                    options.BrowserVersion = "latest";
                    options.AddAdditionalCapability("bstack:options", browserstackOptions);
                    break;
                case "firefox":
                    options = new EdgeOptions();
                    options.AddAdditionalCapability("browserName", "Firefox");
                    options.BrowserVersion = "latest";
                    options.AddAdditionalCapability("bstack:options", browserstackOptions);
                    break;
                case "safari":
                    options = new SafariOptions();
                    options.BrowserVersion = "14.1";
                    options.AddAdditionalCapability("bstack:options", browserstackOptions);
                    break;
                case "edge":
                    options = new EdgeOptions();
                    options.BrowserVersion = "latest";
                    options.AddAdditionalCapability("bstack:options", browserstackOptions);
                    break;
                default:
                    throw new Exception("Incorrect browser specified");
            }

            if (browserstackOptions.ContainsValue("local").ToString().Equals("true"))
            {
                browserStackLocal = new Local();
                List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>();
                bsLocalArgs.Add(new KeyValuePair<string, string>("key", accesskey));
                bsLocalArgs.Add(new KeyValuePair<string, string>("forcelocal", "true"));
                //bsLocalArgs.Add(new KeyValuePair<string, string>("v", "true"));
                //bsLocalArgs.Add(new KeyValuePair<string, string>("logfile", ""));
                //dotnet test --filter FullyQualifiedName~BrowserStack.LocalTest
                browserStackLocal.start(bsLocalArgs);
            }

            RemoteSessionSettings remoteSettings = new RemoteSessionSettings(null, options);
            driver = new RemoteWebDriver(new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/"), remoteSettings);
        }

        [TearDown]
        public void Cleanup()
        {
            driver.Quit();
            if (browserStackLocal != null) browserStackLocal.stop();
        }
    }
}
