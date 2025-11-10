<template>
	<view id="CreateCustomer">
		<top :isRightSlot="true" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff" title="详情"
			:rightText="details.BindOrgId==0?'邀请':'取消邀请'" class="CRM-header">
			<template v-slot:top_right>
				<view v-if="details.BindOrgId==0" class="right_top" @click.stop="handInvite()"
					style="font-size:15px;color:#999999;">
					邀请
				</view>
				<view v-else class="right_top" @click.stop="handCancelInvite()"
					style="font-size:15px;text-align: right;color:#999999;">
					取消邀请
				</view>
			</template>

		</top>
		<view class="PoolDetails-htmlContent">
			<view class="CreateCustomer-header">
				<view class="Avatar-information">
					<view class="iconfont t-icon-kehumorentouxiang1">

					</view>
					<view class="Avatar-information-title">
						{{details.CustomerName}}
					</view>
					<view class="Avatar-information-content">
						<view style="font-size:28rpx;">
							NO.{{details.CustomerNumber}}
						</view>
						<view>
							<view class="Agent" v-if="details.CustomerType==0">
								代理
							</view>
							<view class="DirectSales" v-if="details.CustomerType==1">
								直销
							</view>
						</view>
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
		</view>
		<view v-if="select==0">
			<view class="Information-item">

				<view class="Information" style="margin-top:0rpx;border-top:none;">
					<view class="Information-title">
						负责人
					</view>
					<view class="Information-bottom">
						<text>{{details.LeaderName}}</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;">
					<view class="Information-title">
						归属部门
					</view>
					<view class="Information-bottom">
						<text>Sales center</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;">
					<view class="Information-title">
						协作人
					</view>
					<view class="Information-bottom" v-for="(item,index) in details.HelperUsers" :key="index"
						style="margin-top:20rpx;">
						<img :src="item.Avatar" alt="" class="aviter">
						<text>{{item.RealName}}</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;">
					<view class="Information-title">
						客户来源
					</view>
					<view class="Information-bottom">
						<text v-if="details.FromType=='weixin'">WeChat Clues</text>
						<text v-if="details.FromType=='form'">Process Form</text>
						<text v-if="details.FromType=='other'">other</text>
					</view>
				</view>

				<view class="Information" style="margin-top:0rpx;border-bottom: none;" v-if="details.CompanyUrl">
					<view class="Information-title">
						公司网址
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
				<input class="searchInput" type="text" placeholder-style="color:#C1C1C1;" placeholder="添加跟进动态">
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
					<text class="tit_text">跟进方式:</text>
					<text>{{item.FollowWay}}</text>
				</view>
			</view>
			<!-- <view style="height:50px;"></view> -->
		</view>

		<view v-if="select==2">
			<view class="Follow-up-input" @click="handComment">
				<text class="iconfont icon-tianxiegenjinhepinglun"></text>
				<input class="searchInput" type="text" placeholder="添加评价" placeholder-style="color:#C1C1C1;">
			</view>
			<view class="Comments-item" v-for="(item,index) in payList" :key="index" @click="handPayListDetails(item)"
				style="margin-bottom: 20rpx;">
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
								<text>回复</text>
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
									<text>回复</text>
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<!-- <view style="height:50px;"></view> -->
		</view>
		<view class="BottomPositioning_zhanwei"></view>
		<view class="BottomPositioning">
			<view class="BottomPositioning-item" @click="handReturn('Return')">
				<text class="iconfont icon-tuihui"></text>
				<text>退回</text>
			</view>
			<view class="BottomPositioning-item" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>编辑</text>
			</view>
			<view class="BottomPositioning-item" style="border:none;" @click.stop="handDelete()">
				<text class="iconfont icon-shanchu"></text>
				<text>删除</text>
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
						评论
					</view>
					<view class="iconfont icon-guanbidanchuang" @click="handClose">

					</view>
				</view>
				<view>
					<textarea v-model="content" placeholder="请输入评论" class="addPool-textarea" :rows="4" />
					<view class="addPool-button" @click="handCommentSubmit">
						确认
					</view>
				</view>
			</view>
			<!--回退弹框-->
			<view v-if="alertName=='Return'">
				<view class="AddPopUps-title" style="margin-top:25rpx;">
					<view class="title">
						回退
					</view>
					<view class="iconfont icon-guanbidanchuang" @click="handClose">

					</view>
				</view>
				<view>
					<textarea v-model="reason" placeholder="请输入退货原因" class="addPool-textarea" :rows="4" />
					<view class="addPool-button" @click="handConfirm">
						确认
					</view>
				</view>
			</view>
			<!--删除为空-->
			<view v-if="alertName==''">
				<view class="AddPopUps-item">
					<view class="AddPopUps-title">
						<view class="title">
							创建员工
						</view>
						<view class="iconfont icon-guanbidanchuang" @click="handClose">

						</view>
					</view>
					<view v-if="tipsValue==''">
						<view class="link">
							代理商的工厂
						</view>
						<view class="lianUrlInput">
							<uni-data-select v-model="inviteForm.factoryId" :localdata="AgentFactory" type="line"
								placeholder="请选择客户类型" class="addPool-selected"></uni-data-select>
						</view>
						<view class="link">
							选择代理所属的区域
						</view>
						<view class="lianUrlInput">
							<view class="view_input" @click="choiceWZ">
								<view class="pal_col" v-if="!wzValue||wzValue.length==0">
									请选择地区
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
										iconsColor="#fff"></custom-icons>
								</view>
							</view>
						</view>
						<view class="GenerateLink" @click="createLink()">生成链接</view>
					</view>
					<view v-else class="copyInvite">
						<view class="copyInvite-title">
							通过链接邀请
						</view>
						<view class="copyInvite-lianUrl">
							<view class="copyInvite-lianUrl-url">
								{{tipsValue}}
							</view>
						</view>
						<view class="copuUrl" @click="copy(tipsValue)">
							复制链接
						</view>
						<view class="validityPeriod">
							链接有效期： 邀请链接在<text style="color:#2371FF;"> 7 </text>天后过期
						</view>
					</view>

				</view>
			</view>
		</view>
		<uni-popup ref="clearAlarmPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">取消邀请</view>
					<view class="conten_icon_con">
						<view style="color:#999;margin-bottom: 20rpx;">
							是否要取消代理权限？
						</view>
						<view class="conten_icon t-icon-kaibeifen" v-if="clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(false)"></view>
						<view class="conten_icon t-icon-guan" v-if="!clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(true)"></view>
					</view>
					<button class="submit_button" @click.stop="getWarnMsg()">
						确认
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="#333"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="addressPopup" type="bottom" :z-index="1000">
			<hy-address-multiple ref="addressSelect" :address="wzItems" @confirm="addressConfirm"></hy-address-multiple>
		</uni-popup>
	</view>
