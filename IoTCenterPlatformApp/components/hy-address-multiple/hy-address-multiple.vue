<template>
	<view class="page">
		<button style="" @click="save" type="primary">保存</button>
		<view class="content-box">
			<scroll-view scroll-y="true">
				<view v-for="(item,index) in list" :key="index" class="item-box" @click="provinceClick(item,index)">
					<image v-if="getSerVerUrl()&&item.checkStatus==undefined||getSerVerUrl()&&item.checkStatus==0||getSerVerUrl()&&item.checkStatus==null" class="image-ico" :src="getSerVerUrl()+'/appimg/address/none-selected.png'" @click="provinceIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==1" class="image-ico" :src="getSerVerUrl()+'/appimg/address/half-selected.png'" @click="provinceIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==2" class="image-ico" :src="getSerVerUrl()+'/appimg/address/all-selected.png'" @click="provinceIconClick(item,index)"></image>
					<text :class="selectProvinceIndex===index?'selected-index':''">{{item.Name}}</text>
				</view>
			</scroll-view>
			<scroll-view scroll-y="true" v-if="list[selectProvinceIndex] && list[selectProvinceIndex].children">
				<view v-for="(item,index) in list[selectProvinceIndex].children" :key="index" class="item-box left-gap"
					@click="cityClick(item,index)">
					<image v-if="getSerVerUrl()&&item.checkStatus==undefined||getSerVerUrl()&&item.checkStatus==0||getSerVerUrl()&&item.checkStatus==null" class="image-ico" :src="getSerVerUrl()+'/appimg/address/none-selected.png'" @click="cityIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==1" class="image-ico" :src="getSerVerUrl()+'/appimg/address/half-selected.png'" @click="cityIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==2" class="image-ico" :src="getSerVerUrl()+'/appimg/address/all-selected.png'" @click="cityIconClick(item,index)"></image>
					<text :class="selectCityIndex===index?'selected-index':''">{{item.Name}}</text>
				</view>
			</scroll-view>
			<scroll-view scroll-y="true"
				v-if="list[selectProvinceIndex] && list[selectProvinceIndex].children && list[selectProvinceIndex].children[selectCityIndex]">
				<view v-for="(item,index) in list[selectProvinceIndex].children[selectCityIndex].children" :key="index"
					class="item-box left-gap">
					<image v-if="getSerVerUrl()&&item.checkStatus==undefined||getSerVerUrl()&&item.checkStatus==0||getSerVerUrl()&&item.checkStatus==null" class="image-ico" :src="getSerVerUrl()+'/appimg/address/none-selected.png'" @click="regionIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==1" class="image-ico" :src="getSerVerUrl()+'/appimg/address/half-selected.png'" @click="regionIconClick(item,index)"></image>
					<image v-if="getSerVerUrl()&&item.checkStatus&&item.checkStatus==2" class="image-ico" :src="getSerVerUrl()+'/appimg/address/all-selected.png'" @click="regionIconClick(item,index)"></image>
					</image>
					<text>{{item.Name}}</text>
				</view>
			</scroll-view>
		</view>
		
	</view>
</template>

