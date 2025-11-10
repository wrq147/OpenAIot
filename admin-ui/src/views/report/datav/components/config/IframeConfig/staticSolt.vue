<template>
    <div>
        <el-form-item label="地址">
            <el-input v-model="configData.chartOption.staticDataValue" placeholder="请输入地址" />
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