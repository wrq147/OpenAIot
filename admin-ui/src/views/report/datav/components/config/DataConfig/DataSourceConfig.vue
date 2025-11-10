<template>
  <div>
    <el-form action="" label-width="90px" label-position="top" class="custom_form_item">
      <el-form-item label="数据来源">
        <el-select v-model="type" placeholder="请选择" @change="changeSource">
          <el-option v-for="item in dataSourceOptions" :key="item.value" :label="item.label" :value="item.value"/>
        </el-select>
      </el-form-item>
      <slot name="referFormat">
        <el-form-item v-if="type != 'static'" label="参考格式">
          <el-input type="textarea" :rows="5" :value="refData" />
        </el-form-item>
      </slot>

      <slot name="processorSlot">
        <el-form-item v-if="type === 'gobal'" label="绑定数据">
          <el-select v-model="configData.chartOption.globalData" placeholder="请选择" @change="onChgBind">
            <el-option v-for="(item, idx) in globalArr" :key="idx" :label="item.name" :value="item.name"/>
          </el-select>
        </el-form-item>
        <el-form-item v-if=" type === 'gobal' && configData.chartOption && configData.chartOption.globalData" label="过滤器">
          <el-select v-model="configData.chartOption.globalProcessor" placeholder="请选择" @change="changeDispose">
            <el-option v-for="(item, itemKey) in globalProcessor" :key="itemKey" :label="item" :value="item"/>
          </el-select>
        </el-form-item>
      </slot>
      <slot name="staticSlot"></slot>
      <div v-if="type != 'static'">
        <slot name="dataTips">
          <div class="dataOrigin" v-if="!configData.chartOption.globalData">
            请先添加数据源
          </div>
          <div
            class="dataOrigin"
            v-if="
              configData.chartOption.globalData &&
              !configData.chartOption.globalProcessor
            "
          >
            请选择处理器
          </div>
        </slot>
        <slot>
          <div class="dataProduct">1、数据生成</div>
          <div style="margin-bottom: 20px">
            <el-input type="textarea" :rows="6" :value="rawData" />
          </div>
          <div class="dataProduct">2、数据处理</div>
          <div>
            <el-input
              class="sccls"
              ref="iptEditor"
              type="textarea"
              :rows="4"
              placeholder="点击此处编辑"
              @focus="onScriptEdit"
              :value="configData.chartOption.customData"
            />
          </div>
        </slot>
      </div>
    </el-form>
    <el-dialog
      title="编辑数据处理脚本"
      :close-on-click-modal="false"
      :visible.sync="scriptEditorOpen"
      width="750px"
      top="2vh"
    >
      <data-editor
        @submitData="submitData"
        @cancelData="scriptEditorOpen = false"
        :customData="configData.chartOption.customData"
      />
    </el-dialog>
  </div>
</template>

<script>
import DataEditor from "../runcode/DataEditor";
export default {
  props: [
    "dataSourceType",
    "customData",
    "customId",
    "baseType",
    "drawingList",
    "themeForm",
    "isOnlyStatic",
    "isConfiguration"
  ],
  components: {
    DataEditor,
  },
  data() {
    return {
      dataSourceOptions: [
        {
          value: "static",
          label: "静态数据",
        },
        {
          value: "gobal",
          label: "动态数据",
        },
      ],
      configData: this.customData,
      type: this.dataSourceType,
      activeId: this.customId,
      dataBaseType: this.baseType,
      scriptEditorOpen: false,
    };
  },
  mounted() {
    if (this.isOnlyStatic) {
      this.dataSourceOptions = [
        {
          value: "static",
          label: "静态数据",
        },
      ];
    }
  },
  computed: {
    refData() {
      return JSON.stringify(this.configData.chartOption.staticDataValue);
    },
    rawData() {
      if (this.configData.chartOption.globalData == "") {
        return "";
      }
      console.log(this.themeForm.globalData);
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.configData.chartOption.globalData
      );
      if (tdlist.length > 0) {
        return tdlist[0].rawData;
      } else {
        return "";
      }
    },

    globalArr() {
      if (this.themeForm == null) {
        return [];
      }
      if(this.isConfiguration){
        let filterGlobal=this.themeForm.globalData.filter(row=>row.interfaceURL&&row.interfaceURL=='/IoTRulesService/HttpRule/Live')
        return filterGlobal;
      }else{
        return this.themeForm.globalData;
      }
      
    },
    globalProcessor() {
      if (this.configData.chartOption.globalData == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.configData.chartOption.globalData
      );
      if (tdlist.length > 0 && tdlist[0].rawData !== undefined) {
        let optionData = [];
        JSON.parse(tdlist[0].rawData).forEach((item, index) => {
          optionData.push(item.title);
        });
        return optionData;
      } else {
        return [];
      }
    },
  },
  methods: {
    InitEditorScript() {
      if (this.configData.chartOption.customData == "") {
        return (
          `function cleanData(result){
              return ` +
          this.refData +
          `;
           }`
        );
      }
      return this.configData.chartOption.customData;
    },
    changeSource(val) {
      this.$set(this.configData.chartOption, "dataSourceType", val);
      this.$emit("changeGlobalProcessor", this.configData, true);
      // this.$emit("changeSource", val);
    },
    submitData(data) {
      this.$emit("changeData", data);
      this.scriptEditorOpen = false;
    },
    onScriptEdit() {
      this.$refs.iptEditor.blur();
      this.scriptEditorOpen = true;
    },
    onChgBind(val) {
      let rtval = this.InitEditorScript();
      this.$emit("changeData", rtval);
      this.$set(this.configData.chartOption, "globalData", val);
      this.$set(this.configData.chartOption, "globalProcessor", null);
      // this.$emit("changeGlobalProcessor", this.configData);
    },
    // 点击处理器方法
    changeDispose(val) {
      this.$set(this.configData.chartOption, "globalProcessor", val);
      this.$emit("changeGlobalProcessor", this.configData);
    },
  },
};
</script>
<style lang="scss">
.dataOrigin {
  font-size: 14px;
  color: red;
  margin-bottom: 10px;
}
</style>
<style lang="scss" scoped>
::v-deep {
  .el-select {
    width: 100%;
  }
  .el-form-item {
    margin-bottom: 10px;
  }
}
.sccls {
  textarea {
    cursor: pointer;
  }
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
.dataOrigin {
  font-size: 14px;
  color: red;
  margin-bottom: 10px;
}
</style>
