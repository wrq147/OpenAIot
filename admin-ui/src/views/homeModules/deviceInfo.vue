<template>
  <div class="device_info">
    <div class="home_module_title">
      <i class="zhongtaiiconfont zhongtai-icon-shebeixinxi"></i>
      <span>设备信息</span>
    </div>
    <div class="device_sum">
      <div class="sum_li" v-for="item in deviceInfo" :key="item.name" @click="deviceInfoClick(item.name)" 
         :style="{ cursor: item.name !== '报警事件' ? 'pointer' : 'default' }">
        <div class="li_left">
          <img
            src="@/assets/images/shebeizongshu.png"
            alt=""
            v-if="item.name == '设备总数'"
          />
          <img
            src="@/assets/images/zaixianshuliang.png"
            alt=""
            v-if="item.name == '在线数量'"
          />
          <img
            src="@/assets/images/lixianshuliang.png"
            alt=""
            v-if="item.name == '离线数量'"
          />
          <img
            src="@/assets/images/baojingshijian.png"
            alt=""
            v-if="item.name == '报警事件'"
          />
        </div>
        <div class="li_right">
          <div class="right_label">{{ item.name }}</div>
          <div class="right_val">{{ item.val }}</div>
        </div>
      </div>
    </div>
    <div class="alarm_table">
      <div class="table_title">
        <div class="title_label">报警工单</div>
        <el-tabs
          v-model="activeName"
          tab-position="top"
          type="card"
          @tab-click="handleClick"
        >
          <el-tab-pane label="报警待处理" name="processing"></el-tab-pane>
          <el-tab-pane label="报警已处理" name="processed"></el-tab-pane>
        </el-tabs>
      </div>
      <el-table
        v-loading="loading"
        :data="alarmList"
        max-height="280"
        class="data_table"
        :header-cell-style="cellSty"
        style="width: 100%"
      >
        <el-table-column
          label="设备ID"
          align="center"
          key="DeviceId"
          prop="DeviceId"
        />
        <el-table-column
          label="设备名称"
          align="center"
          key="DeviceName"
          prop="DeviceName"
        />
        <el-table-column
          label="告警名称"
          align="center"
          key="Name"
          prop="Name"
        />
        <el-table-column
          label="事件标识"
          align="center"
          key="Code"
          prop="Code"
        />
        <el-table-column
          label="报警级别"
          align="center"
          key="Level"
          prop="Level"
        >
          <template slot-scope="scope">
            <div>
              <!-- {{scope.row.Level==0?'普通':(scope.row.Level==1?'告警':'紧急')}} -->
              <el-tag type="info" v-if="scope.row.Level == 0">普通</el-tag>
              <el-tag type="warning" v-if="scope.row.Level == 1">告警</el-tag>
              <el-tag type="danger" v-if="scope.row.Level == 2">紧急</el-tag>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="告警时间" align="center" prop="CreateOn">
          <template slot-scope="scope">
            <span>{{ parseTime(scope.row.CreateOn) }}</span>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { deviceStatistics, warningList } from "@/api/home.js";
export default {
  name: "deviceInfo",
  mixins: [resizeTableCon],
  data() {
    return {
      loading: false,
      activeName: "processing",
      alarmList: [],
      deviceInfo: [
        {
          name: "设备总数",
          val: '',
        },
        {
          name: "在线数量",
          val: '',
        },
        {
          name: "离线数量",
          val: '',
        },
        {
          name: "报警事件",
          val: '',
        },
      ],
      warnQuery: {
        pageNum: 1,
        pageSize: 30,
      },
    };
  },
  mounted() {
    this.getDeviceStatistics();
    this.getWarningList();
  },
  methods: {
    getDeviceStatistics() {
      //获取设备统计信息
      deviceStatistics().then((res) => {
        // console.log("设备统计信息res", res);
        let data = res.data;
        this.deviceInfo = [
          {
            name: "设备总数",
            val: data.TotalCount,
          },
          {
            name: "在线数量",
            val: data.OnlineCount,
          },
          {
            name: "离线数量",
            val: data.OfflineCount,
          },
          {
            name: "报警事件",
            val: data.EventCount,
          },
        ];
      });
    },
    handleClick() {
      //设备信息的报警列表的切换
      // this.warnQuery.pageNum = 1;
      this.getWarningList();
    },
    getWarningList() {
      this.warnQuery.Status = 0;
      if (this.activeName == "processing") {
        this.warnQuery.Status = 0;
      } else if (this.activeName == "processed") {
        this.warnQuery.Status = 1;
      }
      // if (this.warnQuery.pageNum == 1) {
      //   this.alarmList = [];
      // }
      warningList(this.warnQuery).then((res) => {
        this.alarmList = res.data.List;
      });
    },
    deviceInfoClick(info) {
      //设备信息的统计信息的点击事件
      if(info === '设备总数'){
        this.$router.push({ path: '/after/dev/list' })
      }else if(info == '在线数量'){
        this.$router.push({ path: '/after/dev/list', query: { id: 1, name: '在线' } })
      }else if(info == '离线数量'){
        this.$router.push({ path: '/after/dev/list', query: { id: 0, name: '离线' } })
      }
    }
  },
};
</script>

<style lang="scss" scoped>
.device_info {
  width: calc(50% - 10px);
  margin-right: 20px;
  // height: 100%;
  margin-bottom: 30px;

  .device_sum {
    width: 100%;
    display: flex;
    justify-content: space-between;
    margin-bottom: 12px;

    .sum_li {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      width: calc(25% - 9px); //(100%-36)/4
      height: 72px;
      padding: 16px;
      box-sizing: border-box;
      background: #ffffff;
      font-size: 4px;

      .li_left {
        width: 40px;
        height: 40px;
        margin-right: 16px;

        img {
          width: 40px;
          height: 40px;
        }
      }

      .li_right {
        .right_label {
          font-size: 14px;
          color: #78829d;
          margin-bottom: 8px;
        }

        .right_val {
          color: #333333;
          font-size: 18px;
          font-weight: 550;
        }
      }
    }
  }

  .alarm_table {
    background: #ffffff;
    padding: 16px 20px 10px;
    border-radius: 4px;
    height: 394px;
    box-sizing: border-box;

    .table_title {
      position: relative;

      .title_label {
        position: absolute;
        left: 0;
        bottom: 15px;
        font-size: 16px;
        color: #333333;
        font-weight: 550;
      }
    }
  }
}
</style>
<style lang="less">
.home_module_title {
  font-size: 14px;
  color: #333333;
  margin-bottom: 12px;

  span {
    margin-left: 6px;
  }
}

.alarm_table {
  background: #ffffff;

  .table_title {
    .el-tabs .el-tabs__header.is-top .el-tabs__nav-scroll {
      display: flex;
      justify-content: flex-end;
    }

    .el-tabs .el-tabs__header.is-top {
      margin-bottom: 0;
    }
  }

  .data_table {
    .el-table__body-wrapper {
      // background-color: #ddd;
      &::-webkit-scrollbar {
        width: 6px;
        height: 6px;
      }

      // 滚动条里面默认的小方块,自定义样式
      &::-webkit-scrollbar-thumb {
        background: #efefef;
        border-radius: 3px;
      }

      // 滚动条里面的轨道
      &::-webkit-scrollbar-track {
        background: transparent;
      }
    }

    // .el-table__body-wrapper::-webkit-scrollbar {
    //   width: 8px !important;
    //   height: 8px !important;
    // }
  }
}
</style>
