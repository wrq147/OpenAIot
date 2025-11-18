<template>
  <div>
    <!-- <el-row :gutter="10" class="mb8 button_row" style="display:flex;justify-content:flex-end;align-items:center">
      <div>
        <el-col :span="1.5">
          <el-button type="warning" plain :loading="exportLoading" @click="handleExport">
          <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
            <span style="margin-left:6px">查询与导出历史报表</span>
          </el-button>
        </el-col>
      </div>
    </el-row> -->
    <div class="info_title">
      <h3>设备信息</h3>
      <div class="edit_class" @click="onEditClick" v-if="!isInfoEdit && canChangeDevice">
        <i class="el-icon-edit"></i><span>编辑</span>
      </div>
      <div class="edit_class" v-if="isInfoEdit && canChangeDevice">
        <el-button type="text" @click="cancelSave()">
          <i class="el-icon-close"></i>取消
        </el-button>
        <el-button type="text" @click="saveDeviceInfo" :loading="saveLoading">
          <i class="el-icon-check"></i>
          {{ saveLoading ? "提交中 ..." : "保 存" }}
        </el-button>
      </div>
    </div>
    <el-form ref="deviceInfosForm" :model="deviceBasicInfos" :rules="infoRules">
      <table border="1" class="configInfo" v-loading="configisLoading">
        <tr>
          <th colspan="1"><span>批次编号</span></th>
          <td class colspan="3">
            <span>{{ deviceBasicInfos.DeviceNumber ? deviceBasicInfos.DeviceNumber : "" }}</span>
          </td>
          <th colspan="1"><span>归属企业</span></th>
          <td colspan="1" style="width: 21.5%">
            <div v-if="!isInfoEdit">{{ deviceBasicInfos.OwnerOrgName || "" }}</div>
          </td>
        </tr>
        <tr>
          <th colspan="1"><span>通讯编码</span></th>
          <td colspan="1" style="width: 21.5%">
            <div v-if="isInfoEdit">
              <el-form-item style="margin-bottom: 0">
                <el-input v-model="deviceBasicInfos.DeviceId" placeholder="请输入设备ID"></el-input>
              </el-form-item>
            </div>
            <div v-else>{{ deviceBasicInfos.DeviceId ? deviceBasicInfos.DeviceId : "" }}</div>
          </td>
          <th colspan="1"><span>设备名称</span></th>
          <td colspan="1" style="width: 21.5%">
            <div v-if="!isInfoEdit">{{ deviceBasicInfos.Name ? deviceBasicInfos.Name : "" }}</div>
            <div v-if="isInfoEdit">
              <el-form-item style="margin-bottom: 0" prop="Name">
                <el-input v-model="deviceBasicInfos.Name" placeholder="请输入设备名称" maxlength="50" minlength="1"></el-input>
              </el-form-item>
            </div>
          </td>
          <th colspan="1"><span>运行状态</span></th>
          <td colspan="1" style="width: 21.5%">
            <span>{{ deviceBasicInfos.DState }}</span>
          </td>
        </tr>
        <tr>
          <th colspan="1"><span>设备说明</span></th>
          <td class colspan="5">
            <div v-if="!isInfoEdit">
              {{ deviceBasicInfos.Remark ? deviceBasicInfos.Remark : "" }}
            </div>
            <div v-if="isInfoEdit">
              <el-form-item style="margin-bottom: 0" prop="Remark">
                <el-input style="width: 450px" type="textarea" :rows="2" v-model="deviceBasicInfos.Remark" placeholder="请输入备注信息" maxlength="500"></el-input>
              </el-form-item>
            </div>
          </td>
        </tr>
        <tr>
          <th colspan="1"><span>协议名称</span></th>
          <td colspan="1" style="width: 21.5%">
            <div v-if="!isInfoEdit&&!isCheckPermi(['/IoTService/IotProduct/ListPage'])" style="width: 90%">{{ productInfos.Name ? productInfos.Name : "" }}</div>
            <div v-if="!isInfoEdit&&isCheckPermi(['/IoTService/IotProduct/ListPage'])" style="width: 90%;cursor: pointer;" @click="jumpToProduct">{{ productInfos.Name ? productInfos.Name : "" }}</div>
            <div v-if="isInfoEdit">
              <el-form-item style="margin-bottom: 0" prop="ProductId">
                <el-select v-model="deviceBasicInfos.ProductId" placeholder="请选择" @change="changeDeviceProduct" style="width: 202px">
                  <el-option v-for="item in productLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
                </el-select>
              </el-form-item>
            </div>
          </td>
          <th colspan="1"><span>协议分类</span></th>
          <td colspan="1" style="width: 21.5%">
            <span>{{ productInfos.ClassifiedId ? (productClassMap.get(productInfos.ClassifiedId) ? productClassMap.get(productInfos.ClassifiedId).Name : "" ): "" }}</span>
          </td>
          <th colspan="1" v-if="this.canChangeDevice"><span>接入方式</span></th>
          <td colspan="1" style="width: 21.5%" v-if="this.canChangeDevice">
            <span>{{ productInfos.NetworkWay ? (channelmap.get(productInfos.NetworkWay) ? channelmap.get(productInfos.NetworkWay).Name : "" ): "" }}</span>
          </td>
        </tr>
        <tr>
          <th colspan="1"><span>固件版本</span></th>
          <td colspan="1" style="width: 21.5%">
            <span>{{ deviceBasicInfos.FirmwareVer}}</span>
          </td>
          <th colspan="1"><span>创建时间</span></th>
          <td colspan="1" style="width: 21.5%">
            <span>{{ deviceBasicInfos.CreateOn ? deviceBasicInfos.CreateOn : "" }}</span>
          </td>
          <th colspan="1"><span>离线时间</span></th>
          <td colspan="1" style="width: 21.5%">
            <span>{{ deviceBasicInfos.Online == 0 ? deviceBasicInfos.LastOnline : (deviceBasicInfos.Online == 1 ? "在线中" : "") }}</span>
          </td>
        </tr>
        <tr>
          <th colspan="1"><span>协议说明</span></th>
          <td class colspan="5"><span>{{ productInfos.Remark ? productInfos.Remark : "" }}</span></td>
        </tr>
      </table>
    </el-form>
    <exportHistory ref="exportHistoryForm" :productInfos="productInfos" :deviceInfos="deviceInfos"></exportHistory>
  </div>
