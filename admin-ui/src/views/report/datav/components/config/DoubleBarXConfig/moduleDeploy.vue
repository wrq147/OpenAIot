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
          <el-form-item label="标题字体大小">
            <el-slider v-model="configData.chartOption.title.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.color!==undefined" label="标题字体颜色">
            <el-color-picker v-model="configData.chartOption.title.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtextStyle.fontSize!==undefined" label="副标题字体大小">
            <el-slider v-model="configData.chartOption.title.subtextStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtextStyle.color!==undefined" label="副标题字体颜色">
            <el-color-picker v-model="configData.chartOption.title.subtextStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="标题上边距">
            <el-slider v-model="configData.chartOption.title.top" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题左边距">
            <el-slider v-model="configData.chartOption.title.left" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题对齐方式">
            <el-select v-model="configData.chartOption.title.textAlign" placeholder="请选择">
              <el-option  label="左对齐" value="left"></el-option>
              <el-option  label="中间对齐" value="center"></el-option>
              <el-option  label="右对齐" value="right"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="整体样式" name="3" v-if="configData.chartOption.bgSetting"> 
          <el-form-item v-show="configData.chartOption.bgSetting.backgroundType !== undefined" label="图形背景" label-width="100px">
            <el-radio-group v-model="configData.chartOption.bgSetting.backgroundType">
              <el-radio label="color">背景色</el-radio>
              <el-radio label="img">背景图</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bgSetting.backgroundColor !== undefined" label="背景颜色" label-width="100px">
            <el-color-picker v-model="configData.chartOption.bgSetting.backgroundColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="背景图" label-width="100px" v-if="configData.chartOption.bgSetting.backgroundImg !== undefined">
            <image-upload v-model="configData.chartOption.bgSetting.backgroundImg" :limit="1"></image-upload>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.width!==undefined " label="图形宽度%">
            <el-slider v-model="configData.chartOption.width" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="整体图形弧度">
            <el-slider v-model="configData.chartOption.bgSetting.radius" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bgSetting.top!==undefined " label="图形间距上">
            <el-slider v-model="configData.chartOption.bgSetting.top" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bgSetting.bottom!==undefined " label="图形间距下">
            <el-slider v-model="configData.chartOption.bgSetting.bottom" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bgSetting.left!==undefined " label="图形间距左">
            <el-slider v-model="configData.chartOption.bgSetting.left" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.bgSetting.right!==undefined " label="图形间距右">
            <el-slider v-model="configData.chartOption.bgSetting.right" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="4"> 
          <el-form-item v-if="configData.chartOption.barWidth!==undefined " label="柱体宽度">
            <el-slider v-model="configData.chartOption.barWidth" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.barRadius!==undefined " label="柱体弧度">
            <el-slider v-model="configData.chartOption.barRadius" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>

          <!-- <el-form-item v-if="configData.chartOption.isThemeColor===false" label="左柱颜色">
            <el-color-picker v-model="configData.chartOption.leftColor" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.isThemeColor===false" label="右柱颜色">
            <el-color-picker v-model="configData.chartOption.rightColor" show-alpha></el-color-picker>
          </el-form-item> -->
        </el-collapse-item>
        <el-collapse-item title="x轴" name="6" v-if="configData.chartOption.xAxis!==undefined">
          <el-form-item v-if="configData.chartOption.xAxisShow!==undefined" label="是否显示横坐标">
            <el-switch v-model="configData.chartOption.xAxisShow" />
          </el-form-item>
          <div v-if="configData.chartOption.xAxisShow">
            <el-form-item v-if="configData.chartOption.position!==undefined" label="坐标轴位置">
              <el-radio-group v-model="configData.chartOption.position">
                <el-radio label="top">上</el-radio>
                <el-radio label="bottom">下</el-radio>
              </el-radio-group>
            </el-form-item> 
            <el-form-item v-if="configData.chartOption.xAxis.margin!==undefined" label="坐标轴线距离">
              <el-slider v-model="configData.chartOption.xAxis.margin" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.xAxis.isShowAxisLabel!==undefined" label="显示坐标轴标签">
              <el-switch v-model="configData.chartOption.xAxis.isShowAxisLabel" />
            </el-form-item>
            <div v-if="configData.chartOption.xAxis.isShowAxisLabel">
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelInside!==undefined" label="坐标轴标签是否显示内侧">
                <el-switch v-model="configData.chartOption.xAxis.axisLabelInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelColor!==undefined" label="坐标轴标签颜色">
                <el-color-picker v-model="configData.chartOption.xAxis.axisLabelColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelFontSize!==undefined" label="坐标轴标签字体大小">
                <el-slider v-model="configData.chartOption.xAxis.axisLabelFontSize" :min="0" :max="100" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelFontWeight !== undefined" label="文字粗细">
                <el-select v-model="configData.chartOption.xAxis.axisLabelFontWeight" placeholder="请选择">
                  <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelFontStyle !== undefined" label="文字样式">
                <el-select v-model="configData.chartOption.xAxis.axisLabelFontStyle" placeholder="请选择">
                  <el-option v-for="(item, index) in fontStyles" :key="index" :label="item.label" :value="item.value">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLabelRotate!==undefined" label="标签旋转角度">
                <el-slider v-model="configData.chartOption.xAxis.axisLabelRotate" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.xAxis.isShowAxisLine!==undefined" label="显示坐标轴轴线">
              <el-switch v-model="configData.chartOption.xAxis.isShowAxisLine" />
            </el-form-item>
            <div v-if="configData.chartOption.xAxis.isShowAxisLine">
              <el-form-item v-if="configData.chartOption.xAxis.axisLineColor!==undefined" label="坐标轴轴线颜色">
                <el-color-picker v-model="configData.chartOption.xAxis.axisLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLineType!==undefined" label="坐标轴样式">
                <el-select v-model="configData.chartOption.xAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisLineWidth!==undefined" label="坐标轴轴线宽度">
                <el-slider v-model="configData.chartOption.xAxis.axisLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.xAxis.isShowAxisTick!==undefined" label="显示刻度线">
              <el-switch v-model="configData.chartOption.xAxis.isShowAxisTick" />
            </el-form-item>
            <div v-if="configData.chartOption.xAxis.isShowAxisTick">
              <el-form-item v-if="configData.chartOption.xAxis.axisLineType!==undefined" label="刻度线样式">
                <el-select v-model="configData.chartOption.xAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisTickInside!==undefined" label="刻度线是否显示内侧">
                <el-switch v-model="configData.chartOption.xAxis.axisTickInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisTickColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.xAxis.axisTickColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisTickWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.xAxis.axisTickWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.axisTickLength!==undefined" label="刻度线长度">
                <el-slider v-model="configData.chartOption.xAxis.axisTickLength" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.xAxis.isShowSplitLine!==undefined" label="显示分割线">
              <el-switch v-model="configData.chartOption.xAxis.isShowSplitLine" />
            </el-form-item>
            <div v-if="configData.chartOption.xAxis.isShowSplitLine">
              <el-form-item v-if="configData.chartOption.xAxis.splitLineType!==undefined" label="分割线样式">
                <el-select v-model="configData.chartOption.xAxis.splitLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.splitLineColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.xAxis.splitLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.xAxis.splitLineWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.xAxis.splitLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
          </div>
        </el-collapse-item>
        <el-collapse-item title="y轴" name="7" v-if="configData.chartOption.yAxis!==undefined">
          <el-form-item v-if="configData.chartOption.yAxisShow!==undefined" label="是否显示纵坐标">
            <el-switch v-model="configData.chartOption.yAxisShow" />
          </el-form-item>
          <div v-if="configData.chartOption.yAxisShow">
            <el-form-item v-if="configData.chartOption.yAxisShow&&configData.chartOption.yAxis.position!==undefined" label="坐标轴位置">
              <el-radio-group v-model="configData.chartOption.yAxis.position">
                <el-radio label="left">左</el-radio>
                <el-radio label="right">右</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisLabel!==undefined" label="显示坐标轴标签">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisLabel" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisLabel">
              <el-form-item v-if="configData.chartOption.yAxis.margin!==undefined" label="坐标轴线距离">
                <el-slider v-model="configData.chartOption.yAxis.margin" :min="-100" :max="100" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelInside!==undefined" label="坐标轴标签是否显示内侧">
                <el-switch v-model="configData.chartOption.yAxis.axisLabelInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelColor!==undefined" label="坐标轴标签颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisLabelColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontSize!==undefined" label="坐标轴标签字体大小">
                <el-slider v-model="configData.chartOption.yAxis.axisLabelFontSize" :min="0" :max="100" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontWeight !== undefined" label="文字粗细">
                <el-select v-model="configData.chartOption.yAxis.axisLabelFontWeight" placeholder="请选择">
                  <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontStyle !== undefined" label="文字样式">
                <el-select v-model="configData.chartOption.yAxis.axisLabelFontStyle" placeholder="请选择">
                  <el-option v-for="(item, index) in fontStyles" :key="index" :label="item.label" :value="item.value">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelRotate!==undefined" label="标签旋转角度">
                <el-slider v-model="configData.chartOption.yAxis.axisLabelRotate" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisLine!==undefined" label="显示坐标轴轴线">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisLine" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisLine">
              <el-form-item v-if="configData.chartOption.yAxis.axisLineColor!==undefined" label="坐标轴轴线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLineType!==undefined" label="坐标轴样式">
                <el-select v-model="configData.chartOption.yAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLineWidth!==undefined" label="坐标轴轴线宽度">
                <el-slider v-model="configData.chartOption.yAxis.axisLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisTick!==undefined" label="显示刻度线">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisTick" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisTick">
              <el-form-item v-if="configData.chartOption.yAxis.axisLineType!==undefined" label="刻度线样式">
                <el-select v-model="configData.chartOption.yAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickInside!==undefined" label="刻度线是否显示内侧">
                <el-switch v-model="configData.chartOption.yAxis.axisTickInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisTickColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.yAxis.axisTickWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickLength!==undefined" label="刻度线长度">
                <el-slider v-model="configData.chartOption.yAxis.axisTickLength" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowSplitLine!==undefined" label="显示分割线">
              <el-switch v-model="configData.chartOption.yAxis.isShowSplitLine" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowSplitLine">
              <el-form-item v-if="configData.chartOption.yAxis.splitLineType!==undefined" label="分割线样式">
                <el-select v-model="configData.chartOption.yAxis.splitLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.splitLineColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.splitLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.splitLineWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.yAxis.splitLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
          </div>
        </el-collapse-item>
        <el-collapse-item title="标签" name="8" v-if="configData.chartOption.label!==undefined">
          <el-form-item v-if="configData.chartOption.label.isShow!==undefined" label="显示标签">
            <el-switch v-model="configData.chartOption.label.isShow" />
          </el-form-item>
          <div v-if="configData.chartOption.label.isShow">
            <el-form-item v-if="configData.chartOption.label.position!==undefined" label="位置">
              <el-radio-group v-model="configData.chartOption.label.position">
                <el-radio label="">默认</el-radio>
                <el-radio label="top">上</el-radio>
                <el-radio label="bottom">下</el-radio>
                <el-radio label="left">左</el-radio>
                <el-radio label="right">右</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.distance!==undefined" label="距离">
              <el-slider v-model="configData.chartOption.label.distance" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.color!==undefined" label="颜色">
              <el-color-picker v-model="configData.chartOption.label.color" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.fontSize!==undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.label.fontSize" :min="0" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
          </div>
        </el-collapse-item>
        <el-collapse-item title="图例设置" name="9">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例宽度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemWidth"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例高度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemHeight"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例列数">
            <el-input-number :min="1" v-model="configData.chartOption.legendCols"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平位置">
            <el-input-number :min="0" v-model="configData.chartOption.legendPositionLeft"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直位置">
            <el-input-number :min="0" v-model="configData.chartOption.legendPositionTop"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平间隔">
            <el-input-number :min="0" v-model="configData.chartOption.legendItemGapX"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直间隔">
            <el-input-number :min="0" v-model="configData.chartOption.legendItemGapY"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例形状">
            <el-select v-model="configData.chartOption.icon" placeholder="请选择">
              <el-option v-for="item in legendShape" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例字号">
            <el-slider v-model="configData.chartOption.legendFontSize"  :step="1" show-input/>
          </el-form-item>
          <el-form-item label="字体颜色">
            <el-color-picker v-model="configData.chartOption.legendFontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="图型颜色" v-if="configData.chartOption.barColorlist!==undefined">
            <div v-for="(item, index) in originData" :key="index" class="select-item">
               <div style="display: flex;align-items: center;">
                  <span style="margin-right: 10px;">{{ item.name }}</span>
                  <el-color-picker v-model="configData.chartOption.barColorlist[index]" show-alpha></el-color-picker>
               </div>
            </div>
          </el-form-item>
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
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
    costomData: {
      type: Object,
      required: true
    },
    themeForm: {
      type: Object
    }
  },
  watch: {
    costomData: {
      immediate: true,
      deep: true,
      handler() {
        this.initResult()
      },
    }
  },
  data() {
    return {
      legendShape:[
        {label:'圆形',value:'circle'}
      ,{label:'矩形',value:'rect'}
      ,{label:'圆形矩形',value:'roundRect'}
      ,{label:'三角形',value:'triangle'}
      ,{label:'菱形',value:'diamond'}
      ,{label:'别针',value:'pin'}
      ,{label:'箭头',value:'arrow'}],
      activeNames: ["1"],
      animateOptions,
      fontWeights:['normal', 'bold', 'bolder', 'lighter'],
      fontStyles:[{value:'normal',label:'默认'},{value:'italic',label:'斜体'},{value:'oblique',label:'倾斜字体'},],
      chartList: this.drawingList,
      originData:[]
    }
  },
  methods: {
    // 表格列值配置
    initResult() {
      let data = this.themeForm.globalData.filter(x => x.name == this.configData.chartOption.globalData);
      let modelValue=this.configData.chartOption.modelValue
      if( this.configData.chartOption.dataSourceType !== 'static' ) {
        if (data.length > 0 && data[0].rawData !== undefined) {
          let resultData = JSON.parse(data[0].rawData);
          resultData.forEach((item, index) => {
            if (item.title === this.configData.chartOption.globalProcessor) {
              let content = item.content
              if(content[0]){
                let keysArr=Object.keys(content[0])
                if(modelValue['name']!==null&&modelValue['name']!==''&&modelValue['name']!==undefined){
                  let filterObj=this.getCategorizedValues(content, keysArr[Number(modelValue['name'])-1])
                  this.originData = Object.keys(filterObj);
                  return
                }
              }
            }
          })
        }
      } else {
        let filterObj=this.getCategorizedValues(this.configData.chartOption.staticDataValue, 'name');
        this.originData = Object.keys(filterObj);
      }
    },
    getCategorizedValues(arr, key) {
      return arr.reduce((accumulator, current) => {
        const category = current[key];
        if (!accumulator[category]) {
          accumulator[category] = [];
        }
        accumulator[category].push(current);
        return accumulator;
      }, {});
    },
    handleCheckedDatesChange(val){
      this.$set(this.configData.chartOption, 'labelFormatter', val);
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