<template>
	<view>
		<view class="wrap">
			<view class="row">
				<text>手机号码</text>
				<view class="right">
					<uni-easyinput class="iptTxt" :clearable="isShowClear" :focus="phoneFocus" trim="both" @focus="inputFocus"
						maxlength="11" v-model="phone" :inputBorder="false" placeholder="请输入电话号码" @clear="cancel" @blur="verifyPhone">
					</uni-easyinput>
				</view>
			</view>
			<view class="row" v-if="imgid">
				<text>图形验证</text>
				<view class="right">
					<uni-easyinput class="iptTxt" :clearable="isShowClear" trim="both" @focus="inputFocus"
						 maxlength="11" v-model="imgCode" :inputBorder="false" placeholder="请输入图形验证码">
					</uni-easyinput>
					<image style="width:150rpx;height:60rpx" lazy-load :src="imgSrc" @click="imgClick" />
				</view>
			</view>
			<view class="row">
				<text>验证码</text>
				<view class="right">
					<!-- <input type="text" placeholder="请输入验证码" /> -->
					<uni-easyinput class="uni-mt-5" trim="all" maxlength="10" :focus="codeFocus" v-model="value" :inputBorder='showBorder'
						placeholder="请输入内容"></uni-easyinput>
					<wh-captcha ref="captcha" :secord="60" title="获取验证码" waitTitle="SECORD秒"
						normalClass="captcha-normal" disabledClass="captcha-disabled" @click="getCaptcha"></wh-captcha>
				</view>
			</view>
			<view class="common_btn" @click="bindPhone">
				完成绑定
			</view>
		</view>
	</view>
</template>

<script>
	import whCaptcha from '../../components/wh-captcha/wh-captcha.vue';
	import test from '@/common/ys-validate.js'
	import {
		getTelCode,
		bindTel,
		getTelImgCode
	} from '@/api/tel.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				phoneFocus:true,//是否聚焦到手机号码输入框
				phone: null,
				value: '',//短信验证码
				showBorder: false,
				isShowClear: false, //是否显示输入框删除标签
				imgCode: "",
				imgSrc: '',
				imgid: '', //
				sendTxt:'发送验证码',
				daonum: 0,
				codeFocus:false//验证码输入框是否获取焦点
			}
		},
		onLoad(options) {
			if (options.phone) {
				this.phone = options.phone
			}
			if (options.phone == 'null') {
				this.phone = ''

			}
			// console.log("options外部值",this.phone);
		},
		components: {
			whCaptcha
		},
		methods: {
			verifyPhone() {
				//验证手机号
				if (!(/^1[3456789]\d{9}$/.test(this.phone))) {
					uni.showToast({
						title: '手机号码格式错误',
						icon: "none"
					});
					this.phoneFocus=true
					return;
				}
			},
			cancel() {
				this.phone = '';
				this.phoneFocus=true
			},
			async bindPhone(){
				//绑定手机号
				if (this.phone == "") {
					uni.showToast({
						title: '请输入手机号码',
						icon: "none"
					});
					return;
				}
				if (this.value == "") {
					uni.showToast({
						title: '请输入验证码',
						icon: "none"
					});
					return;
				}
				if (!(/^1[3456789]\d{9}$/.test(this.phone))) {
					uni.showToast({
						title: '手机号码格式错误',
						icon: "none"
					});
					return;
				}
				uni.showLoading({
					title: '加载中...'
				});
				let rs=await bindTel({
					phone:this.phone,
					code:this.value
				})
				// console.log("短信验证码",rs);
				if(rs.code==0){
					uni.showToast({
						icon: 'success',
						title: '绑定成功',
						duration: 1000
					});
					reloadPrePage(1,'phone');
					setTimeout(() => {
						uni.navigateBack();
					}, 1000);
				}
			},
			inputFocus() {
				//获取焦点
				this.isShowClear = true
			},
			imgClick() {
				//获取图形验证码
				getTelImgCode().then(res => {
					// console.log("图形验证码",res);
					this.imgid = res.data.uuid;
					// console.log("图形验证码uuid",this.imgid);
					this.imgSrc = res.data.base64;
				})
			},
			async getCaptcha() {
				// console.log("imgCode", this.imgCode);
				if (this.phone == "") {
					uni.showToast({
						title: '请输入手机号码',
						icon: "none"
					});
					return;
				}

				if (!(/^1[3456789]\d{9}$/.test(this.phone))) {
					uni.showToast({
						title: '手机号码格式错误',
						icon: "none"
					});
					return;
				}
				if (this.daonum > 0) {
					uni.showToast({
						title: '两分钟内不能重复发短信',
						icon: "none"
					});
					return;
				}
				let imgtxt = this.imgCode.trim();
				if (this.imgid != "") {
					if (imgtxt == "") {
						uni.showToast({
							title: '请输入图形验证码',
							icon: "none"
						});
						return;
					}
				}
				uni.showLoading({
					title: '加载中...'
				});
				getTelCode({
					phone: this.phone,
					code: this.imgCode,
					imgid: this.imgid
				}).then(data => {
					uni.hideLoading();
					if (data.code == 0) {
						uni.showToast({
							title: '发送成功',
							icon: "none"
						});
						this.codeFocus=true
						if (this.$refs.captcha.canSend()) {
							this.$refs.captcha.begin()
						}
					} else {
						uni.showToast({
							title: data.Message,
							icon: "none"
						});
					}
				}).catch(err=>{
					// console.log("错误",err,this.imgCode);
					let imgtxt = this.imgCode.trim();
					if (err.code == 102) {
						this.imgClick();
						if (imgtxt == "") {
							uni.showToast({
								title: '当前ip发送次数太多,需要图形验证码',
								icon: "none",
								duration:2000
							});
						} else {
							uni.showToast({
								title: '图形验证码错误',
								icon: "none"
							});
						}
					
					}
				})
				
			}
		}
	}
</script>

<style lang="scss">
	.wrap {
		width: 100%;

		.row {
			width: 100%;
			height: 120rpx;
			display: flex;
			align-items: center;
			padding: 0 30rpx;
			// box-sizing: border-box;
			background-color: #FFFFFF;
			font-size: 30rpx;
			color: #666666;
			border-top: 1rpx solid #F6F6F6
		}

		.row>text {
			font-weight: bold;
			width: 120rpx;
			margin-right: 60rpx;
		}

		.row .right {
			display: flex;
			align-items: center;

			input {
				display: block;
				outline: none;
				flex: 1;
				height: 120rpx;
				margin-right: 8rpx;
				border: none;
			}

			input::-ms-input-placeholder {
				color: #999999;
				border: none;
			}

			.yzm {
				width: 155rpx;
				height: 60rpx;
				line-height: 60rpx;
				color: #50A6FA;
				border: 1rpx solid #50A6FA;
				font-size: 28rpx;
				text-align: center;
				border-radius: 6rpx;
			}
		}

		.common_btn {
			margin-top: 103rpx;
		}

		// 验证码样式
		.captcha-normal {
			width: 155rpx;
			height: 60rpx;
			line-height: 60rpx;
			color: #50A6FA;
			border: 1rpx solid #50A6FA;
			font-size: 28rpx;
			text-align: center;
			border-radius: 6rpx;
		}

		.captcha-disabled {
			width: 155rpx;
			height: 60rpx;
			line-height: 60rpx;
			color: #50A6FA;
			border: 1rpx solid #50A6FA;
			font-size: 28rpx;
			text-align: center;
			border-radius: 6rpx;
		}
	}
</style>
