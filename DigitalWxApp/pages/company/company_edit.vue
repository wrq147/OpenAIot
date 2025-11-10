<template>
	<view class="common_wrap wrap">
		<view class="title">
			编辑企业资料
		</view>
		<view class="lrrow">
			<view class="l">
				<text><text style="color: red;">*</text>企业的Logo</text>
				<text class="tip">（图片比例1:1）</text>
			</view>
			<view class="r" @click="changeLogo">
				<image v-if="companyData.Logo" class="ximg" :src="companyData.Logo" mode="aspectFill"></image>
				<image v-else class="ximg" src="/static/user/company.png" mode="aspectFill"></image>
			</view>
		</view>
		<view class="row" @click="showInput">
			<text><text style="color: red;">*</text>所在企业</text>
			<view class="right">
				<text v-if="!isInput" style="color: #333333;">{{companyData.OrgName}}</text>
				<uni-easyinput v-else class="iptTxt" trim="both" :focus='isfocus' maxlength="50" @blur="hideInput"
					v-model="companyData.OrgName" :inputBorder="false" @clear='isfocus=true'>
				</uni-easyinput>
			</view>

		</view>
		<view class="row" @click="onHYClick">
			<text><text style="color: red;">*</text>行业类型</text>
			<view class="right">
				{{hyName?hyName:"请选择行业类型"}}
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		<view class="row" @click="onSizeClick">
			<view class="left"><text style="color: red;">*</text>员工规模</view>
			<view class="right">
				{{comSizeName?comSizeName:'请选择员工规模'}}
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		<view class="row" @click="chooseLocation">
			<text><text style="color: red;">*</text>所在城市</text>
			<view class="right">
				{{companyData.AddressName?companyData.AddressName:'请选择所在城市'}}
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		<view class="xrow" v-if="showAddDetail">
			<view class="xleft"><text style="color: red;">*</text>详细地址</view>
			<view class="xright">
				<uni-easyinput class="iptTxt" style="text-align: right;" :clearable="showClear" trim="both"
					maxlength="225" v-model="companyData.AddressDetail" :inputBorder="false" placeholder="请输入详细地址"
					@focus="showClear=true" @blur="showClear=false">
				</uni-easyinput>
			</view>
		</view>
		<view class="detail-row">
			<template v-if="companyData.Intro!=null&&companyData.Intro!=''">
				<navigator url="" @click="nav2Intro" class="title" style="border-bottom: none;">企业介绍<uni-icons
						type="forward" size="20" color="#666666"></uni-icons>
				</navigator>
				<view class="jswrap" @click="nav2Intro">
					<mz-editor-parser :datalist="companyData.Intro"></mz-editor-parser>
				</view>
			</template>
			<template v-else>
				<view class="title" style="border: none;">企业介绍</view>
				<view class="jswrap">
					<navigator url="" class="border-all" @click="nav2Intro">
						<view class="uptip">
							<uni-icons custom-prefix="my-icon" type="my-icon-icon_add" size="14" color="#50A6FA">
							</uni-icons>
							<text style="margin-left: 10rpx;">添加企业介绍</text>
						</view>
						<text class="dtip">向客户更好的介绍您的企业吧</text>
					</navigator>
				</view>
			</template>
		</view>

		<view class="fixed_con">
			<view class="common_btn" @click="editSave">
				保存
			</view>
			<navigator url="" class="warn_btn" @click="dismiss">解散企业</navigator>
		</view>

		<uni-data-picker ref="hypick" :map="{text:'Name',value:'Id',children:'children'}" popup-title="请选择行业类型"
			:value="companyData.Industry" :localdata="hyItems" @change="onChangeHY">
		</uni-data-picker>

		<jp-select ref="sizepick" name="label" idKey="value" :checkAll="false" :list="comSizeItems" :item="companySize"
			select="radio" @checked="onChangeSize" tite="请选择员工规模"></jp-select>
	</view>
</template>

