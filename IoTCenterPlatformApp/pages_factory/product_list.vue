<template>
	<view style="background-color: rgba(245, 248, 249, 1);min-height: 100vh;" @click="clickOtherPlace">
		<top :isCenterSlot="true" :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff" rightIcon="icon-tianjia" @clickRight="openProductAdd">
			<template v-slot:top_center>
				<view class="pro_type_selet">
					<view class="type_selet_text" @click.stop="openTypeList">
						<text class="text">{{activeProductType?activeProductType:'全部'}}</text>
						<custom-icons v-if="!showTypeList" iconsName="icon-xialasanjiao" iconsSize="14rpx"
							iconsColor="rgba(51,51,51, 1)"></custom-icons>
						<custom-icons v-if="showTypeList" iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
							iconsColor="rgba(51,51,51, 1)"></custom-icons>
					</view>
					<view class="pro_type_ul" v-if="showTypeList">
						<view class="pro_type_li" @click.stop="setTypeActive()">全部</view>
						<view class="pro_type_li" v-for="(it,inx) in productTypeList" @click.stop="setTypeActive(it)">
							<view class="type_li_text">{{it.Name}}</view>
							<view class="type_li_handle">
								<view @click.stop="delhandleType(it,inx)">
									<custom-icons iconsName="icon-shanchu" iconsSize="28rpx"
										iconsColor="#999999"></custom-icons>
								</view>
								<view @click.stop="jumpToAddType(it.Id)">
									<custom-icons iconsName="icon-bianji" iconsSize="28rpx"
										iconsColor="#999999"></custom-icons>
								</view>

							</view>
						</view>
						<view class="pro_type_add" @click.stop="jumpToAddType()">
							<view class="add_icon">
								<custom-icons iconsName="icon-tianjia" iconsSize="16rpx"
									iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
							</view>
							<view class="add_text">添加分组</view>
						</view>
					</view>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入产品关键字" :fixed="true"
			backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<!-- <select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :selectListParam="selectListParam"
			:querydata="querydata" @selectFinsh="selectFinsh"></select-compt> -->
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="100rpx" :zIndex="99"
			backgroundColor="#ffffff" v-if="activePropsList && activePropsList.length > 0">
			<template v-slot:allslot>
				<view class="prop_con">
					<view class="prop_li" @click="setFilterProp('')" :key="'typeprop'"
						:class="{ 'active_prop': activeProp == '' }">全部</view>
					<view class="prop_li" @click="setFilterProp(it)" v-for="it in activePropsList"
						:class="{ 'active_prop': it == activeProp }">
						{{it}}
					</view>
				</view>
			</template>
		</uni-nav-bar>

		<view class="product_con">
			<view class="product_total">
				总计（{{total}}）
			</view>
			<view class="product_ul">
				<view class="product_li" v-for="(row,inx) in factoryProductList" @click="toProductDetail(row,inx)">
					<view class="li_cont">
						<view class="cont_title">
							<view class="title_label" :class="{'normal':row.ProductLabel&&row.ProductLabel=='U'}">
								{{row.ProductLabel&&row.ProductLabel=='U'?'半成品':'成品'}}
							</view>
							<view class="title_name">
								<text>{{row.ProductName}}</text>
								<text v-if="row.TypeName" class="line">|</text>
								<text v-if="row.TypeName">{{row.TypeName}}</text>
							</view>
						</view>
						<view class="cont_number" v-if="row.SkuNumber">产品编码：{{row.SkuNumber}}</view>
						<view class="cont_ul">
							<view class="cont_number" v-if="row.ProductFrom">生产来源：{{row.ProductFrom}}</view>
							<view class="cont_number">成本单价：{{row.Price}}</view>
							<view class="cont_number" v-if="row.Total">总量：{{row.Total}}<text v-if="row.MinUnit"
									style="margin-left: 5rpx;">/{{row.MinUnit}}</text></view>
							<view class="cont_number">销售单价：{{row.SalesPrice}}</view>
						</view>
						<view class="type_edit">
							<view class="type_text">
								<text>{{row.TypeName}}</text>
								<text class="line" v-if="row.Prop">|</text>
								<text v-if="row.Prop">{{row.Prop}}</text>
							</view>
							<view class="edit_del">
								<view class="del_btn btn_li" @click="delProductHandle(row,inx)">
									<custom-icons iconsName="icon-shanchu" iconsSize="24rpx"
										iconsColor="#999999"></custom-icons>
									<text class="text">删除</text>
								</view>
								<view class="edit_btn btn_li" @click="openProductAdd(row.Id)">
									<custom-icons iconsName="icon-bianji" iconsSize="24rpx"
										iconsColor="#999999"></custom-icons>
									<text class="text">编辑</text>
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<product_filter @finishSelect="finishSelect" @filterListChange="filterListChange" :filterKey="filterKey"
			ref="product_filter" @choiceFiled="choiceFiled" :productFiledList="productFiledList"></product_filter>
		<filed_select ref="filed_select" @finishFiledChoice="finishFiledChoice"></filed_select>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete" @msgClose="msgClose"></msg-prompt>
	</view>
