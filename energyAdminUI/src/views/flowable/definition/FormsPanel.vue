<template>
  <div class="from-panel" ref="panel" v-loading="copyloading">
    <div style="margin: 20px 20px 0 20px;background: #ffffff;border-radius: 4px;box-shadow:0 2px 12px 0 rgba(0, 0, 0, 0.1);">
      <div class="from-title">
        <div>
          <span style="font-size: 16px">流程面板</span>
        </div>
        <div>
          <span style="color: #ff3535; margin-right: 20px">
            <i class="zhongtaiiconfont zhongtai-icon-a-tishi" style="font-size:14px;margin-right: 6px"></i>
            中台表单流程1.0正式上线啦！
          </span>
          <el-button size="mini" @click="newProcess(null)">
            <!-- <svg-icon icon-class="xinzeng"></svg-icon> -->
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng" style="font-size:12px"></i>
            <span style="margin-left: 6px">新建流程</span>
          </el-button>
          <el-button class="bleBg" @click="addGroup" size="mini">
            <!-- <svg-icon icon-class="xinzeng"></svg-icon> -->
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng" style="font-size:12px"></i>
            <span style="margin-left: 6px">新建分组</span>
          </el-button>
        </div>
      </div>

      <draggable :list="groups" group="group" handle=".el-icon-rank" filter=".undrag" @start="groupsSort = true"
        :options="{
          animation: 300,
          sort: true,
          scroll: true,
          chosenClass: 'choose',
        }"
        @end="groupSort"
      >
        <div :class="{'form-group': true,undrag: group.Id === 0 || group.Id === undefined,}"
          v-show="group.Id > 0 || group.Items.length > 0"
          v-for="(group, gidx) in groups" :key="gidx">
          <div class="form-group-title">
            <div>
              <span>{{ group.Name }}</span>
              <span>({{ group.Items.length }})</span>
              <i class="el-icon-rank" title="长按拖动可对分组排序"></i>
            </div>

            <div>
              <div v-if="group.Id > 0">
                <el-button class="group_button" type="text" @click="newProcess(group.Id)">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng" style="font-size:12px;margin-right: 6px"></i>创建新流程
                </el-button>
              </div>
              <div v-if="!(group.Id === 0 || group.Id === undefined)">
                <el-dropdown>
                  <el-button type="text" class="group_button">
                    <i class="zhongtaiiconfont zhongtai-icon-xiugai" style="font-size:12px;margin-right: 6px"></i>编辑分组
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item icon="el-icon-edit-outline" @click.native="editGroup(group)">修改名称</el-dropdown-item>
                    <el-dropdown-item icon="el-icon-delete" @click.native="delGroup(group)">删除分组</el-dropdown-item>
                  </el-dropdown-menu>
                </el-dropdown>
              </div>
            </div>
          </div>
          <draggable style="width: 100%" :list="group.Items" group="from" @end="groupSort" v-show="!groupsSort" filter=".undrag"
            :options="{
              animation: 300,
              delay: 200,
              chosenClass: 'choose',
              scroll: true,
              sort: true,
            }"
          >
            <div :class="{ 'form-group-item': true, undrag: item.Status == 1 }"
              v-for="(item, index) in group.Items" :key="index" title="长按0.5S后可拖拽表单进行排序">
              <div>
                <i :class="item.Icon" :style="'background: ' + item.Background"></i>
                <span>{{ item.Name }}</span>
                <br />
              </div>
              <div class="desp">{{ item.remark }}</div>
              <div>
                <span>最后更新时间：{{ item.updateTime }}</span>
              </div>
              <div class="button_con_mini">
                <el-button type="text" size="mini" v-if="checkPermission(['/FlowService/Flow/Record'])" @click="applicationRecord(item, group)">记录</el-button>
                <el-button type="text" size="mini" @click="editFrom(item, group)">编辑</el-button>
                <el-button type="text" size="mini" @click="copyItem(item, group)">复制</el-button>

                <el-popover placement="left" trigger="click" width="400" style="margin-left: 10px" @show="moveSelect === null" v-if="item.Status != 1">
                  <el-radio-group v-model="moveSelect" size="mini">
                    <el-radio :label="g.Id" border v-for="(g, zzidx) in groups" :key="zzidx"
                      v-show="g.Id > 0" :disabled="g.Id === group.Id" style="margin: 10px">{{ g.Name }}</el-radio>
                  </el-radio-group>
                  <div style="text-align: right; margin: 0">
                    <el-button type="primary" size="mini" @click="moveFrom(item)">提交</el-button>
                  </div>
                  <el-button slot="reference" type="text" size="mini">移动</el-button>
                </el-popover>
                <el-button type="text" size="mini" @click="moveFrom(item)" v-if="item.Status == 1">删除</el-button>
                <el-button type="text" size="mini" @click="stopFrom(item)"
                  :style="{color:item.Status == 1 ? 'rgba(101, 214, 140, 1)' : 'rgba(53, 114, 255, 1)',}">{{ item.Status == 1 ? "启用" : "停用" }}</el-button>
              </div>
            </div>
          </draggable>
          <!-- <div style="text-align: center" v-if="group.Id>0">
          <el-button
            style="padding-top: 0"
            type="text"
            icon="el-icon-plus"
            @click="newProcess(group.Id)"
          >创建新流程</el-button>
        </div>-->
        </div>
      </draggable>
    </div>
  </div>
