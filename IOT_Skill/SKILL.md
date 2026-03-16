---
name: 我的设备接入技能
description: 当用户询问「物联网设备」「设备数据查询」「操作设备」「监控」，或需要根据现实环境调用某些设备的功能（如在厨房锅里的水烧开时提醒我）时，使用此技能
version: 1.0.0
---

# 接口使用指南

## 概述
本技能提供多种设备的标准化调用方法，覆盖4大核心模块：
1. 我的设备（查询我的设备列表、每个设备基础信息和拥有的功能）
2. 设备状态（查询某个设备的当前状态信息）
3. 执行设备功能（执行设备的某个功能）
4. 环境监控（根据安装的监控设备获取画面的实时快照）

## 接口通用返回格式
所有接口统一返回以下3个参数：
| 参数名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| code | 整型 | 错误代码：0=成功，非0=失败 | 0 |
| message | 字符串 | 接口调用结果描述 | "执行成功" |
| data | 任意类型 | 接口返回的业务数据，不同接口返回不同 | {"Id":"dev123456"} |

## 接口列表
### 1. 物联网设备管理接口
| 接口名称 | 触发短语 | 请求方式 | 接口路径 | 必需参数 |
|----------|----------|----------|----------|----------|
| 按批次编号查询设备 | "按编号查设备"、"获取设备信息" | GET | /IoTService/HttpSync/DeviceByNumber | number（批次编号） |
| 添加物联网设备 | "添加物联网设备"、"创建设备" | POST | /IoTService/HttpSync/AddDevice | ProtocalId（协议Id）、ProductId（产品Id）、Name（设备名称）、DeviceNumber（批次编号）、DeviceId（通讯编码） |
| 编辑物联网设备 | "编辑设备"、"更新设备信息" | POST | /IoTService/HttpSync/EditDevice | 同「添加物联网设备」 |
| 绑定物联网设备 | "绑定设备"、"设备添加或更新" | POST | /IoTService/HttpSync/BindDevice | 同「添加物联网设备」 |
| 删除物联网设备 | "删除设备"、"移除物联网设备" | GET | /IoTService/HttpSync/DelDevice | number（批次编号） |
| 获取协议名称列表 | "查询协议列表"、"获取协议名称" | GET | /IoTService/HttpSync/ProtocalNames | pageNum（分页号）、pageSize（分页大小） |
| 获取设备实时属性数据 | "设备实时数据"、"实时属性" | GET | /IoTRulesService/HttpRule/Live | id（通讯Id） |
| 查询设备列表 | "设备列表"、"设备分页查询" | GET | /IoTService/HttpSync/ListPage | pageNum（可选）、pageSize（可选） |
| 查询设备历史数据 | "设备历史数据"、"历史属性" | GET | /IoTRulesService/HttpRule/SelectHistory | Number（批次编码） |
| 执行设备功能 | "运行设备功能"、"执行设备指令" | POST | /IoTService/HttpSync/ExeFunc | Number（批次编码）、FunctionId（功能标识） |
| 获取设备标签列表 | "设备标签"、"标签列表" | GET | /IoTService/HttpSync/TagList | number（批次编号） |
| 物联卡续费 | "物联卡续费"、"设备服务续费" | POST | /IoTService/HttpSync/Recharge | number（批次编号）、month（续费月份） |

### 2. 仓储管理接口
| 接口名称 | 触发短语 | 请求方式 | 接口路径 | 必需参数 |
|----------|----------|----------|----------|----------|
| 物品入库 | "物品入库"、"添加入库单" | POST | /StorageService/HttpSync/AddPileIn | StockNumber（入库单号）、ToHouseId（目标仓库Id）、InDate（入库时间）、Items（入库物品数组） |
| 物品出库 | "物品出库"、"添加出库单" | POST | /StorageService/HttpSync/AddPileOut | StockNumber（出库单号）、FromHouseId（来源仓库Id）、ToAgentId（目标代理商Id）、OutDate（出库时间）、Items（出库物品数组） |
| 清除指定入库单 | "清除入库单"、"删除入库记录" | GET | /StorageService/HttpSync/ClearPile | number（入库单号） |

### 3. 规则与房间管理接口
| 接口名称 | 触发短语 | 请求方式 | 接口路径 | 必需参数 |
|----------|----------|----------|----------|----------|
| 查询规则列表 | "规则列表"、"规则分页查询" | GET | /IoTRulesService/HttpRule/RuleListPage | pageNum（可选）、pageSize（可选） |
| 查询规则模板详情 | "规则详情"、"规则模板信息" | GET | /IoTRulesService/HttpRule/RuleInfo | id（规则Id） |
| 按房间查询实时数据 | "房间实时数据"、"房间设备数据" | GET | /AfterService/HttpSync/RoomLive | roomId（房间Id） |
| 查询房间列表 | "房间列表"、"获取房间信息" | GET | /AfterService/HttpSync/RoomList | 无 |

## 参考资源
查看 @references/环境监控使用说明.md 获取环境监控详细使用方式
查看 @references/环境监控接口详情.md 获取环境监控完整接口参数说明
查看 @examples/查询设备.py 获取设备查询完整示例
