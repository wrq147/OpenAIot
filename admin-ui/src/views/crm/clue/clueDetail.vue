<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con" v-loading="loading">
        <div class="custom_info_cot elbiaoge_elform">
            <div class="jianjie">
                <div class="jianjie_con">
                    <div class="jianjie_li">
                        <div class="label">联系人：</div>
                        <div class="value">{{ clueInDetail.RealName }}</div>
                    </div>
                    <div class="jianjie_li">
                        <div class="label">手机号：</div>
                        <div class="value">{{ clueInDetail.Mobile }}</div>
                    </div>
                    <div class="jianjie_li">
                        <div class="label">负责人：</div>
                        <div class="value haiImg">
                            <img class="image" :src="clueInDetail.LeaderAvatar" alt="">
                            <span>{{ clueInDetail.LeaderName }}</span>
                        </div>
                    </div>
                </div>
                <div class="button_con">
                    <el-button @click="handleUpdate({ Id: clueInDetail.Id })">编辑</el-button>
                    <el-button type="primary" @click="handleTransform(clueInDetail.Id)" class="blue_btn">转换</el-button>
                </div>
            </div>
            <div class="zhanweifu"></div>
            <div class="cot_body">
                <div class="body_left" id="left_content" ref="leftContent">
                    <div class="tabs_cont">
                        <el-tabs v-model="activeTabs" @tab-click="handleClick">
                            <el-tab-pane label="基本信息" name="first"></el-tab-pane>
                        </el-tabs>
                    </div>
                    <div class="basic_info" v-if="activeTabs == 'first'">
                        <div class="row_cot frist">
                            <div class="cot_label">联系人</div>
                            <div class="cot_value">{{ clueInDetail.RealName }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">部门</div>
                            <div class="cot_value">{{ clueInDetail.DeptName }}</div>
                        </div>
                        <div class="row_cot third">
                            <div class="cot_label">职务</div>
                            <div class="cot_value blue">{{ clueInDetail.PostName }}</div>
                        </div>
                        <div class="row_cot frist">
                            <div class="cot_label">线索来源</div>
                            <div class="cot_value blue">{{ fromMap.get(clueInDetail.FromType) }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">客户名称</div>
                            <div class="cot_value">{{ clueInDetail.CompanyName }}</div>
                        </div>
                        <div class="row_cot frist">
                            <div class="cot_label">负责人</div>
                            <div class="cot_value">{{ clueInDetail.LeaderName }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">协作人</div>
                            <div class="cot_value">{{ clueInDetail.HelperName }}</div>
                        </div>
                        <div class="row_cot third">
                            <div class="cot_label">线索状态</div>
                            <div class="cot_value">{{ clueInDetail.del_flag == 1 ? '被转换' : (clueInDetail.del_flag == 2 ?
                                '删除' :
                                '存在') }}</div>
                        </div>
                        <div class="row_cot line_row">
                            <div class="cot_label">客户详情</div>
                            <div class="cot_value" v-if="clueInDetail.Remark">{{ clueInDetail.Remark }}</div>
                        </div>
                    </div>
                </div>
                <div class="body_right">
                    <div class="right_tabs">
                        <div class="tabs_li" :class="flowActiveTabs == 0 ? 'active' : ''" @click.stop="switchTabs(0)">快速跟进
                        </div>
                        <div v-if="isCheckPermi(['/DiscussService/Comment/List'])" class="tabs_li" :class="flowActiveTabs == 1 ? 'active' : ''" @click.stop="switchTabs(1)">评论
                        </div>
                    </div>
                    <div class="ab_zhanwei"></div>
                    <div class="follow_up" v-if="flowActiveTabs == 0">
                        <div class="follow_up_list" v-if="activeFollow == 'list'">
                            <div class="tianxie">
                                <el-input type="textarea" :rows="3" placeholder="请输入内容" v-model="followCont"
                                    @focus="openFollowForm"></el-input>
                            </div>
                            <div class="follow_list" v-infinite-scroll="loadCustomFollow" style="overflow:auto"
                                v-if="clueFollowList.length > 0">
                                <div class="follow_li" v-for="row in clueFollowList" :key="row.Id">
                                    <div class="title">
                                        <div class="left">
                                            <div class="avatar" v-if="!row.FollowUserAvatar">
                                                {{ row.FollowUserName.slice(row.FollowUserName.length - 1) }}</div>
                                            <div class="avatar" v-if="row.FollowUserAvatar">
                                                <img :src="row.FollowUserAvatar" alt="">
                                            </div>
                                            <div class="text_name">{{ row.FollowUserName }}</div>
                                        </div>
                                    </div>
                                    <div class="content" v-html="row.Remark"></div>
                                    <div class="li_li" v-if="row.OpportName">跟进商机：<span class="blue">{{ row.OpportName
                                    }}</span></div>
                                    <div class="li_li" v-if="row.RealName">联系人：<span class="blue">{{ row.RealName }}</span>
                                    </div>
                                    <div class="li_li">跟进方式：{{ row.FollowWayName }}</div>
                                    <div class="li_li">跟进时间：{{ row.createTime }}</div>
                                </div>
                            </div>
                            <el-empty description="暂无跟进记录" v-else :image-size="200"></el-empty>
                        </div>
                        <div class="follow_add" v-if="activeFollow == 'add'">
                            <el-form class="follow_add_form" :model="followAddForm" ref="followAddForm" :inline="true"
                                :rules="followAddRules">
                                <el-form-item label="跟进内容" prop="remark" style="width: 100%;">
                                    <el-input type="textarea" ref="followInput" v-model="followAddForm.remark"
                                        placeholder="请输入跟进内容" :autosize="{ minRows: 3, maxRows: 5 }"></el-input>
                                </el-form-item>
                                <el-form-item label="跟进时间" prop="followTime" style="width: 100%;">
                                    <el-date-picker v-model="followAddForm.followTime" type="datetime" placeholder="选择日期时间"
                                        style="width:100%">
                                    </el-date-picker>
                                </el-form-item>
                                <el-form-item label="跟进方式" prop="followWay" style="width: 100%;">
                                    <el-select v-model="followAddForm.followWay" placeholder="请选择" style="width:100%">
                                        <el-option :label="item.label" :value="item.value" v-for="item in followWayList"
                                            :key="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                                <el-form-item label="跟进人" prop="followUser" style="width: 100%;">
                                    <el-select class="form_input_style" filterable allow-create default-first-option
                                        v-model="followAddForm.followUserName" ref="selectFollow" placeholder="请选择跟进人"
                                        @focus="getFollowFocus" style="width:100%"></el-select>
                                    <org-picker :multiple="false" ref="followPicker"
                                        :selected="followAddForm.followUserInfo" @ok="selectFollower" />
                                </el-form-item>
                                <div style="margin-top: 20px;">
                                    <el-button type="primary" @click="submitFollow">确 定</el-button>
                                    <el-button @click="closeFollowForm">取 消</el-button>
                                </div>
                            </el-form>
                        </div>
                    </div>
                    <div class="pinglun_con" v-if="flowActiveTabs == 1">
                        <div class="comment_list" v-infinite-scroll="loadClueComment" style="overflow:auto"
                            v-if="Object.keys(clueCommentObj).length > 0">
                            <div class="comment_li" v-for="(row, keys) in clueCommentObj" :key="row.Id">
                                <div class="title">
                                    <div class="avatar" v-if="!row.UserAvatar && row.UserName">
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
                                <div class="return_comment" v-if="row.returnList && row.returnList.length > 0">
                                    <div>
                                        <div class="return_li" v-for="(item, inx) in row.returnList" :key="item.Id"
                                            v-if="inx < row.curExpendNum">
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
                                        <div class="zhankai" @click="expandMore(keys, row.returnList.length)"
                                            v-if="row.returnList.length > row.curExpendNum">
                                            <div class="line"></div>
                                            <div class="zhankai_text">查看更多</div>
                                            <div class="icon">
                                                <i class="el-icon-arrow-down" size="14" color="#78829D"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <el-empty description="暂无评论" v-else :image-size="200"></el-empty>
                        <div class="add_btn">
                            <el-button class="add_pinglun" @click="openPinglunForm">添加评论</el-button>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <customer-dialog ref="customAdd" @errLoading="errLoading" @finishLoading="finishLoading"></customer-dialog>
        <el-dialog title="转换" class="custom_list_dialog" :visible.sync="customListDialog" width="1000px"
            @close="customListDialog = false" top="8vh">
            <div class="custom_list_con">
                <div class="custom_search">
                    <el-input placeholder="请输入内容" prefix-icon="el-icon-search" v-model="customQuery.key"
                        style="width:232px"></el-input>
                    <el-button type="primary" plain @click="handleAddNewCustom" v-hasPermi="['/CRMService/Customer/Add']">
                        <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                        <span style="margin-left:6px">转换新建客户</span>
                    </el-button>
                </div>
                <el-table v-loading="loading" :data="priCustomerList" :row-style="isRed"
                    @selection-change="customSelectionChange" class="data_table" :header-cell-style="cellSty"
                    style="width:100%;" max-height="500">
                    <el-table-column type="selection" width="50" align="center" />
                    <el-table-column label="企业ID" align="center" key="OrgId" prop="OrgId" />
                    <el-table-column label="客户名称" align="center" key="CustomerName" prop="CustomerName"
                        :show-overflow-tooltip="true">
                        <template slot-scope="scope">
                            <p style="white-space: pre-line !important;">{{ scope.row.CustomerName }}</p>
                        </template>
                    </el-table-column>
                    <el-table-column label="客户类型" align="center" key="CustomerType" prop="CustomerType"
                        :show-overflow-tooltip="true">
                        <template slot-scope="scope">
                            <span>{{ scope.row.CustomerType == 0 ? '代理' : '直销' }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="行业类型" align="center" key="IndustryName" prop="IndustryName"
                        :show-overflow-tooltip="true" />
                    <el-table-column label="公司电话" align="center" key="CompanyTel" prop="CompanyTel" />
                    <el-table-column label="公司网址" align="center" key="CompanyUrl" prop="CompanyUrl" />
                    <el-table-column label="公司地址" align="center" key="AddressName" prop="AddressName">
                        <template slot-scope="scope">
                            <p>{{ scope.row.AddressName + scope.row.AddressDetail }}</p>
                        </template>
                    </el-table-column>
                    <el-table-column label="负责人" align="center" key="LeaderName" prop="LeaderName" />
                    <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName">
                        <template slot-scope="scope">
                            <p>{{ scope.row.HelperName }}</p>
                        </template>
                    </el-table-column>
                    <el-table-column label="线索来源" align="center" key="FromType" prop="FromType">
                        <template slot-scope="scope" v-if="scope.row.FromType">
                            <span>{{ fromMap.get(scope.row.FromType) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="创建时间" align="center" prop="createTime" width="240">
                        <template slot-scope="scope">
                            <span>{{ parseTime(scope.row.createTime) }}</span>
                        </template>
                    </el-table-column>
                </el-table>
                <div class="synchronous_pages">
                    <el-checkbox v-model="isSynchronous">同步跟进记录(将线索的跟进记录同步显示在客户详情页的侧栏中)</el-checkbox>
                    <pagination v-show="total > 0" :total="total" :page.sync="customQuery.pageNum"
                        :limit.sync="customQuery.pageSize" @pagination="getCustomerList" />
                </div>
            </div>
            <div slot="footer" class="custom_dialog_footer">
                <el-button class="finish_tranform" type="primary" @click="finishTranform(selectCustomer, true)"
                    :disabled="customerSinger">确 定</el-button>
            </div>
        </el-dialog>
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
        <clue-add ref="clueAdd" @errLoading="clueAddErrLoading" @finishLoading="clueAddfinishLoading"></clue-add>
    </div>
</template>

<script>
import ClueAdd from "@/views/crm/compontent/clue-add";
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
import { listMember } from "@/api/system/Employee";
import {
    followList,
    addFollow,
    editFollow,
} from "@/api/crm/follow";
import { commentAddClue, commentList } from '@/api/crm/comment'
import {
    clueInfo,
    priTransformClue
} from "@/api/crm/clue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
    recordList,
} from "@/api/crm/opport";
import { privateCustomer } from "@/api/crm/customer";
import CustomerDialog from "@/views/crm/compontent/customer_dialog";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import { checkPermi } from "@/utils/permission"; 
export default {
    name: 'AdminUiClueDetail',
    components: { OrgPicker,TextEditor, ClueAdd, CustomerDialog },
    mixins: [resizeTableCon],
    dicts: ["follow_way"],
    data() {
        return {
            total: 0,//客户总数
            loading: true,
            isSynchronous: false, //是同步跟进记录
            customQuery: {
                pageNum: 1,
                pageSize: 10,
                Belong: 2, //为1表示公海，为2表示私海，其它为全部（不用传）
                key: ""
            },
            addCustomDia: false, //是否打开新建客户
            diaTitle: "添加客户",
            //跟进记录相关参数
            followCont: "", //跟进内容
            followSearch: "", //跟进搜索参数
            //跟进记录相关参数
            activeTabs: "first", //活动的线索详情的切换
            fromList: [
                {
                    value: "weixin",
                    label: "微信线索"
                },
                {
                    value: "form",
                    label: "流程表单"
                },
                {
                    value: "other",
                    label: "其他"
                }
            ],
            fromMap: new Map(), //线索来源map
            employeeMap: new Map(), //员工map
            clueInDetail: {},//线索详情
            flowActiveTabs: 0,//快速跟进和评论的切换
            selectCustomer: [], //已有的客户中选中的客户
            priCustomerList: [],//私有线索列表
            customListDialog: false,//转换时客户列表
            customerSinger: true, //客户是否只选择了一行
            FolloMethod: [
                {
                    label: '电话拜访',

                },
                {
                    label: '现场拜访',

                }
            ],
            value: '',
            value1: '',//跟进时间
            RecordList: [],//快速跟进数组
            textarea: '',
            followAddForm: {
                targetType: 1,//0是客户，1是线索
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
            detailsRow: [],
            hideTitle: true,
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

            //跟进评论相关参数
            activeFollow: 'list',//展示的是跟进列表还是跟进添加
            leftContentHei: 0,
            clueFollowList: [],//线索跟进记录
            followPage: 1,//跟进记录当前是那一页
            commentPage: 1,//评论当前页
            clueCommentList: [],//评论列表
            clueCommentObj: [],//评论
            //跟进评论相关参数
            pinglunEdit: false,
            pinglunForm: {
                targetId: 0,
                targetType: null,
                parentCommentId: 0,
                parentCommentUserId: 0,
                content: ''//评论内容
            },
            pinglunRules: {
                content: [
                    {
                        required: true,
                        message: "回复内容不能为空",
                        trigger: ["blur"]
                    }
                ],
            },
            id: '',//线索id
            ids: []
        };
    },

    async mounted() {
        await this.getMemberList();
        this.setFromMap();
        let pars = this.$route.query;
        console.log("参数", pars);
        if (pars.id) {
            this.id = pars.id;
            this.viewInfo(pars.id)
        }
        this.getCustomerList()
    },

    methods: {
        isCheckPermi(val) {//权限判断
            return checkPermi(val)
        },
        handleUpdate(row) {
            //修改线索信息
            if (row.Id) {
                this.$refs.clueAdd.handleUpdate(row)
            }
        },
        clueAddErrLoading() {
            //线索添加失败
            this.loading = false;
        },
        clueAddfinishLoading() {
            //线索添加成功
            this.loading = false;
            this.$refs.clueAdd.cancel()//关闭弹窗
            this.viewInfo(this.id);
        },
        handleTransform(id) {
            //转换线索
            this.customListDialog = true;
        },
        getCustomerList() {
            //获取私海客户列表
            this.loading = true;
            // this.queryParams.orgId = this.$store.getters.orgId;
            privateCustomer(this.customQuery).then(
                response => {
                    console.log("客户列表", response);
                    if (response.data && response.data.List) {
                        response.data.List.map(async row => {
                            row.IndustryName = "";
                            if (row.Industry) {
                                //行业类型
                                row.IndustryName = await this.$store.dispatch(
                                    "datas/industryName",
                                    row.Industry
                                );
                            }
                        });
                        this.priCustomerList = response.data.List;
                    }

                    // for(let i=0;i<3;i++){
                    //   this.agentList=[...this.agentList,...this.agentList]
                    // }
                    this.total = response.data.Total;
                    this.loading = false;
                }
            );
        },
        setFromMap() {
            //设置线索来源map
            this.fromList.map(row => {
                this.fromMap.set(row.value, row.label);
            });
        },
        async handleAddNewCustom() {
            //打开添加客户
            this.$refs.customAdd.setEmployeeMap(this.employeeMap)
            this.$refs.customAdd.handleAdd()
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
                this.$refs.clueAdd.setEmployeeMap(this.employeeMap)
            } catch (err) {
                console.log("报错", err);

                this.$message.error(err.data);
            }
        },
        switchTabs(val) {
            //线索详情的弹窗
            this.flowActiveTabs = val;
        },
        handleClick(tab, event) {
            //线索详情点击tabs切换
            console.log(tab, event);
        },
        viewInfo(val) {
            this.followWayList = this.dict.type.follow_way;

            //线索详情
            // console.log("选中的数据", this.ids);
            if (val) {
                clueInfo({ id: val }).then(async res => {
                    console.log("点击的线索详情", res);
                    this.detailsRow = res.data
                    this.clueInDetail = res.data
                    if (res.data) {
                        recordList({
                            TargetId: res.data.Id
                        }).then((data) => {
                            data.data.List.map(row => {
                                // if (row.FollowUser && this.employeeMap.get(row.FollowUser)) {
                                //     row.FollowUserName = this.employeeMap.get(parseInt(row.FollowUser)).RealName
                                //     row.FollowUserAvatar = this.employeeMap.get(parseInt(row.FollowUser)).Avatar
                                // }
                                if (row.FollowWay) {
                                    row.FollowWayName = this.dict.getName("follow_way", row.FollowWay);
                                    // console.log("跟进方式",row.FollowWayName);
                                }
                            })
                            // console.log(data, '跟进记录列表')
                            this.RecordList = data.data.List

                        })
                        let data = res.data;
                        let useList = [];
                        if (data.Helper) {
                            let arr = data.Helper.split(",");
                            arr.map(it => {
                                // console.log("协作人每项id", it);

                                useList.push(this.employeeMap.get(parseInt(it)));
                            });
                        }
                        if (data.LeaderId) {
                            res.data.LeaderName = this.employeeMap.get(
                                parseInt(data.LeaderId)
                            ).RealName;
                            res.data.LeaderAvatar = this.employeeMap.get(
                                parseInt(data.LeaderId)
                            ).Avatar;
                        }
                        this.clueInDetail = res.data;
                        await this.getClueFollowList(data.Id, 1)//获取跟进记录列表
                        if(this.isCheckPermi(['/DiscussService/Comment/List'])){
                            this.getClueCommentList(1)//获取线索评论列表
                        }
                        
                        this.infoVisible = true; //数据赋值完后再打开弹出层
                        this.$nextTick(() => {
                            this.leftContentHei = this.$refs.leftContent.offsetHeight;
                            // console.log("获取左边元素的高度", this.leftContentHei);
                        })
                    }
                });
            }

        },
        errLoading() {
            //访问接口失败
            // this.loading = false;
        },
        finishLoading(query) {
            //添加客户成功
            // this.loading = false;
            this.addCustomDia = false;
            // console.log("传的参数", query);
            this.finishTranform(query, false)
        },
        submitFollow() {
            //提交指定客户的跟进记录
            this.$refs.followAddForm.validate(valid => {
                if (valid) {
                    let submitForm = {};
                    submitForm = JSON.parse(JSON.stringify(this.followAddForm));
                    delete submitForm.userInfo;
                    delete submitForm.targetName;
                    delete submitForm.followUserName;
                    delete submitForm.followUserAvatar
                    this.loading = true;
                    if (submitForm.id) {
                        editFollow(submitForm)
                            .then(async res => {
                                // console.log(res, "修改跟进记录成功");
                                if (res.code == 0) {
                                    this.loading = false;
                                    this.$modal.msgSuccess("修改成功");
                                    this.closeFollowForm()
                                    await this.getClueFollowList(this.clueInDetail.Id, 1)//获取跟进记录列表
                                }
                            })
                            .catch(err => {
                                console.log("err", err);
                                this.loading = false;
                            });
                    } else {
                        addFollow(submitForm)
                            .then(async res => {
                                // console.log(res, "添加跟进记录成功");
                                if (res.code == 0) {
                                    this.loading = false;
                                    this.$modal.msgSuccess("添加成功");
                                    this.closeFollowForm()
                                    await this.getClueFollowList(this.clueInDetail.Id, 1)//获取跟进记录列表
                                }
                            })
                            .catch(err => {
                                console.log("err", err);
                                this.loading = false;
                            });
                    }
                }
            })
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
                            this.getClueCommentList(1)
                        }
                    })
                }
            })
        },
        editorInput(value) {
            //富文本输入赋值
            this.pinglunForm.content = value
        },
        getClueCommentList(page) {
            //获取客户评论列表
            if (page == 1) {
                this.clueCommentList = []
                this.clueCommentObj = {}
            }
            this.commentPage = page
            commentList({ TargetType: "线索", TargetId: this.clueInDetail.Id, pageNum: page, pageSize: 10, }).then(res => {
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
                        let objKeys = Object.keys(this.clueCommentObj)
                        // console.log("评论键名", objKeys);
                        if (row.ParentCommentId && row.ParentCommentId != 0) {
                            if (objKeys.includes(row.ParentCommentId)) {
                                if (this.clueCommentObj[row.ParentCommentId].returnList && this.clueCommentObj[row.ParentCommentId].returnList.length > 0) {
                                    this.clueCommentObj[row.ParentCommentId].returnList.push(row)
                                } else {
                                    this.clueCommentObj[row.ParentCommentId].returnList = []
                                    this.clueCommentObj[row.ParentCommentId].returnList.push(row)
                                }
                            } else {
                                this.clueCommentObj[row.ParentCommentId] = {}
                                if (this.clueCommentObj[row.ParentCommentId].returnList && this.clueCommentObj[row.ParentCommentId].returnList.length > 0) {
                                    this.clueCommentObj[row.ParentCommentId].returnList.push(row)
                                } else {
                                    this.clueCommentObj[row.ParentCommentId].returnList = []
                                    this.clueCommentObj[row.ParentCommentId].returnList.push(row)
                                }
                            }
                        } else {
                            row.curExpendNum = 2//当前展开的评论的数量
                            if (this.clueCommentObj[row.Id]) {
                                this.clueCommentObj[row.Id] = { ...this.clueCommentObj[row.Id], ...row }
                            } else {
                                this.clueCommentObj[row.Id] = row
                            }
                        }
                    })
                    this.clueCommentList = [...this.clueCommentList, ...res.data.List]
                    console.log(this.clueCommentObj, '评论');
                    this.clueCommentObj = JSON.parse(JSON.stringify(this.clueCommentObj))
                    this.$forceUpdate
                }
            })

        },
        async getClueFollowList(TargetId, page) {
            //获取客户跟进记录
            try {
                if (page == 1) {
                    this.clueFollowList = []
                }
                this.followPage = page
                let res = await followList({ TargetType: '线索', TargetId: TargetId, pageNum: page, pageSize: 10, });
                // console.log("跟进动态列表", res);
                if (res.data && res.data.List && res.data.List.length > 0) {
                    res.data.List.map(row => {
                        row.FollowUserName=row.FollowUserInfo.RealName
                        row.FollowUserAvatar=row.FollowUserInfo.Avatar
                        if (row.FollowWay) {
                            row.FollowWayName = this.dict.getName("follow_way", row.FollowWay);
                            // console.log("跟进方式",row.FollowWayName);
                        }
                    })
                    this.clueFollowList = [...this.clueFollowList, ...res.data.List]
                }
            } catch (error) {
            }
        },
        loadCustomFollow() {
            //继续加载客户跟进
            this.followPage = this.followPage + 1
            this.getClueFollowList(this.clueInDetail.Id, this.followPage)
        },
        loadClueComment() {
            //继续加载客户跟进
            this.commentPage = this.commentPage + 1
            this.getClueCommentList(this.commentPage)
        },
        expandMore(key, length) {
            //展开更多评论
            if (this.clueCommentObj[key].curExpendNum < length) {
                if (length - this.clueCommentObj[key].curExpendNum > 5) {
                    this.clueCommentObj[key].curExpendNum = this.clueCommentObj[key].curExpendNum + 5
                } else {
                    let rence = length - this.clueCommentObj[key].curExpendNum
                    this.clueCommentObj[key].curExpendNum = this.clueCommentObj[key].curExpendNum + rence
                }
            }
        },
        openPinglunForm(parentId, parentUserId) {
            this.pinglunEdit = true
            if (parentId && parentUserId) {
                this.pinglunForm = {
                    parentCommentId: parentId,
                    parentCommentUserId: parentUserId,
                    targetId: this.clueInDetail.Id,
                    targetType: '线索',//评论类型   客户：customer线索：clue商机：oppprt
                    content: '',//评论内容
                }
            } else {
                this.pinglunForm = {
                    targetId: this.clueInDetail.Id,
                    targetType: '线索',//评论类型   客户：customer线索：clue商机：oppprt
                    content: ''//评论内容
                }
            }
            this.resetForm("pinglunForm");

        },
        openFollowForm() {
            //打开跟进记录添加表单
            this.resetForm("followAddForm");
            this.followAddForm = {
                targetType: 1,//0是客户，1是线索
                targetId: this.clueInDetail.Id,
                targetName: this.clueInDetail.RealName,
                followTime: null,
                userInfo: [{ id: parseInt(this.$store.state.user.uid), name: this.$store.state.user.name, avatar: this.$store.state.user.avatar }], //已经选择的跟进人
                followUserName: this.$store.state.user.name, //
                followUserAvatar: this.$store.state.user.avatar,
                followUser: parseInt(this.$store.state.user.uid),//跟进人
                followWay: '',//跟进方式
                remark: "" //跟进记录详情
            }
            this.activeFollow = 'add'
            this.$nextTick(() => {
                this.$refs.followInput.focus()
            })
        },
        closeFollowForm() {
            //关闭跟进记录添加表单
            this.activeFollow = 'list'
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
        finishTranform(form, isRow) {
            let id = 0;
            if (isRow) {
                id = form[0].Id;
            } else {
                id = form;
            }
            this.$modal
                .confirm("是否确认将该线索转换为客户？")
                .then(() => {
                    this.loading = true;
                    return priTransformClue({
                        clueId: parseInt(this.clueInDetail.Id),
                        syncFollow: this.isSynchronous,
                        customer: { id }
                    });
                })
                .then(() => {
                    this.$modal.msgSuccess("转换成功");
                    this.customListDialog = false
                    this.infoVisible = false
                    this.getCustomerList();
                    this.loading = false;
                })
                .catch(() => {
                    this.loading = false;
                });
        },
        // 多选框选中数据
        customSelectionChange(selection) {
            this.selectCustomer = JSON.parse(JSON.stringify(selection));
            this.customerSinger = selection.length != 1;
            this.ids = selection.map(item => item.Id);
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
    },
};
</script>

