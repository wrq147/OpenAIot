<template>
	<view>
		<view class="search">
			<view class="search_icon">
				<!-- <uni-search-bar class="my-search-bar" @confirm="search" radius="5" :focus="true" cancelButton="none" bgColor="#FFFFFF"
					clearButton="none" v-model="searchVal" placeholder="搜索词">
					<uni-icons slot="searchIcon" color="#999999" size="22" type="search" />
				</uni-search-bar> -->
				<uni-easyinput class="my-search-bar" :focus="true" @input="iconClick" :styles="styles"
					prefixIcon="search" clearSize="16" v-model="searchVal" confirm-type="search" @confirm="iconClick"
					placeholder="搜索词" @iconClick="iconClick" @clear="cancel">
				</uni-easyinput>
			</view>
			<view class="cancel" @click="cancel">取消</view>
		</view>
		<view class="topheight-zw"></view>
		<view class="ls-wrap" v-if="!isSearch">
			<view class="ls-t border-bottom" v-if="historyList.length>0">
				<view class="l">历史记录</view>
				<!-- <van-icon name="delete-o" color="#333" size="44" @click="clearHistory" /> -->
				<uni-icons size="25" type='trash' @click="clearHistory"></uni-icons>
			</view>
			<view class="ls-c">
				<view class="c-item" v-for="hitem in historyList" @click="choiceHistory(hitem)">
					{{hitem}}
				</view>
			</view>
			<!-- <empty imgsrc="/static/empty/content_empty.png" txt="暂无搜索内容"></empty> -->
		</view>
		<view class="list" v-else>
			<view v-if="list.length>0" class="item" v-for="item in list" :key="item.Id">
				<navigator :url="'/pages/card_center/card_pro_detail?id='+item.Id+'&uid='+uid+'&cardId='+cardId">
					<view class="left" v-if="item.ImageUrl">
						<!-- <image src="../../static/logo.png"></image> -->
						<image :src="item.ImageUrl" mode="aspectFill"></image>
					</view>
					<view class="left" v-else>
						<text>产品照片</text>
					</view>
					<view class="right">
						<view class="title">
							{{item.ProName}}
						</view>
						<view class="price">
							<view class="wholePrice" v-if="item.WholePrice>0 || item.Price>0">
								<text class="priceIcon" v-if="item.WholePrice>0">￥</text>
								<text class="num">{{item.WholePrice>0?item.WholePrice:'面议'}}</text>
								<text class="tex">批发价</text>
							</view>
							<view class="oncePrice" v-if="item.WholePrice>0 || item.Price>0">
								<text class="num" v-if="item.Price>0">￥{{item.Price}}</text>
								<text class="num" v-else>面议</text>
								<text class="tex">单价</text>
							</view>
							<view class="nullPrice" v-if="item.WholePrice==0 & item.Price==0">
								<text class="tex">价格面议</text>
								<!-- <text class="num"></text> -->

							</view>
						</view>
						<!-- <view class="des">
							{{item.detailText==undefined?'该产品没有添加文字描述':item.detailText}}
						</view> -->
					</view>
				</navigator>
			</view>
			<empty v-if="status!='loading'&& list.length==0" imgsrc="/static/empty/content_empty.png" txt="暂无内容">
			</empty>
			<view v-if="status=='loading' && list.length==0" style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>
			<view v-if="list.length>0" style="padding-top: 20rpx;">
				<uni-load-more iconType="circle" :showText="true" :status="status" />
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getProductList
	} from '@/api/product.js'
	export default {
		data() {
			return {
				searchVal: null, //搜索输入框的值
				styles: {
					color: '#5099FF',
					borderColor: '#5099FF'
				},
				historyList: [],
				userInfo: null,
				list: [],
				orgId: '', //企业id
				page: 1, //设置页码
				status: 'loading', //
				hashitems: [],
				pageSize: 15, //每页的数据的数量
				orgid: 0, //企业id
				cardId: 0, //名片id
				isSearch: false,
				uid:null,//用户id
			};
		},
		async onLoad(options) {
			this.orgid = parseInt(options.orgid || 0);
			this.cardId = parseInt(options.cardId || 0);
			this.uid = parseInt(options.uid || 0);
			this.reloadPages()
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			async reloadPages() {
				this.userInfo = await this.$store.dispatch("userInfo");
				// console.log("本登录用户", this.userInfo);
				this.orgId = this.orgid; //企业id
				this.getHistoryList()

			},
			choiceHistory(hitem) {
				this.searchVal = hitem;
				this.iconClick()
			},
			cancel() {
				this.isSearch = false
				this.searchVal = null;
				this.list = [];
				this.hashitems = {};
				this.$forceUpdate()
			},
			clearHistory() { //清楚历史记录
				uni.showModal({
					title: '提示',
					content: "确定要清除历史记录吗?",
					success: (res) => {
						if (res.confirm) {
							this.historylist = [];
							uni.setStorageSync('proSearchList' + this.userInfo.Id, []);
							this.getHistoryList();
							this.$forceUpdate()
						}
					}
				});



			},
			getHistoryList() {
				this.historyList = uni.getStorageSync('proSearchList' + this.userInfo.Id) || [];
				// console.log("历史搜索记录", this.historyList);
			},
			iconClick() { //点击图标或者失去焦点的时候执行
				this.isSearch = true
				if (this.searchVal == '' || this.searchVal == null) {
					this.list = [];
					this.hashitems = {};
					this.isSearch = false
					return
				}
				if (this.historyList.indexOf(this.searchVal) == -1 && this.searchVal != null && this.searchVal != '') {
					this.historyList.unshift(this.searchVal);
					if (this.historyList.length > 50) {
						this.historyList.pop()
					}
					uni.setStorageSync('proSearchList' + this.userInfo.Id, this.historyList);
				} else if (this.historyList.indexOf(this.searchVal) > -1 && this.searchVal != null && this.searchVal !=
					'') { //如果已经存在搜索历史中，重新搜索将这个搜索关键词放在第一个
					// console.log("搜索记录已经存在");
					for (let i = 0; i < this.historyList.length; i++) {
						if (this.historyList[i] == this.searchVal) {
							// console.log("到了删除这一步",this.historyList[i]);
							this.historyList.splice(i, 1) //先把符合条件的数据从当前数组中删除
							break;
						}
					}
					this.historyList.unshift(this.searchVal) //通过unshift函数把符合要求的数据放到第一位
					uni.setStorageSync('proSearchList' + this.userInfo.Id, this.historyList);
				}

				this.getHistoryList();
				this.page = 1;
				this.status = "loading";
				this.list = [];
				this.hashitems = {};
				this.load_data();
			},
			//加载产品列表
			async load_data() {
				let data = {
					orgId: this.orgId,
					pageSize: this.pageSize,
					pageNum: this.page,
					showAll: true,
					del_flag: 0,
					key: this.searchVal,
					CardId: this.cardId
				};
				// console.log("访问产品列表传递的参数", data);
				let rsp = await getProductList(data);
				// console.log("产品列表数据", rsp);
				rsp.data.List.forEach(it => {
					// console.log("产品列表",it);
					it["Id"] = it.Id.toString();
					if (!this.hashitems[it.Id]) {
						if (it.Detail != null || it.Detail != '') {
							let parseDetail = JSON.parse(it.Detail);
							// console.log("parseDetail", parseDetail);
							parseDetail.forEach(val => {
								let detailObj = {};
								if (val.type == 'text') {
									detailObj = {
										detailText: val.data //将产品详情从json字符串转化为数组，然后将文字描述取出来
									}
								}
								Object.assign(it, detailObj)
							})

							// console.log("it", it);

						}
						this.hashitems[it.Id] = true;
						this.list.push(it); //注意this.productList是数组，it是对象
					}
				});
				// console.log("最后查询到的产品列表", this.productList);
				// console.log("查询到的数据的续长度",rsp.data.List.length,this.pageSize);
				if (rsp.data.List.length < this.pageSize) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}

			},

		}
	}
