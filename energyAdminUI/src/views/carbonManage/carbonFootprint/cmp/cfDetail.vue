<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="cfDetail">
            <el-tabs v-model="tabId">
                <el-tab-pane label="碳足迹结果" name="cfResult">
                    <cfResult :modelData="modelData" :tabId="tabId" />
                </el-tab-pane>
                <el-tab-pane label="生命周期建模" name="cfModel">
                    <cfModel :modelData="modelData" :tabId="tabId" />
                </el-tab-pane>
            </el-tabs>
            <i class="icon-close el-icon-close" @click="closeDialog" />
        </div>
    </div>
</template>
<script>
import { selectProductModelInfo } from '@/api/energy/cyclicalModel';
import cfResult from './cfResult.vue';
import cfModel from './cfModel.vue';
export default {
    name: '',
    components: {
        cfResult,
        cfModel,
    },
    data() {
        return {
            tabId: 'cfResult',
            modelData: {},
        };
    },
    methods: {
        // 打开详情
        openDialog(id) {
            this.tabId = 'cfResult';
            selectProductModelInfo({ Id: id }).then(res => {
                this.modelData = res.data;
            })
        },
        // 关闭详情
        closeDialog(){
            this.$emit('closeDialog');
        }
    }
}
</script>
<style lang="less" scoped>
.cfDetail{
    width: 100%;
    position: relative;
    .icon-close{
        position: absolute;
        top: 18px;
        right: 30px;
        font-size: 24px;
        color: #fff;
        cursor: pointer;
    }
}
::v-deep{
    .el-tabs{
        padding: 0 20px;
    }
    .el-tabs__header{
        margin: 0;
        .el-tabs__item{
            height: 56px;
            padding: 0 16px !important;
            font-weight: 400;
            font-size: 16px;
            line-height: 56px;
            color: rgba(255, 255, 255, 0.6) !important;
            position: relative;
        }
        .el-tabs__item.is-active{
            font-weight: 500;
            color: #FFFFFF !important;
            padding-left: 24px !important;
            &::before{
                content: '';
                position: absolute;
                left: 0;
                top: 50%;
                transform: translateY(-50%);
                background: url('~@/assets/images/zs.png') no-repeat;
                background-size: 100% 100%;
                width: 16px;
                height: 16px;
            }
        }
    }
    .el-tabs .el-tabs__header .el-tabs__nav .el-tabs__active-bar{
        height: 0;
    }
    .el-tabs .el-tabs__header .el-tabs__nav-wrap::after{
        background-color: rgba(255, 255, 255, 0.1);
        height: 1px;
    }
}
</style>