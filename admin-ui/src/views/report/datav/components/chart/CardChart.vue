<template>
  <div :class="[animate, 'cardView']" :style="{ height: height, width: width, background: bgType,borderRadius:borderRadius}" :id="chartOption.bindingDiv"
    @mouseenter="visible" @mouseleave="invisible" ref="text">
    <template v-for="(item, rowindex) in rows">
      <div :style="rowStyle" :key="rowindex">
        <!-- <div :style="cardTop"></div> -->
        <template v-for="(col, colindex) in item">
          <div :style="divStyle" :key="colindex">
            <div :style="headStyle">
              <img :src="col.head.headImg" :style="headImgStyle" />
              <img v-if="headImgShow" :src="tipSrc" :style="tipImgStyle(col)" />
              <div :style="tipTextStyle">
                <span>{{ col.head.tipText }}</span>
              </div>
            </div>

            <div :style="nameStyle">
              {{ col.name }}
            </div>
            <div :style="{
              display: 'flex',
              flexDirection: 'column',
              position: 'relative',
              height: chartOption.infoItem.height + '%'}">
              <template v-for="(info, infoindex) in col.infoItem">
                <div :style="itemStyle(col)" :key="infoindex">
                  <div :style="{
                    marginLeft: chartOption.infoItem.textMarginLeft + '%',
                    marginTop: chartOption.infoItem.textMarginTop + '%'}">
                    {{ info.label }}
                  </div>
                  <div :style="{
                    marginRight: '3%',
                    marginTop: chartOption.infoItem.textMarginTop + '%'}">
                    {{ info.value }}
                  </div>
                </div>
              </template>
            </div>
            <div :style="detailStyle">
              <template v-for="(detail, detailindex) in col.detailItem">
                <div :style="detailInfoStyle(col)" :key="detailindex" @click="fieldClick(col, detail)" class="detail">
                  <div :style="{
                    marginTop: chartOption.detailItem.textMarginTop + '%'}">
                    <a v-if="detail.address == null||detail.address==''">{{ detail.label }}</a>
                    <a v-else :href="detail.address" target="_blank">{{detail.label}}</a>
                  </div>
                </div>
              </template>
            </div>
          </div>
        </template>
      </div>
    </template>
  </div>
</template>

