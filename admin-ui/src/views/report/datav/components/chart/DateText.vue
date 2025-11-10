<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <div :style="normalTextStyle">
      {{ nowTime }}
    </div>
      
  </div>
  
</template>

<script>

require("echarts/theme/macarons"); // echarts theme
import resize from '@/views/dashboard/mixins/resize'
import '../../animate/animate.css'
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [resize,dataChart],
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
    drawingList:{
      type: Array
    }
  },
  data() {
    return {
      nowTime:null,   //存放时分秒变量
      dateTimer: null,           //定义一个定时器的变量
      currentTime: new Date(),       // 获取当前时间
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
    }
  },

  computed:{
    normalTextStyle(){
      const style = {color:this.chartOption.fontColor,fontSize:this.chartOption.fontSize + "px",fontFamily:this.chartOption.fontFamily,
        backgroundColor: this.chartOption.backgroundColor,height: this.height, width: this.width,
        fontWeight:this.chartOption.fontWeight,letterSpacing:this.chartOption.letterSpacing + "px",textAlign:this.chartOption.textAlign}
      return style
    }
    
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    this.dateTimer = setInterval(this.getTime, 1000);
  },
  beforeDestroy() {
    if (this.dateTimer) {
      clearInterval(this.dateTimer); // 在Vue实例销毁前，清除定时器
    }
  },
  methods: {
    setChartVal(result)  {
      let dataOption = this.dataOption;

      //设置自定义格式值
      if(dataOption.isCustom == false){
          this.chartOption.customFormat = dataOption.dateFormat;
      }

      this.nowTime = this.chartOption.customFormat.replace("yyyy",this.dateOption().year)
		   .replace("MM",this.dateOption().month)
		   .replace("dd",this.dateOption().date)
		   .replace("HH",this.dateOption().Hours)
		   .replace("hh",this.dateOption().hours)
		   .replace("mm",this.dateOption().mimu)
		   .replace("ss",this.dateOption().sec)
		   .replace("SSS",this.dateOption().mill)
       .replace("day",this.dateOption().day);
    },
    dateOption(){
      //获取时间格式
      const d = new Date();
      let year = d.getFullYear();
      let month = d.getMonth() + 1;
      let date = d.getDate();
      let Hours= d.getHours();
      let hours = d.getHours();
      let minute = d.getMinutes();
      let second = d.getSeconds();
      let mill = d.getMilliseconds();
      let day = d.getDay();
      if(hours>12) {
        hours -= 12;
      }
      
      month=check(month);
      date=check(date);
      Hours=check(Hours);
      hours=check(hours);
      minute=check(minute);
      second=check(second);
      mill=check(mill);
      function check(i){
          const num = (i<10)?("0"+i) : i;
          return num;
      }
      switch(day){
          case 0:day="日";break;
          case 1:day="一";break;
          case 2:day="二";break;
          case 3:day="三";break;
          case 4:day="四";break;
          case 5:day="五";break;
          case 6:day="六";break;
      }
      return {
              year:year,
              month:month,
              date:date,
              day:"星期"+day,
              Hours:Hours,
              hours:hours,
              mimu:minute,
              sec:second,
              mill:mill
          };
    },
    getTime(){
      //根据时间格式指定获取实时时间
      this.nowTime = this.chartOption.customFormat.replace("yyyy",this.dateOption().year)
		   .replace("MM",this.dateOption().month)
		   .replace("dd",this.dateOption().date)
		   .replace("HH",this.dateOption().Hours)
		   .replace("hh",this.dateOption().hours)
		   .replace("mm",this.dateOption().mimu)
		   .replace("ss",this.dateOption().sec)
		   .replace("SSS",this.dateOption().mill)
       .replace("day",this.dateOption().day);
       
    },
   
  }
};
</script>
<style ang="scss" scoped>

</style>
