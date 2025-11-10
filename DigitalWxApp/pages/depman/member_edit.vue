<template>
	<view>
		<view class="common_wrap2">
			<view class="row" id="realNameId">
				<view class="mdtxt">姓名</view>
				<view class="right ipt">
					<view class="iptTxt" style="font-size:26rpx;color:rgba(112,114,121, 1);">
						{{memberInfo.RealName&&memberInfo.RealName!='微信用户'?memberInfo.RealName:'该成员未修改姓名'}}
					</view>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">头像</view>
				<view class="right">
					<!-- <view class="txt" style="width: 68rpx !important;"> -->
					<image class="avatar" :src="memberInfo.Avatar?memberInfo.Avatar:'../../static/user/user_man.png'"
						mode="aspectFill" @tap.stop="seeImg(memberInfo.Avatar)" @click.stop="seeImg(memberInfo.Avatar)"
						style="width: 68rpx;height: 68rpx;">
					</image>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">手机</view>
				<view class="right ipt">
					<!-- <uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(112,114,121, 1);"
						type="number" trim="both" maxlength="11" v-model.lazy="memberInfo.Mobile"
						:inputBorder="false" placeholder="请输入手机号码" >
					</uni-easyinput> -->
					<view class="iptTxt" style="font-size:26rpx;color:rgba(112,114,121, 1);">
						{{memberInfo.Mobile?memberInfo.Mobile:'该成员未绑定手机号码'}}
					</view>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">邮箱</view>
				<view class="right ipt">
					<view class="iptTxt" style="font-size:26rpx;color:rgba(112,114,121, 1);">
						{{memberInfo.Email?memberInfo.Email:'该成员未绑定邮箱'}}
					</view>
				</view>
			</view>
		</view>
		<view style="font-size:26rpx;color:rgba(112,114,121, 1);margin-top:20rpx;padding-left: 10rpx;">
			*该成员已加入企业，需成员自行修改基本信息
		</view>
		<view class="common_wrap2">
			<view class="row">
				<view class="mdtxt">部门</view>
				<view class="right ipt" @click="tochoiceDept">
					<uni-easyinput placeholder-style="font-size:26rpx;color:rgba(112,114,121, 1);" class="iptTxt"
						trim="both" maxlength="30" v-model="memberInfo.dept_name" :inputBorder="false"
						placeholder="请输入所属部门" :clearable="false">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<view class="mdtxt">职位</view>
				<view class="right ipt">
					<uni-easyinput class="iptTxt" placeholder-style="font-size:26rpx;color:rgba(112,114,121, 1);"
						trim="both" maxlength="50" v-model="memberInfo.post_name" :inputBorder="false"
						placeholder="请输入所属职位">
					</uni-easyinput>
				</view>
			</view>
		</view>
		<view class="common_wrap2" style="padding: 0;margin-top: 60rpx;">
			<view class="row btn_con">
				<view class="del_btn" @click="delMember">
					删除成员
				</view>
				<view class="btn" @click="saveInfo">
					保存信息
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getSelfInfo,
		getUsersInfo,
		editUser,
		getUserCompanyInfo
	} from '@/api/user.js'
	import {
		delCompanyMember,
		editMemberInfo
	} from '@/api/company.js'
	export default {
		data() {
			return {
				menId: '',
				memberInfo: {},
				pageLen: 0, //
				OrgId:0,//当前企业id
			}
		},
		onLoad: function(options) {
			this.menId = parseInt(options.id || "0");
			if (this.menId) {
				this.reloadpage()
			}
			if (options.pageLen) {
				this.pageLen = parseInt(options.pageLen || "0")
			}
		},
		methods: {
			async delMember() {
				//删除成员
				uni.showModal({
					title: '提示',
					content: '确定移除该成员吗？',
					success: async res => {
						if (res.confirm) {
							uni.showLoading({
								title: '加载中...'
							});
				
							try {
								let rsp=await delCompanyMember(this.menId)
								
								uni.hideLoading();
								uni.showToast({
									icon: 'success',
									title: '移除成功',
									duration: 1500
								});
								setTimeout(() => {
									uni.navigateBack({
										delta: this.pageLen
									});
								}, 1500);
							} catch (err) {
								console.info('异常：', err);
								uni.hideLoading();
							}
						}
					}
				})
				

			},
			async saveInfo() {
				if (this.memberInfo.dept_id && this.memberInfo.dept_name && this.memberInfo.post_name) {
					let rs = await editMemberInfo({
						userId: this.memberInfo.Id,
						orgId:this.OrgId,
						dept_id: this.memberInfo.dept_id,
						dept_name: this.memberInfo.dept_name,
						post_name: this.memberInfo.post_name
					})
					if (rs.code == 0) {
						uni.showToast({
							title: "保存成功",
							icon: 'success'
						})
						uni.navigateBack({
							delta: this.pageLen>0?this.pageLen:1
						});
					}
				} else if (this.memberInfo.dept_id && this.memberInfo.dept_name) {
					let rs = await editMemberInfo({
						userId: this.memberInfo.Id,
						orgId:this.OrgId,
						dept_id: this.memberInfo.dept_id,
						dept_name: this.memberInfo.dept_name,
					})
					if (rs.code == 0) {
						uni.showToast({
							title: "保存成功",
							icon: 'success'
						})
						uni.navigateBack({
							delta: this.pageLen>0?this.pageLen:1
						});
					}
				} else if (this.memberInfo.post_name) {
					let rs = await editMemberInfo({
						userId: this.memberInfo.Id,
						orgId:this.OrgId,
						post_name: this.memberInfo.post_name
					})
					if (rs.code == 0) {
						uni.showToast({
							title: "保存成功",
							icon: 'success'
						})
						uni.navigateBack({
							delta: this.pageLen>0?this.pageLen:1
						});
					}
				}

			},
			tochoiceDept() {
				//跳转去选择用户
				uni.navigateTo({
					url: '/pages/depman/only_dept_import'
				});
			},
			choiceDept(params) {
				if (params && params.id) {
					this.memberInfo.dept_id = params.id
					this.memberInfo.dept_name = params.label
					this.$forceUpdate()
				}
			},
			async reloadpage(name) {
				try {
					let rsp = await getUserCompanyInfo(this.menId);
					this.memberInfo = rsp.data
					let usrInfo = await this.$store.dispatch("userInfo");
					this.OrgId=usrInfo.OrgId
					// console.log('成员信息',rsp,usrInfo);

				} catch (e) {
					//TODO handle the exception
					console.info(e);
				}
			},
			seeImg(img) { //预览头像
				if (img) {
					let list = []
					list.push(img)
					uni.previewImage({
						urls: list,
						longPressActions: {
							success: function(data) {

							},
							fail: function(err) {
								console.log(err.errMsg);
							}
						}
					});
				}
			},
		}
	}
