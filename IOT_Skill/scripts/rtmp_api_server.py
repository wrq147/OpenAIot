import cv2
import base64
import time
import threading
import uvicorn
from fastapi import FastAPI, HTTPException, Body  # 仅保留Body用于接收参数
from typing import Dict, Any, Optional
import psutil
import os

# ========== 全局配置 ==========
RTMP_CONFIG = {
    "buffer_size": 1,          # 最小缓存（实时性优先）
    "fps": 10,                 # 每秒拉10帧
    "frame_quality": 80,       # JPG压缩质量
    "reconnect_interval": 5,   # 重连间隔（秒）
}

# ========== 全局状态管理（线程安全） ==========
STREAM_CACHE: Dict[str, Dict[str, Any]] = {}
CACHE_LOCK = threading.Lock()
app = FastAPI(title="RTMP监控API服务", version="1.0")

# ========== 核心监控逻辑（无修改） ==========
def rtmp_monitor_worker(stream_id: str, rtmp_url: str):
    """后台拉流线程：内存缓存最新帧"""
    cap: Optional[cv2.VideoCapture] = None
    reconnect_count = 0

    while True:
        # 检查是否需要停止
        with CACHE_LOCK:
            if not STREAM_CACHE.get(stream_id, {}).get("is_running", False):
                break

        try:
            # 初始化/重连RTMP流
            if cap is None or not cap.isOpened():
                cap = cv2.VideoCapture(rtmp_url, cv2.CAP_FFMPEG)
                cap.set(cv2.CAP_PROP_BUFFERSIZE, RTMP_CONFIG["buffer_size"])
                cap.set(cv2.CAP_PROP_FPS, RTMP_CONFIG["fps"])
                reconnect_count += 1

                # 多次重连失败则暂停
                if reconnect_count > 3:
                    with CACHE_LOCK:
                        STREAM_CACHE[stream_id]["error"] = f"多次重连失败，{RTMP_CONFIG['reconnect_interval']}秒后重试"
                    time.sleep(RTMP_CONFIG["reconnect_interval"])
                    reconnect_count = 0
                    continue

            # 读取最新帧
            ret, frame = cap.read()
            if ret and frame is not None:
                with CACHE_LOCK:
                    STREAM_CACHE[stream_id]["frame"] = frame.copy()  # 深拷贝避免覆盖
                    STREAM_CACHE[stream_id]["error"] = ""
                reconnect_count = 0
            else:
                with CACHE_LOCK:
                    STREAM_CACHE[stream_id]["error"] = "无法读取视频帧"
                if cap is not None:
                    cap.release()
                cap = None
                time.sleep(0.1)

            # 控制拉流频率
            time.sleep(1 / RTMP_CONFIG["fps"])

        except Exception as e:
            error_msg = f"监控异常：{str(e)}"
            with CACHE_LOCK:
                STREAM_CACHE[stream_id]["error"] = error_msg
            if cap is not None:
                cap.release()
            cap = None
            time.sleep(RTMP_CONFIG["reconnect_interval"])

    # 退出清理
    if cap is not None:
        cap.release()
    with CACHE_LOCK:
        if stream_id in STREAM_CACHE:
            STREAM_CACHE[stream_id]["is_running"] = False
            STREAM_CACHE[stream_id]["frame"] = None
    print(f"✅ {stream_id} 监控已停止")

