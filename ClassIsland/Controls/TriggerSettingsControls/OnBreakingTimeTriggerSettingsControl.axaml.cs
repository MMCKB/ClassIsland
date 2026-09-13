using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Models.Automation.Triggers;

namespace ClassIsland.Controls.TriggerSettingsControls;

/// <summary>
/// OnBreakingTimeTriggerSettingsControl.axaml 的交互逻辑
/// </summary>
public partial class OnBreakingTimeTriggerSettingsControl : TriggerSettingsControlBase<OnBreakingTimeTriggerSettings>
{
    public IProfileService ProfileService { get; } = App.GetService<IProfileService>();

    public OnBreakingTimeTriggerSettingsControl()
    {
        InitializeComponent();
    }
}
