<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请给指定参数赋值" header-bgc="#3296FA" :is-svgicon='true' header-icon="shujuhuancun" />
</template>

<script>
import Node from './Node'

export default {
  name: "DataWriteNode",
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
    }
  },
  computed: {
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
    content() {
      if(this.config.props.Counts==null){
        this.config.props.Counts=[];
      }
      if (this.config.props.Writes.length == 0 && this.config.props.Counts.length == 0) {
        return "请添加要赋值的参数";
      }
      let prestr = "";
      if (this.config.props.Counts != null) {
        let cccsss = this.config.props.Counts.map(x => x.code);
        let arrcc = this.ParamList.filter(x => cccsss.indexOf(x.code) >= 0);
        if (arrcc.length > 0) {
          prestr = arrcc.map(x => x.name).join();
        }
      }

      let codess = this.config.props.Writes.map(x => x.code);
      let arr = this.ParamList.filter(x => codess.indexOf(x.code) >= 0);
      if (arr.length > 0 && prestr != "") {
        prestr += "和"
      }
      return "赋值的参数：" + prestr + arr.map(x => x.name).join();
    }
  },
  methods: {
    validate(err) {
      this.showError = false
      this.errorInfo = "";
      if(this.config.props.Counts==null){
        this.config.props.Counts=[];
      }
      if (this.config.props.Writes.length == 0 && this.config.props.Counts.length == 0) {
        this.showError = true
        this.errorInfo = "请添加要赋值的参数"
      }
      this.config.props.Counts.forEach(element => {
        if (element.code == "") {
          this.showError = true
          this.errorInfo = "请指定要赋值的参数"
          return;
        }
      });
      this.config.props.Writes.forEach(element => {
        if (element.code == "") {
          this.showError = true
          this.errorInfo = "请指定要赋值的参数"
          return;
        }
      });

      if (this.showError) {
        err.push(this.errorInfo);
      }
      return !this.showError
    },
  }
}
</script>

<style scoped></style>
