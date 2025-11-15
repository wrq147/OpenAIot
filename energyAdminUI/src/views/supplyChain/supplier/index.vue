<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
              <div class="biaodan_input_con">
                <el-form-item label="供应商名称" prop="providerName">
                  <el-input class="set_radius" style="width:140px" v-model="queryParams.providerName" placeholder="供应商名称" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="供应商编号" prop="providerCode">
                  <el-input class="set_radius" style="width:140px" v-model="queryParams.providerCode" placeholder="供应商编号" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="联系人" prop="manager">
                  <el-input class="set_radius" style="width:140px" v-model="queryParams.manager" placeholder="联系人名称" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="所属区域" prop="alChooseArea">
                  <el-cascader filterable v-model="alChooseArea" clearable placeholder="请选择省市区"
                    :props="{ value: 'Id', label: 'Name', children: 'children', checkStrictly: true,emitPath:true }"
                    :options="areaOptions" style="width:140px;" popper-class="address_popper"></el-cascader>
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
                <el-col :span="1.5">
                  <el-button type="info" plain @click="handleImport">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left:6px">导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="info" plain :loading="exportLoading" @click="handleExport">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="info" plain :disabled="multiple" @click="handleMultipleDelete">
                    <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    <span style="margin-left:6px">删除</span>
                  </el-button>
                </el-col>
              </div>
              <el-col :span="1.5">
                <el-button type="info" plain :loading="exportLoading" @click="onImportTemplate">
                  <i class="zhongtaiiconfont zhongtai-icon-xiazaidaorumoban"></i>
                  <span style="margin-left:6px">导入模板</span>
                </el-button>
              </el-col>
            </el-row>

            <el-table v-loading="loading" :data="tableData" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="序号" align="center" type="index" width="50"/>
              <el-table-column label="供应商编号" align="center" key="ProviderCode" prop="ProviderCode" width="180" :show-overflow-tooltip="true"/>
              <el-table-column label="供应商名称" align="center" key="ProviderName" prop="ProviderName" :show-overflow-tooltip="true"/>
              <el-table-column label="联系人" align="center" key="Manager" prop="Manager" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="联系电话" align="center" key="Contact" prop="Contact" width="130" :show-overflow-tooltip="true"/>
              <el-table-column label="所属区域" align="center" key="Area" prop="Area" width="150" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ retrunAreaText(scope.row.Area)}}</span>
                </template>
              </el-table-column>
              <el-table-column label="详细地址" align="center" key="ProviderAddress" prop="ProviderAddress" :show-overflow-tooltip="true"/>
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
      <supplierAdd :areaOptions="areaOptions" ref="supplierAdd" @loadList="handleQuery"></supplierAdd>
      <uploadFild ref="uploadFild" @getList="handleQuery" @importTemplate="onImportTemplate" />
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import supplierAdd from './cmp/supplierAdd'
import {providerPageList,removeProvider,exportProviderModel,exportProviderList} from '@/api/energy/provider'
import uploadFild from '@/components/uploadFild/index.vue';
export default {
  name: 'materialManagement',
  mixins: [resizeTableCon],
  components:{supplierAdd,uploadFild},
  data() {
    return {
      alChooseArea:[],
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
      areaOptions:[],//区域
    };
  },
  
  mounted() {
    this.getList()
    this.getDatas()
  },
  methods: {
    onImportTemplate() {//导出模板
      exportProviderModel({});
    },
    handleImport() {// 导入供应商数据
        this.$refs.uploadFild.openDialog('EfficiencyService/Production/ImportProvider');
    },
    handleExport(){//导出供应商数据
      exportProviderList({OrgId:this.$store.state.user.orgId})
    },
    retrunAreaText(val){
      if(val){
          let valStr=val.split(',')
          if(valStr&&valStr.length>0){
              return this.loadAllArea(valStr[valStr.length-1],'')
          }else{
              return ''
          }
      }else{
          return ''
      }
  },
    loadAllArea(val,str){
      if(this.tableData&&this.tableData.length&&val){
        let areaAllData=this.$store.state.datas.areaAllData
        if(areaAllData&&areaAllData.length>0){
          let areaFind=areaAllData.find(row=>row.Id==val)
          if(areaFind&&areaFind.ParentId&&areaFind.ParentId!='100000'){
            str=this.loadAllArea(areaFind.ParentId,str)+areaFind.Name
          }else{
            if(areaFind){
              str=areaFind.Name
            }
          }
          return str
        }else{
          return ''
        }
      }else{
        return ''
      }
      
    },
    getDatas() {
      //获取地址列表
      this.$store.dispatch("datas/areaTree").then(area => {
        // console.log("地址", area);
        this.areaOptions = area;
      });
    },
    handleAdd(){
      //新增
      this.$refs.supplierAdd.openDialog()
    },
    getList(){
      this.queryParams.orgId=this.$store.state.user.orgId;
      if(this.alChooseArea&&this.alChooseArea.length>0){
        this.queryParams.area=this.alChooseArea[this.alChooseArea.length-1]
      }else{
        delete this.queryParams.area
      }
      providerPageList(this.addDateRange(this.queryParams, this.dateRange)).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
    },
    
    handleView(row){//详情
      this.$refs.supplierAdd.openDialog(row,true)
    },
    handleUpdate(row){//修改
      this.$refs.supplierAdd.openDialog(row)
    },
    handleDelete(row){//删除
      const Ids = row.Id// || this.ids&&this.ids[0];
      this.$modal.confirm('是否确认删除该行数据项？')
        .then(function() {
          return removeProvider({Id:Ids});
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    handleMultipleDelete(row){//删除
      const Ids = this.ids
      this.$modal.confirm('是否确认删除所有选中的数据项？')
        .then(async function() {
          return await Promise.all(Ids.map(async row=>{
            await removeProvider({Id:row})
          })
          )
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