</template>

<script>
	import {
		factoryProductListPost,
		factoryProductTypeListGet,
		factoryProductTypeInfo,
		orgFormFields,
		factoryProductRemove,
		factoryProductTypeRemove
	} from '@/api/product.js'
	import filed_select from '@/pages_factory/cmp/filed_select.vue'
	import product_filter from '@/pages_factory/cmp/product_filter.vue'
	export default {
		components: {
			product_filter,
			filed_select
		},
		data() {
			return {
				topTitle: '选择产品',
				select: [], //选择的产品
				selectId: [], //选择产品的所有
				isSelect: false, //是否是选择
				isMulSelect: false, //是否是多选
				querydata: {
					pageNum: 1,
					pageSize: 30,
				}, //过滤参数
				factoryProductList: [], //代理商产品列表
				selectListParam: [],
				timeQuery: { //下拉筛选
					name: '创建时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				total: 0, //产品总数
				status: 'loading',
				showTypeList: false, //是否显示产品类型列表
				productTypeList: [], //产品分组列表
				activeFilter: [], //筛选框的筛选
				activeTypeCondition: [], //当前分组的过滤筛选
				productFiledList: [],
				activeProductType: '', //选择的产品类型
				filterKey: 1,
				activeDelId: '', //要被删除的
				activeDelIndx: -1, //要删除的序号
				activePropsList: [], //对应分组的属性
				activeProp: '', //选择过滤的分组属性
				activeDel:'',//删除的是产品还是分组判断
			};
		},
		onLoad(options) {
			this.loadProductTypeList()
			this.loadList()
			this.loadOrgFormFields()
		},
		onReachBottom() {
			//上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = 'loading';
				this.loadfactoryProductList();
			}
		},
		methods: {
			loadTypeList() {
				this.loadProductTypeList()
			},
			setFilterProp(val) {
				//设置进行过滤的类型属性
				if (val == this.querydata.prop || this.querydata.prop == undefined && val == '') {
					return
				}
				this.activeProp = val
				if (this.activeProp) {
					this.querydata.prop = val
				} else {
					delete this.querydata.prop
				}
				this.loadList()
			},
			jumpToAddType(id) {
				//跳转去添加产品分组
				if (id) {
					uni.navigateTo({
						url: '/pages_factory/product_type_add?id=' + id
					})
				} else {
					uni.navigateTo({
						url: '/pages_factory/product_type_add'
					})
				}

			},
			delhandleType(item,inx){//删除产品分组
				try {
					this.activeDel='group'
					this.activeDelId = item.Id
					this.activeDelIndx = inx
					this.$refs.promptMsg.noticeOpen(
						'你确定要删除产品分组 ' + item.Name + ' 吗? (此操作是不可逆的)?'
					)
				} catch (e) {
					console.log("报错了",e);
				}
			},
			finishSelect(items) { //完成过滤筛选
				// this.activeFilter = items ? JSON.parse(JSON.stringify(items)) : [],
				if (items && items.length > 0) {
					this.querydata.items = [...items]
				} else {
					delete this.querydata.items
				}
				this.querydata.pageNum = 1
				this.status = 'loading';
				this.loadfactoryProductList()
			},
			filterListChange(list) { //过滤筛选数据发生变化
				if (list) {
					this.activeFilter = JSON.parse(JSON.stringify(list))
				}
			},
			finishFiledChoice(list) { //过滤筛选选择字段后
				if (list) {
					list.map(row => {
						let findObj = this.activeFilter.find(ro => ro.field == row.mapid)
						if (findObj) {} else {
							let obj = {
								field: row.mapid,
								compare: '', //比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
								val: '', //字符串
								val_num: null, //数字
								val_arr: [] //字符串数组
							}
							this.activeFilter.push(obj)
						}
					})
					this.filterKey++
					this.$refs.product_filter.setNewFilterData(this.activeFilter)
				}
			},
			choiceFiled(filterProductFiledList) { //打开选择字段的弹窗
				this.$refs.filed_select.openPopup(filterProductFiledList, this.activeFilter)
			},
			async loadOrgFormFields() { //获取产品的所有字段
				try {
					let res = await orgFormFields({
						field: '产品',
						ext: true
					})
					// console.log("产品所有字段",res);
					let data = res.data
					this.productFiledList = JSON.parse(JSON.stringify(res.data))

				} catch (error) {
					console.log(error, 'error');
				}
			},
			setTypeActive(it) {
				//设置选择的产品类型
				this.showTypeList = false
				this.activeProp = ''
				delete this.querydata.prop
				if (it) {
					factoryProductTypeInfo({
						id: it.Id
					}).then(res => {
						// console.log("产品分组信息", res);
						this.activeProductType = res.data.Name
						if (res.data.ConditionJson) {
							this.activeTypeCondition = JSON.parse(res.data.ConditionJson)
							this.activeFilter = JSON.parse(res.data.ConditionJson)
						}
						this.activePropsList = res.data.PropList ? res.data.PropList.split(',') : [] //属性
						this.finishSelect(this.activeFilter)
					})
				} else {
					this.activeProductType = ''
					this.activeFilter = []
					this.activePropsList = []
					this.finishSelect(this.activeFilter)
				}
			},
			loadProductTypeList() {
				//加载产品分组列表
				factoryProductTypeListGet().then(res => {
					// console.log("产品分组", res);
					this.productTypeList = res.data
				})
			},
			clickOtherPlace() {
				//点击其他地方
				this.showTypeList = false
			},
			openTypeList() { //打开产品分组筛选列表
				// console.log("切换", this.showTypeList);
				this.showTypeList = !this.showTypeList
				this.$forceUpdate()
			},
			delProductHandle(item, inx) {
				try {
					this.activeDel='product'
					this.activeDelId = item.Id
					this.activeDelIndx = inx
					this.$refs.promptMsg.noticeOpen(
						'你确定要删除产品 ' + item.ProductName + '吗? (此操作是不可逆的)?'
					)
				} catch (e) {}
			},
			async confirmDelete() {
				if(this.activeDel=='product'){
					try {
						this.$refs.promptMsg.loadingOpen()
						await factoryProductRemove({
							id: this.activeDelId
						});
						this.$refs.promptMsg.open('操作成功', 1500)
						setTimeout(() => {
							if (this.activeDelIndx > -1) {
								this.factoryProductList.splice(this.activeDelIndx, 1)
								this.total--
								
							}
							this.activeDelId = ''
							this.activeDelIndx = -1
							this.activeDelId=''
							this.$refs.promptMsg.loadingColse()
						}, 1500)
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
						this.$refs.promptMsg.loadingColse()
					}
				}else if(this.activeDel=='group'){
					try {
						this.$refs.promptMsg.loadingOpen()
						await factoryProductTypeRemove({
							id: this.activeDelId
						});
						this.$refs.promptMsg.open('操作成功', 1500)
						setTimeout(() => {
							if (this.activeDelIndx > -1) {
								this.productTypeList.splice(this.activeDelIndx, 1)
							}
							this.activeDelId = ''
							this.activeDelIndx = -1
							this.activeDelId=''
							this.$refs.promptMsg.loadingColse()
						}, 1500)
					} catch (e) {
						//TODO handle the exception
						this.setMsgTop(e)
						this.$refs.promptMsg.loadingColse()
					}
				}
				
			},
			msgClose() {

			},
			openProductAdd(id) {
				// 去添加产品
				if (id) {
					uni.navigateTo({
						url: '/pages_factory/product_add?id=' + id
					})
				} else {
					uni.navigateTo({
						url: '/pages_factory/product_add'
					})
				}

			},
			toProductDetail(row, inxO) {

			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.loadfactoryProductList();
			},
			loadfactoryProductList() {
				factoryProductListPost(this.querydata).then(res => {
					// console.log("加载代理商产品", res);
					if (this.querydata.pageNum == 1) {
						this.factoryProductList = []
					}
					this.factoryProductList = [...this.factoryProductList, ...res.data.List]
					this.total = res.data.Total
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				})
			},
			searching(val) {
				//搜索关键字函数
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.factoryProductList = []
					this.loadfactoryProductList()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.factoryProductList = []
					this.loadfactoryProductList()
				}
			},
			openSelect() {
				//打开筛选弹窗
				// this.$refs.selectCompt.openSelect()
				this.$refs.product_filter.openPopup(this.activeFilter)
			},
			selectFinsh(query) {
				//筛选完毕结果，筛选参数返回结果函数
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.factoryProductList = []
				this.loadfactoryProductList()
			}
		}
	}
