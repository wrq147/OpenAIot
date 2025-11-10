<template>
	<view>
		<view style="padding-top: 30vh;">
			<uni-load-more iconType="circle" status="loading" />
		</view>
	</view>
</template>

<script>
	import {
		isJoinOrg
	} from '@/api/org.js'
	var App = getApp();
	var jt = App.globalData.jt;
	var info = App.globalData.info;
	var cid = App.globalData.cid;
	export default {
		data() {
			return {
				isFirst: true,
				options: null,
			}
		},
		onLoad: function(options) {
			this.options = options;
			console.log('新进应用首页接收数据', options);
			if(jt&&info){
				this.options.jt=jt
				this.options.info=info
			}
			if(jt&&cid){
				this.options.jt=jt
				this.options.cid=cid
			}
			console.log('首页接收数据this.options', this.options);
			uni.login({
				provider: 'weixin',
				success: loginRes => {
					this.$store.dispatch("weiXinLogin", {
						code: loginRes.code,
						created: true
					}).then(x => {

						this.dealIn(x.isnew);
						// this.$store.commit('SET_CESHI_DATA', false);
						// console.log("在验证登录处",x);
					});

				}
			});
		},
		onLaunch: function(options) {
			console.log("index.vue页面onLaunch的值", options);
			if (options.query && options.query != {}) {
				this.options = options;
				uni.login({
					provider: 'weixin',
					success: loginRes => {
						this.$store.dispatch("weiXinLogin", {
							code: loginRes.code,
							created: true
						}).then(x => {

							this.dealIn(x.isnew);
							// this.$store.commit('SET_CESHI_DATA', false);
							// console.log("在验证登录处",x);
						});

					}
				});
			}

		},
		methods: {
			dealIn: function(isFirst) {
				let jmptype = this.options.jt || "";
				// let backPage = "";
				let backPage = decodeURIComponent(this.options.t || "");
				console.log("yyyyyy", backPage, jmptype);
				//登录绑定成功处理
				if (backPage != "") {
					//返回上一页
					uni.redirectTo({
						url: backPage
					})
				} else {
					switch (jmptype) {
						case "1":
							//crm开通名片
							if (this.options.cid) {
								isJoinOrg({
									id: this.options.cid
								}).then((res) => {
									console.log("是否加入企业", res);
									if (res.data) { //如果已经加入企业则直接进首页
										uni.switchTab({
											url: '/pages/card_center/card_center'
										});
									} else { //如果没有加入企业则直接加入企业
										uni.redirectTo({
											url: "/pages/index_add?cid=" + this.options.cid
										})
									}
								})
							} else if (this.options.info) {
								// console.log("公众号是否有传递参数过来",this.options.info);
								let info = decodeURI(this.options.info)
								console.log("转义后", info);
								uni.redirectTo({
									url: "/pages/index_add?info=" + encodeURIComponent(info)
								})
							}

							break;
						case "2":
							//邀请进入小程序
							let yqcode = this.options.yq || "";
							uni.redirectTo({
								url: "/pages/company/company_join?yq=" + yqcode
							})
							break;
						case "3":
							//分享进入小程序
							// console.log('分享进入小程序处')
							if (isFirst) {
								uni.redirectTo({
									url: '/pages/card_user_add'
								})
							} else {
								break;
							}

						default:
							//直接进入
							if (isFirst) {
								uni.redirectTo({
									url: '/pages/card_user_add'
								})
							} else {
								uni.switchTab({
									url: "/pages/card_center/card_center"
								})
							}
							break;
					}
				}
			},

		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFF;
	}

	.bindPage {
		display: flex;
		flex-direction: column;
		align-items: center;

		.logo {
			margin-top: 200rpx;
			margin-bottom: 40rpx;

			.img {
				width: 200rpx;
				height: 200rpx;
				border-radius: 50%;
			}
		}

		.txt {
			margin-bottom: 80rpx;
		}

		.wxbt {
			width: 690rpx;
			height: 90rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			color: #FFF;
		}
	}
</style>