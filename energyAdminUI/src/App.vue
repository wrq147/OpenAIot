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

/* 全局样式文件 */
.el-button--custom {
    background: #222E40 !important;
    border-color: #222E40 !important;
    font-weight: 400 !important;
    font-size: 14px !important;
    color: #FFFFFF !important;
}

.el-button--custom:hover {
    background: #293342 !important;
    border-color: #293342 !important;
}
//公共的表格和表单样式
.app-main {
  background-color: rgba(19, 25, 34, 1);
  --current-color:rgba(61, 185, 143, 1);
}
.app-main > div { 
  min-height: calc(100vh - 100px);
}
.app-main.notshowTop > div{ 
  min-height: calc(100vh - 60px);
}
.hasTagsView {
  .app-main > div{
    min-height: calc(100vh - 94px);
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
  padding: 16px;
  background-color: rgba(25, 33, 45, 1);
  // margin-bottom: 10px;
  border-radius: 4px;
}
.elbiaoge_elform {
  background-color: rgba(25, 33, 45, 1);
  border-radius: 4px 4px 0 0;
  width: 100%;
  padding: 16px;
  box-sizing: border-box;
  
}

.mb8.button_row {
  margin-bottom: 0;
}
.set_radius {
  height: 36px;
  line-height: 36px;
  border-radius: 4px;
  vertical-align: middle;
  width: 140px;
  background: transparent;

  input {
    // width: 140px;
    border-radius: 4px;
  }
  &.el-range-editor.el-input__inner{
    padding-top: 0;
    padding-bottom: 0;
    padding-left: 6px;
    padding-right: 3px;
    background: transparent;
    border-color: rgba(255, 255, 255, 0.2);
    .el-range-input{
      background: transparent;
    }
    
  }
}
.el-date-editor.el-range-editor.el-input__inner{
  .el-range-input{
    color: #ffffff;
    background: transparent;
  }
  .el-range-input::placeholder{
    color: rgba(255, 255, 255, 0.3); 
  }
  &:hover{
    border-color:rgba(61, 185, 143, 1);
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
  margin-left: 20px;
  margin-bottom: 10px;
  .el-form-item__content {
    line-height: 36px;
    color:rgba(255, 255, 255, 1);
    .el-input__inner {
      height: 36px;
      background: transparent;
    }
    .el-input--suffix{
      .el-input__suffix{
        .el-input__icon{
          line-height: 36px;
        }
      }
    }
    .el-input{
      &.is-disabled{
        .el-input__inner{
          background-color: rgba(34, 46, 64, 1) !important;
          border-color: rgba(34, 46, 64, 1) !important;
          color: rgba(255, 255, 255, 1) !important;
        }
        .el-input__inner:hover{
          border-color: rgba(34, 46, 64, 1) !important;
        }
      }
    }
  }
  .el-form-item__label {
    line-height: 36px;
    color: #fff;
    padding-right: 16px;
  }
  .el-range-editor.el-input__inner{
    padding-top: 0;
    padding-bottom: 0;
    padding-left: 6px;
    padding-right: 3px;
    background: transparent;
    .el-range-input{
      background: transparent;
    }
  }
  &.is-disabled{
    .el-range-editor.el-input__inner{
      background-color: rgba(34, 46, 64, 1) !important;
      border-color: rgba(34, 46, 64, 1) !important;
      color: rgba(255, 255, 255, 1) !important;
    }
    .el-range-editor.el-input__inner:hover{
      border-color: rgba(34, 46, 64, 1) !important;
    }
  }
}
.el-form.biaodan.biaodan_flex{
  display: flex;
  align-items: flex-start;
  .biaodan_input_con{
    display: flex;
    align-items: center;
    flex-wrap: wrap;
  }
  
  .button_con{
    .el-form-item__content{
      display: flex;
      justify-content: flex-end;
    }
    
  }
}
.biaodan.el-form--inline {
  line-height: 36px;
  margin-left: -20px;
  margin-bottom: -15px;
}
.el-form.biaodan .el-form-item .el-button {
  border-radius: 4px;
  height: 36px;
  cursor: pointer;
  padding: 10px 18px;

}
.el-form.biaodan .el-button--primary {
  background: rgba(61, 185, 143, 1);
  border: none;
  &.is-plain{
    i{
      font-size:14px;
    }
    span{
      font-size:14px;
    }
  }
}
.el-form.biaodan .el-button--default{
  background: rgba(34, 46, 64, 1);
  color: #ffffff;
  border: none;
}
.el-form.biaodan .submit_button_con {
  float: right;
}
.button_row {
  padding:0;
  box-sizing: border-box;
  margin-bottom: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  .el-button{
    span{
      display: flex;
      align-items: center;
      justify-content: center;
    }
    i{
      font-size: 12px;
    }
    i.zhongtai-icon-xinzeng{
      font-size: 10px;
    }
  }
  .el-button.is-plain {
    border-radius: 4px;
    padding: 10px 12px;
    border: none;
    color: rgba(255, 255, 255, 1) !important;
    background-color: rgba(34, 46, 64, 1) !important;
    height: 36px;
    min-width: 70px;
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
    color: rgba(255, 255, 255, 1) !important;
    background: rgba(61, 185, 143, 1) !important;
  }
}
.button_row.el-row::before,
.button_row.el-row::after {
  display: none;
}
.el-table{
  background-color: rgba(25, 33, 45, 1) !important;
}
.data_table.el-table {
    margin-top: 16px;
    background-color: rgba(25, 33, 45, 1);
    .el-loading-mask{
      background-color: rgba(34, 46, 64, 0.9);
    }
}
.el-table, .el-table__expanded-cell{
  background: rgba(25, 33, 45, 1);
}
.el-table::before{
  height: 0;
}
.data_table.el-table .el-table__body-wrapper .el-table__empty-block{
  border-bottom: 1px solid rgba(255, 255, 255, 0.1) !important;
}
.data_table.el-table table.el-table__header {
  margin-top: 0;
}
.data_table.el-table table {
  border: none;
  border-collapse: collapse !important;
  background: rgba(25, 33, 45, 1);
 .el-checkbox__inner{
    border: 1px solid rgba(255, 255, 255, 0.6);
    background: transparent;
  }
  .el-checkbox__input.is-checked .el-checkbox__inner, .el-checkbox__input.is-indeterminate .el-checkbox__inner{
    border: 1px solid #3DB98F;
    background-color: #3DB98F;
  }
  tr.el-table__row {
    border-bottom: 1px solid rgba(255, 255, 255, 0.1) !important;
    background: rgba(25, 33, 45, 1);
    &.hover-row{
      background: rgba(32, 63, 65, 1);
    }
    th {
      color: rgba(255, 255, 255, 0.6);
      font-weight: 400;
    }
    
  }

  .fixed-width .cell {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-right: -20px;
    .el-button {
      margin-right: 0;
      &.primary{
        color: rgba(61, 185, 143, 1);
      }
      &.danger{
        color: rgba(241, 92, 92, 1);
      }
    }
    .line{
      height: 14px;
      width: 1px;
      background: rgba(255, 255, 255, 0.2);
      margin: 0 10px;
    }
    .el-dropdown {
      line-height: 28px;
      padding: 7px 0;
    }
  }
}
.el-table__fixed-right th.fixed-class {
  background: #222E40 !important;
  color: rgba(255, 255, 255, 0.6);
}
.data_table.el-table .el-table__body tr:hover>td.el-table__cell{
  background-color: rgba(32, 63, 65, 1);
}
.el-table__body tr.hover-row > td.el-table__cell, .el-table__body tr.hover-row.current-row > td.el-table__cell, .el-table__body tr.hover-row.el-table__row--striped > td.el-table__cell, .el-table__body tr.hover-row.el-table__row--striped.current-row > td.el-table__cell{
  background-color: rgba(32, 63, 65, 1) !important;
}
.data_table tr td {
  color: #333333;
  background: transparent;
}
.data_table.el-table .el-table__header thead tr{
  background:rgba(34, 46, 64, 1);
}
.data_table.el-table th.el-table__cell,
.data_table.el-table th.el-table__cell.is-leaf,
.data_table.el-table td.el-table__cell {
  border-color: rgba(255, 255, 255, 0.1);
  border: none;
}
.data_table.el-table td.el-table__cell {
  padding: 9px 0;
}
.data_table.el-table .el-table__header-wrapper thead th {
  background: rgba(34, 46, 64, 1);
}
.data_table.el-table .el-table__header thead th.el-table__cell {
  padding: 14px 0;
}
.data_table.el-table .el-table__fixed-header-wrapper th.el-table__cell.is-leaf {
  padding: 0;
}
.data_table.el-table .el-table__header thead th.is-leaf .cell{
  border-right: 1px solid rgba(255, 255, 255, 0.1);
}
.data_table.el-table .el-table__header thead th.is-leaf:nth-last-child(2){
  .cell{
    border-right:none;
  }
}
.data_table.el-table .el-table__header thead th.is-leaf:nth-last-child(1){
  .cell{
    border-right:none;
  }
}
.data_table.el-table .el-table__header thead tr> th:not(.is-leaf){
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}
.data_table.el-table .el-table__header thead tr> th:not(.is-leaf) ~ th {
  .cell{
    border-left: 1px solid rgba(255, 255, 255, 0.1);
  }
}
/* 表格最外层边框-底部边框 */
.data_table.el-table--border::after,
.data_table.el-table--group::after {
  width: 0;
}
.data_table::before {
  width: 0;
}
.data_table .el-table__fixed-right{
  box-shadow: -6px 0px 4px 0px rgba(0,0,0,0.2);

}
.data_table .el-table__fixed-right::before,
.data_table.el-table__fixed::before {
  width: 0;
}
.data_table.el-table::before {
  height: 0px !important;
}
.data_table.el-table--border::after {
  width: 0px;
}
.data_table.el-table--border {
  border: none;
}
/* 表格有滚动时表格头边框 */

.data_table.el-table td.el-table__cell > div {
  // font-size: 16px;
  overflow: hidden;
  white-space: nowrap;
  color: rgba(255, 255, 255, 1);
}
.data_table.el-table .el-table__header-wrapper {
  thead {
    th {
      background: transparent !important;
      div {
        font-weight: normal;
        overflow: initial;
        white-space: nowrap;
        color: rgba(255, 255, 255, 0.6);
      }
    }
  }
}
.el-dialog {
  border-radius: 4px !important;
}
.el-select-dropdown.el-popper{
  background: rgba(34, 46, 64, 1);
  border:none;
  &.is-multiple{
    .el-select-dropdown__item.selected{
      background: transparent;
      color: rgba(61, 185, 143, 1);
    }
    .el-select-dropdown__item.hover{
      background: transparent !important;
    }
  }
  .el-select-dropdown__item{
    color: #ffffff;
    background: transparent;
    &.selected{
      background: transparent;
      color: rgba(61, 185, 143, 1);
    }
    &.hover{
      // background: rgba(61, 185, 143, 0.2) !important;
      // color: #3DB98F;
      // color: rgba(61, 185, 143, 1);
    }
    &.selected::after {
      position: absolute;
      right: 20px;
      font-family: element-icons;
      content: "\e6da";
      font-size: 12px;
      font-weight: 700;
      -webkit-font-smoothing: antialiased;
      -moz-osx-font-smoothing: grayscale;
    }
  }
  .popper__arrow::after{
    border-bottom-color: #222e40 !important;
  }
  .popper__arrow{
    border-bottom-color: #222e40 !important;
  }
}
.el-select{
  .el-input.is-focus.el-input--suffix{
    .el-input__inner{
      border-color:rgba(61, 185, 143, 1);
    }
  }
}
.el-input.is-focus.el-input--suffix{
  .el-input__inner{
    border-color:rgba(61, 185, 143, 1);
  }
}
.el-select{
  .el-tag.el-tag--info{
    background: rgba(34, 46, 64, 1);
    color: #ffffff;
    border-color: rgba(34, 46, 64, 1);
    height: 26px;
    .el-tag__close{
      color: #ffffff;
      background-color:transparent;
    }
  }
  .el-input.el-input--suffix{
    .el-input__inner{
      border-color:rgba(255, 255, 255, 0.2);
      color: #ffffff;
      padding-left: 10px;
      padding-right: 20px;
    }
    &.is-disabled{
      .el-input__inner{
        background-color: rgba(34, 46, 64, 1) !important;
        border-color: rgba(34, 46, 64, 1) !important;
        color: rgba(255, 255, 255, 1) !important;
      }
      .el-input__inner:hover{
        border-color: rgba(34, 46, 64, 1) !important;
      }
    }
  }
}
.el-input{
  &.el-input-group.el-input-group--append .el-input__inner:focus,&.el-input-group.el-input-group--append .el-input__inner:hover{
    border-right: 1px solid rgba(61, 185, 143, 1);
  }
  &.el-input-group.el-input-group--append .el-input__inner{
    border-right: none;
  }
  &.el-input-group.el-input-group--prepend .el-input__inner:focus,&.el-input-group.el-input-group--prepend .el-input__inner:hover{
    border-left: 1px solid rgba(61, 185, 143, 1);
  }
  &.el-input-group.el-input-group--prepend .el-input__inner{
    
    border-left: none;
  }
  &.el-input-group.el-input-group--append,&.el-input-group.el-input-group--prepend{
    .el-input-group__append,.el-input-group__prepend{
      background: rgba(34, 46, 64, 1);
      padding-left: 11px;
      padding-right: 11px;
      border-color: rgba(255, 255, 255, 0.2);
      color: rgba(255, 255, 255, 1);
    }
    // .el-input__inner:focus ~ div {
    //   border-color: rgba(61, 185, 143, 1);
    // }
    // .el-input__inner:hover ~ div {
    //   border-color: rgba(61, 185, 143, 1);
    // }
  }
  &.el-input-group.el-input-group--append.el-input-group--prepend .el-input__inner:focus,&.el-input-group.el-input-group--append.el-input-group--prepend .el-input__inner:hover{
    border-left: 1px solid rgba(61, 185, 143, 1);
    border-right: 1px solid rgba(61, 185, 143, 1);
  }
  &.el-input-group.el-input-group--append.el-input-group--prepend .el-input__inner{
    border-left: none;
    border-right: none;
  }
  &.is-disabled.el-input-group.el-input-group--append,&.is-disabled.el-input-group.el-input-group--prepend{
    .el-input-group__append,.el-input-group__prepend{
      border: none;
    }
  }
  .el-input__inner{
    border-color:rgba(255, 255, 255, 0.2);
    color: #ffffff;
  }
  
  &.is-disabled{
    .el-input__inner{
      background-color: rgba(34, 46, 64, 1) !important;
      border-color: rgba(34, 46, 64, 1) !important;
      color: rgba(255, 255, 255, 1) !important;
    }
    .el-input__inner:hover{
      border-color: rgba(34, 46, 64, 1) !important;
    }
  }
  &.el-date-editor--time-select{
    .el-input__inner{
      padding-left: 10px;
      padding-right:10px;
    }
    
    .el-input__prefix{
      left: 0;
      .el-input__icon.el-icon-time{
        display: none !important;
      }
    }
    
  }
  &.el-input--prefix .el-input__inner{
    padding-left: 25px;
    padding-right: 20px;
  }
  &.el-input--prefix .el-input__prefix{
    left: 0;
  }
  &.el-input--prefix .el-input__suffix{
    right: 0;
  }
  .el-input__prefix{
    .el-input__icon{
      line-height: 36px;
    }
  }
  .el-input__inner:focus{
    border-color:rgba(61, 185, 143, 1);
  }
  & ::-webkit-scrollbar-thumb{
    background: rgba(255, 255, 255, 0.6) !important;
  }
  & ::-webkit-scrollbar-track-piece{
    background: rgba(255, 255, 255, 0.2) !important;
  }
  & ::-webkit-scrollbar-track{
    background: rgba(255, 255, 255, 0.2) !important;
  }
  & ::-webkit-scrollbar-corner{
    background: rgba(255, 255, 255, 0.2) !important;
  }
}
.el-textarea{
  .el-textarea__inner{
    background: transparent;
    color: #ffffff;
    border-color: rgba(255, 255, 255, 0.2);
  }
  .el-textarea__inner:focus{
    border-color:rgba(61, 185, 143, 1);
  }
  .el-textarea__inner:hover{
    border-color:rgba(61, 185, 143, 1);
  }
  .el-textarea__inner::placeholder {
    color: rgba(255, 255, 255, 0.3);
  }
  &.is-disabled{
    .el-textarea__inner{
      background-color: rgba(34, 46, 64, 1) !important;
      border-color: rgba(34, 46, 64, 1) !important;
      color: rgba(255, 255, 255, 1) !important;
    }
    .el-textarea__inner:hover{
      border-color: rgba(34, 46, 64, 1) !important;
    }
  }
}

.el-input{
  .el-input__inner{
    background: transparent;
  }
  .el-input__inner:hover{
    border-color:rgba(61, 185, 143, 1);
  }
  .el-input__inner::placeholder { /* 使用 /deep/ 或 >>> 根据你的Vue版本 */
    color: rgba(255, 255, 255, 0.3); /* 修改为你想设置的颜色 */
  }
}
.el-pagination__editor{
  .el-input__inner{
    background: transparent;
    color: #ffffff;
  }
}
.el-input.is-active .el-input__inner, .el-input__inner:focus{
  border-color:rgba(61, 185, 143, 1);
}
.el-date-editor.el-input__inner{
  border-color:rgba(255, 255, 255, 0.2);
  background: transparent;
  .el-range-input{
    background: transparent;
  }
  .el-range-separator{
    color: rgba(255, 255, 255, 0.6);
  }
  .el-range-input::placeholder{
    color: rgba(255, 255, 255, 0.3); 
  }
}
.el-range-editor.is-active, .el-range-editor.is-active:hover{
  border-color:rgba(61, 185, 143, 1) !important;
}

.el-menu-demo.shejiqi{
  background: transparent;
}
.el-menu--horizontal>.el-menu-item:not(.is-disabled):focus, .el-menu--horizontal>.el-menu-item:not(.is-disabled):hover, .el-menu--horizontal>.el-submenu .el-submenu__title:hover{
  background-color: transparent !important;
}
.el-menu-demo.el-menu--horizontal{
  .el-menu-item.is-active{
    color:rgba(61, 185, 143, 1) !important;
    border-bottom-color:rgba(61, 185, 143, 1) !important;
  }
  .el-menu-item{
    color: #ffffff;
  }
}
.el-dialog{
  background: rgba(25, 33, 45, 1) !important;
  .el-dialog__body{
    padding-top: 24px;
    &::-webkit-scrollbar {
        width: 6px;
        height: 6px;
    }
    
    /* 修改滚动条轨道 */
    &::-webkit-scrollbar-track {
        background: rgba(25, 33, 45, 1);
    }
    
    /* 修改滚动条滑块 */
    &::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.2);
        border-radius: 5px;
    }
    
    /* 修改滑块在鼠标悬浮时的样式 */
    &::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.2);
    }
  }
  .el-form.el-form--label-top .el-form-item {
    margin-bottom: 24px;
    .el-form-item__label {
      line-height: 14px;
      color:rgba(255, 255, 255, 0.60);
      font-size: 14px;
    }
  }
}
.el-dialog__header{
  .el-dialog__title{
    color: #ffffff;
  }
}
.el-dialog, .el-pager li{
  background: rgba(25, 33, 45, 1);
}
.el-form-item{
  .el-form-item__label{
    color: rgba(255, 255, 255, 0.60);
  }
}

