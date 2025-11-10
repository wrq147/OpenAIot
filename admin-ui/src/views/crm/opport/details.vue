<template>
   <div id="opport-details">
         <!-- <el-dialog title="" id="shangDetais" :close-on-click-modal="false" :visible.sync="detailsOped" width="1098px"
      append-to-body class="add_dialog_border opport_add" @close="cancel"> -->
      <div class="opport-details-par">
     <!-- <div class="custom_title">
        <div class="tag">商机</div>
         <div class="name">刘佳</div>
      </div> -->
      <div>
        <div class="jianjie">
          <div class="jianjie_con">
            <div class="jianjie_li">
              <div class="label">客户编号：</div>
              <div class="value">客户编号：</div>
            </div>
            <div class="jianjie_li">
              <div class="label">负责人：</div>
              <div class="value haiImg">
                <img class="image" :src="$store.state.user.avatar" alt="">
                <span>{{ leaderNameText }}</span>
              </div>
            </div>
          </div>
          <div class="button_con">
            <el-button @click="handleUpdate({ Id: customInDetail.Id })">编辑</el-button>
            <!-- <el-button type="primary" @click="handleTransform(customInDetail.Id)" class="blue_btn">转换</el-button> -->
          </div>
        </div>
        <div class="AdvanceList">
          <p @click="handleChange(item)" v-for="(item, index) in periodSelectlis" :key="index"
            :class="['AdvanceList-item', index > 3 && item.PeriodName == periodTypeMapText ? 'active3Select' : index <= 3 && item.PeriodName == periodTypeMapText ? 'active1' : index == 0 || index == 1 || index == 2 || index == 3 ? 'active2' : index == 4 ? 'active3' : index == 5 ? 'active4' : index == 6 ? 'active5' : '']">
            {{ item.PeriodName }}</p>
        </div>
        <el-row :gutter="10" style="display:flex;">
          <el-col :span="18" style="width:70%;">
          <el-tabs v-model="information" @tab-click="handleClickInformation" style="width:100%;">
            <el-tab-pane label="基本信息" name="two">


              <div class="basic_info">
                <div class="row_cot ">
                  <div class="cot_label">商机名称</div>
                  <div class="cot_value">{{ detailsRow.OpportName }}</div>
                </div>
                <div class="row_cot">
                  <div class="cot_label">客户名称</div>
                  <div class="cot_value">{{ CustomerIdText }}</div>
                </div>
                <div class="row_cot ">
                  <div class="cot_label">联系人</div>
                  <div class="cot_value">{{ ContactIdText }}</div>
                </div>
                <div class="row_cot ">
                  <div class="cot_label">成交几率</div>
                  <div class="cot_value">{{ detailsRow.Probability }}</div>
                </div>
                <div class="row_cot">
                  <div class="cot_label">负责人</div>
                  <div class="cot_value blue"> {{ leaderNameText }}</div>
                </div>
                <div class="row_cot " v-if="helperNameText">
                  <div class="cot_label">协作人</div>
                  <div class="cot_value">{{ helperNameText }}</div>
                </div>
                <div class="row_cot ">
                  <div class="cot_label">销售阶段</div>
                  <div class="cot_value blue">{{ periodTypeMapText }}</div>
                </div>
                <div class="row_cot">
                  <div class="cot_label">创建时间</div>
                  <div class="cot_value blue">{{ detailsRow.createTime }}</div>
                </div>
                <div class="row_cot " v-if="detailsRow.Remark">
                  <div class="cot_label">商机详情</div>
                  <div class="cot_value">
                    {{ detailsRow.Remark }}</div>
                </div>

              </div>
            </el-tab-pane>

            <el-tab-pane label="商机明细" name="second">
              <el-table :data="detailsRow.DetailList" stripe style="width: 100%" v-loading="choiceLoading">
                <el-table-column prop="TargetId" align="center" label="产品编号" width="180"></el-table-column>
                <el-table-column prop="Name" label="产品名称"></el-table-column>
                <!-- <el-table-column align="center" label="数量" width="140">
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column> -->
                <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
                  <template slot-scope="scope">
                    <div>
                      {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
                    </div>
                  </template>
                </el-table-column>
                <el-table-column prop="address" align="center" label="操作" width="110">
                  <template slot-scope="scope">
                    <el-link type="danger" icon="el-icon-delete" @click="deleteProductLis(scope.$index)">删除</el-link>
                  </template>
                </el-table-column>
              </el-table>

            </el-tab-pane>


          </el-tabs>
    </el-col>



          <el-col :span="6" style="width:30%;margin-left:auto;background:#F9FAFC;padding-left:15px;">

            <div style="margin-left:10px;padding:20px 0px;">
              <button style="border:none;padding:7px 8px;border-radius:4px;margin-right:14px;" type="primary" @click="activeName = 0"
                :class="[activeName == 0 ? 'active' : 'on']">快速跟进</button>
              <button v-if="isCheckPermi(['/DiscussService/Comment/List'])" style="border:none;padding:7px 8px;border-radius:4px;" @click="activeName = 1"
                :class="[activeName == 1 ? 'active' : 'on']">评论</button>
            </div>
            <div v-if="activeName == 0">
              <div>
                <el-form class="biaodan" :model="followAddForm" :rules="followAddRules" ref="queryForm" :inline="true"
                  label-position="top">
                  <div v-if="!hideTitle">


                    <el-form-item label="跟进内容" prop="remark">
                      <el-input type="textarea" :rows="2" placeholder="请填写跟进记录" v-model="followAddForm.remark"
                        style="width:300px;">
                      </el-input>
                    </el-form-item>

                    <el-form-item label="跟进时间" prop="followTime">
                      <el-date-picker v-model="followAddForm.followTime" type="datetime" placeholder="选择日期时间">
                      </el-date-picker>
                    </el-form-item>
                    <el-form-item label="跟进方式" prop="followWay">
                      <el-select v-model="followAddForm.followWay" placeholder="请选择">
                        <el-option v-for="item in followWayList" :key="item.value" :label="item.label"
                          :value="item.value">
                        </el-option>

                      </el-select>
                    </el-form-item>
                    <el-form-item label="跟进人" prop="followUser">

                      <el-select class="form_input_style" filterable allow-create default-first-option
                        v-model="followAddForm.followUserName" ref="selectFollow" placeholder="请选择跟进人"
                        @focus="getFollowFocus" style="width:100%"></el-select>
                      <org-picker :multiple="false" ref="followPicker" :selected="followAddForm.followUserInfo"
                        @ok="selectFollower" />
                    </el-form-item>
                    <!--  <el-form-item label="归属部门">
             <el-select v-model="value" placeholder="请选择">
    <el-option
      v-for="item in options"
      :key="item.value"
      :label="item.label"
      :value="item.value">
    </el-option>
  </el-select>
            </el-form-item> -->
                    <div style="margin-bottom:30px;">
                      <el-button @click="handOk()" type="primary"
                        style="margin-top:15px;width:120px;margin-left:30px;">确定</el-button>
                      <el-button @click="handCancel()"
                        style="margin-top:15px;width:120px;margin-left:20px;">取消</el-button>
                    </div>

                  </div>
                </el-form>

              </div>
              <div v-if="hideTitle">
                <div class="recordParse">
                  <el-input type="textarea" @focus="searchList" v-model="searchInout" placeholder="请输入搜索内容" style="margin-top:20px;"></el-input>
                  <div v-if="RecordList.length>0">
                  <div  v-for="(item, index) in RecordList" :key="index" class="recordList">
                    <div style="display:flex;">
                      <div class="avatar" v-if="!item.FollowUserAvatar">
                        {{ item.FollowUserName.slice(item.FollowUserName.length - 1) }}</div>
                      <div class="avatar" v-if="item.FollowUserAvatar">
                        <img :src="item.FollowUserAvatar" alt="">
                      </div>
                      <div class="text_name">{{ item.FollowUserName }}</div>
                      <el-button @click="handEdit(item)" type="text"
                        style="margin-left:auto;margin-top:-5px;">编辑</el-button>
                    </div>
                    <p>
                      <span>
                        {{ item.Remark }}
                      </span>
                      <!-- <span class="FollowTime">
                  {{item.FollowTime}}
               </span> -->
                    </p>
                    <!-- <p>联系人：123123</p> -->
                    <p>跟进方式： {{ item.FollowWayName }}</p>
                    <p>跟进时间： {{ item.FollowTime }}</p>
                    <!-- <p class="recordList-button"><el-button type="text">查看</el-button><el-button @click="handEdit(item)" type="text">编辑</el-button></p> -->
                  </div>
                  </div>
                 
                   <el-empty style="margin:0 auto;" description="暂无跟进记录" v-else :image-size="200"></el-empty>
                </div>
                <!-- <div v-if="RecordList.length<=0" style="height:300px;width:100%;"></div> -->
                <!-- <el-button @click="handSubmit()" type="primary"
                  style="width:100%;margin-top:15px;background: linear-gradient(90deg, #4C79FF 0%, #6DA8FF 100%);">添加跟进动态</el-button> -->
              </div>

            </div>
            <div v-if="activeName == 1">


              <div style="height:300px;overflow:scroll;">
                <div class="comment_list" v-infinite-scroll="loadCustomComment" style="overflow:auto">
                  <div v-if="customCommentObj.length!=0">

                 
                  <div class="comment_li" v-for="row in customCommentObj" :key="row.Id" >
                    <div class="title">
                      <div class="avatar" v-if="!row.UserAvatar">
                        {{ row.UserName.slice(row.UserName.length - 1) }}</div>
                      <div class="avatar" v-if="row.UserAvatar">
                        <img :src="row.UserAvatar" alt="">
                      </div>
                      <div class="text_name">{{ row.UserName }}</div>
                    </div>
                    <div class="content" v-html="row.Content"></div>
                    <div class="time_btn">
                      <div class="time">{{ row.CreateOn }}</div>
                      <div class="return_btn" @click="openPinglunForm(row.Id, row.UserId)">
                        <svg-icon icon-class="reply-icon" color="#78829D" />
                        <span class="text">回复</span>
                      </div>
                    </div>
                    <div class="return_comment">
                      <div v-if="row.returnList && row.returnList.length > 0">
                        <div class="return_li" v-for="item in row.returnList" :key="item.Id">
                          <div class="avatar_name">
                            <div class="avatar" v-if="!item.UserAvatar && item.UserName">
                              {{ item.UserName.slice(item.UserName.length - 1) }}</div>
                            <div class="avatar" v-if="item.UserAvatar">
                              <img :src="item.UserAvatar" alt="">
                            </div>
                            <div class="text_name">{{ item.UserName }}</div>
                          </div>
                          <div class="return_name" v-if="item.ParentCommentUserId != row.UserId">
                            回复<span class="name">{{ item.ParentCommentUserName }}</span>：
                          </div>
                          <div class="content" v-html="item.Content"></div>
                          <div class="time_btn">
                            <div class="time">{{ item.CreateOn }}</div>
                            <div class="return_btn" @click="openPinglunForm(row.Id, item.UserId)">
                              <svg-icon icon-class="reply-icon" color="#78829D" />
                              <span class="text">回复</span>
                            </div>
                          </div>
                        </div>
                      </div>
                           
                    </div>
                  </div>
                </div>
                  <el-empty description="暂无评论" v-else :image-size="200"></el-empty>
                </div>
              </div>
              <div class="add_btn" style="width:100%;">

                <el-button @click="openPinglunForm" type="primary"
                  style="width:95%;margin-top:45px;background: linear-gradient(90deg, #4C79FF 0%, #6DA8FF 100%);">添加评论</el-button>
              </div>
            </div>


          </el-col>

        </el-row>
      </div>

       <el-dialog :visible.sync="pinglunEdit" width="800px" :show-close="true" @close="pinglunEdit = false"
      class="edit_info_dialog" :close-on-click-modal="false">
      <el-form class="add_clue_form" ref="pinglunForm" :model="pinglunForm" :rules="pinglunRules" label-width="100px"
        label-position="top">
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="回复内容" prop="content">
              <!-- <el-input type="textarea" v-model="form.remark" placeholder="请输入跟进内容"
                  :autosize="{ minRows: 5, maxRows: 5 }"></el-input> -->
              <text-editor :value="pinglunForm.content" @input="editorInput" :fileSize="10" :height="380"
                :editOpen="pinglunEdit"></text-editor>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitPinglunForm">确 定</el-button>
        <el-button @click="closePinglunForm">取 消</el-button>
      </div>
    </el-dialog>
    </div>
    <!-- </el-dialog> -->
          <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="800px" append-to-body
        class="add_dialog_border opport_add" @close="cancel">
        <el-form class="add_clue_form" ref="opportForm" :model="opportForm" :rules="rules" label-width="100px"
          label-position="top">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="商机名称" required prop="opportName">
                <el-input class="form_input_style" v-model="opportForm.opportName" placeholder="请输入商机名称" clearable
                  size="small" style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="商机编号" required prop="opportNumber">
                <el-input class="form_input_style" v-model="opportForm.opportNumber" placeholder="请输入商机编号" clearable
                  size="small" style="width:100%" :disabled="true" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户名称" required prop="customerId">
                <el-input class="form_input_style" v-model="opportForm.customerName" placeholder="请选择客户" clearable
                  size="small" style="width:100%" @focus="openCustomDialog" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户编号" prop="customerNumber">
                <el-input class="form_input_style" v-model="opportForm.customerNumber" placeholder="请输入客户编号" clearable
                  size="small" style="width:100%" :disabled="true" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户联系人" required prop="contactId">
                <el-input class="form_input_style" v-model="opportForm.contactName" placeholder="请选择联系人" clearable
                  size="small" style="width:100%" @focus="openContactDialog" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="销售阶段" required prop="period">
                <el-select class="form_input_style" v-model="opportForm.period" placeholder="请选择客户类型" style="width:100%"
                  @change="periodChange">
                  <el-option :key="item.Id" :label="item.PeriodName" :value="item.Id"
                    v-for="item in periodSelectlis"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="赢率" required prop="probability">
                <el-input class="form_input_style" v-model="opportForm.probability" placeholder="请输入预计成交几率" clearable
                  style="width:100%" type="number" :disabled="true" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="负责人" prop="leaderId">
                <el-select class="form_input_style" filterable allow-create default-first-option
                  v-model="opportForm.leaderName" ref="selectLeader" placeholder="请选择负责人" @focus="getLeaderFocus"
                  style="width:100%"></el-select>
                <org-picker2 :multiple="false" ref="leaderPicker" :selected="opportForm.leaderInfo"
                  @ok="selectLeadered" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="协作人" prop="helperName">
                <el-select class="form_input_style" multiple v-model="opportForm.helperName" ref="selectUsers"
                  placeholder="请选择协作人" @focus="getUsersFocus" style="width:100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="opportForm.userInfo" @ok="selectUsersed" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="商机详情" prop="remark">
                <el-input type="textarea" v-model="opportForm.remark" placeholder="请输入商机详情"
                  :autosize="{ minRows: 2, maxRows: 4 }"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <div>
            <div class="items-title">
              <div style="font-size:16px;color:#333;">商机明细</div>
              <div>
                <el-row :gutter="15" type="flex" justify="end">
                  <el-col :span="1.5">
                    <el-button type="primary" size="mini" plain @click="openWupinDialog">
                      <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                      <span style="margin-left:6px">选择产品</span>
                    </el-button>
                  </el-col>
                </el-row>
              </div>
            </div>
            <el-table :data="detailsRow.DetailList" stripe style="width: 100%" v-loading="choiceLoading">
              <el-table-column prop="number" align="center" label="产品编号" width="180"></el-table-column>
              <el-table-column prop="name" label="产品名称"></el-table-column>
              <el-table-column prop="label" align="center" label="标签" width="220"></el-table-column>
              <el-table-column prop="quantity" align="center" label="数量" width="140">
                <template slot-scope="scope">
                  <span v-if="scope.row.label == '成品'">{{
                    scope.row.quantity
                  }}</span>
                  <el-input-number v-else v-model="scope.row.quantity" :min="1" :max="9999" size="mini"></el-input-number>
                </template>
              </el-table-column>
              <el-table-column prop="price" align="center" label="价格" width="140">
                <template slot-scope="scope">
                  <el-input-number v-model="scope.row.price" :precision="2" size="mini" :min="0"></el-input-number>
                </template>
              </el-table-column>
              <el-table-column align="center" label="操作" width="110">
                <template slot-scope="scope">
                  <el-link type="danger" icon="el-icon-delete" @click="deleteProductLis(scope.$index)">删除</el-link>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
      <wupin-choice ref="wupinChoice" @onWupinConfirm="onWupinConfirm"
      :detailList="opportForm.detailList2"></wupin-choice>
   </div>
</template>
<script>

import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  opportList,
  opportForward,
  opportAdd,
  opportEdit,
  GenerateNumber,
  opportInfo,
  periodList,
  recordList,
  AddComment
} from "@/api/crm/opport";
import { commentAddClue, commentList } from '@/api/crm/comment'
import {
  addFollow,
  editFollow,
} from "@/api/crm/follow";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import OrgPicker2 from "@/views/flowable/common/OrgPicker";
import { listMember } from "@/api/system/Employee";
import {
  contactList,
} from "@/api/crm/contact";
import { privateCustomer } from "@/api/crm/customer";
import WupinChoice from "../compontent/wupin-choice.vue"
import CustomChoice from "@/views/crm/compontent/custom-choice.vue"
import ContactChoice from "@/views/crm/compontent/contact-choice.vue"
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
import { checkPermi } from "@/utils/permission"; 
export default {
  name: "cluePrilist",
  components: { OrgPicker, OrgPicker2, WupinChoice, CustomChoice, ContactChoice, TextEditor },
  mixins: [resizeTableCon],
  dicts: ["follow_way"],
  data() {
    return {
      followAddRules: {
        followTime: [
          {
            required: true,
            message: "跟进时间不能为空",
            trigger: ["blur", "change"]
          }
        ],
        followUser: [
          {
            required: true,
            message: "跟进人不能为空",
            trigger: ["blur"]
          }
        ],
        followWay: [
          {
            required: true,
            message: "跟进方式不能为空",
            trigger: ["blur", "change"]
          },
        ],
        targetId: [
          {
            required: true,
            message: "跟进目标不能为空",
            trigger: ["blur"]
          }
        ],
        remark: [
          {
            required: true,
            message: "跟进内容不能为空",
            trigger: ["blur"]
          }
        ]
      },
      FolloMethod: [
        {
          label: '电话拜访',

        },
        {
          label: '现场拜访',

        }
      ],
      options: [{
        value: '选项1',
        label: '黄金糕'
      }, {
        value: '选项2',
        label: '双皮奶'
      }, {
        value: '选项3',
        label: '蚵仔煎'
      }, {
        value: '选项4',
        label: '龙须面'
      }, {
        value: '选项5',
        label: '北京烤鸭'
      }],
      value: '',
      value1: '',//跟进时间
      AdvanceList: false,
      diaTitle: "添加商机", //弹出层的标题
      open: false, //弹出层
      //表格样式设计
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      //表格样式设计
      // 日期范围
      dateRange: [],
      priList: [], //商机列表
      // 显示搜索条件
      showSearch: true,

      // 列信息
      columns: [
        { key: 0, label: `企业ID`, visible: true },
        { key: 1, label: `客户名称`, visible: true },
        { key: 2, label: `联系人`, visible: true },
        { key: 3, label: `手机号`, visible: true },
        { key: 4, label: `部门`, visible: true },
        { key: 5, label: `职务`, visible: true },
        { key: 6, label: `协作人名称`, visible: true },
        { key: 7, label: `商机来源`, visible: true },
        { key: 8, label: `创建时间`, visible: true }
      ],
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 列表总条数
      total: 0,
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        targetType: '商机',
        Belong: 1 //为1表示公海，为2表示私海，其它为全部（不用传）
      },
      rules: {
        opportName: [
          {
            required: true,
            message: "商机名称不能为空",
            trigger: ["blur", "change"]
          }
        ],
        probability: [
          {
            required: true,
            message: "预计成交几率不能为空",
            trigger: ["blur", "change"]
          }
        ],
        period: [
          {
            required: true,
            message: "销售阶段不能为空",
            trigger: ["blur", "change"]
          }
        ],
        customerName: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur", "change"]
          }
        ],
        leaderId: [
          {
            required: true,
            message: "负责人不能为空",
            trigger: ["blur", "change"]
          }
        ],
        contactId: [
          {
            required: true,
            message: "联系人不能为空",
            trigger: ["blur", "change"]
          }
        ],
      },
      opportForm: {
        opportNumber: "", //商机编码
        opportName: "", //商机名称
        customerId: "", //客户编码
        customerName: "", //客户编码（不传）
        customerNumber: "", //客户编码（不传）
        contactId: "", //联系人id
        contactName: "", //联系人名称（不传）
        probability: null, //预计成交几率
        period: "", //销售阶段
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helper: "",
        remark: "", //线索详情
        leaderId: 0, //负责人
        leaderName: [], //负责人
        leaderInfo: [], //负责人
        detailList: [], //商机明细
        detailList2: [], //商机明细
      },
      fromMap: new Map(), //商机来源map
      employeeMap: new Map(), //员工map
      periodTypeMap: new Map(),//销售阶段列表
      periodSelectlis: [], //销售阶段列表
      //商机明细相关变量
      choiceLoading: false,
      //商机明细相关变量
      //客户、联系人
      customerMap: new Map(),//客户
      contactMap: new Map(),//联系人
      //客户、联系人
      EditOpportId: 0,//是否是编辑
      AdvanceRow: [],//选取的数据
      detailsOped: false,//详情数据
      detailsRow: [],//详情
      textarea: '',
      leaderNameText: '',//详情id取名称---负责人名称
      CustomerIdText: '',//客户名称
      ContactIdText: '',
      periodTypeMapText: '',
      helperNameText: '',//协作人
      activeName: 0,
      information: 'two',
      RecordList: [],
      textareaComment: '',//评论提交
      hideTitle: true,//快速推进显隐
      searchInout: '',//搜索内容
      followAddForm: {
        targetType: 0,//0是客户，1是线索
        targetId: '',
        targetName: '',
        followTime: null,
        userInfo: [], //已经选择的跟进人
        followUserName: [], //跟进人选择
        followUserAvatar: '',
        followUser: null,//跟进人
        followWay: '',//跟进方式
        remark: "", //跟进记录详情
        Id: ''
      },
      followWayList: [],

      commentPage: 1,
      pinglunEdit: false,
      pinglunForm: {
        targetId: 0,
        targetType: null,
        parentCommentId: 0,
        parentCommentUserId: 0,
        content: ''//评论内容
      },
      customCommentList: [],//评论列表
      customCommentObj: [],//评论
      pinglunRules: {
        content: [
          {
            required: true,
            message: "回复内容不能为空",
            trigger: ["blur"]
          }
        ],
      },
    };
  },
  async mounted() {
    await this.getPeriodList()
    await this.getCustomerList()
    await this.getContactList()
    await this.getMemberList();
    this.getOpportList();
  this.handleOpenDetails();
  },
  methods: {
    isCheckPermi(val) {//权限判断
      return checkPermi(val)
    },
    periodChange(event) {
      // console.log("销售阶段改变", event);
      this.opportForm.probability=this.periodTypeMap.get(event).Probability
    },
    closePinglunForm() {
      //关闭评论写的弹窗
      this.pinglunEdit = false
    },
    submitPinglunForm() {
      //提交评论
      this.$refs.pinglunForm.validate(valid => {
        if (valid) {
          commentAddClue(this.pinglunForm).then((res) => {
            // console.log(res, '添加评论');
            if (res.code == 0) {
              this.$modal.msgSuccess("添加成功");
              this.pinglunEdit = false
              this.getCustomCommentList(1)
            }
          })
        }
      })
    },
    editorInput(value) {
      //富文本输入赋值
      this.pinglunForm.content = value
    },
    getCustomCommentList(page) {
      //获取客户评论列表
      if (page == 1) {
        this.customCommentList = []
        this.customCommentObj = {}
      }
      this.commentPage = page
      commentList({ TargetType:'商机', TargetId: this.customInDetail.Id, pageNum: page, pageSize: 10, }).then(res => {
        // console.log(res, '评论列表');
        if (res.data && res.data.List && res.data.List.length > 0) {
          res.data.List.map(row => {
            if (row.UserId && this.employeeMap.get(row.UserId)) {
              row.UserName = this.employeeMap.get(parseInt(row.UserId)).RealName
              row.UserAvatar = this.employeeMap.get(parseInt(row.UserId)).Avatar
            }
            if (row.ParentCommentUserId) {
              row.ParentCommentUserName = this.employeeMap.get(parseInt(row.ParentCommentUserId)).RealName
              row.ParentCommentUserAvatar = this.employeeMap.get(parseInt(row.ParentCommentUserId)).Avatar
            }
            let objKeys = Object.keys(this.customCommentObj)
            // console.log("评论键名", objKeys);
            if (row.ParentCommentId && row.ParentCommentId != 0) {
              if (objKeys.includes(row.ParentCommentId)) {
                if (this.customCommentObj[row.ParentCommentId].returnList && this.customCommentObj[row.ParentCommentId].returnList.length > 0) {
                  this.customCommentObj[row.ParentCommentId].returnList.push(row)
                } else {
                  this.customCommentObj[row.ParentCommentId].returnList = []
                  this.customCommentObj[row.ParentCommentId].returnList.push(row)
                }
              } else {
                this.customCommentObj[row.ParentCommentId] = {}
                if (this.customCommentObj[row.ParentCommentId].returnList && this.customCommentObj[row.ParentCommentId].returnList.length > 0) {
                  this.customCommentObj[row.ParentCommentId].returnList.push(row)
                } else {
                  this.customCommentObj[row.ParentCommentId].returnList = []
                  this.customCommentObj[row.ParentCommentId].returnList.push(row)
                }
              }
            } else {
              // console.log("");
              if (this.customCommentObj[row.Id]) {
                this.customCommentObj[row.Id] = { ...this.customCommentObj[row.Id], ...row }
              } else {
                this.customCommentObj[row.Id] = row
              }
            }
          })
          this.customCommentList = [...this.customCommentList, ...res.data.List]
          // console.log(this.customCommentObj, '评论');
          this.customCommentObj = JSON.parse(JSON.stringify(this.customCommentObj))
          this.$forceUpdate
        }
      })

    },
    loadCustomFollow() {
      //继续加载客户跟进
      this.followPage = this.followPage + 1
      this.getCustomFollowList(this.customInDetail.Id, this.followPage)
    },
    loadCustomComment() {
      //继续加载客户跟进
      this.commentPage = this.commentPage + 1
      this.getCustomCommentList(this.commentPage)
    },
    openPinglunForm(parentId, parentUserId) {
      this.pinglunEdit = true
      if (parentId && parentUserId) {
        this.pinglunForm = {
          parentCommentId: parentId,
          parentCommentUserId: parentUserId,
          targetId: this.customInDetail.Id,
          targetType: '商机',//评论类型   客户：customer线索：clue商机：oppprt
          content: '',//评论内容
        }
      } else {
        this.pinglunForm = {
          targetId: this.customInDetail.Id,
          targetType: '商机',//评论类型   客户：customer线索：clue商机：oppprt
          content: ''//评论内容
        }
      }
      this.resetForm("pinglunForm");

    },
    getFollowFocus() {
      this.$refs.selectFollow.blur();
      if (this.followAddForm.followUser > 0) {
        this.followAddForm.userInfo = [{ id: this.followAddForm.followUser, name: this.followAddForm.followUserName, avatar: this.followAddForm.followUserAvatar, type: "user" }];

      }
      else {
        this.followAddForm.userInfo = [];
      }

      this.$refs.followPicker.show(this.followAddForm.userInfo, "user");
    },
    selectFollower(values) {
      // console.log("选中的跟进人", values);

      this.followAddForm.userInfo = values;
      if (values.length > 0) {
        this.followAddForm.followUser = values[0].id;
        this.followAddForm.followUserName = values[0].name;
        this.followAddForm.followUserAvatar = values[0].avatar;
      }
      else {
        this.followAddForm.followUser = 0;
        this.followAddForm.followUserName = undefined;
        this.followAddForm.followUserAvatar = undefined;
      }
      this.$forceUpdate();
    },
    searchList() {
        this.hideTitle = false
      this.followAddForm.targetId = ''
      this.followAddForm.remark = ''
      this.followAddForm.followTime = ''
      this.followAddForm.followWay = ''
      this.followAddForm.followUser = ''
      this.followAddForm.followUserName = ''
    },
    inputTextareBlur(e) {
      this.hideTitle = false;
    },
    inputTextare(e) {
      // this.hideTitle=true;
    },
    //快速跟进
    handSubmit() {
      this.hideTitle = false
      this.followAddForm.targetId = ''
      this.followAddForm.remark = ''
      this.followAddForm.followTime = ''
      this.followAddForm.followWay = ''
      this.followAddForm.followUser = ''
      this.followAddForm.followUserName = ''

    },
    handEdit(item) {
      this.hideTitle = false;
      //   console.log(item,'item')
      this.followAddForm.remark = item.Remark
      this.followAddForm.followTime = item.FollowTime
      this.followAddForm.followWay = item.FollowWay
      this.followAddForm.followUser = item.FollowUser
      this.followAddForm.followUserName = this.$store.state.user.name
      this.followAddForm.targetId = item.TargetId
      this.followAddForm.Id = item.Id
    },
    handOk() {
      this.$refs.queryForm.validate(valid => {
        let data = {};
        data.followTime = this.followAddForm.followTime
        data.followUser = this.followAddForm.followUser
        data.followWay = this.followAddForm.followWay
        data.remark = this.followAddForm.remark
        data.targetType = 0
        if (this.followAddForm.targetId != '') {
          data.targetId = this.followAddForm.targetId
          data.Id = this.followAddForm.Id
          editFollow(data).then((res) => {
            if (res.code == 0) {
              this.detailsOped = false;
              this.$modal.msgSuccess("编辑成功");

            }
          })
        } else {
          data.targetId = this.detailsRow.CustomerId
          addFollow(data).then((res) => {
            if (res.code == 0) {
              this.detailsOped = false;
              this.$modal.msgSuccess("添加成功");
              this.followAddForm.targetId = ''
              this.followAddForm.remark = ''
              this.followAddForm.followTime = ''
              this.followAddForm.followWay = ''
              this.followAddForm.followUser = ''
              this.followAddForm.followUserName = ''
            }
          })
        }




      })
    },
    handCancel() {
      this.hideTitle = true
    },
    handleClickInformation(tab, event) {

    },
    //协作人与评论
    handleClick(tab, event) {
      //console.log(tab, event);
    },
    async getCustomerList() {
      //获取私海客户列表
      // this.queryParams.orgId = this.$store.getters.orgId;
      try {
        let response = await privateCustomer({ showAll: true });
        if (response.data && response.data.List) {
          // console.log("客户", response.data);
          response.data.List.map(row => {
            this.customerMap.set(row.Id, row);
          });
        }
      } catch (error) {

      }

    },
    async getContactList() {
      //获取联系人列表
      // this.queryParams.orgId = this.$store.getters.orgId;
      try {
        let response = await contactList({ showAll: true });
        if (response.data && response.data.List) {
          // console.log('联系人', response.data);
          response.data.List.map(row => {
            this.contactMap.set(row.Id, row.RealName);
          });
        }
      } catch (error) {

      }
    },
    //提交评论
    handComment() {
      // console.log(this.detailsRow, 'this.detailsRow');
      // return
      AddComment({
        targetId: this.detailsRow.CustomerId,
        content: this.textareaComment,
        targetType: '商机',
        // parentCommentUserId: parentUserId,
        // parentCommentid: parentid,
      }).then((res) => {
        // console.log(res)

      })
    },
    handleContactChange(val) {
      //完成联系人选择
      // console.log(val, 'val');
      if (val != null) {
        this.opportForm.contactId = val.Id
        this.opportForm.contactName = val.CustomerName
      }
    },
    openContactDialog() {
      //选择联系人
      this.$refs.contactOptions.openContactDialog()
    },
    handleCustomChange(val) {
      //完成客户选择
      // console.log(val, 'val');
      if (val != null) {
        this.opportForm.customerId = val.Id
        this.opportForm.customerNumber = val.CustomerNumber
        this.opportForm.customerName = val.CustomerName
      }
    },
    openCustomDialog() {
      //打开客户列表选择弹窗
      this.$refs.customOptions.openCustomDialog()
    },
    deleteProductLis(index) {
      //删除商机明细
      this.opportForm.detailList.splice(index, 1)
      this.opportForm.detailList2.splice(index, 1)
    },
    //选择商机明细
    openWupinDialog() {
      this.$refs.wupinChoice.openWupinDialog()
    },
    onWupinConfirm(selectArr) {
      this.choiceLoading = true
      if (selectArr.length > 0) {
        this.opportForm.detailList = []
        selectArr.forEach(element => {
          if (this.EditOpportId) {
            this.opportForm.detailList.push({
              label: element.ProductLabel == "F" ? "成品" : "半成品",
              productId: element.Id,
              quantity: 1,
              price: element.Price,
              opportId: this.EditOpportId,
              name: element.ProductName,
              number: element.SkuNumber
            });
          } else {
            this.opportForm.detailList.push({
              label: element.ProductLabel == "F" ? "成品" : "半成品",
              productId: element.Id,
              quantity: 1,
              price: element.Price,
              name: element.ProductName,
              number: element.SkuNumber
            });
          }

        })
        this.opportForm.detailList2 = JSON.parse(JSON.stringify(selectArr))
        // console.log("this.opportForm.detailList2", this.opportForm.detailList2);
      }
      this.choiceLoading = false
    },

    //选择商机明细
    async getPeriodList() {
      //获取销售阶段
      try {
        let res = await periodList()
        // console.log("销售阶段", res);
        if (res.data) {
          this.periodSelectlis = res.data;
          res.data.map(row => {
            this.periodTypeMap.set(row.Id, row)
          })

        }
      } catch (error) {
        console.log("销售阶段查询错误", error);
      }

    },
    async getGenerateNumber() {
      //生成商机唯一编号
      try {
        let res = await GenerateNumber();
        // console.log("生成商机编码", res);

        this.opportForm.opportNumber = res.data;
        // console.log("表单信息", this.opportForm);
      } catch (error) {
        console.log("生成编码报错", err);
      }
    },
    handleChange(item) {
      // this.periodTypeMapText=item.PeriodName
      // console.log(this.periodTypeMapText,'this.periodTypeMapText')
      this.$modal
        .confirm('确认更改销售阶段？')//'是否确认推进名称为"' + this.AdvanceRow.OpportName + '"的商机到"'+item.PeriodName+'"阶段？'
        .then(() => {
          this.loading = true;
          return opportForward({ id: this.detailsRow.Id, target: item.Id });
        })
        .then(() => {
           this.getPeriodList();
          this.loading = false;
          this.detailsOped = false
          this.$modal.msgSuccess("推进成功");
        })
        .catch(() => {
          this.AdvanceList = false
          this.loading = false;
        });
    },
    handleReceive(row) {
      this.$modal
        .confirm('是否确认推进名称为"' + row.OpportName + '"的商机？')
        .then(() => {
          this.loading = true;
          return opportForward({ id: row.Id });
        })
        .then(() => {
          this.loading = false;
          this.$modal.msgSuccess("推进成功");
          this.getOpportList();
        })
        .catch(() => {
          this.loading = false;
        });
    },
    //点击打开详情
    handleOpenDetails(row) {
     // this.$router.push({path:'/crm/opport/details',query:{id:row.Id}})
      // this.detailsRow = row;
      // this.followWayList = this.dict.type.follow_way;
      // // console.log(this.followWayList,'this.followWayList')
      // this.detailsOped = true;
      // this.hideTitle = true;

      opportInfo({ id: this.$route.query.id }).then(res => {
        // console.log(res, 'res')
        let data = res.data;
        let leaderName = ""
        let helarnameList = []

        //协作人
        if (data.Helper) {
          let arr = data.Helper.split(",");
          arr.map(it => {
            // console.log("协作人每项id", it);
            if (this.employeeMap.get(parseInt(it))) {
              helarnameList.push(this.employeeMap.get(parseInt(it)).RealName)
            }
          });
          this.helperNameText = helarnameList.join(',')
        }
        if (data.LeaderId) {
          //获取责任人相关信息
          this.leaderNameText = this.employeeMap.get(parseInt(data.LeaderId)).RealName
        }
        if (data.CustomerId) {
          this.CustomerIdText = this.customerMap.get(data.CustomerId).CustomerName
        }
        if (data.ContactId) {
          this.ContactIdText = this.contactMap.get(data.ContactId)
          //console.log(this.contactMap.get(data.ContactId))
        }
        //销售阶段
        if (data.Period) {
          if(this.periodTypeMap.get(data.Period)){
            this.periodTypeMapText = this.periodTypeMap.get(data.Period).PeriodName
          }else{
            this.periodTypeMapText=''
          }
          
          // console.log(this.periodTypeMapText,'this.periodTypeMapText')
        }
        //this.detailsRow.LeaderAvatar=this.$store.state.user.avatar
        this.detailsRow = data;
        // console.log( this.$store.state.user.avatar,'this.detailsRow.DetailList')
        this.customInDetail = data;
        if(this.isCheckPermi(['/DiscussService/Comment/List'])){
          this.getCustomCommentList(1)//获取客户评论列表
        }
        
        recordList({
          TargetId: data.CustomerId
        }).then((res) => {
          res.data.List.map(row => {
            if (row.FollowUser && this.employeeMap.get(row.FollowUser)) {
              row.FollowUserName = this.employeeMap.get(parseInt(row.FollowUser)).RealName
              row.FollowUserAvatar = this.employeeMap.get(parseInt(row.FollowUser)).Avatar
            }
            if (row.FollowWay) {
              row.FollowWayName = this.dict.getName("follow_way", row.FollowWay);
              // console.log("跟进方式",row.FollowWayName);
            }
          })
          // console.log(res, '跟进记录列表')
          this.RecordList = res.data.List
        })
      })

      //console.log(row)
    },
    //打开跟进状态
    handleOpenAdvance(row) {
      this.AdvanceList = true;
      this.AdvanceRow = row
      // console.log(row, 'row')
    },
    handleUpdate(row) {
      //修改商机信息
      if (row.Id) {
        this.EditOpportId = row.Id
        opportInfo({ id: row.Id }).then(res => {
          // console.log("点击的商机详情", res);
          if (res.data) {
            let data = res.data;
            let useList = [];
            let helperName = []
            let leaderList = [];
            let leaderName = "";
            if (data.Helper) {
              let arr = data.Helper.split(",");
              arr.map(it => {
                // console.log("协作人每项id", it);
                if (this.employeeMap.get(parseInt(it))) {
                  useList.push(this.employeeMap.get(parseInt(it)));
                  helperName.push(this.employeeMap.get(parseInt(it)).RealName)
                } else {
                  useList.push({});
                  helperName.push('')
                }
              });
            }
            if (data.LeaderId) {
              //获取责任人相关信息
              leaderList.push(this.employeeMap.get(parseInt(data.LeaderId)));
              leaderName = this.employeeMap.get(parseInt(data.LeaderId)).RealName;
            }
            let detailList = []
            let detailList2 = []
            if (data.DetailList && data.DetailList.length > 0) {
              data.DetailList.map(row => {
                let obj1 = {
                  label: row.ProductInfo.ProductLabel == "F" ? "成品" : "半成品",
                  productId: row.ProductId,
                  opportId: data.Id,
                  quantity: row.Quantity,
                  price: row.Price,
                  name: row.ProductInfo.ProductName,
                  number: row.ProductInfo.SkuNumber
                }

                detailList.push(obj1)
                detailList2.push(row.ProductInfo)
              })
            }
            this.opportForm = {
              opportNumber: data.OpportNumber, //商机编码
              opportName: data.OpportName, //商机名称
              customerId: data.CustomerId, //客户编码
              customerName: this.customerMap.get(data.CustomerId) ? this.customerMap.get(data.CustomerId).CustomerName : '', //客户编码（不传）
              customerNumber: this.customerMap.get(data.CustomerId) ? this.customerMap.get(data.CustomerId).CustomerNumber : '', //客户编码（不传）
              contactId: data.ContactId, //联系人id
              contactName: this.contactMap.get(data.ContactId) ? this.contactMap.get(data.ContactId) : '', //联系人名称（不传）
              probability: data.Probability, //预计成交几率
              period: data.Period, //销售阶段
              userInfo: useList, //已经选择的协作人员工
              helperName: helperName, //
              helper: data.Helper,
              remark: data.Remark, //线索详情
              leaderId: data.LeaderId, //负责人
              leaderName: leaderName, //负责人
              leaderInfo: leaderList, //负责人
              detailList: detailList, //商机明细
              detailList2: detailList2, //商机明细
              id: data.Id
            };
            this.open = true;
            this.diaTitle = "编辑商机";
          }
        });
      }
    },
    async getMemberList() {
      try {
        let rsp = await listMember({ deptIdWithChildren: true, showAll: true,isPrimaryDept:true });
        // console.log("员工列表", rsp);
        if (rsp.data && rsp.data.List) {
          rsp.data.List.map(row => {
            this.employeeMap.set(row.Id, row);
          });
        }
      } catch (err) {
        // console.log("报错", err);

        this.$message.error(err.data);
      }
    },
    submitForm() {
      //提交数据
      this.$refs.opportForm.validate(valid => {
        if (valid) {
          if (!this.opportForm.leaderId) {
            this.$modal.error("负责人不能为空");
            return
          }
          let submitForm = {};
          submitForm = JSON.parse(JSON.stringify(this.opportForm));
          submitForm.helperName = submitForm.helperName.join(",");
          delete submitForm.userInfo;
          delete submitForm.leaderName;
          delete submitForm.leaderInfo;
          delete submitForm.detailList2;
          delete submitForm.customerName;
          delete submitForm.contactName;
          this.loading = true;
          if (submitForm.id) {
            opportEdit(submitForm)
              .then(res => {
                // console.log(res, "编辑商机成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("编辑成功");
                  this.open = false;
                  this.getOpportList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            opportAdd(submitForm)
              .then(res => {
                // console.log(res, "添加商机成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  this.getOpportList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          }
        }
      });
    },
    //选择负责人和协作人
    getLeaderFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectLeader.blur();
      this.$refs.leaderPicker.show(this.opportForm.leaderInfo, "user");
    },
    getUsersFocus() {
      //获取协作人选择下拉列表的焦点
      this.$refs.selectUsers.blur();
      this.$refs.userPicker.show(this.opportForm.userInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      // console.log("选中的协作人", values);
      // this.$refs.userPicker.show(this.opportForm.userInfo, "user");
      // this.opportForm.userInfo = values;
      // let li = [];
      // let li2 = [];
      // values.map((it, ix) => {
      //   console.log(it.RealName);
      //   li.push(parseInt(it.Id));
      //   li2.push(it.RealName);
      //   console.log("li", li);
      // });
      // this.opportForm.helper = li.join(",");
      // this.opportForm.helperName = li2;
      // this.$forceUpdate();
      //this.$refs.userPicker.show(this.opportForm.userInfo, "user");
      this.opportForm.userInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          // console.log(it.name);
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
          // console.log("li", li);
        });
        this.opportForm.helper = li.join(",");
        this.opportForm.helperName = li2;
        this.opportForm.helperAvatar = li3;
      }
      else {
        this.opportForm.helper = undefined;
        this.opportForm.helperName = undefined;
      }
      this.$forceUpdate();
    },
    selectLeadered(values) {
      //选择负责人
      // console.log("选中的负责人", values);
      // this.$refs.leaderPicker.show(this.opportForm.leaderInfo, "user");
      // this.opportForm.leaderInfo = values;
      // let li = [];
      // let li2 = [];
      // values.map((it, ix) => {
      //   console.log(it.RealName);
      //   li.push(parseInt(it.Id));
      //   li2.push(it.RealName);
      //   console.log("li", li);
      // });
      // this.opportForm.leaderId = li[0];
      // this.opportForm.leaderName = li2[0];
      // this.$forceUpdate();


      //选择负责人
      this.opportForm.leaderInfo = values;
      if (values.length > 0) {
        this.opportForm.leaderId = values[0].id;
        this.opportForm.leaderName = values[0].name;
        this.opportForm.leaderAvatar = values[0].avatar
      }
      else {
        this.opportForm.leaderId = 0;
        this.opportForm.leaderName = undefined;
        this.opportForm.leaderAvatar = undefined;
      }
      this.$forceUpdate();
    },
    //选择负责人和协作人
    // 取消按钮
    cancel() {
      this.open = false;
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getOpportList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      //选中行的样式设置
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    handleAdd() {
      //打开添加商机
      this.opportForm = {
        opportNumber: "", //商机编码
        opportName: "", //商机名称
        customerId: "", //客户编码
        customerName: "", //客户编码（不传）
        customerNumber: "", //客户编码（不传）
        contactId: "", //联系人id
        contactName: "", //联系人名称（不传）
        probability: null, //预计成交几率
        period: "", //销售阶段
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helper: "",
        remark: "", //线索详情
        leaderId: 0, //负责人
        leaderName: [], //负责人
        leaderInfo: [], //负责人
        detailList: [], //商机明细
        detailList2: [], //商机明细
      };
      this.diaTitle = "添加商机";
      this.resetForm("opportForm");
      this.getGenerateNumber()
      this.EditOpportId = 0
      this.open = true;
    },
    getOpportList() {
      //获取公海商机列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      opportList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          // console.log("商机列表", response);
          if (response.data && response.data.List) {
            response.data.List.map(row => {
              if (row.Helper) {
                let helperArr = [];
                let arr = row.Helper.split(",");
                arr.map(it => {
                  // console.log("协作人每项id", it);

                  helperArr.push(this.employeeMap.get(parseInt(it)).RealName);
                });
                row.HelperName = helperArr.join(",");
              }
              if (row.LeaderId) {
                //负责人姓名获取
                row.LeaderName = this.employeeMap.get(
                  parseInt(row.LeaderId)
                ).RealName;
              }
            });
            this.priList = response.data.List;
          }

          // for(let i=0;i<3;i++){
          //   this.agentList=[...this.agentList,...this.agentList]
          // }
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    }
  }
};
</script>
<style lang="scss">
#opport-details{
  
}
.opport-details-par{
  background: #fff;
  margin:20px;
  padding:10px 20px;
}
.basic_info {
  width: 100%;
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;
  flex-wrap: wrap;
  font-size: 16px;

  .row_cot {
    padding: 10px 0 0 30px;
    width: 33%;
    height: 150px;
    box-sizing: border-box;

    .cot_label {
      color: #78829D;
      margin-bottom: 13px;
    }

    .cot_value {
      color: #333333;

      &.blue {
        color: #3572FF;
      }
    }

    &.frist {
      padding-left: 0;
      width: 230px;
    }

    &.third {
      width: 198px;
    }

    &.line_row {
      width: 668px;
      padding-left: 0;
    }
  }
}

.active {
  background: #0db3a6;
  color: #fff;
  
}

.on {
  background: #fff;
  color: #333;
  
}

.jianjie {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: calc(100% - 60px);
  //position: absolute;
  padding: 10px 0px;

  .jianjie_con {
    display: flex;
    justify-content: flex-start;
    align-items: center;
  }

  .button_con {
    .blue_btn {

      background: linear-gradient(90deg, #4C79FF 0%, #6DA8FF 100%);
    }
  }

  .jianjie_li {
    justify-content: flex-start;
    align-items: center;
    margin-right: 30px;
    font-size: 16px;
    display: flex;

    .label {
      color: #78829D;
    }

    .value {
      color: #333333;

      &.haiImg {
        display: flex;
        justify-content: flex-start;
        align-items: center;
      }

      .image {
        width: 36px;
        height: 36px;
        margin-right: 12px;
      }
    }
  }
}

.el-dialog__body {
  padding-top: 0px;
}

.custom_title {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  margin-top: -15px;

  .tag {
    width: 48px;
    height: 24px;
    border-radius: 4px;
    background: #F6F9FF;
    color: #3572FF;
    font-size: 14px;
    text-align: center;
    line-height: 24px;
  }

  .name {
    margin-left: 10px;
    font-size: 20px;
    color: #333333;
  }
}

.comment_list {
  width: 100%;

  .comment_li {
    padding: 16px;
    background-color: #ffffff;
    margin-top: 10px;

    .title {
      height: 24px;
      display: flex;
      justify-content: flex-start;
      align-items: center;

      .avatar {
        width: 24px;
        height: 24px;
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: #5D9CEE;
        color: #ffffff;
        border-radius: 50%;
        font-size: 10px;

        img {
          width: 24px;
          height: 24px;
          border-radius: 50%;
        }
      }

      .text_name {
        font-size: 16px;
        color: #333333;
        margin-left: 8px;
      }
    }

    .content {
      margin-top: 16px;
    }

    .time_btn {
      height: 14px;
      font-size: 14px;
      color: #78829D;
      display: flex;
      justify-content: space-between;
      align-items: center;

      .return_btn {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        cursor: pointer;

        .text {
          margin-left: 2px;
        }
      }
    }

    .return_comment {
      padding-top: 24px;

      .return_li {
        padding-bottom: 24px;
        padding-left: 8px;

        .avatar_name {
          height: 24px;
          display: flex;
          justify-content: flex-start;
          align-items: flex-start;

          .avatar {
            width: 24px;
            height: 24px;
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: #5D9CEE;
            color: #ffffff;
            border-radius: 50%;
            font-size: 10px;

            img {
              width: 24px;
              height: 24px;
              border-radius: 50%;
            }
          }

          .text_name {
            font-size: 14px;
            color: #333333;
            margin-left: 8px;
          }
        }

        .return_name {
          margin-top: 5px;
          font-size: 16px;
          color: #333333;

          .name {
            color: #78829D;
          }
        }
      }
    }
  }

  .comment_li:first-child {
    margin-top: 0;
  }
}

.avatar {
  width: 24px;
  height: 24px;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #5D9CEE;
  color: #ffffff;
  border-radius: 50%;
  font-size: 10px;

  img {
    width: 24px;
    height: 24px;
    border-radius: 50%;
  }
}

.text_name {
  margin-left: 10px;
  color: #333333;
  font-size: 14px;
  margin-top: 3px;
}

.recordParse {
  display: flex;
  flex-wrap: wrap;
  margin: 15px 0px;
  height: 100%;
  overflow: scroll;

}

::-webkit-scrollbar {
  display: none;
  /* Chrome Safari */
}

.recordList {
  display: flex;
  width: 100%;
  flex-direction: column;
  border: 1px solid #f5f5f5;
  padding: 10px;
  border-radius: 8px;
  margin-right: 10px;
  margin-bottom: 10px;

}

.recordList>p {
  line-height: 0px;
  font-size: 12px;
}

.recordList-button {
  height: 20px;
}

.FollowTime {
  margin-left: 30px;
}

.el-tabs__nav-wrap::after {
  height: 0px;
}

.detailsOpedRow>el-col {
  padding-bottom: 120px;
}

.contactsEl {
  padding-top: 1px;
  text-align: left;
  padding-bottom: 30px;
  font-size: 16px;

  .contactsEl-name {
    color: #78829D;
    width: 100%;
  }

  .contactsElText {
    color: #333333;
    margin-top: 14px;
  }
}

.AdvanceList {
  display: flex;

  // justify-content: space-between;
  .AdvanceList-item {
    // width: 100px;
    background: #ecf5ff;
    // color:#409eff;
    // padding:10px;
    font-size: 12px;
    // border-radius: 5px;

  }

  .active1,
  .active2,
  .active3 {
    width: 150px;
    height: 40px;
    line-height: 40px;
    text-align: center;
    background: url('../../../assets/images/selected.png') no-repeat;
    background-size: 100% 100%;
    color: #fff
  }

  .active2 {
    background: url('../../../assets/images/notSelect.png') no-repeat;
    background-size: 100% 100%;
    color: #333;
  }

  .active3 {
    width: 120px;
    background: url('../../../assets/images/WinOrder.png') no-repeat;
    background-size: 100% 100%;
    color: #333;
  }

  .active3Select {
    width: 120px;
    background: url('../../../assets/images/winOrderSelect.png') no-repeat;
    background-size: 100% 100%;
    color: #fff;
    text-align: center;
    line-height: 40px;
    padding-left: 15px;
  }

  .active4,
  .active5 {
    width: 80px;
    height: 40px;
    line-height: 40px;
    text-align: center;
    color: #FF3535;
    background: #FFEAEA;
    border-radius: 6px;
    margin-left: 15px;
  }

  .active5 {
    color: #78829D;
    background: #F9FAFC;
  }
}

.items-title {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  height: 48px;
  background-color: rgb(249, 250, 252);
  padding: 0 15px;
  margin-bottom: 5px;
}

.add_dialog_border.opport_add {
  .el-dialog {
    margin-top: 9vh;

    // .add_clue_form {
    //   max-height: 500px;
    //   //   overflow-y: scroll;
    //   overflow-y: auto;
    //   overflow-x: hidden;

    //   &::-webkit-scrollbar {
    //     width: 0;
    //   }

    //   // 滚动条里面默认的小方块,自定义样式
    //   &::-webkit-scrollbar-thumb {
    //     background: #8798af;
    //     border-radius: 2px;
    //   }

    //   // 滚动条里面的轨道
    //   &::-webkit-scrollbar-track {
    //     background: transparent;
    //   }
    // }
  }
}

// .add_dialog_border.opport_add .el-dialog {
//   max-height: 600px !important;
//   overflow: scroll !important;
// }

.add_clue_form {
  .el-form-item {
    margin-bottom: 12px;
  }

  .el-form-item__label {
    padding-bottom: 0;
  }

  .form_input_style {
    height: 42px;
    line-height: 42px;
  }

  .form_input_style input {
    height: 38px;
    line-height: 38px;
  }
}</style>
