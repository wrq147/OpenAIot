<template>
	<view>
		<view class="search" @click="toSearch" v-if="search">
			<uni-easyinput class="my-search-bar" placeholderStyle="color:#999" disabled :styles="styles"
				prefixIcon="search" clearSize="16" v-model.lazy="searchVal" placeholder="请输入关键词">
			</uni-easyinput>
		</view>
		<view class="carousel" v-if="lunboList.length>0">
			<swiper class="swiper" circular indicator-dots="true" autoplay="true" interval="3000" duration="500">
				<swiper-item v-for="url in lunboList">
					<image class="image" :src="url" mode="aspectFill"></image>
				</swiper-item>
			</swiper>
		</view>
		<view class="pro_class" v-if="showCategory && littleClass.length>0">
			<view class="classContent">
				<view class="classList">
					<view v-for="item3,index in littleClass" :key="item3.id" @click="getContent(index,item3.id)"
						:class="['classLi',clickIndex==index?'bold':'']">
						<text>{{item3.label}}</text>
						<view :class="['line1',clickIndex==index?'show':'']"></view>
					</view>
				</view>
				<navigator :url="'/pages/card_center/card_pro_class?orgid='+orgid+'&uid='+cardData.UserId+'&cardId='+cardData.Id"
					class="classTitle">
					<image class="image" src="../../static/icon_more.png" mode="aspectFill"></image>
					<text class="text">分类</text>
				</navigator>
			</view>
			<view class="contentList">
				<scroll-view v-if="littleList.length>0" :scroll-x="true" @scrolltolower="scrollBottom"
					class="scrollview-box">
					<block v-for="item in littleList" :key="item.Id">
						<navigator class="contentLi"
							:url="'/pages/card_center/card_pro_detail?id='+item.Id+'&uid='+cardData.UserId+'&cardId='+cardData.Id">
							<image v-if="item.ImageUrl" class="image" :src="item.ImageUrl" mode="aspectFill"></image>
							<view v-if="item.ImageUrl==''" class="noneImg">
								<text>产品照片</text>
							</view>
							<text class="text">{{item.ProName}}</text>
						</navigator>
					</block>
				</scroll-view>
				<view v-else-if="littleStatus!='loading'&&littleList.length==0"
					style="display: flex;flex-direction: column;align-items: center;">
					<image class="emptyImg" mode="aspectFill" src="/static/empty/content_empty.png" />
					<text class="emptyTxt">暂无内容</text>
				</view>
				<view v-else-if="littleStatus=='loading'&&littleList.length==0" style="padding-bottom: 10rpx;width: 100%;height: 100%">
					<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
					<!-- <J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton> -->
				</view>
			</view>
		</view>
		<view class="newPro" v-if="isNew">
			<view class="title">
				<text>新品推荐</text>
				<view class="line2"></view>
			</view>
			<view class="proContent">
				<view class="txt">
					<view class="textTitle">XX系列</view>
					<view class="txtList">
						<view class="txtLi">
							功能描述14444功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1
						</view>
						<view class="txtLi">
							功能描述14444功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1功能描述1
						</view>
						<view class="txtLi">
							功能描述3
						</view>
					</view>
				</view>
				<image class="image" src="../../static/kyj_big.png" mode="aspectFill"></image>
			</view>
		</view>
		<view class="popularPro" v-if="popList.length>0">
			<view class="popTop">
				<view class="title">
					<text class="text">热门产品</text>
					<view class="line2"></view>
				</view>
				<view class="screen" @click="openScreen" v-if="isChoice">
					<image class="image" src="../../static/icon_screen.png" mode="aspectFill"></image>
					<text class="text">筛选</text>
				</view>
			</view>
			<view class="popProList">
				<navigator :url="'/pages/card_center/card_pro_detail?id='+pop.Id+'&uid='+cardData.UserId+'&cardId='+cardData.Id"
					class="popProLi" v-for="pop in popList" :key="pop.Id">
					<image v-if="pop.ImageUrl" class="image" :src="pop.ImageUrl" mode="aspectFill"></image>
					<view v-else class="image">产品照片</view>
					<view class="text">{{pop.ProName}}</view>
					<view class="price">
						<view class="whole price_con" v-if="pop.WholePrice>0 || pop.Price>0">
							<text class="pricetitle">批发:</text>
							<text class="fuhao" v-if="pop.WholePrice>0">￥</text>
							<text class="pricenum" v-if="pop.WholePrice>0">{{pop.WholePrice}}</text>
							<text class="pricenum" v-else>面议</text>
						</view>
						<view class="unit price_con" v-if="pop.WholePrice>0 || pop.Price>0">
							<text class="pricetitle">单价:</text>
							<text class="fuhao" v-if="pop.Price>0">￥</text>
							<text class="pricenum fuhao" v-if="pop.Price>0">{{pop.Price}}</text>
							<text class="pricenum fuhao" v-else>面议</text>
						</view>
						<view class="price_con null_price" v-if="pop.WholePrice==0 && pop.Price==0">
							<text>价格面议</text>
						</view>
					</view>
				</navigator>
				<view style="width:100%;height: 60rpx;margin-top: 10rpx;">
					<uni-load-more iconType="circle" :showText="true" :status="popStatus" />
				</view>
			</view>
		</view>
		<view class="emptyclass"
			v-if="!search && lunboList.length==0 && !isNew && littleList.length==0 && popList.length==0 && (!showCategory || littleClass.length==0)&&littleStatus!='loading'">
			<empty imgsrc="/static/empty/content_empty.png" txt="暂无产品内容"></empty>
		</view>
		<view class="popup_con">
			<uni-popup ref="popup" background-color="#fff">
				<view class="popup-content">
					<view class="top">
						<image class="image" src="../../static/icon_close.png" mode="aspectFill"></image>
					</view>
					<scroll-view :scroll-y="true" class="popup_scroll">
						<block v-for="item2,index in classList" :key="item2">
							<view :class="['caList',choiceIndex==index?'active':'']"
								@click="choiceClass(index,item2.Id)">
								{{item2.CategoryName}}
							</view>
						</block>
					</scroll-view>
				</view>
			</uni-popup>
		</view>
	</view>
