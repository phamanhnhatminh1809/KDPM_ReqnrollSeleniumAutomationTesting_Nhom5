using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks;
using System.Text.RegularExpressions;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.StepDefinitions
{
    [Binding]
    public class DashboardSteps
    {
        private const string BaseUrl = "http://localhost:44317";
        private const string LoginUrl = "/TaiKhoan/DangNhap";
        private const string DashboardUrl = "/Dashboard/Index";

        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";

        private IWebDriver Driver => TestHooks.Driver;

        private string FullUrl(string path)
        {
            return BaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        private WebDriverWait Wait()
        {
            return new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        private void WaitReady()
        {
            Wait().Until(d =>
                ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").ToString() == "complete"
            );
        }

        private string BodyText()
        {
            return Driver.FindElement(By.TagName("body")).Text;
        }

        private void AssertNoServerError()
        {
            string text = BodyText();
            string title = Driver.Title ?? "";

            string[] keywords =
            {
                "Server Error in",
                "Runtime Error",
                "Exception Details",
                "The resource cannot be found",
                "Object reference not set",
                "Stack Trace",
                "Compilation Error",
                "Parser Error",
                "404 - File or directory not found"
            };

            foreach (string key in keywords)
            {
                if (text.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                    title.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Fail("Phát hiện trang lỗi hệ thống: " + key);
                }
            }
        }

        private int NumberFromText(string text)
        {
            string digits = Regex.Replace(text ?? "", @"\D", "");
            return string.IsNullOrWhiteSpace(digits) ? 0 : int.Parse(digits);
        }

        private long MoneyFromText(string text)
        {
            string digits = Regex.Replace(text ?? "", @"\D", "");
            return string.IsNullOrWhiteSpace(digits) ? 0 : long.Parse(digits);
        }

        private string GetCardValue(string labelText)
        {
            var cards = Driver.FindElements(By.CssSelector(".card"));

            foreach (var card in cards)
            {
                string text = card.Text.Trim();

                if (text.Contains(labelText, StringComparison.OrdinalIgnoreCase))
                {
                    var lines = text.Split('\n')
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                                    .ToList();

                    foreach (var line in lines)
                    {
                        if (!line.Equals(labelText, StringComparison.OrdinalIgnoreCase) &&
                            line.Any(char.IsDigit))
                        {
                            return line;
                        }
                    }
                }
            }

            Assert.Fail("Không tìm thấy thẻ số liệu: " + labelText);
            return "";
        }

        private IWebElement GetRecentTransactionTable()
        {
            var tables = Driver.FindElements(By.CssSelector("table"));

            foreach (var table in tables)
            {
                string text = table.Text;

                if (text.Contains("Mã HĐ") &&
                    text.Contains("Người bán") &&
                    text.Contains("Người nhận"))
                {
                    return table;
                }
            }

            Assert.Fail("Không tìm thấy bảng Giao dịch gần đây");
            return null!;
        }

        private List<Dictionary<string, string>> ParseRecentRows()
        {
            var table = GetRecentTransactionTable();
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var data = new List<Dictionary<string, string>>();

            foreach (var row in rows)
            {
                var cols = row.FindElements(By.CssSelector("td"));

                if (cols.Count >= 6)
                {
                    data.Add(new Dictionary<string, string>
                    {
                        ["ma_hd"] = cols[0].Text.Trim(),
                        ["nguoi_ban"] = cols[1].Text.Trim(),
                        ["nguoi_nhan"] = cols[2].Text.Trim(),
                        ["thoi_gian"] = cols[3].Text.Trim(),
                        ["tong_tien"] = cols[4].Text.Trim(),
                        ["trang_thai"] = cols[5].Text.Trim()
                    });
                }
            }

            return data;
        }

        private void OpenDashboard()
        {
            Driver.Navigate().GoToUrl(FullUrl(DashboardUrl));
            WaitReady();

            string page = BodyText();

            if (page.Contains("Đăng Nhập") && page.Contains("Tên đăng nhập"))
            {
                LoginAdmin();
            }
        }

        private void LoginAdmin()
        {
            Driver.Navigate().GoToUrl(FullUrl(LoginUrl));
            WaitReady();

            var usernameInput = Wait().Until(d => d.FindElement(By.Id("TenDangNhap")));
            var passwordInput = Wait().Until(d => d.FindElement(By.Id("MatKhau")));

            usernameInput.Clear();
            usernameInput.SendKeys(AdminUsername);

            passwordInput.Clear();
            passwordInput.SendKeys(AdminPassword);

            var loginButton = Driver.FindElements(By.XPath("//button[contains(., 'Đăng Nhập')]")).FirstOrDefault();

            if (loginButton == null)
            {
                Assert.Fail("Không tìm thấy nút Đăng Nhập");
            }

            loginButton.Click();

            Thread.Sleep(1500);

            Driver.Navigate().GoToUrl(FullUrl(DashboardUrl));
            WaitReady();

            string pageAfter = BodyText();

            if (pageAfter.Contains("Đăng Nhập") && pageAfter.Contains("Tên đăng nhập"))
            {
                Assert.Fail("Đăng nhập Admin thất bại, vẫn còn ở trang đăng nhập");
            }
        }

        [Given("Admin đã đăng nhập vào hệ thống MedForAll")]
        public void GivenAdminDaDangNhapVaoHeThongMedForAll()
        {
            LoginAdmin();
        }

        [When("Admin truy cập trang Tổng quan Dashboard")]
        public void WhenAdminTruyCapTrangTongQuanDashboard()
        {
            OpenDashboard();
        }

        [Then("trang Dashboard mở được và không hiển thị lỗi hệ thống")]
        public void ThenTrangDashboardMoDuocVaKhongHienThiLoiHeThong()
        {
            AssertNoServerError();
        }

        [Then("trang Dashboard hiển thị tiêu đề Dashboard hoặc Tổng quan")]
        public void ThenTrangDashboardHienThiTieuDeDashboardHoacTongQuan()
        {
            string page = BodyText();
            Assert.That(page, Does.Contain("Dashboard").Or.Contain("Tổng quan"));
        }

        [Then("sidebar hiển thị đủ các menu quản trị")]
        public void ThenSidebarHienThiDuCacMenuQuanTri()
        {
            string page = BodyText();

            string[] requiredMenus =
            {
                "Tổng quan",
                "Danh sách thuốc",
                "Quản lý thuốc bán online",
                "Quản lý Lô thuốc",
                "Quản lý Lô tiêu huỷ",
                "Danh mục",
                "Hoá đơn"
            };

            foreach (string menu in requiredMenus)
            {
                Assert.That(page, Does.Contain(menu), "Sidebar thiếu menu: " + menu);
            }
        }

        [Then("menu Tổng quan đang ở trạng thái active")]
        public void ThenMenuTongQuanDangOTrangThaiActive()
        {
            var active = Driver.FindElements(By.CssSelector(".sidebar a.active"));

            Assert.That(active.Count, Is.GreaterThan(0), "Không tìm thấy menu active");
            Assert.That(active[0].Text, Does.Contain("Tổng quan"));
        }

        [Then("Dashboard hiển thị thẻ Doanh thu hôm nay")]
        public void ThenDashboardHienThiTheDoanhThuHomNay()
        {
            Assert.That(GetCardValue("Doanh thu hôm nay"), Is.Not.Empty);
        }

        [Then("Dashboard hiển thị thẻ Đơn hàng mới")]
        public void ThenDashboardHienThiTheDonHangMoi()
        {
            Assert.That(GetCardValue("Đơn hàng mới"), Is.Not.Empty);
        }

        [Then("Dashboard hiển thị thẻ Lô sắp hết hạn")]
        public void ThenDashboardHienThiTheLoSapHetHan()
        {
            Assert.That(GetCardValue("Lô sắp hết hạn"), Is.Not.Empty);
        }

        [Then("Dashboard hiển thị thẻ Thuốc hết hàng")]
        public void ThenDashboardHienThiTheThuocHetHang()
        {
            Assert.That(GetCardValue("Thuốc hết hàng"), Is.Not.Empty);
        }

        [Then("giá trị Doanh thu hôm nay không được âm")]
        public void ThenGiaTriDoanhThuHomNayKhongDuocAm()
        {
            long value = MoneyFromText(GetCardValue("Doanh thu hôm nay"));
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Then("giá trị Đơn hàng mới không được âm")]
        public void ThenGiaTriDonHangMoiKhongDuocAm()
        {
            int value = NumberFromText(GetCardValue("Đơn hàng mới"));
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Then("giá trị Lô sắp hết hạn không được âm")]
        public void ThenGiaTriLoSapHetHanKhongDuocAm()
        {
            int value = NumberFromText(GetCardValue("Lô sắp hết hạn"));
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Then("giá trị Thuốc hết hàng không được âm")]
        public void ThenGiaTriThuocHetHangKhongDuocAm()
        {
            int value = NumberFromText(GetCardValue("Thuốc hết hàng"));
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Then("biểu đồ doanh thu được render thành công")]
        public void ThenBieuDoDoanhThuDuocRenderThanhCong()
        {
            var canvas = Wait().Until(d => d.FindElement(By.Id("myAreaChart")));

            long width = Convert.ToInt64(((IJavaScriptExecutor)Driver)
                .ExecuteScript("return arguments[0].width || arguments[0].clientWidth;", canvas));

            long height = Convert.ToInt64(((IJavaScriptExecutor)Driver)
                .ExecuteScript("return arguments[0].height || arguments[0].clientHeight;", canvas));

            Assert.That(width, Is.GreaterThan(0));
            Assert.That(height, Is.GreaterThan(0));
        }

        [Then("biểu đồ doanh thu có đủ 7 nhãn ngày")]
        public void ThenBieuDoDoanhThuCoDu7NhanNgay()
        {
            long labelCount = Convert.ToInt64(((IJavaScriptExecutor)Driver)
                .ExecuteScript("return (window.labels || []).length;"));

            Assert.That(labelCount, Is.EqualTo(7));
        }

        [Then("biểu đồ doanh thu có đủ 7 giá trị doanh thu")]
        public void ThenBieuDoDoanhThuCoDu7GiaTriDoanhThu()
        {
            long dataCount = Convert.ToInt64(((IJavaScriptExecutor)Driver)
                .ExecuteScript("return (window.dataValues || []).length;"));

            Assert.That(dataCount, Is.EqualTo(7));
        }

        [Then("giá trị Doanh thu hôm nay phải bằng giá trị ngày cuối trong biểu đồ doanh thu")]
        public void ThenGiaTriDoanhThuHomNayPhaiBangGiaTriNgayCuoiBieuDo()
        {
            long revenue = MoneyFromText(GetCardValue("Doanh thu hôm nay"));

            object lastValueObj = ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                var d = window.dataValues || [];
                return d.length ? d[d.length - 1] : null;
            ");

            Assert.That(lastValueObj, Is.Not.Null, "Không đọc được dataValues của biểu đồ");

            long lastValue = Convert.ToInt64(lastValueObj);
            Assert.That(lastValue, Is.EqualTo(revenue));
        }

        [Then("khu vực Top 5 Bán Chạy được hiển thị")]
        public void ThenKhuVucTop5BanChayDuocHienThi()
        {
            Assert.That(BodyText(), Does.Contain("Top 5 Bán Chạy"));
        }

        [Then("danh sách Top 5 Bán Chạy có sản phẩm")]
        public void ThenDanhSachTop5BanChayCoSanPham()
        {
            var items = Driver.FindElements(By.CssSelector(".list-group-item"));
            Assert.That(items.Count, Is.GreaterThan(0));
            Assert.That(items.Count, Is.LessThanOrEqualTo(5));
        }

        [Then("mỗi sản phẩm bán chạy có thông tin số lượng đã bán")]
        public void ThenMoiSanPhamBanChayCoThongTinSoLuongDaBan()
        {
            var items = Driver.FindElements(By.CssSelector(".list-group-item"));

            foreach (var item in items)
            {
                string text = item.Text;
                Assert.That(text, Does.Contain("Đã bán:"));
                Assert.That(text.Any(char.IsDigit), Is.True);
            }
        }

        [Then("bảng Giao dịch gần đây được hiển thị")]
        public void ThenBangGiaoDichGanDayDuocHienThi()
        {
            Assert.That(GetRecentTransactionTable(), Is.Not.Null);
        }

        [Then("bảng Giao dịch gần đây có dữ liệu")]
        public void ThenBangGiaoDichGanDayCoDuLieu()
        {
            var rows = ParseRecentRows();
            Assert.That(rows.Count, Is.GreaterThan(0));
        }

        [Then("mỗi dòng giao dịch có mã hóa đơn, người bán, người nhận, thời gian, tổng tiền và trạng thái")]
        public void ThenMoiDongGiaoDichCoDayDuThongTin()
        {
            var rows = ParseRecentRows();

            foreach (var row in rows)
            {
                Assert.That(row["ma_hd"], Does.StartWith("#"));
                Assert.That(row["nguoi_ban"], Is.Not.Empty);
                Assert.That(row["nguoi_nhan"], Is.Not.Empty);
                Assert.That(row["thoi_gian"], Is.Not.Empty);
                Assert.That(MoneyFromText(row["tong_tien"]), Is.GreaterThanOrEqualTo(0));
                Assert.That(row["trang_thai"], Is.Not.Empty);
            }
        }

        [Then("Doanh thu hôm nay phải bằng tổng tiền các giao dịch trong ngày hiện tại")]
        public void ThenDoanhThuHomNayBangTongTienGiaoDichHomNay()
        {
            long revenueCard = MoneyFromText(GetCardValue("Doanh thu hôm nay"));
            var rows = ParseRecentRows();

            string today = DateTime.Now.ToString("dd/MM/yyyy");

            long sumToday = rows
                .Where(r => r["thoi_gian"].StartsWith(today))
                .Sum(r => MoneyFromText(r["tong_tien"]));

            Assert.That(sumToday, Is.EqualTo(revenueCard));
        }

        [Then("số Đơn hàng mới phải bằng số giao dịch trong ngày hiện tại")]
        public void ThenSoDonHangMoiBangSoGiaoDichHomNay()
        {
            int newOrders = NumberFromText(GetCardValue("Đơn hàng mới"));
            var rows = ParseRecentRows();

            string today = DateTime.Now.ToString("dd/MM/yyyy");

            int countToday = rows.Count(r => r["thoi_gian"].StartsWith(today));

            Assert.That(countToday, Is.EqualTo(newOrders));
        }

        [When("Admin nhấn nút In Báo Cáo Nhanh")]
        public void WhenAdminNhanNutInBaoCaoNhanh()
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                window._printCalled = false;
                window.print = function() {
                    window._printCalled = true;
                };
            ");

            var btn = Driver.FindElements(By.XPath("//button[contains(., 'In Báo Cáo Nhanh')]")).FirstOrDefault();

            if (btn == null)
            {
                Assert.Fail("Không tìm thấy nút In Báo Cáo Nhanh");
            }

            btn.Click();
            Thread.Sleep(1200);
        }

        [Then("hệ thống gọi chức năng in báo cáo")]
        public void ThenHeThongGoiChucNangInBaoCao()
        {
            bool printCalled = Convert.ToBoolean(((IJavaScriptExecutor)Driver)
                .ExecuteScript("return window._printCalled === true;"));

            Assert.That(printCalled, Is.True);
        }

        [Then("biểu đồ được chuyển thành ảnh trong vùng in báo cáo")]
        public void ThenBieuDoDuocChuyenThanhAnhTrongVungIn()
        {
            string imgSrc = Convert.ToString(((IJavaScriptExecutor)Driver).ExecuteScript(@"
                let img = document.getElementById('print-chart-img');
                return img ? img.src : '';
            ")) ?? "";

            Assert.That(imgSrc, Does.StartWith("data:image/png"));
        }

        [Then("vùng in báo cáo có tiêu đề báo cáo hoạt động trong ngày")]
        public void ThenVungInBaoCaoCoTieuDe()
        {
            string text = GetPrintAreaText();
            Assert.That(text, Does.Contain("BÁO CÁO HOẠT ĐỘNG TRONG NGÀY"));
        }

        [Then("vùng in báo cáo có phần tổng hợp số liệu hôm nay")]
        public void ThenVungInBaoCaoCoTongHopSoLieu()
        {
            string text = GetPrintAreaText();
            Assert.That(text, Does.Contain("Tổng hợp số liệu hôm nay"));
        }

        [Then("vùng in báo cáo có biểu đồ doanh thu 7 ngày qua")]
        public void ThenVungInBaoCaoCoBieuDoDoanhThu()
        {
            string text = GetPrintAreaText();
            Assert.That(text, Does.Contain("Biểu đồ doanh thu 7 ngày qua"));
        }

        [Then("vùng in báo cáo có Top 5 thuốc bán chạy nhất")]
        public void ThenVungInBaoCaoCoTop5BanChay()
        {
            string text = GetPrintAreaText();
            Assert.That(text, Does.Contain("Top 5 Thuốc bán chạy nhất"));
        }

        private string GetPrintAreaText()
        {
            return Convert.ToString(((IJavaScriptExecutor)Driver).ExecuteScript(@"
                let el = document.getElementById('print-area');
                return el ? el.innerText : '';
            ")) ?? "";
        }

        [Then("nút Thoát hiển thị trên giao diện Dashboard")]
        public void ThenNutThoatHienThi()
        {
            var logoutLinks = Driver.FindElements(By.XPath("//a[contains(., 'Thoát')]"));
            Assert.That(logoutLinks.Count, Is.GreaterThan(0));
        }

        [Then("nút Thoát có đường dẫn hợp lệ")]
        public void ThenNutThoatCoDuongDanHopLe()
        {
            var logoutLinks = Driver.FindElements(By.XPath("//a[contains(., 'Thoát')]"));
            string href = logoutLinks.First().GetAttribute("href") ?? "";
            Assert.That(href, Is.Not.Empty);
        }

        [Given("trình duyệt đang ở kích thước màn hình mobile 390px")]
        public void GivenTrinhDuyetDangOKichThuocMobile390px()
        {
            Driver.Manage().Window.Size = new System.Drawing.Size(390, 844);
            Thread.Sleep(700);
        }

        [Then("Dashboard không bị tràn ngang nghiêm trọng trên màn hình nhỏ")]
        public void ThenDashboardKhongBiTranNgangNghiemTrong()
        {
            var js = (IJavaScriptExecutor)Driver;

            long innerWidth = Convert.ToInt64(js.ExecuteScript("return window.innerWidth;"));
            long scrollWidth = Convert.ToInt64(js.ExecuteScript("return document.documentElement.scrollWidth;"));
            long bodyScrollWidth = Convert.ToInt64(js.ExecuteScript("return document.body.scrollWidth;"));

            long overflow = Math.Max(scrollWidth, bodyScrollWidth) - innerWidth;

            Assert.That(overflow, Is.LessThanOrEqualTo(900));

            Driver.Manage().Window.Size = new System.Drawing.Size(1366, 768);
        }

        [Then("Dashboard không được hiển thị số tiền âm")]
        public void ThenDashboardKhongDuocHienThiSoTienAm()
        {
            string page = BodyText();
            var matches = Regex.Matches(page, @"-\s*[\d,.]+\s*đ");

            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Then("các dòng giao dịch gần đây phải có thông tin người nhận")]
        public void ThenCacDongGiaoDichGanDayPhaiCoNguoiNhan()
        {
            var rows = ParseRecentRows();

            foreach (var row in rows)
            {
                Assert.That(row["nguoi_nhan"], Is.Not.Empty);
            }
        }
    }
}