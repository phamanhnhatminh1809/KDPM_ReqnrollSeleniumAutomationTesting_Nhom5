using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.Hooks;
using System.Text.RegularExpressions;

namespace KDPM_ReqnrollSeleniumAutomationTesting_Nhom5.StepDefinitions
{
    [Binding]
    public class GioHangSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private const string BaseUrl = "http://localhost:44317";

        private const int ProductId1 = 1; // Amoxicillin, còn hàng
        private const int ProductId2 = 4; // Panadol Extra, còn hàng
        private const int ProductId3 = 8; // Alpha Choay, còn hàng
        private const int OutOfStockProductId = 2; // Paracetamol, hết hàng

        private const string TestName = "Nguyen Van A";
        private const string TestNameSpace = "   ";
        private const string TestPhoneValid = "0912345678";
        private const string TestPhoneInvalid = "12345";
        private const string TestEmailValid = "test@gmail.com";
        private const string TestEmailInvalid = "abcgmail.com";
        private const string TestAddress = "123 Le Loi";
        private const string TestXssName = "<script>alert('xss')</script>";
        private const string TestSqlAddress = "'; DROP TABLE GioHang;--";

        private IWebDriver Driver => TestHooks.Driver!;

        public GioHangSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        private string FullUrl(string path)
        {
            return BaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        private WebDriverWait Wait(int seconds = 10)
        {
            return new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
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
                    object? result = ((IJavaScriptExecutor)d).ExecuteScript(
                        "return window.jQuery ? jQuery.active === 0 : true;"
                    );

                    return Convert.ToBoolean(result);
                });
            }
            catch
            {
                // Không phải trang nào cũng có jQuery.
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

        private void AssertNoServerError()
        {
            string text = BodyText();
            string title = Driver.Title ?? "";

            string[] errorKeywords =
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

            foreach (string key in errorKeywords)
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

        private void OpenHome()
        {
            Driver.Navigate().GoToUrl(FullUrl("/"));
            WaitReady();
            Wait().Until(d => d.FindElement(By.TagName("body")));
        }

        private void OpenCart()
        {
            Driver.Navigate().GoToUrl(FullUrl("/GioHang/XemGioHang"));
            WaitReady();
            Wait().Until(d => d.FindElement(By.TagName("body")));
        }

        private IWebElement WaitVisibleCss(string css, int seconds = 10)
        {
            return new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds))
                .Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(css));
                    return el.Displayed ? el : null;
                })!;
        }

        private void SafeClick(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "arguments[0].scrollIntoView({block:'center'});",
                element
            );

            Thread.Sleep(150);

            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "arguments[0].click();",
                element
            );
        }

        private void ClickCss(string css)
        {
            var el = WaitVisibleCss(css);
            SafeClick(el);
        }

        private string GetText(string css)
        {
            try
            {
                return Driver.FindElement(By.CssSelector(css)).Text.Trim();
            }
            catch
            {
                return "";
            }
        }

        private IWebElement GetQtyInput(int productId)
        {
            return Driver.FindElement(By.CssSelector($"#qty-{productId}"));
        }

        private int GetQtyValue(int productId)
        {
            try
            {
                return int.Parse(GetQtyInput(productId).GetAttribute("value") ?? "0");
            }
            catch
            {
                return 0;
            }
        }

        private bool HasProductInCart(int productId)
        {
            return Driver.FindElements(By.CssSelector($"#qty-{productId}")).Count > 0;
        }

        private int GetCartTotal()
        {
            var totals = Driver.FindElements(By.CssSelector(".cart-total-display"));

            if (totals.Count == 0)
            {
                return 0;
            }

            return NumberFromText(totals.Last().Text);
        }

        private int GetItemTotal(int productId)
        {
            try
            {
                return NumberFromText(
                    Driver.FindElement(By.CssSelector($"#total-price-{productId}")).Text
                );
            }
            catch
            {
                return 0;
            }
        }

        private List<int> GetAllItemTotals()
        {
            var totals = new List<int>();

            foreach (var el in Driver.FindElements(By.CssSelector("[id^='total-price-']")))
            {
                int value = NumberFromText(el.Text);

                if (value > 0)
                {
                    totals.Add(value);
                }
            }

            return totals;
        }

        private List<int> GetCartProductIds()
        {
            var ids = new List<int>();

            foreach (var el in Driver.FindElements(By.CssSelector("input.input-qty[id^='qty-']")))
            {
                string raw = el.GetAttribute("id") ?? "";
                var match = Regex.Match(raw, @"qty-(\d+)");

                if (match.Success)
                {
                    ids.Add(int.Parse(match.Groups[1].Value));
                }
            }

            return ids;
        }

        private Dictionary<string, object> AddToCartAjax(int productId)
        {
            OpenHome();

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

            object result = ((IJavaScriptExecutor)Driver).ExecuteAsyncScript(script, productId.ToString());
            Thread.Sleep(700);

            if (result is Dictionary<string, object> dict)
            {
                return dict;
            }

            return new Dictionary<string, object>
            {
                ["raw"] = result?.ToString() ?? ""
            };
        }

        private Dictionary<string, object> UpdateQtyAjax(int productId, int newQty)
        {
            OpenCart();

            string script = @"
                const productId = arguments[0];
                const newQty = arguments[1];
                const done = arguments[arguments.length - 1];

                fetch('/GioHang/CapNhatSoLuongAjax', {
                    method: 'POST',
                    credentials: 'same-origin',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: JSON.stringify({ id: productId, soLuong: newQty })
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

            object result = ((IJavaScriptExecutor)Driver).ExecuteAsyncScript(script, productId, newQty);
            Thread.Sleep(700);

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

        private void EnsureProductInCart(int productId = ProductId1)
        {
            AddToCartAjax(productId);
            OpenCart();

            if (!HasProductInCart(productId))
            {
                Assert.Fail($"Không thấy thuốc id={productId} trong giỏ hàng sau khi thêm.");
            }
        }

        private Dictionary<string, object> LoadMiniCart()
        {
            OpenHome();

            string script = @"
                const done = arguments[arguments.length - 1];

                if (!window.jQuery) {
                    done({ ok: false, message: 'Không có jQuery' });
                    return;
                }

                $.get('/GioHang/GetMiniCart')
                    .done(function(data) {
                        $('#miniCartContainer').html(data);
                        $('.mini-cart-popup').show();
                        done({ ok: true, length: data.length });
                    })
                    .fail(function(xhr) {
                        done({
                            ok: false,
                            status: xhr.status,
                            text: xhr.responseText?.substring(0, 300)
                        });
                    });
            ";

            object result = ((IJavaScriptExecutor)Driver).ExecuteAsyncScript(script);
            Thread.Sleep(700);

            if (result is Dictionary<string, object> dict)
            {
                return dict;
            }

            return new Dictionary<string, object>
            {
                ["raw"] = result?.ToString() ?? ""
            };
        }

        private string? AcceptAlertIfAny()
        {
            try
            {
                var alert = Driver.SwitchTo().Alert();
                string msg = alert.Text;
                alert.Accept();
                return msg;
            }
            catch (NoAlertPresentException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void ClearCustomerForm()
        {
            string[] cssList =
            {
                "#txtHoTen",
                "#txtSoDienThoai",
                "#txtEmail",
                "#txtDiaChiCuThe"
            };

            foreach (string css in cssList)
            {
                try
                {
                    var el = Driver.FindElement(By.CssSelector(css));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].value = '';", el);
                }
                catch
                {
                    // Bỏ qua nếu field không tồn tại.
                }
            }
        }

        private void FillBasicInfo(string name = TestName, string phone = TestPhoneValid, string email = "")
        {
            ClearCustomerForm();

            Driver.FindElement(By.CssSelector("#txtHoTen")).SendKeys(name);
            Driver.FindElement(By.CssSelector("#txtSoDienThoai")).SendKeys(phone);

            if (email != null)
            {
                Driver.FindElement(By.CssSelector("#txtEmail")).SendKeys(email);
            }
        }

        private void SelectShippingMethod()
        {
            ClickCss("#GiaoHang");
            Thread.Sleep(500);
        }

        private void SelectPickupMethod()
        {
            ClickCss("#NhanTaiQuay");
            Thread.Sleep(800);
        }

        private void InjectValidShippingData()
        {
            string script = @"
                function setSelect(id, value, text) {
                    let el = document.getElementById(id);
                    if (!el) return false;

                    el.disabled = false;
                    el.innerHTML = '';

                    let opt = new Option(text, value);
                    el.add(opt);
                    el.value = value;

                    el.dispatchEvent(new Event('change', {bubbles:true}));
                    return true;
                }

                setSelect('dataApiTinhThanh', '1', 'TP.HCM');
                setSelect('dataApiQuanHuyen', '1', 'Quận 1');
                setSelect('dataApiPhuongXa', '1', 'Phường Bến Nghé');

                let addr = document.getElementById('txtDiaChiCuThe');
                if (addr) addr.value = arguments[0];

                return true;
            ";

            ((IJavaScriptExecutor)Driver).ExecuteScript(script, TestAddress);
            Thread.Sleep(400);
        }

        private void InjectShippingMissingWard()
        {
            string script = @"
                function setSelect(id, value, text) {
                    let el = document.getElementById(id);
                    if (!el) return false;

                    el.disabled = false;
                    el.innerHTML = '';

                    let opt = new Option(text, value);
                    el.add(opt);
                    el.value = value;

                    el.dispatchEvent(new Event('change', {bubbles:true}));
                    return true;
                }

                setSelect('dataApiTinhThanh', '1', 'TP.HCM');
                setSelect('dataApiQuanHuyen', '1', 'Quận 1');

                let xa = document.getElementById('dataApiPhuongXa');

                if (xa) {
                    xa.disabled = false;
                    xa.innerHTML = '<option value="""">-- Phường/Xã --</option>';
                    xa.value = '';
                }

                let addr = document.getElementById('txtDiaChiCuThe');

                if (addr) addr.value = arguments[0];

                return true;
            ";

            ((IJavaScriptExecutor)Driver).ExecuteScript(script, TestAddress);
            Thread.Sleep(400);
        }

        private void InjectPickupAreaWithoutPharmacy()
        {
            string script = @"
                function setSelect(id, value, text) {
                    let el = document.getElementById(id);
                    if (!el) return false;

                    el.disabled = false;
                    el.innerHTML = '';

                    let opt0 = new Option('-- Chọn --', '');
                    let opt1 = new Option(text, value);

                    el.add(opt0);
                    el.add(opt1);
                    el.value = value;

                    el.dispatchEvent(new Event('change', {bubbles:true}));
                    return true;
                }

                setSelect('ddlTinh_Pickup', 'Hà Nội', 'Hà Nội');
                setSelect('ddlQuan_Pickup', 'Quận Hoàn Kiếm', 'Quận Hoàn Kiếm');

                let selected = document.getElementById('selectedPharmacyId');

                if (selected) {
                    selected.disabled = false;
                    selected.value = '';
                }

                let list = document.getElementById('listNhaThuocArea');

                if (list) {
                    list.innerHTML = `
                        <label class='pharmacy-card d-block bg-white p-3 mb-2 border rounded'>
                            <input type='radio' name='pharmacyRadioFake' value='1'>
                            <span class='fw-bold text-primary'>Nhà thuốc MedForAll Quận Hoàn Kiếm</span>
                        </label>
                    `;
                }

                return true;
            ";

            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(500);
        }

        private void ClickPlaceOrder()
        {
            ClickCss("#btnPlaceOrder");
            Thread.Sleep(800);
            WaitReady();
        }

        private void DoubleClickPlaceOrder()
        {
            var btn = WaitVisibleCss("#btnPlaceOrder");

            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "arguments[0].scrollIntoView({block:'center'});",
                btn
            );

            new Actions(Driver).DoubleClick(btn).Perform();
            Thread.Sleep(1200);
            WaitReady();
        }

        private string GetValidationMessage(string css)
        {
            try
            {
                var el = Driver.FindElement(By.CssSelector(css));

                return ((IJavaScriptExecutor)Driver)
                    .ExecuteScript("return arguments[0].validationMessage || '';", el)
                    ?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private bool IsVisible(string css)
        {
            try
            {
                return Driver.FindElement(By.CssSelector(css)).Displayed;
            }
            catch
            {
                return false;
            }
        }

        private (int R, int G, int B) RgbaToRgbTuple(string value)
        {
            var nums = Regex.Matches(value ?? "", @"[\d.]+")
                            .Select(m => m.Value)
                            .ToList();

            if (nums.Count < 3)
            {
                return (255, 255, 255);
            }

            return (
                int.Parse(double.Parse(nums[0]).ToString("0")),
                int.Parse(double.Parse(nums[1]).ToString("0")),
                int.Parse(double.Parse(nums[2]).ToString("0"))
            );
        }

        private double RelativeLuminance((int R, int G, int B) rgb)
        {
            double Channel(int c)
            {
                double value = c / 255.0;

                return value <= 0.03928
                    ? value / 12.92
                    : Math.Pow((value + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(rgb.R) +
                   0.7152 * Channel(rgb.G) +
                   0.0722 * Channel(rgb.B);
        }

        private double ContrastRatio((int R, int G, int B) fg, (int R, int G, int B) bg)
        {
            double l1 = RelativeLuminance(fg);
            double l2 = RelativeLuminance(bg);

            double lighter = Math.Max(l1, l2);
            double darker = Math.Min(l1, l2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        private string GetEffectiveBgColor(IWebElement element)
        {
            string script = @"
                let el = arguments[0];

                while (el) {
                    let bg = window.getComputedStyle(el).backgroundColor;

                    if (bg && bg !== 'rgba(0, 0, 0, 0)' && bg !== 'transparent') {
                        return bg;
                    }

                    el = el.parentElement;
                }

                return 'rgb(255, 255, 255)';
            ";

            return ((IJavaScriptExecutor)Driver).ExecuteScript(script, element)?.ToString()
                   ?? "rgb(255, 255, 255)";
        }

        private Dictionary<string, object> UpdateOnlineStatus(int productId, int status)
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
            Thread.Sleep(700);

            if (result is Dictionary<string, object> dict)
            {
                return dict;
            }

            return new Dictionary<string, object>
            {
                ["raw"] = result?.ToString() ?? ""
            };
        }

        [When("User truy cập trang giỏ hàng")]
        public void WhenUserTruyCapTrangGioHang()
        {
            OpenCart();
        }

        [Then("trang giỏ hàng mở được và không hiển thị lỗi hệ thống")]
        public void ThenTrangGioHangMoDuocVaKhongLoi()
        {
            AssertNoServerError();
        }

        [When("User thêm thuốc còn hàng vào giỏ bằng Ajax")]
        public void WhenUserThemThuocConHangVaoGioBangAjax()
        {
            AddToCartAjax(ProductId1);
            OpenCart();
        }

        [Then("giỏ hàng hiển thị thuốc vừa thêm")]
        public void ThenGioHangHienThiThuocVuaThem()
        {
            Assert.That(HasProductInCart(ProductId1), Is.True, "Giỏ hàng không hiển thị thuốc vừa thêm");
        }

        [Given("trong giỏ hàng đã có thuốc còn hàng")]
        public void GivenTrongGioHangDaCoThuocConHang()
        {
            EnsureProductInCart(ProductId1);
        }

        [When("User thêm trùng cùng một thuốc vào giỏ")]
        public void WhenUserThemTrungCungMotThuocVaoGio()
        {
            OpenCart();

            int oldQty = GetQtyValue(ProductId1);

            _scenarioContext["OldQty"] = oldQty;

            AddToCartAjax(ProductId1);
            OpenCart();
        }

        [Then("số lượng thuốc trong giỏ phải tăng lên")]
        public void ThenSoLuongThuocTrongGioPhaiTangLen()
        {
            int oldQty = (int)_scenarioContext["OldQty"];
            int newQty = GetQtyValue(ProductId1);

            Assert.That(newQty, Is.GreaterThan(oldQty), $"Thêm trùng nhưng số lượng không tăng. Trước={oldQty}, Sau={newQty}");
        }

        [When("User thêm nhiều thuốc khác nhau vào giỏ")]
        public void WhenUserThemNhieuThuocKhacNhauVaoGio()
        {
            EnsureProductInCart(ProductId1);
            EnsureProductInCart(ProductId2);
            OpenCart();
        }

        [Then("giỏ hàng hiển thị đủ nhiều thuốc khác nhau")]
        public void ThenGioHangHienThiDuNhieuThuocKhacNhau()
        {
            var ids = GetCartProductIds();

            Assert.That(ids, Does.Contain(ProductId1));
            Assert.That(ids, Does.Contain(ProductId2));
        }

        [Given("trong giỏ hàng có nhiều thuốc")]
        public void GivenTrongGioHangCoNhieuThuoc()
        {
            EnsureProductInCart(ProductId1);
            EnsureProductInCart(ProductId2);
            OpenCart();
        }

        [Then("tổng tiền giỏ hàng phải bằng tổng thành tiền các dòng sản phẩm")]
        public void ThenTongTienBangTongThanhTienCacDong()
        {
            var itemTotals = GetAllItemTotals();
            int cartTotal = GetCartTotal();

            Assert.That(itemTotals.Count, Is.GreaterThan(0), "Không đọc được thành tiền từng dòng");

            int sumItems = itemTotals.Sum();

            Assert.That(cartTotal, Is.EqualTo(sumItems), $"Tổng tiền sai. Tổng dòng={sumItems}, Tổng giỏ={cartTotal}");
        }

        [When("User hover vào mini cart")]
        public void WhenUserHoverVaoMiniCart()
        {
            OpenHome();

            var cart = WaitVisibleCss(".cart-wrapper");
            new Actions(Driver).MoveToElement(cart).Perform();

            Thread.Sleep(1000);

            string txt = GetText("#miniCartContainer");

            if (txt.Length < 3)
            {
                LoadMiniCart();
            }
        }

        [Then("mini cart hiển thị danh sách sản phẩm")]
        public void ThenMiniCartHienThiDanhSachSanPham()
        {
            string txt = GetText("#miniCartContainer");

            Assert.That(txt.Length, Is.GreaterThan(2), "Mini cart không hiển thị nội dung");
        }

        [When("User nhấn tên thuốc trong mini cart")]
        public void WhenUserNhanTenThuocTrongMiniCart()
        {
            LoadMiniCart();

            var links = Driver.FindElements(By.CssSelector("#miniCartContainer a[href*='ChiTiet']"));

            if (links.Count == 0)
            {
                links = Driver.FindElements(By.CssSelector("#miniCartContainer a"));
            }

            Assert.That(links.Count, Is.GreaterThan(0), "Không tìm thấy link tên thuốc trong mini cart");

            SafeClick(links[0]);

            Thread.Sleep(1000);
            WaitReady();
        }

        [When("User nhấn tên thuốc trong trang giỏ hàng chi tiết")]
        public void WhenUserNhanTenThuocTrongTrangGioHangChiTiet()
        {
            OpenCart();

            var link = WaitVisibleCss("table tbody a[href*='ChiTiet']");
            SafeClick(link);

            Thread.Sleep(1000);
            WaitReady();
        }

        [Then("hệ thống chuyển sang trang chi tiết thuốc và không hiển thị lỗi hệ thống")]
        public void ThenHeThongChuyenSangChiTietThuocVaKhongLoi()
        {
            AssertNoServerError();
        }

        [Given("trong giỏ hàng đã có thuốc còn hàng với số lượng lớn hơn 1")]
        public void GivenTrongGioHangDaCoThuocConHangVoiSoLuongLonHon1()
        {
            EnsureProductInCart(ProductId1);

            if (GetQtyValue(ProductId1) < 2)
            {
                UpdateQtyAjax(ProductId1, 2);
                OpenCart();
            }
        }

        [Given("trong giỏ hàng đã có thuốc còn hàng với số lượng bằng 1")]
        public void GivenTrongGioHangDaCoThuocConHangVoiSoLuongBang1()
        {
            EnsureProductInCart(ProductId1);
            UpdateQtyAjax(ProductId1, 1);
            OpenCart();
        }

        [When("User tăng số lượng thuốc bằng nút cộng")]
        public void WhenUserTangSoLuongThuocBangNutCong()
        {
            OpenCart();

            _scenarioContext["OldQty"] = GetQtyValue(ProductId1);
            _scenarioContext["OldTotal"] = GetCartTotal();

            var buttons = Driver.FindElements(By.CssSelector($"button[onclick*='updateQuantity({ProductId1}, 1)'], #qty-{ProductId1} ~ .btn-plus"));

            if (buttons.Count > 0)
            {
                SafeClick(buttons[0]);
            }
            else
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript($"updateQuantity({ProductId1}, 1);");
            }

            Thread.Sleep(1200);
            OpenCart();
        }

        [Then("số lượng thuốc tăng lên")]
        public void ThenSoLuongThuocTangLen()
        {
            int oldQty = (int)_scenarioContext["OldQty"];
            int newQty = GetQtyValue(ProductId1);

            Assert.That(newQty, Is.GreaterThan(oldQty), $"Số lượng không tăng. Trước={oldQty}, Sau={newQty}");
        }

        [Then("tổng tiền giỏ hàng tăng lên")]
        public void ThenTongTienGioHangTangLen()
        {
            int oldTotal = (int)_scenarioContext["OldTotal"];
            int newTotal = GetCartTotal();

            Assert.That(newTotal, Is.GreaterThan(oldTotal), $"Tổng tiền không tăng. Trước={oldTotal}, Sau={newTotal}");
        }

        [When("User giảm số lượng thuốc bằng nút trừ")]
        public void WhenUserGiamSoLuongThuocBangNutTru()
        {
            OpenCart();

            _scenarioContext["OldQty"] = GetQtyValue(ProductId1);
            _scenarioContext["OldTotal"] = GetCartTotal();

            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript($"updateQuantity({ProductId1}, -1);");
            }
            catch
            {
                var buttons = Driver.FindElements(By.CssSelector($"button[onclick*='updateQuantity({ProductId1}, -1)']"));

                if (buttons.Count > 0)
                {
                    SafeClick(buttons[0]);
                }
            }

            Thread.Sleep(1200);
            OpenCart();
        }

        [Then("số lượng thuốc giảm xuống")]
        public void ThenSoLuongThuocGiamXuong()
        {
            int oldQty = (int)_scenarioContext["OldQty"];
            int newQty = GetQtyValue(ProductId1);

            Assert.That(newQty, Is.LessThan(oldQty), $"Số lượng không giảm. Trước={oldQty}, Sau={newQty}");
        }

        [Then("tổng tiền giỏ hàng giảm xuống")]
        public void ThenTongTienGioHangGiamXuong()
        {
            int oldTotal = (int)_scenarioContext["OldTotal"];
            int newTotal = GetCartTotal();

            Assert.That(newTotal, Is.LessThan(oldTotal), $"Tổng tiền không giảm. Trước={oldTotal}, Sau={newTotal}");
        }

        [Then("số lượng thuốc không được nhỏ hơn 1")]
        public void ThenSoLuongThuocKhongDuocNhoHon1()
        {
            int qty = GetQtyValue(ProductId1);

            Assert.That(qty, Is.GreaterThanOrEqualTo(1), $"Số lượng bị giảm dưới 1. Qty={qty}");
        }

        [When("User bấm nút cộng liên tục nhiều lần")]
        public void WhenUserBamNutCongLienTucNhieuLan()
        {
            OpenCart();

            _scenarioContext["OldQty"] = GetQtyValue(ProductId1);

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript($"updateQuantity({ProductId1}, 1);");
                }
                catch
                {
                    var buttons = Driver.FindElements(By.CssSelector(".btn-plus"));

                    if (buttons.Count > 0)
                    {
                        SafeClick(buttons[0]);
                    }
                }

                Thread.Sleep(120);
            }

            Thread.Sleep(2000);
            OpenCart();
        }

        [Then("số lượng thuốc vẫn được cập nhật hợp lệ")]
        public void ThenSoLuongThuocVanDuocCapNhatHopLe()
        {
            int oldQty = (int)_scenarioContext["OldQty"];
            int newQty = GetQtyValue(ProductId1);

            Assert.That(newQty, Is.GreaterThan(oldQty), $"Bấm cộng liên tục nhưng số lượng không tăng. Trước={oldQty}, Sau={newQty}");
        }

        [Then("tổng tiền giỏ hàng vẫn hợp lệ")]
        public void ThenTongTienGioHangVanHopLe()
        {
            int total = GetCartTotal();

            Assert.That(total, Is.GreaterThan(0), "Tổng tiền sau khi bấm cộng liên tục bị sai hoặc bằng 0");
        }

        [When("User cập nhật số lượng thuốc vượt tồn kho bằng Ajax")]
        public void WhenUserCapNhatSoLuongThuocVuotTonKhoBangAjax()
        {
            _scenarioContext["OldQty"] = GetQtyValue(ProductId1);

            var result = UpdateQtyAjax(ProductId1, 999999);

            _scenarioContext["AjaxResult"] = result;

            OpenCart();
        }

        [Then("hệ thống phải chặn số lượng vượt tồn kho")]
        public void ThenHeThongPhaiChanSoLuongVuotTonKho()
        {
            var result = (Dictionary<string, object>)_scenarioContext["AjaxResult"];
            bool? success = GetSuccessFromAjaxResult(result);

            int newQty = GetQtyValue(ProductId1);

            if (success == true && newQty >= 999999)
            {
                Assert.Fail("Hệ thống cho cập nhật số lượng vượt tồn kho. Response=" + string.Join(", ", result));
            }

            Assert.That(newQty, Is.LessThan(999999), "Số lượng trong giỏ bị cập nhật vượt tồn kho");
        }

        [When("User thêm thuốc hết hàng vào giỏ bằng Ajax")]
        public void WhenUserThemThuocHetHangVaoGioBangAjax()
        {
            OpenCart();

            int beforeQty = GetQtyValue(OutOfStockProductId);

            _scenarioContext["BeforeOutOfStockQty"] = beforeQty;

            var result = AddToCartAjax(OutOfStockProductId);

            _scenarioContext["AjaxResult"] = result;

            OpenCart();
        }

        [Then("hệ thống không được thêm thuốc hết hàng vào giỏ")]
        public void ThenHeThongKhongDuocThemThuocHetHangVaoGio()
        {
            int beforeQty = (int)_scenarioContext["BeforeOutOfStockQty"];
            int afterQty = GetQtyValue(OutOfStockProductId);

            var result = (Dictionary<string, object>)_scenarioContext["AjaxResult"];
            bool? success = GetSuccessFromAjaxResult(result);

            if (success == true && afterQty > beforeQty)
            {
                Assert.Fail($"Thuốc hết hàng vẫn thêm được vào giỏ. Before={beforeQty}, After={afterQty}");
            }
        }

        [When("User mở modal xóa sản phẩm rồi bấm Đóng")]
        public void WhenUserMoModalXoaSanPhamRoiBamDong()
        {
            OpenCart();

            _scenarioContext["OldRows"] = Driver.FindElements(By.CssSelector("table tbody tr")).Count;

            var trashButtons = Driver.FindElements(By.CssSelector("a[onclick*='confirmDelete']"));

            Assert.That(trashButtons.Count, Is.GreaterThan(0), "Không tìm thấy nút xóa sản phẩm");

            SafeClick(trashButtons[0]);

            WaitVisibleCss("#deleteModal.show");

            var closeButtons = Driver.FindElements(By.XPath("//button[contains(., 'Đóng')]"));

            if (closeButtons.Count == 0)
            {
                closeButtons = Driver.FindElements(By.CssSelector("#deleteModal button[data-bs-dismiss='modal']"));
            }

            Assert.That(closeButtons.Count, Is.GreaterThan(0), "Modal xóa không có nút Đóng");

            SafeClick(closeButtons.Last());
            Thread.Sleep(800);
        }

        [Then("modal xóa được đóng lại")]
        public void ThenModalXoaDuocDongLai()
        {
            var modals = Driver.FindElements(By.CssSelector("#deleteModal.show"));

            Assert.That(modals.Count, Is.EqualTo(0), "Modal xóa vẫn đang mở");
        }

        [Then("sản phẩm không bị xóa khỏi giỏ hàng")]
        public void ThenSanPhamKhongBiXoaKhoiGioHang()
        {
            int oldRows = (int)_scenarioContext["OldRows"];
            int newRows = Driver.FindElements(By.CssSelector("table tbody tr")).Count;

            Assert.That(newRows, Is.EqualTo(oldRows), $"Sản phẩm bị thay đổi. Trước={oldRows}, Sau={newRows}");
        }

        [Given("trong giỏ hàng đã có thuốc dùng để kiểm thử xóa")]
        public void GivenTrongGioHangDaCoThuocDungDeKiemThuXoa()
        {
            Assert.Ignore("Đang tắt để tránh xóa dữ liệu thật, giống cấu hình Python ALLOW_DELETE=False.");
        }

        [When("User xác nhận xóa sản phẩm khỏi giỏ hàng")]
        public void WhenUserXacNhanXoaSanPhamKhoiGioHang()
        {
            Assert.Ignore("Đang tắt để tránh xóa dữ liệu thật.");
        }

        [Then("sản phẩm bị xóa khỏi giỏ hàng")]
        public void ThenSanPhamBiXoaKhoiGioHang()
        {
            Assert.Ignore("Đang tắt để tránh xóa dữ liệu thật.");
        }

        [When("User chuyển đổi giữa Giao hàng tận nơi và Nhận tại nhà thuốc")]
        public void WhenUserChuyenDoiGiaoHangVaNhanTaiNhaThuoc()
        {
            SelectPickupMethod();

            _scenarioContext["PickupVisible"] = IsVisible("#pickup-form");
            _scenarioContext["ShippingVisibleAfterPickup"] = IsVisible("#shipping-form");

            SelectShippingMethod();

            _scenarioContext["ShippingVisible"] = IsVisible("#shipping-form");
            _scenarioContext["PickupVisibleAfterShipping"] = IsVisible("#pickup-form");
        }

        [Then("form giao hàng và form nhận tại nhà thuốc hiển thị đúng theo lựa chọn")]
        public void ThenFormGiaoHangVaNhanTaiNhaThuocHienThiDung()
        {
            Assert.That((bool)_scenarioContext["PickupVisible"], Is.True, "Chọn nhận tại nhà thuốc nhưng pickup-form không hiển thị");
            Assert.That((bool)_scenarioContext["ShippingVisibleAfterPickup"], Is.False, "Chọn nhận tại nhà thuốc nhưng shipping-form vẫn hiển thị");

            Assert.That((bool)_scenarioContext["ShippingVisible"], Is.True, "Chọn giao hàng nhưng shipping-form không hiển thị");
            Assert.That((bool)_scenarioContext["PickupVisibleAfterShipping"], Is.False, "Chọn giao hàng nhưng pickup-form vẫn hiển thị");
        }

        [When("User đặt hàng nhưng để trống họ tên")]
        public void WhenUserDatHangNhungDeTrongHoTen()
        {
            SelectShippingMethod();
            ClearCustomerForm();
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường họ tên")]
        public void ThenHeThongHienThiValidationHoTen()
        {
            string msg = GetValidationMessage("#txtHoTen");
            Assert.That(msg, Is.Not.Empty, "Để trống họ tên nhưng không hiện validationMessage");
        }

        [When("User đặt hàng với họ tên chỉ toàn khoảng trắng")]
        public void WhenUserDatHangVoiHoTenChiToanKhoangTrang()
        {
            SelectShippingMethod();
            FillBasicInfo(TestNameSpace, TestPhoneValid, "");
            ClickPlaceOrder();
        }

        [Then("hệ thống phải chặn họ tên không hợp lệ")]
        public void ThenHeThongPhaiChanHoTenKhongHopLe()
        {
            string msg = GetValidationMessage("#txtHoTen");

            Assert.That(msg, Is.Not.Empty, "Họ tên chỉ toàn khoảng trắng vẫn không bị chặn");
        }

        [When("User đặt hàng với số điện thoại sai định dạng")]
        public void WhenUserDatHangVoiSoDienThoaiSaiDinhDang()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneInvalid, "");
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường số điện thoại")]
        public void ThenHeThongHienThiValidationSoDienThoai()
        {
            string msg = GetValidationMessage("#txtSoDienThoai");
            Assert.That(msg, Is.Not.Empty, "Số điện thoại sai định dạng nhưng không báo lỗi");
        }

        [When("User đặt hàng với email sai định dạng")]
        public void WhenUserDatHangVoiEmailSaiDinhDang()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, TestEmailInvalid);
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường email")]
        public void ThenHeThongHienThiValidationEmail()
        {
            string msg = GetValidationMessage("#txtEmail");
            Assert.That(msg, Is.Not.Empty, "Email sai định dạng nhưng không báo lỗi");
        }

        [When("User chọn giao hàng tận nơi nhưng chưa chọn Tỉnh Thành")]
        public void WhenUserChonGiaoHangNhungChuaChonTinhThanh()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, "");

            ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                let tinh = document.getElementById('dataApiTinhThanh');
                if (tinh) {
                    tinh.disabled = false;
                    tinh.value = '';
                }
            ");

            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường Tỉnh Thành")]
        public void ThenHeThongHienThiValidationTinhThanh()
        {
            string msg = GetValidationMessage("#dataApiTinhThanh");
            Assert.That(msg, Is.Not.Empty, "Chưa chọn Tỉnh/Thành nhưng không báo lỗi");
        }

        [When("User chọn giao hàng tận nơi nhưng chưa chọn Phường Xã")]
        public void WhenUserChonGiaoHangNhungChuaChonPhuongXa()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, "");
            InjectShippingMissingWard();
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường Phường Xã")]
        public void ThenHeThongHienThiValidationPhuongXa()
        {
            string msg = GetValidationMessage("#dataApiPhuongXa");
            Assert.That(msg, Is.Not.Empty, "Chưa chọn Phường/Xã nhưng không báo lỗi");
        }

        [When("User chọn giao hàng tận nơi nhưng chưa nhập số nhà tên đường")]
        public void WhenUserChonGiaoHangNhungChuaNhapSoNha()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, "");
            InjectValidShippingData();

            ((IJavaScriptExecutor)Driver).ExecuteScript("document.getElementById('txtDiaChiCuThe').value = '';");
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị validation cho trường địa chỉ cụ thể")]
        public void ThenHeThongHienThiValidationDiaChiCuThe()
        {
            string msg = GetValidationMessage("#txtDiaChiCuThe");
            Assert.That(msg, Is.Not.Empty, "Chưa nhập số nhà/tên đường nhưng không báo lỗi");
        }

        [When("User chọn nhận tại nhà thuốc nhưng chưa chọn nhà thuốc")]
        public void WhenUserChonNhanTaiNhaThuocNhungChuaChonNhaThuoc()
        {
            SelectPickupMethod();
            FillBasicInfo(TestName, TestPhoneValid, "");
            InjectPickupAreaWithoutPharmacy();
            ClickPlaceOrder();
        }

        [Then("hệ thống hiển thị thông báo yêu cầu chọn nhà thuốc")]
        public void ThenHeThongHienThiThongBaoYeuCauChonNhaThuoc()
        {
            string msg = GetValidationMessage("#ddlQuan_Pickup");
            string page = BodyText();

            bool hasMessage =
                !string.IsNullOrWhiteSpace(msg) ||
                page.Contains("Vui lòng chọn một nhà thuốc", StringComparison.OrdinalIgnoreCase);

            Assert.That(hasMessage, Is.True, "Nhận tại nhà thuốc nhưng chưa chọn nhà thuốc mà không báo lỗi");
        }

        [When("User đặt hàng bằng phương thức thanh toán QR Code")]
        public void WhenUserDatHangBangPhuongThucThanhToanQRCode()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, TestEmailValid);
            InjectValidShippingData();

            var bankRadio = Driver.FindElement(By.CssSelector("input[name='PhuongThucThanhToan'][value='BANK']"));
            SafeClick(bankRadio);

            ClickPlaceOrder();
        }

        [Then("modal QR Code được hiển thị")]
        public void ThenModalQRCodeDuocHienThi()
        {
            WaitVisibleCss("#qrModal.show", 10);
        }

        [Then("QR Code có thông tin thanh toán hợp lệ")]
        public void ThenQRCodeCoThongTinThanhToanHopLe()
        {
            string qrSrc = Driver.FindElement(By.CssSelector("#img-qr-code")).GetAttribute("src") ?? "";
            string qrContent = GetText("#qr-content");
            string qrAmount = GetText("#qr-amount");

            Assert.That(qrSrc, Does.Contain("img.vietqr.io"), "QR không dùng link VietQR đúng");
            Assert.That(qrContent, Does.StartWith("DH"), "Nội dung chuyển khoản không đúng dạng DHxxxxxx");
            Assert.That(NumberFromText(qrAmount), Is.GreaterThan(0), "Số tiền QR không hợp lệ");
        }

        [Given("modal QR Code đang được hiển thị")]
        public void GivenModalQRCodeDangDuocHienThi()
        {
            EnsureProductInCart(ProductId1);

            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, TestEmailValid);
            InjectValidShippingData();

            var bankRadio = Driver.FindElement(By.CssSelector("input[name='PhuongThucThanhToan'][value='BANK']"));
            SafeClick(bankRadio);

            ClickPlaceOrder();
            WaitVisibleCss("#qrModal.show", 10);
        }

        [When("User bấm nút Tôi đã chuyển tiền")]
        public void WhenUserBamNutToiDaChuyenTien()
        {
            ClickCss("#btnConfirmPayment");
            Thread.Sleep(3000);
        }

        [Then("hệ thống hiển thị trạng thái kiểm tra thanh toán")]
        public void ThenHeThongHienThiTrangThaiKiemTraThanhToan()
        {
            string statusText = GetText("#payment-status");

            Assert.That(statusText, Is.Not.Empty, "Không hiển thị trạng thái kiểm tra thanh toán");

            bool valid =
                statusText.Contains("Chưa thấy tiền") ||
                statusText.Contains("Đã nhận") ||
                statusText.Contains("Lỗi") ||
                statusText.Contains("Đang kiểm tra");

            Assert.That(valid, Is.True, "Trạng thái thanh toán khó hiểu: " + statusText);
        }

        [When("User double click nút Đặt hàng ngay với QR")]
        public void WhenUserDoubleClickNutDatHangNgayVoiQR()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, TestEmailValid);
            InjectValidShippingData();

            var bankRadio = Driver.FindElement(By.CssSelector("input[name='PhuongThucThanhToan'][value='BANK']"));
            SafeClick(bankRadio);

            DoubleClickPlaceOrder();
            Thread.Sleep(1000);
        }

        [Then("modal QR Code vẫn hiển thị hợp lệ")]
        public void ThenModalQRCodeVanHienThiHopLe()
        {
            var modals = Driver.FindElements(By.CssSelector("#qrModal.show"));
            Assert.That(modals.Count, Is.GreaterThan(0), "Double click Đặt hàng nhưng không hiện QR Modal");
        }
