<template>
	<page-meta :page-style="`overflow:${pageScrollFlag?'hidden':'visible'}`"></page-meta>
	<view>
		<view class="wrap">
			<view class="mp">
				<image :src="localImg" mode="aspectFill" style="width:690rpx; height:400rpx;"
					@click="jmp2Card(localImg)">
				</image>
				<!-- <image :src="buttonShare" style="width:420rpx;" mode="widthFix"></image> -->
				<navigator url="" class="right-switch" @click="openCardMan">
					<uni-icons custom-prefix="my-icon" type="my-icon-qiehuan" color="#FFFFFF" size="16"></uni-icons>
					<text class="swtext">切换</text>
				</navigator>
			</view>
			<view class="canvas-hide">
				<canvas v-if="sysTyp=='windows'" canvas-id="canvas" id="canvas" style="width:690px; height:400px;" />
				<!-- #ifdef MP-WEIXIN -->
				<canvas id="canvasShare" type="2d" style="width:690px; height:552px" />
				<canvas v-if="sysTyp!='windows'" id="canvas" type="2d" style="width:690px; height:400px" />
				<!-- #endif -->
				<!-- #ifndef MP-WEIXIN -->
				<canvas canvas-id="canvasShare" id="canvasShare" style="width:690px; height:552px" />
				<canvas v-if="sysTyp!='windows'" canvas-id="canvas" id="canvas" style="width:690px; height:400px" />
				<!-- #endif -->
			</view>
			<button open-type="share" class="btn">
				发名片
			</button>
			<view class="option">
				<view class="item">
					<navigator :url="'/pages/card_center/card_edit?id='+userCardId">
						<image src="../../static/mp/editmp.png" mode=""></image>
						<view class="txt">
							编辑名片
						</view>
					</navigator>
				</view>
				<view class="item">
					<navigator :url="'/pages/card_center/card_share?id='+userCardId">
						<image src="../../static/mp/share.png" mode=""></image>
						<view class="txt">
							个性分享
						</view>
					</navigator>
				</view>
			</view>
		</view>
		<view class="visitRecord">
			<view class="title">
				我的浏览记录
			</view>
			<view class="list" v-if="recordList.length>0">
				<navigator :url="'/pages/card_center/card?id='+recitem.TargetId" class="item"
					v-for="recitem in recordList" :key="recitem.Id">
					<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="recitem.Avatar"
						loading-ing-img="oblique-light" />
					<view class="xri border-bottom">
						<view class="mid">
							<view class="txt">
								{{recitem.RealName}}
							</view>
							<view class="tip">
								<text>{{recitem.PostName}}</text>
								<text>{{recitem.OrgName}}</text>
							</view>
						</view>
						<view class="ri">
							{{date2str(recitem.CreatedOn)}}
						</view>
					</view>

				</navigator>
			</view>
			<empty v-else-if="status!='loading'" imgsrc="/static/empty/content_empty.png" txt="暂无内容"></empty>
			<view v-else style="padding: 20rpx 0;">
				<J-skeleton :loading="true" :showTitle="true" :row="2">
				</J-skeleton>
			</view>
			<view v-if="recordList.length>0" style="padding-top: 20rpx;">
				<uni-load-more iconType="circle" :showText="false" :status="status" />
			</view>
		</view>



		<uni-popup ref="popup" type="bottom" @change="popChange" backgroundColor="#fff">
			<view class="switchCard">
				<view class="title">
					切换名片
				</view>
				<view class="cardScroll">
					<view class="myCard">
						<view class="itemwrap" v-for="carditem in cardList" :key="carditem.Id"
							@click="switchClick(carditem.Id)">
							<uni-swipe-action>
								<uni-swipe-action-item>
									<view :class="{'item':true,'cur':carditem.Id==userCardId}">
										<fr-image class="ximg" :lazy-load="true" mode="aspectFill"
											:src="carditem.Avatar" loading-ing-img="oblique-light" />
										<view class="mid">
											<view class="txt">
												{{carditem.RealName}}
											</view>
											<view class="tip">
												<text>{{carditem.PostName}}</text>
												<text>{{carditem.OrgName}}</text>
											</view>
										</view>

										<text class="mk">当前使用</text>
									</view>
									<template v-slot:right>
										<view class="acbts">
											<navigator url="" @click.stop="editCard(carditem.Id)" class="editBTN">
												<text>编辑</text>
												<text>资料</text>
											</navigator>
											<navigator url="" class="delBTN" @click.stop="delCard(carditem.Id)">
												<text>删除</text>
												<text>名片</text>
											</navigator>
										</view>
									</template>
								</uni-swipe-action-item>
							</uni-swipe-action>
						</view>

					</view>
					<navigator url="" @click="addNewCard" class="add">
						<uni-icons type="plusempty" size="18" color="#666666"></uni-icons>
						<text>添加新名片</text>
					</navigator>
				</view>

			</view>
		</uni-popup>

		<bind-tel-modal v-if="showTel"></bind-tel-modal>

		<my-tab-bar :active="0"></my-tab-bar>
	</view>
