<template>
  <div style="padding:20px 20px 0 20px" id="big_con">

    <div style="background:#ffffff;border-radius:10px">
      <div style="padding:10px 20px 0">
        <el-tabs v-model="activeName" @tab-click="handleClick" lazy class="type_tabs">
          <el-tab-pane label="全部" name="all" key="typeall">
            <div slot="label" class="product_type_title">
              <i class="icon el-icon-date"></i><span>全部</span>
              <!-- <i class="el-icon-more right_icon"></i> -->
              <div class="line"></div>
            </div>
          </el-tab-pane>
          <el-tab-pane :label="item.Name" :name="item.Id" v-for="item in productTypeList" :key="'type' + item.Id">
            <div slot="label" class="product_type_title">
              <img class="icon" :src="item.PhotoUrl" alt=""><span>{{ item.Name }}</span>
              <el-dropdown class="product_type_dropdown" @command="handleMenuSelect($event, item)">
                <span class="el-dropdown-link">
                  <i class="el-icon-more right_icon"></i>
                </span>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="edit">编辑</el-dropdown-item>
                  <el-dropdown-item command="del">删除</el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
              <div class="line"></div>
            </div>
          </el-tab-pane>
          <el-tab-pane label="" name="add_icon" :disabled="true">
            <div slot="label" class="product_type_title" style="cursor: pointer;" @click="openViewList">
              <i class="el-icon-plus" style="color:#333333"></i>
            </div>
          </el-tab-pane>
        </el-tabs>
      </div>
      <div class="prop_ul" v-if="activePropsList && activePropsList.length > 0">
        <div class="prop_li" @click="setFilterProp('')" :key="'typeprop'" :class="{ 'active_prop': activeProp == '' }">
          全部
        </div>
        <div class="prop_li" @click="setFilterProp(it)" v-for="it in activePropsList" :key="'typeprop' + it"
          :class="{ 'active_prop': it == activeProp }">{{ it }}</div>
      </div>
      <el-row :gutter="20">
        <!--产品数据-->
        <el-col :span="24" :xs="24">
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="createdProduct">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">创建产品</span>
                  </el-button>
                </el-col>
                <!-- <el-col :span="1.5">
                  <el-button type="primary" plain @click="onClear">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left:6px">导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="onClear">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col> -->
              </div>
              <div>
                <el-col :span="1.5">
                  <el-input @input="getList" v-model="queryParams.key" placeholder="请输入产品关键字" clearable>
                    <i slot="suffix" class="el-input__icon el-icon-search"></i>
                  </el-input>
                </el-col>
                <el-col :span="1.5">
                  <filterPopover :filterFiledList="productFiledList" @setSaveFilterList="setSaveFilterList"
                    @finishSelect="finishSelect"></filterPopover>
                </el-col>
              </div>

            </el-row>

            <el-table v-loading="loading" border :data="dateTableList" :row-style="isRed"
              @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty"
              style="width:100%" :fit="true">
              <template v-for="ite in activeFiledList">
                <el-table-column :fixed="ite.isFixed ? 'left' : false" v-if="ite.isShow" :key="ite.field"
                  :label="ite.fieldName" align="center" :prop="ite.field" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span v-if="ite.field == 'ProductLabel'">{{ returnProductLabelName(scope.row[ite.field])}}</span>
                    <div v-else v-html="ingetFieldShow(scope.row, ite)"></div>
                  </template>
                </el-table-column>
              </template>

              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                  <el-button type="text" icon="el-icon-edit" @click="handleDelete(scope.row)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>
      <productAdd ref="productAdd" @reloadData="getList" :productTypeList="productTypeList"></productAdd>
      <!-- 添加或修改参数配置对话框 -->
      <productViewList ref="viewList" @openAddView="openAddView" @afterSave="loadProductTypeList"></productViewList>
      <productViewEdit ref="viewEdit" @afterSave="afterSave"></productViewEdit>
    </div>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import productAdd from './component/productAdd.vue'
