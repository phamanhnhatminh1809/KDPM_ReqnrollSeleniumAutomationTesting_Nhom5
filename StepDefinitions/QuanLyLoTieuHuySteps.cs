using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[Binding]
public class QuanLyLoTieuHuySteps
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    private const string BaseUrl = "http://localhost:44317";
    private const string LoginUrl = "/TaiKhoan/DangNhap";
    private const string LoTieuHuyUrl = "/LoThuoc/QuanLyTieuHuy";

    [BeforeScenario("@LoTieuHuy")]
    public void BeforeScenario()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--ignore-certificate-errors");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [AfterScenario("@LoTieuHuy")]
    public void AfterScenario()
    {
        try
        {
            _driver?.Quit();
        }
        catch
        {
            // Bỏ qua lỗi đóng trình duyệt nếu có.
        }
    }

    // =========================
    // WHEN
    // =========================

    [When(@"Admin truy cập trang Quản lý Lô tiêu hủy")]
    public void WhenAdminTruyCapTrangQuanLyLoTieuHuy()
    {
        OpenLoTieuHuy();
    }

    [When(@"Admin lọc lô tiêu hủy theo trạng thái Đã hết hạn")]
    public void WhenAdminLocLoTieuHuyTheoTrangThaiDaHetHan()
    {
        OpenLoTieuHuy("?status=expired");
    }

    [When(@"Admin lọc lô tiêu hủy theo trạng thái Sắp hết hạn")]
    public void WhenAdminLocLoTieuHuyTheoTrangThaiSapHetHan()
    {
        OpenLoTieuHuy("?status=warning");
    }

    [When(@"Admin chọn bộ lọc Tất cả lô tiêu hủy")]
    public void WhenAdminChonBoLocTatCaLoTieuHuy()
    {
        OpenLoTieuHuy("?status=");
    }

    [When(@"Admin kiểm tra link sắp xếp Số lô ở trang Quản lý Lô tiêu hủy")]
    public void WhenAdminKiemTraLinkSapXepSoLo()
    {
        OpenLoTieuHuy();
    }

    [When(@"Admin sắp xếp danh sách lô tiêu hủy theo Số lô tăng dần")]
    public void WhenAdminSapXepDanhSachLoTieuHuyTheoSoLoTangDan()
    {
        OpenLoTieuHuy("?sortOrder=solo_asc");
    }

    [When(@"Admin truy cập trang Quản lý Lô tiêu hủy với status không hợp lệ")]
    public void WhenAdminTruyCapTrangQuanLyLoTieuHuyVoiStatusKhongHopLe()
    {
        OpenLoTieuHuy("?status=abcxyz");
    }

    [When(@"Admin mở trang Quản lý Lô tiêu hủy trên màn hình mobile")]
    public void WhenAdminMoTrangQuanLyLoTieuHuyTrenManHinhMobile()
    {
        _driver.Manage().Window.Size = new System.Drawing.Size(390, 844);
        System.Threading.Thread.Sleep(500);
        OpenLoTieuHuy();
    }

    // =========================
    // THEN
    // =========================

    [Then(@"trang Quản lý Lô tiêu hủy mở được, có tiêu đề và không hiển thị lỗi hệ thống")]
    public void ThenTrangQuanLyLoTieuHuyMoDuocCoTieuDeVaKhongLoi()
    {
        var page = BodyText();

        var hasTitle =
            page.Contains("Quản lý tiêu huỷ", StringComparison.OrdinalIgnoreCase) ||
            page.Contains("Quản lý tiêu hủy", StringComparison.OrdinalIgnoreCase);

        Assert.That(hasTitle, Is.True, "Không thấy tiêu đề Quản lý tiêu hủy.");

        Assert.That(page, Does.Contain("Quản lý Kho và Lô thuốc"),
            "Không thấy tiêu đề nội dung Quản lý Kho và Lô thuốc.");

        Assert.That(page, Does.Contain("Sắp hết hạn"), "Thiếu nút Sắp hết hạn.");
        Assert.That(page, Does.Contain("Đã hết hạn"), "Thiếu nút Đã hết hạn.");
        Assert.That(page, Does.Contain("Tất cả"), "Thiếu nút Tất cả.");

        AssertNoServerError();
    }

    [Then(@"menu Quản lý Lô tiêu hủy trên sidebar được active đúng")]
    public void ThenMenuQuanLyLoTieuHuyTrenSidebarDuocActiveDung()
    {
        var activeMenus = _driver.FindElements(By.CssSelector(".sidebar a.active"));

        Assert.That(activeMenus.Count, Is.GreaterThan(0), "Không có menu active trong sidebar.");

        var activeText = activeMenus[0].Text;

        var isCorrect =
            activeText.Contains("Quản lý Lô tiêu huỷ", StringComparison.OrdinalIgnoreCase) ||
            activeText.Contains("Quản lý Lô tiêu hủy", StringComparison.OrdinalIgnoreCase);

        Assert.That(isCorrect, Is.True, $"Menu active không đúng. Active hiện tại: {activeText}");
    }

    [Then(@"bảng lô tiêu hủy hiển thị đủ các cột Thuốc, Số lô, Hạn sử dụng, Giá nhập, Nhập tồn và Trạng thái")]
    public void ThenBangLoTieuHuyHienThiDuCacCot()
    {
        var page = BodyText();

        Assert.That(page, Does.Contain("Thuốc"), "Bảng thiếu cột Thuốc.");
        Assert.That(page, Does.Contain("Số lô"), "Bảng thiếu cột Số lô.");
        Assert.That(page, Does.Contain("Hạn sử dụng"), "Bảng thiếu cột Hạn sử dụng.");
        Assert.That(page, Does.Contain("Giá nhập"), "Bảng thiếu cột Giá nhập.");
        Assert.That(page, Does.Contain("Nhập"), "Bảng thiếu cột Nhập / Tồn.");
        Assert.That(page, Does.Contain("Tồn"), "Bảng thiếu cột Nhập / Tồn.");
        Assert.That(page, Does.Contain("Trạng thái"), "Bảng thiếu cột Trạng thái.");

        Assert.That(page.Contains("Tác vụ"), Is.False,
            "Trang Quản lý Lô tiêu hủy không nên có cột Tác vụ.");
    }

    [Then(@"danh sách lô tiêu hủy hiển thị dữ liệu hợp lệ gồm tên thuốc, số lô, hạn sử dụng, giá nhập và trạng thái")]
    public void ThenDanhSachLoTieuHuyHienThiDuLieuHopLe()
    {
        var rows = GetDestroyedLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Bảng Quản lý Lô tiêu hủy không có dữ liệu.");

        foreach (var row in rows)
        {
            Assert.That(row.Thuoc, Is.Not.Empty, $"Dòng thiếu tên thuốc: {row.Text}");
            Assert.That(row.SoLo, Is.Not.Empty, $"Dòng thiếu số lô: {row.Text}");
            Assert.That(row.HanSuDung, Is.Not.Null, $"Dòng không đọc được hạn sử dụng: {row.Text}");
            Assert.That(NumberFromText(row.GiaNhap), Is.GreaterThan(0), $"Giá nhập không hợp lệ: {row.Text}");
            Assert.That(row.TrangThai, Is.Not.Empty, $"Dòng thiếu trạng thái: {row.Text}");
        }
    }

    [Then(@"tất cả dòng trong bảng phải có trạng thái Đã tiêu hủy")]
    public void ThenTatCaDongPhaiCoTrangThaiDaTieuHuy()
    {
        var rows = GetDestroyedLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu để kiểm tra trạng thái.");

        foreach (var row in rows)
        {
            var status = row.TrangThai.ToLower();

            var isDestroyed =
                status.Contains("đã tiêu huỷ") ||
                status.Contains("đã tiêu hủy");

            Assert.That(isDestroyed, Is.True,
                $"Có lô không phải trạng thái Đã tiêu hủy: {row.SoLo} - {row.TrangThai}");
        }
    }

    [Then(@"tất cả lô đã tiêu hủy phải có tồn kho bằng 0")]
    public void ThenTatCaLoDaTieuHuyPhaiCoTonKhoBang0()
    {
        var rows = GetDestroyedLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu để kiểm tra tồn kho.");

        foreach (var row in rows)
        {
            var nums = Regex.Matches(row.NhapTon, @"\d+")
                            .Select(m => int.Parse(m.Value))
                            .ToList();

            Assert.That(nums.Count, Is.GreaterThanOrEqualTo(2),
                $"Cột Nhập , Tồn không đủ dữ liệu: {row.SoLo} - {row.NhapTon}");

            var ton = nums.Last();

            Assert.That(ton, Is.EqualTo(0),
                $"Lô đã tiêu hủy nhưng tồn kho khác 0: {row.SoLo} - {row.NhapTon}");
        }
    }

    [Then(@"trang lô tiêu hủy chỉ dùng để xem và không có nút xóa, sửa hoặc tiêu hủy lại")]
    public void ThenTrangLoTieuHuyChiDungDeXemVaKhongCoNutTacVu()
    {
        var rows = GetDestroyedLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu để kiểm tra nút tác vụ.");

        foreach (var row in rows)
        {
            Assert.That(row.Buttons.Count, Is.EqualTo(0),
                $"Lô tiêu hủy vẫn có nút thao tác: {row.SoLo}");
        }
    }

    [Then(@"hệ thống chỉ hiển thị các lô tiêu hủy có hạn sử dụng nhỏ hơn ngày hiện tại")]
    public void ThenHeThongChiHienThiLoTieuHuyDaHetHan()
    {
        var rows = GetDestroyedLotRowsMayEmpty();

        if (rows.Count == 0)
            return;

        var today = DateTime.Today;

        foreach (var row in rows)
        {
            Assert.That(row.HanSuDung, Is.Not.Null, $"Không đọc được hạn sử dụng: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.LessThan(today),
                $"Lô không thuộc nhóm đã hết hạn: {row.SoLo}");

            Assert.That(row.HanSuDungText, Does.Contain("(Hết hạn)"),
                $"Lô hết hạn nhưng không có nhãn Hết hạn: {row.SoLo}");
        }
    }

    [Then(@"hệ thống chỉ hiển thị các lô tiêu hủy có hạn sử dụng từ ngày hiện tại đến 90 ngày tới")]
    public void ThenHeThongChiHienThiLoTieuHuySapHetHan()
    {
        var rows = GetDestroyedLotRowsMayEmpty();

        if (rows.Count == 0)
            return;

        var today = DateTime.Today;
        var maxDay = today.AddDays(90);

        foreach (var row in rows)
        {
            Assert.That(row.HanSuDung, Is.Not.Null, $"Không đọc được hạn sử dụng: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.GreaterThanOrEqualTo(today),
                $"Lô không thuộc nhóm sắp hết hạn: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.LessThanOrEqualTo(maxDay),
                $"Lô không thuộc nhóm sắp hết hạn trong 90 ngày: {row.SoLo}");
        }
    }

    [Then(@"hệ thống hiển thị toàn bộ danh sách lô đã tiêu hủy")]
    public void ThenHeThongHienThiToanBoDanhSachLoDaTieuHuy()
    {
        var rows = GetDestroyedLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0),
            "Bộ lọc Tất cả không hiển thị danh sách lô tiêu hủy.");
    }

    [Then(@"các nút Sắp hết hạn, Đã hết hạn và Tất cả có đường dẫn lọc đúng")]
    public void ThenCacNutLocCoDuongDanDung()
    {
        AssertFilterLink("Sắp hết hạn", "status=warning");
        AssertFilterLink("Đã hết hạn", "status=expired");
        AssertFilterLink("Tất cả", "status=");
    }

    [Then(@"link sort Số lô phải ở lại trang Quản lý Lô tiêu hủy và không chuyển nhầm sang trang Quản lý Lô thuốc")]
    public void ThenLinkSortSoLoPhaiOLaiTrangQuanLyLoTieuHuy()
    {
        var sortLinks = _driver.FindElements(By.XPath("//th//a[contains(., 'Số lô')]"));

        Assert.That(sortLinks.Count, Is.GreaterThan(0), "Không tìm thấy link sort Số lô.");

        var href = sortLinks[0].GetAttribute("href") ?? "";

        Assert.That(href, Does.Contain("/LoThuoc/QuanLyTieuHuy"),
            $"Link sort Số lô đang trỏ sai trang. Actual={href}");

        Assert.That(href.Contains("/LoThuoc/Index"), Is.False,
            $"Link sort Số lô bị chuyển nhầm sang trang Quản lý Lô thuốc. Actual={href}");
    }

    [Then(@"danh sách lô tiêu hủy được sắp xếp đúng theo số lô tăng dần")]
    public void ThenDanhSachLoTieuHuySapXepDungTheoSoLoTangDan()
    {
        var rows = GetDestroyedLotRowsMayEmpty();

        if (rows.Count == 0)
            return;

        var actualCodes = rows.Select(r => r.SoLo)
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .ToList();

        var expectedCodes = actualCodes.OrderBy(x => x.ToLower()).ToList();

        CollectionAssert.AreEqual(expectedCodes, actualCodes,
            "Danh sách chưa được sắp xếp theo số lô tăng dần.");
    }

    [Then(@"các lô hết hạn và sắp hết hạn được gắn nhãn hoặc đánh dấu phù hợp")]
    public void ThenCacLoHetHanVaSapHetHanDuocGanNhanHoacDanhDauPhuHop()
    {
        var rows = GetDestroyedLotRows();

        var today = DateTime.Today;
        var maxWarning = today.AddDays(90);

        foreach (var row in rows)
        {
            Assert.That(row.HanSuDung, Is.Not.Null, $"Không đọc được hạn sử dụng: {row.SoLo}");

            var date = row.HanSuDung.Value.Date;

            if (date < today)
            {
                Assert.That(row.HanSuDungText, Does.Contain("(Hết hạn)"),
                    $"Lô hết hạn nhưng thiếu nhãn Hết hạn: {row.SoLo}");
            }
            else if (date >= today && date <= maxWarning)
            {
                var isMarked =
                    row.RowClass.Contains("table-warning") ||
                    row.RowClass.Contains("table-secondary");

                Assert.That(isMarked, Is.True,
                    $"Lô sắp hết hạn không được đánh dấu phù hợp: {row.SoLo}");
            }
        }
    }

    [Then(@"giá nhập của các lô tiêu hủy là số dương và hiển thị đúng định dạng")]
    public void ThenGiaNhapCuaLoTieuHuyLaSoDuongVaDungDinhDang()
    {
        var rows = GetDestroyedLotRows();

        foreach (var row in rows)
        {
            var price = NumberFromText(row.GiaNhap);

            Assert.That(price, Is.GreaterThan(0),
                $"Giá nhập không hợp lệ: {row.SoLo} - {row.GiaNhap}");

            Assert.That(
                Regex.IsMatch(row.GiaNhap, @"\d{1,3}(,\d{3})*"),
                Is.True,
                $"Giá nhập không đúng định dạng có dấu phẩy: {row.SoLo} - {row.GiaNhap}"
            );
        }
    }

    [Then(@"số nhập hiển thị hợp lệ và tồn kho của lô tiêu hủy phải bằng 0")]
    public void ThenSoNhapHopLeVaTonKhoLoTieuHuyBang0()
    {
        var rows = GetDestroyedLotRows();

        foreach (var row in rows)
        {
            var nums = Regex.Matches(row.NhapTon, @"\d+")
                            .Select(m => int.Parse(m.Value))
                            .ToList();

            Assert.That(nums.Count, Is.GreaterThanOrEqualTo(2),
                $"Cột Nhập , Tồn không đủ dữ liệu: {row.SoLo} - {row.NhapTon}");

            var nhap = nums.First();
            var ton = nums.Last();

            Assert.That(nhap, Is.GreaterThan(0),
                $"Số nhập không hợp lệ: {row.SoLo} - {row.NhapTon}");

            Assert.That(ton, Is.EqualTo(0),
                $"Lô đã tiêu hủy nhưng tồn không bằng 0: {row.SoLo} - {row.NhapTon}");
        }
    }

    [Then(@"các lô đã tiêu hủy có màu xám và chữ mờ đúng giao diện")]
    public void ThenCacLoDaTieuHuyCoMauXamVaChuMoDungGiaoDien()
    {
        var rows = GetDestroyedLotRows();

        foreach (var row in rows)
        {
            Assert.That(row.RowClass, Does.Contain("table-secondary"),
                $"Lô đã tiêu hủy chưa có màu xám table-secondary: {row.SoLo}");

            Assert.That(row.RowClass, Does.Contain("text-muted"),
                $"Lô đã tiêu hủy chưa có text-muted: {row.SoLo}");
        }
    }

    [Then(@"ảnh thuốc trong bảng lô tiêu hủy có đường dẫn src")]
    public void ThenAnhThuocTrongBangCoDuongDanSrc()
    {
        var rows = GetDestroyedLotRows();

        foreach (var row in rows)
        {
            Assert.That(row.Image, Is.Not.Null, $"Dòng không có ảnh thuốc: {row.SoLo}");

            var src = row.Image.GetAttribute("src") ?? "";

            Assert.That(src, Is.Not.Empty, $"Ảnh thuốc không có src: {row.SoLo}");
        }
    }

    [Then(@"không có ảnh thuốc bị vỡ trong bảng lô tiêu hủy")]
    public void ThenKhongCoAnhThuocBiVoTrongBangLoTieuHuy()
    {
        var js = (IJavaScriptExecutor)_driver;

        var result = js.ExecuteScript(@"
            const imgs = Array.from(document.querySelectorAll('table tbody img'));
            return imgs
                .filter(img => img.complete && img.naturalWidth === 0)
                .map(img => img.getAttribute('src'));
        ");

        var broken = ((IEnumerable<object>)result)
            .Select(x => x?.ToString() ?? "")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        Assert.That(broken.Count, Is.EqualTo(0),
            "Có ảnh thuốc bị lỗi hoặc không tải được: " + string.Join(", ", broken));
    }

    [Then(@"trang Quản lý Lô tiêu hủy không hiển thị số âm bất thường")]
    public void ThenTrangQuanLyLoTieuHuyKhongHienThiSoAmBatThuong()
    {
        var page = BodyText();

        var negativeNumbers = Regex.Matches(page, @"(?<![\w])-[\s]*\d[\d,.]*")
                                   .Select(m => m.Value)
                                   .ToList();

        Assert.That(negativeNumbers.Count, Is.EqualTo(0),
            "Trang Quản lý Lô tiêu hủy đang hiển thị số âm: " + string.Join(", ", negativeNumbers));
    }

    [Then(@"hệ thống không phát sinh lỗi hệ thống")]
    public void ThenHeThongKhongPhatSinhLoiHeThong()
    {
        AssertNoServerError();
    }

    [Then(@"trang không chứa hàm hoặc endpoint xóa, sửa, tiêu hủy lại lô thuốc")]
    public void ThenTrangKhongChuaHamHoacEndpointXuLyLai()
    {
        var html = _driver.PageSource.ToLower();

        string[] forbidden =
        {
            "deletelo(",
            "tieuhuylo(",
            "/lothuoc/delete",
            "/lothuoc/tieuhuy"
        };

        foreach (var keyword in forbidden)
        {
            Assert.That(html.Contains(keyword), Is.False,
                $"Trang lô tiêu hủy không nên chứa thao tác xử lý lại: {keyword}");
        }
    }

    [Then(@"nút Thoát hiển thị và có đường dẫn hợp lệ")]
    public void ThenNutThoatHienThiVaCoDuongDanHopLe()
    {
        var logoutLinks = _driver.FindElements(By.XPath("//a[contains(., 'Thoát')]"));

        Assert.That(logoutLinks.Count, Is.GreaterThan(0),
            "Không tìm thấy nút Thoát trên trang Quản lý Lô tiêu hủy.");

        var href = logoutLinks[0].GetAttribute("href") ?? "";

        Assert.That(href, Is.Not.Empty, "Nút Thoát không có đường dẫn.");
    }

    [Then(@"trang không bị tràn ngang nghiêm trọng")]
    public void ThenTrangKhongBiTranNgangNghiemTrong()
    {
        var js = (IJavaScriptExecutor)_driver;

        var widthInfo = (Dictionary<string, object>)js.ExecuteScript(@"
            return {
                innerWidth: window.innerWidth,
                scrollWidth: document.documentElement.scrollWidth,
                bodyScrollWidth: document.body.scrollWidth
            };
        ");

        var innerWidth = Convert.ToInt64(widthInfo["innerWidth"]);
        var scrollWidth = Convert.ToInt64(widthInfo["scrollWidth"]);
        var bodyScrollWidth = Convert.ToInt64(widthInfo["bodyScrollWidth"]);

        var overflow = Math.Max(scrollWidth, bodyScrollWidth) - innerWidth;

        Assert.That(overflow, Is.LessThanOrEqualTo(1400),
            $"Trang Quản lý Lô tiêu hủy tràn ngang nghiêm trọng trên mobile. Overflow={overflow}");
    }

    // =========================
    // HÀM HỖ TRỢ
    // =========================

    private void OpenLoTieuHuy(string query = "")
    {
        LoginAdmin();

        _driver.Navigate().GoToUrl(FullUrl(LoTieuHuyUrl + query));
        WaitReady();

        AssertNoServerError();
    }

    private void LoginAdmin()
    {
        _driver.Navigate().GoToUrl(FullUrl(LoginUrl));
        WaitReady();

        var page = BodyText();

        if (page.Contains("Xin chào, Admin", StringComparison.OrdinalIgnoreCase))
            return;

        var usernameInput = _wait.Until(d => d.FindElement(By.CssSelector("#TenDangNhap")));
        var passwordInput = _wait.Until(d => d.FindElement(By.CssSelector("#MatKhau")));

        usernameInput.Clear();
        usernameInput.SendKeys("admin");

        passwordInput.Clear();
        passwordInput.SendKeys("admin123");

        var loginButtons = _driver.FindElements(By.XPath("//button[contains(., 'Đăng Nhập')]"));

        Assert.That(loginButtons.Count, Is.GreaterThan(0), "Không tìm thấy nút Đăng Nhập.");

        SafeClick(loginButtons[0]);
        System.Threading.Thread.Sleep(1200);
        WaitReady();
    }

    private string FullUrl(string path)
    {
        if (path.StartsWith("http"))
            return path;

        return BaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
    }

    private void WaitReady()
    {
        _wait.Until(d =>
        {
            var state = ((IJavaScriptExecutor)d)
                .ExecuteScript("return document.readyState")
                ?.ToString();

            return state == "complete";
        });

        try
        {
            _wait.Until(d =>
            {
                var result = ((IJavaScriptExecutor)d)
                    .ExecuteScript("return window.jQuery ? jQuery.active === 0 : true;");

                return Convert.ToBoolean(result);
            });
        }
        catch
        {
            // Không có jQuery thì bỏ qua.
        }
    }

    private string BodyText()
    {
        try
        {
            return _driver.FindElement(By.TagName("body")).Text;
        }
        catch
        {
            return "";
        }
    }

    private void AssertNoServerError()
    {
        var page = BodyText();
        var title = _driver.Title ?? "";

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
            "404 - File or directory not found",
            "500 Internal Server Error"
        };

        foreach (var key in keywords)
        {
            var hasError =
                page.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                title.Contains(key, StringComparison.OrdinalIgnoreCase);

            Assert.That(hasError, Is.False, $"Phát hiện lỗi hệ thống: {key}");
        }
    }

    private List<DestroyedLotRow> GetDestroyedLotRows()
    {
        var rows = GetDestroyedLotRowsMayEmpty();

        Assert.That(rows.Count, Is.GreaterThan(0), "Bảng Quản lý Lô tiêu hủy không có dữ liệu.");

        return rows;
    }

    private List<DestroyedLotRow> GetDestroyedLotRowsMayEmpty()
    {
        var tableRows = _driver.FindElements(By.CssSelector("table tbody tr"));
        var result = new List<DestroyedLotRow>();

        foreach (var row in tableRows)
        {
            var cells = row.FindElements(By.CssSelector("td"));

            if (cells.Count < 6)
                continue;

            var imgs = row.FindElements(By.CssSelector("img"));
            var buttons = row.FindElements(By.CssSelector("button, a.btn")).ToList();

            result.Add(new DestroyedLotRow
            {
                Element = row,
                Text = row.Text.Trim(),
                Thuoc = cells[0].Text.Trim(),
                SoLo = cells[1].Text.Trim(),
                HanSuDungText = cells[2].Text.Trim(),
                HanSuDung = ParseVnDate(cells[2].Text.Trim()),
                GiaNhap = cells[3].Text.Trim(),
                NhapTon = cells[4].Text.Trim(),
                TrangThai = cells[5].Text.Trim(),
                RowClass = row.GetAttribute("class") ?? "",
                Buttons = buttons,
                Image = imgs.Count > 0 ? imgs[0] : null
            });
        }

        return result;
    }

    private DateTime? ParseVnDate(string text)
    {
        var match = Regex.Match(text ?? "", @"(\d{2}/\d{2}/\d{4})");

        if (!match.Success)
            return null;

        if (DateTime.TryParseExact(
                match.Value,
                "dd/MM/yyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out var date))
        {
            return date;
        }

        return null;
    }

    private long NumberFromText(string text)
    {
        var digits = Regex.Replace(text ?? "", @"\D", "");

        if (long.TryParse(digits, out var number))
            return number;

        return 0;
    }

    private void SafeClick(IWebElement element)
    {
        var js = (IJavaScriptExecutor)_driver;

        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", element);
        System.Threading.Thread.Sleep(200);
        js.ExecuteScript("arguments[0].click();", element);
    }

    private void AssertFilterLink(string text, string keyword)
    {
        var links = _driver.FindElements(By.XPath($"//a[contains(., '{text}')]"));

        Assert.That(links.Count, Is.GreaterThan(0), $"Không tìm thấy nút lọc: {text}");

        var href = links[0].GetAttribute("href") ?? "";

        Assert.That(href, Does.Contain(keyword), $"Nút {text} có href sai: {href}");
    }

    private class DestroyedLotRow
    {
        public IWebElement Element { get; set; }
        public string Text { get; set; }
        public string Thuoc { get; set; }
        public string SoLo { get; set; }
        public string HanSuDungText { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string GiaNhap { get; set; }
        public string NhapTon { get; set; }
        public string TrangThai { get; set; }
        public string RowClass { get; set; }
        public List<IWebElement> Buttons { get; set; }
        public IWebElement Image { get; set; }
    }
}

