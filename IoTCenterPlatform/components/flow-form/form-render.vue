<template>
	<!--渲染表单-->
	<!-- <el-form ref="form" class="process-form" label-position="top" :rules="rules" :model="_value">
		<el-form-item v-if="item.name !== 'SpanLayout' && item.name !== 'Description'" :prop="item.id"
			:label="item.title" v-for="(item, index) in forms" :key="item.name + index">
			<form-design-render :ref="`sub-item_${item.id}`" v-model="_value[item.id]" :valueModel="value" mode="PC"
				:config="item" />
		</el-form-item>
		<form-design-render ref="span-layout" v-else v-model="_value" mode="PC" :valueModel="value" :config="item" />
	</el-form> -->
	<view>
		<uni-forms ref="form" :modelValue="_value" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con pages_form">
				<view v-for="(item, index) in forms" :key="item.name + index" v-if="forms&&forms.length>0">
					<uni-forms-item :required="item.props&&item.props.required"
						v-if="item.name !== 'SpanLayout' && item.name !== 'Description'" :label="item.title"
						:name="item.id" :id="'form_'+item.id" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" labelPosition="top" :isDis="item.props&&item.props.disabled"
						:isLastTop="index==forms.length-1">
						<view :id="'li_'+item.id" class="form_li">
							<form-design-render :ref="'sub-item_'+item.id" v-model="_value[item.id]" :valueModel="value"
								mode="PC" :config="item" :keyId="item.id" @setActiveItem='setActiveItem' />
						</view>
					</uni-forms-item>
					<view :id="'li_'+item.id" class="form_li" :class="{'dis_form_li':item.props&&item.props.disabled}" v-else
						:style="{'margin-top': item.name === 'Description'?'-20rpx':'0'}">
						<form-design-render :ref="'sub-item_'+item.id" v-model="_value" :valueModel="value"
							mode="PC" :config="item" :keyId="item.id" @setActiveItem="setActiveItem" />
					</view>
				</view>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import FormDesignRender from './form-design-render.vue'

	export default {
		name: "FormRender",
		components: {
			FormDesignRender
		},
		props: {
			forms: {
				type: Array,
				default: () => {
					return []
				}
			},
			value: {
				type: Object,
				default: () => {
					return {}
				}
			}
		},
		data() {
			return {
				rules: {},
				isFirst:true
			}
		},
		updated() {
			if(this.isFirst){
				this.loadFormConfig(this.forms)
				this.isFirst=false
			}
		},
		computed: {
			_value: {
				get() {
					return this.value
				},
				set(val) {
					this.$emit('input', val)
				}
			}
		},
		methods: {
			selectDept(val, acitveId) {
				//选择部门
				this.$refs[`sub-item_${acitveId}`][0].selectDept(val, acitveId)
			},
			selectEmplee(val, acitveId) {
				//选择人员
				this.$refs[`sub-item_${acitveId}`][0].selectEmplee(val, acitveId)
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs[`sub-item_${acitveId}`][0].selectDevice(val, acitveId)
			},
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			validate(call) {
				let success = true
				this.$refs.form.validate().then(async valid => {
					success = valid
					if (valid) {
						//校验成功再校验内部
						for (let i = 0; i < this.forms.length; i++) {
							if (this.forms[i].name === 'TableList') {
								let formRef = this.$refs[`sub-item_${this.forms[i].id}`]
								if (formRef && Array.isArray(formRef) && formRef.length > 0) {
									// formRef[0].validate().then(subValid => {
									// 	console.log(subValid,'subValid');
										
									// }).catch(er=>{
										
									// })
									try{
										let subValid=await formRef[0].validate()
										success = subValid
									}catch(er){
										//TODO handle the exception
										success=false
										console.log("er",er);
										if (er && er.length > 0) {
											let firstErr = er[0].key
											// #ifdef MP-WEIXIN
											const query = uni.createSelectorQuery().in(this);
											let str="#li_"+firstErr
											query.select(str).boundingClientRect(data => {
												uni.pageScrollTo({
													scrollTop: data.top - 200,
													// selector: firstErr,
													duration: 300
												});
										
											}).exec();
											// #endif
											// #ifndef MP-WEIXIN 
											uni.pageScrollTo({
												selector: '#form_'+firstErr,
												duration: 300
											});
											// #endif  
										}
									}
									if (!success) {
										break
									}
								}
							}
						}
					}
					call(success)
				}).catch(err=>{
					console.log("err",err);
					call(false)
					if (err && err.length > 0) {
						let firstErr = err[0].key
						// #ifdef MP-WEIXIN
						const query = uni.createSelectorQuery().in(this);
						let str="#li_"+firstErr
						query.select(str).boundingClientRect(data => {
							uni.pageScrollTo({
								scrollTop: data.top - 200,
								// selector: firstErr,
								duration: 300
							});
					
						}).exec();
						// #endif
						// #ifndef MP-WEIXIN 
						uni.pageScrollTo({
							selector: '#form_'+firstErr,
							duration: 300
						});
						// #endif  
					}
				});
			},
			loadFormConfig(forms) {
				forms.map(item => {
					if (item.name === 'SpanLayout') {
						this.loadFormConfig(item.props.items)
					} else {
						this.$set(this._value, item.id, this.value[item.id])
						// if(item.name === 'TableList'&&item.props&&item.props.rowLayout){
						// 	console.log(item,'itemitemitemitem');
						// 	this.loadFormConfig(item.props.columns)
						// }
						if (item.props.required) {
							this.$set(this.rules, item.id, {rules:[{
								type: item.valueType === 'Array' ? 'array' : undefined,
								required: true,
								errorMessage: `请填写${item.title}`,
								// trigger: 'blur'
							}]})
						}
					}
				})
				// console.log(this.rules,'this.rulesthis.rulesthis.rules');
			}
		}
	}
</script>

<style lang="less">
	.select_popup {
		position: fixed;
		left: 100%;
		bottom: 100%;
		z-index: 2;
	}
</style>