import filterPopover from './component/filterPopover.vue'
import productViewList from './component/productViewList.vue'//分组列表
import productViewEdit from './component/productViewEdit.vue'//分组编辑
import { factoryProductListPost, factoryProductRemove, factoryProductTypeListGet, factoryProductTypeInfo, factoryProductTypeRemove } from '@/api/factory/product'
import { orgFormFields } from '@/api/factory/customFields'
import { getFieldShow } from '@/utils/field.js'
export default {
  name: 'AdminUiProductlist',
  mixins: [resizeTableCon],
  components: { productAdd, filterPopover, productViewList, productViewEdit },
  data() {
    return {
      activeIndex: '1',
      total: 0,
      activeName: 'all',
      queryParams: {
        key: '',
        pageNum: 1,
        pageSize: 10,
        prop: undefined
      },
      dateRange: [],
      showSearch: true,
      dateTableList: [],
      loading: false,
      ids: [],//选择的产品
      productTypeList: [],//产品分组列表
      productFiledList: [],//产品的所有字段
      activePropsList: [],//产品分组对应的属性
      activeProp: '',
      activeFiledList: [],//当前分类显示的产品
      activeFilter: [],//筛选框的筛选
      typeConditionJson: [],//分组的过滤筛选
    };
  },

  mounted() {
    this.activeFiledList = [
      { "field": "SkuNumber", "fieldName": "产品编号", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "ProductName", "fieldName": "产品名称", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "ProductLabel", "fieldName": "产品标签", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "TypeName", "fieldName": "产品分组", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "Prop", "fieldName": "产品属性", "type": "文本", "isShow": true, "isFixed": false },
      { "field": "Total", "fieldName": "总计量", "type": "数字", "isShow": true, "isFixed": false },
      { "field": "Price", "fieldName": "成本单价", "type": "数字", "isShow": true, "isFixed": false },
      { "field": "SalesPrice", "fieldName": "销售单价", "type": "数字", "isShow": true, "isFixed": false }
    ]
    this.getList()
    this.loadOrgFormFields('产品', true)
    this.loadProductTypeList()
  },

  methods: {
    ingetFieldShow(obj, field) {
      return getFieldShow(obj, field);
    },
    returnProductLabelName(val) {
      switch (val) {
        case 'M':
          return '原材料';
        case 'F':
          return '成品';
        case 'U':
          return '半成品';
      }
    },
    setFilterProp(val) {
      //设置进行过滤的类型属性
      this.activeProp = val
      if (this.activeProp) {
        this.queryParams.prop = val
      } else {
        delete this.queryParams.prop
      }
      this.queryParams.pageNum = 1
      this.getList()
    },
    finishSelect(items) {//进行筛选
      this.activeFilter = items ? JSON.parse(JSON.stringify(items)) : []
      if (items && items.length > 0) {
        this.queryParams.items = [...items, ...this.typeConditionJson]
      } else {
        if (this.typeConditionJson && this.typeConditionJson.length > 0) {
          this.queryParams.items = [...this.typeConditionJson]
        } else {
          delete this.queryParams.items
        }
      }

      this.queryParams.pageNum = 1
      this.getList()
    },
    setSaveFilterList(filterList) {
      //筛选另存为新分组
      this.$refs.viewEdit.setSaveFilter(filterList)
    },
    async loadProductTypeList() {//产品分组
      try {
        let res = await factoryProductTypeListGet()
        this.productTypeList = JSON.parse(JSON.stringify(res.data))
      } catch (error) {
        console.log(error, 'error');
      }
    },
    async afterSave() {
      //类型保存完成
      await this.loadProductTypeList()
      await this.$refs.viewList.loadProductTypeList()//加载类型列表
    },
    handleMenuSelect(val, item) {//下拉菜单的点击对应菜单事件
      // console.log("点击下拉",val,item);
      if (val == 'edit') {
        this.openAddView(item.Id)
      } else if (val == 'del') {
        this.deleteType(item)
      }
    },
    openAddView(id) {//打开视图编辑弹窗
      this.$refs.viewEdit.setDrawOpen(id)
    },
    deleteType(item) {
      let that = this
      this.$modal.confirm('是否确认删除产品分组"' + item.Name + '"？').then(function () {
        return factoryProductTypeRemove({ id: item.Id });
      }).then(() => {
        that.$modal.msgSuccess("移除成功");
        return that.loadProductTypeList();

      }).catch((err) => {
        console.log("错误", err);
      });
    },
    async openViewList() {
      //打开视图弹窗
      await this.$refs.viewList.setDrawOpen()
    },
    setSearchQuery() {
      //设置搜索
    },
    async loadOrgFormFields(field, ext) {//获取产品字段列表
      let res = await orgFormFields({ field: field, ext })
      // console.log("字段列表",res);
      this.productFiledList = JSON.parse(JSON.stringify(res.data))
    },
    createdProduct() {
      this.$refs.productAdd.openDialog()//打开添加产品的弹窗
    },
    async handleClick() {
      //切换标签
      try {
        if (this.activeName !== 'all') {
          let res = await factoryProductTypeInfo({ id: this.activeName })
          let findTypeRow = res.data
          // console.log('findTypeRow',findTypeRow.ListFieldsJson);
          this.activePropsList = findTypeRow.PropList ? findTypeRow.PropList.split(',') : []//属性
          this.activeFiledList = findTypeRow.ListFieldsJson ? JSON.parse(findTypeRow.ListFieldsJson) : []//字段相关设置
          if (findTypeRow) {
            let ConditionJson = findTypeRow.ConditionJson ? JSON.parse(findTypeRow.ConditionJson) : []
            if (ConditionJson && ConditionJson.length > 0) {
              ConditionJson = ConditionJson.map(rw => {
                if (rw.val_num || rw.val_num == 0) { } else {
                  delete rw.val_num
                }
                if (rw.val_arr && rw.val_arr.length > 0) { } else {
                  delete rw.val_arr
                }
                return rw
              })
            }
            this.typeConditionJson = JSON.parse(JSON.stringify(ConditionJson))
            this.queryParams.items = [...this.activeFilter, ...ConditionJson]
          } else {
            this.typeConditionJson = []
          }

          // this.queryParams.typeId = this.activeName
          delete this.queryParams.prop
          this.queryParams.pageNum = 1
          this.getList()

        } else {
          this.activeFiledList = [
            { "field": "SkuNumber", "fieldName": "产品编号", "type": "文本", "isShow": true, "isFixed": false },
            { "field": "ProductName", "fieldName": "产品名称", "type": "文本", "isShow": true, "isFixed": false },
            { "field": "ProductLabel", "fieldName": "产品标签", "type": "文本", "isShow": true, "isFixed": false },
            { "field": "TypeName", "fieldName": "产品分组", "type": "文本", "isShow": true, "isFixed": false },
            { "field": "Prop", "fieldName": "产品属性", "type": "文本", "isShow": true, "isFixed": false },
            { "field": "Total", "fieldName": "总计量", "type": "数字", "isShow": true, "isFixed": false },
            { "field": "Price", "fieldName": "成本单价", "type": "数字", "isShow": true, "isFixed": false },
            { "field": "SalesPrice", "fieldName": "销售单价", "type": "数字", "isShow": true, "isFixed": false }
          ]
          this.activePropsList = []
          this.typeConditionJson = []
          // delete this.queryParams.typeId
          if (this.activeFilter && this.activeFilter.length > 0) {
            this.queryParams.items = this.activeFilter
          } else {
            delete this.queryParams.items
          }
          delete this.queryParams.prop
          this.getList()
        }

      } catch (error) {
        console.log("报错了", error);
      }
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    onClear() {

    },
    getList() {
      factoryProductListPost(this.queryParams).then(res => {
        // console.log("查询到",res);
        this.dateTableList = res.data.List
        this.total = res.data.Total
      })
    },
    resetQuery() {

    },
    handleQuery() {

    },
    async handleUpdate(row) {
      //修改产品
      // console.log("修改产品",row,row.Id);
      await this.$refs.productAdd.openDialog(row.Id)//打开添加产品的弹窗
    },
    handleDelete(row) {
      let that = this;
      //删除
      this.$modal.confirm('是否确认删除产品"' + row.ProductName + '"？').then(function () {

        return factoryProductRemove({ id: row.Id });
      }).then(() => {
        that.getList();
        that.$modal.msgSuccess("移除成功");
      }).catch((err) => {
        console.log("错误", err);
      });
    }
  },
};
</script>
<style lang="scss" scoped>
::v-deep.product_type_menu.el-menu.el-menu--horizontal {
  border-bottom: none;
}

.type_tabs {
  ::v-deep .el-tabs__item {
    padding-right: 0;
  }

  ::v-deep .el-tabs__active-bar {
    margin-left: -10px;
  }
}

.prop_ul {
  display: flex;
  align-items: center;

  .prop_li {
    color: #666;
    cursor: pointer;
    background-color: #f7f8f8;
    border-radius: 16px;
    padding: 8px 16px;
    font-size: 14px;
    line-height: 14px;
    text-align: center;
    margin-left: 20px;

    &.active_prop {
      background-color: rgba(24, 144, 255, 0.3);
      color: #3572FF;
    }
  }
}

.product_type_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;
  position: relative;

  .icon {
    width: 16px;
    height: 16px;
    margin-right: 5px;
    font-size: 16px;
  }

  .right_icon {
    width: 20px;
    height: 20px;
    margin-left: 10px;
    transform: rotate(90deg);
    /* 旋转90度 */
    /* 可选：如果你想让元素保持其原始大小，可以同时应用transform-origin */
    transform-origin: center center;
    /* 旋转中心点在元素中心 */
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 2px;
  }

  .right_icon:hover {
    background: #EEEEEE;
    color: #333;
  }

  .line {
    margin-left: 8px;
    width: 2px;
    height: 22px;
    background: #EEEEEE;
  }
}
</style>