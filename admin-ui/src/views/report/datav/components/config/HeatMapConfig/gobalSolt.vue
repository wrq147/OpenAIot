<template>
    <div v-if="tableData != ''">
        <el-table border :data="tableData" max-height="500" style="margin-top: 10px;">
            <el-table-column label="序号" type="index" align="center" :show-overflow-tooltip="true" />
            <el-table-column v-for="(column, index) in columnData" :key="index" :label="column.label"
                :prop="column.prop" align="center" :show-overflow-tooltip="true" />
        </el-table>
        <div>
            <div class="settingValue" v-for="(value, key) in staticDataValue" :key="key">
                <span>{{ showText(key) }}</span>
                <el-select v-model="modelValue[key]" filterable allow-create placeholder="值"
                    @change="changeCols(key, $event)">
                    <el-option v-for="(column, index) in columnData" :key="index" :value="column.label" />
                </el-select>
            </div>

        </div>
    </div>
</template>
<script>
export default {
    props: {
        costomData: {
            type: Object
        },
        themeForm: {
            type: Object
        }
    },
    watch: {
        configData: {
            deep: true,
            handler(newVal, oldVal) {
                if (this.isUpdatingFromCostomData) {
                    this.isUpdatingFromCostomData = false;
                    return;
                }
                this.$emit("costom-change", newVal);
            },
        },
        costomData: {
            deep: true,
            handler(newVal) {
                this.isUpdatingFromCostomData = true;
                this.configData = newVal;
                this.tableData = this.configData.chartOption.globalProcessor === null ? [] : this.tableData
                this.staticDataValue = this.configData.chartOption.staticDataValue[0]
                this.initResult()
            },
        },
    },
    data() {
        return {
            configData: this.costomData,
            modelValue: this.costomData.chartOption.tableSelectLine !== undefined ? this.costomData.chartOption.tableSelectLine : {},
            resultData: [],
            tableData: [],
            columnData: [],
            staticDataValue: [],
            isUpdatingFromCostomData: false
        }
    },
    methods: {
        showText(key) {
            if (key == 'yAxisData') {
                return 'y轴'
            } else if (key == 'xAxisData') {
                return 'x轴'
            } else if (key == 'data') {
                return '数值'
            } else {
                return key
            }
        },
        initResult() {
            let configData = this.configData
            this.themeForm.globalData.forEach((item, index) => {
                if (item.name === configData.chartOption.globalData) {
                    this.resultData = item.rawData !== undefined ? JSON.parse(item.rawData) : []
                    return
                }
            })
            this.filterData(configData.chartOption.globalProcessor)
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
            this.$set(this.configData.chartOption, "tableSelectLine", this.modelValue);
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep {
    .dataOrigin {
        font-size: 14px;
        color: red;
        margin-bottom: 10px;
    }
}

.settingValue span {
    display: block;
    margin: 15px 0;
    font-size: 14px;
}
</style>