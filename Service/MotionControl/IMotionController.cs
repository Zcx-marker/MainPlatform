using System;

namespace Service.MotionControl
{
    /// <summary>
    /// 脉冲输出模式（对应 DMC3000 手册 8.2 节表 8.1）
    /// 0 为出厂默认的 脉冲+方向(PULSE/DIR) 模式；
    /// 其余值(1~5)为不同极性组合，具体以手册表 8.1 为准。
    /// </summary>
    public enum PulseOutMode : ushort
    {
        /// <summary>脉冲+方向模式（默认）</summary>
        PulseDir = 0
    }

    /// <summary>
    /// 点位运动坐标模式
    /// </summary>
    public enum MoveMode : ushort
    {
        /// <summary>相对坐标模式</summary>
        Relative = 0,

        /// <summary>绝对坐标模式</summary>
        Absolute = 1
    }

    /// <summary>
    /// 连续运动方向
    /// </summary>
    public enum MoveDirection : ushort
    {
        /// <summary>负方向</summary>
        Negative = 0,

        /// <summary>正方向</summary>
        Positive = 1
    }

    /// <summary>
    /// 停止方式
    /// </summary>
    public enum StopMode : ushort
    {
        /// <summary>减速停止</summary>
        Decel = 0,

        /// <summary>立即停止（急停）</summary>
        Immediate = 1
    }

    /// <summary>
    /// 运动控制抽象接口（底层板卡）。
    /// 当前实现：雷赛 DMC3000 系列脉冲运动控制卡，见 <see cref="Dmc3000Controller"/>。
    /// 所有方法均返回是否成功，最后一次调用的错误码见 <see cref="LastErrorCode"/>。
    /// 位置/距离单位为 pulse，速度单位为 pulse/s。
    /// </summary>
    public interface IMotionController
    {
        #region 板卡管理

        /// <summary>
        /// 初始化控制卡，分配系统资源。调用前应确保驱动已安装、卡已插好。
        /// 成功条件：检测到至少 1 张卡（返回 1~8 为卡数量）。
        /// </summary>
        /// <returns>是否初始化成功</returns>
        bool BoardInit();

        /// <summary>
        /// 控制卡硬件复位。注意：复位后必须等待 5 秒方可再次初始化。
        /// </summary>
        /// <returns>是否成功</returns>
        bool BoardReset();

        /// <summary>
        /// 关闭控制卡，释放系统资源。程序退出前必须调用。
        /// </summary>
        void BoardClose();

        /// <summary>
        /// 初始化后检测到的控制卡数量（0 表示未找到卡）。
        /// </summary>
        int CardCount { get; }

        #endregion

        #region 轴参数设置

        /// <summary>
        /// 设置指定轴的脉冲输出模式。
        /// 注意：运动前必须根据驱动器接收脉冲的模式调用本方法。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="mode">脉冲输出模式</param>
        /// <returns>是否成功</returns>
        bool SetPulseOutMode(ushort axis, PulseOutMode mode);

        /// <summary>
        /// 设置单轴梯形速度曲线参数（位置单位 pulse，速度单位 pulse/s，时间单位 s）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="minVel">起始速度</param>
        /// <param name="maxVel">最大速度</param>
        /// <param name="tacc">加速时间</param>
        /// <param name="tdec">减速时间</param>
        /// <param name="stopVel">停止速度</param>
        /// <returns>是否成功</returns>
        bool SetProfile(ushort axis, double minVel, double maxVel, double tacc, double tdec, double stopVel);

        /// <summary>
        /// 设置 S 形速度曲线参数。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="sMode">S 段模式</param>
        /// <param name="sPara">S 段时间（0 表示无 S 段）</param>
        /// <returns>是否成功</returns>
        bool SetSProfile(ushort axis, ushort sMode, double sPara);

        /// <summary>
        /// 设置原点开关的有效电平（回零前必须先设置）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="homeLogic">原点信号有效电平</param>
        /// <param name="homeMode">保留参数</param>
        /// <returns>是否成功</returns>
        bool SetHomePinLogic(ushort axis, ushort homeLogic, ushort homeMode);

