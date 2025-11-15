<template>
    <div>
      <el-form class="huancun_con" label-position="right" label-width="120px">
        <el-form-item label="属性标识：">
          <el-select v-model="config.PropCode" placeholder="请选择属性" @change="config.Express = 'data'">
            <el-option v-for="item in propItems" :key="item.code" :label="item.name" :value="item.code"></el-option>
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
  import ExpressEditor from "../../ExpressEditor.vue";
  export default {
    name: "SetPropNodeConfig",
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
      propItems() {
        return this.$store.state.rulesFlowable.rulesProductAttr;
      },
    },
    data() {
      return {
        loading: false,
        expressOpen: false,
        dlgExpress: ""
      };
    },
    async mounted() {
    },
    methods: {
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
  
  