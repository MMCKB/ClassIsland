using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services;

namespace ClassIsland.Controls.NotificationProviders;

public partial class ClassNotificationProviderControl : UserControl, INotifyPropertyChanged
{
    private object? _element;
    private string _message = "";
    private int _slideIndex = 0;
    private bool _showTeacherName = false;
    private string _maskMessage = "";

    public object? Element
    {
        get => _element;
        set
        {
            if (Equals(value, _element)) return;
            _element = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            if (value == _message) return;
            _message = value;
            OnPropertyChanged();
        }
    }

    public int SlideIndex
    {
        get => _slideIndex;
        set
        {
            if (value == _slideIndex) return;
            _slideIndex = value;
            OnPropertyChanged();
        }
    }

    public bool ShowTeacherName
    {
        get => _showTeacherName;
        set
        {
            if (value == _showTeacherName) return;
            _showTeacherName = value;
            OnPropertyChanged();
        }
    }

    public string MaskMessage
    {
        get => _maskMessage;
        set
        {
            if (value == _maskMessage) return;
            _maskMessage = value;
            OnPropertyChanged();
        }
    }

    private string _key = "";

    public ILessonsService LessonsService { get; } = App.GetService<ILessonsService>();

    // 通知内容应当在触发时刻定格。课间可能短于提醒的显示时长，
    // 若模板实时绑定课程服务，课间结束后显示的内容会变成下一个时间点的信息。
    // 因此这里在控件创建（即提醒触发）时对展示用的时间点信息做快照。
    public string BreakNameText { get; }
    public string BreakDurationText { get; }
    public string NextClassName { get; }
    public string NextClassTeacherName { get; }
    public TimeSpan NextStartTime { get; }
    public TimeSpan NextEndTime { get; }

    private DispatcherTimer Timer { get; } = new()
    {
        Interval = TimeSpan.FromSeconds(10)
    };

    public ClassNotificationProviderControl(string key)
    {
        InitializeComponent();
        BreakNameText = LessonsService.CurrentTimeLayoutItem.BreakNameText;
        BreakDurationText = FormatTimeSpan(LessonsService.CurrentTimeLayoutItem.Last);
        NextClassName = LessonsService.NextClassSubject.Name;
        NextClassTeacherName = LessonsService.NextClassSubject.TeacherName;
        NextStartTime = LessonsService.NextClassTimeLayoutItem.StartTime;
        NextEndTime = LessonsService.NextClassTimeLayoutItem.EndTime;
        var visual = this.FindResource(key) as Control;
        Element = visual;
        _key = key;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        MainListBox.SelectedIndex = 0;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_key is "ClassPrepareNotifyOverlay" or "ClassOffOverlay")
        {
            Timer.Start();
        }        
        Timer.Tick += TimerOnTick;
        MainListBox.SelectedIndex = SlideIndex;
    }

    private void OnUnloaded(object? o, RoutedEventArgs routedEventArgs)
    {
        Timer.Stop();
        Timer.Tick -= TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Message))
            return;
        MainListBox.SelectedIndex = SlideIndex = SlideIndex == 1 ? 0 : 1;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public static string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalSeconds <= 0) return "0 分钟";

        var parts = new List<string>(3);
        
        if (span.Hours > 0) parts.Add($"{span.Hours} 小时");
        if (span.Minutes > 0)
        {
            if (span.Seconds > 0) parts.Add($"{span.Minutes} 分");
            else parts.Add($"{span.Minutes} 分钟");
        }
        if (span.Seconds > 0) parts.Add($"{span.Seconds} 秒");
    
        return string.Join(" ", parts);
    }
}
