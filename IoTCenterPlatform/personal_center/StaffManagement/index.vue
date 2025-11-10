<template>
	<view id="StaffManagement">
		<top leftIcon="icon-fanhui"   :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Staff" class="CRM-header" :rightText="!hide?'Cancel':''" @clickRight="cancelHide">
			<template v-slot:top_right v-if="hide">
				<view class="right_top right1" @click.stop="handSet" v-if="hide">
					<custom-icons iconsName="icon-guanli" iconsSize="36rpx" ></custom-icons>
				</view>
				<!-- <view class="right_top" v-if="!hide" @click="handSet" style="font-size:30rpx;">
					Cancel
				</view> -->
				<view class="right_top" v-if="hide" @click.stop="handAddAlert">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the username or phone number"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :selectListParam="selectListParam" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
	<!-- 	<view class="Staff-SelectAll" v-if="!hide">
				<view class="left">
				<image v-if="hide1"@click="handHide" class="select" src="../../static/images/select.png"></image>
				<image v-else  class="select" @click="handHide" src="../../static/images/weixuan.png"></image>
						<view class="SelectAllText">
							Select all
						</view>
				</view>
				<view class="right">
					1 selected
				</view>
		</view> -->
		<view class="StaffManagement-item" v-for="(item,index) in MenberList" :key="index" @click="handDetails(item)">
			<view v-if="!hide">
				<image v-if="xuanId==item.Id" @click.stop="handStop(item.Id)" class="select" src="../../static/images/select.png"></image>
				<image v-else @click.stop="handStop(item.Id)" class="select" src="../../static/images/weixuan.png"></image>
			</view>
			
			<view class="NameCutting" v-if="!item.Avatar&&item.Avatar.length<=0">
				{{item.RealName.length>2?item.RealName.slice(-2):item.RealName}}
			</view>
			<image v-else  :src="item.Avatar" class="StaffManagement-item-img"></image>
			<view class="StaffManagement-item-txt">
				<view class="title" :class="[item.status==1||item.status=='1'?'active':'on']">
					{{item.RealName}}
				</view>
				<view class="content">
					{{item.dept_name}}
				</view>
			</view>
			
				<view class="iconfont t-icon-dianhua" @click.stop="TelPhone(item.Mobile)">
					
				</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<view class="BottomButton" v-if="!hide">
		<!-- 	<view class="BottomButton-item">
				<view class="iconfont t-icon-qiyong">
					
				</view>
				 <view>
					 Enable
				 </view>
			</view>
			
			<view class="BottomButton-item">
				<view class="iconfont t-icon-jinyong">
					
				</view>
				 <view>
					 Disable
				 </view>
			</view> -->
			
			<view class="BottomButton-item" >
				<view class="iconfont t-icon-shanchu">
					
				</view>
				 <view @click="handDelete">
					 Delete
				 </view>
			</view>
		</view>
		<!--添加弹窗-->
		<view class="AddPopUps" v-if="alertHide">
			<view class="AddPopUps-item" >
				<view class="AddPopUps-title">
					<view class="title">
						Create Employee
					</view>
					<view class="iconfont t-icon-guanbidanchuang" @click="handClose">
						
					</view>
				</view>
				<view v-if="!alertHideChilred">
					<view class="link">
						Invite through link
					</view>
					<view class="lianUrl">
						{{tipsValue}}
					</view>
					<view class="CopyLink" @click="copy(tipsValueCopy)">
						Copy link
					</view>
					<view class="link">
						Link validity period: 
					</view>
					<view class="Invitation">
						Invitation link expires in <text class="num">7</text>days
					</view>
					<view class="through" @click="handXianChilred">
						Invite through account
					</view>
				</view>
				<view v-if="alertHideChilred">
					<view class="link">
						Invite through account
					</view>
					<view class="lianUrlInput">
						<input v-if="keyInputHide" v-model="keyInput" class="lianUrl-input"  @input="handSearch" type="text" placeholder="Please enter user phone or email"/>
						<view class="xuanSelect" v-else>
							<text>{{keyInput}}</text>
							<text @click="handSearchGuan" class="iconfont icon-crmtianjiaguanbi"></text>
						</view>
					</view>
					<view class="itemXuan" v-if="SearchList.length>0&&!SearchHide">
						<view  v-for="(item,index) in SearchList" :key="index" class="itemXuan-item" @click="handChen(item)" >
							<img class="logoImg" :src="'https://52.28.35.196/'+item.Avatar" alt="">
							<text class="itemXuan-name">{{item.Name}}</text>
						</view>
					</view>
					<view  class="selectImg" v-if="!keyInputHide">
						<view>
							<img class="selectImg-logoImg" :src="'https://52.28.35.196/'+arrSearch.Avatar" alt="">
						</view>
						<view class="selectImg-name">
							{{keyInput}}
						</view>
					</view>
					<view v-if="SearchHide" style="margin-top:20rpx;">
						<view @click="handSelectValue" class="lianUrlContent" >
							<view v-if="range.length==0" style="color:rgba(255, 255, 255, .4);font-size:28rpx;">
								Please select department
							</view>
							<view v-else v-for="(item,index) in range" class="lianUrlItem">
								{{item.name}}
							</view>
						</view>
						<input class="inputPosition" v-model="postName" type="text" placeholder="Please enter the position">
					</view>
					
					<view class="button-yao">
						<view class="button" @click="handQuxiao">
							cancellation
						</view>
						<view class="button yaoActive" @click="handYaoQqing">
							invite
						</view>
					</view>
				</view>
			</view>
			
		</view>
		<view class="move" v-if="alertHide"></view>
		<msg-prompt ref="promptMsg"  @confirm="confirmUnbind"></msg-prompt>
		<!-- <msg-prompt ref="promptMsg"></msg-prompt> -->
	</view>
