<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px" label-position="top" class="custom_form_item">
        <el-collapse v-model="activeNames" accordion>
          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称"/>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="默认样式" name="2">
            <el-form-item v-if="configData.chartOption.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.fontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.letterSpacing !== undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.letterSpacing" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontFamily !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
                <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.borderWidth !== undefined" label="边框宽度">
              <el-slider v-model="configData.chartOption.borderWidth" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.borderColor !== undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.normalBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.backgroundColor !== undefined && configData.chartOption.normalBGFlag == 'color'" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.normalBGFlag == 'img'" label="背景图片">
              <image-upload v-model="configData.chartOption.normalTabBG" :limit="1"></image-upload>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textAlign !== undefined" label="文本对齐方式">
              <el-select v-model="configData.chartOption.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.direction !== undefined" label="方向">
              <el-radio-group v-model="configData.chartOption.direction">
                <el-radio label="horizontal">横向</el-radio>
                <el-radio label="vertical">纵向</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="选中样式" name="3">
            <el-form-item v-if="configData.chartOption.selectedFontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.selectedFontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedLetterSpacing !== undefined" label="字体间距">
              <el-slider
                v-model="configData.chartOption.selectedLetterSpacing"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.selectedFontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontFamily !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.selectedFontFamily" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.selectedFontWeight" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBorderWidth !== undefined" label="边框宽度">
              <el-slider
                v-model="configData.chartOption.selectedBorderWidth"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBorderColor !== undefined" label="边框颜色">
              <el-color-picker
                v-model="configData.chartOption.selectedBorderColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.selectedBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.selectedBackgroundColor !== undefined &&
                configData.chartOption.selectedBGFlag == 'color'
              "
              label="背景颜色"
            >
              <el-color-picker
                v-model="configData.chartOption.selectedBackgroundColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBGFlag == 'img'" label="背景图片">
              <image-upload
                v-model="configData.chartOption.selectedTabBG"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="交互事件" name="4">
            <draggable :animation="340" group="selectItem" handle=".option-drag">
              <div style="display: flex">
                <el-form-item label="Tab页" style="margin-left:20px;"></el-form-item>
                <el-form-item label="组件" style="margin-left: 135px"></el-form-item>
                <!-- <el-form-item label="动画"></el-form-item> -->
              </div>

              <div v-for="(item, index) in configData.chartOption.bindingObjs" :key="index" class="select-item">
                
                <el-select clearable v-model="item.bindid" style="width: 380px" placeholder="请选择" v-if="configData.chartOption.dataSourceType=='static'||configData.chartOption.dataSourceType=='gobal'">
                  <el-option v-for="it in bindIdList" :key="it.bindid" :label="it.content" :value="it.bindid">
                  </el-option>
                </el-select>
                <el-input v-else v-model="item.bindid" placeholder="Tab页" size="small" style="width: 180px"/>
                <span>-</span>
                <el-select v-if="type == 0" v-model="item.chartids" multiple collapse-tags filterable placeholder="请选择" style="width: 400px">
                  <el-option v-for="item in chartList" :key="item.customId" :label="item.layerName" :value="item.customId">
                  </el-option>
                </el-select>

                <el-select v-if="type == 1" v-model="item.chartid" style="width: 380px" placeholder="请选择">
                  <el-option v-for="item in chartList" :key="item.customId" :label="item.layerName" :value="item.customId">
                  </el-option>
                </el-select>

                <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
                  <i class="el-icon-remove-outline" />
                </div>
              </div>
            </draggable>
            <div style="margin-left: 20px">
              <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
                添加选项
              </el-button>
            </div>
          </el-collapse-item>

          <!-- <el-collapse-item title="自动轮播" name="5">
            <el-form-item v-if="configData.chartOption.isRotation !== undefined" label="是否自动轮播">
              <el-switch v-model="configData.chartOption.isRotation" />
            </el-form-item>

            <el-form-item v-if="configData.chartOption.dur !== undefined" label="间隔时长">
              <el-input-number v-model="configData.chartOption.dur" controls-position="right" :step="1000"></el-input-number>
            </el-form-item>
          </el-collapse-item> -->

          <el-collapse-item title="动画" name="6">
            <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value">
                </el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { getLinkChart } from "../../../util/LinkageChart";
import { fontFamilys } from "../../../ComponentsConfig";
import VueEvent from '../../../VueEvent'
import draggable from 'vuedraggable'
export default {
  components: {
    draggable
  },
  props: ["costomData", "drawingList"],
  data() {
    return {
      fontFamilys: fontFamilys,
      fontWeights: ["normal", "bold", "bolder", "lighter"],
      activeNames: ["1"],
      type: 0,
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {
    let chartList = JSON.parse(JSON.stringify(this.drawingList));
    for(let index in chartList){
      if(chartList[index].chartType == "tab"){
          chartList.splice(index, 1)
      }else if(chartList[index].chartType == "group"){
          chartList.splice(index, 1)
      }
      
    }
    this.chartList = chartList;

    if(this.costomData.chartOption.bindingObjs.length > 0){
      for(const obj of this.costomData.chartOption.bindingObjs){
        //判断原有选项卡绑定单个组件设置标识为1，新建绑定栏目依然为单个组件
        //新创建的选项卡改为可绑定多个组件
        if(obj.chartid != ""){
          this.type = 1 ;
        }
      }
    }
  },
  computed: {
    bindIdList(){
      if(this.configData.chartOption.dataSourceType=='static'){
        return this.configData.chartOption.staticDataValue
      }else if(this.configData.chartOption.dataSourceType=="gobal"){
        return this.configData.chartOption.finalResult
      }
    }
  },
  methods: {
    addSelectItem(){
      this.configData.chartOption.bindingObjs.push({
        bindid: '',
        chartid: '',
        // animate: '',
        chartids: []
      })
    },
    removeSelectItem(index){
      //获取恢复展示组件id
      const tabItem = this.configData.chartOption.bindingObjs[index];
      //解除绑定判断是单个组件还是多个组件，如果多个组件循环发送请求
      if(typeof tabItem.chartids != "undefined" && tabItem.chartids != ''){
        tabItem.chartids.forEach(element => {
          //向父组件发送请求
          VueEvent.$emit('removetab',element);
        });
      }else{
        //向父组件发送请求
          VueEvent.$emit('removetab',tabItem.chartid);
      }

      //删除动态表格行内容
      this.configData.chartOption.bindingObjs.splice(index, 1);

    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.select-item {
  display: flex;
  border: 1px dashed #fff;
  box-sizing: border-box;
  align-items: center;
  & .close-btn {
    cursor: pointer;
    color: #f56c6c;
  }
  & .el-input + .el-input {
    margin-left: 4px;
  }
}
::v-deep .select-item .el-input--medium .el-input__inner {
    height: 32px;
    
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
</style>
