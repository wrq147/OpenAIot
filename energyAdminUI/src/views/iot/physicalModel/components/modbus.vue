<template>
  <div style="width: 100%">
    <el-form
      label-width="120px"
      :rules="modbusRules"
      ref="modbussForm"
      :model="modbussForm"
      v-if="activeDefinition == 'modbus'"
      style="width: 100%"
    >
      <el-row>
        <el-col :span="12" style="padding-right: 30px; box-sizing: border-box">
          <el-row>
            <div style="display: flex; align-items: center">
              <h2>Modbus配置：</h2>
              <el-col :span="1.5">
                <el-button
                  type="primary"
                  plain
                  @click="exportRow(modbussForm, true)"
                >
                  <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                  <span style="margin-left: 6px">导出</span>
                </el-button>
              </el-col>
              <el-col :span="1.5">
                <el-button type="primary" plain class="putbutton">
                  <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                  <span style="margin-left: 6px"
                    >导入<input
                      type="file"
                      @change="importRow"
                      id="putbuttonFile"
                  /></span>
                </el-button>
              </el-col>
            </div>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label="波特率" prop="BaudRate">
                <el-select filterable v-model="modbussForm.BaudRate" placeholder="请选择波特率" style="width: 100%">
                  <el-option
                    v-for="ite in BaudRateList"
                    :key="ite.value"
                    :label="ite.label"
                    :value="ite.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="数据位" prop="DataBits">
                <el-select
                  v-model="modbussForm.DataBits"
                  placeholder="请选择数据位"
                  style="width: 100%"
                >
                  <el-option
                    v-for="ite in DataBitsList"
                    :key="ite.value"
                    :label="ite.label"
                    :value="ite.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label="奇偶效验" prop="Parity">
                <el-select
                  v-model="modbussForm.Parity"
                  placeholder="请选择奇偶效验"
                  style="width: 100%"
                >
                  <el-option
                    v-for="ite in ParityList"
                    :key="ite.value"
                    :label="ite.label"
                    :value="ite.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="停止位" prop="StopBits">
                <el-select
                  v-model="modbussForm.StopBits"
                  placeholder="请选择停止位"
                  style="width: 100%"
                >
                  <el-option
                    v-for="ite in StopBitsList"
                    :key="ite.value"
                    :label="ite.label"
                    :value="ite.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-form-item label="轮询周期时间" prop="PollTime">
            <el-row>
              <el-col :span="12">
                <el-input
                  style="margin-right: 10px"
                  type="number"
                  :step="1000"
                  v-model.number="modbussForm.PollTime"
                  placeholder="请输入轮询周期时间"
                ></el-input>
              </el-col>
              <el-col :span="12">
                <div
                  style="
                    padding: 0 10px;
                    background-color: #f5f7fa;
                    margin-left: 10px;
                  "
                >
                  <i
                    class="zhongtaiiconfont zhongtai-icon-zhuyi"
                    style="
                      font-size: 14px;
                      margin-right: 5px;
                      color: rgb(232, 72, 59);
                    "
                  ></i>
                  <span style="color: #6ab2fa">轮询周期时间（单位ms）</span>
                </div>
              </el-col>
            </el-row>
          </el-form-item>
          <el-form-item label="Mode" prop="Mode">
            <el-select
              v-model="modbussForm.Mode"
              placeholder="请选择Mode"
              style="width: 100%"
            >
              <el-option
                v-for="ite in ModeList"
                :key="ite.value"
                :label="ite.label"
                :value="ite.value"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-row style="text-align: right; margin-top: 30px">
            <el-button
              type="primary"
              @click="saveProductInfo"
              :loading="saveLoading"
              >{{ saveLoading ? "提交中 ..." : "保 存" }}</el-button
            >
          </el-row>
        </el-col>
        <el-col
          :span="12"
          style="
            padding-left: 10px;
            padding-right: 10px;
            box-sizing: border-box;
            background-color: #f0f2f5;
            min-height: calc(100vh - 272px);
          "
        >
          <h2>匹配规则：</h2>
          <el-form-item label="规则匹配">
            <div
              v-if="modbussForm.Matches && modbussForm.Matches.length > 0"
              class="param_list_con"
            >
              <div
                class="param_list"
                v-for="(item, inx) in modbussForm.Matches"
                :key="inx"
              >
                <div class="list_left">
                  <span>{{ item.Name }}</span>
                  <span class="params_type">{{
                    funCodeListMap.get(item.FuncCode)
                  }}</span>
                </div>
                <div class="list_right">
                  <i
                    class="el-icon-edit"
                    @click="editOthersParams(item, inx)"
                  ></i>
                  <i
                    class="el-icon-delete"
                    @click="deleteOthersParams(item, inx)"
                  ></i>
                </div>
              </div>
            </div>
            <span
              style="color: #0055ff; cursor: pointer"
              @click="openMatchesDrawer"
              >+增加规则</span
            >
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <modbus-rules
      :modbussForm="modbussForm"
      :activeParamsLine="activeParamsLine"
      :matchesDrawer="matchesDrawer"
      :attrTableData="attrTableData"
      :matchesForm="matchesForm"
      :matchesItemsForm="matchesItemsForm"
      @closeMatchesDrawer="closeMatchesDrawer"
      @saveSetDataFun="saveSetDataFun"
      @addAttributeValue="addAttributeValue"
    ></modbus-rules>
  </div>
