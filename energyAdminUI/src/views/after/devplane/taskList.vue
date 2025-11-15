<template>
  <div>
    <!-- <el-tabs v-model="activeName" type="card" @tab-click="tabClick" style="margin-top:25px;">
        <el-tab-pane label="全部" name="-1"></el-tab-pane>
        <el-tab-pane label="待提交" name="0"></el-tab-pane>
        <el-tab-pane label="待审批" name="1"></el-tab-pane>
        <el-tab-pane label="入库成功" name="2"></el-tab-pane>
        <el-tab-pane label="待退货" name="3"></el-tab-pane>
        <el-tab-pane label="已退货" name="4"></el-tab-pane>
    </el-tabs> -->
    <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty" style="width:100%" row-key="Id">
        <el-table-column label="流程信息" align="left" prop="PlaneNumber" width="160">
            <template slot-scope="scope">
                <div v-if="scope.row.FlowInfo" class="table_icon">
                    <i :class="scope.row.Icon" :style="'background: ' + scope.row.Background"></i>
                    <span>{{ scope.row.FlowInfo.FlowName }}</span>
              </div>
            </template>
        </el-table-column>
        <el-table-column label="任务编号" align="center" prop="PlaneNumber" width="160">
            <template slot-scope="scope">
                <el-link type="primary" @click="viewInfo(scope.row)">
                    {{ scope.row.PlaneNumber }}
                </el-link>
            </template>
        </el-table-column>
        <el-table-column label="设备名称">
            <template slot-scope="scope" v-if="scope.row.TargetDevice">
                <span>{{ scope.row.TargetDevice.Name }}</span>
            </template>
        </el-table-column>
        <el-table-column label="设备编号">
            <template slot-scope="scope" v-if="scope.row.TargetDevice">
                <span>{{ scope.row.TargetDevice.DeviceNumber }}</span>
            </template>
        </el-table-column>
        <el-table-column label="任务状态" align="center" width="100">
            <template slot-scope="scope">
                <el-tag v-if="scope.row.TaskStatus == 0" type="warning">进行中</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 1" type="warning">待执行</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 2" type="warning">执行中</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 3" type="success">已完成</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 4" type="info">已过期</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 5" type="success">已验收</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 6" type="info">验收失败</el-tag>
                <el-tag v-else-if="scope.row.TaskStatus == 7" type="info">已作废</el-tag>
            </template>
        </el-table-column>
        <el-table-column label="任务生成时间" align="center" width="160">
        <template slot-scope="scope">
            <div>{{ parseTime(scope.row.CreatedOn) }}</div>
        </template>
        </el-table-column>
        <el-table-column label="开始时间" align="center" width="160">
        <template slot-scope="scope">
            <div>{{ parseTime(scope.row.StartOn) }}</div>
        </template>
        </el-table-column>
        <el-table-column label="截止时间" align="center" width="160">
        <template slot-scope="scope">
            <div>{{ parseTime(scope.row.EndOn) }}</div>
        </template>
        </el-table-column>
        <!-- <el-table-column label="执行时间" align="center" width="160">
        <template slot-scope="scope">
            <el-tag type="warning" v-if="scope.row.ExecutedOn">{{ parseTime(scope.row.ExecutedOn) }}</el-tag>
        </template>
        </el-table-column> -->
        <el-table-column label="发起人" align="center" prop="UserId" width="120">
            <template slot-scope="scope">
                <div v-if="scope.row.UserId">{{scope.row.UserInfo.RealName}}</div>
            </template>
        </el-table-column>
        <el-table-column label="所属计划" align="center" width="160" prop="PlanName" v-if="!isDevice">
            <template slot-scope="scope">
                <el-link type="primary" @click="viewPlanTypeInfo(scope.row)">
                    {{ scope.row.PlanName }}
                </el-link>
            </template>
        </el-table-column>
        <el-table-column label="所属计划" align="center" width="160" prop="PlanName" v-if="isDevice"></el-table-column>
        <el-table-column label="操作" align="center" class-name="small-padding fixed-width" fixed="right" width="120">
            <template slot-scope="scope" v-if="scope.row.TodoTasks&&scope.row.TodoTasks.length>0||scope.row.TaskStatus != 7">
                <el-button type="text" icon="el-icon-video-play" @click="handleOpenTodp(scope.row)" v-if="scope.row.TodoTasks&&scope.row.TodoTasks.length>0" style="margin-left:0;margin-right:0">执行</el-button>
                <el-button type="text" icon="el-icon-delete" @click="handleDiscard(scope.row)" v-if="scope.row.TaskStatus != 7">作废</el-button>
            </template>
        </el-table-column>
    </el-table>
    <plane_add ref="planeAdd"></plane_add>
    <el-dialog
        :title="detailtitle"
        :visible.sync="detailopen"
        width="1000px"
        :close-on-click-modal="false"
        append-to-body
        :destroy-on-close="true"
    >
        <div class="action-form">
            <!--流程流转记录-->
            <div class="xx-form">
                <form-render ref="form" :forms="formConf" v-model="formValues" style="width:610px;"/>
                <div class="xx-right">
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
            </div>
        </div>
    </el-dialog>
    <el-dialog
        title="任务待办项"
        :visible.sync="toDoOpen"
        width="1000px"
        :close-on-click-modal="false"
        append-to-body
        :destroy-on-close="true"
    >
        <div class="action-form">
            <el-table :close-on-click-modal="false" :data="toDoList" class="data_table" border>
                <el-table-column label="任务编号" align="center" prop="ExecutionNodeId" :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                        <div class="flow_icon">
                            <i
                            :class="scope.row.Icon"
                            :style="'background: ' + scope.row.Background"
                            ></i>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column label="任务编号" align="center" prop="ExecutionNodeId" :show-overflow-tooltip="true"/>
                <el-table-column label="流程名称" align="center" prop="FlowName" />
                <el-table-column label="任务节点" align="center" prop="StepName" />
                <el-table-column label="接收时间" align="center" prop="StartTime" width="180" />
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
                    <template slot-scope="scope">
                        <el-button type="text" icon="el-icon-edit-outline" @click="handleProcess(scope.row)">处理</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
    </el-dialog>
    <record-form ref="recordForm" @finish="finishTask"></record-form>
  </div>
