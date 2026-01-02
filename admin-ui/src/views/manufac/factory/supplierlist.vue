<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    
    <div style="background:#ffffff;border-radius:10px">
      <secondaryGrouping ref="secondaryGrouping" @handleGroupClick="handleGroupClick" table="供应商" :filterFiledList="supplierFiledList" @setFilterProp="setFilterProp"></secondaryGrouping>
      <el-row :gutter="20">
        <!--供应商数据-->
        <el-col :span="24" :xs="24">
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="createdSupplier">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">创建供应商</span>
                  </el-button>
                </el-col>
              </div>
              <div>
                <el-col :span="1.5">
                  <el-input @input="getList" v-model="queryParams.key" placeholder="请输入供应商关键字" clearable>
                    <i slot="suffix" class="el-input__icon el-icon-search"></i>
                  </el-input>
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
                    <div v-html="ingetFieldShow(scope.row, ite)"></div>
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
      <supplierAdd ref="supplierAdd" @reloadData="getList" @choiceMap="choiceMap"></supplierAdd>
      <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
      <!-- 添加或修改参数配置对话框 -->
    </div>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import supplierAdd from './component/supplierAdd.vue'
import {factorySupplierListPost,factorySupplierRemove} from '@/api/factory/supplier'
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
import {orgFormFields} from '@/api/factory/customFields'
import filterPopover from './component/filterPopover.vue'
import secondaryGrouping from './component/secondaryGrouping.vue'
import { getFieldShow } from '@/utils/field.js'
export default {
  name: 'AdminUiProductlist',
  mixins: [resizeTableCon],
  components:{supplierAdd,mapSelectCompt,filterPopover,secondaryGrouping},
  data() {
    return {
      total:0,
      activeName:'all',
      queryParams:{
        key:'',
        pageNum:1,
        pageSize:10,
      },
      dateRange:[],
      showSearch:true,
      dateTableList:[],
      // 列信息
      loading:false,
      ids:[],//选择的供应商
      supplierFiledList:[],//供应商字段
      activeFilter:[],//手动筛选过滤条件
      groupConditionJson:[],//分组过滤条件
      activeFiledList:[],//字段列表
      beforefilterProp:null
    };
  },

  mounted() {
    this.activeFiledList = [
      { "field": "Number", "fieldName": "供应商编号", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "SupplierName", "fieldName": "供应商名称", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "FullName", "fieldName": "供应商全称", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "ContactName", "fieldName": "联系人", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "Tel", "fieldName": "联系人手机号", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "StatusName", "fieldName": "供应商状态", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "updateTime", "fieldName": "更新时间", "type": "时间", "isShow": true, "isFixed": false },
    ]
    this.getList()
    this.loadOrgFormFields('供应商',true)
    this.$nextTick(()=>{
      this.$refs.secondaryGrouping.loadGroupViewList()
    })
  },

  methods: {
    ingetFieldShow(obj, field) {
      return getFieldShow(obj, field);
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
          { "field": "Number", "fieldName": "供应商编号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "SupplierName", "fieldName": "供应商名称", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "FullName", "fieldName": "供应商全称", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "ContactName", "fieldName": "联系人", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "Tel", "fieldName": "联系人手机号", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "StatusName", "fieldName": "供应商状态", "type": "文本", "isShow": true, "isFixed": false },
          { "field": "updateTime", "fieldName": "更新时间", "type": "时间", "isShow": true, "isFixed": false },
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
    async loadOrgFormFields(field,ext){//获取供应商字段列表
      let res=await orgFormFields({field:field,ext})
      // console.log("字段列表",res);
      this.supplierFiledList=JSON.parse(JSON.stringify(res.data))
    },
    choiceMap() {
      //选择位置
      this.$refs.mapSelectCompt.choiceMap();
    },
    returnMapInfo(info){
      this.$refs.supplierAdd.returnMapInfo(info)
    },
    filterSatus(value, row) {
      return row.Status === value;
    },
    createdSupplier(){
      this.$refs.supplierAdd.openDialog()//打开添加供应商的弹窗
    },
    handleClick(){
        //切换标签

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
    onClear(){

    },
    getList(){//供应商列表
      factorySupplierListPost(this.queryParams).then(res=>{
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
      this.$refs.supplierAdd.openDialog(row.Id)//打开添加供应商的弹窗
    },
    handleDelete(row){
      //删除
      let that=this
      this.$modal.confirm('是否确认删除供应商"' + row.SupplierName + '"？').then(function () {
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
.product_type_title{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 100%;

    .right_icon{
        margin-left: 15px;
        transform: rotate(90deg); /* 旋转90度 */
        /* 可选：如果你想让元素保持其原始大小，可以同时应用transform-origin */
        transform-origin: center center; /* 旋转中心点在元素中心 */
    }
    .line{
        margin-left: 8px;
        width: 2px;
        height: 22px;
        background: #EEEEEE;
    }
}
</style>