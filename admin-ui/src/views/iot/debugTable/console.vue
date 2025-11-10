<template>
  <div class="console-cls">
    <el-table :data="infolist" style="width: 100%" height="450" :header-row-style="{ 'background-color': '#B5B5B5' }">
      <el-table-column prop="time" width="240" label="时间/日志类型"></el-table-column>
      <el-table-column prop="msg" label="内容"></el-table-column>
    </el-table>
  </div>
</template>

<script>
export default {
  name: "MqConsole",
  props: {
    xkey: {
      type: String,
      default: ""
    },
    isSubs: {
      type: Boolean,
      default: false
    },
    displayWay: {
      type: String,
      default: "文本"
    }
  },
  data() {
    return {
      infolist: []
    };
  },
  computed: {},
  mounted() {
    this.connect();
  },
  beforeDestroy() {
    this.disconnect();
  },
  methods: {
    clear() {
      this.infolist.splice(0, this.infolist.length);
    },
    timestampToTime(times) {
      let time = times[1];
      let mdy = times[0];
      mdy = mdy.split("/");
      let month = parseInt(mdy[0]);
      let day = parseInt(mdy[1]);
      let year = parseInt(mdy[2]);
      return year + "-" + month + "-" + day + " " + time;
    },
    readUTF(arr) {
      if (typeof arr === 'string') {
        return arr;
      }
      let UTF = '', _arr = arr;
      for (let i = 0; i < _arr.length; i++) {
        let one = _arr[i].toString(2), v = one.match(/^1+?(?=0)/);
        if (v && one.length == 8) {
          let bytesLength = v[0].length;
          let store = _arr[i].toString(2).slice(7 - bytesLength);
          for (let st = 1; st < bytesLength; st++) {
            store += _arr[st + i].toString(2).slice(2)
          }
          UTF += String.fromCharCode(parseInt(store, 2));
          i += bytesLength - 1
        } else {
          UTF += String.fromCharCode(_arr[i])
        }
      }
      return UTF
    },
    hexToStr(str) {
      if (str.length % 2 != 0) {
        return console.log('必须为偶数');
      }
      let buf = [];
      for (let i = 0; i < str.length; i += 2) {
        buf.push(parseInt(str.substring(i, i + 2), 16));
      }
      return this.readUTF(buf);
    },
    connect(xkey) {
      if (xkey) {
        let that = this;
        this.$store.dispatch("mqttclient/getClient").then(client => {
          let tkey = "console/" + xkey;
          client.subscribe(tkey, error => {
            // console.log(error,'errorerror');
            if (!error) {
              this.$store.commit("mqttclient/Add_Handler", {
                key: tkey,
                func: function (message) {
                  let time = new Date();
                  let lllmmm = message.toString();
                  let startIdx = lllmmm.indexOf(":");
                  let tmpstr = '';
                  let tmptip = "";
                  if (startIdx >= 0) {
                    tmpstr = lllmmm.substr(startIdx + 1);
                    tmptip = lllmmm.substring(0, startIdx);
                  }

                  if(tmpstr.indexOf("'")==0||tmpstr.indexOf("\"")==0){
                      tmpstr=tmpstr.substring(1);
                      tmpstr=tmpstr.substring(0,tmpstr.length-1);
                  }
                  let constr = ''
                  if (that.displayWay == "Hex") {
                    constr += tmpstr;
                  }
                  else {
  
                    if (tmpstr.match(/^[0-9a-f]+$/i)) {
                      try {
                        constr += that.hexToStr(tmpstr);
                      }
                      catch {
                        constr += tmpstr;
                      }
                    }
                    else {
                      constr += tmpstr;
                    }

                  }

                  let obj = {};
                  obj = {
                    time: time.toLocaleTimeString() + '/' + tmptip,
                    msg: constr
                  };
                  // console.log("消息订阅", obj);
                  that.infolist.push(obj);
                }
              });
              this.$emit("update:isSubs", false);
            }
          });
        });
      }
    },
    dayin() {
      this.connect();
      // console.log(this.infolist, "this.infolisthhhh");
    },
    disconnect: function (beforekey,cb) {
      this.$store.dispatch("mqttclient/getClient").then(client => {
        let tkey = "console/" + beforekey;
        client.unsubscribe(tkey, error => {
          console.log("断开订阅", error,beforekey);
          this.$store.commit("mqttclient/Del_Handler", "console/" + beforekey);
          if(cb){
            cb()
          }
          this.$emit("update:isSubs",true);
        });
      });
    }
  }
};
</script>

<style  lang="scss" scoped>
.console-cls{
  .el-table{
    ::v-deep .cell {
      white-space: pre-line !important;
    }
  }
} 
</style>
