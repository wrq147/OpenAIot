<template>
  <div>
    <el-form label-width="120px">
      <el-form-item label="聚合数据">
        <el-select v-model="config.CountId" placeholder="请选择聚合数据">
          <el-option
              v-for="(node, i) in nodeOptions"
              :key="i"
              :label="node.name"
              :value="node.id"
            ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="异常检测类型">
        <el-select v-model="config.ExceptType" placeholder="请选择异常检测类型">
          <el-option label="峰值" value="spike"></el-option>
          <el-option label="拐点" value="change"></el-option>
          <el-option label="范围" value="range"></el-option>
          <el-option label="SR-CNN" value="SRCNN"></el-option>
        </el-select>
      </el-form-item>
      <template v-if="config.ExceptType=='range'">
        <el-form-item label="最小值">
          <el-input-number v-model="config.Min" :max="config.Max"></el-input-number>
        </el-form-item>
        <el-form-item label="最大值">
          <el-input-number v-model="config.Max" :min="config.Min"></el-input-number>
        </el-form-item>
      </template>
      <el-form-item v-else label="检测置信度">
        <el-input-number v-model="config.Confidence" :min="0" :max="100"></el-input-number>
      </el-form-item>
      <div style="font-size: 14px;color:#999;padding: 10px 20px;line-height: 26px;">
        提示：检测到异常后节点将变成激活状态，并将异常数据存储到时序数据库中
      </div>
    </el-form>
  </div>
</template>
  
<script>
export default {
  name: "ExceptNodeConfig",
  components: {},
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  data() {
    return {
    };
  },
  computed: {
    nodeOptions() {
      let values = [];
      const excType = [
        "METRONOME",
      ];
      this.$store.state.rulesFlowable.rulesNodeMap.forEach((v) => {
        if (excType.indexOf(v.type) !== -1) {
          values.push({ id: v.id, name: v.name });
        }
      });
      return values;
    },
  },
  mounted() {

  },
  methods: {}
};
</script>
  
<style scoped></style>
  