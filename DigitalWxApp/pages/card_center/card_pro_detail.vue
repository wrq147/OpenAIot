<template>
	<view>
		<view v-if="proContent==null" style="padding-top: 20rpx;">
			<uni-load-more iconType="circle" :showText="false" status="loading" />
		</view>
		<template v-else>
			<view class="top_con" v-if="uid != null && uid != usrInfo.Id">
				<view class="marTop">
					<view class="top_content">
						<image class="avatar" :src="cardData.Avatar?cardData.Avatar:'../../static/user/user_man.png'"
							mode="aspectFill" style="width: 88rpx;height: 88rpx;border-radius: 44rpx;">
						</image>
						<view class='right_con'>
							<view class="right_top">
								<text style="font-weight: 700;">{{cardData.RealName}}</text>
								<text
									style="color: #A4A6A7;font-size: 22rpx;margin-left: 20rpx;">{{cardData.Mobile}}</text>
							</view>
							<view class="right_bottom">
								<text>{{cardData.OrgName}}</text>
							</view>
						</view>
						<view class="share_tips">
							<text>分享者</text>
						</view>
					</view>
				</view>
			</view>
			<view class="con">
				<view class="topTitle">
					<view class="titleText">{{proContent.ProName}}</view>
				</view>
				<view class="proContent">
					<view v-if="proContent.Detail!=''" class="content">
						<mz-editor-parser :datalist="proContent.Detail"></mz-editor-parser>
					</view>
					<empty v-else imgsrc="/static/empty/content_empty.png" txt="暂无内容"></empty>
				</view>
				<view class="button_class">
					<view class="button_con">
						<view class="copyBtn" @click="copyDetail">
							<image src="../../static/copy.png" mode="aspectFill"></image>
						</view>
						<button class="shimg" open-type="share" v-if="cardId !=0"></button>
						<button class="shimg" @click="openCardList" v-else></button>
					</view>
				</view>
			</view>
		</template>
		<uni-popup ref="popup" type="bottom" backgroundColor="#fff">
			<view class="switchCom">
				<view class="title">
					选择企业
				</view>
				<view class="com-scroll">
					<view class="myCom">
						<view class="itemwrap" v-for="item in orgList" :key="item.Id" @click="switchClick(item.Id)">
							<view :class="{'item':true,'cur':item.Id==proContent.OrgId}">
								<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="logoImg(item.Logo)" />
								<view class="cont">
									<text class="txt">{{item.OrgName}}</text>
									<text class="xp">行业：{{item.IndustryName}}</text>
									<text class="p">规模：{{item.SizeName}}</text>
								</view>
							</view>
						</view>

					</view>
				</view>

			</view>
		</uni-popup>
		<uni-popup ref="popup2" type="bottom" backgroundColor="#fff">
			<view class="switchCom">
				<view class="title">
					选择分享产品的名片
				</view>
				<view class="cardScroll">
					<view class="myCard">
						<button class="itemwrap" v-for="carditem in cardList" :key="carditem.Id"
							@click="switchCardClick(carditem.Id)" open-type="share" :id='carditem.Id'>
							<view class="item">
								<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="carditem.Avatar"
									loading-ing-img="oblique-light" />
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
						</button>

					</view>
				</view>

			</view>
		</uni-popup>

	</view>
</template>

