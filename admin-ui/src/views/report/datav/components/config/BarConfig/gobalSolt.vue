<template>
    <div v-if="tableData!=''">
        <el-table border :data="tableData" max-height="500" style="margin-top: 10px;">
            <el-table-column label="序号" type="index" align="center" :show-overflow-tooltip="true" />
            <el-table-column v-for="(column, index) in columnData" :key="index" :label="column.label" :prop="column.prop" align="center" :show-overflow-tooltip="true" />
        </el-table>
        <div>
            <div class="settingValue" v-for="(value , key) in staticDataValue" :key="key" >
                <span >{{ key }}</span>
                <el-select v-model="modelValue[key]" filterable allow-create placeholder="值" @change="changeCols(key, $event)">
                    <el-option v-for="(column, index) in columnData" :key="index" :value="index + 1" :label="column.label" />
                </el-select>
            </div>
        </div>
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
                this.staticDataValue = this.configData.chartOption.staticDataValue[0]
                this.initResult()
            },
        }
    },
    data() {
        return {
            modelValue: this.configData.chartOption.modelValue,
            resultData: [],
            tableData: [],
            columnData: [],
            staticDataValue: []
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
        changeCols(key, event) {
            this.$set(this.configData.chartOption, "modelValue", this.modelValue);
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
.settingValue span{
    display: block;
    margin: 15px 0;
    font-size: 14px;
}
</style>