<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="表格列" name="2">
          <draggable :animation="340"  group="selectItem" handle=".option-drag">
            <div style="display: flex" class="lableText">
              <el-form-item label="列名"></el-form-item>
              <el-form-item label="列值"></el-form-item>
              <el-form-item label="宽度(px)"></el-form-item>
              <el-form-item label="颜色"></el-form-item>
            </div>
  
            <div v-for="(item, index) in cols" :key="index" class="select-item">
              <el-input v-model="item.title" placeholder="列名" size="small" @blur.prevent="changeCols()" />
              <el-select v-model="item.field" filterable allow-create placeholder="列值" @change="changeCols()">
                <el-option v-for="(value , key) in originData" :key="key" :value="key" />
              </el-select>
              <!-- <el-input v-model="item.field" placeholder="列值" size="small" @blur.prevent="changeCols()" /> -->
              <el-input v-model="item.width" placeholder="宽度" size="small" @blur.prevent="changeCols()" />
              <el-color-picker v-model="item.textColor" show-alpha @change="changeCols()"></el-color-picker>
              <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
                <i class="el-icon-remove-outline" />
              </div>
            </div>
          </draggable>
          <div style="margin-left: 20px">
            <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addSelectItem">添加列</el-button>
          </div>
        </el-collapse-item>
        <el-collapse-item title="分页设置" name="3">
          <el-form-item label="开启分页">
            <el-switch v-model="configData.chartOption.pagination" />
          </el-form-item>
          <el-form-item label="显示条数">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.pageSize" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="表头样式" name="4">
          <el-form-item v-if="configData.chartOption.isthead !== undefined" label="显示表头">
            <el-switch v-model="configData.chartOption.isthead" />
          </el-form-item>
          <el-form-item  v-if="configData.chartOption.isLeft !== undefined" label="对齐方式">
            <el-select v-model="configData.chartOption.isLeft" placeholder="请选择">
              <el-option label="居左" value="left" />
              <el-option label="居中" value="center" />
              <el-option label="居右" value="right" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.theadheight !== undefined" label="行高">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.theadheight" controls-position="right" :step="1" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.theadfontsize !== undefined" label="字体大小">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.theadfontsize" controls-position="right" :step="1" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.theadfontfamily !== undefined" label="字体名称">
            <el-select v-model="configData.chartOption.theadfontfamily" placeholder="请选择">
              <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.theadfontcolor !== undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.theadfontcolor" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.theadbackgroundcolor !== undefined" label="字体背景">
            <el-color-picker v-model="configData.chartOption.theadbackgroundcolor" show-alpha />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="表格样式" name="5">
          <el-form-item  label="纵向边框">
            <el-switch v-model="configData.chartOption.border" />
          </el-form-item>
          <el-form-item  label="横向边框">
            <el-switch v-model="configData.chartOption.tdBorder" />
          </el-form-item>
          <el-form-item  v-if="configData.chartOption.tbodyheight !== undefined"  label="行高">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.tbodyheight" controls-position="right" :step="1" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tbodyfontsize !== undefined" label="字体大小">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.tbodyfontsize" controls-position="right" :step="1" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.tbodyfontfamily !== undefined" label="字体名称">
            <el-select v-model="configData.chartOption.tbodyfontfamily" placeholder="请选择">
              <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select> 
          </el-form-item>
  
          <el-form-item label="换行">
            <el-radio-group v-model="configData.chartOption.wordWrap">
              <el-radio label="hidden">超出隐藏</el-radio>
              <el-radio label="breakAll">自动换行</el-radio>
            </el-radio-group>
          </el-form-item>
  
          <!-- <el-form-item v-if="configData.chartOption.tbodyfontcolor !== undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.tbodyfontcolor" show-alpha />
          </el-form-item> -->
  
          <el-form-item v-if="configData.chartOption.tbodybackgroundcolor!==undefined" label="背景颜色">
            <!-- <el-color-picker v-model="configData.chartOption.tbodybackgroundcolor" show-alpha /> -->
            <el-color-picker v-model="configData.chartOption.tbodybackgroundcolor" show-alpha />
          </el-form-item>
          <el-form-item label="选中样式">
            <el-color-picker v-model="configData.chartOption.selectedColor" show-alpha />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="辅助功能" name="6">
          <el-form-item label="开启合计">
            <el-switch v-model="configData.chartOption.isSum" @change="changeSumSwitch" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isSum==true" label="合计列">
            <el-select v-model="sumField" placeholder="合计列" multiple collapse-tags @change="changeSumField">
            <el-option v-for="item in cols" :key="item.field" :label="item.title" :value="item.field" />
            </el-select>
          </el-form-item>
          <el-form-item label="开启排序">
            <el-switch v-model="configData.chartOption.isSort"/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="7">
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
import draggable from "vuedraggable";
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
  components: {
    draggable
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
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      chartList: [],
      originData: [],
      sumField: this.costomData.chartOption.sumField != undefined? this.costomData.chartOption.sumField: [],//合计
      cols: JSON.parse(JSON.stringify(this.costomData.chartOption)).cols,
    }
  },
  methods: {
    // 表格列值配置
    initResult() {
      let data = this.themeForm.globalData.filter(x => x.name == this.configData.chartOption.globalData);
      if( this.configData.chartOption.dataSourceType !== 'static' ) {
        if (data.length > 0 && data[0].rawData !== undefined) {
          let resultData = JSON.parse(data[0].rawData);
          resultData.forEach((item, index) => {
            if (item.title === this.configData.chartOption.globalProcessor) {
              this.originData = item.content[0]
              return
            }
          })
        }
      } else {
        this.originData = this.configData.chartOption.staticDataValue[0]
      }
    },
    addSelectItem() {
      this.cols.push({
        field: "",
        title: "",
        width: '',
        textColor: "#ffffff",
      });
    },
    removeSelectItem(index) {
      let newCols = this.cols;
      newCols.splice(index, 1);
      this.$set(
        this.configData.chartOption,
        "cols",
        JSON.parse(JSON.stringify(newCols))
      );
    },
    changeCols() {
      this.$set(
        this.configData.chartOption,
        "cols",
        JSON.parse(JSON.stringify(this.cols))
      );
    },
    changeState() {
      this.cols = [];
      this.$set(this.configData.chartOption, "cols", []);
    },
    changeSumSwitch(value){
      if(!value){
        this.sumField = undefined;
        this.$set(this.configData.chartOption, 'sumField', this.sumField); 
      }
    },
    changeSumField(){
      this.$set(this.configData.chartOption, 'sumField', this.sumField); 
    }
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