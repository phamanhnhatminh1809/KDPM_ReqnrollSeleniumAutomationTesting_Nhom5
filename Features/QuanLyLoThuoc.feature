
Feature: Quản lý lô thuốc
Kiểm thử chức năng Quản lý lô thuốc của hệ thống MedForAll.
Các scenario trong file này được đồng bộ theo test case Selenium đã chạy.

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_001 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_001 - Mở trang Quản lý Lô thuốc
When Admin truy cập trang Quản lý Lô thuốc
Then trang Quản lý Lô thuốc mở được, có tiêu đề và không hiển thị lỗi hệ thống

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_002 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_002 - Kiểm tra sidebar active
When Admin truy cập trang Quản lý Lô thuốc
Then menu Quản lý Lô thuốc trên sidebar được active đúng

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_003 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_003 - Kiểm tra tiêu đề cột bảng
When Admin truy cập trang Quản lý Lô thuốc
Then bảng lô thuốc hiển thị đủ các cột Thuốc, Số lô, Hạn sử dụng, Giá nhập, Nhập tồn, Trạng thái và Tác vụ

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_004 - Kiểm tra bảng có dữ liệu hợp lệ
When Admin truy cập trang Quản lý Lô thuốc
Then danh sách lô thuốc hiển thị dữ liệu hợp lệ gồm tên thuốc, số lô, hạn sử dụng, giá nhập và trạng thái

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_005 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_005 - Lọc lô Sắp hết hạn
When Admin lọc lô thuốc theo trạng thái Sắp hết hạn
Then hệ thống chỉ hiển thị các lô có hạn sử dụng từ ngày hiện tại đến 90 ngày tới

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_006 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_006 - Lọc lô Đã hết hạn
When Admin lọc lô thuốc theo trạng thái Đã hết hạn
Then hệ thống chỉ hiển thị các lô có hạn sử dụng nhỏ hơn ngày hiện tại

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_007 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_007 - Lọc Tất cả lô thuốc
When Admin chọn bộ lọc Tất cả lô thuốc
Then hệ thống hiển thị toàn bộ danh sách lô thuốc

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_008 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_008 - Kiểm tra đường dẫn 3 nút lọc
When Admin truy cập trang Quản lý Lô thuốc
Then các nút lọc của trang Quản lý Lô thuốc có đường dẫn đúng

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_009 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_009 - Sắp xếp theo Số lô tăng dần
When Admin sắp xếp danh sách lô thuốc theo Số lô tăng dần
Then danh sách lô thuốc được sắp xếp đúng theo số lô tăng dần

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_010 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_010 - Kiểm tra phân loại ngày hết hạn
When Admin truy cập trang Quản lý Lô thuốc
Then các lô hết hạn và sắp hết hạn được gắn nhãn hoặc tô màu đúng

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_011 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_011 - Kiểm tra định dạng giá nhập
When Admin truy cập trang Quản lý Lô thuốc
Then giá nhập của các lô thuốc là số dương và hiển thị đúng định dạng

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_012 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_012 - Kiểm tra định dạng Nhập , Tồn
When Admin truy cập trang Quản lý Lô thuốc
Then số nhập và số tồn hiển thị hợp lệ, không âm và tồn không lớn hơn nhập

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_013 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_013 - Kiểm tra trạng thái lô
When Admin truy cập trang Quản lý Lô thuốc
Then trạng thái lô thuốc thuộc nhóm Đang bán, Ngưng bán hoặc Đã tiêu hủy

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_014 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_014 - Kiểm tra mỗi dòng có nút tác vụ
When Admin truy cập trang Quản lý Lô thuốc
Then mỗi lô thuốc có nút tác vụ phù hợp

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_015 - Kiểm tra nút Xóa gọi đúng hàm deleteLo
When Admin kiểm tra nút Xóa của lô thuốc
Then nút Xóa gọi đúng hàm deleteLo

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_016 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_016 - Kiểm tra nút Tiêu hủy gọi đúng hàm tieuHuyLo
When Admin kiểm tra nút Tiêu hủy của lô thuốc
Then nút Tiêu hủy gọi đúng hàm tieuHuyLo

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_017 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_017 - Hủy xóa lô thuốc
When Admin bấm Xóa lô thuốc nhưng chọn Cancel ở hộp xác nhận
Then hệ thống không xóa lô thuốc và dữ liệu không thay đổi

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_018 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_018 - Hủy tiêu hủy lô thuốc
When Admin bấm Tiêu hủy lô thuốc nhưng chọn Cancel ở hộp xác nhận
Then hệ thống không tiêu hủy lô thuốc và dữ liệu không thay đổi

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_019 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_QUANLYLOTHUOC_019 - Backend chặn xóa ID không tồn tại
When Admin gửi request xóa lô thuốc với ID không tồn tại
Then backend trả về kết quả thất bại và không phát sinh lỗi hệ thống

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_020 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_QUANLYLOTHUOC_020 - Backend chặn tiêu hủy ID không tồn tại
When Admin gửi request tiêu hủy lô thuốc với ID không tồn tại
Then backend trả về kết quả thất bại và không phát sinh lỗi hệ thống

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_021 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_021 - Kiểm tra ảnh lô thuốc có src
When Admin truy cập trang Quản lý Lô thuốc
Then ảnh thuốc trong bảng lô thuốc có đường dẫn src

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_022 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_022 - Kiểm tra ảnh lô thuốc tải được
When Admin truy cập trang Quản lý Lô thuốc
Then không có ảnh thuốc bị vỡ trong bảng lô thuốc

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_023 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_QUANLYLOTHUOC_023 - Kiểm tra không hiển thị số âm
When Admin truy cập trang Quản lý Lô thuốc
Then trang Quản lý Lô thuốc không hiển thị số âm bất thường

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_024 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_QUANLYLOTHUOC_024 - Responsive trang Quản lý Lô thuốc
When Admin mở trang Quản lý Lô thuốc trên màn hình mobile
Then trang Quản lý Lô thuốc không bị tràn ngang nghiêm trọng

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_025 @Severity_HIGH @Priority_MEDIUM @Skip
Scenario: TC_ADMIN_QUANLYLOTHUOC_025 - Xóa lô thuốc thật
When Admin xác nhận xóa thật một lô thuốc
Then lô thuốc bị xóa khỏi danh sách

@LoThuoc @TC_ADMIN_QUANLYLOTHUOC_026 @Severity_HIGH @Priority_MEDIUM @Skip
Scenario: TC_ADMIN_QUANLYLOTHUOC_026 - Tiêu hủy lô thuốc thật
When Admin xác nhận tiêu hủy thật một lô thuốc
Then lô thuốc chuyển tồn kho về 0, trạng thái Đã tiêu hủy và xuất hiện ở trang Quản lý lô tiêu hủy

