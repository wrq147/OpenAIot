<template>
  <div>
    <div v-loading="loading">
        <div class="plane_title_con">
            <el-button @click="returnPlane" size="small" style="margin-right:10px;" icon="el-icon-back" circle></el-button>
            <span>{{planTypeName}}</span>
        </div>
        <taskList :loading="loading" :tbList="tbList" :cellSty="cellSty" @reloadList="handleClick" :isdevice="true"></taskList>
        <pagination v-show="total > 0" :total="total" :page.sync="taskForm.pageNum" :limit.sync="taskForm.pageSize" @pagination="getTaskList()"/>
    </div>
  </div>
</template>

<script>
import taskList from '@/views/after/devplane/taskList';
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { devPlaneTaskList } from "@/api/after/devplane";
export default {
  name: 'deviceTask',
  components:{
    taskList
  },
  props: {
    deviceId: {
      type: String,
      default: '',
    },
    planTypeId: {
      type: String,
      default: '',
    },
    planTypeName: {
      type: String,
      default: '',
    },
  },
  mixins: [resizeTableCon],
  watch: {
    deviceId: {
      handler(val) {
        this.taskForm.pageNum=1
        this.taskForm.PlanTypeId=this.planTypeId
        this.taskForm.DeviceId=this.deviceId
        this.getTaskList()
      },
    }
  },
  data() {
    return {
        taskListOpen:false,
        tbList:[],
        loading:false,
        total:0,
        taskForm:{
          pageNum:1,
          pageSize:10,
        },
    };
  },

  mounted() {
    
  },

  methods: {
    returnPlane(){
        this.$emit('returnPlane')
    },
    getTaskList(){
        this.loading=true
        devPlaneTaskList(this.taskForm).then(res=>{
          // console.log("计划任务111",res);
          this.tbList=res.data.List
          this.total=res.data.Total
          this.loading=false
        }).catch(err=>{
          this.loading=false
        })
    },
    handleClick(){
        this.taskForm.pageNum=1
        this.taskForm.PlanTypeId=this.planTypeId
        this.taskForm.DeviceId=this.deviceId
        this.tbList=[]
        this.loading=true
        this.getTaskList()
      },
  },
};
</script>
<style lang="less" scope>

</style>