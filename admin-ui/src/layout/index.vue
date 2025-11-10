<template>
  <div
    :class="classObj"
    ref="partentSetting"
    class="app-wrapper"
    :style="{'--current-color': theme}"
  >
    <div v-if="device==='mobile'&&sidebar.opened" class="drawer-bg" @click="handleClickOutside" />
    <sidebar class="sidebar-container" />
    <div :class="{hasTagsView:needTagsView}" class="main-container">
      <div :class="{'fixed-header':fixedHeader}">
        <navbar />
        <tags-view v-if="needTagsView" />
      </div>
      <app-main />
      <right-panel>
        <settings />
      </right-panel>
    </div>
  </div>
</template>

<script>
import ResizeMixin from "./mixin/ResizeHandler";
import { mapState } from "vuex";

let RightPanel = () =>
    import ("@/components/RightPanel")
let AppMain = () =>
    import ("./components/AppMain")
let Navbar = () =>
    import ("./components/Navbar")   
let Settings = () =>
    import ("./components/Settings") 
let Sidebar = () =>
    import ("./components/Sidebar") 
let TagsView = () =>
    import ("./components/TagsView") 



export default {
  name: "Layout",
  components: {
    AppMain,
    Navbar,
    RightPanel,
    Settings,
    Sidebar,
    TagsView
  },
  mixins: [ResizeMixin],
  computed: {
    ...mapState({
      theme: state => state.settings.theme,
      sideTheme: state => state.settings.sideTheme,
      sidebar: state => state.app.sidebar,
      device: state => state.app.device,
      needTagsView: state => state.settings.tagsView,
      fixedHeader: state => state.settings.fixedHeader
    }),
    classObj() {
      return {
        hideSidebar: !this.sidebar.opened,
        openSidebar: this.sidebar.opened,
        withoutAnimation: this.sidebar.withoutAnimation,
        mobile: this.device === "mobile"
      };
    },
    variables() {
      return import ("@/assets/styles/variables.scss");
    }
  },
  watch: {
    "$store.getters.size"() {
      console.log("this.$store.getters.size", this.$store.getters.size);

      this.setFontChange();
    }
  },
  mounted() {
    this.setFontChange();
  },
  methods: {
    handleClickOutside() {
      this.$store.dispatch("app/closeSideBar", { withoutAnimation: false });
    },
    setFontChange() {
      if (this.$store.getters.size == "default" || this.$store.getters.size == "medium") {
        
        this.$refs.partentSetting.style.setProperty("--shaixuan", "14px");
        this.$refs.partentSetting.style.setProperty("--fslist", "18px");
        this.$refs.partentSetting.style.setProperty("--fsslde", "16px");
        this.$refs.partentSetting.style.setProperty("--rightcon", "14px");
        this.$refs.partentSetting.style.setProperty("--fsmini", "12px");
        this.$refs.partentSetting.style.setProperty("--sidehei", "56px");
      } else if (this.$store.getters.size == "small") {
        this.$refs.partentSetting.style.setProperty("--shaixuan", "12px");
        this.$refs.partentSetting.style.setProperty("--fslist", "16px");
        this.$refs.partentSetting.style.setProperty("--fsslde", "14px");
        this.$refs.partentSetting.style.setProperty("--rightcon", "12px");
        this.$refs.partentSetting.style.setProperty("--fsmini", "12px");
        this.$refs.partentSetting.style.setProperty("--sidehei", "56px");
      } else if (this.$store.getters.size == "mini") {
        this.$refs.partentSetting.style.setProperty("--shaixuan", "12px");
        this.$refs.partentSetting.style.setProperty("--fslist", "14px");
        this.$refs.partentSetting.style.setProperty("--fsslde", "12px");
        this.$refs.partentSetting.style.setProperty("--rightcon", "12px");
        this.$refs.partentSetting.style.setProperty("--fsmini", "10px");
        this.$refs.partentSetting.style.setProperty("--sidehei", "46px");
      } else {
        this.$refs.partentSetting.style.setProperty("--shaixuan", "14px");
        this.$refs.partentSetting.style.setProperty("--fslist", "20px");
        this.$refs.partentSetting.style.setProperty("--fsslde", "18px");
        this.$refs.partentSetting.style.setProperty("--rightcon", "16px");
        this.$refs.partentSetting.style.setProperty("--fsmini", "14px");
        this.$refs.partentSetting.style.setProperty("--sidehei", "66px");
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "~@/assets/styles/mixin.scss";
@import "~@/assets/styles/variables.scss";
.main-container {
  background-color: #f4f5f9;
  height: 100%;
  box-sizing: border-box;
}
.app-wrapper {
  @include clearfix;
  position: relative;
  height: 100%;
  width: 100%;
  --fslist: 20px;
  --shaixuan:14px;
  --fsslde: 18px;
  --fsmini: 14px;
  --sidehei: 66px;
  --rightcon:16px;
  &.mobile.openSidebar {
    position: fixed;
    top: 0;
  }
}

.drawer-bg {
  background: #000;
  opacity: 0.3;
  width: 100%;
  top: 0;
  height: 100%;
  position: absolute;
  z-index: 999;
}

.fixed-header {
  position: fixed;
  top: 0;
  right: 0;
  z-index: 9;
  width: calc(100% - #{$base-sidebar-width});
  transition: width 0.28s;
  background: #ffffff;
}

.hideSidebar .fixed-header {
  width: calc(100% - 54px);
}

.mobile .fixed-header {
  width: 100%;
}
.sidebar-container {
  box-shadow: none !important;
}
path {
  fill: inherit !important;
} //设置后可修改svg-icon的颜色
</style>
