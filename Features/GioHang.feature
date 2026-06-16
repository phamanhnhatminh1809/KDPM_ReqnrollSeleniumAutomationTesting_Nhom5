Feature: Giỏ hàng
Kiểm thử chức năng Giỏ hàng của hệ thống MedForAll.
Các scenario trong file này được đồng bộ theo test case Selenium Python đã chạy.

@Cart @TC_CART_001 @Severity_LOW @Priority_LOW
Scenario: TC_CART_001 - Mở trang giỏ hàng
When User truy cập trang giỏ hàng
Then trang giỏ hàng mở được và không hiển thị lỗi hệ thống

@Cart @TC_CART_002 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_002 - Thêm thuốc còn hàng vào giỏ bằng Ajax
When User thêm thuốc còn hàng vào giỏ bằng Ajax
Then giỏ hàng hiển thị thuốc vừa thêm

@Cart @TC_CART_003 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_003 - Thêm trùng cùng một thuốc
Given trong giỏ hàng đã có thuốc còn hàng
When User thêm trùng cùng một thuốc vào giỏ
Then số lượng thuốc trong giỏ phải tăng lên

@Cart @TC_CART_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_004 - Giỏ hàng có nhiều sản phẩm
When User thêm nhiều thuốc khác nhau vào giỏ
Then giỏ hàng hiển thị đủ nhiều thuốc khác nhau

@Cart @TC_CART_005 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_005 - Kiểm tra tổng tiền bằng tổng thành tiền các dòng
Given trong giỏ hàng có nhiều thuốc
When User truy cập trang giỏ hàng
Then tổng tiền giỏ hàng phải bằng tổng thành tiền các dòng sản phẩm

@Cart @TC_CART_006 @Severity_LOW @Priority_LOW
Scenario: TC_CART_006 - Hover mini cart
Given trong giỏ hàng đã có thuốc còn hàng
When User hover vào mini cart
Then mini cart hiển thị danh sách sản phẩm

@Cart @TC_CART_007 @Severity_HIGH @Priority_MEDIUM
Scenario: TC_CART_007 - Nhấn tên thuốc trong mini cart
Given trong giỏ hàng đã có thuốc còn hàng
When User nhấn tên thuốc trong mini cart
Then hệ thống chuyển sang trang chi tiết thuốc và không hiển thị lỗi hệ thống

@Cart @TC_CART_008 @Severity_HIGH @Priority_MEDIUM
Scenario: TC_CART_008 - Nhấn tên thuốc trong trang giỏ hàng chi tiết
Given trong giỏ hàng đã có thuốc còn hàng
When User nhấn tên thuốc trong trang giỏ hàng chi tiết
Then hệ thống chuyển sang trang chi tiết thuốc và không hiển thị lỗi hệ thống

@Cart @TC_CART_009 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_009 - Tăng số lượng bằng nút cộng
Given trong giỏ hàng đã có thuốc còn hàng
When User tăng số lượng thuốc bằng nút cộng
Then số lượng thuốc tăng lên
And tổng tiền giỏ hàng tăng lên

@Cart @TC_CART_010 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_010 - Giảm số lượng bằng nút trừ
Given trong giỏ hàng đã có thuốc còn hàng với số lượng lớn hơn 1
When User giảm số lượng thuốc bằng nút trừ
Then số lượng thuốc giảm xuống
And tổng tiền giỏ hàng giảm xuống

@Cart @TC_CART_011 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_011 - Không cho giảm số lượng dưới 1
Given trong giỏ hàng đã có thuốc còn hàng với số lượng bằng 1
When User giảm số lượng thuốc bằng nút trừ
Then số lượng thuốc không được nhỏ hơn 1

@Cart @TC_CART_012 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_012 - Bấm cộng liên tục nhiều lần
Given trong giỏ hàng đã có thuốc còn hàng
When User bấm nút cộng liên tục nhiều lần
Then số lượng thuốc vẫn được cập nhật hợp lệ
And tổng tiền giỏ hàng vẫn hợp lệ

@Cart @TC_CART_013 @Severity_CRITICAL @Priority_HIGH
Scenario: TC_CART_013 - Không cho cập nhật số lượng vượt tồn kho
Given trong giỏ hàng đã có thuốc còn hàng
When User cập nhật số lượng thuốc vượt tồn kho bằng Ajax
Then hệ thống phải chặn số lượng vượt tồn kho

@Cart @TC_CART_014 @Severity_CRITICAL @Priority_HIGH
Scenario: TC_CART_014 - Không cho thêm thuốc hết hàng vào giỏ
When User thêm thuốc hết hàng vào giỏ bằng Ajax
Then hệ thống không được thêm thuốc hết hàng vào giỏ

@Cart @TC_CART_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_015 - Mở modal xóa rồi bấm Đóng
Given trong giỏ hàng đã có thuốc còn hàng
When User mở modal xóa sản phẩm rồi bấm Đóng
Then modal xóa được đóng lại
And sản phẩm không bị xóa khỏi giỏ hàng

@Cart @TC_CART_016 @Severity_MEDIUM @Priority_LOW @Skip_DataChanging
Scenario: TC_CART_016 - Xác nhận xóa sản phẩm khỏi giỏ
Given trong giỏ hàng đã có thuốc dùng để kiểm thử xóa
When User xác nhận xóa sản phẩm khỏi giỏ hàng
Then sản phẩm bị xóa khỏi giỏ hàng

