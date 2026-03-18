---
name: 设备监控与控制能力
description: 当用户询问「物联网设备」「设备数据查询」「操作设备」「监控」，或需要根据现实环境调用某些设备的功能（如在厨房锅里的水烧开时提醒我）时，使用此技能
version: 1.0.0
---

# 接口使用指南

## 概述
本技能提供多种设备的标准化调用方法，由2大模块组成：
1. 设备相关API（包括我的设备、设备实时属性、执行设备功能、获取rtmp流地址）
2. 环境监控API（根据安装的监控设备获取画面的实时快照）

所有接口统一返回以下3个参数：
| 参数名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| code | 整型 | 错误代码：0=成功，非0=失败 | 0 |
| message | 字符串 | 接口调用结果描述 | "执行成功" |
| data | 任意类型 | 接口返回的业务数据，不同接口返回不同 | {"Id":"dev123456"} |

## 1. 设备相关API
### 接口使用前置条件
请确保物联网平台已安装部署
部署后接口地址：http://127.0.0.1:880

### 我的所有设备
接口介绍：查询我的设备列表和每个设备的所在位置、联网状态、设备名称、备注说明、通讯Id、第三方编码、功能定义、属性定义、是否为视频监控
请求方式：GET
接口路径：/AfterService/HttpSync/MyDevices

查看 references/我的所有设备.md 获取详细接口介绍和完整示例

### 设备实时属性
接口介绍：获取指定设备的实时属性数据，例如设备当前温度、开关状态等。设备的属性定义决定了设备会有哪些实时属性数据。
请求方式：GET
接口路径：/IoTRulesService/HttpRule/Live

查看 references/设备实时属性.md 获取详细接口介绍和完整示例

### 执行设备功能
接口介绍：调用设备的某项功能，例如开、关设备。设备的功能定义决定了设备会有哪些可执行的功能。
请求方式：POST
接口路径：/IoTService/HttpSync/ExeFunc

查看 references/执行设备功能.md 获取详细接口介绍和完整示例

### 获取rtmp流地址
接口介绍：获取指定视频监控设备的Rtmp播放流。
请求方式：GET
接口路径：/IoTVideoService/HttpSync/GetRtmpPlayUrl

查看 references/获取rtmp流地址.md 获取详细接口介绍和完整示例

## 2. 环境监控API
### 接口使用前置条件
1. 安装依赖：`pip install -r requirements.txt`
2. 安装FFmpeg（OpenCV解码RTMP必需）：
   - Ubuntu/Debian：`apt install ffmpeg`
   - CentOS/RHEL：`yum install ffmpeg`
   - Windows：下载FFmpeg并添加到环境变量
3. 启动服务（默认端口8111）：`python scripts/rtmp_api_server.py`

服务启动后接口地址：http://127.0.0.1:8111


### 启动监控（后台循环拉流）
```bash
curl -X POST http://127.0.0.1:8111/api/start-monitor \
-H "Content-Type: application/json" \
-d '{"stream_id":"stream1","rtmp_url":"rtmp://192.168.1.100/live/camera1"}'
```

### 停止监控
```bash
curl -X POST http://127.0.0.1:8111/api/stop_monitor \
-H "Content-Type: application/json" \
-d '{"stream_id":"stream1"}'
```

### 按需获取最新画面
```bash
curl -X POST http://127.0.0.1:8111/api/get_frame \
-H "Content-Type: application/json" \
-d '{"stream_id":"stream1"}'
```

### 查看所有监控流状态
```bash
curl http://127.0.0.1:8111/api/list-monitor
```
