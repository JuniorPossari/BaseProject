using Microsoft.Extensions.Configuration;

namespace BaseProject.Util
{
	public static class ConfigurationExtensions
    {

        public static T GetProperty<T>(this IConfiguration config, string section, string prop)
        {
            IConfigurationSection configSection = config.GetSection(section);
            return configSection.GetValue<T>(prop);
        }

        public static T GetProperty<T>(this IConfiguration config, string prop)
        {
            return config.GetValue<T>(prop);
        }


    }
}
