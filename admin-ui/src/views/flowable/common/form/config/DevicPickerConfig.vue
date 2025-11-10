<template>
  <div>
    <el-form-item label="提示文字">
      <el-input size="small" v-model="value.placeholder" placeholder="请设置提示语" />
    </el-form-item>
    <el-form-item label="限制个数">
      <!-- <el-switch v-model="value.multiple"></el-switch> -->
      <el-select v-model="value.limit" placeholder="请选择">
        <el-option v-for="item in selNumber" :key="item" :label="item + '台'" :value="item"></el-option>
      </el-select>
    </el-form-item>
    <el-form-item label="限制产品" prop="product_id">
      <el-select v-model="value.limit_product" multiple filterable remote reserve-keyword placeholder="请选择限制产品"
        :remote-method="remoteMethod" :loading="prodloading">
        <el-option v-for="item in productLists" :key="item.Id" :label="item.Name" :value="item.Id">
        </el-option>
      </el-select>
    </el-form-item>
    <div v-show="value.limit_product.length > 0 && value.limit == 1">
      <div>
        <span style="margin-right: 5px; color: #333333">关联属性绑定</span>
      </div>
      <el-form v-for="(ite, inx) in value.synclist" :key="inx" ref="synclistForm" :model="ite" label-position="top"
        :inline="true" class="synclistForm">
        <el-form-item :label="inx == 0 ? '标识符' : ''" prop="code">
          <el-select filterable v-model="ite.code" placeholder="请选择标识符" @change="ite.field_id = ''">
            <el-option v-for="item in codeList" :key="item" :label="item" :value="item"></el-option>
          </el-select>
          <!-- <el-select v-model="ite.code" placeholder="请选择标识符">
          <el-option-group
            v-for="group in codeList"
            :key="group.label"
            :label="group.label"
          >
            <el-option
              v-for="item in group.options"
              :key="item.code"
              :label="item.code"
              :value="item.code"
            ></el-option>
          </el-option-group>
          </el-select>-->
        </el-form-item>
        <el-form-item :label="inx == 0 ? '控件' : ''" prop="field_id">
          <!-- <el-input v-model="ite.PropertyCode" placeholder="请输入控件" /> -->
          <el-select v-model="ite.field_id" clearable filterable placeholder="请选择控件">
            <template v-for="item in formData">
              <el-option no-data-tex="无匹配数据" v-if="
              ite.code &&
              codeMap.get(ite.code) &&
              codeMap.get(ite.code).option &&
              ((codeMap.get(ite.code).option.type == 'enum' &&
                item.name == 'MultipleSelect') ||
                (codeMap.get(ite.code).option.type == 'date' &&
                  item.valueType == 'Date') ||
                ((codeMap.get(ite.code).option.type == 'int' ||
                  codeMap.get(ite.code).option.type == 'float') &&
                  item.valueType == 'Number') ||
                (codeMap.get(ite.code).option.type == 'string' &&
                  item.valueType == 'String'))
            " :key="item.id" :label="item.title" :value="item.id"></el-option>
            </template>
            
          </el-select>
        </el-form-item>
        <el-form-item :label="inx == 0 ? '操作' : ''" prop="field_id" class="butt_item">
          <!-- <el-input v-model="ite.PropertyCode" placeholder="请输入控件" /> -->
          <el-button plain @click="delSynclist(inx)">-</el-button>
        </el-form-item>
      </el-form>
      <div>
        <el-button style="width: 100%; height: 36px" plain @click="addSynclist()">+</el-button>
      </div>
    </div>
    <!-- <el-form-item label="是否多选">
      <el-switch v-model="value.multiple"></el-switch>
    </el-form-item>-->
  </div>
</template>

