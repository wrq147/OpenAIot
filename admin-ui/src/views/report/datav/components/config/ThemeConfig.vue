<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="主题" name="field" />
    </el-tabs>
    <div class="field-box">

      <el-scrollbar class="right-scrollbar">
        <!-- 组件属性 -->
        <el-form v-if="currentTab === 'field'" size="small" label-width="90px">

          <el-form-item v-if="configData.panelWidth !== undefined" label="大屏宽度">
            <el-input-number controls-position="right" v-model="configData.panelWidth"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.panelHeight !== undefined" label="大屏高度">
            <el-input-number controls-position="right" v-model="configData.panelHeight"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.isSelfAdaption !== undefined" label="是否自适应">
            <el-switch v-model="configData.isSelfAdaption" active-text="是" inactive-text="否" />
          </el-form-item>

          <el-form-item v-if="configData.adaptionType !== undefined && configData.isSelfAdaption == true" label="自适应类型">
            <el-radio-group v-model="configData.adaptionType" class="adaption_radio">
              <el-radio label="0">全自适应</el-radio>
              <el-radio label="1">宽度自适应</el-radio>
              <el-radio label="2">高度自适应</el-radio>
            </el-radio-group>
          </el-form-item>

          

          <el-form-item v-if="configData.bgColor !== undefined" label="背景颜色">
            <el-color-picker v-model="configData.bgColor" show-alpha />
          </el-form-item>

          <el-form-item v-if="configData.bgImage !== undefined" label="背景图片">
            <image-upload v-model="configData.bgImage" :limit="1"></image-upload>
          </el-form-item>

          <el-form-item v-if="configData.themeColor !== undefined" label="主题颜色">
            <div class="theme-plan-group" style="cursor: pointer;" @click="openTheme">
              <div class="theme-plan-color" v-for="(c, i) in (configData.themeColor.color)" :key="i"
                :style="{ 'background-color': c }">
              </div>
              <div class="theme-plan-color" v-show="configData.themeColor.color.length % 8 != 0"
                v-for="(item1, index1) in 8 - configData.themeColor.color.length % 8" :key="index1 + 'txx'"></div>
            </div>
          </el-form-item>

        </el-form>

        
      </el-scrollbar>
    </div>


    <!-- 自定义主题库对话框 -->
    <el-dialog title="主题库" :visible.sync="themeOpen" width="900px" height="600px" append-to-body>
      <el-form :model="themeQueryParams" ref="themeQueryParams" :inline="true" label-width="68px">
        <el-form-item label="主题名称">
          <el-input v-model="themeQueryParams.themeName" placeholder="主题名称" clearable size="small" style="width: 240px" />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleThemeQuery">搜索</el-button>
        </el-form-item>
      </el-form>

      <div style="overflow-y:auto;overflow-x:hidden; height:500px;">
        <ul class="background-images-ul" v-infinite-scroll="themeLoad" infinite-scroll-disabled="themeDisabled">
          <div class="row">
            <div class="col-xs-6" style="display: flex;flex-wrap: wrap;" v-for="(item, index) in themeList" :key="index"
              @click="selectTheme(JSON.parse(item.ThemeOption))">
              <span style="margin-right: 20px;">{{ item.ThemeName }}</span>
              <div class="theme-plan-color" v-for="(c, index) in (JSON.parse(item.ThemeOption).color)" :key="index"
                :style="{ 'background-color': c }">
              </div>

            </div>
          </div>
        </ul>

        <p v-if="themeLoading" style="text-align: center; margin-top: 100px;">加载中...</p>
        <p v-if="themeNoMore" style="text-align: center; margin-top: 100px;">没有更多了</p>
      </div>

      <div slot="footer" class="dialog-footer">
        <el-button @click="cancelTheme">取 消</el-button>
      </div>
    </el-dialog>


  </div>
</template>

<script>

import { listTheme } from "@/api/report/theme";
import VueEvent from '../../VueEvent'
import FormData from './DataConfig/FormData'

