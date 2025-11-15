<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
              <div class="biaodan_input_con">
                <el-form-item label="能源类型" prop="energyType">
                  <el-select style="width:140px" class="set_radius" v-model="queryParams.energyType" placeholder="能源类型" clearable @change="handleQuery">
                    <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
                  </el-select>
                </el-form-item>
                <el-form-item label="计费标准名称" prop="policyName">
                  <el-input class="set_radius" style="width:200px" v-model="queryParams.policyName" placeholder="计费标准名称查询" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <!-- <el-form-item label="创建时间">
                  <el-date-picker class="set_radius" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                </el-form-item> -->
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
              </div>
            </el-row>

            <el-table v-loading="loading" :data="tableData" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="能源类型" align="center" key="TypeName" prop="TypeName" width="180" :show-overflow-tooltip="true"/>
              <el-table-column label="计费标准名称" align="center" key="PolicyName" prop="PolicyName" :show-overflow-tooltip="true"/>
              <el-table-column label="计量单位" align="center" key="Unit" prop="Unit" :show-overflow-tooltip="true"/>
              <el-table-column label="创建时间" align="center" prop="createTime" width="180" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="最近修改时间" align="center" prop="updateTime" width="180" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.updateTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width" fixed="right">
                <template slot-scope="scope">
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
      <freight-add ref="freightAdd" @loadList="handleQuery" :OrgEngryList="OrgEngryList"></freight-add>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import FreightAdd from './compontent/freightAdd.vue'
import {policyPageList,removePolicy} from '@/api/energy/Policy'
import {selectFactorTypeOrg} from "@/api/energy/factorLibrary";
export default {
  name: 'EnergyAdminUIfreightBasis',
  mixins: [resizeTableCon],
  components:{FreightAdd},
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
      OrgEngryList:[],
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
      this.$refs.freightAdd.openDialog()
    },
    getList(){
      if(this.queryParams.energyType){}else{
        delete this.queryParams.energyType
      }
      if(this.queryParams.policyName){}else{
        delete this.queryParams.policyName
      }
      this.queryParams.orgId=this.orgId
      policyPageList(this.addDateRange(this.queryParams, this.dateRange)).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
    },
    handleView(row){//详情
      this.$refs.freightAdd.openDialog(row,true)
    },
    handleUpdate(row){//修改
      this.$refs.freightAdd.openDialog(row)
    },
    handleDelete(row){//删除
      const Ids = row.Id || this.ids&&this.ids[0];
      this.$modal.confirm('是否确认删除该行数据项？')
        .then(function() {
          return removePolicy({Id:Ids});
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