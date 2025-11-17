<template>
  <w-dialog
    :border="false"
    closeFree
    width="60%"
    @ok="selectOk"
    :title="title"
    v-model="visible"
    :selectDev="select.length"
  >
    <div class="device_select_con">
      <el-form ref="selectForm" :inline="true" :model="selectForm" label-width="80px">
        <el-form-item>
          <el-select
            v-model="filterType"
            placeholder="请选择"
            @change="changeFilterType"
            style="width:120px"
          >
            <el-option
              class="device_select_li"
              v-for="ite in filterTypeList"
              :key="ite.value"
              :label="ite.label"
              :value="ite.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-select v-model="filterConditions" placeholder="请选择" style="width:120px">
            <el-option
              class="device_select_li"
              v-for="item in conditionsList"
              :key="item.value"
              :label="item.label"
              :value="item.value"
              v-show="(item.value=='equal'&&filterType!='name')||filterType=='name'"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-input v-model="filterParams.Name" placeholder="请输入设备名称" v-if="filterType=='name'"></el-input>
          <el-input
            v-model="filterParams.Name"
            placeholder="请输入设备编码"
            v-if="filterType=='deviceCode'"
          ></el-input>
        </el-form-item>
        <div class="device_select_btn">
          <el-button icon="el-icon-search" type="primary" @click="getOrgList">搜索</el-button>
          <el-button icon="el-icon-refresh-right" @click="retLoad">重置</el-button>
        </div>
      </el-form>
      <div class="device_list">
        <div class="device_li_con" v-for="its in orgs" :key="its.Id">
          <div
            :class="{'device_li':true, 'disable_li':select.length>=limit && !its.selected && limit!=1}"
            @click="selectChange(its)"
          >
            <div class="device_lis_top">
              <img src="../../../assets/images/shebei.png" alt />
              <div class="lis_top_cot">
                <el-tooltip class="item" :content="its.Name" placement="top">
                  <div class="device_name">{{its.Name}}</div>
                </el-tooltip>
                <div class="device_group_name">
                  <div class="group_name_lft">
                    <div class="title">设备类型</div>
                    <el-tooltip
                      class="item"
                      :content="its.GroupName"
                      placement="top"
                    >
                      <div class="cont">{{its.GroupName}}</div>
                    </el-tooltip>
                  </div>
                  <div class="group_name_rht">
                    <div class="title">协议名称</div>
                    <el-tooltip
                      class="item"
                      :content="its.ProductName"
                      placement="top"
                    >
                      <div
                        class="cont"
                      >{{its.ProductName}}</div>
                    </el-tooltip>
                  </div>
                </div>
              </div>
            </div>
            <div id="div6" v-show="its.selected">
              <span class="check"></span>
            </div>
          </div>
        </div>
      </div>
      <pagination
        :total="total"
        :pageSizes="pageSizes"
        :page.sync="filterParams.pageNum"
        :limit.sync="filterParams.pageSize"
        @pagination="getOrgList"
      />
    </div>
  </w-dialog>
</template>

