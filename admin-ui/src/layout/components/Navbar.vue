<template>
  <div class="navbar">
    <div class="navbar_left" v-if="!isOrg">
      <hamburger id="hamburger-container" :is-active="sidebar.opened" class="hamburger-container"
        @toggleClick="toggleSideBar" />

      <breadcrumb id="breadcrumb-container" class="breadcrumb-container" v-if="!topNav" />
      <top-nav id="topmenu-container" class="topmenu-container" v-if="topNav" />
    </div>
    <div v-if="isOrg">
      <div style="padding:20px" class="org_nav">
        <!-- <svg-icon icon-class="qiyemingcheng" style="color:#B1A3A8"></svg-icon> -->
        <i class="zhongtaiiconfont zhongtai-icon-qiyemingcheng" style="color:#B1A3A8"></i>
        <div class="org_logo">
          <img :src="org.Logo" alt style="width:40px;height:40px;border-radius:50%;" />
        </div>
        <span class="org_name">{{ org.OrgName }}</span>
      </div>
    </div>

    <div class="right-menu">
      <template v-if="device !== 'mobile'">
        <search id="header-search" class="right-menu-item" />
        <screenfull id="screenfull" class="right-menu-item hover-effect" />
        <el-tooltip content="布局大小" effect="dark" placement="bottom">
          <size-select id="size-select" class="right-menu-item hover-effect" />
        </el-tooltip>
        <el-tooltip content="AI助手" effect="dark" placement="bottom"
          v-if="checkPermission(['/LLMService/AI/Assistant'])">
          <div class="right-menu-item hover-effect" @click="openAIAssistant" style="font-size: 14px; color:#7D8598;">
            AI助手
          </div>
        </el-tooltip>
      </template>
      <el-dropdown v-if="checkPermission(['/MsgSrv/Message/List'])" class="right-menu-item hover-effect"
        trigger="click">
        <div>
          <!-- <svg-icon icon-class="xiaoxizhongxin" color="#7D8598" /> -->
          <i class="zhongtaiiconfont zhongtai-icon-xiaoxizhongxin" style="color:#7D8598"></i>
          <div class="yuan1" v-if="noReadCount > 0">{{ noReadCount }}</div>
        </div>
        <el-dropdown-menu slot="dropdown" class="message_ul">
          <el-dropdown-item class="message_li first_li">
            <span class="largeWords">通知</span>
            <span class="otherColor" @click="readAll" v-if="noReadList.length > 0">全部已读</span>
          </el-dropdown-item>
          <el-dropdown-item class="message_li">
            <ul class="infinite-list" style="overflow: scroll; overflow-x: hidden" v-if="noReadList.length > 0">
              <li v-for="(i, ix) in noReadList" :key="ix" class="infinite-list-item">
                <div @click="toEveryMessage(i.click_url, i.id, i)">
                  <div class="noRead_title">{{ i.label }}</div>
                  <div class="msgContent huise">{{ i.content }}</div>
                  <div class="times huise">{{ i.create_time }}</div>
                </div>
              </li>
            </ul>
            <div class="noMessage" v-else>暂无数据</div>
          </el-dropdown-item>
          <el-dropdown-item class="message_li last_li">
            <div @click="toMessageList" class="otherColor" style="width: 100%; height: 100%; text-align: center">前往信息中心
            </div>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>

      <el-dropdown class="avatar-container right-menu-item" trigger="click">
        <div class="avatar-wrapper">
          <el-avatar shape="square" :size="35" fit="cover" :src="avatar"></el-avatar>
          <!-- <i class="xiajiantou" /> -->
          <svg-icon icon-class="xiajiantou" class="avatar-xiajiantou" style="color:#BBC0CD" />
          <!-- <i class="zhongtaiiconfont zhongtai-icon-xiajiantou avatar-xiajiantou" color="#BBC0CD" ></i> -->
        </div>
        <el-dropdown-menu slot="dropdown" style="width: 240px !important">
          <el-dropdown-item class="cmp-blk">
            <div class="cmp-title">企业</div>
            <div class="cmp-item" v-for="orgitem in orgList" :key="orgitem.Id" @click="onSwitchOrg(orgitem.Id)">
              <el-avatar :size="20" shape="square" fit="cover" :src="orgitem.Logo"></el-avatar>
              <span class="txt">{{ orgitem.OrgName }}</span>
              <i class="el-icon-check ric" v-if="curOrgId == orgitem.Id"></i>
            </div>
            <div class="cmp-add" @click="toNewOrg">
              <i class="el-icon-plus" style="font-size: 18px; margin-right: 10px"></i>
              <span>新建企业</span>
            </div>
          </el-dropdown-item>
          <router-link to="/user/profile">
            <el-dropdown-item divided>
              <div class="cp-li">
                <i class="el-icon-user ic"></i>
                <span>账号设置</span>
              </div>
            </el-dropdown-item>
          </router-link>
          <!-- <el-dropdown-item class="cp-li" @click.native="setting = true">
            <i class="el-icon-s-operation ic"></i>
            <span>布局设置</span>
          </el-dropdown-item> -->
          <el-dropdown-item class="cp-li" @click.native="logout" style="color:#e62412">
            <i class="el-icon-switch-button ic"></i>
            <span>退出登录</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import Breadcrumb from "@/components/Breadcrumb";
