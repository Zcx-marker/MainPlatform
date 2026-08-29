using DAL.Database;
using DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DbService
{
    /// <summary>
    /// 用户数据数据库服务,用于查询用户数据
    /// </summary>
    public class UserDataService:DbContext<UserData>
    {
        public UserDataService() { }
    }
}
