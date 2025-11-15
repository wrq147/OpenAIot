<template>
  <div>
    <div v-if="deviceInfosBasic.Online != 0">
      <el-row>
        <div style="display: flex;justify-content: space-between;align-items: center;">
          <el-input
            placeholder="请输入名称"
            v-model="searchVal"
            style="width: 22%"
            @change="filterCodeVal"
          >
            <i slot="suffix" class="el-input__icon el-icon-search"></i>
          </el-input>
          <div style="display: flex;justify-content: flex-end;align-items: center;">
            <div v-if="deviceStorageConfig" class="online_history" @click="setOnlineOld"><i class="zhongtaiiconfont zhongtai-icon-lizaixianjilu"></i>历史数据记录</div>
            <div class="start_status" v-if="deviceInfosBasic.DState">
              <div class="dot" v-if="deviceInfosBasic.DState=='运行'"></div>
              <div class="dot blue" v-if="deviceInfosBasic.DState=='正常'"></div>
              <div class="dot red" v-if="deviceInfosBasic.DState=='维修'"></div>
              <div class="dot orange" v-if="deviceInfosBasic.DState=='保养'"></div>
              <div class="dot grey" v-if="deviceInfosBasic.DState=='停机'"></div>
              <div class="status_text">{{deviceInfosBasic.DState}}</div>
            </div>
            <div v-if="deviceInfosBasic.Online == 1" style="color: #5180ff">
              <i class="zhongtaiiconfont zhongtai-icon-zaixian"></i>
              <span style="margin-left: 10px">在线</span>
            </div>
            <div v-if="deviceInfosBasic.Online == 2" style="color: #78829d">
              <i class="zhongtaiiconfont zhongtai-icon-weichushihua"></i>
              <span style="margin-left: 10px">未初始化</span>
            </div>
          </div>
        </div>
      </el-row>
      <el-row style="margin-top: 20px;display: flex;justify-content: flex-start;flex-wrap: wrap;">
        <div class="li_con" v-for="item in liveInfoList" :key="item.Code">
          <ul class="li_info">
            <li class="title">
              <span>{{ item.Name }}</span>
              <div>
                <i v-if=" deviceStorageConfig&&item.OptionType =='enum'||deviceStorageConfig&&item.OptionType == 'date' || deviceStorageConfig&&item.OptionType == 'float' || deviceStorageConfig&&item.OptionType == 'int' || deviceStorageConfig&&item.OptionType == 'geo' "
                  class="zhongtaiiconfont zhongtai-icon-a-caidanguanli"
                  @click="openInfoVisible(item)"
                  style="margin-left: 8px"
                ></i>
              </div>
            </li>
            <li class="val" v-if="item.OptionType == 'geo'">
              <div>
                <div>经度:{{ item.Value ? item.Value.lng : "" }}</div>
                <div>纬度:{{ item.Value ? item.Value.lat : "" }}</div>
              </div>
              <div>
                <i
                  class="el-icon-location-information"
                  @click="openDeviceRunMap(item.Value.lat, item.Value.lng)"
                ></i>
              </div>
            </li>
            <li class="val" v-else>
              <div>
                {{ item.Value }}
                <span v-if="item.Unit">{{ item.Unit }}</span>
              </div>
            </li>
            <li class="title">更新时间</li>
            <li class="time">{{ item.UpdatedOn }}</li>
          </ul>
        </div>
      </el-row>
    </div>
    <div class="online_history_con" v-if="deviceInfosBasic.Online == 0">
      <div v-if="deviceStorageConfig" class="online_history" @click="setOnlineOld"><i class="zhongtaiiconfont zhongtai-icon-lizaixianjilu"></i>历史数据记录</div>
    </div>
    
    <div class="off_line" v-if="deviceInfosBasic.Online == 0">
      <div style="text-align: center">
        <img src="./lixian.png" alt />
        <div class="span">离线中...</div>
      </div>
    </div>
  </div>
</template>

