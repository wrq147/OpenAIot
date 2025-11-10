<template>
	<view>
		<view v-if="caseContent==null" style="padding-top: 20rpx;">
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
					<view class="titleText">{{caseContent.Title}}</view>
				</view>
				<view class="caseContent">
					<view v-if="caseContent.Detail!=''" class="content">
						<mz-editor-parser :datalist="caseContent.Detail"></mz-editor-parser>
					</view>
					<empty v-else imgsrc="/static/empty/content_empty.png" txt="暂无内容"></empty>
				</view>
				<view class="button_class">
					<view class="button_con">
						<button class="shimg" open-type="share" v-if="cardId !=0"></button>
						<button class="shimg" @click="openCardList" v-else></button>
					</view>
				</view>
			</view>
		</template>
		<uni-popup ref="popup2" type="bottom" backgroundColor="#fff">
			<view class="switchCom">
				<view class="title">
					选择分享案例的名片
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
		getExampleInfo
	} from "@/api/example.js"
	import {
		visit,
		visitEnd
	} from '@/api/record.js'
	import {
		getCard,
		getCardList
	} from '@/api/userCard.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		getUsersInfo
	} from '@/api/user.js'
	export default {
		data() {
			return {
				caseContent: null,
				uid: null,
				visitId: 0,
				cardId: 0,
				chooseCardId: 0,
				cardData: {},
				cardList: [],
				caseOrgId: 0,
				usrInfo: {}, //当前登入用户信息
			}
		},
		async onLoad(options) {
			let rsp = await getExampleInfo({
				id: parseInt(options.id || "0")
			});
			if (rsp.data == null) {
				uni.showToast({
					title: "案例不存在",
					icon: "none",
					duration: 2000
				});
				this.caseContent = ''
				setTimeout(() => {
					uni.navigateBack();
				}, 2000);
				return;
			}
			this.caseOrgId = rsp.data.OrgId
			this.getCardList()
			let res = await getOrg(rsp.data.OrgId)
			// console.log("企业信息", res);
			// if (res.data == null || res.data.del_flag == "2") {
			// 	uni.showToast({
			// 		title: '名片所属企业已不存在',
			// 		icon: "none",
			// 		duration: 2000
			// 	});
			// 	this.caseContent = ''
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
					this.caseContent = ''
					return;
				}
				this.cardData = response.data
			}
			this.usrInfo = await this.$store.dispatch("userInfo");
			this.caseContent = rsp.data;
			// console.log("本人的信息", usrInfo);
			if (this.uid != null && this.uid != this.usrInfo.Id) {
				//访问记录开始
				visit({
					VisitSource: 0,
					VisitType: 2,
					TargetId: this.caseContent.Id,
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
			if (res.target.id) {
				this.chooseCardId = res.target.id
			}
			return {
				title: this.caseContent.Title,
				path: this.cardId ? '/pages/card_center/card_case_detail?id=' + this.caseContent.Id + "&uid=" + this.uid +
					"&cardId=" + this.cardId : '/pages/card_center/card_case_detail?id=' + this.caseContent.Id + "&uid=" +
					this.uid +
					"&cardId=" + this.chooseCardId,
				imageUrl: this.caseContent.ImageUrl
			};
		},
		// 分享到朋友圈
		onShareTimeline() {
			return {
				title: this.caseContent.Title,
				path: '/pages/card_center/card_case_detail?id=' + this.caseContent.Id + "&uid=" + this.uid +
					"&cardId=" + this.cardId,
				imageUrl: this.caseContent.ImageUrl
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
					OrgId: this.caseOrgId,
					showAll: true
				})).data.List;
				// console.log("名片列表");
			},
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #ffffff;
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

			.titleText {
				font-size: 44rpx;
				font-weight: 700;
				color: #333333;
			}

		}

		.caseContent {
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
</style>