</template>



<script>
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
	yuanAarngemnt,
	deleteArngemnt,
	createCode,
	searchDeptSearch,
	memberJionOrg
	} from "@/api/personalCenter";
	import serverUrl from '@/common/constVar.js'
	export default {
		data(){
			return{
				keyInputHide:true,
				SearchHide:false,
				selectListParam: [{
					name: '状态',
					params: 'Status',
					pal: '请选择状态',
					value: null,
					localdata: [{
							text: "正常",
							value: 0
						},
						{
							text: "停用",
							value: 1
						}
					]
				}],
				timeQuery: {
					name: 'date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				deltList:[],
				searchHide:false,
				SearchList:[],
			keyInput:'',
				value:'',
				 range: [
				        
				        ],
				alertHideChilred:false,
				alertHide:false,
				xuanId:'',
				arr:[],
				hide1:true,
				selected:0,
				hide:true,
				MenberList:[],
				status: 'loading',
			      querydata: {
			        pageNum: 1,
			        pageSize: 10,
			        deptIdWithChildren: true
			      },
				  arrSearch:[],
				  postName:'',
				  tipsValue:'',
				  tipsValueCopy:''
			}
		},
		mounted() {
			this.list();//员工管理
		},
		methods:{
			handSearchGuan(){
				this.SearchHide=false;
				this.keyInputHide=true;
				this.keyInput=""
			},
			cancelHide(){
				if(!this.hide){
					this.hide=true
				}
			},
			TelPhone(phone){
				      const res = uni.getSystemInfoSync(); //获取当前的手机机型
				                if (res.platform == 'ios') {
				                    uni.makePhoneCall({
				                        phoneNumber: phone
				                    })
				                } else {
				                    uni.makePhoneCall({
				                        phoneNumber: phone,
				                    })
				                }
			},
			selectFinsh(query) {
				//console.log(query,'111')
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.list();
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			copy(context) {//context被复制的内容
			const that = this
			//复制链接
			console.log("复制链接", context);
			uni.setClipboardData({
				data: context,
				success: () => {
					// uni.showToast({
					// 	title: '已自动复制网址，请在手机浏览器里粘贴该网址',
					// 	duration: 2000,
					// 	icon: 'none'
					// });
					// uni.hideToast()
				},
				fail: (err) => {
					uni.showToast({
						title: 'copy failed',
						duration: 2000,
						icon: 'none'
					});
				}
			});
			  // const that = this
			  // navigator.clipboard.writeText(context)
			  //   .then(() => {
			  //     uni.showToast({
			  //     	title:'Replicating Success！',
					// icon:'none'
			  //     })
			  //   })
			  //   .catch(err => {
				 //  uni.showToast({
				 //  	title:'copy failed！',
				 //  	icon:'none'
				 //  })
			  //   })
			},
		
			handYaoQqing(){
				//console.log(this.Key,'this.Key')
				//console.log(this.arrSearch.Id,this.range[0].id,this.postName);
				//return
				
				memberJionOrg({
					uid: this.arrSearch.Id,
					depId: this.range[0].id,
					postName: this.postName
				}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Invitation successful',
							icon:'none'
						})
						this.alertHideChilred=false;
						this.alertHide=false;
						this.list();
					}
					
				}).catch((err)=>{
					this.setMsgTop(err)
				})
			},
			handChen(ite){
				this.keyInput=ite.Name;
				this.arrSearch=ite;
				this.SearchList=[]
				this.SearchHide=true;
				this.keyInputHide=false;
				//console.log(ite.Name,this.keyInput,'ite')
				this.$forceUpdate()
			},
			
			handSearch(){
				searchDeptSearch({
					key:this.keyInput
				}).then((res)=>{
					//console.log(res);
					this.SearchList=[]
					 if(this.keyInput){
					 	this.SearchList=res.data;
					 }else{
					 	this.arrSearch=[]
						this.SearchList=[]
					 }
				})
			},
			//返回部门数据
			selectDept(data){
				
				this.range=data;
				//console.log(this.range,'this.range');
			},
			handQuxiao(){
				this.alertHideChilred=false;
			},
			handSelectValue(){
				uni.navigateTo({
					url:'/pages_Inventory/employee_select?type=dept'
				})
			},
			handXianChilred(){
				this.alertHideChilred=true;
			},
			handAddAlert(){
				this.alertHide=true;
				createCode().then(res => {
				
				  // console.log("生成的邀请码", res.data);
				  // //获取当前url
				  // let baseUrl = window.location.href;
				  // //当前路由
				  // let baseR = this.$route.path;
				  // //分割url
				  // let yumAry = baseUrl.split(baseR);
				  // //得到域名
				  // let yuming = yumAry[0];
				  let yuming = serverUrl.getServerUrl()+'/jump.html';
				  if (res.code == 0) {
					  this.tipsValueCopy=yuming + "?code=" + encodeURIComponent(res.data)+'&isMobile=true';
					   var lineurl=yuming + "?code=" + encodeURIComponent(res.data)+'&isMobile=true';
					this.tipsValue=lineurl.length>=30?lineurl.slice(0,33)+'...':lineurl
					//console.log(this.tipsValueCopy,'tipsValueCopy')
				  }
				});
			},
			handClose(){
				this.alertHide=false;
			},
			  handleAdd() {
			      this.open = true;
			      this.title = "添加员工";
			      createCode().then(res => {
			       
			        //获取当前url
			        let baseUrl = window.location.href;
			        //当前路由
			        let baseR = this.$route.path;
			        //分割url
			        let yumAry = baseUrl.split(baseR);
			        //得到域名
			        let yuming = yumAry[0];
			        if (res.code == 0) {
			          this.tipsValue = yuming + "?code=" + res.data;
			        }
			      });
			      // });
			    },
			confirmUnbind(){
				deleteArngemnt({id:this.xuanId}).then((res)=>{
					uni.showToast({
						title:'Removal successful！',
						icon:'none'
					})
					this.list();//员工管理
					this.hide=false;
				}).catch((err)=>{
					this.setMsgTop(err)
				})
			},
			handDelete(){
				//console.log(this.xuanId,'this.xuanId');
				if(this.xuanId==''){
					uni.showToast({
						title:'Please select an employee！',
						icon:'none'
					})
				}else{
					this.$refs.promptMsg.noticeOpen(
						'Are you sure to remove user ID'+this.xuanId+'Employees of？'
						)
				}
			},
			handHide(){
				this.hide1=!this.hide1;
			},
			handStop(id){
				this.xuanId=id;
				 // this.selected=!this.selected;
				 // if(this.selected){
					//  this.xuanId=id;
				 // }else{
					//  this.xuanId='';
				 // }
				//console.log(this.xuanId,id,'this.selected')
			},
			handSet(){
				if(this.MenberList.length<=0){
					uni.showToast({
						title:'There are currently no employees！',
						icon:'none'
					})
					return
				}else{
					this.hide=!this.hide;
					console.log(this.hide,'this.hidethis.hidethis.hide');
					this.$forceUpdate()
					this.xuanId=''
				}
			},
			handDetails(item){
				uni.navigateTo({
					url:'./ManagementDetails?id='+item.Id//JSON.stringify(item)
				})
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.MenberList = []
					this.list()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.MenberList = []
					this.list()
				}
			},
			list(){
				yuanAarngemnt(this.querydata).then((res)=>{
					console.log(res,'员工列表');
					if (this.querydata.pageNum == 1) {
						this.MenberList = []
					}
					this.MenberList = [...this.MenberList, ...res.data.List];
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}).catch(err => {
					this.status = 'noMore';
					this.setMsgTop(err)
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	#StaffManagement{
		.move{
			position: fixed;
			height:100%;
			width:100%;
			background: #000;
			opacity: .7;
			top:0px;
			left:0px;
			z-index:1;
		}
		.AddPopUps{
			position: absolute;
			margin:0rpx 5%;
			background: #161A26;
			 z-index:2;
			color:#fff;
			width: 90%;
			border-radius: 8rpx;
			top:250rpx;
			.AddPopUps-item{
				padding:30rpx;
				.num{
					color:rgba(255, 53, 53, 1);
					padding:0rpx 10rpx;
				}
			}
			.through{
				height:88rpx;
				line-height:88rpx;
				padding:0rpx 20rpx;
				border-radius: 12rpx;
				margin-top:15rpx;
				color:#fff;
				text-align: center;
				color:rgba(255, 255, 255, .3);
				background: rgba(28, 34, 50, 1);
				margin-top:50rpx;
				font-size:32rpx;
			}
			.Invitation{
				padding:15rpx 0rpx;
			}
			.CopyLink{
				height:88rpx;
				line-height:88rpx;
				padding:0rpx 20rpx;
				border-radius: 12rpx;
				margin-top:15rpx;
				background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
				color:#fff;
				text-align: center;
				font-size:32rpx;
			}
			.link{
				color:rgba(255, 255, 255, .4);
				margin-top:30rpx;
				font-size:32rpx;
			}
			.addPool-selected{
				margin-top:20rpx;
				/deep/.uni-select__input-text{
					color:#fff!important;
				}
				/deep/.uni-select{
					height: 88rpx!important;
					border:2rpx solid rgba(255, 255, 255, .2);
				}
				/deep/.uni-select__input-placeholder{
					font-size:30rpx!important;
					color:rgba(255, 255, 255, .2)!important;
				}
			}
			.button-yao{
				display:flex;
				justify-content: center;
				margin-top:30rpx;
				font-size:32rpx;
				.yaoActive{
					background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
				}
				.button{
					width: 200rpx;
					height:88rpx;
					line-height: 88rpx;
					border:2rpx solid rgba(255, 255, 255, .1);
					border-radius: 8rpx;
					text-align: center;
					margin:0rpx 20rpx;
				}
			}
			.inputPosition{
				border:2rpx solid rgba(255, 255, 255, .2);
				height:88rpx;
				line-height:88rpx;
				padding:0rpx 20rpx;
				border-radius: 12rpx;
				margin-top:15rpx;
				font-size:28rpx;
			}
			// .addPool-selected{
			// 	height:88rpx;
			// 	line-height:88rpx;
			// 	border:2rpx solid rgba(255, 255, 255, .2);
			// 	margin-top:10rpx;
			// 	border-radius: 10rpx;
			// }
			.selectImg{
				display: flex;
				flex-direction: column;
				align-items: center;
				.selectImg-name{
					margin:10rpx 0rpx;
					font-size:32rpx;
				}
				.selectImg-logoImg{
					width: 100rpx;
					height: 100rpx;
					border-radius: 50%;
					margin-top:46rpx;
				}
			}
			.itemXuan{
				background: #1C2232;
				padding:20rpx 10rpx;
			
				.itemXuan-item{
					display: flex;
					align-items: center;
					height:90rpx;
					// border: 2rpx solid rgba(255, 255, 255, .3);
					// margin-top: 20rpx;
					padding: 0rpx 20rpx;
					border-radius: 8rpx;
				}
				.itemXuan-name{
					margin-left:20rpx;
				}
				.logoImg{
					width: 60rpx;
					height:60rpx;
					border-radius: 50%;
				}
			}
			.lianUrlInput{
				height:88rpx;
				line-height:88rpx;
				padding:0rpx 10rpx;
				border:2rpx solid rgba(255, 255, 255, .2);
				border-radius: 4rpx;
				margin-top:15rpx;
				display: flex;
				align-items: center;
				border-radius:10rpx;
				.xuanSelect{
					display: flex;
					background: #1C2232;
					width: 224rpx;
					height:68rpx;
					padding:0rpx 20rpx;
					line-height: 68rpx;
					text-align: center;
					align-items: center;
					justify-content: space-between;
					font-size:28rpx;
					.icon-crmtianjiaguanbi{
						margin-left:auto;
						font-size:20rpx;
					}
				}
				.lianUrl-input{
					width: 100%;
				}
			}
			.lianUrlContent{
				height:88rpx;
				line-height:88rpx;
				word-break: break-all;
				padding:0rpx 20rpx;
				border:1rpx solid rgba(255, 255, 255, .2);
				border-radius: 4rpx;
				margin-top:15rpx;
			}
			.lianUrl{
				// line-height:88rpx;
				word-break: break-all;
				padding:0rpx 20rpx;
				border:1rpx solid rgba(255, 255, 255, .2);
				border-radius: 4rpx;
				margin-top:15rpx;
				height: 88rpx;
				line-height: 88rpx;
				.lianUrlItem{
					display: flex;
					align-items: center;
					margin-right:15rpx;
					font-size:28rpx;
				}
			}
			.AddPopUps-title{
				position: relative;
				// margin:20rpx 30rpx;
				align-items: center;
				
				.title{
					text-align: center;
					font-size:34rpx;
				}
				.t-icon-guanbidanchuang{
					position: absolute;
					margin-left:auto;
					width:40rpx;
					height:40rpx;
					right:0rpx;
					top:5rpx;
				}
			}
		
		}
	
		.BottomButton{
			position: absolute;
			bottom: 0rpx;
			display: flex;
			background:#1C2232;
			height:98rpx;
			color:#fff;
			align-items: center;
			justify-content: space-around;
			width:100%;
			font-size:32rpx;
			.BottomButton-item{
				display: flex;
				align-items: center;
				.iconfont{
					width: 28rpx;
					height: 28rpx;
					margin-right:10rpx;
				}
			}
			
		}
		.Staff-SelectAll{
			color:#fff;
			display: flex;
			align-items: center;
			margin:20rpx 15rpx;
			.icon-guanli{
				
			}
			.left{
				display: flex;
				
				.SelectAllText{
					margin-left:20rpx;
				}
				.select{
					width: 40rpx;
					height: 40rpx;
					border-radius: 50%;
					margin:0rpx 20rpx;
				}
			
			}
			.right{
				margin-left:auto;
				color:rgba(255, 255, 255, .4);
			}
			
		}
		.StaffManagement-item{
			padding:0rpx 20rpx;
			height:148rpx;
			background: #1C2232;
			border-radius: 8rpx;
			display: flex;
			align-items: center;
			margin:0rpx 20rpx;
			margin-top:20rpx;
			.icon-dianhua{
				margin-left:auto;
			}
			.StaffManagement-item-txt{
				color:#fff;
				margin-left:30rpx;
				
				.title{
					display: flex;
					font-size:32rpx;
				}
				.active{
					text-decoration:line-through;
				}
				.content{
					font-size:27rpx;
					color:rgba(255, 255, 255, .4);
				}
			}
			.select{
				width: 40rpx;
				height: 40rpx;
				border-radius: 50%;
				margin:0rpx 20rpx;
			}
			.weixuanzhongda{
				width: 35rpx;
				height: 35rpx;
				border-radius: 50%;
				margin:0rpx 20rpx;
				border:1rpx solid rgba(255, 255, 255, .4);
				background: red;
			}
			.t-icon-dianhua{
				width: 50rpx;
				height:50rpx;
				margin-left:auto;
			}
			.NameCutting{
				width: 100rpx;
				height:100rpx;
				border-radius: 50%;
				line-height: 100rpx;
				text-align: center;
				color:#fff;
				font-size:28rpx;
				background: #161A26;
			}
			.StaffManagement-item-img{
				width: 100rpx;
				height:100rpx;
				border-radius: 50%;
			}
		}
	}
</style>