<template>
    <div>
        <el-form-item label="图片地址">
            <image-upload v-model="configData.chartOption.staticDataValue" :limit="1"></image-upload>
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