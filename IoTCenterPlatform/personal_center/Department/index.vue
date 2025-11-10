<template>
	<view id="Department">
		<top title="Department" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#161A26"
			rightWidth="157rpx" :isleftBack="true">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the name"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="Department-tree" v-if="childList.length>0">
			<view class="Department-tree-item">
				<view class="Department-tree-item-left">
					<view class="iconfont t-icon-bumenguanli-gongsitubiao">

					</view>
					<view>
						<!-- Fujian Huade Group -->
						{{parentList.deptName}}
					</view>
				</view>
				<view class="backIndex">
					<view class="iconfont t-icon-bianji" @click="handEdit(parentList,0)">

					</view>
					<view class="borderXian" v-if="parentList.parentId">

					</view>
					<view class="iconfont t-icon-bumenguanli-fanhuishangji" v-if="parentList.parentId"
						@click="handBack()">

					</view>
				</view>
			</view>
			<view class="Department-tree-item borderMargin" style="padding-left:20rpx;"
				v-for="(item,index1) in childList" :key="index1" @click="handDtails(item)">
				<view class="Department-tree-item-left">
					<view class="iconfont t-icon-bumenguanli-bumentubiao">

					</view>
					<view>
						<!-- Fujian Huade Group -->
						{{item.deptName}}
					</view>
				</view>
				<view class="Department-tree-item-right">
					<view class="iconfont t-icon-bianji" @click.stop="handEdit(item,100)">

					</view>
					<view class="borderXian">

					</view>
					<view class="iconfont t-icon-a-youjiantoubai">

					</view>
				</view>
			</view>
		</view>
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
					OrgId: ''
				},
				key: '',
				parentList: [],
				childList: [],
				listLoding: [],
				lomoreLoding: '',
				loHide: false
			}
		},
		onLoad() {
			this.querydata.OrgId = this.$store.getters.orgId
			this.list(this.querydata); //部门列表
		},
		methods: {
			handEdit(item, choose) {
				uni.navigateTo({
					url: './CreateDepartment?deptId=' + item.deptId + '&showHide=' + choose
				})
			},
			handBack() {
				// this.list();
				var childParentId = ""
				this.childList.map((item) => {
					childParentId = item.parentId
				})
				console.log(childParentId, 'childParentId')
				var arr = []
				this.listLoding.map((row) => {
					if (this.parentList.parentId == row.deptId) {
						this.parentList = row;
						console.log(row)
					}

					if (childParentId == row.deptId) {
						//console.log(row);
						arr.push(row)
					}
					this.childList = arr;
				})
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
						if (res.data.length > 0) {
							this.childList = [];
							//console.log(res,'部门列表')
							this.listLoding = res.data;
							var deptData = res.data;
							deptData.map((row) => {
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
						} else {
							this.childList = []
							this.parentList = {}
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

					.t-icon-a-youjiantoubai {
						width: 25rpx;
						height: 25rpx;
						margin-left: auto;
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
					color: #fff;
					font-size: 32rpx;

					.t-icon-bumenguanli-gongsitubiao,
					.t-icon-bumenguanli-bumentubiao {
						width: 35rpx;
						height: 35rpx;
						margin-right: 14rpx;
					}
				}

				.t-icon-bianji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
				}

				.t-icon-bumenguanli-fanhuishangji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
				}
			}

			.borderMargin {
				margin: 50rpx 25rpx;
			}
		}
	}
</style>