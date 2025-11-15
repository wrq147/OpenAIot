<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="560px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ title }}产品生命周期模型</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <el-row :gutter="24">
            <el-col :span="24">
                <el-form-item label="产品名称" prop="ProductId">
                    <el-select v-model="addForm.ProductId" :disabled="isOnlyView" filterable style="width:100%" @change="getModelList" placeholder="请选择产品名称">
                       <el-option v-for="item in productList" :key="item.Id" :label="item.ProductName" :value="item.Id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="数据统计时段" prop="beginDate">
                    <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 100%" value-format="yyyy-MM-dd" type="daterange"
                    range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" @change="getDateRange"></el-date-picker>
                </el-form-item>
                <el-form-item label="产品生命周期模型" prop="ModelId">
                    <el-select v-model="addForm.ModelId" :disabled="isOnlyView" filterable style="width:100%" placeholder="请选择产品名称">
                       <el-option v-for="item in modelList" :key="item.Id" :label="item.ModelName" :value="item.Id" />
                    </el-select>
                </el-form-item>
            </el-col>
        </el-row> 
    </el-form>
    <div slot="footer" class="dialog-footer">
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { productPageList } from '@/api/energy/product';
import { addProductModel } from '@/api/energy/cyclicalModel';
import { modelPageList } from '@/api/energy/cyclicalModel';
export default {
    name: 'modelAdd',
    props:{
        title:{
            type: String,
            default: '新增'
        }
    },
    data() {
        return {
            dialog: false,
            isOnlyView: false,
            createDateRange: [],
            addForm: {},
            productList: [],
            modelList: [],
            addRules: {
                ProductId: [{ required: true, message: '请选择产品名称', trigger: 'change' }],
                ModelId: [{ required: true, message: '请选择产品生命周期模型', trigger: 'change' }],
                beginDate: [{ required: true, message: '请选择数据统计时段', trigger: 'change' }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(){
            this.getInitList();
            this.createDateRange = [];
            this.addForm = {
                OrgId: this.$store.getters.orgId,
                ModelId: '',
                ProductId: '',
                beginDate: '',
                endDate: '',
            }
            this.dialog = true
        },
        // 初始化数据
        getInitList() {
            productPageList({ OrgId: this.$store.getters.orgId,  pageNum: 1,  pageSize: 9999,}).then(res => {
                this.productList = res.data.List;
            })
            this.getModelList()
        },
        // 获取产品生命周期模型
        getModelList(){
            modelPageList({ OrgId: this.$store.getters.orgId, pageNum: 1, pageSize: 9999, productId: this.addForm.ProductId, }).then(res => {
                this.modelList = res.data.List;
            })
        },

        // 关闭弹窗
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    this.saveLoading = true;
                    this.setAddList();
                }
            });
        },
        // 新增
        setAddList() {
            addProductModel(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 日期范围选择
        getDateRange() {
            this.addForm.beginDate = this.createDateRange[0];
            this.addForm.endDate = this.createDateRange[1];
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