import TopNav from "@/components/TopNav";
import Hamburger from "@/components/Hamburger";
import Screenfull from "@/components/Screenfull";
import SizeSelect from "@/components/SizeSelect";
import Search from "@/components/HeaderSearch";
import {
  getNoRead,
  setRead,
  noReadCount,
  setReadAll
} from "@/api/message/message";
import { recentHistory } from "@/api/llmchat";
import { switchOrg } from "@/api/system/user";
import { loginThemeInfo } from '@/utils/theme'
import { checkPermi } from "@/utils/permission"; // 权限判断函数
import { Loading } from 'element-ui';
export default {
  components: {
    Breadcrumb,
    TopNav,
    Hamburger,
    Screenfull,
    SizeSelect,
    Search
  },
  props: {
    isOrg: {
      type: Boolean,
      default: false
    },
    org: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      noReadCount: 0,
      noReadList: [],
    };
  },
  computed: {
    ...mapGetters(["sidebar", "avatar", "device", "name"]),
    setting: {
      get() {
        return this.$store.state.settings.showSettings;
      },
      set(val) {
        this.$store.dispatch("settings/changeSetting", {
          key: "showSettings",
          value: val
        });
      }
    },
    topNav: {
      get() {
        return this.$store.state.settings.topNav;
      }
    },
    curOrgId() {
      return this.$store.getters.orgId;
    },
    orgList() {
      //
      let lis = this.reqOrgLis();
      return this.$store.state.orgLis.orgList;
    },
    visitedViews() {
      return this.$store.state.tagsView.visitedViews;
    },
    ischangeMessageLoad() {
      return this.$store.state.user.ischangeMessageLoad;
    }
  },
  watch: {
    $route: {
      // $route可以用引号，也可以不用引号
      async handler(to, from) {
        if (from == undefined) {
          //从登入页面进入重新更新企业列表
          this.$store.commit("orgLis/SET_ORG_LIST", null);
        }
        if (this.$store.state.user.changeOrgId) {
          this.onSwitchOrg(this.$store.state.user.changeOrgId)
          this.$store.commit("SET_CHANGE_ORG", '');
        }

        if (!this.$store.state.user.orgId) {
          let orgList = await this.reqOrgLis()
          // console.log(orgList,'企业列表');
          if (orgList && orgList[0]) {
            this.onSwitchOrg(orgList[0].Id)
          }

        }
      },
      deep: true, // 深度监听
      immediate: true // 第一次初始化渲染就可以监听到
    },
    ischangeMessageLoad: {
      handler(to, from) {
        if (this.ischangeMessageLoad) {
          this.$store.commit("SET_changeMessageLoad", false);
          this.getNoReadCount();
          this.getNoReadList();
        }
      }
    }

  },
  mounted() {
    if (checkPermi(["/LLMService/AI/Assistant"])) {
      //初始化助手的聊天记录
      recentHistory().then(res => {
        this.$store.commit("llm/inithis", res.data);
      });
    }
    if (checkPermi(["/MsgSrv/Message/List"])) {
      this.getNoReadCount();
      this.getNoReadList();
      //订阅刷新消息
      this.$store.dispatch("mqttclient/getClient").then((client) => {
        let tkey = "user/" + this.$store.getters.uid + "/new";
        client.subscribe(tkey, (error) => {
          if (!error) {
            this.$store.commit("mqttclient/Add_Handler", {
              key: tkey,
              func: (message) => {
                if (message == null) {
                  this.getNoReadCount();
                  this.getNoReadList();
                  return;
                }
                let msgcont = message.toString();
                if (msgcont == "") {
                  this.getNoReadCount();
                  this.getNoReadList();
                }
                else {
                  if (msgcont.startsWith("#llm")) {
                    let isThink = msgcont.startsWith("#llmt");
                    //接收到AI助手回复
                    msgcont = msgcont.substring(5);
                    if (msgcont != "") {
                      this.$store.commit("llm/pushmsg", { data: msgcont, isthink: isThink });
                    }
                    else {
                      this.$store.commit("llm/finishmsg", "");
                    }
                  }
                }
              },
            });
          }
        });
      });
    }

  },
  methods: {
    openAIAssistant() {
      this.$router.push("/report/ai/assistant");
    },
    async reqOrgLis() {
      let list = await this.$store.dispatch("orgLis/setOrgList");
      return list;
    },
    toNewOrg() {
      //跳转到新建企业
      this.$router.push("/newOrg");
    },
    onSwitchOrg(id) {
      if (this.curOrgId == id) return;
      switchOrg({ id: id })
        .then(rsp => {
          //重新初始化本地用户信息
          // console.log("链接",window.location.href,window.location.protocol,window.location.pathname,this.$route);
          let url = window.location.protocol + "//" + window.location.host;
          window.location.replace(url); //切换企业后刷新整个网站
          // commit('SET_ROLES', [])
          this.$store.commit("SET_ROLES", []);
          return this.$store.dispatch("GetInfo");
        })
        .then(rsp => {
          //切换企业后重新刷新加载当前页面
          this.$store
            .dispatch("tagsView/delCachedView", this.$route)
            .then(() => {
              const { fullPath } = this.$route;
              this.$nextTick(() => {
                this.$router.replace({
                  path: "/redirect" + fullPath
                });
              });
            });
        });
    },
    toEveryMessage(routs, id, item) {
      setRead({ id }).then(res => {
        if (res.code == 0) {
          this.jmpToUrl(routs, item);
          this.getNoReadCount();
          this.getNoReadList();
        }
      });
    },
    jmpToUrl(routs, item) {
      if (routs) {
        let clickurl = item.click_url.toLowerCase();
        if (clickurl.indexOf("http") == 0) {
          window.open(clickurl);
        }
        else {
          this.$router.push(routs);
        }
      }
    },
    toMessageList() {
      this.$router.push("/report/message/index");
    },
    checkPermission(perms) {
      // console.log("权限管理", checkPermi(perms));

      return checkPermi(perms);
    },
    getNoReadCount() {
      //获取未读数量
      noReadCount().then(res => {
        // console.log("消息未读数量", res);
        this.noReadCount = res.data;
        this.$forceUpdate()
      });
    },
    getNoReadList() {
      //获取前30条未读数量
      getNoRead({ top: 30 }).then(res => {
        // console.log("前30条未读消息", res);
        this.noReadList = res.data;
        this.$forceUpdate()
      });
    },
    readAll() {
      //全部已读
      setReadAll().then(res => {
        // console.log("设置消息全部已读", res);
        if (res.code == 0) {
          this.getNoReadCount();
          this.getNoReadList();
        }
      });
    },
    readMessage() {
      //设置指定消息已读
    },
    toggleSideBar() {
      this.$store.dispatch("app/toggleSideBar");
    },
    async logout() {
      this.$confirm("确定注销并退出系统吗？", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(async () => {

          await this.closeAllTags(); //在退出系统之前关闭所有页面
          let logUid = this.$store.getters.uid
          this.$store.dispatch("LogOut").then(async () => {
            sessionStorage.setItem('wxloginout', true);
            // this.$router.replace(`/login?redirect=${this.$route.fullPath}`);
            this.$store.dispatch("mqttclient/getClient").then((client) => {
              let tkey = "user/" + logUid + "/new";
              client.unsubscribe(tkey, (error) => {
                console.log("消息取消订阅", error);
                this.$store.commit("mqttclient/Del_Handler", tkey);
              });
            });
            let loadingInstance = Loading.service({ fullscreen: true });
            let loginUrl = await loginThemeInfo(this.$store.state.user.orgId)
            if (loginUrl) {
              if (loginUrl.indexOf('?') > -1) {
                window.location.href = `${loginUrl}&redirect=${this.$route.fullPath}`
                window.location.reload()
              } else {
                window.location.href = `${loginUrl}?redirect=${this.$route.fullPath}`
                window.location.reload()
              }

            } else {
              this.$router.replace(`/login?redirect=${this.$route.fullPath}`);
            }
            this.$nextTick(() => { // 以服务的方式调用的 Loading 需要异步关闭
              loadingInstance.close();
            });
          });
        })
        .catch(() => { });
    },
    async closeAllTags() {
      let { visitedViews } = await this.$store.dispatch("tagsView/delAllViews");
      this.$router.replace({ path: "/redirect/index" });
    }
  }
};
</script>
<style lang="scss">
.message_ul {
  left: calc(100vw - 340px) !important;
  position: relative;
}

