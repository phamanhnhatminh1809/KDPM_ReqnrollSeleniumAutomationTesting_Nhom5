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
public class QuanLyLoThuocSteps
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    private const string BaseUrl = "http://localhost:44317";
    private const string LoginUrl = "/TaiKhoan/DangNhap";
    private const string LoThuocUrl = "/LoThuoc/Index";
    private const string LoTieuHuyUrl = "/LoThuoc/QuanLyTieuHuy";

    private int _beforeCount;
    private string _targetCode = "";
    private bool _backendOk;
    private bool? _backendSuccess;
    private string _backendText = "";

    [BeforeScenario("@LoThuoc")]
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

    [AfterScenario("@LoThuoc")]
    public void AfterScenario()
    {
        try
        {
            _driver?.Quit();
        }
        catch
        {
            // Bỏ qua lỗi đóng browser nếu có.
        }
    }

    // =========================
    // WHEN
    // =========================

    [When(@"Admin truy cập trang Quản lý Lô thuốc")]
    public void WhenAdminTruyCapTrangQuanLyLoThuoc()
    {
        OpenLoThuoc();
    }

    [When(@"Admin lọc lô thuốc theo trạng thái Sắp hết hạn")]
    public void WhenAdminLocLoThuocTheoTrangThaiSapHetHan()
    {
        OpenLoThuoc("?status=warning");
    }

    [When(@"Admin lọc lô thuốc theo trạng thái Đã hết hạn")]
    public void WhenAdminLocLoThuocTheoTrangThaiDaHetHan()
    {
        OpenLoThuoc("?status=expired");
    }

    [When(@"Admin chọn bộ lọc Tất cả lô thuốc")]
    public void WhenAdminChonBoLocTatCaLoThuoc()
    {
        OpenLoThuoc("?status=");
    }

    [When(@"Admin sắp xếp danh sách lô thuốc theo Số lô tăng dần")]
    public void WhenAdminSapXepDanhSachLoThuocTheoSoLoTangDan()
    {
        OpenLoThuoc("?sortOrder=solo_asc");
    }

    [When(@"Admin kiểm tra nút Xóa của lô thuốc")]
    public void WhenAdminKiemTraNutXoaCuaLoThuoc()
    {
        OpenLoThuoc();
    }

    [When(@"Admin kiểm tra nút Tiêu hủy của lô thuốc")]
    public void WhenAdminKiemTraNutTieuHuyCuaLoThuoc()
    {
        OpenLoThuoc();
    }

    [When(@"Admin bấm Xóa lô thuốc nhưng chọn Cancel ở hộp xác nhận")]
    public void WhenAdminBamXoaLoThuocNhungChonCancel()
    {
        OpenLoThuoc();

        var rows = GetLotRows();
        var target = rows.FirstOrDefault(r => r.OnClicks.Any(x => x.Contains("deleteLo")));

        Assert.That(target, Is.Not.Null, "Không tìm thấy lô có nút Xóa để kiểm tra Cancel.");

        _beforeCount = rows.Count;
        _targetCode = target.SoLo;

        OverrideConfirmReturnFalse();

        var deleteButton = target.Buttons.FirstOrDefault(b =>
            (b.GetAttribute("onclick") ?? "").Contains("deleteLo"));

        Assert.That(deleteButton, Is.Not.Null, "Không lấy được nút Xóa.");

        SafeClick(deleteButton);
        System.Threading.Thread.Sleep(800);
    }

    [When(@"Admin bấm Tiêu hủy lô thuốc nhưng chọn Cancel ở hộp xác nhận")]
    public void WhenAdminBamTieuHuyLoThuocNhungChonCancel()
    {
        OpenLoThuoc();

        var rows = GetLotRows();
        var target = rows.FirstOrDefault(r => r.OnClicks.Any(x => x.Contains("tieuHuyLo")));

        Assert.That(target, Is.Not.Null, "Không tìm thấy lô có nút Tiêu hủy để kiểm tra Cancel.");

        _beforeCount = rows.Count;
        _targetCode = target.SoLo;

        OverrideConfirmReturnFalse();

        var destroyButton = target.Buttons.FirstOrDefault(b =>
            (b.GetAttribute("onclick") ?? "").Contains("tieuHuyLo"));

        Assert.That(destroyButton, Is.Not.Null, "Không lấy được nút Tiêu hủy.");

        SafeClick(destroyButton);
        System.Threading.Thread.Sleep(800);
    }

    [When(@"Admin gửi request xóa lô thuốc với ID không tồn tại")]
    public void WhenAdminGuiRequestXoaLoThuocVoiIdKhongTonTai()
    {
        OpenLoThuoc();
        SendBackendRequest("/LoThuoc/Delete", -999999);
    }

    [When(@"Admin gửi request tiêu hủy lô thuốc với ID không tồn tại")]
    public void WhenAdminGuiRequestTieuHuyLoThuocVoiIdKhongTonTai()
    {
        OpenLoThuoc();
        SendBackendRequest("/LoThuoc/TieuHuy", -999999);
    }

    [When(@"Admin mở trang Quản lý Lô thuốc trên màn hình mobile")]
    public void WhenAdminMoTrangQuanLyLoThuocTrenManHinhMobile()
    {
        _driver.Manage().Window.Size = new System.Drawing.Size(390, 844);
        System.Threading.Thread.Sleep(500);
        OpenLoThuoc();
    }

    [When(@"Admin xác nhận xóa thật một lô thuốc")]
    public void WhenAdminXacNhanXoaThatMotLoThuoc()
    {
        Assert.Ignore("Test xóa thật đang được bỏ qua để tránh thay đổi dữ liệu thật.");
    }

    [When(@"Admin xác nhận tiêu hủy thật một lô thuốc")]
    public void WhenAdminXacNhanTieuHuyThatMotLoThuoc()
    {
        Assert.Ignore("Test tiêu hủy thật đang được bỏ qua để tránh thay đổi dữ liệu thật.");
    }

    // =========================
    // THEN
    // =========================

    [Then(@"trang Quản lý Lô thuốc mở được, có tiêu đề và không hiển thị lỗi hệ thống")]
    public void ThenTrangQuanLyLoThuocMoDuocCoTieuDeVaKhongLoi()
    {
        var page = BodyText();

        Assert.That(page, Does.Contain("Quản lý Kho và Lô thuốc"),
            "Không thấy tiêu đề Quản lý Kho và Lô thuốc.");

        AssertNoServerError();
    }

    [Then(@"menu Quản lý Lô thuốc trên sidebar được active đúng")]
    public void ThenMenuQuanLyLoThuocTrenSidebarDuocActiveDung()
    {
        var activeMenus = _driver.FindElements(By.CssSelector(".sidebar a.active"));

        Assert.That(activeMenus.Count, Is.GreaterThan(0), "Không có menu active trong sidebar.");
        Assert.That(activeMenus[0].Text, Does.Contain("Quản lý Lô thuốc"),
            $"Menu active không đúng. Active hiện tại: {activeMenus[0].Text}");
    }

    [Then(@"bảng lô thuốc hiển thị đủ các cột Thuốc, Số lô, Hạn sử dụng, Giá nhập, Nhập tồn, Trạng thái và Tác vụ")]
    public void ThenBangLoThuocHienThiDuCacCot()
    {
        var page = BodyText();

        Assert.That(page, Does.Contain("Thuốc"), "Bảng thiếu cột Thuốc.");
        Assert.That(page, Does.Contain("Số lô"), "Bảng thiếu cột Số lô.");
        Assert.That(page, Does.Contain("Hạn sử dụng"), "Bảng thiếu cột Hạn sử dụng.");
        Assert.That(page, Does.Contain("Giá nhập"), "Bảng thiếu cột Giá nhập.");
        Assert.That(page, Does.Contain("Nhập"), "Bảng thiếu cột Nhập / Tồn.");
        Assert.That(page, Does.Contain("Tồn"), "Bảng thiếu cột Nhập / Tồn.");
        Assert.That(page, Does.Contain("Trạng thái"), "Bảng thiếu cột Trạng thái.");
        Assert.That(page, Does.Contain("Tác vụ"), "Bảng thiếu cột Tác vụ.");
    }

    [Then(@"danh sách lô thuốc hiển thị dữ liệu hợp lệ gồm tên thuốc, số lô, hạn sử dụng, giá nhập và trạng thái")]
    public void ThenDanhSachLoThuocHienThiDuLieuHopLe()
    {
        var rows = GetLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Bảng Quản lý Lô thuốc không có dữ liệu.");

        foreach (var row in rows.Take(15))
        {
            Assert.That(row.Thuoc, Is.Not.Empty, $"Dòng thiếu tên thuốc: {row.Text}");
            Assert.That(row.SoLo, Is.Not.Empty, $"Dòng thiếu số lô: {row.Text}");
            Assert.That(row.HanSuDung, Is.Not.Null, $"Dòng không đọc được hạn sử dụng: {row.Text}");
            Assert.That(NumberFromText(row.GiaNhap), Is.GreaterThan(0), $"Giá nhập không hợp lệ: {row.Text}");
            Assert.That(row.TrangThai, Is.Not.Empty, $"Dòng thiếu trạng thái: {row.Text}");
        }
    }

    [Then(@"hệ thống chỉ hiển thị các lô có hạn sử dụng từ ngày hiện tại đến 90 ngày tới")]
    public void ThenHeThongChiHienThiLoSapHetHan()
    {
        var rows = GetLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu lô sắp hết hạn.");

        var today = DateTime.Today;
        var maxDay = today.AddDays(90);

        foreach (var row in rows)
        {
            Assert.That(row.HanSuDung, Is.Not.Null, $"Không đọc được hạn sử dụng: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.GreaterThanOrEqualTo(today),
                $"Lô không thuộc nhóm sắp hết hạn: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.LessThanOrEqualTo(maxDay),
                $"Lô không thuộc nhóm sắp hết hạn trong 90 ngày: {row.SoLo}");

            Assert.That(row.RowClass, Does.Contain("table-warning"),
                $"Lô sắp hết hạn không có màu cảnh báo: {row.SoLo}");
        }
    }

    [Then(@"hệ thống chỉ hiển thị các lô có hạn sử dụng nhỏ hơn ngày hiện tại")]
    public void ThenHeThongChiHienThiLoDaHetHan()
    {
        var rows = GetLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu lô đã hết hạn.");

        var today = DateTime.Today;

        foreach (var row in rows)
        {
            Assert.That(row.HanSuDung, Is.Not.Null, $"Không đọc được hạn sử dụng: {row.SoLo}");

            Assert.That(row.HanSuDung.Value.Date, Is.LessThan(today),
                $"Lô không thuộc nhóm đã hết hạn: {row.SoLo}");

            Assert.That(
                row.HanSuDungText.Contains("(Hết hạn)") || row.RowClass.Contains("table-danger"),
                Is.True,
                $"Lô hết hạn không có nhãn hoặc màu hết hạn: {row.SoLo}"
            );
        }
    }

    [Then(@"hệ thống hiển thị toàn bộ danh sách lô thuốc")]
    public void ThenHeThongHienThiToanBoDanhSachLoThuoc()
    {
        var rows = GetLotRows();
        Assert.That(rows.Count, Is.GreaterThan(0), "Bộ lọc Tất cả không hiển thị dữ liệu.");
    }

    [Then(@"các nút lọc của trang Quản lý Lô thuốc có đường dẫn đúng")]
    public void ThenCacNutLocCoDuongDanDung()
    {
        AssertFilterLink("Sắp hết hạn", "status=warning");
        AssertFilterLink("Đã hết hạn", "status=expired");
        AssertFilterLink("Tất cả", "status=");
    }

    [Then(@"danh sách lô thuốc được sắp xếp đúng theo số lô tăng dần")]
    public void ThenDanhSachLoThuocSapXepDungTheoSoLoTangDan()
    {
        var rows = GetLotRows();
        var actualCodes = rows.Select(r => r.SoLo).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        var expectedCodes = actualCodes.OrderBy(x => x.ToLower()).ToList();

        CollectionAssert.AreEqual(expectedCodes, actualCodes, "Danh sách chưa được sắp xếp theo số lô tăng dần.");
    }

    [Then(@"các lô hết hạn và sắp hết hạn được gắn nhãn hoặc tô màu đúng")]
    public void ThenCacLoHetHanVaSapHetHanDuocGanNhanHoacToMauDung()
    {
        var rows = GetLotRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu để kiểm tra phân loại hạn sử dụng.");

        var today = DateTime.Today;
        var maxWarning = today.AddDays(90);

        foreach (var row in rows)
        {
            if (row.HanSuDung == null)
                continue;

            var date = row.HanSuDung.Value.Date;

            if (date < today)
            {
                Assert.That(
                    row.RowClass.Contains("table-danger") || row.HanSuDungText.Contains("(Hết hạn)"),
                    Is.True,
                    $"Lô hết hạn không được đánh dấu đúng: {row.SoLo}"
                );
            }
            else if (date >= today && date <= maxWarning)
            {
                Assert.That(row.RowClass, Does.Contain("table-warning"),
                    $"Lô sắp hết hạn không được tô màu cảnh báo: {row.SoLo}");
            }
        }
    }

    [Then(@"giá nhập của các lô thuốc là số dương và hiển thị đúng định dạng")]
    public void ThenGiaNhapLaSoDuongVaDungDinhDang()
    {
        var rows = GetLotRows();

        foreach (var row in rows.Take(30))
        {
            var price = NumberFromText(row.GiaNhap);

            Assert.That(price, Is.GreaterThan(0), $"Giá nhập không hợp lệ: {row.SoLo} - {row.GiaNhap}");

            Assert.That(
                Regex.IsMatch(row.GiaNhap, @"\d{1,3}(,\d{3})*"),
                Is.True,
                $"Giá nhập không đúng định dạng có dấu phẩy: {row.SoLo} - {row.GiaNhap}"
            );
        }
    }

    [Then(@"số nhập và số tồn hiển thị hợp lệ, không âm và tồn không lớn hơn nhập")]
    public void ThenSoNhapVaSoTonHienThiHopLe()
    {
        var rows = GetLotRows();

        foreach (var row in rows.Take(30))
        {
            var nums = Regex.Matches(row.NhapTon, @"\d+")
                            .Select(m => int.Parse(m.Value))
                            .ToList();

            Assert.That(nums.Count, Is.GreaterThanOrEqualTo(2),
                $"Cột Nhập , Tồn không đủ dữ liệu: {row.SoLo} - {row.NhapTon}");

            var nhap = nums.First();
            var ton = nums.Last();

            Assert.That(nhap, Is.GreaterThanOrEqualTo(0), $"Số nhập bị âm: {row.SoLo}");
            Assert.That(ton, Is.GreaterThanOrEqualTo(0), $"Số tồn bị âm: {row.SoLo}");
            Assert.That(ton, Is.LessThanOrEqualTo(nhap), $"Số tồn lớn hơn số nhập: {row.SoLo}");
        }
    }

    [Then(@"trạng thái lô thuốc thuộc nhóm Đang bán, Ngưng bán hoặc Đã tiêu hủy")]
    public void ThenTrangThaiLoThuocThuocNhomHopLe()
    {
        var rows = GetLotRows();

        foreach (var row in rows.Take(40))
        {
            var status = row.TrangThai.ToLower();

            var isValid =
                status.Contains("đang bán") ||
                status.Contains("ngưng bán") ||
                status.Contains("đã tiêu huỷ") ||
                status.Contains("đã tiêu hủy");

            Assert.That(isValid, Is.True, $"Trạng thái lô không hợp lệ: {row.SoLo} - {row.TrangThai}");
        }
    }

    [Then(@"mỗi lô thuốc có nút tác vụ phù hợp")]
    public void ThenMoiLoThuocCoNutTacVuPhuHop()
    {
        var rows = GetLotRows();

        foreach (var row in rows.Take(40))
        {
            Assert.That(row.Buttons.Count, Is.GreaterThan(0), $"Dòng không có nút tác vụ: {row.SoLo}");
        }
    }

    [Then(@"nút Xóa gọi đúng hàm deleteLo")]
    public void ThenNutXoaGoiDungHamDeleteLo()
    {
        var rows = GetLotRows();
        var hasDelete = rows.Any(r => r.OnClicks.Any(x => x.Contains("deleteLo")));

        Assert.That(hasDelete, Is.True, "Không tìm thấy nút Xóa gọi hàm deleteLo.");
    }

    [Then(@"nút Tiêu hủy gọi đúng hàm tieuHuyLo")]
    public void ThenNutTieuHuyGoiDungHamTieuHuyLo()
    {
        var rows = GetLotRows();
        var hasDestroy = rows.Any(r => r.OnClicks.Any(x => x.Contains("tieuHuyLo")));

        Assert.That(hasDestroy, Is.True, "Không tìm thấy nút Tiêu hủy gọi hàm tieuHuyLo.");
    }

    [Then(@"hệ thống không xóa lô thuốc và dữ liệu không thay đổi")]
    public void ThenHeThongKhongXoaLoThuocVaDuLieuKhongThayDoi()
    {
        AssertConfirmWasShown("xóa lô");

        OpenLoThuoc();

        var rows = GetLotRows();
        var afterCount = rows.Count;
        var afterCodes = rows.Select(r => r.SoLo).ToList();

        Assert.That(afterCount, Is.EqualTo(_beforeCount), "Cancel xóa nhưng số lượng lô đã thay đổi.");
        Assert.That(afterCodes, Does.Contain(_targetCode), "Cancel xóa nhưng lô đã bị mất khỏi danh sách.");
    }

    [Then(@"hệ thống không tiêu hủy lô thuốc và dữ liệu không thay đổi")]
    public void ThenHeThongKhongTieuHuyLoThuocVaDuLieuKhongThayDoi()
    {
        AssertConfirmWasShown("tiêu");

        OpenLoThuoc();

        var rows = GetLotRows();
        var afterCount = rows.Count;
        var afterCodes = rows.Select(r => r.SoLo).ToList();

        Assert.That(afterCount, Is.EqualTo(_beforeCount), "Cancel tiêu hủy nhưng số lượng lô đã thay đổi.");
        Assert.That(afterCodes, Does.Contain(_targetCode), "Cancel tiêu hủy nhưng lô đã bị mất khỏi danh sách.");
    }

    [Then(@"backend trả về kết quả thất bại và không phát sinh lỗi hệ thống")]
    public void ThenBackendTraVeKetQuaThatBaiVaKhongPhatSinhLoiHeThong()
    {
        Assert.That(_backendOk, Is.True, $"Request backend gây lỗi HTTP hoặc lỗi fetch: {_backendText}");
        Assert.That(_backendSuccess, Is.Not.EqualTo(true), $"Backend xử lý thành công với ID không tồn tại: {_backendText}");

        AssertNoServerError();
    }

    [Then(@"ảnh thuốc trong bảng lô thuốc có đường dẫn src")]
    public void ThenAnhThuocTrongBangCoDuongDanSrc()
    {
        var rows = GetLotRows();

        foreach (var row in rows.Take(30))
        {
            Assert.That(row.Image, Is.Not.Null, $"Dòng không có ảnh thuốc: {row.SoLo}");

            var src = row.Image.GetAttribute("src") ?? "";
            Assert.That(src, Is.Not.Empty, $"Ảnh thuốc không có src: {row.SoLo}");
        }
    }

    [Then(@"không có ảnh thuốc bị vỡ trong bảng lô thuốc")]
    public void ThenKhongCoAnhThuocBiVoTrongBangLoThuoc()
    {
        var js = (IJavaScriptExecutor)_driver;

        var result = js.ExecuteScript(@"
            const imgs = Array.from(document.querySelectorAll('table tbody img'));
            return imgs
                .filter(img => img.complete && img.naturalWidth === 0)
                .map(img => img.getAttribute('src'));
        ");

        var broken = ((IEnumerable<object>)result).Select(x => x.ToString()).ToList();

        Assert.That(broken.Count, Is.EqualTo(0),
            "Có ảnh thuốc bị lỗi hoặc không tải được: " + string.Join(", ", broken));
    }

    [Then(@"trang Quản lý Lô thuốc không hiển thị số âm bất thường")]
    public void ThenTrangQuanLyLoThuocKhongHienThiSoAmBatThuong()
    {
        var page = BodyText();

        // Tránh bắt nhầm tên thuốc kiểu Omega 3-6-9.
        var negativeNumbers = Regex.Matches(page, @"(?<![\w])-[\s]*\d[\d,.]*")
                                   .Select(m => m.Value)
                                   .ToList();

        Assert.That(negativeNumbers.Count, Is.EqualTo(0),
            "Trang đang hiển thị số âm bất thường: " + string.Join(", ", negativeNumbers));
    }

    [Then(@"trang Quản lý Lô thuốc không bị tràn ngang nghiêm trọng")]
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
            $"Trang Quản lý Lô thuốc tràn ngang nghiêm trọng trên mobile. Overflow={overflow}");
    }

    [Then(@"lô thuốc bị xóa khỏi danh sách")]
    public void ThenLoThuocBiXoaKhoiDanhSach()
    {
        Assert.Ignore("Không kiểm tra xóa thật trong môi trường test để tránh thay đổi dữ liệu thật.");
    }

    [Then(@"lô thuốc chuyển tồn kho về 0, trạng thái Đã tiêu hủy và xuất hiện ở trang Quản lý lô tiêu hủy")]
    public void ThenLoThuocChuyenTonKhoVe0VaXuatHienOLoTieuHuy()
    {
        Assert.Ignore("Không kiểm tra tiêu hủy thật trong môi trường test để tránh thay đổi dữ liệu thật.");
    }

    // =========================
    // HÀM HỖ TRỢ
    // =========================

    private void OpenLoThuoc(string query = "")
    {
        LoginAdmin();

        _driver.Navigate().GoToUrl(FullUrl(LoThuocUrl + query));
        WaitReady();

        AssertNoServerError();
    }

    private void LoginAdmin()
    {
        _driver.Navigate().GoToUrl(FullUrl(LoginUrl));
        WaitReady();

        var page = BodyText();

        if (page.Contains("Xin chào, Admin"))
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
            var state = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")?.ToString();
            return state == "complete";
        });

        try
        {
            _wait.Until(d =>
            {
                var result = ((IJavaScriptExecutor)d).ExecuteScript(
                    "return window.jQuery ? jQuery.active === 0 : true;"
                );

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

    private List<LotRow> GetLotRows()
    {
        var tableRows = _driver.FindElements(By.CssSelector("table tbody tr"));
        var result = new List<LotRow>();

        foreach (var row in tableRows)
        {
            var cells = row.FindElements(By.CssSelector("td"));

            if (cells.Count < 7)
                continue;

            var buttons = row.FindElements(By.CssSelector("button")).ToList();
            var onclicks = buttons.Select(b => b.GetAttribute("onclick") ?? "").ToList();
            var imgs = row.FindElements(By.CssSelector("img"));

            result.Add(new LotRow
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
                TacVu = cells[6].Text.Trim(),
                RowClass = row.GetAttribute("class") ?? "",
                Buttons = buttons,
                OnClicks = onclicks,
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

    private void OverrideConfirmReturnFalse()
    {
        var js = (IJavaScriptExecutor)_driver;

        js.ExecuteScript(@"
            window._confirmCalled = false;
            window._confirmMessage = '';
            window.confirm = function(msg) {
                window._confirmCalled = true;
                window._confirmMessage = msg;
                return false;
            };
        ");
    }

    private void AssertConfirmWasShown(string keyword)
    {
        var js = (IJavaScriptExecutor)_driver;

        var called = Convert.ToBoolean(js.ExecuteScript("return window._confirmCalled === true;"));
        var message = js.ExecuteScript("return window._confirmMessage || '';")?.ToString() ?? "";

        Assert.That(called, Is.True, "Không hiển thị hộp xác nhận.");

        Assert.That(
            message.ToLower().Contains(keyword.ToLower()),
            Is.True,
            $"Nội dung hộp xác nhận không đúng. Actual={message}"
        );
    }

    private void SendBackendRequest(string endpoint, int id)
    {
        var js = (IJavaScriptExecutor)_driver;

        var script = @"
            const endpoint = arguments[0];
            const loId = arguments[1];
            const done = arguments[arguments.length - 1];

            fetch(endpoint, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: new URLSearchParams({ id: loId })
            })
            .then(async r => {
                const txt = await r.text();
                let successValue = null;

                try {
                    const json = JSON.parse(txt);
                    successValue = json.success;
                } catch(e) {}

                done({
                    ok: r.ok,
                    success: successValue,
                    text: txt.substring(0, 500)
                });
            })
            .catch(e => done({
                ok: false,
                success: null,
                text: String(e)
            }));
        ";

        var result = (Dictionary<string, object>)js.ExecuteAsyncScript(script, endpoint, id);

        _backendOk = result.ContainsKey("ok") && Convert.ToBoolean(result["ok"]);
        _backendText = result.ContainsKey("text") ? result["text"]?.ToString() ?? "" : "";

        if (result.ContainsKey("success") && result["success"] != null)
            _backendSuccess = Convert.ToBoolean(result["success"]);
        else
            _backendSuccess = null;
    }

    private class LotRow
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
        public string TacVu { get; set; }
        public string RowClass { get; set; }
        public List<IWebElement> Buttons { get; set; }
        public List<string> OnClicks { get; set; }
        public IWebElement Image { get; set; }
    }
}
