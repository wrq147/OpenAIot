<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请设置异常检测" header-bgc="#E6A23C" header-icon="el-icon-warning-outline" />
</template>
  
<script>
import Node from './Node'
export default {
  name: "ExceptNode",
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
  mounted() {

  },
  computed: {
    content() {
      if (this.config.props.CountId === '') {
        return '请选择聚合数据';
      } else {
        let cclist = [];
        this.$store.state.rulesFlowable.rulesNodeMap.forEach((v) => {
          if (v.id == this.config.props.CountId) {
            cclist.push({ id: v.id, name: v.name });
          }
        });
        if (cclist.length > 0) {
          return "检测【" + cclist[0].name + "】异常状态";
        }
        return ''
      }
    },
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      return true;
    }
  }
}
</script>
  
<style scoped></style>
  