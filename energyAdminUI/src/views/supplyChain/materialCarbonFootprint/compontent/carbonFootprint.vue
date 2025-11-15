<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>新增物料碳足迹</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="物料名称" prop="MaterialId">
                <!-- <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.MaterialName" clearable placeholder="请选择物料名称">
                  <el-option label="成品" value="1" />
                  <el-option label="半成品" value="2" />
                </el-select> -->
                <el-select @focus="materialRemoteMethod()" @change="materialValueChange()" :disabled="isOnlyView" style="width:100%" v-model="addForm.MaterialId" clearable placeholder="请选择物料名称" filterable remote reserve-keyword :remote-method="materialRemoteMethod" :loading="optionLoading">
                  <el-option :label="it.MaterialName" :value="it.Id" v-for="(it,ix) in MaterialOption" :key="'Material'+ix"/>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="单位" prop="MaterialUnit">
                <el-input :disabled="true" type="text" v-model="addForm.MaterialUnit" placeholder=""></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="供应商" prop="ProviderId">
                <!-- <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProviderName" placeholder="请选择供应商"></el-input> -->
                <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.ProviderId" clearable placeholder="请选择供应商">
                  <el-option :label="ite.ProviderName" :value="ite.ProviderId" v-for="ite in activeProviderList" :key="'MaterialProvider'+ite.ProviderId"/>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="生命周期边界" prop="ProductBorder">
                <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.ProductBorder" clearable placeholder="请选择生命周期边界">
                  <el-option :label="it.name" :value="it.id" v-for="it in tabList" :key="it.id+it.name"/>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="碳足迹" prop="CarbonEmission">
                <el-input :disabled="isOnlyView" type="number" v-model="addForm.CarbonEmission" placeholder="请输入碳足迹"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="单位" prop="CarbonUnit">
                <el-input :disabled="true" type="text" v-model="addForm.CarbonUnit" placeholder=""></el-input>
              </el-form-item>
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
import {materialPageList,addMaterialCarbon,updateMaterialCarbon,materialInfo} from '@/api/energy/material'
export default {
  name: 'EnergyAdminUIProductAdd',
  props:{
    tabList:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      dialog:false,
      isOnlyView:false,
      addForm:{
        MaterialId:'',
        ProviderId:'',
        MaterialUnit:'',
        ProductBorder:'',
        CarbonEmission:'',
        CarbonUnit:''
      },
      addRules:{
        MaterialId: [{ required: true, trigger: "change", message: "请选择物料" }],
        ProviderId: [{ required: true, trigger: "change", message: "请选择供应商" }],
        ProductBorder: [{ required: true, trigger: "change", message: "请选择生命边界" }],
        CarbonEmission: [{ required: true, trigger: "blur", message: "请输入碳足迹" }],
      },
      saveLoading:false,
      MaterialOption:[],
      MaterialForm:{
        pageNum:1,
        pageSize:20,
        orgId:this.$store.state.user.orgId,
      },
      optionLoading:false,
      activeProviderList:[]
    };
  },

  mounted() {
    
  },

  methods: {
    async loadMaterialInfo(id){
      try{
        let res=await materialInfo(id)
        return res.data
      }catch(err){
        return ''
      }
    },
    async materialValueChange(notclear){
      let activeMaterial=await this.loadMaterialInfo(this.addForm.MaterialId)
      if(activeMaterial){
        this.addForm.MaterialUnit=activeMaterial.MaterialUnit
        this.activeProviderList=activeMaterial.ProviderMaterials
        this.addForm.CarbonUnit='kgCO₂e/'+activeMaterial.MaterialUnit
        if(notclear){}else{
          this.addForm.ProviderId=''
        }
        
      }
    },
    async loadmaterialPageList(){
      let response=await materialPageList(this.MaterialForm);
      this.MaterialOption = JSON.parse(JSON.stringify(response.data.List));
      if(this.optionLoading){
        this.optionLoading=false
      }
    },
    async materialRemoteMethod(query){
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.MaterialForm.materialName=query
          await this.loadmaterialPageList()
          this.optionLoading = false;
        }, 200);
      } else {
        delete this.MaterialForm.materialName
        await this.loadmaterialPageList()
      }
    },
    async openDialog(info,isOnlyView){
      console.log(info,'infoinfo');
      if(info){
        await this.materialRemoteMethod(info.MaterialName)
        this.addForm={
          Id:info.Id,
          MaterialId:info.MaterialId,
          ProviderId:info.ProviderId,
          ProductBorder:info.ProductBorder,
          MaterialUnit:info.MaterialUnit,
          CarbonEmission:info.CarbonEmission,
          CarbonUnit:info.CarbonUnit
        }
        await this.materialValueChange(true)
      }else{
        this.addForm={
          MaterialId:'',
          ProviderId:'',
          ProductBorder:'',
          MaterialUnit:'',
          CarbonEmission:'',
          CarbonUnit:''
        }
      }
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
          this.saveLoading = true;
          if (this.addForm.Id) {
            submitForm.orgId=this.$store.state.user.orgId;
            updateMaterialCarbon(submitForm).then(response => {
              if (response.code == 0) {
                this.$message.success("修改物料碳足迹成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.state.user.orgId;
            addMaterialCarbon(submitForm).then(response => {
              if (response.code == 0) {
                this.$message.success("添加物料碳足迹成功");
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