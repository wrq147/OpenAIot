<template>
    <el-dialog title="视频播放" :visible.sync="open" width="1024px" top="2vh" append-to-body :close-on-click-modal="false"
        @close="handleClose">
        <div class="video-play-container">
            <!-- 页签切换 -->
            <el-tabs v-model="activeTab" type="card" @tab-click="changeTab">
                <!-- 实时播放页签 -->
                <el-tab-pane label="实时播放" name="realTime">
                    <div class="real-time-content">
                        <!-- 视频播放区域 -->
                        <div class="video-player" ref="videoContainer" v-loading="loading">
                            <div ref="devPlayer"></div>
                            <!-- 预置位控制 -->
                            <div class="preset-controls"
                                style="margin-top:20px;border-top:1px solid #eee;padding-top:5px;">
                                <h4>预置位管理</h4>
                                <!-- 仅保留设置预置位功能 -->
                                <el-input v-model="presetId" type="number" placeholder="输入预置位ID(1-255)"
                                    style="width:120px;margin-right:10px;" :min="1" :max="255"></el-input>
                                <el-button type="primary" size="small" @click="setPreset">设置预置位</el-button>

                                <!-- 已保存预置位列表 - 改造：点击标签调用，关闭按钮删除 -->
                                <div class="preset-list" style="margin-top:15px;">
                                    <el-tag v-for="id in presetList" :key="id" closable @close="delPreset(id)"
                                        @click="callPreset(id)" style="margin:5px; cursor: pointer;" effect="dark">
                                        预置位{{ id }}
                                    </el-tag>
                                    <div v-if="presetList.length === 0"
                                        style="color:#999; font-size:12px; margin-top:8px;">
                                        暂无预置位，可输入ID后点击"设置预置位"添加
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- PTZ控制区域 -->
                        <div v-if="videoSource.VideoType === 1 || videoSource.VideoType === 3" class="ptz-controls">
                            <div style="line-height: 24px;font-size: 14px;">PTZ云台控制</div>

                            <!-- 新增：通道选择下拉框 -->
                            <div class="channel-select" style="margin-bottom: 15px; width: 100%;">
                                <el-select v-model="currentChannelId" placeholder="请选择通道" style="width: 100%;"
                                    @change="onChannelChange">
                                    <el-option v-for="channel in channelList" :key="channel.VideoKey"
                                        :label="channel.ChannelName" :value="channel.VideoKey"></el-option>
                                </el-select>
                            </div>

                            <!-- 方向控制 - 九宫格布局 -->
                            <div class="ptz-pad">
                                <!-- 第一行：左上、上、右上 -->
                                <div class="ptz-row">
                                    <el-button circle @click="ptzControl(4)">左上</el-button>
                                    <el-button circle @click="ptzControl(3)">上</el-button>
                                    <el-button circle @click="ptzControl(2)">右上</el-button>
                                </div>
                                <!-- 第二行：左、停止、右 -->
                                <div class="ptz-row">
                                    <el-button circle @click="ptzControl(5)">左</el-button>
                                    <el-button circle @click="ptzControl(0)" type="warning">停止</el-button>
                                    <el-button circle @click="ptzControl(1)">右</el-button>
                                </div>
                                <!-- 第三行：左下、下、右下 -->
                                <div class="ptz-row">
                                    <el-button circle @click="ptzControl(6)">左下</el-button>
                                    <el-button circle @click="ptzControl(7)">下</el-button>
                                    <el-button circle @click="ptzControl(8)">右下</el-button>
                                </div>

                                <!-- 变焦控制 -->
                                <div class="ptz-group">
                                    <span class="group-label">变焦</span>
                                    <el-button circle @click="ptzControl(9)">放大</el-button>
                                    <el-button circle @click="ptzControl(-9)">缩小</el-button>
                                </div>

                                <!-- 聚焦控制 -->
                                <div class="ptz-group">
                                    <span class="group-label">聚焦</span>
                                    <el-button circle @click="ptzControl(11)">近焦</el-button>
                                    <el-button circle @click="ptzControl(-11)">远焦</el-button>
                                </div>

                                <!-- 光圈控制 -->
                                <div class="ptz-group">
                                    <span class="group-label">光圈</span>
                                    <el-button circle @click="ptzControl(10)">调大</el-button>
                                    <el-button circle @click="ptzControl(-10)">调小</el-button>
                                </div>
                            </div>

                            <!-- 速度调节 -->
                            <div class="speed-control">
                                <span>速度</span>
                                <el-slider v-model="ptzSpeed" :min="1" :max="8" :step="1" width="200px"
                                    label="控制速度"></el-slider>
                            </div>


                        </div>

                    </div>
                </el-tab-pane>
                <el-tab-pane label="历史录像" name="history">
                    <historyPlayer ref="hisPlayer" />
                </el-tab-pane>
            </el-tabs>
        </div>

        <div slot="footer" class="dialog-footer">
            <el-button type="primary" @click="open = false">关闭</el-button>
        </div>
    </el-dialog>
