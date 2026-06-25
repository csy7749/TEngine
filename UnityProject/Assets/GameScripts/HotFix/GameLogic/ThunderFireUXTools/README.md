# ThunderFire UXTools

这是从外部项目移植的完整 UXTools 工具集，提供丰富的 UI 开发功能。

## 📁 目录结构

```
ThunderFireUXTools/
├── Runtime/                    # 运行时组件
│   └── UXGUI/                 # UI 组件库
│       ├── Attributes/        # 自定义特性
│       ├── Components/        # UI 组件 (UXImage, UXText, SuperText 等)
│       └── Common/           # 通用工具
├── Editor/                    # Editor 扩展工具
│   ├── Analysis/             # 分析工具
│   ├── Common/               # 通用 Editor 工具
│   └── [其他模块]/           # 各种 Editor 功能
├── Res/                      # 资源文件
│   └── UX-GUI-Feature/       # UI 功能资源
└── 3rdTools/                 # 第三方工具集成
```

## 🔧 主要功能

### 运行时组件
- **UXImage**: 增强的 Image 组件，支持本地化、渐变色等
- **UXText**: 增强的 Text 组件，支持多语言处理
- **SuperText**: 高级文本组件
- **SuperImage**: 高级图像组件
- **SuperToggle**: 高级开关组件
- **SuperScrollView**: 高级滚动视图

### Editor 工具
- **分析工具**: 性能分析、使用统计
- **表格列表**: 数据表格编辑器
- **本地化工具**: 多语言支持
- **配置管理**: 项目配置工具

### 特殊功能
- **新手引导系统**: 完整的引导功能
- **本地化支持**: 多语言文本处理
- **阿拉伯语支持**: 特殊语言文本处理
- **泰语支持**: 字体调整功能

## ⚠️ 与现有 UI 框架的关系

**注意**: 这个 ThunderFireUXTools 与项目中的 `UI/UXTools` 是两个完全不同的系统：

- **UI/UXTools**: TEngine 新 UI 框架的核心组件，轻量级，专注于颜色和组件管理
- **ThunderFireUXTools**: 功能丰富的独立 UI 工具集，提供高级 UI 组件和 Editor 工具

两者可以并存使用，互不冲突。

## 📖 使用指南

1. **安装指南**: 参考 `1_Install Guides.pdf`
2. **使用教程**: 参考 `2_Using Tutorials.pdf`
3. **案例详解**: 参考 `3_Case Details.pdf`

## 🚀 快速开始

```csharp
// 使用 UXImage (ThunderFire 版本)
using UnityEngine.UI;

public class MyUI : MonoBehaviour
{
    public UXImage myImage; // 支持本地化和渐变色

    void Start()
    {
        // 设置渐变色
        myImage.m_ColorType = UXImage.ColorType.Gradient_Color;
    }
}
```

## 📝 更新日志

- **2026-03-13**: 从外部项目完整移植到 TEngine 框架
- 重命名为 ThunderFireUXTools 避免与现有 UI/UXTools 冲突
- 保持原有功能和目录结构完整性