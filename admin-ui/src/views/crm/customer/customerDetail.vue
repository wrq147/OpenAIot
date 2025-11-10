<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con" v-loading="loading">
        <div class="custom_info_cot elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <div class="jianjie">
                <div class="jianjie_con">
                    <div class="jianjie_li">
                        <div class="label">客户编号：</div>
                        <div class="value">{{ customInDetail.CustomerNumber }}</div>
                    </div>
                    <div class="jianjie_li">
                        <div class="label">负责人：</div>
                        <div class="value haiImg">
                            <img class="image" :src="customInDetail.LeaderAvatar" alt="">
                            <span>{{ customInDetail.LeaderName }}</span>
                        </div>
                    </div>
                </div>
                <div class="button_con">
                    <el-button @click="handleUpdate(customInDetail)">编辑</el-button>
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
                            <div class="cot_label">客户</div>
                            <div class="cot_value">{{ customInDetail.CustomerName }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">归属部门</div>
                            <div class="cot_value">{{ customInDetail.DeptName }}</div>
                        </div>
                        <div class="row_cot third">
                            <div class="cot_label">负责人</div>
                            <div class="cot_value">{{ customInDetail.LeaderName }}</div>
                        </div>
                        <div class="row_cot frist">
                            <div class="cot_label">协作人</div>
                            <div class="cot_value">{{ customInDetail.HelperName }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">客户来源</div>
                            <div class="cot_value blue">{{ fromMap.get(customInDetail.FromType) }}</div>
                        </div>
                        <div class="row_cot third">
                            <div class="cot_label">所属行业</div>
                            <div class="cot_value">{{ customInDetail.IndustryName }}</div>
                        </div>
                        <div class="row_cot frist">
                            <div class="cot_label">公司网址</div>
                            <div class="cot_value blue">{{ customInDetail.CompanyUrl }}</div>
                        </div>
                        <div class="row_cot">
                            <div class="cot_label">公司电话</div>
                            <div class="cot_value blue">{{ customInDetail.CompanyTel }}</div>
                        </div>
                        <div class="row_cot third">
                            <div class="cot_label">客户状态</div>
                            <div class="cot_value">{{ customInDetail.del_flag == 0 ? '存在' : '删除' }}</div>
                        </div>
                        <div class="row_cot line_row">
                            <div class="cot_label">公司地址</div>
                            <div class="cot_value" v-if="customInDetail.AddressName">
                                {{ customInDetail.AddressName + customInDetail.AddressDetail }}</div>
                        </div>
                        <div class="row_cot line_row">
                            <div class="cot_label">客户详情</div>
                            <div class="cot_value" v-if="customInDetail.Remark">{{ customInDetail.Remark }}</div>
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
                                v-if="customFollowList.length > 0">
                                <div class="follow_li" v-for="row in customFollowList" :key="row.Id">
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
                                        placeholder="请输入跟进内容" :autosize="{ minRows: 5, maxRows: 5 }"></el-input>
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
                        <div class="comment_list" v-infinite-scroll="loadCustomComment" style="overflow:auto"
                            v-if="Object.keys(customCommentObj).length > 0">
                            <div class="comment_li" v-for="(row, keys) in customCommentObj" :key="row.Id">
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
        <el-dialog :visible.sync="pinglunEdit" width="800px" :show-close="true" @close="pinglunEdit = false"
            class="edit_info_dialog" :close-on-click-modal="false">
            <el-form class="add_clue_form" ref="pinglunForm" :model="pinglunForm" :rules="pinglunRules" label-width="100px"
                label-position="top">
                <el-row :gutter="20">
                    <el-col :span="24">
                        <el-form-item label="回复内容" prop="content">
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
        <customer-dialog ref="customAdd" @errLoading="errLoading" @finishLoading="finishLoading"></customer-dialog>
    </div>
</template>

<script>
import { listDept } from "@/api/system/dept";
import { listMember } from "@/api/system/Employee";
import {
    customerInfo,
} from "@/api/crm/customer";
import {
    followList,
    addFollow,
    editFollow,
} from "@/api/crm/follow";
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { commentAddClue, commentList } from '@/api/crm/comment'
import CustomerDialog from "@/views/crm/compontent/customer_dialog";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import { checkPermi } from "@/utils/permission"; 
export default {
    name: 'AdminUiCustomerDetail',
    components: { TextEditor, CustomerDialog,OrgPicker },
    dicts: ["follow_way"],
    mixins: [resizeTableCon],
    data() {
        return {
            leftContentHei: 0,
            customInDetail: {}, //客户详情
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
            id: '',
            followWayList: [],//跟进方式列表
            followAddForm: {
                targetType: 0,//0是客户，1是线索
                targetId: '',
                targetName: '',
                followTime: null,
                userInfo: [], //已经选择的跟进人
                followUserName: [], //
                followUserAvatar: '',
                followUser: null,//跟进人
                followWay: '',//跟进方式
                remark: "" //跟进记录详情
            },
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
            activeFollow: 'list',//展示的是跟进列表还是跟进添加
            customFollowList: [],//指定客户跟进动态列表

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
            followPage: 1,//跟进记录当前是那一页
            //跟进记录相关参数
            followCont: "", //跟进内容
            followSearch: "", //跟进搜索参数
            //跟进记录相关参数

            commentPage: 1,//评论当前页
            customCommentList: [],//评论列表
            customCommentObj: [],//评论
            activeTabs: "first", //活动的客户详情的切换
            deptMap: new Map(), //部门设置为Map
            fromMap: new Map(), //线索来源map
            employeeMap: new Map(), //员工map
            flowActiveTabs: 0,
            loading: true,
        };
    },

    async mounted() {
        this.setFromMap();
        await this.getMemberList()
        let pars = this.$route.query;
        // console.log("参数", pars);
        if (pars.id) {
            this.id = pars.id;
            this.viewInfo(pars.id)
        }
    },

    methods: {
        isCheckPermi(val) {//权限判断
            return checkPermi(val)
        },
        handleUpdate(row) {
            //修改客户信息
            this.$refs.customAdd.setEmployeeMap(this.employeeMap)
            if (row.Id) {
                this.$refs.customAdd.handleUpdate(row)
            }
        },
        errLoading() {
            //访问接口失败
            this.loading = false;
        },
        finishLoading() {
            this.loading = false;
            this.$refs.customAdd.cancel()//关闭弹窗
            this.viewInfo(this.id);
        },
        handleClick(tab, event) {
            //客户详情点击tabs切换
            // console.log(tab, event);
        },
        async getDeptList() {
            //获取企业部门列表
            try {
                let res = await listDept();
                if (res.data) {
                    res.data.map(row => {
                        this.deptMap.set(row.deptId, row);
                    });
                }
            } catch (error) {
                // console.log("部门列表报错", error);
            }
        },
        async getMemberList() {
            try {
                let rsp = await listMember({ deptIdWithChildren: true, showAll: true,isPrimaryDept:true });
                if (rsp.data && rsp.data.List) {
                    // console.log("所有员工", rsp);
                    rsp.data.List.map(row => {
                        this.employeeMap.set(row.Id, row);
                    });
                    this.$refs.customAdd.setEmployeeMap(this.employeeMap)
                }
            } catch (err) {
                console.log("报错", err);

                this.$message.error(err.data);
            }
        },
        setFromMap() {
            //设置线索来源map
            this.fromList.map(row => {
                this.fromMap.set(row.value, row.label);
            });
        },
        async getIndustry() {
            //获取行业名称
            let name = await this.$store.dispatch("datas/industryName");
        },
        viewInfo(val) {
            //客户详情
            this.followWayList = this.dict.type.follow_way;
            this.resetForm("followAddForm");
            // console.log("详情id", val);
            if (val) {
                this.loading = true
                customerInfo({ id: val })
                    .then(async res => {
                        if (res.data) {
                            // console.log(res.data, '客户详情yyy');
                            let data = res.data;
                            let useList = [];
                            data.LeaderName = "";
                            let industryArr = [];
                            if (data.Industry) {
                                //行业类型
                                data.IndustryName = await this.$store.dispatch(
                                    "datas/industryName",
                                    data.Industry
                                );
                            }
                            if (data.DeptId && this.deptMap.get(data.DeptId)) {
                                data.DeptName = this.deptMap.get(data.DeptId).deptName;
                            } else {
                                data.DeptName = "";
                            }
                            if (data.Helper) {
                                let arr = data.Helper.split(",");
                                arr.map(it => {
                                    useList.push(this.employeeMap.get(parseInt(it)));
                                });
                            }
                            if (data.LeaderId) {
                                //获取责任人相关信息
                                data.LeaderName = this.employeeMap.get(
                                    parseInt(data.LeaderId)
                                ).RealName;
                                data.LeaderAvatar = this.employeeMap.get(
                                    parseInt(data.LeaderId)
                                ).Avatar;
                            }
                            this.customInDetail = data;
                            // console.log("this.customInDetail", this.customInDetail);
                            await this.getCustomFollowList(data.Id, 1)//获取跟进记录列表
                            if(this.isCheckPermi(['/DiscussService/Comment/List'])){
                                this.getCustomCommentList(1)//获取客户评论列表
                            }
                            // this.infoVisible = true; //数据赋值完后再打开弹出层
                            this.$nextTick(() => {
                                this.leftContentHei = this.$refs.leftContent.offsetHeight;
                                // console.log("获取左边元素的高度", this.leftContentHei);
                            })
                            this.$forceUpdate()
                        }
                        this.loading = false
                    })
                    .catch(err => {
                        console.log(err, '错误');
                        this.loading = false
                    });
            }
        },
        switchTabs(val) {
            //切换跟进和评论
            this.flowActiveTabs = val;
        },
        expandMore(key, length) {
            //展开更多评论
            if (this.customCommentObj[key].curExpendNum < length) {
                if (length - this.customCommentObj[key].curExpendNum > 5) {
                    this.customCommentObj[key].curExpendNum = this.customCommentObj[key].curExpendNum + 5
                } else {
                    let rence = length - this.customCommentObj[key].curExpendNum
                    this.customCommentObj[key].curExpendNum = this.customCommentObj[key].curExpendNum + rence
                }
            }
        },
        loadCustomComment() {
            //继续加载客户跟进
            this.commentPage = this.commentPage + 1
            this.getCustomCommentList(this.commentPage)
        },
        loadCustomFollow() {
            //继续加载客户跟进
            this.followPage = this.followPage + 1
            this.getCustomFollowList(this.customInDetail.Id, this.followPage)
        },
        getCustomCommentList(page) {
            //获取客户评论列表
            if (page == 1) {
                this.customCommentList = []
                this.customCommentObj = {}
            }
            this.commentPage = page
            commentList({ TargetType: "客户", TargetId: this.customInDetail.Id, pageNum: page, pageSize: 10, }).then(res => {
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
                            console.log("");
                            row.curExpendNum = 2//当前展开的评论的数量
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
        editorInput(value) {
            //富文本输入赋值
            this.pinglunForm.content = value
        },
        submitPinglunForm() {
            //提交评论
            // console.log(this.pinglunForm, 'this.pinglunForm');
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
        closePinglunForm() {
            //关闭评论写的弹窗
            this.pinglunEdit = false
        },
        openPinglunForm(parentId, parentUserId) {
            this.pinglunEdit = true
            if (parentId && parentUserId) {
                this.pinglunForm = {
                    parentCommentId: parentId,
                    parentCommentUserId: parentUserId,
                    targetId: this.customInDetail.Id,
                    targetType: '客户',//评论类型   客户：customer线索：clue商机：oppprt
                    content: '',//评论内容
                }
            } else {
                this.pinglunForm = {
                    targetId: this.customInDetail.Id,
                    targetType: '客户',//评论类型   客户：customer线索：clue商机：oppprt
                    content: ''//评论内容
                }
            }
            this.resetForm("pinglunForm");

        },
        openFollowForm() {
            //打开跟进记录添加表单
            this.resetForm("followAddForm");
            this.followAddForm = {
                targetType: 0,//0是客户，1是线索
                targetId: this.customInDetail.Id,
                targetName: this.customInDetail.CustomerName,
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
                                console.log(res, "修改跟进记录成功");
                                if (res.code == 0) {
                                    this.loading = false;
                                    this.$modal.msgSuccess("修改成功");
                                    this.closeFollowForm()
                                    await this.getCustomFollowList(this.customInDetail.Id, 1)//获取跟进记录列表
                                }
                            })
                            .catch(err => {
                                console.log("err", err);
                                this.loading = false;
                            });
                    } else {
                        addFollow(submitForm)
                            .then(async res => {
                                console.log(res, "添加跟进记录成功");
                                if (res.code == 0) {
                                    this.loading = false;
                                    this.$modal.msgSuccess("添加成功");
                                    this.closeFollowForm()
                                    await this.getCustomFollowList(this.customInDetail.Id, 1)//获取跟进记录列表
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
        getFollowFocus() {
            //获取跟进人输入框焦点
            this.$refs.selectFollow.blur();
            if (this.followAddForm.followUser > 0) {
                this.followAddForm.userInfo = [{ id: this.followAddForm.followUser, name: this.followAddForm.followUserName, avatar: this.followAddForm.followUserAvatar, type: "user" }];

            }
            else {
                this.followAddForm.userInfo = [];
            }

            this.$refs.followPicker.show(this.followAddForm.userInfo, "user");
        },
        selectFollower() {
            // console.log("选中的跟进人", values);
            //选择跟进人以后返回的数据
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
        async getCustomFollowList(TargetId, page) {
            //获取客户跟进记录
            try {
                if (page == 1) {
                    this.customFollowList = []
                }
                this.followPage = page
                let res = await followList({ TargetType: 0, TargetId: TargetId, pageNum: page, pageSize: 10, });
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
                    this.customFollowList = [...this.customFollowList, ...res.data.List]
                }
            } catch (error) {
            }
        },
    },
};
</script>

<style lang="scss" scoped>
.custom_info_cot {
    // max-height: 600px;
    overflow-y: scroll;
    position: relative;

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
                line-height: 21px;


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

        // justify-content: space-between;
        // align-items: stretch;
        .body_left {
            width: 64%;
            display: flex;
            justify-content: flex-start;
            flex-direction: column;
            align-items: flex-start;
            height: 552px;

            .tabs_cont {
                width: 100%;
                // border-top: 1px solid #f2f2f2;
                height: 29px;
                margin-top: 12px;
                font-size: 16px;
                border-bottom: 1px solid #F6F7FA;
                margin-bottom: 10px;

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
                    width: 36%;
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

                    &.frist {
                        padding-left: 0;
                        width: calc(36% - 20px);
                    }

                    &.third {
                        // width: 198px;
                        width: 18%;
                    }

                    &.line_row {
                        width: 95%;
                        padding-left: 0;
                    }
                }
            }
        }

        .body_right {
            width: 36%;
            background: #F9FAFC;
            // display: flex;
            // justify-content: flex-start;
            // flex: 1;
            // flex-direction: column;
            // align-items: center;
            position: relative;
            padding-top: 54px;
            box-sizing: border-box;
            padding: 0 16px;
            border-radius: 4px;
            overflow: hidden;

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

.custom_infocon {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .custom_title {
        display: flex;
        justify-content: flex-start;
        align-items: center;

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

    .custom_btn {
        display: flex;
        justify-content: flex-end;
        align-items: flex-start;
        height: 24px;

        .icons {
            width: 10px;
            height: 10px;
            font-size: 10px;
            display: flex;
            justify-content: center;
            align-items: center;
        }
    }
}
</style>