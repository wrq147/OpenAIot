<template>
	<view class="wrap">
		<view class="nav">
			<view class="item">
				<navigator class="navigator" :url="'/pages/user/pro_add?id='+orgId">
					<image class="image" src="../../static/user/picon1.png" mode="aspectFill"></image>
					<text class="text">添加产品</text>
				</navigator>
			</view>
			<view class="item">
				<navigator class="navigator" url="/pages/user/pro_settings">
					<image class="image" src="../../static/user/picon2.png" mode="aspectFill"></image>
					<text class="text">设置</text>
				</navigator>
			</view>
			<view class="item">
				<navigator class="navigator" :url="'/pages/user/pro_recycle?orgId='+orgId">
					<image class="image" src="../../static/user/picon3.png" mode="aspectFill"></image>
					<text class="text">回收站</text>
				</navigator>
			</view>
			<view class="item">
				<navigator class="navigator" url="/pages/user/pro_class">
					<image class="image" src="../../static/user/picon4.png" mode="aspectFill"></image>
					<text class="text">产品分类</text>
				</navigator>
			</view>
		</view>
		<view class="list" v-if="productList.length>0">
			<view v-if="productList.length>0" class="item" v-for="item in productList" :key="item.Id">
				<navigator class="navigator"
					:url="'/pages/card_center/card_pro_detail?id='+item.Id+'&uid='+userInfo.Id">
					<view class="left" v-if="item.ImageUrl">
						<!-- <image src="../../static/logo.png"></image> -->
						<image class="image" :src="item.ImageUrl" mode="aspectFill"></image>
					</view>
					<view class="left" v-else>
						<text class="text">产品照片</text>
					</view>
					<view class="right">
						<view class="title">
							{{item.ProName}}
						</view>
						<!-- <view class="des">
							{{item.detailText==undefined?'该产品没有添加文字描述':item.detailText}}
						</view> -->
						<view class="price">
							<view class="whole" v-if="item.WholePrice>0 || item.Price>0">
								<!-- <text class="text">批发价</text> -->
								<text class="fuhao" v-if="item.WholePrice>0">￥</text>
								<text class="num">{{item.WholePrice==0?'面议':item.WholePrice}}</text>
								<text class="text">批发价</text>
							</view>
							<view class="unit" v-if="item.WholePrice>0 || item.Price>0">
								<text class="text">单价 </text>
								<!-- <text class="text" v-if="item.Price==0">:</text> -->
								<text class="fuhao" v-if="item.Price>0">￥</text>
								<text class="num">{{item.Price==0?':面议':item.Price}}</text>
							</view>
							<view class="prinull" v-if="item.WholePrice==0 && item.Price==0">
								<text class="text">价格:</text>
								<text class="fuhao">面议</text>
								<!-- <text class="text">批发价</text> -->
							</view>
						</view>
					</view>

				</navigator>
				<view class="bot">
					<view class="view">
						<navigator class="navigator" :url="'/pages/user/pro_edit?orgId='+orgId+'&id='+item.Id">
							编辑
						</navigator>
					</view>
					<view class="view" @click.stop="deleteProFun(item.Id)">
						删除
					</view>
				</view>
			</view>

			<view v-if="productList.length>0" style="padding-top: 20rpx;">
				<uni-load-more iconType="circle" :showText="false" :status="status" />
			</view>
		</view>
		<empty v-else-if="status!='loading'&&productList.length==0" imgsrc="/static/empty/content_empty.png" txt="暂无内容">
		</empty>
		<view v-else style="padding-bottom: 10rpx;">
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
		</view>
	</view>
</template>

