using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums;
using ClassIsland.Models.Automation.Triggers;
using Microsoft.Extensions.Logging;

namespace ClassIsland.Services.Automation.Triggers;

[TriggerInfo("classisland.lifetime.startup", "应用启动时", "\ue067")]
public class AppStartupTrigger : TriggerBase<AppStartupTriggerSettings>
{
    CancellationTokenSource? _waitCancellationTokenSource;

    ILogger<AppStartupTrigger> Logger { get; } = App.GetService<ILogger<AppStartupTrigger>>();

    public override void Loaded()
    {
        if (AppBase.CurrentLifetime >= ApplicationLifetime.Running)
            return;

        if (!Settings.IsWaitNetworkEnabled)
        {
            Trigger();
            return;
        }

        _waitCancellationTokenSource = new CancellationTokenSource();
        _ = WaitNetworkThenTriggerAsync(_waitCancellationTokenSource.Token);
    }

    public override void UnLoaded()
    {
        _waitCancellationTokenSource?.Cancel();
        _waitCancellationTokenSource = null;
    }

    /// <summary>
    /// 等待网络连接后再触发自动化。超过设置的最长等待时间后，无论网络是否连接都会触发自动化。
    /// </summary>
    async Task WaitNetworkThenTriggerAsync(CancellationToken cancellationToken)
    {
        Logger.LogTrace("启动自动化正在等待网络连接…");
        try
        {
            var stopwatch = Stopwatch.StartNew();
            while (!await NetworkConnectivity.IsNetworkConnectedAsync(cancellationToken))
            {
                if (IsTimeoutExceeded(stopwatch))
                {
                    Logger.LogWarning("等待网络连接超时（{} 秒），继续触发启动自动化。", Settings.Timeout);
                    break;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 触发器已被卸载（如切换了自动化配置方案），此时不应再触发自动化。
            return;
        }

        Dispatcher.UIThread.Invoke(Trigger);
    }

    bool IsTimeoutExceeded(Stopwatch stopwatch)
    {
        return Settings.Timeout > 0 && stopwatch.ElapsedMilliseconds >= Settings.Timeout * 1000;
    }
}
