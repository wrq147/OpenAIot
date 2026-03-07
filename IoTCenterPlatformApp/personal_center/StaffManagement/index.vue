<template>
    <view id="StaffManagement">
        <top
            leftIcon="icon-fanhui"
            :isleftBack="true"
            backgroundColor="#ffffff"
            title="员工管理"
            class="CRM-header"
            :rightText="!hide ? '取消' : ''"
            @clickRight="cancelHide" :isRightSlot="hide">
            <template v-slot:top_right v-if="hide">
                <view class="right_top_con" style="display: flex;align-items: center;">
                	<view class="right_top right1" @click.stop="handSet" v-if="hide">
                	    <custom-icons iconsName="icon-guanli" iconsSize="36rpx"></custom-icons>
                	</view>
                	<view class="right_top" v-if="hide" @click.stop="handAddAlert">
                	    <custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
                	</view>
                </view>
            </template>
        </top>
        <search-compt
            @openSelect="openSelect"
            @searching="searching"
            pal="请输入员工名称或手机号"
            inputBg="#F8F8F8"></search-compt>
        <select-compt
            ref="selectCompt"
            :haidate="true"
            :selectListParam="selectListParam"
            :timeQuery="timeQuery"
            :querydata="querydata"
            @selectFinsh="selectFinsh"
            fixedHeight="216rpx"></select-compt>
        <view
            class="StaffManagement-item"
            v-for="(item, index) in MenberList"
            :key="index"
            @click="handDetails(item)">
            <view v-if="!hide">
                <image
                    v-if="xuanId == item.Id"
                    @click.stop="handStop(item.Id)"
                    class="select"
                    :src="getSerVerUrl()+'/appimg/images/select.png'" />
                <image
                    v-else
                    @click.stop="handStop(item.Id)"
                    class="select"
                    :src="getSerVerUrl()+'/appimg/images/weixuan.png'" />
            </view>

            <view class="NameCutting" v-if="!item.Avatar && item.Avatar.length <= 0">
                {{ item.RealName.length > 2 ? item.RealName.slice(-2) : item.RealName }}
            </view>
            <view
                v-else-if="item.Avatar && item.Avatar.indexOf('profile.png') > -1"
                class="StaffManagement-item-img t-icon-morentouxiang1"
                v-else></view>
            <image v-else :src="item.Avatar" class="StaffManagement-item-img"></image>
            <view class="StaffManagement-item-txt">
                <view
                    class="title"
                    :class="[item.status == 1 || item.status == '1' ? 'active' : 'on']">
                    <view class="text">
                        {{ item.RealName }}
                    </view>
                    <view class="static_text" v-if="item.Signature">
                        {{ item.Signature }}
                    </view>
                </view>
                <view class="content">
                    {{ item.dept_name }}
                </view>
            </view>
            <view class="iconfont t-icon-dianhua1" @click.stop="TelPhone(item.Mobile)"></view>
        </view>

        <view class="BottomButton" v-if="!hide" @click="handDelete">
            <view class="BottomButton-item">
                <view class="iconfont t-icon-shanchu"></view>
                <view>删除</view>
            </view>
        </view>
        <uni-load-more iconType="circle" :status="status" v-if="status" />
        <!--添加弹窗-->
        <view class="AddPopUps" v-if="alertHide">
            <view class="AddPopUps-item">
                <view class="AddPopUps-title">
                    <view class="title">添加员工</view>
                    <view class="iconfont icon-guanbidanchuang" @click="handClose"></view>
                </view>
                <view v-if="!alertHideChilred">
                    <view class="link">通过链接邀请</view>
                    <view class="lianUrl">
                        {{ tipsValue }}
                    </view>
                    <view class="CopyLink" @click="copy(tipsValueCopy)">复制链接</view>
                    <view class="link">
                        链接有效期：
                        <text class="num">7天</text>
                        <text style="color: #333">后邀请链接过期</text>
                    </view>
                    <view class="Invitation"></view>
                    <view class="through" @click="handXianChilred">通过帐户邀请</view>
                </view>
                <view v-if="alertHideChilred">
                    <view class="link">通过帐户邀请</view>
                    <view class="lianUrlInput">
                        <view
                            v-if="keyInputHide"
                            style="
                                display: flex;
                                align-items: center;
                                justify-content: space-between;
                            ">
                            <input
                                placeholder-style="color:#C1C1C1;"
                                v-model="keyInput"
                                class="lianUrl-input"
                                type="text"
                                placeholder="请输入用户电话或电子邮件" />
                            <view class="lianUrl-inputSerach" @click="handSearch">搜索</view>
                        </view>
                        <view class="xuanSelect" v-else>
                            <text>{{ keyInput }}</text>
                            <text
                                @click="handSearchGuan"
                                class="iconfont icon-crmtianjiaguanbi"></text>
                        </view>
                    </view>
                    <view class="itemXuan" v-if="SearchList.length > 0 && !SearchHide">
                        <view
                            v-for="(item, index) in SearchList"
                            :key="index"
                            class="itemXuan-item"
                            @click="handChen(item)">
                            <img
                                class="logoImg"
                                :src="'https://52.28.35.196/' + item.Avatar"
                                alt="" />
                            <text class="itemXuan-name">{{ item.Name }}</text>
                        </view>
                    </view>
                    <view v-if="!keyInputHide" class="selectImg">
                        <view>
                            <img
                                class="selectImg-logoImg"
                                :src="'https://52.28.35.196/' + arrSearch.Avatar"
                                alt="" />
                        </view>
                        <view class="selectImg-name">
                            {{ keyInput }}
                        </view>
                    </view>
                    <view v-if="SearchHide">
                        <view @click="handSelectValue" class="lianUrlContent">
                            <view v-if="range.length == 0" style="color: #c1c1c1; font-size: 32rpx">
                                请选择部门
                            </view>
                            <view v-else v-for="(item, index) in range" class="lianUrlItem">
                                {{ item.name }}
                            </view>
                        </view>
                        <input
                            class="inputPosition"
                            placeholder-style="color:rgb(193, 193, 193);"
                            v-model="postName"
                            type="text"
                            placeholder="请输入职位" />
                    </view>
                    <view class="button-yao">
                        <view class="button" @click="handQuxiao">取消</view>
                        <view class="button yaoActive" @click="handYaoQqing">完成</view>
                    </view>
                </view>
            </view>
        </view>
        <view class="move" v-if="alertHide"></view>
        <msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
        <!-- <msg-prompt ref="promptMsg"></msg-prompt> -->
    </view>
