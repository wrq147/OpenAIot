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
        <el-col :span="10">
          <el-form-item v-show="database.tableName !== ''" label="图例字段" prop="legend.field">
            <el-select v-model="bi.legend.field" placeholder="请选择" @change="legendFieldChanged">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="10">
          <el-form-item v-show="bi.legend.field !== ''" label="筛选" prop="legend.filter.value">
            <el-select v-model="bi.legend.filter.value" multiple collapse-tags filterable placeholder="请选择">
                  <el-option
                    v-for="item in bi.legend.filter.options"
                    :key="item.id"
                    :label="item.label"
                    :value="item.label">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        </el-row>

        <el-row :gutter="15">
        <el-col :span="10">
          <el-form-item v-show="database.tableName !== ''" label="坐标字段" prop="coordinate.field">
            <el-select v-model="bi.coordinate.field" placeholder="请选择" @change="coordinateFieldChanged">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="10">
          <el-form-item v-show="bi.coordinate.field !== ''" label="筛选" prop="coordinate.filter.value">
            <el-select v-model="bi.coordinate.filter.value" multiple collapse-tags filterable placeholder="请选择">
                  <el-option
                    v-for="item in bi.coordinate.filter.options"
                    :key="item.id"
                    :label="item.label"
                    :value="item.label">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        </el-row>

        <el-row :gutter="15">
        <el-col :span="10">
          <el-form-item v-show="database.tableName !== ''" label="分类字段" prop="category.field">
            <el-select v-model="bi.category.field" placeholder="请选择" @change="categoryFieldChanged">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="10">
          <el-form-item v-show="bi.category.field !== ''" label="筛选" prop="category.filter.value">
            <el-select v-model="bi.category.filter.value" multiple collapse-tags filterable placeholder="请选择">
                  <el-option
                    v-for="item in bi.category.filter.options"
                    :key="item.id"
                    :label="item.label"
                    :value="item.label">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        </el-row>

        <el-row :gutter="15">
        <el-col :span="10">
          <el-form-item v-show="database.tableName !== ''" label="统计字段" prop="statistics.field">
            <el-select v-model="bi.statistics.field" placeholder="请选择">
                  <el-option
                    v-for="item in fieldOptions"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="10">
          <el-form-item v-show="bi.statistics.field !== ''" label="统计类型">
            <el-select v-model="bi.statistics.type" placeholder="请选择">
                  <el-option
                    v-for="item in statisticsTypeOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
                  </el-option>
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

                <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
                  <i class="el-icon-remove-outline" />
                </div>

              </div>
              <div style="margin-left: 20px;">
                <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
                  添加列
                </el-button>
              </div>
            </div>
            </el-form-item>
          </el-col>
        </el-row>

      </el-form>
      
      <div slot="footer" class="dialog-footer" style="text-align: right;">
        <el-button type="primary" @click="submitAnalysis">提交</el-button>
      </div>
  </div>
</template>

