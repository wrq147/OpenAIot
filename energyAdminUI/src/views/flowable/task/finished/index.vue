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
          <el-form-item label="名称" prop="key">
            <el-input
              v-model="queryParams.key"
              class="set_radius"
              placeholder="请输入搜索关键字"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item label="接收时间" prop="deployTime">
            <el-date-picker
              v-model="dateRange"
              class="set_radius"
              style="width: 250px"
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
        <el-table v-loading="loading" class="data_table" :data="finishedList" border>
          <el-table-column
            label="任务编号"
            align="center"
            prop="ExecutionNodeId"
            :show-overflow-tooltip="true"
            :header-cell-style="cellSty"
            style="width:100%"
          />
          <el-table-column
            label="流程名称"
            align="center"
            prop="FlowName"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="任务节点" align="center" prop="StepName" />
          <el-table-column label="流程发起人" align="center">
            <template slot-scope="scope">
              <label>
                <el-tag>{{scope.row.startDeptName}}</el-tag>
                {{ scope.row.startRealName }}
              </label>
            </template>
          </el-table-column>
          <el-table-column label="接收时间" align="center" prop="StartTime" width="180" />
          <el-table-column label="处理时间" align="center" prop="EndTime" width="180" />
          <el-table-column label="耗时" align="center" width="180">
            <template slot-scope="scope">
              <label>{{ caldurTime(scope.row) }}</label>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-tickets"
                @click="handleFlowRecord(scope.row)"
              >查看详情</el-button>
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
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="1100px"
        append-to-body
      >
        <div class="action-form">
          <form-render ref="form" :forms="formConf" v-model="formValues" style="width:710px;" />
          <!--流程流转记录-->
          <div v-if="node_list.length > 0" class="right-nodes">
            <!-- style="border-top: solid 1px #dadada" -->
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
import FormRender from "../../common/form/FormRender";
import { flowRecord, finishedList } from "@/api/flowable/process.js";
import { str2Date, deltaTime } from "@/utils/index";
import "../../common/utlity.js";
import { resizeTableCon } from "@/mixins/resizeTableCon";

export default {
  name: "FinishedList",
  components: {
    FormRender,
    NodeInfoTree
  },
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 已办任务列表数据
      finishedList: [],
      nodeId: null,
      formConf: [], // 默认表单数据
      formValues: {},
      node_list: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        key: null
      }
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询流程完成列表 */
    getList() {
      this.loading = true;
      finishedList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.finishedList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
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
    filterPerm(formItems,commitOperates){
      return formItems.filter((it) => {
        if (it.name === "SpanLayout") {
          it.items=this.filterPerm(it.props.items,commitOperates);
          it.props.disabled = false;
          return it.items.length>0;
        }
        else{
          let opval = commitOperates.get(it.id);
          it.props.disabled = true;
          if (opval != null) {
            if (opval.perm == "H") {
              return false;
            } else if (opval.perm == "R") {
              return true;
            }
          }
          return true;
        }
      });
    },
    /** 流程流转记录 */
    async handleFlowRecord(row) {
      this.nodeId = row.ExecutionNodeId;
      this.open = true;
      // 初始化表单
      let rsp = await flowRecord(this.nodeId);
      let commitOperates = rsp.data.Step.FormPerms.toMap("id");
      let jsondata = rsp.data.NodeField;
      this.formValues = rsp.data.Model;
      this.title = rsp.data.FormName;
      this.node_list = rsp.data.NodeList;

      this.formConf = this.filterPerm(jsondata,commitOperates);
    },
    caldurTime: function(row) {
      if (row.EndTime == null) {
        return deltaTime(str2Date(row.StartTime), new Date());
      } else {
        return deltaTime(str2Date(row.StartTime), str2Date(row.EndTime));
      }
    }
  }
};
</script>

<style lang="scss" scope>
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