</template>

<script>
import ModbusRules from "./modbusRules"; //modbus添加规则
export default {
  name: "AdminUiModbus",
  components: {
    ModbusRules,
  },
  props: {
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    modbusData: {
      type: Object,
      default: () => {
        return {};
      },
    },
    attrTableData: {
      type: Array,
      default: () => {
        return [];
      },
    },
    activeDefinition: {
      //接入方式
      type: String,
      default: "",
    },
  },
  data() {
    return {
      matchesForm: {
        Name: "", //规则名称
        SlaveId: 1, //从机地址
        FuncCode: "01", //功能码
        StartAddress: 0, //起始地址
        Items: [],
      },
      matchesDrawer: false, //控制匹配规则的弹窗
      funCodeListMap: new Map(), //功能码代码和名称组合
      saveLoading: false, //是否在保存数据
      matchesItemsForm: [
        {
          ByteOrder: "H", //字节序
          NumRegister: "2", //数据长度
          PropertyCode: "", //对应的属性标识符
        },
      ],
      modbusRules: {
        BaudRate: [
          { required: true, trigger: "change", message: "请选则波特率" },
        ],
        DataBits: [
          { required: true, trigger: "change", message: "请选则数据位" },
        ],
        Parity: [
          { required: true, trigger: "change", message: "请选则奇偶效验" },
        ],
        StopBits: [
          { required: true, trigger: "change", message: "请选则停止位" },
        ],
        Mode: [{ required: true, trigger: "change", message: "请选则Mode" }],
        PollTime: [
          { required: true, trigger: "blur", message: "请输入轮询周期时间" },
        ],
      },
      modbussForm: {
        //功能定义
        BaudRate: 9600, //波特率
        DataBits: 8, //数据位
        Parity: "0", //奇偶效验
        StopBits: "1", //停止位
        Mode: "RTU", //Mode
        PollTime: 5000,
        Matches: [], //规则匹配
      },
      BaudRateList: [
        { label: 1200, value: 1200 },
        { label: 2400, value: 2400 },
        { label: 4800, value: 4800 },
        { label: 9600, value: 9600 },
        { label: 19200, value: 19200 },
        { label: 38400, value: 38400 },
        { label: 57600, value: 57600 },
        { label: 115200, value: 115200 },
        { label: 128000, value: 128000 },
        { label: 921600, value: 921600 },
      ], //波特率选项列表
      DataBitsList: [
        { label: "7位", value: 7 },
        { label: "8位", value: 8 },
      ], //数据位选项列表
      ParityList: [
        { label: "无检验", value: "0" },
        { label: "奇校验", value: "1" },
        { label: "偶校验", value: "2" },
      ], //奇偶效验选项列表
      StopBitsList: [
        { label: "1 位", value: "1" },
        { label: "2 位", value: "2" },
      ], //停止位列表
      ModeList: [
        { label: "RTU", value: "RTU" },
        { label: "ASCII", value: "ASCII" },
        { label: "TCP", value: "TCP" },
      ],
      //功能码列表
      FuncCodeList: [
        { label: "01读线圈状态", value: "01" },
        { label: "02读离散输入状态", value: "02" },
        { label: "03读保持寄存器", value: "03" },
        { label: "04读输入寄存器", value: "04" },
      ],
      activeParamsLine: -1, //当前修改的规则是哪一行
    };
  },

  mounted() {
    this.FuncCodeList.forEach((item) => {
      this.funCodeListMap.set(item.value, item.label);
    });

    if (this.modbusData && this.modbusData.BaudRate) {
      this.modbussForm = this.modbusData;
    } else {
      this.modbussForm = {
        //功能定义
        BaudRate: 9600, //波特率
        DataBits: 8, //数据位
        Parity: "0", //奇偶效验
        StopBits: "1", //停止位
        Mode: "RTU", //Mode
        PollTime: 5000,
        Matches: [], //规则匹配
      };
    }
  },

  methods: {
    addAttributeValue(){
      this.$emit('addAttributeValue')
    },
    processReadFile(file) {
      //读取导入参数的值
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          let jsonArr = JSON.parse(e.target.result);
          if(jsonArr.t&&jsonArr.t=='modbus'){
            this.modbussForm = jsonArr.items;
          }
        } catch (error) {
          console.error("Error parsing JSON", error);

          // this.$store.commit("rulesloadForm", this.setup);
        }
      };
      reader.readAsText(file);
    },
    importRow() {
      //导入功能
      const file = event.target.files[0];
      if (!file) {
        return;
      }
      this.processReadFile(file);
    },
    exportRow(row, isAll) {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let msgitem = {
        t: this.activeDefinition,
        items: [],
      };
      if (isAll) {
        msgitem.items = JSON.parse(JSON.stringify(row));
      } else {
        msgitem.items.push(row);
      }
      let tmname = this.activeDefinition;
      tmploading.close();
      const content = JSON.stringify(msgitem);
      const blobData = new Blob([content], { type: "application/json" });
      const filename = `${tmname}.json`; //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename);
      } else {
        const anchor = document.createElement("a");
        anchor.href = window.URL.createObjectURL(blobData);
        anchor.download = filename;
        anchor.click();
        window.URL.revokeObjectURL(blobData);
      }
    },
    deleteOthersParams(row, rowIndex) {
      //删除参数
      this.$modal
        .confirm('是否确认删除字段名称为"' + row.Name + '"的数据项？')
        .then((rs) => {
          if (rs == "confirm") {
            this.modbussForm.Matches.splice(rowIndex, 1);
            this.$emit("saveSetData", "del", this.modbussForm);
          }
        });
    },
    editOthersParams(row, rowIndex) {
      //编辑添加modbus的规则匹配参数
      this.openMatchesDrawer();
      this.matchesForm = row;
      this.matchesItemsForm = row.Items;
      this.activeParamsLine = rowIndex;
    },
    openMatchesDrawer() {
      this.matchesDrawer = true;
      this.activeParamsLine = -1;
      this.matchesForm = {
        Name: "", //规则名称
        SlaveId: 1, //从机地址
        FuncCode: "01", //功能码
        StartAddress: 0, //起始地址
        Items: [],
      };
      this.matchesItemsForm = [
        {
          ByteOrder: "H", //字节序
          NumRegister: "2", //数据长度
          PropertyCode: "", //对应的属性标识符
          xuhaoNum: 0,
        },
      ];
    },
    closeMatchesDrawer() {
      //关闭匹配规则填写弹窗
      this.matchesDrawer = false;
    },
    saveProductInfo() {
      //保存产品信息
      if (this.activeDefinition == "modbus") {
        if (this.$refs["modbussForm"]) {
          this.$refs["modbussForm"].validate((valid1) => {
            if (valid1) {
              //   this.saveSetData();
              this.saveSetDataFun(this.modbussForm);
            }
          });
        }
      }
    },
    saveSetDataFun(modbussForm) {
      this.$emit("saveSetData", "add", modbussForm);
    },
  },
};
</script>

<style lang="less" scoped>
.putbutton {
  background: #ecf5ff;
  color: #409eff;
  position: relative;
  border: 1px solid #B3D8FF;
  margin-left: 10px;
  i {
    margin-right: 5px;
  }
  #putbuttonFile {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    opacity: 0;
    filter: alpha(opacity=0);
  }
  &::before{
    color: #409eff;
  }
}
.param_list_con {
  background-color: #fafafa;
}
.param_list {
  color: #272e3b;
  width: 100%;
  box-sizing: border-box;
  padding: 12px;
  height: 36px;

  display: flex;
  justify-content: space-between;
  align-items: center;
  .list_left {
    .params_type {
      padding: 0 6px;
      border: 1px solid #595959;
      border-radius: 3px;
      margin-left: 10px;
    }
  }
  .list_right {
    i {
      padding: 0 5px;
      cursor: pointer;
    }
  }
}
</style>