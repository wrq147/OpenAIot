<template>
  <div v-loading="isLoadingTable">
    <div class="choice_time">
      <!-- <el-radio-group v-model="showTime" @input="setShowTime">
        <el-radio-button label="今日"></el-radio-button>
        <el-radio-button label="近一周"></el-radio-button>
        <el-radio-button label="近一月"></el-radio-button>
      </el-radio-group> -->
      <el-date-picker
        v-model="choiceTime"
        type="datetimerange"
        :unlink-panels="true"
        range-separator="至"
        start-placeholder="开始日期"
        end-placeholder="结束日期"
        align="right"
        @change="loadDeviceOldInfo"
      ></el-date-picker>
      <el-select
        v-model="activeCode"
        placeholder="请选择"
        style="margin-left: 20px"
        @change="changeActiveCode"
      >
        <el-option
          v-for="item in propertiesList"
          :key="item.code"
          :label="item.name"
          :value="item.code"
        >
        </el-option>
      </el-select>
      <el-button
        type="primary"
        icon="el-icon-search"
        @click="changeActiveCode"
        style="margin-left: 10px"
        >搜索</el-button
      >
    </div>
    <div
      class="header"
      style="
        border-top: none;
        padding: 0;
        border-bottom: 1px solid #dadada;
        margin-top: 20px;
      "
    >
      <el-menu
        :default-active="dataMode"
        active-text-color="#409eff"
        class="el-menu-demo shejiqi"
        mode="horizontal"
      >
        <el-menu-item index="tableData" @click="dataModeSelect('tableData')"
          >列表</el-menu-item
        >
        <el-menu-item index="chartsData" @click="dataModeSelect('chartsData')"
          >图表</el-menu-item
        >
      </el-menu>
    </div>
    <div v-show="dataMode == 'tableData'">
      <div style="height: 500px">
        <el-table
          :data="oldInfoList"
          style="width: 100%; margin-top: 20px; min-height: 138.5px"
          height="500"
          :header-row-style="{ 'background-color': '#B5B5B5' }"
          v-loading="isLoadingTable"
          highlight-current-row
        >
          <el-table-column prop="Name" label="属性名称"></el-table-column>
          <!-- <el-table-column prop="Code" label="属性代码"></el-table-column> -->
          <el-table-column prop="Value" label="属性值">
            <template slot-scope="scope">
              <div v-if="activeAttr.OptionType == 'geo'">
                <div>经度：{{ scope.row.Value.lat }}</div>
                <div>纬度：{{ scope.row.Value.lng }}</div>
              </div>
              <div v-else>{{ scope.row.Value }}</div>
            </template>
          </el-table-column>
          <el-table-column prop="Unit" label="单位"></el-table-column>
          <el-table-column
            prop="UpdatedOn"
            sortable
            label="更新时间"
            :sort-orders="['ascending']"
          ></el-table-column>
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
            <infinite-loading
              @infinite="loadMore"
              ref="infiniteLoading"
              :distance="3"
              spinner="bubbles"
              :identifier="infiniteId"
              force-use-infinite-wrapper=".el-table__body-wrapper"
            >
              <!--   orce-use-infinite-wrapper 属性在存在多个 el-table 需要更详细的css选择器   -->
              <div class="no-more" slot="no-more"></div>
              <div class="no-more" slot="no-results"></div>
              <div class="no-more" slot="error">出错了</div>
            </infinite-loading>
            <div
              class="no-more"
              style="text-align: center"
              v-if="oldInfoList && oldInfoList.length > 0 && status == 'noMore'"
            >
              没有更多数据
            </div>
          </template>
        </el-table>
      </div>
    </div>
    <div v-show="dataMode == 'chartsData'" class="content_echarts">
      <pagination
        v-show="dataTotal > 0"
        :total="dataTotal"
        :page.sync="deviceOldQuery.pageNum"
        :limit.sync="deviceOldQuery.pageSize"
        :pageSizes="pageSizes"
        @pagination="getDeviceOldInfo"
        layout="sizes, next"
      />
      <div v-if="echartsCodeGroup && echartsCodeGroup.length > 0">
        <div
          v-for="item in echartsCodeGroup"
          :key="item.code"
          :id="'chartsLine_' + item.code"
          :class="'chartsLine_' + item.code"
          :style="{ width: ' 100%', height: echartsHei + 'px' }"
        ></div>
      </div>
      <div
        v-else
        class="noData"
        id="noData"
        :style="{ width: ' 100%', height: echartsHei + 'px' }"
      >
        没有异常数据
      </div>
    </div>
  </div>
</template>

