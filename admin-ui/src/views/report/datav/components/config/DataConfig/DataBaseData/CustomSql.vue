<template>
  <div class="databaseRight">
    <div class="containnerTop">
      <div class="left">
        <div class="title">
          <!-- <div style="display: flex;align-items: center;">
            <span style="font-size:12px">数据源：</span>
            <el-select v-model="sourseItem" placeholder="请选择数据源" @change="sourseChange">
              <el-option v-for="item in sourseListData" :key="item.Id" :label="item.DatabaseName" :value="item.Id" />
            </el-select>
          </div> -->
          <div style="display: flex;align-items: center;">
            <span style="font-size:12px">缓存时间：</span>
            <el-input-number v-model="sqltimeout" :min="0" size="mini" style="width: 150px;" controls-position="right" @change="timeCache"></el-input-number>
            <span style="font-size:12px;margin-left:10px;">秒</span>
          </div>
          <div style="display: flex;align-items: center;">
            <span style="font-size:12px">刷新时间：</span>
            <el-input-number v-model="apiTime" :min="0" size="mini" style="width: 150px;" controls-position="right" @change="timeRefresh"></el-input-number>
            <span style="font-size:12px;margin-left:10px;">秒</span>
            <el-button type="primary" icon="el-icon-video-play" size="mini" style="margin-left: 10px" @click="refresh(true)">执行</el-button>
          </div>
        </div>
        <monacoTemplate ref="monacoTemplate" :sourseItem="sourseItem" :database="sourseListItem"/>
      </div>
      <addParameter ref="addParameter" :costomData="costomData" :drawingList="drawingList" :tableType="tableType" @quoteData="quoteData" @getVariableList="getVariableList" />
    </div>
    <tabsData ref="tabsData" @baseDataTotal="baseDataTotal" :globalData="costomData.globalData"/>
  </div>
</template>

