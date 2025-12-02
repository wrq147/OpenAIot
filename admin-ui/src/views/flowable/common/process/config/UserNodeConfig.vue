<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="⚙ 选择办理对象" prop="text" class="user-type">
        <el-radio-group v-model="nodeProps.assignedType" @change="radioChange">
          <el-radio v-for="t in approvalTypes" :label="t.type" :key="t.type">{{
            t.name
          }}</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.assignedType === 'ASSIGN_USER'">
          <el-button size="mini" icon="el-icon-plus" type="primary" @click="selectUser" round>选择人员</el-button>
          <org-items v-model="nodeProps.assignedUser" />
        </div>
        <div v-else-if="nodeProps.assignedType === 'ROLE'">
          <el-button size="mini" icon="el-icon-plus" type="primary" @click="selectRole" round>选择角色</el-button>
          <org-items v-model="nodeProps.role" />
        </div>
        <div v-else-if="nodeProps.assignedType === 'FORM_USER'">
          <el-form-item label="选择表单联系人项" prop="text" class="approve-end">
            <el-select style="width: 80%" size="small" v-model="nodeProps.formUser" placeholder="请选择人员类型的表单项">
              <el-option v-for="(op, idx) in forms" :label="op.title" :value="op.id" :key="idx"></el-option>
            </el-select>
          </el-form-item>
        </div>
        <div v-else-if="nodeProps.assignedType === 'EQUIP_USER'">
          <el-form-item label="选择表单设备项" prop="text" class="approve-end">
            <el-select style="width: 80%" size="small" v-model="nodeProps.formDevice" placeholder="请选择设备类型的表单项">
              <el-option v-for="(op, idx) in formd" :label="op.title" :value="op.id" :key="idx"></el-option>
            </el-select>
          </el-form-item>
          <div class="item-desc">设备拥有者的设备所属房间的责任人，如不存在则是设备拥有者的管理员</div>
        </div>
        <div v-else>
          <span class="item-desc">发起人自己作为办理人</span>
        </div>
      </el-form-item>

      <!-- <el-divider></el-divider>
      <el-form-item v-if="nodeProps.assignedType === 'FORM_USER'" label="👤 变更办理人" prop="text" class="line-mode">
        <el-switch v-model="nodeProps.enableChange" active-color="#13ce66" inactive-color="#ff4949">
        </el-switch>
      </el-form-item> -->

      <el-divider></el-divider>
      <el-form-item label="✍ 按钮文本设置" prop="text">
        执行文本：<el-input v-model="nodeProps.exeTxt" placeholder="请输入执行显示文本"
          style="width:120px;margin-right: 10px;"></el-input>
        驳回文本：<el-input v-model="nodeProps.refuseTxt" placeholder="请输入驳回显示文本" style="width:120px;"></el-input>
      </el-form-item>
      <el-form-item label="☞ 执行操作初始化" prop="text">
        <el-button type="text" @click="addTextLoad()">+ 添加</el-button>
        <div v-for="(tmpitem, index) in nodeProps.optionInit" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
          <el-select v-model="nodeProps.optionInit[index].optionName" placeholder="操作">
            <el-option :label="nodeProps.exeTxt" :value="nodeProps.exeTxt"></el-option>
            <el-option :label="nodeProps.refuseTxt" :value="nodeProps.refuseTxt"></el-option>
          </el-select>
          <el-select v-model="nodeProps.optionInit[index].fieldid" placeholder="表单">
            <el-option :label="ites.title" :value="ites.id" v-for="ites in loadforms" :key="'l'+ites.id"></el-option>
          </el-select>
          <el-input type="text" v-model="nodeProps.optionInit[index].InitValue" placeholder="初始化值"/>
          <el-button style="margin-left: 10px" size="mini" @click="dellTextLoad(index)" type="danger" icon="el-icon-delete" circle></el-button>
        </div>
      </el-form-item>
      
      <el-form-item label="⏱ 办理期限（为 0 则不生效）" prop="timeLimit">
        <el-input style="width: 180px" placeholder="时长" size="small" type="number"
          v-model="nodeProps.timeLimit.timeout.value">
          <el-select style="width: 85px" v-model="nodeProps.timeLimit.timeout.unit" slot="append" placeholder="请选择">
            <el-option label="天" value="D"></el-option>
            <el-option label="小时" value="H"></el-option>
          </el-select>
        </el-input>
      </el-form-item>
      <el-form-item label="办理期限超时后执行" prop="level" v-if="nodeProps.timeLimit.timeout.value > 0">
        <el-radio-group v-model="nodeProps.timeLimit.handler.type">
          <el-radio label="REFUSE">自动驳回</el-radio>
          <el-radio label="NOTIFY">发送提醒</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.timeLimit.handler.type === 'NOTIFY'">
          <div style="color: #409eef; font-size: small">默认提醒当前办理人</div>
          <el-switch inactive-text="循环" active-text="一次" v-model="nodeProps.timeLimit.handler.notify.once"></el-switch>
          <span style="margin-left: 20px" v-if="!nodeProps.timeLimit.handler.notify.once">
            每隔
            <el-input-number :min="0" :max="10000" :step="1" size="mini"
              v-model="nodeProps.timeLimit.handler.notify.hour" />
            小时提醒一次
          </span>
        </div>
      </el-form-item>
      <el-form-item label="🙅‍ 如果被驳回 👇">
        <el-radio-group v-model="nodeProps.refuse.type">
          <el-radio label="TO_END">直接结束流程</el-radio>
          <el-radio label="TO_BEFORE">驳回到上级办理节点</el-radio>
          <el-radio label="TO_NODE">驳回到指定节点</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.refuse.type === 'TO_NODE'">
          <span>指定节点:</span>
          <el-select style="margin-left: 10px; width: 150px" placeholder="选择跳转步骤" size="small"
            v-model="nodeProps.refuse.target">
            <el-option v-for="(node, i) in nodeOptions" :key="i" :label="node.name" :value="node.id"></el-option>
          </el-select>
        </div>
      </el-form-item>
    </el-form>
    <org-picker :title="pickerTitle" multiple ref="orgPicker" @ok="selected" />
  </div>
