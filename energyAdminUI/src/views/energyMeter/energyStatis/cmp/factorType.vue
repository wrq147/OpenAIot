<template>
    <div class="factorLibrary-tree">
        <div class="factorLibrary-tree-input">
            <div class="factorLibrary-tree-info">
                <i class="el-icon-search" @click="treeSearch"></i>
                <el-input v-model="filterText" type="text" placeholder="请输入设施名称" clearable @clear="getInitList" />
            </div>
        </div>
        <el-tree 
            ref="treeSelect"
            :data="facilityTreeList" 
            node-key="Id"
            :props="defaultProps" 
            :default-expand-all="true"
            :highlight-current="true"
            :check-on-click-node="true"
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            :current-node-key="currentKey"
            @node-click="handleNodeClick"
        >
            <span class="custom-tree-node" slot-scope="{ node }">
                <span>{{ node.label }}</span>
            </span>
        </el-tree>
    </div>
</template>
<script>
import { selectFacilityEquipmentTree } from '@/api/energy/energyMeter';
export default {
    name: 'factorType',
    props:{
        isSetEquipVal:{
            type:Boolean,
            default:false
        }
    },
    data() {
        return {
            defaultProps: {
                children: 'Children',
                label: 'FacilityName'
            },
            facilityTreeList: [],
            currentKey: '',
            filterText: '',
            activeEquipObj:null
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        getInitList() {
            selectFacilityEquipmentTree({ OrgId: this.$store.getters.orgId }).then(res => {
                this.activeEquipObj=null
                this.facilityTreeList = this.transformTree(res.data);
               
                this.$nextTick(() => {
                    if (this.facilityTreeList && this.facilityTreeList.length > 0) {
                        if(!this.isSetEquipVal){
                            this.currentKey = res.data[0].Id;
                            this.$refs['treeSelect'].setCurrentKey(this.currentKey);
                            this.$emit('getList', this.currentKey, false);
                        }else{
                            if(this.activeEquipObj){
                                this.currentKey=this.activeEquipObj.Id
                                this.$refs['treeSelect'].setCurrentKey(this.currentKey);
                                this.$emit('getList', this.currentKey, false);
                            }
                        }
                        
                    }
                });
            });
        },

        // 转换树形结构的递归方法
        transformTree(nodes) {
            if (!nodes || !nodes.length) return [];
            
            return nodes.map(node => {
                // 1. 为所有节点添加CommonId（自身Id）
                const transformedNode = {
                    ...node,
                    isEquipment: false
                };
                
                // 2. 判断是否为最后一级节点（没有Children且有Equipments）
                const isLastLevel = !node.Children || node.Children.length === 0;
                if (isLastLevel && node.Equipments && node.Equipments.length > 0) {
                    // 3. 最后一级：将Equipments转为Children
                    if(this.activeEquipObj==null){
                        this.activeEquipObj=JSON.parse(JSON.stringify(node.Equipments[0]))
                    }
                    transformedNode.Children = node.Equipments.map(equipment => ({
                        ...equipment,
                        FacilityName: equipment.EquipmentName,
                        // 保留设备原有字段，同时标记为设备类型
                        isEquipment: true
                    }));
                    // 清空原始Equipments（已转为Children）
                    transformedNode.Equipments = null;
                } else {
                    // 4. 非最后一级：递归处理子节点
                    transformedNode.Children = this.transformTree(node.Children);
                }
                return transformedNode;
            });
        },

        handleNodeClick(data) {
            this.currentKey = data.Id;
            this.$emit('getList', this.currentKey, data.isEquipment);

        },
        // tree搜索
        treeSearch() {
            this.$refs.treeSelect.filter(this.filterText);
            // 搜索完成后，高亮第一个匹配节点
            this.$nextTick(() => {
                // 获取所有匹配的节点（通过过滤后的树数据筛选）
                const matchedNodes = [];
                const findMatched = (nodes) => {
                    nodes.forEach(node => {
                        // 沿用filterNode的匹配逻辑
                        if (!this.filterText || node.FacilityName?.indexOf(this.filterText) !== -1) {
                            matchedNodes.push(node);
                        }
                        if (node.Children && node.Children.length) {
                            findMatched(node.Children);
                        }
                    });
                };
                findMatched(this.facilityTreeList);
                // 高亮第一个匹配节点
                if (matchedNodes.length > 0) {
                    this.currentKey = matchedNodes[0].Id;
                    this.$refs['treeSelect'].setCurrentKey(this.currentKey);
                    this.$emit('getList', this.currentKey);
                }
            });
        },
        filterNode(value, data) {
            if (!value) return true;
            return data.FacilityName.indexOf(value) !== -1;
        }
    }
}
</script>
<style lang="less" scoped>
.factorLibrary-tree {
    padding: 16px 0;
    .factorLibrary-tree-input{
        padding: 0 20px;
        margin-bottom: 16px;
        .factorLibrary-tree-info{
            width: 100%;
            display: flex;
            align-items: center;
            height: 32px;
            border-radius: 4px;
            border: 1px solid rgba(255, 255, 255, 0.2);
            > i {
                font-size: 14px;
                color: #999999;
                margin-left: 10px;
                cursor: pointer;
            }
            ::v-deep .el-input{
                height: 100%;
                .el-input__inner{
                    border: none !important;
                    background: transparent;
                    height: 100%;
                    color: #999999;
                    padding-left: 8px;
                }
                .el-input__suffix{
                    height: 32px !important; // 与输入框高度完全一致
                    top: 0 !important; // 重置顶部偏移
                    display: flex;
                    align-items: center; // 图标垂直居中
                    padding: 0 8px; // 清除多余内边距
                }
            }
        }
    }
    ::v-deep .el-tree-node{
        .el-tree-node__content{
            padding: 0 20px !important;
            height: 36px;
            background: transparent;
            font-size: 14px;
            font-weight: 400;
        }
        .el-tree-node__children{
            padding-left: 10px;
        }
    }
    ::v-deep .el-tree--highlight-current .el-tree-node.is-current > .el-tree-node__content{
        background: rgba(61, 185, 143, 0.2) !important;
        color: #3DB98F;
    }
    .custom-tree-node{
        width: 100%;
        display: flex;
        justify-content: space-between;
    }
    .el-dropdown-link > i {
        color: rgba(255, 255, 255, 0.6);
        font-size: 10px;
        transform: rotate(90deg);
    }
}

</style>