<template>
  <div class="tags-view-container">
    <!-- 左侧滚动按钮 -->
    <div class="scroll-button left" @click="scrollLeft">
      <i class="zhongtaiiconfont zhongtai-icon-jinru" style="font-size: 14px;" :style="{ color: '#333333' }"></i>
    </div>

    <!-- 滚动内容区域 -->
    <el-scrollbar ref="scrollContainer" :vertical="false" class="scroll-container" @wheel.native.prevent="handleScroll">
      <slot></slot>
    </el-scrollbar>

    <div class="right-container">
      <div class="scroll-button right" @click="scrollRight">
        <i class="zhongtaiiconfont zhongtai-icon-a-jinru1" style="font-size: 14px;" :style="{ color: '#333333' }"></i>
      </div>
      <el-dropdown popper-append-to-body :popper-style="{ zIndex: 9999 }" @command="handleTagsViewCommand">
        <span class="el-dropdown-link">
          <i class="zhongtaiiconfont zhongtai-icon-a-xiajiantouda" style="font-size: 6px; color: #333333"></i>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="closeCurrent" v-if="!isAffix()">关闭当前标签页</el-dropdown-item>
            <el-dropdown-item command="closeOthers">关闭其他标签页</el-dropdown-item>
            <el-dropdown-item command="closeAll">关闭所有标签页</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script>
