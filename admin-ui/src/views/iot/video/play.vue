<template>
  <el-dialog
    title="视频播放"
    :visible.sync="open"
    width="80%"
    append-to-body
    :close-on-click-modal="false"
    @close="handleClose"
  >
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
          <el-select
            v-model="currentChannelId"
            placeholder="请选择通道"
            style="width: 100%;"
            @change="onChannelChange"
          >
            <el-option
              v-for="channel in channelList"
              :key="channel.channelId"
              :label="`通道${channel.channelId} - ${channel.channelName}`"
              :value="channel.channelId"
            ></el-option>
          </el-select>
        </div>

        <!-- 方向控制 - 九宫格布局 -->
        <div class="ptz-pad">
          <!-- 第一行：左上、上、右上 -->
          <div class="ptz-row">
            <el-button icon="el-icon-top-left" circle @click="ptzControl('topLeft')">左上</el-button>
            <el-button icon="el-icon-top" circle @click="ptzControl('up')">上</el-button>
            <el-button icon="el-icon-top-right" circle @click="ptzControl('topRight')">右上</el-button>
          </div>
          <!-- 第二行：左、停止、右 -->
          <div class="ptz-row">
            <el-button icon="el-icon-left" circle @click="ptzControl('left')">左</el-button>
            <el-button icon="el-icon-refresh-left" circle @click="ptzControl('stop')" type="warning">停止</el-button>
            <el-button icon="el-icon-right" circle @click="ptzControl('right')">右</el-button>
          </div>
          <!-- 第三行：左下、下、右下 -->
          <div class="ptz-row">
            <el-button icon="el-icon-bottom-left" circle @click="ptzControl('bottomLeft')">左下</el-button>
            <el-button icon="el-icon-bottom" circle @click="ptzControl('down')">下</el-button>
            <el-button icon="el-icon-bottom-right" circle @click="ptzControl('bottomRight')">右下</el-button>
          </div>
          
          <!-- 变焦控制 -->
          <div class="ptz-group">
            <span class="group-label">变焦</span>
            <el-button icon="el-icon-zoom-in" circle @click="ptzControl('zoomIn')">放大</el-button>
            <el-button icon="el-icon-zoom-out" circle @click="ptzControl('zoomOut')">缩小</el-button>
          </div>

          <!-- 聚焦控制 -->
          <div class="ptz-group">
            <span class="group-label">聚焦</span>
            <el-button icon="el-icon-plus" circle @click="ptzControl('focusNear')">近焦</el-button>
            <el-button icon="el-icon-minus" circle @click="ptzControl('focusFar')">远焦</el-button>
          </div>

          <!-- 光圈控制 -->
          <div class="ptz-group">
            <span class="group-label">光圈</span>
            <el-button icon="el-icon-plus" circle @click="ptzControl('irisOpen')">调大</el-button>
            <el-button icon="el-icon-minus" circle @click="ptzControl('irisClose')">调小</el-button>
          </div>
        </div>

        <!-- 速度调节 -->
        <div class="speed-control">
          <el-slider
            v-model="ptzSpeed"
            :min="1"
            :max="8"
            :step="1"
            show-input
            width="200px"
            label="控制速度"
          ></el-slider>
        </div>

        <!-- 预置位控制 -->
        <div class="preset-controls" style="margin-top:20px;border-top:1px solid #eee;padding-top:15px;">
          <h4>预置位管理</h4>
          <el-input
            v-model="presetId"
            type="number"
            placeholder="输入预置位ID(1-255)"
            style="width:120px;margin-right:10px;"
            :min="1"
            :max="255"
          ></el-input>
          <el-button type="primary" size="small" @click="setPreset">设置预置位</el-button>
          <el-button type="success" size="small" @click="callPreset">调用预置位</el-button>
          <el-button type="danger" size="small" @click="delPreset" style="margin-top:10px;">删除预置位</el-button>

          <!-- 已保存预置位列表 -->
          <div class="preset-list" style="margin-top:15px;">
            <el-tag
              v-for="id in presetList"
              :key="id"
              closable
              @close="delPreset(id)"
              style="margin:5px;"
            >
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
// import { controlPTZ, getPlayUrl, presetPTZ, getChannelList, getPresetList } from '@/api/rules/video'

export default {
  data() {
    return {
      open: false,
      loading: false,
      videoSource: {},
      ptzSpeed: 4,
      videoUrl: '',
      presetId: 1, // 默认预置位ID
      presetList: [1, 2, 3], // 模拟已保存的预置位列表

      // 新增：通道相关数据
      channelList: [], // 设备下的通道列表
      currentChannelId: '' // 当前选中的通道ID
    }
  },
  methods: {
    showDlg(row) {
      this.open = true;
      this.videoSource = { ...row };
      this.loading = true;
      this.getPlayUrl();
      // 加载通道列表和当前通道预置位
      this.loadChannelList();
    },

    async getPlayUrl() {
      try {
        setTimeout(() => {
          this.initVideoPlayer();
          this.loading = false;
        }, 1000);
      } catch (error) {
        this.$message.error('获取播放地址失败');
        this.loading = false;
      }
    },

    initVideoPlayer() {
      const videoElement = this.$refs.videoElement;
      if (!videoElement) return;
      // 模拟播放地址
      videoElement.src = 'https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/720/Big_Buck_Bunny_720_10s_1MB.mp4';
      videoElement.play().catch(err => console.warn('自动播放失败:', err));
    },

    // 新增：加载设备下的通道列表
    async loadChannelList() {
      try {
        // 实际项目中调用接口获取通道列表
        // const res = await getChannelList({ deviceId: this.videoSource.Id })
        // this.channelList = res.data || [];

        // 模拟通道列表数据
        this.channelList = [
          { channelId: '34020000001320000001', channelName: '主通道' },
          { channelId: '34020000001320000002', channelName: '子通道1' },
          { channelId: '34020000001320000003', channelName: '子通道2' }
        ];
        // 默认选中第一个通道
        if (this.channelList.length > 0) {
          this.currentChannelId = this.channelList[0].channelId;
          // 加载当前通道预置位
          this.loadPresetList();
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
        // 实际项目中调用接口获取当前通道预置位
        // const res = await getPresetList({ 
        //   deviceId: this.videoSource.Id,
        //   channelId: this.currentChannelId 
        // })
        // this.presetList = res.data || [];

        // 模拟不同通道的预置位数据
        const presetMap = {
          '34020000001320000001': [1, 2, 3],
          '34020000001320000002': [4, 5, 6],
          '34020000001320000003': [7, 8, 9]
        };
        this.presetList = presetMap[this.currentChannelId] || [];
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
  width: 300px; /* 加宽适配新增控件 */
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