<template>
  <div class="custom_info" @click="closeOpen">
    <div class="home_module_title">
      <!-- <svg-icon icon-class="wodekehu"></svg-icon> -->
      <i class="zhongtaiiconfont zhongtai-icon-wodekehu"></i>
      <span>我的客户</span>
    </div>
    <div class="brief_report">
      <div class="report_title">
        <div class="left">销售简报</div>
        <div class="right">
          <div class="alchoice" @click.stop="openSelect(1)">
            <div class="choice">{{ statisticsDayText }}</div>
            <!-- <svg-icon icon-class="a-xiajiantou"></svg-icon> -->
            <i class="zhongtaiiconfont zhongtai-icon-a-xiajiantou" style="font-size:8px"></i>
          </div>
          <div class="time_list" v-if="open == 1">
            <div class="time_li" @click.stop="changeDay(0, '今日')">今日</div>
            <div class="time_li" @click.stop="changeDay(1, '昨日')">昨日</div>
            <div class="time_li" @click.stop="changeDay(7, '近7天')">近7天</div>
            <div class="time_li" @click.stop="changeDay(30, '近30天')">
              近30天
            </div>
          </div>
        </div>
      </div>
      <div class="report_ul">
        <div class="report_li" v-for="item in reportList" :key="item.label">
          <div class="label">{{ item.label }}</div>
          <div class="val">{{ item.num }}</div>
        </div>
      </div>
    </div>
    <div class="custom_report_chart">
      <div class="report_title">
        <div class="left">客户总数</div>
        <div class="right">
          <div class="alchoice" @click.stop="openSelect(2)">
            <div class="choice">{{ periodDayText }}</div>
            <!-- <svg-icon icon-class="a-xiajiantou"></svg-icon> -->
            <i class="zhongtaiiconfont zhongtai-icon-a-xiajiantou" style="font-size:8px"></i>
          </div>
          <div class="time_list" v-if="open == 2">
            <div class="time_li" @click.stop="changeDay2(0, '今日')">今日</div>
            <div class="time_li" @click.stop="changeDay2(1, '昨日')">昨日</div>
            <div class="time_li" @click.stop="changeDay2(7, '近7天')">
              近7天
            </div>
            <div class="time_li" @click.stop="changeDay2(30, '近30天')">
              近30天
            </div>
          </div>
        </div>
      </div>
      <div id="custom_report_chart"></div>
    </div>
  </div>
</template>

<script>


// 引入 ECharts 主模块
var echarts = require('echarts/lib/echarts');
// 引入柱状图
require('echarts/lib/chart/funnel');
// 引入提示框和标题组件
require('echarts/lib/component/dataZoom');
require('echarts/lib/component/title');
 

