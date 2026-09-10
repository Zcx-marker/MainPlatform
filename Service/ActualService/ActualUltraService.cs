using Caliburn.Micro;
using Service.MotionControl;
using System;
using System.Threading.Tasks;

namespace Service.ActualService
{
    /// <summary>
    /// 真实超声功率校准服务：通过板卡 DA 输出功率给定（超声发生器）、AD 读取功率反馈，
    /// 多点自动标定（最小二乘拟合），补偿发生器老化/温度漂移导致的设定值与实际输出偏差。
    /// 注：DA/AD 通道号需按实际接线图确认。
    /// </summary>
    public class ActualUltraService : IUltra
    {
        // —— 待按实际接线调整的常量 ——
        private const ushort UltraDaChannel = 0;        // 超声功率给定 DA 通道
        private const ushort UltraAdChannel = 0;        // 超声功率反馈 AD 通道
        private const double MaxPowerPercent = 100.0;   // DA 满量程(10V)对应功率百分比
        private const double StableDelayMs = 500;       // 输出后稳定延时，单位 ms

        /// <summary>超声校准标定点（%）</summary>
        private static readonly double[] CalibPoints = { 20, 40, 60, 80, 100 };

        private readonly IMotionController _controller;

        // 功率换算系数：实际功率% = K * 设定功率% + B（由校准拟合得出）
        private double _k = 1.0;
        private double _b = 0.0;

        public ActualUltraService() : this(IoC.Get<IMotionController>())
        {
        }

        public ActualUltraService(IMotionController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public bool IsUltraCalibrated { get; private set; }

        public int LastErrorCode => _controller.LastErrorCode;

        /// <summary>
        /// 超声校准：逐点输出功率给定电压 → 稳定 → 读 AD 反馈换算实际功率 → 最小二乘拟合 K/B。
        /// </summary>
        public async Task<bool> CalibrateUltraAsync()
        {
            try
            {
                if (!_controller.SetDaEnable(1))
                {
                    return false;
                }

                int n = CalibPoints.Length;
                double[] actual = new double[n];
                for (int i = 0; i < n; i++)
                {
                    double volt = CalibPoints[i] / MaxPowerPercent * 10.0;
                    if (!_controller.SetDaOutput(UltraDaChannel, volt))
                    {
                        return false;
                    }

                    await Task.Delay((int)StableDelayMs);

                    actual[i] = _controller.GetDaInput(UltraAdChannel) / 10.0 * MaxPowerPercent;
                }

                FitLinear(CalibPoints, actual, out _k, out _b);
                IsUltraCalibrated = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 最小二乘线性拟合：y = k * x + b。
        /// </summary>
        private static void FitLinear(double[] x, double[] y, out double k, out double b)
        {
            double sx = 0, sy = 0, sxx = 0, sxy = 0;
            int n = x.Length;
            for (int i = 0; i < n; i++)
            {
                sx += x[i];
                sy += y[i];
                sxx += x[i] * x[i];
                sxy += x[i] * y[i];
            }

            double denom = n * sxx - sx * sx;
            if (Math.Abs(denom) < 1e-9)
            {
                k = 1.0;
                b = 0.0;
                return;
            }

            k = (n * sxy - sx * sy) / denom;
            b = (sy - k * sx) / n;
        }
    }
}
