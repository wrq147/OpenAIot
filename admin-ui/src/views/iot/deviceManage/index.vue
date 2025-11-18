<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 136px)' }">
      <div class="from_con" id="from_con" v-show="showSearch" style="margin-bottom:0;padding-bottom:0">
        <el-form :model="deviceForm" ref="deviceForm" :inline="true" class="biaodan">
          <el-form-item label="关键字" prop="Key">
            <el-input v-model="deviceForm.Key" style="width:176px;" placeholder="请输入关键字" />
          </el-form-item>
          <el-form-item label="联网状态" prop="status">
            <el-select v-model="deviceForm.Online" placeholder="联网状态" clearable>
              <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="运行状态" prop="DState" v-if="isShowDStateSerch">
            <el-select v-model="deviceForm.DState" clearable placeholder="运行状态" @clear="dstateDataClear">
              <el-option v-for="item in DStatelist" :key="item.value" :label="item.label" :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="">
            <el-button class="label_filter_btn" @click="openLabelFilter">
              <i class="zhongtaiiconfont zhongtai-icon-shaixuan" style="font-size:14px;color:#333333"></i>
              <span style="margin-left:5px;color:#333333;">标签筛选</span>
              <div class="filter_num">{{ TagConditions.length }}</div>
            </el-button>
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="searchDeviceList">搜索</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div style="padding:20px;">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="info" @click="handleImport" plain v-hasPermi="['/IoTService/IotDevice/ListPage']">
                <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                <span style="margin-left:6px">导入设备</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="primary" plain v-hasPermi="['/IoTService/IotDevice/ListPage']" @click="openAddDevice">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">添加设备</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="primary" plain v-hasPermi="['/IoTService/IotDevice/ListPage']"
                @click="openAddForwardDevice">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">添加转发设备</span>
              </el-button>
            </el-col>
          </div>
          <div style="display:flex; align-items:center">
            <right-toolbar :showSearch.sync="showSearch" :isShowSearch="true" @queryTable="refreshData"></right-toolbar>
            <el-tooltip class="item" effect="dark" :content="isShape ? '模块展示' : '列表展示'" placement="top">
              <div class="tooltipIcon" @click="isShape = !isShape">
                <i v-if="!isShape" class="el-icon-s-fold"></i>
                <i v-if="isShape" class="el-icon-s-grid"></i>
              </div>
            </el-tooltip>
          </div>
        </el-row>
        <div v-if="!isShape" class="device_list" v-loading="configLoading">
          <div class="device_li" v-for="its in tableData" :key="its.Id">
            <div class="device_lis_top">
              <el-image fit="cover" style="width:60px;height:60px;margin-right: 20px;"
                :src="its.PhotoUrl + '?wh=500x500'">
                <img slot="error" src="../../../assets/images/shebei.png" alt />
              </el-image>

              <div class="lis_top_cot">
                <div class="device_name">{{ its.Name }}</div>
                <div class="device_name deviceId">通讯编码：{{ its.DeviceId }}</div>
                <div class="device_name pro_name">协议：{{ its.ProductName }}</div>
                <div class="device_group_name" v-if="its.GroupName">类型：{{ its.GroupName }}</div>
              </div>
            </div>
            <div class="device_lis_bottom">
              <span>{{ returnDevState(its) }}</span>
              <span>{{ parseTime(its.PD, '{y}-{m}-{d}') }}</span>
            </div>
            <div class="card-mask">
              <button @click="toDeviceDetails(its)">
                <svg-icon icon-class="todetails"></svg-icon>
              </button>
              <div class="operationIcon">
                <el-row>
                  <button><i class="el-icon-more" style="margin-right:10px"></i></button>
                </el-row>
                <div class="operation">
                  <el-row>
                    <button style="margin-right:10px" @click="deleteRowData(its)">
                      <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    </button>
                  </el-row>
                  <el-row>
                    <button style="margin-right:10px" @click="editRowData(its)"><i class="el-icon-edit"></i></button>
                  </el-row>
                </div>
              </div>
            </div>
          </div>
        </div>
        <el-table v-loading="configLoading" class="data_table" :header-cell-style="cellSty" border :data="tableData"
          style="width:100%">
          <el-table-column label="通讯编码" align="center" prop="DeviceId" :show-overflow-tooltip="true" />
          <el-table-column label="设备名称" align="center" prop="Name" />
          <el-table-column label="协议名称" align="center" prop="ProductName" />
          <el-table-column label="设备类型" align="center" prop="GroupName" />
          <el-table-column label="运行状态" align="center" prop="DState" />
          <el-table-column label="联网状态" align="center">
            <template slot-scope="scope">
              {{ returnOnlineState(scope.row) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="180">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-edit" @click="editRowData(scope.row)"
                style="margin-right:5px">编辑</el-button>
              <el-button type="text" icon="el-icon-delete" @click="deleteRowData(scope.row)"
                style="margin-right:5px;margin-left:5px;">删除</el-button>
              <el-button type="text" icon="el-icon-view" @click="toDeviceDetails(scope.row)"
                style="margin-left:5px;">详情</el-button>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="deviceForm.pageNum" :limit.sync="deviceForm.pageSize"
          :pageSizes="pageSizes" @pagination="getDeviceList" />
      </div>
    </div>
    <deviceAddDialog ref="deviceAddDialog" @loadDeviceList="loadDeviceList"></deviceAddDialog>
    <deviceForwardAddDialog ref="deviceForwardAddDialog" @loadDeviceList="loadDeviceList"></deviceForwardAddDialog>
    <labelFilter ref="labelFilter" @finishLabelFilter="finishLabelFilter"></labelFilter>
    <!-- 设备导入对话框 -->
    <el-dialog :close-on-click-modal="false" :title="upload.title" :visible.sync="upload.open" width="400px"
      append-to-body>
      <el-upload ref="uploadref" :limit="1" accept=".xlsx, .xls" :headers="upload.headers"
        :action="upload.url + '?updateSupport=' + upload.updateSupport" :disabled="upload.isUploading"
        :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess" :auto-upload="false" drag>
        <i class="el-icon-upload"></i>
        <div class="el-upload__text">
          将文件拖到此处，或
          <em>点击上传</em>
        </div>
        <div class="el-upload__tip text-center" slot="tip">
          <div class="el-upload__tip" slot="tip">
            <el-checkbox v-model="upload.updateSupport" />是否更新已经存在的设备数据
          </div>
          <span style="line-height: 33px;">仅允许导入xls、xlsx格式文件。</span>
          <el-link type="primary" :underline="false" style="font-size:12px;vertical-align: baseline;"
            @click="onImportTemplate">下载模板</el-link>
        </div>
      </el-upload>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitFileForm">确 定</el-button>
        <el-button @click="upload.open = false">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  DeviceList,
  removeDevice,
  exportemplate
} from "@/api/rules/device";
import { getToken } from "@/utils/auth";

import deviceAddDialog from './deviceAddDialog.vue';
import deviceForwardAddDialog from './deviceForwardAddDialog.vue';
import labelFilter from './labelFilter.vue';
import { getConfigKey } from "@/api/system/config.js";
export default {
  mixins: [resizeTableCon],
  components: { deviceAddDialog, labelFilter, deviceForwardAddDialog },
  dicts: ["device_run"],
  data() {
    return {
      // 设备导入参数
      upload: {
        // 是否显示弹出层（设备导入）
        open: false,
        // 弹出层标题（设备导入）
        title: "",
        // 是否禁用上传
        isUploading: false,
        // 是否更新已经存在的用户数据
        updateSupport: false,
        // 设置上传的请求头部
        headers: { Authorization: getToken() },
        // 上传的地址
        url: process.env.VUE_APP_BASE_API + "IoTService/IotDevice/Import"
      },
      //设备
      isShape: true,
      pageSizes: [9, 18, 27, 36],
      total: 0,
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        GroupId: null,
        Online: null,
        Name: ""
      },

      deviceForm: {
        pageNum: 1,
        pageSize: 9,
        GroupId: null,
        Online: null,
        Name: "",
        DeviceId: '',
        DeviceNumber: '',
        type: null
      }, //设备查询form
      statusList: [
        { label: "离线", value: 0 },
        { label: "在线", value: 1 },
        { label: "未知", value: 2 }
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
      tableList: [],
      deviceParamsList: [
        { filed: "DeviceId", filedName: "通讯编码" },
        { filed: "Name", filedName: "设备名称" },
        { filed: "Online", filedName: "在线状态" },
      ],
      groupmap: new Map(),
      DStatelist: [],
      isShowDStateSerch: false,
      TagConditions: [],
    };
  },
  watch: {
  },
  async mounted() {
    this.configLoading = true;
    this.getDeviceList();
    let Configres = await getConfigKey("device.runstate");
    if (Configres.data && Configres.data !== 'false') {
      this.isShowDStateSerch = Configres.data
      if (this.dict.type && this.dict.type.device_run) {
        this.DStatelist = this.dict.type.device_run
      }
    } else {
      this.isShowDStateSerch = false
    }
    this.tableList = this.deviceParamsList;

    this.configLoading = false;
  },
  methods: {
    dstateDataClear(val) {
      this.deviceForm.DState = undefined
    },
    openLabelFilter() {
      //打开标签筛选
      this.$refs.labelFilter.openFilter(this.TagConditions)
    },
    finishLabelFilter(filterList) {
      //完成标签筛选
      if (filterList && filterList.length > 0) {
        this.deviceForm.TagConditions = JSON.parse(JSON.stringify(filterList))
        this.TagConditions = JSON.parse(JSON.stringify(filterList))
      } else {
        if (this.deviceForm.TagConditions != undefined) {
          delete this.deviceForm.TagConditions
        }
        this.TagConditions = []
      }
      this.loadDeviceList()
    },
    returnDevState(row) {
      if (row.Online == 0) {
        return row.DState + '-离线';
      } else if (row.Online == 1) {
        return row.DState + '-在线';
      } else if (row.Online == 2) {
        return row.DState + '-未知';
      }
    },
    returnOnlineState(row) {
      if (row.Online == 0) {
        return '离线'
      } else if (row.Online == 1) {
        return '在线'
      } else if (row.Online == 2) {
        return '未知'
      }
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    loadDeviceList() {
      this.deviceForm.pageNum = 1
      this.getDeviceList()
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    // 文件上传成功处理
    handleFileSuccess(response, file, fileList) {
      this.upload.open = false;
      this.upload.isUploading = false;
      this.$refs.uploadref.clearFiles();
      this.$alert(response.message, "导入结果", { dangerouslyUseHTMLString: true });
      this.searchDeviceList();
    },
    // 提交上传文件
    submitFileForm() {
      this.$refs.uploadref.submit();
    },
    /** 导入按钮操作 */
    handleImport() {
      this.upload.title = "设备导入";
      this.upload.open = true;
    },
    /** 下载模板操作 */
    onImportTemplate() {
      exportemplate();
    },

    async openAddDevice() {
      //打开添加设备
      this.$refs.deviceAddDialog.openAddDevice()
    },
    openAddForwardDevice() {
      //打开添加转发设备
      this.$refs.deviceForwardAddDialog.openAddDevice()
    },
    refreshData() {
      this.getDeviceList();
    },
    searchDeviceList() {
      this.deviceForm.pageNum = 1;
      this.getDeviceList();
    },
    getDeviceList() {
      //获取设备列表
      this.configLoading = true;
      DeviceList(this.deviceForm).then(async response => {
        // console.log("查询到的设备", response);
        this.deviceTableData = response.data.List;
        this.total = response.data.Total;
        this.tableList = this.deviceParamsList;
        this.tableData = this.deviceTableData;
        this.configLoading = false;
      });
    },
    toDeviceDetails(row) {
      this.$router.push({ path: "/iot/deviceManage/deviceDetail", query: { id: row.Id, } });
    },
    async editRowData(row) {
      this.activeGroupId = row.Id;
      this.$refs.deviceAddDialog.editRowData(row)
    },
    deleteRowData(row) {
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

.groupFrom {
  .el-form-item {
    width: 100%;

    .el-form-item__content {
      width: calc(100% - 80px);

      .el-select {
        width: 100%;
      }

      .el-input {
        width: 100%;
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
::v-deep.label_filter_btn.el-button {
  position: relative;

  .filter_num {
    position: absolute;
    padding: 4px 6px;
    font-size: 12px;
    color: rgba(53, 114, 255, 1);
    border-radius: 50%;
    right: -10px;
    top: -10px;
    background: rgba(228, 237, 255, 1);
  }
}

::v-deep .vue-treeselect__menu {
  overflow: auto;
  width: 160px;
}

::v-deep .vue-treeselect__label {
  overflow: unset;
  text-overflow: unset;
}

::v-deep .vue-treeselect div,
.vue-treeselect span {
  box-sizing: content-box;
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
        .device_name {
          margin-bottom: 5px;
          color: #000000;
          font-weight: 600;

          &.pro_name {
            font-weight: normal;
            font-size: 14px;
          }

          &.deviceId {
            font-weight: normal;
            font-size: 14px;
          }
        }

        .device_group_name {
          color: #887e7b;
          font-size: 14px;
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

.tooltipIcon {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 1px solid #DCDFE6;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  margin-left: 10px;
}

.tooltipIcon>i {
  color: #606266;
  width: 16px;
  height: 16px;
}

.tooltipIcon:hover>i {
  color: #6BC7FF;
}
</style>