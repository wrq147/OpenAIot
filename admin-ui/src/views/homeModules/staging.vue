<template>
  <div class="staging">
    <div class="home_module_title">
      <i class="zhongtaiiconfont zhongtai-icon-gongzuotai"></i>
      <span>工作台</span>
    </div>
    <div class="working_nav">
      <div
        class="working_nav_li"
        v-for="row in totalInfo"
        :key="row.name"
        :class="{ no_list: !row.list }"
        @click.stop="jumpFlowList(row)"
      >
        <div class="nav_li_left">
          <div class="li_left_label">
            <div class="icons">
              <i class="zhongtaiiconfont" :class="'zhongtai-icon-'+row.iconName"></i>
            </div>
            <span>{{ row.name }}</span>
          </div>
          <div class="li_left_value">{{ row.totalNum }}</div>
        </div>
        <div class="nav_li_right" v-if="row.list">
          <div class="right_li" v-for="li in row.list" :key="li.name">
            <div class="right_li_label">{{ li.name }}：</div>
            <div class="right_li_val no_start" :class="li.className">
              {{ li.num }}
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="alarm_table" v-if="isCheckPermi(['/FlowService/Task/WaitList'])||isCheckPermi(['/FlowService/Task/List'])">
      <div class="table_title">
        <div class="title_label">我的任务</div>
        <el-tabs
          v-model="activeName"
          tab-position="top"
          type="card"
          @tab-click="handleClick"
        >
          <el-tab-pane label="待处理" name="working" v-if="isCheckPermi(['/FlowService/Task/WaitList'])"></el-tab-pane>
          <el-tab-pane label="已处理" name="finished" v-if="isCheckPermi(['/FlowService/Task/List'])"></el-tab-pane>
        </el-tabs>
      </div>
      <div
        class="working_ul"
        style="overflow: auto"
        v-infinite-scroll="loadworkingList"
        :infinite-scroll-disabled="loadStatus == 'noMore'"
        v-if="workingList.length > 0"
      >
        <div
          class="working_li"
          v-for="(item, index) in workingList"
          :key="index"
        >
          <div class="work_name">
            <div class="icon_con">
              <i class="zhongtaiiconfont zhongtai-icon-a-wodeliuchengqianse"></i>
            </div>
            <div class="name">{{ item.FlowName }}</div>
          </div>
          <!-- <div class="remark">{{ item.remark }}</div> -->
          <div class="time">{{ item.StartTime }}</div>
        </div>
      </div>
      <div class="empty" v-else>暂无数据</div>
    </div>
  </div>
</template>
  
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { followStatisticsInfo } from "@/api/home.js";
import { todoList } from "@/api/flowable/process.js";
import { finishedList } from "@/api/flowable/process.js";
import { checkPermi } from '@/utils/permission.js';
import dayjs from "dayjs";

export default {
  name: "staging",
  mixins: [resizeTableCon],
  data() {
    return {
      activeName: "working",
      totalInfo: [],
      workingList: [
        //任务列表
      ],
      queryParams: {
        pageNum: 1,
        pageSize: 10,
      },
      loadStatus: "loading",
    };
  },
  mounted() {
    this.getFollowStatisticsInfo();
    this.queryParams.pageNum = 1;
    this.workingList = [];
    this.refreshWorkingList();
  },
  methods: {
    jumpFlowList(row) {
      //跳转至不同的流程
      if (row.type == "pending") {
        if(this.isCheckPermi(['/FlowService/Task/WaitList'])){
          this.$router.push("/flowable/todo");
        }
      }
      if (row.type == "copy") {
        if(this.isCheckPermi(['/FlowService/Task/CCList'])){
          this.$router.push("/flowable/cclist");
        }
      }
      if (row.type == "finish") {
        if(this.isCheckPermi(['/FlowService/Task/List'])){
          this.$router.push("/flowable/finished");
        }
      }
    },
    isCheckPermi(val) {
      return checkPermi(val)
    },
    refreshWorkingList() {
      if (this.isCheckPermi(['/FlowService/Task/WaitList'])&&this.activeName == "working") {
        this.getTodoList();
      } else if (this.isCheckPermi(['/FlowService/Task/List'])&&this.activeName == "finished") {
        this.getFinishedList();
      }
    },
    loadworkingList() {
      //滚动加载数据
      this.pageNum = this.pageNum + 1;
      this.loadStatus = "loading";
      this.refreshWorkingList();
    },
    getTodoList() {
      //获取待处理列表

      todoList(this.queryParams).then((response) => {
        if (this.queryParams.pageNum == 1) {
          this.workingList = [];
        }
        this.workingList = [...this.workingList, ...response.data.List];
        if (response.data.List.length < this.queryParams.pageSize) {
          this.loadStatus = "noMore";
        } else {
          this.loadStatus = "more";
        }
      });
    },
    getFinishedList() {
      //获取已处理列表

      finishedList(this.queryParams).then((response) => {
        if (this.queryParams.pageNum == 1) {
          this.workingList = [];
        }
        this.workingList = [...this.workingList, ...response.data.List];
        if (response.data.List.length < this.queryParams.pageSize) {
          this.loadStatus = "noMore";
        } else {
          this.loadStatus = "more";
        }
      });
    },
    getFollowStatisticsInfo() {
      //获取流程任务信息
      followStatisticsInfo().then((res) => {
        // console.log("流程信息", res);
        let data = res.data;
        this.totalInfo = [
          {
            name: "我创建的",
            totalNum: data.MyFlowCount,
            iconName: "wochuangjiande",
            type:'my',
            list: [
              {
                name: "未开始",
                num: data.MyWaitCount,
                className: "no_start",
              },
              {
                name: "进行中",
                num: data.MyDoingCount,
                className: "loading",
              },
              {
                name: "已完成",
                num: data.MyFinishCount,
                className: "finish",
              },
              {
                name: "已取消",
                num: data.MyCancelCount,
                className: "finish",
              },
            ],
          },
          {
            name: "待我处理的",
            totalNum: data.PendingCount,
            iconName: "daichuli",
            type: "pending",
          },
          {
            name: "抄送我的",
            totalNum: data.CopyCount,
            iconName: "caosongwode",
            type: "copy",
          },
          {
            name: "我处理完的",
            totalNum: data.FinishCount,
            iconName: "chuliwancheng",
            type: "finish",
          },
        ];
      });
    },
    handleClick() {
      //设备信息的报警列表的切换
      this.loadStatus = "loading";
      this.queryParams.pageNum = 1;
      this.refreshWorkingList();
    },
  },
};
</script>
  
