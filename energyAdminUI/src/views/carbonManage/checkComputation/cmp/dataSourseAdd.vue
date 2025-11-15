<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="560px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>数据来源</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <el-row :gutter="24">
            <el-col :span="24">
                <el-form-item label="活动数据来源" prop="DataSource">
                    <el-select v-model="addForm.DataSource" style="width:100%" placeholder="请选择活动数据来源">
                        <el-option v-for="(item, key) in dataSourceList" :key="key" :label="item" :value="key" />
                    </el-select>
                </el-form-item>
                <el-form-item label="关联设备" prop="RangeId">
                    <el-select v-model="addForm.EquipmentId" multiple style="width:100%" placeholder="请选择设备名称" clearable>
                        <el-option v-for="item in devList" :key="item.Id" :label="item.EquipmentName" :value="item.Id" />
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
import { equipmentPageList } from '@/api/energy/equip';
import { updateOrgClass } from '@/api/energy/emissionCategory';
export default {
    name: 'categoryAdd',
    props:{
        title:{
            type: String,
            default: '添加'
        },
        dataSourceList: {
            type: Object,
            default: {}
        },
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            devList: [],
            addRules: {
                DataSource: [{ required: true, trigger: "change", message: "请选择活动数据来源" }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(info){
            equipmentPageList({
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 9999,
            }).then(res => {
                this.devList = res.data.List;
            })
            this.addForm = {
                ...info,
                EquipmentId: info.EquipmentIds ? info.EquipmentIds.split(',') : []
            };
            this.dialog = true
        },
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    this.saveLoading = true;
                    this.setEditList();
                }
            });
        },
        // 编辑
        setEditList() {
            this.addForm.EquipmentIds = this.addForm.EquipmentId.join(',')
            updateOrgClass(this.addForm).then(response => {
                this.$message.success("修改成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
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