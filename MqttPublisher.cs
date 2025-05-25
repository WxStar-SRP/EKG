using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;
using MQTTnet.Client;

namespace WxStar_EKG;

public class MqttPublisher
{
    public static IMqttClient Client;
    private static string HOST = "wxstar-data.cascadia.local";
    private static int PORT = 1883;
    private static string USERNAME = "MOON";
    private static string PASSWORD = "intellistar";

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
            .WithTopic("i2m/heartbeat")
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