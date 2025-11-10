<template>
  <div>
    <el-form class="huancun_con" label-width="100px">
      <el-form-item label="计数方式：" class="numberWay">
        <el-select v-model="config.Way" placeholder="请选择计数方式">
          <el-option
            v-for="item in wayList"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="聚合的字段名">
        <el-select v-model="config.FieldName" placeholder="请选择聚合的字段名">
          <el-option v-for="form in formItems" :key="form.code" :label="form.name" :value="form.code"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="计数值：">
        <el-input-number v-model="config.Count" :step="1" :min="1" :max="9999"></el-input-number><span v-if="config.Way==1">秒</span>
      </el-form-item>
      <el-form-item label="保留计数：">
        <el-input-number v-model="config.CountLen" :step="1" :min="0" :max="9999"></el-input-number>
        <div style="color: #999;"><i class="el-icon-info" style="margin-right: 5px;"></i>'聚合数据'触发后保留的数据量，默认为0</div>
      </el-form-item>
      <div style="font-size: 14px;color:#999;padding: 10px 20px;line-height: 26px;">
        提示：'聚合数据'会将触发的属性按指定的字段名进行聚合，聚合后的数据可供其它节点使用。
      </div>
    </el-form>
  </div>
</template>

<script>
export default {
  name: "MetronomeNodeConfig", //聚合数据节点
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    formItems() {
      return this.$store.state.rulesFlowable.rulesProductAttr;
    },
    selectedNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode;
    }
  },
  data() {
    return {
      wayList: [
        { label: "次数", value: 0 },
        { label: "秒计数", value: 1 }
      ]
    };
  },
  mounted() {
  },
  methods: {}
};
</script>

<style lang="less">
.huancun_con {
  .el-form-item {
    .el-form-item__content {
      .el-select {
        width: 100%;
      }
    }
  }
  .item-desc {
    color: rgba(50, 150, 250, 0.71);
    display: block;
    width: 80%;
    height: 36px;
    line-height: 36px;
    background-color: #f5f7fa;
    text-align: left;
    margin-bottom: 10px;
    font-size: 14px;
    border-radius: 5px;
    border: 1px solid #dcdfe6;
    padding-left: 30px;
    box-sizing: border-box;
  }
}
</style>
<style lang="less" scoped>
.choose {
  border-radius: 5px;
  margin-top: 2px;
  background: #f4f4f4;
  border: 1px dashed #1890ff !important;
}

.drag-hover {
  color: #1890ff;
}

.drag-no-choose {
  cursor: move;
  background: #f8f8f8;
  border-radius: 5px;
  margin: 5px 0;
  height: 25px;
  line-height: 25px;
  padding: 5px 10px;
  border: 1px solid #ffffff;
  div {
    display: inline-block;
    font-size: small !important;
  }

  div:nth-child(2) {
    float: right !important;
  }
}
</style>
