<template>
  <el-container style="height: 100%;">
    <el-aside style="padding:0;">
      <div class="components-nav">
        <span @click="libSelect = 0">组件库</span>
      </div>
      <div>
        <div
          class="components"
          style="overflow: hidden;"
          v-for="(group, i) in baseComponents"
          :key="i"
        >
          <p>{{group.name}}</p>
          <ul>
            <draggable
              class="drag"
              :list="group.components"
              :options="{sort: false}"
              :group="{ name: 'form', pull: 'clone', put: false }"
              @start="isStart = true"
              @end="isStart = false"
              :clone="clone"
              :move="move"
              :class="group.name"
            >
              <li v-for="(cp, id) in group.components" :key="id" :class="cp.name">
                <i class="zhongtaiiconfont" :class="cp.icon" v-if="cp.isSvgIcon" style="font-size:12px"></i>
                <i :class="cp.icon" v-else></i>
                <span>{{ cp.title }}</span>
              </li>
            </draggable>
          </ul>
        </div>
      </div>
    </el-aside>

    <el-main class="layout-main">
      <div class="tool-nav">
        <div>
          <el-tooltip class="item" content="撤销" placement="bottom-start">
            <i class="el-icon-refresh-left"></i>
          </el-tooltip>
          <el-tooltip class="item" content="恢复" placement="bottom-start">
            <i class="el-icon-refresh-right"></i>
          </el-tooltip>
        </div>
        <div>
          <el-tooltip class="item" content="预览表单" placement="bottom-start">
            <i class="el-icon-view" @click="viewForms"></i>
          </el-tooltip>
          <el-tooltip class="item" content="移动端" placement="bottom-start">
            <i :class="{'el-icon-mobile':true, 'select': showMobile}" @click="showMobile = true"></i>
          </el-tooltip>
          <el-tooltip class="item" content="PC端" placement="bottom-start">
            <i :class="{'el-icon-monitor':true, 'select': !showMobile}" @click="showMobile = false"></i>
          </el-tooltip>
        </div>
      </div>
      <div class="work-form">
        <div :class="{'mobile': showMobile, 'pc': !showMobile}">
          <div :class="{'bd': showMobile}">
            <div :class="{'form-content': showMobile}">
              <div class="form">
                <div class="tip" v-show="forms.length === 0 && !isStart">👈 请在左侧选择控件并拖至此处</div>
                <draggable
                  class="drag-from"
                  :list="forms"
                  group="form"
                  :options="{animation: 300, chosenClass:'choose', sort:true}"
                  @start="drag = true; selectFormItem = null"
                  @end="drag = false"
                  :move="move"
                >
                  <div
                    v-for="(cp, id) in forms"
                    :key="id"
                    class="form-item"
                    @click="selectItem(cp)"
                    :style="getSelectedClass(cp)"
                    :class="cp.name"
                  >
                    <div class="form-header">
                      <p>
                        <span v-if="cp.props.required">*</span>
                        {{ cp.title }}
                      </p>
                      <div class="option">
                        <!--<i class="el-icon-copy-document" @click="copy"></i>-->
                        <i class="el-icon-close" @click="del(id)"></i>
                      </div>
                      <form-design-render :config="cp" />
                    </div>
                  </div>
                </draggable>
              </div>
            </div>
          </div>
        </div>
      </div>
    </el-main>

    <el-aside class="layout-param">
      <div class="tool-nav-r" v-if="selectFormItem">
        <i :class="selectFormItem.icon" style="margin-right: 5px; font-size: medium"></i>
        <span>{{ selectFormItem.title }}</span>
      </div>
      <div v-if="!selectFormItem || forms.length === 0" class="tip">😀 选中控件后在这里进行编辑</div>
      <div style="text-align:left; padding: 10px" v-else>
        <form-component-config ref="form-config" />
      </div>
    </el-aside>
    <w-dialog
      clickClose
      closeFree
      width="800px"
      :showFooter="false"
      :border="false"
      title="表单预览"
      v-model="viewFormVisible"
    >
      <form-render ref="form" :forms="forms" v-model="formData" />
    </w-dialog>
  </el-container>
