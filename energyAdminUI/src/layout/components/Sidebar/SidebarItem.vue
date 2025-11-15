<template>
  <div v-if="!item.hidden">
    <template v-if="hasOneShowingChild(item.children,item) && (!onlyOneChild.children||onlyOneChild.noShowingChildren)&&!item.alwaysShow">
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path, onlyOneChild.query)">
        <el-menu-item :index="resolvePath(onlyOneChild.path)"
          :class="{'setpadding':true,'noDark':settings.sideTheme !== 'theme-dark','submenu-title-noDropdown':!isNest,'isDarkACtiveMenu':(this.$route.path==resolvePath(onlyOneChild.path)&&settings.sideTheme === 'theme-dark'),'isACtiveMenu':(this.$route.path.indexOf(resolvePath(onlyOneChild.path))>-1&&settings.sideTheme !== 'theme-dark')}">
          <!-- <div class="shuxian" v-show="this.$route.path==resolvePath(onlyOneChild.path)&&settings.sideTheme === 'theme-dark'"></div> -->
          <item :is-nest="isNest" :icon="onlyOneChild.meta.icon||(item.meta&&item.meta.icon)" :title="onlyOneChild.meta.title" :isAvtive="this.$route.path==resolvePath(onlyOneChild.path)"/>
        </el-menu-item>
      </app-link>
    </template>

    <el-submenu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <template slot="title">
        <item v-if="item.meta" :icon="item.meta && item.meta.icon" :title="item.meta.title" />
      </template>
      <sidebar-item v-for="child in item.children" :key="child.path" :is-nest="true" :item="child" :base-path="resolvePath(child.path)" class="nest-menu"/>
    </el-submenu>
  </div>
</template>

<script>
import path from "path";
import { isExternal } from "@/utils/validate";
import Item from "./Item";
import AppLink from "./Link";
import FixiOSBug from "./FixiOSBug";
import { mapGetters, mapState } from "vuex";
export default {
  name: "SidebarItem",
  components: { Item, AppLink },
  mixins: [FixiOSBug],
  props: {
    // route object
    item: {
      type: [Object,Function],
      required: true
    },
    isNest: {
      type: Boolean,
      default: false
    },
    basePath: {
      type: String,
      default: ""
    }
  },
  computed: {
    ...mapState(["settings"])
  },
  data() {
    this.onlyOneChild = null;
    return {
      activeRoute: "",
      activeStyle: '{ color: "#5B8EFF"}',
      styles: '{ color: "#ffffff"}'
    };
  },
  mounted(){
  },
  methods: {
    hasOneShowingChild(children = [], parent) {
      if (!children) {
        children = [];
      }
      const showingChildren = children.filter(item => {
        if (item.hidden) {
          return false;
        } else {
          // Temp set(will be used if only has one showing child)
          this.onlyOneChild = item;
          // console.log('路由',item);

          return true;
        }
      });

      // When there is only one child router, the child router is displayed by default
      if (showingChildren.length === 1) {
        return true;
      }

      // Show parent if there are no child router to display
      if (showingChildren.length === 0) {
        this.onlyOneChild = { ...parent, path: "", noShowingChildren: true };
        return true;
      }

      return false;
    },
    resolvePath(routePath, routeQuery) {
      if (isExternal(routePath)) {
        return routePath;
      }
      if (isExternal(this.basePath)) {
        return this.basePath;
      }
      if (routeQuery) {
        let query = JSON.parse(routeQuery);
        return { path: path.resolve(this.basePath, routePath), query: query };
      }

      return path.resolve(this.basePath, routePath);
    }
  }
};
</script>
<style lang="scss">
  /* .svg-icon,
  #app .sidebar-container .el-menu-item .svg-icon,
  #app .sidebar-container .el-submenu__title .svg-icon,
  #app .sidebar-container .el-menu--collapse .svg-icon {
    font-size: 14px !important; // 与首页一致
    color: #ffffff !important;
    width: 14px;
    height: 14px;
    display: inline-block;
    vertical-align: middle;
  } */
.shuxian {
  background-color: rgba(79, 125, 255, 1);
  width: 4px;
  height: var(--sidehei);
  position: absolute;
  top: 0;
  padding: 0 20px 0 16px;
  display: block;
}

#app .sidebar-container  .submenu-title-noDropdown,#app .sidebar-container .el-menu-item{
  font-size: var(--fsslde);
  height: var(--sidehei);
  line-height: var(--sidehei);
  // padding-left: 16px !important;
}
 /* #app .sidebar-container .el-submenu{
   font-size: 16px;
 } */
#app .sidebar-container .el-submenu__title{
  font-size: var(--fsslde);
  color: rgb(255, 255, 255,0.6) !important;
  // padding-left: 16px !important;
}
#app .sidebar-container .el-submenu__title > i.zhongtaiiconfont{
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container .theme-dark .submenu-title-noDropdown:hover {
  background-color: rgba(61, 185, 143, 1) !important;
  color: #ffffff !important;
}
#app .sidebar-container .theme-light li.is-active > .el-submenu__title {
  // background-color: rgba(79, 125, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container .theme-light .submenu-title-noDropdown:hover {
  background-color: #ffffff !important;
  color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .theme-dark li.is-active > .el-submenu__title {
  // background-color: rgba(79, 125, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container .theme-dark .el-menu--collapse li.is-active > .el-submenu__title{
  color: rgba(255, 255, 255, 1) !important;
  background-color: rgba(61, 185, 143, 1) !important;
}
#app .sidebar-container .theme-dark .el-menu--collapse li.is-active > .el-submenu__title > i.zhongtaiiconfont{
  color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container li.is-active > .el-submenu__title > .svg-icon {
  color: rgba(255, 255, 255, 0.6) !important;
}

