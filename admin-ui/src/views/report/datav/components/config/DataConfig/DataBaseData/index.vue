<template>
  <el-dialog
    v-if="dialogFlag"
    :title="title"
    :visible.sync="dialogFlag"
    :close-on-click-modal="false"
    width="1500px"
    top="2vh"
    @close="cancel"
  >
    <div class="databaseBox">
      <database-left ref="databaseLeft" @sourseDataList="sourseDataList" @sourseDataId="sourseDataId" />
      <custom-sql
        ref="customSql"
        :dataBase="database"
        :staticValue="chartData"
        :costomData="costomData"
        :sourseList="sourseList"
        :drawingList="drawingList"
        :tableType="tableType"
        @changeDataBase="changeDataBase"
        @changeTimeout="changeTimeout"
      />
    </div>
    <span slot="footer" class="dialog-footer">
      <el-button @click="cancel">取消</el-button>
      <el-button type="primary" @click="dataBaseConfirm">确定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import CustomSql from "./CustomSql";
import databaseLeft from "./databaseLeft";
export default {
  props: {
    dialogVisible: {
      type: Boolean,
    },
    title: {
      type: String,
      required: true,
    },
    costomData: {
      type: Object,
      required: true,
    },
    drawingList: {
      type: Array,
      required: true,
    },
    tableType: {
      type: String,
      default: "default",
    }
  },
  components: {
    CustomSql,
    databaseLeft,
  },
  data() {
    return {
      dialogFlag: false,
      dataBaseType: this.baseType,
      chartData: this.costomData,
      database: {},
      loading: true,
      sourseList: [],
      baseDataTotal: []
    };
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
      (this.database = {
        ...this.costomData.chartOption.database,
        fieldName: "",
        chartStucture: {
          legend: [],
          coordinate: [],
          statisticsType: "",
        },
      })
    },
  },
  methods: {
    // 编辑时重置数据方法
    initCom(tmpoption) {
      this.$refs.customSql.initCom(tmpoption)
      this.$refs.databaseLeft.initCom(tmpoption)
    },

    // 编译器返回的数据
    changeDataBase(data) {
      this.baseDataTotal = data
    },

    // 点击弹窗确定
    dataBaseConfirm() {
      console.log(this.baseDataTotal)
      this.$emit("changeDataBase", this.baseDataTotal);
    },

    // 返回的数据源列表数据
    sourseDataList(data) {
      this.sourseList = data;
    },
    // 执行选择数据源
    sourseDataId(id) {
      this.$refs.customSql.sourseChange(id);
    },
    cancel() {
      this.$emit("cancelForm");
    },
    changeTimeout(sqltimeout, apiTime) {
      this.baseDataTotal.timeout = apiTime;
      this.baseDataTotal.database.cacheTime = sqltimeout;
    },
  },
};
</script>
<style lang="scss" scoped>
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }
  .el-input-number {
    width: 100%;
  }
  .el-dialog__body {
    background-color: #f1f2f4;
  }
}
.databaseBox {
  display: flex;
  justify-content: space-between;
}
</style>
