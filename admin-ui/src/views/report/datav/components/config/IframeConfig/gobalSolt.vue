<template>
    <div>
        <el-table v-if="tableData!=''" border :data="tableData" max-height="500" style="margin-top: 10px;" :cell-class-name="tableCellClassName" @cell-click="cellClick" :cell-style="cellStyle"> 
            <el-table-column label="序号" type="index" align="center" :show-overflow-tooltip="true" />
            <el-table-column v-for="(column, index) in columnData" :key="index" :label="column.label" :prop="column.prop" align="center" :show-overflow-tooltip="true" />
        </el-table>
    </div>
</template>
<script>
export default {
    props: {
        configData: {
            type: Object
        },
        themeForm: {
            type: Object
        }
    },
    watch: {
        configData: {
            immediate: true,
            deep: true,
            handler() {
                this.tableData = this.configData.chartOption.globalProcessor === null ? [] : this.tableData
                this.initResult()
            },
        }
    },
    data() {
        return {
            resultData: [],
            tableData: [],
            columnData: [],
            clickedRow: this.configData.chartOption.modelValue !== undefined ? this.configData.chartOption.modelValue[0] : '', // 点击的单元格行号
            clickedColumn: this.configData.chartOption.modelValue !== undefined ? this.configData.chartOption.modelValue[1] : '', // 点击的单元格列名
        }
    },
    methods: {
        initResult () {
            let themeForm = this.themeForm
            themeForm.globalData.forEach((item, index) => {
                if (item.name === this.configData.chartOption.globalData) {
                    this.resultData = item.rawData !== undefined ? JSON.parse(item.rawData) : []
                    return
                }
            })
            this.filterData(this.configData.chartOption.globalProcessor)
        },
        filterData(event) {
            this.resultData.forEach((item, index) => {
                if (item.title === event) {
                    this.tableData = item.content
                    return this.disposeData()
                }
            })
        },
        disposeData() {
            this.columnData = []
            for (const key in this.tableData[0]) {
                let array = { label: key, prop: key }
                this.columnData.push(array) 
            }
        },
        tableCellClassName({ row, column, rowIndex, columnIndex }) {
            //利用单元格的 className 的回调方法，给行列索引赋值
            row.index = rowIndex;
            column.index = columnIndex;
        },
        cellClick(row, column, cell, event) {
            this.clickedRow = row.index
            this.clickedColumn = column.index
            this.finalResult = cell.innerText;
            this.$set(this.configData.chartOption, "modelValue", [row.index, column.index]);
        },
        cellStyle({ row, column, rowIndex, columnIndex }) {
            if (row.index === this.clickedRow && column.index === this.clickedColumn) {
                return 'border:1px solid #2C89E5;color:#2C89E5'
            }else {
                return ''
            }
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .dataOrigin {
        font-size: 14px;
        color: red;
        margin-bottom: 10px;
    }
}
</style>