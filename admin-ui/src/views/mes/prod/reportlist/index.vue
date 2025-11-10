<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    
    <div style="background:#ffffff;border-radius:10px">
      <secondaryGrouping ref="secondaryGrouping" @handleGroupClick="handleGroupClick" table="报工" :filterFiledList="supplierFiledList" @setFilterProp="setFilterProp"></secondaryGrouping>
      <el-row :gutter="20">
        <!--报工数据-->
        <el-col :span="24" :xs="24">
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="createdReport">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加报工</span>
                  </el-button>
                </el-col>
              </div>
              <div>
                <el-col :span="1.5">
                  <el-date-picker class="set_radius" v-model="dateRange" style="width: 250px" value-format="yyyy-MM-dd HH:mm:ss" type="datetimerange" range-separator="-" start-placeholder="开始时间" end-placeholder="结束时间"></el-date-picker>
                  <!-- <el-input @input="getList" v-model="queryParams.key" placeholder="请输入报工关键字" clearable>
                    <i slot="suffix" class="el-input__icon el-icon-search"></i>
                  </el-input> -->
                </el-col>
                <el-col :span="1.5">
                  <filterPopover :filterFiledList="supplierFiledList" :hasSaveButton="true" @finishSelect="finishSelect" @setSaveFilterList="setSaveFilterList"></filterPopover>
                </el-col>
              </div>
            </el-row>
            <el-table v-loading="loading" border :data="dateTableList" :row-style="isRed" @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty"
              style="width:100%" :fit="true">
              <template v-for="ite in activeFiledList">
                <el-table-column :fixed="ite.isFixed ? 'left' : false" v-if="ite.isShow" :key="ite.field"
                  :label="ite.fieldName" align="center" :prop="ite.field" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span v-if="ite.type == '时间'">{{ parseTime(scope.row[ite.field]) }}</span>
                    <span v-else-if="ite.type == '图片'">
                      <el-image fit="cover" style="width:54px;height:54px" :src="scope.row[ite.field] + '?wh=500x500'">
                        <div slot="error" class="image-slot">
                          <i class="el-icon-picture-outline"></i>
                        </div>
                      </el-image>
                    </span>
                    <span v-else-if="ite.type == '关联对象'">{{ returnObjectName(scope.row[ite.field]) }}</span>
                    <span v-else>{{ scope.row[ite.field] }}</span>
                  </template>
                </el-table-column>
              </template>
              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                  <el-button type="text" icon="el-icon-edit" @click="handleDelete(scope.row)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
          </div>
        </el-col>
      </el-row>
      <!-- 添加或修改参数配置对话框 -->
    </div>
    <reportAdd ref="reportAdd" @reloadData="getList" @onOpenWorkTask="onOpenWorkTask"></reportAdd>
    <slectTaskList title="选择生产任务" :dialogVisible="taskDialogVisible" @taskSelect="taskSelect" @cancelForm="taskCancelForm"></slectTaskList>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import filterPopover from '@/views/manufac/factory/component/filterPopover.vue'
