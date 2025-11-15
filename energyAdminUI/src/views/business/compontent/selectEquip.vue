<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="1000px">
    <el-row :gutter="20">
      <!--用户数据-->
      <el-col :span="24" :xs="24">
        <div class="from_con" id="from_con">
          <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
            <div class="biaodan_input_con">
              <el-form-item label="设备编码" prop="equipmentCode">
                <el-input class="set_radius" style="width:140px" v-model="queryParams.equipmentCode" placeholder="设备编码查询" clearable @keyup.enter.native="handleQuery"/>
              </el-form-item>
              <el-form-item label="设备名称" prop="equipmentName">
                <el-input class="set_radius" style="width:140px" v-model="queryParams.equipmentName" placeholder="设备名称查询" clearable @keyup.enter.native="handleQuery"/>
              </el-form-item>
            </div>
            <el-form-item class="button_con">
              <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
              <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            </el-form-item>
          </el-form>
        </div>
        <el-table v-loading="loading" :data="tableData" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
            <el-table-column type="selection" width="50" align="center" />
            <el-table-column label="设备编码" align="center" key="EquipmentCode" prop="EquipmentCode" width="180" :show-overflow-tooltip="true"/>
            <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="150" :show-overflow-tooltip="true"/>
            <el-table-column label="设备类型" align="center" key="EnergyType" prop="EnergyType" width="120" :show-overflow-tooltip="true"/>
            <el-table-column label="数据单位" align="center" key="Unit" prop="Unit" width="100" :show-overflow-tooltip="true"/>
            <el-table-column label="设备状态" align="center" key="EquipmentState" prop="EquipmentState" width="100" :show-overflow-tooltip="true">
              <template slot-scope="scope">
                <span>{{ scope.row.EquipmentState==1?'正常':'离线'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="计费标准" align="center" key="PolicyName" width="180" prop="PolicyName" :show-overflow-tooltip="true"/>
            <el-table-column label="数据状态" align="center" key="DataState" width="130" prop="DataState" :show-overflow-tooltip="true">
              <template slot-scope="scope">
                <span>{{ scope.row.DataState==1?'纳入能源计算':'不纳入能源计算'}}</span>
              </template>
            </el-table-column>
          </el-table>

          <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
      </el-col>
    </el-row>
    <div slot="footer" class="dialog-footer">
      <el-button @click="dialog = false">取消</el-button>
      <el-button type="primary" @click="finishChoice">确定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {equipmentPageList} from '@/api/energy/equip'
export default {
  name: 'EnergyUIselectEquip',
  mixins: [resizeTableCon],
  data() {
    return {
      dialog:false,
      tableData:[],
      single:true,
      multiple:true,
      ids:[],
      queryParams:{
        pageNum:1,
        pageSize:10,
        FacilityState:2
      },
      dateRange:[],
      form:{},
      total:0,
      loading:false,
      exportLoading:false,
    };
  },

  mounted() {
    
  },

  methods: {
    openSelect(){
      this.dialog=true
      this.getList()
    },
    finishChoice(){
      this.dialog=false
      this.$emit('finishChoice',this.ids)
    },
    getList(){
      equipmentPageList(this.queryParams).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
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
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
  },
};
</script>

<style lang="scss" scoped>
</style>