<script>
import Ellipsis from "./Ellipsis.vue";
import WDialog from "./WDialog.vue";
import { myDeviceList } from "@/api/after/dev";
export default {
  name: "OrgPicker",
  components: { Ellipsis, WDialog },
  props: {
    limit_product: {
      type: Array,
      default: () => {
        return [];
      }
    },
    title: {
      default: "请选择",
      type: String
    },
    limit: {
      //设备可选择的数量
      type: Number,
      default: 1
    }
  },
  data() {
    return {
      //设备过滤条件
      pageSizes: [6, 12, 18, 26],
      selectForm: {},
      filterType: "name", //过滤类型
      filterConditions: "contain", //过滤条件
      filterValue: "", //过滤的值
      filterTypeList: [
        { label: "设备名称", value: "name" },
        { label: "设备编码", value: "deviceCode" }
      ],
      conditionsList: [
        { label: "等于", value: "equal" },
        { label: "包含", value: "contain" }
      ],
      filterParams: {
        pageNum: 1,
        pageSize: 6,
        // showAll:true
        GroupId: null,
        productId: null,
        ProductList:[],//协议数组
        Name: ""
      },
      total: 0,
      //设备过滤条件
      visible: false,
      loading: false,
      checkAll: false,
      nowDeptId: null,
      nodes: [],
      select: [], //选中的设备
      search: "",
      deptStack: [],
      type: "device" //org选择部门/人员  user-选人  dept-选部门 role-选角色 device-选设备
    };
  },
  computed: {
    deptStackStr() {
      return String(this.deptStack.map(v => v.name)).replaceAll(",", " > ");
    },
    orgs() {
      return this.nodes;
    },
  },
  mounted(){
    // console.log(this.limit_product,'设备选择组件');
    
  },
  methods: {
    retLoad() {
      //重置搜索
      this.filterParams = {
        pageNum: 1,
        pageSize: 6,
        // showAll:true
        GroupId: null,
        productId: null,
        ProductList:this.limit_product,//协议数组
        Name: ""
      };
      this.getOrgList();
    },
    changeFilterType(val) {
      //根据选中的筛选条件选择
      if (val == "name") {
        this.filterConditions = "contain";
      } else {
        this.filterConditions = "equal";
      }
    },
    show(selected, t) {
      this.visible = true;
      if (t != null) {
        this.type = t;
      }
      this.filterParams.ProductList=this.limit_product//给过滤条件赋值
      this.init(selected);
      this.getOrgList();
    },
    orgItemClass(device) {
      return {
        "device-item": true
      };
    },
    disableDept(node) {
      let isNotSelect = true;
      this.select.forEach(nd => {
        if (nd.Id == node.Id) {
          isNotSelect = false;
        }
      });
      return node.selected && isNotSelect;
    },
    getOrgList() {
      this.loading = true;
      console.log("设备列表查询参数",this.filterParams);
      
      myDeviceList(this.filterParams)
        .then(rsp => {
          console.log("选择设备控件", rsp);
          this.loading = false;
          this.nodes = rsp.data.List;
          this.total = rsp.data.Total;
          this.selectToLeft();
        })
        .catch(err => {
          this.loading = false;
          this.$message.error("接口异常");
        });
    },
    getShortName(name) {
      if (name) {
        return name.length > 2 ? name.substring(1, 3) : name;
      }
      return "**";
    },
    selectToLeft() {
      let nodes = this.nodes;
      nodes.forEach(node => {
        if (this.select.length > 0) {
          for (let i = 0; i < this.select.length; i++) {
            if (this.select[i].id === node.Id) {
              node.selected = true;
              break;
            } else {
              node.selected = false;
            }
          }
        } else {
          node.selected = false;
        }
      });
    },
    selectChange(node) {
      // console.log(node);
      let choiceNode = {
        id: node.Id,
        name: node.Name,
        photoUrl: node.PhotoUrl,
        selected: node.selected,
        deviceNumber:node.DeviceNumber
      };
      choiceNode.type = this.type;
      if (choiceNode.selected) {
        this.checkAll = false;
        for (let i = 0; i < this.select.length; i++) {
          if (this.select[i].id === choiceNode.id) {
            this.select.splice(i, 1);
            break;
          }
        }
        choiceNode.selected = false;
        this.$forceUpdate();
      } else {
        let nodes = this.nodes;

        if (this.limit == 1 && !(this.limit > this.select.length)) {
          this.select = [choiceNode];
          choiceNode.selected = true;
          console.log(this.select);
          this.nodes.forEach(nd => {
            if (choiceNode.id !== nd.Id) {
              nd.selected = false;
              this.$forceUpdate();
            }
          });
        } else if (!(this.limit > this.select.length)) {
          choiceNode.selected = false;
          this.$forceUpdate();
          // this.select = [node];
        } else {
          choiceNode.selected = true;
          this.select.push(choiceNode);
          this.$forceUpdate();
        }
      }
      this.selectToLeft();
      console.log("选中的", this.select, this.orgs);
    },
    recover() {
      this.select = [];
      this.nodes.forEach(nd => (nd.selected = false));
    },
    selectOk() {
      this.$emit("ok", Object.assign([], this.select));
      this.visible = false;
      this.recover();
    },
    clearSelected() {
      this.$confirm("您确定要清空已选中的项?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        this.recover();
      });
    },
    close() {
      this.$emit("close");
      this.recover();
    },
    init(selected) {
      console.log(selected);

      this.checkAll = false;
      this.nowDeptId = null;
      this.deptStack = [];
      this.nodes = [];
      this.select = Object.assign([], selected);
      this.selectToLeft();
    }
  }
};
</script>

