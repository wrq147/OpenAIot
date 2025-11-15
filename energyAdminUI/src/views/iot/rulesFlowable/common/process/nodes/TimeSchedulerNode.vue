<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请指定调度参数" header-bgc="#6106f3" :is-svgicon='false' header-icon="el-icon-s-unfold" />
</template>
  
<script>
import Node from './Node'
import { DeviceList } from "@/api/rules/device";
export default {
  name: "TimeSchedulerNode",
  props: {
    config: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  components: { Node },
  data() {
    return {
      showError: false,
      errorInfo: '',
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 50,
      },
      DeviceList: []
    }
  },
  computed: {
    content() {
      if (this.config.props.DeviceList.length == 0) {
        return "请选择调度的设备";
      }
      return "调度设备：" + this.DeviceList.map(x => x.Name).join();
    }
  },
  watch: {
    "config.props.DeviceList": function (value, oldValue) {
      this.ScheDevices();
    }
  },
  mounted() {
    this.ScheDevices();
  },
  methods: {
    validate(err) {
      this.showError = false
      this.errorInfo = "";

      if (this.config.props.DeviceList.length == 0) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择设备";
        err.push(this.errorInfo);
        return false;
      }

      this.config.props.DeviceSets.forEach(element => {
        let tmpname = element.id;
        let curdd = this.DeviceList.filter(x => x.Id == element.id);
        if (curdd.length > 0) {
          tmpname = curdd[0].Name;
        }
        if (element.conditions.length == 0) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "的调度条件不能为空";
          err.push(this.errorInfo);
          return false;
        }
        else if (element.actions.length == 0) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "的执行动作不能为空";
          err.push(this.errorInfo);
          return false;
        }
      });
      return !this.showError
    },
    async ScheDevices() {
      this.filterParams.Ids = this.config.props.DeviceList;
      let rsp = await DeviceList(this.filterParams);
      this.DeviceList = rsp.data.List;
    },
  }
}
</script>
  
<style lang="scss" scoped></style>
  