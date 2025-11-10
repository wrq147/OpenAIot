<template>
    <div class="com-setting-right">
        <el-tabs v-model="currentTab" class="center-tabs">
            <el-tab-pane label="数据集" name="data">
                <data-integration :themeForm="themeForm" :drawingList="drawingList" :tableType="'spreadSheet'" />
            </el-tab-pane>
            <el-tab-pane label="节点" name="level">
                <fieldFormat :configData="themeForm" @fieldData="fieldData" @getKeyList="getKeyList" />
            </el-tab-pane>
        </el-tabs>
    </div>
</template>
<script>
import dataIntegration from "../datav/LayerItems/dataIntegration.vue";
import fieldFormat from "./fieldFormat.vue";
export default { 
    name: 'information',
    components: {
        dataIntegration,
        fieldFormat
    },
    props: {
        themeForm: {
            type: Object,
            default: () => ({})
        },
        drawingList: {
            type: Array,
            default: () => []
        }
    },
    data() {
        return {
            currentTab: "data",
        }
    },
    methods: {
        fieldData(item) {
            this.$emit('fieldData', item)
        },
        getKeyList(data, type, filtrationId) {
            this.$emit('getKeyList', data, type, filtrationId)
        }
    }
}
</script>
<style lang="scss" scoped>
::v-deep{
    .el-tabs__nav{
        width: 100%;
    }
    .el-tabs__item{
        width: 50%;
        text-align: center;
    }
}
.com-setting-right{
    width: 236px;
    border-left: 1px solid #eeeff0;
}
</style>