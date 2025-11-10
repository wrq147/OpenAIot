<template>
    <div>
        <el-form-item  label="流类型">
            <el-select v-model="configData.chartOption.staticDataValue.type" placeholder="请选择">
              <el-option label="rtmp类型" value="rtmp/mp4"></el-option>
              <el-option label="hls类型" value="application/x-mpegURL"></el-option>
            </el-select>
        </el-form-item>
        <el-form-item label="流地址">
            <el-input type="textarea" :rows="5" v-model="configData.chartOption.staticDataValue.src" />
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