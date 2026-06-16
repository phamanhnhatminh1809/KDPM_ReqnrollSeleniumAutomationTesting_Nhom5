using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks; 

namespace DoAn.StepDefinitions
{
    [Binding]
    public class TimKiemThuocStepDefinitions
    {
        private IWebDriver driver => TestHooks.Driver;
        private WebDriverWait wait;

        public TimKiemThuocStepDefinitions()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"Mở trang chủ nhà thuốc để chuẩn bị tìm kiếm")]
        public void MoTrangChuNhaThuoc()
        {
            driver.Navigate().GoToUrl("http://localhost:44317/");
        }

        [When(@"Người dùng nhập từ khóa tìm kiếm là ""(.*)""")]
        public void NguoiDungNhapTuKhoa(string tuKhoa)
        {
            var txtSearch = wait.Until(d => d.FindElement(By.XPath("//input[@type='search'] | //input[contains(@placeholder,'Tìm kiếm')] | //input[contains(@id,'search')]")));
            txtSearch.Click();
            txtSearch.Clear();
            txtSearch.SendKeys(tuKhoa);
        }

        [When(@"Nhấn nút tìm kiếm hoặc Enter")]
        public void NhanNutTimKiem()
        {
            try
            {
                var btnSearch = driver.FindElement(By.XPath("//button[@type='submit'] | //button[contains(.,'Tìm')] | //i[contains(@class,'search')]/.."));
                btnSearch.Click();
            }
            catch (NoSuchElementException)
            {
                var txtSearch = driver.FindElement(By.XPath("//input[@type='search'] | //input[contains(@placeholder,'Tìm kiếm')]"));
                txtSearch.SendKeys(Keys.Enter);
            }
        }

        [Then(@"Hệ thống phải phản hồi kết quả mong đợi là ""(.*)"" và chứa từ khóa ""(.*)""")]
        public void KiemTraKetQuaTimKiem(string ketQuaMongDoi, string tuKhoaKiemTra)
        {
            var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            if (ketQuaMongDoi == "CO_SAN_PHAM")
            {
                try
                {
                    shortWait.Until(d => d.PageSource.ToLower().Contains(tuKhoaKiemTra.ToLower()));

                    Assert.Pass();
                }
                catch (WebDriverTimeoutException)
                {
                    Assert.Fail($"Lỗi: Đã đợi 5s nhưng tìm kiếm từ khóa '{tuKhoaKiemTra}' không thấy sản phẩm tương ứng xuất hiện trên giao diện.");
                }
            }
            else if (ketQuaMongDoi == "KHONG_CO_SP")
            {
                System.Threading.Thread.Sleep(1000);

                bool noResultDisplayed = driver.PageSource.Contains("Không tìm thấy")
                                         || driver.PageSource.Contains("0 sản phẩm")
                                         || driver.PageSource.Contains("Không có kết quả")
                                         || driver.PageSource.Contains("Chưa có dữ liệu thuốc nào trong hệ thống."); // <- Thêm dòng này

                Assert.That(noResultDisplayed, Is.True,
                    "Lỗi: Nhập từ khóa không tồn tại nhưng hệ thống không hiển thị thông báo 'Chưa có dữ liệu thuốc nào trong hệ thống.'!");
            }
        }
    }
}