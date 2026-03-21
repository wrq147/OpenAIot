<template>
  <div>
    <el-form class="huancun_con" label-position="right" label-width="120px">
      <el-form-item label="转发后的协议：">
        <el-select
          v-model="config.ProductId"
          filterable
          remote
          reserve-keyword
          placeholder="请选择转发后的协议"
          :remote-method="remoteMethod"
          :loading="optionLoading"
          clearable
          @change="productChange"
        >
          <el-option
            v-for="item in productLists"
            :key="item.Id"
            :label="item.Name"
            :value="item.Id"
          ></el-option>
        </el-select>
      </el-form-item>
      <div class="params_con" style="margin-bottom: 25px;">
        <el-button type="primary" @click="config.TargetDtuIdsList.push('')"
          >+ 添加目标通信编码</el-button>
          <span style="color: #999;margin-left: 10px;">注：可用$dtuId代表原通信编码</span>
          <div class="params_li" v-for="(dtuId, inx) in config.TargetDtuIdsList" :key="inx">
            <el-input
              placeholder="请输入转发后的通信编码"
              v-model="config.TargetDtuIdsList[inx]"
            ></el-input>
            <div class="delete_con" @click="config.TargetDtuIdsList.splice(inx, 1)">-</div>
          </div>
      </div>
      <div class="params_con" v-if="topicMsgValue == 'PropReply'||topicMsgValue == 'Event'">
        <el-button type="primary" @click="addIdentifier"
          >+ 添加转换标识符</el-button
        >
        <div class="params_li" v-for="(item, inx) in identifierList" :key="inx">
          <el-select
            v-model="item.key"
            filterable
            placeholder="请选择原标识符"
            style="width: 180px"
          >
            <el-option
              v-for="item in paramsOptions"
              :key="item.code"
              :label="item.name"
              :value="item.code"
            >
            </el-option>
          </el-select>
          <span class="marspan">-</span>
          <el-select
            v-model="item.value"
            filterable
            placeholder="请选择目标标识符"
          >
            <el-option
              v-for="item in afterParamsOptions"
              :key="item.code"
              :label="item.name"
              :value="item.code"
            >
            </el-option>
          </el-select>
          <div class="delete_con" @click="deleteIdentifier(inx)">-</div>
        </div>
      </div>


      
    </el-form>
  </div>
</template>

<script>
import { productList, productInfo } from "@/api/rules/productModel";
export default {
  name: "funcNodeConfig",
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  computed: {
    topicMsgValue() {
      if (
        this.$store.state.rulesFlowable.rulesDesign &&
        this.$store.state.rulesFlowable.rulesDesign.TopicMsg &&
        this.$store.state.rulesFlowable.rulesDesign.TopicMsg.value
      ) {
        return this.$store.state.rulesFlowable.rulesDesign.TopicMsg.value; //规则流程设计
      }
    },
    rulesProductAttr() {
      return this.$store.state.rulesFlowable.rulesProductAttr; //规则流程设计
    },
    rulesProductEvent() {
      return this.$store.state.rulesFlowable.rulesProductEvent; //规则流程设计
    },
    rulesProductInfo(){
      return this.$store.state.rulesFlowable.selectProductInfo; //规则所选协议
    }
    // dentifier(){

    //   for(let i =0;i<this.identifierList.length;i++){
    //     this.config.Maping[this.identifierList[i].key]=this.identifierList[i].value
    //   }
    //   console.log("this.config.Maping",this.config.Maping);
    //   return this.identifierList
    // }
  },
  watch: {
    identifierList: {
      handler(val) {
        if (!this.ismountedTime) {
          let mapObj = {};
          for (let i = 0; i < this.identifierList.length; i++) {
            if(this.identifierList[i].key){
              mapObj[this.identifierList[i].key] = this.identifierList[i].value;
            }
          }
          this.config.Maping = JSON.parse(JSON.stringify(mapObj));
        }
      },
      deep: true,
      immediate: true,
    },
  },
  data() {
    return {
      productLists: [],
      productOptionList: [],
      productForm: {
        pageNum: 1,
        pageSize: 100,
        Name: null,
      },
      optionLoading: false,
      paramsOptions: [],
      afterParamsOptions: [],
      identifierList: [],
      choiceMap: new Map(),
      ismountedTime: true,
    };
  },
  async mounted() {
    await this.getNewProductList();
    // this.$nextTick(() => {
    if (this.config.ProductId) {
      this.productChange(this.config.ProductId);
    }

    if (this.topicMsgValue == "PropReply") {
      this.paramsOptions = this.rulesProductAttr;
    }
    if (this.topicMsgValue == "Event") {
      this.paramsOptions = this.rulesProductEvent;
    }
    if (this.config.Maping) {
      this.identifierList = [];
      for (var keys in this.config.Maping) {
        let obj = {
          key: keys,
          value: this.config.Maping[keys],
        };
        this.identifierList.push(obj);
      }
    } else {
      this.identifierList = [];
    }
    this.ismountedTime = false;
    // });
  },
  methods: {
    deleteIdentifier(inx){
      //删除转化的标识符
      this.identifierList.splice(inx,1)
    },
    addIdentifier() {
      let obj = {
        key: "",
        value: "",
      };
      this.identifierList.push(obj);
    },
    productChange(val) {
      //选择协议后
      if (this.choiceMap.get(val)) {
        let modelObj = this.choiceMap.get(val);
        if (this.topicMsgValue == "PropReply") {
          this.afterParamsOptions = modelObj.properties;
        }
        if (this.topicMsgValue == "Event") {
          this.afterParamsOptions = modelObj.events;
        }
      } else {
        productInfo({ id: val }).then((rsp) => {
          let modelObj = JSON.parse(rsp.data.ModelTSL);
          if (this.topicMsgValue == "PropReply") {
            this.afterParamsOptions = modelObj.properties;
          }
          if (this.topicMsgValue == "Event") {
            this.afterParamsOptions = modelObj.events;
          }
          this.choiceMap.set(val, modelObj);
        });
      }
    },
    async remoteMethod(query) {
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.productForm.Name = query;
          await this.getNewProductList();
          this.optionLoading = false;
          this.productLists = this.productOptionList.filter((item) => {
            return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
        }, 200);
      } else {
        this.productLists = this.productOptionList;
        delete this.productForm.Name;
        await this.getNewProductList();
      }
    },
    async getNewProductList() {
      try {
        let rsp = await productList(this.productForm);
        let rspList=[]
        rspList = rsp.data.List.filter((item) => {//过滤非规则所选的协议
          return item.Id!=this.rulesProductInfo.Id;
        });
        if (rsp.code == 0) {
          if (!this.productForm.key) {
            this.productOptionList = rspList;
          }
          this.productLists = JSON.parse(JSON.stringify(rspList));
        }
      } catch (error) {
        console.log("错误", error);
      }
    },
  },
};
</script>

<style lang="less">
.params_con {
  .params_li {
    margin-top: 20px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    .marspan {
      margin: 0 10px;
      color: #dcdfe6;
    }
    .delete_con{
      display: inline-block;
      width: 32px;
      height: 32px;
      display: flex;
      justify-content: center;
      align-items: center;
      border: 1px solid #23A8F2;
      border-radius: 5px;
      margin-left: 10px;
      color: #23A8F2;
    }
  }
}
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
