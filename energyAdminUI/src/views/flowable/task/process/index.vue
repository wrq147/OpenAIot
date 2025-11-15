<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          ref="queryForm"
          :inline="true"
          class="biaodan"
        >
          <el-form-item label="名称" prop="name">
            <el-input
              v-model="queryParams.name"
              class="set_radius"
              placeholder="请输入名称"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker
              v-model="dateRange"
              class="set_radius"
              style="width: 240px"
              value-format="yyyy-MM-dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="mb8 button_row">
          <el-col :span="1.5">
            <el-button
              type="primary"
              plain
              @click="handleAdd"
              v-hasPermi="['/FlowService/Task/Add']"
            >
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">新增流程</span>
            </el-button>
          </el-col>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          v-loading="loading"
          :data="myProcessList"
          class="data_table"
          :header-cell-style="cellSty"
          style="width:100%"
          border
        >
          <el-table-column label="流程编号" align="center" prop="Id" :show-overflow-tooltip="true" />
          <el-table-column
            label="流程名称"
            align="center"
            prop="FlowName"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="流程类别" align="center" prop="GroupName" width="100px" />
          <el-table-column
            label="流程描述"
            align="center"
            prop="Description"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="提交时间" align="center" prop="createTime" width="180" />
          <el-table-column label="流程状态" align="center" width="100">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0">运行中</el-tag>
              <el-tag type="warning" v-if="scope.row.Status == 1">保存中</el-tag>
              <el-tag type="success" v-if="scope.row.Status == 2">已完成</el-tag>
              <el-tag type="info" v-if="scope.row.Status == 3">已取消</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="耗时" align="center" width="180">
            <template slot-scope="scope">
              <label>{{ caldurTime(scope.row) }}</label>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-dropdown>
                <el-button type="text" class="el-dropdown-link">
                  更多操作
                  <i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item
                    icon="el-icon-tickets"
                    @click.native="handleFlowRecord(scope.row)"
                  >详情</el-dropdown-item>
                  <el-dropdown-item
                    icon="el-icon-circle-close"
                    v-if="scope.row.Status == 1"
                    @click.native="editProccess(scope.row)"
                  >编辑</el-dropdown-item>
                  <el-dropdown-item
                    icon="el-icon-circle-close"
                    v-if="scope.row.Status == 0 || scope.row.Status == 1"
                    @click.native="handleStop(scope.row)"
                  >取消</el-dropdown-item>
                  <el-dropdown-item
                    icon="el-icon-delete"
                    v-if="scope.row.Status == 2 || scope.row.Status == 3"
                    @click.native="handleDelete(scope.row)"
                  >删除</el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
      </div>
      <!-- 发起流程 -->
      <selectProcess ref="startProcess" @handleStartProcess="handleStartProcess" :title="title"></selectProcess>

      <el-dialog :title="detailtitle" :visible.sync="detailopen" width="1000px" :close-on-click-modal="false" append-to-body>
        <div class="action-form">
          <form-render ref="form" :forms="formConf" v-model="formValues" style="width:610px;" :disabled="true"/>
          <!--流程流转记录-->
          <div v-if="node_list.length > 0" class="right-nodes">
            <div class="clearfix" style="font-size: 16px; padding-top: 20px; padding-bottom: 20px">
              <span>流程记录</span>
            </div>
            <div class="block">
              <node-info-tree :nodeList="node_list"></node-info-tree>
            </div>
          </div>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import NodeInfoTree from "../record/NodeInfoTree.vue";
import selectProcess from "./selectProcess.vue";
import {
  myProcessList,
  canProcess,
  delProcess,
  flowRootRecord
} from "@/api/flowable/process";
import { str2Date, deltaTime } from "@/utils/index";
import FormRender from "../../common/form/FormRender";
import "../../common/utlity.js";
import { resizeTableCon } from "@/mixins/resizeTableCon";