<style lang="scss" scoped>
.device_select_li {
  position: relative !important;
  height: 34px !important;
  border: none !important;
  width: 120px !important;
}
.device_select_btn {
  float: right;
}
.device_select_con::after {
  clear: both;
}
.device_list {
  margin-right: -1.5%;
  margin-top: 15px;
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;
  .device_li_con {
    width: 33%;
    padding-right: 1.5%;
    box-sizing: border-box;
    .device_li {
      cursor: pointer;
      box-shadow: 0 0 6px 6px rgba(244, 245, 249, 1);
      margin: 0 0 25px 0;
      border-radius: 5px;
      padding: 20px 10px;
      position: relative;
      box-sizing: border-box;
      .device_lis_top {
        width: 100%;
        display: flex;
        justify-content: flex-start;
        img {
          width: 80px;
          height: 80px;
          margin-right: 20px;
        }
        .lis_top_cot {
          width: calc(100% - 100px);
          .device_name {
            margin-bottom: 5px;
            color: #000000;
            font-weight: 600;
            font-size: 16px;
            width: 100%;
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
          }
          .device_group_name {
            color: #887e7b;
            display: flex;
            justify-content: flex-start;
            align-items: flex-start;
            width: 100%;
            .group_name_lft {
              width: 49%;
              margin-right: 2%;

              .title {
                font-size: 12px;
              }
              .cont {
                margin-top: 5px;
                font-size: 16px;
                color: #1890ff;
                width: 100%;
                white-space: nowrap;
                text-overflow: ellipsis;
                overflow: hidden;
                // display: -webkit-box;
                // -webkit-line-clamp: 2;
                // -webkit-box-orient: vertical;
              }
            }
            .group_name_rht {
              width: 49%;
              .title {
                font-size: 12px;
              }
              .cont {
                margin-top: 5px;
                font-size: 16px;
                color: #1890ff;
                width: 100%;
                white-space: nowrap;
                text-overflow: ellipsis;
                overflow: hidden;
                // display: -webkit-box;
                // -webkit-line-clamp: 2;
                // -webkit-box-orient: vertical;
              }
            }
          }
        }
      }
      /* 三角形 */

      #div6 {
        position: absolute;
        right: 0;
        bottom: 0;
        width: 0;
        height: 0;
        // border: 11px solid #cccccc;
        // border-left: 11px solid transparent;
        // border-top: 11px solid transparent;
        border: 11px solid #3888ff;
        border-left: 11px solid transparent;
        border-top: 11px solid transparent;
      }

      /* 对号 */

      .check {
        position: relative;
        display: inline-block;
        width: 25px;
        height: 25px;
        border-radius: 25px;
      }

      .check::after {
        content: "";
        position: absolute;
        left: -2px;
        top: 0px;
        width: 40%;
        height: 22%;
        border: 2px solid #fff;
        border-radius: 1px;
        border-top: none;
        border-right: none;
        background: transparent;
        transform: rotate(-45deg);
      }
      .disable_li {
        opacity: 0.6;
      }
    }
  }
}
</style>
