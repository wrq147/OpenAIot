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
        <el-form v-if="currentTab === 'field'" size="small" label-width="104px" label-position="top" class="custom_form_item">
          <el-form-item
            v-if="configData.layerName !== undefined"
            label="图层名称"
          >
            <el-input
              v-model="configData.layerName"
              placeholder="请输入图层名称"
            />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.context !== undefined" label="label内容">
            <el-input
              v-model="configData.chartOption.label.context"
              placeholder="请输入label内容"
            />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.params !== undefined" label="参数">
            <!-- <el-input v-model="configData.chartOption.params" placeholder="请输入参数"/> -->
            <el-select v-model="configData.chartOption.params" filterable allow-create default-first-option placeholder="请输入参数">
              <el-option v-for="item in paramsOptions" :key="item.value" :label="item.label" :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isHideInput !== undefined" label="是否隐藏输入框">
            <el-radio-group v-model="configData.chartOption.isHideInput">
              <el-radio :label="false">显示</el-radio>
              <el-radio :label="true">隐藏</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isDisableInput !== undefined" label="是否禁用输入框">
            <el-radio-group v-model="configData.chartOption.isDisableInput">
              <el-radio :label="false">不禁用</el-radio>
              <el-radio :label="true">禁用</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.fontSize !== undefined" label="label字号">
            <el-slider
              v-model="configData.chartOption.label.fontSize"
              :min="1"
              :max="200"
              :step="1"
              show-input
            ></el-slider>
          </el-form-item>
          <el-form-item
            v-if="configData.chartOption.label.position !== undefined"
            label="label位置"
          >
            <el-select
              v-model="configData.chartOption.label.position"
              placeholder="请选择"
            >
              <el-option
                v-for="(item, index) in positionList"
                :key="index"
                :label="item.label"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item
            v-if="configData.chartOption.label.fontFamily !== undefined"
            label="label字体"
          >
            <el-select
              v-model="configData.chartOption.label.fontFamily"
              placeholder="请选择"
            >
              <el-option
                v-for="(item, index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item"
              >
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="label图标" label-width="110px">
            <image-upload
              v-model="configData.chartOption.label.icon"
              :limit="1"
            ></image-upload>
          </el-form-item>
          <el-form-item
            v-if="configData.chartOption.label.fontColor !== undefined"
            label="label字体颜色"
          >
            <el-color-picker
              v-model="configData.chartOption.label.fontColor"
              show-alpha
            ></el-color-picker>
          </el-form-item>

          <el-form-item
            v-if="configData.chartOption.input.name !== undefined"
            label="input的name"
          >
            <el-input
              v-model="configData.chartOption.input.name"
              placeholder="请输入input的name"
            />
          </el-form-item>

          <el-form-item
            v-if="configData.chartOption.input.maxlength !== undefined"
            label="input最大长度"
          >
            <el-input-number
              v-model="configData.chartOption.input.maxlength"
              controls-position="right"
              :min="1"
              :step="1"
            ></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.width" label="宽度">
            <el-input-number
              v-model="configData.chartOption.width"
              controls-position="right"
              :min="1"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.height" label="高度">
            <el-input-number
              v-model="configData.chartOption.height"
              controls-position="right"
              :min="1"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item
            v-if="configData.chartOption.input.type !== undefined"
            label="input输入类型"
          >
            <el-select
              v-model="configData.chartOption.input.type"
              placeholder="请选择"
              style="width: 100%;"
            >
              <el-option
                v-for="(item, index) in types"
                :key="index"
                :label="item"
                :value="item"
              >
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item
            v-if="configData.chartOption.animate !== undefined"
            label="载入动画"
          >
            <el-select
              v-model="configData.chartOption.animate"
              placeholder="请选择"
            >
              <el-option
                v-for="item in animateOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </el-form-item>
        </el-form>
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="76px" label-position="top" class="custom_form_item">
          <data-source-config
            :drawingList="drawingList"
            :themeForm="themeForm"
            :dataSourceType="configData.chartOption.dataSourceType"
            :customData="configData"
            :customId="configData.customId"
            @changeSource="changeSource"
            @changeData="changeData"
            :baseType="'three'"
          ></data-source-config>
        </el-form>
        <!-- 组件交互 -->
        <el-form
          v-if="currentTab === 'interaction'"
          size="small"
          label-width="76px"
          class="custom_form_item"
        >
          <div
            style="
              background-color: #f5f5f5;
              padding: 10px 15px;
              border: solid 1px #dadada;
              font-size: 14px;
              margin-bottom: 15px;
              color: #999;
            "
          >
            事件预处理
          </div>
          <chart-interact :chartOption="configData.chartOption" @changeData="changeInteractData" />
          <div
            style="
              background-color: #f5f5f5;
              padding: 10px 15px;
              border: solid 1px #dadada;
              font-size: 14px;
              margin-bottom: 15px;
              color: #999;
            "
          >
            事件发生后更新数据源
          </div>
          <el-form-item label="变更事件:">
            <el-select
              v-model="configData.chartOption.dataList"
              multiple
              placeholder="请选择"
              @change="bindDataList"
            >
              <el-option
                v-for="item in dataList"
                :key="item.name"
                :label="item.name"
                :value="item.name"
              >
              </el-option>
            </el-select>
          </el-form-item>
        </el-form>
        <!-- 组件位置 -->
        <el-form
          v-if="currentTab === 'location'"
          size="small"
          label-width="54px"
          class="custom_form_item"
        >
          <el-form-item label="X位置">
            <el-input-number
              v-model="configData.x"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="y位置">
            <el-input-number
              v-model="configData.y"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="宽度">
            <el-input-number
              v-model="configData.width"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number
              v-model="configData.height"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="zIndex">
            <el-input-number
              v-model="configData.zindex"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>
  </div>
</template>

<script>
import { animateOptions } from "../../animate/animate";
import ChartInteract from "./interact/ChartInteract";
import DataSourceConfig from "./DataConfig/DataSourceConfig";
import sourceConfig from "../mixins/sourceConfig.js";

export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    ChartInteract,
  },
  data() {
    return {
      fontFamilys: this.fontFamilys,
      types: [
        "text",
        "number",
        "button",
        "textarea",
        "date",
        "datetime-local",
        "time",
        "month",
        "week",
        "password",
      ],
      positionList:[{value:'top',label:'上'},{value:'left',label:'左'},],
      dataList: [],
      currentTab: "field",
      animateOptions,
      paramsOptions:[{
          value: 'DeviceNumber',
          label: '设备第三方编码'
        }, {
          value: 'DeviceId',
          label: '设备通讯编码'
        }, {
          value: 't',
          label: '分享时间'
        }]
    };
  },
  //页面加载完执行
  mounted() {
    //获取可联动数据源列表
    this.dataList = this.themeForm.globalData;
  },
  computed: {},
  methods: {
    bindDataList(val) {
      this.$set(this.configData.chartOption, "dataList", val);
    }
  },
};
</script>

<style scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 25%;
  text-align: center;
}
</style>
