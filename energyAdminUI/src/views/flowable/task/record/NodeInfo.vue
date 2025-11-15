<template>
  <div>
    <div v-if="nodeList.length > 1">
      <div
        v-for="(item, index) in nodeList"
        :key="item.Id"
        :label="'分支' + index"
        :name="index.toString()"
      >
        <el-timeline-item
          :style="{'padding-bottom':!item.Children || item.Children.length==0?'0':'40px'}"
          :icon="item.Status != 3 ? 'el-icon-time' : 'el-icon-check'"
          :color="item.Status != 3 ? '#b3bdbb' : 'rgba(53, 114, 255, 1)'"
          size="large"
          :class="{'last_li':!item.Children || item.Children.length==0}"
        >
          <p
            style="font-weight: 700;margin-top:0;margin-bottom:10px;font-size: 14px;"
          >{{ item.StepName }}</p>
          <div>
            <template v-if="item.ActionUsers != null">
              <div v-for="(userIt, xii) in item.ActionUsers" :key="xii" style="display: flex;flex-direction: row;justify-content: space-between;margin-bottom:10px;">
                <div style="display: flex;flex-direction: row;justify-content: flex-start;">
                  <el-avatar shape="square" :size="53" fit="cover" :src="userIt.Avatar"></el-avatar>
                  <div style=" display: flex; flex-direction: column; justify-content: space-between; margin-left:13px; padding:3px 0;">
                    <span style="line-height: 28px; font-size: 12px">{{userIt.RealName}}</span>
                    <label>{{ userIt.ActionDate }}</label>
                  </div>
                </div>
                <div style="display: flex;flex-direction: row;justify-content: flex-end;align-items: center;">
                  <el-image v-if="userIt.SignImg != null && userIt.SignImg != ''" :src="userIt.SignImg" style="height: 60px; width: 140px; margin-left: 15px" lazy></el-image>
                  <div>
                    <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="success" v-if="userIt.ActionName != '' && userIt.ActionName != '驳回'">{{ userIt.ActionName }}</el-tag>
                    <el-tag type="warning" style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" v-else-if="userIt.ActionName != ''">{{ userIt.ActionName }}</el-tag>
                  </div>
                </div>
              </div>
            </template>
            <template v-if="item.ActionUsers == null&&item.WaitUsers != null">
              <div style="width: 100%;height:260px;" :style="{'overflow-y':item.WaitUsers.length>5?'scroll':'hidden'}">
                <div style="width: 100%;">
                  <div v-for="(userIt, xii) in item.WaitUsers" :key="xii" style="display: flex;flex-direction: row;justify-content: space-between;margin-bottom:10px">
                    <div style="display: flex; justify-content: flex-start">
                      <el-avatar style="width:44px;height:44px;border-radius:22px" shape="square" :size="53" fit="cover" :src="userIt.Avatar"></el-avatar>
                      <div style="display: flex;flex-direction: column;justify-content: space-between;margin-left:13px;padding:3px 0;">
                        <span class="name_text" style="display:block">{{userIt.RealName}}</span>
                        <label class="times_text">{{userIt.ActionDate}}</label>
                      </div>
                    </div>
                    <div style="display: flex;flex-direction: row;justify-content: flex-end;align-items: center;">
                      <el-image v-if="userIt.SignImg != null && userIt.SignImg != ''" :src="userIt.SignImg" style="height: 44px; width: 103px; margin-left: 15px" lazy></el-image>
                      <div>
                        <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="success" v-if="userIt.ActionName != '' && userIt.ActionName != '驳回'">{{ userIt.ActionName }}</el-tag>
                        <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="danger" v-else-if="userIt.ActionName != ''">{{userIt.ActionName}}</el-tag>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </template>
          </div>
        </el-timeline-item>

        <node-info :nodeList="item.Children"></node-info>
      </div>
    </div>
    <div v-else v-for="item in nodeList" :key="item.Id">
      <el-timeline-item
        :style="{'padding-bottom':!item.Children || item.Children.length==0?'0':'40px'}"
        :icon="item.Status != 3 ? 'el-icon-time' : 'el-icon-check'"
        :color="item.Status != 3 ? '#b3bdbb' : 'rgba(53, 114, 255, 1)'"
        size="large"
        :class="{'last_li':!item.Children || item.Children.length==0}"
      >
        <p
          style="font-weight: 700;margin-top:0;margin-bottom:10px;font-size: 14px;"
        >{{ item.StepName }}</p>
        <div>
          <template v-if="item.ActionUsers != null">
            <div v-for="(userIt, xii) in item.ActionUsers" :key="xii" style="display: flex;flex-direction: row;justify-content: space-between;margin-bottom:10px;">
              <div style="display: flex;flex-direction: row;justify-content: flex-start;">
                <el-avatar style="width:44px;height:44px;border-radius:22px" shape="square" :size="53" fit="cover" :src="userIt.Avatar"></el-avatar>
                <div style="display: flex;flex-direction: column;justify-content: space-between;margin-left:13px;padding:3px 0;">
                  <span class="name_text">{{userIt.RealName}}</span>
                  <label class="times_text">{{ userIt.ActionDate }}</label>
                  <el-image v-if="userIt.SignImg != null && userIt.SignImg != ''" :src="userIt.SignImg" style="height: 44px; width: 103px;" lazy></el-image>
                </div>
              </div>
              <div style="display: flex;flex-direction: column;">
                <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="success" v-if="userIt.ActionName != '' && userIt.ActionName != '驳回'">{{userIt.ActionName}}</el-tag>
                <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="danger" v-else-if="userIt.ActionName != ''">{{userIt.ActionName}}</el-tag>
              </div>
            </div>
          </template>
          <template v-if="item.ActionUsers == null&&item.WaitUsers != null">
            <div style="width: 100%;height:260px;" :style="{'overflow-y':item.WaitUsers.length>5?'scroll':'hidden'}">
              <div style="width: 100%;">
                <div v-for="(userIt, xii) in item.WaitUsers" :key="xii" style="display: flex;flex-direction: row;justify-content: space-between;margin-bottom:10px">
                  <div style="display: flex; justify-content: flex-start">
                    <el-avatar style="width:44px;height:44px;border-radius:22px" shape="square" :size="53" fit="cover" :src="userIt.Avatar"></el-avatar>
                    <div style="display: flex;flex-direction: column;justify-content: space-between;margin-left:13px;padding:3px 0;">
                      <span class="name_text" style="display:block">{{userIt.RealName}}</span>
                      <label class="times_text">{{userIt.ActionDate}}</label>
                    </div>
                  </div>
                  <div style="display: flex;flex-direction: row;justify-content: flex-end;align-items: center;">
                    <el-image v-if="userIt.SignImg != null && userIt.SignImg != ''" :src="userIt.SignImg" style="height: 44px; width: 103px; margin-left: 15px" lazy></el-image>
                    <div>
                      <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="success" v-if="userIt.ActionName != '' && userIt.ActionName != '驳回'">{{ userIt.ActionName }}</el-tag>
                      <el-tag style="border-radius:14px;border:none;height:26px;line-height:26px;width:56px;text-align:center;" type="danger" v-else-if="userIt.ActionName != ''">{{userIt.ActionName}}</el-tag>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </template>
        </div>
      </el-timeline-item>

      <node-info :nodeList="item.Children"></node-info>
    </div>
  </div>
</template>

<script>
export default {
  name: "NodeInfo",
  props: {
    nodeList: {
      type: Array,
      default: []
    }
  },
  data() {
    return {};
  },
  methods: {}
};
</script>

<style lang="scss">
// .el-tabs__content{
//   overflow:visible;
// }
</style>
