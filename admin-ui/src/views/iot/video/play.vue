<template>
    <el-dialog title="视频播放" :visible.sync="open" width="80%" append-to-body :close-on-click-modal="false"
        @close="handleClose">
        <div class="video-play-container">
            <!-- 视频播放区域 -->
            <div class="video-player" ref="videoContainer" v-loading="loading">
                <video ref="videoElement" controls autoplay muted class="video-content"></video>
            </div>

            <!-- PTZ控制区域 - 仅GB28181设备显示 -->
            <div v-if="videoSource.VideoType === 1" class="ptz-controls">
                <h4>PTZ云台控制</h4>

                <!-- 新增：通道选择下拉框 -->
                <div class="channel-select" style="margin-bottom: 15px; width: 100%;">
                    <el-select v-model="currentChannelId" placeholder="请选择通道" style="width: 100%;"
                        @change="onChannelChange">
                        <el-option v-for="channel in channelList" :key="channel.ChannelId"
                            :label="channel.ChannelName"
                            :value="channel.ChannelId"></el-option>
                    </el-select>
                </div>

                <!-- 方向控制 - 九宫格布局 -->
                <div class="ptz-pad">
                    <!-- 第一行：左上、上、右上 -->
                    <div class="ptz-row">
                        <el-button icon="el-icon-top-left" circle @click="ptzControl(4)">左上</el-button>
                        <el-button icon="el-icon-top" circle @click="ptzControl(3)">上</el-button>
                        <el-button icon="el-icon-top-right" circle @click="ptzControl(2)">右上</el-button>
                    </div>
                    <!-- 第二行：左、停止、右 -->
                    <div class="ptz-row">
                        <el-button icon="el-icon-left" circle @click="ptzControl(5)">左</el-button>
                        <el-button icon="el-icon-refresh-left" circle @click="ptzControl(0)"
                            type="warning">停止</el-button>
                        <el-button icon="el-icon-right" circle @click="ptzControl(1)">右</el-button>
                    </div>
                    <!-- 第三行：左下、下、右下 -->
                    <div class="ptz-row">
                        <el-button icon="el-icon-bottom-left" circle @click="ptzControl(6)">左下</el-button>
                        <el-button icon="el-icon-bottom" circle @click="ptzControl(7)">下</el-button>
                        <el-button icon="el-icon-bottom-right" circle @click="ptzControl(8)">右下</el-button>
                    </div>

                    <!-- 变焦控制 -->
                    <div class="ptz-group">
                        <span class="group-label">变焦</span>
                        <el-button icon="el-icon-zoom-in" circle @click="ptzControl(9)">放大</el-button>
                        <el-button icon="el-icon-zoom-out" circle @click="ptzControl(-9)">缩小</el-button>
                    </div>

                    <!-- 聚焦控制 -->
                    <div class="ptz-group">
                        <span class="group-label">聚焦</span>
                        <el-button icon="el-icon-plus" circle @click="ptzControl(11)">近焦</el-button>
                        <el-button icon="el-icon-minus" circle @click="ptzControl(-11)">远焦</el-button>
                    </div>

                    <!-- 光圈控制 -->
                    <div class="ptz-group">
                        <span class="group-label">光圈</span>
                        <el-button icon="el-icon-plus" circle @click="ptzControl(10)">调大</el-button>
                        <el-button icon="el-icon-minus" circle @click="ptzControl(-10)">调小</el-button>
                    </div>
                </div>

                <!-- 速度调节 -->
                <div class="speed-control">
                    <el-slider v-model="ptzSpeed" :min="1" :max="8" :step="1" show-input width="200px"
                        label="控制速度"></el-slider>
                </div>

                <!-- 预置位控制 -->
                <div class="preset-controls" style="margin-top:20px;border-top:1px solid #eee;padding-top:15px;">
                    <h4>预置位管理</h4>
                    <el-input v-model="presetId" type="number" placeholder="输入预置位ID(1-255)"
                        style="width:120px;margin-right:10px;" :min="1" :max="255"></el-input>
                    <el-button type="primary" size="small" @click="setPreset">设置预置位</el-button>
                    <el-button type="success" size="small" @click="callPreset">调用预置位</el-button>
                    <el-button type="danger" size="small" @click="delPreset" style="margin-top:10px;">删除预置位</el-button>

                    <!-- 已保存预置位列表 -->
                    <div class="preset-list" style="margin-top:15px;">
                        <el-tag v-for="id in presetList" :key="id" closable @close="delPreset(id)" style="margin:5px;">
                            预置位{{ id }}
                        </el-tag>
                    </div>
                </div>
            </div>
        </div>

        <div slot="footer" class="dialog-footer">
            <el-button type="primary" @click="open = false">关闭</el-button>
        </div>
    </el-dialog>
</template>

<script>
import { getPresetList, getPlayUrl, getChannelList, controlPTZ } from "@/api/rules/video";

