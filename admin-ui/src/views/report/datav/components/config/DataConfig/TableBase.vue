<template>
  <div>
    <el-form :model="bi" ref="bi" :inline="true" label-width="68px"  :rules="dbRules">
        <el-row :gutter="15">
        <el-col :span="24">
          <el-form-item label="目标表">
            <el-select v-model="database.tableName" placeholder="请选择" @change="tableSelectChanged">
                  <el-option
                    v-for="item in tableOptions"
                    :key="item.table_name"
                    :label="item.table_name"
                    :value="item.table_name">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>
        </el-row>

        <el-row :gutter="15">
        <el-col :span="16">
          <el-form-item v-show="database.tableName !== ''" label="展示字段" prop="coordinate.filter.value">
            <el-select v-model="bi.coordinate.filter.value" multiple filterable placeholder="请选择" style="width:400px">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        </el-row>

        <el-row :gutter="15">
          <el-col :span="16">
            <el-form-item v-show="database.tableName !== ''" label="分组字段" >
              <el-select v-model="bi.group.field" multiple filterable placeholder="请选择" style="width:400px">
                    <el-option
                      v-for="item in fieldOptions"
                      :key="item.id"
                      :label="item.label"
                      :value="item.id">
                    </el-option>
                  </el-select>
            </el-form-item>
          </el-col>

        </el-row>

        <el-row :gutter="15">
        <el-col :span="14">
          <el-form-item v-show="database.tableName !== ''" label="排序字段" >
            <el-select v-model="bi.order.field" multiple filterable placeholder="请选择" style="width:400px">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <el-form-item v-show="bi.order.field.length > 0" label="排序方式" >
            <el-select v-model="bi.order.filter.type" placeholder="请选择">
              <el-option label="正序" value="asc"></el-option>
              <el-option label="倒序" value="desc"></el-option>
            </el-select>
          </el-form-item>
        </el-col>

        </el-row>

        <el-row v-show="database.tableName !== ''">
          <el-col :span="24">
            <el-form-item label="筛选条件" class="screen">
              <div style="height:200px;overflow:auto">
              <div v-show="database.tableName !== ''" style="display: flex">
                <el-form-item label="关联关系" style="margin-left: 70px;"></el-form-item>
                <el-form-item label="字段" style="margin-left: 110px;"></el-form-item>
                <el-form-item label="条件" style="margin-left: 110px;"></el-form-item>
                <el-form-item label="值" style="margin-left: 110px;"></el-form-item>
              </div>
              <div v-for="(item, index) in bi.conditions" :key="index" class="select-item">
                <el-select v-model="item.relation" placeholder="请选择" style="width: 197px">
                    <el-option label="与" value="and"></el-option>
                    <el-option label="或" value="or"></el-option>
                </el-select>
                <el-select v-model="item.field" placeholder="请选择" style="width: 197px">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
                <el-select v-model="item.condition" placeholder="请选择" style="width: 197px">
                  <el-option
                    v-for="item in conditionOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
                  </el-option>
                </el-select>
                <el-input v-model="item.value" placeholder="值" style="width: 197px"/>

                <div class="close-btn select-line-icon" @click="removeDatabaseSelectItem(index)">
                  <i class="el-icon-remove-outline" />
                </div>

              </div>
              <div style="margin-left: 20px;">
                <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addDatabaseSelectItem">
                  添加列
                </el-button>
              </div>
            </div>
            </el-form-item>
          </el-col>
        </el-row>

      </el-form>
      
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitAnalysis">提交</el-button>
        <!--<el-button @click="cancelUpload">取 消</el-button>-->
      </div>
  </div>
</template>

<script>
import { getAllField } from '@/api/report/chartDB'
import { statisticsTypeOptions, filterTypeOptions, conditionOptions } from '../../../ComponentsConfig'

const BIBAR = {
        //图例数据分析结构
        legend: {
          field: '',
          filter: {
            options: [],
            type: '',
            value: ''
          }
        },
        //坐标数据分析结构
        coordinate: {
          field: '',
          filter: {
            options: [],
            type: '',
            value: ''
          }
        },
        //统计数据分析结构
        statistics: {
          field: '',
          //统计类型：计数：count；求和：sum。默认计数
          type: 'count',
          filter: {
            options: [],
            type: '',
            value: ''
          }
        },
        //分组数据分析结构
        group: {
          field: ''
        },
        //排序数据分析结构
        order: {
          field: '',
          filter: {
            options: [],
            type: 'asc',
            value: ''
          }
        },
        //条件
        conditions: []
      };

