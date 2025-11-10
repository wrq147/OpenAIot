<template>
	<view id="PoolDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Details"
			class="addPool-top"></top>
		<view class="PoolDetails-content">
			<view class="Avatar-information">
				<view class="iconfont t-icon-xiansuotouxiang">

				</view>
				<view class="Avatar-information-title">
					{{arr.CompanyName}}
				</view>

				<view class="Avatar-Contacts">
					<view class="Avatar-Contacts-left">
						<text>Contacts: </text>
						<!-- <text class="iconfont icon-a-youjiantoubai"></text> -->
					</view>
					<view class="Avatar-Contacts-right">
						<view class="Avatar-Contacts-right-David">
							<text class="aviter iconfont t-icon-morentouxiang"></text>
							<text class="content">{{arr.RealName}}</text>
						</view>
						<view class="iconfont t-icon-dianhua" @click="handMobile()">
							<!-- {{arr.Mobile}} -->
						</view>
					</view>
				</view>
			</view>
			<view class="navigation-nav">
				<view v-for="(item,index) in arr1" :key="index" class="navigation-nav-item" @click="handClickTo(index)"
					:class="[select==index?'active':'on']">
					<view>
						{{item.title}}
					</view>
					<view class="navigation-nav-border" v-if="select==index">

					</view>
				</view>
			</view>
			<!--Information-->
			<view v-if="select==0">
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item itemTextBorde" v-if="arr.LeaderName">
					<view class="Avatar-information-text-item-title">Manager</view>
					<view class="Avatar-information-text-item-header">
						<!-- <view class="Avatar-information-text-item-img"></view> -->
						<view class="content">{{arr.LeaderName}}</view><!--这个等级新增时候应该没有这个字段-->
					</view>
				</view>

				<view class="Avatar-information-text-item" v-if="arr.DeptName">
					<view class="Avatar-information-text-item-title">Department</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.DeptName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.PostName">
					<view class="Avatar-information-text-item-title">Position</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.PostName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Customer source</view>
					<view class="Avatar-information-text-item-header">
						<view class="content" v-if="arr.FromType=='weixin'">WeChat Clues</view>
						<view class="content" v-else-if="arr.FromType=='form'">Process Form</view>
						<view class="content" v-else>other</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.HelperName">
					<view class="Avatar-information-text-item-title">Collaborator</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.HelperName}}</view>
					</view>
				</view>
				<!-- <view class="Avatar-information-text-item itemTextBorde">
					<view class="Avatar-information-text-item-title">Customer details</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.Remark}}</view>
					</view>
				</view> -->
			</view>
			</view>
			<!--Follow-up-->
			<view v-if="select==1">
				<view class="Follow-up-input" @click="handAddFloow">
					<text class="iconfont icon-tianxiegenjinhepinglun"></text>
					<input class="searchInput" type="text" placeholder="Fill in follow-up updates">
				</view>
				
				<view class="Follow-up-item" v-for="(item,index) in FastTracking" :key="index">
					<view class="Follow-up-item-top">
						<img :src="item.FollowUserInfo.Avatar" class="aviter" alt="">
						<view class="Follow-up-item-top-content">
							<view>{{item.FollowUserInfo.RealName}}</view>
							<view class="time">{{item.createTime}}</view>
						</view>
					</view>
					<view class="Follow-up-item-center" v-html="item.Remark">
				
					</view>
					<view class="Follow-up-item-bottom">
						<text class="tit_text">Execution mode:</text>
						<text>{{item.FollowWay}}</text>
					</view>
				</view>
				<view style="height:50px;"></view>
			</view>
			<!--评论-->
			<view v-if="select==2">
				<view class="Follow-up-input" @click="handComment">
					<text class="iconfont icon-tianxiegenjinhepinglun"></text>
					<input class="searchInput" type="text" placeholder="Fill in a comment">
				</view>
				<view class="Comments-item" v-for="(item,index) in payList" :key="index" @click="handPayListDetails(item)" style="margin-bottom: 20rpx;">
					<view class="Comments-item-leftparse">
						<view class="Comments-item-left" style="margin-top:15rpx;">
							<img class="aviter" :src="item.UserInfo.Avatar" alt="">
						</view>
						<view class="Comments-item-txt">
							<view class="Comments-item-txt-title">{{item.UserInfo.RealName}}</view>
							<view class="Comments-item-txt-content" v-html="item.Content">
				
							</view>
							<view class="Comments-item-parse">
								<view>{{item.CreateOn}}</view>
								<view class="reply">
									<text class="iconfont icon-huifu"></text>
									<text>Reply</text>
								</view>
							</view>
						</view>
					</view>
					<view class="Comments-item" v-for="(row,index1) in item.children"
						style="padding-right:0rpx;margin-right:0rpx;padding-bottom: 0rpx;">
						<view class="Comments-item-leftparse">
							<view class="Comments-item-left" style="margin-top:15rpx;margin-left:32rpx;">
								<img class="aviter" :src="row.UserInfo.Avatar" alt="">
							</view>
							<view class="Comments-item-txt">
								<view class="Comments-item-txt-title">{{row.UserInfo.RealName}}</view>
								<view class="Comments-item-txt-content" v-html="row.Content">
				
								</view>
								<view class="Comments-item-parse">
									<view>{{row.CreateOn}}</view>
									<view class="reply">
										<text class="iconfont icon-huifu"></text>
										<text>Reply</text>
									</view>
								</view>
							</view>
						</view>
					</view>
				</view>
				<view style="height:50px;"></view>
			</view>
		</view>
		<view class="PoolDetails-Height"></view>
		<view class="PoolDetails-button">
			<view class="button" @click="handReturn()">
				<text class="iconfont icon-lingqu"></text>
				<text>return</text>
			</view>
			<view class="button" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>Edit</text>
			</view>
			<view class="button" @click="handDelete()">
				<text class="iconfont icon-shanchu"></text>
				<text>Delete</text>
			</view>
		</view>
		<view class="move" v-if="alertHide"></view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<!--添加弹窗-->
		<view class="AddPopUps" v-if="alertHide">
			<!--评论弹框-->
			<view v-if="alertName=='comment'">
				<view class="AddPopUps-title" style="margin-top:25rpx;">
					<view class="title">
						comment
					</view>
					<view class="iconfont t-icon-guanbidanchuang" @click="handClose">
			
					</view>
				</view>
				<view>
					<textarea v-model="content" placeholder="Please enter a comment" class="addPool-textarea"
						:rows="4" />
					<view class="addPool-button" @click="handCommentSubmit">
						confirm
					</view>
				</view>
			</view>
			<view class="AddPopUps-item" v-else>
				<view>
					<view class="AddPopUps-title" style="margin-top:25rpx;">
						<view class="title">
							Return to customer
						</view>
						<view class="iconfont t-icon-guanbidanchuang" @click="handClose">

						</view>
					</view>
					<view>
						<textarea value="" v-model="reason" placeholder="Please enter the reason for return"
							class="addPool-textarea" :rows="4" />
						<view class="addPool-button" @click="handConfirm">
							confirm
						</view>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		ClueDataDetails, //线索池详情
		ClueReturn, //退回
		culeRemove ,//删除
		flowDataRecord, //快速跟进详情
		commentList, //评论列表
		commentAdd //评论添加
	} from "@/api/crmApi";
	import {
		yuanAarngemnt //员工
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				alertName:'',
				alertHide: false,
				select: 0,
				arr1: [{
						title: 'Information'
					},
					{
						title: 'Follow-up'
					},
					{
						title: 'Comments'
					}
				],
				arr: [],
				editId: '',
				reason: '',
				FastTracking: [],
				ExecutionMode: new Map(),
				payList:[]
			}
		},
		async onLoad(option) {
			if (option.id) {
				this.list(option.id);
				this.listData(option.id);
				this.editId = option.id
				this.CommentData(option.id)
				let partUnit = await this.$store.dispatch("data/dictList", 'follow_way');
				partUnit.forEach((item) => {
					this.ExecutionMode.set(item.value, item.label);
				})
			}
		},
		methods: {
			//评论列表
			CommentData(id) {
				commentList({
					TargetType: "线索",
					TargetId: id,
					pageNum: 1,
					pageSize: 10,
				}).then((data) => {
					//console.log(data.data.List,'评论列表');
					//this.payList=data.data.List;
					var res1 = data.data.List;
					var listParse = []
					var listSon = []
					res1.map((row) => {
						if (row.ParentCommentUserId == 0) {
							listParse.push(row)
						} else {
							listSon.push(row)
						}
					})
					listParse.forEach((item, index) => {
						var str = []
						listSon.forEach((v) => {
							if (item.Id == v.ParentCommentId) {
								//console.log(item.value,v.ParentId,'v.ParentId')
								str.push(v)
			
							}
						})
						item['children'] = str;
					})
					this.payList = listParse
					//console.log(listParse,'评论列表分离')
				})
			},
			//评论
			handCommentSubmit() {
				if (this.content == '') {
					uni.showToast({
						title: 'Please enter the comment content！',
						icon: 'none'
					})
					return
				}
				if (this.parentCommentId && this.parentCommentUserId) {
					var data = {
						content: this.content,
						TargetType: "线索",
						TargetId: this.editId,
						parentCommentUserId: this.parentCommentUserId,
						parentCommentId: this.parentCommentId
					}
				} else {
					var data = {
						content: this.content,
						TargetType: "线索",
						TargetId: this.editId
					}
				}
				commentAdd(data).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Comment added successfully!',
							icon: 'none'
						})
						this.alertHide = false;
						this.CommentData(this.editId);
					}
				})
			},
			handPayListDetails(row) {
				this.content = ''
				this.alertHide = true
				this.alertName = 'comment'
				this.parentCommentId = row.Id
				this.parentCommentUserId = row.UserId
			},
			handComment() {
				this.alertHide = true
				this.alertName = 'comment'
				this.content = ''
				this.parentCommentId = ''
				this.parentCommentUserId = ''
			},
		handAddFloow() {
			uni.navigateTo({
				url: '../Customer/AddFollow-up?id=' + this.arr.Id+'&TargetType=1'
			})
		},
			listData(id) {
				flowDataRecord({
					TargetId: id,
					TargetType: 1,
					pageNum: 1,
					pageSize: 10
				}).then((res) => {
					this.FastTracking = res.data.List;
					res.data.List.map((item,index)=>{
						item.FollowWay=this.ExecutionMode.get(item.FollowWay)
					})
				})
			},
			handMobile(){
				uni.makePhoneCall({
					phoneNumber: this.arr.Mobile,// 这里就是自己要拨打的电话号码
					success: (res) => {
						console.log('调用成功!')
					},
					fail: (res) => {
						console.log('调用失败!')
					}
				})
			},
			confirmUnbind() {
				//删除
				culeRemove({
					id: this.editId
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Delete successful！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 500)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete() {
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers " + this.arr.CompanyName + "?"
				)
			},
			handConfirm() {
				if (this.reason == '') {
					uni.showToast({
						title: 'Please enter the reason for return',
						icon: 'none'
					})
					return
				}
				ClueReturn({
					id: this.editId,
					reason: this.reason
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Return successful！',
							icon: 'none'
						})
						this.alertHide = false
						this.reason = ''
						setTimeout(() => {
							setPagesParam('list')
						}, 500)

					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handClose() {
				this.alertHide = false;
			},
			handReturn() {
				this.alertHide = true
				this.alertName=''
			},
			handEdit() {
				uni.navigateTo({
					url: './addClue?id=' + this.editId
				})
			},
			handClickTo(inx) {
				this.select = inx
			},
			list(id) {
				ClueDataDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res)
						this.arr = res.data;
					}
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	
		.Comments-item-leftparse {
			display: flex;
		}
	
		.Comments-item {
			display: flex;
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			background: #1C2232;
			border-radius: 14rpx;
			color: #fff;
			flex-direction: column;
			.Comments-item-left {
				display: flex;
	
				.aviter {
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 20rpx;
				}
			}
	
			.Comments-item-txt {
				// margin-left: auto;
				width: 100%;
	
				.Comments-item-parse {
					display: flex;
					font-size: 24rpx;
					color: rgba(255, 255, 255, .6);
					margin-top: 15rpx;
					width: 100%;
	
					.reply {
						margin-left: auto;
					}
				}
	
				.Comments-item-txt-content {
					font-size: 29rpx;
				}
	
				.Comments-item-txt-title {
					color: rgba(255, 255, 255, .6);
					font-size: 26rpx;
				}
			}
		}
	
		.Follow-up-item {
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			margin-bottom: 20rpx;
			background: #1C2232;
			border-radius: 14rpx;
			color: #fff;
	
			.Follow-up-item-bottom {
				padding-top: 15rpx;
				font-size: 28rpx;
	
				.tit_text {
					color: rgba(255, 255, 255, .4);
					margin-right: 10rpx;
				}
			}
	
			.Follow-up-item-center {
				font-size:32rpx;
				margin-top:10rpx;
			}
	
			.Follow-up-item-top {
				display: flex;
				align-items: center;
	
				.Follow-up-item-top-content {
					font-size:28rpx;
					.time {
						color: rgba(255, 255, 255, .5);
						font-size: 24rpx;
					}
				}
	
				.aviter {
					float: left;
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 20rpx;
				}
			}
		}
	
		.Follow-up-input {
			display: flex;
			align-items: center;
			height: 88rpx;
			border: 1rpx solid rgba(255, 255, 255, .1);
			margin: 20rpx 20rpx;
			border-radius: 8rpx;
	
			.icon-tianxiegenjinhepinglun {
				color: rgba(255, 255, 255, .4);
				margin-left: 30rpx;
			}
	
			.searchInput {
				font-size: 28rpx;
				margin-left: 14rpx;
				color: #fff;
			}
	
			::-webkit-input-placeholder {}
		}
	
		.BottomPositioning {
			display: flex;
			position: fixed;
			height: 98rpx;
			background: #1C2232;
			bottom: 0rpx;
			color: #FFFFFF;
			left: 0rpx;
			width: 100%;
			align-items: center;
	
			.icon-shanchu {}
	
			.BottomPositioning-item {
				width: 33%;
				text-align: center;
				border-right: 1rpx solid rgba(255, 255, 255, .2);
	
				.iconfont {
					margin-right: 9rpx;
				}
			}
		}
	
		.Information-item {
			margin: 0rpx 3%;
			background: #1C2232;
			color: #fff;
			padding: 0rpx 20rpx;
	
			.Information {
				display: flex;
				padding: 30rpx 0rpx;
				flex-direction: column;
				margin-top: 20rpx;
				border-bottom: 1rpx solid rgba(255, 255, 255, .1);
	
				.Information-title {
					color: rgba(255, 255, 255, .6);
				}
	
				.Information-bottom {
					display: flex;
					align-items: center;
					margin-top: 10rpx;
				}
	
				.aviter {
					float: left;
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
					margin-right: 10rpx;
				}
	
			}
	
			.InformBorder {
				border: none;
			}
		}
	
		.navigation-nav {
			display: flex;
			color: #fff;
			margin-top: 30rpx;
	
			.navigation-nav-border {
				width: 30rpx;
				height: 5rpx;
				border-radius: 14rpx;
				background: #fff;
				margin: 0 auto;
				margin-top: 10rpx;
			}
	
			.navigation-nav-item {
				width: 33%;
				text-align: center;
				font-size:28rpx;
			}
	
			.active {}
	
			.on {
				color: rgba(255, 255, 255, .4);
			}
		}
	
		.CreateCustomer-header {
			.Avatar-information {
				position: relative;
				display: flex;
				flex-direction: column;
				color: #fff;
				background: #1C2232;
				text-align: center;
				padding: 30rpx 0rpx;
				margin: 0rpx 3%;
				padding-bottom: 20rpx;
				margin-top: 80rpx;
	
				.Avatar-Contacts {
					display: flex;
					flex-direction: column;
					width: 92%;
					// height: 160rpx;
					// line-height: 160rpx;
					margin: 10rpx 4%;
					padding-top: 15rpx;
					padding-bottom: 10rpx;
					border: 2rpx solid rgba(255, 255, 255, .2);
					border-radius: 8rpx;
	
					.Avatar-Contacts-right {
						display: flex;
						justify-content: space-between;
						padding: 0rpx 20rpx;
						align-items: center;
	
						// margin:15rpx 0rpx;
						.icon-dianhua {
							width: 90rpx;
							height: 90rpx;
							line-height: 90rpx;
							text-align: right;
							font-size: 52rpx;
						}
	
						.Avatar-Contacts-right-David {
							.content {
								height: 60rpx;
								line-height: 60rpx;
								margin-left: 15rpx;
							}
	
							.aviter {
								float: left;
								width: 60rpx;
								height: 60rpx;
								border-radius: 50%;
							}
						}
					}
	
					.Avatar-Contacts-left {
						display: flex;
						justify-content: space-between;
						padding: 0rpx 20rpx;
						margin: 5rpx 0rpx;
						color: rgba(255, 255, 255, .6);
	
						.Avatar-Contacts-iconfont {}
	
						.icon-a-youjiantoubai {
							font-size: 20rpx;
						}
					}
				}
	
				.Agent {
					position: absolute;
					right: 0rpx;
					top: 0rpx;
					// width:100rpx;
					padding: 0rpx 10rpx;
					text-align: center;
					font-size: 23rpx;
					border-top-right-radius: 8rpx;
					border-bottom-left-radius: 15rpx;
					background: linear-gradient(180deg, #EFA902, #F4BE3F);
				}
	
				.Avatar-information-title {
					margin-top: 35rpx;
					font-size: 32rpx;
				}
	
				.Avatar-information-content {
					color: rgba(255, 255, 255, .6);
					font-size: 24rpx;
					margin-top: 5rpx;
				}
	
				.t-icon-kehumorentouxiang {
					width: 140rpx;
					height: 140rpx;
					line-height: 140rpx;
					margin: 0 auto;
					margin-top: -90rpx;
					color: #1C2232;
					border-radius: 10rpx;
				}
			}
		}
	
	
	page {
		background: #161A26;

		.move {
			position: fixed;
			height: 100%;
			width: 100%;
			background: #000;
			opacity: .7;
			top: 0px;
			left: 0px;
			z-index: 222;
		}

		.AddPopUps {
			position: fixed;
			background: #161A26;
			z-index: 333;
			color: #fff;
			width: 90%;
			margin: 0px 5%;
			border-radius: 8rpx;
			top: 250rpx;

			.addPool-button {
				width: 100%;
				height: 80rpx;
				line-height: 80rpx;
				text-align: center;
				border-radius: 10rpx;
				color: #fff;
				margin: 0 auto;
				font-size:32rpx;
				background: linear-gradient(180deg, #ff3535, #ff613d);
			}

			.addPool-textarea {
				height: 290rpx;
				border: 1rpx solid rgba(255, 255, 255, .2);
				border-radius: 10rpx;
				margin: 20rpx auto;
				padding: 20rpx;
			}

			.addPool-selected {
				/deep/.uni-select__input-text {
					color: #fff !important;
				}

				/deep/.uni-select {
					height: 88rpx !important;
					border: 1rpx solid rgba(255, 255, 255, .2);
				}

				/deep/.uni-select__input-placeholder {
					font-size: 30rpx !important;
					color: rgba(255, 255, 255, .4) !important;
				}
			}

			.link {
				color: rgba(255, 255, 255, .4);
				margin: 0rpx 3%;
				margin-top: 30rpx;
			}

			.GenerateLink {
				text-align: center;
				height: 90rpx;
				border-radius: 6px;
				line-height: 90rpx;
				width: 94%;
				margin: 0 auto;
				margin-top: 20px;
				background: linear-gradient(180deg, #ff3535, #ff613d);
			}

			.lianUrlInput {
				height: 88rpx;
				line-height: 88rpx;
				padding: 0rpx 20rpx;
				// border:2rpx solid rgba(255, 255, 255, .2);
				border-radius: 4rpx;
				margin-top: 15rpx;
				display: flex;
				align-items: center;
				border-radius: 10rpx;

				.lianUrl-input {
					width: 100%;
				}
			}

			.AddPopUps-title {
				position: relative;
				// margin:20rpx 30rpx;
				align-items: center;

				.title {
					text-align: center;
					font-size: 34rpx;
				}

				.t-icon-guanbidanchuang {
					position: absolute;
					margin-left: auto;
					width: 40rpx;
					height: 40rpx;
					right: 0rpx;
					top: 5rpx;
				}
			}

			.AddPopUps-item {
				padding: 30rpx;

			}
		}
	}

	.navigation-nav {
		display: flex;
		color: #fff;
		margin-top: 30rpx;

		.navigation-nav-border {
			width: 30rpx;
			height: 5rpx;
			border-radius: 14rpx;
			background: #fff;
			margin: 0 auto;
			margin-top: 10rpx;
		}

		.navigation-nav-item {
			width: 33%;
			text-align: center;
			font-size:28rpx;
		}

		.active {}

		.on {
			color: rgba(255, 255, 255, .4);
		}
	}
</style>