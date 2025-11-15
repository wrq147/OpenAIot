<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="elbiaoge_elform tab_con" :style="{'min-height':tableConHeight+60+'px'}">
      <div class="tabs_ul">
        <div class="tabs_li" :class="{'active':activeTabs=='energy'}" @click="setActiveTabs('energy')">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='energy'">
          <span>能源分析</span>
        </div>
        <div class="tabs_li" :class="{'active':activeTabs=='demand'}" @click="setActiveTabs('demand')">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='demand'">
          <span>需量分析</span>
        </div>
      </div>
      <div class="cont_con">
        <energy v-if="activeTabs=='energy'"></energy>
        <demand v-if="activeTabs=='demand'"></demand>
      </div>
      
    </div>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import energy from './cmp/energy.vue'
import demand from './cmp/demand.vue'
export default {
  name: 'EnergyAdminUIIndex',
  mixins: [resizeTableCon],
  components:{demand,energy},
  data() {
    return {
      activeTabs:'energy',
      queryParams:{}
    };
  },

  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.queryParams.OrgId=this.orgId
  },

  methods: {
    setActiveTabs(val){
      this.activeTabs=val
    },
  },
};
</script>

<style lang="less" scoped>
.elbiaoge_elform.tab_con{
  padding: 0 20px;
  width: 100%;
  box-sizing: border-box;
  .tabs_ul{
    width: 100%;
    display: flex;
    align-items: center;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    height: 55px;
    
    .tabs_li{
      line-height: 16px;
      display: flex;
      align-items: center;
      margin-right: 32px;
      cursor: pointer;
      img{
        width: 16px;
        height: 16px;
        margin-right: 8px;
      }
      span{
        font-size: 16px;
        color: rgba(255, 255, 255, 0.6);
      }
      &.active{
        span{
          color: rgba(255, 255, 255, 1);
        }
      }
    }
  }
  .cont_con{
    // width: calc(100% - 170px);
    width: 100%;
    // padding-top: 24px;
  }
}
</style>