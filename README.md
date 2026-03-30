# Taskbar Overlay Uptime (WPF + .NET 8)

Mini dashboard overlay cho Windows: luôn nổi, có chế độ minimal neo ngay trên thanh taskbar, auto refresh, theo dõi service/ping/http/tcp endpoint.

## Kiến trúc

- `MainWindow` + `MainViewModel` + `MonitorCardViewModel`.
- Monitoring layer:
  - `ServiceMonitor`
  - `PingMonitor`
  - `HttpMonitor`
  - `TcpMonitor`
  - `NodeRegistry` (load config JSON)
- Scheduler:
  - `MonitorScheduler` (`PeriodicTimer`, default 10s)
- Native integration:
  - `OverlayWindowService` (tool window, click-through optional)
  - `TaskbarAnchorService` (dock work area hoặc dock trực tiếp vào vùng taskbar)

## Chạy app

Yêu cầu:
- Windows 10/11
- .NET 8 SDK

Lệnh:

```bash
dotnet build TaskbarOverlayUptime.sln
dotnet run --project src/TaskbarOverlayUptime/TaskbarOverlayUptime.csproj
```

## Cấu hình monitor

Sửa file `src/TaskbarOverlayUptime/Config/nodes.json`:

```json
[
  { "name": "Internal API", "type": "Http", "target": "http://localhost:5000/health" },
  { "name": "Redis Service", "type": "Service", "target": "Redis" },
  { "name": "Public DNS", "type": "Ping", "target": "1.1.1.1" },
  { "name": "Node TCP 8080", "type": "Tcp", "target": "10.241.2.163:8080" }
]
```

`type` hỗ trợ: `Ping`, `Service`, `Http`, `Tcp`.

Ví dụ đúng cho case của bạn:
- Check HTTP: `{ "name": "Node HTTP", "type": "Http", "target": "http://10.241.2.163:8080/" }`
- Check TCP port mở: `{ "name": "Node TCP", "type": "Tcp", "target": "10.241.2.163:8080" }`

## Minimal mode (đã bật mặc định)

- Overlay nằm ngay trên thanh taskbar (không nằm phía trên taskbar như kiểu widget thường).
- Chỉ hiện tên target + chấm trạng thái (`xanh` = up, `đỏ` = down).

## Roadmap next

1. Add toast / tray notification khi service down.
2. Add click-through toggle + keyboard shortcut.
3. Detect taskbar edge (trái/phải/dưới) bằng `SHAppBarMessage`.
4. Add persistence (window size, position, refresh interval).
