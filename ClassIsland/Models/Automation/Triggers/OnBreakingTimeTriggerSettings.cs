using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Models.Automation.Triggers;

/// <summary>
/// 课间休息触发器设置。（issue #2002）
/// </summary>
public class OnBreakingTimeTriggerSettings : ObservableRecipient
{
    private bool _isSubjectFilterEnabled = false;

    public bool IsSubjectFilterEnabled
    {
        get => _isSubjectFilterEnabled;
        set
        {
            if (value == _isSubjectFilterEnabled) return;
            _isSubjectFilterEnabled = value;
            OnPropertyChanged();
        }
    }

    private Guid _selectedSubjectId = Guid.Empty;

    public Guid SelectedSubjectId
    {
        get => _selectedSubjectId;
        set
        {
            if (value == _selectedSubjectId) return;
            _selectedSubjectId = value;
            OnPropertyChanged();
        }
    }
}