</script>

<style lang="scss">
	page {
		background-color: #ffffff;
	}

	.search {
		padding: 0 20rpx;
		width: 710rpx;
		display: flex;
		align-items: center;
		justify-content: space-between;
		// background-color: #EFEFEF;
		border-radius: 8rpx;
		position: fixed;
		padding-bottom: 20rpx;
		background-color: #ffffff;

		input {
			height: 68rpx !important;
		}

		.search_icon {
			width: 600rpx;
			margin: 0;
			padding: 0;
			// input{
			// 	border: 1rpx solid #5099FF !important;
			// 	color: #5099FF;
			// }
		}

		.cancel {
			width: 100rpx;
			// padding: 0 15rpx;
			font-size: 36rpx;
			color: #5099FF;
			text-align: center;
		}
	}

	.topheight-zw {
		height: 90rpx;
	}

	.ls-wrap {
		display: flex;
		flex-direction: column;
		padding: 0 30rpx;
		background-color: #fff;

		.ls-t {
			display: flex;
			flex-direction: row;
			justify-content: space-between;
			height: 90rpx;
			align-items: center;

			.l {
				font-size: 30rpx;
				font-weight: bold;
			}

			van-icon {
				width: 44rpx;
				height: 44rpx;
			}
		}

		.ls-c {
			display: flex;
			flex-direction: row;
			flex-wrap: wrap;
			padding: 20rpx 0;

			.c-item {
				background-color: #f7f8fa;
				border-radius: 10rpx;
				padding: 6rpx 20rpx;
				margin: 20rpx 0;
				font-size: 24rpx;
				margin-right: 20rpx;
			}
		}
	}

	.list {
		width: 690rpx;
		padding: 0 30rpx;
		margin-top: 40rpx;
		border-radius: 8rpx;

		.item {
			width: 100%;
			height: auto;
			background-color: #fff;
			overflow: hidden;
			border-radius: 8rpx;
			box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(0, 0, 0, 0.0500);
			margin-bottom: 30rpx;

			navigator {
				width: 100%;
				min-height: 120rpx;
				box-sizing: border-box;
				display: flex;
				padding: 20rpx 30rpx;
				border-bottom: 1rpx solid #F6F6F6;

				.left {
					text {
						width: 140rpx;
						height: 140rpx;
						line-height: 140rpx;
						text-align: center;
						border-radius: 8rpx;
						background-color: #F1F2F3;
						font-size: 22rpx;
						color: #999999;
						display: block;
					}

					image {
						display: block;
						width: 140rpx;
						height: 140rpx;
						border-radius: 8rpx;
					}
				}

				.right {
					margin-left: 30rpx;
					display: flex;
					flex-direction: column;
					justify-content: space-between;

					.title {
						margin-top: 10rpx;
						font-size: 30rpx;
						// color: #333333;
						// font-family: pfzho;
						margin-bottom: 10rpx;
						color: rgba(51, 51, 51, 1);
						font-weight: 700;
						height: 70rpx;
						line-height: 35rpx;
						vertical-align: top;
					}

					.price {
						display: flex;
						justify-content: flex-start;
						flex-direction: row;
						align-items: flex-end;

						.wholePrice {
							margin-right: 20rpx;
							font-size: 30rpx;
							color: rgba(51, 51, 51, 1);
							font-weight: 600;

							.priceIcon {
								font-size: 22rpx;
							}

							.tex {
								font-size: 22rpx;
								margin-left: 6rpx;
							}
						}

						.nullPrice {
							font-size: 26rpx;
							color: rgba(51, 51, 51, 1);
							font-weight: 600;
							// color: red;
						}

						.oncePrice {
							font-size: 22rpx;
							color: rgba(153, 153, 153, 1);

							.tex {
								margin-left: 6rpx;
							}
						}
					}

					.des {
						font-size: 24rpx;
						color: #999999;
					}
				}
			}

		}
	}
</style>