<script>
import { getAllField, getDistinctOneField } from '@/api/report/chartDB'
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
        //分类数据分析结构
        category: {
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
        'legend.filter.value':[
              { required: true, message: "筛选不能为空", trigger: "blur" }
            ],
        'legend.field':[
              { required: true, message: "图例字段不能为空", trigger: "blur" }
            ],
        'coordinate.field':[
              { required: true, message: "坐标字段不能为空", trigger: "blur" }
            ],
        'coordinate.filter.value':[
              { required: true, message: "筛选不能为空", trigger: "blur" }
            ],
        'statistics.field':[
           { required: true, message: "统计字段不能为空", trigger: "blur" }
          ],
         'category.field':[
           { required: true, message: "分类字段不能为空", trigger: "blur" }
          ],
          'category.filter.value':[
           { required: true, message: "分类字段不能为空", trigger: "blur" }
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
    legendFieldChanged(value) {
      this.$set(this.bi.legend, 'field', value);
      this.database.fieldName = value;
      this.$set(this.bi.legend.filter, 'value', []);
      //获取字段下所有去重分类
      getDistinctOneField(this.database).then(response => {
          //console.log(response);
          if (response.code == 200) {
              
              let treeData = [];
              let object = response.data;
              for (const key in object) {
                if (Object.hasOwnProperty.call(object, key)) {
                  const element = object[key];
                  let o = new Object();
                  o.id = key;
                  o.label = element.fieldName;
                  treeData.push(o);
                }
              }
              
              this.$set(this.bi.legend.filter, 'options', treeData);
          } else {
              this.$message.error('获取数据库下所有字段失败');
          }
      });
    },
    coordinateFieldChanged(value) {
      this.$set(this.bi.coordinate, 'field', value);
      this.database.fieldName = value;
      this.$set(this.bi.coordinate.filter, 'value', []);
      //获取字段下所有去重分类
      getDistinctOneField(this.database).then(response => {
          //console.log(response);
          if (response.code == 200) {
              
              let treeData = [];
              let object = response.data;
              for (const key in object) {
                if (Object.hasOwnProperty.call(object, key)) {
                  const element = object[key];
                  let o = new Object();
                  o.id = key;
                  o.label = element.fieldName;
                  treeData.push(o);
                }
              }
              
              this.$set(this.bi.coordinate.filter, 'options', treeData);
          } else {
              this.$message.error('获取数据库下所有字段失败');
          }
      });
    },
    categoryFieldChanged(value) {
      this.$set(this.bi.category, 'field', value);
      this.database.fieldName = value;
      this.$set(this.bi.category.filter, 'value', []);
      //获取字段下所有去重分类
      getDistinctOneField(this.database).then(response => {
          //console.log(response);
          if (response.code == 200) {
              
              let treeData = [];
              let object = response.data;
              for (const key in object) {
                if (Object.hasOwnProperty.call(object, key)) {
                  const element = object[key];
                  let o = new Object();
                  o.id = key;
                  o.label = element.fieldName;
                  treeData.push(o);
                }
              }
              
              this.$set(this.bi.category.filter, 'options', treeData);
          } else {
              this.$message.error('获取数据库下所有字段失败');
          }
      });
    },
    addSelectItem(){
      this.bi.conditions.push({
        relation: '',
        field: '',
        condition: '',
        value: ''
      })
    },
    removeSelectItem(index){
      let newCols = this.bi.conditions;
      newCols.splice(index, 1);
    },
    submitAnalysis() {
      //判断是否选择目标表
      if(this.database.tableName == "" || this.database.tableName == undefined ){
        this.$message.error('请选择表');
      }else{
        //校验
      this.$refs["bi"].validate((valid) => {
        if (valid) {
          //console.log(this.bi);
          let selectPart = "select ";
          let wherePart = " where 1=1 ";
          let orderByPart = "";
          let groupByPart = " group by ";

          selectPart = selectPart + this.bi.coordinate.field + " as zbwd, " + this.bi.category.field + " as flwd, ";
        for (const item of this.bi.legend.filter.options) {
            selectPart = selectPart + this.bi.statistics.type + "(case when " + this.bi.legend.field + "='" + item.label + "' then " + this.bi.statistics.field + " end) as " + item.label + ",";
          }
          selectPart = selectPart.slice(0,-1);
          selectPart = selectPart + " from " + this.database.tableName;

          if(this.bi.coordinate.filter.value !== '') {
            // if(this.bi.coordinate.filter.type === 'standard') {
            //   let object = this.bi.coordinate.filter.value;
            //   let inPart = " and " + this.bi.coordinate.field + " in (";
            //   for (const i of object) {
            //     inPart = inPart + "'" + i + "',";
            //   }
            //   inPart = inPart.slice(0,-1);
            //   inPart = inPart + ")";
            //   wherePart = wherePart + inPart;
            // } else if(this.bi.coordinate.filter.type === 'range') {

            // }

            let object = this.bi.coordinate.filter.value;
            let inPart = " and " + this.bi.coordinate.field + " in (";
            for (const i of object) {
              inPart = inPart + "'" + i + "',";
            }
            inPart = inPart.slice(0,-1);
            inPart = inPart + ")";

            let categoryObject = this.bi.category.filter.value;
            let categoryInPart = " and " + this.bi.category.field + " in (";
            for (const i of categoryObject) {
              categoryInPart = categoryInPart + "'" + i + "',";
            }
            categoryInPart = categoryInPart.slice(0,-1);
            categoryInPart = categoryInPart + ")";

            wherePart = wherePart + inPart + categoryInPart;

          }

          // 判断筛选条件是否为空
          if (this.bi.conditions) {
            for (const obj of this.bi.conditions) {
              //校验筛选条件
              if(obj.condition == "" || obj.field == "" || obj.relation == ""){
                this.$message.error("请选择筛选条件")
                return false
              }
              //如果是空或null的条件不用拼接值字段
              else if (obj.condition == "is null" || obj.condition == "is not null" || obj.condition == '= ""' || obj.condition == '!= ""' ){
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
          
          groupByPart = groupByPart + this.bi.coordinate.field + ',' + this.bi.category.field;

          let executeSql = selectPart + wherePart + groupByPart + orderByPart;

          //console.log(executeSql);

          this.$set(this.database, 'executeSql', executeSql);

          this.database.chartStucture.legend = this.bi.legend.filter.value;
          this.database.chartStucture.coordinate = this.bi.coordinate.filter.value;
          this.database.chartStucture.category = this.bi.category.filter.value;
          this.database.chartStucture.statisticsType = this.bi.statistics.type;
          this.database.sqlType = 'four';

          this.$emit("changeDataBase", { database: JSON.parse(JSON.stringify( this.database )), bi: JSON.parse(JSON.stringify( this.bi )) });
        }})
      } 
    },
  }
}
</script>