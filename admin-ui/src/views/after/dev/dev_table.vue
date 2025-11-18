<template>
  <div>
    <el-row :gutter="10" class="mb8 button_row">
      <div>
        <el-col :span="1.5">
          <el-button type="warning" plain @click="openChoicePrint2" :disabled="multiple">
            <i class="el-icon-printer"></i>
            <span style="margin-left: 6px">打印</span>
          </el-button>
          <el-button type="info" plain @click="statisticalAnalysis">
            <i class="el-icon-s-operation"></i>
            <span style="margin-left: 6px">统计分析</span>
          </el-button>
        </el-col>
      </div>
    </el-row>
    <el-table v-loading="configLoading" border :data="tableData" :row-style="isRed" @selection-change="handleSelectionChange" class="data_table"
      :header-cell-style="cellSty" style="width: 100%" :fit="true">
      <el-table-column type="selection" width="55"></el-table-column>

      <el-table-column label="批次编号" align="center" key="DeviceNumber" prop="DeviceNumber" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span>{{ scope.row.DeviceNumber }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备名称" align="center" key="Name" prop="Name" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span>{{ scope.row.Name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="车间" align="center" key="RoomName" prop="RoomName" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span>{{ scope.row.RoomName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="联网状态" align="center" key="Online" prop="Online" :show-overflow-tooltip="true">
        <template slot-scope="scope">
            <span>{{ returnOnlineState(scope.row) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="运行状态" align="center" key="DState" prop="DState"></el-table-column>
      <el-table-column label="产品" align="center" key="ProductName" prop="ProductName" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span>{{ scope.row.ProductName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建日期" align="center" prop="CreateOn" width="180">
        <template slot-scope="scope">
          <span>{{ parseTime(scope.row.CreateOn) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="130" fixed="right" class-name="small-padding fixed-width">
        <template slot-scope="scope">
            <el-button type="text" icon="el-icon-copy-document" @click="toDeviceDetails(scope.row)">详情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <EmbedPrint2 @closePrint="closePrint" ref="printDlg2" v-if="showPrintParams"></EmbedPrint2>
    <print_list ref="print_list" @choiceTemplete="openPrint"></print_list>
    <dev_Analysis ref="analysis"></dev_Analysis>
  </div>
</template>

<script>
import EmbedPrint2 from "@/views/report/print/EmbedMultiplePrint";
import print_list from "./print_list";
import dev_Analysis from './dev_Analysis'
export default {
  name: "AdminUiDevTable",
  components: { print_list,EmbedPrint2,dev_Analysis },
  data() {
    return {
        // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      activeId:0,
      printMul:false,
      showPrintParams:false
    };
  },
  props:{
    tableData:{
        type:Array,
        default:()=>{
            return []
        }
    },
    configLoading:{
        type:Boolean,
        default:true
    }
  },
  mounted() {},

  methods: {
    cellSty({ row, rowIndex }) {
        if (rowIndex == 0) {
            let obj = {
                'color': '#78829D',
                'background': '#F9FAFC !important'
            }
            return obj;
        }
    },
    statisticalAnalysis(){
      this.$refs.analysis.openDialog()
    },
    returnOnlineState(row){
      if(row.Online == 0){
        return '离线'
      }else if(row.Online == 1){
        return '在线'
      }else if(row.Online == 2){
        return '未知'
      }
    },
    openChoicePrint2(){
      if(!this.multiple){
        this.printMul=true
        this.$refs.print_list.openDia()
      }
      
    },
    openChoicePrint(item){
      this.printMul=false
      this.activeId=item.Id
      this.$refs.print_list.openDia()
    },
    openPrint(tempId){
      this.showPrintParams=true
      this.$nextTick(()=>{
        if(this.printMul){
          let queryList=this.ids.map(row=>{return {id:row}})
          this.$refs.printDlg2.startPrint(tempId,queryList)
        }else{
          this.$refs.printDlg.showPrint(tempId,{id:this.activeId})
        }
      })
      
    },
    closePrint(){
      //关闭打印弹窗
      this.showPrintParams=false
    },
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    toDeviceDetails(it){
        this.$emit('toDeviceDetails',it)
    }
  },
};
</script>