.vue-treeselect .vue-treeselect__control{
  height: 36px;
  line-height: 36px;
  background: transparent;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 4px;
  .vue-treeselect__placeholder{
    color:rgba(255, 255, 255, 0.3);
  }
}
.vue-treeselect--disabled{
  .vue-treeselect__control{
    background-color: rgba(34, 46, 64, 1) !important;
    border-color: rgba(34, 46, 64, 1) !important;
    cursor: not-allowed;
  }
}
.vue-treeselect:not(.vue-treeselect--disabled):not(.vue-treeselect--focused) .vue-treeselect__control:hover{
  border-color: rgba(61, 185, 143, 1) !important;
}
.vue-treeselect__menu{
  background: rgba(34, 46, 64, 1) !important;
}
.vue-treeselect__single-value{
  color: #ffffff !important;
}
.vue-treeselect__list-item{
  background: transparent !important;
  .vue-treeselect__option{
    background: transparent !important;
  }
  .vue-treeselect__label-container{
    color: #ffffff;
  }
  .vue-treeselect__option--disabled{
    .vue-treeselect__label-container{
      color:rgba(255, 255, 255, 0.6);
    }
  }
}
.vue-treeselect--single .vue-treeselect__option--selected,.vue-treeselect--single .vue-treeselect__option--selected:hover{
  background: transparent !important;
  .vue-treeselect__label-container{
    color: rgba(61, 185, 143, 1);
  }
}
.vue-treeselect.vue-treeselect--open{
  .vue-treeselect__control{
    border-color: rgba(255, 255, 255, 0.2);
  }
  .vue-treeselect__menu{
    width: 100%;
    border-color: rgba(255, 255, 255, 0.2);
  }
}
.vue-treeselect.vue-treeselect--focus{
  .vue-treeselect__control{
    border-color: rgba(61, 185, 143, 1);
  }
}
.el-tree{
  background: transparent !important;
  color: #ffffff !important;
}
.el-tree-node:not(.is-current) > .el-tree-node__content{
  background: transparent !important;
}
.el-tree--highlight-current .el-tree-node.is-current>.el-tree-node__content{
  background: rgba(34, 46, 64, 1) !important;
  .custom-tree-node{
    color: rgba(61, 185, 143, 1);
  }
}
.el-input-number{
  .el-input-number__decrease{
    background: rgba(34, 46, 64, 1);
    border-color: rgba(255, 255, 255, 0.2) !important;
  }
  .el-input-number__increase{
    background: rgba(34, 46, 64, 1);
    border-color: rgba(255, 255, 255, 0.2) !important;
  }
  .el-input-number__decrease:hover, .el-input-number__increase:hover{
    color: rgba(61, 185, 143, 1);
  }
  .el-input-number__decrease:hover:not(.is-disabled)~.el-input .el-input__inner:not(.is-disabled), .el-input-number__increase:hover:not(.is-disabled)~.el-input .el-input__inner:not(.is-disabled){
    border-color: rgba(61, 185, 143, 1);
  }
}
.el-upload--picture-card{
  background-color: transparent !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
}
.el-upload--picture-card i{
  color: #ffffff !important;
}
.el-dialog__footer{
  .dialog-footer{
    .el-button.el-button--primary{
      background: rgba(61, 185, 143, 1);
      border: 1px solid rgba(61, 185, 143, 1);
      color: #ffffff;
    }
    .el-button.el-button--default{
      background: rgba(34, 46, 64, 1);
      border: 1px solid rgba(34, 46, 64, 1);
      color: #ffffff;
    }
  }
}
.el-loading-mask{
  background-color: rgba(34, 46, 64, 0.9) !important;
}
.el-tabs{
  background-color: rgba(25, 33, 45, 1);
  .el-tabs__header{
    background-color: rgba(25, 33, 45, 1);
    .el-tabs__nav-wrap::after{
      background-color:rgba(255, 255, 255, 0.2);
    }
    .el-tabs__nav{
      .el-tabs__item{
        color: #ffffff;
        &.is-active{
          color: rgba(61, 185, 143, 1);
        }
      }
      .el-tabs__active-bar{
        background: rgba(61, 185, 143, 1);
      }
    }
  }
  
}
.el-table__body-wrapper::-webkit-scrollbar {
    width: 6px;
    height: 6px;
}
 
