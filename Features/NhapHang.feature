
Feature: Nhập lô thuốc
Kiểm thử chức năng Nhập lô thuốc của hệ thống MedForAll.
Các scenario trong file này được đồng bộ theo test case Selenium đã chạy.

@NhapLo @TC_ADMIN_NHAPLO_001 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_001 - Mở trang Danh sách thuốc
When Admin truy cập trang Danh sách thuốc
Then trang Danh sách thuốc mở được, có nút Thêm thuốc mới, có nút Nhập và không hiển thị lỗi hệ thống

@NhapLo @TC_ADMIN_NHAPLO_002 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_002 - Kiểm tra sidebar active
When Admin truy cập trang Danh sách thuốc
Then menu Danh sách thuốc trên sidebar được active đúng

@NhapLo @TC_ADMIN_NHAPLO_003 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_003 - Kiểm tra tiêu đề cột bảng thuốc
When Admin truy cập trang Danh sách thuốc
Then bảng Danh sách thuốc hiển thị đủ các cột Ảnh, Tên thuốc, Danh mục, Tồn kho, Giá bán, Trạng thái và Hành động

@NhapLo @TC_ADMIN_NHAPLO_004 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_004 - Kiểm tra bảng thuốc có dữ liệu
When Admin truy cập trang Danh sách thuốc
Then danh sách thuốc hiển thị dữ liệu hợp lệ và mỗi dòng thuốc có nút Nhập

@NhapLo @TC_ADMIN_NHAPLO_005 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_005 - Tìm thuốc trước khi nhập lô
When Admin tìm kiếm thuốc Amoxicillin trước khi nhập lô
Then kết quả tìm kiếm chỉ hiển thị thuốc phù hợp với từ khóa Amoxicillin

@NhapLo @TC_ADMIN_NHAPLO_006 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_006 - Lọc danh mục trước khi nhập lô
When Admin lọc danh sách thuốc theo danh mục Kháng sinh
Then danh sách chỉ hiển thị các thuốc thuộc danh mục Kháng sinh

@NhapLo @TC_ADMIN_NHAPLO_007 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_007 - Kiểm tra link nút Nhập
When Admin kiểm tra nút Nhập của thuốc Amoxicillin
Then nút Nhập trỏ đúng đến trang nhập hàng của thuốc Amoxicillin

@NhapLo @TC_ADMIN_NHAPLO_008 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_008 - Bấm nút Nhập mở form nhập kho
When Admin bấm nút Nhập của thuốc Amoxicillin
Then hệ thống chuyển sang màn hình Nhập lô hàng mới của thuốc Amoxicillin

@NhapLo @TC_ADMIN_NHAPLO_009 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_009 - Kiểm tra thông tin thuốc trên trang nhập kho
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then trang nhập kho hiển thị đúng tên thuốc, đơn vị tính, tồn kho hiện tại và giá bán hiện tại

@NhapLo @TC_ADMIN_NHAPLO_010 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_010 - Kiểm tra hidden field thuốc
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then các hidden field của thuốc chứa đúng IdThuoc, TenThuoc, DonViTinh, GiaBanHienTai và TongTonKho

@NhapLo @TC_ADMIN_NHAPLO_011 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_011 - Kiểm tra form có đủ field nhập lô
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then form nhập lô hiển thị đủ các field Số lô, Ngày sản xuất, Hạn sử dụng, Số lượng nhập, Giá nhập và Thành tiền

@NhapLo @TC_ADMIN_NHAPLO_012 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_012 - Kiểm tra name của các field
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then các field nhập lô có thuộc tính name đúng để gửi POST

@NhapLo @TC_ADMIN_NHAPLO_013 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_013 - Kiểm tra bắt buộc nhập Số lô
When Admin kiểm tra ràng buộc bắt buộc của field Số lô
Then field Số lô có cấu hình validate bắt buộc và thông báo lỗi phù hợp

@NhapLo @TC_ADMIN_NHAPLO_014 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_014 - Kiểm tra min Số lượng nhập
When Admin kiểm tra ràng buộc min của field Số lượng nhập
Then field Số lượng nhập phải có min bằng 1

@NhapLo @TC_ADMIN_NHAPLO_015 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_015 - Kiểm tra min Giá nhập
When Admin kiểm tra ràng buộc min của field Giá nhập
Then field Giá nhập phải có min bằng 1 vì giá nhập phải lớn hơn 0

@NhapLo @TC_ADMIN_NHAPLO_016 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_016 - Kiểm tra giá trị mặc định Số lượng
When Admin kiểm tra giá trị mặc định của field Số lượng nhập
Then giá trị mặc định của Số lượng nhập không được nhỏ hơn min

