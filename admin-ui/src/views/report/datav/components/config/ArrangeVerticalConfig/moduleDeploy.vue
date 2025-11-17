<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="76px" class="custom_form_item" label-position="top">
        <el-collapse  v-model="activeNames" accordion>
          
          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称"/>
            </el-form-item>
            
          </el-collapse-item> 
          <el-collapse-item title="容器样式" name="2" class="nopaddingbottom" v-if="configData.chartOption.cotStyle">
            <el-form-item label="容器背景类型">
              <el-radio-group v-model="configData.chartOption.cotStyle.bgType">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="容器背景图片" v-if="configData.chartOption.cotStyle.bgType=='img'">
              <image-upload v-model="configData.chartOption.cotStyle.containerBgImage" :limit="1"></image-upload>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.bgType=='color'" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.cotStyle.containerColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotStyle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotStyle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.cotStyle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotStyle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotStyle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.cotStyle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.borderRadius !== undefined" label="圆角">
              <el-slider v-model="configData.chartOption.cotStyle.borderRadius" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.borderWidth !== undefined" label="边框大小">
              <el-slider v-model="configData.chartOption.cotStyle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotStyle.borderColor" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.cotStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item> 
          <el-collapse-item title="容器标题" name="3" class="nopaddingbottom" v-if="configData.chartOption.containerTitle">
            <el-form-item v-if="configData.chartOption.containerTitle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.containerTitle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.containerTitle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.containerTitle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.containerTitle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.containerTitle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.containerTitle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.containerTitle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.containerTitle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.containerTitle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.containerTitle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.containerTitle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.containerTitle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.containerTitle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.borderRadius !== undefined" label="圆角">
              <el-slider v-model="configData.chartOption.containerTitle.borderRadius" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.borderWidth !== undefined" label="下边框大小">
              <el-slider v-model="configData.chartOption.containerTitle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.borderColor !== undefined" label="下边框颜色">
              <el-color-picker v-model="configData.chartOption.containerTitle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.cotBgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.containerTitle.cotBgColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.letterSpacing !== undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.containerTitle.letterSpacing" :min="0" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.containerTitle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.containerTitle.textAlign!==undefined" label="文字对齐方式">
              <el-select v-model="configData.chartOption.containerTitle.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="标题序号样式" name="6" class="nopaddingbottom" v-if="configData.chartOption.titleBunberStyle">
            <el-form-item v-if="configData.chartOption.titleBunberStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.titleBunberStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.titleBunberStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.titleBunberStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.titleBunberStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.titleBunberStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.titleBunberStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.titleBunberStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.titleBunberStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.titleBunberStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.titleBunberStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.titleBunberStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.titleBunberStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.titleBunberStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.titleBunberStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.titleBunberStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="内容容器样式" name="4" class="nopaddingbottom" v-if="configData.chartOption.cotUlStyle">
            
            <el-form-item v-if="configData.chartOption.cotUlStyle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlStyle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlStyle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.borderRadius !== undefined" label="圆角">
              <el-slider v-model="configData.chartOption.cotUlStyle.borderRadius" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.borderWidth !== undefined" label="边框大小">
              <el-slider v-model="configData.chartOption.cotUlStyle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.borderColor !== undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.cotUlStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlStyle.cotBgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.cotUlStyle.cotBgColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="内容样式" name="5" class="nopaddingbottom" v-if="configData.chartOption.cotUlLiStyle">
            
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiStyle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiStyle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.width !== undefined" label="宽度占比">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.width" :min="0" :step="1" :max="100" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.borderWidth !== undefined" label="边框大小">
              <el-slider v-model="configData.chartOption.cotUlLiStyle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.borderColor !== undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.cotUlLiStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiStyle.cotBgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.cotUlLiStyle.cotBgColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="内容标签样式" name="6" class="nopaddingbottom" v-if="configData.chartOption.cotUlLiLabelStyle">
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlLiLabelStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlLiLabelStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlLiLabelStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlLiLabelStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlLiLabelStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlLiLabelStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlLiLabelStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlLiLabelStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiLabelStyle.textAlign!==undefined" label="文字对齐方式">
              <el-select v-model="configData.chartOption.cotUlLiLabelStyle.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="内容数值样式" name="7" class="nopaddingbottom" v-if="configData.chartOption.cotUlLiValueStyle">
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlLiValueStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlLiValueStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlLiValueStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlLiValueStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlLiValueStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlLiValueStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlLiValueStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlLiValueStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiValueStyle.textAlign!==undefined" label="文字对齐方式">
              <el-select v-model="configData.chartOption.cotUlLiValueStyle.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="内容单位样式" name="71" class="nopaddingbottom" v-if="configData.chartOption.cotUlLiUnitStyle">
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlLiUnitStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlLiUnitStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlLiUnitStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlLiUnitStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlLiUnitStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlLiUnitStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlLiUnitStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlLiUnitStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlLiUnitStyle.textAlign!==undefined" label="文字对齐方式">
              <el-select v-model="configData.chartOption.cotUlLiUnitStyle.textAlign" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="排列内容容器样式" name="8" class="nopaddingbottom" v-if="configData.chartOption.cotUlRowsStyle">
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.width !== undefined" label="宽度占比">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.width" :min="0" :step="1" :max="100" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.borderRadius !== undefined" label="圆角">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.borderRadius" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.borderWidth !== undefined" label="边框大小">
              <el-slider v-model="configData.chartOption.cotUlRowsStyle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.borderColor !== undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsStyle.cotBgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsStyle.cotBgColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="排列内容样式" name="10" class="nopaddingbottom" v-if="configData.chartOption.cotUlRowsLiStyle">
            
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.paddingLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.paddingLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.paddingTop !== undefined" label="上内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.paddingTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.paddingRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.paddingRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.paddingBottom !== undefined" label="下内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.paddingBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.marginTop !== undefined" label="上外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.marginBottom !== undefined" label="下外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.marginBottom" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.width !== undefined" label="宽度占比">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.width" :min="0" :step="1" :max="100" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.borderWidth !== undefined" label="边框大小">
              <el-slider v-model="configData.chartOption.cotUlRowsLiStyle.borderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.borderColor !== undefined" label="边框颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsLiStyle.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiStyle.cotBgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsLiStyle.cotBgColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="排列内容斜线样式" name="16" class="nopaddingbottom" v-if="configData.chartOption.cotUlRowsLiLineStyle">
            <el-form-item label="是否显示">
              <el-radio-group v-model="configData.chartOption.cotUlRowsLiLineStyle.isShow">
                <el-radio :label="false">否</el-radio>
                <el-radio :label="true">是</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlRowsLiLineStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlRowsLiLineStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsLiLineStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlRowsLiLineStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle.marginLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiLineStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiLineStyle.marginRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiLineStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="排列内容数值样式" name="12" class="nopaddingbottom" v-if="configData.chartOption.cotUlRowsLiValueStyle">
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlRowsLiValueStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlRowsLiValueStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsLiValueStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlRowsLiValueStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle.marginLeft !== undefined" label="左内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiValueStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiValueStyle.marginRight !== undefined" label="右内边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiValueStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="排列内容单位样式" name="13" class="nopaddingbottom" v-if="configData.chartOption.cotUlRowsLiUnitStyle">
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.cotUlRowsLiUnitStyle.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.cotUlRowsLiUnitStyle.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.cotUlRowsLiUnitStyle.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle.fontWeight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.cotUlRowsLiUnitStyle.fontWeight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle.marginLeft !== undefined" label="左外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiUnitStyle.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.cotUlRowsLiUnitStyle.marginRight !== undefined" label="右外边距">
              <el-slider v-model="configData.chartOption.cotUlRowsLiUnitStyle.marginRight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="动画" name="15" class="nopaddingbottom">
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

export default {
  props: ["costomData"],
  data() {
    return {
      fontFamilys: this.fontFamilys,
      fontWeights: ["normal", "bold", "bolder", "lighter"],
      animateOptions,
      configData: this.costomData,
      activeNames:1,
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