<template>
     <div>
        <div style="margin-top: -10px">
            <el-button type="text" @click="addItem">+ 添加</el-button>
        </div>
        <el-table border :data="configData.chartOption.staticDataValue" max-height="800" style="margin-top: 10px;"> 
            <el-table-column label="name" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.name" placeholder="请输入" />
                </template>
            </el-table-column>
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
            <el-table-column label="value" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.value" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="100px" align="center">
                <template v-slot="scope">
                    <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.row)">删除</el-button>
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
        remove(row) {
            this.configData.chartOption.staticDataValue.splice(this.configData.chartOption.staticDataValue.indexOf(row), 1);
        },
        addItem() {
            this.configData.chartOption.staticDataValue.push({
                name: '',
                value: ''
            });
        }
    }
}
</script>
<style lang="scss" scoped>

</style>