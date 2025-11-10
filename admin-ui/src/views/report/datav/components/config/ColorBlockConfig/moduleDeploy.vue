<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="100px" label-position="top" class="custom_form_item">
        <el-collapse  v-model="activeNames" accordion>
          
          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item> 

          <!-- <el-collapse-item title="编辑颜色块" name="1">
            <div style="display:flex">
              <el-form-item label="背景颜色"></el-form-item>
              <el-form-item label="后缀内容"></el-form-item>
              
            </div>
            <div v-for="(item,index) in configData.chartOption.colorBlock" :key="index" class="select-item">
              <el-color-picker v-model="item.color" show-alpha></el-color-picker>
              <el-input v-model="item.suffix" placeholder="后缀内容" size="small"/>
            </div>

          
          </el-collapse-item>-->

          <el-collapse-item title="文本设置" name="2" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.text.left!==undefined" label="文本对齐方式">
              <el-select v-model="configData.chartOption.text.left" placeholder="请选择">
                <el-option label="居左" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="flex-end"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.text.size!==undefined" label="文本字号">
              <el-slider v-model="configData.chartOption.text.size" :min="1" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.text.color!==undefined" label="文本字色">
              <el-color-picker v-model="configData.chartOption.text.color" show-alpha></el-color-picker>
            </el-form-item>           

            <el-form-item v-if="configData.chartOption.text.weight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.text.weight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.text.family!==undefined" label="文本字体">
              <el-select v-model="configData.chartOption.text.family" placeholder="请选择">
                <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.text.width!==undefined" label="文本部分宽度(%)">
              <el-slider v-model="configData.chartOption.text.width" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textLocate!==undefined" label="文本相对值的位置">
              <el-radio-group v-model="configData.chartOption.textLocate">
                <el-radio label="top">上</el-radio>
                <el-radio label="bottom">下</el-radio>
                <el-radio label="left">左</el-radio>
                <el-radio label="right">右</el-radio>
              </el-radio-group>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="值设置" name="3" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.value.left!==undefined" label="值对齐方式">
              <el-select v-model="configData.chartOption.value.left" placeholder="请选择">
                <el-option label="居左" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="flex-end"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.isThousand!==undefined" label="千位分隔符">
              <el-select v-model="configData.chartOption.isThousand" placeholder="请选择">
                <el-option label="无分隔符，例：1000" value="false"></el-option>
                <el-option label="空格，例：1 000" value=" "></el-option>
                <el-option label="逗号，例：1,000" value=","></el-option>
                <el-option label="点号，例：1.000" value="."></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.value.size!==undefined" label="值字号">
              <el-slider v-model="configData.chartOption.value.size" :min="1" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.value.color!==undefined" label="值字色">
              <el-color-picker v-model="configData.chartOption.value.color" show-alpha></el-color-picker>
            </el-form-item>           

            <el-form-item v-if="configData.chartOption.value.weight!==undefined" label="值粗细">
              <el-select v-model="configData.chartOption.value.weight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

           <el-form-item v-if="configData.chartOption.value.family!==undefined" label="值字体">
              <el-select v-model="configData.chartOption.value.family" placeholder="请选择">
                <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
             
            <el-form-item  label="值字体间距">
              <el-slider v-model="configData.chartOption.value.letterSpacing" :min="1" :step="1" show-input></el-slider>
            </el-form-item>
            

          </el-collapse-item>

          <el-collapse-item title="后缀设置" name="4" class="nopaddingbottom">

            

            <el-form-item v-if="configData.chartOption.suffix.size!==undefined" label="后缀字号">
              <el-slider v-model="configData.chartOption.suffix.size" :min="1" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.suffix.color!==undefined" label="后缀字色">
              <el-color-picker v-model="configData.chartOption.suffix.color" show-alpha></el-color-picker>
            </el-form-item>           

            <el-form-item v-if="configData.chartOption.suffix.weight!==undefined" label="后缀粗细">
              <el-select v-model="configData.chartOption.suffix.weight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

           <el-form-item v-if="configData.chartOption.suffix.family!==undefined" label="后缀字体">
              <el-select v-model="configData.chartOption.suffix.family" placeholder="请选择">
                <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.suffix.width!==undefined" label="后缀部分宽度(%)">
              <el-slider v-model="configData.chartOption.suffix.width" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="颜色块总体属性设置" name="5" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.numOfCol!==undefined" label="颜色块列数">
              <el-slider v-model="configData.chartOption.numOfCol" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginLeft!==undefined" label="横向间距(%)">
              <el-slider v-model="configData.chartOption.marginLeft" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginTop!==undefined" label="纵向间距(%)">
              <el-slider v-model="configData.chartOption.marginTop" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

           <!-- <el-form-item v-if="configData.chartOption.bradius!==undefined" label="圆角角度">
              <el-slider v-model="configData.chartOption.bradius" :min="0" :step="1" show-input></el-slider>
            </el-form-item>--> 

          </el-collapse-item>

          <el-collapse-item title="动画" name="6" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
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
import { fontFamilys } from "../../../ComponentsConfig";
export default {
  props: ["costomData"],
  data() {
    return {
      fontFamilys:fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      activeNames: ['1'],
      animateOptions,
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {},
  computed: {},
  methods: {},
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>