<template>
    <div>
        <el-form-item label="视频地址">
            <file-upload v-model="configData.chartOption.staticDataValue" :fileType="['mp4', 'mkv', 'avi']" :limit="1" />
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