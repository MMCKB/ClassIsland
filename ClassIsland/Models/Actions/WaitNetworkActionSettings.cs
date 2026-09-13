using CommunityToolkit.Mvvm.ComponentModel;
namespace ClassIsland.Models.Actions;

public class WaitNetworkActionSettings : ObservableRecipient
{
    double _timeout = 120;

    /// <summary>
    /// 等待网络连接的最长时间（秒）。为 0 时将一直等待，直到网络连接成功。
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
