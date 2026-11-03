# ApeFree.Apedroid

`ApeFree.Apedroid` 是一个基于 **.NET 10 (`net10.0-android`)** 的现代 Android 聚合扩展库与 UI 套件，深度适配 Google **Material Design 3 (Material You)** 视觉语言，并全面落地 **`ApeFree.ApeDialogs`** 跨平台标准规范下的 **全量 19 种高颜值标准弹窗**。

---

## 🌟 核心特性

* **🚀 现代技术底座**：全面告别已过时的 Xamarin.Android，基于 .NET 10 SDK 风格单包聚合架构构建，统一引用，开箱即用。
* **🎨 Material 3 高颜值设计**：
  * **28dp 大圆角卡片**、精细多层阴影（Elevation）与平滑淡入淡出动效；
  * **ReminderColor 顶置醒目指示条**：传承 ApeForms 经典视觉，直观传达信息等级；
  * **水波纹反馈 (Ripple Effect)** 与语义化按钮配色（`DialogOptionTag` 自动适配）。
* **📐 100% 对齐 ApeDialogs 标准契约**：
  * 完美实现 `IDialogProvider<Context>`，实现跨平台业务逻辑一次编写、全端通用；
  * 具备自动校验拦截（`TrySubmit` / `Validator`）、倒计时自动关闭（`Duration`）、不可变结果模型（`DialogResult<T>.Canceled` 无异常流控制）。
* **🧠 智能 Context 机制 (Smart Context)**：
  * 支持传入具体 `Activity` 或 `Context`；
  * 内置全局 `ActivityTracker` 顶层活跃 Activity 自动追踪，支持在 ViewModel 或后台业务层**无参数直接呼出弹窗**（杜绝 `BadTokenException`）。
* **📁 现代 Storage Access Framework (SAF) 存储桥接**：
  * 无需手动声明危险文件管理权限，免重写 `OnActivityResult`，直接 `await` 系统文档与目录授权结果。

---

## 📦 架构与工程结构

```
ApeFree.Apedroid.slnx
│
├── src/
│   └── ApeFree.Apedroid/                  [单一聚合核心主库 (net10.0-android)]
│       ├── Core/                          ActivityTracker, 线程调度与基础扩展
│       ├── Dialogs/
│       │   ├── Adapters/                  ApeColor, DialogFont, 尺寸与样式适配器
│       │   ├── Hosting/                   ApedroidDialogHost (Material 3 卡片宿主)
│       │   ├── Core/                      ApedroidDialogBase, ApedroidAdjustDialog
│       │   ├── Views/                     19 种标准 Dialog 实现与 DataEntryView 表单引擎
│       │   ├── Saf/                       SAF 存储框架异步桥接器 (SafBridge)
│       │   ├── Providers/                 ApedroidDialogProvider : DialogProvider<Context>
│       │   └── Extensions/                DialogContextExtensions 便捷扩展
│
├── samples/
│   └── ApeFree.Apedroid.Demo/             [全功能交互演示 App (net10.0-android)]
│       └── 包含 19 种弹窗的交互展示、实时推流试验场与日志面板
│
└── tests/
    └── ApeFree.Apedroid.Tests/            [单元测试与契约验证工程]
```

---

## 📋 19 种标准 Dialog 支持矩阵

