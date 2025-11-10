<template>
  <div>
    <el-dialog title="预警列表" :visible.sync="open" width="700px" append-to-body :close-on-click-modal="false">
      <div>
        <el-row :gutter="10" class="mb8 button_row">
            <div>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openAddWarn">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">添加预警</span>
                </el-button>
              </el-col>
            </div>
          </el-row>
        <el-table border :data="warnData" max-height="300" style="margin-top: 10px;width: 100%;" v-loading="loading"> 
            <el-table-column label="序号" type="index" align="center" :show-overflow-tooltip="true" >
              <template slot-scope="scope">
                {{scope.$index+1}}
              </template>
            </el-table-column>
            <el-table-column label="预警名称" prop="Name" align="center" :show-overflow-tooltip="true" />
            <el-table-column label="通知方式" prop="NoticeWay" align="center" :show-overflow-tooltip="true" >
              <template slot-scope="scope">
                {{returnNoticeWay(scope.row.NoticeWay)}}
              </template>
            </el-table-column>
            <el-table-column label="状态" prop="noticeWay" align="center" :show-overflow-tooltip="true" >
              <template slot-scope="scope">
                  <el-switch
                    v-model="scope.row.Status"
                    active-value="0"
                    inactive-value="1"
                    @change="handleWarnStatus(scope.row)"
                  ></el-switch>
                </template>
            </el-table-column>
            <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-edit" @click="handleView(scope.row)">详情</el-button>
                <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)">删除</el-button>
              </template>
            </el-table-column>
            <!-- <el-table-column v-for="(column, index) in tableData" :key="index" :label="column.label" :prop="column.prop" align="center" :show-overflow-tooltip="true" /> -->
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="tableForm.pageNum" :limit.sync="tableForm.pageSize" :pageSizes="pageSizes" @pagination="getReportWarnList"/>
      </div>
      <!-- <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitForm('click')" :disabled="saveLoading">确 定</el-button>
        <el-button @click="cancel" :disabled="saveLoading">取 消</el-button>
      </div> -->
    </el-dialog>
    <reportWarnAdd ref="reportWarnAdd" :globalData="globalDatas" @getReportWarnList="getReportWarnList"></reportWarnAdd>
  </div>
</template>

<script>
import reportWarnAdd from "@/views/report/datav/datawarn/warningAdd";
import {reportWarnList,deleteReportWarn,editReportWarn } from '@/api/report/warning.js'
export default {
  name: 'AdminUiWarningList',
  components:{
    reportWarnAdd,
  },
  props:{
    // reportId:{
    //     type:String,
    //     default:'',
    // }
    globalData:{
        type:Array,
        default:()=>{
            return []
        }
    }
  },
  data() {
    return {
        pageSizes:[10,20,30,50],
        open:false,
        warnData:[],
        tableData:[],
        total:0,
        tableForm:{
            pageNum:1,
            pageSize:30
        },
        reportId:'',
        globalDatas:[],
        loading:false
    };
  },

  mounted() {
    
  },
  watch:{
    globalData:{
        handler(to){
            this.globalDatas=to
        },
        immediate:true,
        deep:true
    }
  },
  methods: {
    handleWarnStatus(row){
      let text = "";
      if (row.Status == "0") {
        text = "启用";
      } else {
        text = "停用";
      }
      this.$modal.confirm('确认要"' + text + '""' + row.Name + '"预警吗？').then(function() {
          return editReportWarn({ Id: row.Id, Status: row.Status });
        }).then(() => {
          this.$modal.msgSuccess(text + "成功");
        }).catch(function() {
          row.Status = row.Status == "0" ? "1" : "0";
        });
    },
    returnNoticeWay(val){
      //返回通知类型
      let list=[]
      if(val){
        list=val.split(',')
      }
      let nameList=list.map(row=>{
        switch (row) {
          case "APP":
            return "APP站内通知";
          case "EMAIL":
            return "EMAIL邮件通知";
          case "SMS":
            return "SMS短信通知";
          case "WX":
            return "WX微信通知";
        }
      })
      return nameList.join(',')
    },
    handleView(row){
      //详情
      this.$refs.reportWarnAdd.openAddDialog(this.reportId,row.Id,true)
    },
    handleUpdate(row){
      //修改
      this.$refs.reportWarnAdd.openAddDialog(this.reportId,row.Id)
    },
    handleDelete(row){
      //删除
      this.$modal.confirm('是否确认删除告警"' + row.Name + '"？')
        .then(()=>{
          this.loading = true;
          return deleteReportWarn(row.Id);
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.getReportWarnList();
          this.loading = false;
        })
        .catch((err) => {
          console.log("错误",err);
          
          this.loading = false;
        });
    },
    openDialog(reportId){
        //打开数据预警弹层
        this.reportId=reportId
        this.getReportWarnList()
        this.open=true
    },
    getReportWarnList(){
        //获取数据预警列表
        reportWarnList({ReportId:this.reportId}).then(res=>{
            console.log("res",res);
            this.warnData=res.data.List
            this.total=res.data.Total
        })
    },
    openAddWarn(){
        this.$refs.reportWarnAdd.openAddDialog(this.reportId)
    },
  },
};
</script>