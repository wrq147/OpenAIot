<template>
  <div class="schedule_reminder">
    <div class="home_module_title">
      <!-- <svg-icon icon-class="richengtixing"></svg-icon> -->
      <i class="zhongtaiiconfont zhongtai-icon-richengtixing"></i>
      <span>日程提醒</span>
    </div>
    <div class="schedule_working" v-loading="isLoadingData">
      <div class="schedule">
        <div class="today_choice_con">
          <div class="today_choice" @click="choiceToday">今日</div>
        </div>
        <Calendar
          ref="Calendar"
          v-on:choseDay="clickDay"
          v-on:changeMonth="changeDate"
        ></Calendar>
      </div>
      <div class="working_con">
        <div class="working_con_title">操作记录</div>
        <div
          class="listoper"
          v-infinite-scroll="loadoperList"
          :infinite-scroll-disabled="loadStatus == 'noMore'"
          v-if="operList.length > 0"
        >
          <div
            class="oper_li"
            v-for="(item, index) in operList"
            :key="item.oper_id + 'index' + index"
            @click.stop="jumpUrl(item)"
          >
            <div class="li_left">
              <div class="dot"></div>
              <div class="line" v-if="index < operList.length - 1"></div>
            </div>
            <div class="li_right">
              <div class="time">{{ item.oper_time }}</div>
              <div class="name">{{ item.title }}</div>
            </div>
          </div>
        </div>
        <div class="empty" v-else>暂无数据</div>
      </div>
    </div>
  </div>
</template>
  
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import Calendar from "vue-calendar-component";

import dayjs from "dayjs";
import { personOperLogList } from "@/api/home.js";
export default {
  name: "scheduleReminder",
  components: {
    Calendar,
  },
  mixins: [resizeTableCon],
  data() {
    return {
      days: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
      weeks: [],
      operList: [],
      querydata: {
        pageSize: 10,
        pageNum: 1,
      },
      loadStatus: "loading",
      isLoadingData: false,
    };
  },
  mounted() {
    let choiceDate = dayjs(new Date()).format("YYYY-MM-DD");
    this.$refs.Calendar.ChoseMonth(choiceDate, true);
    this.querydata.beginTime = choiceDate + " 00:00:00";
    this.querydata.endTime = choiceDate + " 23:59:59";
    this.querydata.pageNum = 1;
    // this.getPersonList();
  },
  methods: {
    jumpUrl(item) {
      if (item.oper_url.indexOf("/CRMService/Clue") > -1) {
        let tmpidx = item.oper_url.lastIndexOf("=");
        let id = "";
        if (tmpidx > -1) {
          tmpidx = Number(tmpidx) + 1;
          id = item.oper_url.substring(tmpidx);
        }
        console.log(tmpidx, "tmpidx", id);
        if (id) {
          this.$router.push({
            path: "/crm/clue/clueDetail",
            query: { id: id },
          });
        }
      }
    },
    loadoperList() {
      if (this.loadStatus != "noMore") {
        this.querydata.pageNum++;
        this.getPersonList();
      }
    },
    getPersonList(query) {
      if (query && query == "load"&&this.querydata.pageNum == 1) {
        this.isLoadingData = true;
      }
      personOperLogList(this.querydata)
        .then((res) => {
          if (this.querydata.pageNum == 1) {
            this.operList = [];
          }
          if (res.data && res.data.List && res.data.List.length > 0) {
            this.operList = [...this.operList, ...res.data.List];
          }
          if (res.data.List.length < this.querydata.pageSize) {
            this.loadStatus = "noMore";
          } else {
            this.loadStatus = "more";
          }
          if (query && query == "load"&&this.querydata.pageNum == 1) {
            this.isLoadingData = false;
          }
        })
        .catch((err) => {
          if (query && query == "load"&&this.querydata.pageNum == 1) {
            this.isLoadingData = true;
          }
        });
    },
    choiceToday() {
      let choiceDate = dayjs(new Date()).format("YYYY-MM-DD");
      this.$refs.Calendar.ChoseMonth(choiceDate, true);
      this.querydata.beginTime = choiceDate + " 00:00:00";
      this.querydata.endTime = choiceDate + " 23:59:59";
      this.querydata.pageNum = 1;
      this.getPersonList("load");
    },
    clickDay(data) {
      let choiceDate = dayjs(data).format("YYYY-MM-DD");
      // console.log(data, choiceDate); //选中某天
      this.querydata.beginTime = choiceDate + " 00:00:00";
      this.querydata.endTime = choiceDate + " 23:59:59";
      this.querydata.pageNum = 1;
      this.getPersonList("load");
    },
    changeDate(data) {
      // console.log(data); //左右点击切换月份
    },
    clickToday(data) {
      // console.log(data); // 跳到了本月
    },
  },
};
</script>
  
