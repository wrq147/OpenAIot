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
        <el-form
        v-if="currentTab === 'field'"
          size="small"
          label-width="90px"
        >
          <el-form-item
            v-if="configData.layerName !== undefined"
            label="图层名称"
          >
            <el-input
              v-model="configData.layerName"
              placeholder="请输入图层名称"
            />
          </el-form-item>

          <el-form-item label="自定义图表option">
            <el-input
              type="textarea"
              :rows="15"
              v-model="configData.chartOption.customOption"
              @click.native="coutomChartEdit()"
              readonly
            ></el-input>
          </el-form-item>

          <el-form-item label="组件库">
            <el-button type="primary" @click="openCustomChart"
              >自定义组件库</el-button
            >
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
        <el-form v-if="currentTab === 'data'" size="small" label-width="90px">
          
          <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType" :customData="configData" :customId="configData.customId" 
            @changeSource="changeSource"  @changeData="changeData" :baseType="''"></data-source-config>


        </el-form>
        <!-- 组件位置 -->
        <el-form
        v-if="currentTab === 'location'"
          size="small"
          label-width="90px"
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
            <el-input-number v-model="configData.zindex" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
        </el-form>

        <!-- 组件交互 -->
        <el-form
        v-if="currentTab === 'interaction'"
          size="small"
          label-width="90px"
        >
          <el-form-item label="是否开启图表联动">
            <el-switch
              v-model="configData.chartOption.isLink"
              @change="isLinkChange"
            />
          </el-form-item>

          <el-form-item
            v-show="configData.chartOption.isLink === true"
            label="参数名称"
          >
            <el-input
              v-model="arrName"
              placeholder="请参数名称"
              @blur.prevent="changeArrName()"
            />
          </el-form-item>

          <el-form-item
            v-show="configData.chartOption.isLink === true"
            label="绑定组件"
          >
            <el-select
              v-model="configData.chartOption.bindList"
              multiple
              placeholder="请选择"
              @change="bindCharts"
            >
              <el-option
                v-for="item in chartList"
                :key="item.customId"
                :label="item.layerName"
                :value="item.customId"
              >
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item
            v-show="configData.chartOption.isLink === true"
            label="*参数说明"
          >
            <div class="el-form-item__content">
              <span style="word-wrap: break-word;"
                >在编辑器内获取<i style="color:#F00;">requestParameters</i
                >参数即可获得动态绑定交互的参数，格式为：</span
              >
              <span style="word-wrap: break-word;"
                ><i style="color:#F00;"
                  >参数名={"legendName":"xx","seriesName":"xxx","data":xxx}</i
                ></span
              >
            </div>
            <div class="el-form-item__content">
              <span>legendName:图例名称</span>
              <span style="display:block;">seriesName:系列名称</span>
              <span>data:选中值</span>
            </div>
          </el-form-item>
       

          <!-- 是否开启图表远程控制 -->
          <el-form-item label="是否开启图表远程控制">
            <el-switch v-model="configData.chartOption.isRemote" />
          </el-form-item>
          <el-form-item
            v-show="configData.chartOption.isRemote === true"
            label="*参数说明"
          >
            <div class="el-form-item__content">
              <span style="word-wrap: break-word;"
                ><i style="color:#F00;"
                  >{"legendName":"xx","seriesName":"xxx","data":xxx}</i
                ></span
              >
            </div>
            <div class="el-form-item__content">
              <span>legendName:图例名称</span>
              <span style="display:block;">seriesName:系列名称</span>
              <span>data:选中值</span>
            </div>
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>

    <!-- 在线编辑器对话框 -->
    <el-dialog
      title="在线编辑器"
      v-if="open"
      :visible.sync="open"
      width="1500px"
      hight="800px"
      append-to-body
      :close-on-click-modal="false"
    >


      <div slot="footer" class="dialog-footer">
        <!--<el-button type="primary" @click="editSubmit">确 定</el-button>-->
        <el-button @click="cancel">取 消</el-button>
      </div>
    </el-dialog>

    <!-- 自定义组件库对话框 -->
    <el-dialog
      title="自定义组件库"
      :visible.sync="openCustom"
      width="1000px"
      hight="600px"
      append-to-body
    >
      <el-form
        :model="queryParams"
        ref="queryForm"
        :inline="true"
        label-width="68px"
      >
        <el-form-item label="图表名称" prop="name">
          <el-input
            v-model="queryParams.name"
            placeholder="请输入图表名称"
            clearable
            size="small"
            @keyup.enter.native="handleQuery"
          />
        </el-form-item>

        <el-form-item label="所属图表" prop="graph">
          <el-select
            v-model="queryParams.graph"
            placeholder="请选择所属图表"
            clearable
            size="small"
          >
            <el-option
              v-for="dict in graphOptions"
              :key="dict.dictValue"
              :label="dict.dictLabel"
              :value="dict.dictValue"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="所属组件" prop="component">
          <el-select
            v-model="queryParams.component"
            placeholder="请选择所属组件"
            clearable
            size="small"
          >
            <el-option
              v-for="dict in componentOptions"
              :key="dict.dictValue"
              :label="dict.dictLabel"
              :value="dict.dictValue"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="标签" prop="tag">
          <el-input
            v-model="queryParams.tag"
            placeholder="请输入标签"
            clearable
            size="small"
            @keyup.enter.native="handleQuery"
          />
        </el-form-item>
        <!-- <el-form-item label="是否公开" prop="isOpen">
          <el-select
            v-model="queryParams.isOpen"
            placeholder="请选择是否公开"
            clearable
            size="small"
          >
            <el-option
              v-for="dict in isOpenOptions"
              :key="dict.dictValue"
              :label="dict.dictLabel"
              :value="dict.dictValue"
            />
          </el-select>
        </el-form-item>-->

        <el-form-item>
          <el-button
            type="primary"
            icon="el-icon-search"
            size="mini"
            @click="handleQuery"
            >搜索</el-button
          >
        </el-form-item>
      </el-form>

      <div style="overflow:auto; height:500px;">
        <ul
          class="background-images-ul"
          v-infinite-scroll="load"
          infinite-scroll-disabled="disabled"
        >
          <li
            v-for="item in customchartList"
            @click="selectImg(item.chartOption)"
          >
            <span
              title="删除"
              class="drawing-item-delete"
              @click.stop="deleteCustomChart(item.id)"
              ><i class="el-icon-delete"></i
            ></span>
            <img :src="basePath + item.thumbnail" class="border_img" />
            <label :title="item.name">{{ item.name }}</label>
          </li>
        </ul>
        <p v-if="loading" style="text-align: center; margin-top: 100px;">
          加载中...
        </p>
        <p v-if="noMore" style="text-align: center; margin-top: 100px;">
          没有更多了
        </p>
      </div>

      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { animateOptions } from "../../animate/animate";

