<template>
    <div>
        <el-dialog title="库存变更记录" top="5vh" :close-on-click-modal="false" :visible.sync="open" width="1080px"
            append-to-body>
            <div id="from_con">
                <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
                    <el-form-item label="记录时间">
                        <el-date-picker class="form_input_style" v-model="recordDateRange" style="width:232px"
                            value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                            end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>

                    <el-form-item class="submit_button_con">
                        <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                    </el-form-item>

                </el-form>
            </div>
            <div>

                <el-table border v-loading="isloading" :data="tbList" class="data_table"
                    style="width:100%" row-key="Id">
                    <el-table-column label="变更方式" align="center">
                        <template slot-scope="scope">
                            <span v-if="scope.row.FormType == 0">出库</span>
                            <span v-else-if="scope.row.FormType == 1">入库</span>
                            <span v-else-if="scope.row.FormType == 2">盘亏修正</span>
                            <span v-else-if="scope.row.FormType == 3">盘盈修正</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="变更前数量" align="center" prop="Remnant" width="180">
                        <template slot-scope="scope">
                            {{ scope.row.Remnant+scope.row.LockRemnant }}
                        </template>
                    </el-table-column>
                    <el-table-column label="调整数量" align="center" prop="Quantity" width="120">
                        <template slot-scope="scope">
                            <el-tag type="success" v-if="scope.row.FormType == 1 || scope.row.FormType == 3">+{{
                                scope.row.Quantity }}</el-tag>
                            <el-tag type="danger" v-else>-{{ scope.row.Quantity }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column label="变更后数量" align="center" prop="Remnant" width="180">
                        <template slot-scope="scope">
                            {{ scope.row.changeCount }}
                        </template>
                    </el-table-column>
                    <el-table-column label="原价（元）" align="center" prop="StockPrice" width="120"></el-table-column>
                    <el-table-column label="出入价（元）" align="center" prop="Price" width="120"></el-table-column>
                    <el-table-column label="变更后均价（元）" align="center" width="120">
                        <template slot-scope="scope">
                            {{ scope.row.changePrice }}
                        </template>
                    </el-table-column>
                    <el-table-column label="记录时间" align="center" width="160">
                        <template slot-scope="scope">
                            {{ parseTime(scope.row.CreatedOn)}}
                        </template>
                    </el-table-column>
                </el-table>
                <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                    :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>

        </el-dialog>
    </div>
</template>
      
<script>
import {
    recordList
} from "@/api/storage/stock";
export default {
    data() {
        return {
            open: false,
            isloading: false,
            tbList: [],
            recordDateRange: [],
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 10,
            },
            // 总条数
            total: 0,
        };
    },
    methods: {
        async openDlg(row) {
            this.open = true;
            this.isloading = true;
            this.queryParams.HouseId = row.HouseId;
            this.queryParams.TargetType = row.TargetType;
            this.queryParams.TargetId = row.TargetId;
            await this.getList();
            this.isloading = false;
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.resetForm("queryForm");
            this.handleQuery();
        },
        async getList() {
            this.isloading = true;
            let rsp = await recordList(this.addDateRange(this.queryParams, this.recordDateRange));
            this.tbList = rsp.data.List;
            this.tbList.forEach(item=>{
                item.changeCount=item.Remnant+item.LockRemnant+((item.FormType == 1 || item.FormType == 3)?item.Quantity:-item.Quantity);
                if(item.changeCount==0){
                    item.changePrice=0;
                }
                else{
                    item.changePrice=Math.abs((item.StockPrice*(item.Remnant+item.LockRemnant)+item.Price*((item.FormType == 1 || item.FormType == 3)?item.Quantity:-item.Quantity))/item.changeCount);
                }

            });
            this.total = rsp.data.Total;
            this.isloading = false;
        }
    }
};
</script>
<style lang="scss"></style>