<template>
     <div>
        <el-form-item label="静态轮播图">
            <image-upload :value="CuDatas" @input="submitRotation" :limit="10"></image-upload>
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
    computed: {
        CuDatas(){
            let vals = this.configData.chartOption.staticDataValue.map(x=>x.value);
            return vals.join(',');
        }
    },
    data() {
        return {
            
        }
    },
    methods: {
        //确认选择图片
        submitRotation(vals){
            let result = [];
            let valarr = vals.split(',');
            valarr.forEach(element => {
                result.push({ "value": element })
            });
            this.$set(this.configData.chartOption, 'staticDataValue', JSON.parse(JSON.stringify(result)));
        },
    }
}
</script>
<style lang="scss" scoped>

</style>