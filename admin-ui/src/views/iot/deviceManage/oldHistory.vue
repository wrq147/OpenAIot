<template>
  <div>
    <el-dialog title="历史数据" :visible.sync="infoVisible" top="2vh" width="60%" @close="closeDialog" :close-on-click-modal="false">
      <div slot="title">
        <span style="margin-right:20px">历史数据</span>
        <div style="display: inline-block;" v-hasPermi="['/IoTService/IotData/Update']">
          <el-button type="text" plain @click="handleHistoryEdit" v-if="!isEditHistory" class="text_button">
            <i class="el-icon-edit-outline"></i>
            <span style="margin-left: 6px">编辑</span>
          </el-button>
          <el-button type="text" plain @click="handleHistoryEdit" v-else class="text_button">
            <i class="el-icon-circle-close"></i>
            <span style="margin-left: 6px">取消编辑</span>
          </el-button>
        </div>
      </div>
      <div v-if="isEditHistory">
        <el-input-number v-model="editValue" ></el-input-number>
        <el-date-picker style="margin-top:10px" v-model="editIndate" value-format="timestamp" type="datetime" placeholder="选择日期时间"></el-date-picker>
        <el-button type="warning" plain @click="handleSaveProps">
          <span style="margin-left: 6px">插入数据</span>
        </el-button>
      </div>
      <el-row class="mb8 button_row" style="display: flex; align-items: center">
        <div class="choice_time" style="margin-bottom:-10px;margin-left: -20px;">
          <el-radio-group v-model="showTime" @input="setShowTime" style="margin-left:20px">
            <el-radio-button label="今日"></el-radio-button>
            <el-radio-button label="近一周"></el-radio-button>
            <el-radio-button label="近一月"></el-radio-button>
          </el-radio-group>
          <el-date-picker v-model="choiceTime" type="datetimerange" :unlink-panels="true" range-separator="至"
            start-placeholder="开始日期" end-placeholder="结束日期" align="right" style="margin-left: 20px;margin-top:10px;" @change="loadDeviceOldInfo()"></el-date-picker>
          <el-select v-model="activeOldAttr.Code" placeholder="请选择" @change="changeActiveCode" v-if="isOnlyTable" style="margin-left: 20px;margin-top:10px;">
            <el-option v-for="item in propertiesList" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </div>
        <el-col :span="1.5">
          <el-button type="warning" plain @click="handleExport">
            <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
            <span style="margin-left: 6px">导出</span>
          </el-button>
        </el-col>
      </el-row>
      <div class="header" style="border-top: none;padding: 0;border-bottom: 1px solid #dadada;margin-top: 20px;">
        <el-menu :default-active="dataMode" active-text-color="#409eff" class="el-menu-demo shejiqi" mode="horizontal">
          <el-menu-item index="tableData" @click="dataModeSelect('tableData')">列表</el-menu-item>
          <el-menu-item index="chartsData" @click="dataModeSelect('chartsData')" v-if="!activeOldAttr.type || (activeOldAttr.type && activeOldAttr.type != 'onLine')">图表</el-menu-item>
        </el-menu>
      </div>
      <div v-show="dataMode == 'tableData'">
        <div style="height: 500px" class="history_con">
          <el-table key="oldInfoList" :data="oldInfoList" style="width: 100%; margin-top: 20px; min-height: 138.5px"
            height="500" :header-row-style="{ 'background-color': '#B5B5B5' }" v-loading="isLoadingTable" highlight-current-row
            v-if="!activeOldAttr.type || (activeOldAttr.type && activeOldAttr.type != 'onLine')">
            <el-table-column prop="Name" label="属性名称"></el-table-column>
            <!-- <el-table-column prop="Code" label="属性代码"></el-table-column> -->
            <el-table-column prop="Value" label="属性值">
              <template slot-scope="scope">
                <div v-if="activeOldAttr.OptionType == 'geo'">
                  <div>经度：{{ scope.row.Value.lat }}</div>
                  <div>纬度：{{ scope.row.Value.lng }}</div>
                </div>
                <div v-else>{{ scope.row.Value }}</div>
              </template>
            </el-table-column>
            <el-table-column prop="Unit" label="单位"></el-table-column>
            <el-table-column prop="UpdatedOn" sortable label="更新时间" :sort-orders="['ascending']"></el-table-column>
            <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width" v-if="isEditHistory">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-delete" @click="handleHistoryDelete(scope.row,scope.$index)">删除</el-button>
              </template>
            </el-table-column>
            <template slot="append" v-if="status != 'loading'">
              <!--
                  @infinite: 滚动事件回调函数,当滚动到距离滚动父元素底部特定距离的时候，会被调用
                  distance: 这是滚动的临界值。default: 100; 如果到滚动父元素的底部距离小于这个值，那么 loadMore 回调函数就会被调用。
                  spinner: 通过这个属性，你可以选择一个你最喜爱旋转器作为加载动画
                        'default' | 'bubbles' | 'circles' | 'spiral' | 'waveDots'
                  direction: 如果你设置这个属性为top,那么这个组件将在你滚到顶部的时候，调用on-infinite函数
                        'top' | 'bottom'
                  forceUseInfiniteWrapper: (boolean | string) 强制指定滚动容器，使用CSS 选择器
                  identifier: 识别号，改变时刷新
                  -->
              <infinite-loading key="historyInfiniteLoading" @infinite="loadMore" ref="historyInfiniteLoading" :distance="3" spinner="bubbles"
                :identifier="infiniteId" force-use-infinite-wrapper=".history_con .el-table__body-wrapper">
                <!--   orce-use-infinite-wrapper 属性在存在多个 el-table 需要更详细的css选择器   -->
                <div class="no-more" slot="no-more"></div>
                <div class="no-more" slot="no-results"></div>
                <div class="no-more" slot="error">出错了</div>
              </infinite-loading>
              <div class="no-more" style="text-align: center" v-if="status == 'noMore'">
                没有更多数据
              </div>
            </template>
          </el-table>
          <el-table v-else :data="oldInfoList" style="width: 100%; margin-top: 20px; min-height: 138.5px" height="500"
            :header-row-style="{ 'background-color': '#B5B5B5' }" v-loading="isLoadingTable" highlight-current-row key="oldonlineInfoList">
            <!-- <el-table-column prop="Code" label="属性代码"></el-table-column> -->
            <el-table-column prop="Value" label="联网状态" align="center">
              <template slot-scope="scope">
                <div>
                  <div v-if="scope.row.IsOnline">在线</div>
                  <div v-else>离线</div>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="CreatedOn" sortable label="更新时间" :sort-orders="['ascending']" align="center"></el-table-column>
            <template slot="append" v-if="status != 'loading'">
              <infinite-loading key="historyInfiniteLoading2" @infinite="loadMore" ref="historyInfiniteLoading2" :distance="3"
                spinner="bubbles" :identifier="infiniteId" force-use-infinite-wrapper=".history_con .el-table__body-wrapper">
                <!--   orce-use-infinite-wrapper 属性在存在多个 el-table 需要更详细的css选择器   -->
                <div class="no-more" slot="no-more"></div>
                <div class="no-more" slot="no-results"></div>
                <div class="no-more" slot="error">出错了</div>
              </infinite-loading>
              <div class="no-more" style="text-align: center" v-if="status == 'noMore'">
                没有更多数据
              </div>
            </template>
          </el-table>
        </div>
      </div>
      <div v-show="dataMode == 'chartsData'" class="content_echarts" v-if="!activeOldAttr.type || (activeOldAttr.type && activeOldAttr.type != 'onLine')" :style="{'padding-bottom':activeOldAttr&&activeOldAttr.OptionType=='enum'?'50px':''}">
        <div class="state_con" v-if="activeOldAttr&&activeOldAttr.OptionType=='enum'">
          <div class="state_li" v-for="ke in Object.keys(enumValColor)" :key="'state'+ke">
            <div class="li_box" :style="{background:enumValColor[ke]}"></div>
            <div class="li_text">{{ke}}</div>
          </div>
        </div>
        <div class="state_con" style="font-size:16px;color:#27AFFF;" v-else>
          <span>{{activeOldAttr.Name}}</span>
        </div>
        <div v-if="oldInfoList.length > 0" id="chartsLine" class="chartsLine" :style="{ width: ' 100%', height: echartsHei + 'px' }"></div>
        <div v-else-if="oldInfoList.length == 0" class="noData" id="noData" :style="{ width: ' 100%', height: echartsHei + 'px' }"></div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { DeviceOldInfo, DeviceOnLineOldInfo,iotDeviceDelHistory } from "@/api/rules/device";
