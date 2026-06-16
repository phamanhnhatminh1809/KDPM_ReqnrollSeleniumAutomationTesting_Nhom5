using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks; 

namespace DoAn.StepDefinitions
{
    [Binding]
    public class DatHangStepDefinitions
    {
        private IWebDriver driver => TestHooks.Driver; 
        private WebDriverWait wait;
        private string _hinhThucNhanHienTai = "";

        public DatHangStepDefinitions()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"Mở trang chủ nhà thuốc và tiến hành chuẩn bị giỏ hàng dữ liệu")]
        public void MoTrangChuVaChuanBiGioHang()
        {
            driver.Navigate().GoToUrl("http://localhost:44317/");
            System.Threading.Thread.Sleep(1500);

            var firstProduct = wait.Until(d => d.FindElement(By.CssSelector(".group:nth-child(1) > .relative:nth-child(4)")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", firstProduct);
            System.Threading.Thread.Sleep(1500);

            var addToCartBtn = wait.Until(d => d.FindElement(By.XPath(
                "//button[contains(.,'Thêm vào giỏ')] | //button[contains(.,'Mua ngay')] | //a[contains(.,'Thêm vào giỏ')]"
            )));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addToCartBtn);
            System.Threading.Thread.Sleep(1000);

            var cartBadge = wait.Until(d => d.FindElement(By.Id("cart-badge")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", cartBadge);
            System.Threading.Thread.Sleep(2000);
        }

        [When(@"Người dùng lựa chọn hình thức nhận hàng là ""(.*)""")]
        public void ChuyenHinhThucNhanHang(string hinhThucNhan)
        {
            _hinhThucNhanHienTai = hinhThucNhan; 

            var btnTarget = wait.Until(d => d.FindElement(By.XPath($"//button[contains(.,'{hinhThucNhan}')] | //*[contains(text(),'{hinhThucNhan}')]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnTarget);
            System.Threading.Thread.Sleep(1500);
        }

        [When(@"Người dùng điền thông tin cá nhân gồm họ tên ""(.*)"", số điện thoại ""(.*)"" và email ""(.*)""")]
        public void DienThongTinCaNhan(string hoTen, string soDienThoai, string email)
        {
            var txtHoTen = wait.Until(d => d.FindElement(By.Id("txtHoTen")));
            txtHoTen.Click();
            txtHoTen.Clear();
            if (!string.IsNullOrEmpty(hoTen)) txtHoTen.SendKeys(hoTen);

            var txtSDT = wait.Until(d => d.FindElement(By.Id("txtSoDienThoai")));
            txtSDT.Click();
            txtSDT.Clear();
            if (!string.IsNullOrEmpty(soDienThoai)) txtSDT.SendKeys(soDienThoai);

            var txtEmail = wait.Until(d => d.FindElement(By.Id("txtEmail")));
            txtEmail.Clear();
            if (!string.IsNullOrEmpty(email)) txtEmail.SendKeys(email);
        }

        [When(@"Với hình thức ""(.*)"" người dùng cấu hình địa chỉ theo tỉnh ""(.*)"", quận ""(.*)"", phường ""(.*)"", số nhà hoặc kho ""(.*)""")]
        public void CauHinhDiaChiGiaoNhan(string hinhThuc, string tinh, string quan, string phuong, string soNhaHoacKho)
        {
            bool isGiaoHangTanNoi = hinhThuc.Contains("Giao hàng tận nơi");

            if (isGiaoHangTanNoi)
            {
                Console.WriteLine("[TEST LOG] -> Chạy luồng: GIAO HÀNG TẬN NƠI");

                if (!string.IsNullOrEmpty(tinh))
                {
                    var ddlTinh = wait.Until(d => d.FindElement(By.Id("dataApiTinhThanh")));
                    SelectElement selectTinh = new SelectElement(ddlTinh);
                    try { selectTinh.SelectByText(tinh); }
                    catch (NoSuchElementException)
                    {
                        string tinhChuan = tinh.StartsWith("Thành phố") ? tinh : "Thành phố " + tinh;
                        selectTinh.SelectByText(tinhChuan);
                    }
                    System.Threading.Thread.Sleep(1000);
                }

                if (!string.IsNullOrEmpty(quan))
                {
                    var ddlQuan = wait.Until(d => d.FindElement(By.Id("dataApiQuanHuyen")));
                    wait.Until(d => d.FindElements(By.XPath("//select[@id='dataApiQuanHuyen']/option")).Count > 1);
                    SelectElement selectQuan = new SelectElement(ddlQuan);
                    try { selectQuan.SelectByText(quan); }
                    catch (NoSuchElementException)
                    {
                        string quanChuan = quan.Contains("Quận ") ? quan.Replace("Quận ", "").Trim() : "Quận " + quan;
                        selectQuan.SelectByText(quanChuan);
                    }
                    System.Threading.Thread.Sleep(1000);
                }

                if (!string.IsNullOrEmpty(phuong))
                {
                    var ddlXa = wait.Until(d => d.FindElement(By.Id("dataApiPhuongXa")));
                    wait.Until(d => d.FindElements(By.XPath("//select[@id='dataApiPhuongXa']/option")).Count > 1);
                    SelectElement selectXa = new SelectElement(ddlXa);
                    try { selectXa.SelectByText(phuong); }
                    catch (NoSuchElementException)
                    {
                        string phuongChuan = phuong.Contains("Phường ") ? phuong.Replace("Phường ", "").Trim() : "Phường " + phuong;
                        selectXa.SelectByText(phuongChuan);
                    }
                }

                if (!string.IsNullOrEmpty(soNhaHoacKho) && !soNhaHoacKho.Contains("Chưa trống") && !soNhaHoacKho.Contains("Chưa nhập"))
                {
                    var txtDiaChi = driver.FindElement(By.Id("txtDiaChiCuThe"));
                    txtDiaChi.Clear();
                    txtDiaChi.SendKeys(soNhaHoacKho);
                }
            }
            else
            {
                Console.WriteLine("[TEST LOG] -> Chạy luồng: NHẬN TẠI NHÀ THUỐC");

                if (!string.IsNullOrEmpty(tinh))
                {
                    var selectElementTinh = wait.Until(d => d.FindElement(By.Id("ddlTinh_Pickup")));
                    SelectElement selectTinh = new SelectElement(selectElementTinh);
                    selectTinh.SelectByText(tinh);
                    System.Threading.Thread.Sleep(1500);
                }

                if (!string.IsNullOrEmpty(quan))
                {
                    var selectElementQuan = wait.Until(d => d.FindElement(By.Id("ddlQuan_Pickup")));
                    SelectElement selectQuan = new SelectElement(selectElementQuan);
                    selectQuan.SelectByText(quan);

                    wait.Until(d => d.FindElements(By.XPath("//select[@id='ddlQuan_Pickup']/option")).Count > 1);
                    System.Threading.Thread.Sleep(2000); 
                }

                if (!string.IsNullOrEmpty(soNhaHoacKho)
                    && !soNhaHoacKho.Contains("Chưa click")
                    && !soNhaHoacKho.Contains("Không chọn")
                    && !soNhaHoacKho.Contains("<để trống>"))
                {
                    Console.WriteLine($"[TEST LOG] -> Đang dò kho động: '{soNhaHoacKho}'");

                    var waitStoreAJAX = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var storeLabel = waitStoreAJAX.Until(d => d.FindElement(By.XPath(
                        $"//*[contains(text(),'{soNhaHoacKho}')]" +
                        $" | //label[contains(.,'{soNhaHoacKho}')]" +
                        $" | //div[contains(text(),'{soNhaHoacKho}')]"
                    )));

                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'center'});", storeLabel);
                    System.Threading.Thread.Sleep(1000);

                    Console.WriteLine("[TEST LOG] -> Thực hiện click trực tiếp vào văn bản tên kho.");
                    try
                    {
                        storeLabel.Click(); 
                    }
                    catch (Exception)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", storeLabel);
                    }

                    Console.WriteLine($"[TEST LOG] -> Đã tích chọn nhà thuốc '{soNhaHoacKho}' THÀNH CÔNG.");
                }
                else
                {
                    Console.WriteLine("[TEST LOG] -> Kịch bản cố tình ĐỂ TRỐNG bước chọn kho.");
                    System.Threading.Thread.Sleep(1000);
                }

                System.Threading.Thread.Sleep(1500); 
            }
        }

        [When(@"Người dùng tích chọn phương thức thanh toán là ""(.*)""")]
        public void TichChonPhuongThucThanhToan(string phuongThuc)
        {
            IWebElement optionThanhToan;
            try
            {
                if (phuongThuc.Contains("COD") || phuongThuc.Contains("tiền mặt"))
                    optionThanhToan = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(),'tiền mặt')] | //*[contains(text(),'COD')]")));
                else
                    optionThanhToan = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(),'chuyển khoản')] | //*[contains(text(),'QR')]")));
            }
            catch (Exception)
            {
                string index = (phuongThuc.Contains("COD") || phuongThuc.Contains("tiền mặt")) ? "2" : "3";
                optionThanhToan = wait.Until(d => d.FindElement(By.CssSelector($".payment-option:nth-child({index}) span, .payment-option:nth-child({index})")));
            }

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", optionThanhToan);
            System.Threading.Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", optionThanhToan);
            System.Threading.Thread.Sleep(1000);
        }

        [When(@"Người dùng nhấn nút tiến hành Đặt Hàng Ngay")]
        public void NhanNutXacNhanDatHang()
        {
            var btnOrder = wait.Until(d => d.FindElement(By.XPath(
                "//button[contains(.,'ĐẶT HÀNG NGAY')] " +
                "| //button[contains(.,'Đặt hàng ngay')] " +
                "| //*[contains(text(),'ĐẶT HÀNG NGAY')]/.. " +
                "| //*[@id='btnPlaceOrder']"
            )));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnOrder);
            System.Threading.Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnOrder);
        }

        [Then(@"Hệ thống phải phản hồi trạng thái xử lý đặt hàng mong đợi là ""(.*)""")]
        public void KiemTraTrạngThaiDatHangTongHop(string ketQuaMongDoi)
        {
            System.Threading.Thread.Sleep(3000);
            string currentUrl = driver.Url.ToLower();
            string pageContent = driver.PageSource;

            bool isSuccess = currentUrl.Contains("success")
                             || currentUrl.Contains("thankyou")
                             || currentUrl.Contains("hoan-thanh")
                             || currentUrl.Contains("dathangthanhcong")
                             || (pageContent.Contains("đặt hàng thành công") && !currentUrl.Contains("checkout") && !currentUrl.Contains("thanhtoan"));

            if (ketQuaMongDoi == "THANH_CONG")
            {
                Assert.That(isSuccess, Is.True, "Lỗi chức năng: Dữ liệu đặt hàng hợp lệ nhưng hệ thống không điều hướng sang trang hoàn tất!");
                Console.WriteLine("[TEST LOG] -> XÁC NHẬN: Đặt hàng thành công hoàn tất, điều hướng chuẩn xác.");
            }
            else if (ketQuaMongDoi == "MO_MODAL_QR")
            {
                bool isModalDisplayed = pageContent.Contains("modal-body") || pageContent.ToLower().Contains("qr");
                Assert.That(isModalDisplayed, Is.True, "Lỗi: Hệ thống không hiển thị cửa sổ Modal chứa ảnh QRCode thanh toán!");
            }
            else if (ketQuaMongDoi == "CHAN_BUG")
            {
                Assert.That(isSuccess, Is.False, "--- [BUG LOGIC]: Họ tên chứa ký tự đặc biệt rác hệ thống vẫn cho phép đặt hàng! ---");
            }
            else if (ketQuaMongDoi == "LOI_HOTEN")
            {
                Assert.That(isSuccess, Is.False, "Lỗi nghiêm trọng: Để trống Họ tên hệ thống không chặn trên URL mà vẫn xử lý chuyển trang đặt đơn hàng!");

                var txtHoTen = driver.FindElement(By.Id("txtHoTen"));
                string validationMessage = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", txtHoTen);

                Assert.That(validationMessage, Is.Not.Null.And.Not.Empty, "Lỗi: Trình duyệt không kích hoạt thuộc tính required để chặn trống trường Họ tên!");
                Assert.That(validationMessage.ToLower().Contains("fill out") || validationMessage.Contains("vui lòng điền"), Is.True,
                    $"Lỗi: Thông báo chặn rỗng hiển thị sai nội dung. Thực tế: '{validationMessage}'");
            }
            else if (ketQuaMongDoi == "LOI_SDT")
            {
                Assert.That(isSuccess, Is.False, "Lỗi nghiêm trọng: Số điện thoại sai định dạng/trống hệ thống vẫn cho phép đặt hàng!");

                var txtSDT = driver.FindElement(By.Id("txtSoDienThoai"));
                string validationMessage = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", txtSDT);

                bool hasErrorAlert = pageContent.Contains("Số điện thoại") || !string.IsNullOrEmpty(validationMessage);
                Assert.That(hasErrorAlert, Is.True, "Lỗi: Hệ thống không đưa ra bất kỳ cảnh báo validate nào cho ô Số điện thoại rác/trống!");
            }
            else if (ketQuaMongDoi == "LOI_KHO")
            {
                Assert.That(isSuccess, Is.False, "Lỗi nghiêm trọng: Chưa chọn nhà thuốc lấy hàng hệ thống vẫn xử lý tạo đơn thành công!");

                string validationMessage = "";
                try
                {
                    var ddlQuanPickup = driver.FindElement(By.Id("ddlQuan_Pickup"));
                    validationMessage = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", ddlQuanPickup);

                    if (string.IsNullOrEmpty(validationMessage))
                    {
                        var storeRadio = driver.FindElement(By.XPath("//input[@type='radio' and @name='IdCuaHang']"));
                        validationMessage = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", storeRadio);
                    }
                }
                catch (Exception) { }

                Console.WriteLine($"[TEST LOG] Thông điệp chặn thiếu kho thu được: '{validationMessage}'");

                bool hasStoreError = pageContent.Contains("Vui lòng chọn một nhà thuốc")
                                     || pageContent.Contains("vui lòng chọn")
                                     || (!string.IsNullOrEmpty(validationMessage) && (validationMessage.Contains("chọn") || validationMessage.ToLower().Contains("select")));

                Assert.That(hasStoreError, Is.True, "Lỗi: Chưa chọn nhà thuốc hiển thị nhưng hệ thống không bật tooltip hoặc cảnh báo đỏ ngăn chặn!");
                Console.WriteLine("[TEST LOG] -> XÁC NHẬN: Hệ thống đã chặn lỗi bỏ trống kho lấy hàng thành công!");
            }
            else
            {
                Assert.That(isSuccess, Is.False, $"Lỗi Validation: Hệ thống không chặn dữ liệu thiếu hoặc sai định dạng của kịch bản '{ketQuaMongDoi}'!");
            }
        }
    }
}