// #app .sidebar-container .submenu-title-noDropdown:hover.noDark:hover, #app .sidebar-container .el-submenu__title:hover{
//   background-color: #ffffff !important;
//   color: #333333 !important;
// }

// .noDark:hover {
//   background-color: #ffffff !important;
//   color: #333333;
// }
#app .sidebar-container .el-menu-item.isACtiveMenu {
  background-color: #ffffff !important;
  color: rgba(255, 255, 255, 1) !important;
}
// #app .sidebar-container .isDarkACtiveMenu:hover {
//   background-color: rgba(79, 125, 255, 0.2) !important;
//   color: #ffffff !important;
// }
#app .sidebar-container .el-menu-item.isDarkACtiveMenu {
  background-color: rgba(61, 185, 143, 1) !important;
}
#app .sidebar-container .el-menu-item.isACtiveMenu {
  background-color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .el-menu-item.isACtiveMenu i.zhongtaiiconfont {
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container .isDarkACtiveMenu:hover i.zhongtaiiconfont {
  color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .el-menu-item.isDarkACtiveMenu i.zhongtaiiconfont {
  color: rgba(255, 255, 255, 1) !important;
}
#app
  .sidebar-container
  .submenu-title-noDropdown:hover.noDark:hover
  > i.zhongtaiiconfont {
  // fill:rgba(255, 255, 255, 0.8) !important;
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container .submenu-title-noDropdown:hover > i.zhongtaiiconfont{
  color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .theme-dark .submenu-title-noDropdown:hover > .shuxian {
  display: block;
}
// #app .sidebar-container .theme-dark .el-submenu .nest-menu{
//   height: 50px;
// }
#app .sidebar-container .theme-dark .el-submenu .nest-menu{
  // background-color: #1F2D3D;

  .el-menu-item{
    // padding-left: 40px !important;
  }
}
#app .sidebar-container .theme-dark .el-submenu .nest-menu .el-menu-item:hover {
  background-color: rgba(61, 185, 143, 1) !important;
  color: #ffffff !important;
}
#app .sidebar-container .theme-dark .el-submenu .nest-menu .el-menu-item:hover>i.zhongtaiiconfont {
  color: rgba(255, 255, 255, 1) !important;
}
#app
  .sidebar-container
  .theme-light
  .el-submenu
  .nest-menu
  .el-menu-item:hover {
  background-color: rgba(255, 255, 255, 1) !important;
  color: #333333 !important;
}
#app
  .sidebar-container
  .theme-light
  .el-submenu
  .nest-menu
  .el-menu-item:hover> .svg-icon{
  color: rgba(255, 255, 255, 0.6) !important;
}
#app
  .sidebar-container
  .theme-dark
  .el-submenu
  .nest-menu
  .el-menu-item.is-active {
  background-color: rgba(61, 185, 143, 1) !important;
}
#app
  .sidebar-container
  .theme-light
  .el-submenu
  .nest-menu
  .el-menu-item.is-active {
  background-color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .theme-dark .el-submenu__title:hover {
  background-color: rgba(61, 185, 143, 1) !important;
  color: rgba(255, 255, 255, 1) !important;
}
// #app .sidebar-container .theme-dark .el-submenu__title:hover>.svg-icon {

//   color: rgba(255, 255, 255, 0.8) !important;
// }
#app .sidebar-container .theme-light .el-submenu__title:hover>.svg-icon {
  color: rgba(255, 255, 255, 0.6) !important;
}
#app .sidebar-container a{
  margin: 0 auto;
  padding: 0;
  display: block;
}
/* 收起状态下所有菜单项统一宽高、居中显示 */
#app .sidebar-container:not(.expand-logo) .el-menu--collapse > .el-menu-item,
#app .sidebar-container:not(.expand-logo) .el-menu--collapse > .el-submenu > .el-submenu__title {
  width: 46px;
  height: 48px;
  line-height: 48px;
  display: flex;
  justify-content: center; // 水平居中
  align-items: center;     // 垂直居中
  text-align: center;
  padding: 0 !important;
  background-color: rgba(61, 185, 143, 1) !important;
  color: #ffffff !important;

  .svg-icon {
    font-size: 14px !important;
    color: rgba(255, 255, 255, 0.6) !important;
    /* margin-right: 0 !important; */
    /* vertical-align: middle; */
     margin: 0 !important;
     /* margin: 0 auto; */
    display: block;
    width: 14px;
    height: 14px;
  }

  span {
    display: none;
  }

  .el-submenu__icon-arrow {
    display: none;
  }
}

/* 子菜单弹出层样式统一 */
#app .sidebar-container:not(.expand-logo) .el-menu--collapse > .el-submenu .el-menu {
  min-width: 46px !important;
  background-color: rgba(61, 185, 143, 1) !important;
}

/* 子菜单项样式 */
#app .sidebar-container:not(.expand-logo) .el-menu--collapse > .el-submenu .el-menu-item {
  width: 46px;
  height: 34px;
  line-height: 34px;
  padding: 0 !important;
  text-align: center;
  background-color: rgba(61, 185, 143, 1) !important;
  color: #ffffff !important;

  .svg-icon {
    font-size: 14px !important;
    color: rgba(255, 255, 255, 0.6) !important;
    margin: 0 auto;
    display: block;
  }

  span {
    display: none;
  }
}

/* 图标垂直居中
.svg-icon,
#app .sidebar-container .el-submenu__title .svg-icon {
  display: inline-block;
  vertical-align: middle;
} */

</style>
