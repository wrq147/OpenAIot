<template>
	<view style="width: 100%;">
		<view v-if="disabled" style="width: 100%;">
			<view class="image_con">
				<view class="image_li" v-for="(item,inx) in previewArr" @click="previewImg(inx,previewArr)">
					<image class="image" :src="item+'?wh=500x500'" mode="aspectFit"></image>
				</view>
				<view class="image_li" v-if="!previewArr||previewArr.length==0">
					<view class="image_icon t-icon-shangchuantupian"></view>
				</view>
			</view>
			<view class="tips_text">
				{{ placeholder }} {{ sizeTip }}
			</view>
		</view>
		<view v-else style="width: 100%;">
			<view class="image_con">
				<view class="image_li" v-for="(item,inx) in previewArr" @click="previewImg(inx,previewArr)">
					<image class="image" :src="item+'?wh=500x500'" mode="aspectFit"></image>
					<view class="del_icon" @click.stop="delImg(inx,previewArr)" v-if="!disabled">
						<view class="icons_del t-icon-yichu"></view>
					</view>
				</view>
				<view class="image_li" @click.stop="uploadImg(previewArr,maxSize)" v-if="!disabled">
					<view class="image_icon t-icon-shangchuantupian"></view>
				</view>
			</view>
			<view class="tips_text">
				{{ placeholder }} {{ sizeTip }}
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	export default {
		mixins: [componentMinxins],
		name: "ImageUpload",
		components: {},
		props: {
			value: {
				type: Array,
				default() {
					return [];
				},
			},
			placeholder: {
				type: String,
				default: "请选择图片",
			},
			maxSize: {
				type: Number,
				default: 10,
			},
			maxNumber: {
				type: Number,
				default: 5,
			},
			enableZip: {
				type: Boolean,
				default: true,
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		computed: {
			sizeTip() {
				return this.maxSize > 0 ? `| 每张图不超过${this.maxSize}MB` : "";
			},
			showarr(){
				let arr=Object.assign([], this.value)
				return arr
			},
			previewArr(){
				let arr=this.showarr.map((x) => x.url)
				return arr
			}
		},
		data() {
			return {
				// showarr: [],
				// previewArr: [],
			};
		},
		beforeMount() {
			// console.log(this.value,'this.valuewwww');
			if(!this.value){
				this._value=[]
			}
		},
		// mounted() {
		// 	this.$nextTick(()=>{
				
		// 		this.showarr = Object.assign([], this.value);
		// 		this.previewArr = this.showarr.map((x) => x.url);
		// 		console.log(this.value,this.previewArr,'this.previewArr');
		// 	})
		// },
		methods: {
			previewImg(inx, imgArr) {
				uni.previewImage({
					current: inx,
					urls: imgArr
				});
			},
			async delImg(inx, imagesList) {
				//删除图片
				this.$refs.promptMsg.loadingOpen('deleting...')
				try {
					let rsp2 = await delPhoto(imagesList[inx])
					imagesList.splice(inx, 1)
					this.value.splice(inx, 1)
					this.$refs.promptMsg.loadingColse()
				} catch (err) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				}

			},
			uploadImg(imagesList) {
				//手动上传图片
				this.$nextTick(()=>{
					let need_num = Number(this.maxNumber) - Number(this.value.length)
					if (need_num <= 0) {
						need_num = 0
						this.$refs.promptMsg.open(`Only a maximum of ${this.maxNumber} images can be uploaded`, 2000)
						return
					}
					uni.chooseImage({
						count: need_num,
						sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
						success: async (res) => {
							if (this.value == null) {
								this.value = [];
							}
							this.$refs.promptMsg.loadingOpen('Uploading...')
							if (res.tempFilePaths.length == 0) {
								return;
							}
							for (let i = 0; i < res.tempFiles.length; i++) {
								if (this.maxSize > 0 && res.tempFiles[i].size > Number(this.maxSize) * 1024 * 1024) {
									this.$refs.promptMsg.open(`Uploading images cannot exceed ${this.maxSize}MB`,
										2000)
									return;
								}
								try {
									let paths = res.tempFilePaths[i];
									let rsp = await uploadPhoto(paths)
									console.log("rsp图片结果", rsp);
									imagesList.push(rsp)
									this.value.push({
										name: res.tempFiles[i].name,
										url: rsp
									});
									this.$emit('input', this.value);
									console.log("图片列表", imagesList);
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
		},
	};
</script>

<style lang="less" scoped>
	.design {
		i {
			padding: 10px;
			font-size: xx-large;
			background: white;
			border: 1px dashed #8c8c8c;
		}
	}

	/deep/ .el-upload--picture-card {
		width: 80px;
		height: 80px;
		line-height: 87px;
	}

	/deep/ .el-upload-list__item {
		width: 80px;
		height: 80px;

		.el-upload-list__item-actions {
			&>span+span {
				margin: 1px;
			}
		}
	}
</style>