<script>
import { getItems } from "../../utlity.js";
import { myProductList } from "@/api/after/dev";
import { productInfo } from "@/api/rules/productModel";
export default {
  name: "DevicPicker",
  components: {},
  props: {
    value: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      selNumber: [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
      ],
      productmap: new Map(),
      productCodemap: new Map(),
      codeMap: new Map(), //标识符标识
      productLists: [], //产品列表
      codeGroup: [],
      codeList: [], //属性列表
      queryform: {
        pageNum: 1,
        pageSize: 300,
        Name: "",
        Pids:[]
      },
      prodloading: false
    };
  },
  computed: {
    nowNode() {
      return this.$store.state.flowable.selectedNode;
    },
    formData() {
      let curid = this.$store.state.flowable.selectFormItem.id;
      return getItems(
        this.$store.state.flowable.design.formItems.filter((x) => x.id != curid)
      );
    },
    formPerms() {
      return this.$store.state.flowable.selectedNode.props.formPerms;
    },
  },
  mounted() {
    this.queryform.Pids=this.value.limit_product;
    this.getproductList();
  },
  methods: {
    choiceProduct(val) {
      this.codeList = [];
      this.value.limit_product.forEach(async (item) => {
        if (this.productmap.get(item)) {
          this.productCodemap.get(item).forEach((its) => {
            if (!this.codeList.includes(its.code)) {
              this.codeMap.set(its.code, its);
              this.codeList.push(its.code);
            }
          });
        } else {
          let proInfo = await productInfo({ id: item });
          this.initClassMap(proInfo.data);
          this.productCodemap.get(item).forEach((its) => {
            if (!this.codeList.includes(its.code)) {
              this.codeMap.set(its.code, its);
              this.codeList.push(its.code);
            }
          });
        }
      });
      for (let it = this.value.synclist.length - 1; it >= 0; it--) {
        //使用逆序循环
        if (
          this.value.synclist[it].code &&
          (!this.codeList.includes(this.value.synclist[it].code) ||
            this.codeList.length == 0)
        ) {
          this.value.synclist.splice(it, 1);
        }
      }

      this.value.synclist = JSON.parse(JSON.stringify(this.value.synclist));
      if (this.value.synclist.length == 0) {
        let obj = {
          code: "", //产品物模型的属性标识或标签标识
          field_id: "", //表单字段ID
        };
        this.value.synclist.push(obj);
      }
    },
    initClassMap(node) {
      let curnode = node;
      let ModelTSL = {};
      let arr = [];
      if (curnode.ModelTSL) {
        ModelTSL = JSON.parse(curnode.ModelTSL);

        if (ModelTSL.properties) {
          arr = JSON.parse(JSON.stringify(ModelTSL.properties));
        }
        if (ModelTSL.tags) {
          let arrCode = [];
          if (arr.length > 0) {
            arrCode = arr.map((its) => its.code);
          }
          for (let i = 0; i < ModelTSL.tags.length; i++) {
            if (!arrCode.includes(ModelTSL.tags[i].code)) {
              arr.push(ModelTSL.tags[i]);
            }
          }
        }
      }

      this.productmap.set(node.Id, curnode);
      this.productCodemap.set(node.Id, arr);
    },
    async getproductList() {
      this.prodloading = true;
      let response = await myProductList(this.queryform)
      if (response.code == 0) {
        if (response.data && response.data.List) {
          this.productLists = response.data.List;
        }
        this.choiceProduct();
      }
      this.prodloading = false;
    },
    async remoteMethod(query) {
      if (query !== '') {
        this.queryform.Name = query;
        await this.getproductList();
      }else{
        delete this.queryform.Name;
        await this.getproductList();
      }
    },
    addSynclist() {
      //增加关联值绑定
      let obj = {
        code: "", //产品物模型的属性标识或标签标识
        field_id: "", //表单字段ID
      };
      this.value.synclist.push(obj);
    },
    delSynclist(inx) {
      //移除关联值绑定
      this.value.synclist.splice(inx, 1);
    },
  },
};
</script>

<style lang="less">
.synclistForm {
  width: 100%;

  // border: 1px solid #dddddd;
  .el-form-item {
    width: 40%;
    box-sizing: border-box;
    margin-right: 0;
    margin-bottom: 5px;
  }

  .el-form-item.butt_item {
    width: 20%;
    box-sizing: border-box;
    margin-right: 0;
    margin-bottom: 5px;
  }

  .el-form-item .el-form-item__label {
    padding: 0;
    color: #7a7a7a;
  }

  .el-form--label-top .el-form-item__label {
    padding-bottom: 0;
  }
}

.synclistForm.el-form--inline .el-form-item {
  margin-right: 0;
}
</style>
