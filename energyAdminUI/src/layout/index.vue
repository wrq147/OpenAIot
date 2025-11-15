<template>
  <div class="top_cot_con" ref="top_cot_con">
    <div class="navbar_con"> 
      <navbar />
    </div>
    <div class="navbar_zhanwei"></div>
    <div :class="classObj" ref="partentSetting" class="app-wrapper" :style="{'--current-color': theme}">
      <div v-if="device==='mobile'&&sidebar.opened" class="drawer-bg" @click="handleClickOutside" />
      <sidebar class="sidebar-container" />
      <div :class="{hasTagsView:needTagsView}" class="main-container">
        <div :class="{'fixed-header':fixedHeader}" v-if="showTop">
          <breadcrumb id="breadcrumb-container" class="breadcrumb-container" v-if="!topNav" />
        </div>
        <div :class="{'notShowTop':!showTop}" v-else>
        </div>
        <app-main />
        <right-panel><settings /></right-panel>
      </div>
    </div>
  </div>
</template>

<script>
import ResizeMixin from "./mixin/ResizeHandler";
import { mapState } from "vuex";
import Breadcrumb from "@/components/Breadcrumb";
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
    TagsView,
    Breadcrumb
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
    },
    topNav: {
      get() {
        return this.$store.state.settings.topNav;
      }
    },
    showTop(){
      if(this.$route.path=='/index'){
        return false
      }else{
        return true
      }
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
        
        this.$refs.top_cot_con.style.setProperty("--shaixuan", "14px");
        this.$refs.top_cot_con.style.setProperty("--fslist", "14px");
        this.$refs.top_cot_con.style.setProperty("--fsslde", "14px");
        this.$refs.top_cot_con.style.setProperty("--fssldelogotext", "16px");
        this.$refs.top_cot_con.style.setProperty("--rightcon", "14px");
        this.$refs.top_cot_con.style.setProperty("--fsmini", "12px");
        this.$refs.top_cot_con.style.setProperty("--sidehei", "48px");
      } else if (this.$store.getters.size == "small") {
        this.$refs.top_cot_con.style.setProperty("--shaixuan", "12px");
        this.$refs.top_cot_con.style.setProperty("--fslist", "16px");
        this.$refs.top_cot_con.style.setProperty("--fsslde", "14px");
        this.$refs.top_cot_con.style.setProperty("--fssldelogotext", "16px");
        this.$refs.top_cot_con.style.setProperty("--rightcon", "12px");
        this.$refs.top_cot_con.style.setProperty("--fsmini", "12px");
        this.$refs.top_cot_con.style.setProperty("--sidehei", "48px");
      } else if (this.$store.getters.size == "mini") {
        this.$refs.top_cot_con.style.setProperty("--shaixuan", "12px");
        this.$refs.top_cot_con.style.setProperty("--fslist", "14px");
        this.$refs.top_cot_con.style.setProperty("--fsslde", "14px");
        this.$refs.top_cot_con.style.setProperty("--fssldelogotext", "16px");
        this.$refs.top_cot_con.style.setProperty("--rightcon", "12px");
        this.$refs.top_cot_con.style.setProperty("--fsmini", "10px");
        this.$refs.top_cot_con.style.setProperty("--sidehei", "48px");
      } else {
        this.$refs.top_cot_con.style.setProperty("--shaixuan", "14px");
        this.$refs.top_cot_con.style.setProperty("--fslist", "20px");
        this.$refs.top_cot_con.style.setProperty("--fsslde", "14px");
        this.$refs.top_cot_con.style.setProperty("--fssldelogotext", "16px");
        this.$refs.top_cot_con.style.setProperty("--rightcon", "16px");
        this.$refs.top_cot_con.style.setProperty("--fsmini", "14px");
        this.$refs.top_cot_con.style.setProperty("--sidehei", "48px");
      }
    }
  }
};
</script>

<style lang="scss" scoped>
@import "~@/assets/styles/mixin.scss";
@import "~@/assets/styles/variables.scss";
.navbar_con{
  position: fixed;
  top: 0;
  left: 0;
  z-index: 9;
  width: 100%;
}
.navbar_zhanwei{
  // height: 60px;
}
.main-container {
  background-color: #f4f5f9;
  height: 100%;
  box-sizing: border-box;
}
.top_cot_con{
  --fslist: 20px;
  --shaixuan:14px;
  --fsslde: 14px;
  --fsmini: 14px;
  --sidehei: 48px;
  --rightcon:16px;
  height: 100%
}
.app-wrapper {
  @include clearfix;
  position: relative;
  height: 100%;
  width: 100%;
  
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
  top: 60px;
  right: 0;
  z-index: 9;
  width: calc(100% - #{$base-sidebar-width});
  transition: width 0.28s;
  height: 40px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  box-sizing: border-box;
  background: rgba(19, 25, 34, 1);
}

.hideSidebar .fixed-header {
  width: calc(100% - 54px);
}

.mobile .fixed-header {
  width: 100%;
}
.sidebar-container {
  box-shadow: none !important;
  top: 60px;
}
</style>
