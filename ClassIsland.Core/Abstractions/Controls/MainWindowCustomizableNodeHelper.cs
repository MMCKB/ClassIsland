using Avalonia.Controls;
using ClassIsland.Core.Abstractions.Models.Components;
using ClassIsland.Core.Assists;
using ClassIsland.Core.Helpers.UI;

namespace ClassIsland.Core.Abstractions.Controls;

/// <summary>
/// 主界面可自定义节点助手类
/// </summary>
public static class MainWindowCustomizableNodeHelper
{
    /// <summary>
    /// 应用样式
    /// </summary>
    /// <param name="control">要应用样式的控件</param>
    /// <param name="settings">控件设置</param>
    public static void ApplyStyles(Control control, IMainWindowCustomizableNodeSettings settings)
    {
        if (settings.IsResourceOverridingEnabled)
        {
            control.Resources[nameof(settings.MainWindowSecondaryFontSize)] = settings.MainWindowSecondaryFontSize;
            control.Resources[nameof(settings.MainWindowBodyFontSize)] = settings.MainWindowBodyFontSize;
            control.Resources[nameof(settings.MainWindowEmphasizedFontSize)] = settings.MainWindowEmphasizedFontSize;
            control.Resources[nameof(settings.MainWindowLargeFontSize)] = settings.MainWindowLargeFontSize;
        }
        else
        {
            foreach (var key in (string[])
                     [
                         nameof(settings.MainWindowSecondaryFontSize), nameof(settings.MainWindowBodyFontSize),
                         nameof(settings.MainWindowEmphasizedFontSize), nameof(settings.MainWindowLargeFontSize)
                     ])
            {
                control.Resources.Remove(key);
            }
        }

        if (settings.IsCustomCornerRadiusEnabled)
        {
            control.SetValue(MainWindowStylesAssist.CornerRadiusProperty, settings.CustomCornerRadius);
        }
        else
        {
            control.ClearValue(MainWindowStylesAssist.CornerRadiusProperty);
        }
        if (settings.IsCustomBackgroundColorEnabled)
        {
            control.SetValue(MainWindowStylesAssist.BackgroundCorlorProperty, settings.BackgroundColor);
            control.SetValue(MainWindowStylesAssist.IsCustomBackgroundColorEnabledProperty, true);
        }
        else
        {
            control.ClearValue(MainWindowStylesAssist.BackgroundCorlorProperty);
            control.ClearValue(MainWindowStylesAssist.IsCustomBackgroundColorEnabledProperty);
        }
        if (settings.IsCustomBackgroundOpacityEnabled)
        {
            control.SetValue(MainWindowStylesAssist.BackgroundOpacityProperty, settings.BackgroundOpacity);
        }
        else
        {
            control.ClearValue(MainWindowStylesAssist.BackgroundOpacityProperty);
        }

        if (settings.IsCustomForegroundColorEnabled)
        {
            ControlColorHelper.SetControlForegroundColor(control, settings.ForegroundColor,
                settings.IsCustomForegroundColorEnabled);
        }
        else if (settings.IsCustomBackgroundColorEnabled)
        {
            // 自定义了背景色但未自定义前景色时，按背景亮度自动切换黑/白前景色，
            // 避免浅色背景上的白色文字不可读。（issue #689）
            var bg = settings.BackgroundColor;
            var luminance = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255.0;
            var autoForeground = luminance > 0.6 ? Avalonia.Media.Colors.Black : Avalonia.Media.Colors.White;
            ControlColorHelper.SetControlForegroundColor(control, autoForeground, true);
        }
        else
        {
            ControlColorHelper.SetControlForegroundColor(control, settings.ForegroundColor,
                settings.IsCustomForegroundColorEnabled);
        }
    }
}