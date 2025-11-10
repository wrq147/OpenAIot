<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
export default  {
  name:  'App',
    metaInfo() {
        return {
            title: this.$store.state.settings.dynamicTitle && this.$store.state.settings.title,
            titleTemplate: title => {
                return title ? `${title} - ${process.env.VUE_APP_TITLE}` : process.env.VUE_APP_TITLE
            }
        }
    },
}
</script>
<style lang="less">
//公共的表格和表单样式
.app-main {
  background-color: #f0f2f5;
}
.app-main > div { 
  min-height: calc(100vh - 60px);
}
.hasTagsView {
  .app-main > div{
    min-height: calc(100vh - 106px);
  }
}
.app-main.isFixedHea > div {
  overflow: auto;
  height: 100%;
  overflow-x: hidden;
}
.from_con {
  width: 100%;
  box-sizing: border-box;
  padding: 20px;
  background-color: #ffffff;
  margin-bottom: 10px;
  border-radius: 4px;
}
.elbiaoge_elform {
  background-color: #ffffff;
  border-radius: 4px 4px 0 0;
  width: 100%;
  padding: 20px;
  box-sizing: border-box;
}

.mb8.button_row {
  margin-bottom: 0;
}
.set_radius {
  height: 40px;
  line-height: 40px;
  border-radius: 4px;
  vertical-align: middle;
  width: 232px;

  input {
    width: 232px;
    border-radius: 4px;
  }
}
label.el-form-item__label {
  font-weight: 400;
  color: #333333;
}

.el-form.biaodan::after {
  content: "";
  display: block;
  clear: both;
}
.el-form.biaodan .el-form-item {
  margin-right: 0;
  margin-left: 31px;
  margin-bottom: 15px;
  .el-form-item__content {
    line-height: 40px;
    .el-input__inner {
      height: 40px;
      // font-size: 16px;
    }
    // .el-range-input {
    //   font-size: 16px;
    // }
  }
  .el-form-item__label {
    padding-top: 2px;
    line-height: 40px;
    color: #333333;
    // font-size: 16px;
  }

  // margin-bottom: 0;
}
.biaodan.el-form--inline {
  line-height: 32px;
  margin-left: -25px;
  margin-bottom: -15px;
}
.el-form.biaodan .el-form-item .el-button {
  border-radius: 4px;
  height: 40px;
  // width: 100px;
  // font-size: 16px;
  cursor: pointer;
  padding: 10px 30px;

}
.el-form.biaodan .el-button--primary {
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  border: none;
}
// .el-form.biaodan .el-button--mini {
//   padding: 10px 15px;
// }
.el-form.biaodan .submit_button_con {
  float: right;
}
.button_row {
  padding: 5px;
  box-sizing: border-box;
  margin-bottom: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;

  .el-button.is-plain {
    border-radius: 4px;
    padding: 10px 15px;
    border: none;
    color: rgba(53, 114, 255, 1) !important;
    background-color: rgba(246, 249, 255, 1) !important;
    height: 40px;
    min-width: 100px;
    // font-size: 16px;
  }
  .el-button.el-button--danger {
    color: #ff9292 !important;
    background-color: #ffeded !important;
  }
  .el-button.el-button--warning {
    color: rgba(255, 135, 53, 1) !important;
    background-color: rgba(255, 243, 235, 1) !important;
  }
  .el-button.is-plain.is-disabled {
    opacity: 0.6 !important;
  }
  .el-button.el-button--success {
    color: #67C23A !important;
    background: #f0f9eb !important;
  }
}
.button_row.el-row::before,
.button_row.el-row::after {
  display: none;
}
// .data_table.el-table {
//   // font-size: 16px;
// }
.data_table.el-table .el-table__body-wrapper .el-table__empty-block{
  border-bottom: 1px solid rgba(187, 192, 206, 0.1) !important;
}
.data_table.el-table table.el-table__header {
  margin-top: 15px;
}
.data_table.el-table table {
  border: none;
  border-collapse: collapse !important;
  tr.el-table__row {
    border-bottom: 1px solid rgba(187, 192, 206, 0.1) !important;
    th {
      color: #78829d;
      font-weight: 400;
    }
  }
  .fixed-width .cell {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-right: -20px;
    .el-button {
      margin-right: 20px;
    }
    .el-dropdown {
      line-height: 28px;
      padding: 7px 0;
    }
  }
}

.data_table tr td {
  border: none;
  color: #333333;
}
.data_table.el-table th.el-table__cell.is-leaf,
.data_table.el-table td.el-table__cell {
  border: none !important;
}

.data_table td,
.data_table th.is-leaf {
  border: none;
}
.data_table.el-table .el-table__header-wrapper thead th {
  background: #f9fafc;
}
.data_table tr td:nth-last-child(1) {
  border-radius: 0 4px 4px 0 !important;
}
.data_table tr td:first-child {
  border-radius: 4px 0 0 4px !important;
}
.data_table tr th:nth-last-child(2) {
  border-radius: 0 4px 4px 0 !important;
}
.data_table tr th:first-child {
  border-radius: 4px 0 0 4px !important;
}

/* 表格最外层边框-底部边框 */
.data_table.el-table--border::after,
.data_table.el-table--group::after {
  width: 0;
}
.data_table::before {
  width: 0;
}
.data_table .el-table__fixed-right::before,
.data_table.el-table__fixed::before {
  width: 0;
}
.data_table.el-table::before {
  height: 0px;
}
.data_table.el-table--border::after {
  width: 0px;
}
.data_table.el-table--border {
  border: none;
}
.data_table.el-table--group,
.data_table.el-table--border {
  border: none;
  border-bottom: 1px solid rgba(187, 192, 206, 0.1) !important;
}
/* 表格有滚动时表格头边框 */
.data_table.el-table--border th.gutter:last-of-type {
  border-left: none;
  border: none;
}
.data_table.el-table td.el-table__cell > div {
  // font-size: 16px;
  overflow: hidden;
  white-space: nowrap;
  // span{
  //   font-size: 16px;
  // }
  // .el-button--mini{
  //   font-size: 16px;
  // }
}
.data_table.el-table .el-table__header-wrapper {
  thead {
    th {
      background: #f6f9ff !important;
      div {
        font-weight: normal;
        // font-size: 16px;
        overflow: initial;
        white-space: nowrap;
      }
    }
  }
}
.el-dialog {
  border-radius: 4px !important;
}
//公共的表格和表单样式
</style>