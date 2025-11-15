<template>
  <el-popover placement="bottom-end" title="" width="600" trigger="manual" content="" v-model="filterVisible" :visible-arrow="false">
    <div class="filter_con">
    <div class="filter_title">
        <span class="break">筛选出符合以下</span>
        <span class="grey">所有<i class="el-icon-caret-bottom"></i></span>
        <span class="break">条件的数据</span>
    </div>
    <el-button class="nofocus" type="text" icon="el-icon-circle-plus-outline" plain @click="addFilterList">添加过滤条件</el-button>
    <div class="filter_ul">
      <template v-for="(item,inx) in filterList">
        <el-row class="filter_li" :gutter="10" :key="'filter'+inx">
            <el-col :span="6">
            <el-select v-model="filterList[inx].field" placeholder="请选择字段" @change="fieldSelectChange($event,inx)">
                <el-option v-for="item in filterProductFiledList" :key="item.mapid" :label="item.name" :value="item.mapid"></el-option>
            </el-select>
            </el-col>
            <el-col :span="4">
            <el-select class="time_mini" placeholder="请选择比较符" v-model="filterList[inx].compare">
                <template v-for=" it in compareList">
                    <el-option :label="it" :value="it" :key="it"></el-option>
                </template>
                
            </el-select>
            </el-col>
            <el-col :span="12" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='数字'">
              <el-input type="number" v-model="filterList[inx].val_num" placeholder="请输入值"></el-input>
            </el-col>
            <el-col :span="12" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='时间'">
              <el-date-picker style="width:100%" v-model="filterList[inx].val" type="datetime" placeholder="请选择值" :value-format="returnFormatDate(inx)" :format="returnFormatDate(inx)"></el-date-picker>
            </el-col>
            <el-col :span="12" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='文本'">
              <el-input type="text" v-model="filterList[inx].val" placeholder="请输入值"></el-input>
            </el-col>
            <el-col :span="12" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='关联对象'">
              <el-input type="text" v-model="filterList[inx].val" placeholder="请输入值" v-if="filterList[inx].compare!='关联'"></el-input>
              <el-select @focus="afterValSearch(filterList[inx].val,filterFiledType(inx),filterList[inx].field+inx)" :clearable="true" style="width: 100%"
                v-model="filterList[inx].val" filterable remote reserve-keyword placeholder="请选择" :loading="loading"
                :remote-method="(query)=>associationMethod(query,filterFiledType(inx),filterList[inx].field+inx)" v-if="filterList[inx].compare=='关联'&&filterList[inx].field">
                <el-option v-for="ite in associationObject[filterList[inx].field+inx]" :key="ite.Value" :label="ite.Name" :value="ite.Value+','+ite.ValueName">{{ite.Name}}</el-option>
              </el-select>
            </el-col>
            <el-col :span="2">
                <i class="el-icon-delete"></i>
            </el-col>
        </el-row>
      </template>
    </div>
    <div class="btn_con">
        <div class="btn_left">
            <el-button type="primary" @click="setSearchQuery" :disabled="!filterList||filterList.length==0">筛选</el-button>
            <el-button @click="clearFilter" :disabled="!filterList||filterList.length==0">清空</el-button>
        </div>
        <el-button v-if="hasSaveButton" class="nofocus" type="text" plain @click="saveFilterList" :disabled="!filterList||filterList.length==0">另存为新分组</el-button>
    </div>
    </div>
    <el-button type="primary" plain @click="filterVisible=!filterVisible" slot="reference">
        <i class="zhongtaiiconfont zhongtai-icon-shaixuan"></i>
        <span style="margin-left:6px">筛选</span>
    </el-button>
  </el-popover>
</template>

