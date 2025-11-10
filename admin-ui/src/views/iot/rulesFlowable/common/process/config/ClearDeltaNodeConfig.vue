<template>
  <div>
    <el-form label-width="80px">
      <el-form-item label="清除目标">
        <el-select v-model="config.ClearType" placeholder="请选择要清除的数据" @change="config.Codes = []">
          <el-option label="规则参数" :value="0"></el-option>
          <el-option label="设备缓存" :value="1"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="清除参数" v-if="config.ClearType == 0">
        <el-select v-model="config.Codes" multiple placeholder="请选择要清除的数据">
          <el-option v-for="(item, idx) in ParamList" :key="idx" :label="item.name" :value="item.code"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="目标设备" v-if="config.ClearType == 1">
        <el-select :clearable="true" :multiple-limit="50" multiple v-model="config.Codes" filterable remote
          reserve-keyword placeholder="请选择要清除的设备" @clear="reloadDevice" :remote-method="remoteMethod"
          :loading="searchloading">
          <el-option v-for="item in deviceOptions" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
export default {
  name: "ClearDeltaNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
  },
  data() {
    return {
      searchloading: false,
      deviceOptions: [],
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 50,
      },
    };
  },
  mounted() {
    this.filterParams.DtuIds = this.config.Codes;
    this.remoteMethod("");
  },
  methods: {
    reloadDevice() {
      this.filterParams.DtuIds = [];
      this.remoteMethod("");
    },
    remoteMethod(query) {
      this.searchloading = true;
      this.filterParams.Key = query;
      DeviceList(this.filterParams)
        .then(rsp => {
          this.searchloading = false;
          this.deviceOptions = rsp.data.List;
        });

    },
  }
};
</script>

<style lang="scss"></style>