/* 修改滚动条轨道 */
.el-table__body-wrapper::-webkit-scrollbar-track {
    background: rgba(25, 33, 45, 1);
}
 
/* 修改滚动条滑块 */
.el-table__body-wrapper::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.2);
    border-radius: 5px;
}
 
/* 修改滑块在鼠标悬浮时的样式 */
.el-table__body-wrapper::-webkit-scrollbar-thumb:hover {
    background: rgba(255, 255, 255, 0.2);
}
.el-picker-panel.el-date-picker,.el-picker-panel.el-date-range-picker,.el-picker-panel.time-select{
  border-color:rgba(255, 255, 255, 0.2);
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, .1);
  background: rgba(25, 33, 45, 1);
  .el-date-table td.in-range div, .el-date-table td.in-range div:hover, .el-date-table.is-week-mode .el-date-table__row.current div, .el-date-table.is-week-mode .el-date-table__row:hover div{
    background: rgba(34, 46, 64, 1);
  }
  .el-picker-panel__footer{
    background: rgba(25, 33, 45, 1);
    border-top: 1px solid rgba(255, 255, 255, 0.2);
    .el-picker-panel__link-btn.is-plain{
      background: rgba(61, 185, 143, 1);
      color: #ffffff;
      border: none;
    }
  }
  .el-date-picker__time-header{
    border-color: rgba(255, 255, 255, 0.2);
    
  }
  .el-date-picker__header,.el-date-range-picker__header{
    .el-picker-panel__icon-btn{
      color: rgba(255, 255, 255, 1);
    }
    .el-date-picker__header-label{
      color: rgba(255, 255, 255, 1);
    }
    div{
      color: rgba(255, 255, 255, 1);
    }
  }
  .el-picker-panel__content{
    &.el-date-range-picker__content.is-left{
      border-color: rgba(255, 255, 255, 0.2);
    }
    .el-date-table{
      tr th{
        border-color: rgba(255, 255, 255, 0.2);
        color: rgba(255, 255, 255, 1);
      }
      tr.el-date-table__row{
        td.prev-month{
          color: rgba(255, 255, 255, 0.2);
        }
        td.available{
          color: rgba(255, 255, 255, 1);
          
        }
        td.available.today:not(.in-range){
          span{
            color: rgba(61, 185, 143, 1);
          }
        }
        td.available.today.in-range{
          span{
            color: rgba(255, 255, 255, 1);
          }
        }
        td.next-month{
          color: rgba(255, 255, 255, 0.2);
        }
        td.current:not(.disabled) span{
          background: rgba(61, 185, 143, 1);
        }
        td.available.in-range.start-date,td.available.in-range.end-date{
          span{
            background: rgba(61, 185, 143, 1);
          }
        }
      }
    }
    .time-select-item:hover{
      background: transparent;
    }
    .time-select-item{
      color: rgba(255, 255, 255, 1);
    }
    .time-select-item.selected{
      color: rgba(61, 185, 143, 1);
    }
  }
  .el-time-panel{
    border-color:rgba(255, 255, 255, 0.2);
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, .1);
    background: rgba(25, 33, 45, 1);
    .el-time-spinner__item:hover:not(.disabled):not(.active){
      background-color: rgba(34, 46, 64, 1);
    }
    .el-time-spinner__item.active{
      color: rgba(255, 255, 255, 1);
    }
    .el-time-spinner__item{
      color: rgba(255, 255, 255, 0.6);
    }
    .el-time-panel__footer{
      border-color: rgba(255, 255, 255, 0.2);
    }
  }
  // .popper__arrow::after{
  //   border-color: rgba(255, 255, 255, 0.2) !important;
  // }
  // .popper__arrow{
  //   border-color: rgba(255, 255, 255, 0.6) !important;
  // }
}
.el-message-box{
  background-color: rgba(25, 33, 45, 1) !important;
  border-color:rgba(25, 33, 45, 1) !important;
  .el-message-box__header{
    .el-message-box__title{
      color: rgba(255, 255, 255, 1);
    }
  }
  .el-message-box__content{
    .el-message-box__container{
      color: rgba(255, 255, 255, 1);
      .el-message-box__message{
        
      }
    }
  }
  .el-message-box__btns{
    .el-button--default{
      background: rgba(34, 46, 64, 1);
      color: rgba(255, 255, 255, 0.6);
      border: none;
    }
    .el-button--primary{
      background: rgba(61, 185, 143, 1);
      color: rgba(255, 255, 255, 1);
      border: none;
    }
  }
}
.el-cascader__dropdown{
  background: rgba(34, 46, 64, 1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  .el-cascader-menu{
    color: rgba(255, 255, 255, 1);
    border-color: rgba(255, 255, 255, 0.2) !important;
    .el-cascader-node:not(.is-disabled):focus, .el-cascader-node:not(.is-disabled):hover{
      background: rgba(34, 46, 64, 1) !important;
      color: rgba(61, 185, 143, 1);
    }
    .el-cascader-node.in-active-path, .el-cascader-node.is-active, .el-cascader-node.is-selectable.in-checked-path{
      color: rgba(61, 185, 143, 1);
    }
    .el-cascader-node{
      .el-radio.is-checked{
        .el-radio__input.is-checked .el-radio__inner{
          border-color: rgba(61, 185, 143, 1);
          background: rgba(61, 185, 143, 1);
        }
      }
    }
    
  }
  .el-cascader__suggestion-panel{
    .el-cascader__suggestion-list{
      color: #ffffff;
      .el-cascader__suggestion-item:focus, .el-cascader__suggestion-item:hover{
        background: rgba(34, 46, 64, 0);
        color: #3db98f;
      }
      .el-cascader__suggestion-item.is-checked{
        color: #3db98f;
      }
    }
  }
}
.el-cascader:not(.is-disabled):hover .el-input__inner{
  border-color: #3db98f !important;
}
// .el-row::before, .el-row::after{
//   display: none !important;
// }
.submit_btn_con{
  display: flex;
  justify-content: flex-end;
  align-items: center;
  .el-button.el-button--default{
    background: rgba(34, 46, 64, 1);
    color: rgba(255, 255, 255, 0.6);
    border: none;
  }
  .el-button.el-button--primary{
    background: rgba(61, 185, 143, 1);
    color: rgba(255, 255, 255, 1);
    border: none;
  }
}
.el-radio-group{
  .el-radio{
    color: rgba(255, 255, 255, 1);
    &.is-checked{
      .el-radio__input{
        .el-radio__inner{
          border-color: rgba(61, 185, 143, 1);
        }
        .el-radio__inner::after{
          background-color: rgba(61, 185, 143, 1);
        }
      }
      .el-radio__label{
        color: rgba(255, 255, 255, 1);
      }
    }
    .el-radio__input{
      .el-radio__inner{
        background-color: transparent;
        border-color: rgba(255, 255, 255, 0.6);
      }
    }
  }
}
//公共的表格和表单样式
</style>