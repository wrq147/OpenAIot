<template>
  <div class="container" id="container">
    <div class="left-board">
      <div class="logo-wrapper">
        <el-button
          @click="exit"
          size="small"
          style="margin-right: 10px"
          icon="el-icon-back"
          circle
        ></el-button>
        <span class="logo">{{ this.form.Name }}</span>
      </div>

      <!-- 左侧组件图层 -->

      <el-scrollbar class="left-scrollbar">
        <!-- <draggable item-key="id" v-model="drawingList" ghostClass="ghost">
          <template #item="{ element }">
              <div>{{ element }}</div>
          </template>
        </draggable> -->
        <layer-items
          ref="leftTab"
          :isStartLoading="isStartLoading"
          :activeData="activeData"
          :drawingList="drawingList"
          :activeId="activeId"
          :themeForm="themeForm"
          @active-index-change="activeIndexChange"
          @activeChange="activeChange"
          @closeLoading="closeLoading"
        ></layer-items>
      </el-scrollbar>
    </div>

    <div class="center-board">
      <div class="action-bar">
        <!-- 顶部左侧导航  -->
        <div style="display:flex;justify-content:space-between;" id="justify_con">
          <div style="width: 55%;position: relative;top: 50%;transform: translateY(0%);">
            <el-menu class="el-menu-demo" mode="horizontal">
              <el-submenu index="1">
                <template slot="title"><i class="el-icon-pie-chart"></i>图表</template>
                <div v-for="(item, i) in chartsComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val)">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item)" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>

              <el-submenu index="2">
                <template slot="title"><i class="el-icon-tickets"></i>文本</template>

                <div v-for="(item, i) in textComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val)">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item)" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>

              <el-submenu index="3">
                <template slot="title"><i class="el-icon-picture-outline"></i>媒体</template>

                <div v-for="(item, i) in mediaComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val)">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item)" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>

              <el-submenu index="6">
                <template slot="title"><i class="el-icon-s-grid"></i>表格</template>

                <div v-for="(item, i) in tableComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val)">
                      <i class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item)" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>

              <el-submenu index="7">
                <template slot="title"><i class="el-icon-news"></i>条件</template>

                <div v-for="(item, i) in factorComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val)">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item)" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>
              <el-submenu index="8">
                <template slot="title"><i class="el-icon-datav-iot-groups"></i>组态</template>

                <div v-for="(item, i) in configurationComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val, null, 'configurate')">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item, null, 'configurate')" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>
              <el-submenu index="9">
                <template slot="title"><i class="el-icon-datav-others"></i>其他</template>

                <div v-for="(item, i) in otherComponents" :key="i">
                  <el-submenu :index="item.chartType" v-if="item.child">
                    <template slot="title"><i :class="item.icon"></i>{{ item.layerName }}</template>
                    <el-menu-item :index="val.chartType" v-for="val in item.child" :key="val.chartType" @click="addComponent(val, null, 'configurate')">
                      <i :class="val.icon"></i><span>{{ val.layerName }}</span>
                    </el-menu-item>
                  </el-submenu>
                  <el-menu-item :index="item.customId" @click="addComponent(item, null, 'configurate')" v-else>
                    <i :class="item.icon"></i><span slot="title">{{ item.layerName }}</span>
                  </el-menu-item>
                </div>
              </el-submenu>
            </el-menu>
          </div>

          <!-- 顶部右侧导航 -->
          <div style="width: 45%">
            <topHandleNav :undoStyle="undoStyle" :redoStyle="redoStyle" :justifywidth="justifywidth" :key="topHandleNavKey" @returnHandleSelect="returnHandleSelect" @importFile="importFile"></topHandleNav>
          </div>
        </div>

        <!-- 比例组件 默认60 -->
        <div class="scaleCls">
          <el-radio-group v-if="form.DeviceType === 'double'" v-model="doubleType" style="margin-right: 30px" @change="doubleTypeChange('click', $event)">
            <el-radio-button label="pc">电脑</el-radio-button>
            <el-radio-button label="phone">手机</el-radio-button>
          </el-radio-group>
          <el-input-number v-model="displayscale" :min="60" :max="100" :step="10" label="比例"/>
        </div>
      </div>
      <el-row class="center-board-row">
        <div class="wrapper" id="wrapper">
          <!-- 标尺插件 -->
          <SketchRule
            v-if="showSke"
            :lang="lang"
            :thick="thick"
            :scale="scale"
            :width="width"
            :height="height"
            :startX="startX"
            :startY="startY"
            :shadow="shadow"
            :horLineArr="lines.h"
            :verLineArr="lines.v"
            :cornerActive="true"
            @handleLine="handleLine"
            @onCornerClick="handleCornerClick"
          >
          </SketchRule>
          <div ref="screensRef" id="screens" @wheel="handleWheel" @scroll="handleScroll">
            <div ref="containerRef" class="screen-container" @mouseup="overDrag">
              <div id="canvas" :style="canvasStyle" ref="canvas">
                <vue-drag-items
                  ref="dragItems"
                  :drawing-list="drawingList"
                  :theme="themeForm.themeColor"
                  :scale="scale"
                  :activeId="activeId"
                  @boxSelected="onBoxSelected"
                  @bgClick="canvasClick"
                  @selected="activeIndexChange"
                />
              </div>
            </div>
          </div>
        </div>
      </el-row>
    </div>

    <right-panel :activeData="activeData" :themeForm="themeForm" :drawing-list="drawingList" @activeChange="activeChange"/>
    <reportWarn ref="reportWarn" :globalData="themeForm.globalData"></reportWarn>
    <!-- 数据大屏保存参数配置对话框 -->
    <el-dialog title="保存" :visible.sync="open" width="700px" append-to-body :close-on-click-modal="false">
      <el-form ref="form" :model="form" :rules="rules" label-width="80px" v-loading="saveLoading">
        <el-row>
          <el-col :span="12">
            <el-form-item label="报表名称" prop="Name" required>
              <el-input v-model="form.Name" placeholder="请输入报表名称" />
            </el-form-item>
          </el-col>

          <el-col :span="24">
            <el-form-item label="备注">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm('click')" :disabled="saveLoading">确 定</el-button>
        <el-button @click="cancel" :disabled="saveLoading">取 消</el-button>
      </div>
    </el-dialog>

    <chart-select-panel ref="chartSelector" @add="addComponent"></chart-select-panel>
  </div>
</template>

<script>
import draggable from "vuedraggable";
import echarts from "echarts";
import RightPanel from "./RightPanel";
import VueDragItems from "./VueDragItems";
import LayerItems from "./LayerItems/index";
import SketchRule from "vue-sketch-ruler";
import ChartSelectPanel from "./ChartSelectPanel";

// 引入html2canvas
import html2canvas from "html2canvas";

import {
  chartsComponents,
  textComponents,
  mediaComponents,
  tableComponents,
  factorComponents,
  getNewDefaultForm,
  configurationComponents,
  otherComponents,
} from "./ComponentsConfig";
import VueEvent from "./VueEvent";
import { rptInfo, editRpt } from "@/api/report/report";
import { GetDataSourceByIds } from "@/api/report/sourse";
// import { getFormsource } from "@/api/report/formsource";
// import { chartBIanalysis } from "@/api/report/sourse";
// import { chartApi } from "@/api/report/chartApi";
// import { replaceLinkParam } from "./util/LinkageChart";
import pptxgen from "pptxgenjs";

import Vue from "vue";
import Contextmenu from "vue-contextmenujs";
Vue.use(Contextmenu);

