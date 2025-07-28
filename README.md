# MusicChange

 DotNetBar  buttonX 当鼠标移到控件上 会变色，如果想要鼠标移到控件上不变色
 如果你想在代码中使用 DbContextOptionsBuilder，必须先安装 Microsoft.EntityFrameworkCore NuGet 包，并在文件顶部添加上述 using 语句。

错误	CS8370	功能“创建目标类型对象”在 C# 7.3 中不可用。请使用 9.0 或更高的语言版本。

1.	clips - 存储音频剪辑的基本信息
2.	projects - 存储项目信息
3.	clip_effects - 存储剪辑效果设置
----------------  clips 表  -----------------
字段名	类型	描述
Id		INTEGER	主键
Name		TEXT	剪辑名称
FilePath	TEXT	文件路径
ProjectId	INTEGER	所属项目ID
StartPosition	REAL	开始位置（秒）
EndPosition	REAL	结束位置（秒）
Speed		REAL	播放速度
Pitch		REAL	音调调整
CreatedAt	DATETIME	创建时间
UpdatedAt	DATETIME	更新时间

projects 表
字段名	类型	描述
Id		INTEGER	主键
Name		TEXT	项目名称
Description	TEXT	项目描述
CreatedAt	DATETIME	创建时间
UpdatedAt	DATETIME	更新时间

clip_effects 表
字段名	类型	描述
Id		INTEGER	主键
ClipId	INTEGER	剪辑ID
EffectType	TEXT	效果类型
Value	REAL	效果值
CreatedAt	DATETIME	创建时间

