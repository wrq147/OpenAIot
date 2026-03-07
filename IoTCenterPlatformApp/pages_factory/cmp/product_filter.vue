<template>
	<view>
		<uni-popup ref="popup" type="bottom" @change="popChange" :zIndex="999" v-if="isPopup">
			<view class="condition_con" :key="'filterKey'+filterKey">
				<view class="condition_title">
					<view class="cancel" @click="close">取消</view>
					<view class="title_text">筛选条件</view>
					<view class="condition_add" @click.stop="choiceFiled">
						<view class="icon_con">
							<custom-icons iconsName="icon-tianjia" iconsSize="16rpx"
								iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
						</view>
						<text>添加</text>
					</view>
				</view>
				<view class="condition_ul">
					<view class="condition_li" v-for="(ite,inx) in activeFilterList">
						<view class="icon_del t-icon-shouqi1" @click="delFilterLi(inx)"></view>
						<view class="label_text">{{filterFiledType(inx).name}}</view>
						<view class="compare_con">
							<uni-data-select v-model="activeFilterList[inx].compare" :localdata="returnCompareList(inx)"
								@change="changeCompare" width="100%" placeholder="请选择比较符"
								borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)" :isCustom="true"
								:isDark="false"></uni-data-select>
						</view>
						<view class="value_con">
							<uni-easyinput v-if="filterFiledType(inx)&&filterFiledType(inx).type=='数字'"
								placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="digit" v-model="activeFilterList[inx].val_num" placeholder="请输入值"
								contentFontSize="32rpx" />
							<view class="int_date" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='时间'">
								<view class="date_con long_date" style="color: #fff;">
									<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
										placeholder-style="font-size:32rpx;color:#999999" :clear-icon="true"
										v-model="activeFilterList[inx].val" placeholder='时间值' :isCustom="true" :isDark="false" :border="false">
									</uni-datetime-picker>
								</view>
							</view>
							<uni-easyinput
								v-if="filterFiledType(inx)&&filterFiledType(inx).type=='关联对象'&&activeFilterList[inx].compare!='关联'||filterFiledType(inx)&&filterFiledType(inx).type=='文本'"
								placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="text" v-model="activeFilterList[inx].val" placeholder="请输入值"
								contentFontSize="32rpx" />
							<zxz-uni-data-select :placeholder="'请输入'+filterFiledType(inx).object_type" @focus="afterValSearch(activeFilterList[inx].val,filterFiledType(inx),activeFilterList[inx].field+inx)"
								v-if="filterFiledType(inx)&&filterFiledType(inx).type=='关联对象'&&activeFilterList[inx].compare=='关联'&&activeFilterList[inx].field"
								@inputChange="associationMethod($event,filterFiledType(inx),activeFilterList[inx].field+inx)"
								:filterable="true" v-model="activeFilterList[inx].val" filterable :multiple="false" dataKey="Name"
								dataValue="enValue" :localdata="associationObject[activeFilterList[inx].field+inx]" :isCanCustom="true"
								:iscustom="true"></zxz-uni-data-select>
						</view>
					</view>
				</view>
				<view class="handle_zhanwei"></view>
				<view class="handle_con">
					<view class="handle_left">
						<view class="clear_btn" @click="clearAll">清空</view>
						<view class="save_handle" @click="jumpToSaveFilter">另存为新分组</view>
					</view>
					<view class="filter_handle" @click="setFilterHandle">筛选</view>
				</view>
			</view>
		</uni-popup>
		<view class="condition_con" :key="'filterKey'+filterKey" v-else style="height:auto">
			<view class="condition_ul" style="max-height:100%">
				<view class="condition_li" v-for="(ite,inx) in activeFilterList">
					<view class="icon_del t-icon-shouqi1" @click="delFilterLi(inx)"></view>
					<view class="label_text">{{filterFiledType(inx).name}}</view>
					<view class="compare_con">
						<uni-data-select v-model="activeFilterList[inx].compare" :localdata="returnCompareList(inx)"
							@change="changeCompare" width="100%" placeholder="请选择比较符"
							borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)" :isCustom="true"
							:isDark="false"></uni-data-select>
					</view>
					<view class="value_con">
						<uni-easyinput v-if="filterFiledType(inx)&&filterFiledType(inx).type=='数字'"
							placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="digit" v-model="activeFilterList[inx].val_num" placeholder="请输入值"
							contentFontSize="32rpx" />
						<view class="int_date" v-if="filterFiledType(inx)&&filterFiledType(inx).type=='时间'">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="true"
									v-model="activeFilterList[inx].val" placeholder='时间值' :isCustom="true" :isDark="false" :border="false">
								</uni-datetime-picker>
							</view>
						</view>
						<uni-easyinput
							v-if="filterFiledType(inx)&&filterFiledType(inx).type=='关联对象'&&activeFilterList[inx].compare!='关联'||filterFiledType(inx)&&filterFiledType(inx).type=='文本'"
							placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="activeFilterList[inx].val" placeholder="请输入值"
							contentFontSize="32rpx" />
						<zxz-uni-data-select :placeholder="'请输入'+filterFiledType(inx).object_type" @focus="afterValSearch(activeFilterList[inx].val,filterFiledType(inx),activeFilterList[inx].field+inx)"
							v-if="filterFiledType(inx)&&filterFiledType(inx).type=='关联对象'&&activeFilterList[inx].compare=='关联'&&activeFilterList[inx].field"
							@inputChange="associationMethod($event,filterFiledType(inx),activeFilterList[inx].field+inx)"
							:filterable="true" v-model="activeFilterList[inx].val" filterable :multiple="false" dataKey="Name"
							dataValue="enValue" :localdata="associationObject[activeFilterList[inx].field+inx]" :isCanCustom="true"
							:iscustom="true"></zxz-uni-data-select>
					</view>
				</view>
				<view class="filter_add" @click.stop="choiceFiled">
					<view class="icon_con">
						<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
							iconsColor="#333333"></custom-icons>
					</view>
					<view class="text">添加过滤条件</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	var dayjs = require('@/common/day.js')
	import {
		factorySearchObject
	} from '@/api/product.js'
	export default {
		props: {
			productFiledList: {
				type: Array,
				default: () => {
					return []
				}
			},
			filterKey:{
				type:[String,Number],
				default:'0'
			},
			isPopup:{
				type:Boolean,
				default:true
			}
		},
		data() {
			return {
				activeFilterList: [],
				compareList: [],
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(248, 248, 248, 1)',
					borderColor: '#F8F8F8'
				},
				candidates:[],
				associationObject:{}
			}
		},
		watch:{
			filterKey(){
				
				this.$forceUpdate()
			},
			activeFilterList:{
				immediate:false,
				deep:true,
				handler(newVal) {
					this.$emit('filterListChange',newVal)
				}
			}
		},
		computed: {
			filterProductFiledList() {
				//过滤文本，时间，数字类型的字段
				let list = this.productFiledList.filter(row => row.type == '文本' || row.type == '时间' || row.type == '数字' ||
					row.type == '关联对象')
				return list
			}
		},
		methods: {
			changeTypeFilter(val){
				//有添加默认的过滤分组时，更新过滤的数据
				let findx=this.activeFilterList.findIndex(row=>row.isdef)
				if(findx>-1){
					this.activeFilterList[findx].val=val
				}
			},
			jumpToSaveFilter(){
				// 过滤另存为新分组
				uni.navigateTo({
					url: '/pages_factory/product_type_add',
					success: (res)=>{
						// 通过eventChannel向被打开页面传送数据
						res.eventChannel.emit('acceptFilterData', {
							filterList:this.activeFilterList
						})
					}
				})
			},
			clearAll(){
				//清空
				this.activeFilterList=[]
				this.$emit('filterListChange',this.activeFilterList)
			},
			delFilterLi(inx){
				//删除一行元素
				this.activeFilterList.splice(inx,1)
				this.$emit('filterListChange',this.activeFilterList)
			},
			setFilterHandle(){
				let filterList=JSON.parse(JSON.stringify(this.activeFilterList))
				filterList=filterList.map(rw=>{
				  let rowObj=this.filterProductFiledList.find(row=>row.mapid==rw.field)
				  if(rowObj&&rowObj.type=='时间'){
				    rw.val_num=dayjs(rw.val).valueOf()
				    rw.val=''
				  }
				  if(rw.val_num){}else{
				    delete rw.val_num
				  }
				  if(rw.val_arr&&rw.val_arr.length>0){}else{
				    delete rw.val_arr
				  }
				  return rw
				})
				this.close()
				this.$emit('finishSelect',filterList)
			},
			setNewFilterData(filterList){
				if(filterList){
					this.activeFilterList = JSON.parse(JSON.stringify(filterList))
				}
				this.$forceUpdate()
			},
			choiceFiled(){
				this.$emit('choiceFiled',this.filterProductFiledList)
			},
			returnCompareList(inx) {
				if (this.activeFilterList[inx].field) {
					let rowObj = this.filterProductFiledList.find(row => row.mapid == this.activeFilterList[inx].field)
					if (rowObj && rowObj.type == '数字' || rowObj.type == '时间') {
						return [{
							text: '大于',
							value: '大于'
						}, {
							text: '小于',
							value: '小于'
						}, {
							text: '大于等于',
							value: '大于等于'
						}, {
							text: '小于等于',
							value: '小于等于'
						}, {
							text: '不等于',
							value: '不等于'
						}, {
							text: '等于',
							value: '等于'
						}]
					} else if (rowObj && rowObj.type == '关联对象') {
						return [{
							text: '关联',
							value: '关联'
						}, {
							text: '包含',
							value: '包含'
						}, {
							text: '不包含',
							value: '不包含'
						}]
					} else {
						return [{
							text: '等于',
							value: '等于'
						}, {
							text: '包含',
							value: '包含'
						}, {
							text: '不包含',
							value: '不包含'
						}]
					}
				}
			},
			filterFiledType(inx) {
				if (this.activeFilterList[inx] && this.activeFilterList[inx].field) {
					let findObj = this.filterProductFiledList.find(row => row.mapid == this.activeFilterList[inx].field)
					return findObj
				} else {
					return {}
				}
			},
			changeCompare(val) {

			},
			popChange() {

			},
			close() {
				this.$refs.popup.close();
			},
			openPopup(filterList) {
				if (filterList) {
					filterList=filterList.map((rw,inx)=>{
					  let rowObj=this.productFiledList.find(row=>row.mapid==rw.field)
					  if(rowObj&&rowObj.type=='时间'){
						rw.val=dayjs(rw.val_num).format('YYYY-MM-DD HH:mm:ss')
					  }else if(rowObj&&rowObj.type=='关联对象'){
						if(rw.compare=='关联'){
						  this.afterValSearch(rw.val,rowObj,rw.field+inx)
						}
						
					  }
					  return rw
					})
					this.activeFilterList = JSON.parse(JSON.stringify(filterList))
				}
				if(this.isPopup){
					this.$refs.popup.open('bottom');
				}
				
			},
			afterValSearch(val, item, keymapId) { //关联对象回显时获取列表
				if (val && val.indexOf(',') > -1) {
					let keyVal = val.split(',')
					this.associationMethod(keyVal[1], item, keymapId)
				} else {
					this.associationMethod('', item, keymapId)
				}
			},
			associationMethod(query, item, keymapId) { //关联对象的远程搜索事件
				// console.log("关联对象",query);
				this.getFactorySearchObject(query, item.object_type, keymapId)
			},
			async getFactorySearchObject(key, objtype, mapid) {
				//根据不同的关联对象获取对象的列表
				let obj = {
					key: key,
					objtype: objtype,
					pageNum: 1,
					pageSize: 10
				}
				let res = await factorySearchObject(obj)
				if (res.data.List) {
					// console.log("res.data.List",res.data.List,mapid,this.associationObject);
					let resList=res.data.List.map(row=>{
						row.enValue=row.Value+','+row.ValueName
						return row
					})
					this.associationObject[mapid] = JSON.parse(JSON.stringify(resList))
				}
				this.$forceUpdate()
				// console.log(res,'resres');
			},
		}
	}
