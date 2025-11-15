<template>
    <div style="padding:10px 10px;height: 100%;" id="big_con">
        <div style="height: calc(100vh - 120px);border-radius: 4px;">
            <div class="from_con" id="from_con" style="display: flex; align-items: center; justify-content: space-between;">
                <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                    <div class="biaodan_input_con">
                        <el-form-item label="核算年度">
                            <el-date-picker class="form_input_style" v-model="queryParams.year" style="width: 120px;" 
                            value-format="yyyy" type="year" placeholder="选择年份" :clearable='false' />
                        </el-form-item>
                    </div>
                </el-form>
                <el-row :gutter="10" class="mb8 button_row">
                    <div class="heard-text" @click="handleJump">
                        访问全国碳行情市场
                        <i class="zhongtaiiconfont zhongtai-icon-shuangjiantou"></i>
                    </div>
                </el-row>
            </div>
            <div class="elbiaoge_elform" style="padding: 10px 20px 0">
                <div class="overview-list">
                    <div class="overview-item" v-for="(item, index) in overviewData" :key="index">
                        <div class="overview-item-left">
                            <div class="title">
                                {{ item.CarbonType }} ({{ item.Unit }})
                                <template v-if="item.tip">
                                    <el-tooltip class="item" :content="item.tip" placement="top">
                                        <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color:rgba(255, 255, 255, 0.6);font-size:12px;"></i>
                                    </el-tooltip>
                                </template>
                            </div>
                            <div class="number">{{ item.YearTotal }}</div>
                        </div>
                        <div class="overview-item-right">
                            <img src="@/assets/images/assetIcon.png" alt="">
                        </div>
                    </div>
                </div>
                <div class="carbom-tendency">
                   
                    <div class="tendency-chart">
                        <echartsLinkBar :modelData="tableData" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { selectCarbonAsset } from '@/api/energy/emissionCategory';
import echartsLinkBar from '@/views/carbonManage/carbonAssets/cmp/echartsLinkBar.vue';
export default {
    name: 'checkPlan',
    components: {
        echartsLinkBar,
    },
    data() {
        return {
            queryParams: {
                OrgId: this.$store.getters.orgId,
                year: moment().format('YYYY')
            },
            tableData: {},
            overviewData: []
        };
    },
    mounted() {
        this.getInitList();
        
    },
    methods: {
        // 获取数据
        getInitList() {
            selectCarbonAsset(this.queryParams).then(res => {
                this.tableData = res.data;
                const details = res.data.Details;
                // 提取碳排量（年度排放量）
                const annualEmission = res.data.CarbonEmissionTotal ? res.data.CarbonEmissionTotal : 0;
                // 提取配额总量
                const quotaTotalItem = details.find(item => item.CarbonType === '配额总量');
                const quotaTotal = quotaTotalItem ? quotaTotalItem.YearTotal : 0;
                // 提取CCER总量
                const ccerTotalItem = details.find(item => item.CarbonType === 'CCER总量');
                const ccerTotal = ccerTotalItem ? ccerTotalItem.YearTotal : 0;
                // 计算剩余排放量
                const remainingEmission = quotaTotal - annualEmission;
                // 整理overviewData
                this.overviewData = [
                    { CarbonType: '配额总量', Unit: 'tCO₂e', YearTotal: quotaTotal },
                    { CarbonType: 'CCER总量', Unit: 'tCO₂e', YearTotal: ccerTotal, tip: '国家核证温室气体自愿减排量' },
                    { CarbonType: '年度排放量', Unit: 'tCO₂e', YearTotal: annualEmission },
                    { CarbonType: '剩余排放量', Unit: 'tCO₂e', YearTotal: remainingEmission, tip: '剩余排放量=配额总量 - 年度排放量' }
                ];
  
            })
        },

        // 跳转
        handleJump() {
            window.open('https://www.cets.org.cn');
        }
    }
}
</script>
<style lang="less" scoped>
.heard-text {
    font-size: 14px;
    font-weight: 500;
    color: #3DB98F;
    display: flex;
    align-items: center;
    cursor: pointer;
    >i{
        font-size: 8px;
        color: #3DB98F;
        margin-left: 5px;
    }
}
.elbiaoge_elform{
    height: calc(100% - 70px);
}
.overview-list{
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    height: 102px;
    .overview-item{
        width: calc(25% - 6px);
        height: 100px;
        background: #222E40;
        border-radius: 4px;
        padding: 20px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        .overview-item-left{
            font-size: 14px;
            font-weight: 400;
            .title{
                color: rgba(255, 255, 255, 0.6);
            }
            .number{
                font-size: 32px;
                font-weight: 600;
                color: #fff;
                margin-top: 6px;
            }
        }
        .overview-item-right{
           width: 54px;
           height: 54px;
           >img{
            width: 100%;
            height: 100%;
           }
        }
    }
}
.carbom-tendency{
    width: 100%;
    height: calc(100% - 166px);
    margin-top: 64px;
    .tendency-chart{
        width: 100%;
        height: calc(100% - 20px);
    }
}
</style>