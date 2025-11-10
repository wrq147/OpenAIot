<template>
  <div
    :class="className"
    :style="{ height: height, width: width, overflow: 'hidden' }"
    :id="chartOption.bindingDiv"
  >
    <div id="hot-preview">
      <HotTable
        :root="root"
        ref="testHot"
        :settings="hotSettings"
        :style="{ height: height, width: width }"
        :key="index"
      ></HotTable>
    </div>
  </div>
</template>

<script>
import { HotTable } from "@handsontable-pro/vue";
import "handsontable/dist/handsontable.full.css";
import "handsontable-pro/languages/zh-CN";

import resize from "@/views/dashboard/mixins/resize";
import VueEvent from "../../VueEvent";
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
export default {
  components: {
    HotTable
  },
  mixins: [resize,dataChart],
  props: {
    className: {
      type: String,
      default: "chart"
    },
    width: {
      type: String,
      default: "100%"
    },
    height: {
      type: String,
      default: "100%"
    },
  },
  watch: {
    width() {
    },
    height() {
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {

  },
  data: function() {
    return {
      dataOption: JSON.parse(JSON.stringify(this.chartOption)),
      index: 1,
      root: "test-hot",
      hotSettings: {
        language: "zh-CN",
        licenseKey: "non-commercial-and-evaluation", //提示非商业用途
        filters: true,

        // manualColumnFreeze: true, //手动固定列  ?
        // manualColumnMove: true, //手动移动列
        manualRowMove: true, //手动移动行
        manualColumnResize: true, //手工更改列距
        manualRowResize: true, //手动更改行距
        // columnSorting: true,//排序
        autoColumnSize: true,
        mergeCells: true,
        autoWrapRow: true, //文字是否自动换行(当没有设置colWidths时生效)
        // stretchH: 'all',//根据宽度横向扩展，last:只扩展最后一列，none：默认不扩展
        fillHandle: true, //选中拖拽复制 possible values: true, false, "horizontal", "vertical"
        fixedColumnsLeft: 2, //固定左边列数
        fixedRowsTop: 2, //固定上边列数
        readonly: true, //表格是否只读，有true和false两种选择
        data: [
          [false, "20080101", 10, 11, 12, 13, true],

          [false, "20090101", 20, 11, 14, 13, true],

          [false, "20010101", 30, 15, 12, 13, true],

          [false, "20010101", 32, 213, 21, 312, true],

          [false, "20010201", 32, 213, 21, 312, true]
        ],
        rowHeaders: true,
        // colHeaders: true,
        // colHeaders:   [ '选择','题目', 'A选项', 'B选项', 'C选项', 'D选项','答案'],//自定义列表头or 布尔值
        // rowHeaders: [1,2],

        // columns: [     //添加每一列的数据类型和一些配置
        //   {type: 'checkbox'},  //多选框
        //   {
        //     type: 'date',   //时间格式
        //     dateFormat: 'YYYY-MM-DD',
        //     correctFormat: true,
        //     defaultDate: '19000101'
        //   },
        //   {
        //     type: 'dropdown', //下拉选择
        //     source: ['BMW', 'Chrysler', 'Nissan', 'Suzuki', 'Toyota', 'Volvo'],
        //     strict: false   //是否严格匹配
        //   },
        //   {type: 'numeric'},  //数值
        //   {type: 'numeric',
        //     readOnly: true  //设置只读
        //   },
        //   { type: 'numeric',
        //     format: '$ 0,0.00'},  //指定的数据格式
        //   {},

        // ],
        contextMenu: true
      }
    };
  },
  methods: {
    setChartVal(result) {
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      let colHeaders = [];
      if (dataOption.cols.length > 0 && dataOption.colHeaders == true) {
        dataOption.cols.forEach(element => {
          colHeaders.push(element.title);
        });
      } else {
        colHeaders = dataOption.colHeaders;
      }

      this.$refs.testHot.settings.colHeaders = colHeaders;
      this.$refs.testHot.settings.autoWrapRow = dataOption.autoWrapRow;
      this.$refs.testHot.settings.fixedColumnsLeft =
        dataOption.fixedColumnsLeft;
      this.$refs.testHot.settings.fixedRowsTop = dataOption.fixedRowsTop;
      this.$refs.testHot.settings.rowHeaders = dataOption.rowHeaders;
      this.$refs.testHot.settings.readonly = dataOption.readonly;
      this.index++;
      this.$set(this.hotSettings, "data", result);
    }
  }
};
</script>
