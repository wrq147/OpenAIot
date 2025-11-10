<template>
  <div
    :class="animate"
    :style="{
      height: height,
      width: width,
      overflowX: 'hidden',
      overflowY: overflowY
    }"
    :id="chartOption.bindingDiv"
    @mouseenter="visible"
    @mouseleave="invisible"
    ref="text"
  >
    <el-table
      :show-summary="chartOption.isSum"
      :summary-method="getSummaries"
      :data="getData()"
      @row-click="clickRow"
      :row-class-name="tableRowClassName"
      :cell-style="cellStyle"
      :header-cell-style="headerStyle"
      style="width: 100%;overflow:auto"
      ref="tbody"
      :border="
        this.chartOption.border == undefined ? false : this.chartOption.border
      "
    >
      <el-table-column
        v-for="(item, index) in this.chartOption.cols"
        :sortable="chartOption.isSort"
        :key="index"
        :prop="item.field"
        :label="item.title"
        :width="item.width"
        :show-overflow-tooltip="true"
      >
        <!--<a @click="fieldClick(item.field, item.title)" style="color:blue;cursor:pointer" v-if="isBindingChart(item.field)">{{item.title}}</a>-->
        <template slot-scope="scope">
          <a
            @click="fieldClick(item.field, getFieldValue(item.field, scope))"
            :style="linkStyle(item)"
            v-if="isBindingChart(item.field)"
            >{{ getFieldValue(item.field, scope) }}</a
          >
          <a style="cursor:default;" v-else>{{
            getFieldValue(item.field, scope)
          }}</a>
        </template>
      </el-table-column>
    </el-table>
    <!-- 分页器 -->
    <div style="margin-top:15px;width: 100%;" v-if="chartOption.pagination==undefined || chartOption.pagination==true">
      <el-pagination
        align="center"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="currentPage"
        :page-sizes="[1, 5, 10, 20]"
        :page-size="this.chartOption.pageSize"
        :pager-count="5"
        layout="total, sizes, prev, pager, next, jumper"
        :total="this.tableValue.length"
        style="display: flex;flex-wrap:wrap;"
      >
      </el-pagination>
    </div>
  </div>
</template>

