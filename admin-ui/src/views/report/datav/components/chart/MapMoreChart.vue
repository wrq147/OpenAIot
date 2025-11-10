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
import { addOption } from "../../codegen/codegen";

//加载json文件
const chinaJson=require("../../map_json/data-1527045631990-r1dZ0IM1X.json");
const shanghaiJson=require("../../map_json/data-1482909900836-H1BC_1WHg.json");
const hebeiJson=require("../../map_json/data-1482909799572-Hkgu_yWSg.json");
const shanxi1=require("../../map_json/data-1482909909703-SyCA_JbSg.json");
const neimenggu=require("../../map_json/data-1482909841923-rkqqdyZSe.json");
const liaoning=require("../../map_json/data-1482909836074-rJV9O1-Hg.json");
const jilin=require("../../map_json/data-1482909832739-rJ-cdy-Hx.json");
const heilongjiang=require("../../map_json/data-1482909803892-Hy4__J-Sx.json");
const jiangsu=require("../../map_json/data-1482909823260-HkDtOJZBx.json");
const zhejiang=require("../../map_json/data-1482909960637-rkZMYkZBx.json");
const anhui=require("../../map_json/data-1482909768458-HJlU_yWBe.json");
const fujian=require("../../map_json/data-1478782908884-B1H6yezWe.json");
const jiangxi=require("../../map_json/data-1482909827542-r12YOJWHe.json");
const shandong=require("../../map_json/data-1482909892121-BJ3auk-Se.json");
const henan=require("../../map_json/data-1482909807135-SJPudkWre.json");
const hubei=require("../../map_json/data-1482909813213-Hy6u_kbrl.json");
const hunan=require("../../map_json/data-1482909818685-H17FOkZSl.json");
const guangdong=require("../../map_json/data-1482909784051-BJgwuy-Sl.json");
const guangxi=require("../../map_json/data-1482909787648-SyEPuJbSg.json");
const hainan=require("../../map_json/data-1482909796480-H12P_J-Bg.json");
const sichuan=require("../../map_json/data-1482909931094-H17eKk-rg.json");
const guizhou=require("../../map_json/data-1482909791334-Bkwvd1bBe.json");
const yunnan=require("../../map_json/data-1482909957601-HkA-FyWSx.json");
const xizang=require("../../map_json/data-1482927407942-SkOV6Qbrl.json");
const shanxi3=require("../../map_json/data-1482909918961-BJw1FyZHg.json");
const gansu=require("../../map_json/data-1482909780863-r1aIdyWHl.json");
const qinghai=require("../../map_json/data-1482909853618-B1IiOyZSl.json");
const ningxia=require("../../map_json/data-1482909848690-HJWiuy-Bg.json");
const xinjiang=require("../../map_json/data-1482909952731-B1YZKkbBx.json");
const beijing=require("../../map_json/data-1482818963027-Hko9SKJrg.json");
const tianjin=require("../../map_json/data-1482909944620-r1-WKyWHg.json");
const chongqing=require("../../map_json/data-1482909775470-HJDIdk-Se.json");
const xianggang=require("../../map_json/data-1461584707906-r1hSmtsx.json");
const aomen=require("../../map_json/data-1482909771696-ByVIdJWBx.json");

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
  },
  data() {
    return {
      chart: null,
      area: "china", //初始化为中国地图
      //注册个省份的json
      provinces: {
        china: chinaJson,
        上海: shanghaiJson,
        河北: hebeiJson,
        山西: shanxi1,
        内蒙古: neimenggu,
        辽宁: liaoning,
        吉林: jilin,
        黑龙江: heilongjiang,
        江苏: jiangsu,
        浙江: zhejiang,
        安徽: anhui,
        福建: fujian,
        江西: jiangxi,
        山东: shandong,
        河南: henan,
        湖北: hubei,
        湖南: hunan,
        广东: guangdong,
        广西: guangxi,
        海南: hainan,
        四川: sichuan,
        贵州: guizhou,
        云南: yunnan,
        西藏: xizang,
        陕西: shanxi3,
        甘肃: gansu,
        青海: qinghai,
        宁夏: ningxia,
        新疆: xinjiang,
        北京: beijing,
        天津: tianjin,
        重庆: chongqing,
        香港: xianggang,
        澳门: aomen
      },
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
    area() {
      // 先销毁再重新创建
      // this.chart.dispose();
      this.initChart();
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  //mounted 在模板渲染成html后调用，通常是初始化页面完成后，再对html的dom节点进行一些需要的操作。
  mounted() {
    echarts.registerMap("china", this.provinces['china']);
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    //实例销毁之前调用
    if (!this.chart) {
      return;
    }
    //先清空，再重新初始化
    this.chart.dispose();
    this.chart = null;
  },
  methods: {
    initChart() {
      echarts.registerMap(this.area, this.provinces[this.area]);
      this.chart.setOption({
          series: [{
            map: this.area,
          }]
      });
    },
    setChartVal(result) {
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))

      if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      }

      var color = ["#edfbfb", "#b7d6f3", "#40a9ed", "#3598c1", "#215096"];
      if (dataOption.isTopic == true) {
        if (this.chartOption.theme != null) {
          color = this.chartOption.theme.color;
        } else {
          color = color;
        }
      } else if (dataOption.isTopic == false) {
        color = color;
      }

      let option = {
        //backgroundColor: '#101129',
        bindingType: "mapMore",
        title: {
          text: dataOption.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            //fontStyle: 'normal', //风格
            //fontWeight: 'normal', //粗细
            //fontFamily: '微软雅黑', //字体
            //fontSize: 18, //大小
            fontWeight: dataOption.fontWeight, //粗细
            fontFamily: dataOption.fontFamily, //字体
            fontSize: dataOption.fontSize, //大小
            align: "center" //水平对齐
          },
          subtext: dataOption.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.fontWeight, //粗细
            fontFamily: dataOption.fontFamily,
            fontSize: dataOption.fontSize - 5,
            align: "center"
          },
          x: dataOption.x,
          itemGap: 7
        },
        tooltip: {
          show: true,
          formatter: function(params) {
            if (params.data) return params.name + "：" + params.data["value"];
          }
        },
        visualMap: {
          show: dataOption.isVmap,
          type: "continuous",
          text: ["", ""],
          showLabel: true,
          left: "50",
          min: 0,
          max: 100,
          inRange: {
            color: color
          },
          splitNumber: 0
        },
        series: [
          {
            name: "MAP",
            type: "map",
            mapType: this.area,
            //zoom: 1, //当前视角的缩放比例
            roam: dataOption.roam, //是否开启平游或缩放
            selectedMode: false, //是否允许选中多个区域
            label: {
              normal: {
                show: true,
                color: "#000"
              },
              emphasis: {
                show: true,
                color: "#000"
              }
            },
            data: result
          }
        ]
      };
      this.chart.setOption(option, true);
      //单击切换到省级地图，当mapCode有值,说明可以切换到下级地图
      let timeFn = null;
      //设置单击、双击状态
      let click_type;
      this.chart.on("click", params => {
        //单击
        click_type = false;

        clearTimeout(timeFn);
        timeFn = setTimeout(() => {
          //检测是否为单击
          if (click_type != false) return;
          let mapCode = this.provinces[params.name];
          if (!mapCode) {
            //alert('无此区域地图显示');
            return;
          }
          
          this.area = params.name; //地区name
        }, 300);
      });
      // 绑定双击事件，返回全国地图
      this.chart.on("dblclick", params => {
        //双击状态
        click_type = true;

        //当双击事件发生时，清除单击事件，仅响应双击事件
        clearTimeout(timeFn);

        //返回全国地图
        this.area = "china"; //地区name
      });

      addOption(dataOption.bindingDiv, option);
    },
   
  }
};
</script>