</script>

<style lang="scss" scoped>
	.prop_con {
		display: flex;
		flex-wrap: wrap;
		padding: 20rpx;
		height: 100%;
		box-sizing: border-box;
		margin-right: -10rpx;

		.prop_li {
			padding: 18rpx 30rpx;
			background: rgba(248, 248, 248, 1);
			font-size: 24rpx;
			color: rgba(102, 102, 102, 1);
			border-radius: 40rpx;
			margin-right: 10rpx;

			&.active_prop {
				background: rgba(35, 113, 255, 1);
				color: #ffffff;
			}
		}
	}

	.pro_type_selet {
		position: relative;
		z-index: 999;

		.type_selet_text {
			display: flex;
			justify-content: center;
			align-items: center;
		}

		.pro_type_ul {
			position: absolute;
			top: 88rpx;
			// left: -158rpx;
			width: 100vw;
			z-index: 9990;
			background: rgba(255, 255, 255, 1);
			box-sizing: border-box;
			padding: 0 30rpx;
			border-radius: 0 0 20rpx 20rpx;

			.pro_type_li {
				box-sizing: border-box;
				width: 100%;
				height: 100rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;

				border-bottom: 1rpx solid rgba(234, 234, 234, 1);

				.type_li_handle {
					width: 96rpx;
					font-weight: normal;
					display: flex;
					justify-content: space-between;
					align-items: center;
				}
			}

			.pro_type_add {
				display: flex;
				justify-content: flex-start;
				align-items: center;
				font-weight: normal;
				color: rgba(35, 113, 255, 1);
				font-size: 28rpx;
				height: 100rpx;

				.add_icon {
					width: 32rpx;
					height: 32rpx;
					background: rgba(233, 241, 255, 1);
					border-radius: 50%;
					text-align: center;
					line-height: 32rpx;
					margin-right: 16rpx;
				}

				.add_text {
					line-height: 28rpx;
				}
			}
		}
	}

	.product_ul {
		.product_li {
			padding-bottom: 20rpx;

			.li_cont {
				width: 100%;

				.cont_title {
					margin-bottom: 30rpx;
				}
			}
		}
	}
</style>