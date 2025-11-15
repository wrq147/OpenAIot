<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="560px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ isOnlyView ? '详情' : title }}排放类别</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <el-row :gutter="24">
            <el-col :span="24">
                <el-form-item label="排放类别" prop="ClassName">
                     <el-input type="text" v-model="addForm.ClassName" :disabled="isOnlyView" placeholder="请输入排放类别"></el-input>
                </el-form-item>
                <el-form-item label="所属范围" prop="RangeId">
                    <el-select v-model="addForm.RangeId" :disabled="isOnlyView" style="width:100%" placeholder="请选择所属范围" clearable>
                        <el-option v-for="(item, key) in rangeList" :key="key" :label="item" :value="key" />
                    </el-select>
                </el-form-item>
                <el-form-item label="子类别" prop="SubClass" style="position: relative;">
                    <div v-for="(item, index) in addForm.SubClass" style="margin-bottom: 8px;" :key="index">
                        <el-input type="text" :disabled="isOnlyView" v-model="addForm.SubClass[index].SubClassName" placeholder="请输入排放类别">
                            <template v-if="index > 0 && !isOnlyView" #suffix>
                                <i 
                                class="el-icon-delete" 
                                style="cursor: pointer; color: rgba(255,255,255,0.6);margin-right: 4px;" 
                                @click="typeChildRemove(index)"
                                ></i>
                            </template>
                        </el-input>
                    </div>
                    <div v-if="!isOnlyView" class="typeChildAdd" @click="typeChildAdd">
                        <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                        <span style="margin-left:6px">添加</span>
                    </div>
                </el-form-item>
                <el-form-item label="排放序号" prop="ClassNo">
                     <el-input type="number" :disabled="isOnlyView" v-model="addForm.ClassNo" placeholder="请输入排放序号"></el-input>
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
import { classInfo, classAdd, classEdit } from '@/api/energy/emissionCategory';
export default {
    name: 'categoryAdd',
    props:{
        title:{
            type: String,
            default: '添加'
        },
        rangeList: {
            type: Object,
            default: {}
        },

    },
    data() {
        return {
            dialog: false,
            addForm: {},
            isOnlyView: false,
            typeChildList: [
                {
                    subClassName: ''
                }
            ],
            addRules: {
                ClassName: [{ required: true, trigger: "blur", message: "请输入排放类别" }],
                RangeId: [{ required: true, trigger: "change", message: "请选择所属范围" }],
                SubClass: [{ required: true, trigger: "blur", message: "请输入子类别" }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(info, isOnlyView){
            if(info) {
                classInfo({
                    Id: info.Id
                }).then(res => {
                    this.addForm = res.data;
                })
                
            }else{
                this.addForm = {
                    OrgId: this.$store.getters.orgId,
                    ClassName: '',
                    RangeId: '',
                    ClassNo: 1,
                    SubClass: [{
                        SubClassName: ''
                    }]
                }
            }
            if(isOnlyView) {
                this.isOnlyView = isOnlyView;
            }else{
                this.isOnlyView = false;
            }
            this.dialog = true;
        },
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    for (const item of this.addForm.SubClass) {
                        if (!item.SubClassName) {
                            this.$message.error("请输入子类别");
                            return;
                        }
                    }
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
            classEdit(this.addForm).then(response => {
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
            classAdd(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 新增子类别
        typeChildAdd() {
            this.addForm.SubClass.push({
                SubClassName: ''
            });
        },
        // 删除子类别
        typeChildRemove(index) {
            this.addForm.SubClass.splice(index, 1);
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
.typeChildAdd{
    position: absolute;
    top: -34px;
    right: 0;
    font-weight: 400;
    font-size: 14px;
    color: #3DB98F;
    align-items: center;
    display: flex;
    cursor: pointer;
    >i{
        font-size: 10px;
    }
}
</style>