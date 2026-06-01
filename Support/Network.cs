using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V148.Network;
namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Support
{
    public static class Network
    {
        public static void SetNetworkSlow3G(ChromeDriver driver)
        {
            IDevTools devTools = driver as IDevTools;
            var session = devTools.GetDevToolsSession();

            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V148.DevToolsSessionDomains>();

            var slow3GSettings = new OpenQA.Selenium.DevTools.V148.Network.EmulateNetworkConditionsCommandSettings
            {
                Offline = false,
                Latency = 2000,           // Độ trễ 2s
                DownloadThroughput = 400000,
                UploadThroughput = 400000
            };

            domains.Network.EmulateNetworkConditions(slow3GSettings).Wait();
        }
        public static void ResetNetwork(ChromeDriver driver)
        {
            IDevTools devTools = driver as IDevTools;
            var session = devTools.GetDevToolsSession();
            var networkDomains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V148.DevToolsSessionDomains>();

            var resetDomain = new OpenQA.Selenium.DevTools.V148.Network.EmulateNetworkConditionsCommandSettings
            {
                Offline = false,
                Latency = 0,
                DownloadThroughput = -1, 
                UploadThroughput = -1
            };
            networkDomains.Network.EmulateNetworkConditions(resetDomain).Wait();
        }
    }
}
