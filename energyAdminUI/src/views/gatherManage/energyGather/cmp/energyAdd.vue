<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="640px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ isOnlyView ? '详情' : title }}能源数据</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <div>
            <el-row :gutter="24">
                <el-col :span="12">
                    <el-form-item label="设备名称" prop="EquipmentId">
                        <el-select style="width: 100%" v-model="addForm.EquipmentId" :disabled="title === '编辑' || isOnlyView" @change="getUnit" placeholder="请选择设备名称">
                            <el-option v-for="item in equipmentList" :key="item.Id" :label="item.EquipmentName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="统计日期" prop="DDate">
                         <el-date-picker v-model="addForm.DDate" type="date" :disabled="title === '编辑' || isOnlyView" value-format="yyyy-MM-dd" placeholder="选择日期" style="width:100%" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="期初表码值" prop="InitVale">
                        <el-input type="number" v-model="addForm.InitVale" :disabled="isOnlyView" placeholder="请输入期初表码值" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="期末表码值" prop="EndVale">
                        <el-input type="number" v-model="addForm.EndVale" :disabled="isOnlyView" placeholder="请输入期末表码值" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="单位">
                        <el-input type="text" v-model="addForm.Unit" disabled />
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
import { addEnergy, editEnergy} from '@/api/energy/gatherManage'
export default {
    name: 'energyAdd',
    props:{
        title:{
            type: String,
            default: '添加'
        },
        equipmentList: {
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
                EquipmentId: [{ required: true, trigger: "blur", message: "请选择设备名称" }],
                DDate: [{ required: true, trigger: "blur", message: "请选择统计日期" }],
                InitVale: [{ required: true, trigger: "blur", message: "请输入期初表码值" }],
                EndVale: [{ required: true, trigger: "blur", message: "请输入期末表码值" }],

            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(info, isOnlyView) {
            if(info) {
                this.addForm={
                    OrgId: info.OrgId,
                    Id: info.Id,
                    EquipmentId: info.EquipmentId,
                    DDate: info.DDate,
                    InitVale: info.InitVale,
                    EndVale: info.EndVale,
                    Unit: info.Unit,

                }
            }else{
                this.addForm = {
                    OrgId: this.$store.getters.orgId,
                    EquipmentId: '',
                    DDate: '',
                    InitVale: '',
                    EndVale: '',
                    Unit: ''
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
                const initVale = Number(this.addForm.InitVale);
                const endVale = Number(this.addForm.EndVale);
                
                // 验证数值有效性
                if (isNaN(initVale) || isNaN(endVale)) {
                    this.$message.error("表码值必须为有效的数字");
                    return;
                }
                
                // 验证逻辑
                if (initVale > endVale) {
                    this.$message.error("期初表码值不能大于期末表码值");
                    return;
                }
                if (initVale < 0) {
                    this.$message.error("期初表码值不能小于0");
                    return;
                }
                if (endVale < 0) {
                    this.$message.error("期末表码值不能小于0");
                    return;
                }
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
            editEnergy(this.addForm).then(response => {
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
            addEnergy(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 获取单位
        getUnit(){
            if(this.addForm.EquipmentId) {
                this.equipmentList.forEach(item => {
                    if(item.Id === this.addForm.EquipmentId){
                        this.addForm.Unit = item.Unit;
                    }
                })
            } else {
                this.addForm.Unit = '';
            }
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