<style lang="scss" scoped>
.staging {
  width: calc(50% - 10px);
  // height: 100%;
  margin-bottom: 30px;

  .working_nav {
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;

    .working_nav_li {
      background-color: #ffffff;
      width: calc(100% * 0.421 - 36 * 0.307px);
      display: flex;
      justify-content: flex-start;
      border-radius: 4px;
      height: 114px;

      .nav_li_left {
        width: 40%;
        padding: 16px 0 20px 16px;

        .li_left_label {
          font-size: 14px;
          color: #78829d;
          display: flex;
          justify-content: flex-start;
          align-items: center;
          margin-bottom: 26px;

          span {
            white-space: nowrap;
          }

          .icons {
            font-size: 20px;
            margin-right: 10px;
          }
        }

        .li_left_value {
          font-size: 32px;
          color: #333333;
          height: 32px;
        }
      }

      .nav_li_right {
        width: 60%;
        padding-left: 28px;
        box-sizing: border-box;
        // padding: 0 0 18px 0;
        padding-top: 30px;
        padding-bottom: 20px;
        display: flex;
        // flex-direction: column;
        justify-content: flex-start;
        flex-wrap: wrap;
        align-items: flex-end;

        .right_li {
          display: flex;
          justify-content: flex-start;
          align-items: center;
          height: 16px;
          margin-top: 10px;
          width: 50%;

          .right_li_label {
            font-size: 14px;
            color: #78829d;
          }

          .right_li_val {
            font-size: 16px;
            color: #78829d;

            &.no_start {
              color: #3572ff;
            }

            &.loading {
              color: #61dda9;
            }
            &.finish {
              color: #78829d;
            }
          }
        }
      }
      &.no_list {
        width: calc(100% * 0.193 - 36 * 0.193px);
        .nav_li_left {
          width: 100%;
        }
        .nav_li_right {
        }
      }
    }
  }

  .alarm_table {
    background: #ffffff;
    padding: 16px 20px 20px;
    border-radius: 4px;
    margin-top: 10px;
    height: 352px;
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

    .working_ul {
      max-height: 268px;
      overflow: scroll;

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

      .working_li {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        height: 58px;
        font-size: 16px;

        .work_name {
          width: 30%;
          height: 58px;
          // line-height: 58px;
          display: flex;
          justify-content: flex-start;
          align-items: center;

          .icon_con {
            display: flex;
            justify-content: center;
            align-items: center;
            width: 36px;
            height: 36px;
            border-radius: 6px;
            font-size: 18px;
            margin-right: 10px;
            color: #ffffff;
            background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
          }

          .name {
            white-space: nowrap;
            color: #333333;
          }
        }

        .remark {
          width: 42.7%;
          height: 58px;
          // line-height: 58px;
          color: #78829d;
          display: flex;
          justify-content: flex-start;
          align-items: center;
        }

        .time {
          width: 22.2%;
          height: 58px;
          line-height: 58px;
          color: #78829d;
          text-align: right;
          white-space: nowrap;
        }
      }
    }
    .empty {
      line-height: 60px;
      width: 100%;
      text-align: center;
      color: #909399;
    }
  }
}
</style>
  