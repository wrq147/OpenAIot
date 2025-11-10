<template>
  <div>
    <div class="custom_org_info">
      <div class="info_text">{{ activeOrg }}</div>
      <div class="handle_right">
        <div class="handle_my" @click="switchMyOrgers" v-if="activeOrgId != myOrgId">我的</div>
        <div class="info_handle" @click="switchOrgers">切换</div>
      </div>
    </div>
    <el-row :gutter="20" class="mb8 button_row" style="margin-bottom: 5px; margin-left: 0;overflow: hidden;">
        <el-col :span="1.5">
          <el-button
            style="background: none !important;background-color: none !important;height: 20px;padding: 0;" type="primary" plain @click="openAddCatetory()">
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng" style="font-size: 14px"></i><span style="margin-left: 6px">添加一级分类</span></el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            style="background: none !important;background-color: none !important;height: 20px;padding: 0;min-width: none;" type="info" plain @click="clearNodes()">
            <i class="zhongtaiiconfont zhongtai-icon-qingkong" style="font-size: 14px"></i><span style="margin-left: 6px">清除</span></el-button>
        </el-col>
    </el-row>
    <div class="category_tree_con">
      <el-tree ref="categoryTree" v-loading="configLoading" :data="catetoryTableData" node-key="Id" default-expand-all
        :highlight-current="true" :expand-on-click-node="true" :check-on-click-node="true" :props="{ label: 'Name', children: 'Children' }" @node-click="clickNode">
        <span class="custom-tree-node" slot-scope="{ node, data }">
          <span>{{ node.label }}</span>
          <span>
            <el-button type="text" size="mini" @click="() => openAddCatetory(data)" v-if="node.level<3">添加</el-button>
            <el-button type="text" size="mini" @click="() => editRowData(data)">编辑</el-button>
            <el-button type="text" size="mini" @click="() => deleteRowData(node, data)">删除</el-button>
          </span>
        </span>
      </el-tree>
    </div>
    <el-dialog title="添加分类" :visible.sync="catetoryOpen" center width="600px" :close-on-click-modal="false">
      <el-form :model="catetoryFrom" ref="catetoryFrom" :rules="catetoryRules" label-position="left" class="groupFrom" :inline="true" label-width="80px">
        <el-form-item label="父级分类" prop="parentIdData">
          <treeselect class="groupSet" v-model="catetoryFrom.parentIdData" :options="roomCatetoryTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择父级分类"
            :disabled="returnParentDis()"/>
        </el-form-item>
        <el-form-item label="分类名称" prop="name">
          <el-input type="text" v-model="catetoryFrom.name" placeholder="请输入分类名称" :disabled="isReadonly"></el-input>
        </el-form-item>
        <el-form-item label="分类序号" prop="sort">
          <el-input type="number" v-model.number="catetoryFrom.sort" placeholder="请输入分类序号" :disabled="isReadonly"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="catetoryOpen = false">取 消</el-button>
        <el-button type="primary" @click="addDeviceCatetory">确 定</el-button>
      </div>
    </el-dialog>
    <KfSelecter ref="kfDlg" @ok="onTargetChange" :isFilterInvite="true" :IsInvite="true" title="请选择切换的企业"></KfSelecter>
  </div>
</template>

