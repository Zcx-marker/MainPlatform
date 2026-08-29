using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainPlatform.Tools
{
    public class JsonTool
    {
        // 实现单例模式
        private static readonly Lazy<JsonTool> _instance = new Lazy<JsonTool>(() => new JsonTool());
        public static JsonTool Instance => _instance.Value;

        public JsonTool() { }
        public void SaveToJsonFile<T>(T obj, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,  // 生成缩进格式的 JSON
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 使用 camelCase 命名
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                // 序列化为 JSON
                string json = System.Text.Json.JsonSerializer.Serialize(obj, options);

                // 写入文件（自动创建或覆盖）
                 File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // 处理异常（记录日志或抛出）
                Console.WriteLine($"保存 JSON 文件失败: {ex.Message}");
                throw;
            }
        }

        public T LoadFromJsonFile<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("JSON 文件未找到", filePath);
                // 读取文件内容
                string jsontxt = File.ReadAllText(filePath);
                // 反序列化为对象
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 使用 camelCase 命名
                    PropertyNameCaseInsensitive = true,  // 大小写不敏感，兼容 PascalCase/camelCase 混用
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                T obj = System.Text.Json.JsonSerializer.Deserialize<T>(jsontxt, options);
                return obj;
            }
            catch (Exception ex)
            {
                // 处理异常（记录日志或抛出）
                Console.WriteLine($"加载 JSON 文件失败: {ex.Message}");
                throw;
            }
        }
    }
}
