using System;
using System.Runtime.InteropServices;

namespace Service.MotionControl
{
    /// <summary>
    /// 雷赛 DMC3000 系列运动控制卡实现（P/Invoke 方式调用雷赛驱动动态库）。
    /// 支持型号及轴数：DMC3400A(4轴)、DMC3600(6轴)、DMC3800(8轴)、DMC3C00(12轴)。
    /// 函数签名与说明对应《DMC3000 系列运动控制卡用户使用手册》第 8 章函数库。
    /// </summary>
    /// <remarks>
    /// 使用前需安装雷赛 DMC3000 驱动并将动态库置于运行目录（或系统 PATH）。
    /// 未安装驱动/未插卡时调用会抛出 DllNotFoundException / 返回失败码，请做好异常与错误码处理。
    /// </remarks>
    public class Dmc3000Controller : IMotionController
    {
        /// <summary>
        /// 雷赛动态库文件名。
        /// 若你安装的 SDK 使用其他库名（如 LTDIC.dll / DMC3000.dll），修改此处即可，无需改其他代码。
        /// </summary>
        private const string DllName = "LTDMC.dll";

        private readonly ushort _cardNo;

        /// <summary>
        /// 创建一个 DMC3000 控制器实例。
        /// </summary>
        /// <param name="cardNo">控制卡卡号（拨码开关设置，默认 0）</param>
        public Dmc3000Controller(ushort cardNo = 0)
        {
            _cardNo = cardNo;
        }

        /// <inheritdoc />
        public int LastErrorCode { get; private set; }

        /// <inheritdoc />
        public int CardCount { get; private set; }

        #region P/Invoke 声明（DMC3000 手册第 8 章）

        [DllImport(DllName, EntryPoint = "dmc_board_init")]
        private static extern short dmc_board_init();

        [DllImport(DllName, EntryPoint = "dmc_board_reset")]
        private static extern short dmc_board_reset();

        [DllImport(DllName, EntryPoint = "dmc_board_close")]
        private static extern short dmc_board_close();

        [DllImport(DllName, EntryPoint = "dmc_set_pulse_outmode")]
        private static extern short dmc_set_pulse_outmode(ushort cardNo, ushort axis, ushort outmode);

        [DllImport(DllName, EntryPoint = "dmc_set_profile")]
        private static extern short dmc_set_profile(ushort cardNo, ushort axis, double minVel, double maxVel, double tacc, double tdec, double stopVel);

        [DllImport(DllName, EntryPoint = "dmc_set_s_profile")]
        private static extern short dmc_set_s_profile(ushort cardNo, ushort axis, ushort sMode, double sPara);

        [DllImport(DllName, EntryPoint = "dmc_set_home_pin_logic")]
        private static extern short dmc_set_home_pin_logic(ushort cardNo, ushort axis, ushort homeLogic, ushort homeMode);

        [DllImport(DllName, EntryPoint = "dmc_set_homemode")]
        private static extern short dmc_set_homemode(ushort cardNo, ushort axis, ushort homeDir, ushort velMode, ushort mode, ushort ezCount);

        [DllImport(DllName, EntryPoint = "dmc_set_home_position")]
        private static extern short dmc_set_home_position(ushort cardNo, ushort axis, int homePosition, ushort clearMode);

