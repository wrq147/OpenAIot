<template>
  <el-drawer :title="title" :visible.sync="drawer" direction="rtl" :before-close="handleClose" size="700px">
    <div class="drawer_content_con" id="drawer_content_con">
      <el-tabs tab-position="left" v-model="activeName">
        <el-tab-pane label="基本信息" name="baseinfo"></el-tab-pane>
        <el-tab-pane label="字段配置" name="filedsetting"></el-tab-pane>
        <el-tab-pane label="数据过滤" name="datafilter"></el-tab-pane>
        <el-form ref="form" :model="form" label-width="80px" class="baseinfo_con" :rules="rules">
          <div class="form_title" v-show="activeName=='baseinfo'">基本信息</div>
          <el-form-item label="分组名称" v-show="activeName=='baseinfo'" prop="name">
            <el-input v-model="form.name" @change="typeNameChange"></el-input>
          </el-form-item>
          <el-form-item label="分组图标" prop="photoUrl" v-show="activeName=='baseinfo'">
            <image-upload v-model="form.photoUrl" :limit="1"></image-upload>
          </el-form-item>
          <el-form-item label="分组属性" v-show="activeName=='baseinfo'">
            <el-card class="box-card">
                <div slot="header" class="clearfix">
                    <el-button style="float: left; padding: 3px 0" type="text" @click="addProps" class="el-icon-circle-plus-outline">添加属性</el-button>
                </div>
                <table border style="width:100%;border-collapse: collapse;">
                  <tr><th>操作</th><th>属性名称</th></tr>
                  <tr v-for="(it,ix) in form.props" :key="'rul'+ix">
                    <td>
                      <i class="el-icon-remove-outline" style="color:#FF3B30;font-size:16px;" @click="delProps(ix)"></i>
                    </td>
                    <td>
                      <el-input @input="propsValChange" v-model="form.props[ix]" placeholder="请输入属性名称"></el-input>
                    </td>
                  </tr>
                </table>
            </el-card>
          </el-form-item>
        </el-form>
        <div class="filed_table" v-show="activeName=='filedsetting'">
          <div class="form_title" v-show="activeName=='filedsetting'">字段配置</div>
          <div class="zhuyi"><i class="el-icon-info" style="margin-right: 5px"></i>设置此产品分组下字段的顺序、显隐和固定</div>
          <div class="btn_con">
            <el-button size="mini" plain>全部显示</el-button>
            <el-button size="mini" plain style="margin-left:20px">全部隐藏</el-button>
          </div>
          <div class="search_con">
            <el-input placeholder="搜索字段" prefix-icon="el-icon-search" v-model="fieldSearch" @input="setSearchField"></el-input>
          </div>
          <div class="filed_table_ul" :style="{height:'calc('+divConHeight+'px - 230px)'}">
            <div class="filed_table_li" v-for="(it,inx) in fieldTable" :key="'fieldset'+inx+''+tabsKey" :class="{hide:!it.isShow}">
              <div class="li_left">
                <i class="el-icon-rank sorticon" v-if="!it.isFixed&&isCanSort"></i>
                <i class="el-icon-rank nosorticon" v-if="it.isFixed||!isCanSort"></i>
                <span class="text">{{it.fieldName}}</span>
              </div>
              <div class="li_right">
                <i class="el-icon-key" v-if="it.isFixed" style="color:#02B980;" @click="setlock(false,it.field)"></i>
                <i class="el-icon-key" v-if="!it.isFixed" @click="setlock(true,it.field)"></i>
                <i class="el-icon-view last" v-if="it.isShow" style="color:#02B980;" @click="setHide(false,it.field)"></i>
                <i class="el-icon-view last" v-if="!it.isShow" @click="setHide(true,it.field)"></i>
              </div>
            </div>
          </div>
        </div>
        <div class="filter_con" v-show="activeName=='datafilter'">
          <el-button class="nofocus" type="text" icon="el-icon-circle-plus-outline" plain @click="addFilterList">添加过滤条件</el-button>
          <div class="filter_ul">
            <template v-for="(item,inx) in filterList">
              <el-row class="filter_li" :gutter="10" :key="'filter'+inx+''+tabsKey2">
                <el-col :span="6">
                  <el-select v-model="filterList[inx].field" placeholder="请选择字段" @change="fieldSelectChange($event,inx)">
                    <el-option v-for="item in filterProductFiledList" :key="item.mapid" :label="item.name" :value="item.mapid"></el-option>
                  </el-select>
                </el-col>
                <el-col :span="4">
                  <el-select class="time_mini" placeholder="请选择比较符" v-model="filterList[inx].compare">
                    <template v-for=" it in returnCompareList(inx)">
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
                  <i class="el-icon-delete" @click="deleteFilter(inx)"></i>
                </el-col>
              </el-row>
            </template>
          </div>
        </div>
      </el-tabs>
      <div class="btn_con">
        <el-button type="primary" @click="setSaveFunc">保存</el-button>
        <el-button @click="handleClose">取消</el-button>
      </div>
    </div>
  </el-drawer>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {saveOrgField,orgFormFields} from '@/api/factory/customFields'
