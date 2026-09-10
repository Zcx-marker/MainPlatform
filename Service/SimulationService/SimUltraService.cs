using Service.MotionControl;
using System.Threading.Tasks;

namespace Service.SimulationService
{
    /// <summary>
    /// 模拟超声功率校准服务：不驱动真实板卡，校准直接成功。
    /// 用于 IsSimulationMode=true 时替代 ActualUltraService，便于无卡调试界面流程。
    /// </summary>
    public class SimUltraService : IUltra
    {
        public bool IsUltraCalibrated { get; private set; }

        public int LastErrorCode => 0;

        /// <summary>模拟：校准直接成功</summary>
        public Task<bool> CalibrateUltraAsync()
        {
            IsUltraCalibrated = true;
            return Task.FromResult(true);
        }
    }
}
