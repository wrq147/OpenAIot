<template>
    <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="800px" top="2vh" :close-on-click-modal="false" append-to-body>
        <div slot="title" class="dialog_title">
            <div class="dialog_title_left">
                <img src="@/assets/images/zs.png" alt="">
                <span>完善原材料清单</span>
            </div>
            <div class="dialog_title_right" @click.stop="closeDialog">
                <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
            </div>
        </div>
        <div>
            <el-table ref="materialTable" :data="tags" row-key="Id" class="data_table" style="width:100%; max-height: 600px; overflow: auto;">
                <el-table-column label="序号" align="center" type="index" width="50"/>
                <el-table-column label="物料名称" align="center" prop="MaterialName" />
                <el-table-column label="物料类型" align="center" prop="MaterialType" />
                <el-table-column label="管理单位" align="center" prop="MaterialUnit" />
                <el-table-column label="物料数量" align="center">
                    <template slot-scope="scope">
                        <el-select v-model="scope.row.BomType" placeholder="请选择">
                            <el-option v-for="item in dataBomTypeList" :key="item" :label="item" :value="item" />
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column label="物料数量" align="center">
                    <template slot-scope="scope">
                        <el-input type="number" v-model="scope.row.Dosage" placeholder="请输入数量" />
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div slot="footer" class="dialog-footer">
            <el-button @click="dialog = false">取消</el-button>
            <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
        </div>
    </el-dialog>
</template>

<script>
export default {
    name: 'factorTypeAdd',
    props:{
        ProductBorder: {
            type: String,
            default: ''
        },
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            materialList: [],
            saveLoading: false,
            tags: [],
            dataBomTypeList: ['原料', '耗材', '能源', '资源'],
        };
    },

    methods: {
        openDialog(tags){
            this.tags = tags;
            this.dialog = true;
        },
       
        closeDialog(){
            this.dialog = false;
        },

        submitForm(){
            if(this.tags.some(item => !item.BomType || !item.Dosage)){
                this.$message.error('请完善所有物料信息');
                return;
            }
            this.$emit('submitForm', this.tags);
            this.saveLoading = false;
            this.dialog = false;
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
</style>