</template>

<script>
import plane_add from './devplane_add.vue'
import { cancelTask,devPlaneTaskInfo } from "@/api/after/devplane";
import {
  flowRootRecord
} from "@/api/flowable/process";
import FormRender from "@/views/flowable/common/form/FormRender";
import NodeInfoTree from "@/views/flowable/task/record/NodeInfoTree.vue";
import RecordForm from "@/views/flowable/task/record/edit.vue";
export default {
  name: 'AdminUiTaskList',
  props:['loading','tbList','cellSty','isDevice'],
  components: { plane_add,FormRender,NodeInfoTree,RecordForm },
  data() {
    return {
        toDoOpen:false,
        activeName:'-1',
        formConf: [], // 默认表单数据
        formValues: {},
        detailopen:false,
        node_list:[],
        detailtitle:'任务详情',
        toDoList:[]
    };
  },

  mounted() {
    
  },

  methods: {
    finishTask(){
        //完成任务
        this.toDoOpen=false
        this.$emit('reloadList')
    },
    // 打开处理页面
    handleProcess(row) {
      this.$refs.recordForm.openDialog(row.ExecutionNodeId);
    },
    handleOpenTodp(row){
        //打开待办项
        if(row.TodoTasks&&row.TodoTasks.length==1){
            this.handleProcess(row.TodoTasks[0])
        }else{
            devPlaneTaskInfo({id:row.Id}).then(res=>{
                let data=res.data
                // console.log("流程信息",data);
                this.toDoList=JSON.parse(JSON.stringify(data.TodoTasks))
                this.toDoOpen=true
            })
        }
    },
    /** 流程流转记录 */
    handleDiscard(row){
        const ids = row.Id;
        this.$modal.confirm('是否确认废弃编号为"' + row.PlaneNumber + '"的计划任务？')
        .then(function() {
            return cancelTask({ id: ids });
        })
        .then(rsp => {
            this.$emit('reloadList')
            this.$modal.msgSuccess("废弃成功");
        })
        .catch(() => {});
    },
    async viewInfo(row){
        //任务计划详情即流程详情
        this.detailopen = true;

        // 初始化表单
        let rsp = await flowRootRecord(row.FlowId);
        let commitOperates = rsp.data.Step.FormPerms.toMap("id");
        let jsondata = rsp.data.NodeField;
        this.formValues = rsp.data.Model;
        this.detailtitle = rsp.data.FormName;
        this.node_list = rsp.data.NodeList;

        this.formConf = this.filterPerm(jsondata,commitOperates);
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
            } else if (opval.perm == "R") {
              it.props.disabled = true;
              return true;
            }
          }
          it.props.disabled = true;
          return true;
        }
      });
    },
    tabClick(){

    },
    viewPlanTypeInfo(row){
        //计划类型详情
        this.$refs.planeAdd.openDialog(row.PlanTypeId,true)
    }
  },
};
</script>
<style lang="less" scoped>
.action-form {
  min-height: 300px;
  display: flex;
  flex-direction: column;
  .xx-form{
    display: flex;
    justify-content: space-between;
    .xx-right{
      width:330px;
      padding-left: 20px;
      border-left: solid 1px #dadada;
    }
  }
}
.flow_icon{
    i {
      border-radius: 10px;
      padding: 7px;
      font-size: 20px;
      color: #ffffff;
      margin-right: 10px;
    }
}
.table_icon{
    
    i{
        width: 30px;
        height:30px;
        border-radius:6px;
        display:inline-flex;
        justify-content:center;
        align-items:center;
        font-size:16px;
        color:#ffffff;
        margin-right: 6px;
    }
}
    
</style>