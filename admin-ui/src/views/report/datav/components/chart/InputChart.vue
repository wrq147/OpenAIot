<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv">
    <div class="demo-input-suffix" ref="text" :style="{display:returnDisplay}" v-show="isDraw||chartOption.isHideInput==undefined||!chartOption.isHideInput">
      
      <label class="el-form-item__label imglable" for="" :style="normalTextStyle">
        <img class="label_icon" :src="chartOption.label.icon" alt="" v-if="chartOption.label.icon" :style="{'--size':chartOption.label.fontSize + 'px'}">
        <span>{{ chartOption.label.context }}</span>
      </label>
      <div class="el-form-item__content">

        <el-input placeholder="请输入" class="eleInput" v-model="input" :style="inputStyle" :id="inputId"
          :name="chartOption.input.name" :type="chartOption.input.type"
          :maxlength="chartOption.input.maxlength" @change="renderChart" @input="onInput" :disabled="chartOption.isDisableInput"></el-input>
      </div>

    </div>
  </div>
</template>

<script>
import '../../animate/animate.css'
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      input: '',
      animate: this.className
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
    },
    $route: {
      handler: function (route) {
        // console.log(route,);
        if(route.query){
          let pars = route.query
          if(this.chartOption.params&&pars[this.chartOption.params]){
            this.input=pars[this.chartOption.params]
          }
        }
       
      },
      immediate: true,
    },
  },
  mounted() {
    
    if(this.chartOption.params){
      let pars = this.$route.query;
      if(pars[this.chartOption.params]){
        this.input=pars[this.chartOption.params]
      }
      
    }
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.input;
      };
    }
  },
  beforeDestroy() {
  },
  computed: {
    normalTextStyle() {
      const style = { color: this.chartOption.label.fontColor, fontSize: this.chartOption.label.fontSize + "px", fontFamily: this.chartOption.label.fontFamily }
      return style
    },
    inputId() {
      return 'input' + this.chartOption.bindingDiv
    },
    inputStyle() {
      const style = {
        color: '#fff', borderRadius: '5px', borderColor: 'rgba(255,255,255,0.8)', backgroundColor: 'rgba(0,0,0,0)',
        fontSize: this.chartOption.label.fontSize + "px", fontFamily: this.chartOption.label.fontFamily, width: this.chartOption.width + "px",'--height':this.chartOption.height + "px",
      }

      return style
    },
    returnDisplay(){
      if(this.chartOption.label.position){
        if(this.chartOption.label.position=='left'){
          return 'flex'
        }else if(this.chartOption.label.position=='top'){
          return 'block'
        }
      }else{
        return 'flex'
      }
    }
  },
  methods: {
    setChartVal(result) {
      if(this.chartOption.params){
        let pars = this.$route.query;
        if(pars[this.chartOption.params]){
          this.input=pars[this.chartOption.params]
        }
        
      }
    },
    renderChart(val) {
      this.$emit("onChange", val);
      //更新数据源
      if (this.chartOption.dataList != null) {
        this.chartOption.dataList.forEach(element => {
          this.refreshData(element)
        });
      }
    },
    onInput(val) {
      this.$emit("onInput", val);
    }
  }
};
</script>

<style lang="scss" scoped>
.demo-input-suffix {
  display: flex;
  float: left;
}
.imglable{
  display: flex;
  justify-content: flex-start;
  align-items: center;
  .label_icon{
    width: var(--size);
    height: var(--size);
    margin-right: 5px;
  }
}
::v-deep .eleInput .el-input--medium .el-input__inner {
  height: 36px;
  line-height: 36px;
  background-color: #ffffff;
}

::v-deep .eleInput .el-input__inner {
  background-color: #ffffff !important;
  color: #000 !important;
  height: var(--height);
  line-height: var(--height);
}
</style>