| 分类 | 对话框类型 | 返回值类型 | 界面与交互说明 |
|---|---|---|---|
| **1. 基础信息类** | `MessageDialog` | `bool` | 标准信息卡片，支持长文本折行与确认关闭 |
| | `PromptDialog` | `bool` | 询问卡片，提供确认/取消二元选择 |
| | `TextDialog` | `bool` | 协议/大段长文本，内置平滑滚动容器并支持文字长按复制 |
| | `ToastDialog` | `bool` | 浮动轻提示卡片，支持按 `Duration` 自动倒计时淡出消失 |
| **2. 输入与表单类** | `InputDialog` | `string` | M3 输入框，支持单/多行、最大字数限制、实时字符统计与回车提交 |
| | `PasswordDialog` | `string` | 密码输入框，带眼睛图标快捷切换明文与密文 |
| | `NumberInputDialog` | `decimal` | 数值步进器（[-] 计数框 [+]），支持精度限制与微调 |
| | `ComboBoxInputDialog`| `string` | 可编辑下拉框（Exposed Dropdown Menu） |
| | `DataEntrySheetDialog`| `DataEntrySheet`| 动态表单引擎，根据 SheetField 自动生成复杂控件列表与联动校验 |
| **3. 数据选择类** | `SelectionDialog<T>` | `T` | 单选列表卡片，基于 MaterialRadioButton 单选组 |
| | `MultipleSelectionDialog<T>`| `IReadOnlyList<T>`| 多选复选框列表卡片，基于 MaterialCheckBox |
| | `DateTimeDialog` | `DateTime` | 支持 Date、Time 及完整 DateTime 精度轮盘选择 |
| | `ColorDialog` | `ApeColor` | 包含实时色块预览、RGB/Alpha 调节滑块与快捷预设调色盘 |
| **4. 调节控制类** | `SliderDialog` | `decimal` | 单滑块无级调节，拖动过程实时推流 `ValueChanged` 事件 |
| | `RangeSliderDialog` | `ValueRange<decimal>`| 双滑块区间选择，带浮动指示与实时推流 |
| | `ProgressDialog` | `IProgressDialog` | 支持确定型线性进度条与不确定型动画，动态刷新进度与消息 |
| **5. 存储交互类** | `OpenFileDialog` | `IReadOnlyList<string>`| SAF 系统文档选择器，支持 MIME 过滤与多选 |
| | `SaveFileDialog` | `string` | SAF 系统文档创建器，支持默认文件名与目标 URI 返回 |
| | `OpenFolderDialog` | `IReadOnlyList<string>`| SAF 目录授权树选择器 |

---

## 🚀 快速上手

### 1. 初始化 ActivityTracker（可选但推荐）
在自定义的 `Application` 或主 `Activity.OnCreate` 中注入生命周期监听：

```csharp
using ApeFree.Apedroid.Core;

public class MainApplication : Application
{
    public override void OnCreate()
    {
        base.OnCreate();
        ActivityTracker.Init(this);
    }
}
```

### 2. 便捷调用弹窗

```csharp
using ApeFree.Apedroid.Dialogs.Extensions;

// 1. 弹出消息
var r1 = await this.ShowMessageDialogAsync("系统通知", "操作已成功处理！");
if (!r1.IsCanceled)
{
    // 用户点击了确认
}

// 2. 文本输入
var r2 = await this.ShowInputDialogAsync("修改昵称", "请输入新昵称：", "ApeFree");
if (!r2.IsCanceled)
{
    string newName = r2.Value;
}

// 3. 单选列表
var fruits = new[] { "苹果", "香蕉", "橙子", "西瓜" };
var r3 = await this.ShowSelectionDialogAsync("选择水果", fruits, "香蕉");

// 4. 复杂动态表单 (DataEntrySheet)
var sheet = new DataEntrySheet();
sheet.AddField(new TextField { Title = "服务器地址", Value = "127.0.0.1" });
sheet.AddField(new NumberField { Title = "端口号", Value = 8080 });
sheet.AddField(new PasswordField { Title = "访问令牌", Value = "token_xyz" });
var r4 = await this.ShowDataEntrySheetDialogAsync("连接配置", sheet);

// 5. 无 Context 全局弹窗（自动回退到 Top Activity）
await DialogContextExtensions.ShowToastAsync("数据已自动同步", TimeSpan.FromSeconds(2));
```

---

## 📄 开源许可证

本项目遵循 [Apache-2.0](LICENSE) 许可证。
