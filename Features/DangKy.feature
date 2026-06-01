@dang_ky_feature
Feature: Đăng ký vào hệ thống web bán thuốc MedForAll
	Đăng ký tài khoản vào hệ thống web bán thuốc MedForAll
@luong_thanh_cong
Scenario Outline: Đăng ký thành công
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	And hệ thống chuyển hướng sang trang đăng nhập
Examples: Dữ liệu hợp lệ
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi                                       |
	| TC_USER_DANGKY_001 | TD_01_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@    | Hệ thống thông báo đăng ký thành công và chuyển hướng. |

@kiem_tra_rang_buoc
Scenario Outline: Kiểm tra các ràng buộc đầu vào không hợp lệ (Mục 2, 4, 7)
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	Then hệ thống thông báo lỗi "<ket_qua_mong_doi>"
	
# Ràng buộc các kiểu dữ liệu 
Examples: Ràng buộc Tên đăng nhập
	| tc_id              | data_id  | ten_dang_nhap         | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi               |
	| TC_USER_DANGKY_002 | TD_02_01 |                       | Minh@gmail.com | Minh134@ | Minh134@    | Yêu cầu nhập tên đăng nhập     |
	| TC_USER_DANGKY_003 | TD_03_01 | M                     | Minh@gmail.com | Minh134@ | Minh134@    | tối thiểu                      |
	| TC_USER_DANGKY_004 | TD_04_01 | MinhPham [1000_ky_tu] | Minh@gmail.com | Minh134@ | Minh134@    | [gioi_han_ky_tu]               |
	| TC_USER_DANGKY_005 | TD_05_01 | !@#%                  | Minh@gmail.com | Minh134@ | Minh134@    | không được chứa ký tự đặc biệt |

Examples: Ràng buộc Email
	| tc_id              | data_id  | ten_dang_nhap | email         | mat_khau | xac_nhan_mk | ket_qua_mong_doi          |
	| TC_USER_DANGKY_007 | TD_07_01 | MinhPham      |               | Minh134@ | Minh134@    | Email không được để trống |
	| TC_USER_DANGKY_008 | TD_08_01 | MinhPham      | Minhgmail.com | Minh134@ | Minh134@    | không đúng định dạng      |
	| TC_USER_DANGKY_008 | TD_08_02 | MinhPham      | Minh@.com     | Minh134@ | Minh134@    | không đúng định dạng      |
	| TC_USER_DANGKY_008 | TD_08_03 | MinhPham      | Minh@gmail    | Minh134@ | Minh134@    | không đúng định dạng      |
	
Examples: Ràng buộc Mật khẩu
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau              | xac_nhan_mk           | ket_qua_mong_doi                   |
	| TC_USER_DANGKY_011 | TD_11_01 | MinhPham      | Minh@gmail.com |                       | Minh134@              | Yêu cầu nhập mật khẩu              |
	| TC_USER_DANGKY_011 | TD_11_02 | MinhPham      | Minh@gmail.com | Minh134@              |                       | Xác nhận mật khẩu không được trống |
	| TC_USER_DANGKY_012 | TD_12_01 | MinhPham      | Minh@gmail.com | Mi                    | Mi                    | Mật khẩu phải dài ít nhất 6 ký tự  |
	| TC_USER_DANGKY_013 | TD_13_01 | MinhPham      | Minh@gmail.com | Minh134@ [1000_ky_tu] | Minh134@ [1000_ky_tu] | [gioi_han_ky_tu]                   |
	| TC_USER_DANGKY_014 | TD_14_01 | MinhPham      | Minh@gmail.com | minh134@              | minh134@              | thiếu chữ hoa                      |
	| TC_USER_DANGKY_014 | TD_14_02 | MinhPham      | Minh@gmail.com | M1334@                | M1334@                | thiếu chữ thường                   |
	| TC_USER_DANGKY_014 | TD_14_03 | MinhPham      | Minh@gmail.com | Minhhh@               | Minhhh@               | thiếu số                           |
	| TC_USER_DANGKY_014 | TD_14_04 | MinhPham      | Minh@gmail.com | Minh1344              | Minh1344              | thiếu ký tự đặc biệt               |