import secondaryGrouping from '@/views/manufac/factory/component/secondaryGrouping.vue'
import reportAdd from '@/views/mes/prod/component/reportAdd'
import slectTaskList from '@/views/mes/prod/component/slectTaskList'
import {ReportList} from '@/api/mes/report'
import {orgFormFields} from '@/api/factory/customFields'
export default {
  name: 'AdminUiReportlist',
  mixins: [resizeTableCon],
  components:{filterPopover,secondaryGrouping,reportAdd,slectTaskList},
  data() {
    return {
      total:0,
      activeName:'all',
      queryParams:{
        key:'',
        pageNum:1,
        pageSize:10,
      },
      showSearch:true,
      dateTableList:[],
      loading:false,
      ids:[],//选择的报工
      supplierFiledList:[],//报工字段
      activeFilter:[],//手动筛选过滤条件
      groupConditionJson:[],//分组过滤条件
      activeFiledList:[],//字段列表
      beforefilterProp:null,
      dateRange:[],//过滤开始时间和结束时间
      taskDialogVisible:false
    };
  },

  mounted() {
    this.activeFiledList = [
          { "field": "WorkOrderId", "fieldName": "关联的工单Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "WorkTaskId", "fieldName": "关联的任务Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "OperId", "fieldName": "关联的工序Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "Number", "fieldName": "唯一编号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "BatchNo", "fieldName": "批次编号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "GoodNum", "fieldName": "良品数", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "DefectNum", "fieldName": "不良品数", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "defectStr", "fieldName": "不良品项", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "startWork", "fieldName": "开始时间", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "endWork", "fieldName": "结束时间", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "workTime", "fieldName": "报工时长", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "Status", "fieldName": "状态", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "updateTime", "fieldName": "更新时间", "type": "时间", "isShow": true, "isFixed": false },
          { "field": "overReason", "fieldName": "超时原因", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "flowId", "fieldName": "关联的审核流程Id", "type": "文本", "isShow": false, "isFixed": false },
        ]
    this.getList()
    this.loadOrgFormFields('报工',true)
    this.$nextTick(()=>{
      this.$refs.secondaryGrouping.loadGroupViewList()
    })
  },

  methods: {
    taskCancelForm(){
      this.taskDialogVisible=false
    },
    taskSelect(val){
      //生产计划选择
      console.log("选择的生产计划",val);
      this.$refs.reportAdd.setTaskSelect(val)
    },
    onOpenWorkTask(){
      this.taskDialogVisible=true
    },
    createdReport(){
      this.$refs.reportAdd.openDialog()//打开添加报工的弹窗
    },
    setSaveFilterList(filterList) {
      //筛选另存为新分组
      this.$refs.secondaryGrouping.setSaveFilterList(filterList)
    },
    setFilterProp(propFilter){
      let itemsArr=JSON.parse(JSON.stringify(this.queryParams.items))
      if(this.beforefilterProp){
        this.queryParams.items=itemsArr.filter(row=>JSON.stringify(row)!=JSON.stringify(this.beforefilterProp))
      }
      
      if(propFilter){
        this.beforefilterProp=JSON.parse(JSON.stringify(propFilter))
        if(this.queryParams.items&&this.queryParams.items.length>0){
        }else{
          this.queryParams.items=[]
        }
        this.queryParams.items.push(propFilter)
      }else{
        this.beforefilterProp=null
        if(!this.queryParams.items&&this.queryParams.items&&this.queryParams.items.length==0){
          delete this.queryParams.items
        }
      }
      this.queryParams.pageNum = 1
      this.getList()
    },
    returnObjectName(val) {//显示关联对象字段的名称
      if (val && val.indexOf(',') > -1) {
        let arr = val.split(',')
        return arr[1]
      } else {
        return ''
      }
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    handleGroupClick(activeName,activeFiledList,ConditionJson){
      if(activeName!='all'){
        if(ConditionJson){
          this.groupConditionJson=JSON.parse(JSON.stringify(ConditionJson))
        }else{
          this.groupConditionJson=[]
        }
        this.activeFiledList=JSON.parse(JSON.stringify(activeFiledList))
        this.queryParams.items=[...this.activeFilter,...this.groupConditionJson]
        this.queryParams.pageNum = 1
        this.getList()
      }else{
        this.activeFiledList = [
          { "field": "WorkOrderId", "fieldName": "关联的工单Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "WorkTaskId", "fieldName": "关联的任务Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "OperId", "fieldName": "关联的工序Id", "type": "文本", "isShow": false, "isFixed": false },
          { "field": "Number", "fieldName": "唯一编号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "BatchNo", "fieldName": "批次编号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "GoodNum", "fieldName": "良品数", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "DefectNum", "fieldName": "不良品数", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "defectStr", "fieldName": "不良品项", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "startWork", "fieldName": "开始时间", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "endWork", "fieldName": "结束时间", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "workTime", "fieldName": "报工时长", "type": "数字", "isShow": true, "isFixed": false },
          { "field": "Status", "fieldName": "状态", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "updateTime", "fieldName": "更新时间", "type": "时间", "isShow": true, "isFixed": false },
          { "field": "overReason", "fieldName": "超时原因", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "flowId", "fieldName": "关联的审核流程Id", "type": "文本", "isShow": false, "isFixed": false },
        ]
        this.groupConditionJson=[]
        // delete this.queryParams.typeId
        if(this.activeFilter&&this.activeFilter.length>0){
          this.queryParams.items=this.activeFilter
        }else{
          delete this.queryParams.items
        }
        this.getList()
      }
      
    },
    finishSelect(items){//完成搜索
      this.activeFilter=items?JSON.parse(JSON.stringify(items)):[]
      if (items && items.length > 0) {
        this.queryParams.items=[...items,...this.groupConditionJson]
      } else {
        if(this.groupConditionJson&&this.groupConditionJson.length>0){
          this.queryParams.items=[...this.groupConditionJson]
        }else{
          delete this.queryParams.items
        }
      }
      this.queryParams.pageNum=1
      this.getList()
    },
    async loadOrgFormFields(field,ext){//获取报工字段列表
      let res=await orgFormFields({field:field,ext})
      // console.log("字段列表",res);
      this.supplierFiledList=JSON.parse(JSON.stringify(res.data))
    },
    getList(){//报工列表
      ReportList(this.queryParams).then(res=>{
        // console.log("查询到",res);
        this.dateTableList=res.data.List
        this.total=res.data.Total
      })
    },
    resetQuery(){

    },
    handleQuery(){

    },
    handleUpdate(row){
      //修改
      this.$refs.reportAdd.openDialog(row.Id)//打开添加报工的弹窗
    },
    handleDelete(row){
      //删除
      let that=this
      this.$modal.confirm('是否确认删除报工"' + row.SupplierName + '"？').then(function () {
        return factorySupplierRemove({ id:row.Id });
      }).then(() => {
        that.getList();
        that.$modal.msgSuccess("移除成功");
      }).catch((err) => { 
        console.log("错误",err);
      });
    }
  },
};
</script>

<style lang="less" scoped>

</style>