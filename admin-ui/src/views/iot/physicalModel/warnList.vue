<template>
  <div :style="{padding:(!isComponent?'20px 20px 0 20px':'0')}" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--产品数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch" style="margin-bottom: 0px;">
            <el-form
              class="biaodan"
              :model="queryParams"
              ref="queryForm"
              :inline="true"
            >
              <!-- <el-form-item label="搜索关键词" prop="Name">
                <el-input
                  class="set_radius"
                  v-model="queryParams.Name"
                  placeholder="请输入产品名称"
                  clearable
                  @keyup.enter.native="handleQuery"
                />
              </el-form-item>-->
              <el-form-item label="过滤名称" prop="ProductId">
                <el-input
                  class="set_radius"
                  v-model="queryParams.Name"
                  placeholder="请输入告警名称或设备名称"
                  clearable
                  @keyup.enter.native="handleQuery"
                />
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker
                  class="set_radius"
                  v-model="dateRange"
                  style="width:232px"
                  value-format="yyyy-MM-dd"
                  type="datetimerange"
                  range-separator="-"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                ></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>

          <div
            class="elbiaoge_elform"
            :style="{'min-height':tableConHeight+'px','padding-top':(isComponent?'0':'20px')}">
            <el-row
              :gutter="10"
              class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="danger" plain @click="clearAllWarn" :disabled="warnTableList==0">
                    <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                    <span style="margin-left:6px">全部处理</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5" v-if="filProductId!=null">
                  <el-button type="primary" plain @click="goWarnConfig">
                    <i class="el-icon-setting"></i>
                    <span style="margin-left:6px">配置工单</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>
            <el-table
              v-loading="loading"
              border
              :data="warnTableList"
              :row-style="isRed"
              @selection-change="handleSelectionChange"
              class="data_table"
              :header-cell-style="cellSty"
              style="width:100%"
            >
              <el-table-column
                label="告警名称"
                align="center"
                key="Name"
                prop="Name"
                v-if="columns[0].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="事件标识"
                align="center"
                key="Code"
                prop="Code"
                v-if="columns[1].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="工单编号"
                align="center"
                key="WarnNumber"
                prop="WarnNumber"
                v-if="columns[2].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="告警描述"
                align="center"
                key="Description"
                prop="Description"
                width="150"
                v-if="columns[3].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="报警级别"
                align="center"
                key="Level"
                prop="Level"
                width="80"
                v-if="columns[4].visible"
                :show-overflow-tooltip="true"
                :filters="[{text: '普通', value: 0}, {text: '告警', value: 1}, {text: '紧急', value: 2}]"
                :filter-method="filterLevel"
              >
                <template slot-scope="scope">
                  <div>
                    <!-- {{scope.row.Level==0?'普通':(scope.row.Level==1?'告警':'紧急')}} -->
                    <el-tag type="info" v-if="scope.row.Level==0">普通</el-tag>
                    <el-tag type="warning" v-if="scope.row.Level==1">告警</el-tag>
                    <el-tag type="danger" v-if="scope.row.Level==2">紧急</el-tag>
                  </div>
                </template>
              </el-table-column>
              <el-table-column
                label="设备Id"
                align="center"
                key="DeviceId"
                prop="DeviceId"
                v-if="columns[5].visible"
                :show-overflow-tooltip="true"
                width="180"
              />
              <el-table-column
                label="设备名称"
                align="center"
                key="DeviceName"
                prop="DeviceName"
                v-if="columns[6].visible"
                width="180"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="消息内容"
                align="left"
                key="MsgInfo"
                prop="MsgInfo"
                v-if="columns[7].visible"
                width="150"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="状态"
                align="center"
                key="Status"
                prop="Status"
                v-if="columns[8].visible"
                width="80"
                :filters="[{text: '待处理', value: 0}, {text: '已处理', value: 1}]"
                :filter-method="filterSatus"
              >
                <template slot-scope="scope">
                  <el-tag type="success" v-if="scope.row.Status==1">已处理</el-tag>
                  <el-tag type="danger" v-if="scope.row.Status==0">待处理</el-tag>
                </template>
              </el-table-column>
              <el-table-column
                label="处理时间"
                align="center"
                key="clearOn"
                prop="clearOn"
                v-if="columns[9].visible"
                width="160"
              >
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.ClearOn) }}</span>
                </template>
              </el-table-column>
              <el-table-column
                label="处理人员"
                align="center"
                key="clearId"
                prop="clearId"
                v-if="columns[10].visible"
                :show-overflow-tooltip="true"
                width="100"
              >
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.UpdatedOn) }}</span>
                </template>
              </el-table-column>
              <el-table-column
                label="告警时间"
                align="center"
                key="CreateOn"
                prop="CreateOn"
                v-if="columns[11].visible"
                width="160"
              >
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.CreateOn) }}</span>
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                align="center"
                class-name="fixed-width"
                width="120"
                fixed="right"
              >
                <template slot-scope="scope">
                  <el-button
                    type="text"
                    icon="el-icon-edit"
                    @click="openClearDialog(scope.row)"
                  >处理报警</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination
              v-show="total > 0"
              :total="total"
              :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize"
              @pagination="getList"
            />
          </div>
        </el-col>
      </el-row>
      <el-dialog title="处理报警" :visible.sync="clearDialog" width="580px">
        <div>
          <!-- <el-form ref="clearForm" :model="clearForm">
            <el-form-item prop="isFilterFun" class="switch_arrange">
              <span slot="label">是否过滤事件标识</span>
              <el-switch v-model="clearForm.isFilterFun"></el-switch>
            </el-form-item>
            <el-form-item prop="isFilterDevice" class="switch_arrange">
              <span slot="label">是否过滤设备Id</span>
              <el-switch v-model="clearForm.isFilterDevice"></el-switch>
            </el-form-item>
            <el-form-item label="备注">
              <el-input type="textarea" v-model="clearForm.mark"></el-input>
            </el-form-item>
          </el-form>-->
          <div class="warn_form">
            <div class="warn_form_li" v-if="!filDeviceId">
              <span class="label">清除事件【{{activeWarn.Name}}】的关联报警</span>
              <el-switch v-model="clearForm.isFilterFun"></el-switch>
            </div>
            <div class="warn_form_li" v-if="!filProductId">
              <span class="label">清除设备【{{activeWarn.DeviceName}}】的关联报警</span>
              <el-switch v-model="clearForm.isFilterDevice"></el-switch>
            </div>
            <div class="warn_form_li mark_li">
              <div class="label_text">备注</div>
              <el-input type="textarea" v-model="clearForm.mark"></el-input>
            </div>
          </div>
        </div>
        <div slot="footer">
          <el-button @click="clearDialog = false">取 消</el-button>
          <el-button type="primary" @click="getWarnMsg">确 定</el-button>
        </div>
      </el-dialog>
      <warning-config ref="warnConfigDlg"></warning-config>
    </div>
  </div>
