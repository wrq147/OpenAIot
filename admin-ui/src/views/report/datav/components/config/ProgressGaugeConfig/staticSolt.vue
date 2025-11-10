<template>
     <div>
        <el-table border :data="configData.chartOption.staticDataValue" max-height="800" style="margin-top: 10px;"> 
            <el-table-column label="name" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.name" placeholder="请输入" />
                </template>
            </el-table-column>
            <el-table-column label="value" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-input v-model="scope.row.value" placeholder="请输入" />
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
    methods: {}
}
</script>
<style lang="scss" scoped>

</style>