<template>
  <div>
    <el-dialog title="查询与导出历史报表" :visible.sync="exportOpen" center width="1100px" :close-on-click-modal="false" :destroy-on-close="true">
        <el-form :model="exportForm" ref="exportForm" :rules="exportRules" label-position="left" class="biaodan" label-width="80px" :inline="true">
            <el-form-item label="日期范围" prop="planeNumber" style="margin-left:10px">
                <el-date-picker v-model="exportForm.choiceTime" type="datetimerange" :unlink-panels="true"
                    range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期"
                    align="right" style="width: 356px"
                ></el-date-picker>
            </el-form-item>
            <el-form-item label="展示方式" style="margin-left:10px">
                <el-select  v-model="exportForm.WindowWay" style="width: 160px" placeholder="请选择展示方式">
                    <el-option v-for="item in windowWaylist" :key="item.value" :label="item.text" :value="item.value"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item style="margin-left:10px">
              <el-button type="primary" icon="el-icon-search" @click="loadExportData">搜索</el-button>
            </el-form-item>
        </el-form>
        <div class="device_info_con">
            <el-button type="primary" plain @click="addDeviceItems">
                <i class="el-icon-s-operation"></i>
                <span style="margin-left: 6px">添加数据</span>
            </el-button>
            <div class="device_info_li" v-for="(item,inx) in deviceItems" :key="'item'+inx">
              <div class="number_inx">
                {{inx+1}}
              </div>
              <div class="device_data">
                <div class="name_text" v-if="item.Ids[0]">{{item.devName[0]}}</div>
                <el-button type="primary" plain @click="openAddDevice(inx)" style="border:none;border-radius:5px" v-else>
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">选择设备</span>
                </el-button>
              </div>
              <div class="device_code">
                <el-select  v-model="item.Code" style="width: 160px" placeholder="请选择属性" @change="choiceDevCode($event,item.codeList,inx)">
                    <el-option v-for="it in item.codeList" :key="it.code" :label="it.name" :value="it.code"></el-option>
                </el-select>
              </div>
              <div class="device_code" style="margin-right:10px;">
                <el-select v-model="item.MergeWay" style="width: 500px" placeholder="请选择统计方式" :multiple="true">
                    <el-option v-for="it in mergeWayList" :key="'merge'+it.value" :label="it.text" :value="it.value"></el-option>
                </el-select>
              </div>
              <el-button type="danger" icon="el-icon-delete" circle @click="delectDeviceItems(inx)"></el-button>
            </div>
        </div>
        <div class="device_info_con">
            <div style="display:flex;align-items:center;">
              <el-button type="primary" plain @click="addFieldItems">
                  <i class="el-icon-s-operation"></i>
                  <span style="margin-left: 6px">添加字段</span>
              </el-button>
            </div>
            <div class="device_info_li" v-for="(item,inx) in fieldItems" :key="'item'+inx">
              <div class="device_code" style="margin-left:0">
                <el-input style="width: 160px" v-model="item.name" placeholder="请输入字段名称"></el-input>
              </div>
              <div class="device_code">
                <el-input style="width: 360px" v-model="item.formula" placeholder="请输入公式"></el-input>
              </div>
              <div class="device_code" style="align-items:flex-start;margin-left:10px">
                <i class="el-icon-info" style="margin-right: 5px"></i>公式案例：[1#1]/[2#1] 表示第一行数据的第一种统计方式的数据除以，第二行数据的第一种统计方式的数据
              </div>
              <el-button type="danger" icon="el-icon-delete" circle @click="delectFieldItems(inx)"></el-button>
            </div>
        </div>
        <div style="height: 600px" class="history_con">
        <el-table :data="dataRes" style="width: 100%; margin-top: 20px; min-height: 138.5px" height="600" :header-row-style="{ 'background-color': '#B5B5B5' }"
            v-loading="isLoadingTable" highlight-current-row>
            <el-table-column align="center" :prop="item.filed" :label="item.label" v-for="(item,inx) in tableHeard" :key="'tb'+inx+item.filed">
              <template v-if="item.children&&item.children.length>0">
                <el-table-column align="center" :prop="it.filed" :label="it.label" v-for="(it,ix) in item.children" :key="'tb'+inx+ix+it.filed"></el-table-column>
              </template>
            </el-table-column>
        </el-table>
        </div>

        <div slot="footer" class="dialog-footer">
        <el-button @click="exportOpen = false">取 消</el-button>
        <el-button
            type="primary"
            @click="handleExport"
            >导出</el-button
        >
        </div>
    </el-dialog>
    <select_devicecompt ref="select_devicecompt" @finishSelect="finishSelect"></select_devicecompt>
  </div>
</template>
  
  <script>
import { devSelectMergeList } from "@/api/after/dev";
import { exportExcleUtils2 } from "@/utils/common.js";
import select_devicecompt from './select_devicecompt'
import {
  productInfo
} from "@/api/rules/productModel";
var dayjs = require('@/utils/day.js')
export default {
  name: "AdminUiExportAnalysis",
  components:{select_devicecompt},
  props: {},
  data() {
    return {
      infiniteId: "h" + new Date(), //滚动组件的识别号，改变时刷新
      exportForm: {
        //打印表单
        activeCode: "",
        choiceTime: [],
      },
      propertiesList: [],
      oldInfoList: [],
      isLoadingTable: false,
      deviceOldQuery: {
        pageNum: 1,
        pageSize: 30,
      },
      exportRules: {
        planName: [
          { required: true, trigger: "blur", message: "请输入计划名称" },
        ],
        targetId: [
          { required: true, trigger: "change", message: "请选择设备" },
        ],
      },
      exportOpen: false,
      status: "loading",
      activeAttr: {}, //导出的字段信息
      daochu: {
        pageNum: 1,
        pageSize: 30,
      },
      deviceInfos: {}, //设备
      windowWaylist:[
        {
            text:'按日',
            value:0,
        },
        {
            text:'按月',
            value:1,
        },{
            text:'按时',
            value:2,
        }
      ],//数据展示方式
      mergeWayList:[//数据合并方式
        {
            text:'最大值',
            value:'max',
        },
        {
            text:'最小值',
            value:'min',
        },{
            text:'平均值',
            value:'mean',
        },{
            text:'合计',
            value:'sum',
        },{
            text:'期初值',
            value:'first',
        },{
            text:'期末值',
            value:'last',
        },{
            text:'区间',
            value:'interval',
        }
      ],
      deviceItems:[],
      activeIndex:null,
      fieldItems:[],//添加的自定义字段
      dataRes:[],//表格最后数据
      tableHeard:[],//表格头部数据
    };
  },

  mounted() {},

  methods: {
    async loadExportData(){
      if(this.deviceItems&&this.deviceItems.length>0){
        for(let i=0;i<this.deviceItems.length;i++){
          let row=this.deviceItems[i]
          // console.log(this.deviceItems[i],'this.deviceItems[i]');
          if(row.Ids[0]==''){
            this.$message({
              message: "请选择设备",
              type: "error",
              duration: 3 * 1000,
            });
            return;
          }
          if(row.Code==''){
            this.$message({
              message: "请选择设备属性",
              type: "error",
              duration: 3 * 1000,
            });
            return;
          }
          if(!row.MergeWay||row.MergeWay.length==0){
            this.$message({
              message: "请选择数据方式",
              type: "error",
              duration: 3 * 1000,
            });
            return;
          }
        }
        this.dataRes=[]
        this.tableHeard=[{
          filed:'',
          label:'统计区间',
          children:[{
            filed:'Time',
            label:'日期',
          }]
        }]
        if(this.exportForm.WindowWay==2){
          this.tableHeard[0].children=[{
            filed:'Time',
            label:'日期',
          },{
            filed:'Time',
            label:'时间',
          }]
        }
        await Promise.all(this.deviceItems.map(async row=>{
          if((row.MergeWay.filter(row=>row!='interval')).length==0){
            this.$message({
              message: "获取区间数据需要选择期初值和期末值",
              type: "error",
              duration: 3 * 1000,
            });
            return
          }
          let queryObj={
            Ids:row.Ids,
            MergeWay:row.MergeWay.filter(row=>row!='interval'),
            Code:row.Code,
            hasInterval:(row.MergeWay.filter(row=>row!='interval'))&&(row.MergeWay.filter(row=>row!='interval')).length>0
          }
          let rowRes=await this.requestMergeData(queryObj,row.Code,row.Ids)
          if(this.dataRes.length==0){
            this.dataRes=rowRes
          }else{
            if(rowRes&&rowRes.length==this.dataRes.length){
              this.dataRes=this.dataRes.map((rw,inx)=>{
                for(let rowKey in rowRes[inx]){
                  if(rw[rowKey]==undefined){
                    rw[rowKey]=rowRes[inx][rowKey]
                  }
                }
                return rw
              })
            }else{
              let handleres=this.setArrDateSame(this.dataRes,rowRes,this.exportForm.WindowWay,'Time')
              // console.log("处理结果",handleres);
              this.dataRes=JSON.parse(JSON.stringify(handleres.firstDataList))
              rowRes=JSON.parse(JSON.stringify(handleres.secondDataList))
              this.dataRes=this.dataRes.map((rw,inx)=>{
                for(let rowKey in rowRes[inx]){
                  if(rw[rowKey]==undefined){
                    rw[rowKey]=rowRes[inx][rowKey]
                  }
                }
                return rw
              })
            }
            
          }
          let headObj={
            filed:'',
            label:row.devName[0]+row.Codename,
            children:[]
          }
          headObj.children=row.MergeWay.map(rws=>{
            let obj={
              filed:row.Code+row.Ids+rws+'Val',
              label:this.retunMergeWay(rws),
            }
            return obj
          })
          this.tableHeard.push(headObj)
        }))
        this.setCustomFiled()
        this.setDateFiled()
      }
    },
    setArrDateSame(firstData, secondData,datatype,timefiled) {
      // console.log("用电和用气",firstDataList, secondDataList);
      //设置两个数组的头尾日期是一致的
      let datatypeval='day'
      if(datatype==0){
        datatypeval='day'
      }else if(datatype==1){
        datatypeval='month'
      }else if(datatype==2){
        datatypeval='hour'
      }
      let firstDataList=JSON.parse(JSON.stringify(firstData))
      let secondDataList=JSON.parse(JSON.stringify(secondData))
      if (firstDataList[0] && secondDataList[0]) {
        let diff = Math.abs(dayjs(firstDataList[0][timefiled]).diff(dayjs(secondDataList[0][timefiled]), datatypeval))
        if (dayjs(firstDataList[0][timefiled]).isAfter(dayjs(secondDataList[0][timefiled]))) {
          for (let i = 0; i < diff; i++) {
            let newItem = JSON.parse(JSON.stringify(secondDataList[0]))
            newItem[timefiled] = dayjs(newItem[timefiled]).add(1, datatypeval).format('YYYY-MM-DD HH:mm:ss')
            // newItem[valuefiled]=0
            for(let ke in newItem){
              if(ke.indexOf('Val')>-1){
                newItem[ke]=0
              }
            }
            secondDataList.unshift(newItem);
          }
        } else if (dayjs(firstDataList[0][timefiled]).isBefore(dayjs(secondDataList[0][timefiled]))) {
          for (let i = 0; i < diff; i++) {
            let newItem = JSON.parse(JSON.stringify(firstDataList[0]))
            newItem[timefiled] = dayjs(newItem[timefiled]).add(1, datatypeval).format('YYYY-MM-DD HH:mm:ss')
            for(let ke in newItem){
              if(ke.indexOf('Val')>-1){
                newItem[ke]=0
              }
            }
            firstDataList.unshift(newItem);
          }
        }

      }
      if (firstDataList[firstDataList.length - 1] && secondDataList[secondDataList.length - 1]) {
        let diff = Math.abs(dayjs(firstDataList[firstDataList.length - 1][timefiled]).diff(dayjs(secondDataList[secondDataList.length - 1][timefiled]), datatypeval))
        // console.log(diff,'diffdiff');
        if (dayjs(firstDataList[firstDataList.length - 1][timefiled]).isBefore(dayjs(secondDataList[secondDataList.length -
            1][timefiled]))) {
          for (let i = 0; i < diff; i++) {
            let newItem = JSON.parse(JSON.stringify(secondDataList[secondDataList.length - 1]))
            newItem[timefiled] = dayjs(newItem[timefiled]).subtract(1, datatypeval).format('YYYY-MM-DD HH:mm:ss')
            for(let ke in newItem){
              if(ke.indexOf('Val')>-1){
                newItem[ke]=0
              }
            }
            secondDataList.push(newItem);
          }
        } else if (dayjs(firstDataList[firstDataList.length - 1][timefiled]).isAfter(dayjs(secondDataList[secondDataList
            .length - 1][timefiled]))) {
          for (let i = 0; i < diff; i++) {
            let newItem = JSON.parse(JSON.stringify(firstDataList[firstDataList.length - 1]))
            newItem[timefiled] = dayjs(newItem[timefiled]).subtract(1, datatypeval).format('YYYY-MM-DD HH:mm:ss')
            for(let ke in newItem){
              if(ke.indexOf('Val')>-1){
                newItem[ke]=0
              }
            }
            firstDataList.push(newItem);
          }
        }

      }
      return {
        secondDataList: secondDataList,
        firstDataList: firstDataList
      }
    },
    setDateFiled(){
      //设置日期时间
      this.dataRes=this.dataRes.map(row=>{
        if(this.exportForm.WindowWay==2){
          row.DateVal=dayjs(row.Time).format('YYYY-MM-DD')
          row.TimeVal=dayjs(row.Time).format('HH:mm')+'-'+dayjs(row.Time).add(1, 'hour').format('HH:mm')
          this.tableHeard[0].children=[{
            filed:'DateVal',
            label:'日期',
          },{
            filed:'TimeVal',
            label:'时间',
          }]
        }else{
          row.DateVal=dayjs(row.Time).format('YYYY-MM-DD')
          this.tableHeard[0].children=[{
            filed:'DateVal',
            label:'日期',
          }]
        }
        return row
      })
    },
    setCustomFiled(){//设置自定义字段
      this.fieldItems.map((row,ix)=>{
        let inxArr=this.getArrayContents(row.formula)
        let resArr=inxArr.map(ro=>{
          if(this.deviceItems[Number(ro[0])-1]){
            let resFiled=this.deviceItems[Number(ro[0])-1].Code+this.deviceItems[Number(ro[0])-1].Ids+this.deviceItems[Number(ro[0])-1].MergeWay[Number(ro[1])-1]+'Val'
            let obj={
              resFiled:resFiled,
              resLabel:ro[0]+'#'+ro[1]
            }
            return obj
          }else{
            return ''
          }
        })
        if(resArr&&resArr.length>0){
          this.dataRes=this.dataRes.map(rw=>{
            let filedStr=((resArr.map(ro=>ro.resFiled)).join(','))+ix
            let resDataArr=resArr.map(it=>{
              let obj={
                resFiled:it.resFiled,
                resValue:rw[it.resFiled],
                resLabel:it.resLabel
              }
              return obj
            })
            let macthStr=this.replaceBracketContent(row.formula,resDataArr)
            rw[filedStr]=this.toFixedNoRounding(Number((new Function('', 'return ' + macthStr + ';'))()),3);//执行公式计算
            return rw
          })
        }
        
        let headObj={
          filed:((resArr.map(ro=>ro.resFiled)).join(','))+ix,
          label:row.name,
          children:[]
        }
        this.tableHeard.push(headObj)
      })
    },
    toFixedNoRounding(num,fixNum) {
      // 转换为字符串
      var numStr = num.toString();
      // 检查是否有小数部分
      if (numStr.indexOf('.') !== -1) {
        // 截取小数点前的所有数字和小数点后的两位
        numStr = numStr.slice(0, numStr.indexOf('.') + Number(fixNum)+1);
      }
      // 返回数字类型的结果
      return Number(numStr);
    },
    getArrayContents(str) {//获取公式里面[]中的内容
      const regex = /\[([^\]]+)\]/g; // 匹配方括号内的内容
        // 使用,作为分隔符来分割字符串并过滤空字符串
      const results = [];
      let match;
      while ((match = regex.exec(str)) !== null) {
        results.push(match[1].split('#').filter(Boolean)); // 捕获组1中的内容
      }
      return results;
    },
    replaceBracketContent(str, replacements) {
      // 使用正则表达式匹配文本中的括号内容
      const pattern = /\[([^\]]+)\]/g;
      
      // 使用replace方法和回调函数进行替换
      return str.replace(pattern, (match, p1) => {
        // 检查替换数组中是否有对应的值
        const index = replacements.findIndex(rw=>rw.resLabel==p1);
        if (index >= 0) {
          // 如果有，返回替换后的值
          return replacements[index].resValue;
        }
        // 如果没有，返回原来的值
        return match;
      });
    },
    retunMergeWay(val){
      let text=''
      switch (val) {
        // eslint-disable-line default-case
        case "max":
          text='最大值'
          break;
        case "min":
          text='最小值'
          break;
        case "mean":
          text='平均值'
          break;
        case "sum":
          text='合计'
          break;
        case "first":
          text='期初值'
        break;
        case "last":
          text='期末值'
          break;
        case "interval":
          text='区间'
          break;
      }
      return text
    },
    async requestMergeData(query,code,Ids){
      let obj={}
      if(this.exportForm.WindowWay!==null&&this.exportForm.WindowWay!==undefined){
        query.WindowWay=this.exportForm.WindowWay
      }else{
        this.$message({
          message: "请选择展示方式",
          type: "error",
          duration: 3 * 1000,
        });
        return;
      }
      if (this.exportForm.choiceTime && this.exportForm.choiceTime.length) {
        obj = this.addDateRange(
          query,
          this.exportForm.choiceTime
        );
        obj.beginTime = this.parseTime(obj.beginTime);
        obj.endTime = this.parseTime(obj.endTime);
      } else {
        this.$message({
          message: "请选择日期范围",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      let hasInterval=false
      if(obj.hasInterval){
        hasInterval=true
        delete obj.hasInterval
      }
      try {
        let res=await devSelectMergeList(obj)
        
        // let reslist=this.setMiddleData(res.data,'Time','Val',this.exportForm.WindowWay)
        let MergeWayData=this.groupBy(res.data,'MergeWay')
        let rowResData=[]
        // console.log("结果",res.data);
        if(hasInterval&&MergeWayData['first']&&MergeWayData['last']){
          MergeWayData['last']=this.setMiddleData(MergeWayData['last'],'Time','Val',this.exportForm.WindowWay)
          MergeWayData['first']=this.setMiddleData(MergeWayData['first'],'Time','Val',this.exportForm.WindowWay)
          rowResData=this.subtractArrays(MergeWayData['last'],MergeWayData['first'],'Val',code,Ids)
          delete MergeWayData['last']
          delete MergeWayData['first']
        }
        for(let keyIx in MergeWayData){
          if(rowResData.length==0){
            rowResData=MergeWayData[keyIx].map(row=>{
              row[code+Ids+keyIx+'Val']=row.Val
              return row  
            })
          }else{
            rowResData=rowResData.map((row,index)=>{
              row[code+Ids+keyIx+'Val']=MergeWayData[keyIx][index].Val
              return row
            })
          }
        }
        return rowResData
        
      } catch (error) {
        console.log("出错",error);
      }
    },
    setMiddleData(arr,filed,valuefiled,datatype){
      let datatypeval='day'
      if(datatype==0){
        datatypeval='day'
      }else if(datatype==1){
        datatypeval='month'
      }else if(datatype==2){
        datatypeval='hour'
      }
      let resList = JSON.parse(JSON.stringify(arr))
      let resarr = arr.map((row, inx) => {
        let diff = 0;
        if (arr[inx + 1]) {
          diff = dayjs(row[filed]).diff(dayjs(arr[inx + 1][filed]), datatypeval)
        }
        if (diff >= 2) {
          for(let ix=1;ix<diff;ix++){
            let element = JSON.parse(JSON.stringify(row))
            element[filed] = dayjs(row[filed]).subtract(ix, datatypeval).format('YYYY-MM-DD HH:mm:ss')
            element[valuefiled] = 0
            resList.splice(inx + ix, 0, element);
          }
        }
        return row
      })
      return resList
    },
    subtractArrays(arr1, arr2,filed,code,Ids) {//整合所有数据，将不同的MergeWay数据整合在一起
      if (arr1.length !== arr2.length) {
        throw new Error('期初和期末的数据条数需一样');//两个数组长度需要一样
      }
      return arr1.map((value, index) => {
        let valobj=JSON.parse(JSON.stringify(value))
        valobj.MergeWay='interval'
        valobj[code+Ids+'intervalVal']=this.toFixedNoRounding(Number(value[filed] - arr2[index][filed]),2)
        // if(index==arr1.length-1){
        //   valobj[code+Ids+'intervalVal']=this.toFixedNoRounding(Number(value[filed] - arr2[index][filed]),2)
        // }else{
        //   valobj[code+Ids+'intervalVal']=this.toFixedNoRounding(Number(value[filed] - arr1[index+1][filed]),2)
        // }
        
        valobj[code+Ids+value.MergeWay+'Val']=value[filed]
        valobj[code+Ids+arr2[index].MergeWay+'Val']=arr2[index][filed]
        return valobj
      });
    },
    groupBy(array, key) {//结果数组分组函数
      return array.reduce((result, currentItem) => {
        // 使用 key 函数提取分组键，如果未提供，则直接使用属性名
        const groupKey = typeof key === 'function' ? key(currentItem) : currentItem[key];
    
        // 确保 result 对象中有对应分组的数组
        if (!result[groupKey]) {
          result[groupKey] = [];
        }
    
        // 将当前项添加到对应分组的数组中
        result[groupKey].push(currentItem);

        return result;
      }, {});
    },
    delectDeviceItems(inx){//删除设备喝属性
      this.deviceItems.splice(inx,1)
    },
    delectFieldItems(inx){//删除自定义字段
      this.fieldItems.splice(inx,1)
    },
    async openAddDevice(inx){
      this.activeIndex=inx
      await this.$refs.select_devicecompt.openAddDevice()
    },
    finishSelect(row){
      let activeIndex=this.activeIndex
      this.activeIndex=null
      this.productChange(row.ProductId,(properties)=>{
        if(activeIndex!==undefined&&activeIndex!==null){
          this.deviceItems[activeIndex].codeList=properties
          this.deviceItems[activeIndex].codeList=properties
          this.deviceItems[activeIndex].Ids[0]=row.Id
          this.deviceItems[activeIndex].devName[0]=row.Name
        }
      })
    },
    choiceDevCode(val,codelist,inx){
      //选择属性后
      if(val){
        this.deviceItems[inx].Codename=(codelist.find(row=>row.code==val)).name
      }
    },
    addDeviceItems(){//添加导出的设备以及属性
      let obj={
        Ids:[''],
        MergeWay:[],
        Code:'',
        Codename:'',
        codeList:[],//属性列表
        devName:['']
      }
      this.deviceItems.push(obj)
    },
    addFieldItems(){//添加自定义的字段以及公式
      let obj={
        name:'',
        formula:''
      }
      this.fieldItems.push(obj)
    },
    productChange(productId,cb){
      //产品切换
      productInfo({ id: productId }).then(rsp => {
        let ModelTSL=JSON.parse(rsp.data.ModelTSL)
        let tags=ModelTSL.tags//标签
        let properties=ModelTSL.properties//属性
        if(cb){
          cb(properties)
        }
        
      })
    },
    handleExport() {
      //导出
      let loadingInstance = this.$loading({
        //进入页面设置加载中效果，方便完成页面保存数据的初始化
        lock: true,
        text: "正在导出，请稍等...",
        spinner: "el-icon-loading",
        background: "rgba(0, 0, 0, 1)",
      });
      let table = this.dataRes;
      let name = (this.windowWaylist.find(row=>row.value==this.exportForm.WindowWay)).text;//表格名称
      console.log(name,'namename');
      //表头
      let filterVal = [];
      let headerArr = [];
      let lenIndex=0
      for(let i=0;i<this.tableHeard.length;i++){
        let row=this.tableHeard[i]
        lenIndex=lenIndex+1
        if(row.children&&row.children.length>0){
          let childFiled=row.children.map(rw=>rw.filed)
          filterVal=[...filterVal,...childFiled]
          for(let t=0;t<row.children.length;t++){
            let rw=row.children[t]
            let labelObj={
              range:[1, lenIndex, 1, lenIndex+row.children.length-1],
              label:row.label,
              column:rw.label
            }
            headerArr.push(labelObj)
          }
          lenIndex=lenIndex+row.children.length-1
        }else{
          if(row.label){
            let labelObj={
              range:[1, lenIndex, 2, lenIndex],
              label:row.label,
              column:row.label
            }
            headerArr.push(labelObj)
          }
          if(row.filed){
            filterVal.push(row.filed)
          }
        }
        
      }
      //数据的里字段
      
      let fileName = name;
      loadingInstance.close();
      exportExcleUtils2(headerArr, filterVal, table, fileName);
    },
    
    openDialog() {
      this.propertiesList = [];
      this.deviceOldQuery = {
        pageNum: 1,
        pageSize: 30,
      };
      this.exportOpen = true;
    },
  },
};
</script>
<style lang="scss" scope>
  .device_info_con{
    margin-top: 10px;
    padding: 20px;
    border: 1px solid #A3D3FF;
    border-radius: 10px;
    .device_info_li{
      display: flex;
      align-items: center;
      margin-top: 14px;
      .number_inx{
        width:26px;
        height: 26px;
        border-radius: 50%;
        border:1px solid #333;
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 16px;
        font-weight: bold;
        margin-right: 10px;
      }
      .device_data{
        .name_text{
          padding: 10px;
          border:1px solid #333333;
        }
        .select_btn{
          width: 160px;
          height: 40px;
          display: flex;
          justify-content: center;
          align-items: center;
          background: #F6F9FF;
        }
        
      }
      .device_code{
        margin-left: 20px;
        display: flex;
        align-items: center;
      }
    }
  }
</style>