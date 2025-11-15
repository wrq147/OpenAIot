<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con" style="display: flex; justify-content: space-between;">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item>
                        <el-date-picker class="form_input_style" v-model="queryParams.year" style="width: 120px;" 
                        value-format="yyyy" type="year" placeholder="选择年份" :clearable='false' />
                    </el-form-item>
                </div>
            </el-form>
            <el-row :gutter="10" class="mb8 button_row">
                <div>
                    <el-col v-if="!isEdit" :span="1.5">
                        <el-button type="success" plain @click="isEdit = !isEdit">
                            <i class="zhongtaiiconfont zhongtai-icon-xiugai1"></i>
                            <span style="margin-left:6px">编辑</span>
                        </el-button>
                    </el-col>
                    <el-col v-if="isEdit" :span="1.5">
                        <el-button type="info" plain @click="isEdit = !isEdit;getInitList()">
                            <span>取消</span>
                        </el-button>
                    </el-col>
                    <el-col v-if="isEdit" :span="1.5">
                        <el-button type="success" plain @click="handleSave">
                            <span>保存</span>
                        </el-button>
                    </el-col>
                </div>
            </el-row>
        </div>
        <div class="elbiaoge_elform" style="padding: 10px 20px 0">
            <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                <el-table-column label="产品名称" align="center" prop="CarbonType" />
                <el-table-column label="单位" align="center" prop="Unit" width="150" />
                <el-table-column label="数据粒度" align="center" prop="Granularity" width="100">
                    <template slot-scope="scope">
                        <div v-if="scope.row.CarbonType === '能耗强度' || scope.row.CarbonType === '碳排强度'">-</div>
                        <div v-else>
                            <div v-if="!isEdit">
                                {{ scope.row.Granularity }}
                            </div>
                            <el-select v-else class="set_radius" style="width: 80%;height: 32px;" v-model="scope.row.Granularity" placeholder="请选择">
                                <el-option label="按月" value="按月" />
                                <el-option label="按年" value="按年" />
                            </el-select>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column label="1月" align="center" prop="Month1" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month1" style="width: 80%;" placeholder="请输入" 
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month1 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="2月" align="center" prop="Month2" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month2" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month2 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="3月" align="center" prop="Month3" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month3" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month3 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="4月" align="center" prop="Month4" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month4" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month4 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="5月" align="center" prop="Month5" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month5" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month5 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="6月" align="center" prop="Month6" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month6" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month6 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="7月" align="center" prop="Month7" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month7" style="width: 80%;" placeholder="请输入" 
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month7 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="8月" align="center" prop="Month8" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month8" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month8 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="9月" align="center" prop="Month9" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month9" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month9 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="10月" align="center" prop="Month10" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month10" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month10 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="11月" align="center" prop="Month11" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month11" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month11 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="12月" align="center" prop="Month12" width="150">
                    <template slot-scope="scope">
                        <el-input v-if="scope.row.Granularity === '按月' && isEdit" v-model="scope.row.Month12" style="width: 80%;" placeholder="请输入"
                         @change="handleMonthChange(scope.row)" clearable />
                        <div v-else>{{ scope.row.Month12 }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="年度合计" align="center" prop="YearTotal" width="150" class-name="small-padding fixed-width fixed-class" fixed="right">
                    <template slot-scope="scope">
                        <div 
                            v-if="scope.row.CarbonType === '能耗强度' || scope.row.CarbonType === '碳排强度' "
                        >
                            {{ scope.row.YearTotal }}
                        </div>
                        <div v-else>
                            <div v-if="!isEdit || scope.row.Granularity === '按月'">{{ scope.row.YearTotal }}</div>
                            <el-input 
                                v-else 
                                v-model="scope.row.YearTotal" 
                                style="width: 80%;" 
                                placeholder="请输入" 
                                clearable 
                                @change="handleYearTotalChange(scope.row)"
                            />
                        </div>
                    </template>
                </el-table-column>
            </el-table>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { selectCarbonPlan, updateCarbonPlan } from '@/api/energy/emissionCategory';
export default {
    name: 'checkPlan',
    data() {
        return {
            isEdit: false,
            queryParams: {
                OrgId: this.$store.getters.orgId,
                year: moment().format('YYYY')
            },
            tableData: [],
            loading: false,
        };
    },
    mounted() {
        this.getInitList(); 
    },
    methods: {
        // 获取数据
        getInitList() {
            this.loading = true;
            selectCarbonPlan(this.queryParams).then(res => {
                // 1. 保存原始数据
                const originalDetails = res.data.Details;
                this.tableData = originalDetails.map(item => ({
                    ...item,
                    isOriginal: true // 标记为原始数据（非计算项）
                }));
                
                // 2. 提取计算所需的原始数据（能源需求、总产值、碳排量）
                const energyDemandItem = originalDetails.find(item => item.CarbonType === '能源需求');
                const totalOutputValueItem = originalDetails.find(item => item.CarbonType === '总产值');
                const carbonEmissionItem = originalDetails.find(item => item.CarbonType === '碳排量');

                // 3. 计算“能耗强度”并添加到tableData
                if (energyDemandItem && totalOutputValueItem) {
                    const energyIntensity = {
                        CarbonType: '能耗强度',
                        Unit: '-',
                        Granularity: '-',
                        isOriginal: false,
                        Month1: this.calcRatio(energyDemandItem.Month1, totalOutputValueItem.Month1),
                        Month2: this.calcRatio(energyDemandItem.Month2, totalOutputValueItem.Month2),
                        Month3: this.calcRatio(energyDemandItem.Month3, totalOutputValueItem.Month3),
                        Month4: this.calcRatio(energyDemandItem.Month4, totalOutputValueItem.Month4),
                        Month5: this.calcRatio(energyDemandItem.Month5, totalOutputValueItem.Month5),
                        Month6: this.calcRatio(energyDemandItem.Month6, totalOutputValueItem.Month6),
                        Month7: this.calcRatio(energyDemandItem.Month7, totalOutputValueItem.Month7),
                        Month8: this.calcRatio(energyDemandItem.Month8, totalOutputValueItem.Month8),
                        Month9: this.calcRatio(energyDemandItem.Month9, totalOutputValueItem.Month9),
                        Month10: this.calcRatio(energyDemandItem.Month10, totalOutputValueItem.Month10),
                        Month11: this.calcRatio(energyDemandItem.Month11, totalOutputValueItem.Month11),
                        Month12: this.calcRatio(energyDemandItem.Month12, totalOutputValueItem.Month12),
                        YearTotal: this.calcRatio(energyDemandItem.YearTotal, totalOutputValueItem.YearTotal)
                    };
                    this.tableData.push(energyIntensity);
                }

                // 4. 计算“碳排强度”并添加到tableData（假设核算基准值 = 总产值）
                if (carbonEmissionItem && totalOutputValueItem) {
                    const carbonIntensity = {
                        CarbonType: '碳排强度',
                        Unit: '-',
                        Granularity: '-',
                        isOriginal: false,
                        Month1: this.calcRatio(carbonEmissionItem.Month1, totalOutputValueItem.Month1),
                        Month2: this.calcRatio(carbonEmissionItem.Month2, totalOutputValueItem.Month2),
                        Month3: this.calcRatio(carbonEmissionItem.Month3, totalOutputValueItem.Month3),
                        Month4: this.calcRatio(carbonEmissionItem.Month4, totalOutputValueItem.Month4),
                        Month5: this.calcRatio(carbonEmissionItem.Month5, totalOutputValueItem.Month5),
                        Month6: this.calcRatio(carbonEmissionItem.Month6, totalOutputValueItem.Month6),
                        Month7: this.calcRatio(carbonEmissionItem.Month7, totalOutputValueItem.Month7),
                        Month8: this.calcRatio(carbonEmissionItem.Month8, totalOutputValueItem.Month8),
                        Month9: this.calcRatio(carbonEmissionItem.Month9, totalOutputValueItem.Month9),
                        Month10: this.calcRatio(carbonEmissionItem.Month10, totalOutputValueItem.Month10),
                        Month11: this.calcRatio(carbonEmissionItem.Month11, totalOutputValueItem.Month11),
                        Month12: this.calcRatio(carbonEmissionItem.Month12, totalOutputValueItem.Month12),
                        YearTotal: this.calcRatio(carbonEmissionItem.YearTotal, totalOutputValueItem.YearTotal)
                    };
                    this.tableData.push(carbonIntensity);
                }
                this.loading = false;
            })
        },

         // 月份数据变化时更新年度合计和计算项
        handleMonthChange(row) {
            if (!row.isOriginal) return;

            // 1. 重新计算年度合计（12个月求和）
            let yearTotal = 0;
            for (let i = 1; i <= 12; i++) {
                yearTotal += parseFloat(row[`Month${i}`]) || 0;
            }
            row.YearTotal = parseFloat(yearTotal.toFixed(2));

            // 2. 同步更新计算项
            this.updateCalculatedItems();
        },

        // 辅助方法：安全计算比值（避免除数为0）
        calcRatio(numerator, denominator) {
            if (denominator === 0 || isNaN(denominator) || denominator === undefined) {
                return 0; // 或返回null/空字符串，按业务需求
            }
            return parseFloat((numerator / denominator).toFixed(2)); // 保留两位小数
        },

        // 年度合计变化时，平分到每月
        handleYearTotalChange(row) {
            // 1. 确保年度合计是有效数字
            const yearTotal = parseFloat(row.YearTotal);
            if (isNaN(yearTotal)) {
                this.$message.warning("年度合计需为有效数字");
                return;
            }

            // 2. 计算每月平均值（保留两位小数）
            const monthlyAvg = parseFloat((yearTotal / 12).toFixed(2));

            // 3. 给每月数据赋值（Month1 ~ Month12）
            for (let i = 1; i <= 12; i++) {
                const monthKey = `Month${i}`;
                row[monthKey] = monthlyAvg;
            }

            // 关键：同步更新能耗强度和碳排强度
            this.updateCalculatedItems();
        },

        // 同步更新能耗强度和碳排强度
        updateCalculatedItems() {
            // 1. 重新获取最新的原始数据
            const energyDemandItem = this.tableData.find(item => item.CarbonType === '能源需求');
            const totalOutputValueItem = this.tableData.find(item => item.CarbonType === '总产值');
            const carbonEmissionItem = this.tableData.find(item => item.CarbonType === '碳排量');

            // 2. 找到计算项并更新
            const energyIntensityItem = this.tableData.find(item => item.CarbonType === '能耗强度');
            const carbonIntensityItem = this.tableData.find(item => item.CarbonType === '碳排强度');

            // 3. 更新能耗强度
            if (energyDemandItem && totalOutputValueItem && energyIntensityItem) {
                for (let i = 1; i <= 12; i++) {
                    const monthKey = `Month${i}`;
                    energyIntensityItem[monthKey] = this.calcRatio(
                        energyDemandItem[monthKey], 
                        totalOutputValueItem[monthKey]
                    );
                }
                energyIntensityItem.YearTotal = this.calcRatio(
                    energyDemandItem.YearTotal, 
                    totalOutputValueItem.YearTotal
                );
            }

            // 4. 更新碳排强度
            if (carbonEmissionItem && totalOutputValueItem && carbonIntensityItem) {
                for (let i = 1; i <= 12; i++) {
                    const monthKey = `Month${i}`;
                    carbonIntensityItem[monthKey] = this.calcRatio(
                        carbonEmissionItem[monthKey], 
                        totalOutputValueItem[monthKey]
                    );
                }
                carbonIntensityItem.YearTotal = this.calcRatio(
                    carbonEmissionItem.YearTotal, 
                    totalOutputValueItem.YearTotal
                );
            }

            // // 强制表格刷新
            // this.$nextTick(() => {
            //     this.tableData = [...this.tableData];
            // });
        },

        // 保存
        handleSave() {
            const params = {
                ...this.queryParams,
                details: this.tableData
            }
            updateCarbonPlan(params).then(res => {
                this.$message.success('保存成功');
                this.isEdit = false;
                this.getInitList();
            }).catch(err => {
                this.$message.error('保存失败');
            })
        }
    }
}
</script>
<style lang="less" scoped>
::v-deep{
    .el-select .el-input--suffix{
        height: 100%;
        padding-bottom: 2px;
        >input{
            height: 100%;
        }
        .el-input__icon{
            line-height: 32px;
        }
    }
    .el-input__inner{
       text-align: center;
    }
}
</style>