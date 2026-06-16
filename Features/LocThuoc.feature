Feature:  Lọc sản phẩm thuốc

  Background:
    Given Mở trang chủ nhà thuốc và chuyển sang trang Tất Cả sản phẩm

  @TC_TC01 @Severity_Medium @Priority_High
  Scenario: TC01 - Lọc sản phẩm thuốc theo khoảng giá bán
    When Người dùng chọn lọc theo mức giá thứ "3"
    And Người dùng chọn lọc theo mức giá thứ "4"
    And Người dùng chọn lọc theo mức giá thứ "5"
    Then Danh sách sản phẩm phải được cập nhật theo khoảng giá tương ứng

  @TC_TC02 @Severity_Medium @Priority_High
  Scenario: TC02 - Lọc sản phẩm thuốc theo Hãng sản xuất
    When Người dùng chọn hãng sản xuất là "DHG"
    And Người dùng chọn hãng sản xuất là "Bioderma"
    Then Danh sách sản phẩm phải hiển thị đúng các thuốc của hãng đã chọn

  @TC_TC03 @Severity_High @Priority_Medium
  Scenario: TC03 - Lọc kết hợp cả Giá bán và Hãng sản xuất
    When Người dùng chọn hãng sản xuất là "DHC Japan"
    And Người dùng chọn lọc theo mức giá thứ "3"
    Then Hệ thống trả về danh sách sản phẩm thỏa mãn đồng thời cả hai điều kiện

  @TC_TC04 @Severity_Low @Priority_Low
  Scenario: TC04 - Lọc các điều kiện không có kết quả phù hợp
    When Người dùng chọn hãng sản xuất là "Bà Giằng"
    And Người dùng chọn lọc theo mức giá thứ "4"
    Then Hệ thống phải thông báo không tìm thấy kết quả phù hợp

  @TC_TC05 @Severity_Medium @Priority_Medium
  Scenario: TC05 - Xóa bộ lọc về trạng thái mặc định ban đầu
    When Người dùng chọn hãng sản xuất là "Bà Giằng"
    And Người dùng chọn lọc theo mức giá thứ "4"
    And Người dùng nhấn vào liên kết "Xóa bộ lọc"
    Then Tất cả các bộ lọc phải được bỏ chọn và hiển thị đầy đủ danh sách thuốc

  @TC_TC06 @Severity_Low @Priority_Low
  Scenario: TC06 - Kiểm tra đồng bộ số lượng sản phẩm hiển thị sau khi lọc
    When Người dùng chọn lọc theo mức giá thứ "4"
    Then Số lượng tổng sản phẩm hiển thị phải khớp với bộ lọc dữ liệu