<script>
import { DeviceOldInfo, abnormalDataInfo } from "@/api/rules/device";
import InfiniteLoading from "vue-infinite-loading";
import * as echarts from "echarts";
var dayjs = require("@/utils/day.js");
export default {
  name: "deviceOldHistory",
  components: { InfiniteLoading },
  props: {
    productInfos: {
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
  },
  data() {
    return {
      activeAttr: {},
      echartsHei: 138.5, //设置图表高度
      dataMode: "tableData", //数据展示方式
      showTime: "", //显示的时间范围
      choiceTime: [], //选择的时间
      //   infoVisible: false, //历史数据弹出层
      oldInfoList: [], //设备历史数据
      isLoadingTable: false, //是否正在加载表格数据
      //   activeAttr: {},
      tuBiaoOption: {}, //图表数据
      // isShowMap: true,
      lineChart: null, //绘制图标
      status: "loading",
      infiniteId: new Date(), //滚动组件的识别号，改变时刷新
      deviceOldQuery: {
        pageNum: 1,
        pageSize: 50,
      },
      isOver: false,
      propertiesList: [], //协议属性列表
      activeCode: "",
      errorList: [], //异常数据
      echartsCodeGroup: [],
      dataTotal: 0,
      pageSizes: [30, 50, 100],
      isLastData: false,
      inxErrorList: [], //异常数据的图表展示数据
      maydata: [], //异常数据的图表展示数据
      xData: [], //异常数据的图表展示数据
    };
  },
  watch: {
    deviceInfos:{
      deep:true,
      immediate:true,
      handler(to, from) {
        // console.log("变化的", to);

        this.initTuBiao();
        this.getAttribute();
      }
    },
  },
  mounted() {},
  methods: {
    changeActiveCode() {
      //改变活动的代码
      let activeItem = this.propertiesList.find(
        (row) => row.code == this.activeCode
      );
      this.openInfoVisible(activeItem);
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
          item.option.type == "geo"
      );
      if (this.propertiesList && this.propertiesList.length > 0) {
        // this.activeCode = this.propertiesList[0].code;
        // this.openInfoVisible(this.propertiesList[0]);
        this.openInfoVisible();
      }
    },
    openInfoVisible(item) {
      //打开弹出层
      if (this.productInfos.StorageConfig) {
        let StorageConfig = JSON.parse(this.productInfos.StorageConfig);
        if (StorageConfig.enable && StorageConfig.enable != 0) {
          if (item) {
            this.activeAttr = {
              Code: item.code,
              Name: item.name,
              OptionType: item.option.type,
            };
          } else {
            this.activeAttr = {};
          }

          this.deviceOldQuery.pageNum = 1;
          this.deviceOldQuery.pageSize = 50;
          this.oldInfoList = [];
          this.echartsCodeGroup = [];
          // this.setShowTime();
          this.getDeviceOldInfo();

          // this.activeAttr = item;
        } else {
          this.$message({
            message: "查看历史数据请先设置开发协议的存储方式为启用",
            type: "error",
            duration: 5 * 1000,
          });
        }
      }
    },
    loadMore($state) {
      //无线滚动加载更多
      if (this.status == "more") {
        this.deviceOldQuery.pageNum++;
        this.getDeviceOldInfo($state);
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
        dataset: {
          // dimensions: [
          //   "Value",
          //   "Life Expectancy",
          //   "Population",
          //   "Country",
          //   { name: "UpdatedOn", type: "ordinal" },
          // ],
          source: [],
        },
        series: [
          {
            name: "异常数据",
            type: "scatter",
            datasetIndex: 0,
          },
          {
            name: "历史数据",
            data: [],
            type: "line",
            datasetIndex: 1,
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
                // color: (params) => {
                //   // console.log(this.errorList, "异常数据");
                //   let dateList = this.errorList.map((row) => row.UpdatedOn);
                //   // console.log("params参数", params);
                //   if (dateList.includes(params.data.name)) {
                //     return "#FF3535";
                //   } else {
                //     return "rgba(41, 83, 255, 1)";
                //   }
                // },
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
    dataModeSelect(mode) {
      //选择数据展示方式
      this.dataMode = mode;
      this.deviceOldQuery.pageNum = 1;
      this.oldInfoList = [];
      this.echartsCodeGroup = [];
      this.getDeviceOldInfo();
    },
    loadDeviceOldInfo() {
      this.deviceOldQuery.pageNum = 1;
      this.deviceOldQuery.pageSize = 50;
      this.oldInfoList = [];
      this.echartsCodeGroup = [];
      this.getDeviceOldInfo();
    },
    async getDeviceAbnormalData() {},
    async getDeviceOldInfo($state) {
      //获取设备的历史数据
      if (this.dataMode == "chartsData" || this.deviceOldQuery.pageNum == 1) {
        this.status = "loading";
        this.isLoadingTable = true;
        this.isOver = false;
        this.oldInfoList = [];
        if (this.deviceOldQuery.pageNum == 1) {
          this.dataTotal = 0;
          this.inxErrorList = [];
          this.mydata = [];
          this.xData = [];
        }
        if (this.$refs.infiniteLoading) {
          this.$refs.infiniteLoading.stateChanger.reset();
        }
      }
      let obj = {};
      if (this.choiceTime && this.choiceTime.length) {
        obj = this.addDateRange(
          { Id: this.deviceInfos.Id, Code: this.activeAttr.Code },
          this.choiceTime
        );
        obj.beginTime = this.parseTime(obj.beginTime);
        obj.endTime = this.parseTime(obj.endTime);
      } else {
        //没有时间设置默认时间日期
        obj = { Id: this.deviceInfos.Id, Code: this.activeAttr.Code };
      }

      if (this.dataMode == "tableData") {
        obj.pageNum = this.deviceOldQuery.pageNum;
        obj.pageSize = this.deviceOldQuery.pageSize;
      } else {
        obj.pageNum = this.deviceOldQuery.pageNum;
        obj.pageSize = this.deviceOldQuery.pageSize;
      }
      try {
        let res = {};
        let res2 = await abnormalDataInfo(obj);

        if (
          this.dataMode == "chartsData" &&
          res2.data &&
          res2.data.List &&
          res2.data.List.length > 0
        ) {
          if (res2.data.List.length < this.deviceOldQuery.pageSize) {
            if (this.isLastData) {
              this.dataTotal = this.dataTotal;
            } else {
              this.dataTotal =
                this.dataTotal + res2.data.Total - this.deviceOldQuery.pageSize;
              this.isLastData = true;
            }
          } else {
            if (obj.pageNum == 1) {
              this.dataTotal = this.dataTotal + res2.data.Total * 2;
            } else {
              this.dataTotal = this.dataTotal + res2.data.Total;
            }
            this.isLastData = false;
          }
          let times = res2.data.List.map((rw) => {
            return new Date(rw.UpdatedOn);
          });
          const minTime = new Date(
            Math.min(...times.map((time) => time.getTime()))
          );
          const maxTime = new Date(
            Math.max(...times.map((time) => time.getTime()))
          );
          let startDate = dayjs(minTime).format("YYYY-MM-DD HH:mm:ss");
          let endDate = dayjs(maxTime).format("YYYY-MM-DD HH:mm:ss");
          obj.beginTime = this.parseTime(startDate);
          obj.endTime = this.parseTime(endDate);
          delete obj.pageNum;
          delete obj.pageSize;
          res = await DeviceOldInfo(obj);
        }
        if (this.dataMode == "chartsData") {
          //图表中设置历史数据展示。异常数据特定点展示
          this.oldInfoList = [];
          this.echartsCodeGroup = [];
          if (res.data && res.data.List) {
            this.oldInfoList = JSON.parse(JSON.stringify(res.data.List)); //oldInfoList取历史数据
          }
          let errorList = JSON.parse(JSON.stringify(res2.data.List));
          this.errorList = JSON.parse(JSON.stringify(res2.data.List));
          let afterGroupErrorList = this.listGrouping(errorList, "Code");
          let afterGroupHistoryList = this.listGrouping(
            this.oldInfoList,
            "Code"
          );
          if (errorList && errorList.length > 0) {
            for (let codeKey in afterGroupErrorList) {
              let groupObj = {
                oldList: afterGroupHistoryList[codeKey],
                errorList: afterGroupErrorList[codeKey],
                code: codeKey,
              };
              this.echartsCodeGroup.push(groupObj);
            }
          } else {
            this.errorList = [];
          }
        } else {
          this.oldInfoList = [...this.oldInfoList, ...res2.data.List]; //oldInfoList取异常数据
          if (res2.data.List.length < this.deviceOldQuery.pageSize) {
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
          if (this.isLoadingTable) {
            this.isLoadingTable = false;
          }
        }
        if (this.dataMode == "chartsData") {
          if (this.activeAttr.OptionType == "geo") {
            this.$nextTick(() => {
              this.setMapTrack();
            });
          } else {
            if (this.echartsCodeGroup && this.echartsCodeGroup.length > 0) {
              for (let ix = 0; ix < this.echartsCodeGroup.length; ix++) {
                let attrCode = this.echartsCodeGroup[ix].code;
                // let mydata = [];
                // let xData = [];
                let inxOldInfoList = this.echartsCodeGroup[ix].oldList;
                // let inxErrorList = [];
                for (let i = 0; i < inxOldInfoList.length; i++) {
                  inxOldInfoList[i].value = inxOldInfoList[i].Value;
                  inxOldInfoList[i].name = inxOldInfoList[i].UpdatedOn;
                  this.xData = [
                    ...this.xData,
                    ...[[inxOldInfoList[i].UpdatedOn]],
                  ];
                }
                let errLis = this.echartsCodeGroup[ix].errorList.map((row) => {
                  return [row.UpdatedOn, row.Value];
                });
                this.inxErrorList = [...this.inxErrorList, ...errLis];
                if (inxOldInfoList.length > 0) {
                  this.echartsHei = 338.5;
                }
                this.mydata = [...this.mydata, ...inxOldInfoList];
                let lineChart = {};
                this.$nextTick(() => {
                  if (
                    inxOldInfoList.length > 0 &&
                    this.inxErrorList.length > 0
                  ) {
                    lineChart = echarts.init(
                      document.querySelector(
                        ".content_echarts .chartsLine_" + attrCode
                      )
                    ); //获取第一个折线图
                    let tuBiaoOption = JSON.parse(
                      JSON.stringify(this.tuBiaoOption)
                    );
                    tuBiaoOption.dataset.source = this.inxErrorList;
                    tuBiaoOption.series[1].data = this.mydata;
                    tuBiaoOption.xAxis.data = this.xData;
                    lineChart.setOption(tuBiaoOption);
                    lineChart.resize();
                    window.addEventListener("resize", () => {
                      this.$nextTick(() => {
                        lineChart.resize();
                      });
                    });
                  }
                  if (this.isLoadingTable) {
                    this.isLoadingTable = false;
                  }
                });
              }
            } else {
              if (this.isLoadingTable) {
                this.isLoadingTable = false;
              }
            }
          }
        }
        // if (this.dataMode == "chartsData") {
        //   this.isLoadingTable = false;
        // }
      } catch (error) {
        console.log("错误", error);
      }
    },
    listGrouping(list, groupParams) {
      return list.reduce((result, currentItem) => {
        // 使用 key 函数提取分组键，如果未定义则直接使用属性名
        const groupKey =
          typeof groupParams === "function"
            ? key(currentItem)
            : currentItem[groupParams];

        // 确保 result 对象中有对应分组的数组
        if (!result[groupKey]) {
          result[groupKey] = [];
        }

        // 将当前项添加到对应分组的数组中
        result[groupKey].push(currentItem);

        return result;
      }, {});
    },
    setMapTrack() {
      //设置地图轨迹
      //设置图标
      if (this.errorList.length > 0) {
        let path = [];
        this.errorList.forEach((e) => {
          path.push(new TMap.LatLng(e.Value.lat, e.Value.lng));
        });
        this.echartsHei = 500; //设置元素显示高度
        let clearDom = document.getElementById("chartsLine2");
        clearDom.innerHTML = "";
        let map = new TMap.Map(document.getElementById("chartsLine2"), {
          center: path[0],
          zoom: 15,
        });
        let marker = new TMap.MultiMarker({
          map,
          styles: {
            markersty: new TMap.MarkerStyle({
              width: 25,
              height: 38,
              anchor: { x: 12, y: 32 },
            }),
          },
          geometries: [
            {
              //小车marker的位置信息
              id: "car", //因MultiMarker支持包含多个点标记，因此要给小车一个id
              styleId: "markersty", //绑定样式
              position: path[0], //初始坐标位置
            },
          ],
        });
        if (this.errorList.length > 1) {
          // //画出轨迹
          let polylineLayer = new TMap.MultiPolyline({
            id: "polyline-layer", //图层唯一标识
            map: map, //绘制到目标地图
            //折线样式定义
            styles: {
              style_blue: new TMap.PolylineStyle({
                color: "#3777FF", //线填充色
                width: 6, //折线宽度
                borderWidth: 5, //边线宽度
                borderColor: "#FFF", //边线颜色
                lineCap: "round", //线端头方式
              }),
            },
            //折线数据定义
            geometries: [
              {
                id: "pl_1", //折线唯一标识，删除时使用
                styleId: "style_blue", //绑定样式名
                paths: path,
              },
            ],
          });
          //调用moveAlong，实现小车移动
          marker.moveAlong(
            {
              car: {
                //设置让"car"沿"path"移动，速度70公里/小时
                path,
                speed: 70,
              },
            },
            {
              autoRotation: true, //车头始终向前（沿路线自动旋转）
            }
          );
        }
      } else {
        let clearDom = document.getElementById("noData");
        clearDom.innerHTML = "<span>没有历史数据</span>";
      }
    },
  },
};
</script>
<style lang="less" scoped>
.content_echarts {
  padding-bottom: 10px;
  width: 100%;

  // background-color: #001046;
  .noData {
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 14px;
    color: #dddddd;
  }
}
</style>
