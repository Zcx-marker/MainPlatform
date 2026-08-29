using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MainPlatform.Models
{
    /// <summary>
    /// 软件界面总配置（对应 AppConfig/UiConfig.json）
    /// </summary>
    [DataContract]
    public class UiConfig
    {
        [DataMember]
        public string AppTitle { get; set; } = string.Empty;
        [DataMember]
        public string AppSubTitle { get; set; } = string.Empty;
        [DataMember]
        public List<Menu> MenuList { get; set; } = new List<Menu>();
    }

    /// <summary>
    /// 一级菜单（界面分组）
    /// </summary>
    [DataContract]
    public class Menu : INotifyPropertyChanged
    {
        [DataMember]
        public string Key { get; set; } = string.Empty;
        [DataMember]
        public string DisplayName { get; set; } = string.Empty;
        [DataMember]
        public string Icon { get; set; } = string.Empty;
        [DataMember]
        public List<SubMenu> SubMenuList { get; set; } = new List<SubMenu>();

        #region HidenProp
        [JsonIgnore]
        public string IconGlyph => Common.IconMap.Get(Icon);

        private bool _isExpanded = false;
        [JsonIgnore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set { if (_isExpanded == value) return; _isExpanded = value; OnPropertyChanged(); }
        }

        private bool _isSelected = false;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected == value) return; _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
        #endregion
    }

    /// <summary>
    /// 二级页面（可切换的界面）
    /// </summary>
    [DataContract]
    public class SubMenu : INotifyPropertyChanged
    {
        [DataMember]
        public string Key { get; set; } = string.Empty;
        [DataMember]
        public string DisplayName { get; set; } = string.Empty;
        [DataMember]
        public string Icon { get; set; } = string.Empty;
        [DataMember]
        /// <summary>要加载的 ViewModel 全名，例如 WpfApp1.ViewModels.Pages.OverviewPageViewModel</summary>
        public string ViewModel { get; set; } = string.Empty;

        #region HidenOrio
        [JsonIgnore]
        public string ParentKey { get; set; } = string.Empty;
        [JsonIgnore]
        public string FullKey { get; set; } = string.Empty;

        [JsonIgnore]
        public string IconGlyph => Common.IconMap.Get(Icon);

        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected == value) return; _isSelected = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 该界面的权限标识（用于控制页面的显示模式：只读，读写(可观看可操作)，无权限）:0-无权限，1-只读，2-读写
        /// 默认为"2"拥有读写权限，根据RoleConfig.json中配置的权限来决定是否覆盖默认值。
        /// </summary>
        [JsonIgnore]
        public int Limit { get; set; } = 2;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
        #endregion
    }
}
