<template>
  <div>
    <el-form class="huancun_con" label-position="right" label-width="120px">
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
      <el-form-item label="标签标识：">
        <el-select v-model="config.TagId" placeholder="请选择标签" @change="config.Express = 'data'">
          <el-option v-for="item in tagItems" :key="item.code" :label="item.name" :value="item.code"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="赋值表达式：">
        <el-input placeholder="请输入表达式" v-model="config.Express" style="width:280px;margin-right: 15px;"></el-input>
        <el-link icon="el-icon-edit" @click="popExpressDlg()" style="margin-right: 10px;cursor: pointer;"></el-link>
      </el-form-item>
    </el-form>

    <el-dialog title="编辑表达式" append-to-body :close-on-click-modal="false" :visible.sync="expressOpen" width="670px">
      <div v-if="dlgExpress != null">
        <ExpressEditor :content="dlgExpress" ref="expEd"></ExpressEditor>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="expressOpen = false;">取 消</el-button>
        <el-button type="primary" @click="confirmExpress">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productInfo } from "@/api/rules/productModel";
import ExpressEditor from "../../ExpressEditor.vue";
export default {
  name: "TagNodeConfig",
  components: {
    ExpressEditor
  },
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    tagItems() {
      if (this.config.TargetType == 0) {
        return this.$store.state.rulesFlowable.rulesProductTags.filter(x => x.option.type != 'geo');
      }
      else {
        return this.tagList.filter(x => x.option.type != 'geo');
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
      tagList: [],
      loading: false,
      expressOpen: false,
      dlgExpress: ""
    };
  },
  async mounted() {
    await this.remoteMethod('');
    await this.devChange();
  },
  methods: {
    async devChange() {
      let devlist = this.deviceLists.filter(x => x.Id == this.config.TargetId);
      if (devlist.length == 0) {
        this.$set(this, "tagList", []);
      }
      else {
        await this.getNewProductFun(devlist[0].ProductId);
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
        let rsp = await DeviceList(this.filterParams);
        this.loading = false;
        this.deviceLists = rsp.data.List;
      }
      catch (err) {
        this.loading = false;
        this.$message.error("接口异常");
      }
    },
    async getNewProductFun(pid) {
      let rsp = await productInfo({ id: pid });
      if (rsp.code == 0) {
        let jsonLis = JSON.parse(rsp.data.ModelTSL);
        if (jsonLis.tags) {
          this.$set(this, "tagList", jsonLis.tags);
        }
      }
    },
    popExpressDlg() {
      this.expressOpen = true;
      this.dlgExpress = this.config.Express;
    },
    confirmExpress() {
      let tmpexpress = this.$refs.expEd.getFormulaStr();
      if (this.$refs.expEd.isExpress(tmpexpress).success == false) {
        this.$message.error("表达式格式错误");
        return;
      }
      this.config.Express = tmpexpress;
      this.expressOpen = false;
    }
  }
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
