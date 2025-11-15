<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>新增物料</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="物料编号" prop="MaterialCode">
              <el-input :disabled="true" type="text" v-model="addForm.MaterialCode" placeholder="请输入物料编号"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="物料名称" prop="MaterialName">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.MaterialName" placeholder="请输入物料名称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="物料类型" prop="MaterialType">
              <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.MaterialType" clearable placeholder="请选择物料类型">
                <el-option :label="it.label" :value="it.value" v-for="it in materType" :key="it.label"/>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="管理单位" prop="MaterialUnit">
              <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.MaterialUnit" clearable placeholder="请选择管理单位">
                <el-option :label="it.label" :value="it.label" v-for="it in unitList" :key="it.value"/>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="10">
            <el-col :span="24">
              <div class="table_title">
                  <div class="title_text">供应商信息</div>
                  <div class="title_add" @click="materialSupplyAdd">
                      <i style="font-size:10px;" class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                      <span style="margin-left:6px">添加供应商</span>
                  </div>
              </div>
              <el-table v-loading="loading" :data="supplyTableData" class="data_table" :header-cell-style="cellSty" style="width:100%">
                  <el-table-column label="序号" align="center" type="index" width="70"/>
                  <el-table-column label="供应商" align="center" key="ProviderName" prop="ProviderName" width="180" :show-overflow-tooltip="true"/>
                  <el-table-column label="生产地址" align="center" key="ProviderAddress" prop="ProviderAddress" :show-overflow-tooltip="true"/>
                  <el-table-column label="供应商规格型号" align="center" key="ProductModel" prop="ProductModel" :show-overflow-tooltip="true"/>
                  <el-table-column label="操作" align="center" width="90" class-name="small-padding fixed-width" fixed="right">
                      <template slot-scope="scope">
                          <el-button class="danger" type="text" @click="handleSupplyDelete(scope.row,scope.$index)">删除</el-button>
                      </template>
                  </el-table-column>
              </el-table>
            </el-col>
        </el-row>
      </div>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="dialog = false">取消</el-button>
      <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
// import {addProduct,updateProduct} from '@/api/energy/product'
import {addMaterial,updateMaterial,materialCode,materialInfo} from '@/api/energy/material'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: 'EnergyAdminUIProductAdd',
  mixins: [resizeTableCon],
  dicts: ["sys_enery_measure"],
  data() {
    return {
      materType:[{label:'原材料',value:'原材料'},{label:'半成品',value:'半成品'},{label:'包装材料',value:'包装材料'},{label:'备品备件',value:'备品备件'},{label:'消耗品',value:'消耗品'}],
      loading:false,
      dialog:false,
      isOnlyView:false,
      supplyTableData:[],//供应商列表
      addForm:{
        MaterialCode:'',
        MaterialName:'',
        MaterialType:'',
        MaterialUnit:'',
        ProviderMaterials:'',
      },
      addRules:{
        MaterialCode: [{ required: true, trigger: "change", message: "请输入物料编号" }],
        MaterialName: [{ required: true, trigger: "change", message: "请输入物料名称" }],
        MaterialType: [{ required: true, trigger: "change", message: "请选择物料类型" }],
        MaterialUnit: [{ required: true, trigger: "blur", message: "请选择管理单位" }],
      },
      saveLoading:false,
      unitList:[],
    };
  },

  mounted() {
    this.unitList=this.dict.type.sys_enery_measure;
  },

  methods: {
    async loadMaterialCode(){//获取供应商编号
      let res=await materialCode()
      return res.data
    },
    finishSupplierAdd(val){
      this.supplyTableData.push(val)
    },
    materialSupplyAdd(){
      this.$emit('addSupply')
      // this.$refs.choiceSupplier.openDialog()
    },
    handleSupplyDelete(row,index){
      let that=this
      this.$modal.confirm('是否确认删除该行数据项？').then(function() {
        that.supplyTableData.splice(index,1)
        that.$modal.msgSuccess("删除成功");
      })
      
    },
    async loadMaterialInfo(id){
      let res=await materialInfo(id)
      return res.data
    },
    async openDialog(row,isOnlyView){
      // console.log(row,'row');
      if(row){
        let info=await this.loadMaterialInfo(row.Id)
        this.addForm={
          Id:info.Id,
          MaterialCode:info.MaterialCode,
          MaterialName:info.MaterialName,
          MaterialType:info.MaterialType,
          MaterialUnit:info.MaterialUnit,
          ProviderMaterials:info.ProviderMaterials,
        }
      }else{
        this.addForm={
          MaterialCode:await this.loadMaterialCode(),
          MaterialName:'',
          MaterialType:'',
          MaterialUnit:'',
          ProviderMaterials:[],
        }
      }
      this.supplyTableData=this.addForm.ProviderMaterials.map(rw=>{
        let rowObj={
          ProviderId:rw.ProviderId,
          ProviderName:rw.ProviderName,
          ProviderAddress:rw.ProviderAddress,
          ProductModel:rw.ProductModel,
        }
        return rowObj
      })
      if(isOnlyView){
        this.isOnlyView=isOnlyView
      }else{
        this.isOnlyView=false
      }
      this.dialog=true
    },
    closeDialog(){
      this.dialog=false
    },
    submitForm(){
      this.$refs["addForm"].validate(valid => {
        if (valid) {
          // let submitTag=[]
          let submitForm = JSON.parse(JSON.stringify(this.addForm))
          // submitForm.MarketTime=this.addForm
          submitForm.ProviderMaterials=this.supplyTableData.map(row=>{
            let rowObj={
              ProviderId:row.ProviderId,
              ProductModel:row.ProductModel,
            }
            return rowObj
          })
          this.saveLoading = true;
          if (this.addForm.Id) {
            submitForm.orgId=this.$store.state.user.orgId;
            updateMaterial(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("修改物料成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.state.user.orgId;
            addMaterial(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("添加物料成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          }
        }
      });
    }
  },
};
</script>

<style lang="less" scoped>
.adddialog{
  ::v-deep .el-dialog__header{
    padding: 0;
    color: #ffffff;
  }
}
.table_title{
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
    line-height: 14px;
    margin-bottom: 14px;
    .title_text{
        color: rgba(255, 255, 255, 0.60);
    }
    .title_add{
      cursor: pointer;
      color: rgba(61, 185, 143, 1);
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