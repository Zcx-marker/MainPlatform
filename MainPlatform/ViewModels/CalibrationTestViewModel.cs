using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Service;
using Service.MotionControl;

namespace MainPlatform.ViewModels
{
    /// <summary>
    /// 校准操作页面 VM：水平/垂直紧锁（ILock）+ 力校准/力测试（IForce）+ 超声校准（IUltra）。
    /// 服务由 Bootstrapper 统一注册（模拟/真实分流），VM 只做调用与状态展示。
    /// 注意：主窗口按类型名反射 + 无参构造创建子页，必须保留公共无参构造。
    /// </summary>
    public class CalibrationTestViewModel : Screen
    {
        private readonly ILock _lockService;
        private readonly IForce _forceService;
        private readonly IUltra _ultraService;
        private readonly IBaseDispService _dispService;

        private bool _isHorizontalLocked;
        private bool _isVerticalLocked;

        private string _horizontalLockText = "水平紧锁";
        private string _verticalLockText = "垂直紧锁";
        private Brush _horizontalLockBrush = LockedOffBrush;
        private Brush _verticalLockBrush = LockedOffBrush;

        private string _forceCalibrationText = "力校准: 未校准";
        private Brush _forceCalibrationBrush = ForcePendingBrush;

        private string _ultraCalibrationText = "超声校准: 未校准";
        private Brush _ultraCalibrationBrush = ForcePendingBrush;

        private string _initText = "初始化";
        private Brush _initBrush = ForcePendingBrush;

        /// <summary>力测试默认目标力（g）</summary>
        private const double DefaultTestForceG = 80.0;