@NhapLo @TC_ADMIN_NHAPLO_017 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_017 - Kiểm tra giá trị mặc định Giá nhập
When Admin kiểm tra giá trị mặc định của field Giá nhập
Then giá trị mặc định của Giá nhập không được nhỏ hơn min

@NhapLo @TC_ADMIN_NHAPLO_018 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_NHAPLO_018 - Tự động tính tổng giá trị lô hàng
When Admin nhập số lượng 5 và giá nhập 22000
Then hệ thống tự động tính đúng tổng giá trị lô hàng

@NhapLo @TC_ADMIN_NHAPLO_019 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_NHAPLO_019 - Tính tổng giá trị lô hàng với số lớn
When Admin nhập số lượng lớn và giá nhập lớn trên form nhập lô
Then hệ thống vẫn tính đúng tổng giá trị lô hàng

@NhapLo @TC_ADMIN_NHAPLO_020 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_020 - Cảnh báo giá nhập cao hơn giá bán
When Admin nhập giá nhập cao hơn giá bán hiện tại
Then hệ thống hiển thị cảnh báo giá nhập cao hơn giá bán

@NhapLo @TC_ADMIN_NHAPLO_021 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_021 - Ẩn cảnh báo khi giá nhập hợp lệ lại
When Admin nhập giá nhập cao hơn giá bán rồi sửa lại thành giá hợp lệ
Then cảnh báo giá nhập cao hơn giá bán phải biến mất

@NhapLo @TC_ADMIN_NHAPLO_022 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_022 - Kiểm tra input ngày là type date
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then field Ngày sản xuất và Hạn sử dụng phải là input type date

@NhapLo @TC_ADMIN_NHAPLO_023 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_023 - Kiểm tra format value input date
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then giá trị của input date phải có format yyyy-MM-dd hoặc để rỗng

@NhapLo @TC_ADMIN_NHAPLO_024 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_024 - Kiểm tra nút Hủy bỏ
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then nút Hủy bỏ trỏ đúng về trang Danh sách thuốc

@NhapLo @TC_ADMIN_NHAPLO_025 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_025 - Bấm Hủy bỏ quay về Danh sách thuốc
When Admin bấm nút Hủy bỏ trên trang nhập lô
Then hệ thống quay về trang Danh sách thuốc và không lưu dữ liệu nhập lô

@NhapLo @TC_ADMIN_NHAPLO_026 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_026 - Kiểm tra form action và method
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then form nhập kho gửi POST đúng action của thuốc Amoxicillin

@NhapLo @TC_ADMIN_NHAPLO_027 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_027 - Kiểm tra Anti-forgery token
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then form nhập kho có Anti-forgery token hợp lệ

@NhapLo @TC_ADMIN_NHAPLO_028 @Severity_HIGH @Priority_HIGH
Scenario: TC_ADMIN_NHAPLO_028 - Mở nhập kho với ID thuốc không tồn tại
When Admin mở trang Nhập lô hàng mới với ID thuốc không tồn tại
Then hệ thống không được phát sinh lỗi hệ thống

@NhapLo @TC_ADMIN_NHAPLO_029 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_029 - Kiểm tra ảnh ở trang nhập kho
When Admin mở trang Nhập lô hàng mới của thuốc Amoxicillin
Then ảnh thuốc ở trang nhập kho tải được và không bị vỡ

@NhapLo @TC_ADMIN_NHAPLO_030 @Severity_MEDIUM @Priority_MEDIUM
Scenario: TC_ADMIN_NHAPLO_030 - Kiểm tra không hiển thị số âm
When Admin nhập dữ liệu hợp lệ trên trang nhập lô
Then trang nhập kho không hiển thị số âm bất thường

@NhapLo @TC_ADMIN_NHAPLO_031 @Severity_LOW @Priority_LOW
Scenario: TC_ADMIN_NHAPLO_031 - Responsive trang nhập kho
When Admin mở trang Nhập lô hàng mới trên màn hình mobile
Then trang nhập kho không bị tràn ngang nghiêm trọng

@NhapLo @TC_ADMIN_NHAPLO_032 @Severity_HIGH @Priority_MEDIUM @Skip
Scenario: TC_ADMIN_NHAPLO_032 - Nhập kho thật với dữ liệu hợp lệ
When Admin submit nhập kho thật với dữ liệu hợp lệ
Then hệ thống tạo lô thuốc mới và lô xuất hiện trong danh sách lô thuốc