<script>
import { listSourse, chartBIanalysis, TableNames, AllTableStruct } from '@/api/report/sourse'
import monacoTemplate from "./monacoTemplate";
import addParameter from "./addParameter";
import tabsData from "./tabsData";
var dayjs = require('@/utils/day.js')
export default {
  props:["dataBase","staticValue", 'costomData', 'sourseList', 'drawingList', 'tableType'],//, 'apiTimeout'
  components: { 
    monacoTemplate,
    addParameter,
    tabsData
  },
  data() {
    return {
      monacoEditor: null,
      database: this.dataBase,
      sourseListData: [],
      sourseItem: '',
      sourseListItem: '',
      apiTime: 30,
      variableList: [], // 变量参数数据
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 9999,
        dataType: ''
      },
      sqltimeout:300
    }
  },
  watch: {
    sourseList: {
      deep: true,
      handler() {
        this.sourseListData = JSON.parse(JSON.stringify(this.sourseList));
      },
    }
  },
  
  methods: {
    //获取数据源列表
    async getSourseList() {
      await listSourse(this.queryParams).then((response) => {
        for (const key in response.data.List) {
            response.data.List[key].children = []
        }
        this.sourseListData = response.data.List;
      });
    },
    // 编辑时重置数据方法
    initCom(tmpoption) {
      this.database = { ...tmpoption.database }
      this.sqltimeout = this.database.cacheTime ? this.database.cacheTime : 300;
      this.$set(this,'apiTime',tmpoption.timeout)
      this.sourseItem = this.database.sourseItem;
      // this.sourseChange(this.database.sourseItem);
      let rawData = tmpoption.rawData !== undefined ? JSON.parse(tmpoption.rawData) : [];
      this.$refs.addParameter.variableList = tmpoption.variableList!==undefined ? JSON.parse(tmpoption.variableList) : [];
      this.$refs.tabsData.data = rawData !='' ? [...rawData[0].content] : [];
      this.$refs.tabsData.editableTabs = rawData !='' ? [...rawData] : [ { title: '默认数据', content: [], name: '0', resultPreCode: '' } ];
      this.$refs.tabsData.editableTabsValue = this.$refs.tabsData.editableTabs[0].name;
      this.$refs.tabsData.tabIndex = this.$refs.tabsData.editableTabs[0].name;
      this.$refs.monacoTemplate.setValue(this.database.executeSql === undefined ? '' : this.database.executeSql);
      if (tmpoption.rawData) {
        const maxValue = Math.max(...rawData.map((item) => Number(item.name)));
        
        this.$refs.tabsData.tabIndex = maxValue;
      } else {
        this.$refs.tabsData.tabIndex = 0;
      }
    },
    // 选择数据源操作
    async sourseChange(id) {
      this.sourseItem = id;
      if (this.sourseListData.length === 0) {
        try {
            await this.getSourseList();
        } catch (error) {
            console.error('获取数据源列表失败:', error);
            return;
        }
      }
      for (const item of this.sourseListData) {
        if (item.Id === id) {
          try {
            // 获取子数据
            await this.fetchChildrenData(item);
          } catch (error) {
            console.error('获取子数据失败:', error);
            break;
          }
            // 设置当前数据源项的数据库名称
            this.sourseListItem = item.DatabaseName;
            // 链接数据库
            this.linkDB(item);
            break;
        }
      }
      let val = this.$refs.monacoTemplate.getValue();
      this.$refs.monacoTemplate.setValue(val ? val : 'select * from')
    },
    // 获取数据库低下的表
    async fetchChildrenData(data) {
      let array = {
        type: data.DatabaseType,
        ipAdress: data.IpAddress,
        port: data.Port,
        baseName: data.DatabaseName,
        username: data.UserName,
        password: data.Password
      }
      try {
        const res = await TableNames(array);
        for (const key in res.data) {
          res.data[key].DatabaseName = res.data[key].TABLE_NAME;
          res.data[key].children = [];
          delete res.data[key].TABLE_NAME;
        }
          data.children = res.data;
          this.fetchChildrenTableStruct(data, res.data);
        } catch (error) {
            console.error('TableNames 调用失败:', error);
        }
    },
    // 获取数据库低下的表的字段
    async fetchChildrenTableStruct(data, children) {
      let array = {
        type: data.DatabaseType,
        ipAdress: data.IpAddress,
        port: data.Port,
        baseName: data.DatabaseName,
        username: data.UserName,
        password: data.Password
      }
      try {
          const res = await AllTableStruct(array);
          const batchSize = 100; // 每批处理的数据量
          for (let i = 0; i < children.length; i += batchSize) {
              const batch = children.slice(i, i + batchSize);
              await this.processBatch(batch, res.data);
          }
          this.$nextTick(() => {
              this.formatSourseList(data);
          });
      } catch (error) {
          console.error('AllTableStruct 调用失败:', error);
      }
      // AllTableStruct(array).then(res => {
      //   for (const item of children) {
      //     for (const key in res.data) {
      //       if (item.DatabaseName == res.data[key].TABLE_NAME) {
      //         res.data[key].DatabaseName = res.data[key].COLUMN_NAME
      //         res.data[key].DatabaseType = res.data[key].DATA_TYPE
      //         res.data[key].DatabaseComment = res.data[key].COLUMN_COMMENT
      //         delete res.data[key].COLUMN_NAME
      //         item.children.push(res.data[key])
      //       }
      //     }
      //   }
      //   this.$nextTick(() => {
      //     this.formatSourseList(data);
      //   })
      // })
    },
    // 数据库低下的表的字段分批次处理
    async processBatch(batch, allStructData) {
      return new Promise((resolve) => {
          setTimeout(() => {
              for (const item of batch) {
                  for (const key in allStructData) {
                      if (item.DatabaseName === allStructData[key].TABLE_NAME) {
                          allStructData[key].DatabaseName = allStructData[key].COLUMN_NAME;
                          allStructData[key].DatabaseType = allStructData[key].DATA_TYPE;
                          allStructData[key].DatabaseComment = allStructData[key].COLUMN_COMMENT;
                          delete allStructData[key].COLUMN_NAME;
                          item.children.push(allStructData[key]);
                      }
                  }
              }
              resolve();
          }, 0);
      });
    },
    // 数据源数据格式处理
    formatSourseList(item) {
      let dataForm = {}
      let dataField = {}
      dataForm[item.DatabaseName] = []
      item.children.forEach(v => {
        dataForm[item.DatabaseName].push(v.DatabaseName)
        dataField[v.DatabaseName.toLowerCase()] = []
        v.children.forEach(h => {
          dataField[v.DatabaseName.toLowerCase()].push({"Name":h.DatabaseName,"DataType":h.DatabaseType,"Comment":h.DatabaseComment})
        })
      })
      this.$refs.monacoTemplate.databaseData = dataForm
      this.$refs.monacoTemplate.tableData = dataField
    },

    //引用变量到sql
    quoteData(item) {
      this.$refs.monacoTemplate.insertTextAtCursor(item.key)
    },

    // tabsData组件编译处理返回的数据方法
    baseDataTotal(data){
      this.database.sourseItem = this.sourseItem
      this.$emit("changeDataBase", { database: JSON.parse(JSON.stringify( this.database )), data: data, variableList: this.variableList,timeout:this.apiTime});
    },
    // addParameter组件参数变量返回的数据方法
    getVariableList(data) {
      this.variableList = data
    },

    // 连接数据库获取该数据库数据
    linkDB(item) {
      this.database.type = item.DatabaseType;
      this.database.ipAdress = item.IpAddress;
      this.database.port = item.Port;
      this.database.baseName = item.DatabaseName;
      this.database.username = item.UserName;
      this.database.password = item.Password;
      this.database.cacheTime = item.CacheTime;
    },

    // 执行sql语句方法
    refresh(isbtn){
      let val = this.$refs.monacoTemplate.getValue();
      if(val === "" || this.sourseItem === '') {
        this.$message("请选择数据源或请输入自定义sql语句");
      } else if(!this.checkSqlInj(val)) {
        this.database.executeSql = val;
        let executeSql = this.$refs.addParameter.parameterReplace(val)
        this.database.sqlType = 'custom';
        this.database.cacheTime = this.sqltimeout;

        let database=JSON.parse(JSON.stringify(this.database))
        if(isbtn){
          database.cacheTime =0
        }
        try {
          chartBIanalysis({...database, executeSql}).then(response => {
            this.$refs.tabsData.data = [...response.data];
            this.$refs.tabsData.editableTabs[0].content = [...response.data];
          });
        } catch (error) {
          console.log(error)
        }
      }
    },

    // 缓存时间改变方法
    timeCache(){
      this.$emit("changeTimeout", this.sqltimeout, this.apiTime )
    },

    // 刷新时间改变方法
    timeRefresh(){
      this.$emit("changeTimeout", this.sqltimeout, this.apiTime )
    },

    // sql字符过滤方法
    checkSqlInj(testInput) {
      let sqls = ["insert" ,"delete"];
      let invalid = false;
      let chkInput = (testInput + "").toLowerCase();
      let pos = -1;
      for (let i = 0, n = sqls .length; i < n; i++) {
        pos = chkInput.indexOf(sqls [i]);
        if (pos != -1) {
          this.$message("输入错误：含有非法字符\"" + testInput.substr(pos, sqls [i].length) + "\"!");
          invalid = true;
          break;
        }
      }
      return invalid;
    }
  }
}
</script>

<style lang="scss" scoped>
::v-deep{
  .el-select{
    width: 150px;
    height: 28px;
  }
  .el-input--suffix, .el-input__inner{
    height: 28px;
  }
  .el-input__icon {
    line-height: 28px;
  }
  .el-tabs__new-tab{
    background-color:#1682e6;
    margin-right: 18px;
    line-height: 16px;
  }
}
.databaseRight{
  width: calc(100% - 240px);
  padding: 15px;
  border-radius: 5px;
  background-color: #fff;
  box-shadow: 0 0 2px rgba(0, 0, 0, .1);
}
.containnerTop{
  width: 100%;
  display: flex;
}
.containnerTop .left{
  width: 50%;
}
.title{
  height: 50px;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 10px 10px 10px;
}
</style>