import { listCustomchart, delCustomchart } from "@/api/report/customchart";

import { getLinkChart } from "../../util/LinkageChart";
import DataSourceConfig from './DataConfig/DataSourceConfig'
import sourceConfig from '../mixins/sourceConfig.js'
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
  },
  data() {
    return {
      basePath: process.env.VUE_APP_BASE_API,
      open: false,
      openCustom: false,
      currentTab: "field",
      animateOptions,
      // 所属图表字典
      graphOptions: [],
      // 所属组件字典
      componentOptions: [],
      // 是否公开字典
      isOpenOptions: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        name: undefined,
        chartOption: undefined,
        graph: undefined,
        component: undefined,
        tag: undefined,
        isOpen: undefined,
        status: undefined
      },
      // 总条数
      total: -1,
      // 自定义组件库表格数据
      customchartList: [],
      loading: false,
      chartList: this.drawingList,
    };
  },
  //页面加载完执行
  mounted() {
    // VueEvent.$on("submit_app", data => {
    //     this.open = false;
    //     this.$set(this.configData.chartOption, "customOption", data);
    // });
    //获取可联动组件列表
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
    noMore() {
      if (this.total != -1) {
        return this.total == this.customchartList.length;
      } else {
        return false;
      }
    },
    disabled() {
      return this.loading || this.noMore;
    }
  },
  methods: {
    submitApp(data){
      this.open = false;
      this.$set(this.configData.chartOption, "customOption", data);
    },
    coutomChartEdit(customOption) {
      this.open = true;
    },
    editSubmit() {},
    cancel() {
      this.openCustom = false;
    },
    openCustomChart() {
      this.getDicts("datav_graph").then(response => {
        this.graphOptions = response.data;
      });
      this.getDicts("datav_component").then(response => {
        this.componentOptions = response.data;
      });
      this.getDicts("sys_yes_no").then(response => {
        this.isOpenOptions = response.data;
      });

      this.total = -1;
      this.customchartList = [];
      this.queryParams.pageNum = 1;
      this.openCustom = true;
      this.load();
    },
    load() {
      this.loading = true;
      listCustomchart(this.queryParams).then(response => {
        this.customchartList = this.customchartList.concat(response.rows);
        this.total = response.total;
        this.queryParams.pageNum++;
        this.loading = false;
      });
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.total = -1;
      this.customchartList = [];
      this.queryParams.pageNum = 1;
      this.openCustom = true;
      this.load();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    selectImg(item) {
      this.$set(this.configData.chartOption, "customOption", item);
      //this.configData.chartOption.border = item;
      this.openCustom = false;
    },
    deleteCustomChart(id) {
      this.$confirm("是否确认删除该自定义组件?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          delCustomchart(id).then(response => {
            if (response.code === 200) {
              this.msgSuccess("删除成功");
              //重新加载上传数据
              this.total = -1;
              this.customchartList = [];
              this.queryParams.pageNum = 1;
              this.load();
            } else {
              this.msgError(response.msg);
            }
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除操作"
          });
        });
    },
    changeArrName() {
      this.$set(this.configData.chartOption, "arrName", this.arrName);
    },
    bindCharts(val) {
      this.$set(this.configData.chartOption, "bindList", val);
    },
    isLinkChange() {
      if (this.configData.chartOption.isLink == true) {
        this.$set(this.configData.chartOption, "isDrillDown", false);
        //this.configData.chartOption.isDrillDown == false;
      }
    },
    
    getRemote(val) {
      // console.log(val)
      this.$set(this.configData.chartOption, "remote", val);
    }
  }
};
</script>

<style lang="scss" scoped>
background-images-ul {
  margin-bottom: 20px;
}
.background-images-ul li {
  position: relative;
  margin: 2px;
  display: inline-block;
  width: 200px;
  line-height: 25px;
  padding: 20px;
  border: 1px solid #e2e2e2;
  font-size: 14px;
  text-align: center;
  color: #666;
  transition: all 0.3s;
  -webkit-transition: all 0.3s;
  cursor: pointer;
  &:hover {
    & > .drawing-item-delete {
      display: initial;
    }
  }
  & > .drawing-item-copy,
  & > .drawing-item-delete {
    display: none;
    position: absolute;
    top: 0px;
    left: 175px;
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
  & > .drawing-item-delete {
    right: 24px;
    border-color: #f56c6c;
    color: #f56c6c;
    background: #fff;
    &:hover {
      background: #f56c6c;
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
  color: #0088ff;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.row {
  margin-right: -15px;
  margin-left: -15px;
}
::v-deep .center-tabs .el-tabs__item {
  width: 25%;
  text-align: center;
}</style
>>
