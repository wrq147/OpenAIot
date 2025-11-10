<template>
  <div>
    <el-dialog
      title="编辑组合数据集"
      :close-on-click-modal="false"
      :visible.sync="sourceOpenDis"
      width="1000px"
      top="2vh"
      append-to-body
      @close="closesource"
      class="apidata_dialog_con"
    >
      <div>
        <el-form size="small" label-width="90px" label-position="left">
        <el-form-item label="组合数据集">
          <el-radio-group v-model="combType" @input="combTypeChange">
                <el-radio label="0">拼接</el-radio>
                <el-radio label="1">映射</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="">
        <div>
            <el-button type="text" @click="addItem">+ 添加</el-button>
        </div>
        <el-table border :data="tableColum" max-height="800" style="margin-top: 10px;"> 
            <el-table-column label="数据集" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-select style="width: calc(50% - 14px)" @change="valHasChange('globalData',scope.row,scope.$index,$event)" v-model="scope.row.globalData" placeholder="请选择">
                        <el-option v-for="item in globalDataList" :key="item.name" :label="item.name" :value="item.name">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column label="过滤器" align="center" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                    <el-select style="width: calc(50% - 14px)" @change="valHasChange('globalProcessor',scope.row,scope.$index,$event)" v-model="scope.row.globalProcessor" placeholder="请选择">
                        <el-option v-for="item in getProcess(scope.row,scope.$index)" :key="item.name" :label="item.title" :value="item.name">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column label="关联字段" align="center" :show-overflow-tooltip="true" v-if="combType=='1'">
                <template slot-scope="scope">
                    <el-select style="width: calc(50% - 14px)" @change="valHasChange('globalField',scope.row,scope.$index,$event)" v-model="scope.row.globalField" placeholder="请选择">
                        <el-option v-for="item in getFiledsList(scope.row,scope.$index)" :key="item" :label="item" :value="item">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="80px" align="center">
                <template v-slot="scope">
                    <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.$index)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        </el-form-item>
        <el-row :gutter="10">
            <el-col :span="12">
              <el-form-item label="刷新时间">
                  <el-input-number @change="confirmValue(true)" v-model="apiTimeout" :min="0" controls-position="right" :step="1000"></el-input-number>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="　">
                  <div>
                  <div style="margin-bottom: 15px">
                      <el-button @click="onRefresh" type="primary" plain>刷新结果</el-button>
                  </div>
                  </div>
              </el-form-item>
            </el-col>
        </el-row>
        <tabsData :ref="'tabsData' + curIdx" @baseDataTotal="baseDataTotal" :isShowrequest="true" :globalData="costomData.globalData"/>
        </el-form>
        </div>
        <div slot="footer" class="dialog-footer">
            <el-button @click="closesource">取 消</el-button>
            <el-button type="primary" @click="closesource">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import vueJsonEditor from "vue-json-editor";
