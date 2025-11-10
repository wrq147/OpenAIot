<template>
  <div v-if="!item.hidden">
    <template
      v-if="hasOneShowingChild(item.children,item) && (!onlyOneChild.children||onlyOneChild.noShowingChildren)&&!item.alwaysShow"
    >
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path, onlyOneChild.query)">
        <el-menu-item
          :index="resolvePath(onlyOneChild.path)"
          :class="{'setpadding':true,'noDark':settings.sideTheme !== 'theme-dark','submenu-title-noDropdown':!isNest,'isDarkACtiveMenu':(this.$route.path==resolvePath(onlyOneChild.path)&&settings.sideTheme === 'theme-dark'),'isACtiveMenu':(this.$route.path.indexOf(resolvePath(onlyOneChild.path))>-1&&settings.sideTheme !== 'theme-dark')}"
        >
          <div
            class="shuxian"
            v-show="this.$route.path==resolvePath(onlyOneChild.path)&&settings.sideTheme === 'theme-dark'"
          ></div>
          <item
            :icon="onlyOneChild.meta.icon||(item.meta&&item.meta.icon)"
            :title="onlyOneChild.meta.title"
            :isAvtive="this.$route.path==resolvePath(onlyOneChild.path)"
          />
        </el-menu-item>
      </app-link>
    </template>

    <el-submenu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <template slot="title">
        <item v-if="item.meta" :icon="item.meta && item.meta.icon" :title="item.meta.title" />
      </template>
      <sidebar-item
        v-for="child in item.children"
        :key="child.path"
        :is-nest="true"
        :item="child"
        :base-path="resolvePath(child.path)"
        class="nest-menu"
      />
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
.shuxian {
  background-color: rgba(79, 125, 255, 1);
  width: 4px;
  height: var(--sidehei);
  position: absolute;
  top: 0;
  left: 0px;
  display: block;
}

#app .sidebar-container  .submenu-title-noDropdown,#app .sidebar-container .el-menu-item{
  font-size: var(--fsslde);
  height: var(--sidehei);
  line-height: var(--sidehei);
  padding-left: 30px !important;
}
// #app .sidebar-container .el-submenu{
//   font-size: 16px;
// }
#app .sidebar-container .el-submenu__title{
  font-size: var(--fsslde);
  padding-left: 30px !important;
}
#app .sidebar-container .theme-dark .submenu-title-noDropdown:hover {
  background-color: rgba(79, 125, 255, 0.2) !important;
  color: #ffffff !important;
}
#app .sidebar-container .theme-light li.is-active > .el-submenu__title {
  // background-color: rgba(79, 125, 255, 0.2) !important;
  color: #6aa5ff !important;
}
#app .sidebar-container .theme-light .submenu-title-noDropdown:hover {
  background-color: #ffffff !important;
  color: #333333 !important;
}
#app .sidebar-container .theme-dark li.is-active > .el-submenu__title {
  // background-color: rgba(79, 125, 255, 0.2) !important;
  color: #6aa5ff !important;
}
#app .sidebar-container li.is-active > .el-submenu__title > .svg-icon {
  color: #6aa5ff !important;
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
  color: #333333 !important;
}
// #app .sidebar-container .isDarkACtiveMenu:hover {
//   background-color: rgba(79, 125, 255, 0.2) !important;
//   color: #ffffff !important;
// }
#app .sidebar-container .el-menu-item.isDarkACtiveMenu {
  background-color: rgba(79, 125, 255, 0.2) !important;
}
#app .sidebar-container .el-menu-item.isACtiveMenu {
  background-color: rgba(255, 255, 255, 1) !important;
}
#app .sidebar-container .el-menu-item.isACtiveMenu .svg-icon {
  color: #6aa5ff !important;
}
#app .sidebar-container .isDarkACtiveMenu:hover .svg-icon {
  color: #6aa5ff !important;
}
#app .sidebar-container .el-menu-item.isDarkACtiveMenu .svg-icon {
  color: #6aa5ff !important;
}
#app
  .sidebar-container
  .submenu-title-noDropdown:hover.noDark:hover
  > .svg-icon {
  // fill:#6aa5ff !important;
  color: #6aa5ff !important;
}
#app .sidebar-container .submenu-title-noDropdown:hover > .svg-icon {
  color: #6aa5ff !important;
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
    padding-left: 40px !important;
  }
}
#app .sidebar-container .theme-dark .el-submenu .nest-menu .el-menu-item:hover {
  background-color: rgba(79, 125, 255, 0.2) !important;
  color: #ffffff !important;
}
#app .sidebar-container .theme-dark .el-submenu .nest-menu .el-menu-item:hover>.svg-icon {
  color: #6aa5ff !important;
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
  color: #6aa5ff !important;
}
#app
  .sidebar-container
  .theme-dark
  .el-submenu
  .nest-menu
  .el-menu-item.is-active {
  background-color: rgba(79, 125, 255, 0.2) !important;
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
  background-color: rgba(79, 125, 255, 0.2) !important;
  color: #6aa5ff !important;
}
// #app .sidebar-container .theme-dark .el-submenu__title:hover>.svg-icon {
  
//   color: #6aa5ff !important;
// }
#app .sidebar-container .theme-light .el-submenu__title:hover>.svg-icon {
  color: #6aa5ff !important;
}
#app .sidebar-container a{
  margin: 0 auto;
  padding: 0;
  display: block;
}
</style>
