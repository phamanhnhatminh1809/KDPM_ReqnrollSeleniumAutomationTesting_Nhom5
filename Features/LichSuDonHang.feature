Feature:  Xem Lịch sử và Chi tiết đơn hàng

  @TC_TC_001 @Severity_High @Priority_High
  Scenario: TC01 - Hiển thị danh sách lịch sử đơn hàng của tôi thành công
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "tduc" và mật khẩu "123456"
    When Người dùng nhấp vào menu tài khoản và chọn "Đơn hàng của tôi"
    Then Hệ thống phải điều hướng sang trang lịch sử đơn hàng và hiển thị danh sách gồm 4 đơn hàng

  @TC_TC_002 @Severity_Medium @Priority_Medium
  Scenario: TC02 - Quan sát thứ tự dòng đơn hàng hiển thị mới nhất lên đầu
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "tduc" và mật khẩu "123456"
    When Người dùng nhấp vào menu tài khoản và chọn "Đơn hàng của tôi"
    Then Đơn hàng mới nhất phải xếp đầu tiên trong danh sách

  @TC_TC_006 @Severity_High @Priority_High
  Scenario: TC06 - Xem thông tin chi tiết một đơn hàng hợp lệ (Đơn #4)
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "tduc" và mật khẩu "123456"
    When Người dùng nhấp vào menu tài khoản và chọn "Đơn hàng của tôi"
    And Người dùng nhấn vào nút "Chi tiết" của mã đơn hàng "#4"
    Then Hệ thống phải hiển thị chi tiết đơn hàng thỏa mãn "Mã hiển thị đúng Đơn hàng #4, thời gian 12/06/2026, tổng tiền 25.000 đ"

  @TC_TC_007 @Severity_High @Priority_High
  Scenario: TC07 - Kiểm tra lỗi ẩn dòng sản phẩm thuốc đã mua (Đơn #5)
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "tduc" và mật khẩu "123456"
    When Người dùng nhấp vào menu tài khoản và chọn "Đơn hàng của tôi"
    And Người dùng nhấn vào nút "Chi tiết" của mã đơn hàng "#5"
    Then Hệ thống phải hiển thị cửa sổ hoặc trang thông tin chi tiết của đơn hàng đó và có sản phẩm

  @TC_TC_012 @Severity_High @Priority_High
  Scenario: TC12 - Kiểm tra chặn truy cập lịch sử đơn hàng khi chưa đăng nhập
    Given Người dùng chưa thực hiện đăng nhập vào hệ thống
    When Người dùng cố tình truy cập trực tiếp vào URL đường dẫn "/DonHang/LichSu"
    Then Hệ thống phải tự động chuyển hướng người dùng sang trang đăng nhập "/Account/Login"

  @TC_TC_013 @Severity_Critical @Priority_High
  Scenario: TC13 - Tấn công thay đổi URL ID đơn hàng không thuộc sở hữu (Id #1)
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "tduc" và mật khẩu "123456"
    When Người dùng cố tình thay đổi URL trình duyệt sang đường dẫn "/TaiKhoan/DonHang/ChiTiet/1"
    Then Hệ thống phải từ chối truy cập và trả về kết quả hoặc thông báo lỗi "403"

  @TC_TC_014 @Severity_Medium @Priority_Low
  Scenario: TC14 - Tài khoản mới chưa có lịch sử mua hàng
    Given Người dùng tiến hành đăng nhập vào hệ thống với tài khoản "test1" và mật khẩu "123456"
    When Người dùng nhấp vào menu tài khoản và chọn "Đơn hàng của tôi"
    Then Hệ thống phải hiển thị thông báo trống "Bạn chưa có đơn hàng nào"