</script>

<style lang="scss">
	.common_wrap2 {
		width: 100%;
		background-color: #FFFFFF;
		margin-top: 20rpx;
		padding-top: 20rpx;
		padding-bottom: 20rpx;

		.title {
			display: flex;
			height: 90rpx;
			font-size: 30rpx;
			color: #333333;
			padding: 0 30rpx;
			align-items: center;
			justify-content: space-between;
		}

		.border-all:after {
			border-radius: 4rpx;
		}

		.jswrap {
			display: flex;
			flex-direction: column;
			padding: 0 30rpx;

			.uptip {
				color: #50A6FA;
				display: flex;
				justify-content: center;
				padding-top: 60rpx;
				padding-bottom: 10rpx;
			}

			.dtip {
				font-size: 22rpx;
				color: #666666;
				display: flex;
				justify-content: center;
				padding-bottom: 50rpx;
			}

		}

		.row {
			width: 100%;
			min-height: 90rpx;
			display: flex;
			box-sizing: border-box;
			padding: 0 30rpx;
			height: 90rpx;
			align-items: center;

			&.btn_con {
				padding: 0;
			}

			.del_btn {
				width: 50%;
				text-align: center;
				color: #ff0000;
				height: 90rpx;
				line-height: 90rpx;
				// border: 1rpx solid #999999;
			}

			.btn {
				width: 50%;
				text-align: center;
				color: #FFFFFF;
				height: 90rpx;
				line-height: 90rpx;
				background-color: #50A6FA;
			}

			.mdtxt {
				color: #333333;
				font-size: 30rpx;
				width: 190rpx;
				text-align: left;
				display: flex;
				align-items: center;

				.lbred {
					color: #ff0000;
					margin-left: 10rpx;
					margin-top: 10rpx;
				}
			}

			.right {
				display: flex;
				align-items: center;
				font-size: 28rpx;
				color: #333333;
				flex: 1;
				justify-content: space-between;

				&.ipt {
					margin-left: -10px;
					font-size: 26rpx;
				}

				&.tishi {
					color: #999999;
					margin-left: 0;
					font-size: 24rpx;
				}

				.mbwrap {
					display: flex;
					justify-content: space-between;
					align-items: center;
					flex: 1;

					.pbimg {
						width: 73rpx;
						height: 43rpx;
					}

					.pbtip {
						color: #50A6FA;
						font-size: 24rpx;
					}
				}

				.txt {
					flex: 1;
					display: flex;
					align-items: center;

					image {
						width: 68rpx;
						height: 68rpx;
						border-radius: 10rpx;
						display: block;
					}

					&.txt-tip {
						font-size: 24rpx;
					}

					&.txt-tip2 {
						font-size: 24rpx;
						color: #999999;
					}
				}


				.iconwrap {
					width: 34rpx;
					height: 90rpx;
					display: flex;
					justify-content: flex-end;
					align-items: center;
					padding-left: 20rpx;
				}

			}

		}

	}
</style>
