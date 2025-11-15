<template>
  <section :class="{'app-main':true,'isFixedHea':fixedHeader,'notshowTop':!showTop}" id="app-main">
    <transition name="fade-transform" mode="out-in">
      <!-- <keep-alive :include="cachedViews">
        <router-view :key="key" />
      </keep-alive> -->
      <BaseKeepAlive :include="cachedViews">
        <router-view :key="key" />
      </BaseKeepAlive>
    </transition>
  </section>
</template>

<script>

import BaseKeepAlive from '@/utils/KeepAlive'
export default {
  name: "AppMain",
  components:{BaseKeepAlive},
  computed: {
    fixedHeader() {
      return this.$store.state.settings.fixedHeader;
    },
    cachedViews() {
      return this.$store.state.tagsView.cachedViews;
    },
    key() {
      return this.$route.path;
    },
    showTop(){
      if(this.$route.path=='/index'){
        return false
      }else{
        return true
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.app-main {
  /* 50= navbar  50  */
  // height: calc(100% - 60px);
  min-height: calc(100% - 100px);
  width: 100%;
  position: relative;
  overflow: hidden;
  &.notshowTop{
    min-height: calc(100% - 60px);
  }
}

.fixed-header + .app-main {
  padding-top: 100px;
}
.notShowTop + .app-main {
  padding-top: 60px;
}
.hasTagsView {
  .app-main {
    /* 84 = navbar + tags-view = 50 + 34 */
    min-height: calc(100% - 100px);
  }
  .app-main.isFixedHea {
    height: 100%;
  }

  .fixed-header + .app-main {
    padding-top: 40px;
  }
}
</style>

<style lang="scss">
// fix css style bug in open el-dialog
.el-popup-parent--hidden {
  .fixed-header {
    padding-right: 17px;
  }
}
</style>
