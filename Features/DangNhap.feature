@dang_nhap_feature
Feature: Đăng nhập vào hệ thống web bán thuốc MedForAll
	Đăng nhập tài khoản vào hệ thống web bán thuốc MedForAll
@luong_thanh_cong
Scenario Outline: Đăng nhập thành công
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	When người dùng nhấn vào nút Đăng nhập
	And người dùng nhập thông tin đăng nhập bao gồm "<ten_dang_nhap>" và "<mat_khau>"
	And người dùng nhấn nút Đăng nhập trên giao diện đăng nhập
	Then hệ thống chuyển hướng sang trang chủ và hiển thị tên "<ket_qua_mong_doi>"
Examples: Tên đăng nhập hợp lệ, có khoảng trắng
	| tc_id                | data_id  | ten_dang_nhap | mat_khau | ket_qua_mong_doi |
	| TC_USER_DANGNHAP_001 | TD_01_01 | MinhPham      | Minh134@ | MinhPham         |
	| TC_USER_DANGNHAP_002 | TD_02_01 | " MinhPham"   | Minh134@ | MinhPham         |
	| TC_USER_DANGNHAP_002 | TD_02_02 | "MinhPham "   | Minh134@ | MinhPham         |
	| TC_USER_DANGNHAP_002 | TD_02_03 | " MinhPham "  | Minh134@ | MinhPham         |
	| TC_USER_DANGNHAP_003 | TD_03_01 | minhPham      | Minh134@ | MinhPham         |
	| TC_USER_DANGNHAP_003 | TD_03_02 | MINHPHAM      | Minh134@ | MinhPham         |
	
@kiem_tra_rang_buoc_du_lieu
Scenario Outline: Đăng nhập thất bại
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	When người dùng nhấn vào nút Đăng nhập
	And người dùng nhập thông tin đăng nhập bao gồm "<ten_dang_nhap>" và "<mat_khau>"
	And người dùng nhấn nút Đăng nhập trên giao diện đăng nhập
	Then hệ thống hiển thị tên lỗi "<ket_qua_mong_doi>"

Examples: Ràng buộc Tên đăng nhập
	| tc_id                | data_id  | ten_dang_nhap        | mat_khau | ket_qua_mong_doi           |
	| TC_USER_DANGNHAP_004 | TD_04_01 | MINHPHAM [101_ky_tu] | Minh134@ | [gioi_han_ky_tu]           |
	| TC_USER_DANGNHAP_005 | TD_05_01 |                      | Minh134@ | Yêu cầu nhập tên đăng nhập |

Examples: Ràng buộc Mật khẩu
	| tc_id                | data_id  | ten_dang_nhap | mat_khau             | ket_qua_mong_doi                       |
	| TC_USER_DANGNHAP_006 | TD_06_01 | MinhPham      | minh134@             | Tên đăng nhập hoặc mật khẩu không đúng |
	| TC_USER_DANGNHAP_007 | TD_07_01 | MinhPham      | Minh134@ [101_ky_tu] | [gioi_han_ky_tu]                       |
	| TC_USER_DANGNHAP_008 | TD_08_01 | MinhPham      |                      | Yêu cầu nhập mật khẩu                  |

Examples: Bỏ trống cả 2 trường
	| tc_id                | data_id  | ten_dang_nhap | mat_khau | ket_qua_mong_doi        |
	| TC_USER_DANGNHAP_009 | TD_09_01 |               |          | [hien_thi_loi_2_truong] |

@kiem_tra_bao_mat_ma_doc
Scenario Outline: Kiểm tra bảo mật nhập dữ liệu có chứa mã độc
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	When người dùng nhấn vào nút Đăng nhập
	And người dùng nhập thông tin đăng nhập bao gồm "<ten_dang_nhap>" và "<mat_khau>"
	And người dùng nhấn nút Đăng nhập trên giao diện đăng nhập
	Then hệ thống không chuyển sang trang chủ "<loai_ma_doc>"
Examples: Mã độc html
	| tc_id                | data_id  | ten_dang_nhap                              | mat_khau | loai_ma_doc |
	| TC_USER_DANGNHAP_010 | TD_10_01 | <script>Alert("Lỗ hỏng hệ thống")</script> | Minh134@ | [html]      |


Examples: Mã độc sql
	| tc_id                | data_id  | ten_dang_nhap | mat_khau   | loai_ma_doc |
	| TC_USER_DANGNHAP_011 | TD_11_01 | OR '1'='1'    | OR '1'='1' | [sql]       |

@kiem_tra_thong_bao_khi_nhap_sai
Scenario Outline: Kiểm tra thông báo không được quá chi tiết khi người dùng nhập sai
	Given người dùng truy cập vào trang web "http://localhost:44317/"
	When người dùng nhấn vào nút Đăng nhập
	And người dùng nhập thông tin đăng nhập bao gồm "<ten_dang_nhap>" và "<mat_khau>"
	And người dùng nhấn nút Đăng nhập trên giao diện đăng nhập
	Then hệ thống thông báo không chi tiết "<ket_qua_mong_doi>"

Examples: Thông báo không chi tiết
	| tc_id                | data_id  | ten_dang_nhap                              | mat_khau       | ket_qua_mong_doi                       |
	| TC_USER_DANGNHAP_016 | TD_16_01 | UserKhongTonTai_999                        | Minh134@       | Tên đăng nhập hoặc mật khẩu không đúng |
	| TC_USER_DANGNHAP_017 | TD_17_01 | MinhPham                                   | SaiMatKhau_123 | Tên đăng nhập hoặc mật khẩu không đúng |
	| TC_USER_DANGNHAP_018 | TD_18_01 | <script>Alert("Lỗ hỏng hệ thống")</script> | Minh134@       | [yellow_screen]                        |

