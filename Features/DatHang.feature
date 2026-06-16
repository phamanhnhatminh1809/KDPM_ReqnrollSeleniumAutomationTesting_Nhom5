Feature:  Đặt hàng và Thanh toán 

  Background:
    Given Mở trang chủ nhà thuốc và tiến hành chuẩn bị giỏ hàng dữ liệu

  @OrderTest
  Scenario Outline: Kiểm thử luồng đặt hàng và validate thông tin thanh toán tổng hợp - <MaTestCase>
    When Người dùng lựa chọn hình thức nhận hàng là "<HinhThucNhan>"
    And Người dùng điền thông tin cá nhân gồm họ tên "<HoTen>", số điện thoại "<SoDienThoai>" và email "<Email>"
    And Với hình thức "<HinhThucNhan>" người dùng cấu hình địa chỉ theo tỉnh "<Tinh>", quận "<Quan>", phường "<Phuong>", số nhà hoặc kho "<SoNhaHoacKho>"
    And Người dùng tích chọn phương thức thanh toán là "<PhuongThucThanhToan>"
    And Người dùng nhấn nút tiến hành Đặt Hàng Ngay
    Then Hệ thống phải phản hồi trạng thái xử lý đặt hàng mong đợi là "<KetQuaMongDoi>"

    @TC_<MaTestCase> @Severity_High @Priority_High
    Examples: 
      | MaTestCase | HinhThucNhan       | HoTen         | SoDienThoai | Email       | Tinh         | Quan          | Phuong          | SoNhaHoacKho                      | PhuongThucThanhToan | KetQuaMongDoi |
      | TD_01_01   | Giao hàng tận nơi  | Nguyễn Văn A  | 0901234567   | mfa@gmail.com| Hồ Chí Minh  | Quận 1       | Phường Bến Thành| 140                               | Tiền mặt (COD)      | THANH_CONG    |
      | TD_01_02   | Nhận tại nhà thuốc | Trần Thị B    | 0987654321   |             | Hồ Chí Minh  | Quận Tân Phú  |                 | MedForAll Quận Tân Phú       | Chuyển khoản (QR)   | MO_MODAL_QR   |
      | TD_03_01   | Giao hàng tận nơi  |               | 0912345678   | a@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_HOTEN     |
      | TD_04_01   | Giao hàng tận nơi  | Lê Văn C      |             | c@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_SDT       |
      | TD_05_01   | Giao hàng tận nơi  | Lê Văn C      | 090abc4567  | c@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_SDT       |
      | TD_05_02   | Giao hàng tận nơi  | Lê Văn C      | 090@#$54567 | c@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_SDT       |
      | TD_06_01   | Giao hàng tận nơi  | Phạm Văn D    | 09012345     | d@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_SDT       |
      | TD_06_02   | Giao hàng tận nơi  | Phạm Văn D    | 0901234567 | d@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | THANH_CONG       |
      | TD_07_01   | Giao hàng tận nơi  | Nguyễn Văn E  | 0933445566   |             | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | THANH_CONG    |
      | TD_08_01   | Giao hàng tận nơi  | Nguyễn Văn E  | 0933445566   | e_gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_EMAIL     |
      | TD_08_02   | Giao hàng tận nơi  | Nguyễn Văn E  | 0933445566   | e@gmail    | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | LOI_EMAIL     |
      | TD_09_01   | Giao hàng tận nơi  | Hoàng Văn F   | 0944556677   | f@gmail.com |              |               |                 | 140                  | Tiền mặt (COD)      | LOI_DIACHI    |
      | TD_10_01   | Giao hàng tận nơi  | Hoàng Văn F   | 0944556677   | f@gmail.com | Hồ Chí Minh  | Quận 1  |                 | 140                  | Tiền mặt (COD)      | LOI_DIACHI    |
      | TD_11_01   | Giao hàng tận nơi  | Hoàng Văn F   | 0944556677   | f@gmail.com | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   |                                   | Tiền mặt (COD)      | LOI_DIACHI    |
      | TD_13_01   | Nhận tại nhà thuốc | Ngô Thị G     | 0955667788   | g@gmail.com | Hồ Chí Minh  | Quận Tân Phú  |                 | Chưa click chọn kho cụ thể        | Tiền mặt (COD)      | LOI_KHO       |
      | TD_15_01   | Nhận tại nhà thuốc | Đỗ Văn H      | 0966778899   | h@gmail.com | Hồ Chí Minh  | Quận Tân Phú  |                 | MedForAll Quận Tân Phú       | Tiền mặt (COD)      | THANH_CONG    |
      | TD_20_01   | Giao hàng tận nơi  | abc#$         | 0901222333   |             | Hồ Chí Minh  | Quận 1  | Phường Bến Thành   | 140                  | Tiền mặt (COD)      | CHAN_BUG      |