<template>
  <div style="padding: 0 15px;" @click="toggleClick">
    <!-- <svg-icon
      :class="{'hamburger':true,'is-active':isActive}"
      :icon-class="isActive?'zhankaicebianlan':'shouhuicebianlan'"
      width="64"
      height="64"
    ></svg-icon> -->
    <i class="zhongtaiiconfont" :class="{'zhongtai-icon-zhankaicebianlan':isActive,'zhongtai-icon-shouhuicebianlan':!isActive,'hamburger':true,'is-active':isActive}" style="width: 32px;height:32px;text-align: center;line-height: 32px;"></i>
  </div>
</template>

<script>
export default {
  name: 'Hamburger',
  props: {
    isActive: {
      type: Boolean,
      default: false
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
  color: #78829D;
}

.hamburger.is-active {
  transform: rotate(180deg);
}
</style>
