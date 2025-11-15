<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
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
                <el-form-item label="设备类型" prop="TypeId">
                  <el-select class="set_radius" style="width:140px" v-model="queryParams.TypeId" placeholder="请选择" clearable @change="handleQuery">
                    <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
                  </el-select>
                </el-form-item>
                <el-form-item label="设备状态" prop="equipmentState">
                  <el-select class="set_radius" style="width:140px" v-model="queryParams.equipmentState" placeholder="请选择" clearable @change="handleQuery">
                    <el-option label="正常" value="1" />
                    <el-option label="离线" value="2" />
                  </el-select>
                </el-form-item>
                <el-form-item label="设施绑定状态" prop="thirdState">
                  <el-select class="set_radius" style="width:140px" v-model="queryParams.thirdState" placeholder="请选择" clearable @change="handleQuery">
                    <el-option label="绑定" value="1" />
                    <el-option label="未绑定" value="2" />
                  </el-select>
                </el-form-item>
                <el-form-item label="计费关联" prop="policyState">
                  <el-select class="set_radius" style="width:140px" v-model="queryParams.policyState" placeholder="请选择" clearable @change="handleQuery">
                    <el-option label="已关联" value="1" />
                    <el-option label="未关联" value="2" />
                  </el-select>
                </el-form-item>
              </div>
              <el-form-item class="button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="success" plain @click="handleAdd">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">新增</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="info" :disabled="single" plain @click="handleCorrelation">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left:6px">关联计费标准</span>
                  </el-button>
                </el-col>
              </div>
            </el-row>

            <el-table v-loading="loading" :data="tableData" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="设备编码" align="center" key="EquipmentCode" prop="EquipmentCode" width="180" :show-overflow-tooltip="true"/>
              <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="150" :show-overflow-tooltip="true"/>
              <el-table-column label="第三方编码" align="center" key="ThirdId" prop="ThirdId" width="150" :show-overflow-tooltip="true"/>
              <el-table-column label="设备类型" align="center" key="TypeName" prop="TypeName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="数据单位" align="center" key="Unit" prop="Unit" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="设备状态" align="center" key="EquipmentState" prop="EquipmentState" width="100" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.Online==1?'在线':(scope.row.Online==0?'离线':'未初始化')}}</span>
                </template>
              </el-table-column>
              <el-table-column label="绑定状态" align="center" key="FacilityId" prop="FacilityId" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.FacilityId&&scope.row.FacilityId!='-'&&scope.row.FacilityId>0?'绑定':'未绑定'}}</span>
                </template>
              </el-table-column>
              <el-table-column label="绑定设施" align="center" key="FacilityName" prop="FacilityName" :show-overflow-tooltip="true"/>
              <el-table-column label="碳因子" align="center" key="FactorName" prop="FactorName" :show-overflow-tooltip="true"/>
              <el-table-column label="计费关联" align="center" key="PolicyId" prop="PolicyId">
                <template slot-scope="scope">
                  <span>{{ scope.row.PolicyId&&scope.row.PolicyId!='-'&&scope.row.PolicyId>0?'已关联':'未关联'}}</span>
                </template>
              </el-table-column>
              <el-table-column label="计费标准" align="center" key="PolicyName" width="180" prop="PolicyName" :show-overflow-tooltip="true"/>
              <el-table-column label="数据状态" align="center" key="DataState" width="130" prop="DataState" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.DataState==1?'纳入能源计算':'不纳入能源计算'}}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width" fixed="right">
                <template slot-scope="scope" v-if="scope.row.Id>2">
                  <el-button class="primary" type="text" @click="handleView(scope.row)">详情</el-button>
                  <div class="line"></div>
                  <el-button class="primary" type="text" @click="handleUpdate(scope.row)">修改</el-button>
                  <div class="line"></div>
                  <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
          </div>
        </el-col>
      </el-row>
      <equipAdd ref="equipAdd" @loadList="handleQuery"></equipAdd>
      <policySelect ref="policySelect" @loadList="handleQuery"></policySelect>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import equipAdd from './compontent/equipAdd'
import policySelect from './compontent/policySelect'
import {equipmentPageList,removeEquipment} from '@/api/energy/equip'
import {selectFactorTypeOrg} from "@/api/energy/factorLibrary";

export default {
  name: 'EnergyUIDeviceManagement',
  mixins: [resizeTableCon],
  components:{equipAdd,policySelect},
  data() {
    return {
      tableData:[],
      single:true,
      multiple:true,
      ids:[],
      queryParams:{
        pageNum:1,
        pageSize:10,
      },
      dateRange:[],
      form:{},
      open:false,
      total:0,
      loading:false,
      exportLoading:false,
      OrgEngryList:[],//设备类型即能源类型
      orgId:''
    };
  },

  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.getList()
    this.loadSelectFactorOrg()
  },

  methods: {
    loadSelectFactorOrg(){
      selectFactorTypeOrg({OrgId:this.orgId}).then(res=>{
        this.OrgEngryList=res.data
      })
    },
    handleAdd(){
      //新增
      this.$refs.equipAdd.openDialog()
    },
    getList(){
      this.queryParams.orgId=this.orgId
      equipmentPageList(this.queryParams).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
    },
    handleCorrelation(){
      if(this.ids[0]){
        let findRow=this.tableData.find(row=>row.Id==this.ids[0])
        if(findRow){
          this.$refs.policySelect.openDialog(findRow)
        }
      }
      
    },
    handleView(row){//详情
      this.$refs.equipAdd.openDialog(row,true)
    },
    handleUpdate(row){//修改
      this.$refs.equipAdd.openDialog(row)
    },
    handleDelete(row){//删除
      const Ids = row.Id || this.ids&&this.ids[0];
      this.$modal.confirm('是否确认删除改行数据项？')
        .then(function() {
          return removeEquipment({Id:Ids});
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        Id: undefined,
        UserName: undefined,
        RealName: undefined,
        Password: undefined,
        Sex: "2",
        status: "0",
        Introduction: undefined
      };
      this.resetForm("form");
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