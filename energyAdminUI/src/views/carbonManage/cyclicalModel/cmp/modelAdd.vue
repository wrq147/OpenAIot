<template>
    <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" top="2vh" width="1000px" append-to-body>
        <div slot="title" class="dialog_title">
        <div class="dialog_title_left">
            <img src="~@/assets/images/zs.png" alt="">
            <span>{{ title }}产品生命周期模型</span>
        </div>
        <div class="dialog_title_right" @click.stop="closeDialog">
            <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
        </div>
        </div>
        <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
            <el-row :gutter="24">
                <el-col :span="12">
                    <el-form-item label="产品名称" prop="ProductId">
                        <el-select v-model="addForm.ProductId" filterable :disabled="isOnlyView" style="width:100%" @change="getProductModel" placeholder="请选择产品名称">
                            <el-option v-for="item in productList" :key="item.Id" :label="item.ProductName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="产品型号">
                        <el-input type="text" v-model="addForm.ProductModel" :disabled="true" />
                    </el-form-item>
                </el-col>
                 <el-col :span="12">
                    <el-form-item label="生命周期模型名称" prop="ModelName">
                        <el-input type="text" v-model="addForm.ModelName" placeholder="请输入生命周期模型名称" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="产品生命周期边界">
                        <div class="tab-list">
                            <div :class="['tab-item', { 'active': index + 1 == ProductBorder }]" v-for="(item, index) in tabList" :key="index" @click="changeTab(index)">
                                <div class="tab-item-name">{{ item.name }}</div>
                                <div class="tab-item-tip">{{ item.tip }}</div>
                            </div>
                        </div>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <div class="model-table">
                        <div class="table-header">
                            <div class="table-header-item" v-for="(item, index) in tableHeader" :key="index">
                                {{ item.name }}
                                <span v-if="item.prop">{{ item.prop }}</span>
                            </div>
                        </div>
                        <div class="table-content">
                            <div class="table-content-item" v-for="(item, rowIndex) in modelData" :key="rowIndex">
                                <div class="table-content-item-name">{{ item.LinkName }}</div>
                                <div class="table-content-item-name">
                                    <div class="consumable-container">
                                        <div class="consumable-list" v-for="(process, pIdx) in item.Processes" :key="pIdx">
                                            <div class="consumable-item" v-for="(shuRu, sIdx) in process.ShuRus" :key="sIdx">
                                                <div class="consumable-item-content" :id="`input-item-${rowIndex}-${pIdx}-${sIdx}`">
                                                    <div class="consumable-name">
                                                    {{ shuRu.TypeName }}：<div><span>{{ shuRu.FactorUnit }}</span></div>
                                                    </div>
                                                    <!-- D3会在这里生成曲线 -->
                                                    <div v-if="process.ShuRus.length > 0"  class="curve-container" :id="`curve-${rowIndex}-${pIdx}-${sIdx}`"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="Materials-list">
                                            <div class="Materials-item" v-for="(process, pIdx) in item.Materials" :key="pIdx">
                                                <div class="Materials-item-content">
                                                    <div class="Materials-name">
                                                        {{ process.MaterialName }}：
                                                        <div style="display: flex; align-items: center;">
                                                            {{ process.BomType }}<span>{{ process.CarbonUnit }}</span>
                                                            <i 
                                                                v-if="!isOnlyView"
                                                                @click.stop="materiaChildRemove(rowIndex, pIdx)"
                                                                class="el-icon-delete" 
                                                                style="cursor: pointer; color: rgba(255,255,255,1);font-size:14px;margin: 2px 0 0 5px;" 
                                                            />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div v-if="(item.Processes.length === 0 && item.Materials.length === 0) && title === '详情'">-</div>
                                    </div>
                                    <div v-if="!isOnlyView" class="product-info-btn" @click="addImport(item.LinkId)">
                                        <div class="product-info-icon">
                                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                        </div>
                                        <div class="product-info-add">
                                            添加{{ item.LinkName }}输入
                                        </div>
                                    </div>
                                </div>
                                <div class="table-content-item-name process-column" :id="`process-col-${rowIndex}`">
                                    <div class="process-list">
                                        <div class="process-item-box" v-for="(k, pIdx) in item.Processes" :key="pIdx">
                                            <div class="process-item" :id="`process-item-${rowIndex}-${pIdx}`" data-is-process="true" @click.stop="editProcess(rowIndex, pIdx)">
                                                <div class="process-item-info">
                                                    <div class="tag">工序<span>{{ k.ProcessNo }}</span></div>
                                                    <div class="name">{{ k.ProcessName }}</div>
                                                </div>
                                                <i 
                                                    v-if="!isOnlyView"
                                                    @click.stop="processChildRemove(rowIndex, pIdx)"
                                                    class="el-icon-delete" 
                                                    style="cursor: pointer; color: rgba(255,255,255,1);font-size:14px;" 
                                                />
                                            </div>
                                        </div>
                                        <div v-if="item.Processes.length === 0 && title === '详情'">-</div>
                                    </div>
                                    <div v-if="!isOnlyView" class="product-info-btn" @click="addProcess(item.LinkId)">
                                        <div class="product-info-icon">
                                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                        </div>
                                        <div class="product-info-add">
                                            添加{{ item.LinkName }}工序
                                        </div>
                                    </div>
                                </div>
                                <div class="table-content-item-name">-</div>
                            </div>
                        </div>
                    </div>
                </el-col>
            </el-row> 
        </el-form>
        <div slot="footer" class="dialog-footer">
            <el-button @click="dialog = false">取消</el-button>
            <el-button v-if="!isOnlyView" type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
        </div>
        <processAdd ref="processAdd" :title="titleChild" :productList="productList" :modelData="modelData" @getProcessItem="getProcessItem" />
        <importAdd ref="importAdd" :ProductBorder="ProductBorder" @submitFormTag="submitFormTag" />
    </el-dialog>
