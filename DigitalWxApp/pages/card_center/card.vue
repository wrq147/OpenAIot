<template>
	<view>
		<view v-if="activePage==-1" style="padding-top: 30vh;">
			<uni-load-more iconType="circle" :showText="false" status="loading" />
		</view>
		<CardInfo :cardData="cardData" :localImg="localImg" v-if="activePage==0"></CardInfo>
		<CardPro :orgid.sync="orgid" :cardData="cardData" ref="refpro" v-if="activePage==1"></CardPro>
		<CardCase :orgid.sync="orgid" :cardData="cardData" ref="refcase" v-if="activePage==2"></CardCase>
		<card-tab-bar v-if="activePage>-1" :active.sync="activePage" :orgid.sync="orgid"></card-tab-bar>

		<view class="canvas-hide">
			<!-- #ifdef MP-WEIXIN -->
			<canvas id="canvas" type="2d" style="width:690px; height:400px;" />
			<canvas id="canvasShare" type="2d" style="width:690px; height:552px" />
			<!-- #endif -->
			<!-- #ifndef MP-WEIXIN -->
			<canvas canvas-id="canvas" id="canvas" style="width:690px; height:400px;" />
			<canvas canvas-id="canvasShare" id="canvasShare" style="width:690px; height:552px" />
			<!-- #endif -->
		</view>
	</view>
</template>

<script>
	import CardTabBar from '@/components/card-tab-bar.vue'
	import CardInfo from '@/components/card-com/card-info.vue'
	import CardPro from '@/components/card-com/card-pro.vue'
	import CardCase from '@/components/card-com/card-case.vue'
	import {
		getCard
	} from '@/api/userCard.js'
	import {
		drawCard
	} from '@/common/card.js'
	import {
		visit,
		visitEnd
	} from '@/api/record.js'
	export default {
		components: {
			CardTabBar,
			CardInfo,
			CardPro,
			CardCase
		},
		data() {
			return {
				cardId: 0,
				cardData: {
					Mobile: "",
					WxNumber: "",
					Email: "",
					OrgId: 0
				},
				localImg: "",
				activePage: -1,
				orgid: 0, //企业id
				shareImg: "", //分享用到的图片
				visitId: 0
			}
		},
		async onLoad(options) {
			console.log("链接上的值,名片分享", options);
			this.cardId = parseInt(options.id || "0");
		},
		async onReady() {
			//获取设备信息
			// await this.$store.dispatch("sysType");
			let pages = getCurrentPages();
			let lastPage=pages[pages.length - 2]
			if(!lastPage||lastPage==undefined||lastPage==null){
				uni.login({
					provider: 'weixin',
					success: async loginRes => {
						let x = await this.$store.dispatch("weiXinLogin", {
							code: loginRes.code,
							created: true
						})
						console.log("获取登入信息,x",x);
						if(x.isnew){
							uni.redirectTo({
								url: '/pages/card_user_add?cardId='+this.cardId
							})
						}else{
							this.$store.commit('SET_USER_INFO', null);
							let usrInfo = await this.$store.dispatch("userInfo");
							if(!usrInfo.extObj.hasOwnProperty("CardId")){
								//跳转到创建名片
								uni.showModal({
									title: '提示',
									showCancel:false,
									content: '您还没有创建名片，请先创建一张！',
									success: async res => {
										if (res.confirm) {
											uni.redirectTo({
												url: "/pages/card_center/card_add?cardId="+this.cardId
											});
										}
									}
								});
								return;
							}
							await this.load_cardData()
						}
						
					}
				});
			}else{
				await this.load_cardData()
			}
		},

		// 分享到朋友
		async onShareAppMessage(res) {

			return {
				title: "这是" + this.cardData.RealName + "的数字名片，推荐给你",
				path: '/pages/card_center/card?id=' + this.cardId,
				imageUrl: this.shareImg
			};
		},
		// 分享到朋友圈
		onShareTimeline() {
			return {
				title: "这是" + this.cardData.RealName + "的数字名片，推荐给你",
				path: '/pages/card_center/card?id=' + this.cardId,
				imageUrl: this.localImg
			};
		},
		onReachBottom() {
			if (this.activePage == 1) {
				// console.info(this.$refs.refpro)
				this.$refs.refpro.reachBottom();
			} else if (this.activePage == 2) {
				this.$refs.refcase.reachBottom();
			}
		},
		onUnload() {
			if (this.visitId > 0) {
				visitEnd(this.visitId);
				this.visitId = 0;
			}
		},
		methods: {
			async load_cardData(){
				//初始化名片信息
				let rsp = await getCard(this.cardId);
				// console.log("名片信息",rsp);
				if (rsp.data == null) {
					uni.showToast({
						title: "名片不存在",
						icon: "none",
						duration: 2000
					});
					setTimeout(() => {
						uni.navigateBack();
					}, 2000);
					return;
				}
				uni.setNavigationBarTitle({
					title: rsp.data.RealName + '的名片'
				});
				this.cardData = rsp.data;
				
				let usrInfo = await this.$store.dispatch("userInfo");
				
				// console.log("this.cardData",this.cardData,'usrInfo',usrInfo);
				if (usrInfo.name == '微信用户' || usrInfo.name == '') {
					uni.showModal({
						title: '提示',
						content: '请先完善您的名片信息',
						success: function(res) {
							if (res.confirm) {
								// console.log('用户点击确定');
								uni.navigateTo({
									url: '/pages/card_user_add'
								})
							} else if (res.cancel) {
								// console.log('用户点击取消');
								// uni.navigateBack()
							}
						}
					});
				}
				if (this.cardData.UserId != usrInfo.Id) {
					//访问记录开始
					visit({
						VisitSource: 0,
						VisitType: 0,
						TargetId: this.cardData.Id,
						ReceiveUserId: this.cardData.UserId
					}).then(xrsp => {
						this.visitId = xrsp.data;
						this.finishVisitTime();
					});
				
				}
				this.orgid = rsp.data.OrgId
				let dp = await drawCard('canvas', this.cardData);
				let dp2 = await drawCard('canvasShare', rsp.data, true); //用于分享的图片
				// this.localImg = await dp.create();
				this.$nextTick(async () => {
					this.localImg = await dp.create();
					this.shareImg = await dp2.create();
				});
				
				this.activePage = 0;
			},
			finishVisitTime() {
				if (this.visitId > 0) {
					//访问记录结束
					visitEnd(this.visitId);
					setTimeout(this.finishVisitTime, 3000);
				}
			}
		}
	}
</script>

<style lang="scss">
	.canvas-hide {
		/* 1 */
		position: fixed;
		right: 100vw;
		bottom: 100vh;
		/* 2 */
		z-index: -9999;
		/* 3 */
		opacity: 0;
	}
</style>