<script>
	import {
		getProductMessage
	} from "@/api/product.js"
	import {
		getCardList
	} from '@/api/userCard.js'
	import {
		visit,
		visitEnd
	} from '@/api/record.js'
	import {
		userOrgList,
		switchOrg
	} from '@/api/org.js'
	import request from '@/common/request.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		getUsersInfo
	} from '@/api/user.js'
	import {
		getCard
	} from '@/api/userCard.js'
	export default {
		data() {
			return {
				// navconfig: {
				// 	title:"产品详情页",
				// 	fontSize: '34px',
				// 	height: 44,
				// 	statusBarBackground: "#ffffff",
				// },
				proContent: null,
				uid: null,
				visitId: 0,
				orgList: [],
				usrInfo: {},
				orgInfo: {}, //企业信息
				cardData: {}, //名片信息
				cardList: [],
				proOrgId: 0,
				cardId: 0,
				chooseCardId: 0,
			}
		},

		async onLoad(options) {
			this.getOrgList()
			let rsp = await getProductMessage({
				id: parseInt(options.id || "0")
			});
			if (rsp.data == null) {
				uni.showToast({
					title: "产品不存在",
					icon: "none",
					duration: 2000
				});
				setTimeout(() => {
					uni.navigateBack();
				}, 2000);
				this.proContent = ''
				return;
			}
			this.proOrgId = rsp.data.OrgId
			this.getCardList()
			let res = await getOrg(rsp.data.OrgId)
			// if (res.data == null || res.data.del_flag == "2") {
			// 	uni.showToast({
			// 		title: '名片所属企业已不存在',
			// 		icon: "none",
			// 		duration: 2000
			// 	});
			// 	this.proContent = ''
			// 	return;
			// }
			this.cardId = parseInt(options.cardId || 0)
			this.uid = options.uid;
			if (this.cardId) {
				let response = await getCard(this.cardId);
				// console.log("名片信息", response);
				if (response.data == null) {
					uni.showToast({
						title: "名片不存在",
						icon: "none",
						duration: 2000
					});
					setTimeout(() => {
						uni.navigateBack();
					}, 2000);
					this.proContent = ''
					return;
				}
				this.cardData = response.data
			}
			this.usrInfo = await this.$store.dispatch("userInfo");
			this.proContent = rsp.data;
			// console.log("chanpxinxi", this.proContent);
			if (this.uid != null && this.uid != this.usrInfo.Id) {
				//访问记录开始
				visit({
					VisitSource: 0,
					VisitType: 1,
					TargetId: this.proContent.Id,
					ReceiveUserId: this.uid
				}).then(xrsp => {
					this.visitId = xrsp.data;
				})
			}
			if (this.cardId == 0) {
				uni.hideShareMenu()
			}

		},
		onUnload() {
			if (this.visitId > 0) {
				//访问记录结束
				visitEnd(this.visitId);
			}
		},

		// 分享到朋友
		onShareAppMessage(res) {
			// console.log(res, '分享');
			if (res.target.id) {
				this.chooseCardId = res.target.id
			}
			return {
				title: this.proContent.ProName,
				path: this.cardId ? '/pages/card_center/card_pro_detail?id=' + this.proContent.Id + "&uid=" + this.uid +
					"&cardId=" +
					this.cardId : '/pages/card_center/card_pro_detail?id=' + this.proContent.Id + "&uid=" + this.uid +
					"&cardId=" +
					this.chooseCardId,
				imageUrl: this.proContent.ImageUrl
			};

		},
		// 分享到朋友圈
		onShareTimeline() {
			return {
				title: this.proContent.ProName,
				path: this.cardId ? '/pages/card_center/card_pro_detail?id=' + this.proContent.Id + "&uid=" + this.uid +
					"&cardId=" + this.cardId : '/pages/card_center/card_pro_detail?id=' + this.proContent.Id + "&uid=" +
					this.uid,
				imageUrl: this.proContent.ImageUrl
			};
		},
		methods: {
			openCardList() {
				//打开名片列表
				this.$refs.popup2.open('bottom')
			},
			switchCardClick() {
				//选择分享的名片
				this.$refs.popup2.close();
			},
			async getCardList() {
				this.cardList = (await getCardList({
					OrgId: this.proOrgId,
					showAll: true
				})).data.List;
				// console.log("名片列表");
			},
			logoImg(img) {
				if (img == "") {
					return "/static/user/company.png";
				} else {
					return img;
				}
			},
			async getOrgList() {
				let rsp = await userOrgList(true);
				for (let xi = 0; xi < rsp.data.length; xi++) {
					let x = rsp.data[xi];
					x.IndustryName = "";
					x.SizeName = "";
					if (x.Logo == "") {
						x.Logo = "/static/user/company.png";
					}
					if (x.Size) {
						let rt = await this.$store.dispatch("dictName", {
							name: "org_size",
							value: x.Size
						});
						x.SizeName = rt;
					}
					if (x.Industry) {
						let rt = await this.$store.dispatch("industryName", x.Industry);
						x.IndustryName = rt;
					}

				}
				this.orgList = rsp.data;
				// console.log("企业列表", rsp);
			},
			switchClick(orgId) {
				this.$refs.popup.close();
				// console.log("当前产品的企业id", this.proContent.OrgId, orgId);
				uni.navigateTo({
					url: '/pages/user/pro_add?otherCon=' + encodeURIComponent(JSON.stringify(this.proContent)) +
						'&orgId=' + orgId
				})
			},
			copyDetail() {
				this.$refs.popup.open('bottom')

			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #ffffff;
	}

	.cardScroll {
		overflow-y: scroll;
		max-height: 640rpx;
		white-space: nowrap;
		width: 100%;
	}

	.myCard {
		width: 100%;

		.itemwrap {
			// background-color: #CBCCCD;
			height: 200rpx;
			margin-bottom: 30rpx;
			border-radius: 11rpx;
			padding: 0;
			text-align: left;
		}

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
				justify-content: center;

				.txt {
					margin-left: 49rpx;
					font-size: 30rpx;
					font-weight: bold;
					text-align: left;
					display: inline;
					line-height: 30px;
				}


				.tip {
					margin-top: 20rpx;
					margin-left: 49rpx;
					display: flex;
					flex-direction: column;
					font-size: 22rpx;
					color: #999999;
					line-height: 22px;
				}
			}
		}

	}

	.top_con {
		padding: 0 10rpx;
		height: 140rpx;
		width: 100%;
		box-sizing: border-box;

		.marTop {
			height: 140rpx;
			width: calc(100% - 20rpx);
			position: fixed;
			top: 0;
			left: 0;
			border: 2rpx solid #ADD5FD;
			background-color: #E9F6FE;
			border-radius: 10rpx;
			margin-left: 10rpx;

			.top_content {
				height: 100%;
				width: 100%;
				box-sizing: border-box;
				padding: 0 30rpx;
				display: flex;
				flex-direction: row;
				align-items: center;
				justify-content: flex-start;
				font-size: 30rpx;
				position: relative;

				.right_con {
					display: flex;
					flex-direction: column;
					margin-left: 30rpx;
					text-align: left;

					.right_top {
						vertical-align: bottom;
						margin-bottom: 10rpx;
					}
				}

				.share_tips {
					font-size: 22rpx;
					width: 100rpx;
					height: 46rpx;
					color: #ffffff;
					background-color: #50A6FA;
					border-radius: 0 10rpx 0 10rpx;
					position: absolute;
					right: 0;
					top: 0;
					line-height: 46rpx;
					text-align: center;
				}
			}
		}
	}

	.con {

		width: 100vm;
		margin-top: 35rpx;

		.topTitle {
			width: 690rpx;
			padding: 0 30rpx;
			display: flex;
			align-items: center;
			justify-content: flex-start;
			height: 44rpx;
			margin-bottom: 10rpx;
			// border-bottom: 2rpx solid #DADADA;

			.titleText {
				font-size: 44rpx;
				font-weight: 700;
				color: #333333;
			}


		}

		.proContent {
			// background-color: #ffffff;
			width: 100%;
			box-sizing: border-box;
			padding: 0 30rpx 20rpx;
		}

		.button_class {
			width: 130rpx;
			height: 280rpx;
			position: fixed;
			bottom: 20rpx;
			right: 10rpx;

			.button_con {
				position: relative;

				.shimg {
					margin: 0;
					width: 130rpx;
					height: 150rpx;
					background-size: 100% 100%;
					background-image: url('../../static/share.png');
					background-color: rgba(0, 0, 0, 0);
					position: absolute;
					right: 0;
					top: 130rpx;
				}

				.copyBtn {
					width: 130rpx;
					height: 150rpx;

					image {
						width: 130rpx;
						height: 150rpx;
					}
				}
			}

		}
	}

	.switchCom {
		background-color: #FFFFFF;
		min-height: 590rpx;
		width: 100%;
		box-sizing: border-box;
		padding: 0 30rpx;
		bottom: 0;
		font-family: pfcu;

		.title {
			width: 100%;
			height: 102rpx;
			line-height: 102rpx;
			text-align: left;
			font-size: 30rpx;
		}

		.com-scroll {
			overflow-y: scroll;
			max-height: 640rpx;
			white-space: nowrap;
			width: 100%;
		}

		.myCom {
			width: 100%;

			.itemwrap {
				background-color: #CBCCCD;
				height: 200rpx;
				margin-bottom: 30rpx;
				border-radius: 11rpx;
			}

			.item {
				width: 100%;
				height: 200rpx;
				display: flex;
				border: 1px solid #D7D7D7;
				border-radius: 10rpx;
				align-items: center;

				box-sizing: border-box;
				z-index: 9999;
				overflow: hidden;
				background-color: #FFFFFF;

				.ximg {
					margin-left: 14rpx;
					width: 163rpx;
					height: 174rpx;
					overflow: hidden;
					border-radius: 8rpx;
				}

				.cont {
					margin-left: 50rpx;
					display: flex;
					flex-direction: column;

					.txt {
						margin-top: 10rpx;
						font-size: 30rpx;
						font-weight: bold;
						color: #333333;
					}

					.xp {
						margin-top: 20rpx;
						margin-bottom: 10rpx;
					}

					.p,
					.xp {
						font-size: 22rpx;
						color: #999999;
					}
				}


			}

			.cur {
				border: 1px solid #4EA8FA;
				background-color: #E9F6FE;
				z-index: 999;
			}

		}

		.add {
			display: flex;
			height: 200rpx;
			width: 100%;
			align-items: center;
			justify-content: center;
			color: #666666;
			font-size: 30rpx;
			box-sizing: border-box;
			border: 1px solid #D7D7D7;
			border-radius: 10rpx;
			margin-bottom: 30rpx;

			text {
				margin-left: 10rpx;
			}
		}
	}
</style>
