using System.Reflection;
using Confluent.Kafka;

namespace Blackened.Blue.Diagnostics.HealthChecks.Kafka;

public sealed class KafkaConnection : AdminClientConfig
{
    private static readonly Dictionary<string, string> PropertyAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Username"] = nameof(SaslUsername),
        ["Password"] = nameof(SaslPassword),
    };

    public KafkaConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        foreach (var entry in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = entry.Split('=', 2);

            if (pair.Length != 2)
                throw new ArgumentException(
                    $"Malformed Kafka connection string entry '{entry}'.",
                    nameof(connectionString));

            var propertyName = PropertyAliases.GetValueOrDefault(pair[0], pair[0]);

            var property = GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property is null || !property.CanWrite)
                throw new ArgumentException(
                    $"Unknown Kafka connection string property '{pair[0]}'.",
                    nameof(connectionString));

            object? value = ConvertValue(pair[1], property.PropertyType);

            property.SetValue(this, value);
        }
    }

    private static object? ConvertValue(string value, Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(string))
            return value;

        if (type.IsEnum)
            return Enum.Parse(type, value, ignoreCase: true);

        return Convert.ChangeType(value, type);
    }
}
