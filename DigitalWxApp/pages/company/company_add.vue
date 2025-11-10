<template>
	<view class="common_wrap wrap">
		<view class="title">
			创建全新企业
		</view>
		<view class="row">
			<view class="left"><text style="color: red;">*</text>所在企业</view>
			<view class="right">
				<uni-easyinput class="iptTxt" trim="both" maxlength="50" v-model="OrgName" :focus='true'
					:inputBorder="false" placeholder="请输入所在企业名称">
				</uni-easyinput>
			</view>
		</view>

		<navigator url="" class="row" @click="onHYClick">
			<view class="left"><text style="color: red;">*</text>行业类型</view>
			<view class="right">
				{{hyName}}
				<uni-icons type="forward" size="20" color="#666666"></uni-icons>
			</view>
		</navigator>

		<navigator url="" class="row" @click="onSizeClick">
			<view class="left"><text style="color: red;">*</text>员工规模</view>
			<view class="right">
				{{comSizeName}}
				<uni-icons type="forward" size="20" color="#666666"></uni-icons>
			</view>
		</navigator>

		<navigator url="" class="row" @click="chooseLocation">
			<view class="left"><text style="color: red;">*</text>所在城市</view>
			<view class="right">
				{{addressName}}
				<uni-icons type="forward" size="20" color="#666666"></uni-icons>
			</view>
		</navigator>
		<view class="xrow" v-if="showDetailPlace">
			<view class="xleft"><text style="color: red;">*</text>详细地址</view>
			<view class="xright">
				<uni-easyinput class="iptTxt" style="text-align: right;" :clearable="false" trim="both" maxlength="225"
					v-model="addressDetail" :inputBorder="false" placeholder="请输入详细地址">
				</uni-easyinput>
			</view>
		</view>

		<button class="common_btn" @click="onCreate">
			创建
		</button>

		<uni-data-picker ref="hypick" :map="{text:'Name',value:'Id',children:'children'}" popup-title="请选择行业类型"
			:value="hyValue" :localdata="hyItems" @change="onChangeHY">
		</uni-data-picker>

		<jp-select ref="sizepick" name="label" idKey="value" :checkAll="false" :list="comSizeItems" :item="comSizeValue"
			select="radio" @checked="onChangeSize" tite="请选择员工规模"></jp-select>

	</view>
</template>

<script>
	import {
		createOrg,
		switchOrg
	} from '@/api/org.js'
	import {
		reverseGeocoder
	} from '@/api/address.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				hyName: "请选择",
				hyValue: "0",
				hyItems: [],
				comSizeName: "请选择",
				comSizeValue: {},
				comSizeItems: [],
				OrgName: "",
				showDetailPlace: false,
				addressName: '请选择',
				addressDetail: '',
				lng: -1,
				lat: -1,
				addressCode: ""
			};
		},
		onLoad() {
			this.$store.dispatch("industryTree").then(rt => {
				this.hyItems = rt;
			});
			this.$store.dispatch("dictList", "org_size").then(rt => {
				this.comSizeItems = rt;
			});
		},
		methods: {
			onSizeClick() {
				this.$refs.sizepick.toOpen();
			},
			onHYClick() {
				this.$refs.hypick.show();
			},
			onChangeHY(e) {
				this.hyName = e.detail.value[e.detail.value.length - 2].text + ">" + e.detail.value[e.detail.value.length -
					1].text;
				this.hyValue = e.detail.value[e.detail.value.length - 1].value;
			},
			onChangeSize(e) {
				this.comSizeName = e.label;
				this.comSizeValue = e;
			},
			chooseLocation() {
				uni.chooseLocation({
					success: res => {
						this.showDetailPlace = true;
						this.lng = res.longitude;
						this.lat = res.latitude;
						reverseGeocoder(this.lng, this.lat).then(rsp => {
							// console.log("地址结果", rsp);
							this.addressCode = rsp.ad_info.adcode;
							this.showDetailPlace = true;
							let addressArea = ''
							if (rsp.address_component.province == rsp.address_component.city) {
								addressArea = rsp.address_component.province + rsp.address_component
									.district
							} else {
								addressArea = rsp.address_component.province + rsp.address_component
									.city + rsp.address_component.district
							}
							this.addressName = addressArea

							this.addressDetail = (rsp.address_reference.town.title ? rsp
								.address_reference.town.title : '') + rsp.address.substr(this
								.addressName.length) + rsp.address_reference.landmark_l2.title
							if (res.address) {
								this.addressDetail = res.address.substr(this.addressName.length) + res
									.name
							}

						});
					}
				});
			},
			async onCreate() {
				if (this.OrgName == "") {
					uni.showToast({
						title: "企业名称不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.hyValue == "0") {
					uni.showToast({
						title: "请选择行业类型",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (!this.comSizeValue.hasOwnProperty("value")) {
					uni.showToast({
						title: "请选择员工规模",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (!this.showDetailPlace) {
					uni.showToast({
						title: "请选择所在地址",
						icon: "none",
						duration: 2000
					});
					return;
				}
				uni.showLoading({
					title: '加载中...'
				});
				try {
					console.log("添加公司", {
						OrgName: this.OrgName,
						Logo: "",
						Industry: this.hyValue,
						Size: this.comSizeValue.value,
						Lng: this.lng,
						Lat: this.lat,
						AddressName: this.addressName,
						AddressCode: this.addressCode,
						AddressDetail: this.addressDetail,
						Intro: ""
					});
					let rsp = await createOrg({
						OrgName: this.OrgName,
						Logo: "",
						Industry: this.hyValue,
						Size: this.comSizeValue.value,
						Lng: this.lng,
						Lat: this.lat,
						AddressName: this.addressName,
						AddressCode: this.addressCode,
						AddressDetail: this.addressDetail,
						Intro: ""
					});
					// console.log("加入企业",rsp);
					this.$store.commit('SET_USER_INFO', null);
					reloadPrePage(1, "company");
					uni.navigateBack();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF !important;
	}

	.uni-data-tree-input {
		display: none;
	}

	.uni-easyinput__content-input {
		text-align: right !important;
	}


	.wrap {
		width: 100%;
		background-color: #FFFFFF;

		.title {
			height: 120rpx;
			line-height: 120rpx;
			padding-left: 30rpx;
			box-sizing: border-box;
			color: #333333;
			font-family: pfzho;
			border-bottom: 1px solid #F6F6F6;
			font-size: 34rpx;
		}

		.xrow {
			width: 100%;
			display: flex;
			justify-content: space-between;
			padding: 0 30rpx;
			box-sizing: border-box;
			font-size: 30rpx;
			background-color: #FFFFFF;
			border-bottom: 1rpx solid #F6F6F6;

			.xleft {
				width: 200rpx;
				padding-top: 30rpx;
			}

			.xright {
				flex: 1;
				padding-top: 20rpx;
				padding-bottom: 20rpx;
			}
		}
	}

	.common_btn {
		margin-top: 50rpx;
	}
</style>