</template>

<script>
	import MyTabBar from '@/components/my-tab-bar.vue'
	import {
		drawCard
	} from '@/common/card.js'
	import {
		getCard,
		getCardInfo,
		getCardList,
		switchCard,
		deleteCard
	} from '@/api/userCard.js'
	import {
		getRecordList
	} from '@/api/record.js'
	import {
		dateUtils
	} from '@/common/util.js'
	import {
		verUp
	} from '@/common/ver.js'
	import {
		getNoRead
	} from '@/api/record.js'

	export default {
		components: {
			MyTabBar
		},
		computed: {
			isNewCar() { //是否要刷新名片
				// console.log("是否要刷新", this.$store.state.isNewCard);
				return this.$store.state.isNewCard;
			},
		},
		data() {
			return {
				sysTyp: '',
				localImg: "",
				buttonShare: "",
				userCardId: 0,
				cardList: null,
				shareTitle: "",
				page: 1,
				status: 'loading',
				recordList: [],
				hashitems: {}, //去重
				pageScrollFlag: false,
				showTel: false,
				isNotfirst: false, //用于判断是否是第一次进入该页面
				imgHeight: 0,
				imgWidth: 0,
				shareImg: "",
				hisImg: '',
				dp2: null
			}
		},
		async onLoad() {
			this.sysTyp = this.$store.state.sysType
			// console.log("shebeixinxi1", this.sysTyp);
			this.$store.commit('SET_ISNEWCARD_DATA', true);//刚进页面设置需要更新名片
			await verUp();

		},
		async onReady() {
			// await this.getWidHei()
			// this.$store.dispatch("sysType");
			await this.reloadpage();
		},
		async onShow() {
			if (this.isNotfirst) {
				this.page = 1;
				this.status = "loading";
				this.recordList = [];
				this.hashitems = {};
				//获取我的浏览记录
				await this.load_data();
				// this.$forceUpdate()
				// console.log("onshow执行");
			}
			await this.changeCanCard()
		},
		// 分享到朋友
		async onShareAppMessage(res) {
			this.shareImg = await this.dp2.create();
			return {
				title: this.shareTitle,
				path: '/pages/card_center/card?id=' + this.userCardId,
				imageUrl: this.shareImg
			};
		},
		// 分享到朋友圈
		onShareTimeline() {
			return {
				title: this.shareTitle,
				path: '/pages/card_center/card?id=' + this.userCardId,
				imageUrl: this.localImg
			};
		},
		onReachBottom: function() {
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data();
			}
		},
		methods: {
			async changeCanCard() {
				//绘制名片
				if (this.isNewCar) {
					// uni.showLoading({
					// 	title: '加载名片...'
					// });
					//生成名片
					// console.log("如果刷新名片为真则重新绘制名片");
					try {
						this.hisImg = this.localImg
						this.$store.commit('SET_USER_INFO', null);
						let usrInfo = await this.$store.dispatch("userInfo");
						this.userCardId = usrInfo.extObj.CardId;
						if(!usrInfo.extObj.hasOwnProperty("CardId")){
							//跳转到创建名片
							uni.showModal({
								title: '提示',
								showCancel:false,
								content: '您还没有创建名片，请先创建一张！',
								success: async res => {
									if (res.confirm) {
										uni.navigateTo({
											url: "/pages/card_center/card_add"
										});
									}
								}
							});
							return;
						}
						let rsp = await getCardInfo(this.userCardId);
	
						this.shareTitle = rsp.data.ShareTitle == "" ? "这是我的数字名片，请收下 " : rsp.data.ShareTitle;
						let dp = null
						if (this.sysTyp == 'windows') {
							dp = await drawCard('canvas', rsp.data, false, false);
						} else {
							dp = await drawCard('canvas', rsp.data, false, true);
						}
						this.localImg = await dp.create();
						// console.log("展示的图片", this.localImg)
						this.dp2 = await drawCard('canvasShare', rsp.data, true, true); //用于分享的图片
						this.$store.commit('SET_ISNEWCARD_DATA', false); //更新完名片后设置名片状态为不用刷新

					} catch (err) {
						console.info('异常：', err);
					} finally {

						// uni.hideLoading()
					}
				}
			},
			jmp2Card(img) {
				//图片预览
				// let list = []
				// list.push(this.localImg)
				// uni.previewImage({
				// 	urls: list,
				// 	longPressActions: {
				// 		success: function(data) {
				// 			// console.log('选中了第' + (data.tapIndex + 1) + '个按钮,第' + (data.index + 1) + '张图片');
				// 		},
				// 		fail: function(err) {
				// 			// console.log(err.errMsg);
				// 		}
				// 	}
				// });
				uni.navigateTo({
					url: 'card?id=' + this.userCardId
				});
			},
			popChange(e) {
				this.pageScrollFlag = e.show;
			},
			async reloadpage(name) {

				if (name == "card") {
					await this.reloadCardList();
					return;
				}
				if (name == 'edit') {
					this.$store.commit('SET_USER_INFO', null);

				}
				// uni.showLoading({
				// 	title: '加载中...'
				// });
				try {
					this.page = 1;
					this.status = "loading";
					this.recordList = [];
					this.hashitems = {};
					//获取我的浏览记录
					this.load_data();
					this.isNotfirst = true //第一次访问页面执行完将isNotfirst设置为true
					this.$forceUpdate();
				} catch (e) {
					//TODO handle the exception
				} finally {
					// uni.hideLoading();
				}


			},
			load_data() {
				getRecordList({
					pageSize: 30,
					pageNum: this.page,
					showAll: true,
				}).then(rsp => {
					// console.log("浏览记录接口结果", rsp);
					rsp.data.List.forEach(it => {
						if (!this.hashitems[it.Id]) {
							this.hashitems[it.Id] = true;
							this.recordList.push(it);
						}
					});
					// console.log("浏览记录", this.recordList);
					if (rsp.data.List.length < 30) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				})

			},
			async reloadCardList() {
				this.cardList = (await getCardList({
					showAll: true
				})).data.List;

				// console.log("名片列表信息", this.cardList);
			},
			async openCardMan() {
				if (this.cardList == null) {
					await this.reloadCardList();
				}

				this.$refs.popup.open('bottom');
			},
			date2str(timestr) {
				return dateUtils.parse(timestr).format("yyyy.MM.dd hh:mm");
			},
			async switchClick(id) {
				this.$refs.popup.close();
				if (this.userCardId != id) {
					this.localImg = ''
					// uni.showLoading({
					// 	title: '加载中...'
					// });
					try {

						await switchCard(id); //切换名片
						this.userCardId = id;
						this.$store.commit('SET_ISNEWCARD_DATA', true);
						await this.changeCanCard()
						this.page = 1;
						this.status = "loading";
						this.recordList = [];
						this.hashitems = {};
						this.load_data()

					} catch (err) {
						// console.info('异常：', err);
					} finally {
						// uni.hideLoading();
						this.$refs.popup.close();

					}
				}
			},
			async delCard(id) {
				uni.showModal({
					title: '提示',
					content: '确定删除名片吗？',
					success: async res => {
						if (res.confirm) {
							this.$refs.popup.close();
							uni.showLoading({
								title: '加载中...'
							});
							try {
								await deleteCard(id);
								await this.reloadCardList();
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
			editCard(id) {
				this.$refs.popup.close();
				uni.navigateTo({
					url: '/pages/card_center/card_edit?id=' + id
				});

			},
			addNewCard() {
				this.$refs.popup.close();
				uni.navigateTo({
					url: "/pages/card_center/card_add"
				});

			}
		}
	}
</script>

<style lang="scss">
	.canvas-hide {
		/* 1 */
		position: fixed;
		right: 100vw;
		// top: 100%;
		/* 2 */
		// position: absolute;
		bottom: 100vh;
		// right: 100vw;
		// z-index: -9999;
		/* 3 */
		opacity: 0;
		// display: none;
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding-top: 20rpx;
		padding-left: 30rpx;
		padding-right: 30rpx;
		background-color: #FFFFFF;
		margin-bottom: 20rpx;

		.mp {
			width: 690rpx;
			height: 400rpx;
			// overflow: hidden;
			border-radius: 10rpx;
			box-sizing: border-box;
			color: #FFFFFF;
			position: relative;
			background-color: #14395F;
			box-shadow: 0px 6px 15px 0px rgba(0, 0, 0, 0.2000);

			image {
				image-rendering: -moz-crisp-edges;
				image-rendering: -o-crisp-edges;
				image-rendering: -webkit-optimize-contrast;
				image-rendering: crisp-edges;
				-ms-interpolation-mode: nearest-neighbor;
			}


			.right-switch {
				background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
				border-radius: 30rpx 0rpx 8rpx 0rpx;
				width: 140rpx;
				height: 80rpx;
				position: absolute;
				bottom: 0;
				right: 0;
				display: flex;
				align-items: center;
				justify-content: center;

				.swtext {
					margin-left: 12rpx;
				}

			}

		}

		.btn {
			width: 100%;
			height: 100rpx;
			line-height: 100rpx;
			border-radius: 8rpx;
			margin-top: 38rpx;
			text-align: center;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			box-shadow: 0px 10px 20px 0px rgba(74, 171, 251, 0.5000);
			font-size: 30rpx;
			font-weight: 400;
		}

		.option {
			width: 100%;
			height: 167rpx;
			display: flex;

			.item {
				height: 167rpx;
				width: 50%;

				navigator {
					width: 100%;
					height: 167rpx;
					text-align: center;
					color: #333333;
					font-size: 24rpx;
					font-weight: 400;
					padding-top: 50rpx;
					box-sizing: border-box;

					image {
						width: 44rpx;
						height: 44rpx;
						display: block;
						margin: 0 auto;
					}

					.txt {
						margin-top: 10rpx;
					}
				}
			}
		}
	}

	.visitRecord {
		width: 100%;
		height: auto;
		background-color: #FFFFFF;

		.title {
			color: #666666;
			font-size: 28rpx;
			font-weight: 400;
			height: 69rpx;
			padding-left: 30rpx;
			padding-right: 30rpx;
			padding-top: 20rpx;
			box-sizing: border-box;
		}

		.list {
			width: 100%;
			background-color: #FFFFFF;

			.item {
				height: 149rpx;
				display: flex;
				padding-left: 30rpx;
				padding-right: 30rpx;
				padding-top: 20rpx;

				.ximg {
					width: 68rpx;
					height: 68rpx;
					border-radius: 50%;
					border: 1px solid #F5F5F5;
					overflow: hidden;
				}

				.xri {
					display: flex;
					flex: 1;

					.mid {
						display: flex;
						flex-direction: column;
						flex: 1;

						.txt {
							margin-left: 12rpx;
							font-size: 30rpx;
							font-weight: bold;
							text-align: left;
						}


						.tip {
							margin-top: 20rpx;
							margin-left: 12rpx;
							display: flex;
							flex-direction: column;
							font-size: 22rpx;
							color: #999999;
						}
					}

					.ri {
						width: 200rpx;
						font-size: 20rpx;
						color: #B4B4B4;
						text-align: right;
					}
				}

			}
		}

	}



	.switchCard {

		background-color: #FFFFFF;
		min-height: 60%;
		width: 100%;
		box-sizing: border-box;
		padding: 0 30rpx;
		bottom: 0;
		z-index: 999;

		.title {
			width: 100%;
			height: 102rpx;
			line-height: 102rpx;
			text-align: left;
			font-size: 30rpx;
		}

		.cardScroll {
			overflow-y: scroll;
			max-height: 640rpx;
			white-space: nowrap;
			width: 100%;
		}

		.myCard {
			width: 100%;

			.item {
				width: 100%;
				height: 200rpx;
				display: flex;
				border: 1px solid #D7D7D7;
				border-radius: 10rpx;
				align-items: center;
				padding: 0 13rpx;
				box-sizing: border-box;
				z-index: 9999;
				overflow: hidden;
				background-color: #FFFFFF;
				position: relative;

				.mk {
					position: absolute;
					top: 26rpx;
					right: 34rpx;
					font-size: 26rpx;
					color: #50A6FA;
					display: none;
				}

				&.cur {
					border: 1px solid #50A6FA;
					background-color: #E9F6FE;
				}

				&.cur .mk {
					display: block !important;
				}


				.ximg {
					display: flex;
					overflow: hidden;
					width: 150rpx;
					height: 150rpx;
					border-radius: 50%;
				}

				.mid {
					display: flex;
					flex-direction: column;

					.txt {
						margin-left: 49rpx;
						font-size: 30rpx;
						font-weight: bold;
						text-align: left;
					}


					.tip {
						margin-top: 20rpx;
						margin-left: 49rpx;
						display: flex;
						flex-direction: column;
						font-size: 22rpx;
						color: #999999;
					}
				}
			}

			.acbts {
				display: flex;
				flex-direction: row;

				.editBTN,
				.delBTN {
					flex: 1;
					display: flex;
					flex-direction: column;
					justify-content: center;
				}

				.editBTN {
					background-color: #CBCCCD;
					margin-left: -10rpx;
					padding-left: 30rpx;
					padding-right: 20rpx;
					color: #666666;
				}

				.delBTN {
					flex: 1;
					background-color: #E84C35;
					color: #FFFFFF;
					padding: 0 20rpx;
				}
			}
		}

		.add {
			display: flex;
			height: 200rpx;
			align-items: center;
			justify-content: center;
			color: #666666;
			font-size: 30rpx;
			border: 1px solid #D7D7D7;
			border-radius: 10rpx;
			margin-bottom: 30rpx;

			text {
				margin-left: 10rpx;
			}
		}

		.itemwrap {
			margin-bottom: 32rpx;
		}

		::-webkit-scrollbar {
			width: 4px;
			height: 2px;
			background-color: white;
		}

		::-webkit-scrollbar-thumb {
			border-radius: 16px;
			background-color: #e8e8e8;
		}
	}
</style>
