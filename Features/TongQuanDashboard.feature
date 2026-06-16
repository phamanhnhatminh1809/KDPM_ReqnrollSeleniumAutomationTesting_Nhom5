
Feature: Tổng quan Dashboard
  Kiểm thử chức năng Tổng quan/Dashboard của hệ thống MedForAll.
  Các scenario trong file này được đồng bộ theo test case Selenium Python đã chạy.

Background:
  Given Admin đã đăng nhập vào hệ thống MedForAll

@Dashboard @TC_DASH_001 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_001 - Mở trang Tổng quan Dashboard
  When Admin truy cập trang Tổng quan Dashboard
  Then trang Dashboard mở được và không hiển thị lỗi hệ thống
  And trang Dashboard hiển thị tiêu đề Dashboard hoặc Tổng quan

@Dashboard @TC_DASH_002 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_002 - Kiểm tra sidebar menu Admin
  When Admin truy cập trang Tổng quan Dashboard
  Then sidebar hiển thị đủ các menu quản trị
  And menu Tổng quan đang ở trạng thái active

@Dashboard @TC_DASH_003 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_003 - Kiểm tra 4 thẻ số liệu tổng quan
  When Admin truy cập trang Tổng quan Dashboard
  Then Dashboard hiển thị thẻ Doanh thu hôm nay
  And Dashboard hiển thị thẻ Đơn hàng mới
  And Dashboard hiển thị thẻ Lô sắp hết hạn
  And Dashboard hiển thị thẻ Thuốc hết hàng

@Dashboard @TC_DASH_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_004 - Kiểm tra giá trị các thẻ tổng quan không âm
  When Admin truy cập trang Tổng quan Dashboard
  Then giá trị Doanh thu hôm nay không được âm
  And giá trị Đơn hàng mới không được âm
  And giá trị Lô sắp hết hạn không được âm
  And giá trị Thuốc hết hàng không được âm

@Dashboard @TC_DASH_005 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_005 - Kiểm tra biểu đồ doanh thu 7 ngày
  When Admin truy cập trang Tổng quan Dashboard
  Then biểu đồ doanh thu được render thành công
  And biểu đồ doanh thu có đủ 7 nhãn ngày
  And biểu đồ doanh thu có đủ 7 giá trị doanh thu

@Dashboard @TC_DASH_006 @Severity_HIGH @Priority_HIGH
Scenario: TC_DASH_006 - Đối chiếu doanh thu hôm nay với ngày cuối của biểu đồ
  When Admin truy cập trang Tổng quan Dashboard
  Then giá trị Doanh thu hôm nay phải bằng giá trị ngày cuối trong biểu đồ doanh thu

@Dashboard @TC_DASH_007 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_007 - Kiểm tra khu vực Top 5 Bán Chạy
  When Admin truy cập trang Tổng quan Dashboard
  Then khu vực Top 5 Bán Chạy được hiển thị
  And danh sách Top 5 Bán Chạy có sản phẩm
  And mỗi sản phẩm bán chạy có thông tin số lượng đã bán

@Dashboard @TC_DASH_008 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_008 - Kiểm tra bảng Giao dịch gần đây
  When Admin truy cập trang Tổng quan Dashboard
  Then bảng Giao dịch gần đây được hiển thị
  And bảng Giao dịch gần đây có dữ liệu
  And mỗi dòng giao dịch có mã hóa đơn, người bán, người nhận, thời gian, tổng tiền và trạng thái

@Dashboard @TC_DASH_009 @Severity_HIGH @Priority_HIGH
Scenario: TC_DASH_009 - Đối chiếu doanh thu hôm nay với bảng giao dịch
  When Admin truy cập trang Tổng quan Dashboard
  Then Doanh thu hôm nay phải bằng tổng tiền các giao dịch trong ngày hiện tại

@Dashboard @TC_DASH_010 @Severity_HIGH @Priority_HIGH
Scenario: TC_DASH_010 - Đối chiếu đơn hàng mới với bảng giao dịch
  When Admin truy cập trang Tổng quan Dashboard
  Then số Đơn hàng mới phải bằng số giao dịch trong ngày hiện tại

@Dashboard @TC_DASH_011 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_011 - Kiểm tra nút In Báo Cáo Nhanh
  When Admin truy cập trang Tổng quan Dashboard
  And Admin nhấn nút In Báo Cáo Nhanh
  Then hệ thống gọi chức năng in báo cáo
  And biểu đồ được chuyển thành ảnh trong vùng in báo cáo

@Dashboard @TC_DASH_012 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_012 - Kiểm tra nội dung vùng in báo cáo
  When Admin truy cập trang Tổng quan Dashboard
  Then vùng in báo cáo có tiêu đề báo cáo hoạt động trong ngày
  And vùng in báo cáo có phần tổng hợp số liệu hôm nay
  And vùng in báo cáo có biểu đồ doanh thu 7 ngày qua
  And vùng in báo cáo có Top 5 thuốc bán chạy nhất

@Dashboard @TC_DASH_013 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_013 - Kiểm tra nút Thoát
  When Admin truy cập trang Tổng quan Dashboard
  Then nút Thoát hiển thị trên giao diện Dashboard
  And nút Thoát có đường dẫn hợp lệ

@Dashboard @TC_DASH_014 @Severity_LOW @Priority_LOW
Scenario: TC_DASH_014 - Kiểm tra responsive Dashboard trên mobile
  Given trình duyệt đang ở kích thước màn hình mobile 390px
  When Admin truy cập trang Tổng quan Dashboard
  Then Dashboard không bị tràn ngang nghiêm trọng trên màn hình nhỏ

@Dashboard @TC_DASH_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_015 - Kiểm tra Dashboard không hiển thị tiền âm
  When Admin truy cập trang Tổng quan Dashboard
  Then Dashboard không được hiển thị số tiền âm

@Dashboard @TC_DASH_016 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_DASH_016 - Kiểm tra người nhận trong giao dịch gần đây không rỗng
  When Admin truy cập trang Tổng quan Dashboard
  Then các dòng giao dịch gần đây phải có thông tin người nhận