const tagAndTagSpacing = 4;
import path from "path";
export default {
  data() {
    return {
      left: 0,
      tagList: [
        { name: "home", title: "首页", path: "/" }, // 初始化首页为固定标签
      ],
    };
  },
  computed: {
    scrollWrapper() {
      return this.$refs.scrollContainer.$refs.wrap;
    },
    routes() {
      return this.$store.state.permission.routes;
    },
  },
  mounted() {
    this.scrollWrapper.addEventListener("scroll", this.emitScroll, true);
  },
  beforeDestroy() {
    this.scrollWrapper.removeEventListener("scroll", this.emitScroll);
  },
  methods: {
    addTags() {
      const { name } = this.$route;
      if (name) {
        this.$store.dispatch("tagsView/addView", this.$route);
      }
      return false;
    },
    isAffix() {
      return this.$route.meta && this.$route.meta.affix;
    },
    isActive(tag) {
      return tag.path === this.$route.path;
    },
    handleClick(tag) {
      this.$router.push({ path: tag.path }); // 跳转路由
    },
    moveToCurrentTag() {//路由保持当前页面
      const tags = this.$parent.$refs.tag;
      this.$nextTick(() => {
        for (const tag of tags) {
          if (tag.to.path === this.$route.path) {
            this.moveToTarget(tag);
            // when query is different then update
            if (tag.to.fullPath !== this.$route.fullPath) {
              this.$store.dispatch("tagsView/updateVisitedView", this.$route);
            }
            break;
          }
        }
      });
    },
    closeOthersTags() {
      //关闭其他
      const tagList = this.$parent.$refs.tag;
      let currentTag = tagList.find((row) => row.to.path === this.$route.path);
      this.$router.push(currentTag).catch(() => {});
      this.$store.dispatch("tagsView/delOthersViews", currentTag).then(() => {
        this.$store.dispatch("tagsView/addView", this.$route);//当前路由的页面也已经被关闭，需要添加回来
        this.moveToCurrentTag()
      });
    },
    closeCurrentTag(view) {
      this.$store.dispatch("tagsView/delView", view).then(({ visitedViews }) => {
        if (this.isActive(view)) {
          this.toLastView(visitedViews, view);
        }
      })
      .catch((err) => {
        console.error("删除标签失败:", err);
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
    //  },
    handleTagsViewCommand(command) {
      switch (command) {
        case "closeCurrent":
          const selected = this.$route;
          // console.log('【closeCurrent】selectedTag =', selected); // 可选：用于调试
          this.closeCurrentTag(selected);//关闭当前
          break;
        case "closeOthers":
          this.closeOthersTags();//关闭其他
          break;
        case "closeAll"://关闭所有页面
          this.$store.dispatch("tagsView/delAllViews").then(({ visitedViews }) => {
            this.affixTags = this.filterAffixTags(this.routes)
            if (this.affixTags.some(tag => tag.path === this.$route.path)) {
              return;
            }
            this.toLastView(visitedViews, this.$route);

          });
          break;
        default:
          break;
      }
    },
    filterAffixTags(routes, basePath = "/") {
      let tags = [];
      routes.forEach(route => {
        if (route.meta && route.meta.affix) {
          const tagPath = path.resolve(basePath, route.path);
          tags.push({
            fullPath: tagPath,
            path: tagPath,
            name: route.name,
            meta: { ...route.meta }
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
    scrollLeft() {
      this.scrollWrapper.scrollLeft -= 100;
    },
    scrollRight() {
      this.scrollWrapper.scrollLeft += 100;
    },
    emitScroll() {
      this.$emit("scroll");
    },
    moveToTarget(currentTag) {
      const $container = this.$refs.scrollContainer.$el
      const $containerWidth = $container.offsetWidth
      const $scrollWrapper = this.scrollWrapper
      const tagList = this.$parent.$refs.tag
      let firstTag = null
      let lastTag = null

      // find first tag and last tag
      if (tagList.length > 0) {
        firstTag = tagList[0]
        lastTag = tagList[tagList.length - 1]
      }

      if (firstTag === currentTag) {
        $scrollWrapper.scrollLeft = 0
      } else if (lastTag === currentTag) {
        $scrollWrapper.scrollLeft = $scrollWrapper.scrollWidth - $containerWidth
      } else {
        // find preTag and nextTag
        const currentIndex = tagList.findIndex(item => item === currentTag)
        const prevTag = tagList[currentIndex - 1]
        const nextTag = tagList[currentIndex + 1]

        // the tag's offsetLeft after of nextTag
        const afterNextTagOffsetLeft = nextTag.$el.offsetLeft + nextTag.$el.offsetWidth + tagAndTagSpacing

        // the tag's offsetLeft before of prevTag
        const beforePrevTagOffsetLeft = prevTag.$el.offsetLeft - tagAndTagSpacing

        if (afterNextTagOffsetLeft > $scrollWrapper.scrollLeft + $containerWidth) {
          $scrollWrapper.scrollLeft = afterNextTagOffsetLeft - $containerWidth
        } else if (beforePrevTagOffsetLeft < $scrollWrapper.scrollLeft) {
          $scrollWrapper.scrollLeft = beforePrevTagOffsetLeft
        }
      }
    },
    handleScroll(e) {
      const eventDelta = e.wheelDelta || -e.deltaY * 40
      const $scrollWrapper = this.scrollWrapper
      $scrollWrapper.scrollLeft = $scrollWrapper.scrollLeft + eventDelta / 4
    },
  },
};
</script>

<style lang="scss" scoped>
.tags-view-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 42px;
  max-width: 100%;
  background-color: #fff;
  padding: 0 8px;
  .scroll-button {
    width: 40px;
    height: 42px;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    color: #666;
    // margin: 0 8px;

    &:hover {
      background-color: #f5f7fa;
    }
  }

  .right-container {
    height: 42px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 80px;
    flex-shrink: 0;

    .scroll-button.right,
    .el-dropdown-link {
      width: 40px;
      height: 42px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }

  .scroll-container {
    min-width: 200px;
    flex-grow: 1;
    white-space: nowrap;
    position: relative;
    overflow: hidden;
    width: 100%;
    font-size: 14px;
    color: rgba(51, 51, 51, 1);
    background-color: rgba(246, 246, 246, 1);
    height: 42px;
  }
  .tag-item {
    position: relative;
    padding-right: 25px;
    display: inline-flex;
    align-items: center; // 垂直居中图标和文字
    justify-content: center; // 水平居中
    padding: 0 16px !important;
    height: 46px;
    line-height: 46px;
    margin-right: 10px !important;
    margin-left: 10px !important; // 统一左边距
    background-color: rgba(246, 246, 246, 1);
    transition: all 0.3s;
    font-size: 14px;
    font-family: "Microsoft YaHei", sans-serif;
    flex-shrink: 0; // 防止标签被压缩

    &.active {
      background-color: #fff;
      box-shadow: 0px -2px 6px rgba(0, 0, 0, 0.1);
      // position: relative;

      &::before {
        content: "";
        position: absolute;
        top: -3px;
        left: 0;
        width: 100%;
        height: 3px;
        background-color: rgba(77, 134, 238, 1) !important;
        z-index: 1;
      }
    }

    // &:last-child {
    //     margin-right: 0; /* 移除最后一个标签的右边距 */
    // }
  }

  // 👇 插入点：添加 .close-icon 样式
  .close-icon {
    position: absolute;
    right: 12px;
    top: 50%;
    transform: translateY(-50%);
    font-size: 12px;
    color: #999;
    cursor: pointer;

    &:hover {
      color: red;
    }
  }

  .nav-icon,
  .el-dropdown-link {
    line-height: 46px;
    vertical-align: middle;
  }
}
</style>
