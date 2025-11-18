<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div
      class="elbiaoge_elform"
      :style="{ 'min-height': 'calc(100vh - 136px)' }"
      v-loading="configLoading"
    >
      <div>
        <div class="header" style="border-top: none; padding: 0; border-bottom: 1px solid #dadada">
          <el-menu :default-active="activeDevice" active-text-color="#409eff" class="el-menu-demo shejiqi" mode="horizontal">
            <el-menu-item index="instanceInformation" @click="deviceSelect('instanceInformation')" >实例信息</el-menu-item>
            <el-menu-item v-if="ishasIot" index="runningState" @click="deviceSelect('runningState')" >运行状态</el-menu-item>
            <el-menu-item index="equipmentLocation" @click="deviceSelect('equipmentLocation')" v-if="positionInfo && positionInfo.enable" >设备位置</el-menu-item>
            <el-menu-item index="warning" @click="deviceSelect('warning')">报警工单</el-menu-item>
            <el-menu-item index="func" @click="deviceSelect('func')" v-if="ishasIot" >设备功能</el-menu-item>
            <el-menu-item index="abnormalData" @click="deviceSelect('abnormalData')" v-if="ishasIot" >异常数据</el-menu-item>
            <el-menu-item index="devicePlane" @click="deviceSelect('devicePlane')" v-if="isShowPlane">设备计划</el-menu-item>
            <el-menu-item index="onLineDebug" @click="deviceSelect('onLineDebug')" v-if="productInfos.Status == '0'&&isCheckPermi(['/IoTService/IotProduct/ListPage'])">在线调试</el-menu-item>
            

          </el-menu>
          <div class="name_text">
            {{ deviceInfos == null ? "" : deviceInfos.Name }}
          </div>
        </div>
        <div>
          <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" v-show="activeDevice == 'instanceInformation'">
            <deviceBasic
              ref="devBasicInfo"
              :deviceInfos="deviceInfos"
              :productInfos="productInfos"
              :canChangeDevice="canChangeDevice"
              @cancelSave="cancelSave"
              @changeDeviceProduct="changeDeviceProduct"
              @reloadDevice="getDeviceInfos"
            ></deviceBasic>
            <device-tags-info
              ref="tagsinfo"
              :deviceStorageConfig="deviceStorageConfig"
              :deviceInfos="deviceInfos"
              @labelInfoVisible="labelInfoVisible"
            ></device-tags-info>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" v-show="ishasIot && activeDevice === 'runningState'">
            <device-live ref="deviceLive" :deviceInfos="deviceInfos" :deviceStorageConfig="deviceStorageConfig" @openDeviceRunMap="openDeviceRunMap" @openInfoVisible="openInfoVisible"></device-live>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" v-if="showMap && activeDevice == 'equipmentLocation'">
            <div>
              <div class="guiji_div" v-if="deviceStorageConfig && positionInfo && positionInfo.mapcode">
                <el-button type="primary" @click="labelInfoVisible(positionInfo, true)">查看运行轨迹</el-button>
              </div>
              <div id="allmaptt" style="width: 100%; height: 500px"></div>
            </div>
          </div>
          <warn-list v-if="activeDevice == 'warning'" :isComponent="true" ref="warning-list" :filDeviceId="deviceInfos.Id"></warn-list>
          <div v-if="ishasIot && activeDevice == 'func'">
            <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" style="margin-top: 20px" v-if="productInfos.MonitorReportToken">
              <customFunc :deviceInfos="deviceInfos" :productInfos="productInfos"></customFunc>
            </div>
            <el-row style="margin-top: 20px;display: flex;justify-content: flex-start;flex-wrap: wrap;" v-else>
              <div class="li_con" v-for="(row, index) in attrData" :key="row.Code">
                <div class="li_info func_li">
                  <div class="title_name">{{ row.name }}</div>
                  <div class="params_con">
                    <div class="params_li">
                      <div class="params_lable">
                        {{ row.returnMsg ? row.returnMsg : row.description }}
                      </div>
                    </div>
                  </div>
                  <div class="func_btn">
                    <el-button type="primary" :disabled="row.disabled" :icon="matchesLoading ? '' : 'el-icon-video-play'" :loading="matchesLoading" @click="openFuncDia(row, index)">{{ matchesLoading ? "执行中" : "执 行" }}</el-button>
                  </div>
                </div>
              </div>
            </el-row>
          </div>
          <div style="margin-top: 20px" v-if="ishasIot&&activeDevice == 'abnormalData'">
            <abnormalData :deviceInfos="deviceInfos" :productInfos="productInfos"></abnormalData>
          </div>
          <div style="margin-top: 20px" v-show="activeDevice == 'devicePlane'" v-if="isShowPlane">
            <devicePlane :deviceInfos="deviceInfos" :productInfos="productInfos"></devicePlane>
          </div>
          <div style="margin-top: 20px" v-if="activeDevice == 'onLineDebug'">
            <deviceOnlineDebug :productInfos="productInfos" :deviceInfos="deviceInfos" />
          </div>
          <!-- <div style="margin-top: 20px" v-if="activeDevice == 'workSchedule'">
            <workScheduleInfo :deviceInfos="deviceInfos" />
          </div>
          <div style="margin-top: 20px" v-if="activeDevice == 'sequenceChart'">
            <sequenceChart :deviceInfos="deviceInfos" />
          </div> -->
        </div>
        <el-dialog title="位置" :visible.sync="deviceRunMap" :destroy-on-close="true" @close="deviceRunMap = false" :close-on-click-modal="false" width="60%">
          <div style="padding: 0 20px 20px">
            <div id="deviceRunMapId" style="width: 100%; height: 500px"></div>
          </div>
        </el-dialog>
        <!-- <el-dialog title="历史数据" :visible.sync="infoVisible" width="60%" @close="infoVisible = false" :close-on-click-modal="false"> -->
        <oldHistory
          ref="oldInfoCom"
          @closeHistoryDialog="closeHistoryDialog"
          :visible="infoVisible"
          :productInfos="productInfos"
          :activeAttr="activeAttr"
          :deviceInfos="deviceInfos"
          v-if="infoVisible"
          :isOnlyTable="isOnlyTable"
        ></oldHistory>
        <!-- </el-dialog> -->
        <el-dialog title="执行属性设置" :visible.sync="implementParams">
          <el-form ref="implementParamsForm" :model="implementParamsForm" label-width="100px" label-position="top" :rules="implementParamsForm.paramsRules">
            <el-table :data="implementParamsForm.inputsData" class="data_table">
              <el-table-column property="name" label="参数名称" width="200"></el-table-column>
              <el-table-column property="remark" label="备注" width="210">
                <template slot-scope="scope">
                  <div style="white-space: pre-wrap;word-break: break-all;width: 200px;">
                    {{ scope.row.remark }}
                  </div>
                </template>
              </el-table-column>
              <el-table-column label="值">
                <template slot-scope="scope">
                  <param-item :Item="scope.row" :disabled="scope.row.readOnly" @change="scope.row.codeVal = $event"></param-item>
                </template>
              </el-table-column>
            </el-table>
          </el-form>
          <div class="demo-drawer__footer" style="text-align: right; margin-top: 40px">
            <el-button @click="closeMatchesDrawer">取 消</el-button>
            <el-button type="primary" @click="carryAction" :loading="matchesLoading">{{ matchesLoading ? "执行中 ..." : "执 行" }}</el-button>
          </div>
        </el-dialog>
      </div>
    </div>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { DeviceInfo, deviceFuncList, deviceExeFunc } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
