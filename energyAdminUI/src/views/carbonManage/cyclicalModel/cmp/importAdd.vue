<template>
    <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="1000px" top="2vh" :close-on-click-modal="false" append-to-body>
        <div slot="title" class="dialog_title">
            <div class="dialog_title_left">
                <img src="@/assets/images/zs.png" alt="">
                <span>添加原材料</span>
            </div>
            <div class="dialog_title_right" @click.stop="closeDialog">
                <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
            </div>
        </div>
        <div>
            <div class="tab-list">
                <span v-if="tags.length > 0">已选 {{ tags.length }} 条</span>
                <el-tag v-for="(tag, index) in tags" :key="index" closable effect="dark" type="success" @close="removeTag(index)"> {{tag.MaterialName}} </el-tag>
            </div>
            <el-table ref="materialTable" :data="materialList" @selection-change="handleSelectionChange" row-key="Id" class="data_table" style="width:100%">
                <el-table-column type="selection" width="50" align="center" />
                <el-table-column label="序号" align="center" type="index" width="50" />
                <el-table-column label="物料名称" align="center" prop="MaterialName" />
                <el-table-column label="物料类型" align="center" prop="MaterialType" />
                <el-table-column label="供应商" align="center" prop="ProviderName" />
                <el-table-column label="规格型号" align="center" prop="ProductModel" />
                <el-table-column label="管理单位" align="center" prop="MaterialUnit" />
                <el-table-column label="生命周期边界" align="center" prop="ProductBorder" />
                <el-table-column label="碳足迹" align="center" prop="CarbonEmission" />
                <el-table-column label="碳排放单位" align="center" prop="CarbonUnit" />
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getMaterialList"/>
        </div>
        <div slot="footer" class="dialog-footer">
            <el-button @click="dialog = false">取消</el-button>
            <el-button type="primary" @click="nextStep">下一步</el-button>
        </div>
        <nextStep ref="nextStep" :tags="tags" @submitForm="submitForm" />
    </el-dialog>
</template>

<script>
import { materialCarbonPageList } from '@/api/energy/material';
import nextStep from './nextStep.vue';
export default {
    name: 'factorTypeAdd',
    components: {
        nextStep,
    },
    props:{
        ProductBorder: {
            type: String,
            default: ''
        },

    },
    watch: {
        ProductBorder(newVal, oldVal){
            this.queryParams.ProductBorder = newVal;
            this.getMaterialList();
        }
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            materialList: [],
            tags: [],
            queryParams: {
                pageNum: 1,
                pageSize: 10,
                orgId: this.$store.getters.orgId,
                ProductBorder: this.ProductBorder,
            },
            LinkId: '',
            total: 0,
        };
    },

    methods: {
        openDialog(LinkId){
            this.LinkId = LinkId;
            this.getMaterialList();
            this.dialog = true;
        },
        // 获取原材料列表
        getMaterialList() {
            materialCarbonPageList(this.queryParams).then(res => {
                this.materialList = res.data.List;
                this.total = res.data.Total;
            })
        },
       
        closeDialog(){
            this.dialog = false;
        },

        // 提交表单
        submitForm(tags){
          // 关键：遍历 tags，为每个元素添加 Link 字段（值为 this.Link）
            const tagsWithLink = tags.map(tag => ({
                ...tag,
                LinkId: this.LinkId
            }));
            this.$emit('submitFormTag', tagsWithLink);
            this.closeDialog();
        },

        // tab已选数据
        removeTag(index) {
            const removedTag = this.tags.splice(index, 1)[0];
            if (removedTag) {
                this.materialList.forEach(row => {
                    if (row.Id === removedTag.Id) { // 匹配唯一键（id）
                        this.$refs.materialTable.toggleRowSelection(row, false);
                    }
                });
            }
        },

        // 处理选择变化
        handleSelectionChange(val) {
            this.tags = val;
        },

        // 下一步操作
        nextStep() {
            if (this.tags.length === 0) {
                this.$message({
                    message: '请选择原材料',
                    type: 'warning'
                });
                return;
            }
            this.$refs.nextStep.openDialog(this.tags);
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

.tab-list{
    width: 100%;
    margin-bottom: 16px;
    display: flex;
    align-items: center;
    >span {
        font-weight: 400;
        font-size: 14px;
        color: rgba(255, 255, 255, 0.65);
        margin-right: 16px;
    }
    .el-tag--dark.el-tag--success{
        background-color: #3DB98F;
        border-color: #3DB98F;
        margin-right: 4px;
        color: #fff;
    }
}
</style>