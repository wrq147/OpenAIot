<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="560px" :close-on-click-modal="false" append-to-body>
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>{{ title }}工序</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <div>
            <el-row :gutter="24"> 
                <el-col :span="24">
                    <el-form-item label="所属环节">
                        <el-select v-model="addForm.LinkId" style="width:100%">
                            <el-option v-for="k in modelData" :key="k.LinkId" :label="k.LinkName" :value="k.LinkId" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="工序名称" prop="ProcessName">
                        <el-input type="text" v-model="addForm.ProcessName" placeholder="请输入工序名称"></el-input>
                    </el-form-item>
                    <el-form-item label="工序排序" prop="ProcessNo">
                        <el-input type="number" v-model="addForm.ProcessNo" placeholder="请输入工序排序"></el-input>
                    </el-form-item>
                    <el-form-item label="产出物" prop="ProcessItems" style="position: relative;">
                        <div class="outputProducts">
                            <div class="outputProducts-item" v-for="(item, index) in addForm.ProcessItems" :key="index">
                                <div class="left">
                                    <div class="input-info">
                                        <span>产品名称</span>
                                        <div style="width: 380px;">
                                            <el-select v-model="item.ProductId" filterable style="width:100%" placeholder="请选择产品名称">
                                                <el-option v-for="k in productList" :key="k.Id" :label="k.ProductName" :value="k.Id" />
                                            </el-select>
                                        </div>
                                    </div>
                                    <div class="input-info">
                                        <span>选择设施</span>
                                         <treeselect v-model="item.FacilityId" :multiple="true" :options="facilityList" :show-count="true" 
                                         :normalizer="normalizer" placeholder="请选择设施"/>
                                    </div>
                                </div>
                                <template v-if="index > 0">
                                    <i 
                                        class="el-icon-delete" 
                                        style="cursor: pointer; color: rgba(255,255,255,0.6);font-size:18px;" 
                                        @click="typeChildRemove(index)"
                                    />
                                </template>
                            </div>
                        </div>
                        <div class="typeChildAdd" @click="typeChildAdd">
                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                            <span style="margin-left:6px">添加产出物</span>
                        </div>
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
import { facilityTree } from '@/api/energy/facility';
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
    name: 'factorTypeAdd',
    components: { Treeselect },
    props:{
        title: {
            type: String,
            default: '新增'
        },
        productList: {
            type: Array,
            default: () => []
        },
        modelData: {
            type: Array,
            default: () => {}
        }
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            facilityList: [],
            addRules: {
                ProcessName: [{ required: true, trigger: "blur", message: "请输入工序名称" }],
                ProcessNo: [{ required: true, trigger: "blur", message: "请输入工序排序" }],
                ProcessItems: [{ required: true, trigger: "blur", message: "请添加产出物" }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog(LinkId, info){
            facilityTree({ OrgId: this.$store.getters.orgId }).then(res => {
                this.facilityList = res.data;
            })
            if(info) {
                this.addForm={
                    ...info,
                    ProcessItems: info.ProcessItems.map(item => ({
                        ...item,
                        FacilityId: item.FacilityId.split(',') || null
                    }))
                }
            }else{
                this.addForm = {
                    LinkId: LinkId,
                    ProcessName: '',
                    ProcessNo: 1,
                    ProcessItems: [
                        {
                            ProductId: '',
                            FacilityId: null
                        }
                    ]
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
                label: node.FacilityName,
                children: node.Children,
            };
        },
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    for (const item of this.addForm.ProcessItems) {
                        if (!item.ProductId) {
                            this.$message.error("请输入产品名称");
                            return; // 直接跳出外层函数，终止后续验证
                        }
                    }
                    this.saveLoading = true;
                    const params = JSON.parse(JSON.stringify(this.addForm));
                    if (this.addForm.Id) {
                        this.$emit('getProcessItem', params, true)
                    } else {
                        this.$emit('getProcessItem', params, false)
                    }
                    this.saveLoading = false;
                    this.dialog = false;
                }
            });
        },

        // 新增产出物
        typeChildAdd(){
            this.addForm.ProcessItems.push({
                ProductId: '',
                FacilityId: null
            })
        },

        // 删除产出物
        typeChildRemove(index){
            this.addForm.ProcessItems.splice(index, 1)
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
.outputProducts {
    .outputProducts-item{
        background: #131922;
        border-radius: 4px;
        padding: 16px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        .input-info {
            display: flex;
            align-items: center;
            &:not(:last-child) {
                margin-bottom: 12px;
            }
            > span{
                width: 72px;
                color: rgba(255, 255, 255, 0.60);
                font-size: 14px;
                font-weight: 400;
            }
            ::v-deep {
                .vue-treeselect{
                    width: 380px;
                }
            }
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