import dataV from "@jiaminghi/data-view";
Vue.use(dataV);
//let rectWidth = 1920;
//let rectHeight = 1080;
import {
  confirmValue,
  getbaseData,
  combinationTableColum,
  combinationConfirmValue,
  everyOngetData,
} from "@/views/report/datav/LayerItems/commonRuning";
let defaultForm = getNewDefaultForm();
import topHandleNav from "@/views/report/datav/LayerItems/rightnav";
import reportWarn from "@/views/report/datav/datawarn/warningList";
import LZString from 'lz-string';
export default {
  components: {
    draggable,
    RightPanel,
    VueDragItems,
    LayerItems,
    SketchRule,
    html2canvas,
    ChartSelectPanel,
    topHandleNav,
    reportWarn
  },
  data() {
    return {
      chartsComponents,
      textComponents,
      mediaComponents,
      tableComponents,
      factorComponents,
      configurationComponents,
      otherComponents,
      // 是否显示弹出层
      open: false,
      // 表单参数
      form: {},
      //主题设置
      themeForm: defaultForm["pc"],
      rectWidth: defaultForm["pc"].panelWidth,
      rectHeight: defaultForm["pc"].panelHeight,
      isSelfAdaption: true,
      //全局id
      idGlobal: 0,
      //全局z-index
      zindex: 0,
      //绘画列表
      //drawingList: chartsComponents,
      drawingList: [],
      //活跃组件id
      activeId: defaultForm["pc"].customId,
      //活跃组件数据
      activeData: defaultForm["pc"],
      scale: 0.6, //初始化标尺的缩放
      startX: 0,
      startY: 0,
      showSke: true,
      lines: {
        h: [],
        v: [],
      },
      thick: 20, //标尺厚度
      width: 500,
      height: 400,
      lang: "zh-CN", // 中英文
      isShowRuler: true, // 显示标尺
      isShowReferLine: true, // 显示参考线
      // 表单校验
      rules: {
        Name: [{ required: true, message: "名称不能为空", trigger: "blur" }],
      },
      //保存大屏loading
      saveLoading: false,
      loadingQuery: null, //加载中的设置参数
      isStartLoading: false, //是否开始加载数据
      doubleType: "pc", // 区分电脑端还是手机端
      oldDoubleType: "pc",
      doubleThemeOption: {
        pc: defaultForm["pc"],
        phone: defaultForm["phone"],
      }, // 双端数据处理
      justifywidth:0,
      topHandleNavKey:1,
      reportId:'',//报表Id
      sourseList: [] // 数据库列表
    };
  },
  watch: {
    activeData: {
      deep: true,
      handler(newVal) {
        this.activeData = newVal;
        this.activeId = newVal.customId;
        if (this.activeData.chartType == "themeForm") {
          this.themeForm.isSelfAdaption = this.activeData.isSelfAdaption;
          this.themeForm.adaptionType = this.activeData.adaptionType;
          this.rectWidth = this.activeData.panelWidth;
          this.rectHeight = this.activeData.panelHeight;
          this.themeForm.panelWidth = this.activeData.panelWidth;
          this.themeForm.panelHeight = this.activeData.panelHeight;
          this.themeForm.bgColor = this.activeData.bgColor;
          this.themeForm.bgImage = this.activeData.bgImage;
          this.themeForm.themeColor = this.activeData.themeColor;
        }
        // this.activeData.globalData = this.themeForm.globalData;
        // this.activeData.dataChartOption = this.themeForm.chartOption;
      },
    },
    "themeForm.themeColor": {
      handler(val) {
        //修改主题
        echarts.registerTheme("customTheme", val);
        VueEvent.$emit("themeChange", val);
        this.initDataSource();
      },
    },
    "themeForm.globalData": {
      handler(val) {
        // this.initDataName(val);
      },
    },
    drawingList: {
      deep: true,
      handler(newVal) {
        this.$store.commit("recordSnapshot", this.drawingList);
      },
    },
  },
  computed: {
    displayscale: {
      get() {
        return parseInt(this.scale * 100);
      },
      set(newval) {
        this.scale = newval / 100.0;
        this.showSke = false;
        this.$nextTick(() => {
          this.showSke = true;
          this.handleScroll();
        });
      },
    },
    shadow() {
      return {
        x: 0,
        y: 0,
        width: this.rectWidth,
        height: this.rectHeight,
      };
    },
    canvasStyle() {
      let background =
        this.themeForm.bgImage != ""
          ? `url(${this.themeForm.bgImage}) no-repeat`
          : this.themeForm.bgColor;
      return {
        width: this.rectWidth + "px",
        height: this.rectHeight + "px",
        transform: `scale(${this.scale})`,
        background: background,
      };
    },
    undoStyle() {
      return this.$store.state.snapshot.snapshotIndex <= 0;
    },
    redoStyle() {
      return (
        this.$store.state.snapshot.snapshotData.length ==
        this.$store.state.snapshot.snapshotIndex + 1
      );
    },
  },
  async created() {
    this.loadingQuery = this.$loading({
      //进入页面设置加载中效果，方便完成页面保存数据的初始化
      lock: true,
      text: "Loading",
      spinner: "el-icon-loading",
      background: "rgba(0, 0, 0, 1)",
      target: document.getElementById("container"),
    });
    this.$nextTick(() => {
      document.querySelector(".el-loading-mask").style.zIndex = "999999";
    });
    import("./styles/icon.css");

    defaultForm = getNewDefaultForm();
    //设置快照组件数据为空
    this.$store.commit("getComponentData", []);
    this.$store.commit("resetSnapshotIndex", []);
    // await this.getRptList(); // 获取数据库列表
    await this.initDataDraw();
    // await this.loadInitalData();
    this.isStartLoading = true;
    VueEvent.$on(
      "tabchange",
      function (chartList) {
        if (chartList != null) {
          //根据tab标签绑定组件设置组件的显示情况
          for (const obj of this.drawingList) {
            const chart = chartList.filter(function (item) {
              return item.customId == obj.customId;
            })[0];

            if (chart != null) {
              obj.isShow = chart.isShow;
              //obj.chartOption.animate = chart.animate;
            }
          }
        }
      }.bind(this)
    ),
      VueEvent.$on(
        "removetab",
        function (customId) {
          //删除tab标签行，恢复绑定组件显示情况
          const chart = this.drawingList.filter(function (item) {
            return item.customId == customId;
          })[0];

          if (chart != null) {
            chart.isShow = true;
          }
        }.bind(this)
      );

    //控制指定全局数据源刷新
    VueEvent.$on(
      "refreshGlobal",
      function (name) {
        if (Array.isArray(name)) {
          let optionArr = [];
          for (let i = 0; i < this.themeForm.globalData.length; i++) {
            let tmpOption = this.themeForm.globalData[i];
            if (name.includes(tmpOption.name)) {
              optionArr.push(tmpOption);
            }
          }
          this.initDataName(optionArr);
        } else {
          for (let i = 0; i < this.themeForm.globalData.length; i++) {
            let tmpOption = this.themeForm.globalData[i];
            if (name && tmpOption.name == name) {
              this.initDataName(tmpOption);
            }
          }
        }
      }.bind(this)
    );
  },
  methods: {
    returnHandleSelect(key){
      //点击导航返回
      if(key){
        switch (key) {
				    case 'undo':
					    this.undo()
				      break;
            case 'redo':
					    this.redo()
				      break;
            case 'undo':
					    this.undo()
				      break;
            case 'view':
					    this.view()
				      break;
            case 'handleCommand-image':
					    this.handleCommand('image')
				      break;
            case 'handleCommand-word':
					    this.handleCommand('word')
				      break;
            case 'handleCommand-ppt':
					    this.handleCommand('ppt')
				      break;
            case 'handleCommand-outJson':
					    this.handleCommand('outJson')
				      break;
            case 'handleCommand-inJson':
					    this.handleCommand('inJson')
				      break;
            case 'clear':
					    this.clear()
				      break;
            case 'saveScreen':
					    this.saveScreen()
				      break;
            case 'warning':
					    this.warning()
				      break;
            default:  
              return
        }
      }
    },
    warning(){
      //数据预警
      this.$refs.reportWarn.openDialog(this.reportId)
    },
    async loadInitalData() {
      await this.getRptList(this.themeForm.globalData);
      this.themeForm.globalData = await this.initBasicInfo(this.themeForm.globalData);
      this.isFirstLoading = true;
      for (let i = 0; i < this.themeForm.globalData.length; i++) {
        let tmpoption = this.themeForm.globalData[i];
        let ddtype = tmpoption.dataSourceType;
        if (ddtype === "url") {
          let rawData = await confirmValue(tmpoption, this.drawingList,this.themeForm.globalData);
          tmpoption.rawData = JSON.stringify(rawData);
          this.$set(this.themeForm.globalData, i, tmpoption);
        } else if (ddtype === "database") {
          try {
              let rowGlobal = await getbaseData(tmpoption,this.themeForm);
              rowGlobal.rawData = JSON.stringify(rowGlobal.rawData);
              this.$set(this.themeForm.globalData, i, rowGlobal);
            } catch (error) {
              this.closeLoading();
            }
         
        } else if (ddtype === "combination") {
          let tableColum = await combinationTableColum(
            tmpoption,
            this.themeForm.globalData,
            this.drawingList
          );
          tmpoption.combinationTable = tableColum;
          let rawData = combinationConfirmValue(tmpoption,this.themeForm.globalData);
          tmpoption.rawData = JSON.stringify(rawData);
          this.$set(this.themeForm.globalData, i, tmpoption);
        }
      }
      this.isFirstLoading = false;
      // this.$emit("closeLoading");
      this.$forceUpdate()
      this.closeLoading();
    },
    // 获取数据库列表
    async getRptList(items) {
      try {
        const ids = items.filter(item => item.dataSourceType === 'database').map(item => item.database.sourseItem);
        const idsData = [...new Set(ids)];
        const response = await GetDataSourceByIds(idsData);
        this.sourseList = response.data;
      } catch (error) {
        this.sourseList = [];
      }
    },
    // 初始化数据库基本信息
    async initBasicInfo(globalData) {
      for (const item of this.sourseList) {
        for (const v of globalData) {
          if (v.dataSourceType === 'database') {
            if (item.Id === v.database.sourseItem) {
              v.database.username = item.UserName;
              v.database.type = item.DatabaseType;
              v.database.ipAdress = item.IpAddress;
              v.database.baseName = item.DatabaseName;
              v.database.password = item.Password;
              v.database.port = item.Port;
            }
          }
        }
      }
      return globalData
    },


    initDataSource() {
      for (let i = 0; i < this.themeForm.globalData.length; i++) {
        this.initDataName(this.themeForm.globalData[i]);
      }
      if (this.themeForm.globalData && this.themeForm.globalData.length > 0) {
        // this.$nextTick(()=>{
        //   this.$refs.leftTab.loadInitalData()//初始化数据
        // })
      } else {
        this.loadingQuery.close();
      }
    },
    closeLoading() {
      this.$nextTick(() => {
        this.initDataSource();
        this.$forceUpdate()
      });
      this.loadingQuery.close();
    },
    async initDataName(iptOption) {
      let initResult = "";
      setTimeout(()=>{VueEvent.$emit("GlobalData", initResult, iptOption);},200)
    },
    onBoxSelected(rect) {
      if (rect.x < 0) rect.x = 0;
      if (rect.x > this.rectWidth) return;
      if (rect.y < 0) rect.y = 0;
      if (rect.y > this.rectHeight) return;

      if (rect.w < 40) return;
      if (rect.h < 40) return;
      if (rect.w > this.rectWidth) {
        rect.w = this.rectWidth;
      }
      if (rect.h > this.rectHeight) {
        rect.h = this.rectHeight;
      }

      this.$refs.chartSelector.showDialog(rect);
    },
    overDrag(e) {
      let rect = this.$refs.canvas.getBoundingClientRect();
      let newEvent = {
        pageX: e.pageX,
        pageY: e.pageY,
      };
      let top = rect.top; //上边界
      let left = rect.left; //左边界
      let right = rect.right; //右边界
      let bottom = rect.bottom; //下边界
      if (newEvent.pageX < left) {
        newEvent.pageX = left;
      }
      if (newEvent.pageX > right) {
        newEvent.pageX = right;
      }
      if (newEvent.pageY < top) {
        newEvent.pageY = top;
      }
      if (newEvent.pageY > bottom) {
        newEvent.pageY = bottom;
      }

      this.$refs.dragItems.handMouseUp(newEvent);
    },
    exit() {
      this.$confirm("未发布的内容将不会被保存，是否直接退出 ?", "提示", {
        confirmButtonText: "退出",
        cancelButtonText: "取消",
        type: "warning",
      }).then(() => {
        this.$store.dispatch("tagsView/delView", this.$route);
        this.$router.go(-1);
      });
    },
    handleOpen(key, keyPath) {},
    handleClose(key, keyPath) {},
    handleLine(lines) {
      this.lines = lines;
    },
    handleCornerClick() {
      return;
    },
    handleScroll() {
      const screensRect = document
        .querySelector("#screens")
        .getBoundingClientRect();
      const canvasRect = document
        .querySelector("#canvas")
        .getBoundingClientRect();
      // 标尺开始的刻度
      const startX =
        (screensRect.left + this.thick - canvasRect.left) / this.scale;
      const startY =
        (screensRect.top + this.thick - canvasRect.top) / this.scale;
      this.startX = startX >> 0;
      this.startY = startY >> 0;

    },
    // 控制缩放值
    handleWheel(e) {
      if (e.ctrlKey || e.metaKey) {
        e.preventDefault();
        const nextScale = parseFloat(
          Math.max(0.2, this.scale - e.deltaY / 500).toFixed(2)
        );
        this.scale = nextScale;
        this.showSke = false;
        this.$nextTick(() => {
          this.showSke = true;
          this.handleScroll();
        });
      } else {
        this.$nextTick(() => {
          this.handleScroll();
        });
      }
    },
    initSize() {
      const wrapperRect = document
        .querySelector("#wrapper")
        .getBoundingClientRect();
      const borderWidth = 1;
      this.width = wrapperRect.width - this.thick - borderWidth;
      this.height = wrapperRect.height - this.thick - borderWidth;
    },
    //添加控件
    addComponent(item, rect, type) {
      let clone = this.cloneComponent(item);
      if (clone.chartType === "group") {
        clone.bindingDiv = clone.customId;
        clone.zindex = 0;
        clone.chartOption.forEach((element) => {
          element.customId = ++this.idGlobal;
          element.zindex = ++this.zindex;
          element.chartOption.bindingDiv = element.customId;
          element.layerName = element.layerName + element.customId;
          this.drawingList.push(element);
        });
      }
      if (rect != null) {
        clone.x = rect.x;
        clone.y = rect.y;
        clone.width = rect.w;
        clone.height = rect.h;
      }
      if (type && type == "configurate") {
        clone.comptType = "configurate";
      } else {
        clone.comptType = "normal";
      }
      // clone.globalData = this.themeForm.globalData?this.themeForm.globalData:[];
      // clone.dataChartOption = this.themeForm.chartOption?this.themeForm.chartOption:{};
      this.drawingList.push(clone);
      this.activeId = clone.customId;
      this.activeData = clone;
    },
    cloneComponent(origin) {
      const clone = JSON.parse(JSON.stringify(origin));
      clone.customId = ++this.idGlobal;
      clone.zindex = ++this.zindex;
      clone.layerName = clone.layerName + clone.customId;
      if (origin.chartType !== "group") {
        clone.chartOption.bindingDiv = clone.customId;
      }
      // clone.globalData = this.themeForm.globalData?this.themeForm.globalData:[];
      // clone.dataChartOption = this.themeForm.chartOption?this.themeForm.chartOption:{};
      return clone;
    },
    activeChange(value) {
      this.activeData = value;
      this.activeId = value.customId;
    },
    activeIndexChange(newVal) {
      this.activeId = newVal;
      this.activeData = this.drawingList.filter(function (item) {
        return item.customId == newVal;
      })[0];
    },
    canvasClick() {
      VueEvent.$emit("clear_ctrl");
      this.activeId = this.themeForm.customId;
      this.activeData = this.themeForm;
    },
    // 预览
    view() {
      let viewData = {};
      if (this.form.DeviceType === "double") {
        this.doubleThemeOption[this.doubleType] = JSON.parse(
          JSON.stringify(this.themeForm)
        );
        this.doubleThemeOption.globalData = this.themeForm.globalData;
        delete this.doubleThemeOption[this.oldDoubleType].globalData;
        let drawingList = JSON.parse(this.form.DrawOption);
        drawingList[this.doubleType] = this.drawingList;
        viewData = {
          themeForm: this.doubleThemeOption,
          drawingList: drawingList,
          DeviceType: this.form.DeviceType,
        };
      } else {
        viewData = {
          themeForm: this.themeForm,
          drawingList: this.drawingList,
          DeviceType: this.form.DeviceType,
        };
      }
      // localStorage.setItem("viewdata", JSON.stringify(viewData));
      const compressed = LZString.compress(JSON.stringify(viewData));
      localStorage.setItem("viewdata", compressed);
      const viewRuter = this.$router.resolve({
        path: "/report/datav/datavView",
      });
      window.open(viewRuter.href, "_blank");
      // this.$router.push({path: "/report/datav/datavView"})
    },
    //复制
    copyComponent(comId) {
      let copyTag = this.drawingList.filter(function (item) {
        return item.customId == comId;
      })[0];
      let clone = this.cloneComponent(copyTag);
      if (clone.chartType == "group") {
        clone.bindingDiv = clone.customId;
        clone.zindex = 0;
        clone.chartOption.forEach((element) => {
          element.customId = ++this.idGlobal;
          element.zindex = ++this.zindex;
          element.layerName = element.layerName + element.customId;
          element.chartOption.bindingDiv = element.customId;
          this.drawingList.push(element);
        });
      } else {
        clone.chartOption.isGroup = false;
      }
      // clone.globalData = this.themeForm.globalData?this.themeForm.globalData:[];
      // clone.dataChartOption = this.themeForm.chartOption?this.themeForm.chartOption:{};
      this.drawingList.push(clone);
      this.activeId = clone.customId;
      this.activeData = clone;
    },
    //删除
    deleteComponent(comId) {
      //判断如果是组合，先将组合里的组件存放，再删除组合之后删除组件
      const deleteChart = this.drawingList.filter(function (item) {
        return item.customId == comId;
      })[0];
      if (deleteChart != null) {
        //判断如果是组合，并且删除组合里的组件
        if (deleteChart.chartType === "group") {
          deleteChart.chartOption.forEach((element) => {
            this.drawingList.some((chart, j) => {
              if (chart.customId == element.customId) {
                this.drawingList.splice(j, 1);
              }
            });
          });
        }
        this.drawingList.some((item, i) => {
          if (item.customId == comId) {
            if (item.chartType === "tab") {
              let bindingObjs = item.chartOption.bindingObjs;
              if (bindingObjs != null && bindingObjs.length > 0) {
                //如果是新版选项卡绑定多组件，则循环解绑，全部显示
                for (const obj of bindingObjs) {
                  if (
                    typeof obj.chartids != "undefined" &&
                    obj.chartids != ""
                  ) {
                    obj.chartids.forEach((element) => {
                      const chart = this.drawingList.filter(function (item) {
                        return item.customId == element;
                      })[0];
                      if (chart != null) {
                        chart.isShow = true;
                      }
                    });
                  } else {
                    //如果是旧版选项卡绑定单组件，则单个组件解绑显示
                    const chart = this.drawingList.filter(function (item) {
                      return item.customId == obj.chartid;
                    })[0];

                    if (chart != null) {
                      obj.isShow = true;
                    }
                  }
                }
              }
            }
            //如果是组合里的组件，则在组合中删除该组件
            else if (item.chartOption.isGroup == true) {
            }
            this.drawingList.splice(i, 1);
            // 在数组的some方法中，如果return true，就会立即终止这个数组的后续循环,所以相比较foreach，如果想要终止循环，那么建议使用some
            return true;
          }
        });

        this.$nextTick(() => {
          this.activeId = this.themeForm.customId;
          this.activeData = this.themeForm;
        });
      }
    },
    //置顶
    topLayer(comId) {
      //根据comId找到当前目标对象
      const tagNode = JSON.parse(
        JSON.stringify(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        )
      );
      if (tagNode.chartOption.isGroup) {
        this.$message({
          message: "请先解散组件再移动图层！",
          type: "warning",
        });
      } else if (tagNode.chartType != "group") {
        //找到drawingList集合中zindex的最大值
        const maxIndex = Math.max.apply(
          Math,
          this.drawingList.map((item) => {
            return item.zindex;
          })
        );
        //在drawingList中找到所有大于当前目标对象的数组集合进行置顶修改

        this.drawingList.forEach((item) => {
          const position = this.drawingList.indexOf(item);
          if (item.zindex == tagNode.zindex) {
            item.zindex = maxIndex;
          } else if (item.zindex > tagNode.zindex) {
            item.zindex = item.zindex - 1;
          }
          this.$set(this.drawingList, position, item);
        });
      }
    },
    //置底
    bottomLayer(comId) {
      //根据comId找到当前目标对象
      const tagNode = JSON.parse(
        JSON.stringify(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        )
      );
      if (tagNode.chartOption.isGroup) {
        this.$message({
          message: "请先解散组件再移动图层！",
          type: "warning",
        });
      } else if (tagNode.chartType != "group") {
        //找到drawingList集合中zindex的最大值
        const minIndex = Math.min.apply(
          Math,
          this.drawingList.map((item) => {
            return item.zindex;
          })
        );
        //在drawingList中找到所有大于当前目标对象的数组集合进行置顶修改

        this.drawingList.forEach((item) => {
          const position = this.drawingList.indexOf(item);
          if (item.zindex == tagNode.zindex) {
            item.zindex = minIndex;
          } else if (item.zindex < tagNode.zindex) {
            item.zindex = item.zindex + 1;
          }
          this.$set(this.drawingList, position, item);
        });
      }
    },
    //上移一层
    upLayer(comId) {
      //根据comId找到当前目标对象
      const tagNode = JSON.parse(JSON.stringify(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        )
      );
      if (tagNode.chartOption.isGroup) {
        this.$message({
          message: "请先解散组件再移动图层！",
          type: "warning",
        });
      } else if (tagNode.chartType != "group") {
        const tagNodePosition = this.drawingList.indexOf(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        );

        //找出上一层节点和节点位置
        const nextNode = this.drawingList
          .filter(function (item) {
            return item.zindex > tagNode.zindex;
          })
          .sort((a, b) => {
            return a.zindex - b.zindex;
          })[0];

        const nextNodePosition = this.drawingList.indexOf(nextNode);

        if (nextNode != null) {
          const tagZindex = tagNode.zindex;
          tagNode.zindex = nextNode.zindex;
          nextNode.zindex = tagZindex;
          this.$set(this.drawingList, tagNodePosition, nextNode);
          this.$set(this.drawingList, nextNodePosition, tagNode);
        }
      }
    },
    //下移一层
    downLayer(comId) {
      //根据comId找到当前目标对象
      const tagNode = JSON.parse(
        JSON.stringify(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        )
      );
      if (tagNode.chartOption.isGroup) {
        this.$message({
          message: "请先解散组件再移动图层！",
          type: "warning",
        });
      } else if (tagNode.chartType != "group") {
        const tagNodePosition = this.drawingList.indexOf(
          this.drawingList.filter(function (item) {
            return item.customId == comId;
          })[0]
        );

        //找出上一层节点和节点位置
        const preNode = this.drawingList
          .filter(function (item) {
            return item.zindex < tagNode.zindex;
          })
          .sort((a, b) => {
            return b.zindex - a.zindex;
          })[0];

        const preNodePosition = this.drawingList.indexOf(preNode);

        if (preNode != null) {
          const tagZindex = tagNode.zindex;
          tagNode.zindex = preNode.zindex;
          preNode.zindex = tagZindex;
          this.$set(this.drawingList, tagNodePosition, preNode);
          this.$set(this.drawingList, preNodePosition, tagNode);
        }
      }
    },
    //右键锁定
    onLocked(comId) {
      //根据comId找到当前目标对象
      const tagNode = this.drawingList.filter(function (item) {
        return item.customId == comId;
      })[0];

      if (tagNode.isUnlocked != false) {
        //没锁给锁上
        if (tagNode.chartOption.length > 1) {
          for (let i = 0; i < tagNode.chartOption.length; i++) {
            this.$set(tagNode, "isUnlocked", false);
            this.$set(tagNode.chartOption[i], "isUnlocked", false);
          }
        }
        this.$set(tagNode, "isUnlocked", false);
      } else {
        //锁上给打开
        if (tagNode.chartOption.length > 1) {
          for (let i = 0; i < tagNode.chartOption.length; i++) {
            this.$set(tagNode, "isUnlocked", true);
            this.$set(tagNode.chartOption[i], "isUnlocked", true);
          }
        }
        this.$set(tagNode, "isUnlocked", true);
      }
    },
    saveScreen() {
      this.open = true;
    },
    /** 提交按钮 */
    submitForm(incident) {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          this.saveScreenData();
        }
      });
    },
    // 保存大屏数据
    saveScreenData(incident) {
      this.saveLoading = true;
      // this.form.ThemeOption = JSON.stringify(this.themeForm);
      let themeForm = JSON.parse(JSON.stringify(this.themeForm));
      this.form.IdGlobal = this.idGlobal;
      this.form.Zindex = this.zindex;
      let drawingList = JSON.parse(JSON.stringify(this.drawingList));
      for (let i = 0; i < drawingList.length; i++) {
        delete drawingList[i].chartOption.finalResult;
      }
      if (this.form.DeviceType === "double") {
        this.form.DrawOption = JSON.parse(this.form.DrawOption);
        this.form.DrawOption[this.oldDoubleType] = drawingList;
        this.form.DrawOption = JSON.stringify(this.form.DrawOption);
        this.doubleThemeOption[this.oldDoubleType] = JSON.parse(
          JSON.stringify(this.themeForm)
        );
        this.doubleThemeOption = this.removeGlobalData(this.doubleThemeOption);
      } else {
        this.form.DrawOption = JSON.stringify(drawingList);
      }
      if (themeForm.globalData) {
        for (let i = 0; i < themeForm.globalData.length; i++) {
          let rawData = JSON.parse(themeForm.globalData[i].rawData);
          for (let j = 0; j < rawData.length; j++) {
            rawData[j].content = [];
          }
          
          for (let j = 0;j < themeForm.globalData[i].combinationTable.length;j++) {
            themeForm.globalData[i].combinationTable[j].resData = [];
            themeForm.globalData[i].combinationTable[j].resProcessData = [];
          }
          themeForm.globalData[i].rawData = JSON.stringify(rawData);
        }
      }
      if (this.form.DeviceType === "double") {
        this.doubleThemeOption.globalData = themeForm.globalData;
        this.form.ThemeOption = JSON.stringify(this.doubleThemeOption);
      } else {
        this.form.ThemeOption = JSON.stringify(themeForm);
      }
      let optionObj = Object.create(null);
      let optionJson = JSON.stringify(optionObj);
      this.form.MapOption = optionJson;
      // 第一个参数是需要生成截图的元素,第二个是自己需要配置的参数,宽高等
      html2canvas(this.$refs.canvas, {
        backgroundColor: null,
        useCORS: true, // 如果截图的内容里有图片,可能会有跨域的情况,加上这个参数,解决文件跨域问题
      })
        .then((canvas) => {
          let url = canvas.toDataURL("image/png");
          this.form.Thumbnail = url;
          editRpt(this.form)
            .then((response) => {
              this.$message({ message: "修改成功", type: "success" });
              this.saveLoading = false;
              this.open = false;
              this.$store.dispatch("tagsView/delView", this.$route);
              if (incident === "click") {
                this.getDoubleType();
                this.oldDoubleType = this.doubleType;
              } else {
                this.$router.go(-1);
              }
            })
            .catch((err) => {
              this.saveLoading = false;
            });
        })
        .catch((err) => {
          this.saveLoading = false;
        });
    },
    // 判断是否有globalData
    removeGlobalData(obj) {
      for (let key in obj) {
        if (obj[key].hasOwnProperty("globalData")) {
          delete obj[key].globalData;
        }
      }
      return obj;
    },
    // 取消按钮
    cancel() {
      this.open = false;
    },
    //清空
    clear() {
      this.$confirm("此操作将删除全部组件, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          this.drawingList = [];
          this.activeData = this.themeForm;
          this.$message({
            type: "success",
            message: "清空成功!",
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消清空操作",
          });
        });
    },
    
    // 获取大屏数据
    async getRptInfo() {
      try {
        let response = await getRptInfo(this.reportId);
        this.form = response.data;
        this.getDoubleType();
        this.idGlobal = this.form.IdGlobal;
        this.zindex = this.form.Zindex;
        this.activeId = this.themeForm.customId;
      } catch (error) {}
    },
    // 编辑大屏数据
    async initDataDraw() {
      //获取路由传来的参数
      let sId = this.$route.query.screenId;
      this.reportId=sId
      try {
        let response = await rptInfo(sId);
        this.form = response.data;
        // console.log(this.form,'this.form');
        this.getDoubleType();
        this.idGlobal = this.form.IdGlobal;
        this.zindex = this.form.Zindex;
        this.activeId = this.themeForm.customId;
      } catch (error) {}
    },
    // 双端切换
    doubleTypeChange(incident, e) {
      if (incident === "click") {
        this.$confirm("切换时数据会清空, 是否保存数据?", "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        })
          .then(() => {
            this.saveScreenData("click");
          })
          .catch(() => {
            this.getDoubleType();
            this.oldDoubleType = e
          });
      }
    },
    // 双端切换数据
    async getDoubleType() {
      if (this.form.DeviceType === "double") {
        this.drawingList = JSON.parse(this.form.DrawOption)[this.doubleType];
        let resarr = JSON.parse(this.form.Resolution)[this.doubleType].split("*");
        this.rectWidth = parseInt(resarr[0]);
        this.rectHeight = parseInt(resarr[1]);
        if (this.form.ThemeOption === "") {
          this.themeForm = defaultForm[this.doubleType];
        } else {
          this.doubleThemeOption = JSON.parse(this.form.ThemeOption);
          this.themeForm = JSON.parse(this.form.ThemeOption)[this.doubleType];
          this.themeForm.globalData = this.doubleThemeOption.globalData;
        }
        if (this.doubleType === "phone") {
          this.displayscale = 100;
        } else {
          this.displayscale = 60;
        }
        this.activeData = this.themeForm;
      } else {
        this.drawingList = JSON.parse(this.form.DrawOption);
        this.themeForm =
          this.form.ThemeOption == ""
            ? defaultForm[this.doubleType]
            : JSON.parse(this.form.ThemeOption);
        let resarr = this.form.Resolution.split("*");
        this.rectWidth = parseInt(resarr[0]);
        this.rectHeight = parseInt(resarr[1]);
      }
      this.activeData = this.themeForm;
      await this.loadInitalData();
    },
    // 导出
    //图片格式转换方法
    dataURLToBlob(dataurl) {
      let arr = dataurl.split(",");
      let mime = arr[0].match(/:(.*?);/)[1];
      let bstr = atob(arr[1]);
      let n = bstr.length;
      let u8arr = new Uint8Array(n);
      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }
      return new Blob([u8arr], { type: mime });
    },
    handleCommand(command) {
      //this.$message('click on item ' + command);
      if (command === "image") {
        //导出图片

        let that = this;
        let a = document.createElement("a");
        html2canvas(this.$refs.canvas, {
          backgroundColor: null,
          useCORS: true, // 如果截图的内容里有图片,可能会有跨域的情况,加上这个参数,解决文件跨域问题
          allowTaint:true
        }).then((canvas) => {
          let dom = document.body.appendChild(canvas);
          a.style.display = "none";
          document.body.removeChild(dom);
          let blob = that.dataURLToBlob(dom.toDataURL("image/png"));
          a.setAttribute("href", URL.createObjectURL(blob));
          //这块是保存图片操作  可以设置保存的图片的信息
          let pname = String(Math.random()).replace(".", "");
          a.setAttribute("download", pname + ".png");
          document.body.appendChild(a);
          a.click();
          URL.revokeObjectURL(blob);
          document.body.removeChild(a);
        });
      } else if (command === "ppt") {
        let that = this;
        // // 1. 创建新演示文稿
        // let pres = new pptxgen();
        // // 2. 添加幻灯片
        // let slide = pres.addSlide();
        // // 3. 向幻灯片中添加一个或多个对象（表格、形状、图像、文本和媒体）
        let imgUrl = "";
        html2canvas(this.$refs.canvas, {
          backgroundColor: null,
          useCORS: true, // 如果截图的内容里有图片,可能会有跨域的情况,加上这个参数,解决文件跨域问题
          allowTaint:true
        }).then((canvas) => {
          let dom = document.body.appendChild(canvas);
          let blob = that.dataURLToBlob(dom.toDataURL("image/png"));
          imgUrl = URL.createObjectURL(blob);
          that.ppt(imgUrl);
        });
        //let textboxText = "Hello World from PptxGenJS!";
        //let textboxOpts = { x: 1, y: 1, color: '363636', fill: { color:'F1F1F1' }, align: "center" };
        //slide.addText(textboxText, textboxOpts);
        //slide.addImage({ path: imgUrl });
        // 4. 保存演示文稿
        //let pname = String(Math.random()).replace(".", "");
        //pres.writeFile(pname + ".pptx");
      }else if(command === "outJson"){
        //导出
        this.exportProcess()
      }
    },
    importFile(josnForm){
      this.form.DeviceType=josnForm.DeviceType;
      this.form.IdGlobal=josnForm.IdGlobal;
      this.form.DrawOption=josnForm.DrawOption;
      this.form.MapOption=josnForm.MapOption;
      this.form.Resolution=josnForm.Resolution;
      this.form.ThemeOption=josnForm.ThemeOption;
      this.getDoubleType();
      this.idGlobal = this.form.IdGlobal;
      this.zindex = this.form.Zindex;
      this.activeId = this.themeForm.customId;
    },
    exportProcess() {
      //导出执行参数
      let onlynum = new Date().getTime();
      // let onlynum = this.setup.Name;
      // console.log("时间", onlynum, this.setup.Process);
      let jsonStr = this.loadjsondata();
      const json = JSON.stringify(jsonStr);
      const blob = new Blob([json], { type: "application/json" });
      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = onlynum;
      link.click();
      URL.revokeObjectURL(url);
    },
    loadjsondata(){
      let themeForm = JSON.parse(JSON.stringify(this.themeForm));
      this.form.IdGlobal = this.idGlobal;
      this.form.Zindex = this.zindex;
      let drawingList = JSON.parse(JSON.stringify(this.drawingList));
      for (let i = 0; i < drawingList.length; i++) {
        delete drawingList[i].chartOption.finalResult;
      }
      if (this.form.DeviceType === "double") {
        this.form.DrawOption = JSON.parse(this.form.DrawOption);
        this.form.DrawOption[this.oldDoubleType] = drawingList;
        this.form.DrawOption = JSON.stringify(this.form.DrawOption);
        this.doubleThemeOption[this.oldDoubleType] = JSON.parse(
          JSON.stringify(this.themeForm)
        );
        this.doubleThemeOption = this.removeGlobalData(this.doubleThemeOption);
      } else {
        this.form.DrawOption = JSON.stringify(drawingList);
      }
      if (themeForm.globalData) {
        for (let i = 0; i < themeForm.globalData.length; i++) {
          let rawData = JSON.parse(themeForm.globalData[i].rawData);
          for (let j = 0; j < rawData.length; j++) {
            rawData[j].content = [];
          }
          
          for (let j = 0;j < themeForm.globalData[i].combinationTable.length;j++) {
            themeForm.globalData[i].combinationTable[j].resData = [];
            themeForm.globalData[i].combinationTable[j].resProcessData = [];
          }
          themeForm.globalData[i].rawData = JSON.stringify(rawData);
        }
      }
      if (this.form.DeviceType === "double") {
        this.doubleThemeOption.globalData = themeForm.globalData;
        this.form.ThemeOption = JSON.stringify(this.doubleThemeOption);
      } else {
        this.form.ThemeOption = JSON.stringify(themeForm);
      }
      let optionObj = Object.create(null);
      let optionJson = JSON.stringify(optionObj);
      this.form.MapOption = optionJson;
      let josnForm={
        DeviceType:this.form.DeviceType,
        IdGlobal:this.form.IdGlobal,
        DrawOption:this.form.DrawOption,
        MapOption:this.form.MapOption,
        Resolution:this.form.Resolution,
        ThemeOption:this.form.ThemeOption,
      }
      return josnForm
    },
    ppt(imgUrl) {
      // 1. 创建新演示文稿
      let pres = new pptxgen();
      // 2. 添加幻灯片
      let slide = pres.addSlide();
      // 3. 向幻灯片中添加一个或多个对象（表格、形状、图像、文本和媒体）
      slide.addImage({ path: imgUrl, w: "100%", h: "100%" });
      // 4. 保存演示文稿
      let pname = String(Math.random()).replace(".", "");
      pres.writeFile(pname + ".pptx");
    },
    undo() {
      this.$store.commit("undo");
      this.drawingList = this.$store.state.snapshot.componentData;
    },
    redo() {
      this.$store.commit("redo");
      this.drawingList = this.$store.state.snapshot.componentData;
    },
    getjustifywidth(){
      let justify_con=document.querySelector("#justify_con").getBoundingClientRect();
      this.justifywidth=justify_con.width//中间导航部分的样式
      this.topHandleNavKey++
    }
  },
  beforeMount() {
    window.addEventListener("resize", this.getjustifywidth);
  },
  beforeDestroy() {
    window.removeEventListener("resize", this.getjustifywidth);
  },
  mounted() {
    this.$nextTick(() => {
      this.getjustifywidth()
      this.initSize();
    });

    //右键复制监听
    VueEvent.$on("copy_component", (data) => {
      this.copyComponent(data);
    });

    //右键删除监听
    VueEvent.$on("delete_component", (ctrlSelectArr) => {
      // if (ctrlSelectArr instanceof Array) {
      //   ctrlSelectArr.forEach((item) => {
      //     this.deleteComponent(item);
      //   });
      // } else {

      // }
      this.deleteComponent(ctrlSelectArr);
    });

    //右键置顶监听
    VueEvent.$on("top_layer", (data) => {
      this.topLayer(data);
    });

    //右键置底监听
    VueEvent.$on("bottom_layer", (data) => {
      this.bottomLayer(data);
    });

    //右键上移一层
    VueEvent.$on("up_layer", (data) => {
      this.upLayer(data);
    });

    //右键下移一层
    VueEvent.$on("down_layer", (data) => {
      this.downLayer(data);
    });

    //右键锁定
    VueEvent.$on("on_locked", (data) => {
      this.onLocked(data);
    });

    //右键组合分组监听
    VueEvent.$on("combine_component", (ctrlSelectArr) => {
      if (ctrlSelectArr.length > 1) {
        let groupArr = [];

        for (let i = 0; i < ctrlSelectArr.length; i++) {
          const chart = this.drawingList.filter(function (item) {
            return item.customId == ctrlSelectArr[i];
          })[0];

          if (chart != null && chart.chartOption.isGroup == true) {
            this.$message({
              message: "所选组件已有分组！",
              type: "warning",
            });
            return false;
          } else {
            chart.chartOption.isGroup = true;
            groupArr.push(chart);
          }
        }
        const groupChart = {};
        groupChart.chartType = "group";
        groupChart.icon = "el-icon-datav-folder";
        groupChart.customId = ++this.idGlobal;
        groupChart.layerName = "组合"; // groupChart.customId
        groupChart.bindingDiv = groupChart.customId;
        groupChart.zindex = 0;
        groupChart.isShow = true;
        groupChart.chartOption = groupArr;
        // groupChart.globalData = this.themeForm.globalData?this.themeForm.globalData:[];
        // groupChart.dataChartOption = this.themeForm.chartOption?this.themeForm.chartOption:{};
        this.drawingList.push(groupChart);
        //清空框选，设置当前选中数据
        VueEvent.$emit("clear_ctrl");
        this.activeId = groupChart.customId;
        this.activeData = groupChart;
      }
    });

    //右键解散分组监听
    VueEvent.$on("dismiss_component", (data) => {
      for (let i = 0; i < this.drawingList.length; i++) {
        if (this.drawingList[i].customId == data) {
          //将组合里的组件分组状态设置为false
          const groupOption = this.drawingList[i].chartOption;
          groupOption.forEach((element) => {
            element.chartOption.isGroup = false;
          });
          this.drawingList.splice(i, 1);
          break;
        }
      }
    });

    VueEvent.$on("active-change", (data) => {
      this.activeData = data;
    });
  },
};
</script>

