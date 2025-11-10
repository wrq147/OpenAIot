<template>
  <div :class="animate" :style="{ height: height, width: width, overflow: 'hidden' }" :id="chartOption.bindingDiv" ref="text">
    <table :style="{ height: height, width: width, tableLayout: 'fixed' }">
      <thead :style="headTableStyle">
        <tr>
          <th :style="topheadStyle">排名</th>
          <template v-for="(item, index) in this.chartOption.cols">
            <th :key="index" :style="theadStyle(item)" v-if="!item.hide||item.hide===undefined">{{ item.title }}</th>
          </template>
          
        </tr>
      </thead>
      <tbody :style="tbodyStyle" ref="tbody" @mouseenter="visible" @mouseleave="invisible">
        <tr v-for="(item, index) in tableData" :key="index" :style="trStyle(index)" @click="clickRow(index)">
          <td style="whiteSpace:'nowrap',overflow:'hidden',textOverflow:'ellipsis'" v-if="chartOption.showRank != false">
            <span :style="spanStyle(index)">{{ index + 1 }}</span>
          </td>
          <template v-for="(col, colindex) in item">
            <td :key="colindex" :style="tdStyle(colindex,item,col)" v-if="!col.hide">
              <a @click="fieldClick(col.field, col.value)" :style="linkStyle(col)" v-if="col.isBinding">{{ col.value }}</a>
              <a style="cursor:default;" v-else>{{ col.value }}</a>
            </td>
          </template>
        </tr>
      </tbody>
    </table>
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
      tableData: [],
      key: 1,
      animate: this.className,
      selectedRow: undefined
    };
  },
  watch: {
    width() {
      // this.initChart();
    },
    height() {
      // this.initChart();
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    //setInterval(this.scrollTable, 2000);
  },
  beforeDestroy() {
  },
  computed: {
    headTableStyle() {
      let style = {};
      style.width = "100%";
      if (this.chartOption.isthead == false) {
        style.display = "none";
      }
      return style;
    },
    topheadStyle() {
      let style = {
        height: this.chartOption.theadheight + "px",
        backgroundColor: this.chartOption.theadbackgroundcolor,
        fontSize: this.chartOption.theadfontsize + "px",
        fontFamily: this.chartOption.theadfontfamily,
        color: this.chartOption.theadfontcolor,
        textAlign: this.chartOption.isLeft,
        verticalAlign: "middle"
      };
      if (this.chartOption.showRank == false) {
        style.display = "none";
      }
      return style;
    },
    theadStyle() {
      return function(item) {
        return {
          width: item.width + "%",
          height: this.chartOption.theadheight + "px",
          backgroundColor: this.chartOption.theadbackgroundcolor,
          fontSize: this.chartOption.theadfontsize + "px",
          fontFamily: this.chartOption.theadfontfamily,
          color: this.chartOption.theadfontcolor,
          textAlign: this.chartOption.isLeft,
          verticalAlign: "middle"
        };
      };
    },

    trStyle() {
      return function(index) {
        let style = {
          width: "100%",
          height: this.chartOption.tbodyheight + "px",
          tableLayout: "fixed",
          fontSize: this.chartOption.tbodyfontsize + "px",
          fontFamily: this.chartOption.tbodyfontfamily,
          color: this.chartOption.tbodyfontcolor,
          textAlign: this.chartOption.isLeft,
          verticalAlign: "middle",
          display: "inline-table"
        };
        //判断奇偶行设置不同颜色
        if (index % 2 != 0) {
          style.backgroundColor = this.chartOption.oddcolor;
        } else {
          style.backgroundColor = this.chartOption.evencolor;
        }
        if (
          index == this.selectedRow &&
          this.chartOption.selectedColor != null
        ) {
          style.backgroundColor = this.chartOption.selectedColor;
        }

        return style;
      };
    },
    spanStyle() {
      return function(index) {
        let topcolor = ["#ed405d", "#f78c44", "#49bcf7", "#878787"];
        let size = parseInt(this.chartOption.tbodyfontsize) + 6;
        let style = {
          width: size + "px",
          height: size + "px",
          borderRadius: "3px",
          color: "#fff",
          lineHeight: size + "px",
          textAlign: "ctenter",
          display: "inline-block"
        };
        if (index == 0) {
          style.backgroundColor = topcolor[0];
        } else if (index == 1) {
          style.backgroundColor = topcolor[1];
        } else if (index == 2) {
          style.backgroundColor = topcolor[2];
        } else {
          style.backgroundColor = topcolor[3];
        }
        return style;
      };
    },
    tbodyStyle() {
      let style = {
        height:
          this.height.replace("px", "") - this.chartOption.theadheight + "px",
        width: this.width,
        display: "block"
      };
      return style;
    }
  },
  methods: {
    setChartVal(result) {
      this.getTableData(result);
      this.key = Math.random();
    },

    tdStyle(colindex,row,col) {
      let cols = this.chartOption.cols;
      let style = {};
      if (cols.length > 0) {
        style.width = cols[colindex].width + "%";
        style.color = cols[colindex].textColor
      }
      //自动换行
      if (this.chartOption.wordWrap == "breakAll") {
        style.wordWrap = "break-word";
        style.wordBreak = "normal";
      }
      //自动隐藏
      else {
        style.whiteSpace = "nowrap";
        style.overflow = "hidden";
        style.textOverflow = "ellipsis";
      }
      if (cols.length > 0) {
        if(cols[colindex].isLeft){
          style.textAlign=cols[colindex].isLeft
        }
        style.width = cols[colindex].width + "%";
        style.color = cols[colindex].textColor
        if(this.chartOption.tableColumsStyle&&this.chartOption.tableColumsStyle.length>0){
          let filterColoum=this.chartOption.tableColumsStyle.filter(rw=>rw.col==col.field)
          for(let i= 0;i<filterColoum.length;i++){
            let rw=filterColoum[i]
            let colValue=row.find(rs=>rs.field==rw.field)
            
            if(colValue&&colValue.value==rw.value){
              if(rw.textColor){
                style.color=rw.textColor
              }
              if(rw.textBgColor){
                style.background=rw.textBgColor
              }
            }
          }
        }
      }
      return style;
    },
    getTableData(staticDataValue) {
      let tableData = [];
      //构建表格数据数组，判断是否满足动态列绑定列值，不满足设置空
      if (this.chartOption.cols.length > 0) {
        for (let staticValue of staticDataValue) {
          let trArr = [];

          for (let col of this.chartOption.cols) {
            let flag = false;
            for (let item in staticValue) {
              if (item == col.field) {
                let staticValueObj = {};
                staticValueObj.field = item;
                staticValueObj.value = staticValue[item];
                if (this.isBindingChart(item)) {
                  staticValueObj.isBinding = true;
                } else {
                  staticValueObj.isBinding = false;
                }
                if(col.hide!==undefined){
                  staticValueObj.hide=col.hide
                }else{
                  staticValueObj.hide=false
                }
                //console.log(staticValueObj);
                trArr.push(staticValueObj);
                flag = true;
                break;
              }
            }
            if (flag == false) {
              let staticValueObj = {};
              staticValueObj.field = col.field;
              staticValueObj.value = "";
              trArr.push(staticValueObj);
            }
          }

          tableData.push(trArr);
        }
      }
      this.tableData = tableData;
    },
    visible: function() {
      if (
        this.height.replace("px", "") <
        this.chartOption.theadheight +
        this.tableData.length * this.chartOption.tbodyheight
      ) {
        let style = this.$refs.tbody.style;
        style.overflowY = "scroll";
      }
    },
    invisible: function() {
      let style = this.$refs.tbody.style;
      style.overflowY = "hidden";
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
    fieldClick(field, val) {
      //console.log(field,val);
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
              drillDownChartOption.chartOption.requestParameters =[{"name":drillTag.paramName,"value":val}];
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


    },
  
    clickRow(index) {
      if (index == this.selectedRow) {
        this.selectedRow = undefined;
      } else {
        this.selectedRow = index;
      }
    }
  }
};
</script>
<style ang="scss" scoped></style>
