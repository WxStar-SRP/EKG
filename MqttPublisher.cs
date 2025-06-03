using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;
using MQTTnet.Client;

namespace WxStar_EKG;

public class MqttPublisher
{
    public static IMqttClient Client;
    private static string HOST = Config.config.MqttHost;
    private static int PORT = Config.config.MqttPort;
    private static string USERNAME = Config.config.MqttUser;
    private static string PASSWORD = Config.config.MqttPass;

    public static async Task Connect()
    {
        var factory = new MqttFactory();
        Client = factory.CreateMqttClient();
        await Client.ConnectAsync(new MqttClientOptionsBuilder()
            .WithClientId("EKG")
            .WithTcpServer(HOST, PORT)
            .WithCredentials(USERNAME, PASSWORD)
            .Build());
        
        Console.WriteLine("Connected to MQTT broker");
    }

    public static async Task Disconnect()
    {
        await Client.DisconnectAsync();
    }

    public static async Task PublishCommand(string command, string topic)
    {
        MqttCommand mqttCommand = new MqttCommand { Command = command };

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("wxstar/heartbeat")
            .WithPayload(JsonSerializer.Serialize(mqttCommand))
            .Build();

        await Client.PublishAsync(applicationMessage);
        
        Console.WriteLine("MQTT command published.");
    }
}

public class MqttCommand
{
    [JsonPropertyName("cmd")] public string Command { get; set; }
}