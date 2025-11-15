<template>
    <div class="cfModel">
        <div class="cfModel-info">
            <div class="cfModel-info-title">统计时间<span>({{ formatDate(modelData.BeginDate) }} ~ {{ formatDate(modelData.EndDate) }})</span></div>
             <div class="model-table">
                <div class="table-header">
                    <div class="table-header-item" v-for="(item, index) in tableHeader" :key="index">
                        {{ item.name }}
                        <span v-if="item.prop">{{ item.prop }}</span>
                    </div>
                </div>
                <div class="table-content">
                    <div class="table-content-item" v-for="(item, rowIndex) in modelDataList" :key="rowIndex">
                        <div class="table-content-item-name">{{ item.LinkName }}</div>
                        <div class="table-content-item-name">
                            <div class="consumable-container">
                                <div class="consumable-list" v-for="(process, pIdx) in item.Processes" :key="pIdx">
                                    <div class="consumable-item" v-for="(shuRu, sIdx) in process.shuRus" :key="sIdx">
                                        <div class="consumable-item-content" :id="`input-item-${rowIndex}-${pIdx}-${sIdx}`">
                                            <div class="consumable-name">
                                                {{ shuRu.TypeName }}：<div>{{ shuRu.UseVale }}<span>{{ shuRu.Unit }}</span></div>
                                            </div>
                                                <!-- D3会在这里生成曲线 -->
                                            <div v-if="process.shuRus.length > 0" class="curve-container" :id="`curve-${rowIndex}-${pIdx}-${sIdx}`"></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="Materials-list">
                                    <div class="Materials-item" v-for="(process, pIdx) in item.Materials" :key="pIdx">
                                        <div class="Materials-item-content">
                                            <div class="Materials-name">
                                                {{ process.MaterialName }}：
                                                <div style="display: flex; align-items: center;">
                                                    {{ process.BomType }}<span>{{ process.CarbonUnit }}</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div v-if="item.Processes.length === 0 && item.Materials.length === 0">-</div>
                            </div>
                        </div>
                        <div class="table-content-item-name process-column" :id="`process-col-${rowIndex}`">
                            <div class="process-list">
                                <div class="process-item-box" v-for="(k, pIdx) in item.Processes" :key="pIdx">
                                    <div class="process-item" :id="`process-item-${rowIndex}-${pIdx}`" data-is-process="true">
                                        <div class="process-item-info">
                                            <div class="tag">工序<span>{{ k.ProcessNo }}</span></div>
                                            <div class="name">{{ k.ProcessName }}</div>
                                        </div>
                                    </div>
                                </div>
                                <div v-if="item.Processes.length === 0">-</div>
                            </div>
                        </div>
                        <div class="table-content-item-name">-</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { modelSelectLink } from '@/api/energy/cyclicalModel';
