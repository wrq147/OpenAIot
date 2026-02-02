<template>
  <div class="history-content" v-loading="loading">
    <div class="history-left">
      <div class="history-player" ref="historyPlayerContainer">
        <div ref="historyPlayer"></div>
      </div>
      <div class="keyframe-list" v-if="keyframeList.length > 0">
        <div class="list-title">关键帧列表</div>
        <div class="keyframe-items">
          <div class="keyframe-item" v-for="(frame, index) in keyframeList" :key="index" @click="jumpToKeyframe(frame)">
            <img :src="frame.url" alt="关键帧" class="keyframe-img">
            <div class="keyframe-time">{{ parseTime(frame.KeyDate, "{h}:{i}:{s}") }}</div>
          </div>
        </div>
      </div>
      <div class="empty-keyframe" v-else>
        暂无关键帧数据，请先选择录像文件
      </div>
    </div>

    <div class="history-right">
      <div class="list-title">录像文件列表</div>
      <div class="filter-bar">
        <div style="margin-bottom: 10px;">
          <span style="font-size: 12px; color: #666;">选择通道：</span>
          <el-select v-model="selectedVideoKey" placeholder="请选择通道" style="width: 280px;" @change="loadRecordFileList">
            <el-option v-for="channel in channelList" :key="channel.VideoKey" :label="channel.ChannelName"
              :value="channel.VideoKey"></el-option>
          </el-select>
        </div>
        <div style="margin-bottom: 10px;">
          <span style="font-size: 12px; color: #666;">筛选日期：</span>
          <el-date-picker v-model="dateRange" type="daterange" range-separator="至" start-placeholder="开始日期"
            @change="loadRecordFileList" end-placeholder="结束日期" format="yyyy-MM-dd" value-format="yyyy-MM-dd"
            style="width: 280px;"></el-date-picker>
        </div>
      </div>

      <el-table :data="recordFileList" border @row-click="selectRecordFile" stripe :highlight-current-row="true"
        style="width: 100%;">
        <el-table-column label="录像日期" align="center" width="100">
          <template slot-scope="scope">
            {{ parseTime(scope.row.FileDate, "{y}-{m}-{d}") }}
          </template>
        </el-table-column>
        <el-table-column label="开始时间" align="center" width="100">
          <template slot-scope="scope">
            {{ parseTime(scope.row.StartTime, "{h}:{i}:{s}") }}
          </template>
        </el-table-column>
        <el-table-column label="结束时间" align="center" width="100">
          <template slot-scope="scope">
            {{ parseTime(scope.row.EndTime, "{h}:{i}:{s}") }}
          </template>
        </el-table-column>
        <el-table-column label="存储方式" align="center">
          <template slot-scope="scope">
            {{ scope.row.StorageWay === 0 ? '文件存储' : '云存储' }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination v-show="totalCount > 0" :total="totalCount" :current-page.sync="currentPage"
        :page-size.sync="pageSize" background small layout="prev, pager, next" @current-change="loadRecordFileList"
        @size-change="loadRecordFileList" />
    </div>
  </div>
</template>

<script>
import Player from 'xgplayer'
import Mp4Plugin from "xgplayer-mp4"
import "xgplayer/dist/index.min.css"
import { getChannelList } from "@/api/rules/video";
import {
  recordFileList, recordKeyList
} from "@/api/rules/record";
export default {
  data() {
    return {
      VideoId: "",
      VideoType: 0,
      historyPlayer: null,      // 历史录像播放器实例
      loading: false,           // 加载状态
      recordFileList: [],       // 录像文件列表
      keyframeList: [],         // 关键帧列表
      currentRecordFile: null,  // 当前选中的录像文件
      // 分页参数
      currentPage: 1,
      pageSize: 10,
      totalCount: 0,
      dateRange: null,
      // 2. 新增：通道相关数据
      channelList: [],          // 通道列表（从接口获取）
      selectedVideoKey: ''      // 当前选中的通道videoKey
    }
  },
  methods: {
    async InitVideo(id, t) {
      this.VideoId = id;
      this.VideoType = t;
      if (this.VideoType == 1) {
        await this.loadChannelList();
      }
      this.currentPage = 1;
      await this.loadRecordFileList();
    },
    async loadChannelList() {
      try {
        const res = await getChannelList(this.VideoId);
        this.channelList = res.data || [];

        // 可选：默认选中第一个通道（若需要）
        if (this.channelList.length > 0 && !this.selectedVideoKey) {
          this.selectedVideoKey = this.channelList[0].VideoKey;
        }
      } catch (error) {
        this.$message.error('加载通道列表失败');
        console.error(error);
        this.channelList = [];
      }
    },

    // 加载录像文件列表（修改：使用选中的selectedVideoKey）
    async loadRecordFileList() {
      this.loading = true;
      try {
        const res = await recordFileList({
          VideoId: this.VideoId,
          VideoKey: this.VideoType == 1 ? this.selectedVideoKey : undefined,
          pageNum: this.currentPage,
          pageSize: this.pageSize,
          beginTime: this.dateRange ? this.dateRange[0] : undefined,
          endTime: this.dateRange ? this.dateRange[1] : undefined
        });

        this.recordFileList = res.data.List || [];
        this.totalCount = res.data.Total || 0;
      } catch (error) {
        this.$message.error('加载录像文件列表失败');
        console.error(error);
      } finally {
        this.loading = false;
      }
    },

    async selectRecordFile(row) {
      if (!row) return;

      this.currentRecordFile = row;
      this.loading = true;

      try {
        // 1. 获取录像播放地址并初始化播放器
        await this.initHistoryPlayer(row);
        // 2. 加载该录像文件的关键帧列表
        await this.loadKeyframeList(row);
      } catch (error) {
        this.$message.error('加载录像文件失败');
        console.error(error);
      } finally {
        this.loading = false;
      }
    },

    // 初始化历史录像播放器（无修改，复用原有逻辑）
    async initHistoryPlayer(recordFile) {
      try {
        if (this.historyPlayer == null) {
          // 创建新的播放器实例
          this.historyPlayer = new Player({
            el: this.$refs.historyPlayer,
            isLive: false,
            autoplay: true,
            url: recordFile.PlayUrl,
            plugins: [Mp4Plugin]
          });
          this.historyPlayer.play();
        }
        else {
          this.historyPlayer.src = recordFile.PlayUrl;
          this.historyPlayer.play();
        }

      } catch (error) {
        throw new Error('初始化录像播放器失败');
      }
    },

    // 加载关键帧列表（无修改，复用原有逻辑）
    async loadKeyframeList(recordFile) {
      try {
        const res = await recordKeyList({
          Key: recordFile.VideoKey,
          beginTime: recordFile.StartTime,
          endTime: recordFile.EndTime
        });

        this.keyframeList = res.data || [];

      } catch (error) {
        this.$message.error('加载关键帧列表失败');
        this.keyframeList = [];
      }
    },

    // 跳转到关键帧时间点
    jumpToKeyframe(frame) {
      if (!this.historyPlayer || !frame || !frame.KeyDate) return;

      // 将关键帧时间戳转换为视频播放的秒数
      const videoStartTime = new Date(this.currentRecordFile.StartTime).getTime();
      const keyTime = new Date(frame.KeyDate).getTime();
      const jumpTime = (keyTime - videoStartTime) / 1000;
      this.historyPlayer.seek(jumpTime);
      this.$message.success(`已跳转到 ${frame.KeyDate}`);
    },

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

.filter-bar {
  display: flex;
  flex-direction: column;
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
  font-weight: 400;
  padding: 20px;
}

.history-right {
  width: 400px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
</style>