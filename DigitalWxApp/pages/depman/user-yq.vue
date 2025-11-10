<template>
	<view>
		<view class="tipWrap">
			<image class="tipImg" mode="aspectFill" src="/static/logo.png" />
			<text class="tipTxt">邀请员工一起使用名片吧</text>
		</view>

		<view class="yq-form">
			<navigator url="" class="xrow" @click="onPopDept">
				<text class="l">所在部门</text>
				<view class="r">
					<text class="txt">{{yqdeptName}}</text>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</navigator>
			<view class="mobile-ipt border-top">
				<uni-easyinput type="number" trim="both" maxlength="11" v-model="yqmobile" :inputBorder="false"
					placeholder="输入员工的手机号">
				</uni-easyinput>
			</view>
		</view>
		<view class="btn-wrap">
			<button class="btn" @click="onYqClick">邀请</button>
		</view>
		<view class="other">
			<text class="tip">不记得手机号？可以通过下方其它邀请方式</text>
			<view class="ft">其它邀请方式</view>
			<view class="itt-wrap">
				<button open-type="share" class="yq-it" style="margin-right: 60rpx;">
					<uni-icons custom-prefix="my-icon" type="my-icon-weixin" size="30" color="#54cd86"></uni-icons>
					<text style="margin-top: 10rpx;">微信邀请</text>
				</button>

				<navigator :url="'/pages/depman/user-yq-batch?dp='+yqdeptId" class="yq-it">
					<view class="plyq">
						<uni-icons custom-prefix="my-icon" type="my-icon-yaoqingyuangong" size="18" color="#FFFFFF">
						</uni-icons>
					</view>
					<text style="margin-top: 10rpx;">批量邀请</text>
				</navigator>
			</view>

		</view>

		<uni-data-picker ref="bmpick" popup-title="请选择部门" :value="yqdeptId" :localdata="deptTree" @change="onChangeDep"
			:map="{text:'label',value:'id',children:'children'}">
		</uni-data-picker>
	</view>
</template>

