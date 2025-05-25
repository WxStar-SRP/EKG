using System.Net;
using System.Net.Sockets;

namespace WxStar_EKG;

public class NtpClient
{
    public static DateTime GetNetworkTime()
    {
        const string ntpServer = "pool.ntp.org";
        var ntpData = new byte[48];

        ntpData[0] = 0x1B;
        
        var addresses = Dns.GetHostEntry(ntpServer).AddressList;
        var ipEndpoint = new IPEndPoint(addresses[0], 123);

        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndpoint);
            socket.ReceiveTimeout = 3000; // Stops code hang if NTP is blocked
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
        }

        const byte serverReplyTime = 40;
        
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);
        
        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        
        // Get UTC time 
        var time = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .AddMilliseconds((long)milliseconds);

        Console.WriteLine($"NETWORK TIME: {time.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
        
        return time;
    }
    
    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x)
    {
        return (uint) (((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}