export default {
  props: ["costomData", "drawingList", 'themeForm'],
  components: {
    FormData,
  },
  data() {
    return {
      currentTab: 'field',
      configData: this.costomData.chartType === 'themeForm' ? this.costomData : this.themeForm ,
      themeOpen: false,
      themeQueryParams: {
        pageNum: 1,
        pageSize: 10,
        themeName: undefined
      },
      themeList: [],
      dataBaseType: '',
      themeTotal: -1,
      themeLoading: false,
      curIdx: null,
    }
  },
  //页面加载完执行
  mounted() {

  },
  created() {},
  computed: {
    themeNoMore() {
      if (this.themeTotal != -1) {
        return this.themeTotal == this.themeList.length
      } else {
        return false;
      }
    },
    themeDisabled() {
      return this.themeLoading || this.themeNoMore
    }
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal) {
        // console.log("1=>", newVal);
        //this.$emit("costom-change", newVal);
      }
    },
    costomData: {
      deep: true,
      handler(newVal) {
        // console.log("barConfig=>costomData.watch")
        this.configData = newVal;
        //this.staticDataValue = JSON.stringify(newVal.chartOption.staticDataValue);
      }
    },
    'configData.panelWidth': {
      handler(newValue, oldValue) {
        VueEvent.$emit("canvasResize", this.configData.panelWidth);
      }
    },
    'configData.panelHeight': {
      handler(newValue, oldValue) {
        VueEvent.$emit("canvasResize", this.configData.panelHeight);
      }
    }
  },
  methods: {
    openTheme() {
      this.themeOpen = true;
    },
    themeLoad() {
      this.themeLoading = true;
      listTheme(this.themeQueryParams).then(response => {
        this.themeList = this.themeList.concat(response.data.List);

        this.themeTotal = response.data.Total;
        this.themeQueryParams.pageNum++;
        this.themeLoading = false;
      });
    },
    selectTheme(value) {
      this.$set(this.configData, 'themeColor', value);
      this.themeOpen = false;
    },
    handleThemeQuery() {
      this.themeTotal = -1;
      this.themeList = [];
      this.themeQueryParams.pageNum = 1;
      this.themeOpen = true;
      this.themeLoad();
    },
    uploadOpenTheme() {
      this.themeUploadOpen = true;
    },
    cancelTheme() {
      this.themeOpen = false;
    },
  }

}
</script>

<style lang='scss' scoped>

::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-select{
    width: 100%;
  }
  .el-input-number--small{
    width: 100%;
  }
  .el-tabs__item{
    width: 100%;
  }
  .el-tabs__active-bar{
    width: 100%;
  }
}
.background-images-ul {
  margin-bottom: 20px;
}

.background-images-ul li {
  margin: 2px;
  display: inline-block;
  width: 200px;
  line-height: 25px;
  padding: 20px;
  border: 1px solid #e2e2e2;
  font-size: 14px;
  text-align: center;
  color: #666;
  transition: all .3s;
  -webkit-transition: all .3s;
  cursor: pointer;
  position: relative;

  &:hover {

    &>.drawing-item-delete {
      display: initial;
    }
  }

  &>.drawing-item-copy,
  &>.drawing-item-delete {
    display: none;
    position: absolute;
    top: 0px;
    right: 15px;
    width: 22px;
    height: 22px;
    line-height: 20px;
    text-align: center;
    border-radius: 50%;
    font-size: 12px;
    border: 1px solid;
    cursor: pointer;
    z-index: 1;
  }

  &>.drawing-item-delete {
    right: 0px;
    border-color: #F56C6C;
    color: #F56C6C;
    background: #fff;

    &:hover {
      background: #F56C6C;
      color: #fff;
    }
  }
}

.border_img {
  width: 160px;
  height: 90px;
}

.background-images-ul label {
  display: inline-block;
  width: 160px;
  color: #DBEEFF;
  overflow: hidden;
}

.row {
  margin-right: -15px;
  margin-left: -15px;
}

.col-xs-6 {
  width: 50%;
  float: left;
  cursor: pointer;
  padding: 10px;
  position: relative;

  /**background-color: #b3d4ff;**/
  &:hover {

    &>.drawing-item-delete {
      display: initial;
    }
  }

  &>.drawing-item-copy,
  &>.drawing-item-delete {
    display: none;
    position: absolute;
    top: 9px;
    right: 15px;
    width: 22px;
    height: 22px;
    line-height: 20px;
    text-align: center;
    border-radius: 50%;
    font-size: 12px;
    border: 1px solid;
    cursor: pointer;
    z-index: 1;
  }

  &>.drawing-item-delete {
    right: 24px;
    border-color: #F56C6C;
    color: #F56C6C;
    background: #fff;

    &:hover {
      background: #F56C6C;
      color: #fff;
    }
  }
}
.theme-plan-group {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  width: auto;
  height: 100px;
  overflow: hidden;
  border: 1px solid #eee;
  padding: 5px;
  border-radius: 4px;
  margin-bottom: 8px;
}

.theme-plan-color {
  width: 20px;
  height: 20px;
  margin-bottom: 10px;
  margin-left: 2px;
  margin-right: 2px;
  display: inline-block;
  border-radius: 3px;
}

.adaption_radio {
  display: flex;
  flex-direction: column;
}

.adaption_radio .el-radio {
  line-height: 25px;
}

.background-images-ul label {
  max-width: 160px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

</style>