<style lang="scss" scoped>
.custom_list_dialog {

  ::v-deep .el-dialog__body{
    padding-left: 30px;
    padding-right: 30px;
    }

  .custom_list_con {
    // height: 600px;

    .custom_search {
      width: 100%;
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 48px;
    }
  }

  .synchronous_pages {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 20px;

    .pagination-container {
      position: relative;
      height: inherit;

      .el-pagination {
        position: relative;
      }
    }
  }

  .custom_dialog_footer {
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;

    .finish_tranform {
      width: 280px;
      height: 40px;
      background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);

      &.is-disabled {
        opacity: 0.6;
      }
    }
  }
}
.custom_info_cot {
//   max-height: 600px;
  height:100%;
  overflow-y: auto;
  width: 100%;
  overflow-x: hidden;

  ::v-deep ::-webkit-scrollbar {
    width: 0;
  }

  // 滚动条里面默认的小方块,自定义样式
  ::v-deep ::-webkit-scrollbar-thumb {
    background: #8798af;
    border-radius: 2px;
  }

  // 滚动条里面的轨道
  ::v-deep ::-webkit-scrollbar-track {
    background: transparent;
  }

  .jianjie {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: calc(100% - 60px);
    padding-right: 20px;
    box-sizing: border-box;
    position: absolute;

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

  .zhanweifu {
    width: 100%;
    height: 36px;
  }

  .cot_body {
    width: 100%;
    margin-top: 20px;
    display: flex;
    justify-content: space-between;
    height: calc(100% - 60px);
    
    .body_left {
      display: flex;
      justify-content: flex-start;
      flex-direction: column;
      align-items: flex-start;

      .tabs_cont {
        width: 100%;
        height: 29px;
        margin-top: 12px;
        font-size: 16px;
        border-bottom: 1px solid #F6F7FA;
        margin-bottom: 20px;

        // line-height: 90px;
        ::v-deep .el-tabs__header {
          margin-bottom: 0;

          .el-tabs__nav-wrap::after {
            height: 0;
          }
        }

        ::v-deep .el-tabs__nav-wrap::after {
          background: inherit;
        }

        ::v-deep .el-tabs .el-tabs__nav {
          height: 29px;
        }

        ::v-deep .el-tabs__nav {
          .el-tabs__item {
            height: inherit;
            line-height: inherit;
            font-size: 16px;

            &.is-active {
              color: #3572FF;
            }
          }

          .el-tabs__item:hover {
            color: #3572FF;
          }

          .el-tabs__active-bar {
            background-color: #3572FF;
          }
        }

        ::v-deep .el-tabs .el-tabs__nav .tabs__item {
          height: 29px;
        }
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
          width: 25%;
          height: 105px;
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

          &.line_row {
            width: 668px;
          }
        }
      }
    }

    .body_right {
      background: #F9FAFC;
      position: relative;
      padding-top: 54px;
      box-sizing: border-box;
      padding: 0 16px;
      border-radius: 4px;

      .right_tabs {
        position: absolute;
        left: 16px;
        top: 0;
        padding: 16px;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        width: 100%;
        box-sizing: border-box;
        // border-bottom: 1px solid #e9eaea;
        width: 340px;
        z-index: 99;
        background-color: #F9FAFC;
        margin-left: -10px;

        .tabs_li {
          // height: 26px;
          cursor:pointer;
          line-height: 14px;
          padding: 8px 10px;
          display: inline-block;
          white-space: nowrap;
          border-radius: 4px;
          border: 1px solid #dadada;
          color: #525d69;
          background: #ffffff;
          margin-left: 10px;
          font-size: 14px;

          &.active {
            background: #0db3a6;
            color: #ffffff;
            border: none;
          }
        }

        .tabs_li:first-child {
          margin-left: 0;
        }
      }

      .ab_zhanwei {
        width: 100%;
        height: 64px;
        line-height: 64px;
        box-sizing: border-box;
      }

      .follow_up {
        width: 100%;
        box-sizing: border-box;
        padding-bottom: 152px; //82+72
        max-height: 563px;

        ::v-deep ::-webkit-scrollbar {
          width: 0;
        }

        // 滚动条里面默认的小方块,自定义样式
        ::v-deep ::-webkit-scrollbar-thumb {
          background: #8798af;
          border-radius: 2px;
        }

        // 滚动条里面的轨道
        ::v-deep ::-webkit-scrollbar-track {
          background: transparent;
        }

        .tianxie {
          width: 100%;
          margin-bottom: 10px;
          height: 80px;
        }

        .sousuo {
          width: 100%;

          .follow_search {
            height: 36px;
            border-radius: 20px;
          }
        }

        .follow_list {
          width: 100%;
          max-height: 450px;

          .follow_li {
            border-radius: 10px;
            background-color: #ffffff;
            font-size: 12px;

            padding: 16px;
            margin-top: 10px;

            .title {
              display: flex;
              justify-content: space-between;
              align-items: flex-start;
              margin-bottom: 10px;

              .left {
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
                  margin-left: 10px;
                  color: #333333;
                  font-size: 16px;
                }
              }

              .right {
                color: #3572FF;
              }
            }

            .content {
              margin-bottom: 10px;
            }

            .li_li {
              line-height: 26px;
              font-size: 14px;
              color: #78829D;
            }
          }

          .follow_li:first-child {
            margin-top: 0;
          }
        }

        .add_btn {
          width: 100%;
          padding: 16px;
          background-color: #F9FAFC;
          position: absolute;
          left: 0;
          bottom: 0;

          .add_follow {
            height: 40px;
            width: 100%;
            background: linear-gradient(90deg, #4C79FF 0%, #6DA8FF 100%);
            border-radius: 4px;
            color: #fff;
          }
        }
      }

      .pinglun_con {
        box-sizing: border-box;
        overflow-y: scroll;
        max-height: 563px;
        padding-bottom: 152px; //82+72

        &::-webkit-scrollbar {
          width: 0;
        }

        // 滚动条里面默认的小方块,自定义样式
        &::-webkit-scrollbar-thumb {
          background: #8798af;
          border-radius: 2px;
        }

        // 滚动条里面的轨道
        &::-webkit-scrollbar-track {
          background: transparent;
        }

        .add_btn {
          width: 100%;
          padding: 16px;
          background-color: #F9FAFC;
          position: absolute;
          left: 0;
          bottom: 0;

          .add_pinglun {
            height: 40px;
            width: 100%;
            background: linear-gradient(90deg, #4C79FF 0%, #6DA8FF 100%);
            border-radius: 4px;
            color: #fff;
          }
        }

        .comment_list {
          width: 100%;

          .comment_li {
            padding: 16px;
            background-color: #ffffff;
            margin-top: 10px;
            line-height: 24px;

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
              line-height: 18px;
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

              .zhankai {
                height: 14px;
                font-size: 14px;
                color: #78829D;
                display: flex;
                justify-content: flex-start;
                align-items: center;
                cursor: pointer;

                .line {
                  background-color: #E5E7EE;
                  width: 24px;
                  height: 1px;
                  margin-right: 8px;
                }
              }
            }
          }

          .comment_li:first-child {
            margin-top: 0;
          }
        }
      }
    }
  }
}

</style>