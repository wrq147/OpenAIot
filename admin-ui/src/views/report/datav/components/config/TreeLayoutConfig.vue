<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="交互" name="interaction" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <div class="field-box">
      
      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        <el-form v-if="currentTab==='field'" size="small" label-width="90px">
          <el-collapse v-model="activeName" accordion>          
            <el-collapse-item title="图形设置" name="0">
                <el-form-item label="显示工具栏">
                    <el-switch v-model="configData.chartOption.defaultGraphOptions.allowShowMiniToolBar"  />
                </el-form-item>
                <el-form-item label="显示缩略图">
                    <el-switch v-model="configData.chartOption.defaultGraphOptions.allowShowMiniView"  />
                </el-form-item>
                <el-form-item label="显示搜索框">
                    <el-switch v-model="configData.chartOption.defaultGraphOptions.allowShowMiniNameFilter"  />
                </el-form-item>
                <el-form-item label="禁用鼠标缩放">
                    <el-switch v-model="configData.chartOption.defaultGraphOptions.disableZoom"  />
                </el-form-item>
                <el-form-item label="禁用节点拖动">
                    <el-switch v-model="configData.chartOption.defaultGraphOptions.disableDragNode"  />
                </el-form-item>
                <el-form-item label="连线方式">
                    <el-select v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultJunctionPoint" placeholder="请选择">
                        <el-option
                            v-for="item in defaultJunctionPoint"
                            :key="item.key"
                            :label="item.name"
                            :value="item.key">
                        </el-option>
                    </el-select>
                </el-form-item>
            </el-collapse-item>

            <el-collapse-item title="节点设置" name="1">
                <el-form-item label="节点形状">
                    <el-radio-group v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeShape">
                        <el-radio :label="0">圆形</el-radio>
                        <el-radio :label="1">矩形</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="节点宽度">
                    <el-slider v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeWidth" show-input/>
                </el-form-item>
                <el-form-item label="节点高度">
                    <el-slider v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeHeight" show-input/>
                </el-form-item>
                <el-form-item label="节点颜色">
                    <el-color-picker v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeColor" show-alpha />
                </el-form-item>
                <el-form-item label="边框宽度">
                    <el-slider v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeBorderWidth" show-input/>
                </el-form-item>
                <el-form-item label="边框颜色">
                    <el-color-picker v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeBorderColor" show-alpha/>
                </el-form-item>
                <el-form-item label="文本颜色">
                    <el-color-picker v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultNodeFontColor" show-alpha/>
                </el-form-item>
            </el-collapse-item>
            <el-collapse-item title="线条设置" name="2">
                <el-form-item label="线条样式">
                    <el-select v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultLineShape" placeholder="请选择">
                        <el-option
                            v-for="item in defaultLineShape"
                            :key="item.key"
                            :label="item.name"
                            :value="item.key">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="线条宽度">
                    <el-slider v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultLineWidth" show-input/>
                </el-form-item>
                <el-form-item label="线条颜色">
                    <el-color-picker v-model="configData.chartOption.defaultGraphOptions.layouts[0].defaultLineColor" show-alpha />
                </el-form-item>
            </el-collapse-item>

            <el-collapse-item title="动画设置" name="6">
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
            @changeSource="changeSource"  @changeData="changeData" :baseType="'two'"></data-source-config>         
        
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
import { getLinkChart} from "../../util/LinkageChart";
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
    //条件
    conditions: []
};

export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
  },
  data() {
    return {
      currentTab: 'field',
      animateOptions,
      loading: true,

      chartList: this.drawingList,
      fileList: [],
      addFileName: '',
      activeName:['0'],
      fontFamilys:this.fontFamilys,
      lineStyleColor:[{key:"source",name:"源节点颜色"},{key:"target",name:"目标节点颜色"},{key:"customer",name:"自定义颜色"}],
      labelPosition:['top','left','right','bottom','inside','insideLeft','insideRight','insideTop','insideBottom','insideTopLeft','insideBottomLeft','insideTopRight','insideBottomRight'],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      orient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      nodeAlign:[{key:"justify",name:"双端对齐"},{key:"left",name:"左对齐"},{key:"right",name:"右对齐"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
      defaultLineShape:[{key:1, name:"直线"},{key:4,name:"折线"},{key:2,name:'样式3'},{key:3, name:"样式4"},{key:5,name:"样式5"},{key:6,name:'样式6'}],
      defaultJunctionPoint:[{key:'border', name:"边缘"},{key:'ltrb',name:"上下左右"},{key:'tb',name:'上下'},{key:'lr',name:'左右'}],
    }
  },
  //页面加载完执行
  mounted() {

    //获取可联动组件列表
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;

  },
  computed: {
    
  },
 
  methods: {
    changeArrName(){
      this.$set(this.configData.chartOption, 'arrName', this.arrName);
    },

    isLinkChange() {
      if (this.configData.chartOption.isLink == true) {
        this.$set(this.configData.chartOption, 'isDrillDown', false);
        //this.configData.chartOption.isDrillDown == false;
      }
    },
    
  }
}
</script>


<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 25%;
    text-align: center;
}

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
::v-deep .el-upload-dragger {
    background-color: #fff;
    border: 1px dashed #d9d9d9;
    border-radius: 6px;
    box-sizing: border-box;
    width: 225px;
    height: 180px;
    text-align: center;
    cursor: pointer;
    position: relative;
    overflow: hidden;
}
</style>