.message_ul .popper__arrow::after {
  right: 60px;
  border-left: 15px solid transparent;
  border-right: 15px solid transparent;
  border-bottom: 11px solid #ffffff;
  top: -5px;
  // border: none;
}

.message_ul .popper__arrow {
  box-sizing: border-box;
  // left: calc(50vw) !important;
  // border-left: 15px solid transparent;
  // border-right: 15px solid transparent;
  // border-bottom: 11px solid #ffffff;
  // top: -1px;
  border-color: #ffffff;
  border: none;
}
</style>
<style lang="scss" scoped>
// .right-menu{
//   .svg-icon{
//     font-size: 22px !important;
//   }
// }
.navbar {
  background-color: #ffffff;
}

.message_ul {
  // background-color: #3dacfe;
  // color: #3dacfe;
  // border: 1rpx solid blue;
  position: absolute;
  top: 48px !important;
  width: 300px !important;
  // height: 400px;
  // height: 120px;
  // padding-bottom: 20px;
  padding: 0;
  border: none;
  box-sizing: border-box;
  box-shadow: 5px rgba(0, 44, 63, 0.1);
  // overflow: hidden;
}

.yuan1 {
  background-color: red;
  width: 16px;
  height: 16px;
  text-align: center;
  line-height: 16px;
  border-radius: 10px;
  color: #ffffff;
  position: absolute;
  top: 5px;
  right: 7px;
  font-size: var(--fsmini);
}