import tabsData from "../DataBaseData/tabsData";
import DataEditor from "../../runcode/DataEditor";
export default {
  components: {
    vueJsonEditor,
    DataEditor,
    tabsData,
  },
  props: ["drawingList", "dialogVisible", "curIdx","costomData"],
  data() {
    return {
      sourceOpenDis: false,
      tableColum:[{globalData:'',globalProcessor:'',globalField:''}],
      filedSelected:[],
      defaultKeyList:[],
      selectList:[],
      options:[],
      configData:this.costomData,
      apiTimeout: 30, //刷新时间
      rawData:[],
      combType:"0",//组合方式
    };
  },
  computed: {
    globalDataList(){
        return this.costomData.globalData
    }
  },
  watch: {
    dialogVisible(newValue) {
      // console.log("弹窗样式", newValue);
      this.sourceOpenDis = newValue;
    },
  },
  mounted() {
  },
  methods: {
    getProcess(val,index){
        if(val.globalData){
            let resArr=this.costomData.globalData.find(row=>row.name==val.globalData)
            if(resArr&&resArr.rawData){
                let rawData=JSON.parse(resArr.rawData)

                return rawData
            }else{
                return [] 
            }
        }else{
            return []
        }
    },
    getFiledsList(val,index){
        if(val.globalData&&val.globalProcessor){
            if(val.resProcessData&&val.resProcessData.length>0){
                let data=val.resProcessData[0]
                return Object.keys(data)
            }else{
                return [] 
            }
        }else{
            return []
        }
    },
    confirmValue(enbleRes){
        let arr1={
        }
        if(this.combType=="1"){
          for(let i=0;i<this.tableColum.length;i++){
            if(this.tableColum[i].globalField&&this.tableColum[i].resData){
                if(!arr1.resData&&!arr1.globalField){
                  // console.log("进来了");
                    arr1=JSON.parse(JSON.stringify(this.tableColum[i]))
                }else{
                    let arr2=JSON.parse(JSON.stringify(this.tableColum[i].resData))
                    const combinedArray = arr2.map(item1 => {
                        const item2 = arr1.resData.find(item => item[arr1.globalField] === item1[this.tableColum[i].globalField]);
                        let resObj=Object.assign(item1, item2)
                        return resObj;
                    });
                    arr1.resData=combinedArray
                }
            }
            
          }
        }else if(this.combType=="0"){
          for(let i=0;i<this.tableColum.length;i++){
            if(i==0){
              arr1.resData=[]
            }
            if(this.tableColum[i]&&this.tableColum[i].resProcessData&&this.tableColum[i].resProcessData.length>0){
              arr1.resData=[...arr1.resData,...this.tableColum[i].resProcessData]
            }
            
          }
        }
        if(!arr1.resData){
          arr1.resData=[]
        }
        if(arr1.resData){
            this.$refs["tabsData" + this.curIdx].data = [...arr1.resData];
            this.$refs["tabsData" + this.curIdx].editableTabs[0].content = [...arr1.resData];
            if(enbleRes){
                this.$emit("changeconfirmValue", true);
            }
        }
        if (this.rawData && this.rawData.length > 0) {
          let rawArr = JSON.parse(this.rawData);
          // console.log("一行1111", this.rawData);
          let activName = this.$refs["tabsData" + this.curIdx].editableTabsValue;
          rawArr.map((rw) => {
            if(this.$refs["tabsData" + this.curIdx].cascaderOptions.length==0){
              this.$refs["tabsData" + this.curIdx].getTreeParams(arr1.resData,'')
              this.$refs["tabsData" + this.curIdx].setKeysTree()
            }
            if (rw.resultPreCode) {
              // this.$refs["tabsData" + this.curIdx].editableTabsValue = rw.name;
              if (rw.requestType == "0") {
                this.$refs["tabsData" + this.curIdx].submitData(
                  rw.resultPreCode,rw.name
                );
              } else {
                this.$refs["tabsData" + this.curIdx].submitData('',rw.name);
              }
            }else{
              this.$nextTick(()=>{
                // this.$refs["tabsData" + this.curIdx].editableTabsValue = rw.name;
                this.$refs["tabsData" + this.curIdx].submitData('',rw.name);
              })
            }
            // if(rw.name=='0'&&rw.selectKeyFiled){
            //   this.$refs["tabsData" + this.curIdx].changeFilterFiled(rw.selectKeyFiled)
            // }
            this.$refs["tabsData" + this.curIdx].editableTabsValue = activName;
            // console.log("一行", rw);
          });
        }
        return this.rawData
        
    },
    combTypeChange() {
      this.$emit("changeconfirmValue", true);
    },
    async getVal() {
        let obj={
            rawData:this.rawData,
            tableColum:this.tableColum,
            timeout:this.apiTimeout,
            combType:this.combType
        }
      return obj;
    },
    baseDataTotal(data) {
      // console.log("编辑器结果", data);
      this.rawData = JSON.stringify(data);
      this.$emit("changeconfirmValue", true);
    },
    onRefresh() {
      this.confirmValue(true);
      this.$message.success("刷新成功");
    },
    valHasChange(type,val,index){
    // console.log(type=='globalProcessor'||val&&val.globalData&&val.globalProcessor,'切换数据源');
      if(type=='globalProcessor'||val&&val.globalData&&val.globalProcessor){
        if(val.globalData){
          let resArr=this.costomData.globalData.find(row=>row.name==val.globalData)
          if(resArr&&resArr.rawData){
            let rawData=JSON.parse(resArr.rawData)
            let content=(rawData.find(row=>row.name==val.globalProcessor))
            if(content&&content.content&&content.content.length>0){
                this.tableColum[index].resProcessData=content.content
                this.tableColum[index].resData=content.content
            }else{
              this.tableColum[index].resProcessData=[]
              this.tableColum[index].resData=[]
            }
          }else{
            this.tableColum[index].resProcessData=[]
            this.tableColum[index].resData=[]
          }
        }else{
          this.tableColum[index].resProcessData=[]
          this.tableColum[index].resData=[]
        }
      }
      this.confirmValue(true)
    },
    initCom(option) {
      setTimeout(() => {
        this.tableColum=option.combinationTable?option.combinationTable:[]
        this.rawData = option.rawData;
        let rawData =
            option.rawData || option.rawData !== undefined
            ? JSON.parse(option.rawData)
            : [];
        this.$refs["tabsData" + this.curIdx].data =
            rawData != "" || (rawData && rawData.length > 0)
            ? [...rawData[0].content]
            : [];
        this.$refs["tabsData" + this.curIdx].editableTabs =
            rawData != "" || (rawData && rawData.length > 0)
            ? [...rawData]
            : [{ title: "默认数据", content: [], name: "0",cascaderValue:[],resultPrePath:'', resultPreCode: "" }];
        this.$refs["tabsData" + this.curIdx].editableTabsValue =
            rawData != "" || (rawData && rawData.length > 0)
            ? rawData[0].name
            : "0";
        if (option.rawData) {
            const maxValue = Math.max(...rawData.map((item) => Number(item.name)));
            this.$refs["tabsData" + this.curIdx].tabIndex = maxValue;
        } else {
            this.$refs["tabsData" + this.curIdx].tabIndex = 0;
        }
        this.combType=option.combType
        this.apiTimeout=option.timeout
        if(this.tableColum&&this.tableColum.length>0){
          for(let i=0;i<this.tableColum.length;i++){
            this.valHasChange('globalData',this.tableColum[i],i)
          }
        }else{
          this.confirmValue(false)
        }
        
      }, 100);
    },
    remove(inx){
        //删除组合的数据集
        this.tableColum.splice(inx,1)
        this.confirmValue(true)
    },
    addItem(){//添加组合的数据集
        this.tableColum.push({globalData:'',globalProcessor:'',globalField:''})
    },
    closesource() {
      //关闭接口源
      this.$emit("closesource");
    },
  },
};
</script>

<style lang="scss" scoped>
/* jsoneditor右上角默认有一个链接,加css去掉了 */
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }
  .el-input-number {
    width: 100%;
  }
  .el-input__icon {
    line-height: 28px;
  }
  .el-tabs__new-tab {
    background-color: #1682e6;
    margin-right: 18px;
    line-height: 16px;
  }
}
div.jsoneditor-menu a.jsoneditor-poweredBy {
  display: none;
}
</style>
