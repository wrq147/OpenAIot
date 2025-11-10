<template>
  <el-dialog width="900px" title="请选择分配的设备" :visible.sync="deviceOpen" append-to-body>
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
        <el-button type="primary" icon="el-icon-search" @click="getDeviceList(true)">搜索</el-button>
      </el-form-item>
    </el-form>
    <el-table ref="devTable" :data="deviceList" tooltip-effect="dark" v-loading="deviceLoading" style="width: 100%"
      @selection-change="onDeviceChange" @row-click="clickDevRow" @select="devBoxSelect" :row-key="getRowKeys">
      <el-table-column type="selection" width="55" :reserve-selection="true"></el-table-column>
      <el-table-column prop="DeviceNumber" label="设备编号" align="center" width="150"></el-table-column>
      <el-table-column prop="DeviceId" label="通讯编码" align="center" width="150"></el-table-column>
      <el-table-column label="预览图片" align="center" width="150">
        <template slot-scope="scope">
          <div class="imgwrap" style="max-width: 60px; max-height: 60px">
            <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="Name" label="设备名称"> </el-table-column>
      <el-table-column prop="ProductName" label="所属产品"></el-table-column>
    </el-table>
    <pagination v-show="deviceTotal > 0" :total="deviceTotal" :page.sync="deviceQuery.pageNum" :limit.sync="deviceQuery.pageSize" @pagination="getDeviceList(true)"/>
    <div slot="footer" class="dialog-footer">
      <el-button @click="deviceOpen = false">取 消</el-button>
      <el-button type="primary" @click="addPlaneDevice">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { myDeviceList } from "@/api/after/dev";
export default {
  name: "AdminUiSelectDevice",

  data() {
    return {
        getRowKeys(row) {
            return row.Id;
        },
        deviceList: [],
        deviceQuery: {
            pageNum: 1,
            pageSize: 10,
            Key: "",
        },
        deviceOpen: false,
        devDateRange: [],//时间日期
        afterSelectDevice: [],//选择后的设备列表
        deviceLoading: false,
        deviceTotal: 0,
    };
  },

  mounted() {},

  methods: {
    openAddDevice(planeTargetData) {
      //选择设备
      this.deviceOpen = true
      this.$nextTick(()=>{
        let allselArr=JSON.parse(JSON.stringify(planeTargetData))
        let selList=allselArr.filter(row=>row.TargetType==0)
        this.$refs.devTable.clearSelection()
        this.afterSelectDevice=selList.map(row=>{
          let obj={
            Id:row.TargetId,
            Name:row.TargetName,
            PhotoUrl:row.PhotoUrl
          }
          if(obj){
            this.$refs.devTable.toggleRowSelection(obj, true);
          }
          return obj
        })
      })
      this.deviceQuery.pageNum = 1
      this.getDeviceList()
    },
    onDeviceChange(val) {
      this.afterSelectDevice = JSON.parse(JSON.stringify(val))
    },
    devBoxSelect(arr, row) {
      //点击设备选择多选框
      const selected = this.afterSelectDevice.some(
        (item) => item.Id === row.Id
      );
      if (selected) {
        this.afterSelectDevice = this.afterSelectDevice.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    clickDevRow(row) {
      const selected = this.afterSelectDevice.some(
        (item) => item.Id === row.Id
      );
      if (!selected) {
        // 选择
        this.$refs.devTable.toggleRowSelection(row, true);
      } else {
        // 取消
        this.$refs.devTable.toggleRowSelection(row, false);
        this.afterSelectDevice = this.afterSelectDevice.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    resetSelectDevice() {//重置设备选择搜索
      this.deviceQuery.Key = ''
      this.devDateRange = []
      this.deviceQuery.pageNum = 1
      this.getDeviceList();
    },
    getDeviceList() {
      //获取设备列表
      let deviceForm = {};
      this.deviceLoading = true;
      deviceForm = this.addDateRange(this.deviceQuery, this.devDateRange)
      myDeviceList(deviceForm).then((response) => {
          this.deviceList = response.data.List;
          this.deviceTotal = response.data.Total;
          this.deviceLoading = false;
        })
        .catch((err) => {
          this.deviceLoading = false;
        });
    },
    addPlaneDevice() {
      //添加计划的设备
      let sellist=this.afterSelectDevice.map(row=>{
        let obj={
          TargetId:row.Id,
          TargetType:0,
          PhotoUrl:row.PhotoUrl,
          TargetName:row.Name,
        }
        return obj
      })
      this.$emit('addPlaneDevice',sellist)
      this.deviceOpen = false
    },
  },
};
</script>
