<template>
  <div>
    <el-alert v-if="productInfos.Status == 1" how-icon title="发布状态的协议无法接收到设备上报消息和设备下发消息" type="warning">
    </el-alert>
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" v-loading="dataLoading">
      <div class="debug_con">
        <div class="debugLeft">
          <el-form ref="debugFrom" :model="debugFrom" label-width="80px" :inline="true">
            <el-form-item>
              <el-select style="width: 130px" v-model="debugFrom.debugType" placeholder="请选择" @change="changeDebugType">
                <el-option label="文本调试" value="文本调试"></el-option>
                <el-option label="属性调试" value="属性调试" v-if="CanModbus == false"></el-option>
                <el-option label="功能调试" value="功能调试"></el-option>
                <el-option label="特殊消息" value="特殊消息"></el-option>
                <el-option label="事件消息" value="事件消息"></el-option>
                <el-option label="Modbus调试" value="Modbus调试" v-if="CanModbus == true"></el-option>
              </el-select>
            </el-form-item>
          </el-form>
          <div class="attrList">
            <!-- <div class="funcbtn">
                <el-button type="primary" plain>执行</el-button>
              </div>-->
            <el-row :gutter="10">
              <div style="padding: 0 5px; margin-top: 15px" v-if="debugFrom.debugType == '文本调试'">
                <div class="txtdghd">
                  <span>请输入下发的文本</span>
                  <el-checkbox v-model="isHex">HEX</el-checkbox>
                </div>
                <el-input type="textarea" :rows="12" v-model="DebugText"></el-input>
                <br />
                <div style="display: flex; justify-content: flex-end">
                  <el-button style="margin-top: 15px" type="primary" @click="onSendTxt" >发送</el-button>
                </div>
              </div>
              <div v-else-if="debugFrom.debugType == '特殊消息'" style="padding-top: 15px;display: flex;flex-direction: column;align-items: center;">
                <div style="width: 80%">
                  <el-button @click="onSendBind" type="primary" style="width: 100%">发送协议升级</el-button>
                </div>
                <div style="margin-top: 15px; width: 80%">
                  <el-button @click="onSendICCID" type="primary" style="width: 100%">发送绑定物联卡</el-button>
                </div>
                <div style="margin-top: 15px; width: 80%">
                  <el-button @click="onSendConnect" type="danger" style="width: 100%">发送上线消息</el-button>
                </div>
                <div style="margin-top: 15px; width: 80%">
                  <el-button @click="onSendDisconnect" type="danger" style="width: 100%">发送离线消息</el-button>
                </div>
                <div style="margin-top: 15px; width: 80%">
                  <el-button @click="onDelHistory" type="warning" style="width: 100%">删除历史数据</el-button>
                </div>
              </div>
              <div v-else-if="debugFrom.debugType == '事件消息'" style="padding-top: 15px;display: flex;flex-direction: column;align-items: center;">
                <el-table :data="attrData" style="width: 100%" :header-row-style="{ 'background-color': '#B5B5B5' }">
                  <el-table-column prop="name" label="事件名称" align="center"></el-table-column>
                  <el-table-column label="操作" align="right" width="248" class-name="small-padding fixed-width">
                    <template slot-scope="scope">
                      <el-button type="primary" icon="el-icon-video-play" plain @click="implementRowData(scope.row, scope.$index)">执行</el-button>
                    </template>
                  </el-table-column>
                </el-table>
              </div>
              <el-table v-else :data="attrData" style="width: 100%" :header-row-style="{ 'background-color': '#B5B5B5' }">
                <el-table-column v-if="debugFrom.debugType == 'Modbus调试'" prop="Name" label="名称" align="center"></el-table-column>
                <el-table-column v-else prop="name" label="名称" align="center"></el-table-column>
                <el-table-column label="操作" align="right" width="248" class-name="small-padding fixed-width">
                  <template slot-scope="scope">
                    <el-button type="primary" icon="el-icon-video-play" plain @click="implementRowData(scope.row, scope.$index)">执行</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </el-row>
          </div>
        </div>
        <div class="debugRecord">
          <div class="recordtitle">
            <div>实时日志</div>
            <div>
              <el-select style="margin-right: 15px; width: 100px" v-model="diplayType" placeholder="请选择">
                <el-option label="文本" value="文本"></el-option>
                <el-option label="Hex" value="Hex"></el-option>
              </el-select>
              <el-button type="danger" icon="el-icon-refresh" size="mini" @click="clearRecords" plain>清空日志</el-button>
            </div>
          </div>
          <div class="record_list">
            <mq-console ref="mq-console" :xkey="debugFrom.debugDevice && deviceListMap.get(debugFrom.debugDevice) ? deviceListMap.get(debugFrom.debugDevice).DeviceId : ''"
              :isSubs="true"
              :displayWay="diplayType"
            ></mq-console>
          </div>
        </div>
      </div>
      <el-dialog title="执行属性设置" :visible.sync="implementParams">
        <el-form ref="implementParamsForm" :model="implementParamsForm" label-width="100px" label-position="top" :rules="implementParamsForm.paramsRules">
          <el-table :data="implementParamsForm.inputsData" class="data_table">
            <el-table-column property="name" label="参数名称" width="200"></el-table-column>
            <el-table-column  property="type" label="输入类型" width="200"></el-table-column>
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

      <el-dialog title="执行删除历史数据" :visible.sync="deldlgvis" width="600px">
        <el-form ref="form" :model="delform" label-width="80px">
          <el-form-item label="时间范围">
            <el-date-picker v-model="deldateRange" :picker-options="pickerOptions" value-format="yyyy-MM-dd HH:mm:ss" type="datetimerange"
              range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期">
            </el-date-picker>
          </el-form-item>
          <!-- <el-form-item label="属性">
              <el-select v-model="delform.code" filterable remote reserve-keyword placeholder="请选择属性" clearable>
                  <el-option v-for="item in propertieslist" :key="item.code" :label="item.name" :value="item.code"></el-option>
                </el-select>
            </el-form-item> -->
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitDelForm">执 行</el-button>
          <el-button @click="deldlgvis = false">取 消</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>
  <script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  channelInfo,
  implementModbus,
  implementProperty,
  implementFunc,
  implementText,
  implementMsg,
  implementEvent,
  implementDelHis,
} from "@/api/rules/productModel";
import MqConsole from "../debugTable/console.vue";
import paramItem from "../funInput/paramItem.vue";
export default {
  mixins: [resizeTableCon],
  props: ["productInfos", "deviceInfos"],
  components: { MqConsole, paramItem },
  data() {
    return {
      //在线调试
      implementParamsForm: {
        functionId: "",
        inputsData: [],
        paramsRules: {
          codeVal: [{ required: true, trigger: "blur", message: "请输入" }],
        },
      },
      matchesLoading: false,
      implementParams: false,
      // inputsData: [],
      dataLoading: true,
      debugFrom: {
        debugDevice: "", //调试设备
        debugType: "文本调试",
      },
      attrData: [],
      CanModbus: false,
      deviceListMap: new Map(),
      isFirstConnect: true,
      DebugText: "",
      diplayType: "文本",
      isHex: false,
      deviceOption: [], //过滤后的调试设备列表
      optionLoading: false,
      deldlgvis: false,
      delform: {},
      deldateRange: [],
      pickerOptions: {
        disabledDate(time) {
          const tomorrow = new Date();
          tomorrow.setDate(tomorrow.getDate() + 1);
          return time.getTime() > tomorrow.getTime();
        },
      },
      propertieslist: [],
      isStartconnect:false
    };
  },

  watch: {
    productInfos: {
      handler(newVal, oldVal) {
        this.initAsync();
      },
      immediate: true, //立即监听
    },
    deviceInfos: {
      handler(newVal, oldVal) {
        this.$nextTick(()=>{
          this.choiceDeviceVal();
        })
      },
      immediate: true, //立即监听
    },
  },
  beforeDestroy(){
    let that=this
    if(this.isStartconnect&&this.deviceInfos.DeviceId){
      this.$refs["mq-console"].disconnect(
        that.deviceInfos.DeviceId,
        () => {}
      );
    }
    
  },
  methods: {
    async initAsync() {
      this.dataLoading = true;
      this.debugFrom.debugDevice = this.deviceInfos.Id;
      // if(this.productInfos&&this.productInfos.ModelTSL){
      //   let proModelTSL=JSON.parse(this.productInfos.ModelTSL)
      //   this.propertieslist=proModelTSL.properties
      //   console.log(proModelTSL,this.propertieslist,"属性选择111");
      //   this.propertieslist.unshift({name:'全部',code:'-1'})
      // }

      // await this.getDeviceList();
      await this.getChannelInfo();
    },
    clearRecords() {
      this.$refs["mq-console"].clear();
    },
    choiceDeviceVal() {
      let that=this
      // console.log("设备详情在线调试",that.deviceInfos);
      
      if(this.deviceInfos.DeviceId){
        this.$refs["mq-console"].connect(that.deviceInfos.DeviceId);
        this.isStartconnect=true
      }
      
    },
    carryAction() {
      if (this.$refs["implementParamsForm"]) {
        this.$refs["implementParamsForm"].validate((valid) => {
          console.log(valid);
          if (valid) {
            this.matchesLoading = true;
            let obj = {};
            this.implementParamsForm.inputsData.forEach((its) => {
              obj[its.code] = its.codeVal;
            });
            console.info(obj);
            implementFunc({
              deviceId: this.debugFrom.debugDevice,
              functionId: this.implementParamsForm.functionId,
              inputs: JSON.parse(JSON.stringify(obj)),
            })
              .then((res) => {
                if (res.code == 0) {
                  this.implementParams = false;
                  this.matchesLoading = false;
                }
              })
              .catch(() => {
                this.implementParams = false;
                this.matchesLoading = false;
              });
          }
        });
      }
    },
    closeMatchesDrawer() {
      this.implementParams = false;
    },
    changeDebugType() {
      //切换调试方式
      let modelTSL = JSON.parse(this.productInfos.ModelTSL);
      if (this.debugFrom.debugType == "属性调试") {
        this.attrData = modelTSL.properties;
      } else if (this.debugFrom.debugType == "功能调试") {
        this.attrData = modelTSL.functions;
      } else if (this.debugFrom.debugType == "Modbus调试") {
        if (modelTSL.modbus) {
          this.attrData = modelTSL.modbus.Matches;
        }
      } else if (this.debugFrom.debugType == "事件消息") {
        this.attrData = modelTSL.events;
      }
    },
    onSendTxt() {
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      implementText({
        DeviceId: this.debugFrom.debugDevice,
        Text: this.DebugText,
        IsHex: this.isHex,
      }).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    onSendBind() {
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      implementMsg({
        DeviceId: this.debugFrom.debugDevice,
        MsgType: "Bind",
      }).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    onSendICCID() {
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      implementMsg({
        DeviceId: this.debugFrom.debugDevice,
        MsgType: "QueryICCID",
      }).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    onSendConnect() {
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      implementMsg({
        DeviceId: this.debugFrom.debugDevice,
        MsgType: "Connect",
      }).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    onSendDisconnect() {
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      implementMsg({
        DeviceId: this.debugFrom.debugDevice,
        MsgType: "Disconnect",
      }).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    onDelHistory() {
      this.$set(this, "delform", {});
      this.$set(this, "deldateRange", []);
      this.deldlgvis = true;
    },
    submitDelForm() {
      this.delform["Id"] = this.debugFrom.debugDevice;
      if (this.deldateRange.length < 2) {
        this.$message({
          message: "请选择删除的日期范围",
          type: "error",
        });
        return;
      }
      // if(this.delform.code=='-1'){
      //   delete this.delform.code
      // }
      this.addDateRange(this.delform, this.deldateRange, [
        "BeginTime",
        "EndTime",
      ]);
      console.info(this.delform);
      // return
      implementDelHis(this.delform).then((rsp) => {
        this.$modal.msgSuccess("发送成功");
      });
    },
    implementRowData(row, indexRow) {
      //执行
      if (!this.debugFrom.debugDevice) {
        this.$message({
          message: "请选择调试的设备",
          type: "error",
        });
        return;
      }
      if (this.debugFrom.debugType == "Modbus调试") {
        implementModbus({
          deviceId: this.debugFrom.debugDevice,
          matchName: row.Name,
        }).then((res) => {
          this.$modal.msgSuccess("发送成功");
        });
      } else if (this.debugFrom.debugType == "属性调试") {
        let arr = [];
        arr.push(row.code);
        implementProperty({
          deviceId: this.debugFrom.debugDevice,
          properties: JSON.parse(JSON.stringify(arr)),
        }).then((res) => {
          this.$modal.msgSuccess("发送成功");
        });
      } else if (this.debugFrom.debugType == "功能调试") {
        this.implementParamsForm.functionId = row.code;
        for (let i = 0; i < row.inputs.length; i++) {
          if (row.inputs[i].type == "int" || row.inputs[i].type == "float") {
            row.inputs[i].codeVal = null;
          } else {
            row.inputs[i].codeVal = "";
          }
        }
        let lis = JSON.parse(JSON.stringify(row.inputs));
        this.$set(
          this.implementParamsForm,
          "inputsData",
          JSON.parse(JSON.stringify(lis))
        );

        this.resetForm("implementParamsForm");
        this.implementParams = true;
      } else if (this.debugFrom.debugType == "事件消息") {
        implementEvent({
          DeviceId: this.debugFrom.debugDevice,
          EventId: row.code,
          Inputs: {},
        }).then((res) => {
          this.$modal.msgSuccess("发送成功");
        });
      }
    },
    async getChannelInfo() {
      this.debugFrom.debugType = "文本调试";
      let res = await channelInfo({ code: this.productInfos.NetworkWay });
      if (res.code == 0) {
        this.CanModbus = res.data.CanModbus;
      }
      if (this.dataLoading) {
        this.dataLoading = false;
      }
    },
  },
};
</script>
  <style lang="less" scoped>
.debug_con {
  display: flex;
  justify-content: space-between;
}

.debugLeft {
  width: 35%;

  .attrList {
    width: 100%;

    .funcbtn {
      display: flex;
      justify-content: space-between;
      margin-bottom: 25px;
    }
  }
}

.debugRecord {
  width: 65%;
  padding: 0 10px 0 40px;
  box-sizing: border-box;

  .recordtitle {
    display: flex;
    justify-content: space-between;
    margin-bottom: 15px;
    align-items: center;
  }

  .record_list {
    width: 100%;
    box-sizing: border-box;
    padding: 15px 20px 20px;
    overflow: auto;
    background: #f7f8fa;
  }
}

.txtdghd {
  color: #666;
  font-size: 14px;
  margin-bottom: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>