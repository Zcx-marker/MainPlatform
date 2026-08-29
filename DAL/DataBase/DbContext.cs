using DbModels;
using System.Linq;
using System.Linq.Expressions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using WpfApp1.Database;

namespace DAL.Database
{
    public abstract class DbContext<T> : IDbContext<T> where T : class, new()
    {
        /// <summary>
        /// 实例化一个数据库
        /// </summary>
        private SqlSugarClient _db;

        public SqlSugarClient Db => _db;

        private SqlSugarClient _writeDb;

        /// <summary>
        /// 实例化一个表
        /// </summary>
        private SimpleClient<T> _currentDb => new SimpleClient<T>(_db);

        public SimpleClient<T> CurrentDb => _currentDb;

        private static bool isInit;

        public DbContext()
        {
            #region 生成数据库和表
            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = DALConfigManager.Instance.DbConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
                
            });
            _db.Aop.OnLogExecuting = (sql, param) =>
            {
                //日志逻辑
            };

            _writeDb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = DALConfigManager.Instance.DbConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
            _writeDb.Aop.OnLogExecuting = (sql, param) =>
            {
                //日志逻辑
            };

            if (DALConfigManager.Instance.IsDbInitialized == false)
            {
                // 创建数据库
                _db.DbMaintenance.CreateDatabase();
                // 创建表
                Assembly assembly = Assembly.Load("DbModels");
                Type[] types = System.Linq.Enumerable.Where(assembly.GetTypes(),
        t => t.Namespace == "DbModels" && t.IsClass && !t.IsAbstract)
        .ToArray();
                // createdTable
                _db.CodeFirst.InitTables(types);

                DALConfigManager.Instance.IsDbInitialized = true;
            }

            #endregion 生成数据库和表