        /// <summary>
        /// 选择回原点模式。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="homeDir">回零方向</param>
        /// <param name="velMode">回零速度模式（0=低速）</param>
        /// <param name="mode">回零方式（0=一次回零，1=一次回零加回找，2=两次回零等）</param>
        /// <param name="ezCount">保留参数</param>
        /// <returns>是否成功</returns>
        bool SetHomeMode(ushort axis, ushort homeDir, ushort velMode, ushort mode, ushort ezCount);

        /// <summary>
        /// 设置回零偏移量及回零完成后是否清零。
        /// 注意：参数以 DMC3000 手册 8.3 节 / 官方 SDK 头文件为准。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="homePosition">回零偏移量，单位 pulse</param>
        /// <param name="clearMode">回零完成清零模式</param>
        /// <returns>是否成功</returns>
        bool SetHomePosition(ushort axis, int homePosition, ushort clearMode);

        /// <summary>
        /// 设置原点锁存模式（用于精确回零，配合 <see cref="ResetHomeLatchFlag"/> 使用）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="enable">锁存使能（1=使能）</param>
        /// <param name="logic">锁存触发方式（0=下降沿，1=上升沿）</param>
        /// <param name="source">锁存位置源（0=指令位置，1=编码器位置）</param>
        /// <returns>是否成功</returns>
        bool SetHomeLatchMode(ushort axis, ushort enable, ushort logic, ushort source);

        /// <summary>
        /// 清除原点锁存标志。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>是否成功</returns>
        bool ResetHomeLatchFlag(ushort axis);

        /// <summary>
        /// 设置指定轴指令脉冲计数器的绝对位置。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">绝对位置，单位 pulse</param>
        /// <returns>是否成功</returns>
        bool SetPosition(ushort axis, int position);

        #endregion

        #region 运动指令

        /// <summary>
        /// 指定轴点位运动（相对/绝对）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="distance">目标位置，单位 pulse；相对模式下正值为正向、负值为反向</param>
        /// <param name="mode">相对/绝对坐标模式</param>
        /// <returns>是否成功</returns>
        bool PMove(ushort axis, int distance, MoveMode mode);

        /// <summary>
        /// 指定轴连续运动（定速）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="dir">运动方向</param>
        /// <returns>是否成功</returns>
        bool VMove(ushort axis, MoveDirection dir);

        /// <summary>
        /// 按已设置的模式和速度曲线执行回原点运动（调用前需先 SetHomePinLogic / SetHomeMode / SetProfile）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>是否成功</returns>
        bool HomeMove(ushort axis);

        /// <summary>
        /// 停止指定轴运动。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="mode">停止方式</param>
        /// <returns>是否成功</returns>
        bool Stop(ushort axis, StopMode mode);

        /// <summary>
        /// 在线改变指定轴的当前运动速度。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="currVel">新的速度，单位 pulse/s</param>
        /// <param name="taccdec">保留参数</param>
        /// <returns>是否成功</returns>
        bool ChangeSpeed(ushort axis, double currVel, double taccdec);

        /// <summary>
        /// 在线改变指定轴的当前目标位置（仅适用于点位运动过程中）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="dist">目标位置（绝对坐标值），单位 pulse</param>
        /// <returns>是否成功</returns>
        bool ResetTargetPosition(ushort axis, int dist);

        #endregion

        #region 状态与位置查询

        /// <summary>
        /// 读取指定轴指令位置。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>当前位置，单位 pulse</returns>
        int GetPosition(ushort axis);

        /// <summary>
        /// 检测指定轴的运动状态。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>true=轴已停止，false=轴正在运行</returns>
        bool CheckDone(ushort axis);

        /// <summary>
        /// 检测指令是否到位（在 <see cref="CheckDone"/> 检测到停止后调用）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>true=指令位置在目标位置误差带内</returns>
        bool CheckSuccessPulse(ushort axis);

