<template>
  <div>
    <el-table ref="multipleTable" v-loading="false" border :data="resultTableList" style="width: 100%" :fit="true" max-height="500">
        <el-table-column :label="item.name" align="left" :key="item.key" :prop="item.key" :show-overflow-tooltip="true" v-for="item in tableColum"></el-table-column>
    </el-table>
    <div style="overflow-x:scroll;">
        <pagination v-show="data.length > 0" :total="data.length" :page.sync="tableForm.pageNum" :limit.sync="tableForm.pageSize" :pageSizes="pageSizes" @pagination="setNextPage"/>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AdminUiTablepage',
  props:{
    tableColum:{
        type:Array,
        default:()=>{
            return []
        }
    },
    data:{
        type:Array,
        default:()=>{
            return []
        }
    }
  },
  watch: {
    data: {
        immediate: true,
        deep: true,
        handler() {
            this.tableForm.pageNum=1
            let dataList=JSON.parse(JSON.stringify(this.data))
            this.resultTableList = dataList.slice(0, Math.min(50, dataList.length));
        },
    }
  },
  data() {
    return {
        pageSizes:[50,100,150,200],
        tableForm:{pageNum:1,pageSize:50},
        resultTableList:[]
    };
  },

  mounted() {
    
  },

  methods: {
    setNextPage(page){
        let dataList=JSON.parse(JSON.stringify(this.data))
        this.resultTableList = dataList.slice(page.limit*(page.page-1), Math.min(page.limit*page.page, dataList.length));
    }
  },
};
</script>