export default {
    data() {
        return {
            open: false,
            loading: false,
            videoSource: {},
            ptzSpeed: 4,
            presetId: 1,
            presetList: [],
            channelList: [], // 设备下的通道列表
            currentChannelId: '' // 当前选中的通道ID
        }
    },
    methods: {
        showDlg(row) {
            this.open = true;
            this.videoSource = { ...row };
            if (this.videoSource.VideoType == 0) {
                this.getPlayUrl();
            }
            else if (this.videoSource.VideoType == 1) {
                this.loadChannelList();
            }
        },

        async getPlayUrl(sid, cid) {
            try {
                this.loading = true;
                let res = await getPlayUrl(sid, cid);
                this.initVideoPlayer(res.data);
                this.loading = false;
            } catch (error) {
                this.$message.error('获取播放地址失败');
                this.loading = false;
            }
        },

        initVideoPlayer(src) {
            const videoElement = this.$refs.videoElement;
            if (!videoElement) return;
            // 模拟播放地址
            videoElement.src = src;
            videoElement.play().catch(err => console.warn('自动播放失败:', err));
        },

        // 加载设备下的通道列表
        async loadChannelList() {
            try {
                this.loading = true;
                const res = await getChannelList(this.videoSource.Id)
                this.channelList = res.data || [];
                if (this.channelList.length > 0) {
                    this.currentChannelId = this.channelList[0].ChannelId;
                    await this.loadPresetList();
                }
                this.loading = false;
            } catch (error) {
                this.loading = false;
                this.$message.error('加载通道列表失败');
                console.error(error);
            }
        },

        // 新增：通道切换事件
        async onChannelChange() {
            if (!this.currentChannelId) return;
            this.loading = true;
            // 切换通道后重新加载预置位列表
            await this.loadPresetList();
            // 可扩展：切换通道后切换视频播放流
            // this.switchChannelStream();
            this.loading = false;
        },

        // 加载当前通道的预置位列表
        async loadPresetList() {
            try {
                const res = await getPresetList(this.videoSource.Id)
                this.presetList = res.data || [];
            } catch (error) {
                this.$message.error('加载预置位列表失败');
                console.error(error);
            }
        },

        // 云台方向/变焦/聚焦/光圈控制
        async ptzControl(action) {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            try {
                this.loading = true;
                // 实际接口调用示例：传递当前通道ID
                // const res = await controlPTZ({
                //   deviceId: this.videoSource.Id,
                //   channelId: this.currentChannelId,
                //   action: action,
                //   speed: this.ptzSpeed
                // })

            } catch (error) {
                this.$message.error(`云台控制失败`);
                this.loading = false;
            }
        },

        // 设置预置位
        async setPreset() {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            if (!this.presetId || this.presetId < 1 || this.presetId > 255) {
                return this.$message.warning('预置位ID必须为1-255的整数');
            }
            try {
                this.loading = true;
                // 实际接口调用：传递当前通道ID
                // const res = await presetPTZ({
                //   deviceId: this.videoSource.Id,
                //   channelId: this.currentChannelId,
                //   presetId: this.presetId,
                //   action: 'set'
                // })
                setTimeout(() => {
                    if (!this.presetList.includes(this.presetId)) {
                        this.presetList.push(this.presetId);
                        this.presetList.sort((a, b) => a - b);
                    }
                    this.$message.success(`设置预置位${this.presetId}成功`);
                    this.loading = false;
                }, 500);
            } catch (error) {
                this.$message.error(`设置预置位${this.presetId}失败`);
                this.loading = false;
            }
        },

        // 调用预置位
        async callPreset() {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            if (!this.presetId || this.presetId < 1 || this.presetId > 255) {
                return this.$message.warning('预置位ID必须为1-255的整数');
            }
            try {
                this.loading = true;
                // 实际接口调用：传递当前通道ID
                // const res = await presetPTZ({
                //   deviceId: this.videoSource.Id,
                //   channelId: this.currentChannelId,
                //   presetId: this.presetId,
                //   action: 'call'
                // })
                setTimeout(() => {
                    this.$message.success(`调用预置位${this.presetId}成功`);
                    this.loading = false;
                }, 500);
            } catch (error) {
                this.$message.error(`调用预置位${this.presetId}失败`);
                this.loading = false;
            }
        },

        // 删除预置位
        async delPreset(id) {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            const presetId = id || this.presetId;
            if (!presetId || presetId < 1 || presetId > 255) {
                return this.$message.warning('预置位ID必须为1-255的整数');
            }
            try {
                this.loading = true;
                // 实际接口调用：传递当前通道ID
                // const res = await presetPTZ({
                //   deviceId: this.videoSource.Id,
                //   channelId: this.currentChannelId,
                //   presetId: presetId,
                //   action: 'delete'
                // })
                setTimeout(() => {
                    this.presetList = this.presetList.filter(item => item !== presetId);
                    this.$message.success(`删除预置位${presetId}成功`);
                    this.loading = false;
                }, 500);
            } catch (error) {
                this.$message.error(`删除预置位${presetId}失败`);
                this.loading = false;
            }
        },
        handleClose() {
            const videoElement = this.$refs.videoElement;
            if (videoElement) {
                videoElement.pause();
                videoElement.src = '';
            }
            this.open = false;
        }
    }
}
</script>

<style scoped>
.video-play-container {
    display: flex;
    gap: 20px;
    padding: 10px;
}

.video-player {
    flex: 1;
    min-height: 400px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #000;
}

.video-content {
    width: 100%;
    height: 100%;
    object-fit: contain;
}

.ptz-controls {
    width: 300px;
    /* 加宽适配新增控件 */
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 15px;
}

/* 九宫格布局样式 */
.ptz-pad {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
}

/* 每行按钮容器 */
.ptz-row {
    display: flex;
    gap: 8px;
}

/* 按钮样式优化 */
.ptz-row .el-button {
    width: 60px;
    height: 60px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 12px;
}

/* 变焦/聚焦/光圈 分组样式 */
.ptz-group {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-top: 8px;
}

.group-label {
    width: 40px;
    text-align: right;
    font-size: 14px;
    color: #666;
}

.ptz-group .el-button {
    width: 50px;
    height: 50px;
    font-size: 12px;
}

.speed-control {
    margin-top: 15px;
}

@media (max-width: 768px) {
    .video-play-container {
        flex-direction: column;
    }

    .ptz-controls {
        width: 100%;
    }

    .ptz-row {
        justify-content: center;
    }
}
</style>