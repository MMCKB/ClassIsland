using ClassIsland.Shared.Abstraction.Models;
using ClassIsland.Shared.Enums;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassIsland.Models.NotificationProviderSettings;

public class WeatherNotificationProviderSettings : ObservableRecipient, IWeatherNotificationSettingsBase
{
    private NotificationModes _alertShowMode;
    private NotificationModes _forecastShowMode;
    private bool _isAlertEnabled = true;
    private bool _isForecastEnabled = true;
    private double _weatherAlertSpeed = 7.0;

    public NotificationModes AlertShowMode
    {
        get => _alertShowMode;
        set
        {
            if (value == _alertShowMode) return;
            _alertShowMode = value;
            OnPropertyChanged();
        }
    }

    public NotificationModes ForecastShowMode
    {
        get => _forecastShowMode;
        set
        {
            if (value == _forecastShowMode) return;
            _forecastShowMode = value;
            OnPropertyChanged();
        }
    }

    public bool IsAlertEnabled
    {
        get => _isAlertEnabled;
        set
        {
            if (value == _isAlertEnabled) return;
            _isAlertEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsForecastEnabled
    {
        get => _isForecastEnabled;
        set
        {
            if (value == _isForecastEnabled) return;
            _isForecastEnabled = value;
            OnPropertyChanged();
        }
    }

    public double WeatherAlertSpeed
    {
        get => _weatherAlertSpeed;
        set
        {
            if (value.Equals(_weatherAlertSpeed)) return;
            _weatherAlertSpeed = value;
            OnPropertyChanged();
        }
    }

    private bool _isAlertDurationCustomEnabled = false;

    public bool IsAlertDurationCustomEnabled
    {
        get => _isAlertDurationCustomEnabled;
        set
        {
            if (value == _isAlertDurationCustomEnabled) return;
            _isAlertDurationCustomEnabled = value;
            OnPropertyChanged();
        }
    }

    private double _alertTitleDurationSeconds = 5.0;

    public double AlertTitleDurationSeconds
    {
        get => _alertTitleDurationSeconds;
        set
        {
            if (value.Equals(_alertTitleDurationSeconds)) return;
            _alertTitleDurationSeconds = value;
            OnPropertyChanged();
        }
    }

    private double _alertContentDurationSeconds = 20.0;

    public double AlertContentDurationSeconds
    {
        get => _alertContentDurationSeconds;
        set
        {
            if (value.Equals(_alertContentDurationSeconds)) return;
            _alertContentDurationSeconds = value;
            OnPropertyChanged();
        }
    }
}