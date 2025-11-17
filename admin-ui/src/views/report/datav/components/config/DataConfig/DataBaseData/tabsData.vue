<template>
  <div style="position: relative;" class="tabs_con" :style="{'--absoluteWidth':absoluteWidth+'px'}">
    <div class="level_class" @click="levelSetting" v-if="editableTabsValue=='0' && isShowrequest">配置层级</div>
    <div class="js_ul" v-show="isShowJsList">
      <template v-for="(ite,inx) in jslist">
        <div class="js_li" :key="'js'+inx" @click="handTabsJscriptAdd(ite)" :class="{'first':inx==0}">{{ite.text}}</div>
        
        <!-- <div class="js_li">对象转数组</div>
        <div class="js_li">根据字段分组</div> -->
      </template>
      <div class="js_li" @click="handTabsJscriptAdd()"><i class="el-icon-plus"></i><text style="margin-left:5px;">添加</text></div>
    </div>
    <el-tabs v-model="editableTabsValue" type="border-card" editable @edit="handleTabsEdit">
      <el-tab-pane v-for="(item, index) in editableTabs" :key="'tab' + item.name" :label="item.title" :name="item.name" @click="isShowJsList=false">
        <div class="editor" style="border: 1px solid darkgrey">
          
          <!-- <div v-if="item.name === '0'">
           <div :style="[{'height': isShowrequest ? '230px' : 0}, {'overflow-y': 'auto'}]" :contenteditable="isShowrequest?false:true">
             <JsonView :data="item.content" v-if="!isShowrequest"></JsonView>
             <tree_single :ref="'treeCompent_'+item.name" v-if="isShowrequest" :treeData="cascaderOptions" :treeDataListLoading="treeDataListLoading" @changeFilterFiled="changeFilterFiled"></tree_single>
           </div>
          </div> -->
          <!-- <el-input v-if="item.name !== '0'" class="sccls" ref="iptEditor" type="textarea" :rows="4" placeholder="点击此处编辑" @focus="onScriptEdit(index)" :value="item.resultPreCode"/> -->
          <el-input v-if="item.name !== '0'&&item.requestType == '0'" class="sccls" ref="iptEditor" type="textarea" :rows="4" placeholder="点击此处编辑" @focus="onScriptEdit(index)" :value="item.resultPreCode"/>
          <el-input v-else-if="item.name !== '0'" class="sccls" ref="iptEditor" type="textarea" :rows="4" placeholder="点击此处编辑" @focus="onScriptEdit(index)" :value="item.resultPrePath"/>
          <tableData ref="tableData" :fielForm="item.fielForm ? item.fielForm : []" :data="item.content" @getFieldForm='getFieldForm'/>
          <el-dialog v-if="scriptEditorOpen" title="编辑数据处理脚本" append-to-body :close-on-click-modal="false" :visible.sync="scriptEditorOpen" width="750px" top="2vh" :destroy-on-close="true">
            <div style="margin-bottom: 10px" v-if="isShowrequest">
              <label style="display: inline-block; margin-right: 20px; width: 98px">过滤数据的方式</label>
              <el-radio-group v-model="editableTabs[editableTabsValueIndex].requestType">
                <el-radio label="1">选择层级</el-radio>
                <el-radio label="0">Javescript</el-radio>
              </el-radio-group>
            </div>
            <div style="margin-top: 30px" v-if="editableTabs[editableTabsValueIndex].requestType == '1'">
              <label style="display: inline-block; margin-right: 20px; width: 98px">层级</label>
              <el-cascader style="width:calc(100% - 118px)" :options="cascaderOptions" :props="{ checkStrictly: true }" clearable v-model="editableTabs[editableTabsValueIndex].cascaderValue"></el-cascader>
            </div>
            <data-editor v-if="editableTabs[editableTabsValueIndex].requestType == '0'" @submitData="submitData" @cancelData="scriptEditorOpen = false" :customData="editableTabs[editableTabsValueIndex].resultPreCode"/>
            <div slot="footer" class="dialog-footer" v-if="editableTabs[editableTabsValueIndex].requestType == '1'">
              <el-button @click="scriptEditorOpen = false">取 消</el-button>
              <el-button type="primary" @click="submitData()">确 定</el-button>
            </div>
          </el-dialog>
          <el-dialog v-if="levelSelectOpen" title="选择层级" append-to-body :close-on-click-modal="false" :visible.sync="levelSelectOpen" width="750px" top="2vh" :destroy-on-close="true">
            <div style="margin-top: 30px">
              <label style="display: inline-block; margin-right: 20px; width: 98px">配置层级</label>
              <el-cascader style="width:calc(100% - 118px)" :options="cascaderOptions" :props="{ checkStrictly: true }" clearable v-model="cascaderValue"></el-cascader>
            </div>
            <div slot="footer" class="dialog-footer">
              <el-button @click="levelSelectOpen = false">取 消</el-button>
              <el-button type="primary" @click="finishLevelSetting">确 定</el-button>
            </div>
          </el-dialog>
        </div>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
