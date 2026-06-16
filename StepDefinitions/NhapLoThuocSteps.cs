
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;

[Binding]
[Scope(Tag = "NhapLo")]
public class NhapLoThuocSteps
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    private const string BaseUrl = "http://localhost:44317";
    private const string LoginUrl = "/TaiKhoan/DangNhap";
    private const string ThuocAdminUrl = "/ThuocAdmin/Index";
    private const string NhapHangUrl = "/ThuocAdmin/NhapHang";

    private const int TestProductId = 1;
    private const string TestProductName = "Amoxicillin";
    private const int TestProductStock = 248;
    private const int TestProductPrice = 30000;

    [BeforeScenario("@NhapLo")]
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

    [AfterScenario("@NhapLo")]
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

    [When(@"Admin truy cập trang Danh sách thuốc")]
    public void WhenAdminTruyCapTrangDanhSachThuoc()
    {
        OpenThuocAdmin();
    }

    [When(@"Admin tìm kiếm thuốc Amoxicillin trước khi nhập lô")]
    public void WhenAdminTimKiemThuocAmoxicillinTruocKhiNhapLo()
    {
        OpenThuocAdmin("?searchString=Amoxicillin");
    }

    [When(@"Admin lọc danh sách thuốc theo danh mục Kháng sinh")]
    public void WhenAdminLocDanhSachThuocTheoDanhMucKhangSinh()
    {
        OpenThuocAdmin("?categoryId=1");
    }

    [When(@"Admin kiểm tra nút Nhập của thuốc Amoxicillin")]
    public void WhenAdminKiemTraNutNhapCuaThuocAmoxicillin()
    {
        OpenThuocAdmin();
    }

    [When(@"Admin bấm nút Nhập của thuốc Amoxicillin")]
    public void WhenAdminBamNutNhapCuaThuocAmoxicillin()
    {
        OpenThuocAdmin();

        var product = FindProductRow(TestProductName);
        Assert.That(product.NhapLink, Is.Not.Null, $"Không tìm thấy nút Nhập của thuốc {TestProductName}.");

        SafeClick(product.NhapLink);
        System.Threading.Thread.Sleep(1000);
        WaitReady();
    }

    [When(@"Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin")]
    public void WhenAdminMoTrangNhapLoHangMoiCuaThuocAmoxicillin()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin kiểm tra ràng buộc bắt buộc của field Số lô")]
    public void WhenAdminKiemTraRangBuocBatBuocCuaFieldSoLo()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin kiểm tra ràng buộc min của field Số lượng nhập")]
    public void WhenAdminKiemTraRangBuocMinCuaFieldSoLuongNhap()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin kiểm tra ràng buộc min của field Giá nhập")]
    public void WhenAdminKiemTraRangBuocMinCuaFieldGiaNhap()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin kiểm tra giá trị mặc định của field Số lượng nhập")]
    public void WhenAdminKiemTraGiaTriMacDinhCuaFieldSoLuongNhap()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin kiểm tra giá trị mặc định của field Giá nhập")]
    public void WhenAdminKiemTraGiaTriMacDinhCuaFieldGiaNhap()
    {
        OpenNhapHang(TestProductId);
    }

    [When(@"Admin nhập số lượng 5 và giá nhập 22000")]
    public void WhenAdminNhapSoLuong5VaGiaNhap22000()
    {
        OpenNhapHang(TestProductId);

        SetInputValue("#SoLuong", "5");
        SetInputValue("#GiaNhap", "22000");
    }

    [When(@"Admin nhập số lượng lớn và giá nhập lớn trên form nhập lô")]
    public void WhenAdminNhapSoLuongLonVaGiaNhapLonTrenFormNhapLo()
    {
        OpenNhapHang(TestProductId);

        SetInputValue("#SoLuong", "100");
        SetInputValue("#GiaNhap", "130000");
    }

    [When(@"Admin nhập giá nhập cao hơn giá bán hiện tại")]
    public void WhenAdminNhapGiaNhapCaoHonGiaBanHienTai()
    {
        OpenNhapHang(TestProductId);
        SetInputValue("#GiaNhap", "40000");
    }

    [When(@"Admin nhập giá nhập cao hơn giá bán rồi sửa lại thành giá hợp lệ")]
    public void WhenAdminNhapGiaNhapCaoHonGiaBanRoiSuaLaiThanhGiaHopLe()
    {
        OpenNhapHang(TestProductId);

        SetInputValue("#GiaNhap", "40000");
        System.Threading.Thread.Sleep(300);

        SetInputValue("#GiaNhap", "22000");
        System.Threading.Thread.Sleep(300);
    }

    [When(@"Admin bấm nút Hủy bỏ trên trang nhập lô")]
    public void WhenAdminBamNutHuyBoTrenTrangNhapLo()
    {
        OpenNhapHang(TestProductId);

        var cancelLinks = _driver.FindElements(By.XPath("//a[contains(., 'Hủy bỏ') or contains(., 'Huỷ bỏ')]"));
        Assert.That(cancelLinks.Count, Is.GreaterThan(0), "Không tìm thấy nút Hủy bỏ.");

        SafeClick(cancelLinks[0]);
        System.Threading.Thread.Sleep(1000);
        WaitReady();
    }

    [When(@"Admin mở trang Nhập lô hàng mới với ID thuốc không tồn tại")]
    public void WhenAdminMoTrangNhapLoHangMoiVoiIdThuocKhongTonTai()
    {
        _driver.Navigate().GoToUrl(FullUrl($"{NhapHangUrl}/999999"));
        WaitReady();
    }

    [When(@"Admin nhập dữ liệu hợp lệ trên trang nhập lô")]
    public void WhenAdminNhapDuLieuHopLeTrenTrangNhapLo()
    {
        OpenNhapHang(TestProductId);

        SetInputValue("#SoLuong", "5");
        SetInputValue("#GiaNhap", "22000");
    }

    [When(@"Admin mở trang Nhập lô hàng mới trên màn hình mobile")]
    public void WhenAdminMoTrangNhapLoHangMoiTrenManHinhMobile()
    {
        _driver.Manage().Window.Size = new System.Drawing.Size(390, 844);
        System.Threading.Thread.Sleep(500);

        OpenNhapHang(TestProductId);
    }

    [When(@"Admin submit nhập kho thật với dữ liệu hợp lệ")]
    public void WhenAdminSubmitNhapKhoThatVoiDuLieuHopLe()
    {
        Assert.Ignore("Test nhập kho thật đang được bỏ qua để tránh thay đổi dữ liệu thật.");
    }

    // =========================
    // THEN
    // =========================

    [Then(@"trang Danh sách thuốc mở được, có nút Thêm thuốc mới, có nút Nhập và không hiển thị lỗi hệ thống")]
    public void ThenTrangDanhSachThuocMoDuocCoNutThemVaNutNhap()
    {
        var page = BodyText();

        Assert.That(page, Does.Contain("Danh sách thuốc"), "Không thấy tiêu đề Danh sách thuốc.");
        Assert.That(page, Does.Contain("Thêm thuốc mới"), "Không thấy nút Thêm thuốc mới.");
        Assert.That(page, Does.Contain("Nhập"), "Không thấy nút Nhập lô thuốc.");

        AssertNoServerError();
    }

    [Then(@"menu Danh sách thuốc trên sidebar được active đúng")]
    public void ThenMenuDanhSachThuocTrenSidebarDuocActiveDung()
    {
        var activeMenus = _driver.FindElements(By.CssSelector(".sidebar a.active"));

        Assert.That(activeMenus.Count, Is.GreaterThan(0), "Không có menu active trong sidebar.");
        Assert.That(activeMenus[0].Text, Does.Contain("Danh sách thuốc"),
            $"Menu active không đúng. Active hiện tại: {activeMenus[0].Text}");
    }

    [Then(@"bảng Danh sách thuốc hiển thị đủ các cột Ảnh, Tên thuốc, Danh mục, Tồn kho, Giá bán, Trạng thái và Hành động")]
    public void ThenBangDanhSachThuocHienThiDuCot()
    {
        var page = BodyText();

        string[] headers =
        {
            "Ảnh",
            "Tên thuốc / Hoạt chất",
            "Danh mục",
            "Tồn kho",
            "Giá bán",
            "Trạng thái",
            "Hành động"
        };

        foreach (var header in headers)
        {
            Assert.That(page, Does.Contain(header), $"Bảng Danh sách thuốc thiếu cột: {header}");
        }
    }

    [Then(@"danh sách thuốc hiển thị dữ liệu hợp lệ và mỗi dòng thuốc có nút Nhập")]
    public void ThenDanhSachThuocHienThiDuLieuHopLeVaCoNutNhap()
    {
        var rows = GetProductRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Bảng Danh sách thuốc không có dữ liệu.");

        foreach (var row in rows.Take(20))
        {
            Assert.That(row.Name, Is.Not.Empty, $"Dòng thiếu tên thuốc: {row.Text}");
            Assert.That(row.Category, Is.Not.Empty, $"Dòng thiếu danh mục: {row.Text}");
            Assert.That(row.Stock, Is.Not.Empty, $"Dòng thiếu tồn kho: {row.Text}");
            Assert.That(NumberFromText(row.Price), Is.GreaterThan(0), $"Giá bán không hợp lệ: {row.Text}");
            Assert.That(row.NhapLink, Is.Not.Null, $"Dòng thuốc thiếu nút Nhập: {row.Text}");
        }
    }

    [Then(@"kết quả tìm kiếm chỉ hiển thị thuốc phù hợp với từ khóa Amoxicillin")]
    public void ThenKetQuaTimKiemChiHienThiThuocPhuHop()
    {
        var rows = GetProductRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có kết quả tìm kiếm.");

        foreach (var row in rows)
        {
            Assert.That(row.Text.ToLower(), Does.Contain(TestProductName.ToLower()),
                $"Tìm kiếm {TestProductName} nhưng có dòng không phù hợp: {row.Text}");
        }
    }

    [Then(@"danh sách chỉ hiển thị các thuốc thuộc danh mục Kháng sinh")]
    public void ThenDanhSachChiHienThiThuocThuocDanhMucKhangSinh()
    {
        var rows = GetProductRows();

        Assert.That(rows.Count, Is.GreaterThan(0), "Không có dữ liệu sau khi lọc danh mục.");

        foreach (var row in rows)
        {
            Assert.That(row.Category, Does.Contain("Kháng sinh"),
                $"Lọc Kháng sinh nhưng có dòng sai danh mục: {row.Text}");
        }
    }

    [Then(@"nút Nhập trỏ đúng đến trang nhập hàng của thuốc Amoxicillin")]
    public void ThenNutNhapTroDungDenTrangNhapHangCuaThuocAmoxicillin()
    {
        var product = FindProductRow(TestProductName);

        Assert.That(product.NhapLink, Is.Not.Null, $"Không tìm thấy nút Nhập của thuốc {TestProductName}.");

        var href = product.NhapLink.GetAttribute("href") ?? "";
        var expected = $"/ThuocAdmin/NhapHang/{TestProductId}";

        Assert.That(href, Does.Contain(expected),
            $"Nút Nhập trỏ sai link. Expected chứa {expected}, Actual={href}");
    }

    [Then(@"hệ thống chuyển sang màn hình Nhập lô hàng mới của thuốc Amoxicillin")]
    public void ThenHeThongChuyenSangManHinhNhapLoHangMoi()
    {
        var currentUrl = _driver.Url;
        var page = BodyText();

        Assert.That(currentUrl, Does.Contain($"/ThuocAdmin/NhapHang/{TestProductId}"),
            $"Bấm Nhập nhưng không chuyển đúng trang nhập hàng. URL={currentUrl}");

        Assert.That(page, Does.Contain("Nhập lô hàng mới"),
            "Không thấy tiêu đề Nhập lô hàng mới.");
    }

    [Then(@"trang nhập kho hiển thị đúng tên thuốc, đơn vị tính, tồn kho hiện tại và giá bán hiện tại")]
    public void ThenTrangNhapKhoHienThiDungThongTinThuoc()
    {
        var page = BodyText();

        Assert.That(page, Does.Contain(TestProductName),
            $"Trang nhập kho không hiển thị tên thuốc {TestProductName}.");

        Assert.That(page, Does.Contain("ĐVT: Hộp"),
            "Trang nhập kho không hiển thị đơn vị tính của thuốc.");

        Assert.That(page, Does.Contain("Tồn kho hiện tại"),
            "Trang nhập kho không hiển thị nhãn tồn kho hiện tại.");

        Assert.That(page, Does.Contain(TestProductStock.ToString()),
            "Trang nhập kho không hiển thị đúng tồn kho hiện tại.");

        Assert.That(page, Does.Contain("Giá bán hiện tại"),
            "Trang nhập kho không hiển thị nhãn giá bán hiện tại.");

        Assert.That(page, Does.Contain("30,000 đ"),
            "Trang nhập kho không hiển thị đúng giá bán hiện tại.");
    }

    [Then(@"các hidden field của thuốc chứa đúng IdThuoc, TenThuoc, DonViTinh, GiaBanHienTai và TongTonKho")]
    public void ThenCacHiddenFieldCuaThuocChuaDungGiaTri()
    {
        var expectedValues = new Dictionary<string, string>
        {
            { "#IdThuoc", TestProductId.ToString() },
            { "#TenThuoc", TestProductName },
            { "#DonViTinh", "Hộp" },
            { "#GiaBanHienTai", TestProductPrice.ToString() },
            { "#TongTonKho", TestProductStock.ToString() }
        };

        foreach (var item in expectedValues)
        {
            var actual = GetInputValue(item.Key);

            Assert.That(actual, Is.EqualTo(item.Value),
                $"Hidden field {item.Key} sai. Expected={item.Value}, Actual={actual}");
        }
    }

    [Then(@"form nhập lô hiển thị đủ các field Số lô, Ngày sản xuất, Hạn sử dụng, Số lượng nhập, Giá nhập và Thành tiền")]
    public void ThenFormNhapLoHienThiDuField()
    {
        string[] requiredFields =
        {
            "#SoLo",
            "#NgaySanXuat",
            "#HanSuDung",
            "#SoLuong",
            "#GiaNhap",
            "#ThanhTien"
        };

        foreach (var css in requiredFields)
        {
            var elements = _driver.FindElements(By.CssSelector(css));
            Assert.That(elements.Count, Is.GreaterThan(0), $"Thiếu field {css} trên form nhập lô.");
        }
    }

    [Then(@"các field nhập lô có thuộc tính name đúng để gửi POST")]
    public void ThenCacFieldNhapLoCoNameDung()
    {
        var expectedNames = new Dictionary<string, string>
        {
            { "#SoLo", "SoLo" },
            { "#NgaySanXuat", "NgaySanXuat" },
            { "#HanSuDung", "HanSuDung" },
            { "#SoLuong", "SoLuongNhap" },
            { "#GiaNhap", "GiaNhap" }
        };

        foreach (var item in expectedNames)
        {
            var element = WaitPresent(item.Key);
            var actualName = element.GetAttribute("name") ?? "";

            Assert.That(actualName, Is.EqualTo(item.Value),
                $"Field {item.Key} có name sai. Expected={item.Value}, Actual={actualName}");
        }
    }

    [Then(@"field Số lô có cấu hình validate bắt buộc và thông báo lỗi phù hợp")]
    public void ThenFieldSoLoCoValidateBatBuoc()
    {
        var soLo = WaitPresent("#SoLo");

        var dataVal = soLo.GetAttribute("data-val") ?? "";
        var requiredMsg = soLo.GetAttribute("data-val-required") ?? "";

        Assert.That(dataVal, Is.EqualTo("true"), "Số lô chưa bật data-val=true.");
        Assert.That(requiredMsg, Does.Contain("Vui lòng nhập số lô"),
            $"Số lô thiếu message bắt buộc. Actual={requiredMsg}");
    }

    [Then(@"field Số lượng nhập phải có min bằng 1")]
    public void ThenFieldSoLuongNhapPhaiCoMinBang1()
    {
        var soLuong = WaitPresent("#SoLuong");

        var minValue = soLuong.GetAttribute("min") ?? "";
        var dataMin = soLuong.GetAttribute("data-val-range-min") ?? "";

        Assert.That(minValue, Is.EqualTo("1"), $"Số lượng nhập phải có min=1. Actual min={minValue}");
        Assert.That(dataMin, Is.EqualTo("1"), $"Số lượng nhập phải có data-val-range-min=1. Actual={dataMin}");
    }

    [Then(@"field Giá nhập phải có min bằng 1 vì giá nhập phải lớn hơn 0")]
    public void ThenFieldGiaNhapPhaiCoMinBang1()
    {
        var giaNhap = WaitPresent("#GiaNhap");

        var minValue = giaNhap.GetAttribute("min") ?? "";
        var dataMin = giaNhap.GetAttribute("data-val-range-min") ?? "";

        Assert.That(minValue, Is.EqualTo("1"),
            $"Giá nhập phải có min=1 theo nghiệp vụ > 0. Actual min={minValue}");

        Assert.That(dataMin, Is.EqualTo("1"),
            $"Giá nhập phải có data-val-range-min=1 theo nghiệp vụ > 0. Actual={dataMin}");
    }

    [Then(@"giá trị mặc định của Số lượng nhập không được nhỏ hơn min")]
    public void ThenGiaTriMacDinhSoLuongKhongNhoHonMin()
    {
        var soLuong = WaitPresent("#SoLuong");

        var value = soLuong.GetAttribute("value") ?? "";
        var minValue = soLuong.GetAttribute("min") ?? "1";

        if (!string.IsNullOrWhiteSpace(value))
        {
            Assert.That(int.Parse(value), Is.GreaterThanOrEqualTo(int.Parse(minValue)),
                $"Số lượng nhập mặc định đang nhỏ hơn min. value={value}, min={minValue}");
        }
    }

    [Then(@"giá trị mặc định của Giá nhập không được nhỏ hơn min")]
    public void ThenGiaTriMacDinhGiaNhapKhongNhoHonMin()
    {
        var giaNhap = WaitPresent("#GiaNhap");

        var value = giaNhap.GetAttribute("value") ?? "";
        var minValue = giaNhap.GetAttribute("min") ?? "1";

        if (!string.IsNullOrWhiteSpace(value))
        {
            Assert.That(int.Parse(value), Is.GreaterThanOrEqualTo(int.Parse(minValue)),
                $"Giá nhập mặc định đang nhỏ hơn min. value={value}, min={minValue}");
        }
    }

    [Then(@"hệ thống tự động tính đúng tổng giá trị lô hàng")]
    public void ThenHeThongTuDongTinhDungTongGiaTriLoHang()
    {
        var totalText = WaitPresent("#ThanhTien").Text.Trim();
        var totalNumber = NumberFromText(totalText);
        var expected = 5 * 22000;

        Assert.That(totalNumber, Is.EqualTo(expected),
            $"Tính tổng giá trị lô hàng sai. Expected={expected}, Actual={totalText}");
    }

    [Then(@"hệ thống vẫn tính đúng tổng giá trị lô hàng")]
    public void ThenHeThongVanTinhDungTongGiaTriLoHang()
    {
        var totalText = WaitPresent("#ThanhTien").Text.Trim();
        var totalNumber = NumberFromText(totalText);
        var expected = 100 * 130000;

        Assert.That(totalNumber, Is.EqualTo(expected),
            $"Tính tổng giá trị lô hàng lớn sai. Expected={expected}, Actual={totalText}");
    }

    [Then(@"hệ thống hiển thị cảnh báo giá nhập cao hơn giá bán")]
    public void ThenHeThongHienThiCanhBaoGiaNhapCaoHonGiaBan()
    {
        var warnings = _driver.FindElements(By.CssSelector("#warning-price"));
        var giaNhap = WaitPresent("#GiaNhap");

        Assert.That(warnings.Count, Is.GreaterThan(0),
            "Giá nhập cao hơn giá bán nhưng không hiển thị cảnh báo.");

        Assert.That(giaNhap.GetAttribute("class") ?? "", Does.Contain("is-invalid"),
            "Giá nhập cao hơn giá bán nhưng input không có class is-invalid.");

        Assert.That(warnings[0].Text, Does.Contain("Giá nhập đang cao hơn giá bán"),
            $"Nội dung cảnh báo giá nhập không đúng: {warnings[0].Text}");
    }

    [Then(@"cảnh báo giá nhập cao hơn giá bán phải biến mất")]
    public void ThenCanhBaoGiaNhapCaoHonGiaBanBienMat()
    {
        var warnings = _driver.FindElements(By.CssSelector("#warning-price"));
        var giaNhap = WaitPresent("#GiaNhap");

        Assert.That(warnings.Count, Is.EqualTo(0),
            "Giá nhập đã hợp lệ nhưng cảnh báo vẫn còn hiển thị.");

        Assert.That(giaNhap.GetAttribute("class") ?? "", Does.Not.Contain("is-invalid"),
            "Giá nhập đã hợp lệ nhưng input vẫn còn class is-invalid.");
    }

    [Then(@"field Ngày sản xuất và Hạn sử dụng phải là input type date")]
    public void ThenFieldNgaySanXuatVaHanSuDungLaTypeDate()
    {
        var nsx = WaitPresent("#NgaySanXuat");
        var hsd = WaitPresent("#HanSuDung");

        Assert.That(nsx.GetAttribute("type"), Is.EqualTo("date"),
            "Ngày sản xuất không phải input type=date.");

        Assert.That(hsd.GetAttribute("type"), Is.EqualTo("date"),
            "Hạn sử dụng không phải input type=date.");
    }

    [Then(@"giá trị của input date phải có format yyyy-MM-dd hoặc để rỗng")]
    public void ThenGiaTriInputDateDungFormatHoacRong()
    {
        var nsxValue = GetInputValue("#NgaySanXuat");
        var hsdValue = GetInputValue("#HanSuDung");

        var pattern = @"^\d{4}-\d{2}-\d{2}$";

        if (!string.IsNullOrWhiteSpace(nsxValue))
        {
            Assert.That(Regex.IsMatch(nsxValue, pattern), Is.True,
                $"Ngày sản xuất không đúng format yyyy-MM-dd: {nsxValue}");
        }

        if (!string.IsNullOrWhiteSpace(hsdValue))
        {
            Assert.That(Regex.IsMatch(hsdValue, pattern), Is.True,
                $"Hạn sử dụng không đúng format yyyy-MM-dd: {hsdValue}");
        }
    }

    [Then(@"nút Hủy bỏ trỏ đúng về trang Danh sách thuốc")]
    public void ThenNutHuyBoTroDungVeTrangDanhSachThuoc()
    {
        var cancelLinks = _driver.FindElements(By.XPath("//a[contains(., 'Hủy bỏ') or contains(., 'Huỷ bỏ')]"));

        Assert.That(cancelLinks.Count, Is.GreaterThan(0), "Không tìm thấy nút Hủy bỏ.");

        var href = cancelLinks[0].GetAttribute("href") ?? "";

        Assert.That(href, Does.Contain("/ThuocAdmin/Index"),
            $"Nút Hủy bỏ trỏ sai trang. Actual={href}");
    }

    [Then(@"hệ thống quay về trang Danh sách thuốc và không lưu dữ liệu nhập lô")]
    public void ThenHeThongQuayVeTrangDanhSachThuocVaKhongLuuDuLieuNhapLo()
    {
        Assert.That(_driver.Url, Does.Contain("/ThuocAdmin/Index"),
            $"Bấm Hủy bỏ nhưng không quay về Danh sách thuốc. URL={_driver.Url}");

        Assert.That(BodyText(), Does.Contain("Danh sách thuốc"),
            "Bấm Hủy bỏ nhưng trang hiện tại không phải Danh sách thuốc.");
    }

    [Then(@"form nhập kho gửi POST đúng action của thuốc Amoxicillin")]
    public void ThenFormNhapKhoGuiPostDungAction()
    {
        var form = WaitPresent("form");

        var action = form.GetAttribute("action") ?? "";
        var method = form.GetAttribute("method") ?? "";

        Assert.That(action, Does.Contain($"/ThuocAdmin/NhapHang/{TestProductId}"),
            $"Form nhập kho có action sai. Actual={action}");

        Assert.That(method.ToLower(), Is.EqualTo("post"),
            $"Form nhập kho phải dùng POST. Actual={method}");
    }

    [Then(@"form nhập kho có Anti-forgery token hợp lệ")]
    public void ThenFormNhapKhoCoAntiForgeryTokenHopLe()
    {
        var tokens = _driver.FindElements(By.CssSelector("input[name='__RequestVerificationToken']"));

        Assert.That(tokens.Count, Is.GreaterThan(0), "Form nhập kho thiếu __RequestVerificationToken.");

        var value = tokens[0].GetAttribute("value") ?? "";

        Assert.That(value, Is.Not.Empty, "__RequestVerificationToken bị rỗng.");
    }

    [Then(@"hệ thống không được phát sinh lỗi hệ thống")]
    public void ThenHeThongKhongDuocPhatSinhLoiHeThong()
    {
        AssertNoServerError();
    }

    [Then(@"ảnh thuốc ở trang nhập kho tải được và không bị vỡ")]
    public void ThenAnhThuocOTrangNhapKhoTaiDuocVaKhongBiVo()
    {
        var js = (IJavaScriptExecutor)_driver;

        var result = js.ExecuteScript(@"
            const imgs = Array.from(document.querySelectorAll('img'));
            return imgs
                .filter(img => img.complete && img.naturalWidth === 0)
                .map(img => img.getAttribute('src'));
        ");

        var broken = ((IEnumerable<object>)result)
            .Select(x => x?.ToString() ?? "")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        Assert.That(broken.Count, Is.EqualTo(0),
            "Ảnh ở trang nhập kho bị lỗi hoặc không tải được: " + string.Join(", ", broken));
    }

    [Then(@"trang nhập kho không hiển thị số âm bất thường")]
    public void ThenTrangNhapKhoKhongHienThiSoAmBatThuong()
    {
        var page = BodyText();

        var negativeNumbers = Regex.Matches(page, @"(?<![\w])-[\s]*\d[\d,.]*")
                                   .Select(m => m.Value)
                                   .ToList();

        Assert.That(negativeNumbers.Count, Is.EqualTo(0),
            "Trang nhập kho hiển thị số âm bất thường: " + string.Join(", ", negativeNumbers));
    }

    [Then(@"trang nhập kho không bị tràn ngang nghiêm trọng")]
    public void ThenTrangNhapKhoKhongBiTranNgangNghiemTrong()
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
            $"Trang Nhập kho tràn ngang nghiêm trọng trên mobile. Overflow={overflow}");
    }

    [Then(@"hệ thống tạo lô thuốc mới và lô xuất hiện trong danh sách lô thuốc")]
    public void ThenHeThongTaoLoThuocMoiVaLoXuatHien()
    {
        Assert.Ignore("Không kiểm tra nhập kho thật trong môi trường test để tránh thay đổi dữ liệu thật.");
    }

    // =========================
    // HÀM HỖ TRỢ
    // =========================

    private void OpenThuocAdmin(string query = "")
    {
        LoginAdmin();

        _driver.Navigate().GoToUrl(FullUrl(ThuocAdminUrl + query));
        WaitReady();

        AssertNoServerError();
    }

    private void OpenNhapHang(int productId)
    {
        LoginAdmin();

        _driver.Navigate().GoToUrl(FullUrl($"{NhapHangUrl}/{productId}"));
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

    private List<ProductRow> GetProductRows()
    {
        var tableRows = _driver.FindElements(By.CssSelector("table tbody tr"));
        var result = new List<ProductRow>();

        foreach (var row in tableRows)
        {
            var cells = row.FindElements(By.CssSelector("td"));

            if (cells.Count < 7)
                continue;

            var links = row.FindElements(By.CssSelector("a")).ToList();
            var buttons = row.FindElements(By.CssSelector("button")).ToList();

            IWebElement nhapLink = null;

            foreach (var link in links)
            {
                var href = link.GetAttribute("href") ?? "";
                var text = link.Text.Trim();

                if (href.Contains("/ThuocAdmin/NhapHang/") || text.Contains("Nhập"))
                {
                    nhapLink = link;
                    break;
                }
            }

            result.Add(new ProductRow
            {
                Element = row,
                Text = row.Text.Trim(),
                Name = cells[1].Text.Trim(),
                Category = cells[2].Text.Trim(),
                Stock = cells[3].Text.Trim(),
                Price = cells[4].Text.Trim(),
                Status = cells[5].Text.Trim(),
                Actions = cells[6].Text.Trim(),
                Links = links,
                Buttons = buttons,
                NhapLink = nhapLink
            });
        }

        return result;
    }

    private ProductRow FindProductRow(string productName)
    {
        var rows = GetProductRows();

        foreach (var row in rows)
        {
            if (row.Text.Contains(productName, StringComparison.OrdinalIgnoreCase))
                return row;
        }

        throw new AssertionException($"Không tìm thấy thuốc {productName} trong danh sách.");
    }

    private IWebElement WaitPresent(string css)
    {
        return _wait.Until(d => d.FindElement(By.CssSelector(css)));
    }

    private string GetInputValue(string css)
    {
        return WaitPresent(css).GetAttribute("value") ?? "";
    }

    private void SetInputValue(string css, string value)
    {
        var element = WaitPresent(css);

        var js = (IJavaScriptExecutor)_driver;

        js.ExecuteScript(@"
            arguments[0].value = arguments[1];
            arguments[0].dispatchEvent(new Event('input', { bubbles: true }));
            arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
        ", element, value);

        System.Threading.Thread.Sleep(300);
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

    private class ProductRow
    {
        public IWebElement Element { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Stock { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string Actions { get; set; }
        public List<IWebElement> Links { get; set; }
        public List<IWebElement> Buttons { get; set; }
        public IWebElement NhapLink { get; set; }
    }
}