Examples: Ràng buộc Xác nhận mật khẩu
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi               |
	| TC_USER_DANGKY_015 | TD_15_01 | MinhPham      | Minh@gmail.com | Minh134@ | ""          | Yêu cầu nhập xác nhận mật khẩu |
	| TC_USER_DANGKY_016 | TD_16_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@a   | Mật khẩu xác nhận không khớp   |

Examples: Kiểm tra bảo mật đầu vào (HTML, SQL)
	| tc_id              | data_id  | ten_dang_nhap                                  | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi       |
	| TC_USER_DANGKY_017 | TD_17_01 | <script>alert('XSS')</script>                  | Minh@gmail.com | Minh134@ | Minh134@    | [bao_mat_dau_vao_html] |
	| TC_USER_DANGKY_017 | TD_17_02 | <h1>HTML được cấy thành công vào hệ thống</h1> | Minh@gmail.com | Minh134@ | Minh134@    | [bao_mat_dau_vao_html] |
	| TC_USER_DANGKY_018 | TD_18_01 | "                                              | Minh@gmail.com | Minh134@ | Minh134@    | [bao_mat_dau_vao_sql]  |
	| TC_USER_DANGKY_018 | TD_18_02 | TRUNCATE TABLE THUOC                                              | Minh@gmail.com | Minh134@ | Minh134@    | [bao_mat_dau_vao_sql]  |

Examples: Kiểm tra về an toàn bảo mật
	| tc_id              | data_id  | ten_dang_nhap                 | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi              |
	| TC_USER_DANGKY_023 | TD_23_01 | <script>alert('XSS')</script> | Minh@gmail.com | Minh134@ | Minh134@    | [khong_hien_thi_loi_chi_tiet] |

@kiem_tra_trung_lap
Scenario Outline: Kiểm tra trùng lặp
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	And người dùng nhấn vào nút Đăng ký
	And người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	Then hệ thống thông báo lỗi "<ket_qua_mong_doi>"

Examples: Kiểm tra trùng lặp tên đăng nhập và email
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau | xac_nhan_mk | ket_qua_mong_doi         |
	| TC_USER_DANGKY_010 | TD_10_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@    | Tên đăng nhập đã tồn tại |
	| TC_USER_DANGKY_006 | TD_06_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@    | Gmail đã tồn tại         |

@bao_mat_ui_chong_spam
Scenario Outline: Kiểm tra vô hiệu hóa nút đăng ký để chống spam (TC_024)
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	Then nút Đăng ký phải bị vô hiệu hóa trong lúc chờ xử lý

Examples: Dữ liệu hợp lệ
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau | xac_nhan_mk |
	| TC_USER_DANGKY_024 | TD_24_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@    |

@hien_thi_ui_canh_bao
Scenario Outline: Kiểm tra hiển thị cảnh báo đỏ khi để trống các trường bắt buộc (TC_025)
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And người dùng nhấn nút Đăng ký trên giao diện đăng ký
	Then hệ thống hiển thị cảnh báo đỏ cho tất cả các trường

Examples: Dữ liệu trống
	| tc_id              | data_id  | ten_dang_nhap | email | mat_khau | xac_nhan_mk | ket_qua_mong_doi  |
	| TC_USER_DANGKY_025 | TD_25_01 | ""            | ""    | ""       | ""          | [loi_canh_bao_do] |

@hien_thi_ui_an_mat_khau
Scenario Outline: Kiểm tra xem vùng nhập mật khẩu có phải là type password
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	And người dùng nhấn vào nút Đăng ký
	When người dùng nhập thông tin đăng ký bao gồm "<ten_dang_nhap>", "<email>", "<mat_khau>" và "<xac_nhan_mk>"
	And mật khẩu phải ở dạng dấu chấm

Examples: Dữ liệu kiểm tra
	| tc_id              | data_id  | ten_dang_nhap | email          | mat_khau | xac_nhan_mk |
	| TC_USER_DANGKY_026 | TD_26_01 | MinhPham      | Minh@gmail.com | Minh134@ | Minh134@    |

