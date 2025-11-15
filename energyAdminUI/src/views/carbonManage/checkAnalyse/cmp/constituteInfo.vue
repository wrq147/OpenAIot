<template>
    <div class="constituteInfo">
        <div class="heard-title">构成分析</div>
        <div class="constituteInfo-content">
            <el-collapse class="constituteInfo-collapseFather">
                <el-collapse-item v-for="(item, index) in currentPeriodData" :key="index">
                    <template slot="title">
                        <div class="heard-info">
                            <i class="zhongtaiiconfont zhongtai-icon-xiayiye collapse-icon"></i>
                            <div class="heard-info-content">
                                <div :style="{ backgroundColor: getColor(index) }"></div>
                                <div class="heard-info-content-text">
                                    {{ item.ClassName }}
                                    <div>碳排总量 <span>{{ currentSum.carbonByTopClass[item.ClassName] }}</span> tCO₂e，占比 <span>{{ getTotalPro(item.ClassName) }}</span> %</div>
                                </div>
                            </div>
                        </div>
                    </template>
                    
                    <el-collapse class="constituteInfo-collapseSon">
                        <el-collapse-item v-for="(v, In, index) in item.OrgClasses" :key="index">
                            <template slot="title">
                                <div class="heard-info">
                                    <div style="display: flex; align-items: center;">
                                        <i data-v-798a30d9="" class="zhongtaiiconfont zhongtai-icon-xiayiye collapse-icon"></i>
                                        <span style="margin-left: 10px;">{{ In }}</span>
                                    </div>
                                    <div class="heard-info-content-text">碳排总量 <span>{{ currentSum.carbonBySubClass[In] }}</span> tCO₂e
                                        <div class="heard-info-content-text-pro">
                                            占比 
                                            <div class="heard-info-content-text-pro-line">
                                                <div :style="{ width: getTotalProSon(In, item.ClassName) + '%' }" class="heard-info-content-text-pro-line-son"></div>
                                            </div>
                                            <span> {{ getTotalProSon(In, item.ClassName) }} %</span>
                                        </div>
                                    </div>
                                </div>
                            </template>
                            <el-table :data="v" class="data_table" style="width:100%;">
                                <el-table-column label="设施/活动名称" align="center" prop="FacilityName" />
                                <el-table-column label="排放源" align="center" prop="FactorName" />
                                <el-table-column label="消耗量" align="center" prop="UseVale" />
                                <el-table-column label="碳排放量(tCO₂e)" align="center" prop="CarbonEmission" />
                                <el-table-column label="碳排占比" align="center">
                                    <template slot-scope="scope">
                                      <div class="heard-info-content-text-pro">
                                            <div class="heard-info-content-text-pro-line">
                                                <div :style="{ width: getTotalProSonCarbon(scope.row.CarbonEmission, In) + '%', backgroundColor: '#F5BC24' }" class="heard-info-content-text-pro-line-son"></div>
                                            </div>
                                            <span> {{ getTotalProSonCarbon(scope.row.CarbonEmission, In) }} %</span>
                                        </div>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </el-collapse-item>
                    </el-collapse>
                </el-collapse-item>
            </el-collapse>
        </div>
    </div>
</template>
<script>