export default {
  name: "ProcessList",
  components: { FormRender, NodeInfoTree,selectProcess },
  mixins: [resizeTableCon],
  data() {
    return {
      lis: [1, 2, 3], //用于测试循环的
      // 遮罩层
      loading: true,
      processLoading: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      processTotal: 0,
      // 我发起的流程列表数据
      myProcessList: [],
      node_list: [],
      // 弹出层标题
      title: "发起流程",
      // 是否显示弹出层
      open: false,
      detailtitle: "",
      detailopen: false,
      formConf: [], // 默认表单数据
      formValues: {},
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        name: null,
        templateId: null
      },
    };
  },
  created() {
    this.getList();
  },
  methods: {
    endProcess() {
      //鼠标移开事件
      this.activeProgess = -1;
    },
    startProcess(id) {
      //鼠标移上去的事件
      this.activeProgess = id;
    },

    getList() {
      this.loading = true;
      myProcessList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.myProcessList = response.data.List;
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
      this.resetForm("queryForm");
      this.handleQuery();
    },

    /** 搜索按钮操作 */
    /** 新增按钮操作 */
    handleAdd() {
      this.title = "发起流程";
      this.$refs.startProcess.handleOpen()
    },
    /**编辑流程 */
    editProccess(row) {
      this.$router.push({
        path: "/flowable/record",
        query: {
          procDefId: row.TemplateId,
          id: row.Id
        }
      });
    },
    /**  发起流程申请 */
    handleStartProcess(row) {
      this.$router.push({
        path: "/flowable/record",
        query: {
          procDefId: row.Id
        }
      });
    },
    /**  取消流程申请 */
    handleStop(row) {
      canProcess(row.Id).then(res => {
        this.$modal.msgSuccess("取消成功");
        this.getList();
      });
    },
    filterPerm(formItems,commitOperates){
      return formItems.filter((it) => {
        if (it.name === "SpanLayout") {
          it.items=this.filterPerm(it.props.items,commitOperates);
          it.props.disabled = false;
          return it.items.length>0;
        }
        else{
          let opval = commitOperates.get(it.id);
          if (opval != null) {
            if (opval.perm == "H") {
              return false;
            }
          }
          it.props.disabled = true;
          return true;
        }
      });
    },
    /** 流程流转记录 */
    async handleFlowRecord(row) {
      this.detailopen = true;

      // 初始化表单
      let rsp = await flowRootRecord(row.Id);
      let commitOperates = rsp.data.Step.FormPerms.toMap("id");
      let jsondata = rsp.data.NodeField;
      this.formValues = rsp.data.Model;
      this.detailtitle = rsp.data.FormName;
      this.node_list = rsp.data.NodeList;

      this.formConf = this.filterPerm(jsondata,commitOperates);
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      this.$confirm(
        '是否确认删除流程定义编号为"' + row.Id + '"的数据项?',
        "警告",
        {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning"
        }
      )
        .then(function() {
          return delProcess(row.Id);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        });
    },
    caldurTime: function(row) {
      if (row.FinishTime == null) {
        return deltaTime(str2Date(row.createTime), new Date());
      } else {
        return deltaTime(str2Date(row.createTime), str2Date(row.FinishTime));
      }
    }
  }
};
</script>
<style lang="less" scope>
.content_con {
  padding-top: 15px;
  .content_li_con:last-child {
    border-bottom: 1px solid #ebeef5;
  }
  .content_li_con {
    padding: 12px;
    border-top: 1px solid #ebeef5;

    .title {
      font-size: 16px;
      text-align: left;
    }
    .content_ul {
      width: 100%;
      display: flex;
      justify-content: flex-start;
      flex-wrap: wrap;
      margin: 0;
      padding: 0;
      margin-top: 20px;
      margin-left: -20px;

      .li_con {
        margin-left: 20px;
        width: 20%;
        height: 70px;
        box-sizing: border-box;
        margin-top: 10px;
        display: flex;
        justify-content: flex-start;

        .content_li {
          cursor: pointer;
          width: 100%;
          height: 70px;
          padding: 0 5px 0 10px;
          display: flex;
          justify-content: space-between;
          align-items: center;
          border: 1px solid #ebeef5;
          box-sizing: border-box;
          border-radius: 5px;
          &.active_li {
            border: 1px solid #448ed7;
          }
          .left_cont {
            display: flex;
            align-items: center;
            .li-icons {
              width: 30px;
              height: 30px;
              border-radius: 5px;
              font-size: 20px;
              text-align: center;
              line-height: 30px;
              flex-shrink: 0; //由于子元素宽度之和超出了弹性盒子宽度，因此两个子元素被等比例压缩，解决方法
            }
            span {
              margin-left: 5px;
            }
          }
          .active_content {
            color: #38adff;
            font-size: 12px;
            min-width: 48px;
            // flex-shrink:0;
            text-align: right;
            margin-left: 3px;
          }
        }
      }
    }
  }
}

.action-form{
  display: flex;
  justify-content: space-between;
  .right-nodes{
    width:330px;
    padding-left: 20px;
    border-left: solid 1px #dadada;
  }
}
</style>
