
Feature: Quản lý lô tiêu hủy
Kiểm thử chức năng Quản lý lô tiêu hủy của hệ thống MedForAll.
Các scenario trong file này được đồng bộ theo test case Selenium đã chạy.

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_001 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_001 - Mở trang Quản lý Lô tiêu hủy
When Admin truy cập trang Quản lý Lô tiêu hủy
Then trang Quản lý Lô tiêu hủy mở được, có tiêu đề và không hiển thị lỗi hệ thống

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_002 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_002 - Kiểm tra sidebar active
When Admin truy cập trang Quản lý Lô tiêu hủy
Then menu Quản lý Lô tiêu hủy trên sidebar được active đúng

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_003 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_003 - Kiểm tra tiêu đề cột bảng
When Admin truy cập trang Quản lý Lô tiêu hủy
Then bảng lô tiêu hủy hiển thị đủ các cột Thuốc, Số lô, Hạn sử dụng, Giá nhập, Nhập , Tồn và Trạng thái

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_004 - Kiểm tra bảng có dữ liệu hợp lệ
When Admin truy cập trang Quản lý Lô tiêu hủy
Then danh sách lô tiêu hủy hiển thị dữ liệu hợp lệ gồm tên thuốc, số lô, hạn sử dụng, giá nhập và trạng thái

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_005 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_005 - Kiểm tra chỉ hiển thị trạng thái Đã tiêu hủy
When Admin truy cập trang Quản lý Lô tiêu hủy
Then tất cả dòng trong bảng phải có trạng thái Đã tiêu hủy

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_006 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_006 - Kiểm tra tồn kho sau tiêu hủy bằng 0
When Admin truy cập trang Quản lý Lô tiêu hủy
Then tất cả lô đã tiêu hủy phải có tồn kho bằng 0

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_007 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_007 - Kiểm tra không có nút tác vụ
When Admin truy cập trang Quản lý Lô tiêu hủy
Then trang lô tiêu hủy chỉ dùng để xem và không có nút xóa, sửa hoặc tiêu hủy lại

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_008 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_008 - Lọc lô tiêu hủy Đã hết hạn
When Admin lọc lô tiêu hủy theo trạng thái Đã hết hạn
Then hệ thống chỉ hiển thị các lô tiêu hủy có hạn sử dụng nhỏ hơn ngày hiện tại

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_009 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_009 - Lọc lô tiêu hủy Sắp hết hạn
When Admin lọc lô tiêu hủy theo trạng thái Sắp hết hạn
Then hệ thống chỉ hiển thị các lô tiêu hủy có hạn sử dụng từ ngày hiện tại đến 90 ngày tới

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_010 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_010 - Lọc Tất cả lô tiêu hủy
When Admin chọn bộ lọc Tất cả lô tiêu hủy
Then hệ thống hiển thị toàn bộ danh sách lô đã tiêu hủy

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_011 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_011 - Kiểm tra đường dẫn 3 nút lọc
When Admin truy cập trang Quản lý Lô tiêu hủy
Then các nút Sắp hết hạn, Đã hết hạn và Tất cả có đường dẫn lọc đúng

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_012 @Severity_MEDIUM @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_012 - Kiểm tra link sort Số lô không chuyển sai trang
When Admin kiểm tra link sắp xếp Số lô ở trang Quản lý Lô tiêu hủy
Then link sort Số lô phải ở lại trang Quản lý Lô tiêu hủy và không chuyển nhầm sang trang Quản lý Lô thuốc

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_013 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_013 - Sắp xếp theo Số lô tăng dần
When Admin sắp xếp danh sách lô tiêu hủy theo Số lô tăng dần
Then danh sách lô tiêu hủy được sắp xếp đúng theo số lô tăng dần

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_014 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_014 - Kiểm tra phân loại hạn sử dụng
When Admin truy cập trang Quản lý Lô tiêu hủy
Then các lô hết hạn và sắp hết hạn được gắn nhãn hoặc đánh dấu phù hợp

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_015 - Kiểm tra định dạng giá nhập
When Admin truy cập trang Quản lý Lô tiêu hủy
Then giá nhập của các lô tiêu hủy là số dương và hiển thị đúng định dạng

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_016 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_016 - Kiểm tra định dạng Nhập , Tồn
When Admin truy cập trang Quản lý Lô tiêu hủy
Then số nhập hiển thị hợp lệ và tồn kho của lô tiêu hủy phải bằng 0

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_017 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_017 - Kiểm tra màu giao diện lô đã tiêu hủy
When Admin truy cập trang Quản lý Lô tiêu hủy
Then các lô đã tiêu hủy có màu xám và chữ mờ đúng giao diện

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_018 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_018 - Kiểm tra ảnh thuốc có src
When Admin truy cập trang Quản lý Lô tiêu hủy
Then ảnh thuốc trong bảng lô tiêu hủy có đường dẫn src

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_019 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_019 - Kiểm tra ảnh thuốc tải được
When Admin truy cập trang Quản lý Lô tiêu hủy
Then không có ảnh thuốc bị vỡ trong bảng lô tiêu hủy

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_020 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_020 - Kiểm tra không hiển thị số âm
When Admin truy cập trang Quản lý Lô tiêu hủy
Then trang Quản lý Lô tiêu hủy không hiển thị số âm bất thường

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_021 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_LOTIEUHUY_021 - Kiểm tra status param không hợp lệ
When Admin truy cập trang Quản lý Lô tiêu hủy với status không hợp lệ
Then hệ thống không phát sinh lỗi hệ thống

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_022 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_LOTIEUHUY_022 - Kiểm tra không chứa thao tác xử lý lại
When Admin truy cập trang Quản lý Lô tiêu hủy
Then trang không chứa hàm hoặc endpoint xóa, sửa, tiêu hủy lại lô thuốc

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_023 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_023 - Kiểm tra nút Thoát
When Admin truy cập trang Quản lý Lô tiêu hủy
Then nút Thoát hiển thị và có đường dẫn hợp lệ

@LoTieuHuy @TC_ADMIN_LOTIEUHUY_024 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_LOTIEUHUY_024 - Responsive trang Quản lý Lô tiêu hủy
When Admin mở trang Quản lý Lô tiêu hủy trên màn hình mobile
Then trang không bị tràn ngang nghiêm trọng

