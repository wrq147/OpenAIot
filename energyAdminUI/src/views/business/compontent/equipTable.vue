<template>
  <div style="width:100%">
    <el-row :gutter="10" class="mb8 button_row" v-if="!isShowNumm">
        <div>
        <el-col :span="1.5">
            <el-button type="success" plain @click="handleAdd">
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
            <span style="margin-left:6px">绑定设备</span>
            </el-button>
        </el-col>
        </div>
    </el-row>

    <el-table v-loading="loading" :data="tableData" class="data_table" :header-cell-style="cellSty" style="width:100%" v-if="!isShowNumm">
        <el-table-column label="序号" type="index" width="50" align="center" />
        <el-table-column label="设备编码" align="center" key="EquipmentCode" prop="EquipmentCode" :show-overflow-tooltip="true"/>
        <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" :show-overflow-tooltip="true"/>
        <el-table-column label="设备类型" align="center" key="TypeName" prop="TypeName" :show-overflow-tooltip="true"/>
        <el-table-column label="数据单位" align="center" key="Unit" prop="Unit" :show-overflow-tooltip="true"/>
        <el-table-column label="设备状态" align="center" key="Online" prop="Online" :show-overflow-tooltip="true">
        <template slot-scope="scope">
            <span>{{ scope.row.Online==1?'在线':(scope.row.Online==0?'离线':'未初始化')}}</span>
        </template>
        </el-table-column>
        <el-table-column label="计费关联" align="center" key="PolicyId" prop="PolicyId">
        <template slot-scope="scope">
            <span>{{ scope.row.PolicyId&&scope.row.PolicyId!='-'&&scope.row.PolicyId>0?'已关联':'未关联'}}</span>
        </template>
        </el-table-column>
        <el-table-column label="计费标准" align="center" key="PolicyName" prop="PolicyName" :show-overflow-tooltip="true"/>
        <el-table-column label="数据状态" align="center" key="DataState" prop="DataState" :show-overflow-tooltip="true">
        <template slot-scope="scope">
            <span>{{ scope.row.DataState==1?'纳入能源计算':'仅采集，不纳入能源计算'}}</span>
        </template>
        </el-table-column>
        <el-table-column label="操作" align="center" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope" v-if="scope.row.Id>2">
                <el-button class="primary" type="text" @click="unBind(scope.row)">取消绑定</el-button>
            </template>
        </el-table-column>
    </el-table>
    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" v-if="!isShowNumm"/>
    <el-dialog title="" :visible.sync="tipsDialog" width="30%" :show-close="false" class="unbind_tips_dialog" top="25vh" :close-on-click-modal="false">
        <div class="tips_con">
            <i class="zhongtaiiconfont zhongtai-icon-zhongyaotishi"></i>
            <span>确认是否将xxx设备取消绑定，取消后该电表数据将不计入到该设施单位下。</span>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button @click="tipsDialog = false">否</el-button>
            <el-button type="primary" @click="tipsDialog = false">是</el-button>
        </span>
    </el-dialog>
    <selectEquip ref="selectEquip" @finishChoice="finishChoice"></selectEquip>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {equipmentPageList} from '@/api/energy/equip'
import {facilityBindEquipment,removeFacilityBindEquipment} from '@/api/energy/facility'
import selectEquip from '@/views/business/compontent/selectEquip'
export default {
  name: 'EnergyAdminUIEquipTable',
  mixins: [resizeTableCon],
  components:{selectEquip},
  data() {
    return {
      tableData:[],
      tipsDialog:false,
      queryParams:{
        facilityId:'',
        pageNum:1,
        pageSize:10,
        
      },
      total:0,
      loading:false,
      isShowNumm:false
    };
  },

  mounted() {
    
  },

  methods: {
    finishChoice(ids){
      // console.log(ids,'idsidsids',ids.join(','));
      facilityBindEquipment({
        facilityId: this.queryParams.facilityId,
        equipmentId:ids.join(',')
      }).then(res=>{
        this.$modal.msgSuccess("绑定成功");
        this.getList()
      })
    },
    setShowNull(){
      this.isShowNumm=true
    },
    handleAdd(){
      this.$refs.selectEquip.openSelect()
    },
    unBind(row){
        //取消绑定
      this.$modal.confirm('是否确认取消绑定设备'+row.EquipmentName+'？')
      .then(function() {
        return removeFacilityBindEquipment({Id:row.Id});
      })
      .then(() => {
        this.getList();
        this.$modal.msgSuccess("删除成功");
      })
      .catch(() => {});
    },
    loadData(id){
        this.queryParams.facilityId=id
        this.queryParams.pageNum=1
        this.queryParams.pageSize=10
        this.getList()
    },
    getList(){
        equipmentPageList(this.queryParams).then(res=>{
            this.tableData=res.data.List
            this.total=res.data.Total
        })
    }
  },
};
</script>

<style lang="less" scoped>

.unbind_tips_dialog{
    ::v-deep .el-dialog__header{
      padding: 0;
    }
    .tips_con{
        display: flex;
        color: rgba(255, 255, 255, 1);
        font-size: 16px;
        line-height: 24px;
        i{
            color: rgba(241, 92, 92, 1);
            font-size: 16px;
            margin-right: 8px;
        }
    }
}
</style>