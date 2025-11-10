<template>
	<view>
		<view class="title border-top border-bottom">
			访客记录
		</view>
		<view class="wrap">

			<template v-if="visitlist.length>0">
				<view class="row" v-for="item in visitlist">
					<navigator url="" @click="onItemSelected(item)">
						<view class="newIcon" v-if="item.Status==0">
							<!-- <text>new</text> -->
						</view>
						<view class="photo">
							<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="item.Avatar" />
						</view>
						<view class="content border-bottom">
							<view class="first">
								<text class="name">{{item.RealName}}</text>
								<text class="time">{{date2str(item.CreatedOn)}}</text>
							</view>
							<view class="second" v-if="item.VisitType==0">
								查看了我的名片<text
									class="jiacu">【{{(item.Target.OrgName==null||item.Target.OrgName=="")?"":(item.Target.OrgName+"|")}}{{item.Target.RealName}}】{{item.VisitNumber}}</text>次{{waitTime(item)}}
							</view>
							<view class="second" v-if="item.VisitType==1">
								查看了我的产品<text class="jiacu">【{{item.Target.ProName}}】{{item.VisitNumber}}
								</text>次{{waitTime(item)}}
							</view>
							<view class="second" v-if="item.VisitType==2">
								查看了我的案例<text class="jiacu">【{{item.Target.Title}}】{{item.VisitNumber}}
								</text>次{{waitTime(item)}}
							</view>
							<view class="third">
								来源：{{item.SoureName}}
							</view>
						</view>
					</navigator>
				</view>
			</template>
			<empty v-else-if="status!='loading'" imgsrc="/static/empty/data_empty.png" txt="暂无数据"></empty>
			<view v-else style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>
			<view v-if="visitlist.length>0" style="padding-top: 20rpx;">
				<uni-load-more iconType="circle" :showText="false" :status="status" />
			</view>

		</view>

		<my-tab-bar :active="2"></my-tab-bar>
	</view>
</template>

<script>
	import MyTabBar from '@/components/my-tab-bar.vue'
	import {
		getVisitedList,
		readed
	} from '@/api/record.js'
	import {
		dateUtils
	} from '@/common/util.js'
	import {
		dateDiff
	} from '@/common/date.js'
	import {
		getNoRead
	} from '@/api/record.js'
	export default {
		components: {
			MyTabBar
		},
		data() {
			return {
				visitlist: [],
				page: 1,
				status: 'loading',
				hashitems: {},
				isread: false,
			}
		},
		onReachBottom() {
			// console.log("触底方法执行了");
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data();
			}
		},

		async onShow() {
			await this.reloadpage();
			await readed();
		},
		methods: {
			async reloadpage(name) {
				this.page = 1;
				this.status = "loading";
				this.visitlist = [];
				this.hashitems = {};
				await this.load_data();
				
			},
			async load_data() {
				let rsp = await getVisitedList({
					showAll: true,
					pageSize: 30,
					// pageNum: 2,
					pageNum: this.page,
					InitTarget: true
				});
				// console.log("访客列表", rsp);
				for (let idx = 0; idx < rsp.data.List.length; idx++) {
					let it = rsp.data.List[idx];
					if (!this.hashitems[it.Id]) {
						this.hashitems[it.Id] = true;

						let rt = await this.$store.dispatch("dictName", {
							name: "visit_source",
							value: it.VisitSource
						});
						it.SoureName = rt;
						this.visitlist.push(it);
					}
				}
				// console.log("每次查询后访客记录",this.visitlist);
				if (rsp.data.List.length < 30) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}
			},
			date2str(timestr) {
				return dateUtils.parse(timestr).format("yyyy.MM.dd hh:mm");
			},
			waitTime(item) {
				if (item.EndOn) {
					return "，停留了" + dateDiff(item.CreatedOn, item.EndOn, "second") + "秒";
				} else {
					return "";
				}
			},
			async onItemSelected(item) {
				let jmpurl = "";
				if (item.VisitType == 0) {
					jmpurl = '/pages/card_center/card?id=' + item.VisitCardId; //VisitCardId:访客名片id   TargetId:被访人员名片id
				} else if (item.VisitType == 1) {
					jmpurl = '/pages/card_center/card_pro_detail?id=' + item.TargetId;
				} else if (item.VisitType == 2) {
					jmpurl = '/pages/card_center/card_case_detail?id=' + item.TargetId;
				}
				uni.navigateTo({
					url: jmpurl
				});
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.title {
		font-weight: bold;
		font-size: 30rpx;
		height: 100rpx;
		line-height: 100rpx;
		box-sizing: border-box;
		padding: 0 30rpx;
		text-align: left;
	}

	.wrap {
		width: 100%;

		.row {
			height: 150rpx;
			width: 100%;


			navigator {
				height: 150rpx;
				width: 100%;
				box-sizing: border-box;
				display: flex;
				padding: 0 30rpx;
				padding-top: 30rpx;
				position: relative;

				// .newIcon{
				// 	position: absolute;
				// 	right: 50rpx;
				// 	top: 0;
				// 	font-style: italic;
				// 	width: 30rpx;
				// 	height: 20rpx;
				// 	line-height: 20rpx;
				// 	font-size: 20rpx;
				// 	color: #E44032;
				// }
				.newIcon {
					position: absolute;
					left: 25rpx;
					top: 42rpx;
					width: 10rpx;
					height: 10rpx;
					background-color: #E44032;
					border-radius: 50%;
				}

				.photo {
					margin-right: 13rpx;

					.ximg {
						width: 68rpx;
						height: 68rpx;
						border-radius: 50%;
						overflow: hidden;
					}
				}

				.content {
					flex: 1;
					height: 120rpx;
					box-sizing: border-box;
					margin-top: 2rpx;

					.first {
						display: flex;
						justify-content: space-between;

						.name {
							font-size: 24rpx;
						}

						.time {
							font-size: 20rpx;
							color: #B4B4B4;
						}
					}

					.second {
						color: #999999;
						margin-top: 2rpx;
						font-size: 22rpx;

						.jiacu {
							color: #50A6FA;
						}
					}

					.third {
						color: #B1B1B1;
						font-size: 20rpx;
						font-weight: lighter;
						margin-top: 10rpx;
					}
				}
			}

		}
	}
</style>
