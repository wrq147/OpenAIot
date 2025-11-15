<template>
    <div class="factorLibrary" style="padding:10px" id="big_con">
        <div class="factorLibrary-left">
            <factorType @getList="getList" />
        </div>
        <div class="factorLibrary-right">
            <div class="factorLibrary-table">
                <div class="from_con" id="from_con">
                    <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                        <el-form-item label="因子库发布年份" prop="year">
                            <el-select class="set_radius" v-model="queryParams.year" placeholder="请选择发布年份">
                                <el-option v-for="item in yearList" :key="item.Id" :label="item.Year" :value="item.Year" />
                            </el-select>
                        </el-form-item>
                        <el-form-item prop="version">
                             <el-select class="set_radius" v-model="queryParams.version" placeholder="请选择版本号">
                                <el-option label="全部" value="" />
                                <el-option v-for="item in versionList" :key="item.Id" :label="item.Version" :value="item.Version" />
                            </el-select>
                        </el-form-item>
                        <el-form-item class="button_con">
                            <!-- <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button> -->
                            <el-button type="primary" icon="el-icon-search" @click="handleQuery">查询</el-button>
                        </el-form-item>
                    </el-form>
                </div>
                <div class="elbiaoge_elform">
                    <el-row :gutter="10" class="mb8 button_row">
                        <div>
                            <el-col :span="1.5">
                                <el-button type="success" plain :disabled="!currentFactorType" @click="handleAdd">
                                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                    <span style="margin-left:6px">新增因子</span>
                                </el-button>
                            </el-col>
                        </div>
                    </el-row>
                    <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                        <el-table-column label="所属类别" align="center" prop="TypeName" />
                        <el-table-column label="因子名称" align="center" prop="FactorName" />
                        <el-table-column label="能源单位" align="center" prop="ActivityUnit" />
                        <el-table-column label="版本" align="center" prop="Version" />
                        <el-table-column label="因子值" align="center" prop="EmissionFactor" />
                        <el-table-column label="因子单位" align="center" prop="FactorUnit" />
                        <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
                            <template slot-scope="scope">
                                <el-button class="primary" type="text" @click="handleUpdate(scope.row)">修改</el-button>
                                <div class="line"></div>
                                <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <!-- <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/> -->
                </div>
            </div>
        </div>
        <factorAdd ref="factorAdd" :yearList="yearList" :versionList="versionList" :typeId="queryParams.typeId" @getList="getList" />

    </div>
</template>
<script>
import factorType from './cmp/factorType.vue'
import { selectFactorList, selectFactorYear, removeFactor } from '@/api/energy/factorLibrary'
import factorAdd from './cmp/factorAdd.vue'

export default {
    name: 'factorLibrary',
    components: {
      factorType,
      factorAdd
    },
    data() {
        return {
            queryParams: {
                pageNum: 1,
                pageSize: 10,
                year: '',
                version: '',
                typeId: '',
            },
            title: '添加因子',
            tableData: [],
            total: 0,
            loading: false,
            yearList: [],
            versionList: [],
            currentFactorType: false,
        };
    },
    created() {
        this.getYear();
    },
    methods: {
        // 获取数据
        getList(currentKey, currentFactorType) {
            this.loading = true;
            this.currentFactorType = currentFactorType !== undefined ? currentFactorType : this.currentFactorType;
            this.queryParams.typeId = currentKey ? currentKey : this.queryParams.typeId;
            selectFactorList(this.queryParams).then(res=>{
                this.tableData = res.data.flatMap(item => {
                    if (!item.Factors || item.Factors.length === 0) {
                        return [];
                    }
                    return item.Factors.map(factor => ({
                        ...factor,  // 保留Factor原有字段
                        // 从主数据中添加需要的字段
                        FactorUnit: item.FactorUnit, 
                        ActivityUnit: item.ActivityUnit,
                        TypeName: item.TypeName,
                    }));
                });
                this.loading = false;
           }).catch(error => {
                this.loading = false;
                this.tableData = [];
            });
        },

        // 碳因子年限版本
        getYear() {
            selectFactorYear().then(res=>{
                this.queryParams.year = res.data.length > 0 ? res.data[0].Year : '';
                this.yearList = res.data;
                this.versionList = res.data.length > 0 ? res.data[0].Versions : [];
            })
        },

        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.resetForm("queryForm");
            this.handleQuery();
        },

        // 新增因子
        handleAdd(){
            this.title = '添加因子';
            this.$refs.factorAdd.openDialog()
        },

        // 修改因子
        handleUpdate(data){
            this.title = '修改因子';
            this.$refs.factorAdd.openDialog(data)
        },

        // 删除因子
        handleDelete(data){
            this.$confirm('确定删除该因子吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeFactor({ Id: data.Id }).then(res => {
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
        }
    }
}
</script>
<style lang="less" scoped>
.factorLibrary {
    height: 100%;
    width: 100%;
    display: flex;
     justify-content: space-between;
    .factorLibrary-left {
        width: 240px;
        background: #19212D;
        border-radius: 4px;
    }
    .factorLibrary-right {
        flex: 1;
        margin-left: 10px;
        background: #19212D;
        border-radius: 4px;
        .factorLibrary-table{
            padding: 0 4px;
        }

    }

}

</style>