</template>

<script>
import draggable from "vuedraggable"; //拖拽组件
import {
  getFormGroups,
  groupItemsSort,
  delGroup,
  addGroup,
  updateGroup,
  updateForm,
  delForm,
  getFormDetail,
} from "@/api/flowable/design";
import { checkPermi } from "@/utils/permission"; // 权限判断函数
export default {
  name: "FormsPanel",
  components: { draggable },
  data() {
    return {
      moveSelect: "",
      visible: false,
      groupsSort: false,
      groups: [],
      copyloading:false
    };
  },
  mounted() {
    this.getGroups();
  },
  methods: {
    checkPermission(perms) {
      return checkPermi(perms);
    },
    getGroups() {
      getFormGroups({ withItems: true }).then((rsp) => {
        this.groups = rsp.data;
      });
    },
    newProcess(groupId) {
      this.$store.commit("setIsEdit", false);
      this.$store.commit('tagsView/DEL_CACHED_VIEW',{path:"/definition/design"})
      this.$nextTick(()=>{
        if (groupId != null) {
          this.$router.push("/definition/design?group=" + groupId);
        } else {
          this.$router.push("/definition/design");
        }
      });

    },
    groupSort() {
      this.groupsSort = false;
      let sortList = [];
      this.groups.forEach((it) => {
        if (it.Id > 0) {
          sortList.push(it.Id);
        }
      });
      groupItemsSort(sortList)
        .then((rsp) => {
          this.$message.success("操作成功");
          this.getGroups();
        })
        .catch((err) => {
          this.getGroups();
        });
    },
    addGroup() {
      this.$prompt("请输入要添加的组名", "新的分组名", {
        confirmButtonText: "提交",
        cancelButtonText: "取消",
        inputPattern: /^[\u4E00-\u9FA5A-Za-z0-9\\-]{1,30}$/,
        inputErrorMessage: "分组名不能为空且长度小于30",
        inputPlaceholder: "请输入分组名",
        closeOnClickModal:false
      }).then(({ value }) => {
        addGroup({ Name: value }).then((rsp) => {
          this.$message.success("操作成功");
          this.getGroups();
        });
      });
    },
    delGroup(group) {
      this.$confirm(
        "删除分组并不会删除表单，表单将会被转移到 “其他” 分组，确定要删除分组 " +
          group.Name +
          "?",
        "提示",
        {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }
      ).then(() => {
        delGroup({ id: group.Id }).then((rsp) => {
          this.$message.success("操作成功");
          this.getGroups();
        });
      });
    },
    editGroup(group) {
      this.$prompt("请输入新的组名", "修改分组名", {
        confirmButtonText: "提交",
        cancelButtonText: "取消",
        inputPattern: /^[\u4E00-\u9FA5A-Za-z0-9\\-]{1,30}$/,
        inputErrorMessage: "分组名不能为空且长度小于30",
        inputPlaceholder: "请输入分组名",
        inputValue: group.Name,
        closeOnClickModal:false
      }).then(({ value }) => {
        updateGroup({ Id: group.Id, Name: value }).then((rsp) => {
          this.$message.success("操作成功");
          this.getGroups();
        });
      });
    },
    getTemplateData(data, group) {
      return data;
    },
    applicationRecord(item, group) {
      // this.$store.commit("setIsEdit", true);
      this.$router.push("/definition/applicationrecord?code=" + item.Id);
    },
    editFrom(item, group) {
      this.$store.commit("setIsEdit", true);
      this.$store.commit('tagsView/DEL_CACHED_VIEW',{path:"/definition/design"})
      this.$nextTick(()=>{
        this.$router.push("/definition/design?code=" + item.Id);
      });
      
    },
    copyItem(item){
      this.$confirm(
          "您确定要复制流程 " + item.Name + " 吗",
          "提示",
          {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
          }
        ).then(() => {
          this.copyloading=true
          getFormDetail(item.Id)
            .then((rsp) => {
              let form = rsp.data;
              delete form.Id
              this.saveCopyFrom(form)
            })
            .catch((err) => {
              this.copyloading=false
              this.$message.error(err);
            });
        });
    },
    saveCopyFrom(template){
      updateForm(template)
        .then((rsp) => {
          this.$message.success("复制流程成功");
          this.getGroups();
          this.copyloading=false
        })
        .catch((err) => {
          this.copyloading=false
          this.$message.error(err);
        });
    },
    stopFrom(item) {
      let tip =
        item.Status == 1
          ? " 启用后将会进入 “其他” 分组，是否继续？"
          : " 停用后将会被转移到 “已停用” 分组，您可以再次启用或者删除它，是否继续?";
      this.$confirm(item.Name + tip, "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      }).then(() => {
        updateForm({ Id: item.Id, Status: item.Status == 1?0:1 }).then((rsp) => {
          this.$message.success(rsp.data);
          this.getGroups();
        });
      });
    },
    moveFrom(item) {
      if (item.Status == 1) {
        this.$confirm(
          "您确定要删除流程 " + item.Name + " 吗，删除后无法恢复，是否继续？",
          "提示",
          {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
          }
        ).then(() => {
          delForm(item.Id).then((rsp) => {
            this.$message.success("删除成功");
            this.getGroups();
          });
        });
      } else {
        if (this.moveSelect === null || this.moveSelect === "") {
          this.$message.error("请选择分组");
          return;
        }
        updateForm({ Id: item.Id, GroupId: this.moveSelect }).then((rsp) => {
          this.$message.success("操作成功");
          this.getGroups();
          this.moveSelect = null;
        });
      }
    },
  },
};
</script>