        [DllImport(DllName, EntryPoint = "dmc_home_move")]
        private static extern short dmc_home_move(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_get_home_result")]
        private static extern short dmc_get_home_result(ushort cardNo, ushort axis, out ushort state);

        [DllImport(DllName, EntryPoint = "dmc_set_homelatch_mode")]
        private static extern short dmc_set_homelatch_mode(ushort cardNo, ushort axis, ushort enable, ushort logic, ushort source);

        [DllImport(DllName, EntryPoint = "dmc_reset_homelatch_flag")]
        private static extern short dmc_reset_homelatch_flag(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_get_homelatch_flag")]
        private static extern int dmc_get_homelatch_flag(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_get_homelatch_value")]
        private static extern int dmc_get_homelatch_value(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_pmove")]
        private static extern short dmc_pmove(ushort cardNo, ushort axis, int dist, ushort posiMode);

        [DllImport(DllName, EntryPoint = "dmc_vmove")]
        private static extern short dmc_vmove(ushort cardNo, ushort axis, ushort dir);

        [DllImport(DllName, EntryPoint = "dmc_change_speed")]
        private static extern short dmc_change_speed(ushort cardNo, ushort axis, double currVel, double taccdec);

        [DllImport(DllName, EntryPoint = "dmc_reset_target_position")]
        private static extern short dmc_reset_target_position(ushort cardNo, ushort axis, int dist, ushort posiMode);

        [DllImport(DllName, EntryPoint = "dmc_stop")]
        private static extern short dmc_stop(ushort cardNo, ushort axis, ushort stopMode);

        [DllImport(DllName, EntryPoint = "dmc_check_done")]
        private static extern short dmc_check_done(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_check_success_pulse")]
        private static extern short dmc_check_success_pulse(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_set_position")]
        private static extern short dmc_set_position(ushort cardNo, ushort axis, int pos);

        [DllImport(DllName, EntryPoint = "dmc_get_position")]
        private static extern int dmc_get_position(ushort cardNo, ushort axis);

        [DllImport(DllName, EntryPoint = "dmc_read_inbit")]
        private static extern short dmc_read_inbit(ushort cardNo, ushort bitNo);

        [DllImport(DllName, EntryPoint = "dmc_write_outbit")]
        private static extern short dmc_write_outbit(ushort cardNo, ushort bitNo, ushort level);

        [DllImport(DllName, EntryPoint = "dmc_read_outbit")]
        private static extern short dmc_read_outbit(ushort cardNo, ushort bitNo);

        [DllImport(DllName, EntryPoint = "dmc_read_inport")]
        private static extern uint dmc_read_inport(ushort cardNo, ushort portNo);

        [DllImport(DllName, EntryPoint = "dmc_read_outport")]
        private static extern uint dmc_read_outport(ushort cardNo, ushort portNo);

        [DllImport(DllName, EntryPoint = "dmc_write_outport")]
        private static extern short dmc_write_outport(ushort cardNo, ushort portNo, uint value);

        [DllImport(DllName, EntryPoint = "dmc_set_da_enable")]
        private static extern short dmc_set_da_enable(ushort cardNo, ushort enable);

        [DllImport(DllName, EntryPoint = "dmc_get_da_enable")]
        private static extern short dmc_get_da_enable(ushort cardNo, out ushort enable);

        [DllImport(DllName, EntryPoint = "dmc_set_da_output")]
        private static extern short dmc_set_da_output(ushort cardNo, ushort channel, double vout);

        [DllImport(DllName, EntryPoint = "dmc_get_da_output")]
        private static extern short dmc_get_da_output(ushort cardNo, ushort channel, out double vout);

        [DllImport(DllName, EntryPoint = "dmc_get_da_input")]
        private static extern short dmc_get_da_input(ushort cardNo, ushort channel, out ushort value);

        #endregion

        #region 板卡管理

        /// <inheritdoc />
        public bool BoardInit()
        {
            // dmc_board_init 返回值：0=未找到卡，1~8=检测到的卡数量，负值=卡号冲突
            short ret = dmc_board_init();
            LastErrorCode = ret;
            if (ret > 0)
            {
                CardCount = ret;
                return true;
            }
            CardCount = 0;
            return false;
        }

        /// <inheritdoc />
        public bool BoardReset()
        {
            short ret = dmc_board_reset();
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public void BoardClose()
        {
            LastErrorCode = dmc_board_close();
        }

        #endregion

        #region 轴参数设置

        /// <inheritdoc />
        public bool SetPulseOutMode(ushort axis, PulseOutMode mode)
        {
            short ret = dmc_set_pulse_outmode(_cardNo, axis, (ushort)mode);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetProfile(ushort axis, double minVel, double maxVel, double tacc, double tdec, double stopVel)
        {
            short ret = dmc_set_profile(_cardNo, axis, minVel, maxVel, tacc, tdec, stopVel);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetSProfile(ushort axis, ushort sMode, double sPara)
        {
            short ret = dmc_set_s_profile(_cardNo, axis, sMode, sPara);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetHomePinLogic(ushort axis, ushort homeLogic, ushort homeMode)
        {
            short ret = dmc_set_home_pin_logic(_cardNo, axis, homeLogic, homeMode);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetHomeMode(ushort axis, ushort homeDir, ushort velMode, ushort mode, ushort ezCount)
        {
            short ret = dmc_set_homemode(_cardNo, axis, homeDir, velMode, mode, ezCount);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetHomePosition(ushort axis, int homePosition, ushort clearMode)
        {
            short ret = dmc_set_home_position(_cardNo, axis, homePosition, clearMode);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetHomeLatchMode(ushort axis, ushort enable, ushort logic, ushort source)
        {
            short ret = dmc_set_homelatch_mode(_cardNo, axis, enable, logic, source);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool ResetHomeLatchFlag(ushort axis)
        {
            short ret = dmc_reset_homelatch_flag(_cardNo, axis);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool SetPosition(ushort axis, int position)
        {
            short ret = dmc_set_position(_cardNo, axis, position);
            LastErrorCode = ret;
            return ret == 0;
        }

        #endregion

        #region 运动指令

        /// <inheritdoc />
        public bool PMove(ushort axis, int distance, MoveMode mode)
        {
            short ret = dmc_pmove(_cardNo, axis, distance, (ushort)mode);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool VMove(ushort axis, MoveDirection dir)
        {
            short ret = dmc_vmove(_cardNo, axis, (ushort)dir);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool HomeMove(ushort axis)
        {
            short ret = dmc_home_move(_cardNo, axis);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool Stop(ushort axis, StopMode mode)
        {
            short ret = dmc_stop(_cardNo, axis, (ushort)mode);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool ChangeSpeed(ushort axis, double currVel, double taccdec)
        {
            short ret = dmc_change_speed(_cardNo, axis, currVel, taccdec);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public bool ResetTargetPosition(ushort axis, int dist)
        {
            short ret = dmc_reset_target_position(_cardNo, axis, dist, 0);
            LastErrorCode = ret;
            return ret == 0;
        }

        #endregion

        #region 状态与位置查询

        /// <inheritdoc />
        public int GetPosition(ushort axis)
        {
            return dmc_get_position(_cardNo, axis);
        }

        /// <inheritdoc />
        public bool CheckDone(ushort axis)
        {
            short ret = dmc_check_done(_cardNo, axis);
            LastErrorCode = ret;
            return ret == 1;
        }

        /// <inheritdoc />
        public bool CheckSuccessPulse(ushort axis)
        {
            short ret = dmc_check_success_pulse(_cardNo, axis);
            LastErrorCode = ret;
            return ret == 1;
        }

        /// <inheritdoc />
        public bool IsHomeCompleted(ushort axis)
        {
            ushort state;
            short ret = dmc_get_home_result(_cardNo, axis, out state);
            LastErrorCode = ret;
            return ret == 0 && state == 1;
        }

        /// <inheritdoc />
        public int GetHomeLatchFlag(ushort axis)
        {
            return dmc_get_homelatch_flag(_cardNo, axis);
        }

        /// <inheritdoc />
        public int GetHomeLatchValue(ushort axis)
        {
            return dmc_get_homelatch_value(_cardNo, axis);
        }

        #endregion

        #region 通用 IO

        /// <inheritdoc />
        public ushort ReadInbit(ushort bitNo)
        {
            return (ushort)dmc_read_inbit(_cardNo, bitNo);
        }

        /// <inheritdoc />
        public bool WriteOutbit(ushort bitNo, ushort level)
        {
            short ret = dmc_write_outbit(_cardNo, bitNo, level);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public ushort ReadOutbit(ushort bitNo)
        {
            return (ushort)dmc_read_outbit(_cardNo, bitNo);
        }

        /// <inheritdoc />
        public uint ReadInport(ushort portNo)
        {
            return dmc_read_inport(_cardNo, portNo);
        }

        /// <inheritdoc />
        public uint ReadOutport(ushort portNo)
        {
            return dmc_read_outport(_cardNo, portNo);
        }

        /// <inheritdoc />
        public bool WriteOutport(ushort portNo, uint value)
        {
            short ret = dmc_write_outport(_cardNo, portNo, value);
            LastErrorCode = ret;
            return ret == 0;
        }

        #endregion

        #region 模拟量输出/输入（DA/AD）

        /// <inheritdoc />
        public bool SetDaEnable(ushort enable)
        {
            short ret = dmc_set_da_enable(_cardNo, enable);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public ushort GetDaEnable()
        {
            ushort enable;
            short ret = dmc_get_da_enable(_cardNo, out enable);
            LastErrorCode = ret;
            return ret == 0 ? enable : (ushort)0;
        }

        /// <inheritdoc />
        public bool SetDaOutput(ushort channel, double volt)
        {
            short ret = dmc_set_da_output(_cardNo, channel, volt);
            LastErrorCode = ret;
            return ret == 0;
        }

        /// <inheritdoc />
        public double GetDaOutput(ushort channel)
        {
            double vout;
            short ret = dmc_get_da_output(_cardNo, channel, out vout);
            LastErrorCode = ret;
            return ret == 0 ? vout : 0d;
        }

        /// <inheritdoc />
        public ushort GetDaInput(ushort channel)
        {
            ushort value;
            short ret = dmc_get_da_input(_cardNo, channel, out value);
            LastErrorCode = ret;
            return ret == 0 ? value : (ushort)0;
        }

        #endregion
    }
}
