<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
      <div>
        <div class="from_con" id="from_con" v-show="showSearch">
          <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
  
            <el-form-item label="存储类型" prop="TargetType">
              <el-select v-model="queryParams.TargetType" clearable placeholder="请选择存储类型">
                <el-option label="半成品" value="0"></el-option>
                <el-option label="成品" value="1"></el-option>
              </el-select>
            </el-form-item>
  
  
            <el-form-item label="关键词" prop="Key">
              <el-input placeholder="请输入搜索的设备、耗材名称或唯一编号" style="width: 300px;" v-model="queryParams.Key" clearable>
              </el-input>
            </el-form-item>
  
            <!-- <el-col class="float_right" :span="24"> -->
            <el-form-item class="submit_button_con">
              <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
              <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            </el-form-item>
            <!-- </el-col> -->
          </el-form>
        </div>
        <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
          <el-row :gutter="10" class="mb8 button_row">
            <div>
              <el-col :span="1.5">
                <span style="margin-right:15px;font-size: 14px;color: #1890ff;">当前仓库</span>
                <el-select v-model="queryParams.HouseId" filterable clearable placeholder="请输入仓库" @change="changeHouse">
                  <el-option v-for="item in houseOptions" :key="item.Id" :label="item.StoreName" :value="item.Id">
                  </el-option>
                </el-select>
              </el-col>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="setEdit()" v-loading="submitLoading" :disabled="submitLoading">
                    <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                    <span style="margin-left:6px">{{!isEditWarn?'编辑':'取消'}}</span>
                </el-button>
              </el-col>
              <el-col :span="1.5" v-if="isEditWarn">
                <el-button type="success" plain @click="saveEdit()" v-loading="submitLoading" :disabled="submitLoading">
                    <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                    <span style="margin-left:6px">保存</span>
                </el-button>
              </el-col>
            </div>
  
          </el-row>
  
          <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
            style="width:100%" row-key="Id">
  
            <el-table-column label="物品编号" align="center" prop="DeviceNumber"></el-table-column>
            <el-table-column label="物品名称" align="left" prop="Name"></el-table-column>
            <el-table-column label="存储类型" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.TargetType == 0 ? "半成品" : "成品" }}</span>
              </template>
            </el-table-column>
            <el-table-column label="所在仓库" align="center" prop="StoreName"></el-table-column>
            <el-table-column label="库存数量" align="center" prop="Quantity"></el-table-column>
            <el-table-column label="库存上线" align="center" prop="MaxNum">
                <template slot-scope="scope">
                    <el-input v-if="isEditWarn" type="number" placeholder="请输入库存上线" v-model="scope.row.MaxNum" :min="-1" :step="1"></el-input>
                    <span v-else>{{scope.row.MaxNum}}</span>
                </template>
            </el-table-column>
            <el-table-column label="库存下限" align="center" prop="MinNum">
                <template slot-scope="scope">
                    <el-input v-if="isEditWarn" type="number" placeholder="请输入库存下限" v-model="scope.row.MinNum" :min="-1" :step="1"></el-input>
                    <span v-else>{{scope.row.MinNum}}</span>
                </template>
            </el-table-column>
            <el-table-column label="预警" align="center">
                <template slot-scope="scope">
                    <span style="color:red">{{getWarnValue(scope.row)}}</span>
                </template>
            </el-table-column>
            <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                    <el-link icon="el-icon-switch-button" type="primary" @click="handleCancelWarn(scope.row)">取消预警</el-link>
                </template>
            </el-table-column>
          </el-table>
          <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
            @pagination="getList" />
        </div>
      </div>
    </div>
  </template>
    
  <script>
  import {
    stockList,
    setPileWarn
  } from "@/api/storage/stock";

  import {
    houseList
  } from "@/api/storage/house";
  import { resizeTableCon } from "@/mixins/resizeTableCon";
  export default {
    name: "StockList",
    mixins: [resizeTableCon],
    data() {
      return {
        // 导出遮罩层
        exportLoading: false,
        houseOptions: [],
        // 遮罩层
        loading: true,
        // 显示搜索条件
        showSearch: true,
        // 表格树数据
        tbList: [],
        // 查询参数
        queryParams: {
          pageNum: 1,
          pageSize: 10,
          TargetType: undefined,
          HouseId: undefined,
          Key: undefined,
          IsWarn:true//过滤预警库存
        },
        // 总条数
        total: 0,
        isEditWarn:false,
        submitLoading:false
      };
    },
    async created() {
      let rsp = await houseList({ showAll: true });
      this.houseOptions = rsp.data.List;
      if (this.houseOptions.length > 0) {
        let hidd = this.$cache.local.get("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid);
        this.queryParams.HouseId = this.houseOptions[0].Id;
        if (hidd != null && hidd != '' && this.houseOptions.some(x => x.Id == hidd)) {
          this.queryParams.HouseId = hidd;
        }
      }
      this.getList();
    },
    methods: {
        saveEdit(){
            //保存预警修改
            this.submitLoading=true
            let subForm=[]
            subForm=this.tbList.map(row=>{
                let obj={
                    houseId:row.HouseId,
                    targetType:row.TargetType,
                    targetId:row.TargetId,
                    minNum:row.MinNum,
                    maxNum:row.MaxNum
                }
                return obj
            })
            setPileWarn(subForm).then(res=>{
                this.$modal.msgSuccess("更新成功");
                this.submitLoading=false
                this.isEditWarn=false
                this.queryParams.pageNum=1
                this.getList()
            }).catch(err=>{
                console.log("报错");
                this.submitLoading=false
            })
        },
        handleCancelWarn(row){
            let subForm=[{
                houseId:row.HouseId,
                targetType:row.TargetType,
                targetId:row.TargetId,
                minNum:-1,
                maxNum:-1
            }]
            setPileWarn(subForm).then(res=>{
                this.$modal.msgSuccess("取消预警成功");
                this.queryParams.pageNum=1
                this.getList()
            }).catch(err=>{
                console.log("报错");
            })
        },
        setEdit(){
            this.isEditWarn=!this.isEditWarn
        },
        getWarnValue(row){
            //获取单行的预警值
            if(row.MaxNum!=-1){
                if(row.Quantity>row.MaxNum){
                    return '高于库存上线'
                }
            }
            if(row.MinNum!=-1){
                if(row.Quantity<row.MinNum){
                    return '低于库存下限'
                }
            }
        },
      /** 查询列表 */
      getList() {
        this.loading = true;
        stockList(this.queryParams).then(response => {
          this.tbList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        });
      },
      changeHouse(val) {
        this.$cache.local.set("curhouseId-" + this.$store.state.user.orgId + "-" + this.$store.state.user.uid, val);
        this.getList();
      },
      /** 搜索按钮操作 */
      handleQuery() {
        this.getList();
      },
      /** 重置按钮操作 */
      resetQuery() {
        this.resetForm("queryForm");
        this.handleQuery();
      },
    }
  };
  </script>
  <style lang="scss">
  .imgwrap {
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  
    .el-image {
      display: flex;
      width: 80px;
      height: 80px;
      justify-content: center;
      align-items: center;
    }
  }
  </style>