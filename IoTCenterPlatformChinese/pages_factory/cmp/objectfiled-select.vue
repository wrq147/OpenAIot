<template>
	<uni-popup ref="filedpopup" type="bottom" @change="popChange" :zIndex="999">
		<view class="filed_con">
			<view class="filed_title">
				<view class="cancel" @click="close">取消</view>
				<view class="title_text">请选择</view>
				<view>
					<view class="filed_all" v-if="multiple" @click="setAllChoice">
						<view class="icon_con">
							<custom-icons v-if="isSelectAll" iconsName="icon-weixuanzhong" iconsSize="36rpx"
								iconsColor="rgba(234, 234, 234, 1)"></custom-icons>
							<view v-else class="icon_del t-icon-gouxuan1"></view>
						</view>
						<text style="margin-left: 12px;">全选</text>
					</view>
				</view>
			</view>
			<search-compt :hasKey="key" @searching="searching" pal="请输入产品关键字" :fixed="true"
				backgroundColor="#fff" inputBg="#F8F8F8" :isOnlySearch="true"></search-compt>
			<scroll-view :scroll-top="scrollTop" scroll-y="true" class="filed_ul" @scrolltolower="scrollLower">
				<!-- <view class="filed_ul"> -->
				<view class="filed_li" v-for="(it,inx) in objectList" @click="setRowSelect(it,inx)">

					<view v-if="it.select" class="icon_sel t-icon-gouxuan1"></view>
					<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="36rpx"
						iconsColor="rgba(234, 234, 234, 1)"></custom-icons>
					<view class="filed_li_text">{{it.textname}}</view>
				</view>
				<!-- </view>  -->
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</scroll-view>
			
			<view class="handle_zhanwei" v-if="multiple"></view>
			<view class="handle_con" v-if="multiple">
				<view class="handle_left">
					已选：{{selectLen}}项
				</view>
				<view class="filter_handle" @click="finishFiledChoice">确定</view>
			</view>
		</view>
	</uni-popup>
</template>

<script>
	var dayjs = require('@/common/day.js')
	import {
		factorySearchObject,
		factorySupplierListGet
	} from '@/api/product.js'
	import {iotProductList} from '@/api/ruselSevic.js'
	export default {
		data() {
			return {
				objectList: [],
				scrollTop: 0,
				multiple: false,
				pageNum: 1,
				objtype: '',
				key: '',
				status: 'loading',
				alchoice:[]
			}
		},
		computed: {
			selectLen() {
				let filterAll = this.objectList.filter(row => row.select)
				return filterAll.length
			},
			isSelectAll() {
				let obj = this.objectList.find(row => row.select == undefined||!row.select)
				if (obj) {
					return true
				} else {
					return false
				}
			}
		},
		methods: {
			setAllChoice(){
				if(this.isSelectAll){
					this.objectList=this.objectList.map(row=>{
						row.select=true
						return row
					})
				}else{
					this.objectList=this.objectList.map(row=>{
						row.select=false
						return row
					})
				}
			},
			async scrollLower() {
				if (this.status != 'noMore') {
					this.pageNum++;
					this.status = 'loading';
					if(this.objtype=='物联网产品'){
						await this.getIOTProductList()
					}else if(this.objtype=='供应商'){
						await this.getSupplierList()
					}else{
						await this.getFactorySearchObject();
					}
					
				}
			},
			async getSupplierList() { //获取供应商列表接口
				let obj = {
					key: this.key,
					pageNum: this.pageNum,
					pageSize: 10,
				}
				let res = await factorySupplierListGet(obj)
				//  console.log("查询到供应商",res);
				if (this.pageNum == 1) {
					this.objectList = []
				}
				let resList = res.data.List.map(row => {
					row.enValue = row.Id + ',' + row.SupplierName
					row.name=row.SupplierName
					row.textname=row.SupplierName
					let alFindx=this.alchoice.findIndex(ro=>ro==row.enValue)
					if(alFindx>-1){
						row.select=true
					}
					return row
				})
				this.objectList = [...this.objectList, ...JSON.parse(JSON.stringify(resList))]
				console.log(this.objectList,'this.objectList');
				if (res.data.List.length < 10) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}
			},
			async getIOTProductList() { //获取物联网产品列表接口
				try{
					let obj = {
						pageNum: this.pageNum,
						pageSize: 10,
						Name: this.key,
					}
					let res = await iotProductList(obj)
					 console.log("查询到物联网产品",res);
					if (this.pageNum == 1) {
						this.objectList = []
					}
					let resList = res.data.List.map(row => {
						row.enValue = row.Id + ',' + row.Name
						row.name=row.Name
						row.textname=row.Name
						let alFindx=this.alchoice.findIndex(ro=>ro==row.enValue)
						if(alFindx>-1){
							row.select=true
						}
						return row
					})
					
					this.objectList = [...this.objectList, ...JSON.parse(JSON.stringify(resList))]
					
					if (res.data.List.length < 10) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}catch(e){
					
				}

			},
			finishFiledChoice() {
				let filterSelectArr = this.objectList.filter(row => row.select)
				this.close()
				this.$emit('finishFiledChoice', filterSelectArr)

			},
			setRowSelect(it, inx) {
				if (this.multiple) {
					if (it.select) {
						this.objectList[inx].select = false
					} else {
						this.objectList[inx].select = true
					}
					let list=JSON.parse(JSON.stringify(this.objectList))
					this.objectList=JSON.parse(JSON.stringify(list))
				} else {
					this.$emit('finishFiledChoice', [it])
					this.close()
				}

				this.$forceUpdate()
			},
			close() {
				this.$refs.filedpopup.close();
			},
			popChange() {

			},
			loadList(type) {

			},
			async getFactorySearchObject() {
				//根据不同的关联对象获取对象的列表
				let obj = {
					key: this.key,
					objtype: this.objtype,
					pageNum: this.pageNum,
					pageSize: 10
				}
				let res = await factorySearchObject(obj)
				if (this.pageNum == 1) {
					this.objectList = []
				}
				if (res.data.List) {
					let list=JSON.parse(JSON.stringify(this.objectList))
					this.objectList=JSON.parse(JSON.stringify(list))
					let resList = res.data.List.map(row => {
						row.enValue = row.Value + ',' + row.ValueName
						row.name=row.ValueName
						row.textname=row.Name
						let alFindx=this.alchoice.findIndex(ro=>ro==row.enValue)
						if(alFindx>-1){
							row.select=true
						}
						return row
					})
					this.objectList = [...this.objectList, ...JSON.parse(JSON.stringify(resList))]
					// console.log("数据结果",this.objectList);
					if (res.data.List.length < 10) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}
				this.$forceUpdate()
				// console.log(res,'resres');
			},
			async searching(val){
				this.key=val
				this.pageNum=1
				this.status='loading'
				if(this.objtype=='物联网产品'){
					await this.getIOTProductList()
				}else if(this.objtype=='供应商'){
					await this.getSupplierList()
				}else{
					await this.getFactorySearchObject();
				}
			},
			async openPopup(type, multiple, alchoice) {
				this.objtype = type
				if(alchoice[0]&&alchoice[0].indexOf(',')>-1){
					let keyVal=alchoice[0].split(',')
					this.key = keyVal[1]
				}else{
					this.key = ''
				}
				if(alchoice){
					this.alchoice=alchoice
					
				}else{
					this.alchoice=[]
				}
				if(type=='物联网产品'){
					await this.getIOTProductList()
				}else if(type=='供应商'){
					await this.getSupplierList()
				}else{
					await this.getFactorySearchObject();
				}
				
				this.$refs.filedpopup.open('bottom');
			}
		}
	}
