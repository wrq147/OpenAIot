<template>
  <el-dialog title="查询与导出历史报表" :visible.sync="exportOpen" center width="900px" :close-on-click-modal="false" :destroy-on-close="true">
    <el-form :model="exportForm" ref="exportForm" :rules="exportRules" label-position="left" class="groupFrom" label-width="110px">
        <el-form-item label="日期范围" prop="planeNumber">
            <el-date-picker
                v-model="exportForm.choiceTime"
                type="datetimerange"
                :unlink-panels="true"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                align="right"
                @change="loadDeviceOldInfo"
            ></el-date-picker>
        </el-form-item>
        <el-form-item label="导出字段">
            <el-select v-model="exportForm.activeCode" style="width: 360px;" placeholder="请选择" @change="changeActiveCode">
                <el-option v-for="item in propertiesList" :key="item.code" :label="item.name" :value="item.code">
                </el-option>
            </el-select>
        </el-form-item>
        <div style="height: 500px" class="history_con">
            <el-table
          :data="oldInfoList"
          style="width: 100%; margin-top: 20px; min-height: 138.5px"
          height="500"
          :header-row-style="{ 'background-color': '#B5B5B5' }"
          v-loading="isLoadingTable"
          highlight-current-row
        >
          <el-table-column prop="Name" label="字段名称"></el-table-column>
          <!-- <el-table-column prop="Code" label="属性代码"></el-table-column> -->
          <el-table-column prop="Value" label="字段值">
            <template slot-scope="scope">
              <div v-if="activeAttr&&activeAttr.OptionType == 'geo'">
                <div>经度：{{ scope.row.Value.lat }}</div>
                <div>纬度：{{ scope.row.Value.lng }}</div>
              </div>
              <div v-else>{{ scope.row.Value }}</div>
            </template>
          </el-table-column>
          <el-table-column prop="Unit" label="单位"></el-table-column>
          <el-table-column prop="UpdatedOn" label="更新时间"></el-table-column>
          <!-- <el-table-column prop="Unit" label="单位"></el-table-column>
          <el-table-column prop="UpdatedOn" sortable label="更新时间" :sort-orders="['ascending']"></el-table-column> -->
          <template slot="append" v-if="status != 'loading'">
            <!--
                  @infinite: 滚动事件回调函数,当滚动到距离滚动父元素底部特定距离的时候，会被调用
                  distance: 这是滚动的临界值。default: 100; 如果到滚动父元素的底部距离小于这个值，那么 loadMore 回调函数就会被调用。
                  spinner: 通过这个属性，你可以选择一个你最喜爱旋转器作为加载动画
                        'default' | 'bubbles' | 'circles' | 'spiral' | 'waveDots'
                  direction: 如果你设置这个属性为top,那么这个组件将在你滚到顶部的时候，调用on-infinite函数
                        'top' | 'bottom'
                  forceUseInfiniteWrapper: (boolean | string) 强制指定滚动容器，使用CSS 选择器
                  identifier: 识别号，改变时刷新
                  -->
            <infinite-loading
              @infinite="loadMore"
              ref="historyInfiniteLoading"
              :distance="3"
              spinner="bubbles"
              :identifier="infiniteId"
              force-use-infinite-wrapper=".history_con .el-table__body-wrapper"
            >
              <!--   orce-use-infinite-wrapper 属性在存在多个 el-table 需要更详细的css选择器   -->
              <div class="no-more" slot="no-more"></div>
              <div class="no-more" slot="no-results"></div>
              <div class="no-more" slot="error">出错了</div>
            </infinite-loading>
            <div class="no-more" style="text-align: center" v-if="status == 'noMore'">
              没有更多数据
            </div>
          </template>
        </el-table>
        </div>
    </el-form>
    <div slot="footer" class="dialog-footer">
        <el-button @click="exportOpen = false">取 消</el-button>
        <el-button type="primary" @click="handleExport" v-loading="submitLoading" :disabled="submitLoading">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { DeviceOldInfo,DeviceOnLineOldInfo } from "@/api/rules/device";
import InfiniteLoading from "vue-infinite-loading";
import {exportExcleUtils} from '@/utils/common.js';

