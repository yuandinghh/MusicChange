# MusicChange

 DotNetBar  buttonX 当鼠标移到控件上 会变色，如果想要鼠标移到控件上不变色
 如果你想在代码中使用 DbContextOptionsBuilder，必须先安装 Microsoft.EntityFrameworkCore NuGet 包，并在文件顶部添加上述 using 语句。

错误	CS8370	功能“创建目标类型对象”在 C# 7.3 中不可用。请使用 9.0 或更高的语言版本。

命名空间“MusicChange”中不存在类型或命名空间名“VideoEditor”(是否缺少程序集引用?)

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

数据库表关系图
 
这个设计提供了完整的用户管理功能，包括：
1.	基础用户信息 (users) - 存储用户的基本账户信息
2.	用户配置文件 (user_profiles) - 存储用户的个性化设置
3.	用户会话 (user_sessions) - 管理用户登录会话
4.	用户偏好 (user_preferences) - 存储用户的详细偏好设置
5.	 (user_logintime)   - 记录用户登录时间
特点：
•	使用外键约束保证数据完整性
•	创建索引提高查询性能
•	使用触发器自动更新时间戳
•	支持级联删除，当用户被删除时相关数据也会被自动清理
•	提供默认数据插入功能
	数据库表结构设计

## 1. users 表 - 用户信息表
列名			类型		约束					描述
id			INTEGER	PRIMARY KEY AUTOINCREMENT	用户ID
username		TEXT	NOT NULL UNIQUE	用户名
email	TEXT		NOT NULL UNIQUE	邮箱
password_hash	TEXT	NOT NULL	密码哈希值
full_name		TEXT		真实姓名
avatar_path		TEXT		头像路径
is_active		INTEGER	NOT NULL DEFAULT 1	是否激活
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间

	2. user_profiles 表 - 用户详细信息表
列名					类型	约束	描述
id					INTEGER	PRIMARY KEY AUTOINCREMENT	配置ID
user_id				INTEGER	NOT NULL UNIQUE, FOREIGN KEY REFERENCES users(id)	用户ID
preferred_language		TEXT		偏好语言
theme					TEXT		主题设置
default_project_location	TEXT		默认项目保存位置
auto_save_interval		INTEGER		自动保存间隔（分钟）
max_undo_steps			INTEGER		最大撤销步数
created_at				DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at				DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间

	3. user_sessions 表 - 用户会话表
列名			类型	约束	描述
id			INTEGER	PRIMARY KEY AUTOINCREMENT	会话ID
user_id		INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
session_token	TEXT	NOT NULL UNIQUE	会话令牌
ip_address		TEXT		IP地址
user_agent		TEXT		用户代理
expires_at		DATETIME	NOT NULL	过期时间
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间

	4. user_preferences 表 - 用户偏好设置表
列名				类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	偏好ID
user_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
preference_key		TEXT	NOT NULL	偏好键名
preference_value		TEXT	NOT NULL	偏好值
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间

	5. userslogintime 表 - 用户登录时间表
列名			类型	约束	描述
id			INTEGER	PRIMARY KEY AUTOINCREMENT	用户ID
iduser		INTEGER	NOT NULL UNIQUE, FOREIGN KEY REFERENCES users(id)	用户ID
login_time		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	登录时间
end_time		DATETIME		登出时间
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
count 		INTEGER	NOT NULL DEFAULT 1	登录次数
timelength		INTEGER	NOT NULL DEFAULT 0	在线时长（秒）



视频剪辑软件中的事务表主要用于记录用户的操作历史、项目变更、_undo/redo_操作等。以下是如何定义视频剪辑软件的事务表结构：
事务表结构设计
1. projects 表 - 项目信息表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	项目ID
user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name	TEXT	NOT NULL	项目名称
description	TEXT		项目描述
width	INTEGER	NOT NULL DEFAULT 1920	项目宽度
height	INTEGER	NOT NULL DEFAULT 1080	项目高度
framerate	REAL	NOT NULL DEFAULT 30.0	帧率
duration	REAL	NOT NULL DEFAULT 0.0	项目时长
thumbnail_path	TEXT		缩略图路径
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间


2. media_assets 表 - 媒体资源表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	资源ID
user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name	TEXT	NOT NULL	资源名称
file_path	TEXT	NOT NULL	文件路径
file_size	INTEGER	NOT NULL	文件大小(字节)
media_type	TEXT	NOT NULL	媒体类型(video/audio/image)
duration	REAL		时长(秒)
width	INTEGER		宽度
height	INTEGER		高度
framerate	REAL		帧率
codec	TEXT		编解码器
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间

3. timeline_tracks 表 - 时间线轨道表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	轨道ID
project_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_type	TEXT	NOT NULL	轨道类型(video/audio)
track_index	INTEGER	NOT NULL	轨道索引
name	TEXT	NOT NULL	轨道名称
is_muted	INTEGER	NOT NULL DEFAULT 0	是否静音
is_locked	INTEGER	NOT NULL DEFAULT 0	是否锁定
volume	REAL	NOT NULL DEFAULT 1.0	音量(0.0-1.0)
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间

 4. clips 表 - 剪辑片段表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	剪辑ID
project_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES timeline_tracks(id)	轨道ID
media_asset_id	INTEGER	FOREIGN KEY REFERENCES media_assets(id)	媒体资源ID
name	TEXT	NOT NULL	剪辑名称
start_time	REAL	NOT NULL	在时间线上的开始时间
end_time	REAL	NOT NULL	在时间线上的结束时间
media_start_time	REAL	NOT NULL DEFAULT 0.0	在媒体中的开始时间
media_end_time	REAL	NOT NULL	在媒体中的结束时间
position_x	REAL	NOT NULL DEFAULT 0.0	X位置
position_y	REAL	NOT NULL DEFAULT 0.0	Y位置
scale_x	REAL	NOT NULL DEFAULT 1.0	X缩放
scale_y	REAL	NOT NULL DEFAULT 1.0	Y缩放
rotation	REAL	NOT NULL DEFAULT 0.0	旋转角度
volume	REAL	NOT NULL DEFAULT 1.0	音量
is_muted	INTEGER	NOT NULL DEFAULT 0	是否静音
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
5. transactions 表 - 事务表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	事务ID
project_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
transaction_type	TEXT	NOT NULL	事务类型
description	TEXT	NOT NULL	事务描述
timestamp	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	时间戳
is_undone	INTEGER	NOT NULL DEFAULT 0	是否已撤销
6. transaction_details 表 - 事务详情表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	详情ID
transaction_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES transactions(id)	事务ID
operation_type	TEXT	NOT NULL	操作类型(create/update/delete)
table_name	TEXT	NOT NULL	表名
record_id	INTEGER	NOT NULL	记录ID
old_values	TEXT		旧值(JSON格式)
new_values	TEXT		新值(JSON格式)
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
7. effects 表 - 效果表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	效果ID
name	TEXT	NOT NULL	效果名称
effect_type	TEXT	NOT NULL	效果类型
description	TEXT		效果描述
parameters	TEXT		参数定义(JSON格式)
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
8. clip_effects 表 - 剪辑效果关联表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	关联ID
clip_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES clips(id)	剪辑ID
effect_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES effects(id)	效果ID
parameters	TEXT	NOT NULL	参数值(JSON格式)
start_time	REAL	NOT NULL DEFAULT 0.0	开始时间
end_time	REAL	NOT NULL	结束时间
order_index	INTEGER	NOT NULL	排序索引
is_enabled	INTEGER	NOT NULL DEFAULT 1	是否启用
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