import { crmStatisticsInfo, periodCountList } from "@/api/home.js";
export default {
  name: "AdminUiCustomInfo",

  data() {
    return {
      open: 0, //是否显示筛选
      reportList: [
        { label: "新增客户数", num: 0 },
        { label: "新增商机数", num: 0 },
        { label: "新增跟进数", num: 0 },
        { label: "商机预测总金额", num: 0 },
        { label: "跟进线索数", num: 0 },
        { label: "跟进客户数", num: 0 },
        { label: "跟进商机数", num: 0 },
        { label: "商机赢单数", num: 0 },
      ],
      customData: [
        ["需求发现", 0],
        ["需求确认", 0],
        ["方案报价", 0],
        ["商务谈判", 0],
        ["赢单", 0],
        ["丢单", 0],
        ["无效", 0],
      ],
      options: {
        labelLine: {
          show: true,
        },
        series: [
          {
            name: "漏斗",
            type: "funnel",
            top: 0,
            bottom: 0,
            left: "-20%",
            width: "100%",
            height: "196",
            min: 0,
            max: 100,
            minSize: "30%",
            maxSize: "60%",
            sort: "descending",
            gap: 0,
            color: ["rgba(255, 255, 255, 1)"],
            labelLine: {
              //视觉引导线样式
              length: 60,
              lineStyle: {
                width: 1,
                type: "solid",
              },
            },
            label: {
              //漏斗外部显示的
              show: false,
              position: "right", //位置
              formatter: "{b}", //显示的内容
              fontStyle: "normal",
              fontSize: 12,
              color: "#8992A9",
              // textBorderColor: '#fff'
            },
            emphasis: {
              //鼠标移入数据项的tooltip设置
              show: false,
            },
            itemStyle: {
              opacity: 1, //图形透明度
              borderColor: "#fff", //图形边框颜色
              borderWidth: 0, //图形边框宽度
            },
            data: [
              //我的数据是根据需求自己设置的name
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
              {
                value: 0,
                name: "数量：0，金额：0",
              },
            ],
            z: 99,
          },
          {
            name: "漏斗",
            type: "funnel",
            top: 0,
            bottom: 0,
            left: "-20%",
            width: "100%",
            height: "196",
            min: 0,
            max: 100,
            minSize: "30%",
            maxSize: "60%",
            // sort: 'ascending',
            gap: 0,
            color: [
              "rgba(120,211,248, 1)",
              "rgba(116,176,225, 1)",
              "rgba(60,139,160, 1)",
              "rgba(204,243,228, 1)",
              "rgba(97,221,169, 1)",
              "rgba(205,221,253, 1)",
              "rgba(91,143,249, 1)",
            ],
            labelLine: {
              //视觉引导线样式
              length: 90,
              lineStyle: {
                width: 1,
                type: "solid",
              },
            },
            label: {
              //漏斗外部显示的
              position: "right", //位置
              formatter: "{b}", //显示的内容
              fontStyle: "normal",
              fontSize: 12,
              color: "#8992A9",
              // textBorderColor: '#fff'
            },
            // emphasis: {
            //     label: {
            //         fontSize: 14 //鼠标移入字体变大 显示toolList
            //     }
            // },
            itemStyle: {
              opacity: 1, //图形透明度
              borderColor: "#fff", //图形边框颜色
              borderWidth: 0, //图形边框宽度
            },
            data: [
              //我的数据是根据需求自己设置的name
            ],
            z: 100,
          },
        ],
      },
      periodDay: 0, //销售漏斗图数据时间
      periodDayText: "今日", //销售漏斗图数据时间
      statisticsDay: 0, //统计天数
      statisticsDayText: "今日", //统计天数
    };
  },
  mounted() {
    this.getCrmStatisticsInfo();
    this.getPeriodCountList();
  },
  beforeDestroy(){
    window.removeEventListener('resize', ()=>{});
  },
  methods: {
    closeOpen() {
      this.open = 0;
    },
    changeDay(val, text) {
      //销售漏斗不同时间切换
      this.open = 0;
      this.statisticsDay = val;
      this.statisticsDayText = text;
      this.getCrmStatisticsInfo();
    },
    changeDay2(val, text) {
      //客户统计不同时间切换
      this.open = 0;
      this.periodDay = val;
      this.periodDayText = text;

      this.getPeriodCountList();
    },
    getPeriodCountList() {
      //获取销售漏斗数据
      periodCountList({ day: this.periodDay }).then((res) => {
        // console.log("销售漏斗数据", res);
        let seriesData = [];
        // let arr = res.data.filter((row) => {
        //   return row.Name != "赢单" && row.Name != "输单" && row.Name != "无效";
        // });
        // console.log(arr, "销售漏斗数据过滤");
        // let ave = Math.trunc(100 / arr.length);
        // for (let i = res.data.length - 1; i >= 0; i--) {
        //   if (i < arr.length) {
        //     let obj = {
        //       value: ave * (arr.length - i),
        //       name:
        //         res.data[i].Name +
        //         " 数量：" +
        //         res.data[i].Count +
        //         "，金额：" +
        //         res.data[i].TotalPrice,
        //     };
        //     seriesData.push(obj);
        //   } else {
        //     let obj = {
        //       value: 0,
        //       name:
        //         res.data[i].Name +
        //         " 数量：" +
        //         res.data[i].Count +
        //         "，金额：" +
        //         res.data[i].TotalPrice,
        //     };
        //     seriesData.push(obj);
        //   }
        // }
        const totalCount = res.data.flat().reduce((sum, item) => sum + item.Count, 0);
        for (let i = 0; i < res.data.length; i++) {
            const item = res.data[i];
            const percentage = (item.Count / totalCount) * 100;
            const formattedPercentage = percentage.toFixed(2); // 保留两位小数
              let obj = {
              value: formattedPercentage,
              name:
                res.data[i].Name +
                " 数量：" +
                res.data[i].Count +
                "，金额：" +
                res.data[i].TotalPrice,
            };
            seriesData.push(obj);
        }
        // console.log(seriesData, "销售漏斗数据")
        this.options.series[1].data = seriesData.reverse();
        var myChart = echarts.init(
          document.getElementById("custom_report_chart")
        );
        myChart.setOption(this.options);
        myChart.resize();
        window.addEventListener("resize", () => {
          myChart.setOption(this.options);
          myChart.resize();
        });
      });
    },
    getCrmStatisticsInfo() {
      //客户统计数据
      crmStatisticsInfo({ day: this.statisticsDay }).then((res) => {
        // console.log("客户统计数据", res);
        let data = res.data;
        this.reportList = [
          { label: "新增客户数", num: data.NewKfCount },
          { label: "新增商机数", num: data.NewOpportCount },
          { label: "新增跟进数", num: data.NewFollowCount },
          { label: "商机预测总金额", num: data.MaybeTotalPrice },
          { label: "跟进线索数", num: data.FollowClueCount },
          { label: "跟进客户数", num: data.FollowCustomerCount },
          { label: "跟进商机数", num: data.FollowOpportCount },
          { label: "商机赢单数", num: data.WinOpportCount },
        ];
      });
    },
    openSelect(val) {
      if (val == this.open) {
        this.open = 0;
      } else {
        this.open = val;
      }
    },
  },
};
</script>