</template>

<script>
import { renderCurves } from './modelRenderLine.js'
import { productPageList } from '@/api/energy/product';
import { modelSelectLink, modelSelectShuRu, addModel, editModel, modelInfo } from '@/api/energy/cyclicalModel';
import processAdd from './processAdd.vue';
import importAdd from './importAdd.vue';

export default {
    name: 'modelAdd',
    components: {
        processAdd,
        importAdd
    },
    props:{
        title:{
            type: String,
            default: '新增'
        },
        tabList: {
            type: Array,
            default: () => []
        }
    },
    data() {
        return {
            titleChild: '',
            dialog: false,
            isOnlyView: false,
            addForm: {},
            productList: [],
            ProductBorder: '',
            addRules: {
                ProductId: [{ required: true, message: '请选择产品名称', trigger: 'blur' }],
                ModelName: [{ required: true, message: '请输入生命周期模型名称', trigger: 'blur' }],
            },
            saveLoading: false,
            tableHeader: [
                {
                    name: '环节'
                },
                {
                    name: '输入',
                    prop: '原料/耗材/能源/资源'
                },
                {
                    name: '-'
                },
                {
                    name: '输出',
                    prop: '废料/废气/废水'
                },
            ],
            modelData: []
        };
    },

    methods: {
        openDialog(info, isOnlyView) {
            const loading = this.$loading({
                lock: true,
                text: '数据加载中...',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            this.isOnlyView = !!isOnlyView;
            this.ProductBorder = info ? info.ProductBorder : '1';
            this.tableHeader[2].name = '-';
            this.addForm = {
                OrgId: this.$store.getters.orgId,
                ProductBorder: this.ProductBorder,
                ProductId: '',
                ProductModel: '',
                ModelName: '',
                Processes: [],
                Materials: [],
                Id: info?.Id // 编辑时携带ID
            };

            // 2. 并行加载产品列表和生命周期模型结构（优化性能）
            Promise.all([
                // 加载产品列表
                productPageList({
                    OrgId: this.$store.getters.orgId,
                    pageNum: 1,
                    pageSize: 9999,
                }),
                // 加载生命周期环节（modelData）
                this.getModelLink() // 后续会修改getModelLink为返回Promise
                ]).then(([productRes, modelData]) => {
                    this.productList = productRes.data.List;
                    // 3. 编辑/详情模式：加载并匹配工序数据
                    if (info) {
                        modelInfo({ Id: info.Id }).then(res => {
                            const detail = res.data;
                            this.addForm = { 
                                ...detail,
                                Materials: detail.Materials.map(rawMat => ({
                                    MaterialCarbonId: rawMat.Id,
                                    Dosage: Number(rawMat.Dosage) || 0,
                                    LinkId: rawMat.LinkId,
                                    BomType: rawMat.BomType || "原料"
                                })),
                            }; // 正确赋值对象（原数组解构错误）

                            // 关键：按LinkId将Processes匹配到modelData对应环节
                            if (detail.Processes && modelData.length) {
                                detail.Processes.forEach(apiProcess => {
                                    // 匹配相同LinkId的环节
                                    const targetLink = modelData.find(
                                        link => link.LinkId === apiProcess.LinkId
                                    );
                                    if (targetLink) {
                                        const localProcess = {
                                            ...apiProcess
                                        };
                                        targetLink.Processes.push(localProcess);
                                    }
                                });
                            }

                            if (detail.Materials && modelData.length) {
                                detail.Materials.forEach(apiMater => {
                                    // 匹配相同LinkId的环节
                                    const targetLink = modelData.find(
                                        link => link.LinkId === apiMater.LinkId
                                    );
                                    if (targetLink) {
                                        const localMater = {
                                            ...apiMater
                                        };
                                        targetLink.Materials.push(localMater);
                                    }
                                });
                            }
                            

                            // 回显产品型号
                            this.getProductModel();
                            // 打开弹窗并绘制曲线
                            loading.close();
                            this.dialog = true;
                            this.$nextTick(() => renderCurves(this.modelData));
                        });
                    } else {
                        loading.close();
                        this.dialog = true;
                    }
            });
        },

        closeDialog(){
            this.dialog = false;
        },

        // 获取生命周期模型
        getModelLink(){
            return new Promise((resolve) => {
                modelSelectLink({ ProductBorder: this.ProductBorder }).then(res => {
                    // 初始化每个环节的processes数组
                    const formattedModelData = res.data.map(item => ({
                        ...item,
                        Processes: [],
                        Materials: [],
                    }));
                    this.modelData = formattedModelData;
                    resolve(formattedModelData); // 返回处理后的modelData供后续使用
                });
            });
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

        // 新增
        setAddList() {
            addModel(this.addForm).then(response => {
                this.$message.success("添加成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },

        // 编辑
        setEditList() {
            editModel(this.addForm).then(response => {
                this.$message.success("修改成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },

        // 获取产品型号
        getProductModel(){
            this.addForm.ProductModel = this.productList.find(item => item.Id == this.addForm.ProductId).ProductModel;
            this.tableHeader[2].name = this.productList.find(item => item.Id == this.addForm.ProductId).ProductName;
        },

        // 添加工序
        addProcess(LinkId){
            this.titleChild = '新增';
            this.$refs.processAdd.openDialog(LinkId)
        },

        // 编辑工序
        editProcess(linkIndex, processIndex) {
            if(this.isOnlyView) return false;
            this.titleChild = '编辑';
            this.$refs.processAdd.openDialog('', this.modelData[linkIndex].Processes[processIndex]);
        },

        // 删除工序
        processChildRemove(linkIndex, processIndex) {
            // 1. 找到对应的环节
            const targetLink = this.modelData[linkIndex];
            if (!targetLink) return;

            // 2. 删除该环节下指定索引的工序
            targetLink.Processes.splice(processIndex, 1);

            // 3. 同步更新 addForm（若需要提交时携带工序数据）
            this.addForm.Processes = this.modelData.flatMap(link => link.Processes);

            // 4. 重新绘制曲线，确保删除后界面同步
            this.$nextTick(() => renderCurves(this.modelData));
        },

        // 添加输入
        addImport(LinkId){
            this.$refs.importAdd.openDialog(LinkId);
        },

        // 获取到tags数据（Materia）
        submitFormTag(tags){
           // 遍历每个tag，单独处理其LinkId
            tags.forEach(tag => {
                // 校验当前tag是否有LinkId
                if (!tag.LinkId) {
                    this.$message.warning(`【${tag.MaterialName}】缺少环节标识，已跳过`);
                return;
                }

                // 找到当前tag对应的环节
                const currentLink = this.modelData.find(link => link.LinkId === tag.LinkId);
                if (!currentLink) {
                    this.$message.warning(`【${tag.MaterialName}】的环节（${tag.LinkId}）不存在，已跳过`);
                    return;
                }

                //  去重：避免重复添加同一原材料（按MaterialId判断）
                const isDuplicate = currentLink.Materials.some(
                    mat => mat.MaterialId === tag.MaterialId
                );
                if (isDuplicate) {
                    this.$message.info(`【${tag.MaterialName}】已存在于该环节，无需重复添加`);
                    return;
                }

                //  添加当前tag到对应环节的Materials中
                currentLink.Materials.push(tag);
            });

            // 同步更新addForm中的Materials（汇总所有环节的原材料）
            this.addForm.Materials = this.modelData.flatMap(link => link.Materials || []);
            this.addForm.Materials = this.modelData
                .flatMap(link => link.Materials || []) // 先汇总所有环节的原材料（空数组兜底）
                .map(rawMat => ({
                    MaterialCarbonId: rawMat.Id,
                    Dosage: Number(rawMat.Dosage) || 0,
                    LinkId: rawMat.LinkId,
                    BomType: rawMat.BomType || "原料"
            }));
        },

        // 删除输入
        materiaChildRemove(linkIndex, materialIndex) {
            // 找到当前选中的环节
            const currentLink = this.modelData[linkIndex];
            if (!currentLink) return;

            // 2. 删除该环节下指定索引的原料
            currentLink.Materials.splice(materialIndex, 1);

            // 3. 同步更新 addForm（若需要提交时携带原料数据）
            this.addForm.Materials = this.modelData.flatMap(link => link.Materials);
        },

        // 生命周期边界切换
        changeTab(index){
            if(this.title !=='新增') {
                return false;
            }
            this.ProductBorder = index + 1;
            this.addForm.ProductBorder = index + 1;
            this.getModelLink();
        },

        // 获取工序数据
        getProcessItem(item, isEdit) {
            const targetItem = this.modelData.find(modelItem => modelItem.LinkId === item.LinkId);
            if (!targetItem) return;

            // 循环处理 processItems 并调用接口
            const processPromises = item.ProcessItems.map(async (processItem, index) => {
                // 将 facilityId 转为字符串
                processItem.FacilityId = Array.isArray(processItem.FacilityId) 
                    ? processItem.FacilityId.join(',') 
                    : (processItem.FacilityId || '');
                
                const facilityIdStr = processItem.FacilityId;
                if (!facilityIdStr) return;

                try {
                    const res = await modelSelectShuRu({  
                        OrgId: this.$store.getters.orgId, 
                        FacilityId: facilityIdStr 
                    });
                    const dataToAdd = Array.isArray(res.data) ? res.data : [res.data];
                    if (isEdit) {
                        item.ShuRus.splice(0, item.ShuRus.length); // 清空数组
                    }
                    item.ShuRus = item.ShuRus || [];
                    item.ShuRus.push(...dataToAdd);
                } catch (err) {
                    this.$message.error(`处理第${index + 1}个工序失败，请重试`);
                }
            });

            // 所有接口请求完成后处理
            Promise.all(processPromises).then(() => {
                // 处理item本身的facilityId
                if (item.FacilityId) {
                    item.FacilityId = Array.isArray(item.FacilityId) 
                        ? item.FacilityId.join(',') 
                        : String(item.FacilityId);
                } else {
                    item.FacilityId = '';
                }

                // 根据isEdit判断是新增还是编辑
                if (isEdit) {
                    // 编辑：替换原工序（通过唯一标识ID匹配）
                    const targetProcessIndex = targetItem.Processes.findIndex(
                        process => process.Id === item.Id
                    );
                    if (targetProcessIndex > -1) {
                        // 替换环节中的工序
                        targetItem.Processes.splice(targetProcessIndex, 1, item);
                        
                        // 同步更新表单中的工序
                        const formProcessIndex = this.addForm.Processes.findIndex(
                            process => process.Id === item.Id
                        );
                        if (formProcessIndex > -1) {
                            this.addForm.Processes.splice(formProcessIndex, 1, item);
                        }
                    }
                } else {
                    // 新增：直接添加
                    targetItem.Processes.push(item);
                    this.addForm.Processes.push(item);
                }

                // 重新绘制曲线
                this.$nextTick(() => renderCurves(this.modelData));
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
.tab-list{
    display: flex;
    align-items: center;
    .tab-item{
        width: 240px;
        height: 68px;
        background: #222E40;
        border-radius: 4px;
        padding: 0 16px;
        cursor: pointer;
        margin-right: 20px;
        display: flex;
        flex-direction: column;
        justify-content: center;
        position: relative;
        
        .tab-item-name{
            font-weight: 500;
            font-size: 14px;
            color: #FFFFFF;
            line-height: 14px;
            margin-bottom: 10px;
        }
        .tab-item-tip{
            font-weight: 400;
            font-size: 12px;
            color: rgba(255, 255, 255, 0.6);
            line-height: 14px;
        }
    }
    .active{
       &::after{
            content: '';
            position: absolute;
            bottom: 0;
            left: 0;
            width: 240px;
            height: 68px;
            background-image: url("~@/assets/images/modelTab.png");
            background-size: cover; /* 调整背景图片的尺寸适应伪元素 */
        }
    }
}
.model-table{
    width: 100%;
    max-height: 410px;
    overflow-y: auto;
    border-left: 1px solid rgba(255, 255, 255, 0.1);
    border-top: 1px solid rgba(255, 255, 255, 0.1);
    .table-header{
        display: flex;
        align-items: center;
        height:64px;
        background: #222E40;
        .table-header-item{
            width: 260px;
            height: 100%;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            font-weight: 400;
            font-size: 14px;
            color: #FFFFFF;
            border-right: 1px solid rgba(255, 255, 255, 0.1);
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            >span{
                font-size: 12px;
                color: rgba(255, 255, 255, 0.6);
                margin-top: 6px;
            }
            &:first-child{
                width: 180px;
            }
        }
    }
    .table-content{
        width: 100%;
        background-color: #19212D;
        .table-content-item{
            min-height: 64px;
            display: flex;
            align-items: stretch;
            position: relative;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            overflow: hidden;
            >div{
                width: 260px;
                padding: 24px 0;
                font-weight: 400;
                font-size: 14px;
                color: #FFFFFF;
                border-right: 1px solid rgba(255, 255, 255, 0.1);
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                &:first-child{
                    width: 180px;
                }
                &:nth-child(3){
                    padding: 4px 0;
                }
            }
        }
    }
}
.product-info-btn{
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    margin-bottom: 24px;
    .product-info-icon{
        width: 16px;
        height: 16px;
        background: #3DB98F;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 10px;
        >i{
            font-size: 8px;
            color: #fff;
        }
    }
    .product-info-add{
        font-weight: 500;
        font-size: 14px;
        color: #3DB98F;
    }
}
.consumable-container{
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}
.consumable-list, .Materials-list{
    width: 100%;
    &:not(:last-child){
        margin-bottom: 10px;
    }
    .consumable-item, .Materials-item {
        margin-bottom: 10px;
        width: 100%;
        display: flex;
        flex-direction: column; 
        align-items: center;
        justify-content: center;
        .consumable-item-content, .Materials-item-content{
            // 作为SVG的定位容器
            display: flex;
            align-items: center; // 耗材名称和曲线垂直居中
            gap: 10px; // 名称和曲线之间的间距
            width: 210px;
            height: 36px;
            border-radius: 20px;
            border: 1px solid #3DB98F;
            padding: 0 12px;
            // 耗材名称样式（可选，根据你的设计调）
            .consumable-name, .Materials-name {
                width: 100%;
                font-weight: 500;
                color: #fff;
                font-size: 14px;
                display: flex;
                align-items: center;
                justify-content: space-between;
                >div{
                    display: flex;
                    align-items: center;
                    >span {
                        font-weight: 400;
                        color: rgba(255, 255, 255, 0.6);
                        margin-left: 10px;
                    }
                }
            }

            .curve-container {
                position: absolute;
                right: 0; // 与曲线宽度匹配
                top: 0;
                width: 100%; // 扩大宽度，避免曲线超出
                height: 100%;
                pointer-events: none;
                background-color: transparent;
            }
       }
        .Materials-item-content{
            border: 1px solid #35C8FF;
        }
    }
}
.Materials-list{
    margin-bottom: 0 !important;
}
.process-list {
    min-height: 0;
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column; 
    align-items: center;
    justify-content: center;
    .process-item-box{
        height: 104px;
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        position: relative;
        &:not(:last-child)::after {
            content: '';
            position: absolute;
            right: 50%;
            bottom: 0;
            transform: translateX(50%);
            width: 10px;
            height: 32px;
            background: url('~@/assets/images/lineBom.png') no-repeat center center;
            background-size: 100% 100%;
        }
         &:not(:first-child)::before {
           content: '';
            position: absolute;
            right: 50%;
            top: 0;
            transform: translateX(50%);
            width: 10px;
            height: 32px;
            background: url('~@/assets/images/lineArrow.png') no-repeat center center;
            background-size: 100% 100%; 
        }
    }
    .process-item{
        width: 210px;
        height: 40px;
        display: flex;
        align-items: center;
        background: #3DB98F;
        border-radius: 20px;
        padding: 0 12px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        cursor: pointer;
        .process-item-info{
            width: 100%;
            display: flex;
            align-items: center;
            .tag{
                font-weight: 500;
                font-size: 14px;
                color: #fff;
                padding-right: 12px;
                position: relative;
                display: flex;
                align-items: center;
                >span{
                    width: 16px;
                    height: 16px;
                    background: #FFFFFF;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 12px;
                    color: #3DB98F;
                    margin-left: 6px;
                }
                &::after{
                    content: '';
                    position: absolute;
                    right: 0;
                    top: 50%;
                    transform: translateY(-50%);
                    width: 1px;
                    height: 14px;
                    background: #FFFFFF;
                }
            }
            .name{
                font-weight: 500;
                font-size: 14px;
                color: #fff;
                margin-left: 20px;
            }
        }
    }
}
</style>