<style lang="scss">
.field-box {
  .right-scrollbar {
    .el-form-item .el-form-item__label {
      font-size: 14px;
      color: #666;
    }
    .custom_form_item {
      .el-form-item {
        margin-bottom: 9px;
        .el-input-number.is-controls-right {
          width: 100%;
        }
        .el-select {
          width: 100%;
        }
        .el-form-item__content {
          .el-input__inner {
            color: #333;
          }
        }
        .el-slider {
          padding-left: 10px;
        }
      }
    }
  }
}
body,
html {
  margin: 0;
  padding: 0;
  background: #fff;
  -moz-osx-font-smoothing: grayscale;
  -webkit-font-smoothing: antialiased;
  text-rendering: optimizeLegibility;
  font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Helvetica, Arial,
    sans-serif, Apple Color Emoji, Segoe UI Emoji;
}

input,
textarea {
  font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Helvetica, Arial,
    sans-serif, Apple Color Emoji, Segoe UI Emoji;
}

.editor-tabs {
  background: #121315;
  .el-tabs__header {
    margin: 0;
    border-bottom-color: #121315;
    .el-tabs__nav {
      border-color: #121315;
    }
  }
  .el-tabs__item {
    height: 32px;
    line-height: 32px;
    color: #888a8e;
    border-left: 1px solid #121315 !important;
    background: #363636;
    margin-right: 5px;
    user-select: none;
  }
  .el-tabs__item.is-active {
    background: #1e1e1e;
    border-bottom-color: #1e1e1e !important;
    color: #fff;
  }
  .el-icon-edit {
    color: #f1fa8c;
  }
  .el-icon-document {
    color: #a95812;
  }
}

