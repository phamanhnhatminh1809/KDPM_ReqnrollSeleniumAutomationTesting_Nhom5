Feature: Quản lý thuốc bán online
Kiểm thử chức năng Quản lý thuốc bán online của hệ thống MedForAll.
Các scenario trong file này được đồng bộ theo test case Selenium Python đã chạy.

Background:
Given Admin đã đăng nhập vào hệ thống MedForAll

@Online @TC_ONLINE_001 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_001 - Mở trang Quản lý thuốc bán online
When Admin truy cập trang Quản lý thuốc bán online
Then trang Quản lý thuốc bán online mở được và không hiển thị lỗi hệ thống
And trang hiển thị tiêu đề Quản lý thuốc bán online
And trang hiển thị cột hoặc trạng thái Cho phép bán online

@Online @TC_ONLINE_002 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_002 - Kiểm tra sidebar active
When Admin truy cập trang Quản lý thuốc bán online
Then menu Quản lý thuốc bán online đang ở trạng thái active

@Online @TC_ONLINE_003 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_003 - Kiểm tra tiêu đề cột bảng
When Admin truy cập trang Quản lý thuốc bán online
Then bảng thuốc online hiển thị đủ các cột cần thiết

@Online @TC_ONLINE_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_004 - Kiểm tra bảng có dữ liệu thuốc
When Admin truy cập trang Quản lý thuốc bán online
Then bảng thuốc online có dữ liệu thuốc hợp lệ

@Online @TC_ONLINE_005 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_005 - Tìm kiếm thuốc theo tên chính xác
When Admin tìm kiếm thuốc online với từ khóa "Amoxicillin"
Then kết quả tìm kiếm chỉ hiển thị thuốc phù hợp với từ khóa "Amoxicillin"

@Online @TC_ONLINE_006 @Severity_LOW @Priority_MEDIUM
Scenario: TC_ONLINE_006 - Tìm kiếm thuốc theo từ khóa ngắn
When Admin tìm kiếm thuốc online với từ khóa "amo"
Then kết quả tìm kiếm có thuốc Amoxicillin

@Online @TC_ONLINE_007 @Severity_LOW @Priority_MEDIUM
Scenario: TC_ONLINE_007 - Tìm kiếm không phân biệt hoa thường
When Admin tìm kiếm thuốc online với từ khóa "AMOXICILLIN"
Then kết quả tìm kiếm có thuốc Amoxicillin

@Online @TC_ONLINE_008 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_008 - Tìm kiếm từ khóa không tồn tại
When Admin tìm kiếm thuốc online với từ khóa "abcxyzkhongtontai"
Then danh sách kết quả tìm kiếm không hiển thị thuốc không liên quan

@Online @TC_ONLINE_009 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_009 - Tìm kiếm bằng chuỗi đặc biệt
When Admin tìm kiếm thuốc online với chuỗi đặc biệt "<script>alert(1)</script>"
Then hệ thống không thực thi script
And hệ thống không hiển thị lỗi Server Error

@Online @TC_ONLINE_010 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_010 - Lọc danh mục Kháng sinh
When Admin lọc thuốc online theo danh mục Kháng sinh
Then danh sách chỉ hiển thị thuốc thuộc danh mục Kháng sinh

@Online @TC_ONLINE_011 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_011 - Lọc danh mục Giảm đau Hạ sốt
When Admin lọc thuốc online theo danh mục Giảm đau Hạ sốt
Then danh sách chỉ hiển thị thuốc thuộc danh mục Giảm đau Hạ sốt

@Online @TC_ONLINE_012 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_012 - Lọc danh mục Kháng viêm
When Admin lọc thuốc online theo danh mục Kháng viêm
Then danh sách chỉ hiển thị thuốc thuộc danh mục Kháng viêm

@Online @TC_ONLINE_013 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_013 - Lọc danh mục Vitamin và khoáng chất
When Admin lọc thuốc online theo danh mục Vitamin và khoáng chất
Then danh sách chỉ hiển thị thuốc thuộc danh mục Vitamin và khoáng chất

@Online @TC_ONLINE_014 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_014 - Lọc tất cả danh mục
When Admin chọn bộ lọc tất cả danh mục thuốc online
Then hệ thống hiển thị lại danh sách thuốc online

@Online @TC_ONLINE_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_015 - Kết hợp tìm kiếm và đúng danh mục
When Admin tìm kiếm thuốc "Amoxicillin" trong danh mục Kháng sinh
Then danh sách hiển thị thuốc thỏa cả tên thuốc và danh mục

@Online @TC_ONLINE_016 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_016 - Kết hợp tìm kiếm và sai danh mục
When Admin tìm kiếm thuốc "Amoxicillin" trong danh mục Giảm đau Hạ sốt
Then danh sách không hiển thị thuốc sai danh mục

@Online @TC_ONLINE_017 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_017 - Lọc categoryId không hợp lệ
When Admin lọc thuốc online với categoryId không hợp lệ
Then hệ thống không hiển thị lỗi Server Error