</template>

<script>
import {
  getWarnList,
  clearAllWarning,
  clearEachWarning
} from "@/api/rules/productModel";
import warningConfig from './components/warningConfig.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "WarnList",
  components: {
    warningConfig
  },
  mixins: [resizeTableCon],
  props: {
    filProductId: {
      type: String,
      default: ""
    },
    filDeviceId: {
      type: String,
      default: ""
    },
    isComponent:{
      type:Boolean,
      default:false
    }
  },
  data() {
    return {
      clearForm: {
        isFilterFun: false,
        isFilterDevice: false,
        mark: ""
      },
      activeWarn: {},
      clearDialog: false,
      classmap: new Map(),
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 升级列表表格数据
      warnTableList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 表单参数
      form: {},
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Name: ""
      },
      // 列信息
      columns: [
        { key: 0, label: `告警名称`, visible: true },
        { key: 1, label: `事件标识`, visible: true },
        { key: 2, label: `工单编号`, visible: true },
        { key: 3, label: `告警描述`, visible: true },
        { key: 4, label: `报警级别`, visible: true },
        { key: 5, label: `设备Id`, visible: true },
        { key: 6, label: `消息内容`, visible: true },
        { key: 7, label: `状态`, visible: true },
        { key: 8, label: `处理时间`, visible: true },
        { key: 9, label: `处理人员`, visible: true },
        { key: 10, label: `告警时间`, visible: true },
        { key: 11, label: `设备名称`, visible: true }
      ],
      warnFilter: {}
    };
  },
  mounted(){
    this.getList();
  },
  methods: {
    getWarnMsg() {
      this.clearDialog = false;
      if (this.clearForm.isFilterDevice && this.clearForm.isFilterFun) {
        this.warnFilter = {
          devId: this.activeWarn.DeviceId,
          code: this.activeWarn.Code,
          remark: this.clearForm.mark
        };
        this.clearPartWarn(this.warnFilter);
      } else if (this.clearForm.isFilterDevice && !this.clearForm.isFilterFun) {
        this.warnFilter = {
          devId: this.activeWarn.DeviceId,
          remark: this.clearForm.mark
        };
        this.clearPartWarn(this.warnFilter);
      } else if (!this.clearForm.isFilterDevice && this.clearForm.isFilterFun) {
        this.warnFilter = {
          code: this.activeWarn.Code,
          remark: this.clearForm.mark
        };
        this.clearPartWarn(this.warnFilter);
      } else {
        this.warnFilter = {
          id: this.activeWarn.Id,
          remark: this.clearForm.mark
        };
        this.clearPartWarn(this.warnFilter);
      }
      
    },
    openClearDialog(row) {
      //打开处理弹窗
      this.clearDialog = true;
      this.activeWarn = row;
      this.clearForm={
        isFilterFun: false,
        isFilterDevice: false,
        mark: ""
      }
    },
    clearAllWarn() {
      //清除所有报警
      if (this.filDeviceId) {
        this.$modal
          .confirm("是否确认处理全部报警？")
          .then(function() {
            return clearAllWarning({ devId: this.filDeviceId });
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("处理成功");
          })
          .catch(() => {});
      } else {
        this.$modal
          .confirm("是否确认处理全部报警？")
          .then(function() {
            return clearAllWarning();
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("处理成功");
          })
          .catch(() => {});
      }
    },
    clearPartWarn(filterParam) {
      //处理部分报警列表
      if (filterParam.id) {
        clearEachWarning(filterParam).then(() => {
            this.getList();
            this.$modal.msgSuccess("处理成功");
          });
      } else {
        let text=''
        if(filterParam.devId&&filterParam.code){
          text='设备'+this.activeWarn.DeviceName+'和'+'标识符'+this.activeWarn.Name+'相关的'
        }else if(filterParam.devId){
          text='设备'+this.activeWarn.DeviceName+'相关的'
        }else if(filterParam.code){
          text='标识符'+this.activeWarn.Name+'相关的'
        }
        this.$modal
          .confirm("是否确认处理"+text+"全部报警？")
          .then(function() {
            return clearAllWarning(filterParam);
          })
          .then(() => {
            this.getList();
            this.$modal.msgSuccess("处理成功");
          })
          .catch(() => {});
      }
    },
    filterSatus(value, row) {
      return row.Status === value;
    },
    filterLevel(value, row) {
      return row.Level === value;
    },
    /** 查询报警列表 */
    getList() {
      this.loading = true;
      if (this.filProductId) {
        this.queryParams.ProductId = this.filProductId;
      }
      if (this.filDeviceId) {
        this.queryParams.DeviceId = this.filDeviceId;
      }
      getWarnList(this.addDateRange(this.queryParams, this.dateRange)).then(
        async response => {
          // console.log("查询到的报警列表", response);
          this.warnTableList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },

    goWarnConfig(){
      this.$refs.warnConfigDlg.openDlg(this.filProductId);
    }
  }
};
</script>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";
//@import "~@/assets/icons/iconfont.css";
// .app-container {
//   padding-right: 30px;
// }
.data_table {
  th div.cell .el-table__column-filter-trigger .el-icon-arrow-down::before {
    font-family: "zhongtaiiconfont" !important;
    // font-size: 24px;
    font-size: var(--shaixuan);
    font-style: normal;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    content: "\e6d2";
  }
  .col_con {
    width: 320px;
    display: flex;
    justify-content: flex-start;
    .col_left {
      display: flex;
      align-items: center;
      img {
        width: 54px;
        height: 54px;
      }
    }
    .col_right {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-left: 12px;
      .col_right_top {
        line-height: 22px;
        text-align: left;
        width: 100%;
        color: #0054fc;
      }
      .col_right_bottom {
        line-height: 22px;
        .right_bottom1 {
          display: flex;
          align-items: flex-start;
          justify-content: flex-start;
          flex-direction: column;
          .proName {
            color: #0054fc;
          }
        }
        .right_bottom2 {
          display: flex;
          align-items: center;
          justify-content: flex-start;
          .data_icon {
            display: block;
            width: 44px;
            height: 16px;
            color: #0054fc;
            .el-tooltip__popper .popper__arrow {
              border-width: 6px;
              border-color: rgb(0, 0, 0, 1);
            }
            i {
              display: none;
              margin-left: 8px;
            }
          }
        }
      }
    }
  }
}
.data_table.el-table table tr.el-table__row:hover {
  .col_con .col_right .col_right_bottom .right_bottom2 .data_icon i {
    display: inline;
    cursor: pointer;
  }
}
.warn_form {
  .warn_form_li {
    margin-bottom: 30px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    span.label {
      margin-right: 12px;
      &.tips{
        font-size: 12px;
        color: #78829D;
      }
    }

  }
  .warn_form_li.mark_li {
    margin-bottom: 30px;
    display: block;
    div.label_text {
      margin-bottom: 10px;
    }
  }
}
</style>