using System.Xml.Serialization;

namespace WxStar_EKG;

[XmlRoot("ServerConfig")]
public class Config
{
    public static string ConfigPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "StarEKG.config");

    [XmlElement] public string NtpPool { get; set; } = "pool.ntp.org";
    [XmlElement] public string MqttHost { get; set; } = "127.0.0.1";
    [XmlElement] public int MqttPort { get; set; } = 1883;
    [XmlElement] public string MqttUser { get; set; } = "REPLACE_ME";
    [XmlElement] public string MqttPass { get; set; } = "REPLACE_ME";


    public static Config config = new();

    public static Config Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("", "");

        if (!File.Exists(ConfigPath))
        {
            config = new Config();
            serializer.Serialize(File.Create(ConfigPath), config, ns);

            return config;
        }

        using (FileStream fs = new FileStream(ConfigPath, FileMode.Open))
        {
            var deserializedConfig = serializer.Deserialize(fs);

            if (deserializedConfig != null && deserializedConfig is Config cfg)
            {
                config = cfg;
                return config;
            }

            return new Config();
        }
    }
}