@Online @TC_ONLINE_018 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_018 - Kiểm tra combobox trạng thái
When Admin truy cập trang Quản lý thuốc bán online
Then mỗi dòng thuốc có combobox trạng thái bán online
And combobox có đủ lựa chọn Cho phép bán online và Không cho phép bán online

@Online @TC_ONLINE_019 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_019 - Tắt bán online và kiểm tra lưu trạng thái
When Admin tắt bán online cho thuốc Amoxicillin
Then trạng thái Không cho phép bán online được lưu sau khi reload

@Online @TC_ONLINE_020 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_020 - Bật bán online và kiểm tra lưu trạng thái
When Admin bật bán online cho thuốc Amoxicillin
Then trạng thái Cho phép bán online được lưu sau khi reload

@Online @TC_ONLINE_021 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_021 - Kiểm tra màu trạng thái khi bật bán online
When Admin bật bán online cho thuốc Amoxicillin
Then trạng thái bật bán online có value bằng 1
And combobox trạng thái có style hiển thị

@Online @TC_ONLINE_022 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_022 - Kiểm tra màu trạng thái khi tắt bán online
When Admin tắt bán online cho thuốc Amoxicillin
Then trạng thái tắt bán online có value bằng 0
And combobox trạng thái có style hiển thị

@Online @TC_ONLINE_023 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_023 - User UI khi thuốc cho phép bán online
When Admin bật bán online cho thuốc Amoxicillin
And User tìm kiếm thuốc Amoxicillin trên giao diện người dùng
Then giao diện User hiển thị nút mua hoặc thêm giỏ cho thuốc Amoxicillin

@Online @TC_ONLINE_024 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_024 - User UI khi thuốc không cho phép bán online
When Admin tắt bán online cho thuốc Amoxicillin
And User tìm kiếm thuốc Amoxicillin trên giao diện người dùng
Then giao diện User hiển thị trạng thái mua tại cửa hàng hoặc không bán online cho thuốc Amoxicillin

@Online @TC_ONLINE_025 @Severity_CRITICAL @Priority_HIGH
Scenario: TC_ONLINE_025 - Backend chặn thêm giỏ khi tắt bán online
When Admin tắt bán online cho thuốc Amoxicillin
And User gửi request Ajax thêm thuốc Amoxicillin vào giỏ hàng
Then backend phải từ chối thêm thuốc đã tắt bán online vào giỏ

@Online @TC_ONLINE_026 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_026 - Backend cho thêm giỏ khi bật bán online
When Admin bật bán online cho thuốc Amoxicillin
And User gửi request Ajax thêm thuốc Amoxicillin vào giỏ hàng
Then backend cho phép thêm thuốc đã bật bán online vào giỏ

@Online @TC_ONLINE_027 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_027 - Hiển thị thuốc hết hàng trong Admin
When Admin tìm kiếm thuốc online với từ khóa "Paracetamol"
Then thuốc Paracetamol hiển thị trạng thái Hết hàng trong trang Admin

@Online @TC_ONLINE_028 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_028 - Hiển thị cảnh báo lô sắp hết hạn
When Admin tìm kiếm thuốc online với từ khóa "Amoxicillin"
Then thuốc Amoxicillin có cảnh báo lô sắp hết hạn

@Online @TC_ONLINE_029 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_029 - Kiểm tra ảnh thuốc có src
When Admin truy cập trang Quản lý thuốc bán online
Then ảnh thuốc trong bảng có đường dẫn src

@Online @TC_ONLINE_030 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ONLINE_030 - Kiểm tra ảnh thuốc tải được
When Admin truy cập trang Quản lý thuốc bán online
Then ảnh thuốc trong bảng không bị vỡ hoặc không tải được

@Online @TC_ONLINE_031 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_031 - Kiểm tra định dạng giá bán
When Admin truy cập trang Quản lý thuốc bán online
Then giá bán trong bảng là số dương
And giá bán trong bảng có ký hiệu đ

@Online @TC_ONLINE_032 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_032 - Kiểm tra định dạng tồn kho
When Admin truy cập trang Quản lý thuốc bán online
Then tồn kho trong bảng là số không âm hoặc nhãn Hết hàng

@Online @TC_ONLINE_033 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_033 - Kiểm tra tooltip cảnh báo lô sắp hết hạn
When Admin tìm kiếm thuốc online với từ khóa "Amoxicillin"
Then cảnh báo lô sắp hết hạn có tooltip mô tả đúng

@Online @TC_ONLINE_034 @Severity_HIGH @Priority_HIGH
Scenario: TC_ONLINE_034 - Kiểm tra nhiều điều kiện tìm kiếm lọc liên tục
When Admin thực hiện nhiều điều kiện tìm kiếm và lọc thuốc online liên tục
Then không điều kiện nào gây lỗi Server Error

@Online @TC_ONLINE_035 @Severity_LOW @Priority_LOW
Scenario: TC_ONLINE_035 - Responsive trang Quản lý thuốc online
Given trình duyệt đang ở kích thước màn hình mobile 390px
When Admin truy cập trang Quản lý thuốc bán online
Then trang Quản lý thuốc online không bị tràn ngang nghiêm trọng trên mobile
