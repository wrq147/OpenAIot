<template>
  <div id="tags-view-container" class="tags-view-container">
    <scroll-pane ref="scrollPane" :selectedTag="selectedTag" class="tags-view-wrapper" @scroll="handleScroll">
      <router-link v-for="tag in visitedViews" ref="tag" :key="tag.path" :class="{'tag-item': true,active: isActive(tag),}" :to="{ path: tag.path, query: tag.query, fullPath: tag.fullPath }"
        tag="span" class="tags-view-item" :style="activeStyle(tag)" @click.middle.native="!isAffix(tag) ? closeSelectedTag(tag) : ''">
        <i v-if="isAffix(tag)" class="zhongtaiiconfont zhongtai-icon-zhuye" style="margin-right: 8px"></i>
        {{ tag.title }}
        <span v-if="!isAffix(tag)" class="el-icon-close" @click.prevent.stop="closeSelectedTag(tag)"/>
      </router-link>
    </scroll-pane>
  </div>
</template>

<script>
import ScrollPane from "./ScrollPane";
import path from "path";

export default {
  components: { ScrollPane },
  data() {
    return {
      visible: false,
      top: 0,
      left: 0,
      selectedTag: {},
      affixTags: [],
    };
  },
  computed: {
    visitedViews() {
      return this.$store.state.tagsView.visitedViews;
    },
    routes() {
      return this.$store.state.permission.routes;
    },
    theme() {
      return this.$store.state.settings.theme;
    },
  },
  watch: {
    $route() {
      this.addTags();
      this.moveToCurrentTag();
    },
    visible(value) {
      if (value) {
        document.body.addEventListener("click", this.closeMenu);
      } else {
        document.body.removeEventListener("click", this.closeMenu);
      }
    },
  },
  beforeDestroy() {
    document.body.removeEventListener("click", this.closeMenu);
  },
  mounted() {
    this.initTags();
    this.addTags();
  },
  methods: {
    isActive(route) {
      return route.path === this.$route.path;
    },
    activeStyle(tag) {
      if (!this.isActive(tag)) return {};
      return {};
    },
    isAffix(tag) {
      return tag.meta && tag.meta.affix;
    },
    filterAffixTags(routes, basePath = "/") {
      let tags = [];
      routes.forEach((route) => {
        if (route.meta && route.meta.affix) {
          const tagPath = path.resolve(basePath, route.path);
          tags.push({
            fullPath: tagPath,
            path: tagPath,
            name: route.name,
            meta: { ...route.meta },
          });
        }
        if (route.children) {
          const tempTags = this.filterAffixTags(route.children, route.path);
          if (tempTags.length >= 1) {
            tags = [...tags, ...tempTags];
          }
        }
      });
      return tags;
    },
    initTags() {
      const affixTags = (this.affixTags = this.filterAffixTags(this.routes));
      for (const tag of affixTags) {
        // Must have tag name
        if (tag.name) {
          this.$store.dispatch("tagsView/addVisitedView", tag);
        }
      }
    },
    addTags() {
      const { name } = this.$route;
      if (name) {
        this.$store.dispatch("tagsView/addView", this.$route);
      }
      return false;
    },
    moveToCurrentTag() {
      const tags = this.$refs.tag;
      this.$nextTick(() => {
        for (const tag of tags) {
          if (tag.to.path === this.$route.path) {
            this.$refs.scrollPane.moveToTarget(tag);
            // when query is different then update
            if (tag.to.fullPath !== this.$route.fullPath) {
              this.$store.dispatch("tagsView/updateVisitedView", this.$route);
            }
            break;
          }
        }
      });
    },
    closeSelectedTag(view) {
      this.$store.dispatch("tagsView/delView", view).then(({ visitedViews }) => {
        if (this.isActive(view)) {
          this.toLastView(visitedViews, view);
        }
      });
    },
    toLastView(visitedViews, view) {
      const latestView = visitedViews.slice(-1)[0];
      if (latestView) {
        this.$router.push(latestView.fullPath);
      } else {
        // now the default is to redirect to the home page if there is no tags-view,
        // you can adjust it according to your needs.
        if (view.name === "Dashboard") {
          // to reload home page
          this.$router.replace({ path: "/redirect" + view.fullPath });
        } else {
          this.$router.push("/");
        }
      }
    },
    closeMenu() {
      this.visible = false;
    },
    handleScroll() {
      this.closeMenu();
    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep .tags-view-container .scroll-container .el-scrollbar__wrap {
  background-color: #fff !important;
}
/* 在 ScrollPane.vue 或 index.vue 的 <style> 部分 */
::v-deep .el-scrollbar__thumb {
  width: 0 !important; /* 水平滚动条 */
  height: 0 !important; /* 垂直滚动条 */
}
#tags-view-container,
.tags-view-container {
  width: 100%;
  background: #fff;
  display: flex;
  align-items: center;
  height: 42px; /* 设置固定高度 */
  .tags-view-wrapper {
    background: #fff;
    width: 100%;
    height: 100%; /* 确保高度为100% */
    display: flex; /* 使用flex布局 */
    align-items: flex-start; /* 垂直居中对齐 */
    white-space: nowrap; /* 防止换行 */
    overflow-x: hidden; /* 水平滚动 */
    padding: 0; /* 移除内边距 */
    margin: 0; /* 移除外边距 */
    overflow-y: hidden; /* 隐藏垂直滚动条 */
    justify-content: flex-start; /* 确保标签从左到右依次排列 */
    ::v-deep .el-scrollbar__view {
      display: flex;
    }
    .tags-view-item {
      display: inline-flex;
      position: relative;
      cursor: pointer;
      flex-shrink: 0;
      align-items: center;
      justify-content: center;
      // justify-content: space-between; // 横向分布文字与关闭图标
      height: 42px !important;
      line-height: 42px !important;
      border-radius: 0;
      color: #495060;
      background: #fff;
      padding: 0 14px 0 16px !important;
      font-size: 14px;
      //font-size: var(--fsslde);
      margin: 0;
      box-sizing: border-box;
      > span:not(.el-icon-close) {
        margin-right: 16px; // 文字和关闭图标的间距为16px
      }
      i {
        line-height: 42px; // 与标签高度一致
        vertical-align: middle;
      }
      .tags-view-item:first-child {
        margin-left: 0px; /* 第一个标签与容器边缘的间距 */
      }
      // margin-top: 0;
      // margin-bottom: 0;
      // &:first-of-type {
      //   margin-left: 15px;
      // }
      // &:last-of-type {
      //   // margin-right: 15px;
      // }
      &.active {
        text-align: center; //原来的是center
        background: rgba(246, 246, 246, 1);
        color: black;
        justify-content: space-between; /* 保持flex布局特性 */
        margin-left: 0; /* 或者保持与普通标签一致 */
        height: 42px !important;
        line-height: 42px !important;
        padding: 0 14px !important;
        &::before {
          content: "";
          background: rgba(77, 134, 238, 1);
          display: block;
          width: 100%;
          height: 3px;
          position: absolute;
          margin-right: 0;
          top: 0;
          left: 0;
        }
        .el-icon-close {
          // position: absolute;
          right: 12px; // 从右边框留出12px
          top: 50%;
          // transform: translateY(-50%);
          width: 14px; // 与字体大小相同
          height: 14px;
          // z-index: 1;
          // margin-left: auto; // 确保关闭图标始终靠右
          // margin-right: 0;   // 由 padding 控制右边距
        }
        &:hover {
          // background-color: rgba(246, 246, 246, 1) !important;
          // color: #fff !important;
        }
        i {
          font-size: 14px; // 与字体大小相同
        }
      }
    }
  }
  // .router-link-exact-active.router-link-active.active {
  //   height: 34px;
  //   line-height: 34px;
  //   display: inline-flex;
  //   align-items: center;
  //   justify-content: center;
  // }
  // .router-link-exact-active.router-link-active.active::before {
  //   margin-right: 8px;
  // }
  .contextmenu {
    margin: 0;
    background: rgba(255, 255, 255);
    z-index: 3000;
    position: absolute;
    list-style-type: none;
    padding: 5px 0;
    border-radius: 4px;
    font-size: 14px;
    font-weight: 400;
    color: #333;
    box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.3);
    right: 20px;
    top: 14px;
    transform: translateX(-30px);
    min-width: 120px;
    // li {
    //   // margin: 0;
    //   // padding: 0 16px;
    //   // width: 120px;
    //   // cursor: pointer;
    //   // &:hover {
    //   //   background: #eee;
    //   // }
    // }
  }
}
</style>

<style lang="scss">
//reset element css of el-icon-close
.tags-view-container > .scroll-container > .el-scrollbar__wrap {
  height: 57px !important;
  line-height: 42px;
  box-sizing: border-box;
}
.tags-view-wrapper {
  .tags-view-item {
    // width:88px; //新添加的
    height: 42px;
    .el-icon-close {
      //设置关闭标签的样式
      width: 14px;
      height: 14px;
      font-size: 14px;
      // vertical-align: 2px;
      border-radius: 50%;
      text-align: center;
      transition: all 0.3s cubic-bezier(0.645, 0.045, 0.355, 1);
      transform-origin: 100% 50%;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      margin-left: 8px;
      background-color: #bbc0ce;
      color: #fff;
      &:before {
        transform: scale(0.6);
        display: inline-block;
      }
      &:hover {
        background-color: #bbc0ce;
        color: #fff;
      }
    }
  }
}
</style>