</template>

<script>
import draggable from "vuedraggable";

import WDialog from "../../common/WDialog.vue";
import FormRender from "../../common/form/FormRender";
import FormDesignRender from "./form/FormDesignRender";
import FormComponentConfig from "../../common/form/FormComponentConfig";
import { baseComponents } from "../../common/form/ComponentsConfigExport";
import { generateId } from "@/api/flowable/design";
import "../../common/utlity.js";
export default {
  name: "FormDesign",
  components: {
    WDialog,
    draggable,
    FormComponentConfig,
    FormDesignRender,
    FormRender
  },
  data() {
    return {
      formData: {},
      libSelect: 0,
      viewFormVisible: false,
      isStart: false,
      showMobile: true,
      baseComponents,
      select: null,
      drag: false
    };
  },
  computed: {
    forms() {
      return this.$store.state.flowable.design.formItems;
    },
    selectFormItem: {
      get() {
        return this.$store.state.flowable.selectFormItem;
      },
      set(val) {
        this.$store.state.flowable.selectFormItem = val;
      }
    },
    nodeMap() {
      return this.$store.state.flowable.nodeMap;
    }
  },
  methods: {
    move(e){
      if (e.dragged._prevClass.indexOf("SpanLayout")>-1) {
        // if(e.to.className.indexOf('spanLayout')>-1){
        //   // this.$message.warning("分栏内禁止再加分栏");
        //   return false
        // }else 
        if(e.to.className.indexOf('tablelist')>-1){
          // this.$message.warning("明细表内禁止加分栏");
          return false
        }
      }
    },
    onStartMove(e){
      this.isStart = true
    },
    copy(node, index) {
      let xobj = Object.assign({}, node);
      generateId().then(rp => {
        xobj.id = rp.data.toString();
      });
      this.form.splice(index + 1, 0, xobj);
    },
    getId() {
      //生成临时Id
      return (
        "T" +
        (Math.floor(Math.random() * (99999 - 10000)) + 10000).toString() +
        new Date()
          .getTime()
          .toString()
          .substring(5)
      );
    },
    del(index) {
      this.$confirm(
        "删除组件将会连带删除包含该组件的条件以及相关设置，是否继续?",
        "提示",
        {
          confirmButtonText: "确 定",
          cancelButtonText: "取 消",
          type: "warning"
        }
      ).then(() => {
        if (this.forms[index].name === "SpanLayout") {
          //删除的是分栏则遍历删除分栏内所有子组件
          this.forms[index].props.items.forEach(item => {
            this.removeFormItemAbout(item);
          });
          this.forms[index].props.items.length = 0;
        } else {
          this.removeFormItemAbout(this.forms[index]);
        }
        this.forms.splice(index, 1);
      });
    },
    async removeFormItemAbout(item) {
      this.nodeMap.forEach(node => {
        //搜寻条件，进行移除
        if (node.type === "CONDITION") {
          node.props.groups.forEach(group => {
            let i = group.cids.remove(item.id);
            if (i > -1) {
              //从子条件移除
              group.conditions.splice(i, 1);
            }
          });
        }
        //搜寻权限，进行移除
        if (
          node.type === "ROOT" ||
          node.type === "APPROVAL" ||node.type === "USER" ||
          node.type === "CC"
        ) {
          node.props.formPerms.removeByKey("id", item.id);
          if (node.props.formUser === item.id) {
            node.props.formUser = "";
          }
        }
      });
    },
    clone(obj) {
      // console.log(obj,'obj')
      obj.id = this.getId();
      let xobj = JSON.parse(JSON.stringify(obj));
      generateId().then(rp => {
        xobj.id = rp.data.toString();
      });
      return xobj;
    },
    viewForms() {
      this.viewFormVisible = true;
    },
    selectItem(cp) {
      // console.log("表单设计器选中的控件", cp);
      this.selectFormItem = cp;
    },
    getSelectedClass(cp) {
      return this.selectFormItem && this.selectFormItem.id === cp.id
        ? "border-left: 4px solid #409eff"
        : "";
    },
    validateItem(err, titleSet, item) {
      if (titleSet.has(item.title) && item.name !== "SpanLayout") {
        err.push(`表单 ${item.title} 名称重复`);
      }
      titleSet.add(item.title);
      if (item.name === "SelectInput" || item.name === "MultipleSelect") {
        if (item.props.options.length === 0) {
          err.push(`${item.title} 未设置选项`);
        }
      } else if (item.name === "TableList") {
        if (item.props.columns.length === 0) {
          err.push(`明细表 ${item.title} 内未添加组件`);
        }else{
          let arr=item.props.columns.filter(row=>row.name=="SpanLayout")
          if(arr&&arr.length>0){
            err.push(`明细表 ${item.title} 内禁止添加分栏`);
          }
        }
      } else if (item.name === "SpanLayout") {
        if (item.props.items.length === 0) {
          err.push("分栏内未添加组件");
        } else {
          let arr=item.props.items.filter(row=>row.name=="SpanLayout")
          // if(arr&&arr.length>0){
          //   err.push(`分栏 ${item.title} 内禁止添加分栏`);
          // }
          item.props.items.forEach(sub =>
            this.validateItem(err, titleSet, sub)
          );
        }
      }
    },
    validate() {
      let err = [];
      if (this.forms.length > 0) {
        let titleSet = new Set();
        this.forms.forEach(item => {
          //主要校验表格及分栏/选择器/表单名称/是否设置
          this.validateItem(err, titleSet, item);
        });
      } else {
        err.push("表单为空，请添加组件");
      }
      return err;
    }
  }
};
</script>

