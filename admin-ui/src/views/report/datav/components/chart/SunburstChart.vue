<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  />
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import { getLinkChart } from "../../util/LinkageChart";
import VueEvent from "../../VueEvent";
import { addOption } from "../../codegen/codegen";
import dataChart from '../mixins/dataChart.js'
export default {
  mixins: [resize,dataChart],
  props: {
    className: {
      type: String,
      default: "chart"
    },
    width: {
      type: String,
      default: "100%"
    },
    height: {
      type: String,
      default: "100%"
    },
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      chart: null,
      animate: this.className
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    height() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    "chartOption.theme": {
      handler() {
        if (this.chart != null) {
          this.chart.dispose();
          this.chart = null;
        }
      }
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    if (!this.chart) {
      return;
    }
    this.chart.dispose();
    this.chart = null;
  },
  methods: {
    setChartVal(result) {
      
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));

      let option = {
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center"
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        series: {
          type: "sunburst",
          emphasis: {
            focus: "ancestor"
          },
          sort: dataOption.sort == "null" ? null : dataOption.sort,
          data: result,
          radius: dataOption.radius,
          itemStyle: dataOption.itemStyle,
          label: dataOption.label
          // levels: [
          //   {},
          // {
          //   r0: '15%',
          //   r: '35%',
          //   itemStyle: {
          //     borderWidth: 2
          //   },
          //   label: {
          //     rotate: 'tangential'
          //   }
          // },
          // {
          //   r0: '35%',
          //   r: '70%',
          //   label: {
          //     align: 'right'
          //   }
          // },
          // {
          //   r0: '70%',
          //   r: '72%',
          //   label: {
          //     position: 'outside',
          //     padding: 3,
          //     silent: false
          //   },
          //   itemStyle: {
          //     borderWidth: 3
          //   }
          // }
          // ]
        }
      };

      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = dataOption.bindList;

          if (bindList.length > 0) {
            getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
          }
        });
      }
      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let drillDownChartOption = this.chartOption.drillDownChartOption;

          if (
            drillDownChartOption != undefined &&
            drillDownChartOption != null
          ) {
            this.$set(
              this.chartOption.drillDownChartOption.chartOption,
              "requestParameters",
              [{"name":"drillParam","value":arrs}]
            );
            //发送下钻消息
            VueEvent.$emit(
              "drill_down_msg",
              this.chartOption.drillDownChartOption
            );
          }
        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.off("click");
      }

    },

  }
};
</script>