</template>

<script>
	import {
		getProClassList,
		getProductList,
		getClassTree
	} from '@/api/product.js'
	import {
		getOrg
	} from '@/api/org.js'
	import request from '@/common/request.js'
	import {
		procf
	} from '@/mixin/mixin.js'
	export default {
		props: {
			orgid: {
				type: Number,
				default: 0
			},
			cardData: {
				type: Object,
				default: null
			}
		},
		mixins: [procf], //混入文件
		data() {
			return {
				styles: {
					color: '#999999',
					disableColor: '#EFEFEF',
					borderColor: '#EFEFEF'
				}, //搜索框的样式
				searchVal: null, //搜索输入框的值
				// userInfo: null, //用户登录信息
				classList: null, //产品分类列表
				popList: null, //热门产品处的产品列表
				popItems: {},
				popPage: 1, //热门产品页码
				popStatus: 'loading', //热门产品
				littleClass: [], //只取四个分类
				activeClassId: null, //第一个列表id
				littleList: [], //只取四个分类处的产品列表
				littleItems: {},
				littlePage: 1, //横向滚动页码
				littleStatus: 'loading',
				clickIndex: 0,
				choiceIndex: 0,
				list: ['1', '2', '3', '4', '5', '1', '2', '3', '4', '5'],
				classId: null, //类别id
				isNew: false, //是否上新
				isChoice: false, //是否进行筛选
				dpmap: new Map(),
				infoClass: 'pro' //当前滑动展示的是类别还是商品商品用"pro"类别用"class"
			}
		},
		mounted() {
			this.reloadPages();
		},

		methods: {
			toProList(item) {
				this.$store.commit('SET_PROLISTTITLE_DATA', item.ProName);
				uni.navigateTo({
					url: '/pages/card_center/card_pro_list?id=' + item.id +'&uid='+cardData.UserId+'&cardId='+cardData.Id
				})
			},
			reachBottom() {
				if (this.popStatus != 'noMore') {
					this.popPage++;
					this.popStatus = "loading";
					this.getProList(null, 'popClass', 9, this.popPage);
					// console.log("现在是第几页", this.page);
				}
			},
			toSearch() {
				uni.navigateTo({
					url: '/pages/card_center/card_pro_search?orgid=' + this.orgid +'&uid='+this.cardData.UserId+'&cardId='+this.cardData.Id
				})
			},
			getPopPro() { //获取热门产品
				this.popPage = 1;
				this.popStatus = "loading";
				this.popList = [];
				this.popItems = {};
				this.getProList(null, 'popClass', 9, this.popPage)
			},
			async reloadPages() {
				//获取产品设置信息
				let orgInfo = await getOrg(this.orgid);
				if (orgInfo.data == null || orgInfo.data.del_flag == "2") {
					uni.showToast({
						title: '名片所属企业已不存在',
						icon: "none",
						duration: 2000
					});
					return;
				}
				this.setProConfig(orgInfo.data.ProConfig)
				//获取产品设置信息

				let res = await getClassTree({
					OrgId: this.orgid,
					CardId: this.cardData.Id
				})
				this.dpmap = new Map();
				this.classList = res.data;
				// console.log("产品分类列表", res);
				let data = Array.from(res.data); //将数组设置为真正的数组
				data.forEach((val, i) => {
					// console.log("9999");
					if (i == 0) {
						this.activeClassId = val.id;
					}
					if (i < 4) {
						this.littleClass.push(val);
					}
				})
				// console.log("littleClass", this.activeClassId, this.littleClass);
				await this.getContent(0, this.activeClassId) //第一个类别id获取后执行查询

				this.getPopPro(); //页面初始加载时获取热门产品

				//初始加载页面
			},
			scrollBottom() { //横向滚动触底了执行
				if (this.littleStatus != 'noMore') {
					this.littlePage++;
					this.littleStatus = "loading";
					this.getProList(this.activeClassId, 'fouthClass', 10, this.littlePage);
					// console.log("现在是第几页", this.page);
				}

			},
			async getProList(classId, choice, pageSize, page) {
				let data = {};
				if (classId) {
					data = {
						orgId: this.orgid,
						pageSize: pageSize,
						pageNum: page,
						showAll: true,
						del_flag: 0,
						CardId: this.cardData.Id,
						CategoryId: classId,
					}
				} else {
					data = {
						orgId: this.orgid,
						pageSize: pageSize,
						pageNum: page,
						showAll: true,
						CardId: this.cardData.Id,
						del_flag: 0,
					}
				}
				// console.log("查询列表传参",data);
				try {
					let rsp = await getProductList(data);
					// console.log("产品列表数据", rsp);
					rsp.data.List.forEach(it => {
						// console.log("产品列表",it);

						it["Id"] = it.Id.toString();
						if (choice == 'popClass') { //choice用于区分是从哪里传递过来的产品分类
							if (!this.popItems[it.Id]) {

								this.popItems[it.Id] = true;
								this.popList.push(it);
							}
						} else if (choice == 'fouthClass') {
							if (!this.littleItems[it.Id]) {
								this.littleItems[it.Id] = true;
								this.littleList.push(it);

							}
						}
					});
					if (choice == 'popClass') { //choice用于区分是从哪里传递过来的产品分类
						if (rsp.data.List.length < pageSize) {
							this.popStatus = 'noMore';
						} else {
							this.popStatus = 'more';
						}
					} else if (choice == 'fouthClass') {
						if (rsp.data.List.length < pageSize) {
							this.littleStatus = 'noMore';
						} else {
							this.littleStatus = 'more';
						}
					}
				} catch (e) {

				}

			},
			choiceClass(index, id) {
				this.choiceIndex = index;
			},
			async openScreen() { //打开筛选的弹出层
				this.$refs.popup.open('left');


			},
			async getContent(index, id) { //点击每一个产品分类
				this.clickIndex = index;
				// console.log("点击的类别id", id);
				this.activeClassId = id;
				this.littlePage = 1;
				this.littleStatus = "loading";
				this.littleList = [];
				this.littleItems = {};
				await this.getProList(this.activeClassId, 'fouthClass', 10, this.littlePage);
				// console.log("只有四个类别的点击事件查询到的产品列表", this.littleList);
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.emptyclass {
		//数据都为空的时候显示的样式
		height: 100vh;
		box-sizing: border-box;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.search {
		padding: 20rpx 30rpx;
		width: 690rpx;
		// background-color: #EFEFEF;
		border-radius: 8rpx;

	}

	.carousel {
		margin: 0 30rpx 10rpx;
		// padding: 0 10rpx;
		height: 330rpx;
		width: 690rpx;
		box-sizing: border-box;
		border-radius: 8rpx;
		background-color: #EFEFEF;
		border-radius: 8rpx;

		.swiper {
			height: 330rpx;
			border-radius: 8rpx;

			.image {
				border-radius: 8rpx;
				height: 330rpx;
				width: 690rpx;
			}
		}
	}

	.pro_class {
		margin: 20rpx 30rpx;
		width: 690rpx;
		height: 330rpx;
		border-radius: 8rpx;
		background-color: #FFFFFF;
		box-shadow: 0 0 20rpx rgba(229, 229, 229, 1);

		.classContent {
			width: 690rpx;
			// display: flex;
			// align-items: center;
			// justify-content: space-between;
			height: 90rpx;
			font-size: 30rpx;
			border-bottom: 2rpx solid #EFEFEF;

			// line-height: 80rpx;
			.classList::after {
				display: block;
				content: "";
				clear: both;
			}

			.classList {
				padding: 0 15rpx;
				color: #6B6B6B;
				height: 90rpx;
				line-height: 90rpx;
				// position: relative;
				float: left;
				width: 490rpx;
				text-align: center;



				.classLi {
					// padding: 0 10rpx;
					text-align: center;
					height: 90rpx;
					position: relative;
					font-weight: 100;
					// width: 102rpx;
					width: 25%;
					box-sizing: border-box;
					float: left;
					text-overflow: ellipsis;
					white-space: nowrap;
					overflow: hidden;
					font-size: 28rpx;
					// float: left;
					display: inline-block;

					.line1 {
						width: 64rpx;
						height: 4rpx;
						background-color: #46ADFA;
						position: absolute;
						bottom: 0;
						left: 29.25rpx;
						display: none;
					}

					.show {
						display: block;
					}
				}

				.bold {
					font-weight: 600;


				}
			}

			.classTitle {
				width: 120rpx;
				padding: 0 25rpx;
				display: flex;
				align-items: center;
				justify-content: space-between;
				height: 90rpx;
				color: #808080;
				border-left: 2rpx solid #EFEFEF;
				float: right;
				// display: inline-block;

				.image {
					width: 46rpx;
					height: 56rpx;
					margin-right: 10rpx;
				}
			}
		}

		.contentList {
			height: 190rpx;
			// display: flex;
			// align-items: center;
			// justify-content: space-between;
			padding: 30rpx 30rpx 20rpx 30rpx;

			.scrollview-box {
				display: flex;
				align-items: center;
				justify-content: space-between;
				white-space: nowrap;
				height: 190rpx;

				.contentLi {
					width: 160rpx;
					text-align: center;
					margin-right: 30rpx;
					display: inline-flex; // item的外层定义成行内元素才可进行滚动 inline-block / inline-flex 均可
					flex-direction: column;
					align-items: center;
					vertical-align: top;
					// justify-content: space-around;
					height: 190rpx;

					.noneImg {
						width: 160rpx;
						box-sizing: border-box;
						padding: 45rpx;
						height: 140rpx;
						line-height: 30rpx;
						font-size: 26rpx;
						color: #666666;
						margin-bottom: 10rpx;
						border: 1rpx dashed #999999;
						display: flex;
						justify-content: center;
						flex-wrap: wrap;
						align-items: center;
						white-space: pre-wrap;

					}

					.image {
						width: 160rpx;
						height: 140rpx;
						margin-bottom: 10rpx;
						// margin-top: 20rpx;
					}

					.text {
						width: 160rpx;
						height: 40rpx;
						line-height: 40rpx;
						font-size: 26rpx;
						text-overflow: ellipsis;
						white-space: nowrap;
						overflow: hidden;

					}

				}
			}

			.emptyImg {
				width: 100rpx;
				height: 100rpx;
			}

			.emptyTxt {
				color: #666666;
				line-height: 40rpx;
			}


		}
	}

	.title {
		height: 42rpx;
		margin-bottom: 20rpx;
		font-size: 30rpx;
		font-weight: 800;
		width: 120rpx;
		position: relative;
		padding-left: 2rpx;



		.line2 {
			width: 98%;
			height: 10rpx;
			border-radius: 8rpx;
			background-color: #46ADFA;
			position: absolute;
			left: 0;
			bottom: 6rpx;
			z-index: -1;
		}
	}

	.newPro {
		margin: 40rpx 30rpx;
		width: 690rpx;

		.proContent {
			width: 610rpx;
			padding: 0 40rpx;
			height: 340rpx;
			border-radius: 8rpx;
			background-color: #FFFFFF;
			box-shadow: 0 0 20rpx rgba(229, 229, 229, 1);
			display: flex;
			align-items: center;
			justify-content: space-between;

			.txt {
				width: 322rpx;
				height: 240rpx;
				color: rgba(102, 102, 102, 1);

				.textTitle {
					font-size: 28rpx;
					margin-bottom: 10rpx;
					height: 40rpx;
					line-height: 40rpx;
				}

				.txtList {
					display: flex;
					align-items: center;
					justify-content: space-between;

					.txtLi {
						width: 102rpx;
						margin-right: 12rpx;
						font-size: 20rpx;
						height: 200rpx;
						line-height: 33rpx;
						display: -webkit-box;
						/*弹性伸缩盒子模型显示*/
						-webkit-box-orient: vertical;
						/*排列方式*/
						-webkit-line-clamp: 6;
						/*显示文本行数*/
						text-overflow: ellipsis;
						overflow: hidden;
						/*溢出隐藏*/
					}
				}
			}

			.image {
				width: 267rpx;
				height: 260rpx;
			}
		}

	}

	.popularPro {
		padding: 0 30rpx;

		.popTop {
			display: flex;
			align-items: center;
			justify-content: space-between;
			margin-bottom: 10rpx;

			.screen {
				display: flex;
				align-items: center;
				justify-content: space-between;

				.image {
					width: 36rpx;
					height: 36rpx;
					margin-right: 10rpx;
				}

				.text {
					font-size: 28rpx;
				}
			}
		}

		.popProList {
			display: flex;
			align-items: center;
			justify-content: space-between;
			flex-flow: row wrap;

			width: 100%;
			box-sizing: border-box;

			.popProLi:nth-child(2n-1) {
				margin-right: 2.4%;
			}

			.popProLi {
				// width: 285rpx;
				width: 48.8%;
				box-sizing: border-box;
				padding: 0 10rpx;
				// padding: 30rpx 5rpx 20rpx;
				height: 370rpx;
				border-radius: 8rpx;
				background-color: #FFFFFF;
				display: flex;
				align-items: center;
				justify-content: flex-start;
				flex-flow: row wrap;
				justify-content: center;
				box-shadow: 0 0 20rpx rgba(229, 229, 229, 1);
				margin-bottom: 20rpx;

				.image {
					margin-top: 10rpx;
					width: 100%;
					box-sizing: border-box;
					height: 240rpx;
					display: block;
					line-height: 240rpx;
					text-align: center;
					font-size: 30rpx;

				}

				.text {
					display: block;
					margin: 10rpx 0;
					// width: 285rpx;
					width: 100%;
					box-sizing: border-box;
					height: 60rpx;
					line-height: 30rpx;
					display: -webkit-box;
					-webkit-box-orient: vertical;
					/* 表示盒子对象的子元素的排列方式 */
					-webkit-line-clamp: 2;
					/* 限制文本的行数，表示文本第多少行省略 */
					text-overflow: ellipsis;
					/*  打点展示 */
					overflow: hidden;
					/*超出部分进行隐藏*/
					font-size: 28rpx;
					color: #333333;
				}

				.price {
					// width: 285rpx;
					padding-left: 10rpx;
					margin-bottom: 10rpx;
					width: 100%;
					box-sizing: border-box;
					height: 30rpx;
					line-height: 30rpx;
					font-family: '宋体';
					color: red;
					// text-overflow: ellipsis;
					// white-space: nowrap;
					// overflow: hidden;
					display: flex;
					justify-content: space-between;

					.price_con {
						font-size: 20rpx;


						.pricetitle {

							color: #666666;
						}

						.pricenum {
							color: red;
							font-size: 28rpx;
							font-family: sans-serif;
						}

						.fuhao {
							font-size: 20rpx;
						}
					}

					.null_price {
						width: 100%;
						// justify-content: flex-end;
						font-size: 28rpx;
						color: red;
					}

				}

			}
		}
	}


	.popup-content {
		background-color: #F5F5F5;
		width: 450rpx;
		height: 100%;

		.popup_scroll {
			height: 1000rpx;
			// padding-bottom: 40rpx;
			display: flex;
			flex-direction: column;
			align-items: center;
			justify-content: center;

			.active {
				background-color: #FFFFFF;
			}

			.caList {
				padding-left: 60rpx;
				height: 100rpx;
				line-height: 100rpx;
				font-size: 30rpx;
			}
		}

		.top {
			padding: 30rpx;
			height: 100rpx;
			background-color: #FFFFFF;
			position: relative;

			image {
				width: 40rpx;
				height: 40rpx;
				position: absolute;
				top: 30rpx;
				right: 30rpx;
			}
		}


	}
</style>
