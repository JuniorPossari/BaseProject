using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BaseProject.Util
{
	public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum val)
        {
            return val.GetType()
                  .GetMember(val.ToString())
                  .FirstOrDefault()
                  ?.GetCustomAttribute<DisplayAttribute>(false)
                  ?.Description
                  ?? val.ToString();
        }

        public static string GetEnumDisplayName(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DisplayAttribute[])fi.GetCustomAttributes(
                typeof(DisplayAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].Name;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string GetDisplayShortName(this Enum val)
        {
            return val.GetType()
                      .GetMember(val.ToString())
                      .FirstOrDefault()
                      ?.GetCustomAttribute<DisplayAttribute>(false)
                      ?.ShortName
                      ?? val.ToString();
        }

        public static string GetEnumDisplayShortName(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DisplayAttribute[])fi.GetCustomAttributes(
                typeof(DisplayAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].ShortName;
            }
            else
            {
                return value.ToString();
            }
        }

        public static T GetValueFromName<T>(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics())
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics())
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }

        public static T GetValueFromShortName<T>(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.ShortName.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics())
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics())
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }

        public static int GetEnumOrder(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DisplayAttribute[])fi.GetCustomAttributes(
                typeof(DisplayAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].Order;
            }
            else
            {
                return -1;
            }
        }

    }
}
