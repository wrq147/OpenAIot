<template>
  <div class="history-content" v-loading="loading">
    <!-- 左侧：播放器 + 关键帧 -->
    <div class="history-left">
      <!-- 录像播放器 -->
      <div class="history-player" ref="historyPlayerContainer">
        <div ref="historyPlayer"></div>
      </div>

      <!-- 关键帧列表 -->
      <div class="keyframe-list" v-if="keyframeList.length > 0">
        <div class="list-title">关键帧列表</div>
        <div class="keyframe-items">
          <div 
            class="keyframe-item" 
            v-for="(frame, index) in keyframeList" 
            :key="index"
            @click="jumpToKeyframe(frame)"
          >
            <img :src="frame.url" alt="关键帧" class="keyframe-img">
            <div class="keyframe-time">{{ formatTime(frame.time) }}</div>
          </div>
        </div>
      </div>
      <div class="empty-keyframe" v-else>
        暂无关键帧数据，请先选择录像文件
      </div>
    </div>

    <!-- 右侧：录像文件列表 -->
    <div class="history-right">
      <div class="list-title">录像文件列表</div>
      <el-table 
        :data="recordFileList" 
        border 
        stripe 
        @row-click="selectRecordFile"
        :highlight-current-row="true"
        style="width: 100%;"
      >
        <el-table-column 
          prop="FileDate" 
          label="录像日期" 
          width="180"
          :formatter="formatTableDate"
        ></el-table-column>
        <el-table-column 
          prop="StartTime" 
          label="开始时间" 
          width="180"
          :formatter="formatTableDateTime"
        ></el-table-column>
        <el-table-column 
          prop="EndTime" 
          label="结束时间" 
          width="180"
          :formatter="formatTableDateTime"
        ></el-table-column>
        <el-table-column 
          prop="StorageWay" 
          label="存储方式" 
          width="100"
        >
          <template slot-scope="scope">
            {{ scope.row.StorageWay === 0 ? '文件存储' : '云存储' }}
          </template>
        </el-table-column>
        <el-table-column 
          prop="Status" 
          label="状态" 
          width="100"
        >
          <template slot-scope="scope">
            {{ scope.row.Status === 0 ? '已结束' : '录像中' }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[10, 20, 50, 100]"
        :page-size="pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="totalCount"
        style="margin-top: 15px; text-align: right;"
      >
      </el-pagination>
    </div>
  </div>
</template>

<script>
import Player from 'xgplayer'
import "xgplayer/dist/index.min.css"
import { 
  getRecordFileList,
  getRecordPlayUrl,
  getKeyframeList
} from "@/api/rules/video";

export default {
  name: 'VideoHistoryPlayer',
  props: {
    // 视频源ID（必传）
    videoId: {
      type: String,
      required: true
    },
    // 通道Key（可选）
    videoKey: {
      type: String,
      default: ''
    },
    // 初始加载数据（可选）
    initLoad: {
      type: Boolean,
      default: true
    }
  },
  data() {
    return {
      historyPlayer: null,      // 历史录像播放器实例
      loading: false,           // 加载状态
      recordFileList: [],       // 录像文件列表
      keyframeList: [],         // 关键帧列表
      currentRecordFile: null,  // 当前选中的录像文件
      // 分页参数
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
    }
  },
  watch: {
    // 监听videoId变化，重新加载数据
    videoId: {
      immediate: true,
      handler() {
        if (this.initLoad) {
          this.loadRecordFileList();
        }
      }
    },
    // 监听videoKey变化
    videoKey() {
      this.currentPage = 1;
      this.loadRecordFileList();
    }
  },
  beforeDestroy() {
    // 组件销毁时销毁播放器
    this.destroyPlayer();
  },
  methods: {
    // 销毁播放器实例
    destroyPlayer() {
      if (this.historyPlayer) {
        this.historyPlayer.destroy();
        this.historyPlayer = null;
      }
    },

    // 加载录像文件列表
    async loadRecordFileList() {
      this.loading = true;
      try {
        const res = await getRecordFileList({
          VideoId: this.videoId,
          VideoKey: this.videoKey,
          page: this.currentPage,
          size: this.pageSize
        });
        
        this.recordFileList = res.data?.records || [];
        this.totalCount = res.data?.total || 0;
      } catch (error) {
        this.$message.error('加载录像文件列表失败');
        console.error(error);
      } finally {
        this.loading = false;
      }
    },

    // 选择录像文件
    async selectRecordFile(row) {
      if (!row) return;
      
      this.currentRecordFile = row;
      this.loading = true;
      
      try {
        // 1. 获取录像播放地址并初始化播放器
        await this.initHistoryPlayer(row);
        
        // 2. 加载该录像文件的关键帧列表
        await this.loadKeyframeList(row);
        
        // 触发选中事件，供父组件监听
        this.$emit('file-selected', row);
      } catch (error) {
        this.$message.error('加载录像文件失败');
        console.error(error);
      } finally {
        this.loading = false;
      }
    },

    // 初始化历史录像播放器
    async initHistoryPlayer(recordFile) {
      try {
        // 先销毁旧播放器
        this.destroyPlayer();

        // 获取录像播放地址
        const res = await getRecordPlayUrl({
          FileId: recordFile.Id,
          VideoId: recordFile.VideoId,
          VideoKey: recordFile.VideoKey
        });

        // 创建新的播放器实例
        const playerConfig = {
          el: this.$refs.historyPlayer,
          isLive: false,  // 非直播
          autoplay: true,
          url: res.data,
          plugins: []
        };

        this.historyPlayer = new Player(playerConfig);
        
        // 触发播放器初始化事件
        this.$emit('player-init', this.historyPlayer);
      } catch (error) {
        throw new Error('初始化录像播放器失败');
      }
    },

    // 加载关键帧列表
    async loadKeyframeList(recordFile) {
      try {
        const res = await getKeyframeList({
          FileId: recordFile.Id,
          VideoId: recordFile.VideoId
        });
        
        // 关键帧数据格式：[{ url: 'xxx.jpg', time: 1735689600000 }, ...]
        this.keyframeList = res.data || [];
        
        // 触发关键帧加载完成事件
        this.$emit('keyframes-loaded', this.keyframeList);
      } catch (error) {
        this.$message.error('加载关键帧列表失败');
        console.error(error);
        this.keyframeList = [];
      }
    },

    // 跳转到关键帧时间点
    jumpToKeyframe(frame) {
      if (!this.historyPlayer || !frame || !frame.time) return;
      
      // 将关键帧时间戳转换为视频播放的秒数
      const videoStartTime = new Date(this.currentRecordFile.StartTime).getTime();
      const jumpTime = (frame.time - videoStartTime) / 1000;
      
      // 设置视频播放位置
      this.historyPlayer.currentTime = jumpTime;
      this.historyPlayer.play();
      
      // 触发关键帧跳转事件
      this.$emit('keyframe-jump', frame, jumpTime);
      this.$message.success(`已跳转到 ${this.formatTime(frame.time)}`);
    },

    // 分页大小改变
    handleSizeChange(val) {
      this.pageSize = val;
      this.loadRecordFileList();
    },

    // 当前页改变
    handleCurrentChange(val) {
      this.currentPage = val;
      this.loadRecordFileList();
    },

    // 格式化日期（仅日期）
    formatTableDate(row) {
      return row.FileDate ? this.$moment(row.FileDate).format('YYYY-MM-DD') : '-';
    },

    // 格式化日期时间
    formatTableDateTime(row, column) {
      return row[column.prop] ? this.$moment(row[column.prop]).format('YYYY-MM-DD HH:mm:ss') : '-';
    },

    // 格式化时间（用于关键帧）
    formatTime(timestamp) {
      return this.$moment(timestamp).format('HH:mm:ss');
    },

    // 外部调用：手动刷新列表
    refreshList() {
      this.currentPage = 1;
      this.loadRecordFileList();
    },

    // 外部调用：销毁播放器
    destroy() {
      this.destroyPlayer();
    }
  }
}
</script>

<style scoped>
.history-content {
  display: flex;
  gap: 20px;
  height: 100%;
  width: 100%;
  padding: 10px 0;
}

.history-left {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.history-player {
  width: 100%;
  height: 400px;
  background: #000;
  border-radius: 4px;
}

.keyframe-list {
  width: 100%;
}

.list-title {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 10px;
}

.keyframe-items {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  max-height: 150px;
  overflow-y: auto;
  padding: 5px;
  border: 1px solid #eee;
  border-radius: 4px;
}

.keyframe-item {
  width: 120px;
  cursor: pointer;
  border: 2px solid transparent;
  border-radius: 4px;
  transition: all 0.2s;
}

.keyframe-item:hover {
  border-color: #409eff;
}

.keyframe-img {
  width: 100%;
  height: 80px;
  object-fit: cover;
  border-radius: 4px;
}

.keyframe-time {
  text-align: center;
  font-size: 12px;
  color: #666;
  margin-top: 5px;
}

.empty-keyframe {
  text-align: center;
  color: #999;
  font-size: 14px;
  padding: 20px;
}

.history-right {
  width: 400px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

</style>