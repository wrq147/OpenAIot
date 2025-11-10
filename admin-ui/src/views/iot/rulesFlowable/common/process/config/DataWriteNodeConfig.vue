<template>
  <div>
    <div v-if="TriggerWay == 0" style="margin-bottom: 30px;">
      <div style="display: flex;flex-direction: row;align-items: center;justify-content: space-between;">
        <span style="font-size: 15px;color: #666;">
          【聚合数据改参】
        </span>
        <el-button type="text" @click="onAddJPAc" style="margin-left: 20px;">+ 添加</el-button>
      </div>
      <div class="dw-ac-bg" v-if="config.Counts != null && config.Counts.length > 0">
        <div v-for="(acitem, idx) in config.Counts" :key="idx"
          style="border-bottom: dashed 1px #dadada;margin-bottom: 10px;padding-bottom: 10px;">
          <el-select placeholder="选择参数" v-model="acitem.code" style="width:120px;" @change="acitem.express = 'data'">
            <el-option :label="ppite.name" :value="ppite.code" v-for="ppite in ParamList" :key="ppite.code"></el-option>
          </el-select>
          <span style="margin:0 10px;">=</span>
          <span style="display:inline-block; width:200px;margin-right: 15px;font-size: 14px;">{{ jpToName(acitem) }}</span>
          <el-link icon="el-icon-edit" @click="popEditCC(acitem)" style="margin-right: 10px;cursor: pointer;"></el-link>
          <el-link icon="el-icon-delete" @click="onDelCC(idx)"
            style="color: #ff0000;margin-right: 10px;cursor: pointer;"></el-link>
        </div>
      </div>
    </div>
    <div>
      <div style="display: flex;flex-direction: row;align-items: center;justify-content: space-between;">
        <div style="display: flex;flex-direction: row; align-items: center;">
          <span style="font-size: 15px;color: #666;">【表达式改参】</span>
          <el-popover placement="top-start" title="例子" width="200" trigger="hover">
            <p>参数表达式格式：<br />
              data+param("TriggerDelta")<br />
              data+3<br />
              data*2+3</p>
            <div slot="reference" style="font-size: 14px;color:#999;margin-left: 10px;">注：表达式说明<i class="el-icon-info"
                style="margin-left: 10px;"></i></div>
          </el-popover>
        </div>
        <el-button type="text" @click="onAddAction">+ 添加</el-button>
      </div>
      <div class="dw-ac-bg" v-if="config.Writes.length > 0">
        <div v-for="(acitem, idx) in config.Writes" :key="idx"
          style="border-bottom: dashed 1px #dadada;margin-bottom: 10px;padding-bottom: 10px;">
          <el-select placeholder="选择参数" v-model="acitem.code" style="width:120px;" @change="acitem.express = 'data'">
            <el-option :label="ppite.name" :value="ppite.code" v-for="ppite in ParamList" :key="ppite.code"></el-option>
          </el-select>
          <span style="margin:0 10px;">=</span>
          <el-input placeholder="请输入表达式" v-model="acitem.express" style="width:200px;margin-right: 15px;"></el-input>
          <el-link icon="el-icon-edit" @click="popExpressDlg(acitem)"
            style="margin-right: 10px;cursor: pointer;"></el-link>
          <el-link icon="el-icon-delete" @click="onDelAction(idx)"
            style="color: #ff0000;margin-right: 10px;cursor: pointer;"></el-link>
        </div>
      </div>
    </div>

    <el-dialog title="数据统计" append-to-body :close-on-click-modal="false" :visible.sync="countOpen" width="500px">
      <div v-if="editCountItem != null">
        <el-form label-width="120px">
          <el-form-item label="统计">
            <el-select v-model="editCountItem.calway" placeholder="请选择统计方式">
              <el-option label="合计" value="sum"></el-option>
              <el-option label="平均" value="average"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="聚合数据">
            <el-select v-model="editCountItem.nodeId" placeholder="请选择聚合数据">
              <el-option v-for="(node, i) in nodeOptions" :key="i" :label="node.name" :value="node.id"></el-option>
            </el-select>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="countOpen = false;">取 消</el-button>
        <el-button type="primary" @click="confirmCount">确 定</el-button>
      </div>
    </el-dialog>

    <el-dialog title="编辑表达式" append-to-body :close-on-click-modal="false" :visible.sync="expressOpen" width="670px">
      <div v-if="editAcItem != null">
        <ExpressEditor :content="editAcItem.express" ref="expEd"></ExpressEditor>
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
  name: "DataWriteNodeConfig",
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
    attrsItems() {
      return this.$store.state.rulesFlowable.rulesProductAttr.filter(x=>x.option.type=="int"||x.option.type=="float");
    },
    TriggerWay() {
      return this.$store.state.rulesFlowable.rulesDesign.TriggerWay;
    },
    ParamList() {
      let httpss = this.$store.state.rulesFlowable.rulesDesign.HttpParams;
      if (httpss != null) {
        return httpss;
      }
      else {
        return [];
      }
    },
    nodeOptions() {
      let values = [];
      const excType = [
        "METRONOME",
      ];
      this.$store.state.rulesFlowable.rulesNodeMap.forEach((v) => {
        if (excType.indexOf(v.type) !== -1) {
          values.push({ id: v.id, name: v.name,props:v.props });
        }
      });
      return values;
    },
  },
  data() {
    return {
      expressOpen: false,
      editAcItem: null,
      countOpen: false,
      editCountItem: null
    };
  },
  mounted() {
  },
  methods: {

    onAddAction() {
      this.config.Writes.push({ code: "", express: "" });
    },
    onDelAction(idx) {
      this.$confirm('确定删除该参数赋值吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.$delete(this.config.Writes, idx);
      });
    },
    popExpressDlg(item) {
      this.editAcItem = item;
      this.expressOpen = true;
    },
    confirmExpress() {
      let tmpexpress = this.$refs.expEd.getFormulaStr();
      if (this.$refs.expEd.isExpress(tmpexpress).success == false) {
        this.$message.error("表达式格式错误");
        return;
      }
      this.editAcItem.express = tmpexpress;
      this.editAcItem = null;
      this.expressOpen = false;
    },
    jpToName(item) {
      let tmpnode = this.nodeOptions.filter(x => x.id == item.nodeId);
      if(tmpnode.length==0){
        return "";
      }
      let clstr = "";
      if (item.calway == "sum") {
        clstr = "合计";
      }
      else if (item.calway == "average") {
        clstr = "平均";
      }

      return clstr + "[" + tmpnode[0].name + "]中的" + tmpnode[0].props.FieldName;
    },
    onDelCC(idx) {
      this.$confirm('确定删除该参数赋值吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.$delete(this.config.Counts, idx);
      });
    },
    onAddJPAc() {
      this.editCountItem={ code: "", nodeId: "", prop: "", calway: "" };
      this.config.Counts.push(this.editCountItem);
      this.countOpen = true;
    },
    popEditCC(item){
      this.editCountItem = item;
      this.countOpen = true;
    },
    confirmCount() {
      this.countOpen = false;
      this.editCountItem=null;
    }
  }
};
</script>

<style lang="scss">
.dw-ac-bg {
  background-color: #F5F7FA;
  border: solid 1px #dadada;
  padding: 15px 10px;

  .acrow {
    display: flex;
    flex-direction: row;
  }
}
</style>
