<template>
	<view @click="parIsFocus=false" style="min-height: 100vh;">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff">
		</top>
		<uni-forms ref="classForm" :modelValue="classForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="上级分类" name="parentId" id="parentId_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="parentId" class="form_li" @click.stop="choiceParentClass">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="classForm.parentName"
							placeholder="请选择上级分类" contentFontSize="32rpx" :clearable="false" />
						<view class="form_sel_icon" :class="{'xuanzhuan_top':parIsFocus}">
							<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
								iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
						</view>
						<view class="select_con" v-if="parIsFocus">
							<leoTree :data="classListData" @editItem="editItem" @node-click="nodeClick"
								:defaultProps="{ id: 'Id', label: 'Name', children: 'Children' }"
								:isInputSel="true"></leoTree>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="分类名称" required name="name" id="name_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="name" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="classForm.name"
							placeholder="请输入分类名称" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="排序值" required name="sort" id="sort_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="sort" class="form_li">
						<view style="width: 100%;">
							<view class="li_flex">
								<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="number" v-model="classForm.sort"
									placeholder="请输入排序值" contentFontSize="32rpx" @input="filterValue($event,0)" />
								<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
									<view class="cli_up" @click.stop="numUp()">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown()">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
								</view>
							</view>
							<view class="tips_text">
								排序值越小，排名越前
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="分类封面" required name="photoUrl" id="photoUrl_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="photoUrl" class="form_li">
						<view>
							<view class="image_con">
								<view class="image_li" @click="previewImg()" v-if="classForm.photoUrl">
									<image class="image" :src="classForm.photoUrl+'?wh=500x500'" mode="aspectFit">
									</image>
									<view class="del_icon" @click.stop="delImg()">
										<view class="icons_del t-icon-yichu"></view>
									</view>
								</view>
								<view class="image_li" @click.stop="uploadImg()" v-if="!classForm.photoUrl"
									style="background-color: rgba(248, 248, 248, 1);">
									<view class="image_icon t-icon-shangchuantupian"></view>
								</view>
							</view>
							<view class="tips_text">
								请上传大小不超过10MB，格式为png/jpg/jpeg
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="备注说明" name="remark" id="remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="classForm.remark"
							placeholder="请输入备注" contentFontSize="32rpx" autoHeight :maxlength="255" />
					</view>
				</uni-forms-item>
				<button :style="{'opacity':isSubmit?0.6:1}" class="submit_button" @click="submitClassForm"
					:disabled="isSubmit" :loading="isSubmit" v-if="!classForm.id">
					保存
				</button>
				<view class="btn_con" v-else>
					<button class="jump_button" @click="deleteClass" :disabled="isSubmit"
						:style="{'opacity':isSubmit?0.6:1}" :loading="isSubmit">
						删除
					</button>
					<button class="submit_button" @click="submitClassForm" :disabled="isSubmit"
						:style="{'opacity':isSubmit?0.6:1}" :loading="isSubmit">
						保存
					</button>
				</view>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
	</view>
</template>

