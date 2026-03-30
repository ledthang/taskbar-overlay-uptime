# Taskbar Overlay Uptime (WPF + .NET 8)

Mini overlay cho Windows, hiển thị tên service/node + chấm xanh/đỏ, neo sát taskbar.

## Chạy app

Yêu cầu:
- Windows 10/11
- .NET 8 SDK

```bash
dotnet build TaskbarOverlayUptime.sln
dotnet run --project src/TaskbarOverlayUptime/TaskbarOverlayUptime.csproj
```

## Cấu hình (1 file duy nhất)

Sửa `src/TaskbarOverlayUptime/Config/appsettings.json`:

```json
{
  "overlay": {
    "fontSize": 11,
    "maxRows": 2,
    "allowResize": false,
    "overlayOnTaskbar": true,
    "rightPadding": 170,
    "autoRightPaddingFromTray": true
  },
  "targets": [
    { "name": "Node HTTP", "type": "Http", "target": "http://10.241.2.163:8080/" },
    { "name": "Node TCP 8080", "type": "Tcp", "target": "10.241.2.163:8080" }
  ]
}
```

### Ý nghĩa
- `fontSize`: cỡ chữ.
- `maxRows`: số dòng tối đa trước khi bị cắt.
- `allowResize`: hiện tại để `false` (đã tắt resize để tránh layout lỗi).
- `overlayOnTaskbar`: neo vào dải taskbar.
- `rightPadding`: chừa khoảng bên phải (tránh đè clock/system tray).
- `autoRightPaddingFromTray`: tự ước lượng độ rộng khay hệ thống (TrayNotifyWnd) để cộng padding phải.

## Hành vi UI
- Không nền / không viền để cảm giác dính vào taskbar.
- Không còn scrollbar ngang; content tự wrap sang dòng mới theo `maxRows`.
- Chiều cao overlay sẽ cố bám theo độ dày taskbar (đặc biệt khi taskbar nằm dưới/trên).

## Debug log

Nếu overlay không hiện hoặc lúc ẩn lúc hiện, xem:
- `logs/overlay.log` (cùng thư mục chạy `.exe`)

