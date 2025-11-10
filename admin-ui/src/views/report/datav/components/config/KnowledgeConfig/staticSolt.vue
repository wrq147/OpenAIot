<template>
    <div>
        <div style="margin-top: -10px">
            <el-button type="text" @click="addItem('nodes')">+ 添加</el-button>
        </div>
        <el-table border :data="configData.chartOption.staticDataValue.nodes" max-height="800" style="margin-top: 10px;"> 
            <el-table-column label="name" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.name" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column label="x" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.x" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column label="y" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.y" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="100px" align="center">
                <template v-slot="scope">
                    <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.row, 'nodes')">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div style="margin: 10px 0">
            <el-button type="text" @click="addItem('links')">+ 添加</el-button>
        </div>
        <el-table border :data="configData.chartOption.staticDataValue.links" max-height="800" style="margin-top: 10px;"> 
            <el-table-column label="source" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.source" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column label="target" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.target" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="100px" align="center">
                <template v-slot="scope">
                    <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.row, 'links')">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>
<script>
export default {
    props: {
        configData: {
            type: Object
        }
    },
    data() {
        return {
            
        }
    },
    methods: {
        remove(row, name) {
            this.configData.chartOption.staticDataValue[name].splice(this.configData.chartOption.staticDataValue[name].indexOf(row), 1);
        },
        addItem(name) {
            if (name === 'nodes') {
                this.configData.chartOption.staticDataValue.nodes.push({
                    name: '',
                    x: '',
                    y: ''
                });
            } else {
                this.configData.chartOption.staticDataValue.links.push({
                    source: '',
                    target: ''
                });
            }
        }
    }
}
</script>
<style lang="scss" scoped>

</style>