        /// <summary>
        /// 读取回零执行状态。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>true=回零完成，false=未完成</returns>
        bool IsHomeCompleted(ushort axis);

        /// <summary>
        /// 读取原点锁存标志（1=已触发锁存）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>锁存标志</returns>
        int GetHomeLatchFlag(ushort axis);

        /// <summary>
        /// 获取原点锁存值（触发瞬间的位置）。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>锁存位置值，单位 pulse</returns>
        int GetHomeLatchValue(ushort axis);

        #endregion

        #region 通用 IO

        /// <summary>
        /// 读取指定输入端口的电平（0~15 号输入口）。
        /// </summary>
        /// <param name="bitNo">输入端口号</param>
        /// <returns>0=低电平，1=高电平</returns>
        ushort ReadInbit(ushort bitNo);

        /// <summary>
        /// 设置指定输出端口的电平。
        /// </summary>
        /// <param name="bitNo">输出端口号</param>
        /// <param name="level">0=低电平，1=高电平</param>
        /// <returns>是否成功</returns>
        bool WriteOutbit(ushort bitNo, ushort level);

        /// <summary>
        /// 读取指定输出端口的电平。
        /// </summary>
        /// <param name="bitNo">输出端口号</param>
        /// <returns>0=低电平，1=高电平</returns>
        ushort ReadOutbit(ushort bitNo);

        /// <summary>
        /// 读取全部输入端口的电平状态（按位组合，建议按十六进制处理）。
        /// </summary>
        /// <param name="portNo">端口号</param>
        /// <returns>全部输入口电平状态</returns>
        uint ReadInport(ushort portNo);

        /// <summary>
        /// 读取全部输出端口的电平状态（按位组合，建议按十六进制处理）。
        /// </summary>
        /// <param name="portNo">端口号</param>
        /// <returns>全部输出口电平状态</returns>
        uint ReadOutport(ushort portNo);

        /// <summary>
        /// 设置全部输出端口的电平状态（按位组合，建议按十六进制赋值）。
        /// </summary>
        /// <param name="portNo">端口号</param>
        /// <param name="value">输出电平状态</param>
        /// <returns>是否成功</returns>
        bool WriteOutport(ushort portNo, uint value);

        #endregion

        #region 模拟量输出/输入（DA/AD）——超声、EFO 等板卡的功率给定

        /// <summary>
        /// 设置 DA 输出使能（向超声板卡等发送模拟功率给定前必须先使能）。
        /// </summary>
        /// <param name="enable">0=禁止，1=使能</param>
        /// <returns>是否成功</returns>
        bool SetDaEnable(ushort enable);

        /// <summary>
        /// 读取 DA 输出使能状态。
        /// </summary>
        /// <returns>0=禁止，1=使能</returns>
        ushort GetDaEnable();

        /// <summary>
        /// 设置 DA 输出电压（通道 0/1，范围 -10V~10V，12bit 精度）。
        /// 典型用法：超声板卡功率给定 0~10V 对应 0~100% 功率。
        /// </summary>
        /// <param name="channel">DA 通道（0 或 1）</param>
        /// <param name="volt">输出电压，单位 V</param>
        /// <returns>是否成功</returns>
        bool SetDaOutput(ushort channel, double volt);

        /// <summary>
        /// 读取 DA 当前输出电压。
        /// </summary>
        /// <param name="channel">DA 通道（0 或 1）</param>
        /// <returns>输出电压，单位 V</returns>
        double GetDaOutput(ushort channel);

        /// <summary>
        /// 读取 AD 输入采样值（可用于监视超声板卡功率反馈等模拟信号）。
        /// </summary>
        /// <param name="channel">AD 通道</param>
        /// <returns>AD 采样值</returns>
        ushort GetDaInput(ushort channel);

        #endregion

        /// <summary>
        /// 最后一次调用返回的错误码（0=成功，其余为错误代码，详见手册附录）。
        /// </summary>
        int LastErrorCode { get; }
    }
}
