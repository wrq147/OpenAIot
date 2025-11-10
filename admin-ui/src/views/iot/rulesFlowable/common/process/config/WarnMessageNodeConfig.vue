<template>
  <div>
    <el-form class="huancun_con" label-width="120px">
      <el-form-item label="目标类型：">
        <el-select v-model="config.TargetType" placeholder="请选择目标类型" @change="choiceTargetType">
          <el-option label="当前设备" :value="0" v-if="enableCur"></el-option>
          <el-option label="选择设备" :value="1"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="目标设备Id：" v-if="config.TargetType == 1">
        <el-select v-model="config.TargetId" clearable filterable remote reserve-keyword :remote-method="remoteMethod"
          @clear="remoteMethod('')" :loading="loading" placeholder="请选择目标设备Id" @change="devChange">
          <el-option v-for="item in deviceLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="事件标识：">
        <el-select v-model="config.EventId" placeholder="请选择事件标识">
          <el-option
            v-for="item in eventItems"
            :key="item.code"
            :label="item.name"
            :value="item.code"
          ></el-option>
        </el-select>
      </el-form-item>

    </el-form>

  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
export default {
  name: "WarnMessageNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    eventItems() {
      if (this.config.TargetType == 0) {
        return this.$store.state.rulesFlowable.rulesProductEvent;
      }
      else {
        return this.evtList;
      }
    },
    enableCur() {
      if (this.$store.state.rulesFlowable.selectProductInfo != null) {
        return true;
      }
      else {
        return false;
      }
    }
  },
  data() {
    return {
      deviceLists: [],
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
      },
      evtList: [],
      loading: false
    };
  },
  async mounted() {
    await this.remoteMethod('');
    this.$nextTick(()=>{
      if(this.config.TargetId){
        this.devChange()
      }
    })
  },
  methods: {
    devChange() {
      let devlist = this.deviceLists.filter(x => x.Id == this.config.TargetId);

      if (devlist.length == 0) {
        this.$set(this, "funList", []);
      }
      else {
        this.getNewProductFun(devlist[0].ProductId);
      }

    },
    choiceTargetType() {
      this.config.TargetId = ""
    },
    async remoteMethod(query) {
      this.loading = true;
      if (this.config.TargetId != "") {
        this.filterParams.Ids = [this.config.TargetId];
      }
      else {
        this.filterParams.Ids = undefined;
      }
      this.filterParams.Key = query;
      try {
       let rsp=await DeviceList(this.filterParams)
       this.loading = false;
       this.deviceLists = rsp.data.List;
      } catch (error) {
        this.loading = false;
        this.$message.error("接口异常");
      }
      
        
    },

    getNewProductFun(pid) {
      productInfo({ id: pid }).then(rsp => {
        if (rsp.code == 0) {
          let jsonLis = JSON.parse(rsp.data.ModelTSL);
          if (jsonLis.events) {
            this.$set(this, "evtList", jsonLis.events);
          }
        }
      });
    },
  }
};
</script>

<style lang="less">
.center_form {
  .el-form-item {
    display: flex;
    justify-content: center;
  }
}
.huancun_con {
  .el-form-item {
    .el-form-item__content {
      .el-select {
        width: 100%;
      }
    }
  }

}
</style>
