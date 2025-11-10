<template>
  <div>
    <el-table v-loading="loading" :data="tableData" class="data_table" style="width: 100%">
      <!-- <el-table-column type="selection" width="50" align="center" /> -->
      <el-table-column label="代理商ID" align="center" key="Id" prop="Id" />
      <el-table-column label="代理商名称" align="center" key="OrgName" prop="OrgName" :show-overflow-tooltip="true"/>
      <el-table-column label="联系人" align="center" key="ContactName" prop="ContactName" :show-overflow-tooltip="true"/>
      <el-table-column label="邀请方式" align="center" key="PhoneCode" prop="PhoneCode" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span>{{ scope.row.PhoneCode?'短信邀请':'链接邀请' }}</span>
        </template>
      </el-table-column>
      <el-table-column label="联系方式" align="center" key="Tel" prop="Tel" :show-overflow-tooltip="true"/>
      <!-- <el-table-column label="代理级别" align="center" key="GradeName" prop="GradeName" :show-overflow-tooltip="true"/> -->
      <!-- <el-table-column label="上级企业名称" align="center" key="ParentOrgName" prop="ParentOrgName" :show-overflow-tooltip="true"/>
      <el-table-column label="代理区域名称" align="center" key="RegionsName" prop="RegionsName"/> -->

      <el-table-column label="创建时间" align="center" prop="createTime" width="240">
        <template slot-scope="scope">
          <span>{{ parseTime(scope.row.createTime) }}</span>
        </template>
      </el-table-column>

      <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
        <template slot-scope="scope">
          <el-button type="text" icon="el-icon-finished" @click="handleAgain(scope.row)" >再次邀请</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="loadyqRecord"/>
  </div>
</template>

<script>
import { yqRecord } from "@/api/manufac/agentMansge";
import dayjs from "dayjs";
export default {
  name: "AdminUiInviteRecord",
  data() {
    return {
      queryParams: {
        pageNum: 1,
        pageSize: 10,
      },
      tableData: [], //记录数据
      activeType:0,//邀请来源：0不过滤，1为生产商代理的邀请，2为CRM的邀请客户
      total: 0,
      loading:false,
    };
  },

  mounted() {
    
  },

  methods: {
    timeLimit(time){
      return dayjs().isBefore(dayjs(time).add(7, 'day'))
      
    },
    handleAgain(row){
      //再次邀请
      this.$emit('reloadInvite',row)
      
    },
    firstLoad(type){
        if(type){
            this.activeType=type
            this.queryParams.YQFrom=type
            this.queryParams.pageNum=1
            this.loadyqRecord()
        }
    },
    setParams(dateRange) {
        if(dateRange&&dateRange.length==2){
            this.queryParams=this.addDateRange(this.queryParams, dateRange)
        }
        this.queryParams.pageNum=1
        this.loadyqRecord()
    },
    loadyqRecord() {
        this.loading = true;
        yqRecord(this.queryParams).then(res=>{
            console.log("邀请记录",res);
            this.tableData=res.data.List
            this.total = res.data.Total;
            this.loading = false;
        })
    },
  },
};
</script>

<style scoped>
</style>