# ========== API接口（改用Body接收参数，移除Pydantic模型） ==========
@app.post("/api/start-monitor", summary="启动RTMP监控")
async def start_monitor(
    stream_id: str = Body(..., description="流唯一标识"),
    rtmp_url: str = Body(..., description="RTMP流地址")
):
    """启动指定流的后台监控（无Pydantic，直接用Body接收参数）"""
    with CACHE_LOCK:
        # 检查是否已在运行
        if stream_id in STREAM_CACHE and STREAM_CACHE[stream_id]["is_running"]:
            raise HTTPException(status_code=400, detail=f"{stream_id} 已在监控中")

        # 初始化缓存
        STREAM_CACHE[stream_id] = {
            "frame": None,
            "is_running": True,
            "error": ""
        }

    # 启动后台拉流线程
    monitor_thread = threading.Thread(
        target=rtmp_monitor_worker,
        args=(stream_id, rtmp_url),
        daemon=True
    )
    monitor_thread.start()

    # 等待1秒确保线程启动
    time.sleep(1)

    return {
        "code": 0,
        "message": f"{stream_id} 监控启动成功",
        "data": {
            "stream_id": stream_id,
            "rtmp_url": rtmp_url,
            "status": "running"
        }
    }

@app.post("/api/stop-monitor", summary="停止RTMP监控")
async def stop_monitor(
    stream_id: str = Body(..., description="流唯一标识")
):
    """停止指定流的后台监控"""
    with CACHE_LOCK:
        if stream_id not in STREAM_CACHE:
            raise HTTPException(status_code=404, detail=f"{stream_id} 未启动监控")
        if not STREAM_CACHE[stream_id]["is_running"]:
            raise HTTPException(status_code=400, detail=f"{stream_id} 监控已停止")

        # 标记为停止（线程会自动退出）
        STREAM_CACHE[stream_id]["is_running"] = False

    return {
        "code": 0,
        "message": f"{stream_id} 监控停止成功",
        "data": {
            "stream_id": stream_id,
            "status": "stopped"
        }
    }

@app.post("/api/get-frame", summary="获取最新画面")
async def get_frame(
    stream_id: str = Body(..., description="流唯一标识")
):
    """从内存缓存获取最新帧（Base64格式）"""
    with CACHE_LOCK:
        if stream_id not in STREAM_CACHE:
            raise HTTPException(status_code=404, detail=f"{stream_id} 未启动监控")
        stream_info = STREAM_CACHE[stream_id]

        if not stream_info["is_running"]:
            raise HTTPException(status_code=400, detail=f"{stream_id} 监控已停止")

        if stream_info["frame"] is None:
            raise HTTPException(status_code=500, detail=stream_info["error"] or "暂无画面缓存")

        # 内存帧转Base64
        frame = stream_info["frame"]
        _, buffer = cv2.imencode('.jpg', frame, [cv2.IMWRITE_JPEG_QUALITY, RTMP_CONFIG["frame_quality"]])
        jpg_base64 = base64.b64encode(buffer).decode('utf-8')

    return {
        "code": 0,
        "message": "获取画面成功",
        "data": {
            "stream_id": stream_id,
            "snapshot_base64": f"data:image/jpeg;base64,{jpg_base64}",
            "error": ""
        }
    }

@app.get("/api/list-monitor", summary="查看所有监控流状态")
async def list_monitor():
    """查看当前所有监控流的状态"""
    with CACHE_LOCK:
        result = []
        for stream_id, info in STREAM_CACHE.items():
            result.append({
                "stream_id": stream_id,
                "is_running": info["is_running"],
                "has_frame": info["frame"] is not None,
                "error": info["error"]
            })
    return {
        "code": 0,
        "message": "查询成功",
        "data": result
    }

# ========== 启动服务 ==========
def start_api_server(host: str = "0.0.0.0", port: int = 8111):
    """启动FastAPI服务（常驻）"""
    # 先清理僵尸进程（可选）
    for proc in psutil.process_iter(['pid', 'name', 'cmdline']):
        try:
            if "rtmp_api_server.py" in proc.cmdline() and proc.pid != os.getpid():
                proc.terminate()
        except:
            pass

    # 启动UVicorn服务
    uvicorn.run(
        app,
        host=host,
        port=port,
        log_level="info"
    )

if __name__ == "__main__":
    # 启动API服务（默认端口8111）
    start_api_server(host="0.0.0.0", port=8111)