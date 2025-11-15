<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ title }}</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <el-row :gutter="24">
            <el-col :span="12">
                <el-form-item label="因子库发布年份" prop="Year">
                    <el-select v-model="addForm.Year" style="width:100%" placeholder="请选择发布年份" clearable>
                        <el-option v-for="item in yearList" :key="item.Id" :label="item.Year" :value="item.Year" />
                    </el-select>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item label="版本" prop="Version">
                    <el-select v-model="addForm.Version" style="width:100%" placeholder="请选择版本" clearable>
                        <el-option v-for="item in versionList" :key="item.Id" :label="item.Version" :value="item.Version" />
                    </el-select>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item label="因子名称" prop="FactorName">
                    <el-input type="text" v-model="addForm.FactorName" placeholder="请输入因子名称"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item label="因子值" prop="EmissionFactor">
                    <el-input type="number" v-model="addForm.EmissionFactor" placeholder="请输入因子值"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item label="平均低位发热量" prop="AvgCalorific">
                    <el-input type="text" v-model="addForm.AvgCalorific" placeholder="请输入平均低位发热量"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="12">
                <el-form-item label="折标准煤系数" prop="EqCoal">
                    <el-input type="text" v-model="addForm.EqCoal" placeholder="请输入折标准煤系数"></el-input>
                </el-form-item>
            </el-col>
            <el-col :span="24">
                <el-form-item label="来源说明">
                    <el-input type="textarea" :rows="2" v-model="addForm.Memo" placeholder="请输入来源说明"></el-input>
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
import { addFactor, editFactor} from '@/api/energy/factorLibrary'
export default {
    name: 'factorTypeAdd',
    props:{
        title:{
            type: String,
            default: '添加因子'
        },
        yearList: {
            type: Array,
            default: () => []
        },
        versionList: {
            type: Array,
            default: () => []
        },
        typeId: {
            type: String,
            default: ''
        },

    },
    data() {
        return {
            dialog: false,
            addForm: {},
            typeTreeList: [],
            addRules: {
                Year: [{ required: true, trigger: "change", message: "请选择发布年份" }],
                Version: [{ required: true, trigger: "change", message: "请选择版本" }],
                FactorName: [{ required: true, trigger: "blur", message: "请输入因子名称" }],
                EmissionFactor: [{ required: true, trigger: "blur", message: "请输入因子值" }],
                AvgCalorific: [{ required: true, trigger: "blur", message: "请输入平均低位发热量" }],
                EqCoal: [{ required: true, trigger: "blur", message: "请输入折标准煤系数" }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(info){
            if(info) {
                this.addForm={
                    OrgId: info.OrgId,
                    Id: info.Id,
                    Year: info.Year,
                    Version: info.Version,
                    FactorName: info.FactorName,
                    TypeId: info.TypeId,
                    EmissionFactor: info.EmissionFactor,
                    AvgCalorific: info.AvgCalorific,
                    EqCoal: info.EqCoal,
                    Memo: info.Memo,
                }
            }else{
                this.addForm = {
                    OrgId: this.$store.getters.orgId,
                    Year: '',
                    Version: '',
                    FactorName: '',
                    TypeId: this.typeId,
                    EmissionFactor: '',
                    AvgCalorific: '',
                    EqCoal: '',
                    Memo: '',
                }
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
        // 编辑
        setEditList() {
            editFactor(this.addForm).then(response => {
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
            addFactor(this.addForm).then(response => {
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