var GlobalApiData={}//用于填写js时赋值数据集的数据
import DataEditor from "../../runcode/DataEditor";
import JsonView from "vue-json-views";
import tableData from "./tableData";
import {jslist,jsObject} from './general'
export default {
  components: {
    JsonView,
    DataEditor,
    tableData,
  },
  props:{
    isShowrequest: {
      type: Boolean,
      default: false,
    },
    globalData:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      absoluteWidth:120,
      levelSelectOpen:false,
      treeDataListLoading:'',//树结构是否加载中
      cascaderOptions: [], //层级相关数据
      editableTabsValue: "0",
      editableTabsValueIndex:0,
      scriptEditorOpen: false,
      editableTabs: [
        { title: "默认数据", content: [], name: "0",cascaderValue:[],resultPrePath:'', resultPreCode: "",selectKeyFiled:'' },
      ],
      resultPreCodeList: {},
      resultPreCode: "",
      tabIndex: 0,
      data: [], // 执行sql语句 返回的数据
      dataMap:new Map(),
      resultDataTree:[],
      cascaderValue:[],
      jslist:jslist,//脚本列表
      jsObject:jsObject,//对应脚本
      isShowJsList:false
    };
  },
  watch: {
    globalData:{
      handler(to){
        to.map(row=>{
          GlobalApiData[row.name]=row
        })
      },
      immediate:true,
      deep:true
    },
    editableTabsValue:{
      handler: function (val) {
        if(val=='0'){
          this.absoluteWidth=120
        }else{
          this.absoluteWidth=0
        }
        let index=this.editableTabs.findIndex(row=>row.name==val)
        if(index!==null&&index!==undefined){
          this.editableTabsValueIndex= index
        }else{
          this.editableTabsValueIndex= 0

        }
      },
      immediate:true,
    },
    editableTabs: {
      handler: function (val) {
        this.$emit("baseDataTotal", this.editableTabs);
      },
      deep: true,
    },
    data: {
      handler: function (val) {
        this.dataMap=new Map()
        this.treeDataListLoading=true
        try {
          this.getTreeParams(val,'')
          this.setKeysTree()
          this.treeDataListLoading=false
        } catch (error) {
          this.treeDataListLoading=false
        }
        
      },
      deep: true,
    },
  },
  methods: {
    // 节点配置数据保存
    getFieldForm(form) {
      this.editableTabs[this.editableTabsValueIndex].fielForm = form
    },
    levelSetting(){
      //层级配置
      if(this.editableTabs[0]&&this.editableTabs[0].selectKeyFiled){}else{this.cascaderValue=[]}
      
      this.levelSelectOpen=true
    },
    finishLevelSetting(){
      let filedKey=''
      for(let ix=0;ix<this.cascaderValue.length;ix++){
        let r=this.cascaderValue[ix]
        if(ix==0){
          filedKey=r
        }else{
          filedKey=filedKey+'/'+r
        }
      }
      this.editableTabs[0].content=this.changeFilterFiled(filedKey)
      this.editableTabs[0].selectKeyFiled=filedKey
      this.levelSelectOpen=false
    },
    changeFilterFiled(filedKey){
      let resData=[]
      if(this.editableTabs&&this.editableTabs[0]){
        if(filedKey){
          let pathFiled=filedKey
          if(this.dataMap.get(pathFiled)){
            let rsData=this.dataMap.get(pathFiled)
            if (typeof rsData === "string"){
              resData=JSON.parse(this.dataMap.get(pathFiled))
            }else{
              resData=this.dataMap.get(pathFiled)
            }
          }
        }else{
          resData=JSON.parse(JSON.stringify(this.data))
        }
      }
      return resData
      
    },
    getTreeParams(val,orgPath) {
      for(let i=0;i<val.length;i++){
        let row=val[i]
        for(let key in row){
          let keyPath=orgPath+key
          if(row[key]&&typeof(row[key])=='object'&&row[key]&&row[key].length==undefined){//对象类型数据的处理
            for(let keyName in row[key]){
              let keyPath2=keyPath+'_'+keyName
              let valObj={}
              if(this.dataMap.get(keyPath2)){
                let hsval=this.dataMap.get(keyPath2)
                valObj[key+'_'+keyName]=row[key][keyName]
                hsval.push(valObj)
                this.dataMap.set(keyPath2,hsval)
              }else{
                valObj[key+'_'+keyName]=row[key][keyName]
                this.dataMap.set(keyPath2,[valObj])
              }
              
            }
          }else{
            if(this.dataMap.get(keyPath)){//已经有设置过值
              let hsval=this.dataMap.get(keyPath)
              const value = row[key];
              let valObj={}
              if(Array.isArray(value)){//数组类型数据的处理
                let hsval2=JSON.parse(hsval)
                let afterArr=[...Array.from(hsval2),...value]
                this.dataMap.set(keyPath,JSON.stringify(afterArr))
                this.getTreeParams(value,keyPath+'/')
              }else{
                if(value!=null&&value!=undefined){
                    valObj[key]=value
                    hsval.push(valObj)
                    this.dataMap.set(keyPath,hsval)
                  }
              }
            }else{//初始设置值
              const value = row[key];
              let valObj={}
              if(value&&Array.isArray(value)){//数组类型数据的处理
                this.dataMap.set(keyPath,JSON.stringify(value))
                this.getTreeParams(value,keyPath+'/')
              }else{
                if(value!=null&&value!=undefined){
                  valObj[key]=value
                  this.dataMap.set(keyPath,[valObj])
                  
                }
              }
            }
          }
          
        }
      }
    },
    returnParams(filedKey,treeArr){
      for(let i=0;i<treeArr.length;i++){
        if(treeArr[i].id==filedKey){
          return treeArr[i]
        }else{
          if(treeArr[i].children&&treeArr[i].children.length>0){
            return this.returnParams(filedKey,treeArr[i].children)
          }
        }
      }
    },
    setKeysTree(){
      //设置所有key以树状形式展示
      let keysArr=Array.from(this.dataMap.keys())
      let arr2= keysArr.map(row=>row.split('/'))
      let valueArr=[]
      arr2.map(row=>{//先将二维数组转成一维数组
        let path=''
        row.map((rs,inx)=>{
          let orgPath=path
          path=path+'/'+rs
          let obj={
            id:path,
            parentId:orgPath,
            value:rs,
            label:rs
          }
          valueArr.push(obj)
        })
      })
      valueArr=this.uniqueByKey(valueArr,'id')//过滤相同的数据
      this.cascaderOptions=this.buildTree(valueArr,'')//将一维数组转成树状
    },
    uniqueByKey(array, key) {
      const seen = new Set();
      return array.filter((item) => {
          const seenKey = JSON.stringify(item[key]);
          return seen.has(seenKey) ? false : seen.add(seenKey);
      });
    },
    buildTree(data, parentId = null) {
      let tree = [];
      for (let i = 0; i < data.length; i++) {
        if (data[i].parentId === parentId) {
          let children = this.buildTree(data, data[i].id);
          if (children.length > 0) {
            data[i].children = children;
          }
          tree.push(data[i]);
        }
      }
      return tree;
    },
    onScriptEdit(index) {
      this.scriptEditorOpen = true;
    },
    submitData(data,rwName) {
      // console.log(this.editableTabs,'this.editableTabs');
      let tabs = this.editableTabs;
      if(rwName){
        this.editableTabsValue=rwName
      }else{
        if(this.editableTabs[this.editableTabsValueIndex]){
          this.editableTabsValue=this.editableTabs[this.editableTabsValueIndex].name
        }
      }
      
      tabs.map((tab) => {
        if (tab.name === this.editableTabsValue) {
          tab.resultPreCode = data?data:tab.resultPreCode;
          if(tab.name==0){
            tab.content =this.changeFilterFiled(tab.selectKeyFiled)
            if(tab.selectKeyFiled){
              this.cascaderValue=tab.selectKeyFiled.split('/')
            }else{
              this.cascaderValue=[]
            }
          }else{
            if(tab.requestType=='1'){
              let valStr=''
              for(let ix=0;ix<tab.cascaderValue.length;ix++){
                let r=tab.cascaderValue[ix]
                if(ix==0){
                  valStr=r
                }else{
                  valStr=valStr+'/'+r
                }
              }
              tab.resultPrePath=valStr
              if(this.dataMap.get(valStr)){
                let rsData=this.dataMap.get(valStr)
                if (typeof rsData === "string"){
                  tab.content=JSON.parse(this.dataMap.get(valStr))
                }else{
                  tab.content=this.dataMap.get(valStr)
                }
                
              }
            }
          }
        }
      });
      if(data){
        this.functionProcessor(data);
      }
      // console.log("后续",this.editableTabs);
      this.scriptEditorOpen = false;
    },
    // 编译器返回的函数进行处理
    async functionProcessor(data) {
      let callFunction = eval(data);
      let initResult = callFunction(JSON.parse(JSON.stringify(this.data)),GlobalApiData);
      let tabs = this.editableTabs;
      tabs.forEach((tab) => {
        if (tab.name === this.editableTabsValue) {
          
          tab.content =initResult.length === undefined ? [initResult] : JSON.parse(JSON.stringify(initResult));
          
        }
      });
    },
    // 添加删除 tabs标签
    handleTabsEdit(targetName, action) {
      if (action === "add") {
        // let newTabName = ++this.tabIndex + "";
        // this.editableTabs.push({
        //   title: "过滤器" + newTabName,
        //   name: newTabName,
        //   content: [],
        //   resultPreCode: `(result,GlobalApiData)=>{
        //     return result;
        //   }`,
        //   cascaderValue:[],
        //   requestType:'0',
        //   resultPrePath:'',
        //   selectKeyFiled:''
        // });
        // this.resultPreName = "";
        // this.editableTabsValue = newTabName;
        this.isShowJsList=!this.isShowJsList
      }
      if (action === "remove") {
        let tabs = this.editableTabs;
        let activeName = this.editableTabsValue;
        if (activeName === targetName) {
          tabs.forEach((tab, index) => {
            if (tab.name === targetName) {
              let nextTab = tabs[index + 1] || tabs[index - 1];
              if (nextTab) {
                activeName = nextTab.name;
              }
            }
          });
        }
        this.editableTabs = tabs.filter((tab) => tab.name !== targetName);
        this.editableTabsValue = this.editableTabs[0]?this.editableTabs[0].name:'0';
      }
    },
    handTabsJscriptAdd(item){
      this.isShowJsList=false
      let newTabName = ++this.tabIndex + "";
      let jsStr=''
      if(item){
        jsStr=this.jsObject[item.type]//获取对应js
      }
      this.editableTabs.push({
        title: "过滤器" + newTabName,
        name: newTabName,
        content: [],
        resultPreCode: `(result,GlobalApiData)=>{
          `+jsStr+`
          return result;
        }`,
        cascaderValue:[],
        requestType:'0',
        resultPrePath:'',
        selectKeyFiled:''
      });
      this.resultPreName = "";
      this.editableTabsValue = newTabName;
    }
  },
};
</script>
<style lang="scss" scoped>