import {
  addProductTypeSave,
  editProductTypeSave,
  factoryProductTypeInfo,
  factorySearchObject
} from "@/api/factory/product";
import Sortable from 'sortablejs';
import dayjs from 'dayjs';
export default {
  name: 'AdminUiViewEdit',
  mixins: [resizeTableCon],
  data() {
    return {
      loading:false,
      title:'创建产品分组',
      drawer:false,
      form:{},
      activeName:'baseinfo',
      fieldTable:[],
      fieldSearch:'',//搜索字段
      originalField:[],
      divConHeight:0,
      isCanSort:true,
      tabsKey:0,
      tabsKey2:0,
      filterList:[],
      compareList:['大于','小于','大于等于','小于等于','不等于','等于','包含','不包含'],
      productFiledList:[],//产品字段
      rules: {
        name: [{ required: true, trigger: "blur", message: "产品分组名称不能为空" },],
        photoUrl: [{ required: true, trigger: "blur", message: "产品分组图片不能为空" },],
      },
      associationObject:{},//所有关联对象对应的下拉的参数列表
      
    };
  },
  computed:{
    filterProductFiledList(){
      //过滤文本，时间，数字类型的字段
      let list=this.productFiledList.filter(row=>row.type=='文本'||row.type=='时间'||row.type=='数字'||row.type=='关联对象')
      return list
    }
  },
  async mounted() {
    // await this.getorgFormFields()
  },

  methods: {
    typeNameChange(){
      if(this.form.id){
        return
      }
      let findIx=this.filterList.findIndex(row=>row.isdef)
      if(findIx!=undefined&&findIx>-1){
        this.filterList[findIx].val=this.form.name
      }
    },
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
    deleteFilter(inx){//删除过滤
      this.filterList.splice(inx,1)
      this.tabsKey2++
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
      this.tabsKey2++
      this.$forceUpdate()
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
    fieldSelectChange(val,inx){
      console.log(inx,'inx');
      // if(this.filterList[inx].field){
      //   let rowObj=this.filterProductFiledList.find(row=>row.mapid==this.filterList[inx].field)
      //   if(rowObj&&rowObj.type=='数字'||rowObj.type=='时间'){
      //     this.compareList=['大于','小于','大于等于','小于等于','不等于','等于']
      //   }else if(rowObj&&rowObj.type=='关联对象'){
      //     this.compareList=['关联','包含','不包含']
      //   }else{
      //     this.compareList=['等于','包含','不包含']
      //   }
      // }
      this.$forceUpdate()
    },
    returnCompareList(inx){
      console.log(inx,'inx');
      if(this.filterList[inx].field){
        let rowObj=this.filterProductFiledList.find(row=>row.mapid==this.filterList[inx].field)
        if(rowObj&&rowObj.type=='数字'||rowObj.type=='时间'){
          return ['大于','小于','大于等于','小于等于','不等于','等于']
        }else if(rowObj&&rowObj.type=='关联对象'){
          return ['关联','包含','不包含']
        }else{
          return ['等于','包含','不包含']
        }
      }
    },
    returnFormatDate(inx){
      let activeFiled=this.filterFiledType(inx)
      // console.log("时间类型",activeFiled);
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
    setHide(val,field){
      //设置是否隐藏
      this.fieldTable=this.fieldTable.map(rw=>{
        if(rw.field==field){
          rw.isShow=val
        }
        return rw
      })
      this.originalField=this.originalField.map(rw=>{
        if(rw.field==field){
          rw.isShow=val
        }
        return rw
      })
    },
    setlock(val,field){
      //设置是否固定
      this.fieldTable=this.fieldTable.map(rw=>{
        if(rw.field==field){
          rw.isFixed=val
        }
        return rw
      })
      let fixedArr=this.fieldTable.filter(row=>row.isFixed)
      let nofixedArr=this.fieldTable.filter(row=>!row.isFixed)
      this.fieldTable=[...fixedArr,...nofixedArr]
      this.originalField=this.originalField.map(rw=>{
        if(rw.field==field){
          rw.isFixed=val
        }
        return rw
      })
      let fixedArr2=this.originalField.filter(row=>row.isFixed)
      let nofixedArr2=this.originalField.filter(row=>!row.isFixed)
      this.originalField=[...fixedArr2,...nofixedArr2]
    },
    setSearchField(){
      //设置搜索
      if(this.fieldSearch){
        this.isCanSort=false
        this.fieldTable=this.originalField.filter(row=>row.field.toLowerCase().indexOf(this.fieldSearch.toLowerCase())>-1||row.fieldName.toLowerCase().indexOf(this.fieldSearch.toLowerCase())>-1)
      }else{
        this.fieldTable=JSON.parse(JSON.stringify(this.originalField))
        this.isCanSort=true
      }
    },
    async getorgFormFields(beforeVal){
      //获取产品的所有字段
      try {
        this.fieldTable=[]
        let res=await orgFormFields({field:'产品',ext:true})
        // console.log("产品所有字段",res);
        let data=res.data
        this.productFiledList=JSON.parse(JSON.stringify(res.data))
        if(data&&data.length>0){
          data.map(row=>{
            let obj=null
            if(beforeVal&&beforeVal.length>0){
              obj=beforeVal.find(rw=>row.mapid==rw.field)
            }
            if(obj){}else{
              obj={
                field:row.mapid,//字段
                fieldName:row.name,//字段名称
                type:row.type,
                isShow:true,//是否显示
                isFixed:false,//是否固定
              }
            }
            this.fieldTable.push(obj)
          })
        }
        this.originalField=JSON.parse(JSON.stringify(this.fieldTable))
        
      } catch (error) {
        console.log(error,'error');
      }
    },
    startSort() {
      this.showsort = true;
      //初始化排序
      let that=this
      const tbody = document.querySelector(".filed_table .filed_table_ul");
      new Sortable(tbody, {
        animation: 150,
        filter: ".nosorticon",
        // 需要在odEnd方法中处理原始eltable数据，使原始数据与显示数据保持顺序一致
        ghostClass: 'blue-background-class', // 拖动时元素的样式类
        handle: ".filed_table_li .li_left .sorticon",
        onEnd: ({ newIndex, oldIndex }) => {
          // console.log('排序结果',newIndex, oldIndex);
          let tmparr=JSON.parse(JSON.stringify(that.fieldTable))
          const targetRow = tmparr[oldIndex];
          if(!tmparr[newIndex].isFixed){
            tmparr.splice(oldIndex, 1);
            tmparr.splice(newIndex, 0, targetRow);
          }
          that.fieldTable=JSON.parse(JSON.stringify(tmparr))
          this.tabsKey++
          that.originalField=JSON.parse(JSON.stringify(tmparr))
          this.$nextTick(()=>{
            this.startSort()//重新设置排序
          })
        },
      });
    },
    setSaveFunc(){
      //保存产品分组
      this.$refs["form"].validate((valid,validateResult) => {
        if(valid){
          console.log(this.filterList,this.fieldTable);
          let filterList=JSON.parse(JSON.stringify(this.filterList))
          filterList=filterList.map(rw=>{
            let rowObj=this.productFiledList.find(row=>row.mapid==rw.field)
            if(rowObj&&rowObj.type=='时间'){
              rw.val_num=dayjs(rw.val).valueOf()
              rw.val=''
            }
            if(rw.val_num){}else {
              rw.val_num=null
            }
            return rw
          })
          let submitform={
            name:this.form.name,
            photoUrl:this.form.photoUrl,
            propList:this.form.props&&this.form.props.length>0?this.form.props.join(','):'',
            conditionJson:filterList&&filterList.length>0?JSON.stringify(filterList):'',
            listFieldsJson:this.fieldTable&&this.fieldTable.length>0?JSON.stringify(this.fieldTable):'',
          }
          if(this.form.id){
            submitform.id=this.form.id
            editProductTypeSave(submitform).then(res=>{
              this.$modal.msgSuccess("修改成功");
              this.drawer=false
              this.$emit('afterSave')
            }).catch(err=>{
              console.log("错误");
            })
          }else{
            addProductTypeSave(submitform).then(res=>{
              this.$modal.msgSuccess("添加成功");
              this.drawer=false
              this.$emit('afterSave')
            }).catch(err=>{
              console.log("错误");
            })
          }
          
        }else{
          this.activeName='baseinfo'
        }
      })
      
    },
    propsValChange(){
      //数据发生改变
      let form=JSON.parse(JSON.stringify(this.form))
      this.form=JSON.parse(JSON.stringify(form))
    },
    async setDrawOpen(id){
      
      if(id){
        this.title='修改产品分组'
        let res=await factoryProductTypeInfo({id:id})
        // console.log(res,'resres');
        this.form={
          id:id,
          sort:res.data.Sort,
          photoUrl:res.data.PhotoUrl,
          name:res.data.Name,
          props:res.data.PropList?res.data.PropList.split(','):[]
        }
        let fieldTable=res.data.ListFieldsJson?JSON.parse(res.data.ListFieldsJson):[]
        await this.getorgFormFields(fieldTable)
        let filterList=res.data.ConditionJson?JSON.parse(res.data.ConditionJson):[]
        
        filterList=filterList.map((rw,inx)=>{
          let rowObj=this.productFiledList.find(row=>row.mapid==rw.field)
          if(rowObj&&rowObj.type=='时间'){
            let newStr = rowObj.format.replace(/y/g, "Y");
            newStr =newStr.replace(/d/g, "D")
            rw.val=dayjs(rw.val_num).format(newStr)
          }else if(rowObj&&rowObj.type=='关联对象'){
            if(rw.compare=='关联'){
              this.afterValSearch(rw.val,rowObj,rw.field+inx)
            }
            
          }
          return rw
        })
        console.log(filterList,'filterListfilterList');
        this.filterList=JSON.parse(JSON.stringify(filterList))
        this.filterList.map((rw,ix)=>{
          this.fieldSelectChange('',ix)
        })
      }else{
        this.title='创建产品分组'
        this.filterList=[
          {
            field:'TypeName',
            compare:'等于',//比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
            val:'',//字符串
            val_num:null,//数字
            val_arr:[],//字符串数组
            isdef:true
          }
        ]
        this.fieldTable=[]
        this.form={}
        await this.getorgFormFields()
      }
      this.drawer=true
      this.$nextTick(()=>{
        let div = document.getElementById("drawer_content_con");
        this.divConHeight=div.offsetHeight
        this.startSort()
      })
    },
    async setSaveFilter(filterList){
      //另存新视图的操作
      this.filterList=JSON.parse(JSON.stringify(filterList))
      this.fieldTable=[]
      this.form={}
      await this.getorgFormFields()
      this.drawer=true
      this.$nextTick(()=>{
        let div = document.getElementById("drawer_content_con");
        console.log(div.offsetHeight,'容器高度');
        this.divConHeight=div.offsetHeight
        this.startSort()
      })
    },
    handleClose(){
      this.drawer=false
    },
    addProps(){
      //添加属性
      if(this.form.props){
        this.form.props.push('')
      }else{
        this.form.props=[]
        this.form.props.push('')
      }
      this.$forceUpdate()
    },
    delProps(ix){
      this.form.props.splice(ix,1)
      this.$forceUpdate()
    }
  },
};
</script>
<style lang="less" scoped>
.blue-background-class{
  background: #F2F2F2;
}
.form_title{
  font-weight: bold;
  color: #333333;
  font-size: 16px;
  margin-bottom: 10px;
}
.baseinfo_con{
  padding: 0 30px 0 10px;
  height: 100%;
}
.drawer_content_con{
  width: 100%;
  height: 100%;
  overflow: hidden;
  position: relative;;
}
.btn_con{
  text-align: right;
  // padding-right: 30px;
  width: 100%;
  border-top: 1px solid #F0F0F0;
  position: absolute;
  bottom: 0;
  right: 0;
  padding: 20px 30px 20px 0;
  background: #ffffff;
}
.filed_table{
  padding: 0 30px 0 10px;
  width: 100%;
  height: 100%;
  box-sizing: border-box;
  .zhuyi{
    font-size: 12px;
    color: #999999;
  }
  .btn_con{
    text-align: left;
    margin-top: 10px;
    position: relative;
    padding: 0;
    border-top: 0;
  }

  .search_con{
    width: 100%;
    margin-top: 10px;
  }
  .filed_table_ul{
    // max-height: calc(100% - 200px);
    flex: 1;
    position: relative;
    overflow-y: scroll;
    margin-top: 10px;
    .filed_table_li:hover{
      background: #F2F2F2;
    }
    .filed_table_li{
      width: 100%;
      box-sizing: border-box;
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: #333333;
      font-size: 14px;
      line-height: 36px;
      border-radius: 5px;
      padding: 0 10px;
      box-sizing: border-box;
      &.hide{
        opacity: 0.5;
      }
      .li_left{
        .sorticon{
          cursor: pointer;
        }
        .text{
          margin-left: 20px;
        }
        .nosorticon{
          color: #999999;
        }
      }
      .li_right{
        .last{
          margin-left: 20px;
          padding-right: 10px;
        }
      }
    }
  }
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
.nofocus.el-button.is-plain:focus, .nofocus.el-button.is-plain:hover{
  border: none;
}
</style>