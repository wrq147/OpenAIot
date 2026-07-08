# 项目部署与开发文档

项目完全开源，开源目标只为卷死其它物联网相关平台！有意向加入的，可以加最底部的Q群。

## 一、开发环境前置要求

Windows 开发环境：Visual Studio 2022，安装.NET 9 SDK 运行时与开发组件

Linux 部署环境：CentOS 7+/Ubuntu，Docker & Docker Compose

数据库 / 中间件依赖：MySQL、Redis、NATS（统一通过 docker compose 一键拉起）

## 二、项目使用说明

1. 安装docker

2. 执行 `docker compose up -d` 安装依赖服务

3. 将mysql连接字符串配置在 `appsettings.json` 的 `connstr` 节点

4. 将redis的连接字符串配置在 `appsettings.json` 的 `redisconn` 节点

5. 将nats的连接字符串配置在 `appsettings.json` 的 `event_bus_conn` 节点，并在event_bus_user和event_bus_pass填上账号、密码

6. 修改 `appsettings.json` 的 `General=>url` 为对应域名

### 物联网配置说明

Channel目录存放着所有可使用的设备接入方式，其中MqttChannel为当前主流的mqtt接入方式。以下是mqtt接入方式的配置：

1、将MqttChannel发布到指定服务器。

2、修改MqttChannel里的appsettings.json，按下面修改配置
    "event_conn": "nas总线地址",
    "event_user": "nas总线用户名",
    "event_pass": "nas总线密码",
    "redis_conn": "redis连接串",
    "mqtt_server": "emqx服务地址",
    "mqtt_port": emqx服务端口,
    "mqtt_username": "emqx服务连接用户名",
    "mqtt_password": "emqx服务连接密码"

3、./MqttChannel &  启动服务

## 三、国内CentOS系统安装Docker

### 步骤1：获取 docker-ce 软件源的 yum repo

```bash
sudo yum install -y yum-utils device-mapper-persistent-data lvm2 
sudo yum-config-manager --add-repo http://mirrors.aliyun.com/docker-ce/linux/centos/docker-ce.repo
```

### 步骤2：安装Docker

```bash
sudo yum install -y docker-ce docker-ce-cli containerd.io
```

### 步骤3：启动 docker 并设置开机启动

```bash
sudo systemctl start docker
sudo systemctl enable docker
```

### 步骤4：设置docker国内镜像

编辑 `/etc/docker/daemon.json`（文件不存在则创建），添加以下配置：

```json
{  
  "registry-mirrors": [  
    "https://docker.xuanyuan.me",
    "https://docker.m.daocloud.io",
    "https://docker.imgdb.de",
    "https://docker-0.unsee.tech",
    "https://docker.hlmirror.com"
  ]  
}
```
systemctl daemon-reload
systemctl restart docker

## 四、项目目录结构说明

|目录/文件|说明|
|---|---|
|admin-ui|中台前端|
|AirJointUI、AirJointUI.Desktop|嵌入式联控屏|
|IoTCenterPlatformApp|中台移动端项目|
|App|中台后端启动入口项目|
|Channel|中台物联通道项目|
|DigitalWxApp|数字名片移动端项目|
|energyAdminUI|能碳管理项目|
|Modules|中台后端各个业务模块|
|MyAccess|中台后端ORM框架|
|TemplateAction、TemplateAction.NetCore|中台后端模块化MVC框架|
|Third|第三方模板|
|autoexe.sh|Linux环境自启动脚本|

## 五、后端模块功能说明

|模块名称|功能描述|
|---|---|
|AfterService|设备中台模块|
|AuthService|会员、身份验证、部门、角色等基础信息管理|
|CardService|名片模块|
|CRMService|CRM客户管理模块|
|DeveloperService|开发者接口模块|
|DictService|字典数据模块|
|DiscussService|主题评论模块|
|EfficiencyService|能效管理模块|
|EmailService|邮件服务模块|
|FlowService|工作流模块|
|IoTAIService|物联AI模块|
|IoTRulesService|物联规则引擎模块|
|IoTService|物联核心模块|
|IoTVideoService|视频监控模块|
|LLMService|AI助手模块|
|MessageService|站内消息模块|
|MESService|制造执行系统模块|
|MonitorService|系统监控、定时任务模块|
|MqttService|Mqtt消息服务模块|
|PayService|支付模块|
|ProducerService|厂家资料管理|
|ReportService|报表模块|
|SMSService|短信服务模块|
|StorageService|数智仓储模块|
|ThirdPartyService|天气等第三方接口服务|
|WeiXinService|微信与企业微信集成模块|

## 六、模块开发规范

1. 每个模块需在 `Modules` 目录下创建独立文件夹管理

2. 需启用的模块，在 `App` 入口项目的 `services.txt` 文件中添加模块名称完成注册。当一个模块不在`services.txt` 文件，但又被另一人模块引用时，这个模块将只被当成类库使用。

3. 在 `App` 入口项目引用对应模块，编译后模块会自动生成至项目目录

4. 每个模块下必须创建 `PluginConfig.cs` 文件，且需继承 `TANetCorePluginConfig`，模块启动时会自动执行该文件初始化逻辑

5. 需要使用定时任务的模块，在 `Configure` 方法中添加 `plg.RegisterQuartzTask();`

## 七、模块间通信规范

> 注意：模块间除上下级关系外，**禁止直接调用**，统一通过以下方式通信
> 
> 

1. 在 `PluginConfig` 中通过 `RegisterCall`、`RegisterBus` 注册监听，接收其他模块触发的事件或返回信息

2. `RegisterCall`：可向调用方返回数据，**仅支持单个模块处理同一类事件**

3. `RegisterBus`：**支持多个模块同时处理同一类事件**

4. 调用方式：使用 `BusUtility.Call` 或 `BusUtility.Dispatch` 触发其他模块的监听事件


## 八、预览图片

![输入图片说明](%E5%9B%BE1.png)

![输入图片说明](%E5%9B%BE2.png)

![输入图片说明](%E5%9B%BE3.png)

![输入图片说明](%E5%9B%BE4.png)



## 九、服务支持

<a target="_blank" href="https://qm.qq.com/cgi-bin/qm/qr?k=dHXu51C6p20eWQUhI0OnlCDjnekFpZhq&jump_from=webapi&authKey=hS5MEHqSZdFEk4/4pxxUnYeMvX7sQiL7L0Xi5G5xyEeeSH26K1HyS/I1izCcS+yD"><img border="0" src="//pub.idqqimg.com/wpa/images/group.png" alt="AIOT物联网中台交流群" title="AIOT物联网中台交流群">1046865293</a>

回复：重新读取  就可加群
