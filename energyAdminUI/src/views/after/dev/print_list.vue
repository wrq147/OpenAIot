<template>
  <el-dialog
  v-loading="loading"
    title="选择打印模板"
    :visible.sync="open"
    center
    width="600px"
    :close-on-click-modal="false"
    :destroy-on-close="true"
  >
  <div class="print_ul">
    <div class="print_li" @click="choiceTemplete(item)" v-for="item in tbList" :key="item.Id">{{item.Name}}</div>
  </div>
  <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
  </el-dialog>
</template>

<script>
import {
  listPrintTemplate,
  deletePrintTemplate,
  dataList
} from "@/api/report/printTemplate";
export default {
  name: "AdminUiPrintList",

  data() {
    return {
      open: false,
      queryParams:{
        pageNum: 1,
        pageSize: 10,
      },
      total:0,
      loading:false,
      tbList:[]
    };
  },

  mounted() {},

  methods: {
    choiceTemplete(row){
        this.open=false
        this.$emit('choiceTemplete',row.Id)
    },
    openDia(){
        this.open=true
        this.getList()
    },
    getList() {
      this.loading=true;
      listPrintTemplate(this.queryParams).then(rsp=>{
        this.tbList=rsp.data.List;
        this.total = rsp.data.Total;
        this.loading=false;
      });
    },
  },
};
</script>
<style lang="less" scoped>
.print_ul{
    .print_li{
        width: 100%;
        height: 40px;
        border-radius: 4px;
        background: #EFEFEF;
        line-height: 40px;
        padding-left: 20px;
        box-sizing: border-box;
        font-size: 16px;
        margin-top: 10px;
        cursor: pointer;
    }
}
</style>
