Feature:  Tra cứu Thuoc

  Background:
    Given Mở trang chủ nhà thuốc và chuyển sang trang Tra Cứu Thuốc

  @OpenFDATest
  Scenario Outline: Kiểm thử các trường hợp tra cứu thông tin thuốc - <MaTestCase>
    When Người dùng nhập từ khóa tra cứu là "<TuKhoa>"
    And Nhấn nút Tra cứu thông tin
    Then Hệ thống phải phản hồi kết quả tra cứu mong đợi là "<KetQuaMongDoi>" và chứa từ khóa "<TuKhoaKiemTra>"

    @TC_<MaTestCase> @Severity_High @Priority_High
    Examples: 
      | MaTestCase       | TuKhoa                               | KetQuaMongDoi | TuKhoaKiemTra                |
      | TRACUU_001_TD01  | tylenol                              | CO_KET_QUA    | TYLENOL Extra Strength       |
      | TRACUU_001_TD02  | aspirin                              | CO_KET_QUA    | Aspirin                      |
      | TRACUU_002_TD01  | ibuprofen                            | CO_KET_QUA    | ibuprofen                    |
      | TRACUU_003_TD01  | TYLENOL                              | CO_KET_QUA    | TYLENOL Extra Strength       |
      | TRACUU_003_TD02  | TyLeNoL                              | CO_KET_QUA    | TYLENOL Extra Strength       |
      | TRACUU_004_TD01  |  tylenol                             | CO_KET_QUA    | TYLENOL Extra Strength       |
      | TRACUU_005_TD01  |                                      | DE_TRONG      | Please fill out this field.  |
      | TRACUU_006_TD01  | abcxyz12345                          | KHONG_CO_DATA | Không tìm thấy thuốc nào.    |
      | TRACUU_006_TD02  | Thuốc ho bổ phế                      | KHONG_CO_DATA | Không tìm thấy thuốc nào.    |
      | TRACUU_007_TD01  | @#$ %^&*                             | KHONG_CO_DATA | Không tìm thấy thuốc nào.    |
      | TRACUU_007_TD01  | TyLeNoL@                             | KHONG_CO_DATA | Không tìm thấy thuốc nào.    |
      | TRACUU_008_TD01  | <script>alert('FDA')</script>        | SECURITY_OK   | <script>alert('FDA')</script>|