<style lang="scss" scoped>
@import "~@/assets/styles/element-variables.scss";

.choose {
  border: 1px dashed $--color-primary !important;
}

.process-form {
  ::v-deep .el-form-item__label {
    padding: 0 0;
  }
}

.components-nav {
  box-sizing: content-box;
  display: flex;
  align-items: center;
  margin: 12px 12px 0;
  height: 28px;
  box-shadow: 0 2px 4px 0 rgba(17, 31, 44, 0.04);
  border: 1px solid #ecedef;
  border-radius: 16px;
  background-color: #fff;

  .selected {
    color: $--color-primary;
  }

  .border {
    border-left: 1px solid #f5f6f6;
    border-right: 1px solid #f5f6f6;
  }

  span {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    font-size: 12px;
    color: rgba(17, 31, 44, 0.72);
    cursor: pointer;

    &:hover {
      color: $--color-primary;
    }
  }
}

.components {
  overflow-x: hidden;
  overflow-y: scroll;
  //margin-top: 20px;
  //padding: 0 20px;
  font-size: 12px;
  width: 100%;
  color: rgba(17, 31, 44, 0.85);

  ul {
    padding: 0;
  }

  & > p {
    padding: 0 20px;
  }

  .drag {
    margin-left: 20px;
    display: flex;
    flex-wrap: wrap;
    align-items: center;

    li {
      text-align: center;
      display: flex;
      align-items: center;
      width: 124px;
      height: 38px;
      margin-bottom: 12px;
      border: 1px solid transparent;
      border-radius: 8px;
      cursor: grab;
      background-color: #fff;

      &:hover {
        border: 1px solid $--color-primary;
        color: $--color-primary;
      }

      i {
        margin: 0 12px;
      }
    }

    li:nth-child(odd) {
      margin-right: 8px;
    }
  }
}

::v-deep .el-main {
  padding: 0;
}

