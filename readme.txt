c# DotNetBar  buttonX 当鼠标移到控件上 会变色，如果想要鼠标移到控件上不变色

 EF6 的配置方式。

 如果你想在代码中使用 DbContextOptionsBuilder，必须先安装 Microsoft.EntityFrameworkCore NuGet 包，并在文件顶部添加上述 using 语句。

错误	CS8370	功能“创建目标类型对象”在 C# 7.3 中不可用。请使用 9.0 或更高的语言版本。

我将为您设计一个视频剪辑程序的个人用户数据库表，并提供 SQLite 建表程序。
数据库表结构设计
1. users 表 - 用户信息表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	用户ID
username	TEXT	NOT NULL UNIQUE	用户名
email	TEXT	NOT NULL UNIQUE	邮箱
password_hash	TEXT	NOT NULL	密码哈希值
full_name	TEXT		真实姓名
avatar_path	TEXT		头像路径
is_active	INTEGER	NOT NULL DEFAULT 1	是否激活
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
2. user_profiles 表 - 用户详细信息表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	配置ID
user_id	INTEGER	NOT NULL UNIQUE, FOREIGN KEY REFERENCES users(id)	用户ID
preferred_language	TEXT		偏好语言
theme	TEXT		主题设置
default_project_location	TEXT		默认项目保存位置
auto_save_interval	INTEGER		自动保存间隔（分钟）
max_undo_steps	INTEGER		最大撤销步数
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
3. user_sessions 表 - 用户会话表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	会话ID
user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
session_token	TEXT	NOT NULL UNIQUE	会话令牌
ip_address	TEXT		IP地址
user_agent	TEXT		用户代理
expires_at	DATETIME	NOT NULL	过期时间
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
4. user_preferences 表 - 用户偏好设置表
列名	类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	偏好ID
user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
preference_key	TEXT	NOT NULL	偏好键名
preference_value	TEXT	NOT NULL	偏好值
created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
C# SQLite 建表程序
 
使用示例
 
数据库表关系图
 
这个设计提供了完整的用户管理功能，包括：
1.	基础用户信息 (users) - 存储用户的基本账户信息
2.	用户配置文件 (user_profiles) - 存储用户的个性化设置
3.	用户会话 (user_sessions) - 管理用户登录会话
4.	用户偏好 (user_preferences) - 存储用户的详细偏好设置
特点：
•	使用外键约束保证数据完整性
•	创建索引提高查询性能
•	使用触发器自动更新时间戳
•	支持级联删除，当用户被删除时相关数据也会被自动清理
•	提供默认数据插入功能

