<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="from_con" id="from_con" v-if="showSearch">
      <el-form :model="taskForm" ref="taskForm" :inline="true" class="biaodan">
        <el-form-item label="计划发起类型" prop="StartWay">
          <el-select class="set_radius" v-model="taskForm.StartWay" placeholder="请选择计划发起类型" clearable>
            <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value" />
          </el-select>
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
        <el-form-item class="submit_button_con">
          <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="getList">搜索</el-button>
        </el-form-item>
      </el-form>
    </div>
    <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
      <el-row :gutter="10" class="mb8 button_row">
        <div>
          <el-col :span="1.5">
            <el-button type="primary" plain @click="handleAdd">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">新增设备计划</span>
            </el-button>
          </el-col>
        </div>
        <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>
      <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty" style="width:100%" row-key="Id">
        <el-table-column label="编码" align="left" prop="Id"></el-table-column>
        <el-table-column label="计划名称" align="left" prop="Name"></el-table-column>
        <el-table-column label="发起方式" align="center" width="100">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.StartWay == 0" type="info">手动发起</el-tag>
            <el-tag v-else-if="scope.row.StartWay == 1" type="warning">定时发起</el-tag>
            <el-tag v-else-if="scope.row.StartWay == 2" type="success">设备事件</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="定时时间" align="center" prop="TimerName"></el-table-column>
        <el-table-column label="计划执行天数" align="center" prop="PlaneDays" width="100"></el-table-column>
        <el-table-column label="发起人名称" align="center" prop="FlowCreatedUserName" width="120">
          <template slot-scope="scope">
            <div v-if="scope.row.FlowCreatedUser">{{scope.row.FlowCreatedUser.RealName}}</div>
          </template>
        </el-table-column>
        <el-table-column label="流程模板名称" align="center" prop="FlowTemplateName"></el-table-column>
        <el-table-column label="创建时间" align="center" width="160">
          <template slot-scope="scope">
            {{ parseTime(scope.row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-edit" @click="handleView(scope.row)">详情</el-button>
            <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
            <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="total > 0" :total="total" :page.sync="taskForm.pageNum" :limit.sync="taskForm.pageSize"
        @pagination="getList" />
    </div>
    <plane_add ref="planeAdd" @reloadList="reloadList"></plane_add>
  </div>
</template>
<script>
import { devPlaneList,devPlaneRemove } from "@/api/after/devplane";
import plane_add from './devplane_add.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  mixins: [resizeTableCon],
  components: { plane_add },
  data() {
    return {
      showSearch:true,
      statusList:[
        {value:'0',label:'手动发起'},
        {value:'1',label:'定时发起'},
        {value:'2',label:'设备事件'},
      ],
      taskForm: {
        StartWay: '',
        pageNum:1,
        pageSize:10,
      },
      dateRange:[],
      tbList:[],
      loading:false,
      total:0
    };
  },
  mounted(){
    this.getList()
  },
  methods: {
    getExpireNoticeUsersName(row){
      if(row.ExpireNoticeUserList&&row.ExpireNoticeUserList.length>0){
        let nameList=row.ExpireNoticeUserList.map(rs=>rs.RealName)
        return nameList.join(',')
      }else{
        return ''
      }
      
    },
    handleDelete(row){
      //删除任务
      const ids = row.Id;
      this.$modal.confirm('是否确认删除名称为"' + row.Name + '"的设备计划？').then(function() {
        return devPlaneRemove({ id: ids });
      })
      .then(rsp => {
        // console.log("删除返回值", rsp);
        this.getList();
        this.$modal.msgSuccess("删除成功");
      })
      .catch(() => {});
    },
    handleView(row){
      //详情
      this.$refs.planeAdd.openDialog(row.Id,true)
    },
    handleUpdate(row){
      // console.log("任务类型",row);
      this.$refs.planeAdd.openDialog(row.Id)
    },
    reloadList(){
      //重新加载列表
      // this.taskForm.pageNum=1
      // this.tbList=[]
      this.loading=true
      this.getList()
    },
    handleAdd(){
      this.$refs.planeAdd.openDialog()
    },
    resetQuery() {
      this.dateRange = [];
      this.taskForm.StartWay=undefined
      this.resetForm("taskForm");
      this.getList();
    },
    getList(){
      this.loading=true
      console.log()
      devPlaneList(this.addDateRange(this.taskForm, this.dateRange)).then(res=>{
        this.tbList=JSON.parse(JSON.stringify(res.data.List))
        this.total=res.data.Total
        this.loading=false
      }).catch(err=>{
        this.loading=false
      })
    },
    /** 重置按钮操作 */
    resetQuery() {
        this.dateRange = [];
        this.resetForm("taskForm");
        this.getList();
    },
    handleClick(tab, event) {
      console.log(tab, event);
    }
  }
};
</script>