export default {
    name: 'constituteInfo',
    props: {
        currentPeriodData: {
            type: Array,
            default: () => []
        },
        currentSum: {
            type: Object,
            default: () => {}
        },
        typeColor: {
            type: Array,
            default: () => []
        }
    },
    data() {
        return {
            
        };
    },
    methods: {
        // 获取颜色
        getColor(index) {
            return this.typeColor[index]
        },
        // 获取顶层类别碳排占总碳排的百分比
        getTotalPro(name) {
            // 安全获取顶层类别碳排值（默认0）
            const topClassCarbon = Number(this.currentSum?.carbonByTopClass?.[name]) || 0;
            // 安全获取总碳排值（默认0）
            const totalCarbon = Number(this.currentSum?.totalCarbon) || 0;
            
            // 处理总碳排为0的情况（避免除以0）
            if (totalCarbon <= 0) {
                return '0.00';
            }
            
            // 计算百分比并保留2位小数
            const ratio = (topClassCarbon / totalCarbon * 100).toFixed(2);
            
            // 确保结果有效（防止极端情况下的NaN）
            return isNaN(Number(ratio)) ? '0.00' : ratio;
        },

        // 获取子类别碳排占总碳排的百分比
        getTotalProSon(name, topName){
            // 安全获取顶层类别碳排值（默认0）
            const subClassCarbon = Number(this.currentSum?.carbonBySubClass?.[name]) || 0;
            // 安全获取总碳排值（默认0）
            const totalCarbon = Number(this.currentSum?.carbonByTopClass?.[topName]) || 0;
            
            // 处理总碳排为0的情况（避免除以0）
            if (totalCarbon <= 0) {
                return '0.00';
            }
            
            // 计算百分比并保留2位小数
            const ratio = (subClassCarbon / totalCarbon * 100).toFixed(2);
            
            // 确保结果有效（防止极端情况下的NaN）
            return isNaN(Number(ratio)) ? '0.00' : ratio;
        },

        // 获取子类别数据碳排占总碳排的百分比
        getTotalProSonCarbon(value, name) {
            // 安全获取顶层类别碳排值（默认0）
            const subClassCarbon = Number(this.currentSum?.carbonBySubClass?.[name]) || 0;
            // 处理总碳排为0的情况（避免除以0）
            if (subClassCarbon <= 0) {
                return '0.00';
            }
            // 计算百分比并保留2位小数
            const ratio = (Number(value) / subClassCarbon * 100).toFixed(2);
            
            // 确保结果有效（防止极端情况下的NaN）
            return isNaN(Number(ratio)) ? '0.00' : ratio;
        }

    }
}
</script>
<style lang="less" scoped>
.constituteInfo{
    padding: 16px;
    .heard-title{
        font-weight: 500;
        font-size: 14px;
        color: #FFFFFF;
        padding-left: 12px;
        position: relative;
        &::before{
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
    .constituteInfo-content{
        width: 100%;
        margin-top: 16px;
    }
}
::v-deep .constituteInfo-collapseFather{
    border: 0;
    .el-collapse-item__header{
        background: #222E40;
        height: 48px;
        line-height: 48px;
        border: 0;
        border-radius: 4px;
        color: #fff;
        justify-content: flex-start;
        margin-bottom: 16px;
        width: 100%;
        .el-collapse-item__arrow{
            display: none;
        }
    }
    .el-collapse-item__wrap{
        background: #19212D;
        border: 0;
        
    }
}
::v-deep .constituteInfo-collapseSon{
    border: 0;
    .el-collapse-item__header{
        background: #19212D;
        height: 28px;
        line-height: 28px;
        border: 0;
        border-radius: 0;
        color: #fff;
        justify-content: flex-start;
        margin-bottom: 0;
        width: 100%;
        .el-collapse-item__arrow{
            display: none;
        }
        .heard-info{
            justify-content: space-between;
            .heard-info-content-text{
                padding-right: 16px;
                font-weight: 400;
                display: flex;
                align-items: center;
                >span{
                    color: #35C8FF;
                    margin: 0 4px;
                }
                
            }
        }
    }
}
.heard-info-content-text-pro{
    display: flex;
    align-items: center;
    margin-left: 24px;
    .heard-info-content-text-pro-line{
        width: 160px;
        height: 4px;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 4px;
        position: relative;
        margin: 0 8px;
        .heard-info-content-text-pro-line-son{
            position: absolute;
            left: 0;
            top: 0;
            width: 10%;
            height: 4px;
            background: #35C8FF;
            border-radius: 4px;
        }
    }
}
::v-deep .heard-info{
    padding-left: 18px;
    height: 48px;
    display: flex;
    align-items: center;
    width: 100%;
    .collapse-icon{
        font-size: 10px;
        color: rgba(255, 255, 255, 0.6);
        transition: transform 0.3s ease; // 添加平滑过渡
    }
}
::v-deep .el-collapse-item__header.is-active .heard-info .collapse-icon{
    transform: rotate(90deg) !important;
}
.heard-info-content{
    display: flex;
    align-items: center;
    font-weight: 500;
    font-size: 14px;
    color: #FFFFFF;
    margin-left: 10px;
    width: 100%;
    >div{
        width: 12px;
        height: 12px;
        margin-right: 8px;
        display: flex;
        align-items: center;
    }
    .heard-info-content-text{
        width: 100%;
        font-weight: 400;
        >div{
            margin-left: 8px;
            >span{
                color: #3DB98F;
            }
        }
    }
}
</style>
