using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClassIsland.Services.Automation;

/// <summary>
/// 网络连接状态检测。
/// </summary>
/// <remarks>
/// 网络接口可用（<see cref="NetworkInterface.GetIsNetworkAvailable"/>）不代表能够访问互联网，
/// 例如部分校园网在完成认证前网络接口已经连接，此时仍无法联网。
/// 因此除了检查网络接口状态外，还需要通过实际连接测试端点来判断网络是否可用。
/// </remarks>
public static class NetworkConnectivity
{
    static readonly (string Host, int Port)[] TestEndpoints =
    [
        ("223.5.5.5", 53),               // AliDNS
        ("119.29.29.29", 53),            // DNSPod
        ("www.msftconnecttest.com", 80), // Windows 网络连接测试服务
    ];

    /// <summary>
    /// 检查当前能否连接到互联网。
    /// </summary>
    public static async Task<bool> IsNetworkConnectedAsync(CancellationToken cancellationToken)
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return false;

        foreach (var (host, port) in TestEndpoints)
        {
            if (await CanConnectAsync(host, port, cancellationToken))
                return true;
        }

        return false;
    }

    static async Task<bool> CanConnectAsync(string host, int port, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new TcpClient();
            using var timeoutCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3));
            await client.ConnectAsync(host, port, timeoutCancellationTokenSource.Token);
            return client.Connected;
        }
        catch (Exception e) when (e is SocketException or ObjectDisposedException or OperationCanceledException)
        {
            return false;
        }
    }
}
