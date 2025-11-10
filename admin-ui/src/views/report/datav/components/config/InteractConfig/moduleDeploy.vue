<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px" label-position="top" class="custom_form_item">

        <el-collapse  v-model="activeNames" accordion>
          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="文字样式" name="2" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.textStyle.content!==undefined" label="文字内容">
              <el-input v-model="configData.chartOption.textStyle.content" />
            </el-form-item>
            
            <el-form-item v-if="configData.chartOption.textStyle.fontSize!==undefined" label="字体大小">
                <el-slider v-model="configData.chartOption.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textStyle.fontFamily!==undefined" label="字体名称">
              <el-select v-model="configData.chartOption.textStyle.fontFamily" placeholder="请选择">
                <el-option
                  v-for="(item,index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textStyle.fontColor!==undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.textStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textStyle.letterSpacing!==undefined" label="字体间距">
                <el-slider v-model="configData.chartOption.textStyle.letterSpacing" :min="0" :max="200" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textStyle.lineHeight!==undefined" label="字体行高">
              <el-input-number v-model="configData.chartOption.textStyle.lineHeight" controls-position="right"  :step="1"></el-input-number>
            </el-form-item>                    

            <el-form-item v-if="configData.chartOption.textStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.textStyle.fontWeight" placeholder="请选择">
                <el-option
                  v-for="(item,index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.textStyle.textAlign!==undefined" label="对齐方式">
              <el-select v-model="configData.chartOption.textStyle.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
          
          <el-collapse-item title="背景设置" name="3" class="nopaddingbottom">
            
            <el-form-item label="背景">
              <el-radio-group v-model="configData.chartOption.background.type">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.background.type == 'color'" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.background.backgroundColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.background.type == 'img'" label="背景图片">
              <!-- <image-gallary @getImg="getSelectedBg"></image-gallary> -->
              <image-upload
                v-model="configData.chartOption.background.backgroundImg"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="动画" name="4" class="nopaddingbottom">

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
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { fontFamilys } from "../../../ComponentsConfig";
export default {
  props: ["costomData","drawingList"],
  data() {
    return {
      fontFamilys:fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      activeNames: ['1'],
      animateOptions,
      configData: this.costomData,
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.configData = newVal;
      },
    },
  },
  //页面加载完执行
  mounted() {
  },
  computed: {},
  methods: {
    getSelectedBg(val){
       this.$set(this.configData.chartOption.background, 'backgroundImg', val);
    },
  },
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