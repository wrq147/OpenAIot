<template>
  <div :class="{'hidden':hidden}" class="pagination-container">
    <el-pagination
      :background="background"
      :current-page.sync="currentPage"
      :page-size.sync="pageSize"
      :layout="layout"
      :page-sizes="pageSizes"
      :pager-count="pagerCount"
      :total="total"
      v-bind="$attrs"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />
  </div>
</template>

<script>
import { scrollTo } from '@/utils/scroll-to'

export default {
  name: 'Pagination',
  props: {
    total: {
      required: true,
      type: Number
    },
    page: {
      type: Number,
      default: 1
    },
    limit: {
      type: Number,
      default: 20
    },
    pageSizes: {
      type: Array,
      default() {
        return [10, 20, 30, 50]
      }
    },
    // 移动端页码按钮的数量端默认值5
    pagerCount: {
      type: Number,
      default: document.body.clientWidth < 992 ? 5 : 7
    },
    layout: {
      type: String,
      default: 'total, sizes, prev, pager, next, jumper'
    },
    background: {
      type: Boolean,
      default: true
    },
    autoScroll: {
      type: Boolean,
      default: true
    },
    hidden: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    currentPage: {
      get() {
        return this.page
      },
      set(val) {
        this.$emit('update:page', val)
      }
    },
    pageSize: {
      get() {
        return this.limit
      },
      set(val) {
        this.$emit('update:limit', val)
      }
    }
  },
  methods: {
    handleSizeChange(val) {
      this.$emit('pagination', { page: this.currentPage, limit: val })
      if (this.autoScroll) {
        scrollTo(0, 800)
      }
    },
    handleCurrentChange(val) {
      this.$emit('pagination', { page: val, limit: this.pageSize })
      if (this.autoScroll) {
        scrollTo(0, 800)
      }
    }
  }
}
</script>

<style lang="scss">
.pagination-container {
  /* background: #fff; */
  padding: 10px 20px;
  border-radius: 4px !important;
  overflow-x: auto;
  width: 100%;
}
.pagination-container.hidden {
  display: none;
}

.el-pagination.set_page_radius{
  font-weight: normal;
  .el-pagination__jump{
    margin-left: 15px;
    // font-size: 16px;
    // .el-input .el-input__inner{
    //   font-size: 16px;
    // }
  }
  .el-pagination__sizes{
    margin-right: 5px;
    
    // .el-input .el-input__inner{
    //   font-size: 16px;
    // }
  }
  // .el-pagination__total{
  //   font-size: 16px;
  // }
}
.el-pagination.set_page_radius input{
  border-radius: 4px !important;
}
.el-pagination.is-background .btn-prev, .el-pagination.is-background .btn-next{
  background-color: rgba(34, 46, 64, 1) !important;
  border-radius: 4px !important;
  // font-size: 16px;
}
 .el-pagination.is-background .el-pager li{
  background-color: transparent !important;
  border-radius: 4px !important;
  // font-size: 16px;
}
.el-pagination.is-background .el-pager li.active{
  background: rgba(61, 185, 143, 1);
  color: #ffffff;
}

.el-pagination.is-background .el-pager li:not(.disabled).active{
  background: rgba(61, 185, 143, 1) !important;
}
 .el-pagination.is-background button i{
     color: rgba(255, 255, 255, 0.6);
  }
 .el-pagination.is-background button[disabled='disabled'] i{
    color: rgba(255, 255, 255, 0.2);
  }
</style>