<script>
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
import {cardDataHandle} from '../../util/commonChartChange'
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
      rows: [],
      bgType: '',
      borderRadius:0,
      tipText: "", //提示框文字
      name: "", //名称
      cardheight: "", //每个卡片高度占比
      cardwidth: "", //每个卡片宽度占比
      tipSrc: require("../../image/tip.png"),
      animate: this.className
    };
  },
  watch: {
    width() { },
    height() { },
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
    rowStyle() {
      let style = {
        width: "100%",
        display: "flex",
        flexDirection: "row",
        height: "100%",
        marginTop: this.chartOption.marginTop + "%"
      };
      return style;
    },
    cardTop() {
      const style = { height: this.chartOption.marginTop + "%" };
      return style;
    },
    divStyle() {
      const style = {
        width: this.cardwidth + "%",
        height: "100%",
        position: "relative",
        marginLeft: this.chartOption.marginLeft + "%"
      };
      return style;
    },
    headStyle() {
      const style = {
        width: "100%",
        height: this.chartOption.head.height + "%",
        marginTop: this.chartOption.head.marginTop + "%",
        position: "relative"
      };
      return style;
    },
    headImgShow(){
      if(this.chartOption.head.tipImg.show=="block"){
        return true;
      }
      else{
        return false;
      }
    },
    headImgStyle() {
      const style = {
        width: this.chartOption.head.headImg.width + "%",
        height: this.chartOption.head.headImg.height + "%",
        marginLeft: this.chartOption.head.headImg.marginLeft + "%",
        position: "absolute"
      };
      return style;
    },
    tipTextStyle() {
      const style = {
        color: this.chartOption.head.tipText.fontColor,
        fontSize: this.chartOption.head.tipText.fontSize + "px",
        fontFamily: this.chartOption.head.tipText.fontFamily,
        fontWeight: this.chartOption.head.tipText.fontWeight,
        letterSpacing: this.chartOption.head.tipText.letterSpacing + "px",
        marginLeft: this.chartOption.head.tipText.marginLeft + "%",
        marginTop: this.chartOption.head.tipText.marginTop + "%",
        position: "absolute"
      };
      return style;
    },
    nameStyle() {
      const style = {
        color: this.chartOption.name.fontColor,
        fontSize: this.chartOption.name.fontSize + "px",
        fontFamily: this.chartOption.name.fontFamily,
        fontWeight: this.chartOption.name.fontWeight,
        letterSpacing: this.chartOption.name.letterSpacing + "px",
        marginLeft: this.chartOption.name.marginLeft + "%",
        marginTop: this.chartOption.name.marginTop + "%",
        marginBottom: this.chartOption.name.marginBottom + "%",
        position: "relative"
      };
      return style;
    },
    detailStyle() {
      const style = {
        display: "flex",
        flexDirection: "column",
        position: "relative",
        color: this.chartOption.detailItem.fontColor,
        fontSize: this.chartOption.detailItem.fontSize + "px",
        fontFamily: this.chartOption.detailItem.fontFamily,
        fontWeight: this.chartOption.detailItem.fontWeight,
        letterSpacing: this.chartOption.detailItem.letterSpacing + "px",
        marginLeft: "3%",
        width: "94%",
        height: this.chartOption.detailItem.height + "%"
      };

      return style;
    }
  },
  methods: {
    setChartVal(resData,rowGlobal) {
      let result=[]
      // let result=resData
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        result =cardDataHandle(rowGlobal,this.chartOption)?cardDataHandle(rowGlobal,this.chartOption):[]
      }else{
        result = this.chartOption.staticDataValue;
      }
      if (this.chartOption.bgType === 'color') {
        this.bgType = this.chartOption.bgTypeColor;
      } else {
        this.bgType = `url(${this.chartOption.bgTypeImg}) no-repeat center`
      }
      if(this.chartOption.borderRadius){
        this.borderRadius=this.chartOption.borderRadius+'px'
      }
      this.rows = this.getRows(result);
    },
    getRows(chartVal) {
      //根据列数计算数据的行数，并按顺序将数据构造成按指定行、列分隔的数组
      let results = [];
      //列数，判断列数不能大于数据数组的总长度
      let colsNum = chartVal.length;
      if (this.chartOption.numOfCol < chartVal.length) {
        colsNum = this.chartOption.numOfCol;
      }
      let col = 0; //卡片行数
      for (let i = 0; i < chartVal.length;) {
        let rows = [];
        for (let j = 0; j < colsNum && i < chartVal.length; j++) {
          //每行数据
          rows.push(chartVal[i]);
          i++;
        }
        results.push(rows);
        col++;
      }
      //每个卡片宽度占比
      this.cardwidth =
        (100 - this.chartOption.marginLeft * this.chartOption.numOfCol) /
        this.chartOption.numOfCol;
      //每个卡片高度占比
      this.cardheight = (100 - col * this.chartOption.marginTop) / col;

      return results;
    },
    visible: function () {

      let style = document.getElementById(this.chartOption.bindingDiv).style;
      style.overflowY = 'auto'

    },
    invisible: function () {
      let style = document.getElementById(this.chartOption.bindingDiv).style;
      style.overflowY = 'hidden'

    },
    //信息航样式
    itemStyle(card) {
      //计算平均每行高度
      const itemRows = card.infoItem.length;

      const height =
        (100 - this.chartOption.infoItem.marginTop * itemRows) / itemRows;

      const style = {
        display: "flex",
        justifyContent: "space-between",
        color: this.chartOption.infoItem.fontColor,
        fontSize: this.chartOption.infoItem.fontSize + "px",
        fontFamily: this.chartOption.infoItem.fontFamily,
        fontWeight: this.chartOption.infoItem.fontWeight,
        letterSpacing: this.chartOption.infoItem.letterSpacing + "px",
        marginLeft: "3%",
        marginTop: this.chartOption.infoItem.marginTop + "%",
        width: "94%",
        height: height + "%"
      };
      if (this.chartOption.infoItem.background == "color") {
        style.backgroundColor = this.chartOption.infoItem.backgroundColor;
      } else {
        style.backgroundImage = `url(${this.chartOption.infoItem.backgroundImg
          }) `;
        style.backgroundSize = "100% 100%";
        style.backgroundRepeat = "no-repeat";
      }
      return style;
    },
    detailInfoStyle(card) {
      //计算平均每行高度
      const itemRows = card.detailItem.length;

      const height =
        (100 - this.chartOption.detailItem.marginTop * itemRows) / itemRows;

      const style = {
        textAlign: "center",
        marginTop: this.chartOption.detailItem.marginTop + "%",
        height: height + "%"
      };
      if (this.chartOption.detailItem.background == "color") {
        style.backgroundColor = this.chartOption.detailItem.backgroundColor;
      } else {
        style.backgroundImage = `url(${this.chartOption.detailItem.backgroundImg
          })`;
        style.backgroundSize = "100% 100%";
        style.backgroundRepeat = "no-repeat";
      }
      return style;
    },
    tipImgStyle(col) {
      const style = {
        width: this.chartOption.head.tipImg.width + "%",
        height: this.chartOption.head.tipImg.height + "%",
        marginLeft: this.chartOption.head.tipImg.marginLeft + "%",
        marginTop: this.chartOption.head.tipImg.marginTop + "%",
        position: "absolute"
      };
      //判断是否和提示框文字同步显示
      if (this.chartOption.head.tipImg.sync) {
        if (col.head.tipText == "") {
          style.display = "none";
        } else {
          style.display = "show";
        }
      } else {
        style.display = this.chartOption.head.tipImg.show;
      }

      return style;
    },

    fieldClick(col, detail) {
      this.$emit("onItemClick",{id:col.id,detail:detail});
    },

  }
};
</script>
<style ang="scss" scoped>
.cardView{
  background-size: 100% 100%;
  overflow-y: hidden;
}
.detail:hover {
  cursor: default;
}
</style>