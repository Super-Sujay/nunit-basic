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
using System.Threading;

namespace BrowserStack
{
    [TestFixture]
    public class BrowserStackNUnitTest
    {
        private static readonly ThreadLocal<RemoteWebDriver> driverThread = new ThreadLocal<RemoteWebDriver>();
        protected string profile, environment;
        private Local browserStackLocal;

        public BrowserStackNUnitTest(string profile, string environment)
        {
            this.profile = profile;
            this.environment = environment;
        }

        [SetUp]
        public void Init()
        {
            RemoteWebDriver driver;
            Uri uri = new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/");

            NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
            NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

            String username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
            if (username == null) username = ConfigurationManager.AppSettings.Get("user");

            String accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
            if (accesskey == null) accesskey = ConfigurationManager.AppSettings.Get("key");

            if (caps["browserstack.local"] != null && caps["browserstack.local"].Equals("true"))
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

            switch (environment)
            {
                case "chrome":
                    ChromeOptions chromeOptions = new ChromeOptions();
                    foreach (string key in caps.AllKeys) chromeOptions.AddAdditionalCapability(key, caps[key], true);
                    foreach (string key in settings.AllKeys) chromeOptions.AddAdditionalCapability(key, settings[key], true);
                    chromeOptions.AddAdditionalCapability("browserstack.user", username, true);
                    chromeOptions.AddAdditionalCapability("browserstack.key", accesskey, true);
                    driver = new RemoteWebDriver(uri, chromeOptions);
                    break;
                case "firefox":
                    FirefoxOptions firefoxOptions = new FirefoxOptions();
                    foreach (string key in caps.AllKeys) firefoxOptions.AddAdditionalCapability(key, caps[key], true);
                    foreach (string key in settings.AllKeys) firefoxOptions.AddAdditionalCapability(key, settings[key], true);
                    firefoxOptions.AddAdditionalCapability("browserstack.user", username, true);
                    firefoxOptions.AddAdditionalCapability("browserstack.key", accesskey, true);
                    driver = new RemoteWebDriver(uri, firefoxOptions);
                    break;
                case "safari":
                    SafariOptions safariOptions = new SafariOptions();
                    foreach (string key in caps.AllKeys) safariOptions.AddAdditionalCapability(key, caps[key]);
                    foreach (string key in settings.AllKeys) safariOptions.AddAdditionalCapability(key, settings[key]);
                    safariOptions.AddAdditionalCapability("browserstack.user", username);
                    safariOptions.AddAdditionalCapability("browserstack.key", accesskey);
                    driver = new RemoteWebDriver(uri, safariOptions);
                    break;
                case "edge":
                    EdgeOptions edgeOptions = new EdgeOptions();
                    foreach (string key in caps.AllKeys) edgeOptions.AddAdditionalCapability(key, caps[key]);
                    foreach (string key in settings.AllKeys) edgeOptions.AddAdditionalCapability(key, settings[key]);
                    edgeOptions.AddAdditionalCapability("browserstack.user", username);
                    edgeOptions.AddAdditionalCapability("browserstack.key", accesskey);
                    driver = new RemoteWebDriver(uri, edgeOptions);
                    break;
                case "ios":
                    SafariOptions iosOptions = new SafariOptions();
                    foreach (string key in caps.AllKeys) iosOptions.AddAdditionalCapability(key, caps[key]);
                    foreach (string key in settings.AllKeys) iosOptions.AddAdditionalCapability(key, settings[key]);
                    iosOptions.AddAdditionalCapability("browserstack.user", username);
                    iosOptions.AddAdditionalCapability("browserstack.key", accesskey);
                    driver = new RemoteWebDriver(uri, iosOptions);
                    break;
                case "android":
                    ChromeOptions androidOptions = new ChromeOptions();
                    foreach (string key in caps.AllKeys) androidOptions.AddAdditionalCapability(key, caps[key], true);
                    foreach (string key in settings.AllKeys) androidOptions.AddAdditionalCapability(key, settings[key], true);
                    androidOptions.AddAdditionalCapability("browserstack.user", username, true);
                    androidOptions.AddAdditionalCapability("browserstack.key", accesskey, true);
                    driver = new RemoteWebDriver(uri, androidOptions);
                    break;
                default:
                    throw new Exception("Incorrect browser specified");
            }
            driverThread.Value = driver;
        }

        [TearDown]
        public void Cleanup()
        {
            driverThread.Value.ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \"Test Passed\"}}");
            driverThread.Value.Quit();
            if (browserStackLocal != null) browserStackLocal.stop();
        }

        public static IWebDriver GetWebDriver()
        {
            return driverThread.Value;
        }
    }
}
