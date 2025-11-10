<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <table :style="headTableStyle">
      <thead>
        <tr>
          <template v-for="(item, index) in this.chartOption.cols">
            <th :key="index" :style="theadStyle(item)" v-if="!item.hide||item.hide===undefined">{{ item.title }}</th>
          </template>
        </tr>
      </thead>
    </table>

    <vue-seamless-scroll :key="key" :data="tableData" :style="scrollTableStyle()" :class-option="defaultOption">
      <table :style="{width: width,tableLayout: 'fixed',borderCollapse: 'collapse',borderSpacing: 0,}">
        <tbody :style="{ width: width }">
          <!-- <tr  v-for="(item,index) in  dataOption.staticDataValue" :key="index" :style="{width: width }">
            
            <td v-for="(tdValue,dindex) in item" :key="tdValue" :style="tbodyStyle(tdValue,dindex,index)" >
              {{getTdValue(item,dindex,index)}}
                
            </td>
          </tr>           -->
          <tr v-for="(item, index) in tableData" :key="index" :style="trStyle(index, item)" @click="clickRow(index)">
            <template v-for="(col, colindex) in item">
              <td :key="colindex" :style="tdStyle(colindex,item,col)" v-if="!col.hide">
                <a @click="fieldClick(col.field, col.value)" :style="linkStyle(col)" v-if="col.isBinding">{{ col.value }}</a>
                <a style="cursor:default;" v-else>{{ col.value }}</a>
              </td>
            </template>
            
          </tr>
        </tbody>
      </table>
    </vue-seamless-scroll>
  </div>
</template>

<script>
import VueEvent from "../../VueEvent";
import "../../animate/animate.css";
import vueSeamlessScroll from "vue-seamless-scroll";
import { getLinkChart } from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'

export default {
  components: {
    vueSeamlessScroll
  },
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
    },
    height() {
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed: {
    defaultOption() {
      return {
        step: this.chartOption.velocity, // 数值越大速度滚动越快
        limitMoveNum: this.chartOption.staticDataValue.length, // 开始无缝滚动的数据量 this.dataList.length
        hoverStop: true, // 是否开启鼠标悬停stop
        direction: parseInt(this.chartOption.direction), // 0向下 1向上 2向左 3向右
        openWatch: true, // 开启数据实时监控刷新dom
        singleWidth: 0, // 单步运动停止的宽度(默认值0是无缝不停止的滚动) direction => 2/3
        waitTime: this.chartOption.waitTime
          ? parseFloat(this.chartOption.waitTime)
          : 0, // 单步运动停止的时间(默认值1000ms)
        singleHeight: this.chartOption.singleHeight
          ? parseFloat(this.chartOption.singleHeight)
          : 0,
        hoverStop: this.chartOption.hoverStop
          ? this.chartOption.hoverStop
          : false
      };
    },
    cellStyle() {
      return {
        color: this.chartOption.tbodyColor
      };
    },
    headerStyle() {
      return {
        color: this.chartOption.theadColor
      };
    },
    headTableStyle() {
      let style = { tableLayout: "fixed" };
      style.width = "100%";
      if (this.chartOption.isthead == false) {
        style.display = "none";
      }
      style.borderSpacing= 1
      return style;
    },
    theadStyle() {
      return function(item) {
        let textAlignval=this.chartOption.isLeft
        if(item.isLeft){
          textAlignval=item.isLeft
        }
        return {
          width: item.width + "%",
          height: this.chartOption.theadheight + "px",
          backgroundColor: this.chartOption.theadbackgroundcolor,
          fontSize: this.chartOption.theadfontsize + "px",
          fontFamily: this.chartOption.theadfontfamily,
          color: this.chartOption.theadfontcolor,
          textAlign: textAlignval,
          verticalAlign: "middle"
        };
      };
    },
    tbodyStyle() {
      return function(item, dindex, index) {
        let theadList = this.chartOption.cols;

        let td = theadList.filter(function(item) {
          return dindex == item.field;
        })[0];

        let style = {
          height: this.chartOption.tbodyheight + "px",
          fontSize: this.chartOption.tbodyfontsize + "px",
          fontFamily: this.chartOption.tbodyfontfamily,
          color: this.chartOption.tbodyfontcolor,
          textAlign: this.chartOption.isLeft,
          verticalAlign: "middle"
        };

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
        //判断奇偶行

        if (index % 2 != 0) {
          style.backgroundColor = this.chartOption.oddcolor;
        } else {
          style.backgroundColor = this.chartOption.evencolor;
        }
        if (td == null) {
          style.display = "none";
        } else {
          style.width = td.width + "%";
        }
        return style;
      };
    },

    trStyle() {
      return function(index) {
        let style = {
          width: "100%",
          height: this.chartOption.tbodyheight + "px",
          display: "table",
          tableLayout: "fixed",
          fontSize: this.chartOption.tbodyfontsize + "px",
          fontFamily: this.chartOption.tbodyfontfamily,
          color: this.chartOption.tbodyfontcolor,
          textAlign: this.chartOption.isLeft,
          verticalAlign: "middle"
        };
        //判断奇偶行设置不同颜色
        if (index % 2 == 0) {
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
    }
  },
  methods: {
    setChartVal(result) {
      this.getTableData(result);
      this.key = Math.random();
    },
    scrollTableStyle() {
      return {
        height:
          this.height.replace("px", "") - this.chartOption.theadheight + "px",
        overflow: "hidden",
        width: "100%"
      };
    },
    getTdValue(item, dindex, index) {
      let theadList = this.chartOption.cols;

      let td = theadList.filter(function(item) {
        return item.field == dindex;
      })[0];

      if (td != null) {
        return item[td.field];
      } else {
        return null;
      }
    },
    tdStyle(colindex,row,col) {
      let cols = this.chartOption.cols;
      let style = {};
      //自动换行
      if (this.chartOption.wordWrap == "breakAll") {
        style = {
          wordWrap: "break-word",
          tableLayout: "fixed",
          wordBreak: "normal"
        };
      }
      //超出隐藏
      else {
        style = {
          whiteSpace: "nowrap",
          overflow: "hidden",
          textOverflow: "ellipsis"
        };
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
                // console.log(staticValueObj);
                trArr.push(staticValueObj);
                flag = true;
                break;
              }
            }
            if (flag == false) {
              let staticValueObj = {};
              staticValueObj.field = col.field;
              staticValueObj.value = "";
              if(col.hide!==undefined){
                staticValueObj.hide=col.hide
              }else{
                staticValueObj.hide=false
              }
              trArr.push(staticValueObj);
            }
          }

          tableData.push(trArr);
        }
      }
      this.tableData = tableData;
    },
    scrollTable() {
      var tmp = this.tableData.shift(); //删掉第一行数据并返回
      this.tableData.push(tmp); //将第一行数据存到末尾
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
 
    tableRowClassName({ row, rowIndex }) {
      row.index = rowIndex;
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