.tabs_con{
  $absoluteWidth:var(--absoluteWidth);
  .level_class{
    position: absolute;
    z-index: 99;
    right: 40px;
    top: 0;
    width: $absoluteWidth;
    height: 44px;
    display: flex;
    justify-content: center;
    align-items: center;
    background: #1682e6;
    color: #ffffff;
    cursor: pointer;
  }
  .js_ul{
    position: absolute;
    right: 10px;
    top: 40px;
    z-index: 99;
    background: #F5F7FA;
    box-shadow: 2px 2px 4px rgba(0,0,0,0.5);
    border-radius: 5px;
    .js_li{
      height: 40px;
      line-height: 40px;
      color: black;
      cursor: pointer;
      padding: 0 10px;
      border-top: 1px solid #E4E7ED;
      text-align: center;
      font-size: 14px;
      &.first{
        border-top: 0;
      }
    }
    .js_li:hover{
      color: #1682e6;
    }
  }
  ::v-deep {
    .el-tabs__nav {
      width: 110px !important;
      .el-tabs__header{
        .el-tabs__new-tab {
          background-color: #1682e6;
          margin: 12px 18px 9px 10px;
          // margin-right: 18px;
          line-height: 16px;
        }
      }
      
    }
    .el-tabs__item {
      width: 100% !important;
    }
    .el-tabs__nav-scroll{
      overflow-x: scroll;
      width: calc(100% - var(--absoluteWidth));
      &::-webkit-scrollbar {
        width: 0;
        height: 5px;
      }

      // 滚动条里面默认的小方块,自定义样式
      &::-webkit-scrollbar-thumb {
        background: transparent;
        border-radius: 2px;
        
      }

      // 滚动条里面的轨道
      &::-webkit-scrollbar-track {
        background: transparent;
      }
      &:hover::-webkit-scrollbar-thumb {
        background: #8798af; /* 定制滑块在悬停状态下的背景颜色 */
      }
    }
    
  }
}

</style>