<script>
import {
  addRoomCatetory,
  roomCatetoryTree,
  removeRoomCatetory,
  editRoomCatetory
} from "@/api/after/room";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import KfSelecter from "@/components/KFSelecter/kfselecter";
import { orgInfo } from "@/api/system/company";
export default {
  name: "roomCatetoryCompt",
  components: { KfSelecter,Treeselect },
  data() {
    return {
        configLoading: true, //配置信息是否处于
      isReadonly: false, //是否仅仅只读
      roomCatetoryTreeList: [],
      catetoryOpen: false, //添加分类弹窗
      catetoryFrom: {
        parentIdData: "",
        id: "",
        name: "",
        sort: "",
        targetOrgId: "",
      },
      catetoryRules: {
        parentIdData: [
          { required: true, trigger: "change", message: "请选择父级分类" },
        ],
        name: [{ required: true, trigger: "blur", message: "请输入分类名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入分类序号" }],
      },
      catetoryOpen: false, //分类添加弹窗
      activeOrg: "",
      activeOrgId: "",
      activeCustomerId: "",
      myOrginfo: "", //当前登录用户的企业信息
      myOrgId: this.$store.getters.orgId,
      catetoryTableData: [],//分类列表总数居
    };
  },

  async mounted() {
    this.activeOrgId = this.$store.getters.orgId;
    let res = await orgInfo({ id: this.activeOrgId });

    this.myOrginfo = res.data;
    this.activeOrg = this.myOrginfo.OrgName;
    this.getCatetoryList();
  },

  methods: {
    clearNodes(){
      this.$refs.categoryTree.setCurrentKey(null);
      this.$emit("clickNode", {TargetOrgId:this.activeOrgId,CustomerId:this.activeCustomerId},this.activeOrg);
      this.$forceUpdate()
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.Name,
        children: node.Children,
      };
    },
    returnParentDis() {
      if (this.catetoryFrom.Id) {
        return true;
      } else {
        return false;
      }
    },
    setCurrentKey() {
      if (this.catetoryTableData && this.catetoryTableData.length > 0) {
        this.$nextTick(() => {
          this.$refs.categoryTree.setCurrentNode(this.catetoryTableData[0]);
          this.$emit("clickNode", this.catetoryTableData[0],this.activeOrg);
        });
      }
    },
    clickNode(data) {
      //点击分类节点
      // console.log("分类节点点击", data);
      this.$emit("clickNode", data,this.activeOrg);
    },
    switchOrgers() {
      //切换客户
      this.$refs.kfDlg.openAgentDialog();
    },
    switchMyOrgers() {
      //切换成我的
      this.activeOrgId = this.$store.getters.orgId;
      this.activeOrg = this.myOrginfo.OrgName;
      this.activeCustomerId = "";
      this.getCatetoryList();
    },
    onTargetChange(val, iscustom) {
      this.activeCustomerId = "";
      this.activeOrgId = val.OrgId;
      this.activeOrg = val.OrgName;
      if (iscustom) {
        this.activeCustomerId = val.Id;
      } else {
        this.activeCustomerId = "";
      }

      // console.log(val, "val");
      this.getCatetoryList();
    },
    addDeviceCatetory() {
      //添加设备分类
      this.$refs["catetoryFrom"].validate((valid) => {
        if (valid) {
          let catetoryFrom = JSON.parse(JSON.stringify(this.catetoryFrom));
          if (catetoryFrom.parentIdData == -1) {
            catetoryFrom.parentId = "";
          } else {
            catetoryFrom.parentId = catetoryFrom.parentIdData;
          }
          delete catetoryFrom.parentIdData;
          if (catetoryFrom.id) {
            editRoomCatetory(catetoryFrom)
              .then((response) => {
                this.$message.success("修改分类成功");
                if (response.code == 0) {
                  this.getCatetoryList();
                }
                this.catetoryOpen = false;
              })
              .catch((err) => {
                console.log("错误打印", err);

                this.$message.error(err.message);
              });
          } else {
            addRoomCatetory(catetoryFrom)
              .then((response) => {
                this.$message.success("创建分类成功");
                if (response.code == 0) {
                  this.getCatetoryList();
                }
                this.catetoryOpen = false;
              })
              .catch((err) => {
                console.log("错误打印", err);

                this.$message.error(err.message);
              });
          }
        }
      });
    },
    openAddCatetory(val) {
      //打开添加分类
      this.resetForm("catetoryFrom");
      this.catetoryFrom = {
        parentIdData: null,
        id: "",
        name: "",
        sort: "",
        targetOrgId: this.activeOrgId,
        customerId: this.activeCustomerId,
      };
      if (
        !this.roomCatetoryTreeList[0] ||
        this.roomCatetoryTreeList[0].Id != "-1"
      ) {
        this.roomCatetoryTreeList.unshift({
          Name: "作为一级分类",
          Id: "-1",
          ParentId: 0,
        });
      }
      if (val) {
        this.catetoryFrom.parentIdData = val.Id == "" ? "-1" : val.Id;
      } else {
        this.catetoryFrom.parentIdData = "-1";
      }
      this.catetoryOpen = true;
    },
    async getCatetoryList() {
      //获取设备分类列表
      this.configLoading = true;
      let res = await roomCatetoryTree({ orgid: this.activeOrgId });
      try {
        if (res.code == 0) {
          this.catetoryTableData = res.data;
          let lists = [];
          lists = JSON.parse(JSON.stringify(res.data));
          this.roomCatetoryTreeList = this.setSelDis(lists,1); //选择分类时分类树
          this.configLoading = false;
          this.$emit("clickNode", {TargetOrgId:this.activeOrgId,CustomerId:this.activeCustomerId},this.activeOrg);
          this.$emit('setcatetoryTableData',this.catetoryTableData)
        }
      } catch (error) {}
    },
    setSelDis(list,level){
      for(let i=0;i<list.length;i++){
        if(level<3){
          list[i].isDisabled =false
        }else{
          list[i].isDisabled =true
        }
        if(list[i].Children&&list[i].Children.length>0){
          list[i].Children=this.setSelDis(list[i].Children,level+1)
        }
      }
      return list
    },
    async editRowData(row, isView) {
      //修改一行的数据
      // console.log("点击的那一行的数据", row);
      if (isView) {
        this.isReadonly = true;
      }
      await this.openAddCatetory();
      this.catetoryFrom = {
        parentIdData: row.ParentId == "" ? -1 : row.ParentId,
        id: row.Id,
        name: row.Name,
        sort: row.Sort,
        targetOrgId: row.TargetOrgId,
      };
    },
    deleteRowData(node, data) {
      //删除表格中的一行的数据
      this.$modal
        .confirm('是否确认移除名为"' + data.Name + '"的车间分类？')
        .then(function () {
          return removeRoomCatetory({ id: data.Id });
        })
        .then(() => {
          this.getCatetoryList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch(() => {});
    },
  },
};
</script>
<style lang="scss" scoped>
.custom_org_info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  margin-bottom: 5px;
  border-bottom: 1px solid #f6f6f6;
  .handle_right {
    display: flex;
    justify-content: flex-end;
    align-items: center;
    .handle_my {
      font-size: 16px;
      margin-right: 15px;
      color: #2878ff;
      cursor: pointer;
      text-decoration: underline;
    }
  }
  .info_handle {
    width: 60px;
    height: 30px;
    background: #e6f7f6;
    border: 1px solid #01ada8;
    color: #01ada8;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 3px;
    cursor: pointer;
  }
}
::v-deep .el-tree .custom-tree-node {
  //分类树的样式
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 14px;
  padding-right: 8px;
  height: 45px;
}
::v-deep .el-tree-node:not(.is-current)>.el-tree-node__content:hover{
      background: #ffffff;
    }
::v-deep .el-tree-node:not(.is-current)>.el-tree-node__content{
    background: #ffffff;
  }
::v-deep .el-tree-node__content {
  height: 100%;
}
</style>
