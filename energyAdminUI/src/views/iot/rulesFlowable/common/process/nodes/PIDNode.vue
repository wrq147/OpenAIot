<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请指定PID参数" header-bgc="#2FCAEB" :is-svgicon='true' header-icon="PIDjiedian" />
</template>
  
<script>
import Node from './Node'
import { DeviceList } from "@/api/rules/device";
export default {
  name: "PIDNode",
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
      if(this.config.props.refdevid){
        let tmplist = this.DeviceList.filter(x => x.Id == this.config.props.refdevid);
        if (tmplist.length > 0) {
          return "PID设备：" + tmplist[0].Name;
        }else{
          return this.config.props.refdevid
        }
        
      }else{
        return '当前设备'
      }
      
    }
  },
  watch: {
    "config.props.refdevid": function (value, oldValue) {
      this.ScheDevices();
    },
    "config.props.DevControllerItem": function (value, oldValue) {
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

      if (!this.config.props.refprop) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择参考的设备属性";
        err.push(this.errorInfo);
        return false;
      }
      if (!this.config.props.targetval) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择目标值参数";
        err.push(this.errorInfo);
        return false;
      }
      if (!this.config.props.pname) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择比例参数";
        err.push(this.errorInfo);
        return false;
      }
      if (!this.config.props.iname) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择积分时间";
        err.push(this.errorInfo);
        return false;
      }
      if (!this.config.props.dname) {
        this.showError = true
        this.errorInfo = "节点【" + this.config.name + "】未选择微分时间";
        err.push(this.errorInfo);
        return false;
      }
      this.config.props.DevControllerItem.forEach(element => {
        let tmpname = element.id;
        let curdd = this.DeviceList.filter(x => x.Id == element.id);
        if (curdd.length > 0) {
          tmpname = curdd[0].Name;
        }
        if (!element.maxval) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "的最大控制量参数不能为空";
          err.push(this.errorInfo);
          return false;
        }
        else if (!element.minval) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "的最小控制量参数不能为空";
          err.push(this.errorInfo);
          return false;
        }else if (!element.code) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "执行的功能不能为空";
          err.push(this.errorInfo);
          return false;
        }else if (!element.codeval) {
          this.showError = true
          this.errorInfo = "设备" + tmpname + "的功能的控制量参数不能为空";
          err.push(this.errorInfo);
          return false;
        }
      });
      return !this.showError
    },
    async ScheDevices() {
      let devlist=this.config.props.DevControllerItem.map(row=>row.id)
      devlist.push(this.config.props.refdevid)
      this.filterParams.Ids = devlist;
      let rsp = await DeviceList(this.filterParams);
      this.DeviceList = rsp.data.List;
    },
  }
}
</script>
  
<style lang="scss" scoped></style>
  