            //Type[] dbContextTypes = Assembly.Load("Service").GetTypes().Where(t => t.IsSubclassOf(typeof(DbContext<T>))&&(!t.FullName.Contains("User"))).ToArray();
        }

        public virtual bool DeleteNav<T1>(T t, Expression<Func<T, List<T1>>> expression) where T1 : class, new()
        {
            return _db.DeleteNav(t).Include(expression).ExecuteCommand();
        }

        public virtual bool DeleteNav<T1, T2>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new()
        {
            return _db.DeleteNav(t).Include(es1).Include(es2).ExecuteCommand();
        }

        public virtual bool DeleteNav<T1, T2>(Expression<Func<T, bool>> expression, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new()
        {
            return _db.DeleteNav(expression).Include(es1).Include(es2).ExecuteCommand();
        }

        public virtual bool DeleteNav<T1, T2, T3>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3) where T1 : class, new() where T2 : class, new() where T3 : class, new()
        {
            return _db.DeleteNav(t).Include(es1).Include(es2).Include(es3).ExecuteCommand();
        }

        public virtual bool DeleteNav<T1, T2, T3, T4>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            return _db.DeleteNav(t).Include(es1).Include(es2).Include(es3).Include(es4).ExecuteCommand();
        }

        public virtual bool Delete(T t)
        {
            return _currentDb.Delete(t);
        }

        public virtual bool Delete(Expression<Func<T, bool>> expression)
        {
            return _currentDb.Delete(expression);
        }

        public virtual bool DeleteRange(List<T> list)
        {
            return _currentDb.Delete(list);
        }

        public virtual bool VaccumTable()
        {
            try
            {
                _db.Ado.ExecuteCommand("VACUUM");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual List<T> QueryableNav<T1>(Expression<Func<T, List<T1>>> expression)
        {
            return _db.Queryable<T>().Includes(expression).ToList();
        }

        public virtual List<T> QueryableNav<T1, T2>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2)
        {
            return _db.Queryable<T>().Includes(es1).Includes(es2).ToList();
        }

        public virtual List<T> QueryableNav<T1, T2, T3>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3)
        {
            return _db.Queryable<T>().Includes(es1).Includes(es2).Includes(es3).ToList();
        }

        public virtual List<T> QueryableNav<T1, T2, T3, T4>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4)
        {
            return _db.Queryable<T>().Includes(es1).Includes(es2).Includes(es3).Includes(es4).ToList();
        }

        public virtual bool InsertOrUpdateNav<T1>(List<T> list, Expression<Func<T, List<T1>>> expression) where T1 : class, new()
        {
            if (this.InsertOrUpdate(list))
            {
                return _db.UpdateNav(list).Include(expression).ExecuteCommand();
            }

            return false;
        }

        public virtual bool InsertOrUpdateNav<T1, T2>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2)
            where T1 : class, new()
            where T2 : class, new()
        {
            if (this.InsertOrUpdate(list))
            {
                return _db.UpdateNav(list).Include(es1).Include(es2).ExecuteCommand();
            }

            return false;
        }

        public virtual bool InsertOrUpdateNav<T1, T2, T3>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
        {
            if (this.InsertOrUpdate(list))
            {
                return _db.UpdateNav(list).Include(es1).Include(es2).Include(es3).ExecuteCommand();
            }

            return false;
        }

        public virtual bool InsertOrUpdateNav<T1, T2, T3, T4>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            if (this.InsertOrUpdate(list))
            {
                return _db.UpdateNav(list).Include(es1).Include(es2).Include(es3).Include(es4).ExecuteCommand();
            }

            return false;
        }

        public virtual List<T> GetAll()
        {
            return _currentDb.GetList();
        }

        public virtual T GetById(int id)
        {
            return _currentDb.GetById(id);
        }

        public virtual List<T> GetList(Expression<Func<T, bool>> expression)
        {
            return _currentDb.GetList(expression);
        }

        public virtual bool Insert(T t)
        {
            try
            {
                return _currentDb.Insert(t);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public virtual List<long> InsertSplit(T t)
        {
            return _db.Insertable(t).SplitTable().ExecuteReturnSnowflakeIdList();
        }

        public virtual bool InsertRange(List<T> list)
        {
            return _currentDb.InsertRange(list);
        }

        public virtual List<long> InsertRangeSplit(List<T> list)
        {
            return _writeDb.Insertable(list).SplitTable().ExecuteReturnSnowflakeIdList();
        }

        public virtual int InsertReturnIdentity(T t)
        {
            return _currentDb.InsertReturnIdentity(t);
        }

        public virtual bool UpdateNav<T1>(List<T> list, Expression<Func<T, List<T1>>> expression) where T1 : class, new()
        {
            return _db.UpdateNav(list).Include(expression).ExecuteCommand();
        }

        public virtual bool UpdateNav<T1, T2>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new()
        {
            return _db.UpdateNav(list).Include(es1).Include(es2).ExecuteCommand();
        }

        public virtual bool UpdateNav<T1, T2, T3>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3) where T1 : class, new() where T2 : class, new() where T3 : class, new()
        {
            return _db.UpdateNav(list).Include(es1).Include(es2).Include(es3).ExecuteCommand();
        }

        public virtual bool UpdateNav<T1, T2, T3, T4>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            return _db.UpdateNav(list).Include(es1).Include(es2).Include(es3).Include(es4).ExecuteCommand();
        }

        public virtual bool Update(T t)
        {
            return _currentDb.Update(t);
        }

        public virtual bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expression)
        {
            return _currentDb.Update(columns, expression);
        }

        public virtual bool UpdateRange(List<T> list)
        {
            return _currentDb.UpdateRange(list);
        }

        public virtual bool InsertOrUpdate(T t)
        {
            return (_currentDb.InsertOrUpdate(t));
        }

        public virtual bool InsertOrUpdate(List<T> list)
        {
            return _currentDb.InsertOrUpdate(list);
        }

        public virtual List<T> Queryable<T>(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().Where(expression).ToList();
        }

        public virtual List<T> Queryable<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, object>> expression2, OrderByType type = OrderByType.Asc)
        {
            return _db.Queryable<T>().Where(expression1).OrderBy(expression2, type).ToList();
        }

        public virtual List<T> Queryable<T>(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            return _db.Queryable<T>().OrderBy(expression, type).ToList();
        }

        public virtual List<T> QueryableSplit<T>(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().Where(expression).SplitTable().ToList();
        }

        public virtual List<T> QueryableSplit<T>(DateTime startTime, DateTime endTime)
        {
            return _db.Queryable<T>().SplitTable(startTime, endTime).ToList();
        }

        public virtual List<T> QueryableSplit<T>(Expression<Func<T, bool>> expression, DateTime startTime, DateTime endTime)
        {
            return _db.Queryable<T>().Where(expression).SplitTable(startTime, endTime).ToList();
        }

        public virtual int DeleteSplit<T>(List<T> list)
        {
            return _db.Deleteable(list).SplitTable().ExecuteCommand();
        }

        public virtual bool DeleteAll()
        {
            return _db.DbMaintenance.TruncateTable<T>();
        }
    }
}