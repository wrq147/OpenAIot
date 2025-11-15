<template>
  <div>
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }"
      v-if="activeSelect == 'deviceManagement'">
      <div>
        <div>
          <div class="from_con" id="from_con" style="margin-bottom:0;padding-bottom:0" v-show="showSearch">
            <el-form :model="deviceForm" ref="deviceForm" :inline="true" class="biaodan">
              <el-form-item label="设备分组" prop="GroupId">
                <treeselect class="set_radius groupSet" v-model="deviceForm.GroupId" :options="groupTreeList"
                  :show-count="true" :normalizer="normalizer" placeholder="请选择设备分组" />
              </el-form-item>
              <el-form-item label="关键字" prop="Key">
                <el-input v-model="deviceForm.Key" placeholder="请输入关键字" />
              </el-form-item>
              <el-form-item label="联网状态" prop="status">
                <el-select v-model="deviceForm.Online" placeholder="请输入联网状态" clearable>
                  <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value" />
                </el-select>
              </el-form-item>
              <el-form-item label="运行状态" prop="DState" v-if="isShowDStateSerch">
                <el-select v-model="deviceForm.DState" filterable reserve-keyword allow-create
                  :clearable="true" placeholder="请输入运行状态">
                    <el-option v-for="item in DStatelist" :key="item" :label="item" :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="" prop="DState">
                <el-button icon="el-icon-refresh">标签筛选</el-button>
              </el-form-item>
              <!-- <el-col class="float_right" :span="24"> -->
              <el-form-item class="submit_button_con">
                <!-- <el-button icon="el-icon-refresh">重置</el-button> -->
                <el-button type="primary" icon="el-icon-search" @click="getDeviceList">搜索</el-button>
              </el-form-item>
              <!-- </el-col> -->
            </el-form>
          </div>
          <div style="padding:20px;">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="info" plain v-hasPermi="['/IoTService/IotDevice/ListPage']">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i><span style="margin-left:6px">导入设备</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="primary" plain v-hasPermi="['/IoTService/IotDevice/ListPage']" @click="openAddDevice">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i><span style="margin-left:6px">添加设备</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" :isShowSearch="true" @queryTable="refreshData"></right-toolbar>
            </el-row>
            <div class="device_list" v-loading="configLoading">
              <div class="device_li" v-for="its in tableData" :key="its.Id">
                <div class="device_lis_top">
                  <img src="../../../assets/images/shebei.png" alt />
                  <div class="lis_top_cot">
                    <div class="device_name">{{ its.Name }}</div>
                    <div class="device_name deviceId">通讯编码：{{ its.DeviceId }}</div>
                    <div class="device_name pro_name">产品：{{ its.ProductName }}</div>
                    <div class="device_group_name">{{ its.GroupName }}</div>
                  </div>
                </div>
                <div class="device_lis_bottom">
                  <span>{{ returnDevState(its) }}</span>
                  <span>{{ parseTime(its.PD, '{y}-{m}-{d}') }}</span>
                </div>
                <div class="card-mask">
                  <button @click="toDeviceDetails(its)"><svg-icon icon-class="todetails"></svg-icon></button>
                  <div class="operationIcon">
                    <el-row><button><i class="el-icon-more" style="margin-right:10px"></i></button></el-row>
                    <div class="operation">
                      <el-row>
                        <button style="margin-right:10px" @click="deleteRowData(its)"><i class="zhongtaiiconfont zhongtai-icon-shanchu"></i></button>
                      </el-row>
                      <el-row>
                        <button style="margin-right:10px" @click="editRowData(its)"><i class="el-icon-edit"></i></button>
                      </el-row>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <pagination v-show="total > 0" :total="total" :page.sync="deviceForm.pageNum"
              :limit.sync="deviceForm.pageSize" :pageSizes="pageSizes" @pagination="getDeviceList" />
          </div>
        </div>
      </div>
    </div>

    <deviceAddDialog ref="deviceAddDialog" @loadDeviceList="loadDeviceList" :groupTreeList="groupTreeList" :isProductDev="true"></deviceAddDialog>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  groupTree,
  DeviceList,
  removeDevice,
} from "@/api/rules/device";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import deviceAddDialog from './deviceAddDialog.vue';
export default {
  mixins: [resizeTableCon],
  props: ["activeSelect", "productInfos"],
  components: { Treeselect,deviceAddDialog },
  data() {
    const fileMustUpload = (rule, value, callback) => {
      if (this.deviceAddFrom.PhotoUrl == null||this.deviceAddFrom.PhotoUrl == '') {
        // 未上传文件
        callback("请上传设备图片");
      }
      callback();
    };
    return {
      //设备
      saveDeviceLoading: false,
      pageSizes: [9, 18, 27, 36],
      groupTreeList: [],
      total: 0,
      queryParams: {
        pageNum: 1,
        pageSize: 9,
        GroupId: null,
        Online: null,
        Name: ""
      },
      deviceAddOpen: false, //添加设备弹窗
      deviceAddFrom: {
        productId: 0,
        productName: "",
        groupId: null,
        name: "",
        deviceId: "",
        PhotoUrl:'',
        Price:0,
        pd: "",
        Remark:""
      },
      deviceAddRules: {
        productId: [
          { required: true, trigger: "change", message: "请选择产品" }
        ],
        name: [{ required: true, trigger: "blur", message: "请输入设备名称" }],
        Price: [
          { required: true, message: "原价不能为空", trigger: "change" }
        ],
        PhotoUrl:[{ validator: fileMustUpload, trigger: "change" }]
        
      },
      activeDevice: "deviceManage",
      deviceForm: {
        ProductId: 0,
        pageNum: 1,
        pageSize: 9,
        GroupId: null,
        Online: null,
        Name: "",
        DeviceId:'',
        type: null
      }, //设备查询form
      statusList: [
        { label: "离线", value: 0 },
        { label: "在线", value: 1 }
      ],
      tableData: [],
      deviceTableData: [],
      groupTableData: [],
      // 重新渲染表格状态
      refreshTable: true,
      // 是否展开，默认全部折叠
      isExpandAll: false,
      configLoading: true, //配置信息是否处于
      // 显示搜索条件
      showSearch: true,
      // 列信息
      columns: [
        { key: 0, label: `设备名称`, visible: true },
        { key: 1, label: `所属分组`, visible: true },
        { key: 2, label: `设备编码`, visible: true },
        { key: 3, label: `设备状态`, visible: true }
      ],
      deviceAddDialog:'添加设备',
      DStatelist:[],
      isShowDStateSerch:false
      //设备
    };
  },
  watch:{
    productInfos:{
      handler(to){
        let ModelTSL=JSON.parse(to.ModelTSL)
        let tags=ModelTSL.tags
        let state=tags.find(row=>row.code == "state")
        if(state){
          this.isShowDStateSerch=true
          if(state.option&&state.option.elements){
            this.DStatelist=Object.values(state.option.elements)
          }
        }else{
          this.isShowDStateSerch=false
        }
      },
      immediate:true,
      deep:true
    }
  },
  async mounted() {
    this.configLoading = true;
    this.getGroupList();
    this.getDeviceList();
    // console.log("产品名称", this.productInfos);
    this.configLoading = false;
  },
  methods: {
    returnDevState(row){
      //展示设备状态
      if(row.Online == 0){
        return '离线'
      }else if(row.Online == 1){
        if(row.DState){
          return row.DState
        }else{
          return '在线'
        }
      }else if(row.Online == 2){
        if(row.DState){
          return row.DState
        }else{
          return '未知'
        }
      }
    },
    toDeviceDetails(row) {
      this.$router.push({ path: "/iot/deviceManage/deviceDetail", query: { id: row.Id, } });
    },
    loadDeviceList(){
      // this.deviceForm.pageNum=1
      this.getDeviceList(this.deviceForm);
    },
    async openAddDevice() {
      //打开添加设备
      await this.$refs.deviceAddDialog.openAddDevice(this.productInfos)
    },
    to(path) {
      this.activeSelect = path;
    },

    refreshData() {
      this.getDeviceList();
      this.getGroupList();
    },
    getDeviceList() {
      //获取设备列表
      this.configLoading = true;
      this.deviceForm.ProductId = this.productInfos.Id;
      DeviceList(this.deviceForm).then(async response => {
        // console.log("查询到的设备", response);
        this.deviceTableData = response.data.List;
        this.total = response.data.Total;
        this.tableData = this.deviceTableData;
        this.configLoading = false;
      });
    },
    getGroupList() {
      //获取设备分组列表
      this.configLoading = true;
      groupTree().then(res => {
        if (res.code == 0) {
          this.groupTableData = res.data;
          let lists = [];
          lists = JSON.parse(JSON.stringify(res.data));
          this.groupTreeList = lists;

          this.configLoading = false;
        }
      });
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.GroupName,
        children: node.Children
      };
    },
    editRowData(row) {
      //修改一行的数据
      this.$refs.deviceAddDialog.editRowData(row)
    },
    deleteRowData(row) {
      //删除表格中的一行的数据
      this.$modal
        .confirm('是否确认移除名为"' + row.Name + '"的设备？')
        .then(function () {
          return removeDevice({ id: row.Id });
        })
        .then(() => {
          this.getDeviceList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch(() => { });
    }
  }
};
</script>
<style lang="less">
.el-dialog__wrapper.deviceAddDialog {

  // --contentheight: 100%;
  .el-dialog__body {
    padding: 15px 30px 30px;
    max-height: calc(100vh - 200px);
    overflow-y: scroll;

    &::-webkit-scrollbar {
      width: 0 !important;
    }

    // 隐藏垂直方向的滚动条

    .deviceAddfrom {
      h2 {
        font-weight: 700;
        color: #4e514e;
      }

      .el-row {
        .el-col {
          .el-form-item {
            width: 100%;

            .el-select {
              width: 100%;
            }

            .el-input {
              width: 100%;
            }

            .el-form-item__content {
              width: 100%;
            }

            .el-form-item__label {
              text-align: left;
              font-weight: 700;
              color: #4e514e;
            }
          }
        }

        // .el-col:nth-child(2n) {
        //   text-align: right;
        // }
        .el-form-item.configInfo {
          width: 100%;

          .el-form-item__content {
            width: 100%;
          }
        }
      }

      .el-table th.el-table__cell.is-leaf,
      .el-table td.el-table__cell {
        border-bottom: none;
      }
    }
  }
}

.groupSet {
  .vue-treeselect__input-container {
    display: flex;
    align-items: center;
  }
}
</style>
<style lang="less" scoped>
::v-deep .vue-treeselect__menu{
  overflow: auto;
   width: calc(100% + 10px);
}
::v-deep .vue-treeselect__label{
  overflow: unset;
  text-overflow: unset;
}
::v-deep .vue-treeselect div, .vue-treeselect span{
  box-sizing:content-box;
}
.device_list {
  margin-right: -3%;
  margin-top: 15px;
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;

  .device_li {
    width: 30%;
    box-shadow: 0 0 6px 6px rgba(244, 245, 249, 1);
    margin: 0 3% 25px 0;
    border-radius: 5px;
    padding: 20px 20px 10px;
    position: relative;

    .device_lis_top {
      width: 100%;
      display: flex;
      justify-content: flex-start;

      img {
        width: 60px;
        height: 60px;
        margin-right: 20px;
      }

      .lis_top_cot {
        font-size: 14px;

        .device_name {
          margin-bottom: 5px;
          color: #000000;
          font-weight: 600;
          &.pro_name{
            font-weight: normal;
            font-size: 14px;
          }
          &.deviceId{
            font-weight: normal;
            font-size: 14px;
          }
        }

        .device_group_name {
          color: #887e7b;
        }
      }
    }

    .device_lis_bottom {
      display: flex;
      justify-content: space-between;
      margin-top: 20px;
      color: #887e7b;
      font-size: 12px;
    }

    .card-mask {
      position: absolute;
      top: 0;
      left: 0;
      z-index: 2;
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      height: 100%;
      color: #fff;
      background-color: transparent;
      visibility: hidden;
      cursor: pointer;
      border-radius: 5px;

      button {
        .svg-icon {
          color: #ffffff;
        }

        .svg-icon:hover {
          color: #5e99f6;
        }
      }

      .operationIcon {
        position: absolute;
        top: 0;
        right: 0;

        .operation {
          display: none;
        }

        i {
          font-size: 16px;
        }

        button {
          .svg-icon {
            font-size: 16px;
          }

          i {
            font-size: 16px;
            color: #ffffff;
          }

          i:hover {
            color: #5e99f6;
          }
        }
      }

      .operationIcon:hover {
        .operation {
          display: block;
        }
      }
    }
  }

  .device_li:hover {
    .card-mask {
      background-color: rgba(0, 0, 0, 0.5);
      visibility: visible;

      button {
        border: none;
        cursor: pointer;
        font-size: 24px;
        background: rgba(0, 0, 0, 0);
      }
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
}
</style>