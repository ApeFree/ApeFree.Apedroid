# ApeFree.Apedroid 架构设计文档

> **定位**：ApeFree 组织面向现代 Android 生态的聚合扩展库与 UI 套件，兼具核心组件生命周期扩展与符合 `ApeFree.ApeDialogs` 标准的 19 种高颜值 Material 3 对话框体系。

---

## 1. 总体架构拓扑

```
┌─────────────────────────────────────────────────────────────┐
│                    业务调用方 (Consumer)                     │
│       await this.ShowInputDialogAsync("修改昵称", "…");      │
│       或 await ApedroidDialogProvider.Default...             │
│       或 await DialogContextExtensions.ShowToastAsync(…);    │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  ApedroidDialogProvider : DialogProvider<Context>            │
│  · 19 个 CreateXxxDialog 标准工厂方法                         │
│  · Smart Context 解析（显式 Context / 自动回退 Top Activity） │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  ApedroidDialog<TResult> / ApedroidAdjustDialog<TResult>    │
│  (继承 BaseDialog<ApedroidDialogHost, OptionButton, ...>)   │
│  · 装配 Host 宿主（圆角/微投影/ReminderColor/标题/正文/操作栏）│
│  · 挂载动态内容区（19 种不同类型的 ContentView）              │
│  · 拦截 TrySubmit 执行 Validator 校验与失败提示               │
│  · 管理 Duration 倒计时自动关闭与 CancellationToken 取消      │
└──────────────────────────────┬──────────────────────────────┘
                               │
       ┌───────────────────────┴───────────────────────┐
       ▼                                               ▼
┌─────────────────────────────┐         ┌─────────────────────────────┐
│ 动态表单引擎 (DataEntryView) │         │ SAF 异步存储桥接 (SafBridge) │
│ 自动依据字段元数据生成表单     │         │ 免重写 OnActivityResult    │
└─────────────────────────────┘         └─────────────────────────────┘
```

---

## 2. 视觉设计规范 (Material 3 Design Tokens)

| 设计维度 | 参数 / 规范值 | 说明 |
|---|---|---|
| **卡片圆角 (Corner Radius)** | `28dp` | 遵循 Google Material Design 3 Dialog Container 规范 |
| **卡片背景色** | `#FAFAFC` (浅色) | 柔和表面微色，与底层蒙层形成自然对比 |
| **卡片微投影 (Elevation)** | `8dp` | 硬件加速卡片层级投射 |
| **蒙层昏暗度 (Dim Amount)** | `0.45f` | 聚焦用户视线同时保留底层感知 |
| **顶部指示条 (Reminder Bar)**| 高度 `4dp`，圆角嵌入 | 映射 `ReminderColor` / `Theme.ReminderColor` |
| **标题排版 (Title)** | 字体尺寸 `20sp`，粗体 (Bold) | 字体色 `#1C1B1F`，边距 24dp, 20dp, 24dp, 8dp |
| **正文排版 (Content)** | 字体尺寸 `15sp`，行高 1.15 | 字体色 `#49454F`，边距 24dp, 0dp, 24dp, 12dp |
| **操作按钮 (OptionButton)** | 高度 `44dp`，圆角 `12dp` | 依据 `DialogOptionTag` 自动适配语义色与对比前景色 |

---

## 3. 架构决策记录 (ADR)

### ADR-001：废弃 Xamarin.Android，全面拥抱 .NET 10 (`net10.0-android`)
* **背景**：原仓库基于 Xamarin.Android v12 开发，微软已于 2024 年彻底终止对 Xamarin 的维护。
* **决策**：全面推倒重构，采用现代化 SDK-style `<TargetFramework>net10.0-android</TargetFramework>`。
* **收益**：享受 C# 13 语法特性、.NET 10 高性能 JIT/AOT 与新版 Android API 支持。

### ADR-002：单包聚合方案 (All-in-One) 替代散落子工程
* **背景**：旧版拆分为 Core、Dialog、Permission、Supplement.Mqtt 等 5 个子工程，引用与发包碎片化严重。
* **决策**：将常用扩展、生命周期管理与 Dialog 体系全部整合入单一的 `ApeFree.Apedroid` NuGet 程序包（类似 `ApeFree.ApeForms` 模式）。
* **收益**：消费者只需引用一个包即可开箱即用所有功能，维护与依赖成本降低 80% 以上。

### ADR-003：代码化 Material 3 容器布局，解除 XML R.layout 资源绑定耦合
* **背景**：Android 库若依赖大量独立 XML 布局资源，易与宿主应用的资源 ID 发生编译期冲突，且重写难度高。
* **决策**：`ApedroidDialogHost` 与各子 Dialog 视图全部采用 C# 编程式构建原生 View 树，辅以 `GradientDrawable` 与 Material 3 控件。
* **收益**：库体积精简至极致，无任何资源 ID 污染与合并冲突风险，渲染性能卓越。

### ADR-004：基于 ActivityTracker 的智能上下文自动回退 (Smart Context)
* **背景**：在 Android 中弹出窗口必须持有 Activity 的 WindowToken，否则抛出致命的 `WindowManager.BadTokenException`；但在 ViewModel 或后台任务中往往难以方便持有 Activity。
* **决策**：`IDialogProvider<Context>` 的 `context` 参数可省略，内部通过 `ActivityTracker`（`Application.IActivityLifecycleCallbacks`）自动获取当前处于前台活跃状态的 Top Activity 托管展示。
* **收益**：真正实现无感知安全呼出弹窗，兼顾灵活性与安全性。

### ADR-005：基于 SAF (Storage Access Framework) 的无权限安全文件桥接
* **背景**：现代 Android 实施严格分区存储（Scoped Storage），传统文件路径访问受限且需要申请危险权限。
* **决策**：通过轻量透明的 `SafBridgeActivity` 异步调度系统原生文档选择与目录授权树，以 `Task<IReadOnlyList<string>>` 返回 URI/路径。
* **收益**：无需向用户索取敏感存储权限，100% 满足 Google Play 与各大应用市场安全合规要求。
