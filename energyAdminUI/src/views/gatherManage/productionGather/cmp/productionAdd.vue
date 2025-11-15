<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ isOnlyView ? '详情' : title }}生产数据</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <div>
            <el-row :gutter="24">
                <el-col :span="12">
                    <el-form-item label="设施名称" prop="FacilityId">
                        <treeselect v-model="addForm.FacilityId" :disabled="title === '编辑' || isOnlyView" :options="facilityList" :show-count="true" :normalizer="normalizer" placeholder="请选择设施名称"/>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="产品名称" prop="ProductId">
                       <el-select :filter-method="productListFilter" v-model="addForm.ProductId" :disabled="title === '编辑' || isOnlyView" filterable style="width:100%" placeholder="请选择产品名称" clearable>
                           <el-option v-for="item in optionsList" :key="item.Id" :label="item.ProductName" :value="item.Id" :ProductModel="item.ProductModel">
                            <div class="product_option_li">
                                <span>{{item.ProductName}}</span>
                                <span class="model">{{item.ProductModel}}</span>
                            </div>
                           </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="统计日期" prop="DDate">
                         <el-date-picker v-model="addForm.DDate" :disabled="title === '编辑' || isOnlyView" type="date" value-format="yyyy-MM-dd" placeholder="请选择日期" style="width:100%" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="产量" prop="OutPut">
                        <el-input type="text" v-model="addForm.OutPut" :disabled="isOnlyView" placeholder="请输入产量" />
                    </el-form-item>
                </el-col>
            </el-row>
        </div>
      </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="dialog = false">取消</el-button>
      <el-button v-if="!isOnlyView" type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import { addProduction, editProduction} from '@/api/energy/gatherManage'
export default {
    name: 'productionAdd',
     components: {
        Treeselect
    },
    props:{
        title:{
            type: String,
            default: '添加'

        },
        facilityList: {
            type: Array,
            default: () => []
        },
        productList: {
            type: Array,
            default: () => []
        },
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            typeTreeList: [],
            isOnlyView: false,
            addRules: {
                FacilityId: [{ required: true, trigger: "change", message: "请选择设施名称" }],
                ProductId: [{ required: true, trigger: "blur", message: "请选择产品名称" }],
                DDate: [{ required: true, trigger: "blur", message: "请选择统计日期" }],
                OutPut: [{ required: true, trigger: "blur", message: "请输入产量" }],

            },
            saveLoading: false,
            optionsList:[]
        };
    },
    watch:{
        productList:{
            handler(newval){
                this.optionsList=JSON.parse(JSON.stringify(newval))
            },
            deep:true,
            immediate:true
        }
    },

    methods: {
        productListFilter(value){
            let productList=JSON.parse(JSON.stringify(this.productList))
            if(value){
                this.optionsList = productList.filter(item => item.ProductName.toLowerCase().indexOf(value.toLowerCase())>-1 || item.ProductModel.toLowerCase().indexOf(value.toLowerCase())>-1);
            }else{
                this.optionsList = JSON.parse(JSON.stringify(productList))
            }
            
        },
        openDialog(info, isOnlyView){
            if(info) {
                this.addForm={
                    OrgId: info.OrgId,
                    Id: info.Id,
                    FacilityId: info.FacilityId,
                    ProductId: info.ProductId,
                    DDate: info.DDate,
                    OutPut: info.OutPut,
                }
            }else{
                this.addForm = {
                    OrgId: this.$store.getters.orgId,
                    FacilityId: null,
                    ProductId: '',
                    DDate: '',
                    OutPut: '',
                }
            }
            if(isOnlyView) {
                this.isOnlyView = isOnlyView
            }else{
                this.isOnlyView = false
            }
            this.dialog = true
        },
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    this.saveLoading = true;
                    if (this.addForm.Id) {
                        this.setEditList();
                    } else {
                        this.setAddList();
                    }
                }
            });
        },
         normalizer(node) {
            if (node.Children == null || !node.Children.length) {
                delete node.Children;
            }
            return {
                id: node.Id,
                label: node.FacilityName,
                children: node.Children,
            };
        },
        // 编辑
        setEditList() {
            editProduction(this.addForm).then(response => {
                this.$message.success("修改成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 新增
        setAddList() {
            addProduction(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        }
    },
};
</script>

<style lang="less" scoped>
.product_option_li{
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    .model{
        font-size: 12px;
        color: rgba(255, 255, 255, 0.6);
        margin-left: 10px;
    }
}
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