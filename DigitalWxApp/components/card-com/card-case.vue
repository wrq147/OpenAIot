<template>
	<view>
		<view class="search" v-if="search" @click="toSearch">
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
		<div v-if="isReload">
			<view class="popularcase" v-if="caseList.length>0">
				<view class="popcaseList">
					<navigator :url="'/pages/card_center/card_case_detail?id='+item.Id+'&uid='+cardData.UserId+'&cardId='+cardData.Id"
						class="popcaseLi" v-for="item in caseList" :key="item.Id">
						<image class="image" :src="item.ImageUrl" mode="aspectFill"></image>
						<view class="text">{{item.Title}}</view>
					</navigator>
				</view>
			</view>
			<view class="emptyclass"
				v-else-if="!search && lunboList.length==0 && caseList.length==0 && status !='loading'">
				<empty imgsrc="/static/empty/content_empty.png" txt="暂无案例信息"></empty>
			</view>
			<view v-else-if="!search && lunboList.length==0 && caseList.length==0 && status =='loading'" style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>
		</div>


	</view>
</template>

<script>
	import {
		getOrg
	} from '@/api/org.js'
	import request from '@/common/request.js'
	import {
		getExampleList
	} from '@/api/example.js'
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
		data() {
			return {
				styles: {
					color: '#999999',
					disableColor: '#EFEFEF',
					borderColor: '#EFEFEF'
				}, //输入框的样式
				searchVal: null, //搜索输入框的值
				// userInfo: null, //登录用户信息
				page: 1,
				pageSize: 9,
				caseList: [],
				status: 'loading',
				hashitems: {},
				lunboList: [], //轮播图列表
				search: false, //判断搜索是否要出现
				isReload:false
			}
		},
		mounted() {
			this.reloadPages();
		},
		methods: {
			reachBottom() {
				if (this.status != 'noMore') {
					this.page++;
					this.status = "loading";
					this.getCaseList();
					// console.log("现在是第几页", this.page);
				}
			},
			async reloadPages() {
				
				// this.userInfo = await this.$store.dispatch("userInfo");
				// console.log("用户信息",this.userInfo);
				//获取案例设置信息
				let orgInfo = await getOrg(this.orgid);
				if (orgInfo.data == null || orgInfo.data.del_flag == "2") {
					uni.showToast({
						title: '名片所属企业已不存在',
						icon: "none",
						duration: 2000
					});
					return;
				}
				this.isReload=true//开始处理数据了
				// console.log('组织信息', orgInfo);
				let CaseConfig = orgInfo.data.CaseConfig;
				// console.log("案例设置信息", CaseConfig);
				if (CaseConfig) {
					let config = JSON.parse(CaseConfig);
					let config2 = JSON.parse(CaseConfig)
					if (config2[0]) {
						config = {
							oldInfo: config2
						}
					}
					if (config.lunbo) {
						if (config.lunbo.firstCaseImg) {
							this.lunboList.push(config.lunbo.firstCaseImg)
						}
						if (config.lunbo.secondCaseImg) {
							this.lunboList.push(config.lunbo.secondCaseImg)
						}
						if (config.lunbo.thirdCaseImg) {
							this.lunboList.push(config.lunbo.thirdCaseImg)
						}
					}else if (config.oldInfo) {
						config.oldInfo.map((item, index) => {
							if (item.type == 'search') {
								this.search = item.data
							}
							if (item.type == 'lunbo') {
								item.data.forEach((url, i) => {
									// console.log("轮播图片链接", url.url);
									if (url.url && url.url != [] && url.url != '') {
										// console.log("轮播图片链接存在", url.url);
										let imgUrl = request.config.baseURL + url.url
										this.lunboList.push(imgUrl);
									}
								})
							}
						})
					}
					if (config.search) {
						this.search = config.search
					}
					

				}
				//获取案例设置信息
				this.page = 1;
				this.status = "loading";
				this.caseList = [];
				this.hashitems = {};
				await this.getCaseList();
			},
			//跳转到案例搜索
			toSearch() {
				uni.navigateTo({
					url: '/pages/card_center/card_case_search?orgid=' + this.orgid+'&uid='+this.cardData.UserId+'&cardId='+this.cardData.Id
				})
			},
			getCaseList() {
				let query = {
					OrgId: this.orgid,
					pageNum: this.page,
					pageSize: this.pageSize,
					showAll: true
				}
				getExampleList(query).then((res) => {
					// console.log("案例列表信息", res);

					res.data.List.forEach(it => {
						// console.log("案例列表",it);
						it["Id"] = it.Id.toString();
						if (!this.hashitems[it.Id]) {

							this.hashitems[it.Id] = true;
							this.caseList.push(it); //注意this.caseList是数组，it是对象
						}
					});
					if (res.data.List.length < this.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				})
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.emptyclass {
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
		height: 330rpx;
		width: 690rpx;
		border-radius: 8rpx;
		background-color: #EFEFEF;

		.swiper {
			height: 330rpx;

			.image {
				border-radius: 8rpx;
				height: 330rpx;
				width: 690rpx;
			}
		}
	}



	.popularcase {
		margin: 0 30rpx;
		margin-top: 30rpx;

		.popcaseList {
			display: flex;
			align-items: center;
			justify-content: space-between;
			flex-flow: row wrap;
			width: 100%;
			margin-top: 30rpx;
			box-sizing: border-box;

			.popcaseLi:nth-child(2n-1) {
				margin-right: 2.4%;
			}

			.popcaseLi {
				width: 48.6%;
				box-sizing: border-box;
				padding: 0 10rpx;
				height: 280rpx;
				border-radius: 8rpx;
				background-color: #FFFFFF;
				display: flex;
				align-items: center;
				justify-content: space-between;
				flex-flow: row wrap;
				justify-content: center;
				box-shadow: 0 0 20rpx rgba(229, 229, 229, 1);
				margin-bottom: 20rpx;

				.image {
					margin-top: 10rpx;
					width: 100%;
					box-sizing: border-box;
					height: 180rpx;
					display: block;

				}

				.text {
					margin-bottom: 10rpx;
					width: 100%;
					box-sizing: border-box;
					height: 60rpx;
					line-height: 30rpx;
					font-size: 32rpx;
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
				}
			}
		}
	}
</style>
