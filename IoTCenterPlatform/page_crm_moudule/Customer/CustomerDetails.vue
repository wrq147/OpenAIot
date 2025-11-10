<template>
	<view id="CreateCustomer">
		<top :isleftBack="true" leftIcon="icon-fanhui" leftText="Back" backgroundColor="#161A26" title="Details"
			rightText="Invite" class="CRM-header">
			<template v-slot:top_right >
				<view @click.stop="handDelete()">
					Delete
				</view>
			</template>
			
		</top>
		<view class="CreateCustomer-header">
			<view class="Avatar-information">
				<view class="iconfont t-icon-kehumorentouxiang">

				</view>
				<view class="Avatar-information-title">
					{{details.CustomerName}}
				</view>
				<view class="Avatar-information-content">
					NO.{{details.CustomerNumber}}
				</view>
				<view class="Agent" v-if="details.CustomerType==0">
					Agent
				</view>
				<view class="DirectSales" v-if="details.CustomerType==1">
					Direct Sales
				</view>
			</view>
		</view>
		<view class="navigation-nav">
			<view v-for="(item,index) in arr" :key="index" class="navigation-nav-item" @click="handClickTo(index)"
				:class="[select==index?'active':'on']">
				<view>
					{{item.title}}
				</view>
				<view class="navigation-nav-border" v-if="select==index">

				</view>
			</view>
		</view>
		<view v-if="select==0">
			<view class="Information-item">

				<view class="Information InformBorder" style="margin-top:0rpx;">
					<view class="Information-title">
						Manager
					</view>
					<view class="Information-bottom">
						<text>{{details.LeaderName}}</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;">
					<view class="Information-title">
						Department
					</view>
					<view class="Information-bottom">
						<text>Sales center</text>
					</view>
				</view>

				<view class="Information">
					<view class="Information-title">
						Collaborator
					</view>
					<view class="Information-bottom" v-for="(item,index) in details.HelperUsers" :key="index"
						style="margin-top:20rpx;">
						<img :src="item.Avatar" alt="" class="aviter">
						<text>{{item.RealName}}</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;">
					<view class="Information-title">
						Customer source
					</view>
					<view class="Information-bottom">
						<text v-if="details.FromType=='weixin'">WeChat Clues</text>
						<text v-if="details.FromType=='form'">Process Form</text>
						<text v-if="details.FromType=='other'">other</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;" v-if="details.CompanyUrl">
					<view class="Information-title">
						Company website
					</view>
					<view class="Information-bottom">
						<text>{{details.CompanyUrl}}</text>
					</view>
				</view>
			</view>
		</view>
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
		<view class="BottomPositioning">
			<view class="BottomPositioning-item" @click="handReturn('Return')">
				<text class="iconfont icon-tuihui"></text>
				<text>Return</text>
			</view>
			<view class="BottomPositioning-item" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>Edit</text>
			</view>
			<view class="BottomPositioning-item" style="border:none;">
				<view v-if="details.BindOrgId==0" class="right_top" @click.stop="handInvite()" style="font-size:15px;">
					invite
				</view>
				<view v-if="details.BindOrgId>0" class="right_top" @click.stop="handCancelInvite()" style="font-size:15px;">
					Un-invite
				</view> 
			</view>
			
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<view class="move" v-if="alertHide"></view>
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
			<!--回退弹框-->
			<view v-if="alertName=='Return'">
				<view class="AddPopUps-title" style="margin-top:25rpx;">
					<view class="title">
						Return to customer
					</view>
					<view class="iconfont t-icon-guanbidanchuang" @click="handClose">

					</view>
				</view>
				<view>
					<textarea v-model="reason" placeholder="Please enter the reason for return" class="addPool-textarea"
						:rows="4" />
					<view class="addPool-button" @click="handConfirm">
						confirm
					</view>
				</view>
			</view>
			<!--删除为空-->
			<view v-if="alertName==''">
				<view class="AddPopUps-item">
					<view class="AddPopUps-title">
						<view class="title">
							Create Employee
						</view>
						<view class="iconfont t-icon-guanbidanchuang" @click="handClose">

						</view>
					</view>
					<view v-if="tipsValue==''">
						<view class="link">
							Agent's factory
						</view>
						<view class="lianUrlInput">
							<uni-data-select v-model="inviteForm.factoryId" :isDark="true" :localdata="AgentFactory"
								type="line" placeholder="Please select a customer type"
								class="addPool-selected"></uni-data-select>
						</view>
						<view class="link">
							Select the region where the agent belongs
						</view>
						<view class="lianUrlInput">
							<view class="view_input" @click="choiceWZ">
								<view class="pal_col" v-if="!wzValue||wzValue.length==0">
									Please select the region
								</view>
								<view class="view_li_con personnel_li_con" v-if="wzValue&&wzValue.length>0">
									<view class="view_li_cot personnel_li_cot">
										<view class="view_li" v-for="(item,inx) in wzValue">
											<view class="view_text">
												{{item}}
											</view>
										</view>
									</view>
								</view>
								<view class="view_mask"></view>
								<view class="form_sel_icon">
									<custom-icons iconsName="icon-xialajiantou" iconsSize="14rpx"
										iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								</view>
							</view>
						</view>
						<view class="GenerateLink" @click="createLink()">Generate Link</view>
					</view>
					<view v-else class="copyInvite">
						<view class="copyInvite-title">
							Invite through link
						</view>
						<view class="copyInvite-lianUrl">
							<view class="copyInvite-lianUrl-url">
								{{tipsValue}}
							</view>
						</view>
						<view class="copuUrl" @click="copy(tipsValue)">
							copy link
						</view>
						<view class="validityPeriod">
							Link validity period： <text style="color:#FF3535;"> 7 </text>Invitation link expired after days
						</view>
					</view>

				</view>
			</view>
		</view>
		

			<uni-popup ref="clearAlarmPopup" type="center" :mask-click="false" :zIndex="999">
				<view class="notice_con">
					<view class="notice_cont">
						<view class="title">cancel invitation</view>
						<view class="conten_icon_con">
							<view style="color:#fff;margin-bottom: 20rpx;">
								Do you want to cancel the proxy permission?
							</view>
							<view class="conten_icon t-icon-kaibeifen" v-if="clearForm.isFilterDevice"
								@click.stop="setDeviceFilter(false)"></view>
							<view class="conten_icon t-icon-guan" v-if="!clearForm.isFilterDevice"
								@click.stop="setDeviceFilter(true)"></view>
						</view>
						<button class="submit_button" @click.stop="getWarnMsg()">
							Confirm
						</button>
						<view class="close_icon" @click="noticeColse">
							<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
								iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
						</view>
					</view>
				</view>
			</uni-popup>
			<sx-address-picker ref="areaTree" v-model="addressPopup" @confirm="addressConfirm($event, 'formData2')" :selected="wzCode"></sx-address-picker>
	</view>