// home
.right-scrollbar {
  .el-scrollbar__view {
    padding: 20px 18px 15px 15px;
  }
}
.left-scrollbar .el-scrollbar__wrap {
  box-sizing: border-box;
  overflow-x: hidden !important;
  margin-bottom: 0 !important;
}
.center-tabs {
  .el-tabs__header {
    margin-bottom: 0 !important;
  }
  .el-tabs__item {
    width: 50%;
    text-align: center;
  }
  .el-tabs__nav {
    width: 100%;
  }
}
.reg-item {
  padding: 12px 6px;
  background: #f8f8f8;
  position: relative;
  border-radius: 4px;
  .close-btn {
    position: absolute;
    right: -6px;
    top: -6px;
    display: block;
    width: 16px;
    height: 16px;
    line-height: 16px;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 50%;
    color: #fff;
    text-align: center;
    z-index: 1;
    cursor: pointer;
    font-size: 12px;
    &:hover {
      background: rgba(210, 23, 23, 0.5);
    }
  }
  & + .reg-item {
    margin-top: 18px;
  }
}
.action-bar {
  & .el-button + .el-button {
    margin-left: 10px;
  }
  & i {
    font-size: 14px;
    vertical-align: middle;
    position: relative;
    top: -1px;
    margin-left: 5px;
  }
}

