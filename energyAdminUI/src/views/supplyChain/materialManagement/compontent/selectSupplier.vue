<template>
  <div>
    <el-dialog :visible.sync="supplierDiage" class="adddialog" :show-close="false" width="1000px" append-to-body :destroy-on-close="true">
      <div slot="title" class="dialog_title">
        <div class="dialog_title_left">
          <img src="@/assets/images/zs.png" alt="">
          <span>选择供应商</span>
        </div>
        <div class="dialog_title_right" @click.stop="cancel">
          <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
        </div>
      </div>
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
      <el-table v-loading="loading" :data="tableData" class="data_table" :header-cell-style="cellSty" style="width:100%" highlight-current-row @current-change="onSupplierChange" >
          <!-- <el-table-column type="selection" width="50" align="center" /> -->
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
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
    </el-dialog>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {providerPageList} from '@/api/energy/provider'
export default {
  name: 'EnergyAdminUISelectSupplier',
  mixins: [resizeTableCon],
  props:{
    areaOptions:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      supplierDiage:false,
      loading:'',
      tableData:[],
      total:0,
      queryParams:{
        pageNum:1,
        pageSize:10
      },
      dateRange:[],
      alChooseArea:[]
    };
  },

  mounted() {
  },

  methods: {
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
    cancel(){
      this.supplierDiage=false
    },
    openDiage(){
      this.getList()
      this.supplierDiage=true
    },
    onSupplierChange(val) {
      this.$emit('finishSupplier', val)
      this.cancel()
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
  },
};
</script>

<style lang="less" scoped>
.adddialog{
  ::v-deep .el-dialog__header{
    padding: 0;
    color: #ffffff;
  }
  ::v-deep .el-dialog__body{
    padding-top: 0;
  }
}
.dialog_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  .dialog_title_left{
    font-size: 16px;
    color: #ffffff;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    padding-left: 20px;
    line-height: 16px;
    height: 56px;
    .img{
      width: 16px;
      height: 16px;
    }
    span{
      margin-left: 6px;
    }
  }
  .dialog_title_right{
    margin-right: 20px;
    cursor: pointer;
    i.zhongtaiiconfont{
      color: rgba(255, 255, 255, 0.60);
      font-size: 12px;
    }
  }
}
</style>