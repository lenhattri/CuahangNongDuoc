# Cửa hàng Nông Dược

Ứng dụng máy tính để bàn hỗ trợ quản lý vận hành cho cửa hàng kinh doanh vật tư nông dược. Hệ thống được xây dựng trên .NET Framework 4.8 với giao diện Windows Forms, tách lớp rõ ràng giữa tầng dữ liệu, nghiệp vụ và trình bày để dễ bảo trì và mở rộng.

## Chức năng nổi bật
- Quản lý danh mục nông dược, đơn vị tính, mã sản phẩm và nhà cung cấp, tối ưu cho nghiệp vụ nhập hàng và kiểm soát tồn kho.
- Quản lý khách hàng và đại lý, theo dõi lịch sử giao dịch, bán lẻ và bán sỉ ngay trên cùng giao diện MDI.
- Hỗ trợ quy trình nhập hàng, lập phiếu bán lẻ/bán sỉ, in ấn và lưu trữ chứng từ với Report Viewer.
- Theo dõi công nợ, lập phiếu thu – chi, ghi nhận chi phí phát sinh và lý do chi nhằm minh bạch dòng tiền.
- Bộ báo cáo thống kê doanh thu, tồn kho, cảnh báo hàng sắp hết hạn và các chỉ số vận hành phục vụ ra quyết định.
- Quản trị hệ thống với phân quyền người dùng, đăng nhập, thiết lập trợ giúp và tài nguyên giao diện nhất quán.

## Kiến trúc và công nghệ
- **Tầng Domain**: Định nghĩa các thực thể nghiệp vụ như sản phẩm, khách hàng, phiếu nhập, phiếu bán, công nợ và chi phí.
- **Tầng DAL (Data Access Layer)**: Làm việc với cơ sở dữ liệu, cung cấp `DataService` và các bộ sưu tập dữ liệu chuyên biệt.
- **Tầng BLL (Business Logic Layer)**: Điều phối nghiệp vụ, ánh xạ dữ liệu sang đối tượng và kiểm soát luồng xử lý giữa giao diện và dữ liệu.
- **Tầng UI**: Ứng dụng Windows Forms (MDI) với các màn hình Danh Mục, Nhập Hàng, Bán Hàng, Công Nợ, Phiếu Thu Chi, Báo Cáo và cấu hình hệ thống.
- **Thư viện hỗ trợ**: Sử dụng BCrypt để băm mật khẩu, Microsoft Report Viewer cho in ấn/báo cáo, cùng các tiện ích nội bộ trong thư mục `Utils`.

## Cấu trúc thư mục chính
- `Domain/`: Các thực thể nghiệp vụ.
- `DAL/`: Truy xuất dữ liệu và lớp kết nối.
- `BLL/`: Điều phối nghiệp vụ và ánh xạ dữ liệu.
- `UI/`: Giao diện Windows Forms phân theo chức năng (Bán hàng, Nhập hàng, Công nợ, Báo cáo, Hệ thống…).
- `Utils/`: Hàm hỗ trợ và tiện ích dùng chung.
- `Resources/` & `images/`: Tài nguyên giao diện, biểu tượng, ảnh nền.

## Đối tượng sử dụng
Phù hợp cho cửa hàng nông dược hoặc đại lý vật tư nông nghiệp cần một giải pháp quản lý tại chỗ, giao diện thân thiện, hỗ trợ nghiệp vụ bán lẻ/bán sỉ, nhập hàng, công nợ và báo cáo vận hành.
