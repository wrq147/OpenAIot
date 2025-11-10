<template>
    <div style="padding:20px 20px;height:100%">
        <div class="sales_config_con">
            <div class="sales_config_li">
                <div class="sales_title">
                    <div style="font-size: 18px;">阶段回退</div>
                    <div><el-button type="primary" v-loading="saveloading" @click="onSave">保存设置</el-button></div>
                </div>
                
                <div class="config_val">
                    <el-row>
                        <el-checkbox v-model="setCrmForm.EnableIngBack">进行中阶段回退</el-checkbox>
                    </el-row>
                    <el-row>
                        <el-checkbox v-model="setCrmForm.EnableWinBack">终点阶段回退</el-checkbox>
                    </el-row>
                </div>
            </div>
            <div class="sales_config_li">
                <div class="label">跨阶段推进</div>
                <div class="config_val">
                    <el-switch v-model="setCrmForm.EnableKPer" active-color="#328BEB" inactive-color="#BFBFBF">
                    </el-switch>
                </div>
            </div>
            <div class="sales_config_li">
                <div class="label">未跟进回收天数</div>
                <div class="config_val">
                    <el-row style="padding: 5px 0;" class="flex_box">
                        <el-input-number v-model="setCrmForm.FollowReturnDay" @change="handleChange"
                            :min="0"></el-input-number>

                    </el-row>
                </div>
            </div>

            <div class="sales_config_li">
                <div class="label" style="margin-top: 20px;">销售阶段列表</div>
                <div><el-button @click="addSales">添加阶段</el-button></div>
                <el-table border class="salesConfigTable" v-loading="tbloading" :data="salesList" row-key="Id"
                    style="width:100%">
                    <el-table-column label="销售阶段" align="center" width="100">
                        <template slot-scope="scope">
                            {{ scope.row.PeriodName }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="Probability" align="center" label="赢率" width="100">
                        <template slot-scope="scope">
                            {{ scope.row.Probability + '%' }}
                        </template>
                    </el-table-column>
                    <el-table-column label="阶段类型" align="center" width="100" prop="PeriodType">
                        <template slot-scope="scope">
                            {{ scope.row.PeriodType == 'ing' ? '进行中' : scope.row.PeriodName }}
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" align="center">
                        <template v-slot:default="scope">
                            <el-button class="handle" size="mini" v-if="scope.row.PeriodType == 'ing'"><i
                                    class="el-icon-rank" /> 移动</el-button>
                            <el-button @click="openEditSales(scope.row)" size="mini" v-if="scope.row.PeriodType == 'ing'"><i
                                    class="el-icon-edit" />
                                修改</el-button>
                            <el-button type="danger" @click="delGrade(scope.row)" size="mini"
                                v-if="scope.row.PeriodType == 'ing'"><i class="el-icon-delete" /> 删除</el-button>
                        </template>
                    </el-table-column>
                </el-table>
            </div>
        </div>
        <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="salesopen" width="500px">
            <el-form ref="salesForm" :model="salesForm" :rules="salesrules" label-width="120px">
                <el-form-item label="级别名称" prop="periodName">
                    <el-input v-model="salesForm.periodName" placeholder="请输入代理级别名称" maxlength="20" />
                </el-form-item>

                <el-form-item label="成交几率" prop="probability">
                    <el-input v-model="salesForm.probability" placeholder="请输入预计成交几率" type="number" />
                </el-form-item>
            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="onGradeSubmit" v-loading="saveGradeloading">确 定</el-button>
                <el-button @click="salesopen = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
import {
    periodList,
} from "@/api/crm/opport";
import Sortable from 'sortablejs';
import {
    setCrmConfig,
    getCrmConfig,
    editPeriod,
    addPeriod,
    removePeriod
} from "@/api/crm/config";
export default {
    name: 'AdminUiCustomConfig',

    data() {
        return {
            setCrmForm: {
                EnableIngBack: false,// 进行中阶段回退
                EnableWinBack: false,// 终点阶段回退
                EnableKPer: false,// 跨阶段推进
                FollowReturnDay: 0// 设置几天未跟进将被回收，为0不回收
            },
            tbloading: false,
            salesList: [],//销售阶段列表
            salesForm: {
                orgId: 0,
                periodName: '',
                periodType: '',
                probability: 0,
                sort: 0
            },
            // 表单校验
            salesrules: {
                periodName: [
                    { required: true, message: '请输入代理级别名称', trigger: "blur" }
                ],
                probability: [
                    { required: true, message: '请输入预计成交几率', trigger: "blur" }
                ]
            },
            salesopen: false,//修改弹窗
            title: '添加销售阶段',
            saveloading: false,
            saveGradeloading: false
        };
    },

    mounted() {
        this.getCrmConfig()
        this.getPeriodList()
        this.rowInitDrop()
    },

    methods: {
        addSales() {
            //添加阶段
            this.salesopen = true
            this.title = '添加销售阶段'
            this.salesForm = {
                orgId: 0,
                periodName: '',
                periodType: 'ing',
                probability: 0,
                sort: 0
            }
        },
        async onSave() {
            //保存crm相关设置
            let result = await setCrmConfig(this.setCrmForm)
            this.saveloading = true;
            this.$modal.msgSuccess("保存成功");
            this.saveloading = false;
        },
        getCrmConfig() {
            //获取crm客户相关设置   id等于0表示当前企业的CRM设置
            getCrmConfig(0).then(res => {
                console.log('当前企业的相关设置', res);
                this.setCrmForm = res.data
            })
        },
        onGradeSubmit() {
            if (this.salesForm.Id) {
                editPeriod(this.salesForm).then(res => {
                    console.log(res);
                    this.saveGradeloading = true;
                    this.$modal.msgSuccess("修改成功");
                    this.saveGradeloading = false;
                    this.salesopen = false
                    this.getPeriodList()
                })
            } else {
                addPeriod(this.salesForm).then(res => {
                    console.log(res);
                    this.saveGradeloading = true;
                    this.$modal.msgSuccess("添加成功");
                    this.saveGradeloading = false;
                    this.salesopen = false
                    this.getPeriodList()
                })
            }

        },
        async delGrade(row) {
            this.$modal
                .confirm('是否确认删除阶段"' + row.PeriodName + '"？')
                .then(() => {
                    return removePeriod(row.Id);
                })
                .then(() => {
                    this.$modal.msgSuccess("删除成功");
                    this.getPeriodList()
                })
                .catch(() => {
                    this.loading = false;
                });
        },
        openEditSales(row) {
            console.log(row, '阶段信息');
            //修改销售阶段
            this.salesopen = true
            this.title = '修改销售阶段'
            this.salesForm = {
                id: row.Id,
                orgId: row.OrgId,
                periodName: row.PeriodName,
                periodType: row.PeriodType,
                probability: row.Probability,
                sort: row.Sort
            }

        },
        handleChange() {
            //
        },
        async getPeriodList() {
            //获取销售阶段
            try {
                let res = await periodList()
                console.log("销售阶段", res);
                if (res.data) {
                    this.salesList = res.data;

                }
            } catch (error) {
                console.log("销售阶段查询错误", error);
            }

        },
        //行拖拽,排序方法
        rowInitDrop() {

            // 获取对象
            const tbody = document.querySelector(".salesConfigTable .el-table__body-wrapper tbody");
            const _this = this;
            Sortable.create(tbody, {
                animation: 50,
                draggable: ".el-table__row",
                ghostClass: "ghost",
                handle: ".handle",
                onEnd(evt) {
                    let { newIndex, oldIndex } = evt
                    console.log("排序", newIndex, oldIndex, evt);
                    if (newIndex == oldIndex) return;
                    const currRow = _this.salesList.splice(oldIndex, 1)[0]
                    console.log("currRow当前行的数据", currRow);
                    _this.salesList.splice(newIndex, 0, currRow)
                    if (_this.salesList[newIndex].PeriodType != "ing" || _this.salesList[oldIndex].PeriodType != "ing") {
                        _this.$message({
                            message: '只能修改进行中的销售阶段的排序',
                            type: 'error',
                            duration: 5 * 1000
                        })
                        const item = _this.salesList.splice(newIndex, 1)[0];
                        _this.salesList.splice(oldIndex, 0, item);
                        // 复原拖拽之前的 dom
                        const tagName = evt.item.tagName;
                        const items = evt.from.getElementsByTagName(tagName);
                        if (evt.oldIndex > evt.newIndex) {
                            evt.from.insertBefore(evt.item, items[evt.oldIndex + 1]);
                        } else {
                            evt.from.insertBefore(evt.item, items[evt.oldIndex]);
                        }
                        return
                    } else {
                        let sortSalesForm = {
                            id: currRow.Id,
                            orgId: currRow.OrgId,
                            periodName: currRow.PeriodName,
                            periodType: currRow.PeriodType,
                            probability: currRow.Probability,
                            sort: newIndex
                        }
                        editPeriod(sortSalesForm).then(res => {
                            console.log(res);
                            this.$modal.msgSuccess("移动成功");
                            this.getPeriodList()
                        })
                    }
                },
                // onMove(evt) {
                //     console.log(evt.oldIndex);
                //     return false;
                // }
            });
        },
    },
};
</script>

<style lang="less" scoped>
.sales_config_con {
    background-color: #fff;
    padding:20px 20px;
    .sales_title{
        display: flex;justify-content: space-between;align-items: center;
    }
    .sales_config_li {
        line-height: 32px;
        .label{
            font-size: 18px;
            padding:25px 0 5px 0;
        }
        .el-table{
            margin-top: 10px;
        }
    }
}
</style>