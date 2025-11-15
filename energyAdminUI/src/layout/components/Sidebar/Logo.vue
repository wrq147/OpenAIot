<template>
  <div class="sidebar-logo-container" :class="{ 'collapse': collapse }"
    :style="{ backgroundColor: sideTheme === 'theme-dark' ? 'transparent' : variables.menuLightBackground }">
    <transition name="sidebarLogoFade">
      <router-link v-if="collapse" key="collapse" class="sidebar-logo-link" to="/">
        <img v-if="logo" src="@/assets/images/name_logo.png" class="sidebar-logo" />
        <!-- <h1 v-else class="sidebar-title"
          :style="{ 'font-weight': '500', color: sideTheme === 'theme-dark' ? variables.logoTitleColor : variables.logoLightTitleColor, }">
          {{ title }} </h1> -->
      </router-link>
      <router-link v-else key="expand" class="sidebar-logo-link" to="/">
        <img v-if="logo" src="@/assets/images/name_logo.png" class="sidebar-logo" />
        <!-- <h1 class="sidebar-title"
          :style="{ 'font-weight': '500', color: sideTheme === 'theme-dark' ? variables.logoTitleColor : variables.logoLightTitleColor, }">
          {{ title }} </h1> -->
      </router-link>
    </transition>
  </div>
</template>

<script>
import { orgInfo } from "@/api/system/company";
import variables from '@/assets/styles/variables.scss'

export default {
  name: 'SidebarLogo',
  props: {
    collapse: {
      type: Boolean,
      required: true
    }
  },
  computed: {
    variables() {
      return variables;
    },
    sideTheme() {
      return this.$store.state.settings.sideTheme
    }
  },
  data() {
    return {
      title: '',
      logo: ''
    }
  },
  mounted() {
    // if (this.$store.state.user.orgId > 0) {
    //   orgInfo({ id: this.$store.state.user.orgId }).then(res => {
    //     // this.logo = res.data.Logo;
    //     this.logo = res.data.Logo;
    //     this.title = res.data.OrgName;
    //   });
    // } //else {
      // this.logo = this.$store.state.user.avatar;
      this.logo = '/assets/images/name_logo.png';
      this.title = this.$store.state.user.name;
    // }
    if(this.$isNotEmpty(this.title)){
      this.title = "后台管理系统";
    }
  },
}
</script>

<style lang="scss" scoped>
.sidebarLogoFade-enter-active {
  transition: opacity 1.5s;
}

.sidebarLogoFade-enter,
.sidebarLogoFade-leave-to {
  opacity: 0;
}

.sidebar-logo-container {
  position: relative;
  width: 100%;
  height: 36px;
  line-height: 36px;
  background: transparent;
  text-align: center;
  overflow: hidden;
  margin-left:20px;

  & .sidebar-logo-link {
    height: 100%;
    width: 100%;
    display: flex;
    align-items: center;

    & .sidebar-logo {
      height: 36px;
      vertical-align: middle;
      margin-right: 16px;
    }

    & .sidebar-title {
      display: inline-block;
      margin: 0;
      color: rgba(255, 255, 255, 0.8);
      font-weight: 600;
      line-height: 52px;
      font-size: var(--fssldelogotext);
      font-family: Avenir, Helvetica Neue, Arial, Helvetica, sans-serif;
      vertical-align: middle;
    }
  }

  &.collapse {
    .sidebar-logo {
      margin-right: 0px;
    }
  }
}
</style>