</template>

<script>
	import {
		uploadPhotoImg,
	} from '@/api/user.js'
	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif

	let ser = ''
	// #ifdef H5
	ser = serverUrl1.getServerUrl()
	// #endif
	// #ifndef H5
	ser = serverUrl.getServerUrl()
	// #endif
	import {
		getToken
	} from '@/common/auth.js'
	import {
		flowDataRecord, //快速跟进详情
		DataPoolsDetails, //客户详情（私海）
		customerDelete, //删除
		AuthorizationData,
		customerInvite, //邀请
		customerReturn, //退回
		commentList, //评论列表
		commentAdd, //评论添加
		cancelInvitation, //取消邀请
		UploadImages //上传图片
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
				addressPopup: false,
				pinglunEdit: true,
				pinglunForm: {
					targetId: 0,
					targetType: null,
					parentCommentId: 0,
					parentCommentUserId: 0,
					content: '' //评论内容
				},
				usersList: [{
						id: 1,
						name: '张三'
					},
					{
						id: 2,
						name: '李四'
					},
					// 更多用户
				],
				placeholder: '请输入评论！',
				html: '<p>你好<span data-w-e-type="mention" data-w-e-is-void data-w-e-is-inline data-value="A张三" data-info="%7B%22id%22%3A%22a%22%7D">@A张三</span></p>',
				title: 'Hello',
				fileServer: {
					url: ser + "/AuthService/File/Upload?withDomain=true",
					name: "wangeditor-uploaded-image",
					header: {
						Authorization: 'eyJVc2VySWQiOjEyNjc5Mzg5ODU0MTEyNSwiVGltZSI6MTcxMjI4MTcwNjgyOSwiTW9kZSI6MCwiRXh0IjoiIiwiU2lnbiI6IjQxMzUzOTIwRjdCMjZBQjAzOTQ2OEEzQkFCNTZGNzM5MDY2OTgzMUEifQ=='
					},
					formData: {
						//userid: "123456"
					},
					fileNum: 1,
					sizeType: ['original', 'compressed'],
					sourceType: ['album', 'camera'],
					inImgData: true //返回信息是否插入图片标签data中
				},
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
						title: '基本信息'
					},
					{
						title: '快速跟进'
					},
					{
						title: '评论'
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
				//yuanGong: new Map(),
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
				setTimeout(async () => {
					this.wzItems = await this.$store.dispatch("data/areaTree");
				}, 2000)
				//	this.wzItems = await this.$store.dispatch("data/areaTree");
				partUnit.forEach((item) => {
					this.ExecutionMode.set(item.value, item.label);
				})

			}
		},
		methods: {
			addressConfirm(resArr, resNameArr) {
				this.wzValue = JSON.parse(JSON.stringify(resNameArr))
				this.wzCode = JSON.parse(JSON.stringify(resArr))
				this.inviteForm.regions = this.wzCode.join(',')
				this.$refs.addressPopup.close()
			},
			choiceWZ() {
				//选择位置
				//打开选择器
				this.$refs.addressPopup.open()
				if (this.isresetAddress) {
					this.$nextTick(async () => {
						let arr = await this.$store.dispatch("data/areaTree")
						// console.log(arr,'arr');
						this.wzItems = JSON.parse(JSON.stringify(arr))
						this.$refs.addressSelect.newAddress = JSON.parse(JSON.stringify(arr))
						this.isresetAddress = false
					})
				}
			},
			editorInput(value) {
				//富文本输入赋值
				// console.log(value,'富文本输入值')
				this.pinglunForm.content = value
			},
			updataInput(value) {
				// console.log(this.html,value,'this.html');
			},
			setPosition(e) {
				let x = e.touches[0].pageX;
				let y = e.touches[0].pageY;
				this.site = {
					x,
					y
				}
				// console.log(e);
			},
			gethtml() {
				uni.showModal({
					content: this.html
				})
			},
			sethtml() {
				this.html = '<b style="color:#33ff14">内容已经被修改</b>'
			},
			upFile(callback) {
				uni.chooseImage({
					count: this.fileServer.fileNum, // 2.2.0 版本起插入图片时支持多张（修改图片链接时仅限一张）
					sizeType: this.fileServer.sizeType,
					sourceType: this.fileServer.sourceType,
					success: async res => {
						if (!this.fileServer.url) {
							uni.showModal({
								content: "设置文件服务器URL",
								showCancel: false
							})
							return false;
						}
						uni.showLoading({
							title: '上传中'
						});

						for (let i = 0; i < res.tempFiles.length; i++) {

							try {
								let paths = res.tempFilePaths[i];

								let rsp = await uploadPhotoImg(paths)
								let imgobj = {
									src: rsp, // JSON.parse(rs.data).Data,//'http://localhost:21825/' + JSON.parse(rs.data).Data,
									data: rsp.data
								}
								callback(imgobj)

							} catch (e) {
								uni.showModal({
									content: '网络错误，请检查你的网络',
									showCancel: false
								});
							}
						}
						uni.hideLoading();
					}
				})
			},



			noticeColse() {
				this.$refs.clearAlarmPopup.close()
			},
			setDeviceFilter(val) {
				//是否过滤设备
				this.clearForm.isFilterDevice = val
			},
			getWarnMsg() {
				cancelInvitation({
					id: this.EditId,
					cancelAgent: this.clearForm.isFilterDevice
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '取消成功!',
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
			//评论列表
			CommentData(id) {
				commentList({
					targetType: "客户",
					targetId: id,
					pageNum: 1,
					pageSize: 10,
				}).then((data) => {
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
						title: '请输入评论内容！',
						icon: 'none'
					})
					return
				}
				if (this.parentCommentId && this.parentCommentUserId) {
					var data = {
						content: this.content, //this.html,
						targetType: "客户",
						targetId: this.EditId,
						parentCommentUserId: this.parentCommentUserId,
						parentCommentId: this.parentCommentId
					}
				} else {
					var data = {
						content: this.content,
						targetType: "客户",
						targetId: this.EditId
					}
				}
				commentAdd(data).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '评论添加成功!',
							icon: 'none'
						})
						this.alertHide = false;
						this.content=''
						this.CommentData(this.EditId);
					}
				})
			},
			handPayListDetails(row) {
				this.html = ''
				this.alertHide = true
				this.alertName = 'comment'
				this.parentCommentId = row.Id
				this.parentCommentUserId = row.UserId
			},
			handComment() {
				this.alertHide = true
				this.alertName = 'comment'
				this.html = ''
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
					//console.log(res,'res')
					this.FastTracking = res.data.List;
					res.data.List.map((item, index) => {
						item.FollowWay = this.ExecutionMode.get(item.FollowWay)
					})
				})
			},
			handAddFloow() {
				//console.log(1111)
				uni.navigateTo({
					url: './AddFollow-up?id=' + this.EditId
				})
			},
			handConfirm() {
				if (this.reason == '') {
					uni.showToast({
						title: '请输入退货原因',
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
							title: '退回成功！',
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
					success: () => {},
					fail: (err) => {
						uni.showToast({
							title: '复制失败',
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
						title: '请选择代理工厂',
						icon: 'none'
					})
					return
				}

				customerInvite(this.inviteForm).then(res => {
					let yuming = ser + '/#/index';
					// let yuming='http://52.28.35.196/#/index'
					if (res.code == 0) {
						uni.showToast({
							title: '链接已生成！',
							icon: 'none'
						})
						this.tipsValue = yuming + "?yaoqingId=" + res.data + '&isMobile=true';
						console.log(this.tipsValue, 'tipsValue')
					}
				}).catch((err) => {
					this.setMsgTop(err)
				});;
			},
			handCancelInvite() {
				this.$refs.clearAlarmPopup.open()
				this.EditId = this.details.Id
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
							title: '删除成功！',
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
					"你确定要删除名为 " + this.details.CustomerName + "的客户吗?"
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
					console.log(res, '111')
					this.details = res.data;
				})
			},
		}
	}
