<template>
  <div>
    <div class="fun_message">
      <div class="title">企业归属</div>
      <div class="belong_msg">
        <img :src="creator&&creator.Avatar" />
        <span>{{creator&&creator.RealName?creator.RealName:''}}</span>
      </div>
      <div class="handovercon">
        <el-button type="primary" @click="choiceHandover">移交企业</el-button>
      </div>
    </div>
    <div class="fun_message">
      <div class="title">删除企业</div>
      <div class="tips_msg">
        <span>一旦你删除了企业，企业内所有项目、部门、成员，项目中所有内容以及所关联的所有文档将会被永久删除。这是一个不可恢复的操作，请谨慎对待！</span>
      </div>
      <div>
        <el-button type="danger" @click="delOrg">删除企业</el-button>
      </div>
    </div>
    <el-dialog
      title="移交组织"
      :visible.sync="dialogVisible"
      width="300px"
      center
      :append-to-body="false"
      :close-on-click-modal="true"
      :modal-append-to-body="false"
      :destroy-on-close="true"
      :modal="false"
      custom-class="handover-dialog"
      top="0"
    >
      <div class="handover_con" v-loading="searchLoading">
        <el-input placeholder="请输入移交成员用户名、姓名或手机号" v-model="searchValue" @input="selectSearchMember">
          <i slot="prefix" class="el-input__icon el-icon-search"></i>
        </el-input>
        <ul class="user_list" v-if="searchMemberList.length>0">
          <li
            class="user_li"
            v-for="(item,inx) in searchMemberList"
            :key="inx"
            @click="choiceHandoverMem(item)"
          >
            <img :src="item.Avatar" alt />
            <span>{{item.RealName}}</span>
          </li>
        </ul>
        <el-empty description="无任何成员" v-if="searchMemberList.length==0"></el-empty>
      </div>
    </el-dialog>
    <el-dialog
      title="移交企业"
      :visible.sync="tipsDialog"
      width="30%"
      :append-to-body="false"
      :close-on-click-modal="false"
      :modal-append-to-body="false"
      :destroy-on-close="true"
      :modal="true"
    >
      <span>
        确定把企业移交给
        <span style="color:#3572FF">{{selectedMenber.RealName}}</span> ？移交后你的角色将变为成员
      </span>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancelHandel">取 消</el-button>
        <el-button type="primary" @click="confirmHandle">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { getUser } from "@/api/system/user";
import { searchMember, listMember } from "@/api/system/Employee";
import { handoverOrg, removeOrg } from "@/api/system/company";
export default {
  name: "AdminUiOrgotherfun",
  props: {
    org: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      tipsDialog: false,
      searchValue: "",
      dialogVisible: false,
      searchLoading: false,
      searchMemberList: [],
      selectedMenber: {}, //选中交接的成员
      creator: {} //归属人信息
    };
  },

  mounted() {
    this.creator = this.org.Creator;
  },
  methods: {
    delOrg() {
      //删除企业
      this.$modal
        .confirm("是否确认删除企业？")
        .then(()=> {
          return removeOrg({ id: this.creator.OrgId });
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.$emit("reLoadsystem");
        })
        .catch(() => {});
    },
    cancelHandel() {
      //取消移交企业
      this.selectedMenber = {};
      this.tipsDialog = false;
    },
    confirmHandle() {
      //确认移交企业

      handoverOrg({ id: this.creator.OrgId, uid: this.selectedMenber.Id }).then(
        res => {
          if (res.code == 0) {
            this.$modal.msgSuccess("移交成功");
            this.$emit("reLoadOrg");
          }
        }
      );
      this.tipsDialog = false;
    },
    choiceHandoverMem(item) {
      //选择移交的成员
      this.selectedMenber = item;
      this.dialogVisible = false;
      this.tipsDialog = true;
    },
    selectSearchMember(query) {
      //全局搜索指定用户
      // if (query !== "") {
      this.searchLoading = true;
      listMember({ key: query, showAll: true,isPrimaryDept:true }).then(res => {
        // for (let i = 0; i < 10; i++) {
        //   res.data = [...res.data, ...res.data];
        // }

        setTimeout(() => {
          this.searchLoading = false;
          this.searchMemberList = res.data.List.filter(item => {
            return item;
          });
        }, 200);
      });
      // } else {
      //   this.searchMemberList = [];
      // }
    },
    choiceHandover() {
      //打开移交企业选择
      this.dialogVisible = true;
      this.selectSearchMember("");
    },
    getBelongUser(data) {
      //获取归属人信息
      this.creator = data.Creator;
    }
  }
};
</script>

<style lang="scss" scoped>
.handover_con {
  width: 250px;
  height: 352px;
  display: flex;
  flex-direction: column;

  .user_list {
    flex: 1;
    list-style: none;
    overflow-y: scroll;
    height: 316px;
    padding: 0;
    .user_li {
      // height: 40px;
      line-height: 40px;
      display: flex;
      justify-content: flex-start;
      align-items: center;
      padding-left: 20px;
      cursor: pointer;
      padding-bottom: 10px;
      padding: 5px 0 5px 20px;
      img {
        width: 36px;
        height: 36px;
        border-radius: 50%;
        margin-right: 10px;
      }
      &:hover {
        background: #f7f7f7;
      }
    }
    &::-webkit-scrollbar {
      width: 3px;
    }
    /* 滚动槽 */
    &::-webkit-scrollbar-track {
      -webkit-box-shadow: inset006pxrgba(0, 0, 0, 0.3);
      border-radius: 10px;
    }
    /* 滚动条滑块 */
    &::-webkit-scrollbar-thumb {
      border-radius: 10px;
      background: rgba(0, 0, 0, 0.1);
      -webkit-box-shadow: inset006pxrgba(0, 0, 0, 0.5);
    }
  }
}

.fun_message {
  margin-bottom: 30px;
  .el-button {
    border: none;
  }
  .el-button.el-button--danger {
    color: #ff4848 !important ;
    background-color: #ffeded !important;
  }
  .el-button.el-button--primary {
    color: rgba(53, 114, 255, 1) !important;
    background-color: rgba(246, 249, 255, 1) !important;
  }
  .tips_msg {
    color: #333333;
    margin: 20px 0;
  }
  .belong_msg {
    color: #333333;
    margin: 20px 0;
    display: flex;
    align-items: center;
    justify-content: flex-start;
    img {
      width: 44px;
      height: 44px;
      border-radius: 50%;
    }
    span {
      margin-left: 15px;
    }
  }
  .title {
    color: #78829d;
  }
}
</style>