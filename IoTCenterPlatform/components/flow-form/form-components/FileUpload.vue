<template>
	<view style="width: 100%;">
		<view v-if="disabled" style="width: 100%;">
			<view class="file_con">
				<view class="file_li" v-for="(item,inx) in showarr">
					<view class="file_icon t-icon-wenjian"></view>
					<view class="file_name">
						{{item.name}}
					</view>
				</view>
				<view class="file_li">
					<view class="file_icon t-icon-fujian"></view>
					<view class="file_name pal">
						Select file
					</view>
				</view>
			</view>
			<view class="tips_text">
				{{sizeTip}}
			</view>
		</view>
		<view v-else style="width: 100%;">
			<view class="file_con">
				<view class="file_li" v-for="(item,inx) in showarr">
					<view class="file_icon t-icon-wenjian"></view>
					<view class="file_name">
						{{item.name}}
					</view>
					<view class="del_icon" @click.stop="handleDeleteFile(inx,showarr)" v-if="!disabled">
						<view class="icons_del t-icon-yichu"></view>
					</view>
				</view>
				<view class="file_li" @click.stop="handleUploadClick('file',fileTypes)" v-if="!disabled">
					<view class="file_icon t-icon-fujian"></view>
					<view class="file_name pal">
						Select file
					</view>
				</view>
			</view>
			<view class="tips_text">
				{{sizeTip}}
			</view>
			<xe-upload ref="XeUpload" :options="uploadOptions"
				@callback="handleUploadCallback($event,showarr)"></xe-upload>
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
		name: "FileUpload",
		props: {
			placeholder: {
				type: String,
				default: "请选择附件",
			},
			value: {
				type: Array,
				default: () => {
					return [];
				},
			},
			maxSize: {
				type: Number,
				default: 10,
			},
			maxNumber: {
				type: Number,
				default: 5,
			},
			fileTypes: {
				type: Array,
				default: () => {
					return [];
				},
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		computed: {
			sizeTip() {
				if (this.fileTypes.length > 0) {
					let str=''
					this.fileTypes.map(row=>{
						str+='、'+row
					})
					str=str.substring(1)
					return ` | 只允许上传[${str}]格式的文件，且单个附件不超过${this.maxSize}MB`;
				}
				return this.maxSize > 0 ? ` | 单个附件不超过${this.maxSize}MB` : "";
			},
			showarr(){
				let arr=Object.assign([], this.value)
				return arr
			}
		},
		data() {
			return {
				// showarr: [],
				uploadOptions: { //上传文件参数
					// url: '', // 不传入上传地址则返回本地链接
					// headers: {
					// 	Authorization: getToken(),
					// },
				},
			};
		},
		// created() {
		// 	this.showarr = Object.assign([], this.value);
		// },
		methods: {
			handleUploadClick(type, fileType) {
				console.log("iiiiii", type, fileType);
				let fileArr = []
				fileType.map(row => {
					// #ifndef H5
					let str = ''
					if (row.indexOf('txt') > -1) {
						str = 'text/plain'
					}
					if (row.indexOf('ppt') > -1) {
						str = 'application/vnd.ms-powerpoint'
					}
					if (row.indexOf('doc') > -1) {
						str = 'application/msword'
					}
					if (row.indexOf('xls') > -1) {
						str = 'application/vnd.ms-excel'
					}
					if (row.indexOf('pdf') > -1) {
						str = 'application/pdf'
					}
					// #endif
					// #ifdef H5
					let str = '.' + row
					// #endif
					fileArr.push(str)
				})
				// 使用默认配置则不需要传入第二个参数
				// App、H5 文件拓展名过滤 { extension: ['.doc', '.docx'] } 或者 { extension: '.doc, .docx' }
				this.$refs.XeUpload.upload(type, {
					extension: fileArr
				});
			},
			handleUploadCallback(e, fileList) {
				console.log('UploadCallback', e);
				if(!this.value){
					this._value=[]
				}
				if (this.value == null) {
					this.value = [];
				}
				this.$nextTick(async ()=>{
					if (['choose', 'success'].includes(e.type)) {
						this.$refs.promptMsg.loadingOpen('Uploading...')
						// 根据接口返回修改对应的response相关的逻辑
						try{
							for (let i = 0; i < (e.data || []).length; i++) {
								let {
									response,
									tempFilePath,
									size,
									name,
									fileType
								} = e.data[i]
								if (size && size > Number(this.maxSize) * 1024 * 1024) {
									this.$refs.promptMsg.open(`Uploading images cannot exceed ${this.maxSize}MB`, 2000)
									return;
								}
								const res = response?.result || {};
								const tmpUrl = res.filePath ?? tempFilePath;
								const tmpName = res.fileName ?? name;
								let fileUpUrl = await uploadPhoto(tmpUrl)
								console.log("上传后", fileUpUrl);
								let tmpFiles = {
									url: fileUpUrl,
									name: tmpName,
								};
								console.log(tmpFiles, 'yyyyyyy上传的');
								fileList.push(tmpFiles);
								console.log('this.value',this.value,typeof this.value);
								this.value.push({
									url: fileUpUrl,
									name: tmpName,
								});
								this.$emit('input', this.value);
								console.log('this.value22221',this.value,typeof this.value);
							}
							this.$refs.promptMsg.loadingColse()
						}catch(e){
							//TODO handle the exception
							this.$refs.promptMsg.loadingColse()
							this.setMsgTop(e)
						}
						
					}
				})
			},
			async handleDeleteFile(inx, fileList) {
				//删除文件
				this.$refs.promptMsg.loadingOpen('deleting...')
				try {
					console.log("fileList", fileList[inx]);
					let rsp2 = await delPhoto(fileList[inx].url)
					fileList.splice(inx, 1)
					this.value.splice(inx, 1)
					this.$refs.promptMsg.loadingColse()
				} catch (err) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				}
			},
		},
	};
</script>

<style lang="less" scoped>
</style>