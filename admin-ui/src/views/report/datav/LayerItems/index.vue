<template>
  <div class="components-list">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="图层" name="level">
        <el-menu
          :default-active="activeIndex.toString()"
          class="el-menu-vertical-demo"
          @select="handleSelect"
        >
          <el-menu-item
            v-for="item in drawingList"
            :key="item.customId"
            :index="item.customId.toString()"
          >
            <div
              @contextmenu.prevent="onContextmenu($event, item.customId)"
              v-if="item.chartType !== 'group'"
            >
              <div v-if="item.chartOption.isGroup !== true">
                <i :class="item.icon"></i>
                <span slot="title">{{ item.layerName }}</span>
                <span
                  class="isLock_position"
                  v-show="item.isUnlocked == false"
                  @click="unclock(item.customId)"
                >
                  <i class="el-icon-lock"></i>
                </span>
              </div>
            </div>
            <div
              @contextmenu.prevent="onContextmenu($event, item.customId)"
              v-else
            >
              <el-tree
                :data="getChildren(item)"
                :props="defaultProps"
                :id="'tree' + item.customId"
                :default-expand-all="false"
                @node-expand="nodeExpandClick"
                @node-click="handleNodeClick"
                :render-content="renderContent"
                style="margin-left: -22px"
              >
              </el-tree>
              <!--<el-submenu index="1" style="margin-left: -20px;">
                <template slot="title">
                  <i class="el-icon-datav-folder"></i>
                  <span>{{item.layerName}}</span>
                  <span class="isLock_position" v-if="item.isUnlocked==false" @click="unclock(item.customId)">
                    <i class="el-icon-lock"></i>
                  </span>
                </template>

                <el-menu-item-group>
                  
                    <el-menu-item v-for="(childMenu, childIndex) in item.chartOption" :key="childMenu.customId" :index="childMenu.customId.toString()">
                      <i :class="childMenu.icon"></i>
                      {{childMenu.layerName}}
                      <span class="isLock_position" v-if="childMenu.isUnlocked==false" @click="unclock(childMenu.customId)">
                        <i class="el-icon-lock"></i>
                      </span>
                    </el-menu-item>
                
                </el-menu-item-group>
              </el-submenu>-->
            </div>
          </el-menu-item>
        </el-menu>
      </el-tab-pane>
      <el-tab-pane label="数据集" name="data">
        <data-integration ref="dataComponents" :isStartLoading="isStartLoading" :themeForm="themeForm" :drawingList="drawingList" :tableType="'default'" @closeLoading="closeLoading"/>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import VueEvent from "../VueEvent";
