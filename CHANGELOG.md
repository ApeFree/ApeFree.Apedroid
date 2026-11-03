# 更新日志 (Change Log)

All notable changes to this project will be documented in this file.

## [1.0.0-alpha1] - 2026-11-04

### 重构与全新架构
* **升级至 .NET 10**：全面重写底层运行时，升级为 SDK-style `net10.0-android`；
* **单包聚合**：推倒原分散的 5 个子工程，聚合成单一核心程序集 `ApeFree.Apedroid`；
* **旧版本归档**：历史 Xamarin.Android 代码已完整固定于 `legacy` 分支。

### 弹窗与 UI (ApeDialogs 对齐)
* **全量落地 19 种标准 Dialog**：
  * **基础信息类**：MessageDialog, PromptDialog, TextDialog, ToastDialog；
  * **输入与表单类**：InputDialog, PasswordDialog, NumberInputDialog, ComboBoxInputDialog, DataEntrySheetDialog (含动态表单引擎 DataEntryView)；
  * **数据选择类**：SelectionDialog<T>, MultipleSelectionDialog<T>, DateTimeDialog, ColorDialog；
  * **调节与过程类**：SliderDialog, RangeSliderDialog, ProgressDialog (含 IProgressDialog 接口推流)；
  * **存储交互类**：OpenFileDialog, SaveFileDialog, OpenFolderDialog (基于 SAF 存储访问框架)；
* **Material 3 现代视觉风格**：28dp 大圆角卡片、Elevation 微投影、ReminderColor 顶部醒目条、水波纹反馈与语义标签色彩映射；
* **智能 Context 机制**：内置 `ActivityTracker`，支持在 ViewModel 或无 Activity 场景下自动回退至前台 Activity 呼出弹窗；
* **极简扩展**：引入 `DialogContextExtensions`，支持一行代码异步调用。

### 演示与测试
* **Demo 演示工程**：创建 `ApeFree.Apedroid.Demo`，涵盖 19 种弹窗的交互式功能试验场；
* **单元测试工程**：创建 `ApeFree.Apedroid.Tests`，覆盖颜色转换、字体映射、接口契约与表单验证。
