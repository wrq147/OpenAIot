<template>
	<uni-popup ref="popup" type="bottom" @change="popChange" :z-index="1000">
		<view class="sx-address-picker">
			<view class="sx-address-picker__close">
				<uni-icons class="absolute" size="20" color="#ccc" type="closeempty"
					@click.native="$refs.popup.close"></uni-icons>
			</view>
			<view class="sx-address-picker__title">
				Please select your region
			</view>
			<!-- 已选择区域 -->
			<uni-list v-for="item in activeItems" :key="item.Id" :border="false">
				<uni-list-item show-arrow clickable @click="selectLevel(item)" :customStyle="customStyle">
					<template v-slot:header>
						<view
							:class="(activeLevel === item.level-1 || !item.Name || world ==1) ? 'sx-address-picker__select--active':''" v-if="item.Name">
							{{item.Name}}
						</view>
						<view class="sx-address-picker__select--active" v-else-if="selectTitle">
							{{selectTitle}}
						</view>
					</template>
				</uni-list-item>
			</uni-list>
			<view class="sx-address-picker--divider"></view>
			<view class="sx-address-picker__list">
				<view class="sx-address-picker__list-title">
					{{selectTitle}}
				</view>
				<view class="sx-address-picker__list-content" @click="reGetList">
					<uni-load-more v-if="loadingStatus !== 'more'" :status="loadingStatus"
						:content-text="{contentnomore:'Loading failed, please click retry'}"></uni-load-more>
					<!-- 省市区选择 -->
					<uni-list v-if="world==0" :border="false">
						<uni-list-item v-for="(item,index) in currentList" :key="item.Id" :title="item.Name"
							:border="false" clickable @click="selectHandle(item,index)">
							<template v-slot:footer>
								<uni-icons
									v-if="activeItems[item.level] ? activeItems[item.level].Id === item.Id :false"
									type="checkmarkempty" class="sx-address-picker__list-tab--active"></uni-icons>
							</template>
						</uni-list-item>
					</uni-list>
					<!-- 国家地区列表选择 -->
					<uni-list v-else :border="false">
						<uni-list-item v-for="(item,index) in country.filter((v, i) => v.value !== 0)" :key="item.id"
							:title="item.name" :border="false" clickable @click="selectHandle(item, index, true)">
							<template v-slot:footer>
								<uni-icons v-if="countryName == item.name" type="checkmarkempty"
									class="sx-address-picker__list-tab--active"></uni-icons>
							</template>
						</uni-list-item>
					</uni-list>
				</view>
			</view>
		</view>
	</uni-popup>
