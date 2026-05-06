# MinIO 安装与配置指南

# MinIO 安装与配置文档

## 一、appsettings\.json 配置项

编辑项目中的 `appsettings\.json` 文件，补充 MinIO 相关配置参数：

```json
"minio_server": "",
"minio_access": "",
"minio_secret": "",
"minio_bucket": "",
"minio_url": ""
```

## 二、Windows 系统安装 MinIO

打开 **PowerShell** 执行以下命令：

```powershell
# 下载 MinIO 可执行文件到 C 盘
Invoke-WebRequest -Uri "https://dl.minio.org.cn/server/minio/release/windows-amd64/minio.exe" -OutFile "C:\minio.exe"

# 设置 MinIO 管理员账号
setx MINIO_ROOT_USER admin

# 设置 MinIO 管理员密码
setx MINIO_ROOT_PASSWORD password

# 启动 MinIO 服务（数据存储在 F:\Data，控制台端口 9001）
C:\minio.exe server F:\Data --console-address ":9001"
```

## 三、Linux 系统安装 MinIO

根据服务器架构选择对应下载命令，执行以下操作：

### 1\. ARM64 架构

```bash
# 下载 MinIO
wget https://dl.min.io/server/minio/release/linux-arm64/minio

# 添加执行权限
chmod +x minio

# 启动 MinIO 服务（数据存储在 /mnt/data，控制台端口 9001）
MINIO_ROOT_USER=admin MINIO_ROOT_PASSWORD=password ./minio server /mnt/data --console-address ":9001"
```

### 2\. AMD64 架构

```bash
# 下载 MinIO
wget https://dl.min.io/server/minio/release/linux-amd64/minio

# 添加执行权限
chmod +x minio

# 启动 MinIO 服务（数据存储在 /mnt/data，控制台端口 9001）
MINIO_ROOT_USER=admin MINIO_ROOT_PASSWORD=password ./minio server /mnt/data --console-address ":9001"
```

### 总结

1. 配置文件需填写 MinIO 服务地址、密钥、存储桶等参数；

2. Windows 安装需下载 exe 文件，通过 `setx` 配置账号密码并启动服务；

3. Linux 分架构下载，赋予执行权限后直接启动，默认账号密码均为 `admin/password`。

> （注：文档部分内容可能由 AI 生成）
