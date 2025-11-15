<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item>
                        <el-select class="set_radius" style="width: 94px" v-model="queryParams.productType" placeholder="请选择">
                            <el-option label="产品名称" :value="1" />
                            <el-option label="产品型号" :value="2" />
                        </el-select>
                    </el-form-item>
                    <el-form-item v-if="queryParams.productType == 1" style="margin-left: 4px;" prop="productName">
                        <el-input style="width: 140px;" v-model="queryParams.productName" placeholder="请输入" clearable />
                    </el-form-item>
                    <el-form-item v-else style="margin-left: 4px;" prop="productModel">
                        <el-input style="width: 140px;" v-model="queryParams.productModel" placeholder="请输入" clearable />
                    </el-form-item>
                    <el-form-item prop="ProductBorder" label="生命周期边界">
                        <el-select class="set_radius" v-model="queryParams.ProductBorder" clearable placeholder="请选择">
                            <el-option v-for="item in tabList" :key="item.Id" :label="item.name" :value="item.id" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="创建日期">
                        <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
                        range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                </div>
                <el-form-item class="button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">查询</el-button>
                </el-form-item>
            </el-form>
        </div>
        <div class="elbiaoge_elform" style="padding: 16px 6px;">
            <el-row :gutter="10" style="padding-left: 10px;" class="mb8 button_row">
                <div style="display: flex;align-items: center;">
                    <el-col :span="1.5">
                        <el-button type="success" plain @click="handleAdd">
                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                            <span style="margin-left:6px">新增产品生命周期模型</span>
                        </el-button>
                    </el-col>
                </div>
            </el-row>
            <div class="cyclicalModel-info">
                <div class="cyclicalModel-info-list" v-for="item in tableData" :key="item.Id">
                    <div class="cyclicalModel-info-top">
                        <img src="@/assets/images/model.png" alt="">
                        <div class="cyclicalModel-info-top-text">
                            <div class="title">{{ item.ModelName }}</div>
                            <div class="type">{{ item.ProductName }}<span>型号：{{ item.ProductModel }}</span></div>
                        </div>
                    </div>
                    <div class="cyclicalModel-info-bom">
                        <div>{{ tabList.find(tab => tab.id === item.ProductBorder) ? tabList.find(tab => tab.id == item.ProductBorder).name   : '未设置生命周期边界'  }}</div>
                        <div class="tab-btn">
                            <div @click="handleDetail(item)">详情</div>
                            <div @click="handleEdit(item)">编辑</div>
                            <div class="del" @click="handleDel(item.Id)">删除</div>
                        </div>
                    </div>
                </div>
            </div>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
        </div>
        <modelAdd ref="modelAdd" :title="title" :tabList="tabList" @getList="handleQuery" />
    </div>
</template>
<script>
import { modelPageList, removeModel } from '@/api/energy/cyclicalModel';
import modelAdd from './cmp/modelAdd.vue';
export default {
    name: 'cyclicalModel',
    components: {
        modelAdd
    },
    data() {
        return {
            title: '',
            createDateRange: [],
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                productType: 1,
                productName: '',
                productModel: '',
                ProductBorder: ''
            },
            tabList: [
                {
                    id: '1',
                    name: '从摇篮到大门',
                    tip: '从资源开采到产品出厂'
                },
                {
                    id: '2',
                    name: '从大门到大门',
                    tip: '从产品制造到产品出厂'
                },
                {
                    id: '3',
                    name: '从摇篮到坟墓',
                    tip: '从资源开采到产品废弃'
                },
            ],
            tableData: [],
            total: 0,
        };
    },
    mounted() {
        this.getList();
    },
    methods: {
        getList() {
            modelPageList(this.queryParams).then(res => {
                this.tableData = res.data.List;
                this.total = res.data.Total;
            })
        },
        // 新增
        handleAdd() {
            this.title = '新增';
            this.$refs.modelAdd.openDialog();
        },

        // 详情
        handleDetail(row) {
            this.title = '详情';
            this.$refs.modelAdd.openDialog(row, true);
        },

        // 编辑
        handleEdit(row) {
            this.title = '编辑';
            this.$refs.modelAdd.openDialog(row);
        },

        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.createDateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },
        
        // 删除
        handleDel(id) {
            this.$confirm('确定删除该产品生命周期吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeModel({ Id: id }).then(res => {
                    this.$message({
                        type: 'success',
                        message: '删除成功'
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
    }
}
</script>
<style lang="less" scoped>
.cyclicalModel-info{
    margin-top: 24px;
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    .cyclicalModel-info-list{
        width: 380px;
        height: 140px;
        background: #222E40;
        border-radius: 4px;
        margin-left: 10px;
        margin-right: 10px;
        margin-bottom: 20px;
        .cyclicalModel-info-top{
            height: 93px;
            display: flex;
            align-items: center;
            padding: 20px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            >img{
                width: 62px;
                height: 56px;
                margin-right: 24px;
            }
            .title{
                font-weight: 600;
                font-size: 16px;
                color: #FFFFFF;
                margin-bottom: 6px;
            }
            .type{
                font-weight: 400;
                font-size: 14px;
                color: rgba(255, 255, 255, 0.6);
                >span{
                   margin-left: 20px;
                }
            }
        }
        .cyclicalModel-info-bom{
            height: calc(100% - 93px);
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 20px;
            font-weight: 400;
            font-size: 14px;
            color: rgba(255, 255, 255, 0.6);
            .tab-btn{
                display: flex;
                align-items: center;
                color: #3DB98F;
                >div{
                    margin-right: 10px;
                    position: relative;
                    cursor: pointer;
                    &:not(:last-child){
                       padding-right: 10px;
                    }
                    &:not(:last-child)::after{
                        content: '';
                        position: absolute;
                        right: 0;
                        top: 50%;
                        transform: translateY(-50%);
                        width: 1px;
                        height: 14px;
                        background: rgba(255, 255, 255, 0.2);
                    }
                }
            }
            .del{
                color: #F15C5C;
            }
        }
    }
}
</style>