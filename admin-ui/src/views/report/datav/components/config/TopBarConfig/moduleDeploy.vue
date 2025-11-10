<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.istitle!==undefined" label="是否显示标题">
            <el-switch v-model="configData.chartOption.istitle" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="3"> 
          <el-form-item v-if="configData.chartOption.width!==undefined" label="柱体宽度">
            <el-slider v-model="configData.chartOption.width" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.order!==undefined" label="排序方式">
            <el-select v-model="configData.chartOption.order" placeholder="请选择">
              <el-option label="不排序" value="false"></el-option>
              <el-option label="降序" value="descend"></el-option>
              <el-option label="升序" value="ascend"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="圆角显示">
            <el-switch v-model="configData.chartOption.radiusShow" />
          </el-form-item> 
          <el-form-item v-if="configData.chartOption.radiusShow" label="圆角半径">
              <el-slider v-model="configData.chartOption.radius" :step="1" show-input></el-slider>
          </el-form-item>  
        </el-collapse-item>
        <el-collapse-item title="x轴" name="4">
          <el-form-item label="X轴名称">
            <el-input v-model="configData.chartOption.xAxisName" placeholder="请输入X轴名称" />
          </el-form-item>
          <el-form-item  label="x轴线">
            <el-switch v-model="configData.chartOption.xLineShow" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item  label="x轴颜色">
            <el-color-picker v-model="configData.chartOption.xLineColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="x轴粗细">
            <el-slider v-model="configData.chartOption.xLineWidth"  :step="1" show-input></el-slider>
          </el-form-item>   
          <el-form-item label="x轴标签字号">
            <el-slider v-model="configData.chartOption.xFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="x轴字体颜色">
            <el-color-picker v-model="configData.chartOption.xFontColor" show-alpha></el-color-picker>
          </el-form-item>         
        </el-collapse-item>
        <el-collapse-item title="y轴" name="5">
          <el-form-item label="y轴名称">
            <el-input v-model="configData.chartOption.yAxisName" placeholder="请输入y轴名称" />
          </el-form-item>

          <el-form-item  label="y轴线">
            <el-switch v-model="configData.chartOption.yLineShow" active-text="显示" inactive-text="不显示"/>
          </el-form-item>

          <el-form-item  label="y轴颜色">
            <el-color-picker v-model="configData.chartOption.yLineColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item label="y轴粗细">
            <el-slider v-model="configData.chartOption.yLineWidth"  :step="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>

        <el-collapse-item title="文本标签" name="6">
          <el-form-item v-if="configData.chartOption.label.position!==undefined" label="标签位置">
            <el-radio-group v-model="configData.chartOption.label.position">
              <el-radio label="top" style="margin-right: 15px">上</el-radio>
              <el-radio label="bottom" style="margin-right: 15px">下</el-radio>                
              <el-radio label="left" style="margin-right: 15px">左</el-radio>
              <el-radio label="right">右</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item label="自定义位置">
            <el-switch v-model="configData.chartOption.customPosition" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.customPosition" label="左边距">
              <el-slider v-model="configData.chartOption.leftPositon" :min="-100" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.customPosition" label="上边距">
              <el-slider v-model="configData.chartOption.topPositon" :min="-200" :step="1" show-input></el-slider>
          </el-form-item>            

          <el-form-item v-if="configData.chartOption.label.textStyle.fontSize!==undefined" label="标签字号">
                <el-slider v-model="configData.chartOption.label.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.textStyle.fontFamily!==undefined" label="标签字体名称">
            <el-select v-model="configData.chartOption.label.textStyle.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.textStyle.color!==undefined" label="标签字体颜色">
            <el-color-picker v-model="configData.chartOption.label.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        
        <el-collapse-item title="值标签" name="7">
          <el-form-item label="是否显示">
            <el-switch v-model="configData.chartOption.valueLabelShow" />
          </el-form-item>             

          <el-form-item v-if="configData.chartOption.valueLabel.textStyle.fontSize!==undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.valueLabel.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.valueLabel.textStyle.fontFamily!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.valueLabel.textStyle.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.valueLabel.textStyle.color!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.valueLabel.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.suf!==undefined" label="后缀（数据单位）">
            <el-input v-model="configData.chartOption.suf" placeholder="请输入后缀" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="分隔线" name="8">
          <el-form-item label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.splitLineShow" />
          </el-form-item>
          <el-form-item label="样式">
            <el-select v-model="configData.chartOption.splitLineStyle" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="粗细">
            <el-input-number class="inputFontSize" :min="1" :max="10" v-model="configData.chartOption.splitLineWidth"/>
          </el-form-item>
          <el-form-item label="颜色">
            <el-color-picker v-model="configData.chartOption.splitLineColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图形" name="9"> 
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>
          <draggable
            :animation="340"
            group="selectItem"
            handle=".option-drag"
          >  
            <div style="display:flex">
              <el-form-item label="颜色(始)"></el-form-item>
              <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
            </div>
            <div v-for="(item, index) in configData.chartOption.color" :key="index" class="select-item">
              <el-color-picker v-model="item.color0" show-alpha style="margin-left:30px"></el-color-picker>
              <div :style="gradientColor(item)"></div>
              <el-color-picker v-model="item.color1" show-alpha></el-color-picker>
            
              <!-- <div class="close-btn select-line-icon" @click="removeSelectItem(index)" style="margin-left:20px">
                <i class="el-icon-remove-outline" />
              </div> -->
            </div>
          </draggable>
          <!-- <div style="margin-left: 20px;">
            <el-button style="padding-bottom: 0" icon="el-icon-circle-plus-outline" type="text" @click="addSelectItem">
              添加颜色
            </el-button>
          </div>  -->
        </el-collapse-item>
        <el-collapse-item title="动画" name="10">
          <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
      </el-collapse>
    </el-form>
</template>
<script>
import { animateOptions } from "../../../animate/animate";
import draggable from 'vuedraggable'
export default {
  components: {
    draggable
  },
  props: {
    configData: {
      type: Object,
      required: true
    },
    costomData: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      fontWeights:['normal','bold','bolder','lighter'],
      chartList: this.drawingList,
      fontFamilys: []
    }
  },
  methods: {
    // addSelectItem(){
    //   console.log(this.configData.chartOption.color)
    //   this.configData.chartOption.color.push({
    //     color0: '#fff',
    //     color1: '#fff'
    //   })
    // },
    // removeSelectItem(index){
    //   this.configData.chartOption.color.splice(index, 1);
    // },
    gradientColor(item){
      let style = {width:'100px',height:'30px',marginLeft:'20px',background:`-webkit-linear-gradient(left, ${item.color0},${item.color1})`}
      return style
    },
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
    .el-input--medium .el-input__inner {
      height: 32px;
      width: 100%;
    }
    .inputFontSize{
      width: 100%;
    }
    .el-select{
      height: 32px;
      width: 100%;
    }
    .el-input--suffix, .el-input__inner{
      height: 32px;
    }
    .el-select .el-input__icon {
      line-height: 32px; //el-select 改了多高，这边多高
    }
    .lableText .el-form-item__label{
        float: none;
    }
  }
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>