export default {
  name: 'AdminUiExportHistory',
  props: {
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  components: { InfiniteLoading },
  data() {
    return {
        infiniteId: 'h'+new Date(), //滚动组件的识别号，改变时刷新
        exportForm:{
            //打印表单
            activeCode:'',
            choiceTime:[]
        },
        propertiesList:[],
        oldInfoList:[],
        isLoadingTable:false,
        deviceOldQuery: {
            pageNum: 1,
            pageSize: 30,
        },
        exportRules:{
            planName: [{ required: true, trigger: "blur", message: "请输入计划名称" }],
            targetId:[{ required: true, trigger: "change", message: "请选择设备" }],
        },
        exportOpen:false,
        submitLoading:false,
        status:'loading',
        activeAttr:{},//导出的字段信息
        daochu:{
          pageNum: 1,
            pageSize: 30,
        }
    };
  },

  mounted() {
    
  },

  methods: {
    handleExport(){
        //导出
        let loadingInstance = this.$loading({
          //进入页面设置加载中效果，方便完成页面保存数据的初始化
          lock: true,
          text: "正在导出，请稍等...",
          spinner: "el-icon-loading",
          background: "rgba(0, 0, 0, 1)",
        });
      let table = this.oldInfoList;
		//表头
      let tHeader = [
		     '字段名称',
		     '字段值',
		     '单位',
		     '更新时间',
      ];
		//数据的里字段
      let filterVal = [
	        'Name',
	    	'Value',
	    	'Unit',
	    	'UpdatedOn',
      ];
      let name=this.propertiesList.find(row=>row.code==this.exportForm.activeCode)
      let fileName = name.name+'的历史数据';
      loadingInstance.close();
      exportExcleUtils(tHeader, filterVal, table, fileName);
        
    },
    
    openDialog(){
        this.propertiesList=[]
        this.deviceOldQuery={
            pageNum: 1,
            pageSize: 30,
        }
        this.getAttribute()
        this.exportOpen=true
    },
    loadMore($state) {
      //无线滚动加载更多
      if (this.status == "more") {
        this.deviceOldQuery.pageNum++;
        // console.log("滚动监听", this.deviceOldQuery.pageNum);
        // this.getDeviceOldInfo($state);
        this.getDeviceOldInfo($state);
      } else if (this.status == "noMore") {
        $state.complete();
      }
    },
    getAttribute() {
      // console.log(this.productInfos, "this.productInfosthis.productInfos");
      let mds = JSON.parse(this.productInfos.ModelTSL);
      let properties = JSON.parse(JSON.stringify(mds.properties));

      this.propertiesList = properties.filter(
        (item) =>
          item.option.type == "date" ||
          item.option.type == "float" ||
          item.option.type == "int" ||
          item.option.type == "geo"
      );
      this.propertiesList.push({name:'离在线记录',code:'leavingOnline',option:{type:'onLine'}})
    },
    changeActiveCode(val){
        this.activeAttr=this.propertiesList.find(row=>row.code==val)
        if(this.exportForm.choiceTime&&this.exportForm.choiceTime.length>0){
            this.deviceOldQuery.pageNum = 1
            let type=this.activeAttr.option.type
            if(type&&type=='onLine'){
              this.getLineOldInfo()
            }else{
              this.getDeviceOldInfo()
            }
            
        }
    },
    loadDeviceOldInfo(){
        if(this.exportForm.activeCode){
            this.deviceOldQuery.pageNum = 1
            let type=this.activeAttr.option.type
            if(type&&type=='onLine'){
              this.getLineOldInfo()
            }else{
              this.getDeviceOldInfo()
            }
        }
    },
    getLineOldInfo($state){
      //获取离在线历史数据
      this.$refs.exportForm.validate((valid) => {
        if(valid){
            if (this.deviceOldQuery.pageNum == 1) {
                this.status = "loading";
                this.isLoadingTable = true;
                this.oldInfoList = [];
                if (this.$refs.historyInfiniteLoading) {
                this.$refs.historyInfiniteLoading.stateChanger.reset();
                }
            }
            let obj = {};
            if (this.exportForm.choiceTime && this.exportForm.choiceTime.length) {
                obj = this.addDateRange(
                { Id: this.deviceInfos.Id},
                this.exportForm.choiceTime
                );
                obj.BeginTime = this.parseTime(obj.beginTime);
                obj.EndTime = this.parseTime(obj.endTime);
                obj.pageNum=1
                obj.pageSize=0
                delete obj.beginTime
                delete obj.endTime
            } else {
                this.$message({
                    message: "请选择日期范围",
                    type: "error",
                    duration: 5 * 1000,
                });
                return
            }
            console.log("传值参数",obj);
            DeviceOnLineOldInfo(obj).then((res) => {
              // console.log("数据",res);
                if (this.isLoadingTable) {
                this.isLoadingTable = false;
                }
                this.oldInfoList = res.data.List;
                this.status = "noMore";
                if ($state && $state != undefined) {
                    $state.complete(); // 全部加载完成
                }
            });
        }
      })
    },
    getDeviceOldInfo($state){
      //获取离在线历史数据
      this.$refs.exportForm.validate((valid) => {
        if(valid){
            if (this.deviceOldQuery.pageNum == 1) {
                this.status = "loading";
                this.isLoadingTable = true;
                this.oldInfoList = [];
                if (this.$refs.historyInfiniteLoading) {
                this.$refs.historyInfiniteLoading.stateChanger.reset();
                }
            }
            let obj = {};
            if (this.exportForm.choiceTime && this.exportForm.choiceTime.length) {
                obj = this.addDateRange(
                { Id: this.deviceInfos.Id,Code: this.exportForm.activeCode},
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
                return
            }
            DeviceOldInfo(obj).then((res) => {
              // console.log("数据",res);
                if (this.isLoadingTable) {
                this.isLoadingTable = false;
                }
                this.oldInfoList = res.data.List;
                this.status = "noMore";
                if ($state && $state != undefined) {
                    $state.complete(); // 全部加载完成
                }
            });
        }
      })
    },
  },
};
</script>