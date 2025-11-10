<template>
    <el-dialog width="960px" title="请选择目标设备" :visible.sync="deviceOpen" append-to-body>
      <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
        <div>
          <el-form-item label="查询关键字" prop="Key">
            <el-input v-model="deviceQuery.Key" placeholder="请输入设备名称、通讯编码或编号" clearable></el-input>
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker class="form_input_style" v-model="devDateRange" style="width: 232px" value-format="yyyy-MM-dd" type="daterange"
              range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
        </div>
        <el-form-item>
          <el-button icon="el-icon-refresh" @click="resetSelectDevice">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="getMyDevList()">搜索</el-button>
        </el-form-item>
      </el-form>
      <el-table ref="devTable" :data="deviceList" tooltip-effect="dark" v-loading="deviceLoading" style="width: 100%"
        @row-click="clickDevRow" :row-key="getRowKeys">
        <!-- <el-table-column type="selection" width="55" :reserve-selection="true"> </el-table-column> -->
        <el-table-column prop="DeviceNumber" label="设备编号" align="center" width="150"></el-table-column>
        <el-table-column prop="DeviceId" label="通讯编码" align="center" width="150"></el-table-column>
        <el-table-column label="预览图片" align="center" width="150">
          <template slot-scope="scope">
            <div class="imgwrap" style="max-width: 60px;max-height:60px;">
              <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="Name" label="设备名称"> </el-table-column>
        <el-table-column prop="ProductName" label="所属产品"></el-table-column>
      </el-table>
      <pagination v-show="deviceTotal > 0" :total="deviceTotal" :page.sync="deviceQuery.pageNum" :limit.sync="deviceQuery.pageSize" @pagination="getMyDevList"/>
    </el-dialog>
</template>
    
<script>
  import { myDeviceList } from "@/api/after/dev";
  export default {
    name: "AdminUiSelectDevicecompt",
    props: {},
    data() {
      return {
        deviceOpen:false,
        devDateRange:[],
        deviceQuery: {
          pageNum: 1,
          pageSize: 10,
          Key: "",
        },
        deviceTotal:0,
        deviceList:[],
        afterSelectDevice:[],
        getRowKeys(row) {
            return row.Id;
        },
        deviceLoading:false,
      };
    },
  
    mounted() {},
  
    methods: {
        async resetSelectDevice(){
            this.deviceQuery.Key = ''
            this.devDateRange = []
            this.deviceQuery.pageNum = 1
            await this.getMyDevList();
        },
        async getMyDevList(){
            try {
                this.deviceLoading=true
                let res=await myDeviceList(this.deviceQuery)
                this.deviceList=res.data.List
                this.deviceTotal=res.data.Total
                this.deviceLoading=false
            } catch (error) {
                this.deviceLoading=false
            }
        },
        async openAddDevice(){
            this.deviceOpen=true
            await this.getMyDevList()
        },
        async clickDevRow(row) {
            this.afterSelectDevice=[row]
            this.deviceOpen=false
            this.$emit('finishSelect',row)
        },
    },
  };
</script>