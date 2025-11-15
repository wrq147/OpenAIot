<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
              <div class="biaodan_input_con">
                <el-form-item label="产品名称" prop="ProductName">
                  <el-input class="set_radius" style="width:200px" v-model="queryParams.ProductName" placeholder="产品名称查询" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="产品简称" prop="ShortName">
                  <el-input class="set_radius" style="width:200px" v-model="queryParams.ShortName" placeholder="产品简称查询" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="产品型号" prop="ProductModel">
                  <el-input class="set_radius" style="width:200px" v-model="queryParams.ProductModel" placeholder="产品型号查询" clearable @keyup.enter.native="handleQuery"/>
                </el-form-item>
                <el-form-item label="状态" prop="ProductType">
                  <el-select class="set_radius" v-model="queryParams.ProductType" placeholder="产品类型" clearable @change="handleQuery">
                    <el-option label="成品" value="1" />
                    <el-option label="半成品" value="2" />
                  </el-select>
                </el-form-item>
                <el-form-item label="品牌" prop="BrandName">
                  <el-input class="set_radius" style="width:140px" v-model="queryParams.BrandName" placeholder="品牌关键词查询" clearable @keyup.enter.native="handleQuery"/>
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
                  <el-button type="info" plain :disabled="single" @click="handleDelete">
                    <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    <span style="margin-left:6px">删除</span>
                  </el-button>
                </el-col>
              </div>
            </el-row>

            <el-table v-loading="loading" :data="tableData" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="产品型号" align="center" key="ProductModel" prop="ProductModel" width="180" :show-overflow-tooltip="true"/>
              <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" :show-overflow-tooltip="true"/>
              <el-table-column label="产品简称或缩写" align="center" key="ShortName" prop="ShortName" width="180" :show-overflow-tooltip="true"/>
              <el-table-column label="品牌" align="center" key="BrandName" prop="BrandName" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="产品类型" align="center" key="ProductType" prop="ProductType" width="150" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.ProductType==1?'成品':'半成品'}}</span>
                </template>
              </el-table-column>
              <el-table-column label="零售单价（元）" align="center" key="ProductPrice" prop="ProductPrice" :show-overflow-tooltip="true"/>
              <el-table-column label="单位" align="center" key="Unit" prop="Unit" :show-overflow-tooltip="true"/>
              <el-table-column label="是否上市" align="center" key="OnMarket" prop="OnMarket" />
              <el-table-column label="上市时间" align="center" prop="MarketTime" width="180" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.MarketTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="产品说明" align="center" key="Memo" prop="Memo" :show-overflow-tooltip="true"/>
              <el-table-column label="创建时间" align="center" prop="createTime" width="180" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作人" align="center" key="createName" prop="createName" :show-overflow-tooltip="true"/>
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
      <productAdd ref="productAdd" @loadList="handleQuery" />
      <uploadFild ref="uploadFild" @getList="handleQuery" @importTemplate="importTemplate" />
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import productAdd from './compontent/productAdd';
import { productPageList, removeProduct, importProductTemplate, exportProductList } from '@/api/energy/product';
import uploadFild from '@/components/uploadFild/index.vue';
export default {
  name: 'EnergyAdminUIproductLibrary',
  mixins: [resizeTableCon],
  components:{ productAdd, uploadFild },
  data() {
    return {
      tableData: [],
      single: true,
      multiple: true,
      ids: [],
      queryParams: {
        ProductName: '',
        BrandName: '',
        ProductType: '',
        pageNum:1,
        pageSize:10,
      },
      dateRange: [],
      form: {},
      open: false,
      total: 0,
      loading: false,
      exportLoading: false,
    };
  },

  mounted() {
    this.getList()
  },

  methods: {
    handleAdd(){
      //新增
      this.$refs.productAdd.openDialog()
    },
    getList(){
      this.queryParams.orgId=this.$store.state.user.orgId;
      productPageList(this.addDateRange(this.queryParams, this.dateRange)).then(res=>{
        this.total=res.data.Total
        this.tableData=res.data.List
      })
    },
    handleImport(){//导入
      this.$refs.uploadFild.openDialog('EfficiencyService/Common/ImportProduct');
    },
    handleExport(){//导出
      exportProductList({ 
        OrgId: this.$store.state.user.orgId,
        ProductName: this.queryParams.ProductName,
        BrandName: this.queryParams.BrandName,
        ProductType: this.queryParams.ProductType
      });
    },
    // 导入能源数据模版
    importTemplate() {
        importProductTemplate();
    },
    handleView(row){//详情
      this.$refs.productAdd.openDialog(row,true)
    },
    handleUpdate(row){//修改
      this.$refs.productAdd.openDialog(row)
    },
    handleDelete(row){//删除
      const Ids = row.Id || this.ids&&this.ids[0];
      this.$modal.confirm('是否确认删除该行数据项？')
        .then(function() {
          return removeProduct({Id:Ids});
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