[When("User hover nút Mua thêm thuốc khác")]
   public void WhenUserHoverNutMuaThemThuocKhac()
   {
       OpenCart();

       var btnCandidates = Driver.FindElements(By.XPath("//a[contains(., 'Mua thêm thuốc khác')]"));

       Assert.That(btnCandidates.Count, Is.GreaterThan(0), "Không tìm thấy nút Mua thêm thuốc khác");

       var btn = btnCandidates[0];

       new Actions(Driver).MoveToElement(btn).Perform();

       Thread.Sleep(500);

       // Thay đổi ValueOfCssProperty thành GetCssValue
       string color = btn.GetCssValue("color"); 
       string bg = GetEffectiveBgColor(btn);

       double ratio = ContrastRatio(RgbaToRgbTuple(color), RgbaToRgbTuple(bg));

       _scenarioContext["ContrastRatio"] = ratio;
       _scenarioContext["Color"] = color;
       _scenarioContext["BackgroundColor"] = bg;
   }

        [Then("chữ trên nút vẫn dễ đọc sau khi hover")]
        public void ThenChuTrenNutVanDeDocSauKhiHover()
        {
            double ratio = (double)_scenarioContext["ContrastRatio"];
            string color = _scenarioContext["Color"].ToString() ?? "";
            string bg = _scenarioContext["BackgroundColor"].ToString() ?? "";

            Assert.That(ratio, Is.GreaterThanOrEqualTo(3), $"Độ tương phản thấp. contrast={ratio:0.00}, color={color}, bg={bg}");
        }

        [Then("giao diện giỏ hàng không bị tràn ngang nghiêm trọng trên mobile")]
        public void ThenGiaoDienGioHangKhongBiTranNgangMobile()
        {
            OpenCart();

            var js = (IJavaScriptExecutor)Driver;

            long innerWidth = Convert.ToInt64(js.ExecuteScript("return window.innerWidth;"));
            long scrollWidth = Convert.ToInt64(js.ExecuteScript("return document.documentElement.scrollWidth;"));
            long bodyScrollWidth = Convert.ToInt64(js.ExecuteScript("return document.body.scrollWidth;"));

            long overflow = Math.Max(scrollWidth, bodyScrollWidth) - innerWidth;

            Assert.That(overflow, Is.LessThanOrEqualTo(80), "Giao diện mobile bị tràn ngang nhiều");

            Driver.Manage().Window.Size = new System.Drawing.Size(1366, 768);
        }

        [When("User nhập dữ liệu XSS vào họ tên khi đặt hàng")]
        public void WhenUserNhapDuLieuXSSVaoHoTen()
        {
            SelectShippingMethod();
            FillBasicInfo(TestXssName, TestPhoneValid, "");
            InjectValidShippingData();

            var bankRadio = Driver.FindElement(By.CssSelector("input[name='PhuongThucThanhToan'][value='BANK']"));
            SafeClick(bankRadio);

            ClickPlaceOrder();

            Thread.Sleep(800);
        }

        [Then("hệ thống không được thực thi script XSS")]
        public void ThenHeThongKhongDuocThucThiScriptXSS()
        {
            string? alertMsg = AcceptAlertIfAny();

            Assert.That(alertMsg, Is.Null, "Dữ liệu XSS trong họ tên bị thực thi alert: " + alertMsg);
        }

        [When("User nhập chuỗi đặc biệt giống SQL injection vào địa chỉ")]
        public void WhenUserNhapChuoiDacBietGiongSQLInjectionVaoDiaChi()
        {
            SelectShippingMethod();
            FillBasicInfo(TestName, TestPhoneValid, TestEmailValid);
            InjectValidShippingData();

            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "document.getElementById('txtDiaChiCuThe').value = arguments[0];",
                TestSqlAddress
            );

            var bankRadio = Driver.FindElement(By.CssSelector("input[name='PhuongThucThanhToan'][value='BANK']"));
            SafeClick(bankRadio);

            ClickPlaceOrder();

            Thread.Sleep(1000);
        }

        [When("User cập nhật số lượng giỏ hàng ở tab thứ nhất")]
        public void WhenUserCapNhatSoLuongGioHangOTabThuNhat()
        {
            OpenCart();

            int oldQty = GetQtyValue(ProductId1);
            int expectedQty = oldQty + 1;

            _scenarioContext["ExpectedQtyTab2"] = expectedQty;

            string mainTab = Driver.CurrentWindowHandle;

            ((IJavaScriptExecutor)Driver).ExecuteScript(
                "window.open(arguments[0], '_blank');",
                FullUrl("/GioHang/XemGioHang")
            );

            Thread.Sleep(1000);

            var tabs = Driver.WindowHandles;
            string secondTab = tabs.First(t => t != mainTab);

            _scenarioContext["MainTab"] = mainTab;
            _scenarioContext["SecondTab"] = secondTab;

            Driver.SwitchTo().Window(mainTab);

            UpdateQtyAjax(ProductId1, expectedQty);
        }

        [Then("tab thứ hai sau khi reload phải hiển thị số lượng mới")]
        public void ThenTabThuHaiSauReloadPhaiHienThiSoLuongMoi()
        {
            int expectedQty = (int)_scenarioContext["ExpectedQtyTab2"];
            string mainTab = _scenarioContext["MainTab"].ToString()!;
            string secondTab = _scenarioContext["SecondTab"].ToString()!;

            Driver.SwitchTo().Window(secondTab);
            Driver.Navigate().Refresh();
            WaitReady();

            Thread.Sleep(800);

            int qtyInSecondTab = GetQtyValue(ProductId1);

            Driver.Close();
            Driver.SwitchTo().Window(mainTab);

            Assert.That(qtyInSecondTab, Is.EqualTo(expectedQty), $"Giỏ hàng không đồng bộ giữa 2 tab. Expected={expectedQty}, Tab2={qtyInSecondTab}");
        }

        [Given("Admin đã tắt bán online cho thuốc kiểm thử")]
        public void GivenAdminDaTatBanOnlineChoThuocKiemThu()
        {
            Assert.Ignore("Đang tắt để tránh đổi dữ liệu Admin thật, giống cấu hình Python ALLOW_ADMIN_STATUS_TOGGLE=False.");
        }

        [When("User thêm thuốc đã bị tắt bán online vào giỏ")]
        public void WhenUserThemThuocDaBiTatBanOnlineVaoGio()
        {
            Assert.Ignore("Đang tắt để tránh đổi dữ liệu Admin thật.");
        }

        [Then("hệ thống không được thêm thuốc đã tắt bán online vào giỏ")]
        public void ThenHeThongKhongDuocThemThuocDaTatBanOnlineVaoGio()
        {
            Assert.Ignore("Đang tắt để tránh đổi dữ liệu Admin thật.");
        }

        [When("User đặt hàng COD với dữ liệu hợp lệ")]
        public void WhenUserDatHangCODVoiDuLieuHopLe()
        {
            Assert.Ignore("Đang tắt để tránh tạo đơn hàng thật, giống cấu hình Python ALLOW_CREATE_ORDER=False.");
        }

        [Then("hệ thống tạo đơn hàng thành công")]
        public void ThenHeThongTaoDonHangThanhCong()
        {
            Assert.Ignore("Đang tắt để tránh tạo đơn hàng thật.");
        }
        
[Given("trình duyệt đang ở kích thước màn hình mobile 390px")]
public void GivenTrinhDuyetDangOKichThuocManHinhMobile390px()
        {
            Driver.Manage().Window.Size = new System.Drawing.Size(390, 844);
            Thread.Sleep(500);
        }

        [Then("hệ thống không hiển thị lỗi Server Error")]
        public void ThenHeThongKhongHienThiLoiServerError()
        {
            AssertNoServerError();
        }


    }
}

