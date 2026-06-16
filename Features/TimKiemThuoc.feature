Feature:  Tìm kiếm thuốc

  @SearchTest
  Scenario Outline: Kiểm thử các trường hợp tìm kiếm sản phẩm thuốc - <MaTestCase>
    Given Mở trang chủ nhà thuốc để chuẩn bị tìm kiếm
    When Người dùng nhập từ khóa tìm kiếm là "<TuKhoa>"
    And Nhấn nút tìm kiếm hoặc Enter
    Then Hệ thống phải phản hồi kết quả mong đợi là "<KetQuaMongDoi>" và chứa từ khóa "<TuKhoaKiemTra>"

    @TC_<MaTestCase> @Severity_High @Priority_Medium
    Examples: 
      | MaTestCase | TuKhoa       | KetQuaMongDoi | TuKhoaKiemTra |
      | TC01       | Paracetamol  | CO_SAN_PHAM   | Paracetamol   |
      | TC02       | para         | CO_SAN_PHAM   | Paracetamol   |
      | TC03       | ThUốc hO     | CO_SAN_PHAM   | Thuốc         |
      | TC04       | Pa na dol    | KHONG_CO_SP   |               |
      | TC05       | abcxyz123    | KHONG_CO_SP   |               |
      | TC06       | @#$          | KHONG_CO_SP   |               |