<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div
      class="elbiaoge_elform"
      :style="{ 'min-height': 'calc(100vh - 136px)' }"
    >
      <div class="room_con">
        <div class="room_left_con">
          <catetory_compt @clickNode="clickNode" @setcatetoryTableData="setcatetoryTableData"></catetory_compt>
        </div>
        <div style="padding: 20px; width: 79%">
          <el-row :gutter="10" class="mb8 button_row">
            <div>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openAddRoom">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  <span style="margin-left: 6px">添加车间</span>
                </el-button>
              </el-col>
            </div>
            <right-toolbar :isShowSearch="false" @queryTable="refreshData" :columns="columns"></right-toolbar>
          </el-row>
          <el-table ref="agentTable" v-loading="roomLoading" :data="roomTableData" tooltip-effect="dark" style="width: 100%" class="data_table">
            <el-table-column label="车间名称" align="center" key="Name" prop="Name" v-if="columns[0].visible"/>
            <el-table-column label="所属客户" align="center" key="CategoryId" prop="CategoryId" :show-overflow-tooltip="true" v-if="columns[1].visible">
              <template>
                <div>{{activeCategoryInfo.activeOrg?activeCategoryInfo.activeOrg:''}}</div>
              </template>
            </el-table-column>
            <el-table-column label="所属分类" align="center" key="CategoryName" prop="CategoryName" :show-overflow-tooltip="true"  v-if="columns[2].visible"></el-table-column>
            <el-table-column label="负责人" align="center" key="LeaderId" prop="LeaderId" :show-overflow-tooltip="true" v-if="columns[3].visible">
            <template slot-scope="scope">
              <div>{{scope.row.LeaderInfo.RealName}}</div>
            </template>
            </el-table-column>
            <el-table-column label="操作" align="center" class-name="small-padding" width="300">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-edit" @click="handleAllocation(scope.row)" >分配</el-button>
                <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" >修改</el-button>
                <el-button type="text" icon="el-icon-delete" @click="deleteRoomRowData(scope.row)" >删除</el-button>
                <el-dropdown v-if="scope.row.ReportToken!=='' && parseReportToken(scope.row.ReportToken)[0].url !== ''" style="margin-left: 10px" @command="lookScrent($event)">
                  <el-button type="text" icon="el-icon-s-platform" style="color: #e6a23c">
                    看板<i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item v-for="(item, index) in parseReportToken(scope.row.ReportToken)" :key="index" :command="item.url">{{ item.name }}</el-dropdown-item>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>
    </div>

    <el-dialog :title="title" top="2vh" :visible.sync="roomOpen" center width="900px" :close-on-click-modal="false">
      <el-form :model="roomForm" ref="roomForm" :rules="roomRules" label-position="left" class="groupFrom" :inline="true" label-width="80px">
        <el-form-item label="车间名称" prop="name">
          <el-input type="text" v-model="roomForm.name" placeholder="请输入车间名称" :disabled="isReadonly"></el-input>
        </el-form-item>
        <el-form-item label="所属分类" prop="categoryId">
          <treeselect class="groupSet" v-model="roomForm.categoryId" :options="catetoryTableData" :show-count="true" :normalizer="normalizer" placeholder="请选择车间分类"/>
        </el-form-item>
        <el-form-item label="所属客户" prop="targetOrgId">
          <el-select v-model="roomForm.targetOrgId" placeholder="请选择" :disabled="true">
            <el-option v-for="item in targetOrgIdOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="负责人" prop="leaderId">
          <!-- <el-input type="number" v-model.number="roomForm.leaderId" placeholder="请选择车间负责人" :disabled="isReadonly"></el-input> -->
          <el-select v-model="roomForm.leaderId" placeholder="请选择" :disabled="true" v-if="roomForm.customerId">
            <el-option v-for="item in leaderIdOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
          <el-select v-else class="form_input_style" v-model="roomForm.leaderName" ref="selectUsers2" placeholder="请选择负责人" @focus="getUsersFocus2" style="width: 100%"></el-select>
          <org-picker :multiple="false" ref="userPicker2" :selected="leaderUserInfo" @ok="selectUsersed2"/>
        </el-form-item>
        <el-form-item label="协作人" prop="helperName">
          <el-select class="form_input_style" multiple v-model="roomForm.helperName" ref="selectUsers" placeholder="请选择协作人" @focus="getUsersFocus" style="width: 100%"></el-select>
          <org-picker :multiple="true" ref="userPicker" :selected="helperUserInfo" @ok="selectUsersed"/>
        </el-form-item>
        <el-form-item label="排序序号" prop="Sort">
          <el-input type="number" v-model.number="roomForm.Sort" placeholder="请输入车间序号" :disabled="isReadonly"></el-input>
        </el-form-item>
        <el-form-item label="关联客户" prop="autoAdd">
          <el-switch v-model="roomForm.autoAdd" active-color="#13ce66" inactive-color="#DCDFE6"></el-switch>
          <span style="margin-left: 10px; color: #999; font-size: 12px">是否自动同步关联客户的设备到车间</span>
        </el-form-item>
        <el-form-item label="监控报表">
          <el-button type="primary" @click="addBoardData">添加</el-button>
          <el-table style="width: 100%; margin-top: 10px" ref="devTable" :data="boardData" tooltip-effect="dark" border>
            <el-table-column label="看板名称" align="center" width="200">
              <template slot-scope="scope">
                <el-input type="text" v-model="scope.row.name" placeholder="请输入看板名称"/>
              </template>
            </el-table-column>
            <el-table-column label="看板地址" align="center">
              <template slot-scope="scope">
                <el-input type="text" v-model="scope.row.url" placeholder="请输入看板地址"/>
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" width="100">
              <template slot-scope="scope">
                <el-button type="text" icon="el-icon-delete" @click="delBoardData(scope.$index)" >删除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <!-- <el-input type="text" v-model="roomForm.ReportToken" placeholder="请输入自定义的房间监控报表"/> -->
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input type="textarea" v-model="roomForm.remark" placeholder="请输入备注"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer" style="text-align: end">
        <el-button @click="roomOpen = false">取 消</el-button>
        <el-button type="primary" @click="addDeviceRoom">确 定</el-button>
      </div>
    </el-dialog>
    <room_device ref="devAllocation"></room_device>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  roomCatetoryInfo,
  deviceRoomList,
  deviceRoomInfo,
  removeDeviceRoom,
  addDeviceRoom,
  editDeviceRoom
} from "@/api/after/room";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import catetory_compt from "./catetory_compt";
import room_device from "./room_device";
import { customerInfo } from "@/api/crm/customer";
import OrgPicker from "@/views/flowable/common/OrgPicker";
export default {
  name: "deviceRoom",
  mixins: [resizeTableCon],
  components: { Treeselect, catetory_compt,room_device,OrgPicker },
  data() {
    return {
      loading: false,
      isReadonly: false, //是否仅仅只读
      activeCategoryInfo: {}, //分类信息
      catetoryTableData: [],
      // 重新渲染表格状态
      configLoading: true, //配置信息是否处于
      // 列信息
      columns: [
        { key: 0, label: `车间名称`, visible: true },
        { key: 1, label: `所属分类`, visible: true },
        { key: 2, label: `所属客户`, visible: true },
        // { key: 3, label: `分类路径`, visible: true },
        { key: 3, label: `负责人`, visible: true },
      ],
      //车间相关参数
      roomOpen: false,
      roomLoading: false, //车间列表是否在加载
      roomTableData: [], //车间表格数据
      roomForm: {
        name: "",
        targetOrgId: "",
        leaderId: "",
        categoryId: "",
        Sort: 0,
        remark: "",
        autoAdd: false,
        ReportToken:''
      },
      targetOrgIdOptions: [], //所属客户下拉列表
      leaderIdOptions: [], //负责人下拉列表
      roomRules: {
        name: [{ required: true, trigger: "blur", message: "请输入车间名称" }],
        targetOrgId: [
          { required: true, trigger: "blur", message: "请选择车间所属客户" },
        ],
        leaderId: [
          { required: true, trigger: "blur", message: "请选择车间负责人" },
        ],
      },
      helperUserInfo:[],//协作人列表
      leaderUserInfo:[],
      title: '添加车间',
      boardData: [{name: '', url: ''}], // 监控报表数据
    };
  },
  watch: {
    "catetoryFrom.parentIdData"() {
      this.$nextTick(() => {
        if (this.$refs["catetoryFrom"]) {
          this.$refs["catetoryFrom"].validateField("parentIdData", (valid) => {
            if (!valid) {
              console.log(!valid);
            } else {
              console.log("error submit!!");
              return false;
            }
          });
        }
      });
    },
  },
  async mounted() {
    this.configLoading = true;

    this.configLoading = false;
  },
  methods: {
    getUsersFocus2() {
      //获取用户下拉列表的焦点
      this.$refs.selectUsers2.blur();
      
      let arr=[]
      if(this.roomForm.leaderId){
        let leaderinfo ={
          id: this.roomForm.leaderId,
          name: this.roomForm.leaderName,
          avatar: this.roomForm.leaderAvatar,
          type: "user",
        };
        arr.push(leaderinfo)
      }
      this.$refs.userPicker2.show(arr, "user");
    },
    getUsersFocus() {
      //获取用户下拉列表的焦点
      this.$refs.selectUsers.blur();
      let helperList =[]
      if(this.roomForm.helper){
        helperList = this.roomForm.helper.split(",");
      }
      
      if (helperList && helperList.length > 0) {
        let arr = [];
        helperList.map((row, index) => {
          if (row > 0) {
            let obj = {
              id: parseInt(row),
              name: this.roomForm.helperName[index],
              avatar: this.roomForm.helperAvatar[index],
              type: "user",
            };
            arr.push(obj);
          }
        });
        this.helperUserInfo = JSON.parse(JSON.stringify(arr));
      } else {
        this.helperUserInfo = [];
      }
      this.$refs.userPicker.show(this.helperUserInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      this.helperUserInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
        });
        this.roomForm.helper = li.join(",");
        this.roomForm.helperName = li2;
        this.roomForm.helperAvatar = li3;
      } else {
        this.roomForm.helper = undefined;
        this.roomForm.helperName = undefined;
        this.roomForm.helperAvatar = undefined;
      }
       //清除el-select多选时的清除按钮
       this.$nextTick(() => {
        const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
        const arr = Array.from(closeIcons);
        arr.forEach((item) => {
          item.style.display = "none";
        });
      });
      this.$forceUpdate();
    },
    selectUsersed2(values) {
      //选择责任人
      this.leaderUserInfo=values
      if(values&&values.length>0){
        this.roomForm.leaderId = values[0].id;
        this.roomForm.leaderName = values[0].name;
        this.roomForm.leaderAvatar = values[0].avatar;
      }
      
       //清除el-select多选时的清除按钮
       this.$nextTick(() => {
          const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
          const arr = Array.from(closeIcons);
          arr.forEach((item) => {
            item.style.display = "none";
          });
        });
      this.$forceUpdate();
    },
    handleAllocation(row){
      //分配设备
      this.$refs.devAllocation.openDialog(row)
    },
    setcatetoryTableData(table) {
      this.catetoryTableData = JSON.parse(JSON.stringify(table));
    },
    // 修改车间弹窗
    handleUpdate(row) {
      this.title = '修改车间';
      //修改车间
      deviceRoomInfo({ id: row.Id }).then((res) => {
        console.log('车间详情',res);
        let data = res.data;
        this.roomForm = {
          id: data.Id,
          name: data.Name,
          targetOrgId: data.TargetOrgId,
          leaderId: data.LeaderId,
          categoryId: data.CategoryId?data.CategoryId:undefined,
          customerId: data.CustomerId,
          Sort: data.Sort,
          remark: data.Remark,
          autoAdd: data.AutoAdd,
          helper:data.Helper,
          helperName:[],
          helperAvatar:[],
          ReportToken:data.ReportToken,
        };
        this.boardData = data.ReportToken === '' ? [{name: '', url: ''}] : JSON.parse(data.ReportToken); // 监控报表数据
        if (data.TargetOrgId) {
          this.targetOrgIdOptions = [
            {
              label: data.TargetName,
              value: data.TargetOrgId,
            },
          ]; //所属客户下拉列表
          
        }
        if(data.customerId){
          this.leaderIdOptions = [
            {
              label: data.LeaderInfo.RealName,
              value: data.LeaderId,
            },
          ]; //负责人下拉列表
        }else{
          this.roomForm.leaderId = data.LeaderInfo.Id;
          this.roomForm.leaderName = data.LeaderInfo.RealName;
          this.roomForm.leaderAvatar = data.LeaderInfo.Avatar;
        }
        if(data.HelperUsers){
          this.helperUserInfo=JSON.parse(JSON.stringify(data.HelperUsers))
          this.roomForm.helperName= this.helperUserInfo.map(row=>row.RealName)
          this.roomForm.helperAvatar= this.helperUserInfo.map(row=>row.Avatar)
        }
        this.$nextTick(() => {
          const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
          const arr = Array.from(closeIcons);
          arr.forEach((item) => {
            item.style.display = "none";
          });
        });
        this.roomOpen = true;
      });
    },

    async loadDeviceRoomList() {
      //加载车间列表
        try {
          let response = await deviceRoomList({
            TargetOrgId: this.activeCategoryInfo.TargetOrgId,
            CategoryId: this.activeCategoryInfo.Id,
          });
          // console.log("车间列表", response);
          this.roomTableData = response.data;
          this.$forceUpdate()
        } catch (error) {}
    },

    clickNode(data,activeOrg) {
      //点击分类节点
      this.activeCategoryInfo = JSON.parse(JSON.stringify(data));
      this.activeCategoryInfo.activeOrg=activeOrg
      this.loadDeviceRoomList();
    },

    async openAddRoom() {
      this.resetForm("roomForm");
      this.title = '添加车间';
      this.boardData = [{name: '', url: ''}];
      this.roomForm = {
          name: "",
          targetOrgId: this.activeCategoryInfo.TargetOrgId,
          leaderId: "",
          categoryId: undefined,
          customerId: this.activeCategoryInfo.CustomerId?this.activeCategoryInfo.CustomerId:'',
          Sort: 0,
          remark: "",
          autoAdd: false,
        };
        if (this.activeCategoryInfo && this.activeCategoryInfo.CustomerId) {
          let customIddetail = await this.viewInfo(
            this.activeCategoryInfo.CustomerId
          );
          this.targetOrgIdOptions = [
            {
              label: customIddetail.CustomerName,
              value: this.activeCategoryInfo.TargetOrgId,
            },
          ]; //所属客户下拉列表
          this.leaderIdOptions = [
            {
              label: customIddetail.LeaderName,
              value: customIddetail.LeaderId,
            },
          ]; //负责人下拉列表
          this.roomForm.leaderId = customIddetail.LeaderId;
        } else {
          if(this.activeCategoryInfo.Id){
            this.roomForm.categoryId=this.activeCategoryInfo.Id
            let res1 = await roomCatetoryInfo({ id: this.activeCategoryInfo.Id });
            let categoryDetail = res1.data;
            this.targetOrgIdOptions = [
              {
                label: categoryDetail.TargetName,
                value: categoryDetail.TargetOrgId,
              },
            ];
          }else{
            this.targetOrgIdOptions = [
              {
                label: this.activeCategoryInfo.activeOrg,
                value: this.activeCategoryInfo.TargetOrgId,
              },
            ];
          }
        }

        this.roomOpen = true;
      // } else {
      //   this.$message.error("请先选择车间分类");
      // }
    },
    async viewInfo(val) {
      //客户详情
      if (val) {
        try {
          let res = await customerInfo({ id: val });
          return res.data;
        } catch (error) {
          return {};
        }
      }
    },
    addDeviceRoom() {
      this.$refs["roomForm"].validate((valid) => {
        let submitform=JSON.parse(JSON.stringify(this.roomForm));
        submitform.ReportToken = JSON.stringify(this.boardData);
        if(submitform.categoryId==undefined){
          submitform.categoryId=''
        }
        if(submitform.helperName){
          submitform.helperName = submitform.helperName.join(",");
        }
        if(!submitform.helper){
          submitform.helper=''
        }
        delete submitform.helperAvatar;
        if (valid) {
          if (this.roomForm.id) {
            editDeviceRoom(submitform)
              .then((response) => {
                this.$message.success("修改车间成功");
                if (response.code == 0) {
                  this.loadDeviceRoomList();
                }
                this.roomOpen = false;
              })
              .catch((err) => {
                console.log("错误打印", err);

                this.$message.error(err.message);
              });
          } else {
            addDeviceRoom(submitform)
              .then((response) => {
                this.$message.success("创建车间成功");
                if (response.code == 0) {
                  this.loadDeviceRoomList();
                }
                this.roomOpen = false;
              })
              .catch((err) => {
                console.log("错误打印", err);

                this.$message.error(err.message);
              });
          }
        }
      });
    },
    refreshData() {
      this.loadDeviceRoomList();
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

    deleteRoomRowData(data) {
      //删除表格中的一行的数据
      this.$modal
        .confirm('是否确认删除名为"' + data.Name + '"的车间？')
        .then(function () {
          return removeDeviceRoom({ id: data.Id });
        })
        .then(() => {
          this.loadDeviceRoomList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    // 添加监控报表
    addBoardData() {
      this.boardData.push({ name: '', url: '' });
    },
    // 删除监控报表
    delBoardData(index) {
      this.boardData.splice(index, 1);
    },
    // 解析reportToken
    parseReportToken(reportToken) {
      const dashboards = JSON.parse(reportToken)
      return dashboards
    },
    // 跳转四色灯大屏
    lookScrent(link) {
      console.log(link)
      window.open(`${link}`, '_blank');
    }
  },
};
</script>
<style lang="less">
.groupFrom {
  .el-form-item {
    width: 100%;

    .el-form-item__content {
      width: calc(100% - 80px);

      .el-select {
        width: 100%;
      }

      .el-input {
        width: 100%;
      }
    }
  }
}

.groupSet {
  &.vue-treeselect--disabled .vue-treeselect__input-container{
    display: none;
  }
  .vue-treeselect__input-container {
    display: flex;
    align-items: center;
  }
}
</style>
<style lang="less" scoped>
.room_con {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;
}
.room_left_con {
  width: 20%;
  box-shadow: 0px 0px 2px 0px rgba(0, 0, 0, 0.25);
  border-radius: 5px;
}
</style>