</template>

<script>
	import serverUrl from '@/common/constVar.js'
	import {
		flowDataRecord, //快速跟进详情
		DataPoolsDetails, //客户详情（私海）
		customerDelete, //删除
		AuthorizationData,
		customerInvite, //邀请
		customerReturn, //退回
		commentList, //评论列表
		commentAdd ,//评论添加
		cancelInvitation//取消邀请
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
				addressPopup:false,
				wzCode: [],
				wzValue: [],
				wzItems: [],
				wzValPar: [],
				wzValPar2: [],
				clearForm: {
					isFilterFun: false,
					isFilterDevice: false,
					mark: ""
				},
				status: 'loading',
				alertName: '',
				select: 0,
				arr: [{
						title: 'Information'
					},
					{
						title: 'Follow-up'
					},
					{
						title: 'Comments'
					}
				],
				details: [],
				EditId: '',
				alertHide: false,
				AgentFactory: [],
				comSizeItems: [],
				inviteForm: {
					regions: '',
					factoryId: '',
					bindCustomerId: ''
				},
				tipsValue: '',
				reason: '',
				FastTracking: [],
				ExecutionMode: new Map(),
				yuanGong: new Map(),
				payList: [],
				content: '',
				parentCommentId: '',
				parentCommentUserId: '',
			}
		},
		async onLoad(option) {
			if (option.id) {
				this.list(option.id);
				this.listData(option.id);
				this.EditId = option.id
				this.CommentData(option.id)
				let partUnit = await this.$store.dispatch("data/dictList", 'follow_way');
				setTimeout(async ()=>{
					this.wzItems = await this.$store.dispatch("data/areaTree");
				},2000)
			//	this.wzItems = await this.$store.dispatch("data/areaTree");
				partUnit.forEach((item) => {
					this.ExecutionMode.set(item.value, item.label);
				})

			}
		},
		methods: {
			
			addressConfirm(address, key) {
				this.wzValue=[]
				this.wzCode=[]
				const [country, province, city] = address
				let address2 = province ? address.filter((v, i) => i > 0) : address;
				address2.map(row => {
					this.wzValue.push(row.Name)
					this.wzCode.push(row.Id)
				})
			},
			noticeColse() {
				this.$refs.clearAlarmPopup.close()
			},
			setDeviceFilter(val) {
				//是否过滤设备
				this.clearForm.isFilterDevice = val
			},
			_confirm(val) {
				//最后选择结果
				 console.log(val, '结果');
				this.wzValue = []
				this.wzCode = []
				this.wzValPar = []
				val.map(row => {
					let parStr = row.parentNameList.join('/')
					if (parStr) {
						parStr = parStr + '/' + row.Name
					} else {
						parStr = row.Name
					}
					this.wzValue.push(parStr)
					this.wzCode.push(row.Id)
					if (row.parentIdList && row.parentIdList.length > 0) {
						this.wzValPar.push(row.parentIdList)
					}
				})
				this.inviteForm.regions=this.wzCode.join(',')
			},
			getWarnMsg(){
				cancelInvitation({
					id:this.EditId,
					cancelAgent:this.clearForm.isFilterDevice
				}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title: 'Cancel successful!',
							icon: 'none'
						});
						this.$refs.clearAlarmPopup.close()
						//this.list(this.EditId);
						setTimeout(() => {
							setPagesParam('list')
						}, 500)
					}
				})
			},
			choiceWZ() {
				//选择位置
				//打开选择器
				// this.wzValPar2 = []
				
				// this.wzValPar.map(row => {
				// 	this.wzValPar2 = [...this.wzValPar2, ...row]
				// })
				// this.$refs.gqTree._show();
				this.addressPopup=true
			},
			//评论列表
			CommentData(id) {
				commentList({
					TargetType: "客户",
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
						TargetType: "客户",
						TargetId: this.EditId,
						parentCommentUserId: this.parentCommentUserId,
						parentCommentId: this.parentCommentId
					}
				} else {
					var data = {
						content: this.content,
						TargetType: "客户",
						TargetId: this.EditId
					}
				}
				commentAdd(data).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Comment added successfully!',
							icon: 'none'
						})
						this.alertHide = false;
						this.CommentData(this.EditId);
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
			listData(id) {
				flowDataRecord({
					TargetId: id,
					TargetType: 0,
					pageNum: 1,
					pageSize: 10
				}).then((res) => {
					this.FastTracking = res.data.List;
					res.data.List.map((item,index)=>{
						item.FollowWay=this.ExecutionMode.get(item.FollowWay)
					})
				})
			},
			handAddFloow() {
				uni.navigateTo({
					url: './AddFollow-up?id=' + this.EditId
				})
			},
			handConfirm() {
				if (this.reason == '') {
					uni.showToast({
						title: 'Please enter the reason for return',
						icon: 'none'
					})
					return
				}
				customerReturn({
					id: this.EditId,
					reason: this.reason
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Return successful！',
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
			handReturn(name) {
				this.alertHide = true
				this.alertName = name;
			},
			handClose() {
				this.alertHide = false;
			},
			copy(context) { //context被复制的内容
			
				const that = this
				//复制链接
				console.log("复制链接", context);
				uni.setClipboardData({
					data: context,
					success: () => {
						
					},
					fail: (err) => {
						uni.showToast({
							title: 'copy failed',
							duration: 2000,
							icon: 'none'
						});
					}
				});
			},
			createLink() {
				//生成邀请链接
				// console.log(this.inviteForm,'this.inviteForm')
				// return
				if (this.inviteForm.factoryId == '') {
					uni.showToast({
						title: 'Please select a proxy factory',
						icon: 'none'
					})
					return
				}

				customerInvite(this.inviteForm).then(res => {
					let yuming = serverUrl.getServerUrl()+'/jump.html';
					// let yuming='http://52.28.35.196/#/index'
					if (res.code == 0) {
						uni.showToast({
							title: 'Successfully generated！',
							icon: 'none'
						})
						this.tipsValue = yuming + "?yaoqingId=" + res.data+'&isMobile=true';
						console.log(this.tipsValue, 'tipsValue')
					}
				}).catch((err)=>{
					this.setMsgTop(err)
				});;
			},
			handCancelInvite(){
				this.$refs.clearAlarmPopup.open()
				this.EditId=this.details.Id
			},
			handInvite() {
				this.alertName = ''
				this.inviteForm = {
						regions: '',
						factoryId: '',
						bindCustomerId: ''
					},
					this.tipsValue = ''
				this.alertHide = true;
				this.inviteForm.bindCustomerId = this.EditId
				
				AuthorizationData().then((res) => {
					//console.log(res,'AuthorizationData')
					this.AgentFactory = []
					res.data.forEach((ite) => {
						this.AgentFactory.push({
							value: ite.FactoryId,
							text: ite.FactoryName
						})
					})
				})
			},
			confirmUnbind() {
				//删除
				customerDelete({
					id: this.EditId
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
					"Are you sure to delete the customers " + this.details.CustomerName + "?"
				)
			},
			handEdit() {
				uni.navigateTo({
					url: './CreateCustomer?id=' + this.EditId
				})
			},
			handClickTo(inx) {
				this.select = inx
			},
			list(id) {
				DataPoolsDetails({
					id: id
				}).then((res) => {
					console.log(res,'111')
					this.details = res.data;
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;
	.view_input {
			width: 100%;
			height: 88rpx;
			line-height: 88rpx;
			background-color: rgba(22, 26, 38, 1);
			border-radius: 10rpx;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			color: #FFFFFF;
			font-size: 28rpx;
			padding: 10rpx 0;
			box-sizing: border-box;
			position: relative;
			display: flex;
			justify-content: flex-start;
			align-items: center;
			overflow: hidden;
			.tips_input{
				display: flex;
				justify-content: flex-start;
				align-items: center;
				overflow-x: scroll;
				white-space: nowrap;
				width: 100%;
				padding: 0 20rpx;
				box-sizing: border-box;
			}
			
			.view_mask {
				position: absolute;
				right: 0;
				top: 0;
				width: 92rpx;
				height: 88rpx;
				background-color: rgba(22, 26, 38, 1);
				// z-index: 1;
			}
		
			.view_li_con {
				width: calc(100% - 92rpx);
				height: 68rpx;
				overflow: hidden;
		
				&.personnel_li_con {
					overflow-x: scroll;
					line-height: 68rpx;
				}
			}
		
			.view_li_cot {
				display: flex;
				justify-content: flex-start;
				align-items: center;
				width: auto;
				position: absolute;
		
				// position: relative;
				&.personnel_li_cot {
					overflow-x: scroll;
					position: relative;
				}
			}
		
			.shenglue_li::before {
				content: "...";
				position: absolute;
				bottom: 0;
				right: 63rpx;
				padding-left: 10px;
				z-index: 2;
			}
		
			.pal_col {
				color: rgba(255, 255, 255, 0.20);
				font-size: 32rpx;
				padding-left: 20rpx;
				box-sizing: border-box;
				width: 100%;
				height: 100%;
				display: flex;
				align-items: center;
			}
		
			.view_li {
				display: flex;
				justify-content: flex-start;
				padding: 20rpx 30rpx 20rpx 20rpx;
				background-color: rgba(28, 34, 50, 1);
				line-height: 28rpx;
				margin-left: 10rpx;
				white-space: nowrap;
		
				.view_icon {
					margin-left: 10rpx;
				}
		
				&.disabled_text {
					color: rgba(255, 255, 255, 0.50);
				}
			}
		
			.form_sel_icon {
				top: 0;
				width: 92rpx;
				z-index: 1;
				// text-align: right;
				margin-right: 10rpx;
			}
		}

		.addPool-button {
			width: 200rpx;
			height: 70rpx;
			line-height: 70rpx;
			text-align: center;
			border-radius: 10rpx;
			color: #fff;
			background: linear-gradient(180deg, #ff3535, #ff613d);
		}

		.addPool-textarea {
			height: 290rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 10rpx;
			margin: 20rpx auto;
			padding: 20rpx;
		}

		.copyInvite {
			.copyInvite-title {
				margin: 20rpx 0px;
			}

			.validityPeriod {
				border-radius: 10rpx;
				border: 1rpx solid rgba(255, 255, 255, .2);
				height: 90rpx;
				// line-height: 90rpx;
				margin-top: 20rpx;
				padding-left: 20rpx;
			}

			.copyInvite-lianUrl {
				display: flex;
				align-items: center;

				.copyInvite-lianUrl-url {
					border: 1rpx solid rgba(255, 255, 255, .2);
					border-radius: 6rpx;
					padding: 10px 20px;
					line-height: 30rpx;
				}

			}
			.copuUrl {
				border-radius: 6rpx;
				background: linear-gradient(180deg, #ff3535, #ff613d);
				padding: 10px 10px;
				font-size: 24rpx;
				text-align: center;
				margin-top:22rpx;
				font-size:32rpx;
				margin-bottom: 25rpx;
			}
		}

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

	#CreateCustomer {
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
					font-size: 32rpx;
					margin-top:10rpx;
				}
				/deep/.Comments-item-txt-content img{
					width: 400rpx;
				}
				.Comments-item-txt-title {
					color: rgba(255, 255, 255, .6);
					font-size: 28rpx;
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
				margin-top:15rpx;
			}

			.Follow-up-item-top {
				display: flex;
				align-items: center;

				.Follow-up-item-top-content {
					font-size: 28rpx;
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
				font-size:32rpx;
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
				border-top: 1rpx solid rgba(255, 255, 255, .1);

				.Information-title {
					color: rgba(255, 255, 255, .6);
					font-size:28rpx;
				}

				.Information-bottom {
					display: flex;
					align-items: center;
					margin-top: 10rpx;
					font-size:32rpx;
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
					font-size: 20rpx;
					border-top-right-radius: 8rpx;
					border-bottom-left-radius: 15rpx;
					background: linear-gradient(180deg, #EFA902, #F4BE3F);
				}

				.Avatar-information-title {
					margin-top: 35rpx;
					font-size: 36rpx;
				}

				.Avatar-information-content {
					color: rgba(255, 255, 255, .6);
					font-size: 28rpx;
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

	}
	.DirectSales{
		position: absolute;
		right:0rpx;
		top:0rpx;
		padding:0rpx 10rpx;
		text-align: center;
		font-size:24rpx;
		line-height: 38rpx;
		border-top-right-radius: 15rpx;
		border-bottom-left-radius: 15rpx;
		background: linear-gradient(180deg,#EFA902,#F4BE3F);
	}
</style>