.layout-main {
  background-color: #feffff;

  .tool-nav {
    font-size: medium;
    padding: 8px 20px;
    background: #fafafb;
    border-bottom: 1px solid #ebecee;

    div:first-child {
      display: inline-block;
      text-align: left;

      i {
        margin-right: 10px;
      }
    }

    div:last-child {
      float: right;

      i {
        margin-left: 10px;
      }
    }

    i {
      color: #7a7a7a;
      cursor: pointer;

      &:hover {
        color: #4b4b4b;
      }
    }
  }

  .work-form {
    margin: 0 auto;
    height: calc(100% - 38px);
    overflow-y: auto;
    background: rgb(245, 246, 246);
    border-left: 1px solid rgb(235, 236, 238);
    border-right: 1px solid rgb(235, 236, 238);

    .pc {
      .drag-from {
        height: 460px;
        background-color: rgb(245, 246, 246);

        .form-item,
        li {
          cursor: grab;
          background: #ffffff;
          padding: 10px;
          border: 1px solid #ebecee;
          margin: 5px 0;
        }
      }
    }

    .mobile {
      margin-left: auto;
      margin-right: auto;
      width: 360px;
      max-height: calc(100% - 20px);
      margin-top: 20px;
      border-radius: 24px;
      box-shadow: 0 8px 40px 0 rgba(17, 31, 44, 0.12);

      .bd {
        border: 1px solid rgba(17, 31, 44, 0.08);
        border-radius: 24px;
        padding: 10px 10px;
        background-color: #ffffff;

        .form-content {
          padding: 3px 2px;
          border-radius: 14px;
          background-color: #f2f4f5;

          .drag-from {
            width: 100%;
            height: 460px;
            min-height: 200px;
            max-height: 600px;
          }

          .form {
            overflow-y: auto;
            width: 100%;
            display: inline-block;
            max-height: 640px;
            height: 640px;
            .form-item,
            li {
              border: 1px solid #ffffff;
              list-style: none;
              background: #ffffff;
              padding: 10px;
              margin: 5px 0;
              cursor: grab;
            }
          }
        }
      }
    }

    .tip {
      //float: left;
      margin: 0 auto;
      width: 65%;
      max-width: 400px;
      padding: 35px 20px;
      border-radius: 10px;
      border: 1px dashed rgba(25, 31, 37, 0.12);
      margin-top: 50px;
      text-align: center;
      font-size: 14px;
      color: rgb(122, 122, 122);
      z-index: 9999;

      &:hover {
        border: 1px dashed $--color-primary;
      }
    }
  }
}

.layout-param {
  text-align: center;
  font-size: 14px;
  color: rgb(122, 122, 122);
  overflow-y: scroll;
  .tool-nav-r {
    text-align: left;
    font-size: small;
    border-left: 1px solid #ebecee;
    padding: 10px 20px;
    background: #fafafb;
    border-bottom: 1px solid #ebecee;
  }

  .tip {
    margin-top: 150px;
  }
}

.flip-list-move {
  transition: transform 0.5s;
}

.no-move {
  transition: transform 0s;
}

.select {
  color: #4b4b4b !important;
}

.form-header {
  font-size: small;
  color: #818181;
  text-align: left;
  position: relative;
  background-color: #fff;

  p {
    position: relative;
    margin: 0 0 10px 0;

    span {
      position: absolute;
      left: -8px;
      top: 3px;
      color: rgb(217, 0, 19);
    }
  }

  .option {
    position: absolute;
    top: -10px;
    right: -10px;

    i {
      font-size: large;
      cursor: pointer;
      color: #8c8c8c;
      padding: 5px;

      &:hover {
        color: #f56c6c;
      }
    }
  }
}

::-webkit-scrollbar {
  width: 4px;
  height: 4px;
  background-color: #f8f8f8;
}

::-webkit-scrollbar-thumb {
  border-radius: 16px;
  background-color: #e8e8e8;
}
</style>
