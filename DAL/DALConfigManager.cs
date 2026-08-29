using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALConfigManager
    {
        // 懒汉单例模式
        private static readonly Lazy<DALConfigManager> _lazyInstance =
        new Lazy<DALConfigManager>(() => new DALConfigManager());
        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static DALConfigManager Instance => _lazyInstance.Value;

        /// <summary>
        /// 判断数据库有没有被初始化
        /// </summary>
        public bool IsDbInitialized { get; set; }

        /// <summary>
        /// Sqlite数据库链接字符串
        /// </summary>
        public string DbConnectionString;
        public DALConfigManager()
        {
            DbConnectionString = GetSQLCONNECTIONSTRING();
        }

        /// <summary>
        /// 获取数据库字符串
        /// </summary>
        /// <returns></returns>
        public string GetSQLCONNECTIONSTRING()
        {
            string runFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //在运行目录下建Data文件夹放sqlite
            string dataPath = Path.Combine(runFolder, "Data");
            //Directory.CreateDirectory(dataPath);

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            string dbFullPath = Path.Combine(dataPath, "scmc.db");
            string conn = $"Data Source={dbFullPath}";
            return conn;
        }
    }
}
