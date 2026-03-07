<template>
	<view id="Department">
		<top :isRightSlot="hide" title="角色管理" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			  :isleftBack="true" :rightText="!hide?'取消':''" @clickRight="cancelHide">
			<template v-slot:top_right v-if="hide">
				<view class="right_top_con" style="display: flex;align-items: center;">
					<view class="right_top right1" @click.stop="handSet" v-if="hide">
						<custom-icons iconsName="icon-guanli" iconsSize="36rpx" ></custom-icons>
					</view>
					<!-- <view class="right_top" v-if="!hide" @click="handSet" style="font-size:30rpx;">
						Cancel
					</view> -->
					<view class="right_top" v-if="hide" @click.stop="handAddAlert">
						<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
					</view>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入角色名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :selectListParam="selectListParam" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
			<view class="Department-tree">
	
				<view class="Department-tree-item borderMargin" style="padding-left:20rpx;" v-for="(item,index1) in listLoding" :key="index1" >
					
					<view v-if="!hide" style="display: flex;">
						<image v-if="xuanId==item.roleId" @click.stop="handStop(item)" class="select" :src="getSerVerUrl()+'/appimg/images/select.png'"></image>
						<image v-else @click.stop="handStop(item)" class="select" :src="getSerVerUrl()+'/appimg/images/weixuan.png'"></image>
					</view>
					
					<view class="Department-tree-item-left">
						<view  :class="[item.status==1?'addDelete':'']" class="Department-tree-item-left-xitong">
							<!-- Fujian Huade Group -->
								{{item.roleName}}
						</view>
						<view class="SystemTags" v-if="item.IsSystem==1">
							系统
						</view>
						<view v-else class="SystemTags activeQiye">
							企业
						</view>
					</view>
					<view class="Department-tree-item-right">
						<view class="iconfont icon-bianji" @click.stop="handEdit(item)">
						</view>
					</view>
				</view>
			</view>
			<view class="PoolDetails-button" v-if="!hide">
				<view class="button" @click="handDisable('0')">
					<text class="iconfont icon-lingqu"></text>
					<text>启用</text>
				</view>
				<view class="button" @click="handDisable('1')">
					<text class="iconfont icon-jinyong"></text>
					<text>停用</text>
				</view>
				<view class="button" @click="handDelete()">
					<text class="iconfont icon-shanchu"></text>
					<text>删除</text>
				</view>
			</view>
			<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
			<msg-prompt ref="promptReceive" @confirm="confireCloseDrawer"></msg-prompt>
	</view>
</template>