.custom-tree-node {
  width: 100%;
  font-size: 14px;
  .node-operation {
    float: right;
  }
  i[class*="el-icon"] + i[class*="el-icon"] {
    margin-left: 6px;
  }
  .el-icon-plus {
    color: #409eff;
  }
  .el-icon-delete {
    color: #157a0c;
  }
}

.left-scrollbar .el-scrollbar__view {
  overflow-x: hidden;
}

.el-rate {
  display: inline-block;
  vertical-align: text-top;
}
.el-upload__tip {
  line-height: 1.2;
}

$selectedColor: #f6f7ff;
$lighterBlue: #409eff;

.container {
  position: relative;
  width: 100%;
  height: 100%;
}

.components-list {
  padding: 8px;
  box-sizing: border-box;
  height: 100%;
  .components-item {
    display: inline-block;
    width: 48%;
    margin: 1%;
    transition: transform 0ms !important;
  }
}
.components-draggable {
  padding-bottom: 20px;
}
.components-title {
  font-size: 14px;
  color: #222;
  margin: 6px 2px;
  .svg-icon {
    color: #666;
    font-size: 18px;
  }
}

.components-body {
  padding: 8px 10px;
  background: $selectedColor;
  font-size: 12px;
  cursor: move;
  border: 1px dashed $selectedColor;
  border-radius: 3px;
  .svg-icon {
    color: #777;
    font-size: 15px;
  }
  &:hover {
    border: 1px dashed #787be8;
    color: #787be8;
    .svg-icon {
      color: #787be8;
    }
  }
}

