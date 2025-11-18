<template>
  <div>
    <el-form class="huancun_con" label-position="right" label-width="120px">
      <el-form-item label="目标类型：">
        <el-select v-model="config.TargetType" placeholder="请选择目标类型" @change="choiceTargetType">
          <el-option label="当前设备" :value="0" v-if="enableCur"></el-option>
          <el-option label="选择设备" :value="1"></el-option>
          <el-option label="选择协议" :value="2"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="目标设备：" v-if="config.TargetType == 1">
        <el-select v-model="config.TargetId" clearable filterable remote reserve-keyword :remote-method="remoteMethod"
          @clear="remoteMethod('')" :loading="loading" placeholder="请选择目标设备" @change="devChange">
          <el-option v-for="item in deviceLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="目标协议：" v-if="config.TargetType == 2">
        <el-select v-model="config.TargetId" clearable filterable remote reserve-keyword :remote-method="remoteMethod"
          @clear="remoteMethod('')" :loading="loading" placeholder="请选择目标协议" @change="devChange">
          <el-option v-for="item in productLists" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="功能标识：">
        <el-select v-model="config.FunctionId" placeholder="请选择功能标识">
          <el-option v-for="item in funcItems" :key="item.code" :label="item.name" :value="item.code"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="是否输入数据：">
        <el-switch v-model="config.EventInput" active-color="#409EFF" inactive-color="#c1c1c1" :active-value="true"
          :inactive-value="false" active-text="是" inactive-text="否" />
      </el-form-item>
      <div v-if="config.EventInput == false" style="margin-bottom: 20px;margin-top:-20px;">
        <div>
          <el-table :data="funcInputArr" class="data_table">
            <el-table-column property="name" label="参数名称" width="100"></el-table-column>
            <el-table-column property="type" label="输入类型" width="100"></el-table-column>
            <el-table-column label="值">
              <template slot-scope="scope">
                <param-item :Item="scope.row" :disabled="scope.row.readOnly"
                  @change="chgFunParam(scope.row.code, $event)"></param-item>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>
      <el-form-item v-if="config.TargetType != 2" label="是否输出数据：">
        <el-switch v-model="config.ReturnOutput" active-color="#409EFF" inactive-color="#c1c1c1" :active-value="true"
          :inactive-value="false" active-text="是" inactive-text="否" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { DeviceList } from "@/api/rules/device";
import { productList, productInfo } from "@/api/rules/productModel";
import paramItem from "../../../../funInput/paramItem.vue";
export default {
  name: "funcNodeConfig",
  components: { paramItem },
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      }
    }
  },
  computed: {
    funcItems() {
      if (this.config.TargetType == 0) {
        return this.$store.state.rulesFlowable.rulesProductFunc;
      }
      else {
        return this.funList;
      }
    },
    funcInputArr() {
      let curitems = this.funcItems.filter(x => x.code == this.config.FunctionId);
      let tmparr = [];
      if (curitems.length > 0) {
        tmparr = curitems[0].inputs;
      }
      if (tmparr == null) {
        tmparr = [];
      }
      if (this.config.InputData == null) {
        this.config.InputData = {};
      }
      tmparr.forEach(element => {
        if (this.config.InputData.hasOwnProperty(element.code)) {
          element.defval = this.config.InputData[element.code];
        }
      });
      return tmparr;
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
      productLists: [],
      productForm: {
        pageNum: 1,
        pageSize: 100,
        Name: null,
      },
      filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
      },
      funList: [],
      loading: false
    };
  },
  async mounted() {
    await this.remoteMethod('');
  },
  methods: {
    async getNewProductList() {
      try {
        let rsp = await productList(this.productForm);
        if (rsp.code == 0) {
          this.productLists = JSON.parse(JSON.stringify(rsp.data.List));
        }
      } catch (error) {
        console.log("错误", error);
      }
    },
    devChange() {
      if (this.config.TargetType == 2) {
        this.getNewProductFun(this.config.TargetId);
      }
      else {
        let devlist = this.deviceLists.filter(x => x.Id == this.config.TargetId);
        if (devlist.length == 0) {
          this.$set(this, "funList", []);
        }
        else {
          this.getNewProductFun(devlist[0].ProductId);
        }
      }
    },
    async choiceTargetType() {
      this.config.TargetId = ""
      await this.remoteMethod('');
    },
    async remoteMethod(query) {
      this.loading = true;
      if (this.config.TargetType == 1) {
        if (this.config.TargetId != "") {
          this.filterParams.Ids = [this.config.TargetId];
        }
        else {
          this.filterParams.Ids = undefined;
        }
        this.filterParams.Key = query;

        try {
          let rsp = await DeviceList(this.filterParams);
          this.deviceLists = rsp.data.List;
        }
        catch (err) {
          this.$message.error("接口异常");
        }
      }
      else if (this.config.TargetType == 2) {
        if (query !== "") {
          setTimeout(async () => {
            this.productForm.Name = query;
            await this.getNewProductList();
            this.productLists = this.productOptionList.filter((item) => {
              return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
            });
          }, 200);
        } else {
          this.productLists = this.productOptionList;
          delete this.productForm.Name;
          await this.getNewProductList();
        }
      }
      this.devChange();
      this.loading = false;
    },
    getNewProductFun(pid) {
      productInfo({ id: pid }).then(rsp => {
        if (rsp.code == 0) {
          let jsonLis = JSON.parse(rsp.data.ModelTSL);
          if (jsonLis.functions) {
            this.$set(this, "funList", jsonLis.functions);
          }
        }
      });
    },
    chgFunParam(code, val) {
      if (this.config.InputData == null) {
        this.config.InputData = {};
      }
      this.config.InputData[code] = val;
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