export default {
  props:["costomData","taBleOptions","dataBase"],
  data(){
    return{
      statisticsTypeOptions,
      filterTypeOptions,
      conditionOptions,
      database: this.dataBase,
      bi: (this.costomData.chartOption.bi != undefined && this.costomData.chartOption.bi != null) ? this.costomData.chartOption.bi : JSON.parse(JSON.stringify( BIBAR )),
      //选择目标数据表集合
      tableOptions: this.taBleOptions,
      //选择目标字段集合
      fieldOptions: [],
      //数据库配置表单校验
      dbRules:{
        'coordinate.filter.value':[
              { required: true, message: "展示字段不能为空", trigger: "blur" }
            ],
      },
    }
  },
  watch: {
    'costomData.chartOption.database.baseName': {
      handler(val) {
        this.bi = JSON.parse(JSON.stringify( BIBAR ));
        this.$set(this.database, 'tableName', '');
      }
    },
  },
  mounted(){
    this.getAllField();
  },
  methods:{
    tableSelectChanged(value) {
      this.bi = JSON.parse(JSON.stringify( BIBAR ));
      this.$set(this.database, 'tableName', value);

      this.getAllField();
    },
    getAllField(){
      //获取数据库下所有字段
      getAllField(this.database).then(response => {
          if (response.code == 200) {
              //alert("连接数据库成功");
              // this.$message({
              //     message: '连接数据库成功',
              //     type: 'success'
              // });
              //console.log(response.data);
              let treeData = [];
              for (const item of response.data) {
                let o = new Object();
                o.id = item.columnName;
                o.label = item.columnName + "( " + item.comment + " )";
                treeData.push(o);
              }
              this.fieldOptions = treeData;
          } else {
              this.$message.error('获取数据库下所有字段失败');
          }
      });
    },
addDatabaseSelectItem(){
      this.bi.conditions.push({
        relation: '',
        field: '',
        condition: '',
        value: ''
      })
    },
    removeDatabaseSelectItem(index){
      let newCols = this.bi.conditions;
      newCols.splice(index, 1);
    },
    submitAnalysis() {
      //console.log(this.bi);
       //判断是否选择目标表
      if(this.database.tableName == "" || this.database.tableName == undefined ){
        this.$message.error('请选择表');
      }else{
        //校验
      this.$refs["bi"].validate((valid) => {
        if (valid) {
          let selectPart = "select ";
          let wherePart = " where 1=1 ";
          let orderByPart = " order by ";

          //selectPart = selectPart + this.bi.coordinate.field + " as zbwd, ";
          for (const item of this.bi.coordinate.filter.value) {
            selectPart += item + ",";
          }
          selectPart = selectPart.slice(0,-1);
          selectPart = selectPart + " from " + this.database.tableName;

          // 判断筛选条件是否为空
          if (this.bi.conditions) {
            for (const obj of this.bi.conditions) {
              //校验筛选条件
              if(obj.condition == "" || obj.field == "" || obj.relation == ""){
                this.$message.error("请选择筛选条件")
                return false
              }
              //如果是空或null的条件不用拼接值字段
              if (obj.condition == "is null" || obj.condition == "is not null" || obj.condition == '= ""' || obj.condition == '!= ""' ){
                  wherePart = wherePart + ' '+ obj.relation+' ' + obj.field + " " + obj.condition  
              }
              //如果是包含关系，先将可能输入的中文逗号换成英文逗号，再转成数组拼成分别用单引号包起来的字符串
              else if(obj.condition == "in" || obj.condition == "not in"){

                let valueArr = (obj.value.replace("，",",")).split(",");
                let linkResult = '';
                valueArr.forEach(element => {
                  linkResult += "'" + element + "',";
                });
                
                linkResult = linkResult.slice(0,linkResult.length-1)

                wherePart = wherePart + ' '+ obj.relation+' ' + obj.field +' ' + obj.condition + "(" + linkResult + ")"
              }
              //其他关系直接拼接
              else{
                  wherePart = wherePart + ' '+ obj.relation+' ' + obj.field+' ' + obj.condition + "'" + obj.value + "'"
              }
              
            }
          }
          let executeSql = selectPart + wherePart;

          //判断分组是否为空
          let groupByPart = " group by ";
          for (const item of this.bi.group.field) {
            groupByPart += item + ",";
          }
          groupByPart = groupByPart.slice(0,-1);
          
          if(this.bi.group.field != ''){
            executeSql = executeSql + groupByPart;
          }

          //判断排序是否为空
          for (const item of this.bi.order.field) {
            orderByPart += item + " " + this.bi.order.filter.type +",";
          }
          orderByPart = orderByPart.slice(0,-1);
          
          if(this.bi.order.field != ''){
            executeSql = executeSql + orderByPart;
          }

          //console.log(executeSql)

          this.$set(this.database, 'executeSql', executeSql);

          this.database.chartStucture.legend = this.bi.legend.filter.value;
          this.database.chartStucture.coordinate = this.bi.coordinate.filter.value;
          this.database.chartStucture.statisticsType = this.bi.statistics.type;
          this.database.sqlType = 'table';

          this.$emit("changeDataBase", { database: JSON.parse(JSON.stringify( this.database )), bi: JSON.parse(JSON.stringify( this.bi )) });
        }})}
    },
  }
}
</script>