</template>
<script>
import { log } from 'mqtt/dist/mqtt'
	const cityDataUrl =
		"https://static-mp-97eb3f65-f610-4509-81e4-a6cd5df3859b.next.bspapp.com/cos/data.json"
	const countryDataUrl =
		"https://static-mp-97eb3f65-f610-4509-81e4-a6cd5df3859b.next.bspapp.com/cos/country.json"

	// 后续计划TODO 常用城市、国家
	// const quickCitys = ['上海市', '北京市', '苏州市', '杭州市', '广州市', '深圳市', '南京市', '天津市']
	// const quickCountys = ['中国香港', '中国澳门', '中国台湾', '新加坡']


	export default {
		props: {
			value: Boolean,
			selected: {
				type: Array,
				default () {
					return []
				}
			},
		},
		data() {
			return {
				world: '0',
				loading: true,
				loadingStatus: 'loading',
				countryName: '',
				country: [],
				// worlds: ['中国内地（大陆）', '港澳台地区及海外'],
				worlds: ['Mainland China'],
				quickIndex: '0',
				list: [],
				activeLevel: -1,
				activeIndex: -1,
				activeItems: [],
				childList: [],
				customStyle:{
					padding: '',
					backgroundColor: '#161a26',
					color:'#fff'
				}
			}
		},
		methods: {
			// 弹窗是否显示状态事件
			resetData(){
				//初始化数据
				this.activeLevel = -1
				this.activeIndex = -1
				this.activeItems = []
				this.loadingStatus='loading'
			},
			popChange(e) {
				const status = e.show
				// pref: 选择省市区未选择完毕重置
				// console.log();
				if (this.world == 0 && this.activeLevel !== 2&&!status) {
					this.activeLevel = -1
					this.activeIndex = -1
					this.activeItems = []
				}else{
					if(status&& this.activeLevel === 2){
						this.activeLevel = this.activeItems.length - 2
					}
				}
				this.$emit('input', status)
			},
			// 已选择项切换事件
			selectLevel(item) {
				if (item.Id && this.world === '0') {
					this.activeLevel = item.level - 1
					if(this.activeItems[this.activeItems.length-1].Id==0){
						this.activeItems.splice(this.activeItems.length-1,1)
					}
				}
			},
			typeChange(e) {
				const value = e.detail.value
				this.world = value
				this.getList()
			},
			// 选择省市区/国家事件 isOhter区分选择省市区
			selectHandle(item, index, isOhter = false) {
				if (isOhter) {
					// 选择国家
					this.activeLevel = -1
					this.activeIndex = 0
					this.countryName = item.name;
					this.activeItems = [item]
					this.$refs.popup.close()
					this.$emit('confirm', this.activeItems)
				} else {
					// 选择省市区
					this.activeLevel = item.level
					this.activeIndex = index
					this.$set(this.activeItems, item.level, item)
					this.activeItems.splice(item.level + 1)
					if (item.children) {
						this.$set(this.activeItems, item.level + 1, {Id:0})
					} else {
						this.$emit('confirm', [{
							Id: 'CN',
							level: -1,
							Name: '中国'
						}, ...this.activeItems])
						this.$refs.popup.close()
						
					}

				}
			},
			// 获取中国省市区数据
			getCityData() {
				if (!this.list.length) {
					this.loadingStatus = 'loading'
					// 循环给省市区增加level级别字段
					const eachList = function(arr, level) {
						arr.forEach(v => {
							v.level = level
							if (v.children) {
								eachList(v.children, v.level + 1)
							}
						})
					}
					this.$store.dispatch("data/areaTree").then(res=>{
						const list = res
						if (Array.isArray(list)) {
							eachList(list, 0)
							this.list = list;
							if (this.selected.length) {
								const activeItems = [];
								if (this.selected.length !== 1) {
									activeItems[0] = this.list.find(v => v.Id === this.selected[1])
									activeItems[1] = activeItems[0].children.find(v => v.Id === this
										.selected[2])
									activeItems[2] = activeItems[1].children.find(v => v.Id === this
										.selected[3])
									activeItems[3] = activeItems[2].children.find(v => v.Id === this
										.selected[4])
									this.activeItems = activeItems
									this.activeLevel = 2
								} else {
									this.activeLevel = -1
								}
							}
							this.loadingStatus = 'more'
						} else {
							this.loadingStatus = 'noMore'
						}
					}).catch(err=>{
						this.loadingStatus = 'noMore'
					})
				} else {
					this.loadingStatus = 'more'
				}
			},
			// 获取国家数据
			getCountryData() {
				if (this.world == 1 && !this.country.length) {
					this.loadingStatus = 'loading'
					uni.request({
						url: countryDataUrl,
						success: res => {
							const data = res.data
							if (data.others) {
								const list = data.others.reduce((arr, v) => {
									arr.push(...v.items)
									return arr
								}, [])
								this.country = list;
								if (this.selected.length) {
									const item = list.find(v => this.selected[0] === v.name)
									this.activeItems = [item]
								}
								this.loadingStatus = 'more'
							} else {
								this.loadingStatus = 'noMore'
							}
						},
						fail: () => {
							this.loadingStatus = 'noMore'
						}
					})
				} else {
					this.loadingStatus = 'more'
				}
			},
			reGetList() {
				if (this.loadingStatus === 'noMore') {
					this.getList()
				}
			},
			getList() {
				// switch (this.world) {
				// 	case '0':
				// 		this.getCityData()
				// 		break;
				// 	case '1':
				// 		this.getCountryData()
				// 		break;
				// }
				this.getCityData()
			}
		},
		computed: {
			// 动态计算当前要选择的列表
			currentList() {
				if (this.activeLevel !== -1) {
					return this.activeItems[this.activeLevel].children
				}
				return this.list
			},
			// 动态标题显示
			selectTitle() {
				const titles = {
					'-1': 'Please select a province/region',
					0: 'Please select a city',
					1: 'Please select district/county',
					// 2: '请选择街道',
				}
				return titles[this.activeLevel]
			},
			// TODO 后续更新
			// quickTitle() {
			// 	const items = {
			// 		0: '常用城市',
			// 		1: '常用国家/地区'
			// 	}
			// 	return items[this.world]
			// },
			// TODO 后续计划 常用选项
			// quickList() {
			// 	return this.world === '0' ? quickCitys : quickCountys
			// }
		},
		watch: {
			// 监听value触发弹窗显示状态以及事件处理
			async value(val) {
				if (val) {
					this.$refs.popup.open()
					const countyName = this.selected[0]
					this.world = this.selected.length === 1 ? '1' : '0'
					if (this.activeItems.length === 4) {
						this.activeLevel = 2
					}
					this.countryName = countyName
					this.getList()
				} else {
					this.$refs.popup.close()
				}
			}
		}
	}
</script>
<style lang="scss" scoped>
	$uni-color-primary:#FF3535;
	
	.sx-address-picker {
		border-top-left-radius: 20px;
		border-top-right-radius: 20px;
		background-color: #161a26;
		padding-left: 8px;
		padding-right: 8px;
		padding-top: 16px;
		font-size: 16px;

		&__close {
			text-align: right;
			margin-right: 16px;
			margin-top: 4px;
			position: absolute;
			left: 0;
			right: 0;
		}
		&__list-title{
			color: #fff;
		}
		&__title {
			text-align: center;
			font-weight: 500;
			font-size: 18px;
			margin-bottom: 20px;
			color: #fff;
		}

		&__select {
			&--active {
				color: $uni-color-primary;
			}
		}

		&--divider {
			border-top: 0.5px solid rgba(255, 255, 255, 0.20);
		}

		&__list {
			max-height: 400px;
			overflow-y: auto;
			margin-top: 12px;

			&-tab {
				display: flex;
				margin-bottom: 20px;
				justify-content: space-between;
				padding-left: 16px;
				padding-right: 16px;

				&--hide {
					display: none;
				}

				&--active {
					color: $uni-color-primary !important;
				}
			}

			&-title {
				font-weight: bold;
				margin-bottom: 16px;
			}

			.uni-load-more {
				padding: 100px 0;
			}
		}
	}
</style>