<template>
  <div>
    <el-dialog title="Cron表达式生成器" :visible.sync="timesVisible" width="600" :destroy-on-close="true" :close-on-click-modal="false">
      <div class="times_con">
        <div class="label">
          <div class="label_line"></div>
          <div class="label_text">重复方式</div>
        </div>
        <div class="value_con">
          <el-radio-group v-model="timeType" @input="timeTypeChange">
            <el-radio :label="0" style="width: 200px; margin-right: 0">按时间</el-radio>
            <el-radio :label="1" style="width: 200px; margin-right: 0">按间隔</el-radio>
          </el-radio-group>
        </div>
        <div v-if="timeType == 0">
          <div class="label">
            <div class="label_line"></div>
            <div class="label_text">重复</div>
          </div>
          <div class="value_con">
            <el-radio-group v-model="repeatVal" @input="repeatValChange">
              <el-radio :label="0" style="width: 150px; margin-right: 0">只执行一次</el-radio>
              <el-radio :label="1" style="width: 150px; margin-right: 0">每天</el-radio>
              <el-radio :label="2" style="width: 150px; margin-right: 0">每月</el-radio>
              <el-radio :label="3" style="width: 150px; margin-right: 0">自定义</el-radio>
            </el-radio-group>
          </div>
          <div class="week_con" v-if="isShowWeekLis">
            <div class="week_li" v-for="item in weekArr" :key="item.val" @click="setWeekVal(item)">
              <div class="li_left">{{ item.text }}</div>
              <div class="li_right" v-if="weekVal && weekVal.includes(item.val)">
                <i class="el-icon-check"></i>
              </div>
            </div>
          </div>
          <div class="month_con" v-if="repeatVal == 2">
            <div class="month_li" v-for="item in dayList" :key="item" @click="seDayVal(item)">
              <div class="li_left">{{ item }}</div>
              <div class="li_right" v-if="dayVal && dayVal.includes(item)">
                <i class="el-icon-check"></i>
              </div>
            </div>
          </div>
          <div class="label">
            <div class="label_line"></div>
            <div class="label_text">指定时间点</div>
          </div>
          <div class="value_con" v-if="repeatVal!=0">
            <!-- <el-time-select v-model="timeValue" :picker-options="{ start: '00:00', step: '00:01', end: '23:59' }" placeholder="选择时间">
            </el-time-select> -->
            <el-time-picker v-model="timeValue" :picker-options="{ selectableRange: '00:00:00 - 23:59:59'}" placeholder="选择时间" value-format="HH:mm:ss"></el-time-picker>
          </div>
          <div class="value_con" v-if="repeatVal==0">
            <el-date-picker v-model="datetimeValue" type="datetime" placeholder="选择日期时间" @change="changeStartTime"></el-date-picker>
          </div>
        </div>
        <div v-if="timeType == 1">
          <div class="label">
            <div class="label_line"></div>
            <div class="label_text">间隔类型</div>
          </div>
          <div class="value_con">
            <el-radio-group v-model="intervalType" @input="intervalTypeChange">
              <el-radio :label="0" style="width: 200px; margin-right: 0">秒</el-radio>
              <el-radio :label="1" style="width: 200px; margin-right: 0">分</el-radio>
              <el-radio :label="2" style="width: 200px; margin-right: 0">时</el-radio>
            </el-radio-group>
          </div>
          <div class="label">
            <div class="label_line"></div>
            <div class="label_text">间隔数</div>
          </div>
          <div class="value_con">
            <!-- <el-input-number v-model="intervalNum" :precision="0" :min="1" :step="1" :max="intervalMax"></el-input-number> -->
            <div class="interval_value_con">
              <div class="interval_value_li" v-for="ite in activeIntervalList" :key="'interval'+ite" @click="setIntervalValue(ite)" :class="{ active_li: ite==intervalNum }">
                <div class="li_left">{{ite}}</div>
                <div class="li_right" v-if="ite==intervalNum">
                  <i class="el-icon-check"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="demo-drawer__footer" style="text-align: right; margin-top: 40px">
        <el-button @click="timesVisible = false">取 消</el-button>
        <el-button type="primary" @click="finishTimeChoice">确定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import dayjs from "dayjs";
