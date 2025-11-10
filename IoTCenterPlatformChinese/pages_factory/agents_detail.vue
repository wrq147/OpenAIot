<template>
	<view style="min-height: 100vh;background-color: rgba(245, 248, 249, 1);">
		<top title="详情" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"></top>
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
					<view class="li_label">级别</view>
					<view class="li_val">{{agentDetail.GradeName}}</view>
				</view>
				<view class="basic_info_li" v-if="agentDetail.Regions">
					<view class="li_label">地区</view>
					<view class="li_val">{{agentDetail.RegionsName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">代理厂家</view>
					<view class="li_val">{{agentDetail.FactoryName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">上级代理</view>
					<view class="li_val">{{agentDetail.ParentOrgName}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">创建时间</view>
					<view class="li_val">{{agentDetail.createTime}}</view>
				</view>
			</view>
			<button class="jump_button" @click="revokeAgent" :disabled="isSubmit" :loading="isSubmit">
				取消授权
			</button>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmRevoke"></msg-prompt>
		<uni-popup ref="clearAgentPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">提示</view>
					<view class="title_content" style="font-size:32rpx">
						是否确认取消【{{agentDetail.OrgName}}】的代理权限？
					</view>
					<view class="flex_content" style="margin: 30rpx 0 70rpx 0;">
						<view class="conten_icon_con" style="margin-right: 20rpx;height: 32rpx;line-height: 32rpx;" @click="setFunFilter(!isCancelAll)">
							<view v-if="isCancelAll" class="radio_icon t-icon-gouxuan1">
							</view>
							<custom-icons v-if="!isCancelAll" iconsName="icon-weixuanzhong" iconsSize="32rpx"
								iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
						</view>
						<view class="title_content" style="margin-top: 0;margin-bottom: 0;color: rgba(102, 102, 102, 1);">
							是否取消该代理及其所有下级代理的授权
						</view>
					</view>
					<button class="submit_button" @click.stop="confirmRevoke">
						确定
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
				isCancelAll: false
			};
		},
		async onLoad(options) {
			if (options.id) {
				this.id = options.id
				await this.getAgentInfo()
			}
		},
		methods: {
			noticeColse() {
				this.$refs.clearAgentPopup.close()
			},
			setFunFilter(val) {
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
					this.$refs.promptMsg.open('操作成功', 1500)
					setTimeout(() => {
						setPagesParam('loadList', 'load', 1)
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
				}
			},
			async getAgentInfo() {
				//获取代理商信息
				try {
					uni.showLoading({
						title:'加载中'
					})
					let res = await agentInfo({
						id: this.id
					})

					let res2 = await orgInfo({
						id: res.data.OrgId
					})
					res.data.Logo = res2.data.Logo
					res.data.OrgName = res2.data.OrgName
					this.agentDetail = res.data
					uni.hideLoading()
				} catch (e) {
					//TODO handle the exception
					uni.hideLoading()
					this.setMsgTop(e)
				}
			}
		}
	}
</script>

<style lang="less">

</style>