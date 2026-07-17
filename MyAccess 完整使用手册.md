# MyAccess 完整使用手册

---

## 目录

1. 框架概述

2. 核心命名空间与程序集

3. 实体映射特性（完整）

4. 初始化与基础配置

5. 单表 CRUD 完整用法

6. 查询高级用法（分页、排序、Take、Count、Exists）

7. 多表联查 JOIN（Left/Inner/Right/Full）

8. Include 子对象自动映射

9. 字段增量更新

10. 事务使用

11. 存储过程调用

12. 异步方法大全

13. Lambda 表达式转 SQL 规则（安全防注入）

14. 类型映射 DBMapping 规则

15. 常用执行器说明

16. 最佳实践与注意事项

---

# 1\. 框架概述

MyAccess.DB 是一套**面向 .NET 平台的轻量级 ORM 框架**，基于 [ADO.NET](https://ADO.NET) 封装，支持：

- 强类型 Lambda 生成 SQL

- 自动实体映射

- 单表 / 多表联查

- 增删改查 / 批量 / 事务 / 存储过程

- 同步 / 异步双模式

- 多数据库兼容（通过 ICompatible 扩展）

- 无侵入式实体特性映射

**核心优势**：不用写 SQL、不用配置文件、链式调用、性能接近原生 [ADO\.NET](https://ADO.NET)。

---

# 2\. 核心命名空间

```csharp
using MyAccess.DB;
using MyAccess.DB.Attr;
using MyAccess.DB.Builder;
```

---

# 3\. 实体映射特性（完整版）

所有特性均在 `MyAccess.DB.Attr` 下。

## 3\.1 TableNameAttribute（表名映射）

作用于**类**，指定实体对应数据库表名。

```csharp
[TableName("sys_user")]
public class SysUser
{
}
```

## 3\.2 IDAttribute（主键标记）

作用于**属性**，标记主键，支持自增、Oracle 序列。

```csharp
// MySQL 自增
[ID(true)]
public long Id { get; set; }

// Oracle 序列
[ID(false, "SEQ_USER")]
public long Id { get; set; }

// 非自增主键
[ID]
public long Id { get; set; }
```

## 3\.3 DataIgnoreAttribute（忽略字段）

作用于**属性**，表示该属性不参与 SQL 生成。

```csharp
[DataIgnore]
public string ExtData { get; set; }
```

## 3\.4 ColumnByAttribute（自定义列映射）

作用于**属性**，指定属性对应数据库列名，支持跨表映射。

```csharp
// 直接映射列名
[ColumnBy("user_name")]
public string UserName { get; set; }

// 映射其他实体类对应的列
[ColumnBy(typeof(SysRole), "role_name")]
public string RoleName { get; set; }
```

---

# 4\. 初始化与基础配置

## 4\.1 创建 DbHelp 实例

DbHelp 是数据库连接核心对象（需由具体数据库实现：MySQL/SQLServer/Oracle）。

```csharp
string connStr = "Data Source=...;";
DbHelp db = new MySqlDbHelp(connStr); // 具体实现类
```

## 4\.2 创建 SqlBuilder

```csharp
SqlBuilder sql = new SqlBuilder(db);
```

---

# 5\. 单表 CRUD 完整用法

## 5\.1 Insert（插入）

### 单条插入

```csharp
var user = new SysUser { UserName = "张三", Age = 20 };
int rows = sql.Insert(user).Do();
```

### 批量插入

```csharp
var list = new List<SysUser> { ... };
int rows = sql.Insert(list).Do();
```

### 插入并返回自增 ID

```csharp
var result = sql.Insert(user).DoReturnIdentity();
long newId = result.LastInsertedId;
```

## 5\.2 Update（更新）

### 按实体自动按主键更新

```csharp
var user = new SysUser { Id = 1, UserName = "张三三" };
int rows = sql.Update(user).Do();
```

### Lambda 条件更新

```csharp
int rows = sql.Update<SysUser>(user, u => u.Id == 1).Do();
```

## 5\.3 Delete（删除）

### 按主键删除

```csharp
int rows = sql.Delete<SysUser>().Do(1);
```

### 按 Lambda 删除

```csharp
int rows = sql.Delete<SysUser>(u => u.Age < 18).Do();
```

### 批量删除

```csharp
long[] ids = { 1, 2, 3 };
int rows = sql.Delete<SysUser>().Do(ids);
```

## 5\.4 Select（查询）

### 按主键查询单条

```csharp
SysUser user = sql.Query<SysUser>().ToEntity(1);
```

### Lambda 条件查询

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .Where(u => u.Age > 18 && u.UserName.Contains("张三"))
    .ToList();
```

### 查询第一条

```csharp
SysUser user = sql.Query<SysUser>()
    .Where(u => u.Id == 1)
    .ToFirst();
```

---

# 6\. 查询高级用法

## 6\.1 分页查询（带总数）

```csharp
int total = 0;
List<SysUser> list = sql.Query<SysUser>()
    .Where(u => true)
    .ToPage(
        page: 1,
        size: 10,
        ref total,
        orderby: "Id DESC"
    );
```

## 6\.2 仅分页（不带总数）

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .Where(u => true)
    .ToPage(1, 10, "Id DESC");
```

## 6\.3 限制返回条数（Take）

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .Where(u => true)
    .Take(5)
    .ToList();
```

## 6\.4 统计数量

```csharp
int count = sql.Query<SysUser>().Count(u => u.Age > 18);
```

## 6\.5 判断是否存在

```csharp
bool exists = sql.Query<SysUser>().Some(u => u.UserName == "张三");
```

---

# 7\. 多表联查 JOIN（最多 5 表）

框架自动分配别名：
**a \(第 1 张\) → b → c → d → e**

## 7\.1 两表 Left Join

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .LeftJoin<SysRole>((u, r) => u.RoleId == r.Id)
    .Where((u, r) => u.Age > 18)
    .ToList();
```

## 7\.2 三表 Inner Join

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .InnerJoin<SysRole>((u, r) => u.RoleId == r.Id)
    .InnerJoin<SysDept>((u, r, d) => r.DeptId == d.Id)
    .Where((u, r, d) => u.Id > 0)
    .ToList();
```

支持：

- LeftJoin

- InnerJoin

- RightJoin

- FullJoin

---

# 8\. Include 子对象自动映射

将关联表数据自动映射到实体子对象。

实体定义：

```csharp
public class SysUser
{
    [ID(true)]
    public long Id { get; set; }
    public long RoleId { get; set; }
    public SysRole Role { get; set; } // 子对象
}
```

查询：

```csharp
List<SysUser> list = sql.Query<SysUser>()
    .Include(u => u.Role, u => u.RoleId)
    .Where(u => u.Id == 1)
    .ToList();
```

---

# 9\. 字段增量更新

例如：数量 \+1、金额 × 倍率

```csharp
int rows = sql.UpdateColumns<SysUser>()
    .SetColum(u => u.Age, u => u.Age + 1)
    .Where(u => u.Id == 1)
    .Do();
```

---

# 10\. 事务使用

```csharp
try
{
    db.BeginTran();

    sql.Insert(user).Do();
    sql.Update(role).Do();

    db.Commit();
}
catch
{
    db.RollBack();
}
```

异步事务：

```csharp
await db.BeginTranAsync();
await db.CommitAsync();
await db.RollBackAsync();
```

---

# 11\. 存储过程调用

## 11\.1 执行无结果集

```csharp
int rows = sql.Do<DoExecStored>().RowCount;
```

## 11\.2 执行有结果集

```csharp
var result = sql.DoQueryOneStored<SysUser>();
SysUser user = result.First.ToFirst();
```

## 11\.3 获取输出参数

```csharp
var stored = sql.Do<DoQueryStored>();
int outVal = stored.OutInt("OUT_ID");
string strVal = stored.OutStr("OUT_NAME");
```

---

# 12\. 异步方法大全

几乎所有同步方法都提供异步版本，只需加 `Async` 后缀。

```csharp
// 插入
int rows = await sql.Insert(user).DoAsync();

// 查询
List<SysUser> list = await sql.Query<SysUser>().ToListAsync();

// 分页
var list = await query.ToPageAsync(1, 10, "Id DESC");

// 删除
int rows = await sql.Delete<SysUser>().DoAsync(1);
```

---

# 13\. Lambda 转 SQL 规则

框架自动将以下方法转为 SQL：

- Contains → LIKE '%xxx%'

- StartsWith → LIKE 'xxx%'

- EndsWith → LIKE '%xxx'

- == → =

-  \> \< =  !=→ 对应 SQL 大于、小于、等于、不等于

- && → AND

- \|\| → OR

**所有参数自动参数化，防 SQL 注入**。

---

# 14\. DBMapping 类型映射（内置）

框架自动完成 C\# ↔ DB 类型映射：

```Plain Text
DateTime ↔ DateTime
decimal ↔ decimal
string ↔ string
Int32 ↔ int
Int64 ↔ bigint
bool ↔ bit
byte / char / short / double / float 均支持
```

---

# 15\. 常用执行器（Do）

|类型|用途|
|---|---|
|DoExecSql|执行增删改，返回影响行数|
|DoQueryScalar|返回单值（COUNT/MAX/MIN）|
|DoQuerySql|返回实体列表|
|DoExecStored|执行存储过程|
|DoQueryTwo\&lt;T1,T2\&gt;|一次执行两条 SQL|

---

# 16\. 最佳实践

1. 所有实体必须标记 `[TableName]` 与 `[ID]`

2. 不使用字符串拼接 SQL，一律用 `AppendParam`

3. 多表查询最多 5 张表，复杂 SQL 建议拆分

4. 大批量操作优先用批量插入 / 更新

5. 写操作必须使用事务保证一致性

6. 优先使用异步方法提高吞吐量

7. 加 `[DataIgnore]` 忽略非数据库字段

---