<script>
import VueEvent from "../../VueEvent";
import "../../animate/animate.css";
import { getLinkChart } from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
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
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      currentPage: 1, // 当前页码
      animate: this.className,
      selectedRow: undefined,
      overflowY: "hidden",
      tableValue:this.chartOption.staticDataValue,
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    console.log(this.height, 'height')
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed: {
    cellStyle() {
      var self = this;
      return function({ row, column, rowIndex, columnIndex }) {
        let style = {
          height: self.chartOption.tbodyheight + "px",
          background: "transparent",
          fontSize: self.chartOption.tbodyfontsize + "px",
          fontFamily: self.chartOption.tbodyfontfamily,
          color: self.chartOption.tbodyfontcolor,
          textAlign: self.chartOption.isLeft,
          verticalAlign: "middle"
        };
        for (const key of self.chartOption.cols) {
          if (column.property === key.field) {
            style.color = key.textColor;
          }
        }
        if (rowIndex == self.selectedRow && self.chartOption.selectedColor != null) {
          style.background = self.chartOption.selectedColor;
        } else if (self.chartOption.tbodybackgroundcolor != null) {
          style.background = self.chartOption.tbodybackgroundcolor;
        }
        return style;
      };
    },
    

    headerStyle() {
      return {
        height: this.chartOption.theadheight + "px",
        backgroundColor: this.chartOption.theadbackgroundcolor,
        fontSize: this.chartOption.theadfontsize + "px",
        fontFamily: this.chartOption.theadfontfamily,
        color: this.chartOption.theadfontcolor,
        textAlign: this.chartOption.isLeft,
        verticalAlign: "middle"
      };
    }
  },
  methods: {
    getData(){
      if(this.chartOption.pagination == true || this.chartOption.pagination == undefined){
       return this.tableValue.slice(
          (this.currentPage - 1) * this.chartOption.pageSize,
          this.currentPage * this.chartOption.pageSize
          
        )
      }else{
        return this.tableValue
      }
    },
    getSummaries(param) {
      
        const { columns, data } = param;
        const sums = [];
        columns.forEach((column, index) => {
          if (index === 0) {
            sums[index] = '总计';
            return;
          }
          if(this.chartOption.sumField != undefined){
            if(this.chartOption.sumField.indexOf(column.property) > -1){
              const values = data.map(item => Number(item[column.property]));
              if (!values.every(value => isNaN(value))) {
                sums[index] = values.reduce((prev, curr) => {
                  const value = Number(curr);
                  if (!isNaN(value)) {
                    return prev + curr;
                  } else {
                    return prev;
                  }
                }, 0);
                // sums[index] += ' 元';
              } else {
                sums[index] = 'N/A';
              }
            }
          }
          
        });
        if(this.chartOption.sumField != undefined){
          let that = this.$el;
          if(that != undefined){
            let current = document.getElementById(this.chartOption.bindingDiv)
                .querySelector(".el-table__footer-wrapper")
                .querySelector(".el-table__footer").getElementsByTagName("div");
            
            if(current!= undefined){
              for (let cell of current) {
                cell.style.fontSize= this.chartOption.tbodyfontsize + "px" 
                cell.style.height= this.chartOption.tbodyheight + "px";
                cell.style.backgroundColor= this.chartOption.tbodybackgroundcolor;
                cell.style.fontSize= this.chartOption.tbodyfontsize + "px";
                cell.style.fontFamily= this.chartOption.tbodyfontfamily;
                cell.style.color= this.chartOption.tbodyfontcolor;
                cell.style.textAlign= this.chartOption.isLeft;
                cell.style.lineHeight= this.chartOption.tbodyheight + "px";
                cell.style.verticalAlign= "middle"
              }
            }
          }
        }
        return sums;
    },

    setChartVal(result) {
      let dataOption = this.dataOption;

      this.tableValue = result;
      
      this.getData();
      //设置边框
      if(dataOption.tdBorder == false){
        let rows =  document.getElementById(this.chartOption.bindingDiv)
                  .querySelectorAll(".el-table__row")
        
        for (let row of rows) {
         let tds =  row.getElementsByTagName("td")
          for (let td of tds) {
            td.style.borderBottom = "none"
          }
        }
      }else{
        let rows =  document.getElementById(this.chartOption.bindingDiv)
                  .querySelectorAll(".el-table__row")
        
        for (let row of rows) {
         let tds =  row.getElementsByTagName("td")
          for (let td of tds) {
            td.style.borderBottom = "1px solid #dfe6ec"
          }
        }
      }

    },
    //每页条数改变时触发 选择一页显示多少行
    handleSizeChange(val) {
      this.currentPage = 1;
      this.chartOption.pageSize = val;
    },
    //当前页改变时触发 跳转其他页
    handleCurrentChange(val) {
      this.currentPage = val;
    },
    rowClick(val) {},
    getFieldValue(field, scope) {
      return scope.row["" + field + ""];
    },
    fieldClick(field, val) {
      //获取联动图表
      if (this.chartOption.isLink == true) {
        let linkageTag = this.chartOption.linkageCols.filter(function(item) {
          return item.field == field;
        })[0];
        if (linkageTag != undefined) {
          let bindList = linkageTag.bindingChart;

          if (bindList.length > 0) {
            getLinkChart(linkageTag.paramName, val, bindList, this.drawingList);
          }
        }
      }
      //开启图表下钻
      else if (this.chartOption.isDrillDown == true) {
        //获取联动图表
        let drillTag = this.chartOption.drillCols.filter(function(item) {
          return item.field == field;
        })[0];
        if (drillTag != undefined && drillTag.bindingChart != "") {
          let num = 0;
          for (let i = 0; i < this.chartOption.drillCols.length; i++) {
            if (this.chartOption.drillCols[i].field == drillTag.field) {
              num = i;
            }
          }

          //获取绑定的图表
          let drillDownChartOption = drillTag.drillDownChartOption;

          drillDownChartOption.drillBgColor = drillTag.drillBgColor;

          if (
            drillDownChartOption != undefined &&
            drillDownChartOption != null
          ) {
            //判断是否有参数传递
            if (
              drillTag.paramName != null &&
              drillTag.paramName != "" &&
              drillTag.paramName != undefined
            ) {
              this.$set(
                this.chartOption.drillCols[num].drillDownChartOption
                  .chartOption,
                "requestParameters",
                [{"name":drillTag.paramName,"value":val}]
              );
              drillDownChartOption.chartOption.requestParameters = [{"name":drillTag.paramName,"value":val}]
            } else {
              this.$set(
                this.chartOption.drillCols[num].drillDownChartOption
                  .chartOption,
                "requestParameters",
                []
              );
              drillDownChartOption.chartOption.requestParameters = [];
            }
            //发送下钻消息
            VueEvent.$emit("drill_down_msg", drillDownChartOption);
          }
        }
      }
     
     
    },
    isBindingChart(field) {
      let remoteTag = false;
      let linkTag = false;

      if (
        this.chartOption.isRemote == true &&
        this.chartOption.remoteCols != undefined
      ) {
        let remoteLinkTag = this.chartOption.remoteCols.filter(function(item) {
          return item.field == field;
        })[0];
        if (remoteLinkTag != null) {
          remoteTag = true;
        }
      }

      if (
        this.chartOption.isLink == true &&
        this.chartOption.linkageCols != undefined
      ) {
        let linkageTag = this.chartOption.linkageCols.filter(function(item) {
          return item.field == field;
        })[0];
        if (linkageTag != null) {
          linkTag = true;
        }
      } else if (
        this.chartOption.isDrillDown == true &&
        this.chartOption.drillCols != undefined
      ) {
        let drillTag = this.chartOption.drillCols.filter(function(item) {
          return item.field == field;
        })[0];
        if (drillTag != null) {
          linkTag = true;
        }
      }

      //远程控制标志以及联动标志同时为false则为false，否则为true
      if (remoteTag == false && linkTag == false) {
        return false;
      } else {
        return true;
      }
    },
    linkStyle(colItem) {
      if (this.chartOption.isLink == true) {
        //获取联动图表
        let linkageTag = this.chartOption.linkageCols.filter(function(item) {
          return item.field == colItem.field;
        })[0];

        if (linkageTag != undefined) {
          return { color: linkageTag.linkColor, cursor: "pointer" };
        }
      } else if (this.chartOption.isDrillDown == true) {
        //获取下钻图表
        let drillTag = this.chartOption.drillCols.filter(function(item) {
          return item.field == colItem.field;
        })[0];

        if (drillTag != undefined) {
          return { color: drillTag.linkColor, cursor: "pointer" };
        }
      }

      if (this.chartOption.isRemote == true) {
        //获取远程控制图表
        let remoteTag = this.chartOption.remoteCols.filter(function(item) {
          return item.field == colItem.field;
        })[0];

        if (remoteTag != undefined) {
          return { color: remoteTag.linkColor, cursor: "pointer" };
        }
      }
    },
    visible() {
      this.overflowY = "auto";
    },
    invisible() {
      this.overflowY = "hidden";
    },
  
    tableRowClassName({ row, rowIndex }) {
      row.index = rowIndex;
    },
    clickRow(row) {
      if (this.selectedRow === row.index) {
        this.selectedRow = undefined;
      } else {
        this.selectedRow = row.index;
      }
    }
  }
};
</script>
<style ang="scss" scoped>
.tabBorder {
  border-style: solid;
  height: 90%;
  position: relative;
}
.tabText {
  position: absolute;
  top: 38%;
  width: 100%;
}
.tabli {
  list-style: none;
  height: 100%;
  padding-left: 1px;
  float: left;
  cursor: default;
}
/*最外层透明*/
::v-deep .el-table,
::v-deep .el-table__expanded-cell {
  background-color: transparent;
}
/* 表格内背景颜色 */
::v-deep .el-table th,
::v-deep .el-table tr,
::v-deep .el-table td {
  background-color: transparent;
}

::v-deep .el-pager li {
  background-color: transparent;
}
::v-deep .el-pagination__jump .el-input__inner {
  color: #fff;
  background-color: transparent;
}
::v-deep .el-pagination__sizes .el-input__inner {
  color: #fff;
  background-color: transparent;
}
::v-deep .el-pagination .btn-prev,
.el-pagination .btn-next {
  color: #fff;
  background-color: transparent;
}
::v-deep .btn-next {
  color: #fff;
  background-color: transparent;
}
::v-deep .el-pagination button:disabled {
  background-color: transparent;
}
::v-deep.el-pagination {
  color: #fff;
}
::v-deep .el-pagination__jump {
  color: #fff;
}
::v-deep.el-pagination .el-pagination__total {
  color: #fff;
}
::v-deep .el-table .cell {
  white-space: pre !important;
}
::v-deep .el-table .el-table__cell{
  padding: 0 !important;
}
::v-deep.el-pager li.btn-quicknext,
.el-pager li.btn-quickprev {
  line-height: 28px;
  color: #ffffff;
}
::v-deep .el-table::before {
  height: 0px;
}
::v-deep.el-table th{
  border-bottom: none !important;
  border-top: none !important;
}
</style>
