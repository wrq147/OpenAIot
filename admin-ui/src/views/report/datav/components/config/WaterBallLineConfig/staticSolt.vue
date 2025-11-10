<template>
    <div>
        <el-form-item label="内容">
            <el-input v-model="configData.chartOption.staticDataValue[0].value" placeholder="内容" />
        </el-form-item>
        <el-form-item label="基准线">
            <el-input v-model="configData.chartOption.staticDataValue[0].line" placeholder="基准线" />
        </el-form-item>
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

    }
}
</script>
<style lang="scss" scoped>

</style>