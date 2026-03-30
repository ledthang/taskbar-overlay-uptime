# Taskbar Overlay Uptime (WPF + .NET 8)

Mini dashboard overlay cho Windows: luôn nổi, bám gần taskbar, auto refresh, theo dõi service/ping/http endpoint.

## Kiến trúc

- `MainWindow` + `MainViewModel` + `MonitorCardViewModel`.
- Monitoring layer:
  - `ServiceMonitor`
  - `PingMonitor`
  - `HttpMonitor`
  - `NodeRegistry` (load config JSON)
- Scheduler:
  - `MonitorScheduler` (`PeriodicTimer`, default 10s)
- Native integration:
  - `OverlayWindowService` (tool window, click-through optional)
  - `TaskbarAnchorService` (dock góc dưới-phải trong work area)

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
  { "name": "Public DNS", "type": "Ping", "target": "1.1.1.1" }
]
```

`type` hỗ trợ: `Ping`, `Service`, `Http`.

## Roadmap next

1. Add toast / tray notification khi service down.
2. Add click-through toggle + keyboard shortcut.
3. Detect taskbar edge (trái/phải/dưới) bằng `SHAppBarMessage`.
4. Add persistence (window size, position, refresh interval).
