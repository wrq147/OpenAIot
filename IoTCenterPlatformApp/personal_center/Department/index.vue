<template>
	<view id="Department">
		<top :isRightSlot="true" title="部门管理" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="157rpx" :isleftBack="true">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入部门名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt"  :selectListParam="selectListParam"  :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="Department-tree" v-if="childList.length>0">
			<view class="Department-tree-item">
				<view class="Department-tree-item-left">
					<view class="iconfont t-icon-qiye">

					</view>
					<view style="font-size:32rpx;">
						<!-- Fujian Huade Group -->
						{{parentList.deptName}}
					</view>
				</view>
				<view class="backIndex">
					<view class="iconfont icon-bianji" @click.stop="showActionSheet(parentList,0)"></view>
					<view class="borderXian">

					</view>
					<view v-if="parentList.parentId" class="iconfont icon-bumenguanli-fanhuishangji"
						@click="handBack()">

					</view>
				</view>
			</view>
			<view class="Department-tree-item borderMargin" style="padding-left:20rpx;"
				v-for="(item,index1) in childList" :key="index1" @click="handDtails(item)">
				<view class="Department-tree-item-left">
					<view class="iconfont icon-bumenguanli-bumentubiao">

					</view>
					<view style="font-size:30rpx;" :class="[item.status==1?'addDelete':'']" class="Department-tree-item-left-xitong">
						<!-- Fujian Huade Group -->
						{{item.deptName}}
						
					</view>
				</view>
				<view class="Department-tree-item-right">
					<view class="iconfont icon-bianji" @click.stop="showActionSheet(item, 100)"></view>
					<view class="borderXian">

					</view>
					<view class="iconfont icon-a-youjiantoubai">

					</view>
				</view>
			</view>
		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" @click.stop="handEdit">
					编辑
				</view>
				<view class="popup_li" @click.stop="handlePrincipal">
					管理
				</view>
				<view class="popup_li cancel" @click="closePopup">
					取消
				</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import {
		DeptamentList, //部门列表
	} from "@/api/personalCenter";

	export default {
		data() {
			return {
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
				parentId: '',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					OrgId:''
				},
				// timeQuery: {
				// 	name: '日期',
				// 	params: ['beginTime', 'endTime'],
				// 	value: [],
				// },
				parentList: [],
				childList: [],
				listLoding: [],
				lomoreLoding: '',
				loHide: false,
				DepartmentList: new Map(), //部门列表
				departItem: '',
				departChoose: ''
			}
		},
		onShow() {
			this.querydata.OrgId=this.$store.getters.orgId
			this.list(this.querydata); //部门列表
		},
		methods: {
			showActionSheet(item, choose) {
				this.$refs.bottomPop.open()
				this.departItem = item;
				this.departChoose = choose;
			},
			closePopup() {
				this.$refs.bottomPop.close()
			},
			handEdit(item, choose) {
				uni.navigateTo({
					url: './CreateDepartment?deptId=' + this.departItem.deptId + '&showHide=' + this.departChoose
				})
				this.$refs.bottomPop.close()
			},
			handlePrincipal(item) {
				uni.navigateTo({
					url: './principalAdd?deptId=' + this.departItem.deptId
				})
				this.$refs.bottomPop.close()
			},
			handBack() {
				// this.list();
				var childParentId = ""
				this.childList.map((item) => {
					childParentId = item.parentId
				})
				//console.log(childParentId, 'childParentId')
				var arr = []
				this.listLoding.map((row) => {
					if (this.parentList.parentId == row.deptId) {
						this.parentList = row;
						//console.log(row)
					}

					if (childParentId == row.deptId) {
						//console.log(row);
						arr.push(row)
					}
					this.childList = arr;
				})
                this.list(this.querydata)
			},
			//三级
			handDtails(item) {
				//console.log(item,'this.parentId');
				var arr1 = []
				this.listLoding.map((row) => {
					if (row.parentId == item.deptId) {
						//console.log(row.parentId,item.deptId,'有子元素')
						//this.childList.push(row);
						arr1.push(row)
						this.parentId = item.parentId
						this.parentList = item;
						this.childList = arr1;
					}

				})

			},
			list() {
				DeptamentList(this.querydata).then((res) => {
					if (res.code == 0) {
						if(res.data.length>0){
						this.childList = [];
						//	console.log(res,'部门列表')
						this.listLoding = res.data;
						var deptData = res.data;
						deptData.map((row) => {
							this.DepartmentList.set(row.parentId, row);
							if (row.parentId == 0) {
								this.parentList = row;
								this.parentId = row.parentId
								//console.log(this.parentList,'this.parentList')
							}
							if (row.parentId == this.parentList.deptId) {
								//console.log(row,'二级')
								this.childList.push(row)
							}

						})
						}else{
							this.childList=[]
							this.parentList={}
						}
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
			handAdd() {
				uni.navigateTo({
					url: './CreateDepartment'
				})
			},
		}
	}
</script>
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	#Department {
		
		.Department-tree {

			.Department-tree-item {
				display: flex;
				align-items: center;
				margin: 30rpx 25rpx;

				.Department-tree-item-right {
					margin-left: auto;
					display: flex;
					align-items: center;

					.borderXian {
						width: 1rpx;
						height: 40rpx;
						background: rgba(255, 255, 255, .2);
						margin: 0rpx 25rpx;
					}

					.icon-a-youjiantoubai {
						margin-top: 18rpx;
						margin-left: auto;
						color: #C1C1C1;
						font-size: 24rpx;
					}
				}

				.backIndex {
					display: flex;
					margin-left: auto;

					.borderXian {
						width: 1rpx;
						height: 40rpx;
						background: rgba(255, 255, 255, .2);
						margin: 0rpx 25rpx;
					}
				}

				.Department-tree-item-left {
					display: flex;
					align-items: center;
					color: #333;
					font-weight: bold;
					.Department-tree-item-left-xitong{
						display: flex;
						align-items: center;
					
					}
					.addDelete{
						text-decoration: line-through;
						color:#999999;
					}
					.t-icon-qiye,
					.icon-bumenguanli-bumentubiao {
						width: 35rpx;
						height: 35rpx;
						margin-right: 14rpx;
						font-weight: normal;
					}
				}

				.icon-bianji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
					color: #999999;
				}
				.icon-fuzeren{
					idth: 25rpx;
					height: 25rpx;
					margin-left: 35rpx;
					color: #999999;
				}
				.icon-bumenguanli-fanhuishangji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
					color: #999999;
				}
			}

			.borderMargin {
				margin: 50rpx 25rpx;
			}
		}
	}
	.popup_list {
		background-color: #ffffff;
		border-radius: 20rpx 20rpx 0 0;
	
		.popup_li {
			height: 120rpx;
			line-height: 120rpx;
			text-align: center;
			color: #333333;
			font-size: 36rpx;
	
			&.borradio {
				border-radius: 20rpx 20rpx 0 0;
			}
	
			&.cancel {
				color: #999999;
			}
		}
	}
</style>