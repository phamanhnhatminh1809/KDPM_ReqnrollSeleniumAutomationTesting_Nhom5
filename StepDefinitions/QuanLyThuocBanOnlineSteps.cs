
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks;
using System.Text.RegularExpressions;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.StepDefinitions
{
    [Binding]
    public class QuanLyThuocBanOnlineSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private const string BaseUrl = "http://localhost:44317";
        private const string LoginUrl = "/TaiKhoan/DangNhap";
        private const string AdminOnlineUrl = "/ThuocAdmin/QuanLyThuocBanOnline";

        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";

        private const int TestProductId = 1;
        private const string TestProductName = "Amoxicillin";

        private const int OutOfStockProductId = 2;
        private const string OutOfStockProductName = "Paracetamol";

        private IWebDriver Driver => TestHooks.Driver!;

        public QuanLyThuocBanOnlineSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

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
                ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")!.ToString() == "complete"
            );

            try
            {
                Wait().Until(d =>
                {
                    var result = ((IJavaScriptExecutor)d).ExecuteScript(
                        "return window.jQuery ? jQuery.active === 0 : true;"
                    );

                    return Convert.ToBoolean(result);
                });
            }
            catch
            {
                // Không phải trang nào cũng dùng jQuery, bỏ qua nếu không có.
            }
        }

        private string BodyText()
        {
            try
            {
                return Driver.FindElement(By.TagName("body")).Text;
            }
            catch
            {
                return "";
            }
        }

        private int NumberFromText(string text)
        {
            string digits = Regex.Replace(text ?? "", @"\D", "");
            return string.IsNullOrWhiteSpace(digits) ? 0 : int.Parse(digits);
        }

        private string NormalizeText(string text)
        {
            return (text ?? "").Trim().ToLower();
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
                "Object reference not set",
                "The resource cannot be found",
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

        private void AssertNoAlert()
        {
            try
            {
                var alert = Driver.SwitchTo().Alert();
                string alertText = alert.Text;
                alert.Accept();

                Assert.Fail("Xuất hiện alert không mong muốn: " + alertText);
            }
            catch (NoAlertPresentException)
            {
                // Không có alert là đúng.
            }
        }

        private void LoginAdmin()
        {
            Driver.Navigate().GoToUrl(FullUrl(LoginUrl));
            WaitReady();

            string page = BodyText();

            if (page.Contains("Xin chào, Admin", StringComparison.OrdinalIgnoreCase) ||
                page.Contains("Quản lý thuốc bán online", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

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

            loginButton!.Click();
            Thread.Sleep(1200);

            Driver.Navigate().GoToUrl(FullUrl(AdminOnlineUrl));
            WaitReady();

            string pageAfter = BodyText();

            if (pageAfter.Contains("Đăng Nhập") && pageAfter.Contains("Tên đăng nhập"))
            {
                Assert.Fail("Đăng nhập Admin thất bại, vẫn còn ở trang đăng nhập");
            }
        }

        private string BuildAdminOnlineUrl(string? search = null, string? categoryId = null)
        {
            var query = new List<string>();

            if (search != null)
            {
                query.Add("searchString=" + Uri.EscapeDataString(search));
            }

            if (categoryId != null)
            {
                query.Add("categoryId=" + Uri.EscapeDataString(categoryId));
            }

            string url = AdminOnlineUrl;

            if (query.Count > 0)
            {
                url += "?" + string.Join("&", query);
            }

            return FullUrl(url);
        }

        private void OpenAdminOnline(string? search = null, string? categoryId = null)
        {
            string url = BuildAdminOnlineUrl(search, categoryId);

            Driver.Navigate().GoToUrl(url);
            WaitReady();

            string page = BodyText();

            if (page.Contains("Đăng Nhập") && page.Contains("Tên đăng nhập"))
            {
                LoginAdmin();
                Driver.Navigate().GoToUrl(url);
                WaitReady();
            }

            AssertNoServerError();
        }

        private List<Dictionary<string, object>> ParseAdminRows()
        {
            var rows = Driver.FindElements(By.CssSelector("table tbody tr"));
            var data = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.CssSelector("td"));

                if (cells.Count < 7)
                {
                    continue;
                }

                var selects = row.FindElements(By.CssSelector("select.status-select"));

                string statusValue = selects.Count > 0 ? selects[0].GetAttribute("value") ?? "" : "";
                string productId = selects.Count > 0 ? selects[0].GetAttribute("data-id") ?? "" : "";

                data.Add(new Dictionary<string, object>
                {
                    ["row"] = row,
                    ["product_id"] = productId,
                    ["image_text"] = cells[0].Text.Trim(),
                    ["name_text"] = cells[1].Text.Trim(),
                    ["category"] = cells[2].Text.Trim(),
                    ["stock"] = cells[3].Text.Trim(),
                    ["price"] = cells[4].Text.Trim(),
                    ["batch_status"] = cells[5].Text.Trim(),
                    ["online_status_value"] = statusValue,
                    ["row_text"] = row.Text.Trim()
                });
            }

            return data;
        }

        private IWebElement GetOnlineSelectByProduct(int productId)
        {
            return Wait().Until(d => d.FindElement(By.CssSelector($"select.status-select[data-id='{productId}']")));
        }

        private Dictionary<string, object> ExecuteAjaxUpdateOnlineStatus(int productId, int status)
        {
            string script = @"
                const productId = arguments[0];
                const statusVal = arguments[1];
                const done = arguments[arguments.length - 1];

                fetch('/ThuocAdmin/UpdateTrangThaiBan', {
                    method: 'POST',
                    credentials: 'same-origin',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: new URLSearchParams({ id: productId, status: statusVal })
                })
                .then(async r => {
                    const txt = await r.text();
                    let json = null;
                    try { json = JSON.parse(txt); } catch(e) {}
                    done({
                        ok: r.ok,
                        status: r.status,
                        json: json,
                        text: txt.substring(0, 500)
                    });
                })
                .catch(e => done({
                    ok: false,
                    error: String(e)
                }));
            ";

            object result = ((IJavaScriptExecutor)Driver).ExecuteAsyncScript(script, productId, status.ToString());

            if (result is Dictionary<string, object> dict)
            {
                return dict;
            }

            return new Dictionary<string, object>
            {
                ["raw"] = result?.ToString() ?? ""
            };
        }

        private Dictionary<string, object> ExecuteAjaxAddToCart(int productId)
        {
            string script = @"
                const productId = arguments[0];
                const done = arguments[arguments.length - 1];

                fetch('/GioHang/ThemGioHangAjax', {
                    method: 'POST',
                    credentials: 'same-origin',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: new URLSearchParams({ id: productId })
                })
                .then(async r => {
                    const txt = await r.text();
                    let json = null;
                    try { json = JSON.parse(txt); } catch(e) {}
                    done({
                        ok: r.ok,
                        status: r.status,
                        json: json,
                        text: txt.substring(0, 500)
                    });
                })
                .catch(e => done({
                    ok: false,
                    error: String(e)
                }));
            ";

            object result = ((IJavaScriptExecutor)Driver).ExecuteAsyncScript(script, productId);

            if (result is Dictionary<string, object> dict)
            {
                return dict;
            }

            return new Dictionary<string, object>
            {
                ["raw"] = result?.ToString() ?? ""
            };
        }

        private bool? GetSuccessFromAjaxResult(Dictionary<string, object> result)
        {
            if (!result.TryGetValue("json", out object? jsonObj) || jsonObj == null)
            {
                return null;
            }

            if (jsonObj is Dictionary<string, object> jsonDict &&
                jsonDict.TryGetValue("success", out object? successObj))
            {
                if (successObj is bool b)
                {
                    return b;
                }

                if (bool.TryParse(successObj?.ToString(), out bool parsed))
                {
                    return parsed;
                }
            }

            return null;
        }

        private void SetOnlineStatusAndVerify(int productId, int status)
        {
            OpenAdminOnline();

            var result = ExecuteAjaxUpdateOnlineStatus(productId, status);

            if (result.TryGetValue("ok", out object? okObj) && okObj is bool ok && !ok)
            {
                Assert.Fail("Request đổi trạng thái thất bại: " + result);
            }

            bool? success = GetSuccessFromAjaxResult(result);

            if (success == false)
            {
                Assert.Fail("Backend trả success=false khi đổi trạng thái: " + result);
            }

            Thread.Sleep(800);

            OpenAdminOnline();

            var select = GetOnlineSelectByProduct(productId);
            string currentValue = select.GetAttribute("value") ?? "";

            if (currentValue != status.ToString())
            {
                Assert.Fail($"Trạng thái sau reload không đúng. Expected={status}, Actual={currentValue}");
            }
        }

        private Dictionary<string, string> GetProductAreaOnUserPage(string productName)
        {
            string script = @"
                const productName = arguments[0].toLowerCase();

                function visibleText(el) {
                    return (el.innerText || el.textContent || '').trim();
                }

                const all = Array.from(document.querySelectorAll('body *'));
                const matches = all.filter(el => visibleText(el).toLowerCase().includes(productName));

                if (matches.length === 0) {
                    return {
                        found: 'false',
                        text: document.body.innerText || '',
                        html: document.body.innerHTML || ''
                    };
                }

                let el = matches[0];
                let container = el.closest('.card, .product-card, .product-item, .col-md-3, .col-md-4, .col-lg-3, .col-lg-4, tr, .row') || el;

                return {
                    found: 'true',
                    text: visibleText(container),
                    html: container.outerHTML || ''
                };
            ";

            object result = ((IJavaScriptExecutor)Driver).ExecuteScript(script, productName);

            if (result is Dictionary<string, object> dictObj)
            {
                return dictObj.ToDictionary(
                    x => x.Key,
                    x => x.Value?.ToString() ?? ""
                );
            }

            return new Dictionary<string, string>
            {
                ["found"] = "false",
                ["text"] = BodyText(),
                ["html"] = ""
            };
        }

        private bool UserAreaHasBuyOnline(Dictionary<string, string> area)
        {
            string text = NormalizeText(area.GetValueOrDefault("text", ""));
            string html = NormalizeText(area.GetValueOrDefault("html", ""));

            string[] keywords =
            {
                "mua ngay",
                "thêm vào giỏ",
                "thêm giỏ",
                "addtocart",
                "themgiohang"
            };

            return keywords.Any(k => text.Contains(k) || html.Contains(k));
        }

        private bool UserAreaHasBuyAtStore(Dictionary<string, string> area)
        {
            string text = NormalizeText(area.GetValueOrDefault("text", ""));
            string html = NormalizeText(area.GetValueOrDefault("html", ""));

            string[] keywords =
            {
                "mua tại cửa hàng",
                "mua tại nhà thuốc",
                "liên hệ",
                "không bán online"
            };

            return keywords.Any(k => text.Contains(k) || html.Contains(k));
        }

        private void OpenUserProductSearch(string productName)
        {
            string query = Uri.EscapeDataString(productName);

            Driver.Navigate().GoToUrl(FullUrl("/TrangChu/TimKiem?query=" + query));
            WaitReady();

            if (!BodyText().Contains(productName, StringComparison.OrdinalIgnoreCase))
            {
                Driver.Navigate().GoToUrl(FullUrl("/DanhSach/DanhSachThuoc/0"));
                WaitReady();
            }
        }

        private void RestoreAmoxicillinOnlineStatus()
        {
            try
            {
                SetOnlineStatusAndVerify(TestProductId, 1);
            }
            catch
            {
                // Không để lỗi khôi phục trạng thái làm che lỗi test chính.
            }
        }

        [When("Admin truy cập trang Quản lý thuốc bán online")]
        public void WhenAdminTruyCapTrangQuanLyThuocBanOnline()
        {
            LoginAdmin();
            OpenAdminOnline();
        }

        [Then("trang Quản lý thuốc bán online mở được và không hiển thị lỗi hệ thống")]
        public void ThenTrangQuanLyThuocBanOnlineMoDuoc()
        {
            AssertNoServerError();
        }

        [Then("trang hiển thị tiêu đề Quản lý thuốc bán online")]
        public void ThenTrangHienThiTieuDeQuanLyThuocBanOnline()
        {
            Assert.That(BodyText(), Does.Contain("Quản lý thuốc bán online").Or.Contain("QuanLyThuocBanOnline"));
        }

        [Then("trang hiển thị cột hoặc trạng thái Cho phép bán online")]
        public void ThenTrangHienThiCotChoPhepBanOnline()
        {
            Assert.That(BodyText(), Does.Contain("Cho phép bán online"));
        }

        [Then("menu Quản lý thuốc bán online đang ở trạng thái active")]
        public void ThenMenuQuanLyThuocBanOnlineActive()
        {
            var active = Driver.FindElements(By.CssSelector(".sidebar a.active"));

            Assert.That(active.Count, Is.GreaterThan(0), "Không có menu active trong sidebar");
            Assert.That(active[0].Text, Does.Contain("Quản lý thuốc bán online"));
        }

        [Then("bảng thuốc online hiển thị đủ các cột cần thiết")]
        public void ThenBangThuocOnlineHienThiDuCot()
        {
            string page = BodyText();

            string[] requiredHeaders =
            {
                "Ảnh",
                "Tên thuốc",
                "Danh mục",
                "Tồn kho",
                "Giá bán",
                "Trạng thái",
                "Cho phép bán online"
            };

            foreach (string header in requiredHeaders)
            {
                Assert.That(page, Does.Contain(header), "Bảng thiếu cột: " + header);
            }
        }

        [Then("bảng thuốc online có dữ liệu thuốc hợp lệ")]
        public void ThenBangThuocOnlineCoDuLieuHopLe()
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.GreaterThan(0), "Bảng thuốc online không có dữ liệu");

            foreach (var item in rows.Take(10))
            {
                Assert.That(item["product_id"].ToString(), Is.Not.Empty);
                Assert.That(item["name_text"].ToString(), Is.Not.Empty);
                Assert.That(item["category"].ToString(), Is.Not.Empty);

                int price = NumberFromText(item["price"].ToString() ?? "");
                Assert.That(price, Is.GreaterThan(0));
            }
        }

        [When(@"Admin tìm kiếm thuốc online với từ khóa ""(.*)""")]
        public void WhenAdminTimKiemThuocOnlineVoiTuKhoa(string keyword)
        {
            LoginAdmin();
            OpenAdminOnline(search: keyword);
            _scenarioContext["LastSearchKeyword"] = keyword;
        }

        [When(@"Admin tìm kiếm thuốc online với chuỗi đặc biệt ""(.*)""")]
        public void WhenAdminTimKiemThuocOnlineVoiChuoiDacBiet(string keyword)
        {
            LoginAdmin();
            OpenAdminOnline(search: keyword);
            _scenarioContext["LastSearchKeyword"] = keyword;
        }

        [Then(@"kết quả tìm kiếm chỉ hiển thị thuốc phù hợp với từ khóa ""(.*)""")]
        public void ThenKetQuaTimKiemChiHienThiThuocPhuHop(string keyword)
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.GreaterThan(0), "Tìm kiếm không có kết quả");

            foreach (var item in rows)
            {
                string rowText = item["row_text"].ToString() ?? "";

                Assert.That(rowText.ToLower(), Does.Contain(keyword.ToLower()));
            }
        }

        [Then("kết quả tìm kiếm có thuốc Amoxicillin")]
        public void ThenKetQuaTimKiemCoThuocAmoxicillin()
        {
            var rows = ParseAdminRows();

            bool found = rows.Any(item =>
                (item["row_text"].ToString() ?? "").Contains("Amoxicillin", StringComparison.OrdinalIgnoreCase)
            );

            Assert.That(found, Is.True, "Không tìm thấy Amoxicillin trong kết quả tìm kiếm");
        }

        [Then("danh sách kết quả tìm kiếm không hiển thị thuốc không liên quan")]
        public void ThenDanhSachKetQuaTimKiemKhongHienThiThuocKhongLienQuan()
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.EqualTo(0), "Tìm từ khóa không tồn tại nhưng vẫn có kết quả");
        }

        [Then("hệ thống không thực thi script")]
        public void ThenHeThongKhongThucThiScript()
        {
            AssertNoAlert();
        }

        [Then("hệ thống không hiển thị lỗi Server Error")]
        public void ThenHeThongKhongHienThiLoiServerError()
        {
            AssertNoServerError();
        }

        [When(@"Admin lọc thuốc online theo danh mục (.*)")]
        public void WhenAdminLocThuocOnlineTheoDanhMuc(string categoryName)
        {
            LoginAdmin();

            string categoryId = categoryName.Trim() switch
            {
                "Kháng sinh" => "1",
                "Giảm đau Hạ sốt" => "2",
                "Kháng viêm" => "3",
                "Vitamin và khoáng chất" => "4",
                _ => ""
            };

            OpenAdminOnline(categoryId: categoryId);
            _scenarioContext["LastCategoryName"] = categoryName.Trim();
        }

        [Then(@"danh sách chỉ hiển thị thuốc thuộc danh mục (.*)")]
        public void ThenDanhSachChiHienThiThuocThuocDanhMuc(string categoryName)
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.GreaterThan(0), "Lọc danh mục không có kết quả");

            foreach (var item in rows)
            {
                string category = item["category"].ToString() ?? "";

                bool isValid = categoryName.Trim() switch
                {
                    "Kháng sinh" => category.Contains("Kháng sinh"),
                    "Giảm đau Hạ sốt" => category.Contains("Giảm đau") || category.Contains("Hạ sốt"),
                    "Kháng viêm" => category.Contains("Kháng viêm"),
                    "Vitamin và khoáng chất" => category.Contains("Vitamin") || category.ToLower().Contains("khoáng chất"),
                    _ => false
                };

                Assert.That(isValid, Is.True, "Có dòng sai danh mục: " + item["row_text"]);
            }
        }

        [When("Admin chọn bộ lọc tất cả danh mục thuốc online")]
        public void WhenAdminChonBoLocTatCaDanhMuc()
        {
            LoginAdmin();
            OpenAdminOnline(categoryId: "");
        }

        [Then("hệ thống hiển thị lại danh sách thuốc online")]
        public void ThenHeThongHienThiLaiDanhSachThuocOnline()
        {
            var rows = ParseAdminRows();
            Assert.That(rows.Count, Is.GreaterThan(0));
        }

        [When(@"Admin tìm kiếm thuốc ""(.*)"" trong danh mục Kháng sinh")]
        public void WhenAdminTimKiemThuocTrongDanhMucKhangSinh(string productName)
        {
            LoginAdmin();
            OpenAdminOnline(search: productName, categoryId: "1");
        }

        [When(@"Admin tìm kiếm thuốc ""(.*)"" trong danh mục Giảm đau Hạ sốt")]
        public void WhenAdminTimKiemThuocTrongDanhMucGiamDauHaSot(string productName)
        {
            LoginAdmin();
            OpenAdminOnline(search: productName, categoryId: "2");
        }

        [Then("danh sách hiển thị thuốc thỏa cả tên thuốc và danh mục")]
        public void ThenDanhSachHienThiThuocThoaCaTenVaDanhMuc()
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.GreaterThan(0));

            bool found = false;

            foreach (var item in rows)
            {
                string rowText = item["row_text"].ToString() ?? "";
                string category = item["category"].ToString() ?? "";

                if (rowText.Contains("Amoxicillin", StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                }

                Assert.That(category, Does.Contain("Kháng sinh"));
            }

            Assert.That(found, Is.True, "Không tìm thấy Amoxicillin");
        }

        [Then("danh sách không hiển thị thuốc sai danh mục")]
        public void ThenDanhSachKhongHienThiThuocSaiDanhMuc()
        {
            var rows = ParseAdminRows();
            Assert.That(rows.Count, Is.EqualTo(0));
        }

        [When("Admin lọc thuốc online với categoryId không hợp lệ")]
        public void WhenAdminLocThuocOnlineVoiCategoryIdKhongHopLe()
        {
            LoginAdmin();
            OpenAdminOnline(categoryId: "999");
        }

        [Then("mỗi dòng thuốc có combobox trạng thái bán online")]
        public void ThenMoiDongThuocCoComboboxTrangThaiBanOnline()
        {
            var rows = ParseAdminRows();

            Assert.That(rows.Count, Is.GreaterThan(0));

            foreach (var item in rows.Take(10))
            {
                var row = (IWebElement)item["row"];
                var selects = row.FindElements(By.CssSelector("select.status-select"));

                Assert.That(selects.Count, Is.GreaterThan(0), "Dòng thuốc thiếu combobox trạng thái");
            }
        }

        [Then("combobox có đủ lựa chọn Cho phép bán online và Không cho phép bán online")]
        public void ThenComboboxCoDuLuaChon()
        {
            var rows = ParseAdminRows();

            foreach (var item in rows.Take(10))
            {
                var row = (IWebElement)item["row"];
                var select = row.FindElement(By.CssSelector("select.status-select"));
                var options = select.FindElements(By.CssSelector("option"));

                string values = string.Join(" ", options.Select(o => o.GetAttribute("value")));
                string text = string.Join(" ", options.Select(o => o.Text));

                Assert.That(values, Does.Contain("1"));
                Assert.That(values, Does.Contain("0"));
                Assert.That(text, Does.Contain("Cho phép bán online"));
                Assert.That(text, Does.Contain("Không cho phép bán online"));
            }
        }

        [When("Admin tắt bán online cho thuốc Amoxicillin")]
        public void WhenAdminTatBanOnlineChoThuocAmoxicillin()
        {
            LoginAdmin();
            SetOnlineStatusAndVerify(TestProductId, 0);
        }

        [When("Admin bật bán online cho thuốc Amoxicillin")]
        public void WhenAdminBatBanOnlineChoThuocAmoxicillin()
        {
            LoginAdmin();
            SetOnlineStatusAndVerify(TestProductId, 1);
        }

        [Then("trạng thái Không cho phép bán online được lưu sau khi reload")]
        public void ThenTrangThaiKhongChoPhepBanOnlineDuocLuu()
        {
            try
            {
                OpenAdminOnline();
                var select = GetOnlineSelectByProduct(TestProductId);
                string value = select.GetAttribute("value") ?? "";

                Assert.That(value, Is.EqualTo("0"));
            }
            finally
            {
                RestoreAmoxicillinOnlineStatus();
            }
        }

        [Then("trạng thái Cho phép bán online được lưu sau khi reload")]
        public void ThenTrangThaiChoPhepBanOnlineDuocLuu()
        {
            OpenAdminOnline();
            var select = GetOnlineSelectByProduct(TestProductId);
            string value = select.GetAttribute("value") ?? "";

            Assert.That(value, Is.EqualTo("1"));
        }

        [Then("trạng thái bật bán online có value bằng 1")]
        public void ThenTrangThaiBatBanOnlineCoValueBang1()
        {
            OpenAdminOnline();
            var select = GetOnlineSelectByProduct(TestProductId);

            Assert.That(select.GetAttribute("value"), Is.EqualTo("1"));
        }

        [Then("trạng thái tắt bán online có value bằng 0")]
        public void ThenTrangThaiTatBanOnlineCoValueBang0()
        {
            OpenAdminOnline();
            var select = GetOnlineSelectByProduct(TestProductId);

            Assert.That(select.GetAttribute("value"), Is.EqualTo("0"));
        }

        [Then("combobox trạng thái có style hiển thị")]
        public void ThenComboboxTrangThaiCoStyleHienThi()
        {
            try
            {
                var select = GetOnlineSelectByProduct(TestProductId);
                // Sửa ValueOfCssProperty thành GetCssValue
                string bg = select.GetCssValue("background-color");

                Assert.That(bg, Is.Not.Empty);
            }
            finally
            {
                try
                {
                    var select = GetOnlineSelectByProduct(TestProductId);
                    string value = select.GetAttribute("value") ?? "";

                    if (value == "0")
                    {
                        RestoreAmoxicillinOnlineStatus();
                    }
                }
                catch
                {
                    RestoreAmoxicillinOnlineStatus();
                }
            }
        }

        [When("User tìm kiếm thuốc Amoxicillin trên giao diện người dùng")]
        public void WhenUserTimKiemThuocAmoxicillinTrenGiaoDienNguoiDung()
        {
            OpenUserProductSearch(TestProductName);
        }

        [Then("giao diện User hiển thị nút mua hoặc thêm giỏ cho thuốc Amoxicillin")]
        public void ThenGiaoDienUserHienThiNutMuaHoacThemGio()
        {
            var area = GetProductAreaOnUserPage(TestProductName);

            Assert.That(area["found"], Is.EqualTo("true"), "Không tìm thấy Amoxicillin trên giao diện user");
            Assert.That(UserAreaHasBuyOnline(area), Is.True, "Không thấy nút mua/thêm giỏ");
        }

        [Then("giao diện User hiển thị trạng thái mua tại cửa hàng hoặc không bán online cho thuốc Amoxicillin")]
        public void ThenGiaoDienUserHienThiMuaTaiCuaHang()
        {
            try
            {
                var area = GetProductAreaOnUserPage(TestProductName);

                Assert.That(area["found"], Is.EqualTo("true"), "Không tìm thấy Amoxicillin trên giao diện user");

                bool hasBuyOnline = UserAreaHasBuyOnline(area);
                bool hasBuyAtStore = UserAreaHasBuyAtStore(area);

                if (hasBuyOnline && !hasBuyAtStore)
                {
                    Assert.Fail("Thuốc đã tắt bán online nhưng giao diện user vẫn có nút mua online");
                }

                Assert.That(hasBuyAtStore, Is.True, "Không thấy trạng thái mua tại cửa hàng/liên hệ/không bán online");
            }
            finally
            {
                RestoreAmoxicillinOnlineStatus();
            }
        }

        [When("User gửi request Ajax thêm thuốc Amoxicillin vào giỏ hàng")]
        public void WhenUserGuiRequestAjaxThemThuocAmoxicillinVaoGioHang()
        {
            var result = ExecuteAjaxAddToCart(TestProductId);
            _scenarioContext["AddCartResult"] = result;
        }

        [Then("backend phải từ chối thêm thuốc đã tắt bán online vào giỏ")]
        public void ThenBackendPhaiTuChoiThemThuocTatOnline()
        {
            try
            {
                var result = (Dictionary<string, object>)_scenarioContext["AddCartResult"];
                bool? success = GetSuccessFromAjaxResult(result);

                Assert.That(success, Is.Not.True, "Thuốc đã tắt bán online nhưng backend vẫn cho thêm vào giỏ");
            }
            finally
            {
                RestoreAmoxicillinOnlineStatus();
            }
        }

        [Then("backend cho phép thêm thuốc đã bật bán online vào giỏ")]
        public void ThenBackendChoPhepThemThuocBatOnline()
        {
            var result = (Dictionary<string, object>)_scenarioContext["AddCartResult"];
            bool? success = GetSuccessFromAjaxResult(result);

            Assert.That(success, Is.True, "Thuốc đã bật bán online nhưng backend không cho thêm vào giỏ");
        }

        [Then("thuốc Paracetamol hiển thị trạng thái Hết hàng trong trang Admin")]
        public void ThenThuocParacetamolHienThiHetHang()
        {
            var rows = ParseAdminRows();

            var row = rows.FirstOrDefault(item =>
                item["product_id"].ToString() == OutOfStockProductId.ToString() ||
                (item["row_text"].ToString() ?? "").Contains(OutOfStockProductName, StringComparison.OrdinalIgnoreCase)
            );

            Assert.That(row, Is.Not.Null, "Không tìm thấy Paracetamol");

            string rowText = row!["row_text"].ToString() ?? "";
            Assert.That(rowText, Does.Contain("Hết hàng"));
        }

        [Then("thuốc Amoxicillin có cảnh báo lô sắp hết hạn")]
        public void ThenThuocAmoxicillinCoCanhBaoLoSapHetHan()
        {
            var rows = ParseAdminRows();

            var item = rows.FirstOrDefault(x =>
                (x["row_text"].ToString() ?? "").Contains("Amoxicillin", StringComparison.OrdinalIgnoreCase)
            );

            Assert.That(item, Is.Not.Null, "Không tìm thấy Amoxicillin");

            var row = (IWebElement)item!["row"];
            var warningIcons = row.FindElements(By.CssSelector(".bi-exclamation-triangle-fill, .text-warning, [title]"));

            Assert.That(warningIcons.Count, Is.GreaterThan(0), "Không thấy cảnh báo lô sắp hết hạn");
        }

        [Then("ảnh thuốc trong bảng có đường dẫn src")]
        public void ThenAnhThuocTrongBangCoDuongDanSrc()
        {
            var images = Driver.FindElements(By.CssSelector("table tbody img"));

            Assert.That(images.Count, Is.GreaterThan(0), "Bảng thuốc không có hình ảnh");

            foreach (var img in images.Take(15))
            {
                string src = img.GetAttribute("src") ?? "";
                Assert.That(src, Is.Not.Empty, "Có ảnh thiếu src");
            }
        }

        [Then("ảnh thuốc trong bảng không bị vỡ hoặc không tải được")]
        public void ThenAnhThuocTrongBangKhongBiVo()
        {
            object result = ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                const imgs = Array.from(document.querySelectorAll('table tbody img'));
                return imgs
                    .filter(img => img.complete && img.naturalWidth === 0)
                    .map(img => img.getAttribute('src'));
            ");

            var broken = new List<string>();

            if (result is IEnumerable<object> list)
            {
                broken = list.Select(x => x?.ToString() ?? "").Where(x => x != "").ToList();
            }

            Assert.That(broken.Count, Is.EqualTo(0), "Có ảnh thuốc bị lỗi: " + string.Join(", ", broken));
        }

        [Then("giá bán trong bảng là số dương")]
        public void ThenGiaBanTrongBangLaSoDuong()
        {
            var rows = ParseAdminRows();

            foreach (var item in rows.Take(20))
            {
                int priceValue = NumberFromText(item["price"].ToString() ?? "");
                Assert.That(priceValue, Is.GreaterThan(0), "Giá bán không hợp lệ: " + item["row_text"]);
            }
        }

        [Then("giá bán trong bảng có ký hiệu đ")]
        public void ThenGiaBanTrongBangCoKyHieuDong()
        {
            var rows = ParseAdminRows();

            foreach (var item in rows.Take(20))
            {
                string price = item["price"].ToString() ?? "";
                string rowText = item["row_text"].ToString() ?? "";

                Assert.That(price + rowText, Does.Contain("đ"));
            }
        }

        [Then("tồn kho trong bảng là số không âm hoặc nhãn Hết hàng")]
        public void ThenTonKhoTrongBangLaSoKhongAmHoacHetHang()
        {
            var rows = ParseAdminRows();

            foreach (var item in rows.Take(20))
            {
                string stockText = item["stock"].ToString() ?? "";

                if (stockText.Contains("Hết hàng"))
                {
                    continue;
                }

                Assert.That(stockText.Any(char.IsDigit), Is.True, "Tồn kho không phải số hoặc nhãn Hết hàng");
                Assert.That(NumberFromText(stockText), Is.GreaterThanOrEqualTo(0));
            }
        }

        [Then("cảnh báo lô sắp hết hạn có tooltip mô tả đúng")]
        public void ThenCanhBaoLoSapHetHanCoTooltipDung()
        {
            var rows = ParseAdminRows();

            var item = rows.FirstOrDefault(x =>
                (x["row_text"].ToString() ?? "").Contains("Amoxicillin", StringComparison.OrdinalIgnoreCase)
            );

            Assert.That(item, Is.Not.Null, "Không tìm thấy Amoxicillin");

            var row = (IWebElement)item!["row"];
            var titleElements = row.FindElements(By.CssSelector("[title]"));

            var titles = titleElements
                .Select(x => x.GetAttribute("title") ?? "")
                .Where(x => x != "")
                .ToList();

            Assert.That(titles.Count, Is.GreaterThan(0), "Không có tooltip title");
            Assert.That(titles.Any(t => t.ToLower().Contains("sắp hết hạn")), Is.True, "Tooltip không đúng nội dung");
        }

        [When("Admin thực hiện nhiều điều kiện tìm kiếm và lọc thuốc online liên tục")]
        public void WhenAdminThucHienNhieuDieuKienTimKiemLocLienTuc()
        {
            LoginAdmin();

            var testCases = new List<(string? Search, string? CategoryId)>
            {
                (null, null),
                ("Amoxicillin", null),
                ("Paracetamol", null),
                ("abcxyzkhongtontai", null),
                (null, "1"),
                (null, "2"),
                (null, "3"),
                (null, "4"),
                ("Amoxicillin", "1"),
                ("Amoxicillin", "2"),
                ("<script>alert(1)</script>", null)
            };

            foreach (var item in testCases)
            {
                OpenAdminOnline(search: item.Search, categoryId: item.CategoryId);
                AssertNoServerError();
            }
        }

        [Then("không điều kiện nào gây lỗi Server Error")]
        public void ThenKhongDieuKienNaoGayLoiServerError()
        {
            AssertNoServerError();
        }

        [Then("trang Quản lý thuốc online không bị tràn ngang nghiêm trọng trên mobile")]
        public void ThenTrangQuanLyThuocOnlineKhongBiTranNgangMobile()
        {
            OpenAdminOnline();

            var js = (IJavaScriptExecutor)Driver;

            long innerWidth = Convert.ToInt64(js.ExecuteScript("return window.innerWidth;"));
            long scrollWidth = Convert.ToInt64(js.ExecuteScript("return document.documentElement.scrollWidth;"));
            long bodyScrollWidth = Convert.ToInt64(js.ExecuteScript("return document.body.scrollWidth;"));

            long overflow = Math.Max(scrollWidth, bodyScrollWidth) - innerWidth;

            Assert.That(overflow, Is.LessThanOrEqualTo(1200));

            Driver.Manage().Window.Size = new System.Drawing.Size(1366, 768);
        }
    }
}