<style lang="less" scoped>
.custom_info {
  width: calc(50% - 10px);
  // height: 100%;
  margin-bottom: 30px;

  .brief_report {
    width: 100%;
    box-sizing: border-box;
    background: #ffffff;
    border-radius: 4px;

    .report_ul {
      width: 100%;
      display: flex;
      justify-content: flex-start;
      align-items: flex-start;
      flex-wrap: wrap;

      .report_li {
        width: calc(25% - 11.5px);
        text-align: center;
        height: 80px;
        padding-top: 10px;
        box-sizing: border-box;

        .label {
          color: #78829d;
          font-size: 14px;
          margin-bottom: 12px;
        }

        .val {
          font-size: 24px;
          color: #333333;
        }
      }
    }
  }

  .report_title {
    padding: 0 20px;
    height: 46px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: relative;

    .left {
      font-size: 16px;
      color: #333333;
    }

    .right {
      font-size: 10px;
      color: #333333;
      display: flex;
      justify-content: flex-end;
      align-items: center;

      .alchoice {
        display: flex;
        justify-content: flex-end;
        align-items: center;
      }

      .choice {
        font-size: 14px;
        margin-right: 4px;
      }

      .time_list {
        position: absolute;
        width: 86px;
        box-shadow: 0px 10px 20px 0px rgba(0, 25, 130, 0.1);
        border-radius: 10px;
        background: #ffffff;
        top: 36px;
        right: 16px;
        z-index: 999;

        .time_li {
          height: 38px;
          line-height: 38px;
          text-align: center;
        }
      }
    }
  }

  .custom_report_chart {
    width: 100%;
    height: 260px;
    margin-top: 12px;
    background: #ffffff;
    padding: 0 0 20px;
    box-sizing: border-box;

    #custom_report_chart {
      padding: 0 20px;
      box-sizing: border-box;
      width: 100%;
      height: 212px;
    }
  }
}
</style>