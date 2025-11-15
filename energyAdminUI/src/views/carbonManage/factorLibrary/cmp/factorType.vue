<template>
    <div class="factorLibrary-tree">
        <div class="factorLibrary-tree-input">
            <div class="factorLibrary-tree-btn">
                <el-button type="custom" icon="el-icon-plus" @click="handleAdd('添加类别')">添加类别</el-button>
                <el-button type="custom" icon="el-icon-plus" :disabled="currentFactorType" @click="handleAdd('添加子类别')">添加能源</el-button>
            </div>
            <div class="factorLibrary-tree-info">
                <i class="el-icon-search" @click="treeSearch"></i>
                <el-input v-model="filterText" type="text" placeholder="请输入类别名称" clearable @clear="getInitList" />
            </div>
        </div>
        
        <el-tree 
            ref="treeSelect"
            :data="factorTypeList" 
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
            <span class="custom-tree-node" slot-scope="{ node, data }">
                <span>{{ node.label }}</span>
                <el-dropdown @command="handleUpdate($event, data)">
                    <span class="el-dropdown-link">
                        <i class="el-icon-more"></i>
                    </span>
                    <el-dropdown-menu slot="dropdown" class="factorLibrary-dropdown">
                        <el-dropdown-item command="edit"><i class="zhongtaiiconfont zhongtai-icon-xiugai1"></i>编辑</el-dropdown-item>
                        <el-dropdown-item command="delete"><i class="el-icon-delete"></i>删除</el-dropdown-item>
                    </el-dropdown-menu>
                </el-dropdown>
            </span>
        </el-tree>
        <factorTypeAdd ref="factorTypeAdd" :currentKey="currentKey" :currentKeyName="currentKeyName" :typeList="factorTypeList" @getInitList="getInitList" />

    </div>
</template>
<script>
import { selectFactorTypeTree, removeFactorType } from '@/api/energy/factorLibrary'
import factorTypeAdd from './factorTypeAdd.vue'
export default {
    name: 'factorType',
    components: {
      factorTypeAdd
    },
    data() {
        return {
            title: '添加类别',
            defaultProps: {
                children: 'Children',
                label: 'TypeName'
            },
            filterText: '',
            factorTypeList: [],
            currentKey: '',
            currentKeyName: '',
            currentFactorType: false
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        getInitList() {
            selectFactorTypeTree().then(res => {
                this.factorTypeList = res.data;
                this.$nextTick(() => {
                    if (res.data && res.data.length > 0) {
                        this.currentKey = res.data[0].Id;
                        this.currentKeyName = res.data[0].TypeName;
                        this.currentFactorType = res.data[0].FactorType;
                        this.$refs['treeSelect'].setCurrentKey(this.currentKey);
                        this.$emit('getList', this.currentKey, this.currentFactorType);
                    }
                    
                });
            })
        },
        // 新增类别
        handleAdd(title){
            this.title = title;
            this.$refs.factorTypeAdd.openDialog(title)
        },
        // 修改类别
        handleUpdate(cmd, data) {
            if(cmd === 'edit' ) {
               const title = data.FactorType ? '编辑子类别' : '编辑类别';
               this.$refs.factorTypeAdd.openDialog(title, data);
            } else {
                this.$confirm('确定删除该类别吗？', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    removeFactorType({ Id: data.Id }).then(res => {
                        this.$message({
                            message: '删除成功',
                            type: 'success'
                        });
                        this.getInitList();
                    })
                }).catch(() => {
                    this.$message({
                        type: 'info',
                        message: '已取消删除'
                    });
                });

            }
        },
        handleNodeClick(data) {
            this.currentKey = data.Id;
            this.currentKeyName = data.TypeName;
            this.currentFactorType = data.FactorType;
            this.$emit('getList', this.currentKey, this.currentFactorType);
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
                        if (!this.filterText || node.TypeName?.indexOf(this.filterText) !== -1) {
                        matchedNodes.push(node);
                        }
                        if (node.Children && node.Children.length) {
                        findMatched(node.Children);
                        }
                    });
                };
                findMatched(this.factorTypeList);

                // 高亮第一个匹配节点
                if (matchedNodes.length > 0) {
                    this.currentKeyName = matchedNodes[0].TypeName;
                    this.currentFactorType = matchedNodes[0].FactorType;
                    this.currentKey = matchedNodes[0].Id;
                    this.$refs['treeSelect'].setCurrentKey(this.currentKey);
                    this.$emit('getList', this.currentKey, this.currentFactorType);
                }
            });
        },
        filterNode(value, data) {
            if (!value) return true;
            return data.TypeName.indexOf(value) !== -1;
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
            margin-top: 20px;
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
        .factorLibrary-tree-btn{
            display: flex;
            justify-content: space-between;
            ::v-deep .el-button{
                width: 96px;
                padding: 0;
                height: 36px;
                display: flex;
                justify-content: center;
                align-items: center;
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
.el-dropdown-menu{
    background: #222E40 !important;
    border: 1px solid #222E40 !important;
    box-shadow: 0px 10px 20px 0px rgba(0,0,0,0.4);
    border-radius: 4px;
    padding: 2px 0 0;
    .el-dropdown-menu__item{
        font-weight: 400;
        font-size: 12px;
        color: #FFFFFF;
        padding: 0 12px;
        line-height: 30px;
        > i {
            font-size: 11px;
        }
    }
    ::v-deep .popper__arrow{
        border-bottom-color: #222e40 !important;
    }
    ::v-deep .popper__arrow::after{
        border-bottom-color: #222e40 !important;
    }
    .el-dropdown-menu__item:focus, .el-dropdown-menu__item:not(.is-disabled):hover{
        background: rgba(61, 185, 143, 0.2) !important;
        color: #3DB98F;
    }
}

</style>