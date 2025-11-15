<template>
  <div class="con" style="height:100%">
    <div class="top_select">
      <!-- <el-radio-group v-model="MessageStatus" @input="changeChoice">
        <el-radio :label="-1">全部消息</el-radio>
        <el-radio :label="0">仅显示未读消息</el-radio>
        <el-radio :label="1">仅显示已读消息</el-radio>
      </el-radio-group>-->
      <div :class="{'status_li':true,'active_status':MessageStatus==-1}" @click="setMessageStatus(-1)">全部消息</div>
      <div :class="{'status_li':true,'active_status':MessageStatus==0}" @click="setMessageStatus(0)">未读消息</div>
      <div :class="{'status_li':true,'active_status':MessageStatus==1}" @click="setMessageStatus(1)">已读消息</div>
    </div>
    <div class="messages_con">
      <ul class="messages_ul" v-if="messageLists.length>0">
        <li class="messages_li" v-for="(item,inx) in messageLists" :key="inx" @click="toEveryMessage(item.click_url,item.id,item)">
          <div class="rowFlex">
            <div class="messages_title">
              <span>{{item.label}}</span>
              <el-tag style="border-radius:16px;border:none;" :key="item.type==0?'私有':(item.type==1?'部门':'公告')" :type="item.type==0?'danger':(item.type==1?'warning':'')"
              >{{item.type==0?'私有':(item.type==1?'部门':'公告')}}</el-tag>
            </div>
            <span class="huise">{{item.create_time}}</span>
          </div>
          <div class="msgCon huise">{{item.content}}</div>
        </li>
      </ul>
      <el-empty :description="MessageStatus==-1?'暂无消息':(MessageStatus==1?'暂无已读消息':'暂无未读消息')" v-else-if="status !='loading'&&messageLists.length==0"
        :image-size="200"
      ></el-empty>
      <el-skeleton v-else :rows="6" animated />
    </div>
    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getMessageList"/>
  </div>
</template>

<script>
import { messageList, setRead } from "@/api/message/message";
export default {
  name: "Message",

  data() {
    return {
      MessageStatus: -1, //选中的值
      // 总条数
      total: 0,
      queryParams: {
        pageNum: 1,
        pageSize: 10
      },
      messageLists: [],
      status: "loading"
    };
  },

  mounted() {
    this.getMessageList(); //获取消息列表
  },

  methods: {
    toEveryMessage(routs, id, item) {
      if (item.status == 0) {
        setRead({ id }).then(res => {
          this.$store.commit("SET_changeMessageLoad", true);
          if (res.code == 0) {
            this.jmpToUrl(routs,item);
            this.getMessageList()
          }
        });
      } else {
        this.jmpToUrl(routs,item);
      }
    },
    jmpToUrl(routs,item){
      if (routs) {
        let clickurl= item.click_url.toLowerCase();
        if(clickurl.indexOf("http")==0){
          window.open(clickurl);
        }
        else{
          this.$router.push(routs);
        }
      }
    },
    setMessageStatus(status) {
      this.MessageStatus = status;
      this.queryParams.pageNum = 1;
      this.getMessageList();
    },
    getMessageList() {
      this.status = "loading";
      let datas = {
        status: this.MessageStatus,
        pageNum: this.queryParams.pageNum,
        pageSize: this.queryParams.pageSize
      };
      messageList(datas).then(res => {
        // console.log("消息列表", res);
        if (res.code == 0) {
          this.messageLists = res.data.List;
          this.total = res.data.Total;
          this.status = "finish";
        }
      });
    }
  }
};
</script>

<style lang="scss">
.app-main {
  background: rgba(244, 245, 249, 1);
  // justify-content: center;
}
.con {
  width: 100%;
  //   height: calc(100vh - 84px);
  box-sizing: border-box;
  // justify-content: center;
}
.top_select {
  width: 100%;
  // padding: 0 50px;
  background: #ffffff;
  box-sizing: border-box;
  display: flex;
  justify-content: flex-start;
  // align-items: center;
  // margin-top: 32px;
  padding-left: 30px;
  height: 60px;
  line-height: 60px;
  padding-top: 15px;
  padding-bottom: 16px;
  box-sizing: border-box;
  .status_li {
    // width: 64px;
    height: 29px;
    line-height: 29px;
    margin-right: 50px;
    font-size: 14px;
    color: rgba(51, 51, 51, 1);
    cursor: pointer;
    // padding-top: 10px;
  }
  .active_status {
    border-bottom: 2px solid rgba(53, 114, 255, 1);
    color: rgba(53, 114, 255, 1);
  }
}
.messages_con {
  width: 100%;
  box-sizing: border-box;
  // padding: 20px 0 20px 0;
  justify-content: center;
  // margin-left: -100px;
  .messages_ul {
    width: 100%;
    padding: 0 10px;
    margin: 0 auto;
    box-sizing: border-box;
    .messages_li {
      background: #ffffff;
      width: 100%;
      // width: 60%;
      // height: 50px;
      // line-height: 50px;
      border: none;
      box-shadow: 0 5px 10px rgba(0, 28, 112, 0.04);
      list-style: none;
      margin-top: 10px;
      // display: flex;
      // flex-direction: column;
      // justify-content: space-between;
      box-sizing: border-box;
      padding: 22px 20px 20px 20px;
      z-index: 898;
      border-radius: 6px;
      color: #3c3c3c;
      .rowFlex {
        width: 100%;
        display: flex;
        justify-content: space-between;
        // height: 50px;
        // line-height: 50px;
      }
      .msgCon {
        width: 100%;
        font-size: 14px;
        margin-top: 12px;
      }
      .el-tag {
        margin-left: 20px;
      }
      .huise {
        //字体颜色为灰色
        color: #9a9a9a;
      }
    }
  }
}
// .pagination-container {
//   width: 100%;
//   display: flex;
//   justify-content: right;
//   // background: #ffffff !important;
//   height: 30px;
//   line-height: 30px;
//   margin-top: 18px;
//   box-sizing: border-box;
//   .el-pagination {
//     // position: relative;
//     .el-pager li {
//       background: rgba(0, 0, 0, 0.1);
//     }
//   }
// }
</style>