; ============================================================
; MainPlatform 引线键合机上位机 - Inno Setup 安装包脚本
; 编译方式: ISCC.exe /DBuildConfig=Release script.iss
; ============================================================

; ---------- ISPP 变量定义 ----------
#ifndef BuildConfig
  #define BuildConfig "Release"
#endif

#define MyAppName       "引线键合机上位机"
#define MyAppVersion    "1.0.0"
#define MyAppPublisher  "SCMC"
#define MyAppExeName    "MainPlatform.exe"
#define MyAppId         "{{8B2E1F4A-7C3D-4E5F-9A6B-1D2C3E4F5A6B}}"
#define MySetupPassword "scmc2026"

; ---------- 源文件根目录（相对于本脚本所在的 Wizard\ 目录） ----------
#define SrcDir "..\MainPlatform\bin\" + BuildConfig

[Setup]
; --- 程序标识 ---
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

; --- 安装路径 ---
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

; --- 输出 ---
OutputDir=..\Installer
OutputBaseFilename=MainPlatform_Setup_v{#MyAppVersion}_{#BuildConfig}
Compression=lzma2/ultra
SolidCompression=yes
WizardStyle=modern
SetupIconFile=..\MainPlatform\Resources\Images\SCMC.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

; --- 安全（无密码） ---
; Password=
; Encryption=

; --- 卸载 ---
Uninstallable=yes
AllowNoIcons=yes

; ---------- 语言 ----------
[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Default.isl"

; ---------- 中文界面消息覆盖（仅使用 Inno Setup 6.4 有效名称） ----------
[Messages]
; 窗口标题
SetupAppTitle=安装程序
SetupWindowTitle=引线键合机上位机 - 安装向导
UninstallAppTitle=卸载程序
UninstallAppFullTitle=引线键合机上位机 - 卸载向导

; 欢迎页
WelcomeLabel1=欢迎使用引线键合机上位机安装向导
WelcomeLabel2=本程序将在您的计算机上安装 %1。%n%n建议在继续之前关闭所有其他应用程序。

; 密码页
WizardPassword=密码
PasswordLabel1=此安装程序已加密。
PasswordLabel3=请输入安装密码，然后单击"下一步"继续。密码区分大小写。
PasswordEditLabel=密码(&P):
IncorrectPassword=密码不正确。请重试。

; 选择任务页
WizardSelectTasks=选择附加任务
SelectTasksDesc=请选择安装引线键合机上位机时要执行的附加任务。
SelectTasksLabel2=选择要安装程序执行的附加任务，然后单击"下一步"。

; 准备安装
WizardReady=准备安装
ReadyLabel1=安装程序已准备好开始安装。
ReadyLabel2a=单击"安装"以继续安装。
ReadyMemoDir=安装位置:
ReadyMemoGroup=开始菜单文件夹:
ReadyMemoTasks=附加任务:

; 选择安装目录
WizardSelectDir=选择目标位置
SelectDirDesc=请选择引线键合机上位机的安装文件夹。
SelectDirLabel3=安装程序将把引线键合机上位机安装到以下文件夹。
SelectDirBrowseLabel=要继续，请单击"下一步"。要选择其他文件夹，请单击"浏览"。
DiskSpaceMBLabel=至少需要 [mb] MB 的可用磁盘空间。
DiskSpaceGBLabel=至少需要 [gb] GB 的可用磁盘空间。

; 选择开始菜单文件夹
WizardSelectProgramGroup=选择开始菜单文件夹
SelectStartMenuFolderDesc=请选择要在其中创建程序快捷方式的开始菜单文件夹。
SelectStartMenuFolderLabel3=安装程序将在以下开始菜单文件夹中创建程序快捷方式。
SelectStartMenuFolderBrowseLabel=要继续，请单击"下一步"。要选择其他文件夹，请单击"浏览"。

; 准备安装
WizardReady=准备安装
ReadyLabel1=安装程序已准备好开始安装。
ReadyLabel2a=单击"安装"以继续安装。

; 安装中
WizardInstalling=正在安装
InstallingLabel=请稍候，正在安装引线键合机上位机...
StatusCreateDirs=正在创建文件夹
StatusExtractFiles=正在解压文件
StatusCreateIcons=正在创建快捷方式
StatusSavingUninstall=正在保存卸载信息

; 完成页
FinishedHeadingLabel=完成引线键合机上位机安装向导
FinishedLabelNoIcons=引线键合机上位机已成功安装到您的计算机。
FinishedLabel=引线键合机上位机已成功安装到您的计算机。%n%n单击"完成"关闭此向导。
FinishedRestartLabel=要完成安装，必须重新启动计算机。是否立即重新启动？
ClickFinish=单击"完成"退出安装程序。

; 对话框标题
ConfirmTitle=确认
ErrorTitle=错误
InformationTitle=信息

; 底部标签
BeveledLabel=引线键合机上位机 v1.0.0

; 卸载
ConfirmUninstall=您确定要完全删除引线键合机上位机及其所有组件吗？
UninstallStatusLabel=正在卸载引线键合机上位机...
UninstalledAll=引线键合机上位机已成功从您的计算机中移除。

; 按钮
ButtonBack=< 上一步(&B)
ButtonNext=下一步(&N) >
ButtonInstall=安装(&I)
ButtonOK=确定
ButtonCancel=取消
ButtonYes=是(&Y)
ButtonNo=否(&N)
ButtonFinish=完成(&F)
ButtonBrowse=浏览(&R)...
ButtonWizardBrowse=浏览(&R)...

; 其他
ClickNext=请单击"下一步"继续，或单击"取消"退出安装程序。
BrowseDialogTitle=浏览文件夹
BrowseDialogLabel=请选择安装目标文件夹：
ExitSetupTitle=退出安装程序
ExitSetupMessage=您确定要退出安装吗？
SetupAborted=安装已取消。
ErrorCreatingDir=安装程序无法创建文件夹：%n%n%1%n%n请检查磁盘是否已满或是否有写入权限。
InvalidParameter=命令行参数无效：%n%n%1
InvalidPath=指定的路径无效。
InvalidDrive=指定的驱动器无效。
DiskSpaceWarningTitle=磁盘空间不足
DiskSpaceWarning=安装程序需要至少 %1 MB 的可用磁盘空间，但选定的驱动器只有 %2 MB 可用。%n%n是否继续？
DirExistsTitle=文件夹已存在
DirExists=文件夹 %1 已存在。是否要安装到此文件夹？

; ---------- 自定义消息（{cm:...} 引用） ----------
[CustomMessages]
NameAndVersion=%1 版本 %2
AdditionalIcons=附加图标:
CreateDesktopIcon=创建桌面快捷方式(&D)
CreateQuickLaunchIcon=创建快速启动栏图标(&Q)
UninstallProgram=卸载 %1
LaunchProgram=启动 %1

; ---------- 自定义任务 ----------
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

; ---------- 安装时创建的目录 ----------
; 说明：软件运行时会向 Data\scmc.db 写入登录时间/记住密码等数据。
; 安装目录位于 Program Files 下默认 Users 仅有只读权限，会导致 UserDataService.Update 写库失败、
; 登录无法进入主界面。故必须为 Data 目录授予 Users 修改(写)权限。
[Dirs]
Name: "{app}\Data"; Flags: uninsalwaysuninstall; Permissions: users-modify

; ---------- 打包文件 ----------
[Files]
; 主程序与配置
Source: "{#SrcDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SrcDir}\*.config"; DestDir: "{app}"; Flags: ignoreversion

; 托管依赖 DLL（排除调试符号和文档）
Source: "{#SrcDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion; Excludes: "*.pdb,*.xml"

; 配置文件目录
Source: "{#SrcDir}\Config\*"; DestDir: "{app}\Config"; Flags: ignoreversion recursesubdirs createallsubdirs

; 数据目录（运行时生成，源不存在则跳过）
Source: "{#SrcDir}\Data\*"; DestDir: "{app}\Data"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

; SQLite 原生互操作库（直接从 NuGet packages 取，避免输出目录复制时序问题）
Source: "..\packages\Stub.System.Data.SQLite.Core.NetFramework.1.0.119.0\build\net46\x64\SQLite.Interop.dll"; DestDir: "{app}\x64"; Flags: ignoreversion
Source: "..\packages\Stub.System.Data.SQLite.Core.NetFramework.1.0.119.0\build\net46\x86\SQLite.Interop.dll"; DestDir: "{app}\x86"; Flags: ignoreversion

; ---------- 快捷方式 ----------
[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

; ---------- 安装后运行 ----------
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

; ---------- 卸载时删除数据和配置 ----------
[UninstallDelete]
Type: filesandordirs; Name: "{app}\Data"
Type: filesandordirs; Name: "{app}\Config"