<script>
	import {
		classTree,
		addClass,
		editClass,
		classInfo,
		removeClass
	} from "@/api/partscls";
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import leoTree from '@/pages_factory/cmp/leo-tree/components/leo-tree/leo-tree.vue'
	export default {
		components:{leoTree},
		data() {
			return {
				parIsFocus: false,
				topTitle: '创建耗材分类',
				classForm: {
					name: "",
					sort: 0,
					remark: "",
					parentId: 0,
					photoUrl: null
				},
				rules: {
					name: {
						rules: [{
							required: true,
							errorMessage: "请输入分类名称"
						}]
					},
					sort: {
						rules: [{
							required: true,
							errorMessage: "请输入排序值"
						}]
					},
					//自定义校验器
					photoUrl: {
						rules: [{
							required: true,
							errorMessage: "请上传分类封面"
						}]
					}
					// photoUrl: [{ required: true, trigger: "change", message: "请上传封面" }]
				}, //产品分类添加验证
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(248, 248, 248, 1)',
					borderColor: '#F8F8F8'
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
				this.loadData()
				if (options.id) {
					let row = await classInfo({
						id: options.id
					})
					this.classForm = {
						id: row.data.Id,
						name: row.data.Name,
						sort: row.data.Sort,
						remark: row.data.Remark,
						photoUrl: row.data.PhotoUrl,
						parentId: row.data.ParentId,
					}
					await this.getParentInfo(this.classForm.parentId)
				}
				uni.hideLoading()
			} catch (e) {
				//TODO handle the exception
				uni.hideLoading()
				this.setMsgTop(e)
			}
		},
		methods: {
			choiceParentClass() {
				uni.hideKeyboard()
				this.parIsFocus = !this.parIsFocus
			},
			async confirmDelete() {
				//确认删除动作
				try {
					this.isSubmit = true
					await removeClass({
						id: this.classForm.id
					});
					this.$refs.promptMsg.open('操作成功', 1500)
					setTimeout(() => {
						setPagesParam('loadData', 'load', 1)
					}, 1500)
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					this.isSubmit = false
				}
			},
			deleteClass() {
				try {
					this.$refs.promptMsg.noticeOpen(
						'你确定要删除分类 ' + this.classForm.name +
						'吗? (此操作不可逆)?'
					)
				} catch (e) {
					console.log(e);
				}
			},
			async getParentInfo(id) {
				try {
					if (id) {
						let res = await classInfo({
							id: id
						})
						this.classForm.parentName = res.data.Name
						this.$forceUpdate()
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			editItem(item) {
				// console.log("item编辑", item);
			},
			nodeClick(e) {
				this.classForm.parentId = e.Id
				this.classForm.parentName = e.Name
				this.$forceUpdate()
				this.parIsFocus = false
			},
			choiceParentCate() {

			},
			loadData() {
				this.status = 'loading';
				classTree().then(response => {

					this.classListData = response.data;
					this.status = 'noMore';
				}).catch(err => {
					this.setMsgTop(err)
				});
			},
			submitClassForm() {
				//提交保存分类
				this.$refs["classForm"].validate().then(valid => {
					let submitForm = JSON.parse(JSON.stringify(this.classForm))
					delete submitForm.parentName
					if (this.classForm.parentId == 0 || !this.classForm.parentId) {
						submitForm.parentId = ''
					}
					if (valid) {
						this.isSubmit = true
						if (this.classForm.id) {
							editClass(submitForm)
								.then(rsp => {
									if (rsp.code == 0) {
										this.$refs.promptMsg.open('更新成功', 1500)
										setTimeout(() => {
											setPagesParam('loadData', 'load', 1)
										}, 1500)
									}
								})
								.catch(err => {
									this.setMsgTop(err)
									this.isSubmit = false
								});
						} else {
							addClass(submitForm)
								.then(rsp => {
									if (rsp.code == 0) {
										this.$refs.promptMsg.open('添加成功', 1500)
										setTimeout(() => {
											setPagesParam('loadData', 'load', 1)
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
				imgArr.push(this.classForm.photoUrl)
				uni.previewImage({
					current: 1,
					urls: imgArr
				});
			},
			async delImg() {
				//删除图片
				this.$refs.promptMsg.loadingOpen('删除中...')
				try {
					let rsp2 = await delPhoto(this.classForm.photoUrl)
					this.classForm.photoUrl = ''
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
							this.$refs.promptMsg.loadingOpen('上传中...')
							if (res.tempFilePaths.length == 0) {
								return;
							}
							for (let i = 0; i < res.tempFiles.length; i++) {
								if (this.maxSize > 0 && res.tempFiles[i].size > 10 * 1024 * 1024) {
									this.$refs.promptMsg.open(`上传图片不可以超过10MB`,
										2000)
									return;
								}
								try {
									let paths = res.tempFilePaths[i];
									let rsp = await uploadPhoto(paths)
									this.classForm.photoUrl = rsp
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
					let value = this.classForm.sort;
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
						this.classForm.sort = value
						// })
					} else { ////限制只允许输入数字
						value = value.replace(/\D+/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						this.classForm.sort = value
					}
				})
			},
			numUp() {
				if (!this.disabled) {
					if (String(this.classForm.sort).indexOf('.') > -1) {
						let arr = String(this.classForm.sort).split('.')
						this.classForm.sort = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.classForm.sort = Number(this.classForm.sort) + 1
					}
				}
			},
			numDown() {
				if (!this.disabled) {
					if (this.classForm.sort > 0) {
						if (this.classForm.sort - 1 <= 0) {
							this.classForm.sort = 0
							return
						}
						if (String(this.classForm.sort).indexOf('.') > -1) {
							let arr = String(this.classForm.sort).split('.')
							this.classForm.sort = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
						} else {
							this.classForm.sort = Number(this.classForm.sort) - 1
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