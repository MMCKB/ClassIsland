using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Models.Actions;
namespace ClassIsland.Services.Automation.Actions;

[ActionInfo("classisland.action.waitNetwork", "等待网络连接", "\uE701")]
public class WaitNetworkAction : ActionBase<WaitNetworkActionSettings>
{
    readonly Stopwatch _sw = Stopwatch.StartNew();

    protected override async Task OnInvoke()
    {
        await base.OnInvoke();

        try
        {
            while (!await NetworkConnectivity.IsNetworkConnectedAsync(InterruptCancellationToken))
            {
                if (IsTimeoutExceeded())
                {
                    ActionItem.Progress = 100;
                    break;
                }

                UpdateProgress();
                await Task.Delay(1000, InterruptCancellationToken);
            }
        }
        catch (OperationCanceledException) { }
    }

    bool IsTimeoutExceeded()
    {
        return Settings.Timeout > 0 && _sw.ElapsedMilliseconds >= Settings.Timeout * 1000;
    }

    void UpdateProgress()
    {
        // 未设置等待时限（一直等待）时无法估计剩余时间，因此不报告进度。
        if (Settings.Timeout <= 0) return;
        ActionItem.Progress = Math.Min(_sw.ElapsedMilliseconds * 100 / (Settings.Timeout * 1000), 100);
    }
}
