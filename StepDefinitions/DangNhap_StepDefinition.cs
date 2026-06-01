using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Support;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.StepDefinitions
{
    [Binding]
    [Scope(Tag = "dang_nhap_feature")]
    public class DangNhap_StepDefinition
    {
        private static TestDatabaseCheckpoint _checkpoint = new TestDatabaseCheckpoint();
        private IWebDriver driver;
        private WebDriverWait wait;

        [BeforeScenario]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        [Given(@"người dùng truy cập vào trang web ""(.*)""")]
        public void GivenNgườiDungTruyCậpVaoTrangWeb(string link_web)
        {
            driver.Navigate().GoToUrl(link_web);
        }

        [When("người dùng nhấn vào nút Đăng nhập")]
        public void WhenNgườiDungNhấnVaoNutDangNhập()
        {
            IWebElement dangNhapButton = wait.Until(d => d.FindElement(By.CssSelector("a[href*='/TaiKhoan/DangNhap']")));
            dangNhapButton.Click();
        }

        [When(@"người dùng nhập thông tin đăng nhập bao gồm ""(.*)"" và ""(.*)""")]
        public void WhenNgườiDungNhậpThongTinDangNhậpBaoGồmVa(string tenDangNhap, string matKhau)
        {
            IWebElement tenDangNhapBox = wait.Until(d => d.FindElement(By.Id("TenDangNhap")));
            IWebElement matKhauBox = wait.Until(d => d.FindElement(By.Id("MatKhau")));

            if (tenDangNhap.Contains("[101_ky_tu]"))
            {
                for(int i = 0; i < 102; i++)
                {
                    tenDangNhap += "a";
                }
            }
            if (matKhau.Contains("[101_ky_tu]"))
            {
                for (int i = 0; i < 102; i++)
                {
                    matKhau += "a";
                }
            }
            
            tenDangNhap = tenDangNhap.Trim('"');
            matKhau = matKhau.Trim('"');

            tenDangNhapBox.Clear();
            tenDangNhapBox.SendKeys(tenDangNhap);

            matKhauBox.Clear();
            matKhauBox.SendKeys(matKhau);


        }

        [When("người dùng nhấn nút Đăng nhập trên giao diện đăng nhập")]
        public void WhenNgườiDungNhấnNutDangNhậpTrenGiaoDiệnDangNhập()
        {
            IWebElement dangNhapButton = wait.Until(d => d.FindElement(By.CssSelector("button.btn-auth")));
            dangNhapButton.Click();
        }

        [Then(@"hệ thống chuyển hướng sang trang chủ và hiển thị tên ""(.*)""")]
        public void ThenHệThốngChuyểnHướngSangTrangChủVaHiểnThịTen(string ketQuaMongDoi)
        {
            IWebElement nameElement = null;

            try
            {
                nameElement = wait.Until(d => d.FindElement(By.CssSelector("span.d-none.d-md-inline.fw-bold")));
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail($"Lỗi: Đăng nhập thất bại hoặc giao diện không tải được tên người dùng '{ketQuaMongDoi}'.");
            }

            Assert.That(nameElement.Text.ToLower(), Is.EqualTo(ketQuaMongDoi.ToLower()),
                "Tên người dùng hiển thị sau khi đăng nhập không khớp với mong đợi.");

        }
        [Then(@"hệ thống hiển thị tên lỗi ""(.*)""")]
        public void ThenHệThốngHiểnThịTenLỗi(string errorMessage)
        {
            if (errorMessage == "[gioi_han_ky_tu]")
            {
                IWebElement tenDangNhapBox = wait.Until(d => d.FindElement(By.Id("TenDangNhap")));
                IWebElement matKhauBox = wait.Until(d => d.FindElement(By.Id("MatKhau")));

                string maxLengthTenDanhNhapValue = tenDangNhapBox.GetAttribute("maxlength");
                string maxLengthMatKhauValue = matKhauBox.GetAttribute("maxlength");
                Assert.Multiple(() =>
                {
                    Assert.That(maxLengthTenDanhNhapValue, Is.Not.Null, "LỖI: Ô nhập liệu không hề cấu hình thuộc tính maxlength!");
                    Assert.That(maxLengthMatKhauValue, Is.Not.Null, "LỖI: Ô nhập liệu không hề cấu hình thuộc tính maxlength!");
                    if (maxLengthTenDanhNhapValue != null && maxLengthMatKhauValue != null)
                    {
                        if (maxLengthTenDanhNhapValue.Length > 100 || maxLengthMatKhauValue.Length > 100)
                        {
                            Assert.Fail("Lỗi: Ô nhập liệu không giới hạn số lượng tối đa");
                        }
                    }
                });
            }
            if (errorMessage == "[hien_thi_loi_2_truong]")
            {
                bool isBothErrorsDisplayed = false;
                try
                {
                    isBothErrorsDisplayed = wait.Until(d => {
                        var errorTexts = d.FindElements(By.CssSelector("span.field-validation-error")).Select(e => e.Text).ToList();
                        bool hasUserError = errorTexts.Any(t => t.Contains("Yêu cầu nhập tên đăng nhập", StringComparison.OrdinalIgnoreCase));
                        bool hasPassError = errorTexts.Any(t => t.Contains("Yêu cầu nhập mật khẩu", StringComparison.OrdinalIgnoreCase));
                        Console.WriteLine("CONSOLE CHECK: ", errorTexts);
                        Console.WriteLine("CONSOLE CHECK: ", errorTexts);
                        return hasUserError && hasPassError;
                    });
                }
                catch (WebDriverTimeoutException) { }

                Assert.That(isBothErrorsDisplayed, Is.True, "Lỗi: Không hiển thị đủ 2 thông báo lỗi yêu cầu nhập tên đăng nhập và mật khẩu.");
                return;
            }

            bool foundError = false;
            string actualAlertText = null;

            try
            {
                foundError = wait.Until(d => {
                    // Kiểm tra lỗi hiển thị tại trường nhập liệu (field validation)
                    var fieldErrors = d.FindElements(By.CssSelector("span.field-validation-error"));
                    if (fieldErrors.Any(e => e.Text.Contains(errorMessage, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }

                    // Nếu không có, kiểm tra thông báo lỗi chung của form (alert)
                    var alertElements = d.FindElements(By.CssSelector(".alert.alert-danger"));
                    if (alertElements.Count > 0)
                    {
                        actualAlertText = alertElements[0].Text;
                        if (actualAlertText.Contains(errorMessage, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }
            catch (WebDriverTimeoutException) { }

            if (!foundError)
            {
                if (!string.IsNullOrEmpty(actualAlertText))
                {
                    Assert.Fail($"Lỗi: Nội dung thông báo lỗi không chính xác. Mong đợi chứa: '{errorMessage}', Thực tế: '{actualAlertText}'");
                }
                else
                {
                    Assert.Fail($"Lỗi: Không tìm thấy thông báo lỗi '{errorMessage}' trên giao diện (cả ở trường nhập liệu và thông báo chung).");
                }
            }
        }
        

        [Then(@"hệ thống không chuyển sang trang chủ ""(.*)""")]
        public void ThenHệThốngKhongChuyểnSangTrangChủ(string loaiMaDoc)
        {
            IWebElement nameElement = null;
            try
            {
                nameElement = wait.Until(d => d.FindElement(By.CssSelector("span.d-none.d-md-inline.fw-bold")));

                if (loaiMaDoc == "[sql]")
                {
                    Assert.Fail("Lỗi bảo mật (SQL Injection): Hệ thống không chặn mã độc SQL hoặc hệ thống chỉ xem đây là chuỗi thuần túy");
                }
                else
                {
                    Assert.Fail("Lỗi bảo mật (HTML Injection/XSS): Hệ thống không thực hiện mã hóa (sanitize) đầu vào, cho phép tài khoản chứa mã độc đăng nhập thành công!");
                }
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Pass($"Thành công: Hệ thống đã chặn không cho đăng nhập khi sử dụng {loaiMaDoc}.");
            }
        }


        [Then(@"hệ thống thông báo không chi tiết ""(.*)""")]
        public void ThenHệThốngThongBaoKhongChiTiết(string errorMessage)
        {
            IWebElement warningElement = null;
            var tieuDeLoiCuaIIS = wait.Until(d => d.FindElements(By.XPath("//h1[contains(text(), 'Server Error in')]")));
            bool isLoiCoHienThiChiTiet = tieuDeLoiCuaIIS.Count > 0;

            if (isLoiCoHienThiChiTiet)
            {
                Assert.Fail("Lỗ hổng bảo mật: Hệ thống đang để lộ màn hình lỗi chi tiết (Yellow Screen of Death) của ASP.NET!);");
            }
            else
            {

                try
                {
                    warningElement = wait.Until(d => d.FindElement(By.CssSelector("div.alert.alert-danger")));
                }
                catch (WebDriverTimeoutException)
                {
                    Assert.Fail("Lỗi: Hệ thống không hiển thị thông báo lỗi đăng nhập (thay vào đó có thể đã đăng nhập thành công hoặc treo giao diện).");
                }

            // if(errorMessage == "[yellow_screen]")
            
                Assert.That(warningElement.Text.ToLower(), Is.EqualTo(errorMessage.ToLower()),
                    "Lỗi: Nội dung thông báo lỗi không hợp lệ");
            }

        }


        [AfterScenario]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

    }
}
