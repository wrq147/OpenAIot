<template>
  <div>
    <ul class="infinite-list" style="overflow-y: auto;word-break:break-all;">
      <li
        v-for="(i, index) in infolist"
        :key="index"
        class="infinite-list-item"
      >
        {{ i }}
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "MqConsole",
  props: {
    xkey: {
      type: String,
      default: "",
    },
    isSubs:{
      type:Boolean,
      default:false
    }
  },
  data() {
    return {
      infolist:[],
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
    clear(){
      this.infolist.splice(0, this.infolist.length);
    },
    connect() {
      this.$store.dispatch("mqttclient/getClient").then((client) => {
        let tkey = "console/" + this.xkey;
        let that=this
        client.subscribe(tkey, (error) => {
          if (!error) {
            this.$store.commit("mqttclient/Add_Handler", {
              key: tkey,
              func: function (message) {
                // console.log("规则调试信息",message,message.toString());
                
                that.infolist.push(message.toString());
              },
            });
            this.$emit("update:isSubs",false);
          }
        });
      });
    },
    disconnect: function () {
      this.$store.dispatch("mqttclient/getClient").then((client) => {
        let tkey = "console/" + this.xkey;
        client.unsubscribe(tkey, (error) => {
            this.$store.commit("mqttclient/Del_Handler", tkey);
            this.$emit("update:isSubs",true);
        });
      });


    },
  },
};
</script>

<style>
.infinite-list-item{margin-bottom: 10px;background-color: #f4f4f4;padding:5px 5px;color:#333;}
</style>
