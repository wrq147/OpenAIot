<template>
  <el-form size="small" label-width="90px">
    <el-collapse v-model="activeNames" accordion>
      <el-collapse-item title="图层" name="1">
        <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
          <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
        </el-form-item>
      </el-collapse-item>
      <el-collapse-item title="表格列" name="2">
        <draggable :animation="340" group="selectItem" handle=".option-drag">
          <div style="display: flex" class="lableText">
            <el-form-item label="隐藏" label-width="30px"></el-form-item>
            <el-form-item label="列名" label-width="65px"></el-form-item>
            <el-form-item label="列值" label-width="55px"></el-form-item>
            <el-form-item label="宽度(%)" label-width="55px"></el-form-item>
            <el-form-item label="对齐" label-width="55px"></el-form-item>
            <el-form-item label="颜色" label-width="55px"></el-form-item>
          </div>

          <div v-for="(item, index) in cols" :key="index" class="select-item" style="align-items: center;">
            <el-checkbox v-model="item.hide" @change="changeCols()"></el-checkbox>
            <el-input v-model="item.title" placeholder="列名" size="small" @blur.prevent="changeCols()" />
            <el-select v-model="item.field" filterable allow-create placeholder="列值" @change="changeCols()">
              <el-option v-for="(value , key) in originData" :key="key" :value="key" />
            </el-select>
            <!-- <el-input v-model="item.field" placeholder="列值" size="small" @blur.prevent="changeCols()" /> -->
            <el-input v-model="item.width" placeholder="宽度" size="small" @blur.prevent="changeCols()" />
            <el-select v-model="item.isLeft" placeholder="请选择" @change="changeCols()">
              <el-option label="居左" value="left" />
              <el-option label="居中" value="center" />
              <el-option label="居右" value="right" />
            </el-select>
            <el-color-picker v-model="item.textColor" show-alpha @change="changeCols()" size="mini"></el-color-picker>
            <div class="close-btn select-line-icon" @click="removeSelectItem(index)">
              <i class="el-icon-remove-outline" />
            </div>
          </div>
        </draggable>
        <div style="margin-left: 20px">
          <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addSelectItem">添加列</el-button>
        </div>
      </el-collapse-item>
      <el-collapse-item title="表格行样式设置" name="22">
        <draggable :animation="340" group="selectItem" handle=".option-drag">
          <div style="display: flex" class="lableText">
            <el-form-item label="列" label-width="75px"></el-form-item>
            <el-form-item label="字段" label-width="75px"></el-form-item>
            <el-form-item label="字段值" label-width="75px"></el-form-item>
            <el-form-item label="颜色" label-width="35px"></el-form-item>
            <el-form-item label="背景" label-width="35px"></el-form-item>
          </div>

          <div v-for="(item, index) in configData.chartOption.tableColumsStyle" :key="index" class="select-item" style="align-items: center;">
            <el-select v-model="configData.chartOption.tableColumsStyle[index].col" filterable allow-create placeholder="列">
              <template v-for="(value , key) in cols">
                <el-option :key="key" :value="value.field" :label="value.title" v-if="!value.hide||value.hide===undefined"/>
              </template>
            </el-select>
            <el-select v-model="configData.chartOption.tableColumsStyle[index].field" filterable allow-create placeholder="字段">
              <el-option v-for="(value , key) in originData" :key="key" :value="key" />
            </el-select>
            <el-input v-model="configData.chartOption.tableColumsStyle[index].value" placeholder="字段值" size="small"/>
            <el-color-picker v-model="configData.chartOption.tableColumsStyle[index].textColor" show-alpha size="small"></el-color-picker>
            <el-color-picker v-model="configData.chartOption.tableColumsStyle[index].textBgColor" show-alpha size="small"></el-color-picker>
            <div class="close-btn select-line-icon" @click="removeTableColumsCondition(index)">
              <i class="el-icon-remove-outline" />
            </div>
          </div>
        </draggable>
        <div style="margin-left: 20px">
          <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addTableColumsCondition">添加条件</el-button>
        </div>
      </el-collapse-item>
      <el-collapse-item title="表头样式" name="3">
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
      <el-collapse-item title="表格样式" name="4">
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
        <el-form-item label="显示行边框">
          <el-switch v-model="configData.chartOption.showRowBorder" />
        </el-form-item>
        <el-form-item label="显示列边框">
          <el-switch v-model="configData.chartOption.showColumnBorder" />
        </el-form-item>
        <el-form-item label="边框颜色">
          <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.tbodyfontsize !== undefined" label="边框宽度">
          <el-input-number class="inputFontSize" v-model="configData.chartOption.borderWidth" controls-position="right" :step="1" />
        </el-form-item>
        <el-form-item label="换行">
          <el-radio-group v-model="configData.chartOption.wordWrap">
            <el-radio label="hidden">超出隐藏</el-radio>
            <el-radio label="breakAll">自动换行</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.tbodyfontcolor !== undefined" label="字体颜色">
          <el-color-picker v-model="configData.chartOption.tbodyfontcolor" show-alpha />
        </el-form-item>

        <el-form-item v-if="configData.chartOption.evencolor !== undefined" label="奇数行颜色">
          <el-color-picker v-model="configData.chartOption.evencolor" show-alpha />
        </el-form-item>

        <el-form-item v-if="configData.chartOption.oddcolor !== undefined" label="偶数行颜色">
          <el-color-picker v-model="configData.chartOption.oddcolor" show-alpha />
        </el-form-item>

        <el-form-item label="选中样式">
          <el-color-picker  v-model="configData.chartOption.selectedColor" show-alpha />
        </el-form-item>
      </el-collapse-item>
      <el-collapse-item title="分页设置" name="5">
          <el-form-item label="开启分页">
            <el-switch v-model="configData.chartOption.pagination" />
          </el-form-item>
          <el-form-item label="只有第一页是否隐藏">
            <el-switch v-model="configData.chartOption.hideOne" />
          </el-form-item>
          <el-form-item label="分页颜色">
            <el-color-picker v-model="configData.chartOption.pageColor" show-alpha></el-color-picker>
          </el-form-item>
        <el-form-item label="显示条数">
          <el-input-number class="inputFontSize" v-model="configData.chartOption.pageSize" controls-position="right" :min="1" :step="1"></el-input-number>
        </el-form-item>
      </el-collapse-item>
      <el-collapse-item title="动画" name="6">
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
  watch: {
    costomData: {
      immediate: true,
      deep: true,
      handler() {
        this.initResult()
      },
    }
  },
  components: {
    draggable
  },
  data() {
    return {
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      chartList: [],
      originData: [],
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
        hide:false,
        field: "",
        title: "",
        width: 10,
        isLeft:'',
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
    addTableColumsCondition(){
      if(this.configData.chartOption.tableColumsStyle&&this.configData.chartOption.tableColumsStyle.length>0){
        this.configData.chartOption.tableColumsStyle.push({
          col:'',
          field:'',
          value:'',
          textColor:'',//背景颜色
          textBgColor:''
        })
      }else{
        this.configData.chartOption.tableColumsStyle=[]
        this.configData.chartOption.tableColumsStyle.push({
          col:'',
          field:'',
          value:'',
          textColor:'',
          textBgColor:''//文字颜色
        })
      }
    },
    removeTableColumsCondition(index){
      this.configData.chartOption.tableColumsStyle.splice(index, 1);
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