using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WpfApp1.Database
{
    public interface IDbContext<T> where T : class, new()
    {
        #region insert  新增

        /// <summary>
        /// 插入单个值
        /// </summary>
        /// <param name="t">要插入的对象</param>
        /// <returns>是否插入成功</returns>
        bool Insert(T t);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="list">插入的内容</param>
        /// <returns>是否插入成功</returns>
        bool InsertRange(List<T> list);

        /// <summary>
        /// 单条数据插入并返回ID
        /// </summary>
        /// <param name="t">插入的数据</param>
        /// <returns>返回ID</returns>
        int InsertReturnIdentity(T t);

        /// <summary>
        /// 单条数据的更新或者插入
        /// </summary>
        /// <param name="t">需要更新或者插入的数据</param>
        /// <returns>是否成功</returns>
        bool InsertOrUpdate(T t);

        /// <summary>
        /// 多条数据的更新或者插入
        /// </summary>
        /// <param name="t">需要更新或者插入的数据</param>
        /// <returns>是否成功</returns>
        bool InsertOrUpdate(List<T> list);

        /// <summary>
        /// 多条数据的插入或者导航更新
        /// </summary>
        /// <typeparam name="T1">导航更新的从表面</typeparam>
        /// <param name="list">需要更新或者插入的主表数据</param>
        /// <param name="expression">表达式</param>
        /// <returns>是否成功</returns>
        bool InsertOrUpdateNav<T1>(List<T> list, Expression<Func<T, List<T1>>> expression) where T1 : class, new();

        /// <summary>
        /// 导航更新，支持导航两个从表
        /// </summary>
        /// <returns></returns>
        bool InsertOrUpdateNav<T1, T2>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new();

        /// <summary>
        /// 导航更新，支持导航三个从表
        /// </summary>
        /// <returns></returns>
        bool InsertOrUpdateNav<T1, T2, T3>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3) where T1 : class, new() where T2 : class, new() where T3 : class, new();

        /// <summary>
        /// 导航更新，支持导航四个从表
        /// </summary>
        /// <returns></returns>
        bool InsertOrUpdateNav<T1, T2, T3, T4>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new();

        #endregion insert  新增

        #region delete  删除

        /// <summary>
        /// 导航删除，支持删除一个从表
        /// </summary>
        /// <typeparam name="T1">导航删除从表名</typeparam>
        /// <param name="t">需要删除的主表数据</param>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool DeleteNav<T1>(T t, Expression<Func<T, List<T1>>> expression) where T1 : class, new();

        /// <summary>
        /// 导航删除，支持导航两个从表
        /// </summary>
        /// <returns></returns>
        bool DeleteNav<T1, T2>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new();

        /// <summary>
        /// 根据表达式&导航删除，支持导航两个从表
        /// </summary>
        /// <returns></returns>
        bool DeleteNav<T1, T2>(Expression<Func<T, bool>> expression, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new();

        /// <summary>
        /// 导航删除，支持导航三个从表
        /// </summary>
        /// <returns></returns>
        bool DeleteNav<T1, T2, T3>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3) where T1 : class, new() where T2 : class, new() where T3 : class, new();

        /// <summary>
        /// 导航删除，支持导航四个从表
        /// </summary>
        /// <returns></returns>
        bool DeleteNav<T1, T2, T3, T4>(T t, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new();

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="t">删除多条数据</param>
        /// <returns>是否删除成功</returns>
        bool Delete(T t);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="list"> 需要批量删除的数据</param>
        /// <returns>删除是否成功</returns>
        bool DeleteRange(List<T> list);

        /// <summary>
        /// 通过条件删除数据
        /// </summary>
        /// <param name="expression">删除的条件</param>
        /// <returns>删除是否成功</returns>
        bool Delete(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 数据全部清空，清除，自增初始化
        /// </summary>
        /// <returns>是否成功</returns>
        bool DeleteAll();

        /// <summary>
        /// 清理并重新组织SqLite数据库文件
        /// </summary>
        /// <returns>是否成功</returns>
        bool VaccumTable();

        #endregion delete  删除

        #region update 修改

        /// <summary>
        /// 导航更新，支持导航一个从表
        /// </summary>
        /// <typeparam name="T1">导航更新从表名</typeparam>
        /// <param name="list">需要更新的主表数据</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        bool UpdateNav<T1>(List<T> list, Expression<Func<T, List<T1>>> expression) where T1 : class, new();

        /// <summary>
        /// 导航更新，支持导航两个从表
        /// </summary>
        /// <returns></returns>
        bool UpdateNav<T1, T2>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2) where T1 : class, new() where T2 : class, new();

        /// <summary>
        /// 导航更新，支持导航三个从表
        /// </summary>
        /// <returns></returns>
        bool UpdateNav<T1, T2, T3>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3) where T1 : class, new() where T2 : class, new() where T3 : class, new();

        /// <summary>
        /// 导航更新，支持导航四个从表
        /// </summary>
        /// <returns></returns>
        bool UpdateNav<T1, T2, T3, T4>(List<T> list, Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new();

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="t">需要更新的数据</param>
        /// <returns>更新是否成功</returns>
        bool Update(T t);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="list">需要更新的数据</param>
        /// <returns>更新是否成功</returns>
        bool UpdateRange(List<T> list);

        /// <summary>
        /// 按照条件更新数据
        /// </summary>
        /// <param name="columns">需要更新的列</param>
        /// <param name="expression">更新的条件</param>
        /// <returns>是否更新成功</returns>
        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expression);

        #endregion update 修改

        #region select 查询

        /// <summary>
        /// 导航查询，支持导航一个从表
        /// </summary>
        /// <typeparam name="T1">导航从表名</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<T> QueryableNav<T1>(Expression<Func<T, List<T1>>> expression);

        /// <summary>
        /// 导航查询，支持同时导航两个从表
        /// </summary>
        /// <returns></returns>
        List<T> QueryableNav<T1, T2>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2);

        /// <summary>
        /// 导航查询，支持同时导航三个从表
        /// </summary>
        /// <returns></returns>
        List<T> QueryableNav<T1, T2, T3>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3);

        /// <summary>
        /// 导航查询，支持同时导航四个从表
        /// </summary>
        /// <returns></returns>
        List<T> QueryableNav<T1, T2, T3, T4>(Expression<Func<T, List<T1>>> es1, Expression<Func<T, List<T2>>> es2, Expression<Func<T, List<T3>>> es3, Expression<Func<T, List<T4>>> es4);

        /// <summary>
        /// 通过Id查询
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>返回ID对应的信息</returns>
        T GetById(int id);

        /// <summary>
        /// 通过表达式查询
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>返回满足表达式的信息</returns>
        List<T> GetList(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 获取表内所有信息
        /// </summary>
        /// <returns>返回此表的所有信息</returns>
        List<T> GetAll();

        List<T> Queryable<T>(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);

        #endregion select 查询
    }
}