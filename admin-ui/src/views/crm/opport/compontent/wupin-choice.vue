<template>
    <div>
        <el-dialog width="960px" title="请选择产品" :visible.sync="wupinOpen" :close-on-click-modal="false" append-to-body
            :destroy-on-close="true">
            <el-form :model="wupinQuery" :inline="true" ref="wupinForm"
                style="display: flex;justify-content: space-between;">
                <div>
                    <el-form-item label="搜索关键字" prop="Key">
                        <el-input v-model="wupinQuery.Key" placeholder="搜索产品名称或批次编号" clearable></el-input>
                    </el-form-item>
                    <el-form-item label="创建日期">
                        <el-date-picker class="form_input_style" v-model="wupinDateRange" style="width:232px"
                            value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                            end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                </div>
                <el-form-item>
                    <el-button icon="el-icon-refresh" @click="resetWupin">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="loadWupinList">搜索</el-button>
                </el-form-item>
            </el-form>
            <el-table ref="wupinTable" :data="wupinList" tooltip-effect="dark" v-loading="wupinLoading" style="width: 100%"
                @selection-change="onWupinChange" :row-key="getRowKey">
                <el-table-column type="selection" width="55" :reserve-selection="true"></el-table-column>
                <el-table-column prop="SkuNumber" label="产品编号" align="center" width="150"></el-table-column>
                <el-table-column label="产品标签" align="center">
                    <template slot-scope="scope">
                    <div>{{ scope.row.ProductLabel === 'U' ? '半成品' : '成品' }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="预览图片" align="center" width="150">
                    <template slot-scope="scope">
                        <div class="imgwrap">
                            <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]">
                            </el-image>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column prop="ProductName" label="产品名称"></el-table-column>
            </el-table>

            <pagination v-show="wupinTotal > 0" :total="wupinTotal" :page.sync="wupinQuery.pageNum"
                :limit.sync="wupinQuery.pageSize" @pagination="loadWupinList" />

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="onWupinConfirm">确 定</el-button>
                <el-button @click="wupinOpen = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
import {
    agentProductListGet
} from "@/api/factory/product";

export default {
    name: 'AdminUiWupinChoice',
    props: {
        detailList: {
            type: Array,
            default: (() => {
                return []
            })
        },
    },
    data() {
        return {
            //耗材相关参数
            wupinOpen: false,
            wupinLoading: false,
            wupinDateRange: [],
            wupinQuery: {
                pageNum: 1,
                pageSize: 20
            },
            wupinTotal: 0,
            wupinList: [],
            wupinSelectArr: [],
            //耗材相关参数
        };
    },
    mounted() {
    },
    methods: {
        getRowKey(row) { // console.log(’================:’, JSON.stringify(row))
            return row.Id
        },
        setCurrent() {
            // console.log(this.detailList, 'this.detailList');
            this.detailList.map(row => {
                // console.log("row", row);
                this.$refs.wupinTable.toggleRowSelection(row, true);
            })
        },
        //选择商机明细
        openWupinDialog() {
            this.wupinOpen = true;
            this.loadWupinList();

        },
        resetWupin() {
            //重置耗材选择列表
            this.wupinDateRange = [];
            this.resetForm("wupinForm");
            this.loadWupinList();
        },
        loadWupinList() {
            //加载耗材列表
            this.wupinLoading = true;
            this.wupinSelectArr = [];
            agentProductListGet(this.addDateRange(this.wupinQuery, this.wupinDateRange)).then(
                rsp => {
                    this.wupinList = rsp.data.List;
                    this.wupinTotal = rsp.data.Total;
                    this.wupinLoading = false;
                    this.setCurrent()

                }
            );
        },
        onWupinChange(val) {
            //选择耗材
            // console.log("表格被选中的", val);
            this.wupinSelectArr = val;
        },
        onWupinConfirm() {
            //确定选择的配件
            if (this.wupinSelectArr.length > 0) {
                this.$emit('onWupinConfirm', this.wupinSelectArr)

            }
            this.wupinOpen = false;
        },

        //选择商机明细
    },
};
</script>

<style lang="less" scoped></style>