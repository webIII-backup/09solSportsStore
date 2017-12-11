using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsStore.Helpers
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumType)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                    .Select(e => new { Id = e, Name = e.GetDisplayName() });
            return new SelectList(values, "Id", "Name");
        }

        public static string GetDisplayName<TEnum>(this TEnum enumValue)
        {
            return typeof(TEnum).GetMember(enumValue.ToString())[0]
                           .GetCustomAttribute<DisplayAttribute>()?
                           .Name ?? enumValue.ToString();
        }
     }
}


