<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>新增产品</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
      <div>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="产品型号" prop="ProductModel">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProductModel" placeholder="请输入产品型号"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="产品名称" prop="ProductName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.ProductName" placeholder="请输入产品名称"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="产品简写或缩写" prop="ShortName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.ShortName" placeholder="请输入产品简写或缩写"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="品牌" prop="BrandName">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.BrandName" placeholder="请输入品牌"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="产品类型" prop="ProductType">
                <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.ProductType" clearable placeholder="请选择产品类型">
                  <el-option label="成品" value="1" />
                  <el-option label="半成品" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="零售单价(元)" prop="ProductPrice">
                <el-input :disabled="isOnlyView" type="number" v-model="addForm.ProductPrice" placeholder="请输入零售单价"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="单位" prop="Unit">
                <el-input :disabled="isOnlyView" type="text" v-model="addForm.Unit" placeholder="请输入单位"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="是否上市" prop="OnMarket">
                <el-select :disabled="isOnlyView" style="width:100%" v-model="addForm.OnMarket" clearable placeholder="请选择是否上市">
                  <el-option label="是" value="是" />
                  <el-option label="否" value="否" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12" v-if="addForm.OnMarket=='是'">
              <el-form-item label="上市日期" prop="MarketTime">
                <el-date-picker :disabled="isOnlyView" style="width:100%" v-model="addForm.MarketTime" value-format="yyyy-MM-dd" type="date" placeholder="选择上市日期"></el-date-picker>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="产品说明" prop="Memo">
                <el-input :disabled="isOnlyView" type="textarea" :rows="4" v-model="addForm.Memo" placeholder="请输入产品说明" maxlength="500"></el-input>
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
export default {
  name: 'EnergyAdminUIProductAdd',

  data() {
    return {
      dialog:false,
      isOnlyView:false,
      addForm:{
        ProductModel:'',
        ProductName:'',
        ShortName:'',
        BrandName:'',
        ProductType:'',
        ProductPrice:0,
        Unit:'',
        OnMarket:'否',
        MarketTime:'',
        Memo:'',
      },
      addRules:{
        ProductType: [{ required: true, trigger: "change", message: "请选择产品类型" }],
        OnMarket: [{ required: true, trigger: "change", message: "请选择是否上市" }],
        MarketTime: [{ required: true, trigger: "change", message: "请选择上市时间" }],
        ProductModel: [{ required: true, trigger: "blur", message: "请输入产品型号" }],
        ProductName: [{ required: true, trigger: "blur", message: "请输入产品名称" }],
        BrandName: [{ required: true, trigger: "blur", message: "请输入品牌" }],
        ProductPrice: [{ required: true, trigger: "blur", message: "请输入零售单价" }],
        Unit: [{ required: true, trigger: "blur", message: "请输入单位" }],
      },
      saveLoading:false,
    };
  },

  mounted() {
    
  },

  methods: {
    openDialog(info,isOnlyView){
      if(info){
        this.addForm={
          Id:info.Id,
          ProductModel:info.ProductModel,
          ProductName:info.ProductName,
          ShortName:info.ShortName,
          BrandName:info.BrandName,
          ProductType:info.ProductType,
          ProductPrice:info.ProductPrice,
          Unit:info.Unit,
          OnMarket:info.OnMarket,
          MarketTime:info.MarketTime,
          Memo:info.Memo,
        }
      }else{
        this.addForm={
          ProductModel:'',
          ProductName:'',
          ShortName:'',
          BrandName:'',
          ProductType:'',
          ProductPrice:0,
          Unit:'',
          OnMarket:'否',
          MarketTime:'',
          Memo:'',
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
            // submitForm.orgId=this.$store.state.user.orgId;
            updateProduct(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("修改产品成功");
                this.dialog = false;
                this.$emit('loadList')
                this.saveLoading = false;
              }
            }).catch(err => {
              this.saveLoading = false;
            });
          } else {
            submitForm.orgId=this.$store.state.user.orgId;
            addProduct(submitForm).then(response => {

              if (response.code == 0) {
                this.$message.success("添加产品成功");
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