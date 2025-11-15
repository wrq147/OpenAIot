<template>
  <div>
    <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
      <div slot="title" class="dialog_title">
        <div class="dialog_title_left">
          <img src="@/assets/images/zs.png" alt="">
          <span>添加供应商</span>
        </div>
        <div class="dialog_title_right" @click.stop="closeDialog">
          <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
        </div>
      </div>
      <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <div>
            <el-row :gutter="20">
              <el-col :span="24">
                <el-form-item label="供应商" prop="ProviderName">
                  <el-input @focus="openSupplierSelect" :disabled="isOnlyView" type="text" v-model="addForm.ProviderName" placeholder="请输入供应商"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item label="生产地址" prop="ProviderAddress">
                  <el-input :disabled="true" type="text" v-model="addForm.ProviderAddress" placeholder="请输入生产地址"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item label="供应商规格型号" prop="ProductModel">
                  <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProductModel" placeholder="请输入供应商规格型号"></el-input>
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
    <selectSupplier ref="selectSupplier" @finishSupplier="finishSupplier"></selectSupplier>
  </div>
</template>

<script>
import selectSupplier from './selectSupplier'
export default {
  name: 'EnergyAdminUIChoiceSupplier',
  components:{selectSupplier},
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
      addForm:{
        ProviderId:'',
        ProviderName:'',
        ProviderAddress:'',
        ProductModel:'',
      },
      addRules:{
        ProviderName: [{ required: true, trigger: "change", message: "请选择供应商" }],
        ProviderAddress: [{ required: true, trigger: "change", message: "请输入生产地址" }],
        ProductModel: [{ required: true, trigger: "change", message: "请输入供应商规格型号" }],
      },
      isOnlyView:false,
      saveLoading:false,
    };
  },

  mounted() {
    
  },

  methods: {
    openSupplierSelect(){
      this.$nextTick(()=>{
        this.$refs.selectSupplier.openDiage()
      })
    },
    finishSupplier(val){
      console.log("选择结果",val);
      this.addForm.ProviderId=val.Id
      this.addForm.ProviderName=val.ProviderName
      this.addForm.ProviderAddress=val.Province+val.City+val.District+val.ProviderAddress
      // this.addForm.ProviderCode=val.ProviderCode
      this.$forceUpdate()
    },
    openDialog(){
      this.addForm={
        ProviderId:'',
        ProviderName:'',
        ProviderAddress:'',
        ProductModel:'',
      }
      this.dialog=true
    },
    closeDialog(){
      this.dialog=false
    },
    submitForm(){
        this.$emit('finishSupplierAdd',this.addForm)
        this.dialog=false
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