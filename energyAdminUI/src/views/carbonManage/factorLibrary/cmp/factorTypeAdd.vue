<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" :width="title === '添加类别' || title === '编辑类别' ? '400px' : '640px'">
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
        <div>
            <el-row v-if="title === '添加类别' || title === '编辑类别'" :gutter="24"> 
                <el-col :span="24">
                    <el-form-item label="父级分类" prop="ParentId">
                        <treeselect class="groupSet" v-model="addForm.ParentId" :options="typeTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择父级分类"
                        />
                    </el-form-item>
                    <el-form-item label="类别名称" prop="TypeName">
                        <el-input type="text" v-model="addForm.TypeName" placeholder="请输入类别名称"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row v-else :gutter="24">
                <el-col :span="12">
                    <el-form-item label="所属类别" prop="ParentId">
                       <div class="defaultShow"> {{ addForm.ParentName }}</div>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="子类别名称" prop="TypeName">
                        <el-input type="text" v-model="addForm.TypeName" placeholder="请输入子类别名称"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="碳排单位" prop="FactorUnit">
                        <el-select v-model="addForm.FactorUnit" style="width:100%" clearable placeholder="请选择碳排单位">
                            <el-option label="tCO2/TJ" value="tCO2/TJ" />
                            <el-option label="kg/TJ" value="kg/TJ" />
                            <el-option label="m³CH4/t" value="m³CH4/t" />
                            <el-option label="kgCO2/kWh" value="kgCO2/kWh" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="能源单位" prop="ActivityUnit">
                        <el-select v-model="addForm.ActivityUnit" style="width:100%" clearable placeholder="请选择能源单位">
                            <el-option v-for="item in eneryMeasure" :label="item.label" :value="item.label" :key="item.value" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="转换系数" prop="ActivityConversion">
                        <el-input type="number" v-model="addForm.ActivityConversion" placeholder="请输入转换系数"></el-input>
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
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import { addFactorType, editFactorType} from '@/api/energy/factorLibrary'
export default {
    name: 'factorTypeAdd',
    components: { Treeselect },
    dicts: ['sys_enery_measure'],
    props:{
        typeList: {
            type: Array,
            default: () => []
        },
        currentKey: {
            type: String,
            default: ''
        },
        currentKeyName: {
            type: String,
            default: ''
        }
    },
    watch: {
        typeList: {
            handler(newVal, oldVal) {
                this.typeTreeList = [...newVal];
                // 判断是否已存在“作为一级分类”，不存在才添加
                const hasRoot = this.typeTreeList.some(item => item.Id === '-');
                if (!hasRoot) {
                    this.typeTreeList.unshift({
                        TypeName: "作为一级分类",
                        Id: '-',
                        Children: []
                    });
                }
            },
            deep: true
        }
    },
    data() {
        return {
            title: '',
            dialog: false,
            addForm: {},
            typeTreeList: [],
            addRules: {
                TypeName: [{ required: true, trigger: "blur", message: "请输入类别名称" }],
                ParentId: [{ required: true, trigger: "change", message: "请选择父级分类" }],
                ActivityConversion: [{ required: true, trigger: "blur", message: "请输入转换系数" }],
                FactorUnit: [{ required: true, trigger: "change", message: "请选择碳排单位" }],
                ActivityUnit: [{ required: true, trigger: "change", message: "请选择能源单位" }],
            },
            eneryMeasure: [],
            saveLoading: false,
        };
    },

    mounted() {
        this.eneryMeasure = this.dict.type.sys_enery_measure;
    },

    methods: {
        openDialog(title, info){
            this.title = title;
            if(info) {
                this.addForm={
                    Id: info.Id,
                    TypeName: info.TypeName,
                    ParentId: info.ParentId,
                    ParentName: info.ParentId === '-' ? info.TypeName : info.ParentName,
                    NameTitle: info.NameTitle,
                    FactorTitle: info.FactorTitle,
                    FactorDigits: info.FactorDigits,
                    FactorUnit: info.FactorUnit,
                    ActivityUnit: info.ActivityUnit,
                    FactorType: info.FactorType,
                    ActivityConversion: info.ActivityConversion,
                }
            }else{
                this.addForm = {
                    TypeName:'',
                    ParentId: this.title === '添加类别' ? '-' : this.currentKey,
                    ParentName: this.title === '添加类别' ? '' : this.currentKeyName,
                    NameTitle: "因子名称",
                    FactorTitle: "因子",
                    FactorDigits: 1,
                    FactorUnit: '',
                    ActivityUnit: '',
                    FactorType: this.title === '添加类别' ? false : true,
                    ActivityConversion: '1.00'
                }
            }
            this.dialog = true
        },
        normalizer(node) {
            if (node.Children == null || !node.Children.length) {
                delete node.Children;
            }
            return {
                id: node.Id,
                label: node.TypeName,
                children: node.Children,
            };
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
            editFactorType(this.addForm).then(response => {
                this.$message.success("修改成功");
                this.dialog = false;
                this.$emit('getInitList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 新增
        setAddList() {
            addFactorType(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getInitList');
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
.defaultShow{
    width: 100%;
    height: 40px;
    background: #222E40;
    border-radius: 4px;
    font-weight: 400;
    font-size: 14px;
    color: #FFFFFF;
    padding-left: 10px;
}
</style>