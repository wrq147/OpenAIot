<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
              <div class="biaodan_input_con">
                <el-form-item label="物料名称" prop="materialName">
                  <el-input class="set_radius" style="width:200px" v-model="queryParams.materialName" placeholder="请输入物料名称" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="物料类型" prop="materialType">
                  <el-select class="set_radius" v-model="queryParams.materialType" placeholder="请选择" clearable @change="handleQuery">
                    <el-option :label="it.label" :value="it.value" v-for="it in materType" :key="it.label"/>
                  </el-select>
                </el-form-item>
                <el-form-item label="供应商" prop="providerId">
                  <el-select @focus="providerRemoteMethod()" @change="handleQuery" style="width:100%" v-model="queryParams.providerId" clearable placeholder="请选择供应商" filterable remote reserve-keyword :remote-method="providerRemoteMethod" :loading="optionLoading">
                    <el-option :label="it.ProviderName" :value="it.Id" v-for="(it,ix) in providerOption" :key="'provider'+ix"/>
                  </el-select>
                </el-form-item>
                <el-form-item label="供应商规格型号" prop="productModel">
                  <el-input class="set_radius" style="width:140px" v-model="queryParams.productModel" placeholder="请输入供应商规格型号" clearable @keyup.enter.native="handleQuery"/>
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
              <el-table-column label="物料名称" align="center" key="MaterialName" prop="MaterialName" width="140" :show-overflow-tooltip="true"/>
              <el-table-column label="物料类型" align="center" key="MaterialType" prop="MaterialType" width="120" :show-overflow-tooltip="true"></el-table-column>
              <el-table-column label="供应商" align="center" key="ProviderName" prop="ProviderName" width="160" :show-overflow-tooltip="true"/>
              <el-table-column label="供应商规格型号" align="center" key="ProductModel" prop="ProductModel" width="160" :show-overflow-tooltip="true"/>
              <el-table-column label="管理单位" align="center" key="MaterialUnit" prop="MaterialUnit" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="生产地址" align="center" key="ProviderAddress" prop="ProviderAddress" :show-overflow-tooltip="true"/>
              <el-table-column label="生命周期边界" align="center" key="ProductBorder" prop="ProductBorder" :show-overflow-tooltip="true">
                <template slot-scope="scope" v-if="scope.row.Id>2">
                  <span>{{returnProductBorder(scope.row.ProductBorder)}}</span>
                </template>
              </el-table-column>
              <el-table-column label="碳足迹" align="center" key="CarbonEmission" prop="CarbonEmission" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="碳排放单位" align="center" key="CarbonUnit" prop="CarbonUnit" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="操作" align="center" width="188" class-name="small-padding fixed-width" fixed="right">
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
      <carbonFootprint ref="carbonFootprint" @loadList="handleQuery" :tabList="tabList"></carbonFootprint>
      <uploadFild ref="uploadFild" @getList="handleQuery" @importTemplate="onImportTemplate" />
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import carbonFootprint from './compontent/carbonFootprint'
import {materialCarbonPageList,removeMaterialCarbon,exportMaterialCarbonModel,exportMaterialCarbonList} from '@/api/energy/material'
import {providerPageList} from '@/api/energy/provider'
import uploadFild from '@/components/uploadFild/index.vue';
export default {
  name: 'materialCarbonFootprint',
  mixins: [resizeTableCon],
  components:{carbonFootprint,uploadFild},
  data() {
    return {
      materType:[{label:'原材料',value:'原材料'},{label:'半成品',value:'半成品'},{label:'包装材料',value:'包装材料'},{label:'备品备件',value:'备品备件'},{label:'消耗品',value:'消耗品'}],
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
      tabList: [
        {
            id: '1',
            name: '从摇篮到大门',
            tip: '从资源开采到产品出厂'
        },
        {
            id: '2',
            name: '从大门到大门',
            tip: '从产品制造到产品出厂'
        },
        {
            id: '3',
            name: '从摇篮到坟墓',
            tip: '从资源开采到产品废弃'
        },
      ],
      providerOption:[],
      providerForm:{
        pageNum:1,
        pageSize:20,
        orgId:this.$store.state.user.orgId,
      },
      optionLoading:false
    };
  },
  mounted() {
    this.getList()
  },

  methods: {
    returnProductBorder(val){
      if(val){
        let findRow=this.tabList.find(row=>row.id==val)
        return findRow.name
      }else{
        return ''
      }
    },
    onImportTemplate() {//导出物料碳足迹模板
      exportMaterialCarbonModel({});
    },
    handleImport() {// 导入物料碳足迹数据
        this.$refs.uploadFild.openDialog('EfficiencyService/Production/ImportMaterialCarbon');
    },
    handleExport(){//导出物料碳足迹数据
      exportMaterialCarbonList({OrgId:this.$store.state.user.orgId})
    },
    async loadproviderPageList(){
      let response=await providerPageList(this.providerForm);
      this.providerOption = JSON.parse(JSON.stringify(response.data.List));
      if(this.optionLoading){
        this.optionLoading=false
      }
    },
    async providerRemoteMethod(query){
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.providerForm.providerName=query
          await this.loadproviderPageList()
          this.optionLoading = false;
        }, 200);
      } else {
        delete this.providerForm.providerName
        await this.loadproviderPageList()
      }
    },
    handleAdd(){
      //新增
      this.$refs.carbonFootprint.openDialog()
    },
    getList(){
      this.queryParams.orgId=this.$store.state.user.orgId;
      materialCarbonPageList(this.addDateRange(this.queryParams, this.dateRange)).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
    },
    handleView(row){//详情
      this.$refs.carbonFootprint.openDialog(row,true)
    },
    handleUpdate(row){//修改
      this.$refs.carbonFootprint.openDialog(row)
    },
    handleDelete(row){//删除
      const Ids = row.Id || this.ids&&this.ids[0];
      this.$modal.confirm('是否确认删除该行数据项？')
        .then(function() {
          return removeMaterialCarbon({Id:Ids});
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    handleMultipleDelete(row){//多项删除
      const Ids = this.ids
      this.$modal.confirm('是否确认删除所有选中的数据项？')
        .then(async function() {
          return await Promise.all(Ids.map(async row=>{
            await removeMaterialCarbon({Id:row})
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