</template>

<script>
import { getPresetList, getPlayUrl, getChannelList, controlPTZ } from "@/api/rules/video";
import Player from 'xgplayer'
import FlvPlugin from 'xgplayer-flv'
import "xgplayer/dist/index.min.css"
import historyPlayer from './historyPlayer.vue';
export default {
    components: {
        historyPlayer
    },
    data() {
        return {
            activeTab: "realTime",
            tmpplayer: null,
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
        changeTab(item) {
            if (item.name == "history") {
                this.$refs.hisPlayer.InitVideo(this.videoSource.Id, this.videoSource.VideoType);
            }
        },
        async showDlg(row) {
            this.activeTab = "realTime";
            this.loading = true;
            this.open = true;
            this.videoSource = { ...row };
            if (this.videoSource.VideoType == 0) {
                await this.initVideo(this.videoSource.Id, null);
            }
            else {
                await this.loadChannelList();
            }
            this.loading = false;
        },
        async initVideo(sid, cid) {
            try {
                if (this.tmpplayer == null) {
                    let res = await getPlayUrl(sid, cid, "flv");
                    this.tmpplayer = new Player({
                        el: this.$refs.devPlayer,
                        isLive: true,
                        url: res.data,
                        autoplay: true,
                        plugins: [FlvPlugin]
                    })
                    this.tmpplayer.play();
                }
                else {
                    let res = await getPlayUrl(sid, cid, "flv");
                    this.tmpplayer.src = res.data;
                    this.tmpplayer.play();
                }
            } catch (error) {
                this.$message.error('获取播放地址失败');
            }
        },
        // 加载设备下的通道列表
        async loadChannelList() {
            try {
                const res = await getChannelList(this.videoSource.Id)
                this.channelList = res.data || [];
                if (this.channelList.length > 0) {
                    this.currentChannelId = this.channelList[0].VideoKey;
                    await this.loadPresetList();
                    await this.initVideo(this.videoSource.Id, this.currentChannelId);
                }
            } catch (error) {
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
                await controlPTZ({
                    SourceId: this.videoSource.Id,
                    VideoKey: this.currentChannelId,
                    Cmd: action,
                    Speed: this.ptzSpeed
                });
            } catch (error) {
                this.$message.error(`云台控制失败`);
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
                await presetPTZ({
                    SourceId: this.videoSource.Id,
                    VideoKey: this.currentChannelId,
                    Cmd: 12,
                    PresetId: this.presetId
                })

            } catch (error) {
                this.$message.error(`设置预置位${this.presetId}失败`);
            }
            await this.loadPresetList();
        },
        // 调用预置位
        async callPreset(id) {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            const presetId = id;
            if (!presetId || presetId < 1 || presetId > 255) {
                return this.$message.warning('预置位ID无效');
            }
            try {
                await presetPTZ({
                    SourceId: this.videoSource.Id,
                    VideoKey: this.currentChannelId,
                    Cmd: 13,
                    PresetId: presetId
                });

            } catch (error) {
                this.$message.error(`调用预置位${presetId}失败`);
            }
        },

        // 删除预置位 - 保持原有逻辑，优化提示
        async delPreset(id) {
            if (!this.currentChannelId) {
                return this.$message.warning('请先选择通道');
            }
            const presetId = id;
            if (!presetId || presetId < 1 || presetId > 255) {
                return this.$message.warning('预置位ID无效');
            }
            try {
                await presetPTZ({
                    SourceId: this.videoSource.Id,
                    VideoKey: this.currentChannelId,
                    Cmd: 14,
                    PresetId: presetId
                });
            } catch (error) {
                this.$message.error(`删除预置位${presetId}失败`);
            }

            await this.loadPresetList();
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
::v-deep .el-dialog__body {
    padding-top: 10px;
}

.video-play-container {
    width: 100%;
    height: 100%;
}

.real-time-content {
    display: flex;
    gap: 20px;
    padding: 10px;
}

.video-player {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
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
    gap: 10px;
}

/* 每行按钮容器 */
.ptz-row {
    display: flex;
    gap: 8px;
}

/* 按钮样式优化 */
.ptz-row .el-button {
    width: 50px;
    height: 50px;
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
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 12px;
}

.speed-control {
    margin-top: 5px;
    padding: 0 15px;
    width: 100%;
}
</style>