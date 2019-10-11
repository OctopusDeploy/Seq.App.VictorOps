using System;
using Seq.Apps.LogEvents;

namespace Seq.App.VictorOps
{
    public static class IdentifierParser
    {
        public static string GetCustomerIdentifier(LogEventData data)
        {
            string[] propertyNamesSearchOrder = new[]
            {
                "InstanceId",
                "CustomerId",
                "OctopusServerId"
            };

            return FindProperty(data, propertyNamesSearchOrder);
        }

        public static string GetEnvironmentIdentifier(LogEventData data)
        {
            string[] propertyNamesSearchOrder = new[]
            {
                "Environment"
            };

            return FindProperty(data, propertyNamesSearchOrder);
        }

        public static string GetRegionIdentifier(LogEventData data)
        {
            string[] propertyNamesSearchOrder = new[]
            {
                "Reef",
                "Region"
            };

            return FindProperty(data, propertyNamesSearchOrder);
        }

        private static string FindProperty(LogEventData data, string[] propertyNamesSearchOrder)
        {
            foreach (var propertyName in propertyNamesSearchOrder)
            {
                foreach (var dataProperty in data.Properties)
                {
                    if (propertyName.Equals(dataProperty.Key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return dataProperty.Value.ToString();
                    }
                }
            }

            return null;
        }
    }
}