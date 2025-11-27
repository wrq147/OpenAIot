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
import dataChart from '../mixins/dataChart.js'
import VueEvent from "../../VueEvent";

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
     
      let data = JSON.parse(JSON.stringify(result));
      //*******************************************使用主题******************************************************/
      let color = [ "#c12e34", "#e6b600", "#0098d9", "#2b821d", "#005eaa", "#339ca8", "#cda819", "#32a487" ];
      if (dataOption.isTopic == true) {
        if (this.chartOption.theme != null) {
          color = this.chartOption.theme.color;
        } else {
          color = color;
        }
      } else if (dataOption.isTopic == false) {
        color = color;
      }

      //若分类依据错误则默认橙色，100
      //console.log(theme,color);
      let categories = [];
      for (let x = 0; x < data.nodes.length; x++) {
        var this_style = data.nodes[x].category;

        if (dataOption.pointStyles[this_style] == undefined) {
          let styles = {};
          styles.itemStyle = {};
          styles.label = {};
          //styles.label.color = "#fff";
          // styles.label.fontSize = "14";
          styles.itemStyle.color = color[x % color.length];
          //styles.symbolSize = ((x * 5) % 50) + 20;
          styles.name = "type" + this_style;
          dataOption.pointStyles[this_style] = styles;
        }
        categories.push(dataOption.pointStyles[this_style]);
      }
      dataOption.pointStyles = categories;
      
      //初始化路径样式+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
      var linkstyles = {};
      for (let x = 0; x < data.links.length; x++) {
        this_style = data.links[x].name;

        if (dataOption.lineStyles[this_style] == undefined) {
          var styles = {};
          styles.color = color[x % color.length];
          styles.label = {};
          styles.label.color = color[x % color.length];
          //styles.label.fontSize = "14";
          dataOption.lineStyles[this_style] = styles;
        }
        linkstyles[this_style] = dataOption.lineStyles[this_style];
      }
      dataOption.lineStyles = linkstyles;
      data.links.forEach(link => {
        link.label = {
          align: "center"
          //fontSize: 12,
        };

        if (
          !dataOption.lineStyles[link.name] ||
          !dataOption.lineStyles[link.name].color
        ) {
          //未定义则默认为100
          link.lineStyle = {
            color: color[2]
          };
        } else {
          link.lineStyle = {
            color: dataOption.lineStyles[link.name].color
          };
          link.label = dataOption.lineStyles[link.name].label;
        }
      });
      // console.log(dataOption.pointStyles,"+++++",dataOption.lineStyles);

      let option = {
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontFamily: "微软雅黑", //字体
            fontWeight: "normal", //粗细
            fontSize: 17, //大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff", //颜色
            fontFamily: "微软雅黑", //字体
            fontWeight: "normal", //粗细
            fontSize: 12, //大小
            align: "center" //水平对齐
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        legend: dataOption.legend,
        series: [
          {
            type: "graph",
            layout: "force",
            symbolSize: dataOption.symbolSize ? dataOption.symbolSize : 20,
            width: "50%",
            height: "50%",
            draggable: true,
            roam: false,
            focusNodeAdjacency: true,
            categories: categories,
            edgeSymbol: ["", "arrow"],
            // edgeSymbolSize: [80, 10],
            edgeLabel: {
              normal: {
                show: true,
                textStyle: {
                  fontSize: dataOption.fontSize ? dataOption.fontSize : 20
                },
                formatter(x) {
                  return x.data.name;
                }
              }
            },
            label: {
              show: true,
              textStyle: {
                fontSize: dataOption.fontSize ? dataOption.fontSize : 20,
                fontFamily: dataOption.fontFamily
                  ? dataOption.fontFamily
                  : "微软雅黑",
                fontWeight: dataOption.fontWeight
                  ? dataOption.fontWeight
                  : "normal",
                color: dataOption.fontColor ? dataOption.fontColor : "#fff"
              }
            },
            force: {
              repulsion: dataOption.repulsion
                ? parseInt(dataOption.repulsion)
                : 500,
              edgeLength: 120
            },
            data: data.nodes,
            links: data.links
          }
        ]
      };
      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //console.log("params", params);

          //设置参数
          let arrObject = {
            legendName: params.name,
            data: params.data.category
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = this.chartOption.bindList;

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
            data: params.data.category
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