</template>

<script>
import OrgPicker from "../../OrgPicker.vue";
import OrgItems from "../OrgItems";
import { getItems } from "../../utlity"
import { checkPermi } from '@/utils/permission.js';

export default {
  name: "UserNodeConfig",
  components: { OrgPicker, OrgItems },
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      showOrgSelect: false,
      orgPickerType: "user",
      approvalTypes: [
        { name: "指定人员", type: "ASSIGN_USER" },
        { name: "角色", type: "ROLE" },
        { name: "发起人自己", type: "SELF" },
        { name: "表单内联系人", type: "FORM_USER" }
      ],
      selectObj: null,
      selectNo: false,
    };
  },
  computed: {
    nodeProps() {
      return this.$store.state.flowable.selectedNode.props;
    },
    select() {
      return this.config.assignedUser || [];
    },
    loadforms() {
      return getItems(this.$store.state.flowable.design.formItems).filter((f) => {
        return f.name === "TextareaInput"||f.name === "TextInput";
      });
    },
    forms() {
      return getItems(this.$store.state.flowable.design.formItems).filter((f) => {
        return f.name === "UserPicker";
      });
    },
    formd(){
      return getItems(this.$store.state.flowable.design.formItems).filter((f) => {
        return f.name === "DevicPicker";
      });
    },
    pickerTitle() {
      switch (this.orgPickerType) {
        case "user":
          return "请选择人员";
        case "role":
          return "请选择系统角色";
        default:
          return null;
      }
    },
    nodeOptions() {
      let values = [];
      const excType = [
        "ROOT",
        "EMPTY",
        "CONDITION",
        "CONDITIONS",
        "CONCURRENT",
        "CONCURRENTS",
      ];
      this.$store.state.flowable.nodeMap.forEach((v) => {
        if (excType.indexOf(v.type) === -1) {
          values.push({ id: v.id, name: v.name });
        }
      });
      return values;
    },
  },
  mounted() {
    if (checkPermi(['/AgentMan/'])) {
      if(!this.approvalTypes.some(x=>x.name=="设备拥有者")){
        this.approvalTypes.push({ name: "设备拥有者", type: "EQUIP_USER" })
      }
    }

  },
  methods: {
    addTextLoad(){
      //添加初始化文本
      if(this.nodeProps.optionInit==undefined){
        this.nodeProps.optionInit=[]
      }
      this.nodeProps.optionInit.push({
        optionName:'',
        fieldid:'',
        InitValue:''
      })
    },
    dellTextLoad(index){
      this.nodeProps.optionInit.splice(index,1)
      this.$forceUpdate()
    },
    selectUser() {
      this.selectObj = this.select;
      this.orgPickerType = "user";
      this.$refs.orgPicker.show(this.select, this.orgPickerType);
    },
    selectRole() {
      this.selectObj = this.config.role;
      this.orgPickerType = "role";
      this.$refs.orgPicker.show(this.config.role, this.orgPickerType);
    },
    selectChangeUser() {
      this.selectObj = this.config.changeUser;
      this.orgPickerType = "user";
      this.$refs.orgPicker.show(
        this.config.changeUser,
        this.orgPickerType
      );
    },
    selectChangeRole() {
      this.selectObj = this.config.changeRole;
      this.orgPickerType = "role";
      this.$refs.orgPicker.show(this.config.changeRole, this.orgPickerType);
    },
    selected(select) {
      this.selectObj.length = 0;
      select.forEach((val) => this.selectObj.push(val));
    },
    radioChange() {
      if (this.selectObj != null) {
        this.selectObj.length = 0;
      }
    },
    changeway() {
      this.config.changeUser.length = 0;
      this.config.changeRole.length = 0;
    },
  },
};
</script>

<style lang="less" scoped>
.user-type {
  /deep/ .el-radio {
    width: 110px;
    margin-top: 10px;
    margin-bottom: 20px;
  }
}

/deep/ .line-mode {
  .el-radio {
    width: 90px;
    margin: 5px;
  }
}

/deep/ .el-form-item__label {
  line-height: 25px;
}

/deep/ .approve-mode {
  .el-radio {
    float: left;
    width: 100%;
    display: block;
    margin-top: 15px;
  }
}

/deep/ .approve-end {
  position: relative;

  .el-radio-group {
    width: 160px;
  }

  .el-radio {
    margin-bottom: 5px;
    width: 100%;
  }

  .approve-end-leave {
    position: absolute;
    bottom: -5px;
    left: 150px;
  }
}

/deep/ .el-divider--horizontal {
  margin: 10px 0;
}
.item-desc{
  color:#999;
}
</style>