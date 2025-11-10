<template>
    <div>
        <el-form-item label="路径">
            <el-input v-model="configData.chartOption.staticDataValue" placeholder="请输入路径" />
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