import { initMap } from "@/utils/amap";
import InfiniteLoading from "vue-infinite-loading";

import paramItem from "../funInput/paramItem.vue";
import oldHistory from "./oldHistory.vue"; //历史数据组件
import deviceBasic from "./deviceBasic.vue"; //历史数据组件
import deviceTagsInfo from "./deviceTagsInfo.vue"; //历史数据组件
import DeviceLive from "./deviceLive.vue";
import abnormalData from "./abnormalData.vue";
import devicePlane from "./devicePlane.vue";
import customFunc from "./customFunc.vue";
import deviceOnlineDebug from "./deviceOnlineDebug.vue";
import { getConfigKey } from "@/api/system/config.js";
import { checkPermi } from "@/utils/permission"; 
export default {
  mixins: [resizeTableCon],
  components: {
    InfiniteLoading,
    paramItem,
    oldHistory,
    deviceBasic,
    deviceTagsInfo,
    DeviceLive,
    abnormalData,
    devicePlane,
    customFunc,
    deviceOnlineDebug,
  },
  data() {
    return {
      showMap: true,
      //设备功能相关参数
      implementParamsForm: {
        functionId: "",
        inputsData: [],
        paramsRules: {
          codeVal: [{ required: true, trigger: "blur", message: "请输入" }],
        },
      },
      matchesLoading: false,
      implementParams: false,
      attrData: [],
      //设备功能相关参数
      //设备详情
      deviceRunMap: false, //是否显示位置
      infoVisible: false, //历史数据弹出层
      saveLoading: false,
      activeDevice: "instanceInformation",
      configLoading: false,
      deviceInfos: {}, //设备信息
      id: "",
      productInfos: {}, //协议信息
      isOnlyTable: false, //是否是只展示表格数据
      activeAttr: {}, //活动的设备历史数据相关属性信息
      canChangeDevice: false, //判断是否可以修改设备信息
      isExcuteWatch: false,
      issubscribeDevicefunc: false, //是否有订阅设备功能信息
      activeFuncIndex: null, //执行的功能的序号
      positionInfo: {}, //位置标签的信息
      isShowMap: true, //是否在添加地图key script成功，成功才可以初始化地图
      infoWindow: null, //腾讯地图窗体信息
      ishasIot: false, //是否是物联设备判断，非物联设备详情里面的运行状态、设备功能、异常数据不显示
      isShowPlane: false, //是否展示设备计划
      deviceStorageConfig: false, //设备的参数历史信息是否开启存储
    };
  },
  created() {
    // 是否展示设备计划
    getConfigKey("device.plane").then((res) => {
      this.isShowPlane = res.data == "false" ? true : false;
    });
  },
  beforeCreate() {
    initMap()
      .then((xx) => {
        this.isShowMap = true;
      })
      .catch((ex) => {
        // this.$message.error(ex);
      });
  },
  // activated钩子执行resize()方法
  activated() {
    this.$nextTick(() => {
      if (this.infoVisible && !this.isOnlyTable) {
        this.$refs.oldInfoCom.lineChartResize();
      }
    });
  },
  // 监听,当路由发生变化的时候执行
  watch: {
    $route(to, from) {
      if (to.path == "/iot/deviceManage/deviceDetail") {
        let pars = this.$route.query;
        if (pars.id) {
          this.id = pars.id;
        }
        if (pars.isCustom) {
          this.canChangeDevice = false;
        } else {
          this.canChangeDevice = true;
        }
        // console.log(this.canChangeDevice,'canChangeDevicecanChangeDevicecanChangeDevice');
        let activenavpath = "";
        activenavpath = this.activeDevice;
        this.activeDevice = "other";
        if (activenavpath == "equipmentLocation") {
          this.activeDevice = "equipmentLocation";
          this.showMap = false;
          this.$nextTick(async () => {
            this.showMap = true;
            await this.getDeviceInfos();
            setTimeout(() => {
              this.positionInfo = {};
              let mds = JSON.parse(this.productInfos.ModelTSL);
              let tags = mds.tags;
              tags.map((row) => {
                if (row.code == "position") {
                  this.positionInfo = row;
                  this.$forceUpdate();
                }
              });
              this.init(this.deviceInfos.Lat, this.deviceInfos.Lng, "allmaptt");
            }, 50);
            this.$refs.devBasicInfo.loadData(); //加载编辑时需要的设备相关信息
            if (this.ishasIot) {
              this.getdeviceFuncList();
            }
          });
        } else {
          this.$nextTick(async () => {
            await this.getDeviceInfos();
            if (this.ishasIot) {
              this.getdeviceFuncList();
            }
            this.deviceSelect(activenavpath, true);
          });
        }
      }
    },
  },
  async mounted() {
    // sessionStorage.removeItem("map.key");
    let pars = this.$route.query;
    if (pars.id) {
      this.id = pars.id;
    }
    if (pars.isCustom) {
      this.canChangeDevice = false;
    } else {
      this.canChangeDevice = true;
    }
    await this.getDeviceInfos();
    if (this.ishasIot) {
      this.getdeviceFuncList();
    }
    this.$nextTick(() => {
      this.isExcuteWatch = true;
    });
  },
  methods: {
    isCheckPermi(val) {
      return checkPermi(val)
    },
    subscribeFunc() {
      //订阅设备功能信息
      let that = this;
      this.$store.dispatch("mqttclient/getClient").then((client) => {
        let tkey = "newfun/" + this.deviceInfos.DeviceId;
        client.subscribe(tkey, (error) => {
          // console.log(error, "订阅消息");
          this.issubscribeDevicefunc = true;
          if (!error) {
            that.$store.commit("mqttclient/Add_Handler", {
              key: tkey,
              func: function (message) {
                let msgtxt = message.toString();
                if (msgtxt.indexOf("update") > -1) {
                  that.getdeviceFuncList();
                }
                if (msgtxt.indexOf("redirect") > -1) {
                  let urlStr = msgtxt.substring(8, msgtxt.length);
                  that.jumpFuncUrl(urlStr);
                }
                if (msgtxt.indexOf("show") > -1) {
                  let msgStr = msgtxt.substring(4, msgtxt.length);
                  that.attrData[that.activeFuncIndex].returnMsg = msgStr;
                  that.$forceUpdate();
                }
              },
            });
          }
        });
      });
    },
    jumpFuncUrl(val) {
      window.open(val, "_blank");
    },
    labelInfoVisible(item, isposition) {
      //打开弹出层
      if (this.productInfos.StorageConfig) {
        //有设置协议存储信息才可以查看历史数据
        let StorageConfig = JSON.parse(this.productInfos.StorageConfig);
        if (StorageConfig.enable && StorageConfig.enable != 0) {
          let attrObj = {};
          if (isposition) {
            attrObj = {
              Code: item.mapcode,
              Name: item.name,
              OptionType: item.option.type,
            };
          } else {
            attrObj = {
              Code: item.MapCode,
              Name: item.Name,
              OptionType: item.Option.type,
            };
          }
          this.infoVisible = true;
          this.isOnlyTable = false;
          this.activeAttr = attrObj;
        } else {
          if (isposition) {
            this.$message({
              message: "查看设备轨迹请先设置开发协议的存储方式为启用",
              type: "error",
              duration: 5 * 1000,
            });
          } else {
            this.$message({
              message: "查看历史数据请先设置开发协议的存储方式为启用",
              type: "error",
              duration: 5 * 1000,
            });
          }
        }
      }
    },
    openFuncDia(row, index) {
      //打开执行功能的弹窗表格
      // console.log("当前执行的功能定义", row);
      // console.log("当前调试的表格信息", this.attrData);
      this.activeFuncIndex = index;
      this.implementParamsForm.functionId = row.code;
      this.attrData[this.activeFuncIndex].isloading = true;
      if (row.inputs && row.inputs.length > 0) {
        for (let i = 0; i < row.inputs.length; i++) {
          row.inputs[i].text = "请输入" + row.inputs[i].name;
          // row.inputs[i].type = row.inputs[i].type;
          if (row.inputs[i].type == "int" || row.inputs[i].type == "float") {
            row.inputs[i].codeVal = null;
          } else {
            row.inputs[i].codeVal = "";
          }
        }
        let lis = JSON.parse(JSON.stringify(row.inputs));
        this.implementParamsForm.inputsData = JSON.parse(JSON.stringify(lis));
        this.resetForm("implementParamsForm");
        this.implementParams = true;
      } else {
        this.noInputCarryAction();
      }
    },
    getdeviceFuncList() {
      //获取设备功能
      deviceFuncList({ id: this.id }).then((res) => {
        // console.log("设备对应功能列表", res);
        this.attrData = res.data;
      });
    },
    closeMatchesDrawer() {
      this.implementParams = false;
    },
    carryAction() {
      if (this.$refs["implementParamsForm"]) {
        this.$refs["implementParamsForm"].validate((valid) => {
          // console.log(valid);
          if (valid) {
            this.matchesLoading = true;
            let obj = {};
            this.implementParamsForm.inputsData.forEach((its) => {
              obj[its.code] = its.codeVal;
            });
            deviceExeFunc({
              id: this.id,
              functionId: this.implementParamsForm.functionId,
              inputs: JSON.parse(JSON.stringify(obj)),
            })
              .then((res) => {
                if (res.code == 0) {
                  this.implementParams = false;
                  this.matchesLoading = false;
                  this.$message({
                    message: "执行完成",
                    type: "success",
                    duration: 5 * 1000,
                  });
                }
              })
              .catch((err) => {
                this.matchesLoading = false;
                console.log("报错", err);
                if (err.message) {
                  this.$msgbox.alert(err.message, "系统提示", {
                    confirmButtonText: "确定",
                    callback: () => {},
                  });
                }
              });
          }
        });
      }
    },
    noInputCarryAction() {
      //没有inputs直接执行功能
      this.matchesLoading = true;
      deviceExeFunc({
        id: this.id,
        functionId: this.implementParamsForm.functionId,
        inputs: {},
      })
        .then((res) => {
          if (res.code == 0) {
            this.matchesLoading = false;
            this.$message({
              message: "执行完成",
              type: "success",
              duration: 5 * 1000,
            });
          }
        })
        .catch((err) => {
          this.matchesLoading = false;
          // console.log("报错", err);
          if (err.message) {
            this.$msgbox.alert(err.message, "系统提示", {
              confirmButtonText: "确定",
              callback: () => {},
            });
          }
        });
    },
    openDeviceRunMap(lat, lng) {
      //打开显示地图
      this.deviceRunMap = true;
      this.$nextTick(() => {
        this.init(lat, lng, "deviceRunMapId");
      });
    },
    
    init(lat, lng, dom = "allmaptt") {
      let map = new AMap.Map(dom, {
        zoom: 15,
        viewMode: "2D",
        center: [lng, lat],
      });
      const marker = new AMap.Marker({
        position: [lng, lat],
        image: require('@/assets/images/address_map.png'),
        map: map,
      });
      const icon = new AMap.Icon({
        size: new AMap.Size(36, 36),
        imageOffset: new AMap.Pixel(0, 0),
        imageSize: new AMap.Size(36, 36),
      });
      let _this = this;
      marker.setIcon(icon);
      let line = _this.deviceInfos.Online == 1 ? "在线" : "离线";
      this.infoWindow = new AMap.InfoWindow({
        isCustom: true,
        draggable: false, //是否可拖动
        offset: new AMap.Pixel(-238, -205),
        center: [lng, lat],
        content:
          '<div id="map_info_card" class="info_card"><div class="title"><span class="title_name">' +
          _this.deviceInfos.Name +
          '</span><div id="map_close_img" class="close_img">' +
          '<span class="min top_img"></span>' +
          '<span class="min right_img"></span>' +
          '<span class="min bottom_img"></span>' +
          '<span class="min left_img"></span></div></div>' +
          '<div align="left" class="content">设备编号：' +
          _this.deviceInfos.DeviceId +
          "</div>" +
          '<div align="left" class="content">联网状态：' +
          line +
          "</div>" +
          '<span class="cancle bot"></span><span class="cancle top"></span></div>',
      });
      // console.log(marker,'markermarker');
      this.infoWindow.open(map,marker.getPosition());
      marker.on("click", function (e) {
        console.log(e,'eeeeeeeeeeeee');
        _this.infoWindow.open(map,marker.getPosition());
      });
      document.getElementById("map_close_img").addEventListener("click", function () {
        //点击id为div_link时调用的处理函数
        _this.infoWindow.close();
      });
    },
    closeInfoWindow() {
      this.infoWindow.close();
    },
    openInfoWindow() {
      this.infoWindow.open();
    },
    closeHistoryDialog(val) {
      //关闭
      this.infoVisible = val;
    },
    openInfoVisible(item, oldType) {
      //打开弹出层
      if (this.productInfos.StorageConfig) {
        let StorageConfig = JSON.parse(this.productInfos.StorageConfig);
        if (StorageConfig.enable && StorageConfig.enable != 0) {
          this.infoVisible = true;
          if (oldType && oldType == "onLine") {
            this.isOnlyTable = true;
            this.activeAttr = item;
          } else {
            this.isOnlyTable = false;
            this.activeAttr = item;
          }
        } else {
          this.$message({
            message: "查看历史数据请先设置开发协议的存储方式为启用",
            type: "error",
            duration: 5 * 1000,
          });
        }
      }
    },
    cancelSave() {
      //取消保存后重新加载设备信息
      this.getDeviceInfos();
    },
    changeDeviceProduct(val) {
      //改变设备协议
      this.deviceInfos.ProductId = val;
      productInfo({ id: this.deviceInfos.ProductId, notsl: false }).then(
        (rsp) => {
          this.productInfos = JSON.parse(JSON.stringify(rsp.data));
        }
      );
    },
    async getDeviceInfos() {
      //获取设备信息
      this.configLoading = true;
      let res = await DeviceInfo({ id: this.id });
      let rsp = await productInfo({ id: res.data.ProductId, notsl: false });
      if (res.code == 0) {
        this.deviceInfos = res.data;
        this.productInfos = JSON.parse(JSON.stringify(rsp.data));
        if (this.productInfos.StorageConfig) {
          //设置设备对应协议是否开启历史信息存储功能
          let StorageConfig = JSON.parse(this.productInfos.StorageConfig);
          if (StorageConfig.enable && StorageConfig.enable != 0) {
            this.deviceStorageConfig = true;
          } else {
            this.deviceStorageConfig = false;
          }
        } else {
          this.deviceStorageConfig = false;
        }

        this.positionInfo = {};
        let mds = JSON.parse(this.productInfos.ModelTSL);
        let tags = mds.tags;
        tags.map((row) => {
          if (row.code == "position") {
            this.positionInfo = row;
            this.$forceUpdate();
          }
        });
        if (this.productInfos.NetworkWay) {
          this.ishasIot = true;
        } else {
          this.ishasIot = false;
        }
        this.configLoading = false;
      }
      // console.log(this.productInfos, 'this.productInfos')
    },

   deviceSelect(path, isfirst) {
      //切换设备管理
      this.configLoading = true;
      this.activeDevice = path;
      if (this.activeDevice == "runningState") {
        if (!isfirst) {
          this.$refs.deviceLive.subscribeDeviceLive(true);
        } 
      } else if (this.activeDevice == "func") {
        if (!this.issubscribeDevicefunc) {
          this.subscribeFunc();
        }
      } else if (this.activeDevice == "equipmentLocation") {
        this.positionInfo = {};
        let mds = JSON.parse(this.productInfos.ModelTSL);
        let tags = mds.tags;
        tags.map((row) => {
          if (row.code == "position") {
            this.positionInfo = row;
          }
        });
        this.$nextTick(() => {
          console.log(this.isShowMap,'this.isShowMapthis.isShowMap');
          if (this.isShowMap) {
            this.init(this.deviceInfos.Lat, this.deviceInfos.Lng, "allmaptt");
          }
        });
      } else if (this.activeDevice == "warning") {
        this.$nextTick(() => {
          this.$refs["warning-list"].getList();
        });
      }
      this.configLoading = false;
    },
  },
};
</script>
<style lang="less">

