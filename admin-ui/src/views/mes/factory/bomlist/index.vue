<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item prop="Key" label="关键字">
                    <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable />
                </el-form-item>
                <el-form-item label="创建日期">
                    <el-date-picker class="set_radius" v-model="time" style="width:232px" value-format="yyyy-MM-dd"
                    type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" />
                </el-form-item>
                <el-form-item class="submit_button_con">
                  <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                  <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
              <el-row :gutter="10" class="mb8 button_row">
                <div>
                  <el-col :span="1.5">
                    <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增物料清单</el-button>
                  </el-col>
                  <el-col :span="1.5">
                    <el-button :disabled="single" type="success" plain @click="handleAdd(selection[0])">
                      <i class="el-icon-edit"></i>
                      <span style="margin-left:6px">修改</span>
                    </el-button>
                  </el-col>
                  <el-col :span="1.5">
                    <el-button :disabled="single" type="danger" plain @click="handleDelete(selection[0])">
                      <i class="el-icon-delete"></i>
                      <span style="margin-left:6px">删除</span>
                    </el-button>
                  </el-col>
                </div>
              </el-row>
  
              <el-table v-loading="loading" :data="bomList" class="data_table" :row-style="isRed" @selection-change="handleSelectionChange" :header-cell-style="cellSty" style="width:100%" :fit="true">
                <el-table-column type="selection" width="55"></el-table-column>
                <el-table-column label="父物料编码" align="center" prop="Id" :show-overflow-tooltip="true" >
                  <template slot-scope="scope">
                    <el-link @click.stop="handleAdd(scope.row,true)">{{scope.row.SkuNumber}}</el-link>
                  </template>
                </el-table-column>
                <el-table-column label="物料名称" align="center" prop="ProductName" />
                <el-table-column label="创建者" align="center" prop="createName" />
                <el-table-column label="更新者" align="center" prop="updateName" />
                <el-table-column label="创建时间" align="center" prop="createTime" />
                <el-table-column label="更新时间" align="center" prop="updateTime" />
                <!-- <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                  <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                    <el-button type="text" icon="el-icon-delete" style="color:red"
                      @click="handleDelete(scope.row.Id)">删除</el-button>
                  </template>
                </el-table-column> -->
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
          </el-col>
        </el-row>
      </div>
      <!-- 新增/编辑不良品项弹窗 -->
      <add-bom ref="addBom" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    </div>
</template>
  
<script>
  import { bomList, bomRemove,bomInfo } from "@/api/mes/bom";
  import addBom from './cmp/addBom.vue'
  import { resizeTableCon } from "@/mixins/resizeTableCon";
  export default {
    name: "BatchList",
    mixins: [resizeTableCon],
    components: {
      addBom
    },
    data() {
      return {
        loading: false,
        open: false,
        title: '新增物料清单',
        // 查询参数
        queryParams: {
          pageNum: 1,
          pageSize: 20,
          key: '',
          beginTime: '',
          endTime: ''
        },
        time: [],
        total: 0,
        bomList: [],
        selection:[],
        single:true,
        multiple:true,
        ids:[],
      }
    },
    created() {
      this.getList();
    },
    methods: {
      isRed({ row }) {
        let checkIdList = this.ids;
        // console.log("选中的",checkIdList,this.ids,row);
        if (checkIdList.includes(row.Id)) {
          return {
            backgroundColor: "#F6F9FF"
          };
        }
      },
        // 多选框选中数据
      handleSelectionChange(selection) {
        this.selection=selection
        this.ids = selection.map(item => item.Id);
        // console.log("选中的",this.ids);

        this.single = selection.length != 1;
        this.multiple = !selection.length;
      },
      getList() {
        this.open = false;
        this.loading = true;
        if(this.time.length > 0) {
          this.queryParams.beginTime = this.time[0];
          this.queryParams.endTime = this.time[1];
        }
        bomList(this.queryParams).then(response => {
          this.bomList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        })
      },
      /** 搜索按钮操作 */
      handleQuery() {
        this.queryParams.pageNum = 1;
        this.getList();
      },
      /** 重置按钮操作 */
      resetQuery() {
        this.time= []; 
        this.resetForm("queryForm");
        this.handleQuery();
      },
      /** 新增按钮操作 */
      handleAdd(data,isReadonly) {
        if (data === '') {
          this.title = '新增物料清单';
          this.$refs['addBom'].ruleForm = {
            id: '',
            productId: '',
            productName: '',
            productFrom: '',
            specs: '',
            quantity: '',
            items: []
          };
          this.$refs['addBom'].isReadonly=false
        } else {
          if(isReadonly){
            this.title = '物料清单详情';
            bomInfo({id:data.Id}).then((res)=>{
              this.$refs['addBom'].ruleForm = {
                id: res.data.Id,
                productName: res.data.ProductName,
                productId: res.data.ProductId,
                productFrom: '',
                specs: '',
                quantity: '',
                items: res.data.Items
              };
              this.$refs['addBom'].getProductInfo(data.ProductId);
              this.$refs['addBom'].isReadonly=true
            })
          }else{
            this.title = '编辑物料清单';
            bomInfo({id:data.Id}).then((res)=>{
              this.$refs['addBom'].ruleForm = {
                id: res.data.Id,
                productName: res.data.ProductName,
                productId: res.data.ProductId,
                productFrom: '',
                specs: '',
                quantity: '',
                items: res.data.Items
              };
              this.$refs['addBom'].getProductInfo(data.ProductId);
              this.$refs['addBom'].isReadonly=false
            })
          }
        }
        this.open = true;
      },
      /** 删除按钮操作 */
      handleDelete(row) {
        this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
            bomRemove({ id: row.Id }).then(res => {
              this.$message.success('删除成功!')
              this.getList()
            })
        }).catch(() => { })
      },
      cancelForm() {
        this.open = false;
      },
    }
  }
</script>