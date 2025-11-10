<template>
	<view @click="parIsFocus=false" style="min-height: 100vh;">
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" rightWidth="0rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#161A26">
		</top>
		<uni-forms ref="consumbleForm" :modelValue="consumbleForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="Consumables number" required name="DeviceNumber" id="DeviceNumber_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="DeviceNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="consumbleForm.DeviceNumber"
							placeholder="Please enter the warehouse entry number" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Consumables name" required name="Name" id="Name_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="Name" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="consumbleForm.Name"
							placeholder="Please enter the category name" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="original price" required name="Price" id="Price_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="Price" class="form_li">
						<view style="width: 100%;">
							<view class="li_flex">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="number" v-model="consumbleForm.Price"
									placeholder="Please enter the original price" contentFontSize="32rpx"
									primaryColor="rgba(255, 255, 255, 0.5)" @input="filterValue($event,2)" />
								<view class="num_cli">
									<view class="cli_up" @click.stop="numUp()">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown()">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
									</view>
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="unit" name="Unit" id="Unit_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
					<view id="Unit" class="form_li">
						<uni-data-select v-model="consumbleForm.Unit" :localdata="localdata" width="100%"
							placeholder="Please select a unit" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(255, 255, 255, 0.2)" :isCustom="true" :isDark="true"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Category" name="ClassifiedId" id="ClassifiedId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ClassifiedId" class="form_li" @click.stop="choiceCategory">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="consumbleForm.ClassName"
							placeholder="Please select a consumable category" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" /> -->
						<view class="form_input"
							:class="{'placeholder_input':!consumbleForm.ClassName||consumbleForm.ClassName==''}">
							{{consumbleForm.ClassName?consumbleForm.ClassName:'Please select a consumable category'}}
						</view>
						<view class="form_sel_icon" :class="{'xuanzhuan_top':parIsFocus}">
							<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
						</view>
						<view class="select_con" v-if="parIsFocus">
							<leo-tree :data="classListData" @editItem="editItem" @node-click="nodeClick"
								:defaultProps="{ id: 'Id', label: 'Name', children: 'Children' }"
								:isInputSel="true"></leo-tree>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Consumable picture" required name="PhotoUrl" id="PhotoUrl_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="PhotoUrl" class="form_li">
						<view>
							<view class="image_con">
								<view class="image_li" @click="previewImg()" v-if="consumbleForm.PhotoUrl">
									<image class="image" :src="consumbleForm.PhotoUrl+'?wh=500x500'" mode="aspectFit">
									</image>
									<view class="del_icon" @click.stop="delImg()">
										<view class="icons_del t-icon-yichu"></view>
									</view>
								</view>
								<view class="image_li" @click.stop="uploadImg()" v-if="!consumbleForm.PhotoUrl">
									<view class="image_icon t-icon-shangchuantupian"></view>
								</view>
							</view>
							<view class="tips_text">
								Please upload a file with a size not exceeding 10MB in the format of png/jpg/jpeg
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="consumbleForm.Remark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" autoHeight :maxlength="255" />
					</view>
				</uni-forms-item>
				<button :style="{'opacity':isSubmit?0.6:1}" class="submit_button" @click="submitconsumbleForm"
					:disabled="isSubmit" :loading="isSubmit">
					Save
				</button>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		classTree,
	} from "@/api/partscls";
	import {
		addParts,
		delParts,
		editParts,
		partsInfo,
		generatePartsNumber
	} from "@/api/parts";
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				localdata: [],
				parIsFocus: false,
				topTitle: 'Create consumables',
				consumbleForm: {
					Id: undefined,
					DeviceNumber: "",
					Name: "",
					PhotoUrl: "",
					Remark: "",
					ClassifiedId: undefined,
					Unit: undefined,
					Price: 0
				},
				rules: {
					DeviceNumber: {
						rules: [{
							required: true,
							errorMessage: "Please enter the Consumables number"
						}]
					},
					Name: {
						rules: [{
							required: true,
							errorMessage: "Please enter the category name"
						}]
					},
					Price: {
						rules: [{
							required: true,
							errorMessage: "The original price cannot be empty"
						}]
					},
					//自定义校验器
					PhotoUrl: {
						rules: [{
							required: true,
							errorMessage: "Please select a category cover"
						}]
					},
					ClassifiedId: {
						rules: [{
							required: true,
							errorMessage: "Please select a consumable category"
						}]
					}
					// photoUrl: [{ required: true, trigger: "change", message: "请上传封面" }]
				}, //产品分类添加验证
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				classListData: [],
				status: 'loading',
				isSubmit: false
			};
		},
		async onLoad(options) {
			uni.showLoading({
				title: 'loading'
			})
			try {
				let partUnit = await this.$store.dispatch("data/dictList", 'parts_unit');
				// console.log("partUnit",partUnit);
				this.localdata = partUnit.map(it => {
					return {
						text: it.label,
						value: it.value
					}
				})
				// console.log('this.localdata单位',this.localdata);
				this.loadData()
				if (options.id) {
					let row = await partsInfo(options.id)
					this.consumbleForm = row.data;
					this.topTitle = "Modify consumables"
				} else {
					generatePartsNumber().then(rsp => {
						this.consumbleForm.DeviceNumber = rsp.data;
					})
				}
				uni.hideLoading()
			} catch (e) {
				//TODO handle the exception
				uni.hideLoading()
				this.setMsgTop(e)
			}
		},
		methods: {
			choiceCategory() {
				uni.hideKeyboard()
				this.parIsFocus = !this.parIsFocus
			},
			editItem(item) {
				// console.log("item编辑", item);
			},
			nodeClick(e) {
				console.log('点击的项目', e);
				this.consumbleForm.ClassifiedId = e.Id
				this.consumbleForm.ClassName = e.Name
				this.$forceUpdate()
				this.parIsFocus = false
			},
			loadData() {
				this.status = 'loading';
				classTree().then(response => {
					console.log("查询到的产品分类", response);

					this.classListData = response.data;
					this.status = 'noMore';
				}).catch(err => {
					this.setMsgTop(err)
				});
			},
			submitconsumbleForm() {
				//提交保存分类
				this.$refs["consumbleForm"].validate().then(valid => {
					let submitForm = JSON.parse(JSON.stringify(this.consumbleForm))
					if (valid) {
						this.isSubmit = true
						if (this.consumbleForm.Id) {
							editParts(submitForm)
								.then(rsp => {
									console.log("编辑后返回值", rsp);
									if (rsp.code == 0) {
										this.$refs.promptMsg.open('Modified successfully', 1500)
										setTimeout(() => {
											setPagesParam('loadList', 'load', 1)
										}, 1500)
									}
								})
								.catch(err => {
									this.setMsgTop(err)
									this.isSubmit = false
								});
						} else {
							addParts(submitForm)
								.then(rsp => {
									console.log("添加后返回值", rsp);
									if (rsp.code == 0) {
										this.$refs.promptMsg.open('Added successfully', 1500)
										setTimeout(() => {
											setPagesParam('loadList', 'load', 1)
										}, 1500)
									}
								})
								.catch(err => {
									this.setMsgTop(err)
									this.isSubmit = false
								});
						}
					}
				});
			},
			//照片相关函数
			previewImg() {
				let imgArr = []
				imgArr.push(this.consumbleForm.PhotoUrl)
				uni.previewImage({
					current: 1,
					urls: imgArr
				});
			},
			async delImg() {
				//删除图片
				this.$refs.promptMsg.loadingOpen('deleting...')
				try {
					let rsp2 = await delPhoto(this.consumbleForm.PhotoUrl)
					this.consumbleForm.PhotoUrl = ''
					this.$refs.promptMsg.loadingColse()
				} catch (err) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				}

			},
			uploadImg(imagesList) {
				//手动上传图片
				this.$nextTick(() => {
					uni.chooseImage({
						count: 1,
						sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
						success: async (res) => {
							this.$refs.promptMsg.loadingOpen('Uploading...')
							if (res.tempFilePaths.length == 0) {
								return;
							}
							for (let i = 0; i < res.tempFiles.length; i++) {
								if (this.maxSize > 0 && res.tempFiles[i].size > 10 * 1024 * 1024) {
									this.$refs.promptMsg.open(`Uploading images cannot exceed 10MB`,
										2000)
									return;
								}
								try {
									let paths = res.tempFilePaths[i];
									let rsp = await uploadPhoto(paths)
									console.log("rsp图片结果", rsp);
									this.consumbleForm.PhotoUrl = rsp
									this.$refs.promptMsg.loadingColse()
								} catch (e) {
									//TODO handle the exception
									this.$refs.promptMsg.loadingColse()
									// console.log(e);
									this.setMsgTop(e)
								}
							}
						}
					})
				})
			},
			//照片相关函数
			//排序值输入框
			filterValue(input, n) {
				this.$nextTick(() => {
					let value = this.consumbleForm.Price;
					if (n > 0) { //实现保留指定小数位
						value = value.replace(/[^\d.]/g, '');
						value = value.replace(/^\./g, '');
						value = value.replace('.', '$#$').replace(/\./g, '').replace('$#$', '.');
						if (n && Number(n) > 0) { //限制n位
							var d = new Array(Number(n)).fill(`\\d`).join('');
							var reg = new RegExp(`^(\\-)*(\\d+)\\.(${d}).*$`, 'ig');
							value = value.replace(reg, '$1$2.$3')
						}
						if (value && !value.includes('.')) {
							value = Number(value).toString() //去掉开头多个0
						}
						// this.$nextTick(() => {
						this.consumbleForm.Price = value
						// })
					} else { ////限制只允许输入数字
						value = value.replace(/\D+/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						this.consumbleForm.Price = value
					}
				})
			},
			numUp() {
				if (!this.disabled) {
					if (String(this.consumbleForm.Price).indexOf('.') > -1) {
						let arr = String(this.consumbleForm.Price).split('.')
						this.consumbleForm.Price = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.consumbleForm.Price = Number(this.consumbleForm.Price) + 1
					}
				}
			},
			numDown() {
				if (!this.disabled) {
					if (this.consumbleForm.Price > 0) {
						if (this.consumbleForm.Price - 1 <= 0) {
							this.consumbleForm.Price = 0
							return
						}
						if (String(this.consumbleForm.Price).indexOf('.') > -1) {
							let arr = String(this.consumbleForm.Price).split('.')
							this.consumbleForm.Price = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
						} else {
							this.consumbleForm.Price = Number(this.consumbleForm.Price) - 1
						}
					}
				}
			},
			//排序值输入框相关函数
		}
	}
</script>

<style lang="less">

</style>