</template>

<script>
import { setPagesParam } from '@/common/utillib.js';
import {
    yuanAarngemnt,
    deleteArngemnt,
    createCode,
    searchDeptSearch,
    memberJionOrg,
} from '@/api/personalCenter';
// #ifdef H5
import serverUrl1 from '@/common/constVar.js';
// #endif
// #ifndef H5
import serverUrl from '@/common/constVar.js';
// #endif

let ser = '';
// #ifdef H5
ser = serverUrl1.getServerUrl();
// #endif
// #ifndef H5
ser = serverUrl.getServerUrl();
// #endif
export default {
    data() {
        return {
            keyInputHide: true,
            SearchHide: false,
            selectListParam: [
                {
                    name: '状态',
                    params: 'Status',
                    pal: '请选择状态',
                    value: null,
                    localdata: [
                        {
                            text: '正常',
                            value: 0,
                        },
                        {
                            text: '停用',
                            value: 1,
                        },
                    ],
                },
            ],
            timeQuery: {
                name: '日期',
                params: ['beginTime', 'endTime'],
                value: [],
            },
            deltList: [],
            searchHide: false,
            SearchList: [],
            keyInput: '',
            value: '',
            range: [],
            alertHideChilred: false,
            alertHide: false,
            xuanId: '',
            arr: [],
            hide1: true,
            selected: 0,
            hide: true,
            MenberList: [],
            status: 'loading',
            querydata: {
                pageNum: 1,
                pageSize: 10,
                deptIdWithChildren: true,
            },
            arrSearch: [],
            postName: '',
            tipsValue: '',
            tipsValueCopy: '',
        };
    },
    mounted() {
        this.list(); //员工管理
    },
    methods: {
        handSearchGuan() {
            this.SearchHide = false;
            this.keyInputHide = true;
            this.keyInput = '';
        },
        cancelHide() {
            if (!this.hide) {
                this.hide = true;
            }
        },
        TelPhone(phone) {
            const res = uni.getSystemInfoSync(); //获取当前的手机机型
            if (res.platform == 'ios') {
                uni.makePhoneCall({
                    phoneNumber: phone,
                });
            } else {
                uni.makePhoneCall({
                    phoneNumber: phone,
                });
            }
        },
        selectFinsh(query) {
            //console.log(query,'111')
            this.querydata = JSON.parse(JSON.stringify(query));
            this.querydata.pageNum = 1;
            this.status = 'loading';
            this.list();
        },
        openSelect() {
            this.$refs.selectCompt.openSelect();
        },
        copy(context) {
            //context被复制的内容
            const that = this;
            //复制链接
            console.log('复制链接', context);
            uni.setClipboardData({
                data: context,
                success: () => {
                    // uni.showToast({
                    // 	title: '已自动复制网址，请在手机浏览器里粘贴该网址',
                    // 	duration: 2000,
                    // 	icon: 'none'
                    // });
                    // uni.hideToast()
                },
                fail: (err) => {
                    uni.showToast({
                        title: '复制失败！',
                        duration: 2000,
                        icon: 'none',
                    });
                },
            });
        },

        handYaoQqing() {
            memberJionOrg({
                uid: this.arrSearch.Id,
                depId: this.range[0].id,
                postName: this.postName,
            })
                .then((res) => {
                    if (res.code == 0) {
                        uni.showToast({
                            title: '邀请成功',
                            icon: 'none',
                        });
                        this.alertHideChilred = false;
                        this.alertHide = false;
                        this.list();
                    }
                })
                .catch((err) => {
                    this.setMsgTop(err);
                });
        },
        handChen(ite) {
            this.keyInput = ite.Name;
            this.arrSearch = ite;
            this.SearchList = [];
            this.SearchHide = true;
            this.keyInputHide = false;
            //console.log(ite.Name,this.keyInput,'ite')
            this.$forceUpdate();
        },

        handSearch() {
            searchDeptSearch({
                key: this.keyInput,
            }).then((res) => {
                //console.log(res);
                this.SearchList = [];
                if (this.keyInput) {
                    if (res.data.length > 0) {
                        this.SearchList = res.data;
                    } else {
                        uni.showToast({
                            title: '并未查询到该员工！',
                            icon: 'none',
                            duration: 2000,
                        });
                    }
                } else {
                    this.arrSearch = [];
                    this.SearchList = [];
                }
            });
        },
        //返回部门数据
        selectDept(data) {
            this.range = data;
            //console.log(this.range,'this.range');
        },
        handQuxiao() {
            this.alertHideChilred = false;
        },
        handSelectValue() {
            uni.navigateTo({
                url: '/pages_flow/inventory/employee_select?type=dept',
            });
        },
        handXianChilred() {
            this.alertHideChilred = true;
        },
        handAddAlert() {
            this.alertHide = true;
            createCode().then((res) => {
                // console.log("生成的邀请码", res.data);
                // //获取当前url
                // let baseUrl = window.location.href;
                // //当前路由
                // let baseR = this.$route.path;
                // //分割url
                // let yumAry = baseUrl.split(baseR);
                // //得到域名
                // let yuming = yumAry[0];
                let yuming = ser + '/jump.html';
                if (res.code == 0) {
                    this.tipsValueCopy =
                        yuming +
                        '?code=' +
                        encodeURIComponent(res.data) +
                        '&isMobile=true' +
                        '&lang=CN';
                    var lineurl =
                        yuming +
                        '?code=' +
                        encodeURIComponent(res.data) +
                        '&isMobile=true' +
                        '&lang=CN';
                    //this.tipsValue = yuming + "?code=" + encodeURIComponent(res.data)+'&isMobile=true'+"&lang=CN";
                    this.tipsValue = lineurl.length >= 30 ? lineurl.slice(0, 33) + '...' : lineurl;
                }
            });
        },
        handClose() {
            this.alertHide = false;
        },
        handleAdd() {
            this.open = true;
            this.title = '添加员工';
            createCode().then((res) => {
                //获取当前url
                let baseUrl = window.location.href;
                //当前路由
                let baseR = this.$route.path;
                //分割url
                let yumAry = baseUrl.split(baseR);
                //得到域名
                let yuming = yumAry[0];
                if (res.code == 0) {
                    this.tipsValue = yuming + '?code=' + res.data;
                }
            });
            // });
        },
        confirmUnbind() {
            deleteArngemnt({
                id: this.xuanId,
            })
                .then((res) => {
                    uni.showToast({
                        title: '删除成功！',
                        icon: 'none',
                    });
                    this.list(); //员工管理
                    this.hide = false;
                })
                .catch((err) => {
                    this.setMsgTop(err);
                });
        },
        handDelete() {
            //console.log(this.xuanId,'this.xuanId');
            if (this.xuanId == '') {
                uni.showToast({
                    title: '请选择一名员工！',
                    icon: 'none',
                });
            } else {
                this.$refs.promptMsg.noticeOpen('您确定要删除用户ID' + this.xuanId + '的员工吗？');
            }
        },
        handHide() {
            this.hide1 = !this.hide1;
        },
        handStop(id) {
            this.xuanId = id;
            // this.selected=!this.selected;
            // if(this.selected){
            //  this.xuanId=id;
            // }else{
            //  this.xuanId='';
            // }
            //console.log(this.xuanId,id,'this.selected')
        },
        handSet() {
            if (this.MenberList.length <= 0) {
                uni.showToast({
                    title: '目前没有员工！',
                    icon: 'none',
                });
                return;
            } else {
                this.hide = !this.hide;
                console.log(this.hide, 'this.hidethis.hidethis.hide');
                this.$forceUpdate();
                this.xuanId = '';
            }
        },
        handDetails(item) {
            uni.navigateTo({
                url: './ManagementDetails?id=' + item.Id, //JSON.stringify(item)
            });
        },
        searching(val) {
            this.key = val;
            if (this.key) {
                this.querydata.Key = this.key;
                this.querydata.pageNum = 1;
                this.status = 'loading';
                this.MenberList = [];
                this.list();
            } else {
                delete this.querydata.Key;
                this.querydata.pageNum = 1;
                this.status = 'loading';
                this.MenberList = [];
                this.list();
            }
        },
        list(query) {
            if (query && query == 'load') {
                this.querydata.pageNum = 1;
                this.status = 'loading';
            }
            yuanAarngemnt(this.querydata)
                .then((res) => {
                    // console.log(res,'员工列表');
                    if (this.querydata.pageNum == 1) {
                        this.MenberList = [];
                    }
                    this.MenberList = [...this.MenberList, ...res.data.List];
                    if (this.MenberList.length < res.data.Total) {
                        this.status = 'more';
                    } else {
                        this.status = 'noMore';
                    }
                })
                .catch((err) => {
                    this.status = 'noMore';
                    this.setMsgTop(err);
                });
        },
        onReachBottom() {
            if (this.status === 'more') {
                this.status = 'loading';
                this.querydata.pageNum++;
                this.list();
            }
        },
    },
};
</script>
<style>
page {
    background: #f5f8f9;
}
</style>
<style lang="less" scoped>
#StaffManagement {
    padding-bottom: 98rpx;

    .t-icon-dianhua1 {
        width: 50rpx;
        height: 50rpx;
        color: #e9f1ff;
        text-align: right;
        margin-left: auto;
        margin-right: 20rpx;
        font-size: 50rpx;
    }

    .move {
        position: fixed;
        height: 100%;
        width: 100%;
        background: #000;
        opacity: 0.7;
        top: 0px;
        left: 0px;
        z-index: 1;
    }

    .AddPopUps {
        position: absolute;
        margin: 0rpx 5%;
        background: #ffffff;
        z-index: 2;
        color: #333;
        width: 90%;
        border-radius: 8rpx;
        top: 250rpx;

        .AddPopUps-item {
            padding: 30rpx;

            .num {
                color: #2371ff;
                padding: 0rpx 10rpx;
            }
        }

        .through {
            height: 88rpx;
            line-height: 88rpx;
            padding: 0rpx 20rpx;
            border-radius: 12rpx;
            margin-top: 15rpx;
            color: #fff;
            text-align: center;
            color: #2371ff;
            margin-top: 50rpx;
            border: 1rpx solid #2371ff;
        }

        .Invitation {
            padding: 15rpx 0rpx;
        }

        .CopyLink {
            height: 88rpx;
            line-height: 88rpx;
            padding: 0rpx 20rpx;
            border-radius: 12rpx;
            margin-top: 15rpx;
            background: #2371ff;
            color: #fff;
            text-align: center;
        }

        .link {
            color: #999999;
            margin-top: 30rpx;
            font-size: 28rpx;
        }

        .addPool-selected {
            margin-top: 20rpx;

            ::v-deep.uni-select__input-text {
                color: #fff !important;
            }

            ::v-deep.uni-select {
                height: 88rpx !important;
                border: 2rpx solid rgba(255, 255, 255, 0.2);
            }

            ::v-deep.uni-select__input-placeholder {
                font-size: 30rpx !important;
                color: rgba(255, 255, 255, 0.2) !important;
            }
        }

        .button-yao {
            display: flex;
            justify-content: center;
            margin-top: 30rpx;

            .button {
                width: 200rpx;
                height: 88rpx;
                line-height: 88rpx;
                border: 2rpx solid rgba(255, 255, 255, 0.1);
                border-radius: 8rpx;
                text-align: center;
                margin: 0rpx 20rpx;
                background: #f8f8f8;
                color: #999999;
            }

            .yaoActive {
                background: #2371ff;
                color: #ffffff;
            }
        }

        .inputPosition {
            border: 2rpx solid rgba(255, 255, 255, 0.2);
            height: 88rpx;
            line-height: 88rpx;
            padding: 0rpx 20rpx;
            border-radius: 12rpx;
            margin-top: 15rpx;
            background: #f8f8f8;
            color: #333;
            font-size: 28rpx;
        }

        // .addPool-selected{
        // 	height:88rpx;
        // 	line-height:88rpx;
        // 	border:2rpx solid rgba(255, 255, 255, .2);
        // 	margin-top:10rpx;
        // 	border-radius: 10rpx;
        // }
        .selectImg {
            display: flex;
            flex-direction: column;
            align-items: center;

            .selectImg-name {
                margin: 10rpx 0rpx;
                font-size: 28rpx;
            }

            .selectImg-logoImg {
                width: 100rpx;
                height: 100rpx;
                border-radius: 50%;
                margin-top: 46rpx;
            }
        }

        .itemXuan {
            .itemXuan-item {
                display: flex;
                align-items: center;
                height: 90rpx;
                // border: 2rpx solid rgba(255, 255, 255, .3);
                margin-top: 20rpx;
                padding: 0rpx 20rpx;
                border-radius: 8rpx;
            }

            .itemXuan-name {
                margin-left: 20rpx;
                font-size: 32rpx;
            }

            .logoImg {
                width: 60rpx;
                height: 60rpx;
                border-radius: 50%;
            }
        }

        .lianUrlInput {
            height: 88rpx;
            line-height: 88rpx;
            padding: 0rpx 20rpx;
            border: 2rpx solid rgba(255, 255, 255, 0.2);
            border-radius: 4rpx;
            margin-top: 15rpx;
            border-radius: 10rpx;
            background: #f8f8f8;

            .xuanSelect {
                display: flex;
                background: #ffffff;
                width: 114rpx;
                height: 68rpx;
                padding: 0rpx 20rpx;
                line-height: 68rpx;
                text-align: center;
                align-items: center;
                justify-content: space-between;
                font-size: 28rpx;
                color: #333333;

                .icon-crmtianjiaguanbi {
                    margin-left: auto;
                    font-size: 18rpx;
                }
            }

            .lianUrl-input {
                width: 70%;
                margin-right: 10rpx;
            }
            .lianUrl-inputSerach {
                width: 28%;
                text-align: center;
                border-radius: 4rpx;
                background: #2371ff;
                color: #ffffff;
            }
        }

        .lianUrlContent {
            height: 88rpx;
            line-height: 88rpx;
            word-break: break-all;
            padding: 0rpx 20rpx;
            border: 1rpx solid rgba(255, 255, 255, 0.2);
            border-radius: 4rpx;
            margin-top: 15rpx;
            background: #f8f8f8;
            color: #333;
        }

        .lianUrl {
            // line-height:88rpx;
            word-break: break-all;
            padding: 0rpx 20rpx;
            border: 1rpx solid rgba(255, 255, 255, 0.2);
            border-radius: 4rpx;
            margin-top: 15rpx;
            background: #f8f8f8;
            height: 88rpx;
            line-height: 88rpx;

            .lianUrlItem {
                display: flex;
                align-items: center;
                margin-right: 15rpx;
            }
        }

        .AddPopUps-title {
            position: relative;
            // margin:20rpx 30rpx;
            align-items: center;

            .title {
                text-align: center;
                font-size: 34rpx;
                font-weight: bold;
            }

            .icon-guanbidanchuang {
                position: absolute;
                margin-left: auto;
                width: 40rpx;
                height: 40rpx;
                right: 0rpx;
                top: 5rpx;
                color: #c1c1c1;
            }
        }
    }

    .BottomButton {
        position: fixed;
        bottom: 0rpx;
        display: flex;
        background: #ffffff;
        height: 98rpx;
        color: #333;
        align-items: center;
        justify-content: space-around;
        width: 100%;

        .BottomButton-item {
            display: flex;
            align-items: center;

            .iconfont {
                width: 28rpx;
                height: 28rpx;
                margin-right: 10rpx;
            }
        }
    }

    .Staff-SelectAll {
        color: #fff;
        display: flex;
        align-items: center;
        margin: 20rpx 15rpx;

        .icon-guanli {
        }

        .left {
            display: flex;

            .SelectAllText {
                margin-left: 20rpx;
            }

            .select {
                width: 40rpx;
                height: 40rpx;
                border-radius: 50%;
                margin: 0rpx 20rpx;
            }
        }

        .right {
            margin-left: auto;
            color: rgba(255, 255, 255, 0.4);
        }
    }

    .StaffManagement-item {
        padding: 30rpx 20rpx;
        // height: 148rpx;
        background: #ffffff;
        border-radius: 8rpx;
        display: flex;
        align-items: center;
        margin: 0rpx 20rpx;
        margin-top: 20rpx;

        .icon-dianhua {
            margin-left: auto;
        }

        .StaffManagement-item-txt {
            color: #333;
            margin-left: 30rpx;

            .title {
                display: flex;
                font-size: 30rpx;
                line-height: 44rpx;
                align-items: center;

                .static_text {
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    padding: 0 8rpx;
                    margin-left: 20rpx;
                    font-size: 20rpx;
                    background-color: #e9f1ff;
                    color: #2371ff;
                    // line-height: 30rpx;
                    max-width: 136rpx;
                    height: 30rpx;
                    border-radius: 4rpx;
                    box-sizing: border-box;
                    overflow: hidden;
                    /* 隐藏溢出的内容 */
                    white-space: nowrap;
                    /* 确保文本不会换行 */
                    text-overflow: ellipsis;
                    /* 可选：当文本被裁剪时显示省略号 */
                }
            }

            .active {
                text-decoration: line-through;
            }

            .content {
                font-size: 24rpx;
                color: #999999;
                line-height: 38rpx;
                margin-top: 6rpx;
            }
        }

        .select {
            width: 40rpx;
            height: 40rpx;
            border-radius: 50%;
            margin: 0rpx 20rpx;
        }

        .weixuanzhongda {
            width: 35rpx;
            height: 35rpx;
            border-radius: 50%;
            margin: 0rpx 20rpx;
            border: 1rpx solid rgba(255, 255, 255, 0.4);
            background: red;
        }

        .icon-dianhua {
            margin-left: auto;
            color: #e9f1ff;
            font-size: 45rpx;
        }

        .NameCutting {
            width: 88rpx;
            height: 88rpx;
            border-radius: 50%;
            line-height: 88rpx;
            text-align: center;
            color: #fff;
            font-size: 24rpx;
            background: rgba(35, 113, 255, 1);
        }

        .StaffManagement-item-img {
            width: 88rpx;
            height: 88rpx;
            border-radius: 50%;
        }
    }
}
</style>
