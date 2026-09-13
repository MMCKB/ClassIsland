using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Models.Automation.Triggers;

public class AppStartupTriggerSettings : ObservableRecipient
{
    bool _isWaitNetworkEnabled;
    double _timeout = 60;

    /// <summary>
    /// 触发自动化前等待网络连接。
    /// </summary>
    public bool IsWaitNetworkEnabled
    {
        get => _isWaitNetworkEnabled;
        set
        {
            if (value == _isWaitNetworkEnabled) return;
            _isWaitNetworkEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 等待网络连接的最长时间（秒）。超过此时间后，无论网络是否连接都会触发自动化。为 0 表示一直等待。
    /// </summary>
    public double Timeout
    {
        get => _timeout;
        set
        {
            if (value == _timeout) return;
            _timeout = value;
            OnPropertyChanged();
        }
    }
}