</script>

<style lang="less" scoped>
	.filed_con {
		// height: 70%;
		border-radius: 20rpx 20rpx 0 0;
		background-color: #ffffff;

		.filed_ul {
			height:calc(70vh - 128rpx) ;
			max-height: calc(70vh - 128rpx);
			overflow-y: auto;
		}

		.handle_zhanwei {
			height: 128rpx;
			width: 100%;
		}

		.handle_con {
			position: fixed;
			bottom: 0;
			left: 0;
			display: flex;
			width: 100%;
			justify-content: space-between;
			align-items: center;
			padding: 20rpx 30rpx 30rpx;
			height: 128rpx;
			border-top: 1rpx solid rgba(234, 234, 234, 1);
			box-sizing: border-box;
			background: #ffffff;

			.handle_left {
				display: flex;
				justify-content: flex-start;
				align-items: center;
				font-size: 30rpx;
				color: rgba(102, 102, 102, 1);

				.clear_btn {
					padding: 24rpx 50rpx;
					border: 1rpx solid rgba(216, 216, 216, 1);
					border-radius: 44rpx;
				}

				.save_handle {
					padding: 24rpx 30rpx;
					border: 1rpx solid rgba(216, 216, 216, 1);
					border-radius: 44rpx;
					margin-left: 10rpx;
				}
			}

			.filter_handle {
				padding: 24rpx 50rpx;
				color: rgba(255, 255, 255, 1);
				background: rgba(35, 113, 255, 1);
				border-radius: 44rpx;
			}
		}

		.filed_title_zhanwei {
			width: 100%;
			padding: 38rpx 30rpx;
			height: 112rpx;
			box-sizing: border-box;
		}

		.filed_title {
			width: 100%;
			padding: 38rpx 30rpx;
			height: 112rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
			background: #ffffff;
			box-sizing: border-box;
			border-radius: 20rpx 20rpx 0 0;

			.title_text {
				font-size: 32rpx;
				color: rgba(51, 51, 51, 1);
				font-weight: bold;
			}

			.filed_all {
				display: flex;
				justify-content: center;
				align-items: center;
				color: rgba(102, 102, 102, 1);
			}
		}

		.filed_li {
			width: 100%;
			padding: 0 30rpx;
			display: flex;
			justify-content: flex-start;
			align-items: center;
			height: 100rpx;
			box-sizing: border-box;

			.icon_sel {
				width: 36rpx;
				height: 36rpx;
			}

			.filed_li_text {
				margin-left: 20rpx;
				color: rgba(51, 51, 51, 1);
				font-size: 28rpx;
			}
		}
	}
</style>