export default {
  name: "rulesCronTime",

  data() {
    return {
      //定时触发相关参数
      dayList: [], //日期
      dayVal: [], //日期值
      weekArr: [
        {
          text: "周日",
          val: 1,
        },{
          text: "周一",
          val: 2,
        },
        {
          text: "周二",
          val: 3,
        },
        {
          text: "周三",
          val: 4,
        },
        {
          text: "周四",
          val: 5,
        },
        {
          text: "周五",
          val: 6,
        },
        {
          text: "周六",
          val: 7,
        }
      ], //星期
      isShowWeekLis: false, //是否显示星期列表
      weekVal: [], //星期值
      repeatVal: 0, //定时器重复属性
      timeValue: "", //时间
      datetimeValue:'',//指定日期时间点
      timesVisible: false,
      timeType: 0, //时间表达式重复方式
      intervalType: 0, //间隔类型
      intervalNum: 1, //间隔数
      intervalMax: null, //间隔的最大值
      TimerCron: "", //定时表达式生成器生成结果
      //定时触发相关参数
      //时分秒对应间隔数组
      secondsList:[1,2,5,10,15,20,30],
      minutesList:[1,2,5,10,15,20,30],
      hoursList:[1,2,3,4,6,12],
    };
  },
  computed: {
    activeIntervalList() {
      if (this.intervalType == 0) {
        return this.secondsList;
      } else if (this.intervalType == 1) {
        return this.minutesList;
      }else if (this.intervalType == 2) {
        return this.hoursList;
      }
    },
  },
  mounted() {
    this.dayList = [];
    for (let i = 0; i < 31; i++) {
      this.dayList.push(i + 1);
    }
  },

  methods: {
    setIntervalValue(val){
      this.intervalNum=val
    },
    changeStartTime(val){
      //只执行一次切换时间
      if(dayjs(val).isBefore(dayjs())){
        this.datetimeValue=''
        this.$message.warning("所选时间需大于当前时间");
      }
    },
    seDayVal(val) {
      //设置选中的日的值
      // this.dayVal=val
      if (this.dayVal.includes(val)) {
        let ix = this.dayVal.indexOf(val);
        this.dayVal.splice(ix, 1);
      } else {
        this.dayVal.push(val);
      }
    },
    /** cron表达式按钮操作 */
    handleShowCron(val) {
      this.TimerCron = val;
      if (this.TimerCron) {
        let arr = this.TimerCron.split(" ");
        // console.log(arr, "arr");
        if (this.TimerCron.indexOf("/") > -1) {
          this.timeType = 1;
          if (arr[0].indexOf("/") > -1) {
            this.intervalType = 0;
            this.intervalMax = 59;
            let numarr = arr[0].split("/");
            this.intervalNum = numarr[1];
          } else if (arr[1].indexOf("/") > -1) {
            this.intervalType = 1;
            this.intervalMax = 59;
            let numarr = arr[1].split("/");
            this.intervalNum = numarr[1];
          } else if (arr[1].indexOf("/") > -1) {
            this.intervalType = 2;
            this.intervalMax = 23;
            let numarr = arr[2].split("/");
            this.intervalNum = numarr[1];
          }
        } else {
          this.timeType = 0;
          let h = arr[2].length == 2 ? arr[2] : "0" + arr[2];
          let m = arr[1].length == 2 ? arr[1] : "0" + arr[1];
          let s = arr[0].length == 2 ? arr[0] : "0" + arr[0];
          this.timeValue = h + ":" + m+ ":" + s;
          if (arr.length == 7) {
            this.repeatVal = 0;
            this.timeValue=''
            let dateStr=arr[6]+'-'+arr[4]+'-'+arr[3]+' '+arr[2]+':'+arr[1]+':'+arr[0]
            this.datetimeValue=dayjs(dateStr).format('YYYY-MM-DD HH:mm:ss')
            this.isShowWeekLis = false;
          } else if (arr.length == 6 && arr[3] == "*"&& arr[5] == "?") {
            this.repeatVal = 1;
            this.isShowWeekLis = false;
          } else if (arr.length == 6 && arr[5] == "?" && arr[4] == "*") {
            this.repeatVal = 2;
            let dayVal = arr[3].split(",");
            let arr2 = [];
            dayVal.map((row) => {
              arr2.push(Number(row));
            });
            this.dayVal = JSON.parse(JSON.stringify(arr2));
            this.$forceUpdate()
          } else if (arr.length == 6 && arr[3] == "?"){
            this.repeatVal = 3;
            this.weekVal = arr[5].split(",");
            let arr2 = [];
            this.weekVal.map((row) => {
              arr2.push(Number(row));
            });
            this.weekVal = JSON.parse(JSON.stringify(arr2));
            // console.log(this.weekVal, "this.weekVal");
            this.isShowWeekLis = true;
          }
        }
      }
      this.timesVisible = true;
    },
    timeTypeChange() {
      if (this.timeType == 0) {
        this.timeValue = null;
      }
    },
    intervalTypeChange() {
      //间隔类型
      if (this.intervalType == 0) {
        if(this.secondsList.includes(this.intervalNum)){}else{
          this.intervalNum = 1;
        }
      } else if (this.intervalType == 1) {
        if(this.minutesList.includes(this.intervalNum)){}else{
          this.intervalNum = 1;
        }
      } else if (this.intervalType == 2) {
        if(this.hoursList.includes(this.intervalNum)){}else{
          this.intervalNum = 1;
        }
      }
      
    },
    repeatValChange(val) {
      if (val == 3) {
        this.isShowWeekLis = true;
      } else {
        this.isShowWeekLis = false;
      }
    },
    setWeekVal(item) {
      //设置星期值
      let val = item.val;
      if (this.weekVal.includes(val)) {
        let ix = this.weekVal.indexOf(val);
        this.weekVal.splice(ix, 1);
      } else {
        this.weekVal.push(val);
      }
    },
    finishTimeChoice() {
      //生成定时触发表达式
      if (this.timeType == 0) {
        this.timeChioce();
      } else if (this.timeType == 1) {
        this.timeChioce2();
      }
      this.$emit("finishTimeChoice", this.TimerCron);
    },
    timeChioce2() {
      let str = "";

      if (this.intervalType == 0) {
        str = "0/" + this.intervalNum + " * * * * ?";
      } else if (this.intervalType == 1) {
        str = "0 0/" + this.intervalNum + " * * * ?";
      } else if (this.intervalType == 2) {
        str = "0 0 0/" + this.intervalNum + " * * ?";
      }
      // console.log("最后表达式", str);
      this.TimerCron = str;
      this.timesVisible = false;
    },
    timeChioce() {
      let str = "";
      console.log(this.timeValue,'this.timeValuethis.timeValue');
      let arr =[]
      if(this.timeValue){
        arr = this.timeValue.split(":");
      }else{
        arr = [0,0,0]
      }
      
      if (this.repeatVal == 0) {
        // let nowDate = new Date();
        // var hour = nowDate.getHours() + 1;
        // var minutes = nowDate.getMinutes();
        // if(Number(arr[0]) < hour ||(Number(arr[0]) == hour && Number(arr[1]) < minutes)) {
        //   nowDate.setDate(nowDate.getDate() + 1);
        // }
        if(this.datetimeValue){}else{
          this.datetimeValue=new Date()
        }
        let second = dayjs(this.datetimeValue).second();
        let minuttes = dayjs(this.datetimeValue).minute();
        let hour = dayjs(this.datetimeValue).hour();
        let day = dayjs(this.datetimeValue).date();
        let mon = dayjs(this.datetimeValue).month() + 1;
        let year = dayjs(this.datetimeValue).year();

        str =second +" " +minuttes +" " +hour +" " +day +" " +mon +" ? " +year;
      } else if (this.repeatVal == 1) {
        str = Number(arr[2]) + " " + Number(arr[1]) + " " + Number(arr[0]) + " * * ?";
      } else if (this.repeatVal == 2) {
        if (!this.dayVal) {
          this.$message.warning("请勿选择重复日");
          return;
        }
        let dayStr = this.dayVal.join(",");
        str = Number(arr[2]) + " " + Number(arr[1]) + " " + Number(arr[0]) +" "+ dayStr +" * ?";
      } else if (this.repeatVal == 3) {
        if (this.weekVal.length == 0) {
          this.$message.warning("请勿选择重复周期");
          return;
        }
        let weekStr = this.weekVal.join(",");
        str =Number(arr[2]) + " " + Number(arr[1]) + " " + Number(arr[0]) + " ? * " + weekStr;
      }
      this.TimerCron = str;
      this.timesVisible = false;
    },
  },
};
</script>
<style lang="less" scoped>
.times_con {
  margin-top: -20px;
  font-size: 16px;
  .label {
    display: flex;
    align-items: center;
    color: #333;
    width: 100%;
    .label_line {
      margin-right: 10px;
      width: 4px;
      height: 12px;
      border: 4px;
      background: #3572ff;
    }
    .label_text {
      font-size: 16px;
    }
  }
  .value_con {
    width: 100%;
    margin-top: 20px;
    margin-bottom: 50px;
  }
  .week_con {
    display: flex;
    align-items: center;
    margin-right: -12px;
    margin-top: -10px;
    .week_li {
      width: 120px;
      height: 54px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #f8f8f8;
      border-radius: 4px;
      margin-right: 12px;
      padding: 0 10px;
      box-sizing: border-box;
      .li_right {
        color: #2371ff;
      }
    }
  }
  .month_con{
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    margin-right: -10px;
    .month_li{
      width: 70px;
      height: 40px;
      display: flex;
      justify-content: center;
      align-items: center;
      background: #f8f8f8;
      padding: 0 10px;
      box-sizing: border-box;
      margin-right: 10px;
      margin-bottom: 10px;
      .li_right {
        color: #2371ff;
        margin-left: 15px;
      }
    }
  }
}
.interval_value_con{
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  margin-right: -10px;
    
  .interval_value_li{
    width: 70px;
    height: 40px;
    display: flex;
    justify-content: center;
    align-items: center;
    background: #f8f8f8;
    padding: 0 10px;
    box-sizing: border-box;
    margin-right: 10px;
    border-radius: 5px;
    margin-bottom: 10px;
    &.active_li{
      border: 1px solid #2371ff;
    }
    .li_left{
      white-space:nowrap;
    }
    .li_right {
      color: #2371ff;
      margin-left: 15px;
    }
  }
}
</style>
