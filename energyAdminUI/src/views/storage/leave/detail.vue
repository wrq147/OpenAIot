<template>
    <div class="returnpage">
        <el-dialog title="出库单" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1380px"
            top="2vh">
            <div style="display:flex;justify-content: space-between;">
                <div style="flex:1;width: 0;">
                    <el-descriptions title="基本信息" :column="3" border>
                        <el-descriptions-item label="出库单号">
                            {{ form.StockNumber }}
                        </el-descriptions-item>
                        <el-descriptions-item label="出库方式">
                            {{ LeaveMethodName(form.LeaveMethod) }}
                        </el-descriptions-item>
                        <el-descriptions-item label="状态">
                            <el-tag v-if="form.Status == 0" type="warning">待提交</el-tag>
                            <el-tag v-else-if="form.Status == 1" type="warning">待审批</el-tag>
                            <el-tag v-else-if="form.Status == 2" type="success">出库成功</el-tag>
                            <el-tag v-else-if="form.Status == 3" type="danger">出库失败</el-tag>
                            <el-tag v-else-if="form.Status == 4" type="info">已取消</el-tag>
                            <el-link style="margin-left:15px;" @click="onLook" v-if="form.LeaveMethod == 1" type="primary">查看入库单</el-link>
                        </el-descriptions-item>
                        <el-descriptions-item label="所出仓库">
                            {{ form.FromHouseName }}
                        </el-descriptions-item>
                        <el-descriptions-item label="所入仓库">
                            <template v-if="form.ToHouseName != ''">
                                <el-tag>{{ form.ToName }}</el-tag>
                                <span style="margin-left:10px;">{{form.ToHouseName }}</span>
                            </template>
                            <template v-else>
                                无
                            </template>
                        </el-descriptions-item>

                        <el-descriptions-item label="出库时间">
                            {{ parseTime(form.OutDate) }}
                        </el-descriptions-item>
                        <el-descriptions-item label="物流信息" span="3">
                            <template v-if="this.form.ExpressNumber != ''">
                                <a type="primary" :href="'https://www.kuaidi100.com/?nu=' + this.form.ExpressNumber" target="_blank">
                                    <el-tag>{{ this.form.ExpressCompanyName }}</el-tag>
                                    <span style="margin-left:15px;">{{ this.form.ExpressNumber }}</span>
                                    <span v-if="this.form.ExpressPhone != ''">（{{ this.form.ExpressPhone }}）</span>
                                    <span style="margin-left:25px;color:#409EFF;"><i class="el-icon-view"
                                            style="margin-right:5px;"></i>点击查询快递详情</span>
                                </a>
                            </template>
                        </el-descriptions-item>

                        <el-descriptions-item label="备注" span="3">
                            {{ form.Remark }}
                        </el-descriptions-item>
                    </el-descriptions>

                    <div class="wp-title">出库物品
                        <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
                        <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
                    </div>
                    <el-table :data="form.List" style="width: 100%" border stripe
                        :header-cell-style="{'color': '#78829D','background': '#F9FAFC !important'}">
                        <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
                        </el-table-column>
                        <el-table-column prop="TargetName" label="物品名称">
                        </el-table-column>
                        <el-table-column label="存储类型" align="center" width="100">
                            <template slot-scope="scope">
                                <span v-if="scope.row.TargetType==1">成品</span>
                                <span v-else-if="scope.row.TargetType==0">半成品</span>
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
                        <el-table-column prop="Quantity" align="center" label="数量" width="140">
                        </el-table-column>
                        <el-table-column prop="Price" align="center" label="单价（元）" width="140">
                        </el-table-column>
                    </el-table>
                </div>
                <DetailEmbed ref="nodelDet" curStyle="width:320px;margin-left:20px;">
                    <div class="wp-title">出库审核</div>
                </DetailEmbed>
            </div>
        </el-dialog>

    </div>
</template>
      
<script>
import DetailEmbed from "@/views/flowable/task/record/DetailEmbed";
import {
    getLeaveInfo
} from "@/api/storage/stock";
export default {
    name: "LeaveDetail",
    components: { DetailEmbed},
    data() {
        return {
            allloading: false,
            open: false,
            form: {},
            finishedTotal:0,
            useTotal:0,
        };
    },
    methods: {
        async openDialog(id) {
            this.open = true;
            this.allloading = true;
            let leaveInfo = await getLeaveInfo(id);
            this.form = leaveInfo.data;
            this.finishedTotal=0
            this.useTotal=0
            leaveInfo.data.List.map(ro=>{
                if(ro.TargetType==1){
                    this.finishedTotal=this.finishedTotal+Number(ro.Quantity)
                }else if(ro.TargetType==0){
                    this.useTotal=this.useTotal+Number(ro.Quantity)
                }
            })
            this.form.ExpressCompanyName = await this.$store.dispatch("datas/kuaiName", this.form.ExpressCompany);
            await this.$refs.nodelDet.initNodes(this.form.FlowId);
            this.allloading = false;
        },
        LeaveMethodName(way) {
            switch (way) {
                case 0:
                    return "出库";
                case 1:
                    return "退货";
                case 2:
                    return "调拨";
                case 3:
                    return "领料";
            }
            return "";
        },
        onLook(){
            this.open = false;
            this.$emit('lk',this.form.SourceEnterId);
        }
    }
};
</script>
<style lang="scss">
.num_li{
    font-weight: normal;
    font-size: 14px;
    color: #666666;
    margin-left: 10px;
}
.wp-title {
    margin-top: 20px;
    font-size: 16px;
    font-weight: bold;
    color: #303133;
    margin-bottom: 20px;
}

.imgwrap {
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;

    .el-image {
        display: flex;
        width: 80px;
        height: 80px;
        justify-content: center;
        align-items: center;
    }
}
</style>