.left-board {
  width: 260px;
  position: absolute;
  left: 0;
  top: 0;
  height: 100vh;
}
.left-scrollbar {
  height: calc(100vh - 42px);
  overflow: hidden;
}
.center-scrollbar {
  height: calc(100vh - 42px);
  overflow: hidden;
  border-left: 1px solid #f1e8e8;
  border-right: 1px solid #f1e8e8;
  box-sizing: border-box;
}
.center-board {
  height: 100vh;
  width: auto;
  margin: 0 350px 0 260px;
  box-sizing: border-box;
}
.empty-info {
  position: absolute;
  top: 46%;
  left: 0;
  right: 0;
  text-align: center;
  font-size: 18px;
  color: #ccb1ea;
  letter-spacing: 4px;
}
.action-bar {
  position: relative;
  height: 42px;
  text-align: right;
  padding: 0 15px;
  box-sizing: border-box;
  border: 1px solid #f1e8e8;
  border-top: none;
  border-left: none;
  .delete-btn {
    color: #f56c6c;
  }
}
.logo-wrapper {
  position: relative;
  height: 42px;
  background: #fff;
  border-bottom: 1px solid #f1e8e8;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  padding-left: 15px;
}
.logo {
  line-height: 30px;
  color: #00afff;
  font-weight: 600;
  font-size: 17px;
  white-space: nowrap;
}

