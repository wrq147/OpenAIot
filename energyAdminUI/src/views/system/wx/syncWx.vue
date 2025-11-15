<template>
  <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="630px" append-to-body>
    <div>
      <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="同步" name="handle">
          <el-form ref="timeSyncForm" :rules="rules" :model="timeSyncForm" label-width="100px">
            <el-row>
              <el-col :span="24">
                <el-form-item label="同步速度" prop="speed" style="margin-left:0px">
                    <el-select v-model="timeSyncForm.speed" placeholder="请选择">
                        <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="同步账号">
                  <el-input style="width:350px;" v-model="timeSyncForm.username" placeholder="请输入企业微信需要同步的字段名称"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <div class="hanlde_btn">
            <el-button type="primary" plain @click="handleMoveSync" :disabled="isSyncing">手动同步</el-button>
            <el-button type="success" plain @click="handleTimeSync(true)" v-if="!isSyncing">开启定时同步</el-button>
            <el-button type="danger" plain @click="handleTimeSync(false)" v-else>停止定时同步</el-button>
          </div>
        </el-tab-pane>
        <el-tab-pane label="同步记录" name="record">
          <div class="from_con" id="from_con" style="padding:0">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建日期" style="margin-left:25px">
                <el-date-picker class="set_radius" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
                  type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con" style="margin-left:20px">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
              <!-- </el-col> -->
            </el-form>
          </div>
          <el-table  border v-loading="loading" :data="asyncTaskList" class="data_table"
            :header-cell-style="cellSty" style="width: 100%" row-key="TaskId">
            <el-table-column prop="AppId" label="企业微信的AppId" align="center"></el-table-column>
            <el-table-column label="状态" align="center" key="Status">
              <template slot-scope="scope">
                <el-tag v-if="scope.row.Status==0">同步中</el-tag>
                <el-tag v-if="scope.row.Status==1">已完成</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="创建时间" align="center" prop="CreatedOn">
              <template slot-scope="scope">
                <span>{{ parseTime(scope.row.CreatedOn) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-tickets" @click="handRowInfo(scope.row)">详情</el-button>
              </template>
            </el-table-column>
          </el-table>
          <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
            :limit.sync="queryParams.pageSize" @pagination="selectSyncList" />
        </el-tab-pane>
      </el-tabs>
    </div>
  </el-dialog>
</template>

<script>
import {corpInfo, wxStartSync,wxStopSync,wxManualSync,corpTaskList} from '@/api/system/sync.js';
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "AdminUiSyncWx",
  mixins: [resizeTableCon],
  data() {
    return {
      options:[{
        value: 1,
        label: '快速'
      },{
        value: 2,
        label: '慢速'
      }],
      open: false,
      title: "企业微信同步",
      timeSyncForm:{
        speed:1,
        username:''
      },
      rules: {
        speed: [{ required: true, message: "同步速度", trigger: "change" }],
      },
      activeName:'handle',
      isSyncing:false,//是否启用定时同步
      appid:'',
      asyncTaskList:[],//同步任务列表
      queryParams:{
        pageNum:1,
        pageSize:10
      },
      dateRange:[],
      total:0,
      loading:false
    };
  },

  mounted() {},

  methods: {
    resetQuery(){
      this.dateRange=[]
      this.handleQuery()
    },
    handleQuery(){
      this.loading=true
      this.queryParams.pageNum=1
      this.selectSyncList()
    },
    handRowInfo(row){
      this.$emit('handleInfo',row)
    },
    async openDialog(AppId){
      try {
        this.appid=AppId
        let res=await corpInfo(AppId)
        this.timeSyncForm.speed=res.data.Speed
        this.timeSyncForm.username=res.data.UserName
        if(res.data.Status=="0"){
          this.isSyncing=true
        }else{
          this.isSyncing=false
        }
        this.open=true
      } catch (error) {
        
      }
    },
    async handleMoveSync(){
        //手动同步
        try {
          let res=await wxManualSync({appid:this.appid,username:this.timeSyncForm.username})
          console.log(res,'res');
          if(res.data){
            this.$emit('handleMoveSync',res.data)
          }
          
        } catch (error) {
          
        }
    },
    handleClick(){
        //切换导航
        if(this.activeName=='record'){
          this.loading=true
          this.queryParams.pageNum=1
          this.selectSyncList()
        }
    },
    selectSyncList(){
      if(this.queryParams.pageNum==1){
        this.asyncTaskList=[]
      }
      corpTaskList(this.addDateRange(this.queryParams, this.dateRange)).then(res=>{
        this.asyncTaskList=res.data.List
        this.total=res.data.Total;
        this.loading=false
      }).catch(err=>{
        this.loading=false
      })
    },
    handleTimeSync(isStart){
        //定时同步
        if(isStart){
          this.$refs["timeSyncForm"].validate(async (valid) => {
            if(valid){
              await this.wxStartSyncFun()
            }
          })
        }else{
          this.wxStopSyncFun()
        }
    },
    async wxStartSyncFun(){
      try {
        let res=await wxStartSync({
          appid:this.appid,
          speed:this.timeSyncForm.speed,
          username:this.timeSyncForm.username
        })
        this.isSyncing=true
        this.$modal.msgSuccess("开启定时同步成功");
      } catch (error) {
        
      }
    },
    wxStopSyncFun(){
      wxStopSync({
        appid:this.appid,
      }).then(res=>{
        this.isSyncing=false
        this.$modal.msgSuccess("停止定时同步成功");
      })
    },
  },
};
</script>
<style lang="less" scoped>
.hanlde_btn{
  text-align: right;
}
::v-deep.el-dialog__body{
  padding-left: 10px;
  padding-right: 10px;
}
</style>