import { renderCurves } from '../../cyclicalModel/cmp/modelRenderLine.js'
export default {
    name: 'cfModel',
    props: {
        modelData: {
            type: Object,
            default: () => {},
        },
        tabId: {
            type: String,
            default: 'cfModel',
        },
    },
    data() {
        return {
            tableHeader: [
                {
                    name: '环节'
                },
                {
                    name: '输入',
                    prop: '原料/耗材/能源/资源'
                },
                {
                    name: '-'
                },
                {
                    name: '输出',
                    prop: '废料/废气/废水'
                },
            ],
            modelDataList: []
        };
    },
    watch: {
        tabId: {
            handler(newVal, oldVal) {
                if (newVal === 'cfModel') {
                    this.tableHeader[2].name = this.modelData.ProductName;
                    this.getLinkList(this.modelData.ProductBorder);
                }
            },
            deep: true
        }
    },
    methods: {
        // 时间格式化
        formatDate(date) {
            return moment(date).format('YYYY/MM/DD');
        },
        getLinkList(ProductBorder) {
            modelSelectLink({ ProductBorder }).then(res => {
                // 1. 获取原始数据和modelData中的Processes
                const linkList = res.data || [];
                const modelProcesses = this.modelData.Processes || [];
                const modelMaterials = this.modelData.Materials || [];
                // 2. 为每个link项添加Processes字段（匹配LinkId）
                const modelDataList = linkList.map(linkItem => {
                    // 筛选出与当前linkItem.LinkId匹配的工序
                    const matchedProcesses = modelProcesses.filter(
                        process => process.LinkId === linkItem.LinkId
                    );
                    // 筛选出与当前linkItem.LinkId匹配的原材料
                    const matchedMaterials = modelMaterials.filter(
                        material => material.LinkId === linkItem.LinkId
                    );
                    
                    // 返回新对象：原linkItem + 匹配的Processes
                    return {
                        ...linkItem,
                        Processes: matchedProcesses, // 新增Processes字段
                        Materials: matchedMaterials // 新增Materials字段
                    };
                });
                
                // 3. 输出结果（可根据需要赋值给data中的变量）
                this.modelDataList = modelDataList;
                setTimeout(() => {
                    renderCurves(modelDataList);
                }, 300)
            })
        }
    }
}
</script>
<style lang="less" scoped>
.cfModel{
    margin-top: 20px;
    width: 100%;
    height: 100%;
    .cfModel-info{
        width: 100%;
        padding-bottom: 20px;
        .cfModel-info-title{
            font-weight: 500;
            font-size: 14px;
            color: #FFFFFF;
            >span {
                font-weight: 400;
                font-size: 14px;
                color: rgba(255, 255, 255, 0.6);
                margin-left: 8px;
            }
        }
    }
}
.model-table{
    margin-top: 16px;
    width: 960px;
    border-left: 1px solid rgba(255, 255, 255, 0.1);
    border-top: 1px solid rgba(255, 255, 255, 0.1);
    .table-header{
        display: flex;
        align-items: center;
        height:64px;
        background: #222E40;
        .table-header-item{
            width: 260px;
            height: 100%;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            font-weight: 400;
            font-size: 14px;
            color: #FFFFFF;
            border-right: 1px solid rgba(255, 255, 255, 0.1);
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            >span{
                font-size: 12px;
                color: rgba(255, 255, 255, 0.6);
                margin-top: 6px;
            }
            &:first-child{
                width: 180px;
            }
        }
    }
    .table-content{
        width: 100%;
        background-color: #19212D;
        .table-content-item{
            min-height: 64px;
            display: flex;
            align-items: stretch;
            position: relative;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            // overflow: hidden;
            >div{
                width: 260px;
                padding: 24px 0;
                font-weight: 400;
                font-size: 14px;
                color: #FFFFFF;
                border-right: 1px solid rgba(255, 255, 255, 0.1);
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                &:first-child{
                    width: 180px;
                }
                &:nth-child(3){
                    padding: 4px 0;
                }
            }
        }
    }
}
.consumable-container{
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}
.consumable-list, .Materials-list{
    width: 100%;
    &:not(:last-child){
        margin-bottom: 10px;
    }
    .consumable-item, .Materials-item  {
        margin-bottom: 10px;
        width: 100%;
        display: flex;
        flex-direction: column; 
        align-items: center;
        justify-content: center;
        .consumable-item-content, .Materials-item-content{
            // 作为SVG的定位容器
            display: flex;
            align-items: center; // 耗材名称和曲线垂直居中
            gap: 10px; // 名称和曲线之间的间距
            width: 210px;
            height: 36px;
            border-radius: 20px;
            border: 1px solid #3DB98F;
            padding: 0 12px;
            // 耗材名称样式（可选，根据你的设计调）
            .consumable-name, .Materials-name {
                width: 100%;
                font-weight: 500;
                color: #fff;
                font-size: 14px;
                display: flex;
                align-items: center;
                justify-content: space-between;
                >div{
                    display: flex;
                    align-items: center;
                    >span {
                        font-weight: 400;
                        color: rgba(255, 255, 255, 0.6);
                        margin-left: 10px;
                    }
                }
            }

            .curve-container {
                position: absolute;
                right: 0;
                top: 0;
                width: 100%;
                height: 100%;
                pointer-events: none;
                background-color: transparent;
            }
        }
        .Materials-item-content{
            border: 1px solid #35C8FF;
        }
    }
    
}
.process-list {
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column; 
    align-items: center;
    justify-content: center;
    .process-item-box{
        height: 104px;
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        position: relative;
        &:not(:last-child)::after {
            content: '';
            position: absolute;
            right: 50%;
            bottom: 0;
            transform: translateX(50%);
            width: 10px;
            height: 32px;
            background: url('~@/assets/images/lineBom.png') no-repeat center center;
            background-size: 100% 100%;
        }
         &:not(:first-child)::before {
           content: '';
            position: absolute;
            right: 50%;
            top: 0;
            transform: translateX(50%);
            width: 10px;
            height: 32px;
            background: url('~@/assets/images/lineArrow.png') no-repeat center center;
            background-size: 100% 100%; 
        }
    }
    .process-item{
        width: 210px;
        height: 40px;
        display: flex;
        align-items: center;
        background: #3DB98F;
        border-radius: 20px;
        padding: 0 12px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        .process-item-info{
            width: 100%;
            display: flex;
            align-items: center;
            .tag{
                font-weight: 500;
                font-size: 14px;
                color: #fff;
                padding-right: 12px;
                position: relative;
                display: flex;
                align-items: center;
                >span{
                    width: 16px;
                    height: 16px;
                    background: #FFFFFF;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 12px;
                    color: #3DB98F;
                    margin-left: 6px;
                }
                &::after{
                    content: '';
                    position: absolute;
                    right: 0;
                    top: 50%;
                    transform: translateY(-50%);
                    width: 1px;
                    height: 14px;
                    background: #FFFFFF;
                }
            }
            .name{
                font-weight: 500;
                font-size: 14px;
                color: #fff;
                margin-left: 20px;
            }
        }
    }
}
</style>