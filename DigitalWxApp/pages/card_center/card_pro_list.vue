<template>
	<view>
		<view class="search">
			<view class="nothing">
			</view>
			<view class="search_icon">
				<!-- <uni-search-bar class="my-search-bar" @confirm="search" radius="5" :focus="true" cancelButton="none" bgColor="#FFFFFF"
					clearButton="none" v-model="searchVal" placeholder="搜索词">
					<uni-icons slot="searchIcon" color="#999999" size="22" type="search" />
				</uni-search-bar> -->
				<uni-easyinput class="my-search-bar" :focus="false" :styles="styles" prefixIcon="search" clearSize="16" :clearable="false"
					v-model.lazy="searchVal" confirm-type="search" @confirm="iconClick" placeholder="搜索词" @input="iconClick"
					@iconClick="iconClick">
				</uni-easyinput>
			</view>
			<view class="ls-wrap">
				<view class="ls-t">
					<view class="l">产品列表</view>
					<!-- <van-icon name="delete-o" color="#333" size="44" @click="clearHistory" /> -->
					<view class="paixu" @click="priceSort">
						<view class="tex">
							<text>按批发价排序</text>
						</view>
						<view class="paixu_icon">
							<uni-icons custom-prefix="my-icon" type="my-icon-shangjiantou" size="8" :color="sort==1?'#333333':'#999999'">
							</uni-icons>
							<uni-icons custom-prefix="my-icon" type="my-icon-xiajiantou" size="8" :color="sort==0?'#333333':'#999999'">
							</uni-icons>
						</view>
					</view>
				</view>
				<!-- <empty imgsrc="/static/empty/content_empty.png" txt="暂无搜索内容"></empty> -->
			</view>
			<!-- <view class="cancel" @click="cancel">取消</view> -->
		</view>
		<view class="topheight-zw"></view>
		
		<view class="list">
			<view v-if="list.length>0" class="item_item" v-for="item in list" :key="item.Id">
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
					</view>
				</navigator>
			</view>
			<empty v-if="status!='loading'&& list.length==0" imgsrc="/static/empty/content_empty.png" txt="暂无内容">
			</empty>
			<view v-if="list.length>0" style="padding-top: 20rpx;">
				<uni-load-more iconType="circle" :showText="false" :status="status" />
			</view>
			<view v-if="status=='loading'&& list.length==0" style="padding-bottom: 10rpx;width: 100%;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
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
				searchVal: '', //搜索输入框的值
				styles: {
					color: '#5099FF',
					borderColor: '#5099FF'
				},
				list: [],
				orgId: 0, //企业id
				page: 1, //设置页码
				status: 'loading', //
				hashitems: [],
				pageSize: 15, //每页的数据的数量
				orgid: 0, //企业id
				cardId: 0, //名片id
				uid: 0, //用户id
				classId: 0, //产品类别id
				sort: 1,
				isSort:false
			};
		},
		async onLoad(options) {
			// console.log("传递的值", options);
			if (options.id) {
				this.classId = parseInt(options.id)
			}
			if (options.uid) {
				this.uid = parseInt(options.uid)
			}
			if (options.cardId) {
				this.cardId = parseInt(options.cardId)
			}
			uni.setNavigationBarTitle({
				title: this.$store.state.proListTitle
			});
			// console.log("标题",this.$store.state.proListTitle);
			this.reloadPages()
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data(this.isSort);
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			priceSort() {
				//按价格排序
				if(this.sort==1){
					this.sort=0
				}else{
					this.sort=1
				}
				this.isSort=true
				this.page = 1;
				this.status = "loading";
				this.list = [];
				this.hashitems = {};
				this.load_data(true)
			},
			async reloadPages() {
				
				await this.load_data();

			},
			cancel() {
				this.searchVal = '';
				this.list = [];
				this.status = "loading";
				this.$forceUpdate()
			},

			iconClick() { //点击图标或者失去焦点的时候执行

				this.page = 1;
				this.status = "loading";
				this.list = [];
				this.hashitems = {};
				this.load_data();
			},
			//加载产品列表
			async load_data(isSort) {

				let data = {
					pageSize: this.pageSize,
					pageNum: this.page,
					showAll: true,
					del_flag: 0,
					key: this.searchVal,
					CardId: this.cardId,
					CategoryId: this.classId,
				};
				if (isSort) {
					data = {
						pageSize: this.pageSize,
						pageNum: this.page,
						showAll: true,
						del_flag: 0,
						key: this.searchVal,
						CardId: this.cardId,
						CategoryId: this.classId,
						orderByColumn:'WholePrice',
						isAsc:this.sort==1?'asc':'desc'
					};
				}
				let rsp = await getProductList(data);
				// console.log("产品列表数据", rsp);
				rsp.data.List.forEach(it => {
					// console.log("产品列表",it);
					it["Id"] = it.Id.toString();
					if (!this.hashitems[it.Id]) {
						
						this.hashitems[it.Id] = true;
						this.list.push(it); //注意this.productList是数组，it是对象
					}
				});
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
		// display: flex;
		// align-items: center;
		// background-color: #EFEFEF;
		border-radius: 8rpx;
		position: fixed;
		background-color: #fff;

		input {
			height: 68rpx !important;
		}
		.nothing{
			width: 710rpx;
			height: 40rpx;
			background-color: #ffffff;
		}
		.search_icon {
			width: 100%;
			box-sizing: border-box;
			margin: 0;
			padding: 0;
			

			// border-radius: 12rpx;
			.uni-easyinput__content {
				border-radius: 12rpx !important;
				background-color: #EFEFEF !important;
				border: none !important;
			}

			input {
				// border: 1rpx solid #5099FF !important;
				// color: #5099FF;
				
				
			}
		}

		.cancel {
			width: 100rpx;
			// padding: 0 15rpx;
			font-size: 36rpx;
			color: #1677FF;
			text-align: center;
		}
	}

	.topheight-zw {
		height: 180rpx;
	}

	.ls-wrap {
		// display: flex;
		// flex-direction: column;
		padding: 0 30rpx;
		background-color: #fff;
		margin-top: 42rpx;
		background-color: #fff;
		padding-bottom: 40rpx;

		.ls-t {
			display: flex;
			flex-direction: row;
			// justify-content: flex-end;
			justify-content: space-between;
			height: 28rpx;
			align-items: center;

			.l {
				font-size: 28rpx;
				// font-weight: bold;
				color: rgba(153, 153, 153, 1);

			}

			.paixu {
				display: flex;
				flex-direction: row;
				justify-content: flex-end;
				align-items: center;
				height: 28rpx;

				.tex {
					margin-right: 10rpx;
					font-size: 28rpx;
					height: 28rpx;
					line-height: 28rpx;

				}

				.paixu_icon {
					height: 28rpx;
					line-height: 10rpx;
					display: flex;
					flex-direction: column;
					justify-content: space-around;
					align-items: center;
				}
			}
		}

	}

	.list {
		width: 690rpx;
		padding: 0 30rpx;
		margin-top: 40rpx;
		border-radius: 8rpx;

		.item_item {
			width: 100%;
			height: auto;
			background-color: #fff;
			overflow: hidden;
			border-radius: 8rpx;
			box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(239, 241, 246, 1);
			margin-bottom: 30rpx;

			navigator {
				width: 100%;
				min-height: 120rpx;
				box-sizing: border-box;
				display: flex;
				padding: 20rpx 30rpx;
				// border-bottom: 1rpx solid #F6F6F6;

				.left {
					text {
						width: 150rpx;
						height: 150rpx;
						line-height: 150rpx;
						text-align: center;
						border-radius: 8rpx;
						background-color: #F1F2F3;
						font-size: 22rpx;
						color: #999999;
						display: block;
					}

					image {
						display: block;
						width: 150rpx;
						height: 150rpx;
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
						// font-family: pfzho;
						margin-bottom: 10rpx;
						color: rgba(51, 51, 51, 1);
						font-weight: 700;
						height: 70rpx;
						line-height: 35rpx;
						vertical-align: top;
						display: -webkit-box;
						/*弹性伸缩盒子模型显示*/
						-webkit-box-orient: vertical;
						/*排列方式*/
						-webkit-line-clamp: 2;
						/*显示文本行数*/
						text-overflow: ellipsis;
						overflow: hidden;
						/*溢出隐藏*/
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
						.nullPrice{
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
				}
			}

		}
	}
</style>
