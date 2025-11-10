<template>
	<view>
		<top title="Details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26"></top>
		<view class="detail_con" style="padding-top: 90rpx;">
			<!-- v-if="agentDetail.Id" -->
			<view class="basic_info_list">
				<view class="agent_info">
					<image class="image" :src="agentDetail.Logo+'?wh=500x500'" mode=""></image>
					<view class="name">
						{{agentDetail.OrgName}}
					</view>
					<view class="id">
						{{agentDetail.Id}}
					</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Grade</view>
					<view class="li_val">{{agentDetail.GradeName}}</view>
				</view>
				<view class="basic_info_li" v-if="agentDetail.Regions">
					<view class="li_label">Region</view>
					<view class="li_val">{{agentDetail.RegionsName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">factory</view>
					<view class="li_val">{{agentDetail.FactoryName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Superior</view>
					<view class="li_val">{{agentDetail.ParentOrgName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Creation time</view>
					<view class="li_val">{{agentDetail.createTime}}</view>
				</view>
			</view>
			<button class="jump_button" @click="revokeAgent" :disabled="isSubmit" :loading="isSubmit">
				Revoke
			</button>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmRevoke"></msg-prompt>
		<uni-popup ref="clearAgentPopup" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0.7)">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">prompt</view>
					<view class="title_content" style="font-size:32rpx">
						Are you sure to cancel the proxy permission for {{agentDetail.OrgName}}?
					</view>
					<view class="flex_content" style="margin: 30rpx 0 70rpx 0;">
						<view class="conten_icon_con" style="margin-right: 20rpx;height: 32rpx;line-height: 32rpx;padding: 0;" @click="setFunFilter(!isCancelAll)">
							<view v-if="isCancelAll" class="radio_icon t-icon-gouxuan">
							</view>
							<custom-icons v-if="!isCancelAll" iconsName="icon-weixuanzhong" iconsSize="32rpx"
								iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
						</view>
						<view class="title_content" style="margin-top: 0;margin-bottom: 0;color: rgba(102, 102, 102, 1);font-size: 28rpx;line-height: 36rpx;">
							Do you want to cancel the authorization of this agent and all its subordinate agents
						</view>
					</view>
					<button class="submit_button" @click.stop="confirmRevoke">
						Confirm
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="#c1c1c1"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import {
		agentInfo,
		orgInfo,
		CancelProxy
	} from "@/api/factory";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				agentDetail: {},
				id: '',
				isCancelAll:false
			};
		},
		async onLoad(options) {
			if (options.id) {
				this.id = options.id
				await this.getAgentInfo()
			}
		},
		methods: {
			noticeColse(){
				this.$refs.clearAgentPopup.close()
			},
			setFunFilter(val) {
				console.log('权限',val);
				this.isCancelAll = val
				this.$forceUpdate()
			},
			async confirmRevoke() {
				//确认取消授权动作
				try {
					this.$refs.clearAgentPopup.close()
					this.isSubmit = true
					await CancelProxy({
						id: this.id,
						cancelDown: this.isCancelAll
					});
					this.$refs.promptMsg.open('Operation successful', 1500)
					setTimeout(() => {
						setPagesParam('loadData', 'load', 1)
					}, 1500)
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					this.isSubmit = false
				}
			},
			revokeAgent() {
				try {
					this.$refs.clearAgentPopup.open()
				} catch (e) {
					console.log(e);
				}
			},
			async getAgentInfo() {
				//获取代理商信息
				try {
					let res = await agentInfo({
						id: this.id
					})
					console.log(res, '代理信息');
					
					let res2 = await orgInfo({
						id: res.data.OrgId
					})
					 res.data.Logo=res2.data.Logo
					 res.data.OrgName=res2.data.OrgName
					this.agentDetail = res.data
					console.log("企业信息", res2);
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			}
		}
	}
</script>

<style lang="less">

</style>