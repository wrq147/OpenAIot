<template>
  <div style="width:100%;height:100vh;" :style="appContainerStyle">
    <div id="canvas" :style="canvasStyle">
      <div v-for="(item, index) in noSvgGroupDraw" :key="index" :style="chartStyle(item)" v-show="item.isShow == true">
        <component :is="chartName(item)" :theme="viewTheme.themeColor" :isDraw="false" :width="item.width + 'px'"
          :height="item.height + 'px'" :chartOption="item.chartOption" :className="item.chartOption.animate"
          :drawingList="viewData" :customId="item.customId" v-if="item.isShow == true" pageState="preview"
          :alLoadData="alLoadData" @setAlLoadData="setAlLoadData"></component>
      </div>
      <svg ref="svgChartRef" width="100%" height="100%" preserveAspectRatio="none">
        <g v-for="(item, index) in svgGroupDraw" :key="index">
          <component :is="chartName(item)" :theme="viewTheme.themeColor" :isDraw="false" :chartOption="item.chartOption"
            :drawingList="viewData" :customId="item.customId" :dragchartdata="item">
          </component>
        </g>
      </svg>
    </div>
  </div>
</template>

<script>

import './animate/animate.css'
// import { getFormsource } from "@/api/report/formsource";
// import { chartBIanalysis } from "@/api/report/sourse";
// import { chartApi } from "@/api/report/chartApi";
// import {replaceLinkParam} from "./util/LinkageChart"
import VueEvent from './VueEvent'
import AllComponents from './ComponentsExport'
import Vue from 'vue'
import dataV from '@jiaminghi/data-view'
Vue.use(dataV);
import {
  confirmValue,
  getbaseData,
  combinationTableColum,
  combinationConfirmValue,
  everyOngetData
} from "@/views/report/datav/LayerItems/commonRuning";
import LZString from 'lz-string';
export default {
  components: AllComponents,
  data() {
    return {
      viewTheme: null,
      viewData: [],
      //屏幕宽高
      screenWidth: document.body.offsetWidth,
      screenHeight: document.body.offsetHeight,
      basePath: process.env.VUE_APP_BASE_API,
      drillDownDialogFlag: false,
      drillDownDialogData: null,
      globalTimers: [],
      doubleThemeOption: {},
      doubleType: 'pc',
      alLoadData: []//
    }
  },
  async created() {
    // this.doubleThemeOption = JSON.parse(localStorage.getItem("viewdata"));

    this.doubleThemeOption = JSON.parse(LZString.decompress(localStorage.getItem('viewdata')));
    await this.initTypeApply()
    VueEvent.$on('tabchange', function (chartList) {
      if (chartList != null) {
        //根据tab标签绑定组件设置组件的显示情况
        for (const obj of this.viewData) {
          const chart = chartList.filter(function (item) {
            return item.customId == obj.customId;
          })[0]

          if (chart != null) {
            obj.isShow = chart.isShow;
            //obj.chartOption.animate = chart.animate;
          }
        }
      }
    }.bind(this))

    //交互组件更新组件配置
    VueEvent.$on('interactChange', function (chartId, act) {
      let index = this.viewData.findIndex(item => item.customId == chartId);
      if (index > -1) {
        if (this.viewData[index].chartOption.interactData != undefined) {
          let chartOption = JSON.parse(JSON.stringify(this.viewData[index].chartOption));
          chartOption.interactData = act;
        }
        this.$set(this.viewData[index].chartOption, 'interactData', act);
      }
    }.bind(this))


    //交互组件控制组件显隐
    VueEvent.$on('interactShow', function (chartId, act) {
      let chart = this.viewData.find(item => item.customId == chartId);
      if (chart != null) {
        if (act == 'show') {
          document.getElementById(chartId).style.visibility = 'visible'
        } else {
          document.getElementById(chartId).style.visibility = 'hidden'
        }
      }
    }.bind(this))

    //控制指定全局数据源刷新
    VueEvent.$on('refreshGlobal', function (name, isNotLoad) {
      if (Array.isArray(name)) {
        let optionArr = []
        for (let i = 0; i < this.viewTheme.globalData.length; i++) {
          let tmpOption = this.viewTheme.globalData[i];
          if (name.includes(tmpOption.name)) {
            optionArr.push(tmpOption)
          }
        }
        this.initDataName(optionArr, isNotLoad);
      } else {
        for (let i = 0; i < this.viewTheme.globalData.length; i++) {
          let tmpOption = this.viewTheme.globalData[i];
          if (name && tmpOption.name == name) {
            this.initDataName(tmpOption, isNotLoad);
          }
        }
      }
    }.bind(this))

  },
  computed: {
    noSvgGroupDraw() {
      let list = this.viewData.filter(row => row.chartTypeGroup != 'svgGroup')
      return list
    },
    svgGroupDraw() {
      let list = this.viewData.filter(row => row.chartTypeGroup == 'svgGroup')
      return list
    },
    canvasStyle() {
      let background = this.viewTheme.bgImage != '' ? `url(${this.viewTheme.bgImage}) no-repeat` : this.viewTheme.bgColor
      return {
        width: this.viewTheme.isSelfAdaption ? this.screenWidth + "px" : this.viewTheme.panelWidth + "px",
        height: this.viewTheme.isSelfAdaption ? this.screenHeight + "px" : this.viewTheme.panelHeight + "px",
        background: background,
        position: 'relative',
        overflow: 'hidden',
      };
    },
    appContainerStyle() {
      let background = this.viewTheme.bgImage != '' ? `url(${this.viewTheme.bgImage}) no-repeat` : this.viewTheme.bgColor
      return { background: background }
    },
  },
  mounted() {
    //监听下钻事件
    VueEvent.$on("drill_down_msg", data => {
      this.drillDownDialogData = data;
      this.drillDownDialogFlag = true;
    })

    //初始化数据源
    // setTimeout(()=>{
    this.initDataSource();
    // },200)
    this.checkDeviceType();
    window.addEventListener('resize', this.checkDeviceType);
  },
  beforeDestroy() {
    for (let i = this.globalTimers.length; i >= 0; i--) {
      clearInterval(this.globalTimers[i]);
    }
    window.removeEventListener('resize', this.checkDeviceType);
  },
  methods: {
    setAlLoadData(val) {
      this.alLoadData = val
    },
    async initTypeApply() {
      // let viewData = JSON.parse(localStorage.getItem("viewdata"));
      let viewData = JSON.parse(LZString.decompress(localStorage.getItem('viewdata')))
      if (viewData.DeviceType === 'double') {
        this.viewTheme = viewData.themeForm[this.doubleType]
        this.viewTheme.globalData = viewData.themeForm.globalData
      } else {
        this.viewTheme = viewData.themeForm;
      }
      //获取编辑页宽高
      let rectWidth = this.viewTheme.panelWidth;
      let rectHeight = this.viewTheme.panelHeight;
      //是否自适应
      let isSelfAdaption = this.viewTheme.isSelfAdaption;
      //自适应类型
      let adaptionType = this.viewTheme.adaptionType;

      //是自适应
      if (isSelfAdaption) {
        //全自适应
        if (viewData.DeviceType === 'double') {
          if (typeof adaptionType == 'undefined' || adaptionType == '0') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList[this.doubleType].forEach(item => {
              item.width = (item.width / rectWidth) * this.screenWidth;
              item.height = (item.height / rectHeight) * this.screenHeight;
              item.x = (item.x / rectWidth) * this.screenWidth;
              item.y = (item.y / rectHeight) * this.screenHeight;
            });
          }
          //宽度自适应
          else if (adaptionType == '1') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList[this.doubleType].forEach(item => {
              item.width = (item.width / rectWidth) * this.screenWidth;
              item.x = (item.x / rectWidth) * this.screenWidth;
            });
          }
          //高度自适应
          else if (adaptionType == '2') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList[this.doubleType].forEach(item => {
              item.height = (item.height / rectHeight) * this.screenHeight;
              item.y = (item.y / rectHeight) * this.screenHeight;
            });
          }
        } else {
          if (typeof adaptionType == 'undefined' || adaptionType == '0') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList.forEach(item => {
              item.width = (item.width / rectWidth) * this.screenWidth;
              item.height = (item.height / rectHeight) * this.screenHeight;
              item.x = (item.x / rectWidth) * this.screenWidth;
              item.y = (item.y / rectHeight) * this.screenHeight;
            });
          }
          //宽度自适应
          else if (adaptionType == '1') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList.forEach(item => {
              item.width = (item.width / rectWidth) * this.screenWidth;
              item.x = (item.x / rectWidth) * this.screenWidth;
            });
          }
          //高度自适应
          else if (adaptionType == '2') {
            //遍历组件重新计算自适应宽高
            viewData.drawingList.forEach(item => {
              item.height = (item.height / rectHeight) * this.screenHeight;
              item.y = (item.y / rectHeight) * this.screenHeight;
            });
          }
        }
      }
      if (viewData.DeviceType === 'double') {
        this.viewData = viewData.drawingList[this.doubleType];
      } else {
        this.viewData = viewData.drawingList;
      }
    },
    initDataSource() {
      for (let i = 0; i < this.viewTheme.globalData.length; i++) {
        let tmpOption = this.viewTheme.globalData[i];
        if (tmpOption.timeout > 0) {
          let timerTask = () => {

            this.globalTimers.push(setInterval(() => {
              // timerTask();
              this.initDataName(tmpOption);
            }, Number(tmpOption.timeout) * 1000));
          };
          timerTask();
        } else {
          console.log(tmpOption, 'tmpOption')
          this.initDataName(tmpOption);
        }
      }
    },
    async initDataName(iptOption, isNotLoad) {
      if (isNotLoad) {
        VueEvent.$emit("GlobalData", '', iptOption);
      } else {
        let initResult = "";
        let newiptOption = await everyOngetData(iptOption, this.viewTheme.globalData, this.viewData)
        let curitem = null
        this.viewTheme.globalData.find((x, inx) => {
          if (x.name == newiptOption.name) {
            curitem = inx
          }
        });
        if (curitem != null) {
          this.$set(this.viewTheme.globalData, curitem, newiptOption);
        }
        VueEvent.$emit("GlobalData", initResult, newiptOption);
      }

    },
    chartName(item) {
      if (item.chartType == "text") {
        return "NormalText";
      }
      else if (item.chartType == "lamp") {
        return "LampText";
      }
      else if (item.chartType == "date") {
        return "DateText";
      }
      else if (item.chartType == "textCheckBox") {
        return "TextCheckBox";
      }
      else {
        return item.chartType + "Chart";
      }
    },
    chartStyle(val) {
      return {
        //transform: translate(val.x + "px", val.y + "px"),
        left: val.x + "px",
        top: val.y + "px",
        width: val.width + "px",
        height: val.height + "px",
        position: 'absolute',
        'z-index': val.zindex
      }
    },
    checkDeviceType() {
      const mobile = this.isMobileDevice();
      if (mobile) {
        this.doubleType = 'phone';
        this.initTypeApply()
      } else {
        this.doubleType = 'pc';
        this.initTypeApply()
      }
    },
    isMobileDevice() {//判断当前设备是否为移动端
      const ua = navigator.userAgent.toLowerCase();
      const t1 = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(
        ua
      );
      // const t2 = ua.match("iphone") && navigator.maxTouchPoints > 1;
      return t1;
    }
  },
};
</script>

<style>
#svgDraw {
  position: absolute;
  left: 0;
  top: 0;
  height: 100%;
  width: 100%;
}
</style>