.center-board-row {
  padding: 12px 12px 15px 12px;
  box-sizing: border-box;
  & > .el-form {
    // 69 = 12+15+42
    height: calc(100vh - 69px);
  }
}
.drawing-board {
  height: 100%;
  position: relative;
  .components-body {
    padding: 0;
    margin: 0;
    font-size: 0;
  }
  .sortable-ghost {
    position: relative;
    display: block;
    overflow: hidden;
    &::before {
      content: " ";
      position: absolute;
      left: 0;
      right: 0;
      top: 0;
      height: 3px;
      background: rgb(89, 89, 223);
      z-index: 2;
    }
  }
  .components-item.sortable-ghost {
    width: 100%;
    height: 60px;
    background-color: $selectedColor;
  }
  .active-from-item {
    & > .el-form-item {
      background: $selectedColor;
      border-radius: 6px;
    }
    & > .drawing-item-copy,
    & > .drawing-item-delete {
      display: initial;
    }
    & > .component-name {
      color: $lighterBlue;
    }
  }
  .el-form-item {
    margin-bottom: 15px;
  }
}
.drawing-item {
  position: relative;
  cursor: move;
  &.unfocus-bordered:not(.activeFromItem) > div:first-child {
    border: 1px dashed #ccc;
  }
  .el-form-item {
    padding: 12px 10px;
  }
}
.drawing-row-item {
  position: relative;
  cursor: move;
  box-sizing: border-box;
  border: 1px dashed #ccc;
  border-radius: 3px;
  padding: 0 2px;
  margin-bottom: 15px;
  .drawing-row-item {
    margin-bottom: 2px;
  }
  .el-col {
    margin-top: 22px;
  }
  .el-form-item {
    margin-bottom: 0;
  }
  .drag-wrapper {
    min-height: 80px;
  }
  &.active-from-item {
    border: 1px dashed $lighterBlue;
  }
  .component-name {
    position: absolute;
    top: 0;
    left: 0;
    font-size: 12px;
    color: #bbb;
    display: inline-block;
    padding: 0 6px;
  }
}
.drawing-item,
.drawing-row-item {
  &:hover {
    & > .el-form-item {
      background: $selectedColor;
      border-radius: 6px;
    }
    & > .drawing-item-copy,
    & > .drawing-item-delete {
      display: initial;
    }
  }
  & > .drawing-item-copy,
  & > .drawing-item-delete {
    display: none;
    position: absolute;
    top: -10px;
    width: 22px;
    height: 22px;
    line-height: 22px;
    text-align: center;
    border-radius: 50%;
    font-size: 12px;
    border: 1px solid;
    cursor: pointer;
    z-index: 1;
  }
  & > .drawing-item-copy {
    right: 56px;
    border-color: $lighterBlue;
    color: $lighterBlue;
    background: #fff;
    &:hover {
      background: $lighterBlue;
      color: #fff;
    }
  }
  & > .drawing-item-delete {
    right: 24px;
    border-color: #f56c6c;
    color: #f56c6c;
    background: #fff;
    &:hover {
      background: #f56c6c;
      color: #fff;
    }
  }
}

