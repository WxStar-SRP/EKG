namespace WxStar_EKG;

public class Program
{
    static async Task Main(string[] args)
    {
        await MqttPublisher.Connect();
        
        Console.WriteLine("WeatherStar EKG Daemon");
        Console.WriteLine("May your i2 not fall 2 hours behind <3");
        Console.WriteLine("======================================");
        
        while (true)
        {
            try
            {
                DateTime currentNetTime = NtpClient.GetNetworkTime();

                string currentNetTimeStr = currentNetTime.ToString("MM/dd/yyyy hh:mm:ss.000 tt");
                await MqttPublisher.PublishCommand($"heartbeat(Time={currentNetTimeStr})", "i2m/heartbeat");

                await Task.Delay(1000 * 60 * 5); // 5-minute intervals
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to post heartbeat, trying again in 5 minutes..");
                Console.WriteLine("Either NTP is blocked or we're being rate limited.");
                await Task.Delay(1000 * 60 * 5);
            }
        }
    }
}