.info_card {
  display: inline-block;
  margin: 50px auto;
  position: absolute;
  width: 450px;
  // height: 100px;
  background-color: #c7c9c8;
  border: 5px solid #ffffff;
  color: #000000;
  padding-bottom: 10px;
}

.info_card .title {
  width: 100%;
  line-height: 20px;
  background-color: #000000;
  color: #ffffff;
  padding: 10px 0;
  padding-right: 35px;
  vertical-align: text-top;
  display: flex;
  align-items: center;
}

.title span.title_name {
  position: relative;
  top: 0;
  left: 10px;
  font-size: 18px;
}

.info_card .title .close_img {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 20px;
  height: 20px;
  background-color: #ffffff;
  cursor: pointer;
}

.info_card .title .close_img .min {
  width: 0;
  height: 0;
  font-size: 0;
  overflow: hidden;
  position: absolute;
  border-width: 10px;
}

.info_card .title .close_img .top_img {
  border-style: solid dashed dashed;
  border-color: #000000 transparent transparent transparent;
  top: -2px;
}

.info_card .title .close_img .right_img {
  border-style: solid dashed dashed;
  border-color: transparent #000000 transparent transparent;
  left: 2px;
}

.info_card .title .close_img .bottom_img {
  border-style: solid dashed dashed;
  border-color: transparent transparent #000000 transparent;
  top: 2px;
}

