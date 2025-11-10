<template>
    <el-tabs v-model="activeName">
        <el-tab-pane label="执行结果" name="first">
            <el-table v-if="data!=''" border :data="localValue" max-height="300" style="margin-top: 10px;"> 
                <el-table-column label="序号" type="index" align="center" :show-overflow-tooltip="true" />
                <el-table-column v-for="(column, index) in tableData" :key="index" :label="column.label" :prop="column.prop" align="center" :show-overflow-tooltip="true" />
            </el-table>
            <pagination v-show="data.length > 0" :total="data.length" :page.sync="tableForm.pageNum" :limit.sync="tableForm.pageSize" :pageSizes="pageSizes" @pagination="setNextPage"/>
        </el-tab-pane>
        <el-tab-pane label="节点配置" name="second">
            <fieldFormatting :data="data" :fielForm="fielForm" @getFieldForm='getFieldForm'/>
        </el-tab-pane>
    </el-tabs>
</template>
<script>
import fieldFormatting from "./fieldFormatting";
export default {
    props: {
        data: {
            type: Array
        },
        fielForm: {
            type: Array,
            default: []
        }
    },
    components: {
        fieldFormatting
    },
    data() {
        return {
            pageSizes:[50,100,150,200],
            tableForm:{pageNum:1,pageSize:50},
            tableData: [],
            activeName: 'first',
            localValue: this.data
        }
    },
    watch: {
        data: {
            immediate: true,
            deep: true,
            handler() {
                this.tableForm.pageNum=1
                let dataList=JSON.parse(JSON.stringify(this.data))
                this.localValue = dataList.slice(0, Math.min(50, dataList.length));
                this.disposeData();
            },
        }
    },
    methods: {
        disposeData() {
            this.tableData = []
            for (const key in this.data[0]) {
                let array = { label: key, prop: key }
                this.tableData.push(array) 
            }
        },
        setNextPage(page){
            let dataList=JSON.parse(JSON.stringify(this.data))
            this.localValue = dataList.slice(page.limit*(page.page-1), Math.min(page.limit*page.page, dataList.length));
        },
        getFieldForm(form) {
            this.fielForm.push(form)
            this.$emit("getFieldForm", this.fielForm)
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .el-table__empty-block{
        width: 100% !important;
    }
    .el-tabs__nav-scroll{
        .el-tabs__item{
            // padding: 0 20px !important;
            text-align: center !important;
        }
    }
}
</style>