<script>
import { DeviceLiveInfo } from "@/api/rules/device";
import { Time2Local } from "@/utils/common.js";
export default {
  name: "AdminUiDeviceLive",
  props: {
    deviceInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
    deviceStorageConfig:{
      type:Boolean,
      default:false
    }
  },
  data() {
    return {
      deviceInfosBasic: this.deviceInfos,
      liveInfoList: [], //设备实时数据
      orgliveInfoList: [],
      issubscribeDeviceData: false, //是否有订阅设备实时消息
      searchVal: "", //搜索值
      timer:null,
      timer2:null,
      msgTextList:[]
    };
  },
  beforeDestroy() {
    this.$store.dispatch("mqttclient/getClient").then((client) => {
      let tkey = "newprop/" + this.deviceInfos.DeviceId;
      client.unsubscribe(tkey, (error) => {
        console.log("取消订阅", error);
        this.$store.commit("mqttclient/Del_Handler", tkey);
      });
    });
    if(this.timer){
      clearInterval(this.timer)
    }
  },
  mounted() {},
  watch: {
    deviceInfos(to, from) {
      // console.log("变化的", to,from);
      this.deviceInfosBasic = to;
      if(from.DeviceId){
        this.$store.dispatch("mqttclient/getClient").then((client) => {
          let tkey = "newprop/" + from.DeviceId;
          client.unsubscribe(tkey, (error) => {
            this.$store.commit("mqttclient/Del_Handler", tkey);
          });
        });
      }
      this.issubscribeDeviceData=false
      this.subscribeDeviceLive(true);
    },
  },
  methods: {
    filterCodeVal(val) {
      if (val) {
        this.liveInfoList = this.liveInfoList.filter((item) => {
          return (
            item.Name.toLowerCase().indexOf(val.toLowerCase()) > -1 ||
            item.Code.toLowerCase().indexOf(val.toLowerCase()) > -1
          );
        });
      } else {
        this.liveInfoList = JSON.parse(JSON.stringify(this.orgliveInfoList));
      }
      this.$forceUpdate()
    },
    openDeviceRunMap(lat, lng) {
      this.$emit('openDeviceRunMap',lat, lng);
    },
    openInfoVisible(item) {
      this.$emit("openInfoVisible", item);
    },
    setOnlineOld(){
      this.$emit("openInfoVisible", {},'onLine');
    },
    async subscribeDeviceLive(isSend) {
      await this.loadDeviceLiveInfo(isSend)
      // console.log("设备运行数据", res.data);
      if(this.timer){
        // console.log("清除定时器");
        clearInterval(this.timer)
      }
      if(this.timer2){
        // console.log("清除定时器2");
        clearInterval(this.timer2)
      }
      this.timer=setInterval(async ()=>{
        // console.log("执行定时器");
        await this.loadDeviceLiveInfo(false)
      },30000)
      this.timer2=setInterval(async ()=>{
        // console.log("执行定时器2");
        if(this.msgTextList&&this.msgTextList.length>0){
          this.msgTextList=[]
          await this.loadDeviceLiveInfo(false)
        }
        
      },1000)
      //订阅刷新消息
      let that = this;
      if (!this.issubscribeDeviceData) {
        this.$store.dispatch("mqttclient/getClient").then((client) => {
          let tkey = "newprop/" + this.deviceInfosBasic.DeviceId;
          client.subscribe(tkey, (error) => {
            console.log("订阅成功", error);
            this.issubscribeDeviceData = true;
            if (!error) {
              that.$store.commit("mqttclient/Add_Handler", {
                key: tkey,
                func: async function (message) {
                  let msgtxt = message.toString();
                  if (msgtxt == "online") {
                    that.deviceInfosBasic.Online = 1;
                  } else if (msgtxt == "Offline") {
                    that.deviceInfosBasic.Online = 0;
                  } else {
                    // that.updateDeviceLiveInfo(JSON.parse(msgtxt));
                    that.msgTextList.push(msgtxt)
                  }
                },
              });
            }
          });
        });
      }
    },
    async loadDeviceLiveInfo(isSend){//传值：是否发送消息
      let res = await DeviceLiveInfo({
        id: this.deviceInfosBasic.DeviceId,
        needTag: false,
        needSend: isSend,
      });
      this.liveInfoList = res.data;
      this.orgliveInfoList = JSON.parse(JSON.stringify(this.liveInfoList));
    },
    updateDeviceLiveInfo(itemList) {
      if (this.liveInfoList == null) {
        this.liveInfoList = [];
      }
      this.liveInfoList = JSON.parse(JSON.stringify(this.orgliveInfoList));
      itemList.map(async (item) => {
        let curitem = this.liveInfoList.find((x) => x.Code == item.Code);
        let curitem2 = this.orgliveInfoList.find((x) => x.Code == item.Code);
        if (curitem != null) {
          curitem.Name = item.Name;
          curitem.Value = item.Value;
          curitem.Unit = item.Unit;
          curitem.OptionType = item.OptionType;
          curitem.UpdatedOn = await Time2Local(item.UpdatedOn);
          curitem.Description = item.Description;
          curitem2.Name = item.Name;
          curitem2.Value = item.Value;
          curitem2.Unit = item.Unit;
          curitem2.OptionType = item.OptionType;
          curitem2.UpdatedOn = await Time2Local(item.UpdatedOn);
          curitem2.Description = item.Description;
        } else {
          this.liveInfoList.push(item);
          this.orgliveInfoList.push(item);
        }
      });
      if(this.searchVal){
        this.liveInfoList = this.liveInfoList.filter((item) => {
          return (
            item.Name.toLowerCase().indexOf(this.searchVal.toLowerCase()) > -1 ||
            item.Code.toLowerCase().indexOf(this.searchVal.toLowerCase()) > -1
          );
        });
      }
    },
  },
};
</script>
<style lang="less" scoped>
.off_line {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  flex-wrap: wrap;
  padding-top: 100px;

  img {
    width: 100px;
    height: 100px;
    margin-bottom: 20px;
  }

  div.span {
    font-size: 20px;
    color: #bbc0ce;
  }
}
.online_history_con{
  display: flex;
  justify-content: flex-end;
  align-items: center;
  width: 100%;
  height: 16px;
  margin-right: -46px;
}
.online_history{
  font-size: 16px;
  color: #999999;
  line-height: 16px;
  margin-right: 46px;
  cursor: pointer;
  i{
    margin-right: 8px;
  }
}
.start_status{
  display: flex;
  align-items: center;
  margin-right: 30px;
  font-size: 14px;
  color: #333333;
  .dot{
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #45D74D;
    margin-right: 8px;
    &.blue{
      background: #6795FF;
    }
    &.red{
      background: #FF3535;
    }
    &.orange{
      background: #FFAF35;
    }
    &.grey{
      background: #CDCDCD;
    }
  }
}
.li_con {
  width: 25%;
  padding-right: 20px;
  box-sizing: border-box;
  margin-bottom: 10px;

  .li_info {
    // height: 120px;
    background-color: #fafafa;
    border: 1px solid #f0f0f0;
    border-radius: 5px;
    margin: 0 auto;
    padding: 0 auto;
    padding: 25px 20px;

    li {
      list-style: none;
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: #575757;
      font-size: 12px;

      i {
        cursor: pointer;
      }
    }

    li.val {
      padding: 25px 0;
      font-size: 20px;
      color: #000000;
      font-weight: 700;

      i {
        cursor: pointer;
      }
    }

    li.time {
      color: #000050;
      margin-top: 12px;
      font-size: 14px;
    }
  }
}
</style>