</template>

<script>
import { editDevice, groupTree } from "@/api/rules/device";
import { productList, classTree, channelList } from "@/api/rules/productModel";
import exportHistory from "./exportHistory.vue"
import { checkPermi } from "@/utils/permission";
export default {
  name: "deviceBasic",
  components: { exportHistory },
  props: {
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    canChangeDevice: {
      type: Boolean,
      default: true,
    },
    configLoading: {
      type: Boolean,
      default: true,
    },
  },

  data() {
    return {
      exportLoading:false,//是否处于导出状态
      workWayList: [], //设备接入方式列表
      productClassMap: new Map(),
      productClassList: [],
      channelmap: new Map(),
      productLists: [],
      configisLoading: this.configLoading,
      deviceBasicInfos: this.deviceInfos,
      isInfoEdit: false,
      saveLoading: false,
      infoRules: {
        ProductId: [
          { required: true, trigger: "change", message: "请选择协议" },
        ],
        Name: [
          { required: true, trigger: "blur", message: "请输入设备名称" },
          {
            min: 1,
            max: 50,
            message: "长度在 1 到 50 个字符",
            trigger: "blur",
          },
        ],
      },
    };
  },

  mounted() {
    this.$nextTick(()=>{
      this.loadData();
    })
  },
  watch: {
    deviceInfos(to, from) {
    //   console.log("变化的", to);
      this.deviceBasicInfos = to;
    },
    canChangeDevice(to){
      // console.log('是否可以编辑',to);
    }
  },
  methods: {
    isCheckPermi(val) {
      return checkPermi(val)
    },
    jumpToProduct(){
      //跳转到设备详情
      this.$router.push({
        path: "/iot/physicalModel/productAdd/"+this.productInfos.Id,
        query: { classId: this.productInfos.ClassifiedId }
      });
    },
    handleExport(){
      //导出历史报表
      this.$refs.exportHistoryForm.openDialog()
    },
    changeDeviceProduct() {
      this.$emit("changeDeviceProduct", this.deviceBasicInfos.ProductId);
    },
    cancelSave() {
      this.isInfoEdit = false;
      this.$emit("cancelSave");
    },
    saveDeviceInfo() {
      //保存设备信息
      this.$refs["deviceInfosForm"].validate((valid) => {
        // console.log("valid", valid);
        if (valid) {
          editDevice({
            id: this.deviceBasicInfos.Id,
            deviceId: this.deviceBasicInfos.DeviceId,
            name: this.deviceBasicInfos.Name,
            groupId: this.deviceBasicInfos.GroupId,
            pd: this.deviceBasicInfos.PD,
            productId: this.deviceBasicInfos.ProductId,
            Remark: this.deviceBasicInfos.Remark,
          }).then((res) => {
            if (res.code == 0) {
              this.$modal.msgSuccess("修改成功");
              this.getDeviceInfos();
              this.isInfoEdit = false;
            }
          });
        }
      });
    },
    getDeviceInfos(){
        this.$emit('reloadDevice')
    },
    onEditClick() {
      //修改设备信息
      this.isInfoEdit = !this.isInfoEdit;
      if (this.isInfoEdit) {
        this.getproductList();
      }
    },
    getproductList() {
      productList({ showAll: true }).then((response) => {
        this.productLists = response.data.List;
      });
    },
    loadData() {
      // console.log("loadData",this.canChangeDevice);
      if (this.canChangeDevice) {
        this.getProductClassList(); //协议分类列表
        this.getchannelList(); //设备接入方式列表
      }else{
        this.configisLoading = false;
      }
    },
    getProductClassList() {
      //获取协议分类信息
      classTree().then((response) => {
        if (response.data.length > 0) {
          // this.classmap.set(0, response.data);
          // this.initClassList(response.data);
          this.productClassList = response.data;
          this.initClassList(response.data);
        }
      });
    },
    initClassList(node) {
      //设置协议分类列表为Map类型数据，方便根据id获取name
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        this.productClassMap.set(node[idx].Id, curnode);
        if (
          curnode.hasOwnProperty("Children") &&
          curnode.Children &&
          curnode.Children.length > 0
        ) {
          this.initClassList(curnode.Children);
        }
      }
    },
    getchannelList() {
      //获取设备接入方式
      channelList().then((rsp) => {
        // console.log("设备接入方式", rsp);
        this.workWayList = rsp.data;
        this.initChannelMap(rsp.data);
      });
    },
    initChannelMap(node) {
      //设置设备接入列表为Map类型数据，方便根据value获取name
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        // console.log("设备接入方式列表", curnode);
        this.channelmap.set(node[idx].Code, curnode);
      }
    },
  },
};
</script>
<style lang="less" scoped>
::v-deep .vue-treeselect__menu{
  overflow: auto;
  width: 100%;
}
::v-deep .vue-treeselect__label{
  overflow: unset;
  text-overflow: unset;
}
::v-deep .vue-treeselect div, .vue-treeselect span{
  box-sizing:content-box;
}
.edit_class {
  font-size: 16px;
  color: #bfbfbf;
  margin-left: 10px;
  cursor: pointer;

  span {
    margin-left: 5px;
  }

  button {
    font-size: 16px;
  }
}

.info_title {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

table.configInfo {
  border-spacing: 0px;
  border-collapse: collapse;
  width: 100%;
  border: none;
  // border-top: solid 1px #efefef;
  border-left: solid 1px #efefef;

  tr {
    height: 48px;
    line-height: 48px;
    color: rgba(0, 0, 0, 0.85);
    font-size: 14px;

    th {
      background-color: #fafafa;
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;
    }

    td {
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;

      div {
        display: flex;
        align-items: center;
        // justify-content: center;
      }
    }
  }
}
</style>
