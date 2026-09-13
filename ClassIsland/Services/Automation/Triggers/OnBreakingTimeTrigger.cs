using System;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Models.Automation.Triggers;

namespace ClassIsland.Services.Automation.Triggers;

[TriggerInfo("classisland.lessons.onBreakingTime", "课间休息时", "\ue4c4")]
public class OnBreakingTimeTrigger(ILessonsService lessonsService) : TriggerBase<OnBreakingTimeTriggerSettings>
{
    private ILessonsService LessonsService { get; } = lessonsService;

    private IProfileService ProfileService { get; } = App.GetService<IProfileService>();

    public override void Loaded()
    {
        LessonsService.OnBreakingTime += LessonsServiceOnOnBreakingTime;
    }
    public override void UnLoaded()
    {
        LessonsService.OnBreakingTime -= LessonsServiceOnOnBreakingTime;
    }

    private void LessonsServiceOnOnBreakingTime(object? sender, EventArgs e)
    {
        // 启用科目过滤时，仅在刚结束课程的科目与所选科目一致时触发。
        // 课间时 CurrentSubject 是伪科目“课间”，因此使用 GetLastClassSubject 获取刚结束课程的科目。（issue #2002）
        if (Settings.IsSubjectFilterEnabled)
        {
            var lastClassSubject = LessonsService.GetLastClassSubject();
            var matched = ProfileService.Profile.Subjects.TryGetValue(Settings.SelectedSubjectId, out var expected) &&
                          ReferenceEquals(expected, lastClassSubject);
            if (!matched)
            {
                return;
            }
        }
        Trigger();
    }
}