<script>
	import {
		getCompanyMessage,
		dismissEnterprise,
		orgEditSave
	} from '@/api/company.js'
	import {
		reverseGeocoder
	} from '@/api/address.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		uploadFile
	} from '@/api/file.js'
	import request from '@/common/request.js'

	export default {
		data() {
			return {
				isfocus: false, //是否自动获取公司名称输入框焦点
				array: ['教育', '餐饮', '网络科技'],
				index: 0,
				array2: ['10~50', '50~150', '200~500', '500以上'],
				index2: 0,
				companyData: {}, //logo表示企业logo
				isInput: false,
				hyName: '', //行业分类
				hyItems: [], //行业分类
				comSizeName: '', //员工规模
				comSizeItems: [], //员工规模
				//设置地址参数
				showDetailPlace: true,
				lng: -1,
				lat: -1,
				addressCode: "",
				enterpriseId: '',
				companySize: null,
				showAddDetail: false, //是否显示地址详情
				showClear: false, //是否显示地址详情的清除按钮
			}
		},
		async onLoad(options) {
			this.enterpriseId = options.id;
			this.reloadpage();

			this.$store.dispatch("industryTree").then(rt => {
				this.hyItems = rt;
			});
			this.$store.dispatch("dictList", "org_size").then(rt => {
				this.comSizeItems = rt;
				// console.log("规模列表",rt);
			});

		},
		methods: {
			changeLogo() {
				//上传logo图片
				uni.chooseImage({
					count: 1, //上传图片的数量
					sizeType: ['compressed'],
					success: async (res) => {
						// console.log("上传图片", res);
						if (res.tempFilePaths.length == 0) {
							return;
						}
						if (res.tempFilePaths.length > 1) {
							uni.showToast({
								title: '只能上传一张图片',
								icon: 'none',
								duration: 1000
							})
						}
						let paths = res.tempFilePaths[0];

						let files = await uploadFile(paths);
						this.companyData.Logo = request.config.baseURL + files
					}
				})
			},
			async reloadpage() {
				try {
					let data = await getCompanyMessage(this.enterpriseId);
					// await getIndustryList(IndustryId);
					if (data.code == 0) {
						this.companyData = data.data;
						if (this.companyData.AddressDetail) {
							this.showAddDetail = true
						}
						// console.log("企业信息", this.companyData);
						if (this.companyData.Industry) {
							let IndustryId = this.companyData.Industry;
							//状态管理   获取行业类型
							this.$store.dispatch("industryName", IndustryId).then(rsp => {
								// console.log('行业信息',rsp);
								this.hyName = rsp;
							})
						}
						//获取企业规模
						if (this.companyData.Size) {
							let size = this.companyData.Size;
							this.$store.dispatch("dictName", {
								name: "org_size",
								value: size
							}).then(rsp => {
								// console.log('企业规模', rsp);
								this.comSizeName = rsp;
								this.comSizeItems.forEach((items) => {
									// console.log("items",items);
									if (items.value == size) {
										this.companySize = items;
									}
								})


							})
						}
					}
				} catch (err) {
					console.log('异常', err)
				}
			},
			//设置显示输入框隐藏text
			showInput() {
				this.isInput = true;
			},
			//设置因此输入框显示text
			hideInput() {
				this.isInput = false;
			},
			//设置行业分类
			onChangeHY(e) {
				this.hyName = e.detail.value[e.detail.value.length - 2].text + ">" + e.detail.value[e.detail.value.length -
					1].text;
				this.companyData.Industry = e.detail.value[e.detail.value.length - 1].value
			},
			//行业选择器显示
			onHYClick() {
				this.$refs.hypick.show();
			},
			//设置地址
			chooseLocation() {
				uni.chooseLocation({
					success: res => {
						// console.log("hhhdhdh", res);
						this.showDetailPlace = true;
						this.companyData.Lng = res.longitude;
						this.companyData.Lat = res.latitude;
						reverseGeocoder(this.companyData.Lng, this.companyData.Lat).then(rsp => {
							// console.log("地址结果", rsp);
							this.companyData.AddressCode = rsp.ad_info.adcode;
							this.showDetailPlace = true;
							let addressArea = ''
							if (rsp.address_component.province == rsp.address_component.city) {
								addressArea = rsp.address_component.province + rsp.address_component
									.district
							} else {
								addressArea = rsp.address_component.province + rsp.address_component
									.city + rsp.address_component.district
							}
							this.companyData.AddressName = addressArea

							this.companyData.AddressDetail = (rsp.address_reference.town.title ? rsp
								.address_reference.town.title : '') + rsp.address.substr(this
								.companyData
								.AddressName.length) + rsp.address_reference.landmark_l2.title
							if (res.address) {
								this.companyData.AddressDetail = res.address.substr(
									this.companyData
									.AddressName.length) + res.name
							}
							this.showAddDetail = true

						})
					}
				});
			},
			//打开员工规模列表
			onSizeClick() {
				this.$refs.sizepick.toOpen();
			},
			//设置员工规模
			onChangeSize(e) {
				// console.log("设置员工规模选中",e);
				this.comSizeName = e.label;
				this.companySize = e;
				this.companyData.Size = e.value;
			},
			nav2Intro() {
				this.$store.state.submitMod.formArrary.push(this.companyData);
				uni.navigateTo({
					url: '/pages/company/company_editor'
				});
			},
			//提交修改后的数据，并进行保存
			async editSave() {
				// console.log('修改后企业的信息', this.companyData)
				if (this.companyData.OrgName == "") {
					uni.showToast({
						title: "企业名称不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.companyData.Industry == "0") {
					uni.showToast({
						title: "请选择行业类型",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (!this.companyData.Size) {
					uni.showToast({
						title: "请选择员工规模",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (!this.companyData.AddressDetail) {
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
					await orgEditSave(this.companyData);
					this.$store.commit('SET_USER_INFO', null);
					reloadPrePage(1, "company");
					uni.navigateBack();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}


			},
			//解散企业
			dismiss() {
				uni.showModal({
					title: '提示',
					content: '确定要解散企业吗？',
					success: async res => {
						if (res.confirm) {
							// this.$refs.popup.close();
							uni.showLoading({
								title: '加载中...'
							});
							try {
								await dismissEnterprise(this.enterpriseId); //访问解散企业后端
								setTimeout(() => {
									uni.showToast({
										icon: 'success',
										title: '解散成功'
									}, 200);
								})
								reloadPrePage(1, "companyEdit");
								uni.navigateBack(); //返回我的

							} catch (err) {
								console.info('异常：', err);
							} finally {
								uni.hideLoading();
							}
						}
					}
				});
				// await dismissEnterprise(this.enterpriseId)

			},
			// bindPickerChange: function(e) {
			// 	console.log('picker1')
			// 	this.index = e.detail.value
			// },
			// bindPickerChange2: function(e) {
			// 	console.log('picker2')
			// 	this.index2 = e.detail.value
			// },

			chooseAdd() {
				uni.navigateTo({
					// url:'./chooseComAdd';
				})
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.uni-data-tree-input {
		display: none;
	}

	.uni-easyinput__content-input {
		text-align: right !important;
	}

	.wrap {
		width: 100%;
		padding-bottom: 260rpx;


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

	.fixed_con {
		position: fixed;
		bottom: 0;
		left: 0;
		width: 100%;
		padding-bottom: 20rpx;
		background-color: #FFFFFF;

		.common_btn {
			margin-top: 30rpx;
		}

		.warn_btn {
			margin-top: 20rpx;
			height: 60rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			color: #ff0000;
		}

	}


	picker {
		color: #999999;
		font-size: 30rpx;
		font-family: pfzho;
	}


	.lrrow {
		display: flex;
		padding: 20rpx 30rpx;
		border-bottom: 1rpx solid #F6F6F6;
		justify-content: space-between;

		.l {
			font-size: 30rpx;
			color: #333333;
			display: flex;
			flex-direction: column;
			justify-content: center;

			.tip {
				font-size: 24rpx;
				color: #999999;
				margin-top: 16rpx;
			}
		}

		.r {
			.ximg {
				width: 160rpx;
				height: 160rpx;
			}
		}
	}

	.detail-row {
		.title {
			display: flex;
			height: 90rpx;
			font-size: 30rpx;
			color: #333333;
			padding: 0 30rpx;
			align-items: center;
			justify-content: space-between;
		}

		.jswrap {
			display: flex;
			flex-direction: column;
			padding: 0 30rpx;

			.uptip {
				color: #50A6FA;
				display: flex;
				justify-content: center;
				padding-top: 60rpx;
				padding-bottom: 10rpx;
			}

			.dtip {
				font-size: 22rpx;
				color: #666666;
				display: flex;
				justify-content: center;
				padding-bottom: 50rpx;
			}

		}
	}
</style>
