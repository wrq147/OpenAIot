<template>
  <div style="padding: 0 20px;height:40px;text-align: left;line-height: 40px;font-size:14px;color:rgba(255, 255, 255, 1);" @click="toggleClick">
    <!-- <svg-icon
      :class="{'hamburger':true,'is-active':isActive}"
      :icon-class="isActive?'zhankaicebianlan':'shouhuicebianlan'"
      width="64"
      height="64"
    ></svg-icon> -->
    <i class="zhongtaiiconfont" :class="{'zhongtai-icon-zhankaicebianlan':isActive,'zhongtai-icon-shouqicebianlan':!isActive,'hamburger':true,'is-active':isActive}" style="width: 32px;height:40px;text-align: left;line-height: 40px;font-size:14px;color:rgba(255, 255, 255, 0.6);"></i>
    <span>{{title}}</span>
  </div>
</template>

<script>
import { orgInfo } from "@/api/system/company";
export default {
  name: 'Hamburger',
  props: {
    isActive: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      title: '',
    }
  },
  mounted(){
    if (this.$store.state.user.orgId > 0) {
      orgInfo({ id: this.$store.state.user.orgId }).then(res => {
        // this.logo = res.data.Logo;
        // this.logo = res.data.Logo;
        this.title = res.data.OrgName;
      });
    } 
  },
  methods: {
    toggleClick() {
      this.$emit('toggleClick')
    }
  }
}
</script>

<style scoped>
.hamburger {
  display: inline-block;
  vertical-align: middle;
  width: 20px;
  height: 20px;
  color: rgba(255, 255, 255, 0.6);
}

.hamburger.is-active {
  /* transform: rotate(180deg); */
}

</style>
