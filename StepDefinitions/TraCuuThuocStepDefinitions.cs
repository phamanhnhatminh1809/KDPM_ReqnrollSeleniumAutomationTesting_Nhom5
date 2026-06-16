using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks; 

namespace DoAn.StepDefinitions
{
    [Binding]
    public class TraCuuOpenFDAStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver driver => TestHooks.Driver;
        private WebDriverWait wait;

        public TraCuuOpenFDAStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"Mở trang chủ nhà thuốc và chuyển sang trang Tra Cứu Thuốc")]
        public void MoTrangChuVaChuyenSangTraCuu()
        {
            driver.Navigate().GoToUrl("http://localhost:44317/");
            var lnkTraCuu = wait.Until(d => d.FindElement(By.LinkText("Tra Cứu Thuốc")));
            lnkTraCuu.Click();
        }

        [When(@"Người dùng nhập từ khóa tra cứu là ""(.*)""")]
        public void NhapTuKhoaTraCuu(string tuKhoa)
        {
            var txtKeyword = wait.Until(d => { try { return d.FindElement(By.Name("keyword")); } catch (NoSuchElementException) { return d.FindElement(By.XPath("//input[@id='keyword' or @placeholder='Tra cứu']")); } });
            txtKeyword.Click();
            txtKeyword.Clear();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                txtKeyword.SendKeys(tuKhoa);
            }
        }

        [When(@"Nhấn nút Tra cứu thông tin")]
        public void NhanNutTraCuu()
        {
            try
            {
                var txtKeyword = driver.FindElement(By.Name("keyword"));
                txtKeyword.SendKeys(Keys.Enter);
            }
            catch (Exception)
            {
                var btnSearch = wait.Until(d => d.FindElement(By.CssSelector(".btn-primary, button[type='submit'], #btnSearch")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnSearch);
            }
        }

        [Then(@"Hệ thống phải phản hồi kết quả tra cứu mong đợi là ""(.*)"" và chứa từ khóa ""(.*)""")]
        public void KiemTraKetQuaTraCuu(string ketQuaMongDoi, string tuKhoaKiemTra)
        {
            System.Threading.Thread.Sleep(2000); 

            if (ketQuaMongDoi == "CO_KET_QUA")
            {
                string pageSourceLower = driver.PageSource.ToLower();
                bool hasDetailData = pageSourceLower.Contains(tuKhoaKiemTra.ToLower())
                                     || pageSourceLower.Contains("manufacturer")
                                     || pageSourceLower.Contains("brand_name");

                Assert.That(hasDetailData, Is.True, $"Lỗi: Không tìm thấy thông tin dược điển của '{tuKhoaKiemTra}' hiển thị trên màn hình.");
            }
            else if (ketQuaMongDoi == "KHONG_CO_DATA")
            {
                bool hasNoDataAlert = driver.PageSource.Contains(tuKhoaKiemTra);

                Assert.That(hasNoDataAlert, Is.True, $"Lỗi: Hệ thống không hiển thị đúng thông báo lỗi thực tế: '{tuKhoaKiemTra}'");
            }
            else if (ketQuaMongDoi == "DE_TRONG")
            {
                var txtKeyword = driver.FindElement(By.Name("keyword"));

                string validationMessage = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", txtKeyword);

                Assert.That(validationMessage, Is.Not.Null.And.Not.Empty, "Lỗi: Trình duyệt không kích hoạt chặn trống trường nhập liệu (Required attribute)!");
                Assert.That(validationMessage.Contains(tuKhoaKiemTra) || validationMessage.ToLower().Contains("fill out"), Is.True,
                    $"Lỗi: Thông báo chặn trống hiển thị sai. Mong đợi chứa: '{tuKhoaKiemTra}', Thực tế thu được: '{validationMessage}'");
            }
            else if (ketQuaMongDoi == "SECURITY_OK")
            {
                bool isScriptEncoded = !driver.PageSource.Contains("<script>alert");
                Assert.That(isScriptEncoded, Is.True, "Lỗi bảo mật: Hệ thống thực thi trực tiếp mã script (XSS Flaw) hoặc không mã hóa chuỗi dữ liệu!");
            }
        }
    }
}