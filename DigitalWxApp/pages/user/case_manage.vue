<template>
	<view>
		<view class="wrap">
			<uni-swipe-action v-if="examList.length>0">
				<view class="item" v-for="item in examList" :key="item.Id">
					<!-- <uni-swipe-action-item> -->
					<navigator class="imgCon"
						:url="'/pages/card_center/card_case_detail?id='+item.Id+'&uid='+userInfo.Id">
						<image class="cover" :src="item.ImageUrl" mode="aspectFill"></image>
						<view class="txt">
							<text class="text">{{item.Title}}</text>
						</view>
						<navigator url="" @click.stop="editExample(item.Id)" class="editBTN">
							<image class="image" src="/static/edit.png" mode="aspectFill"></image>
							<!-- <text>案 例</text> -->
						</navigator>
						<navigator url="" class="delBTN" @click.stop="delExample(item.Id)">
							<image class="image" src="/static/del.png" mode="aspectFill"></image>
							<!-- <text>案 例</text> -->
						</navigator>
					</navigator>

					<!-- <template v-slot:right> -->
					<!-- <view class="acbts">
						<view style="background-color: #ffffff;">
							<navigator url="" @click.stop="editExample(item.Id)" class="editBTN">
								<text>编 辑</text>
							</navigator>
							<navigator url="" class="delBTN" @click.stop="delExample(item.Id)">
								<text>删 除</text>
							</navigator>
						</view>

					</view> -->
					<!-- </template> -->
					<!-- </uni-swipe-action-item> -->

				</view>
			</uni-swipe-action>
			<empty v-else-if="examList.length==0 && status!='loading'" imgsrc="/static/empty/content_empty.png"
				txt="暂无内容"></empty>
			<view v-else style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>
		</view>
		<view class="bnt_con">
			<view class="btn" @click="jumpSetting">
				设置
			</view>
			<view class="btn" @click="jumpAdd">
				添加案例
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getExampleList,
		delExample
	} from '@/api/example.js'
	export default {
		data() {
			return {
				userInfo: null, //用户信息
				OrgId: null,
				examList: [], //案例列表
				page: 1, //当前页数
				pageSize: 5, //每页展示的数据
				hashitems: [],
				status: 'loading', //设置查询的数据是否全部展示的状态
				changeId: '', //向左滑动的那个列表的id
				userInfo: null
			}
		},
		async onLoad() {
			this.userInfo = await this.$store.dispatch("userInfo");
			this.reloadpage();
		},
		onShow() {
			// this.reloadpage();
		},
		onReachBottom() {
			if (this.status != 'noMore') {
				this.page++,
					this.load_data()
			}
		},
		methods: {
			async reloadpage(name) {
				if (name == 'edit' || name == 'add') {
					this.page = 1;
					this.hashitems = [];
					this.examList = [];
					this.load_data();
				}
				this.userInfo = await this.$store.dispatch("userInfo");
				this.OrgId = this.userInfo.OrgId;
				this.page = 1;
				this.hashitems = [];
				this.examList = [];
				this.load_data();

			},
			//获取案例列表
			async load_data() {
				let query = {
					OrgId: this.OrgId,
					pageNum: this.page,
					pageSize: this.pageSize,
					showAll: true
				}
				let rsp = await getExampleList(query);
				// console.log("打印案例查询列表", rsp);
				rsp.data.List.forEach(it => {
					it["Id"] = it.Id.toString();
					if (!this.hashitems[it.Id]) {
						this.hashitems[it.Id] = true;
						this.examList.push(it);
					}
				})
				if (rsp.data.List.length < this.pageSize) {
					this.status = 'noMore'
				} else {
					this.status = 'more'
				}

			},
			// //获取向左滑动的那个列表的id
			// change(id) {
			// 	console.log("向右滑动的列表的id",id);
			// 	this.changeId = id;
			// },
			//编辑案例
			editExample(id) {
				uni.navigateTo({
					url: '/pages/user/case_edit?id=' + id
				})
			},

			//删除案例操作
			delExample(id) {
				uni.showModal({
					title: '提示',
					content: '确定删除该案例吗？',
					success: async res => {
						if (res.confirm) {
							// this.$refs.popup.close();
							uni.showLoading({
								title: '加载中...'
							});
							try {
								await delExample({
									id: id
								});
								await this.reloadpage();
								setTimeout(() => {
									uni.showToast({
										icon: 'success',
										title: '删除成功'
									}, 200);
								})
							} catch (err) {
								console.info('异常：', err);
							} finally {
								uni.hideLoading();
							}
						}
					}
				});
			},
			//跳转到案例设置
			jumpSetting() {
				uni.navigateTo({
					url: '/pages/user/case_setting'
				})
			},
			//跳转到添加案例
			jumpAdd() {
				this.OrgId = this.userInfo.OrgId;
				uni.navigateTo({
					url: "/pages/user/case_add?orgId=" + this.OrgId
				})
			}

		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.wrap {
		padding: 30rpx;
		box-sizing: border-box;
		width: 100%;
		padding-bottom: 236rpx;

		.item {
			width: 100%;
			border-radius: 10rpx;
			height: 340rpx;
			// background-color: #efefef;
			// background-size: 100% 100%;
			background-position: center center;
			display: flex;
			flex-direction: row;
			justify-content: flex-end;
			margin-bottom: 50rpx;
			align-items: center;

			.acbts {
				display: flex;
				flex-direction: row;

				height: 100%;
				font-size: 28rpx;
				line-height: 85rpx;

				.editBTN,
				.delBTN {
					flex: 1;
					display: flex;
					flex-direction: row;
					justify-content: center;
					height: 50%;
					// margin: 40rpx 0;
					text-align: center;
					width: 80rpx;
					box-sizing: border-box;
				}

				.editBTN {
					// background-color: #29C0FB;
					// opacity: 0.5;
					// background-color: rgba(56,185,251, 0.3);
					background-color: rgba(203, 204, 205, 0.3);
					// background-color: #CBCCCD;
					// background-color: #FFFFFF;
					padding: 0 10rpx;
					// color: #333333;
					color: #29C0FB;
					border-radius: 0 10rpx 0 0;
				}

				.delBTN {
					flex: 1;
					// background-color: #E84C35;
					// background-color: #FF4407;
					background-color: rgba(255, 68, 7, 0.3);
					color: #E84C35;
					padding: 0 10rpx;
					border-radius: 0 0 10rpx 0;
				}
			}

			.imgCon {
				width: 100%;
				height: 340rpx;
				position: relative;

				.editBTN,
				.delBTN {
					position: absolute;
					top: 20rpx;
					right: 20rpx;
					flex: 1;
					display: flex;
					flex-direction: row;
					justify-content: center;
					width: 54rpx;
					height: 54rpx;
					text-align: center;
					box-sizing: border-box;

					.image {
						width: 54rpx;
						height: 54rpx;
					}
				}

				.editBTN {
					right: 98rpx;
				}

				.cover {
					width: 100%;
					border-radius: 10rpx;
					// border-radius: 10rpx 0 0 10rpx;
					height: 340rpx;
					position: absolute;
					left: 0;
					top: 0;
				}

				.txt {
					position: absolute;
					left: 0;
					bottom: 0;
					width: 100%;
					// opacity: 0.6;
					box-sizing: border-box;
					padding-left: 20rpx;
					text-align: center;
					font-size: 28rpx;
					height: 66rpx;
					line-height: 66rpx;
					// background-color: rgba(51, 51, 51, 1);
					background: linear-gradient(rgba(0, 0, 0, 0), rgba(0, 0, 0, 0.6));
					// margin-bottom: 30rpx;
					border-radius: 0 0 10rpx 10rpx;

					.text {
						opacity: 1;
						color: #FFFFFF;
					}
				}
			}

		}
	}


	.bnt_con {
		width: 690rpx;
		padding: 10rpx 30rpx 30rpx 30rpx;
		position: fixed;
		bottom: 0;
		left: 0;
		// display: flex;
		// justify-content: center;
		background-color: #FFFFFF;

		.btn {
			margin-top: 20rpx;
			width: 690rpx;
			height: 90rpx;
			line-height: 90rpx;
			text-align: center;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			font-size: 30rpx;

			z-index: 9;
		}
	}
</style>
