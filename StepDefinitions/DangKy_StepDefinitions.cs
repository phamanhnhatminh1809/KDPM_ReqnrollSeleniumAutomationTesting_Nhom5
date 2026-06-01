using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System.Xml.Linq;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Support;
using SeleniumExtras.WaitHelpers;
namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.StepDefinitions
{
    [Binding]
    [Scope(Tag = "dang_ky_feature")]
    public class DangKy_StepDefinitions
    {
        private static TestDatabaseCheckpoint _checkpoint = new TestDatabaseCheckpoint();
        private IWebDriver driver;
        private WebDriverWait wait;

        [BeforeScenario]
        public async Task Setup()
        {
            await _checkpoint.InitializeCheckpointAsync();
            await _checkpoint.ResetDatabaseAsync();

            ChromeOptions options = new ChromeOptions();

            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"người dùng truy cập vào trang web ""(.*)""")]
        public void GivenNgườiDungTruyCậpVaoTrangWeb(string link_web)
        {
            driver.Navigate().GoToUrl(link_web);
        }

        [Given("người dùng nhấn vào nút Đăng ký")]
        public void GivenNgườiDungNhấnVaoNutDangKy()
        {
            IWebElement registerButton = wait.Until(d => d.FindElement(By.CssSelector("a[href*='/TaiKhoan/DangKy']")));
            registerButton.Click();
        }
        [When("người dùng nhấn vào nút Đăng ký")]
        public void WhenNgườiDungNhấnVaoNutDangKy()
        {
            IWebElement registerButton = wait.Until(d => d.FindElement(By.CssSelector("a[href*='/TaiKhoan/DangKy']")));
            registerButton.Click();
        }


        [When(@"người dùng nhập thông tin đăng ký bao gồm ""(.*)"", ""(.*)"", ""(.*?)"" và ""(.*?)""")]
        public void WhenNgườiDungNhậpThongTinDangKyBaoGồmVa(string tenDangNhap, string email, string matKhau, string xacNhanMatKhau)
        {
            IWebElement tenDangNhapBox = wait.Until(d => d.FindElement(By.Id("TenDangNhap")));
            IWebElement emailBox = wait.Until(d => d.FindElement(By.Id("Email")));
            IWebElement matKhauBox = wait.Until(d => d.FindElement(By.Id("MatKhau")));
            IWebElement xacNhanMatKhauBox = wait.Until(d => d.FindElement(By.Id("XacNhanMatKhau")));

            if (tenDangNhap.Contains("[1000_ky_tu]"))
            {
                for (int i = 0; i < 1001; i++)
                {
                    tenDangNhap += "a";
                }
            }
            if (matKhau.Contains("[1000_ky_tu]"))
            {
                for (int i = 0; i < 1001; i++)
                {
                    matKhau += "a";
                }
            }
            if (xacNhanMatKhau.Contains("[1000_ky_tu]"))
            {
                for (int i = 0; i < 1001; i++)
                {
                    matKhau += "a";
                }
            }
        
            tenDangNhapBox.Clear();
            tenDangNhapBox.SendKeys(tenDangNhap);

            emailBox.Clear();
            emailBox.SendKeys(email);

            matKhauBox.Click();
            matKhauBox.Clear();
            matKhauBox.SendKeys(matKhau);

            xacNhanMatKhauBox.Click();
            xacNhanMatKhauBox.Clear();
            xacNhanMatKhauBox.SendKeys(xacNhanMatKhau);
        }

        [When("người dùng nhấn nút Đăng ký trên giao diện đăng ký")]
        public void WhenNgườiDungNhấnNutDangKyTrenGiaoDiệnDangKy()
        {
            IWebElement dangKyNut = wait.Until(d => d.FindElement(By.CssSelector("button.btn-auth")));
            dangKyNut.Click();
        }

        [Scope(Tag = "luong_thanh_cong")]
        [When("hệ thống chuyển hướng sang trang đăng nhập")]
        public void WhenHệThốngChuyểnHướngSangTrangDangNhập()
        {
            IWebElement tieuDeDangNhap = wait.Until(d => d.FindElement(By.CssSelector("h2.auth-title")));
            Assert.That(tieuDeDangNhap.Text.ToLower(), Does.Contain("đăng nhập"), "Lỗi: Tên lỗi không khớp");
        }


        [Scope(Tag = "kiem_tra_rang_buoc")]
        [Scope(Tag = "kiem_tra_trung_lap")]
        [Then(@"hệ thống thông báo lỗi ""(.*)""")]
        public void ThenHệThốngThongBaoLỗi(string errorMessage)
        {
            // Kiểm tra xem có bị chuyển hướng sang trang Đăng nhập hay không
            var dangNhapElements = driver.FindElements(By.CssSelector("h2.lauth-title"));
            bool daChuyenSangDangNhap = dangNhapElements.Count > 0;

            var tieuDeLoiCuaIIS = driver.FindElements(By.XPath("//h1[contains(text(), 'Server Error in')]"));
            bool isLoiCoHienThiChiTiet = tieuDeLoiCuaIIS.Count > 0;

            if (!isLoiCoHienThiChiTiet)
            {
                string pageSource = driver.PageSource;
                if (pageSource.Contains("HttpRequestValidationException") || pageSource.Contains("Stack Trace"))
                {
                    isLoiCoHienThiChiTiet = true;
                }
            }

            if (errorMessage == "[bao_mat_dau_vao_html]")
            {
                Assert.That(daChuyenSangDangNhap, Is.False, "Lỗ hổng bảo mật: Hệ thống đã chấp nhận dữ liệu mã độc và cho phép chuyển sang trang Đăng nhập!");
            }
            else if(errorMessage == "[bao_mat_dau_vao_sql]")
            {
                Assert.That(daChuyenSangDangNhap, Is.True, "Lỗ hổng bảo mật: Hệ thống có thể chấp mã sql, cần kiểm tra xem có hiển thị thông tin lỗi sql ở phần thông tin người dùng!");

            }else if (errorMessage == "[gioi_han_ky_tu]")
            {
                IWebElement tenDangNhapBox = wait.Until(d => d.FindElement(By.Id("TenDangNhap")));
                IWebElement matKhauBox = wait.Until(d => d.FindElement(By.Id("MatKhau")));
                IWebElement xacNhanMatKhauBox = wait.Until(d => d.FindElement(By.Id("XacNhanMatKhau")));

                string maxLengthTenDanhNhapValue = tenDangNhapBox.GetAttribute("maxlength");
                string maxLengthMatKhauValue = matKhauBox.GetAttribute("maxlength");
                string maxLengthXacNhanMatKhauValue = xacNhanMatKhauBox.GetAttribute("maxlength");

                Assert.Multiple(() =>
                {
                    Assert.That(maxLengthTenDanhNhapValue, Is.Not.Null, "LỖI: Ô Tên đăng nhập không cấu hình thuộc tính maxlength!");
                    Assert.That(maxLengthMatKhauValue, Is.Not.Null, "LỖI: Ô Mật khẩu không cấu hình thuộc tính maxlength!");
                    Assert.That(maxLengthXacNhanMatKhauValue, Is.Not.Null, "LỖI: Ô Xác nhận mật khẩu không cấu hình thuộc tính maxlength!");

                    if (maxLengthTenDanhNhapValue != null && maxLengthMatKhauValue != null && maxLengthXacNhanMatKhauValue != null)
                    {
                        if (maxLengthTenDanhNhapValue.Length > 100 || maxLengthMatKhauValue.Length > 100 || maxLengthXacNhanMatKhauValue.Length > 100)
                        {
                            Assert.Fail("Lỗi: Có ô nhập liệu vượt quá giới hạn 100 ký tự cho phép");
                        }
                    }
                });
            }
            else if (errorMessage == "[khong_hien_thi_loi_chi_tiet]")
            {
                Assert.That(isLoiCoHienThiChiTiet, Is.False,
        "Lỗ hổng bảo mật: Hệ thống đang để lộ màn hình lỗi chi tiết (Yellow Screen of Death) của ASP.NET!");
            }
            else
            {
                // Nếu hệ thống lỡ cho qua trang Đăng nhập khi đang test lỗi thông thường -> Fail ngay
                if (daChuyenSangDangNhap)
                {
                    Assert.Fail($"Test thất bại: Hệ thống không báo lỗi '{errorMessage}' mà lại chuyển thẳng sang trang Đăng nhập.");
                }

                // Nếu vẫn ở trang Đăng ký, bắt đầu tìm thông báo lỗi
                var errorElements = wait.Until(d => d.FindElements(By.CssSelector("span.field-validation-error")));

                bool isHienThiLoi = false;
                foreach (var element in errorElements)
                {
                    if (element.Text.Contains(errorMessage.ToLower(), StringComparison.OrdinalIgnoreCase))
                    {

                        isHienThiLoi = true;
                        break;
                    }
                    System.Console.WriteLine("CHECK: errorInput: " + errorMessage);
                    System.Console.WriteLine("CHECK: errorText: " + element.Text);
                }

                Assert.That(isHienThiLoi, Is.True, $"Tên lỗi '{errorMessage}' không được hiển thị trên giao diện.");

            }
        }
        [Scope(Tag = "bao_mat_ui_chong_spam")]
        [When("người dùng nhấn vào nút Đăng ký trong điều kiện hệ thống đang chậm")]
        public void WhenNgườiDungNhấnVaoNutDangKyTrongDiềuKiệnHệThốngDangChậm()
        {
            Network.SetNetworkSlow3G((ChromeDriver)driver);
            IWebElement registerButton = driver.FindElement(By.CssSelector("a[href*='/TaiKhoan/DangKy']"));
            registerButton.Click();
        }

        [Scope(Tag = "bao_mat_ui_chong_spam")]
        [Then(@"nút Đăng ký phải bị vô hiệu hóa trong lúc chờ xử lý")]
        public void ThenNutPhảiBịVoHiệuHoaTrongLucChờXửLy()
        {
            IWebElement buttonDangKy = wait.Until(d => d.FindElement(By.CssSelector("button.btn-auth")));
            Assert.That(buttonDangKy.GetAttribute("disabled"), Is.Not.Null, "Nút phải bị disabled!");
            Network.ResetNetwork((ChromeDriver)driver);
        }

        [Scope(Tag = "hien_thi_ui_canh_bao")]
        [Then("hệ thống hiển thị cảnh báo đỏ cho tất cả các trường")]
        public void ThenHệThốngHiểnThịCảnhBaoDỏChoTấtCảCacTrường()
        {
            var canhBaoElement = wait.Until(d => d.FindElements(By.CssSelector(".field-validation-valid")));
            bool isMauDo = false;

            foreach(var element in canhBaoElement)
            {
                string colorValue = element.GetCssValue("color");

                if (colorValue.Contains("255, 0, 0") || colorValue.Contains("220, 53, 69"))
                {
                    isMauDo = true;
                }
                else
                {
                    isMauDo = false;
                    break;
                }
            }
            Assert.That(isMauDo, Is.True, "Có thông báo lỗi nhưng không phải màu đỏ!");

        }

        [Scope(Tag = "hien_thi_ui_an_mat_khau")]
        [When("mật khẩu phải ở dạng dấu chấm")]
        public void WhenMậtKhẩuPhảiỞDạngDấuChấm()
        {
            IWebElement matKhauBox = wait.Until(d => d.FindElement(By.Id("MatKhau")));
            IWebElement xacNhanMatKhauBox = wait.Until(d => d.FindElement(By.Id("XacNhanMatKhau")));

            string matKhauType = matKhauBox.GetAttribute("type");
            string xacNhanType = xacNhanMatKhauBox.GetAttribute("type");

            Assert.That(matKhauType, Is.EqualTo("password"), "Trường Mật khẩu không hiển thị dạng dấu chấm bảo mật!");
            Assert.That(xacNhanType, Is.EqualTo("password"), "Trường Xác nhận mật khẩu không hiển thị dạng dấu chấm bảo mật!");
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