<style lang="less" scoped>
body {
  background: #ffffff !important;
}

.from-panel {
  // padding: 10px 30px;
  width: 100%;
  // background: #ffffff;
  box-sizing: border-box;
  font-size: 14px;

  /deep/ .from-title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    // margin-top: -10px;
    // margin-bottom: 30px;
    width: 100%;
    padding: 0 20px;
    height: 62px;
    div {
      // float: right;

      .el-button {
        border-radius: 15px;
        color: #78829d;
        height: 32px;
        width: 100px;
        background-color: #ffffff;
      }
      .bleBg.el-button {
        background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
        color: #ffffff;
      }
      .return_button {
        margin-right: 6px;
        background: #ffffff;
        color: #78829d;
        width: 18px;
        height: 18px;
        line-height: 18px;
        font-size: 16px;
        padding: 0;
        text-align: center;
        border: 1px solid #78829d;
      }
    }
  }

  //height: 100vh;
}



.form-group {
  padding: 0 20px;
  //border: 1px solid #d3d3d3;
  // box-shadow: 0 0 10px rgba(0, 28, 112, 0.1);

  // &:hover {
  //   box-shadow: 1px 1px 12px 0 #b3b3b3;
  // }

  .form-group-title {
    border-top: solid 1px #efefef;
    
    height: 66px;
    line-height: 66px;
    // border-bottom: 1px solid #d3d3d3;
    display: flex;
    justify-content: space-between;
    align-items: center;
    // margin-bottom: 10px;

    .el-icon-rank {
      display: none;
      cursor: move;
    }

    &:hover {
      .el-icon-rank {
        display: inline-block;
      }
    }

    div {
      display: inline-block;
      // float: right;
    }

    span:first-child {
      margin-right: 5px;
      font-size: 15px;
      // font-weight: bold;
      color: rgba(51, 51, 51, 1);
    }

    span:nth-child(2) {
      color: #656565;
      font-size: 15px;
      margin-right: 10px;
    }

    /deep/ .el-button {
      // color: #404040;
      &:hover {
        color: #95b5ff;
      }
    }
    .group_button {
      color: rgba(53, 114, 255, 1);
      margin-left: 32px;
      border-radius: 16px;
      background-color: rgba(246, 249, 255, 1);
      width: 120px;
    }
  }

  .form-group-item:nth-child(1) {
    border-top: none !important;
  }

  .form-group-item {
    color: #3e3e3e;
    font-size: small;
    padding-left: 20px;
    padding-right: 30px;
    height: 58px;
    line-height: 58px;
    position: relative;
    border-bottom: 1px solid rgba(187, 192, 206, 0.1);
    &.choose {
      background: #fafafa !important;
    }
    .button_con_mini {
      .el-button {
        margin-left: 40px;
        color: rgba(53, 114, 255, 1);
      }
      // .el-button:first-child{
      //   margin-left: 0;
      // }
    }
    div {
      display: inline-block;
    }

    i {
      border-radius: 10px;
      padding: 7px;
      font-size: 20px;
      color: #ffffff;
      margin-right: 10px;
    }

    div:nth-child(1) {
      float: left;
    }

    div:nth-child(2) {
      position: absolute;
      // color: #7a7a7a;
      font-size: 12px;
      left: 200px;
      max-width: 300px;
      overflow: hidden;
    }

    div:nth-child(3) {
      position: absolute;
      right: 30%;
    }

    div:nth-child(4) {
      float: right;
    }
  }
  .form-group-item:last-child {
    border-bottom: none;
  }
}
.undrag {
  color: rgba(187, 192, 206, 1);
  // background: #ebecee !important;
  .form-group-title {
    span:first-child {
      // font-weight: bold;
      color: rgba(187, 192, 206, 1);
    }

    span:nth-child(2) {
      color: rgba(187, 192, 206, 1);
    }
  }
  .undrag {
    div {
      color: rgba(187, 192, 206, 1);
      div {
        color: rgba(187, 192, 206, 1);
      }
    }
  }
}
@media screen and (max-width: 1000px) {
  .desp {
    display: none !important;
  }
}

@media screen and (max-width: 800px) {
  .from-panel {
    padding: 50px 10px;
  }
}
</style>