<script>
import {
  factorySearchObject
} from "@/api/factory/product";
export default {
  name: 'AdminUiFilterPopover',
  props:{
    filterFiledList:{
        type:Array,
        default:()=>{
            return []
        }
    },
    hasSaveButton:{
      type:Boolean,
      default:true
    }
  },
  data() {
    return {
      filterVisible:false,//视图的弹窗是否显示
      filterList:[],//过滤条件列表
      compareList:['大于','小于','大于等于','小于等于','不等于','等于','包含','不包含'],
      associationObject:{},//所有关联对象对应的下拉的参数列表
      loading:false,
    };
  },
  computed:{
    filterProductFiledList(){
      //过滤文本，时间，数字类型的字段
      let list=this.filterFiledList.filter(row=>row.type=='文本'||row.type=='时间'||row.type=='数字'||row.type=='关联对象')
      return list
    }
  },
  mounted() {
    
  },

  methods: {
    afterValSearch(val,item,keymapId){//关联对象回显时获取列表
      if(val&&val.indexOf(',')>-1){
        let keyVal=val.split(',')
        this.associationMethod(keyVal[1],item,keymapId)
      }else{
        this.associationMethod('',item,keymapId)
      }
    },
    associationMethod(query,item,keymapId){//关联对象的远程搜索事件
      // console.log("关联对象",query);
      this.getFactorySearchObject(query,item.object_type,keymapId)
    },
    async getFactorySearchObject(key,objtype,mapid){
      //根据不同的关联对象获取对象的列表
      let obj={
        key:key,
        objtype:objtype,
        pageNum:1,
        pageSize:10
      }
      let res=await factorySearchObject(obj)
      if(res.data.List){
        // console.log("res.data.List",res.data.List,mapid,this.associationObject);
        this.associationObject[mapid]=JSON.parse(JSON.stringify(res.data.List))
      }
      this.$forceUpdate()
      // console.log(res,'resres');
    },
    fieldSelectChange(val,inx){
      if(this.filterList[inx].field){
        let rowObj=this.filterProductFiledList.find(row=>row.mapid==this.filterList[inx].field)
        if(rowObj&&rowObj.type=='数字'||rowObj.type=='时间'){
          this.compareList=['大于','小于','大于等于','小于等于','不等于','等于']
        }else if(rowObj&&rowObj.type=='关联对象'){
          this.compareList=['关联','包含','不包含']
        }else{
          this.compareList=['等于','包含','不包含']
        }
      }
      this.$forceUpdate()
    },
    clearFilter(){
        this.filterList=[]
        this.$emit('finishSelect',[])
    },
    setSearchQuery(){
      let filterList=JSON.parse(JSON.stringify(this.filterList))
      filterList=filterList.map(rw=>{
        let rowObj=this.filterFiledList.find(row=>row.mapid==rw.field)
        if(rowObj&&rowObj.type=='时间'){
          rw.val_num=dayjs(rw.val).valueOf()
          rw.val=''
        }
        if(rw.val_num){}else{
          delete rw.val_num
        }
        if(rw.val_arr&&rw.val_arr.length>0){}else{
          delete rw.val_arr
        }
        return rw
      })
      this.filterVisible=false
      this.$emit('finishSelect',filterList)
    },
    saveFilterList(){
      this.$emit('setSaveFilterList',this.filterList)
    },
    filterFiledType(inx){
      if(this.filterList[inx]&&this.filterList[inx].field){
        let findObj=this.filterProductFiledList.find(row=>row.mapid==this.filterList[inx].field)
        // console.log(findObj,'findObj');
        return findObj
      }else{
        return {}
      }
    },
    returnFormatDate(inx){
      let activeFiled=this.filterFiledType(inx)
      console.log("时间类型",activeFiled);
      if(activeFiled&&activeFiled.format){
        switch (activeFiled.format){
          case '日':
            return 'yyyy-MM-dd';
          case '时':
            return 'yyyy-MM-dd HH';
          case '分':
            return 'yyyy-MM-dd HH:mm';
          case '秒':
            return 'yyyy-MM-dd HH:mm:ss';
        }
      }
    },
    addFilterList(){
      //添加过滤条件
      let rowObj={
        field:'',
        compare:'',//比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
        val:'',//字符串
        val_num:null,//数字
        val_arr:[]//字符串数组
      }
      this.filterList.push(rowObj)
      this.$forceUpdate()
    },
  },
};
</script>

<style lang="scss" scoped>
.btn_con{
    display: flex;
    justify-content: space-between;
    align-items: center;
}
.filter_con{
  .filter_title{
    font-size: 14px;
    color: #333333;
    .grey{
      color: #838892;
      margin: 0 10px;
      i{
        color: #838892;
        margin-left: 3px;
      }
    }
  }
  .filter_ul{
    .filter_li{
      display: flex;
      align-items: center;
      margin-bottom: 10px;
    }
  }
  
}
.nofocus.el-button.is-plain{
  border: none;
}
.nofocus.el-button.is-plain:focus, .nofocus.el-button.is-plain:hover{
  border: none;
}
.el-form.biaodan .el-button--primary{
  background: #e8f4ff;
}
.el-form.biaodan .el-button--primary.is-plain:hover{
  background: #1890FF;
}
</style>