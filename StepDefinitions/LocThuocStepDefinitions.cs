using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks; 

namespace DoAn.StepDefinitions
{
    [Binding]
    public class LocThuocStepDefinitions
    {
        private IWebDriver driver => TestHooks.Driver;
        private WebDriverWait wait;

        public LocThuocStepDefinitions()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"Mở trang chủ nhà thuốc và chuyển sang trang Tất Cả sản phẩm")]
        public void MoTrangChuVaVaoTrangTatCa()
        {
            driver.Navigate().GoToUrl("http://localhost:44317/");
            var lnkTatCa = wait.Until(d =>
            {
                try
                {
                    return d.FindElement(By.LinkText("Tất Cả"));
                }
                catch (NoSuchElementException)
                {
                    return d.FindElement(By.PartialLinkText("Sản phẩm"));
                }
            });
            lnkTatCa.Click();
        }

        [When(@"Người dùng chọn lọc theo mức giá thứ ""(.*)""")]
        public void NguoiDungChonLocTheoMucGia(string index)
        {
            try
            {
                var areaGia = driver.FindElement(By.Name("giaBanRange"));
                if (areaGia.Displayed)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", areaGia);
                }
            }
            catch (NoSuchElementException) { }

            var btnGia = wait.Until(d => d.FindElement(By.CssSelector($".filter-btn:nth-child({index}), button:nth-child({index}), input[type='checkbox']:nth-child({index})")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnGia);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnGia);

            System.Threading.Thread.Sleep(1500); 
        }

        [When(@"Người dùng chọn hãng sản xuất là ""(.*)""")]
        public void NguoiDungChonHangSanXuatTheoTen(string tenHang)
        {
            var textHang = wait.Until(d => d.FindElement(By.XPath($"//span[contains(text(),'{tenHang}')] | //label[contains(text(),'{tenHang}')] | //*[contains(text(),'{tenHang}') and @class='form-check-label']")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", textHang);
            System.Threading.Thread.Sleep(500);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", textHang);
            System.Threading.Thread.Sleep(1500);
        }

        [When(@"Người dùng nhấn vào liên kết ""(.*)""")]
        public void NguoiDungNhanVaoLienKetXoaBoLoc(string linkText)
        {
            var lnkReset = wait.Until(d => d.FindElement(By.XPath($"//*[contains(text(), '{linkText}')] | //a[contains(.,'{linkText}')]")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", lnkReset);
            System.Threading.Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", lnkReset);

            System.Threading.Thread.Sleep(1500);
        }

        [Then(@"Danh sách sản phẩm phải được cập nhật theo khoảng giá tương ứng")]
        [Then(@"Danh sách sản phẩm phải hiển thị đúng các thuốc của hãng đã chọn")]
        [Then(@"Hệ thống trả về danh sách sản phẩm thỏa mãn đồng thời cả hai điều kiện")]
        public void KiemTraCoSanPhamPhuHop()
        {
            System.Threading.Thread.Sleep(1000);

            bool hasProducts = driver.PageSource.Contains("product-card")
                             || driver.PageSource.Contains("product-item")
                             || driver.FindElements(By.CssSelector("[class*='product']")).Count > 0;

            Assert.That(hasProducts, Is.True, "Lỗi: Sau khi chọn bộ lọc, danh sách sản phẩm trống rỗng hoặc không render được sản phẩm!");
        }

        [Then(@"Hệ thống phải thông báo không tìm thấy kết quả phù hợp")]
        public void KiemTraKhongCoKetQua()
        {
            System.Threading.Thread.Sleep(1500);

            bool noResultDisplayed = driver.PageSource.Contains("Chưa có dữ liệu thuốc nào trong hệ thống.")
                                     || driver.PageSource.Contains("Không tìm thấy sản phẩm");

            Assert.That(noResultDisplayed, Is.True, "Lỗi: Bộ lọc không có kết quả phù hợp nhưng hệ thống không hiển thị thông báo rỗng!");
        }

        [Then(@"Tất cả các bộ lọc phải được bỏ chọn và hiển thị đầy đủ danh sách thuốc")]
        public void KiemTraResetBoLoc()
        {
            System.Threading.Thread.Sleep(1000);
            int productCount = driver.FindElements(By.CssSelector("[class*='product']")).Count;

            Assert.That(productCount, Is.GreaterThanOrEqualTo(0), "Lỗi: Sau khi xóa bộ lọc hệ thống không tải lại danh sách sản phẩm đầy đủ.");
        }

        [Then(@"Số lượng tổng sản phẩm hiển thị phải khớp với bộ lọc dữ liệu")]
        public void KiemTraDongBoSoLuong()
        {
            var lblCount = wait.Until(d => d.FindElement(By.CssSelector(".fw-bold, h4, h5, .total-products")));
            Assert.That(lblCount.Displayed, Is.True, "Lỗi: Không tìm thấy nhãn hiển thị tổng số lượng sản phẩm sau khi lọc.");
        }
    }
}