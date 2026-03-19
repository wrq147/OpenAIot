# 获取rtmp流地址的接口介绍与示例
## 接口介绍
1. 请求参数：

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | String | 通讯Id |

2. 返回的data类型为键值对，键名为流Id,键值为rtmp播放地址


## 操作示例

GET 请求
```bash
curl /IoTVideoService/HttpSync/GetRtmpPlayUrl?id=dtu323
```

响应示例：
```json
{
  "code": 0,
  "message": "",
  "data": {
    "camera1": "rtmp://192.168.1.100/live/camera1",
    "camera2": "rtmp://192.168.1.100/live/camera2"
  }
}
```