<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <div class="field-box">
      
      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        
        <el-form v-if="currentTab==='field'" size="small" label-width="90px">
        <el-collapse  v-model="activeNames" accordion>
          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="表格列" name="2">
            <draggable
              
              :animation="340"
              group="selectItem"
              handle=".option-drag"
            >  
            <div style="display:flex">
              <el-form-item label="列名"></el-form-item>
              <el-form-item label="列值" style="margin-left: 60px;"></el-form-item>
            </div>
            
            
              <div v-for="(item, index) in cols" :key="index" class="select-item">
                
                  <el-input v-model="item.title" placeholder="列名" size="small"   @blur.prevent="changeCols()"/>
                  
                
                <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
                  <i class="el-icon-remove-outline" />
                </div>
              </div>
             </draggable>
            <div style="margin-left: 20px;">
              <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
                添加列
              </el-button>
            </div> 
            
          </el-collapse-item>

          <el-collapse-item title="表格设置" name="3">
            <el-form-item label="固定左边列数">
              <el-input-number v-model="configData.chartOption.fixedColumnsLeft" controls-position="right" :min="0" :step="1"></el-input-number>
            </el-form-item>

            <el-form-item label="固定上边列数">
              <el-input-number v-model="configData.chartOption.fixedRowsTop" controls-position="right" :min="0" :step="1"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.autoWrapRow!==undefined" label="是否自动换行">
              <el-switch v-model="configData.chartOption.autoWrapRow" />
            </el-form-item>

            <el-form-item v-if="configData.chartOption.colHeaders!==undefined" label="是否显示表头">
              <el-switch v-model="configData.chartOption.colHeaders" />
            </el-form-item>

            <el-form-item v-if="configData.chartOption.rowHeaders!==undefined" label="是否显示行号">
              <el-switch v-model="configData.chartOption.rowHeaders" />
            </el-form-item>

            <el-form-item v-if="configData.chartOption.readonly!==undefined" label="是否只读">
              <el-switch v-model="configData.chartOption.readonly" />
            </el-form-item>

          </el-collapse-item>
          

           
          <el-collapse-item title="动画" name="6">
            <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option
                  v-for="item in animateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value">
                </el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>

        </el-form>
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="90px">
          
          <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType" :customData="configData" :customId="configData.customId" 
            @changeSource="changeSource"  @changeData="changeData" :baseType="''"></data-source-config>       
            
        </el-form>
        <!-- 组件位置 -->
        <el-form v-if="currentTab === 'location'" size="small" label-width="90px">
          <el-form-item label="X位置">
            <el-input-number v-model="configData.x" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="y位置">
            <el-input-number v-model="configData.y" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="宽度">
            <el-input-number v-model="configData.width" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number v-model="configData.height" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="zIndex">
            <el-input-number v-model="configData.zindex" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>
   
  
  </div>
</template>

<script>

import {animateOptions} from '../../animate/animate'
import draggable from 'vuedraggable'

import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'
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
  mixins: [sourceConfig],
  components: {
    draggable,
    DataSourceConfig
  },
  data() {
    return {
      fontFamilys:this.fontFamilys,
      activeNames: ['1'],
      currentTab: 'field',
      animateOptions,
      cols: JSON.parse(JSON.stringify(this.costomData.chartOption)).cols,
      loading: true,
       // 表单参数
      form: {},
      // 表单校验
      rules: {
        databaseType: [
          { required: true, message: "数据库类型不能为空", trigger: "blur" }
        ],
        ipAddress: [
          { required: true, message: "ip地址不能为空", trigger: "blur" }
        ],
        port: [
          { required: true, message: "端口号不能为空", trigger: "blur" }
        ],
        databaseName: [
          { required: true, message: "数据库名称不能为空", trigger: "blur" }
        ],
        linkName: [
          { required: true, message: "链接名称不能为空", trigger: "blur" }
        ],
        username: [
          { required: true, message: "用户名不能为空", trigger: "blur" }
        ],
        password: [
          { required: true, message: "密码不能为空", trigger: "blur" }
        ]
      },
    }
  },
  //页面加载完执行
  mounted() {

  },
  computed: {
    
  },
  methods: {
    addSelectItem(){
      this.cols.push({
        title: ''
      })
    },
    removeSelectItem(index){
      let newCols = this.cols;
      newCols.splice(index, 1);
      this.$set(this.configData.chartOption, 'cols',JSON.parse(JSON.stringify( newCols)));

    },
    changeCols(){
      this.$set(this.configData.chartOption, 'cols',JSON.parse(JSON.stringify( this.cols)));
    },
    changeState(){
      this.cols = []
      this.$set(this.configData.chartOption, 'cols', []);
    }
  }
}
</script>

<style lang="scss" scoped>
.select-item {
  display: flex;
  border: 1px dashed #fff;
  box-sizing: border-box;
  & .close-btn {
    cursor: pointer;
    color: #f56c6c;
  }
  & .el-input + .el-input {
    margin-left: 4px;
  }
}
.el-input--medium .el-input__inner {
    height: 32px !important;
    width: 90px !important;
}
.el-form-item__label{
  width:30px
}
.select-item + .select-item {
  margin-top: 4px;
}
.select-item.sortable-chosen {
  border: 1px dashed #409eff;
}
.select-line-icon {
  line-height: 32px;
  font-size: 22px;
  padding: 0 4px;
  color: #777;
}

::v-deep .center-tabs .el-tabs__item {
    width: 33%;
    text-align: center;
}
.screen>:first-child{
  margin-top:37px
}
/**  滚动条凹槽的颜色，还可以设置边框属性  **/
::-webkit-scrollbar-track-piece { 

  background-color:#f8f8f8; 
  border-radius: 10px;
}
/** 滚动条的宽度  **/
::-webkit-scrollbar {

  width:9px;

  height:9px; 

}
/** 滚动条的设置  **/
::-webkit-scrollbar-thumb {

  background-color:#dddddd;

  background-clip:padding-box;

  min-height:28px; 
  border-radius: 10px;

}

::-webkit-scrollbar-thumb:hover {

  background-color:#bbb; 

}
</style>