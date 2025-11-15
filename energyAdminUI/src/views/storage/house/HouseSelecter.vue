<template>
  <div>
    <el-dialog width="620px" title="请选择目标仓库" :visible.sync="houseOpen" :close-on-click-modal="false" append-to-body>
      <el-form :model="houseQuery" ref="queryForm" :inline="true" style="display: flex;justify-content: space-between;">
        <el-form-item label="仓库名称" prop="Name">
          <el-input v-model="houseQuery.Name" placeholder="请输入仓库名称" clearable></el-input>
        </el-form-item>
        <el-form-item class="submit_button_con">
          <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="loadHouseList">搜索</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="houseOptions" tooltip-effect="dark" style="width: 100%" highlight-current-row
        v-loading="houseLoading" @current-change="onSelected">
        <el-table-column prop="StoreName" align="left" label="仓库名称"></el-table-column>
        <el-table-column label="仓库类型" align="center" width="120">
          <template slot-scope="scope">
            <el-tag type="success" v-if="scope.row.IsSystem == 1">系统</el-tag>
            <el-tag type="info" v-else>一般</el-tag>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="houseTotal > 0" :total="houseTotal" :page.sync="houseQuery.pageNum"
        :limit.sync="houseQuery.pageSize" @pagination="loadHouseList" />
    </el-dialog>

  </div>
</template>
      
<script>
import { houseList } from "@/api/storage/house";
export default {
  name: "HouseSelecter",
  data() {
    return {
      title: "",
      houseOpen: false,
      houseLoading: false,
      houseOptions: [],
      houseTotal: 0,
      houseQuery: { Name: undefined, Status: "1", PageNum: 1, PageSize: 20 },
    };
  },
  mounted() {

  },
  methods: {
    openHouseDialog(title) {
      this.title = title;
      this.houseOpen = true;
      this.loadHouseList();
    },
    resetQuery() {
      this.resetForm("queryForm");
      this.loadHouseList();
    },
    loadHouseList() {
      this.houseLoading = true;
      houseList(this.houseQuery).then(rsp => {
        this.houseOptions = rsp.data.List;
        this.houseTotal = rsp.data.Total;
        this.houseLoading = false;
      });
    },
    onSelected(val) {
      if(val!=null){
        this.$emit("ok", val);
        this.houseOpen = false;
      }

    }
  },
};
</script>
<style lang="scss" scoped></style>