<script>
	import {
		getDeptTree,
		getDeptInfo
	} from '@/api/dept'
	import {
		inviteOrg,
		sendYqSms,
		getOrg
	} from '@/api/org'
	export default {
		data() {
			return {
				yqmobile: "",
				yqdeptId: 0,
				yqdeptName: "",
				deptTree: [],
				orgId: 0
			}
		},
		async onLoad(options) {
			this.yqdeptId = parseInt(options.dp);
			let depRsp = await getDeptInfo(this.yqdeptId);
			this.orgId = depRsp.data.OrgId;
			getDeptTree({
				OrgId: this.orgId
			}).then(rsp => {
				this.deptTree = rsp.data;
				let it = this.findTreeName(rsp.data[0], this.yqdeptId);
				this.deptInit(rsp.data[0]);
				this.yqdeptName = it.label;
			})
		},
		// 分享到朋友
		async onShareAppMessage(res) {
			uni.showLoading({
				title: '加载中...'
			});
			let yqcode;
			let orgName;
			let orgLogo;
			try {
				let rsp = await inviteOrg({
					OrgId: this.orgId,
					DeptId: this.yqdeptId,
					Limit: true,
					YQCodeType: 1
				});
				yqcode = rsp.data;
				// console.log("微信邀请",yqcode);
				let orgRsp = await getOrg(this.orgId);
				orgName = orgRsp.data.OrgName;
				orgLogo = orgRsp.data.Logo;
				if (orgLogo == "") {
					orgLogo = "/static/user/company.png";
				}
			} catch (err) {
				console.info('异常：', err);
			} finally {
				uni.hideLoading();
			}
			return {
				title: "你好，邀请您加入企业'" + orgName + "'",
				path: '/pages/index?jt=2&yq=' + yqcode,
				imageUrl: orgLogo
			};
		},
		methods: {
			onPopDept() {
				this.$refs.bmpick.show();
			},
			onChangeDep(e) {
				if (e.detail.value.length > 0) {
					this.yqdeptId = e.detail.value[e.detail.value.length - 1].value;
					if (this.yqdeptId == -1) {
						this.yqdeptId = e.detail.value[e.detail.value.length - 2].value;
						this.yqdeptName = e.detail.value[e.detail.value.length - 2].text;
					} else {
						this.yqdeptName = e.detail.value[e.detail.value.length - 1].text;
					}

				}

			},
			deptInit(item) {
				if (!item.hasOwnProperty("children") || item.children.length == 0) {
					return;
				}
				for (let i = 0; i < item.children.length; i++) {
					this.deptInit(item.children[i]);
				}
				item.children.splice(0, 0, {
					"label": "--",
					"id": -1
				});
			},
			findTreeName(item, id) {
				if (item.id == id) {
					return item;
				}
				if (!item.hasOwnProperty("children")) {
					return null;
				}
				for (let i = 0; i < item.children.length; i++) {
					let rt = this.findTreeName(item.children[i], id);
					if (rt != null) {
						return rt;
					}
				}
				return null;
			},
			async onYqClick() {
				if (this.yqmobile == "") {
					uni.showToast({
						title: "请输入员工的手机号",
						icon: "none",
						duration: 2000
					});
					return;
				}

				uni.showLoading({
					title: '加载中...'
				});

				try {
					let usrInfo = await this.$store.dispatch("userInfo");
					let rsp = await inviteOrg({
						OrgId: usrInfo.OrgId,
						DeptId: this.yqdeptId,
						Limit: true,
						YQCodeType: 1
					});
					rsp = await sendYqSms({
						tel: this.yqmobile,
						tk: rsp.data
					});
					uni.showToast({
						icon: 'success',
						title: '发送成功',
						duration: 1500
					});
					setTimeout(() => {
						uni.navigateBack();
					}, 1500);
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
	.uni-data-tree-input {
		display: none;
	}

	button::after {
		border: none;
	}

	.tipWrap {
		display: flex;
		flex-direction: column;
		align-items: center;
		margin-top: 40rpx;

		.tipImg {
			width: 200rpx;
			height: 200rpx;
			border-radius: 50%;
			overflow: hidden;
		}

		.tipTxt {
			margin-top: 30rpx;
		}
	}

	.yq-form {
		margin-top: 50rpx;
		background-color: #fff;

		.mobile-ipt {
			height: 90rpx;
			display: flex;
			align-items: center;

			.uni-easyinput__content-input {
				text-align: center;
			}
		}

		.xrow {
			height: 90rpx;
			padding: 0 30rpx;
			display: flex;
			align-items: center;
			justify-content: space-between;

			.l {}

			.r {
				display: flex;
				align-items: center;

				.txt {
					margin-right: 20rpx;
					color: #666;
				}
			}
		}
	}

	.btn-wrap {
		margin-top: 50rpx;

		.btn {
			margin-left: 60rpx;
			margin-right: 60rpx;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
		}
	}

	.other {
		margin-top: 30rpx;
		display: flex;
		flex-direction: column;
		align-items: center;

		.tip {
			font-size: 24rpx;
			color: #999;
		}

		.ft {
			position: relative;
			margin-top: 60rpx;
			font-size: 24rpx;
		}

		.ft::before {
			content: ' ';
			position: absolute;
			left: -180rpx;
			top: 18rpx;
			width: 160rpx;
			height: 1px;
			border-top: 1px solid transparent;
			border-image: -webkit-radial-gradient(#F6F6F6, #CCCCCC) 1 10;
		}

		.ft::after {
			content: ' ';
			position: absolute;
			right: -180rpx;
			top: 18rpx;
			width: 160rpx;
			height: 1px;
			border-top: 1px solid transparent;
			border-image: -webkit-radial-gradient(#F6F6F6, #CCCCCC) 1 10;
		}

		.itt-wrap {
			display: flex;

			.yq-it {
				margin-top: 30rpx;
				display: flex;
				flex-direction: column;
				align-items: center;
				padding: 14rpx;
				font-size: 28rpx;
				line-height: 1;
			}

			.plyq {
				background-color: #00aaff;
				border-radius: 50%;
				width: 30px;
				height: 30px;
				display: flex;
				align-items: center;
				justify-content: center;
			}
		}

	}
</style>