<script>
	import {
	RoleList,//角色列表
	RoleEdit,//编辑
	RoleDelete//角色删除
	} from "@/api/personalCenter";
	import {
		checkPermi
	} from '@/common/permission.js';
	export default{
		data(){
			return{
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
				key:'',
				xuanId:'',
				hide:true,
				querydata: {
					pageNum: 1,
					pageSize: 10,
				},
				timeQuery: {
					name: '日期',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				parentList:[],
				childList:[],
				listLoding:[],
				IsSystem:'',
				RolesName:'',
				disabStatus:''
			}
		},
		onLoad() {
			this.list();//部门列表
		},
		methods:{
		cancelHide(){
			if(!this.hide){
				this.hide=true
			}
		},
			confireCloseDrawer(){
				RoleDelete(this.xuanId).then((res)=>{
					if(res.code==0){
						this.xuanId=''
						this.hide=true;
						this.list();//部门列表
					}
					
				})
			},
			handDelete(){
				if(this.xuanId==''){
					uni.showToast({
						title:'请选择一个角色！',
						icon:'none'
					})
					return
				}
				if(this.IsSystem=='1'){//系统角色不可删除
					uni.showToast({
						title:'无法删除系统角色',
						icon:'none'
					})
					return
				}else{
					//console.log('删除')
					this.$refs.promptReceive.noticeOpen(
						"确定启/禁用"+this.RolesName+"?"
					)
				}
			},
		
			isCheckPermi(val) {
				return checkPermi(val)
			},
			confirmUnbind(){
				RoleEdit({
					roleId:this.xuanId,
					status:this.disabStatus
				}).then((res)=>{
					if(res.code==0){
						this.xuanId=''
						this.hide=true;
						this.list();//部门列表
					}
					
				})
			},
			handDisable(status){
				if(this.xuanId==''){
					uni.showToast({
						title:'请选择一个角色！',
						icon:'none'
					})
					return
				}else{
					this.$refs.promptMsg.noticeOpen(
						"确认启/禁用"+this.RolesName+"?"
					)
					this.disabStatus=status
				}
				
			},
			handStop(ite){
				this.xuanId=ite.roleId;
				this.IsSystem=ite.IsSystem
				this.RolesName=ite.roleName
			},
			handEdit(ite){
				//编辑
				uni.navigateTo({
					url:'./addRoles?deptId='+ite.roleId
				})
			},
			handAddAlert(){
				uni.navigateTo({
					url:'./addRoles'
				})
			},
			handSet(){
				if(this.listLoding.length<=0){
					uni.showToast({
						title:'当前没有可用的角色！',
						icon:'none'
					})
					return
				}else{
					this.hide=!this.hide;
					this.xuanId=''
				}
				
			},
			
		
		
			list(){
				RoleList(this.querydata).then((res)=>{
					if(res.code==0){
						console.log(res,'角色列表')
						this.listLoding=res.data.List;
					}
				})
			},
			selectFinsh(query) {
				//console.log(query,'111')
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.list();
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				}
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			handAdd(){
				uni.navigateTo({
					url:'./CreateDepartment'
				})
			},
		}
	}
</script>
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	
	#Department{
		.PoolDetails-button{
			position: fixed;
			bottom: 0rpx;
			left:0rpx;
			width: 100%;
			display: flex;
			color:#999999;
			padding:25rpx 0rpx;
			align-items: center;
			background: #ffffff;
			.button{
				width:33%;
				text-align: center;
				margin:0 auto;
				.iconfont{
					margin-right:9rpx;
				}
			}
		}
		.Department-tree{
			
			.Department-tree-item{
				display: flex;
				align-items: center;
				margin:30rpx 25rpx;
				.select{
					width: 40rpx;
					height: 40rpx;
					border-radius: 50%;
					margin:0rpx 20rpx;
				}
				.Department-tree-item-right{
					margin-left:auto;
					display: flex;
					align-items: center;
					.borderXian{
						width: 1rpx;
						height:40rpx;
						background: rgba(255, 255, 255, .2);
						margin:0rpx 25rpx;
					}
					.t-icon-a-youjiantoubai{
						width: 25rpx;
						height:25rpx;
						margin-left:auto;
					}
				}
				.backIndex{
					display: flex;
					margin-left:auto;
					.borderXian{
						width: 1rpx;
						height:40rpx;
						background: rgba(255, 255, 255, .2);
						margin:0rpx 25rpx;
					}
				}
				.Department-tree-item-left{
					display: flex;
					align-items: center;
					color:#333;
					font-weight: bold;
					
					.SystemTags{
						color:#FF3535;
						background: rgba(255, 53, 53, 0.10);
						font-size:20rpx;
						padding:0rpx 8rpx;
						border-radius: 4rpx;
						line-height: 36rpx;
						margin-left:20rpx;
					}
					.activeQiye{
						background: rgba(233, 241, 255, 1);
						color:rgba(35, 113, 255, 1);
					}
					.Department-tree-item-left-xitong{
						display: flex;
						align-items: center;
					
					}
					.addDelete{
						text-decoration: line-through;
						color:#999999;
					}
					.t-icon-bumenguanli-gongsitubiao,.t-icon-bumenguanli-bumentubiao{
						width: 35rpx;
						height:35rpx;
						margin-right:14rpx;
					}
				}
				.icon-bianji{
					width: 25rpx;
					height:25rpx;
					margin-left:auto;
					color:#999;
				}
				.t-icon-bumenguanli-fanhuishangji{
					width: 25rpx;
					height:25rpx;
					margin-left:auto;
				}
			}
			.borderMargin{
				margin:20rpx 25rpx;
				background: #FFFFFF;
				padding:35rpx 30rpx;
				border-radius: 10rpx;
			}
		}
	}
</style>