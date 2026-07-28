using System.Reflection;
using Confluent.Kafka;

namespace Blackened.Blue.Diagnostics.HealthChecks.Kafka;

public static class OptionsBuilder
{
    private static readonly Dictionary<string, string> PropertyAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Username"] = "SaslUsername",
        ["Password"] = "SaslPassword",
    };

    public static AdminClientConfig UseKafka(string connectionString)
    {
        var config = new AdminClientConfig();

        foreach (var entry in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = entry.Split('=', 2);
            if (pair.Length != 2)
                throw new ArgumentException($"Malformed Kafka connection string entry '{entry}'.", nameof(connectionString));

            var propertyName = PropertyAliases.GetValueOrDefault(pair[0], pair[0]);
            var property = typeof(AdminClientConfig).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? throw new ArgumentException($"Unknown Kafka connection string property '{pair[0]}'.", nameof(connectionString));

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var value = propertyType.IsEnum ? Enum.Parse(propertyType, pair[1], ignoreCase: true) : Convert.ChangeType(pair[1], propertyType);

            property.SetValue(config, value);
        }

        return config;
    }
}