.info_card .title .close_img .left_img {
  border-style: solid dashed dashed;
  border-color: transparent transparent transparent #000000;
  left: -2px;
}

.info_card span.cancle {
  width: 0;
  height: 0;
  font-size: 0;
  overflow: hidden;
  position: absolute;
}

.info_card span.bot {
  border-width: 20px;
  border-style: solid dashed dashed;
  border-color: #ffffff transparent transparent;
  left: 205px;
  bottom: -40px;
}

.info_card span.top {
  border-width: 20px;
  border-style: solid dashed dashed;
  border-color: #c7c9c8 transparent transparent;
  left: 205px;
  bottom: -33px;
}

.info_card .content {
  margin-top: 5px;
  margin-left: 10px;
  margin-right: 10px;
}
</style>
<style lang="less" scoped>
.guiji_div {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  margin-bottom: 10px;
}
#map_track {
  display: flex;
  justify-content: center;
  align-items: center;
}

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

.content_echarts .chartsLine {
  margin-top: 10px;
  width: 100%;
  height: 138.5px;
  // background-color: #05185c;
  border-radius: 5px;
}

.choice_time {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

.li_con {
  width: 25%;
  padding-right: 20px;
  box-sizing: border-box;
  margin-bottom: 10px;

  .li_info {
    // height: 120px;
    background-color: #fafafa;
    border: 1px solid #f0f0f0;
    border-radius: 5px;
    margin: 0 auto;
    padding: 0 auto;
    padding: 25px 20px;
    &.func_li {
      min-height: 150px;
      font-size: 16px;
      color: #333;
      position: relative;
      .params_con {
        margin-top: 10px;
        font-size: 12px;
        color: #999999;
        width: calc(100% - 90px);
        .params_li {
          line-height: 22px;
          //flex-wrap: wrap;
          display: flex;
          justify-content: flex-start;
          align-items: flex-start;
          // word-warp:break-word;
          word-break: keep-all;
          .params_lable {
            word-break: break-all;
            &.jump {
              cursor: pointer;
            }
          }
        }
      }
      .func_btn {
        position: absolute;
        right: 20px;
        bottom: 20px;
        width: 80px;
        box-sizing: border-box;
        .el-button {
          width: 80px;
          box-sizing: border-box;
          padding-left: 0;
          padding-right: 0;
        }
      }
    }

    li {
      list-style: none;
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: #575757;
      font-size: 12px;

      i {
        cursor: pointer;
      }
    }

    li.val {
      padding: 25px 0;
      font-size: 20px;
      color: #000000;
      font-weight: 700;

      i {
        cursor: pointer;
      }
    }

    li.time {
      color: #000050;
      margin-top: 12px;
      font-size: 14px;
    }
  }
}

/deep/ .header {
  //   min-width: 980px;
  background-color: #ffffff;
  border-top: 1px solid #dadada;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  // justify-content: space-between;
  //   line-height: 70px;
  //   height: 70px;
  position: relative;
  padding: 10px 0;

  .el-menu {
    top: 0;
    // z-index: 999;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    width: 100%;
  }

  .el-menu.el-menu--horizontal {
    border-bottom: none;
  }

  .shejiqi {
    height: 38px;
    line-height: 38px;
    border: none;
  }

  .shejiqi .el-menu-item {
    padding: 0;
    margin: 0 25px;
    height: 38px;
    line-height: 38px;
    font-size: 16px;
  }

  .publish {
    position: absolute;
    top: 10px;
    right: 50px;
    // z-index: 1000;

    i {
      margin-right: 6px;
    }

    button {
      border-radius: 15px;
      margin-right: 10px;
    }
  }

  .back {
    position: absolute;
    // z-index: 1000;
    top: 15px;
    left: 20px;

    // font-size: small;
    .return_button {
      margin-right: 6px;
      background: #ffffff;
      color: #78829d;
      width: 18px;
      height: 18px;
      line-height: 18px;
      // font-size: 16px;
      padding: 0;
      text-align: center;
      border: 1px solid #78829d;
    }

    span {
      i {
        border-radius: 10px;
        padding: 7.8px;
        // font-size: 20px;
        color: #ffffff;
        margin: 0 10px;
      }
    }
  }

  .scale {
    z-index: 999;
    position: absolute;
    left: 22px;
    top: 10px;

    span {
      margin: 0 10px;
      // font-size: 15px;
      color: #7a7a7a;
      width: 50px;
    }
  }
  .name_text {
    display: flex;
    justify-content: center;
    align-items: center;
    padding-right: 10px;
    white-space: nowrap;
    font-size: 20px;
    color: rgba(76, 121, 255, 1);
    font-weight: bold;
  }
}
</style>