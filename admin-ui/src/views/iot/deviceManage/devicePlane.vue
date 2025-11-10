<template>
  <div>
    <div v-if="!isTask">
      <div class="from_con" id="from_con" v-if="showSearch" style="margin-bottom:0">
        <el-form :model="taskForm" ref="taskForm" :inline="true" class="biaodan">
          <el-form-item label="流程发起人" prop="UserName">
            <el-select filterable allow-create default-first-option v-model="taskForm.UserName" ref="selectFlowCreatedUser"
                  placeholder="请选择发起人" @focus="getCreatedFocus" style="width:160px" :disabled="false"></el-select>
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker
              class="set_radius"
              v-model="dateRange"
              style="width:232px"
              value-format="yyyy-MM-dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>
          <el-form-item label="任务状态" prop="TaskStatus">
            <el-select class="set_radius" v-model="taskForm.TaskStatus" placeholder="请选择任务状态" clearable style="width:160px">
              <el-option v-for="dict in statusList" :key="dict.val" :label="dict.text" :value="dict.val" />
            </el-select>
          </el-form-item>
          <el-form-item label="计划类型" prop="PlanTypeId">
            <el-select class="set_radius" v-model="taskForm.PlanTypeId" placeholder="请选择计划类型" clearable style="width:160px">
              <el-option v-for="dict in planetbList" :key="dict.Id" :label="dict.Name" :value="dict.Id" />
            </el-select>
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleClick">搜索</el-button>
          </el-form-item>
        </el-form>
      </div>
      <el-row :gutter="10" class="mb8 button_row">
        <div>
          <el-col :span="1.5">
            <el-button type="primary" plain @click="handleStartTask">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">发起任务</span>
            </el-button>
          </el-col>
        </div>
        <right-toolbar :showSearch.sync="showSearch" @queryTable="getTaskList"></right-toolbar>
      </el-row>
      <taskList :loading="loading" :tbList="taskList" :cellSty="cellSty" @reloadList="handleClick" :isdevice="true"></taskList>
      <pagination v-show="total > 0" :total="total" :page.sync="taskForm.pageNum" :limit.sync="taskForm.pageSize"
        @pagination="getTaskList" />
    </div>
    <task_add ref="task_add" @reloadList="handleClick"></task_add>
      <org-picker :multiple="false" ref="flowCreatedUser" @ok="selectLeadered"/>
      <plane_dialog ref="planedialog" @openTaskAdd="openTaskAdd"></plane_dialog>
  </div>
</template>

<script>
import { devPlaneTaskList,devPlaneList } from "@/api/after/devplane";
import deviceTask from './deviceTask.vue'
import taskList from '@/views/after/devplane/taskList';
import plane_dialog from '@/views/after/devplane/plane_dialog';
import { resizeTableCon } from "@/mixins/resizeTableCon";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import task_add from '@/views/after/devplane/task_add';
export default {
  name: 'devicePlane',
  mixins: [resizeTableCon],
  components: {
    deviceTask,
    plane_dialog,
    taskList,
    OrgPicker,
    task_add
  },
  props: {
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      taskList:[],
      dateRange:[],
      statusList:[],
      showSearch:true,
      tbList: [],
      loading: false,
      total: 0,
      planeForm: {
        pageNum: 1,
        pageSize: 0,
        PlanTypeId: '',
        UserName: '',
        TaskStatus: '',
        CanStart: true
      },
      taskForm:{
        pageNum:1,
        pageSize:10,
        TaskStatus: ''
      },
      isTask: false,
      activePlaneId: '',
      activePlanTypeName: '',
      planetbList:[],
    };
  },

  mounted() {
    this.setstatusList()
    
  },
  watch: {
    deviceInfos: {
      handler(val) {
        if(this.deviceInfos.Id){
          this.isTask = false
          this.taskForm.pageNum = 1
          this.taskForm.DeviceId = this.deviceInfos.Id
          this.planeForm.DeviceId = this.deviceInfos.Id
          this.getTaskList()
          this.$nextTick(()=>{
            this.getDevPlaneList()
          })
        }
        
      },
      deep: true,
      immediate: true
    }
  },
  methods: {
    getDevPlaneList(){
        devPlaneList(this.planeForm).then(res=>{
          // console.log(res,'计划类型');
          this.planetbList=JSON.parse(JSON.stringify(res.data.List))
          // for(let i=0;i<5;i++){
          //     this.tbList=[...this.tbList,...this.tbList]
          // }
        }).catch(err=>{
        })
      },
    getTaskList(){
      this.loading=true
      devPlaneTaskList(this.addDateRange(this.taskForm, this.dateRange)).then(res=>{
        // console.log("计划任务",res);
        this.taskList=res.data.List
        this.total=res.data.Total
        this.loading=false
      }).catch(err=>{
        this.loading=false
      })
    },
    openTaskAdd(item){
      //打开添加
      this.$refs.task_add.openDialog(item.Id)
      // this.$refs.task_add.openAddDevice(item.Id)
    },
    getCreatedFocus(){
      //发起人选择开始
      this.$refs.selectFlowCreatedUser.blur();
      let flowCreatedUser = []
      this.$refs.flowCreatedUser.show(flowCreatedUser, "user");
    },
    selectLeadered(val){
      //选择人
      if(val&&val.length>0){
        this.taskForm.UserId=val[0].id
        this.taskForm.UserName=val[0].name
      }else{
        delete this.taskForm.UserId
        delete this.taskForm.UserName
      }
      
      // this.devplaneFrom.Avatar=val[0].avatar
      this.$forceUpdate()
    },
    handleStartTask(){
      this.$refs.planedialog.openDialog(this.deviceInfos.Id)
    },
    handleClick(){
      this.taskForm.pageNum=1
      this.tbList=[]
      this.loading=true
      this.getTaskList()
    },
    /** 重置按钮操作 */
    resetQuery() {
        this.dateRange = [];
        this.resetForm("taskForm");
        this.handleClick();
    },
    setstatusList(){
        this.statusList=[{
          text:'进行中',
          color:'#F158D7',
          val:0
        },{
          text:'待执行',
          color:'#DEA11E',
          val:1
        },{
          text:'执行中',
          color:'#358AEF',
          val:2
        },{
          text:'已完成',
          color:'#78BF34',
          val:3
        },{
          text:'已过期',
          color:'#B5B5B5',
          val:4
        },{
          text:'已验收',
          color:'#47CFB6',
          val:5
        },{
          text:'验收失败',
          color:'#EF357E',
          val:6
        },{
          text:'已作废',
          color:'#666666',
          val:7
        },]
      },
  },
};
</script>
<style lang="less" scoped>
   .el-select{
    ::v-deep input{
      width: 100%;
    }
  }
</style>