<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>新增供应商</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="供应商编号" prop="ProviderCode">
                <el-input :disabled="true" type="text" v-model="addForm.ProviderCode" placeholder="请输入供应商编号"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="供应商名称" prop="ProviderName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProviderName" placeholder="请输入供应商名称"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="联系人" prop="Manager">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.Manager" placeholder="请输入联系人"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="联系电话" prop="Contact">
                <el-input :disabled="isOnlyView" maxlength="11" type="number" v-model="addForm.Contact" placeholder="请输入联系电话"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="所属区域" prop="Area">
                <el-cascader filterable style="width:100%" :disabled="isOnlyView" v-model="addForm.Area" clearable placeholder="请选择所属区域"
                    :props="{ value: 'Id', label: 'Name', children: 'children', checkStrictly: true,emitPath:true }"
                    :options="areaOptions" popper-class="address_popper"></el-cascader>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="详细地址" prop="ProviderAddress">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProviderAddress" placeholder="请输入详细地址"></el-input>
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
import {addProduct,updateProduct} from '@/api/energy/product'
import {addProvider,updateProvider,providerInfo,providerCode} from '@/api/energy/provider'
export default {
  name: 'EnergySupplierAdd',
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
      dialog:false,
      isOnlyView:false,
      addForm:{
        Manager:'',
        ProviderCode:'',
        ProviderName:'',
        Contact:'',
        Area:'',
        ProviderAddress:''
      },
      addRules:{
        Manager: [{ required: true, trigger: "blur", message: "请输入联系人" }],
        ProviderCode: [{ required: true, trigger: "blur", message: "请输入供应商编号" }],
        ProviderName: [{ required: true, trigger: "blur", message: "请输入供应商名称" }],
        Contact: [{ required: true, trigger: "blur", message: "请输入联系电话" }],
        Area: [{ required: true, trigger: "change", message: "请选择所属区域" }],
      },
      saveLoading:false,
    };
  },

  mounted() {
    
  },
  methods: {
    async loadProviderCode(){//获取供应商编号
      let res=await providerCode()
      return res.data
    },
   async openDialog(info,isOnlyView){
      if(info){
        this.addForm={
          Id:info.Id,
          Manager:info.Manager,
          ProviderCode:info.ProviderCode,
          ProviderName:info.ProviderName,
          Contact:info.Contact,
          Area:info.Area?info.Area.split(','):[],
          ProviderAddress:info.ProviderAddress
        }
      }else{
        this.addForm={
          Manager:'',
          ProviderCode:await this.loadProviderCode(),
          ProviderName:'',
          Contact:'',
          Area:'',
          ProviderAddress:''
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
          submitForm.Area=this.addForm.Area.join(',')
          this.saveLoading = true;
          if (this.addForm.Id) {
            updateProvider(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("修改供应商成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.state.user.orgId;
            addProvider(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("添加供应商成功");
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