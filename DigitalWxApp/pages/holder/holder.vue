<template>
	<view>
		<view class="topSearch">
			<uni-search-bar cancelButton="none" v-model.lazy="key" bgColor="#EFEFEF" placeholder="请输入搜索的关键词"
				@confirm="this.isSearch=true,this.reloadpage()" @input='this.isSearch=true,this.reloadpage()'
				@clear="this.key = '',this.isSearch=false,this.reloadpage()"></uni-search-bar>
			<navigator url="/pages/holder/apply" class="exc-block border-top">
				<view class="l">
					<image class="ximg" src="/static/logo.png" mode="widthFix"></image>
					<text>名片交换请求</text>
				</view>
				<view class="r">
					<uni-badge class="uni-badge-left-margin" :text="reqnum" absolute="rightTop" :offset="[10, -15]"
						size="small"></uni-badge>
					<text>全部请求</text>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</navigator>
		</view>

		<view style="margin-top: 220rpx;background-color: #fff;">
			<view class="tit-rs border-bottom">
				{{rstxt}}
			</view>
			<view class="rs-list" v-if="holderList.length>0">
				<navigator :url="'/pages/card_center/card?id='+hd.Id" class="rs-item border-bottom" v-for="hd in holderList" :key="hd.Id">
					<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="hd.Avatar"
						loading-ing-img="oblique-light" />
					<view class="mid">
						<text class="t">{{hd.RealName}}</text>
						<text class="c">{{hd.OrgName}}</text>
					</view>
					<text class="date">{{date2str(hd.CreatedOn)}}</text>
				</navigator>
			</view>
			<empty v-else-if="status!='loading'&&isSearch" imgsrc="/static/empty/result_empty.png" txt="暂无结果"></empty>
			<empty v-else-if="status!='loading'" imgsrc="/static/empty/data_empty.png" txt="暂无数据"></empty>
			<view v-else style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="1">
				</J-skeleton>
			</view>
			<uni-load-more v-if="holderList.length>0" iconType="circle" :showText="false" :status="status" />
		</view>

		<my-tab-bar :active="1"></my-tab-bar>
	</view>
</template>

<script>
	import MyTabBar from '@/components/my-tab-bar.vue'
	import {
		getHolderList
	} from '@/api/userCard.js'
	import {
		getPendingCount
	} from '@/api/user.js'
	import {
		dateUtils
	} from '@/common/util.js'
	import {getNoRead} from '@/api/record.js'
	export default {
		components: {
			MyTabBar
		},
		data() {
			return {
				key: "",
				rstxt: "",
				page: 1,
				status: 'loading',
				holderList: [],
				hashitems: {},
				isSearch: false,
				reqnum: 0
			};
		},
		onReady() {
			
		},
		async onLoad() {
			
			this.reloadpage();
		},
		onReachBottom: function() {
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data();
			}
		},
		async onShow() {
			let rsp = await getPendingCount();
			this.reqnum = rsp.data;
			let nd = this.$store.state.holderReloadQueue.pop();
			if (nd) {
				this.reloadpage();
			}
		},
		methods: {
			async reloadpage() {
				this.page = 1;
				this.status = "loading";
				this.holderList = [];
				this.hashitems = {};
				await this.load_data();
			},
			async load_data() {
				let rsp = await getHolderList({
					Key: this.key,
					pageSize: 30,
					pageNum: this.page
				});
				if (this.isSearch) {
					this.rstxt = "相关结果(" + rsp.data.Total + ")";
				} else {
					this.rstxt = "所有名片(" + rsp.data.Total + ")";
				}

				rsp.data.List.forEach(it => {
					if (!this.hashitems[it.Id]) {
						this.hashitems[it.Id] = true;
						this.holderList.push(it);
					}
				});
				if (rsp.data.List.length < 30) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}
				// console.log("名片列表",this.holderList);
			},
			date2str(timestr) {
				return dateUtils.parse(timestr).format("yyyy.MM.dd");//日期格式化到日
			}
		}
	}
</script>

<style lang="scss">
	.topSearch {
		z-index: 199999;
		background-color: #fff;
		position: fixed;
		top: 0;
		width: 100vw;
	}

	.exc-block {
		display: flex;
		justify-content: space-between;
		padding-left: 30rpx;
		padding-right: 30rpx;
		height: 90rpx;
		align-items: center;

		.l {
			display: flex;

			.ximg {
				width: 40rpx;
				height: 40rpx;
				border-radius: 11rpx;
				overflow: hidden;
				margin-right: 13rpx;
			}
		}

		.r {
			display: flex;
			align-items: center;
		}
	}

	.tit-rs {
		height: 90rpx;
		display: flex;
		align-items: center;
		font-weight: bold;
		color: #333333;
		padding-left: 30rpx;
	}

	.rs-item {
		display: flex;
		padding: 20rpx 30rpx;

		.ximg {
			width: 98rpx;
			height: 98rpx;
			border-radius: 50%;
			overflow: hidden;
		}

		.mid {
			margin-left: 22rpx;
			flex: 1;
			display: flex;
			flex-direction: column;

			.t {
				margin-top: 10rpx;
				font-size: 30rpx;
				font-weight: bold;
				color: #333333;
			}

			.c {
				margin-top: 6rpx;
				font-size: 24rpx;
				color: #999999;
			}
		}

		.date {
			margin-top: 10rpx;
			width: 120rpx;
			font-size: 22rpx;
			color: #999999;
		}
	}
</style>