.message_li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(144, 147, 153, 0.3);
  padding: 0 !important;

  .otherColor {
    color: #6aa5ff;
  }

  .largeWords {
    // font-size: 16px;
  }

  .noMessage {
    height: 150px;
    width: 100%;
    padding: auto;
    margin: 0 auto;
    line-height: 150px;
    text-align: center;
  }

  .infinite-list {
    max-height: 300px;
    width: 100%;
    padding: auto;
    margin: 0 auto;
    box-sizing: border-box;
    padding: 0 6px 0 17px;
    visibility: top;

    &::-webkit-scrollbar {
      width: 6px;
      height: 6px;
    }

    // 滚动条的轨道的两端按钮，允许通过点击微调小方块的位置。
    &::-webkit-scrollbar-button {
      display: none;
    }

    // 滚动条的轨道（里面装有Thumb）
    &::-webkit-scrollbar-track {
      background: transparent;
    }

    // 滚动条的轨道（里面装有Thumb）
    &::-webkit-scrollbar-track-piece {
      background-color: transparent;
    }

    // 滚动条里面的小方块，能向上向下移动（或往左往右移动，取决于是垂直滚动条还是水平滚动条）
    &::-webkit-scrollbar-thumb {
      background: rgba(144, 147, 153, 0.3);
      cursor: pointer;
      border-radius: 4px;
    }

    // 边角，即两个滚动条的交汇处
    &::-webkit-scrollbar-corner {
      display: none;
    }

    // 两个滚动条的交汇处上用于通过拖动调整元素大小的小控件
    &::-webkit-resizer {
      display: none;
    }

    .infinite-list-item {
      list-style: none;
      // height: 50px;
      width: 100%;
      box-sizing: border-box;
      border-bottom: 1px solid rgba(144, 147, 153, 0.3);
      padding: 5px 0;
      line-height: 24px;

      .noRead_title {
        //单行溢出设置
        white-space: nowrap; // 文本不会换行。
        overflow: hidden; // 溢出多余裁剪
        text-overflow: ellipsis; // 显示省略符号来代表被修剪的文本。
        line-height: 24px;
      }

      .msgContent {
        // font-size: 14px;
        max-height: 36px;
        display: -webkit-box;
        line-height: 18px;
        -webkit-box-orient: vertical;
        /* 表示盒子对象的子元素的排列方式 */
        -webkit-line-clamp: 2;
        /* 限制文本的行数，表示文本第多少行省略 */
        text-overflow: ellipsis;
        /*  打点展示 */
        overflow: hidden;
        /*超出部分进行隐藏*/
      }

      .times {
        margin-top: 2px;
      }

      .huise {
        //字体颜色为灰色
        color: #9a9a9a;
      }
    }

    .infinite-list-item:last-child {
      border-bottom: none;
    }
  }
}