</script>
<style>
	page {
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	.custom-mention-class {
		background: red;
		width: 200px;
		height: 200px;
	}

	::v-deep.view_input .pal_col {
		color: #C1C1C1 !important;
	}

	::v-deep.Comments-item-txt-content img {
		width: 100rpx;
	}

	.Avatar-information-content {
		display: flex;
		justify-content: center;
	}

	.Agent {
		position: relative !important;
		margin-left: 10rpx;
	}

	.DirectSales {
		position: relative !important;
		padding: 0rpx 10rpx;
		text-align: center;
		font-size: 20rpx;
		border-radius: 8rpx;
		background: rgba(237, 253, 254, 1);
		color: rgba(71, 226, 241, 1);
		margin-left: 10rpx;
	}

	.PoolDetails-htmlContent {
		padding: 0rpx 3%;
		background: #ffffff;
	}

	.view_input {
		width: 100%;
		height: 88rpx;
		line-height: 88rpx;
		background-color: #f8f8f8;
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

		.tips_input {
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
			background-color: #f8f8f8;
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
			background-color: #ffffff;
			line-height: 28rpx;
			margin-left: 10rpx;
			white-space: nowrap;
			color: #333;

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
		background: #F8F8F8;
		color: #333;
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
				align-items: center;
				border: 1rpx solid #EAEAEA;
				border-radius: 6rpx;
				padding: 20rpx 20rpx;
				line-height: 30rpx;
				word-break: break-all;
			}

		}

		.copuUrl {
			border-radius: 6rpx;
			background: #2371FF;
			padding: 10px 10px;
			font-size: 24rpx;
			text-align: center;
			margin-top: 22rpx;
			color: #fff;
			font-size: 32rpx;
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
		background: #ffffff;
		z-index: 333;
		color: #333;
		width: 90%;
		margin: 0px 5%;
		border-radius: 8rpx;
		top: 250rpx;

		.addPool-selected {
			::v-deep.uni-select__input-text {
				color: #333 !important;
			}

			::v-deep.uni-select {
				height: 88rpx !important;
				border: 1rpx solid rgba(255, 255, 255, .2);
			}

			::v-deep.uni-select__input-placeholder {
				font-size: 30rpx !important;
				color: #C1C1C1 !important;
			}
		}

		.link {
			color: #999;
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
			background: #2371FF;
			color: #fff;
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
			background: #f8f8f8;
			color: #333;

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

			.icon-guanbidanchuang {
				position: absolute;
				margin-left: auto;
				width: 50rpx;
				height: 50rpx;
				right: 0rpx;
				top: 5rpx;
				color: #333;
			}
		}

		.AddPopUps-item {
			padding: 30rpx;

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
			background: #ffffff;
			border-radius: 14rpx;
			color: #333;
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
					color: #999999;
					margin-top: 10rpx;
					width: 100%;

					.reply {
						margin-left: auto;
					}
				}

				.Comments-item-txt-content {
					font-size: 29rpx;
					margin-top: 10rpx;
				}

				.Comments-item-txt-title {
					color: #999999;
					font-size: 28rpx;
				}
			}
		}

		.Follow-up-item {
			margin: 0rpx 3%;
			padding: 20rpx 30rpx;
			margin-bottom: 20rpx;
			background: #ffffff;
			border-radius: 10rpx;
			color: #333;

			.Follow-up-item-bottom {
				padding-top: 10rpx;
				font-size: 28rpx;
				color: #999999;

				.tit_text {
					margin-right: 10rpx;
				}
			}

			.Follow-up-item-center {
				font-size: 30rpx;
				margin-top: 10rpx;
			}

			.Follow-up-item-top {
				display: flex;
				align-items: center;

				.Follow-up-item-top-content {
					.time {
						color: #999999;
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
			//border: 1rpx solid rgba(255, 255, 255, .1);
			margin: 20rpx 20rpx;
			border-radius: 8rpx;
			background: #fff;

			.icon-tianxiegenjinhepinglun {
				color: #C1C1C1;
				margin-left: 30rpx;
			}

			.searchInput {
				font-size: 32rpx;
				margin-left: 14rpx;
				color: #C1C1C1;
			}

			::-webkit-input-placeholder {}
		}
		.BottomPositioning_zhanwei{
			width: 100%;
			height: 98rpx;
		}
		.BottomPositioning {
			display: flex;
			position: fixed;
			height: 98rpx;
			background: #ffffff;
			bottom: 0rpx;
			color: #999999;
			left: 0rpx;
			width: 100%;
			align-items: center;

			.icon-shanchu {}

			.BottomPositioning-item {
				width: 33%;
				text-align: center;
				border-right: 1rpx solid #EAEAEA;

				.iconfont {
					margin-right: 9rpx;
				}
			}
		}

		.Information-item {
			margin: 20rpx 3%;
			background: #fff;
			color: #fff;
			padding: 0rpx 20rpx;
			border-radius: 10rpx;

			.Information {
				display: flex;
				padding: 30rpx 0rpx;
				flex-direction: column;
				margin-top: 20rpx;
				border-top: 1rpx solid #EAEAEA;

				.Information-title {
					color: #999999;
				}

				.Information-bottom {
					display: flex;
					align-items: center;
					margin-top: 10rpx;
					color: #333;
					font-size: 32rpx;
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
			color: #333;
			margin-top: 30rpx;
			font-weight: bold;

			.navigation-nav-border {
				width: 30rpx;
				height: 5rpx;
				border-radius: 14rpx;
				background: #2371FF;
				margin: 0 auto;
				margin-top: 10rpx;
			}

			.navigation-nav-item {
				width: 33%;
				text-align: center;
			}

			.active {}

			.on {
				color: #999999;
				font-weight: normal;
			}
		}

		.CreateCustomer-header {
			.Avatar-information {
				position: relative;
				display: flex;
				flex-direction: column;
				color: #fff;
				background: #ffffff;
				text-align: center;
				padding: 30rpx 0rpx;
				margin: 0rpx 3%;
				padding-bottom: 20rpx;

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
					//width:100rpx;
					text-align: center;
					color: rgba(35, 113, 255, 1);
					font-size: 21rpx;
					border-radius: 8rpx;
					padding: 0rpx 10rpx;
					background: rgba(233, 241, 255, 1);
				}

				.Avatar-information-title {
					margin-top: 35rpx;
					font-size: 32rpx;
					color: #333333;
				}

				.Avatar-information-content {
					color: #999999;
					font-size: 24rpx;
					margin-top: 5rpx;
				}

				.t-icon-kehumorentouxiang1 {
					width: 140rpx;
					height: 140rpx;
					line-height: 140rpx;
					margin: 0 auto;
					color: #1C2232;
					border-radius: 10rpx;
				}
			}
		}

	}
</style>

<style scoped>
	.content {
		/* height: 600rpx; */
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
	}

	.test {
		position: absolute;
		top: 0px;
		left: 0px;
	}

	.logo {
		height: 200rpx;
		width: 200rpx;
		margin-top: 200rpx;
		margin-left: auto;
		margin-right: auto;
		margin-bottom: 50rpx;
	}

	.text-area {
		display: flex;
		justify-content: center;
	}

	.title {
		font-size: 36rpx;
		color: #8f8f94;
	}

	::v-deep.ql-editor {
		padding: 0rpx 20rpx;
	}
</style>