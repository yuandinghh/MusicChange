/*
1. ----------------------- projects 表 - 项目信息表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT				项目ID
user_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name			TEXT	NOT NULL								项目名称
description		TEXT		项目描述
width			INTEGER	NOT NULL DEFAULT 1920				项目宽度
height			INTEGER	NOT NULL DEFAULT 1080				项目高度
framerate		REAL	NOT NULL DEFAULT 30.0				帧率
duration		REAL	NOT NULL DEFAULT 0.0				项目时长
thumbnail_path		TEXT		缩略图路径
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note			TEXT		备注信息
		约束：PRIMARY KEY(id)
		约束：FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE
		索引：UNIQUE(name, user_id)

2. ------------------- media_assets 表 - 媒体资源表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	资源ID
user_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name			TEXT	NOT NULL					资源名称
file_path			TEXT	NOT NULL			文件路径
file_size			INTEGER	NOT NULL				文件大小(字节)
media_type			TEXT	NOT NULL				媒体类型(video/audio/image)
duration			REAL		时长(秒)
width				INTEGER		宽度
height				INTEGER		高度
framerate			REAL		帧率
codec				TEXT		编解码器
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	 DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息

3. ----------------------------- timeline_tracks 表 - 时间线轨道表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	轨道ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_type			TEXT	NOT NULL	轨道类型(video/audio)
track_index			INTEGER	NOT NULL	轨道索引
name				TEXT	NOT NULL	轨道名称
is_muted			INTEGER	NOT NULL DEFAULT 0	是否静音
is_locked			INTEGER	NOT NULL DEFAULT 0	是否锁定
volume				REAL	NOT NULL DEFAULT 1.0	音量(0.0-1.0)
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息

4. ----------------------------- clips 表 - 剪辑片段表
列名				类型	约束	描述
id					INTEGER	PRIMARY KEY AUTOINCREMENT	剪辑ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES timeline_tracks(id)	轨道ID
media_asset_id		INTEGER	FOREIGN KEY REFERENCES media_assets(id)	媒体资源ID
name				TEXT	NOT NULL		剪辑名称
start_time			REAL	NOT NULL		在时间线上的开始时间
end_time			REAL	NOT NULL		在时间线上的结束时间
media_start_time	REAL	NOT NULL DEFAULT 0.0	在媒体中的开始时间
media_end_time		REAL	NOT NULL				在媒体中的结束时间
position_x			REAL	NOT NULL DEFAULT 0.0	X位置
position_y			REAL	NOT NULL DEFAULT 0.0	Y位置
scale_x				REAL	NOT NULL DEFAULT 1.0	X缩放
scale_y				REAL	NOT NULL DEFAULT 1.0	Y缩放
rotation			REAL	NOT NULL DEFAULT 0.0	旋转角度
volume				REAL	NOT NULL DEFAULT 1.0	音量
is_muted			INTEGER	NOT NULL DEFAULT 0		是否静音
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间?????????
note				TEXT		备注信息


5. --------------------------- transactions 表 - 事务表
列名				类型	约束	描述
id					INTEGER	PRIMARY KEY AUTOINCREMENT	事务ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
user_id				INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
transaction_type	TEXT	NOT NULL									事务类型
description			TEXT	NOT NULL							事务描述
timestamp			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	时间戳

is_undone			INTEGER	NOT NULL DEFAULT 0					是否已撤销
is_redone			INTEGER	NOT NULL DEFAULT 0					是否已重做??????????
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息
		约束：PRIMARY KEY(id)
		约束：FOREIGN KEY REFERENCES projects(id) ON DELETE CASCADE
		约束：FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE

6. ----------------------- transaction_details 表 - 事务详情表
列名			类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	详情ID
transaction_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES transactions(id)	事务ID
operation_type	TEXT	NOT NULL	操作类型(create/update/delete)
table_name		TEXT	NOT NULL	表名
record_id		INTEGER	NOT NULL	记录ID
old_values		TEXT		旧值(JSON格式)
new_values		TEXT		新值(JSON格式)
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息

7. ----------------------------effects 表 - 效果表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	效果ID
name			TEXT	NOT NULL	效果名称
effect_type		TEXT	NOT NULL	效果类型
description		TEXT		效果描述
parameters		TEXT		参数定义(JSON格式)
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息

8. -----------------clip_effects 表 - 剪辑效果关联表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	关联ID
clip_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES clips(id)	剪辑ID
effect_id		INTEGER	NOT NULL, FOREIGN KEY REFERENCES effects(id)	效果ID
parameters		TEXT	NOT NULL	参数值(JSON格式)
start_time		REAL	NOT NULL DEFAULT 0.0	开始时间
end_time		REAL	NOT NULL	结束时间
order_index		INTEGER	NOT NULL	排序索引
is_enabled		INTEGER	NOT NULL DEFAULT 1	是否启用
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicChange
{
	internal class temp
	{
		//public TransactionRepository(string dbPath)
		//{
		//	//_connectionString = $"Data Source={dbPath};Version=3;";
		//}

	}
	/*
	*/
}