@Cart @TC_CART_017 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_017 - Chuyển đổi Giao hàng và Nhận tại nhà thuốc
Given trong giỏ hàng đã có thuốc còn hàng
When User chuyển đổi giữa Giao hàng tận nơi và Nhận tại nhà thuốc
Then form giao hàng và form nhận tại nhà thuốc hiển thị đúng theo lựa chọn

@Cart @TC_CART_018 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_018 - Bắt buộc nhập họ tên
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng nhưng để trống họ tên
Then hệ thống hiển thị validation cho trường họ tên

@Cart @TC_CART_019 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_019 - Họ tên chỉ nhập khoảng trắng
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng với họ tên chỉ toàn khoảng trắng
Then hệ thống phải chặn họ tên không hợp lệ

@Cart @TC_CART_020 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_020 - Số điện thoại sai định dạng
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng với số điện thoại sai định dạng
Then hệ thống hiển thị validation cho trường số điện thoại

@Cart @TC_CART_021 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_021 - Email sai định dạng
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng với email sai định dạng
Then hệ thống hiển thị validation cho trường email

@Cart @TC_CART_022 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_022 - Giao hàng nhưng chưa chọn Tỉnh Thành
Given trong giỏ hàng đã có thuốc còn hàng
When User chọn giao hàng tận nơi nhưng chưa chọn Tỉnh Thành
Then hệ thống hiển thị validation cho trường Tỉnh Thành

@Cart @TC_CART_023 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_023 - Giao hàng nhưng chưa chọn Phường Xã
Given trong giỏ hàng đã có thuốc còn hàng
When User chọn giao hàng tận nơi nhưng chưa chọn Phường Xã
Then hệ thống hiển thị validation cho trường Phường Xã

@Cart @TC_CART_024 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_024 - Giao hàng nhưng chưa nhập số nhà
Given trong giỏ hàng đã có thuốc còn hàng
When User chọn giao hàng tận nơi nhưng chưa nhập số nhà tên đường
Then hệ thống hiển thị validation cho trường địa chỉ cụ thể

@Cart @TC_CART_025 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_025 - Nhận tại nhà thuốc nhưng chưa chọn nhà thuốc
Given trong giỏ hàng đã có thuốc còn hàng
When User chọn nhận tại nhà thuốc nhưng chưa chọn nhà thuốc
Then hệ thống hiển thị thông báo yêu cầu chọn nhà thuốc

@Cart @TC_CART_026 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_026 - Chọn thanh toán QR Code
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng bằng phương thức thanh toán QR Code
Then modal QR Code được hiển thị
And QR Code có thông tin thanh toán hợp lệ

@Cart @TC_CART_027 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_027 - Bấm Tôi đã chuyển tiền khi test QR
Given modal QR Code đang được hiển thị
When User bấm nút Tôi đã chuyển tiền
Then hệ thống hiển thị trạng thái kiểm tra thanh toán

@Cart @TC_CART_028 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_028 - Double click nút Đặt hàng ngay với QR
Given trong giỏ hàng đã có thuốc còn hàng
When User double click nút Đặt hàng ngay với QR
Then modal QR Code vẫn hiển thị hợp lệ
And hệ thống không hiển thị lỗi Server Error

@Cart @TC_CART_029 @Severity_LOW @Priority_LOW
Scenario: TC_CART_029 - Hover nút Mua thêm thuốc khác
Given trong giỏ hàng đã có thuốc còn hàng
When User hover nút Mua thêm thuốc khác
Then chữ trên nút vẫn dễ đọc sau khi hover

@Cart @TC_CART_030 @Severity_LOW @Priority_LOW
Scenario: TC_CART_030 - Responsive mobile màn hình 390px
Given trình duyệt đang ở kích thước màn hình mobile 390px
When User truy cập trang giỏ hàng
Then giao diện giỏ hàng không bị tràn ngang nghiêm trọng trên mobile

@Cart @TC_CART_031 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_031 - Nhập dữ liệu XSS vào họ tên
Given trong giỏ hàng đã có thuốc còn hàng
When User nhập dữ liệu XSS vào họ tên khi đặt hàng
Then hệ thống không được thực thi script XSS

@Cart @TC_CART_032 @Severity_HIGH @Priority_HIGH
Scenario: TC_CART_032 - Nhập chuỗi đặc biệt vào địa chỉ
Given trong giỏ hàng đã có thuốc còn hàng
When User nhập chuỗi đặc biệt giống SQL injection vào địa chỉ
Then hệ thống không hiển thị lỗi Server Error

@Cart @TC_CART_033 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_CART_033 - Đồng bộ số lượng khi mở 2 tab
Given trong giỏ hàng đã có thuốc còn hàng
When User cập nhật số lượng giỏ hàng ở tab thứ nhất
Then tab thứ hai sau khi reload phải hiển thị số lượng mới

@Cart @TC_CART_034 @Severity_CRITICAL @Priority_HIGH @Skip_DataChanging
Scenario: TC_CART_034 - Admin tắt bán online rồi user thêm vào giỏ
Given Admin đã tắt bán online cho thuốc kiểm thử
When User thêm thuốc đã bị tắt bán online vào giỏ
Then hệ thống không được thêm thuốc đã tắt bán online vào giỏ

@Cart @TC_CART_035 @Severity_MEDIUM @Priority_LOW @Skip_CreateOrder
Scenario: TC_CART_035 - Đặt hàng COD dữ liệu hợp lệ
Given trong giỏ hàng đã có thuốc còn hàng
When User đặt hàng COD với dữ liệu hợp lệ
Then hệ thống tạo đơn hàng thành công
