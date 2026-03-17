<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="⚙ 选择审批对象" prop="text" class="user-type">
        <el-radio-group v-model="nodeProps.assignedType" @change="radioChange">
          <el-radio v-for="t in approvalTypes" :label="t.type" :key="t.type">{{
            t.name
          }}</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.assignedType === 'ASSIGN_USER'">
          <el-button
            size="mini"
            icon="el-icon-plus"
            type="primary"
            @click="selectUser"
            round
            >选择人员</el-button
          >
          <org-items v-model="nodeProps.assignedUser" />
        </div>
        <div v-else-if="nodeProps.assignedType === 'SELF_SELECT'">
          <el-radio-group size="mini" v-model="nodeProps.selfSelect.multiple">
            <!-- <el-radio-button :label="false">自选一个人</el-radio-button> -->
            <el-radio-button :label="true">自选多个人</el-radio-button>
          </el-radio-group>
        </div>
        <div v-else-if="nodeProps.assignedType === 'LEADER_TOP'">
          <el-divider />
          <el-form-item label="审批终点" prop="text" class="approve-end">
            <el-radio-group v-model="nodeProps.leaderTop.endCondition">
              <el-radio label="TOP">直到最上层主管</el-radio>
              <el-radio label="LEAVE">不超过发起人的</el-radio>
            </el-radio-group>
            <div
              class="approve-end-leave"
              v-if="nodeProps.leaderTop.endCondition === 'LEAVE'"
            >
              <span>第 </span>
              <el-input-number
                :min="1"
                :max="20"
                :step="1"
                size="mini"
                v-model="nodeProps.leaderTop.endLevel"
              />
              <span> 级主管</span>
            </div>
          </el-form-item>
        </div>
        <div v-else-if="nodeProps.assignedType === 'LEADER'">
          <el-divider />
          <el-form-item label="指定主管" prop="text">
            <span>发起人的第 </span>
            <el-input-number
              :min="1"
              :max="20"
              :step="1"
              size="mini"
              v-model="nodeProps.leader.level"
            ></el-input-number>
            <span> 级主管</span>
            <div style="color: #409eff; font-size: small">
              👉 直接主管为 第 1 级主管
            </div>
          </el-form-item>
        </div>
        <div v-else-if="nodeProps.assignedType === 'ROLE'">
          <el-button
            size="mini"
            icon="el-icon-plus"
            type="primary"
            @click="selectRole"
            round
            >选择角色</el-button
          >
          <org-items v-model="nodeProps.role" />
        </div>
        <div v-else-if="nodeProps.assignedType === 'FORM_USER'">
          <el-form-item
            label="选择表单联系人项"
            prop="text"
            class="approve-end"
          >
            <el-select
              style="width: 80%"
              size="small"
              v-model="nodeProps.formUser"
              placeholder="请选择包含联系人的表单项"
            >
              <el-option
                v-for="(op,idx) in forms"
                :label="op.title"
                :value="op.id"
                :key="idx"
              ></el-option>
            </el-select>
          </el-form-item>
        </div>
        <div v-else>
          <span class="item-desc">发起人自己作为审批人进行审批</span>
        </div>
      </el-form-item>

      <el-divider></el-divider>
      <el-form-item label="👤 审批人为空时" prop="text" class="line-mode">
        <el-radio-group v-model="nodeProps.nobody.handler" @change="nochange">
          <el-radio label="TO_PASS">自动通过</el-radio>
          <el-radio label="TO_REFUSE">自动驳回</el-radio>
          <el-radio label="TO_USER">转交到指定人员</el-radio>
        </el-radio-group>

        <div
          style="margin-top: 10px"
          v-if="nodeProps.nobody.handler === 'TO_USER'"
        >
          <el-button
            size="mini"
            icon="el-icon-plus"
            type="primary"
            @click="selectNoSetUser"
            round
            >选择人员</el-button
          >
          <org-items v-model="nodeProps.nobody.assignedUser" />
        </div>
      </el-form-item>

      <div v-if="showMode">
        <el-divider />
        <el-form-item
          label="👩‍👦‍👦 多人审批时审批方式"
          prop="text"
          class="approve-mode"
        >
          <el-radio-group v-model="nodeProps.mode">
            <el-radio label="NEXT"
              >会签 （按选择顺序审批，每个人必须同意）</el-radio
            >
            <el-radio label="AND">会签（可同时审批，每个人必须同意）</el-radio>
            <el-radio label="OR">或签（有一人同意即可）</el-radio>
          </el-radio-group>
        </el-form-item>
      </div>

      <el-divider>高级设置</el-divider>
      <el-form-item label="✍ 审批同意时是否需要签字" prop="text">
        <el-switch
          inactive-text="不用"
          active-text="需要"
          v-model="nodeProps.sign"
        ></el-switch>
        <el-tooltip
          class="item"
          content="如果全局设置了需要签字，则此处不生效"
          placement="top-start"
        >
          <i
            class="el-icon-question"
            style="margin-left: 10px; font-size: medium; color: #b0b0b1"
          ></i>
        </el-tooltip>
      </el-form-item>
      <el-form-item label="☞ 执行操作初始化" prop="text">
        <el-button type="text" @click="addTextLoad()">+ 添加</el-button>
        <div v-for="(tmpitem, index) in nodeProps.optionInit" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
          <el-select v-model="nodeProps.optionInit[index].optionName" placeholder="操作">
            <el-option label="同意" value="同意"></el-option>
            <el-option label="驳回" value="驳回"></el-option>
          </el-select>
          <el-select v-model="nodeProps.optionInit[index].fieldid" placeholder="表单">
            <el-option :label="ites.title" :value="ites.id" v-for="ites in loadforms" :key="'l'+ites.id"></el-option>
          </el-select>
          <el-autocomplete v-model="nodeProps.optionInit[index].InitValue" :fetch-suggestions="queryUserNodeSearch"
            placeholder="请输入内容" @select="handleUseNodeSelect(index,$event)" :clearable="true">
            <template slot-scope="{ item }">
              <div class="atname">{{ item.name }}</div>
            </template>
          </el-autocomplete>
          <el-button style="margin-left: 10px" size="mini" @click="dellTextLoad(index)" type="danger" icon="el-icon-delete" circle></el-button>
        </div>
      </el-form-item>
      <el-form-item label="⏱ 审批期限（为 0 则不生效）" prop="timeLimit">
        <el-input
          style="width: 180px"
          placeholder="时长"
          size="small"
          type="number"
          v-model="nodeProps.timeLimit.timeout.value"
        >
          <el-select
            style="width: 85px"
            v-model="nodeProps.timeLimit.timeout.unit"
            slot="append"
            placeholder="请选择"
          >
            <el-option label="天" value="D"></el-option>
            <el-option label="小时" value="H"></el-option>
          </el-select>
        </el-input>
      </el-form-item>
      <el-form-item
        label="审批期限超时后执行"
        prop="level"
        v-if="nodeProps.timeLimit.timeout.value > 0"
      >
        <el-radio-group v-model="nodeProps.timeLimit.handler.type">
          <el-radio label="PASS">自动通过</el-radio>
          <el-radio label="REFUSE">自动驳回</el-radio>
          <el-radio label="NOTIFY">发送提醒</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.timeLimit.handler.type === 'NOTIFY'">
          <div style="color: #409eef; font-size: small">默认提醒当前审批人</div>
          <el-switch
            inactive-text="循环"
            active-text="一次"
            v-model="nodeProps.timeLimit.handler.notify.once"
          ></el-switch>
          <span
            style="margin-left: 20px"
            v-if="!nodeProps.timeLimit.handler.notify.once"
          >
            每隔
            <el-input-number
              :min="0"
              :max="10000"
              :step="1"
              size="mini"
              v-model="nodeProps.timeLimit.handler.notify.hour"
            />
            小时提醒一次
          </span>
        </div>
      </el-form-item>
      <el-form-item label="🙅‍ 如果审批被驳回 👇">
        <el-radio-group v-model="nodeProps.refuse.type">
          <el-radio label="TO_END">直接结束流程</el-radio>
          <el-radio label="TO_BEFORE">驳回到上级审批节点</el-radio>
          <el-radio label="TO_NODE">驳回到指定节点</el-radio>
        </el-radio-group>
        <div v-if="nodeProps.refuse.type === 'TO_NODE'">
          <span>指定节点:</span>
          <el-select
            style="margin-left: 10px; width: 150px"
            placeholder="选择跳转步骤"
            size="small"
            v-model="nodeProps.refuse.target"
          >
            <el-option
              v-for="(node, i) in nodeOptions"
              :key="i"
              :label="node.name"
              :value="node.id"
            ></el-option>
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
import {getItems} from "../../utlity"
export default {
  name: "ApprovalNodeConfig",
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
        { name: "发起人自选", type: "SELF_SELECT" },
        { name: "连续多级主管", type: "LEADER_TOP" },
        { name: "主管", type: "LEADER" },
        { name: "角色", type: "ROLE" },
        { name: "发起人自己", type: "SELF" },
        { name: "表单内联系人", type: "FORM_USER" },
      ],
      selectObj: null,
      selectNo: false,
    };
  },
  computed: {
    loadforms() {
      return getItems(this.$store.state.flowable.design.formItems).filter((f) => {
        return f.name === "TextareaInput"||f.name === "TextInput";
      });
    },
    nodeProps() {
      return this.$store.state.flowable.selectedNode.props;
    },
    select() {
      return this.config.assignedUser || [];
    },
    forms() {
      return getItems(this.$store.state.flowable.design.formItems).filter((f) => {
        return f.name === "UserPicker";
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
    showMode() {
      switch (this.nodeProps.assignedType) {
        case "ASSIGN_USER":
          return this.nodeProps.assignedUser.length > 0;
        case "SELF_SELECT":
          return this.nodeProps.selfSelect.multiple;
        case "LEADER_TOP":
          return true;
        case "FORM_USER":
          return true;
        case "ROLE":
          return true;
        default:
          return false;
      }
    },
  },
  methods: {
    queryUserNodeSearch(queryString, cb) {
      cb([{ "name": "审批人姓名", "value": "@审批人姓名" }]);
    },
    handleUseNodeSelect(index,item){
      this.nodeProps.optionInit[index].InitValue=item.value;
    },
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
    },
    selectUser() {
      this.selectObj = this.select;
      this.selectNo = false;
      this.orgPickerType = "user";
      this.$refs.orgPicker.show(this.select, this.orgPickerType);
    },
    selectRole() {
      this.selectObj = this.config.role;
      this.selectNo = false;
      this.orgPickerType = "role";
      this.$refs.orgPicker.show(this.config.role, this.orgPickerType);
    },
    selectNoSetUser() {
      this.orgPickerType = "user";
      this.selectNo = true;
      this.$refs.orgPicker.show(
        this.config.nobody.assignedUser,
        this.orgPickerType
      );
    },
    selected(select) {
      if (this.selectNo) {
        this.config.nobody.assignedUser.length = 0;
        select.forEach((val) => this.config.nobody.assignedUser.push(val));
      } else {
        this.selectObj.length = 0;
        select.forEach((val) => this.selectObj.push(val));
      }
    },
    radioChange() {
      if (this.selectObj != null) {
        this.selectObj.length = 0;
      }
      if(this.nodeProps.assignedType=='SELF_SELECT'){
        this.nodeProps.selfSelect.multiple=true
      }
    },
    nochange() {
      this.config.nobody.assignedUser.length = 0;
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
    width: 150px;
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
</style>
