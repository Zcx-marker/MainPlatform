using Caliburn.Micro;

namespace MainPlatform.ViewModels
{
    /// <summary>
    /// Xh 楔型焊接 - 弧形参数页 ViewModel。
    /// 仿参考图布局：左列 8 项 + 右列 8 项，参数全部可读写绑定。
    /// </summary>
    public class XhHuXingViewModel : Screen
    {
        // ============ 左列 ============
        private string _arcType = "标准";
        public string ArcType
        {
            get => _arcType;
            set { _arcType = value; NotifyOfPropertyChange(() => ArcType); }
        }

        private string _reverseHeight = "180";
        public string ReverseHeight
        {
            get => _reverseHeight;
            set { _reverseHeight = value; NotifyOfPropertyChange(() => ReverseHeight); }
        }

        private string _reverseAngle = "-45";
        public string ReverseAngle
        {
            get => _reverseAngle;
            set { _reverseAngle = value; NotifyOfPropertyChange(() => ReverseAngle); }
        }

        private string _arcHeight = "150";
        public string ArcHeight
        {
            get => _arcHeight;
            set { _arcHeight = value; NotifyOfPropertyChange(() => ArcHeight); }
        }

        private string _pullArcType = "圆弧";
        public string PullArcType
        {
            get => _pullArcType;
            set { _pullArcType = value; NotifyOfPropertyChange(() => PullArcType); }
        }

        private string _rotateDistance = "+100";
        public string RotateDistance
        {
            get => _rotateDistance;
            set { _rotateDistance = value; NotifyOfPropertyChange(() => RotateDistance); }
        }

        private bool _arcTopClamp;
        public bool ArcTopClamp
        {
            get => _arcTopClamp;
            set { _arcTopClamp = value; NotifyOfPropertyChange(() => ArcTopClamp); }
        }

        private string _actionThreshold = "100";
        public string ActionThreshold
        {
            get => _actionThreshold;
            set { _actionThreshold = value; NotifyOfPropertyChange(() => ActionThreshold); }
        }

        // ============ 右列 ============
        private string _afAngle = "-15";
        public string AfAngle
        {
            get => _afAngle;
            set { _afAngle = value; NotifyOfPropertyChange(() => AfAngle); }
        }

        private string _reverseDistance = "200";
        public string ReverseDistance
        {
            get => _reverseDistance;
            set { _reverseDistance = value; NotifyOfPropertyChange(() => ReverseDistance); }
        }

        private string _arcBase = "固定弧高";
        public string ArcBase
        {
            get => _arcBase;
            set { _arcBase = value; NotifyOfPropertyChange(() => ArcBase); }
        }

        private string _arcHeightComp = "+0";
        public string ArcHeightComp
        {
            get => _arcHeightComp;
            set { _arcHeightComp = value; NotifyOfPropertyChange(() => ArcHeightComp); }
        }

        private string _arcAngle = "36";
        public string ArcAngle
        {
            get => _arcAngle;
            set { _arcAngle = value; NotifyOfPropertyChange(() => ArcAngle); }
        }

        private bool _presetWeld = true;
        public bool PresetWeld
        {
            get => _presetWeld;
            set { _presetWeld = value; NotifyOfPropertyChange(() => PresetWeld); }
        }

        private string _heightDiff = "+0";
        public string HeightDiff
        {
            get => _heightDiff;
            set { _heightDiff = value; NotifyOfPropertyChange(() => HeightDiff); }
        }

        private string _weldPitch = "150";
        public string WeldPitch
        {
            get => _weldPitch;
            set { _weldPitch = value; NotifyOfPropertyChange(() => WeldPitch); }
        }
    }
}