body {
  margin: 0;
  padding: 0;
  font-family: sans-serif;
  overflow: hidden;
}
body * {
  box-sizing: border-box;
  user-select: none;
}
.wrapper {
  background-color: #f5f5f5;
  position: absolute;
  top: 0px;
  left: 0px;
  width: 100%;
  // height: 890px;
  height: calc(100vh - 42px);
  border: 1px solid #dadadc;
  overflow-x: scroll;
}
#screens {
  position: absolute;
  width: 100%;
  height: 100%;
  overflow: auto;
}
.screen-container {
  position: absolute;
  width: 2500px;
  height: 1800px;
}
.scale-value {
  position: absolute;
  left: 0;
  bottom: 100%;
}
.button {
  position: absolute;
  left: 100px;
  bottom: 100%;
}
.button-ch {
  position: absolute;
  left: 200px;
  bottom: 100%;
}
.button-en {
  position: absolute;
  left: 230px;
  bottom: 100%;
}

#canvas {
  position: absolute;
  top: 80px;
  left: 80px;
  width: 160px;
  height: 200px;
  transform-origin: 0 0;
}
.el-submenu__title {
  padding: 0 10px;
}
.action-bar .el-button + .el-button {
  margin-left: 5px;
}
background-images-ul {
  margin-bottom: 20px;
}
.background-images-ul li {
  position: relative;
  margin: 2px;
  display: inline-block;
  width: 200px;
  line-height: 25px;
  padding: 20px;
  border: 1px solid #e2e2e2;
  font-size: 14px;
  text-align: center;
  color: #666;
  transition: all 0.3s;
  -webkit-transition: all 0.3s;
  cursor: pointer;
  &:hover {
    & > .drawing-item-delete {
      display: initial;
    }
  }
  & > .drawing-item-copy,
  & > .drawing-item-delete {
    display: none;
    position: absolute;
    top: 0px;
    left: 175px;
    width: 22px;
    height: 22px;
    line-height: 20px;
    text-align: center;
    border-radius: 50%;
    font-size: 12px;
    border: 1px solid;
    cursor: pointer;
    z-index: 1;
  }
  & > .drawing-item-delete {
    right: 24px;
    border-color: #f56c6c;
    color: #f56c6c;
    background: #fff;
    &:hover {
      background: #f56c6c;
      color: #fff;
    }
  }
}
.border_img {
  width: 160px;
  height: 90px;
}
.background-images-ul label {
  display: inline-block;
  width: 160px;
  color: #0088ff;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.row {
  margin-right: -15px;
  margin-left: -15px;
}
.col-xs-6 {
  width: 50%;
  float: left;
  cursor: pointer;
  padding: 10px;
  /**background-color: #b3d4ff;**/
}
.theme-plan-group {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  width: auto;
  height: 100px;
  overflow: hidden;
  border: 1px solid #eee;
  padding: 5px;
  border-radius: 4px;
  margin-bottom: 8px;
}
.theme-plan-color {
  width: 20px;
  height: 20px;
  margin-bottom: 10px;
  margin-left: 2px;
  margin-right: 2px;
  display: inline-block;
  border-radius: 3px;
}
.el-menu-demo {
  display: flex;
}
.el-menu--horizontal > .el-submenu {
  float: none;
  width: 14.2%;
}
.action-bar ul {
  width: 90%;
}

.scaleCls {
  position: absolute;
  top: 70px;
  right: 15px;
  z-index: 1000;
  .el-input__inner {
    background-color: transparent;
    border: none;
  }
  .el-input-number__decrease {
    border: none;
    background-color: #fff;
  }
  .el-input-number__increase {
    border: none;
    background-color: #fff;
  }
}
</style>