</script>

<style lang="less" scoped>
	.condition_con {
		// height: 70%;
		border-radius: 20rpx 20rpx 0 0;
		background-color: #ffffff;
		height: 70vh;
		position: relative;
		.condition_ul{
			max-height: calc(70vh - 128rpx);
		}
		.handle_zhanwei{
			height: 128rpx;
			width: 100%;
		}
		.handle_con {
			background: #ffffff;
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

		.condition_title {
			border-radius: 20rpx 20rpx 0 0;
			height: 92rpx;
			padding: 40rpx 30rpx 20rpx 30rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			width: 100%;
			box-sizing: border-box;

			.cancel {
				font-size: 28rpx;
				color: rgba(153, 153, 153, 1);
			}

			.title_text {
				font-size: 32rpx;
				color: rgba(51, 51, 51, 1);
				font-weight: bold;
				
			}

			.condition_add {
				display: flex;
				align-items: center;
				justify-content: center;
				font-size: 28rpx;
				color: rgba(35, 113, 255, 1);

				.icon_con {
					display: flex;
					align-items: center;
					justify-content: center;
					background: rgba(233, 241, 255, 1);
					width: 32rpx;
					height: 32rpx;
					border-radius: 50%;
					margin-right: 10rpx;
				}
			}
		}

		.condition_li {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			margin-top: 20rpx;
			width: 100%;
			padding: 0 20rpx;
			box-sizing: border-box;

			.icon_del {
				width: 44rpx;
				height: 44rpx;
				border-radius: 50%;
			}

			.label_text {
				margin-left: 20rpx;
				font-size: 28rpx;
				color: rgba(51, 51, 51, 1);
				width: 168rpx;
			}

			.compare_con {
				width: 180rpx;
				height: 80rpx;
				margin-left: 10rpx;
			}

			.value_con {
				width: 278rpx;
				height: 80rpx;
				margin-left: 30rpx;
				margin-left: 10rpx;
				.int_date {
					.date_con {
						overflow: hidden;
						.date_slot {
							// justify-content: center;
							overflow-x: auto;
							white-space: nowrap;
							overflow-y: hidden;
						}
					}
				}
			}
		}
		.filter_add{
			display: flex;
			justify-content: center;
			align-items: center;
			width: 100%;
			height: 88rpx;
			font-size: 28rpx;
			color: rgba(51, 51, 51, 1);
			background: rgba(248, 248, 248, 1);
			margin-top: 30rpx;
			.icon_con{
				margin-right: 10rpx;
				line-height: 24rpx;
			}
			.text{
				line-height: 28rpx;
			}
		}
	}
</style>