<style lang="scss" scoped>
.schedule_reminder {
  width: calc(50% - 10px);
  // height: 100%;
  margin-bottom: 30px;

  .schedule_working {
    display: flex;
    justify-content: space-between;
  }

  .schedule {
    width: calc(55% - 12px * 0.55);
    height: 478px;
    background-color: #fff;
    display: flex;
    justify-content: center;
    border-radius: 4px;
  }

  .working_con {
    background-color: #fff;
    border-radius: 4px;
    width: calc(45% - 12px * 0.45);
    height: 478px;
    padding: 24px 24px;
    box-sizing: border-box;

    .working_con_title {
      font-size: 16px;
      color: #333333;
    }
    .listoper {
      width: 100%;
      margin-top: 16px;
      height: 418px;
      overflow-y: scroll;
      &::-webkit-scrollbar {
        width: 0 !important;
      } // 隐藏垂直方向的滚动条

      &::-webkit-scrollbar-thumb {
        height: 0 !important;
      }
      .oper_li {
        display: flex;
        justify-content: flex-start;
        align-items: flex-start;
        font-size: 14px;
        height: 60px;
        .li_left {
          display: flex;
          flex-direction: column;
          justify-content: flex-start;
          align-items: center;
          margin-right: 10px;
          padding: 3px;
          .dot {
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: rgba(53, 114, 255, 1);
          }
          .line {
            width: 2px;
            height: 54px;
            background: rgba(246, 249, 255, 1);
          }
        }
        .li_right {
          font-size: 12px;
          .time {
            color: rgba(153, 153, 153, 1);
            margin-bottom: 10px;
          }
          .name {
            color: rgba(51, 51, 51, 1);
          }
        }
      }
    }
    .empty {
      color: #666666;
      margin-top: 100px;
      text-align: center;
    }
  }
}
</style>
<style lang="less">
.schedule_reminder {
  .schedule {
    position: relative;

    .today_choice_con {
      height: 47px;
      display: flex;
      justify-content: center;
      align-items: center;
      position: absolute;
      right: 20px;
      top: 0;

      .today_choice {
        cursor: pointer;
        width: 50px;
        height: 27px;
        background: #f6f9ff;
        color: #3572ff;
        font-size: 14px;
        text-align: center;
        line-height: 27px;
        border-radius: 4px;
      }
    }

    .wh_container {
      height: 100%;
      margin: 0;
      width: 100%;

      .wh_content_all {
        /*主体*/
        background-color: #ffffff;
        // border: 1px silver solid;
        border-radius: 5px;
        font-size: 16px;
        height: 100%;

        .wh_top_changge {
          width: 40%;
          margin-left: 13px;
        }

        .wh_jiantou1 {
          /*左箭头*/
          width: 8px;
          height: 8px;
          border-top: 2px solid #78829d;
          border-left: 2px solid #78829d;
          font-size: 12px;
        }

        .wh_jiantou2 {
          /*右箭头*/
          width: 8px;
          height: 8px;
          border-top: 2px solid #78829d;
          border-right: 2px solid #78829d;
          font-size: 12px;
        }

        .wh_top_changge li {
          /*当前年月标题*/
          color: #333333;
          font-size: 16px;
        }

        .wh_content {
          display: flex;
          justify-content: space-between;
          align-content: flex-start;
        }

        .wh_content_item {
          margin: 5px 0;
          font-size: 14px;
          line-height: 52px;
          height: 52px;

          .wh_top_tag {
            /*星期标题*/
            color: #78829d;
          }

          .wh_item_date {
            /*当前月*/
            color: #333333;
            border-radius: 4px;
            font-weight: 550;
          }

          .wh_item_date:hover {
            //悬浮
            color: #ffffff;
            background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
            border-radius: 50%;
            border-radius: 4px;
          }

          .wh_other_dayhide {
            /*上月和下月时间*/
            color: #78829d;
            border-radius: 4px;
          }

          .wh_isToday {
            /*当前天*/
            /*background: #33ad53;*/
            // background: #ff4d4d;
            background: #ffffff;
            color: #4c79ff;
            border-radius: 4px;
          }

          .wh_chose_day {
            //选中
            background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
            color: #ffffff;
            border-radius: 4px;
          }
        }
      }
    }
  }
}
</style>
  