        private static readonly Brush LockedOffBrush = new SolidColorBrush(Color.FromRgb(0xF0, 0xA5, 0x00));
        private static readonly Brush LockedOnBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0xA8, 0x4F));
        private static readonly Brush ForcePendingBrush = new SolidColorBrush(Color.FromRgb(0xF0, 0xA5, 0x00));
        private static readonly Brush ForceDoneBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0xA8, 0x4F));
        private static readonly Brush ForceFailBrush = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));

        public CalibrationTestViewModel()
            : this(IoC.Get<ILock>(), IoC.Get<IForce>(), IoC.Get<IUltra>(), IoC.Get<IBaseDispService>())
        {
        }

        public CalibrationTestViewModel(ILock lockService, IForce forceService, IUltra ultraService, IBaseDispService dispService)
        {
            _lockService = lockService;
            _forceService = forceService;
            _ultraService = ultraService;
            _dispService = dispService;
            UpdateInitState();
        }

        #region 水平/垂直紧锁

        public bool IsHorizontalLocked
        {
            get { return _isHorizontalLocked; }
            set
            {
                if (_isHorizontalLocked == value) return;
                _isHorizontalLocked = value;
                NotifyOfPropertyChange(() => IsHorizontalLocked);
                UpdateHorizontalLockState();
            }
        }

        public bool IsVerticalLocked
        {
            get { return _isVerticalLocked; }
            set
            {
                if (_isVerticalLocked == value) return;
                _isVerticalLocked = value;
                NotifyOfPropertyChange(() => IsVerticalLocked);
                UpdateVerticalLockState();
            }
        }

        public string HorizontalLockText
        {
            get { return _horizontalLockText; }
            set { _horizontalLockText = value; NotifyOfPropertyChange(() => HorizontalLockText); }
        }

        public string VerticalLockText
        {
            get { return _verticalLockText; }
            set { _verticalLockText = value; NotifyOfPropertyChange(() => VerticalLockText); }
        }

        public Brush HorizontalLockBrush
        {
            get { return _horizontalLockBrush; }
            set { _horizontalLockBrush = value; NotifyOfPropertyChange(() => HorizontalLockBrush); }
        }

        public Brush VerticalLockBrush
        {
            get { return _verticalLockBrush; }
            set { _verticalLockBrush = value; NotifyOfPropertyChange(() => VerticalLockBrush); }
        }

        public async void LockHorizontal()
        {
            try
            {
                bool result = IsHorizontalLocked
                    ? await _lockService.UnlockHorizontalAsync()
                    : await _lockService.LockHorizontalAsync();

                if (result)
                {
                    IsHorizontalLocked = !IsHorizontalLocked;
                }
                else
                {
                    MessageBox.Show("水平紧锁操作失败，请检查设备状态。", "提示",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("水平紧锁操作异常：" + ex.Message, "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async void LockVertical()
        {
            try
            {
                bool result = IsVerticalLocked
                    ? await _lockService.UnlockVerticalAsync()
                    : await _lockService.LockVerticalAsync();

                if (result)
                {
                    IsVerticalLocked = !IsVerticalLocked;
                }
                else
                {
                    MessageBox.Show("垂直紧锁操作失败，请检查设备状态。", "提示",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("垂直紧锁操作异常：" + ex.Message, "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateHorizontalLockState()
        {
            HorizontalLockText = IsHorizontalLocked ? "水平解锁" : "水平紧锁";
            HorizontalLockBrush = IsHorizontalLocked ? LockedOnBrush : LockedOffBrush;
        }

        private void UpdateVerticalLockState()
        {
            VerticalLockText = IsVerticalLocked ? "垂直解锁" : "垂直紧锁";
            VerticalLockBrush = IsVerticalLocked ? LockedOnBrush : LockedOffBrush;
        }

        #endregion

        #region 力校准 / 力测试

        public string ForceCalibrationText
        {
            get { return _forceCalibrationText; }
            set { _forceCalibrationText = value; NotifyOfPropertyChange(() => ForceCalibrationText); }
        }

        public Brush ForceCalibrationBrush
        {
            get { return _forceCalibrationBrush; }
            set { _forceCalibrationBrush = value; NotifyOfPropertyChange(() => ForceCalibrationBrush); }
        }

        /// <summary>
        /// 力校准：逐点输出-反馈-拟合（服务内部完成），完成后更新状态栏。
        /// </summary>
        public async void ForceCalibrate()
        {
            ForceCalibrationText = "力校准: 校准中...";
            ForceCalibrationBrush = ForcePendingBrush;

            bool ok = await _forceService.CalibrateForceAsync();
            if (ok)
            {
                ForceCalibrationText = "力校准: 已完成";
                ForceCalibrationBrush = ForceDoneBrush;
                MessageBox.Show("力校准完成。", "力校准",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ForceCalibrationText = "力校准: 失败";
                ForceCalibrationBrush = ForceFailBrush;
                MessageBox.Show("力校准失败，请检查板卡与力传感器接线。", "力校准",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 力测试：输出默认目标力并读反馈，弹窗显示实测值与判定结果。
        /// </summary>
        public async void ForceTest()
        {
            double measured = await _forceService.TestForceAsync(DefaultTestForceG);
            if (double.IsNaN(measured))
            {
                MessageBox.Show("力测试失败，请检查板卡与力传感器接线。", "力测试",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool pass = _forceService.LastTestPassed;
            MessageBox.Show(
                string.Format("力测试结果\n目标力: {0:0.#}g\n实测力: {1:0.#}g\n判定: {2}",
                    DefaultTestForceG, measured, pass ? "通过" : "超差"),
                "力测试",
                MessageBoxButton.OK,
                pass ? MessageBoxImage.Information : MessageBoxImage.Warning);
        }

        #endregion

        #region 超声校准

        public string UltraCalibrationText
        {
            get { return _ultraCalibrationText; }
            set { _ultraCalibrationText = value; NotifyOfPropertyChange(() => UltraCalibrationText); }
        }

        public Brush UltraCalibrationBrush
        {
            get { return _ultraCalibrationBrush; }
            set { _ultraCalibrationBrush = value; NotifyOfPropertyChange(() => UltraCalibrationBrush); }
        }

        /// <summary>
        /// 超声校准：逐点输出功率给定-反馈-拟合（服务内部完成），完成后更新状态栏。
        /// </summary>
        public async void UltraCalibrate()
        {
            UltraCalibrationText = "超声校准: 校准中...";
            UltraCalibrationBrush = ForcePendingBrush;

            bool ok = await _ultraService.CalibrateUltraAsync();
            if (ok)
            {
                UltraCalibrationText = "超声校准: 已完成";
                UltraCalibrationBrush = ForceDoneBrush;
                MessageBox.Show("超声校准完成。", "超声校准",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                UltraCalibrationText = "超声校准: 失败";
                UltraCalibrationBrush = ForceFailBrush;
                MessageBox.Show("超声校准失败，请检查板卡与超声发生器接线。", "超声校准",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region 设备初始化

        public string InitText
        {
            get { return _initText; }
            set { _initText = value; NotifyOfPropertyChange(() => InitText); }
        }

        public Brush InitBrush
        {
            get { return _initBrush; }
            set { _initBrush = value; NotifyOfPropertyChange(() => InitBrush); }
        }

        /// <summary>
        /// 手动重新初始化板卡（复用启动链路 IBaseDispService.Initialize），完成后更新按钮状态。
        /// </summary>
        public async void Initialize()
        {
            InitText = "初始化中...";
            InitBrush = ForcePendingBrush;

            await Task.Run(() => _dispService.Initialize());

            if (_dispService.IsInitialized)
            {
                InitText = "已初始化";
                InitBrush = ForceDoneBrush;
                MessageBox.Show("初始化成功。", "初始化",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                InitText = "初始化失败";
                InitBrush = ForceFailBrush;
                MessageBox.Show("初始化失败，请检查板卡与驱动。", "初始化",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 根据启动时初始化结果设置按钮初始状态。
        /// </summary>
        private void UpdateInitState()
        {
            InitText = _dispService.IsInitialized ? "已初始化" : "初始化";
            InitBrush = _dispService.IsInitialized ? ForceDoneBrush : ForcePendingBrush;
        }

        #endregion
    }
}
