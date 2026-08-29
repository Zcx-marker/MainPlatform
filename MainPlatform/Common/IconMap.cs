using System.Collections.Generic;

namespace MainPlatform.Common
{
    /// <summary>
    /// 将配置中的图标名映射为 Segoe MDL2 Assets 字形。
    /// </summary>
    public static class IconMap
    {
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ["home"] = "\uE80F",
            ["viewdashboard"] = "\uE80F",
            ["dashboard"] = "\uE80F",
            ["monitor"] = "\uE7F4",
            ["tvmonitor"] = "\uE7F4",
            ["cog"] = "\uE713",
            ["settings"] = "\uE713",
            ["tools"] = "\uE90F",
            ["maintenance"] = "\uE90F",
            ["warning"] = "\uE7BA",
            ["alarm"] = "\uE7BA",
            ["alert"] = "\uE7BA",
            ["contact"] = "\uE716",
            ["people"] = "\uE716",
            ["account"] = "\uE77B",
            ["pencil"] = "\uE70F",
            ["manual"] = "\uE70F",
            ["list"] = "\uE8FD",
            ["bulletedlist"] = "\uE8FD",
            ["process"] = "\uE8EF",
            ["system"] = "\uE713",
            ["overview"] = "\uE80F"
        };

        public static string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "\uE8A9";
            return Map.TryGetValue(key.Trim(), out var glyph) ? glyph : "\uE8A9";
        }
    }
}
