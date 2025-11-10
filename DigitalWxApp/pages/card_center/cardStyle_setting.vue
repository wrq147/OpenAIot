<template>
	<view>
		<view class="wrap">
			<view class="mp">
				<!-- #ifdef MP-WEIXIN -->
				<canvas id="canvas" type="2d" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
				<!-- #ifndef MP-WEIXIN -->
				<canvas canvas-id="canvas" id="canvas" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
			</view>
		</view>
		<view class="choose">
			<view class="title">
				选择版式
			</view>
			<scroll-view :scroll-x="true" class="content">
				<block>
					<view class="contwrap">
						<view v-for="(bs,idx) in bslist" class="item" style="border-radius: 6rpx;overflow: hidden;"
							@click="bsSelected(idx)">
							<view class="ac" v-if="(idx+1)==cardData.TemplateId"></view>
							<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="bs"
								loading-ing-img="oblique-light" />
						</view>
					</view>
				</block>
			</scroll-view>
			<view class="border-bottom" style="padding-top: 40rpx;"></view>
			<view class="title" style="padding-top: 30rpx;">
				选择背景
			</view>
			<scroll-view :scroll-x="true" class="content">
				<block>
					<view class="contwrap">
						<view v-for="(bg,idx) in showbglist" class="item" @click="bgSelected(bg.num)">
							<view class="ac" v-if="(bg.num+1)==cardData.TemplateBk"></view>
							<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="bg.src"
								loading-ing-img="oblique-light" />
						</view>
					</view>
				</block>
			</scroll-view>
			<view class="border-bottom" style="padding-top: 40rpx;"></view>
			<view class="title" style="padding-top: 30rpx;">
				展示信息
			</view>
			<view class="info">{{showInfo}}</view>
		</view>
		<view class="border-btn">
			<button class="btn" @click="saveClick">
				保存
			</button>
		</view>
	</view>
</template>

<script>
	import {
		getDrawObj,
		drawCard
	} from '@/common/card.js'
	import {
		getCard,
		editCard
	} from '@/api/userCard.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				cardId: 0,
				cardData: {
					RealName: ""
				},
				bslist: [], //版式列表
				bglist: [], //所有的北京列表
				showbglist: [], //展示的背景列表
				showInfo: ""
			}
		},
		onLoad(options) {

			this.cardData = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1];
			// console.log("名片样式",this.cardData);
			this.cardId = parseInt(options.id || "0");
			this.bslist = getDrawObj().bslist;
			this.bglist = getDrawObj().bglist;

		},
		async onReady() {
			try {
				await drawCard('canvas', this.cardData);
				this.showInfo = getDrawObj().templatelist[this.cardData.TemplateId - 1].info;
				let li = getDrawObj().templatelist[this.cardData.TemplateId - 1].bglist
				let list = []
				this.bglist.map((item, index) => {
					li.map((it, idx) => {
						if (index == it) { //只展示与模板适配的背景
							list.push({
								num: index, //指背景的序号
								src: item //背景的图片路径
							})
						} else {
							return
						}
					})
				})
				this.showbglist = list
			} catch (err) {
				console.info('异常：', err);
			}
		},
		methods: {
			bsSelected(idx) {
				this.cardData.TemplateId = idx + 1;
				drawCard('canvas', this.cardData);
				this.showInfo = getDrawObj().templatelist[this.cardData.TemplateId - 1].info;
				let li = getDrawObj().templatelist[this.cardData.TemplateId - 1].bglist
				let list = []
				this.bglist.map((item, index) => {
					li.map((it, idx) => {
						if (index == it) { //只展示与模板适配的背景
							list.push({
								num: index, //指背景的序号
								src: item //背景的图片路径
							})
						} else {
							return
						}
					})
				})
				this.showbglist = list
				this.cardData.TemplateBk = this.showbglist[0].num + 1
				// console.log("展示的背景",getDrawObj().templatelist[this.cardData.TemplateId - 1].bglist,this.showbglist);
			},
			bgSelected(idx) {
				this.cardData.TemplateBk = idx + 1;
				drawCard('canvas', this.cardData);
			},
			async saveClick() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					await editCard({
						Id: this.cardData.Id,
						TemplateId: this.cardData.TemplateId,
						TemplateBk: this.cardData.TemplateBk
					});
					reloadPrePage(1, "style"); //修改上一页
					this.$store.commit('SET_ISNEWCARD_DATA', true); //名片样式改变，设置名片重新刷新
					uni.navigateBack();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}

			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding-left: 30rpx;
		padding-right: 30rpx;
		background-color: #FFFFFF;
		margin-bottom: 20rpx;

		.mp {
			width: 690rpx;
			height: 400rpx;
			overflow: hidden;
			border-radius: 10rpx;
			box-sizing: border-box;
			color: #FFFFFF;
			position: relative;
			background-color: #14395F;
			box-shadow: 0px 6px 15px 0px rgba(0, 0, 0, 0.2000);


		}

	}

	.choose {
		margin-top: 61rpx;
		padding-left: 30rpx;
		padding-right: 30rpx;

		.title {
			box-sizing: border-box;
			width: 100%;
			font-size: 30rpx;
			color: #999999;
			margin-bottom: 30rpx;
		}

		.content {

			height: 87rpx;
			white-space: nowrap;
			width: 100%;

			::-webkit-scrollbar {
				background-color: white !important;
			}

			::-webkit-scrollbar-thumb {
				background-color: #FFFFFF !important;
			}

			.contwrap {


				.item {
					display: inline-block;
					margin-right: 22rpx;
					position: relative;

					.ac {
						position: absolute;
						top: 0;
						left: 0;
						width: 147rpx;
						height: 87rpx;
						background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
						background-image: url('/static/icon_choose_n.png');
						z-index: 9;
						background-size: 50rpx 50rpx;
						background-repeat: no-repeat;
						background-position: center;
					}

					.ximg {
						display: flex;
						width: 147rpx;
						height: 87rpx;
						overflow: hidden;
					}
				}
			}


		}

		.info {
			word-break: break-all;
			word-wrap: break-word;
		}
	}
</style>
