<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item label="选择日期">
                        <el-date-picker class="form_input_style" v-model="statisticDateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
                        range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                </div>
                <el-form-item class="button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">查询</el-button>
                </el-form-item>
            </el-form>
        </div>
        <div class="elbiaoge_elform">
            <el-collapse v-loading="loading">
                <el-collapse-item v-for="item in tableData" :key="item.Id">
                    <template slot="title">
                       <div class="heard-title">
                          {{ item.ClassName }}
                           <div class="heard-btn" @click.stop="handleAdd(item.Id)">
                                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                <span style="margin-left:6px">新增设施/活动</span>
                           </div>
                       </div>
                    </template>
                    <el-table :data="item.OrgClasses" class="data_table" style="width:100%">
                        <el-table-column label="子类别" align="center">
                            <template slot-scope="scope">
                                <el-select v-if="scope.row.isEdit" v-model="scope.row.SubClassId" placeholder="请选择">
                                    <el-option v-for="item in getClassInfo(item.Id)" :key="item.Id" :label="item.SubClassName" :value="item.Id" />
                                </el-select>
                                <span v-else>{{ scope.row.SubClassName }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="活动/设施" align="center">
                            <template slot-scope="scope">
                                <treeselect v-if="scope.row.isEdit" v-model="scope.row.FacilityId" :appendToBody="true" :options="facilityList" :show-count="true" :normalizer="normalizer" placeholder="请选择设施名称" />
                                <span v-else>{{ scope.row.FacilityName }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="排放源" align="center">
                            <template slot-scope="scope">
                                <el-select v-if="scope.row.isEdit" v-model="scope.row.FactorId" placeholder="请选择能源类型" @change="handleFactor($event, scope.row)">
                                    <el-option v-for="item in factorList" :key="item.Id" :label="item.FactorName" :value="item.Id" />
                                </el-select>
                                <span v-else>{{ scope.row.FactorName }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="活动数据来源" align="center">
                            <template slot-scope="scope">
                                <el-select v-if="scope.row.isEdit" v-model="scope.row.DataSource" placeholder="请选择">
                                    <el-option v-for="(item, key) in dataSourceList" :key="key" :label="item" :value="key" />
                                </el-select>
                                <span v-else>{{ dataSourceList[scope.row.DataSource] }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="活动数据单位" align="center" prop="ActivityUnit" />
                        <el-table-column label="碳排放因子" align="center" prop="EmissionFactor" />
                        <el-table-column label="排放因子单位" align="center" prop="FactorUnit" />
                        <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
                            <template slot-scope="scope">
                                <div v-if="scope.row.isEdit" style="display: flex; align-items: center;">
                                    <el-button class="primary" style="color: rgba(255, 255, 255, 0.6)" type="text" @click="handleClose(item.Id, scope.row)">取消</el-button>
                                    <div class="line"></div>
                                    <el-button class="primary" type="text" @click="handleSave(scope.row)">保存</el-button>
                                </div>
                                <div v-else style="display: flex; align-items: center;">
                                    <el-button class="primary" type="text" @click="handleUpdate(scope.row)">编辑</el-button>
                                    <div class="line"></div>
                                    <el-button class="primary" type="text" @click="handleView(scope.row)">数据来源</el-button>
                                    <div class="line"></div>
                                    <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                                </div>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-collapse-item>
            </el-collapse>
        </div>
        <dataSourseAdd ref="dataSourseAdd" :dataSourceList="dataSourceList" @getList="getList" />
    </div>
</template>
<script>
import { classInfo, orgClassInfo, removeOrgClass, addOrgClass, updateOrgClass } from '@/api/energy/emissionCategory';
import { selectFactorOrg } from '@/api/energy/factorLibrary';
import { facilityTree } from '@/api/energy/facility';
import dataSourseAdd from './cmp/dataSourseAdd.vue'
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
    name: 'energyGather',
    components: {
        Treeselect,
        dataSourseAdd
    },
    data() {
        return {
            title: '新增',
            loading: false,
            exportLoading: false,
            statisticDateRange: [],
            tableData: [],
            subClassMap: {},
            factorList: [],
            facilityList: [],
            dataSourceList: {
                '1': '计量设备',
                '2': '手工录入',
                '3': '供应链数据',
            },
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                beginDate: '',
                endDate: '',
            }
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        getInitList() {
            selectFactorOrg({ OrgId: this.$store.getters.orgId }).then(res => {
                this.factorList = res.data;
            });
            facilityTree({ OrgId: this.$store.getters.orgId }).then(res => {
                this.facilityList = res.data;
            });
            this.getList();
        },
        // 获取列表
        getList() {
            this.loading = true;
            if (this.statisticDateRange.length > 0) {
                this.queryParams.beginDate = this.statisticDateRange[0];
                this.queryParams.endDate = this.statisticDateRange[1];
            }
            orgClassInfo(this.queryParams).then(res => {
                res.data.forEach(item => {
                    item.OrgClasses.forEach(item => {
                        item.isEdit = false;
                    })
                })
                this.tableData = res.data;
                this.loading = false;
            })
        },

        // 获取子类别
        getClassInfo(parentId) {
            // 1. 已加载过数据：直接返回
            if (this.subClassMap[parentId]) {
                return this.subClassMap[parentId];
            }
            // 2. 正在请求中：返回空数组（避免重复请求）
            if (this.subClassMap[`${parentId}_loading`]) {
                return [];
            }
            // 3. 标记为“请求中”（用 $set 确保响应式）
            this.$set(this.subClassMap, `${parentId}_loading`, true);
            
            // 4. 发起异步请求
            classInfo({ Id: parentId }).then(res => {
                const subClassData = res.data.SubClass || [];
                // 关键：用 $set 给 subClassMap 新增属性，触发模板重新渲染
                this.$set(this.subClassMap, parentId, subClassData);
            }).catch(error => {
                console.error('获取子类别失败', error);
                // 错误时也用 $set 赋值空数组，确保后续不重复请求
                this.$set(this.subClassMap, parentId, []);
            }).finally(() => {
                // 清除请求标记（用 delete 不影响响应式，因标记是临时属性）
                delete this.subClassMap[`${parentId}_loading`];
            });

            // 初始返回空数组（请求完成后 $set 会触发模板更新）
            return [];
        },

         /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.statisticDateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },

        // 选择排放源操作
        handleFactor(val, row) {
            row.EmissionFactor = this.factorList.find(item => item.Id == val)?.EmissionFactor;
            row.ActivityUnit = this.factorList.find(item => item.Id == val)?.ActivityUnit;
            row.FactorUnit = this.factorList.find(item => item.Id == val)?.FactorUnit;
        },

        // 新增
        handleAdd(id) {
            this.tableData.forEach(item => {
                if(item.Id == id) {
                    item.OrgClasses.push({
                        OrgId: this.$store.getters.orgId,
                        SubClassId: '',
                        FacilityId: null,
                        FactorId: '',
                        DataSource: '',
                        EquipmentIds: '',
                        ActivityUnit: '',
                        EmissionFactor: '',
                        FactorUnit: '',
                        isEdit: true,
                    })
                }
            })
            this.title = '新增';
        },

        // 修改
        handleUpdate(row) {
            this.title = '编辑';
            row.isEdit = true;
        },

        // 保存
        handleSave(row) {
            if(this.title === '新增') {
                addOrgClass(row).then(res => {
                    this.$message({
                        message: '新增成功',
                        type: 'success'
                    });
                    this.getList();
                })
            } else {
                updateOrgClass(row).then(res => {
                    this.$message({
                        message: '编辑成功',
                        type: 'success'
                    });
                    this.getList();
                })
            }
        },
        
        // 详情
        handleView(row){
            this.$refs.dataSourseAdd.openDialog(row, true)
        },
      
        // 删除
        handleDelete(row) {
            this.$confirm('确定删除该数据吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeOrgClass({ Id: row.Id }).then(res => {
                    this.$message({
                        message: '删除成功',
                        type: 'success'
                    });
                    this.getList();
                })
            }).catch(() => {
                this.$message({
                    type: 'info',
                    message: '已取消删除'
                });
            });
        },

        // 点击取消
        handleClose(parentId, row) {
            if(this.title === '新增') {
                const targetParent = this.tableData.find(parentItem => parentItem.Id === parentId);
                if (targetParent && targetParent.OrgClasses) {
                    // 2. 找到要删除的新增行在 OrgClasses 中的索引（通过 row 的引用或唯一标识匹配）
                    const rowIndex = targetParent.OrgClasses.findIndex(item => {
                        // 新增行的特征：isEdit: true + 可通过临时标识或引用匹配（这里用 row 本身的引用）
                        return item.isEdit && item === row; 
                    });
                    // 3. 索引存在则删除该行（避免删除非新增行）
                    if (rowIndex > -1) {
                        targetParent.OrgClasses.splice(rowIndex, 1);
                    }
                }
            } else {
                row.isEdit = false;
            }
        },

        normalizer(node) {
            if (node.Children == null || !node.Children.length) {
                delete node.Children;
            }
            return {
                id: node.Id,
                label: node.FacilityName,
                children: node.Children,
            };
        },

    }
}
</script>
<style lang="less" scoped>
::v-deep .el-collapse{
    border: 0;
    .el-collapse-item__header{
        background-color: #19212D;
        font-weight: 500;
        font-size: 16px;
        color: #FFFFFF;
        height: 64px;
        line-height: 64px;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        .heard-title{
            position: relative;
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding-left: 24px;
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
            .heard-btn{
                margin-right: 8px;
                width: 132px;
                height: 32px;
                background: #3DB98F;
                border-radius: 4px;
                display: flex;
                align-items: center;
                justify-content: center;
                font-weight: 400;
                font-size: 14px;
                color: #FFFFFF;
                > i {
                    font-size: 10px;
                    color: #fff;
                }
            }
        }
        
        .el-collapse-item__arrow {
            width: 32px;
            height: 32px;
            background: #222E40;
            border-radius: 4px;
            font-size: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
    }
    .el-collapse-item__wrap{
        background-color: #19212D;
        border: 0;
    }
}
</style>
<style>
.vue-treeselect__menu-container{
    font-size: 12px;
}
</style>