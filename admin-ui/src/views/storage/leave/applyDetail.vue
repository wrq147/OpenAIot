<template>
    <div class="returnpage">
        <el-dialog title="出库申请单" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1380px"
            top="2vh">
            <div style="display:flex;justify-content: space-between;">
                <div style="flex:1;width: 0;">
                    <el-descriptions title="基本信息" :column="3" border>
                        <el-descriptions-item label="出库申请单号">
                            {{ form.ApplyNumber }}
                        </el-descriptions-item>
                        <el-descriptions-item label="申请类型">
                            {{ LeaveMethodName(form.ApplyType) }}
                        </el-descriptions-item>
                        <el-descriptions-item label="状态">
                            <el-tag v-if="form.Status == 0" type="warning">待提交</el-tag>
                            <el-tag v-else-if="form.Status == 1" type="warning">待审批</el-tag>
                            <el-tag v-else-if="form.Status == 2" type="success">申请成功</el-tag>
                            <el-tag v-else-if="form.Status == 3" type="danger">申请失败</el-tag>
                            <el-tag v-else-if="form.Status == 4" type="info">已取消</el-tag>
                            <el-link style="margin-left:15px;" @click="onLook" v-if="form.LeaveMethod == 1" type="primary">查看入库单</el-link>
                        </el-descriptions-item>
                        <el-descriptions-item label="出库仓库">
                            {{ form.House?form.House.StoreName:'' }}
                        </el-descriptions-item>

                        <el-descriptions-item label="申请时间">
                            {{ parseTime(form.ApplyOn) }}
                        </el-descriptions-item>
                        <el-descriptions-item>
                            
                        </el-descriptions-item>
                        <el-descriptions-item label="备注" span="3">
                            {{ form.Reason }}
                        </el-descriptions-item>
                    </el-descriptions>

                    <div class="wp-title">工单明细
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
                    <div class="wp-title">审批信息</div>
                </DetailEmbed>
            </div>



        </el-dialog>

    </div>
</template>
      
<script>
import DetailEmbed from "@/views/flowable/task/record/DetailEmbed";
import { ApplyInfo } from "@/api/storage/apply";
export default {
    name: "LeaveDetail",
    components: { DetailEmbed},
    dicts: ["apply_type"],
    data() {
        return {
            allloading: false,
            open: false,
            form: {},
        };
    },
    computed:{
        finishedTotal(){
            let total=0
            if(this.form&&this.form.List){
                this.form.List.map(ro=>{
                    if(ro.TargetType==1){
                    total=total+Number(ro.Quantity)
                    }
                })
            }
            return total
        },
        useTotal(){
            let total=0
            if(this.form&&this.form.List){
                this.form.List.map(ro=>{
                    if(ro.TargetType==0){
                    total=total+Number(ro.Quantity)
                    }
                })
            }
            return total
        }
    },
    methods: {
        async openDialog(id) {
            this.open = true;
            this.allloading = true;
            let leaveInfo = await ApplyInfo(id);
            this.form = leaveInfo.data;
            await this.$refs.nodelDet.initNodes(this.form.FlowId);
            this.allloading = false;
        },
        LeaveMethodName(way) {
            let wayName=this.dict.getName("apply_type", way);
            return wayName;
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