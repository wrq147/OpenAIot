<template>
  <div class="ptselector">
    <el-select v-model="templateId" placeholder="请选择模板" clearable :loading="loading">
      <el-option v-for="item in TempList" :key="item.Id" :label="item.Name" :value="item.Id">
        <div class="name">{{ item.Name }}</div>
        <span class="temp">纸张宽高={{ item.PaperWidth }}×{{ item.PaperHeight }}</span>
      </el-option>
    </el-select>

    <div class="pt-newbtn">
      <el-button @click="addTemplate" type="primary" size="small" icon="el-icon-plus" plain>新增</el-button>
    </div>
  </div>
</template>

<script>
import { listPrintTemplate } from '@/api/report/printTemplate'
export default {
  name: "PrintSelector",
  props: {
    dataId: {
      type: String,
      default: ''
    },
    value: String
  },
  computed: {
    templateId: {
      get() {
        return this.value;
      },
      set(newValue) {
        this.$emit('input', newValue);
      }
    }
  },
  data() {
    return {
      loading: true,
      TempList: []
    };
  },
  mounted() {
    this.loading = true;
    listPrintTemplate({ showAll: true, DataId: this.dataId }).then(res => {
      this.TempList = res.data.List;
      this.loading = false;
    })
  },
  methods: {
    addTemplate() {
      this.$router.push({
        path: "/report/print/design",
        query: { data: this.dataId }
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.ptselector {

  .name {
    text-overflow: ellipsis;
    overflow: hidden;
  }

  .temp {
    font-size: 12px;
    color: #b4b4b4;
  }

  .pt-newbtn {
    display: inline-block;
    margin-left: 15px;
  }
}
</style>