import InfiniteLoading from "vue-infinite-loading";
import * as echarts from "echarts";
var dayjs = require("@/utils/day.js");
import { exportExcleUtils } from "@/utils/common.js";
import { saveDeviceProp } from "@/api/rules/device";
import { initMap } from "@/utils/amap";
export default {
  name: "deviceOldHistory",
  components: { InfiniteLoading },
  props: {
    activeAttr: {
      type: Object,
      default: () => {
        return {};
      },
    },
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    isOnlyTable: {
      type: Boolean,
      default: false,
    },
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    visible: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      editValue:0,
      editIndate:null,
      infoVisible: false,
      echartsHei: 138.5, //设置图表高度
      dataMode: "tableData", //数据展示方式
      showTime: "", //显示的时间范围
      choiceTime: [], //选择的时间
      //   infoVisible: false, //历史数据弹出层
      oldInfoList: [], //设备历史数据
      isLoadingTable: false, //是否正在加载表格数据
      activeOldAttr: {},
      tuBiaoOption: {}, //图表数据
      isShowMap: true,
      infoWindow: null, //腾讯地图窗体信息
      lineChart: null, //绘制图标
      status: "loading",
      infiniteId: "h" + new Date(), //滚动组件的识别号，改变时刷新
      deviceOldQuery: {
        pageNum: 1,
        pageSize: 30,
      },
      isOver: false,
      propertiesList: [],
      daochu: {
        pageNum: 1,
        pageSize: 300000,
      },
      loadPageSize:100,//导出时设置300000
      isActiveDaochu:false,//是否是本地手动操作导出
      isEditHistory:false,
      showEdit:false,
      enumValColor:{},
      colorList:['rgb(65,105,225)','rgb(135,206,250)','rgb(0,191,255)','rgb(176,224,230)','rgb(95,158,160)','rgb(25,25,112)','rgb(176,196,222)','rgb(30,144,255)',
      'rgb(255,182,193)','rgb(220,20,60)','rgb(255,20,147)','rgb(218,112,214)','rgb(139,0,139)','rgb(86,85,21)','rgb(175,238,238)','rgb(0,206,209)','rgb(47,79,79)','rgb(0,128,128)',
      'rgb(32,178,170)','rgb(127,255,170)','rgb(245,255,250)','rgb(173,255,47)','rgb(107,142,35)','rgb(255,2550)','rgb(238,232,170)','rgb(128,128,0)','rgb(222.184,135)','rgb(255,140,0)',
      'rgb(205,133,63)','rgb(210,105,30)','rgb(139,69,19)','rgb(255,160,122)','rgb(233,150,122)','rgb(250,128,114)','rgb(240,128,128)','rgb(188,143,143)','rgb(205,92,92)','rgb(220,220,220)','rgb(169,169,169)','rgb(128,128,128)','rgb(0,0,0)']
    };
  },
  watch: {
    activeAttr: {
      handler(to) {
        this.activeOldAttr = JSON.parse(JSON.stringify(to));
        if (!this.isOnlyTable) {
          if(this.activeOldAttr.OptionType=='enum'){
            this.initTuBiao2();
          }else{
            this.initTuBiao();
          }
          
        }
        if (this.isOnlyTable) {
          this.getAttribute();
        } else {
          this.oldInfoVisible();
        }
      },
      immediate: true,
      deep: true,
    },
    visible: {
      handler(to) {
        this.infoVisible = to;
      },
      immediate: true,
    },
  },
  async beforeCreate() {
    initMap();
  },
  mounted() {
    if(this.$store.state.user.Id==1){
      this.showEdit=true;
    }
  },
  methods: {
    handleSaveProps() {
      //保存标签信息
      if(this.activeOldAttr.Code&&this.editIndate&&this.editValue){
        let arr = [];
        let obj = {};
        obj[this.activeOldAttr.Code]=this.editValue
        arr.push(obj);
        let subQuery={ id: this.deviceInfos.Id, newVals: obj }
        if(this.editIndate){
          subQuery.indate=dayjs(this.editIndate).format('YYYY-MM-DD HH:mm:ss')
        }
        saveDeviceProp(subQuery).then((res) => {
          // console.log("保存标签信息返回", res);
          if (res.code == 0) {
            this.$modal.msgSuccess("修改成功");
          //   this.cancelEditLable(inx);
          //   this.getDeviceTagList();
          }
        });
      }
      
    },
    handleHistoryDelete(row,inx){
      // console.log(row,'row',inx);
      // return
      let that=this
      this.$modal.confirm('是否确认删除"' + row.UpdatedOn + '"时间点的前后一秒内的历史数据'+'？').then(function () {
        return iotDeviceDelHistory({ id: that.deviceInfos.Id,beginTime:dayjs(row.UpdatedOn).subtract(1, 'second').format('YYYY-MM-DD HH:mm:ss'),endTime:dayjs(row.UpdatedOn).add(1, 'second').format('YYYY-MM-DD HH:mm:ss') });
      }).then(() => {
        that.oldInfoList.splice(inx,1)
        that.$modal.msgSuccess("删除成功");
      }).catch((err) => { 
        console.log("错误",err);
      });
    },
    handleHistoryEdit(){
      this.isEditHistory=!this.isEditHistory
    },
    closeDialog() {
      this.$emit("closeHistoryDialog", false);
    },
    handleExport() {
      //导出
      let loadingInstance = this.$loading({
        //进入页面设置加载中效果，方便完成页面保存数据的初始化
        lock: true,
        text: "正在导出，请稍等...",
        spinner: "el-icon-loading",
        background: "rgba(0, 0, 0, 1)",
      });
      if(this.isActiveDaochu){
        this.loadDeviceOldInfo((list) => {
          let table = list;
          //表头
          let tHeader = ["属性名称", "属性值", "单位", "更新时间"];
          //数据的里字段
          let filterVal = ["Name", "Value", "Unit", "UpdatedOn"];
          let name = this.activeOldAttr.Name;
          let fileName = name + "的历史数据";
          loadingInstance.close();
          exportExcleUtils(tHeader, filterVal, table, fileName);
        });
      }else{
        let table = this.oldInfoList;
        //表头
        let tHeader = ["属性名称", "属性值", "单位", "更新时间"];
        //数据的里字段
        let filterVal = ["Name", "Value", "Unit", "UpdatedOn"];
        let name = this.activeOldAttr.Name;
        let fileName = name + "的历史数据";
        loadingInstance.close();
        exportExcleUtils(tHeader, filterVal, table, fileName);
      }
      
      
    },
    getAttribute() {
      // console.log(this.productInfos, "this.productInfosthis.productInfos");
      let mds = JSON.parse(this.productInfos.ModelTSL);
      let properties = JSON.parse(JSON.stringify(mds.properties));

      this.propertiesList = properties.filter(
        (item) =>
          item.option.type == "date" ||
          item.option.type == "float" ||
          item.option.type == "int" ||
          item.option.type == "geo"||
          item.option.type == "enum"
      );
      this.propertiesList.push({
        name: "离在线记录",
        code: "leavingOnline",
        type: "onLine",
      });
      this.activeOldAttr = {
        name: "离在线记录",
        Code: "leavingOnline",
        type: "onLine",
      };
      this.oldInfoVisible();
    },
    changeActiveCode(code) {
      let obj = this.propertiesList.find((row) => row.code == code);
      if (obj) {
        this.activeOldAttr = {
          Code: obj.code,
          Name: obj.name,
          OptionType: obj.option ? obj.option.type : "",
          type: obj.type ? obj.type : "",
        };
        if (obj.type && obj.type == "onLine") {
          this.activeOldAttr = {
            Name: "离在线记录",
            Code: "leavingOnline",
            type: "onLine",
          };
        } else {
          if(obj.option.type=='enum'){
            this.initTuBiao2();
          }else{
            this.initTuBiao();
          }
          
        }
        this.oldInfoVisible();
      }
    },
    lineChartResize() {
      if (this.dataMode == "chartsData") {
        this.lineChart.resize();
      }
    },
    loadMore($state) {
      //无线滚动加载更多
      if (this.status == "more") {
        this.deviceOldQuery.pageNum++;
        // console.log("滚动监听", this.deviceOldQuery.pageNum);
        // this.getDeviceOldInfo($state);
        if (
          this.activeOldAttr &&
          this.activeOldAttr.type &&
          this.activeOldAttr.type == "onLine"
        ) {
          this.getLineOldInfo($state);
        } else {
          this.getDeviceOldInfo($state);
        }
      } else if (this.status == "noMore") {
        // console.log($state.complete(), "$state.complete()");
        $state.complete();
      }
    },
    initTuBiao() {
      let _this = this;
      this.tuBiaoOption = {
        //设置两个标题需要将标题设置为数组
        tooltip: {
          trigger: "axis",
        },
        grid:{
          top:'5%'
        },
        xAxis: {
          data: [],
          type: "category",
          boundaryGap: false,
          axisLine: {
            show: true,
            lineStyle: {
              color: "#CACDD1",
            },
          },
          axisTick: {
            show: true, //是否显示网状线 默认为true
            inside: true, // 坐标轴刻度是否朝内，默认朝外
            alignWithLabel: false,
          },
        },
        yAxis: {
          type: "value",
          boundaryGap: [0, "100%"],
        },
        dataZoom: [
          {
            type: "inside",
            start: 0,
            end: 100,
          },
          {
            start: 0,
            end: 100,
          },
        ],
        series: [
          {
            data: [],
            type: "line",
            label: {
              show: true,
              formatter: function (params) {
                //标签内容

                return params.data.value + params.data.Unit;
              },
              position: "top",
            },
            smooth: true,
            color: ["#1c84c6"], //折线条的颜色
            symbol: "none", //设定为实心点
            symbolSize: 8, //设定实心点的大小
            itemStyle: {
              normal: {
                color: "rgba(41, 83, 255, 1)", //改变折线点的颜色
                borderColor: "#ffffff",
                borderWidth: 2,
                lineStyle: {
                  color: "rgba(41, 83, 255, 1)", //改变折线颜色
                },
              },
            },
            areaStyle: {
              color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                {
                  offset: 0,
                  color: "rgba(41, 83, 255, 0.2000)",
                },
                {
                  offset: 1,
                  color: "rgba(41, 83, 255, 0)",
                },
              ]),
            },
            axisLabel: {
              formatter(val, i) {
                return val;
              },
            },
          },
        ],
      };
    },
    initTuBiao2() {
      let _this = this;
      this.tuBiaoOption = {
        tooltip: {
          trigger: 'item',
          backgroundColor: 'rgba(255, 255, 255, 0.9)',
          borderColor: '#eee',
          borderWidth: 1,
          textStyle: { color: '#333' },
          align: 'left',
          formatter: (params) => {
            const stateInfo = params.data;
            return `
              <div style="text-align: left;">${stateInfo.name}</div>
              <div>
                <div style="width: 10px; height: 10px; border-radius: 50%; display: inline-block; margin-right: 5px; background-color: ${stateInfo.itemStyle.color};"></div>
                ${stateInfo.itemStyle.name}时间: ${dayjs(stateInfo.value[1]).format('YYYY/MM/DD HH:mm:ss')}-${dayjs(stateInfo.value[2]).format('YYYY/MM/DD HH:mm:ss')}
                </div>
              <div style="text-align: left;">
                <div style="width: 10px; height: 10px; border-radius: 50%; display: inline-block; margin-right: 5px; background-color: ${stateInfo.itemStyle.color};"></div>
                ${stateInfo.itemStyle.name}: ${this.formatDuration(stateInfo.value[3])}
              </div>
            `;
          }
        },
        grid: {
          top: '0',
          left: '1%',
          right: '4%',
          bottom: '35%',
          containLabel: true,
          borderWidth: 1,
          color: 'rgba(255, 255, 255, 0.5)',
          show: false,
        },
        xAxis: {
          type: 'time',
          min: 0,
          max: 100,
          boundaryGap: false,
          splitLine: { show: false },
          // splitNumber: 2,
          // interval: 7200000,
          axisLine: {
            show: false // 设为 false 即可隐藏
          },
          axisLabel: {
            formatter:(value,index)=>{ // 第一个参数没有用 所以用_代替
              return `${dayjs(value).format('YYYY-MM-DD')} \n ${dayjs(value).format('HH:mm:ss')}`; // 其他位置不显示标签
            },
            showMaxLabel: true,
            showMinLabel: true,
            // interval: 'auto',
          }
        },
        yAxis: {
          type: 'category',
          data: [],
          axisLabel: {
            interval: 0,
            fontSize: 18,
            fontWeight: 500,
          },
          axisLine: {
            show: false // 设为 false 即可隐藏
          }
        },
        dataZoom: [
          {
            type: 'slider',
            filterMode: 'weakFilter',
            showDataShadow: false,
            top: 160,
            labelFormatter: ''
          },
          {
            type: 'inside',
            filterMode: 'weakFilter'
          }
        ],
        series: [
          {
            id: 'timeline',
            type: 'custom',
            renderItem: this.renderItem,
            itemStyle: {
              opacity: 0.8
            },
            encode: {
              x: [1, 2],
              y: 0
            },
            data: []
          }
        ]
      };
    },
    formatDuration(durationMs) {//时间格式化
      const totalSeconds = Math.floor(durationMs / 1000);
      const hours = Math.floor(totalSeconds / 3600);
      const minutes = Math.floor((totalSeconds % 3600) / 60);
      const seconds = totalSeconds % 60;
      
      if (hours > 0) {
        return `${hours}小时${minutes}分${seconds}秒`;
      } else if (minutes > 0) {
        return `${minutes}分${seconds}秒`;
      } else {
        return `${seconds}秒`;
      }
    },
    loadArrType(list){
      let arr=JSON.parse(JSON.stringify(list))
      let names = new Set(arr.map(item => item.Value));
      return names.size
    },
    processData(sortedStates) {//处理枚举历史数据
      const processedData = [];
      // 处理所有状态时间段
      for (let i = sortedStates.length-1; i >=0; i--) {
        let index=sortedStates.length-1-i
        const state = sortedStates[i];
        const startTime = new Date(state.UpdatedOn);
        let endTime;
        
        // 如果不是最后一个状态，结束时间为下一个状态的开始时间
        if (i >0) {
          endTime = new Date(sortedStates[i - 1].UpdatedOn);
        } 
        // 如果是最后一个状态，结束时间为班次结束时间
        else {
          endTime = new Date()
        }
        let activeColor=''
        if(this.enumValColor[state.Value]){
          activeColor=this.enumValColor[state.Value]
        }else{
          let keyArr=Object.keys(this.enumValColor)
          if(this.colorList[keyArr.length]){
            activeColor=this.colorList[keyArr.length]
          }else{
            activeColor=this.getRandomColor()
          }
          this.enumValColor[state.Value]=activeColor
        }
        this.$forceUpdate()
        // 确保时间段有效
        if (endTime > startTime) {
          const duration = endTime - startTime;
          processedData.push({
            name: this.activeOldAttr.Name,
            value: [index, startTime, endTime, duration],
            itemStyle: {
              normal: {
                nameLight: state.Value,
                name: state.Value,
                color: activeColor
              }
            },
          })
          if(state.Value=='自动加载运行'){
            console.log('数据',{
            name: this.activeOldAttr.Name,
            value: [index, startTime, endTime, duration],
            itemStyle: {
              normal: {
                nameLight: state.Value,
                name: state.Value,
                color: activeColor
              }
            },
          });
          }
        }
      }
      this.processDataList = processedData;
      return processedData;
    },

    getRandomColor() {
      var r = Math.floor(Math.random() * 256);
      var g = Math.floor(Math.random() * 256);
      var b = Math.floor(Math.random() * 256);
      let colorStr='rgb(' + r + ',' + g + ',' + b + ')';
      if(this.colorList.includes(colorStr)){
        return this.getRandomColor()
      }
      return colorStr;
    },
    renderItem(params, api) {//甘特图处理
      // console.log("数据api",api);
      var categoryIndex = api.value(0);
      
      var start = api.coord([api.value(1), categoryIndex]);
      var end = api.coord([api.value(2), categoryIndex]);
      var height = api.size([0, 1])[1] * 0.35;
      var rectShape = echarts.graphic.clipRectByRect(
        {
          x: start[0],
          y: start[1] - height / 2,
          width: end[0] - start[0],
          height: height
        },
        {
          x: params.coordSys.x,
          y: params.coordSys.y,
          width: params.coordSys.width,
          height: params.coordSys.height
        }
      );
      return (
        rectShape && {
          type: 'rect',
          transition: ['shape'],
          shape: rectShape,
          style: api.style()
        }
      );
    },
    loadAllEnumData(){
      
    },
    oldInfoVisible() {
      this.showTime = "";
      this.dataMode = "tableData";
      this.deviceOldQuery.pageNum = 1;
      this.deviceOldQuery.pageSize = 100;
      this.choiceTime = [];
      this.oldInfoList = [];
      // this.setShowTime();
      if (
        this.activeOldAttr &&
        this.activeOldAttr.type &&
        this.activeOldAttr.type == "onLine"
      ) {
        this.getLineOldInfo();
      } else {
        this.getDeviceOldInfo();
      }
    },
    getLineOldInfo($state, cb) {
      //获取离在线历史数据
      if (this.deviceOldQuery.pageNum == 1) {
        this.status = "loading";
        this.isLoadingTable = true;
        this.isOver = false;
        this.oldInfoList = [];
        if (this.$refs.historyInfiniteLoading2) {
          this.$refs.historyInfiniteLoading2.stateChanger.reset();
        }
      }
      let obj = {};
      if (this.choiceTime && this.choiceTime.length) {
        obj = this.addDateRange({ Id: this.deviceInfos.Id }, this.choiceTime);
        obj.beginTime = this.parseTime(obj.beginTime);
        obj.endTime = this.parseTime(obj.endTime);
      } else {
        obj = { Id: this.deviceInfos.Id };
      }

      if (this.dataMode == "tableData") {
        obj.pageNum = this.deviceOldQuery.pageNum;
        obj.pageSize = this.deviceOldQuery.pageSize;
        console.log(cb);
        if (cb && cb !== undefined) {
          if (obj.beginTime && obj.endTime) {
            obj.pageNum = this.daochu.pageNum;
            obj.pageSize = this.daochu.pageSize;
          }
        }
      }
      DeviceOnLineOldInfo(obj).then((res) => {
        if (this.isLoadingTable) {
          this.isLoadingTable = false;
        }
        this.oldInfoList = [...this.oldInfoList, ...res.data.List];
        if (cb && cb !== undefined) {
          cb(res.data.List);
        }
        if (res.data.List.length < this.deviceOldQuery.pageSize) {
          this.isOver = true;
          this.status = "noMore";
          if ($state && $state != undefined) {
            $state.complete(); // 全部加载完成
          }
        } else {
          this.isOver = false;
          this.status = "more";
          if ($state && $state != undefined) {
            $state.loaded(); // 单个数据加载完毕
          }
        }
      });
    },
    setShowTime() {
      //设置展示时间
      this.choiceTime = [];
      if (this.showTime == "今日") {
        let end = new Date(
          new Date(new Date().toLocaleDateString()).getTime() +
            24 * 60 * 60 * 1000 -
            1
        );
        let start = new Date(
          new Date(new Date().toLocaleDateString()).getTime()
        );
        end = dayjs(new Date(end)).format("YYYY-MM-DD HH:mm:ss");
        start = dayjs(new Date(start)).format("YYYY-MM-DD HH:mm:ss");
        this.choiceTime.push(start);
        this.choiceTime.push(end);
      }
      if (this.showTime == "近一周") {
        let end = new Date();
        let start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
        end = dayjs(new Date(end)).format("YYYY-MM-DD HH:mm:ss");
        start = dayjs(new Date(start)).format("YYYY-MM-DD HH:mm:ss");
        this.choiceTime.push(start);
        this.choiceTime.push(end);
      }
      if (this.showTime == "近一月") {
        let end = new Date();
        let start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
        end = dayjs(new Date(end)).format("YYYY-MM-DD HH:mm:ss");
        start = dayjs(new Date(start)).format("YYYY-MM-DD HH:mm:ss");
        this.choiceTime.push(start);
        this.choiceTime.push(end);
      }
      this.deviceOldQuery.pageNum = 1;
      this.deviceOldQuery.pageSize = 30;
      this.oldInfoList = [];
      if (
        this.activeOldAttr &&
        this.activeOldAttr.type &&
        this.activeOldAttr.type == "onLine"
      ) {
        this.getLineOldInfo();
      } else {
        this.getDeviceOldInfo();
      }
    },
    dataModeSelect(mode) {
      //选择数据展示方式
      this.dataMode = mode;
      this.deviceOldQuery.pageNum = 1;
      this.oldInfoList = [];
      // if(this.dataMode == "chartsData"){
      if (
        this.activeOldAttr &&
        this.activeOldAttr.type &&
        this.activeOldAttr.type == "onLine"
      ) {
        this.getLineOldInfo();
      } else {
        this.getDeviceOldInfo();
      }
      // }
    },
    loadDeviceOldInfo(cb) {
      this.deviceOldQuery.pageNum = 1;
      this.deviceOldQuery.pageSize = this.loadPageSize;
      this.oldInfoList = [];
      if (
        this.activeOldAttr &&
        this.activeOldAttr.type &&
        this.activeOldAttr.type == "onLine"
      ) {
        if (cb && cb !== undefined) {
          this.getLineOldInfo("", (list) => {
            cb(list);
          });
        } else {
          this.getLineOldInfo();
        }
      } else {
        if (cb && cb !== undefined) {
          this.getDeviceOldInfo("", (list) => {
            cb(list);
          });
        } else {
          this.getDeviceOldInfo();
        }
      }
    },
    getDeviceOldInfo($state, cb) {
      //获取设备的历史数据
      if (this.dataMode == "chartsData" || this.deviceOldQuery.pageNum == 1) {
        this.status = "loading";
        this.isLoadingTable = true;
        this.isOver = false;
        this.oldInfoList = [];
        if (this.$refs.historyInfiniteLoading) {
          this.$refs.historyInfiniteLoading.stateChanger.reset();
        }
      }
      let obj = {};
      if (this.choiceTime && this.choiceTime.length) {
        obj = this.addDateRange(
          { Id: this.deviceInfos.Id, Code: this.activeOldAttr.Code },
          this.choiceTime
        );
        obj.beginTime = this.parseTime(obj.beginTime);
        obj.endTime = this.parseTime(obj.endTime);
      } else {
        obj = { Id: this.deviceInfos.Id, Code: this.activeOldAttr.Code };
      }

      if (this.dataMode == "tableData") {
        obj.pageNum = this.deviceOldQuery.pageNum;
        obj.pageSize = this.deviceOldQuery.pageSize;
        if (cb) {
          if (obj.beginTime && obj.endTime) {
            obj.pageNum = this.daochu.pageNum;
            obj.pageSize = this.daochu.pageSize;
          }
        }
      } else {
        if (obj.beginTime && obj.endTime) {
          delete obj.pageNum;
          delete obj.pageSize;
        } else {
          obj.pageNum = this.deviceOldQuery.pageNum;
          obj.pageSize = this.deviceOldQuery.pageSize;
        }
      }
      // console.log("导出时",obj);
      DeviceOldInfo(obj).then((res) => {
        if (this.dataMode == "chartsData") {
          this.oldInfoList = [];
          this.oldInfoList = JSON.parse(JSON.stringify(res.data.List));
        } else {
          if (this.isLoadingTable) {
            this.isLoadingTable = false;
          }
          this.oldInfoList = [...this.oldInfoList, ...res.data.List];
          if (cb) {
            cb(res.data.List);
          }
          // console.log("历史数据",this.oldInfoList);
          if (res.data.List.length < this.deviceOldQuery.pageSize) {
            this.isOver = true;
            this.status = "noMore";
            if ($state && $state != undefined) {
              $state.complete(); // 全部加载完成
            }
          } else {
            this.isOver = false;
            this.status = "more";
            if ($state && $state != undefined) {
              $state.loaded(); // 单个数据加载完毕
            }
          }
        }
        if (this.dataMode == "chartsData") {
          if (this.activeOldAttr.OptionType == "geo") {
            this.$nextTick(() => {
              this.setMapTrack();
            });
          } else {
            if(this.activeOldAttr.OptionType=='enum'){
              // console.log("历史数据",this.oldInfoList);
              // let sizeType=this.loadArrType(this.oldInfoList)
              if (this.oldInfoList.length > 0) {
                this.echartsHei = 238.5;
              }
              this.$nextTick(() => {
                if (this.oldInfoList.length > 0) {
                  this.lineChart = echarts.init(
                        document.querySelector(".content_echarts .chartsLine")
                      ); //获取第一个甘特图
                  let afterData=this.processData(this.oldInfoList)
                  // console.log("得到的最后数据",afterData);
                  this.tuBiaoOption.yAxis.data=[this.activeOldAttr.Name]
                  this.tuBiaoOption.xAxis.min=new Date(this.oldInfoList[this.oldInfoList.length-1].UpdatedOn)
                  this.tuBiaoOption.xAxis.max=new Date()
                  this.tuBiaoOption.series[0].data=afterData
                  // let funstr=`(value,index)=>{ // 第一个参数没有用 所以用_代替
                  //   if (index === 0 || index === ${afterData.length - 1}) {
                  //         return value; // 返回任务的名称或其他标识信息
                  //     } else {
                  //         return ''; // 其他位置不显示标签
                  //     }
                  // },`
                  // this.tuBiaoOption.xAxis.axisLabel.formatter=eval(funstr)
                  // console.log("图表数据结果",this.tuBiaoOption);
                  this.lineChart.setOption(this.tuBiaoOption);
                  this.lineChart.resize();
                  window.addEventListener("resize", () => {
                    this.$nextTick(() => {
                      this.lineChart.resize();
                    });
                  });
                } else {
                  if (this.lineChart) {
                    this.lineChart.dispose();
                    let clearDom = document.getElementById("noData");
                    clearDom.innerHTML = "没有历史数据";
                  }
                }
              });
            }else{
              let mydata = [];
              let xData = [];
              for (let i = 0; i < this.oldInfoList.length; i++) {
                this.oldInfoList[i].value = this.oldInfoList[i].Value;
                this.oldInfoList[i].name = this.oldInfoList[i].UpdatedOn;
                xData = [...xData, ...[[this.oldInfoList[i].UpdatedOn]]];
              }
              if (this.oldInfoList.length > 0) {
                this.echartsHei = 338.5;
              }
              mydata = JSON.parse(JSON.stringify(this.oldInfoList));
              this.$nextTick(() => {
                if (this.oldInfoList.length > 0) {
                  this.lineChart = echarts.init(
                    document.querySelector(".content_echarts .chartsLine")
                  ); //获取第一个折线图
                  this.tuBiaoOption.series[0].data = mydata;
                  this.tuBiaoOption.xAxis.data = xData;
                  this.lineChart.setOption(this.tuBiaoOption);
                  this.lineChart.resize();
                  window.addEventListener("resize", () => {
                    this.$nextTick(() => {
                      this.lineChart.resize();
                    });
                  });
                } else {
                  if (this.lineChart) {
                    this.lineChart.dispose();
                    let clearDom = document.getElementById("noData");
                    clearDom.innerHTML = "没有历史数据";
                  }
                }
              });
            }
            
          }
        }
        if (this.dataMode == "chartsData") {
          this.isLoadingTable = false;
        }
      });
    },
    
    setMapTrack() {
      if (this.oldInfoList.length > 0) {
        let path = [];
        this.oldInfoList.map((e) => {
          if(path&&path.length>0){
            if(path[path.length-1][0]==e.Value.lng&&path[path.length-1][1]==e.Value.lat){}else{
              path.push([e.Value.lng, e.Value.lat])
            }
          }else{
            path.push([e.Value.lng, e.Value.lat])
          }
          
        });
        this.echartsHei = 500; //设置元素显示高度
        // let clearDom = document.getElementById("chartsLine");
        const map = new AMap.Map("chartsLine", {
          zoom: 17, //地图级别
          // viewMode: "2D",
          resizeEnable: true,
          center: path[0], // 设置地图中心点
        });
        let _this=this
        
        //启动页面
          _this.initPage(map,path);
      } else {
        let clearDom = document.getElementById("noData");
        clearDom.innerHTML = "<span>没有历史数据</span>";
      }
    },
    initPage(map,path) {
      // console.log(path,'path');
      //创建组件实例
      let that=this
      AMap.plugin('AMap.MoveAnimation', function(){
        var marker=null
        marker = new AMap.Marker({
          map: map,
          position: path[0],
          icon: "https://a.amap.com/jsapi_demos/static/demo-center-v2/car.png",
          offset: new AMap.Pixel(-13, -26),
        });

        // 绘制历史轨迹
        var polyline = new AMap.Polyline({
          // map: map,
          path: path,
          showDir:true,
          strokeColor: "#28F",  //线颜色
          // strokeOpacity: 1,     //线透明度
          strokeWeight: 6,      //线宽
          // strokeStyle: "solid"  //线样式
        }); 
        map.add(polyline); 
        // 驾驶途径过的轨迹
        var passedPolyline = new AMap.Polyline({
          // map: map,
          strokeColor: "#AF5",  //线颜色
          strokeWeight: 6,      //线宽
        });
        map.add(passedPolyline); 

        // 监听车辆移动事件
        marker.on('moving', function (e) {
          // 延长驾驶途径过的轨迹
          passedPolyline.setPath(e.passedPath);
          // 将车辆位置设置为地图中心点
          map.setCenter(e.target.getPosition(),true)
        });
        marker.on("click", (e) => {
          startAnimation()
        });
        // map.setFitView();
        
        // 开始移动
        window.startAnimation = function startAnimation () {
          marker.moveAlong(path, {
            // 每一段的时长
            duration: 500,//可根据实际采集时间间隔设置
            // JSAPI2.0 是否延道路自动设置角度在 moveAlong 里设置
            autoRotation: true,
          });
        };
        // 暂停移动
        window.pauseAnimation = function () {
          marker.pauseMove();
        };
        // 恢复移动
        window.resumeAnimation = function () {
          marker.resumeMove();
        };
        // 停止移动
        window.stopAnimation = function () {
          marker.stopMove();
        };
        startAnimation()
      });
      
      // //这里构建两条简单的轨迹，仅作示例
      // pathSimplifierIns.setData([
      //   {
      //     name: "轨迹0",
      //     path: path,
      //   },
      // ]);
    },
  },
};
</script>
<style lang="scss" scoped>
.text_button.el-button.is-plain:hover, .text_button.el-button.is-plain:focus{
  border: none;
}
::v-deep .el-dialog__body {
  padding: 10px 20px;
}

.state_con{
  display:flex;
  margin-right: -10px;
  flex-wrap: wrap;
  align-items: center;
  margin-top: 20px;
  justify-content: center;
  .state_li{
    display:flex;
    justify-content: flex-start;
    align-items: center;
    margin-right: 10px;
    .li_box{
      width: 15px;
      height: 15px;
      border-radius: 3px;
    }
    .li_text{
      font-size: 12px;
      color: #333333;
      margin-left: 5px;
    }
  }
}
</style>