using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace DbModels
{
    [SqlSugar.SugarTable("UserData")]
    public class UserData
    {

        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsNullable = false)]
        public string UserName { get; set; }

        [SqlSugar.SugarColumn(IsNullable = false)]
        public string Password { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true)]
        public string PhoneNumber { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Identity { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? UpdateTime { get; set; } = null;

        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? LastLoginTime { get; set; } = null;

        [SqlSugar.SugarColumn(IsNullable = true)]
        public bool IsRemembered { get; set; } = false;

        public UserData() { }
    }
}
