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
import "echarts/map/js/world";
import "echarts/map/js/china";
import "echarts/map/js/china-contour";
import "echarts/map/js/province/anhui";
import "echarts/map/js/province/aomen";
import "echarts/map/js/province/hebei";
import "echarts/map/js/province/heilongjiang";
import "echarts/map/js/province/henan";
import "echarts/map/js/province/hubei";
import "echarts/map/js/province/hunan";
import "echarts/map/js/province/jiangsu";
import "echarts/map/js/province/jiangxi";
import "echarts/map/js/province/jilin";
import "echarts/map/js/province/liaoning";
import "echarts/map/js/province/neimenggu";
import "echarts/map/js/province/beijing";
import "echarts/map/js/province/ningxia";
import "echarts/map/js/province/qinghai";
import "echarts/map/js/province/shandong";
import "echarts/map/js/province/shanghai";
import "echarts/map/js/province/shanxi";
import "echarts/map/js/province/shanxi1";
import "echarts/map/js/province/sichuan";
import "echarts/map/js/province/taiwan";
import "echarts/map/js/province/tianjin";
import "echarts/map/js/province/xianggang";
import "echarts/map/js/province/chongqing";
import "echarts/map/js/province/xinjiang";
import "echarts/map/js/province/xizang";
import "echarts/map/js/province/yunnan";
import "echarts/map/js/province/zhejiang";
import "echarts/map/js/province/fujian";
import "echarts/map/js/province/gansu";
import "echarts/map/js/province/guangdong";
import "echarts/map/js/province/guangxi";
import "echarts/map/js/province/guizhou";
import "echarts/map/js/province/hainan";

import { dealWithData, sortsFunDesc } from "../../util/utilFun";
import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import dataChart from "../mixins/dataChart.js";
import VueEvent from "../../VueEvent";

export default {
  mixins: [resize, dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      chart: null,
      animate: this.className,
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
      },
    },
    className: {
      handler(value) {
        this.animate = value;
      },
    },
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

      //let mapName = "china";
      let mapName = dataOption.mapName;
      let geoCoordMap = {};
      /*获取地图数据*/
      //myChart.showLoading();
      let mapFeatures = echarts.getMap(mapName).geoJson.features;
      //myChart.hideLoading();
      mapFeatures.forEach(function (v) {
        // 地区名称
        let name = v.properties.name;
        // 地区经纬度
        geoCoordMap[name] = v.properties.cp;
      });

      let data = [...result];
      let max = data.sort(sortsFunDesc("value"))[0].value;
      let res = [];
      let lines = [];
      for (let i = 0; i < data.length; i++) {
        let fromCoord = geoCoordMap[data[i].from];
        let toCoord = geoCoordMap[data[i].to];
        if (toCoord) {
          res.push({
            name: data[i].to,
            value: toCoord.concat(data[i].value),
            type: data[i].type == undefined ? "" : data[i].type,
          });
        } else if (data[i].lng2 != undefined && data[i].lat2 != undefined) {
          res.push({
            name: data[i].to,
            value: [data[i].lng2, data[i].lat2, data[i].value],
            type: data[i].type == undefined ? "" : data[i].type,
          });
        }

        if (fromCoord && toCoord) {
          lines.push([
            {
              coord: fromCoord,
              value: data[i].value,
            },
            {
              coord: toCoord,
            },
          ]);
        } else if (fromCoord) {
          lines.push([
            {
              coord: fromCoord,
              value: data[i].value,
            },
            {
              coord: [data[i].lng2, data[i].lat2],
            },
          ]);
        } else if (toCoord) {
          lines.push([
            {
              coord: [data[i].lng1, data[i].lat1],
              value: data[i].value,
            },
            {
              coord: toCoord,
            },
          ]);
        } else {
          lines.push([
            {
              coord: [data[i].lng1, data[i].lat1],
              value: data[i].value,
            },
            {
              coord: [data[i].lng2, data[i].lat2],
            },
          ]);
        }
      }

      let resultRes = [];
      let datas = dealWithData(res, "type");
      datas.forEach((element) => {
        if (element.type == "node") {
          resultRes = element.data;
        }
      });

      datas.forEach((element) => {
        if (element.type != "node") {
          resultRes = resultRes.concat(element.data);
        }
      });

      const rr = new Map();
      resultRes = resultRes.filter((a) => !rr.has(a.name) && rr.set(a.name, 1));

      let seriesData = [
        {
          type: "lines",
          zlevel: 1,
          effect: dataOption.lines.effect,
          lineStyle: dataOption.lines.lineStyle,
          data: lines,
        },
        {
          type: "effectScatter",
          coordinateSystem: "geo",
          zlevel: 2,
          rippleEffect: dataOption.rippleEffect,
          label: {
            normal: {
              show: true,
              position: dataOption.label.position, //显示位置
              offset: [5, 0], //偏移设置
              formatter: function (params) {
                //圆环显示文字
                return params.data.name;
              },
              fontSize: dataOption.label.fontSize,
              color: dataOption.label.color,
              zlevel: 1,
            },
            emphasis: {
              show: true,
            },
          },
          symbol: "circle",
          symbolSize: function (val) {
            if (dataOption.symbol.auto) {
              return val[2] * dataOption.symbol.coefficient;
            } else {
              return dataOption.symbol.size;
            }
          },
          itemStyle: {
            normal: {
              show: false,
            },
          },
          data: resultRes,
        },
      ];

      let option = {
        tooltip: {
          trigger: "item",
          showDelay: 0,
          hideDelay: 0,
          enterable: true,
          transitionDuration: 0,
          extraCssText: "z-index:100",
          formatter: function (params, ticket, callback) {
            //根据业务自己拓展要显示的内容
            if (params.seriesType == "effectScatter") {
              let res = "";
              let name = params.name;
              let value = params.value[params.seriesIndex + 1];
              res =
                "<span style='color:#fff;font-size:" +
                dataOption.tooltip.fontSize +
                "px'>" +
                name +
                "：" +
                value +
                dataOption.suffix +
                "</span>";
              return res;
            }
          },
        },
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center", //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center",
          },
          x: dataOption.title.x,
          itemGap: 7,
        },
        //tooltip: dataOption.tooltip,
        visualMap: {
          //图例值控制
          min: 0,
          max: max,
          calculable: true,
          show: dataOption.visualMap.show,
          color: ["#f44336", "#fc9700", "#ffde00", "#ffde00", "#00eaff"],
          textStyle: {
            color: "#fff",
          },
          left: dataOption.visualMap.left + "%",
          bottom: dataOption.visualMap.bottom + "%",
        },
        geo: {
          map: mapName,
          zoom: 1.2,
          label: {
            emphasis: {
              show: false,
            },
          },
          roam: dataOption.roam, //是否允许缩放
          itemStyle: dataOption.itemStyle,
        },

        series: seriesData,
        bindingType: "mapline",
      };

      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", (params) => {
          if (params.value != undefined) {
            //设置参数
            let arrObject = {
              legendName: params.name,
              data: params.value[2],
            };

            let arrs = JSON.stringify(arrObject);

            //获取绑定的图表
            let bindList = this.chartOption.bindList;

            if (bindList.length > 0) {
              getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
            }
          }
        });
      }
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", (params) => {
          if (params.value != undefined) {
            //设置参数
            let arrObject = {
              legendName: params.name,
              data: params.value,
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
                [{ name: "drillParam", value: arrs }]
              );
              //发送下钻消息
              VueEvent.$emit(
                "drill_down_msg",
                this.chartOption.drillDownChartOption
              );
            }
          }
        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.off("click");
      }
    },
  },
};
</script>
