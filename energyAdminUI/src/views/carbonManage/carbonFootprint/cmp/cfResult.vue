<template>
    <div class="cfResult">
        <div class="cfResult-info">
            <div class="result_title">产品信息</div>
            <div class="cfResult-info-contain">
                <div class="left">
                    <div class="content-info">
                        <div class="title">产品名称</div>
                        <div class="content">{{ modelData.ProductName }}</div>
                    </div>
                    <div class="content-info">
                        <div class="title">产品类型</div>
                        <div class="content">{{ modelData.ProductType === '1' ? '成品' : '半成品' }}</div>
                    </div>
                    <div class="content-info">
                        <div class="title">产品型号</div>
                        <div class="content">{{ modelData.ProductModel }}</div>
                    </div>
                    <div class="content-info">
                        <div class="title">产品产量</div>
                        <div class="content">{{ modelData.OutPut }} {{ modelData.Unit }}</div>
                    </div>
                    <div class="content-info">
                        <div class="title">数据统计时段</div>
                        <div class="content">{{ formatDate(modelData.BeginDate) }} - {{ formatDate(modelData.EndDate) }}</div>
                    </div>
                    <div class="content-info">
                        <div class="title">生命周期边界</div>
                        <div class="content">{{ modelData.BorderTitle }}</div>
                    </div>
                </div>
                <div class="right">
                    <div class="icon-contain">
                        <div class="icon-contain-info">
                            <div class="title">产品碳足迹</div>
                            <div class="desc">产品生命周期内产生的温室气体排放量</div>
                            <div class="num">{{ averageCarbon }}</div>
                            <div class="unit">kgCO₂e/台</div>
                        </div>
                        <img class="bgIcon" src="@/assets/images/zuji.png" alt="">
                    </div>
                    <img class="iconShow" src="@/assets/images/zujiIcon.png" alt="">
                </div>
            </div>
        </div>
        <div class="cfResult-container">
            <div class="cfResult-container-heard">
                <div class="result_title">碳足迹排行<span>(单位：kgCO₂e/台)</span></div>
                <el-select popper-class="search-data-select" v-model="dataType" placeholder="请选择">
                    <el-option label="按环节" value="link" />
                    <el-option label="按工序" value="process" />
                </el-select>
            </div>
            <div class="cfResult-container-content">
                <echartsLinkBar v-show="dataType === 'link'" :dataType="dataType" :modelData="modelData" />
                <echartsProcessBar v-show="dataType === 'process'" :dataType="dataType" :modelData="modelData" />
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import echartsLinkBar from './echartsLinkBar.vue';
import echartsProcessBar from './echartsProcessBar.vue';
export default {
    name: 'cfResult',
    props: {
        modelData: {
            type: Object,
            default: () => {}
        },
        tabId: {
            type: String,
            default: 'cfResult',
        },
    },
    components: {
        echartsLinkBar,
        echartsProcessBar
    },
    data() {
        return {
            dataType: 'link',
            modelDataList: [],
            averageCarbon: 0,
        };
    },
    watch: {
        modelData: {
            handler() {
                this.getCarbonEmission();
            },
            deep: true
        },
        tabId: {
            handler(newVal) {
                if (newVal === 'cfResult') {
                    this.dataType = 'link';
                    this.getCarbonEmission();
                }
            }
        }
    },
    methods: {
        // 时间格式化
        formatDate(date) {
            return moment(date).format('YYYY/MM/DD');
        },
        getCarbonEmission() {
            // 1. 解构获取所需数据（默认空值避免报错）
            const { Processes = [], OutPut = 0 } = this.modelData || {};
            
            // 2. 计算所有ProcessItems的CarbonEmission总和
            const totalCarbon = Processes.reduce((sum, process) => {
                // 累加当前工序下所有ProcessItems的CarbonEmission
                const processTotal = (process.ProcessItems || []).reduce((itemSum, item) => {
                    // 确保CarbonEmission是数字，默认0
                    return itemSum + (Number(item.CarbonEmission) || 0);
                }, 0);
                return sum + processTotal;
            }, 0);
            
            // 3. 计算平均值（处理除以0的情况）
            let avgCarbon = 0;
            if (OutPut > 0) {
                avgCarbon = totalCarbon / OutPut;
            } else {
                this.averageCarbon = 0;
            }
            
            // 4. 保留两位小数（四舍五入）
            const result = parseFloat(avgCarbon.toFixed(2));
            
            this.averageCarbon = result;
        }
    }
}
</script>
<style lang="less" scoped>
.cfResult{
    margin-top: 20px;
    width: 100%;
    height: 100%;
}
.cfResult-info{
    width: 100%;
    .cfResult-info-contain{
        margin-top: 20px;
        width: 100%;
        display: flex;
        justify-content: space-between;
        .left{
            width: calc(100% - 452px);
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            .content-info{
                display: flex;
                align-items: center;
                justify-content: flex-end;
                width: calc(50% - 25px);
                min-width: 342px;
                margin-bottom: 24px;
                .title{
                    font-weight: 400;
                    font-size: 14px;
                    color: rgba(255, 255, 255, 0.6);
                }
                .content{
                    height: 36px;
                    width: calc(100% - 120px);
                    min-width: 222px;
                    font-weight: 400;
                    font-size: 14px;
                    color: #FFFFFF;
                    background: #222E40;
                    border-radius: 4px;
                    line-height: 36px;
                    padding-left: 10px;
                    margin-left: 16px;
                }
            }
        }
        .right{
            width: 400px;
            height: 156px;
            display: flex;
            justify-content: flex-end;
            position: relative;
            .icon-contain{
                width: 320px;
                height: 100%;
                background: #222E40;
                border-radius: 60px 0px 60px 0px;
                display: flex;
                justify-content: flex-end;
                position: relative;
                z-index: 2;
                .icon-contain-info{
                    width: 226px;
                    height: 100%;
                    padding-right: 22px;
                    display: flex;
                    flex-direction: column;
                    justify-content: center;
                }
                .title{
                    font-weight: 600;
                    font-size: 16px;
                    color: #FFFFFF;
                }
                .desc{
                    font-weight: 400;
                    font-size: 12px;
                    color: rgba(255, 255, 255, 0.6);
                    margin-top: 10px;
                }
                .num{
                    font-weight: 600;
                    font-size: 36px;
                    color: #3DB98F;
                    margin-top: 6px;
                }
                .unit{
                    font-weight: 400;
                    font-size: 14px;
                    color: rgba(255, 255, 255, 0.6);
                }
                .bgIcon{
                    position: absolute;
                    width: 162px;
                    height: 120px;
                    right: 40px;
                    top: 50%;
                    transform: translateY(-50%);
                    z-index: 1;
                }
            }
            .iconShow{
                position: absolute;
                width: 150px;
                height: 150px;
                left: 0;
                top: 50%;
                transform: translateY(-50%);
                z-index: 2;
            }
        }
    }
}
.cfResult-container{
    width: 100%;
    height: calc(100% - 300px);
    margin-top: 44px;
    .cfResult-container-heard{
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }
    .cfResult-container-content{
        width: 100%;
        height: 390px;
        margin-top: 20px;
    }
}
.result_title{
    font-weight: 500;
    font-size: 14px;
    color: #FFFFFF;
    position: relative;
    padding-left: 12px;
    >span{
        font-weight: 500;
        font-size: 12px;
        color: rgba(255, 255, 255, 0.6);
        margin-left: 8px;
    }
    &::after{
        content: '';
        position: absolute;
        left: 0;
        top: 50%;
        transform: translateY(-50%);
        width: 4px;
        height: 14px;
        background: #3DB98F;
        border-radius: 2px;
    }
}
::v-deep .el-select{
    width: 76px;
    height: 30px;
    .el-input__inner{
        border: 0;
        height: 30px;
        line-height: 30px;
        font-weight: 400;
        font-size: 12px;
    }
    .el-input__icon{
        line-height: 30px !important;
    }
}
.el-select-dropdown.el-popper .el-select-dropdown__item.selected::after{
    content: '';
}
</style>