<script>
	// import address from './address.js'
	export default {
		props: {
			value: Boolean,
			address:{
				type: Array,
				default () {
					return []
				}
			},
			selected: {
				type: Array,
				default () {
					return []
				}
			},
		},
		data() {
			return {
				list: [],
				selectProvinceIndex: 0,
				selectCityIndex: 0,
				eventName: 'selectAddressEvent',
				allRegionCodes: [],
				newAddress:[]
			};
		},
		watch: {
			// 监听value触发弹窗显示状态以及事件处理
			async address(val) {
				if (val&&this.newAddress&&this.newAddress.length>0) {
					// console.log("val变化的",val,this.newAddress);
					this.list = JSON.parse(JSON.stringify(this.newAddress))
					this.newAddress=[]
				}
			}
		},
		methods: {
			// 查询地区数据
			getListGroup() {
				this.list = this.address
			},
			getIcon(item) {
				let map = {
					0: 'none-selected', // 全部未选
					1: 'half-selected', // 部分选
					2: 'all-selected' // 全选
				}
				let checkStatus = item.checkStatus ?? 0
				
				return `${getSerVerUrl()}+/appimg/address/${map[checkStatus]}.png`
			},

			// 省icon点击 省市区点击事件全部关联到区的选中取消事件 省市的选中状态根据区的选中状态处理
			provinceIconClick(item, index) {
				// 全选 点击设置成非全选 相反设置成空
				let bol = item.checkStatus === 2 ? 0 : 2
				item.checkStatus = bol
				item?.children?.forEach(m => {
					m.checkStatus = bol
					m?.children?.forEach(n => {
						n.checkStatus = bol
					})
				})
				this.$set(this.list, index, item)
			},

			// 市icon点击
			cityIconClick(item, index) {
				// 全选 点击设置成非全选 相反设置成空
				let bol = item.checkStatus != 2 ? 2 : 0
				item.checkStatus = bol
				item?.children?.forEach(m => {
					m.checkStatus = bol
				})
				this.$set(this.list[this.selectProvinceIndex].children, index, item)
				// 向上触发联动
				this.cityLinkEvent()
			},

			// 区icon点击
			regionIconClick(item, index) {
				let bol = item.checkStatus != 2 ? 2 : 0
				item.checkStatus = bol
				this.$set(this.list[this.selectProvinceIndex].children[this.selectCityIndex].children, index, item)
				// 向上触发联动
				this.regionLinkEvent()
			},

			// 区勾选触发的向上联动
			regionLinkEvent() {
				// 当前市下面所有区
				let tempList = this.list[this.selectProvinceIndex].children[this.selectCityIndex].children || []
				// 所有区为选中，则当前市为全选
				let bol = tempList.every(o => {
					return o.checkStatus === 2
				})
				if (bol) {
					this.$set(this.list[this.selectProvinceIndex].children[this.selectCityIndex], 'checkStatus', 2)
					this.cityLinkEvent()
					return
				}
				// 所有的都为未选 则当前市未选
				let bol1 = tempList.every(o => {
					return o.checkStatus === 0
				})
				if (bol1) {
					this.$set(this.list[this.selectProvinceIndex].children[this.selectCityIndex], 'checkStatus', 0)
					this.cityLinkEvent()
					return
				}
				// 区半选 则市半选 省半选
				this.$set(this.list[this.selectProvinceIndex].children[this.selectCityIndex], 'checkStatus', 1)
				this.$set(this.list[this.selectProvinceIndex], 'checkStatus', 1)
			},

			// 市勾选状态改变触发的向上联动事件
			cityLinkEvent() {
				// 当前省下面所有市
				let tempList = this.list[this.selectProvinceIndex].children || []
				// 所有市为选中，则当前省为全选
				let bol = tempList.every(o => {
					return o.checkStatus === 2
				})
				if (bol) {
					this.$set(this.list[this.selectProvinceIndex], 'checkStatus', 2)
					return
				}
				// 所有的都为未选 则当前市未选
				let bol1 = tempList.every(o => {
					return o.checkStatus === 0
				})
				if (bol1) {
					this.$set(this.list[this.selectProvinceIndex], 'checkStatus', 0)
					return
				}
				this.$set(this.list[this.selectProvinceIndex], 'checkStatus', 1)
			},

			// 省文字点击
			provinceClick(item, index) {
				this.selectProvinceIndex = index
				this.selectCityIndex = 0
			},

			// 城市文字点击
			cityClick(item, index) {
				this.selectCityIndex = index
			},

			/**
			 * 获取全选的省市区code
			 * 省全选 则下面的市区code不加入市区code数组
			 * 市全选 则下面的区code不全选
			 */
			getProcinceCityRegionCodes() {
				let resArr=[]
				let resNameArr=[]
				for (let i = 0; i < this.list.length; i++) {
					let tempObj = this.list[i]
					// 全选
					if (tempObj?.checkStatus === 2) {
						resArr.push(tempObj.Id)
						resNameArr.push(tempObj.Name)
						continue
						// 省份半选
					} else if (tempObj.checkStatus === 1) {
						for (let j = 0; j < tempObj?.children.length; j++) {
							let tempObj1 = tempObj.children[j]
							// console.log(tempObj1, 'tempObj11------------------')
							// 市全选
							if (tempObj1?.checkStatus === 2) {
								resArr.push(tempObj1.Id)
								resNameArr.push(tempObj1.Name)
								continue
								// 市半选	
							} else if (tempObj1?.checkStatus === 1) {
								for (let k = 0; k < tempObj1.children.length; k++) {
									let tempObj2 = tempObj1.children[k]
									// 区选中
									if (tempObj2?.checkStatus === 2) {
										resArr.push(tempObj2.Id)
										resNameArr.push(tempObj2.Name)
									}
								}
							}
						}
					}
				}
				return {resArr,resNameArr}
			},

			/**
			 * 获取所有选中区的code
			 * 省市选中则下面的区也为选中
			 * 当省为选中状态,省下面没有市区时候也加入code
			 * 当市为选中状态, 市下面没有区时候也加入code
			 */
			getAllRegionCode() {
				let code = []
				for (let i = 0; i < this.list.length; i++) {
					let tempObj = this.list[i]
					// 为0或者空未选中状态跳过 空选状态
					if (!tempObj.checkStatus) {
						continue;
					}
					// 全选状态
					// 状态为2 并且没有子节点则直接加入code并跳过直接进入下一个循环
					if (tempObj.checkStatus === 2 && !tempObj?.children) {
						code.push(tempObj.Id)
						continue;
					}
					// 半选状态 不可能存在没有子节点
					for (let j = 0; j < tempObj.children.length; j++) {
						let tempObj1 = tempObj.children[j]
						if (!tempObj1.checkStatus) {
							continue;
						}
						if (tempObj1.checkStatus === 2 && !tempObj1?.children) {
							code.push(tempObj1.Id)
							continue;
						}
						for (let k = 0; k < tempObj1.children.length; k++) {
							let tempObj2 = tempObj1.children[j]
							if (tempObj2?.checkStatus === 2) {
								code.push(tempObj1.Id)
							}
						}
					}
				}
				return code
			},

			// 保存
			save() {
				let {resArr,resNameArr} = this.getProcinceCityRegionCodes()
				// let codes = this.getAllRegionCode()
				this.$emit('confirm',resArr,resNameArr)
			}
		},
		created() {
			this.getListGroup()
		}
	}
</script>
<style lang="scss" scoped>
	.page {
		width: 100%;
		height: 70vh;
		background-color: #fff;

		.content-box {
			padding: 40rpx 40rpx;
			display: flex;

			.left-gap {
				margin-left: 16rpx;
			}

			.item-box {
				padding: 10rpx 0;
				flex: 1;
				padding-right: 10rpx;
				display: flex;
				align-items: center;

				text {
					font-size: 26rpx;
					margin-left: 10rpx;
				}
			}
		}
	}

	scroll-view {
		width: 223rpx;
		height: calc(70vh - 100rpx);
	}

	.image-ico {
		width: 30rpx;
		height: 30rpx;
		flex-shrink: 0;
	}

	.selected-index {
		color: #0081ff;
	}

	::v-deep ::-webkit-scrollbar {
		// 滚动条整体样式
		display: block;
		width: 4rpx !important;
		height: 4rpx !important;
		overflow: auto !important;
	}

	::v-deep ::-webkit-scrollbar-thumb {
		// 滚动条里面小方块
		background-color: #90909070 !important;
	}

	::v-deep ::-webkit-scrollbar-track {
		// 滚动条
		background-color: #f0f0f0 !important;
	}
</style>