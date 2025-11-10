<template>
	<view>
		<top title="Details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26" rightText="Edit" @clickRight="editConsumable"></top>
		<view class="device_list_con details_top_con" id="detail_top" v-if="consumbleDetail.Id">
			<view class="device_list">
				<view class="list_li">
					<view class="li_left">
						<image class="image" :src="consumbleDetail.PhotoUrl+'?wh=500x500'" mode="aspectFit">
						</image>
					</view>
					<view class="li_right">
						<view class="dev_name">
							{{consumbleDetail.Name}}
						</view>
						<view class="group space_flex">
							<view class="text">
								{{consumbleDetail.DeviceNumber}}
							</view>
							<view class="text">
								Price: ￥{{consumbleDetail.Price}}
							</view>
						</view>
						<view class="group">
							Category: {{consumbleDetail.ClassName}}
						</view>
					</view>
				</view>
			</view>
		</view>
		<view class="detail_con" v-if="consumbleDetail.Id">
			<view class="basic_info_list">
				<view class="basic_info_li" style="border-top: none;">
					<view class="li_label">Notes</view>
					<view class="li_val">{{consumbleDetail.Remark}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Unit</view>
					<view class="li_val">{{consumbleDetail.Unit}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Creation time</view>
					<view class="li_val">{{consumbleDetail.createTime}}</view>
				</view>
			</view>
			<button class="jump_button" @click="deleteConsumable" :disabled="isSubmit" :loading="isSubmit">
				Delete
			</button>
		</view>

		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
	</view>
</template>

<script>
	import {
		partsInfo,
		delParts
	} from "@/api/parts";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				consumbleDetail: {},
				id: ''
			};
		},
		onLoad(options) {
			this.$nextTick(async () => {
				this.$refs.promptMsg.loadingOpen()
				try {
					if (options.id) {
						this.id = options.id
						await this.loadDetail()
					}
					this.$refs.promptMsg.loadingColse()
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					this.$refs.promptMsg.loadingColse()
				}
			})
		},
		methods: {
			async confirmDelete() {
				//确认删除动作
				try {
					this.isSubmit = true
					await delParts(this.consumbleDetail.Id);
					this.$refs.promptMsg.open('Operation successful', 1500)
					setTimeout(() => {
						setPagesParam('loadList', 'load', 1)
					}, 1500)
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					this.isSubmit = false
				}
			},
			deleteConsumable() {
				this.$refs.promptMsg.noticeOpen('Are you sure to delete the consumables ' + this.consumbleDetail.Name +
					'? (This operation is irreversible)?')
			},
			async loadDetail() {
				//加载数据
				try {
					let row = await partsInfo(this.id)
					this.consumbleDetail = row.data;
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async loadList() {
				await this.loadDetail()
				setPagesParam('loadList', 'load', 1, false)
			},
			editConsumable() {
				uni.navigateTo({
					url: '/pages_factory/consumable_add?id=' + this.consumbleDetail.Id
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.details_top_con {
		// padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;
		padding-top: 10rpx;
		background-color: #161A26;
		margin-bottom: 30rpx;

		.device_list {
			.list_li {
				background-color: #161A26;
				margin-top: 0;
				padding: 0;

				.li_left {
					width: 150rpx;
					height: 150rpx;

					.image {
						width: 150rpx;
						height: 150rpx;
					}
				}
			}
		}
	}
</style>