<script>
	import {
		getProductList,
		deletePro
	} from '@/api/product.js'
	export default {
		data() {
			return {
				productList: [],
				orgId: '',
				page: 1, //设置页码
				status: 'loading', //
				hashitems: [],
				pageSize: 30,
				userInfo: null
			}
		},
		async onLoad(options) {
			this.userInfo = await this.$store.dispatch("userInfo");
			this.orgId = options.orgId;
		},
		onShow() {
			this.reloadpage();
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
			async reloadpage() {
				this.page = 1;
				this.status = "loading";
				this.productList = [];
				this.hashitems = {};
				this.load_data(); //加载产品列表
			},
			//加载产品列表
			async load_data() {
				let data = {
					orgId: this.orgId,
					pageSize: this.pageSize,
					pageNum: this.page,
					showAll: true,
					del_flag: 0,
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
						this.productList.push(it); //注意this.productList是数组，it是对象
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
			//删除操作点击事件
			deleteProFun(id) {
				// console.log("点击了删除按钮", id);
				uni.showModal({
					title: '提示',
					content: '确定删除该产品吗？',
					success: async res => {
						if (res.confirm) {
							// this.$refs.popup.close();
							uni.showLoading({
								title: '加载中...'
							});
							try {
								await deletePro(id);
								await this.reloadpage();
								setTimeout(() => {
									uni.showToast({
										icon: 'success',
										title: '删除成功'
									});
								}, 200)
							} catch (err) {
								console.info('异常：', err);
							} finally {
								uni.hideLoading();
							}
						}
					}
				});
			},

		}
	}
</script>

<style lang="scss">
	.wrap {
		padding: 30rpx;
		width: 100%;
		box-sizing: border-box;

		.nav {
			width: 100%;
			height: 228rpx;
			background-color: #FFFFFF;
			box-shadow: 0rpx -4rpx 10rpx 0rpx rgba(0, 0, 0, 0.0500), 0rpx 4rpx 10rpx 0rpx rgba(0, 0, 0, 0.0500);
			border-radius: 8rpx;
			display: flex;
			justify-content: space-around;
			align-items: center;

			.item {
				width: 20%;
				text-align: center;

				.navigator {
					width: 100%;
					text-align: center;

					.image {
						width: 88rpx;
						height: 88rpx;
						border-radius: 50%;
						display: block;
						margin: 0 auto;
						margin-bottom: 20rpx;
					}

					.text {
						color: #666666;
						font-size: 22rpx;
					}
				}
			}
		}

		.list {
			width: 100%;
			margin-top: 40rpx;
			border-radius: 8rpx;

			.item::after {
				content: "";
				display: block;
				clear: both;
			}

			.item {
				width: 100%;
				height: auto;
				background-color: #fff;
				overflow: hidden;
				border-radius: 8rpx;
				box-shadow: 0rpx -4rpx 10rpx 0rpx rgba(0, 0, 0, 0.0500), 0rpx 4rpx 10rpx 0rpx rgba(0, 0, 0, 0.0500);
				margin-bottom: 30rpx;

				.navigator {
					width: 690rpx;
					min-height: 120rpx;
					box-sizing: border-box;
					display: flex;
					padding: 20rpx 30rpx;
					border-bottom: 1rpx solid #F6F6F6;

					.left {
						.text {
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

						.image {
							display: block;
							width: 140rpx;
							height: 140rpx;
							border-radius: 8rpx;
						}
					}

					.right {
						margin-left: 30rpx;
						width: 520rpx;
						height: 140rpx;
						padding: 10rpx 0;
						display: flex;
						flex-direction: column;
						justify-content: space-between;

						// align-items: flex-end;
						// vertical-align: bottom;
						.title {
							// margin-top: 10rpx;
							font-size: 28rpx;
							color: #333333;
							font-family: pfzho;
							// margin-bottom: 10rpx;
						}

						.des {
							font-size: 24rpx;
							color: #999999;
						}

						.price {
							// text-align: right;
							display: flex;
							justify-content: flex-start;
							align-items: flex-end;
							vertical-align: bottom;
							margin-top: 30rpx;
							margin-bottom: 20rpx;

							// font-family: '宋体';
							.unit {
								vertical-align: bottom;
								color: #979797;
								// display: inline-block;
								margin-right: 10rpx;
								font-size: 22rpx;

								.text {
									font-size: 22rpx;
									margin-right: 6rpx;
								}

								.fuhao {
									font-size: 22rpx;
									// width: 6rpx;
								}

								.num {
									font-size: 22rpx;
								}
							}

							.whole {
								vertical-align: bottom;
								color: red;
								// display: inline-block;
								font-size: 30rpx;
								margin-right: 30rpx;
								// background-color: red;
								// color: #fff;
								// border-radius: 28rpx;
								font-weight: 600;
								height: 30rpx;
								line-height: 30rpx;

								.text {
									font-size: 22rpx;
									font-weight: normal;
									color: #979797;
								}

								.fuhao {
									font-size: 22rpx;

								}

								.num {
									font-size: 30rpx;
									margin-right: 6rpx;
									vertical-align: bottom;
								}
							}

							.prinull {
								vertical-align: bottom;
								color: red;
								// display: inline-block;
								font-size: 30rpx;
								margin-right: 30rpx;
								// background-color: red;
								// color: #fff;
								// border-radius: 28rpx;
								font-weight: 600;

								.text {
									margin-right: 6rpx;
								}

								.fuhao {
									font-size: 30rpx;

								}
							}
						}
					}

				}

				.bot {
					height: 80rpx;
					display: flex;
					justify-content: flex-end;
					align-items: center;

					.view {
						font-size: 24rpx;
						color: #999999;
						border: 1rpx solid #979797;
						text-align: center;
						color: #979797;
						border-radius: 8rpx;
						width: 98rpx;
						height: 48rpx;
						line-height: 48rpx;
						font-weight: 400;
						margin-right: 30rpx;
						overflow: hidden;


						.navigator {
							display: block;
							font-size: 24rpx;
							color: #999999;
							height: 47rpx;
							line-height: 48rpx;
							width: 98rpx;
							padding: 0;
							min-height: 40rpx;
							box-sizing: border-box;
						}
					}

				}
			}
		}

	}
</style>