import dataIntegration from "./dataIntegration";
export default {
  props: ["activeData", "drawingList", "activeId","themeForm","isStartLoading"],
  components: {
    dataIntegration
  },
  data() {
    return {
      //活跃组件id
      actData: this.activeData,
      activeIndex: this.activeId != null ? this.activeId : "",
      defaultProps: {
        children: "children",
        label: "label",
      },
      dismissFlag: false,
      currentTab: "level",
      configData: this.activeData,
    };
  },
  //页面加载完执行
  mounted() {
    VueEvent.$on("to_layer_msg", (data) => {//组态组件信息更新
      this.actData = data;
      this.activeIndex = data.customId;
      if (data.chartType == "group") {
        document.getElementById("tree" + data.customId).childNodes[0].childNodes[0].childNodes[3].style.color = "#1890ff";
      }
    });
  },
  watch: {
    actData: {
      deep: true,
      handler(newVal) {
        this.$emit("active-change", newVal);
        //只有组合才可以解散分组
        if (newVal.chartType == "group") {
          this.dismissFlag = false;
        } else {
          this.dismissFlag = true;
        }
      },
    },
    configData: {
      deep: true,
      handler(newVal, oldValue) {
        // console.log("变了呀",newVal);
        // if(newVal.customId != oldValue.customId) {
        this.$emit("activeChange", newVal);
        // }
      },
    },
    activeData: {
      deep: true,
      handler(newVal) {
        // console.log("左侧数据变化了",newVal);
        this.actData = newVal;
        this.configData = newVal;
        this.activeIndex = newVal.customId;
      },
    },
  },
  methods: {
    closeLoading(){
      this.$emit('closeLoading')
    },
    handleSelect(key, keyPath) {
      // console.log(key);
      this.activeIndex = key;
      this.$emit("active-index-change", key);
    },
    onContextmenu(event, id) {
      this.activeIndex = id;
      this.$emit("active-index-change", id);

      this.$contextmenu({
        items: [
          {
            label: "解散分组",
            icon: "el-icon-close",
            disabled: this.dismissFlag,
            onClick: () => {
              //console.log("下移一层", this.activeIndex);
              VueEvent.$emit("dismiss_component", this.activeIndex);
            },
          },
          {
            label: "复制图层",
            icon: "el-icon-document-copy",
            onClick: () => {
              //console.log("复制图层", this.activeIndex);
              VueEvent.$emit("copy_component", this.activeIndex);
            },
          },
          {
            label: "删除图层",
            icon: "el-icon-delete",
            onClick: () => {
              //console.log("删除图层", this.activeIndex);
              VueEvent.$emit("delete_component", this.activeIndex);
            },
          },
          {
            label: "置顶图层",
            icon: "el-icon-arrow-up",
            onClick: () => {
              //console.log("置顶图层", this.activeIndex);
              VueEvent.$emit("top_layer", this.activeIndex);
            },
          },
          {
            label: "置底图层",
            icon: "el-icon-arrow-down",
            onClick: () => {
              //console.log("置底图层", this.activeIndex);
              VueEvent.$emit("bottom_layer", this.activeIndex);
            },
          },
          {
            label: "上移一层",
            icon: "el-icon-top",
            onClick: () => {
              //console.log("上移一层", this.activeIndex);
              VueEvent.$emit("up_layer", this.activeIndex);
            },
          },
          {
            label: "下移一层",
            icon: "el-icon-bottom",
            onClick: () => {
              //console.log("下移一层", this.activeIndex);
              VueEvent.$emit("down_layer", this.activeIndex);
            },
          },
          {
            label: this.actData.isUnlocked == false ? "解锁" : "锁定",
            icon:
              this.actData.isUnlocked == false ? "el-icon-key" : "el-icon-lock",
            onClick: () => {
              //console.log("锁定", this.activeIndex);
              VueEvent.$emit("on_locked", this.activeIndex);
            },
          },
          // {
          //   label: "自定义组件上传",
          //   icon: "el-icon-refresh",
          //   disabled: true,
          //   onClick: () => {
          //     console.log("自定义组件上传", this.activeIndex);
          //   }
          // },
        ],
        event,
        //x: event.clientX,
        //y: event.clientY,
        customClass: "custom-class",
        zIndex: 3,
        minWidth: 230,
      });
      return false;
    },
    getChildren(item) {
      console.log(item, "=====item");
      let childrenOptions = item.chartOption;
      let treeData = [];
      let children = {};
      let childrenData = [];
      children.label = item.layerName;
      children.icon = item.icon;
      children.key = item.customId;
      childrenOptions.forEach((element) => {
        let layer = {};
        layer.label = element.layerName;
        layer.key = element.customId;
        layer.icon = element.icon;
        childrenData.push(layer);
      });
      children.children = childrenData;
      treeData.push(children);
      return treeData;
    },
    handleNodeClick(data) {
      if (data.children != undefined) {
        document.getElementById(
          "tree" + data.key
        ).childNodes[0].childNodes[0].childNodes[3].style.color = "#1890ff";
      }
      this.activeIndex = data.key;
      this.$emit("active-index-change", data.key);
    },
    renderContent(h, { node, data, store }) {
      const groupChart = this.drawingList.filter(function (item) {
        return item.customId == data.key;
      })[0];
      if (groupChart.isUnlocked == false) {
        return (
          <span class="custom-tree-node">
            <i class={data.icon}></i>
            <span style="margin-left:5px;">{data.label}</span>
            <span
              class="isLock_position"
              on-click={() => this.unclock(data.key)}
            >
              <i class="el-icon-lock"></i>
            </span>
          </span>
        );
      } else {
        return (
          <span class="custom-tree-node">
            <i class={data.icon}></i>
            <span style="margin-left:5px;">{data.label}</span>
          </span>
        );
      }
    },
    nodeExpandClick(data) {
      this.activeIndex = data.key;
      this.$emit("active-index-change", data.key);
    },
    unclock(comId) {
      VueEvent.$emit("on_locked", comId);
    },
    
  },
};
</script>
<style lang="scss" scoped>
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }
}
::v-deep .el-menu-item.is-active {
  height: auto;
}
::v-deep .isLock_position {
  position: absolute;
  right: 7px;
}
::v-deep .el-tree-node__content {
  display: flex;
  align-items: center;
  height: auto;
  cursor: pointer;
}
::v-deep.el-menu-item {
  height: auto;
  line-height: 56px;
  font-size: 14px;
  color: #303133;
  padding: 0 20px;
  list-style: none;
  cursor: pointer;
  position: relative;
  transition: border-color 0.3s, background-color 0.3s, color 0.3s;
  box-sizing: border-box;
  white-space: nowrap;
}
::v-deep .el-menu-item.is-active {
  color: #1890ff;
}
::v-deep .custom-tree-node {
  width: 100%;
  font-size: 14px;
  position: relative;
}
</style>