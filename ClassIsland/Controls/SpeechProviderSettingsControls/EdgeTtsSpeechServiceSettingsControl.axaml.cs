using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Interactivity;
using Avalonia.Platform;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Services;
using ClassIsland.Shared.Interfaces;
using Edge_tts_sharp;
using Edge_tts_sharp.Model;
using Microsoft.Extensions.Logging;

namespace ClassIsland.Controls.SpeechProviderSettingsControls;

/// <summary>
/// EdgeTtsSpeechServiceSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class EdgeTtsSpeechServiceSettingsControl : SpeechProviderControlBase
{
    private ILogger<EdgeTtsSpeechServiceSettingsControl> Logger { get; }

    public SettingsService SettingsService { get; }

    public IAudioService AudioService { get; }

    public List<eVoice> EdgeVoices { get; } =
        EdgeTts.GetVoice().FindAll(i => i.Locale.Contains("zh-CN"));

    /// <summary>
    /// 可选的音频输出设备列表，第一项代表跟随系统默认输出设备。
    /// </summary>
    public ObservableCollection<EdgeTtsPlaybackDeviceItem> PlaybackDevices { get; } = new()
    {
        new EdgeTtsPlaybackDeviceItem("")
    };

    private CancellationTokenSource? _testPlaybackCancellationTokenSource;

    public EdgeTtsSpeechServiceSettingsControl(SettingsService settingsService, IAudioService audioService,
        ILogger<EdgeTtsSpeechServiceSettingsControl> logger)
    {
        SettingsService = settingsService;
        AudioService = audioService;
        Logger = logger;
        InitializeComponent();
        _ = RefreshPlaybackDevicesAsync();
    }

    private async Task RefreshPlaybackDevicesAsync()
    {
        try
        {
            // 音频引擎只能在 MTA 线程上访问，因此设备枚举需要放到后台线程进行。
            var deviceNames = await AudioService.GetPlaybackDeviceNamesAsync();
            foreach (var deviceName in deviceNames)
            {
                PlaybackDevices.Add(new EdgeTtsPlaybackDeviceItem(deviceName));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "枚举音频输出设备失败。");
        }
    }

    private async void ButtonTestPlaybackDevice_OnClick(object? sender, RoutedEventArgs e)
    {
        _testPlaybackCancellationTokenSource?.Cancel();
        _testPlaybackCancellationTokenSource = new CancellationTokenSource();
        try
        {
            await using var stream = AssetLoader.Open(INotificationProvider.DefaultNotificationSoundUri);
            await AudioService.PlayAudioAsync(stream, (float)SettingsService.Settings.SpeechVolume,
                SettingsService.Settings.EdgeTtsPlaybackDevice, _testPlaybackCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "播放 EdgeTTS 输出设备测试音频失败。");
        }
    }
}

/// <summary>
/// 表示一个可供 EdgeTTS 选择的音频输出设备。
/// </summary>
/// <param name="DeviceName">设备名称；空字符串代表跟随系统默认输出设备。</param>
public record EdgeTtsPlaybackDeviceItem(string DeviceName)
{
    public string DisplayName => string.IsNullOrWhiteSpace(DeviceName) ? "跟随系统默认输出设备" : DeviceName;
}