.message_li:hover {
  background-color: #ffffff !important;
  box-sizing: border-box;
  width: 100%;
  color: #606266 !important;
}

.first_li,
.last_li {
  height: 50px;
  line-height: 50px !important;
}

.last_li {
  border-radius: 0 0 5px 5px;
  border: none;
  justify-content: center;
}

.first_li {
  padding: 0 17px !important;
  border-radius: 5px 5px 0 0;
}

// .message_li:hover span {
//   // color: #fff;
// }

.cmp-blk:hover {
  background-color: #ffffff !important;
  color: #606266;
}

.cmp-blk {
  padding: 0 !important;

  .cmp-title {
    padding: 0 17px;
    font-size: 12px;
    color: #bfbfbf;
  }

  .cmp-item:hover {
    background-color: #e8f4ff;
    color: #6aa5ff;
  }

  .cmp-item {
    padding: 0 17px;
    display: flex;
    align-items: center;
    height: 36px;

    .txt {
      flex: 1 1 auto;
      margin-left: 8px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .ric {
      color: #6aa5ff;
      font-size: 16px;
      margin-left: 10px;
    }
  }

  .cmp-add {
    padding: 0 17px;
    display: flex;
    align-items: center;
    color: #6aa5ff;
    height: 36px;
  }

  .cmp-add:hover {
    background-color: #e8f4ff;
    color: #6aa5ff;
  }
}

.cp-li {
  display: flex;
  height: 36px;
  align-items: center;

  .ic {
    margin-right: 10px;
    font-size: 18px;
  }
}

.navbar {
  height: 60px;
  line-height: 60px;
  overflow: hidden;
  position: relative;
  // padding: 11px 0;
  border: none;
  box-sizing: border-box;
  // background: #fff;
  // box-shadow: 0 1px 4px rgba(0, 21, 41, 0.12);
  display: flex;
  justify-content: space-between;
  align-items: center;

  .navbar_left {
    display: flex;
    align-items: center;
  }

  .org_nav {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    margin-left: 30px;

    .org_name {
      font-size: 12px;
      font-weight: 600;
      margin-left: 10px;
    }

    .org_logo {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      margin-left: 10px;
    }
  }

  .hamburger-container {
    line-height: 45px;
    height: 45px;
    float: left;
    cursor: pointer;
    transition: background 0.3s;
    -webkit-tap-highlight-color: transparent;
    font-size: var(--fslist);

    &:hover {
      background: rgba(0, 0, 0, 0.025);
    }
  }

  .breadcrumb-container {
    float: left;
  }

  .el-breadcrumb.app-breadcrumb {
    // font-size: 18px;//固定头部导航栏字体样式
    font-size: var(--fsslde);
  }

  .topmenu-container {
    position: absolute;
    left: 50px;
  }

  .errLog-container {
    display: inline-block;
    vertical-align: top;
  }

  .right-menu {
    float: right;
    height: 45px;
    line-height: 45px;

    &:focus {
      outline: none;
    }

    .right-menu-item {
      display: inline-block;
      padding: 0 15px;
      height: 100%;
      // font-size: 18px;
      font-size: var(--fslist);
      color: #5a5e66;
      vertical-align: text-bottom;
      box-sizing: border-box;

      &.hover-effect {
        cursor: pointer;
        transition: background 0.3s;

        &:hover {
          background: rgba(0, 0, 0, 0.025);
        }
      }
    }

    .avatar-container {
      margin-right: 30px;

      .avatar-wrapper {
        margin-top: 5px;
        position: relative;
        cursor: pointer;

        .el-icon-caret-bottom {
          cursor: pointer;
          position: absolute;
          right: -20px;
          top: 25px;
          // font-size: 12px;
        }

        .avatar-xiajiantou {
          //头像旁边的箭头的样式
          position: absolute;
          top: 13px;
          right: -22px;
          width: 14px;
          height: 8px;
        }
      }
    }
  }
}
</style>
